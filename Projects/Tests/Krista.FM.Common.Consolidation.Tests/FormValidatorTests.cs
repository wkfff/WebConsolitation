using System;

using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests
{
    [TestFixture]
    public class FormValidatorTests
    {
        [Test]
        public void IncorrectInternalNameTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "1IntName",
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(2, errors.Count);
            Assert.Contains("Некорректное внутреннее имя формы \"1IntName\".", errors);
            Assert.Contains("У формы должен быть хотя бы один раздел.", errors);
        }

        [Test]
        public void IncorrectCodeTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "Код Формы",
                InternalName = "IntrName1234",
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(2, errors.Count);
            Assert.Contains("Некорректный код формы \"Код Формы\".", errors);
            Assert.Contains("У формы должен быть хотя бы один раздел.", errors);
        }

        [Test]
        public void TooLongNameTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntrName12345",
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(2, errors.Count);
            Assert.Contains("Внутреннее имя формы \"IntrName12345\" длиннее 12 символов.", errors);
            Assert.Contains("У формы должен быть хотя бы один раздел.", errors);
        }

        [Test]
        public void NotUniquePartsTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntName",
                Parts =
                    {
                        new D_Form_Part
                            {
                                InternalName = "Part1",
                                Code = "КодРаздела",
                                Columns = { new D_Form_TableColumn { InternalName = "C1", Code = "C1", DataType = "System.String" } },
                                Rows = { new D_Form_TableRow { Name = "Row1" } }
                            },
                        new D_Form_Part
                            {
                                InternalName = "Part1",
                                Code = "КодРаздела",
                                Columns = { new D_Form_TableColumn { InternalName = "C1", Code = "C1", DataType = "System.String" } },
                                Rows = { new D_Form_TableRow { Name = "Row1" } }
                            }
                    }
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(2, errors.Count);
            Assert.True(errors[0].Contains("В разделах есть неуникальное внутреннее имя \"Part1\"."));
            Assert.True(errors[1].Contains("В разделах есть неуникальный код \"КодРаздела\"."));
        }

        [Test]
        public void NotUniqueRequisitesTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntName",
                Requisites =
                    {
                        new D_Form_Requisites { InternalName = "R1", Code = "Рекв1", IsHeader = true, DataType = "System.String" },
                        new D_Form_Requisites { InternalName = "R2", Code = "Рекв1", IsHeader = true, DataType = "System.Decimal" },
                        new D_Form_Requisites { InternalName = "R2", Code = "Рекв2", IsHeader = false, DataType = "System.Int32" },
                        new D_Form_Requisites { InternalName = "R1", Code = "Рекв1", IsHeader = false, DataType = "System.DateTime" }
                    },
                Parts =
                    {
                        new D_Form_Part
                            {
                                InternalName = "Part1",
                                Code = "КодРаздела",
                                Columns = { new D_Form_TableColumn { InternalName = "C1", Code = "C1", DataType = "System.String" } },
                                Rows = { new D_Form_TableRow { Name = "Row1" } }
                            }
                    },
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(2, errors.Count);
            Assert.True(errors[0].Contains("В реквизитах есть неуникальной внутреннее имя \"R1\"."));
            Assert.True(errors[1].Contains("В реквизитах есть неуникальный код \"Рекв1\"."));
        }

        [Test]
        public void NotUniquePartColumnTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntName",
                Parts =
                    {
                        new D_Form_Part
                            {
                                InternalName = "Part1",
                                Code = "КодРаздела",
                                Columns =
                                    {
                                        new D_Form_TableColumn { InternalName = "N1", Code = "Колонка1", DataType = "System.String" },
                                        new D_Form_TableColumn { InternalName = "N2", Code = "Колонка2", DataType = "System.Int32" },
                                        new D_Form_TableColumn { InternalName = "N2", Code = "Колонка1", DataType = "System.DateTime" }
                                    },
                                Rows = { new D_Form_TableRow { Name = "Row1" } }
                            }
                    }
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(2, errors.Count);
            Assert.True(errors[0].Contains("В разделе \"КодРаздела\" есть неуникальное внутреннее имя графы \"N2\"."));
            Assert.True(errors[1].Contains("В разделе \"КодРаздела\" есть неуникальный код графы \"Колонка1\"."));
        }

        [Test]
        public void IncorrectColumnsTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntName",
                Parts =
                    {
                        new D_Form_Part
                            {
                                InternalName = "Part1",
                                Code = "КодРаздела",
                                Columns =
                                    {
                                        new D_Form_TableColumn { InternalName = "5r2", Code = "Колонка1", DataType = "System.String" },
                                        new D_Form_TableColumn { InternalName = "r.42", Code = "Колонка2", DataType = "System.Int32" },
                                        new D_Form_TableColumn { InternalName = "r42", Code = "Колонка.3", DataType = "System.DateTime" },
                                    },
                                Rows = { new D_Form_TableRow { Name = "Row1" } }
                            }
                    }
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(3, errors.Count);
            Assert.True(errors[0].Contains("В разделе \"КодРаздела\" есть некорректное внутреннее имя графы \"5r2\"."));
            Assert.True(errors[1].Contains("В разделе \"КодРаздела\" есть некорректное внутреннее имя графы \"r.42\"."));
            Assert.True(errors[2].Contains("В разделе \"КодРаздела\" есть некорректный код графы \"Колонка.3\"."));
        }

        [Test]
        public void IncorrectColumnTypeTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntName",
                Parts =
                    {
                        new D_Form_Part
                            {
                                InternalName = "Part1",
                                Code = "КодРаздела",
                                Columns =
                                    {
                                        new D_Form_TableColumn { InternalName = "t1", Code = "Колонка1", DataType = "System.Str1ng" },
                                        new D_Form_TableColumn { InternalName = "t2", Code = "Колонка2", DataType = String.Empty },
                                        new D_Form_TableColumn { InternalName = "t3", Code = "Колонка3", DataType = null },
                                    },
                                Rows = { new D_Form_TableRow { Name = "Row1" } }
                            }
                    }
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(3, errors.Count);
            Assert.True(errors[0].Contains("В разделе \"КодРаздела\" для графы \"Колонка1\" указан неизвестный тип \"System.Str1ng\"."));
            Assert.True(errors[1].Contains("В разделе \"КодРаздела\" для графы \"Колонка2\" не указан тип данных."));
            Assert.True(errors[2].Contains("В разделе \"КодРаздела\" для графы \"Колонка3\" не указан тип данных."));
        }

        [Test]
        public void IncorrectRequsiteTypeTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntName",
                Parts =
                {
                    new D_Form_Part
                    {
                        InternalName = "Part1",
                        Code = "КодРаздела",
                        Columns =
                        {
                            new D_Form_TableColumn { InternalName = "Row", Code = "Колонка1", DataType = "System.String" },
                        },
                        Rows = { new D_Form_TableRow { Name = "Row1" } },
                        Requisites =
                        {
                            new D_Form_Requisites { InternalName = "r1", Code = "Рекв1", DataType = "System.Int32" },
                            new D_Form_Requisites { InternalName = "r2", Code = "Рекв2", DataType = "System1.String" },
                            new D_Form_Requisites { InternalName = "Number", Code = "Рекв3", DataType = String.Empty },
                        }
                    }
                },
                Requisites =
                {
                    new D_Form_Requisites { InternalName = "fr1", Code = "fРекв1", DataType = "System.DateTime" },
                    new D_Form_Requisites { InternalName = "fr2", Code = "fРекв2" }
                }
            };

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(5, errors.Count);
            Assert.True(errors[0].Contains("Для реквизита \"fРекв2\" не указан тип данных."));
            Assert.True(errors[1].Contains("Для реквизита \"Рекв2\" указан неизвестный тип \"System1.String\"."));
            Assert.True(errors[2].Contains("Внутреннее имя реквизита \"Number\" не может быть использовано, т.к. это зарезервированное слово."));
            Assert.True(errors[3].Contains("Для реквизита \"Рекв3\" не указан тип данных."));
            Assert.True(errors[4].Contains("Внутреннее имя графы \"Row\" не может быть использовано, т.к. это зарезервированное слово."));
        }

        [Test]
        public void IncorrectCellValueTest()
        {
            var form = new D_CD_Templates { Class = "ConsForm", Code = "КодФормы", InternalName = "IntName" };

            var part = new D_Form_Part { InternalName = "Part1", Code = "КодРаздела" };
            form.Parts.Add(part);
            var column = new D_Form_TableColumn { InternalName = "r1", Code = "Колонка1", DataType = "System.Int32" };
            var row = new D_Form_TableRow { Name = "Row1" };

            part.Columns.Add(column);
            part.Rows.Add(row);
            part.Cells.Add(new D_Form_TableCell { RefColumn = column, RefRow = row, RefSection = part, DefaultValue = "10g" });

            var errors = new FormValidator().Validate(form);

            Assert.AreEqual(1, errors.Count);
            Assert.True(errors[0].Contains("Для ячейки (\"Колонка1\":\"Row1\") значение по умолчанию \"10g\" не соответствует формату графы с типом System.Int32 (10g не является допустимым значением для 'Int32'.)"));
        }

        [Test]
        public void NotPartsTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntName",
            };

            var errors = new FormValidator().Validate(form);

            Assert.Contains("У формы должен быть хотя бы один раздел.", errors);
        }
        
        [Test]
        public void NotColumnsPartsTest()
        {
            var form = new D_CD_Templates
            {
                Class = "ConsForm",
                Code = "КодФормы",
                InternalName = "IntName",
                Parts =
                    {
                        new D_Form_Part
                            {
                                InternalName = "Part1",
                                Code = "КодРаздела",
                            },
                    }
            };

            var validator = new FormValidator();
            var errors = validator.Validate(form);

            Assert.Contains("У раздела должна быть хотя бы одна графа.", errors);
        }
    }
}
