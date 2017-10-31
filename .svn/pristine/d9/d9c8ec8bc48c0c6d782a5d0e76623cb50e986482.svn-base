using System.Collections.Generic;
using Ext.Net;

using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class CalculationDataProviderTests
    {
        private MockRepository mocks;
        private IReportSectionDataService dataService;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            dataService = mocks.DynamicMock<IReportSectionDataService>();
        }

        [Test]
        public void Test()
        {
            var form = new D_CD_Templates();
            var formSection = new D_Form_Part { Code = "Main" };
            form.Parts.Add(formSection);
            var report = new D_CD_Report { RefForm = form };
            var data = new List<IRecord>
            {
                new ReportDataRecord(new DatabaseVersions { ID = 1, Name = "Name1" }),
                new ReportDataRecord(new DatabaseVersions { ID = 2, Name = "Name1" }),
                new ReportDataRecord(new DatabaseVersions { ID = 3, Name = "Name1" })
            };

            Expect.Call(dataService.GetAll(report, "Main")).Return(data);
            Expect.Call(() => dataService.Save(null, null, new List<IRecord>()))
                .IgnoreArguments();

            mocks.ReplayAll();

            var dataProvader = new CalculationPrimaryDataProvider(dataService);
            dataProvader.SetLeftContext(report, "Main");
            var rows = dataProvader.GetSectionRows();
            rows[0].Set("Name", "Test");
            dataProvader.Save();

            mocks.VerifyAll();
        }
    }
}
