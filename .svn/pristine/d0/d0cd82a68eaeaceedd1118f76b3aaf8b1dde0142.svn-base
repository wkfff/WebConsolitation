using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.CommonClass;
using System.Text.RegularExpressions;
using System.Data;

namespace Krista.FM.Client.MDXExpert.Data
{
    public delegate void PivotDataEventHandler();

    public delegate void PivotDataAppChangeEventHandler(bool isNeedRecalculateGrid);

    public delegate void PivotDataAdomdExceptionEventHandler(Exception e);

    public delegate void PivotDataChangeOrderEventHandler(PivotAxis axis);
    public delegate void PivotDataChangeSortEventHandler(PivotAxis axis);



    public class PivotData : PivotObject
    {
        #region поля

        private string _cubeName;

        private List<Axis> axes;

        private PivotAxis columnAxis;
        private PivotAxis rowAxis;
        private PivotAxis filterAxis;
        private TotalAxis totalAxis;

        private string selection;
        private SelectionType selectionType;
        private PivotObject _selectedObject;

        protected MemberList memberList = null;
        //protected MembersFilterForm _filterForm = null;

        private bool isDeferDataUpdating;
        private bool _isVisualTotals;

        private bool _isCustomMDX;
        private string _mdxQuery;

        private bool _dynamicLoadData;

        private PivotDataType type;
        private static AdomdConnection _adomdConn;
        private static AnalysisServicesVersion _analysisServicesVersion;

        private CellSetInfo _clsInfo = null;

        private DateTime cubeLastProcessed;

        private bool _isMembersFiltering = false;
        private string _curLoadedMember;

        private AverageSettings _averageSettings;
        private MedianSettings _medianSettings;
        private TopCountSettings _topCountSettings;
        private BottomCountSettings _bottomCountSettings;

        #endregion

        #region свойства


        /// <summary>
        /// Куб, из которого берутся данные для элемента отчета
        /// </summary>
        public CubeDef Cube
        {
            get
            {
                CubeDef result = null;
                try
                {
                    if (AdomdConn != null)
                    {
                        if (AdomdConn.State != System.Data.ConnectionState.Open)
                            AdomdConn.Open();

                        if (AdomdConn.State == System.Data.ConnectionState.Open)
                             result = AdomdConn.Cubes.Find(this.CubeName);
                    }
                }
                catch(Exception exc)
                {
                    DoAdomdExceptionReceived(exc);
                }

                return result;
            }
        }

        public static AdomdConnection AdomdConn
        {
            get { return _adomdConn; }
            set 
            { 
                _adomdConn = value;
                AnalysisServicesVersion = GetAnalysisVersion(value);
            }
        }

        public static AnalysisServicesVersion AnalysisServicesVersion
        {
            get { return PivotData._analysisServicesVersion; }
            set { PivotData._analysisServicesVersion = value; }
        }

        public string CubeName
        {
            get 
            { 
                string result = this._cubeName;
                if (this.IsCustomMDX)
                    result = this.RetrieveCubeNameFromMDX(this.MDXQuery);
                return result; 
            }
            set { this._cubeName = value; }
        }

        public List<Axis> Axes
        {
            get { return axes; }
        }

        public PivotAxis ColumnAxis
        {
            get { return columnAxis; }
            set { columnAxis = value; }
        }

        public PivotAxis RowAxis
        {
            get { return rowAxis; }
            set { rowAxis = value; }
        }

        public PivotAxis FilterAxis
        {
            get { return filterAxis; }
            set { filterAxis = value; }
        }

        public TotalAxis TotalAxis
        {
            get { return totalAxis; }
            set { totalAxis = value; }
        }

        /// <summary>
        /// Выделенный объект Пивот даты
        /// </summary>
        public PivotObject SelectedObject
        {
            get { return _selectedObject; }
            set { _selectedObject = value; }
        }

        /// <summary>
        /// Выделенное поле
        /// </summary>
        public PivotField SelectedField
        {
            get 
            {
                PivotField result = null;
                if ((this.SelectedObject != null) && (this.SelectedObject is PivotField))
                    result = (PivotField)this.SelectedObject;
                return result;
            }
        }

        /// <summary>
        /// Выделенная мера
        /// </summary>
        public PivotTotal SelectedMeasure
        {
            get
            {
                PivotTotal result = null;
                if ((this.SelectedObject != null) && (this.SelectedObject is PivotTotal))
                    result = (PivotTotal)this.SelectedObject;
                return result;
            }
        }

        /// <summary>
        /// Данные структуры в виде xml
        /// </summary>
        public XmlNode XmlSettings
        {
            get
            {
                XmlNode root = new XmlDocument().CreateNode(XmlNodeType.Element, Consts.pivotData, null);
                this.Save(root);

                return root;
            }
            set { this.Load(value, true); }
        }

        /// <summary>
        /// Данные структуры в виде строки
        /// </summary>
        public string StrSettings
        {
            get { return XmlSettings.OuterXml; }
            set { XmlSettings.InnerXml = value; }
        }

        /// <summary>
        /// уникальное имя выбранного элемента
        /// </summary>
        public string Selection
        {
            get 
            {
                if (this.selectionType != SelectionType.SingleObject)
                {
                    selection = "";
                }
                return selection; 
            }
            set 
            {
                if (value != "")
                {
                    this.selectionType = SelectionType.SingleObject;
                }
                SetSelection(this.selectionType, value);
            }
        }

        /// <summary>
        /// тип выбранного элемента
        /// </summary>
        public SelectionType SelectionType
        {
            get 
            { 
                if (this.selection != "")
                {
                    selectionType = SelectionType.SingleObject;
                }
                return selectionType; 
            }
            set 
            {
                if (value != SelectionType.SingleObject)
                {
                    this.selection = "";
                }
                selectionType = value; 
            }
        }

        /// <summary>
        /// Флаг, можно ли обрабатывать событие - изменение структуры
        /// </summary>
        public bool IsDeferDataUpdating
        {
            get { return isDeferDataUpdating; }
            set { isDeferDataUpdating = value; }
        }

        /// <summary>
        /// Признак, что выполняется пользовательский (введенный в ручну) MDX запрос
        /// </summary>
        public bool IsCustomMDX
        {
            get { return _isCustomMDX; }
            set { _isCustomMDX = value; }
        }

        /// <summary>
        /// MDX запрос элемента отчета
        /// </summary>
        public string MDXQuery
        {
            get { return _mdxQuery; }
            set { _mdxQuery = value; }
        }

        /// <summary>
        /// Признак что итоги вычисляются по видимым (выбранным) элементам
        /// </summary>
        public bool IsVisualTotals
        {
            get { return _isVisualTotals; }
            set 
            { 
                _isVisualTotals = value;
                this.DoDataChanged();
            }
        }

        /// <summary>
        /// Динамическая загрузка данных возможна только у стандартных таблиц
        /// </summary>
        public bool DynamicLoadData
        {
            get
            { 
                return _dynamicLoadData && !this.IsCustomMDX;
            }
            set 
            { 
                _dynamicLoadData = value;
                Application.DoEvents();
                this.DoDataChanged();
            }
        }

        /// <summary>
        /// Тип пивот даты (зависит от элемента, для которого строится)
        /// </summary>
        public PivotDataType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Существует ли на данный момент соединение с сервером
        /// </summary>
        public bool IsExistsConnection
        {
            get
            {
                if ((AdomdConn == null) || (AdomdConn.State != System.Data.ConnectionState.Open))
                    return false;
                try
                {
                    //Даже если потеряно соединение Adomd молчит до последнего, и берет данные 
                    //из локального кеша. При обращеник к свойству Database, Adomd вынужден 
                    //послать запрос на сервер, если соединения нет, здесь мы его и поймаем.
                    string dataBase = AdomdConn.Database;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool CheckConnection()
        {
            if ((AdomdConn == null) || (AdomdConn.State != System.Data.ConnectionState.Open))
            {
                DoForceDataChanged();
                return false;
            }
            try
            {
                //Даже если потеряно соединение Adomd молчит до последнего, и берет данные 
                //из локального кеша. При обращеник к свойству Database, Adomd вынужден 
                //послать запрос на сервер, если соединения нет, здесь мы его и поймаем.
                string dataBase = AdomdConn.Database;
            }
            catch(Exception exc)
            {
                this.DoAdomdExceptionReceived(exc);
                if (this.IsExistsConnection)
                {
                    DoForceDataChanged();
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Информация извлеченная из селсета
        /// </summary>
        public CellSetInfo ClsInfo
        {
            get { return _clsInfo; }
            set { _clsInfo = value; }
        }

        /// <summary>
        /// Идет фильтрация элементов измерения
        /// </summary>
        public bool IsMembersFiltering
        {
            get { return this.memberList.IsFiltering; }
        }

        /// <summary>
        /// Текущий элемент, загруженный в список
        /// </summary>
        public string CurLoadedMember
        {
            get { return this.memberList.CurrLoadedMember; }
        }

        /// <summary>
        /// Число загруженных элементов в выпадающем списке
        /// </summary>
        public int LoadedMembersCount
        {
            get { return this.memberList.LoadedMembersCount; }
        }

        /// <summary>
        /// Настройки среднего значения
        /// </summary>
        public AverageSettings AverageSettings
        {
            get { return this._averageSettings; }
            set
            {
                this._averageSettings = value;
                //this.DoDataChanged();
            }
        }

        /// <summary>
        /// Настройки k-первых
        /// </summary>
        public TopCountSettings TopCountSettings
        {
            get { return this._topCountSettings; }
            set
            {
                this._topCountSettings = value;
                //this.DoDataChanged();
            }
        }

        /// <summary>
        /// Настройки k-последних
        /// </summary>
        public BottomCountSettings BottomCountSettings
        {
            get { return this._bottomCountSettings; }
            set
            {
                this._bottomCountSettings = value;
                //this.DoDataChanged();
            }
        }



        /// <summary>
        /// Настройки медианы
        /// </summary>
        public MedianSettings MedianSettings
        {
            get { return this._medianSettings; }
            set
            {
                this._medianSettings = value;
                //this.DoDataChanged();
            }
        }


        #endregion

        #region события

        //при изменении структуры данных
        private PivotDataEventHandler _dataChanged = null;
        //при изменении вида структуры в редакторе(используется для отложенного обновления макета)
        private PivotDataEventHandler _viewChanged = null;
        //при изменении выбранного объекта
        private PivotDataEventHandler _selectionChanged = null;
        //при изменении внешнего вида какого либо объекта
        private PivotDataAppChangeEventHandler _appearanceChanged = null;
        //при возникновении исключения AdomdConnection
        private PivotDataAdomdExceptionEventHandler _adomdExceptionReceived = null;
        private PivotDataEventHandler _structureChanged = null;
        //при изменении порядка следования элементов оси
        private PivotDataChangeOrderEventHandler _elementsOrderChanged = null;
        private PivotDataChangeSortEventHandler _elementsSortChanged = null;

        /// <summary>
        /// при изменении структуры данных
        /// </summary>
        public event PivotDataEventHandler DataChanged
        {
            add { _dataChanged += value; }
            remove { _dataChanged -= value; }
        }

        /// <summary>
        /// при изменении вида структуры в редакторе(используется для отложенного обновления макета)
        /// </summary>
        public event PivotDataEventHandler ViewChanged
        {
            add { _viewChanged += value; }
            remove { _viewChanged -= value; }
        }

        /// <summary>
        /// при изменении выбранного объекта
        /// </summary>
        public event PivotDataEventHandler SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        /// <summary>
        /// при изменении внешнего вида какого либо объекта
        /// </summary>
        public event PivotDataAppChangeEventHandler AppearanceChanged 
        {
            add { _appearanceChanged += value; }
            remove { _appearanceChanged -= value; }
        }

        /// <summary>
        /// при возникновении исключения AdomdConnection
        /// </summary>
        public event PivotDataAdomdExceptionEventHandler AdomdExceptionReceived
        {
            add { _adomdExceptionReceived += value; }
            remove { _adomdExceptionReceived -= value; }
        }


        /// <summary>
        /// при изменении структуры отображения (применятся в случаях, когда нужна 
        /// переинициализация элемента, не требуещего запроса данных с сервера)
        /// </summary>
        public event PivotDataEventHandler StructureChanged
        {
            add { _structureChanged += value; }
            remove { _structureChanged -= value; }
        }


        public PivotDataChangeOrderEventHandler ElementsOrderChanged
        {
            get { return _elementsOrderChanged; }
            set { _elementsOrderChanged = value; }
        }

        public PivotDataChangeSortEventHandler ElementsSortChanged
        {
            get { return _elementsSortChanged; }
            set { _elementsSortChanged = value; }
        }



        public void DoDataChanged()
        {
            if (!IsDeferDataUpdating)
            {
                _dataChanged();
            }
            else
            {
                if (_viewChanged != null)
                {
                    _viewChanged();
                }
            }
        }

        public void DoForceDataChanged()
        {
            if (_dataChanged != null)
            {
                _dataChanged();
            }
        }

        public void DoAppearanceChanged(bool isNeedRecalculateGrid)
        {
            if (!IsDeferDataUpdating)
            {
                _appearanceChanged(isNeedRecalculateGrid);
            }
        }

        public void DoForceAppearanceChanged(bool isNeedRecalculateGrid)
        {
            _appearanceChanged(isNeedRecalculateGrid);
        }


        public void DoElementsOrderChanged(PivotAxis pivotAxis)
        {
            if (IsDeferDataUpdating)
                return;

            if (_elementsOrderChanged != null)
                _elementsOrderChanged(pivotAxis);
        }

        public void DoElementsSortChanged(PivotAxis pivotAxis)
        {
            if (IsDeferDataUpdating)
                return;
            if (_elementsSortChanged != null)
                _elementsSortChanged(pivotAxis);
        }

        public void DoStructureChanged()
        {
            if (IsDeferDataUpdating)
                return;
            if (_structureChanged != null)
                _structureChanged();
        }

        public void DoAdomdExceptionReceived(Exception exc)
        {
            if (_adomdExceptionReceived != null)
            {
                if ((exc is AdomdException)||(exc.Source == "Microsoft.AnalysisServices.AdomdClient"))
                {
                    _adomdExceptionReceived(exc);
                }
                else
                {
                    Common.CommonUtils.ProcessException(exc);
                }
            }
        }

        #endregion

        public PivotData(PivotDataType type)
        {
            this.Type = type;

            columnAxis = new PivotAxis(this, AxisType.atColumns);
            rowAxis = new PivotAxis(this, AxisType.atRows);
            filterAxis = new PivotAxis(this, AxisType.atFilters);
            totalAxis = new TotalAxis(this);

            axes = new List<Axis>();
            axes.Add(columnAxis);
            axes.Add(rowAxis);
            axes.Add(filterAxis);
            axes.Add(totalAxis);

            filterAxis.Caption = "Фильтры";

            this.selection = null;
            this._parentPivotData = this;
            this.objectType = PivotObjectType.poNone;

            this.memberList = new MemberList();
            //this._filterForm = new MembersFilterForm();

            this.isDeferDataUpdating = false;
            //по умолчанию вычисление итогов будет по видимым элементам
            this._isVisualTotals = true;
            //динамическая загрузка данных реализована только для таблицы
            this._dynamicLoadData = false;//(type == PivotDataType.Table);

            this._averageSettings = new AverageSettings();
            this._medianSettings = new MedianSettings();
            this._topCountSettings = new TopCountSettings();
            this._bottomCountSettings = new BottomCountSettings();
            this.ClsInfo = new CellSetInfo(this);
        }

        ~PivotData()
        {
            //AdomdConn = null;
        }

        /// <summary>
        /// Вынести список элементов на первый план, если он открыт
        /// </summary>
        public void MemberListSetTop()
        {
            if (this.memberList != null)
            {
                this.memberList.SetTopIsVisible();
            }
        }

        /// <summary>
        /// Получение имени для меры отклонения от среднего
        /// </summary>
        /// <param name="measureName">мера, для которой считается отклонение</param>
        /// <returns></returns>
        public static string GetAverageDeviationMeasureName(string measureName)
        {
            return string.Format("[Measures].[Отклонение ({0})]", GetNameFromUniqueName(measureName));
        }

        /// <summary>
        /// Признак того что требуется вычислять среднее значение
        /// </summary>
        /// <returns></returns>
        public bool IsNeedAverageCalculation()
        {
            return ((this.RowAxis.FieldSets.Count > 0) &&
                    (this.AverageSettings.AverageType != AverageType.None) &&
                    (this.TotalAxis.Totals.Count > 0));
        }


        /// <summary>
        /// Признак того что требуется вычислять медиану
        /// </summary>
        /// <returns></returns>
        public bool IsNeedMedianCalculation()
        {
            return ((this.RowAxis.FieldSets.Count > 0) &&
                    (this.MedianSettings.IsMedianCalculate) &&
                    (this.TotalAxis.Totals.Count > 0));
        }


        /// <summary>
        /// Признак того что требуется вычислять k-первых
        /// </summary>
        /// <returns></returns>
        public bool IsNeedTopCountCalculation()
        {
            return ((this.RowAxis.FieldSets.Count > 0) &&
                    (this.TopCountSettings.IsTopCountCalculate) &&
                    (this.TotalAxis.Totals.Count > 0));
        }


        /// <summary>
        /// Признак того что требуется вычислять k-последних
        /// </summary>
        /// <returns></returns>
        public bool IsNeedBottomCountCalculation()
        {
            return ((this.RowAxis.FieldSets.Count > 0) &&
                    (this.BottomCountSettings.IsBottomCountCalculate) &&
                    (this.TotalAxis.Totals.Count > 0));
        }

        /// <summary>
        /// Очистка структуры
        /// </summary>
        public void Clear()
        {
            foreach (Axis axis in axes)
            {
                axis.Clear();
            }
            this.SelectedObject = null;
        }

        public void Load(XmlNode settingsNode, bool isForceDataUpdate)
        {
            bool customDataUpdating = XmlHelper.GetBoolAttrValue(settingsNode, Consts.deferDataUpdating, false);
            IsDeferDataUpdating = true;
            Clear();

            this._isVisualTotals = XmlHelper.GetBoolAttrValue(settingsNode, Consts.isVisualTotals, true);
            this.IsCustomMDX = XmlHelper.GetBoolAttrValue(settingsNode, Consts.isCustomMDX, false);
            this.MDXQuery = XmlHelper.GetStringAttrValue(settingsNode, Consts.mdxQuery, string.Empty);
            this.DynamicLoadData = XmlHelper.GetBoolAttrValue(settingsNode, Consts.dynamicLoadData, false);
            this.AverageSettings.Load(settingsNode.SelectSingleNode(Consts.averageSettings));
            this.MedianSettings.Load(settingsNode.SelectSingleNode(Consts.medianSettings));
            this.TopCountSettings.Load(settingsNode.SelectSingleNode(Consts.topCountSettings));
            this.BottomCountSettings.Load(settingsNode.SelectSingleNode(Consts.bottomCountSettings));


            if (settingsNode != null)
            {
                foreach (Axis axis in Axes)
                {
                    axis.SetXml(settingsNode.SelectSingleNode("axis[@type = '" + axis.AxisType.ToString() + "']"));
                }
            }
            this.IsDeferDataUpdating = customDataUpdating;

            CheckContent();
            this.RefreshIncludingToQuery();
            if (isForceDataUpdate)
            {
                _dataChanged();
            }
        }

        public void Save(XmlNode settingsNode)
        {
            XmlNode axisSettings, axisNode;

            XmlHelper.SetAttribute(settingsNode, Consts.deferDataUpdating, this.IsDeferDataUpdating.ToString());
            XmlHelper.SetAttribute(settingsNode, Consts.isVisualTotals, this.IsVisualTotals.ToString());
            XmlHelper.SetAttribute(settingsNode, Consts.isCustomMDX, this.IsCustomMDX.ToString());
            XmlHelper.SetAttribute(settingsNode, Consts.mdxQuery, this.MDXQuery);
            XmlHelper.SetAttribute(settingsNode, Consts.dynamicLoadData, this.DynamicLoadData.ToString());

            this.AverageSettings.Save(XmlHelper.AddChildNode(settingsNode, Consts.averageSettings));
            this.MedianSettings.Save(XmlHelper.AddChildNode(settingsNode, Consts.medianSettings));
            this.TopCountSettings.Save(XmlHelper.AddChildNode(settingsNode, Consts.topCountSettings));
            this.BottomCountSettings.Save(XmlHelper.AddChildNode(settingsNode, Consts.bottomCountSettings));

            foreach (Axis axis in Axes)
            {
                axisNode = XmlHelper.AddChildNode(settingsNode, "axis", new string[] { "type", axis.AxisType.ToString()});

                axis.SaveSettingsXml(axisNode);
            }
         
        }

        public void Initialize(CellSet cls)
        {
            //получаем нужную нам информацию из cls
            bool isCreateMeasureSections = (this.PivotDataType == PivotDataType.Table);
            this.ClsInfo.Initialize(cls, isCreateMeasureSections);
            //если запрос пользовательсий, инициализируем пивот дату по селсету
            if (this.IsCustomMDX)
            {
                this.InitializeByClsInfo(this.ClsInfo);
            }
            
        }

        /// <summary>
        /// Инициализируем пивот дату по селсету
        /// </summary>
        /// <param name="clsInfo"></param>
        public void InitializeByClsInfo(CellSetInfo clsInfo)
        {
            this.RowAxis.InitialByClsInfo(clsInfo.RowsInfo);
            this.ColumnAxis.InitialByClsInfo(clsInfo.ColumnsInfo);
            this.TotalAxis.InitialByClsInfo(clsInfo.MeasuresSectionsInfo);

            this.RowAxis.ClearAxis(clsInfo.RowsInfo);
            this.ColumnAxis.ClearAxis(clsInfo.ColumnsInfo);
        }

        public void InitFieldSetMemberNames(FieldSet fs, DimensionInfo dimInfo)
        {
            if (fs != null)
            {
                Hierarchy h = fs.AdomdHierarchy;
                if (h != null)
                {
                    try
                    {
                        fs.MemberNames = memberList.InitMemberList(h.Levels[0].GetMembers(), dimInfo);
                    }
                    catch(Exception exc)
                    {
                        DoAdomdExceptionReceived(exc);
                    }
                }
            }
        }

        /// <summary>
        /// Из текста MDX запроса извлекаем имя куба
        /// </summary>
        /// <returns></returns>
        private string RetrieveCubeNameFromMDX(string query)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(query))
            {
                string pattern = @"from[\s]*\[(?<cubeName>[\S\s]*?)\]";
                MatchCollection cubeName = Regex.Matches(query, pattern, RegexOptions.IgnoreCase);
                if (cubeName.Count > 0)
                    result = cubeName[0].Groups["cubeName"].Value;
            }
            return result;
        }

        /// <summary>
        /// Получение типа оси, на которой нах-ся элемент
        /// </summary>
        /// <param name="objName">Юникнейм элемента</param>
        /// <returns>тип оси</returns>
        public AxisType GetAxisTypeByObjectName(string objName)
        {
            foreach (Axis axis in Axes)
            {
                if (axis.FieldSets.ObjectIsPreset(objName))
                {
                    return axis.AxisType;
                }
            }

            return AxisType.atNone;
        }

        /// <summary>
        /// Получение оси по типу
        /// </summary>
        /// <param name="axisType">тип оси</param>
        /// <returns>ось</returns>
        public Axis GetAxis(AxisType axisType)
        {
            foreach (Axis result in Axes)
            {
                if (result.AxisType == axisType)
                {
                    return result;
                }
            }
            return null;
        }


        /// <summary>
        /// Получение текущего списка имен мер 
        /// </summary>
        /// <returns></returns>
        public List<string> GetMeasureNames()
        {
            List<string> result = new List<string>();

            foreach (Measure measure in this.Cube.Measures)
            {
                result.Add(measure.UniqueName.ToUpper());
            }

            foreach (PivotTotal total in this.TotalAxis.Totals)
            {
                if (total.IsCustomTotal)
                {
                    result.Add(total.UniqueName.ToUpper());
                }
            }
            return result;
        }

        /// <summary>
        /// Есть ли в структуре элемента уже такая мера
        /// </summary>
        /// <returns></returns>
        public bool IsTotalNameExists(string measureCaption)
        {
            List<string> measureNames = this.GetMeasureNames();
            string currentName = String.Format("[Measures].[{0}]", measureCaption).ToUpper();
            return measureNames.Contains(currentName);
        }

        /// <summary>
        /// Получить количество отображаемых уровней в измерении, следующих после указанного
        /// </summary>
        /// <param name="dimensionUN">юник нейм измерения</param>
        /// <param name="levelUN">юник нейм уровня</param>
        /// <returns></returns>
        public int GetFollowUpLevelCount(string dimensionUN, string levelUN)
        {
            int result = 0;
            if ((dimensionUN == string.Empty) || (levelUN == string.Empty))
                return result;

            Axis axis = this.GetAxisByObjectName(dimensionUN);
            if (axis != null)
            {
                FieldSet dimension = axis.FieldSets.GetFieldSetByName(dimensionUN);
                if (dimension != null)
                {
                    //прошли ли нужный уровень
                    bool isMeetLevel = false;
                    foreach (PivotField level in dimension.Fields)
                    {
                        if (isMeetLevel)
                            result++;

                        if (level.UniqueName == levelUN)
                            isMeetLevel = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Проверяем видимость итогов у уровня измрения
        /// </summary>
        /// <param name="levelUN"></param>
        /// <returns></returns>
        public bool IsVisibleLevelTotal(string levelUN)
        {
            PivotField field = this.GetPivotField(levelUN);
            return (field != null) ? field.IsVisibleTotal : true;
        }

        /// <summary>
        /// Получение оси, на кот. нах-ся элемент
        /// </summary>
        /// <param name="objName">юникнейм элемента</param>
        /// <returns>ось</returns>
        public Axis GetAxisByObjectName(string objName)
        {

            foreach (Axis axis in Axes)
            {
                if (axis.FieldSets.ObjectIsPreset(objName))
                {
                    return axis;
                }
            }
            return null;
        }

        public bool ObjectIsPreset(string uniqueName)
        {
            foreach (Axis axis in Axes)
            {
                if (axis.FieldSets.ObjectIsPreset(uniqueName))
                {
                    return true;
                }
            }
            return false;
        }

        public PivotObject GetPivotObject(string uniqueName)
        {
            PivotObject obj = null;
            if (uniqueName == string.Empty)
                return obj;

            foreach (Axis axis in Axes)
            {
                obj = axis.GetPivotObject(uniqueName);

                if (obj != null)
                {
                    return obj;
                }
            }
            return obj;
            
        }

        public PivotField GetPivotField(string uniqueName)
        {
            PivotObject pivotObject = this.GetPivotObject(uniqueName);
            return ((pivotObject != null) && (pivotObject is PivotField)) ? (PivotField)pivotObject : null;
        }

        public FieldSet GetFieldSet(string uniqueName)
        {
            FieldSet result = this.RowAxis.FieldSets.GetFieldSetByName(uniqueName);
            if (result == null)
                result = this.ColumnAxis.FieldSets.GetFieldSetByName(uniqueName);
            if (result == null)
                result = this.FilterAxis.FieldSets.GetFieldSetByName(uniqueName);
            return result;
        }

        /// <summary>
        /// Добавление узла с настройками
        /// </summary>
        /// <param name="parentNode">родительский узел</param>
        /// <returns>добавленный узел с настройками</returns>
        public XmlNode AppendSettingsTo(XmlNode parentNode)
        {
            XmlNode xmlSettings = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "PivotData", null);
            xmlSettings.InnerXml = this.XmlSettings.InnerXml;
            parentNode.AppendChild(xmlSettings);
            return xmlSettings;
        }

        public Hierarchy GetAdomdHierarchy(string uniqueName)
        {
            try
            {
                if (this.Cube == null)
                    return null;

                DimensionCollection dims = this.Cube.Dimensions;
                foreach (Dimension dim in dims)
                {
                    foreach (Hierarchy h in dim.Hierarchies)
                    {
                        if (h.UniqueName == uniqueName)
                        {
                            return h;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                if (exc is AdomdException)
                {
                    _adomdExceptionReceived((AdomdException)exc);
                }
            }
            return null;
        }

        public Level GetAdomdLevel(string uniqueName)
        {
            try
            {
                if (this.Cube == null)
                    return null;

                DimensionCollection dims = this.Cube.Dimensions;
                foreach (Dimension dim in dims)
                {
                    foreach (Hierarchy h in dim.Hierarchies)
                    {
                        foreach(Level lev in h.Levels)
                        if (lev.UniqueName == uniqueName)
                        {
                            return lev;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                if (exc is AdomdException)
                {
                    _adomdExceptionReceived((AdomdException)exc);
                }
            }
            return null;
        }

        /// <summary>
        /// Показать список элементов иерархии
        /// </summary>
        /// <param name="hierUniqueName">уникальное имя иерархии</param>
        public void ShowMemberList(string hierUniqueName)
        {
            /*
            if (!this.IsExistsConnection)
            {
                FormException.ShowErrorForm(new Exception("MDXExpert-AdomdConnectionIsNull"),
                                            ErrorFormButtons.WithoutTerminate);
                return;
            }*/
            if (!CheckConnection())
                return;

            Axis axis = GetAxisByObjectName(hierUniqueName);
            if ((axis == null))
            {
                return;
            }

            if ((this.Type != Data.PivotDataType.Table) || (axis.AxisType != AxisType.atTotals))
            {
                FieldSet fs = axis.FieldSets.GetFieldSetByName(hierUniqueName);
                if (fs != null)
                {
                    Hierarchy h = GetAdomdHierarchy(hierUniqueName);
                    if (h != null)
                    {
                        try
                        {
                            if (memberList.GetMemberNames(h.Levels[0].GetMembers(), ref fs, false,
                                                          this.ParentPivotData.IsCustomMDX))
                            {
                                this.RefreshIncludingToQuery();
                                DoDataChanged();
                            }
                        }
                        catch(Exception exc)
                        {
                            DoAdomdExceptionReceived(exc);
                        }
                    }
                }
            }
        }

        public void ShowMemberListEx(Hierarchy h, ref FieldSet fs)
        {
            /*
            if (!this.IsExistsConnection)
            {
                FormException.ShowErrorForm(new Exception("MDXExpert-AdomdConnectionIsNull"), 
                                            ErrorFormButtons.WithoutTerminate);
                return;
            }*/
            if (!CheckConnection())
                return;

            if (h != null)
            {
                try
                {
                    memberList.GetMemberNames(h.Levels[0].GetMembers(), ref fs, false, false);
                }
                catch (Exception exc)
                {
                    DoAdomdExceptionReceived(exc);
                }

            }
        }

        public bool ShowFilters(Hierarchy h, ref FieldSet fs)
        {
            if (!CheckConnection())
                return false;

            if (h != null)
            {
                try
                {
                    if (h.UniqueName == "[Measures]")
                        return false;

                    return this.memberList.GetFilterMemberNames(h.Levels[0].GetMembers(), ref fs);
                }
                catch (Exception exc)
                {
                    DoAdomdExceptionReceived(exc);
                }

            }
            return false;
        }


        public void ShowMemberListEx(Hierarchy h, ref XmlNode memberNames)
        {
            if (!CheckConnection())
                return;

            if (h != null)
            {
                try
                {
                    memberList.GetMemberNames(h, ref memberNames);
                }
                catch (Exception exc)
                {
                    DoAdomdExceptionReceived(exc);
                }

            }
        }



        public void GetMemberNames(Hierarchy h, XmlNode memberNames, List<string> included, 
                                   List<string> excluded)
        {
            //if (this.IsExistsConnection)
            if (CheckConnection())
            {
                if (h != null)
                {
                    try
                    {
                        memberList.GetMemberNames(h.Levels[0].GetMembers(), memberNames, included, excluded, this, GetFieldSet(h.UniqueName));
                    }
                    catch (Exception exc)
                    {
                        DoAdomdExceptionReceived(exc);
                    }

                }
            }
        }

        /// <summary>
        /// Выбрать объект
        /// </summary>
        /// <param name="selectionType">тип выбранного элемента</param>
        /// <param name="uniqueName">юникнейм, если выбираем конкретный объект(если тип SingleObject)</param>
        public void SetSelection(SelectionType selectionType, string uniqueName)
        {
            if ((this.SelectionType == selectionType) && (this.Selection == uniqueName))
            {
                return;
            }

            this.selectionType = selectionType;
            if (selectionType == SelectionType.SingleObject)
            {
                this.selection = uniqueName;
            }
            else
            {
                this.selection = "";
            }
            this.SelectedObject = this.GetPivotObject(this.selection);

            if (this._selectionChanged != null)
                _selectionChanged();
        }

        /// <summary>
        /// Добавить уровень в структуру
        /// </summary>
        /// <param name="axisType">тип оси</param>
        /// <param name="hierarchyName">имя иерахии</param>
        /// <param name="levelName">имя добавляемого уровня</param>
        public void AddLevel(string dimensionUN, string levelUN)
        {
            if (!string.IsNullOrEmpty(dimensionUN) && !string.IsNullOrEmpty(levelUN))
            {
                Axis axis = this.GetAxisByObjectName(dimensionUN);
                if (axis != null)
                {
                    PivotField field = axis.FieldSets.GetFieldByName(levelUN);
                    if (field == null)
                    {
                        field = axis.FieldSets.AddField(dimensionUN, GetNameFromUniqueName(dimensionUN),
                            levelUN, GetNameFromUniqueName(levelUN));
                    }
                    if (!field.IsIncludeToQuery || !this.DynamicLoadData)
                    {
                        field.IsIncludeToQuery = true;
                        this.DoDataChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Обновим состояние влкюченности уровней в запрос
        /// </summary>
        public void RefreshIncludingToQuery()
        {
            this.RowAxis.RefreshIncludingToQuery();
            this.ColumnAxis.RefreshIncludingToQuery();
        }

        /// <summary>
        /// Исключить поле, из запроса
        /// </summary>
        /// <param name="pivotObject"></param>
        public void ExcludeOfQuery(PivotObject pivotObject)
        {
            switch (pivotObject.ObjectType)
            {
                case PivotObjectType.poField:
                    {
                        ((PivotField)pivotObject).IsIncludeToQuery = false;
                        break;
                    }
                case PivotObjectType.poFieldSet:
                    {
                        FieldSet set = ((FieldSet)pivotObject);
                        if (set.Fields.Count > 0)
                            set.Fields[0].IsIncludeToQuery = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// Состояние включенности в запрос уровня
        /// </summary>
        /// <param name="pivotObject"></param>
        public bool StateIncludingInQuery(PivotObject pivotObject)
        {
            switch (pivotObject.ObjectType)
            {
                case PivotObjectType.poField:
                    {
                        return ((PivotField)pivotObject).IsIncludeToQuery;
                    }
                case PivotObjectType.poFieldSet:
                    {
                        FieldSet set = ((FieldSet)pivotObject);
                        if (set.Fields.Count > 0)
                            return set.Fields[0].IsIncludeToQuery;
                        break;
                    }
            }
            return true;
        }

        /// <summary>
        /// Добавить в структуру уровень или измерение следующий за указанными
        /// </summary>
        /// <param name="curDimUN"></param>
        /// <param name="curLevelUN"></param>
        public void ExpandNextLevel(string curDimUN, string curLevelUN)
        {
            if (!CheckConnection())
                return;

            if (!string.IsNullOrEmpty(curDimUN) && !string.IsNullOrEmpty(curLevelUN))
            {
                FieldSet fieldSet = this.GetFieldSet(curDimUN);
                if (fieldSet == null)
                {
                    MessageBox.Show(
                        string.Format("Измерение \"{0}\" отсутствует в структуре таблицы.", curDimUN),
                        "MDXExpert 3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                PivotField pivotField = fieldSet.GetFieldByName(curLevelUN);
                if ((fieldSet != null) && (pivotField != null))
                {
                    //требуется добавить новый уровень в измерение
                    if (fieldSet.IsLastSetInAxis && pivotField.IsLastFieldInSet)
                    {
                        string onlyLevelName = GetNameFromUniqueName(curLevelUN);
                        int curLevelNumber = fieldSet.AdomdHierarchy.Levels[onlyLevelName].LevelNumber;
                        //если 
                        if (fieldSet.AdomdHierarchy.Levels.Count <= curLevelNumber + 1)
                            return;
                        string nextLevelUN = fieldSet.AdomdHierarchy.Levels[curLevelNumber + 1].UniqueName;
                        this.AddLevel(curDimUN, nextLevelUN);
                    }
                    else
                    {
                        if (this.DynamicLoadData)
                        {
                            FieldSetCollection fsCollection = fieldSet.ParentCollection;
                            //требуется добавить уровень в следующем измерение
                            if (pivotField.IsLastFieldInSet)
                            {
                                int curFieldSetNumber = fsCollection.IndexOf(fieldSet);
                                FieldSet nextFieldSet = fsCollection[curFieldSetNumber + 1];
                                PivotField nextPivoField = nextFieldSet.Fields[0];
                                this.AddLevel(nextFieldSet.UniqueName, nextPivoField.UniqueName);
                            }
                                //требуется добавить cледующий уровень в этом измерение
                            else
                            {
                                int curLevelIndex = fieldSet.Fields.IndexOf(pivotField);
                                PivotField nextPivoField = fieldSet.Fields[curLevelIndex + 1];
                                this.AddLevel(fieldSet.UniqueName, nextPivoField.UniqueName);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Выбрать тип сортировки у объекта
        /// </summary>
        /// <param name="uniqueName">уникальное имя объекта</param>
        /// <param name="sortType">тип сортировки</param>
        public void ChangeObjectSort(string uniqueName, SortType sortType)
        {
            this.ChangeObjectSort(uniqueName, sortType, string.Empty);
        }

        /// <summary>
        /// Выбрать тип сортировки у объекта
        /// </summary>
        /// <param name="uniqueName">уникальное имя объекта</param>
        /// <param name="sortType">тип сортировки</param>
        /// <param name="sortedTupleUN">если сортируем меру, указываеться UN кортежа, по 
        /// которому и идет сортировка</param>
        public void ChangeObjectSort(string uniqueName, SortType sortType, string sortedTupleUN)
        {
            if (this.IsCustomMDX)
            {
                MessageBox.Show("В таблице построенной по пользовательскому MDX запросу, изменение сортировки не возможно.",
                    "MDXExpert 3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PivotObject pivotObject = this.GetPivotObject(uniqueName);
            if (pivotObject != null)
            {
                switch (pivotObject.ObjectType)
                {
                    case PivotObjectType.poField:
                        {
                            PivotField field = (PivotField)pivotObject;
                            field.SortType = sortType;
                            break;
                        }
                    case PivotObjectType.poFieldSet:
                        {
                            FieldSet fieldSet = (FieldSet)pivotObject;
                            fieldSet.SortType = sortType;
                            break;
                        }
                    case PivotObjectType.poTotal:
                        {
                            PivotTotal total = (PivotTotal)pivotObject;
                            total.SortedTupleUN = sortedTupleUN;
                            total.SortType = sortType;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// При выборе сортировке, будем смотреть не была ли она задана ранее у дрого измерения 
        /// или показателя, если была, сбрасываем признак.
        /// </summary>
        /// <param name="sender"></param>
        public void RefreshTypeSort(PivotObject sender)
        {
            switch (sender.ObjectType)
            {
                case PivotObjectType.poTotal:
                    {
                        this.TotalAxis.RefreshTypeSort(sender);
                        this.RowAxis.RefreshTypeSort(sender);
                        break;
                    }
                case PivotObjectType.poField:
                case PivotObjectType.poFieldSet:
                    {
                        bool isRowAxis = sender.ObjectType == PivotObjectType.poField ?
                            this.RowAxis.FieldSets.FieldIsPreset(sender) :
                            this.RowAxis.FieldSets.FieldSetIsPreset(sender);

                        if (isRowAxis)
                        {
                            this.TotalAxis.RefreshTypeSort(null);
                            this.RowAxis.RefreshTypeSort(sender);
                        }
                        else
                        {
                            this.ColumnAxis.RefreshTypeSort(sender);
                        }
                        break;
                    }
                case PivotObjectType.poAxis:
                    {
                        PivotAxis axis = (PivotAxis)sender;
                        if (axis.AxisType == AxisType.atRows)
                        {
                            this.TotalAxis.RefreshTypeSort(null);
                            this.RowAxis.FieldSets.RefreshSortType(sender);
                        }
                        if (axis.AxisType == AxisType.atColumns)
                        {
                            this.ColumnAxis.FieldSets.RefreshSortType(sender);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Сбросить сортировку у объекта
        /// </summary>
        /// <param name="sender"></param>
        public void ResetTypeSort(PivotObject sender)
        {
            switch (sender.ObjectType)
            {
                case PivotObjectType.poTotal:
                    {
                        PivotTotal total = (PivotTotal)sender;
                        total.SetSortTypeWithoutRefresh(SortType.None);
                        total.SortedTupleUN = string.Empty;
                        break;
                    }
                case PivotObjectType.poFieldSet:
                    {
                        FieldSet fieldSet = (FieldSet)sender;
                        bool isColumnAxis = this.ColumnAxis.FieldSets.FieldSetIsPreset(sender);
                        //если удаляем элемент колонок, у показателей сбрасываем признак сортировки,
                        //т.к. он был выствлен для конкретного кортежа, но произошли изменения в оси
                        //и этого кортежа больше не стало
                        if (isColumnAxis)
                            this.TotalAxis.RefreshTypeSort(null);
                        fieldSet.SetSortTypeWithoutRefresh(SortType.None);
                        break;
                    }
            }
        }

        /// <summary>
        /// установка фильтра по выделенному элементу
        /// </summary>
        /// <param name="member">элемент, по которому фильтруем</param>
        public void FilterBySelected(Member member)
        {
            if (member == null)
            {
                return;
            }

            if (!CheckConnection())
                return;


            FieldSet fs = GetFieldSet(member.ParentLevel.ParentHierarchy.UniqueName);
            if (fs == null)
            {
                return;
            }

            List<Member> mbrList = new List<Member>();

            try
            {
                while (member != null)
                {
                    mbrList.Add(member);
                    member = member.Parent;
                }
            }
            catch (Exception exc)
            {
                DoAdomdExceptionReceived(exc);
            }

            XmlNode xmlRoot = new XmlDocument().CreateNode(XmlNodeType.Element, "dummy", null);
            XmlNode parentNode = xmlRoot;

            for (int i = mbrList.Count - 1; i >= 0; i--)
            {
                XmlNode node = XmlHelper.AddChildNode(parentNode, "member", new string[]{ "uname", mbrList[i].UniqueName });
                XmlHelper.SetAttribute(parentNode, "childrentype", "included");
                parentNode = node;
            }

            fs.MemberNames = xmlRoot;

            DoDataChanged();
        }

        /// <summary>
        /// Вернет уникальное имя первого показателя
        /// </summary>
        /// <returns></returns>
        public string GetCurrentTotalID()
        {
            string result = string.Empty;
            //Если есть коллекция показателей то берем из нее
            if (this.TotalAxis.Totals.Count > 0)
            {
                result = this.TotalAxis.Totals[0].UniqueName;
            }
            //иначе, будем смотреть не размещены ли меры среди других осей
            else
            {
                FieldSet totalSet = this.GetFieldSet("[Measures]");
                if (totalSet != null)
                    result = totalSet.GetFirstIncludeMemberID();
                else
                    result = "[Measures].CurrentMember";
            }
            return result;
        }

        /// <summary>
        /// Вернет уникальные имена показателей
        /// </summary>
        /// <returns></returns>
        public List<string> GetTotalsID()
        {
            List<string> result = new List<string>();
            if (this.TotalAxis.Totals.Count > 0)
            {
                foreach (PivotTotal total in this.TotalAxis.Totals)
                {
                    result.Add(total.UniqueName);
                }
            }
            //иначе, будем смотреть не размещены ли меры среди других осей
            else
            {
                FieldSet totalSet = this.GetFieldSet("[Measures]");
                if (totalSet != null)
                    result = totalSet.GetIncludeMembersID();
                else
                    result.Add("[Measures].CurrentMember");
            }
            return result;
        }

        public void RefreshMetadata()
        {
            AdomdConn.RefreshMetadata();
        }

        #region тестовые методы для записи/чтения настроек в файл

        public void SavePivotDataSettings(string fileName)
        {
            string fullfileName = string.Format(@"{0}\{1}", Path.GetDirectoryName(Application.ExecutablePath),
                                                fileName);
            FileStream stream = null;

            stream = new FileStream(fullfileName, FileMode.Create, FileAccess.Write, FileShare.None);

            XmlSerializer xmlFormat = new XmlSerializer(typeof(XmlNode));
            xmlFormat.Serialize(stream, XmlSettings);
            stream.Close();
        }

        public void LoadPivotDataSettings(string fileName)
        {
            string fullfileName = string.Format(@"{0}\{1}", Path.GetDirectoryName(Application.ExecutablePath),
                                                fileName);

            FileStream stream = null;

            stream = new FileStream(fullfileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer xmlFormat = new XmlSerializer(typeof(XmlNode));

            XmlSettings = ((XmlNode)xmlFormat.Deserialize(stream));
            stream.Close();
        }

        /// <summary>
        /// Определяем версию сервиса анализа
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static AnalysisServicesVersion GetAnalysisVersion(AdomdConnection connection)
        {
            if (connection == null)
                return AnalysisServicesVersion.v2000;
            else
            {
                int version;
                Int32.TryParse(connection.ServerVersion.Substring(0, connection.ServerVersion.IndexOf('.')),
                    out version);
                switch (version)
                {
                    case 8: return AnalysisServicesVersion.v2000;
                    case 9: return AnalysisServicesVersion.v2005;
                    case 10: return AnalysisServicesVersion.v2008;
                    default: return AnalysisServicesVersion.v2000;
                }
            }
        }

        #endregion

        #region проверка содержимого пивот даты

        //очищаем имя от скобок
        public static string ClearBrackets(string source)
        {
            source = source.Replace("[", "");
            //source = source.Replace(']', ' ');
            int lastBracketPos = source.LastIndexOf(']');
            if (lastBracketPos > 0)
            {
                source = source.Remove(lastBracketPos);
            }
            return source; //.Trim();
        }

        //получение имени объекта по юникнейму
        public static string GetNameFromUniqueName(string uniqueName)
        {
            string[] arrStr = uniqueName.Split('[');
            if (arrStr.Length > 0)
            {
                return ClearBrackets(arrStr[arrStr.Length - 1]);
            }

            return uniqueName;
        }

        //проверка мембер пропертис
        private void CheckProperties(PivotField field, Level level)
        {
            List<string> allProperties = new List<string>();
            foreach (LevelProperty property in level.LevelProperties)
            {
                allProperties.Add(property.Name);
            }

            for (int i = 0; i < field.MemberProperties.VisibleProperties.Count; i++)
            {
                if (!allProperties.Contains(field.MemberProperties.VisibleProperties[i]))
                {
                    field.MemberProperties.VisibleProperties.Remove(field.MemberProperties.VisibleProperties[i]);
                    i--;
                }
            }
        }

        public Hierarchy GetHierarchyByUniqueName(string uniqueName)
        {
            CubeDef cube = this.Cube;
            if (cube == null)
            {
                return null;
            }

            foreach (Dimension dim in cube.Dimensions)
            {
                foreach (Hierarchy h in dim.Hierarchies)
                {
                    if (h.UniqueName == uniqueName)
                    {
                        return h;
                    }
                }
            }
            return null;
        }

        public Level GetLevelByUniqueName(string uniqueName)
        {
            CubeDef cube = this.Cube;
            if (cube != null)
            {
                foreach (Dimension dim in cube.Dimensions)
                {
                    foreach (Hierarchy h in dim.Hierarchies)
                    {
                        foreach (Level level in h.Levels)
                        {
                            if (level.UniqueName == uniqueName)
                            {
                                return level;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static Member GetMemberByUniqueName(Hierarchy parentHierarchy, string uniqueName)
        {
            /*
            if (parentHierarchy == null)
                return null;

            foreach (Level level in parentHierarchy.Levels)
            {
                string memberName = GetNameFromUniqueName(uniqueName);
                Microsoft.AnalysisServices.AdomdClient.MemberFilter memFilter = new Microsoft.AnalysisServices.AdomdClient.MemberFilter("UniqueName", uniqueName);
                Member member = level.GetMembers(0, 1, memFilter).Find(memberName);
                if (member != null)
                    return member;
            }
            return null;
             */
            Member mbr = (Member)parentHierarchy.ParentDimension.ParentCube.GetSchemaObject(SchemaObjectType.ObjectTypeMember, uniqueName);
            return mbr;
        }

        //проверить объект пивот даты на наличие в кубе
        public bool CheckCubeObject(PivotObject pivotObj)
        {
            CubeDef cube = this.Cube;

            switch (pivotObj.ObjectType)
            {
                case PivotObjectType.poFieldSet:
                    foreach (Dimension dim in cube.Dimensions)
                    {
                        if (dim.Hierarchies.Find(GetNameFromUniqueName(((FieldSet)pivotObj).UniqueName)) != null)
                        {
                            return true;
                        }
                    }
                    break;

                case PivotObjectType.poField:
                    Level level = GetLevelByUniqueName(((PivotField)pivotObj).UniqueName);
                    if (level != null)
                    {
                        CheckProperties((PivotField)pivotObj, level);
                        return true;
                    }
                    break;

                case PivotObjectType.poTotal:
                    try
                    {
                        foreach (Member member in cube.Dimensions["Measures"].Hierarchies[0].Levels[0].GetMembers())
                        {
                            if (member.UniqueName == ((PivotTotal)pivotObj).UniqueName)
                            {
                                return true;
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        DoAdomdExceptionReceived(exc);
                    }

                    break;
            }

            // MessageBox.Show("Объект " + uniqueName + " не найден");
            return false;
        }

        //возвращает уровень вложенности xml узла относительно корня мемберов
        private int GetNodeLevel(XmlNode node)
        {
            int result = -1;

            while (node.Name != "dummy")
            {
                result++;
                node = node.ParentNode;
            }
            
            return result;
        }

        //проверка: есть ли мембер, соответствующий xml узлу в заданной иерархии
        private bool CheckMember(XmlNode node, Hierarchy h)
        {
            string uniqueName = XmlHelper.GetStringAttrValue(node, "uname", "");
            //Если проверяем меры, то это может быть пользовательская мера, ее нет в базе, но удалять ее не надо
            if (h.UniqueName == "[Measures]")
            {
                if (this.TotalAxis.GetCustomTotalByName(uniqueName) != null)
                    return true;
            }
            //имя мембера
            string mbrName = GetNameFromUniqueName(uniqueName);

            if (mbrName != "")
            {
                //уровень на котором нах-ся мембер
                Level level = h.Levels[GetNodeLevel(node)];

                try
                {
                    string memberName = GetNameFromUniqueName(uniqueName);
                    Microsoft.AnalysisServices.AdomdClient.MemberFilter memFilter = new Microsoft.AnalysisServices.AdomdClient.MemberFilter("UniqueName", uniqueName);
                    Member member = level.GetMembers(0, 1, memFilter).Find(memberName);
                    return (member != null);
                }
                catch (Exception exc)
                {
                    DoAdomdExceptionReceived(exc);
                }

            }
            return true;
            
        }

        //корректировка xml структуры мемберов с учетом наличия в иерархии
        //узлы мемберов, кот. отсутствуют в иерархии удаляем
        private void CheckMembers(XmlNode node, Hierarchy h)
        {
            //ссылка на узел, кот. будем удалять
            XmlNode nodeForDel;

            while (node != null)
            {
                nodeForDel = null;
                if (!CheckMember(node, h))
                {
                    nodeForDel = node; 
                }
                else
                {
                    if (node.HasChildNodes)
                    {
                        CheckMembers(node.FirstChild, h);
                    }
                }
                node = node.NextSibling;

                if (nodeForDel != null)
                {
                    //если узел, кот. будем удалять - единственный ребенак, то мочим у родителя признак типа детей
                    if ((nodeForDel.ParentNode != null) && (nodeForDel.ParentNode.ChildNodes.Count == 1))
                    {
                        XmlHelper.RemoveAttribute(nodeForDel.ParentNode, "childrentype");
                    }

                    nodeForDel.CreateNavigator().DeleteSelf();
                }
            }
        }

        /// <summary>
        /// Проверка содержимого пивот даты 
        /// Удаление объектов структуры, не найденых в кубе
        /// </summary>
        public void CheckContent()
        {
            if (this.Cube == null)
            {
                return;
            }

            foreach (Axis axis in Axes)
            {
                if (axis.AxisType == AxisType.atTotals)
                {
                    //проверяем итоги на наличие в кубе
                    PivotTotal total;
                    for (int i = 0; i < ((TotalAxis)axis).Totals.Count; i++)
                    {
                        total = ((TotalAxis)axis).Totals[i];

                        if (!(total.IsCustomTotal) && !CheckCubeObject(total))
                        {
                            ((TotalAxis)axis).Totals.Remove(total);
                            i--; 
                        }
                    }

                    //проверяем на наличие дублированых элементов
                    PivotTotal cmprTotal;
                    for (int i = 0; i < ((TotalAxis)axis).Totals.Count; i++)
                    {
                        total = ((TotalAxis)axis).Totals[i];
                        for (int k = i + 1; k < ((TotalAxis)axis).Totals.Count; k++)
                        {
                            cmprTotal = ((TotalAxis)axis).Totals[k];
                            if(total.UniqueName == cmprTotal.UniqueName)
                            {
                                ((TotalAxis)axis).Totals.Remove(total);
                                i--;
                                break;
                            }
                        }
                    }

                    //У карт ось показателей ведет себя как обычная ось
                    if (this.PivotDataType != PivotDataType.Map)
                    {
                        ((TotalAxis) axis).RefreshMemberNames();
                    }
                }

                if ((axis.AxisType == AxisType.atColumns) || 
                    (axis.AxisType == AxisType.atFilters) || 
                    (axis.AxisType == AxisType.atRows) || 
                    ((axis.AxisType == AxisType.atTotals) && (this.PivotDataType == PivotDataType.Map))) //У карт ось показателей ведет себя как обычная ось
                {
                    FieldSet fs;
                    PivotField field;
                    for (int i = 0; i < axis.FieldSets.Items.Count; i++)
                    {
                        //ищем иерархии, соответствующие филдсетам 
                        fs = axis.FieldSets.Items[i];
                        if (!CheckCubeObject(fs))
                        {
                            axis.FieldSets.Items.Remove(fs);
                            i--;
                            continue;
                        }

                        //проверяем поля (уровни) на наличие в иерархии
                        for (int k = 0; k < fs.Fields.Count; k++)
                        {
                            field = fs.Fields[k];
                            if (!CheckCubeObject(field))
                            {
                                fs.Fields.Remove(field);
                                k--;
                                continue;
                            }
                        }

                        //Если нет ни одного поля (уровня) удаляем весь филдсет (иерархию)
                        if (fs.Fields.Count == 0)
                        {
                            axis.FieldSets.Items.Remove(fs);
                            i--;
                            continue;
                        }

                        //Проверяем мемберы на наличие в иерархии
                        CheckMembers(fs.MemberNames, GetAdomdHierarchy(fs.UniqueName));
                        //Если нет ни одного мембера удаляем весь филдсет (иерархию)
                        if ((fs.MemberNames != null) && (fs.MemberNames.ChildNodes.Count == 0))
                        {
                            axis.FieldSets.Items.Remove(fs);
                            i--;
                            continue;
                        }
                    }
                }
            }
        }

        #endregion

    }

    public enum PivotDataType
    {
        Chart,
        Table,
        Map,
        Gauge
    }

    /// <summary>
    /// тип сортировки (мер, измерений)
    /// </summary>
    public enum SortType
    {
        [System.ComponentModel.Description("Нет")]
        None,
        [System.ComponentModel.Description("По возрастанию")]
        ASC,
        [System.ComponentModel.Description("По убыванию")]
        DESC,
        [System.ComponentModel.Description("По возрастанию без учета иерархии")]
        BASC,
        [System.ComponentModel.Description("По убыванию без учета иерархии")]
        BDESC
    }

    /// <summary>
    /// Версия сервиса анализа
    /// </summary>
    public enum AnalysisServicesVersion
    {
        v2000,
        v2005,
        v2008
    }
}