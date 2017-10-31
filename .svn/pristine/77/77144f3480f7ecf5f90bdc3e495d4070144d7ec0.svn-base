using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using FluentAssertions;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Core.Tests.Helpers;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Tests
{
    [TestFixture]
    public class ExtensionTests
    {
        private static B_Regions_Bridge userRegionBridge = new B_Regions_Bridge { ID = 1 };
        private static D_Territory_RF userTerritory = new D_Territory_RF { ID = 1, RefRegionsBridge = userRegionBridge };
        private static D_Territory_RF rootTerritory = new D_Territory_RF { ID = 2, RefRegionsBridge = null, Code = 1000 };
        private static FX_Date_Year currentYear = new FX_Date_Year { ID = 2011 };

        private MockRepository mocks;
        private IScheme scheme;
        private IGlobalConstsManager constsManager;
        private IRepository<FX_Date_Year> yearRepository;
        private ILinqRepository<D_Regions_Analysis> regionsRepository;
        private ILinqRepository<D_Territory_RF> territoryRepository;
        private ILinqRepository<DataSources> sourceRepository;
        private ILinqRepository<D_OMSU_ResponsOIVUser> oivUserRepository;
        private ILinqRepository<D_OMSU_ResponsOIV> oivRepository;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            scheme = mocks.DynamicMock<IScheme>();
            constsManager = mocks.DynamicMock<IGlobalConstsManager>();
            yearRepository = mocks.DynamicMock<IRepository<FX_Date_Year>>();
            regionsRepository = mocks.DynamicMock<ILinqRepository<D_Regions_Analysis>>();
            territoryRepository = mocks.DynamicMock<ILinqRepository<D_Territory_RF>>();
            sourceRepository = mocks.DynamicMock<ILinqRepository<DataSources>>();
            oivUserRepository = mocks.DynamicMock<ILinqRepository<D_OMSU_ResponsOIVUser>>();
            oivRepository = mocks.DynamicMock<ILinqRepository<D_OMSU_ResponsOIV>>();

            var refRegionAnalisys = new D_Regions_Analysis { ID = 1, RefBridgeRegions = userRegionBridge };
            var user = new Users { ID = 1, Name = "user", RefRegion = refRegionAnalisys.ID };

            var territories = new List<D_Territory_RF>
                                  {
                                      userTerritory,
                                      rootTerritory
                                  };
            var oiv = new D_OMSU_ResponsOIV { ID = 1, Name = "ОИВ" };
            var responsOivUsers = new List<D_OMSU_ResponsOIVUser>
            {
                new D_OMSU_ResponsOIVUser { ID = 1, RefUser = user.ID, RefResponsOIV = oiv }
            };

            MockHttpContext context = new MockHttpContext(false);
            HttpContext.Current = context.Context;
            HttpContext.Current.User = new BasePrincipal(new GenericIdentity(user.Name), new[] { OivRoles.OivAndOmsu }, user);

            string oktmoValue = "1 000";
            Expect.Call(scheme.GlobalConstsManager).Return(constsManager);
            IGlobalConstsCollection constCollection = mocks.DynamicMock<IGlobalConstsCollection>();
            Expect.Call(constsManager.Consts).Return(constCollection);
            IGlobalConst constOKTMO = mocks.DynamicMock<IGlobalConst>();
            Expect.Call(constOKTMO.Value).Return(oktmoValue);
            Expect.Call(constCollection["OKTMO"]).Return(constOKTMO);
            
            Expect.Call(oivUserRepository.FindAll()).Return(responsOivUsers.AsQueryable());
            Expect.Call(oivRepository.FindAll()).Return(new List<D_OMSU_ResponsOIV> { oiv } .AsQueryable());

            Expect.Call(regionsRepository.FindOne(refRegionAnalisys.ID)).Return(refRegionAnalisys);
            Expect.Call(territoryRepository.FindAll()).Return(territories.AsQueryable()).Repeat.Any();

            var sources = new List<DataSources>
                              {
                               new DataSources { ID = 1, DataCode = 6, SupplierCode = "ФО", KindsOfParams = "1", Deleted = "0" },
                               new DataSources { ID = 2, DataCode = 10, SupplierCode = "РЕГИОН", KindsOfParams = "1", Deleted = "0", Year = "2010" },
                               new DataSources { ID = 3, DataCode = 10, SupplierCode = "РЕГИОН", KindsOfParams = "1", Deleted = "0", Year = "2011" }
                              };

            Expect.Call(sourceRepository.FindAll()).Return(sources.AsQueryable()).Repeat.Any();
            Expect.Call(yearRepository.Get(2011)).Return(currentYear);
        }

        [Test]
        public void InitializeTest()
        {
            mocks.ReplayAll();

            var extension = new Region10MarksOivExtension(scheme, yearRepository, regionsRepository, territoryRepository, sourceRepository, oivUserRepository, oivRepository);
            
            bool result = extension.Initialize();
            
            result.Should().BeTrue();
            extension.CurrentYear.Should().Be(currentYear);
            extension.CurrentYearVal.Should().Be(2011);
            extension.DataSourceOiv.ShouldHave().Properties(x => x.ID, x => x.Year).EqualTo(new DataSources { ID = 3, Year = "2011" });
            extension.RootTerritoryRf.Should().Be(rootTerritory);
            extension.UserResponseOiv.Should().NotBeNull();
            extension.UserResponseOiv.Name.Should().Be("ОИВ");
            extension.UserTerritoryRf.Should().Be(userTerritory);
            extension.Years.Should().HaveCount(2)
                                    .And.Equal(2010, 2011);
            mocks.VerifyAll();
        }
    }
}
