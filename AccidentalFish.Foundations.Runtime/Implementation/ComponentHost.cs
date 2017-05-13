using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AccidentalFish.ApplicationSupport.Resources.Abstractions.Runtime.Implementation
{
    internal class ComponentHost : IComponentHost
    {
        private readonly IComponentHostRestartHandler _componentHostRestartHandler;
        public const string FullyQualifiedName = "com.accidentalfish.application-support.component-host";
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger<ComponentHost> _logger;

        public ComponentHost(IComponentHostRestartHandler componentHostRestartHandler, ILogger<ComponentHost> logger)
        {
            _componentHostRestartHandler = componentHostRestartHandler;
            _logger = logger;
        }

        public Action<Exception, int> CustomErrorHandler { get; set; }

        public async Task<IEnumerable<Task>> StartAsync(IComponentHostConfigurationProvider configurationProvider, CancellationTokenSource cancellationTokenSource)
        {
            IEnumerable<ComponentConfiguration> componentConfigurations = await configurationProvider.GetConfigurationAsync();
            _cancellationTokenSource = cancellationTokenSource;
            List<Task> tasks = new List<Task>();
            foreach (ComponentConfiguration componentConfiguration in componentConfigurations)
            {
                _logger?.LogTrace(
                    "Starting {INSTANCES} instances of {NAME}", componentConfiguration.Instances, componentConfiguration.Name);
                for (int instance = 0; instance < componentConfiguration.Instances; instance++)
                {
                    tasks.Add(StartTask(componentConfiguration.Name, componentConfiguration.Factory, componentConfiguration.RestartEvaluator));
                }
            }
            return tasks;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private Task StartTask(string name, Func<IHostableComponent> factory, Func<Exception, int, bool> restartEvaluator)
        {
            return Task.Run(async () =>
            {
                int retryCount = 0;
                bool shouldRetry = true;
                while (shouldRetry)
                {
                    try
                    {
                        await Task.Run(async () =>
                        {
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                _logger?.LogTrace("Creating component {NAME} with supplied factory", name);
                            }
                            else
                            {
                                _logger?.LogTrace("Creating unnamed component with supplied factory");
                            }
                            var component = factory();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                _logger?.LogTrace("Starting hostable component {NAME}", name);
                            }
                            else
                            {
                                _logger?.LogTrace($"Starting unnamed hostable component with supplied factory");
                            }
                                                    
                            await component.StartAsync(_cancellationTokenSource.Token);
                            shouldRetry = false; // normal exit
                            _logger?.LogTrace("Hostable component {NAME} is exiting", name);
                        }, _cancellationTokenSource.Token);
                        shouldRetry = false;
                    }
                    catch (Exception ex)
                    {
                        CustomErrorHandler?.Invoke(ex, retryCount);

                        retryCount++;
                        if (restartEvaluator != null)
                        {
                            shouldRetry = restartEvaluator(ex, retryCount);
                        }
                        else
                        {
                            shouldRetry = await _componentHostRestartHandler.HandleRestart(ex, name, retryCount, _logger);
                        }
                        if (shouldRetry)
                        {
                            _logger?.LogTrace("Restarting {RETRYCOUNT} for component {NAME} after exception {EX}", retryCount, name, ex);
                            AggregateException exception = ex as AggregateException;
                            if (exception != null)
                            {
                                foreach (Exception innerException in exception.InnerExceptions)
                                {
                                    _logger?.LogError("Aggregate exception {EX} for component {NAME} on retry {RETRYCOUNT}", innerException, name, retryCount);
                                }
                            }
                        }
                        else
                        {
                            AggregateException exception = ex as AggregateException;
                            if (exception != null)
                            {
                                foreach (Exception innerException in exception.InnerExceptions)
                                {
                                    _logger?.LogError("Component failure {EX} on {RETRYCOUNT} for component {NAME}", innerException, retryCount, name);
                                }
                            }
                            else
                            {
                                _logger?.LogError("Component failure {EX} on {RETRYCOUNT} for component {NAME}", ex, retryCount, name);
                            }
                        }
                    }
                }
                
            }, _cancellationTokenSource.Token);
        }
    }
}
