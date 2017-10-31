using System;
using System.Data;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Converts a <see cref="DataRow"/> object to and from JSON.
    /// </summary>
    public class DataRowConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="dataRow">Значение сериализуемого объекта.</param>
        /// <param name="serializer">Сериализатор (значение можно не задавать).</param>
        public override void WriteJson(JsonWriter writer, object dataRow, JsonSerializer serializer)
        {
            DataRow row = dataRow as DataRow;

            // *** HACK: need to use root serializer to write the column value
            //     should be fixed in next ver of JSON.NET with writer.Serialize(object)
            JsonSerializer ser = new JsonSerializer();

            writer.WriteStartObject();
            foreach (DataColumn column in row.Table.Columns)
            {
                writer.WritePropertyName(column.ColumnName.ToUpper());
                if (row[column] is DateTime && row[column] != DBNull.Value)
                {
                    ser.Serialize(writer, row[column].ToString());
                }
                else
                {
                    ser.Serialize(writer, row[column]);
                }
            }

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
            return typeof(DataRow).IsAssignableFrom(valueType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}