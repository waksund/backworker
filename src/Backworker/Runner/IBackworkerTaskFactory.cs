namespace Backworker.Runner;

public interface IBackworkerTaskFactory
{
    IBackworkerTaskAct? GetTask(int type);
}