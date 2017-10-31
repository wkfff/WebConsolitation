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
    /// ������ � ������� ����������
    /// </summary>
    public class MeasureData : GridCollection<MeasureCell>
    {
        private int _rowsCount;
        private CaptionCell _measureCaption;
        private bool _isVisible = true;
        private Rectangle _offsetBounds = Rectangle.Empty;
        private MeasuresData _measureData;

        /// <summary>
        /// ������ ������ � ������� � ���������
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
        /// ������������� ������
        /// </summary>
        /// <param name="values">������ ����� ���� ����, �������� ������� ������ ������������ � ������ ������</param>
        /// <param name="rowsLeafs">�������� �������� ��� �����</param>
        /// <param name="measureCaption">��������� ����������, � �������� ����������� ������ ������</param>
        public void Initialize(Cell[] values, DimensionCell[] rowsLeafs, CaptionCell measureCaption, bool[] isTopCell, bool[] isBottomCell)
        {
            this.MeasureCaption = measureCaption;
            for (int i = 0; (i < values.Length) || (i < rowsLeafs.Length); i++)
            {
                this.Add(new MeasureCell(this, (i < values.Length ? values[i] : null), (rowsLeafs.Length > 0 ? rowsLeafs[i] : null), isTopCell[i], isBottomCell[i]));
            }
        }

        /// <summary>
        /// ���������� ������������ ������ � ������� � �� ���������
        /// </summary>
        /// <param name="startPoint"></param>
        public override void RecalculateCoordinates(Point startPoint)
        {
            //���������� �� ����������� ����� � ��������� � �������� ����������� ������ � �������
            startPoint.X = this.MeasureCaption.Location.X;
            this.Location = startPoint;
            this.IsVisible = this.GetVisibility();
        }

        /// <summary>
        /// ��������� ������ � �������
        /// </summary>
        /// <returns>bool</returns>
        public bool GetVisibility()
        {
            return ((this.MeasureCaption != null) && (this.MeasureCaption.Captions as MeasuresCaptionsSection).IsVisible);
        }

        /// <summary>
        /// ���������� ������ ������, ������� ����������� ������������ ���������� �����
        /// </summary>
        /// <param name="point"></param>
        /// <returns>int(������ ������)</returns>
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

            //��������� � ������ ����
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
        /// ���������� ������� ����� �������� � �������� �������
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
            //��������� � ������ ����
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
        /// ���� ������� ���������� �������� ����� ��������� ���...
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

            //��������������� ����� ��� ������ ������
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
                    //���� � ������ ���������� �������� �����, ���������� � ���������� ���������������� �����
                    measureCell.OnPaint(graphics, painter);
                }
                else
                {
                    //���� ������ �� ������ � � ������ ������ � ������� ��� ��������� ��� �����, 
                    //������ ����� �������� ���������, ������...
                    dummyCell.RowCell = measureCell.RowCell.GetVisibleParent();

                    if (dummyCell.RowCell != null)
                    {
                        if ((prepareParent != dummyCell.RowCell) && !dummyCell.RowCell.IsExistTotal)
                        {
                            dummyCell.OnPaint(graphics, painter, true);
                        }
                        else
                        {
                            //���� ������ ����, ������ �� ���������� ������� ����������� ������,
                            //��� �� ����� �� ������, ������ ��� ����(���� ��� �����, �� ���������� �������),
                            //� ��������� ������� � ��� ��������� ������
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
        /// �������
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

        //��������, ��� ������ ����������� � MeasuresData
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
        /// ���������� ������� ������ ������ � �������
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
        /// ���������� ������� ������ ������ � �������
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
        /// ���������� ������������� ��������� ������ � �������
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
        /// ���������� ������������� ��������� ������ � �������
        /// </summary>
        /// <returns>Rectangle</returns>
        public Rectangle GetOffsetBounds(Rectangle bounds)
        {
            bounds.X -= this.Grid.HScrollBarState.Offset;
            bounds.Y -= this.Grid.VScrollBarState.Offset;
            return bounds;
        }

        /// <summary>
        /// ������������� ���������� ������ � �������
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
        /// ������ �� ��������� ������ � �������
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
        /// ���������� �����
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
        /// ������ �� ��������� ����������, � �������� ����������� ������ ������
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
        /// ������ ������
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
        /// ������ ������
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
        /// ��������� ������
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
    /// ��������� ������ � �������
    /// </summary>
    public class MeasuresData : GridCollection<MeasureData>
    {
        private int _rowsCount;
        private MeasureCaptionsSections _measureCaptionsSections;
        private int _cellStartIndex; //������ ������, � ������� ���������� ���������
        private int _cellEndIndex; //������ ������, �� ������� ������������� ���������
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

            //���� ����� �� ���������� � ������, �������� ��� � ������ ����������
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


            //���� ����� �� ���������� � ������, �������� ��� � ������ ����������
            this.Style.StringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.Style.StringFormat.Alignment = StringAlignment.Far;

            this.MeasureCaptionsSections = grid.MeasureCaptionsSections;
        }

        /// <summary>
        /// ������ ���������� ��������� ������ � �������
        /// </summary>
        /// <param name="startPoint">����� ������</param>
        public override void RecalculateCoordinates(Point startPoint)
        {
            this.Location = startPoint;
            foreach (MeasureData measureData in this)
            {
                measureData.RecalculateCoordinates(startPoint);
            }
        }

        /// <summary>
        /// ���� ������, �� ������������� �����������
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
        /// ���� ������, �� ������������� �����������
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
        /// ���������� ������ ������, ������� ����������� ������������ ���������� �����
        /// </summary>
        /// <param name="point"></param>
        /// <returns>int(������ ������)</returns>
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

            //��������� � ����� ����
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
        /// ���������� ������� ������, ������� ������� �������� �������
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
        /// ��������� �������� ������ ������� ���� �������� (���������� � ������� �������), ��� �� ��������� �������� 
        /// ����� ������� ���� �������� � ������� � �������. ����� ��� ������
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
                //��������� �������� ��������, ������� �������� ������

                //����� ������, ������� ���������, �.�. ����� ���� �� ���������� �����������, �������������� �������� 
                //�� ����������� (������ �� ������������� ����������, ������ ����������)
                Point startPoint = new Point(this.VisibleBounds.Location.X + this.Grid.HScrollBarState.Offset, 
                    this.VisibleBounds.Location.Y);
                //����� �����, ������� ���������
                Point endPoint = new Point(this.VisibleBounds.Right - 1 + this.Grid.HScrollBarState.Offset,
                    this.VisibleBounds.Location.Y);
                
                int startIndex = this.FindIntersectItem(startPoint);
                int endIndex = this.FindIntersectItem(endPoint);
               
                if ((startIndex > this.Count - 1) || (endIndex > this.Count - 1))
                    return;
                
                //����� �� ��������� �������� ����� ��� ���������, ��� �� �� ������ ��� � ������ ������ ������
                MeasureData measureData = this[startIndex];

                startPoint = new Point(this.VisibleBounds.Location.X,
                    this.VisibleBounds.Location.Y + this.Grid.VScrollBarState.Offset);
                //����� �����, ������� ���������
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
        /// ��� �� ��� ������������� ������ ��� �� ��������� ������ � �������, ������ ����������� ��, ���� ���� ��
        /// ���������, ���� �����, �������...
        /// </summary>
        /// <param name="columnsCount"></param>
        private void Balanse(int columnsCount)
        {
            if (columnsCount == this.Count)
                return;
            if (columnsCount > this.Count)
            {
                //��������� ����������� ������
                while (this.Count < columnsCount)
                {
                    this.Add(new MeasureData(this));
                }
            }
            else
            {
                //������� ������ ������
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
        /// ������������� ���������
        /// </summary>
        /// <param name="rowsLeafs">�������� �������� ��� �����</param>
        /// <param name="measuresCaptions">������ ���������� �����������</param>
        /// <param name="cls">���� ���</param>
        public void Initialize(DimensionCell[] rowsLeafs, CaptionCell[] measuresCaptions, CellSet cls)
        {
            //� ������ �����, ��� ���������� � ���� ������. �.�. �� ����������� ������ ��������(� ������ 
            //Axes.InitAxisMembers) �� � ������� ������������ ������ �� ��� ������ �� �������. ���������
            //������� ������ �������� ������������... ��� ������������� ��� �� ���������� ������� �������� ���������
            //�����������, �������� ��� � DimensionCell.TupleIndex ������ � ������ ������ ����. ��� ��������������
            //������ ��������� ������ ���� �������� ��������� �����, ����� � ��� � ������� ������� ��������. 
            //������ �������� ��� ������� �������� �� ����������� �����������, ��� ����������� �� ������ 
            //�������������� ����������.
            
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


            //����������� 
            this.Balanse(measuresCaptions.Length);

            if (measuresCaptions.Length == 0)
            {
                return;
            }

            //���� �������� � ���������� ��������
            bool univariateCells = this.IsUnivariateCells(cls);

            //�.�. ������������� ������ ���� �� �������(�� ��������), ���������� ���������� ��������� � 
            //���������������� �������, ��� ����� ���������� ����� ��� 1 ���� ����� ���.
            Cell[] temp = new Cell[Math.Max(rowsLeafs.Length, 1)];
            //������ � ���������� ������ ������ � k-������ ��� ���
            bool[] isTopCell = new bool[Math.Max(rowsLeafs.Length, 1)];
            //������ � ���������� ������ ������ � k-��������� ��� ���
            bool[] isBottomCell = new bool[Math.Max(rowsLeafs.Length, 1)];

            //�������� ������ �� ��������� ���������� �����������
            MeasureCaptionsSections captionsSections = (measuresCaptions[0].Captions as MeasuresCaptionsSection).Sections;
            //�.� � ������ ������������� ��� pivotData ������ ����������� ����� ����� ������ ���������� ����������
            //�� ������������� ������� ��� �������� ������� ������ - ���! ������� ������ �������.
            int sectionIndex = 0;
            if ((measuresCaptions[0].Captions as MeasuresCaptionsSection).ColumnCell != null)
            {
                //���� � ������ ���������� ���� ����� � ������� ��������, ������ ��� ������� �� �����, � ������������� 
                //����� ��������� � �� ������

                //���� �� ������� ���������� �����������
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
                        //�������������� ������ � �������
                        this[sectionIndex].Initialize(temp, rowsLeafs, measuresCaptions[sectionIndex], isTopCell, isBottomCell);
                        sectionIndex++;
                    }
                }
            }
            else
            {
                //���� �������� � ������ ���� ���� ����������, �� ��� ��� ��������
                for (int c = 0; c < measuresCaptions.Length; c++)
                {
                    CaptionCell measureCaption = captionsSections[0][c];
                    //���� ��������� ���������� �������� ������ ��� ����������� � ���� �� �����
                    if (measureCaption.IsDummy || (cls.Cells.Count < 1))
                    {
                        Array.Clear(temp, 0, temp.Length);
                    }
                    else
                    {
                        //����� ���������� ���������� ������� (������� ��� ������������ ������ � �������) 
                        //���������� ����� ������� 
                        if (rowsLeafs.Length == 0)
                        {
                            temp[0] = this.GetCell(cls.Cells, measureCaption.TupleIndex);
                        }
                        else
                        {
                            this.FillDataSection(rowsLeafs, measuresCaptions, cls, univariateCells, temp, measureCaption, isTopCell, isBottomCell);
                        }
                    }
                    //�������������� ������ � �������
                    this[c].Initialize(temp, rowsLeafs, measuresCaptions[c], isTopCell, isBottomCell);
                }
            }
        }

        /// <summary>
        /// ��������� ������� ������ ��� ������ ����������
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
                        //�������, ��� ������� ������ ��������� � k-������
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
                        //�������, ��� ������� ������ ��������� � k-������
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
        /// ���� ������ ���������� ������ true
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
        /// �������
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
        /// ������� ������� ���������
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = this.Bounds;
            //�.�. �������� �� ���������� ������� ������ ������ �����������, ���������� ���������, ����� �������, ���
            //������� ������ ������ ������ ����� ������� ������ ������
            result.Height = Math.Min(this.Grid.GridBounds.Bottom - result.Location.Y, result.Height + 1);
            result.Width = this.Grid.GridBounds.Right - result.Location.X;
            return result;
        }

        /// <summary>
        /// �������� ������� ������� ������
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
        /// ���������� ������� ������� ������
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
        /// � ������ ���� ��� ������ ������������ ����� (��� � ������ DimensiocCell, CaptionCell), 
        /// ������� ��� ���������� ���� ��� ���� (����� ������) �� ������� ������, � ����� ����� ����� 
        /// ����������� ������ �������� Style
        /// </summary>
        /// <param name="style"></param>
        public override void SetStyle(CellStyle style)
        {
            style.Grid = this.Grid;
            this.Style.CopyDefaultStyle(style);
            this.Style = style;
        }

        /// <summary>
        /// ����� ��� ������
        /// </summary>
        /// <param name="totalStyle"></param>
        public void SetTotalStyle(CellStyle totalStyle)
        {
            totalStyle.Grid = this.Grid;
            this.StyleForTotals.CopyDefaultStyle(totalStyle);
            this.StyleForTotals = totalStyle;
        }

        /// <summary>
        /// ����� ��������� �����
        /// </summary>
        /// <param name="dummyStyle"></param>
        public void SetDummyStyle(CellStyle dummyStyle)
        {
            dummyStyle.Grid = this.Grid;
            this.StyleForDummyCells.CopyDefaultStyle(dummyStyle);
            this.StyleForDummyCells = dummyStyle;
        }

        /// <summary>
        /// ������ �� ��������� �����������
        /// </summary>
        public MeasureCaptionsSections MeasureCaptionsSections
        {
            get { return this._measureCaptionsSections; }
            set { this._measureCaptionsSections = value; }
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        public int RowsCount
        {
            get { return this._rowsCount; }
            set { this._rowsCount = value; }
        }

        /// <summary>
        /// ������
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
        /// ������
        /// </summary>
        public override int Width
        {
            get { return this.MeasureCaptionsSections.Width;}
            set { }
        }

        /// <summary>
        /// ������ ������, � ������� ���������� ��������� � �������
        /// </summary>
        public int CellStartIndex
        {
            get { return this._cellStartIndex; }
            set { this._cellStartIndex = value; }
        }

        /// <summary>
        /// ������ ������, �� ������� ������������� ��������� � �������
        /// </summary>
        public int CellEndIndex
        {
            get { return this._cellEndIndex; }
            set { this._cellEndIndex = value; }
        }

        /// <summary>
        /// ������ ����� ��� ������ � �������
        /// </summary>
        public CellStyle Style
        {
            get { return this._style; }
            set { this._style = value; }
        }

        /// <summary>
        /// ������ ����� ��� ������
        /// </summary>
        public CellStyle StyleForTotals
        {
            get { return this._styleForTotals; }
            set { this._styleForTotals = value; }
        }

        /// <summary>
        /// ����� ��� ����� �� ��������� ���� ��������
        /// </summary>
        public CellStyle StyleForLowerAverageCells
        {
            get { return this._styleForLowerAverageCells; }
            set { this._styleForLowerAverageCells = value; }
        }

        /// <summary>
        /// ����� ��� ����� �� ��������� ���� ��������
        /// </summary>
        public CellStyle StyleForHigherAverageCells
        {
            get { return this._styleForHigherAverageCells; }
            set { this._styleForHigherAverageCells = value; }
        }

        /// <summary>
        /// ����� ��� ����� �� ��������� ���� �������
        /// </summary>
        public CellStyle StyleForLowerMedianCells
        {
            get { return this._styleForLowerMedianCells; }
            set { this._styleForLowerMedianCells = value; }
        }

        /// <summary>
        /// ����� ��� ����� �������� � k-������
        /// </summary>
        public CellStyle StyleForTopCountCells
        {
            get { return this._styleForTopCountCells; }
            set { this._styleForTopCountCells = value; }
        }

        /// <summary>
        /// ����� ��� ����� �������� � k-���������
        /// </summary>
        public CellStyle StyleForBottomCountCells
        {
            get { return this._styleForBottomCountCells; }
            set { this._styleForBottomCountCells = value; }
        }

        /// <summary>
        /// ����� ��� ����� �� ��������� ���� �������
        /// </summary>
        public CellStyle StyleForHigherMedianCells
        {
            get { return this._styleForHigherMedianCells; }
            set { this._styleForHigherMedianCells = value; }
        }


        /// <summary>
        /// ����� ��� ����� �� ��������� ���� ������� ������������ ����������
        /// </summary>
        public CellStyle StyleForLowerDeviationCells
        {
            get { return this._styleForLowerDeviationCells; }
            set { this._styleForLowerDeviationCells = value; }
        }

        /// <summary>
        /// ����� ��� ����� �� ��������� ���� ������� ������������ ����������
        /// </summary>
        public CellStyle StyleForHigherDeviationCells
        {
            get { return this._styleForHigherDeviationCells; }
            set { this._styleForHigherDeviationCells = value; }
        }



        /// <summary>
        /// ������ ����� ��� ��������� �����
        /// </summary>
        public CellStyle StyleForDummyCells
        {
            get { return _styleForDummyCells; }
            set { _styleForDummyCells = value; }
        }

        /// <summary>
        /// �������� ������, ������� ��������
        /// </summary>
        public NumberFormatInfo NumberFormat
        {
            get { return this._numberFormat; }
            set { this._numberFormat = value; }
        }
    }
}
