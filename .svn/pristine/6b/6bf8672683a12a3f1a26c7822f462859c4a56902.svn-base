using System.Text;
using Krista.FM.Server.Dashboards.SgmSupport;
using System;

namespace Krista.FM.Server.Dashboards.reports.SGM
{
    public class SGMSQLTexts
    {
        /// <summary>
        /// Фильтр, оставляющий только ФО
        /// </summary>
        /// <returns>Строка фильтра</returns>
        public string GetFOFilter()
        {
            return "(L_A1 = 1) and (COD <> 0) and (KOD <> 999)";
        }

        #region SGM_0016_SQL

        public string GetDeseaseSQLText_SGM0016(string subjectCodes, int year)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append("select sum(m.cc), m.yr, m.inf, m.vozr, m.priv ");
            strBuilder.Append("from ");
            strBuilder.Append(" {0}\\{1} m ");
            strBuilder.Append("where ");
            strBuilder.Append(" m.area in ({2}) ");
            strBuilder.Append(" and m.vozr in (2,3,8,16,37,38,39,30,15,7,32) ");
            strBuilder.Append(" and m.priv in (1,22,80,82,24,26,30,20,44,99,84) ");
            strBuilder.Append(" and m.yr > {3} and m.yr < {4} ");
            strBuilder.Append("group by m.yr, m.inf, m.vozr, m.priv ");

            var dataRotator = new SGMDataRotator();
            return string.Format(strBuilder.ToString(),
                dataRotator.PathIllData, dataRotator.ClsPrivYearData, subjectCodes,
                year - 5, year + 1);
        }

        #endregion

        #region SGM_0017_SQL

        public string GetDeseaseSQLText_SGM0017(string deseaseCode,
            int year, string subjectCodes)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append("select sum(m.cc), m.priv, m.vozr, m.area ");
            strBuilder.Append("from ");
            strBuilder.Append(" {0}\\{1} m ");
            strBuilder.Append("where ");
            strBuilder.Append(" m.inf = {2} and m.yr = {3}  and m.area in ({4}) ");
            strBuilder.Append(" and m.vozr in (2,3,8,16,37,38,39,30,15,7,32) ");
            strBuilder.Append(" and m.priv in (1,22,80,82,24,26,30,20,44,99,84) ");
            strBuilder.Append("group by m.priv, m.vozr, m.area ");

            var dataRotator = new SGMDataRotator();

            return String.Format(strBuilder.ToString(),
                dataRotator.PathIllData, dataRotator.ClsPrivYearData,
                deseaseCode, year, subjectCodes);
        }

        public string GetAreaCaptionsSQLText_SGM0017()
        {
            var dataRotator = new SGMDataRotator();
            return string.Format("select a.name_mem, a.kod from {0}\\{1} a",
                dataRotator.PathClsData, SGMDataRotator.ClsAreaFull);
        }

        #endregion

        #region SGM_0018_SQL

        public string GetDeseaseSQLText_SGM0018(string subjectCodes, int year)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append("select sum(m.cc), m.yr, m.inf, m.vozr, m.priv, m.area ");
            strBuilder.Append("from ");
            strBuilder.Append(" {0}\\{1} m ");
            strBuilder.Append("where ");
            strBuilder.Append(" m.area in ({2}) ");
            strBuilder.Append(" and m.vozr in (2,3,7,8,16,15,30,37,38,39) ");
            strBuilder.Append(" and m.priv in (1,22,80,82,24,26,30,20,44,99,84) ");
            strBuilder.Append(" and m.inf in (11,10,4,18,5,6,30,99) ");
            strBuilder.Append(" and m.yr = {3} ");            
            strBuilder.Append("group by m.yr, m.inf, m.vozr, m.priv, m.area ");
            
            var dataRotator = new SGMDataRotator();

            return string.Format(strBuilder.ToString(),
                dataRotator.PathIllData, dataRotator.ClsPrivYearData, subjectCodes, year);
        }

        #endregion
    }
}
