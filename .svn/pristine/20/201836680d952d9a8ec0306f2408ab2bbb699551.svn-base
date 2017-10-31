using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Collections;

namespace Krista.FM.Client.MDXExpert
{
    public abstract class CustomCollectionForm : Form
    {
        // Fields
        private const short EditableDynamic = 0;
        private const short EditableNo = 2;
        private short editableState;
        private const short EditableYes = 1;
        private CustomCollectionEditor editor;
        private object value;

        // Methods
        public CustomCollectionForm(CustomCollectionEditor editor)
        {
            this.editor = editor;
        }

        protected bool CanRemoveInstance(object value)
        {
            return this.editor.CanRemoveInstance(value);
        }

        protected virtual bool CanSelectMultipleInstances()
        {
            return this.editor.CanSelectMultipleInstances();
        }

        protected object CreateInstance(Type itemType)
        {
            return this.editor.CreateInstance(itemType);
        }

        protected void DestroyInstance(object instance)
        {
            this.editor.DestroyInstance(instance);
        }

        protected virtual void DisplayError(Exception e)
        {
            IUIService service = (IUIService)this.GetService(typeof(IUIService));
            if (service != null)
            {
                service.ShowError(e);
            }
            else
            {
                string message = e.Message;
                if ((message == null) || (message.Length == 0))
                {
                    message = e.ToString();
                }
                MessageBox.Show(null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, 0);
            }
        }

        protected override object GetService(Type serviceType)
        {
            return this.editor.GetService(serviceType);
        }

        protected abstract void OnEditValueChanged();
        protected internal virtual DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc)
        {
            return edSvc.ShowDialog(this);
        }

        // Properties
        internal virtual bool CollectionEditable
        {
            get
            {
                if (this.editableState != 0)
                {
                    return (this.editableState == 1);
                }
                bool flag = typeof(IList).IsAssignableFrom(this.editor.CollectionType);
                if (flag)
                {
                    IList editValue = this.EditValue as IList;
                    if (editValue != null)
                    {
                        return !editValue.IsReadOnly;
                    }
                }
                return flag;
            }
            set
            {
                if (value)
                {
                    this.editableState = 1;
                }
                else
                {
                    this.editableState = 2;
                }
            }
        }

        protected Type CollectionItemType
        {
            get
            {
                return this.editor.CollectionItemType;
            }
        }

        protected Type CollectionType
        {
            get
            {
                return this.editor.CollectionType;
            }
        }

        protected ITypeDescriptorContext Context
        {
            get
            {
                return this.editor.Context;
            }
        }

        public object EditValue
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.OnEditValueChanged();
            }
        }

        protected object[] Items
        {
            get
            {
                return this.editor.GetItems(this.EditValue);
            }
            set
            {
                bool flag = false;
                try
                {
                    flag = this.Context.OnComponentChanging();
                }
                catch (Exception exception)
                {
                    /*
                    if (ClientUtils.IsCriticalException(exception))
                    {
                        throw;
                    }*/
                    this.DisplayError(exception);
                }
                if (flag)
                {
                    object obj2 = this.editor.SetItems(this.EditValue, value);
                    if (obj2 != this.EditValue)
                    {
                        this.EditValue = obj2;
                    }
                    this.Context.OnComponentChanged();
                }
            }
        }

        protected Type[] NewItemTypes
        {
            get
            {
                return this.editor.NewItemTypes;
            }
        }
    }

}
