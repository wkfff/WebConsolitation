using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Grid.Exporter
{
    /// <summary>
    /// Класс для экспорта грида в Excel
    /// </summary>
    class ExcelExporter
    {
        private ExpertGrid _exportedGrid;

        //начальные координаты ячеек для размещения коллекций грида
        private Point startGridLocation = Point.Empty;
        private Point endGridLocation = Point.Empty;
        private Point filterCaptionsLocation = Point.Empty;
        private Point columnCaptionsLocation = Point.Empty;
        private Point columnLocation = Point.Empty;
        private Point rowCaptionsLocation = Point.Empty;
        private Point rowLocation = Point.Empty;
        private Point measureCaptionsLocation = Point.Empty;
        private Point measureLocation = Point.Empty;

        const string expertGridName = "ExpertGrid";

        private bool _isSeparateProperties = false;

        public bool IsSeparateProperties
        {
            get { return this._isSeparateProperties; }
            set { this._isSeparateProperties = value; }
        }

        public ExcelExporter()
        {
        }

        /// <summary>
        /// Экспорт грида по указаному пути книги, и имени листа
        /// </summary>
        /// <param name="exportedGrid">экспортируемый грид</param>
        /// <param name="bookPath">путь к ниге (если нет такой, то создаст)</param>
        /// <param name="sheetName">имя листа</param>
        public Excel.Worksheet ToWorkbook(ExpertGrid exportedGrid, string bookPath, string sheetName,
            bool isPrintVersion)
        {
            Excel.Application excelApl = ExcelUtils.StartExcel(true);

            Excel.Workbook book = ExcelUtils.OpenWorkbook(excelApl, bookPath, true);
            return this.ToWorkbook(exportedGrid, book, sheetName, isPrintVersion);

        }

        /// <summary>
        /// Экспорт листа в указанную экселевскую книгу
        /// </summary>
        /// <param name="exportedGrid">экспортируемый грид</param>
        /// <param name="book">экселевская книга</param>
        /// <param name="sheetName">имя листа</param>
        public Excel.Worksheet ToWorkbook(ExpertGrid exportedGrid, Excel.Workbook book, string sheetName, 
            bool isPrintVersion)
        {
            if ((book == null) || (exportedGrid == null))
                return null;

            this.ExportedGrid = exportedGrid;

            Excel.Worksheet sheet = ExcelUtils.GetWorksheet(book.Sheets.Add(Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing));
            sheet.Name = ExcelUtils.GetSheetName(book, sheetName);
   
            this.ToWorksheet(exportedGrid, sheet, isPrintVersion);
            return sheet;
        }

        public void ToWorksheet(ExpertGrid exportedGrid, Excel.Worksheet sheet, bool isPrintVersion)
        {
            this.ToWorksheet(exportedGrid, sheet, this.GetGridStartLocation(sheet), isPrintVersion);
        }

        public void ToWorksheet(ExpertGrid exportedGrid, Excel.Worksheet sheet, Point startLocation, 
            bool isPrintVersion)
        {
            if ((sheet == null) || (exportedGrid == null))
                return;

            string sheetName = sheet.Name;
            Excel.Application excelAplication = sheet.Application;
            
            bool customScreenUpdating = excelAplication.ScreenUpdating;
            try
            {
                ExcelUtils.SetScreenUpdating(excelAplication, false);

                //Координаты ячейки листа, с нее наченем размещать данные грида
                this.startGridLocation = startLocation;
                this.MapGrid(exportedGrid, sheet, isPrintVersion);
                ExcelUtils.SetScreenUpdating(excelAplication, customScreenUpdating);
            }
            catch
            {
                ExcelUtils.SetScreenUpdating(excelAplication, true);
                throw new Exception(string.Format("Во время экспорта в Excel элемента отчета \"{0}\", произошла ошибка.", sheetName));
            }
        }

        private void MapGrid(ExpertGrid exportedGrid, Excel.Worksheet sheet, bool isPrintVersion)
        {

            bool isExistExpertGrid = this.IsExistExpertGrid(sheet);

            this.DetermineCollectionLocation(this.startGridLocation, exportedGrid, isExistExpertGrid);

            if (!isExistExpertGrid)
            {
                this.MapFilter(sheet, this.filterCaptionsLocation, exportedGrid.FilterCaptions, isPrintVersion);
                this.MapColumnCaptions(sheet, this.columnCaptionsLocation, exportedGrid.ColumnCaptions, isPrintVersion);
                this.MapColumnAxis(sheet, this.columnLocation, exportedGrid.Column, isPrintVersion);
                this.MapRowCaptions(sheet, this.rowCaptionsLocation, exportedGrid.RowCaptions, isPrintVersion);
                this.MapRowAxis(sheet, this.rowLocation, exportedGrid.Row, isPrintVersion);
                this.MapMeasureCaptions(sheet, this.measureCaptionsLocation, exportedGrid.MeasureCaptionsSections, isPrintVersion);
                this.MapMeasureData(sheet, this.measureLocation, exportedGrid.MeasuresData, isPrintVersion);
                this.DeleteDuplicateNames(sheet);
            }
            else
            {
                this.MapRowAxis(sheet, this.rowLocation, exportedGrid.Row, isPrintVersion);
                this.MapMeasureData(sheet, this.measureLocation, exportedGrid.MeasuresData, isPrintVersion);
                this.DeleteDuplicate(sheet);
            }

            int tableWidth = this.GetGridWidth(exportedGrid) - 1;
            if (tableWidth < 1)
                tableWidth = 1;

            int tableHeight = this.GetGridHeight(exportedGrid, isExistExpertGrid);

            Excel.Range gridRange = null;
            if (isExistExpertGrid)
            {
                Excel.Range oldGridRange = ExcelUtils.GetExcelName(sheet, expertGridName).RefersToRange;
                gridRange = ExcelUtils.GetExcelRange(sheet, oldGridRange.Row, oldGridRange.Column,
                    oldGridRange.Row + oldGridRange.Rows.Count + tableHeight - 2, this.startGridLocation.Y + tableWidth);
                gridRange = ExcelUtils.UnionRange(gridRange, oldGridRange);
            }
            else
            {
                this.endGridLocation = new Point(this.startGridLocation.X + tableHeight,
                    this.startGridLocation.Y + tableWidth);
                gridRange = ExcelUtils.GetExcelRange(sheet, this.startGridLocation, this.endGridLocation);
            }
            ExcelUtils.MarkExcelName(gridRange, expertGridName, false);
        }

        private int GetGridWidth(ExpertGrid grid)
        {
            int result = Math.Max(grid.FilterCaptions.Count, grid.Column.LeafCount + 1);
            //здесь узнаем количество ячеек которое распологается в строке с заголовками строк и мер
            result = Math.Max(result, grid.RowCaptions.Count
                + grid.MeasureCaptionsSections.GetMeasuresCaptions().Length
                + (grid.MeasuresStub.IsVisible ? 1 : 0));

            return result;
        }

        private int GetGridHeight(ExpertGrid grid, bool isExistExpertGrid)
        {
            int result = 0;
            //фильтры таблицы
            if (!grid.FilterCaptions.IsEmpty && grid.FilterCaptions.Visible)
            {
                result += isExistExpertGrid ? 0 : 2;
            }
            //заголовки столбцов
            result += isExistExpertGrid ? 0 : grid.ColumnCaptions.Count - 1;

            //заголовки строк и заголовки мер
            if (!grid.RowCaptions.IsEmpty || !grid.MeasureCaptionsSections.IsEmpty)
                result++;
            //строки и меры
            if (!grid.MeasuresData.IsEmpty)
            {
                foreach (MeasureCell mCell in grid.MeasuresData[0])
                {
                    if (mCell.IsVisible)
                        result ++;
                }
            }
            else
                if (!grid.Row.IsEmpty)
                {
                    result += grid.Row.GetLeafCount();
                }
            return result;
        }

        private void MapFilter(Excel.Worksheet sheet, Point startLocation, FilterCaptions filterCaptions,
            bool isPrintVersion)
        {
            if (filterCaptions.Visible && !filterCaptions.IsEmpty)
            {
                Excel.Range filterCaptionRange = null;
                Excel.Range filterValueRange = null;
                foreach (CaptionCell caption in filterCaptions)
                {
                    Excel.Range captionRange = ExcelUtils.MapExcelCell(sheet, caption.Text, startLocation);
                    Point valueLocation = new Point(startLocation.X + 1, startLocation.Y);
                    Excel.Range valueRange = ExcelUtils.MapExcelCell(sheet, caption.OptionalCell.Text, valueLocation);

                    //выставим такие же размеры как у заголовка в гриде
                    this.SetExcelCellSize(captionRange, caption.OriginalSize);
                    this.SetExcelCellSize(valueRange, caption.OptionalCell.OriginalSize);

                    filterCaptionRange = ExcelUtils.UnionRange(filterCaptionRange, captionRange);
                    filterValueRange = ExcelUtils.UnionRange(filterValueRange, valueRange);
                    startLocation.Y++;
                }
                this.SetCaptionCollectionStyle(filterCaptionRange, filterCaptions.Style, isPrintVersion);
                this.SetAxisStyle(filterValueRange);
                this.SetRangeStyle(filterValueRange, filterCaptions.ValueCellStyle, isPrintVersion);
            }
        }

        private void MapColumnCaptions(Excel.Worksheet sheet, Point startLocation, ColumnCaptions columnCaptions,
            bool isPrintVersion)
        {
            int rowCaptionCount = Math.Max(columnCaptions.Grid.RowCaptions.Count - 1, 0);
            int rowPropertiesCount = GetPropertiesCount(columnCaptions.Grid, AxisType.Rows);

            Excel.Range columnRange = null;
            FieldSetCollection fieldsets = columnCaptions.Grid.PivotData.ColumnAxis.FieldSets;
            foreach (CaptionCell caption in columnCaptions)
            {
                Point endLocation = new Point(startLocation.X, startLocation.Y + rowCaptionCount + rowPropertiesCount);
                Excel.Range captionRange = ExcelUtils.MapExcelCell(sheet, caption.Text, startLocation, endLocation);

                //выставим такие же размеры как у заголовка в гриде
                //this.SetExcelCellSize(captionRange, caption.OriginalSize);
                
                columnRange = ExcelUtils.UnionRange(columnRange, captionRange);
                startLocation.X++;

                foreach (string property in GetPropertiesByCaptionCell(fieldsets, caption))
                {
                    endLocation = new Point(startLocation.X, startLocation.Y + rowCaptionCount + rowPropertiesCount);
                    captionRange = ExcelUtils.MapExcelCell(sheet, property, startLocation, endLocation);
                    columnRange = ExcelUtils.UnionRange(columnRange, captionRange);
                    startLocation.X++;
                    this.SetPropertyCaptionStyle(captionRange, columnCaptions.Style, isPrintVersion);

                }


            }
            this.SetCaptionCollectionStyle(columnRange, columnCaptions.Style, isPrintVersion);
        }

        private void MapColumnAxis(Excel.Worksheet sheet, Point startLocation, ColumnAxis columnAxis,
            bool isPrintVersion)
        {
            if (columnAxis.IsEmpty)
                return;

            //индекс столбца на котором сейчас идет размещение
            int mapColumnIndex = startLocation.Y;
            //максимальный индекс строки, занимаемый осью столбцов
            int maxRowIndex = startLocation.X + columnAxis.Grid.ColumnCaptions.Count - 1;

            //списки названий свойств для всех уровней
            List<List<string>> levelsProperties = new List<List<string>>();

            if (this.IsSeparateProperties)
            {
                FieldSetCollection fieldsets = columnAxis.Grid.PivotData.ColumnAxis.FieldSets;
                foreach (CaptionCell caption in columnAxis.Grid.ColumnCaptions)
                {
                    levelsProperties.Add(GetPropertiesByCaptionCell(fieldsets, caption));
                }

                foreach (CaptionCell caption in columnAxis.Grid.ColumnCaptions)
                {
                    maxRowIndex += GetPropertiesByCaptionCell(fieldsets, caption).Count;
                }
            }

            this.MapColumnCell(sheet, startLocation, columnAxis, columnAxis.Root, ref mapColumnIndex,
                maxRowIndex, isPrintVersion, levelsProperties);

            object startCell = sheet.Cells[startLocation.X, startLocation.Y];
            object endCell = sheet.Cells[maxRowIndex, mapColumnIndex];
            Excel.Range axisRange = sheet.get_Range(startCell, endCell);

            this.SetAxisStyle(axisRange);
        }

        private void MapColumnCell(Excel.Worksheet sheet, Point cellLocation, ColumnAxis columnAxis,
            DimensionCell dimCell, ref int mapColumnIndex, int maxRowIndex, bool isPrintVersion, List<List<string>> levelsProperties)
        {
            Point startLocation = new Point(dimCell.IsDummy ? cellLocation.X - 1 : cellLocation.X, cellLocation.Y);
            Excel.Range excelCell = null;
            if ((dimCell.HasChilds) && (dimCell.Expanded))
            {
                for (int i = 0; i < dimCell.Children.Count; i++)
                {
                    DimensionCell child = dimCell.Children[i];

                    int propCount = 0;

                    if (this.IsSeparateProperties)
                    {
                        for (int k = 0; k < child.DepthLevel; k++)
                        {
                            propCount += levelsProperties[k].Count;
                        }
                    }

                    Point childLocation = new Point(dimCell.IsRoot ? cellLocation.X : cellLocation.X + propCount + 1,
                                                    (i == 0) ? mapColumnIndex : ++mapColumnIndex);

                    this.MapColumnCell(sheet, childLocation, columnAxis, child, ref mapColumnIndex,
                                       maxRowIndex, isPrintVersion, levelsProperties);
                }

                //корень оси у нас невидим, так что его размещать не будем
                if (!dimCell.IsRoot)
                {
                    //вычилим конечную координату расоложения ячейки
                    Point endLocation = new Point(dimCell.IsDummy ? startLocation.X + 1 : startLocation.X,
                                                  cellLocation.Y + (mapColumnIndex - cellLocation.Y));
                    excelCell = ExcelUtils.MapExcelCell(sheet, dimCell.Text, startLocation, endLocation);


                    if ((this.IsSeparateProperties)&&(levelsProperties[dimCell.DepthLevel].Count > 0))
                    {
                        List<MemberProperty> props = GetNotNullableMemberProperties(dimCell.CellProperties_.MemberProperties);

                        foreach (string propName in levelsProperties[dimCell.DepthLevel])
                        {
                            MemberProperty mProp = GetPropertyByName(propName, props);

                            //вычислим конечную координату расположения ячейки
                            int propPosition = levelsProperties[dimCell.DepthLevel].IndexOf(propName);

                            Point propLocation = new Point(startLocation.X + propPosition + 1, startLocation.Y);
                            endLocation = new Point(propLocation.X, endLocation.Y);
                            string cellValue = ((mProp != null) && (mProp.Value != null)) ? CellProperties.GetMemberPropertyValue(mProp.Value) : String.Empty;
                            ExcelUtils.MapExcelCellWithoutFormatting(sheet, cellValue, propLocation, endLocation);
                        }

                    }

                }

            }
            else
            {
                if (!dimCell.IsRoot)
                {
                    //количество заголовков мер у листа колонок
                    int captionCount = (columnAxis.Grid.MeasureCaptionsSections.IsEmpty) ? 1 : (dimCell.LeafIndex < 0) ? columnAxis.Grid.MeasureCaptionsSections[0].Count :
                        columnAxis.Grid.MeasureCaptionsSections[dimCell.LeafIndex].Count;
                    mapColumnIndex += captionCount - 1;


                    int propCount = 0;
                    DimensionCell childCell = null;

                    if (this.IsSeparateProperties)
                    {
                        if (dimCell.Expanded)
                        {
                            propCount = levelsProperties[dimCell.DepthLevel].Count;
                        }
                        else
                        {
                            if (dimCell.CellProperties_.MemberProperties.Count > 0)
                            {
                                childCell = dimCell.GetLastDescendantInDimension();
                                propCount = levelsProperties[childCell.DepthLevel].Count;
                            }
                        }
                    }


                    Point endLocation = new Point(0, 0);
                    if (propCount == 0)
                    {
                        endLocation = new Point(startLocation.X + (maxRowIndex - startLocation.X),
                                                cellLocation.Y + captionCount - 1);
                    }
                    else
                    {
                        if (dimCell.Expanded)
                        {
                            endLocation = new Point(startLocation.X, cellLocation.Y + captionCount - 1);
                        }
                        else
                        {
                            if (childCell != null)
                            {
                                endLocation = new Point(startLocation.X + (childCell.DepthLevel - dimCell.DepthLevel), cellLocation.Y + captionCount - 1);
                            }
                        }

                        
                        if (dimCell.IsTotal)
                        {
                            //после строк со свойствами объединим ячейки итога
                            Point startTotal = new Point(Math.Min(startLocation.X + propCount + 1, maxRowIndex), cellLocation.Y);
                            Point endTotal = new Point(maxRowIndex, cellLocation.Y + captionCount - 1);

                            ExcelUtils.MapExcelCell(sheet, String.Empty, startTotal, endTotal);
                        }
                    }

                    excelCell = ExcelUtils.MapExcelCell(sheet, dimCell.Text, startLocation,
                        endLocation);



                    if ((this.IsSeparateProperties)&&(levelsProperties[dimCell.DepthLevel].Count > 0))
                    {
                        List<MemberProperty> props =
                            GetNotNullableMemberProperties(dimCell.CellProperties_.MemberProperties);

                        int propPosition = 0;
                        foreach (string propName in levelsProperties[dimCell.DepthLevel])
                        {
                            MemberProperty mProp = GetPropertyByName(propName, props);


                            if ((!dimCell.Expanded) && (childCell != null))
                            {
                                propPosition = levelsProperties[childCell.DepthLevel].IndexOf(propName);
                            }
                            else
                            {
                                propPosition = levelsProperties[dimCell.DepthLevel].IndexOf(propName);
                            }
                            //вычислим конечную координату расположения ячейки
                            Point propLocation = new Point(startLocation.X + propPosition + 1, startLocation.Y);
                            Point propEndLocation = new Point(startLocation.X + propPosition + 1, endLocation.Y);
                            string cellValue = (mProp != null) && (mProp.Value != null) ? CellProperties.GetMemberPropertyValue(mProp.Value) : String.Empty;
                            ExcelUtils.MapExcelCellWithoutFormatting(sheet, cellValue, propLocation, propEndLocation);
                        }
                    }

                 }
            }

            this.SetRangeStyle(excelCell, dimCell.Style, isPrintVersion);

            if ((columnAxis.IsAppearPropInDimCells)&&(!this.IsSeparateProperties))
                this.MapCellProperties(excelCell, dimCell, isPrintVersion);
        }

        private void MapRowCaptions(Excel.Worksheet sheet, Point startLocation, RowCaptions rowCaptions,
            bool isPrintVersion)
        {
            Excel.Range rowRange = null;

            FieldSetCollection fieldsets = rowCaptions.Grid.PivotData.RowAxis.FieldSets;
            foreach (CaptionCell caption in rowCaptions)
            {
                Excel.Range captionRange = ExcelUtils.MapExcelCell(sheet, caption.Text, startLocation);
                //выставим такие же размеры как у заголовка в гриде
                this.SetExcelCellSize(captionRange, caption.OriginalSize);
                rowRange = ExcelUtils.UnionRange(rowRange, captionRange);
                startLocation.Y++;
                this.SetCaptionCollectionStyle(captionRange, rowCaptions.Style, isPrintVersion);
                
                if (this.IsSeparateProperties)
                {
                    foreach (string property in GetPropertiesByCaptionCell(fieldsets, caption))
                    {
                        captionRange = ExcelUtils.MapExcelCell(sheet, property, startLocation);
                        rowRange = ExcelUtils.UnionRange(rowRange, captionRange);
                        startLocation.Y++;
                        this.SetPropertyCaptionStyle(captionRange, rowCaptions.Style, isPrintVersion);

                    }
                }
            }
            //this.SetCaptionCollectionStyle(rowRange, rowCaptions.Style, isPrintVersion);
        }

        private void MapRowAxis(Excel.Worksheet sheet, Point startLocation, RowAxis rowAxis,
            bool isPrintVersion)
        {
            if (rowAxis.IsEmpty)
                return;

            //индекс строки на котором сейчас идет размещение
            int mapRowIndex = startLocation.X;
            //максимальный индекс колонки, занимаемый осью строк
            int maxColumnIndex = startLocation.Y + rowAxis.Grid.RowCaptions.Count - 1;

            //списки названий свойств для всех уровней
            List<List<string>> levelsProperties = new List<List<string>>();

            if (this.IsSeparateProperties)
            {
                FieldSetCollection fieldsets = rowAxis.Grid.PivotData.RowAxis.FieldSets;
                foreach (CaptionCell caption in rowAxis.Grid.RowCaptions)
                {
                    levelsProperties.Add(GetPropertiesByCaptionCell(fieldsets, caption));
                }

                foreach (CaptionCell caption in rowAxis.Grid.RowCaptions)
                {
                    maxColumnIndex += GetPropertiesByCaptionCell(fieldsets, caption).Count;
                }
            }

            this.MapRowCell(sheet, startLocation, rowAxis, rowAxis.Root, ref mapRowIndex,
                maxColumnIndex, isPrintVersion, levelsProperties);

            object startCell = sheet.Cells[startLocation.X, startLocation.Y];
            object endCell = sheet.Cells[mapRowIndex, maxColumnIndex];
            Excel.Range axisRange = sheet.get_Range(startCell, endCell);

            this.SetAxisStyle(axisRange);
        }

        private List<MemberProperty> GetNotNullableMemberProperties(MemberPropertyCollection properties)
        {
            List<MemberProperty> result = new List<MemberProperty>();
            if (!this.IsSeparateProperties)
                return result;

            foreach(MemberProperty prop in properties)
            {
                if (prop.Value != null)
                    result.Add(prop);
            }
            return result;
        }


        private void MapRowCell(Excel.Worksheet sheet, Point cellLocation, RowAxis rowAxis,
            DimensionCell dimCell, ref int mapRowIndex, int maxColumnIndex, bool isPrintVersion, List<List<string>> levelsProperties)
        {
            Point startLocation = new Point(cellLocation.X, dimCell.IsDummy ? cellLocation.Y - 1 : cellLocation.Y);
            Excel.Range excelCell = null;
            if ((dimCell.HasChilds) && (dimCell.Expanded))
            {
                for (int i = 0; i < dimCell.Children.Count; i++)
                {
                    DimensionCell child = dimCell.Children[i];

                    int propCount = 0;
                    
                    if (this.IsSeparateProperties)
                    {
                        for (int k = 0; k < child.DepthLevel; k++)
                        {
                            propCount += levelsProperties[k].Count;
                        }
                    }

                    Point childLocation = new Point((i == 0) ? mapRowIndex : ++mapRowIndex,
                                                    dimCell.IsRoot ? cellLocation.Y : cellLocation.Y + propCount + 1);

                    this.MapRowCell(sheet, childLocation, rowAxis, child, ref mapRowIndex,
                                    maxColumnIndex, isPrintVersion, levelsProperties);
                }

                //корень оси у нас невидим, так что его размещать не будем
                if (!dimCell.IsRoot)
                {
                    //вычислим конечную координату расположения ячейки
                    Point endLocation = new Point(cellLocation.X + (mapRowIndex - cellLocation.X),
                                                  dimCell.IsDummy ? startLocation.Y + 1 : startLocation.Y);
                    excelCell = ExcelUtils.MapExcelCell(sheet, dimCell.Text, startLocation, endLocation);

                    if ((this.IsSeparateProperties)&&(levelsProperties[dimCell.DepthLevel].Count > 0))
                    {
                        List<MemberProperty> props = GetNotNullableMemberProperties(dimCell.CellProperties_.MemberProperties);

                        foreach (string propName in levelsProperties[dimCell.DepthLevel])
                        {
                            MemberProperty mProp = GetPropertyByName(propName, props);

                                //вычислим конечную координату расположения ячейки
                            int propPosition = levelsProperties[dimCell.DepthLevel].IndexOf(propName);

                            Point propLocation = new Point(startLocation.X, startLocation.Y + propPosition + 1);
                            endLocation = new Point(endLocation.X, propLocation.Y);
                            string cellValue = ((mProp != null) && (mProp.Value != null)) ? CellProperties.GetMemberPropertyValue(mProp.Value) : String.Empty;
                            ExcelUtils.MapExcelCellWithoutFormatting(sheet, cellValue, propLocation, endLocation);
                        }

                    }
                }
            }
            else
            {
                if (!dimCell.IsRoot)
                {
                    int propCount = 0;
                    DimensionCell childCell = null;

                    if (this.IsSeparateProperties)
                    {

                        if (dimCell.Expanded)
                        {
                            propCount = levelsProperties[dimCell.DepthLevel].Count;
                        }
                        else
                        {
                            if (dimCell.CellProperties_.MemberProperties.Count > 0)
                            {
                                childCell = dimCell.GetLastDescendantInDimension();
                                propCount = levelsProperties[childCell.DepthLevel].Count;
                            }
                        }
                    }

                    //если сразу после ячейки элемента измерения идут колонки со свойствами то ячейки итога объединять не будем
                    Point endLocation = new Point(0, 0);
                    if (propCount == 0)
                    {
                        endLocation = new Point(cellLocation.X, maxColumnIndex);
                    }
                    else
                    {
                        if (dimCell.Expanded)
                        {
                            endLocation = new Point(cellLocation.X, startLocation.Y);
                        }
                        else
                        {
                            if (childCell != null)
                            {
                                endLocation = new Point(cellLocation.X, startLocation.Y + (childCell.DepthLevel - dimCell.DepthLevel));
                            }
                        }

                        if (dimCell.IsTotal)
                        {
                            //после колонок со свойствами объединим ячейки итога
                            Point startTotal = new Point(cellLocation.X, Math.Min(startLocation.Y + propCount + 1, maxColumnIndex));
                            Point endTotal = new Point(cellLocation.X, maxColumnIndex);

                            ExcelUtils.MapExcelCell(sheet, String.Empty, startTotal, endTotal);
                        }
                    }

                    excelCell = ExcelUtils.MapExcelCell(sheet, dimCell.Text, startLocation, endLocation);

                    if (this.IsSeparateProperties)
                    {
                        List<MemberProperty> props =
                            GetNotNullableMemberProperties(dimCell.CellProperties_.MemberProperties);

                        int propPosition = 0;
                        foreach (MemberProperty mProp in props)
                        {
                            if ((!dimCell.Expanded) && (childCell != null))
                            {
                                propPosition = levelsProperties[childCell.DepthLevel].IndexOf(mProp.Name);
                            }
                            else
                            {
                                propPosition = levelsProperties[dimCell.DepthLevel].IndexOf(mProp.Name);
                            }
                            //вычислим конечную координату расположения ячейки
                            Point propLocation = new Point(startLocation.X, endLocation.Y + propPosition + 1);
                            string cellValue = mProp.Value != null ? CellProperties.GetMemberPropertyValue(mProp.Value) : String.Empty;
                            ExcelUtils.MapExcelCellWithoutFormatting(sheet, cellValue, propLocation, propLocation);
                        }
                    }

                }
            }

            this.SetRangeStyle(excelCell, dimCell.Style, isPrintVersion);

            if (!dimCell.IsDummy)
            {
                if (!dimCell.IsTotal && rowAxis.IsAppearPropInDimCells && !this.IsSeparateProperties)
                    this.MapCellProperties(excelCell, dimCell, isPrintVersion);

                //Как известно, метод AutoFit для подбора высоты объединенных ячеек не срабатывает. 
                //Высоту у таких ячеек мы будем подбирать самостоятельно
                ExcelUtils.AutoFitMergeCell(excelCell);

                string excelName = this.GetExcelName(dimCell);
                Excel.Name name = ExcelUtils.GetExcelName(sheet, excelName);
                if (name != null)
                {
                    string dupName = GridConsts.duplicate + "_" + excelName;
                    int k = 0;
                    while (ExcelUtils.GetExcelName(sheet, dupName) != null)
                    {
                        k++;
                        dupName = String.Format("{0}{1}_{2}", GridConsts.duplicate, k, excelName);
                    }
                    //пометим предыдущее имя как дубликат
                    name.Name = dupName;

                    ExcelUtils.MarkExcelName(excelCell, excelName, false);
                }
                else
                    if (this.IsMarkCellName(dimCell))
                        ExcelUtils.MarkExcelName(excelCell, excelName, false);
            }
        }

        private MemberProperty GetPropertyByName(string name, List<MemberProperty> props)
        {
            foreach(MemberProperty prop in props)
            {
                if (prop.Name == name)
                    return prop;
            }
            return null;
        }

        /// <summary>
        /// Получение имен memberproperties по заголовку оси.
        /// Если заголовок является заголовком последнего уровня в измерении, то для него выдается
        /// общий список memberproperties для всех уровней измерения, иначе - пустой список.
        /// </summary>
        /// <param name="fieldsets"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        private List<string> GetPropertiesByCaptionCell(FieldSetCollection fieldsets, CaptionCell caption)
        {
            List<string> properties = new List<string>();
            if (!this.IsSeparateProperties)
                return properties;

            PivotField field = fieldsets.GetFieldByName(caption.UniqueName);

            if (field != null)
            {
                if (field.IsLastFieldInSet)
                {
                    FieldSet fs = field.ParentFieldSet;
                    foreach (PivotField f in fs.Fields)
                    {
                        foreach (string property in f.MemberProperties.VisibleProperties)
                        {
                            if (!properties.Contains(property))
                            {
                                properties.Add(property);
                            }
                        }
                    }
                }
            }
            return properties;
        }

        private int GetPropertiesCount(ExpertGrid grid, AxisType axisType)
        {
            if (!this.IsSeparateProperties)
                return 0;

            FieldSetCollection fieldsets;
            CaptionsList captions;

            switch (axisType)
            {
                case AxisType.Rows:
                    fieldsets = grid.PivotData.RowAxis.FieldSets;
                    captions = grid.RowCaptions;
                    break;
                case AxisType.Columns:
                    fieldsets = grid.PivotData.ColumnAxis.FieldSets;
                    captions = grid.ColumnCaptions;
                    break;
                default:
                    return 0;

            }
            int result = 0;

            foreach(CaptionCell caption in captions)
            {
                result += GetPropertiesByCaptionCell(fieldsets, caption).Count;
            }
            return result;
        }

        /// <summary>
        /// Помечать экселевским именем будем только если мемебр лежит в последней ветви страницы и 
        /// на последней позиции родителя, если у родителя есть итог, то можно и на предпоследней
        /// </summary>
        /// <param name="dimCell"></param>
        /// <returns></returns>
        private bool IsMarkCellName(DimensionCell dimCell)
        {
            bool result = false;
            if (!dimCell.IsRoot)
            {
                result = true;
                DimensionCell child = dimCell;
                //родитель
                DimensionCell parent = child.Parent;

                if (!parent.IsRoot)
                {//Предварительная проверка, ищем пропродителя, и смотрим чтобы он лежал в последней ветви
                    while (!parent.IsRoot)
                    {
                        child = parent;
                        parent = parent.Parent;
                    }
                    if (!this.IsMarkCellName(child))
                        return false;
                }

                child = dimCell;
                parent = child.Parent;
                while ((parent != null) && result)
                {
                    int findRange = parent.IsExistTotal ? 2 : 1;

                    if (parent.Children.Count > findRange)
                    {
                        for (int i = parent.Children.Count - 1; i >= parent.Children.Count - findRange; i--)
                        {
                            result = (child == parent.Children[i]);
                            if (result)
                                break;
                        }
                    }
                    child = parent;
                    parent = parent.Parent;
                }
                
            }
            return result;
        }

        /// <summary>
        /// Т.к. таблица может распологаться на нескольких вкладках, будет появляться лишняя 
        /// информациия (итоги, элементы измерения) в этом методе все это причесываем
        /// </summary>
        private void DeleteDuplicate(Excel.Worksheet sheet)
        {
            //сначала удалим итоги - дубликаты
            this.DelCoincidentNames(sheet, GridConsts.duplicate + "_" + GridConsts.total);
            //удалим общий итог
            this.DelNameAndRange(sheet, GridConsts.duplicate + "_" + GridConsts.grandTotal);
            //объединим элементы строк распологавшиеся на разных страницах
            this.UnionRowMembers(sheet);
        }

        /// <summary>
        /// Т.к. таблица может распологаться на нескольких вкладках, будет появляться лишняя 
        /// информациия (итоги, элементы измерения) в этом методе все это причесываем
        /// Удаляем только имена (на листе могут располагаться неск. многостраничных таблиц)
        /// </summary>
        private void DeleteDuplicateNames(Excel.Worksheet sheet)
        {
            //сначала удалим итоги - дубликаты
            this.DelCoincidentNamesWithoutRange(sheet, GridConsts.duplicate + "_" + GridConsts.total);
            //удалим общий итог
            this.DelName(sheet, GridConsts.duplicate + "_" + GridConsts.grandTotal);
            //объединим элементы строк распологавшиеся на разных страницах
        }


        private void UnionRowMembers(Excel.Worksheet sheet)
        {
            if (sheet == null)
                return;

            foreach (Excel.Name duplicateName in sheet.Names)
            {
                if (duplicateName.Name.Contains(GridConsts.duplicate))
                {
                    //получим оригинал
                    //string originalStrName = duplicateName.Name.Replace(GridConsts.duplicate + "_", "");
                    string originalStrName = GetOriginalStrName(duplicateName.Name);
                    //если есть дубликат, то должен быти и оригинал
                    Excel.Name originalName = ExcelUtils.GetExcelName(sheet, originalStrName);
                    System.Windows.Forms.Application.DoEvents();

                    if (originalName != null)
                    {
                        //удалим схлопнутые элементы - дубликаты
                        if (duplicateName.Name.Contains(GridConsts.collapsedMember))
                        {
                            this.DelNameAndRange(sheet, duplicateName.Name);
                            continue;
                        }

                        Excel.Name greaterRangeName = this.GetGreaterRangeName(duplicateName, originalName);
                        if (greaterRangeName != null)
                        {
                            //это ошибочная ситуация, связанная с отображением на предыдушей 
                            //страницы не разложенного до конца мембера, сейчас просто удаляем его,
                            //так же такая ситуация возможна если на следующей странице в заголовке 
                            //расположен дефолтовый элемент с предыдущей страницы
                            this.DelNameAndRange(greaterRangeName);
                        }
                        else
                        {
                            Excel.Range duplicateRange = ExcelUtils.GetRefersToRange(duplicateName);
                            Excel.Range originalRange = ExcelUtils.GetRefersToRange(originalName);

                            if (originalRange != null)
                            {
                                if (duplicateRange != null)
                                    originalName.RefersToRange.Value2 = "";
                                Excel.Range unionRange = ExcelUtils.UnionRange(duplicateRange, originalRange);
                                unionRange.Merge(false);
                                ExcelUtils.MarkExcelName(unionRange, originalStrName, false);
                            }
                            originalName.Delete();
                            duplicateName.Delete();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// получение оригинального имени из дубликата
        /// </summary>
        /// <param name="dupName"></param>
        /// <returns></returns>
        private string GetOriginalStrName(string dupName)
        {
            //начало части подстроки, указывающей что это дубликат
            int dupPartStart = dupName.IndexOf(GridConsts.duplicate);
            if (dupPartStart < 0)
                return dupName;

            //подстрока - метка дубликата
            string dupPart = GridConsts.duplicate;
            for (int i = dupPartStart + GridConsts.duplicate.Length; i < dupName.Length; i++)
            {
                dupPart += dupName[i];
                if (dupName[i] == '_')
                    break;
            }
            return dupName.Replace(dupPart, "");
        }

        /// <summary>
        /// Если у диапазонов имен разная ширина, вернет тот у которорого она больше
        /// </summary>
        /// <param name="range1"></param>
        /// <param name="range2"></param>
        /// <returns></returns>
        private Excel.Name GetGreaterRangeName(Excel.Name duplicateName, Excel.Name originalName)
        {
            Excel.Name result = null;
            if ((duplicateName == null) || (originalName == null))
                return result;

            Excel.Range duplicate, original;
            try
            {
                duplicate = duplicateName.RefersToRange;
                original = originalName.RefersToRange;
            }
            catch
            {
                return null;
            }
            
            int duplicateWidth = duplicate.Columns.Count;
            int originalWidth = original.Columns.Count;

            try
            {
                duplicateWidth = Math.Max(duplicateWidth, duplicate.MergeArea.Columns.Count);
            }
            catch
            {
            }

            try
            {
                originalWidth = Math.Max(originalWidth, original.MergeArea.Columns.Count);
            }
            catch
            {
            }

            if (duplicateWidth != originalWidth)
                result = (duplicateWidth > originalWidth) ? duplicateName : originalName;

            return result ;
        }

        /// <summary>
        /// Удалить экселевское имя, а так же диапазон, ячейки таблицы
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="dimCell"></param>
        private void DelNameAndRange(Excel.Worksheet sheet, DimensionCell dimCell)
        {
            this.DelNameAndRange(sheet, this.GetExcelName(dimCell));
        }

        /// <summary>
        /// Удаляет экселевское имя, а так же диапазон на который оно ссылается
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="excelName"></param>
        private void DelNameAndRange(Excel.Worksheet sheet, string excelName)
        {
            this.DelNameAndRange(ExcelUtils.GetExcelName(sheet, excelName));
        }

        /// <summary>
        /// Удалить экселевское имя, а так же диапазон, ячейки таблицы
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="dimCell"></param>
        private void DelNameAndRange(Excel.Name name)
        {
            if (name != null)
            {
                name.RefersToRange.EntireRow.Delete(Type.Missing);
                name.Delete();
            }
        }

        /// <summary>
        /// Удалить экселевское имя
        /// </summary>
        /// <param name="name"></param>
        private void DelName(Excel.Name name)
        {
            if (name != null)
            {
                name.Delete();
            }
        }

        /// <summary>
        /// Удаляет экселевское имя
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="excelName"></param>
        private void DelName(Excel.Worksheet sheet, string excelName)
        {
            this.DelName(ExcelUtils.GetExcelName(sheet, excelName));
        }

        /// <summary>
        /// Удалит все имена которые содержат coincidentPart, а так же диапазоны на которые эти имена ссылаются
        /// </summary>
        /// <param name="sheet">лист в котором просмотриваем имена</param>
        /// <param name="coincidentPart">совпадающая часть</param>
        private void DelCoincidentNames(Excel.Worksheet sheet, string coincidentPart)
        {
            if ((sheet == null) || (coincidentPart == string.Empty))
                return;

            foreach (Excel.Name name in sheet.Names)
            {
                if (name.Name.Contains(coincidentPart))
                {
                    this.DelNameAndRange(name);
                }
            }
        }

        private void DelCoincidentNamesWithoutRange(Excel.Worksheet sheet, string coincidentPart)
        {
            if ((sheet == null) || (coincidentPart == string.Empty))
                return;

            foreach (Excel.Name name in sheet.Names)
            {
                if (name.Name.Contains(coincidentPart))
                {
                    this.DelName(name);
                }
            }
        }


        private string GetExcelName(DimensionCell dimCell)
        {
            string result = string.Empty;
            if (dimCell.ClsMember != null)
            {
                if (dimCell.IsGrandTotal)
                    result = GridConsts.grandTotal;
                else
                {
                    if (dimCell.IsTotal)
                    {
                        result = GridConsts.total + "_" + dimCell.GetAllHashCode();
                    }
                    else
                    {
                        if ((dimCell.HasChilds) && (!dimCell.Expanded))
                        {
                            result = GridConsts.collapsedMember + "_" + dimCell.GetAllHashCode();
                        }
                        else
                        {
                            result = GridConsts.member + "_" + dimCell.GetAllHashCode();
                        }
                    }
                }
            }

            return this.DecodeExcelName(result);
        }

        private string DecodeExcelName(string excelName)
        {
            return excelName.Replace("-", "minus");
        }

        private void MapCellProperties(Excel.Range excelCell, DimensionCell dimCell,
            bool isPrintVersion)
        {
            const string indent = "    ";

            if ((excelCell == null) || (dimCell.CellProperties_ == null)
                || (dimCell.CellProperties_.MemberProperties == null))
                return;

            if (dimCell.CellProperties_.MemberProperties.Count < 1)
                return;

            string cellText = string.Empty;
            if (excelCell.Value2 != null)
            {
                try
                {
                    if (excelCell.Value2.GetType() == typeof (string))
                    {
                        cellText = (string) excelCell.Value2;
                    }
                    else
                    {
                        //если ячейка объедененная в ней содержиться массив значений, нас интересует первое
                        object[,] values = (object[,]) excelCell.Value2;
                        cellText = (values[1, 1] != null ? values[1, 1].ToString() : string.Empty);
                    }
                }
                catch
                {
                    cellText = (string)excelCell.Text;
                }
            }

            int endText = cellText.Length;
            //Здесь будем хранить координаты свойств, после размещения их в ячейку, спомощью
            //данного хранилища присвоим свойствам стили
            List<CellPropertiesLocation> propLocations = new List<CellPropertiesLocation>();

            foreach (MemberProperty memProperty in dimCell.CellProperties_.MemberProperties)
            {
                if (memProperty.Value == null)
                    continue;

                CellPropertiesLocation location = new CellPropertiesLocation();
                location.NameStartCharacter = cellText.Length + 1;
                string propName = "\n" + indent + memProperty.Name + ":";
                cellText += propName;
                location.NameLenght = propName.Length;

                location.ValueStartCharacter = location.NameStartCharacter + location.NameLenght;
                string propValue = indent + CellProperties.GetMemberPropertyValue(memProperty.Value);
                cellText += propValue;
                location.ValueLenght = propValue.Length;

                propLocations.Add(location);
            }
            
            excelCell.Value2 = cellText;
            //Присваиваем свойствам стили
            foreach (CellPropertiesLocation location in propLocations)
            {
                this.SetPropertyStyle(excelCell, location, dimCell.Axis.MemProperNameStyle,
                    dimCell.Axis.MemProperValueStyle, isPrintVersion);
            }
        }

        private void SetPropertyStyle(Excel.Range excelCell, CellPropertiesLocation location,
            CellStyle nameStyle, CellStyle valueStyle, bool isPrintVersion)
        {
            Excel.Characters nameCharacters = excelCell.get_Characters(location.NameStartCharacter,
                location.NameLenght);

            Excel.Characters valueCharacters = excelCell.get_Characters(location.ValueStartCharacter,
                location.ValueLenght);

            ExcelUtils.SynchronizeFont(nameCharacters.Font, nameStyle.Font);
            ExcelUtils.SynchronizeFont(valueCharacters.Font, valueStyle.Font);

            if (isPrintVersion)
            {
                nameCharacters.Font.ColorIndex = 16;
            }
            else
            {
                nameCharacters.Font.Color = ColorTranslator.ToWin32(nameStyle.ForeColor);
                valueCharacters.Font.Color = ColorTranslator.ToWin32(valueStyle.ForeColor);
            }
        }

        private void MapMeasureCaptions(Excel.Worksheet sheet, Point startLocation,
            MeasureCaptionsSections measureCaptionsSections, bool isPrintVersion)
        {
            Excel.Range measureRange = null;
            bool firstCaption = true;
            foreach (MeasuresCaptionsSection section in measureCaptionsSections)
            {
                if (!section.IsVisible)
                {
                    continue;
                }

                foreach (CaptionCell caption in section)
                {
                    string captionText = caption.Text;
                    //Если формат отображения едениц измерения равен - отображать в заголовке, то к 
                    //имени прикручиваем еденицы измерения
                    if ((caption.MeasureValueFormat != null) &&
                        (caption.MeasureValueFormat.UnitDisplayType == UnitDisplayType.DisplayAtCaption))
                    {
                        string unit = measureCaptionsSections.Grid.PivotData.TotalAxis.GetFormatUnit(caption.MeasureValueFormat.FormatType);
                        captionText += (unit != string.Empty ? ", " + unit : string.Empty);
                    }

                    Excel.Range captionRange = ExcelUtils.MapExcelCell(sheet, captionText, startLocation);
                    //выставим такие же размеры как у заголовка в гриде
                    captionRange.ColumnWidth = caption.OriginalWidth / 7;
                    if (firstCaption)
                        //высоту будем выставлять один раз, ибо она для всех заголовков одинакова
                        captionRange.RowHeight = caption.OriginalHeight * 0.75f;
                    
                    measureRange = ExcelUtils.UnionRange(measureRange, captionRange);
                    startLocation.Y++;
                    firstCaption = false;
                }
            }
            this.SetCaptionCollectionStyle(measureRange, measureCaptionsSections.Style, isPrintVersion);
        }

        /// <summary>
        /// Найти родительскую схлопнутую ячейку со скрытым итогом
        /// </summary>
        /// <param name="mCell">дочерняя ячейка</param>
        /// <returns>Вернет схлопнутую ячейку со скрытым итогом, если у нее скрыт итог</returns>
        private DimensionCell GetCollapsedCellWithInvisibleTotal(MeasureCell mCell)
        {
            DimensionCell collapsedCell = null;

            if (!mCell.IsVisible)
            {
                DimensionCell rowCell = mCell.RowCell;
                while (rowCell.Parent != null)
                {
                    rowCell = rowCell.Parent;
                    if (!rowCell.Expanded)
                    {
                        collapsedCell = rowCell;
                    }
                }

                if (collapsedCell != null)
                {
                    return !collapsedCell.IsExistTotal ? collapsedCell : null;
                }

            }
            return collapsedCell;
        }

        /// <summary>
        /// Инкрементирует индекс строки, если нада
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="currCollapsedCell"></param>
        /// <param name="mCell"></param>
        /// <returns></returns>
        private int IncRow(int rowIndex, ref DimensionCell currCollapsedCell, MeasureCell mCell)
        {
            if (mCell.IsVisible)
                return rowIndex + 1;

            DimensionCell newCollapsedCell = GetCollapsedCellWithInvisibleTotal(mCell);
            if (currCollapsedCell != newCollapsedCell)
            {
                rowIndex++;
                currCollapsedCell = newCollapsedCell;
            }
            return rowIndex;
        }

        private void MapMeasureData(Excel.Worksheet sheet, Point startLocation,
            MeasuresData measureData, bool isPrintVersion)
        {
            if (measureData.IsEmpty)
                return;

            //Посчитаем количество видимых ячеек
            int vRowCount = 0;

            DimensionCell collapsedCell = null;
            foreach (MeasureCell mCell in measureData[0])
            {
                vRowCount = IncRow(vRowCount, ref collapsedCell, mCell);
                /*
                if (mCell.IsVisible)
                {
                    vRowCount++;
                }
                else 
                {
                    vRowCount = IncRowWithInvisibleTotal(vRowCount, ref collapsedCell, mCell);
                }*/
            }

            int vColumnsCount = 0;
            foreach(MeasureData mData in measureData)
            {
                if (mData.IsVisible)
                    vColumnsCount++;
            }

            int rowCount = measureData[0].Count;
            int columnsCount = measureData.Count;

            object[,] arrData = (object[,])Array.CreateInstance(typeof(object),
                new int[2] { vRowCount, vColumnsCount }, // длины массива
                new int[2] { 0, 0 });
            //Собираем все значения области данных в массив, что бы потом разместить его за один раз
            int vCol = 0;
            for (int c = 0; c < columnsCount; c++)
            {
                MeasureData section = measureData[c];
                if (section.IsVisible)
                {
                    int vRow = 0;
                    collapsedCell = null;
                    for (int r = 0; r < rowCount; r++)
                    {
                        if (section[r].IsVisible)
                            arrData[vRow, vCol] = section[r].GetValueForExcel();

                         vRow = IncRow(vRow, ref collapsedCell, section[r]);
                    }
                    vCol++;

                }
            }

            Excel.Range range = sheet.get_Range(sheet.Cells[startLocation.X, startLocation.Y],
              sheet.Cells[startLocation.X + vRowCount - 1, startLocation.Y + vColumnsCount - 1]);
            range.Value2 = arrData;// запись данных

            //Для каждого показателя создадим экселевский формат
            int visibleColumnCount = 0;
            for (int i = 0; i < measureData.Count; i++)
            {
                MeasureData section = measureData[i];
                if (!section.IsVisible)
                {
                    continue;
                }
                int sectionStartColumn = startLocation.Y + visibleColumnCount;
                Excel.Range sectionRange = ExcelUtils.GetExcelRange(sheet, startLocation.X, sectionStartColumn,
                    startLocation.X + vRowCount - 1, sectionStartColumn);
                this.SetSectionStyle(sectionRange, section, isPrintVersion);
                visibleColumnCount++;
            }

            range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
        }


        private string GetFormatMask(ValueFormat format)
        {
            string result = string.Empty;

            if (format.ThousandDelimiter && format.IsThousandDelimiterEnable)
                result += "# ##";

            if (format.IsDigitCountEnable)
            {
                result += "0";
                for (int i = 0; i < format.DigitCount; i++)
                {
                    result += (i == 0) ? ",0" : "0";
                }
            }

            if (format.UnitDisplayType == UnitDisplayType.DisplayAtValue)
            {
                switch (format.FormatType)
                {
                    case FormatType.Currency: result += "р."; break;
                    case FormatType.MilliardsCurrency:
                    case FormatType.MilliardsCurrencyWitoutDivision:
                        {
                            result += "\"млрд.\"р.";
                            break;
                        }
                    case FormatType.MilliardsNumeric: result += "\"млрд.\""; break;
                    case FormatType.MillionsCurrency:
                    case FormatType.MillionsCurrencyWitoutDivision:
                        {
                            result += "\"млн.\"р.";
                            break;
                        }
                    case FormatType.MillionsNumeric: result += "\"млн.\""; break;
                    case FormatType.Percent: result += "\"%\""; break;
                    case FormatType.ThousandsCurrency:
                    case FormatType.ThousandsCurrencyWitoutDivision:
                        {
                            result += "\"тыс.\"р."; 
                            break;
                        }
                    case FormatType.ThousandsNumeric: result += "\"тыс.\""; break;
                    case FormatType.Exponential: result += "E+000"; break;
                }
            }
            return result;
        }

        private void SetSectionStyle(Excel.Range sectionRange, MeasureData section, bool isPrintVersion)
        {
            if ((sectionRange == null) || (section == null))
                return;

            CellStyle style = section.MeasuresData.Style;
            CellStyle styleForTotals = section.MeasuresData.StyleForTotals;

            ValueFormat sectionFormat = section.MeasureCaption.MeasureValueFormat;

            //Если формат ячейки влияет хоть на один из следующих признаков, то будем получать маску для него
            if (sectionFormat.IsDigitCountEnable || sectionFormat.IsThousandDelimiterEnable
                || sectionFormat.IsUnitDisplayTypeEnable)
                sectionRange.NumberFormatLocal = this.GetFormatMask(sectionFormat);

            //Выставляем выравнивание
            Excel.XlHAlign excelAlign = Excel.XlHAlign.xlHAlignRight;
            if (sectionFormat.ValueAlignment == StringAlignment.Near)
                excelAlign = Excel.XlHAlign.xlHAlignLeft;
            else
                if (sectionFormat.ValueAlignment == StringAlignment.Center)
                    excelAlign = Excel.XlHAlign.xlHAlignCenter;

            //Выставляем стиль для секции
            this.SetRangeStyle(sectionRange, style, isPrintVersion);
            //Выставляем стиль для итогов в этой секции
            int vRow = 0;
            DimensionCell collapsedCell = null;
            for (int i = 0; i < section.Count; i++)
            {
                if (section[i].IsVisible)
                {
                    if (section[i].IsTotal)
                    {
                        this.SetRangeStyle(ExcelUtils.GetExcelRange(sectionRange.Worksheet, sectionRange.Row + vRow,
                                                                    sectionRange.Column), styleForTotals, isPrintVersion);
                    }
                    else
                    {
                        CellStyle cStyle = section[i].GetCellStyle(false);
                        this.SetRangeStyle(ExcelUtils.GetExcelRange(sectionRange.Worksheet, sectionRange.Row + vRow,
                                                                    sectionRange.Column), cStyle, isPrintVersion);
                        
                    }
                }
                vRow = IncRow(vRow, ref collapsedCell, section[i]);

            }

            sectionRange.HorizontalAlignment = excelAlign;
        }

        private void SetRangeStyle(Excel.Range range, CellStyle style, bool isPrintVersion)
        {
            if ((range == null) || (style == null))
                return;

            if (!isPrintVersion)
            {
                range.Interior.Color = ColorTranslator.ToWin32(style.BackColorStart);
                range.Font.Color = ColorTranslator.ToWin32(style.ForeColor);
            }
            ExcelUtils.SynchronizeFont(range.Font, style.OriginalFont);
        }

        private void SetExcelCellSize(Excel.Range excelCell, Size gridCellSize)
        {
            if (excelCell != null)
            {
                excelCell.ColumnWidth = gridCellSize.Width / 7;
                excelCell.RowHeight = gridCellSize.Height * 0.75f;
            }
        }

        private void SetCaptionCollectionStyle(Excel.Range captionRange, CellStyle style, bool isPrintVersion)
        {
            if (captionRange != null)
            {
                captionRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                captionRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignJustify;
                captionRange.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                
                this.SetRangeStyle(captionRange, style, isPrintVersion);
                
                if (isPrintVersion)
                    captionRange.Font.Bold = true;
            }
        }

        private void SetPropertyCaptionStyle(Excel.Range captionRange, CellStyle style, bool isPrintVersion)
        {
            SetCaptionCollectionStyle(captionRange, style, isPrintVersion);
            captionRange.Font.Color = ColorTranslator.ToWin32(Color.Gray);
        }

        private void SetAxisStyle(Excel.Range axisRange)
        {
            if (axisRange != null)
            {
                axisRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                axisRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                axisRange.WrapText = true;
                //axisRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignJustify;
                axisRange.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
            }
        }

        private void InsertRows(Excel.Worksheet sheet, Point startLocation, ExpertGrid exportedGrid)
        {
            int rowCount = 0;
            //заголовок таблицы
            rowCount++;
            //фильтры таблицы
            if (!exportedGrid.FilterCaptions.IsEmpty)
                rowCount++;
            //заголовки столбцов
            rowCount += exportedGrid.ColumnCaptions.Count;
            //заголовки сток и заголовки мер
            if (!exportedGrid.RowCaptions.IsEmpty || !exportedGrid.MeasureCaptionsSections.IsEmpty)
                rowCount++;
            //строки и меры
            if (!exportedGrid.MeasuresData.IsEmpty)
                rowCount += exportedGrid.MeasuresData[0].Count;
            else
                if (!exportedGrid.Row.IsEmpty)
                    rowCount += exportedGrid.Row.GetLeafCount();
            Excel.Range row = (Excel.Range)sheet.Cells[startLocation.X, 1];
            row = row.EntireRow;
            row.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Type.Missing);
        }

        /// <summary>
        /// Вычисление начальных координат ячеек для размещения коллекций грида
        /// </summary>
        /// <param name="gridLocation"></param>
        /// <param name="grid"></param>
        /// <param name="isExistExpertGrid">существует ли уже на листе экспортированный грид</param>
        private void DetermineCollectionLocation(Point gridLocation, ExpertGrid grid, bool isExistExpertGrid)
        {
            int rowPropertiesCount = GetPropertiesCount(grid, AxisType.Rows);
            int columnPropertiesCount = GetPropertiesCount(grid, AxisType.Columns);

            if (!isExistExpertGrid)
            {
                this.filterCaptionsLocation = gridLocation;

                this.columnCaptionsLocation.X = (!grid.FilterCaptions.IsEmpty && grid.FilterCaptions.Visible) ? 
                    this.filterCaptionsLocation.X + 2 : this.filterCaptionsLocation.X;
                this.columnCaptionsLocation.Y = this.filterCaptionsLocation.Y;

                this.columnLocation.X = this.columnCaptionsLocation.X;
                if (grid.ColumnCaptions.IsEmpty)
                    this.columnLocation.Y = this.columnCaptionsLocation.Y;
                else
                {
                    this.columnLocation.Y = grid.RowCaptions.IsEmpty ? this.columnCaptionsLocation.Y + 1 :
                        this.columnCaptionsLocation.Y + grid.RowCaptions.Count + rowPropertiesCount;
                }

                this.rowCaptionsLocation.X = (!grid.ColumnCaptions.IsEmpty) ?
                    this.columnCaptionsLocation.X + grid.ColumnCaptions.Count + columnPropertiesCount : this.columnCaptionsLocation.X;
                this.rowCaptionsLocation.Y = this.columnCaptionsLocation.Y;

                this.rowLocation.X = this.rowCaptionsLocation.X + 1;
                this.rowLocation.Y = this.rowCaptionsLocation.Y;

                this.measureCaptionsLocation.X = this.rowCaptionsLocation.X;
                this.measureCaptionsLocation.Y = this.rowCaptionsLocation.Y + grid.RowCaptions.Count 
                    + (grid.MeasuresStub.IsVisible ? 1 : 0);

                this.measureCaptionsLocation.Y += rowPropertiesCount;

                this.measureLocation.X = this.measureCaptionsLocation.X + 1;
                this.measureLocation.Y = this.measureCaptionsLocation.Y;
            }
            else
            {
                this.rowLocation = gridLocation;

                this.measureLocation.X = gridLocation.X;
                this.measureLocation.Y = this.rowLocation.Y + grid.RowCaptions.Count;

                this.measureLocation.Y += rowPropertiesCount;
            }
        }

        /// <summary>
        /// Получить начальные координат размещения грида в листе
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private Point GetGridStartLocation(Excel.Worksheet sheet)
        {
            Point result = new Point(1, 1);
            Excel.Name gridName = ExcelUtils.GetExcelName(sheet, expertGridName);
            
            //Если на листе уже есть экспортированная таблица, значит размещаем под ней
            if (gridName != null)
            {
                result.X = gridName.RefersToRange.Row + gridName.RefersToRange.Rows.Count;
                result.Y = gridName.RefersToRange.Column;
            }
            return result;
        }

        /// <summary>
        /// Существует ли в листе экспортированная таблица
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private bool IsExistExpertGrid(Excel.Worksheet sheet)
        {
            return (ExcelUtils.GetExcelName(sheet, expertGridName) != null);
        }

        /// <summary>
        /// Экспортируемый грид
        /// </summary>
        private ExpertGrid ExportedGrid
        {
            get { return _exportedGrid; }
            set { _exportedGrid = value; }
        }

        /// <summary>
        /// Структура хранящая координаты свойства ячейки
        /// </summary>
        struct CellPropertiesLocation
        {
            private int _nameStartCharacter;
            private int _valueStartCharacter;
            private int _nameLenght;
            private int _valueLenght;

            /// <summary>
            /// Инекс начального символа имени свойства
            /// </summary>
            public int NameStartCharacter
            {
                get { return _nameStartCharacter; }
                set { _nameStartCharacter = value; }
            }

            /// <summary>
            /// Индекс начального символа значения свойства
            /// </summary>
            public int ValueStartCharacter
            {
                get { return _valueStartCharacter; }
                set { _valueStartCharacter = value; }
            }

            /// <summary>
            /// Длина имени свойства
            /// </summary>
            public int NameLenght
            {
                get { return _nameLenght; }
                set { _nameLenght = value; }
            }

            /// <summary>
            /// Длина значения свойства
            /// </summary>
            public int ValueLenght
            {
                get { return _valueLenght; }
                set { _valueLenght = value; }
            }
        }
    }
}
