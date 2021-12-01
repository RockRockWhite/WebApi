using System.Dynamic;
using System.Reflection;

namespace WebApi.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObj = new ExpandoObject();

            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                propertyInfoList.AddRange(typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance));
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo is null)
                    {
                        throw new Exception($"Property:{propertyName} 没有找到: {typeof(TSource)}");
                    }

                    propertyInfoList.Add(propertyInfo);
                }
            }

            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(source);
                expandoObj.TryAdd(propertyInfo.Name, propertyValue);
            }

            return expandoObj;
        }
    }
}
