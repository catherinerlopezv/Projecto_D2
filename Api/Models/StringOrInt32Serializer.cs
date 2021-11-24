using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Api.Models
{
    public sealed class StringOrInt32Serializer : IBsonSerializer
    {
        public Type ValueType { get; } = typeof(string);

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
            if (context.Reader.CurrentBsonType == BsonType.Int32) return GetNumberValue(context);

            return context.Reader.ReadString();
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value) {
            context.Writer.WriteString(value as string);
        }

        private static object GetNumberValue(BsonDeserializationContext context) {
            var value = context.Reader.ReadInt32();

            return value.ToString();
        }
    }
}
