using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.FactTables.CapitalOperations;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;
using Krista.FM.Client.Reports.Planning.Data;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// Московская область - Долговая нагрузка с обслуживанием + замена
        /// </summary>
        public DataTable[] GetMODebtServiceLoadingWithReplaceData(Dictionary<string, string> reportParams)
        {
            var paramList = reportParams[ReportConsts.ParamVariantID].Split(',');
            var lstYears = new List<int>();
            var lstBasement = new List<CapitalBasementInfo>();

            // определяем диапазон лет
            foreach (var paramID in paramList)
            {
                var infoBasement = new CapitalBasementInfo();
                lstBasement.Add(infoBasement);
                var fltParam = String.Format("={0}", paramID);

                // Выкупы
                var cdoParam = new CommonDataObject();
                cdoParam.InitObject(scheme);
                cdoParam.useSummaryRow = false;
                cdoParam.ObjectKey = f_S_ReplaceIssForLn.internalKey;
                cdoParam.mainFilter[f_S_ReplaceIssForLn.ID] = fltParam;
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.ID);
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.RedemptionDate);
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.OfficialNumber);
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.TotalNom);
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CrdtSum);
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.Count);
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.TotalCpnInc);
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.TotalDiffPCNom);
                cdoParam.AddDataColumn(f_S_ReplaceIssForLn.EndCrdtDate);
                var tblCalc = cdoParam.FillData();
                var rowCalc = GetLastRow(tblCalc);
                infoBasement.RedemptionDate = Convert.ToString(rowCalc[f_S_ReplaceIssForLn.RedemptionDate]);
                infoBasement.EndCrdtDate = Convert.ToString(rowCalc[f_S_ReplaceIssForLn.EndCrdtDate]);
                infoBasement.TotalNom = GetDecimal(rowCalc[f_S_ReplaceIssForLn.TotalNom]);
                infoBasement.CrdtSum = GetDecimal(rowCalc[f_S_ReplaceIssForLn.CrdtSum]);
                infoBasement.Count = GetDecimal(rowCalc[f_S_ReplaceIssForLn.Count]);
                infoBasement.TotalCpnInc = GetDecimal(rowCalc[f_S_ReplaceIssForLn.TotalCpnInc]);
                infoBasement.TotalDiffPCNom = GetDecimal(rowCalc[f_S_ReplaceIssForLn.TotalDiffPCNom]);
                // Займы
                var cdoCapital = new CapitalDataObject();
                cdoCapital.InitObject(scheme);
                cdoCapital.useSummaryRow = false;
                cdoCapital.mainFilter[f_S_Capital.OfficialNumber] = String.Format("='{0}'",
                    Convert.ToString(rowCalc[f_S_ReplaceIssForLn.OfficialNumber]));
                cdoCapital.AddDataColumn(f_S_Capital.id);
                cdoCapital.AddDataColumn(f_S_Capital.DateDischarge);
                cdoCapital.AddDetailColumn(String.Format("[{0}][{1}]", t_S_CPPlanDebt.key, t_S_CPPlanService.key));
                var tblCapital = cdoCapital.FillData();
                infoBasement.PlanDbtRecs = new List<DataRow>();
                infoBasement.PlanPctRecs = new List<DataRow>();
                infoBasement.LnPctRecs = new List<DataRow>();

                var cdoLnPct = new CommonDataObject();
                cdoLnPct.InitObject(scheme);
                cdoLnPct.useSummaryRow = false;
                cdoLnPct.ObjectKey = t_S_ReIssForLnServ.internalKey;
                cdoLnPct.mainFilter[t_S_ReIssForLnServ.RefReplaceIssFLn] = Convert.ToString(rowCalc[f_S_ReplaceIssForLn.ID]);
                cdoLnPct.AddDataColumn(t_S_ReIssForLnServ.EndDate);
                cdoLnPct.AddDataColumn(t_S_ReIssForLnServ.Sum);
                var tblLnPct = cdoLnPct.FillData();

                foreach (DataRow rowLnPct in tblLnPct.Rows)
                {
                    infoBasement.LnPctRecs.Add(rowLnPct);
                }

                foreach (DataRow rowCap in tblCapital.Rows)
                {
                    var id = Convert.ToInt32(rowCap[f_S_Capital.id]);
                    var formulaDbt = String.Format("[{0}](1>{1})", t_S_CPPlanDebt.key, infoBasement.RedemptionDate);
                    var formulaPct = String.Format("[{0}](2>{1})", t_S_CPPlanService.key, infoBasement.RedemptionDate);
                    cdoCapital.ParseFormula(id, ref formulaDbt);
                    infoBasement.PlanDbtRecs.AddRange(cdoCapital.sumIncludedRows);
                    cdoCapital.ParseFormula(id, ref formulaPct);
                    infoBasement.PlanPctRecs.AddRange(cdoCapital.sumIncludedRows);
                    var dateStart = Convert.ToDateTime(rowCalc[f_S_ReplaceIssForLn.RedemptionDate]);
                    var loYear = dateStart.Year;

                    if (!lstYears.Contains(loYear))
                    {
                        lstYears.Add(loYear);
                    }

                    if (rowCap[f_S_Capital.DateDischarge] == DBNull.Value)
                    {
                        continue;
                    }

                    var hiYear = Convert.ToDateTime(rowCap[f_S_Capital.DateDischarge]).Year;

                    for (var i = loYear + 1; i <= hiYear; i++)
                    {
                        if (!lstYears.Contains(i))
                        {
                            lstYears.Add(i);
                        }
                    }
                }
            }

            const int GroupSize = 8;
            lstYears.Sort();
            var tblData = CreateReportCaptionTable(Math.Max(GroupSize, lstYears.Count * GroupSize), 12);

            for (var i = 0; i < tblData.Columns.Count; i++)
            {
                foreach (DataRow row in tblData.Rows)
                {
                    row[i] = 0;
                }
            }

            var dataContracts = GetMODebtServiceLoadingData(reportParams, lstYears);
            var tables = new DataTable[dataContracts.Length + 2];

            for (var i = 0; i < dataContracts.Length; i++)
            {
                tables[i] = dataContracts[i];
            }

            foreach (var infoBasement in lstBasement)
            {
                var redemptionDate = Convert.ToDateTime(infoBasement.RedemptionDate);
                var creditEndDate = Convert.ToDateTime(infoBasement.EndCrdtDate);
                var redemptionColIndex = lstYears.IndexOf(redemptionDate.Year) * GroupSize + 0;
                var redemptionRowIndex = redemptionDate.Month - 1;
                var creditColIndex = lstYears.IndexOf(creditEndDate.Year) * GroupSize + 1;
                var creditRowIndex = creditEndDate.Month - 1;
                AddValueToCell(tblData, redemptionRowIndex, redemptionColIndex, infoBasement.TotalNom);
                AddValueToCell(tblData, creditRowIndex, creditColIndex, infoBasement.CrdtSum);
                redemptionColIndex += 4;
                AddValueToCell(tblData, redemptionRowIndex, redemptionColIndex, infoBasement.TotalCpnInc);
                redemptionColIndex += 1;
                AddValueToCell(tblData, redemptionRowIndex, redemptionColIndex, infoBasement.TotalDiffPCNom);

                foreach (var rowDbt in infoBasement.PlanDbtRecs)
                {
                    if (rowDbt[t_S_CPPlanDebt.EndDate] == DBNull.Value)
                    {
                        continue;
                    }

                    var date = Convert.ToDateTime(rowDbt[t_S_CPPlanDebt.EndDate]);
                    var year = date.Year;
                    var month = date.Month;
                    var colIndex = lstYears.IndexOf(year) * GroupSize + 2;
                    var rowIndex = month - 1;
                    var percent = GetDecimal(rowDbt[t_S_CPPlanDebt.PercentNom]);
                    AddValueToCell(tblData, rowIndex, colIndex, 10 * percent * infoBasement.Count);
                }

                foreach (var rowPct in infoBasement.PlanPctRecs)
                {
                    if (rowPct[t_S_CPPlanService.PaymentDate] == DBNull.Value)
                    {
                        continue;
                    }

                    var date = Convert.ToDateTime(rowPct[t_S_CPPlanService.PaymentDate]);
                    var year = date.Year;
                    var month = date.Month;
                    var colIndex = lstYears.IndexOf(year) * GroupSize + 6;
                    var rowIndex = month - 1;
                    var income = GetDecimal(rowPct[t_S_CPPlanService.Income]);
                    AddValueToCell(tblData, rowIndex, colIndex, 10 * income * infoBasement.Count);
                }

                foreach (var rowPct in infoBasement.LnPctRecs)
                {
                    if (rowPct[t_S_ReIssForLnServ.EndDate] == DBNull.Value)
                    {
                        continue;
                    }

                    var date = Convert.ToDateTime(rowPct[t_S_ReIssForLnServ.EndDate]);
                    var year = date.Year;
                    var month = date.Month;
                    var colIndex = lstYears.IndexOf(year) * GroupSize + 3;
                    var rowIndex = month - 1;
                    AddValueToCell(tblData, rowIndex, colIndex, rowPct[t_S_ReIssForLnServ.Sum]);
                }
            }

            tables[tables.Length - 2] = tblData;
            var rowCaption = CreateReportParamsRow(tables);

            for (var i = 0; i < lstYears.Count; i++)
            {
                rowCaption[10 + i] = lstYears[i];
            }

            rowCaption[0] = reportParams[ReportConsts.ParamOutputMode];
            rowCaption[1] = GetEnumItemIndex(new DebtLoadingListTypeEnum(), reportParams[ReportConsts.ParamOutputMode]);
            rowCaption[2] = lstYears.Count;
            return tables;
        }

        /// <summary>
        /// Московская область - Долговая нагрузка с обслуживанием + выкуп
        /// </summary>
        public DataTable[] GetMODebtServiceLoadingWithRedemptionData(Dictionary<string, string> reportParams)
        {
            var paramList = reportParams[ReportConsts.ParamVariantID].Split(',');
            var lstYears = new List<int>();
            var lstBasement = new List<CapitalBasementInfo>();

            // определяем диапазон лет
            foreach (var paramID in paramList)
            {
                var infoBasement = new CapitalBasementInfo();
                lstBasement.Add(infoBasement);
                var fltParam = String.Format("={0}", paramID);

                // Выкупы
                var cdoParam = new CommonDataObject();
                cdoParam.InitObject(scheme);
                cdoParam.useSummaryRow = false;
                cdoParam.ObjectKey = f_S_BondBuyback.internalKey;
                cdoParam.mainFilter[f_S_BondBuyback.ID] = fltParam;
                cdoParam.AddDataColumn(f_S_BondBuyback.RedemptionDate);
                cdoParam.AddDataColumn(f_S_BondBuyback.OfficialNumber);
                cdoParam.AddDataColumn(f_S_BondBuyback.TotalNom);
                cdoParam.AddDataColumn(f_S_BondBuyback.TotalCostServLn);
                cdoParam.AddDataColumn(f_S_BondBuyback.TotalCount);
                var tblCalc = cdoParam.FillData();
                var rowCalc = GetLastRow(tblCalc);
                infoBasement.RedemptionDate = Convert.ToString(rowCalc[f_S_BondBuyback.RedemptionDate]);
                infoBasement.TotalNom = GetDecimal(rowCalc[f_S_BondBuyback.TotalNom]);
                infoBasement.TotalCostServLn = GetDecimal(rowCalc[f_S_BondBuyback.TotalCostServLn]);
                infoBasement.TotalCount = GetDecimal(rowCalc[f_S_BondBuyback.TotalCount]);

                // Займы
                var cdoCapital = new CapitalDataObject();
                cdoCapital.InitObject(scheme);
                cdoCapital.useSummaryRow = false;
                cdoCapital.mainFilter[f_S_Capital.OfficialNumber] = String.Format("='{0}'",
                    Convert.ToString(rowCalc[f_S_BondBuyback.OfficialNumber]));
                cdoCapital.AddDataColumn(f_S_Capital.id);
                cdoCapital.AddDataColumn(f_S_Capital.DateDischarge);
                cdoCapital.AddDetailColumn(String.Format("[{0}][{1}]", t_S_CPPlanDebt.key, t_S_CPPlanService.key));
                var tblCapital = cdoCapital.FillData();
                infoBasement.PlanDbtRecs = new List<DataRow>();
                infoBasement.PlanPctRecs = new List<DataRow>();

                foreach (DataRow rowCap in tblCapital.Rows)
                {
                    var id = Convert.ToInt32(rowCap[f_S_Capital.id]);
                    var formulaDbt = String.Format("[{0}](1>{1})", t_S_CPPlanDebt.key, infoBasement.RedemptionDate);
                    var formulaPct = String.Format("[{0}](2>{1})", t_S_CPPlanService.key, infoBasement.RedemptionDate);
                    cdoCapital.ParseFormula(id, ref formulaDbt);
                    infoBasement.PlanDbtRecs.AddRange(cdoCapital.sumIncludedRows);
                    cdoCapital.ParseFormula(id, ref formulaPct);
                    infoBasement.PlanPctRecs.AddRange(cdoCapital.sumIncludedRows);

                    var dateStart = Convert.ToDateTime(rowCalc[f_S_BondBuyback.RedemptionDate]);
                    var loYear = dateStart.Year;

                    if (!lstYears.Contains(loYear))
                    {
                        lstYears.Add(loYear);
                    }

                    if (rowCap[f_S_Capital.DateDischarge] == DBNull.Value)
                    {
                        continue;
                    }

                    var hiYear = Convert.ToDateTime(rowCap[f_S_Capital.DateDischarge]).Year;

                    for (var i = loYear + 1; i <= hiYear; i++)
                    {
                        if (!lstYears.Contains(i))
                        {
                            lstYears.Add(i);
                        }
                    }
                }
            }

            const int GroupSize = 5;
            lstYears.Sort();
            var tblData = CreateReportCaptionTable(Math.Max(GroupSize, lstYears.Count * GroupSize), 12);

            for (var i = 0; i < tblData.Columns.Count; i++)
            {
                foreach (DataRow row in tblData.Rows)
                {
                    row[i] = 0;
                }
            }

            var dataContracts = GetMODebtServiceLoadingData(reportParams, lstYears);
            var tables = new DataTable[dataContracts.Length + 2];

            for (var i = 0; i < dataContracts.Length; i++)
            {
                tables[i] = dataContracts[i];
            }

            foreach (var infoBasement in lstBasement)
            {
                var redemptionDate = Convert.ToDateTime(infoBasement.RedemptionDate);
                var redemptionYear = redemptionDate.Year;
                var redemptionMonth = redemptionDate.Month;
                var redemptionColIndex = lstYears.IndexOf(redemptionYear) * GroupSize + 0;
                var redemptionRowIndex = redemptionMonth - 1;
                tblData.Rows[redemptionRowIndex][redemptionColIndex] =
                    GetDecimal(tblData.Rows[redemptionRowIndex][redemptionColIndex]) + infoBasement.TotalNom;
                redemptionColIndex += 2;
                tblData.Rows[redemptionRowIndex][redemptionColIndex] =
                    GetDecimal(tblData.Rows[redemptionRowIndex][redemptionColIndex]) + infoBasement.TotalCostServLn;

               foreach (DataRow rowDbt in infoBasement.PlanDbtRecs)
                {
                    if (rowDbt[t_S_CPPlanDebt.EndDate] == DBNull.Value)
                    {
                        continue;
                    }

                    var date = Convert.ToDateTime(rowDbt[t_S_CPPlanDebt.EndDate]);
                    var year = date.Year;
                    var month = date.Month;
                    var colIndex = lstYears.IndexOf(year) * GroupSize + 1;
                    var rowIndex = month - 1;
                    tblData.Rows[rowIndex][colIndex] = 
                        GetDecimal(tblData.Rows[rowIndex][colIndex]) + 
                        10 * GetDecimal(rowDbt[t_S_CPPlanDebt.PercentNom]) * infoBasement.TotalCount;
                }

               foreach (DataRow rowPct in infoBasement.PlanPctRecs)
               {
                   if (rowPct[t_S_CPPlanService.PaymentDate] == DBNull.Value)
                   {
                       continue;
                   }

                   var date = Convert.ToDateTime(rowPct[t_S_CPPlanService.PaymentDate]);
                   var year = date.Year;
                   var month = date.Month;
                   var colIndex = lstYears.IndexOf(year) * GroupSize + 3;
                   var rowIndex = month - 1;
                   tblData.Rows[rowIndex][colIndex] = 
                       GetDecimal(tblData.Rows[rowIndex][colIndex]) + 
                       10 * GetDecimal(rowPct[t_S_CPPlanService.Income]) * infoBasement.TotalCount;
               }
            }

            tables[tables.Length - 2] = tblData;
            var rowCaption = CreateReportParamsRow(tables);

            for (var i = 0; i < lstYears.Count; i++)
            {
                foreach (DataRow row in tblData.Rows)
                {
                    var startBlock = i * GroupSize;
                    row[startBlock + 4] = 
                        GetDecimal(row[startBlock + 0]) + 
                        GetDecimal(row[startBlock + 1]) +
                        GetDecimal(row[startBlock + 2]) +
                        GetDecimal(row[startBlock + 3]);
                }

                rowCaption[10 + i] = lstYears[i];
            }

            rowCaption[0] = reportParams[ReportConsts.ParamOutputMode];
            rowCaption[1] = GetEnumItemIndex(new DebtLoadingListTypeEnum(), reportParams[ReportConsts.ParamOutputMode]);
            rowCaption[2] = lstYears.Count;
            return tables;
        }

        private class CapitalBasementInfo
        {
            public string BondId { get; set; }
            public string RedemptionDate { get; set; }
            public decimal TotalCostServLn { get; set; }
            public decimal TotalCount { get; set; }
            public decimal TotalNom { get; set; }
            public List<DataRow> PlanDbtRecs { get; set; }
            public List<DataRow> PlanPctRecs { get; set; }
            public decimal CrdtSum { get; set; }
            public decimal Count { get; set; }
            public decimal TotalCpnInc { get; set; }
            public decimal TotalDiffPCNom { get; set; }
            public string EndCrdtDate { get; set; }
            public List<DataRow> LnPctRecs { get; set; }
        }

        /// <summary>
        /// Московская область - Долговая нагрузка с обслуживанием + размещение займа
        /// </summary>
        public DataTable[] GetMODebtServiceLoadingWithCapBasementData(Dictionary<string, string> reportParams)
        {
            var paramList = reportParams[ReportConsts.ParamVariantID].Split(',');
            var lstYears = new List<int>();
            var lstBasement = new List<CapitalBasementInfo>();

            // определяем диапазон лет
            foreach (var paramID in paramList)
            {
                var infoBasement = new CapitalBasementInfo();
                lstBasement.Add(infoBasement);
                var fltParam = String.Format("={0}", paramID);
                // парметры
                var cdoParam = new CommonDataObject();
                cdoParam.InitObject(scheme);
                cdoParam.useSummaryRow = false;
                cdoParam.ObjectKey = f_S_ICalcParam.internalKey;
                
                if (paramID.Length > 0)
                {
                    cdoParam.mainFilter[f_S_ICalcParam.ID] = fltParam;
                }

                cdoParam.AddDataColumn(f_S_ICalcParam.CurrencyBorrow);
                cdoParam.AddDataColumn(f_S_ICalcParam.StartCpnDate);
                var tblCalc = cdoParam.FillData();
                var rowCalc = GetLastRow(tblCalc);

                // Размещение займа
                var cdoCapital = new CommonDataObject();
                cdoCapital.InitObject(scheme);
                cdoCapital.useSummaryRow = false;
                cdoCapital.ObjectKey = f_S_IssueBond.internalKey;
                
                if (paramID.Length > 0)
                {
                    cdoCapital.mainFilter[f_S_IssueBond.IDCalcParam] = fltParam;
                }

                cdoCapital.AddDataColumn(f_S_IssueBond.ID);
                var tblCapital = cdoCapital.FillData();
                var rowCapital = GetLastRow(tblCapital);

                // Раcходы по выплате
                if (rowCapital != null)
                {
                    infoBasement.BondId = Convert.ToString(rowCapital[f_S_IssueBond.ID]);
                }

                if (rowCalc == null || rowCalc[f_S_ICalcParam.StartCpnDate] == DBNull.Value)
                {
                    continue;
                }

                var dateStart = Convert.ToDateTime(rowCalc[f_S_ICalcParam.StartCpnDate]);
                var loYear = dateStart.Year;
                        
                if (!lstYears.Contains(loYear))
                {
                    lstYears.Add(loYear);
                }

                if (rowCalc[f_S_ICalcParam.CurrencyBorrow] == DBNull.Value)
                {
                    continue;
                }

                var hiYear = dateStart.AddDays(Convert.ToInt32(rowCalc[f_S_ICalcParam.CurrencyBorrow])).Year;

                for (var i = loYear + 1; i <= hiYear; i++)
                {
                    if (!lstYears.Contains(i))
                    {
                        lstYears.Add(i);
                    }
                }
            }

            lstYears.Sort();
            var tblData = CreateReportCaptionTable(Math.Max(3, lstYears.Count * 3), 12);
            var dataContracts = GetMODebtServiceLoadingData(reportParams, lstYears);
            var tables = new DataTable[dataContracts.Length + 2];

            for (var i = 0; i < dataContracts.Length; i++)
            {
                tables[i] = dataContracts[i];
            }

            foreach (var infoBasement in lstBasement)
            {
                // Раcходы по выплате
                if (infoBasement.BondId == null)
                {
                    continue;
                }

                var cdoCosts = new CommonDataObject();
                cdoCosts.InitObject(scheme);
                cdoCosts.useSummaryRow = false;
                cdoCosts.ObjectKey = t_S_IPaymnts.internalKey;
                cdoCosts.mainFilter[t_S_IPaymnts.RefIssBonds] = String.Format("={0}", infoBasement.BondId);
                cdoCosts.AddDataColumn(t_S_IPaymnts.PayDate);
                cdoCosts.AddDataColumn(t_S_IPaymnts.NomSum);
                cdoCosts.AddDataColumn(t_S_IPaymnts.EndDate);
                cdoCosts.AddDataColumn(t_S_IPaymnts.TotalCoupon);
                cdoCosts.AddDataColumn(t_S_IPaymnts.TotalCount);
                var tblCosts = cdoCosts.FillData();

                foreach (DataRow rowPayment in tblCosts.Rows)
                {
                    decimal sumDbt;
                    decimal sumPct;

                    if (rowPayment[t_S_IPaymnts.PayDate] != DBNull.Value)
                    {
                        var date = Convert.ToDateTime(rowPayment[t_S_IPaymnts.PayDate]);
                        var year = date.Year;
                        var month = date.Month;
                        sumDbt = GetDecimal(rowPayment[t_S_IPaymnts.NomSum]) *
                                 GetDecimal(rowPayment[t_S_IPaymnts.TotalCount]);
                        var colIndex = lstYears.IndexOf(year)*3 + 0;
                        var rowIndex = month - 1;
                        tblData.Rows[rowIndex][colIndex] = GetDecimal(tblData.Rows[rowIndex][colIndex]) + sumDbt;
                    }

                    if (rowPayment[t_S_IPaymnts.EndDate] != DBNull.Value)
                    {
                        var date = Convert.ToDateTime(rowPayment[t_S_IPaymnts.EndDate]);
                        var year = date.Year;
                        var month = date.Month;
                        sumPct = GetDecimal(rowPayment[t_S_IPaymnts.TotalCoupon]);
                        var colIndex = lstYears.IndexOf(year) * 3 + 1;
                        var rowIndex = month - 1;
                        tblData.Rows[rowIndex][colIndex] = GetDecimal(tblData.Rows[rowIndex][colIndex]) + sumPct;
                    }
                }
            }

            tables[tables.Length - 2] = tblData;
            var rowCaption = CreateReportParamsRow(tables);

            for (var i = 0; i < lstYears.Count; i++)
            {
                foreach (DataRow row in tblData.Rows)
                {
                    var startBlock = i * 3;
                    row[startBlock + 2] = GetDecimal(row[startBlock + 0]) + GetDecimal(row[startBlock + 1]);
                }

                rowCaption[10 + i] = lstYears[i];
            }

            rowCaption[0] = reportParams[ReportConsts.ParamOutputMode];
            rowCaption[1] = GetEnumItemIndex(new DebtLoadingListTypeEnum(), reportParams[ReportConsts.ParamOutputMode]);
            rowCaption[2] = lstYears.Count;
            return tables;
        }

        /// <summary>
        /// МО - Сравнение размещений займа
        /// </summary>
        public DataTable[] GetMOCompareCapitalBasementData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[7];

            for (var i = 0; i < tables.Length; i++)
            {
                tables[i] = CreateReportCaptionTable(20);
            }

            var multiCalcs = reportParams.ContainsKey(ReportConsts.ParamVariantID);
            var paramFilterName = !multiCalcs ? ReportConsts.ParamMasterFilter : ReportConsts.ParamVariantID;

            var currentNum = 1;
            var paramList = reportParams[paramFilterName].Split(',');
            var maxPayCount = 0;
            var maxCpnCount = 0;

            foreach (var paramID in paramList)
            {
                var localTables = new DataTable[tables.Length - 1];
                var fltParam = String.Format("={0}", paramID);
                // парметры
                var cdoParam = new CommonDataObject();
                cdoParam.InitObject(scheme);
                cdoParam.useSummaryRow = false;
                cdoParam.ObjectKey = f_S_ICalcParam.internalKey;
                cdoParam.mainFilter[f_S_ICalcParam.ID] = fltParam;
                cdoParam.AddDataColumn(f_S_ICalcParam.Name);
                cdoParam.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoParam.AddDataColumn(f_S_ICalcParam.CalcDate);
                cdoParam.AddDataColumn(f_S_ICalcParam.Basis);
                cdoParam.AddDataColumn(f_S_ICalcParam.Nominal);
                cdoParam.AddDataColumn(f_S_ICalcParam.CurrencyBorrow);
                cdoParam.AddDataColumn(f_S_ICalcParam.StartCpnDate);
                cdoParam.AddDataColumn(f_S_ICalcParam.CouponR);
                cdoParam.AddDataColumn(f_S_ICalcParam.YTM);
                cdoParam.AddDataColumn(f_S_ICalcParam.CurrPriceRub);
                cdoParam.AddDataColumn(f_S_ICalcParam.CurrPrice);
                cdoParam.AddDataColumn(f_S_ICalcParam.TotalCount);
                cdoParam.AddDataColumn(f_S_ICalcParam.TotalSum);
                cdoParam.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoParam.SetColumnNameParam(TempFieldNames.SortStatus);
                localTables[0] = cdoParam.FillData();
                // Погашение номинальной стоимости
                var cdoRepay = new CommonDataObject();
                cdoRepay.InitObject(scheme);
                cdoRepay.useSummaryRow = false;
                cdoRepay.ObjectKey = t_S_IRepaymentNom.internalKey;
                cdoRepay.mainFilter[t_S_IRepaymentNom.RefICalcParam] = fltParam;
                cdoRepay.AddDataColumn(t_S_IRepaymentNom.Num, ReportConsts.ftInt32);
                cdoRepay.AddDataColumn(t_S_IRepaymentNom.DayCount);
                cdoRepay.AddDataColumn(t_S_IRepaymentNom.NomSum);
                cdoRepay.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoRepay.SetColumnNameParam(TempFieldNames.SortStatus);
                cdoRepay.sortString = StrSortUp(t_S_IRepaymentNom.Num);
                localTables[1] = cdoRepay.FillData();
                maxPayCount = Math.Max(maxPayCount, localTables[1].Rows.Count);
                // Купоны
                var cdoCoupon = new CommonDataObject();
                cdoCoupon.InitObject(scheme);
                cdoCoupon.useSummaryRow = false;
                cdoCoupon.ObjectKey = t_S_ICoupons.internalKey;
                cdoCoupon.mainFilter[t_S_ICoupons.RefICalcParam] = fltParam;
                cdoCoupon.AddDataColumn(t_S_ICoupons.Num, ReportConsts.ftInt32);
                cdoCoupon.AddDataColumn(t_S_ICoupons.DayCpnCount);
                cdoCoupon.AddDataColumn(t_S_ICoupons.CouponRate);
                cdoCoupon.AddDataColumn(t_S_ICoupons.Nomi);
                cdoCoupon.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCoupon.SetColumnNameParam(TempFieldNames.SortStatus);
                cdoCoupon.sortString = StrSortUp(t_S_ICoupons.Num);
                localTables[2] = cdoCoupon.FillData();
                maxCpnCount = Math.Max(maxCpnCount, localTables[2].Rows.Count);
                // Размещение займа
                var cdoCapital = new CommonDataObject();
                cdoCapital.InitObject(scheme);
                cdoCapital.useSummaryRow = false;
                cdoCapital.ObjectKey = f_S_IssueBond.internalKey;
                cdoCapital.mainFilter[f_S_IssueBond.IDCalcParam] = fltParam;
                cdoCapital.AddDataColumn(f_S_IssueBond.Basis);
                cdoCapital.AddDataColumn(f_S_IssueBond.Nominal);
                cdoCapital.AddDataColumn(f_S_IssueBond.YTM);
                cdoCapital.AddDataColumn(f_S_IssueBond.CurrPriceRub);
                cdoCapital.AddDataColumn(f_S_IssueBond.CurrPrice);
                cdoCapital.AddDataColumn(f_S_IssueBond.DiffBtwNP);
                cdoCapital.AddDataColumn(f_S_IssueBond.TotalCount);
                cdoCapital.AddDataColumn(f_S_IssueBond.TotalSum);
                cdoCapital.AddDataColumn(f_S_IssueBond.TotalDiffBtwNP);
                cdoCapital.AddCalcColumn(CalcColumnType.cctUndefined);
                cdoCapital.SetColumnNameParam(TempFieldNames.SortStatus);
                var tblCapital = cdoCapital.FillData();
                localTables[3] = tblCapital;
                var rowCapital = GetLastRow(tblCapital);

                // Раcходы по выплате
                if (rowCapital != null)
                {
                    var fltCapital = String.Format("={0}", rowCapital[f_S_IssueBond.ID]);
                    var cdoCosts = new CommonDataObject();
                    cdoCosts.InitObject(scheme);
                    cdoCosts.useSummaryRow = false;
                    cdoCosts.ObjectKey = t_S_IPaymnts.internalKey;
                    cdoCosts.mainFilter[t_S_IPaymnts.RefIssBonds] = fltCapital;
                    cdoCosts.AddDataColumn(t_S_IPaymnts.Basis);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.Num);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.StartDate);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.EndDate);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.DayCpnCount);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.PayDate);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.NomSum);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.Nomi);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.Rate);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.Coupon);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.TotalCount);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.TotalCoupon);
                    cdoCosts.AddDataColumn(t_S_IPaymnts.DayCount);
                    cdoCosts.AddCalcColumn(CalcColumnType.cctUndefined);
                    cdoCosts.SetColumnNameParam(TempFieldNames.SortStatus);
                    var tblCostsFull = cdoCosts.FillData();
                    localTables[4] = tblCostsFull;
                    var tblCostsShort = tblCostsFull.Copy();
                    tblCostsShort.Columns.RemoveAt(3);
                    tblCostsShort.Columns.RemoveAt(2);

                    foreach (DataRow rowShort in tblCostsShort.Rows)
                    {
                        rowShort[t_S_IPaymnts.PayDate] = rowShort[t_S_IPaymnts.DayCount];
                    }

                    localTables[5] = tblCostsShort;
                }

                var currentTable = 0;
                foreach (var tblData in localTables)
                {
                    if (tblData != null)
                    {
                        var columnIndex = GetColumnIndex(tblData, TempFieldNames.SortStatus);

                        if (columnIndex >= 0)
                        {
                            SetColumnValue(tblData, columnIndex, currentNum);
                        }

                        if (currentNum == 1)
                        {
                            tables[currentTable] = tblData;
                        }
                        else
                        {
                            tables[currentTable].Merge(tblData);
                        }
                    }

                    currentTable++;
                }



                currentNum++;
            }

            var rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = currentNum - 1;
            rowCaption[1] = maxPayCount;
            rowCaption[2] = maxCpnCount;
            rowCaption[3] = multiCalcs;
            return tables;
        }

        /// <summary>
        /// МО - Планирование операций с ЦБ\Выкуп облигаций
        /// </summary>
        public DataTable[] GetMOCapitalRedemptionData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[1];
            var paramID = reportParams[ReportConsts.ParamMasterFilter];

            if (reportParams[ReportConsts.ParamVariantID] != ReportConsts.UndefinedKey)
            {
                paramID = reportParams[ReportConsts.ParamVariantID];
            }

            // парметры
            var cdoParam = new CommonDataObject();
            cdoParam.InitObject(scheme);
            cdoParam.useSummaryRow = false;
            cdoParam.ObjectKey = f_S_BondBuyback.internalKey;
            cdoParam.mainFilter[f_S_BondBuyback.ID] = paramID;
            cdoParam.AddDataColumn(f_S_BondBuyback.Name);
            cdoParam.AddDataColumn(f_S_BondBuyback.CalcDate);
            cdoParam.AddDataColumn(f_S_BondBuyback.OfficialNumber);
            cdoParam.AddDataColumn(f_S_BondBuyback.RedemptionDate);
            cdoParam.AddDataColumn(f_S_BondBuyback.StartCpnDate);
            cdoParam.AddDataColumn(f_S_BondBuyback.EndCpnDate);
            cdoParam.AddDataColumn(f_S_BondBuyback.CouponRate);
            cdoParam.AddDataColumn(f_S_BondBuyback.Nom);
            cdoParam.AddDataColumn(f_S_BondBuyback.Cpn);
            cdoParam.AddDataColumn(f_S_BondBuyback.AI);
            cdoParam.AddDataColumn(f_S_BondBuyback.YTM);
            cdoParam.AddDataColumn(f_S_BondBuyback.CPRub);
            cdoParam.AddDataColumn(f_S_BondBuyback.CP);
            cdoParam.AddDataColumn(f_S_BondBuyback.TotalCount);
            cdoParam.AddDataColumn(f_S_BondBuyback.TotalSum);
            cdoParam.AddDataColumn(f_S_BondBuyback.CostServLn);
            cdoParam.AddDataColumn(f_S_BondBuyback.TotalCPN);
            dtTables[0] = cdoParam.FillData();
            return dtTables;
        }

        /// <summary>
        /// МО - Планирование операций с ЦБ\Замена займа на кредит
        /// </summary>
        public DataTable[] GetMOExchangeCapitalCreditData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[1];
            var paramID = reportParams[ReportConsts.ParamMasterFilter];

            if (reportParams[ReportConsts.ParamVariantID] != ReportConsts.UndefinedKey)
            {
                paramID = reportParams[ReportConsts.ParamVariantID];
            }

            // парметры
            var cdoParam = new CommonDataObject();
            cdoParam.InitObject(scheme);
            cdoParam.useSummaryRow = false;
            cdoParam.ObjectKey = f_S_ReplaceIssForLn.internalKey;
            cdoParam.mainFilter[f_S_ReplaceIssForLn.ID] = paramID;
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.Name);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CalcDate);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.OfficialNumber);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.RedemptionDate);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.StartCpnDate);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.DateDischarge);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CouponRate);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.Nom);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.Cpn);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.AI);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.YTM);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CPRub);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CP);
            cdoParam.AddParamColumn(CalcColumnType.cctFlagColumn, f_S_ReplaceIssForLn.IsCoverNom);
            cdoParam.AddParamColumn(CalcColumnType.cctFlagColumn, f_S_ReplaceIssForLn.IsCalcRate);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CrdtRate);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.ServCrdt);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CostServLn);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CostServCrdt);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.Count);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.CrdtSum);
            cdoParam.AddDataColumn(f_S_ReplaceIssForLn.RepaymentSum);
            dtTables[0] = cdoParam.FillData();
            return dtTables;
        }

        /// <summary>
        /// МО - Размещение займа
        /// </summary>
        public DataTable[] GetMOCapitalBasementData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[6];

            for (var i = 0; i < dtTables.Length; i++)
            {
                dtTables[i] = CreateReportCaptionTable(20);
            }

            var paramID = reportParams[ReportConsts.ParamMasterFilter];
            var fltParam = String.Format("={0}", paramID);
            // парметры
            var cdoParam = new CommonDataObject();
            cdoParam.InitObject(scheme);
            cdoParam.useSummaryRow = false;
            cdoParam.ObjectKey = f_S_ICalcParam.internalKey;
            cdoParam.mainFilter[f_S_ICalcParam.ID] = fltParam;
            cdoParam.AddDataColumn(f_S_ICalcParam.Name);
            cdoParam.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoParam.AddDataColumn(f_S_ICalcParam.CalcDate);
            cdoParam.AddDataColumn(f_S_ICalcParam.Basis);
            cdoParam.AddDataColumn(f_S_ICalcParam.Nominal);
            cdoParam.AddDataColumn(f_S_ICalcParam.CurrencyBorrow);
            cdoParam.AddDataColumn(f_S_ICalcParam.StartCpnDate);
            cdoParam.AddDataColumn(f_S_ICalcParam.CouponR);
            cdoParam.AddDataColumn(f_S_ICalcParam.YTM);
            cdoParam.AddDataColumn(f_S_ICalcParam.CurrPriceRub);
            cdoParam.AddDataColumn(f_S_ICalcParam.CurrPrice);
            cdoParam.AddDataColumn(f_S_ICalcParam.TotalCount);
            cdoParam.AddDataColumn(f_S_ICalcParam.TotalSum);
            dtTables[0] = cdoParam.FillData();
            // Погашение номинальной стоимости
            var cdoRepay = new CommonDataObject();
            cdoRepay.InitObject(scheme);
            cdoRepay.useSummaryRow = false;
            cdoRepay.ObjectKey = t_S_IRepaymentNom.internalKey;
            cdoRepay.mainFilter[t_S_IRepaymentNom.RefICalcParam] = fltParam;
            cdoRepay.AddDataColumn(t_S_IRepaymentNom.Num);
            cdoRepay.AddDataColumn(t_S_IRepaymentNom.DayCount);
            cdoRepay.AddDataColumn(t_S_IRepaymentNom.NomSum);
            dtTables[1] = cdoRepay.FillData();
            // Купоны
            var cdoCoupon = new CommonDataObject();
            cdoCoupon.InitObject(scheme);
            cdoCoupon.useSummaryRow = false;
            cdoCoupon.ObjectKey = t_S_ICoupons.internalKey;
            cdoCoupon.mainFilter[t_S_ICoupons.RefICalcParam] = fltParam;
            cdoCoupon.AddDataColumn(t_S_ICoupons.Num);
            cdoCoupon.AddDataColumn(t_S_ICoupons.DayCpnCount);
            cdoCoupon.AddDataColumn(t_S_ICoupons.CouponRate);
            cdoCoupon.AddDataColumn(t_S_ICoupons.Nomi);
            dtTables[2] = cdoCoupon.FillData();
            // Размещение займа
            var cdoCapital = new CommonDataObject();
            cdoCapital.InitObject(scheme);
            cdoCapital.useSummaryRow = false;
            cdoCapital.ObjectKey = f_S_IssueBond.internalKey;
            cdoCapital.mainFilter[f_S_IssueBond.IDCalcParam] = fltParam;
            cdoCapital.AddDataColumn(f_S_IssueBond.Basis);
            cdoCapital.AddDataColumn(f_S_IssueBond.Nominal);
            cdoCapital.AddDataColumn(f_S_IssueBond.YTM);
            cdoCapital.AddDataColumn(f_S_IssueBond.CurrPriceRub);
            cdoCapital.AddDataColumn(f_S_IssueBond.CurrPrice);
            cdoCapital.AddDataColumn(f_S_IssueBond.DiffBtwNP);
            cdoCapital.AddDataColumn(f_S_IssueBond.TotalCount);
            cdoCapital.AddDataColumn(f_S_IssueBond.TotalSum);
            cdoCapital.AddDataColumn(f_S_IssueBond.TotalDiffBtwNP);
            var tblCapital = cdoCapital.FillData();
            dtTables[3] = tblCapital;
            var rowCapital = GetLastRow(tblCapital);

            // Раcходы по выплате
            if (rowCapital != null)
            {
                var fltCapital = String.Format("={0}", rowCapital[f_S_IssueBond.ID]);
                var cdoCosts = new CommonDataObject();
                cdoCosts.InitObject(scheme);
                cdoCosts.useSummaryRow = false;
                cdoCosts.ObjectKey = t_S_IPaymnts.internalKey;
                cdoCosts.mainFilter[t_S_IPaymnts.RefIssBonds] = fltCapital;
                cdoCosts.AddDataColumn(t_S_IPaymnts.Basis);
                cdoCosts.AddDataColumn(t_S_IPaymnts.Num);
                cdoCosts.AddDataColumn(t_S_IPaymnts.StartDate);
                cdoCosts.AddDataColumn(t_S_IPaymnts.EndDate);
                cdoCosts.AddDataColumn(t_S_IPaymnts.DayCpnCount);
                cdoCosts.AddDataColumn(t_S_IPaymnts.PayDate);
                cdoCosts.AddDataColumn(t_S_IPaymnts.NomSum);
                cdoCosts.AddDataColumn(t_S_IPaymnts.Nomi);
                cdoCosts.AddDataColumn(t_S_IPaymnts.Rate);
                cdoCosts.AddDataColumn(t_S_IPaymnts.Coupon);
                cdoCosts.AddDataColumn(t_S_IPaymnts.TotalCount);
                cdoCosts.AddDataColumn(t_S_IPaymnts.TotalCoupon);
                cdoCosts.AddDataColumn(t_S_IPaymnts.DayCount);
                var tblCostsFull = cdoCosts.FillData();
                dtTables[4] = tblCostsFull;
                var tblCostsShort = tblCostsFull.Copy();
                tblCostsShort.Columns.RemoveAt(3);
                tblCostsShort.Columns.RemoveAt(2);

                foreach (DataRow rowShort in tblCostsShort.Rows)
                {
                    rowShort[t_S_IPaymnts.PayDate] = rowShort[t_S_IPaymnts.DayCount];
                }

                dtTables[5] = tblCostsShort;
            }

            return dtTables;
        }

        /// <summary>
        /// МО - Карточка учета долга по займу
        /// </summary>
        public DataTable[] GetCapitalDebtHistoryMOData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[11];
            var officialNumber = reportParams[ReportConsts.ParamRegNum];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var cdoCapital = new CapitalDataObject();
            cdoCapital.InitObject(scheme);
            cdoCapital.mainFilter[f_S_Capital.OfficialNumber] = GetMOSelectedRegNumFilter(officialNumber);
            cdoCapital.useSummaryRow = false;
            // Формирование колонок
            cdoCapital.AddDataColumn(f_S_Capital.RegNumber);
            cdoCapital.AddCalcColumn(CalcColumnType.cctCapKind);
            cdoCapital.AddDataColumn(f_S_Capital.OfficialNumber);
            cdoCapital.AddDataColumn(f_S_Capital.RegEmissionDate);
            cdoCapital.AddCalcColumn(CalcColumnType.cctOKVFullName);
            cdoCapital.AddCalcColumn(CalcColumnType.cctPercentValues);
            cdoCapital.AddCalcColumn(CalcColumnType.cctCapCouponPeriod);
            cdoCapital.AddCalcColumn(CalcColumnType.cctUndefined);
            cdoCapital.AddDataColumn(f_S_Capital.DateDischarge);
            cdoCapital.AddDetailColumn(String.Format("[{0}](1<{1})", t_S_CPFactCapital.key, maxDate), CreateValuePair(t_S_CPFactCapital.Price), true);
            // служебные
            cdoCapital.AddDataColumn(f_S_Capital.RefOKV);
            cdoCapital.AddDataColumn(f_S_Capital.id);
            cdoCapital.AddDetailColumn(String.Format("[{0}][{1}][{2}][{3}][{4}][{5}]",
                t_S_CPPlanCapital.key,
                t_S_CPPlanDebt.key,
                t_S_CPFactCapital.key,
                t_S_CPFactDebt.key,
                t_S_CPFactService.key,
                t_S_CPPlanService.key));
            // Заполнение данными
            dtTables[0] = cdoCapital.FillData();

            var rowCapital = GetLastRow(dtTables[0]);

            if (rowCapital != null)
            {
                var sumField = ReportConsts.SumField;
                
                if (Convert.ToInt32(rowCapital[10]) != ReportConsts.codeRUB)
                {
                    sumField = ReportConsts.CurrencySumField;
                }
                
                var datesList = new Collection<string>();
                var loDate = DateTime.MinValue.ToShortDateString();
                var hiDate = calcDate;
                var masterKey = Convert.ToInt32(rowCapital[f_S_Capital.id]);
                var refField = cdoCapital.GetParentRefName();

                var cdoLines = new CapitalDataObject();
                cdoLines.InitObject(scheme);
                cdoLines.useSummaryRow = false;
                cdoLines.mainFilter[f_S_Capital.OfficialNumber] = String.Format("='{0}'", rowCapital[f_S_Capital.OfficialNumber]);
                cdoLines.mainFilter[f_S_Capital.id] = String.Format("<>{0}", masterKey);
                cdoLines.AddDataColumn(f_S_Capital.StartDate, ReportConsts.ftDateTime);
                cdoLines.sortString = FormSortString(f_S_Capital.StartDate);
                var tblLines = cdoLines.FillData();

                // 1. Формирование таблицы «Движение основного долга»

                // 1.1 Формирование таблицы «План»

                // список изменений по плану привлечения
                var detailIndex1 = Convert.ToInt32(t_S_CPPlanCapital.key);
                var tblDetail1 = cdoCapital.dtDetail[detailIndex1];
                var dateField1 = t_S_CPPlanCapital.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                // список изменений по плану погашения
                var detailIndex2 = Convert.ToInt32(t_S_CPPlanDebt.key);
                var tblDetail2 = cdoCapital.dtDetail[detailIndex2];
                var dateField2 = t_S_CPPlanDebt.EndDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, maxDate, refField, masterKey);
                var linesList = new Collection<string>();

                foreach (DataRow rowLine in tblLines.Rows)
                {
                    var lineKey = Convert.ToInt32(rowLine[f_S_Capital.id]);
                    linesList = FillDetailDateList(linesList, tblDetail2, dateField2, loDate, maxDate, refField, lineKey);                    
                }

                var tableDebtMoving = CreateReportCaptionTable(10);
                for (var i = 0; i < datesList.Count; i++)
                {
                    var rowDebt = tableDebtMoving.Rows.Add();
                    var changeDate = Convert.ToDateTime(datesList[i]);
                    rowDebt[0] = datesList[i];
                    var sumAttr = cdoCapital.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 8, cdoCapital.sumIncludedRows, t_S_CPPlanCapital.Note);
                    var sumDebt = cdoCapital.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 8, cdoCapital.sumIncludedRows, t_S_CPPlanDebt.Note);

                    if (linesList.Contains(datesList[i]))
                    {
                        var rowLineResult = tableDebtMoving.Rows.Add();
                        rowLineResult[0] = datesList[i];

                        foreach (DataRow rowLine in tblLines.Rows)
                        {
                            var lineKey = Convert.ToInt32(rowLine[f_S_Capital.id]);
                            var sumLineDebt = cdoCapital.GetSumValue(tblDetail2, lineKey, dateField2, sumField, changeDate, changeDate, true, true);
                            CombineCellValues(rowLineResult, 8, cdoCapital.sumIncludedRows, t_S_CPPlanDebt.Note);
                            rowLineResult[5] = sumLineDebt;
                        }
                    }

                    rowDebt[3] = sumAttr;
                    rowDebt[5] = sumDebt;
                }

                dtTables[1] = tableDebtMoving;

                // 1.2 Формирование таблицы «Факт»

                datesList.Clear();
                // список изменений по факту привлечения
                detailIndex1 = Convert.ToInt32(t_S_CPFactCapital.key);
                tblDetail1 = cdoCapital.dtDetail[detailIndex1];
                dateField1 = t_S_CPFactCapital.DateDoc;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, hiDate, refField, masterKey);
                // список изменений по факту погашения
                detailIndex2 = Convert.ToInt32(t_S_CPFactDebt.key);
                tblDetail2 = cdoCapital.dtDetail[detailIndex2];
                dateField2 = t_S_CPFactDebt.DateDischarge;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, hiDate, refField, masterKey);

                var tableDebtFact = CreateReportCaptionTable(11);
                for (var i = 0; i < datesList.Count; i++)
                {
                    var rowDebt = tableDebtFact.Rows.Add();
                    var changeDate = Convert.ToDateTime(datesList[i]);
                    rowDebt[0] = datesList[i];
                    rowDebt[3] = cdoCapital.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 7, cdoCapital.sumIncludedRows, t_S_CPFactCapital.Note);
                    CombineCellValues(rowDebt, 9, cdoCapital.sumIncludedRows, t_S_CPFactCapital.NumPayOrder);
                    rowDebt[5] = cdoCapital.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 7, cdoCapital.sumIncludedRows, t_S_CPFactDebt.Note);
                    CombineCellValues(rowDebt, 9, cdoCapital.sumIncludedRows, t_S_CPFactDebt.NumPayOrder);
                }

                dtTables[2] = tableDebtFact;

                // 2. Формирование раздела «Проценты»

                // 2.1 Формирование таблицы «План»

                //dtTables[3] = CreateReportCaptionTable(9);

                datesList.Clear();
                // список изменений по факту привлечения
                detailIndex1 = Convert.ToInt32(t_S_CPPlanService.key);
                tblDetail1 = cdoCapital.dtDetail[detailIndex1];
                dateField1 = t_S_CPPlanService.StartDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                var tablePercentAttr = CreateReportCaptionTable(9);
                for (var i = 0; i < datesList.Count; i++)
                {
                    var rowDebt = tablePercentAttr.Rows.Add();
                    var changeDate = Convert.ToDateTime(datesList[i]);
                    // поля по планам
                    rowDebt[4] = cdoCapital.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 0, cdoCapital.sumIncludedRows, t_S_CPPlanService.StartDate, true);
                    CombineCellValues(rowDebt, 1, cdoCapital.sumIncludedRows, t_S_CPPlanService.EndDate, true);
                    CombineCellValues(rowDebt, 2, cdoCapital.sumIncludedRows, t_S_CPPlanService.Period, true);
                    CombineCellValues(rowDebt, 6, cdoCapital.sumIncludedRows, t_S_CPPlanService.PaymentDate, true);
                    CombineCellValues(rowDebt, 7, cdoCapital.sumIncludedRows, t_S_CPPlanService.Note);
                }

                dtTables[3] = tablePercentAttr;

                // 2.2 Формирование таблицы «Факт»

                var tablePercentDebt = CreateReportCaptionTable(7);
                tablePercentAttr = CreateReportCaptionTable(5);
                // список НКД
                datesList.Clear();
                detailIndex2 = Convert.ToInt32(t_S_CPFactService.key);
                tblDetail2 = cdoCapital.dtDetail[detailIndex2];
                var fltCoupon = String.Format(
                    "{0} = {1}",
                    t_S_CPFactService.RefTypeSum, 
                    ReportConsts.CapTypeSumCoupon);
                tblDetail2 = DataTableUtils.FilterDataSet(tblDetail2, fltCoupon);
                dateField2 = t_S_CPFactService.OperationDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, hiDate, refField, masterKey);
                var dateListNKD = new Collection<string>();

                for (var i = 0; i < datesList.Count; i++)
                {
                    var changeDate = Convert.ToDateTime(datesList[i]);
                    var pctSum = cdoCapital.GetSumValue(tblDetail2, masterKey, dateField2, t_S_CPFactService.Sum, changeDate, changeDate, true, true);
                    var nkdSum = cdoCapital.GetSumValue(tblDetail2, masterKey, dateField2, t_S_CPFactService.DiscountSum, changeDate, changeDate, true, true);

                    if (pctSum <= 0)
                    {
                        tablePercentAttr.Rows.InsertAt(tablePercentAttr.NewRow(), 0);
                        dateListNKD.Add(datesList[i]);
                        var rowDebt = tablePercentDebt.Rows.Add();
                        rowDebt[3] = datesList[i];
                        rowDebt[2] = nkdSum;
                        CombineCellValues(rowDebt, 4, cdoCapital.sumIncludedRows, t_S_CPFactService.Note);
                    }
                }

                // список изменений по факту погашения
                datesList.Clear();
                dateField2 = t_S_CPFactService.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, hiDate, refField, masterKey);

                for (var i = 0; i < datesList.Count; i++)
                {
                    var changeDate = Convert.ToDateTime(datesList[i]);
                    var rowDebt = tablePercentDebt.Rows.Add();
                    rowDebt[0] = datesList[i];
                    rowDebt[1] = cdoCapital.GetSumValue(tblDetail2, masterKey, dateField2, sumField, changeDate, changeDate, true, true);;
                    CombineCellValues(rowDebt, 4, cdoCapital.sumIncludedRows, t_S_CPFactService.Note);
                    rowDebt[2] = cdoCapital.GetSumValue(tblDetail2, masterKey, dateField2, t_S_CPFactService.DiscountSum, changeDate, changeDate, true, true);
                    CombineCellValues(rowDebt, 3, cdoCapital.sumIncludedRows, t_S_CPFactService.OperationDate, true);

                    // это дата операции уже была в отдельном списке
                    if (dateListNKD.Contains(Convert.ToString(rowDebt[3])))
                    {
                        rowDebt[2] = null;
                        rowDebt[3] = null;
                    }

                    // если пустые погашения и нкд, то удаляем строку
                    if (Math.Abs(GetNumber(rowDebt[1])) <= 0 && Math.Abs(GetNumber(rowDebt[2])) <= 0)
                    {
                        tablePercentDebt.Rows.Remove(rowDebt);
                    }
                }

                dtTables[5] = tablePercentDebt;

                datesList.Clear();
                // список изменений по факту привлечения
                detailIndex1 = Convert.ToInt32(t_S_CPPlanService.key);
                tblDetail1 = cdoCapital.dtDetail[detailIndex1];
                dateField1 = t_S_CPPlanService.EndDate;
                datesList = FillDetailDateList(datesList, tblDetail1, dateField1, loDate, maxDate, refField, masterKey);
                detailIndex2 = Convert.ToInt32(t_S_CPFactService.key);
                tblDetail2 = cdoCapital.dtDetail[detailIndex2];
                tblDetail2 = DataTableUtils.FilterDataSet(tblDetail2, fltCoupon);
                tblDetail2 = DataTableUtils.FilterDataSet(tblDetail2, String.Format("{0} is not null", t_S_CPFactService.ChargeSum));
                dateField2 = t_S_CPFactService.FactDate;
                datesList = FillDetailDateList(datesList, tblDetail2, dateField2, loDate, maxDate, refField, masterKey);
                var listComments = new Dictionary<int, string>();
                
                for (var i = 0; i < datesList.Count; i++)
                {
                    var changeDate = Convert.ToDateTime(datesList[i]);
                    var sumPlan = cdoCapital.GetSumValue(tblDetail1, masterKey, dateField1, sumField, changeDate, changeDate, true, true);

                    if (Math.Abs(sumPlan) > 0)
                    {
                        var rowDebt = tablePercentAttr.Rows.Add();
                        CombineCellValues(rowDebt, 0, cdoCapital.sumIncludedRows, t_S_CPPlanService.StartDate, true);
                        CombineCellValues(rowDebt, 1, cdoCapital.sumIncludedRows, t_S_CPPlanService.EndDate, true);
                        CombineCellValues(rowDebt, 2, cdoCapital.sumIncludedRows, t_S_CPPlanService.Period, true);
                        rowDebt[3] = sumPlan;
                    }

                    var sumFact = cdoCapital.GetSumValue(tblDetail2, masterKey, dateField2, t_S_CPFactService.ChargeSum, changeDate, changeDate, true, true);

                    if (Math.Abs(sumFact) > 0)
                    {
                        var rowNum = tablePercentAttr.Rows.Count;
                        var rowDebt = tablePercentAttr.Rows.Add();
                        CombineCellValues(rowDebt, 1, cdoCapital.sumIncludedRows, t_S_CPFactService.FactDate, true);
                        CombineCellValues(rowDebt, 4, cdoCapital.sumIncludedRows, t_S_CPFactService.Note);
                        listComments.Add(rowNum, Convert.ToString(rowDebt[4]));
                        rowDebt[4] = DBNull.Value;
                        rowDebt[3] = sumFact;
                    }
                }

                foreach (var commentInfo in listComments)
                {
                    var newRow = tablePercentDebt.NewRow();
                    newRow[4] = commentInfo.Value;

                    while (tablePercentDebt.Rows.Count < commentInfo.Key)
                    {
                        tablePercentDebt.Rows.Add();
                    }

                    tablePercentDebt.Rows.InsertAt(newRow, commentInfo.Key);
                }

                dtTables[4] = tablePercentAttr;

                // 2.3 Формирование таблицы «Комиссии»

                dtTables[6] = CreateReportCaptionTable(1);

                // 2.4 Формирование таблицы «Санкции»

                var cdoCommission = new CommonDataObject();
                cdoCommission.InitObject(scheme);
                cdoCommission.useSummaryRow = false;
                cdoCommission.ObjectKey = t_S_CPFactService.InternalKey;
                cdoCommission.mainFilter[t_S_CPFactService.RefTypeSum] = ReportConsts.CapTypeSumCommission;
                cdoCommission.mainFilter[t_S_CPFactService.RefCap] = String.Format("={0}", masterKey);
                cdoCommission.AddDataColumn(t_S_CPFactService.FactDate);
                cdoCommission.AddDataColumn(t_S_CPFactService.Sum);
                cdoCommission.AddDataColumn(t_S_CPFactService.Note);
                dtTables[7] = cdoCommission.FillData();

                // 2.5 Формирование таблицы «Дисконт»
                var cdoDiscount = new CommonDataObject();
                cdoDiscount.InitObject(scheme);
                cdoDiscount.useSummaryRow = false;
                cdoDiscount.ObjectKey = t_S_CPFactService.InternalKey;
                cdoDiscount.mainFilter[t_S_CPFactService.RefTypeSum] = ReportConsts.CapTypeSumDiscount;
                cdoDiscount.mainFilter[t_S_CPFactService.RefCap] = String.Format("={0}", masterKey);
                cdoDiscount.AddDataColumn(t_S_CPFactService.FactDate);
                cdoDiscount.AddDataColumn(t_S_CPFactService.Sum);
                cdoDiscount.AddDataColumn(t_S_CPFactService.Note);
                dtTables[8] = cdoDiscount.FillData();
                dtTables[9] = CreateReportCaptionTable(1);
            }
            else
            {
                for (var i = 1; i < dtTables.Length - 1; i++)
                {
                    dtTables[i] = CreateReportCaptionTable(50);
                }
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = reportParams[ReportConsts.ParamRegNum];
            FillSignatureData(drCaption, 4, reportParams, ReportConsts.ParamExecutor1, ReportConsts.ParamExecutor2, ReportConsts.ParamExecutor3);
            return dtTables;
        }

        /// <summary>
        /// Ценные бумаги ДК МФРФ
        /// </summary>
        public DataTable[] GetDKMFRFSubjectCapitalData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();

            var couponSumFieldName = Combine(t_S_CPFactService.Sum, t_S_CPFactService.DiscountSum);
            var couponFieldList = CombineList(couponSumFieldName, couponSumFieldName);

            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.nonEmptySum = true;
            cdoCap.mainFilter.Add(f_S_Capital.StartDate, String.Format("<'{0}'", calcDate));
            cdoCap.mainFilter.Add(f_S_Capital.RefVariant, ReportConsts.ActiveVariantID);
            cdoCap.reportParams.Add(ReportConsts.ParamHiDate, calcDate);
            // 0
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            // 1
            cdoCap.AddCalcColumn(CalcColumnType.cctCapKind);
            // 2 
            cdoCap.AddCalcColumn(CalcColumnType.cctCapForm);
            // 3
            cdoCap.AddDataColumn(f_S_Capital.RegEmissnNumber);
            // 4
            cdoCap.AddDataColumn(f_S_Capital.RegEmissionDate);
            // 5
            cdoCap.AddDataColumn(f_S_Capital.NameNPA);
            // 6
            cdoCap.AddCalcColumn(CalcColumnType.cctOKVName);
            // 7
            cdoCap.AddDetailColumn(String.Format("[5](1<{0})", maxDate));
            // 8
            cdoCap.AddCalcColumn(CalcColumnType.cctUndefined);
            // 9
            cdoCap.AddDataColumn(f_S_Capital.Owner);
            // 10
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            // 11
            cdoCap.AddDataColumn(f_S_Capital.DateDischarge);
            // 12
            cdoCap.AddParamColumn(CalcColumnType.cctOperationDates, t_S_CPFactDebt.key);
            // 13
            cdoCap.AddDetailColumn(String.Format("[0](1<{0})", calcDate));
            cdoCap.SetColumnCondition(t_S_CPFactCapital.Secondary, ReportConsts.CapRowPrimary);
            // 14
            cdoCap.AddDetailColumn(String.Format("[6](1<{0})", calcDate));
            // 15
            cdoCap.AddParamColumn(CalcColumnType.cctOperationDates, t_S_CPPlanService.key);
            cdoCap.SetColumnParam(cdoCap.ParamValue3, ReportConsts.strTrue);
            // 16
            cdoCap.AddParamColumn(CalcColumnType.cctOperationDates, t_S_CPPlanService.key, t_S_CPPlanService.Per);
            cdoCap.SetColumnParam(cdoCap.ParamValue3, ReportConsts.strTrue);
            // 17
            cdoCap.AddDetailTextColumn(String.Format("[7](1<{0})", maxDate), cdoCap.ParamFieldList, CreateValuePair(t_S_CPPlanService.Income));
            cdoCap.SetColumnParam(cdoCap.ParamOnlyValues, String.Empty);
            // 18 слить с нкд
            cdoCap.AddDetailTextColumn(String.Format("[3](1<{0})", calcDate), cdoCap.ParamOnlyValues, String.Empty);
            cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, ReportConsts.CapTypeSumCoupon);
            cdoCap.SetColumnParam(cdoCap.ParamFieldList, couponFieldList);
            cdoCap.SetColumnParam(cdoCap.ParamSumValueType, "+");
            // 19
            cdoCap.AddParamColumn(CalcColumnType.cctNearestDown, t_S_CPPlanService.key, t_S_CPPlanService.Discount);  
            // 20
            cdoCap.AddDetailColumn(String.Format("[3](1<{0})", calcDate));
            cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, ReportConsts.CapTypeSumDiscount);
            // 21
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+29;+20;+36");
            // 22
            cdoCap.AddDataColumn(f_S_Capital.ShortGenAgent);
            // 23
            cdoCap.AddDataColumn(f_S_Capital.ShrtDepository);
            // 24
            cdoCap.AddDataColumn(f_S_Capital.ShrtTrade);
            // 25
            cdoCap.AddDetailColumn(String.Format("[3](1>{0})", maxDate));
            // 26
            cdoCap.AddDetailColumn(String.Format("[1](1>{0})", maxDate));
            // 27 
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+25;+26");
            // 28
            cdoCap.AddDetailColumn(String.Format("-[0](1<{0})[1](1<{0})", calcDate));
            // 29
            cdoCap.AddDetailColumn(String.Format("[3](1<{0})", calcDate));
            cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, ReportConsts.CapTypeSumCoupon);
            // 30
            cdoCap.AddDetailTextColumn(String.Format("[1](1<{0})", maxDate), cdoCap.ParamOnlyDates, String.Empty);
            // 31
            cdoCap.AddDataColumn(f_S_Capital.ExstraIssue);
            // 32
            cdoCap.AddDataColumn(f_S_Capital.StartDate, ReportConsts.ftDateTime);
            // 33
            cdoCap.AddParamColumn(CalcColumnType.cctRecordCount, t_S_CPPlanDebt.key);
            // 34 
            cdoCap.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.RowType);
            // 35
            cdoCap.AddDetailTextColumn(String.Format("[5](0<{0})", maxDate), cdoCap.ParamOnlyValues, String.Empty);
            // 36
            cdoCap.AddDetailColumn(String.Format("[3](1<{0})", calcDate));
            cdoCap.SetColumnCondition(t_S_CPFactService.RefTypeSum, ReportConsts.CapTypeSumCommission);

            // Приехали         
            cdoCap.summaryColumnIndex.Add(14);
            cdoCap.sortString = StrSortUp(f_S_Capital.StartDate);
            var tblCap = cdoCap.FillData();
            var tblCapResult = tblCap.Clone();
            var capNumbers = new Collection<string>();

            foreach (DataRow dr in tblCap.Rows)
            {
                var officialNum = dr[f_S_Capital.OfficialNumber].ToString();

                if (officialNum.Length > 0 && !capNumbers.Contains(officialNum))
                {
                    capNumbers.Add(officialNum);
                }


                if (Convert.ToString(dr[f_S_Capital.id]).Length > 0)
                {
                    var cntPlanDebt = Convert.ToInt32(dr[33]);

                    if (cntPlanDebt < 2)
                    {
                        dr[14] = DBNull.Value;
                        dr[12] = DBNull.Value;
                    }
                }

                if (dr[32] != DBNull.Value)
                {
                    dr[8] = Convert.ToDateTime(dr[32]).ToShortDateString();
                }
            }

            tblCap = cdoCap.RecalcSummary(tblCap);

            foreach (var officialNum in capNumbers)
            {
                var rowsCap = tblCap.Select(
                    String.Format("{0} = '{1}'", f_S_Capital.OfficialNumber, officialNum),
                    FormSortString(f_S_Capital.ExstraIssue, f_S_Capital.StartDate));
                var rowResult = rowsCap[0];
                rowResult[TempFieldNames.RowType] = 1;
                var planSumList = Convert.ToString(rowResult[35]).Split(new [] {", "}, StringSplitOptions.RemoveEmptyEntries);

                if (planSumList.Length > 0)
                {
                    rowResult[7] = planSumList[0];
                }

                tblCapResult.ImportRow(rowResult);

                for (var i = 1; i < rowsCap.Length; i++)
                {
                    var rowCap = rowsCap[i];
                    rowCap[TempFieldNames.RowType] = 2;

                    if (planSumList.Length > i)
                    {
                        rowCap[7] = planSumList[i];
                    }

                    tblCapResult.ImportRow(rowCap);
                }
            }

            tblCapResult.ImportRow(GetLastRow(tblCap));
            var rowSummary = GetLastRow(tblCapResult);
            rowSummary[TempFieldNames.RowType] = 3;

            dtTables[0] = ClearCloseCreditMO(cdoCap, tblCapResult, Convert.ToDateTime(calcDate), 28, 30, -1);

            // заголовочные штуки
            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            FillSignatureData(drCaption, 10, reportParams, ReportConsts.ParamExecutor1);
            return dtTables;
        }

        /// <summary>
        /// Ценные бумаги Самара
        /// </summary>
        public DataTable GetCapitalSamaraData(Dictionary<string, string> reportParams, DataRow drCaption)
        {
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var maxDate = DateTime.MaxValue.ToShortDateString();
            var sumFieldPair = CreateValuePair(ReportConsts.SumField);
            var cdoCap = new CapitalDataObject();
            cdoCap.InitObject(scheme);
            cdoCap.reportParams.Add(ReportConsts.ParamHiDate, calcDate);
            SetCapitalFilter(cdoCap.mainFilter, calcDate, calcDate);
            // 0
            cdoCap.AddCalcColumn(CalcColumnType.cctPosition);
            // 1
            cdoCap.AddDataColumn(f_S_Capital.RegNumber);
            // 2 
            cdoCap.AddCalcColumn(CalcColumnType.cctCapNPANames);
            // 3
            cdoCap.AddDataColumn(f_S_Capital.Purpose);
            // 4
            cdoCap.AddCalcColumn(CalcColumnType.cctCapNumberRegDate);
            // 5
            cdoCap.AddDataColumn(f_S_Capital.OfficialNumber);
            // 6
            cdoCap.AddCalcColumn(CalcColumnType.cctCapNameKind);
            // 7
            cdoCap.AddCalcColumn(CalcColumnType.cctCapForm);
            // 8
            cdoCap.AddCalcColumn(CalcColumnType.cctOrganization);
            // 9
            cdoCap.AddDataColumn(f_S_Capital.Depository);
            // 10
            cdoCap.AddDataColumn(f_S_Capital.Trade);
            // 11
            cdoCap.AddDataColumn(f_S_Capital.StartDate);
            // 12
            cdoCap.AddDataColumn(f_S_Capital.DateDischarge);
            // 13
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            // 14
            cdoCap.AddDetailColumn(String.Format("[0](1<={0})", calcDate), sumFieldPair, true);
            // 15
            cdoCap.AddCalcColumn(CalcColumnType.cctPercentValues2);
            // 16
            cdoCap.AddCalcColumn(CalcColumnType.cctCapCoupon);
            // 17
            cdoCap.AddCalcColumn(CalcColumnType.cctCapPayPeriod);
            // 18
            cdoCap.AddDetailColumn(String.Format("+[0](1<={0})[7](1<={1})", calcDate, maxDate), sumFieldPair, true);
            // 19
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+14;-25");
            // 20
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+14;-25;+28");
            // 21
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+22;+23");
            // 22
            cdoCap.AddDetailColumn(String.Format("[6](1<={0})", calcDate));
            // 23
            cdoCap.AddDetailColumn(String.Format("[7](1<={0})", calcDate));
            // 24
            cdoCap.AddParamColumn(CalcColumnType.cctRelation, "+25;+26");
            // 25
            cdoCap.AddDetailColumn(String.Format("[1](1<={0})", calcDate));
            // 26
            cdoCap.AddDetailColumn(String.Format("[3](1<={0})", calcDate));
            // 27 
            cdoCap.AddDataColumn(f_S_Capital.Note);
            // 28
            cdoCap.AddDetailColumn(String.Format("-[7](1<={1})[3](1<={0})", calcDate, maxDate), sumFieldPair, true);
            // Приехали            
            var dtTable = cdoCap.FillData();
            FillExchangeValueSamara(drCaption, 6, cdoCap);
            return dtTable;
        }

        /// <summary>
        /// Приложение 6. Отчет о выполнении обязательств по государственным облигациям
        /// </summary>
        public DataTable[] GetStateDebtApplication6YarData(Dictionary<string, string> reportParams)
        {
            var dtTables = new DataTable[2];
            var cdo = new CapitalDataObject();
            var calcDate = GetParamDate(reportParams, ReportConsts.ParamEndDate);
            var dateYearStart = GetYearStart(calcDate);
            cdo.InitObject(scheme);
            cdo.useSummaryRow = false;
            cdo.ignoreNegativeFact = true;
            cdo.mainFilter[f_S_Capital.RefVariant] = ReportConsts.ActiveVariantID;
            // 0
            cdo.AddDataColumn(f_S_Capital.OfficialNumber);
            // 1
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+8;-9;*7");
            // 2
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // 3
            cdo.AddCalcColumn(CalcColumnType.cctUndefined);
            // 4
            cdo.AddParamColumn(CalcColumnType.cctRelation, "+10;-11;*7");
            // 5
            cdo.AddDetailColumn(String.Format("[7](1>={0}1<{1})", dateYearStart, calcDate));
            // 6
            cdo.AddDetailColumn(String.Format("[3](1>={0}1<{1})", dateYearStart, calcDate));
            cdo.SetColumnCondition(t_S_CPFactService.RefTypeSum, "8");
            // служебные
            // 7
            cdo.AddDataColumn(f_S_Capital.Nominal);
            // 8
            cdo.AddDetailColumn(String.Format("[0](1<{0})", dateYearStart), CreateValuePair(t_S_CPFactCapital.Quantity), true);
            // 9
            cdo.AddDetailColumn(String.Format("[1](1<{0})", dateYearStart), CreateValuePair(t_S_CPFactDebt.Quantity), true);
            // 10
            cdo.AddDetailColumn(String.Format("[0](1<{0})", calcDate), CreateValuePair(t_S_CPFactCapital.Quantity), true);
            // 11
            cdo.AddDetailColumn(String.Format("[1](1<{0})", calcDate), CreateValuePair(t_S_CPFactDebt.Quantity), true);
            cdo.AddDataColumn(f_S_Capital.StartDate, ReportConsts.ftDateTime);
            cdo.sortString = StrSortUp(f_S_Capital.StartDate);
            dtTables[0] = cdo.FillData();

            foreach (var index in cdo.summaryColumnIndex)
            {
                CorrectBillionSumValue(dtTables[0], index);
            }

            var drCaption = CreateReportParamsRow(dtTables);
            drCaption[0] = calcDate;
            drCaption[1] = dateYearStart;
            return dtTables;
        }

        private decimal[] GetCapitalSum(CommonQueryParam procParams)
        {
            var cdoCap = new CapitalDataObject();
            var result = new decimal[4];
            cdoCap.InitObject(scheme);
            cdoCap.mainFilter.Add(f_S_Capital.RefVariant, Combine(ReportConsts.ActiveVariantID, procParams.variantID));
            cdoCap.mainFilter.Add(f_S_Capital.SourceID, procParams.sourceID);
            cdoCap.AddDataColumn(f_S_Capital.Nominal);
            cdoCap.AddDetailColumn(String.Format("[1](1<={0})", procParams.yearStart));
            cdoCap.AddDetailColumn(String.Format("[6](1<={0})", procParams.yearStart));
            cdoCap.AddDetailColumn(String.Format("[0](1<={0})", procParams.yearStart));
            cdoCap.AddDetailColumn(String.Format("[5](1<={0})", procParams.yearStart));
            cdoCap.summaryColumnIndex.Add(0);
            var dtCapital = cdoCap.FillData();
            decimal cSum = 0;
            decimal cPlanSum = 0;
            var dtFactCapital = cdoCap.dtDetail[0];
            var dtPlanCapital = cdoCap.dtDetail[5];
            foreach (DataRow dr in dtCapital.Rows)
            {
                if (dr[f_S_Capital.id] != DBNull.Value)
                {
                    var id = Convert.ToInt32(dr[f_S_Capital.id]);
                    var nominal = GetNumber(dr[f_S_Capital.Nominal]);
                    var drs1 = dtFactCapital.Select(String.Format("{0} = {1}", t_S_CPFactCapital.RefCap, id));
                    var drs2 = dtPlanCapital.Select(String.Format("{0} = {1} and {2} < '{3}'",
                        t_S_CPPlanCapital.RefCap, id, t_S_CPPlanCapital.EndDate, procParams.yearStart));
                    decimal cSumFactDetail = 0;
                    foreach (var dr1 in drs1)
                        if (dr1[t_S_CPFactCapital.Quantity] != DBNull.Value)
                            cSumFactDetail += GetNumber(dr1[t_S_CPFactCapital.Quantity]);
                    decimal cSumPlanDetail = 0;
                    foreach (var dr2 in drs2)
                        if (dr2[t_S_CPPlanCapital.Quantity] != DBNull.Value)
                            cSumPlanDetail += GetNumber(dr2[t_S_CPPlanCapital.Quantity]);
                    cSum = nominal * cSumFactDetail + nominal * (cSumPlanDetail - cSumFactDetail);
                    cPlanSum += nominal * cSumPlanDetail;
                }
            }

            var cSumFact = GetLastRowValue(dtCapital, 0);
            var cSumPlan = GetLastRowValue(dtCapital, 1);
            result[0] = ConvertTo1000(cPlanSum);
            result[1] = ConvertTo1000(cSumFact);
            result[2] = ConvertTo1000(cSumPlan);
            result[3] = ConvertTo1000(cSum - (2 * cSumFact - cSumPlan));
            return result;
        }
    }
}