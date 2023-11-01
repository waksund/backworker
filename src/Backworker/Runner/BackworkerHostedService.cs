using Microsoft.Extensions.Hosting;
namespace Backworker.Runner;

internal class BackworkerHostedService : IHostedService
{
    private readonly BackworkerManager _backworkerManager;

    public BackworkerHostedService(
        BackworkerManager backworkerManager)
    {
        _backworkerManager = backworkerManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _backworkerManager.StartAsync();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _backworkerManager.StopAsync();
        return Task.CompletedTask;
    }
}