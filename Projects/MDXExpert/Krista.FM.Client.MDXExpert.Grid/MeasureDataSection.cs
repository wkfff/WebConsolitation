using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Секция с данными показателя
    /// </summary>
    public class MeasureData : GridCollection<MeasureCell>
    {
        private int _rowsCount;
        private CaptionCell _measureCaption;
        private bool _isVisible = true;
        private Rectangle _offsetBounds = Rectangle.Empty;
        private MeasuresData _measureData;

        /// <summary>
        /// Индекс секции с данными в коллекции
        /// </summary>
        public int Index
        {
            get { return this.MeasuresData.IndexOf(this); }
        }


        public MeasureData(MeasuresData measuresData)
        {
            this.MeasuresData = measuresData;
            this.Grid = measuresData.Grid;
        }

        /// <summary>
        /// Инициализация секции
        /// </summary>
        /// <param name="values">Массив ячеек селл сета, значения которых должны отображаться в данной секции</param>
        /// <param name="rowsLeafs">Листовые элементы оси строк</param>
        /// <param name="measureCaption">Заголовок показателя, к которуму принадлежит данная секция</param>
        public void Initialize(Cell[] values, DimensionCell[] rowsLeafs, CaptionCell measureCaption, bool[] isTopCell, bool[] isBottomCell)
        {
            this.MeasureCaption = measureCaption;
            for (int i = 0; (i < values.Length) || (i < rowsLeafs.Length); i++)
            {
                this.Add(new MeasureCell(this, (i < values.Length ? values[i] : null), (rowsLeafs.Length > 0 ? rowsLeafs[i] : null), isTopCell[i], isBottomCell[i]));
            }
        }

        /// <summary>
        /// Определяет расположение секции с данными и ее видимость
        /// </summary>
        /// <param name="startPoint"></param>
        public override void RecalculateCoordinates(Point startPoint)
        {
            //координату по горизонтали берем у заголовка к которому принадлежит секция с данными
            startPoint.X = this.MeasureCaption.Location.X;
            this.Location = startPoint;
            this.IsVisible = this.GetVisibility();
        }

        /// <summary>
        /// Видимость секции с данными
        /// </summary>
        /// <returns>bool</returns>
        public bool GetVisibility()
        {
            return ((this.MeasureCaption != null) && (this.MeasureCaption.Captions as MeasuresCaptionsSection).IsVisible);
        }

        /// <summary>
        /// Возвращает индекс ячейки, которой принадлежит передающаяся параметром точка
        /// </summary>
        /// <param name="point"></param>
        /// <returns>int(индекс ячейки)</returns>
        public int FindIntersectItem(Point point)
        {
            int startIndex = 0;
            int endIndex = this.Count - 1;
            int middle;

            if (endIndex < 0)
                return -1;

            if (startIndex == endIndex)
                return startIndex;
            Rectangle range, startIndexRange, middleIndexRange;

            //Дихотомия в чистом виде
            do
            {
                middle = (int)((endIndex + startIndex) / 2);

                startIndexRange = this[startIndex].GetBounds();
                middleIndexRange = this[middle].GetBounds();

                range = Rectangle.Union(startIndexRange, middleIndexRange);
                range.X = point.X;
                if (range.Height == 0)
                    range.Height = 1;

                if (range.Contains(point))
                    endIndex = middle;
                else
                    startIndex = middle + 1;
            }
            while (startIndex != endIndex);

            return startIndex;
        }


        /// <summary>
        /// Возвращает индексы ячеек входящих в заданную область
        /// </summary>
        /// <param name="searchBounds"></param>
        /// <returns></returns>
        public List<int> FindIntersectItems(Rectangle searchBounds)
        {
            List<int> result = new List<int>();
            int startIndex = 0;
            int endIndex = this.Count - 1;
            int middle;

            if (endIndex < 0)
            {
                result.Add(-1);
                return result;
            }

            if (startIndex == endIndex)
            {
                result.Add(startIndex);
                return result;
            }
            Rectangle range;

            for (int i = 0; i <= endIndex; i++ )
            {
                range = this[i].GetBounds();
                range.X = searchBounds.X;
                if (range.Height == 0)
                    range.Height = 1;

                if (range.IntersectsWith(searchBounds))
                {
                    result.Add(i);
                }
            }
                /*
            //Дихотомия в чистом виде
                do
                {
                    middle = (int)((endIndex + startIndex) / 2);

                    startIndexRange = this[startIndex].GetBounds();
                    middleIndexRange = this[middle].GetBounds();

                    range = Rectangle.Union(startIndexRange, middleIndexRange);
                    range.X = point.X;
                    if (range.Height == 0)
                        range.Height = 1;

                    if (range.Contains(point))
                        endIndex = middle;
                    else
                        startIndex = middle + 1;
                }
                while (startIndex != endIndex);
                */
            return result;
        }


        /// <summary>
        /// Зная заранее вычисленый диапазон ячеек размечаем его...
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public override void Draw(Graphics graphics, Painter painter)
        {
            Rectangle bounds = this.Bounds;
            this.OffsetBounds = this.GetOffsetBounds(bounds);
            this.VisibleBounds = this.GetVisibleBounds(bounds);

            int startIndex = this.MeasuresData.CellStartIndex;
            int endIndex = this.MeasuresData.CellEndIndex;
            MeasureCell measureCell;
            DimensionCell prepareParent = null;
            MeasureCell dummyCell = new MeasureCell(this, null, null, false, false);

            //подкорректируем стили под данную секцию
            this.MeasuresData.Style.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForTotals.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForHigherAverageCells.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForLowerAverageCells.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForHigherDeviationCells.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForLowerDeviationCells.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForHigherMedianCells.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForLowerMedianCells.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForTopCountCells.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;
            this.MeasuresData.StyleForBottomCountCells.StringFormat.Alignment = this.MeasureCaption.MeasureValueFormat.ValueAlignment;

            for (; startIndex <= endIndex; startIndex++)
            {
                measureCell = this[startIndex];
                if (measureCell.IsVisible)
                {
                    //если в ячейке содержится значение итога, отображаем с наложением соответствующего стиля
                    measureCell.OnPaint(graphics, painter);
                }
                else
                {
                    //Если ячейка не видима и у ячейки строки к которой она привязана нет итога, 
                    //значит будем рисовать фиктивную, пустую...
                    dummyCell.RowCell = measureCell.RowCell.GetVisibleParent();

                    if (dummyCell.RowCell != null)
                    {
                        if ((prepareParent != dummyCell.RowCell) && !dummyCell.RowCell.IsExistTotal)
                        {
                            dummyCell.OnPaint(graphics, painter, true);
                        }
                        else
                        {
                            //Если попали сюда, значит мы перебираем индексы схлопнутого уровня,
                            //что бы этого не делать, найдем его итог(если нет итога, то последнего потомка),
                            //и продолжим перебор с его листового инекса
                            if (dummyCell.RowCell.IsExistTotal)
                                startIndex = dummyCell.RowCell.GetTotal().LeafIndex - 1;
                            else
                                startIndex = dummyCell.RowCell.GetLastDescendant().LeafIndex;
                        }
                    }
  
                    prepareParent = dummyCell.RowCell;
                }
            }
        }

        /// <summary>
        /// Очистка
        /// </summary>
        public new void Clear()
        {
            foreach (MeasureCell cell in this)
            {
                cell.Clear();
            }
            this.MeasureCaption = null;
            this.MeasuresData = null;
            this.Grid = null;
            base.Clear();
        }

        //Заглушки, эти методы реализованы в MeasuresData
        public override void Load(System.Xml.XmlNode captionsNode, bool isLoadTemplate)
        {
        }
        public override void Save(System.Xml.XmlNode captionsNode)
        {
        }
        public override void SetStyle(CellStyle style)
        {
            style.Grid = this.Grid;
        }
        /// <summary>
        /// Вычисление видимой обасти секции с данными
        /// </summary>
        /// <returns>Rectangle</returns>
        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = this.Bounds;
            result.X = Math.Max(result.X - this.Grid.HScrollBarState.Offset, this.MeasuresData.VisibleBounds.X);
            result.Y = Math.Max(result.Y - this.Grid.VScrollBarState.Offset, this.MeasuresData.VisibleBounds.Y);
            result.Width = Math.Min(this.Grid.GridBounds.Right - result.X, result.Width + 1);
            result.Height = Math.Min(this.Grid.GridBounds.Bottom - result.Y, result.Height + 1);
            return result;
        }

        /// <summary>
        /// Вычисление видимой обасти секции с данными
        /// </summary>
        /// <returns>Rectangle</returns>
        public Rectangle GetVisibleBounds(Rectangle bounds)
        {
            bounds.X = Math.Max(bounds.X - this.Grid.HScrollBarState.Offset, this.MeasuresData.VisibleBounds.X);
            bounds.Y = Math.Max(bounds.Y - this.Grid.VScrollBarState.Offset, this.MeasuresData.VisibleBounds.Y);
            bounds.Width = Math.Min(this.Grid.GridBounds.Right - bounds.X, bounds.Width + 1);
            bounds.Height = Math.Min(this.Grid.GridBounds.Bottom - bounds.Y, bounds.Height + 1);
            return bounds;
        }

        /// <summary>
        /// Вычисление относительных координат секции с данными
        /// </summary>
        /// <returns>Rectangle</returns>
        public Rectangle GetOffsetBounds()
        {
            Rectangle result = this.Bounds;
            result.X -= this.Grid.HScrollBarState.Offset;
            result.Y -= this.Grid.VScrollBarState.Offset;
            return result;
        }

        /// <summary>
        /// Вычисление относительных координат секции с данными
        /// </summary>
        /// <returns>Rectangle</returns>
        public Rectangle GetOffsetBounds(Rectangle bounds)
        {
            bounds.X -= this.Grid.HScrollBarState.Offset;
            bounds.Y -= this.Grid.VScrollBarState.Offset;
            return bounds;
        }

        /// <summary>
        /// Относительные координаты секции с данными
        /// </summary>
        /// <returns>Rectangle</returns>
        public Rectangle OffsetBounds
        {
            get
            {
                return this._offsetBounds;
            }
            set
            {
                this._offsetBounds = value;
            }
        }

        /// <summary>
        /// Ссылка на коллекцию секций с данными
        /// </summary>
        public MeasuresData MeasuresData
        {
            get
            {
                return this._measureData;
            }
            set
            {
                this._measureData = value;
            }
        }

        /// <summary>
        /// Количестов строк
        /// </summary>
        public int RowsCount
        {
            get
            {
                return this._rowsCount;
            }
            set
            {
                this._rowsCount = value;
            }
        }

        /// <summary>
        /// Ссылка на зоголовок показателя, к которому принадлежит даннай секция
        /// </summary>
        public CaptionCell MeasureCaption
        {
            get
            {
                return this._measureCaption;
            }
            set
            {
                this._measureCaption = value;
            }
        }

        /// <summary>
        /// Высота секции
        /// </summary>
        public override int Height
        {
            get
            {
                int result = 0;
                if (this.IsVisible)
                {
                    if (!this.Grid.Row.IsEmpty)
                    {
                        result = this.Grid.Row.Height;
                    }
                    else
                    {
                        result = this.MeasuresData.Style.Font.Height + 1;
                    }
                }
                return result;
            }
            set { }
        }

        /// <summary>
        /// Ширина секции
        /// </summary>
        public override int Width
        {
            get
            {
                int result = 0;
                if (this.IsVisible)
                    result = (this.MeasureCaption != null) ? this.MeasureCaption.Width : 0;
                return result;
            }
            set { }
        }

        /// <summary>
        /// Видимость секции
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }
            set
            {
                this._isVisible = value;
            }
        }
    }

    /// <summary>
    /// Коллекция секций с данными
    /// </summary>
    public class MeasuresData : GridCollection<MeasureData>
    {
        private int _rowsCount;
        private MeasureCaptionsSections _measureCaptionsSections;
        private int _cellStartIndex; //индекс ячейки, с которой начинается отрисовка
        private int _cellEndIndex; //индекс ячейки, на которой заканчивается отрисовка
        private CellStyle _style;
        private CellStyle _styleForTotals;
        private CellStyle _styleForDummyCells;
        private CellStyle _styleForLowerAverageCells;
        private CellStyle _styleForHigherAverageCells;
        private CellStyle _styleForLowerDeviationCells;
        private CellStyle _styleForHigherDeviationCells;
        private CellStyle _styleForLowerMedianCells;
        private CellStyle _styleForHigherMedianCells;
        private CellStyle _styleForTopCountCells;
        private CellStyle _styleForBottomCountCells;



        private NumberFormatInfo _numberFormat;

        public MeasuresData(ExpertGrid grid)
        {
            this.Grid = grid;
            this.NumberFormat = CultureInfo.CurrentCulture.NumberFormat;
            this.Style = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);
            this.StyleForTotals = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);


            this.StyleForLowerAverageCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);

            this.StyleForHigherAverageCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);

            this.StyleForLowerDeviationCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);

            this.StyleForHigherDeviationCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);

            this.StyleForLowerMedianCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);

            this.StyleForHigherMedianCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);

            this.StyleForTopCountCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);

            this.StyleForBottomCountCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);


            this.StyleForDummyCells = new CellStyle(this.Grid, Color.White, Color.White,
                GridConsts.gridAxisForeColor, GridConsts.gridCommonBorderColor);

            //если текст не помещается в ячейку, обрезаем его и ставим многоточие
            this.StyleForTotals.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForTotals.StringFormat.Alignment = StringAlignment.Far;

            this.StyleForLowerAverageCells.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForLowerAverageCells.StringFormat.Alignment = StringAlignment.Far;

            this.StyleForHigherAverageCells.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForHigherAverageCells.StringFormat.Alignment = StringAlignment.Far;

            this.StyleForLowerDeviationCells.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForLowerDeviationCells.StringFormat.Alignment = StringAlignment.Far;

            this.StyleForHigherDeviationCells.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForHigherDeviationCells.StringFormat.Alignment = StringAlignment.Far;

            this.StyleForLowerMedianCells.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForLowerMedianCells.StringFormat.Alignment = StringAlignment.Far;

            this.StyleForHigherMedianCells.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForHigherMedianCells.StringFormat.Alignment = StringAlignment.Far;

            this.StyleForTopCountCells.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForTopCountCells.StringFormat.Alignment = StringAlignment.Far;

            this.StyleForBottomCountCells.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.StyleForBottomCountCells.StringFormat.Alignment = StringAlignment.Far;


            //если текст не помещается в ячейку, обрезаем его и ставим многоточие
            this.Style.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.Style.StringFormat.Alignment = StringAlignment.Far;

            this.MeasureCaptionsSections = grid.MeasureCaptionsSections;
        }

        /// <summary>
        /// Расчет абсолютных координат секций с данными
        /// </summary>
        /// <param name="startPoint">Точка старта</param>
        public override void RecalculateCoordinates(Point startPoint)
        {
            this.Location = startPoint;
            foreach (MeasureData measureData in this)
            {
                measureData.RecalculateCoordinates(startPoint);
            }
        }

        /// <summary>
        /// Ищем ячейку, по относительным координатам
        /// </summary>
        /// <param name="mousePoint"></param>
        /// <returns></returns>
        public MeasureCell FindMeasureCell(Point mousePoint, bool isFindByOffsetBounds)
        {
            if (isFindByOffsetBounds)
            {
                mousePoint.X = mousePoint.X + this.Grid.HScrollBarState.Offset;
                mousePoint.Y = mousePoint.Y + this.Grid.VScrollBarState.Offset;
            }

            int sectionIndex = this.FindIntersectItem(mousePoint);
            if ((sectionIndex >= 0) && (sectionIndex < this.Count))
            {
                if (!this[sectionIndex].Bounds.Contains(mousePoint))
                    return null;
                MeasureData measureData = this[sectionIndex];
                if (measureData.IsVisible)
                {
                    int cellIndex = measureData.FindIntersectItem(mousePoint);
                    if ((cellIndex >= 0) && (cellIndex < measureData.Count))
                    {
                        if (measureData[cellIndex].GetBounds().Contains(mousePoint))
                            return measureData[cellIndex];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Ищем ячейку, по относительным координатам
        /// </summary>
        /// <param name="mousePoint"></param>
        /// <returns></returns>
        public List<GridControl> FindMeasureCells(Rectangle searchBounds, bool isFindByOffsetBounds)
        {
            List<GridControl> result = new List<GridControl>();
            if (isFindByOffsetBounds)
            {
                if (searchBounds.X < this.Location.X)
                {
                    searchBounds.Width -= this.Location.X - searchBounds.X;
                    searchBounds.X = this.Location.X;
                }

                if (searchBounds.Y < this.Location.Y)
                {
                    searchBounds.Height -= this.Location.Y - searchBounds.Y;
                    searchBounds.Y = this.Location.Y;
                }

                searchBounds.X = searchBounds.X + this.Grid.HScrollBarState.Offset;
                searchBounds.Y = searchBounds.Y + Math.Min(this.Grid.VScrollBarState.BeginOffset, this.Grid.VScrollBarState.Offset);
                searchBounds.Height += Math.Abs(this.Grid.VScrollBarState.BeginOffset - this.Grid.VScrollBarState.Offset);
            }

            List<int> sectionIndexes = this.FindIntersectItems(searchBounds);
            foreach (int sectionIndex in sectionIndexes)
            {
                if ((sectionIndex >= 0) && (sectionIndex < this.Count))
                {
                    if (!this[sectionIndex].Bounds.IntersectsWith(searchBounds))
                        continue;

                    MeasureData measureData = this[sectionIndex];
                    if (measureData.IsVisible)
                    {
                        List<int> cellIndexes = measureData.FindIntersectItems(searchBounds);
                        foreach (int cellIndex in cellIndexes)
                        {
                            if ((cellIndex >= 0) && (cellIndex < measureData.Count))
                            {
                                if ((measureData[cellIndex].RowCell == null) ||
                                    ((measureData[cellIndex].RowCell != null) && measureData[cellIndex].RowCell.IsVisible))
                                {
                                    if (measureData[cellIndex].GetBounds().IntersectsWith(searchBounds))
                                    {
                                        result.Add(measureData[cellIndex]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// Возвращает индекс секции, которой принадлежит передающаяся параметром точка
        /// </summary>
        /// <param name="point"></param>
        /// <returns>int(индекс ячейки)</returns>
        public int FindIntersectItem(Point point)
        {
            if (this.Count == 0)
                return -1;

            int startIndex = 0;
            int endIndex = this.Count - 1;
            int middle;

            if (startIndex == endIndex)
                return startIndex;
            Rectangle range;

            //Дихотомия в чисто виде
            do
            {
                middle = (int)((endIndex + startIndex) / 2);
                range = Rectangle.Union(this[startIndex].Bounds, this[middle].Bounds);
                range.Y = point.Y;
                if (range.Height == 0)
                    range.Height = 1;

                if (range.Contains(point))
                    endIndex = middle;
                else
                    startIndex = middle + 1;
            }
            while (startIndex != endIndex);

            return startIndex;
        }


        /// <summary>
        /// Возвращает индексы секций, которые входятв заданную область
        /// </summary>
        /// <param name="searchBounds"></param>
        /// <returns></returns>
        public List<int> FindIntersectItems(Rectangle searchBounds)
        {
            List<int> result = new List<int>();

            if (this.Count == 0)
            {
                result.Add(-1);
                return result;
            }

            int startIndex = 0;
            int endIndex = this.Count - 1;
            int middle;

            if (startIndex == endIndex)
            {
                result.Add(startIndex);
                return result;
            }
            Rectangle range;
            for (int i = 0; i <= endIndex; i++)
            {
                range = this[i].Bounds;
                range.Y = searchBounds.Y;
                if (range.Height == 0)
                    range.Height = 1;

                if (range.IntersectsWith(searchBounds))
                {
                    result.Add(i);
                }


            }

            return result;
        }

        /// <summary>
        /// Вычисляем диапазон секций которые надо рисовать (попадающие в видимую область), так же вычислиям диапазон 
        /// ячеек которые надо рисовать в секциях с данными. Далее все рисуем
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public override void Draw(Graphics graphics, Painter painter)
        {
            Region oldClip = graphics.Clip;
            try
            {
                this.VisibleBounds = this.GetVisibleBounds();
                graphics.Clip = new Region(this.VisibleBounds);

                if ((this.IsEmpty) || (this.VisibleBounds.Width <= 0) || (this.VisibleBounds.Height <= 0))
                    return;
                //вычисляем диапазон индексов, видимых областей данных

                //точка начала, области видимости, т.к. метод ищет по абсолютным координатам, приплюсовываем смещение 
                //по горизонтали (тоесть из относительной координаты, делаем абсолютную)
                Point startPoint = new Point(this.VisibleBounds.Location.X + this.Grid.HScrollBarState.Offset, 
                    this.VisibleBounds.Location.Y);
                //точка конца, области видимости
                Point endPoint = new Point(this.VisibleBounds.Right - 1 + this.Grid.HScrollBarState.Offset,
                    this.VisibleBounds.Location.Y);
                
                int startIndex = this.FindIntersectItem(startPoint);
                int endIndex = this.FindIntersectItem(endPoint);
               
                if ((startIndex > this.Count - 1) || (endIndex > this.Count - 1))
                    return;
                
                //здесь же вычисляем диапазон ячеек для отрисовки, что бы не делать это в каждой секции данных
                MeasureData measureData = this[startIndex];

                startPoint = new Point(this.VisibleBounds.Location.X,
                    this.VisibleBounds.Location.Y + this.Grid.VScrollBarState.Offset);
                //точка конца, области видимости
                endPoint = new Point(this.VisibleBounds.Location.X,
                    this.VisibleBounds.Bottom - 2 + this.Grid.VScrollBarState.Offset);

                this.CellStartIndex = measureData.FindIntersectItem(startPoint);
                this.CellEndIndex = measureData.FindIntersectItem(endPoint);
                
                for (; startIndex <= endIndex; startIndex++)
                {
                    measureData = this[startIndex];
                    if (measureData.IsVisible)
                    {
                        measureData.Draw(graphics, painter);
                    }
                }
            }
            finally
            {
                graphics.Clip = oldClip;
                graphics = null;
            }
        }

        /// <summary>
        /// Что бы при инициализации каждый раз не создавать секции с данными, просто балансируем их, если мало то
        /// досоздаем, если много, удаляем...
        /// </summary>
        /// <param name="columnsCount"></param>
        private void Balanse(int columnsCount)
        {
            if (columnsCount == this.Count)
                return;
            if (columnsCount > this.Count)
            {
                //добавляем недостающие секции
                while (this.Count < columnsCount)
                {
                    this.Add(new MeasureData(this));
                }
            }
            else
            {
                //удаляем лишние секции
                while (this.Count > columnsCount)
                {
                    MeasureData removeData = this[this.Count - 1];
                    this.Remove(removeData);
                }
            }
            this.TrimExcess();
        }

        private void CreateMembers(int membersCount)
        {
            for (int i = 0; i < membersCount; i++)
            {
                this.Add(new MeasureData(this));
            }
        }

        /// <summary>
        /// Инициализация коллекции
        /// </summary>
        /// <param name="rowsLeafs">Листовые элементы оси строк</param>
        /// <param name="measuresCaptions">Массив заголовков показателей</param>
        /// <param name="cls">селл сет</param>
        public void Initialize(DimensionCell[] rowsLeafs, CaptionCell[] measuresCaptions, CellSet cls)
        {
            //В кратце опишу, что происходит в этом методе. Т.к. мы искуственно делаем иерархию(в методе 
            //Axes.InitAxisMembers) то в таблице отображаются далеко не все данные из селсета. Некоторые
            //кортежи просто напросто игнорируются... При инициализации оси мы запоминаем индексы кортежей требующие
            //отображения, хранятся они в DimensionCell.TupleIndex тоесть у каждой ячейки осей. Для инициализанции
            //данных требуются список всех листовых элементов строк, имено у них и берутся индексы кортежей. 
            //Инексы кортежей для колонок беруться из заголовоков показателей, они вычислиются на стадии 
            //иниициализации заголовков.
            
            if (cls == null)
                return;

            Color lowerAverageColor = this.Grid.PivotData.AverageSettings.LowerAverageColor;
            Color higherAverageColor = this.Grid.PivotData.AverageSettings.HigherAverageColor;
            Color lowerDeviationColor = this.Grid.PivotData.AverageSettings.LowerDeviationColor;
            Color higherDeviationColor = this.Grid.PivotData.AverageSettings.HigherDeviationColor;
            Color lowerMedianColor = this.Grid.PivotData.MedianSettings.LowerMedianColor;
            Color higherMedianColor = this.Grid.PivotData.MedianSettings.HigherMedianColor;
            Color topCountColor = this.Grid.PivotData.TopCountSettings.TopCountColor;
            Color bottomCountColor = this.Grid.PivotData.BottomCountSettings.BottomCountColor;

            this.StyleForLowerAverageCells.BackColorStart = lowerAverageColor;
            this.StyleForLowerAverageCells.BackColorEnd = lowerAverageColor;
            this.StyleForHigherAverageCells.BackColorStart = higherAverageColor;
            this.StyleForHigherAverageCells.BackColorEnd = higherAverageColor;

            this.StyleForLowerDeviationCells.BackColorStart = lowerDeviationColor;
            this.StyleForLowerDeviationCells.BackColorEnd = lowerDeviationColor;
            this.StyleForHigherDeviationCells.BackColorStart = higherDeviationColor;
            this.StyleForHigherDeviationCells.BackColorEnd = higherDeviationColor;

            this.StyleForLowerMedianCells.BackColorStart = lowerMedianColor;
            this.StyleForLowerMedianCells.BackColorEnd = lowerMedianColor;
            this.StyleForHigherMedianCells.BackColorStart = higherMedianColor;
            this.StyleForHigherMedianCells.BackColorEnd = higherMedianColor;

            this.StyleForTopCountCells.BackColorStart = topCountColor;
            this.StyleForTopCountCells.BackColorEnd = topCountColor;
            this.StyleForBottomCountCells.BackColorStart = bottomCountColor;
            this.StyleForBottomCountCells.BackColorEnd = bottomCountColor;


            //балансируем 
            this.Balanse(measuresCaptions.Length);

            if (measuresCaptions.Length == 0)
            {
                return;
            }

            //если работаем с одномерным массивом
            bool univariateCells = this.IsUnivariateCells(cls);

            //т.к. инициализация данных идет по секциям(по колонкам), определяем количество элементов в 
            //инициализирующем массиве, оно равно количеству строк или 1 если строк нет.
            Cell[] temp = new Cell[Math.Max(rowsLeafs.Length, 1)];
            //Массив с признаками входит ячейка в k-первых или нет
            bool[] isTopCell = new bool[Math.Max(rowsLeafs.Length, 1)];
            //Массив с признаками входит ячейка в k-последних или нет
            bool[] isBottomCell = new bool[Math.Max(rowsLeafs.Length, 1)];

            //получаем ссылку на коллекцию заголовков показателей
            MeasureCaptionsSections captionsSections = (measuresCaptions[0].Captions as MeasuresCaptionsSection).Sections;
            //т.к в режиме инициализации без pivotData секции показателей могут иметь разное количество заголовков
            //то универсальной формулы для текущего индекса секции - нет! заведем просто счетчик.
            int sectionIndex = 0;
            if ((measuresCaptions[0].Captions as MeasuresCaptionsSection).ColumnCell != null)
            {
                //если у секции заголовков есть связь с ячейкой колонкой, значит ось колонок не пуста, и инициализация 
                //будет проходить с ее учетом

                //идем по секциям заголовков показателей
                for (int s = 0; s < captionsSections.Count; s++)
                {
                    MeasuresCaptionsSection captionsSection = captionsSections[s];
                    for (int c = 0; c < captionsSection.Count; c++)
                    {
                        CaptionCell measureCaption = captionsSection[c];
                        if (measureCaption.IsDummy || (cls.Cells.Count == 0))
                        {
                            Array.Clear(temp, 0, temp.Length);
                        }
                        else
                        {
                            if (rowsLeafs.Length == 0)
                            {
                                temp[0] = this.GetCell(cls.Cells, measureCaption.TupleIndex);
                            }
                            else
                            {
                                this.FillDataSection(rowsLeafs, measuresCaptions, cls, univariateCells, temp, measureCaption, isTopCell, isBottomCell);
                            }
                        }
                        //инициализируем секцию с данными
                        this[sectionIndex].Initialize(temp, rowsLeafs, measuresCaptions[sectionIndex], isTopCell, isBottomCell);
                        sectionIndex++;
                    }
                }
            }
            else
            {
                //сюда попадаем в случае если есть показатели, но нет оси столбцов
                for (int c = 0; c < measuresCaptions.Length; c++)
                {
                    CaptionCell measureCaption = captionsSections[0][c];
                    //Если заголовок показателя фиктивен данных для отображения у него не будет
                    if (measureCaption.IsDummy || (cls.Cells.Count < 1))
                    {
                        Array.Clear(temp, 0, temp.Length);
                    }
                    else
                    {
                        //здесь происходит наполнение массива (нужного для иницилизации секции с данными) 
                        //значениями ячеек селсета 
                        if (rowsLeafs.Length == 0)
                        {
                            temp[0] = this.GetCell(cls.Cells, measureCaption.TupleIndex);
                        }
                        else
                        {
                            this.FillDataSection(rowsLeafs, measuresCaptions, cls, univariateCells, temp, measureCaption, isTopCell, isBottomCell);
                        }
                    }
                    //инициализируем секцию с данными
                    this[c].Initialize(temp, rowsLeafs, measuresCaptions[c], isTopCell, isBottomCell);
                }
            }
        }

        /// <summary>
        /// Наполняем данными массив для секции показателя
        /// </summary>
        /// <param name="rowsLeafs"></param>
        /// <param name="measuresCaptions"></param>
        /// <param name="cls"></param>
        /// <param name="univariateCells"></param>
        /// <param name="temp"></param>
        /// <param name="measureCaption"></param>
        private void FillDataSection(DimensionCell[] rowsLeafs, CaptionCell[] measuresCaptions, CellSet cls, bool univariateCells, Cell[] temp, CaptionCell measureCaption, bool[] isTopCell, bool[] isBottomCell)
        {
            if (univariateCells)
            {
                for (int r = 0; r < rowsLeafs.Length; r++)
                {
                    temp[r] = this.GetCell(cls.Cells, measuresCaptions.Length * r + measureCaption.TupleIndex);
                }
            }
            else
            {
                for (int r = 0; r < rowsLeafs.Length; r++)
                {
                    temp[r] = this.GetCell(cls.Cells, measureCaption.TupleIndex, rowsLeafs[r].TupleIndex);
                    
                    isTopCell[r] = false;
                    if (this.Grid.PivotData.IsNeedTopCountCalculation())
                    {
                        //признак, что текущая ячейка находится в k-первых
                        Cell tempCell = this.GetCell(cls.Cells, measureCaption.TopCountTupleIndex, rowsLeafs[r].TupleIndex);
                        if ((tempCell != null)&&(tempCell.Value != null))
                        {
                            if (tempCell.Value.ToString() == "1")
                            {
                                isTopCell[r] = true;
                            }
                        }
                    }

                    isBottomCell[r] = false;
                    if (this.Grid.PivotData.IsNeedBottomCountCalculation())
                    {
                        //признак, что текущая ячейка находится в k-первых
                        Cell tempCell = this.GetCell(cls.Cells, measureCaption.BottomCountTupleIndex, rowsLeafs[r].TupleIndex);
                        if ((tempCell != null) && (tempCell.Value != null))
                        {
                            if (tempCell.Value.ToString() == "1")
                            {
                                isBottomCell[r] = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Если массив одномерный вернет true
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        private bool IsUnivariateCells(CellSet cls)
        {
            try
            {
                Cell temp = cls[0, 0];
                if (temp != null)
                    return false;
            }
            catch
            {
            }
            return true;
        }

        private Cell GetCell(CellCollection cells, int index)
        {
            if (cells != null)
            {
                if (index > -1)
                    return cells[index];
            }
            return null;
        }

        private Cell GetCell(CellCollection cells, int index1, int index2)
        {
            if (cells != null)
            {
                if ((index1 > -1) && (index2 > -1))
                    return cells[index1, index2];
            }
            return null;
        }

        /// <summary>
        /// Очистка
        /// </summary>
        public new void Clear()
        {
            foreach (MeasureData measureData in this)
            {
                measureData.Clear();
            }
            base.Clear();
        }

        /// <summary>
        /// Видимая область коллекции
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = this.Bounds;
            //т.к. операция по вычислинею видимых границ данных показателей, достаточно трудоемка, будем считать, что
            //видимая ширина данных всегда равна видимой ширене экрана
            result.Height = Math.Min(this.Grid.GridBounds.Bottom - result.Location.Y, result.Height + 1);
            result.Width = this.Grid.GridBounds.Right - result.Location.X;
            return result;
        }

        /// <summary>
        /// Загрузка свойств области данных
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Load(System.Xml.XmlNode collectionNode, bool isLoadTemplate)
        {
            if (collectionNode == null)
                return;
            this.Style.Load(collectionNode.SelectSingleNode(GridConsts.style), isLoadTemplate);
            this.StyleForTotals.Load(collectionNode.SelectSingleNode(GridConsts.totalsStyle), isLoadTemplate);
            this.StyleForDummyCells.Load(collectionNode.SelectSingleNode(GridConsts.dummyStyle), isLoadTemplate);
        }

        /// <summary>
        /// Сохранение свойств области данных
        /// </summary>
        /// <param name="captionsNode"></param>
        public override void Save(System.Xml.XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;
            this.Style.Save(XmlHelper.AddChildNode(collectionNode, GridConsts.style));
            this.StyleForTotals.Save(XmlHelper.AddChildNode(collectionNode, GridConsts.totalsStyle));
            this.StyleForDummyCells.Save(XmlHelper.AddChildNode(collectionNode, GridConsts.dummyStyle));
        }

        /// <summary>
        /// У ячейки меры нет своего собственного стиля (как у других DimensiocCell, CaptionCell), 
        /// поэтому она использует один для всех (кроме итогов) из области данных, и новый стиль можно 
        /// присваивать просто свойству Style
        /// </summary>
        /// <param name="style"></param>
        public override void SetStyle(CellStyle style)
        {
            style.Grid = this.Grid;
            this.Style.CopyDefaultStyle(style);
            this.Style = style;
        }

        /// <summary>
        /// Стиль для итогов
        /// </summary>
        /// <param name="totalStyle"></param>
        public void SetTotalStyle(CellStyle totalStyle)
        {
            totalStyle.Grid = this.Grid;
            this.StyleForTotals.CopyDefaultStyle(totalStyle);
            this.StyleForTotals = totalStyle;
        }

        /// <summary>
        /// Стиль фиктивных ячеек
        /// </summary>
        /// <param name="dummyStyle"></param>
        public void SetDummyStyle(CellStyle dummyStyle)
        {
            dummyStyle.Grid = this.Grid;
            this.StyleForDummyCells.CopyDefaultStyle(dummyStyle);
            this.StyleForDummyCells = dummyStyle;
        }

        /// <summary>
        /// Ссылка на заголовки показателей
        /// </summary>
        public MeasureCaptionsSections MeasureCaptionsSections
        {
            get { return this._measureCaptionsSections; }
            set { this._measureCaptionsSections = value; }
        }

        /// <summary>
        /// Количестов строк
        /// </summary>
        public int RowsCount
        {
            get { return this._rowsCount; }
            set { this._rowsCount = value; }
        }

        /// <summary>
        /// Высота
        /// </summary>
        public override int Height
        {
            get
            {
                int result = 0;
                if (!this.Grid.Row.IsEmpty)
                {
                    result = this.Grid.Row.Height;
                }
                else
                {
                    if (!this.IsEmpty)
                    {
                        foreach(MeasureData measureData in this)
                            result = Math.Max(result, measureData.Height);
                    }
                }
                return result;
            }
            set { }
        }

        /// <summary>
        /// Ширина
        /// </summary>
        public override int Width
        {
            get { return this.MeasureCaptionsSections.Width;}
            set { }
        }

        /// <summary>
        /// Индекс ячейки, с которой начинается отрисовка в секциях
        /// </summary>
        public int CellStartIndex
        {
            get { return this._cellStartIndex; }
            set { this._cellStartIndex = value; }
        }

        /// <summary>
        /// Индекс ячейки, на которой заканчивается отрисовка в секциях
        /// </summary>
        public int CellEndIndex
        {
            get { return this._cellEndIndex; }
            set { this._cellEndIndex = value; }
        }

        /// <summary>
        /// Эталон стиля для секций с данными
        /// </summary>
        public CellStyle Style
        {
            get { return this._style; }
            set { this._style = value; }
        }

        /// <summary>
        /// Эталон стиля для итогов
        /// </summary>
        public CellStyle StyleForTotals
        {
            get { return this._styleForTotals; }
            set { this._styleForTotals = value; }
        }

        /// <summary>
        /// Стиль для ячеек со значением ниже среднего
        /// </summary>
        public CellStyle StyleForLowerAverageCells
        {
            get { return this._styleForLowerAverageCells; }
            set { this._styleForLowerAverageCells = value; }
        }

        /// <summary>
        /// Стиль для ячеек со значением выше среднего
        /// </summary>
        public CellStyle StyleForHigherAverageCells
        {
            get { return this._styleForHigherAverageCells; }
            set { this._styleForHigherAverageCells = value; }
        }

        /// <summary>
        /// Стиль для ячеек со значением ниже медианы
        /// </summary>
        public CellStyle StyleForLowerMedianCells
        {
            get { return this._styleForLowerMedianCells; }
            set { this._styleForLowerMedianCells = value; }
        }

        /// <summary>
        /// Стиль для ячеек входящих в k-первых
        /// </summary>
        public CellStyle StyleForTopCountCells
        {
            get { return this._styleForTopCountCells; }
            set { this._styleForTopCountCells = value; }
        }

        /// <summary>
        /// Стиль для ячеек входящих в k-последних
        /// </summary>
        public CellStyle StyleForBottomCountCells
        {
            get { return this._styleForBottomCountCells; }
            set { this._styleForBottomCountCells = value; }
        }

        /// <summary>
        /// Стиль для ячеек со значением выше медианы
        /// </summary>
        public CellStyle StyleForHigherMedianCells
        {
            get { return this._styleForHigherMedianCells; }
            set { this._styleForHigherMedianCells = value; }
        }


        /// <summary>
        /// Стиль для ячеек со значением ниже границы стандартного отклонения
        /// </summary>
        public CellStyle StyleForLowerDeviationCells
        {
            get { return this._styleForLowerDeviationCells; }
            set { this._styleForLowerDeviationCells = value; }
        }

        /// <summary>
        /// Стиль для ячеек со значением выше границы стандартного отклонения
        /// </summary>
        public CellStyle StyleForHigherDeviationCells
        {
            get { return this._styleForHigherDeviationCells; }
            set { this._styleForHigherDeviationCells = value; }
        }



        /// <summary>
        /// Эталон стиля для фиктивных ячекк
        /// </summary>
        public CellStyle StyleForDummyCells
        {
            get { return _styleForDummyCells; }
            set { _styleForDummyCells = value; }
        }

        /// <summary>
        /// Числовой формат, текущей культуры
        /// </summary>
        public NumberFormatInfo NumberFormat
        {
            get { return this._numberFormat; }
            set { this._numberFormat = value; }
        }
    }
}
