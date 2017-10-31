using System;
using System.Data;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public interface IDebtBookExportServiceStavropol
    {
        RecordType GetRecordType(object value);

        DataTable[] GetDebtBookStavropolData(int refVariant, int refRegion, DateTime calculateDate);
    }
}
