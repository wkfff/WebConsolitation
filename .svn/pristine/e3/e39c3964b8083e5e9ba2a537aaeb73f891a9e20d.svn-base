using System;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump.PumpWebCons
{
    /// <summary>
    /// Отчет о финансовых результатах деятельности учреждения
    /// </summary>
    public sealed class F0503721PumpWebCons
    {
        public const string F0503721V1 = "F_0503721of";
        public const string F0503721V2 = "F_0503721of01062014";
        public const string F0503721V3 = "F_0503721of20150101";

        private readonly INewRestService newRestService;
        private readonly F_F_ParameterDoc doc;

        public F0503721PumpWebCons(INewRestService newRestService, F_F_ParameterDoc doc)
        {
            this.newRestService = newRestService;
            this.doc = doc;
        }

        public void ProcessFormData0503721V1(string data)
        {
            var xmldoc = F0503721.Objects.Parse(data);

            var formData = xmldoc.F_0503721of.reportSections.F_0503721ofS1.rows.F_0503721ofS1Row.Where(x => x.col1.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503721;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.Section == GetF0503721DetailsByLineCode(x.col1) && d.lineCode.Equals(x.col1)) ??
                                            new F_Report_BalF0503721
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = GetF0503721DetailsByLineCode(x.col1)
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.analyticCode = x.col2;
                row.targetFunds = x.col3 ?? 0;
                row.services = x.col4 ?? 0;
                row.temporaryFunds = x.col5 ?? 0;
                row.total = x.col6 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503721V2(string data)
        {
            var xmldoc = F_0503721of01062014.Objects.Parse(data);

            var formData = xmldoc.F_0503721of01062014.reportSections.F_0503721of01062014S1.rows.F_0503721of01062014S1Row.Where(x => x.col1.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503721;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.Section == GetF0503721DetailsByLineCode(x.col1) && d.lineCode.Equals(x.col1)) ??
                                            new F_Report_BalF0503721
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = GetF0503721DetailsByLineCode(x.col1)
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.analyticCode = x.col2;
                row.targetFunds = x.col3 ?? 0;
                row.services = x.col4 ?? 0;
                row.temporaryFunds = x.col5 ?? 0;
                row.total = x.col6 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503721V3(string data)
        {
            var xmldoc = F_0503721of20150101.Objects.Parse(data);

            var formData = xmldoc.F_0503721of20150101.reportSections.F_0503721of20150101S1.rows.F_0503721of20150101S1Row.Where(x => x.col1.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503721;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.Section == GetF0503721DetailsByLineCode(x.col1) && d.lineCode.Equals(x.col1)) ??
                                            new F_Report_BalF0503721
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = GetF0503721DetailsByLineCode(x.col1)
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.analyticCode = x.col2;
                row.total = x.col6 ?? 0;
                row.targetFunds = x.col7 ?? 0;
                row.StateTaskFunds = x.col8 ?? 0;
                row.RevenueFunds = x.col9 ?? 0;

                newRestService.Save(row);
            });
        }

        private int GetF0503721DetailsByLineCode(string lineCode)
        {
            int intLineCode = Convert.ToInt32(lineCode.Substring(0, 2));

            if (intLineCode <= 11)
            {
                return (int)F0503721Details.Incomes;
            }

            if (intLineCode <= 30)
            {
                return (int)F0503721Details.Expenses;
            }

            if (intLineCode <= 37)
            {
                return (int)F0503721Details.NonFinancialAssets;
            }

            if (intLineCode <= 54)
            {
                return (int)F0503721Details.FinancialAssetsLiabilities;
            }

            return 0;
        }
    }
}