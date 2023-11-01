using Dapper.Contrib.Extensions;

namespace Backworker.Database.Postgres.DbTables;

[Table("tasks")]
internal class tasks
{
    [Key]
    public int id { get; set; }
    public string name { get; set; }
    public int type { get; set; }
    public bool active { get; set; }
    public string magic_string { get; set; }

    public int milliseconds_repeat_period { get; set; }
    public DateTime? time_day_start { get; set; }
    public DateTime? time_day_stop { get; set; }
    public int milliseconds_restart_delay { get; set; }
    public int milliseconds_crash_restart_delay { get; set; }

}
