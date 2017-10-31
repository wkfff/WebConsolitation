using System;
using System.Data;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Converts a <see cref="DataSet"/> object to JSON. No support for reading.
    /// </summary>
    public class DataSetConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="dataset">The value.</param>
        /// <param name="serializer">Сериализатор (значение можно не задавать).</param>
        public override void WriteJson(JsonWriter writer, object dataset, JsonSerializer serializer)
        {
            DataSet dataSet = dataset as DataSet;

            DataTableConverter converter = new DataTableConverter();

            writer.WriteStartObject();

            writer.WritePropertyName("Tables");
            writer.WriteStartArray();

            foreach (DataTable table in dataSet.Tables)
            {
                converter.WriteJson(writer, table, serializer);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Determines whether this instance can convert the specified value type.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>
        ///     <c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type valueType)
        {
            return typeof(DataSet).IsAssignableFrom(valueType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
