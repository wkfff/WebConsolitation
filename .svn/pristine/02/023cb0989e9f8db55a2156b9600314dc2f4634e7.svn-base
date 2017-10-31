using System;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests
{
    [TestFixture]
    public class ConsTasksFilterTests
    {
        [Test]
        [Description("По статусу задачи")]
        public void StatusSuccessTest()
        {
            var reportsTree = new Domain.ReportsTree { StatusId = 1, Deadline = new DateTime(2010, 1, 1) };
            
            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, true, true, true, true },
                new DateTime(2010, 1, 2));

            Assert.True(result);
        }

        [Test]
        [Description("По статусу задачи")]
        public void StatusFailTest()
        {
            var reportsTree = new Domain.ReportsTree { StatusId = 1, Deadline = new DateTime(2010, 1, 1) };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { false, true, true, true, true, true, true },
                new DateTime(2010, 1, 2));

            Assert.False(result);
        }

        [Test]
        [Description("Задачи просроченные")]
        public void DeadlineSuccessTest()
        {
            var reportsTree = new Domain.ReportsTree { StatusId = 1, Deadline = new DateTime(2010, 1, 1) };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, true, false, false, false },
                new DateTime(2010, 1, 2));

            Assert.True(result);
        }

        [Test]
        [Description("Задачи просроченные")]
        public void DeadlineFailTest()
        {
            var reportsTree = new Domain.ReportsTree { StatusId = 1, Deadline = new DateTime(2010, 1, 1) };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, true, false, false, false },
                new DateTime(2010, 1, 1));

            Assert.False(result);
        }

        [Test]
        [Description("Задачи для исполнения")]
        public void ToDoSuccessTest()
        {
            var reportsTree = new Domain.ReportsTree
            {
                StatusId = 1,
                EndDate = new DateTime(2010, 1, 1),
                Deadline = new DateTime(2010, 1, 20)
            };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, false, true, false, false },
                new DateTime(2010, 1, 10));

            Assert.True(result);
        }

        [Test]
        [Description("Задачи для исполнения")]
        public void ToDoFailTest()
        {
            var reportsTree = new Domain.ReportsTree
                                  {
                                      StatusId = 1,
                                      EndDate = new DateTime(2010, 1, 1),
                                      Deadline = new DateTime(2010, 1, 20)
                                  };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, false, true, false, false },
                new DateTime(2010, 1, 21));

            Assert.False(result);
        }

        [Test]
        [Description("Задачи текущего периода")]
        public void FutureSuccessTest()
        {
            var reportsTree = new Domain.ReportsTree
            {
                StatusId = 1,
                BeginDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2010, 1, 2)
            };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, false, false, true, false },
                new DateTime(2010, 1, 1));

            Assert.True(result);
        }

        [Test]
        [Description("Задачи текущего периода")]
        public void FutureFailTest()
        {
            var reportsTree = new Domain.ReportsTree
            {
                StatusId = 1,
                BeginDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2010, 1, 2)
            };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, false, false, true, false },
                new DateTime(2010, 2, 1));

            Assert.False(result);
        }

        [Test]
        [Description("Задачи текущего года")]
        public void CurrentYearSuccessTest()
        {
            var reportsTree = new Domain.ReportsTree { StatusId = 1, BeginDate = new DateTime(2010, 10, 1) };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, false, false, false, true },
                new DateTime(2010, 1, 2));

            Assert.True(result);
        }

        [Test]
        [Description("Задачи текущего года")]
        public void CurrentYearFailTest()
        {
            var reportsTree = new Domain.ReportsTree { StatusId = 1, Deadline = new DateTime(2011, 1, 1) };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { true, true, true, false, false, false, true },
                new DateTime(2010, 1, 1));

            Assert.False(result);
        }

        [Test]
        [Description("По состоянию и времени")]
        public void StateAndTimelineSuccessTest()
        {
            var reportsTree = new Domain.ReportsTree
            {
                StatusId = 2,
                BeginDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2010, 1, 2)
            };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { false, true, true, false, false, true, false },
                new DateTime(2010, 1, 1));

            Assert.True(result);
        }

        [Test]
        [Description("По состоянию и времени")]
        public void StateAndTimelineFailTest()
        {
            var reportsTree = new Domain.ReportsTree
            {
                StatusId = 2,
                BeginDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2010, 1, 2)
            };

            var result = ConsTasksFilter.FilterStoreData(
                reportsTree,
                new[] { false, true, true, false, false, true, false },
                new DateTime(2010, 2, 1));

            Assert.False(result);
        }
    }
}
