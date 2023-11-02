using Backworker.Runner;

namespace Example.Application.Backworker.Tasks;

public class SaySomethingTask : IBackworkerTaskAct
{
    private readonly ILogger<SaySomethingTask> _logger;

    public SaySomethingTask(ILogger<SaySomethingTask> logger)
    {
        _logger = logger;
    }

    public Task RunAsync(string magicString)
    {
        _logger.LogInformation(magicString);
        return Task.CompletedTask;
    }
}