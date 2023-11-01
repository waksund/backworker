using System.Reflection;
using Backworker.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Backworker;

public static class BackworkerServiceCollectionExtensions
{
    public static IServiceCollection UseBackworker(this IServiceCollection services, Type actFactoryType, params Assembly[] assemblies)
    {
        services.AddSingleton<BackworkerManager>();

        services.AddTransient(typeof(IBackworkerTaskFactory), actFactoryType);

        AddTasksImplementations(services, assemblies);

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
        List<TypeInfo> concretions = assembliesToScan
            .SelectMany(a => a.DefinedTypes)
            .Where(type => type.FindInterfacesThatClose(typeof(IBackworkerTaskAct)).Any())
            .ToList();

        foreach (TypeInfo type in concretions)
        {
            services.AddTransient(type);
        }
    }
    
    private static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
    {
        return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
    }
    
    private static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
    {
        if (pluggedType == null) yield break;

        if (!pluggedType.IsConcrete()) yield break;

        if (templateType.GetTypeInfo().IsInterface)
        {
            foreach (
                    var interfaceType in
                    pluggedType.GetInterfaces()
                        .Where(type => (type == templateType)))
            {
                yield return interfaceType;
            }
        }
    }
    
    private static bool IsConcrete(this Type type)
    {
        return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
    }
}