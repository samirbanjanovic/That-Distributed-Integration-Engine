using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TDIE.ComponentHost._Core.Interfaces;
using TDIE.ComponentHost.Classes;
using TDIE.ComponentHost.Models;
using TDIE.Utilities.Mappers.Core;

namespace TDIE.ComponentHost
{
    public class ComponentManagerInteractionService
        : IStateManagerBackgroundService
    {

        //incremented/decremented using Interlocked calls
        //we're using this as a boolean to identify if a 
        //configuration is being processed
        private int _hasServiceConfigurationRequest = 0;

        private readonly IObjectMapperService _objectMapperService;
        private readonly ILogger<ComponentManagerInteractionService> _logger;
        private readonly ComponentManager _stateManager;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        private ServiceConfiguration _serviceConfiguration;
        
        public ComponentManagerInteractionService(ComponentManager stateManager, IObjectMapperService objectMapperService,ILogger<ComponentManagerInteractionService> logger)
        {
            _stateManager = stateManager;
            _objectMapperService = objectMapperService;
            _logger = logger;                        
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IRequestResponse GetStateManagerDetails()
        {
            throw new NotImplementedException();
        }

        public IRequestResponse RequestComponentStart()
        {
            throw new NotImplementedException();
        }

        public IRequestResponse RequestComponentStop()
        {
            throw new NotImplementedException();
        }

        public IRequestResponse RequestMessagePublisherStart()
        {
            throw new NotImplementedException();
        }

        public IRequestResponse RequestMessagePublisherStop()
        {
            throw new NotImplementedException();
        }

        public IRequestResponse SubmitConfiguration(ServiceConfiguration serviceConfiguration)
        {
            //if there hasn't been a request to submit a new service configuration 
            //we will proceed with workflow
            if(Interlocked.CompareExchange(ref _hasServiceConfigurationRequest, 0, 0) == 0)
            {
                Interlocked.Increment(ref _hasServiceConfigurationRequest);
                _serviceConfiguration = serviceConfiguration;

                Task.Run(async () => await ProcessConfigurationRequest(), _cancellationTokenSource.Token);
            }

            return null;
        }

        private async Task ProcessConfigurationRequest()
        {
            var configurationModel = _objectMapperService.Map<ServiceConfigurationModel, ServiceConfiguration>(_serviceConfiguration);

            await _stateManager.Configure(configurationModel);

            Interlocked.Decrement(ref _hasServiceConfigurationRequest);
        }
    }
}
