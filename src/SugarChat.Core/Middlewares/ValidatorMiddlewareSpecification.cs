﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Exceptions;
using SugarChat.Message.Commands;
using System.Runtime.ExceptionServices;

namespace SugarChat.Core.Middlewares
{
    public class ValidatorMiddlewareSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : IContext<IMessage>
    {
        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            var command = context.Message as IdRequiredCommand;
            return command is null || !string.IsNullOrEmpty(command.Id);
        }

        public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            if (ShouldExecute(context, cancellationToken))
            {
                return Task.CompletedTask;
            }

            throw new BusinessWarningException(Prompt.IdIsNullOrEmpty);
        }

        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task AfterExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task OnException(Exception ex, TContext context)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw ex;
        }
    }
}