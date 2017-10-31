using System;
using System.Collections.Generic;
using System.Reflection;
using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests.Helpers;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using Krista.FM.Domain;

using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    [TestFixture]
    public class AssignRelationTests
    {
        [Test]
        public void ScalarIntTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Code = "200", Debt = 2 },
                new Record { Code = "300", Debt = 3 }
            };

            Calculate("СТРОКА[.CODE != '100'].[DEBT]", ":=", "10", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(10, ((Record)records[1]).Debt);
            Assert.AreEqual(10, ((Record)records[2]).Debt);
        }

        [Test]
        public void ScalarIntTowColumnsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Code = "200", Debt = 2 },
                new Record { Code = "300", Debt = 3 }
            };

            Calculate("СТРОКА[.CODE != '100'].[DEBT, CRED]", ":=", "10", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(10, ((Record)records[1]).Debt);
            Assert.AreEqual(10, ((Record)records[2]).Debt);

            Assert.AreEqual(0, ((Record)records[0]).Cred);
            Assert.AreEqual(10, ((Record)records[1]).Cred);
            Assert.AreEqual(10, ((Record)records[2]).Cred);
        }

        [Test]
        public void ScalarDecimalTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Code = "200", Debt = 2 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "10.50", records);

            Assert.AreEqual(10.5m, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
        }

        [Test]
        public void ScalarStringTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Code = "200", Debt = 2 }
            };

            Calculate("СТРОКА[.DEBT = 2].[CODE]", ":=", "'NewCode'", records);

            Assert.AreEqual("100", ((Record)records[0]).Code);
            Assert.AreEqual("NEWCODE", ((Record)records[1]).Code);
        }

        #region Utils

        private static void Calculate(string left, string op, string right, List<IRecord> records)
        {
            var expr = Expression.Compile(RelationsFactory.Create(left, op, right));
            var provider = new DataProvider { Records = records };
            var visitor = new EvaluationVisitor(provider, provider);
            visitor.SetContext(String.Empty, String.Empty, provider.GetMetaColumns());
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
        }

        #endregion Utils
    }
}
