using Backworker.Runner;

namespace Example.Application.Backworker.Tasks;

public class SayHelloTask : IBackworkerTaskAct
{
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