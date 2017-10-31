using System.Collections.Generic;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Grid.Style;
using System.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Является базовой для многих коллекций грида
    /// </summary>
    /// <typeparam name="T">Тип элемента</typeparam>
    public abstract class GridCollection <T> : List<T>
    {
        private ExpertGrid _grid;
        private Point _location = Point.Empty;
        private Rectangle _visibleBounds = Rectangle.Empty;

        /// <summary>
        /// Ссылка на грид
        /// </summary>
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
        /// Расположение
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
        /// Граница
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                Size size = new Size(this.Width, this.Height);
                return new Rectangle(this.Location, size);
            }
        }

        /// <summary>
        /// Пустая
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (this.Count == 0);
            }
        }

        /// <summary>
        /// Видимая граница
        /// </summary>
        public Rectangle VisibleBounds
        {
            get
            {
                return this._visibleBounds;
            }
            set
            {
                this._visibleBounds = value;
            }
        }

        public abstract void Draw(Graphics graphics, Painter painter);
        public abstract void RecalculateCoordinates(Point startPoint);
        public abstract Rectangle GetVisibleBounds();
        public abstract void Load(XmlNode collectionNode, bool isLoadTemplate);
        public abstract void Save(XmlNode collectionNode);
        public abstract void SetStyle(CellStyle style);
        public abstract int Width { get; set;}
        public abstract int Height { get; set;}
    }
}
