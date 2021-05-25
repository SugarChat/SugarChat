﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using SugarChat.Core.Basic;
using SugarChat.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
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
            if (_unifiedType == null || ex.GetType() != typeof(BusinessException))
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw ex;
            }

            var businessException = ex as BusinessException;
            var unifiedTypeInstance = (ISugarChatResponse)Activator.CreateInstance(_unifiedType);
            unifiedTypeInstance.Data = context.Result;
            unifiedTypeInstance.Code = businessException.Code;
            unifiedTypeInstance.Message = businessException.Message;
            context.Result = unifiedTypeInstance;
            return Task.CompletedTask;
        }

        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
