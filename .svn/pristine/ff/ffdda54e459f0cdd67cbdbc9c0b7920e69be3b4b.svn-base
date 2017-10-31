using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.OrgGKH.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.OrgGKH.Tests
{
    [TestFixture]
    public class ExportServiceTests
    {
        private MockRepository mocks;
        private ILinqRepository<D_Org_RegistrOrg> orgRepository;
        private IRepository<D_Org_MarksGKH> marksRepository;
        private ILinqRepository<F_Org_GKH> planRepository;
        private ILinqRepository<FX_Org_StatusD> statusRepository;
        
        [Test]
        public void CanExportMonthTest()
        {
            var orgId = 1;
            var periodId = 20120117;

            PrepareData(orgId, periodId);

            mocks.ReplayAll();
            var stream = new ExportService(marksRepository, planRepository, orgRepository, statusRepository)
                .ExportMonth(orgId, periodId, 1, "some terr");

            mocks.VerifyAll();

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            /*using (var file = File.Create(@"x:\Exports\testOrgGkhMonth.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }

       [Test]
        public void CanExportWeekTest()
        {
            var orgId = 1;
            var periodId = 20120202;

            PrepareData(orgId, periodId);

            mocks.ReplayAll();
            var stream = new ExportService(marksRepository, planRepository, orgRepository, statusRepository)
                .ExportWeek(orgId, periodId, 1, null);

            mocks.VerifyAll();

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            /*using (var file = File.Create(@"x:\Exports\testOrgGkhWeek.xls"))
            {
                file.Write(buffer, 0, buffer.Length);
            }*/
        }

       private void PrepareData(int orgId, int periodId)
       {
           mocks = new MockRepository();
           orgRepository = mocks.DynamicMock<ILinqRepository<D_Org_RegistrOrg>>();
           marksRepository = mocks.DynamicMock<IRepository<D_Org_MarksGKH>>();
           planRepository = mocks.DynamicMock<ILinqRepository<F_Org_GKH>>();
           statusRepository = mocks.DynamicMock<ILinqRepository<FX_Org_StatusD>>();

           var region = new D_Regions_Analysis { ID = 1, Name = "Регион" };
           var org = new D_Org_RegistrOrg
           {
               ID = orgId,
               NameOrg = "Организация",
               RefRegionAn = region
           };

           var period = new FX_Date_YearDayUNV { ID = periodId };

           var marks = new List<D_Org_MarksGKH>();
           for (var i = 10; i > 0; i--)
           {
               var mark = new D_Org_MarksGKH
               {
                   ID = i,
                   Name = "Показатель c очень-очень длинным таким названием, для того, чтобы можно было проверить, работает ли перенос слов" + i,
                   Code = i * 100,
                   PrAssigned = "AD",
                   PrAssignedOP = "AD",
                   PrPerformed = "AD",
                   PrPerformedOP = "AD",
                   PrPlanO = "AD",
                   PrPlanOOP = "AD",
                   PrPlanS = "AD",
                   PrPlanSOP = "AD",
                   PrPlanY = "AD"
               };
               marks.Add(mark);
           }

           Expect.Call(marksRepository.GetAll()).Return(marks);

           var status = new FX_Org_StatusD
           {
               ID = 1,
               Name = "На редактировании"
           };

           var orgPlanList = new List<F_Org_GKH>();
           for (var i = 10; i > 0; i--)
           {
               var orgPlan = new F_Org_GKH
               {
                   ID = i,
                   Assigned = i,
                   AssignedOP = i,
                   Performed = i,
                   PerformedOP = i,
                   PlanO = i * 100,
                   PlanOOP = i * 1000,
                   PlanS = i * 10000,
                   PlanSOP = i * 100000,
                   RefMarksGKH = marks[i - 1],
                   RefRegistrOrg = org,
                   RefYearDayUNV = period,
                   SourceID = 1,
                   RefStatusD = status,
                   Value = i
               };
               orgPlanList.Add(orgPlan);
           }

           Expect.Call(planRepository.FindAll()).Return(orgPlanList.AsQueryable());
           Expect.Call(orgRepository.FindOne(orgId)).Return(org);
           Expect.Call(statusRepository.FindOne(status.ID)).Return(status);
       }
    }
}
