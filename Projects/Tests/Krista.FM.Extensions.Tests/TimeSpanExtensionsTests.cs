using System;
using NUnit.Framework;

namespace Krista.FM.Extensions.Tests
{
    [TestFixture]
    public class TimeSpanExtensionsTests
    {
        [Test]
        public void ZeroTest()
        {
            var result = (new DateTime(2011, 5, 11) - new DateTime(2011, 5, 11)).ToFrendlyString();

            Assert.AreEqual("менее 5 секунд", result);
        }

        [Test]
        public void One21SecondTest()
        {
            var result = (new DateTime(2011, 5, 11, 0, 0, 21) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("21 секунда", result);
        }

        [Test]
        public void Two22SecondTest()
        {
            var result = (new DateTime(2011, 5, 11, 0, 0, 22) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("22 секунды", result);
        }

        [Test]
        public void Three23SecondTest()
        {
            var result = (new DateTime(2011, 5, 11, 0, 0, 23) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("23 секунды", result);
        }

        [Test]
        public void Four24SecondTest()
        {
            var result = (new DateTime(2011, 5, 11, 0, 0, 24) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("24 секунды", result);
        }

        [Test]
        public void Five25SecondTest()
        {
            var result = (new DateTime(2011, 5, 11, 0, 0, 25) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("25 секунд", result);
        }

        [Test]
        public void OneDayTest()
        {
            var result = (new DateTime(2011, 5, 12, 0, 0, 0) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("1 день", result);
        }

        [Test]
        public void TwoDayTest()
        {
            var result = (new DateTime(2011, 5, 13, 0, 0, 0) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("2 дня", result);
        }

        [Test]
        public void Five5DayTest()
        {
            var result = (new DateTime(2011, 5, 16, 0, 0, 0) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("5 дней", result);
        }

        [Test]
        public void OneWeek1DayTest()
        {
            var result = (new DateTime(2011, 5, 19, 0, 0, 0) - new DateTime(2011, 5, 11, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("1 неделя 1 день", result);
        }

        [Test]
        public void OneMonth1Week1DayTest()
        {
            var result = (new DateTime(2011, 2, 8, 0, 0, 0) - new DateTime(2011, 1, 1, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("1 месяц 1 неделя", result);
        }

        [Test]
        public void OneMonth1DayTest()
        {
            var result = (new DateTime(2011, 2, 2, 0, 0, 0) - new DateTime(2011, 1, 1, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("1 месяц", result);
        }

        [Test]
        public void TwoYearTest()
        {
            var result = (new DateTime(2013, 1, 1, 0, 0, 0) - new DateTime(2011, 1, 1, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("2 года", result);
        }

        [Test]
        public void TwoYear1MonthTest()
        {
            var result = (new DateTime(2013, 2, 1, 0, 0, 0) - new DateTime(2011, 1, 1, 0, 0, 0)).ToFrendlyString();

            Assert.AreEqual("2 года 1 месяц", result);
        }

        [Test]
        public void TwoYear1Month1WeekTest()
        {
            var result = (new DateTime(2013, 2, 8, 0, 0, 0) - new DateTime(2011, 1, 1, 0, 0, 0)).ToFrendlyString(3);

            Assert.AreEqual("2 года 1 месяц 1 неделя", result);
        }

        [Test]
        public void LateTest()
        {
            var result = (new DateTime(2011, 1, 1, 0, 0, 0) - new DateTime(2011, 1, 31, 0, 0, 0)).ToFrendlyString(3);

            Assert.AreEqual("опоздание 1 месяц", result);
        }
    }
}
