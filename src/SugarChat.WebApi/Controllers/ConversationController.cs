using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Shared.Dtos.Conversations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("getMessageList"), HttpGet]
        public async Task<IActionResult> GetPagingMessageList([FromQuery] GetMessageListRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetMessageListRequest, SugarChatResponse<MessageListResult>>(request);

            return Ok(response);
        }

        [Route("getConversationList"), HttpGet]
        public async Task<IActionResult> GetConversationListByUserId([FromQuery] GetConversationListRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetConversationListRequest, SugarChatResponse<IEnumerable<ConversationDto>>>(request);

            return Ok(response);
        }

        [Route("getConversationProfile"), HttpGet]
        public async Task<IActionResult> GetConversationProfileById([FromQuery] GetConversationProfileRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetConversationProfileRequest, SugarChatResponse<ConversationDto>>(request);

            return Ok(response);
        }

        [Route("setMessageRead"), HttpPost]
        public async Task<IActionResult> SetMessageRead(SetMessageAsReadCommand command)
        {
            var response = await _mediator.SendAsync<SetMessageAsReadCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }

        [Route("deleteConversation"), HttpPost]
        public async Task<IActionResult> DeleteConversation(DeleteConversationCommand command)
        {
            var response = await _mediator.SendAsync<DeleteConversationCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }


    }
}
