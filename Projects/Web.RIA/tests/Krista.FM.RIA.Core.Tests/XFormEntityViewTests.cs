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
    public class XFormEntityViewTests
    {
        [Test]
        public void XFormEntityViewTest()
        {
            MockRepository mocks = new MockRepository();

            IPackage package = mocks.DynamicMock<IPackage>();
            IScheme scheme = mocks.DynamicMock<IScheme>();
            IEntity entity = mocks.DynamicMock<IEntity>();
            IPresentationCollection presentationCollection = mocks.DynamicMock<IPresentationCollection>();
            IPresentation presentation = mocks.DynamicMock<IPresentation>();
            IControlFactory controlFactory = mocks.DynamicMock<IControlFactory>();

            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new Stub.PrincipalProviderStub())
                .Repeat.Any();

            Expect.Call(controlFactory.CreateControl(typeof(FormEntityView)))
                .Return(new FormEntityView());

            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("f37827df-c22a-4569-9512-c0c48791d46c"))
                .Return(entity);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection.ContainsKey(""))
                .IgnoreArguments()
                .Return(true);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection[""])
                .IgnoreArguments()
                .Return(presentation);

            mocks.ReplayAll();

            ParametersService parametersService = new ParametersService(null);

            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            ViewService service = new ViewService();
            installer.ConfigureViews(service, parametersService);

            var view = service.GetView("XFormEntityViewTest");

            ControlBuilder.Current.SetControlFactory(controlFactory);
            XControl xControl = new XBuilderFactory(scheme, parametersService).Create(view);
            Control control = xControl.Create();

            mocks.VerifyAll();
        }
    }
}
