using System.Drawing;
using System;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Client.MDXExpert.CommonClass;

namespace Krista.FM.Client.MDXExpert.Grid
{
    public partial class ExpertGrid
    {
        #region Реализация работы пользователя с гридом

        public Color HighLightColor
        {
            get { return this.Painter.HighlightColor; }
            set { this.Painter.HighlightColor = value; }
        }

        //Заголовки фильтров

        public Font FilterCaptionsFont
        {
            get
            {
                return this.FilterCaptions.Style.OriginalFont;
            }
            set
            {
                this.FilterCaptions.Style.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color FilterCaptionsStartBackColor
        {
            get
            {
                return this.FilterCaptions.Style.BackColorStart;
            }
            set
            {
                if (this.FilterCaptions.Style.BackColorStart != value)
                {
                    this.FilterCaptions.Style.BackColorStart = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color FilterCaptionsEndBackColor
        {
            get
            {
                return this.FilterCaptions.Style.BackColorEnd;
            }
            set
            {
                if (this.FilterCaptions.Style.BackColorEnd != value)
                {
                    this.FilterCaptions.Style.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color FilterCaptionsForeColor
        {
            get
            {
                return this.FilterCaptions.Style.ForeColor;
            }
            set
            {
                if (this.FilterCaptions.Style.ForeColor != value)
                {
                    this.FilterCaptions.Style.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color FilterCaptionsBorderColor
        {
            get
            {
                return this.FilterCaptions.Style.BorderColor;
            }
            set
            {
                if (this.FilterCaptions.Style.BorderColor != value)
                {
                    this.FilterCaptions.Style.BorderColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Font FilterValuesFont
        {
            get
            {
                return this.FilterCaptions.ValueCellStyle.OriginalFont;
            }
            set
            {
                this.FilterCaptions.ValueCellStyle.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color FilterValuesStartBackColor
        {
            get
            {
                return this.FilterCaptions.ValueCellStyle.BackColorStart;
            }
            set
            {
                if (this.FilterCaptions.ValueCellStyle.BackColorStart != value)
                {
                    this.FilterCaptions.ValueCellStyle.BackColorStart = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color FilterValuesEndBackColor
        {
            get
            {
                return this.FilterCaptions.ValueCellStyle.BackColorEnd;
            }
            set
            {
                if (this.FilterCaptions.ValueCellStyle.BackColorEnd != value)
                {
                    this.FilterCaptions.ValueCellStyle.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color FilterValuesForeColor
        {
            get
            {
                return this.FilterCaptions.ValueCellStyle.ForeColor;
            }
            set
            {
                if (this.FilterCaptions.ValueCellStyle.ForeColor != value)
                {
                    this.FilterCaptions.ValueCellStyle.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color FilterValuesBorderColor
        {
            get
            {
                return this.FilterCaptions.ValueCellStyle.BorderColor;
            }
            set
            {
                if (this.FilterCaptions.ValueCellStyle.BorderColor != value)
                {
                    this.FilterCaptions.ValueCellStyle.BorderColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public bool FilterCaptionsVisible
        {
            get { return this.FilterCaptions.Visible; }
            set 
            {
                if (value != this.FilterCaptions.Visible)
                {
                    this.FilterCaptions.Visible = value;
                    this.RecalculateGrid();
                }
            }
        }

        public int TipDisplayMaxMemberCount
        {
            get { return this.FilterCaptions.TipDisplayMaxMemberCount; }
            set { this.FilterCaptions.TipDisplayMaxMemberCount = value; }
        }

        public DisplayMemberCount DisplayMemberCount
        {
            get { return this.FilterCaptions.DisplayMemberCount; }
            set
            {
                if (value != this.FilterCaptions.DisplayMemberCount)
                {
                    this.FilterCaptions.DisplayMemberCount = value;
                    this.FilterCaptions.SynchronizePivotData(this.PivotData);
                    this.RecalculateGrid();
                }
            }
        }

        public bool IsCaptionIncludeParents
        {
            get { return this.FilterCaptions.IsCaptionIncludeParents; }
            set
            {
                if (value != this.FilterCaptions.IsCaptionIncludeParents)
                {
                    this.FilterCaptions.IsCaptionIncludeParents = value;
                    this.FilterCaptions.SynchronizePivotData(this.PivotData);
                    this.RecalculateGrid();
                }
            }
        }

        //Заголовки строк

        public Font RowCaptionsFont
        {
            get
            {
                return this.RowCaptions.Style.OriginalFont;
            }
            set
            {
                this.RowCaptions.Style.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color RowCaptionsStartBackColor
        {
            get
            {
                return this.RowCaptions.Style.BackColorStart;
            }
            set
            {
                if (this.RowCaptions.Style.BackColorStart != value)
                {
                    this.RowCaptions.Style.BackColorStart = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color RowCaptionsEndBackColor
        {
            get
            {
                return this.RowCaptions.Style.BackColorEnd;
            }
            set
            {
                if (this.RowCaptions.Style.BackColorEnd != value)
                {
                    this.RowCaptions.Style.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color RowCaptionsForeColor
        {
            get
            {
                return this.RowCaptions.Style.ForeColor;
            }
            set
            {
                if (this.RowCaptions.Style.ForeColor != value)
                {
                    this.RowCaptions.Style.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color RowCaptionsBorderColor
        {
            get
            {
                return this.RowCaptions.Style.BorderColor;
            }
            set
            {
                if (this.RowCaptions.Style.BorderColor != value)
                {
                    this.RowCaptions.Style.BorderColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public CellStyle RowCaptionsStyle
        {
            get
            {
                return this.RowCaptions.Style;
            }
            set
            {
                if (value != null)
                {
                    this.RowCaptions.SetStyle(value);
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        //Заголовки колонок

        public Font ColumnCaptionsFont
        {
            get
            {
                return this.ColumnCaptions.Style.OriginalFont;
            }
            set
            {
                this.ColumnCaptions.Style.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color ColumnCaptionsStartBackColor
        {
            get
            {
                return this.ColumnCaptions.Style.BackColorStart;
            }
            set
            {
                if (this.ColumnCaptions.Style.BackColorStart != value)
                {
                    this.ColumnCaptions.Style.BackColorStart = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color ColumnCaptionsEndBackColor
        {
            get
            {
                return this.ColumnCaptions.Style.BackColorEnd;
            }
            set
            {
                if (this.ColumnCaptions.Style.BackColorEnd != value)
                {
                    this.ColumnCaptions.Style.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color ColumnCaptionsForeColor
        {
            get
            {
                return this.ColumnCaptions.Style.ForeColor;
            }
            set
            {
                if (this.ColumnCaptions.Style.ForeColor != value)
                {
                    this.ColumnCaptions.Style.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color ColumnCaptionsBorderColor
        {
            get
            {
                return this.ColumnCaptions.Style.BorderColor;
            }
            set
            {
                if (this.ColumnCaptions.Style.BorderColor != value)
                {
                    this.ColumnCaptions.Style.BorderColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public CellStyle ColumnCaptionsStyle
        {
            get
            {
                return this.ColumnCaptions.Style;
            }
            set
            {
                if (value != null)
                {
                    this.ColumnCaptions.SetStyle(value);
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        //Заголовки показателей

        public Font MeasureCaptionsFont
        {
            get
            {
                return this.MeasureCaptionsSections.Style.OriginalFont;
            }
            set
            {
                this.MeasureCaptionsSections.Style.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color MeasureCaptionsStartBackColor
        {
            get
            {
                return this.MeasureCaptionsSections.Style.BackColorStart;
            }
            set
            {
                if (this.MeasureCaptionsSections.Style.BackColorStart != value)
                {
                    this.MeasureCaptionsSections.Style.BackColorStart = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color MeasureCaptionsEndBackColor
        {
            get
            {
                return this.MeasureCaptionsSections.Style.BackColorEnd;
            }
            set
            {
                if (this.MeasureCaptionsSections.Style.BackColorEnd != value)
                {
                    this.MeasureCaptionsSections.Style.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color MeasureCaptionsForeColor
        {
            get
            {
                return this.MeasureCaptionsSections.Style.ForeColor;
            }
            set
            {
                if (this.MeasureCaptionsSections.Style.ForeColor != value)
                {
                    this.MeasureCaptionsSections.Style.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color MeasureCaptionsBorderColor
        {
            get
            {
                return this.MeasureCaptionsSections.Style.BorderColor;
            }
            set
            {
                if (this.MeasureCaptionsSections.Style.BorderColor != value)
                {
                    this.MeasureCaptionsSections.Style.BorderColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public CellStyle MeasureCaptionsStyle
        {
            get
            {
                return this.MeasureCaptionsSections.Style;
            }
            set
            {
                if (value != null)
                {
                    this.MeasureCaptionsSections.SetStyle(value);
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        //Строки

        public Font RowAxisFont
        {
            get
            {
                return this.Row.Style.OriginalFont;
            }
            set
            {
                this.Row.Style.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color RowAxisStartBackColor
        {
            get
            {
                return this.Row.Style.BackColorStart;
            }
            set
            {
                if (this.Row.Style.BackColorStart != value)
                {
                    this.Row.Style.BackColorStart = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color RowAxisEndBackColor
        {
            get
            {
                return this.Row.Style.BackColorEnd;
            }
            set
            {
                if (this.Row.Style.BackColorEnd != value)
                {
                    this.Row.Style.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color RowAxisForeColor
        {
            get
            {
                return this.Row.Style.ForeColor;
            }
            set
            {
                if (this.Row.Style.ForeColor != value)
                {
                    this.Row.Style.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color RowAxisBorderColor
        {
            get
            {
                return this.Row.Style.BorderColor;
            }
            set
            {
                if (this.Row.Style.BorderColor != value)
                {
                    this.Row.Style.BorderColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public bool AutoSizeRows
        {
            get
            {
                return this.Row.AutoSizeRows;
            }
            set
            {
                this.Row.AutoSizeRows = value;
                this.RecalculateGrid();
            }
        }

        public CellStyle RowAxisStyle
        {
            get
            {
                return this.Row.Style;
            }
            set
            {
                if (value != null)
                {
                    this.Row.SetStyle(value);
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        //Колонки

        public Font ColumnAxisFont
        {
            get
            {
                return this.Column.Style.OriginalFont;
            }
            set
            {
                this.Column.Style.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color ColumnAxisStartBackColor
        {
            get
            {
                return this.Column.Style.BackColorStart;
            }
            set
            {
                if (this.Column.Style.BackColorStart != value)
                {
                    this.Column.Style.BackColorStart = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color ColumnAxisEndBackColor
        {
            get
            {
                return this.Column.Style.BackColorEnd;
            }
            set
            {
                if (this.Column.Style.BackColorEnd != value)
                {
                    this.Column.Style.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color ColumnAxisForeColor
        {
            get
            {
                return this.Column.Style.ForeColor;
            }
            set
            {
                if (this.Column.Style.ForeColor != value)
                {
                    this.Column.Style.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color ColumnAxisBorderColor
        {
            get
            {
                return this.Column.Style.BorderColor;
            }
            set
            {
                if (this.Column.Style.BorderColor != value)
                {
                    this.Column.Style.BorderColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public CellStyle ColumnAxisStyle
        {
            get
            {
                return this.Column.Style;
            }
            set
            {
                if (value != null)
                {
                    this.Column.SetStyle(value);
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        //Область данных

        public Font DataAreaFont
        {
            get
            {
                return this.MeasuresData.Style.OriginalFont;
            }
            set
            {
                this.MeasuresData.Style.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color DataAreaBackColor
        {
            get
            {
                return this.MeasuresData.Style.BackColorEnd;
            }
            set
            {
                if (this.MeasuresData.Style.BackColorEnd != value)
                {
                    this.MeasuresData.Style.BackColorStart = value;
                    this.MeasuresData.Style.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color DataAreaForeColor
        {
            get
            {
                return this.MeasuresData.Style.ForeColor;
            }
            set
            {
                if (this.MeasuresData.Style.ForeColor != value)
                {
                    this.MeasuresData.Style.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color DataAreaBorderColor
        {
            get
            {
                return this.MeasuresData.Style.BorderColor;
            }
            set
            {
                if (this.MeasuresData.Style.BorderColor != value)
                {
                    this.MeasuresData.Style.BorderColor = value;
                    this.MeasuresData.StyleForTotals.BorderColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public CellStyle DataAreaStyle
        {
            get
            {
                return this.MeasuresData.Style;
            }
            set
            {
                if (value != null)
                {
                    this.MeasuresData.SetStyle(value);
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Font DataAreaTotalsFont
        {
            get
            {
                return this.MeasuresData.StyleForTotals.OriginalFont;
            }
            set
            {
                this.MeasuresData.StyleForTotals.OriginalFont = value;
                this.RecalculateGrid();
            }
        }

        public Color DataAreaTotalsBackColor
        {
            get
            {
                return this.MeasuresData.StyleForTotals.BackColorEnd;
            }
            set
            {
                if (this.MeasuresData.StyleForTotals.BackColorEnd != value)
                {
                    this.MeasuresData.StyleForTotals.BackColorStart = value;
                    this.MeasuresData.StyleForTotals.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color DataAreaTotalsForeColor
        {
            get
            {
                return this.MeasuresData.StyleForTotals.ForeColor;
            }
            set
            {
                if (this.MeasuresData.StyleForTotals.ForeColor != value)
                {
                    this.MeasuresData.StyleForTotals.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public CellStyle DataTotalsAreaStyle
        {
            get
            {
                return this.MeasuresData.StyleForTotals;
            }
            set
            {
                if (value != null)
                {
                    this.MeasuresData.SetTotalStyle(value);
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color DataAreaDummyBackColor
        {
            get
            {
                return this.MeasuresData.StyleForDummyCells.BackColorEnd;
            }
            set
            {
                if (this.MeasuresData.StyleForDummyCells.BackColorEnd != value)
                {
                    this.MeasuresData.StyleForDummyCells.BackColorStart = value;
                    this.MeasuresData.StyleForDummyCells.BackColorEnd = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public Color DataAreaDummyForeColor
        {
            get
            {
                return this.MeasuresData.StyleForDummyCells.ForeColor;
            }
            set
            {
                if (this.MeasuresData.StyleForDummyCells.ForeColor != value)
                {
                    this.MeasuresData.StyleForDummyCells.ForeColor = value;
                    this.DrawGrid(AreaSet.All);
                }
            }
        }

        public CellStyle DataDummyAreaStyle
        {
            get
            {
                return this.MeasuresData.StyleForDummyCells;
            }
            set
            {
                if (value != null)
                {
                    this.MeasuresData.SetDummyStyle(value);
                    this.DrawGrid(AreaSet.All);
                }
            }
        }


        //Общие свойства

        public Point GridLocation
        {
            get
            {
                return this._gridLocation;
            }
            set
            {
                this._gridLocation = value;
                this.RecalculateGrid();
            }
        }

        public int OriginalSeparatorHeight
        {
            get
            {
                return this._separatorHeight;
            }
            set
            {
                this._separatorHeight = value;
                this.RecalculateGrid();
            }
        }

        public int SeparatorHeight
        {
            get
            {
                return this.GridScale.GetScaledValue(this._separatorHeight);
            }
            set
            {
                this._separatorHeight = value;
                this.RecalculateGrid();
            }
        }

        //комментарии
        public int CommentDisplayDelay 
        {
            get
            {
                return this.CellComments.TimerInterval;
            }
            set
            {
                this.CellComments.TimerInterval = value;
            }
        }

        public bool IsDisplayComments 
        {
            get
            {
                return this.CellComments.IsDisplayComments;
            }
            set
            {
                this.CellComments.IsDisplayComments = value;
            }
        }

        public int CommentMaxWidth 
        { 
            get
            {
                return this.CellComments.MaxWidth;
            }
            set
            {
                this.CellComments.MaxWidth = value;
            }
        }

        /// <summary>
        /// Стиль отображения комментария
        /// </summary>
        public CellStyle CommentStyle 
        { 
            get
            {
                return this.CellComments.Style;
            }
            set
            {
                if (value != null)
                {
                    this.CellComments.Style = value;
                }
            }
        }

        /// <summary>
        /// Отображать комментарий к ячейке, до ее смены
        /// </summary>
        public bool DisplayCommentUntilControlChange 
        {
            get { return this.CellComments.DisplayUntillControlChange; }
            set { this.CellComments.DisplayUntillControlChange = value; }
        }

        //MemberProperties
        /// <summary>
        /// Шрифт имени, свойства элемента оси строк
        /// </summary>
        public Font RowMemberPropertiesNameFont 
        {
            get { return this.Row.MemProperNameStyle.OriginalFont;}
            set 
            {
                if (value != this.Row.MemProperNameStyle.OriginalFont)
                {
                    this.Row.MemProperNameStyle.OriginalFont = value;
                    this.RecalculateGrid();
                }
            }
        }

        /// <summary>
        /// Шрифт значения, свойства элемента оси строк
        /// </summary>
        public Font RowMemberPropertiesValueFont
        {
            get { return this.Row.MemProperValueStyle.OriginalFont; }
            set
            {
                if (value != this.Row.MemProperValueStyle.OriginalFont)
                {
                    this.Row.MemProperValueStyle.OriginalFont = value;
                    this.RecalculateGrid();
                }
            }
        }

        /// <summary>
        /// Цвет имени, свойства элемента оси строк
        /// </summary>
        public Color RowMemberPropertiesNameForeColor
        {
            get { return this.Row.MemProperNameStyle.ForeColor; }
            set
            {
                this.Row.MemProperNameStyle.ForeColor = value;
                this.DrawGrid(AreaSet.AllAxis);
            }
        }

        /// <summary>
        /// Цвет значения, свойства элемента оси строк
        /// </summary>
        public Color RowMemberPropertiesValueForeColor
        {
            get { return this.Row.MemProperValueStyle.ForeColor; }
            set
            {
                this.Row.MemProperValueStyle.ForeColor = value;
                this.DrawGrid(AreaSet.AllAxis);
            }
        }

        public CellStyle RowMemberPropertiesStyle
        {
            get
            {
                return this.Row.MemProperValueStyle;
            }
            set
            {
                if (value != null)
                {
                    this.Row.MemProperValueStyle = value;
                    this.DrawGrid(AreaSet.AllAxis);
                }
            }
        }

        /// <summary>
        /// Шрифт имени, свойства элемента оси столбцов
        /// </summary>
        public Font ColumnMemberPropertiesNameFont
        {
            get { return this.Column.MemProperNameStyle.OriginalFont; }
            set
            {
                if (value != this.Column.MemProperNameStyle.OriginalFont)
                {
                    this.Column.MemProperNameStyle.OriginalFont = value; ;
                    this.RecalculateGrid();
                }
            }
        }

        /// <summary>
        /// Шрифт значения, свойства элемента оси столбцов
        /// </summary>
        public Font ColumnMemberPropertiesValueFont
        {
            get { return this.Column.MemProperValueStyle.OriginalFont; }
            set
            {
                if (value != this.Column.MemProperValueStyle.OriginalFont)
                {
                    this.Column.MemProperValueStyle.OriginalFont = value;
                    this.RecalculateGrid();
                }
            }
        }

        /// <summary>
        /// Цвет имени, свойства элемента оси столбцов
        /// </summary>
        public Color ColumnMemberPropertiesNameForeColor
        {
            get { return this.Column.MemProperNameStyle.ForeColor; }
            set
            {
                this.Column.MemProperNameStyle.ForeColor = value;
                this.DrawGrid(AreaSet.AllAxis);
            }
        }

        /// <summary>
        /// Цвет значения, свойства элемента оси столбцов
        /// </summary>
        public Color ColumnMemberPropertiesValueForeColor
        {
            get { return this.Column.MemProperValueStyle.ForeColor; }
            set
            {
                this.Column.MemProperValueStyle.ForeColor = value;
                this.DrawGrid(AreaSet.AllAxis);
            }
        }

        public CellStyle ColumnMemberPropertiesStyle
        {
            get
            {
                return this.Column.MemProperValueStyle;
            }
            set
            {
                if (value != null)
                {
                    this.Column.MemProperValueStyle = value;
                    this.DrawGrid(AreaSet.AllAxis);
                }
            }
        }

        /// <summary>
        /// Масштаб таблицы
        /// </summary>
        public int GridPercentScale
        {
            get { return this.GridScale.PercentValue; }
            set {  this.GridScale.PercentValue = value; }
        }

        #endregion

        #region Делегаты
        public delegate void SortClickHandler(object sender, string uniqueName,
            Krista.FM.Client.MDXExpert.Data.SortType sortType, string sortedTupleUN);
        public delegate void DropButtonClickHandler(object sender, string hierarchyUN);
        public delegate void ObjectSelectedHandler(SelectionType selectionType, string objectUN);
        public delegate void DrillThroughHandler(string measureUN, string rowCellUN, bool rowCellIsTotal,
            string columnCellUN, bool columnCellIsTotal, string actionName);
        public delegate void ExpandedMemberHandler(string dimemsionUN, string levelUN, bool state);
        #endregion

        #region События
        private event SortClickHandler _sortClick;
        private event DropButtonClickHandler _dropButtonClick;
        private event ObjectSelectedHandler _objectSelected;
        private event DrillThroughHandler _drillThrough;
        private event ExpandedMemberHandler _expandedMember;
        private event EventHandler _recalculatedGrid;
        private event EventHandler _gridSizeChanged;
        private event EventHandler _colorRulesChanged;
        private event EventHandler _scaleChanged;

        /// <summary>
        /// Нажали на сортировку
        /// </summary>
        public event SortClickHandler SortClick
        {
            add { _sortClick += value; }
            remove { _sortClick -= value; }
        }

        /// <summary>
        /// Нажали на кнопку показа структуры измерения
        /// </summary>
        public event DropButtonClickHandler DropButtonClick
        {
            add { _dropButtonClick += value; }
            remove { _dropButtonClick -= value; }
        }

        /// <summary>
        /// Возникает при выделенеии элемента таблицы
        /// </summary>
        public event ObjectSelectedHandler ObjectSelected
        {
            add { _objectSelected += value; }
            remove { _objectSelected -= value; }
        }

        /// <summary>
        /// Возникает при запросе детальных данных яейки меры
        /// </summary>
        public event DrillThroughHandler DrillThrough
        {
            add { _drillThrough += value; }
            remove { _drillThrough -= value; }
        }

        /// <summary>
        /// Возникает при разворачивания элмента, при динамическом режиме загрузки данных
        /// </summary>
        public event ExpandedMemberHandler ExpandedMember
        {
            add { _expandedMember += value; }
            remove { _expandedMember -= value; }
        }

        /// <summary>
        /// Возникает при пересчете размеров таблицы
        /// </summary>
        public event EventHandler RecalculatedGrid
        {
            add { _recalculatedGrid += value; }
            remove { _recalculatedGrid -= value; }
        }

        /// <summary>
        /// Возникает при изменении размеров таблицы
        /// </summary>
        public event EventHandler GridSizeChanged
        {
            add { _gridSizeChanged += value; }
            remove { _gridSizeChanged -= value; }
        }

        /// <summary>
        /// Возникает при изменении условной раскраски
        /// </summary>
        public event EventHandler ColorRulesChanged
        {
            add { _colorRulesChanged += value; }
            remove { _colorRulesChanged -= value; }
        }

        /// <summary>
        /// Возникает при изменении масштаба
        /// </summary>
        public event EventHandler ScaleChanged
        {
            add { _scaleChanged += value; }
            remove { _scaleChanged -= value; }
        }

        #endregion

        #region Экспорт

        public void ExportToWorkbook(string bookPath, string sheetName, bool isPrintVersion, bool isSeparateProperties)
        {
            this.Exporter.ToExcelWorkbook(bookPath, sheetName, isPrintVersion, isSeparateProperties);
        }

        #endregion
    }
}
