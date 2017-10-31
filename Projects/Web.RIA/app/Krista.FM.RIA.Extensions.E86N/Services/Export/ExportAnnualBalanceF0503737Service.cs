using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportAnnualBalanceF0503737Service
    {
        /// <summary>
        /// для этих кодов строки с кодами аналитики пишем в сабитемы без кода строки 
        /// </summary>
        private static readonly string[] WithAnaliticCode = { "200", "520" };

        /// <summary>
        /// для этого кода создается репортитем без аналитики если его нет на интерфейсе и записи с таким кодом и с аналитикой добавляются в сабитемы
        /// </summary>
        private static readonly string Code911 = "911";

        private static D_Org_Structure target;

        private static D_Org_UserProfile placerProfile;

        private static int year;
        
        public static Dictionary<string, string> GetTypeFinSupportsInDoc(F_F_ParameterDoc header)
        {
            var data = header.AnnualBalanceF0503737.Select(
                x =>
                new
                    {
                        x.RefTypeFinSupport.Code,
                        x.RefTypeFinSupport.Name
                    }).Distinct().ToList();

            if (!data.Any())
            {
                throw new InvalidDataException("Не заполнен ни один из интерфейсов документа");
            }

            return data.ToDictionary(item => item.Code, item => item.Name);
        }

        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            target = header.RefUchr;
            placerProfile = authService.Profile;
            year = header.RefYearForm.ID;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            if (year < 2015)
            {
                return ExportServiceHelper.Serialize(
                    new annualBalanceF0503737
                        {
                            header = ExportServiceHelper.HeaderType(),
                            body = new annualBalanceF0503737.bodyLocalType { position = Position(header) }
                        }.Save);
            }

            return ExportServiceHelper.Serialize(
                    new annualBalanceF0503737_2015
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new annualBalanceF0503737_2015.bodyLocalType { position = Position2015(header) }
                    }.Save);
        }

        private static annualBalanceF0503737Type Position(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503737Type
                       {
                           positionId = Guid.NewGuid().ToString(),
                           changeDate = DateTime.Now,
                           placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                           initiator = target.ID != placerProfile.RefUchr.ID
                               ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                               : null,
                           formationPeriod = year,
                           generalData = AnnualBalanceFounderFinSupportDataType(header),
                           document = ExportServiceHelper.Documents(
                               header.Documents.Any(x => !x.RefTypeDoc.Code.Equals("N") && !x.Url.Equals("НетФайла"))
                                   ? header.Documents.Where(x => !x.RefTypeDoc.Code.Equals("N")).ToList()
                                   : header.Documents.Where(x => x.RefTypeDoc.Code.Equals("N")).ToList())
                       };
        }

        private static annualBalanceF0503737Type_2015 Position2015(F_F_ParameterDoc header)
        {
            return new annualBalanceF0503737Type_2015
            {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                    ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                    : null,
                formationPeriod = year,
                generalData = AnnualBalanceFounderFinSupportDataType2015(header),
                document = ExportServiceHelper.Documents(
                    header.Documents.Any(x => !x.RefTypeDoc.Code.Equals("N") && !x.Url.Equals("НетФайла"))
                        ? header.Documents.Where(x => !x.RefTypeDoc.Code.Equals("N")).ToList()
                        : header.Documents.Where(x => x.RefTypeDoc.Code.Equals("N")).ToList())
            };
        }

        private static reportItemF0503737TopLevelType ReportItem(F_Report_BalF0503737 docItem, IList<reportItemF0503737BaseType> subItems)
        {
            var result = new reportItemF0503737TopLevelType
                             {
                                 name = docItem.Name,
                                 lineCode = docItem.lineCode,
                                 reportSubItem = subItems,
                                 approvedPlanAssignments = docItem.approvePlanAssign,
                                 execPersonalAuthorities = docItem.execPersonAuthorities,
                                 execBankAccounts = docItem.execBankAccounts,
                                 execNonCashOperations = docItem.execNonCashOperation,
                                 execCashAgency = docItem.execCashAgency,
                                 execTotal = docItem.execTotal,
                                 unexecPlanAssignments = docItem.unexecPlanAssign
                             };

            if (docItem.analyticCode.IsNotNullOrEmpty())
            {
                result.analyticCode = docItem.analyticCode;
            }

            return result;
        }

        private static reportItemF0503737BaseType ReportSubItem(F_Report_BalF0503737 docItem)
        {
            var result = new reportItemF0503737TopLevelType
            {
                name = docItem.Name,
                approvedPlanAssignments = docItem.approvePlanAssign,
                execPersonalAuthorities = docItem.execPersonAuthorities,
                execBankAccounts = docItem.execBankAccounts,
                execNonCashOperations = docItem.execNonCashOperation,
                execCashAgency = docItem.execCashAgency,
                execTotal = docItem.execTotal,
                unexecPlanAssignments = docItem.unexecPlanAssign
            };

            if (!WithAnaliticCode.Contains(docItem.lineCode))
            {
                result.lineCode = docItem.lineCode;
            }

            if (docItem.analyticCode.IsNotNullOrEmpty())
            {
                result.analyticCode = docItem.analyticCode;
            }

            return result;
        }

        private static reportItemF0503737TopLevelReturnExpenseType_2015 ReportItem2015(F_Report_BalF0503737 docItem, IList<reportItemF0503737BaseReturnExpenseType_2015> subItems)
        {
            var result = new reportItemF0503737TopLevelReturnExpenseType_2015
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                reportSubItem = subItems,
                returnBankAccounts = docItem.execBankAccounts,
                returnCashAgency = docItem.execCashAgency,
                returnNonCashOperations = docItem.execNonCashOperation,
                returnPersonalAuthorities = docItem.execPersonAuthorities,
                returnTotal = docItem.execTotal
            };

            if (docItem.analyticCode.IsNotNullOrEmpty())
            {
                result.analyticCode = docItem.analyticCode;
            }
            
            return result;
        }
        
        private static reportItemF0503737BaseReturnExpenseType_2015 ReportSubItem2015(F_Report_BalF0503737 docItem)
        {
            var result = new reportItemF0503737TopLevelReturnExpenseType_2015
            {
                name = docItem.Name,
                
                returnCashAgency = docItem.execCashAgency,
                returnBankAccounts = docItem.execBankAccounts,
                returnNonCashOperations = docItem.execNonCashOperation,
                returnPersonalAuthorities = docItem.execPersonAuthorities,
                returnTotal = docItem.execTotal
            };
            
            if (docItem.analyticCode.IsNotNullOrEmpty())
            {
                result.analyticCode = docItem.analyticCode;
            }

            if (!docItem.lineCode.Equals(Code911))
            {
                result.lineCode = docItem.lineCode;
            }

            return result;
        }

        private static List<reportItemF0503737TopLevelType> ReportItems(List<F_Report_BalF0503737> data, bool flag = false)
        {
            var reportItems = new[] { "700" };

            var reportSubItems = new[] { "710", "720" };

            var union = reportItems.Concat(reportSubItems);
            
            var result = new List<reportItemF0503737TopLevelType>();

            foreach (var reportItemCode in data.Where(x => !union.Contains(x.lineCode)).Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";

                var reportItem = WithAnaliticCode.Contains(lineCodeReportItem) 
                                ? data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem) && x.analyticCode.IsNullOrEmpty()) 
                                : data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));
                
                string code = reportItemCode;
                var subItems = data.Where(x => !union.Contains(x.lineCode) 
                                            && x.lineCode.Substring(0, 2).Equals(code)
                                            && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItem).ToList();

                if (WithAnaliticCode.Contains(lineCodeReportItem))
                {
                    var subItemsAnaliticCodes = data.Where(x => !union.Contains(x.lineCode)
                                                      && x.lineCode.Equals(lineCodeReportItem)
                                                      && x.analyticCode.IsNotNullOrEmpty())
                                          .Select(ReportSubItem).ToList();

                    subItems.AddRange(subItemsAnaliticCodes);
                }

                if (reportItem != null)
                {
                    result.Add(ReportItem(reportItem, subItems));
                }
            }

            if (flag)
            {
                var ri = data.FirstOrDefault(x => x.lineCode.Equals(reportItems.First())) ??
                                new F_Report_BalF0503737
                                {
                                    lineCode = reportItems.First(),
                                    Name = "Изменение остатков средств"
                                };

                result.Add(ReportItem(ri, data.Where(x => reportSubItems.Contains(x.lineCode)).Select(ReportSubItem).ToList()));
            }

            return result;
        }

        private static List<reportItemF0503737TopLevelReturnExpenseType_2015> ReportItems2015(List<F_Report_BalF0503737> data)
        {
            var result = new List<reportItemF0503737TopLevelReturnExpenseType_2015>();

            foreach (var reportItemCode in data.Where(x => !Code911.Equals(x.lineCode)).Select(x => x.lineCode.Substring(0, 2)).Distinct())
            {
                var lineCodeReportItem = reportItemCode + "0";
                var reportItem = data.FirstOrDefault(x => x.lineCode.Equals(lineCodeReportItem));

                string code = reportItemCode;
                var subItems = data.Where(x => !Code911.Equals(x.lineCode)
                                            && x.lineCode.Substring(0, 2).Equals(code)
                                            && !x.lineCode.Equals(lineCodeReportItem)).Select(ReportSubItem2015).ToList();

                if (reportItem != null)
                {
                    result.Add(ReportItem2015(reportItem, subItems));
                }
            }
            
            var ri = data.FirstOrDefault(x => x.lineCode.Equals(Code911) && x.analyticCode.IsNullOrEmpty()) ??
                            new F_Report_BalF0503737
                            {
                                lineCode = Code911,
                                Name = "из них по кодам аналитики"
                            };

            result.Add(ReportItem2015(ri, data.Where(x => Code911.Equals(x.lineCode) && x.analyticCode.IsNotNullOrEmpty()).Select(ReportSubItem2015).ToList()));
            
            return result;
        }

        private static annualBalanceFounderFinSupportDataType AnnualBalanceFounderFinSupportDataType(F_F_ParameterDoc header)
        {
            F_F_Founder founder;
            
            D_Org_Structure uchr = header.RefUchr;
            F_Report_HeadAttribute reportHeadAttribute = header.ReportHeadAttribute.First();

            F_Org_Passport passport = ExportServiceHelper.GetLastPassport(uchr.ID);

            try
            {
                founder = Resolver.Get<ILinqRepository<F_F_Founder>>().FindAll().First(x => x.RefPassport.ID == passport.ID);
            }
            catch
            {
                throw new InvalidOperationException("Отсутствует учредитель");
            }

            var financialSupportDataTmp = header.AnnualBalanceF0503737.ToLookup(x => x.RefTypeFinSupport.Code)
                .Select(
                    x => new annualBalanceFounderFinSupportDataType.financialSupportDataLocalType
                             {
                                 typeFinancialSupport = x.Key,
                                 income = new annualBalanceFounderFinSupportDataType.financialSupportDataLocalType.incomeLocalType
                                              {
                                                  reportItem = ReportItems(x.Where(ri => ri.Section == (int)F0503737Details.Incomes).ToList())
                                              },
                                 expense = new annualBalanceFounderFinSupportDataType.financialSupportDataLocalType.expenseLocalType
                                               {
                                                   reportItem = ReportItems(x.Where(ri => ri.Section == (int)F0503737Details.Expenses).ToList())
                                               },
                                 fundingSources = new annualBalanceFounderFinSupportDataType.financialSupportDataLocalType.fundingSourcesLocalType
                                                      {
                                                          reportItem = ReportItems(x.Where(ri => ri.Section == (int)F0503737Details.SourcesOfFinancing).ToList(), true)
                                                      }
                             })
                .Where(
                    financialSupportData =>
                        financialSupportData.income.reportItem.Any(
                            reportItem =>
                                reportItem.approvedPlanAssignments != 0 || reportItem.execPersonalAuthorities != 0 || reportItem.execBankAccounts != 0 ||
                                reportItem.execCashAgency != 0 || reportItem.execNonCashOperations != 0 || reportItem.execTotal != 0 || reportItem.unexecPlanAssignments != 0 ||

                                reportItem.reportSubItem.Any(
                                    reportSubItem =>
                                        reportSubItem.approvedPlanAssignments != 0 || reportSubItem.execPersonalAuthorities != 0 || reportSubItem.execBankAccounts != 0 ||
                                        reportSubItem.execCashAgency != 0 || reportSubItem.execNonCashOperations != 0 || reportSubItem.execTotal != 0
                                        || reportSubItem.unexecPlanAssignments != 0)) ||

                        financialSupportData.expense.reportItem.Any(
                            reportItem =>
                                reportItem.approvedPlanAssignments != 0 || reportItem.execPersonalAuthorities != 0 || reportItem.execBankAccounts != 0 ||
                                reportItem.execCashAgency != 0 || reportItem.execNonCashOperations != 0 || reportItem.execTotal != 0 || reportItem.unexecPlanAssignments != 0 ||

                                reportItem.reportSubItem.Any(
                                    reportSubItem =>
                                        reportSubItem.approvedPlanAssignments != 0 || reportSubItem.execPersonalAuthorities != 0 || reportSubItem.execBankAccounts != 0 ||
                                        reportSubItem.execCashAgency != 0 || reportSubItem.execNonCashOperations != 0 || reportSubItem.execTotal != 0
                                        || reportSubItem.unexecPlanAssignments != 0)) ||

                        financialSupportData.fundingSources.reportItem.Any(
                            reportItem =>
                                reportItem.approvedPlanAssignments != 0 || reportItem.execPersonalAuthorities != 0 || reportItem.execBankAccounts != 0 ||
                                reportItem.execCashAgency != 0 || reportItem.execNonCashOperations != 0 || reportItem.execTotal != 0 || reportItem.unexecPlanAssignments != 0 ||

                                reportItem.reportSubItem.Any(
                                    reportSubItem =>
                                        reportSubItem.approvedPlanAssignments != 0 || reportSubItem.execPersonalAuthorities != 0 || reportSubItem.execBankAccounts != 0 ||
                                        reportSubItem.execCashAgency != 0 || reportSubItem.execNonCashOperations != 0 || reportSubItem.execTotal != 0
                                        || reportSubItem.unexecPlanAssignments != 0)))
                .ToList();

            if (!financialSupportDataTmp.Any())
            {
                throw new InvalidOperationException("Документ пуст.");
            }

            return
                new annualBalanceFounderFinSupportDataType
                    {
                        date = reportHeadAttribute.Datedata,
                        periodicity = reportHeadAttribute.RefPeriodic.NameEng,
                        okei = new refNsiOkeiType
                                   {
                                       code = "383",
                                       symbol = "руб"
                                   },
                        okpo = passport.OKPO,
                        okato = new refNsiOkatoType
                                    {
                                        code = passport.RefOKATO.Code
                                    },
                        section = string.Format("{0,3}", uchr.RefOrgGRBS.Code.PadLeft(3, '0')),
                        founderName = founder.RefYchred.Name,
                        founderAuthority = new refNsiConsRegSoftType
                                               {
                                                   // todo бывает что у учредителя не проставлен nsiOgs!!!
                                                   regNum = founder.RefYchred.RefNsiOgs.regNum,
                                                   fullName = founder.RefYchred.RefNsiOgs.FullName
                                               },
                        founderAuthorityOkpo = reportHeadAttribute.founderAuthorityOkpo,
                        financialSupportData = financialSupportDataTmp
                    };
        }

        private static annualBalanceFounderFinSupportDataType_2015 AnnualBalanceFounderFinSupportDataType2015(F_F_ParameterDoc header)
        {
            F_F_Founder founder;

            D_Org_Structure uchr = header.RefUchr;
            F_Report_HeadAttribute reportHeadAttribute = header.ReportHeadAttribute.First();

            F_Org_Passport passport = ExportServiceHelper.GetLastPassport(uchr.ID);

            try
            {
                founder = Resolver.Get<ILinqRepository<F_F_Founder>>().FindAll().First(x => x.RefPassport.ID == passport.ID);
            }
            catch
            {
                throw new InvalidOperationException("Отсутствует учредитель");
            }

            var financialSupportDataTmp = header.AnnualBalanceF0503737.ToLookup(x => x.RefTypeFinSupport.Code)
                .Select(
                    x => new annualBalanceFounderFinSupportDataType_2015.financialSupportDataLocalType
                             {
                                typeFinancialSupport = x.Key,
                                income = new annualBalanceFounderFinSupportDataType_2015.financialSupportDataLocalType.incomeLocalType
                                              {
                                                  reportItem = ReportItems(x.Where(ri => ri.Section == (int)F0503737Details.Incomes).ToList())
                                              },
                                expense = new annualBalanceFounderFinSupportDataType_2015.financialSupportDataLocalType.expenseLocalType
                                               {
                                                   reportItem = ReportItems(x.Where(ri => ri.Section == (int)F0503737Details.Expenses).ToList())
                                               },
                                fundingSources = new annualBalanceFounderFinSupportDataType_2015.financialSupportDataLocalType.fundingSourcesLocalType
                                                      {
                                                          reportItem = ReportItems(x.Where(ri => ri.Section == (int)F0503737Details.SourcesOfFinancing).ToList(), true)
                                                      },
                                returnExpense = new annualBalanceFounderFinSupportDataType_2015.financialSupportDataLocalType.returnExpenseLocalType
                                                        {
                                                            reportItem = ReportItems2015(x.Where(ri => ri.Section == (int)F0503737Details.ReturnExpense).ToList())
                                                        }
                             })
                .Where(
                    financialSupportData =>
                        financialSupportData.income.reportItem.Any(
                            reportItem =>
                                reportItem.approvedPlanAssignments != 0 || reportItem.execPersonalAuthorities != 0 || reportItem.execBankAccounts != 0 ||
                                reportItem.execCashAgency != 0 || reportItem.execNonCashOperations != 0 || reportItem.execTotal != 0 || reportItem.unexecPlanAssignments != 0 ||

                                reportItem.reportSubItem.Any(
                                    reportSubItem =>
                                        reportSubItem.approvedPlanAssignments != 0 || reportSubItem.execPersonalAuthorities != 0 || reportSubItem.execBankAccounts != 0 ||
                                        reportSubItem.execCashAgency != 0 || reportSubItem.execNonCashOperations != 0 || reportSubItem.execTotal != 0
                                        || reportSubItem.unexecPlanAssignments != 0)) ||

                        financialSupportData.expense.reportItem.Any(
                            reportItem =>
                                reportItem.approvedPlanAssignments != 0 || reportItem.execPersonalAuthorities != 0 || reportItem.execBankAccounts != 0 ||
                                reportItem.execCashAgency != 0 || reportItem.execNonCashOperations != 0 || reportItem.execTotal != 0 || reportItem.unexecPlanAssignments != 0 ||

                                reportItem.reportSubItem.Any(
                                    reportSubItem =>
                                        reportSubItem.approvedPlanAssignments != 0 || reportSubItem.execPersonalAuthorities != 0 || reportSubItem.execBankAccounts != 0 ||
                                        reportSubItem.execCashAgency != 0 || reportSubItem.execNonCashOperations != 0 || reportSubItem.execTotal != 0
                                        || reportSubItem.unexecPlanAssignments != 0)) ||

                        financialSupportData.fundingSources.reportItem.Any(
                            reportItem =>
                                reportItem.approvedPlanAssignments != 0 || reportItem.execPersonalAuthorities != 0 || reportItem.execBankAccounts != 0 ||
                                reportItem.execCashAgency != 0 || reportItem.execNonCashOperations != 0 || reportItem.execTotal != 0 || reportItem.unexecPlanAssignments != 0 ||

                                reportItem.reportSubItem.Any(
                                    reportSubItem =>
                                        reportSubItem.approvedPlanAssignments != 0 || reportSubItem.execPersonalAuthorities != 0 || reportSubItem.execBankAccounts != 0 ||
                                        reportSubItem.execCashAgency != 0 || reportSubItem.execNonCashOperations != 0 || reportSubItem.execTotal != 0
                                        || reportSubItem.unexecPlanAssignments != 0)) ||

                        financialSupportData.returnExpense.reportItem.Any(
                            reportItem =>
                                reportItem.returnPersonalAuthorities != 0 || reportItem.returnBankAccounts != 0 || reportItem.returnCashAgency != 0 ||
                                reportItem.returnNonCashOperations != 0 || reportItem.returnTotal != 0 ||

                                reportItem.reportSubItem.Any(
                                    reportSubItem =>
                                        reportSubItem.returnPersonalAuthorities != 0 || reportSubItem.returnBankAccounts != 0 || reportSubItem.returnCashAgency != 0 ||
                                        reportSubItem.returnNonCashOperations != 0 || reportSubItem.returnTotal != 0)))
                .ToList();

            if (!financialSupportDataTmp.Any())
            {
                throw new InvalidOperationException("Документ пуст.");
            }

            return
                new annualBalanceFounderFinSupportDataType_2015
                    {
                        date = reportHeadAttribute.Datedata,
                        periodicity = reportHeadAttribute.RefPeriodic.NameEng,
                        okei = new refNsiOkeiType
                                   {
                                       code = "383",
                                       symbol = "руб"
                                   },
                        okpo = passport.OKPO,
                        oktmo = new refNsiOktmoType
                                    {
                                        code = passport.RefOKTMO.Code
                                    },
                        section = string.Format("{0,3}", uchr.RefOrgGRBS.Code.PadLeft(3, '0')),
                        founderName = founder.RefYchred.Name,
                        founderAuthority = new refNsiConsRegSoftType
                                               {
                                                   // todo бывает что у учредителя не проставлен nsiOgs!!!
                                                   regNum = founder.RefYchred.RefNsiOgs.regNum,
                                                   fullName = founder.RefYchred.RefNsiOgs.FullName
                                               },
                        founderAuthorityOkpo = reportHeadAttribute.founderAuthorityOkpo,
                        financialSupportData = financialSupportDataTmp
                    };
        }
    }
}
