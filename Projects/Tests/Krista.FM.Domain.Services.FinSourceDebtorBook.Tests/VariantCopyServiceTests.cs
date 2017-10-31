using System;
using System.Linq;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Tests
{
    [TestFixture]
    public class VariantCopyServiceTests
    {
        [Test]
        [Ignore]
        public void CanCopyTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            var clientSession = ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=dv;Data Source=VOLOGDA", "Oracle", "9");
            //NHibernateSession.InitializeNHibernateSession("Password=dv;Persist Security Info=True;User ID=yaroslavl;Initial Catalog=Yaroslavl;Data Source=fm4-3\\sql2008", "SQL", "10");

            //var repository = new NHibernateLinqRepository<F_S_SchBCreditincome>();
            //var queryable = repository.FindAll();
            //var regions = new NHibernateLinqRepository<D_Regions_Analysis>().FindAll();


            VariantCopyService service = new VariantCopyService(new RegionsAccordanceService(null, new DataSourceService(null)));
            //service.Copy(86, 463);
            service.CopyVariantData(87, 463, 4269, true, true);
        }

        [Test]
        [Ignore]
        public void TitleReportCopyTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
                "Oracle",
                "10");
            
            VariantCopyService service = new VariantCopyService(null);
            ////service.MergeTitleRecordRegionToNewYear(2011, 2012);
            Assert.IsTrue(true);
        }
    }
}
