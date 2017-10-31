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
    public class InStatementTests
    {
        [Test]
        public void StringTest()
        {
            DataProvider dataProvider = new DataProvider
            {
                Records =
                    {
                        new Record { Code = "Абв", Debt = 0 },
                        new Record { Code = "Abc", Debt = 0 },
                        new Record { Code = "Xyz", Debt = 0 }
                    }
            };

            Calculate("СТРОКА[.CODE В ('Абв', 'Xyz')].[DEBT]", ":=", "1.00", dataProvider);

            Assert.AreEqual(1, ((Record)dataProvider.Records[0]).Debt);
            Assert.AreEqual(0, ((Record)dataProvider.Records[1]).Debt);
            Assert.AreEqual(1, ((Record)dataProvider.Records[2]).Debt);
        }

        [Test]
        public void IntTest()
        {
            DataProvider dataProvider = new DataProvider
            {
                Records =
                    {
                        new Record { Debt = 10 },
                        new Record { Debt = 20 },
                        new Record { Debt = 30 }
                    }
            };

            Calculate("СТРОКА[.DEBT В (20, 30)].[DEBT]", ":=", "1.00", dataProvider);

            Assert.AreEqual(10, ((Record)dataProvider.Records[0]).Debt);
            Assert.AreEqual(1, ((Record)dataProvider.Records[1]).Debt);
            Assert.AreEqual(1, ((Record)dataProvider.Records[2]).Debt);
        }

        #region Utils

        private static void Calculate(string left, string op, string right, DataProvider dataProvider)
        {
            var expr = Expression.Compile(RelationsFactory.Create(left, op, right));
            var visitor = new EvaluationVisitor(dataProvider, dataProvider);
            visitor.SetContext(String.Empty, String.Empty, dataProvider.GetMetaColumns());
            expr.Accept(visitor);
        }

        private class DataProvider : IPrimaryDataProvider, IDataProvider
        {
            public DataProvider()
            {
                Records = new List<IRecord>();
            }

            public List<IRecord> Records { get; private set; }

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
