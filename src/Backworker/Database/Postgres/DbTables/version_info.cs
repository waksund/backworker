using Dapper.Contrib.Extensions;

namespace Backworker.Database.Postgres.DbTables;

[Table("version_info")]
internal class version_info
{
    [Key]
    public int id { get; set; }
    public DateTime update_at { get; set; }
}