using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Krista.FM.Client.MDXExpert.Common;
using Microsoft.AnalysisServices.AdomdClient;
using System.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Класс оси колонок
    /// </summary>
    public class ColumnAxis : Axis
    {
        public ColumnAxis(ExpertGrid grid)
            : base(grid, AxisType.Columns)
        {
        }

        /// <summary>
        /// Вычисление координат всех элементов оси
        /// </summary>
        /// <param name="location">Координата начала оси</param>
        public override void RecalculateCoordinates(Point location)
        {
            base.Location = location;
            base.LeafCount = 0;
            this.RecalculateCoordinates(base.Root, location, -1);
        }

        protected void RecalculateCoordinates(DimensionCell cell, Point startPoint, int depthLevel)
        {
            if (cell == null)
                return;
            cell.Location = startPoint;
            cell.DepthLevel = depthLevel;
            if (cell.Parent != null)
                cell.IsVisible = cell.IsTotal || cell.IsDummy ? cell.Parent.IsVisible : (cell.Parent.IsVisible && cell.Parent.Expanded);

            //инициализируем наличие кнопки
            cell.InitCollapseButton();

            if (!cell.IsRoot)
            {
                if (!cell.HasChilds || (!cell.Expanded && !cell.IsLeaf))
                    base.LeafCount++;

                this.SetCellWidth(cell);
                this.SetCellHeight(cell);
                //если ячейка фиктивная, рисовать ее не будем, следовательно ее высоту добавляем к родителю 
                //этой фиктивной ячейки
                if (cell.IsDummy)
                {
                    DimensionCell parentDummy = this.GetDummyCellParent(cell);
                    if (parentDummy != null)
                    {
                        parentDummy.SetUncheckHeight(parentDummy.OriginalSize.Height + cell.OriginalSize.Height);
                        this.SetTextHeight(parentDummy);
                    }
                }
                if (!cell.Expanded && !cell.IsLeaf)
                    this.LeafCount--;
            }

            if (cell.HasChilds)
            {
                for (int i = 0; i < cell.Children.Count; i++)
                {
                    this.RecalculateCoordinates(cell.Children[i],
                        new Point(((i == 0) || !cell.Expanded || !cell.IsVisible ?
                        cell.Location.X : cell.Children[i - 1].Location.X + cell.Children[i - 1].Width),
                        cell.Location.Y + cell.Height),
                        depthLevel + 1);
                }

                int childWidth = 0;
                foreach (DimensionCell childCell in cell.Children)
                {
                    childWidth += childCell.IsVisible ? childCell.Size.Width : 0;
                }
                if (childWidth != 0)
                {
                    cell.SetUncheckWidth(childWidth);
                    this.SetTextHeight(cell);
                }
            }
        }

        /// <summary>
        /// Установить высоту ячейки
        /// </summary>
        /// <param name="cell"></param>
        private void SetCellHeight(DimensionCell cell)
        {
            int cellHeight = 0;

            if (cell.IsVisible)
            {
                //if (cell.IsTotal || ((!cell.Expanded || cell.IsLeaf) && !cell.IsExistTotal))
                if (cell.IsTotal || !cell.Expanded || cell.IsLeaf)
                {
                    cellHeight = this.Grid.ColumnCaptions.Bounds.Bottom - cell.Location.Y;
                    cellHeight = this.Grid.GridScale.GetNonScaledValue(cellHeight);
                }
                else
                {
                    if ((cell.DepthLevel > -1) && (base.Grid.ColumnCaptions.Count > cell.DepthLevel))
                        cellHeight = base.Grid.ColumnCaptions[cell.DepthLevel].OriginalSize.Height;
                }
            }
            cell.SetUncheckHeight(cellHeight);
            this.SetTextHeight(cell);
        }

        /// <summary>
        /// Установить высоту текста в ячейке, и расчитать координаты ее свойств
        /// </summary>
        /// <param name="cell"></param>
        private void SetTextHeight(DimensionCell cell)
        {
            if (cell.IsVisible)
            {
                cell.TextHeight = cell.Height - cell.CellProperties_.Height;
                cell.CellProperties_.RecalulateCoordinate();
            }
        }

        /// <summary>
        /// Установить ширину ячейки
        /// </summary>
        /// <param name="cell"></param>
        private void SetCellWidth(DimensionCell cell)
        {
            int cellWidth = 0;

            if (cell.IsVisible)
            {
                cellWidth = cell.MinWidth;
                if ((!cell.HasChilds) || cell.IsTotal || (!cell.Expanded && !cell.IsLeaf))
                {
                    if (base.LeafCount <= base.Grid.MeasureCaptionsSections.Count)
                        cellWidth = base.Grid.MeasureCaptionsSections[base.LeafCount - 1].Width;
                    else
                        cellWidth = 120;
                }
            }
            cell.SetUncheckWidth(cellWidth);
        }
 
        /// <summary>
        /// Инициализация по селсету
        /// </summary>
        /// <param name="cls">селсет</param>
        public override void InitializeMembers(CellSet cls)
        {
            if (cls != null)
            {
                //Сохраним и пересоздадим коллекцию состоятний развернутости элементов
                base.PrepareStateMembersExpand = base.StateMembersExpand;
                //В постраничном режиме отображения, будем хранить сотояния всех страниц
                if (!this.Grid.IsPaddingModeEnabled)
                    base.StateMembersExpand = new MembersExpandDictionary();

                if (cls.OlapInfo.AxesInfo.Axes.Count > 0)
                {
                    PositionCollection poses = cls.Axes[0].Positions;
                    //Common.CommonUtils.WritePoses(poses, "C:\\1.txt");
                    //Узнаем в какой позиция размещен главный итог
                    base.GrandTotalPosition = base.GetGrandTotalPosition(poses);
                    base.InitAxisMembers(poses, 0, poses.Count - 1, 0, base.Root);
                }
            }
        }

        /// <summary>
        /// Видимая область оси
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = base.Bounds;
            result.Height = Math.Min(base.Grid.GridBounds.Bottom - result.Y - 1, result.Height + 1);
            result.Width = Math.Min(base.Grid.GridBounds.Right - result.X - 1, result.Width + 1);
            result.X++;
            return result;
        }

        public override int Width
        {
            get 
            { 
                return base.Root.Width; 
            }
        }

        public override int Height
        {
            get 
            {
                return base.Grid.ColumnCaptions.Height;
            }
        }
    }
}
