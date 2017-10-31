using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using Krista.FM.ServerLibrary;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace Krista.FM.Client.Design.Editors
{
    /// <summary>
    /// Редактор настроики видов источника для классификаторов делящихся по источникам
    /// </summary>
    public class DataSourcesEditor : CollectionEditor
    {
        public DataSourcesEditor()
            : base(typeof(DataSourcesEditor))
        {
 
        }

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string propertyValue =
                (string)
                context.Instance.GetType().InvokeMember(context.PropertyDescriptor.Name,
                                                        System.Reflection.BindingFlags.GetProperty, null,
                                                        context.Instance, null);

            DataSourcesTreeForm form = DataSourcesTreeForm.InstanceForm(propertyValue);

            if (form.ShowDialog() == DialogResult.OK)
               return form.DataSourceControl.CheckedNodes();

            return value;
        }
    }
}
