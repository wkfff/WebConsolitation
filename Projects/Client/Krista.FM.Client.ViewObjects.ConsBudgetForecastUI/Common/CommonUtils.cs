using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common
{
    public class CommonUtils
    {
        public static int GetSourceId(int year, IDatabase db)
        {
            DataTable dt =
                (DataTable)
                db.ExecQuery(
                    "select ID from DataSources where SupplierCode = 'ФО' and DataCode = 29 and Year = ? and deleted = 0",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", year));
            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0][0]);
            return -1;
        }

    }
}
