using System;

using Newtonsoft.Json;

namespace Krista.FM.Domain.Serialization.Json
{
    public class ReferenceIdJsonConverter<T> : JsonConverter where T : DomainObject
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((T)value).ID);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(DomainObject));
        }
    }
}
