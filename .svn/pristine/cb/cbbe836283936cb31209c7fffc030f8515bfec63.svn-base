using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.FieldList
{
    public partial class PivotDataContainer : UserControl
    {
        private bool canFieldDrop = false;
        private ItemData currItemData = null;
        private List<AxisArea> areaList = new List<AxisArea>();
        private Data.PivotData pivotData;
        private FieldListEditor _fieldList;
        private CustomReportElement element;

        public FieldListEditor FieldList
        {
            get { return this._fieldList; }
            set { this._fieldList = value; }
        }

        public bool CanFieldDrop
        {
            get { return canFieldDrop; }
            set { canFieldDrop = value; }
        }

        public ItemData CurrItemData
        {
            get { return currItemData; }
            set { currItemData = value; }
        }

        public List<AxisArea> AreaList
        {
            get { return areaList; }
            set { areaList = value; }
        }

        public PivotData PivotData
        {
            get { return pivotData; }
            set { pivotData = value; }
        }

        public CustomReportElement Element
        {
            get { return this.element; }
        }

        public PivotDataContainer()
        {
        }

        public PivotDataContainer(Control parent, PivotData pivotData, CustomReportElement element)
        {
            InitializeComponent();

            this.Parent = parent;
            this.Dock = DockStyle.Fill;

            this.pivotData = pivotData;
            this.element = element;
        }

        /// <summary>
        /// Инициализация заголовков областей
        /// </summary>
        public virtual void InitCaptions()
        { 
        }

        public AxisArea GetAreaByAxisType(AxisType axisType)
        {
            foreach (AxisArea area in areaList)
            {
                if (area.AxisType == axisType)
                {
                    return area;
                }
            }
            return null;
        }

        /// <summary>
        /// Очистка дроп зон 
        /// </summary>
        public void ClearAreas()
        {
            foreach (AxisArea area in AreaList)
            {
                area.Nodes.Clear();
            }
        }

        /// <summary>
        /// Очистка заголовков
        /// </summary>
        public void ClearCaptions()
        {
            foreach (AxisArea area in AreaList)
            {
                area.Parent.Text = " ";
            }
        }

        public void AdjustColors(Color panelColor, Color borderColor, Color darkPanelColor)
        {
            foreach (AxisArea area in AreaList)
            {
                area.Appearance.BorderColor = borderColor;
            }
        }

        /// <summary>
        /// поиск поля во всех дроп зонах
        /// </summary>
        /// <param name="uniqueName">юникнейм поля</param>
        /// <returns>найденый узел</returns>
        public UltraTreeNode FindAxisAreaItem(string uniqueName)
        {
            AxisArea area = null;
            try
            {
                area = GetAreaByAxisType(pivotData.GetAxisTypeByObjectName(uniqueName));
            }
            catch
            { }

            if (area != null)
            {
                return area.FindItem(uniqueName);
            }
            else
            {
                return null;
            }
        }

        public void RemoveField(UltraTreeNode node)
        {
            ItemData iData = FieldListHelper.GetItemData(node);

            if (iData == currItemData)
            {
                return;
            }

            currItemData = iData;
            ClearItem(iData);
        }

        /// <summary>
        /// Удаляем элемент из дроп зоны и очищаем соответствующий узел в дереве
        /// </summary>
        /// <param name="iData">данные элемента</param>
        private void ClearItem(ItemData iData)
        {
            UltraTreeNode axisAreaItem = FindAxisAreaItem(FieldListHelper.GetUniqueNameFromItemData(iData));

            if (axisAreaItem != null)
            {
                //прежде чем удалить объект, сбросим у него сортировку
                this.pivotData.ResetTypeSort(iData.PivotObj);

                //сохраним состояние включености уровня в запрос
                bool stateIncludingInQuery = this.pivotData.StateIncludingInQuery(iData.PivotObj);
                //исключение уровня из зароса
                this.pivotData.ExcludeOfQuery(iData.PivotObj);

                if ((axisAreaItem.Parent != null) && (axisAreaItem.Parent.Nodes.Count == 1))
                {
                    axisAreaItem.Parent.Remove();
                }
                else
                {
                    axisAreaItem.Remove();
                }

                this.RefreshPivotData(iData, stateIncludingInQuery);
            }
        }

        #region Обновление данных для осей
        private void InitMeasures(UltraTreeNode measuresNode, ref XmlNode memberNames)
        {
            if (!FieldListHelper.IsMeasuresNode(measuresNode))
            {
                return;
            }

            memberNames = new XmlDocument().CreateNode(XmlNodeType.Element, "dummy", null);
            XmlHelper.SetAttribute(memberNames, "childrentype", "included");

            string uName = "";
            foreach (UltraTreeNode node in measuresNode.Nodes)
            {
                uName = FieldListHelper.GetUniqueNameFromNode(node);
                XmlHelper.AddChildNode(memberNames, "member", new string[]{ "uname", uName });
            }
        }

        /// <summary>
        /// Обновление данных оси
        /// </summary>
        /// <param name="axis">ось</param>
        private void SetAxisData(Data.Axis axis)
        {
            AxisArea area = GetAreaByAxisType(axis.AxisType);
            if (area == null)
            {
                return;
            }

            if ((axis.AxisType == AxisType.atTotals) && (this.PivotData.PivotDataType == PivotDataType.Map))
            {
                ((TotalAxis) axis).FieldSets.Clear();
            }
            else
            {
                axis.Clear();
            }


            ItemData iData;
            FieldSet fs;
            Hierarchy h;

            foreach (UltraTreeNode dzItem in area.Nodes)
            {
                iData = FieldListHelper.GetItemData(dzItem);

                if (iData.ItemType == ItemType.ntHierarch)
                {
                    h = ((Hierarchy)iData.AdomdObj);

                    fs = ((FieldSet)iData.PivotObj);
                    fs.Fields.Clear();

                    PivotField f;
                    if (FieldListHelper.IsMeasuresNode(dzItem))
                    {
                        XmlNode mbrNames = fs.MemberNames;
                        InitMeasures(dzItem, ref mbrNames);
                        fs.MemberNames = mbrNames;
                        f = new PivotField(fs, PivotData, h.Levels[0].UniqueName, h.Levels[0].Caption);
                        fs.Fields.Add(f);

                        //**if ((this is TablePivotDataContainer) || (this is MapPivotDataContainer))
                            RefreshTotals(dzItem);
                    }
                    else
                    {
                        foreach (UltraTreeNode fieldNode in dzItem.Nodes)
                        {
                            iData = FieldListHelper.GetItemData(fieldNode);
                            f = ((PivotField)iData.PivotObj);
                            f.ParentFieldSet = fs;
                            fs.Fields.Add(f);
                        }
                    }
                    axis.FieldSets.Add(fs);
                    fs.AxisType = axis.AxisType;
                }
            }
        }

        /// <summary>
        /// Обновление оси итогов
        /// </summary>
        private void RefreshTotals(UltraTreeNode totalsNode)
        {
            ItemData iData;
            Member total;
            PivotTotal pivotTotal;

            PivotData.TotalAxis.Totals.Clear();
            PivotData.TotalAxis.FieldSets.Clear();

            foreach (UltraTreeNode node in totalsNode.Nodes)
            {
                iData = FieldListHelper.GetItemData(node);

                if (iData.ItemType == ItemType.ntMeasure)
                {
                    pivotTotal = ((PivotTotal)iData.PivotObj);
                    if (pivotTotal.IsCustomTotal)
                    {
                        pivotTotal.IsCalculate = true;
                    }
                    else
                    {
                        total = ((Member) iData.AdomdObj);

                        if (total != null)
                        {
                            pivotTotal.IsCalculate = (total.Type == MemberTypeEnum.Formula);
                        }
                    }

                    PivotData.TotalAxis.Totals.Add(pivotTotal);
                }
            }
        }

        /// <summary>
        /// Обновление данных всех осей
        /// </summary>
        public void RefreshPivotData()
        {
            this.RefreshPivotData(null);
        }

        /// <summary>
        /// Обновление данных всех осей
        /// </summary>
        public void RefreshPivotData(ItemData editingItem)
        {
            this.RefreshPivotData(editingItem, false);
        }

        /// <summary>
        /// Обновление данных всех осей
        /// </summary>
        public void RefreshPivotData(ItemData editingItem, bool isForceDataChanged)
        {
            foreach (Data.Axis axis in PivotData.Axes)
            {
                SetAxisData(axis);
            }
            this.PivotData.RefreshIncludingToQuery();
            if (isForceDataChanged || this.IsNeedDataChanged(editingItem))
                this.PivotData.DoDataChanged();
            else
                this.PivotData.DoStructureChanged();
        }

        /// <summary>
        /// Требуется ли выполнения запроса, если не динамический режим, то требуется всегда,
        /// если дин. то только когда уровень учитывается в запросе
        /// </summary>
        /// <param name="editingItem">редактируемый элемент</param>
        /// <returns></returns>
        private bool IsNeedDataChanged(ItemData editingItem)
        {
            bool result = true;
            if (this.PivotData.DynamicLoadData && (editingItem != null) && (editingItem.PivotObj != null))
            {
                PivotField field = null;
                switch (editingItem.PivotObj.ObjectType)
                {
                    case PivotObjectType.poField:
                        {
                            field = (PivotField)editingItem.PivotObj;
                            break;
                        }
                    case PivotObjectType.poFieldSet:
                        {
                            field = ((FieldSet)editingItem.PivotObj).Fields[0];
                            break;
                        }
                }
                if (field != null)
                    result = field.IsIncludeToQuery;
            }
            return result;
        }

        #endregion

        virtual public void SetEnabledGroup(CustomReportElement reportElement)
        {
        }
    }
}
