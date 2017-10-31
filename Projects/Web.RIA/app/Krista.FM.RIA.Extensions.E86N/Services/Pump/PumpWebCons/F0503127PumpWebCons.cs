using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump.PumpWebCons
{
    /// <summary>
    /// Отчет об исполнении бюджета главного распорядителя (распорядителя),
    /// получателя средств бюджета, главного администратора,
    /// администратора источников финансирования дефицита бюджета,
    /// главного администратора, администратора доходов бюджета
    /// </summary>
    public sealed class F0503127PumpWebCons
    {
        public const string F0503127V1 = "F_0503127_Qof01022011";
        public const string F0503127V2016 = "F_0503127of20160101";
        
        private readonly INewRestService newRestService;
        private readonly F_F_ParameterDoc doc;

        public F0503127PumpWebCons(INewRestService newRestService, F_F_ParameterDoc doc)
        {
            this.newRestService = newRestService;
            this.doc = doc;
        }

        public void ProcessFormData0503127V1(string data)
        {
            var xmldoc = F0503127.Objects.Parse(data);

            var formDataIncomes = xmldoc.F_0503127_Qof01022011.reportSections.F_0503127_Qof01022011S1.rows.F_0503127_Qof01022011S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataExpenses = xmldoc.F_0503127_Qof01022011.reportSections.F_0503127_Qof01022011S2.rows.F_0503127_Qof01022011S2Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataFinancing = xmldoc.F_0503127_Qof01022011.reportSections.F_0503127_Qof01022011S3.rows.F_0503127_Qof01022011S3Row.Where(x => x.col1.IsNotNullOrEmpty());

            var docDataBudgetIncomes = doc.AnnualBalanceF0503127.Where(x => x.Section == (int)F0503127Details.BudgetIncomes);
            var docDataBudgetExpenses = doc.AnnualBalanceF0503127.Where(x => x.Section == (int)F0503127Details.BudgetExpenses);
            var docDataSourcesOfFinancing = doc.AnnualBalanceF0503127.Where(x => x.Section == (int)F0503127Details.SourcesOfFinancing);

            formDataIncomes.Each(x =>
            {
                var code = string.Concat(
                    x.col3 != null ? x.col3.ClsGRBSkey.clsId : string.Empty,
                    x.col4 != null ? x.col4.ClsDohodykey.clsId : string.Empty);

                var row = docDataBudgetIncomes.FirstOrDefault(d => d.lineCode.Equals(x.col1) && d.budgClassifCode.Equals(code)) ??
                    new F_Report_Bal0503127
                    {
                        ID = 0,
                        RefParametr = doc,
                        Section = (int)F0503127Details.BudgetIncomes
                    };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.budgClassifCode = code;

                row.ApproveBudgAssign = x.col5 ?? 0;
                row.execFinAuthorities = x.col7 ?? 0;
                row.execBankAccounts = x.col8 ?? 0;
                row.execNonCashOperation = x.col9 ?? 0;
                row.execTotal = x.col10 ?? 0;
                row.unexecAssignments = x.col11 ?? 0;

                newRestService.Save(row);
            });

            formDataExpenses.Each(x =>
            {
                var code = string.Concat(
                    x.col3 != null ? x.col3.ClsGRBSkey.clsId : string.Empty,
                    x.col4 != null ? x.col4.ClsRAZDLPODRAZDLkey.clsId : string.Empty,
                    x.col5 != null ? x.col5.ClsCSRkey.clsId : string.Empty,
                    x.col6 != null ? x.col6.ClsVRkey.clsId : string.Empty,
                    x.col7 != null ? x.col7.ClsOSGUkey.clsId : string.Empty);

                var row = docDataBudgetExpenses.FirstOrDefault(d => d.lineCode.Equals(x.col1) && d.budgClassifCode.Equals(code)) ??
                                            new F_Report_Bal0503127
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = (int)F0503127Details.BudgetExpenses
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.budgClassifCode = code;

                row.ApproveBudgAssign = x.col8 ?? 0;
                row.budgObligatLimits = x.col9 ?? 0;
                row.execFinAuthorities = x.col11 ?? 0;
                row.execBankAccounts = x.col12 ?? 0;
                row.execNonCashOperation = x.col13 ?? 0;
                row.execTotal = x.col14 ?? 0;
                row.unexecAssignments = x.col16 ?? 0;
                row.unexecBudgObligatLimit = x.col17 ?? 0;

                newRestService.Save(row);
            });

            formDataFinancing.Each(x =>
            {
                var code = string.Concat(
                    x.col3 != null ? x.col3.ClsGRBSkey.clsId : string.Empty,
                    x.col4 != null ? x.col4.ClsIstochnikykey.clsId : string.Empty);

                var row = docDataSourcesOfFinancing.FirstOrDefault(d => d.lineCode.Equals(x.col1) && d.budgClassifCode.Equals(code)) ??
                                            new F_Report_Bal0503127
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = (int)F0503127Details.SourcesOfFinancing
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.budgClassifCode = code;

                row.ApproveBudgAssign = x.col5 ?? 0;
                row.execFinAuthorities = x.col7 ?? 0;
                row.execBankAccounts = x.col8 ?? 0;
                row.execNonCashOperation = x.col9 ?? 0;
                row.execTotal = x.col10 ?? 0;
                row.unexecAssignments = x.col11 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503127V2016(string data)
        {
            var xmldoc = F_0503127of20160101.Objects.Parse(data);

            var formDataIncomes = xmldoc.F_0503127of20160101.reportSections.F_0503127of20160101S1.rows.F_0503127of20160101S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataExpenses = xmldoc.F_0503127of20160101.reportSections.F_0503127of20160101S2.rows.F_0503127of20160101S2Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataFinancing = xmldoc.F_0503127of20160101.reportSections.F_0503127of20160101S3.rows.F_0503127of20160101S3Row.Where(x => x.col1.IsNotNullOrEmpty());

            var docDataBudgetIncomes = doc.AnnualBalanceF0503127.Where(x => x.Section == (int)F0503127Details.BudgetIncomes);
            var docDataBudgetExpenses = doc.AnnualBalanceF0503127.Where(x => x.Section == (int)F0503127Details.BudgetExpenses);
            var docDataSourcesOfFinancing = doc.AnnualBalanceF0503127.Where(x => x.Section == (int)F0503127Details.SourcesOfFinancing);

            formDataIncomes.Each(x =>
            {
                var code = string.Concat(
                    x.col3 != null ? x.col3.ClsGRBSkey.clsId : string.Empty,
                    x.col4 != null ? x.col4.ClsDohodykey.clsId : string.Empty);

                var row = docDataBudgetIncomes.FirstOrDefault(d => d.lineCode.Equals(x.col1) && d.budgClassifCode.Equals(code)) ??
                    new F_Report_Bal0503127
                    {
                        ID = 0,
                        RefParametr = doc,
                        Section = (int)F0503127Details.BudgetIncomes
                    };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.budgClassifCode = code;

                row.ApproveBudgAssign = x.col5 ?? 0;
                row.execFinAuthorities = x.col7 ?? 0;
                row.execBankAccounts = x.col8 ?? 0;
                row.execNonCashOperation = x.col9 ?? 0;
                row.execTotal = x.col10 ?? 0;
                row.unexecAssignments = x.col11 ?? 0;

                newRestService.Save(row);
            });

            formDataExpenses.Each(x =>
            {
                var code = string.Concat(
                    x.col3 != null ? x.col3.ClsGRBSkey.clsId : string.Empty,
                    x.col4 != null ? x.col4.ClsRAZDLPODRAZDLkey.clsId : string.Empty,
                    x.col5 != null ? x.col5.ClsCSRkey.clsId : string.Empty,
                    x.col6 != null ? x.col6.ClsVRkey.clsId : string.Empty,
                    x.col7 ?? string.Empty);

                var row = docDataBudgetExpenses.FirstOrDefault(d => d.lineCode.Equals(x.col1) && d.budgClassifCode.Equals(code)) ??
                                            new F_Report_Bal0503127
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = (int)F0503127Details.BudgetExpenses
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.budgClassifCode = code;

                row.ApproveBudgAssign = x.col8 ?? 0;
                row.budgObligatLimits = x.col9 ?? 0;
                row.execFinAuthorities = x.col11 ?? 0;
                row.execBankAccounts = x.col12 ?? 0;
                row.execNonCashOperation = x.col13 ?? 0;
                row.execTotal = x.col14 ?? 0;
                row.unexecAssignments = x.col16 ?? 0;
                row.unexecBudgObligatLimit = x.col17 ?? 0;

                newRestService.Save(row);
            });

            formDataFinancing.Each(x =>
            {
                var code = string.Concat(
                    x.col3 != null ? x.col3.ClsGRBSkey.clsId : string.Empty,
                    x.col4 != null ? x.col4.ClsIstochnikykey.clsId : string.Empty);

                var row = docDataSourcesOfFinancing.FirstOrDefault(d => d.lineCode.Equals(x.col1) && d.budgClassifCode.Equals(code)) ??
                                            new F_Report_Bal0503127
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = (int)F0503127Details.SourcesOfFinancing
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.budgClassifCode = code;

                row.ApproveBudgAssign = x.col5 ?? 0;
                row.execFinAuthorities = x.col7 ?? 0;
                row.execBankAccounts = x.col8 ?? 0;
                row.execNonCashOperation = x.col9 ?? 0;
                row.execTotal = x.col10 ?? 0;
                row.unexecAssignments = x.col11 ?? 0;

                newRestService.Save(row);
            });
        }
    }
}