using MongoDB.Bson;
using MongoDB.Driver;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Tools;
using SugarChat.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task AddAsync(User user)
        {
            return _context.Collection.InsertOneAsync(user);
        }

        public Task UpdateAsync(User newUser, Expression<Func<User, object>> updatedFields)
        {
            var filterBuilder = Builders<User>.Filter.Eq(e => e.Id, newUser.Id);
            UpdateDefinition<User> updateBuilder = Builders<User>.Update.Unset(e => e.Id);
            var visitor = new NewExpressionVisitor<User>(newUser);
            visitor.Visit(updatedFields);
            if (visitor.PropAndValues?.Any() == true)
            {
                foreach (var propAndValue in visitor.PropAndValues)
                {
                    updateBuilder.Set(propAndValue.Key, propAndValue.Value);
                }
            }
            return _context.Collection.UpdateOneAsync(filterBuilder, updateBuilder);
        }

        public Task DeleteAsync(Guid userId)
        {
            var filter = Builders<User>.Filter.Eq(e => e.Id, userId);
            return _context.Collection.DeleteOneAsync(filter);
        }

        public Task FindAsync(Guid userId)
        {
            var filter = Builders<User>.Filter.Eq(e => e.Id, userId);
            return _context.Collection.FindAsync(filter);
        }
    }
}
