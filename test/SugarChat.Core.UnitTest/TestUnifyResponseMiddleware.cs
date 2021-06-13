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
using SugarChat.Core.UnitTest.Events;
using SugarChat.Core.Exceptions;

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

            Assert.Equal(Prompt.NotAllUsersExists.Code, response.Code);
            Assert.Equal(Prompt.NotAllUsersExists.Message, response.Message);
        }
        [Fact]
        public async Task TestUnifyResponseMiddleware2()
        {
            var builder = SetupMediatorBuilderWithMiddleware();
            var mediator = SetupIMediator2(builder);
            var response = await
                mediator.RequestAsync<TestRequest, SugarChatResponse<string>>(new TestRequest());


            Assert.Equal(20000, response.Code);
            Assert.Equal("Success", response.Message);
            Assert.Equal("TestData", response.Data);
        }
        [Fact]
        public async Task TestUnifyResponseMiddlewarePublish()
        {
            var builder = SetupMediatorBuilderWithMiddleware();
            var mediator = SetupIMediator3(builder);
            await mediator.PublishAsync<TestEvent>(new TestEvent());
        }
        [Fact]
        public async Task TestUnifyResponseMiddlewarePublishEx()
        {
            var builder = SetupMediatorBuilderWithMiddleware();
            var mediator = SetupIMediator4(builder);
            try
            {
                await mediator.PublishAsync<TestEvent>(new TestEvent());
            }
            catch(Exception ex)
            {
                Assert.True(ex is BusinessWarningException);
            }
        }
        MediatorBuilder SetupMediatorBuilderWithMiddleware()
        {
            var builder = new MediatorBuilder();
            builder.ConfigureGlobalReceivePipe(config => config.AddPipeSpecifications());
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
        IMediator SetupIMediator3(MediatorBuilder builder)
        {
            return builder.RegisterHandlers(() =>
            {
                var binding = new List<MessageBinding>
                {
                    new MessageBinding(typeof(TestEvent), typeof(TestEventHandler)),
                };
                return binding;
            }).Build();
        }
        IMediator SetupIMediator4(MediatorBuilder builder)
        {
            return builder.RegisterHandlers(() =>
            {
                var binding = new List<MessageBinding>
                {
                    new MessageBinding(typeof(TestEvent), typeof(TestEventExHandler)),
                };
                return binding;
            }).Build();
        }
    }
}
