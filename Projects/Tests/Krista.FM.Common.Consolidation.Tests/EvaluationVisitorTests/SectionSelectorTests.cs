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
    public class SectionSelectorTests
    {
        [Test]
        public void SectionSelectorTest()
        {
            var dataProvider = new DataProvider 
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "ОСНОВНОЙ", 
                        new List<IRecord>
                            {
                                new Record { Code = "100", Debt = 1, Cred = 10 },
                                new Record { Code = "200", Debt = 2, Cred = 20 }
                            }
                    },
                    {
                        "СПРАВКА", 
                        new List<IRecord>
                            {
                                new Record { Code = "100", Debt = 3, Cred = 30 },
                                new Record { Code = "200", Debt = 4, Cred = 40 }
                            }
                    }
                }
            };

            dataProvider.SetContext("ОСНОВНОЙ", null, false);
            Calculate("СТРОКА[].ГРАФА[DEBT]", ":=", "ПОДОТЧЕТНЫЙ[].ФОРМА['Форма1'].РАЗДЕЛ['Справка'].СТРОКА[(.CODE = 100)]", dataProvider);

            Assert.AreEqual(3, ((Record)dataProvider.Records["ОСНОВНОЙ"][0]).Debt);
            Assert.AreEqual(3, ((Record)dataProvider.Records["ОСНОВНОЙ"][0]).Debt);
        }

        [Test]
        public void SectionSelectorArithmiticsTest()
        {
            var dataProvider = new DataProvider
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "РАЗДЕЛ1", 
                        new List<IRecord>
                            {
                                new Record { Code = "100", Debt = 1, Cred = 10 },
                                new Record { Code = "200", Debt = 2, Cred = 20 }
                            }
                    },
                    {
                        "РАЗДЕЛ2", 
                        new List<IRecord>
                            {
                                new Record { Code = "100", Debt = 1, Cred = 10 },
                                new Record { Code = "200", Debt = 2, Cred = 20 }
                            }
                    },
                    {
                        "СПРАВКА", 
                        new List<IRecord>
                            {
                                new Record { Code = "100", Debt = 3, Cred = 30 },
                                new Record { Code = "200", Debt = 4, Cred = 40 }
                            }
                    }
                }
            };

            dataProvider.SetContext("СПРАВКА", null, false);
            Calculate("СТРОКА[.CODE = '100'].ГРАФА[DEBT]", ":=", "РАЗДЕЛ['РАЗДЕЛ1'].СТРОКА[.CODE = '100'].ГРАФА[DEBT] + РАЗДЕЛ['РАЗДЕЛ2'].СТРОКА[.CODE = '100'].ГРАФА[DEBT]", dataProvider);

            Assert.AreEqual(2, ((Record)dataProvider.Records["СПРАВКА"][0]).Debt);
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
            private string section;
            private string form;
            private bool isSlave;

            public DataProvider()
            {
                Records = new Dictionary<string, List<IRecord>>();
            }

            public Dictionary<string, List<IRecord>> Records { get; set; }

            public void SetContext(string sectionName, string formName, bool slave)
            {
                section = sectionName;
                form = formName;
                isSlave = slave;
            }

            public IList<IRecord> GetSectionRows()
            {
                return Records[section];
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
