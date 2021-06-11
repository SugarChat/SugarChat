using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using SugarChat.Core.Domain;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MongoDB;
using MongoDB.Driver.Linq;

namespace SugarChat.Data.MongoDb.Extensions
{
    public static class MongoQueryableExtensions
    {
        private static PropertyInfo AccessProp<T, TResult>(Expression<Func<T, TResult>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            var type = typeof(T);
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new Exception("Expression is not available!");
            }
            var propertyName = memberExpression.Member.Name;
            return type.GetProperties(BindingFlags.Public).FirstOrDefault(e => e.Name == propertyName);
        }

        private static async Task<List<TResult>> GetPropertyValues<T, TResult>(this IQueryable<T> queryable, Expression<Func<T, TResult>> property, CancellationToken cancellationToken)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            var enumerable = queryable as IAsyncEnumerable<T>;
            List<TResult> vals = new List<TResult>();
            var prop = AccessProp(property);
            await foreach (var item in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                TResult val = (TResult)prop.GetValue(item);
                vals.Add(val);
            }
            return vals;
        }

        private static async Task<List<T>> GetEntities<T>(this IQueryable<T> queryable, CancellationToken cancellationToken)
        {
            List<T> vals = new List<T>();
            var enumerable = queryable as IAsyncEnumerable<T>;
            await foreach (var item in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                vals.Add(item);
            }
            return vals;
        }
        public static async Task<double> SumAsync<T>(this IQueryable<T> queryable, Expression<Func<T, double>> property, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            var results = await queryable.GetPropertyValues(property, cancellationToken);
            return results.Sum();
        }

        public static async Task<double> AverageAsync<T>(this IQueryable<T> queryable, Expression<Func<T, double>> property, CancellationToken cancellationToken = default)
        {
            var results = await queryable.GetPropertyValues(property, cancellationToken);
            return results.Average();
        }

        public static async Task<double> MinAsync<T>(this IQueryable<T> queryable, Expression<Func<T, double>> property, CancellationToken cancellationToken = default)
        {
            var results = await queryable.GetPropertyValues(property, cancellationToken);
            return results.Min();
        }

        public static async Task<double> MaxAsync<T>(this IQueryable<T> queryable, Expression<Func<T, double>> property, CancellationToken cancellationToken = default)
        {
            var results = await queryable.GetPropertyValues(property, cancellationToken);
            return results.Max();
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            var results = await queryable.GetEntities(cancellationToken);
            return results;
        }

        public static async Task<int> CountAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            var enumerable = queryable as IAsyncEnumerable<T>;
            int count = 0;
            await foreach (var item in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                count++;
            }
            return count;
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
