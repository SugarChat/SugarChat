using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace SugarChat.Data.MongoDb.Extensions
{
    public static class MongoQueryableExtension
    {
        public static async Task<double> SumAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<double> AverageAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<double> MinAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<double> MaxAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<int> CountAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<T> SingleAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<T> FirstAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }

        public static async Task<bool> AnyAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return default;
        }
    }
}
