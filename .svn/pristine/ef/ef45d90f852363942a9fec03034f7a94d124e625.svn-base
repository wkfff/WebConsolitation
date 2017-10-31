using System;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace Krista.FM.Client.MDXExpert.Data
{
    /// <summary>
    /// Класс для осей столбцов, строк, фильтров
    /// </summary>
    public class PivotAxis : Axis
    {
        /// <summary>
        ///Порог (количество элементов в оси) включения в автоматическом режиме
        ///скрытия пустых на уровне таблицы фактов
        /// </summary>
        const long automateModeThreshold = 5000;
        #region поля

        private bool hideEmptyPositions = true;
        private HideEmptyMode _hideEmptyMode;
        private bool grandTotalVisible;
        private bool _includeInChartLabelParentLevel;
        private MemberPropertiesDisplayType propertiesDisplayType;
        private SortType _sortType;
        private bool _sortByName;
        private bool _reverseOrder;

        #endregion

        #region Свойства

        /// <summary>
        /// Скрывать пустые позиции в оси?
        /// (Свойство влият только на сборку MDX 
        ///   => относится к структуре данных элемента
        ///   => расположено здесь)
        /// </summary>
        public bool HideEmptyPositions
        {
            get { return hideEmptyPositions; }
            set 
            { 
                hideEmptyPositions = value; 
                this.ParentPivotData.DoDataChanged();
            }
        }

        /// <summary>
        /// Показывать общий итог
        /// </summary>
        public bool GrandTotalVisible
        {
            get { return grandTotalVisible; }
            set 
            { 
                grandTotalVisible = value;
                this.ParentPivotData.DoDataChanged();
            }
        }

        /// <summary>
        /// Есть гранд тотал реально 
        /// </summary>
        public bool GrandTotalExists
        {
            get 
            {
                //return (GrandTotalVisible && GrandTotalCapable() && (FieldSets.Count > 0));

                //Сейчас главный итог возможно получить всегда. 
                //итог = значение по мемберу ALL каждого измерения в оси. 
                //Если мембера ALL в измерении нет, то берется Default Member
                return (GrandTotalVisible && (FieldSets.Count > 0));
            }
        }

        public MemberPropertiesDisplayType PropertiesDisplayType
        {
            get { return propertiesDisplayType; }
            set
            {
                propertiesDisplayType = value;
                this.ParentPivotData.DoAppearanceChanged(true);
            }
        }

        /// <summary>
        /// У диаграмм включать в название лейблов имена родительских элементов с 
        /// предыдущих вынесенных уровней 
        /// </summary>
        public bool IncludeInChartLabelParentMember
        {
            get { return _includeInChartLabelParentLevel; }
            set 
            {
                if (value != _includeInChartLabelParentLevel)
                {
                    _includeInChartLabelParentLevel = value;
                    this.ParentPivotData.DoDataChanged();
                }
            }
        }

        /// <summary>
        /// Тип сортировки у оси, когда его выставляем, сбрасываем сортировку у всех измерений в оси
        /// </summary>
        public SortType SortType
        {
            get { return _sortType; }
            set
            {
                if ((this.PivotDataType == PivotDataType.Table)
                    && ((value == SortType.BDESC) || (value == SortType.BASC)))
                {
                    DialogResult dialogResult = MessageBox.Show(Common.Consts.sortWarning, "MDXExpert ", System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.No)
                        return;
                }

                _sortType = value;
                this.FieldSets.RefreshSortType(this);
                //Если это таблица, и выставляем сортировку оси строк, признак сортировки надо сбросить и 
                //у показателей
                if ((this.PivotDataType == PivotDataType.Table) && (this.AxisType == AxisType.atRows))
                    this.ParentPivotData.TotalAxis.RefreshTypeSort(null);
                this.ParentPivotData.RefreshTypeSort(this);
                if (!this.ParentPivotData.IsDeferDataUpdating)
                    this.ParentPivotData.DoDataChanged();

                this.ParentPivotData.DoElementsSortChanged(this);
            }
        }

        /// <summary>
        /// Сортировка по имени
        /// </summary>
        public bool SortByName
        {
            get { return _sortByName; }
            set
            {
                _sortByName = value;
                this.ParentPivotData.DoElementsSortChanged(this);
            }
        }

        /// <summary>
        /// Обратный порядок элементов
        /// </summary>
        public bool ReverseOrder
        {
            get { return _reverseOrder; }
            set
            {
                _reverseOrder = value;
                this.ParentPivotData.DoElementsOrderChanged(this);
                //this.ParentPivotData.DoStructureChanged();
            }
        }


        /// <summary>
        /// Существует ли у оси хотя бы один объект с сортировкой
        /// </summary>
        public bool IsSorted
        {
            get
            {
                return (this.SortType != SortType.None) || (this.FieldSets.IsSorted);
            }
        }

        #endregion

        public PivotAxis(Client.MDXExpert.Data.PivotData pivotData, AxisType axisType)
        {
            this.axisType = axisType;
            this.FieldSets = new FieldSetCollection(pivotData);
            this.objectType = PivotObjectType.poAxis;
            base._parentPivotData = pivotData;
            //для карты по умолчанию выбираем и пустые элементы тоже
            if ((this.PivotDataType == PivotDataType.Map)&&(axisType == Data.AxisType.atRows))
                this.hideEmptyPositions = false;
            this.propertiesDisplayType = MemberPropertiesDisplayType.DisplayInReport;
            this.grandTotalVisible = (pivotData.Type == PivotDataType.Table);
            this._hideEmptyMode = HideEmptyMode.Automate;
        }

        /// <summary>
        /// Очистка данных оси
        /// </summary>
        public override void Clear()
        {
            FieldSets.Clear();
        }

        public override PivotObject GetPivotObject(string objectName)
        {
            PivotObject obj = FieldSets.GetFieldSetByName(objectName);
            if (obj == null)
            {
                obj = FieldSets.GetFieldByName(objectName);
            }
            return obj; 
        }

        /// <summary>
        /// Получение настройки оси в виде xml
        /// </summary>
        /// <returns>узел с настройками оси</returns>
        public override XmlNode SaveSettingsXml(XmlNode parentNode)
        {
            XmlNode root = XmlHelper.AddChildNode(parentNode, "fieldsets", "", null);
            XmlNode fieldSetNode, nameListNode, fieldListNode, fieldNode;

            XmlHelper.SetAttribute(root, "hideempty", HideEmptyPositions.ToString());
            XmlHelper.SetAttribute(root, "gtotalvisible", GrandTotalVisible.ToString());
            XmlHelper.SetAttribute(root, "mpdisplaytype", this.PropertiesDisplayType.ToString());
            XmlHelper.SetAttribute(root, "includeInChartLabelParentMember", 
                this.IncludeInChartLabelParentMember.ToString());
            XmlHelper.SetAttribute(root, Consts.hideEmptyMode, this.NonDeterminatedHideEmptyMode.ToString());
            XmlHelper.SetAttribute(root, "axisSortType", this.SortType.ToString());
            XmlHelper.SetAttribute(root, "axisSortByName", this.SortByName.ToString());
            XmlHelper.SetAttribute(root, "reverseOrder", this.ReverseOrder.ToString());

            foreach (FieldSet fieldSet in FieldSets)
            {
                fieldSetNode = XmlHelper.AddChildNode(root, "fieldset", 
                                                        new string[2] {"uname", fieldSet.UniqueName},
                                                        new string[2] {"caption", fieldSet.Caption});

                if (fieldSet.MemberNames != null)
                {
                    nameListNode = XmlHelper.AddChildNode(fieldSetNode, "membernames", "", null);
                    nameListNode.InnerXml = fieldSet.MemberNames.OuterXml;
                }
                if(fieldSet.ExceptedMembers != null)
                {
                    XmlHelper.AddStringListNode(fieldSetNode, "exceptedMembers", fieldSet.ExceptedMembers);
                }

                XmlHelper.SetAttribute(fieldSetNode, "useinchartlabels", fieldSet.UsedInChartLabels.ToString());
                XmlHelper.SetAttribute(fieldSetNode, "displayMemberType", fieldSet.DisplayMemberType.ToString());
                //тип сортировки
                XmlHelper.SetAttribute(fieldSetNode, "sortType", fieldSet.SortType.ToString());

                fieldListNode = XmlHelper.AddChildNode(fieldSetNode, "fields", "", null);

                fieldSet.MemberFilters.SaveXml(fieldSetNode);

                foreach (PivotField field in fieldSet.Fields)
                {
                    fieldNode = XmlHelper.AddChildNode(fieldListNode, "field", 
                                            new string[2] {"uname", field.UniqueName},
                                            new string[2] {"caption", field.Caption});
                    //Видимость итога у уровня
                    XmlHelper.SetAttribute(fieldNode, Consts.isVisibleTotal, field.IsVisibleTotal.ToString());
                    //Видимость дата мембера у уровня
                    XmlHelper.SetAttribute(fieldNode, Consts.isVisibleDataMember, field.IsVisibleDataMember.ToString());
                    
                    if (PropertiesDisplayType != MemberPropertiesDisplayType.None)
                    {
                        XmlHelper.SetAttribute(fieldNode, "mbrprops", field.MemberProperties.ToString());
                    }
                    XmlHelper.SetAttribute(fieldNode, Consts.isIncludeToQuery, field.IsIncludeToQuery.ToString());
                    //тип сортировки
                    XmlHelper.SetAttribute(fieldNode, "sortType", field.SortType.ToString());

                }
            }
            return root;
        }

        /// <summary>
        /// Установка настроек оси
        /// </summary>
        /// <param name="node">узел с настройками оси</param>
        public override void SetXml(XmlNode node)
        {
            FieldSet fieldSet;

            node = node.SelectSingleNode("fieldsets");
            if (node == null)
                return;

            //для карты по умолчанию выбираем и пустые элементы тоже
            if ((this.PivotDataType == PivotDataType.Map) && (axisType == Data.AxisType.atRows))
            {
                HideEmptyPositions = false;
            }
            else
            {
                HideEmptyPositions = XmlHelper.GetBoolAttrValue(node, "hideempty", true);
            }
            //режим скрытия пустых
            string sHideEmptyMode = XmlHelper.GetStringAttrValue(node, Consts.hideEmptyMode, HideEmptyMode.Automate.ToString());
            this.HideEmptyMode = (HideEmptyMode)Enum.Parse(typeof(HideEmptyMode), sHideEmptyMode);
            
            GrandTotalVisible = XmlHelper.GetBoolAttrValue(node, "gtotalvisible", true);
            PropertiesDisplayType = (MemberPropertiesDisplayType)Enum.Parse(typeof(MemberPropertiesDisplayType), 
                XmlHelper.GetStringAttrValue(node, "mpdisplaytype", "None"));
            this.IncludeInChartLabelParentMember = XmlHelper.GetBoolAttrValue(node,
                "includeInChartLabelParentMember", false);
            this.SetSortTypeWithoutRefresh((SortType)Enum.Parse(typeof(SortType),
                XmlHelper.GetStringAttrValue(node, "axisSortType", "None")));
            this.SetSortByNameWithoutRefresh(XmlHelper.GetBoolAttrValue(node, "axisSortByName", false));
            this.SetReverseOrderWithoutRefresh(XmlHelper.GetBoolAttrValue(node, "reverseOrder", false));


            XmlNodeList fieldSetNodeList = node.SelectNodes("fieldset");
            XmlNodeList fieldNodeList;
            PivotField f;
            string[] tmpStrList;            

            foreach (XmlNode fieldSetNode in fieldSetNodeList)
            {
                fieldSet = new FieldSet(this.AxisType, this.ParentPivotData, 
                                        XmlHelper.GetStringAttrValue(fieldSetNode, "uname", ""),
                                        XmlHelper.GetStringAttrValue(fieldSetNode, "caption", "")
                                        );
                                         
                fieldSet.MemberNames = fieldSetNode.SelectSingleNode("membernames/dummy");
                fieldSet.ExceptedMembers = XmlHelper.GetStringListFromXmlNode(fieldSetNode.SelectSingleNode("exceptedMembers"));

                fieldSet.UsedInChartLabels = XmlHelper.GetBoolAttrValue(fieldSetNode, "useinchartlabels", true);
                fieldSet.DisplayMemberType = (DisplayMemberType)Enum.Parse(typeof(DisplayMemberType),
                    XmlHelper.GetStringAttrValue(fieldSetNode, "displayMemberType", "Auto"));
                //тип сортировки
                fieldSet.SetSortTypeWithoutRefresh((SortType)Enum.Parse(typeof(SortType),
                    XmlHelper.GetStringAttrValue(fieldSetNode, "sortType", "None")));
                fieldNodeList = fieldSetNode.SelectNodes("fields/field");

                fieldSet.MemberFilters.LoadXml(fieldSetNode.SelectSingleNode("memberFilters"));

                foreach (XmlNode fieldNode in fieldNodeList)
                {
                    f = new PivotField(fieldSet, fieldSet.ParentPivotData, 
                                        XmlHelper.GetStringAttrValue(fieldNode, "uname", ""), 
                                        XmlHelper.GetStringAttrValue(fieldNode, "caption", ""));

                    tmpStrList = XmlHelper.GetStringAttrValue(fieldNode, "mbrprops", "").Split(';');
                    for(int i = 0; i < tmpStrList.Length - 1; i++)
                    {
                        f.MemberProperties.VisibleProperties.Add(tmpStrList[i]);  
                    }
                    //Видимость итога у уровня, у таблицы она по умолчанию включена, у диаграммы отключена
                    f.IsVisibleTotal = XmlHelper.GetBoolAttrValue(fieldNode, Consts.isVisibleTotal,
                        (this.ParentPivotData.Type == PivotDataType.Table) ? true : false);
                    //Видимость дата мемберов у уровня
                    f.IsVisibleDataMember = XmlHelper.GetBoolAttrValue(fieldNode, Consts.isVisibleDataMember, true);

                    f.IsIncludeToQuery = XmlHelper.GetBoolAttrValue(fieldNode, Consts.isIncludeToQuery, false);
                    //тип сортировки
                    f.SetSortTypeWithoutRefresh((SortType)Enum.Parse(typeof(SortType),
                        XmlHelper.GetStringAttrValue(fieldNode, "sortType", "None")));

                    fieldSet.Fields.Add(f);
                }

                FieldSets.Add(fieldSet);
            }
        }

        /// <summary>
        /// Обновление признаков включености уровней в запрос
        /// </summary>
        public void RefreshIncludingToQuery()
        {
            //Первый раз пробегаем по всем уровням, и включенным элемент будет если он
            //1) Является первым в оси
            //2) Включен, а также включены все урони до него
            //3) Если у элемента множественный выбор
            bool isAllIncuded = true;
            foreach (FieldSet fieldSet in this.FieldSets)
            {
                foreach (PivotField field in fieldSet.Fields)
                {
                    if (!field.IsIncludeToQuery)
                        isAllIncuded = false;
                    field.IsIncludeToQuery = this.GetFlagIncludeToQuery(field, isAllIncuded);
                }
            }

            //Второй раз пробегаем с конца, и если встречаются уровни с множественным выбором,
            //то для корректного отображения данных, включаем все уровни до имеющего множественный выбор
            bool isExistIncludeField = false;
            for (int i = this.FieldSets.Count - 1; i >= 0; i--)
            {
                FieldSet fieldSet = this.FieldSets[i];
                for (int j = fieldSet.Fields.Count - 1; j >= 0; j--)
                {
                    PivotField field = fieldSet.Fields[j];
                    if (field.IsIncludeToQuery)
                    {
                        isExistIncludeField = true;
                        continue;
                    }
                    if (isExistIncludeField)
                        field.IsIncludeToQuery = true;
                }
            }
        }

        /// <summary>
        /// Вернет true если, или не включен режим динамической загрузки
        /// 1) Является первым в оси
        /// 2) Включен, а также включены все урони до него
        /// 3) Если у элемента множественный выбор
        /// </summary>
        /// <param name="field">уровень</param>
        /// <param name="isAllInclude">если все уровни до него включены</param>
        /// <returns></returns>
        private bool GetFlagIncludeToQuery(PivotField field, bool isAllInclude)
        {
            if (this.ParentPivotData.DynamicLoadData)
            {
                bool isFirstFieldSetInAxis = (field.ParentFieldSet == field.ParentFieldSet.ParentCollection[0]);
                bool isFirstLevel = isFirstFieldSetInAxis && (field == field.ParentFieldSet.Fields[0]);
                return isFirstLevel || (field.IsIncludeToQuery && isAllInclude) 
                    || field.ParentFieldSet.IsMultipleChoise();
            }
            else
                return true;
        }

        public void InitialByClsInfo(List<DimensionInfo> axisInfo)
        {
            FieldSetCollection fieldSetCollection = new FieldSetCollection(this.ParentPivotData);
            foreach (DimensionInfo dimInfo in axisInfo)
            {
                FieldSet fieldSet = this.ParentPivotData.GetFieldSet(dimInfo.UniqueName);
                if (fieldSet == null)
                    fieldSet = fieldSetCollection.Add(dimInfo.UniqueName, dimInfo.Сaption);
                else
                    fieldSetCollection.Add(fieldSet);

                List<PivotField> pivotFields = new List<PivotField>();
                foreach (LevelInfo levelInfo in dimInfo.LevelsInfo)
                {
                    PivotField pivotField = fieldSet.GetFieldByName(levelInfo.UniqueName);
                    if (pivotField == null)
                        pivotFields.Add(new PivotField(fieldSet, this.ParentPivotData, levelInfo.UniqueName, levelInfo.Caption));
                    else
                        pivotFields.Add(pivotField);
                }
                fieldSet.Fields = pivotFields;
                //инициализируем membersNames измерения
                this.ParentPivotData.InitFieldSetMemberNames(fieldSet, dimInfo);
            }
            this.FieldSets = fieldSetCollection;
        }

        /// <summary>
        /// Очистим ось от измерений которых больше нет в cellSet
        /// </summary>
        /// <param name="axisInfo"></param>
        public void ClearAxis(List<DimensionInfo> axisInfo)
        {
            if (axisInfo.Count == 0)
                this.FieldSets.Clear();
        }

        /// <summary>
        /// Признак возможности получения общега итога 
        /// </summary>
        /// <returns> TRUE, если у всех элементов коллекции (измерений) есть уровень All.
        /// Имя это жесткое, поэтому проверять наличие можно так: Самый первый уровень измерения имеет имя (All).
        /// Если хоть у одного это не так, тогда вся ф-я - FALSE</returns>
        private bool GrandTotalCapable()
        {
            Hierarchy h = null;

            foreach (FieldSet fs in FieldSets)
            {
                h = this.ParentPivotData.GetAdomdHierarchy(fs.UniqueName); 
                if (h != null)
                {
                    if (h.Levels[0].Name != "(All)")
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Вычисляем какой режим скрытия пустых использовать для данной оси
        /// </summary>
        /// <returns></returns>
        private HideEmptyMode GetHideEmptyMode()
        {
            HideEmptyMode result = _hideEmptyMode;
            //Если стоит автоматический режим, проверим количестов элементов на в оси, если оно больше определеного
            //значения, используем NonEmptyCrossJoin
            if (result == HideEmptyMode.Automate)
            {
                long membresCount = 0;
                foreach (FieldSet fieldSet in this.FieldSets)
                {
                    foreach (PivotField field in fieldSet.Fields)
                    {
                        membresCount += field.AdomdLevel.MemberCount;
                    }
                }
                result = (membresCount > automateModeThreshold) ? HideEmptyMode.NonEmptyCrossJoin : HideEmptyMode.NonEmpty;
            }

            if ((PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2000)
                && (result == HideEmptyMode.NonEmpty2005))
            {
                return HideEmptyMode.NonEmpty;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Установить режим скрытия пустых, с возможностью обновления данных
        /// </summary>
        /// <param name="mode">режим</param>
        /// <param name="IsRefreshData">обновлять ли данные</param>
        public void SetHideEmptyMode(HideEmptyMode mode, bool IsRefreshData)
        {
            if (mode != this.NonDeterminatedHideEmptyMode)
            {
                //предыдущий режим скрытия
                HideEmptyMode prepareEmptyMode = this.HideEmptyMode;
                this.HideEmptyMode = mode;
                //если требуется обновление, и предварительный режим скрытия не равен текущему, обновим элемент
                if (IsRefreshData && !this.ParentPivotData.IsDeferDataUpdating && (prepareEmptyMode != this.HideEmptyMode))
                    this.ParentPivotData.DoDataChanged();
            }
        }

        /// <summary>
        /// Режим скрытия пустых, может быть автоматическим, когда режим зависит от количества элементов 
        /// на уровнях учавствующих в выборке
        /// </summary>
        public HideEmptyMode HideEmptyMode
        {
            get 
            {
                return GetHideEmptyMode();
            }
            set { _hideEmptyMode = value; }
        }

        /// <summary>
        /// Режим скрытия, такой какой есть, не вычисляющийся даже при автоматическом режиме
        /// </summary>
        public HideEmptyMode NonDeterminatedHideEmptyMode
        {
            get { return _hideEmptyMode; }
        }

        /// <summary>
        /// Все ли измерения в оси имеют уровень All
        /// </summary>
        /// <returns></returns>
        public bool IsAllDimHasLevelAll
        {
            get
            {
                foreach (FieldSet fs in FieldSets)
                {
                    if (!fs.ExistLevelAll)
                        return false;
                }
                return true;
            }
        }

        public void RefreshTypeSort(PivotObject sender)
        {
            this.FieldSets.RefreshSortType(sender);
            this.SetSortTypeWithoutRefresh(SortType.None);
        }

        public void SetSortTypeWithoutRefresh(SortType sortType)
        {
            this._sortType = sortType;
        }

        public void SetSortByNameWithoutRefresh(bool value)
        {
            this._sortByName = value;
        }

        public void SetReverseOrderWithoutRefresh(bool value)
        {
            this._reverseOrder = value;
        }

    }
    /// <summary>
    /// Режим скрытия пустых
    /// </summary>
    public enum HideEmptyMode
    {
        [Description("Автоматическое")]
        Automate,
        [Description("На уровне оси (Non Empty)")]
        NonEmpty,
        [Description("На уровне множества (Filter)")]
        UsingFilter,
        [Description("На уровне таблицы фактов (NonEmptyCrossJoin)")]
        NonEmptyCrossJoin,
        [Description("На уровне множества (MASS2005)")]
        NonEmpty2005
    }
}
