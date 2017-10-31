using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class TaskBuilderTests
    {
        private MockRepository mocks;
        private ILinqRepository<D_CD_Task> taskRepository;
        private ILinqRepository<D_CD_Subjects> subjectRepository;
        private IRepository<FX_Date_Year> yearRepository;
        private IRepository<FX_FX_FormStatus> statusRepository;
        private DataModel model;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            taskRepository = mocks.DynamicMock<ILinqRepository<D_CD_Task>>();
            subjectRepository = mocks.DynamicMock<ILinqRepository<D_CD_Subjects>>();
            yearRepository = mocks.DynamicMock<IRepository<FX_Date_Year>>();
            statusRepository = mocks.DynamicMock<IRepository<FX_FX_FormStatus>>();

            model = new DataModel();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void YearBuildForSubjectTest()
        {
            D_CD_Task task = null;

            Expect.Call(yearRepository.Get(2010)).Return(model.Years[0]).Repeat.Once();
            Expect.Call(statusRepository.Get(1)).Return(model.FormStatus[0]).Repeat.Once();
            Expect.Call(subjectRepository.FindAll()).Return(model.Subjects.AsQueryable()).Repeat.Once();
            Expect.Call(taskRepository.FindAll()).Return(new List<D_CD_Task>().AsQueryable()).Repeat.Once();
            Expect.Call(() => taskRepository.Save(null)).Repeat.Once().IgnoreArguments()
                .WhenCalled(m => task = m.Arguments[0] as D_CD_Task);

            mocks.ReplayAll();

            var builder = new YearTaskBuilder(taskRepository, subjectRepository, yearRepository, statusRepository);

            builder.Build(model.Reglaments[0], new DateTime(2010, 1, 1), new DateTime(2011, 1, 1));

            Assert.NotNull(task);
            Assert.AreEqual("Сбор данных для оценки деятельности ОМСУ за 2010 год", task.Name);
            Assert.AreEqual(new DateTime(2010, 1, 1), task.BeginDate);
            Assert.AreEqual(new DateTime(2011, 1, 1), task.EndDate);
            Assert.AreEqual(new DateTime(2011, 1, 11), task.Deadline);
            Assert.AreEqual(model.Subjects[0], task.RefSubject);
        }

        [Test]
        public void YearBuildForRegionTest()
        {
            List<D_CD_Task> tasks = new List<D_CD_Task>();

            Expect.Call(yearRepository.Get(2010)).Return(model.Years[0]).Repeat.Once();
            Expect.Call(statusRepository.Get(1)).Return(model.FormStatus[0]).Repeat.Once();
            Expect.Call(subjectRepository.FindAll()).Return(model.Subjects.AsQueryable()).Repeat.Once();
            Expect.Call(taskRepository.FindAll()).Return(new List<D_CD_Task>().AsQueryable()).Repeat.Times(3);
            Expect.Call(() => taskRepository.Save(null)).Repeat.Times(3).IgnoreArguments()
                .WhenCalled(m => tasks.Add(m.Arguments[0] as D_CD_Task));

            mocks.ReplayAll();

            var builder = new YearTaskBuilder(taskRepository, subjectRepository, yearRepository, statusRepository);

            builder.Build(model.Reglaments[1], new DateTime(2010, 1, 1), new DateTime(2011, 1, 1));

            Assert.NotNull(tasks[0]);
            Assert.AreEqual("Сбор данных для оценки деятельности ОМСУ за 2010 год", tasks[0].Name);
            Assert.AreEqual(new DateTime(2010, 1, 1), tasks[0].BeginDate);
            Assert.AreEqual(new DateTime(2011, 1, 1), tasks[0].EndDate);
            Assert.AreEqual(new DateTime(2011, 1, 10), tasks[0].Deadline);
            Assert.AreEqual(model.Subjects[1], tasks[0].RefSubject);
        }

        [Test]
        public void QuarterBuildForRegionTest()
        {
            List<D_CD_Task> tasks = new List<D_CD_Task>();

            Expect.Call(yearRepository.Get(2010)).Return(model.Years[0]).Repeat.Once();
            Expect.Call(statusRepository.Get(1)).Return(model.FormStatus[0]).Repeat.Once();
            Expect.Call(subjectRepository.FindAll()).Return(model.Subjects.AsQueryable()).Repeat.Once();
            Expect.Call(taskRepository.FindAll()).Return(new List<D_CD_Task>().AsQueryable()).Repeat.Times(3 * 4);
            Expect.Call(() => taskRepository.Save(null)).Repeat.Times(3 * 4).IgnoreArguments()
                .WhenCalled(m => tasks.Add(m.Arguments[0] as D_CD_Task));

            mocks.ReplayAll();

            var builder = new QuarterTaskBuilder(taskRepository, subjectRepository, yearRepository, statusRepository);

            builder.Build(model.Reglaments[1], new DateTime(2010, 1, 1), new DateTime(2011, 1, 1));

            Assert.AreEqual(3 * 4, tasks.Count);
            Assert.NotNull(tasks[0]);
            Assert.AreEqual("Сбор данных для оценки деятельности ОМСУ за 1 квартал", tasks[0].Name);
            Assert.AreEqual(new DateTime(2010, 1, 1), tasks[0].BeginDate);
            Assert.AreEqual(new DateTime(2010, 4, 1), tasks[0].EndDate);
            Assert.AreEqual(new DateTime(2010, 4, 10), tasks[0].Deadline);
            Assert.AreEqual(model.Subjects[1], tasks[0].RefSubject);
        }

        [Test]
        public void MonthBuildForRegionTest()
        {
            List<D_CD_Task> tasks = new List<D_CD_Task>();

            Expect.Call(yearRepository.Get(2010)).Return(model.Years[0]).Repeat.Once();
            Expect.Call(statusRepository.Get(1)).Return(model.FormStatus[0]).Repeat.Once();
            Expect.Call(subjectRepository.FindAll()).Return(model.Subjects.AsQueryable()).Repeat.Once();
            Expect.Call(taskRepository.FindAll()).Return(new List<D_CD_Task>().AsQueryable()).Repeat.Times(3 * 12);
            Expect.Call(() => taskRepository.Save(null)).Repeat.Times(3 * 12).IgnoreArguments()
                .WhenCalled(m => tasks.Add(m.Arguments[0] as D_CD_Task));

            mocks.ReplayAll();

            var builder = new MonthTaskBuilder(taskRepository, subjectRepository, yearRepository, statusRepository);

            builder.Build(model.Reglaments[1], new DateTime(2010, 1, 1), new DateTime(2011, 1, 1));

            Assert.AreEqual(3 * 12, tasks.Count);
            Assert.NotNull(tasks[0]);
            Assert.AreEqual("Сбор данных для оценки деятельности ОМСУ за январь", tasks[0].Name);
            Assert.AreEqual(new DateTime(2010, 1, 1), tasks[0].BeginDate);
            Assert.AreEqual(new DateTime(2010, 2, 1), tasks[0].EndDate);
            Assert.AreEqual(new DateTime(2010, 2, 10), tasks[0].Deadline);
            Assert.AreEqual(model.Subjects[1], tasks[0].RefSubject);
        }
    }
}
