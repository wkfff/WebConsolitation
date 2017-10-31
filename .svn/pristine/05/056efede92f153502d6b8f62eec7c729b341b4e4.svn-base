using System.Collections.Generic;

using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class ReportSectionDataServiceTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void GetTest()
        {
            var form = new D_CD_Templates { InternalName = "f1", FormVersion = 1 };
            var formSection = new D_Form_Part { InternalName = "s1", Code = "s1", RefForm = form };
            form.Parts.Add(formSection);
            formSection.Columns.Add(new D_Form_TableColumn { InternalName = "col1", DataType = "System.Int32", DataTypeSize = 5, RefPart = formSection });

            var report = new D_CD_Report { RefForm = form };
            var section = new D_Report_Section { ID = 1, RefReport = report, RefFormSection = formSection };
            report.Sections.Add(section);

            var repository = mocks.DynamicMock<IReportDataRepository>();
            var typeResolver = mocks.DynamicMock<IDomainTypesResolver>();

            Expect.Call(typeResolver.GetRecordType(formSection)).Return(typeof(string));
            Expect.Call(repository.FindAll(null, null)).Return(new List<IRecord>()).IgnoreArguments();

            mocks.ReplayAll();

            var service = new ReportSectionDataService(null, null, typeResolver, repository);
            service.GetAll(report, "s1");

            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ReportDataAccessException), ExpectedMessage = "Внутреннее имя раздела \"s2\" не найдено.")]
        public void GetWithoutFormSectionTest()
        {
            var form = new D_CD_Templates { Code = "f1", FormVersion = 1 };
            var formSection = new D_Form_Part { Code = "s1", RefForm = form };
            form.Parts.Add(formSection);
            var report = new D_CD_Report { RefForm = form };

            var service = new ReportSectionDataService(null, null, null, null);
            service.GetAll(report, "s2");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ReportDataAccessException), ExpectedMessage = "Раздел данных отчета \"s2\" не найден.")]
        public void GetWithoutReportSectionTest()
        {
            var form = new D_CD_Templates { InternalName = "f1", FormVersion = 1 };
            var formSection = new D_Form_Part { InternalName = "s2", Code = "s2", Name = "Раздел 2", RefForm = form };
            form.Parts.Add(formSection);
            formSection.Columns.Add(new D_Form_TableColumn { InternalName = "col1", DataType = "System.Int32", DataTypeSize = 5, RefPart = formSection });

            var report = new D_CD_Report { RefForm = form };

            var service = new ReportSectionDataService(null, null, null, null);
            service.GetAll(report, "s2");
        }
    }
}
