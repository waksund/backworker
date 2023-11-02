using System.Data.Common;
using Backworker.Database.Postgres.DbTables;
using Backworker.Model;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Backworker.Database.Postgres;

internal class DbManager : IDbManager, IBackworkerMigration
{
    private readonly DbAcrionRunner _acrionRunner;

    public DbManager(
        DbAcrionRunner acrionRunner)
    {
        _acrionRunner = acrionRunner;
    }

    public Task<BackworkerTask> GetAndLockStartBackworkerTaskAsync()
    {
        return _acrionRunner.PerformDbActionAsync(async connection =>
        {
            BackworkerTask runTask = null;
            DateTime now = DateTime.UtcNow;

            await connection.ExecuteAsync("lock backworker.active_tasks");

            using var multipleResults = await connection
                    .QueryMultipleAsync(
                        $"select * from backworker.active_tasks; select * from backworker.tasks; select * from backworker.task_log")
                ;
            IEnumerable<active_tasks> activeTasks = await multipleResults.ReadAsync<active_tasks>();
            IEnumerable<tasks> dtoBwTasks = await multipleResults.ReadAsync<tasks>();
            IEnumerable<task_log> bwTaskLogs = await multipleResults.ReadAsync<task_log>();

            foreach (var dtoBwTask in dtoBwTasks.Where(t => !activeTasks.ToList().Exists(at => at.id == t.id)))
            {
                var taskLog = bwTaskLogs.SingleOrDefault(l => l.id == dtoBwTask.id);
                var task = Convert(dtoBwTask, taskLog);
                if (task.NeedToStart(now, null))
                {
                    runTask = task;
                    var dtoActiveTask = new active_tasks {id = runTask.Id, name = runTask.Name, lock_time = now};
                    await connection.DeleteAsync(dtoActiveTask);
                    await connection.InsertAsync(dtoActiveTask);
                    break;
                }
            }

            if (runTask == null)
            {
                foreach (var dtoBwTask in dtoBwTasks.Where(t => activeTasks.ToList().Exists(at => at.id == t.id)))
                {
                    var taskLog = bwTaskLogs.SingleOrDefault(l => l.id == dtoBwTask.id);
                    var activeTask = activeTasks.Single(at => at.id == dtoBwTask.id);
                    var task = Convert(dtoBwTask, taskLog);
                    if (task.NeedToStart(now, activeTask.lock_time))
                    {
                        runTask = task;
                        var dtoActiveTask = new active_tasks {id = runTask.Id, name = runTask.Name, lock_time = now};
                        await connection.DeleteAsync(dtoActiveTask);
                        await connection.InsertAsync(dtoActiveTask);
                        break;
                    }
                }
            }

            return runTask;
        });
    }

    public Task SaveLogAsync(BackworkerTask backworkerTask)
    {
        return _acrionRunner.PerformDbActionAsync(async connection =>
        {
            var dto = new task_log
            {
                id = backworkerTask.Id,
                name = backworkerTask.Name,
                last_start_date = backworkerTask.LastStartTime,
                last_stop_date = backworkerTask.LastStopTime
            };

            await connection.DeleteAsync(dto);
            await connection.InsertAsync(dto);
        });
    }

    public Task UnlockBackworkerTask(BackworkerTask backworkerTask)
    {
        return _acrionRunner.PerformDbActionAsync(connection =>
        {
            return connection.DeleteAsync(new active_tasks {id = backworkerTask.Id});
        });
    }

    public Task MigrateUpAsync()
    {
        return _acrionRunner.PerformDbActionAsync(async connection =>
        {
            await connection.ExecuteAsync(@"create schema if not exists backworker");
            await connection
                    .ExecuteAsync(
                        @"create table if not exists backworker.version_info (id integer NOT NULL,update_at timestamp without time zone NOT NULL)");

            var versions = await connection.QueryAsync<version_info>("select * from backworker.version_info");

            int needVersion = 1;
            int maxVersion = !versions.Any() ? 0 : versions.Max(v => v.id);
            for (int i = maxVersion + 1; i <= needVersion; i++)
            {
                switch (i)
                {
                    case 1:
                        await MigrateUp_1_Async(connection);
                        break;
                    default:
                        throw new Exception($"migration {i} not found");
                }
            }
        });
    }

    private async Task MigrateUp_1_Async(DbConnection connection)
    {
        int id = 1;

        await connection
                .ExecuteAsync(
                    @"create table backworker.active_tasks (id integer NOT NULL, name text NOT NULL, lock_time timestamp without time zone NOT NULL)")
            ;
        await connection
                .ExecuteAsync(
                    @"create table backworker.task_log (id integer NOT NULL, name text NOT NULL, last_start_date timestamp without time zone NOT NULL, last_stop_date timestamp without time zone NOT NULL)")
            ;
        await connection.ExecuteAsync(@"create table backworker.tasks (
id integer PRIMARY KEY, 
name text NOT NULL, 
type integer NOT NULL, 
active boolean NOT NULL, 
magic_string text NOT NULL, 
milliseconds_repeat_period integer NOT NULL, 
time_day_start timestamp without time zone, 
time_day_stop timestamp without time zone, 
milliseconds_restart_delay integer NOT NULL, 
milliseconds_crash_restart_delay integer NOT NULL
)");

        var dtoVersionInfo = new version_info {id = id, update_at = DateTime.UtcNow};
        await connection
            .ExecuteAsync($"insert into backworker.version_info (id, update_at) values (@id, @update_at)",
                param: dtoVersionInfo);
    }
    
    private BackworkerTask Convert(tasks dtoBwTasks, task_log taskLog)
    {
        var bwTask = new BackworkerTask
        {
            Id = dtoBwTasks.id,
            Name = dtoBwTasks.name,
            Type = dtoBwTasks.type,
            Active = dtoBwTasks.active,
            MagicString = dtoBwTasks.magic_string,
            LastStartTime = taskLog?.last_start_date ?? DateTime.MinValue,
            LastStopTime = taskLog?.last_stop_date ?? DateTime.MinValue
        };

        var schedule = new ScheduleTaskSettings
        {
            MillisecondsRepeatPeriod = dtoBwTasks.milliseconds_repeat_period,
            TimeDayStart = dtoBwTasks.time_day_start,
            TimeDayStop = dtoBwTasks.time_day_stop,
            MillisecondsRestartDelay = dtoBwTasks.milliseconds_restart_delay,
            MillisecondsCrashRestartDelay = dtoBwTasks.milliseconds_crash_restart_delay
        };

        bwTask.Schedule = schedule;

        return bwTask;
    }

}
