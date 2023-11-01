using Microsoft.Extensions.DependencyInjection;

namespace Backworker.Database.Postgres;

public static class PostgresBackworkerBuilderExtensions
{
    public static IBackworkerBuilder AddPostgres(this IBackworkerBuilder builder)
    {
        builder.Services.AddSingleton<DbAcrionRunner>();
        
        builder.Services.AddSingleton<IDbManager, DbManager>();
        builder.Services.AddSingleton<IBackworkerMigration, DbManager>();

        return builder;
    }
}
