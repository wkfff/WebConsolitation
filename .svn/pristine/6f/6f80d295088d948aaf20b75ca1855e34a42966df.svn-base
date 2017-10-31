using System.Collections.Generic;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controllers;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Tests.Controllers
{
    [TestFixture]
    public class MarksOivControllerTests
    {
        private MockRepository mocks;
        private IMarksOmsuExtension extension;
        private IMarksOmsuRepository repository;
        private IMarksRepository marksRepository;
        private IRegionsRepository regionRepository;
        private IRepository<FX_OMSU_StatusData> statusRepository;
        private IMarksDataInitializer dataInitializer;
        private IMarksCalculator marksCalculator;
        private IDbContext context;
        private List<F_OMSU_Reg16> data;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            extension = mocks.DynamicMock<IMarksOmsuExtension>();
            repository = mocks.DynamicMock<IMarksOmsuRepository>();
            marksRepository = mocks.DynamicMock<IMarksRepository>();
            regionRepository = mocks.DynamicMock<IRegionsRepository>();
            statusRepository = mocks.DynamicMock<IRepository<FX_OMSU_StatusData>>();
            dataInitializer = mocks.DynamicMock<IMarksDataInitializer>();
            marksCalculator = mocks.DynamicMock<IMarksCalculator>();
            context = mocks.DynamicMock<IDbContext>();

            var mark = new D_OMSU_MarksOMSU { ID = 1, RefOKEI = new D_Units_OKEI { ID = 1, Symbol = "Sy" } };
            var region = new D_Regions_Analysis { ID = 1 };
            var status = new FX_OMSU_StatusData { ID = 1 };
            var year = new FX_Date_YearDayUNV { ID = 20100001 };
            data = new List<F_OMSU_Reg16>
            {
                new F_OMSU_Reg16 { ID = 1, RefMarksOMSU = mark, RefRegions = region, RefStatusData = status, RefYearDayUNV = year },
                new F_OMSU_Reg16 { ID = 2, RefMarksOMSU = mark, RefRegions = region, RefStatusData = status, RefYearDayUNV = year }
            };
        }

        [Test]
        public void LoadTest()
        {
            Expect.Call(extension.CurrentYear).Return(2010);
            Expect.Call(() => dataInitializer.CreateRegionsForMark("mark1_2010", 1));
            Expect.Call(repository.GetForOIV(1)).Return(data);

            mocks.ReplayAll();

            MarksOivController controller = new MarksOivController(extension, repository, marksRepository, regionRepository, statusRepository, dataInitializer, marksCalculator);
            var result = controller.Load(1, null);

            mocks.VerifyAll();

            Assert.IsAssignableFrom(typeof(AjaxStoreResult), result);
            Assert.AreEqual(2, ((AjaxStoreResult)result).Total);
        }

        [Test]
        public void SaveTest()
        {
            Expect.Call(extension.DataSourceOmsu).Return(new DataSources { ID = 1 });
            Expect.Call(extension.CurrentYearUNV).Return(new FX_Date_YearDayUNV { ID = 20100001 });
            Expect.Call(extension.UserRegionCurrent).Return(new D_Regions_Analysis { ID = 1 }).Repeat.Any();
            Expect.Call(regionRepository.FindOne(1)).Return(new D_Regions_Analysis { ID = 1 }).IgnoreArguments().Repeat.Any();
            Expect.Call(() => repository.Save(null)).IgnoreArguments();
            Expect.Call(() => marksCalculator.Calc(null, 0, true)).IgnoreArguments();
            Expect.Call(repository.DbContext).Return(context);
            Expect.Call(() => context.CommitChanges());

            mocks.ReplayAll();

            MarksOivController controller = new MarksOivController(extension, repository, marksRepository, regionRepository, statusRepository, dataInitializer, marksCalculator);
            var result = controller.Save(new[] { Resources.Resources.OivStoreSaveJSON }, false);

            mocks.VerifyAll();

            Assert.IsAssignableFrom(typeof(AjaxStoreResult), result);
            Assert.AreEqual(StoreResponseFormat.Save, ((AjaxStoreResult)result).ResponseFormat);
        }

        [Test]
        public void LoadMarkDescriptionTest()
        {
            var okei = new D_Units_OKEI { Name = "okei" };
            var mark = new D_OMSU_MarksOMSU { CodeRepDouble = "1", Description = "Descr", CalcMark = "cm", InfoSource = "is", RefOKEI = okei };
            Expect.Call(marksRepository.FindOne(1)).Return(mark);

            mocks.ReplayAll();

            MarksOivController controller = new MarksOivController(extension, repository, marksRepository, regionRepository, statusRepository, dataInitializer, marksCalculator);
            var result = controller.LoadMarkDescription(1);

            mocks.VerifyAll();

            Assert.IsAssignableFrom(typeof(AjaxResult), result);
            Assert.AreEqual(Resources.Resources.MarkDescriptionResult, ((AjaxResult)result).Script);
        }
    }
}
