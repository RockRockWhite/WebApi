using System.Collections;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using WebApi.Services;

namespace WebApi.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (mappingDictionary is null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderByAfterSplit = orderBy.Split(',');
            // 反转 后面的排序优先级小 所以先排
            foreach (var orderByClause in orderByAfterSplit.Reverse())
            {
                var trimmedOrderByClause = orderByClause.Trim();

                // 判断desc
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                // 移除结尾的' desc'
                //var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                //var properName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);
                var properName = trimmedOrderByClause.Replace(" desc", "");

                if (!mappingDictionary.ContainsKey(properName))
                {
                    throw new ArgumentException($"没有找到Key为{properName}的映射");
                }

                var propertyMappingValue = mappingDictionary[properName];
                if (propertyMappingValue is null)
                {
                    throw new ArgumentNullException(nameof(propertyMappingValue));
                }

                // 反转 后面的排序优先级小 所以先排
                foreach (var distinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }

                    source = source.OrderBy(distinationProperty + (orderDescending ? " descending" : " ascending"));
                }
              
            }
            return source;
        }

    }
}
