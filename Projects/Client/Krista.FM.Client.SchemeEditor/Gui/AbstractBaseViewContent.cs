using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    public abstract class AbstractBaseViewContent : IBaseViewContent
    {
        IWorkbenchWindow workbenchWindow = null;

        public event EventHandler WorkbenchWindowChanged;

        protected virtual void OnWorkbenchWindowChanged(EventArgs e)
        {
            if (WorkbenchWindowChanged != null)
            {
                WorkbenchWindowChanged(this, e);
            }
        }

        #region IBaseViewContent implementation
        
        public abstract Control Control { get; }

        public virtual IWorkbenchWindow WorkbenchWindow
        {
            get { return workbenchWindow; }
            set
            {
                workbenchWindow = value;
                OnWorkbenchWindowChanged(EventArgs.Empty);
            }
        }

        public virtual void SwitchedTo()
        {
        }

        public virtual void Selected()
        {
        }

        public virtual void Deselected()
        {
        }

        public virtual void Deselecting()
        {
        }

        public virtual void RedrawContent()
        {
        }

        #region IDisposable implementation
        
        public virtual void Dispose()
        {
            workbenchWindow = null;
        }
        
        #endregion

        #endregion
    }
}
