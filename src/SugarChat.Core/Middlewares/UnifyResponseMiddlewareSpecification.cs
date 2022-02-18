using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Serilog;
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

        public UnifyResponseMiddlewareSpecification()
        {
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
            Log.Error(ex, ex.Message);
            if (ex is not BusinessException || context.Message is IEvent)
            {
                if (ex is TimeoutException)
                {
                    ex = new BusinessWarningException(Prompt.DatabaseTimeout, ex.InnerException);
                }
                else if (ex is MongoDB.Driver.MongoWriteException)
                {
                    var mongoWriteException = (MongoDB.Driver.MongoWriteException)ex;
                    switch (mongoWriteException.WriteError.Code)
                    {
                        case 11000:
                            ex = new BusinessWarningException(Prompt.DatabaseDuplicateKey, ex.InnerException);
                            break;
                        default:
                            ExceptionDispatchInfo.Capture(ex).Throw();
                            break;
                    }
                }
                else
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
            var businessException = ex as BusinessException;
            if (context.Result is null)
            {
                var unifiedTypeInstance = Activator.CreateInstance(context.ResultDataType);
                context.Result = unifiedTypeInstance;
            }

            if (context.Result is ISugarChatResponse response)
            {
                response.Code = businessException.Code;
                response.Message = businessException.Message;
            }
            return Task.CompletedTask;
        }

        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}