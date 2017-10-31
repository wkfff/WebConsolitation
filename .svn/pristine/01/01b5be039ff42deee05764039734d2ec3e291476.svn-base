using System;
using System.Collections.Generic;
using System.Xml;
using Krista.FM.Server.Scheme.Classes;
using Krista.FM.ServerLibrary;

using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.Server.Scheme.Tests.Classes
{
   	[TestFixture]
    public class UniqueKeyCollectionTests
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
        public void CreateNewUniqueKeyCollectionTest()
        {
            mocks.ReplayAll();
            UniqueKeyCollection uniqueKeyCollection = new UniqueKeyCollection(entity, ServerSideObjectStates.New);
            Assert.NotNull(uniqueKeyCollection);
        }

        [Test]
        public void CreateItemTest()
        {
            Entity entityStub = GetEntityStub();
            
            UniqueKeyCollection uniqueKeyCollection = new UniqueKeyCollection(entityStub, ServerSideObjectStates.New);

            int uniqueKeyCollectionCountOld = uniqueKeyCollection.Count;
            
            string uniqueKeyCaption = "Уникальный ключ 1";
            List<string> uniqueKeyFields = new List<string>(1);
            uniqueKeyFields.Add("field1");

            //Cоздаем новый ключ
            IUniqueKey uniqueKey = uniqueKeyCollection.CreateItem(uniqueKeyCaption, uniqueKeyFields, false);
            
            //Проверяем параметры нового ключа
            Assert.IsNotNull(uniqueKey );
            Assert.IsNotNull(uniqueKey.Name);
            Assert.IsNotNull(uniqueKey.Fields);
            Assert.IsTrue(uniqueKey.Fields.Count == uniqueKeyFields.Count);
            Assert.IsTrue(uniqueKey.Caption == uniqueKeyCaption);
            foreach (string field in uniqueKey.Fields)
            {
                Assert.Contains(field, uniqueKeyFields);
            }

            //Проверяем что новый ключ добавился в общую коллекцию
            Assert.IsTrue(uniqueKeyCollection.Count == (uniqueKeyCollectionCountOld + 1));

            mocks.VerifyAll();

            Assert.True(true);
        }

        [Test]
        public void ParseXmlTest()
        {
            Entity entityStub = GetEntityStub();
            
            UniqueKeyCollection uniqueKeyCollection = new UniqueKeyCollection(entityStub, ServerSideObjectStates.New);
            Assert.NotNull(uniqueKeyCollection);
            

            string ukObjectKey = Guid.NewGuid().ToString();
            string ukCaption = "UK1";
            string ukField1Name = "field1";
            string ukHashable = Boolean.TrueString.ToLower();
            string str = String.Format( "<UniqueKey objectKey=\"{0}\" caption=\"{1}\" hashable=\"{2}\"><Field name=\"{3}\"/></UniqueKey>"
                                       ,ukObjectKey, ukCaption, ukHashable, ukField1Name
                                       );
            
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(str);
            uniqueKeyCollection.Initialize(xml);
            Assert.IsTrue(uniqueKeyCollection.Count == 1);

            IEnumerator<IUniqueKey> ukEnum = uniqueKeyCollection.Values.GetEnumerator();
            ukEnum.MoveNext();
            UniqueKey uk = (UniqueKey) ukEnum.Current;
            Assert.IsTrue(uk.ObjectKey == ukObjectKey);
            Assert.IsTrue(uk.Caption == ukCaption);
            Assert.IsTrue(uk.Fields.Count == 1);
            Assert.Contains(ukField1Name, uk.Fields);

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

            //Для инициализации
            EntityDataAttribute dataAttribute = new EntityDataAttribute(Guid.Empty.ToString(), "dummyField", null, ServerSideObjectStates.Consistent);
            
            mocks.ReplayAll();

            Assert.IsTrue(entityStub.Attributes.Count == 1);
            Assert.IsTrue(entityStub.Associations.Count == 1);
            Assert.IsTrue(entityStub.UniqueKeys.Count == 0);

            return entityStub;
        }
         
         
    }
}
