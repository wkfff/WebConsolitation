using System.Data;

using Krista.FM.Extensions;

using NUnit.Framework;

namespace Krista.FM.RIA.Core.Tests
{
    [TestFixture]
    public class DataTableExtensionsTests
    {
        private DataTable dt;

        [SetUp]
        public void Setup()
        {
            dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Rows.Add(new object[] { 0 });
            dt.Rows.Add(new object[] { 1 });
            dt.Rows.Add(new object[] { 2 });
            dt.Rows.Add(new object[] { 3 });
            dt.Rows.Add(new object[] { 4 });
            dt.Rows.Add(new object[] { 5 });
            dt.Rows.Add(new object[] { 6 });
            dt.Rows.Add(new object[] { 7 });
            dt.Rows.Add(new object[] { 8 });
            dt.Rows.Add(new object[] { 9 });
        }

        public void CanSelectFromEmptyTableTest()
        {
            var result = new DataTable().SelectPage(0, 10);

            Assert.AreEqual(0, result.Rows.Count);
        }

        [Test]
        public void CanGetPageOnTopTest()
        {
            var result = dt.SelectPage(0, 4);

            Assert.AreEqual(4, result.Rows.Count);
            Assert.AreEqual("0", result.Rows[0][0]);
            Assert.AreEqual("3", result.Rows[3][0]);
        }

        [Test]
        public void CanGetPageOnMiddleTest()
        {
            var result = dt.SelectPage(2, 4);

            Assert.AreEqual(4, result.Rows.Count);
            Assert.AreEqual("2", result.Rows[0][0]);
            Assert.AreEqual("5", result.Rows[3][0]);
        }

        [Test]
        public void CanGetPageWithNegativePageSizeTest()
        {
            var result = dt.SelectPage(4, -2);

            Assert.AreEqual(0, result.Rows.Count);
        }

        [Test]
        public void CanGetPageWithMinusOnePageSizeTest()
        {
            var result = dt.SelectPage(5, -1);

            Assert.AreEqual(5, result.Rows.Count);
            Assert.AreEqual("5", result.Rows[0][0]);
        }

        [Test]
        public void CanGetPageWithNegativeStartTest()
        {
            var result = dt.SelectPage(-2, 4);

            Assert.AreEqual(4, result.Rows.Count);
            Assert.AreEqual("0", result.Rows[0][0]);
            Assert.AreEqual("3", result.Rows[3][0]);
        }

        [Test]
        public void CanSortPageTest()
        {
            var result = dt.SelectPage(0, 5, "ID", "DESC");

            Assert.AreEqual(5, result.Rows.Count);
            Assert.AreEqual("9", result.Rows[0][0]);
            Assert.AreEqual("5", result.Rows[4][0]);
        }
    }
}
