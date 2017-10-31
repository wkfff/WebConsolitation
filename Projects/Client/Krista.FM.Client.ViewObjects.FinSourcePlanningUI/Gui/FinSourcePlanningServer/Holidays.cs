using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
    public class Holidays
    {
        private DataTable dtHolydays;

        public Holidays(IScheme scheme)
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

        /// <summary>
        /// находим рабочий день с указанной датой или следующий рабочий день за ним, если указанный день - выходной
        /// </summary>
        /// <param name="date"></param>
        /// <returns>true если указанный день рабочий</returns>
        public bool GetWorkDate(ref DateTime date)
        {
            DataRow[] rows = dtHolydays.Select(string.Format("DayoffDate = '{0}'", date));
            // если у нас следующий день в справочнике выходных помечен как рабочий, отдаем его
            if (rows.Length > 0 && !Convert.ToBoolean(rows[0]["DayOff"]))
                return true;

            while ((rows.Length != 0 && Convert.ToBoolean(rows[0]["DayOff"])) ||
                (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday))
            {
                date = date.AddDays(1);
                rows = dtHolydays.Select(string.Format("DayoffDate = '{0}'", date));
            }

            return false;
        }

        /// <summary>
        /// получаем рабочий день с учетом коррекции
        /// </summary>
        /// <returns>true если указанный день рабочий</returns>
        public bool GetWorkDate(DayCorrection dayCorrection, ref DateTime date)
        {
            if (dayCorrection == DayCorrection.NoCorrection)
                return true;
            DataRow[] rows = dtHolydays.Select(string.Format("DayoffDate = '{0}'", date));
            // если у нас следующий день в справочнике выходных помечен как рабочий, отдаем его
            if (rows.Length > 0 && !Convert.ToBoolean(rows[0]["DayOff"]))
                return true;

            while ((rows.Length != 0 && Convert.ToBoolean(rows[0]["DayOff"])) ||
                (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday))
            {
                date = date.AddDays(Convert.ToInt32(dayCorrection));
                rows = dtHolydays.Select(string.Format("DayoffDate = '{0}'", date));
            }

            return false;
        }
    }
}
