using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Requirement
{
    // Todo: change the TResource
    public class DomainRestrictedRequirement :
    AuthorizationHandler<DomainRestrictedRequirement, HubCallerContext>,
    IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DomainRestrictedRequirement requirement, HubCallerContext resource)
        {
            // Todo
            return Task.CompletedTask;
        }
    }
}
