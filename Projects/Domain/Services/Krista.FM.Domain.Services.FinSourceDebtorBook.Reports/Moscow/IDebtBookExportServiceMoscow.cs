using System;
using System.Data;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public interface IDebtBookExportServiceMoscow
    {
        DataTable[] GetDebtBookMoscowData(int refVariant, int refRegion, DateTime calculateDate);
    }
}
