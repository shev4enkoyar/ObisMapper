namespace ObisMapper.Mapper;

public interface IObisMapper
{
    void AddProfile<TDestination>(ObisMappingConfiguration<TDestination> configuration);

    void Map<TDestination>(TDestination destination, string obisCode, object value);
}