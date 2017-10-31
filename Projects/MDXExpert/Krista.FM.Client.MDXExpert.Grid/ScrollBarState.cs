using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Infragistics.Win.UltraWinScrollBar;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Состояние скролл бара
    /// </summary>
    public class ScrollBarState
    {
        private ExpertGrid _grid;
        private UltraScrollBar _scrollBar;
        private GridCell _focusCell;
        private Direction _direction;
        private int _value = 0;
        private bool _changed;
        private double _percentOffset = 0;
        private int _offset = 0;
        private int _beginOffset = 0;

        public ScrollBarState(ExpertGrid grid, UltraScrollBar scrollBar, GridCell focusCell, 
            Direction direction, int value)
        {
            this.Grid = grid;
            this.ScrollBar = scrollBar;
            this.FocusCell = focusCell;
            this.ScrollDirection = direction;
            this.Value = value;
            this.Changed = false;
        }

        /// <summary>
        /// Здесь идет проверка на выход значения из допустимого диапазона
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetSafeValue(int value)
        {
            value = Math.Min(value, this.ScrollBar.Maximum);
            value = Math.Max(value, this.ScrollBar.Minimum);
            return value;
        }

        /// <summary>
        /// Учитывая направление скроллирования и координаты ячейки(на которой сфокусирован скролл), возвращает
        /// значение скролл бара
        /// </summary>
        /// <returns>Значение скролл бара</returns>
        public int GetValue()
        {
            int result = this.Value;
            if ((this.FocusCell != null) && (this.FocusCell.Grid != null))
            {
                ExpertGrid grid = this.FocusCell.Grid;

                switch (this.ScrollDirection)
                {
                    case Direction.TopDown:
                        {
                            result = this.FocusCell.Bounds.Bottom - grid.Row.Bounds.Top;
                            break;
                        }
                    case Direction.BottomUp:
                        {
                            result = this.FocusCell.Bounds.Top - grid.Row.Bounds.Top;

                            break;
                        }
                    case Direction.LeftToRight:
                        {
                            Rectangle bounds = grid.MeasureCaptionsSections.IsEmpty ? grid.Column.Bounds : grid.MeasureCaptionsSections.Bounds;
                            result = this.FocusCell.Bounds.Right - bounds.Left;
                            break;
                        }
                    case Direction.RightToLeft:
                        {
                            Rectangle bounds = grid.MeasureCaptionsSections.IsEmpty ? grid.Column.Bounds : grid.MeasureCaptionsSections.Bounds;
                            result = this.FocusCell.Bounds.Left - bounds.Left;
                            break;
                        }
                }
            }
            else
            {
                //если нет ячейки на которой фокусируемся это значит что только произошла инициализация
                //таблицы, и мы вычисляем новое положение скролл бара, относительно старого положения
                //запомненого в процентах
                result = (int)this.PercentOffset;
            }
            result = this.GetSafeValue(result);
            return result;
        }

        /// <summary>
        /// Очистка всех ссылок
        /// </summary>
        public void Clear()
        {
            this.PercentOffset = this.Offset * 10000 / Math.Max(this.ScrollBar.Maximum, 1);
            this.FocusCell = null;
            this.Value = 0;
            this.Changed = false;
        }

        /// <summary>
        /// Ячейка на которой сфокусирован скролл
        /// </summary>
        public GridCell FocusCell
        {
            get
            {
                return this._focusCell;
            }
            set
            {
                this._focusCell = value;
            }
        }

        /// <summary>
        /// Направление движения скролла
        /// </summary>
        public Direction ScrollDirection
        {
            get
            {
                return this._direction;
            }
            set
            {
                this._direction = value;
            }
        }

        /// <summary>
        /// Значение скролла
        /// </summary>
        public int Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        /// <summary>
        /// Признак, что скролл уже выбран(заводится для исключения зацикливания в обработчиках)
        /// </summary>
        public bool Changed
        {
            get
            {
                return this._changed;
            }
            set
            {
                this._changed = value;
            }
        }

        /// <summary>
        /// Процент смещения скролла относитильно максимальному значению
        /// </summary>
        public double PercentOffset
        {
            get 
            {
                double result = this._percentOffset / 10000 * this.ScrollBar.Maximum;
                return Math.Min(result, this.ScrollBar.Maximum - this.ScrollBar.LargeChange - 1);
            }
            set 
            { 
                this._percentOffset = value; 
            }
        }

        public UltraScrollBar ScrollBar
        {
            get 
            { 
                return this._scrollBar; 
            }
            set 
            { 
                this._scrollBar = value; 
            }
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

        public int Offset
        {
            get 
            { 
                return this._offset; 
            }
            set 
            { 
                this._offset = value; 
            }
        }

        /// <summary>
        /// Начальное смещение. Используется при выделении нескольких ячеек, когда при выделении используется прокрутка
        /// </summary>
        public int BeginOffset
        {
            get { return _beginOffset; }
            set { _beginOffset = value; }
        }
    }
}
