using System;
using System.Collections.Generic;
using System.Globalization;
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
    public static class ExportAnnualBalanceF0503127Service
    {
        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            D_Org_Structure target = header.RefUchr;
            D_Org_UserProfile placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            List<F_Doc_Docum> documents = header.Documents.ToList();
            int year = header.RefYearForm.ID;

            var position =
                new annualBalanceF0503127Type
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null,
                        formationPeriod = year,
                        generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataType(header),
                        income = new annualBalanceF0503127Type.incomeLocalType
                                     {
                                         reportItem = ReportItems(header.AnnualBalanceF0503127.Where(
                                             x => x.Section == (int)F0503127Details.BudgetIncomes).ToList())
                                     },
                        expense = new annualBalanceF0503127Type.expenseLocalType
                            {
                                          reportItem = ReportItems(header.AnnualBalanceF0503127.Where(
                                              x => x.Section == (int)F0503127Details.BudgetExpenses).ToList())
                            },
                        fundingSources = new annualBalanceF0503127Type.fundingSourcesLocalType
                            {
                                                 reportItem = ReportItems(header.AnnualBalanceF0503127.Where(
                                                     x => x.Section == (int)F0503127Details.SourcesOfFinancing).ToList())
                            },
                        document = ExportServiceHelper.Documents(documents)
                    };

            return ExportServiceHelper.Serialize(
                new annualBalanceF0503127
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new annualBalanceF0503127.bodyLocalType { position = position }
                    }.Save);
        }

        private static reportItemF0503127TopLevelType ReportItem(F_Report_Bal0503127 docItem, IList<reportItemF0503127BaseType> subItems)
        {
            var result = new reportItemF0503127TopLevelType
                {
                    name = docItem.Name,
                    lineCode = docItem.lineCode,
                    reportSubItem = subItems,
                    approvedBudgetAssignments = docItem.ApproveBudgAssign,
                    budgetObligationLimits = docItem.budgObligatLimits,
                    execFinancialAuthorities = docItem.execFinAuthorities,
                    execBankAccounts = docItem.execBankAccounts,
                    execNonCashOperations = docItem.execNonCashOperation,
                    execTotal = docItem.execTotal,
                    unexecAssignments = docItem.unexecAssignments,
                    unexecBudgetObligationLimit = docItem.unexecBudgObligatLimit
                };

            if (docItem.budgClassifCode.IsNotNullOrEmpty())
            {
                result.budgetClassificationCode = docItem.budgClassifCode;
            }

            return result;
        }

        private static reportItemF0503127BaseType ReportSubItem(F_Report_Bal0503127 docItem)
        {
            var result = new reportItemF0503127BaseType
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                approvedBudgetAssignments = docItem.ApproveBudgAssign,
                budgetObligationLimits = docItem.budgObligatLimits,
                execFinancialAuthorities = docItem.execFinAuthorities,
                execBankAccounts = docItem.execBankAccounts,
                execNonCashOperations = docItem.execNonCashOperation,
                execTotal = docItem.execTotal,
                unexecAssignments = docItem.unexecAssignments,
                unexecBudgetObligationLimit = docItem.unexecBudgObligatLimit
            };

            if (docItem.budgClassifCode.IsNotNullOrEmpty())
            {
                result.budgetClassificationCode = docItem.budgClassifCode;
            }

            return result;
        }

        private static List<reportItemF0503127TopLevelType> ReportItems(List<F_Report_Bal0503127> data)
        {
            var result = new List<reportItemF0503127TopLevelType>();

            // TODO на всякий случай подстрахуемся от зацикливания. На вряд ли больше 500 строчек в интерфейсе будет 
            var iterations = 0;
            while (CheckDuplicationsLineCode(data))
            {
                RenumberDuplicationsLineCode(data);

                iterations++;

                if (iterations >= 500)
                {
                    throw new StackOverflowException("Произошло зацикливание при перекодировке дублей в форме F0503127");
                }
            }

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

        private static bool CheckDuplicationsLineCode(List<F_Report_Bal0503127> data)
        {
            return data.Any(ri => data.Count(x => x.lineCode.Equals(ri.lineCode)) > 1);
        }

        // TODO полный ахтунг конечно!
        private static void RenumberDuplicationsLineCode(List<F_Report_Bal0503127> data)
        {
            // выбираем все дубликаты
            var duplication = data.Where(ri => data.Count(x => x.lineCode.Equals(ri.lineCode)) > 1).ToList();
            
            foreach (var duplicate in duplication.Select(ri => ri.lineCode).Distinct().ToList())
            {
                string dupl = duplicate;
                int counter;
                
                // если код строки состоит из 3х символов ХХХ, то нумеруем дубликаты добавляя 2а знака после точки
                if (dupl.Length == 3)
                {
                    // если есть дубликаты без КБК
                    if (data.Count(ri => ri.lineCode.Equals(dupl)
                                        && ri.budgClassifCode.Trim().IsNullOrEmpty()) > 1)
                    {
                        var reportItemDublicates = data.Where(ri => ri.lineCode.Equals(dupl)
                                                             && ri.budgClassifCode.Trim().IsNullOrEmpty()).Skip(1);

                        counter = 1;
                        foreach (var reportItemDublicate in reportItemDublicates)
                        {
                            var code = reportItemDublicate.lineCode.Substring(0, 2);
                            reportItemDublicate.lineCode = code + counter.ToString(CultureInfo.InvariantCulture);
                            counter++;
                        }
                    }
                    
                    // дубликаты с КБК
                    var duplications = data.Where(ri => ri.lineCode.Equals(dupl) && ri.budgClassifCode != null &&
                            !ri.budgClassifCode.Trim().IsNullOrEmpty());

                    counter = 1;
                    var lineCode = dupl;
                    foreach (var itemF0503127 in duplications)
                    {
                        if (counter > 99)
                        {
                            counter = 1;
                            lineCode = (Convert.ToInt32(lineCode) + 1).ToString(CultureInfo.InvariantCulture);
                        }

                        itemF0503127.lineCode = "{0}.{1:00}".FormatWith(lineCode, counter);    
                        
                        counter++;
                    }    
                }
                else
                {
                    // если код строки состоит из 6ти символов ХХХ.ХХ
                    var duplications = data.Where(ri => ri.lineCode.Equals(dupl));
                    counter = Convert.ToInt16(dupl.Substring(4));
                    foreach (var itemF0503127 in duplications)
                    {
                        if (counter != Convert.ToInt16(dupl.Substring(4)))
                        {
                            itemF0503127.lineCode = "{0}.{1:00}".FormatWith(itemF0503127.lineCode.Remove(3), counter);
                        }

                        counter++;
                    }    
                }
            }
        }
    }
}
