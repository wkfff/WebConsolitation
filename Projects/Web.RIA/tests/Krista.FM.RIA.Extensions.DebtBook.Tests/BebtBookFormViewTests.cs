using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Krista.FM.Domain;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Core.Tests.Stub;
using Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class BebtBookFormViewTests
    {
        private MockRepository mocks;
        private IEntity entity;
        private IPresentation presentation;
        private IViewService viewService;
        private IParametersService parametersService;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            entity = mocks.DynamicMock<IEntity>();
            presentation = mocks.DynamicMock<IPresentation>();
            viewService = mocks.DynamicMock<IViewService>();
            parametersService = mocks.DynamicMock<IParametersService>();
            IDataAttributeCollection attributes = mocks.DynamicMock<IDataAttributeCollection>();
            IDataAttribute attrId = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute ConsMunDebt1 = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute MunDebt1 = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute PosDebt1 = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute ConsMunService1 = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute RefRegion = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute RefVariant = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute sourceId = mocks.DynamicMock<IDataAttribute>();
            IEntityAssociationCollection associationCollection = mocks.DynamicMock<IEntityAssociationCollection>();
            IEntityAssociation association = mocks.DynamicMock<IEntityAssociation>();

            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new PrincipalProviderStub())
                .Repeat.Any();

            Expect.Call(entity.ObjectKey).Return("TestGuid").Repeat.Any();
            Expect.Call(entity.FullDBName).Return("f_Test_Test").Repeat.Any();

            Expect.Call(entity.Attributes).Return(attributes).Repeat.Any();
            Expect.Call(presentation.Attributes).Return(attributes).Repeat.Any();

            Expect.Call(attributes.Values)
                .Return(new List<IDataAttribute>
                            {
                                attrId,
                                sourceId,
                                RefRegion, 
                                RefVariant, 
                                ConsMunDebt1, 
                                MunDebt1,
                                PosDebt1,
                                ConsMunService1
                            })
                .Repeat.Any();

            Expect.Call(attrId.Name).Return("ID").Repeat.Any();
            Expect.Call(attrId.Type).Return(DataAttributeTypes.dtInteger)
                .Repeat.Any();

            Expect.Call(sourceId.Name).Return("SOURCEID").Repeat.Any();
            Expect.Call(sourceId.Type).Return(DataAttributeTypes.dtInteger)
                .Repeat.Any();
            Expect.Call(sourceId.Class).Return(DataAttributeClassTypes.System)
                .Repeat.Any();
            Expect.Call(sourceId.LookupType).Return(LookupAttributeTypes.None)
                .Repeat.Any();

            Expect.Call(ConsMunDebt1.Name).Return("ConsMunDebt1").Repeat.Any();
            Expect.Call(ConsMunDebt1.Class).Return(DataAttributeClassTypes.Typed)
                .Repeat.Any();
            Expect.Call(ConsMunDebt1.Type).Return(DataAttributeTypes.dtDouble)
                .Repeat.Any();
            Expect.Call(ConsMunDebt1.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();
            Expect.Call(ConsMunDebt1.GroupTags).Return("Group1")
                .Repeat.Any();

            Expect.Call(MunDebt1.Name).Return("MunDebt1").Repeat.Any();
            Expect.Call(MunDebt1.Class).Return(DataAttributeClassTypes.Typed)
                .Repeat.Any();
            Expect.Call(MunDebt1.Type).Return(DataAttributeTypes.dtDouble)
                .Repeat.Any();
            Expect.Call(MunDebt1.LookupType).Return(LookupAttributeTypes.None)
                .Repeat.Any();
            Expect.Call(MunDebt1.GroupTags).Return("Group1")
                .Repeat.Any();

            Expect.Call(PosDebt1.Name).Return("PosDebt1").Repeat.Any();
            Expect.Call(PosDebt1.Class).Return(DataAttributeClassTypes.Typed)
                .Repeat.Any();
            Expect.Call(PosDebt1.Type).Return(DataAttributeTypes.dtDouble)
                .Repeat.Any();
            Expect.Call(PosDebt1.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();
            Expect.Call(PosDebt1.GroupTags).Return("Group1")
                .Repeat.Any();

            Expect.Call(ConsMunService1.Name).Return("ConsMunService1").Repeat.Any();
            Expect.Call(ConsMunService1.Class).Return(DataAttributeClassTypes.Typed)
                .Repeat.Any();
            Expect.Call(ConsMunService1.Type).Return(DataAttributeTypes.dtDouble)
                .Repeat.Any();
            Expect.Call(ConsMunService1.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();
            Expect.Call(ConsMunService1.IsNullable).Return(true)
                .Repeat.Any();
            Expect.Call(ConsMunService1.GroupTags).Return("Group2")
                .Repeat.Any();

            Expect.Call(RefRegion.Name).Return("RefRegion").Repeat.Any();
            Expect.Call(RefRegion.Class).Return(DataAttributeClassTypes.Reference)
                .Repeat.Any();
            Expect.Call(RefRegion.Type).Return(DataAttributeTypes.dtInteger)
                .Repeat.Any();
            Expect.Call(RefRegion.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();
            Expect.Call(RefRegion.GroupTags).Return("Group0")
                .Repeat.Any();
            Expect.Call(RefRegion.OwnerObject).Return(association)
                .Repeat.Any();

            Expect.Call(RefVariant.Name).Return("RefVariant").Repeat.Any();
            Expect.Call(RefVariant.Class).Return(DataAttributeClassTypes.Reference)
                .Repeat.Any();
            Expect.Call(RefVariant.Type).Return(DataAttributeTypes.dtInteger)
                .Repeat.Any();
            Expect.Call(RefVariant.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();

            Expect.Call(entity.Associations).Repeat.Any().Return(associationCollection);

            Expect.Call(associationCollection[null])
                .Repeat.Any()
                .IgnoreArguments()
                .Return(association);

            Expect.Call(association.RoleBridge).Return(entity).Repeat.Any();
            Expect.Call(association.RoleData).Return(entity).Repeat.Any();
        }

        [Test]
        public void CanBuild()
        {
            IVariantProtocolService protocolService = mocks.DynamicMock<IVariantProtocolService>();

            DebtBookExtension debtBookExtension = new DebtBookExtension(null, null, null, null, null, null);
            debtBookExtension.Variant = new VariantDescriptor(0, null, null,null, 2010, DateTime.Now);

            Expect.Call(protocolService.GetStatus(0, 0))
                .Return(new T_S_ProtocolTransfer { RefStatusSchb = new FX_S_StatusSchb { ID = 1 } });

            Expect.Call(parametersService.GetParameterValue("OKTMO")).Return("78 000 000");
            
            IUnityContainer container = mocks.DynamicMock<IUnityContainer>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            Expect.Call(container.Resolve(typeof(IPrincipalProvider)))
                .Return(new PrincipalProviderStub())
                .Repeat.Any();

            mocks.ReplayAll();

            BebtBookFormView form = new BebtBookFormView(protocolService, debtBookExtension, parametersService);
            form.Entity = entity;
            form.Presentation = presentation;
            form.ViewService = viewService;
            form.Id = "form1";
            form.Title = "Название формы";
            form.Fields.Add("ConsMunDebt1", new ColumnState { CalcFormula = "MunDebt1 + PosDebt1" });
            form.Fields.Add("ConsMunGrnt1", new ColumnState { CalcFormula = "MunGrnt1 + PosGrnt1" });

            mocks.ReplayAll();

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            HttpContext.Current.Request.Browser = new HttpBrowserCapabilities();

            var resourceManager = new Ext.Net.ResourceManager();
            ViewPage page = new ViewPage();
            ((System.Web.UI.Control) resourceManager).Page = page;

            HttpContext.Current.Items.Add(typeof(Ext.Net.ResourceManager), resourceManager);

            page.Items.Add(typeof(Ext.Net.ResourceManager), resourceManager);

            List<Ext.Net.Component> list = form.Build(page);

            mocks.VerifyAll();
        }
    }
}
