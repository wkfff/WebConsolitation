using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.MarksOMSU.Params;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Tests
{
    [TestFixture]
    public class MarksOmsuExtensionTests
    {
        private MockRepository mocks;
        private IScheme scheme;
        private IUsersManager usersManager;
        private IRepository<Users> userRepository;
        private IRepository<FX_Date_YearDayUNV> yearRepository;
        private ILinqRepository<D_Regions_Analysis> regionRepository;
        private ILinqRepository<DataSources> sourceRepository;
        private ILinqRepository<D_OMSU_ResponsOIVUser> oivUserRepository;
        private ILinqRepository<D_OMSU_ResponsOIV> oivRepository;
        private Users userEditor;
        private Users userWatcher;
        private Users userOther;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            scheme = mocks.DynamicMock<IScheme>();
            usersManager = mocks.DynamicMock<IUsersManager>();
            userRepository = mocks.DynamicMock<IRepository<Users>>();
            yearRepository = mocks.DynamicMock<IRepository<FX_Date_YearDayUNV>>();
            regionRepository = mocks.DynamicMock<ILinqRepository<D_Regions_Analysis>>();
            sourceRepository = mocks.DynamicMock<ILinqRepository<DataSources>>();
            oivUserRepository = mocks.DynamicMock<ILinqRepository<D_OMSU_ResponsOIVUser>>();
            oivRepository = mocks.DynamicMock<ILinqRepository<D_OMSU_ResponsOIV>>();

            Expect.Call(scheme.UsersManager).Return(usersManager);

            this.userOther = new Users { ID = 1, RefRegion = 1 };
            this.userWatcher = new Users { ID = 2, RefRegion = 1 };
            this.userEditor = new Users { ID = 3, RefRegion = 1 };

            Expect.Call(yearRepository.Get((2010 * 10000) + 1)).Return(new FX_Date_YearDayUNV { ID = (2010 * 10000) + 1 });
            Expect.Call(yearRepository.Get((2009 * 10000) + 1)).Return(new FX_Date_YearDayUNV { ID = (2009 * 10000) + 1 });
            Expect.Call(regionRepository.FindOne(1)).Return(new D_Regions_Analysis { ID = 1, SourceID = 2 }).Repeat.Any();
            var dataSourceses = new List<DataSources>
            {
                new DataSources { ID = 1, SupplierCode = "РЕГИОН", DataCode = 16, Year = "2010", Deleted = "0" },
                new DataSources { ID = 2, SupplierCode = "ФО", DataCode = 6, Year = "2010", Deleted = "0" }                                               
            };
            Expect.Call(sourceRepository.FindAll()).Return(dataSourceses.AsQueryable()).Repeat.Any();

            var oiv = new D_OMSU_ResponsOIV { ID = 1, Name = "Test" };
            var responsOivUsers = new List<D_OMSU_ResponsOIVUser>
            {
                new D_OMSU_ResponsOIVUser { ID = 1, RefUser = 1, RefResponsOIV = oiv }
            };
            Expect.Call(oivUserRepository.FindAll()).Return(responsOivUsers.AsQueryable());
            Expect.Call(oivRepository.FindAll()).Return(new List<D_OMSU_ResponsOIV> { oiv } .AsQueryable());
        }

        [Test]
        public void InitializeTest()
        {
            Expect.Call(userRepository.Get(1)).Return(userOther);
            Expect.Call(usersManager.GetCurrentUserID()).Return(1);
            mocks.ReplayAll();
            
            MarksOmsuExtension ext = new MarksOmsuExtension(scheme, userRepository, yearRepository, regionRepository, sourceRepository, oivUserRepository, oivRepository);
            var result = ext.Initialize();
            mocks.VerifyAll();

            Assert.True(result);
            Assert.AreEqual(2010, ext.CurrentYear);
            Assert.AreEqual(20100001, ext.CurrentYearUNV.ID);
            Assert.AreEqual(20090001, ext.PreviousYearUNV.ID);
            Assert.AreEqual(1, ext.DataSourceOmsu.ID);
            Assert.IsNull(ext.DataSourceOmsuPrevious);
            Assert.AreEqual("Test", ext.ResponsOIV.Name);
            Assert.AreEqual(1, ext.User.ID);
            Assert.AreEqual(1, ext.UserRegionCurrent.ID);
        }

        #region userOther

        [Test]
        [Ignore] // Note: Для тестирования требуется HTTP-сессия, актуально заполненная данными о пользователе.
        public void UserOtherCanNotSeeIneffGkhExpenses()
        {
            Expect.Call(userRepository.Get(1)).Return(userOther);
            Expect.Call(usersManager.GetCurrentUserID()).Return(1);
            mocks.ReplayAll();
            MarksOmsuExtension ext = new MarksOmsuExtension(scheme, userRepository, yearRepository, regionRepository, sourceRepository, oivUserRepository, oivRepository);
            ext.Initialize();

            var result = new UserCanSeeIneffExpenses().GetValue();
            mocks.VerifyAll();

            Assert.AreNotEqual(result, "1");
            Assert.AreEqual(result, "0");
        }

        #endregion

        #region userWatcher

        [Test]
        [Ignore] // Note: Для тестирования требуется HTTP-сессия, актуально заполненная данными о пользователе.
        public void UserWatcherCanSeeIneffGkhExpenses()
        {
            Expect.Call(userRepository.Get(2)).Return(userWatcher);
            Expect.Call(usersManager.GetCurrentUserID()).Return(2);
            mocks.ReplayAll();
            MarksOmsuExtension ext = new MarksOmsuExtension(scheme, userRepository, yearRepository, regionRepository, sourceRepository, oivUserRepository, oivRepository);
            ext.Initialize();

            var result = new UserCanSeeIneffExpenses().GetValue();
            mocks.VerifyAll();

            Assert.AreNotEqual(result, "0");
            Assert.AreEqual(result, "1");
        }

        #endregion

        #region userEditor

        [Test]
        [Ignore] // Note: Для тестирования требуется HTTP-сессия, актуально заполненная данными о пользователе.
        public void UserEditorCanSeeIneffGkhExpenses()
        {
            Expect.Call(userRepository.Get(3)).Return(userEditor);
            Expect.Call(usersManager.GetCurrentUserID()).Return(3);
            mocks.ReplayAll();
            MarksOmsuExtension ext = new MarksOmsuExtension(scheme, userRepository, yearRepository, regionRepository, sourceRepository, oivUserRepository, oivRepository);
            ext.Initialize();

            var result = new UserCanSeeIneffExpenses().GetValue();
            mocks.VerifyAll();

            Assert.AreNotEqual(result, "0");
            Assert.AreEqual(result, "1");
        }

        #endregion
    }
}
