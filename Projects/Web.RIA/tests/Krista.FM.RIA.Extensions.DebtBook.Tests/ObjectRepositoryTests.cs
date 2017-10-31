using System;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.DebtBook.Services.DAL;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class ObjectRepositoryTests
    {
        [Test]
        [Ignore]
        public void ObjectRepositoryTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
                "Oracle",
                "10");
            //NHibernateSession.InitializeNHibernateSession(
            //    new SimpleSessionStorage(),
            //    @"Data Source=FM5-2\SQL2005;User ID=Moscow;Initial Catalog=Moscow;Password=dv;Persist Security Info=True",
            //    "SQL",
            //    "9");
            
            IObjectRepository repository = new ObjectRepository();
            int refTerritory = 1282;            
            string serverFilter = String.Format("(RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {0}))) and (ParentID is null)", refTerritory);
            int? refVariant = 13;
            int? sourceId = 73;
            var result = repository.GetRows("f_S_SchBGuarantissued", serverFilter, refVariant, sourceId);
            Console.WriteLine("Число записей: " + result.Count);
        }
    }
}
