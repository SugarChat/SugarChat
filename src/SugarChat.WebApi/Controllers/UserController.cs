using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("getUserProfile"), HttpGet]
        public async Task<IActionResult> GetUserProfile(GetUserRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetUserRequest, GetUserResponse>(request);

            return Ok(response);
        }


    }
}
