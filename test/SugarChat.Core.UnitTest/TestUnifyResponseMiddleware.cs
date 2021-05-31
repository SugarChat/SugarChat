using Mediator.Net;
using Mediator.Net.Binding;
using SugarChat.Core.Basic;
using SugarChat.Core.Middlewares;
using SugarChat.Core.UnitTest.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.Core.UnitTest
{
    public class TestUnifyResponseMiddleware
    {
        [Fact]
        public async Task TestUnifyResponseMiddleware1()
        {
            var builder = SetupMediatorBuilderWithMiddleware();
            var mediator = SetupIMediator(builder);
            var response = await
                mediator.RequestAsync<TestRequest, TestRequestResponse>(new TestRequest());

            Assert.True(response is ISugarChatResponse);
        }
        MediatorBuilder SetupMediatorBuilderWithMiddleware()
        {
            var builder = new MediatorBuilder();
            builder.ConfigureGlobalReceivePipe(config => config.UnifyResponseMiddleware(typeof(SugarChatResponse)));
            return builder;
        }

        IMediator SetupIMediator(MediatorBuilder builder)
        {
            return builder.RegisterHandlers(() =>
            {
                var binding = new List<MessageBinding>
                {
                    new MessageBinding(typeof(TestRequest), typeof(TestRequestHandler)),
                };
                return binding;
            }).Build();
        }
    }
}
