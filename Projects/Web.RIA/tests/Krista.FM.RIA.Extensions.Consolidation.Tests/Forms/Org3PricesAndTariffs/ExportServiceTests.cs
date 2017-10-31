using System.IO;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Forms.Org3PricesAndTariffs
{
    [TestFixture]
    public class ExportServiceTests
    {
        [Test]
        [Ignore]
        public void GasolineExportServiceTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            ////NHibernateSession.InitializeNHibernateSession(
            ////    new SimpleSessionStorage(),
            ////    "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
            ////    "Oracle",
            ////    "10");
            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                @"Data Source=fm5-5\sql2008std;User ID=yanaoacr;Initial Catalog=yanaoacr;Password=dv;Persist Security Info=True",
                "SQL",
                "9");

            ILinqRepository<D_CD_Task> taskRepository = new NHibernateLinqRepository<D_CD_Task>();
            ILinqRepository<T_Org_CPrice> factRepository = new NHibernateLinqRepository<T_Org_CPrice>();
            
            var exportService = new Consolidation.Forms.Org3PricesAndTariffs.Gasoline.ExportService(factRepository, taskRepository);

            var result = exportService.GetExcelReport(10);
            
            SaveDocument(result, @"d:\report.xls");
        }

        [Test]
        [Ignore]
        public void FoodExportServiceTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            ////NHibernateSession.InitializeNHibernateSession(
            ////    new SimpleSessionStorage(),
            ////    "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
            ////    "Oracle",
            ////    "10");
            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                @"Data Source=fm5-5\sql2008std;User ID=yanaoacr;Initial Catalog=yanaoacr;Password=dv;Persist Security Info=True",
                "SQL",
                "9");

            ILinqRepository<D_CD_Task> taskRepository = new NHibernateLinqRepository<D_CD_Task>();
            ILinqRepository<T_Org_CPrice> factRepository = new NHibernateLinqRepository<T_Org_CPrice>();

            var exportService = new Consolidation.Forms.Org3PricesAndTariffs.Food.ExportService(factRepository, taskRepository);

            var result = exportService.GetExcelReport(11);

            SaveDocument(result, @"d:\report.xls");
        }

        private void SaveDocument(Stream stream, string fileFullName)
        {
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            using (var file = File.Create(fileFullName))
            {
                file.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
