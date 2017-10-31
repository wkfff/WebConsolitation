using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core
{
    public class CubeDimensionInfo
    {
        #region Поля

        private Dictionary<string, string> dimensionElements;
        private string name;
        private string queryId;
        private DataProvider provider;
        private DataTable dt;

        private const string ElementCaptionColumnName = "Элемент измерения";
        private const string ElementUniqueNameColumnName = "Уникальное имя";
        private const string ElementCodeColumnName = "Код";

        #endregion

        #region Свойства

        public Dictionary<string, string> DimensionElements
        {
            get
            {
                if (dimensionElements == null || dimensionElements.Count == 0)
                {
                    FillDimensionInfo(queryId);
                }
                return dimensionElements;
            }
        }

        #endregion

        public CubeDimensionInfo(DataProvider providerName, string dimensionName, string queryName)
        {
            name = dimensionName;
            queryId = queryName;
            provider = providerName;
        }

        private void FillDimensionInfo(string queryName)
        {
            dimensionElements = new Dictionary<string, string>();

            dt = new DataTable();
            string query = DataProvider.GetQueryText(queryName, CRHelper.BasePath);
            provider.GetDataTableForChart(query, ElementCaptionColumnName, dt);

            foreach (DataRow row in dt.Rows)
            {
                if (dt.Columns.Contains(ElementUniqueNameColumnName))
                {
                    string elementUniqueName = row[ElementUniqueNameColumnName].ToString();
                    if (row[ElementCodeColumnName] != DBNull.Value)
                    {
                        string code = row[ElementCodeColumnName].ToString();
                        AddUniqueDictionaryPair(dimensionElements, code, elementUniqueName);
                    }
                }
            }
        }

        public string GetDimensionElement(string elementCode)
        {
            if (DimensionElements.ContainsKey(elementCode))
            {
                return DimensionElements[elementCode];
            }
            return String.Empty;
        }

        private static string GetDictionaryUniqueKey(Dictionary<string, string> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }

        private static void AddUniqueDictionaryPair(Dictionary<string, string> dictionary, string key, string value)
        {
            string uniqueKey = GetDictionaryUniqueKey(dictionary, key);
            dictionary.Add(uniqueKey, value);
        }
    }
}
