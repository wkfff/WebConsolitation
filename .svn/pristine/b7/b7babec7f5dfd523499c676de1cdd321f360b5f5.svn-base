using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Dundas.Maps.WinControl;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using Infragistics.UltraChart.Resources.Editor;


namespace Krista.FM.Client.MDXExpert
{
    public class ColorIntervalNameEditor : UITypeEditor
    {

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (context.Instance != null)&& (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    if ((value is CustomColorCollection)&&(context.Instance is ShapeRuleBrowseAdapter))
                    {
                        ShapeRuleBrowseAdapter shRuleBrowse = (ShapeRuleBrowseAdapter) context.Instance;

                        MapIntervalNameForm frm = new MapIntervalNameForm((CustomColorCollection)value, shRuleBrowse.MapElement);
                        frm.Tag = svc;

                        frm.ShowDialog();
                    }

                }
            }
            return base.EditValue(context, provider, value);
        }

        /// <summary>
        /// Возвращаем стиль редактора - выпадающее окно
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            else
            {
                return base.GetEditStyle(context);
            }
        }

    }
    
}
