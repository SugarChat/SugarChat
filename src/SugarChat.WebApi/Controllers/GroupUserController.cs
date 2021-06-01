using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Commands.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupUserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupUserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("join"), HttpPost]
        public async Task<IActionResult> JoinGroup(JoinGroupCommand command)
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

        [Route("addGroupMember"), HttpPost]
        public async Task<IActionResult> AddGroupMember(AddGroupMemberCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }

        [Route("deleteGroupMember"), HttpPost]
        public async Task<IActionResult> DeleteGroupMember(AddGroupMemberCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }

        [Route("setMessageRemindType"), HttpPost]
        public async Task<IActionResult> SetMessageRemindType(SetMessageRemindTypeCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }
    }
}
