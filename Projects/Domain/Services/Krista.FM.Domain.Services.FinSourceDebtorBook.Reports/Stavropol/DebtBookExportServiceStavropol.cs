using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    public enum RecordType
    {
        /// <summary>
        /// Строка данных
        /// </summary>
        Data = 0,

        /// <summary>
        /// Итог по поселениям
        /// </summary>
        SummarySettle = 1,

        /// <summary>
        /// Итог по району
        /// </summary>
        SummaryMR = 2,

        /// <summary>
        /// Итог по всем районам
        /// </summary>
        SummaryAllMR = 3,

        /// <summary>
        /// Итог по всем го
        /// </summary>
        SummaryAllGO = 4,

        /// <summary>
        /// Итог по всему
        /// </summary>
        SummaryTotal = 5,

        /// <summary>
        /// Итог по ГО
        /// </summary>
        SummaryGO = 6,

        /// <summary>
        /// Итог по субъекту
        /// </summary>
        SummarySB = 7,

        /// <summary>
        /// Заголовочная строка
        /// </summary>
        Caption = 8,

        /// <summary>
        /// Итог по субъекту
        /// </summary>
        SummaryMrWoSettles = 9,
    }

    /// <summary>
    /// Получение данных - Москва
    /// </summary>
    public class DebtBookExportServiceStavropol : IDebtBookExportServiceStavropol
    {
        private const int ColRowType = 0;
        private const int ResultColumnCount = 30;
        private const int ServiceColumnCount = 5;
        private const int CodeGP = 5;
        private const int CodeSP = 6;
        private const int CodeMR = 4;
        private const int CodeGO = 7;
        private const int CodeSB = 3;
        private readonly ILinqRepository<F_S_SchBLimit> limitRepository;
        private readonly ILinqRepository<D_Regions_Analysis> regRepository;
        private readonly ILinqRepository<F_S_SchBCreditincome> crdRepository;
        private readonly ILinqRepository<F_S_SchBCapital> capRepository;
        private readonly ILinqRepository<F_S_SchBGuarantissued> grnRepositopry;
        private readonly ILinqRepository<D_Variant_Schuldbuch> variantRepository;
        private decimal[,] currentSummary;
        private DataTable currentTable;
        private bool chkRegion;
        private int regionId;
        private int variantId;
        private List<int> settleList;
        private D_Regions_Analysis userRegion;
        private D_Regions_Analysis curRegion;
        private D_Regions_Analysis oldRegion;
        private List<int> lstExcludeSummaryColumn = new List<int>();

        public DebtBookExportServiceStavropol(
            ILinqRepository<F_S_SchBLimit> limitRepository,
            ILinqRepository<D_Regions_Analysis> regRepository,
            ILinqRepository<D_Variant_Schuldbuch> variantRepository,
            ILinqRepository<F_S_SchBCreditincome> crdRepository,
            ILinqRepository<F_S_SchBCapital> capRepository,
            ILinqRepository<F_S_SchBGuarantissued> grnRepositopry)
        {
            this.limitRepository = limitRepository;
            this.variantRepository = variantRepository;
            this.regRepository = regRepository;
            this.crdRepository = crdRepository;
            this.capRepository = capRepository;
            this.grnRepositopry = grnRepositopry;
        }

        public RecordType GetRecordType(object value)
        {
            return (RecordType)Convert.ToInt32(value);
        }

        public DataTable[] GetDebtBookStavropolData(int refVariant, int refRegion, DateTime calculateDate)
        {
            const string TemplateNumDate = "№{0} от {1:d}";
            const int CodeSubject = 3;
            const int CapRegionIndex = 0;
            const int CrdOrgRegionIndex = 1;
            const int CrdBudRegionIndex = 2;
            const int GrnRegionIndex = 3;
            const int LimRegionIndex = 4;
            const int SumRegionIndex = 5;
            var tables = new DataTable[7];
            var currentRegion = regRepository.FindOne(refRegion);
            userRegion = currentRegion;
            chkRegion = refRegion >= 0 && currentRegion.RefTerr.ID != CodeSubject;
            regionId = refRegion;
            settleList = new List<int>();
            variantId = refVariant;
            var currentVariant = variantRepository.FindOne(refVariant);

            if (chkRegion)
            {
                var childs = regRepository.FindAll()
                    .Where(x => x.ParentID == refRegion)
                    .Select(child => child.ID)
                    .ToList();

                settleList = regRepository.FindAll()
                    .Where(x => childs.Contains(x.ParentID.Value))
                    .Select(s => s.ID)
                    .ToList();
            }

            for (var i = 0; i < tables.Length; i++)
            {
                tables[i] = ReportsDataService.CreateReportCaptionTable(ResultColumnCount);
            }

            var queryCrdOrg = GetCredits(0);
            var queryCrdBud = GetCredits(1);
            var queryCap = GetCapitals();
            var queryGrn = GetGarants();
            var queryLim = GetLimits();

            var capSplitSummarySplit = new decimal[10, ResultColumnCount];
            var grnSplitSummarySplit = new decimal[10, ResultColumnCount];
            var crbSplitSummarySplit = new decimal[10, ResultColumnCount];
            var croSplitSummarySplit = new decimal[10, ResultColumnCount];
            var limSplitSummarySplit = new decimal[10, ResultColumnCount];

            var curType = -1;
            var curId = -1;
            var oldType = -1;
            var oldId = -1;

            currentSummary = capSplitSummarySplit;
            currentTable = tables[CapRegionIndex];

            var lstCap = SortContractList(queryCap.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBCapital>();
            var hasCaption = false;
            lstExcludeSummaryColumn = new List<int> { 14 };

            foreach (var row in lstCap)
            {
                curType = row.RefRegion.RefTerr.ID;
                curId = GetRegionParentId(row.RefRegion.ID, row.RefRegion.ParentID, curType);
                var region = regRepository.FindOne(Convert.ToInt32(curId));
                curRegion = region;

                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data), // 0
                                     row.RefRegion.RefTerr.ID, // 1
                                     curId, // 2
                                     String.Empty, // 3
                                     String.Empty, // 4
                                     String.Format("{0}, {1}, {2}", row.RefRegion.Name, row.GenAgent, row.Depository), // 5
                                     String.Format(TemplateNumDate, row.NumberNPA, row.DateNPA), // 6
                                     String.Format(
                                         "{0}, {1}, {2}, {3}, {4}",
                                         ReportsDataService.GetDateValue(row.RegEmissionDate),
                                         row.RefKindCapital.Name,
                                         row.RefFofmCapital.Name,
                                         row.Count,
                                         row.Nominal), // 7
                                     row.RegNumber, // 8
                                     row.OfficialNumber, // 9
                                     ReportsDataService.GetDateValue(row.StartDate), // 10
                                     row.RefOKV.Name, // 11
                                     row.Sum, // 12
                                     row.IssueSum, // 13
                                     row.Coupon, // 14
                                     row.Collateral, // 15
                                     ReportsDataService.GetDateValue(row.DateDischarge), // 16
                                     ReportsDataService.GetDateValue(row.DatePartDischarge), // 17
                                     row.RemnsEndMnthDbt, // 18
                                     row.StaleDebt, // 19
                                     row.Note // 20
                                 };
                
                var hasSummary = AddSummaries(curId, curType, oldId, oldType);

                if (hasSummary || !hasCaption)
                {
                    AddTitle(region);
                    hasCaption = true;
                }

                UpdateSummaries(values, curType, curId, oldId);
                currentTable.Rows.Add(values);
                oldId = curId;
                oldType = curType;
                oldRegion = curRegion;
            }

            AddSummaries(-1, -1, oldId, oldType);
            AddSummaryRow(currentTable, RecordType.SummaryTotal);

            oldType = -1;
            oldId = -1;

            currentSummary = croSplitSummarySplit;
            currentTable = tables[CrdOrgRegionIndex];

            var lstCrdOrg = SortContractList(queryCrdOrg.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBCreditincome>();
            hasCaption = false;
            lstExcludeSummaryColumn = new List<int> { 12 };

            foreach (var row in lstCrdOrg)
            {
                curType = row.RefRegion.RefTerr.ID;
                curId = GetRegionParentId(row.RefRegion.ID, row.RefRegion.ParentID, curType);
                var region = regRepository.FindOne(Convert.ToInt32(curId));
                curRegion = region;
                var settleName = curType == CodeSP || curType == CodeGP
                                     ? String.Format(", {0}", row.RefRegion.Name)
                                     : String.Empty;

                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data), // 0
                                     row.RefRegion.RefTerr.ID, // 1
                                     curId, // 2
                                     String.Empty, // 3
                                     String.Empty, // 4
                                     String.Format("{0}{1}", row.Num, settleName), // 5
                                     row.Creditor, // 6
                                     row.Occasion, // 7
                                     row.Collateral, // 8
                                     ReportsDataService.GetDateValue(row.ContractDate), // 9
                                     ReportsDataService.GetDateValue(row.PaymentDate), // 10
                                     row.RefOKV.Name, // 11
                                     row.CreditPercentNum, // 12
                                     row.Attract, // 13
                                     row.Discharge, // 14
                                     row.CapitalDebt, // 15
                                     row.StaleDebt, // 16
                                     row.FactDate, // 17
                                     row.PlanService, // 18
                                     row.FactService, // 19
                                     row.ServiceDebt, // 20
                                     row.ChargePenlt, // 21
                                     row.FactPenlt, // 22
                                     row.RemnsEndMnthPenlt, // 23
                                     row.Note // 24
                                 };

                var hasSummary = AddSummaries(curId, curType, oldId, oldType);

                if (hasSummary || !hasCaption)
                {
                    AddTitle(region);
                    hasCaption = true;
                }

                UpdateSummaries(values, curType, curId, oldId);
                currentTable.Rows.Add(values);
                oldId = curId;
                oldType = curType;
                oldRegion = curRegion;
            }

            AddSummaries(-1, -1, oldId, oldType);
            AddSummaryRow(currentTable, RecordType.SummaryTotal);

            oldType = -1;
            oldId = -1;

            currentSummary = grnSplitSummarySplit;
            currentTable = tables[GrnRegionIndex];

            var lstGrn = SortContractList(queryGrn.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBGuarantissued>();
            hasCaption = false;
            lstExcludeSummaryColumn = new List<int>();

            foreach (var row in lstGrn)
            {
                curType = row.RefRegion.RefTerr.ID;
                curId = GetRegionParentId(row.RefRegion.ID, row.RefRegion.ParentID, curType);
                var region = regRepository.FindOne(Convert.ToInt32(curId));
                curRegion = region;

                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data), // 0
                                     row.RefRegion.RefTerr.ID, // 1
                                     curId, // 2
                                     String.Empty, // 3
                                     String.Empty, // 4
                                     row.Num, // 5
                                     row.PrincipalNum, // 6
                                     row.Creditor, // 7
                                     row.Principal, // 8
                                     row.RefRegion.Name, // 9
                                     row.Collateral, // 10
                                     ReportsDataService.GetDateValue(row.StartDate), // 11
                                     row.RefOKV.Name, // 12
                                     row.Sum, // 13
                                     row.StalePrincipalDebt, // 14
                                     ReportsDataService.GetDateValue(row.PrincipalEndDate), // 15
                                     ReportsDataService.GetDateValue(row.EndCreditDate), // 16
                                     row.DateDemand, // 17
                                     ReportsDataService.GetDateValue(row.RenewalDate), // 18
                                     row.Purpose, // 19
                                     row.Note // 20
                                 };

                var hasSummary = AddSummaries(curId, curType, oldId, oldType);

                if (hasSummary || !hasCaption)
                {
                    AddTitle(region);
                    hasCaption = true;
                }

                UpdateSummaries(values, curType, curId, oldId);
                currentTable.Rows.Add(values);
                oldId = curId;
                oldType = curType;
                oldRegion = curRegion;
            }

            AddSummaries(-1, -1, oldId, oldType);
            AddSummaryRow(currentTable, RecordType.SummaryTotal);

            oldType = -1;
            oldId = -1;

            currentSummary = crbSplitSummarySplit;
            currentTable = tables[CrdBudRegionIndex];

            var lstCrdBud = SortContractList(queryCrdBud.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBCreditincome>();
            hasCaption = false;
            lstExcludeSummaryColumn = new List<int> { 12 };

            foreach (var row in lstCrdBud)
            {
                curType = row.RefRegion.RefTerr.ID;
                curId = GetRegionParentId(row.RefRegion.ID, row.RefRegion.ParentID, curType);
                var region = regRepository.FindOne(Convert.ToInt32(curId));
                curRegion = region;
                var settleName = curType == CodeSP || curType == CodeGP
                                     ? String.Format(", {0}", row.RefRegion.Name)
                                     : String.Empty;

                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data),
                                     row.RefRegion.RefTerr.ID,
                                     curId,
                                     String.Empty,
                                     String.Empty,
                                     String.Format("{0}{1}", row.Num, settleName),
                                     row.Creditor,
                                     row.Occasion,
                                     row.Collateral,
                                     ReportsDataService.GetDateValue(row.ContractDate),
                                     ReportsDataService.GetDateValue(row.PaymentDate),
                                     row.RefOKV.Name,
                                     row.CreditPercentNum,
                                     row.Attract,
                                     row.Discharge,
                                     row.CapitalDebt,
                                     row.StaleDebt,
                                     row.FactDate,
                                     row.PlanService,
                                     row.FactService,
                                     row.ServiceDebt,
                                     row.ChargePenlt,
                                     row.FactPenlt,
                                     row.RemnsEndMnthPenlt,
                                     row.Note
                                 };

                var hasSummary = AddSummaries(curId, curType, oldId, oldType);

                if (hasSummary || !hasCaption)
                {
                    AddTitle(region);
                    hasCaption = true;
                }

                UpdateSummaries(values, curType, curId, oldId);
                currentTable.Rows.Add(values);
                oldId = curId;
                oldType = curType;
                oldRegion = curRegion;
            }

            AddSummaries(-1, -1, oldId, oldType);
            AddSummaryRow(currentTable, RecordType.SummaryTotal);

            oldType = -1;
            oldId = -1;

            currentSummary = limSplitSummarySplit;
            currentTable = tables[LimRegionIndex];

            var lstLim = SortContractList(queryLim.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBLimit>();
            hasCaption = false;
            lstExcludeSummaryColumn = new List<int>();

            foreach (var row in lstLim)
            {
                curType = row.RefRegion.RefTerr.ID;
                curId = GetRegionParentId(row.RefRegion.ID, row.RefRegion.ParentID, curType);
                var region = regRepository.FindOne(Convert.ToInt32(curId));
                curRegion = region;

                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data),
                                     row.RefRegion.RefTerr.ID,
                                     curId,
                                     String.Empty,
                                     String.Empty,
                                     row.RefRegion.Name,
                                     row.ConsMunDebt1,
                                     row.OwnBudgRevns,
                                     row.ConsMunDebt2,
                                     row.ConsMunGrnt1,
                                     row.ConsMunService1,
                                     row.MunIssue1,
                                     row.NormAct
                                 };

                var hasSummary = AddSummaries(curId, curType, oldId, oldType);

                if (hasSummary || !hasCaption)
                {
                    AddTitle(region);
                    hasCaption = true;
                }

                UpdateSummaries(values, curType, curId, oldId);
                currentTable.Rows.Add(values);
                oldId = curId;
                oldType = curType;
                oldRegion = curRegion;
            }

            AddSummaries(-1, -1, oldId, oldType);
            AddSummaryRow(currentTable, RecordType.SummaryTotal);

            if (curRegion != null)
            {
                var sumaryColumnIndex = ServiceColumnCount + 6;
                var summaryIndex = Convert.ToInt32(RecordType.Data);
                var lstIdxFields = new List<int> { 18, 15, 15, 13 };
                var lstTerrTypes = new List<int> { CodeSB, CodeGO, CodeMR };
                currentTable = tables[SumRegionIndex];
                var lstRegions = regRepository.FindAll()
                    .Where(f => f.Code > 0 && f.SourceID == curRegion.SourceID && lstTerrTypes.Contains(f.RefTerr.ID))
                    .OrderBy(f => f.Code);

                foreach (var regionInfo in lstRegions)
                {
                    var rowRegion = currentTable.Rows.Add();
                    rowRegion[0] = Convert.ToInt32(RecordType.Data);
                    rowRegion[1] = ConvertTerrType(regionInfo.RefTerr.ID);
                    rowRegion[2] = regionInfo.ID;

                    rowRegion[ServiceColumnCount + 0] = regionInfo.Code;
                    rowRegion[ServiceColumnCount + 1] = regionInfo.Name;

                    for (var i = 0; i < 4; i++)
                    {
                        var sum = 0.0;

                        var strSelect = String.Empty;

                        if (regionInfo.RefTerr.ID != CodeSB)
                        {
                            strSelect = String.Format(
                                "{0} = '{2}' and {1} = '{3}'",
                                tables[i].Columns[0].ColumnName,
                                tables[i].Columns[2].ColumnName,
                                summaryIndex,
                                regionInfo.ID);
                        }
                        else
                        {
                            strSelect = String.Format(
                                "{0} = '{2}' and {1} = '{3}'",
                                tables[i].Columns[0].ColumnName,
                                tables[i].Columns[1].ColumnName,
                                summaryIndex,
                                regionInfo.RefTerr.ID);
                        }

                        var rowsData = tables[i].Select(strSelect);

                        if (rowsData.Length > 0)
                        {
                            sum = rowsData.Sum(f => ReportsDataService.GetDoubleValue(f[lstIdxFields[i]]));
                            rowRegion[ServiceColumnCount + i + 2] = sum;
                        }

                        rowRegion[sumaryColumnIndex] = ReportsDataService.GetDoubleValue(rowRegion[sumaryColumnIndex]) + sum; 
                    }
                }
            } 

            var rowCaption = tables[tables.Length - 1].Rows.Add();
            rowCaption[0] = currentRegion.RefTerr.ID != 3;
            rowCaption[1] = currentRegion.RefTerr.ID != 7;
            rowCaption[2] = currentRegion.RefTerr.FullName;

            rowCaption[3] = GetReportCaption(0);
            rowCaption[4] = GetReportCaption(1);

            rowCaption[6] = ReportsDataService.GetDateValue(currentVariant.ReportDate);
            rowCaption[7] = currentVariant.Name;

            rowCaption[8] = GetInfoText();
            rowCaption[9] = GetCreditAppendixText();

            return tables;
        }

        private int ConvertTerrType(int terrType)
        {
            switch (terrType)
            {
                case CodeMR:
                    return 0;
                case CodeGO:
                    return 1;
                case CodeSB:
                    return 2;
            }

            return 0;
        }

        private void AddTitle(D_Regions_Analysis region)
        {
            var rowTitle = currentTable.Rows.Add();
            rowTitle[ColRowType] = Convert.ToInt32(RecordType.Caption);
            var caption = region.Name;

            switch (region.RefTerr.ID)
            {
                case CodeGO:
                case CodeMR:
                    caption = String.Format("{0} {1}", region.Name, region.RefTerr.ID == CodeMR ? "МР" : String.Empty);
                    break;
            }

            rowTitle[ServiceColumnCount] = caption;
        }

        private string GetInfoText()
        {
            switch (userRegion.RefTerr.ID)
            {
                case CodeSB:
                    return "Параметры, утвержденные решением о бюджете";
                case CodeMR:
                case CodeGO:
                    return "Параметры, утвержденные законом о бюджете";
            }

            return String.Empty;
        }

        private string GetCreditAppendixText()
        {
            return userRegion.RefTerr.ID == CodeSB ? 
                ", иностранных банков и международных финансовых организаций" : 
                String.Empty;
        }

        private string GetReportCaption(int variant)
        {
            if (variant == 0)
            {
                switch (userRegion.RefTerr.ID)
                {
                    case CodeSB:
                        return "Ставропольского края";
                    case CodeMR:
                        return String.Format("МО ({0} район)", userRegion.Name);
                    case CodeGO:
                        return String.Format("муниципального образования {0}", userRegion.Name);
                }
            }
            else
            {
                switch (userRegion.RefTerr.ID)
                {
                    case CodeSB:
                        return "Ставропольским краем";
                    case CodeMR:
                        return String.Format("МО ({0} район)", userRegion.Name);
                    case CodeGO:
                        return String.Format("муниципальным образованием {0}", userRegion.Name);
                }                
            }

            return String.Empty;
        }

        private List<DebtorBookFactBase> SortContractList(List<DebtorBookFactBase> query)
        {
            var lstCap = new List<DebtorBookFactBase>();

            if (query.Count() > 0)
            {
                var srcId = query.FirstOrDefault().RefRegion.SourceID;
                var lstMR = regRepository.FindAll()
                    .Where(f => f.RefTerr.ID == CodeMR && f.SourceID == srcId)
                    .OrderBy(f => f.Code);

                foreach (var mr in lstMR)
                {
                    var lstMrData = query
                        .Where(f => f.RefRegion.ID == mr.ID)
                        .OrderBy(f => f.RefRegion.Code);
                    lstCap.AddRange(SortByDate(lstMrData));

                    var lstSettles = GetSettleList(mr.ID);

                    foreach (var settle in lstSettles)
                    {
                        var lstSettleData = query
                            .Where(f => f.RefRegion.ID == settle.ID)
                            .OrderBy(f => f.RefRegion.Code);
                        lstCap.AddRange(SortByDate(lstSettleData));
                    }
                }

                lstCap.AddRange(SortByDate(query.Where(f => f.RefRegion.RefTerr.ID == CodeGO).OrderBy(f => f.RefRegion.Code)));
                lstCap.AddRange(SortByDate(query.Where(f => f.RefRegion.RefTerr.ID == CodeSB).OrderBy(f => f.RefRegion.Code)));
            }

            return lstCap;
        }

        private List<DebtorBookFactBase> SortByDate(IOrderedEnumerable<DebtorBookFactBase> data)
        {
            var lst = 
                from d in data
                let dataValue = GetContractDateValue(d)
                orderby dataValue 
                select d;
            return lst.ToList();
        }

        private DateTime? GetContractDateValue(DebtorBookFactBase element)
        {
            if (element is F_S_SchBCapital)
            {
                var rec = (F_S_SchBCapital)element;
                return rec.RegEmissionDate;
            }

            if (element is F_S_SchBCreditincome)
            {
                var rec = (F_S_SchBCreditincome)element;
                return rec.ContractDate;
            }

            if (element is F_S_SchBGuarantissued)
            {
                var rec = (F_S_SchBGuarantissued)element;
                return rec.StartDate;
            }

            return DateTime.Now;
        }

        private int GetRegionParentId(int settleId, int? parentId, int terrType)
        {
            var result = settleId;
            var lstSettles = new List<int> { CodeGP, CodeSP };

            if (lstSettles.Contains(terrType))
            {
                var groupRecord = regRepository.FindOne(Convert.ToInt32(parentId));
                result = regRepository.FindOne(Convert.ToInt32(groupRecord.ParentID)).ID;
            }

            return result;
        }

        private IEnumerable<D_Regions_Analysis> GetSettleList(int id)
        {
            var recFictive = regRepository.FindAll().Where(f => f.ParentID == id).FirstOrDefault();
            return regRepository.FindAll().Where(f => f.ParentID == recFictive.ID).ToList();
        }

        private void UpdateSumRow(int rowIndex, IList<object> values)
        {
            for (var j = ServiceColumnCount; j < values.Count; j++)
            {
                if (!lstExcludeSummaryColumn.Contains(j) && values[j] is decimal)
                {
                    currentSummary[rowIndex, j] += Convert.ToDecimal(values[j]);
                }
            }
        }

        private void UpdateSummaries(IList<object> values, int curType, int curId, int oldId)
        {
            if (curId != oldId)
            {
                var rowIndex = Convert.ToInt32(RecordType.SummaryMrWoSettles);
                for (var i = ServiceColumnCount; i < ResultColumnCount; i++)
                {
                    currentSummary[rowIndex, i] = 0;
                }
            }

            UpdateSumRow(Convert.ToInt32(RecordType.SummaryTotal), values);

            switch (curType)
            {
                case CodeSB:
                    UpdateSumRow(Convert.ToInt32(RecordType.SummarySB), values);
                    break;
                case CodeMR:
                    UpdateSumRow(Convert.ToInt32(RecordType.SummaryMrWoSettles), values);
                    UpdateSumRow(Convert.ToInt32(RecordType.SummaryMR), values);
                    UpdateSumRow(Convert.ToInt32(RecordType.SummaryAllMR), values);
                    break;
                case CodeGO:
                    UpdateSumRow(Convert.ToInt32(RecordType.SummaryGO), values);
                    UpdateSumRow(Convert.ToInt32(RecordType.SummaryAllGO), values);
                    break;
                case CodeSP:
                case CodeGP:
                    UpdateSumRow(Convert.ToInt32(RecordType.SummarySettle), values);
                    UpdateSumRow(Convert.ToInt32(RecordType.SummaryMR), values);
                    UpdateSumRow(Convert.ToInt32(RecordType.SummaryAllMR), values);
                    break;
            }
        }

        private bool AddSummaries(int curId, int curType, int oldId, int oldType)
        {
            if (oldType < 0 || curId == oldId)
            {
                if (oldType == CodeMR && (curType == CodeGP || curType == CodeSP))
                {
                    AddSummaryRow(currentTable, RecordType.SummaryMrWoSettles);  
                }

                return false;
            }

            switch (oldType)
            {
                case CodeGP:
                case CodeSP:
                    AddSummaryRow(currentTable, RecordType.SummarySettle);
                    AddSummaryRow(currentTable, RecordType.SummaryMR);
                    return true;
                case CodeMR:
                    AddSummaryRow(currentTable, RecordType.SummaryMR);
                    return true;
                case CodeGO:
                    AddSummaryRow(currentTable, RecordType.SummaryGO);
                    return true;
                case CodeSB:
                    AddSummaryRow(currentTable, RecordType.SummarySB);
                    return true;
            }

            return false;
        }

        private void AddSummaryRow(DataTable tblData, RecordType recType, bool clearSummary = true)
        {
            var rowIndex = Convert.ToInt32(recType);
            var rowSummary = tblData.Rows.Add();
            rowSummary[ColRowType] = rowIndex;

            for (var i = ServiceColumnCount; i < ResultColumnCount; i++)
            {
                if (currentSummary[rowIndex, i] != 0)
                {
                    rowSummary[i] = currentSummary[rowIndex, i];
                }
            }

            if (clearSummary)
            {
                for (var i = 0; i < ResultColumnCount; i++)
                {
                    currentSummary[rowIndex, i] = 0;
                }
            }

            rowSummary[0] = rowIndex;
            
            if (recType != RecordType.SummaryTotal &&
                recType != RecordType.SummaryAllMR &&
                recType != RecordType.SummaryAllGO)
            {
                rowSummary[2] = curRegion.ID;
            }

            var resultText = String.Empty;
            var terrText = String.Empty;

            switch (recType)
            {
                case RecordType.SummaryGO:
                    terrText = "ГО";
                    break;
                case RecordType.SummaryMR:
                    terrText = "район";
                    break;
            }

            switch (recType)
            {
                case RecordType.SummaryAllGO:
                    resultText = "по всем ГО";
                    break;
                case RecordType.SummaryAllMR:
                    resultText = "по всем МР";
                    break;
                case RecordType.SummaryMrWoSettles:
                    resultText = String.Format("по {0} МР (без пос.)", oldRegion.Name);
                    break;
                case RecordType.SummaryGO:
                case RecordType.SummaryMR:
                case RecordType.SummarySB:
                    resultText = String.Format("по {0} {1}", oldRegion.Name, terrText);
                    break;
                case RecordType.SummarySettle:
                    resultText = String.Format("по пос. {0} район", oldRegion.Name);
                    break;
            }

            rowSummary[ServiceColumnCount] = String.Format("Итого {0}", resultText).Trim();
        }

        private List<F_S_SchBCreditincome> GetCredits(int creditType)
        {
            return crdRepository.FindAll().Where(x =>
                x.RefTypeCredit.ID == creditType &&
                x.RefVariant.ID == variantId &&
                (!chkRegion || ((x.RefRegion.ID == regionId) || settleList.Contains(x.RefRegion.ID)))).ToList();
        }

        private List<F_S_SchBGuarantissued> GetGarants()
        {
            return grnRepositopry.FindAll().Where(x =>
                x.RefVariant.ID == variantId &&
                (!chkRegion || ((x.RefRegion.ID == regionId) || settleList.Contains(x.RefRegion.ID)))).ToList();
        }

        private List<F_S_SchBLimit> GetLimits()
        {
            return limitRepository.FindAll().Where(x =>
                x.RefVariant.ID == variantId &&
                (!chkRegion || ((x.RefRegion.ID == regionId) || settleList.Contains(x.RefRegion.ID)))).ToList();
        }

        private List<F_S_SchBCapital> GetCapitals()
        {
            return capRepository.FindAll().Where(x =>
                x.RefVariant.ID == variantId &&
                (!chkRegion || ((x.RefRegion.ID == regionId) || settleList.Contains(x.RefRegion.ID)))).ToList();
        }
    }
}
