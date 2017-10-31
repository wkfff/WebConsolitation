using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump.PumpWebCons
{
    /// <summary>
    /// Баланс ГРБС, ПБС, РБС, ГАИФ, АИФ, ГАДБ, АДБ
    /// </summary>
    public sealed class F0503130PumpWebCons
    {
        public const string F0503130ClassV1 = "F_0503130of01022011";
        public const string F0503130ClassV2 = "F_0503130of01012012";
        public const string F0503130ClassV3 = "F_0503130of01012013";
        public const string F0503130ClassV4 = "F_0503130of01012014";
        public const string F0503130ClassV5 = "F_0503130of20150101";
        public const string F0503130ClassV2016 = "F_0503130of20161201";

        private readonly INewRestService newRestService;
        private readonly F_F_ParameterDoc doc;

        public F0503130PumpWebCons(INewRestService newRestService, F_F_ParameterDoc doc)
        {
            this.newRestService = newRestService;
            this.doc = doc;
        }

        public void ProcessFormData0503130V2(string data)
        {
            var xmldoc = F0503130V2.Objects.Parse(data);

            var formData = xmldoc.F_0503130of01012012.reportSections.F_0503130of01012012S1.rows.F_0503130of01012012S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503130of01012012.reportSections.F_0503130of01012012S2.rows.F_0503130of01012012S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503130.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503130.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.budgetActivityBegin = x.col3 ?? 0;
                    row.availableMeansBegin = x.col5 ?? 0;
                    row.totalBegin = x.col6 ?? 0;
                    row.budgetActivityEnd = x.col8 ?? 0;
                    row.availableMeansEnd = x.col10 ?? 0;
                    row.totalEnd = x.col11 ?? 0;
                    newRestService.Save(row);
                }
            });

            formDataInformation.Each(x =>
            {
                var row = docDataInformation.FirstOrDefault(d => d.lineCode.Equals(x.col2)) ??
                                            new F_Report_BalF0503130
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Name = PumpWebCons.GetIndicatorName(x.col1),
                                                lineCode = x.col2,
                                                Section = (int)F0503130F0503730Details.Information
                                            };

                row.totalBegin = x.col6 ?? 0;
                row.totalEnd = x.col10 ?? 0;
                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503130V3(string data)
        {
            var xmldoc = F0503130.Objects.Parse(data);

            var formData = xmldoc.F_0503130of01012013.reportSections.F_0503130of01012013S1.rows.F_0503130of01012013S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503130of01012013.reportSections.F_0503130of01012013S2.rows.F_0503130of01012013S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503130.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503130.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.budgetActivityBegin = x.col3 ?? 0;
                    row.availableMeansBegin = x.col5 ?? 0;
                    row.totalBegin = x.col6 ?? 0;
                    row.budgetActivityEnd = x.col8 ?? 0;
                    row.availableMeansEnd = x.col10 ?? 0;
                    row.totalEnd = x.col11 ?? 0;
                    newRestService.Save(row);
                }
            });

            formDataInformation.Each(x =>
            {
                var row = docDataInformation.FirstOrDefault(d => d.lineCode.Equals(x.col2)) ??
                                            new F_Report_BalF0503130
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Name = PumpWebCons.GetIndicatorName(x.col1),
                                                lineCode = x.col2,
                                                Section = (int)F0503130F0503730Details.Information
                                            };

                row.totalBegin = x.col3 ?? 0;
                row.totalEnd = x.col7 ?? 0;
                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503130V4(string data)
        {
            var xmldoc = F_0503130of01012014.Objects.Parse(data);

            var formData = xmldoc.F_0503130of01012014.reportSections.F_0503130of01012014S1.rows.F_0503130of01012014S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503130of01012014.reportSections.F_0503130of01012014S2.rows.F_0503130of01012014S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503130.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503130.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.budgetActivityBegin = x.col3 ?? 0;
                    row.availableMeansBegin = x.col5 ?? 0;
                    row.totalBegin = x.col6 ?? 0;
                    row.budgetActivityEnd = x.col8 ?? 0;
                    row.availableMeansEnd = x.col10 ?? 0;
                    row.totalEnd = x.col11 ?? 0;
                    newRestService.Save(row);
                }
            });

            formDataInformation.Each(x =>
            {
                var row = docDataInformation.FirstOrDefault(d => d.lineCode.Equals(x.col2)) ??
                                            new F_Report_BalF0503130
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Name = PumpWebCons.GetIndicatorName(x.col1),
                                                lineCode = x.col2,
                                                Section = (int)F0503130F0503730Details.Information
                                            };

                row.totalBegin = x.col3 ?? 0;
                row.totalEnd = x.col7 ?? 0;
                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503130V5(string data)
        {
            var xmldoc = F_0503130of20150101.Objects.Parse(data);

            var formData = xmldoc.F_0503130of20150101.reportSections.F_0503130of20150101S1.rows.F_0503130of20150101S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503130of20150101.reportSections.F_0503130of20150101S2.rows.F_0503130of20150101S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503130.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503130.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            var docYear = doc.RefYearForm.ID;
            var partDoc = doc.RefPartDoc.ID;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.budgetActivityBegin = x.col3 ?? 0;
                    row.availableMeansBegin = x.col5 ?? 0;
                    row.totalBegin = x.col6 ?? 0;
                    row.budgetActivityEnd = x.col8 ?? 0;
                    row.availableMeansEnd = x.col10 ?? 0;
                    row.totalEnd = x.col11 ?? 0;
                    newRestService.Save(row);
                }
            });

            formDataInformation.Each(x =>
            {
                var row = docDataInformation.FirstOrDefault(d => d.lineCode.Equals(x.col2));

                if (row == null)
                {
                    if (newRestService.GetItems<D_Marks_ItfSettings>().Any(
                        setting => setting.RefPartDoc.ID == partDoc &&
                                   setting.Section == (int)F0503130F0503730Details.Information &&
                                   setting.RefIndicators.LineCode == x.col2 &&
                                   (!setting.StartYear.HasValue || docYear >= setting.StartYear) &&
                                   (!setting.EndYear.HasValue || docYear <= setting.EndYear)))
                    {
                        row = new F_Report_BalF0503130
                        {
                            ID = 0,
                            RefParametr = doc,
                            Name = PumpWebCons.GetIndicatorName(x.col1),
                            lineCode = x.col2,
                            Section = (int)F0503130F0503730Details.Information
                        };
                    }
                    else
                    {
                        return;
                    }
                }

                row.totalBegin = x.col3 ?? 0;
                row.totalEnd = x.col7 ?? 0;
                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503130V2016(string data)
        {
            var xmldoc = F_0503130of20161201.Objects.Parse(data);

            var formData = xmldoc.F_0503130of20161201.reportSections.F_0503130of20161201S1.rows.F_0503130of20161201S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503130of20161201.reportSections.F_0503130of20161201S2.rows.F_0503130of20161201S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503130.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503130.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            var docYear = doc.RefYearForm.ID;
            var partDoc = doc.RefPartDoc.ID;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.budgetActivityBegin = x.col3 ?? 0;
                    row.availableMeansBegin = x.col5 ?? 0;
                    row.totalBegin = x.col6 ?? 0;
                    row.budgetActivityEnd = x.col8 ?? 0;
                    row.availableMeansEnd = x.col10 ?? 0;
                    row.totalEnd = x.col11 ?? 0;
                    newRestService.Save(row);
                }
            });

            formDataInformation.Each(x =>
            {
                var row = docDataInformation.FirstOrDefault(d => d.lineCode.Equals(x.col2));

                if (row == null)
                {
                    if (newRestService.GetItems<D_Marks_ItfSettings>().Any(
                        setting => setting.RefPartDoc.ID == partDoc &&
                                   setting.Section == (int)F0503130F0503730Details.Information &&
                                   setting.RefIndicators.LineCode == x.col2 &&
                                   (!setting.StartYear.HasValue || docYear >= setting.StartYear) &&
                                   (!setting.EndYear.HasValue || docYear <= setting.EndYear)))
                    {
                        row = new F_Report_BalF0503130
                        {
                            ID = 0,
                            RefParametr = doc,
                            Name = PumpWebCons.GetIndicatorName(x.col1),
                            lineCode = x.col2,
                            Section = (int)F0503130F0503730Details.Information
                        };
                    }
                    else
                    {
                        return;
                    }
                }

                row.totalBegin = x.col3 ?? 0;
                row.totalEnd = x.col7 ?? 0;
                newRestService.Save(row);
            });
        }
    }
}