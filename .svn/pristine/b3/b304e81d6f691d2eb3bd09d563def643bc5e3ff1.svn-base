
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
    public class SyncEditor : UITypeEditor
    {

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    if (context.Instance is ChartReportElementBrowseAdapter)
                    {
                        SyncControl syncCtrl = new SyncControl((ChartSynchronization) value);
                        syncCtrl.Tag = svc;

                        svc.DropDownControl(syncCtrl);
                        if (syncCtrl.Changed)
                        {
                            ((ChartSynchronization)value).ChartElement.MainForm.Saved = false;
                            ((ChartSynchronization)value).BoundTo = syncCtrl.BoundTo;
                            ((ChartSynchronization)value).MeasureInRows = syncCtrl.MeasureInRows;
                            ((ChartSynchronization)value).ChartElement.Synchronize();
                            ((ChartSynchronization)value).ChartElement.MainForm.FieldListEditor.InitEditor(((ChartSynchronization)value).ChartElement);
                            ((ChartSynchronization) value).ChartElement.MainForm.PropertyGrid.Refresh();
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