using System;
using System.Linq;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Tests.Db
{
    [TestFixture]
    public class DbAuditTests
    {
        private const string UserName = "nunit";
        private const string SessionId = "nunit-session-id";

        [SetUp]
        public void SetUp()
        {
            log4net.Config.XmlConfigurator.Configure();

            LogicalCallContextData.SetAuthorization(UserName);
            LogicalCallContextData.GetContext()["SessionID"] = SessionId;
            ClientSession.CreateSession(SessionClientType.Server);
        }

        [Ignore]
        [Test]
        public void DbMSSqlTest()
        {
            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                @"Data Source=fmserv\mssql2005;User ID=dv;Initial Catalog=DV;Password=dv;Persist Security Info=True",
                "SQL",
                "9");

            NHibernateRepository<FX_Date_YearDayUNV> repository1 = new NHibernateRepository<FX_Date_YearDayUNV>();
            NHibernateRepository<FX_Date_YearDay> repository2 = new NHibernateRepository<FX_Date_YearDay>();

            var row1 = repository1.Get(20000101);
            var row2 = repository2.Get(20000101);

            var queryText = "select 1, " +
             "RTRIM(cast(substring(cast(CONTEXT_INFO() as varbinary(128)), 2, 24) as varchar(24))) as SessionID, " +
              "RTRIM(cast(substring(cast(CONTEXT_INFO() as varbinary(128)), 26, 64) as varchar(64))) as UserName";
            var query = NHibernateSession.Current.CreateSQLQuery(queryText);
            var list = query.List();

            Assert.IsTrue((string)((object[])list[0])[1] == SessionId);
            Assert.IsTrue((string)((object[])list[0])[2] == UserName);
        }

        [Ignore]
        [Test]
        public void DbOracleTest()
        {
            NHibernateSession.InitializeNHibernateSession(
               new SimpleSessionStorage(),
               "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
               "Oracle",
               "10");
            
            string testClientInfoString = "my_client_info";

            var queryText = String.Format("call dbms_application_info.set_client_info('{0}')", testClientInfoString);
            var query = NHibernateSession.Current.CreateSQLQuery(queryText);
            var list = query.List();

            NHibernateRepository<FX_Date_YearDayUNV> repository1 = new NHibernateRepository<FX_Date_YearDayUNV>();
            NHibernateRepository<FX_Date_YearDay> repository2 = new NHibernateRepository<FX_Date_YearDay>();
            var row1 = repository1.Get(20000101);
            var row2 = repository2.Get(20000101);

            queryText = "select S.sid, S.username, S.process, S.client_info, S.logon_time from v$session S where S.audsid = USERENV('SESSIONID')";
            query = NHibernateSession.Current.CreateSQLQuery(queryText);
            list = query.List();
            Assert.IsTrue((string)((object[])list[0])[3] == testClientInfoString);

            queryText = "SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') FROM dual";
            query = NHibernateSession.Current.CreateSQLQuery(queryText);
            list = query.List();

            Assert.IsTrue((string)((object[])list[0])[0] == UserName);
            Assert.IsTrue((string)((object[])list[0])[1] == SessionId);
        }
    }
}

