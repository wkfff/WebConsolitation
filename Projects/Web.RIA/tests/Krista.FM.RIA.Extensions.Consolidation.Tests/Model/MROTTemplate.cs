using System.Collections.Generic;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Model
{
    public class MROTTemplate
    {
        public static D_CD_Templates GetTemplate()
        {
            D_CD_Templates template = new D_CD_Templates
            {
                Name = "Name", 
                ShortName = "ShortName", 
                NameCD = "NameCD", 
                Code = "Code", 
                Class = "ConsForm", 
                InternalName = "test"
            };

            var sheetMapping = new Sheet
            {
                Name = "Лист1",
                Region = "Форма",
                HeaderRequisites = new RequisiteMap
                {
                    Region = "Форма_Реквизиты_заголовка",
                    Requisites = new List<Element>
                    {
                        new Element { Code = "Region", Region = "Реквизит_Район" },
                        new Element { Code = "Period", Region = "Реквизит_За_период" }
                    }
                },
                FooterRequisites = new RequisiteMap
                {
                    Region = "Форма_Реквизиты_заключительные",
                    Requisites = new List<Element>
                    {
                        new Element { Code = "Duer", Region = "Реквизит_ФИО_Исполнителя" },
                        new Element { Code = "Phone", Region = "Реквизит_Телефон" },
                        new Element { Code = "FillDate", Region = "Реквизит_Дата_заполнения_отчета" }
                    }
                },
                Sections = new List<Section>
                {
                    new Section
                    {
                        Code = "s1",
                        Region = "Раздел1_Основной",
                        Table = new Table
                        {
                            Region = "Раздел1_Основной",
                            HeaderRegion = "Раздел1_ЗаголовокТаблицы",
                            RowsRegion = "Раздел1_ТелоТаблицы",
                            Columns = new List<Element>
                            {
                                new Element { Code = "Code", Region = "Раздел1_КлонкаКод" },
                                new Element { Code = "Name", Region = "Раздел1_КлонкаНаименование" },
                                new Element { Code = "Total", Region = "Раздел1_КлонкаВсего" },
                                new Element { Code = "Org", Region = "Раздел1_КлонкаКрупные" },
                                new Element { Code = "TinyOrg", Region = "Раздел1_КлонкаМелкие" }
                            },
                            Rows = new List<Element>
                            {
                                new Element { Code = "1.0", Region = "0" },
                                new Element { Code = "1.1", Region = "1" },
                                new Element { Code = "1.2", Region = "2" },
                                new Element { Code = "2.0", Region = "3" },
                                new Element { Code = "2.1", Region = "4" },
                                new Element { Code = "2.2", Region = "5" },
                                new Element { Code = "3", Region = "6" },
                                new Element { Code = "3.0", Region = "7" },
                                new Element { Code = "3.1", Region = "8" },
                                new Element { Code = "3.2", Region = "9" },
                                new Element { Code = "4.0", Region = "10" },
                                new Element { Code = "4.1", Region = "11" },
                                new Element { Code = "4.2", Region = "12" },
                                new Element { Code = "5.0", Region = "13" },
                                new Element { Code = "5.1", Region = "14" },
                                new Element { Code = "5.2", Region = "15" }
                            }
                        }
                    }
                }
            };

            var mapping = new Form
            {
                Sheets = { sheetMapping }
            };

            template.SetFormMarkupMappings(mapping);

            var section = new D_Form_Part
            {
                Name = "Раздел 1",
                Code = "s1", 
                InternalName = "s1", 
                Ord = 1,
                RefForm = template,
                Rows =
                    {
                    }
            };

            section.Columns.Add(new D_Form_TableColumn { Code = "Code", InternalName = "Code", Name = "Code", DataType = "System.Int32", Ord = 0, GroupTag = "1", GroupLevel = 1, RefPart = section });
            section.Columns.Add(new D_Form_TableColumn { Code = "Name", InternalName = "Name", Name = "Code", DataType = "System.Int32", Ord = 1, GroupTag = "1", GroupLevel = 1, RefPart = section });
            section.Columns.Add(new D_Form_TableColumn { Code = "Total", InternalName = "Total", Name = "Code", DataType = "System.Int32", Ord = 2, GroupTag = "1", GroupLevel = 1, RefPart = section });
            section.Columns.Add(new D_Form_TableColumn { Code = "Org", InternalName = "Org", Name = "Code", DataType = "System.Int32", Ord = 3, GroupTag = "1", GroupLevel = 1, RefPart = section });
            section.Columns.Add(new D_Form_TableColumn { Code = "TinyOrg", InternalName = "TinyOrg", Name = "Code", DataType = "System.Int32", Ord = 4, GroupTag = "1", GroupLevel = 1, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "1.0", Ord = 0, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "1.1", Ord = 1, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "1.2", Ord = 2, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "2.0", Ord = 3, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "2.1", Ord = 4, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "2.2", Ord = 5, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "3", Ord = 6, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "3.0", Ord = 7, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "3.1", Ord = 8, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "3.2", Ord = 9, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "4.0", Ord = 10, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "4.1", Ord = 11, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "4.2", Ord = 12, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "5.0", Ord = 13, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "5.1", Ord = 14, Multiplicity = false, RefPart = section });
            section.Rows.Add(new D_Form_TableRow { Name = "5.2", Ord = 15, Multiplicity = false, RefPart = section });

            template.Parts.Add(section);

            template.TemplateFile = Resources.Resource.MROTTemplate;

            return template;
        }
    }
}
