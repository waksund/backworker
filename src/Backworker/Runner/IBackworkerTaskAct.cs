namespace Backworker.Runner;

public interface IBackworkerTaskAct
{
    Task RunAsync(string magicString);
}