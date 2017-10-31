using System;
using System.Collections.Generic;
using System.Linq;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportAnnualBalanceF0503130Service
    {
        private static D_Org_UserProfile placerProfile;

        private static int year;

        private static D_Org_Structure target;

        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            target = header.RefUchr;
            placerProfile = authService.Profile;
            year = header.RefYearForm.ID;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            if (year < 2013)
            {
                return ExportServiceHelper.Serialize(
                        new annualBalanceF0503130
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503130.bodyLocalType { position = Position(header) }
                        }.Save);
            }

            if (year == 2013)
            {
                return ExportServiceHelper.Serialize(
                    new annualBalanceF0503130_2013
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503130_2013.bodyLocalType { position = Position2013(header) }
                        }.Save);
            }

            if (year == 2014)
            {
                return ExportServiceHelper.Serialize(
                    new annualBalanceF0503130_2014
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503130_2014.bodyLocalType { position = Position2014(header) }
                        }.Save);
            }

            return ExportServiceHelper.Serialize(
                new annualBalanceF0503130_2015
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new annualBalanceF0503130_2015.bodyLocalType { position = Position2015(header) }
                    }.Save);
        }

        private static annualBalanceF0503130Type Position(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503130Type
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null,
                        formationPeriod = year,
                        generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataType(header),
                        nonFinancialAssets = new annualBalanceF0503130Type.nonFinancialAssetsLocalType
                                                 {
                                                     reportItem = ReportItems(header.AnnualBalanceF0503130.Where(
                                                         x => x.Section == (int)F0503130F0503730Details.NonfinancialAssets).ToList())
                                                 },
                        financialAssets = new annualBalanceF0503130Type.financialAssetsLocalType
                            {
                                                  reportItem = ReportItems(header.AnnualBalanceF0503130.Where(
                                                      x => x.Section == (int)F0503130F0503730Details.FinancialAssets).ToList())
                            },
                        commitments = new annualBalanceF0503130Type.commitmentsLocalType
                            {
                                              reportItem = ReportItems(header.AnnualBalanceF0503130.Where(
                                                  x => x.Section == (int)F0503130F0503730Details.Liabilities).ToList())
                            },
                        financialResult = new annualBalanceF0503130Type.financialResultLocalType
                            {
                                                  reportItem = ReportItems(header.AnnualBalanceF0503130.Where(
                                                      x => x.Section == (int)F0503130F0503730Details.FinancialResult).ToList())
                            },
                        reference = new annualBalanceF0503130Type.referenceLocalType
                            {
                                            reportItem = ReportItems(header.AnnualBalanceF0503130.Where(
                                                x => x.Section == (int)F0503130F0503730Details.Information).ToList())
                            },
                        document = ExportServiceHelper.Documents(header.Documents.ToList())
                    };
        }

        private static annualBalanceF0503130Type_2013 Position2013(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503130Type_2013
                       {
                           positionId = Guid.NewGuid().ToString(),
                           changeDate = DateTime.Now,
                           placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                           initiator = target.ID != placerProfile.RefUchr.ID
                               ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                               : null,
                           formationPeriod = year,
                           generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataType(header),
                           nonFinancialAssets = new annualBalanceF0503130Type_2013.nonFinancialAssetsLocalType
                                                    {
                                                        reportItem = ReportItems2013(
                                                            header.AnnualBalanceF0503130.Where(
                                                                x => x.Section == (int)F0503130F0503730Details.NonfinancialAssets).ToList())
                                                    },
                           financialAssets = new annualBalanceF0503130Type_2013.financialAssetsLocalType
                                                 {
                                                     reportItem = ReportItems2013(
                                                         header.AnnualBalanceF0503130.Where(
                                                             x => x.Section == (int)F0503130F0503730Details.FinancialAssets).ToList())
                                                 },
                           commitments = new annualBalanceF0503130Type_2013.commitmentsLocalType
                                             {
                                                 reportItem = ReportItems2013(
                                                     header.AnnualBalanceF0503130.Where(
                                                         x => x.Section == (int)F0503130F0503730Details.Liabilities).ToList())
                                             },
                           financialResult = new annualBalanceF0503130Type_2013.financialResultLocalType
                                                 {
                                                     reportItem = ReportItems2013(
                                                         header.AnnualBalanceF0503130.Where(
                                                             x => x.Section == (int)F0503130F0503730Details.FinancialResult).ToList())
                                                 },
                           reference = new annualBalanceF0503130Type_2013.referenceLocalType
                                           {
                                               reportItemRefer = ReportItemsRefer2013(
                                                   header.AnnualBalanceF0503130.Where(
                                                       x => x.Section == (int)F0503130F0503730Details.Information).ToList())
                                           },
                           document = ExportServiceHelper.Documents(header.Documents.ToList())
                       };
        }

        private static annualBalanceF0503130Type_2014 Position2014(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503130Type_2014
                       {
                           positionId = Guid.NewGuid().ToString(),
                           changeDate = DateTime.Now,
                           placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                           initiator = target.ID != placerProfile.RefUchr.ID
                               ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                               : null,
                           formationPeriod = year,
                           generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataTypeCommon2014(header),
                           nonFinancialAssets = new annualBalanceF0503130Type_2014.nonFinancialAssetsLocalType
                                                    {
                                                        reportItem = ReportItems2013(
                                                            header.AnnualBalanceF0503130.Where(
                                                                x => x.Section == (int)F0503130F0503730Details.NonfinancialAssets).ToList())
                                                    },
                           financialAssets = new annualBalanceF0503130Type_2014.financialAssetsLocalType
                                                 {
                                                     reportItem = ReportItems2013(
                                                         header.AnnualBalanceF0503130.Where(
                                                             x => x.Section == (int)F0503130F0503730Details.FinancialAssets).ToList())
                                                 },
                           commitments = new annualBalanceF0503130Type_2014.commitmentsLocalType
                                             {
                                                 reportItem = ReportItems2013(
                                                     header.AnnualBalanceF0503130.Where(
                                                         x => x.Section == (int)F0503130F0503730Details.Liabilities).ToList())
                                             },
                           financialResult = new annualBalanceF0503130Type_2014.financialResultLocalType
                                                 {
                                                     reportItem = ReportItems2013(
                                                         header.AnnualBalanceF0503130.Where(
                                                             x => x.Section == (int)F0503130F0503730Details.FinancialResult).ToList())
                                                 },
                           reference = new annualBalanceF0503130Type_2014.referenceLocalType
                                           {
                                               reportItemRefer = ReportItemsRefer2013(
                                                   header.AnnualBalanceF0503130.Where(
                                                       x => x.Section == (int)F0503130F0503730Details.Information).ToList())
                                           },
                           document = ExportServiceHelper.Documents(header.Documents.ToList())
                       };
        }

        private static annualBalanceF0503130Type_2015 Position2015(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503130Type_2015
                       {
                           positionId = Guid.NewGuid().ToString(),
                           changeDate = DateTime.Now,
                           placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                           initiator = target.ID != placerProfile.RefUchr.ID
                               ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                               : null,
                           formationPeriod = year,
                           generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataType2014(header),
                           nonFinancialAssets = new annualBalanceF0503130Type_2015.nonFinancialAssetsLocalType
                                                    {
                                                        reportItem = ReportItems2013(
                                                            header.AnnualBalanceF0503130.Where(
                                                                x => x.Section == (int)F0503130F0503730Details.NonfinancialAssets).ToList())
                                                    },
                           financialAssets = new annualBalanceF0503130Type_2015.financialAssetsLocalType
                                                 {
                                                     reportItem = ReportItems2013(
                                                         header.AnnualBalanceF0503130.Where(
                                                             x => x.Section == (int)F0503130F0503730Details.FinancialAssets).ToList())
                                                 },
                           commitments = new annualBalanceF0503130Type_2015.commitmentsLocalType
                                             {
                                                 reportItem = ReportItems2013(
                                                     header.AnnualBalanceF0503130.Where(
                                                         x => x.Section == (int)F0503130F0503730Details.Liabilities).ToList())
                                             },
                           financialResult = new annualBalanceF0503130Type_2015.financialResultLocalType
                                                 {
                                                     reportItem = ReportItems2013(
                                                         header.AnnualBalanceF0503130.Where(
                                                             x => x.Section == (int)F0503130F0503730Details.FinancialResult).ToList())
                                                 },
                           reference = new annualBalanceF0503130Type_2015.referenceLocalType
                                           {
                                               reportItemRefer = ReportItemsRefer2013(
                                                   header.AnnualBalanceF0503130.Where(
                                                       x => x.Section == (int)F0503130F0503730Details.Information).ToList())
                                           },
                           document = ExportServiceHelper.Documents(header.Documents.ToList())
                       };
        }

        private static reportItemF0503130TopLevelType ReportItem(F_Report_BalF0503130 docItem, IList<reportItemF0503130BaseType> subItems)
        {
            var result = new reportItemF0503130TopLevelType
                             {
                                 name = docItem.Name,
                                 lineCode = docItem.lineCode,
                                 reportSubItem = subItems,
                                 budgetActivityBeginYear = docItem.budgetActivityBegin,
                                 budgetActivityEndYear = docItem.budgetActivityEnd,
                                 incomeActivityBeginYear = docItem.incomeActivityBegin,
                                 incomeActivityEndYear = docItem.incomeActivityEnd,
                                 availableMeansBeginYear = docItem.availableMeansBegin,
                                 availableMeansEndYear = docItem.availableMeansEnd,
                                 totalBeginYear = docItem.totalBegin,
                                 totalEndYear = docItem.totalEnd
                             };

            return result;
        }

        private static reportItemF0503130BaseType ReportSubItem(F_Report_BalF0503130 docItem)
        {
            var result = new reportItemF0503130TopLevelType
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                budgetActivityBeginYear = docItem.budgetActivityBegin,
                budgetActivityEndYear = docItem.budgetActivityEnd,
                incomeActivityBeginYear = docItem.incomeActivityBegin,
                incomeActivityEndYear = docItem.incomeActivityEnd,
                availableMeansBeginYear = docItem.availableMeansBegin,
                availableMeansEndYear = docItem.availableMeansEnd,
                totalBeginYear = docItem.totalBegin,
                totalEndYear = docItem.totalEnd
            };

            return result;
        }

        private static List<reportItemF0503130TopLevelType> ReportItems(List<F_Report_BalF0503130> data)
        {
            var result = new List<reportItemF0503130TopLevelType>();

            foreach (var reportItemCode in data.Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";
                var reportItem = data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));

                string code = reportItemCode;
                var subItems = data.Where(x => x.lineCode.Substring(0, 2).Equals(code) && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItem).ToList();

                if (reportItem != null)
                {
                    result.Add(ReportItem(reportItem, subItems));
                }
            }

            return result;
        }

        private static reportItemF0503130TopLevelType_2013 ReportItem2013(F_Report_BalF0503130 docItem, IList<reportItemF0503130BaseType_2013> subItems)
        {
            var result = new reportItemF0503130TopLevelType_2013
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                reportSubItem = subItems,
                budgetActivityBeginYear = docItem.budgetActivityBegin,
                budgetActivityEndYear = docItem.budgetActivityEnd,
                availableMeansBeginYear = docItem.availableMeansBegin,
                availableMeansEndYear = docItem.availableMeansEnd,
                totalBeginYear = docItem.totalBegin,
                totalEndYear = docItem.totalEnd
            };

            return result;
        }

        private static reportItemF0503130BaseType_2013 ReportSubItem2013(F_Report_BalF0503130 docItem)
        {
            var result = new reportItemF0503130TopLevelType_2013
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                budgetActivityBeginYear = docItem.budgetActivityBegin,
                budgetActivityEndYear = docItem.budgetActivityEnd,
                availableMeansBeginYear = docItem.availableMeansBegin,
                availableMeansEndYear = docItem.availableMeansEnd,
                totalBeginYear = docItem.totalBegin,
                totalEndYear = docItem.totalEnd
            };

            return result;
        }

        private static List<reportItemF0503130TopLevelType_2013> ReportItems2013(List<F_Report_BalF0503130> data)
        {
            var result = new List<reportItemF0503130TopLevelType_2013>();

            foreach (var reportItemCode in data.Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";
                var reportItem = data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));

                string code = reportItemCode;
                var subItems = data.Where(x => x.lineCode.Substring(0, 2).Equals(code) && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItem2013).ToList();

                if (reportItem != null)
                {
                    result.Add(ReportItem2013(reportItem, subItems));
                }
            }

            return result;
        }
        
        private static reportItemF0503130TopLevelReferenceType_2013 ReportItemRefer2013(F_Report_BalF0503130 docItem, IList<reportItemF0503130BaseReferenceType_2013> subItems)
        {
            var result = new reportItemF0503130TopLevelReferenceType_2013
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                reportSubItemRefer = subItems,
                totalBeginYear = docItem.totalBegin,
                totalEndYear = docItem.totalEnd
            };

            return result;
        }

        private static reportItemF0503130BaseReferenceType_2013 ReportSubItemRefer2013(F_Report_BalF0503130 docItem)
        {
            var result = new reportItemF0503130BaseReferenceType_2013
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                totalBeginYear = docItem.totalBegin,
                totalEndYear = docItem.totalEnd
            };

            return result;
        }

        private static List<reportItemF0503130TopLevelReferenceType_2013> ReportItemsRefer2013(List<F_Report_BalF0503130> data)
        {
            var result = new List<reportItemF0503130TopLevelReferenceType_2013>();

            foreach (var reportItemCode in data.Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";
                var reportItem = data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));

                string code = reportItemCode;
                var subItems = data.Where(x => x.lineCode.Substring(0, 2).Equals(code) && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItemRefer2013).ToList();

                if (reportItem != null)
                {
                    result.Add(ReportItemRefer2013(reportItem, subItems));
                }
            }

            return result;
        }
    }
}
