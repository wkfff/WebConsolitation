using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms
{
    public class ReportBuilder : IReportBuilder
    {
        public void CreateReport(D_CD_Task task)
        {
            var reportForm = Resolver.GetAll<IReportForm>().Where(x => x.ID == task.RefTemplate.Class).FirstOrDefault();
            if (reportForm == null)
            {
                throw new InvalidOperationException("Не найдена реализация формы с классом \"{0}\".".FormatWith(task.RefTemplate.Class));
            }

            reportForm.CreateReport(task);
        }
    }
}
