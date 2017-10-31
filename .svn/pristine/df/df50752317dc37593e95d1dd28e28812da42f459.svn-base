using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Common;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.RIA.Extensions.DebtBook.Tests
{
    [TestFixture]
    public class ChangesCalcServiceTests
    {
        private MockRepository mocks;
        private IEntity entity;
        private IDataUpdater du;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            entity = mocks.DynamicMock<IEntity>();
            du = mocks.DynamicMock<IDataUpdater>();

            var data = new DataTable();
            data.Columns.Add("ID", typeof(int));
            data.Columns.Add("ChargeDate", typeof(DateTime));
            data.Columns.Add("Fact", typeof(Decimal));
            data.Columns.Add("Plan", typeof(Decimal));
            data.Columns.Add("ParentId", typeof(int));
            data.Rows.Add(1, new DateTime(2011, 1, 1), 10, 20, null);
            data.Rows.Add(2, new DateTime(2011, 2, 1), 12, 26, 1);
            data.Rows.Add(3, new DateTime(2011, 3, 1), 13, 27, 1);
            data.Rows.Add(4, new DateTime(2011, 4, 1), 14, 28, 1);

            Expect.Call(entity.GetDataUpdater(null, null, new DbParameterDescriptor("name", 1)))
                .Return(du)
                .IgnoreArguments();
            var dummy = new DataTable();
            Expect.Call(du.Fill(ref dummy)).OutRef(data).Return(1).IgnoreArguments();
            Expect.Call(du.Update(ref dummy)).OutRef(data).Return(1).IgnoreArguments();
            Expect.Call(du.Dispose);

            mocks.ReplayAll();
        }

        [Test]
        public void RecalcTest()
        {
            var service = new ChangesCalcService();

            var formules = new Dictionary<string, ColumnState>
                               {
                                   { "Plan", new ColumnState { CalcFormula =  "prev(Fact, Plan) * 10" } },
                                   { "Fact", new ColumnState { CalcFormula =  "[Plan] * 10" } },
                               };
            service.Recalc(entity, 1, formules);

            mocks.VerifyAll();
        }
    }
}
