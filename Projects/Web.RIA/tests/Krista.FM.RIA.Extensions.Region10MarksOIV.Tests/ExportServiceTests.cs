using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Region10MarksOIV.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Tests
{
    [TestFixture]
    public class ExportServiceTests
    {
         private static D_Territory_RF territory = new D_Territory_RF { ID = 1 };

         private static D_OIV_Mark mark = new D_OIV_Mark
                           {
                               ID = 1,
                               RefOIV = new D_OIV_Group { Name = "Группа1" },
                               Name = "Показатель1",
                               RefUnits = new D_Units_OKEI { Symbol = "см" },
                               CodeRep = "12.3"
                           };

         private static IList<F_OIV_REG10Qual> facts = new List<F_OIV_REG10Qual>
            {
                new F_OIV_REG10Qual { ID = 1, RefTerritory = territory, RefOIV = mark, Fact = 0 },
                new F_OIV_REG10Qual { ID = 2, RefTerritory = territory, RefOIV = mark, Forecast = 0 },
                new F_OIV_REG10Qual { ID = 3, RefTerritory = territory, RefOIV = mark, Forecast2 = 0 },
                new F_OIV_REG10Qual { ID = 4, RefTerritory = territory, RefOIV = mark, Forecast3 = 0 }
            };

        [Test]
        public void ExportForOmsuTest()
        {
            MockRepository mocks = new MockRepository();

            var factRepository = mocks.DynamicMock<IFactRepository>();
            var extension = mocks.DynamicMock<IRegion10MarksOivExtension>();
            var territoryRepository = mocks.DynamicMock<ITerritoryRepository>();

            Expect.Call(extension.CurrentYearVal).Return(2011).Repeat.Any();
            Expect.Call(territoryRepository.FindOne(1)).Return(territory).Repeat.Any();
            Expect.Call(factRepository.GetMarks(territory)).Return(facts).Repeat.Any();

            mocks.ReplayAll();

            var stream = new ExportService(factRepository, extension, territoryRepository)
                            .ExportForOmsu(1, 1);

            mocks.VerifyAll();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            Assert.Greater(buffer.Length, 0);

            /*using (var file = File.Create(@"d:\test.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }

        [Test]
        public void ExportForOivTest()
        {
            MockRepository mocks = new MockRepository();

            var factRepository = mocks.DynamicMock<IFactRepository>();
            var extension = mocks.DynamicMock<IRegion10MarksOivExtension>();
            var territoryRepository = mocks.DynamicMock<ITerritoryRepository>();

            Expect.Call(extension.CurrentYearVal).Return(2011).Repeat.Any();
            Expect.Call(territoryRepository.FindOne(1)).Return(territory).Repeat.Any();
            Expect.Call(factRepository.GetMarksForOiv()).Return(facts).Repeat.Any();

            mocks.ReplayAll();

            var stream = new ExportService(factRepository, extension, territoryRepository)
                            .ExportForOiv(1, 1, false);

            mocks.VerifyAll();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            Assert.Greater(buffer.Length, 0);

            /*using (var file = File.Create(@"d:\test.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }

        [Test]
        public void ExportForOivIMATest()
        {
            MockRepository mocks = new MockRepository();

            var factRepository = mocks.DynamicMock<IFactRepository>();
            var extension = mocks.DynamicMock<IRegion10MarksOivExtension>();
            var territoryRepository = mocks.DynamicMock<ITerritoryRepository>();

            Expect.Call(extension.CurrentYearVal).Return(2011).Repeat.Any();
            Expect.Call(territoryRepository.FindOne(1)).Return(territory).Repeat.Any();
            Expect.Call(factRepository.GetMarksForIMA()).Return(facts).Repeat.Any();

            mocks.ReplayAll();

            var stream = new ExportService(factRepository, extension, territoryRepository)
                            .ExportForOiv(1, 1, true);

            mocks.VerifyAll();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            Assert.Greater(buffer.Length, 0);

            /*using (var file = File.Create(@"d:\test.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }

        [Test]
        public void ExportForOmsuCompareTest()
        {
            MockRepository mocks = new MockRepository();

            var factRepository = mocks.DynamicMock<IFactRepository>();
            var extension = mocks.DynamicMock<IRegion10MarksOivExtension>();
            var territoryRepository = mocks.DynamicMock<ITerritoryRepository>();

            Expect.Call(extension.CurrentYearVal).Return(2011).Repeat.Any();
            Expect.Call(territoryRepository.FindOne(1)).Return(territory).Repeat.Any();
            Expect.Call(factRepository.GetTerritories(1)).Return(facts).Repeat.Any();

            mocks.ReplayAll();

            var stream = new ExportService(factRepository, extension, territoryRepository)
                            .ExportForOmsuCompare(1, 1);

            mocks.VerifyAll();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            Assert.Greater(buffer.Length, 0);

            /*using (var file = File.Create(@"d:\test.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }
    }
}
