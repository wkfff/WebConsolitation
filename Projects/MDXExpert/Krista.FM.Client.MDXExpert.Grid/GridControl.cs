using System;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// ������� ����� ��������� ���� ���������� ��������� �����
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
        /// �������
        /// </summary>
        public void Clear()
        {
            this.Grid = null;
        }

        /// <summary>
        /// ��� �������
        /// </summary>
        public GridObject GridObject
        {
            get { return this._gridObject; }
            set { this._gridObject = value; }
        }

        /// <summary>
        /// ������ �� ����
        /// </summary>
        public ExpertGrid Grid
        {
            get { return this._grid; }
            set { this._grid = value; }
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        public ControlState State
        {
            get { return this._state; }
            set { this._state = value; }
        }

        /// <summary>
        /// �������� �����������
        /// </summary>
        /// <returns></returns>
        public abstract string GetComment();

        /// <summary>
        /// �������� ��� ��� ������� �����
        /// </summary>
        /// <returns></returns>
        public abstract int GetHashCode();
        /// <summary>
        /// �������� �������� ������������ � ��������
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