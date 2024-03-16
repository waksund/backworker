using System.Diagnostics;
using Backworker.Database;
using Microsoft.Extensions.Logging;

namespace Backworker.Runner;

internal class BackworkerManager(IDbManager dbManager,
    ILogger<BackworkerManager> logger,
    IBackworkerTaskFactory backworkerTaskFactory)
{
    private bool _stop;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Backworker start");
            _stop = false;

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!_stop && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    stopwatch.Restart();

                    await RunTasksAsync();

                    stopwatch.Stop();

                    var sleepTime = 500 - (int)stopwatch.ElapsedMilliseconds;
                    if (sleepTime > 0 && !_stop)
                        await Task.Delay(sleepTime);
                }
                catch (Exception e)
                {
                    logger.LogError("unknown error", e);
                }
            }

            logger.LogInformation("Backworker stop");
        }

        public Task StopAsync()
        {
            _stop = true;
            return Task.CompletedTask;
        }

        private async Task RunTasksAsync()
        {
            Model.BackworkerTask runTask = await dbManager.GetAndLockStartBackworkerTaskAsync();

            if (runTask == null)
                return;

            IBackworkerTaskAct? task = backworkerTaskFactory.GetTask(runTask.Type);

            try
            {
                runTask.Start();
                await dbManager.SaveLogAsync(runTask);

                await task.RunAsync(runTask.MagicString);
                runTask.Stop();
            }
            catch(Exception e)
            {
                logger.LogError($"Uncknow error run backwork task {runTask}", e);
            }

            await dbManager.SaveLogAsync(runTask);
            await dbManager.UnlockBackworkerTask(runTask);

        }
    }
