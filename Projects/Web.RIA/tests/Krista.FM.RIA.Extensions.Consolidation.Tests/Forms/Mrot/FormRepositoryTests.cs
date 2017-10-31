using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Forms.Mrot
{
    [TestFixture]
    public class FormRepositoryTests
    {
        private MockRepository mocks;
        private ILinqRepository<F_Marks_MOFOTrihedralAgr> factRepository;
        private ILinqRepository<FX_OrgType_TrihedrAlagr> orgRepository;
        private ILinqRepository<FX_Marks_TrihedrAlagr> markRepository;
        private IRepository<FX_Date_YearDayUNV> periodRepository;
        private ILinqRepository<DataSources> datasourceRepository;
        private IRepository<D_CD_Task> taskRepository;
        private ILinqRepository<D_Report_TrihedrAgr> reportRepository;
        private D_CD_Task task;
        private D_Report_TrihedrAgr report;
        private D_Regions_Analysis region;
        private FX_Date_YearDayUNV period;
        private List<FX_OrgType_TrihedrAlagr> orgs;
        private List<FX_Marks_TrihedrAlagr> marks;
        private List<F_Marks_MOFOTrihedralAgr> facts;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            factRepository = mocks.DynamicMock<ILinqRepository<F_Marks_MOFOTrihedralAgr>>();
            orgRepository = mocks.DynamicMock<ILinqRepository<FX_OrgType_TrihedrAlagr>>();
            markRepository = mocks.DynamicMock<ILinqRepository<FX_Marks_TrihedrAlagr>>();
            periodRepository = mocks.DynamicMock<IRepository<FX_Date_YearDayUNV>>();
            datasourceRepository = mocks.DynamicMock<ILinqRepository<DataSources>>();
            taskRepository = mocks.DynamicMock<IRepository<D_CD_Task>>();
            reportRepository = mocks.DynamicMock<ILinqRepository<D_Report_TrihedrAgr>>();

            task = new D_CD_Task { ID = 1, BeginDate = new DateTime(2010, 1, 1) };
            report = new D_Report_TrihedrAgr { ID = 1, RefTask = task };
            region = new D_Regions_Analysis { ID = 1 };
            period = new FX_Date_YearDayUNV { ID = 20110000 };
            orgs = new List<FX_OrgType_TrihedrAlagr>
            {
                new FX_OrgType_TrihedrAlagr { ID = 1 },
                new FX_OrgType_TrihedrAlagr { ID = 2 },
                new FX_OrgType_TrihedrAlagr { ID = 3 },
            };

            marks = new List<FX_Marks_TrihedrAlagr>
            {
                new FX_Marks_TrihedrAlagr { ID = 1, Code = 10, Name = "10" },
                new FX_Marks_TrihedrAlagr { ID = 2, Code = 11, Name = "11" },
                new FX_Marks_TrihedrAlagr { ID = 3, Code = 12, Name = "12" },
                new FX_Marks_TrihedrAlagr { ID = 4, Code = 50, Name = "50" },
                new FX_Marks_TrihedrAlagr { ID = 5, Code = 51, Name = "51" },
                new FX_Marks_TrihedrAlagr { ID = 6, Code = 52, Name = "52" },
            };

            facts = new List<F_Marks_MOFOTrihedralAgr>
            {
                new F_Marks_MOFOTrihedralAgr { ID = 1, Value = 1, RefOrgType = orgs[1], RefMarks = marks[1], RefRegions = region, SourceID = 1, RefReport = report },
                new F_Marks_MOFOTrihedralAgr { ID = 2, Value = 2, RefOrgType = orgs[2], RefMarks = marks[1], RefRegions = region, SourceID = 1, RefReport = report },
                new F_Marks_MOFOTrihedralAgr { ID = 3, Value = 3, RefOrgType = orgs[0], RefMarks = marks[4], RefRegions = region, SourceID = 1, RefReport = report },
                new F_Marks_MOFOTrihedralAgr { ID = 4, Value = 4, RefOrgType = orgs[0], RefMarks = marks[5], RefRegions = region, SourceID = 1, RefReport = report },
            };

            Expect.Call(factRepository.FindAll()).Return(facts.AsQueryable()).Repeat.Any();
            Expect.Call(orgRepository.FindAll()).Return(orgs.AsQueryable()).Repeat.Any();
            Expect.Call(markRepository.FindAll()).Return(marks.AsQueryable()).Repeat.Any();
            Expect.Call(periodRepository.Get(20110000)).Return(period).Repeat.Any();
        }

        [Test]
        public void GetFormDataTest()
        {
            Expect.Call(reportRepository.FindAll()).Return(new List<D_Report_TrihedrAgr> { report } .AsQueryable());

            mocks.ReplayAll();

            var repository = new FormRepository(factRepository, orgRepository, markRepository, periodRepository, datasourceRepository, taskRepository, reportRepository);

            var result = repository.GetFormData(1);

            mocks.VerifyAll();

            Assert.AreEqual(6, result.Count);
            Assert.AreEqual(null, result[0].OrgType1);
            Assert.AreEqual(null, result[0].OrgType2);
            Assert.AreEqual(null, result[0].OrgType3);
            Assert.AreEqual(false, result[0].IsEditable);
            Assert.AreEqual("1.0", result[0].CodeStr);
            
            Assert.AreEqual(null, result[1].OrgType1);
            Assert.AreEqual(1, result[1].OrgType2);
            Assert.AreEqual(2, result[1].OrgType3);
            Assert.AreEqual(true, result[1].IsEditable);
            Assert.AreEqual("1.1", result[1].CodeStr);
            
            Assert.AreEqual(4, result[5].OrgType1);
            Assert.AreEqual(null, result[5].OrgType2);
            Assert.AreEqual(null, result[5].OrgType3);
            Assert.AreEqual(true, result[5].IsEditable);
            Assert.AreEqual("5.2", result[5].CodeStr);
        }

        [Test]
        public void CheckNullSaveTest()
        {
            var repository = new FormRepository(factRepository, orgRepository, markRepository, periodRepository, datasourceRepository, taskRepository, reportRepository);

            repository.Save(new AjaxStoreSaveDomainModel<FormModel>(), 1, region);
        }

        [Test]
        public void SaveExistFactTest()
        {
            var mofoWebFormSoglModels = new List<FormModel>
                {
                    new FormModel { Code = 11, CodeStr = "1.1", OrgType1 = null, OrgType2 = 10, OrgType3 = 20 },
                    new FormModel { Code = 51, CodeStr = "5.1", OrgType1 = 50, OrgType2 = null, OrgType3 = null },
                };

            Expect.Call(taskRepository.Get(1)).Return(task);
            Expect.Call(reportRepository.FindAll()).Return(new List<D_Report_TrihedrAgr> { report } .AsQueryable());
            Expect.Call(datasourceRepository.FindAll()).Return(
                new List<DataSources> { new DataSources { ID = 1, SupplierCode = "ФО", DataCode = 6, Year = "2010" } } .AsQueryable());

            var fact1 = facts[0];
            fact1.Value = 10;
            Expect.Call(() => factRepository.Save(fact1));

            var fact2 = facts[1];
            fact1.Value = 20;
            Expect.Call(() => factRepository.Save(fact2));

            var fact3 = facts[2];
            fact3.Value = 50;
            Expect.Call(() => factRepository.Save(fact3));

            mocks.ReplayAll();

            var repository = new FormRepository(factRepository, orgRepository, markRepository, periodRepository, datasourceRepository, taskRepository, reportRepository);
            repository.Save(
                new AjaxStoreSaveDomainModel<FormModel> { Updated = mofoWebFormSoglModels.ToArray() },
                1,
                region);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveNewFactTest()
        {
            var mofoWebFormSoglModels = new List<FormModel>
                {
                    new FormModel { Code = 12, CodeStr = "1.2", OrgType1 = null, OrgType2 = 11, OrgType3 = 22 }
                };

            Expect.Call(taskRepository.Get(1)).Return(task);
            Expect.Call(reportRepository.FindAll()).Return(new List<D_Report_TrihedrAgr> { report } .AsQueryable());
            Expect.Call(datasourceRepository.FindAll()).Return(
                new List<DataSources> { new DataSources { ID = 1, SupplierCode = "ФО", DataCode = 6, Year = "2010" } } .AsQueryable());

            Expect.Call(orgRepository.FindOne(2)).Return(orgs[1]);
            Expect.Call(orgRepository.FindOne(3)).Return(orgs[2]);

            var fact1 = new F_Marks_MOFOTrihedralAgr { Value = 11, RefOrgType = orgs[1], RefMarks = marks[2], SourceID = 1, RefRegions = region, RefYearDayUNV = new FX_Date_YearDayUNV { ID = 20110000 } };
            Expect.Call(() => factRepository.Save(fact1)).Callback(new Delegates.Function<bool, F_Marks_MOFOTrihedralAgr>(CheckFact1SaveNewFact));

            var fact2 = new F_Marks_MOFOTrihedralAgr { Value = 22, RefOrgType = orgs[2], RefMarks = marks[2], SourceID = 1, RefRegions = region, RefYearDayUNV = new FX_Date_YearDayUNV { ID = 20110000 } };
            Expect.Call(() => factRepository.Save(fact2)).Callback(new Delegates.Function<bool, F_Marks_MOFOTrihedralAgr>(CheckFact2SaveNewFact));

            mocks.ReplayAll();

            var repository = new FormRepository(factRepository, orgRepository, markRepository, periodRepository, datasourceRepository, taskRepository, reportRepository);
            repository.Save(
                new AjaxStoreSaveDomainModel<FormModel> { Updated = mofoWebFormSoglModels.ToArray() },
                1,
                region);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveNewReportFactTest()
        {
            var mofoWebFormSoglModels = new List<FormModel>
                {
                    new FormModel { Code = 12, CodeStr = "1.2", OrgType1 = null, OrgType2 = 11, OrgType3 = 22 }
                };

            Expect.Call(taskRepository.Get(1)).Return(task);
            Expect.Call(reportRepository.FindAll()).Return(new List<D_Report_TrihedrAgr>().AsQueryable());
            Expect.Call(() => reportRepository.Save(null)).IgnoreArguments();
            Expect.Call(datasourceRepository.FindAll()).Return(
                new List<DataSources> { new DataSources { ID = 1, SupplierCode = "ФО", DataCode = 6, Year = "2010" } } .AsQueryable());

            Expect.Call(orgRepository.FindOne(2)).Return(orgs[1]);
            Expect.Call(orgRepository.FindOne(3)).Return(orgs[2]);

            var fact1 = new F_Marks_MOFOTrihedralAgr { Value = 11, RefOrgType = orgs[1], RefMarks = marks[2], SourceID = 1, RefRegions = region, RefYearDayUNV = new FX_Date_YearDayUNV { ID = 20110000 } };
            Expect.Call(() => factRepository.Save(fact1)).Callback(new Delegates.Function<bool, F_Marks_MOFOTrihedralAgr>(CheckFact1SaveNewFact));

            var fact2 = new F_Marks_MOFOTrihedralAgr { Value = 22, RefOrgType = orgs[2], RefMarks = marks[2], SourceID = 1, RefRegions = region, RefYearDayUNV = new FX_Date_YearDayUNV { ID = 20110000 } };
            Expect.Call(() => factRepository.Save(fact2)).Callback(new Delegates.Function<bool, F_Marks_MOFOTrihedralAgr>(CheckFact2SaveNewFact));

            mocks.ReplayAll();

            var repository = new FormRepository(factRepository, orgRepository, markRepository, periodRepository, datasourceRepository, taskRepository, reportRepository);
            repository.Save(
                new AjaxStoreSaveDomainModel<FormModel> { Updated = mofoWebFormSoglModels.ToArray() },
                1,
                region);

            mocks.VerifyAll();
        }

        private static bool CheckFact1SaveNewFact(F_Marks_MOFOTrihedralAgr fact)
        {
            return fact.Value == 11 && fact.RefOrgType.ID == 2 && fact.RefMarks.ID == 3;
        }

        private static bool CheckFact2SaveNewFact(F_Marks_MOFOTrihedralAgr fact)
        {
            return fact.Value == 22 && fact.RefOrgType.ID == 3 && fact.RefMarks.ID == 3;
        }
    }
}
