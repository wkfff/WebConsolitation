using System.Reflection;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class XControlTests
    {
        [Test]
        public void CanCreateCustomType()
        {
            MockRepository mocks = new MockRepository();

            IPackage package = mocks.DynamicMock<IPackage>();
            IScheme scheme = mocks.DynamicMock<IScheme>();
            IEntity entity = mocks.DynamicMock<IEntity>();
            IControlFactory controlFactory = mocks.DynamicMock<IControlFactory>();
            
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new Stub.PrincipalProviderStub())
                .Repeat.Any();
            
            Expect.Call(controlFactory.CreateControl(typeof(Stub.Gui.GridViewDescendant)))
                .Return(new Stub.Gui.GridViewDescendant());

            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("f37827df-c22a-4569-9512-c0c48791d46c"))
                .Return(entity);

            mocks.ReplayAll();

            ParametersService parametersService = new ParametersService(null);
            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            ViewService service = new ViewService();
            installer.ConfigureViews(service, parametersService);

            var view = service.GetView("XControlCustomTypeTest");

            ControlBuilder.Current.SetControlFactory(controlFactory);
            XControl xControl = new XBuilderFactory(scheme, parametersService).Create(view);
            Control control = xControl.Create();

            mocks.VerifyAll();

            Assert.IsInstanceOf(typeof(Stub.Gui.GridViewDescendant), control);
        }
    }
}
