using System;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class EntityDataServiceTests
    {
        private MockRepository mocks;
        private IScheme scheme;
        private ISchemeDWH schemeDwh;
        private IDatabase database;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();

            scheme = mocks.DynamicMock<IScheme>();
            schemeDwh = mocks.DynamicMock<ISchemeDWH>();
            database = mocks.DynamicMock<IDatabase>();

            Expect.Call(scheme.SchemeDWH).Return(schemeDwh).Repeat.Times(2);
            Expect.Call(schemeDwh.FactoryName).Return(ProviderFactoryConstants.OracleClient).Repeat.Once();
            Expect.Call(schemeDwh.DB).Return(database).Repeat.Once();
        }

        [Test]
        public void CanBuildQueryWithoutAssociationsTest()
        {
            // Настройка
            IEntity entity = mocks.DynamicMock<IEntity>();

            IDbDataParameter[] parameters = new IDbDataParameter[0];
            Expect.Call(database.ExecQuery(
                "select * from (select t0.ID as ID,t0.SourceID as SourceID,t0.Code as Code,t0.RefVariant as RefVariant from f_Test_Test t0) T", 
                QueryResultTypes.DataTable, parameters))
                .Return(new DataTable());

            IEntityDataQueryBuilder queryBuilder = mocks.DynamicMock<IEntityDataQueryBuilder>();
            Expect.Call(queryBuilder.BuildQuery(entity, "||")).Return(
                new StringBuilder(
                    "select t0.ID as ID,t0.SourceID as SourceID,t0.Code as Code,t0.RefVariant as RefVariant from f_Test_Test t0"))
                .Repeat.Any();

            mocks.ReplayAll();

            // Выполнение
            EntityDataService dataService = new EntityDataService(scheme);
            dataService.GetData(entity, 0, 1, String.Empty, String.Empty, String.Empty, parameters, queryBuilder);

            // Проверки
            mocks.VerifyAll();
        }
    }
}
