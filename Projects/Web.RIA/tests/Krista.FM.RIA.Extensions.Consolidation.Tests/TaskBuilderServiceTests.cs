using System;
using System.Collections.Generic;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class TaskBuilderServiceTests
    {
        private MockRepository mocks;
        private IReportBuilder reportBuilder;
        private ITaskBuilder taskBuilder;
        private ITaskBuilderFactory taskBuilderFactory;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            taskBuilder = mocks.DynamicMock<ITaskBuilder>();
            reportBuilder = mocks.DynamicMock<IReportBuilder>();
            taskBuilderFactory = mocks.DynamicMock<ITaskBuilderFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Ignore]
        [Test]
        public void CanBuildTasksTest()
        {
            Expect.Call(taskBuilderFactory.CreateBuilder(null)).Return(taskBuilder).Repeat.Once().IgnoreArguments();
            Expect.Call(taskBuilder.Build(null, DateTime.Now, DateTime.Now))
                .Repeat.Once()
                .IgnoreArguments()
                .Return(new List<D_CD_Task> { new D_CD_Task() });
            Expect.Call(() => reportBuilder.CreateReport(null)).Repeat.Once().IgnoreArguments();

            mocks.ReplayAll();

            var service = new TaskBuilderService(reportBuilder, null, null, null, null, null, null);

            service.BuildTasks(DateTime.Now, DateTime.Now, new DataModel().Reglaments[0]);
        }
    }
}
