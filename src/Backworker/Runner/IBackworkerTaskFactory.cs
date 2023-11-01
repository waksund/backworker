namespace Backworker.Runner;

public interface IBackworkerTaskFactory
{
    IBackworkerTaskAct GetAct(int type);
}