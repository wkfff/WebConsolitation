using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
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
    public class XBebtBookFormViewTests
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
            IVariantProtocolService protocolService = mocks.DynamicMock<IVariantProtocolService>();
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            
            Expect.Call(container.Resolve(typeof(ViewServiceStub)))
                .Return(new ViewServiceStub())
                .Repeat.Any();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new PrincipalProviderStub())
                .Repeat.Any();

            DebtBookExtension debtBookExtension = new DebtBookExtension(null, null, null, null, null, null);
            debtBookExtension.Variant = new VariantDescriptor(0, null, null,null, 2010, DateTime.Now);

            Expect.Call(protocolService.GetStatus(0, 0))
                .Return(new T_S_ProtocolTransfer { RefStatusSchb = new FX_S_StatusSchb { ID = 1 }});

            mocks.Replay(protocolService);
            mocks.Replay(container);

            Expect.Call(controlFactory.CreateControl(typeof(BebtBookFormView)))
                .Return(new BebtBookFormView(protocolService, debtBookExtension, null));

            Expect.Call(scheme.RootPackage)
                .Return(package);
            Expect.Call(package.FindEntityByName("8ae51f5d-32a8-402e-a0c2-9d292dee76d6"))
                .Return(entity);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection.ContainsKey("80142117-0b7d-4d9b-9bca-a312570bd516"))
                .IgnoreArguments()
                .Return(true);
            Expect.Call(entity.Presentations)
                .Return(presentationCollection);
            Expect.Call(presentationCollection["80142117-0b7d-4d9b-9bca-a312570bd516"])
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

            var view = service.GetView("LimitsFormSubject");
            ControlBuilder.Current.SetControlFactory(controlFactory);
            XControl xControl = new XBuilderFactory(scheme, parametersService).Create(view);
            Control control = xControl.Create();

            mocks.VerifyAll();

            BebtBookFormView formView = control as BebtBookFormView;

            Assert.IsNotNull(formView);
            Assert.IsNotNull(formView.Entity);
            Assert.IsNotNull(formView.Presentation);
            Assert.AreEqual(typeof(ViewServiceStub), formView.ViewService.GetType());
            Assert.AreEqual(9, formView.Fields.Count);
            Assert.AreEqual(2, formView.TabRegionType);
        }
    }
}
