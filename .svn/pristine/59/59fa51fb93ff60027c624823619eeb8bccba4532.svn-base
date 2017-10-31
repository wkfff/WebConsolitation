using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Krista.FM.RIA.Core
{
    public static class JsonDataSetParser
    {
        public static Dictionary<string, List<Dictionary<string, object>>> Parse(string sourceData)
        {
            JsonTextReader reader = new JsonTextReader(new System.IO.StringReader(sourceData));

            var dataSet = new Dictionary<string, List<Dictionary<string, object>>>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }

                // Читаем наименование таблицы
                if (reader.TokenType == JsonToken.StartObject)
                {
                    reader.Read();
                }

                string tableName = Convert.ToString(reader.Value);

                dataSet.Add(tableName, ReadTable(reader));
            }

            return dataSet;
        }

        private static List<Dictionary<string, object>> ReadTable(JsonReader reader)
        {
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }

                table.Add(ReadRow(reader));
            }

            return table;
        }

        private static Dictionary<string, object> ReadRow(JsonReader reader)
        {
            Dictionary<string, object> row = new Dictionary<string, object>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartArray || reader.TokenType == JsonToken.StartObject)
                {
                    continue;
                }

                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }

                string colName = Convert.ToString(reader.Value);
                reader.Read();
                row.Add(colName, reader.Value);
            }

            return row;
        }
    }
}