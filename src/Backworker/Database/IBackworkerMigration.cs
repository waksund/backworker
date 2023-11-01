namespace Backworker.Database;

public interface IBackworkerMigration
{
    Task MigrateUpAsync();
}