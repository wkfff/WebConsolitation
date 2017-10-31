using System;
using System.Collections.Generic;

using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests.Helpers;
using Krista.FM.Common.Consolidation.Tests.Helpers;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests.EvaluationVisitorTests
{
    public class DependContextParamColumnTests
    {
        [Test]
        public void DependContextParamColumnTest()
        {
            var dataProvider = new DataProvider
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "ОСНОВНОЙ", 
                        new List<IRecord>
                            {
                                new RecordA { Code = "100", Debt = 1, Cred = 10 },
                                new RecordA { Code = "200", Debt = 2, Cred = 20 }
                            }
                    },
                    {
                        "СПРАВКА", 
                        new List<IRecord>
                            {
                                new RecordB { CodeB = "100", DebtB = 3, CredB = 30 },
                                new RecordB { CodeB = "200", DebtB = 4, CredB = 40 }
                            }
                    }
                }
            };

            dataProvider.SetContext("Основной", null, false);
            Calculate("СТРОКА[].ГРАФА[DEBT]", ":=", "ПОДОТЧЕТНЫЙ[].ФОРМА['Форма1'].РАЗДЕЛ['Справка'].СТРОКА[(.CODEB = $.CODE)].ГРАФА[DEBTB]", dataProvider);

            Assert.AreEqual(3, ((RecordA)dataProvider.Records["ОСНОВНОЙ"][0]).Debt);
            Assert.AreEqual(4, ((RecordA)dataProvider.Records["ОСНОВНОЙ"][1]).Debt);
        }

        [Test]
        public void SumDependContextParamColumnTest()
        {
            var dataProvider = new DataProvider
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "ОСНОВНОЙ", 
                        new List<IRecord>
                            {
                                new RecordA { Code = "100", Debt = 1, Cred = 10 },
                                new RecordA { Code = "200", Debt = 2, Cred = 20 }
                            }
                    },
                    {
                        "СПРАВКА", 
                        new List<IRecord>
                            {
                                new RecordB { CodeB = "100", DebtB = 3, CredB = 30 },
                                new RecordB { CodeB = "100", DebtB = 4, CredB = 30 },
                                new RecordB { CodeB = "200", DebtB = 5, CredB = 40 },
                                new RecordB { CodeB = "200", DebtB = 6, CredB = 40 }
                            }
                    }
                }
            };

            dataProvider.SetContext("Основной", null, false);
            Calculate("СТРОКА[].ГРАФА[DEBT]", ":=", "СУММА(ПОДОТЧЕТНЫЙ[].ФОРМА['Форма1'].РАЗДЕЛ['Справка'].СТРОКА[(.CODEB = $.CODE)].ГРАФА[DEBTB])", dataProvider);

            Assert.AreEqual(7, ((RecordA)dataProvider.Records["ОСНОВНОЙ"][0]).Debt);
            Assert.AreEqual(11, ((RecordA)dataProvider.Records["ОСНОВНОЙ"][1]).Debt);
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
                section = sectionName.ToUpper();
                form = formName.With(x => x.ToUpper());
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
                if (section == "ОСНОВНОЙ")
                {
                    return new List<D_Form_TableColumn>
                    {
                        new D_Form_TableColumn { Code = "Code", InternalName = "Code" },
                        new D_Form_TableColumn { Code = "Debt", InternalName = "Debt" },
                        new D_Form_TableColumn { Code = "Cred", InternalName = "Cred" }
                    };
                }

                return new List<D_Form_TableColumn>
                {
                    new D_Form_TableColumn { Code = "CodeB", InternalName = "CodeB" },
                    new D_Form_TableColumn { Code = "DebtB", InternalName = "DebtB" },
                    new D_Form_TableColumn { Code = "CredB", InternalName = "CredB" }
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

        private class RecordA : RecordBase
        {
            public string Code { get; set; }

            public decimal Debt { get; set; }

            public decimal Cred { get; set; }
        }

        private class RecordB : RecordBase
        {
            public string CodeB { get; set; }

            public decimal DebtB { get; set; }

            public decimal CredB { get; set; }
        }

        #endregion Utils
    }
}
