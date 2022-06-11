using System;
using System.Collections.Generic;
using System.Linq;
using SugarChat.Message.Paging;

namespace SugarChat.Core.Services
{
    public static class PagingExtensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> collection, PageSettings pageSettings)
        {
            if (pageSettings is null)
            {
                throw new ArgumentException("PageSettings is required.");
            }
            return collection.Skip(pageSettings.PageSize * (pageSettings.PageNum - 1)).Take(pageSettings.PageSize);
        }

        public static IEnumerable<T> Paging<T>(this IEnumerable<T> collection, PageSettings pageSettings)
        {
            return collection.Skip(pageSettings.PageSize * (pageSettings.PageNum - 1)).Take(pageSettings.PageSize);
        }
    }
}