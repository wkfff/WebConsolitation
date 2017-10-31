using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Grid
{
    public class GridSplitter
    {
        private ExpertGrid grid;
        private GridCell currentCell;
        private bool isStartDrag;
        private Point startPosition = Point.Empty;
        private Point currentPosition = Point.Empty;

        public GridSplitter(ExpertGrid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Если есть что перетаскивать, таскаем...
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Cursor(тип курсора)</returns>
        public void Move(Point position)
        {
            if (this.IsDrag)
            {
                this.currentPosition = position;
                using (Graphics graphics = this.grid.GetGridGraphics())
                {
                    //рисуем грид в первоночальном виде(до сплитера)
                    graphics.DrawImage(this.grid.GridShot, 0, 0);
                    //рисуем сплитер
                    this.Draw(graphics, this.grid.Painter);
                }
            }
        }

        /// <summary>
        /// Установить оптимальную ширину секции с учетом ширины текста
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public void SetSectionWidth(Point position)
        {
            position.X = position.X + this.grid.HScrollBarState.Offset;
            position.Y = position.Y + this.grid.VScrollBarState.Offset;

            this.currentCell = this.grid.LocationHelper.CellForResize;
            CaptionCell captionCell = this.CurrentCaption;
            if (captionCell == null)
                return;

            int maxCellWidth = 0;
            try
            {
                int curWidth = 0;
                Graphics graph = this.grid.GetGridGraphics();

                switch (captionCell.Captions.Type)
                {
                    case CaptionType.Measures:

                        int sectionIndex = this.grid.MeasuresData.FindIntersectItem(position);
                        MeasureData mData = this.grid.MeasuresData[sectionIndex];

                        //максимальная ширина ячейки в области данных
                        foreach (MeasureCell mCell in mData)
                        {
                            Font f = mCell.IsTotal
                                         ? this.grid.MeasuresData.StyleForTotals.Font
                                         : this.grid.MeasuresData.Style.Font;

                            curWidth = (int) CommonUtils.GetStringWidth(graph, mCell.GetValue(), f);

                            if (curWidth > maxCellWidth)
                                maxCellWidth = curWidth;
                        }

                        //Сравниваем с шириной заголовка показателей
                        curWidth =
                            (int)
                            CommonUtils.GetStringWidth(graph, mData.MeasureCaption.GetValue(),
                                                       mData.MeasureCaption.Style.Font);
                        //учитываем кнопку сортировки
                        curWidth += mData.MeasureCaption.SortButton.Width + 4;
                        if (curWidth > maxCellWidth)
                            maxCellWidth = curWidth;

                        //Сравниваем с шириной заголовка столбцов
                        if (mData.MeasureCaption.Captions != null)
                        {
                            DimensionCell columnCell =
                                ((MeasuresCaptionsSection) mData.MeasureCaption.Captions).ColumnCell;
                            if (columnCell != null)
                            {
                                curWidth =
                                    (int) CommonUtils.GetStringWidth(graph, columnCell.Text, columnCell.Style.Font);
                                //тут нужно учитывать кнопку для раскрытия дочерних элементов
                                curWidth += columnCell.ExistCollapseButton ? this.grid.GridScale.GetScaledValue(20) : 0;
                                if (curWidth > maxCellWidth)
                                    maxCellWidth = curWidth;
                            }
                        }
                        maxCellWidth = this.grid.GridScale.GetNonScaledValue(maxCellWidth);
                        this.currentCell.Width = maxCellWidth + 2;
                        break;

                    case CaptionType.Columns:
                        curWidth = (int)CommonUtils.GetStringWidth(graph, captionCell.Text, captionCell.Style.Font);
                        //учитываем кнопку сортировки и кнопку для вызова списка элементов иерархии
                        curWidth += captionCell.SortButton.Width + captionCell.DropButton.Width + 10;
                        curWidth = this.grid.GridScale.GetNonScaledValue(curWidth);

                        if (this.grid.RowCaptions.IsEmpty)
                        {
                            captionCell.Captions.Width = curWidth;
                        }
                        else if (this.grid.RowCaptions.OriginalWidth < curWidth)
                        {
                            this.grid.RowCaptions.Width = curWidth;
                        }
                        break;

                    case CaptionType.Rows:
                        curWidth = (int)CommonUtils.GetStringWidth(graph, captionCell.Text, captionCell.Style.Font);
                        //учитываем кнопку сортировки и кнопку для вызова списка элементов иерархии
                        curWidth += captionCell.SortButton.Width + captionCell.DropButton.Width + 10;
                        curWidth = this.grid.GridScale.GetNonScaledValue(curWidth);
                        this.currentCell.Width = curWidth;
                        break;

                    case CaptionType.Filters:
                        maxCellWidth = (int)CommonUtils.GetStringWidth(graph, captionCell.Text, captionCell.Style.Font);
                        //учитываем кнопку для вызова списка элементов иерархии
                        maxCellWidth += captionCell.DropButton.Width + 6;
                        //Сравниваем с текстом в ячейке фильтра
                        curWidth = (int)CommonUtils.GetStringWidth(graph, captionCell.OptionalCell.Text, captionCell.OptionalCell.Style.Font) + 2;


                        if (curWidth > maxCellWidth)
                            maxCellWidth = curWidth;

                        maxCellWidth = this.grid.GridScale.GetNonScaledValue(maxCellWidth);
                        this.currentCell.Width = maxCellWidth;
                        break;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// обработка двойного щелчка по сплиттеру
        /// </summary>
        /// <param name="position"></param>
        public bool DoubleClick(Point position)
        {
          
            if (Cursors.VSplit.Equals(Cursor.Current))
            {
                this.startPosition = position;
                this.currentPosition = this.startPosition;

                SetSectionWidth(position);
                this.grid.RecalculateGrid();

                this.ResetDrag();
                Application.DoEvents();
                this.grid.OnGridSizeChanged();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Рисуем сплитер
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public void Draw(Graphics graphics, Painter painter)
        {
            if ((this.currentCell != null) && this.isStartDrag)
            {
                Rectangle gridBounds = this.grid.GridBounds;
                graphics.Clip = new Region(gridBounds);
                int x, y;
                if (Cursors.VSplit.Equals(Cursor.Current))
                {
                    x = this.currentPosition.X;
                    y = gridBounds.Y;
                    Rectangle bounds = new Rectangle(x, y, 0, this.grid.Height - y);
                    painter.DrawVSplitterLine(graphics, bounds);
                }
                else
                {
                    if (this.CurrentCaption != null)
                    {
                        if (this.CurrentCaption.Captions.Type == CaptionType.Measures)
                            x = this.grid.MeasureCaptionsSections.Location.X;
                        else
                            x = this.CurrentCaption.Captions.Location.X;
                    }
                    else
                    {
                        x = this.currentCell.Location.X;
                    }
                    y = this.currentPosition.Y;
                    Rectangle bounds = new Rectangle(x, y, this.grid.Width - x, 0);
                    painter.DrawHSplitterLine(graphics, bounds);
                }
            }
        }

        /// <summary>
        /// После окончания перетаскивания, определяем новые границы ячейки
        /// </summary>
        public void EndDrag()
        {
            if (this.IsDrag)
            {
                CaptionCell captionCell = this.CurrentCaption;
                if (Cursors.VSplit.Equals(Cursor.Current))
                {
                    int widthCell = this.currentPosition.X - this.currentCell.OffsetBounds.Location.X + 1;
                    widthCell = this.grid.GridScale.GetNonScaledValue(widthCell);

                    if (captionCell != null)
                    {
                        switch (captionCell.Captions.Type)
                        {
                            case CaptionType.Filters:
                            case CaptionType.Rows:
                            case CaptionType.Measures:
                                {
                                    this.currentCell.Width = widthCell;
                                    break;
                                }
                            case CaptionType.Columns:
                                {

                                    if (this.grid.RowCaptions.IsEmpty)
                                        captionCell.Captions.Width = widthCell;
                                    else
                                        this.grid.RowCaptions.Width = widthCell;
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    int heightCell = this.currentPosition.Y - this.currentCell.OffsetBounds.Location.Y + 1;

                    heightCell = this.grid.GridScale.GetNonScaledValue(heightCell);

                    if (captionCell != null)
                    {
                        switch (captionCell.Captions.Type)
                        {
                            case CaptionType.Filters:
                            case CaptionType.Rows:
                                {
                                    captionCell.Captions.Height = heightCell;
                                    break;
                                }
                            case CaptionType.Columns:
                                {
                                    this.currentCell.Height = heightCell;
                                    break;
                                }
                            case CaptionType.Measures:
                                {
                                    if (this.grid.RowCaptions.IsEmpty)
                                        captionCell.Captions.Height = heightCell;
                                    else
                                        this.grid.RowCaptions.Height = heightCell;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        this.currentCell.Height = heightCell;
                    }
                }
                this.grid.RecalculateGrid();
            }
            this.ResetDrag();
        }

        public void ResetDrag()
        {
            this.currentCell = null;
            this.isStartDrag = false;
            this.startPosition = Point.Empty;
            this.currentPosition = Point.Empty;
            this.grid.GridShot = null;
        }

        /// <summary>
        /// Начинаем редактирование границ ячейки, проверяем ряд условий, если редактирование возможно, возвращает 
        /// признак успеха
        /// </summary>
        /// <param name="startPosition"></param>
        /// <returns>bool(ячейку подцепили, и редактируем ее границы)</returns>
        public bool StartDrag(Point startPosition)
        {
            this.startPosition = startPosition;
            this.currentPosition = this.startPosition;
            this.currentCell = this.grid.LocationHelper.CellForResize;
            this.isStartDrag = (this.currentCell != null) &&
                (((this.currentCell.OffsetBounds.Right < (startPosition.X + GridConsts.boundsDeflection)) &&
                (this.currentCell.OffsetBounds.Right > (startPosition.X))) ||
                ((this.currentCell.OffsetBounds.Bottom < (startPosition.Y + GridConsts.boundsDeflection)) &&
                (this.currentCell.OffsetBounds.Bottom > (startPosition.Y))));
            return this.isStartDrag;
        }

        /// <summary>
        /// Признак, что уже перетаскиваем ячейку
        /// </summary>
        public bool IsDrag
        {
            get
            {
                return (this.isStartDrag) && (this.currentCell != null);
            }
        }

        private CaptionCell CurrentCaption
        {
            get
            {
                if ((this.currentCell != null) && (this.currentCell.GridObject == GridObject.CaptionCell))
                    return (CaptionCell)this.currentCell;
                else
                    return null;
            }
        }
    }
}
