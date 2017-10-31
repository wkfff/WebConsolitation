namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    using System;
    using System.Collections.Generic;

    using Krista.FM.Common.Consolidation.Calculations;
    using Krista.FM.Common.Consolidation.Calculations.Visitors;
    using Krista.FM.Common.Consolidation.Data;
    using Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests.Helpers;
    using Krista.FM.Common.Consolidation.Tests.Helpers;
    using Krista.FM.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class InRowRelationTests
    {
        [Test]
        public void ArithmeticSubstractTest()
        {
            var records = new List<IRecord>
            {
                new Record { Code = "100", Cred = 5, Debt = 2 },
                new Record { Code = "100", Cred = 2, Debt = 4 },
                new Record { Code = "200", Cred = -3, Debt = 4, Balance = 3 }
            };

            Calculate("СТРОКА[.CODE = '100'].[BALANCE]", ":=", ".CRED - .DEBT", records);

            Assert.AreEqual(3, ((Record)records[0]).Balance);
            Assert.AreEqual(-2, ((Record)records[1]).Balance);
            Assert.AreEqual(3, ((Record)records[2]).Balance);
        }

        [Test]
        public void ArithmeticSubstractWithGraphSelectorTest()
        {
            var records = new List<IRecord>
            {
                new Record { Code = "100", Cred = 5, Debt = 2, Balance = 0 },
                new Record { Code = "100", Cred = 2, Debt = 4, Balance = 0 },
                new Record { Code = "200", Cred = -3, Debt = 4, Balance = 3 }
            };

            Calculate("СТРОКА[.CODE != '100'].[BALANCE]", ":=", ".ГРАФА[CRED] - .ГРАФА[DEBT]", records);

            Assert.AreEqual(0, ((Record)records[0]).Balance);
            Assert.AreEqual(0, ((Record)records[1]).Balance);
            Assert.AreEqual(-7, ((Record)records[2]).Balance);
        }

        [Test]
        public void InRowSelectorAndGeneralRowSelectorTest()
        {
            var records = new List<IRecord>
            {
                new Record { Code = "100", Cred = 5, Debt = 2, Balance = 0 },
                new Record { Code = "100", Cred = 2, Debt = 4, Balance = 0 },
                new Record { Code = "200", Cred = -3, Debt = 4, Balance = 3 }
            };

            Calculate("СТРОКА[.CODE = '100'].[BALANCE]", ":=", ".ГРАФА[CRED] - СТРОКА[.CODE = '200'].ГРАФА[DEBT]", records);

            Assert.AreEqual(1, ((Record)records[0]).Balance);
            Assert.AreEqual(-2, ((Record)records[1]).Balance);
            Assert.AreEqual(3, ((Record)records[2]).Balance);
        }

        [Test]
        public void InRowSelectorAndGroupRowSelectorTest()
        {
            var records = new List<IRecord>
            {
                new Record { Code = "100", Cred = 5, Debt = 2, Balance = 1 },
                new Record { Code = "100", Cred = 2, Debt = 4, Balance = 1 },
                new Record { Code = "200", Cred = 8, Debt = 4, Balance = 3 }
            };

            Calculate("СТРОКА[.CODE = '200'].[BALANCE]", ":=", ".ГРАФА[CRED] - СУММА(СТРОКА[.CODE = '100'].ГРАФА[DEBT])", records);

            Assert.AreEqual(1, ((Record)records[0]).Balance);
            Assert.AreEqual(1, ((Record)records[1]).Balance);
            Assert.AreEqual(2, ((Record)records[2]).Balance);
        }

        #region Utils

        private static void Calculate(string left, string op, string right, List<IRecord> records)
        {
            var expr = Expression.Compile(RelationsFactory.Create(left, op, right));
            var provider = new DataProvider { Records = records };
            var visitor = new EvaluationVisitor(provider, provider);
            visitor.SetContext(string.Empty, string.Empty, provider.GetMetaColumns());
            expr.Accept(visitor);
        }

        private class DataProvider : IPrimaryDataProvider, IDataProvider
        {
            public DataProvider()
            {
                Records = new List<IRecord>();
            }

            public List<IRecord> Records { get; set; }

            public void SetContext(string sectionName, string formName, bool slave)
            {
            }

            public IList<IRecord> GetSectionRows()
            {
                return Records;
            }

            public IList<IRecord> GetSectionRows(string sqlFilter)
            {
                throw new NotImplementedException();
            }

            public IList<D_Form_TableColumn> GetMetaColumns()
            {
                return new List<D_Form_TableColumn> 
                {
                    new D_Form_TableColumn { Code = "Debt", InternalName = "Debt" },
                    new D_Form_TableColumn { Code = "Cred", InternalName = "Cred" },
                    new D_Form_TableColumn { Code = "Balance", InternalName = "Balance" },
                    new D_Form_TableColumn { Code = "Code", InternalName = "Code" }
                };
            }

            public IRecord CreateRecord(IRecord template)
            {
                throw new NotImplementedException();
            }

            public void AppendRecord(IRecord record)
            {
                throw new NotImplementedException();
            }

            public IList<IRecord> GetMultipliesRowsTemplates()
            {
                throw new NotImplementedException();
            }
        }

        private class Record : RecordBase
        {
            public string Code { get; set; }

            public decimal Debt { get; set; }

            public decimal Cred { get; set; }
            
            public decimal Balance { get; set; }
        }

        #endregion Utils
    }
}