using Dapper.Contrib.Extensions;

namespace Backworker.Database.Postgres.DbTables;

[Table("active_tasks")]
internal class active_tasks
{
    [Key]
    public int id { get; set; }
    public string name { get; set; }
    public DateTime lock_time { get; set; }
}