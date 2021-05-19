using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Data.MongoDb.Tools
{
    public class LocalDateTimeSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTimeOffset) ? DateTimeSerializer.LocalInstance : null;
        }
    }
}
