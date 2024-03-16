using Backworker.Runner;

namespace Example.Backworker.Tasks;

public class SayHelloTask : IBackworkerTaskAct
{
    public static int Type => (int)TaskTypes.SayHello;
    
    private readonly ILogger<SayHelloTask> _logger;

    public SayHelloTask(ILogger<SayHelloTask> logger)
    {
        _logger = logger;
    }

    public Task RunAsync(string magicString)
    {
        _logger.LogInformation("Hello");
        return Task.CompletedTask;
    }
}