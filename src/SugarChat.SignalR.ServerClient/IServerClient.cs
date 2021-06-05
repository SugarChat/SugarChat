using SugarChat.SignalR.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.SignalR.ServerClient
{
    public interface IServerClient
    {

        Task<string> GetConnectionUrl(string userIdentifier, bool isInterior = false);

        Task Group(GroupActionModel model);

        Task SendMessage(SendMessageModel model);

        Task SendMassMessage(SendMassMessageModel model);

        Task SendCustomMessage(SendCustomMessageModel model);

        Task SendMassCustomMessage(SendMassCustomMessageModel model);
    }
}
