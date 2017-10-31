using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    public class Holydays
    {
        private DataTable dtHolydays;

        public Holydays(IScheme scheme)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Fact_Holidays);
            dtHolydays = new DataTable();
            using (IDataUpdater du = entity.GetDataUpdater())
            {
                du.Fill(ref dtHolydays);
            }
        }

        /// <summary>
        /// возвращает невыходной день, который является либо указанной датой, либо следующий невыходной день 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime GetWorkDate(DateTime date)
        {
            DateTime nextPeriod = date.AddDays(0);

            DataRow[] rows = dtHolydays.Select(string.Format("DayoffDate = '{0}'", nextPeriod));
            // если у нас следующий день в справочнике выходных помечен как рабочий, отдаем его
            if (rows.Length > 0 && !Convert.ToBoolean(rows[0]["DayOff"]))
                return date;

            while ((rows.Length != 0 && Convert.ToBoolean(rows[0]["DayOff"])) ||
                (nextPeriod.DayOfWeek == DayOfWeek.Sunday || nextPeriod.DayOfWeek == DayOfWeek.Saturday))
            {
                nextPeriod = nextPeriod.AddDays(1);
                rows = dtHolydays.Select(string.Format("DayoffDate = '{0}'", nextPeriod));
            }

            return nextPeriod.AddDays(0);
        }
    }
}
