using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Krista.FM.Expert.PivotData;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;

namespace Krista.FM.Client.MDXExpert

{
    /// <summary>
    /// Свойства уровня оси
    /// </summary>
    public class PivotFieldBrowseAdapter : PivotObjectBrowseAdapterBase
    {

        public PivotFieldBrowseAdapter(PivotField pivotField, CustomReportElement reportElement)
            : base(pivotField, pivotField.Caption, reportElement)
        {
        }

        protected PivotField CurrentPivotObject
        {
            get { return (PivotField)base.PivotObject; }
        }

        [Category("Общие")]
        [DisplayName("Заголовок")]
        [Description("Заголовок уровня")]
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

        [Category("Общие")]
        [DisplayName("Уникальное имя")]
        [Description("Уникальное имя уровня")]
        public string UniqueName
        {
            get 
            {
                return this.CurrentPivotObject.UniqueName; 
            }
        }

        [Category("Управление данными")]
        [DisplayName("Показывать итоги")]
        [Description("Показывать итоги, принадлежащие элементам этого уровня")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DynamicPropertyFilter("ElementType", "eTable")]
        [DefaultValue(true)]
        public bool IsVisibleTotal
        {
            get
            {
                return this.CurrentPivotObject.IsVisibleTotal;
            }
            set
            {
                this.CurrentPivotObject.IsVisibleTotal = value;
            }
        }
    }
}
