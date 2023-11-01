using Microsoft.Extensions.DependencyInjection;

namespace Backworker;

public class BackworkerBuilder: IBackworkerBuilder
{
    public BackworkerBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}