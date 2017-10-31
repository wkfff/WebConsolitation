using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ext.Net;

using Krista.FM.RIA.Core.Helpers.JsonConverters;
using Krista.FM.ServerLibrary;

using Newtonsoft.Json;

using NUnit.Framework;

using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests.Helpers.JsonConverters
{
    [TestFixture]
    class AttributesToRecordsConverterTests
    {
        private ICollection<IDataAttribute> attributes;

        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();

            attributes = mocks.DynamicMock<Collection<IDataAttribute>>();
            IDataAttribute attrId = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrSourceId = mocks.DynamicMock<IDataAttribute>();
            IDataAttribute attrCode = mocks.DynamicMock<IDataAttribute>();

            attributes = new Collection<IDataAttribute> { attrId, attrSourceId, attrCode };

            Expect.Call(attrId.Name).Ret("ID");
            Expect.Call(attrId.Type).Ret(DataAttributeTypes.dtDouble);

            Expect.Call(attrSourceId.Name).Ret("PumpId");
            Expect.Call(attrSourceId.Type).Ret(DataAttributeTypes.dtInteger);
            Expect.Call(attrSourceId.Class).Ret(DataAttributeClassTypes.System);
            Expect.Call(attrSourceId.LookupType).Ret(LookupAttributeTypes.None);

            Expect.Call(attrCode.Name).Ret("SourceId");
            Expect.Call(attrCode.Type).Ret(DataAttributeTypes.dtString);
            Expect.Call(attrCode.Class).Ret(DataAttributeClassTypes.Typed);
            Expect.Call(attrCode.LookupType).Ret(LookupAttributeTypes.None);
            Expect.Call(attrCode.DefaultValue).Ret(10);
        }

        [Test]
        public void CanConvert()
        {
            bool result = new AttributesToRecordsConverter().CanConvert(attributes.GetType());
            Assert.AreEqual(result, true);

            List<DataRowConverter> notAttributes = new List<DataRowConverter>();
            result = new AttributesToRecordsConverter().CanConvert(notAttributes.GetType());
            Assert.AreEqual(result, false);
        }

        [Test]
        public void WriteJson()
        {
            mocks.ReplayAll();
            string result = JSON.Serialize(attributes, new List<JsonConverter> { new AttributesToRecordsConverter() });
            mocks.VerifyAll();
            Assert.AreEqual(result, "[{  name: \'ID\'},{  name: \'PUMPID\'},{  name: \'SOURCEID\'}]");
        }
    }
}
