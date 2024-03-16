using System.Reflection;
using Backworker.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Backworker;

public static class BackworkerServiceCollectionExtensions
{
    public static IServiceCollection UseBackworker(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<BackworkerManager>();

        services.AddSingleton<IBackworkerTaskFactory, BackworkerTaskFactory>();
        
        AddTasksImplementations(services, assemblies);
        
        services.AddHostedService<BackworkerHostedService>();

        return services;
    }
    
    public static IServiceCollection ConfigureBackworker(this IServiceCollection services, Action<IBackworkerBuilder> configure)
    {
        var builder = new BackworkerBuilder(services);
        configure.Invoke(builder);

        return services;
    }
    
    private static void AddTasksImplementations(IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
    {
        var acts = assembliesToScan
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                typeof(IBackworkerTaskAct).IsAssignableFrom(type)
                && !type.IsInterface
                && !type.IsAbstract);
        
        foreach (var act in acts)
        {
            int type =
                (int) act!
                    .GetProperty("Type")!
                    .GetValue(null, null)!;
            services.AddKeyedTransient(typeof(IBackworkerTaskAct), type, act);
        }
    }
}