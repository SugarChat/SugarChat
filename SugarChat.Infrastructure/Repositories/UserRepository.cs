using MongoDB.Driver;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Tools;
using SugarChat.Infrastructure.Contexts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SugarChat.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SugarChatDbContext _context;
        public UserRepository(SugarChatDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task AddAsync(User user)
        {
            return _context.User.InsertOneAsync(user);
        }

        public Task UpdateAsync(User newUser, Expression<Func<User, object>> updatedFields)
        {
            var filterBuilder = Builders<User>.Filter.Eq(e => e.Id, newUser.Id);
            UpdateDefinition<User> updateBuilder = Builders<User>.Update.Set(e => e.Id, newUser.Id);
            var visitor = new NewExpressionVisitor<User>(newUser);
            visitor.Visit(updatedFields);
            if (visitor.PropAndValues?.Any() == true)
            {
                foreach (var propAndValue in visitor.PropAndValues)
                {
                    updateBuilder = updateBuilder.Set(propAndValue.Key, propAndValue.Value);
                }
            }
            return _context.User.UpdateOneAsync(filterBuilder, updateBuilder);
        }

        public Task DeleteAsync(Guid userId)
        {
            var filter = Builders<User>.Filter.Eq(e => e.Id, userId);
            return _context.User.DeleteOneAsync(filter);
        }

        public async Task<User> FindAsync(Guid userId)
        {
            var res = await _context.User.FindAsync(e => e.Id == userId);
            return res.FirstOrDefault();
        }
    }
}
