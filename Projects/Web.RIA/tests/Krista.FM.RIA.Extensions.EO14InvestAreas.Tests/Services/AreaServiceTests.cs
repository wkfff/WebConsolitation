using System;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Models;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Servises;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Tests.Services
{
    [TestFixture]
    public class DummyTests
    {
        private MockRepository mocks;
        private IUserCredentials userCredentials;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Ignore]
        [Test]
        public void GetAreasTableTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV",
                "Oracle",
                "10");

            var areaRepositoryReal = new NHibernateLinqRepository<D_InvArea_Reestr>();
            var areaService = new AreaService(areaRepositoryReal, null, null, null);

            userCredentials = mocks.DynamicMock<IUserCredentials>();
            var user = new Users { Name = "asd" };

            Expect.Call(userCredentials.IsInRole(InvAreaRoles.Creator)).Return(false).Repeat.Any();
            Expect.Call(userCredentials.IsInRole(InvAreaRoles.Coordinator)).Return(true).Repeat.Any();
            Expect.Call(userCredentials.User).Return(user).Repeat.Any();
            mocks.ReplayAll();

            var model = areaService.GetAreasTable(true, true, true, userCredentials);
            Assert.NotNull(model);
            Assert.GreaterOrEqual(model.Count, 0);
            Console.WriteLine(String.Format("Выбрано записей: {0}", model.Count));

            model = areaService.GetAreasTable(false, false, false, userCredentials);
            Assert.NotNull(model);
            Assert.IsTrue(model.Count == 0);
        }
    }
}
