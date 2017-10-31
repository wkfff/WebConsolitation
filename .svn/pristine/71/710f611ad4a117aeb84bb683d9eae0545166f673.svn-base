using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump.PumpWebCons
{
    /// <summary>
    /// Отчет об исполнении учреждением плана его ФХД. Собственные доходы учреждения
    /// </summary>
    public sealed class F05037372PumpWebCons
    {
        public const string F05037372V1 = "F_0503737_2of";
        public const string F05037372V2 = "F_0503737_2of01122012";
        public const string F05037372V3 = "F_0503737_2of20150101";
        public const string F05037372V2016 = "F_0503737_2of20160101";

        private readonly INewRestService newRestService;
        private readonly F_F_ParameterDoc doc;

        public F05037372PumpWebCons(INewRestService newRestService, F_F_ParameterDoc doc)
        {
            this.newRestService = newRestService;
            this.doc = doc;
        }

        public void ProcessFormData05037372V2(string data)
        {
            var xmldoc = F05037372.Objects.Parse(data);

            if (xmldoc.F_0503737_2of01122012.headerReqHolder != null)
            {
                PumpWebCons.SetHeadAttr(xmldoc.F_0503737_2of01122012.headerReqHolder.F_0503737_2of01122012HR.req9, doc);
            }

            if (xmldoc.F_0503737_2of01122012.reportSections != null)
            {
                var formDataIncomes = xmldoc.F_0503737_2of01122012.reportSections.F_0503737_2of01122012S1.rows.F_0503737_2of01122012S1Row.Where(x => x.col1.IsNotNullOrEmpty());
                var formDataExpenses = xmldoc.F_0503737_2of01122012.reportSections.F_0503737_2of01122012S2.rows.F_0503737_2of01122012S2Row.Where(x => x.col1.IsNotNullOrEmpty());
                var formDataFinancing = xmldoc.F_0503737_2of01122012.reportSections.F_0503737_2of01122012S3.rows.F_0503737_2of01122012S3Row.Where(x => x.col1.IsNotNullOrEmpty());

                const int TypeFinSupport = FX_FX_typeFinSupport.OwnRevenuesID;

                var docDataBudgetIncomes = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.Incomes
                                                                           && x.RefTypeFinSupport.ID == TypeFinSupport);
                var docDataBudgetExpenses = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.Expenses
                                                                            && x.RefTypeFinSupport.ID == TypeFinSupport);
                var docDataSourcesOfFinancing = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.SourcesOfFinancing
                                                                                && x.RefTypeFinSupport.ID == TypeFinSupport);

                formDataIncomes.Each(x =>
                {
                    var row = docDataBudgetIncomes.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.Incomes,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });

                formDataExpenses.Each(x =>
                {
                    var row = docDataBudgetExpenses.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.Expenses,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });

                formDataFinancing.Each(x =>
                {
                    var row = docDataSourcesOfFinancing.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.SourcesOfFinancing,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });
            }
        }

        public void ProcessFormData05037372V3(string data)
        {
            var xmldoc = F_0503737_2of20150101.Objects.Parse(data);

            if (xmldoc.F_0503737_2of20150101.headerReqHolder != null)
            {
                PumpWebCons.SetHeadAttr(xmldoc.F_0503737_2of20150101.headerReqHolder.F_0503737_2of20150101HR.req9, doc);
            }

            if (xmldoc.F_0503737_2of20150101.reportSections != null)
            {
                var formDataIncomes = xmldoc.F_0503737_2of20150101.reportSections.F_0503737_2of20150101S1.rows.F_0503737_2of20150101S1Row.Where(x => x.col1.IsNotNullOrEmpty());
                var formDataExpenses = xmldoc.F_0503737_2of20150101.reportSections.F_0503737_2of20150101S2.rows.F_0503737_2of20150101S2Row.Where(x => x.col1.IsNotNullOrEmpty());
                var formDataFinancing = xmldoc.F_0503737_2of20150101.reportSections.F_0503737_2of20150101S3.rows.F_0503737_2of20150101S3Row.Where(x => x.col1.IsNotNullOrEmpty());
                var formDataReturnExpense = xmldoc.F_0503737_2of20150101.reportSections.F_0503737_2of20150101S4.rows.F_0503737_2of20150101S4Row.Where(x => x.col1.IsNotNullOrEmpty());

                const int TypeFinSupport = FX_FX_typeFinSupport.OwnRevenuesID;

                var docDataBudgetIncomes = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.Incomes
                                                                           && x.RefTypeFinSupport.ID == TypeFinSupport);
                var docDataBudgetExpenses = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.Expenses
                                                                            && x.RefTypeFinSupport.ID == TypeFinSupport);
                var docDataSourcesOfFinancing = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.SourcesOfFinancing
                                                                                && x.RefTypeFinSupport.ID == TypeFinSupport);
                var docDataReturnExpense = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.ReturnExpense
                                                                                    && x.RefTypeFinSupport.ID == FX_FX_typeFinSupport.SubsidyID);

                formDataIncomes.Each(x =>
                {
                    var row = docDataBudgetIncomes.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.Incomes,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });

                formDataExpenses.Each(x =>
                {
                    var row = docDataBudgetExpenses.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.Expenses,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });

                formDataFinancing.Each(x =>
                {
                    var row = docDataSourcesOfFinancing.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.SourcesOfFinancing,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });

                formDataReturnExpense.Each(x =>
                {
                    var row = docDataReturnExpense.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.ReturnExpense,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(FX_FX_typeFinSupport.SubsidyID)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);

                    // костыль строки 910 с аналитиками должны на самом деле быть 911ми
                    string[] analiticCode = { "130", "180" };

                    if (x.col1.Equals("910") && analiticCode.Contains(x.col2))
                    {
                        row.lineCode = "911";
                    }
                    else
                    {
                        row.lineCode = x.col1;
                    }
                    
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });
            }
        }

        public void ProcessFormData05037372V2016(string data)
        {
            var xmldoc = F_0503737_2of20160101.Objects.Parse(data);

            if (xmldoc.F_0503737_2of20160101.headerReqHolder != null)
            {
                PumpWebCons.SetHeadAttr(xmldoc.F_0503737_2of20160101.headerReqHolder.F_0503737_2of20160101HR.req9, doc);
            }

            if (xmldoc.F_0503737_2of20160101.reportSections != null)
            {
                var formDataIncomes = xmldoc.F_0503737_2of20160101.reportSections.F_0503737_2of20160101S1.rows.F_0503737_2of20160101S1Row.Where(x => x.col1.IsNotNullOrEmpty());
                var formDataExpenses = xmldoc.F_0503737_2of20160101.reportSections.F_0503737_2of20160101S2.rows.F_0503737_2of20160101S2Row.Where(x => x.col1.IsNotNullOrEmpty());
                var formDataFinancing = xmldoc.F_0503737_2of20160101.reportSections.F_0503737_2of20160101S3.rows.F_0503737_2of20160101S3Row.Where(x => x.col1.IsNotNullOrEmpty());
                var formDataReturnExpense = xmldoc.F_0503737_2of20160101.reportSections.F_0503737_2of20160101S4.rows.F_0503737_2of20160101S4Row.Where(x => x.col1.IsNotNullOrEmpty());

                const int TypeFinSupport = FX_FX_typeFinSupport.OwnRevenuesID;

                var docDataBudgetIncomes = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.Incomes
                                                                           && x.RefTypeFinSupport.ID == TypeFinSupport);
                var docDataBudgetExpenses = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.Expenses
                                                                            && x.RefTypeFinSupport.ID == TypeFinSupport);
                var docDataSourcesOfFinancing = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.SourcesOfFinancing
                                                                                && x.RefTypeFinSupport.ID == TypeFinSupport);
                var docDataReturnExpense = doc.AnnualBalanceF0503737.Where(x => x.Section == (int)F0503737Details.ReturnExpense
                                                                                    && x.RefTypeFinSupport.ID == FX_FX_typeFinSupport.SubsidyID);

                formDataIncomes.Each(x =>
                {
                    var row = docDataBudgetIncomes.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.Incomes,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;
                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });

                formDataExpenses.Each(x =>
                {
                    var row = docDataBudgetExpenses.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.Expenses,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });

                formDataFinancing.Each(x =>
                {
                    var row = docDataSourcesOfFinancing.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.SourcesOfFinancing,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);
                    row.lineCode = x.col1;
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = x.col3 ?? 0;
                    row.execPersonAuthorities = x.col5 ?? 0;
                    row.execBankAccounts = x.col6 ?? 0;
                    row.execCashAgency = x.col7 ?? 0;
                    row.execNonCashOperation = x.col8 ?? 0;
                    row.execTotal = x.col9 ?? 0;
                    row.unexecPlanAssign = x.col10 ?? 0;

                    newRestService.Save(row);
                });

                formDataReturnExpense.Each(x =>
                {
                    var row = docDataReturnExpense.FirstOrDefault(d => d.lineCode.Equals(x.col1)) ??
                                                new F_Report_BalF0503737
                                                {
                                                    ID = 0,
                                                    RefParametr = doc,
                                                    Section = (int)F0503737Details.ReturnExpense,
                                                    RefTypeFinSupport = newRestService.GetItem<FX_FX_typeFinSupport>(TypeFinSupport)
                                                };

                    row.Name = PumpWebCons.GetIndicatorName(x.col0);

                    // костыль строки 910 с аналитиками должны на самом деле быть 911ми
                    string[] analiticCode = { "130", "180" };

                    if (x.col1.Equals("910") && analiticCode.Contains(x.col2))
                    {
                        row.lineCode = "911";
                    }
                    else
                    {
                        row.lineCode = x.col1;
                    }
                    
                    row.analyticCode = x.col2;

                    row.approvePlanAssign = 0;
                    row.execPersonAuthorities = x.col4.GetValueOrDefault();
                    row.execBankAccounts = x.col5.GetValueOrDefault();
                    row.execCashAgency = x.col6.GetValueOrDefault();
                    row.execNonCashOperation = x.col7.GetValueOrDefault();
                    row.execTotal = x.col8.GetValueOrDefault();
                    row.unexecPlanAssign = 0;

                    newRestService.Save(row);
                });
            }
        }
    }
}