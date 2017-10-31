using System.Collections.Generic;
using Krista.FM.RIA.Extensions.DebtBook.Tests.Helpers;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class DefaultValuesQueryBuilderTests
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

            var builder = new DefaultValuesQueryBuilder();

            string query = builder.BuildQuery(entity, "||").ToString();

            mocks.VerifyAll();
            
            Assert.AreEqual(
                "select -1 as ID,null as SourceID,10 as Code,-1 as RefVariant from dual t0",
                query);
        }

        [Test]
        public void CanBuildQueryWithAssociationsTest()
        {
            IEntity rb = mocks.DynamicMock<IEntity>();
            Expect.Call(rb.FullDBName).Ret("d_r_b");

            IDataAttribute attrID = mocks.DynamicMock<IDataAttribute>();
            Expect.Call(attrID.Name).Ret("ID");
            Expect.Call(attrID.LookupType).Ret(LookupAttributeTypes.None);

            IDataAttribute attrCode = mocks.DynamicMock<IDataAttribute>();
            Expect.Call(attrCode.Name).Ret("Code");
            Expect.Call(attrCode.LookupType).Ret(LookupAttributeTypes.Primary);

            IDataAttribute attrName = mocks.DynamicMock<IDataAttribute>();
            Expect.Call(attrName.Name).Ret("Name");
            Expect.Call(attrName.LookupType).Ret(LookupAttributeTypes.Secondary);
            Expect.Call(attrName.Size).Ret(255);

            IDataAttributeCollection attrCol = mocks.DynamicMock<IDataAttributeCollection>();
            Expect.Call(rb.Attributes).Ret(attrCol);
            Expect.Call(attrCol.Values).Ret(new List<IDataAttribute> { attrID, attrCode, attrName });

            IEntityAssociation association = mocks.DynamicMock<IEntityAssociation>();
            Expect.Call(association.RoleBridge).Ret(rb);
            Expect.Call(association.RoleDataAttribute).Ret(attrRefVariant);

            Expect.Call(associationCollection.Values).Repeat.Any()
                .Return(new List<IEntityAssociation> { association });
            
            mocks.ReplayAll();

            var builder = new DefaultValuesQueryBuilder();

            string query = builder.BuildQuery(entity, "||").ToString();

            mocks.VerifyAll();

            Assert.AreEqual(
                "select -1 as ID,null as SourceID,10 as Code,-1 as RefVariant,cast(t1.Code as varchar(25)) as lp_RefVariant,cast(t1.Name as varchar(255)) as ls_RefVariant from dual t0 left outer join d_r_b t1 on (-1 = t1.ID)",
                query);
        }
    }
}
