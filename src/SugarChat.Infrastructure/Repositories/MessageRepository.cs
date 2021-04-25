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
    public class MessageRepository : IMessageRepository
    {
        private readonly SugarChatDbContext _context;
        public MessageRepository(SugarChatDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task AddAsync(Message user)
        {
            return _context.Message.InsertOneAsync(user);
        }

        public Task UpdateAsync(Message newMessage, Expression<Func<Message, object>> updatedFields)
        {
            var filterBuilder = Builders<Message>.Filter.Eq(e => e.Id, newMessage.Id);
            UpdateDefinition<Message> updateBuilder = Builders<Message>.Update.Set(e => e.Id, newMessage.Id);
            var visitor = new NewExpressionVisitor<Message>(newMessage);
            visitor.Visit(updatedFields);
            if (visitor.PropAndValues?.Any() == true)
            {
                foreach (var propAndValue in visitor.PropAndValues)
                {
                    updateBuilder = updateBuilder.Set(propAndValue.Key, propAndValue.Value);
                }
            }
            return _context.Message.UpdateOneAsync(filterBuilder, updateBuilder);
        }

        public Task DeleteAsync(Guid userId)
        {
            var filter = Builders<Message>.Filter.Eq(e => e.Id, userId);
            return _context.Message.DeleteOneAsync(filter);
        }

        public async Task<Message> FindAsync(Guid userId)
        {
            var message = await _context.Message.FindAsync(e => e.Id == userId);
            return message.FirstOrDefault();
        }
    }
}
