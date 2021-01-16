using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Yvtu.Core.Entities;
using Yvtu.Infra.Data;
using Yvtu.Infra.Data.Interfaces;

namespace Yvtu.BgService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAppDbContext db;
        private Queue<AppBackgroundService> appBuffer;
        private string fileLocation;

        public Worker(ILogger<Worker> logger, IAppDbContext db)
        {
            _logger = logger;
            this.db = db;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (appBuffer != null && appBuffer.Count > 0)
                    {
                        var obj = appBuffer.Dequeue();
                         _logger.LogInformation("before Recharge Collection Called", appBuffer.Count);
                            await new ExportRechargeCollections(db, obj, fileLocation).ExportAsync();
                            _logger.LogInformation("after Recharge Collection Called", appBuffer.Count);
                        
                    }
                    else
                    {
                        appBuffer = await new AppBackgroundServiceRepo(db).GetBackgroundServicesAsync(" WHERE status = 'pending' and sysdate > active_time ", null);
                        if (appBuffer == null)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                        _logger.LogInformation("Fill Buffer {0}", appBuffer?.Count);
                    }
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception exp)
                {
                    _logger.LogError(exp.GetType().Name + " - " + exp.Message);
                }
            }
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            fileLocation = new AppGlobalSettingsRepo(db).GetSingle("BgServiceFileLocation").SettingValue;
            appBuffer = new Queue<AppBackgroundService>();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
