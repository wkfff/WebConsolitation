using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;
using Ext.Net;
using Krista.FM.RIA.Core.ExtMaxExtensions;
using Krista.FM.RIA.Core.Tests.Helpers;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests.ExtNetMaxExtentions
{
     [TestFixture]
    class AdjacencyListStoreTests
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

            Expect.Call(attrId.Name).Ret("Size");
            Expect.Call(attrId.Type).Ret(DataAttributeTypes.dtDouble);
            Expect.Call(attrId.DefaultValue).Ret(22.2);

            Expect.Call(attrSourceId.Name).Ret("Code3");
            Expect.Call(attrSourceId.Type).Ret(DataAttributeTypes.dtInteger);
            Expect.Call(attrSourceId.Class).Ret(DataAttributeClassTypes.System);
            Expect.Call(attrSourceId.LookupType).Ret(LookupAttributeTypes.None);
            Expect.Call(attrSourceId.DefaultValue).Ret(115);

            Expect.Call(attrCode.Name).Ret("Name");
            Expect.Call(attrCode.Type).Ret(DataAttributeTypes.dtString);
            Expect.Call(attrCode.Class).Ret(DataAttributeClassTypes.Typed);
            Expect.Call(attrCode.LookupType).Ret(LookupAttributeTypes.None);
            Expect.Call(attrCode.DefaultValue).Ret("NameForAttrName");
        }
                 private object[] Data
        {
            get
            {
                return new object[]
                {
                    //             ID   Name ParentId
                    new object[] { "1", "a", null, false },
                    new object[] { "2", "b", null, false },
                    new object[] { "3", "cc", null, false },
                    new object[] { "4", "dd", null, false },
                    new object[] { "21", "b1", 2, true },
                    new object[] { "22", "b2", 2, true }
                };
            }
        }

         [Test]
         public void CreateTest()
         {
             try
             {
                 Page page= new Page();
                 var store = new AdjacencyListStore(page) {ID = "maxStore"};
                 store.Reader.Add(new JsonReader
                 {
                     IDProperty = "id",
                     Root = "data",
                     TotalProperty = "total",
                     SuccessProperty = "success",
                     Fields = { new RecordField("id"), new RecordField("name"), new RecordField("_parent"), new RecordField("_is_leaf") }
                 });
                 store.Proxy.Add(new HttpProxy
                 {
                     Method = HttpMethod.POST
                 });

                 store.DirectEventConfig.Method = HttpMethod.POST;
                 store.DirectEventConfig.CleanRequest = true;

                 store.DataSource = Data;
                 store.LoadData(Data);
                 var x = store.DataSource;
                 store.AddField(new RecordField("parammmmmm"));
                 store.LoadData(Data);
             }
             catch(Exception ex)
             {
                 throw ex;
             }
         }

    }
}
