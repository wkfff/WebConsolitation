using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    public class WorkspaceLayout : IWorkbenchLayout
    {
        private SchemeEditor _wb;

        #region IWorkbenchLayout Members

        public IWorkbenchWindow ActiveWorkbenchWindow
        {
            get 
            {
                return _wb.TabbedMdiManager.ActiveTab.Form as IWorkbenchWindow;
            }
        }

        public void Attach(IWorkbench workbench)
        {
            _wb = (SchemeEditor)workbench;

            ShowViewContents();
            RedrawAllComponents();
        }

        public void Detach()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RedrawAllComponents()
        {
            //!RedrawMainMenu();
            //!RedrawToolbars();
            //!RedrawStatusBar();
        }

        public IWorkbenchWindow ShowView(IViewContent content)
        {
            if (content.WorkbenchWindow is WorkspaceWindow)
            {
                WorkspaceWindow oldSdiWindow = (WorkspaceWindow)content.WorkbenchWindow;
                if (!oldSdiWindow.IsDisposed)
                {
                    oldSdiWindow.Show(_wb.Form);
                    return oldSdiWindow;
                }
            }

            if (!content.Control.Visible)
            {
                content.Control.Visible = true;
            }

            WorkspaceWindow sdiWorkspaceWindow = new WorkspaceWindow(content);
            //sdiWorkspaceWindow.CloseEvent += new EventHandler(CloseWindowEvent);
            if (_wb.Form != null)
            {
                sdiWorkspaceWindow.Show();
                if (ActiveWorkbenchWindowChanged != null)
                    ActiveWorkbenchWindowChanged(this, null);
            }

            return sdiWorkspaceWindow;
        }

        public event EventHandler ActiveWorkbenchWindowChanged;

        #endregion

        void ShowViewContents()
        {
            foreach (IViewContent content in SchemeEditor.Instance.ViewContentCollection)
            {
                ShowView(content);
            }
        }
    }
}
