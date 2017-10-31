
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
    public class MultiGaugeSyncEditor : UITypeEditor
    {

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    if (context.Instance is MultiGaugeReportElementBrowseAdapter)
                    {
                        MultiGaugeSyncControl syncCtrl = new MultiGaugeSyncControl((MultipleGaugeSynchronization)value);
                        syncCtrl.Tag = svc;

                        svc.DropDownControl(syncCtrl);
                        if (syncCtrl.Changed)
                        {
                            ((MultipleGaugeSynchronization)value).GaugeElement.MainForm.Saved = false;
                            ((MultipleGaugeSynchronization)value).BoundTo = syncCtrl.BoundTo;
                            ((MultipleGaugeSynchronization)value).GaugeElement.Synchronize();
                            ((MultipleGaugeSynchronization)value).GaugeElement.MainForm.FieldListEditor.InitEditor(((MultipleGaugeSynchronization)value).GaugeElement);
                            ((MultipleGaugeSynchronization) value).GaugeElement.MainForm.PropertyGrid.Refresh();
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