using System;
using System.Security.Principal;
using System.Web;
using Krista.FM.Common;
using Krista.FM.Common.Constants;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Core.Tests.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Tests.Services
{
    public class ProgramServiceTests
    {
        private MockRepository mocks;
        private IExtension extension;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            extension = mocks.DynamicMock<IExtension>();
            Expect.Call(extension.OKTMO).Return(OKTMO.HMAO).Repeat.Any();
        }

        [Ignore]
        [Test]
        public void GetProgramsTableTest()
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

            var programRepositoryReal = new NHibernateLinqRepository<D_ExcCosts_ListPrg>();
            var actionsRepository = new NHibernateLinqRepository<D_ExcCosts_Events>();
            var npaRepository = new NHibernateLinqRepository<D_ExcCosts_NPA>();

            var npaService = new NpaService(npaRepository);
            
            var progrmaService = new ProgramService(programRepositoryReal, actionsRepository, npaService, null, null, extension);

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            var user = new Users { Name = "asd" };
            HttpContext.Current.User = new BasePrincipal(new GenericIdentity(user.Name), new[] { ProgramRoles.Viewer }, user);
            
            mocks.ReplayAll();

            var model = progrmaService.GetProgramsTable(true, true, true, false, false, true);
            Assert.NotNull(model);
            Assert.GreaterOrEqual(model.Count, 0);
            Console.WriteLine(String.Format("Выбрано записей: {0}", model.Count));
        }

        [Ignore]
        [Test]
        public void CreateProgramTest()
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
            var actionsRepository = new NHibernateLinqRepository<D_ExcCosts_Events>();
            var npaRepository = new NHibernateLinqRepository<D_ExcCosts_NPA>();
            var creatorsRepository = new NHibernateLinqRepository<D_ExcCosts_Creators>();
            var yearDayUnvRepository = new NHibernateLinqRepository<FX_Date_YearDayUNV>();
            var programTypeRepository = new NHibernateLinqRepository<FX_ExcCosts_TpPrg>();

            var npaService = new NpaService(npaRepository);
            var additionalService = new AdditionalService(creatorsRepository, yearDayUnvRepository, programTypeRepository, null, null);
            var programService = new ProgramService(programRepository, actionsRepository, npaService, null, additionalService, null);

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            var user = new Users { Name = "webreg" };
            HttpContext.Current.User = new BasePrincipal(new GenericIdentity(user.Name), new[] { ProgramRoles.Viewer }, user);
            
            mocks.ReplayAll();

            var entity = new D_ExcCosts_ListPrg
                             { 
                               ID = 0,
                               Name = "zzzzzzzzz",
                               ShortName = "short",
                               RefBegDate = additionalService.GetRefYear(2011),
                               RefEndDate = additionalService.GetRefYear(2011),
                               RefCreators = additionalService.GetCreator("webreg"),
                               RefTypeProg = additionalService.GetRefTypeProg(2),
                             };
            
            programService.SaveProject(entity, null);
            programRepository.DbContext.CommitChanges();            
            Console.WriteLine(String.Format("Создана запись с id= {0}", entity.ID));

            NHibernateSession.Current.Close();

            int id = entity.ID;
            var list = NHibernateSession.SessionFactory.OpenSession().CreateSQLQuery("select id from d_ExcCosts_ListPrg where id = :progId")
                       .AddEntity(typeof(D_ExcCosts_ListPrg))
                       .SetInt32("progId", id)
                       .List<ReportsTree>();
            Assert.IsTrue(list.Count == 1);
        }
    }
}
