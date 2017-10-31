using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// Получение данных - Москва
    /// </summary>
    public class DebtBookExportServiceMoscow : IDebtBookExportServiceMoscow
    {
        private readonly ILinqRepository<D_Regions_Analysis> regRepository;
        private readonly ILinqRepository<F_S_SchBCreditincome> crdRepository;
        private readonly ILinqRepository<F_S_SchBCapital> capRepository;
        private readonly ILinqRepository<F_S_SchBGuarantissued> grnRepositopry;
        private readonly ILinqRepository<D_Variant_Schuldbuch> variantRepository;

        private bool rubSum;
        private bool chkRegion;
        private int regionId;
        private int variantId;
        private List<int> terrList;
        private List<int> settleList;

        public DebtBookExportServiceMoscow(
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

        public DataTable[] GetDebtBookMoscowData(int refVariant, int refRegion, DateTime calculateDate)
        {
            const int ResultColumnCount = 100;
            const string TemplateNumDate = "№{0} от {1:d}";
            const string TemplateGrnNumDate = "{0} от {1:d}";
            const int CodeSubject = 3;
            const int CodeRegion = 4;
            const int CodeCity = 7;
            const int CodeGP = 5;
            const int CodeSP = 6;
            const int CapRegionIndex = 0;
            const int CrdOrgRegionIndex = 4;
            const int GrnRegionIndex = 8;
            const int CrdBudRegionIndex = 12;
            const int CrdUnkRegionIndex = 16;
            var tables = new DataTable[21];
            var bounds = new int[tables.Length];
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

            for (var i = 0; i < 4; i++)
            {
                terrList = i == 0 || i == 2 ? 
                    new List<int> { CodeRegion, CodeCity } :
                    new List<int> { CodeGP, CodeSP };

                rubSum = i < 2;

                var queryCrdOrg = GetCredits(0);
                var queryCrdBud = GetCredits(1);
                var queryCrdUnk = GetCredits(5);
                var queryCap = GetCapitals();
                var queryGrn = GetGarants();

                var capSummary = new decimal[ResultColumnCount];

                foreach (var row in queryCap)
                {
                    var values = new object[]
                                     {
                                         row.OfficialNumber,
                                         row.RefSCap == null ? String.Empty : row.RefSCap.Name,
                                         row.FormCap,
                                         row.RegNumber,
                                         ReportsDataService.GetDateValue(row.RegEmissionDate),
                                         String.Format(
                                            "{0} {1} {2:d} {3}", 
                                            row.NameNPA, 
                                            row.NameOrg, 
                                            row.DateNPA,
                                            row.NumberNPA),
                                         row.RefOKV.Name,
                                         row.Sum,
                                         ReportsDataService.GetDateValue(row.StartDate),
                                         row.Owner,
                                         row.Nominal,
                                         ReportsDataService.GetDateValue(row.DateDischarge),
                                         ReportsDataService.GetDateValue(row.DatePartDischarge),
                                         row.IssueSum,
                                         row.Discharge,
                                         ReportsDataService.GetDateValue(row.DueDate),
                                         row.Coupon,
                                         row.Income,
                                         row.FactServiceSum,
                                         row.Discount,
                                         row.FactDiscountSum,
                                         row.TotalSum,
                                         row.GenAgent,
                                         row.Depository,
                                         row.Trade,
                                         row.StalePenlt,
                                         row.StaleDebt,
                                         row.StaleInterest,
                                         row.Attract,
                                         row.RefRegion.Name
                                     };

                    for (var j = 0; j < values.Length; j++)
                    {
                        if (values[j] is decimal)
                        {
                            capSummary[j] += Convert.ToDecimal(values[j]);
                        }
                    }

                    bounds[CapRegionIndex + i] = values.Length;
                    tables[CapRegionIndex + i].Rows.Add(values);
                }

                tables[CapRegionIndex + i] = AddSummaryRow(tables[CapRegionIndex + i], capSummary);

                var orgSummary = new decimal[ResultColumnCount];

                foreach (var row in queryCrdOrg)
                {
                    var values = new object[]
                                     {
                                         row.Occasion,
                                         String.Format(TemplateNumDate, row.Num, row.ContractDate),
                                         String.Format(TemplateNumDate, row.OldAgrNum, row.OldAgreementDate),
                                         String.Format(TemplateNumDate, row.RegNum, row.OffsetDate),
                                         String.Format(TemplateNumDate, row.RenwlNum, row.RenewalDate),
                                         row.RefOKV.Name,
                                         String.Format(TemplateNumDate, row.FurtherConvention, row.DebtStartDate),
                                         String.Format(TemplateNumDate, row.ComprAgrNum, row.ComprAgreeDate),
                                         row.Creditor,
                                         ReportsDataService.GetDateValue(row.ChargeDate),
                                         row.Sum,
                                         row.CreditPercent,
                                         ReportsDataService.GetDateValue(row.PaymentDate),
                                         row.StaleInterest,
                                         row.RemnsEndMnthDbt,
                                         row.StaleDebt,
                                         row.Attract,
                                         row.RefRegion.Name
                                     };

                    for (var j = 0; j < values.Length; j++)
                    {
                        if (values[j] is decimal)
                        {
                            orgSummary[j] += Convert.ToDecimal(values[j]);
                        }
                    }

                    bounds[CrdOrgRegionIndex + i] = values.Length;
                    tables[CrdOrgRegionIndex + i].Rows.Add(values);
                }

                tables[CrdOrgRegionIndex + i] = AddSummaryRow(tables[CrdOrgRegionIndex + i], orgSummary);

                var grnSummary = new decimal[ResultColumnCount];

                foreach (var row in queryGrn)
                {
                    var values = new object[]
                                     {
                                         String.Format(TemplateGrnNumDate, row.Num, row.StartDate),
                                         String.Format(TemplateNumDate, row.OldAgrNum, row.OldAgreementDate),
                                         String.Format(TemplateNumDate, row.RenewalNum, row.RenewalDate),
                                         String.Format(TemplateNumDate, row.PrincipalNum, row.PrincipalStartDate),
                                         row.RefOKV.Name,
                                         row.Garant,
                                         row.Principal,
                                         row.Creditor,
                                         ReportsDataService.GetDateValue(row.StartCreditDate),
                                         ReportsDataService.GetDateValue(row.ChargeDate),
                                         ReportsDataService.GetDateValue(row.EndCreditDate),
                                         ReportsDataService.GetDateValue(row.PrincipalEndDate),
                                         row.TotalDebt,
                                         row.CapitalDebt,
                                         row.DownDebt,
                                         row.DownGarant,
                                         row.StalePrincipalDebt,
                                         row.RemnsEndMnthDbt,
                                         row.BgnYearDebt,
                                         row.RefRegion.Name
                                     };

                    for (var j = 0; j < values.Length; j++)
                    {
                        if (values[j] is decimal)
                        {
                            grnSummary[j] += Convert.ToDecimal(values[j]);
                        }
                    }

                    bounds[GrnRegionIndex + i] = values.Length;
                    tables[GrnRegionIndex + i].Rows.Add(values);
                }

                tables[GrnRegionIndex + i] = AddSummaryRow(tables[GrnRegionIndex + i], grnSummary);

                var budSummary = new decimal[ResultColumnCount];

                foreach (var row in queryCrdBud)
                {
                    var values = new object[]
                                     {
                                         row.Occasion,
                                         String.Format(TemplateNumDate, row.Num, row.ContractDate),
                                         String.Format(TemplateNumDate, row.OldAgrNum, row.OldAgreementDate),
                                         String.Format(TemplateNumDate, row.RegNum, row.OffsetDate),
                                         String.Format(TemplateNumDate, row.RenwlNum, row.RenewalDate),
                                         row.RefOKV.Name,
                                         String.Format(TemplateNumDate, row.FurtherConvention, row.DebtStartDate),
                                         String.Format(TemplateNumDate, row.ComprAgrNum, row.ComprAgreeDate),
                                         row.Creditor,
                                         row.Sum,
                                         ReportsDataService.GetDateValue(row.ChargeDate),
                                         ReportsDataService.GetDateValue(row.PaymentDate),
                                         row.StaleDebt,
                                         row.Attract,
                                         row.RefRegion.Name
                                     };

                    for (var j = 0; j < values.Length; j++)
                    {
                        if (values[j] is decimal)
                        {
                            budSummary[j] += Convert.ToDecimal(values[j]);
                        }
                    }

                    bounds[CrdBudRegionIndex + i] = values.Length;
                    tables[CrdBudRegionIndex + i].Rows.Add(values);
                }

                tables[CrdBudRegionIndex + i] = AddSummaryRow(tables[CrdBudRegionIndex + i], budSummary);

                var unkSummary = new decimal[ResultColumnCount];

                foreach (var row in queryCrdUnk)
                {
                    var values = new object[]
                                     {
                                         row.Occasion,
                                         row.Purpose,
                                         String.Format(TemplateNumDate, row.Num, row.ContractDate),
                                         row.RefOKV.Name,
                                         String.Format(TemplateNumDate, row.OldAgrNum, row.OldAgreementDate),
                                         String.Format(TemplateNumDate, row.RenwlNum, row.RenewalDate),
                                         String.Format(TemplateNumDate, row.ComprAgrNum, row.ComprAgreeDate),
                                         row.Collateral,
                                         row.Creditor,
                                         row.Sum,
                                         ReportsDataService.GetDateValue(row.StartDate),
                                         ReportsDataService.GetDateValue(row.EndDate),
                                         row.StaleDebt,
                                         row.CapitalDebt,
                                         row.RefRegion.Name
                                     };

                    for (var j = 0; j < values.Length; j++)
                    {
                        if (values[j] is decimal)
                        {
                            unkSummary[j] += Convert.ToDecimal(values[j]);
                        }
                    }

                    bounds[CrdUnkRegionIndex + i] = values.Length;
                    tables[CrdUnkRegionIndex + i].Rows.Add(values);
                }

                tables[CrdUnkRegionIndex + i] = AddSummaryRow(tables[CrdUnkRegionIndex + i], unkSummary);
            }

            var rowCaption = tables[tables.Length - 1].Rows.Add();
            rowCaption[0] = currentRegion.RefTerr.ID != 7;
            rowCaption[1] = currentRegion.RefTerr.ID;
            rowCaption[2] = currentRegion.RefTerr.FullName;
            rowCaption[6] = currentVariant.ReportDate;
            rowCaption[7] = currentVariant.Name;
            return tables;
        }

        private DataTable AddSummaryRow(DataTable tblData, IList<decimal> summary)
        {
            var rowSummary = tblData.Rows.Add();

            for (var i = 0; i < summary.Count; i++)
            {
                if (summary[i] != 0)
                {
                    rowSummary[i] = summary[i];
                }
            }

            return tblData;
        }

        private IQueryable<F_S_SchBCreditincome> GetCredits(int creditType)
        {
            return crdRepository.FindAll().Where(x => 
                x.RefTypeCredit.ID == creditType && 
                x.RefVariant.ID == variantId &&
                terrList.Contains(x.RefRegion.RefTerr.ID) &&
                (!chkRegion || ((x.RefRegion.ID == regionId) || settleList.Contains(x.RefRegion.ID))) &&
                ((rubSum && x.RefOKV.ID == -1) || (!rubSum && x.RefOKV.ID != -1)));
        }

        private IQueryable<F_S_SchBGuarantissued> GetGarants()
        {
            return grnRepositopry.FindAll().Where(x =>
                x.RefVariant.ID == variantId &&
                terrList.Contains(x.RefRegion.RefTerr.ID) &&
                (!chkRegion || ((x.RefRegion.ID == regionId) || settleList.Contains(x.RefRegion.ID))) &&
                ((rubSum && x.RefOKV.ID == -1) || (!rubSum && x.RefOKV.ID != -1)));
        }

        private IQueryable<F_S_SchBCapital> GetCapitals()
        {
            return capRepository.FindAll().Where(x =>
                x.RefVariant.ID == variantId &&
                terrList.Contains(x.RefRegion.RefTerr.ID) &&
                (!chkRegion || ((x.RefRegion.ID == regionId) || settleList.Contains(x.RefRegion.ID))) &&
                ((rubSum && x.RefOKV.ID == -1) || (!rubSum && x.RefOKV.ID != -1)));
        }
    }
}
