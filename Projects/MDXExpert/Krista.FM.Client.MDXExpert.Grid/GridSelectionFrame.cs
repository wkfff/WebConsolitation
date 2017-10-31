using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// ����� ��������� �����
    /// </summary>
    public class GridSelectionFrame
    {
        private ExpertGrid grid;
        private GridCell currentCell;
        private bool isStartDrag;
        private Point startPosition = Point.Empty;
        private Point currentPosition = Point.Empty;

        public GridSelectionFrame(ExpertGrid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// ���� ���� ��� �������������, �������...
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Cursor(��� �������)</returns>
        public void Move(Point position)
        {
            if (this.IsDrag)
            {
                this.currentPosition = position;
                using (Graphics graphics = this.grid.GetGridGraphics())
                {
                    //������ ���� � �������������� ����(�� ��������)
                    graphics.DrawImage(this.grid.GridShot, 0, 0);
                    //������ �����
                    this.Draw(graphics, this.grid.Painter);
                }
            }
        }

        /// <summary>
        /// ������ �����
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="painter"></param>
        public void Draw(Graphics graphics, Painter painter)
        {
            if (this.isStartDrag)
            {
                Rectangle gridBounds = this.grid.GridBounds;
                graphics.Clip = new Region(gridBounds);
                int x = (this.currentPosition.X < this.startPosition.X) ? this.currentPosition.X : this.startPosition.X;
                int y = (this.currentPosition.Y < this.startPosition.Y) ? this.currentPosition.Y : this.startPosition.Y;
                int width = Math.Abs(this.currentPosition.X - this.startPosition.X);
                int height = Math.Abs(this.currentPosition.Y - this.startPosition.Y);
                Rectangle bounds = new Rectangle(x, y, width, height);
                painter.DrawSelectionFrame(graphics, bounds);
            }
        }

        /// <summary>
        /// ����� ��������� ��������������, ���������� ����� ������� ������
        /// </summary>
        public void EndDrag(bool isNeedRedraw)
        {
            this.ResetDrag();
            if (isNeedRedraw)
            {
                //this.grid.RecalculateGrid();
                this.grid.DrawGrid(AreaSet.All);
            }
        }

        public void ResetDrag()
        {
            this.currentCell = null;
            this.isStartDrag = false;
            this.startPosition = Point.Empty;
            this.currentPosition = Point.Empty;
            this.grid.GridShot = null;
            Cursor.Clip = Screen.PrimaryScreen.Bounds;
        }

        /// <summary>
        /// �������� �������������� ������ ������, ��������� ��� �������, ���� �������������� ��������, ���������� 
        /// ������� ������
        /// </summary>
        /// <param name="startPosition"></param>
        /// <returns>bool(������ ���������, � ����������� �� �������)</returns>
        public bool StartDrag(Point startPosition)
        {
            this.startPosition = startPosition;
            this.currentPosition = this.startPosition;
            this.isStartDrag = true;
                
            /*((this.startPosition.X - this.currentPosition.X) != 0) ||
                               ((this.startPosition.Y - this.currentPosition.Y) != 0);
             */
            return this.isStartDrag;
        }

        /// <summary>
        /// �������, ��� ��� ������������� ������
        /// </summary>
        public bool IsDrag
        {
            get
            {
                return (this.isStartDrag);
            }
        }
    }
}
