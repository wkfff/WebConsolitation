using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core
{
    public class CubeInfo
    {
        #region Поля

        private DataTable dateDT;
        private string name;
        private DataProvider provider;
        private string lastDateQueryId;
        private DateTime lastDate;

        private Dictionary<string, CubeDimensionInfo> dimensions;

        #endregion

        #region Свойства

        public DateTime LastDate
        {
            get
            {
                if (lastDate == DateTime.MinValue)
                {
                    FillLastDate();
                }
                return lastDate;
            }
        }

        #endregion

        public CubeInfo(DataProvider cubeProvider, string cubeName, string lastDateQueryName)
        {
            name = cubeName;
            provider = cubeProvider;
            lastDateQueryId = lastDateQueryName;

            dimensions = new Dictionary<string, CubeDimensionInfo>();
            lastDate = DateTime.MinValue;
        }

        public void AddDimensionInfo(string dimensionName, string queryName)
        {
            if (!dimensions.ContainsKey(dimensionName))
            {
                dimensions.Add(dimensionName, new CubeDimensionInfo(provider, dimensionName, queryName));
            }
        }

        public string GetDimensionElement(string dimensionName, string elementCode)
        {
            if (dimensions.ContainsKey(dimensionName))
            {
                return dimensions[dimensionName].GetDimensionElement(elementCode);
            }
            return String.Empty;
        }

        public void FillLastDate()
        {
            lastDate = GetLastDate(provider, lastDateQueryId, true);
        }

        public static DateTime GetLastDate(DataProvider dataProvider, string queryId)
        {
            return GetLastDate(dataProvider, queryId, false);
        }

        public static DateTime GetLastDate(DataProvider dataProvider, string queryId, bool useBasePath)
        {
            return GetLastDate(dataProvider, queryId, useBasePath, 0);
        }

        public static DateTime GetLastDate(DataProvider dataProvider, string queryId, bool useBasePath, int rowIndex)
        {
            DataTable dateDT = new DataTable();
            string query = useBasePath ? DataProvider.GetQueryText(queryId, CRHelper.BasePath) : DataProvider.GetQueryText(queryId);
            dataProvider.GetDataTableForChart(query, "Dummy", dateDT);

            if (dateDT.Rows.Count > rowIndex)
            {
                if (dateDT.Rows[rowIndex][1] != DBNull.Value && dateDT.Rows[rowIndex][1].ToString() != String.Empty)
                {
                   // CRHelper.SaveToErrorLog(queryId + " - " + dateDT.Rows[rowIndex][1]);
                    return CRHelper.PeriodDayFoDate(dateDT.Rows[rowIndex][1].ToString());
                }
            }

            return new DateTime(2020, 1, 1);
        }
    }
}