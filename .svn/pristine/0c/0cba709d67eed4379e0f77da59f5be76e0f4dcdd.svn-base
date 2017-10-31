using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Tests
{
    [TestFixture]
    public class MarksOmsuRepositoryTests
    {
        private MockRepository mocks;
        private ILinqRepository<F_OMSU_Reg16> factRepository;
        private IMarksOmsuExtension extension;
        private IMarksRepository marksRepository;
        private IRegionsRepository regionRepository;
        private IRepository<FX_OMSU_StatusData> statusRepository;
        private D_Regions_Analysis region;
        private MarksOmsuRepository repository;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            factRepository = mocks.DynamicMock<ILinqRepository<F_OMSU_Reg16>>();
            extension = mocks.DynamicMock<IMarksOmsuExtension>();
            marksRepository = mocks.DynamicMock<IMarksRepository>();
            regionRepository = mocks.DynamicMock<IRegionsRepository>();
            statusRepository = mocks.DynamicMock<IRepository<FX_OMSU_StatusData>>();

            var refYearDayUnv = new FX_Date_YearDayUNV { ID = 20100001 };
            var sourceIdOmsu = 1;
            var sourceIdTerritory = 10;
            
            var oiv = new D_OMSU_ResponsOIV { ID = 1 };

            IList<D_OMSU_TypeMark> typeMarks = new List<D_OMSU_TypeMark>
            {
                new D_OMSU_TypeMark { ID = 1, Code = 1 },
                new D_OMSU_TypeMark { ID = 2, Code = 2 }
            };

            IList<D_OMSU_MarksOMSU> marks = new List<D_OMSU_MarksOMSU>
            {
                new D_OMSU_MarksOMSU { ID = 1, SourceID = sourceIdOmsu, RefTypeMark = typeMarks[0], MO = true },
                new D_OMSU_MarksOMSU { ID = 2, SourceID = sourceIdOmsu, RefTypeMark = typeMarks[0], MO = false, RefResponsOIV = oiv },
                new D_OMSU_MarksOMSU { ID = 3, SourceID = sourceIdOmsu, RefTypeMark = typeMarks[0], MO = true },
                new D_OMSU_MarksOMSU { ID = 4, SourceID = sourceIdOmsu, RefTypeMark = typeMarks[0], MO = true },
                new D_OMSU_MarksOMSU { ID = 5, SourceID = sourceIdOmsu, RefTypeMark = typeMarks[0], MO = true, ParentID = 1 },
                new D_OMSU_MarksOMSU { ID = 6, SourceID = sourceIdOmsu, RefTypeMark = typeMarks[1], MO = true, RefResponsOIV = oiv },
                new D_OMSU_MarksOMSU { ID = 7, SourceID = sourceIdOmsu, RefTypeMark = typeMarks[0], MO = true, Grouping = true }
            };

            IList<FX_FX_TerritorialPartitionType> terrTypes = new List<FX_FX_TerritorialPartitionType>
            {
                new FX_FX_TerritorialPartitionType { ID = 1 },
                new FX_FX_TerritorialPartitionType { ID = 4 },
                new FX_FX_TerritorialPartitionType { ID = 7 }
            };

            IList<D_Regions_Analysis> regions = new List<D_Regions_Analysis>
            {
                new D_Regions_Analysis { ID = 1, SourceID = sourceIdTerritory, RefTerr = terrTypes[0] },
                new D_Regions_Analysis { ID = 2, SourceID = sourceIdTerritory, RefTerr = terrTypes[1] },
                new D_Regions_Analysis { ID = 3, SourceID = sourceIdTerritory, RefTerr = terrTypes[1] },
                new D_Regions_Analysis { ID = 4, SourceID = sourceIdTerritory, RefTerr = terrTypes[2] },
                new D_Regions_Analysis { ID = 5, SourceID = sourceIdTerritory, RefTerr = terrTypes[2] },
                new D_Regions_Analysis { ID = 6, SourceID = sourceIdTerritory, RefTerr = terrTypes[0] }
            };

            List<F_OMSU_Reg16> data = new List<F_OMSU_Reg16>
            {
                new F_OMSU_Reg16 { ID = 1, SourceID = sourceIdOmsu, RefMarksOMSU = marks[0], RefRegions = regions[0], RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 2, SourceID = sourceIdOmsu, RefMarksOMSU = marks[1], RefRegions = regions[0], RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 3, SourceID = sourceIdOmsu, RefMarksOMSU = marks[2], RefRegions = regions[0], RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 4, SourceID = sourceIdOmsu, RefMarksOMSU = marks[3], RefRegions = null, RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 5, SourceID = sourceIdOmsu, RefMarksOMSU = marks[4], RefRegions = regions[0], RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 6, SourceID = sourceIdOmsu, RefMarksOMSU = marks[0], RefRegions = regions[1], RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 7, SourceID = sourceIdOmsu, RefMarksOMSU = marks[0], RefRegions = regions[2], RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 8, SourceID = sourceIdOmsu, RefMarksOMSU = marks[0], RefRegions = regions[3], RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 9, SourceID = sourceIdOmsu, RefMarksOMSU = marks[5], RefRegions = regions[0], RefYearDayUNV = refYearDayUnv },
                new F_OMSU_Reg16 { ID = 10, SourceID = sourceIdOmsu, RefMarksOMSU = marks[6], RefRegions = regions[0], RefYearDayUNV = refYearDayUnv },
            };

            region = regions[0];

            Expect.Call(extension.ResponsOIV).Return(oiv).Repeat.Any();
            Expect.Call(extension.DataSourceOmsu).Return(new DataSources { ID = 1 }).Repeat.Any();
            Expect.Call(marksRepository.FindAll()).Return(marks.AsQueryable()).Repeat.Any();
            Expect.Call(marksRepository.FindOne(1)).Return(marks[0]).Repeat.Any();
            Expect.Call(statusRepository.Get(1)).Return(new FX_OMSU_StatusData { ID = 1 }).Repeat.Any();
            Expect.Call(extension.CurrentYearUNV).Return(refYearDayUnv).Repeat.Any();
            Expect.Call(regionRepository.FindAll()).Return(regions.AsQueryable()).Repeat.Any();
            Expect.Call(factRepository.FindAll()).Return(data.AsQueryable()).Repeat.Any();

            mocks.ReplayAll();

            repository = new MarksOmsuRepository(factRepository, extension, marksRepository, regionRepository, statusRepository);
        }

        [Test]
        public void GetForMORootTest()
        {
            var result = repository.GetForMO(region);

            mocks.VerifyAll();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(10, result[3].ID);
        }

        [Test]
        public void GetForMOChildsTest()
        {
            var result = repository.GetForMO(region, 1);

            mocks.VerifyAll();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(5, result[0].ID);
        }

        [Test]
        public void GetForOivTest()
        {
            var result = repository.GetForOIV(1);

            mocks.VerifyAll();

            Assert.AreEqual(4, result.Count);
        }

        ////[Test]
        ////public void GetMarksListForOIVTest()
        ////{
        ////    var result = repository.GetMarksListForOIV();

        ////    mocks.VerifyAll();

        ////    Assert.AreEqual(1, result.Count);
        ////    Assert.AreEqual(1, result[0].RefResponsOIV.ID);
        ////}

        ////[Test]
        ////public void GetMarksListForCompareTest()
        ////{
        ////    var result = repository.GetMarksListForCompare();

        ////    mocks.VerifyAll();

        ////    Assert.AreEqual(5, result.Count);
        ////}

        [Test]
        public void GetFactForMarkRegionTest()
        {
            var result = repository.GetFactForMarkRegion(1, 1);

            mocks.VerifyAll();

            Assert.AreEqual(1, result.ID);
            Assert.AreEqual(1, result.RefRegions.ID);
        }

        [Test]
        public void GetEmptyFactForMarkRegionTest()
        {
            var result = repository.GetFactForMarkRegion(1, 6);

            mocks.VerifyAll();

            Assert.AreEqual(0, result.ID);
        }
    }
}
