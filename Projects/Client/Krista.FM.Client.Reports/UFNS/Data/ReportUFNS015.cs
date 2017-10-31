using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        /// 014 ПОКАЗАТЕЛИ ФОРМЫ 5-ДДК
        /// </summary>
        public DataTable[] GetUFNSReport015Data(Dictionary<string, string> reportParams)
        {
            const int stylesCount = 2;
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var paramMark = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamMark]);
            var paramIncomes = reportParams[ReportConsts.ParamIncomes];
            var paramPersons = reportParams[ReportConsts.ParamPersons];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var marksTable = new MarksDDKTable(QFNS5DDKTotal.Keys.Mark, QFNS5DDKTotal.Keys.MarkBridge);

            // периоды
            var years = GetSelectedYears(reportParams[ReportConsts.ParamYear], false);
            var periods = years.Select(year => GetUNVYearPlanLoBound(year)).ToList();

            // виды доходов
            var fltIncomesAll = filterHelper.EqualIntFilter(f_D_FNS5DDKTotal.RefTypesIncomes, fx_Types_Incomes.All);
            var incomesId = new List<int> { fx_Types_Incomes.All };
            if (paramIncomes == String.Empty)
            {
                paramIncomes = ReportConsts.UndefinedKey;
            }

            var tblIncomes = dbHelper.GetEntityData(fx_Types_Incomes.InternalKey,
                                                    filterHelper.RangeFilter(fx_Types_Incomes.ID, paramIncomes));
            incomesId.AddRange(from DataRow row in tblIncomes.Rows
                               select Convert.ToInt32(row[fx_Types_Incomes.ID]));

            // виды лиц
            var fltPeople = filterHelper.EqualIntFilter(f_D_FNS5DDKTotal.RefTypes, fx_Types_Persons.People);
            var personsId = new List<int> { fx_Types_Persons.People };
            if (paramPersons == String.Empty)
            {
                paramPersons = ReportConsts.UndefinedKey;
            }
            var tblPersons = dbHelper.GetEntityData(fx_Types_Persons.InternalKey,
                                                    filterHelper.RangeFilter(fx_Types_Persons.ID, paramPersons));
            personsId.AddRange(from DataRow row in tblPersons.Rows
                               select Convert.ToInt32(row[fx_Types_Persons.ID]));

            // получаем данные из т.ф. «ФНС_5 ДДК_Сводный» (f.D.FNS5DDKTotal)
            var filterList = new List<QFilter>
            {
                new QFilter(QFNS5DDKTotal.Keys.Day, ConvertToString(periods)),
                new QFilter(QFNS5DDKTotal.Keys.Income, ConvertToString(incomesId)),
                new QFilter(QFNS5DDKTotal.Keys.Person, ConvertToString(personsId)),
                new QFilter(marksTable.FilterKey, paramMark)
            };
            var groupList = new List<Enum>
            {
                marksTable.FilterKey,
                QFNS5DDKTotal.Keys.Income,
                QFNS5DDKTotal.Keys.Person,
                QFNS5DDKTotal.Keys.Day
            };
            var queryText = new QFNS5DDKTotal().GetQueryText(filterList, groupList);
            var tblData = dbHelper.GetTableData(queryText);

            // параметры отчета
            var rep = new Report(f_D_FNS5DDKTotal.InternalKey)
            {
                AddTotalRow = false,
                RowFilter = null
            };

            // группировка по показателям
            var markGrouping = rep.AddGrouping(f_D_FNS5DDKTotal.RefMarks);
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

            foreach (DataRow row in tblPersons.Rows)
            {
                var fltPerson = filterHelper.EqualIntFilter(f_D_FNS5DDKTotal.RefTypes,
                                                            Convert.ToInt32(row[fx_Types_Persons.ID]));
                foreach (var period in periods)
                {
                    var fltPeriod = filterHelper.EqualIntFilter(f_D_FNS5DDKTotal.RefYearDayUNV, period);
                    rep.AddValueColumn(f_D_FNS5DDKTotal.Value, CombineAnd(fltPeriod, fltPerson, fltIncomesAll));
                }
            }

            foreach (DataRow row in tblIncomes.Rows)
            {
                var fltIncomes = filterHelper.EqualIntFilter(f_D_FNS5DDKTotal.RefTypesIncomes,
                                                            Convert.ToInt32(row[fx_Types_Incomes.ID]));
                foreach (var period in periods)
                {
                    var fltPeriod = filterHelper.EqualIntFilter(f_D_FNS5DDKTotal.RefYearDayUNV, period);
                    rep.AddValueColumn(f_D_FNS5DDKTotal.Value, CombineAnd(fltPeriod, fltIncomes, fltPeople));
                }
            }

            // формируем таблицу отчета
            rep.ProcessTable(tblData);
            var dt = rep.GetReportData();
            var dividerValue = GetDividerValue(divider);
            var sumColumnsIndexies = GetColumnsList(refUnitsColumn.Index + 1,
                                                    (tblPersons.Rows.Count + tblIncomes.Rows.Count)*periods.Count);
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

            foreach (DataRow row in tblPersons.Rows)
            {
                dtColumns.Rows.Add(row[fx_Types_Persons.Name]);
            }

            foreach (DataRow row in tblIncomes.Rows)
            {
                dtColumns.Rows.Add(row[fx_Types_Incomes.Name]);
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
