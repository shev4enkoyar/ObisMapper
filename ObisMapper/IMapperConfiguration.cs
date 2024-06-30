namespace ObisMapper;

public interface IMapperConfiguration
{
    void Configure<TDestination>(ObisMappingConfiguration<TDestination> configuration);
}