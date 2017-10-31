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
    public class ThreeValuedLogicTests
    {
        #region AND

        /*
         * Таблица истинности AND
         * AND F T U
         *  F  F F F
         *  T  F T U
         *  U  F U U
         */
        [Test]
        public void AndFUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = false, B = null } };

            var result = Calculate("СТРОКА[.A И .B]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void AndTUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = true, B = null } };

            var result = Calculate("СТРОКА[.A И .B]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void AndUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = true, B = null } };

            var result = Calculate("СТРОКА[.A И .B]", records);

            Assert.AreEqual(0, result.Count);
        }

        #endregion AND

        #region OR

        /*
         * Таблица истинности OR
         * OR  F T U
         *  F  F T U
         *  T  T T T
         *  U  U T U
         */
        [Test]
        public void OrFUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = false, B = null } };

            var result = Calculate("СТРОКА[.A ИЛИ .B]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void OrTUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = true, B = null } };

            var result = Calculate("СТРОКА[.A ИЛИ .B]", records);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void OrUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = null, B = null } };

            var result = Calculate("СТРОКА[.A ИЛИ .B]", records);

            Assert.AreEqual(0, result.Count);
        }

        #endregion OR

        #region NOT
        
        /*
         * Таблица истинности NOT
         * NOT
         *  F  T
         *  T  F
         *  U  U
         */
        [Test]
        public void NotFTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = false } };

            var result = Calculate("СТРОКА[НЕ .A]", records);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void NotTTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = true } };

            var result = Calculate("СТРОКА[НЕ .A]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void NotUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { A = null } };

            var result = Calculate("СТРОКА[НЕ .A]", records);

            Assert.AreEqual(0, result.Count);
        }

        #endregion NOT

        #region Арифметика с NULL

        [Test]
        public void AddUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = null, Y = null } };

            var result = Calculate("СТРОКА[.X + .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void AddVUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 1, Y = null } };

            var result = Calculate("СТРОКА[.X + .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void AddVVTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 1, Y = 1 } };

            var result = Calculate("СТРОКА[.X + .Y > 0]", records);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void SubstractUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = null, Y = null } };

            var result = Calculate("СТРОКА[.X - .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SubstractVUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 2, Y = null } };

            var result = Calculate("СТРОКА[.X - .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SubstractVVTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 2, Y = 1 } };

            var result = Calculate("СТРОКА[.X - .Y > 0]", records);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void TimesUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = null, Y = null } };

            var result = Calculate("СТРОКА[.X * .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void TimesVUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 2, Y = null } };

            var result = Calculate("СТРОКА[.X * .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void TimesVVTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 2, Y = 1 } };

            var result = Calculate("СТРОКА[.X * .Y > 0]", records);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void DivUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = null, Y = null } };

            var result = Calculate("СТРОКА[.X / .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void DivVUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 2, Y = null } };

            var result = Calculate("СТРОКА[.X / .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void DivVVTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 2, Y = 1 } };

            var result = Calculate("СТРОКА[.X / .Y > 0]", records);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ModUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = null, Y = null } };

            var result = Calculate("СТРОКА[.X % .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ModVUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 3, Y = null } };

            var result = Calculate("СТРОКА[.X % .Y > 0]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ModVVTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 3, Y = 2 } };

            var result = Calculate("СТРОКА[.X % .Y > 0]", records);

            Assert.AreEqual(1, result.Count);
        }

        #endregion Арифметика с NULL

        #region Сравнение с NULL

        [Test]
        public void EqualUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = null, Y = null } };

            var result = Calculate("СТРОКА[.X = .Y]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void EqualVUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 1, Y = null } };

            var result = Calculate("СТРОКА[.X = .Y]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void EqualVVTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 1, Y = 1 } };

            var result = Calculate("СТРОКА[.X = .Y]", records);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void NotEqualUUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = null, Y = null } };

            var result = Calculate("СТРОКА[.X != .Y]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void NotEqualVUTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 1, Y = null } };

            var result = Calculate("СТРОКА[.X != .Y]", records);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void NotEqualVVTest()
        {
            List<IRecord> records = new List<IRecord> { new Record { X = 1, Y = 2 } };

            var result = Calculate("СТРОКА[.X != .Y]", records);

            Assert.AreEqual(1, result.Count);
        }

        #endregion Сравнение с NULL

        #region Utils

        private static List<IRecord> Calculate(string left, List<IRecord> records)
        {
            var expr = Expression.Compile(RelationsFactory.Create(left, null, null));
            var provider = new DataProvider { Records = records };
            var visitor = new EvaluationVisitor(provider, provider);
            visitor.SetContext(String.Empty, String.Empty, provider.GetMetaColumns());
            expr.Accept(visitor);

            return (List<IRecord>)visitor.Result;
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
                    new D_Form_TableColumn { Code = "X", InternalName = "X" },
                    new D_Form_TableColumn { Code = "Y", InternalName = "Y" },
                    new D_Form_TableColumn { Code = "A", InternalName = "A" },
                    new D_Form_TableColumn { Code = "B", InternalName = "B" }
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

            public decimal? X { get; set; }

            public decimal? Y { get; set; }

            public bool? A { get; set; }

            public bool? B { get; set; }

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
