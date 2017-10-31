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
    public class GroupsArithmeticTests
    {
        #region Графы левой и правой частей совпадают

        [Test]
        public void SummGroupsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT]) + СУММА(СТРОКА[.CODE != '100'].ГРАФА[CRED])", records);

            Assert.AreEqual(55, ((Record)records[0]).Debt);
        }

        [Test]
        public void MinusGroupsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT]) - СУММА(СТРОКА[.CODE != '100'].ГРАФА[CRED])", records);

            Assert.AreEqual(-45, ((Record)records[0]).Debt);
        }

        [Test]
        public void TimesGroupsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT]) * СУММА(СТРОКА[.CODE != '100'].ГРАФА[CRED])", records);

            Assert.AreEqual(250, ((Record)records[0]).Debt);
        }

        [Test]
        public void DivGroupsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[CRED]) / СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT])", records);

            Assert.AreEqual(10, ((Record)records[0]).Debt);
        }

        [Test]
        public void ModGroupsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 10, Cred = 10 },
                new Record { Code = "200", Debt = 1, Cred = 2 },
                new Record { Code = "300", Debt = 1, Cred = 3 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[CRED]) % СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
        }

        #endregion Графы левой и правой частей совпадают

        #region В левой части арифметического оператора граф больше чем в правой

        [Test]
        public void SummGroupsLeftGreaterTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT, CRED]", ":=", "СУММА(СТРОКА[.CODE != '100']) + СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT])", records);

            Assert.AreEqual(10, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
            Assert.AreEqual(3, ((Record)records[2]).Debt);
            Assert.AreEqual(50, ((Record)records[0]).Cred);
            Assert.AreEqual(20, ((Record)records[1]).Cred);
            Assert.AreEqual(30, ((Record)records[2]).Cred);
        }

        [Test]
        public void DivGroupsLeftGreaterTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 0, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT, CRED]", ":=", "СУММА(СТРОКА[.CODE != '100']) / СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT])", records);

            Assert.AreEqual(1, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
            Assert.AreEqual(3, ((Record)records[2]).Debt);
            Assert.AreEqual(50, ((Record)records[0]).Cred);
            Assert.AreEqual(20, ((Record)records[1]).Cred);
            Assert.AreEqual(30, ((Record)records[2]).Cred);
        }

        #endregion В левой части арифметического оператора граф больше чем в правой

        #region В правой части арифметического оператора граф больше чем в левой

        [Test]
        public void SummGroupsRightGreaterTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT, CRED]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT]) + СУММА(СТРОКА[.CODE != '100'])", records);

            Assert.AreEqual(10, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
            Assert.AreEqual(3, ((Record)records[2]).Debt);
            Assert.AreEqual(50, ((Record)records[0]).Cred);
            Assert.AreEqual(20, ((Record)records[1]).Cred);
            Assert.AreEqual(30, ((Record)records[2]).Cred);
        }

        [Test]
        public void MinusGroupsRightGreaterTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT, CRED]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT]) - СУММА(СТРОКА[.CODE != '100'])", records);

            Assert.AreEqual(0, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
            Assert.AreEqual(3, ((Record)records[2]).Debt);
            Assert.AreEqual(50, ((Record)records[0]).Cred);
            Assert.AreEqual(20, ((Record)records[1]).Cred);
            Assert.AreEqual(30, ((Record)records[2]).Cred);
        }

        #endregion В левой части арифметического оператора граф больше чем в правой

        #region Несколько операторов

        [Test]
        public void MultiGroupsOpTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "1", Debt = 1, Cred = 10 },
                new Record { Code = "2", Debt = 2, Cred = 20 },
                new Record { Code = "3", Debt = 3, Cred = 30 },
                new Record { Code = "4", Debt = 4, Cred = 40 },
            };

            Calculate("СТРОКА[.CODE = '1'].[DEBT, CRED]", ":=", "СУММА(СТРОКА[.CODE = '2']) + СУММА(СТРОКА[.CODE = '3']) + СУММА(СТРОКА[.CODE = '4'])", records);

            Assert.AreEqual(9, ((Record)records[0]).Debt);
            Assert.AreEqual(2, ((Record)records[1]).Debt);
            Assert.AreEqual(3, ((Record)records[2]).Debt);
            Assert.AreEqual(90, ((Record)records[0]).Cred);
            Assert.AreEqual(20, ((Record)records[1]).Cred);
            Assert.AreEqual(30, ((Record)records[2]).Cred);
        }

        #endregion Несколько операторов

        #region Некорректные выражения

        [Test]
        [ExpectedException(ExpectedException = typeof(EvaluationException), ExpectedMessage = "Над результатами групповых функций можно применять только следующие операторы: +, -, *, / и %. Выражение: СУММА(СТРОКА[.CODE != '100' ]ГРАФА[DEBT]) И СУММА(СТРОКА[.CODE != '100' ])")]
        public void InvalidGroupsOpTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 10 },
                new Record { Code = "200", Debt = 2, Cred = 20 },
                new Record { Code = "300", Debt = 3, Cred = 30 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT, CRED]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT]) И СУММА(СТРОКА[.CODE != '100'])", records);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(EvaluationException), ExpectedMessage = "Преобразование выражения RightValueSelector в тип GroupFuncResult не реализовано.")]
        public void InvalidGroupsOpConvertToGroupResultTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 1, Cred = 2 },
                new Record { Code = "200", Debt = 2, Cred = 2 },
                new Record { Code = "300", Debt = 3, Cred = 3 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СУММА(СТРОКА[.CODE != '100'].ГРАФА[DEBT]) - СТРОКА[.CODE = '100'].ГРАФА[CRED]", records);
        }

        #endregion Некорректные выражения

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
