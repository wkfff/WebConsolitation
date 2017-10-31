using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.FactTables;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// Заимствования государственных унитарных предприятий Московской области у третьих лиц
        /// </summary>
        public DataTable[] GetDrawingGovFactoryData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[2];
            var paramYear = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;
            var fltPeriod = GetUNVYearQuarter(paramYear, quarter);
            var dataObj = new CommonDataObject();
            dataObj.InitObject(scheme);
            dataObj.useSummaryRow = false;
            dataObj.mainFilter[f_Charge_DebtUnEnt.IsDebtMUE] = "=0";
            dataObj.mainFilter[f_Charge_DebtUnEnt.RefYearDayUNV] = fltPeriod;
            dataObj.ObjectKey = f_Charge_DebtUnEnt.internalKey;
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefOrg,
                d_Organizations_Analysis.internalKey,
                d_Organizations_Analysis.Name,
                String.Empty);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.KindDebt);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Creditor);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Purpose);
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonTemplate,
                "№{0} от {1}",
                new List<string> { f_Charge_DebtUnEnt.Num, f_Charge_DebtUnEnt.StartDate }
                );
            dataObj.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.CreditPercent);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Collateral);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Sum);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.EndDate);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Attract);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.AttractCYr);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Discharge);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.DischargeCYr);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Interest);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.InterestCYr);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Penalty);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.PenaltyCYr);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Debt);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.SlaleDebt);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Note);
            // служебные
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefOrg,
                d_Organizations_Analysis.internalKey,
                d_Organizations_Analysis.ID,
                String.Empty);
            dataObj.SetColumnNameParam(TempFieldNames.SortStatus);
            dataObj.SetColumnParam(dataObj.ParamDataType, ReportConsts.ftInt32);
            dataObj.sortString = StrSortUp(TempFieldNames.SortStatus);
            tables[0] = dataObj.FillData();
            var rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = paramYear;
            rowCaption[1] = paramQuarter;
            rowCaption[2] = GetStartQuarter(paramYear, quarter);
            return tables;
        }

        /// <summary>
        /// ГУПы в бухгалтерию
        /// </summary>
        public DataTable[] GetDrawingGUPBuhData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[2];
            var paramYear = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;
            var fltPeriod = GetUNVYearQuarter(paramYear, quarter);
            var dataObj = new CommonDataObject();
            dataObj.InitObject(scheme);
            dataObj.useSummaryRow = false;
            dataObj.mainFilter[f_Charge_DebtUnEnt.IsDebtMUE] = "=0";
            dataObj.mainFilter[f_Charge_DebtUnEnt.RefYearDayUNV] = fltPeriod;
            dataObj.ObjectKey = f_Charge_DebtUnEnt.internalKey;
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefOrg,
                d_Organizations_Analysis.internalKey,
                d_Organizations_Analysis.Name,
                String.Empty);
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefOrg,
                d_Organizations_Analysis.internalKey,
                d_Organizations_Analysis.RefRegion,
                String.Empty);
            dataObj.SetColumnNameParam(TempFieldNames.RegionName);
            dataObj.AddParamColumn(
                CalcColumnType.cctCalcBookValue,
                TempFieldNames.RegionName,
                d_Regions_Analysis.internalKey,
                d_Regions_Analysis.Name,
                String.Empty);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Debt);
            // служебная
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefOrg,
                d_Organizations_Analysis.internalKey,
                d_Organizations_Analysis.ID,
                String.Empty);
            dataObj.SetColumnParam(dataObj.ParamDataType, ReportConsts.ftInt32);
            dataObj.SetColumnNameParam(TempFieldNames.SortStatus);
            dataObj.sortString = StrSortUp(TempFieldNames.SortStatus);
            tables[0] = dataObj.FillData();
            tables[0].Columns.RemoveAt(1);
            var rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = paramYear;
            rowCaption[1] = paramQuarter;
            rowCaption[2] = GetStartQuarter(paramYear, quarter);
            return tables;
        }

        /// <summary>
        /// Заимствования муниципальных унитарных предприятий
        /// </summary>
        public DataTable[] GetDrawingMunFactoryData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[2];
            var paramYear = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;
            var fltPeriod = GetUNVYearQuarter(paramYear, quarter);
            var dataObj = new CommonDataObject();
            dataObj.InitObject(scheme);
            dataObj.useSummaryRow = false;
            dataObj.mainFilter[f_Charge_DebtUnEnt.IsDebtMUE] = "=1";
            dataObj.mainFilter[f_Charge_DebtUnEnt.RefYearDayUNV] = fltPeriod;
            dataObj.ObjectKey = f_Charge_DebtUnEnt.internalKey;
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefRegion,
                d_Regions_Analysis.internalKey,
                d_Regions_Analysis.Name,
                String.Empty);
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefOrg,
                d_Organizations_Analysis.internalKey,
                d_Organizations_Analysis.Name,
                String.Empty);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.KindDebt);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Creditor);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Purpose);
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonTemplate,
                "№{0} от {1}",
                new List<string> {f_Charge_DebtUnEnt.Num, f_Charge_DebtUnEnt.StartDate}
                );
            dataObj.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.CreditPercent);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Collateral);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Sum);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.EndDate);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Attract);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.AttractCYr);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Discharge);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.DischargeCYr);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Interest);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.InterestCYr);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Penalty);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.PenaltyCYr);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Debt);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.SlaleDebt);
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Note);
            // служебные
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefRegion,
                d_Regions_Analysis.internalKey,
                d_Regions_Analysis.Code,
                String.Empty);
            dataObj.SetColumnParam(dataObj.ParamDataType, ReportConsts.ftInt32);
            dataObj.SetColumnNameParam(TempFieldNames.SortStatus);
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefOrg,
                d_Organizations_Analysis.internalKey,
                d_Organizations_Analysis.ID,
                String.Empty);
            dataObj.SetColumnParam(dataObj.ParamDataType, ReportConsts.ftInt32);
            dataObj.SetColumnNameParam(TempFieldNames.RowType);
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefRegion,
                d_Regions_Analysis.internalKey,
                d_Regions_Analysis.RefTerr,
                String.Empty);
            dataObj.SetColumnNameParam(d_Regions_Analysis.RefTerr);
            dataObj.sortString = FormSortString(
                StrSortUp(TempFieldNames.SortStatus),
                StrSortUp(TempFieldNames.RowType));
            var tblData = dataObj.FillData();
            var tblResult = tblData.Clone();

            var oldCode = String.Empty;
            var summary = new decimal[tblData.Columns.Count];
            var totals = new decimal[tblData.Columns.Count];

            foreach (DataRow rowData in tblData.Rows)
            {
                var code = Convert.ToString(rowData[TempFieldNames.SortStatus]);
                var newCode = code.Length > 0 ? code.Substring(0, 2) : String.Empty;

                if (oldCode.Length > 0 && newCode != oldCode)
                {
                    AddSummaryDrawingMunFactoryRow(tblResult, summary);
                    Array.Clear(summary, 0, summary.Length);
                }

                for (var i = 9; i < tblData.Columns.Count; i++)
                {
                    summary[i] += GetDecimal(rowData[i]);
                    totals[i] += GetDecimal(rowData[i]);
                }

                tblResult.ImportRow(rowData);
                var rowInsert = GetLastRow(tblResult);
                rowInsert[TempFieldNames.RowType] = 0;
                oldCode = newCode;
            }

            if (tblResult.Rows.Count > 0)
            {
                AddSummaryDrawingMunFactoryRow(tblResult, summary);
            }

            AddSummaryDrawingMunFactoryRow(tblResult, totals, "Всего");
            tables[0] = tblResult;
            var rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = paramYear;
            rowCaption[1] = paramQuarter;
            rowCaption[2] = GetStartQuarter(paramYear, quarter);
            return tables;
        }

        private void AddSummaryDrawingMunFactoryRow(DataTable tblResult, decimal[] summary, string text = "Итого")
        {
            var rowSummary = tblResult.Rows.Add();

            for (var i = 1; i < summary.Length; i++)
            {
                if (summary[i] != 0)
                {
                    rowSummary[i] = summary[i];
                }
            }

            rowSummary[0] = text;
            rowSummary[TempFieldNames.RowType] = 1;
        }

        /// <summary>
        /// MУПы в бухгалтерию
        /// </summary>
        public DataTable[] GetDrawingMUPBuhData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[2];
            var paramYear = Convert.ToInt32(reportParams[ReportConsts.ParamYear]);
            var paramQuarter = reportParams[ReportConsts.ParamQuarter];
            var quarter = Convert.ToInt32(GetEnumItemValue(new QuarterEnum(), paramQuarter)) + 1;
            var fltPeriod = GetUNVYearQuarter(paramYear, quarter);
            var dataObj = new CommonDataObject();
            dataObj.InitObject(scheme);
            dataObj.useSummaryRow = false;
            dataObj.mainFilter[f_Charge_DebtUnEnt.IsDebtMUE] = "=1";
            dataObj.mainFilter[f_Charge_DebtUnEnt.RefYearDayUNV] = fltPeriod;
            dataObj.ObjectKey = f_Charge_DebtUnEnt.internalKey;
            // колонки
            // 0
            dataObj.AddCalcColumn(CalcColumnType.cctUndefined);
            // 1
            dataObj.AddCalcColumn(CalcColumnType.cctUndefined);
            // 3
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.Debt);
            // служебные
            // 4 - ссылка на район
            dataObj.AddDataColumn(f_Charge_DebtUnEnt.RefRegion, ReportConsts.ftInt32);
            // 5 - тип территории
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefRegion,
                d_Regions_Analysis.internalKey,
                d_Regions_Analysis.RefTerr,
                String.Empty);
            dataObj.SetColumnNameParam(d_Regions_Analysis.RefTerr);
            dataObj.SetColumnParam(dataObj.ParamDataType, ReportConsts.ftInt32);
            // 6 - код
            dataObj.AddParamColumn(
                CalcColumnType.cctCommonBookValue,
                f_Charge_DebtUnEnt.RefRegion,
                d_Regions_Analysis.internalKey,
                d_Regions_Analysis.Code,
                String.Empty);
            dataObj.SetColumnNameParam(d_Regions_Analysis.Code);
            dataObj.SetColumnParam(dataObj.ParamDataType, ReportConsts.ftInt32);
            // 7 
            dataObj.AddParamColumn(
                CalcColumnType.cctCalcBookValue,
                d_Organizations_Analysis.RefRegion,
                d_Regions_Analysis.internalKey,
                d_Regions_Analysis.SourceID,
                String.Empty);
            dataObj.SetColumnNameParam(d_Regions_Analysis.SourceID);
            var tblData = dataObj.FillData();

            var sourceId = -1;

            if (tblData.Rows.Count > 0)
            {
                sourceId = Convert.ToInt32(tblData.Rows[0][d_Regions_Analysis.SourceID]);
            }

            var tblResult = CreateDrawningMUPStructure(tblData, sourceId);
            tables[0] = tblResult;
            // заголовки
            var rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = paramYear;
            rowCaption[1] = paramQuarter;
            rowCaption[2] = GetStartQuarter(paramYear, quarter);

            const int dataIndex = 2;
            var terrColumn = tblResult.Columns[3].ColumnName;
            var rowsMun = tblResult.Select(GetRangeFilter(terrColumn, "4"));
            var rowsGoo = tblResult.Select(GetRangeFilter(terrColumn, "7"));
            var rowsStl = tblResult.Select(GetRangeFilter(terrColumn, "5,6"));
            rowCaption[5] = rowsGoo.Sum(dataRow => GetDecimal(dataRow[dataIndex]));
            rowCaption[6] = rowsMun.Sum(dataRow => GetDecimal(dataRow[dataIndex]));
            rowCaption[7] = rowsStl.Sum(dataRow => GetDecimal(dataRow[dataIndex]));
            rowCaption[8] = GetDecimal(rowCaption[5]) + GetDecimal(rowCaption[6]) + GetDecimal(rowCaption[7]);
            return tables;
        }

        private string GetEqualFilter(object fieldName, object value)
        {
            return String.Format("{0} = {1}", fieldName, value);
        }

        private string GetRangeFilter(object fieldName, object values)
        {
            return String.Format("{0} in ({1})", fieldName, values);
        }

        private DataTable CreateDrawningMUPStructure(DataTable tblData, int sourceId)
        {
            var tblResult = CreateReportCaptionTable(5);
            var dbHelper = new ReportDBHelper(scheme);
            var tblRegions = dbHelper.GetEntityData(d_Regions_Analysis.internalKey,
                GetEqualFilter(d_Regions_Analysis.SourceID, sourceId));
            var rowsGoo = tblRegions.Select(GetEqualFilter(d_Regions_Analysis.RefTerr, 7), d_Regions_Analysis.Name);
            var rowsMup = tblRegions.Select(GetEqualFilter(d_Regions_Analysis.RefTerr, 4), d_Regions_Analysis.Name);
            var counter = 1;

            for (var i = 0; i < rowsGoo.Length; i++)
            {
                var rowCls = rowsGoo[i];
                counter = AddRegionStructureRow(counter, tblResult, rowCls, tblData);
                AddRegionSummaryRow(tblResult, 0);
            }

            for (var i = 0; i < rowsMup.Length; i++)
            {
                var rowCls = rowsMup[i];
                counter = AddRegionStructureRow(counter, tblResult, rowCls, tblData);
                var lstSettles = GetSettlesList(tblRegions, rowCls);

                foreach (var settleRow in lstSettles)
                {
                    counter = AddRegionStructureRow(counter, tblResult, settleRow, tblData);                    
                }

                AddRegionSummaryRow(tblResult, lstSettles.Count);
            }

            return tblResult;
        }

        private List<DataRow> GetSettlesList(DataTable tblCls, DataRow rowCls)
        {
            var fltSettle = GetEqualFilter(d_Regions_Analysis.ParentID, rowCls[d_Regions_Analysis.id]);
            var settleRows = tblCls.Select(fltSettle);
            
            if (settleRows.Length > 0)
            {
                var fltSettles = GetEqualFilter(d_Regions_Analysis.ParentID, settleRows[0][d_Regions_Analysis.id]);
                var rowChild = tblCls.Select(fltSettles, d_Regions_Analysis.Name);
                return new List<DataRow>(rowChild);
            }

            return new List<DataRow>();
        }

        private int AddRegionStructureRow(int counter, DataTable tblResult, DataRow rowCls, DataTable tblData)
        {
            var rowResult = tblResult.Rows.Add();
            var fltRgn = GetEqualFilter(d_Organizations_Analysis.RefRegion, rowCls[d_Regions_Analysis.id]);
            var rowsData = tblData.Select(fltRgn);
            rowResult[0] = counter++;
            rowResult[1] = rowCls[d_Regions_Analysis.Name];
            rowResult[2] = rowsData.Sum(dataRow => GetDecimal(dataRow[f_Charge_DebtUnEnt.Debt]));
            rowResult[3] = rowCls[d_Regions_Analysis.RefTerr];
            return counter;
        }

        private void AddRegionSummaryRow(DataTable tblResult, int cntSettleRow)
        {
            decimal sum = 0;

            for (var i = 0; i < cntSettleRow + 1; i++)
            {
                sum += GetDecimal(tblResult.Rows[tblResult.Rows.Count - 1 - i][2]);
            }

            var rowResult = tblResult.Rows.Add();
            rowResult[0] = "Итого";
            rowResult[2] = sum;
        }
    }
}
