using System;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;


namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Ось строк
    /// </summary>
    public class RowAxis : Axis
    {
        //автоматический расчет высоты ячеек у оси строк
        private bool _autoSizeRows = true;
        //минимальная высота ячейки у меры
        private int minMeasureCellHeight;
        //минимальная высота ячейки итога у меры
        private int minMeasureTotalCellHeight;

        public RowAxis(ExpertGrid grid)
            : base(grid, AxisType.Rows)
        {
        }

        /// <summary>
        /// Вычисление абсолютных координат
        /// </summary>
        /// <param name="location">Точка начала</param>
        public override void RecalculateCoordinates(Point location)
        {
            base.Location = location;
            base.LeafCount = 0;
            //минимальная высота ячейки у меры
            this.minMeasureCellHeight = this.Grid.MeasuresData.IsEmpty ? 0 : this.Grid.MeasuresData.Style.TextHeight;
            //минимальная высота ячейки итога у меры
            this.minMeasureTotalCellHeight = this.Grid.MeasuresData.IsEmpty ? 0 : this.Grid.MeasuresData.StyleForTotals.TextHeight;
            
            this.RecalculateCoordinates(base.Root, location, -1);
        }

        protected void RecalculateCoordinates(DimensionCell cell, Point startPoint, int depthLevel)
        {
            if (cell == null)
                return;
            cell.Location = startPoint;
            cell.DepthLevel = depthLevel;
            //если есть родитель, определяем видимость ячейки
            if (cell.Parent != null)
                cell.IsVisible = cell.IsTotal || cell.IsDummy ? cell.Parent.IsVisible : (cell.Parent.IsVisible && cell.Parent.Expanded);
            //инициализируем наличие кнопки
            cell.InitCollapseButton();
            //высота ячейки оси строк, не должна быть меньше высоты ячейки мер
            int minCellHeight = cell.IsTotal ? this.minMeasureTotalCellHeight : this.minMeasureCellHeight;

            //у каждой ячеки, кроме корня вычисляем ширину
            if (!cell.IsRoot)
            {
                this.SetCellWidth(cell);
                //если ячейка фиктивная, рисовать ее не будем, следовательно ее ширину добавляем к родителю 
                //этой фиктивной ячейки
                if (cell.IsDummy)
                {
                    DimensionCell parentDummy = this.GetDummyCellParent(cell);
                    if (parentDummy != null)
                    {
                        parentDummy.SetUncheckWidth(parentDummy.Width + cell.Width);
                        //т.к изменилась ширина ячейки, надо пересчитать ее высоту...
                        this.SetCellHeight(parentDummy, minCellHeight);
                    }
                }
            }

            if (cell.HasChilds)
            {
                for (int i = 0; i < cell.Children.Count; i++)
                {
                    this.RecalculateCoordinates(cell.Children[i],
                        //если ячейка свернута или не видима или высчитываем координаты первого ребенка то, координата Y не изменяется
                        new Point(cell.Location.X + cell.Width, (!cell.Expanded || !cell.IsVisible || (i == 0)) ? cell.Location.Y
                        : cell.Children[i - 1].Location.Y + cell.Children[i - 1].Height),
                        depthLevel + 1);
                }

                #region Здесь проверяем если высота ячейки, больше высоты всех ее детей, то к последней прибавляем недостающую высоту

                int childsHeight = 0;
                foreach (DimensionCell childCell in cell.Children)
                {
                    childsHeight += childCell.IsVisible ? childCell.Size.Height : 0;
                }
                //высота родительского элемента, должена быть не меньше высоты всех его детей
                this.SetCellHeight(cell, childsHeight);

                if ((childsHeight > 0) && (cell.Height > childsHeight))
                {
                    DimensionCell cellLastChild = cell.Children[cell.Children.Count - 1];
                    while (cellLastChild != null)
                    {
                        this.SetCellHeight(cellLastChild, cellLastChild.Height + (cell.Height - childsHeight));
                        if (cellLastChild.HasChilds)
                            cellLastChild = cellLastChild.Children[cellLastChild.Children.Count - 1];
                        else
                            cellLastChild = null;
                    }
                }

                #endregion
            }
            else
            {
                //Вычисляем высоту ячейки
                this.SetCellHeight(cell, minCellHeight);
            }
        }

        /// <summary>
        /// Устанавливаем высоту ячейки, с учетом минимальной высоты
        /// </summary>
        /// <param name="cell"></param>
        private void SetCellHeight(DimensionCell cell, int minCellHeight)
        {
            int cellHeight = 0;
            if (cell.IsVisible)
            {
                int propertiesHeight = cell.CellProperties_.Height;
                cell.TextHeight = (cell.Text != string.Empty) ? CommonUtils.GetStringHeight(base.Grid.GridGraphics, cell.Text, cell.Style.Font, 
                    (cell.ExistCollapseButton ? cell.Width - this.Grid.GridScale.GetScaledValue(20) : cell.Width)) : cell.MinHeight;

                if (this.AutoSizeRows)
                {
                    //если выставлен признак автоматического расчета высоты ячейки, значит ограничений по ее 
                    //высоте - НЕТ
                    cellHeight = Math.Max(cell.TextHeight + propertiesHeight, minCellHeight);
                }
                else
                {
                    //если не выставлено признака автоматического расчета высоты, значит максимальной высотой 
                    //будет либо minHeight, либо минимальная высота строки + высота свойств
                    cellHeight = Math.Max(minCellHeight, cell.MinHeight + propertiesHeight);
                    //здесь приоритет по отображению выше у свойств, поэтому урезаем высоту текста
                    if (cellHeight < cell.TextHeight + propertiesHeight)
                        cell.TextHeight = cellHeight - propertiesHeight;
                }
                cell.CellProperties_.RecalulateCoordinate();
            }
            cell.SetUncheckHeight(this.Grid.GridScale.GetNonScaledValue(cellHeight));
        }

        /// <summary>
        /// Устанавливаем ширину ячейки
        /// </summary>
        /// <param name="cell">Ячейка</param>
        private void SetCellWidth(DimensionCell cell)
        {
            int cellWidth = 0;
            if (cell.IsVisible)
            {
                cellWidth = cell.MinWidth;
                //if (cell.IsTotal || ((!cell.Expanded || cell.IsLeaf) && !cell.IsExistTotal))
                if (cell.IsTotal || !cell.Expanded || cell.IsLeaf)
                {
                    cellWidth = base.Grid.RowCaptions.Bounds.Right - cell.Location.X;
                }
                else
                {
                    if ((cell.DepthLevel > -1) && (base.Grid.RowCaptions.Count > cell.DepthLevel))
                    {
                        cellWidth = base.Grid.RowCaptions[cell.DepthLevel].Width;
                    }
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

                if (cls.OlapInfo.AxesInfo.Axes.Count > 1)
                {
                    PositionCollection poses = cls.Axes[1].Positions;
                    //Common.CommonUtils.WritePoses(poses, "C:\\1.txt");
                    //Узнаем в какой позиция размещен главный итог
                    base.GrandTotalPosition = base.GetGrandTotalPosition(poses);
                    base.InitAxisMembers(poses, 0, poses.Count - 1, 0, base.Root);
                }
            }
        }

        /// <summary>
        /// Получает массив с высотой каждого листового элемента в измерении
        /// </summary>
        /// <returns>int[]</returns>
        public int[] GetRowsHeights()
        {
            List<int> result = new List<int>();
            this.GetRowsHeights(this.Root, ref result);
            return result.ToArray();
        }

        private void GetRowsHeights(DimensionCell cell, ref List<int> result)
        {
            if (cell == null)
                return;
            if (cell.IsLeaf)
            {
                result.Add(cell.Height);
            }
            foreach (DimensionCell childCell in cell.Children)
            {
                this.GetRowsHeights(childCell, ref result);
            }
        }

        /// <summary>
        /// Видимая область оси
        /// </summary>
        /// <returns>Rectangle</returns>
        public override Rectangle GetVisibleBounds()
        {
            Rectangle result = base.Bounds;
            result.Height = Math.Min(base.Grid.GridBounds.Bottom - result.Location.Y - 1, result.Height + 1);
            result.Width = Math.Min(base.Grid.GridBounds.Right - result.Location.X - 1, result.Width + 1);
            result.Y += 1;
            return result;
        }

        /// <summary>
        /// Загрузить настройки оси строк из Xml
        /// </summary>
        public override void Load(XmlNode axisNode, bool isLoadTemplate)
        {
            base.Load(axisNode, isLoadTemplate);
            if (axisNode == null)
                return;

            //загружаем дополнительные свойства
            XmlNode addinPropertys = axisNode.SelectSingleNode(GridConsts.addinPropertys);
            if (addinPropertys != null)
            {
                this.AutoSizeRows = XmlHelper.GetBoolAttrValue(addinPropertys, GridConsts.autoSizeRows, true);
            }
        }

        /// <summary>
        /// Сохранить настройки оси строк в Xml
        /// </summary>
        public override void Save(XmlNode axisNode)
        {
            base.Save(axisNode);
            if (axisNode == null)
                return;

            //сохраняем дополнительные свойства
            XmlNode addinPropertys = XmlHelper.AddChildNode(axisNode, GridConsts.addinPropertys);
            XmlHelper.SetAttribute(addinPropertys, GridConsts.autoSizeRows, this.AutoSizeRows.ToString());
        }

        /// <summary>
        /// Ширина оси
        /// </summary>
        public override int Width
        {
            get 
            { 
                //она равна ширине заголовков показателей
                return base.Grid.RowCaptions.Width; 
            }
        }

        /// <summary>
        /// Высота оси
        /// </summary>
        public override int Height
        {
            get 
            { 
                return base.Root.Height; 
            }
        }

        /// <summary>
        /// Автоматический расчет высоты ячекк
        /// </summary>
        public bool AutoSizeRows
        {
            get
            {
                return this._autoSizeRows;
            }
            set
            {
                this._autoSizeRows = value;
            }
        }

    }
}