using System;
using System.Collections.Generic;

using Ext.Net;

using Krista.FM.Extensions;

using Newtonsoft.Json;

namespace Krista.FM.RIA.Core.Helpers.JsonConverters
{
    /// <summary>
    /// Converts a Dictionary of (string, object) object to and from JSON.
    /// </summary>
    public class DictionaryConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="dictionary">Значение сериализуемого объекта.</param>
        /// <param name="serializer">Сериализатор (значение можно не задавать).</param>
        public override void WriteJson(JsonWriter writer, object dictionary, JsonSerializer serializer)
        {
            Dictionary<string, object> dic = dictionary as Dictionary<string, object>;

            // *** HACK: need to use root serializer to write the column value
            //     should be fixed in next ver of JSON.NET with writer.Serialize(object)
            JsonSerializer ser = new JsonSerializer();

            writer.WriteStartObject();
            char comma = ' ';
            foreach (KeyValuePair<string, object> pair in dic)
            {
                if (typeof(Field).IsAssignableFrom(pair.Value.GetType()))
                {
                    writer.WriteRaw("{0} {1}: {2}".FormatWith(comma, pair.Key, GetEditor((Field)pair.Value)));
                }
                else
                if (pair.Key.Equals("renderer"))
                {
                    writer.WriteRaw("{0} {1}: {2}".FormatWith(comma, pair.Key, pair.Value.ToString()));
                }
                else
                if (typeof(string).IsAssignableFrom(pair.Value.GetType()))
                {
                    writer.WriteRaw("{0} {1}: '{2}'".FormatWith(comma, pair.Key, pair.Value));
                }
                else
                if (typeof(bool).IsAssignableFrom(pair.Value.GetType()))
                {
                    writer.WriteRaw("{0} {1}: {2}".FormatWith(comma, pair.Key, pair.Value.ToString().ToLower()));
                }
                else
                {
                    writer.WriteRaw("{0} {1}: {2}".FormatWith(comma, pair.Key, pair.Value));
                }

                comma = ',';
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
            return typeof(Dictionary<string, object>).IsAssignableFrom(valueType);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static string GetEditor(Field field)
        {
            if (field is Ext.Net.TextField) 
            {
                return "new Ext.form.TextField({{allowBlank: {0} }})"
                    .FormatWith(((Ext.Net.TextField)field).AllowBlank.ToString().ToLower());
            }

            if (field is Ext.Net.NumberField)
            {
                return "new Ext.form.NumberField({{allowBlank: {0}, allowDecimals: {1} }})"
                    .FormatWith(
                        ((Ext.Net.NumberField)field).AllowBlank.ToString().ToLower(), 
                        ((Ext.Net.NumberField)field).AllowDecimals.ToString().ToLower());
            }

            return "new Ext.form.TextField({})";
        }
    }
}
