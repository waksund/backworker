using Backworker.Model;

namespace Backworker.Database;

internal interface IDbManager
{
    Task<BackworkerTask> GetAndLockStartBackworkerTaskAsync();
    Task SaveLogAsync(BackworkerTask backworkerTask);
    Task UnlockBackworkerTask(BackworkerTask backworkerTask);
}
