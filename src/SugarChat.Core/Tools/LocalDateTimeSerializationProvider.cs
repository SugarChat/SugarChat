using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace SugarChat.Core.Tools
{
    public class LocalDateTimeSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            return type == typeof(DateTime) ? DateTimeSerializer.LocalInstance : null;
        }
    }
}
