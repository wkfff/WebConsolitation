using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Design.Editors
{
    /// <summary>
    /// Редактор коллекций IDictionary&lt;string, string&gt;
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class SessionStringEditor : CollectionEditor
    {
        /// <summary>
        /// Редактор коллекций IDictionary&lt;string, string&gt;
        /// </summary>
        public SessionStringEditor()
            : base(typeof(SessionStringEditor))
        {
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string) };
        }

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
            IDictionary<string, ISession> propertyValue = (IDictionary<string, ISession>)context.Instance.GetType().InvokeMember(context.PropertyDescriptor.Name, System.Reflection.BindingFlags.GetProperty, null, context.Instance, null);

            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                // Display an angle selection control and retrieve the value.
                SessionEditorForm dse = new SessionEditorForm(propertyValue);
                DialogResult dr = edSvc.ShowDialog(dse);
            }
            return value;
        }
    }
}
