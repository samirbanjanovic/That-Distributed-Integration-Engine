using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TDIE.Core;
using TDIE.Extensions.Logging;
using Quartz;
using Quartz.Impl;

namespace TDIE.Components.QuartzScheduler
{
    public class QuartzSchedulerComponent
        : IComponent
    {
        //used to keep track of the state for the 
        //underlying quartz trigger + job
        private enum TriggerState
        {
            Initialized = 0,
            Started = 1,
            Paused = 2
        }

        private static readonly IScheduler _quartzGlobalScheduler = GenerateGlobalQuartzScheduler();

        //Creates an instance of IScheduler to be used by every
        //instance of QuartzTaskNotifier
        private static IScheduler GenerateGlobalQuartzScheduler()
        {
            var quartzScheduler = new StdSchedulerFactory().GetScheduler()
                                                     .ConfigureAwait(false)
                                                     .GetAwaiter()
                                                     .GetResult();

            //start scheduler as soon as it's created
            //NOTE: a better solution would be to have a 
            //concurrent static state tracker, that
            //monitors all notifiers, first call to start 
            //a notifier checks the state of the scheduler
            //if it isn't started start it
            quartzScheduler.Start()
                           .ConfigureAwait(false)
                           .GetAwaiter()
                           .GetResult();



            return quartzScheduler;
        }

        //if the notifier enters a state that requires recovery we 
        //want to evaluate if the reset state is a recoverable one
        //this array contains list of recoverable states
        private static readonly ObjectState[] _validResetToStates = new ObjectState[]
        {
            ObjectState.Initialized,
            ObjectState.Started,
            ObjectState.Stopped
        };

        // instance used to publish event to a destination
        private readonly IMessagePublisher _messagePublisher;

        //quartz trigger responsible for
        //executing job - which uses IEventPublisher
        //to submit jobs
        private ITrigger _trigger;
        //job used by quartz to publish events
        //via the IEventPublisher
        private IJobDetail _jobDetail;

        // schedule used by quartz trigger to execute publish job
        private string _cronSchedule;

        // the state of the underlying QuartzTrigger        
        private TriggerState _triggerState;

        //if true then a job can fire even when an instance of it
        //is already running. If false a job will not run until
        //the previous instance has completed
        private bool _isReentrant = false;

        //context log for a given instance of QuartzSchedulerComponent
        private readonly ILogger<QuartzSchedulerComponent> _logger;

        /// <summary>
        /// Publishes events based on a configued schedule. Used to schedule task
        /// for repetative execution.
        /// </summary>
        public QuartzSchedulerComponent(IComponentSettings settings, IMessagePublisher messagePublisher, ILogger<QuartzSchedulerComponent> logger)
        {
            Settings = settings;
            _logger = logger;
            
            _messagePublisher = messagePublisher;            

            ConfigureIsReentrant();
            ConfigureCronSchedule();
        }

        //configuration for instance of notfier
        public IComponentSettings Settings { get; private set; }

        public Guid InstanceId { get; } = Guid.NewGuid();
        public ObjectState State { get; private set; }
        public string Name => Settings.Name ?? nameof(QuartzSchedulerComponent);

        public void Dispose()
        {
            using (_logger.ExtendLogScope(this))
            {
                DestroyCurrentTriggerAndJobInstances()
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (_logger.ExtendLogScope(this))
            {
                if (_trigger == null || _jobDetail == null)
                {
                    InitializeJobDetailAndTrigger();
                }

                if (State == ObjectState.Stopped || State == ObjectState.Initialized)
                {
                    if (_triggerState == TriggerState.Initialized)
                    {
                        await _quartzGlobalScheduler.ScheduleJob(_jobDetail, _trigger).ConfigureAwait(false);

                        SetStateAsStartedAndLog();
                    }
                    else if (_triggerState == TriggerState.Paused)
                    {
                        await _quartzGlobalScheduler.ResumeTrigger(_trigger.Key).ConfigureAwait(false);

                        SetStateAsStartedAndLog();
                    }
                }
                else
                {
                    _logger.LogWarning("{Message}", $"Cannot start Quartz trigger when in {State} state");
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using (_logger.ExtendLogScope(this))
            {
                if (State == ObjectState.Started)
                {
                    await _quartzGlobalScheduler.PauseTrigger(_trigger.Key).ConfigureAwait(false);
                    State = ObjectState.Stopped;
                    _triggerState = TriggerState.Paused;
                    _logger.LogInformation("{Message}", $"Quartz trigger stopped");
                }
                else
                {
                    _logger.LogWarning("{Message}", $"Cannot stop Quartz trigger when in {State} state");
                }
            }
        }

        private void SetStateAsStartedAndLog()
        {
            _logger.LogInformation("{Message}", "Quartz trigger running");

            State = ObjectState.Started;
            _triggerState = TriggerState.Started;
        }

        //initialize corresponding quartz job and trigger
        private void InitializeJobDetailAndTrigger()
        {
            try
            {
                _logger.LogInformation("{Message}", "Preparing to initialize for staging");

                _jobDetail = CreateInitialJobBuilder()
                                    .WithIdentity($"{Name}_job")
                                    .StoreDurably(false)
                                    .UsingJobData(new JobDataMap(BuildKeyValueQuartzDataMap()))
                                    .Build();

                _trigger = TriggerBuilder.Create()
                                    .WithIdentity(Name)
                                    .WithCronSchedule(_cronSchedule)
                                    .Build();

                _triggerState = TriggerState.Initialized;
                _logger.LogInformation("{Message}", "Successfully intialized Quartz trigger and job");

                State = ObjectState.Initialized;
            }
            catch (Exception exception)
            {
                State = ObjectState.Errored;
                _logger.LogError(exception, "{Message}", $"Failed to initialize");
            }
        }

        //build JobDataMap that is used by QuartzJob to 
        //execute work. Values include a logger, name,
        //event publisher that will be used to send the event
        //and the notifiers ConfigurationItems
        private IDictionary<string, object> BuildKeyValueQuartzDataMap()
        {
            //datamap to be sent to quartz job, this key value pair is
            //transmitted to the even publisher 
            return new Dictionary<string, object>()
            {
                {"logger", _logger },
                {"name", Name },
                {"eventPublisher", _messagePublisher },
                {"properties", Settings.Properties }
            };
        }

        //If reentrant is true - such that a new job can start before the 
        //previous instance finished - then a concurrent job is 
        //used, otherwise we use jobs that have attributes:
        //[DisallowConcurrentExecution]
        //[PersistJobDataAfterExecution]
        private JobBuilder CreateInitialJobBuilder()
        {
            if (_isReentrant)
            {
                return JobBuilder.CreateForAsync<QuartzPublishEventJobConcurrent>();
            }
            else
            {
                return JobBuilder.CreateForAsync<QuartzPublishEventJob>();
            }
        }

        //stop, unschedule, and null trigger and job 
        //inside of quartz
        private async Task DestroyCurrentTriggerAndJobInstances()
        {
            try
            {
                await StopAsync(CancellationToken.None).ConfigureAwait(false);
                await _quartzGlobalScheduler.UnscheduleJob(_trigger.Key).ConfigureAwait(false);


                _trigger = null;
                _jobDetail = null;

                State = ObjectState.Destroyed;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{Message}", "Failed to dispose of current instance");
                State = ObjectState.Errored;
            }
        }

        //3 methods below read the configuration items 
        //to extract desired values for intializing 
        private void ConfigureCronSchedule()
        {
            //make these fields required - throw exception if not present
            if (!Settings.Properties.TryGetValue("cronSchedule", out _cronSchedule))
            {
                //_logger.LogError("{Message}", "Invalid configuraiton items - cronSchedule not supplied");
                throw new ArgumentNullException("\"cronSchedule\" is a required property");
            }

        }

        private void ConfigureIsReentrant()
        {
            //make these fields required - throw exception if not present
            if (Settings.Properties.TryGetValue("isReentrant", out string isReentrant))
            {
                bool.TryParse(isReentrant, out _isReentrant);
            }
        }

    }
}
