using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.IRepositories
{
    public interface ISugarChatQueryable<T> : IQueryable<T>
    {
        Task<List<T>> ToListAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<T> FirstAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<double> SumAsync(Expression<Func<T, double>> predicate, CancellationToken cancellationToken = default);

        Task<double> AverageAsync(Expression<Func<T, double>> predicate, CancellationToken cancellationToken = default);

        Task<double> MinAsync(Expression<Func<T, double>> predicate, CancellationToken cancellationToken = default);

        Task<double> MaxAsync(Expression<Func<T, double>> predicate, CancellationToken cancellationToken = default);
        ISugarChatQueryable<T> Where(Expression<Func<T, bool>> predicate);
        ISugarChatQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> predicate);
        ISugarChatQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> predicate);
        ISugarChatQueryable<T> Distinct();
        ISugarChatQueryable<T> Take(int count);
        ISugarChatQueryable<T> Skip(int count);
        ISugarChatQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);
        ISugarChatQueryable<TResult> SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector);
        ISugarChatQueryable<IGrouping<IEnumerable<TResult>, T>> GroupBy<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector);
        ISugarChatQueryable<TResult> GroupBy<TKey, TResult>(Expression<Func<T, TKey>> keySelector, Expression<Func<TKey, IEnumerable<T>, TResult>> resultSelector);
        ISugarChatQueryable<TResult> GroupJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, IEnumerable<TInner>, TResult>> resultSelector);
        ISugarChatQueryable<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector);
    }
}
