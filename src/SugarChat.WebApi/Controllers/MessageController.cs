using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Messages;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Paging;
using SugarChat.Message.Basic;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("send"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> SendMessage(SendMessageCommand command)
        {
            var response = await _mediator.SendAsync<SendMessageCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("revoke"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> RevokeMessage(RevokeMessageCommand command)
        {
            var response = await _mediator.SendAsync<RevokeMessageCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("getUnreadMessageCount"), HttpGet]
        public async Task<IActionResult> GetUnreadMessageCountForGet([FromQuery] GetUnreadMessageCountRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);

            return Ok(response);
        }

        [Route("getUnreadMessageCount"), HttpPost]
        public async Task<IActionResult> GetUnreadMessageCountForPost(GetUnreadMessageCountRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);

            return Ok(response);
        }

        [Route("getUnreadMessagesFromGroupRequest"), HttpGet]
        public async Task<IActionResult> GetUnreadMessagesFromGroup([FromQuery] GetUnreadMessagesFromGroupRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetUnreadMessagesFromGroupRequest, SugarChatResponse<IEnumerable<MessageDto>>>(request);

            return Ok(response);
        }

        [Route("getAllToUserFromGroup"), HttpGet]
        public async Task<IActionResult> GetAllToUserFromGroup([FromQuery] GetAllMessagesFromGroupRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetAllMessagesFromGroupRequest, SugarChatResponse<IEnumerable<MessageDto>>>(request);

            return Ok(response);
        }

        [Route("getMessagesOfGroup"), HttpGet]
        public async Task<IActionResult> GetMessagesOfGroup([FromQuery] GetMessagesOfGroupRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetMessagesOfGroupRequest, SugarChatResponse<PagedResult<MessageDto>>>(request);

            return Ok(response);
        }

        [Route("getMessagesOfGroupBefore"), HttpGet]
        public async Task<IActionResult> GetMessagesOfGroupBefore([FromQuery] GetMessagesOfGroupBeforeRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetMessagesOfGroupBeforeRequest, SugarChatResponse<IEnumerable<MessageDto>>>(request);

            return Ok(response);
        }

        [Route("getMessagesByGroupIds"), HttpGet]
        public async Task<IActionResult> GetMessagesByGroupIdsForGet([FromQuery] GetMessagesByGroupIdsRequest request)
        {
            var response = await _mediator.RequestAsync<GetMessagesByGroupIdsRequest, SugarChatResponse<IEnumerable<MessageDto>>>(request);
            return Ok(response);
        }

        [Route("getMessagesByGroupIds"), HttpPost]
        public async Task<IActionResult> GetMessagesByGroupIdsForPost(GetMessagesByGroupIdsRequest request)
        {
            var response = await _mediator.RequestAsync<GetMessagesByGroupIdsRequest, SugarChatResponse<IEnumerable<MessageDto>>>(request);
            return Ok(response);
        }

        [Route("UpdateMessageData"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> UpdateMessageData(UpdateMessageDataCommand command)
        {
            var response = await _mediator.SendAsync<UpdateMessageDataCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("MigrateCustomProperty"), HttpPost]
        public async Task<IActionResult> MigrateCustomProperty(MigrateMessageCustomPropertyCommand command)
        {
            var response = await _mediator.SendAsync<MigrateMessageCustomPropertyCommand, SugarChatResponse>(command);
            return Ok(response);
        }
    }
}
