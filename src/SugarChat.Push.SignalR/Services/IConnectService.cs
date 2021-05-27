using SugarChat.Push.SignalR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services
{
    public interface IConnectService
    {
        Task<string> GetConnectionUrl(GetConnectionUrlModel model);
    }
}
