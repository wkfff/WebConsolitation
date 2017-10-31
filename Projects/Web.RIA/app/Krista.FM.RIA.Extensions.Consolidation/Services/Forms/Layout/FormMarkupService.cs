using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ext.Net;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Common.Consolidation.Forms.ExcelMapping;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.Consolidation.Models;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms
{
    /// <summary>
    /// Предоставляет функционал по работе с разметкой шаблона формы листа Ecxel.
    /// </summary>
    public class FormLayoutMarkupService
    {
        /// <summary>
        /// Возвращает разметку шаблона формы листа Ecxel.
        /// </summary>
        /// <param name="form">Метаданные описывающие структуру формы.</param>
        /// <param name="mapping">Метаданные визуальной структуры. Описание размещения элементов формы на листе Excel.</param>
        public LayoutMarkupViewModel GetLayoutMarkup(D_CD_Templates form, Form mapping)
        {
            if (mapping.Sheets.Count == 0)
            {
                throw new LayoutException("В разметке шаблона формы не найдено ни одной области формы.");
            }

            HSSFWorkbook wb;
            using (var memoryStream = new MemoryStream(form.TemplateFile))
            {
                wb = new HSSFWorkbook(memoryStream);
            }

            var sheet = wb.GetSheet(mapping.Sheets[0].Code);

            var model = new LayoutMarkupViewModel();

            var formRegion = wb.GetNameIndex(mapping.Sheets[0].Region);
            var region = wb.GetNameAt(formRegion);
            var area = new AreaReference(region.Reference);

            model.TotalColumns = area.LastCell.Col - area.FirstCell.Col + 1;

            FormLayoutMarkupExtractor markupExtractor = new FormLayoutMarkupExtractor(wb, sheet, model);
            for (int rowIndex = area.FirstCell.Row, rowMetaIndex = 0; rowIndex <= area.LastCell.Row; rowIndex++, rowMetaIndex++)
            {
                bool markNewRow = true;
                for (int colIndex = area.FirstCell.Col; colIndex <= area.LastCell.Col; colIndex++)
                {
                    var cell = markupExtractor.GetCell(rowIndex, colIndex);
                    if (cell == null)
                    {
                        continue;
                    }

                    model.Cells.Add(cell);

                    if (markNewRow)
                    {
                        var value = new List<string>();
                        value.Add("ux-grid-static-row");
                        value.Add("ux-grid-multi-row");
                        model.Rows.Add(rowMetaIndex, value);
                    }

                    markNewRow = false;
                }
            }

            return model;
        }

        /// <summary>
        /// Возвращает разметку реквизитов формы.
        /// </summary>
        /// <param name="form">Метаданные описывающие структуру формы.</param>
        /// <param name="mapping">Метаданные визуальной структуры. Описание размещения элементов формы на листе Excel.</param>
        /// <param name="requisiteKind">Вид реквизитов.</param>
        public FormGridViewModel GetFormRequisite(D_CD_Templates form, Sheet mapping, RequisiteKinds requisiteKind)
        {
            var requisites = requisiteKind == RequisiteKinds.Header ? mapping.HeaderRequisites : mapping.FooterRequisites;
            if (requisites == null)
            {
                throw new LayoutException("В разметке шаблона формы не найдено реквизитов \"{0}\" части.".FormatWith(requisiteKind == RequisiteKinds.Header ? "заголовочной" : "заключительной"));
            }

            HSSFWorkbook wb;
            using (var memoryStream = new MemoryStream(form.TemplateFile))
            {
                wb = new HSSFWorkbook(memoryStream);
            }

            var region = wb.GetRegion(requisites.Region);
            var area = wb.GetRegionArea(requisites.Region);
            var sheet = wb.GetSheet(region.SheetName);

            var formRequisites = form.Requisites.Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header));
            List<ColumnBase> columns = new List<ColumnBase>();
            Dictionary<CellReference, string> metaColumns = new Dictionary<CellReference, string>();
            foreach (var xRequisite in requisites.Requisites)
            {
                var requisiteArea = wb.GetRegionArea(xRequisite.Region);
                string requisiteCode = xRequisite.Code;
                metaColumns.Add(requisiteArea.FirstCell, requisiteCode);

                D_Form_Requisites requisite = formRequisites.FirstOrDefault(x => x.InternalName == requisiteCode);
                if (requisite != null)
                {
                    var column = new Column
                    {
                        DataIndex = requisite.InternalName,
                        ColumnID = requisite.InternalName,
                        Header = requisite.Name
                    };
                    if (!requisite.ReadOnly)
                    {
                        column.SetEditableString();
                    }

                    columns.Add(column);
                }
            }

            var model = new LayoutMarkupViewModel();
            model.TotalColumns = area.LastCell.Col - area.FirstCell.Col + 1;
            model.FirstRow = 0;
            model.LastRow = area.LastCell.Row - area.FirstCell.Row + 1;

            FormLayoutMarkupExtractor markupExtractor = new FormLayoutMarkupExtractor(wb, sheet, model);
            for (int rowIndex = area.FirstCell.Row, rowMetaIndex = 0; rowIndex <= area.LastCell.Row; rowIndex++, rowMetaIndex++)
            {
                for (int colIndex = area.FirstCell.Col; colIndex <= area.LastCell.Col; colIndex++)
                {
                    var cell = markupExtractor.GetCell(rowIndex, colIndex);
                    if (cell == null)
                    {
                        continue;
                    }

                    foreach (var metaColumn in metaColumns)
                    {
                        if (metaColumn.Key.Col == colIndex && metaColumn.Key.Row == rowIndex)
                        {
                            cell.ColumnId = metaColumn.Value;
                            break;
                        }
                    }

                    model.Cells.Add(cell);
                }

                model.Height += Convert.ToInt16(Math.Round(sheet.GetRow(rowIndex).Height / 15.46)); 
            }

            return new FormGridViewModel { Layout = model, Columns = columns };
        }

        /// <summary>
        /// Возвращает разметку реквизитов раздела формы.
        /// </summary>
        /// <param name="form">Метаданные описывающие структуру формы.</param>
        /// <param name="mapping">Метаданные визуальной структуры. Описание размещения элементов формы на листе Excel.</param>
        /// <param name="sectionCode">Код раздела формы.</param>
        /// <param name="requisiteKind">Вид реквизитов.</param>
        public FormGridViewModel GetSectionRequisite(D_CD_Templates form, Sheet mapping, string sectionCode, RequisiteKinds requisiteKind)
        {
            var section = mapping.Sections.Where(x => x.Code == sectionCode).FirstOrDefault();
            if (section == null)
            {
                throw new LayoutException("В разметке шаблона формы не найдено раздела с кодом \"{0}\".".FormatWith(sectionCode));
            }

            var requisites = requisiteKind == RequisiteKinds.Header ? section.HeaderRequisites : section.FooterRequisites;
            if (requisites == null)
            {
                return null;
            }

            HSSFWorkbook wb;
            using (var memoryStream = new MemoryStream(form.TemplateFile))
            {
                wb = new HSSFWorkbook(memoryStream);
            }

            var region = wb.GetRegion(requisites.Region);
            var area = wb.GetRegionArea(requisites.Region);
            var sheet = wb.GetSheet(region.SheetName);

            var sectionRequisites = form.Parts.First(x => x.InternalName == sectionCode).Requisites.Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header));
            List<ColumnBase> columns = new List<ColumnBase>();
            Dictionary<CellReference, string> metaColumns = new Dictionary<CellReference, string>();
            foreach (var xRequisite in requisites.Requisites)
            {
                var requisiteArea = wb.GetRegionArea(xRequisite.Region);
                string requisiteCode = xRequisite.Code;
                metaColumns.Add(requisiteArea.FirstCell, requisiteCode);

                D_Form_Requisites requisite = sectionRequisites.FirstOrDefault(x => x.InternalName == requisiteCode);
                if (requisite != null)
                {
                    var column = new Column
                    {
                        DataIndex = requisite.InternalName,
                        ColumnID = requisite.InternalName,
                        Header = requisite.Name
                    };
                    if (!requisite.ReadOnly)
                    {
                        column.SetEditableString();
                    }

                    columns.Add(column);
                }
            }

            var model = new LayoutMarkupViewModel();
            model.TotalColumns = area.LastCell.Col - area.FirstCell.Col + 1;
            model.FirstRow = 0;
            model.LastRow = area.LastCell.Row - area.FirstCell.Row + 1;

            FormLayoutMarkupExtractor markupExtractor = new FormLayoutMarkupExtractor(wb, sheet, model);
            for (int rowIndex = area.FirstCell.Row, rowMetaIndex = 0; rowIndex <= area.LastCell.Row; rowIndex++, rowMetaIndex++)
            {
                for (int colIndex = area.FirstCell.Col; colIndex <= area.LastCell.Col; colIndex++)
                {
                    var cell = markupExtractor.GetCell(rowIndex, colIndex);
                    if (cell == null)
                    {
                        continue;
                    }

                    foreach (var metaColumn in metaColumns)
                    {
                        if (metaColumn.Key.Col == colIndex && metaColumn.Key.Row == rowIndex)
                        {
                            cell.ColumnId = metaColumn.Value;
                            break;
                        }
                    }

                    model.Cells.Add(cell);
                }

                model.Height += Convert.ToInt16(Math.Round(sheet.GetRow(rowIndex).Height / 15.46));
            }

            return new FormGridViewModel { Layout = model, Columns = columns };
        }

        /// <summary>
        /// Возвращает разметку реквизитов раздела формы.
        /// </summary>
        /// <param name="form">Метаданные описывающие структуру формы.</param>
        /// <param name="mapping">Метаданные визуальной структуры. Описание размещения элементов формы на листе Excel.</param>
        /// <param name="sectionCode">Код раздела формы.</param>
        public FormGridViewModel GetSectionTable(D_CD_Templates form, Sheet mapping, string sectionCode)
        {
            var section = form.Parts.FirstOrDefault(x => x.InternalName == sectionCode);
            if (section == null)
            {
                throw new LayoutException("В метаданных формы не найден запрашиваемый раздел \"{0}\"".FormatWith(sectionCode));
            }

            var xSection = GetXSection(mapping, sectionCode);

            // Формируем описание метаданных строк
            Dictionary<int, string> metaRows = new Dictionary<int, string>();
            foreach (var xRow in xSection.Table.Rows)
            {
                metaRows.Add(Convert.ToInt32(xRow.Region), xRow.Code);
            }

            HSSFWorkbook wb;
            using (var memoryStream = new MemoryStream(form.TemplateFile))
            {
                wb = new HSSFWorkbook(memoryStream);
            }

            var tableRegion = wb.GetRegion(xSection.Table.Region);
            var sheet = wb.GetSheet(tableRegion.SheetName);

            // Область таблицы раздела
            var area = wb.GetRegionArea(xSection.Table.Region);
            
            // Область строк таблицы раздела
            var tableRowsArea = wb.GetRegionArea(xSection.Table.RowsRegion);

            List<ColumnBase> columns = new List<ColumnBase>();
            Dictionary<int, string> metaColumns = new Dictionary<int, string>();
            foreach (var xColumn in xSection.Table.Columns)
            {
                var columnArea = wb.GetRegionArea(xColumn.Region);
                string columnCode = xColumn.Code;
                metaColumns.Add(columnArea.FirstCell.Col, columnCode);

                var column = section.Columns.First(x => x.InternalName == columnCode);
                var extColumn = new Column { DataIndex = column.InternalName, ColumnID = column.InternalName, Header = column.Name };
                if (!column.ReadOnly)
                {
                    if (column.DataType == "System.String")
                    {
                        extColumn.SetEditableString();
                    }
                    else
                    {
                        extColumn.SetEditableDouble(2);
                    }
                }

                columns.Add(extColumn);
            }

            var model = new LayoutMarkupViewModel();
            model.TotalColumns = area.LastCell.Col - area.FirstCell.Col + 1;
            model.FirstRow = tableRowsArea.FirstCell.Row - area.FirstCell.Row;
            model.LastRow = tableRowsArea.LastCell.Row - area.FirstCell.Row;

            FormLayoutMarkupExtractor markupExtractor = new FormLayoutMarkupExtractor(wb, sheet, model);
            for (int rowIndex = area.FirstCell.Row, rowMetaIndex = 0; rowIndex <= area.LastCell.Row; rowIndex++, rowMetaIndex++)
            {
                bool markNewRow = true;
                for (int colIndex = area.FirstCell.Col; colIndex <= area.LastCell.Col; colIndex++)
                {
                    var cell = markupExtractor.GetCell(rowIndex, colIndex);
                    if (cell == null)
                    {
                        continue;
                    }

                    // Добавляем метаданные для колонки
                    if (metaColumns.ContainsKey(colIndex) && RowInRegion(rowIndex, tableRowsArea))
                    {
                        cell.ColumnId = metaColumns[colIndex];
                    }

                    // Добавляем метаданные для строки
                    if (markNewRow)
                    {
                        if (RowInRegion(rowIndex, tableRowsArea))
                        {
                            var metaRowIndex = rowIndex - tableRowsArea.FirstCell.Row;
                            if (metaRows.ContainsKey(metaRowIndex))
                            {
                                var metaRowCode = metaRows[metaRowIndex];
                                var row = section.Rows.FirstOrDefault(x => x.Name == metaRowCode);
                                if (row == null)
                                {
                                    throw new LayoutException(
                                        "В метаданных формы не найдена запрашиваемая строка \"{0}\" в разделе \"{1}\"".
                                            FormatWith(metaRowCode, sectionCode));
                                }

                                var value = new List<string>();
                                value.Add(row.Multiplicity ? "ux-excelgrid-row-user" : "ux-excelgrid-row-fixed");
                                value.Add("ux-excelgrid-metarow-{0}".FormatWith(row.ID));
                                model.Rows.Add(rowMetaIndex, value);
                            }
                        }
                        else
                        {
                            model.Rows.Add(rowMetaIndex, new List<string> { "ux-excelgrid-row-whitespace" });
                        }
                    }

                    model.Cells.Add(cell);

                    markNewRow = false;
                }
            
                model.Height += Convert.ToInt16(Math.Round(sheet.GetRow(rowIndex).Height / 15.46));
            }

            return new FormGridViewModel { Layout = model, Columns = columns };
        }

        /// <summary>
        /// Проверяет попадает ли строка с индексом rowIndex в диапазон tableBodyArea.
        /// </summary>
        /// <param name="rowIndex">Индекс строки.</param>
        /// <param name="tableBodyArea">Диапазон для проверки.</param>
        private static bool RowInRegion(int rowIndex, AreaReference tableBodyArea)
        {
            return rowIndex >= tableBodyArea.FirstCell.Row && rowIndex <= tableBodyArea.LastCell.Row;
        }

        private static Section GetXSection(Sheet mapping, string sectionCode)
        {
            var section = mapping.Sections.Where(x => x.Code == sectionCode).FirstOrDefault();
            if (section == null)
            {
                throw new LayoutException("В разметке шаблона формы не найдено раздела с кодом \"{0}\".".FormatWith(sectionCode));
            }

            return section;
        }
    }
}
