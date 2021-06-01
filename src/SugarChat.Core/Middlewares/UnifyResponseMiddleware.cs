﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using SugarChat.Core.Basic;
using SugarChat.Core.Exceptions;
using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares
{
    public class UnifyResponseMiddlewareSpecification<TContext> : IPipeSpecification<TContext>
         where TContext : IContext<IMessage>
    {
        private readonly Type _unifiedType;

        public UnifyResponseMiddlewareSpecification(Type unifiedType)
        {
            _unifiedType = unifiedType;
        }

        public Task AfterExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task OnException(Exception ex, TContext context)
        {
            if (_unifiedType == null || ex is not BusinessException)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw ex;
            }
            var businessException = ex as BusinessException;
            if (context.Result is null)
            {
                var tArgs = context.ResultGenericArguments;
                var targetType = tArgs != null && tArgs.Any() ? _unifiedType.MakeGenericType(tArgs) : _unifiedType;

                var unifiedTypeInstance = Activator.CreateInstance(targetType);
                context.Result = unifiedTypeInstance;
            }

            if (!(context.Result is SugarChatResponse response))
                return Task.CompletedTask;

            response.Code = businessException.Code;
            response.Message = businessException.Message;
            return Task.CompletedTask;
        }

        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}