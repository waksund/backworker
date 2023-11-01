using Microsoft.Extensions.DependencyInjection;

namespace Backworker;

public interface IBackworkerBuilder
{
    IServiceCollection Services { get; }
}