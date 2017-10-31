using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Forms.Org3PricesAndTariffs
{
    [TestFixture]
    public class FactServiceTests
    {
        private static D_Regions_Analysis regionsAnalysis = new D_Regions_Analysis { ID = 1, RefBridgeRegions = new B_Regions_Bridge { ID = 1 } };

        private static D_CD_Templates template = new D_CD_Templates { ID = 1 };

        private static D_CD_Subjects subject = new D_CD_Subjects { ID = 1, RefRegion = regionsAnalysis };

        private static IList<D_CD_Task> tasks = new List<D_CD_Task>
            {
                new D_CD_Task { ID = 1, BeginDate = new DateTime(2012, 12, 24), RefTemplate = template, RefStatus = new FX_FX_FormStatus { ID = (int)TaskViewModel.TaskStatus.Edit }, RefSubject = subject },
                new D_CD_Task { ID = 2, BeginDate = new DateTime(2012, 11, 24), RefTemplate = template, RefStatus = new FX_FX_FormStatus { ID = (int)TaskViewModel.TaskStatus.Edit }, RefSubject = subject }
            };
        
        private static IList<D_Org_Report> reports = new List<D_Org_Report>
                                                         {
                                                             new D_Org_Report { ID = 1, RefTask = tasks[0] },
                                                             new D_Org_Report { ID = 2, RefTask = tasks[1] }
                                                         };
        
        private static IList<FX_Org_ProdGroup> prodGroups = new List<FX_Org_ProdGroup>
                                                         {
                                                             new FX_Org_ProdGroup { ID = (int)GoodType.Gasoline }
                                                         };

        private static IList<D_Org_RegistrOrg> org = new List<D_Org_RegistrOrg>
            {
                new D_Org_RegistrOrg { ID = 1, NameOrg = "Заправка Чака Норриса", RefRegionAn = regionsAnalysis, RefProdGroup = prodGroups[0] },
                new D_Org_RegistrOrg { ID = 2, NameOrg = "Заправка дяди Васи", RefRegionAn = regionsAnalysis, RefProdGroup = prodGroups[0] },
                new D_Org_RegistrOrg { ID = 3, NameOrg = "Заправка новая", RefRegionAn = regionsAnalysis, SignCN = true, RefProdGroup = prodGroups[0] }
            };

        private static IList<D_Org_Good> goods = new List<D_Org_Good>
            {
                new D_Org_Good { ID = 1, Name = "Бензин АИ-98 с закисью азота", Code = 1, RefProdGroup = prodGroups[0] },
                new D_Org_Good { ID = 2, Name = "Соляра от трактора", Code = 2, RefProdGroup = prodGroups[0] }
            };

        private MockRepository mocks;
        private ILinqRepository<T_Org_CPrice> factRepository;
        private ILinqRepository<D_Org_RegistrOrg> organizationRepository;
        private ILinqRepository<D_Org_Good> goodRepository;
        private ILinqRepository<D_Org_Report> reportRepository;
        private ILinqRepository<D_CD_Task> taskRepository;
        private ITaskService taskService;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();

            factRepository = mocks.DynamicMock<ILinqRepository<T_Org_CPrice>>();
            organizationRepository = mocks.DynamicMock<ILinqRepository<D_Org_RegistrOrg>>();
            goodRepository = mocks.DynamicMock<ILinqRepository<D_Org_Good>>();
            reportRepository = mocks.DynamicMock<ILinqRepository<D_Org_Report>>();
            taskService = mocks.DynamicMock<ITaskService>();
            taskRepository = mocks.DynamicMock<ILinqRepository<D_CD_Task>>();
            
            IList<T_Org_CPrice> facts = new List<T_Org_CPrice>
            {
                new T_Org_CPrice { ID = 1, RefReport = reports[0], RefRegistrOrg = org[0], RefGood = goods[0], Price = 10 },
                new T_Org_CPrice { ID = 2, RefReport = reports[0], RefRegistrOrg = org[1], RefGood = goods[1], Price = 10 },
                new T_Org_CPrice { ID = 3, RefReport = reports[0], RefRegistrOrg = org[0], RefGood = goods[1], Price = 100 },
                new T_Org_CPrice { ID = 4, RefReport = reports[0], RefRegistrOrg = org[1], RefGood = goods[0], Price = 100 },
                new T_Org_CPrice { ID = 5, RefReport = reports[1], RefRegistrOrg = org[0], RefGood = goods[0], Price = 1 },
            };

            Expect.Call(factRepository.FindAll()).Return(facts.AsQueryable()).Repeat.Any();
        }

        [Test]
        public void LoadFactDataTest()
        {
            mocks.ReplayAll();

            var factService = new FactService(factRepository, null, null, null, null, null, null, null, null, null, null);

            var result = factService.LoadFactData(tasks[0].ID);

            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Count.Should().Be(4);
            result.Should().Contain(x => x.RefGood == goods[0].ID);
            result.Should().Contain(x => x.RefGood == goods[1].ID);
            result.Should().Contain(x => x.RefRegistrOrg == org[0].ID);
            result.Should().Contain(x => x.RefRegistrOrg == org[1].ID);
        }

        [Test]
        public void GetInitialDataTest()
        {
            var task = tasks[0];
            var taskModel = new TaskViewModel
                           {
                               ID = task.ID, 
                               RefStatus = (int)TaskViewModel.TaskStatus.Edit,
                               Region = regionsAnalysis
                           };
            Expect.Call(taskService.GetTaskViewModel(task.ID)).Return(taskModel).Repeat.Any();
            
            Expect.Call(organizationRepository.FindAll()).Return(org.AsQueryable()).Repeat.Any();
            Expect.Call(goodRepository.FindAll()).Return(goods.AsQueryable()).Repeat.Any();
            
            mocks.ReplayAll();
            
            var factService = new FactService(factRepository, organizationRepository, null, goodRepository, reportRepository, null, taskService, null, null, null, null);
            var result = factService.GetInitialData(task.ID, GoodType.Gasoline);
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Count.Should().Be(6);
            result.Should().Contain(x => x.RefRegistrOrg == org[2].ID);
        }

        [Test]
        public void GetOrganizationsTest()
        {
            var task = tasks[0];
            var taskModel = new TaskViewModel
            {
                ID = task.ID,
                RefStatus = (int)TaskViewModel.TaskStatus.Edit,
                Region = regionsAnalysis
            };
            Expect.Call(taskService.GetTaskViewModel(task.ID)).Return(taskModel).Repeat.Any();

            Expect.Call(organizationRepository.FindAll()).Return(org.AsQueryable()).Repeat.Any();

            mocks.ReplayAll();

            var factService = new FactService(factRepository, organizationRepository, null, goodRepository, reportRepository, null, taskService, null, null, null, null);
            var result = factService.GetOrganizations(task.ID, null, GoodType.Gasoline);
            
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Count.Should().Be(3);

            result[0].GetType().GetProperty("ID").PropertyType.FullName.Should().Be(typeof(int).FullName);
            result[0].GetType().GetProperty("Name").PropertyType.FullName.Should().Be(typeof(string).FullName);
            result[0].GetType().GetProperty("IsMarketGrid").PropertyType.FullName.Should().Be(typeof(bool).FullName);

            result[2].GetType().GetProperty("IsMarketGrid").GetValue(result[2], null).Should().Be(true);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateDataTest()
        {
            Expect.Call(reportRepository.FindAll()).Return(reports.AsQueryable()).Repeat.Any();
            
            IDbContext databaseContext = mocks.DynamicMock<IDbContext>();
            Expect.Call(databaseContext.CommitChanges).Repeat.Once();
            Expect.Call(factRepository.DbContext).Return(databaseContext).Repeat.Any();

            ////Expect.Call(() => factRepository.Save(null)).IgnoreArguments().Repeat.Once();

            Expect.Call(organizationRepository.FindOne(org[0].ID)).Return(org[0]).Repeat.Any();
            Expect.Call(organizationRepository.FindOne(org[1].ID)).Return(org[1]).Repeat.Any();
            Expect.Call(goodRepository.FindOne(goods[0].ID)).Return(goods[0]).Repeat.Any();

            mocks.ReplayAll();

            var factService = new FactService(factRepository, organizationRepository, null, goodRepository, reportRepository, null, taskService, null, null, null, null);

            FormModel[] rows = new[]
                                   {
                                       new FormModel { ID = 1, RefRegistrOrg = org[0].ID, RefGood = goods[0].ID, Price = (decimal?)0.1 },
                                       new FormModel { ID = 2, RefRegistrOrg = org[1].ID, RefGood = goods[0].ID, Price = (decimal?)0.1 }
                                   };

            factService.CreateData(tasks[0].ID, rows);

            mocks.VerifyAll();
        }

        [Test]
        public void UpdateDataTest()
        {
            Expect.Call(reportRepository.FindAll()).Return(reports.AsQueryable()).Repeat.Any();

            IDbContext databaseContext = mocks.DynamicMock<IDbContext>();
            Expect.Call(databaseContext.CommitChanges).Repeat.Once();
            Expect.Call(factRepository.DbContext).Return(databaseContext).Repeat.Any();

            ////Expect.Call(() => factRepository.Save(null)).IgnoreArguments().Repeat.Once();

            Expect.Call(organizationRepository.FindOne(org[0].ID)).Return(org[0]).Repeat.Any();
            Expect.Call(organizationRepository.FindOne(org[1].ID)).Return(org[1]).Repeat.Any();
            Expect.Call(goodRepository.FindOne(goods[0].ID)).Return(goods[0]).Repeat.Any();

            mocks.ReplayAll();

            var factService = new FactService(factRepository, organizationRepository, null, goodRepository, reportRepository, null, taskService, null, null, null, null);

            FormModel[] rows = new[]
                                   {
                                       new FormModel { ID = 1, RefRegistrOrg = org[0].ID, RefGood = goods[0].ID, Price = (decimal?)0.1 },
                                       new FormModel { ID = 2, RefRegistrOrg = org[1].ID, RefGood = goods[0].ID, Price = (decimal?)0.1 }
                                   };

            factService.UpdateData(tasks[0].ID, rows);

            mocks.VerifyAll();
        }

        [Test]
        public void IncludeOrganizationTest()
        {
            Expect.Call(reportRepository.FindAll()).Return(reports.AsQueryable()).Repeat.Any();
            Expect.Call(organizationRepository.FindOne(org[2].ID)).Return(org[2]).Repeat.Any();
            Expect.Call(goodRepository.FindAll()).Return(goods.AsQueryable()).Repeat.Any();

            IDbContext databaseContext = mocks.DynamicMock<IDbContext>();
            Expect.Call(databaseContext.CommitChanges).Repeat.Once();
            Expect.Call(factRepository.DbContext).Return(databaseContext).Repeat.Any();

            mocks.ReplayAll();

            var factService = new FactService(factRepository, organizationRepository, null, goodRepository, reportRepository, null, taskService, null, null, null, null);
            factService.IncludeOrganization(tasks[0].ID, org[2].ID);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateAndIncludeOrganizationTest()
        {
            Expect.Call(reportRepository.FindAll()).Return(reports.AsQueryable()).Repeat.Any();
            Expect.Call(organizationRepository.FindAll()).Return(org.AsQueryable()).Repeat.Any();
            Expect.Call(organizationRepository.FindOne(org[2].ID)).Return(org[2]).Repeat.Any();
            Expect.Call(goodRepository.FindAll()).Return(goods.AsQueryable()).Repeat.Any();

            IDbContext databaseContext = mocks.DynamicMock<IDbContext>();
            Expect.Call(databaseContext.CommitChanges).Repeat.Once();
            Expect.Call(organizationRepository.DbContext).Return(databaseContext).Repeat.Any();
            Expect.Call(factRepository.DbContext).Return(databaseContext).Repeat.Any();

            ILinqRepository<FX_Org_ProdGroup> prodGroupRepository = mocks.DynamicMock<ILinqRepository<FX_Org_ProdGroup>>();
            ILinqRepository<D_OK_OKOPF> okokopfRepository = mocks.DynamicMock<ILinqRepository<D_OK_OKOPF>>();
            ILinqRepository<D_OK_OKFS> okokfsRepository = mocks.DynamicMock<ILinqRepository<D_OK_OKFS>>();
            ILinqRepository<D_Org_TypeOrg> orgtypeorgRepository = mocks.DynamicMock<ILinqRepository<D_Org_TypeOrg>>();
            ILinqRepository<B_Org_OrgBridge> orgbridgeRepository = mocks.DynamicMock<ILinqRepository<B_Org_OrgBridge>>();

            Expect.Call(prodGroupRepository.FindOne((int)GoodType.Gasoline)).Return(new FX_Org_ProdGroup { ID = 2 }).Repeat.Any();
            Expect.Call(okokopfRepository.FindOne(-1)).Return(null).Repeat.Any();
            Expect.Call(okokfsRepository.FindOne(-1)).Return(null).Repeat.Any();
            Expect.Call(orgtypeorgRepository.FindOne(-1)).Return(null).Repeat.Any();
            Expect.Call(orgbridgeRepository.FindOne(-1)).Return(null).Repeat.Any();
            
            mocks.ReplayAll();

            var factService = new FactService(factRepository, organizationRepository, prodGroupRepository, goodRepository, reportRepository, null, taskService, okokopfRepository, okokfsRepository, orgtypeorgRepository, orgbridgeRepository);

            Action act = () => factService.CreateAndIncludeOrganization(tasks[0].ID, org[1].NameOrg, false, GoodType.Gasoline);

            act.ShouldThrow<Exception>().WithMessage("Организация с такими параметрами уже существует");

            factService.CreateAndIncludeOrganization(tasks[0].ID, "Нет такой организации", true, GoodType.Gasoline);

            mocks.ReplayAll();
        }

        [Test]
        public void ExcludeOrganizationTest()
        {
            Expect.Call(reportRepository.FindAll()).Return(reports.AsQueryable()).Repeat.Any();
            Expect.Call(organizationRepository.FindOne(org[0].ID)).Return(org[0]).Repeat.Any();
            
            IDbContext databaseContext = mocks.DynamicMock<IDbContext>();
            Expect.Call(databaseContext.CommitChanges).Repeat.Once();
            Expect.Call(factRepository.DbContext).Return(databaseContext).Repeat.Any();

            mocks.ReplayAll();

            var factService = new FactService(factRepository, organizationRepository, null, goodRepository, reportRepository, null, taskService, null, null, null, null);
            factService.ExcludeOrganization(tasks[0].ID, org[0].ID);

            mocks.VerifyAll();
        }

        [Test]
        public void GetOldTaskDatesTest()
        {
            var task = tasks[0];
            Expect.Call(taskRepository.FindAll()).Return(tasks.AsQueryable()).Repeat.Any();
            Expect.Call(taskRepository.FindOne(task.ID)).Return(task).Repeat.Any();

            mocks.ReplayAll();

            var factService = new FactService(factRepository, organizationRepository, null, goodRepository, reportRepository, taskRepository, taskService, null, null, null, null);
            var result = factService.GetOldTaskDates(task.ID);

            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Count.Should().Be(1);

            result[0].GetType().GetProperty("ID").PropertyType.FullName.Should().Be(typeof(int).FullName);
            result[0].GetType().GetProperty("ReportDate").PropertyType.FullName.Should().Be(typeof(string).FullName);
            
            result[0].GetType().GetProperty("ReportDate").GetValue(result[0], null).Should().Be("24.11.2012");

            mocks.VerifyAll();
        }

        [Test]
        public void CopyFromTaskTest()
        {
            Expect.Call(taskRepository.FindAll()).Return(tasks.AsQueryable()).Repeat.Any();
            Expect.Call(reportRepository.FindAll()).Return(reports.AsQueryable()).Repeat.Any();
            
            IDbContext databaseContext = mocks.DynamicMock<IDbContext>();
            Expect.Call(databaseContext.CommitChanges).Repeat.Once();
            Expect.Call(factRepository.DbContext).Return(databaseContext).Repeat.Any();

            mocks.ReplayAll();

            var factService = new FactService(factRepository, organizationRepository, null, goodRepository, reportRepository, taskRepository, taskService, null, null, null, null);
            factService.CopyFromTask(tasks[0].ID, tasks[1].ID);
            
            mocks.VerifyAll();
        }
    }
}
