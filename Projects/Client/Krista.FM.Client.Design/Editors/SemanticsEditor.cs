using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Krista.FM.ServerLibrary;
using System.Reflection;


namespace Krista.FM.Client.Design.Editors
{
    public class SemanticsEditor : CollectionEditor
    {
        public SemanticsEditor()
            : base(typeof(SemanticsEditor))
        { }

        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string) };
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        // Displays the UI for value selection.
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IDictionary<string, string> propertyValue = (IDictionary<string, string>)context.Instance.GetType().InvokeMember(context.PropertyDescriptor.Name, System.Reflection.BindingFlags.GetProperty, null, context.Instance, null);

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                  ListBox tbText = new ListBox();
                // настраиваем его свойства и задаем редактируемый текст.

                tbText.Size = new System.Drawing.Size(200, 95);
                tbText.ScrollAlwaysVisible = true;
                tbText.BorderStyle = BorderStyle.Fixed3D;

                foreach (KeyValuePair<string, string> item in propertyValue)
                {
                    tbText.Items.Add(item.Value);
                }
                edSvc.DropDownControl(tbText);
            }
            return value;
        }
    }
}
