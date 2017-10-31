using System.Reflection;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Core.Tests.Stub;
using Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders;
using Krista.FM.RIA.Extensions.DebtBook.Tests.Stub;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class XFormEntityWithDetailsViewTests
    {
        [Test]
        public void CanCreate()
        {
            MockRepository mocks = new MockRepository();

            IPackage package = mocks.DynamicMock<IPackage>();
            IScheme scheme = mocks.DynamicMock<IScheme>();
            IEntity entity = mocks.DynamicMock<IEntity>();
            IPresentationCollection presentationCollection = mocks.DynamicMock<IPresentationCollection>();
            IPresentation presentation = mocks.DynamicMock<IPresentation>();
            IControlFactory controlFactory = mocks.DynamicMock<IControlFactory>();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();

            Expect.Call(container.Resolve(typeof(ViewServiceStub)))
                .Return(new ViewServiceStub())
                .Repeat.Any();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new PrincipalProviderStub())
                .Repeat.Any();

            Expect.Call(controlFactory.CreateControl(typeof(FormEntityWithDetailsView)))
                .Return(new FormEntityWithDetailsView());

            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("43c55c92-c819-4e0b-95a1-3b941bc2789f"))
                .Return(entity);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection.ContainsKey("447f2657-3bd1-45c9-9365-d5462c213b0a"))
                .IgnoreArguments()
                .Return(true);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection["447f2657-3bd1-45c9-9365-d5462c213b0a"])
                .IgnoreArguments()
                .Return(presentation);

            mocks.ReplayAll();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            ParametersService parametersService = new ParametersService(null);

            var installer = new ExtensionInstallerBase(
                Assembly.GetExecutingAssembly(),
                "Krista.FM.RIA.Extensions.DebtBook.Tests.Data.extension.config.xml");

            Core.ExtensionModule.Services.ViewService service = new Core.ExtensionModule.Services.ViewService();
            installer.ConfigureViews(service, parametersService);

            var view = service.GetView("SubjectOrganizationCreditForm");
            ControlBuilder.Current.SetControlFactory(controlFactory);
            XControl xControl = new XBuilderFactory(scheme, parametersService).Create(view);
            Control control = xControl.Create();

            mocks.VerifyAll();

            FormEntityWithDetailsView formView = control as FormEntityWithDetailsView;

            Assert.IsNotNull(formView);
            Assert.IsNotNull(formView.Entity);
            Assert.IsNotNull(formView.Presentation);
            Assert.AreEqual(typeof(ViewServiceStub), formView.ViewService.GetType());
            Assert.AreEqual(1, formView.Params.Count);
            Assert.AreEqual("0", formView.Params["UserRegionType"]);
            Assert.AreEqual("alert();", formView.StoreListeners["BeforeLoad"]);
        }
    }
}
