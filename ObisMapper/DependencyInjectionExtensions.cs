using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ObisMapper.Mapper;

namespace ObisMapper;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddObisMapper(this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.TryAdd(ServiceDescriptor.Describe(typeof(IObisMapper), typeof(BaseObisMapper), serviceLifetime));
        return services;
    }
}