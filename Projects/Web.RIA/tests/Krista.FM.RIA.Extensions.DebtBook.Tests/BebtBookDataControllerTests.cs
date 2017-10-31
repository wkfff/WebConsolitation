using System;
using System.Data;
using Ext.Net.MVC;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.DebtBook.Presentation.Controllers;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class BebtBookDataControllerTests
    {
        private MockRepository mocks;
        private IScheme scheme;
        private IEntityDataService dataService;
        private IDebtBookExtension extension;
        private DataTable data;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();

            scheme = mocks.DynamicMock<IScheme>();
            extension = mocks.DynamicMock<IDebtBookExtension>();
            dataService = mocks.DynamicMock<IEntityDataService>();
            var package = mocks.DynamicMock<IPackage>();
            var entity = mocks.DynamicMock<IEntity>();

            Expect.Call(scheme.RootPackage)
                .Return(package)
                .Repeat.Once();
            Expect.Call(package.FindEntityByName(String.Empty))
                .IgnoreArguments()
                .Return(entity)
                .Repeat.Once();

            data = new DataTable();
            data.Columns.Add("REFREGION", typeof(string));
            data.Columns.Add("LP_REFREGION", typeof(string));
            data.Columns.Add("REFVARIANT", typeof(string));
            data.Columns.Add("SOURCEID", typeof(string));
            data.Rows.Add(new object[] { 1, "a", 2, 3 });
        }

        [Test]
        public void CanGetDefaultRecordTest()
        {
            Expect.Call(dataService.GetData(null, 0, 0, String.Empty, String.Empty, String.Empty, null, null))
                .IgnoreArguments()
                .Return(new AjaxStoreResult { Data = data })
                .Repeat.Once();

            Expect.Call(extension.CurrentSourceId).Repeat.Once().Return(1200);

            Expect.Call(extension.Variant).Repeat.Once().Return(
                new VariantDescriptor(4, String.Empty, String.Empty, String.Empty, 2010, DateTime.Now));

            mocks.ReplayAll();

            BebtBookDataController controller = new BebtBookDataController(scheme, dataService, extension, null, null, null, null);

            var result = controller.GetRecord("", -1);

            mocks.VerifyAll();

            var dt = (DataTable) result.Data;
            Assert.AreEqual(DBNull.Value, dt.Rows[0][0]);
            Assert.AreEqual(DBNull.Value, dt.Rows[0][1]);
            Assert.AreEqual("4", dt.Rows[0][2]);
            Assert.AreEqual("1200", dt.Rows[0][3]);
        }

        [Test]
        public void CanGetStoredRecordTest()
        {
            Expect.Call(dataService.GetData(null, 0, 0, String.Empty, String.Empty, String.Empty, null))
                .IgnoreArguments()
                .Return(new AjaxStoreResult { Data = data })
                .Repeat.Once();

            mocks.ReplayAll();

            BebtBookDataController controller = new BebtBookDataController(scheme, dataService, extension, null, null, null, null);

            var result = controller.GetRecord("", 1);

            mocks.VerifyAll();

            var dt = (DataTable)result.Data;
            Assert.AreEqual("1", dt.Rows[0][0]);
            Assert.AreEqual("a", dt.Rows[0][1]);
            Assert.AreEqual("2", dt.Rows[0][2]);
            Assert.AreEqual("3", dt.Rows[0][3]);
        }
    }
}
