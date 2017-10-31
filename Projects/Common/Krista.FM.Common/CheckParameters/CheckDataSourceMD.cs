using System;
using System.Data;

namespace Krista.FM.Common
{
    /// <summary>
    /// Проверка имени реляционной базы, на которую настроена многомерка
    /// </summary>
    class CheckDataSourceMD : CheckRule
    {
        public CheckDataSourceMD(string parameter, string value, string errorMessage, bool invalid)
            : base(parameter, value, errorMessage, invalid)
        { 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="table"></param>
        public static void CheckMDDataSource(CheckRule rule, DataTable table)
        {
            DataRow[] rowsMD = table.Select(String.Format("uniqueName = '{0}'", rule.Parametr));
            DataRow[] rowsDWH = table.Select(String.Format("uniqueName = '{0}'", "DBName"));

            if (rowsMD.Length != 0 && rowsDWH.Length != 0)
            {
                if (!rowsMD[0]["value"].ToString().ToUpper().Equals(rowsDWH[0]["value"].ToString().ToUpper()))
                {
                    rowsMD[0]["suspect"] = true;
                    rowsMD[0]["suspectDescription"] = rule.ErrorMessage;
                }
            }
        }
    }
}
