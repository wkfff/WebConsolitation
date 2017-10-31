using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.CommonClass;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Класс для работы с выделенными ячейками 
    /// </summary>
    public class SelectedCells
    {
        private ExpertGrid _grid;
        private List<GridControl> _currentCells;
        private List<int> _hashCodes;
        private List<GridControl> _prepareCells;
        //выделенная ячейка
        private GridControl _currentCell;
        //предыдущая выделенная ячейка
        private GridControl _prepareCell;
        private Cursor cursor;
        //заголовок, над которым проводили курсором...
        private CaptionCell prepareMoveCaption;
        //координаты предыдущей ячейки выделенной с помощью клавиатуры
        private Point prepareKeyLocation = new Point(-1, -1);
        //количество нажатий на ячейку
        private byte numberClick = 0;

        private Point _currentCellLocation;

        public SelectedCells(ExpertGrid grid)
        {
            this.Grid = grid;
            this._hashCodes = new List<int>();
            this._currentCells = new List<GridControl>();
            this._prepareCells = new List<GridControl>();
        }

        public void Clear()
        {
            //this._currentCellLocation = this.GetCurrentCellLocation();
            this.CurrentCell = null;
            this.PrepareCell = null;
            if (this.CurrentCells != null)
                this.CurrentCells.Clear();
            if (PrepareCells != null)
                this.PrepareCells.Clear();
        }

        //отрисовка кнопки нужна в случае если выделена dropButton но она не покрашена или
        //если не выделена dropButton и она покрашена
        private bool IsPaintCaption(CaptionCell caption)
        {
            CaptionCell.DropButtonType dropButton = caption.DropButton;
            if (dropButton == null)
                return (caption.State == ControlState.Normal);
            else
            {
                bool isContain = dropButton.Contain(this.Grid.CurrentMousePosition);
                return (caption.State == ControlState.Normal) || (isContain && (dropButton.State == ControlState.Normal)) ||
                    (!isContain && (dropButton.State == ControlState.Hot));
            }
        }

        /// <summary>
        /// Ищет возможность изменить размеры ячеек, если это возможно меняет курсор, на соответсвующий сплитер
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Cursor MouseMove(MouseEventArgs e)
        {
            this.cursor = Cursors.Default;
            CaptionCell caption = this.Grid.LocationHelper.SelectedCaption;

            bool isEqualsCaption = false;
            if ((caption != null) && (caption.State != ControlState.Selected))
            {
                isEqualsCaption = caption.Equals(this.prepareMoveCaption);
                //если перешли на другой заголовок или если нужна перерисовка кнопки, перерисовываем
                if ((!isEqualsCaption) || IsPaintCaption(caption))
                {
                    caption.State = ControlState.Hot;
                    Graphics graphics = this.Grid.GetGridGraphics();
                    graphics.Clip = new Region(caption.GetVisibleBounds());
                    caption.OnPaint(graphics, this.Grid.Painter);
                }
            }

            if ((!isEqualsCaption) && (this.prepareMoveCaption != null) 
                && (this.prepareMoveCaption.Captions != null)
                && (this.prepareMoveCaption.State != ControlState.Selected))
            {
                Graphics graphics = this.Grid.GetGridGraphics();
                graphics.Clip = new Region(prepareMoveCaption.GetVisibleBounds());
                this.prepareMoveCaption.State = ControlState.Normal;
                this.prepareMoveCaption.OnPaint(graphics, this.Grid.Painter);
            }

            GridCell resizeCell = this.Grid.LocationHelper.CellForResize;
            if (resizeCell != null)
            {
                Rectangle captionBounds = resizeCell.OffsetBounds;
                if ((captionBounds.Right < (e.Location.X + GridConsts.boundsDeflection)) 
                    && (captionBounds.Right > e.Location.X))
                {
                    this.cursor = Cursors.VSplit;
                }
                else
                {
                    if (captionBounds.Bottom < (e.Location.Y + GridConsts.boundsDeflection) 
                        && (captionBounds.Bottom > e.Location.Y))
                        this.cursor = Cursors.HSplit;
                }
            }

            this.prepareMoveCaption = caption;
            return this.cursor;
        }

        /// <summary>
        /// Нажатие на выделенную кнопку
        /// </summary>
        /// <param name="location"></param>
        private void ClickCell(Point location)
        {
            if (this.CurrentCell != null)
            {
                if (this.CurrentCell.GetHitTest(location, true))
                {
                    //если кнопку мышки отпустили на нажатой ячейке, выполняем клик
                    this.CurrentCell.OnClick(location);
                }
            }
        }

        /// <summary>
        /// Подсвечивает выбранную ячейку
        /// </summary>
        /// <param name="location"></param>
        private void SelectCell(Point location)
        {
            GridControl selectedCell = this.Grid.LocationHelper.SelectedControl;

            Rectangle visibleBounds = Rectangle.Empty;

            if (selectedCell != null)
            {
                /*
                if (selectedCell.Equals(this.CurrentCell))
                {
                    this.CurrentCell = selectedCell;
                    this.PrepareCell = this.CurrentCell;
                    visibleBounds = this.CurrentCell.GetVisibleBounds();
                    this.Grid.GridGraphics.Clip = new Region(visibleBounds);

                    this.HashCodes.Clear();
                    this.HashCodes.Add(this.CurrentCell.GetHashCode());
                    this.CurrentCell.OnPaint(this.Grid.GridGraphics, this.Grid.Painter);

                    return;
                }*/

                this.Grid.GridGraphics = this.Grid.GetGridGraphics();

                ClearSelection();
                /*
                this.PrepareCell = this.CurrentCell;

                if (this.PrepareCell != null)
                {
                    this.PrepareCell.State = ControlState.Normal;
                    visibleBounds = this.PrepareCell.GetVisibleBounds();
                    this.Grid.GridGraphics.Clip = new Region(visibleBounds);
                    this.PrepareCell.OnPaint(this.Grid.GridGraphics, this.Grid.Painter);
                }*/

                this.CurrentCell = selectedCell;
                this.CurrentCell.State = ControlState.Selected;
                visibleBounds = this.CurrentCell.GetVisibleBounds();
                this.Grid.GridGraphics.Clip = new Region(visibleBounds);
                this.HashCodes.Clear();
                this.HashCodes.Add(this.CurrentCell.GetHashCode());
                this.CurrentCell.OnPaint(this.Grid.GridGraphics, this.Grid.Painter);

                this.Grid.GridGraphics = null;

            }
        }

        /// <summary>
        /// Выделяем ячейки в указанной области
        /// </summary>
        /// <param name="searchBounds"></param>
        public void SelectRegion(Rectangle searchBounds)
        {
            this.Grid.LocationHelper.Initialize(searchBounds, true);
            this.SelectCells(searchBounds);
        }

        /// <summary>
        /// Подсвечивает выбранные ячейки
        /// </summary>
        /// <param name="location"></param>
        private void SelectCells(Rectangle searchBounds)
        {
            List<GridControl> selectedCells = this.Grid.LocationHelper.SelectedControls;

            if (selectedCells != null)
            {
                if (selectedCells.Equals(this.CurrentCells))
                {
                    this.CurrentCells = selectedCells;
                    this.PrepareCells = this.CurrentCells;
                    this.HashCodes.Clear();
                    foreach (GridControl cell in this.CurrentCells)
                        this.HashCodes.Add(cell.GetHashCode());
                    return;
                }

                ClearSelection();
                Rectangle visibleBounds = Rectangle.Empty;

                this.CurrentCells.AddRange(selectedCells);
                if (this.CurrentCells != null)
                {
                    foreach (GridControl cell in this.CurrentCells)
                    {
                        cell.State = ControlState.Selected;
                        visibleBounds = cell.GetVisibleBounds();
                        this.Grid.GridGraphics.Clip = new Region(visibleBounds);
                        cell.OnPaint(this.Grid.GridGraphics, this.Grid.Painter);
                        this.HashCodes.Add(cell.GetHashCode());
                    }
                }
                this.Grid.GridGraphics = null;

            }
        }

        /// <summary>
        /// Очищает текущие выделенные ячейки
        /// </summary>
        public void ClearSelection()
        {
            this.Grid.GridGraphics = this.Grid.GetGridGraphics();
            Rectangle visibleBounds = Rectangle.Empty;

            if (this.PrepareCells == null)
                this.PrepareCells = new List<GridControl>();

            if (this.CurrentCells == null)
                this.CurrentCells = new List<GridControl>();

            this.PrepareCells.Clear();
            this.PrepareCells.AddRange(this.CurrentCells);

            foreach (GridControl cell in this.PrepareCells)
            {

                cell.State = ControlState.Normal;
                visibleBounds = cell.GetVisibleBounds();
                this.Grid.GridGraphics.Clip = new Region(visibleBounds);
                cell.OnPaint(this.Grid.GridGraphics, this.Grid.Painter);
            }

            this.PrepareCell = this.CurrentCell;

            if (this.PrepareCell != null)
            {
                this.PrepareCell.State = ControlState.Normal;
                visibleBounds = this.PrepareCell.GetVisibleBounds();
                this.Grid.GridGraphics.Clip = new Region(visibleBounds);
                this.PrepareCell.OnPaint(this.Grid.GridGraphics, this.Grid.Painter);
            }

            this.CurrentCells.Clear();
            this.HashCodes.Clear();
        }


        /// <summary>
        /// Сфокусировать на элемент, в PropertyGrid
        /// </summary>
        public void ChangeElementInPropertyGrid()
        {
            this.ChangeElementInPropertyGrid(false);
        }

        /// <summary>
        /// Сфокусировать на элемент, в PropertyGrid
        /// </summary>
        /// <param name="IsEscapePress">признак что переключение происходит по нажатию на Escapr</param>
        public void ChangeElementInPropertyGrid(bool isEscapePress)
        {
            this.IncreaseNumberClick(isEscapePress);
            if (this.numberClick == 0)
                return;

            switch (this.CurrentCell.GridObject)
            {
                case GridObject.CaptionCell:
                    {
                        this.ChangeElementByCaption((CaptionCell)this.CurrentCell);
                        break;
                    }
                case GridObject.DimensionCell:
                    {
                        DimensionCell dimensioCell = (DimensionCell)this.CurrentCell;
                        CaptionCell caption = (dimensioCell.IsBelongToRowAxis) ?
                            this.Grid.RowCaptions[dimensioCell.DepthLevel] :
                            this.Grid.ColumnCaptions[dimensioCell.DepthLevel];
                        this.ChangeElementByCaption(caption);
                        break;
                    }
                case GridObject.MeasureCell:
                    {
                        this.ChangeElementByCaption(((MeasureCell)this.CurrentCell).MeasureData.MeasureCaption);
                        break;
                    }
            }
        }

        /// <summary>
        /// Все необходимую информацию для выбора элемента в PropertyGrid, берет у заголовка
        /// </summary>
        /// <param name="caption"></param>
        private void ChangeElementByCaption(CaptionCell caption)
        {
            if (caption == null)
                return;

            switch (caption.Captions.Type)
            {
                case CaptionType.Measures:
                    {
                        switch (this.numberClick)
                        {
                            case 1:
                                {
                                    this.Grid.OnObjectSelected(SelectionType.SingleObject,
                                        caption.UniqueName);
                                    break;
                                }
                            case 2:
                                {
                                    this.Grid.OnObjectSelected(SelectionType.Totals, string.Empty);
                                    break;
                                }
                            case 3:
                                {
                                    this.Grid.OnObjectSelected(SelectionType.GeneralArea, string.Empty);
                                    break;
                                }
                        }
                        break;
                    }
                case CaptionType.Columns:
                case CaptionType.Rows:
                case CaptionType.Filters:
                    {
                        switch (this.numberClick)
                        {
                            case 1:
                                {
                                    this.Grid.OnObjectSelected(SelectionType.SingleObject,
                                        caption.UniqueName);
                                    break;
                                }
                            case 2:
                                {
                                    this.Grid.OnObjectSelected(SelectionType.SingleObject,
                                        caption.HierarchyUN);
                                    break;
                                }
                            case 3:
                                {
                                    SelectionType selType = SelectionType.SingleObject;

                                    switch (caption.Captions.Type)
                                    {
                                        case CaptionType.Filters: selType = SelectionType.Filters; break;
                                        case CaptionType.Columns: selType = SelectionType.Columns; break;
                                        case CaptionType.Rows: selType = SelectionType.Rows; break;
                                    }
                                    this.Grid.OnObjectSelected(selType, string.Empty);
                                    break;
                                }
                            case 4:
                                {
                                    this.Grid.OnObjectSelected(SelectionType.GeneralArea, string.Empty);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Считаем количество кликов на ячейке. Если предыдущая и текущая ячейки совпадают, увеличивает 
        /// количество кликов на ней. Не дает переполниться циклу нажатий.
        /// </summary>
        /// <param name="IsEscapePress">признак что переключение происходит по нажатию на Escapr</param>
        private void IncreaseNumberClick(bool isEscapePress)
        {
            if (this.CurrentCell == null)
            {
                this.numberClick = 0;
                return;
            }

            if (this.CurrentCell == this.PrepareCell)
            {
                switch (this.CurrentCell.GridObject)
                {
                    case GridObject.DimensionCell:
                        {
                            //у измерений, количество выбераемых элементов равно 4 (уровень, измерение,
                            //коллекция измерений, таблица) если больше, возвращаемся на начало цикла
                            if (this.numberClick >= 4)
                            {
                                if (!isEscapePress)
                                    this.numberClick = 1;
                            }
                            else
                                this.numberClick++;
                            break;
                        }
                    case GridObject.MeasureCell:
                        {
                            //у мер, количество выбераемых элементов равно 3 (мера, коллекция мер,
                            //таблица) если больше, возвращаемся на начало цикла
                            if (this.numberClick >= 3)
                            {
                                if (!isEscapePress)
                                    this.numberClick = 1;
                            }
                            else
                                this.numberClick++;
                            break;
                        }
                    case GridObject.CaptionCell:
                        {
                            CaptionsList captions = ((CaptionCell)this.CurrentCell).Captions;
                            //Максимально возможное количество выбераемых элементов
                            int maxNumberClick = (captions.Type == CaptionType.Measures) ? 3 : 4;

                            if (this.numberClick >= maxNumberClick)
                            {
                                if (!isEscapePress)
                                    this.numberClick = 1;
                            }
                            else
                                this.numberClick++;
                            break;
                        }
                    default:
                        {
                            this.numberClick = 0;
                            break;
                        }
                }
            }
            else
                this.numberClick = 1;
        }

        /// <summary>
        /// Событие на нажатие кнопки мыши
        /// </summary>
        /// <param name="e"></param>
        public void MouseDown(MouseEventArgs e)
        {
            //this.Grid.LocationHelper.Initialize(e.Location, false);
            this.SelectCell(e.Location);
            //если есть выделеный контрол, и нажатие произошло левой кнопкой мыши и на него самого, 
            //а не дочерний элемент то выберем элемент в PropertyGrid()
            if ((e.Button == MouseButtons.Left) && (this.CurrentCell != null) && 
                !this.CurrentCell.IsClickChildElement(e.Location))
                this.ChangeElementInPropertyGrid();
            this.prepareKeyLocation = new Point(-1, -1);
        }

        /// <summary>
        /// Событие на отпускание кнопки мыши
        /// </summary>
        /// <param name="e"></param>
        public void MouseClick(MouseEventArgs e)
        {
            this.ClickCell(e.Location);
        }

        /// <summary>
        /// Получить координаты следующей ячейки
        /// </summary>
        private Point GetNextCellLocation(GridControl cell, Keys key)
        {
            Point result = Point.Empty;
            if (cell == null)
                return result;
            Rectangle cellBounds = cell.GetBounds();

            switch (key)
            {
                case Keys.Up:
                    {
                        result = new Point(cellBounds.X + 1, cellBounds.Top - 10);
                        if (prepareKeyLocation.X != -1)
                            result.X = prepareKeyLocation.X;
                        else
                            prepareKeyLocation.X = result.X;
                        prepareKeyLocation.Y = -1;
                        break;
                    }
                case Keys.Down:
                    {
                        result = new Point(cellBounds.X + 1, cellBounds.Bottom + 10);
                        if (prepareKeyLocation.X != -1)
                            result.X = prepareKeyLocation.X;
                        else
                            prepareKeyLocation.X = result.X;
                        prepareKeyLocation.Y = -1;
                        break;
                    }
                case Keys.Left:
                    {
                        result = new Point(cellBounds.Left - 40, cellBounds.Y + 1);
                        if (prepareKeyLocation.Y != -1)
                            result.Y = prepareKeyLocation.Y;
                        else
                            prepareKeyLocation.Y = result.Y;
                        prepareKeyLocation.X = -1;
                        break;
                    }
                case Keys.Right:
                    {
                        result = new Point(cellBounds.Right + 40, cellBounds.Y + 1);
                        if (prepareKeyLocation.Y != -1)
                            result.Y = prepareKeyLocation.Y;
                        else
                            prepareKeyLocation.Y = result.Y;
                        prepareKeyLocation.X = -1;
                        break;
                    }
            }
            return result;
        }

        /// <summary>
        /// Смотрим, что нанажимал пользователь
        /// </summary>
        /// <param name="e"></param>
        public void PreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (this.CurrentCell == null)
                return;

            if ((e.KeyData == (Keys.Control | Keys.C)) || (e.KeyData == (Keys.Control | Keys.Insert)))
            {
                //копируем в буфер обмена
                string text = GetSelectedText();
                if (!String.IsNullOrEmpty(text))
                    Clipboard.SetText(text);
            }

            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    {
                        if (this.Grid.IsAllowMoveByGrid)
                        {
                            try
                            {
                                this.Grid.IsAllowMoveByGrid = false;
                                GridControl prepareCell = this.CurrentCell;
                                Point nextCellLocation = this.GetNextCellLocation(this.CurrentCell, e.KeyCode);
                                this.Grid.LocationHelper.Initialize(nextCellLocation, false);
                                this.SelectCell(nextCellLocation);
                                this.MoveGridFocus(e.KeyCode, prepareCell, this.Grid.LocationHelper.SelectedControl);
                            }
                            finally
                            {
                                this.Grid.IsAllowMoveByGrid = true;
                                this.ChangeElementInPropertyGrid();
                            }
                        }
                        break;
                    }
                case Keys.Enter:
                    {
                        this.ClickCell(this.CurrentCell.GetOffsetBounds().Location);
                        break;
                    }
                case Keys.Escape:
                    {
                        this.PrepareCell = this.CurrentCell;
                        this.ChangeElementInPropertyGrid(true);
                        break;
                    }
            }
        }

        /// <summary>
        /// Получение текста из выделенных ячеек в формате таблицы
        /// </summary>
        /// <returns></returns>
        public string GetSelectedText()
        {
            string result = String.Empty;
            if ((this.CurrentCells.Count == 0) && (this.CurrentCell != null))
                return GetTextFromCell(this.CurrentCell, -1, -1, String.Empty);

            //все отступы ячеек по столбцам
            List<int> colsOffsets = new List<int>();
            //все отступы ячеек по строкам
            List<int> rowsOffsets = new List<int>();

            foreach(GridControl cell in this.CurrentCells)
            {
                AddCellLocation(cell, ref colsOffsets, ref rowsOffsets);
            }
            
            colsOffsets.Sort();
            rowsOffsets.Sort();

            //удалим лишние смещения
            for (int i = 1; i < colsOffsets.Count; i++)
            {
                if (IsEqualNumbers(colsOffsets[i-1], colsOffsets[i]))
                {
                    colsOffsets.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 1; i < rowsOffsets.Count; i++)
            {
                if (IsEqualNumbers(rowsOffsets[i - 1], rowsOffsets[i]))
                {
                    rowsOffsets.RemoveAt(i);
                    i--;
                }
            }

            //флаг что нужная ячейка найдена
            bool isFindCell = false;

            //формируем таблицу
            foreach (int row in rowsOffsets)
            {
                foreach (int col in colsOffsets)
                {
                    isFindCell = false;
                    foreach (GridControl cell in this.CurrentCells)
                    {
                        string text = GetTextFromCell(cell, col, row, "\t");
                        if (text != String.Empty)
                        {
                            result += text;
                            isFindCell = true;
                            break;
                        }
                    }
                    if (!isFindCell)
                        result += "\t";
                }
                //убираем лишнюю табуляцию
                if (result.Length >= 2)
                    result = result.Remove(result.Length-1);
               
                result += "\r\n";
            }
            
            return result;
        }

        /// <summary>
        /// Получение текста из ячейки
        /// </summary>
        /// <param name="cell">ячейка</param>
        /// <param name="col">координата по X</param>
        /// <param name="row">координата по Y</param>
        /// <returns>возвращает текст, если ячейка совпадает по координатам</returns>
        private string GetTextFromCell(GridControl cell, int col, int row, string separator)
        {
            int x = cell.GetBounds().X;
            int y = cell.GetBounds().Y;
            if (((col == -1)&&(row == -1))||(IsEqualNumbers(x, col) && IsEqualNumbers(y, row)))
            {
                // result += (cell is MeasureCell) ? String.Format("{0, 20}\t", cell.GetValue()) : String.Format("{0, -20}\t", cell.GetValue());
                if (cell is CaptionCell.OptionalTextCell)
                {
                    return String.Format("{0}{1}", ((CaptionCell.OptionalTextCell)cell).Text, separator);
                }
                else
                {
                    if (cell is MeasureCell)
                    {
                        return String.Format("{0}{1}", ((MeasureCell)cell).GetValueForExcel(), separator);
                    }
                    else
                    {
                        return String.Format("{0}{1}", cell.GetValue(), separator);
                    }
                }
            }

            if (cell is CaptionCell)
            {
                if (((CaptionCell) cell).OptionalCell != null)
                {
                    return GetTextFromCell(((CaptionCell) cell).OptionalCell, col, row, separator);
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Добавляет координаты ячейки в списки
        /// </summary>
        /// <param name="cell">ячейка</param>
        /// <param name="cols">координаты X</param>
        /// <param name="rows">координаты Y</param>
        private void AddCellLocation(GridControl cell, ref List<int> cols, ref List<int> rows)
        {
            int x = cell.GetBounds().X;
            int y = cell.GetBounds().Y;
            if (!cols.Contains(x))
                cols.Add(x);
            if (!rows.Contains(y))
                rows.Add(y);

            if (cell is CaptionCell)
            {
                if (((CaptionCell)cell).OptionalCell != null)
                {
                    AddCellLocation(((CaptionCell)cell).OptionalCell, ref cols, ref rows);
                }
            }

        }

        /// <summary>
        /// Сравнивает 2 числа c учетом погрешности
        /// </summary>
        /// <param name="a">первое число</param>
        /// <param name="b">второе число</param>
        /// <returns>true если числа равны</returns>
        private bool IsEqualNumbers(int a, int b)
        {
            //this.Grid.GridScale.Value
            return (Math.Abs(a - b) <= 1);
        }

        /// <summary>
        /// Обновляет границы
        /// </summary>
        /// <param name="gridControl">контрол</param>
        /// <param name="controlBounds">его абсолютные границы</param>
        /// <param name="controlOffsetBounds">его относительные границы</param>
        /// <param name="parentVisibleBounds">видимые границы его родителя</param>
        private void RefreshBounds(GridControl gridControl, out Rectangle controlBounds,
            out Rectangle controlOffsetBounds, out Rectangle parentVisibleBounds)
        {
            controlBounds = Rectangle.Empty;
            controlOffsetBounds = Rectangle.Empty;
            parentVisibleBounds = Rectangle.Empty;

            if (gridControl == null)
                return;

            controlBounds = gridControl.GetBounds();
            controlOffsetBounds = gridControl.GetOffsetBounds();
            parentVisibleBounds = gridControl.GetParentVisibleBounds();
        }

        /// <summary>
        /// Фокусируем таблицу на gridControl
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prepareCell"></param>
        /// <param name="gridControl"></param>
        private void MoveGridFocus(Keys key, GridControl prepareCell, GridControl gridControl)
        {
            if (gridControl == null)
                return;

            Rectangle controlBounds, controlOffsetBounds, parentVisibleBounds;
            this.RefreshBounds(gridControl, out controlBounds, out controlOffsetBounds, 
                out parentVisibleBounds);

            switch (key)
            {
                case Keys.Up:
                    {
                        if (controlOffsetBounds.Top + 1 < parentVisibleBounds.Top)
                            this.Grid.ScrollByVertical(controlBounds.Top + 1 - parentVisibleBounds.Top, true);
                        else
                            if (controlOffsetBounds.Bottom > parentVisibleBounds.Bottom)
                                this.Grid.ScrollByVertical(controlBounds.Bottom - parentVisibleBounds.Bottom, true);
                        break;
                    }
                case Keys.Down:
                    {
                        if (controlOffsetBounds.Bottom > parentVisibleBounds.Bottom)
                        {
                            if ((controlBounds.Bottom - parentVisibleBounds.Height) < controlBounds.Top)
                                this.Grid.ScrollByVertical(controlBounds.Bottom - parentVisibleBounds.Bottom - 1, true);
                            else
                                this.Grid.ScrollByVertical(controlBounds.Top - parentVisibleBounds.Top, true);
                        }
                        else
                        {
                            if (controlOffsetBounds.Top < parentVisibleBounds.Top)
                                this.Grid.ScrollByVertical(controlBounds.Top - parentVisibleBounds.Top, true);
                        }

                        break;
                    }
                case Keys.Left:
                    {
                        if (controlOffsetBounds.Left < parentVisibleBounds.Left)
                            this.Grid.ScrollByHorizontal(controlBounds.Left - parentVisibleBounds.Left + 1, true);
                        else
                            if (controlOffsetBounds.Right > parentVisibleBounds.Right)
                                this.Grid.ScrollByHorizontal(controlBounds.Right - parentVisibleBounds.Right, true);
                        break;
                    }
                case Keys.Right:
                    {
                        if (controlOffsetBounds.Right > parentVisibleBounds.Right)
                        {
                            if ((controlBounds.Right - parentVisibleBounds.Width) < controlBounds.Left)
                                //если ячейка помещается целиком на экран, поциционируем на ее конец
                                this.Grid.ScrollByHorizontal(controlBounds.Right - parentVisibleBounds.Right, true);
                            else
                                //если ячека не умещается целиком на экран, то позиционируем на ее начало
                                this.Grid.ScrollByHorizontal(controlBounds.Left - parentVisibleBounds.Left, true);
                        }
                        else
                        {
                            if (controlOffsetBounds.Left < parentVisibleBounds.Left)
                                //если ячейка находилась не в зоне видимости родительской области,
                                //позиционируем на ее начало
                                this.Grid.ScrollByHorizontal(controlBounds.Left - parentVisibleBounds.Left, true);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Загрузка настроек из Xml
        /// </summary>
        /// <param name="collectionNode"></param>
        /// <param name="isLoadTemplate"></param>
        public void Load(XmlNode selCellsNode, bool isLoadTemplate)
        {
            if (selCellsNode == null)
                return;
            XmlNodeList selCellNodes = selCellsNode.SelectNodes(GridConsts.selectedCellItem);
            this.HashCodes.Clear();
            foreach (XmlNode cellNode in selCellNodes)
            {
                int hashCode = XmlHelper.GetIntAttrValue(cellNode, "hashCode", -1);
                this.HashCodes.Add(hashCode);
            }

        }

        public void Save(XmlNode selCellsNode)
        {
            foreach(int hashCode in this.HashCodes)
            {
                XmlHelper.AddChildNode(selCellsNode, GridConsts.selectedCellItem,
                                       new string[] {"hashCode", hashCode.ToString()});

            }
        }



        /// <summary>
        /// Выбрать ячейку по координатам в гриде
        /// </summary>
        /// <param name="location"></param>
        private void SelectCellByLocation(Point location)
        {
            this.Grid.IsAllowMoveByGrid = false;
            this.Grid.LocationHelper.Initialize(location, false);
            this.SelectCell(location);

            this.Grid.IsAllowMoveByGrid = true;
            this.ChangeElementInPropertyGrid();
        }

        /// <summary>
        /// Получить координаты выделенной ячейки
        /// </summary>
        /// <returns></returns>
        private Point GetCurrentCellLocation()
        {
            if (this.CurrentCell == null)
                return Point.Empty;

            Rectangle selBounds = this.CurrentCell.GetBounds();
            return new Point(selBounds.Left + 1, selBounds.Top + 1);
        }


        /// <summary>
        /// Обновить текущую выделенную ячейку
        /// </summary>
        public void Refresh()
        {
            if (this.CurrentCells.Count > 0)
            {
                this.CurrentCell = this.CurrentCells[0];
                this.ChangeElementInPropertyGrid();
            }
            //SelectCellByLocation(this._currentCellLocation);
        }

        public ExpertGrid Grid
        {
            get 
            {  
                return this._grid;
            }
            set 
            { 
                this._grid = value; 
            }
        }

        //Текущая выделенная ячейка
        public GridControl CurrentCell
        {
            get 
            {
                return this._currentCell; 
            }
            set 
            {
                this._currentCell = value;
            }
        }

        public GridControl PrepareCell
        {
            get 
            {
                return this._prepareCell;
            }
            set 
            {
                this._prepareCell = value;
            }
        }

        
        public List<GridControl> CurrentCells
        {
            get { return _currentCells; }
            set { _currentCells = value; }
        }

        public List<int> HashCodes
        {
            get { return _hashCodes; }
            set { _hashCodes = value; }
        }


        public List<GridControl> PrepareCells
        {
            get { return _prepareCells; }
            set { _prepareCells = value; }
        }

        /// <summary>
        /// Выделенная область
        /// </summary>
        public Rectangle SelectedBounds
        {
            get
            {
                Rectangle result = Rectangle.Empty;
                if (this.CurrentCells.Count > 0)
                {
                    Point startPoint = this.CurrentCells[0].GetBounds().Location;
                    Rectangle endSelectedBounds = this.CurrentCells[this.CurrentCells.Count - 1].GetBounds();
                    Point endPoint = new Point(endSelectedBounds.Right, endSelectedBounds.Bottom);
                    result = new Rectangle(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                }
                return result;
            }
        }
    }
}
