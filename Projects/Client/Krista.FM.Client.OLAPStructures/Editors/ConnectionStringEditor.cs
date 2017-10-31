using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Krista.FM.Client.OlapAdmin.Connection;

namespace Krista.FM.Client.OLAPStructures.Editors
{
    public sealed class ConnectionStringEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            DataSourceProperty dataSourceControlBlock = value as DataSourceProperty;
            if (svc != null && dataSourceControlBlock != null)
            {
                string connectionFilePath =
                    Path.Combine(
                        Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                                     dataSourceControlBlock.DataSourceComponent.ParentServer.Name),
                        dataSourceControlBlock.DataSourceComponent.Parent.Name);
                using (ConnectionForm form = new ConnectionForm("DWH", connectionFilePath, dataSourceControlBlock.DataSourceComponent.ConnectionString))
                {
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                    }
                }
            }

            return value; // can also replace the wrapper object here
        }
    }
}
