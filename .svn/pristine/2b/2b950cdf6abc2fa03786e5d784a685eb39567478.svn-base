using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.Region10MarksOIV.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Tests
{
    [TestFixture]
    public class FactRepositoryTests
    {
        private MockRepository mocks;

        private ILinqRepository<F_OIV_REG10Qual> factDatabaseRepository;
        private IRegion10MarksOivExtension extension;
        private IMarksRepository marksRepository;
        private ITerritoryRepository territoryRepository;
        private IRepository<FX_OIV_StatusData> statusRepository;

        private FactRepository factRepository;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            factDatabaseRepository = mocks.DynamicMock<ILinqRepository<F_OIV_REG10Qual>>();
            extension = mocks.DynamicMock<IRegion10MarksOivExtension>();
            marksRepository = mocks.DynamicMock<IMarksRepository>();
            territoryRepository = mocks.DynamicMock<ITerritoryRepository>();
            statusRepository = mocks.DynamicMock<IRepository<FX_OIV_StatusData>>();

            var refYear = new FX_Date_Year { ID = 2011 };
            var sourceIdOiv = 1;
            
            var oiv = new D_OMSU_ResponsOIV { ID = 1 };

            IList<D_OIV_TypeMark> typeMarks = new List<D_OIV_TypeMark>
            {
                new D_OIV_TypeMark { ID = (int)TypeMark.Gather, Code = (int)TypeMark.Gather },
                new D_OIV_TypeMark { ID = (int)TypeMark.TargetValue, Code = (int)TypeMark.TargetValue }
            };

            IList<D_OIV_Mark> marks = new List<D_OIV_Mark>
            {
                new D_OIV_Mark { ID = 1, SourceID = sourceIdOiv, RefTypeMark = typeMarks[0], MO = true },
                new D_OIV_Mark { ID = 2, SourceID = sourceIdOiv, RefTypeMark = typeMarks[0], MO = false },
                new D_OIV_Mark { ID = 3, SourceID = sourceIdOiv, RefTypeMark = typeMarks[0], MO = true },
                new D_OIV_Mark { ID = 4, SourceID = sourceIdOiv, RefTypeMark = typeMarks[0], MO = true, RefResponsOIV = oiv },
                new D_OIV_Mark { ID = 5, SourceID = sourceIdOiv, RefTypeMark = typeMarks[0], MO = false, RefResponsOIV1 = oiv }
            };

            IList<D_Territory_RF> territories = new List<D_Territory_RF>
            {
                new D_Territory_RF { ID = 1 },
                new D_Territory_RF { ID = 2 },
                new D_Territory_RF { ID = 3 },
                new D_Territory_RF { ID = 4 },
                new D_Territory_RF { ID = 5 },
                new D_Territory_RF { ID = 6 }
            };

            List<F_OIV_REG10Qual> data = new List<F_OIV_REG10Qual>
            {
                new F_OIV_REG10Qual { ID = 1, SourceID = sourceIdOiv, RefOIV = marks[0], RefTerritory = territories[0], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 2, SourceID = sourceIdOiv, RefOIV = marks[1], RefTerritory = territories[0], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 3, SourceID = sourceIdOiv, RefOIV = marks[2], RefTerritory = territories[0], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 4, SourceID = sourceIdOiv, RefOIV = marks[3], RefTerritory = territories[4], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 5, SourceID = sourceIdOiv, RefOIV = marks[4], RefTerritory = territories[0], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 6, SourceID = sourceIdOiv, RefOIV = marks[0], RefTerritory = territories[1], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 7, SourceID = sourceIdOiv, RefOIV = marks[0], RefTerritory = territories[2], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 8, SourceID = sourceIdOiv, RefOIV = marks[0], RefTerritory = territories[3], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 9, SourceID = sourceIdOiv, RefOIV = marks[3], RefTerritory = territories[0], RefYear = refYear },
                new F_OIV_REG10Qual { ID = 10, SourceID = sourceIdOiv, RefOIV = marks[4], RefTerritory = territories[1], RefYear = refYear },
            };

            ////region = regions[0];

            Expect.Call(extension.UserResponseOiv).Return(oiv).Repeat.Any();
            Expect.Call(extension.CurrentYear).Return(refYear).Repeat.Any();
            Expect.Call(extension.DataSourceOiv).Return(new DataSources { ID = sourceIdOiv }).Repeat.Any();
            Expect.Call(extension.RootTerritoryRf).Return(territories[0]).Repeat.Any();
            Expect.Call(marksRepository.FindAll()).Return(marks.AsQueryable()).Repeat.Any();
            Expect.Call(marksRepository.FindOne(1)).Return(marks[0]).Repeat.Any();
            Expect.Call(marksRepository.FindOne(5)).Return(marks[4]).Repeat.Any();
            Expect.Call(territoryRepository.FindAll()).Return(territories.AsQueryable()).Repeat.Any();
            Expect.Call(territoryRepository.FindOne(1)).Return(territories[0]).Repeat.Any();
            Expect.Call(territoryRepository.FindOne(2)).Return(territories[1]).Repeat.Any();
            Expect.Call(territoryRepository.FindOne(3)).Return(territories[2]).Repeat.Any();
            Expect.Call(statusRepository.Get((int)OivStatus.OnEdit)).Return(new FX_OIV_StatusData { ID = (int)OivStatus.OnEdit }).Repeat.Any();
            Expect.Call(factDatabaseRepository.FindAll()).Return(data.AsQueryable()).Repeat.Any();

            mocks.ReplayAll();

            factRepository = new FactRepository(factDatabaseRepository, extension, marksRepository, territoryRepository, statusRepository);
        }

        [Test]
        public void GetFactForMarkTerritoryTest()
        {
            var result = factRepository.GetFactForMarkTerritory(1, 1);

            mocks.VerifyAll();

            Assert.AreEqual(result.ID, 1);
            Assert.AreEqual(result.RefOIV.ID, 1);
            Assert.AreEqual(result.RefTerritory.ID, 1);
        }

        [Test]
        public void GetEmptyFactForMarkTerritoryTest()
        {
            // Нет существующей записи для данного показателя/территории в таблице фактов
            var result = factRepository.GetFactForMarkTerritory(5, 3);

            mocks.VerifyAll();

            Assert.AreEqual(result.ID, 0);
            Assert.AreEqual(result.RefOIV.ID, 5);
            Assert.AreEqual(result.RefTerritory.ID, 3);
        }

        [Test]
        public void GetMarksForOivTest()
        {
            var result = factRepository.GetMarksForOiv();

            mocks.VerifyAll();

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].ID, 5);
            Assert.AreEqual(result[1].ID, 9);
        }

        [Test]
        public void GetMarksForIMATest()
        {
            var result = factRepository.GetMarksForIMA();

            mocks.VerifyAll();

            Assert.AreEqual(result.Count, 5);
            Assert.AreEqual(result[0].ID, 1);
            Assert.AreEqual(result[1].ID, 2);
            Assert.AreEqual(result[2].ID, 3);
            Assert.AreEqual(result[3].ID, 5);
            Assert.AreEqual(result[4].ID, 9);
        }

        [Test]
        public void GetMarksTest()
        {
            var territory = territoryRepository.FindOne(2);
            var result = factRepository.GetMarks(territory);

            mocks.VerifyAll();

            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].ID, 6);
        }

        [Test]
        public void GetTerritoriesTest()
        {
            var result = factRepository.GetTerritories(1);

            mocks.VerifyAll();

            Assert.AreEqual(result.Count, 4);
            Assert.AreEqual(result[0].ID, 1);
            Assert.AreEqual(result[1].ID, 6);
            Assert.AreEqual(result[2].ID, 7);
            Assert.AreEqual(result[3].ID, 8);
        }
    }
}
