using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// Получение данных - Калмыкия
    /// </summary>
    public class DebtBookExportServiceKalmykia : IDebtBookExportServiceKalmykia
    {
        private const int ResultColumnCount = 40;
        private const int CodeMR = 4;
        private const int CodeGO = 7;
        private const int CodeSB = 3;
        private readonly ILinqRepository<D_Regions_Analysis> regRepository;
        private readonly ILinqRepository<F_S_SchBCreditincome> crdRepository;
        private readonly ILinqRepository<F_S_SchBCapital> capRepository;
        private readonly ILinqRepository<F_S_SchBGuarantissued> grnRepositopry;
        private readonly ILinqRepository<D_Variant_Schuldbuch> variantRepository;
        private DataTable currentTable;
        private bool chkRegion;
        private int regionId;
        private int variantId;
        private List<int> settleList;

        public DebtBookExportServiceKalmykia(
            ILinqRepository<D_Regions_Analysis> regRepository,
            ILinqRepository<D_Variant_Schuldbuch> variantRepository,
            ILinqRepository<F_S_SchBCreditincome> crdRepository,
            ILinqRepository<F_S_SchBCapital> capRepository,
            ILinqRepository<F_S_SchBGuarantissued> grnRepositopry)
        {
            this.variantRepository = variantRepository;
            this.regRepository = regRepository;
            this.crdRepository = crdRepository;
            this.capRepository = capRepository;
            this.grnRepositopry = grnRepositopry;
        }

        public DataTable[] GetDebtBookKalmykiaData(int refVariant, int refRegion, DateTime calculateDate)
        {
            const string TemplateDateNum = "{0:d}, {1}";
            const string TemplateNumDate = "{0}, {1:d}";
            const int CodeSubject = 3;
            const int CapRegionIndex = 1;
            const int CrdOrgRegionIndex = 0;
            const int CrdBudRegionIndex = 2;
            const int GrnRegionIndex = 3;
            var tables = new DataTable[5];
            var currentRegion = regRepository.FindOne(refRegion);
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

            currentTable = tables[CapRegionIndex];
            var lstCap = SortContractList(queryCap.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBCapital>();

            foreach (var row in lstCap)
            {
                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data), // 0
                                     row.RefRegion.RefTerr.ID, // 1
                                     row.RefRegion.ID, // 2
                                     String.Empty, // 3
                                     String.Empty, // 4
                                     row.RefRegion.Name, // 5
                                     String.Format(TemplateDateNum, row.DebtStartDate, row.RegNumber), // 6
                                     String.Format("{0}, {1}, {2}", row.RefKindCapital.Name, row.CodeCapital, row.CodeTransh), // 7
                                     String.Format(TemplateNumDate, row.OfficialNumber, row.RegEmissionDate), // 8
                                     row.Reason, // 9
                                     row.Collateral, // 10
                                     ReportsDataService.GetDateValue(row.GetBackDate), // 11
                                     ReportsDataService.GetDateValue(row.DateDischarge), // 12
                                     row.Nominal, // 13
                                     row.Count, // 14
                                     row.Coupon, // 15
                                     row.Sum, // 16
                                     row.IssueSum, // 17
                                     row.Income, // 18
                                     row.BgnYearDbt, // 19
                                     row.BgnYearInterest, // 20
                                     String.Empty, // 21
                                     row.Attract, // 22
                                     row.FactServiceSum, // 23
                                     String.Empty, // 24
                                     row.Discharge, // 25
                                     row.DischargePrice, // 26
                                     row.FactDiscountSum, // 27
                                     row.PlanService, // 28
                                     row.FactService, // 29
                                     row.ChargePenlt, // 30
                                     row.FactPenlt, // 31
                                     String.Empty, // 32
                                     row.RemnsEndMnthDbt, // 33
                                     row.RemnsEndMnthInterest, // 34
                                     row.RemnsEndMnthPenlt, // 35
                                     String.Empty, // 36
                                 };
                
                currentTable.Rows.Add(values);
            }

            currentTable = tables[CrdOrgRegionIndex];
            var lstCrdOrg = SortContractList(queryCrdOrg.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBCreditincome>();

            foreach (var row in lstCrdOrg)
            {
                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data), // 0
                                     row.RefRegion.RefTerr.ID, // 1
                                     row.RefRegion.ID, // 2
                                     String.Empty, // 3
                                     String.Empty, // 4
                                     String.Format(TemplateDateNum, row.DebtStartDate, row.RegNum), // 5
                                     row.Creditor, // 6
                                     row.RefRegion.Name, // 7
                                     String.Format("{0}, {1:d}, {2}", row.RefTypeContract.Name, row.ContractDate, row.Num), // 8
                                     row.Purpose, // 9
                                     row.Collateral, // 10
                                     row.StartDate, // 11
                                     row.FactDate, // 12
                                     row.Sum, // 13
                                     row.CreditPercent, // 14
                                     row.BgnYearDbt, // 15
                                     row.BgnYearInterest, // 16
                                     row.BgnYearPenlt, // 17
                                     String.Empty, // 18
                                     row.Attract, // 19
                                     0, // 20
                                     0, // 21
                                     String.Empty, // 22
                                     row.Discharge, // 23
                                     row.PlanService, // 24
                                     row.FactService, // 25
                                     row.ChargePenlt, // 26
                                     row.FactPenlt, // 27
                                     String.Empty, // 28
                                     row.RemnsEndMnthDbt, // 29
                                     row.RemnsEndMnthInterest, // 30
                                     row.RemnsEndMnthPenlt, // 31
                                     String.Empty, // 32
                                 };

                currentTable.Rows.Add(values);
            }

            currentTable = tables[GrnRegionIndex];
            var lstGrn = SortContractList(queryGrn.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBGuarantissued>();

            foreach (var row in lstGrn)
            {
                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data), // 0
                                     row.RefRegion.RefTerr.ID, // 1
                                     row.RefRegion.ID, // 2
                                     String.Empty, // 3
                                     String.Empty, // 4
                                     String.Format(TemplateDateNum, row.StartDate, row.RegNum), // 5
                                     row.Creditor, // 6
                                     row.Principal, // 7
                                     row.RefRegion.Name, // 8
                                     row.PrincipalNum, // 9
                                     row.Num, // 10
                                     row.Collateral, // 11
                                     ReportsDataService.GetDateValue(row.StartCreditDate), // 12
                                     ReportsDataService.GetDateValue(row.PrincipalEndDate), // 13
                                     row.Sum, // 14
                                     row.CreditPercent, // 15
                                     row.BgnYearDebt, // 16
                                     row.BgnInterest, // 17
                                     row.BgnPenalty, // 18
                                     String.Empty, // 19
                                     row.TotalDebt, // 20
                                     0, // 21
                                     0, // 22
                                     String.Empty, // 23
                                     String.Empty, // 24
                                     row.DownPrincipal, // 25
                                     row.DownGarant, // 26
                                     row.UpService, // 27
                                     row.DownService, // 28
                                     row.DownServiceGarant, // 29
                                     row.UpPenalty, // 30
                                     row.DownPenalty, // 31
                                     row.DownPenaltyGarant, // 32
                                     String.Empty, // 33
                                     row.RemnsEndMnthDbt, // 34
                                     row.EndInterest, // 35
                                     row.EndPenalty, // 36
                                     String.Empty, // 37
                                 };

                currentTable.Rows.Add(values);
            }

            currentTable = tables[CrdBudRegionIndex];
            var lstCrdBud = SortContractList(queryCrdBud.Cast<DebtorBookFactBase>().ToList()).Cast<F_S_SchBCreditincome>();

            foreach (var row in lstCrdBud)
            {
                var values = new object[]
                                 {
                                     Convert.ToInt32(RecordType.Data), // 0
                                     row.RefRegion.RefTerr.ID, // 1
                                     row.RefRegion.ID, // 2
                                     String.Empty, // 3
                                     String.Empty, // 4
                                     String.Format(TemplateDateNum, row.DebtStartDate, row.RegNum), // 5
                                     row.Creditor, // 6
                                     row.RefRegion.Name, // 7
                                     String.Format("{0}, {1:d}, {2}", row.RefTypeContract.Name, row.ContractDate, row.Num), // 8
                                     row.Purpose, // 9
                                     row.Collateral, // 10
                                     row.StartDate, // 11
                                     row.FactDate, // 12
                                     row.Sum, // 13
                                     row.CreditPercent, // 14
                                     row.BgnYearDbt, // 15
                                     row.BgnYearInterest, // 16
                                     row.BgnYearPenlt, // 17
                                     String.Empty, // 18
                                     row.Attract, // 19
                                     0, // 20
                                     0, // 21
                                     String.Empty, // 22
                                     row.Discharge, // 23
                                     row.PlanService, // 24
                                     row.FactService, // 25
                                     row.StaleInterest, // 26
                                     row.ChargePenlt, // 27
                                     row.FactPenlt, // 28
                                     row.StalePenlt, // 29
                                     String.Empty, // 30
                                     row.RemnsEndMnthDbt, // 31
                                     row.RemnsEndMnthInterest, // 32
                                     row.RemnsEndMnthPenlt, // 33
                                     String.Empty // 34
                                 };

                currentTable.Rows.Add(values);
            }

            var rowCaption = tables[tables.Length - 1].Rows.Add();
            rowCaption[0] = currentRegion.RefTerr.ID != 3;
            rowCaption[1] = currentRegion.RefTerr.ID != 7;
            rowCaption[2] = currentRegion.RefTerr.FullName;
            rowCaption[3] = currentRegion.RefTerr.ID == 3 ? String.Empty : currentRegion.Name;
            rowCaption[6] = ReportsDataService.GetDateValue(currentVariant.ReportDate);
            rowCaption[7] = currentVariant.Name;
            return tables;
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
                return rec.DebtStartDate;
            }

            if (element is F_S_SchBCreditincome)
            {
                var rec = (F_S_SchBCreditincome)element;
                return rec.DebtStartDate;
            }

            if (element is F_S_SchBGuarantissued)
            {
                var rec = (F_S_SchBGuarantissued)element;
                return rec.StartDate;
            }

            return DateTime.Now;
        }

        private IEnumerable<D_Regions_Analysis> GetSettleList(int id)
        {
            var recFictive = regRepository.FindAll().Where(f => f.ParentID == id).FirstOrDefault();
            return regRepository.FindAll().Where(f => f.ParentID == recFictive.ID).ToList();
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

        private List<F_S_SchBCapital> GetCapitals()
        {
            return capRepository.FindAll().Where(x =>
                x.RefVariant.ID == variantId &&
                (!chkRegion || ((x.RefRegion.ID == regionId) || settleList.Contains(x.RefRegion.ID)))).ToList();
        }
    }
}
