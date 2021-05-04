using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventsAPI.Services
{
    public class BackgroundRegistrationWorker : BackgroundService
    {
        private readonly ILogger<BackgroundRegistrationWorker> _logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("doing some stuff");
                await Task.Delay(1000);
            }
        }
    }
}
