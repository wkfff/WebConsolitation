using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using Krista.FM.Domain;

using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    [TestFixture]
    public class GroupFunctionTests
    {
        [Test]
        public void SummNudeTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 2 },
                new Record { Code = "200", Debt = 3 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[ИСТИНА])", records);

            Assert.AreEqual(5, ((Record)records[0]).Debt);
            Assert.AreEqual(3, ((Record)records[1]).Debt);
        }

        [Test]
        public void SummCondTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Code = "200", Debt = 2 },
                new Record { Code = "300", Debt = 3 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[.CODE != '100'])", records);

            Assert.AreEqual(5, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
            Assert.AreEqual(3, ((Record)records[2]).Debt);
        }

        [Test]
        public void SummCondWithColumnTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Code = "200", Debt = 2 },
                new Record { Code = "300", Debt = 3 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT])", records);

            Assert.AreEqual(5, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
            Assert.AreEqual(3, ((Record)records[2]).Debt);
        }

        [Test]
        public void SummCondWithMultiColumnsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 2 },
                new Record { Code = "200", Debt = 2, Cred = 4 },
                new Record { Code = "300", Debt = 3, Cred = 8 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT, CRED]", ":=", "СУММА(СТРОКА[.CODE != '100'])", records);

            Assert.AreEqual(5, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
            Assert.AreEqual(3, ((Record)records[2]).Debt);

            Assert.AreEqual(12, ((Record)records[0]).Cred);
            Assert.AreEqual(4, ((Record)records[1]).Cred);
            Assert.AreEqual(8, ((Record)records[2]).Cred);
        }

        [Test]
        public void MinNudeTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 3 },
                new Record { Code = "200", Debt = 2 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "МИНИМУМ(СТРОКА[ИСТИНА])", records);

            Assert.AreEqual(2, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
        }

        [Test]
        public void MaxNudeTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 2 },
                new Record { Code = "200", Debt = 7 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "МАКСИМУМ(СТРОКА[ИСТИНА])", records);

            Assert.AreEqual(7, ((Record)records[0]).Debt);
            Assert.AreEqual(7, ((Record)records[1]).Debt);
        }

        [Test]
        public void AvgNudeTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 2 },
                new Record { Code = "200", Debt = 7 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СРЕДНЕЕ(СТРОКА[ИСТИНА])", records);

            Assert.AreEqual(4.5m, ((Record)records[0]).Debt);
            Assert.AreEqual(7, ((Record)records[1]).Debt);
        }

        [Test]
        public void CountNudeTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1 },
                new Record { Code = "200", Debt = 3 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СЧЕТ(СТРОКА[ИСТИНА])", records);

            Assert.AreEqual(2, ((Record)records[0]).Debt);
            Assert.AreEqual(3, ((Record)records[1]).Debt);
        }

        [Test]
        public void SummWithNullInConditionsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100000", Debt = 1 },
                new Record { Debt = 2 },
                new Record { Code = "200000", Debt = 3 },
                new Record { Code = "200010", Debt = 4 },
            };

            Calculate("СТРОКА[ПУСТО(.CODE)].[DEBT]", ":=", "СУММА(СТРОКА[(.CODE % 1000) = 0])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(4, ((Record)records[1]).Debt);
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

            public decimal Cred { get; set; }

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
