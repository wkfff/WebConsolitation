using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// ��������������� �����, � ������� ����� ���������� � ������� ���������� �������� � �������� �����.
    /// ���� �����, ��� ���� ��� ����������� ���� ���, � � ������ ������ ���������� ��� ����������.
    /// </summary>
    class LocationHelper
    {
        private ExpertGrid _grid;
        private GridControl _selectedControl;
        private List<GridControl> _selectedControls;
        private GridCell _resizeCell;
        private GridArea _selectedArea;

        public LocationHelper(ExpertGrid grid)
        {
            this.Grid = grid;
        }

        public void Clear()
        {
            this.SelectedControl = null;
            this.SelectedArea = GridArea.Nothing;
            this.CellForResize = null;
            if (this.SelectedControls != null)
                this.SelectedControls.Clear();
        }

        /// <summary>
        /// ������� ��� ���������� �������� ����� ���� ���, ������ ���������� ������������
        /// </summary>
        /// <param name="locationPoint"></param>
        /// <param name="isFindByOffsetBounds">������� ��� ����� ������ ����� ������������� �� 
        /// ������������� �����������, � ������ �������������.</param>
        public void Initialize(Point locationPoint, bool isFindByOffsetBounds)
        {
            this.Clear();

            this.SelectedArea = this.GetSelectedArea(locationPoint, isFindByOffsetBounds);
            this.SelectedControl = this.GetSelectedControl(locationPoint, isFindByOffsetBounds);
            this.CellForResize = this.GetCellForResize(locationPoint, isFindByOffsetBounds);
        }

        public void Initialize(Rectangle searchBounds, bool isFindByOffsetBounds)
        {
            this.Clear();
            this.SelectedControls = this.GetSelectedControls(searchBounds, isFindByOffsetBounds);
        }


        /// <summary>
        /// ���� ������� �����, ������� ����������� ������������ ���������� �����
        /// </summary>
        /// <param name="position">����������</param>
        /// <param name="isFindByOffsetBounds">������� ��� ����� ������ ����� ������������� �� 
        /// ������������� �����������, � ������ �������������.</param>
        /// <returns>GridArea</returns>
        private GridArea GetSelectedArea(Point position, bool isFindByOffsetBounds)
        {
            GridArea result = GridArea.Nothing;
            if (isFindByOffsetBounds)
            {
                if (this.Grid.FilterCaptions.Visible && this.Grid.FilterCaptions.VisibleBounds.Contains(position))
                    result = GridArea.FiltersCaption;
                else
                    if (this.Grid.ColumnCaptions.VisibleBounds.Contains(position))
                        result = GridArea.ColumnsCaption;
                    else
                        if (this.Grid.RowCaptions.VisibleBounds.Contains(position))
                            result = GridArea.RowsCaption;
                        else
                            if (this.Grid.MeasureCaptionsSections.VisibleBounds.Contains(position))
                                result = GridArea.MeasuresCaption;
                            else
                                if (this.Grid.Column.VisibleBounds.Contains(position))
                                    result = GridArea.Columns;
                                else
                                    if (this.Grid.Row.VisibleBounds.Contains(position))
                                        result = GridArea.Rows;
                                    else
                                        if (this.Grid.MeasuresData.VisibleBounds.Contains(position))
                                            result = GridArea.MeasuresData;
            }
            else
            {
                if (this.Grid.FilterCaptions.Visible && this.Grid.FilterCaptions.Bounds.Contains(position))
                    result = GridArea.FiltersCaption;
                else
                    if (this.Grid.ColumnCaptions.Bounds.Contains(position))
                        result = GridArea.ColumnsCaption;
                    else
                        if (this.Grid.RowCaptions.Bounds.Contains(position))
                            result = GridArea.RowsCaption;
                        else
                            if (this.Grid.MeasureCaptionsSections.Bounds.Contains(position))
                                result = GridArea.MeasuresCaption;
                            else
                                if (this.Grid.Column.Bounds.Contains(position))
                                    result = GridArea.Columns;
                                else
                                    if (this.Grid.Row.Bounds.Contains(position))
                                        result = GridArea.Rows;
                                    else
                                        if (this.Grid.MeasuresData.Bounds.Contains(position))
                                            result = GridArea.MeasuresData;
            }
            return result;
        }

        /// <summary>
        /// �� ���������� ���� �������
        /// </summary>
        /// <param name="position">����������</param>
        /// <param name="isFindByOffsetBounds">������� ��� ����� ������ ����� ������������� �� 
        /// ������������� �����������, � ������ �������������.</param>
        /// <returns>GridControl</returns>
        private GridControl GetSelectedControl(Point position, bool isFindByOffsetBounds)
        {
            return this.GetSelectedControl(position, AreaSet.All, isFindByOffsetBounds);
        }

        /// <summary>
        /// �� ����������� ���� ���������� ��������, ������ ���������� ��������� ������ �������� � ������� 
        /// ����� ������
        /// </summary>
        /// <param name="position">����������</param>
        /// <param name="searchArea">������ �������� � ������� ����� ������</param>
        /// <param name="isFindByOffsetBounds">������� ��� ����� ������ ����� ������������� �� 
        /// ������������� �����������, � ������ �������������.</param>
        /// <returns>BaseControl</returns>
        private GridControl GetSelectedControl(Point position, GridArea[] searchArea, bool isFindByOffsetBounds)
        {
            GridControl result = null;
            GridArea selectedArea = this.SelectedArea;

            if (selectedArea == GridArea.Nothing)
                return result;

            //������� �������� �� ��������� ������� � ������ �������� ��� ������
            bool isAffiliate = false;
            foreach (GridArea gridArea in searchArea)
            {
                isAffiliate = (gridArea == selectedArea);
                if (isAffiliate)
                    break;
            }
            //���� ���������� ������� �� ������ � ������ �������� ��� ������, �������
            if (!isAffiliate)
                return result;

            switch (selectedArea)
            {
                case GridArea.FiltersCaption:
                    {
                        result = this.Grid.FilterCaptions.FindCaption(position, isFindByOffsetBounds);
                        break;
                    }
                case GridArea.ColumnsCaption:
                    {
                        result = this.Grid.ColumnCaptions.FindCaption(position);
                        break;
                    }
                case GridArea.RowsCaption:
                    {
                        result = this.Grid.RowCaptions.FindCaption(position);
                        break;
                    }
                case GridArea.MeasuresCaption:
                    {
                        result = this.Grid.MeasureCaptionsSections.FindCaption(position, isFindByOffsetBounds);
                        break;
                    }
                case GridArea.MeasuresData:
                    {
                        result = this.Grid.MeasuresData.FindMeasureCell(position, isFindByOffsetBounds);
                        break;
                    }
                case GridArea.Columns:
                    {
                        result = this.Grid.Column.FindCell(position, isFindByOffsetBounds);
                        break;
                    }
                case GridArea.Rows:
                    {
                        result = this.Grid.Row.FindCell(position, isFindByOffsetBounds);
                        break;
                    }
            }
            return result;
        }


        private List<GridControl> GetSelectedControls(Rectangle bounds, bool isFindByOffsetBounds)
        {
            List<GridControl> result = new List<GridControl>();

            //���� ��������, ������� ������ � ���������� ������� �� ���� ��������
            //FiltersCaptions:
            result.AddRange(this.Grid.FilterCaptions.FindCaptions(bounds, isFindByOffsetBounds));

            //ColumnsCaptions:
            result.AddRange(this.Grid.ColumnCaptions.FindCaptions(bounds));
                    
            //RowsCaption:
            result.AddRange(this.Grid.RowCaptions.FindCaptions(bounds));
            //MeasuresCaptions:
            result.AddRange(this.Grid.MeasureCaptionsSections.FindCaptions(bounds, isFindByOffsetBounds));
            //MeasuresData:
            result.AddRange(this.Grid.MeasuresData.FindMeasureCells(bounds, isFindByOffsetBounds));
            //Columns:
            result.AddRange(this.Grid.Column.FindCells(bounds, isFindByOffsetBounds));
            //Rows:
            result.AddRange(this.Grid.Row.FindCells(bounds, isFindByOffsetBounds));
            
            return result;
        }

        /// <summary>
        /// ���� ������ ��� ��������� ��� �������� � ������� ��������, ���������� � ������������� ���������� 
        /// position.Y, ��� ���������� � ����������� �� ������� � ������� ������ ��������� ������
        /// </summary>
        /// <param name="position"></param>
        /// <param name="isFindByOffsetBounds">������� ��� ����� ������ ����� ������������� �� 
        /// ������������� �����������, � ������ �������������.</param>
        /// <returns></returns>
        private GridCell GetCellForResize(Point position, bool isFindByOffsetBounds)
        {
            GridCell result = null;
            GridArea selectedArea = this.SelectedArea;

            switch (selectedArea)
            {
                case GridArea.FiltersCaption:
                case GridArea.ColumnsCaption:
                case GridArea.MeasuresCaption:
                case GridArea.RowsCaption:
                    {
                        result = this.SelectedCaption;
                        break;
                    }

                case GridArea.Columns:
                    {
                        GridCell columnCell = this.SelectedCell;
                        if (columnCell == null)
                            break;

                        result = this.Grid.MeasureCaptionsSections.FindCaption(new Point(position.X,
                            this.Grid.MeasureCaptionsSections.Location.Y + 1), isFindByOffsetBounds);

                        if (result != null)
                        {
                            if (result.OffsetBounds.Right != columnCell.OffsetBounds.Right)
                            {
                                result = null;
                            }
                            else
                            {
                                if (result.OffsetBounds.Right < (position.X + GridConsts.boundsDeflection))
                                {
                                    break;
                                }
                            }
                        }

                        result = this.Grid.ColumnCaptions.FindCaption(new Point(this.Grid.ColumnCaptions.Location.X + 1,
                            position.Y));

                        if (result != null)
                        {
                            if (result.OffsetBounds.Bottom != columnCell.OffsetBounds.Bottom)
                            {
                                result = null;
                            }
                            else
                            {
                                if (result.OffsetBounds.Bottom < (position.Y + GridConsts.boundsDeflection))
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }

                case GridArea.Rows:
                    {
                        GridCell rowCell = this.SelectedCell;
                        result = this.Grid.RowCaptions.FindCaption(new Point(position.X,
                            this.Grid.RowCaptions.Location.Y + 1));
                        if ((result != null) && (rowCell != null))
                        {
                            if (result.OffsetBounds.Right != rowCell.OffsetBounds.Right)
                            {
                                result = null;
                            }
                        }
                        break;
                    }

                case GridArea.MeasuresData:
                    {
                        result = this.Grid.MeasureCaptionsSections.FindCaption(new Point(position.X,
                            this.Grid.MeasureCaptionsSections.Location.Y + 1), isFindByOffsetBounds);
                        break;
                    }
            }
            return result;
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

        /// <summary>
        /// ���������� ������ �����
        /// </summary>
        public List<GridControl> SelectedControls
        {
            get
            {
                return this._selectedControls;
            }
            set
            {
                this._selectedControls = value;
            }
        }


        /// <summary>
        /// ���������� ������ �����
        /// </summary>
        public GridControl SelectedControl
        {
            get 
            { 
                return this._selectedControl; 
            }
            set 
            { 
                this._selectedControl = value; 
            }
        }

        /// <summary>
        /// ���������� ������ (���������, ���������� ��� ��������� �������)
        /// </summary>
        public GridCell SelectedCell
        {
            get
            {
                if (this.SelectedControl != null)
                {
                    return (this.SelectedControl.GridObject == GridObject.CaptionCell)
                        || (this.SelectedControl.GridObject == GridObject.DimensionCell)
                        || (this.SelectedControl.GridObject == GridObject.GridCaption) ?
                        (GridCell)this.SelectedControl : null;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// ���������� ������ ����
        /// </summary>
        public MeasureCell SelectedMeasureCell
        {
            get
            {
                if (this.SelectedControl != null)
                {
                    return this.SelectedControl.GridObject == GridObject.MeasureCell ?
                        (MeasureCell)this.SelectedControl : null;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// ���������� ���������
        /// </summary>
        public CaptionCell SelectedCaption
        {
            get
            {
                if (this.SelectedControl != null)
                {
                    return (this.SelectedControl.GridObject == GridObject.CaptionCell) ? 
                        (CaptionCell)this.SelectedControl : null;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// ������ ��� ��������� �������� � ������� ��������
        /// </summary>
        public GridCell CellForResize
        {
            get 
            { 
                return this._resizeCell; 
            }
            set 
            { 
                this._resizeCell = value; 
            }
        }

        /// <summary>
        /// ���������� ������� �����
        /// </summary>
        public GridArea SelectedArea
        {
            get 
            { 
                return this._selectedArea; 
            }
            set 
            { 
                this._selectedArea = value; 
            }
        }
    }
}
