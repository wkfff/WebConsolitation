using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    internal class WorkspaceWindow : Form, IWorkbenchWindow
    {
        private IViewContent content;

        public WorkspaceWindow(IViewContent content)
        {
            this.content = content;
            this.content.WorkbenchWindow = this;
            this.MdiParent = SchemeEditor.Instance.Form;

        }

        #region IWorkbenchWindow Members

        public IViewContent ViewContent
        {
            get { return content; }
        }

        public IBaseViewContent ActiveViewContent
        {
            get { return content; }
        }

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public bool CloseWindow(bool force)
        {
            CloseEvent(this, null);
            throw new Exception("The method or operation is not implemented.");
        }

        public void SelectWindow()
        {
            Show();
        }

        public virtual void RedrawContent()
        {
        }

        public event EventHandler CloseEvent;

        #endregion
    }
}
