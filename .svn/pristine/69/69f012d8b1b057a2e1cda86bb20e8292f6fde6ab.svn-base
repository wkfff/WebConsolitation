using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Krista.FM.Expert.PivotData;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Krista.FM.Client.MDXExpert.Controls;

namespace Krista.FM.Client.MDXExpert
{
    class TablePivotFieldBrowseAdapter : PivotFieldBrowseAdapter
    {

        public TablePivotFieldBrowseAdapter(PivotField pivotField, CustomReportElement reportElement)
            : base(pivotField, reportElement)
        {
        }

        [Browsable(false)]
        public bool IsEmptyMeberProperties
        {
            get { return this.CurrentPivotObject.MemberProperties.AllProperties.Count == 0; }
        }

        [Category("Общие")]
        [DisplayName("Свойства элемента")]
        [Description("Выбор свойств элементов для отображения")]
        [Editor(typeof(MemberPropertiesDropDownEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("IsEmptyMeberProperties", "False")]
        public MemberProperties MemberProperties
        {
            get
            {
                return this.CurrentPivotObject.MemberProperties;
            }
            set
            {
                this.CurrentPivotObject.MemberProperties.VisibleProperties = value.VisibleProperties;
            }
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
