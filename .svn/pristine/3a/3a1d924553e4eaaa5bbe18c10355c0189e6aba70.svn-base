
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
    public class MapSyncEditor : UITypeEditor
    {

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    if (context.Instance is MapReportElementBrowseAdapter)
                    {
                        MapSyncControl syncCtrl = new MapSyncControl((MapSynchronization) value);
                        syncCtrl.Tag = svc;

                        svc.DropDownControl(syncCtrl);
                        if (syncCtrl.Changed)
                        {
                            ((MapSynchronization) value).MapElement.MainForm.Saved = false;
                            ((MapSynchronization)value).BoundTo = syncCtrl.BoundTo;
                            ((MapSynchronization)value).ObjectsInRows = syncCtrl.ObjectsInRows;
                            ((MapSynchronization)value).MapElement.Synchronize();
                            ((MapSynchronization)value).MapElement.MainForm.FieldListEditor.InitEditor(((MapSynchronization)value).MapElement);
                            ((MapSynchronization) value).MapElement.MainForm.PropertyGrid.Refresh();
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