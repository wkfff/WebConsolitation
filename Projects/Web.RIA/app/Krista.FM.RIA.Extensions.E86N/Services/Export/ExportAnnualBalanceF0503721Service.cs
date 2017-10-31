using System;
using System.Collections.Generic;
using System.Linq;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportAnnualBalanceF0503721Service
    {
        private static D_Org_Structure target;

        private static D_Org_UserProfile placerProfile;

        private static int year;

        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            target = header.RefUchr;
            placerProfile = authService.Profile;
            year = header.RefYearForm.ID;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            if (year < 2014)
            {
                return ExportServiceHelper.Serialize(
                    new annualBalanceF0503721
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503721.bodyLocalType { position = Position(header) }
                        }.Save);
            }

            if (year == 2014)
            {
                return ExportServiceHelper.Serialize(
                    new annualBalanceF0503721_2014
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503721_2014.bodyLocalType { position = Position2014(header) }
                        }.Save);
            }

            return ExportServiceHelper.Serialize(
                    new annualBalanceF0503721_2015
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new annualBalanceF0503721_2015.bodyLocalType { position = Position2015(header) }
                    }.Save);
        }

        private static annualBalanceF0503721Type Position(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503721Type
                {
                    positionId = Guid.NewGuid().ToString(),
                    changeDate = DateTime.Now,
                    placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                    initiator = target.ID != placerProfile.RefUchr.ID
                                    ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                    : null,
                    formationPeriod = year,
                    generalData = ExportServiceHelper.AnnualBalanceFounderDataType(header),
                    income = new annualBalanceF0503721Type.incomeLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503721.Where(
                            x => x.Section == (int)F0503721Details.Incomes).ToList())
                    },
                    expense = new annualBalanceF0503721Type.expenseLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503721.Where(
                            x => x.Section == (int)F0503721Details.Expenses).ToList())
                    },
                    nonFinancialAssets = new annualBalanceF0503721Type.nonFinancialAssetsLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503721.Where(
                            x => x.Section == (int)F0503721Details.NonFinancialAssets).ToList())
                    },
                    financialAssets = new annualBalanceF0503721Type.financialAssetsLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503721.Where(
                                        x => x.Section == (int)F0503721Details.FinancialAssetsLiabilities).ToList())
                    },
                    document = ExportServiceHelper.Documents(header.Documents.ToList())
                };
        }

        private static annualBalanceF0503721Type_2014 Position2014(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503721Type_2014
            {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                                ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                : null,
                formationPeriod = year,
                generalData = ExportServiceHelper.AnnualBalanceFounderDataType2014(header),
                income = new annualBalanceF0503721Type_2014.incomeLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503721.Where(
                        x => x.Section == (int)F0503721Details.Incomes).ToList())
                },
                expense = new annualBalanceF0503721Type_2014.expenseLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503721.Where(
                        x => x.Section == (int)F0503721Details.Expenses).ToList())
                },
                nonFinancialAssets = new annualBalanceF0503721Type_2014.nonFinancialAssetsLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503721.Where(
                        x => x.Section == (int)F0503721Details.NonFinancialAssets).ToList())
                },
                financialAssets = new annualBalanceF0503721Type_2014.financialAssetsLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503721.Where(
                                    x => x.Section == (int)F0503721Details.FinancialAssetsLiabilities).ToList())
                },
                document = ExportServiceHelper.Documents(header.Documents.ToList())
            };
        }

        private static annualBalanceF0503721Type_2015 Position2015(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503721Type_2015
            {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                                ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                : null,
                formationPeriod = year,
                generalData = ExportServiceHelper.AnnualBalanceFounderDataType2014(header),
                income = new annualBalanceF0503721Type_2015.incomeLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503721.Where(
                        x => x.Section == (int)F0503721Details.Incomes).ToList())
                },
                expense = new annualBalanceF0503721Type_2015.expenseLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503721.Where(
                        x => x.Section == (int)F0503721Details.Expenses).ToList())
                },
                nonFinancialAssets = new annualBalanceF0503721Type_2015.nonFinancialAssetsLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503721.Where(
                        x => x.Section == (int)F0503721Details.NonFinancialAssets).ToList())
                },
                financialAssets = new annualBalanceF0503721Type_2015.financialAssetsLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503721.Where(
                                    x => x.Section == (int)F0503721Details.FinancialAssetsLiabilities).ToList())
                },
                document = ExportServiceHelper.Documents(header.Documents.ToList())
            };
        }

        private static reportItemF0503721TopLevelType ReportItem(F_Report_BalF0503721 docItem, IList<reportItemF0503721BaseType> subItems)
        {
            var result = new reportItemF0503721TopLevelType
                             {
                                 name = docItem.Name,
                                 lineCode = docItem.lineCode,
                                 reportSubItem = subItems,
                                 targetFunds = docItem.targetFunds,
                                 services = docItem.services,
                                 temporaryFunds = docItem.temporaryFunds,
                                 total = docItem.total
                             };

            if (docItem.analyticCode.IsNotNullOrEmpty())
            {
                result.analyticCode = docItem.analyticCode;
            }

            return result;
        }

        private static reportItemF0503721BaseType ReportSubItem(F_Report_BalF0503721 docItem)
        {
            var result = new reportItemF0503721TopLevelType
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                targetFunds = docItem.targetFunds,
                services = docItem.services,
                temporaryFunds = docItem.temporaryFunds,
                total = docItem.total
            };

            if (docItem.analyticCode.IsNotNullOrEmpty())
            {
                result.analyticCode = docItem.analyticCode;
            }

            return result;
        }

        private static List<reportItemF0503721TopLevelType> ReportItems(List<F_Report_BalF0503721> data)
        {
            var codes = new[] { "301", "302" };

            var result = new List<reportItemF0503721TopLevelType>();

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

        private static reportItemF0503721TopLevelType2015 ReportItem2015(F_Report_BalF0503721 docItem, IList<reportItemF0503721BaseType2015> subItems)
        {
            var result = new reportItemF0503721TopLevelType2015
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                reportSubItem = subItems,
                targetFunds = docItem.targetFunds,
                stateTaskFunds = docItem.StateTaskFunds,
                revenueFunds = docItem.RevenueFunds,
                total = docItem.total
            };

            if (docItem.analyticCode.IsNotNullOrEmpty())
            {
                result.analyticCode = docItem.analyticCode;
            }

            return result;
        }

        private static reportItemF0503721BaseType2015 ReportSubItem2015(F_Report_BalF0503721 docItem)
        {
            var result = new reportItemF0503721TopLevelType2015
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                targetFunds = docItem.targetFunds,
                stateTaskFunds = docItem.StateTaskFunds,
                revenueFunds = docItem.RevenueFunds,
                total = docItem.total
            };

            if (docItem.analyticCode.IsNotNullOrEmpty())
            {
                result.analyticCode = docItem.analyticCode;
            }

            return result;
        }

        private static List<reportItemF0503721TopLevelType2015> ReportItems2015(List<F_Report_BalF0503721> data)
        {
            var codes = new[] { "301", "302" };

            var result = new List<reportItemF0503721TopLevelType2015>();

            foreach (var reportItemCode in data.Where(x => !codes.Contains(x.lineCode)).Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";
                var reportItem = data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));

                string code = reportItemCode;
                var subItems = data.Where(x => !codes.Contains(x.lineCode)
                                               && x.lineCode.Substring(0, 2).Equals(code)
                                               && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItem2015).ToList();

                if (reportItem != null)
                {
                    result.Add(ReportItem2015(reportItem, subItems));
                }
            }

            foreach (var reportItem in data.Where(x => codes.Contains(x.lineCode)))
            {
                result.Add(ReportItem2015(reportItem, null));
            }

            return result;
        }
    }
}
