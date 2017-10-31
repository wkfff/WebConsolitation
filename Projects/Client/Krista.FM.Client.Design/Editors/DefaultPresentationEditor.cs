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
    /// Редактор выбора представления по умолчанию
    /// </summary>
    public class DefaultPresentationEditor : CollectionEditor 
    {
        public DefaultPresentationEditor()
            : base(typeof(DefaultPresentationEditor))
        {
        }

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] {typeof(string)};
        }

        IWindowsFormsEditorService edSvc = null;

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IPresentationCollection collection = (IPresentationCollection)context.Instance;

            edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (edSvc == null)
                return value;

            ListBox lb = new ListBox();
            lb.SelectedValueChanged += new EventHandler(lb_SelectedValueChanged);

            lb.ScrollAlwaysVisible = true;
            lb.BorderStyle = BorderStyle.None;

            foreach (KeyValuePair<string, IPresentation> item in collection)
            {
                lb.Items.Add(new PresentationListBoxItem(item.Key, item.Value.Name));
            }

            edSvc.DropDownControl(lb);

            return (lb.SelectedItem == null) ? value : ((PresentationListBoxItem)lb.SelectedItem).Key;
        }

        void lb_SelectedValueChanged(object sender, EventArgs e)
        {
            if (edSvc != null)
                edSvc.CloseDropDown();
        }
    }

    public class PresentationListBoxItem
    {
        private string key;

        private string name;

        public PresentationListBoxItem(string key, string name)
        {
            this.key = key;
            this.name = name;
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }

        /// <summary>
        /// В интерфейс выводим имя представления
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }
    }
}
