using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Text;
using System.Xml;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Design.Editors
{
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class XmlViewEditor : UITypeEditor
    {
        /// <summary>
        /// Задает вид редактора свойства
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            string doc =(string)context.Instance.GetType().InvokeMember(context.PropertyDescriptor.Name, System.Reflection.BindingFlags.GetProperty, null, context.Instance, null);
            
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                // Display an angle selection control and retrieve the value.
                XmlViewEditorForm xve = new XmlViewEditorForm(doc);
                DialogResult dr = edSvc.ShowDialog(xve);
            }
            return value;
        }
    }
}
