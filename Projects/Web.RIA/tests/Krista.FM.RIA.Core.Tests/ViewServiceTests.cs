using System.Collections.Generic;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class ViewServiceTests
    {
        [Test]
        public void CanAddNavigationTest()
        {
            MockRepository mocks = new MockRepository();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new Stub.PrincipalProviderStub())
                .Repeat.Any();
            mocks.ReplayAll();

            ViewService service = new ViewService();
            service.AddView("id", new View { Id = "id" });
            var view = service.GetView("id");
            
            Assert.AreEqual("id", view.Id);
        }

        [Test]
        public void CanClearNavigationTest()
        {
            MockRepository mocks = new MockRepository();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new Stub.PrincipalProviderStub())
                .Repeat.Any();
            mocks.ReplayAll();

            ViewService service = new ViewService();
            service.AddView("id", new View { Id = "id" });
            var view = service.GetView("id");

            Assert.AreEqual("id", view.Id);

            service.Clear();

            try
            {
                view = service.GetView("id");
            }
            catch(KeyNotFoundException)
            {
                view = null;
            }

            Assert.AreEqual(null, view);
        }
    }
}
