using System.Collections.Generic;
using Krista.FM.RIA.Core.Tests.Helpers;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class EntityDataQueryBuilderTests
    {
        private MockRepository mocks;
        private IEntity entity;
        private IEntityAssociationCollection associationCollection;
        private IDataAttribute attrRefVariant;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();

            entity = mocks.DynamicMock<IEntity>();
            IDataAttributeCollection attributes = mocks.DynamicMock<IDataAttributeCollection>();
            IDataAttribute attrId = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrSourceId = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrCode = mocks.DynamicMock<IDataAttribute>();
            attrRefVariant = mocks.DynamicMock<IDataAttribute>();
            associationCollection = mocks.DynamicMock<IEntityAssociationCollection>();

            Expect.Call(entity.FullDBName).Return("f_Test_Test").Repeat.Any();

            Expect.Call(entity.Attributes).Return(attributes).Repeat.Any();

            Expect.Call(attributes.Values)
                .Ret(new List<IDataAttribute> { attrId, attrSourceId, attrCode, attrRefVariant });

            Expect.Call(attrId.Name).Ret("ID");
            Expect.Call(attrId.Type).Ret(DataAttributeTypes.dtInteger);

            Expect.Call(attrSourceId.Name).Ret("SourceID");
            Expect.Call(attrSourceId.Type).Ret(DataAttributeTypes.dtInteger);
            Expect.Call(attrSourceId.Class).Ret(DataAttributeClassTypes.System);
            Expect.Call(attrSourceId.LookupType).Ret(LookupAttributeTypes.None);

            Expect.Call(attrCode.Name).Ret("Code");
            Expect.Call(attrCode.Type).Ret(DataAttributeTypes.dtString);
            Expect.Call(attrCode.Class).Ret(DataAttributeClassTypes.Typed);
            Expect.Call(attrCode.LookupType).Ret(LookupAttributeTypes.None);
            Expect.Call(attrCode.DefaultValue).Ret(10);

            Expect.Call(attrRefVariant.Name).Ret("RefVariant");
            Expect.Call(attrRefVariant.Type).Ret(DataAttributeTypes.dtInteger);
            Expect.Call(attrRefVariant.Class).Ret(DataAttributeClassTypes.Typed);
            Expect.Call(attrRefVariant.LookupType).Ret(LookupAttributeTypes.None);
            Expect.Call(attrRefVariant.DefaultValue).Ret(-1);

            Expect.Call(entity.Associations).Ret(associationCollection);
        }

        [Test]
        public void CanBuildQueryWithoutAssociationsTest()
        {
            Expect.Call(associationCollection.Values).Ret(new List<IEntityAssociation>());

            mocks.ReplayAll();

            var builder = new EntityDataQueryBuilder();

            string query = builder.BuildQuery(entity, "||").ToString();

            mocks.VerifyAll();

            Assert.AreEqual(
                "select t0.ID as ID,t0.SourceID as SourceID,t0.Code as Code,t0.RefVariant as RefVariant from f_Test_Test t0",
                query);
        }

        [Test]
        public void CanBuildQueryWithAssociationsTest()
        {
            IEntity rb = mocks.DynamicMock<IEntity>();
            Expect.Call(rb.FullDBName).Ret("d_r_b");

            IDataAttribute attrId = mocks.DynamicMock<IDataAttribute>();
            Expect.Call(attrId.Name).Ret("ID");
            Expect.Call(attrId.LookupType).Ret(LookupAttributeTypes.None);

            IDataAttribute attrCode = mocks.DynamicMock<IDataAttribute>();
            Expect.Call(attrCode.Name).Ret("Code");
            Expect.Call(attrCode.LookupType).Ret(LookupAttributeTypes.Primary);

            IDataAttribute attrName = mocks.DynamicMock<IDataAttribute>();
            Expect.Call(attrName.Name).Ret("Name");
            Expect.Call(attrName.LookupType).Ret(LookupAttributeTypes.Secondary);
            Expect.Call(attrName.Size).Ret(255);

            IDataAttributeCollection attrCol = mocks.DynamicMock<IDataAttributeCollection>();
            Expect.Call(rb.Attributes).Ret(attrCol);
            Expect.Call(attrCol.Values).Ret(new List<IDataAttribute> { attrId, attrCode, attrName });

            IEntityAssociation association = mocks.DynamicMock<IEntityAssociation>();
            Expect.Call(association.RoleBridge).Ret(rb);
            Expect.Call(association.RoleDataAttribute).Ret(attrRefVariant);

            Expect.Call(associationCollection.Values).Repeat.Any()
                .Return(new List<IEntityAssociation> { association });

            mocks.ReplayAll();

            var builder = new EntityDataQueryBuilder();

            string query = builder.BuildQuery(entity, "||").ToString();

            mocks.VerifyAll();

            Assert.AreEqual(
                "select t0.ID as ID,t0.SourceID as SourceID,t0.Code as Code,t0.RefVariant as RefVariant,cast(t1.Code as varchar(25)) as lp_RefVariant,cast(t1.Name as varchar(255)) as ls_RefVariant from f_Test_Test t0 left outer join d_r_b t1 on (t0.RefVariant = t1.ID)",
                query);
        }
    }
}
