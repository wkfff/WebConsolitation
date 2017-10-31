
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.UFK.ReportMaster;

namespace Krista.FM.Client.Reports.MOFO.Helpers
{
    internal class MOFOAteGrouping : AteGrouping
    {
        private readonly int titleLevel = 0;

        public void InsertArrears(List<ReportRow> rows, int groupingIndex)
        {
            const string tblAteKey = b_Regions_Bridge.InternalKey;
            var typeField = new TableField(tblAteKey, b_Regions_Bridge.RefTerrType);
            var ateTypeIndex = GetLookupFieldIndex(typeField);
            var groupingRows = from row in rows
                               where row.GroupingIndex == groupingIndex &&
                                     (row.Level == 2 || row.Level == 4 ||
                                     (row.Level == 3 && row.LookupFields[ateTypeIndex].Equals(typeMR)))
                               select row;

            if (groupingRows.Count() <= 0)
            {
                return;
            }

            var newRows = InsertMainChild(groupingRows, null, titleLevel, false);
            rows.AddRange(newRows);
        }

        public MOFOAteGrouping(int style)
            : base(style)
        {
            titleLevel = ViewParams.Count;
            ViewParams.Add(titleLevel, new RowViewParams { Style = style + titleLevel });
            ViewParams[titleLevel].BreakSumming = true; // исключаем строки с организациями из суммирования

            Function = delegate(List<ReportRow> rows, int groupingIndex)
            {
                MoveSubjectRow(rows, groupingIndex);
                InsertMRBudjet(rows, groupingIndex);
                InsertArrears(rows, groupingIndex);
            };
        }
    }

    class ReportMOFOHelper
    {
        public static DataTable GetAbsentAteTable(IEnumerable<int> presentAteId)
        {
            var reportHelper = new ReportMonthMethods(ConvertorSchemeLink.scheme);

            // устанавливаем параметры отчета
            var rep = new Report(String.Empty) { RowFilter = null, AddTotalRow = false };

            // группировка по АТЕ
            var ateGrouping = rep.AddGrouping(String.Empty);
            const string regionsTableKey = b_Regions_Bridge.InternalKey;
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            ateGrouping.AddLookupField(regionsTableKey, b_Regions_Bridge.CodeLine);
            var terTypeName = ateGrouping.AddLookupField(regionsTableKey, b_Regions_Bridge.RefTerrType);
            terTypeName.AddField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name);
            ateGrouping.AddLookupField(regionsTableKey, b_Regions_Bridge.Name);
            ateGrouping.AddSortField(regionsTableKey, b_Regions_Bridge.CodeLine);

            // настраиваем колонки отчета
            rep.AddCaptionColumn(ateGrouping, regionsTableKey, b_Regions_Bridge.CodeLine);
            rep.AddCaptionColumn(ateGrouping, terTypeTableKey, fx_FX_TerritorialPartitionType.Name);
            rep.AddCaptionColumn(ateGrouping, regionsTableKey, b_Regions_Bridge.Name);
            rep.AddValueColumn(String.Empty);

            // определяем ГО, по которым нет данных
            var filterCity = Convert.ToString(AteGrouping.typeCity);
            var cityId = reportHelper.GetIDByField(regionsTableKey, b_Regions_Bridge.RefTerrType, filterCity);
            var cities = ReportDataServer.ConvertToIntList(cityId);
            var ate = cities.Where(element => !presentAteId.Contains(element)).ToList();

            // определяем МР, по которым нет данных
            var filterMR = Convert.ToString(AteGrouping.typeMR);
            var mrId = reportHelper.GetIDByField(regionsTableKey, b_Regions_Bridge.RefTerrType, filterMR);
            var MR = ReportDataServer.ConvertToIntList(mrId);
            ate.AddRange(from mr in MR.Where(element => !presentAteId.Contains(element))
                         let nestedId = reportHelper.GetNestedIDByField(regionsTableKey, b_Regions_Bridge.ID, Convert.ToString(mr))
                         where !ReportDataServer.ConvertToIntList(nestedId).Any(element => presentAteId.Contains(element))
                         select mr);

            if (ate.Count() > 0)
            {
                ateGrouping.SetFixedValues(ReportDataServer.ConvertToString(ate));
            }

            return rep.GetReportData();
        }
    }
}
