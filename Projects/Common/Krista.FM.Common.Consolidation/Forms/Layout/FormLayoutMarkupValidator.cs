using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.Common.Consolidation.Forms.Layout
{
    /// <summary>
    /// Отвечает за проверку разметки в книге Excel.
    /// </summary>
    public class FormLayoutMarkupValidator
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

            return Validate(form, mapping);
        }

        [CLSCompliant(false)]
        public List<string> Validate(D_CD_Templates form, Form mapping)
        {
            List<string> errors = new List<string>();

            HSSFWorkbook wb;
            using (var memoryStream = new MemoryStream(form.TemplateFile))
            {
                wb = new HSSFWorkbook(memoryStream);
            }

            if (!errors.Test(mapping.Sheets.Count > 0, "В сопоставлении разметки не задано ни одного листа."))
            {
                return errors;
            }

            string headerRequisitesSheetName = null;
            string footerRequisitesSheetName = null;

            foreach (var xSheet in mapping.Sheets)
            {
                if (!errors.Test(xSheet.Region.IsNotNullOrEmpty(), "Для листа \"{0}\" не указана область.", xSheet))
                {
                    continue;
                }

                if (!errors.Test(wb.RegionExists(xSheet.Region), "Область \"{0}\" не найдена в шаблоне формы.", xSheet))
                {
                    return errors;
                }

                // Область формы на листе
                var formRegion = wb.GetRegion(xSheet.Region);
                var formArea = wb.GetRegionArea(xSheet.Region);

                // Реквизиты заголовка формы
                if (xSheet.HeaderRequisites != null)
                {
                    if (errors.Test(headerRequisitesSheetName == null, "Реквизиты заголовка формы уже определены на листе \"{0}\".", headerRequisitesSheetName))
                    {
                        headerRequisitesSheetName = formRegion.SheetName;

                        errors.AddRange(VisitRequisites(wb, xSheet.HeaderRequisites, formRegion, formArea));
                    }
                }

                // Разделы
                foreach (var xSection in xSheet.Sections)
                {
                    errors.AddRange(VisitSection(wb, xSection, formRegion, formArea, xSheet));
                }

                // Заключительные реквизиты формы
                if (xSheet.FooterRequisites != null)
                {
                    if (errors.Test(footerRequisitesSheetName == null, "Заключительные реквизиты формы уже определены на листе \"{0}\".", headerRequisitesSheetName))
                    {
                        footerRequisitesSheetName = formRegion.SheetName;

                        errors.AddRange(VisitRequisites(wb, xSheet.FooterRequisites, formRegion, formArea));
                    }
                }
            }

            return errors;
        }

        private static List<string> VisitRequisites(HSSFWorkbook wb, RequisiteMap xRequisites, HSSFName parentRegion, AreaReference parentArea)
        {
            List<string> errors = new List<string>();

            if (!errors.Test(xRequisites.Region.IsNotNullOrEmpty(), "Для реквизитов листа \"{0}\" не указана область.", parentRegion.SheetName))
            {
                return errors;
            }

            if (!errors.Test(wb.RegionExists(xRequisites.Region), "Область \"{0}\" не найдена в шаблоне формы.", xRequisites.Region))
            {
                return errors;
            }

            // Область раздела
            var requisitesRegion = wb.GetRegion(xRequisites.Region);
            var requisitesArea = wb.GetRegionArea(xRequisites.Region);

            errors.Test(requisitesRegion.SheetName == parentRegion.SheetName, "Область реквизитов \"{0}\" должна быть расположена на листе \"{1}\".", xRequisites.Region, parentRegion.SheetName);
            errors.Test(parentArea.IsSubArea(requisitesArea), "Область реквизитов \"{0}\" не должна выходить за пределы области \"{1}\".", xRequisites.Region, parentRegion.SheetName);

            foreach (var xRequisite in xRequisites.Requisites)
            {
                errors.AddRange(VisitRequisite(wb, xRequisite, requisitesRegion, requisitesArea));
            }

            return errors;
        }

        private static List<string> VisitRequisite(HSSFWorkbook wb, Element xRequisite, HSSFName parentRegion, AreaReference parentArea)
        {
            List<string> errors = new List<string>();

            if (!errors.Test(xRequisite.Region.IsNotNullOrEmpty(), "Для реквизита листа \"{0}\" не указана область.", parentRegion.SheetName))
            {
                return errors;
            }

            if (!errors.Test(wb.RegionExists(xRequisite.Region), "Область \"{0}\" не найдена в шаблоне формы.", xRequisite))
            {
                return errors;
            }

            // Область реквизита
            var requisiteRegion = wb.GetRegion(xRequisite.Region);
            var requisiteArea = wb.GetRegionArea(xRequisite.Region);

            errors.Test(requisiteRegion.SheetName == parentRegion.SheetName, "Область реквизитов \"{0}\" должна быть расположена на листе \"{1}\".", xRequisite.Region, parentRegion.SheetName);
            errors.Test(parentArea.IsSubArea(requisiteArea), "Область реквизита \"{0}\" не должна выходить за пределы области \"{1}\".", xRequisite.Region, parentRegion.SheetName);

            return errors;
        }

        private static List<string> VisitSection(HSSFWorkbook wb, Section xSection, HSSFName parentRegion, AreaReference parentArea, Sheet xSheet)
        {
            List<string> errors = new List<string>();

            if (!errors.Test(xSection.Region.IsNotNullOrEmpty(), "Не задана область раздела для формы \"{0}\".", parentRegion.NameName))
            {
                return errors;
            }

            if (!errors.Test(wb.RegionExists(xSection.Region), "Область \"{0}\" не найдена в шаблоне формы.", xSection))
            {
                return errors;
            }

            // Область раздела
            var sectionRegion = wb.GetRegion(xSection.Region);
            var sectionArea = wb.GetRegionArea(xSection.Region);

            errors.Test(sectionRegion.SheetName == parentRegion.SheetName, "Область раздела \"{0}\" должна быть расположена на листе \"{1}\".", xSection, parentRegion.SheetName);
            errors.Test(parentArea.IsSubArea(sectionArea), "Область раздела \"{0}\" не должна выходить за пределы области \"{1}\".", xSection, xSheet);

            // Реквизиты заголовка раздела
            if (xSection.HeaderRequisites != null)
            {
                errors.AddRange(VisitRequisites(wb, xSection.HeaderRequisites, sectionRegion, sectionArea));
            }

            errors.AddRange(VisitTable(wb, xSection.Table, sectionRegion, sectionArea));

            // Заключительные реквизиты раздела
            if (xSection.FooterRequisites != null)
            {
                errors.AddRange(VisitRequisites(wb, xSection.FooterRequisites, sectionRegion, sectionArea));
            }

            return errors;
        }

        private static List<string> VisitTable(HSSFWorkbook wb, Table xTable, HSSFName parentRegion, AreaReference sectionArea)
        {
            var errors = new List<string>();

            // Область таблицы
            if (!errors.Test(xTable.Region.IsNotNullOrEmpty(), "Не задана область таблицы раздела \"{0}\".", parentRegion.NameName))
            {
                return errors;
            }

            if (!errors.Test(wb.RegionExists(xTable.Region), "Область \"{0}\" не найдена в шаблоне формы.", xTable))
            {
                return errors;
            }

            var tableRegion = wb.GetRegion(xTable.Region);
            var tableArea = wb.GetRegionArea(xTable.Region);

            errors.Test(tableRegion.SheetName == parentRegion.SheetName, "Область таблицы раздела \"{0}\" должна быть расположена на листе \"{1}\".", xTable, parentRegion.SheetName);
            errors.Test(sectionArea.IsSubArea(tableArea), "Область таблицы раздела \"{0}\" не должна выходить за пределы области раздела \"{1}\".", xTable, parentRegion.NameName);

            if (!errors.Test(xTable.HeaderRegion.IsNotNullOrEmpty(), "Не задана область заголовка таблицы раздела \"{0}\".", parentRegion.NameName))
            {
                return errors;
            }

            if (!errors.Test(wb.RegionExists(xTable.HeaderRegion), "Область \"{0}\" не найдена в шаблоне формы.", xTable.HeaderRegion))
            {
                return errors;
            }

            // Область шапки таблицы
            if (!errors.Test(xTable.HeaderRegion.IsNotNullOrEmpty(), "Не задана область заголовка таблицы раздела \"{0}\".", parentRegion.NameName))
            {
                return errors;
            }

            var tableHeaderRegion = wb.GetRegion(xTable.HeaderRegion);
            var tableHeaderArea = wb.GetRegionArea(xTable.HeaderRegion);

            if (errors.Test(tableHeaderRegion.SheetName == parentRegion.SheetName, "Область шапки таблицы \"{0}\" должна быть расположена на листе \"{1}\".", xTable.HeaderRegion, parentRegion.SheetName))
            {
                // Область шапки таблицы не должна выходить за пределы области таблицы
                errors.Test(tableArea.IsSubArea(tableHeaderArea), "Область шапки таблицы \"{0}\" не должна выходить за пределы области таблицы \"{1}\".", xTable.HeaderRegion, xTable.Region);

                // Графы
                foreach (var xColumn in xTable.Columns)
                {
                    if (!errors.Test(xColumn.Region.IsNotNullOrEmpty(), "Не задана область столбца таблицы \"{0}\".", xTable))
                    {
                        continue;
                    }

                    if (!errors.Test(wb.RegionExists(xColumn.Region), "Область \"{0}\" не найдена в шаблоне формы.", xColumn))
                    {
                        return errors;
                    }

                    var columnRegion = wb.GetRegion(xColumn.Region);
                    var columnArea = wb.GetRegionArea(xColumn.Region);

                    errors.Test(columnRegion.SheetName == parentRegion.SheetName, "Область графы \"{0}\" должна быть расположена на листе \"{1}\".", xColumn, parentRegion.SheetName);
                    errors.Test(tableHeaderArea.IsSubArea(columnArea), "Область графы \"{0}\" не должна выходить за пределы области шапки таблицы \"{1}\".", xColumn, xTable.Region);
                }
            }

            // Область строк таблицы
            if (!errors.Test(xTable.RowsRegion.IsNotNullOrEmpty(), "Не задана область строк таблицы раздела \"{0}\".", parentRegion.NameName))
            {
                return errors;
            }

            if (!errors.Test(wb.RegionExists(xTable.RowsRegion), "Область \"{0}\" не найдена в шаблоне формы.", xTable.RowsRegion))
            {
                return errors;
            }

            var tableBodyRegion = wb.GetRegion(xTable.RowsRegion);
            var tableBodyArea = wb.GetRegionArea(xTable.RowsRegion);

            if (errors.Test(tableBodyRegion.SheetName == parentRegion.SheetName, "Область строк таблицы \"{0}\" должна быть расположена на листе \"{1}\".", xTable.RowsRegion, parentRegion.SheetName))
            {
                // Область строк таблицы не должна выходить за пределы области таблицы
                errors.Test(tableArea.IsSubArea(tableBodyArea), "Область строк таблицы \"{0}\" не должна выходить за пределы области таблицы \"{1}\".", xTable.RowsRegion, xTable.Region);

                // Строки
                for (int rowIndex = 0; rowIndex < tableBodyArea.LastCell.Row - tableBodyArea.FirstCell.Row + 1; rowIndex++)
                {
                    if (!xTable.Rows.Any(x => x.Region == Convert.ToString(rowIndex)))
                    {
                        errors.Add(
                            "Не найдено сопоставление для строки \"{0}\" раздела \"{1}\"."
                                .FormatWith(rowIndex + 1, parentRegion.NameName));
                    }
                }
            }

            return errors;
        }
    }
}
