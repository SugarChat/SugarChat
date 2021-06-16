using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Messages;
using SugarChat.Shared.Dtos;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Net.Client.HttpClients
{
    public class SugarChatHttpClient : ISugarChatClient
    {
        private const string CreateUserUrl = "api/user/create";
        private const string GetUserProfileUrl = "api/User/getUserProfile";
        private const string UpdateMyProfileUrl = "api/User/updateMyProfile";
        private const string CreateGroupUrl = "api/Group/create";

        private const string GetUnreadMessageCountUrl = "api/message/getUnreadMessageCount";

        private readonly IHttpClientFactory _httpClientFactory;
        public SugarChatHttpClient()
        {
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        }

        private async Task<string> ExecuteAsync(string url, HttpMethod method, string requestString = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpRequestMessage(method, url);
            request.Content = new StringContent(requestString);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5000/");

            var response = await client.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                return result;
            }
            else
            {
                throw new HttpRequestException($"接口请求错误{response.StatusCode},错误原因{response.ReasonPhrase}");
            }
        }

        public async Task<SugarChatResponse> CreateUserAsync(AddUserCommand command, CancellationToken cancellationToken)
        {
            var response = await ExecuteAsync(CreateUserUrl, HttpMethod.Post, JsonConvert.SerializeObject(command), cancellationToken);
            var result = JsonConvert.DeserializeObject<SugarChatResponse>(response);
            return result;
        }

        public async Task<SugarChatResponse<UserDto>> GetUserProfileAsync(GetUserRequest request, CancellationToken cancellationToken)
        {
            var response = await ExecuteAsync($"{GetUserProfileUrl}?id={request.Id}", HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<SugarChatResponse<UserDto>>(response);
            return result;
        }

        public async Task<SugarChatResponse> UpdateMyProfileAsync(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var response = await ExecuteAsync(UpdateMyProfileUrl, HttpMethod.Post, JsonConvert.SerializeObject(command), cancellationToken);
            var result = JsonConvert.DeserializeObject<SugarChatResponse>(response);
            return result;
        }

        public async Task<SugarChatResponse> CreateGroupAsync(AddGroupCommand command, CancellationToken cancellationToken)
        {
            var response = await ExecuteAsync(CreateGroupUrl, HttpMethod.Post, JsonConvert.SerializeObject(command), cancellationToken);
            var result = JsonConvert.DeserializeObject<SugarChatResponse>(response);
            return result;
        }




        public async Task<SugarChatResponse<int>> GetUnreadMessageCountAsync(GetUnreadMessageCountRequest request, CancellationToken cancellationToken)
        {
            var response = await ExecuteAsync($"{GetUnreadMessageCountUrl}?userId={request.UserId}", HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<SugarChatResponse<int>>(response);
            return result;
        }


    }


}
