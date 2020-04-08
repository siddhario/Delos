using Delos.Contexts;
using Delos.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delos.Services
{
    public class HostService : IHostedService, IDisposable
    {
        private Timer _timer;
        private ISyncService _serviceModel;
        private DelosDbContext _dbContext;

        public HostService(ISyncService serviceModel,DelosDbContext dbContext)
        {
            this._serviceModel = serviceModel;
            this._dbContext = dbContext;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(this._serviceModel.IntervalInMinutes));

            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            try
            {
                var artikli = await _serviceModel.SyncAsync();
                _serviceModel.UdpateDb(this._dbContext,artikli);
            }
            catch (Exception ex)
            {
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
