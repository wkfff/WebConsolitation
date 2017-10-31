using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Common.CommonParamForm;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// 017 ПОКАЗАТЕЛИ ФОРМЫ 5-УСН
        /// </summary>
        public DataTable[] GetUFNSReport017Data(Dictionary<string, string> reportParams)
        {
            const int stylesCount = 3;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var paramMark = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramPersons = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamPersons]);
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var marksTable = new MarksYSNTable(QFNS5YSNTotal.Keys.Mark, QFNS5YSNTotal.Keys.MarkBridge);

            // периоды
            var years = GetSelectedYears(reportParams[ReportConsts.ParamYear], false);
            var periods = years.Select(year => GetUNVMonthStart(year, 12)).ToList();

            // виды лиц
            var personsFilter = ParamStore.container[ReportConsts.ParamPersons].BookInfo.GetRowsFilter();
            var tblPersons = dbHelper.GetEntityData(fx_Types_Persons.InternalKey, personsFilter);
            var hPersons = new Hierarchy(tblPersons, fx_Types_Persons.InternalKey);
            hPersons.Sort(fx_Types_Persons.ID);
            var persons = paramPersons == String.Empty
                ? hPersons.ChildAll()
                : hPersons.ChildAll().Where(e => ConvertToIntList(paramPersons).Contains(e.Id)).ToList();
            var allPersonsId = persons.SelectMany(person => person.ToList().Select(e => e.Id)).ToList();

            // получаем данные из т.ф. «ФНС_5 УСН_Сводный» (f.D.FNS5YSNTotal)
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS5YSNTotal.Keys.Day, ConvertToString(periods)),
                new QFilter(QFNS5YSNTotal.Keys.Person, ConvertToString(allPersonsId.Distinct())),
                new QFilter(marksTable.FilterKey, paramMark)
            };
            var groupList = new List<Enum>
            {
                marksTable.FilterKey,
                QFNS5YSNTotal.Keys.Person,
                QFNS5YSNTotal.Keys.Day
            };
            var queryText = new QFNS5YSNTotal().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // параметры отчета
            var rep = new Report(f_D_FNS5YSNTotal.InternalKey)
            {
                AddTotalRow = false,
                RowFilter = null
            };

            // группировка по показателям
            var markGrouping = rep.AddGrouping(f_D_FNS5YSNTotal.RefMarks);
            markGrouping.HierarchyTable = dbHelper.GetEntityData(marksTable.TableKey);
            markGrouping.HideHierarchyLevels = true;
            markGrouping.AddLookupField(marksTable.TableKey, marksTable.Id).Type = typeof(int);
            markGrouping.AddLookupField(marksTable.TableKey, marksTable.Name);
            markGrouping.AddLookupField(marksTable.TableKey, marksTable.RefUnits);
            markGrouping.AddLookupField(marksTable.TableKey, marksTable.Code).Type = typeof(int);
            markGrouping.ViewParams.Add(1, new RowViewParams { BreakSumming = true, Style = 1 });
            markGrouping.ViewParams.Add(2, new RowViewParams { BreakSumming = true, Style = 2, ForAllLevels = true });
            markGrouping.AddSortField(marksTable.TableKey, marksTable.Code);
            markGrouping.AddSortField(marksTable.TableKey, marksTable.Id);
            markGrouping.SetFixedValues(paramMark);

            // настраиваем колонки отчета
            var codeColumn = rep.AddCaptionColumn();
            codeColumn.SetMask(markGrouping, 0, String.Empty);
            codeColumn.SetMask(markGrouping, 1, marksTable.TableKey, marksTable.Code).ForAllLevels = true;
            var nameColumn = rep.AddCaptionColumn();
            nameColumn.SetMask(markGrouping, 0, marksTable.TableKey, marksTable.Name).ForAllLevels = true;
            var refUnitsColumn = rep.AddCaptionColumn();
            refUnitsColumn.SetMask(markGrouping, 0, marksTable.TableKey, marksTable.RefUnits).ForAllLevels = true;

            foreach (var person in persons)
            {
                var fltPerson = filterHelper.RangeFilter(f_D_FNS5YSNTotal.RefTypes,
                                                         ConvertToString(person.ToList().Select(e => e.Id)));
                foreach (var period in periods)
                {
                    var fltPeriod = filterHelper.EqualIntFilter(f_D_FNS5YSNTotal.RefYearDayUNV, period);
                    rep.AddValueColumn(f_D_FNS5YSNTotal.ValueReport, CombineAnd(fltPeriod, fltPerson));
                }
            }

            // формируем таблицу отчета
            rep.ProcessTable(tblData);
            var dt = rep.GetReportData();
            var dividerValue = GetDividerValue(divider);
            var sumColumnsIndexies = GetColumnsList(refUnitsColumn.Index + 1, persons.Count*periods.Count);

            foreach (DataRow row in dt.Rows)
            {
                if (!IsRubRef(row[refUnitsColumn.Index]))
                {
                    row[STYLE] = Convert.ToInt32(row[STYLE]) + stylesCount; // меняем стиль у нерублевых величин
                }
                else if (dividerValue != 1)
                {
                    foreach (var i in sumColumnsIndexies.Where(i => row[i] != DBNull.Value))
                    {
                        row[i] = GetDecimal(row[i]) / dividerValue; // делим рублевые величины
                    }
                }
            }

            dt.Columns.RemoveAt(refUnitsColumn.Index);
            tablesResult[0] = dt;

            // заполняем таблицу колонок
            var dtColumns = new DataTable();
            dtColumns.Columns.Add(NAME);

            foreach (var person in persons)
            {
                dtColumns.Rows.Add(person.Row[fx_Types_Persons.Name]);
            }

            tablesResult[1] = dtColumns;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, ConvertToString(years));
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
