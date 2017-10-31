using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Microsoft.AnalysisServices.AdomdClient;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.Data
{
    public class PivotField : PivotObject
    {
        #region поля

        private string _caption;
        private string _uniqueName;
        private MemberProperties _memberProperties;
        private bool _isVisibleTotal;
        private bool _isIncludeToQuery;
        private FieldSet _parentFieldSet;
        //private Level _adomdLevel;
        private SortType _sortType;
        private bool _isVisibleDataMember;

        #endregion

        public PivotField(Client.MDXExpert.Data.PivotData pivotData, string uniqueName, string caption)
            : this(null, pivotData, uniqueName, caption)
        {
        }

        public PivotField(FieldSet parentFieldSet, Client.MDXExpert.Data.PivotData pivotData, string uniqueName, string caption)
        {
            this.ParentFieldSet = parentFieldSet;
            this._uniqueName = uniqueName;
            this._caption = caption;
            this.objectType = PivotObjectType.poField;
            this._parentPivotData = pivotData;
            this.IsIncludeToQuery = !this.ParentPivotData.DynamicLoadData;
            //this._adomdLevel = this.ParentPivotData.GetAdomdLevel(this.UniqueName);

            this._memberProperties = new MemberProperties();
            this.InitMemberProperties();
            this._memberProperties.Changed += new MemberPropertiesEventHandler(MemberPropertiesChanged);

            //у таблицы итоги по умолчанию видны, у диаграммы нет
            this._isVisibleTotal = (pivotData.Type == PivotDataType.Table) ? true : false;
            this._isVisibleDataMember = true;
        }

        private void InitMemberProperties()
        {
            if (this.AdomdLevel == null)
                return;
            if (this.ParentFieldSet == null)
                return;

            //Задаем мембер пропертис для поля (берем из уровня) 
            foreach (LevelProperty prop in this.AdomdLevel.LevelProperties)
            {
                this.MemberProperties.AllProperties.Add(prop.Name);
                if ((this.ParentPivotData.PivotDataType == PivotDataType.Map) && (this.ParentFieldSet.AxisType == AxisType.atRows))
                {
                    if ((prop.Name == Consts.mapObjectCode) || (prop.Name == Consts.mapObjectName))
                    {
                        if (!this.MemberProperties.VisibleProperties.Contains(prop.Name))
                            this.MemberProperties.VisibleProperties.Add(prop.Name);
                    }
                }
            }

        }

        private void MemberPropertiesChanged()
        {
            this.ParentPivotData.DoDataChanged();
        }

        /// <summary>
        /// Получает следующий уровень из оси
        /// </summary>
        /// <returns></returns>
        public PivotField GetNextField()
        {
            if (this.IsLastFieldInSet)
            {
                if (this.ParentFieldSet.IsLastSetInAxis)
                    return null;
                else
                {
                    FieldSetCollection setCollection = this.ParentFieldSet.ParentCollection;
                    int currentSetIndex = setCollection.IndexOf(this.ParentFieldSet);
                    return setCollection[currentSetIndex + 1].Fields[0];
                }
            }
            else
            {
                int currentPivotIndex = this.ParentFieldSet.Fields.IndexOf(this);
                return this.ParentFieldSet.Fields[currentPivotIndex + 1];
            }
        }

        /// <summary>
        /// Установка типа сортировки уровне без последующего возбуждения события об этом
        /// </summary>
        /// <param name="sortType"></param>
        public void SetSortTypeWithoutRefresh(SortType sortType)
        {
            this._sortType = sortType;
        }

        #region свойства
        public Level AdomdLevel
        {
            get { return this.ParentPivotData.GetAdomdLevel(this.UniqueName); }
        }
        
        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                this.ParentPivotData.DoAppearanceChanged(false);
            }
        }

        public string UniqueName
        {
            get { return _uniqueName; }
        }

        public MemberProperties MemberProperties
        {
            get { return _memberProperties; }
            set { _memberProperties = value; }
        }

        /// <summary>
        /// Отображать ли у уровня итог
        /// </summary>
        public bool IsVisibleTotal
        {
            get { return _isVisibleTotal; }
            set 
            {
                if (value != _isVisibleTotal)
                {
                    _isVisibleTotal = value;

                    //this.ParentPivotData.DoStructureChanged();
                    //Сейчас для корректного отображения вычислимых мер требуется обновлять не только структуру
                    this.ParentPivotData.DoDataChanged();
                }
            }
        }

        public FieldSet ParentFieldSet
        {
            get { return _parentFieldSet; }
            set { _parentFieldSet = value; }
        }

        /// <summary>
        /// Включать ли уровень в запрос, этот признак учитывается при динамическом режиме 
        /// загрузки данных
        /// </summary>
        public bool IsIncludeToQuery
        {
            get { return _isIncludeToQuery || !this.ParentPivotData.DynamicLoadData; }
            set { _isIncludeToQuery = value; }
        }

        /// <summary>
        /// Является ли последним уровнем в сэте
        /// </summary>
        public bool IsLastFieldInSet
        {
            get
            {
                return (this == this.ParentFieldSet.Fields[this.ParentFieldSet.Fields.Count - 1]);
            }
        }

        public SortType SortType
        {
            get { return _sortType; }
            set
            {
                if (_sortType != value)
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
                    this.ParentPivotData.RefreshTypeSort(this);
                    if (!this.ParentPivotData.IsDeferDataUpdating)
                        this.ParentPivotData.DoDataChanged();
                }
            }
        }

        /// <summary>
        /// Отображать дата мембер
        /// </summary>
        public bool IsVisibleDataMember
        {
            get { return _isVisibleDataMember; }
            set
            {
                if (value != _isVisibleDataMember)
                {
                    _isVisibleDataMember = value;
                    this.ParentPivotData.DoDataChanged();
                }

            }
        }

        #endregion
    }
}
