using System;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump.PumpWebCons
{
    /// <summary>
    /// Отчет о финансовых результатах деятельности
    /// </summary>
    public sealed class F0503121PumpWebCons
    {
        public const string F0503121ClassV1 = "F_0503121of01022011";
        public const string F0503121ClassV2 = "F_0503121of01022013";
        public const string F0503121ClassV3 = "F_0503121of20140101";

        private readonly INewRestService newRestService;
        private readonly F_F_ParameterDoc doc;

        public F0503121PumpWebCons(INewRestService newRestService, F_F_ParameterDoc doc)
        {
            this.newRestService = newRestService;
            this.doc = doc;
        }

        public void ProcessFormData0503121V1(string data)
        {
            var xmldoc = F0503121V1.Objects.Parse(data);

            var formData = xmldoc.F_0503121of01022011.reportSections.F_0503121of01022011S1.rows.F_0503121of01022011S1Row.Where(x => x.col1.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503121;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.Section == GetF0503121DetailsByLineCode(x.col1) && d.lineCode.Equals(x.col1)) ??
                                            new F_Report_Bal0503121
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = GetF0503121DetailsByLineCode(x.col1)
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.RefKosgy = newRestService.GetItems<D_KOSGY_KOSGY>().FirstOrDefault(k => k.Code.Equals(x.col2));
                row.budgetActivity = x.col3 ?? 0;
                row.availableMeans = x.col5 ?? 0;
                row.total = x.col6 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503121V2(string data)
        {
            var xmldoc = F0503121.Objects.Parse(data);

            var formData = xmldoc.F_0503121of01022013.reportSections.F_0503121of01022013S1.rows.F_0503121of01022013S1Row.Where(x => x.col1.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503121;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.Section == GetF0503121DetailsByLineCode(x.col1) && d.lineCode.Equals(x.col1)) ??
                                            new F_Report_Bal0503121
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = GetF0503121DetailsByLineCode(x.col1)
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.RefKosgy = newRestService.GetItems<D_KOSGY_KOSGY>().FirstOrDefault(k => k.Code.Equals(x.col2));
                row.budgetActivity = x.col3 ?? 0;
                row.availableMeans = x.col5 ?? 0;
                row.total = x.col6 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503121V3(string data)
        {
            var xmldoc = F_0503121of20140101.Objects.Parse(data);

            var formData = xmldoc.F_0503121of20140101.reportSections.F_0503121of20140101S1.rows.F_0503121of20140101S1Row.Where(x => x.col1.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503121;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.Section == GetF0503121DetailsByLineCode(x.col1) && d.lineCode.Equals(x.col1)) ??
                                            new F_Report_Bal0503121
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Section = GetF0503121DetailsByLineCode(x.col1)
                                            };

                row.Name = PumpWebCons.GetIndicatorName(x.col0);
                row.lineCode = x.col1;
                row.RefKosgy = newRestService.GetItems<D_KOSGY_KOSGY>().FirstOrDefault(k => k.Code.Equals(x.col2));
                row.budgetActivity = x.col3 ?? 0;
                row.availableMeans = x.col5 ?? 0;
                row.total = x.col6 ?? 0;

                newRestService.Save(row);
            });
        }

        private int GetF0503121DetailsByLineCode(string lineCode)
        {
            var intLineCode = Convert.ToInt32(lineCode);

            if (intLineCode <= 110)
            {
                return (int)F0503121Details.Incomes;
            }

            if (intLineCode <= 280)
            {
                return (int)F0503121Details.Expenses;
            }

            if (intLineCode <= 292)
            {
                return (int)F0503121Details.OperatingResult;
            }

            if (intLineCode <= 372)
            {
                return (int)F0503121Details.OperationNonfinancialAssets;
            }

            if (intLineCode <= 542)
            {
                return (int)F0503121Details.OperationFinancialAssets;
            }

            return 0;
        }
    }
}