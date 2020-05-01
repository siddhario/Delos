using Delos.Contexts;
using Delos.Helpers;
using Delos.Model;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Delos.Helpers.Helper;

namespace Delos.Services
{
    public class ImportHostService : IHostedService, IDisposable
    {
        private Timer _timer;
        private DateTime _startOfSchedule;
        private IImportService _serviceModel;
        private DelosDbContext _dbContext;
        private bool _started;

        public ImportHostService(IImportService serviceModel,DelosDbContext dbContext)
        {
            this._serviceModel = serviceModel;
            this._dbContext = dbContext;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            if (_serviceModel.Config.StartImmediately == true)
                Run();
           
            _startOfSchedule = Helper.NextRunOn(_serviceModel.Config);
            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero,
               TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private async Task Run()
        {
            try
            {
                var artikli = await _serviceModel.SyncAsync();
                _serviceModel.UdpateDb(this._dbContext, artikli);
            }
            catch (Exception ex)
            {
                Helpers.Helper.LogException(ex);
            }
        }

        private async void DoWorkAsync(object state)
        {
            if (!_started)
            {
                if (DateTime.Now >= _startOfSchedule)
                {
                    _started = true;
                    switch (_serviceModel.Config.RecurringMode)
                    {
                        case RecurringModeEnum.DAILY:
                            {
                                _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromDays(_serviceModel.Config.RecurringInterval));
                                break;
                            }
                        case RecurringModeEnum.HOURLY:
                            {
                                _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromHours(_serviceModel.Config.RecurringInterval));
                                break;
                            }
                        case RecurringModeEnum.MINUTELY:
                            {
                                _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(_serviceModel.Config.RecurringInterval));
                                break;
                            }
                        case RecurringModeEnum.WEEKLY:
                            {
                                _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromDays(7 * _serviceModel.Config.RecurringInterval));
                                break;
                            }
                    }
                }
            }
            else
                await Run();
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
