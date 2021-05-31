using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
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
        public async Task<IActionResult> GetPagingMessageList([FromQuery]GetMessageListRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetMessageListRequest, GetMessageListResponse>(request);

            return Ok(response);
        }

        [Route("getConversationList"), HttpGet]
        public async Task<IActionResult> GetConversationListByUserId([FromQuery] GetConversationListRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetConversationListRequest, GetConversationListResponse>(request);

            return Ok(response);
        }

        [Route("getConversationProfile"), HttpGet]
        public async Task<IActionResult> GetConversationProfileById([FromQuery] GetConversationProfileRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetConversationProfileRequest, GetConversationProfileResponse>(request);

            return Ok(response);
        }

        [Route("setMessageRead"), HttpPost]
        public async Task<IActionResult> SetMessageRead(SetMessageAsReadCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }





    }
}
