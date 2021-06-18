using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Services
{
    public interface ISecurityManager : IService
    {
        Task<bool> IsSupperAdmin();
    }

    public class SecurityManager : ISecurityManager
    {
        public Task<bool> IsSupperAdmin()
        {
            return Task.FromResult(true);
        }
    }
}
