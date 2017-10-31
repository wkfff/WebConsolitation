using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Infragistics.Win.UltraWinDock;
using Krista.FM.Client.MDXExpert.CommonClass;

namespace Krista.FM.Client.MDXExpert
{
    public delegate void RepElementObjectBoxEventHandler();

    public partial class RepElementObjectBox : UserControl
    {
        #region поля

        private Object selectedObject;

        private UltraTree objectTree;

        private string currPath = "";

        private DockableControlPane pane;

        private bool isCanLoadObjects = true;


        #endregion 

        #region свойства

        public Object SelectedObject
        {
            get 
            { 
                return selectedObject; 
            }
        }

        #endregion

        #region события

        private RepElementObjectBoxEventHandler selectionChanged = null;
        [Category("Internal events")]
        [Description("Вызывается при выборе объекта в дереве")]
        public event RepElementObjectBoxEventHandler SelectionChanged
        {
            add 
            { 
                selectionChanged += value; 
            }
            remove 
            { 
                selectionChanged -= value; 
            }
        }


        #endregion

        public RepElementObjectBox()
        {
            InitializeComponent();
            AddTree();

        }


        /// <summary>
        /// Добавление выпадающего дерева
        /// </summary>
        private void AddTree()
        {
            objectTree = new UltraTree();
            objectTree.Visible = false;
            objectTree.Height = 200;
            objectTree.ScrollBounds = ScrollBounds.ScrollToFill;
            objectTree.ShowRootLines = false;
            objectTree.BorderStyle = UIElementBorderStyle.WindowsVista;
            objectTree.ImageList = imageList;

            //objectTree.AfterSelect += new AfterNodeSelectEventHandler(this.OnObjectTreeAfterSelect);
            objectTree.MouseDown +=new MouseEventHandler(this.OnObjectTreeMouseDown); 
            objTreePopup.PopupControl = objectTree;
        }


        private void InitEvents()
        {
            CustomReportElement elem = ((CustomReportElement)pane.Control);
            elem.PivotData.AppearanceChanged -= new PivotDataAppChangeEventHandler(OnPivotDataAppearanceChange);
            elem.PivotData.AppearanceChanged += new PivotDataAppChangeEventHandler(OnPivotDataAppearanceChange);
            elem.PivotData.SelectionChanged -= new PivotDataEventHandler(OnPivotDataSelectionChange);
            elem.PivotData.SelectionChanged += new PivotDataEventHandler(OnPivotDataSelectionChange);
            elem.PivotData.DataChanged -= new PivotDataEventHandler(OnPivotDataChange);
            elem.PivotData.DataChanged += new PivotDataEventHandler(OnPivotDataChange);
        }

        public void Init(DockableControlPane dockableControlPane)
        {
            this.pane = dockableControlPane;

            InitEvents();
            
            LoadReportElementObjects();

            SelectObject(currPath);

        }

        public void Clear()
        {
            pane = null;
            objectTree.Nodes.Clear();
            selectedObject = null;
            currPath = "";
            dropDownButton.Text = "";
            dropDownButton.Appearance.Image = null;
        }

        private void SetSelectedNode(UltraTreeNode node)
        {
            dropDownButton.Text = node.Text;
            dropDownButton.Appearance.Image = node.Override.NodeAppearance.Image;

            selectedObject = node.Tag;
            currPath = node.FullPath;

            if (selectionChanged != null)
            {
                selectionChanged();
            }
        }

        #region заполнение выпадающего дерева
        /// <summary>
        /// приклепляем итоги
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        /// <param name="totals">итоги</param>
        private void LoadTotals(UltraTreeNode parentNode, List<PivotTotal> totals)
        {
            UltraTreeNode totalNode;
            foreach (PivotTotal total in totals)
            {
                totalNode = parentNode.Nodes.Add();
                switch (total.PivotDataType)
                {
                    case PivotDataType.Table:
                        totalNode.Tag = new TablePivotTotalBrowseAdapter(total, this.ReportElement);
                        break;
                    case PivotDataType.Chart:
                        totalNode.Tag = new PivotTotalBrowseAdapter(total, this.ReportElement);
                        break;
                    case PivotDataType.Map:
                        totalNode.Tag = new PivotTotalBrowseAdapter(total, this.ReportElement);
                        break;
                }

                totalNode.Text = ((PivotObjectBrowseAdapterBase)totalNode.Tag).Header;

                if (total.IsCustomTotal)
                {
                    totalNode.Override.NodeAppearance.Image = 11;
                }
                else
                {
                    totalNode.Override.NodeAppearance.Image = total.IsCalculate ? 9 : 6;
                }

                CheckSelection(totalNode, SelectionType.SingleObject, total.UniqueName);
            }
        }

        /// <summary>
        /// прикрепляем иерархии
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        /// <param name="fieldSets">набор иерархий</param>
        private void LoadFieldsets(UltraTreeNode parentNode, FieldSetCollection fieldSets)
        {
            UltraTreeNode fsNode;

            foreach (FieldSet fs in fieldSets)
            {
                fsNode = parentNode.Nodes.Add();

                if (fs.UniqueName == "[Measures]")
                {
                    fsNode.Tag = new FieldSetBrowseAdapter(fs, this.ReportElement);
                    fsNode.Text = "Меры";
                    fsNode.Override.NodeAppearance.Image = 5;
                    LoadTotals(fsNode, fs.ParentPivotData.TotalAxis.Totals);
                }
                else
                {
                    fsNode.Tag = new FieldSetBrowseAdapter(fs, this.ReportElement);
                    fsNode.Text = ((PivotObjectBrowseAdapterBase)fsNode.Tag).Header;
                    fsNode.Override.NodeAppearance.Image = 7;
                    LoadFields(fsNode, fs.Fields);
                }
                CheckSelection(fsNode, SelectionType.SingleObject, fs.UniqueName);
            }
        }

        /// <summary>
        /// прикрепляем поля
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        /// <param name="fields">поля</param>
        private void LoadFields(UltraTreeNode parentNode, List<PivotField> fields)
        {
            UltraTreeNode fieldNode;

            foreach (PivotField f in fields)
            {
                fieldNode = parentNode.Nodes.Add();
                switch (f.PivotDataType)
                {
                    case PivotDataType.Table:
                        fieldNode.Tag = new TablePivotFieldBrowseAdapter(f, this.ReportElement);
                        break;
                    case PivotDataType.Chart:
                        fieldNode.Tag = new ChartPivotFieldBrowseAdapter(f, this.ReportElement);
                        break;
                    case PivotDataType.Map:
                        fieldNode.Tag = new MapPivotFieldBrowseAdapter(f, this.ReportElement);
                        break;
                }
                fieldNode.Text = ((PivotObjectBrowseAdapterBase)fieldNode.Tag).Header;
                fieldNode.Override.NodeAppearance.Image = 8;

                CheckSelection(fieldNode, SelectionType.SingleObject, f.UniqueName);
            }
        }

        /// <summary>
        /// прикрепляем ось
        /// </summary>
        /// <param name="axes">ось</param>
        private void LoadAxes(UltraTreeNode parentNode, List<Axis> axes)
        {
            UltraTreeNode axisNode;

            foreach (Axis axis in axes)
            {
                if (axis.AxisType == AxisType.atTotals)
                {
                    if (axis.PivotDataType == PivotDataType.Chart)
                        continue;

                    if ((((TotalAxis)axis).Totals.Count > 0) || (axis.FieldSets.Count > 0))
                    {
                        axisNode = parentNode.Nodes.Add();
                        if (axis.PivotDataType == PivotDataType.Table)
                        {
                            axisNode.Tag = new TableTotalAxisBrowseAdapter((TotalAxis)axis, this.ReportElement);
                        }
                        else
                        {
                            axisNode.Tag = new TotalAxisBrowseAdapter((TotalAxis)axis, this.ReportElement);
                        }
                        axisNode.Text = ((PivotObjectBrowseAdapterBase)axisNode.Tag).Header;
                        axisNode.Override.NodeAppearance.Image = 5;

                        if (axis.PivotDataType == PivotDataType.Table)
                        {
                            LoadTotals(axisNode, ((TotalAxis) axis).Totals);
                        }

                        if (axis.PivotDataType == PivotDataType.Map)
                        {
                            LoadFieldsets(axisNode, axis.FieldSets);
                        }

                        CheckSelection(axisNode, SelectionType.Totals, "");
                    }
                }
                else
                {
                    if (axis.FieldSets.Count > 0)
                    {
                        axisNode = parentNode.Nodes.Add();
                        switch (axis.PivotDataType)
                        {
                            case PivotDataType.Table:
                                axisNode.Tag = new TablePivotAxisBrowseAdapter((PivotAxis)axis, this.ReportElement);
                                break;
                            case PivotDataType.Chart:
                                axisNode.Tag = new ChartPivotAxisBrowseAdapter((PivotAxis)axis, this.ReportElement);
                                break;
                            case PivotDataType.Map:
                                axisNode.Tag = new MapPivotAxisBrowseAdapter((PivotAxis)axis, this.ReportElement);
                                break;
                        }
                       // axisNode.Tag = new PivotAxisBrowseAdapter((PivotAxis)axis, this.GetGridUserInterface());
                        axisNode.Text = ((PivotObjectBrowseAdapterBase)axisNode.Tag).Header;
                        switch (axis.AxisType)
                        {
                            case AxisType.atColumns:
                                axisNode.Override.NodeAppearance.Image = 2;
                                CheckSelection(axisNode, SelectionType.Columns, "");
                                break;
                            case AxisType.atRows:
                                axisNode.Override.NodeAppearance.Image = 3;
                                CheckSelection(axisNode, SelectionType.Rows, "");
                                break;
                            case AxisType.atFilters:
                                axisNode.Override.NodeAppearance.Image = 4;
                                CheckSelection(axisNode, SelectionType.Filters, "");
                                break;
                        }
                        LoadFieldsets(axisNode, axis.FieldSets);
                    }
                }
            }
        }

        private CustomReportElement ReportElement
        {
            get
            {
                return (pane != null) ? (CustomReportElement)pane.Control : null;
            }
        }

        private void AddRoot()
        {
            UltraTreeNode root = null;
            if (pane == null)
            {
                return;
            }
            CustomReportElement reportElement = (CustomReportElement)pane.Control;

            switch (reportElement.ElementType)
            {
                case ReportElementType.eChart:
                    root = objectTree.Nodes.Add();
                    root.Tag = new ChartReportElementBrowseAdapter(this.pane);
                    root.Text = "Диаграмма";
                    root.Override.NodeAppearance.Image = 1;
                    break;
                case ReportElementType.eMultiGauge:
                    root = objectTree.Nodes.Add();
                    root.Tag = new MultiGaugeReportElementBrowseAdapter(this.pane);
                    root.Text = "Множественный индикатор";
                    root.Override.NodeAppearance.Image = 12;
                    break;
                case ReportElementType.eTable:
                    root = objectTree.Nodes.Add();
                    root.Tag = new TableReportElementBrowseAdapter(this.pane);
                    root.Text = "Таблица";
                    root.Override.NodeAppearance.Image = 0;
                    break;
                case ReportElementType.eMap:
                    root = objectTree.Nodes.Add();
                    root.Tag = new MapReportElementBrowseAdapter(this.pane);
                    root.Text = "Карта";
                    root.Override.NodeAppearance.Image = 10;
                    break;
                case ReportElementType.eGauge:
                    root = objectTree.Nodes.Add();
                    root.Tag = new GaugeReportElementBrowseAdapter(this.pane);
                    root.Text = "Индикатор";
                    root.Override.NodeAppearance.Image = 12;
                    break;


            }
            CheckSelection(root, SelectionType.GeneralArea, "");

            LoadAxes(root, reportElement.PivotData.Axes);
        }

        private void LoadReportElementObjects()
        {
            objectTree.Nodes.Clear();
            AddRoot();
            if (objectTree.Nodes.Count > 0)
            {
                objectTree.ExpandAll(); // Nodes[0].Expanded = true;
            }

        }

        /// <summary>
        /// Выделение заданного узла, если он соответствует выбранному объекту в pivotData
        /// </summary>
        /// <param name="node">узел, который проверяем</param>
        /// <param name="selectionType">тип выбранного объекта</param>
        /// <param name="selectionName">уникальное имя объекта</param>
        private void CheckSelection(UltraTreeNode node, SelectionType selectionType, string selectionName)
        {
            CustomReportElement elem = ((CustomReportElement)pane.Control);

            if ((selectionType == elem.PivotData.SelectionType) && (selectionName == elem.PivotData.Selection))
            {
                SetSelectedNode(node);
            }
        }

        #endregion

        #region обработчики
        /*
        private void OnObjectTreeAfterSelect(object sender, SelectEventArgs e)
        {
            //if (objectTree.SelectedNodes.Count > 0)
            
            UltraTreeNode selectedNode = null;}
            if (e.NewSelections.Count > 0)
            {
                selectedNode = e.NewSelections[0];
                if (selectedNode != null)
                {
                    if (dropDownButton != null)
                    {
                        dropDownButton.Text = selectedNode.Text;
                        dropDownButton.CloseUp();
                    }
                    else
                    {
                        dropDownButton.Text = selectedNode.Text;
                    }

                    SetSelectedNode(selectedNode);

                    SetPivotDataSelObject(selectedNode.Tag);
                }
            }
            
        }
        */

        private void OnObjectTreeMouseDown(object sender, MouseEventArgs e)
        {
            UltraTreeNode selectedNode = ((UltraTree)sender).GetNodeFromPoint(e.Location);
            if (selectedNode != null)
            {
                if (dropDownButton != null)
                {
                    dropDownButton.Text = selectedNode.Text;
                    dropDownButton.CloseUp();
                }
                else
                {
                    dropDownButton.Text = selectedNode.Text;
                }

                SetSelectedNode(selectedNode);

                SetPivotDataSelObject(selectedNode.Tag);
            }
        }


        private SelectionType GetSelTypeFromAxisBrowseAdapter(PivotAxisBrowseAdapter adapter)
        {
            switch (adapter.CurrentAxisType)
            {
                case AxisType.atColumns:
                    return SelectionType.Columns;
                case AxisType.atRows:
                    return SelectionType.Rows;
                case AxisType.atFilters:
                    return SelectionType.Filters;
            }
            return SelectionType.GeneralArea;
        }

        private void SetPivotDataSelObject(Object selectedObject)
        {

            SelectionType selType = SelectionType.GeneralArea;
            string selName = "";

            switch (selectedObject.GetType().Name)
            {
                case "ChartReportElementBrowseAdapter":
                case "MultiGaugeReportElementBrowseAdapter":
                case "TableReportElementBrowseAdapter":
                case "MapReportElementBrowseAdapter":
                    selType = SelectionType.GeneralArea;
                    break;
                case "ChartPivotAxisBrowseAdapter":
                case "TablePivotAxisBrowseAdapter":
                case "MapPivotAxisBrowseAdapter":
                    selType = GetSelTypeFromAxisBrowseAdapter((PivotAxisBrowseAdapter)selectedObject);
                    break;
                case "TotalAxisBrowseAdapter":
                case "TableAxisBrowseAdapter":
                    selType = SelectionType.Totals;
                    break;
                case "ChartPivotFieldBrowseAdapter":
                case "TablePivotFieldBrowseAdapter":
                case "MapPivotFieldBrowseAdapter":
                    selName = ((PivotFieldBrowseAdapter)selectedObject).UniqueName;
                    selType = SelectionType.SingleObject;
                    break;
                case "FieldSetBrowseAdapter":
                    selName = ((FieldSetBrowseAdapter)selectedObject).UniqueName;
                    selType = SelectionType.SingleObject;
                    break;
                case "PivotTotalBrowseAdapter":
                case "TablePivotTotalBrowseAdapter":
                case "MapPivotTotalBrowseAdapter":
                    selName = ((PivotTotalBrowseAdapter)selectedObject).UniqueName;
                    selType = SelectionType.SingleObject;
                    break;
            }

            isCanLoadObjects = false;
            CustomReportElement elem = ((CustomReportElement)pane.Control);
            elem.PivotData.SetSelection(selType, selName);
            isCanLoadObjects = true;
        }

        private void OnPivotDataChange()
        {
            LoadReportElementObjects();
            
            if (objectTree.SelectedNodes.Count > 0)
            {
                objectTree.SelectedNodes[0].Selected = false;
            }
            
            SelectObject(currPath);
        }

        private void OnPivotDataAppearanceChange(bool isNeedRecalculateGrid)
        {
            if (SelectedObject != null)
            {
                if (SelectedObject is PivotObjectBrowseAdapterBase)
                {
                    dropDownButton.Text = ((PivotObjectBrowseAdapterBase)SelectedObject).Header;
                }
            }
        }

        private void OnPivotDataSelectionChange()
        {
            if (isCanLoadObjects)
            {
                LoadReportElementObjects();
            }
        }


        #endregion

        #region Выделение текущего объекта в дереве

        private void SelectObject(string path)
        {
            UltraTreeNode node = this.FindClosestNode(path);

            if (node != null)
            {
                objectTree.SelectedNodes.Clear();
                objectTree.ActiveNode = node;
                node.BringIntoView(false);
            }
            else
            {
                if (objectTree.Nodes.Count > 0)
                {
                    node = objectTree.Nodes[0];
                }
            }

            if (node != null)
            {
                SetSelectedNode(node);
                currPath = node.FullPath;
            }
        }

        private UltraTreeNode FindClosestNode(string path)
        {
            if (objectTree.Nodes.Count == 0)
                return null;

            if (path == null || path.Length == 0)
            {
                if (objectTree.Nodes[0].Visible)
                    return objectTree.Nodes[0];

                return objectTree.Nodes[0].NextVisibleNode;
            }

            string[] dirs = path.Split('\\');

            UltraTreeNode closestNode = FindChild(objectTree.Nodes, dirs[0]);

            if (closestNode == null)
                return null;

            for (int i = 1; i < dirs.Length; i++)
            {
                UltraTreeNode node = FindChild(closestNode, dirs[i]);

                if (node == null)
                    return closestNode;
                else
                    closestNode = node;
            }

            return closestNode;
        }

        private UltraTreeNode FindChild(UltraTreeNode node, string directory)
        {
            if (node == null)
                return null;

            bool expanded = false;

            if (node.HasExpansionIndicator && !node.HasNodes)
            {
                node.Expanded = true;
                expanded = true;
            }

            UltraTreeNode child = FindChild(node.Nodes, directory);

            if (child == null && expanded)
                node.Expanded = false;

            return child;
        }

        private UltraTreeNode FindChild(TreeNodesCollection nodes, string directory)
        {
            if (nodes == null || nodes.Count == 0)
                return null;

            foreach (UltraTreeNode node in nodes)
            {
                if (node.Text == directory)
                    return node;
            }

            return null;
        }

        #endregion 

        private void dropDownButton_DroppingDown(object sender, CancelEventArgs e)
        {
            objectTree.Width = dropDownButton.Width;
            OnPivotDataChange();
        }

    }
}
