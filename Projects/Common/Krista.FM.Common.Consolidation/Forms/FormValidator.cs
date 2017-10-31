using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Krista.FM.Domain;
using Krista.FM.Extensions;

namespace Krista.FM.Common.Consolidation.Forms
{
    public class FormValidator
    {
        private readonly Regex internalNameEx = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]*$");
        private readonly Regex codeEx = new Regex(@"^[_a-zA-Zа-яА-Я][_a-zA-Z0-9а-яА-Я]*$");

        [CLSCompliant(false)]
        public List<string> Validate(D_CD_Templates form)
        {
            var errors = new List<string>();

            errors.Test(form.InternalName.Length <= 12, "Внутреннее имя формы \"{0}\" длиннее 12 символов.", form.InternalName);
            errors.Test(internalNameEx.IsMatch(form.InternalName), "Некорректное внутреннее имя формы \"{0}\".", form.InternalName);
            errors.Test(codeEx.IsMatch(form.Code), "Некорректный код формы \"{0}\".", form.Code);

            errors.Test(form.Parts.Any(), "У формы должен быть хотя бы один раздел.");

            var partsNameDup = form.Parts.GroupBy(x => x.InternalName)
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .FirstOrDefault(x => x.Count > 1);
            errors.Test(partsNameDup == null, "В разделах есть неуникальное внутреннее имя \"{0}\".", partsNameDup != null ? partsNameDup.Value : null);

            var partsCodeDup = form.Parts.GroupBy(x => x.Code)
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .FirstOrDefault(x => x.Count > 1);
            errors.Test(partsCodeDup == null, "В разделах есть неуникальный код \"{0}\".", partsCodeDup != null ? partsCodeDup.Value : null);

            errors.AddRange(ValidateRequisites(form.Requisites));

            foreach (var part in form.Parts)
            {
                errors.AddRange(ValidateRequisites(part.Requisites));

                errors.AddRange(ValidateSection(part));
            }

            return errors;
        }

        private IEnumerable<string> ValidateRequisites(IEnumerable<D_Form_Requisites> requisites)
        {
            List<string> errors = new List<string>();

            if (requisites != null)
            {
                var headerReqNameDup = requisites.GroupBy(x => x.InternalName)
                    .Select(g => new { Value = g.Key, Count = g.Count() })
                    .FirstOrDefault(x => x.Count > 1);
                errors.Test(headerReqNameDup == null, "В реквизитах есть неуникальной внутреннее имя \"{0}\".", headerReqNameDup != null ? headerReqNameDup.Value : null);

                var headerReqCodeDup = requisites.GroupBy(x => x.Code)
                    .Select(g => new { Value = g.Key, Count = g.Count() })
                    .FirstOrDefault(x => x.Count > 1);
                errors.Test(headerReqCodeDup == null, "В реквизитах есть неуникальный код \"{0}\".", headerReqCodeDup != null ? headerReqCodeDup.Value : null);

                foreach (var requisite in requisites)
                {
                    errors.Test(requisite.InternalName.Length <= 20, "Внутреннее имя реквизита \"{0}\" длиннее 20 символов.", requisite.InternalName);
                    errors.Test(!ReservedWordsClass.ReservedWords.Contains(requisite.InternalName.ToUpper()), "Внутреннее имя реквизита \"{0}\" не может быть использовано, т.к. это зарезервированное слово.", requisite.InternalName);
                    errors.Test(internalNameEx.IsMatch(requisite.InternalName), "Некорректное внутреннее имя реквизита \"{0}\".", requisite.InternalName);
                    errors.Test(codeEx.IsMatch(requisite.Code), "Некорректный код реквизита \"{0}\".", requisite.Code);

                    if (errors.Test(requisite.DataType.IsNotNullOrEmpty(), "Для реквизита \"{0}\" не указан тип данных.", requisite.Code))
                    {
                        var cellType = Type.GetType(requisite.DataType);
                        errors.Test(cellType != null, "Для реквизита \"{0}\" указан неизвестный тип \"{1}\".", requisite.Code, requisite.DataType);
                    }
                }
            }

            return errors;
        }

        private IEnumerable<string> ValidateSection(D_Form_Part part)
        {
            List<string> errors = new List<string>();

            errors.Test(part.InternalName.Length <= 12, "Внутреннее имя раздела \"{0}\" длиннее 12 символов.", part.InternalName);
            errors.Test(internalNameEx.IsMatch(part.InternalName), "Некорректное внутреннее имя раздела \"{0}\".", part.InternalName);
            errors.Test(codeEx.IsMatch(part.Code), "Некорректный код раздела \"{0}\".", part.Code);

            errors.Test(part.Columns.Any(), "У раздела должна быть хотя бы одна графа.");

            var colsNameDup = part.Columns.GroupBy(x => x.InternalName)
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .FirstOrDefault(x => x.Count > 1);
            errors.Test(colsNameDup == null, "В разделе \"{0}\" есть неуникальное внутреннее имя графы \"{1}\".", part.Code, colsNameDup != null ? colsNameDup.Value : null);

            var colsCodeDup = part.Columns.GroupBy(x => x.Code)
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .FirstOrDefault(x => x.Count > 1);
            errors.Test(colsCodeDup == null, "В разделе \"{0}\" есть неуникальный код графы \"{1}\".", part.Code, colsCodeDup != null ? colsCodeDup.Value : null);

            errors.Test(part.Rows.Any(), "У раздела должна быть хотя бы одна строка.");

            var rowsCodeDup = part.Rows.GroupBy(x => x.Name)
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .FirstOrDefault(x => x.Count > 1);
            errors.Test(rowsCodeDup == null, "В разделе \"{0}\" есть неуникальный код строки \"{1}\".", part.Code, rowsCodeDup != null ? rowsCodeDup.Value : null);

            foreach (var column in part.Columns)
            {
                errors.Test(column.InternalName.Length <= 20, "В разделе \"{0}\" внутреннее имя графы \"{1}\" длиннее 20 символов.", part.Code, column.InternalName);
                errors.Test(!ReservedWordsClass.ReservedWords.Contains(column.InternalName.ToUpper()), "Внутреннее имя графы \"{0}\" не может быть использовано, т.к. это зарезервированное слово.", column.InternalName);
                errors.Test(internalNameEx.IsMatch(column.InternalName), "В разделе \"{0}\" есть некорректное внутреннее имя графы \"{1}\".", part.Code, column.InternalName);
                errors.Test(codeEx.IsMatch(column.Code), "В разделе \"{0}\" есть некорректный код графы \"{1}\".", part.Code, column.Code);

                if (errors.Test(column.DataType.IsNotNullOrEmpty(), "В разделе \"{0}\" для графы \"{1}\" не указан тип данных.", part.Code, column.Code))
                {
                    var cellType = Type.GetType(column.DataType);
                    errors.Test(cellType != null, "В разделе \"{0}\" для графы \"{1}\" указан неизвестный тип \"{2}\".", part.Code, column.Code, column.DataType);
                }
            }

            // Проверяем значения по умолчанию для ячеек на возможность операции присвоения
            foreach (var cell in part.Cells)
            {
                if (cell.DefaultValue.IsNotNullOrEmpty())
                {
                    var cellType = Type.GetType(cell.RefColumn.DataType);
                    if (cellType != null)
                    {
                        var typeConverter = TypeDescriptor.GetConverter(cellType);
                        try
                        {
                            typeConverter.ConvertFromString(cell.DefaultValue);
                        }
                        catch (Exception e)
                        {
                            errors.Add("Для ячейки (\"{0}\":\"{1}\") значение по умолчанию \"{2}\" не соответствует формату графы с типом {3} ({4})".FormatWith(cell.RefColumn.Code, cell.RefRow.Name, cell.DefaultValue, cell.RefColumn.DataType, e.Message));
                        }
                    }
                }
            }

            return errors;
        }
    }
}
