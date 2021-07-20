using Newtonsoft.Json;
using SugarChat.Message.Commands.Friends;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Users;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Net.Client.Exceptions;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Requests.Messages;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Message.Basic;
using SugarChat.Message.Requests.GroupUsers;
using SugarChat.Message.Dtos;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Dtos.GroupUsers;
using SugarChat.Message.Paging;

namespace SugarChat.Net.Client.HttpClients
{
    public partial class SugarChatHttpClient : ISugarChatClient
    {
        private const string _getConnectionUrl = "api/chat/getConnectionUrl";
        private const string _getMessageListUrl = "api/conversation/getMessageList";
        private const string _getConversationListUrl = "api/conversation/getConversationList";
        private const string _getConversationProfileUrl = "api/conversation/getConversationProfile";
        private const string _setMessageReadUrl = "api/conversation/setMessageRead";
        private const string _deleteConversationUrl = "api/conversation/deleteConversation";
        private const string _addFriendUrl = "api/friend/add";
        private const string _removeFriendUrl = "api/friend/remove";
        private const string _createGroupUrl = "api/group/create";
        private const string _dismissGroupUrl = "api/group/dismiss";
        private const string _getGroupListUrl = "api/group/getGroupList";
        private const string _getGroupProfileUrl = "api/group/getGroupProfile";
        private const string _updateGroupProfileUrl = "api/group/updateGroupProfile";
        private const string _removeGroupUrl = "api/group/remove";
        private const string _getGroupMemberListUrl = "api/groupUser/getGroupMemberList";
        private const string _setGroupMemberCustomFieldUrl = "api/groupUser/setGroupMemberCustomField";
        private const string _joinGroupUrl = "api/groupUser/join";
        private const string _quitGroupUrl = "api/groupUser/quit";
        private const string _changeGroupOwnerUrl = "api/groupUser/changeGroupOwner";
        private const string _addGroupMemberUrl = "api/groupUser/addGroupMember";
        private const string _deleteGroupMemberUrl = "api/groupUser/deleteGroupMember";
        private const string _setMessageRemindTypeUrl = "api/groupUser/setMessageRemindType";
        private const string _setGroupMemberRoleUrl = "api/groupUser/setGroupMemberRole";
        private const string _sendMessageUrl = "api/message/send";
        private const string _revokeMessageUrl = "api/message/revoke";
        private const string _getUnreadMessageCountUrl = "api/message/getUnreadMessageCount";
        private const string _getUnreadMessagesFromGroupUrl = "api/message/getUnreadMessagesFromGroup";
        private const string _getAllToUserFromGroupUrl = "api/message/getAllToUserFromGroup";
        private const string _getMessagesOfGroupUrl = "api/message/getMessagesOfGroup";
        private const string _getMessagesOfGroupBeforeUrl = "api/message/getMessagesOfGroupBefore";
        private const string _createUserUrl = "api/user/create";
        private const string _getUserProfileUrl = "api/user/getUserProfile";
        private const string _updateMyProfileUrl = "api/user/updateMyProfile";
        private const string _setMessageReadSetByUserBasedOnGroupIdUrl = "api/conversation/setMessageReadSetByUserBasedOnGroupId";
        private const string _getUserIdsByGroupIdsUrl = "api/groupUser/getUserIdsByGroupIds";
        private const string _getConversationByKeywordUrl = "api/conversation/getConversationByKeyword";
        private const string _getByCustomPropertiesUrl = "api/group/getByCustomProperties";
        private const string _getMessagesByGroupIdsUrl = "api/message/getMessagesByGroupIds";
        private const string _batchAddUsersUrl = "api/user/batchAddUsers";


        private string _baseUrl = "";
        public SugarChatHttpClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }

            public T Object { get; }

            public string Text { get; }
        }

        public bool ReadResponseAsString { get; set; }

        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, ReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }

            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var serializer = JsonSerializer.Create();
                        var typedBody = serializer.Deserialize<T>(jsonTextReader);
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }

        private async Task<T> ExecuteAsync<T>(string url, HttpMethod method, string requestString = "", CancellationToken cancellationToken = default)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_baseUrl);

            try
            {
                using (var request = new HttpRequestMessage(method, url))
                {
                    var content = new StringContent(requestString);
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    request.Content = content;
                    request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("text/plain"));

                    var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                    try
                    {
                        var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                        if (response.Content != null && response.Content.Headers != null)
                        {
                            foreach (var item in response.Content.Headers)
                                headers[item.Key] = item.Value;
                        }

                        var status = (int)response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            var objectResponse = await ReadObjectResponseAsync<T>(response, new ReadOnlyDictionary<string, IEnumerable<string>>(headers), cancellationToken).ConfigureAwait(false);
                            if (objectResponse.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                            }
                            return objectResponse.Object;
                        }
                        else
                        {
                            var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                        }
                    }
                    finally
                    {
                        response.Dispose();
                    }
                }
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task<SugarChatResponse<UserDto>> GetUserProfileAsync(GetUserRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getUserProfileUrl}?id={request.Id}";
            return await ExecuteAsync<SugarChatResponse<UserDto>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> UpdateMyProfileAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_updateMyProfileUrl, HttpMethod.Post, JsonConvert.SerializeObject(command), cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> CreateUserAsync(AddUserCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_createUserUrl, HttpMethod.Post, JsonConvert.SerializeObject(command), cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> GetConnectionUrlAsync(string userIdentifier, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getConnectionUrl}?userIdentifier={userIdentifier}";
            return await ExecuteAsync<string>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<GetMessageListResponse>> GetMessageListAsync(GetMessageListRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getMessageListUrl}?userId={request.UserId}&conversationId={request.ConversationId}&nextReqMessageId={request.NextReqMessageId}&count={request.Count}&index={request.Index}";
            return await ExecuteAsync<SugarChatResponse<GetMessageListResponse>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<ConversationDto>>> GetConversationListAsync(GetConversationListRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getConversationListUrl}?userId={request.UserId}&pageSettings.pageSize={request.PageSettings.PageSize}&pageSettings.pageNum={request.PageSettings.PageNum}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<ConversationDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<ConversationDto>> GetConversationProfileAsync(GetConversationProfileRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getConversationProfileUrl}?conversationId={request.ConversationId}&userId={request.UserId}";
            return await ExecuteAsync<SugarChatResponse<ConversationDto>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> SetMessageReadAsync(SetMessageReadByUserBasedOnMessageIdCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_setMessageReadUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> DeleteConversationAsync(RemoveConversationCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_deleteConversationUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> AddFriendAsync(AddFriendCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_addFriendUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> RemoveFriendAsync(RemoveFriendCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_removeFriendUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> CreateGroupAsync(AddGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_createGroupUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> DismissGroupAsync(DismissGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_dismissGroupUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<PagedResult<GroupDto>>> GetGroupListAsync(GetGroupsOfUserRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getGroupListUrl}?id={request.Id}&pageSettings.pageSize={request.PageSettings.PageSize}&pageSettings.pageNum={request.PageSettings.PageNum}";
            return await ExecuteAsync<SugarChatResponse<PagedResult<GroupDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<GroupDto>> GetGroupProfileAsync(GetGroupProfileRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getGroupProfileUrl}?userId={request.UserId}&groupId={request.GroupId}";
            return await ExecuteAsync<SugarChatResponse<GroupDto>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> UpdateGroupProfileAsync(UpdateGroupProfileCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_updateGroupProfileUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> RemoveGroupAsync(RemoveGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_removeGroupUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<GroupUserDto>>> GetGroupMemberListAsync(GetMembersOfGroupRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getGroupMemberListUrl}?userId={request.UserId}&groupId={request.GroupId}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<GroupUserDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> SetGroupMemberCustomFieldAsync(SetGroupMemberCustomFieldCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_setGroupMemberCustomFieldUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> JoinGroupAsync(JoinGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_joinGroupUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> QuitGroupAsync(QuitGroupCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_quitGroupUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> ChangeGroupOwnerAsync(ChangeGroupOwnerCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_changeGroupOwnerUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> AddGroupMemberAsync(AddGroupMemberCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_addGroupMemberUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> DeleteGroupMemberAsync(RemoveGroupMemberCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_deleteGroupMemberUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> SetMessageRemindTypeAsync(SetMessageRemindTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_setMessageRemindTypeUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> SetGroupMemberRoleAsync(SetGroupMemberRoleCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_setGroupMemberRoleUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> SendMessageAsync(SendMessageCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_sendMessageUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> RevokeMessageAsync(RevokeMessageCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_revokeMessageUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<int>> GetUnreadMessageCountAsync(GetUnreadMessageCountRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getUnreadMessageCountUrl}?userId={request.UserId}";
            return await ExecuteAsync<SugarChatResponse<int>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> GetUnreadMessagesFromGroupAsync(GetUnreadMessagesFromGroupRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getUnreadMessagesFromGroupUrl}?userId={request.UserId}&groupId={request.GroupId}&messageId={request.MessageId}&count={request.Count}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<MessageDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> GetAllToUserFromGroupAsync(GetAllMessagesFromGroupRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getAllToUserFromGroupUrl}?userId={request.UserId}&groupId={request.GroupId}&index={request.Index}&messageId={request.MessageId}&count={request.Count}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<MessageDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> GetMessagesOfGroupAsync(GetMessagesOfGroupRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getMessagesOfGroupUrl}?groupId={request.GroupId}&count={request.Count}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<MessageDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> GetMessagesOfGroupBeforeAsync(GetMessagesOfGroupBeforeRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getMessagesOfGroupBeforeUrl}?messageId={request.MessageId}&count={request.Count}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<MessageDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> SetMessageReadSetByUserBasedOnGroupIdAsync(SetMessageReadByUserBasedOnGroupIdCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_setMessageReadSetByUserBasedOnGroupIdUrl, HttpMethod.Post, JsonConvert.SerializeObject(command)).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<string>>> GetUserIdsByGroupIdsAsync(GetUserIdsByGroupIdsRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getUserIdsByGroupIdsUrl}?groupIds={request.GroupIds}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<string>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<PagedResult<ConversationDto>>> GetConversationByKeywordAsync(GetConversationByKeywordRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getConversationByKeywordUrl}?userId={request.UserId}&searchParms={request.SearchParms}&pageSettings.pageSize={request.PageSettings.PageSize}&pageSettings.pageNum={request.PageSettings.PageNum}&isExactSearch={request.IsExactSearch}";
            return await ExecuteAsync<SugarChatResponse<PagedResult<ConversationDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<GroupDto>>> GetByCustomPropertiesAsync(GetGroupByCustomPropertiesRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getByCustomPropertiesUrl}?userId={request.UserId}&customPropertys={request.CustomPropertys}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<GroupDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse<IEnumerable<MessageDto>>> GetMessagesByGroupIdsAsync(GetMessagesByGroupIdsRequest request, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_getMessagesByGroupIdsUrl}?userId={request.UserId}&groupIds={request.GroupIds}";
            return await ExecuteAsync<SugarChatResponse<IEnumerable<MessageDto>>>(requestUrl, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<SugarChatResponse> BatchAddUsers(BatchAddUsersCommand command, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync<SugarChatResponse>(_batchAddUsersUrl, HttpMethod.Post, JsonConvert.SerializeObject(command), cancellationToken).ConfigureAwait(false);
        }
    }
}
