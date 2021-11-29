using Microsoft.EntityFrameworkCore;

namespace WebApi.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int Limit { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasProvious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> Items, int count, int offset, int limit)
        {
            TotalCount = count;
            CurrentPage = offset + 1;
            Limit = limit;
            TotalPages = (int)Math.Ceiling(count / (double)limit);

            AddRange(Items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int offset, int limit)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(offset * limit).Take(limit).ToListAsync();

            return new PagedList<T>(items, count, offset, limit);
        }
    }
}
