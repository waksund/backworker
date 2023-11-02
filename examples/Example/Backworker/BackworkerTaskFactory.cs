using Backworker.Runner;
using Example.Backworker.Tasks;

namespace Example.Backworker;

public class BackworkerTaskFactory : IBackworkerTaskFactory
{
    private readonly IServiceProvider _serviceProvider;

    public BackworkerTaskFactory(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IBackworkerTaskAct? GetTask(int type) => type switch
    {
        (int) TaskTypes.SayHello => GetTask<SayHelloTask>(),
        (int) TaskTypes.SaySomething => GetTask<SaySomethingTask>(),
        _ => throw new Exception($"unknown type '${type}'")
    };
    
    private T? GetTask<T>() where T : class, IBackworkerTaskAct
    {
        return _serviceProvider.GetRequiredService(typeof(T)) as T;
    }
}