using System;
using System.Reflection;
using System.Web.Routing;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Core.Tests.Stub;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class ExtensionInstallerBaseTests
    {
        [Test]
        public void CanInstallRoutesTest()
        {
            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            var routes = new RouteCollection();
            installer.InstallRoutes(routes);

            Assert.AreEqual(2, routes.Count);
        }

        [Test]
        public void CanRegisterTypesTest()
        {
            MockRepository mocks = new MockRepository();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();

            Expect.Call(container.RegisterType(null, null, null, null))
                .IgnoreArguments()
                .Return(container);

            Expect.Call(container.RegisterType(null, null, null, null))
                .IgnoreArguments()
                .Return(container);

            mocks.ReplayAll();

            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            installer.RegisterTypes(container);

            mocks.VerifyAll();
        }

        [Test]
        public void CanConfigureNavigationTest()
        {
            MockRepository mocks = new MockRepository();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            Expect.Call(container.Resolve<ParameterValueProvider>())
                .IgnoreArguments()
                .Return(new ParameterValueProvider());

            mocks.ReplayAll();

            ParametersService parametersService = new ParametersService(container);

            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            installer.RegisterParameters(parametersService);
            var navigation = installer.ConfigureNavigation(parametersService);

            mocks.VerifyAll();

            Assert.NotNull(navigation);
            Assert.AreEqual(5, navigation.Items.Count);
            Assert.AreEqual(3, navigation.Items[4].Items.Count);
            Assert.AreEqual(1, navigation.Items[0].Params.Count);
            Assert.AreEqual(4, navigation.Commands.Count);
            Assert.AreEqual("cmddbeTransfert", navigation.Commands[0].Id);
            Assert.AreEqual("Перенос данных из источников финансирования", navigation.Commands[0].Title);
        }

        [Test]
        public void ExtensionConfigParametersServiceTest()
        {
            MockRepository mocks = new MockRepository();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            Expect.Call(container.Resolve<ParameterValueProvider>())
                .IgnoreArguments()
                .Return(new ParameterValueProvider());

            mocks.ReplayAll();

            ParametersService service = new ParametersService(container);

            service.RegisterExtensionConfigParameters(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Krista.FM.RIA.Core.Tests.Data.extension.config.xml"));


            Assert.AreEqual("Subject", service.GetParameterValue("UserRegionType"));

            mocks.VerifyAll();
        }

        [Test]
        public void ConfigureViewsTest()
        {
            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            MockRepository mocks = new MockRepository();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new PrincipalProviderStub())
                .Repeat.Any();
            mocks.ReplayAll();

            ViewService service = new ViewService();
            installer.ConfigureViews(service, new ParametersService(null));

            View view = service.GetView("Capital");
            Assert.AreEqual(typeof(XTabbedView), view.Type);
            Assert.AreEqual("ДК Ценные бумаги", view.Title);
            service.GetView("CapitalRFForm");
        }

        [Test]
        public void ViewBuilderFactoryTest()
        {
            MockRepository mocks = new MockRepository();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new PrincipalProviderStub())
                .Repeat.Any();
            mocks.ReplayAll();

            var view = new View {Id = "id", Title = "title", Type = typeof (XTabbedView)};
            object obj = new XBuilderFactory(null, null).Create(view);
            Assert.AreEqual(typeof(XTabbedView), obj.GetType());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), UserMessage = "Не найден ресурс config1.xml.")]
        public void MustThrowExceptionOnNotExistingResourceTest()
        {
            new ExtensionInstallerBase(Assembly.GetExecutingAssembly(), "config1.xml")
                .RegisterParameters(null);
        }

        [Test]
        public void CanLoadEmptyConfigTest()
        {
            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.empty.extension.config.xml");
            var parametersService = new ParametersService(null);
            installer.ConfigureNavigation(parametersService);
            installer.ConfigureViews(new ViewService(), parametersService);
            installer.ConfigureWindows(new WindowService());
            installer.ConfigureClientExtension(new ClientExtensionService());
            installer.InstallRoutes(new RouteCollection());
            installer.RegisterTypes(null);
        }
    }
}
