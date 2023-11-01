using System.Diagnostics;
using Backworker.Database;
using Microsoft.Extensions.Logging;

namespace Backworker.Runner;

internal class BackworkerManager
    {
        private readonly IDbManager _dbManager;
        private readonly ILogger<BackworkerManager> _logger;
        private readonly IBackworkerTaskFactory _backworkerTaskFactory;
        private bool _stop;

        public BackworkerManager(
            IDbManager dbManager,
            ILogger<BackworkerManager> logger,
            IBackworkerTaskFactory backworkerTaskFactory)
        {
            _dbManager = dbManager;
            _logger = logger;
            _backworkerTaskFactory = backworkerTaskFactory;
        }

        public Task StartAsync()
        {
            return Task.Run(async () => {

                _logger.LogInformation("Backworker start");
                _stop = false;

                Stopwatch stopwatch = Stopwatch.StartNew();
                while (!_stop)
                {
                    try
                    {
                        stopwatch.Restart();

                        await RunTasksAsync();

                        stopwatch.Stop();

                        var sleepTime = 1000 - (int)stopwatch.ElapsedMilliseconds;
                        if (sleepTime > 0 && !_stop)
                            await Task.Delay(sleepTime);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("unknown error", e);
                    }
                }

                _logger.LogInformation("Backworker stop");
            });
        }

        public Task StopAsync()
        {
            _stop = true;
            return Task.CompletedTask;
        }

        private async Task RunTasksAsync()
        {
            Model.BackworkerTask runTask = await _dbManager.GetAndLockStartBackworkerTaskAsync();

            if (runTask == null)
                return;

            IBackworkerTaskAct act = _backworkerTaskFactory.GetAct(runTask.Type);

            try
            {
                runTask.Start();
                await _dbManager.SaveLogAsync(runTask);

                await act.RunAsync(runTask.MagicString);
                runTask.Stop();
            }
            catch(Exception e)
            {
                _logger.LogError($"Uncknow error run backwork task {runTask}", e);
            }

            await _dbManager.SaveLogAsync(runTask);
            await _dbManager.UnlockBackworkerTask(runTask);

        }
    }
