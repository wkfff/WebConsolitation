using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Krista.FM.Common.Consolidation.Forms.Layout
{
    public class FormMappingValidator
    {
        [CLSCompliant(false)]
        public List<string> Validate(D_CD_Templates form)
        {
            Form mapping;
            try
            {
                mapping = form.GetFormMarkupMappings();
            }
            catch (LayoutException e)
            {
                return new List<string> { e.Message };
            }

            List<string> errors = new List<string>();

            if (!errors.Test(mapping.Sheets.Count > 0, "В сопоставлении разметки не задано ни одного листа."))
            {
                return errors;
            }

            // Листы
            foreach (var sheet in mapping.Sheets)
            {
                errors.Test(form.InternalName == sheet.Code, "Код формы \"{0}\" не совпадает с кодом \"{1}\" области шаблона.", form.InternalName, sheet.Code);

                // Реквизиты заголовка
                if (sheet.HeaderRequisites != null)
                {
                    foreach (var requisite in sheet.HeaderRequisites.Requisites)
                    {
                        errors.Test(form.Requisites.Any(x => x.InternalName == requisite.Code && x.IsHeader), "Код области реквизита \"{0}\" не найден в форме \"{1}\".", requisite.Code, form.Code);
                    }
                }

                // Разделы
                foreach (var section in sheet.Sections)
                {
                    if (errors.Test(form.Parts.Any(x => x.InternalName == section.Code), "Код области раздела \"{0}\" не найден в форме \"{1}\".", section.Code, form.Code))
                    {
                        var part = form.Parts.First(x => x.InternalName == section.Code);

                        // Реквизиты заголовка раздела
                        if (section.HeaderRequisites != null)
                        {
                            foreach (var requisite in section.HeaderRequisites.Requisites)
                            {
                                errors.Test(part.Requisites.Any(x => x.InternalName == requisite.Code && x.IsHeader), "Код области реквизита \"{0}\" не найден в разделе \"{1}\".", requisite.Code, part.Code);
                            }
                        }

                        // Колонки
                        foreach (var column in section.Table.Columns)
                        {
                            errors.Test(part.Columns.Any(x => x.InternalName == column.Code), "Код области графы \"{0}\" не найден в разделе \"{1}\".", column.Code, part.Code);
                        }

                        // Строки
                        foreach (var row in section.Table.Rows)
                        {
                            errors.Test(part.Rows.Any(x => x.Name == row.Code), "Код области строки \"{0}\" не найден в разделе \"{1}\".", row.Code, part.Code);
                        }

                        // Реквизиты заключительные раздела
                        if (section.FooterRequisites != null)
                        {
                            foreach (var requisite in section.FooterRequisites.Requisites)
                            {
                                errors.Test(part.Requisites.Any(x => x.InternalName == requisite.Code && !x.IsHeader), "Код области реквизита \"{0}\" не найден в разделе \"{1}\".", requisite.Code, part.Code);
                            }
                        }
                    }
                }

                // Реквизиты заключительные
                if (sheet.FooterRequisites != null)
                {
                    foreach (var requisite in sheet.FooterRequisites.Requisites)
                    {
                        errors.Test(form.Requisites.Any(x => x.InternalName == requisite.Code && !x.IsHeader), "Код области реквизита \"{0}\" не найден в форме \"{1}\".", requisite.Code, form.Code);
                    }
                }
            }

            return errors;
        }
    }
}
