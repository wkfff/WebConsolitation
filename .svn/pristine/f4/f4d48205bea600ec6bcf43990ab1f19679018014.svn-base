using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Krista.FM.Client.Reports.UFK14.Helpers
{
    public class ReportUFKHelper
    {
        public const decimal LimitSumRate = 1000000;
        // структуры
        public const int StructureNot = 0;      // не структура
        public const int StructureMO = 1;       // структура субъекта
        public const int StructureNotMO = 2;    // структура другого субъекта
        // показатели
        public const int MarksInpaymentsReturn = 2; // Возвращено платежей
        public const int MarksInpaymentsTransfer = 3; // Перечислено в бюджет поступлений

        public static List<int> GetSelectedID(string selectedID, string tableKey)
        {
            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            var result = ReportDataServer.ConvertToIntList(selectedID);

            if (selectedID == String.Empty)
            {
                var source = dbHelper.GetActiveSource(tableKey);
                var filter = source != -1
                                 ? String.Format("sourceid = {0} and id <> -1", source)
                                 : String.Format("id <> -1");
                var tblKD = dbHelper.GetEntityData(tableKey, filter);
                result = tblKD.Rows.Cast<DataRow>().Select(row => Convert.ToInt32(row["id"])).ToList();
            }

            if (result.Count == 0)
            {
                result.Add(Convert.ToInt32(ReportConsts.UndefinedKey));
            }

            return result;
        }
    }
}
