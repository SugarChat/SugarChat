﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SugarChat.Core.IRepositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Data.MongoDb
{
    public class MongoDbQueryable<T> : IMongoQueryable<T>, ISugarChatQueryable<T>
    {
        private IMongoQueryable<T> _query;
        public MongoDbQueryable(IMongoQueryable<T> query)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }
        public Type ElementType => _query.ElementType;

        public Expression Expression => _query.Expression;

        public IQueryProvider Provider => _query.Provider;

        public IEnumerator<T> GetEnumerator()
        {
            return _query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ISugarChatQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            _query = _query.Where(predicate);
            return this;
        }

        public ISugarChatQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> predicate)
        {
            _query = _query.OrderBy(predicate);
            return this;
        }

        public ISugarChatQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> predicate)
        {
            _query = _query.OrderByDescending(predicate);
            return this;
        }

        public ISugarChatQueryable<T> Distinct()
        {
            _query = _query.Distinct();
            return this;
        }

        public ISugarChatQueryable<T> Take(int count)
        {
            _query = _query.Take(count);
            return this;
        }
        public ISugarChatQueryable<T> Skip(int count)
        {
            _query = _query.Skip(count);
            return this;
        }

        public ISugarChatQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            return new MongoDbQueryable<TResult>(_query.Select(selector));
        }

        public ISugarChatQueryable<TResult> SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector)
        {
            return new MongoDbQueryable<TResult>(_query.SelectMany(selector));
        }

        public ISugarChatQueryable<IGrouping<IEnumerable<TResult>, T>> GroupBy<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector)
        {
            return new MongoDbQueryable<IGrouping<IEnumerable<TResult>, T>>(_query.GroupBy(selector));
        }

        public ISugarChatQueryable<TResult> GroupBy<TKey, TResult>(Expression<Func<T, TKey>> keySelector, Expression<Func<TKey, IEnumerable<T>, TResult>> resultSelector)
        {
            return new MongoDbQueryable<TResult>(_query.GroupBy(keySelector, resultSelector));
        }

        public ISugarChatQueryable<TResult> GroupJoin<TInner, TKey, TResult>(IEnumerable<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, IEnumerable<TInner>, TResult>> resultSelector)
        {
            return new MongoDbQueryable<TResult>(_query.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector));
        }

        public ISugarChatQueryable<TResult> Join<TInner, TKey, TResult>(IEnumerable<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
        {
            return new MongoDbQueryable<TResult>(_query.Join(inner, outerKeySelector, innerKeySelector, resultSelector));
        }

        public async Task<double> SumAsync(Expression<Func<T, double>> predicate, CancellationToken cancellationToken = default)
        {
            return await _query.SumAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<double> AverageAsync(Expression<Func<T, double>> predicate, CancellationToken cancellationToken = default)
        {
            return await _query.AverageAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<double> MinAsync(Expression<Func<T, double>> predicate, CancellationToken cancellationToken = default)
        {
            return await _query.MinAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<double> MaxAsync(Expression<Func<T, double>> predicate, CancellationToken cancellationToken = default)
        {
            return await _query.MaxAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<T>> ToListAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return await _query
                    .Where(predicate)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            return await _query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return await _query.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
            }
            return await _query.CountAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return await _query.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            }
            return await _query.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return await _query.SingleAsync(predicate, cancellationToken).ConfigureAwait(false);
            }
            return await _query.SingleAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return await _query.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
            }
            return await _query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> FirstAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return await _query.FirstAsync(predicate, cancellationToken).ConfigureAwait(false);
            }
            return await _query.FirstAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate != null)
            {
                return await _query.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
            }
            return await _query.AnyAsync(cancellationToken).ConfigureAwait(false);
        }

        public QueryableExecutionModel GetExecutionModel()
        {
            return _query.GetExecutionModel();
        }

        public IAsyncCursor<T> ToCursor(CancellationToken cancellationToken = default)
        {
            return _query.ToCursor(cancellationToken);
        }

        public async Task<IAsyncCursor<T>> ToCursorAsync(CancellationToken cancellationToken = default)
        {
            return await _query.ToCursorAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}