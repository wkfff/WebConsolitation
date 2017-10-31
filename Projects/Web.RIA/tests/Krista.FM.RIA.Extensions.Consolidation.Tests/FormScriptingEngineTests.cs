using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;
using Krista.FM.ServerLibrary;

using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class FormScriptingEngineTests
    {
        private MockRepository mocks;
        private IDatabaseObjectHashNameResolver hashNameResolver;
        private IScheme scheme;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            hashNameResolver = mocks.DynamicMock<IDatabaseObjectHashNameResolver>();
            scheme = mocks.DynamicMock<IScheme>();
            ISchemeDWH schemeDwh = mocks.DynamicMock<ISchemeDWH>();

            Expect.Call(scheme.SchemeDWH).Return(schemeDwh).Repeat.Any();
            Expect.Call(schemeDwh.FactoryName).Return(ProviderFactoryConstants.OracleDataAccess);
        }

        [Test]
        public void EmptyMetadataTest()
        {
            mocks.ReplayAll();

            var service = new FormScriptingEngine(new ScriptingEngineFactory(scheme), hashNameResolver);
            service.Create(new D_CD_Templates(), 1);

            mocks.VerifyAll();
        }

        [Test(Description = "Форма без реквизитов с разделом без реквизитов")]
        public void OneSectionTest()
        {
            var form = new D_CD_Templates { InternalName = "053127" };

            var part = new D_Form_Part { InternalName = "s1", RefForm = form };
            form.Parts.Add(part);

            part.Columns.Add(new D_Form_TableColumn { InternalName = "col1", DataType = "System.Int32" });
            part.Columns.Add(new D_Form_TableColumn { InternalName = "col2", DataType = "System.Decimal", DataTypeSize = 10, DataTypeScale = 2 });
            part.Columns.Add(new D_Form_TableColumn { InternalName = "col3", DataType = "System.Decimal" });
            part.Columns.Add(new D_Form_TableColumn { InternalName = "col4", DataType = "System.String" });
            part.Columns.Add(new D_Form_TableColumn { InternalName = "col5", DataType = "System.String", DataTypeSize = 255 });
            part.Columns.Add(new D_Form_TableColumn { InternalName = "col6", DataType = "System.DateTime" });
            part.Columns.Add(new D_Form_TableColumn { InternalName = "col7", DataType = "System.Int32", Required = true });

            Expect.Call(hashNameResolver.Create("PKx053127______s1__________1__rw", ObjectTypes.ForeignKeysConstraint))
                .Return("PKx053127______s1__________1__rw").Repeat.Once();
            
            Expect.Call(hashNameResolver.Create("FKx053127______s1__________1__rwID", ObjectTypes.ForeignKeysConstraint))
                .Return("FKx053127______s1__________1__rwID").Repeat.Once();

            mocks.ReplayAll();

            var service = new FormScriptingEngine(new ScriptingEngineFactory(scheme), hashNameResolver);
            var script = service.Create(form, 1);

            mocks.VerifyAll();

            Assert.IsTrue(script.Contains("create table x053127______s1__________1__rw(ID NUMBER(10)  NOT NULL,col1 NUMBER(10)  NULL,col2 NUMBER(10, 2)  NULL,col3 NUMBER(17, 4)  NULL,col4 VARCHAR2(4000)  NULL,col5 VARCHAR2(255)  NULL,col6 DATE  NULL,col7 NUMBER(10)  NULL)"));
            Assert.IsTrue(script.Contains("alter table x053127______s1__________1__rw add constraint PKx053127______s1__________1__rw primary key (ID)"));
            Assert.IsTrue(script.Contains("alter table x053127______s1__________1__rw add constraint FKx053127______s1__________1__rwID foreign key (ID) references D_Report_Row (ID)"));
        }

        [Test(Description = "Форма без реквизитов с разделом и реквизитами")]
        public void OneSectionWithRequisitesTest()
        {
            var form = new D_CD_Templates { InternalName = "053127" };

            var part = new D_Form_Part { InternalName = "s1", RefForm = form };
            form.Parts.Add(part);

            part.Columns.Add(new D_Form_TableColumn { InternalName = "col1", DataType = "System.Int32" });

            var headerRequisites = new D_Form_Requisites { InternalName = "ReqHead1", DataType = "System.Int32", IsHeader = true, RefPart = part };
            part.Requisites.Add(headerRequisites);

            var footerRequisites = new D_Form_Requisites { InternalName = "ReqFoot1", DataType = "System.Int32", RefPart = part };
            part.Requisites.Add(footerRequisites);

            Expect.Call(hashNameResolver.Create("PKx053127______s1__________1__rw", ObjectTypes.ForeignKeysConstraint))
                .Return("PKx053127______s1__________1__rw").Repeat.Once();
            Expect.Call(hashNameResolver.Create("FKx053127______s1__________1__rwID", ObjectTypes.ForeignKeysConstraint))
                .Return("FKx053127______s1__________1__rwID").Repeat.Once();

            Expect.Call(hashNameResolver.Create("PKx053127______s1__________1__rh", ObjectTypes.ForeignKeysConstraint))
                .Return("PKx053127______s1__________1__rh").Repeat.Once();
            Expect.Call(hashNameResolver.Create("PKx053127______s1__________1__rf", ObjectTypes.ForeignKeysConstraint))
                .Return("PKx053127______s1__________1__rf").Repeat.Once();

            mocks.ReplayAll();

            var service = new FormScriptingEngine(new ScriptingEngineFactory(scheme), hashNameResolver);
            var script = service.Create(form, 1);

            mocks.VerifyAll();

            Assert.IsTrue(script.Contains("create table x053127______s1__________1__rw(ID NUMBER(10)  NOT NULL,col1 NUMBER(10)  NULL)"));
            Assert.IsTrue(script.Contains("alter table x053127______s1__________1__rw add constraint PKx053127______s1__________1__rw primary key (ID)"));
            Assert.IsTrue(script.Contains("alter table x053127______s1__________1__rw add constraint FKx053127______s1__________1__rwID foreign key (ID) references D_Report_Row (ID)"));

            Assert.IsTrue(script.Contains("create table x053127______s1__________1__rh(ID NUMBER(10)  NOT NULL,ReqHead1 NUMBER(10)  NULL)"));
            Assert.IsTrue(script.Contains("alter table x053127______s1__________1__rh add constraint PKx053127______s1__________1__rh primary key (ID)"));

            Assert.IsTrue(script.Contains("create table x053127______s1__________1__rf(ID NUMBER(10)  NOT NULL,ReqFoot1 NUMBER(10)  NULL)"));
            Assert.IsTrue(script.Contains("alter table x053127______s1__________1__rf add constraint PKx053127______s1__________1__rf primary key (ID)"));
        }

        [Test(Description = "Форма с реквизитами с разделом без реквизитов")]
        public void OneSectionWithReportRequisitesTest()
        {
            var form = new D_CD_Templates { InternalName = "053127" };

            var headerRequisites = new D_Form_Requisites { InternalName = "ReqHead1", DataType = "System.Int32", IsHeader = true, RefForm = form };
            form.Requisites.Add(headerRequisites);

            var footerRequisites = new D_Form_Requisites { InternalName = "ReqFoot1", DataType = "System.Int32", RefForm = form };
            form.Requisites.Add(footerRequisites);

            var part = new D_Form_Part { InternalName = "s1", RefForm = form };
            form.Parts.Add(part);

            part.Columns.Add(new D_Form_TableColumn { InternalName = "col1", DataType = "System.Int32" });

            Expect.Call(hashNameResolver.Create("PKx053127______s1__________1__rw", ObjectTypes.ForeignKeysConstraint))
                .Return("PKx053127______s1__________1__rw").Repeat.Once();
            Expect.Call(hashNameResolver.Create("FKx053127______s1__________1__rwID", ObjectTypes.ForeignKeysConstraint))
                .Return("FKx053127______s1__________1__rwID").Repeat.Once();

            Expect.Call(hashNameResolver.Create("PKx053127______1__rh", ObjectTypes.ForeignKeysConstraint))
                .Return("PKx053127______1__rh").Repeat.Once();
            Expect.Call(hashNameResolver.Create("PKx053127______1__rf", ObjectTypes.ForeignKeysConstraint))
                .Return("PKx053127______1__rf").Repeat.Once();

            mocks.ReplayAll();

            var service = new FormScriptingEngine(new ScriptingEngineFactory(scheme), hashNameResolver);
            var script = service.Create(form, 1);

            mocks.VerifyAll();

            Assert.IsTrue(script.Contains("create table x053127______s1__________1__rw(ID NUMBER(10)  NOT NULL,col1 NUMBER(10)  NULL)"));
            Assert.IsTrue(script.Contains("alter table x053127______s1__________1__rw add constraint PKx053127______s1__________1__rw primary key (ID)"));
            Assert.IsTrue(script.Contains("alter table x053127______s1__________1__rw add constraint FKx053127______s1__________1__rwID foreign key (ID) references D_Report_Row (ID)"));

            Assert.IsTrue(script.Contains("create table x053127______1__rh(ID NUMBER(10)  NOT NULL,ReqHead1 NUMBER(10)  NULL)"));
            Assert.IsTrue(script.Contains("alter table x053127______1__rh add constraint PKx053127______1__rh primary key (ID)"));
            Assert.IsTrue(script.Contains("alter table x053127______1__rh add constraint  foreign key (ID) references D_CD_Report (ID)"));

            Assert.IsTrue(script.Contains("create table x053127______1__rf(ID NUMBER(10)  NOT NULL,ReqFoot1 NUMBER(10)  NULL)"));
            Assert.IsTrue(script.Contains("alter table x053127______1__rf add constraint PKx053127______1__rf primary key (ID)"));
            Assert.IsTrue(script.Contains("alter table x053127______1__rf add constraint  foreign key (ID) references D_CD_Report (ID)"));
        }
    }
}
