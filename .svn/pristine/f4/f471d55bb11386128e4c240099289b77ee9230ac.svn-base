using System;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Grid.Style;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Класс от которого наследуется большинство контролов грида, уже имеет свой стиль, 
    /// бордюры, и отображаемый текст
    /// </summary>
    public abstract class GridCell : GridControl
    {
        private string _text = string.Empty;
        private CellStyle _style;
        private Point _location = Point.Empty;
        private Size _size = Size.Empty;
        private int _minWidth = 50;
        private Rectangle _offsetBounds = Rectangle.Empty;

        public GridCell(ExpertGrid grid, GridObject gridObject)
            : base(grid, gridObject)
        {
            this.Size = new System.Drawing.Size(100, 17);
        }

        /// <summary>
        /// Если точка принадлежит этой ячеки вернет True
        /// </summary>
        /// <param name="point"></param>
        /// <returns>bool</returns>
        public override bool GetHitTest(Point point)
        {
            return this.Bounds.Contains(point);
        }

        /// <summary>
        /// Если ячейка входит в область вернет True
        /// </summary>
        /// <param name="searchBounds"></param>
        /// <returns>bool</returns>
        public override bool GetHitTest(Rectangle searchBounds)
        {
            return this.Bounds.IntersectsWith(searchBounds);
        }


        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string GetValue()
        {
            return this.Text;
        }

        /// <summary>
        /// Если точка принадлежит этой ячеки вернет True
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isFindByOffsetBounds">признак, что будет учитывться смещение</param>
        /// <returns>bool</returns>
        public override bool GetHitTest(Point point, bool isFindByOffsetBounds)
        {
            if (isFindByOffsetBounds)
                return this.GetOffsetBounds().Contains(point);
            else
                return this.GetHitTest(point);
        }

        /// <summary>
        /// Устанавливать ширину ячейки, без проверки на минимальный размер
        /// </summary>
        /// <param name="width"></param>
        public void SetUncheckWidth(int width)
        {
            this._size.Width = this.Grid.GridScale.GetNonScaledValue(width);
        }

        /// <summary>
        /// Устанавливать высоту ячейки, без проверки на минимальный размер
        /// </summary>
        /// <param name="width"></param>
        public void SetUncheckHeight(int height)
        {
            this._size.Height = height;
        }

        /// <summary>
        /// Получить комментарий
        /// </summary>
        /// <returns></returns>
        public override string GetComment()
        {
            return this.Text;
        }

        public override Rectangle GetBounds()
        {
            return this.Bounds;
        }

        /// <summary>
        /// Очистка всех ссылок
        /// </summary>
        public new void Clear()
        {
            this.Style = null;
            this.Text = null;
            base.Clear();
        }

        /// <summary>
        /// Текст отображающийся в ячейке
        /// </summary>
        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
            }
        }

        /// <summary>
        /// Стиль ячейки
        /// </summary>
        public CellStyle Style
        {
            get
            {
                return this._style;
            }
            set
            {
                this._style = value;
            }
        }

        /// <summary>
        /// При установки ширины проверяется, не меньше ли присваиваемое значение минимального
        /// </summary>
        public virtual int Width
        {
            get
            {
                return this.Grid.GridScale.GetScaledValue(this._size.Width);
            }
            set
            {
                if (value < this.MinWidth)
                    this._size.Width = this.MinWidth;
                else
                    this._size.Width = value;
            }
        }
        
        /// <summary>
        /// При установки высоты проверяется, не меньше ли присваиваемое значение минимального
        /// </summary>
        public virtual int Height
        {
            get
            {
                return this.Grid.GridScale.GetScaledValue(this._size.Height);
            }
            set
            {
                if (value < this.MinHeight)
                    this._size.Height = this.MinHeight;
                else
                    this._size.Height = value;
            }
        }

        public Size OriginalSize
        {
            get { return this._size; }
        }

        /// <summary>
        /// Расположение ячейки
        /// </summary>
        public Point Location
        {
            get
            {
                return this._location;
            }
            set
            {
                this._location = value;
            }
        }

        /// <summary>
        /// Размеры ячейки
        /// </summary>
        public virtual Size Size
        {
            get
            {
                Size zoomSize = new Size(this.Grid.GridScale.GetScaledValue(this._size.Width), this.Grid.GridScale.GetScaledValue(this._size.Height));
                return zoomSize;
                //return this._size;
            }
            set
            {
                this._size = value;
            }
        }

        /// <summary>
        /// Минимальная высота
        /// </summary>
        public int MinHeight
        {
            get
            {
                return this.Style != null ? this.Style.OriginalTextHeight : 17;
            }
        }

        /// <summary>
        /// Минимальная ширина
        /// </summary>
        public int MinWidth
        {
            get
            {
                return _minWidth;
            }
        }

        /// <summary>
        /// Абсолютные границы ячейки
        /// </summary>
        public Rectangle Bounds
        {
            get { return new Rectangle(this.Location, this.Size);
            }
        }

        /// <summary>
        /// Относительные границы ячейки
        /// </summary>
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
    }
}
