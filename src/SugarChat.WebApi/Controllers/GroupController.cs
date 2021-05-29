using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("getGroupList"), HttpGet]
        public async Task<IActionResult> GetGroupList([FromQuery]GetGroupsOfUserRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetGroupsOfUserRequest, GetGroupsOfUserResponse>(request);

            return Ok(response);
        }

    }
}
