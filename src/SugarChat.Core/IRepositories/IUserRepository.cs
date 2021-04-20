using SugarChat.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.IRepositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task DeleteAsync(Guid userId);
        Task FindAsync(Guid userId);
        Task UpdateAsync(User newUser, Expression<Func<User, object>> updatedFields);
    }
}
