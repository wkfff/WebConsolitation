using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Tests.Helpers;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;
using ActionDescriptor = Krista.FM.RIA.Core.ViewModel.ActionDescriptor;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class GridViewTests
    {
        private MockRepository mocks;
        private IEntity entity;
        private IPresentation presentation;
        private IViewService viewService;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            entity = mocks.DynamicMock<IEntity>();
            presentation = mocks.DynamicMock<IPresentation>();
            viewService = mocks.DynamicMock<IViewService>();
            IDataAttributeCollection attributes = mocks.DynamicMock<IDataAttributeCollection>();
            IDataAttribute attrId = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrName = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrCode = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrDate = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrDouble = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrRefData = mocks.DynamicMock<IDataAttribute>();
            IEntityAssociationCollection associationCollection = mocks.DynamicMock<IEntityAssociationCollection>();
            IEntityAssociation association = mocks.DynamicMock<IEntityAssociation>();

            Expect.Call(entity.ObjectKey).Return("TestGuid").Repeat.Any();
            Expect.Call(entity.FullDBName).Return("f_Test_Test").Repeat.Any();

            Expect.Call(entity.Attributes).Return(attributes).Repeat.Any();
            Expect.Call(presentation.Attributes).Return(attributes).Repeat.Any();

            Expect.Call(attributes.Values)
                .Return(new List<IDataAttribute>
                            {
                                attrId, 
                                attrName, 
                                attrRefData, 
                                attrCode,
                                attrDate,
                                attrDouble
                            })
                .Repeat.Any();

            Expect.Call(attrId.Name).Return("ID").Repeat.Any();
            Expect.Call(attrId.Type).Return(DataAttributeTypes.dtInteger)
                .Repeat.Any();

            Expect.Call(attrName.Name).Return("Name").Repeat.Any();
            Expect.Call(attrName.Class).Return(DataAttributeClassTypes.Typed)
                .Repeat.Any();
            Expect.Call(attrName.Type).Return(DataAttributeTypes.dtString)
                .Repeat.Any();
            Expect.Call(attrName.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();

            Expect.Call(attrCode.Name).Return("Code").Repeat.Any();
            Expect.Call(attrCode.Class).Return(DataAttributeClassTypes.Typed)
                .Repeat.Any();
            Expect.Call(attrCode.Type).Return(DataAttributeTypes.dtDouble)
                .Repeat.Any();
            Expect.Call(attrCode.LookupType).Return(LookupAttributeTypes.None)
                .Repeat.Any();

            Expect.Call(attrDate.Name).Return("Date").Repeat.Any();
            Expect.Call(attrDate.Class).Return(DataAttributeClassTypes.Typed)
                .Repeat.Any();
            Expect.Call(attrDate.Type).Return(DataAttributeTypes.dtDate)
                .Repeat.Any();
            Expect.Call(attrDate.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();

            Expect.Call(attrDouble.Name).Return("Double").Repeat.Any();
            Expect.Call(attrDouble.Class).Return(DataAttributeClassTypes.Typed)
                .Repeat.Any();
            Expect.Call(attrDouble.Type).Return(DataAttributeTypes.dtDouble)
                .Repeat.Any();
            Expect.Call(attrDouble.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();
            Expect.Call(attrDouble.IsNullable).Return(true)
                .Repeat.Any();

            Expect.Call(attrRefData.Name).Return("RefData").Repeat.Any();
            Expect.Call(attrRefData.Class).Return(DataAttributeClassTypes.Reference)
                .Repeat.Any();
            Expect.Call(attrRefData.Type).Return(DataAttributeTypes.dtInteger)
                .Repeat.Any();
            Expect.Call(attrRefData.LookupType).Return(LookupAttributeTypes.Secondary)
                .Repeat.Any();

            Expect.Call(entity.Associations).Repeat.Any().Return(associationCollection);

            Expect.Call(associationCollection[null])
                .Repeat.Any()
                .IgnoreArguments()
                .Return(association);

            Expect.Call(association.RoleBridge).Return(entity).Repeat.Any();
            Expect.Call(association.RoleData).Return(entity).Repeat.Any();

            Expect.Call(viewService.Actions)
                .Repeat.Any()
                .Return(new List<ActionDescriptor>
                            {
                                new ReportDescriptor {Id = "1", Handler = "test"}
                            });

        }

        [Test]
        public void CanBuildTest()
        {
            GridView grid = new GridView();
            grid.Entity = entity;
            grid.Presentation = presentation;
            grid.ViewService = viewService;
            grid.Params.Add("ParamKey", "ParamValue");
            grid.StoreListeners.Add("BeforeLoad", "alert('Test');");
            grid.GridListeners.Add("Command", "alert('Test');");

            mocks.ReplayAll();

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            HttpContext.Current.Request.Browser = new HttpBrowserCapabilities();

            ViewPage page = new ViewPage();
            page.Items.Add(typeof(Ext.Net.ResourceManager), new Ext.Net.ResourceManager());
            List<Ext.Net.Component> list = grid.Build(page);

            mocks.VerifyAll();
        }

        [Test]
        public void CanBuildWithCustomViewTest()
        {
            GridView grid = new GridView();
            grid.Entity = entity;
            
            RowEditorFormViewDescriptor vd = new RowEditorFormViewDescriptor();
            vd.Url = "/Custom/View";
            vd.Params.Add(new RowEditorFormViewParameterDescriptor
                { Name = "p1", Value = "v1", Mode = RowEditorFormViewParameterMode.Value});
            grid.RowEditorFormView = vd;

            mocks.ReplayAll();

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            HttpContext.Current.Request.Browser = new HttpBrowserCapabilities();

            ViewPage viewPage = new ViewPage();
            viewPage.Items.Add(typeof(Ext.Net.ResourceManager), new Ext.Net.ResourceManager());
            List<Ext.Net.Component> list = grid.Build(viewPage);

            mocks.VerifyAll();
        }

        [Test]
        public void CanBuildWithoutPresentationTest()
        {
            GridView grid = new GridView();
            grid.Entity = entity;
            grid.Presentation = null;
            grid.ViewService = viewService;
            grid.Params.Add("ParamKey", "ParamValue");
            grid.StoreListeners.Add("BeforeLoad", "alert('Test');");

            mocks.ReplayAll();

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            HttpContext.Current.Request.Browser = new HttpBrowserCapabilities();

            ViewPage viewPage = new ViewPage();
            viewPage.Items.Add(typeof(Ext.Net.ResourceManager), new Ext.Net.ResourceManager());
            List<Ext.Net.Component> list = grid.Build(viewPage);

            mocks.VerifyAll();
        }

        [Test]
        public void CanBuildWithoutViewServiceTest()
        {
            GridView grid = new GridView();
            grid.Entity = entity;
            grid.Presentation = presentation;
            grid.ViewService = null;
            grid.Params.Add("ParamKey", "ParamValue");
            grid.StoreListeners.Add("BeforeLoad", "alert('Test');");

            mocks.ReplayAll();

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            HttpContext.Current.Request.Browser = new HttpBrowserCapabilities();

            ViewPage viewPage = new ViewPage();
            viewPage.Items.Add(typeof(Ext.Net.ResourceManager), new Ext.Net.ResourceManager());
            List<Ext.Net.Component> list = grid.Build(viewPage);

            mocks.VerifyAll();
        }
    }
}
