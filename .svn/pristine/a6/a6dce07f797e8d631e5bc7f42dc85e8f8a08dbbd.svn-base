using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.DebtBook.Tests;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controllers;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Tests.Controllers
{
    [TestFixture]
    public class MarksOmsuControllerTests
    {
        private MockRepository mocks;
        private IMarksOmsuExtension extension;
        private IMarksOmsuRepository marksOmsuRepository;
        private IMarksRepository marksRepository;
        private IRepository<FX_OMSU_StatusData> statusRepository;
        private IMarksCalculator marksCalculator;
        private IDbContext context;
        private List<F_OMSU_Reg16> data;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            extension = mocks.DynamicMock<IMarksOmsuExtension>();
            marksOmsuRepository = mocks.DynamicMock<IMarksOmsuRepository>();
            marksRepository = mocks.DynamicMock<IMarksRepository>();
            statusRepository = mocks.DynamicMock<IRepository<FX_OMSU_StatusData>>();
            marksCalculator = mocks.DynamicMock<IMarksCalculator>();
            context = mocks.DynamicMock<IDbContext>();

            var mark = new D_OMSU_MarksOMSU { ID = 1, RefOKEI = new D_Units_OKEI { ID = 1, Symbol = "Sy" } };
            var region = new D_Regions_Analysis { ID = 1 };
            var status = new FX_OMSU_StatusData { ID = 1 };
            var year = new FX_Date_YearDayUNV { ID = 20100000 };
            data = new List<F_OMSU_Reg16>
            {
                new F_OMSU_Reg16 { ID = 1, RefMarksOMSU = mark, RefRegions = region, RefStatusData = status, RefYearDayUNV = year },
                new F_OMSU_Reg16 { ID = 2, RefMarksOMSU = mark, RefRegions = region, RefStatusData = status, RefYearDayUNV = year }
            };
        }

        [Test]
        public void SaveTest()
        {
            Expect.Call(extension.DataSourceOmsu).Return(new DataSources { ID = 1 });
            Expect.Call(extension.CurrentYearUNV).Return(new FX_Date_YearDayUNV { ID = 20100000 });
            Expect.Call(extension.UserRegionCurrent).Return(new D_Regions_Analysis { ID = 1 }).Repeat.Any();
            Expect.Call(() => marksOmsuRepository.Save(null)).IgnoreArguments();
            Expect.Call(() => marksCalculator.Calc(null, 0, true)).IgnoreArguments();
            Expect.Call(marksOmsuRepository.DbContext).Return(context);
            Expect.Call(() => context.CommitChanges());

            mocks.ReplayAll();

            MarksOmsuController controller = new MarksOmsuController(extension, new MarksOmsuGridControl(extension), marksOmsuRepository, marksRepository, statusRepository, null, marksCalculator);
            var result = controller.Save(new[] { Resources.Resources.OivStoreSaveJSON });

            mocks.VerifyAll();

            Assert.IsAssignableFrom(typeof(AjaxStoreResult), result);
            Assert.AreEqual(StoreResponseFormat.Save, ((AjaxStoreResult)result).ResponseFormat);
        }

        [Test]
        [Ignore]
        public void ExpandDescriptionTest()
        {
            var region = new D_Regions_Analysis { ID = 1 };
            Expect.Call(extension.UserRegionCurrent).Return(region).Repeat.Any();
            Expect.Call(marksOmsuRepository.GetForMO(region, 1)).Return(new List<F_OMSU_Reg16>());
            
            var okei = new D_Units_OKEI { Name = "okei" };
            var oiv = new D_OMSU_ResponsOIV { Name = "oiv" };
            var mark = new D_OMSU_MarksOMSU { CodeRepDouble = "1", Description = "Descr", CalcMark = "cm", InfoSource = "is", RefOKEI = okei, RefResponsOIV = oiv };
            Expect.Call(marksRepository.FindOne(1)).Return(mark);

            mocks.ReplayAll();

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            HttpContext.Current.Request.Browser = new HttpBrowserCapabilities();

            var resourceManager = new Ext.Net.ResourceManager();
            ViewPage page = new ViewPage();
            resourceManager.Page = page;

            context.Context.Handler = page;

            HttpContext.Current.Items.Add(typeof(Ext.Net.ResourceManager), resourceManager);

            page.Items.Add(typeof(Ext.Net.ResourceManager), resourceManager);

            MarksOmsuController controller = new MarksOmsuController(extension, new MarksOmsuGridControl(extension), marksOmsuRepository, marksRepository, statusRepository, null, null);
            var result = controller.Expand(1, 1, true, null);

            mocks.VerifyAll();
        }
    }
}
