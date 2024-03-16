using Backworker.Database;
using Microsoft.Extensions.Hosting;
namespace Backworker.Runner;

internal class BackworkerHostedService(
        BackworkerManager backworkerManager,
        IBackworkerMigration backworkerMigration)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await backworkerMigration.MigrateUpAsync();
        await backworkerManager.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        backworkerManager.StopAsync();
        return Task.CompletedTask;
    }
}