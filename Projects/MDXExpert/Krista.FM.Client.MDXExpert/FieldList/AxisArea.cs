using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.MDXExpert.Common;
using Microsoft.AnalysisServices.AdomdClient;
using System.Xml;
using Krista.FM.Client.MDXExpert.Data;
using Axis = Krista.FM.Client.MDXExpert.Data.Axis;

namespace Krista.FM.Client.MDXExpert.FieldList
{
    public partial class AxisArea : UltraTree
    {

        private AxisAreaDrawFilterClass AxisAreaDrawFilter = new AxisAreaDrawFilterClass();
        private AxisType axisType;
        private PivotDataContainer container;
        private string description;

        public AxisType AxisType
        {
            get { return axisType; }
            set { axisType = value; }
        }

        /// <summary>
        /// Описание области
        /// </summary>
        public string Description
        {
            get
            {
                if (this.container is MapPivotDataContainer)
                {
                    //для карты по данным пользователя в областях структуры ничего не выводим
                    MapReportElement mapElement = ((MapPivotDataContainer) container).MapElement;

                    if ((mapElement != null)&& mapElement.IsCustomMap)
                    {
                        return String.Empty;
                    }
                }
                
                return this.description;
            }
        }

        public AxisArea()
        {
            InitializeComponent();

        }

        public AxisArea(IContainer container)
        {
            container.Add(this);
            InitializeComponent();

            AxisAreaDrawFilter.Area = this;
            AxisAreaDrawFilter.Invalidate += new EventHandler(this.AxisAreaDrawFilter_Invalidate);
            AxisAreaDrawFilter.QueryStateAllowedForNode += new AxisAreaDrawFilterClass.QueryStateAllowedForNodeEventHandler(this.AxisAreaDrawFilter_QueryStateAllowedForNode);

            DrawFilter = AxisAreaDrawFilter;
        }

        public void Init(PivotDataContainer container, string description)
        {
            this.container = container;
            this.description = description;
        }

        /// <summary>
        /// Получение ближайшего к курсору узла 
        /// </summary>
        /// <param name="pt">положение курсора</param>
        /// <returns>ближайший к курсору узел</returns>
        private UltraTreeNode GetNearestNode(Point pt)
        {
            Infragistics.Win.UIElement nodeElement = null;
            UltraTreeNode node = TopNode;

            while (node != null)
            {
                nodeElement = node.UIElement;

                if (nodeElement == null)
                    break;

                if (pt.Y >= nodeElement.Rect.Top &&
                    pt.Y <= nodeElement.Rect.Bottom)
                {
                    return node;
                }

                node = node.NextVisibleNode;
            }
            return null;
        }


        /// <summary>
        /// поиск поля в конкретной дроп зоне
        /// </summary>
        /// <param name="uniqueName">юникнейм поля</param>
        /// <returns>найденый узел</returns>
        public UltraTreeNode FindItem(string uniqueName)
        {
            return GetNodeByKey(uniqueName);
        }

        /// <summary>
        /// Добавление поля в дроп зону
        /// </summary>
        /// <param name="node">поле, из которого берутся данные для добавляемого поля</param>
        /// <param name="parentNode">если требуется, сами задаем родительский узел(сейчас нужно для вычисляемых мер)</param>
        /// <returns>добавленное поле</returns>
        public UltraTreeNode AddDropZoneItem(UltraTreeNode node, UltraTreeNode parentNode)
        {
            ItemData iData = FieldListHelper.GetItemData(node);

            UltraTreeNode resultNode = null, fieldNode = null, measureNode = null;

            switch (iData.ItemType)
            {
                case ItemType.ntHierarch:
                    if (FieldListHelper.IsMeasuresNode(node))
                    {
                        resultNode = CopyMeasure(null, node);
                        foreach (UltraTreeNode childNode in node.Nodes)
                        {
                            CopyMeasure(resultNode, childNode);
                        }
                    }
                    else
                    {
                        resultNode = CopyField(null, node);
                        foreach (UltraTreeNode childNode in node.Nodes)
                        {
                            CopyField(resultNode, childNode);
                        }
                    }
                    break;

                case ItemType.ntLevel:
                    resultNode = FindItem(node.Parent.Key) ?? CopyField(null, node.Parent);

                    fieldNode = FindItem(node.Key);
                    if (fieldNode == null)
                    {
                        fieldNode = CopyField(resultNode, node);
                        RepositionField(fieldNode);
                    }
                    break;

                case ItemType.ntMeasure:

                    if (!((PivotTotal)iData.PivotObj).IsCustomTotal)
                    {
                        resultNode = FindItem(node.Parent.Key);
                        if (resultNode == null)
                        {
                            resultNode = CopyMeasure(null, node.Parent);
                        }

                        measureNode = FindItem(node.Key) ?? CopyMeasure(resultNode, node);
                    }
                    else
                    {
                        if (parentNode == null)
                            parentNode = node.Parent;

                        resultNode = FindItem("[Measures]") ?? CopyMeasure(null, parentNode);
                        measureNode = CopyMeasure(resultNode, node);
                        measureNode.Override.NodeAppearance.Image = 24;
                        //resultNode.Nodes.Add(node);
                    }
                    break;
            }

            resultNode.Expanded = true;
            return resultNode;
        }

        /// <summary>
        /// Копирование поля в область
        /// </summary>
        /// <param name="parentNode">поле, к которому прицепляем</param>
        /// <param name="fieldNode">поле, которое переносим</param>
        /// <returns>поле которое добавили</returns>
        private UltraTreeNode CopyField(UltraTreeNode parentNode, UltraTreeNode fieldNode)
        {
            UltraTreeNode resultNode;

            if (parentNode == null)
            {
                //Копируем узел филдсета(иерархии)
                resultNode = Nodes.Add(fieldNode.Key, fieldNode.Text);
                resultNode.LeftImages.Clear();
                resultNode.LeftImages.Add(imageList.Images[22]);
            }
            else
            {
                //копируем узел поля(уровня)
                resultNode = parentNode.Nodes.Add(fieldNode.Key, fieldNode.Text);
                //скрываем уровни у фильтров
                resultNode.Visible = (AxisType != AxisType.atFilters);
            }

            resultNode.Override.NodeAppearance.Image = fieldNode.Override.NodeAppearance.Image;
            resultNode.Tag = fieldNode.Tag;

            if (parentNode != null)
            {
                if (!FieldListHelper.IsMeasuresNode(parentNode))
                {
                    //Задаем мембер пропертис для поля (берем из уровня) 
                    ((PivotField)FieldListHelper.GetItemData(resultNode).PivotObj).MemberProperties.AllProperties.Clear();
                    foreach (LevelProperty prop in ((Level)FieldListHelper.GetItemData(resultNode).AdomdObj).LevelProperties)
                    {
                        PivotField f = ((PivotField) FieldListHelper.GetItemData(resultNode).PivotObj); 
                        f.MemberProperties.AllProperties.Add(prop.Name);
                        if ((container is MapPivotDataContainer)&&(AxisType == AxisType.atRows))
                        {
                            if ((prop.Name == Consts.mapObjectCode) || (prop.Name == Consts.mapObjectName))
                            {
                                if (!f.MemberProperties.VisibleProperties.Contains(prop.Name))
                                    f.MemberProperties.VisibleProperties.Add(prop.Name);
                            }
                        }
                    }
                }
            }

            return resultNode;
        }

        /// <summary>
        /// Копирование меры
        /// </summary>
        /// <param name="parentNode">узел филдсета, к которому прицепляем</param>
        /// <param name="measureNode"></param>
        /// <returns>поле которое добавили</returns>
        private UltraTreeNode CopyMeasure(UltraTreeNode parentNode, UltraTreeNode measureNode)
        {
            UltraTreeNode resultNode;

            if (parentNode == null)
            {
                //Копируем узел филдсета(иерархии)
                resultNode = Nodes.Add(measureNode.Key, measureNode.Text);
                if (AxisType != AxisType.atTotals)
                {
                    resultNode.LeftImages.Clear();
                    resultNode.LeftImages.Add(imageList.Images[22]);
                }
            }
            else
            {
                resultNode = parentNode.Nodes.Add(measureNode.Key, measureNode.Text);
               // if (FieldListHelper.GetItemData(measureNode).ItemType == ItemType.ntMeasure)
                {
                    //Устанавливаем признак для итога - вычислимый или нет
                    PivotTotal total = ((PivotTotal) FieldListHelper.GetItemData(measureNode).PivotObj);
                    
                    if (total.IsCustomTotal)
                    {
                        total.IsCalculate = true;
                    }
                    else
                    {
                        Member mbr = ((Member) FieldListHelper.GetItemData(measureNode).AdomdObj);
                        total.IsCalculate = (mbr.Type == MemberTypeEnum.Formula);
                    }
                }
            }

            resultNode.Override.NodeAppearance.Image = measureNode.Override.NodeAppearance.Image;
            resultNode.Tag = measureNode.Tag;
            return resultNode;
        }

        /// <summary>
        /// Устанавливаем поле в позицию в соответствии с номером уровня
        /// </summary>
        /// <param name="node">поле, которое устнавливаем</param>
        private void RepositionField(UltraTreeNode node)
        {
            ItemData iData = FieldListHelper.GetItemData(node);

            if (iData.ItemType == ItemType.ntLevel)
            {
                Level lev = ((Level)iData.AdomdObj);

                while ((node.GetSibling(NodePosition.Previous) != null) &&
                    (GetLevelNumber(node.GetSibling(NodePosition.Previous)) > lev.LevelNumber))
                {
                    node.Reposition(node.GetSibling(NodePosition.Previous), NodePosition.Previous);
                }
            }
        }

        /// <summary>
        /// Получение номера уровня из поля
        /// </summary>
        /// <param name="node">поле</param>
        /// <returns>-1 - если поле не содержит уровень, иначе номер уровня</returns>
        private int GetLevelNumber(UltraTreeNode node)
        {
            ItemData iData = FieldListHelper.GetItemData(node);
            if (iData.ItemType == ItemType.ntLevel)
            {
                Level lev = ((Level)iData.AdomdObj);
                return lev.LevelNumber;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Показать форму для выбора элементов поля
        /// </summary>
        /// <param name="iData">Данные поля</param>
        private void ShowMemberList(ItemData iData)
        {
            string hierUniqueName = String.Empty;
            Hierarchy h = null;

            switch (iData.ItemType)
            {
                case ItemType.ntHierarch:
                    h = ((Hierarchy)iData.AdomdObj);
                    break;
                case ItemType.ntLevel:
                    h = ((Level)iData.AdomdObj).ParentHierarchy;
                    break;
                default:
                    return;
            }

            if (h != null)
            {
                hierUniqueName = h.UniqueName;
            }

            //проверяем если в pivotData есть этот объект то получаем список объектов стандартным методом и обновляем данные
            if (container.PivotData.ObjectIsPreset(hierUniqueName))
            {
                container.PivotData.ShowMemberList(hierUniqueName);
                //!!RefreshDropZonesView();
            }
            else
            {
                //иначе просто получаем список элементов
                //XmlNode memberNames = ((FieldSet)iData.PivotObj).MemberNames;
                FieldSet fs = (FieldSet) iData.PivotObj;

                container.PivotData.ShowMemberListEx(h, ref fs);
                
                if (fs.MemberNames != null)
                {
                    //((FieldSet)iData.PivotObj).MemberNames = memberNames;
                    container.RefreshPivotData();
                }
            }
        }


        /// <summary>
        /// Для карты в зоне показателей и в зоне серий могут находиться 
        /// либо меры, либо поля, но не то и другое вместе
        /// В связи с этим проводим очистку лишних узлов
        /// </summary>
        /// <param name="node">узел, который добавляем в зону</param>
        private bool ClearMeasures(UltraTreeNode node)
        {
            if (container is MapPivotDataContainer)
            {
                if ((this.AxisType == Data.AxisType.atColumns) || (this.AxisType == Data.AxisType.atTotals))
                {
                    bool newNodeIsMeasure = ((FieldListHelper.GetItemData(node).ItemType == ItemType.ntMeasure) ||
                                             FieldListHelper.IsMeasuresNode(node));
                    
                    List<string> fieldSetNames = new List<string>();
                    

                    for (int i = 0; i < this.Nodes.Count; i++)
                    {
                        if (FieldListHelper.IsMeasuresNode(this.Nodes[i]) != newNodeIsMeasure)
                        {
                            fieldSetNames.Add(this.Nodes[i].Text);
                        }
                    }

                    if (fieldSetNames.Count > 0)
                    {
                        string fieldSetStr = "";
                        if (fieldSetNames.Count == 1)
                        {
                            fieldSetStr = String.Format("Измерение \"{0}\" будет удалено из области \"{1}\". Продолжить?", fieldSetNames[0], this.Parent.Text);
                        }
                        else
                        {
                            foreach (string fsName in fieldSetNames)
                            {
                                if (fieldSetStr != "")
                                {
                                    fieldSetStr += ", ";
                                } 
                                fieldSetStr += "\"" + fsName + "\"";
                            }

                            fieldSetStr = String.Format("Измерения {0} будут удалены из области \"{1}\". Продолжить?", fieldSetStr, this.Parent.Text);
                        }

                        if (MessageBox.Show(fieldSetStr, "MDX Эксперт",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            return false;
                        }
                    }

                    for (int i = 0; i < this.Nodes.Count; i++)
                    {
                        if (FieldListHelper.IsMeasuresNode(this.Nodes[i]) != newNodeIsMeasure)
                        {
                            this.Nodes[i].Remove();
                            i--;
                        }
                    }
                }
                
            }
            return true;
        }

        private void AxisArea_DragDrop(object sender, DragEventArgs e)
        {
            UltraTreeNode dropNode = AxisAreaDrawFilter.DropHightLightNode;
            UltraTreeNode node = ((UltraTreeNode)e.Data.GetData(typeof(UltraTreeNode)));

            bool isMeasures = false;
            string dropNodeKey = string.Empty;

            if (dropNode != null)
            {
                isMeasures = ((FieldListHelper.GetItemData(dropNode).ItemType == ItemType.ntMeasure) &&
                                (FieldListHelper.GetItemData(node).ItemType == ItemType.ntMeasure));

                if (!isMeasures)
                {
                    dropNode = dropNode.RootNode;
                }
                dropNodeKey = dropNode.Key;
            }

            AxisType nodeAxisType = container.PivotData. GetAxisTypeByObjectName(FieldListHelper.GetUniqueNameFromNode(node));
            if ((nodeAxisType == AxisType.atColumns) || (this.AxisType == AxisType.atColumns))
                container.PivotData.TotalAxis.RefreshTypeSort(null);

            if (!container.CanFieldDrop)
            {
                container.RemoveField(node);
                return;
            }

            if (!ClearMeasures(node))
            {
                AxisAreaDrawFilter.ClearDropHighlight();
                return;
            }

            ItemData iData = FieldListHelper.GetItemData(node);

            Axis pAxis = this.container.PivotData.GetAxis(this.AxisType);


            //Устанавливаем видимость итогов при создании по умолчанию
            if (pAxis is PivotAxis)
            {
                ((PivotAxis) pAxis).GrandTotalVisible = !MainForm.IsHideTotalsByDefault;
            }

            if (iData.PivotObj is FieldSet)
            {
                FieldSet fs = (FieldSet) iData.PivotObj;
                fs.IsVisibleTotals = !MainForm.IsHideTotalsByDefault;
            }

            if (iData.PivotObj is PivotField)
            {
                PivotField f = (PivotField)iData.PivotObj;
                f.IsVisibleTotal = !MainForm.IsHideTotalsByDefault;
            }


            UltraTreeNode newNode = null;
            //если просто меняем позицию узла в области, то ничего не создаем и не удаляем
            if (sender != node.Control)
            {
                newNode = AddDropZoneItem(node, null);
                UltraTreeNode oldNode = container.FindAxisAreaItem(FieldListHelper.GetUniqueNameFromNode(newNode));

                if ((oldNode != null) && (newNode != oldNode))
                {
                    oldNode.Control.Nodes.Remove(oldNode);
                }
                container.PivotData.ExcludeOfQuery(iData.PivotObj);
            }
            else
            {
                newNode = node.RootNode;
            }

            if (isMeasures)
            {
                newNode = FindItem(FieldListHelper.GetUniqueNameFromNode(node));
            }

            dropNode = GetNodeByKey(dropNodeKey);

            if (dropNode != null)
            {
                if (newNode != dropNode)
                {
                    switch (AxisAreaDrawFilter.DropLinePosition)
                    {
                        case DropLinePositionEnum.BelowNode:
                            {
                                newNode.Reposition(dropNode, NodePosition.Next);
                                break;
                            }
                        case DropLinePositionEnum.AboveNode:
                            {
                                newNode.Reposition(dropNode, NodePosition.Previous);
                                break;
                            }
                    }

                    container.PivotData.ExcludeOfQuery(iData.PivotObj);
                }
                AxisAreaDrawFilter.ClearDropHighlight();
            }

            ((UltraTree)newNode.Control).Refresh();
            container.RefreshPivotData(iData);
            container.CurrItemData = null;
        }

        private void AxisArea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(UltraTreeNode)))
            {
                UltraTreeNode node = ((UltraTreeNode)e.Data.GetData(typeof(UltraTreeNode)));

                ItemData iData = ((ItemData)node.Tag);


                //задаем условия возможности перетаскивания в зону
                bool CanDrop = false;
                //перетаскиваемый объект - мера или узел с дочерними узлами - мерами
                bool isMeasure = ((iData.ItemType == ItemType.ntMeasure) || (FieldListHelper.IsMeasuresNode(node)));
                //перетаскиваемый объект - не мера
                bool isNoMeasure = ((iData.ItemType != ItemType.ntMeasure) && (!FieldListHelper.IsMeasuresNode(node)));

                if (container is TablePivotDataContainer)
                {
                    //меру можно перетаскивать только на зону итогов
                    bool isMeasure_To_Totals = (isMeasure && (AxisType == AxisType.atTotals));
                    //любой другой объект(не мера) - куда угодно, кроме зоны итогов
                    bool isNoMeasure_To_Any_ExceptTotals = (isNoMeasure && (AxisType != AxisType.atTotals));

                    CanDrop = (isMeasure_To_Totals || isNoMeasure_To_Any_ExceptTotals );
                }

                if (container is MapPivotDataContainer)
                {
                    //меру можно перетаскивать куда угодно, кроме области для объектов
                    bool isMeasure_To_Totals = (isMeasure && (AxisType != AxisType.atRows));

                    CanDrop = (isMeasure_To_Totals || isNoMeasure);
                }


                if ((container is ChartPivotDataContainer) || (container is MultiGaugePivotDataContainer))
                {
                //В случае с контейнером пивот даты для диаграммы, перетаскиваем что угодно, куда угодно,
                //только запрещаем перетаскивание мер в фильтры
                //    CanDrop = ((isMeasure && (AxisType != AxisType.atFilters)) || isNoMeasure);
                    //разрешаем перетаскивание куда угодно
                    CanDrop = true;
                }

                e.Effect = CanDrop ? DragDropEffects.All : DragDropEffects.None;
            }

        }

        private void AxisArea_DragLeave(object sender, EventArgs e)
        {
            AxisAreaDrawFilter.ClearDropHighlight();
        }

        private void AxisArea_DragOver(object sender, DragEventArgs e)
        {
            UltraTreeNode node = ((UltraTreeNode)e.Data.GetData(typeof(UltraTreeNode)));

            UltraTree tree = (UltraTree)sender;
            if (tree.Nodes.Count == 0)
            {
                return;
            }

            if (e.Effect != DragDropEffects.All)
            {
                return;
            }

            Point pointInTree = tree.PointToClient(new Point(e.X, e.Y));

            //узел, на который будем кидать
            UltraTreeNode dropNode = GetNearestNode(pointInTree);

            if (dropNode == null)
            {
                dropNode = tree.Nodes[tree.Nodes.Count - 1];
                pointInTree.X = dropNode.Bounds.Left + 2;
                pointInTree.Y = dropNode.Bounds.Bottom - 2;
            }

            if (dropNode.Parent != null)
            {
                if ((FieldListHelper.GetItemData(node).ItemType != ItemType.ntMeasure)||
                        ((FieldListHelper.GetItemData(node).ItemType == ItemType.ntMeasure) && 
                        (FieldListHelper.GetItemData(dropNode).ItemType != ItemType.ntMeasure)))
                {
                    //карявое какое та условие, надо будет исправить
                    if ((dropNode.Parent.Nodes.Count > 1) || (FieldListHelper.GetItemData(node).ItemType == ItemType.ntMeasure))
                    {
                        dropNode = dropNode.Parent.Nodes[0].GetSibling(NodePosition.Last);
                        if (dropNode != null)
                        {
                            pointInTree.X = dropNode.Bounds.Left + 2;
                            pointInTree.Y = dropNode.Bounds.Bottom - 2;
                        }
                    }
                }

                
            }
            if (dropNode != null)
            {
                AxisAreaDrawFilter.SetDropHighlightNode(dropNode, pointInTree);
            }
        }

        private void AxisArea_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            container.CanFieldDrop = (e.Effect == DragDropEffects.All);
        }

        private void AxisArea_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if ((e.Action == DragAction.Drop) && (!container.CanFieldDrop))
            {
                Refresh();
                container.RemoveField(SelectedNodes[0]);
            }
        }

        private void AxisArea_SelectionDragStart(object sender, EventArgs e)
        {
            container.CanFieldDrop = true;
            UltraTreeNode node = ((UltraTree)sender).SelectedNodes[0];
            ItemType nodeType = FieldListHelper.GetItemData(node).ItemType;

            switch (nodeType)
            {
                case ItemType.ntLevel:
                case ItemType.ntMeasure:
                case ItemType.ntHierarch:
                    ((UltraTree)sender).DoDragDrop(node, DragDropEffects.All);
                    break;
            }
            container.CurrItemData = null;
        }

        private void AxisAreaDrawFilter_QueryStateAllowedForNode(Object sender, AxisAreaDrawFilterClass.QueryStateAllowedForNodeEventArgs e)
        {
            e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode;

            AxisAreaDrawFilter.EdgeSensitivity = e.Node.Bounds.Height / 2;

            if (e.Node.Parent != null)
            {
                e.StatesAllowed = DropLinePositionEnum.AboveNode | DropLinePositionEnum.BelowNode;
                return;
            }

        }

        private void AxisAreaDrawFilter_Invalidate(object sender, EventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Показать фильтры для элементов измерения
        /// </summary>
        /// <param name="iData"></param>
        private void ShowFilters(ItemData iData)
        {
            string hierUniqueName = String.Empty;
            Hierarchy h = null;

            switch (iData.ItemType)
            {
                case ItemType.ntHierarch:
                    h = ((Hierarchy)iData.AdomdObj);
                    break;
                case ItemType.ntLevel:
                    h = ((Level)iData.AdomdObj).ParentHierarchy;
                    break;
                default:
                    return;
            }

            if (h != null)
            {
                hierUniqueName = h.UniqueName;
            }

            /*
            //проверяем если в pivotData есть этот объект то получаем список объектов стандартным методом и обновляем данные
            if (PivotData.ObjectIsPreset(hierUniqueName))
            {
                PivotData.ShowFilters(hierUniqueName);
                RefreshDropZonesView();
            }
            else
            {*/
            //иначе просто получаем список элементов
            FieldSet fieldSet = (FieldSet)iData.PivotObj;

            bool canRefreshData = container.PivotData.ShowFilters(h, ref fieldSet);


            if ((canRefreshData)&&((container != null) && (container.PivotData.GetFieldSet(fieldSet.UniqueName) != null)))
            {
                //container.FieldList.RefreshDropZonesView();
                container.RefreshPivotData();
            }
            // }

        }

        private void AxisArea_MouseClick(object sender, MouseEventArgs e)
        {
            AxisArea area = (AxisArea) sender;

            UltraTreeNode node = area.GetNodeFromPoint(e.X, e.Y);

            if (node != null)
            {
                ItemData iData = FieldListHelper.GetItemData(node);
                if (iData.ItemType == ItemType.ntLevel)
                {
                    iData = null;
                }

                if (iData != null)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        ShowFilters(iData);
                    }
                    else if (((node.Bounds.Left + 32) > e.X) && ((node.Bounds.Left + 16) < e.X))
                    {
                        ShowMemberList(iData);
                    }
                }
            }
        }
    }
}
