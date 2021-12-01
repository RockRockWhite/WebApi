
namespace WebApi.Services
{
    public interface IPropertyMappingService
    {
        void AddMapping<TSource, TDestination>(Dictionary<string, PropertyMappingValue> mapping);
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDsetination>();
        bool ValidMappingExists<TSource, TDestination>(string fields);
    }
}