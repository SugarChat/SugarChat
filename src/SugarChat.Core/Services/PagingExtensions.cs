using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;

namespace SugarChat.Core.Services
{
    public static class PagingExtensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> collection, PageSettings pageSettings)
        {
            return collection.Skip(pageSettings.PageSize * (pageSettings.PageNum - 1)).Take(pageSettings.PageSize);
        }

        public static IEnumerable<T> Paging<T>(this IEnumerable<T> collection, PageSettings pageSettings)
        {
            return collection.Skip(pageSettings.PageSize * (pageSettings.PageNum - 1)).Take(pageSettings.PageSize);
        }

        public static IMongoQueryable<T> Paging<T>(this IMongoQueryable<T> collection, PageSettings pageSettings)
        {
            if (pageSettings is null)
            {
                return collection;
            }

            return collection.Skip(pageSettings.PageSize * (pageSettings.PageNum - 1)).Take(pageSettings.PageSize);
        }
    }
}