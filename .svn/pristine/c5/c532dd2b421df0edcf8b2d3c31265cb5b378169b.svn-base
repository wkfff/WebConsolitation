using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert
{
    class TablePivotFieldBrowseAdapter : PivotFieldBrowseAdapter
    {

        public TablePivotFieldBrowseAdapter(Data.PivotField pivotField, CustomReportElement reportElement)
            : base(pivotField, reportElement)
        {
        }

        [Browsable(false)]
        public bool IsShowMeberProperties
        {
            get 
            { 
                return (this.CurrentPivotObject.MemberProperties.AllProperties.Count != 0) && 
                    !this.IsCustomMDX; 
            }
        }

        /// <summary>
        /// Отображать ли свойство сортировки
        /// </summary>
        [Browsable(false)]
        public bool IsDisplaySortType
        {
            get
            {
                return (!this.IsCustomMDX && this.CurrentPivotObject.ParentFieldSet.UniqueName != "[Measures]") &&
                    (this.CurrentPivotObject.ParentFieldSet.AxisType != AxisType.atFilters);
            }
        }

        [Category("Общие")]
        [DisplayName("Свойства элемента")]
        [Description("Выбор свойств элементов для отображения")]
        [Editor(typeof(MemberPropertiesDropDownEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("IsShowMeberProperties", "True")]
        public MemberProperties MemberProperties
        {
            get { return this.CurrentPivotObject.MemberProperties; }
            set 
            { 
                this.CurrentPivotObject.MemberProperties.VisibleProperties = value.VisibleProperties;
            }
        }

        [Category("Управление данными")]
        [DisplayName("Сортировка")]
        [Description("Вид сортировки измерения")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [DefaultValue(Data.SortType.None)]
        [DynamicPropertyFilter("IsDisplaySortType", "True")]
        public Data.SortType SortType
        {
            get { return this.CurrentPivotObject.SortType; }
            set { this.CurrentPivotObject.SortType = value; }
        }
    }

    public class MemberPropertiesDropDownEditor : UITypeEditor
    {

        /// <summary>
        /// Реализация метода редактирования
        /// </summary>
        public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
        {
            //MemberProperties result = null;
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (svc != null)
                {
                    MemberPropertiesControl flctrl = new MemberPropertiesControl((MemberProperties)value);
                    flctrl.Tag = svc;

                    svc.DropDownControl(flctrl);


                    //result = new MemberProperties();

                    value = flctrl.MemberProperties;


                }
            }

            return base.EditValue(context, provider, value); //result 
        }

        /// <summary>
        /// Возвращаем стиль редактора - выпадающее окно
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.DropDown;
            }
            else
            {
                return base.GetEditStyle(context);
            }
        }

    }

}
