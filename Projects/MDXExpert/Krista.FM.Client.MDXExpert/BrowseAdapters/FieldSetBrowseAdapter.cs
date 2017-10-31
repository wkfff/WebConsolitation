using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Krista.FM.Expert.PivotData;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Свойства измерения оси
    /// </summary>
    public class FieldSetBrowseAdapter : PivotObjectBrowseAdapterBase
    {
        public FieldSetBrowseAdapter(FieldSet fieldSet, CustomReportElement reportElement)
            : base(fieldSet, fieldSet.Caption, reportElement)
        {
        }

        #region Свойства

        private FieldSet CurrentPivotObject
        {
            get { return (FieldSet)base.PivotObject; }
        }

        [Category("Общие")]
        [DisplayName("Уникальное имя")]
        [Description("Уникальное имя измерения")]
        public string UniqueName
        {
            get 
            {
                return this.CurrentPivotObject.UniqueName; 
            }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок измерения")]
        public string Caption
        {
            get 
            {
                return this.CurrentPivotObject.Caption; 
            }
            set 
            {
                base.Header = value;
                this.CurrentPivotObject.Caption = value;
            }
        }

        [Category("Управление данными")]
        [DisplayName("Показывать итоги")]
        [Description("Показывать итоги, принадлежащие элементам данного измерения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("ElementType", "eTable")]
        [DefaultValue(true)]
        public bool IsTableVisibleTotals
        {
            get
            {
                return this.CurrentPivotObject.IsVisibleTotals;
            }
            set
            {
                this.CurrentPivotObject.IsVisibleTotals = value;
            }
        }

        [Category("Управление данными")]
        [DisplayName("Показывать нелистовые элементы")]
        [Description("Показывать нелистовые элементы")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("ElementType", "eChart")]
        [DefaultValue(false)]
        public bool IsChartVisibleTotals
        {
            get
            {
                return this.CurrentPivotObject.IsVisibleTotals;
            }
            set
            {
                this.CurrentPivotObject.IsVisibleTotals = value;
            }
        }

        #endregion
    }
}
