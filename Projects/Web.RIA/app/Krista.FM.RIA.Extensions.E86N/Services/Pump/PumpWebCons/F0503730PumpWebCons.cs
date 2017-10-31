using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump.PumpWebCons
{
    /// <summary>
    /// Баланс государственного (муниципального) учреждения
    /// </summary>
    public sealed class F0503730PumpWebCons
    {
        public const string F0503730V1 = "F_0503730of01012011";
        public const string F0503730V2 = "F_0503730of01012012";
        public const string F0503730V3 = "F_0503730of01012013";
        public const string F0503730V4 = "F_0503730of01062014";
        public const string F0503730V5 = "F_0503730of20150101";
        public const string F0503730V2016 = "F_0503730of20161201";

        private readonly INewRestService newRestService;
        private readonly F_F_ParameterDoc doc;

        public F0503730PumpWebCons(INewRestService newRestService, F_F_ParameterDoc doc)
        {
            this.newRestService = newRestService;
            this.doc = doc;
        }

        public void ProcessFormData0503730V2(string data)
        {
            var xmldoc = F0503730of2012.Objects.Parse(data);

            var formData = xmldoc.F_0503730of01012012.reportSections.F_0503730of01012012S1.rows.F_0503730of01012012S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503730of01012012.reportSections.F_0503730of01012012S2.rows.F_0503730of01012012S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503730.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503730.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.targetFundsBegin = x.col3 ?? 0;
                    row.servicesBegin = x.col4 ?? 0;
                    row.temporaryFundsBegin = x.col5 ?? 0;
                    row.totalStartYear = x.col6 ?? 0;
                    row.targetFundsEnd = x.col8 ?? 0;
                    row.servicesEnd = x.col9 ?? 0;
                    row.temporaryFundsEnd = x.col10 ?? 0;
                    row.totalEndYear = x.col11 ?? 0;

                    newRestService.Save(row);
                }
            });

            formDataInformation.Each(x =>
            {
                var row = docDataInformation.FirstOrDefault(d => d.lineCode.Equals(x.col2)) ??
                                            new F_Report_BalF0503730
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Name = PumpWebCons.GetIndicatorName(x.col1),
                                                lineCode = x.col2,
                                                Section = (int)F0503130F0503730Details.Information
                                            };

                row.targetFundsBegin = x.col4 ?? 0;
                row.servicesBegin = x.col5 ?? 0;
                row.totalStartYear = x.col6 ?? 0;
                row.targetFundsEnd = x.col8 ?? 0;
                row.servicesEnd = x.col9 ?? 0;
                row.totalEndYear = x.col10 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503730V3(string data)
        {
            var xmldoc = F0503730.Objects.Parse(data);

            var formData = xmldoc.F_0503730of01012013.reportSections.F_0503730of01012013S1.rows.F_0503730of01012013S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503730of01012013.reportSections.F_0503730of01012013S2.rows.F_0503730of01012013S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503730.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503730.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.targetFundsBegin = x.col3 ?? 0;
                    row.servicesBegin = x.col4 ?? 0;
                    row.temporaryFundsBegin = x.col5 ?? 0;
                    row.totalStartYear = x.col6 ?? 0;
                    row.targetFundsEnd = x.col8 ?? 0;
                    row.servicesEnd = x.col9 ?? 0;
                    row.temporaryFundsEnd = x.col10 ?? 0;
                    row.totalEndYear = x.col11 ?? 0;

                    newRestService.Save(row);
                }
            });

            formDataInformation.Each(x =>
            {
                var row = docDataInformation.FirstOrDefault(d => d.lineCode.Equals(x.col2)) ??
                                            new F_Report_BalF0503730
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Name = PumpWebCons.GetIndicatorName(x.col1),
                                                lineCode = x.col2,
                                                Section = (int)F0503130F0503730Details.Information
                                            };

                row.targetFundsBegin = x.col4 ?? 0;
                row.servicesBegin = x.col5 ?? 0;
                row.totalStartYear = x.col6 ?? 0;
                row.targetFundsEnd = x.col8 ?? 0;
                row.servicesEnd = x.col9 ?? 0;
                row.totalEndYear = x.col10 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503730V4(string data)
        {
            var xmldoc = F_0503730of01062014.Objects.Parse(data);

            var formData = xmldoc.F_0503730of01062014.reportSections.F_0503730of01062014S1.rows.F_0503730of01062014S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503730of01062014.reportSections.F_0503730of01062014S2.rows.F_0503730of01062014S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503730.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503730.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.targetFundsBegin = x.col3 ?? 0;
                    row.servicesBegin = x.col4 ?? 0;
                    row.temporaryFundsBegin = x.col5 ?? 0;
                    row.totalStartYear = x.col6 ?? 0;
                    row.targetFundsEnd = x.col8 ?? 0;
                    row.servicesEnd = x.col9 ?? 0;
                    row.temporaryFundsEnd = x.col10 ?? 0;
                    row.totalEndYear = x.col11 ?? 0;

                    newRestService.Save(row);
                }
            });

            formDataInformation.Each(x =>
            {
                var row = docDataInformation.FirstOrDefault(d => d.lineCode.Equals(x.col2)) ??
                                            new F_Report_BalF0503730
                                            {
                                                ID = 0,
                                                RefParametr = doc,
                                                Name = PumpWebCons.GetIndicatorName(x.col1),
                                                lineCode = x.col2,
                                                Section = (int)F0503130F0503730Details.Information
                                            };

                row.targetFundsBegin = x.col4 ?? 0;
                row.servicesBegin = x.col5 ?? 0;
                row.totalStartYear = x.col6 ?? 0;
                row.targetFundsEnd = x.col8 ?? 0;
                row.servicesEnd = x.col9 ?? 0;
                row.totalEndYear = x.col10 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503730V5(string data)
        {
            var xmldoc = F_0503730of20150101.Objects.Parse(data);

            var formData = xmldoc.F_0503730of20150101.reportSections.F_0503730of20150101S1.rows.F_0503730of20150101S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503730of20150101.reportSections.F_0503730of20150101S2.rows.F_0503730of20150101S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503730.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503730.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            var docYear = doc.RefYearForm.ID;
            var partDoc = doc.RefPartDoc.ID;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.targetFundsBegin = x.col12 ?? 0;
                    row.targetFundsEnd = x.col15 ?? 0;

                    row.totalStartYear = x.col6 ?? 0;
                    row.totalEndYear = x.col11 ?? 0;

                    row.StateTaskFundStartYear = x.col13 ?? 0;
                    row.StateTaskFundEndYear = x.col16 ?? 0;

                    row.RevenueFundsStartYear = x.col14 ?? 0;
                    row.RevenueFundsEndYear = x.col17 ?? 0;

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
                        row = new F_Report_BalF0503730
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

                row.targetFundsBegin = x.col11 ?? 0;
                row.targetFundsEnd = x.col14 ?? 0;

                row.totalStartYear = x.col6 ?? 0;
                row.totalEndYear = x.col10 ?? 0;

                row.StateTaskFundStartYear = x.col12 ?? 0;
                row.StateTaskFundEndYear = x.col15 ?? 0;

                row.RevenueFundsStartYear = x.col13 ?? 0;
                row.RevenueFundsEndYear = x.col16 ?? 0;

                newRestService.Save(row);
            });
        }

        public void ProcessFormData0503730V6(string data)
        {
            var xmldoc = F_0503730of20161201.Objects.Parse(data);

            var formData = xmldoc.F_0503730of20161201.reportSections.F_0503730of20161201S1.rows.F_0503730of20161201S1Row.Where(x => x.col1.IsNotNullOrEmpty());
            var formDataInformation = xmldoc.F_0503730of20161201.reportSections.F_0503730of20161201S2.rows.F_0503730of20161201S2Row.Where(x => x.col2.IsNotNullOrEmpty());

            var docData = doc.AnnualBalanceF0503730.Where(x => x.Section != (int)F0503130F0503730Details.Information);
            var docDataInformation = doc.AnnualBalanceF0503730.Where(x => x.Section == (int)F0503130F0503730Details.Information);

            var docYear = doc.RefYearForm.ID;
            var partDoc = doc.RefPartDoc.ID;

            formData.Each(x =>
            {
                var row = docData.FirstOrDefault(d => d.lineCode.Equals(x.col1));
                if (row != null)
                {
                    row.targetFundsBegin = x.col12.GetValueOrDefault();
                    row.targetFundsEnd = x.col15.GetValueOrDefault();

                    row.totalStartYear = x.col6.GetValueOrDefault();
                    row.totalEndYear = x.col11.GetValueOrDefault();

                    row.StateTaskFundStartYear = x.col13.GetValueOrDefault();
                    row.StateTaskFundEndYear = x.col16.GetValueOrDefault();

                    row.RevenueFundsStartYear = x.col14.GetValueOrDefault();
                    row.RevenueFundsEndYear = x.col17.GetValueOrDefault();

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
                        row = new F_Report_BalF0503730
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

                row.targetFundsBegin = x.col11.GetValueOrDefault();
                row.targetFundsEnd = x.col14.GetValueOrDefault();

                row.totalStartYear = x.col6.GetValueOrDefault();
                row.totalEndYear = x.col10.GetValueOrDefault();

                row.StateTaskFundStartYear = x.col12.GetValueOrDefault();
                row.StateTaskFundEndYear = x.col15.GetValueOrDefault();

                row.RevenueFundsStartYear = x.col13.GetValueOrDefault();
                row.RevenueFundsEndYear = x.col16.GetValueOrDefault();

                newRestService.Save(row);
            });
        }
    }
}
