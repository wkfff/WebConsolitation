using System.IO;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Forms.Mrot
{
    [TestFixture]
    public class ExportServiceTests
    {
        private const int TaskId = 1355;

        [SetUp]
        public void SetUp()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=mostest;Initial Catalog=Moscow;Data Source=Fm5-2\\sql2005",
                "SQL",
                "5"); 
        }

        [Ignore]
        [Test]
        public void ExportUnactedRegionsTest()
        {
            var exportObj = CreateExportObj();
            var stream = exportObj.ExportUnactedRegions(TaskId);
            var docName = exportObj.GetDocumentName(TaskId, "Не поступившие данные", true);
            SaveDocument(stream, docName);
        }

        [Ignore]
        [Test]
        public void ExportFormCollectionTest()
        {
            var exportObj = CreateExportObj();
            var stream = exportObj.ExportFormCollection(1209);
            var docName = exportObj.GetDocumentName(TaskId, "Форма сбора", false);
            SaveDocument(stream, docName);
        }

        [Ignore]
        [Test]
        public void ExportExecutersTest()
        {
            var exportObj = CreateExportObj();
            var stream = exportObj.ExportExecuters(TaskId);
            var docName = exportObj.GetDocumentName(TaskId, "Должности", true);
            SaveDocument(stream, docName);
        }

        [Ignore]
        [Test]
        public void ExportSubjectTrihedrDataTest()
        {
            var exportObj = CreateExportObj();
            var stream = exportObj.ExportMOReport(TaskId);
            var docName = exportObj.GetDocumentName(TaskId, "Отчет", true);
            SaveDocument(stream, docName);
        }

        #region ServiceMethods

        private ExportService CreateExportObj()
        {
            var jobTitleRepository = new NHibernateLinqRepository<D_Report_TAJobTitle>();
            var factRepository = new NHibernateLinqRepository<F_Marks_MOFOTrihedralAgr>();
            var orgRepository = new NHibernateLinqRepository<FX_OrgType_TrihedrAlagr>();
            var markRepository = new NHibernateLinqRepository<FX_Marks_TrihedrAlagr>();
            var periodRepository = new NHibernateLinqRepository<FX_Date_YearDayUNV>();
            var datasourceRepository = new NHibernateLinqRepository<DataSources>();
            var reportRepository = new NHibernateLinqRepository<D_Report_TrihedrAgr>();
            var protocolRepository = new NHibernateLinqRepository<D_CD_Protocol>();
            var taskRepository = new NHibernateLinqRepository<D_CD_Task>();

            var jobService = new JobTitleService(jobTitleRepository);
            var protocolService = new ProtocolService(protocolRepository);

            var repository = new FormRepository(
                factRepository,
                orgRepository,
                markRepository,
                periodRepository,
                datasourceRepository,
                taskRepository,
                reportRepository);

            var exportRepository = new ExportRepository(taskRepository, factRepository);

            return new ExportService(exportRepository, repository, jobService, protocolService);
        }

        private void SaveDocument(Stream stream, string reportName)
        {
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            using (var file = File.Create(string.Format(@"c:\{0}.xls", reportName)))
            {
                file.Write(buffer, 0, buffer.Length);
            }
        }

#endregion
    }
}
