using System;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Базовый класс абсолютно всех визуальных элементов грида
    /// </summary>
    public abstract class GridControl
    {
        private ExpertGrid _grid;
        private GridObject _gridObject;
        private ControlState _state;

        public GridControl(ExpertGrid grid, GridObject gridObject)
        {
            this.Grid = grid;
            this.GridObject = gridObject;
        }

        /// <summary>
        /// Очистка
        /// </summary>
        public void Clear()
        {
            this.Grid = null;
        }

        /// <summary>
        /// Тип объекта
        /// </summary>
        public GridObject GridObject
        {
            get { return this._gridObject; }
            set { this._gridObject = value; }
        }

        /// <summary>
        /// Ссылка на грид
        /// </summary>
        public ExpertGrid Grid
        {
            get { return this._grid; }
            set { this._grid = value; }
        }

        /// <summary>
        /// Состояние ячейки
        /// </summary>
        public ControlState State
        {
            get { return this._state; }
            set { this._state = value; }
        }

        /// <summary>
        /// Получить комментарий
        /// </summary>
        /// <returns></returns>
        public abstract string GetComment();

        /// <summary>
        /// Получить хеш код объекта грида
        /// </summary>
        /// <returns></returns>
        public abstract int GetHashCode();
        /// <summary>
        /// Получить значение отображаемое в контроле
        /// </summary>
        /// <returns></returns>
        public abstract string GetValue();
        public abstract void OnClick(Point mousePosition);
        public abstract void OnPaint(Graphics graphics, Painter painter);
        public abstract bool GetHitTest(Point point);
        public abstract bool GetHitTest(Rectangle searchBounds);
        public abstract bool GetHitTest(Point point, bool isFindByOffsetBounds);
        //public abstract bool GetHitTest(Rectangle searchBounds, bool isFindByOffsetBounds);
        public abstract bool IsClickChildElement(Point point);
        public abstract Rectangle GetVisibleBounds();
        public abstract Rectangle GetOffsetBounds();
        public abstract Rectangle GetBounds();
        public abstract Rectangle GetParentVisibleBounds();
    }
}