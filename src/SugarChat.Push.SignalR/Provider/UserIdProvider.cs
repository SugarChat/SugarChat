using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Push.SignalR.Provider
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Todo: Return CurrentUserId
            return connection.UserIdentifier;
        }
    }
}
