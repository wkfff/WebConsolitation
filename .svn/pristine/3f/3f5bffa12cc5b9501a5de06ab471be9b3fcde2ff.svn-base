using System.Collections.Generic;
using System.Data;

namespace Krista.FM.Client.Reports.UFK14.Helpers
{
    class ParamUFKHelper
    {
        public const string KVSR = "KVSR";
        public const string KD = "KD";
        public const string STARTDATE = "STARTDATE";
        public const string ENDDATE = "ENDDATE";
        public const string NOW = "NOW";
        public const string MEASURE = "MEASURE";
        public const string REGION_LIST_TYPE = "REGION_LIST_TYPE";
        public const string BDGT_LEVEL = "BDGT_LEVEL";
        public const string BDGT_NAME = "BDGT_NAME";
        public const string PRECISION = "PRECISION";
        public const string YEARS = "YEARS";
        public const string YEAR = "YEAR";
        public const string INN = "INN";
        public const string COUNT = "COUNT";
        public const string SUMM = "SUMM";
        public const string SETTLEMENT = "SETTLEMENT";
        public const string OKVED = "OKVED";
        public const string DGROUP = "DGROUP";
        public const string ARREARS = "ARREARS";
        public const string MONTH = "MONTH";
        public const string CONTRACT = "CONTRACT";
        public const string QUARTER = "QUARTER";
        public const string STRUCTURE = "STRUCTURE";
        public const string VARIANT = "VARIANT";
        public const string MARK = "MARK";
        public const string HALF_YEAR = "HALF_YEAR";
        public const string PROFIT = "PROFIT";
        public const string PERSON = "PERSON";
        public const string COLUMNS = "COLUMNS";

        private DataRow row;

        public ParamUFKHelper(DataRow rowParams)
        {
            row = rowParams;
        }

        public ParamUFKHelper(IList<DataTable> tableList)
        {
            row = tableList[tableList.Count - 1].Rows[0];
        }

        private static readonly Dictionary<string, int> paramIndexer = new Dictionary<string, int>()
        {
            { KD, 0 },
            { STARTDATE, 1 },
            { ENDDATE, 2 },
            { NOW, 3 },
            { MEASURE, 4 },
            { REGION_LIST_TYPE, 5 },
            { BDGT_LEVEL, 6 },
            { BDGT_NAME, 7 },
            { PRECISION, 8 },
            { YEARS, 9 },
            { YEAR, 10 },
            { INN, 11 },
            { KVSR, 12 },
            { COUNT, 13 },
            { SUMM, 14 },
            { SETTLEMENT, 15},
            { OKVED, 16},
            { DGROUP, 17},
            { ARREARS, 18},
            { MONTH, 19},
            { CONTRACT, 20},
            { QUARTER, 21},
            { STRUCTURE, 22},
            { VARIANT, 23},
            { MARK, 24},
            { HALF_YEAR, 25},
            { PROFIT, 26},
            { PERSON, 27},
            { COLUMNS, 28}
        };

        public static Dictionary<string, int> GetParamList()
        {
            return paramIndexer;
        }

        public static int GetParamIndex(string paramName)
        {
            if (paramIndexer.ContainsKey(paramName))
            {
                return paramIndexer[paramName];
            }

            return -1;
        }

        public void SetParamValue(string paramName, object paramValue)
        {
            var paramIndex = GetParamIndex(paramName);

            if (paramIndex >= 0)
            {
                row[paramIndex] = paramValue;
            }
        }

        public object GetParamValue(string paramName)
        {
            var paramIndex = GetParamIndex(paramName);

            if (paramIndex >= 0)
            {
                return row[paramIndex];
            }

            return null;
        }

        public static object GetParamValue(IList<DataTable> tableList, string paramName)
        {
            var paramIndex = GetParamIndex(paramName);

            if (paramIndex < 0 || tableList == null || tableList.Count == 0)
            {
                return null;
            }

            var dt = tableList[tableList.Count - 1];
            
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return dt.Rows[0][paramIndex];
        }

    }
}
