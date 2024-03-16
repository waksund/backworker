namespace Backworker.Runner;

public interface IBackworkerTaskAct
{
    static int Type { get; }
    
    Task RunAsync(string magicString);
}