using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Mediator.CommandHandlers.Groups;
using SugarChat.Message.Commands.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Route("create"), HttpPost]
        public async Task<IActionResult> CreateGroup(AddGroupCommand command)
        {
            var response = await _mediator.SendAsync<AddGroupCommand, AddGroupResponse>(command);
            return Ok(response);
        }

        [Route("dismiss"), HttpPost]
        public async Task<IActionResult> DismissGroup(DismissGroupCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }

        [Route("join"), HttpPost]
        public async Task<IActionResult> JoinGroup(DismissGroupCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }

        [Route("quit"), HttpPost]
        public async Task<IActionResult> QuitGroup(QuitGroupCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }

        [Route("changeGroupOwner"), HttpPost]
        public async Task<IActionResult> ChangeGroupOwner(ChangeGroupOwnerCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }
    }
}