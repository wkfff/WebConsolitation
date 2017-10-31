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
    public static class ExportAnnualBalanceF0503121Service
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
                    new annualBalanceF0503121
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new annualBalanceF0503121.bodyLocalType { position = Position(header) }
                    }.Save);    
            }

            if (year == 2013)
            {
                return ExportServiceHelper.Serialize(
                    new annualBalanceF0503121_2013
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503121_2013.bodyLocalType { position = Position2013(header) }
                        }.Save);
            }

            return ExportServiceHelper.Serialize(
                new annualBalanceF0503121_2014
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new annualBalanceF0503121_2014.bodyLocalType { position = Position2014(header) }
                    }.Save);
        }

        private static annualBalanceF0503121Type Position(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503121Type
                       {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                                ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                : null,
                formationPeriod = year,
                generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataType(header),
                income = new annualBalanceF0503121Type.incomeLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503121.Where(
                            x => x.Section == (int)F0503121Details.Incomes).ToList())
                },
                expense = new annualBalanceF0503121Type.expenseLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.Expenses).ToList())
                },
                netOperatingResults = new annualBalanceF0503121Type.netOperatingResultsLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503121.Where(
                            x => x.Section == (int)F0503121Details.OperatingResult).ToList())
                },
                nonFinancialAssetsTransactions = new annualBalanceF0503121Type.nonFinancialAssetsTransactionsLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.OperationNonfinancialAssets).ToList())
                },
                financialAssetsLiabilitiesTransactions = new annualBalanceF0503121Type.financialAssetsLiabilitiesTransactionsLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.OperationFinancialAssets).ToList())
                },
                document = ExportServiceHelper.Documents(header.Documents.ToList())
            };
        }

        private static annualBalanceF0503121Type_2013 Position2013(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503121Type_2013
            {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                                ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                : null,
                formationPeriod = year,
                generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataType(header),

                income = new annualBalanceF0503121Type_2013.incomeLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                            x => x.Section == (int)F0503121Details.Incomes).ToList())
                },
                expense = new annualBalanceF0503121Type_2013.expenseLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.Expenses).ToList())
                },
                netOperatingResults = new annualBalanceF0503121Type_2013.netOperatingResultsLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                            x => x.Section == (int)F0503121Details.OperatingResult).ToList())
                },
                nonFinancialAssetsTransactions = new annualBalanceF0503121Type_2013.nonFinancialAssetsTransactionsLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.OperationNonfinancialAssets).ToList())
                },
                financialAssetsLiabilitiesTransactions = new annualBalanceF0503121Type_2013.financialAssetsLiabilitiesTransactionsLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.OperationFinancialAssets).ToList())
                },
                document = ExportServiceHelper.Documents(header.Documents.ToList())
            };
        }

        private static annualBalanceF0503121Type_2014 Position2014(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503121Type_2014
            {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                                ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                : null,
                formationPeriod = year,
                generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataType2014(header),

                income = new annualBalanceF0503121Type_2014.incomeLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                            x => x.Section == (int)F0503121Details.Incomes).ToList())
                },
                expense = new annualBalanceF0503121Type_2014.expenseLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.Expenses).ToList())
                },
                netOperatingResults = new annualBalanceF0503121Type_2014.netOperatingResultsLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                            x => x.Section == (int)F0503121Details.OperatingResult).ToList())
                },
                nonFinancialAssetsTransactions = new annualBalanceF0503121Type_2014.nonFinancialAssetsTransactionsLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.OperationNonfinancialAssets).ToList())
                },
                financialAssetsLiabilitiesTransactions = new annualBalanceF0503121Type_2014.financialAssetsLiabilitiesTransactionsLocalType
                {
                    reportItem = ReportItems2013(header.AnnualBalanceF0503121.Where(
                        x => x.Section == (int)F0503121Details.OperationFinancialAssets).ToList())
                },
                document = ExportServiceHelper.Documents(header.Documents.ToList())
            };
        }

        private static reportItemF0503121TopLevelType ReportItem(F_Report_Bal0503121 docItem, IList<reportItemF0503121BaseType> subItems)
        {
            var result = new reportItemF0503121TopLevelType
                       {
                           name = docItem.Name,
                           lineCode = docItem.lineCode,
                           reportSubItem = subItems,
                           budgetActivity = docItem.budgetActivity,
                           incomeActivity = docItem.incomeActivity,
                           availableMeans = docItem.availableMeans,
                           total = docItem.total
                       };

            if (docItem.RefKosgy != null)
            {
                result.sectionNumber = docItem.RefKosgy.Code;
            }

            return result;
        }

        private static reportItemF0503121BaseType ReportSubItem(F_Report_Bal0503121 docItem)
        {
            var result = new reportItemF0503121BaseType
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                budgetActivity = docItem.budgetActivity,
                incomeActivity = docItem.incomeActivity,
                availableMeans = docItem.availableMeans,
                total = docItem.total
            };

            if (docItem.RefKosgy != null)
            {
                result.sectionNumber = docItem.RefKosgy.Code;
            }

            return result;
        }

        private static List<reportItemF0503121TopLevelType> ReportItems(List<F_Report_Bal0503121> data)
        {
            var codes = new[] { "291", "292" };

            var result = new List<reportItemF0503121TopLevelType>();

            foreach (var reportItemCode in data.Where(x => !codes.Contains(x.lineCode)).Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";
                var reportItem = data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));

                string code = reportItemCode;
                var subItems = data.Where(x => !codes.Contains(x.lineCode) 
                                               && x.lineCode.Substring(0, 2).Equals(code)
                                               && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItem).ToList();

                if (reportItem != null)
                {
                    result.Add(ReportItem(reportItem, subItems));
                }
            }

            foreach (var reportItem in data.Where(x => codes.Contains(x.lineCode)))
            {
                result.Add(ReportItem(reportItem, null));
            }

            return result;
        }

        private static reportItemF0503121TopLevelType_2013 ReportItem2013(F_Report_Bal0503121 docItem, IList<reportItemF0503121BaseType_2013> subItems)
        {
            var result = new reportItemF0503121TopLevelType_2013
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                reportSubItem = subItems,
                budgetActivity = docItem.budgetActivity,
                availableMeans = docItem.availableMeans,
                total = docItem.total
            };

            if (docItem.RefKosgy != null)
            {
                result.sectionNumber = docItem.RefKosgy.Code;
            }

            return result;
        }

        private static reportItemF0503121BaseType_2013 ReportSubItem2013(F_Report_Bal0503121 docItem)
        {
            var result = new reportItemF0503121BaseType_2013
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                budgetActivity = docItem.budgetActivity,
                availableMeans = docItem.availableMeans,
                total = docItem.total
            };

            if (docItem.RefKosgy != null)
            {
                result.sectionNumber = docItem.RefKosgy.Code;
            }

            return result;
        }

        private static List<reportItemF0503121TopLevelType_2013> ReportItems2013(List<F_Report_Bal0503121> data)
        {
            var codes = new[] { "291", "292" };

            var result = new List<reportItemF0503121TopLevelType_2013>();

            foreach (var reportItemCode in data.Where(x => !codes.Contains(x.lineCode)).Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";
                var reportItem = data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));

                string code = reportItemCode;
                var subItems = data.Where(x => !codes.Contains(x.lineCode)
                                               && x.lineCode.Substring(0, 2).Equals(code)
                                               && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItem2013).ToList();

                if (reportItem != null)
                {
                    result.Add(ReportItem2013(reportItem, subItems));
                }
            }

            foreach (var reportItem in data.Where(x => codes.Contains(x.lineCode)))
            {
                result.Add(ReportItem2013(reportItem, null));
            }

            return result;
        }
    }
}
