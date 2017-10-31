using System;
using System.Collections.Generic;
using System.Reflection;
using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using Krista.FM.Domain;

using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    [TestFixture]
    public class LogicFunctionTests
    {
        [Test]
        public void ColumnsUndependTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Debt = 2 },
                new Record { Code = "200", Debt = 3 },
                new Record { Code = "200", Debt = 4 },
            };

            Calculate("СТРОКА[ПУСТО(.CODE)].[DEBT]", ":=", "СУММА(СТРОКА[НЕПУСТО(.CODE)])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(8, ((Record)records[1]).Debt);
        }

        [Test]
        public void ColumnsUndepend2Test()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Debt = 2 },
                new Record { Code = "200", Debt = 3 },
                new Record { Code = "200", Debt = 4 },
            };

            Calculate("СТРОКА[ПУСТО(.CODE)].[DEBT]", ":=", "СУММА(СТРОКА[ПУСТО(.CODE)])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
        }

        [Test]
        public void ColumnsDependTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Debt = 2 },
                new Record { Code = "200", Debt = 3 },
                new Record { Code = "300", Debt = 4 },
            };

            Calculate("СТРОКА[ПУСТО(.CODE)].[DEBT]", ":=", "СУММА(СТРОКА[НЕПУСТО($.CODE)])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(0, ((Record)records[1]).Debt);
        }

        [Test]
        public void MultiColumnsUndependTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Debt = 2, Cred = 20 },
                new Record { Code = "200", Debt = 3, Cred = null },
                new Record { Code = "200", Debt = 4, Cred = 40 },
            };

            Calculate("СТРОКА[ПУСТО(.CODE)].[DEBT]", ":=", "СУММА(СТРОКА[НЕПУСТО(.CODE, .CRED)])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(5, ((Record)records[1]).Debt);
        }

        [Test]
        public void MultiColumnsUndepend2Test()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = null },
                new Record { Debt = 2, Cred = 20 },
                new Record { Code = "200", Debt = 3, Cred = null },
                new Record { Code = "200", Debt = 4, Cred = 40 },
            };

            Calculate("СТРОКА[ПУСТО(.CODE)].[DEBT]", ":=", "СУММА(СТРОКА[ПУСТО(.CODE, .CRED)])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(0, ((Record)records[1]).Debt);
        }

        [Test]
        public void MultiColumnsUndepend3Test()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = null },
                new Record { Debt = 2, Cred = null },
                new Record { Code = "200", Debt = 3, Cred = null },
                new Record { Code = "200", Debt = 4, Cred = 40 },
            };

            Calculate("СТРОКА[ПУСТО(.CODE)].[DEBT]", ":=", "СУММА(СТРОКА[ПУСТО(.CODE, .CRED)])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
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
                    new D_Form_TableColumn { Code = "Code", InternalName = "Code" },
                    new D_Form_TableColumn { Code = "Debt", InternalName = "Debt" },
                    new D_Form_TableColumn { Code = "Cred", InternalName = "Cred" }
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

        private class Record : IRecord
        {
            public string Code { get; set; }

            public decimal Debt { get; set; }

            public decimal? Cred { get; set; }

            public int MetaRowId
            {
                get { throw new NotImplementedException(); }
            }

            public ReportDataRecordState State
            {
                get { throw new NotImplementedException(); }
            }

            public object Value
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsMultiplicity()
            {
                throw new NotImplementedException();
            }

            public object Get(string column)
            {
                foreach (PropertyInfo pi in GetType().GetProperties())
                {
                    if (pi.Name.ToUpper() == column.ToUpper())
                    {
                        return pi.GetValue(this, null);
                    }
                }

                throw new InvalidOperationException(String.Format("Колонка с именем {0} не найдена.", column));
            }

            public void Set(string column, object value)
            {
                foreach (PropertyInfo pi in GetType().GetProperties())
                {
                    if (pi.Name.ToUpper() == column.ToUpper())
                    {
                        pi.SetValue(this, Convert.ChangeType(value, pi.PropertyType), null);
                        break;
                    }
                }
            }

            public void AssignRecord(object assignRecord)
            {
                throw new NotImplementedException();
            }

            public void Delete()
            {
                throw new NotImplementedException();
            }
        }

        #endregion Utils
    }
}
