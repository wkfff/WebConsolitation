using System;
using System.Data;
using Krista.FM.Common.OfficeHelpers;

namespace Krista.FM.Common.ReportHelpers
{
    /// <summary>
    /// общий класс для всех отчетов
    /// </summary>
    public abstract class ReportsHelper : DisposableObject
    {
        public virtual void CreateReport()
        {
        }

        public virtual void CreateReport(object reportParam, string templateName)
        {
        }

        public virtual void CreateReport(string templateName, DataTable dtReportData)
        {
        }

        public virtual OfficeDocument CreateReport(string templateName, DataTable[] tables)
        {
            return null;
        }

        public abstract void ShowReport();

        public abstract void Save(string fileName);

        public abstract void Quit();

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    // Вызываем сборщик мусора для немедленной очистки памяти
                    GC.GetTotalMemory(true);
                }
            }
        }
    }
}
