using System;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Tests.Services
{
    public class FinanceServiceTests
    {
        private MockRepository mocks;
        /*private IUserCredentials userCredentials;*/

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Ignore]
        [Test]
        public void GetReportTableTest()
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
                @"Data Source=fmserv\mssql2005;User ID=dv;Initial Catalog=DV;Password=dv;Persist Security Info=True",
                "SQL",
                "9");

            var programRepository = new NHibernateLinqRepository<D_ExcCosts_ListPrg>();
            var progrmaService = new ProgramService(programRepository, null, null, null, null, null);

            int programId = 59;
            var program = progrmaService.GetProgram(programId);
            
            Assert.IsNotNull(program);
            
            var financeRepository = new NHibernateLinqRepository<D_ExcCosts_Finances>();
            var factRepository = new NHibernateLinqRepository<F_ExcCosts_Finance>();
            var creatorRepository = new NHibernateLinqRepository<D_ExcCosts_Creators>();
            var yearDayUnvRepository = new NHibernateLinqRepository<FX_Date_YearDayUNV>();
            var datasourceRepository = new NHibernateLinqRepository<DataSources>();
            
            var additionalService = new AdditionalService(creatorRepository, yearDayUnvRepository, null, null, null);
            var datasourceService = new DatasourceService(null, datasourceRepository);

            var financeService = new FinanceService(financeRepository, factRepository, additionalService, datasourceService);

            var data = financeService.GetReportTable(program, 2011);
            
            Assert.NotNull(data);
            
            Console.WriteLine(String.Format("Выбрано записей: {0}", data.Rows.Count));
        }
    }
}
