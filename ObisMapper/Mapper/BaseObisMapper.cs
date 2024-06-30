using Microsoft.Extensions.Logging;

namespace ObisMapper.Mapper;

public class BaseObisMapper : IObisMapper
{
    private readonly ILogger<IObisMapper> _logger;
    private readonly Dictionary<Type, object> _profiles = new();

    public BaseObisMapper(ILogger<IObisMapper> logger)
    {
        _logger = logger;
    }

    public void AddProfile<TDestination>(ObisMappingConfiguration<TDestination> configuration)
    {
        _profiles[typeof(TDestination)] = configuration;
    }

    public void Map<TDestination>(TDestination destination, string obisCode, object value)
    {
        if (!_profiles.TryGetValue(typeof(TDestination), out var profile))
            throw new InvalidOperationException($"No mapping profile for {typeof(TDestination)}");

        var typedProfile = (ObisMappingConfiguration<TDestination>)profile;

        var mapping = typedProfile.GetMapping(obisCode);
        if (mapping != null)
        {
            mapping.Invoke(destination, value, obisCode);
            return;
        }

        var patternMapping = typedProfile.GetPatternMapping(obisCode);
        if (patternMapping != null)
        {
            patternMapping.Invoke(destination, value, obisCode);
            return;
        }

        _logger.LogInformation("No mapping found for OBIS code {obisCode}", obisCode);
    }
}