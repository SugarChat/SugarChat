using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SugarChat.Core.Domain;
using SugarChat.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Infrastructure.Contexts
{
    public class MessageContext : Context<Message>
    {
        public MessageContext(IOptions<MongoDbSettings> options) : base(options)
        {
        }
        public override IMongoCollection<Message> Collection
        {
            get
            {
                return Database.GetCollection<Message>("Message");
            }
        }
    }
}
