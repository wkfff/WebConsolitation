using System.Collections.Generic;
using System.IO;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Tests
{
    [TestFixture]
    public class ExportServiceTests
    {
        [Test]
        public void CanExportForOMSUTest()
        {
            MockRepository mocks = new MockRepository();
            var repository = mocks.DynamicMock<IMarksOmsuRepository>();
            var regionsRepository = mocks.DynamicMock<IRegionsRepository>();
            var extensionService = mocks.DynamicMock<IMarksOmsuExtension>();

            var region = new D_Regions_Analysis { ID = 1 };
            var mark = new D_OMSU_MarksOMSU 
            { 
                ID = 1, 
                RefCompRep = new D_OMSU_CompRep { Name = "Test" },
                RefOKEI = new D_Units_OKEI { Symbol = "Test" }
            };

            Expect.Call(extensionService.CurrentYear).Return(2011);
            Expect.Call(regionsRepository.FindOne(1)).Return(region);
            Expect.Call(repository.GetAllMarksForMO(region)).Return(new List<F_OMSU_Reg16>
            {
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 0 },
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 1 },
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 2 }
            });

            mocks.ReplayAll();

            var stream = new ExportService(repository, regionsRepository, extensionService, null, null)
                .ExportForOmsu(1, 1);

            mocks.VerifyAll();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            /*using (var file = File.Create(@"d:\test.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }

        [Test]
        public void CanExportForOIVTest()
        {
            MockRepository mocks = new MockRepository();
            var repository = mocks.DynamicMock<IMarksOmsuRepository>();
            var extensionService = mocks.DynamicMock<IMarksOmsuExtension>();
            var marksRepository = mocks.DynamicMock<IMarksRepository>();

            var region = new D_Regions_Analysis { ID = 1 };
            var mark = new D_OMSU_MarksOMSU 
            { 
                ID = 1, 
                RefCompRep = new D_OMSU_CompRep { Name = "Test" },
                RefOKEI = new D_Units_OKEI { Symbol = "Test" }
            };

            Expect.Call(extensionService.CurrentYear).Return(2011);
            Expect.Call(repository.GetForOIV(1)).Return(new List<F_OMSU_Reg16>
            {
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 0 },
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 1 },
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 2 }
            });
            Expect.Call(marksRepository.FindOne(1)).Return(mark);

            mocks.ReplayAll();

            var stream = new ExportService(repository, null, extensionService, marksRepository, null)
                .ExportForOiv(1, 1);

            mocks.VerifyAll();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
        }

        [Test]
        public void CanExportOivInputDataTest()
        {
            MockRepository mocks = new MockRepository();
            var repository = mocks.DynamicMock<IMarksOmsuRepository>();
            var extensionService = mocks.DynamicMock<IMarksOmsuExtension>();
            var marksRepository = mocks.DynamicMock<IMarksRepository>();

            var region = new D_Regions_Analysis { ID = 1 };
            var mark = new D_OMSU_MarksOMSU
            {
                ID = 1,
                RefCompRep = new D_OMSU_CompRep { Name = "Test" },
                RefOKEI = new D_Units_OKEI { Symbol = "Test" }
            };

            Expect.Call(extensionService.CurrentYear).Return(2011);
            Expect.Call(repository.GetForOIV(1)).Return(new List<F_OMSU_Reg16>
            {
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 0 },
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 1 },
                new F_OMSU_Reg16 { ID = 1, RefRegions = region, RefMarksOMSU = mark, Prognoz1 = 2 }
            });
            Expect.Call(marksRepository.FindOne(1)).Return(mark);

            mocks.ReplayAll();

            var stream = new ExportService(repository, null, extensionService, marksRepository, null)
                .ExportOivInputData(1, 1);

            mocks.VerifyAll();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
        }

        [Ignore]
        [Test]
        public void WithDbTest()
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                "Password=dv;Persist Security Info=True;User ID=HMAOACR;Initial Catalog=HMAOACR;Data Source=mzserv\\mssql2008",
                "SQL",
                "10");

            MockRepository mocks = new MockRepository();
            var extension = mocks.DynamicMock<IMarksOmsuExtension>();

            Expect.Call(extension.CurrentYear).Return(2010).Repeat.Any();
            Expect.Call(extension.CurrentYearUNV).Return(new FX_Date_YearDayUNV { ID = 20100001 }).Repeat.Any();
            Expect.Call(extension.UserRegionCurrent).Return(new D_Regions_Analysis { ID = 8227 }).Repeat.Any();
            Expect.Call(extension.DataSourceOmsu).Return(new DataSources { ID = 8 }).Repeat.Any();
            mocks.ReplayAll();

            IRegionsRepository regionsRepository = new RegionsRepository(new NHibernateLinqRepository<D_Regions_Analysis>(), extension);
            IMarksRepository marksRepository = new MarksRepository(new NHibernateLinqRepository<D_OMSU_MarksOMSU>(), extension);
            
            MarksOmsuRepository factRepository = new MarksOmsuRepository(
                new NHibernateLinqRepository<F_OMSU_Reg16>(),
                extension,
                marksRepository,
                regionsRepository,
                new NHibernateRepository<FX_OMSU_StatusData>());

            var service = new ExportService(factRepository, regionsRepository, extension, null, null);

            /*var stream = service.ExportForOmsu(1, 8227);*/
            var stream = service.ExportForOiv(1, 1);

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            using (var file = File.Create(@"d:\test.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
