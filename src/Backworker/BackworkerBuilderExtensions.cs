using Microsoft.Extensions.DependencyInjection;

namespace Backworker;

public static class BackworkerBuilderExtensions
{
    public static IBackworkerBuilder WithGlobalConnectionString(
        this IBackworkerBuilder builder,
        string connectionString)
    {
        builder.Services.Configure<BackworkerOptions>(opt => opt.ConnectionString = connectionString);

        return builder;
    }
}
