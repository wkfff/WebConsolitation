using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
    public class XTabbedViewTests
    {
        [Test]
        public void XTabbedViewTest()
        {
            MockRepository mocks = new MockRepository();

            IPackage package = mocks.DynamicMock<IPackage>();
            IScheme scheme = mocks.DynamicMock<IScheme>();
            IEntity entity = mocks.DynamicMock<IEntity>();
            IPresentationCollection presentationCollection = mocks.DynamicMock<IPresentationCollection>();
            IControlFactory controlFactory = mocks.DynamicMock<IControlFactory>();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new Stub.PrincipalProviderStub())
                .Repeat.Any();

            Expect.Call(container.Resolve(typeof (Stub.ViewServiceStub)))
                .Return(new Stub.ViewServiceStub())
                .Repeat.Any();

            Expect.Call(controlFactory.CreateControl(typeof(TabbedView)))
                .Return(new TabbedView());
            Expect.Call(controlFactory.CreateControl(typeof(GridView)))
                .Return(new GridView())
                .Repeat.Any();
            
            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("328a93cf-9769-4980-97e3-32570636b125"))
                .Return(entity);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection.ContainsKey(""))
                .IgnoreArguments()
                .Return(false);

            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("328a93cf-9769-4980-97e3-32570636b125"))
                .Return(entity);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection.ContainsKey(""))
                .IgnoreArguments()
                .Return(false);

            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("328a93cf-9769-4980-97e3-32570636b125"))
                .Return(entity);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection.ContainsKey(""))
                .IgnoreArguments()
                .Return(false);

            mocks.ReplayAll();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            ParametersService parametersService = new ParametersService(null);

            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            installer.RegisterParameters(parametersService);

            ViewService service = new ViewService();
            installer.ConfigureViews(service, parametersService);

            var view = service.GetView("Capital");

            ControlBuilder.Current.SetControlFactory(controlFactory);
            XControl xControl = new XBuilderFactory(scheme, parametersService).Create(view);
            Control control = xControl.Create();

            mocks.VerifyAll();
        }
    }
}
