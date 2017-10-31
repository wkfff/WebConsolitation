using System;
using System.IO;
using System.Threading;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.Win.UltraWinTree;

using Krista.FM.Common.Xml;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Controls;


namespace Krista.FM.Client.MDXExpert.FieldList
{   
    //редактор полей
    public partial class FieldListEditor : UserControl
    {
        private PivotDataContainer pdContainer = null;
        private PivotData _pivotData;
        // редактор композитных диаграмм
        private CompositeChartEditor _compositeChartEditor;
        private CustomReportElement reportElement;
        private bool isCompositeChart = false;

        #region свойства

        /// <summary>
        /// Контейнер с областями структуры
        /// </summary>
        public PivotDataContainer PivotDataContainer
        {
            get { return this.pdContainer; }
        }

        public PivotData PivotData
        {
            get { return _pivotData; }
            set
            {
                _pivotData = value;
                
                if (value != null)
                {
                    LoadCube(_pivotData.Cube);
                }
                RefreshDropZonesView();
            }
        }

        public CompositeChartEditor CompositeChartEditor
        {
            get { return _compositeChartEditor; }
            set { _compositeChartEditor = value; }
        }

        #endregion

        public FieldListEditor()
        {
            InitializeComponent();
            
            //добавляем редактор композитных диаграмм
            this.CompositeChartEditor = new CompositeChartEditor();
            this.fieldListContainer.Controls.Add(this.CompositeChartEditor);
        }

        private void CreatePivotDataContainer(ReportElementType elemType, PivotData pivotData)
        {
            switch (elemType)
            {
                case ReportElementType.eTable:
                    if (pdContainer != null)
                    {
                        if (pdContainer is TablePivotDataContainer)
                        {
                            pdContainer.PivotData = pivotData;
                            return;
                        }
                        else
                        {
                            pdContainer.Dispose();
                        }
                    }
                    pdContainer = new TablePivotDataContainer(fieldsContainerPanel, pivotData);
                    fieldsContainerPanel.Height = parentPanel.Height / 3;
                    break;

                case ReportElementType.eChart:
                    if (pdContainer != null)
                    {
                        if (pdContainer is ChartPivotDataContainer)
                        {
                            pdContainer.PivotData = pivotData;
                            return;
                        }
                        else
                        {
                            pdContainer.Dispose();
                        }
                    }

                    pdContainer = new ChartPivotDataContainer(fieldsContainerPanel, pivotData);
                    if (isCompositeChart)
                    {
                        fieldsContainerPanel.Height = parentPanel.Height;
                    }
                    else
                    {
                        fieldsContainerPanel.Height = parentPanel.Height / 3;
                    }
                    break;

                case ReportElementType.eMultiGauge:
                    if (pdContainer != null)
                    {
                        if (pdContainer is MultiGaugePivotDataContainer)
                        {
                            pdContainer.PivotData = pivotData;
                            return;
                        }
                        else
                        {
                            pdContainer.Dispose();
                        }
                    }

                    pdContainer = new MultiGaugePivotDataContainer(fieldsContainerPanel, pivotData);
                    fieldsContainerPanel.Height = parentPanel.Height / 3;

                    break;

                case ReportElementType.eMap:
                    if (pdContainer != null)
                    {
                        if (pdContainer is MapPivotDataContainer)
                        {
                            pdContainer.PivotData = pivotData;
                            return;
                        }
                        else
                        {
                            pdContainer.Dispose();
                        }
                    }
                    MapReportElement mapElement = (this.reportElement is MapReportElement) ? (MapReportElement)this.reportElement : null;
                    pdContainer = new MapPivotDataContainer(fieldsContainerPanel, pivotData, mapElement);
                    fieldsContainerPanel.Height = parentPanel.Height / 3;
                    break;
            }

            if (pdContainer != null)
            {
                pdContainer.AdjustColors(this.BackColor, tvFields.Appearance.BorderColor, splitter.BackColor);
                pdContainer.FieldList = this;
            }
            fieldsContainerPanel.Height += 1;
            fieldsContainerPanel.Height -= 1;
        }

        #region инициализация редактора

        /// <summary>
        /// Инициализация редактора. 
        /// </summary>
        /// <param name="reportElement">элемент, для которого инициализируется редактор</param>
        public void InitEditor(CustomReportElement reportElement)
        {
            this.reportElement = reportElement;
            isCompositeChart = (reportElement is ChartReportElement &&
                                ((ChartReportElement) reportElement).IsCompositeChart);
            bool isGaugeElement = reportElement is GaugeReportElement;

            if (isGaugeElement)
            {
                this.utStructure.Visible = false;
                this.CompositeChartEditor.Visible = false;
                fieldsContainerPanel.Visible = false;
                cbDeferUpdating.Visible = false;
                btUpdate.Visible = false;
                Invalidate(true);
            }
            else
            if (isCompositeChart)
            {
                this.utStructure.Visible = false;
                this.CompositeChartEditor.Visible = true;
                this.CompositeChartEditor.Dock = DockStyle.Fill;
                fieldsContainerPanel.Visible = false;
                cbDeferUpdating.Visible = false;
                btUpdate.Visible = false;

                this.CompositeChartEditor.ReportElement = (ChartReportElement)reportElement;
                this.CompositeChartEditor.RefreshEditor();
            }
            else
            {
                this.utStructure.Visible = true;
                this.utpStructure.Tab.Visible = !reportElement.PivotData.IsCustomMDX;
                this.CompositeChartEditor.Visible = false;
                fieldsContainerPanel.Visible = true;
                cbDeferUpdating.Visible = true;
                btUpdate.Visible = true;
                
                CreatePivotDataContainer(reportElement.ElementType, reportElement.PivotData);
                this.PivotData = reportElement.PivotData;
                InitEvents();

                this.customMDXEditor.RefreshEditor(this.reportElement.PivotData);
                cbDeferUpdating.Checked = this.PivotData.IsDeferDataUpdating;
                if (this.pdContainer != null)
                    this.pdContainer.SetEnabledGroup(reportElement);
                tvFields.Enabled = true;

                if (this.reportElement is ChartReportElement)
                    tvFields.Enabled = String.IsNullOrEmpty(((ChartReportElement)this.reportElement).Synchronization.BoundTo);

                if (this.reportElement is MapReportElement)
                    tvFields.Enabled = String.IsNullOrEmpty(((MapReportElement)this.reportElement).Synchronization.BoundTo);

                if (this.reportElement is MultipleGaugeReportElement)
                    tvFields.Enabled = String.IsNullOrEmpty(((MultipleGaugeReportElement)this.reportElement).Synchronization.BoundTo);

                Invalidate(true);
            }
        }

        /// <summary>
        /// Проверка различаются ли кубы в структуре и в отображаемом дереве (в списке полей)
        /// </summary>
        /// <returns></returns>
        private bool IsCubeChanged()
        {
            if (tvFields.Nodes.Count > 0)
            {
                UltraTreeNode cubeNode = tvFields.Nodes[0];
                ItemData iData = FieldListHelper.GetItemData(cubeNode);
                if (iData.ItemType == ItemType.ntCube)
                {
                    if (iData.AdomdObj != null)
                        if (_pivotData.Cube.Name == ((CubeDef)iData.AdomdObj).Name)
                        {
                            return false;
                        }
                }

            }
            return true;
        }

        //обновление объектов в узлах общего списка полей и в дроп зонах
        private void RefreshFields()
        {
            if (_pivotData != null)
            {
                //Куб мог измениться, перезагрузим его
                if (IsCubeChanged())
                    LoadCube(_pivotData.Cube);

                RefreshAdomdObjects(tvFields.Nodes.Count > 0 ? tvFields.Nodes[0] : null);
            }
            RefreshDropZonesView();
        }

        private void RefreshAdomdObjects(UltraTreeNode node)
        {
            CubeDef cube = this.PivotData.Cube;
            if (cube == null)
            {
                return;
            }

            Dimension dim;
            Hierarchy hierch;
            Level level;

            while(node != null)
            {
                ItemData iData = FieldListHelper.GetItemData(node);

                switch (iData.ItemType) 
                {
                    case ItemType.ntCube:
                        iData.AdomdObj = cube;
                        break;
                    case ItemType.ntDim:
                        iData.AdomdObj = cube.Dimensions[((Dimension)iData.AdomdObj).Name];
                        break;
                    case ItemType.ntHierarch:
                        dim = cube.Dimensions[((Hierarchy)iData.AdomdObj).ParentDimension.Name];
                        iData.AdomdObj = dim.Hierarchies[((Hierarchy)iData.AdomdObj).Name];
                        break;
                    case ItemType.ntLevel:
                        dim = cube.Dimensions[((Level)iData.AdomdObj).ParentHierarchy.ParentDimension.Name];
                        hierch = dim.Hierarchies[((Level)iData.AdomdObj).ParentHierarchy.Name];
                        iData.AdomdObj = hierch.Levels[((Level)iData.AdomdObj).Name];
                        break;
                    case ItemType.ntMember:
                        dim = cube.Dimensions[((Member)iData.AdomdObj).ParentLevel.ParentHierarchy.ParentDimension.Name];
                        hierch = dim.Hierarchies[((Member)iData.AdomdObj).ParentLevel.ParentHierarchy.Name];
                        level = hierch.Levels[((Member)iData.AdomdObj).ParentLevel.Name];
                        iData.AdomdObj = level.GetMembers().Find(((Member)iData.AdomdObj).Name);
                        break;
                }

                if (node.Nodes.Count > 0)
                    RefreshAdomdObjects(node.Nodes[0]);
                node = node.GetSibling(NodePosition.Next);
            }
        }

        #endregion

        #region методы для поиска полей


        private void InitEvents()
        {
            this.PivotData.ViewChanged -= new PivotDataEventHandler(OnPivotDataViewChange);
            this.PivotData.ViewChanged += new PivotDataEventHandler(OnPivotDataViewChange);
            this.PivotData.DataChanged -= new PivotDataEventHandler(OnPivotDataViewChange);
            this.PivotData.DataChanged += new PivotDataEventHandler(OnPivotDataViewChange);
            this.PivotData.AdomdExceptionReceived -= new PivotDataAdomdExceptionEventHandler(OnAdomdExceptionReceived);
            this.PivotData.AdomdExceptionReceived += new PivotDataAdomdExceptionEventHandler(OnAdomdExceptionReceived);
        }

        /// <summary>
        /// Поиск узла в общем дереве полей
        /// </summary>
        /// <param name="uName">юникнейм поля</param>
        /// <returns>найденый узел</returns>
        private UltraTreeNode FindNodeByUniqueName(string uName)
        {
            return tvFields.GetNodeByKey(uName);
        }

        #endregion

        #region методы для выделения узлов

        private void ClearNodeBoldWithChilds(UltraTreeNode root)
        {
            while (root != null)
            {
                SetNodeBold(root, false);
                if (root.HasNodes)
                {
                    ClearNodeBoldWithChilds(root.Nodes[0]);
                }
                root = root.GetSibling(NodePosition.Next);
            }
        }

        private void ClearNodesColor(UltraTreeNode root)
        {
            while (root != null)
            {
                SetNodeColor(root, Color.Black);
                if (root.HasNodes)
                {
                    ClearNodesColor(root.Nodes[0]);
                }
                root = root.GetSibling(NodePosition.Next);
            }
        }

        private void RefreshNodesColor(UltraTreeNode root)
        {
            while (root != null)
            {
                ItemData iData = FieldListHelper.GetItemData(root);
                SetNodeColor(root, Color.Black);
                if (iData.ItemType == ItemType.ntHierarch)
                {
                    FieldSet fs = (FieldSet) iData.PivotObj;
                    if (fs.MemberFilters.Count > 0)
                        SetNodeColor(root, Color.Blue);
                }
                /*
                foreach(UltraTreeNode node in root.Nodes)
                {
                    RefreshNodesColor(node);
                }*/
                root = root.GetSibling(NodePosition.Next);
            }
        }

        private void SetNodeBold(UltraTreeNode node, bool isBold)
        {
            if (node == null)
                return;

            if (isBold)
            {
                node.Override.NodeAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            }
            else
            {
                node.Override.NodeAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
            }
            node.Text = node.Text;
        }

        private void SetNodeColor(UltraTreeNode node, Color color)
        {
            if (node == null)
                return;

            node.Override.NodeAppearance.ForeColor = color;
        }


        private void SetNodeBold(string uName, bool isBold)
        {
            UltraTreeNode node = FindNodeByUniqueName(uName);
            if (node != null)
            {
                SetNodeBold(node, isBold);
            }
        }

        /// <summary>
        /// очистка выделения узла и его дочерних узлов 
        /// </summary>
        /// <param name="node">узел</param>
        private void ClearNodeBold(UltraTreeNode node)
        {
            while (node != null)
            {
                SetNodeBold(node, false);
                if (node.Nodes.Count > 0)
                {
                    ClearNodeBold(node.Nodes[0]);
                }
                node = node.GetSibling(NodePosition.Next); 
            }
        }

        #endregion

        #region Обновление вида дроп зон
        
        /// <summary>
        /// Обновление вида дроп зоны
        /// </summary>
        /// <param name="axis">ось, соответствующая этой дроп зоне</param>
        private void SetDropZoneView(Data.Axis axis)
        {
            AxisArea area = null;
            if (pdContainer != null)
            {
                area = pdContainer.GetAreaByAxisType(axis.AxisType);
            }

            if (area == null)
            {
                return;
            }

            UltraTreeNode node;
            ItemData itemData;
            string fieldName;

            area.Nodes.Clear();
            if (this.PivotData.Cube == null)
                return;

            foreach (FieldSet fieldSet in axis.FieldSets)
            {
                node = FindNodeByUniqueName(fieldSet.UniqueName);
                if (node == null)
                {
                    continue;
                }

                SetNodeBold(node, true);

                if (FieldListHelper.GetItemData(node.Parent).ItemType == ItemType.ntDim)
                {
                    node.Parent.Expanded = true;
                }

                itemData = FieldListHelper.GetItemData(node); 

                itemData.PivotObj = fieldSet;

                if (FieldListHelper.IsMeasuresNode(node))
                {
                    string uName = "";
                    foreach (XmlNode totalNode in fieldSet.MemberNames.ChildNodes)
                    {
                        uName = XmlHelper.GetStringAttrValue(totalNode, "uname", string.Empty);
                        UltraTreeNode measureNode = FindNodeByUniqueName(uName);
                        if (measureNode != null)
                        {
                            SetNodeBold(measureNode, true);
                            area.AddDropZoneItem(measureNode, null);
                        }
                        else //если не нашли в дереве, значит это пользовательская мера
                        {
                            string name = this.ExtractName(uName);
                            if (name.Trim() == string.Empty)
                                name = "Пустая мера";
                            measureNode = new UltraTreeNode(uName, name);

                            PivotTotal pTotal = this.PivotData.TotalAxis.GetTotalByName(uName);
                            if (pTotal == null)
                                pTotal = new PivotTotal(fieldSet.ParentPivotData, uName, name, true, string.Empty,
                                                        string.Empty, MeasureFormulaType.Custom);
                            measureNode.Tag = new ItemData(ItemType.ntMeasure, itemData.AdomdObj, pTotal);
                            area.AddDropZoneItem(measureNode, node);
                        }
                    }
                }
                else
                {
                    foreach (PivotField f in fieldSet.Fields)
                    {
                        fieldName = f.UniqueName;
                        node = FindNodeByUniqueName(fieldName);
                        if (node != null)
                        {
                            node.Parent.Expanded = true;
                        }

                        itemData = FieldListHelper.GetItemData(node);
                        itemData.PivotObj = f;

                        SetNodeBold(node, true);

                        area.AddDropZoneItem(node, null);
                    }
                }
                
            }
        }

        private string ExtractName(string uniqueName)
        {
            string result = uniqueName;
            try
            {
                string[] parts = uniqueName.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                result = parts[parts.Length - 1];
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Обновляем вид итогов
        /// </summary>
        private void RefreshTotalsView()
        {
            UltraTreeNode node;
            AxisArea area = null;
            if (pdContainer != null)
            {
                area = pdContainer.GetAreaByAxisType(AxisType.atTotals);
            }

            if (area == null)
            {
                return;
            }

            area.Nodes.Clear();
            if (this.PivotData.Cube == null)
                return;

            foreach (PivotTotal total in PivotData.TotalAxis.Totals)
            {
                node = FindNodeByUniqueName(total.UniqueName);
                if (total.IsCustomTotal || (node == null))
                {
                    total.IsCustomTotal = true;
                    node = new UltraTreeNode(total.UniqueName, total.Caption);
                    node.Tag = new ItemData(ItemType.ntMeasure, null, total);
                    if (tvFields.Nodes.Count > 0)
                        area.AddDropZoneItem(node, FieldListHelper.GetMeasuresNode(tvFields.Nodes[0]));
                }
                else
                {
                    FieldListHelper.GetItemData(node).PivotObj = total;
                    SetNodeBold(node, true);
                    area.AddDropZoneItem(node, null);
                }
            }
        }

        /// <summary>
        /// Обновление вида всех зон
        /// </summary>
        public void RefreshDropZonesView()
        {
            if (PivotData == null)
            {
                ClearEditor();
                return;
            }

            if (tvFields.Nodes.Count > 0)
            {
                ClearNodeBoldWithChilds(tvFields.Nodes[0]);
                //RefreshNodesColor(tvFields.Nodes[0]);
            }

            foreach (Data.Axis axis in PivotData.Axes)
            {
                if ((axis.AxisType == AxisType.atTotals)&&(this.reportElement.ElementType == ReportElementType.eTable))
                {
                    RefreshTotalsView();
                }
                else
                {
                    SetDropZoneView(axis);
                }
                 
            }
        }

        #endregion

        #region Очистка редактора

        /// <summary>
        /// Очистка всего редактора
        /// </summary>
        public void ClearEditor()
        {
            tvFields.Nodes.Clear();
            if (pdContainer != null)
            {
                pdContainer.ClearAreas();
                //pdContainer.ClearCaptions();
            }
            this.CompositeChartEditor.ResetEditor();
            this.customMDXEditor.ResetEditor();
        }

        #endregion

        #region загрузка общего дерева полей

        private void InitMemberNames(Hierarchy h, ref XmlNode memberNames)
        {
            memberNames = new XmlDocument().CreateNode(XmlNodeType.Element, "dummy", null);
            XmlHelper.SetAttribute(memberNames, "childrentype", "included");

            foreach (Member member in h.Levels[0].GetMembers())
            {
                XmlHelper.AddChildNode(memberNames, "member", new string[] { "uname", member.UniqueName });
            }
        }

        public void LoadCube(CubeDef adomdCube)
        {
            UltraTreeNode cubeNode, dimNode = null;
            
            ClearEditor();

            try
            {
                if (adomdCube != null)
                {
                    PivotData.CubeName = adomdCube.Name;
                }
                if (adomdCube == null)
                {
                    return;
                }

                DimensionCollection adomdDims = adomdCube.Dimensions;

                cubeNode = tvFields.Nodes.Add(adomdCube.Name);
                cubeNode.Tag = new ItemData(ItemType.ntCube, null, null);
                cubeNode.Override.NodeAppearance.Image = 0;

                foreach (Dimension dim in adomdDims)
                {
                    if (dim.Hierarchies.Count > 1)
                    {
                        dimNode = cubeNode.Nodes.Add(dim.Caption);
                        dimNode.Tag = new ItemData(ItemType.ntDim, dim, null);
                        dimNode.Override.NodeAppearance.Image = 1;

                        foreach (Hierarchy hierch in dim.Hierarchies)
                        {
                            AddHierNode(dimNode, hierch);
                        }
                    }
                    else
                    {
                        AddHierNode(cubeNode, dim.Hierarchies[0]);
                    }
                }
                cubeNode.Expanded = true;
            }
            catch(Exception exc) 
            {
                if (exc is AdomdException)
                {
                    if (AdomdExceptionHandler.ProcessOK((AdomdException)exc))
                    {
                        AdomdExceptionHandler.IsRepeatedProcess = true;
                        LoadCube(PivotData.Cube);
                        AdomdExceptionHandler.IsRepeatedProcess = false;
                        return;
                    }
                }

                Common.CommonUtils.ProcessException(exc);
            }
        }

        private UltraTreeNode AddHierNode(UltraTreeNode root, Hierarchy adomdHierch)
        {
            UltraTreeNode node = null;

            FieldSet fieldset = new FieldSet(PivotData, adomdHierch.UniqueName, adomdHierch.Caption);

            XmlNode memberNames = null;

            InitMemberNames(adomdHierch, ref memberNames);

            fieldset.MemberNames = memberNames;

            ItemData iData = new ItemData(ItemType.ntHierarch, adomdHierch, fieldset);

            if (adomdHierch.Name == "Measures")
            {
                node = root.Nodes.Add(adomdHierch.UniqueName, "Меры");

                if (node != node.Parent.Nodes[0])
                {
                    node.Reposition(node.GetSibling(NodePosition.First), NodePosition.Previous);
                }
                LoadMembers(node, adomdHierch.Levels[0].GetMembers());
                node.Override.NodeAppearance.Image = 3;
                node.Expanded = true;
            }
            else
            {
                node = root.Nodes.Add(adomdHierch.UniqueName, adomdHierch.Caption);
                LoadHierarch(node, adomdHierch);
                node.Override.NodeAppearance.Image = 2;
                node.LeftImages.Add(imageList.Images[22]);
            }
            node.Tag = iData;
            return node;
        }

        private void LoadHierarch(UltraTreeNode root, Hierarchy adomdHierch)
        {
            UltraTreeNode node;
            PivotField field;

            foreach (Level level in adomdHierch.Levels)
            {
                if (level.Name != "(All)")
                {
                    field = new PivotField(PivotData, level.UniqueName, level.Caption);

                    foreach(LevelProperty prop in level.LevelProperties)
                    {
                        field.MemberProperties.AllProperties.Add(prop.Name);
                    }
                    node = root.Nodes.Add(level.UniqueName, level.Caption);
                    node.Tag = new ItemData(ItemType.ntLevel, level, field); 
                    node.Override.NodeAppearance.Image = level.LevelNumber + 5;
                }
            }
        }

        private void LoadMembers(UltraTreeNode root, MemberCollection adomdMbrs)
        {
            UltraTreeNode node;
            PivotTotal total;

            foreach (Member member in adomdMbrs)
            {
                total = new PivotTotal(PivotData, member.UniqueName, member.Caption, false, "", string.Empty, MeasureFormulaType.Custom);
                node = root.Nodes.Add(member.UniqueName, member.Caption);

                if (member.Type == MemberTypeEnum.Formula)
                {
                    node.Override.NodeAppearance.Image = 23;
                    total.IsCalculate = true;
                }
                else
                {
                    node.Override.NodeAppearance.Image = 4;
                    total.IsCalculate = false;
                }

                node.Tag = new ItemData(ItemType.ntMeasure, member, total);

            }
            root.Nodes.Override.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
        }

        #endregion
        
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
            if (PivotData.ObjectIsPreset(hierUniqueName))
            {
                PivotData.ShowMemberList(hierUniqueName);
                RefreshDropZonesView();
            }
            else
            {
                //иначе просто получаем список элементов
                FieldSet fieldSet = (FieldSet)iData.PivotObj;
                
                PivotData.ShowMemberListEx(h, ref fieldSet);

                if ((pdContainer != null) && (this.PivotData.GetFieldSet(fieldSet.UniqueName) != null))
                {
                    pdContainer.RefreshPivotData();
                }
            }
        }
        
        private void ShowPivotDataState()
        {
            //фильтрация была начата
            bool isBeginFiltering = false;
            while (this.PivotData.IsMembersFiltering || !isBeginFiltering)
            {
                if (this.PivotData.IsMembersFiltering)
                {
                    MainForm.Instance.Operation.Text = String.Format("Загрузка элементов...\n Текущий элемент: '{0}'\n Количество найденных: {1}",
                                                                     this.PivotData.CurLoadedMember, this.PivotData.LoadedMembersCount);
                    isBeginFiltering = true;
                }
            }
            MainForm.Instance.Operation.StopOperation();
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

                bool canRefreshData = PivotData.ShowFilters(h, ref fieldSet);

                if ((canRefreshData)&&((pdContainer != null) && (this.PivotData.GetFieldSet(fieldSet.UniqueName) != null)))
                {
                    //this.RefreshDropZonesView();
                    pdContainer.RefreshPivotData();
                }
           // }

        }


        public void AdjustColors(Color panelColor, Color borderColor, Color darkPanelColor)
        {
            this.BackColor = panelColor;
            splitter.BackColor = darkPanelColor;
            tvFields.Appearance.BorderColor = borderColor;
            this.CompositeChartEditor.AdjustColors(panelColor, borderColor, darkPanelColor);
            if (pdContainer != null)
            {
                pdContainer.AdjustColors(panelColor, borderColor, darkPanelColor);
            }
        }

        #region обработчики

        private void parentPanel_Resize(object sender, EventArgs e)
        {
            if (fieldsContainerPanel.Height > (parentPanel.Height - fieldListContainer.MinimumSize.Height - 5))
            {
                fieldsContainerPanel.Height = parentPanel.Height - fieldListContainer.MinimumSize.Height - 5;
            }
        }

        private void tvFields_SelectionDragStart(object sender, EventArgs e)
        {
            pdContainer.CanFieldDrop = true;
            UltraTreeNode node = ((UltraTree)sender).SelectedNodes[0];
            ItemType nodeType = FieldListHelper.GetItemData(node).ItemType;

            switch (nodeType)
            {
                case ItemType.ntLevel:
                case ItemType.ntMeasure:
                    ((UltraTree)sender).DoDragDrop(node, DragDropEffects.All);
                    break;

                case ItemType.ntHierarch:
                    ((UltraTree)sender).DoDragDrop(node.Nodes[0], DragDropEffects.All);
                    break;

            }
            pdContainer.CurrItemData = null;
        }

        private void tvFields_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            UltraTreeNode node = tvFields.GetNodeFromPoint(e.X, e.Y);
            if (node != null)
            {
                node.Selected = true;
            }
            */
        }

        private void tvFields_MouseClick(object sender, MouseEventArgs e)
        {
            UltraTree treeView = (UltraTree)sender;

            UltraTreeNode node = treeView.GetNodeFromPoint(e.X, e.Y);

            if (node != null)
            {
                ItemData iData = FieldListHelper.GetItemData(node);
                if ((iData.ItemType == ItemType.ntHierarch) && (!FieldListHelper.IsMeasuresNode(node)))
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        ShowFilters(iData);
                        //RefreshNodesColor(tvFields.Nodes[0]);
                    }
                    else if (((node.Bounds.Left + 54) > e.X) && ((node.Bounds.Left + 38) < e.X))
                    {

                        ShowMemberList(iData);
                    }
                }
            }
        }

        private void tvFields_AfterSelect(object sender, SelectEventArgs e)
        {
            /*
            if (lastSelectedNode != null)
            {
                if (lastSelectedNode.RightImages.Count > 0)
                {
                    lastSelectedNode.RightImages.Clear();
                }
            }

            if (e.NewSelections.Count == 0)
            {
                return;
            }

            lastSelectedNode = e.NewSelections[0];

            ItemType iType = FieldListHelper.GetItemData(lastSelectedNode).ItemType;
            if ((iType == ItemType.ntHierarch) || (iType == ItemType.ntLevel))
            {
                if (!FieldListHelper.IsMeasuresNode(lastSelectedNode))
                {
                    lastSelectedNode.RightImages.Add(imageList.Images[22]);
                }
            }
            */
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            if (PivotData != null)
            {
                bool fl = PivotData.IsDeferDataUpdating;
                PivotData.IsDeferDataUpdating = false;
                PivotData.DoDataChanged();
                PivotData.IsDeferDataUpdating = fl;
            }
        }

        private void cbDeferUpdating_CheckedChanged(object sender, EventArgs e)
        {
            if (PivotData != null)
            {
                PivotData.IsDeferDataUpdating = cbDeferUpdating.Checked;
            }
        }

        private void OnPivotDataViewChange()
        {
            RefreshFields();
            //RefreshDropZonesView();
        }

        private void OnAdomdExceptionReceived(Exception exc)
        {
            AdomdExceptionHandler.ProcessOK(exc);
            //RefreshData();
        }

        #endregion

        #region тесты

        private void button1_Click_1(object sender, EventArgs e)
        {
            PivotData.SavePivotDataSettings("temp.xml");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PivotData.LoadPivotDataSettings("temp.xml");
            RefreshDropZonesView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PivotData.ColumnAxis.FieldSets.Count; i++)
            {
                if (PivotData.ColumnAxis.FieldSets[i].MemberNames != null)
                {
                    MessageBox.Show(PivotData.ColumnAxis.FieldSets[i].MemberNames.OuterXml);
                }
            }
            for (int i = 0; i < PivotData.RowAxis.FieldSets.Count; i++)
            {
                if (PivotData.RowAxis.FieldSets[i].MemberNames != null)
                {
                    MessageBox.Show(PivotData.RowAxis.FieldSets[i].MemberNames.OuterXml);
                }
            }
            for (int i = 0; i < PivotData.FilterAxis.FieldSets.Count; i++)
            {
                if (PivotData.FilterAxis.FieldSets[i].MemberNames != null)
                {
                    MessageBox.Show(PivotData.FilterAxis.FieldSets[i].MemberNames.OuterXml);
                }
            }
        }
        #endregion
    }
}
