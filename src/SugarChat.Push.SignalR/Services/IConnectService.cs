using SugarChat.Push.SignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public interface IConnectService
    {
        Task<string> GetConnectionUrl(string userIdentifier);
    }
}
