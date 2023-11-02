using Backworker.Database;
using Microsoft.Extensions.Hosting;
namespace Backworker.Runner;

internal class BackworkerHostedService : IHostedService
{
    private readonly BackworkerManager _backworkerManager;
    private readonly IBackworkerMigration _backworkerMigration;

    public BackworkerHostedService(
        BackworkerManager backworkerManager,
        IBackworkerMigration backworkerMigration)
    {
        _backworkerManager = backworkerManager;
        _backworkerMigration = backworkerMigration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _backworkerMigration.MigrateUpAsync();
        await _backworkerManager.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _backworkerManager.StopAsync();
        return Task.CompletedTask;
    }
}