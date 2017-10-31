using System;
using System.Collections.Generic;
using Krista.FM.Server.Scheme.Classes;
using Krista.FM.ServerLibrary;

using NUnit.Framework;
using Rhino.Mocks;


namespace Krista.FM.Server.Scheme.Tests.Classes
{
    
   	[TestFixture]
    public class UniqueKeyTests
    {
        private MockRepository mocks;
   	    private Entity entity;
        

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            entity = mocks.DynamicMock<Entity>();
        }


   	    [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Для данного типа таблиц уникальные ключи не предусмотрены.")]
        public void ParentClassTypeIsAssociationTest()
        {
            Expect.Call(entity.ClassType).Return(ClassTypes.clsAssociation).Repeat.Any();
            mocks.ReplayAll();
   	        UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), entity, ServerSideObjectStates.New);
            mocks.VerifyAll();
        }

        [Test]
        public void ParentClassTypeIsBridgeClassifierTest()
        {
            Expect.Call(entity.ClassType).Return(ClassTypes.clsBridgeClassifier).Repeat.Any();
            mocks.ReplayAll();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), entity, ServerSideObjectStates.New);
            mocks.VerifyAll();
            Assert.True(true);
        }

        [Test]
        public void ParentClassTypeIsClsDataClassifierTest()
        {
            Expect.Call(entity.ClassType).Return(ClassTypes.clsDataClassifier).Repeat.Any();
            mocks.ReplayAll();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), entity, ServerSideObjectStates.New);
            mocks.VerifyAll();
            Assert.True(true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Для данного типа таблиц уникальные ключи не предусмотрены.")]
        public void ParentClassTypeIsFactDataTest()
        {
            Expect.Call(entity.ClassType).Return(ClassTypes.clsFactData).Repeat.Any();
            mocks.ReplayAll();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), entity, ServerSideObjectStates.New);
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Для данного типа таблиц уникальные ключи не предусмотрены.")]
        public void ParentClassTypeIsFixedClassifierTest()
        {
            Expect.Call(entity.ClassType).Return(ClassTypes.clsFixedClassifier).Repeat.Any();
            mocks.ReplayAll();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), entity, ServerSideObjectStates.New);
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Для данного типа таблиц уникальные ключи не предусмотрены.")]
        public void ParentClassTypeIsPackageTest()
        {
            Expect.Call(entity.ClassType).Return(ClassTypes.clsPackage).Repeat.Any();
            mocks.ReplayAll();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), entity, ServerSideObjectStates.New);
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Для данного типа таблиц уникальные ключи не предусмотрены.")]
        public void ParentClassTypeIsDocumentEntityTest()
        {
            Expect.Call(entity.ClassType).Return(ClassTypes.DocumentEntity).Repeat.Any();
            mocks.ReplayAll();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), entity, ServerSideObjectStates.New);
            mocks.VerifyAll();
        }

        [Test]
        public void ParentClassTypeIsTableTest()
        {
            Expect.Call(entity.ClassType).Return(ClassTypes.Table).Repeat.Any();
            mocks.ReplayAll();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), entity, ServerSideObjectStates.New);
            mocks.VerifyAll();
            Assert.True(true);
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException), UserMessage = "Не указан владелец.")]
        public void ParentNotNullTest()
        {
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), null, ServerSideObjectStates.New);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Недопустимый тип родительского объекта.")]
        public void ParentIsPackageTest()
        {
            Package parent = mocks.DynamicMock<Package>();
            UniqueKey uk  = new UniqueKey(Guid.NewGuid().ToString(), parent, ServerSideObjectStates.New);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Недопустимый тип родительского объекта.")]
        public void ParentIsUniqueKeyCollectionTest()
        {
            UniqueKeyCollection parent = mocks.DynamicMock<UniqueKeyCollection>();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), parent, ServerSideObjectStates.New);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Недопустимый тип родительского объекта.")]
        public void ParentIsAssociationTest()
        {
            Association parent = mocks.DynamicMock<Association>();
            UniqueKey uk = new UniqueKey(Guid.NewGuid().ToString(), parent, ServerSideObjectStates.New);
        }

        [Test]
        public void SetFieldsTest()
        {
            Entity entityStub = GetEntityStub();
            
            UniqueKey uniqueKey = new UniqueKey(Guid.NewGuid().ToString(), entityStub, ServerSideObjectStates.New);
            Assert.IsNotNull(uniqueKey);

            List<string> fields = new List<string>();
            fields.Add("field1");
            Assert.IsTrue(fields.Count == 1);

            uniqueKey.Fields = fields;
            Assert.True(true);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), UserMessage = "Параметр не может быть null")]
        public void SetFieldsNullTest()
        {
            Entity entityStub = GetEntityStub();

            UniqueKey uniqueKey = new UniqueKey(Guid.NewGuid().ToString(), entityStub, ServerSideObjectStates.New);
            Assert.IsNotNull(uniqueKey);

            List<string> fields = null;
            
            uniqueKey.Fields = fields;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), UserMessage = "Параметр должен иметь значения")]
        public void SetFieldsEmptyTest()
        {
            Entity entityStub = GetEntityStub();

            UniqueKey uniqueKey = new UniqueKey(Guid.NewGuid().ToString(), entityStub, ServerSideObjectStates.New);
            Assert.IsNotNull(uniqueKey);

            List<string> fields = new List<string>();
            
            uniqueKey.Fields = fields;
        }


        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Поля должны быть уникальными")]
        public void SetFieldsNotUniqueIgnoreCaseTest()
        {
            Entity entityStub = GetEntityStub();

            UniqueKey uniqueKey = new UniqueKey(Guid.NewGuid().ToString(), entityStub, ServerSideObjectStates.New);
            Assert.IsNotNull(uniqueKey);

            List<string> fields = new List<string>();
            fields.Add("field1");
            fields.Add("Field1");

            uniqueKey.Fields = fields;
        }

        [Test]
        public void SetFieldsFromAssociationTest()
        {
            Entity entityStub = GetEntityStub();

            UniqueKey uniqueKey = new UniqueKey(Guid.NewGuid().ToString(), entityStub, ServerSideObjectStates.New);
            Assert.IsNotNull(uniqueKey);

            List<string> fields = new List<string>();
            fields.Add("associationfield1");

            uniqueKey.Fields = fields;
            Assert.True(true);
        }


        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), UserMessage = "Поле OtherTableField1 не принадлежит данной таблице.")]
        public void SetFieldsNotFromThisEntityTest()
        {
            Entity entityStub = GetEntityStub();

            UniqueKey uniqueKey = new UniqueKey(Guid.NewGuid().ToString(), entityStub, ServerSideObjectStates.New);
            Assert.IsNotNull(uniqueKey);

            List<string> fields = new List<string>();
            fields.Add("OtherTableField1");

            uniqueKey.Fields = fields;
        }


        /// <summary>
        /// Генерит объект-заглушку Entity с одним полем, одной ассоциацией и пустым списком существующих уникальных ключей
        /// </summary>
        /// <returns></returns>
        internal Entity GetEntityStub()
        {
            Entity entityStub = mocks.DynamicMock<Entity>();
            Expect.Call(entityStub.ClassType).Return(ClassTypes.clsDataClassifier).Repeat.Any();

            ICollection<IDataAttribute> dataAttributes = new List<IDataAttribute>();
            IDataAttribute dataAttribute1 = mocks.DynamicMock<IDataAttribute>();
            Expect.Call(dataAttribute1.Name).Return("field1").Repeat.Any();
            dataAttributes.Add(dataAttribute1);

            IDataAttributeCollection dataAttributeCollection = mocks.DynamicMock<IDataAttributeCollection>();
            Expect.Call(dataAttributeCollection.Values).Return(dataAttributes).Repeat.Any();
            Expect.Call(dataAttributeCollection.Count).Return(dataAttributes.Count).Repeat.Any();

            Expect.Call(entityStub.Attributes).Return(dataAttributeCollection).Repeat.Any();
            Expect.Call(((IEntity)entityStub).Attributes).Return(dataAttributeCollection).Repeat.Any();

            IUniqueKeyCollection uniqueKeyCollection = mocks.DynamicMock<IUniqueKeyCollection>();
            ICollection<IUniqueKey> uniqueKeys = new List<IUniqueKey>();
            Expect.Call(uniqueKeyCollection.Values).Return(uniqueKeys).Repeat.Any();
            Expect.Call(uniqueKeyCollection.Count).Return(uniqueKeys.Count).Repeat.Any();

            Expect.Call(entityStub.UniqueKeys).Return(uniqueKeyCollection).Repeat.Any();
            Expect.Call(((IEntity)entityStub).UniqueKeys).Return(uniqueKeyCollection).Repeat.Any();

            ICollection<IEntityAssociation> associations = new List<IEntityAssociation>();
            IAssociation association1 = mocks.DynamicMock<IAssociation>();
            Expect.Call(association1.FullDBName).Return("associationfield1").Repeat.Any();
            associations.Add(association1);

            IEntityAssociationCollection associationCollection = mocks.DynamicMock<IEntityAssociationCollection>();
            Expect.Call(associationCollection.Values).Return(associations).Repeat.Any();
            Expect.Call(associationCollection.Count).Return(associations.Count).Repeat.Any();

            Expect.Call(entityStub.Associations).Return(associationCollection).Repeat.Any();
            Expect.Call(((IEntity)entityStub).Associations).Return(associationCollection).Repeat.Any();

            mocks.ReplayAll();

            Assert.IsTrue(entityStub.Attributes.Count == 1);
            Assert.IsTrue(entityStub.Associations.Count == 1);
            Assert.IsTrue(entityStub.UniqueKeys.Count == 0);

            return entityStub;
        }


    }

        
}
