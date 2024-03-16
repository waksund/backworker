using Backworker.Runner;

namespace Example.Backworker.Tasks;

public class SaySomethingTask : IBackworkerTaskAct
{
    public static int Type => (int)TaskTypes.SaySomething;
    
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