using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using NUnit.Framework;

namespace Krista.FM.Common.Consolidation.Tests
{
    [TestFixture]
    public class FormMappingValidatorTests
    {
        [Test]
        public void EmptySheetsTest()
        {
            var form = new D_CD_Templates { InternalName = "FormCode" };
            var mapping = new Form();

            form.SetFormMarkupMappings(mapping);

            var errors = new FormMappingValidator().Validate(form);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("В сопоставлении разметки не задано ни одного листа.", errors[0]);
        }

        [Test]
        public void FormCodeTest()
        {
            var form = new D_CD_Templates { InternalName = "FormCode" };
            var mapping = new Form { Sheets = { new Sheet { Code = "Formcode" } } };

            form.SetFormMarkupMappings(mapping);

            var errors = new FormMappingValidator().Validate(form);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Код формы \"FormCode\" не совпадает с кодом \"Formcode\" области шаблона.", errors[0]);
        }

        [Test]
        public void SectionCodeTest()
        {
            var form = new D_CD_Templates
            {
                InternalName = "FormCode",
                Code = "КодФормы",
                Parts = { new D_Form_Part { InternalName = "SectionCode" } }
            };
            
            var mapping = new Form
            {
                Sheets =
                {
                    new Sheet
                    {
                        Code = "FormCode",
                        Sections = { new Section { Code = "Sectioncode" } }
                    }
                }
            };

            form.SetFormMarkupMappings(mapping);

            var errors = new FormMappingValidator().Validate(form);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Код области раздела \"Sectioncode\" не найден в форме \"КодФормы\".", errors[0]);
        }

        [Test]
        public void ColumnCodeTest()
        {
            var form = new D_CD_Templates
            {
                InternalName = "FormCode",
                Code = "КодФормы",
                Parts = 
                { 
                    new D_Form_Part
                    {
                        InternalName = "SectionCode",
                        Code = "КодРаздела",
                        Columns = { new D_Form_TableColumn { InternalName = "ColumnCode" } }
                    } 
                }
            };

            var mapping = new Form
            {
                Sheets =
                {
                    new Sheet
                    {
                        Code = "FormCode",
                        Sections =
                        {
                            new Section
                            {
                                Code = "SectionCode",
                                Table = new Table { Columns = { new Element { Code = "Columncode" } } }
                            }
                        }
                    }
                }
            };

            form.SetFormMarkupMappings(mapping);

            var errors = new FormMappingValidator().Validate(form);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Код области графы \"Columncode\" не найден в разделе \"КодРаздела\".", errors[0]);
        }

        [Test]
        public void RowCodeTest()
        {
            var form = new D_CD_Templates
            {
                InternalName = "FormCode",
                Code = "КодФормы",
                Parts = 
                { 
                    new D_Form_Part
                    {
                        InternalName = "SectionCode",
                        Code = "КодРаздела",
                        Rows = { new D_Form_TableRow { Name = "RowCode" } }
                    } 
                }
            };

            var mapping = new Form
            {
                Sheets =
                {
                    new Sheet
                    {
                        Code = "FormCode",
                        Sections =
                        {
                            new Section
                            {
                                Code = "SectionCode",
                                Table = new Table { Rows = { new Element { Code = "Rowcode" } } }
                            }
                        }
                    }
                }
            };

            form.SetFormMarkupMappings(mapping);

            var errors = new FormMappingValidator().Validate(form);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("Код области строки \"Rowcode\" не найден в разделе \"КодРаздела\".", errors[0]);
        }

        [Test]
        public void FormRequisiteCodeTest()
        {
            var form = new D_CD_Templates
            {
                InternalName = "FormCode",
                Code = "КодФормы",
                Requisites =
                {
                    new D_Form_Requisites { InternalName = "ReqCode1", IsHeader = true },
                    new D_Form_Requisites { InternalName = "ReqCode2", IsHeader = false },
                }
            };

            var mapping = new Form
            {
                Sheets =
                {
                    new Sheet
                    {
                        Code = "FormCode",
                        HeaderRequisites = new RequisiteMap { Requisites = { new Element { Code = "Reqcode1" } } },
                        FooterRequisites = new RequisiteMap { Requisites = { new Element { Code = "Reqcode2" } } }
                    }
                }
            };

            form.SetFormMarkupMappings(mapping);

            var errors = new FormMappingValidator().Validate(form);

            Assert.AreEqual(2, errors.Count);
            Assert.AreEqual("Код области реквизита \"Reqcode1\" не найден в форме \"КодФормы\".", errors[0]);
            Assert.AreEqual("Код области реквизита \"Reqcode2\" не найден в форме \"КодФормы\".", errors[1]);
        }

        [Test]
        public void PartRequisiteCodeTest()
        {
            var form = new D_CD_Templates
            {
                InternalName = "FormCode",
                Code = "КодФормы",
                Parts =
                {
                    new D_Form_Part
                    {
                        InternalName = "SectionCode",
                        Code = "КодРаздела",
                        Requisites =
                        {
                            new D_Form_Requisites { InternalName = "ReqCode1", IsHeader = true },
                            new D_Form_Requisites { InternalName = "ReqCode2", IsHeader = false },
                        }
                    }
                }
            };

            var mapping = new Form
            {
                Sheets =
                {
                    new Sheet
                    {
                        Code = "FormCode",
                        Sections =
                        {
                            new Section
                            {
                                Code = "SectionCode",
                                HeaderRequisites = new RequisiteMap { Requisites = { new Element { Code = "Reqcode1" } } },
                                FooterRequisites = new RequisiteMap { Requisites = { new Element { Code = "Reqcode2" } } },
                                Table = new Table()
                            }
                        }
                    }
                }
            };

            form.SetFormMarkupMappings(mapping);

            var errors = new FormMappingValidator().Validate(form);

            Assert.AreEqual(2, errors.Count);
            Assert.AreEqual("Код области реквизита \"Reqcode1\" не найден в разделе \"КодРаздела\".", errors[0]);
            Assert.AreEqual("Код области реквизита \"Reqcode2\" не найден в разделе \"КодРаздела\".", errors[1]);
        }
    }
}
