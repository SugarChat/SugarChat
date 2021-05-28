using Mediator.Net;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SugarChat.Push.SignalR.Hubs;
using SugarChat.Push.SignalR.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.SignalR.ServerClient.Test
{
    public class ServerHttpClientUnitTest
    {

        [Fact]
        public async Task TestHub()
        {
            var hub = new ApiHub(new Mock<IMediator>().Object, new Mock<IConnectService>().Object);
            var mockClients = new Mock<IHubCallerClients>();
            hub.Clients = mockClients.Object;
            await hub.SendMessage(new Mock<Push.SignalR.Models.SendMessageModel>().Object);
            Assert.True(true);
        }

    }
}
