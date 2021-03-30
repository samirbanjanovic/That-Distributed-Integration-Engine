using System.Threading.Tasks;
using Quartz;

namespace OnTrac.Integration.Components.QuartzScheduler
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    internal sealed class QuartzPublishEventJob
        : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await QuartzJobWorker.DoWork(context.JobDetail.JobDataMap);
        }

    }



}