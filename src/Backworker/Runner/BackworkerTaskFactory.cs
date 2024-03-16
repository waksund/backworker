using Microsoft.Extensions.DependencyInjection;

namespace Backworker.Runner;

public class BackworkerTaskFactory : IBackworkerTaskFactory
{
    private readonly IServiceProvider _serviceProvider;

    public BackworkerTaskFactory(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IBackworkerTaskAct? GetTask(int type)
    {
        return _serviceProvider.GetKeyedService<IBackworkerTaskAct>(type);
    }
}