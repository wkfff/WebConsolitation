using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Client.SchemeEditor.Gui;


namespace Krista.FM.Client.SchemeEditor
{
    public partial class SchemeEditor
    {
        private List<IViewContent> workbenchContentCollection = new List<IViewContent>();
        private IWorkbenchLayout layout = null;
        private bool closeAll = false;

        #region IWorkbench Members

        /// <summary>
        /// Ќазвание отображаемое в заголовке формы
        /// </summary>
        public string Title
        {
            get
            {
                return mainForm.Text;
            }
            set
            {
                mainForm.Text = value;
            }
        }

        public List<IViewContent> ViewContentCollection
        {
            get 
            {
                System.Diagnostics.Debug.Assert(workbenchContentCollection != null);
                return workbenchContentCollection;
            }
        }

        public IWorkbenchWindow ActiveWorkbenchWindow
        {
            get 
            {
                if (layout == null)
                {
                    return null;
                }
                return layout.ActiveWorkbenchWindow;
            }
        }

        public IWorkbenchLayout WorkbenchLayout
        {
            get { return layout; }
            set
            {
                if (layout != null)
                {
                    layout.ActiveWorkbenchWindowChanged -= OnActiveWindowChanged;
                    layout.Detach();
                }
                value.Attach(this);
                layout = value;
                layout.ActiveWorkbenchWindowChanged += OnActiveWindowChanged;
            }
        }

        public void ShowView(IViewContent content)
        {
            System.Diagnostics.Debug.Assert(layout != null);
            
            ViewContentCollection.Add(content);

            layout.ShowView(content);
            content.WorkbenchWindow.SelectWindow();
            OnViewOpened(new ViewContentEventArgs(content));
        }

        public void CloseContent(IViewContent content)
        {
            if (ViewContentCollection.Contains(content))
            {
                ViewContentCollection.Remove(content);
            }
            OnViewClosed(new ViewContentEventArgs(content));
            content.Dispose();
            content = null;
        }

        public void CloseAllViews()
        {
            try
            {
                closeAll = true;
                List<IViewContent> fullList = new List<IViewContent>(workbenchContentCollection);
                foreach (IViewContent content in fullList)
                {
                    IWorkbenchWindow window = content.WorkbenchWindow;
                    window.CloseWindow(false);
                }
            }
            finally
            {
                closeAll = false;
                OnActiveWindowChanged(this, EventArgs.Empty);
            }
        }

        public void RedrawAllComponents()
        {
            foreach (IViewContent content in workbenchContentCollection)
            {
                content.RedrawContent();
                if (content.WorkbenchWindow != null)
                {
                    content.WorkbenchWindow.RedrawContent();
                }
            }

            if (layout != null)
            {
                layout.RedrawAllComponents();
            }
        }

        protected virtual void OnViewOpened(ViewContentEventArgs e)
        {
            if (ViewOpened != null)
            {
                ViewOpened(this, e);
            }
        }

        protected virtual void OnViewClosed(ViewContentEventArgs e)
        {
            if (ViewClosed != null)
            {
                ViewClosed(this, e);
            }
        }

        public event ViewContentEventHandler ViewOpened;
        public event ViewContentEventHandler ViewClosed;
        public event EventHandler ActiveWorkbenchWindowChanged;

        #endregion

        private void OnActiveWindowChanged(object sender, EventArgs e)
        {
            if (!closeAll && ActiveWorkbenchWindowChanged != null)
            {
                ActiveWorkbenchWindowChanged(this, e);
            }
        }
    }
}
