using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Message.Dtos;
using SugarChat.Message.Dtos.Conversations;
using System.Collections.Generic;
using System.Threading.Tasks;
using SugarChat.Message.Paging;

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
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<GetMessageListResponse>))]
        public async Task<IActionResult> GetPagingMessageList([FromQuery] GetMessageListRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetMessageListRequest, SugarChatResponse<GetMessageListResponse>>(request);

            return Ok(response);
        }

        [Route("getConversationList"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<IEnumerable<ConversationDto>>))]
        public async Task<IActionResult> GetConversationListByUserId([FromQuery] GetConversationListRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetConversationListRequest, SugarChatResponse<IEnumerable<ConversationDto>>>(request);

            return Ok(response);
        }

        [Route("getConversationProfile"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<ConversationDto>))]
        public async Task<IActionResult> GetConversationProfileById([FromQuery] GetConversationProfileRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetConversationProfileRequest, SugarChatResponse<ConversationDto>>(request);

            return Ok(response);
        }

        [Route("setMessageRead"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> SetMessageRead(SetMessageReadByUserBasedOnMessageIdCommand command)
        {
            var response = await _mediator.SendAsync<SetMessageReadByUserBasedOnMessageIdCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("deleteConversation"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> DeleteConversation(RemoveConversationCommand command)
        {
            var response = await _mediator.SendAsync<RemoveConversationCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("setMessageReadSetByUserBasedOnGroupId"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> SetMessageReadSetByUserBasedOnGroupId( SetMessageReadByUserBasedOnGroupIdCommand command)
        {
            var response = await _mediator.SendAsync<SetMessageReadByUserBasedOnGroupIdCommand, SugarChatResponse>(command);
            return Ok(response);
        }
        [Route("getConversationByKeyword"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<PagedResult<ConversationDto>>))]
        public async Task<IActionResult> GetConversationByKeyword([FromQuery] GetConversationByKeywordRequest request)
        {
            var response = await _mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(request);
            return Ok(response);
        }
    }
}
