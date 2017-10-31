using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms.Design;
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraGauge.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    public abstract class CustomGaugeCollectionEditorBase : UITypeEditor
    {
        // Methods
        protected CustomGaugeCollectionEditorBase()
        {
        }

        [ComVisible(false), PermissionSet(SecurityAction.LinkDemand, Name = "linkcheck", Unrestricted = true)]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (((context != null) && (context.Instance != null)) && (provider != null))
            {
                PropertyInfo property = context.Instance.GetType().GetProperty(this.CollectionPropertyName);
                if (property != null)
                {
                    GaugeCollectionBase collection = property.GetValue(context.Instance, null) as GaugeCollectionBase;
                    if (collection != null)
                    {
                        object instance = context.Instance;
                        CustomGaugeCollectionEditorBaseForm dialog = new CustomGaugeCollectionEditorBaseForm(collection, context.PropertyDescriptor);
                        if (dialog != null)
                        {
                            dialog.Text = this.FormCaption;
                            dialog.SetItemTypes(this.ItemTypes);
                            dialog.SetTypeNames(this.TypeNames);
                            dialog.ForceMultiAdd = this.ForceMultiAdd;
                            IWindowsFormsEditorService service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                            if (service != null)
                            {
                                service.ShowDialog(dialog);
                            }
                        }
                    }
                }
            }
            return value;
        }

        [ComVisible(false), PermissionSet(SecurityAction.LinkDemand, Name = "linkcheck", Unrestricted = true)]
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if ((context != null) && (context.Instance != null))
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }

        [ComVisible(false), PermissionSet(SecurityAction.LinkDemand, Name = "linkcheck", Unrestricted = true)]
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        // Properties
        protected abstract string CollectionPropertyName { get; }

        protected virtual bool ForceMultiAdd
        {
            get
            {
                return false;
            }
        }

        protected abstract string FormCaption { get; }

        protected abstract Type[] ItemTypes { get; }

        protected abstract string[] TypeNames { get; }
    }


}
