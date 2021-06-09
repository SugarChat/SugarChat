using System.Threading.Tasks;
using SugarChat.SignalR.Server.Models;
using SugarChat.SignalR.ServerClient;

namespace SugarChat.IntegrationTest
{
    public class SignalRClientMock : IServerClient
    {
        public Task<string> GetConnectionUrl(string userIdentifier, bool isInterior = false)
        {
            return Task.FromResult(string.Empty);
        }

        public Task Group(GroupActionModel model)
        {
            return Task.CompletedTask;
        }

        public Task SendMessage(SendMessageModel model)
        {
            return Task.CompletedTask;
        }

        public Task SendMassMessage(SendMassMessageModel model)
        {
            return Task.CompletedTask;
        }

        public Task SendCustomMessage(SendCustomMessageModel model)
        {
            return Task.CompletedTask;
        }

        public Task SendMassCustomMessage(SendMassCustomMessageModel model)
        {
            return Task.CompletedTask;
        }
    }
}