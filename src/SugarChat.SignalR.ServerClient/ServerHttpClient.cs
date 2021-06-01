using Newtonsoft.Json;
using SugarChat.SignalR.Server.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.SignalR.ServerClient
{
    public class ServerHttpClient : IServerClient
    {
        const string ConnectionUrl = "api/chat/ConnectionUrl";
        const string GroupUrl = "api/chat/Group";
        const string MessageUrl = "api/chat/Message";
        const string MassMessageUrl = "api/chat/MassMessage";
        const string CustomMessageUrl = "api/chat/CustomMessage";
        const string MassCustomMessageUrl = "api/chat/MassCustomMessage";
        public HttpClient Client { get; private set; }

        public ServerHttpClient(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public async Task<string> GetConnectionUrl(string userIdentifier)
        {
            var result = await Client.GetStringAsync($"{ConnectionUrl}?userIdentifier={userIdentifier}").ConfigureAwait(false);
            return result;
        }

        public async Task Group(GroupActionModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(GroupUrl, content).ConfigureAwait(false);
        }

        public async Task SendMessage(SendMessageModel model)
        {
            var strlist = new List<string>();
            foreach (var message in model.Messages)
            {
                strlist.Add(message.ToString());
            }
            var jsonModle = new
            {
                model.SendTo,
                model.SendWay,
                Messages = strlist
            };
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(MessageUrl, content).ConfigureAwait(false);
        }

        public async Task SendMassMessage(SendMassMessageModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(MassMessageUrl, content).ConfigureAwait(false);
        }

        public async Task SendCustomMessage(SendCustomMessageModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(CustomMessageUrl, content).ConfigureAwait(false);
        }

        public async Task SendMassCustomMessage(SendMassCustomMessageModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await Client.PostAsync(MassCustomMessageUrl, content).ConfigureAwait(false);
        }
    }
}
