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
    public static class ExportAnnualBalanceF0503137Service
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
                new annualBalanceF0503137Type
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null,
                        formationPeriod = year,
                        generalData = ExportServiceHelper.AnnualBalanceBudgetGeneralDataType(header),
                        income = new annualBalanceF0503137Type.incomeLocalType
                                     {
                                         reportItem = ReportItems(
                                             header.AnnualBalanceF0503137
                                                 .Where(x => x.Section == (int)F0503137Details.Incomes).ToList())
                                     },
                        expense = new annualBalanceF0503137Type.expenseLocalType
                            {
                                          reportItem = ReportItems(header.AnnualBalanceF0503137
                                                                       .Where(x => x.Section == (int)F0503137Details.Expenses).ToList())
                                      },
                        fundingSources = new annualBalanceF0503137Type.fundingSourcesLocalType
                            {
                                                 reportItem = ReportItems(header.AnnualBalanceF0503137
                                                    .Where(x => x.Section == (int)F0503137Details.SourcesOfFinancing).ToList())
                                             },
                        document = ExportServiceHelper.Documents(documents)
                    };

            return ExportServiceHelper.Serialize(
                new annualBalanceF0503137
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new annualBalanceF0503137.bodyLocalType { position = position }
                    }.Save);
        }

        private static reportItemF0503137TopLevelType ReportItem(F_Report_BalF0503137 docItem, IList<reportItemF0503137BaseType> subItems)
        {
            var result = new reportItemF0503137TopLevelType
                             {
                                 name = docItem.Name,
                                 lineCode = docItem.lineCode,
                                 reportSubItem = subItems,
                                 approvedEstimateAssignments = docItem.approveEstimateAssign,
                                 execFinancialAuthorities = docItem.execFinancAuthorities,
                                 execBankAccounts = docItem.execBankAccounts,
                                 execNonCashOperations = docItem.execNonCashOperation,
                                 execTotal = docItem.execTotal,
                                 unexecAssignments = docItem.unexecAssignments
                             };

            if (docItem.budgClassifCode.IsNotNullOrEmpty())
            {
                result.budgetClassificationCode = docItem.budgClassifCode;
            }

            return result;
        }

        private static reportItemF0503137BaseType ReportSubItem(F_Report_BalF0503137 docItem)
        {
            var result = new reportItemF0503137BaseType
            {
                name = docItem.Name,
                lineCode = docItem.lineCode,
                approvedEstimateAssignments = docItem.approveEstimateAssign,
                execFinancialAuthorities = docItem.execFinancAuthorities,
                execBankAccounts = docItem.execBankAccounts,
                execNonCashOperations = docItem.execNonCashOperation,
                execTotal = docItem.execTotal,
                unexecAssignments = docItem.unexecAssignments
            };

            if (docItem.budgClassifCode.IsNotNullOrEmpty())
            {
                result.budgetClassificationCode = docItem.budgClassifCode;
            }

            return result;
        }

        private static List<reportItemF0503137TopLevelType> ReportItems(List<F_Report_BalF0503137> data)
        {
            var result = new List<reportItemF0503137TopLevelType>();

            // TODO �� ������ ������ ������������� �� ������������. �� ���� �� ������ 500 ������� � ���������� ����� 
            var iterations = 0;
            while (CheckDuplicationsLineCode(data))
            {
                RenumberDuplicationsLineCode(data);

                iterations++;

                if (iterations >= 500)
                {
                    throw new StackOverflowException("��������� ������������ ��� ������������� ������ � ����� F0503137");
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

        private static bool CheckDuplicationsLineCode(List<F_Report_BalF0503137> reportItem)
        {
            return reportItem.Any(ri => reportItem.Count(x => x.lineCode.Equals(ri.lineCode)) > 1);
        }

        // TODO ������ ������ �������!
        private static void RenumberDuplicationsLineCode(List<F_Report_BalF0503137> reportItem)
        {
            // �������� ��� ���������
            var duplication = reportItem.Where(ri => reportItem.Count(x => x.lineCode.Equals(ri.lineCode)) > 1).ToList();

            foreach (var duplicate in duplication.Select(ri => ri.lineCode).Distinct().ToList())
            {
                string dupl = duplicate;
                int counter;

                // ���� ��� ������ ������� �� 3� �������� ���, �� �������� ��������� �������� 2� ����� ����� �����
                if (dupl.Length == 3)
                {
                    // ���� ���� ��������� ��� ���
                    if (reportItem.Count(ri => ri.lineCode.Equals(dupl)
                                        && ri.budgClassifCode.Trim().IsNullOrEmpty()) > 1)
                    {
                        var reportItemDublicates = reportItem.Where(ri => ri.lineCode.Equals(dupl)
                                                             && ri.budgClassifCode.Trim().IsNullOrEmpty()).Skip(1);

                        counter = 1;
                        foreach (var reportItemDublicate in reportItemDublicates)
                        {
                            var code = reportItemDublicate.lineCode.Substring(0, 2);
                            reportItemDublicate.lineCode = code + counter.ToString(CultureInfo.InvariantCulture);
                            counter++;
                        }
                    }

                    // ��������� � ���
                    var duplications = reportItem.Where(ri => ri.lineCode.Equals(dupl) 
                                                        && ri.budgClassifCode != null 
                                                        && !ri.budgClassifCode.Trim().IsNullOrEmpty() 
                                                        && ri.budgClassifCode.Trim().All(char.IsDigit));

                    counter = 1;
                    var lineCode = dupl;
                    foreach (var itemF0503137 in duplications)
                    {
                        if (counter > 99)
                        {
                            counter = 1;
                            lineCode = (Convert.ToInt32(lineCode) + 1).ToString(CultureInfo.InvariantCulture);
                        }

                        itemF0503137.lineCode = "{0}.{1:00}".FormatWith(lineCode, counter);

                        counter++;
                    }
                }
                else
                {
                    // ���� ��� ������ ������� �� 6�� �������� ���.��
                    var duplications = reportItem.Where(ri => ri.lineCode.Equals(dupl));
                    counter = Convert.ToInt16(dupl.Substring(4));
                    foreach (var itemF0503137 in duplications)
                    {
                        if (counter != Convert.ToInt16(dupl.Substring(4)))
                        {
                            itemF0503137.lineCode = "{0}.{1:00}".FormatWith(itemF0503137.lineCode.Remove(3), counter);
                        }

                        counter++;
                    }
                }
            }
        }
    }
}
