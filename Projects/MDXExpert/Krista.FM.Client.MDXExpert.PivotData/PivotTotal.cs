using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;


namespace Krista.FM.Client.MDXExpert.Data
{
    public class PivotTotal : PivotObject
    {
        #region поля

        private string _caption;
        private string _uniqueName;
        private ValueFormat _format;
        private bool _isCalculate;
        private SortType _sortType = SortType.None;
        private string _sortedTupleUN;

        private bool _isCustomTotal;
        private string _expression = "0";
        private MeasureFormulaType _formulaType;
        private string _measureSource;

        private string _lookupCubeName;
        private XmlNode _filters = new XmlDocument().CreateNode(XmlNodeType.Element, "filters", null);
        private bool _isLookupMeasure;
        private bool _isVisible = true;

        #endregion 

        #region свойства

        public string Caption
        {
            get 
            { 
                return _caption; 
            }
            set 
            { 
                _caption = value;
                if (this.IsCustomTotal)
                {
                    this.ParentPivotData.TotalAxis.RefreshMemberNames();
                    this.ParentPivotData.DoDataChanged();
                }
                else
                {
                    this.ParentPivotData.DoAppearanceChanged(false);
                }
            }
        }

        public string UniqueName
        {
            get 
            {
                if (_isCustomTotal)
                {
                    return "[Measures].[" + this.Caption + "]"; 
                }
                else
                {
                    return _uniqueName;
                }
            }
        }

        public ValueFormat Format
        {
            get 
            { 
                return _format; 
            }
            set 
            { 
                _format = value;
                this.ParentPivotData.DoAppearanceChanged(false);
            }
        }

        /// <summary>
        /// Вычислимая мера, созданная пользователем
        /// </summary>
        public bool IsCustomTotal
        {
            get { return _isCustomTotal; }
            set { _isCustomTotal = value; }
        }

        /// <summary>
        /// Вычислимая мера из базы
        /// </summary>
        public bool IsCalculate
        {
            get 
            {
                return _isCalculate;
            }
            set
            {
                _isCalculate = value;
            }
        }

        /// <summary>
        /// Отображать или нет меру в отчете
        /// </summary>
        public bool IsVisible
        {
            get { return this._isVisible; }
            set
            {
                this._isVisible = value;
                this.ParentPivotData.TotalAxis.RefreshMemberNames();
                this.ParentPivotData.DoDataChanged();
            }
        }

        /// <summary>
        /// Выражение для вычислимой меры, созданной пользователем
        /// </summary>
        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value;
                this.ParentPivotData.DoDataChanged();
            }
        }

        public SortType SortType
        {
            get { return _sortType; }
            set
            {
                _sortType = value;
                this.ParentPivotData.RefreshTypeSort(this);
                this.ParentPivotData.DoDataChanged();
            }
        }



        /// <summary>
        /// Мера может быть разложена по многим элементам измерения, и тогда не понятно по какому 
        /// из них она должна сортироваться, здесь хранится UN кортежа, по которому будет произведена 
        /// сортировка
        /// </summary>
        public string SortedTupleUN
        {
            get { return _sortedTupleUN; }
            set { _sortedTupleUN = value; }
        }

        /// <summary>
        /// тип формулы для вычислимой меры
        /// </summary>
        public MeasureFormulaType FormulaType
        {
            get { return _formulaType; }
            set
            {
                _formulaType = value;
                this.ParentPivotData.DoDataChanged();
            }
        }

        /// <summary>
        /// мера - источник для вычислимой меры(используется в типовых вычислениях)
        /// </summary>
        public string MeasureSource
        {
            get { return _measureSource; }
            set { _measureSource = value; }
        }

        /// <summary>
        /// Имя куба (указывается когда мера выбрана из другого куба, например с помощью функции LookupCube)
        /// </summary>
        public string LookupCubeName
        {
            get { return _lookupCubeName; }
            set { _lookupCubeName = value; }
        }

        /// <summary>
        /// Частные фильтры для меры, выбранной из другого куба
        /// </summary>
        public XmlNode Filters
        {
            get { return _filters; }
            set { _filters = value; }
        }

        public bool IsLookupMeasure
        {
            get { return _isLookupMeasure; }
            set { _isLookupMeasure = value; }
        }

        #endregion

        public PivotTotal(PivotData pivotData, string uName, string caption, bool isCustomTotal, string expression, string measureSource, MeasureFormulaType formulaType)
        {
            this._parentPivotData = pivotData;
            this.objectType = PivotObjectType.poTotal;
            this.SortedTupleUN = string.Empty;

            this._isCustomTotal = isCustomTotal;
            this._caption = caption;

            this._expression = expression;
            this._uniqueName = uName;

            this._measureSource = measureSource;
            this._formulaType = formulaType;

            this._format = new ValueFormat();
            this._format.Changed += new ValueFormatEventHandler(ValueFormatChanged);
        }

        public PivotTotal(PivotData pivotData, string caption, string expression)
            :this(pivotData, string.Empty, caption, true, expression, string.Empty, MeasureFormulaType.Custom)
        {
        }
        
        private void ValueFormatChanged()
        {
            this.ParentPivotData.DoForceAppearanceChanged(false);
        }

        /// <summary>
        /// Установка типа сортировки показазателя без последующего возбуждения события об этом
        /// </summary>
        /// <param name="sortType"></param>
        public void SetSortTypeWithoutRefresh(SortType sortType)
        {
            this._sortType = sortType;
        }

        /// <summary>
        /// Установить флаг видимости без обновления
        /// </summary>
        /// <param name="value"></param>
        public void SetIsVisibleWithoutRefresh(bool value)
        {
            this._isVisible = value;
        }

        public override string ToString()
        {
            if(this.IsCustomTotal)
            {
                return this.Expression;
            }
            else
            {
                return this.UniqueName;
            }
        }
    }
}
