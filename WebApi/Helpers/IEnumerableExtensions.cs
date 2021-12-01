using System.Dynamic;
using System.Reflection;

namespace WebApi.Helpers
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expendoObjectList = new List<ExpandoObject>(source.Count());

            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrEmpty(fields))
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

            // 生成数据

            foreach (var obj in source)
            {
                var shapedObj = new ExpandoObject();

                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(obj);

                    shapedObj.TryAdd(propertyInfo.Name, propertyValue);
                }
                expendoObjectList.Add(shapedObj);
            }
            return expendoObjectList;
        }
    }
}
