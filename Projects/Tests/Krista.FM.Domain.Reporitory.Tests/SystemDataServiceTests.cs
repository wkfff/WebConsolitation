using System.Collections.Generic;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.Domain.Reporitory.Tests
{
    [TestFixture]
    public class SystemDataServiceTests
    {
        private IList<DataSources> data;

        [SetUp]
        public void Setup()
        {
            data = new List<DataSources>
                    {
                        new DataSources {ID = 1 },
                        new DataSources {ID = 2 },
                    };
        }

        [Test]
        public void QueryBuildTest()
        {
            MockRepository mocks = new MockRepository();
            IDatabase database = mocks.DynamicMock<IDatabase>();
            Expect.Call(database.ExecQuery(null, QueryResultTypes.DataTable))
                .IgnoreArguments()
                .Return(DataTableHelper.CreateDataTable(data));

            mocks.ReplayAll();

            SystemDataService dataService = new SystemDataService(database);
            DomainRepository repository = new DomainRepository(dataService);
            DataSources ds = repository.Get<DataSources>(1);

            mocks.VerifyAll();
        }
    }
}
