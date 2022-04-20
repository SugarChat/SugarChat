﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares.TransactionMiddlewares
{
    public class TransactionSpecification<TContext> : IPipeSpecification<TContext> where TContext : IContext<IMessage>
    {
        readonly IRepository _repository;

        public TransactionSpecification(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AfterExecute(TContext context, CancellationToken cancellationToken)
        {
            await _repository.CommitTransactionAsync();
        }

        public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public async Task OnException(Exception ex, TContext context)
        {
            await _repository.AbortTransactionAsync();
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw ex;
        }

        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
