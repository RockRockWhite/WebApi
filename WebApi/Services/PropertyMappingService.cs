using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            AddMapping<CompanyDto, Company>(new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id",new PropertyMappingValue(new List<String>{"Id" }) },
                 { "CompanyName",new PropertyMappingValue(new List<String>{"Name" }) },

            });
        }
        public void AddMapping<TSource, TDestination>(Dictionary<string, PropertyMappingValue> mapping)
        {
            _propertyMappings.Add(new PropertyMapping<TSource, TDestination>(mapping));
        }

        public bool ValidMappingExists<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedFields = field.Trim();
                var propertyName = trimmedFields.Replace(" desc", "");

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception($"无法找到唯一的映射关系: {typeof(TSource)}->{typeof(TDestination)}");
        }
    }
}
