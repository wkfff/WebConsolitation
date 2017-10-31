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
    [TestFixture]
    public class ConsRowGenRelationTests
    {
        [Test(Description = "Первоначальная генерация записей, если генерации еще не производилось.")]
        public void FirstPassTest()
        {
            var metaRows = new List<D_Form_TableRow>
            {
                new D_Form_TableRow { Multiplicity = false },
                new D_Form_TableRow { Multiplicity = true },
            };

            var dataProvider = new DataProvider
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "ОТЧЕТ", 
                        new List<IRecord>
                        {
                            new PartRecord { Code = null, Name = "Всего", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 211000, Name = "Показатель1", Fact1 = 10, Fact2 = 100 },
                            new PartRecord { Code = 212000, Name = "Показатель2", Fact1 = 20, Fact2 = null },
                            new PartRecord { Code = 213000, Name = "Показатель3", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 223021, Name = "Деталь1", Fact1 = 12, Fact2 = null },
                            new PartRecord { Code = 223022, Name = "Деталь2", Fact1 = 18, Fact2 = null },
                            new PartRecord { Code = 234000, Name = "Показатель4", Fact1 = null, Fact2 = 400 },
                        }
                    },
                    {
                        "СВОД", 
                        new List<IRecord>
                        {
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.1.0", Nme = "Всего1", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.2.0", Nme = "Всего2", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.2.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                        }
                    }
                }
            };

            dataProvider.SetContext("СВОД");
            Consolidate(
                "строка[.Cde = '1.1.1'].графа[Nme]", 
                "^-",
                "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0].(.Name)", 
                dataProvider);

            Assert.AreEqual(7, dataProvider.Records["СВОД"].Count);
            Assert.AreEqual("Показатель1", dataProvider.Records["СВОД"][4].Get("Nme"));
            Assert.AreEqual("Показатель2", dataProvider.Records["СВОД"][5].Get("Nme"));
            Assert.AreEqual("Показатель4", dataProvider.Records["СВОД"][6].Get("Nme"));

            try
            {
                dataProvider.SetContext("СВОД");
                Calculate(
                    "строка[.Cde = '1.1.1' и непусто(.Nme)].графа[Fct1]",
                    ":=",
                    "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0 и .Name = $.Nme].графа[Fact1]",
                    dataProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [Test(Description = "Повторная генерация записей, если генерация уже производилась.")]
        public void SecondPassTest()
        {
            var metaRows = new List<D_Form_TableRow>
            {
                new D_Form_TableRow { Multiplicity = false },
                new D_Form_TableRow { Multiplicity = true },
            };

            var dataProvider = new DataProvider
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "ОТЧЕТ", 
                        new List<IRecord>
                        {
                            new PartRecord { Code = null, Name = "Всего", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 211000, Name = "Показатель1", Fact1 = 10, Fact2 = 100 },
                            new PartRecord { Code = 212000, Name = "Показатель2", Fact1 = 20, Fact2 = null },
                            new PartRecord { Code = 213000, Name = "Показатель3", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 223021, Name = "Деталь1", Fact1 = 12, Fact2 = null },
                            new PartRecord { Code = 223022, Name = "Деталь2", Fact1 = 18, Fact2 = null },
                            new PartRecord { Code = 234000, Name = "Показатель4", Fact1 = null, Fact2 = 400 },
                        }
                    },
                    {
                        "СВОД", 
                        new List<IRecord>
                        {
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.1.0", Nme = "Всего1", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель1", Fct1 = 10, Fct2 = 100 },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель2", Fct1 = 20, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель4", Fct1 = null, Fct2 = 400 },
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.2.0", Nme = "Всего2", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.2.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                        }
                    }
                }
            };

            dataProvider.SetContext("СВОД");
            Consolidate(
                "строка[.Cde = '1.1.1'].графа[Nme]",
                "^-",
                "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0].(.Name)",
                dataProvider);

            Assert.AreEqual(7, dataProvider.Records["СВОД"].Count);
            Assert.AreEqual("Показатель1", dataProvider.Records["СВОД"][2].Get("Nme"));
            Assert.AreEqual("Показатель2", dataProvider.Records["СВОД"][3].Get("Nme"));
            Assert.AreEqual("Показатель4", dataProvider.Records["СВОД"][4].Get("Nme"));

            try
            {
                dataProvider.SetContext("СВОД");
                Calculate(
                    "строка[.Cde = '1.1.1' и непусто(.Nme)].графа[Fct1]",
                    ":=",
                    "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0 и .Name = $.Nme].графа[Fact1]",
                    dataProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [Test(Description = "Повторная генерация записей, если генерация уже производилась и была добавлена навая запись.")]
        public void SecondPassWithNewRocordsTest()
        {
            var metaRows = new List<D_Form_TableRow>
            {
                new D_Form_TableRow { Multiplicity = false },
                new D_Form_TableRow { Multiplicity = true },
            };

            var dataProvider = new DataProvider
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "ОТЧЕТ", 
                        new List<IRecord>
                        {
                            new PartRecord { Code = null, Name = "Всего", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 211000, Name = "Показатель1", Fact1 = 10, Fact2 = 100 },
                            new PartRecord { Code = 212000, Name = "Показатель2", Fact1 = 20, Fact2 = null },
                            new PartRecord { Code = 213000, Name = "Показатель3", Fact1 = 30, Fact2 = null },
                            new PartRecord { Code = 223021, Name = "Деталь1", Fact1 = 12, Fact2 = null },
                            new PartRecord { Code = 223022, Name = "Деталь2", Fact1 = 18, Fact2 = null },
                            new PartRecord { Code = 234000, Name = "Показатель4", Fact1 = null, Fact2 = 400 },
                        }
                    },
                    {
                        "СВОД", 
                        new List<IRecord>
                        {
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.1.0", Nme = "Всего1", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель1", Fct1 = 10, Fct2 = 100 },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель2", Fct1 = 20, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель4", Fct1 = null, Fct2 = 400 },
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.2.0", Nme = "Всего2", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.2.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                        }
                    }
                }
            };

            dataProvider.SetContext("СВОД");
            Consolidate(
                "строка[.Cde = '1.1.1'].графа[Nme]",
                "^-",
                "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0].(.Name)",
                dataProvider);

            Assert.AreEqual(8, dataProvider.Records["СВОД"].Count);
            Assert.AreEqual("Показатель1", dataProvider.Records["СВОД"][2].Get("Nme"));
            Assert.AreEqual("Показатель2", dataProvider.Records["СВОД"][3].Get("Nme"));
            Assert.AreEqual("Показатель4", dataProvider.Records["СВОД"][4].Get("Nme"));
            Assert.AreEqual("Показатель3", dataProvider.Records["СВОД"][7].Get("Nme"));

            try
            {
                dataProvider.SetContext("СВОД");
                Calculate(
                    "строка[.Cde = '1.1.1' и непусто(.Nme)].графа[Fct1, Fct2]",
                    ":=",
                    "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0 и .Name = $.Nme].графы[Fact1, Fact2]",
                    dataProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [Test(Description = "Повторная генерация записей, если генерация уже производилась и была удалена запись.")]
        public void SecondPassWithRemoveRocordsTest()
        {
            var metaRows = new List<D_Form_TableRow>
            {
                new D_Form_TableRow { Multiplicity = false },
                new D_Form_TableRow { Multiplicity = true },
            };

            var dataProvider = new DataProvider
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "ОТЧЕТ", 
                        new List<IRecord>
                        {
                            new PartRecord { Code = null, Name = "Всего", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 211000, Name = "Показатель1", Fact1 = 10, Fact2 = 100 },
                            new PartRecord { Code = 212000, Name = "Показатель2", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 213000, Name = "Показатель3", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 223021, Name = "Деталь1", Fact1 = 12, Fact2 = null },
                            new PartRecord { Code = 223022, Name = "Деталь2", Fact1 = 18, Fact2 = null },
                            new PartRecord { Code = 234000, Name = "Показатель4", Fact1 = null, Fact2 = 400 },
                        }
                    },
                    {
                        "СВОД", 
                        new List<IRecord>
                        {
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.1.0", Nme = "Всего1", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель1", Fct1 = 10, Fct2 = 100 },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель2", Fct1 = 20, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель4", Fct1 = null, Fct2 = 400 },
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.2.0", Nme = "Всего2", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.2.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                        }
                    }
                }
            };

            dataProvider.SetContext("СВОД");
            Consolidate(
                "строка[.Cde = '1.1.1'].графа[Nme]",
                "^-",
                "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0].(.Name)",
                dataProvider);

            Assert.AreEqual(7, dataProvider.Records["СВОД"].Count);
            Assert.AreEqual("Показатель1", dataProvider.Records["СВОД"][2].Get("Nme"));
            Assert.AreEqual("Показатель2", dataProvider.Records["СВОД"][3].Get("Nme"));
            Assert.AreEqual("Показатель4", dataProvider.Records["СВОД"][4].Get("Nme"));

            try
            {
                dataProvider.SetContext("СВОД");
                Calculate(
                    "строка[.Cde = '1.1.1' и непусто(.Nme)].графа[Fct1, Fct2]",
                    ":=",
                    "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0 и .Name = $.Nme].графы[Fact1, Fact2]",
                    dataProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [Test(Description = "Повторная генерация записей, если генерация уже производилась и были удалены и дабавлены записи.")]
        public void SecondPassWithRemoveAndNewRocordsTest()
        {
            var metaRows = new List<D_Form_TableRow>
            {
                new D_Form_TableRow { Multiplicity = false },
                new D_Form_TableRow { Multiplicity = true },
            };

            var dataProvider = new DataProvider
            {
                Records = new Dictionary<string, List<IRecord>>
                {
                    {
                        "ОТЧЕТ", 
                        new List<IRecord>
                        {
                            new PartRecord { Code = null, Name = "Всего", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 211000, Name = "Показатель1", Fact1 = 10, Fact2 = 100 },
                            new PartRecord { Code = 212000, Name = "Показатель2", Fact1 = null, Fact2 = null },
                            new PartRecord { Code = 213000, Name = "Показатель3", Fact1 = 30, Fact2 = null },
                            new PartRecord { Code = 223021, Name = "Деталь1", Fact1 = 12, Fact2 = null },
                            new PartRecord { Code = 223022, Name = "Деталь2", Fact1 = 18, Fact2 = null },
                            new PartRecord { Code = 234000, Name = "Показатель4", Fact1 = null, Fact2 = 400 },
                        }
                    },
                    {
                        "СВОД", 
                        new List<IRecord>
                        {
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.1.0", Nme = "Всего1", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель1", Fct1 = 10, Fct2 = 100 },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель2", Fct1 = 20, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.1.1", Nme = "Показатель4", Fct1 = null, Fct2 = 400 },
                            new ConsRecord { MetaRow = metaRows[0], Cde = "1.2.0", Nme = "Всего2", Fct1 = null, Fct2 = null },
                            new ConsRecord { MetaRow = metaRows[1], Cde = "1.2.1", Nme = String.Empty, Fct1 = null, Fct2 = null },
                        }
                    }
                }
            };

            dataProvider.SetContext("СВОД");
            Consolidate(
                "строка[.Cde = '1.1.1'].графа[Nme]",
                "^-",
                "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0].(.Name)",
                dataProvider);

            Assert.AreEqual(8, dataProvider.Records["СВОД"].Count);
            Assert.AreEqual("Показатель1", dataProvider.Records["СВОД"][2].Get("Nme"));
            Assert.AreEqual("Показатель2", dataProvider.Records["СВОД"][3].Get("Nme"));
            Assert.AreEqual("Показатель4", dataProvider.Records["СВОД"][4].Get("Nme"));
            Assert.AreEqual("Показатель3", dataProvider.Records["СВОД"][7].Get("Nme"));

            try
            {
                dataProvider.SetContext("СВОД");
                Calculate(
                    "строка[.Cde = '1.1.1' и непусто(.Nme)].графа[Fct1, Fct2]",
                    ":=",
                    "раздел['ОТЧЕТ'].строка[(непусто(.Fact1) или непусто(.Fact2)) и (.Code % 1000) = 0 и .Name = $.Nme].графы[Fact1, Fact2]",
                    dataProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #region Utils

        private static void Calculate(string left, string op, string right, DataProvider dataProvider)
        {
            var expr = Expression.Compile(RelationsFactory.Create(left, op, right));
            var visitor = new EvaluationVisitor(dataProvider, dataProvider);
            visitor.SetContext(String.Empty, String.Empty, dataProvider.GetMetaColumns());
            expr.Accept(visitor);
        }

        private static void Consolidate(string left, string op, string right, DataProvider dataProvider)
        {
            var expr = Expression.Compile(RelationsFactory.Create(left, op, right, 2));
            var visitor = new EvaluationVisitor(dataProvider, dataProvider);
            visitor.SetContext(String.Empty, String.Empty, dataProvider.GetMetaColumns());
            expr.Accept(visitor);
        }

        private class DataProvider : IPrimaryDataProvider, IDataProvider
        {
            private string primarySection;
            private string section;
            private string form;
            private bool isSlave;

            public DataProvider()
            {
                Records = new Dictionary<string, List<IRecord>>();
            }

            public Dictionary<string, List<IRecord>> Records { get; set; }

            public void SetContext(string sectionName)
            {
                if (primarySection == null)
                {
                    primarySection = sectionName;
                }

                section = sectionName;
            }

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
                if (section == "ОТЧЕТ")
                {
                    return new List<D_Form_TableColumn>
                    {
                        new D_Form_TableColumn { Code = "Code", InternalName = "Code" },
                        new D_Form_TableColumn { Code = "Name", InternalName = "Name" },
                        new D_Form_TableColumn { Code = "Fact1", InternalName = "Fact1" },
                        new D_Form_TableColumn { Code = "Fact2", InternalName = "Fact2" }
                    };
                }

                return new List<D_Form_TableColumn>
                {
                    new D_Form_TableColumn { Code = "Cde", InternalName = "Cde" },
                    new D_Form_TableColumn { Code = "Nme", InternalName = "Nme" },
                    new D_Form_TableColumn { Code = "Fct1", InternalName = "Fct1" },
                    new D_Form_TableColumn { Code = "Fct2", InternalName = "Fct2" }
                };
            }

            public IRecord CreateRecord(IRecord template)
            {
                var record = new ConsRecord();
                if (template is ConsRecord)
                {
                    record.MetaRow = ((ConsRecord)template).MetaRow;
                    record.Cde = ((ConsRecord)template).Cde;
                }

                return record;
            }

            public void AppendRecord(IRecord record)
            {
                Records[primarySection].Add(record);
            }

            public IList<IRecord> GetMultipliesRowsTemplates()
            {
                return new List<IRecord> { new ConsRecord { Cde = "1.1.1", MetaRow = new D_Form_TableRow { Multiplicity = true } } };
            }
        }

        private class PartRecord : Record
        {
            public int? Code { get; set; }

            public string Name { get; set; }

            public decimal? Fact1 { get; set; }

            public decimal? Fact2 { get; set; }

            public override string ToString()
            {
                return "PartRecord(Code: {0}, Name: {1}, Fact1: {2}, Fact2: {3})".FormatWith(Code, Name, Fact1, Fact2);
            }
        }

        private class ConsRecord : Record
        {
            public string Cde { get; set; }

            public string Nme { get; set; }

            public decimal? Fct1 { get; set; }

            public decimal? Fct2 { get; set; }

            public override string ToString()
            {
                return "ConsRecord(Cde: {0}, Nme: {1}, Fct1: {2}, Fct2: {3})".FormatWith(Cde, Nme, Fct1, Fct2);
            }
        }

        private class Record : RecordBase
        {
            public D_Form_TableRow MetaRow { get; set; }

            public override bool IsMultiplicity()
            {
                return MetaRow.Multiplicity;
            }
        }

        #endregion Utils
    }
}
