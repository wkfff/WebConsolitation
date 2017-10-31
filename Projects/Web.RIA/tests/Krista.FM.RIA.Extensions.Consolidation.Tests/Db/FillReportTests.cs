using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using Krista.FM.RIA.Extensions.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Db
{
    [TestFixture]
    public class FillReportTests
    {
        [SetUp]
        public void SetUp()
        {
            NHibernateHelper.SetupNHibernate("Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV");
        }

        [Ignore]
        [Test]
        public void FillReportForTaskTest()
        {
            var formReport = new ReportForm(new NHibernateLinqRepository<D_CD_Report>(), new ReportDataRepository(), null);
            var taskRepository = new NHibernateLinqRepository<D_CD_Task>();
            formReport.CreateReport(taskRepository.Get(405));
        }
    }
}
