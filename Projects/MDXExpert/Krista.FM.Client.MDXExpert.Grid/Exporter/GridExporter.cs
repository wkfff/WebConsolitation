using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Grid.Exporter
{
    /// <summary>
    /// Ёкспорт грида
    /// </summary>
    public class GridExporter
    {
        ExcelExporter _excelExporter;
        private ExpertGrid _exportedGrid;

        public GridExporter(ExpertGrid exportedGrid)
        {
            this.ExcelExporter = new ExcelExporter();
            this.ExportedGrid = exportedGrid;
        }

        /// <summary>
        /// Ёкспортировать грид (указынный в конструкторе) в указанную экселевскую книгу
        /// </summary>
        /// <param name="book">экселевска€ книга</param>
        /// <param name="sheetName">им€ листа</param>
        public Excel.Worksheet ToExcelWorkbook(Excel.Workbook book, string sheetName, bool isPrintVersion, bool isSeparateProperties)
        {
            this.ExcelExporter.IsSeparateProperties = isSeparateProperties;
            return this.ExcelExporter.ToWorkbook(this.ExportedGrid, book, sheetName, isPrintVersion);
        }

        /// <summary>
        /// Ёкспортировать указанный грид в указанную экселевскую книгу
        /// </summary>
        /// <param name="exportedGrid">экспортируемый грид</param>
        /// <param name="book">экселевска€ книга</param>
        /// <param name="sheetName">им€ листа</param>
        public Excel.Worksheet ToExcelWorkbook(ExpertGrid exportedGrid, Excel.Workbook book, string sheetName,
            bool isPrintVersion, bool isSeparateProperties)
        {
            this.ExcelExporter.IsSeparateProperties = isSeparateProperties;
            return this.ExcelExporter.ToWorkbook(exportedGrid, book, sheetName, isPrintVersion);
        }

        /// <summary>
        /// Ёкспортировать указанный грид в экселевскую книгу
        /// </summary>
        /// <param name="exportedGrid">экспортируемый грид</param>
        /// <param name="bookPath">путь к книге</param>
        /// <param name="sheetName">им€ листа</param>
        public Excel.Worksheet ToExcelWorkbook(ExpertGrid exportedGrid, string bookPath, string sheetName,
            bool isPrintVersion, bool isSeparateProperties)
        {
            this.ExcelExporter.IsSeparateProperties = isSeparateProperties;
            return this.ExcelExporter.ToWorkbook(exportedGrid, bookPath, sheetName, isPrintVersion);
        }

        /// <summary>
        /// Ёкспортирует грид (указаный в конструкторе) в экселевскую книгу
        /// </summary>
        /// <param name="bookPath">путь к книге</param>
        /// <param name="sheetName">им€ листа</param>
        public Excel.Worksheet ToExcelWorkbook(string bookPath, string sheetName, bool isPrintVersion, bool isSeparateProperties)
        {
            this.ExcelExporter.IsSeparateProperties = isSeparateProperties;
            return this.ExcelExporter.ToWorkbook(this.ExportedGrid, bookPath, sheetName, isPrintVersion);
        }

        /// <summary>
        /// Ёкспортировать указанный грид в экселевский лист
        /// </summary>
        /// <param name="exportedGrid">экспортируемый грид</param>
        /// <param name="sheet">экселевский лист</param>
        public void ToExcelWorksheet(ExpertGrid exportedGrid, Excel.Worksheet sheet, bool isPrintVersion, bool isSeparateProperties)
        {
            this.ExcelExporter.IsSeparateProperties = isSeparateProperties;
            this.ExcelExporter.ToWorksheet(exportedGrid, sheet, isPrintVersion);
        }

        /// <summary>
        /// Ёкспортирует грид (указынный в конструкторе) в экселевский лист
        /// </summary>
        /// <param name="sheet">экселевский лист</param>
        public void ToExcelWorksheet(Excel.Worksheet sheet, bool isPrintVersion, bool isSeparateProperties)
        {
            this.ExcelExporter.IsSeparateProperties = isSeparateProperties;
            this.ExcelExporter.ToWorksheet(this.ExportedGrid, sheet, isPrintVersion);
        }

        /// <summary>
        /// Ёкспортирует грид в экселевский лист, размеща€ с указанных координат
        /// </summary>
        /// <param name="sheet">экселевский лист</param>
        public void ToExcelWorksheet(Excel.Worksheet sheet, Point startLocation, bool isPrintVersion, bool isSeparateProperties)
        {
            this.ExcelExporter.IsSeparateProperties = isSeparateProperties;
            this.ExcelExporter.ToWorksheet(this.ExportedGrid, sheet, startLocation, isPrintVersion);
        }

        /// <summary>
        /// Ёкспортер таблицы в Excel
        /// </summary>
        internal ExcelExporter ExcelExporter
        {
            get { return _excelExporter; }
            set { _excelExporter = value; }
        }

        /// <summary>
        /// Ёкспортируемый грид
        /// </summary>
        internal ExpertGrid ExportedGrid
        {
            get { return _exportedGrid; }
            set { _exportedGrid = value; }
        }
    }
}
