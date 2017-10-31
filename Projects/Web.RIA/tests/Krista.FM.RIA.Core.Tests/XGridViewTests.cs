using System.Reflection;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Core.Tests.Stub;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class XGridViewTests
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();

            IControlFactory controlFactory = mocks.DynamicMock<IControlFactory>();

            Expect.Call(controlFactory.CreateControl(typeof(GridView)))
                .Return(new GridView());

            ControlBuilder.Current.SetControlFactory(controlFactory);
        }

        [Test]
        public void XGridViewTest()
        {
            IPackage package = mocks.DynamicMock<IPackage>();
            IScheme scheme = mocks.DynamicMock<IScheme>();
            IEntity entity = mocks.DynamicMock<IEntity>();
            IPresentationCollection presentationCollection = mocks.DynamicMock<IPresentationCollection>();
            IPresentation presentation = mocks.DynamicMock<IPresentation>();
            
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new Stub.PrincipalProviderStub())
                .Repeat.Any();

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

            var view = service.GetView("EntityPresentationForm");

            XControl xControl = new XBuilderFactory(scheme, parametersService).Create(view);
            Control control = xControl.Create();

            mocks.VerifyAll();
        }

        [Test]
        public void XGridViewWithoutPresentationTest()
        {
            IPackage package = mocks.DynamicMock<IPackage>();
            IScheme scheme = mocks.DynamicMock<IScheme>();
            IEntity entity = mocks.DynamicMock<IEntity>();
            //IPresentationCollection presentationCollection = mocks.DynamicMock<IPresentationCollection>();

            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new Stub.PrincipalProviderStub())
                .Repeat.Any();

            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("f37827df-c22a-4569-9512-c0c48791d46c"))
                .Return(entity);
            //Expect.Call(entity.Presentations)
            //    .Return(presentationCollection);
            //Expect.Call(presentationCollection.ContainsKey(""))
            //    .IgnoreArguments()
            //    .Return(false);

            mocks.ReplayAll();

            ParametersService parametersService = new ParametersService(null);

            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            ViewService service = new ViewService();
            installer.ConfigureViews(service, parametersService);

            var view = service.GetView("D_Variant_Schuldbuch");

            XControl xControl = new XBuilderFactory(scheme, parametersService).Create(view);
            Control control = xControl.Create();

            mocks.VerifyAll();
        }

        [Test]
        public void XGridViewCustomViewTest()
        {
            IPackage package = mocks.DynamicMock<IPackage>();
            IScheme scheme = mocks.DynamicMock<IScheme>();
            IEntity entity = mocks.DynamicMock<IEntity>();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new Stub.PrincipalProviderStub())
                .Repeat.Any();

            Expect.Call(container.Resolve(typeof(ParameterValueProvider)))
                .Return(null);

            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("f37827df-c22a-4569-9512-c0c48791d46c"))
                .Return(entity);

            mocks.ReplayAll();

            ParametersService parametersService = new ParametersService(container);

            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Core.Tests.Data.extension.config.xml");

            installer.RegisterParameters(parametersService);

            ViewService service = new ViewService();
            installer.ConfigureViews(service, parametersService);

            var view = service.GetView("GridWithCustomEditorView");

            XControl xControl = new XBuilderFactory(scheme, parametersService).Create(view);
            Control control = xControl.Create();

            mocks.VerifyAll();

            Assert.IsInstanceOf(typeof(GridView), control);
            GridView gv = (GridView) control;
            Assert.IsNotNull(gv.RowEditorFormView);
            Assert.AreEqual("/BebtBook/ShowDetails", gv.RowEditorFormView.Url);
            Assert.AreEqual(2, gv.RowEditorFormView.Params.Count);
            Assert.AreEqual("objectKey", gv.RowEditorFormView.Params[0].Name);
            Assert.AreEqual("userRegionType", gv.RowEditorFormView.Params[1].Name);
            Assert.AreEqual("grid.ModelObjectKey", gv.RowEditorFormView.Params[0].Value);
            Assert.AreEqual(RowEditorFormViewParameterMode.Raw, gv.RowEditorFormView.Params[0].Mode);
            Assert.AreEqual(RowEditorFormViewParameterMode.Value, gv.RowEditorFormView.Params[1].Mode);
        }
    }
}
