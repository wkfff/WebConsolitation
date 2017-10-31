using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.Domain.Reporitory.Tests
{
    [TestFixture]
    public class SchemeRepositoryTest
    {
        [Test, Ignore]
        public void SchemeRepositoryGetAllTest()
        {
            IList<D_S_TestObject> data =
                new List<D_S_TestObject>
                    {
                        new D_S_TestObject {ID = 1, Name = "Name1", Date = new DateTime(2010, 7, 16), Comment = "Comment"},
                        new D_S_TestObject {ID = 2, Name = "Name2", IntNullable = 10 },
                    };

            MockRepository mocks = new MockRepository();
            IDomainDataService dataService = mocks.DynamicMock<IDomainDataService>();
            Expect.Call(dataService.GetObjectData(null, null))
                .IgnoreArguments()
                .Return(DataTableHelper.CreateDataTable(data).Select());
            
            mocks.ReplayAll();

            DomainRepository repository = new DomainRepository(dataService);
            var list = repository.GetAll<D_S_TestObject>();

            mocks.VerifyAll();

            Assert.AreEqual(data.Count, list.Count());
            
            Assert.AreEqual(data[0].ID, list[0].ID);
            Assert.AreEqual(data[1].ID, list[1].ID);
            
            Assert.AreEqual(data[0].Name, list[0].Name);
            Assert.AreEqual(data[1].Name, list[1].Name);
            
            Assert.AreEqual(data[0].Date, list[0].Date);
            Assert.AreEqual(data[1].Date, list[1].Date);
            
            Assert.AreEqual(data[0].IntNullable, list[0].IntNullable);
            Assert.AreEqual(data[1].IntNullable, list[1].IntNullable);
        }
    }
}
