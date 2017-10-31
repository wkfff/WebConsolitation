using System;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public abstract class Report
    {
        public abstract string TemplateName { get; }

        public DateTime ReportDate { get; set; }

        public abstract void Create(string templateDocumentName, int currentVariantId, int regionId, DateTime calculateDate, D_S_TitleReport titleReport);
    }
}
