
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Infragistics.UltraChart.Resources.Editor;
//using Infragistics.UltraChart.Design;
using System.ComponentModel;
using System;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Win.UltraWinTabControl;

namespace Krista.FM.Client.MDXExpert
{
    public class GaugeSyncEditor : UITypeEditor
    {

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    if (context.Instance is GaugeReportElementBrowseAdapter)
                    {
                        GaugeSyncControl syncCtrl = new GaugeSyncControl((GaugeSynchronization) value);
                        syncCtrl.Tag = svc;

                        svc.DropDownControl(syncCtrl);
                        if (syncCtrl.Changed)
                        {
                            ((GaugeSynchronization) value).GaugeElement.MainForm.Saved = false;
                            ((GaugeSynchronization)value).BoundTo = syncCtrl.BoundTo;
                            ((GaugeSynchronization)value).IsCurrentColumnValues = syncCtrl.IsCurrentColumnValues;
                            ((GaugeSynchronization)value).GaugeElement.Synchronize();
                            ((GaugeSynchronization)value).GaugeElement.MainForm.FieldListEditor.InitEditor(((GaugeSynchronization)value).GaugeElement);
                        }
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
                return UITypeEditorEditStyle.DropDown;
            }
            else
            {
                return base.GetEditStyle(context);
            }
        }

    }
}