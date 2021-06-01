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
using SugarChat.Core.UnitTest.Request;
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
                mediator.RequestAsync<TestRequest, SugarChatResponse<string>>(new TestRequest());


            Assert.Equal(0, response.Code);
            Assert.Equal("Test", response.Message);
        }
        [Fact]
        public async Task TestUnifyResponseMiddleware2()
        {
            var builder = SetupMediatorBuilderWithMiddleware();
            var mediator = SetupIMediator2(builder);
            var response = await
                mediator.RequestAsync<TestRequest, SugarChatResponse<string>>(new TestRequest());


            Assert.Equal(0, response.Code);
            Assert.Equal("TestMessage", response.Message);
            Assert.Equal("TestData", response.Data);
        }
        MediatorBuilder SetupMediatorBuilderWithMiddleware()
        {
            var builder = new MediatorBuilder();
            builder.ConfigureGlobalReceivePipe(config => config.UnifyResponseMiddleware(typeof(SugarChatResponse<>)));
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
        IMediator SetupIMediator2(MediatorBuilder builder)
        {
            return builder.RegisterHandlers(() =>
            {
                var binding = new List<MessageBinding>
                {
                    new MessageBinding(typeof(TestRequest), typeof(TestDataFromResponseRequestHandler)),
                };
                return binding;
            }).Build();
        }
    }
}
