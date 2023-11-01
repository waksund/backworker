using Dapper.Contrib.Extensions;

namespace Backworker.Database.Postgres.DbTables;

[Table("task_log")]
internal class task_log
{
    [Key]
    public int id { get; set; }
    public string name { get; set; }
    public DateTime last_start_date { get; set; }
    public DateTime last_stop_date { get; set; }
}
