using System;
using System.Data;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Converts a DataTable to JSON. Note no support for deserialization
    /// </summary>
    public class DataTableConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="dataTable">The value.</param>
        /// <param name="serializer">Сериализатор (значение можно не задавать).</param>
        public override void WriteJson(JsonWriter writer, object dataTable, JsonSerializer serializer)
        {
            DataTable table = dataTable as DataTable;
            DataRowConverter converter = new DataRowConverter();

            writer.WriteStartObject();

            writer.WritePropertyName("Rows");
            writer.WriteStartArray();

            foreach (DataRow row in table.Rows)
            {
                converter.WriteJson(writer, row, serializer);
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
            return typeof(DataTable).IsAssignableFrom(valueType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}