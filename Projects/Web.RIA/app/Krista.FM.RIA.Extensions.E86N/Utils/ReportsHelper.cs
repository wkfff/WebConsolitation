using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;

using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    public class ReportsHelper
    {
        #region Экспорт реестра документов в Excel

        /// <summary>
        /// Записывает данные в книгу
        /// </summary>
        /// <typeparam name="T">Модель данных</typeparam>
        /// <param name="data">Сами данные</param>
        /// <param name="workBook">Книга в которую записывать</param>
        /// <param name="startRow">Строка с которой начинать вывод</param>
        /// <param name="columns">Список ID колонок, которые необходимо отобразить</param>
        public static void BuildModelTable<T>(IEnumerable<T> data, HSSFWorkbook workBook, int startRow = 0, IEnumerable<string> columns = null) where T : ViewModelBase
        {
            var sheet = workBook.GetSheetAt(0);
            var coll = 0;
            var propertys = new List<PropertyInfo>();
            var row = sheet.CreateRow(startRow);
            foreach (var info in typeof(T).GetProperties())
            {
                var dataAttribute = UiBuilders.GetDataAttribute(info);
                string value = dataAttribute != null ? dataAttribute.Caption : string.Empty;

                if (value.Equals(string.Empty))
                {
                    var temp = info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (temp.Length != 0)
                    {
                        value = ((DescriptionAttribute)temp[0]).Description;
                    }
                }
                
                if (columns == null || columns.Contains(info.Name))
                {
                    row.CreateCell(coll);
                    row.GetCell(coll).CellStyle = new HSSFCellStyleForColumns(workBook).CellStyle;
                    row.Height = 500;
                    row.GetCell(coll).SetCellValue(value);
                    sheet.SetColumnWidth(coll, value.Length * 400);
                    coll++;
                    propertys.Add(info);
                }
            }

            coll = 0;
            startRow++;

            var styleForData = new HSSFCellStyleForData(workBook).CellStyle;
            foreach (var item in data)
            {
                row = sheet.CreateRow(startRow);
                foreach (var value in propertys.Select(info => info.GetValue(item, null)))
                {
                    row.CreateCell(coll);
                    row.GetCell(coll).CellStyle = styleForData;
                    row.GetCell(coll).SetCellValue(value == null ? string.Empty : value.ToString());
                    coll++;
                }

                coll = 0;
                startRow++;
            }
        }

        public static string CreateFilterHeader(ViewModelBase model, FilterConditions filters, Sort sort)
        {
            var rez = new StringBuilder(((DescriptionAttribute)model.GetType().GetCustomAttributes(typeof(DescriptionAttribute), false)[0]).Description);
            if (filters.Conditions.Count != 0)
            {
                rez.Append(@" с фильтрами ");
                foreach (var filter in filters.Conditions)
                {
                    var name = model.DescriptionOf(filter.Name);
                    if (filter.FilterType == FilterType.String)
                    {
                        rez.Append(string.Concat('[', name, ":", filter.Value, @"] "));
                        continue;
                    }

                    if (filter.FilterType == FilterType.Numeric || filter.FilterType == FilterType.Date)
                    {
                        rez.Append(string.Concat('[', name));
                        switch (filter.Comparison)
                        {
                            case Comparison.Eq:
                                rez.Append("=");
                                break;
                            case Comparison.Lt:
                                rez.Append("<");
                                break;
                            case Comparison.Gt:
                                rez.Append(">");
                                break;
                        }

                        rez.Append(filter.Value);
                        rez.Append(@"] ");
                        continue;
                    }

                    if (filter.FilterType == FilterType.Boolean)
                    {
                        rez.Append(string.Concat('[', name, @"] "));
                        continue;
                    }

                    if (filter.FilterType == FilterType.List)
                    {
                        rez.Append(string.Concat('[', name, ":"));
                        filter.ValuesList.Each(x => rez.Append(string.Concat(" ", x, ", ")));
                        rez.Remove(rez.Length - 2, 2);
                        rez.Append(@"] ");
                    }
                }
            }

            if (sort != null)
            {
                rez.Append(", c сортировками ")
                    .Append('[' + model.DescriptionOf(sort.Field) + ":" + " ")
                    .Append(sort.Direction.ToLower() == "asc" ? "по возрастанию]" : "по убыванию]");
            }

            return rez.ToString();
        }

        public static string CreateFirstHeader(FilterConditions filters, List<Sort> sorts)
        {
            var stringFilters = new StringBuilder();
            if (filters.Conditions.Count != 0)
            {
                foreach (var filter in filters.Conditions)
                {
                    stringFilters.Append(filter == filters.Conditions.First() ? " с фильтрами " : string.Empty);
                    switch (filter.FilterType)
                    {
                        case FilterType.Boolean:
                            stringFilters.Append('[' + GetRightColumnsName(filter.Name) + "]");
                            break;
                        case FilterType.Date:
                            stringFilters.Append('[' + GetRightColumnsName(filter.Name));
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    stringFilters.Append("=");
                                    break;
                                case Comparison.Lt:
                                    stringFilters.Append("<");
                                    break;
                                case Comparison.Gt:
                                    stringFilters.Append(">");
                                    break;
                            }

                            stringFilters.Append(filter.Value + "]");
                            break;
                        case FilterType.List:
                            stringFilters.Append('[' + GetRightColumnsName(filter.Name) + ":");
                            filter.ValuesList.Each(x => stringFilters.Append(" " + x + ',' + " "));
                            stringFilters.Remove(stringFilters.Length - 2, 2);
                            stringFilters.Append("]");
                            break;
                        case FilterType.String:
                            stringFilters.Append('[' + GetRightColumnsName(filter.Name) + ":" + " " + filter.Value + "]");
                            break;
                    }
                }
            }

            var stringSorters = new StringBuilder();
            if (sorts.Count != 0)
            {
                foreach (var sort in sorts.Where(sort => sort.Field != "ID"))
                {
                    stringSorters.Append(sort == sorts.First() ? ", c сортировками " : string.Empty)
                        .Append('[' + GetRightColumnsName(sort.Field) + ":" + " ")
                        .Append(sort.Direction.ToLower() == "asc" ? "по возрастанию]" : "по убыванию]");
                }
            }

            return
                "Реестр документов {0} {1}".FormatWith(stringFilters, stringSorters);
        }

        public static string CreateSecondHeader()
        {
            return "(по состоянию на {0})".FormatWith(DateTime.Now.ToShortDateString());
        }

        public static List<Column> GetRightColumnsNames(List<Column> columns)
        {
            columns.ForEach(
                x =>
                {
                    switch (x.ID)
                    {
                        case "StructureCloseDate":
                            x.ID = "Дата закрытия учреждения";
                            break;
                        case "StructureName":
                            x.ID = "Название";
                            break;
                        case "StructureGrbs":
                            x.ID = "ГРБС";
                            break;
                        case "StructurePpo":
                            x.ID = "ППО";
                            break;
                        case "State":
                            x.ID = "Состояние";
                            break;
                        case "Type":
                            x.ID = "Документ";
                            break;
                        case "Note":
                            x.ID = "Примечание";
                            break;
                        case "Year":
                            x.ID = "Год формирования";
                            break;
                        case "StructureInn":
                            x.ID = "ИНН";
                            break;
                        case "StructureKpp":
                            x.ID = "КПП";
                            break;
                        case "StructureType":
                            x.ID = "Тип учреждения";
                            break;
                    }
                });
            return columns;
        }

        public static string GetRightColumnsName(string name)
        {
            switch (name)
            {
                case "StructureCloseDate":
                    return "Дата закрытия учреждения";
                case "StructureName":
                    return "Название";
                case "StructureGrbs":
                    return "ГРБС";
                case "StructurePpo":
                    return "ППО";
                case "State":
                    return "Состояние";
                case "Type":
                    return "Документ";
                case "Note":
                    return "Примечание";
                case "Year":
                    return "Год формирования";
                case "Closed":
                    return "Закрытые документы";
                case "NotClosed":
                    return "Открытые документы";
                case "ClosedOrg":
                    return "Закрытые учреждения";
                case "NotClosedOrg":
                    return "Открытые учреждения";
                case "StructureType":
                    return "Тип учреждения";
            }

            return string.Empty;
        }

        #endregion Экспорт реестра документов в Excel

        #region Стили для HSSFWorkBook

        #region Nested type: HSSFCellStyleBoldText

        public struct HSSFCellStyleBoldText
        {
            public HSSFCellStyle CellStyle;

            public HSSFCellStyleBoldText(HSSFWorkbook workBook)
            {
                var boldFont = workBook.CreateFont();
                boldFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
                CellStyle = workBook.CreateCellStyle();
                CellStyle.SetFont(boldFont);
            }
        }

        #endregion

        #region Nested type: HSSFCellStyleForColumns

        public struct HSSFCellStyleForColumns
        {
            public HSSFCellStyle CellStyle;

            public HSSFCellStyleForColumns(HSSFWorkbook workBook)
            {
                var boldFont = workBook.CreateFont();
                boldFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
                CellStyle = workBook.CreateCellStyle();
                CellStyle.SetFont(boldFont);
                CellStyle.WrapText = true;
                CellStyle.BorderBottom = HSSFBorderFormatting.BORDER_THIN;
                CellStyle.BorderLeft = HSSFBorderFormatting.BORDER_THIN;
                CellStyle.BorderRight = HSSFBorderFormatting.BORDER_THIN;
                CellStyle.BorderTop = HSSFBorderFormatting.BORDER_THIN;

                CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                CellStyle.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            }
        }

        #endregion

        #region Nested type: HSSFCellStyleForData

        public struct HSSFCellStyleForData
        {
            public HSSFCellStyle CellStyle;

            public HSSFCellStyleForData(HSSFWorkbook workBook)
            {
                CellStyle = workBook.CreateCellStyle();
                CellStyle.WrapText = true;
                CellStyle.BorderBottom = HSSFBorderFormatting.BORDER_THIN;
                CellStyle.BorderLeft = HSSFBorderFormatting.BORDER_THIN;
                CellStyle.BorderRight = HSSFBorderFormatting.BORDER_THIN;
                CellStyle.BorderTop = HSSFBorderFormatting.BORDER_THIN;
                CellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            }
        }

        #endregion

        #region Nested type: HSSFCellStyleForHeader

        public struct HSSFCellStyleForHeader
        {
            public HSSFCellStyle CellStyle;

            public HSSFCellStyleForHeader(HSSFWorkbook workBook)
            {
                var boldFont = workBook.CreateFont();
                boldFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
                CellStyle = workBook.CreateCellStyle();
                CellStyle.WrapText = true;
                CellStyle.SetFont(boldFont);
                CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            }
        }

        #endregion

        #endregion Стили для HSSFWorkBook

        #region Nested type: Column

        public class Column
        {
            public string ID { get; set; }

            public int Width { get; set; }

            public bool Hidden { get; set; }
        }

        #endregion

        #region Nested type: ColumnsAndSort

        public class ColumnsAndSort
        {
            public List<Column> Columns { get; set; }

            public Sort Sort { get; set; }
        }

        #endregion

        #region Nested type: ColumnsAndSorts

        public class ColumnsAndSorts
        {
            public List<Column> Columns { get; set; }

            public List<Sort> Sort { get; set; }
        }

        #endregion

        #region Nested type: Sort

        public class Sort
        {
            public string Field { get; set; }

            public string Direction { get; set; }
        }

        #endregion

        #region Nested type: Sorts

        public class Sorts
        {
            public List<Sort> Sort { get; set; }
        }

        #endregion
    }
}
