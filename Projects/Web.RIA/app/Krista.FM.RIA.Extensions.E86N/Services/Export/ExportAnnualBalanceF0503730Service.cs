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
    public static class ExportAnnualBalanceF0503730Service
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

            if (year < 2014)
            {
                return ExportServiceHelper.Serialize(
                    new annualBalanceF0503730
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503730.bodyLocalType { position = Posistion(header) }
                        }.Save);
            }

            if (year == 2014)
            {
                return ExportServiceHelper.Serialize(
                    new annualBalanceF0503730_2014
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503730_2014.bodyLocalType { position = Posistion2014(header) }
                        }.Save);
            }

            return ExportServiceHelper.Serialize(
                    new annualBalanceF0503730_2015
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new annualBalanceF0503730_2015.bodyLocalType { position = Posistion2015(header) }
                    }.Save);
        }

        public static annualBalanceF0503730Type Posistion(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503730Type
                {
                    positionId = Guid.NewGuid().ToString(),
                    changeDate = DateTime.Now,
                    placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                    initiator = target.ID != placerProfile.RefUchr.ID
                                    ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                    : null,
                    formationPeriod = year,
                    generalData = ExportServiceHelper.AnnualBalanceFounderDataType(header),
                    nonFinancialAssets = new annualBalanceF0503730Type.nonFinancialAssetsLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                            x => x.Section == (int)F0503130F0503730Details.NonfinancialAssets).ToList())
                    },
                    financialAssets = new annualBalanceF0503730Type.financialAssetsLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                            x => x.Section == (int)F0503130F0503730Details.FinancialAssets).ToList())
                    },
                    commitments = new annualBalanceF0503730Type.commitmentsLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                            x => x.Section == (int)F0503130F0503730Details.Liabilities).ToList())
                    },
                    financialResult = new annualBalanceF0503730Type.financialResultLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                            x => x.Section == (int)F0503130F0503730Details.FinancialResult).ToList())
                    },
                    reference = new annualBalanceF0503730Type.referenceLocalType
                    {
                        reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                            x => x.Section == (int)F0503130F0503730Details.Information).ToList())
                    },
                    document = ExportServiceHelper.Documents(header.Documents.ToList())
                };
        }

        public static reportItemF0503730TopLevelType ReportItem(F_Report_BalF0503730 docItem, IList<reportItemF0503730BaseType> subItems)
        {
            var result = new reportItemF0503730TopLevelType
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                reportSubItem = subItems,
                targetFundsStartYear = docItem.targetFundsBegin,
                targetFundsEndYear = docItem.targetFundsEnd,
                servicesStartYear = docItem.servicesBegin,
                servicesEndYear = docItem.servicesEnd,
                temporaryFundsStartYear = docItem.temporaryFundsBegin,
                temporaryFundsEndYear = docItem.temporaryFundsEnd,
                totalStartYear = docItem.totalStartYear,
                totalEndYear = docItem.totalEndYear
            };

            return result;
        }

        public static reportItemF0503730TopLevelType_2015 ReportItem2015(F_Report_BalF0503730 docItem, IList<reportItemF0503730BaseType2015> subItems)
        {
            var result = new reportItemF0503730TopLevelType_2015
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                reportSubItem = subItems,
                targetFundsStartYear = docItem.targetFundsBegin,
                targetFundsEndYear = docItem.targetFundsEnd,
                stateTaskFundsStartYear = docItem.StateTaskFundStartYear,
                stateTaskFundsEndYear = docItem.StateTaskFundEndYear,
                revenueFundsStartYear = docItem.RevenueFundsStartYear,
                revenueFundsEndYear = docItem.RevenueFundsEndYear,
                totalStartYear = docItem.totalStartYear,
                totalEndYear = docItem.totalEndYear
            };

            return result;
        }

        private static annualBalanceF0503730Type_2014 Posistion2014(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503730Type_2014
            {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                                ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                : null,
                formationPeriod = year,
                generalData = ExportServiceHelper.AnnualBalanceFounderDataType2014(header),
                nonFinancialAssets = new annualBalanceF0503730Type_2014.nonFinancialAssetsLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.NonfinancialAssets).ToList())
                },
                financialAssets = new annualBalanceF0503730Type_2014.financialAssetsLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.FinancialAssets).ToList())
                },
                commitments = new annualBalanceF0503730Type_2014.commitmentsLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.Liabilities).ToList())
                },
                financialResult = new annualBalanceF0503730Type_2014.financialResultLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.FinancialResult).ToList())
                },
                reference = new annualBalanceF0503730Type_2014.referenceLocalType
                {
                    reportItem = ReportItems(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.Information).ToList())
                },
                document = ExportServiceHelper.Documents(header.Documents.ToList())
            };
        }

        private static annualBalanceF0503730Type_2015 Posistion2015(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503730Type_2015
            {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                                ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                : null,
                formationPeriod = year,
                generalData = ExportServiceHelper.AnnualBalanceFounderDataType2014(header),
                nonFinancialAssets = new annualBalanceF0503730Type_2015.nonFinancialAssetsLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.NonfinancialAssets).ToList())
                },
                financialAssets = new annualBalanceF0503730Type_2015.financialAssetsLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.FinancialAssets).ToList())
                },
                commitments = new annualBalanceF0503730Type_2015.commitmentsLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.Liabilities).ToList())
                },
                financialResult = new annualBalanceF0503730Type_2015.financialResultLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.FinancialResult).ToList())
                },
                reference = new annualBalanceF0503730Type_2015.referenceLocalType
                {
                    reportItem = ReportItems2015(header.AnnualBalanceF0503730.Where(
                        x => x.Section == (int)F0503130F0503730Details.Information).ToList())
                },
                document = ExportServiceHelper.Documents(header.Documents.ToList())
            };
        }

        private static reportItemF0503730BaseType ReportSubItem(F_Report_BalF0503730 docItem)
        {
            var result = new reportItemF0503730TopLevelType
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                targetFundsStartYear = docItem.targetFundsBegin,
                targetFundsEndYear = docItem.targetFundsEnd,
                servicesStartYear = docItem.servicesBegin,
                servicesEndYear = docItem.servicesEnd,
                temporaryFundsStartYear = docItem.temporaryFundsBegin,
                temporaryFundsEndYear = docItem.temporaryFundsEnd,
                totalStartYear = docItem.totalStartYear,
                totalEndYear = docItem.totalEndYear
            };

            return result;
        }

        private static List<reportItemF0503730TopLevelType> ReportItems(List<F_Report_BalF0503730> data)
        {
            var result = new List<reportItemF0503730TopLevelType>();

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

        private static reportItemF0503730BaseType2015 ReportSubItem2015(F_Report_BalF0503730 docItem)
        {
            var result = new reportItemF0503730TopLevelType_2015
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                targetFundsStartYear = docItem.targetFundsBegin,
                targetFundsEndYear = docItem.targetFundsEnd,
                stateTaskFundsStartYear = docItem.StateTaskFundStartYear,
                stateTaskFundsEndYear = docItem.StateTaskFundEndYear,
                revenueFundsStartYear = docItem.RevenueFundsStartYear,
                revenueFundsEndYear = docItem.RevenueFundsEndYear,
                totalStartYear = docItem.totalStartYear,
                totalEndYear = docItem.totalEndYear
            };

            return result;
        }

        private static List<reportItemF0503730TopLevelType_2015> ReportItems2015(List<F_Report_BalF0503730> data)
        {
            var result = new List<reportItemF0503730TopLevelType_2015>();

            foreach (var reportItemCode in data.Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";
                var reportItem = data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));

                string code = reportItemCode;
                var subItems = data.Where(x => x.lineCode.Substring(0, 2).Equals(code) && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItem2015).ToList();

                if (reportItem != null)
                {
                    result.Add(ReportItem2015(reportItem, subItems));
                }
            }

            return result;
        }
    }
}
