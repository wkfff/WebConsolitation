using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.MOFO;
using Krista.FM.Client.Reports.MOFO.Queries;
using Krista.FM.Client.Reports.MOFO0022.Helpers;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.Client.Reports.UFK.ReportMaster;
using Krista.FM.Client.Reports.UFK14.Helpers;
using Krista.FM.Client.Reports.UFNS.ReportQueries;

namespace Krista.FM.Client.Reports
{

    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 005 ОТЧЕТ О ЗАДОЛЖЕННОСТИ ПО АРЕНДНОЙ ПЛАТЕ
        /// </summary>
        public DataTable[] GetMOFO0022Report005Data(Dictionary<string, string> reportParams)
        {
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var tablesResult = new DataTable[3];
            var divider = reportParams[ReportConsts.ParamSumModifier];
            var precision = reportParams[ReportConsts.ParamPrecision];
            var paramRegion = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamRegionComparable]);
            var paramYear = ReportMonthMethods.CheckBookValue(reportParams[ReportConsts.ParamYear]);
            var year = paramYear != String.Empty ? ReportMonthMethods.GetSelectedYear(paramYear) : DateTime.Now.Year;
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;
            var filterQuarter = GetUNVYearQuarter(year, quarter);
            var paramMark = reportParams[ReportConsts.ParamMark];
            var contractType = GetEnumItemValue(new MOFOContractTypeEnum(), paramMark);
            var paramRgnListType = reportParams[ReportConsts.ParamRegionListType];
            var hideSettlement = String.Compare(paramRgnListType, RegionListTypeEnum.i1.ToString(), true) != 0;
            var paramHideEmptyStr = Convert.ToBoolean(reportParams[ReportConsts.ParamHideEmptyStr]);
            var filterMarks = ReportMOFO0022Helper.GetContractTypeCodes((ContractType)contractType, true);
            var marks = ConvertToIntList(filterMarks);

            // уровни
            var levelAll = ConvertToString(new[] { ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.All) });
            var levelSubject = ConvertToString(new[] { ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Subject) });
            var levelMunicipal = ConvertToString(new[]
            {
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Municipal),
                ReportMonthMethods.GetBdgtLvlCodes(SettleLvl.Settle)
            });

            var levels = new [] { levelAll, levelSubject, levelMunicipal };
            
            // фильтры фактов
            var filterList = new List<QFilter>
            {
                new QFilter(QRent.Keys.Day, filterQuarter),
                new QFilter(QRent.Keys.Org, ReportMOFO0022Helper.FixOrgRef),
                new QFilter(QRent.Keys.Lvl, String.Join(",", levels)),
                new QFilter(QRent.Keys.Mark, filterMarks),
                new QFilter(QRent.Keys.Okato, paramRegion)
            };

            var groupFields = new List<Enum> { QRent.Keys.Lvl, QRent.Keys.Okato, QRent.Keys.Mark };
            var query = new QRent().GetQueryText(filterList, groupFields);
            var tblData = dbHelper.GetTableData(query);

            // устанавливаем параметры отчета
            var rep = new Report(f_F_Rent.TableKey)
            {
                Divider = GetDividerValue(divider),
                RowFilter = paramHideEmptyStr ? Functions.IsNotNullRow : (RowViewParams.Function) null
            };

            // группировка по АТЕ
            var ateGrouping = new AteGrouping(0, hideSettlement);
            rep.AddGrouping(d_Regions_Plan.RefBridge, ateGrouping);
            var fixedId = paramHideEmptyStr
                              ? ateGrouping.AteMainId
                              : paramRegion != String.Empty
                                    ? Combine(paramRegion, ateGrouping.AteMainId)
                                    : ateGrouping.GetRegionsId();
            ateGrouping.SetFixedValues(fixedId);

            // настраиваем колонки отчета
            const string regionsTableKey = b_Regions_Bridge.InternalKey;
            const string terTypeTableKey = fx_FX_TerritorialPartitionType.InternalKey;
            var masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.CodeLine));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(terTypeTableKey, fx_FX_TerritorialPartitionType.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            masks = new AteOutMasks(new TableField(regionsTableKey, b_Regions_Bridge.Name));
            rep.AddCaptionColumn().SetMasks(ateGrouping, masks);
            var columns = new List<int> { 0, 1, 2 };
            
            var fltLevelAll = filterHelper.RangeFilter(f_F_Rent.RefBudgetLevels, levelAll);
            var fltLevelSubject = filterHelper.RangeFilter(f_F_Rent.RefBudgetLevels, levelSubject);
            var fltLevelMunicipal = filterHelper.RangeFilter(f_F_Rent.RefBudgetLevels, levelMunicipal);
            var subjectColumns = new List<int>();
            var municipalColumns = new List<int>();
            var rubColumns = new List<int>();

            // За земли после разграничения государственной собственности
            var mark = ReportMOFO0022Helper.MunicipalDifRealtyIncome;
            if (marks.Contains(mark))
            {
                var fltMark = filterHelper.RangeFilter(f_F_Rent.RefMarks, mark);
                var fltAll = String.Format("{0} and {1}", fltMark, fltLevelAll);
                var fltMunicipal = String.Format("{0} and {1}", fltMark, fltLevelMunicipal);
                rep.AddValueColumn(f_F_Rent.Quantity, fltAll).Divider = 1;
                municipalColumns.Add(rep.AddValueColumn(SUM, fltMunicipal).Index);
                rep.AddValueColumn(SUM1, fltMunicipal);
                columns.AddRange(new[] {3, 4, 5});
                rubColumns.AddRange(new[] {4, 5});
            }

            // За земли, государственная собственность на которые не разграничена, по договорам, заключенным муниципальными образованиями
            mark = ReportMOFO0022Helper.MunicipalNonDifRealtyIncome;
            if (marks.Contains(mark))
            {
                var fltMark = filterHelper.RangeFilter(f_F_Rent.RefMarks, mark);
                var fltAll = String.Format("{0} and {1}", fltMark, fltLevelAll);
                var fltSubject = String.Format("{0} and {1}", fltMark, fltLevelSubject);
                var fltMunicipal = String.Format("{0} and {1}", fltMark, fltLevelMunicipal);
                rep.AddValueColumn(f_F_Rent.Quantity, fltAll).Divider = 1;
                subjectColumns.Add(rep.AddValueColumn(SUM, fltSubject).Index);
                rep.AddValueColumn(SUM1, fltSubject);
                municipalColumns.Add(rep.AddValueColumn(SUM, fltMunicipal).Index);
                rep.AddValueColumn(SUM1, fltMunicipal);
                columns.AddRange(new[] {6, 7, 8, 9, 10});
                rubColumns.AddRange(new[] {7, 8, 9, 10});
            }

            // По доходам от сдачи в аренду имущества
            mark = ReportMOFO0022Helper.AssetRentIncome;
            if (marks.Contains(mark))
            {
                var fltMark = filterHelper.RangeFilter(f_F_Rent.RefMarks, mark);
                var fltAll = String.Format("{0} and {1}", fltMark, fltLevelAll);
                var fltMunicipal = String.Format("{0} and {1}", fltMark, fltLevelMunicipal);
                rep.AddValueColumn(f_F_Rent.Quantity, fltAll).Divider = 1;
                municipalColumns.Add(rep.AddValueColumn(SUM, fltMunicipal).Index);
                rep.AddValueColumn(SUM1, fltMunicipal);
                columns.AddRange(new[] {11, 12, 13});
                rubColumns.AddRange(new[] {12, 13});
            }

            // За земли, государственная собственность на которые не разграничена, по договорам, заключенным минмособлимуществом
            mark = ReportMOFO0022Helper.AssetDepartmentIncome;
            if (marks.Contains(mark))
            {
                var fltMark = filterHelper.RangeFilter(f_F_Rent.RefMarks, mark);
                var fltAll = String.Format("{0} and {1}", fltMark, fltLevelAll);
                var fltSubject = String.Format("{0} and {1}", fltMark, fltLevelSubject);
                var fltMunicipal = String.Format("{0} and {1}", fltMark, fltLevelMunicipal);
                rep.AddValueColumn(f_F_Rent.Quantity, fltAll).Divider = 1;
                subjectColumns.Add(rep.AddValueColumn(SUM, fltSubject).Index);
                rep.AddValueColumn(SUM1, fltSubject);
                municipalColumns.Add(rep.AddValueColumn(SUM, fltMunicipal).Index);
                rep.AddValueColumn(SUM1, fltMunicipal);
                columns.AddRange(new[] {14, 15, 16, 17, 18});
                rubColumns.AddRange(new[] {15, 16, 17, 18});
            }

            // всего
            ReportCalcColumn.Calculate sumSubjectFunction =
                (row, index) => Functions.Sum(subjectColumns.Select(i => row.Values[i]));

            ReportCalcColumn.Calculate sumMunicipalFunction =
                (row, index) => Functions.Sum(municipalColumns.Select(i => row.Values[i]));

            rep.AddCalcColumn(sumSubjectFunction);
            rep.AddCalcColumn(sumMunicipalFunction);
            columns.AddRange(new[] {19, 20});
            rubColumns.AddRange(new[] {19, 20});

            // заполняем таблицу колонок
            var dtColumns = new DataTable();
            dtColumns.Columns.Add("index");
            dtColumns.Columns.Add("rub", typeof(bool));

            foreach (var column in columns)
            {
                dtColumns.Rows.Add(column, rubColumns.Contains(column));
            }

            rep.ProcessTable(tblData);
            tablesResult[0] = rep.GetReportData();
            tablesResult[1] = dtColumns;

            // заполняем таблицу параметров
            var rowCaption = CreateReportParamsRow(tablesResult);
            var paramHelper = new ParamUFKHelper(rowCaption);
            var contract = GetEnumItemDescription(new MOFOContractTypeEnum(), paramMark).ToUpper();
            paramHelper.SetParamValue(ParamUFKHelper.YEARS, year);
            paramHelper.SetParamValue(ParamUFKHelper.QUARTER, quarter);
            paramHelper.SetParamValue(ParamUFKHelper.CONTRACT, contract);
            paramHelper.SetParamValue(ParamUFKHelper.MEASURE, ReportMonthMethods.GetDividerDescr(divider));
            paramHelper.SetParamValue(ParamUFKHelper.PRECISION, ReportMonthMethods.GetPrecisionIndex(precision));
            return tablesResult;
        }
    }
}
