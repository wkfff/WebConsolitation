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
    public class RightValueSelectorTests
    {
        [Test]
        public void SelectorWithColumnTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 0 },
                new Record { Code = "200", Debt = 11 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СТРОКА[.CODE = '200'].ГРАФА[DEBT]", records);

            Assert.AreEqual(11, ((Record)records[0]).Debt);
        }

        [Test]
        public void SelectorWithoutColumnTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 0 },
                new Record { Code = "200", Debt = 11 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СТРОКА[.CODE = '200']", records);

            Assert.AreEqual(11, ((Record)records[0]).Debt);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Правая часть выражения присваивания должна возвращать одну строку либо скалярное значение.", ExpectedException = typeof(EvaluationException))]
        public void SelectorReturnManyRowsTest()
        {
            List<IRecord> records = new List<IRecord>
            {
                new Record { Code = "100", Debt = 0 },
                new Record { Code = "200", Debt = 11 }
            };

            Calculate("СТРОКА[.CODE = '100'].[DEBT]", ":=", "СТРОКА[ИСТИНА]", records);

            Assert.AreEqual(11, ((Record)records[0]).Debt);
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
                        pi.SetValue(this, value, null);
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
