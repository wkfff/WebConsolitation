using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinDock;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Workplace.Services;

namespace Krista.FM.Client.Workplace.Gui
{
    internal class WorkplaceLayout : IWorkplaceLayout
    {
        private Workplace workplaceForm;

        private Dictionary<string, PadContentWrapper> contentHash = new Dictionary<string, PadContentWrapper>();

        public IWorkplaceWindow ActiveWorkplaceWindow
        {
            get
            {
                WorkplaceWindow window = TabbedMdiService.ActiveWindow as WorkplaceWindow;
                if (window == null || window.IsDisposed)
                {
                    return null;
                }
                return window;
            }
        }

        private object lastActiveContent;

        public object ActiveContent
        {
            get
            {
                object activeContent;

                activeContent = TabbedMdiService.ActiveWindow ?? lastActiveContent;

                if (activeContent != null)
                {
                    if (activeContent is Control && ((Control)activeContent).IsDisposed)
                        activeContent = null;
                    if (activeContent is Infragistics.Shared.DisposableObject && ((Infragistics.Shared.DisposableObject)activeContent).Disposed)
                        activeContent = null;
                }

                lastActiveContent = activeContent;

                if (activeContent is IWorkplaceWindow)
                {
                    return ((IWorkplaceWindow) activeContent).ActiveViewContent;
                }

                if (activeContent is PadContentWrapper)
                {
                    return ((PadContentWrapper) activeContent).PadContent;
                }

                return activeContent;
            }
        }

        public void Attach(IWorkplace workplace)
        {
			Trace.TraceVerbose("Attach WorkplaceLayout");
            
			workplaceForm = (Workplace)workplace;
            workplaceForm.SuspendLayout();

            workplaceForm.Controls.Add(StatusBarService.Control);

            DockManagerService.Attach(workplaceForm);
            TabbedMdiService.Attach(workplaceForm);

            TabbedMdiService.Control.TabSelected += new Infragistics.Win.UltraWinTabbedMdi.MdiTabEventHandler(ActiveMdiChanged);
            ActiveMdiChanged(this, EventArgs.Empty);

            workplaceForm.ResumeLayout();
        }

        public void Detach()
        {
        }

        class PadContentWrapper : DockableControlPane
        {
            private PaneDescriptor padDescriptor;
            private bool isInitialized = false;
            internal bool allowInitialize = false;

            public IPaneContent PadContent
            {
                get { return padDescriptor.PadContent; }
            }

            public PadContentWrapper(PaneDescriptor padDescriptor)
            {
                if (padDescriptor == null)
                    throw new ArgumentNullException("padDescriptor");
                
                this.padDescriptor = padDescriptor;

                Settings.CanDisplayAsMdiChild = Infragistics.Win.DefaultableBoolean.False;
                Settings.AllowDockAsTab = Infragistics.Win.DefaultableBoolean.False;
                Settings.AllowFloating = Infragistics.Win.DefaultableBoolean.True;
                Settings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.True;
                Settings.AllowDockRight = Infragistics.Win.DefaultableBoolean.True;
                Settings.AllowDockTop = Infragistics.Win.DefaultableBoolean.True;
                Settings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.True;

                //HideOnClose = true;
            }

            public void DetachContent()
            {
                //Controls.Clear();
                this.Control = null;
                padDescriptor = null;
            }

            public override void Show()
            {
                if (this.DockAreaPane == null)
                {
                    DockAreaPane dap = new DockAreaPane(DockedLocation.DockedLeft);
                    dap.Size = PadContent.Control.Size;
                    dap.Panes.Add(this);
                    DockManagerService.Control.DockAreas.Add(dap);
                }

                base.Show();
                AllowInitialize();
            }

            /*protected override void OnVisibleChanged(EventArgs e)
            {
                base.OnVisibleChanged(e);
                if (Visible && Width > 0)
                    ActivateContent();
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);
                if (Visible && Width > 0)
                    ActivateContent();
            }
            */
            public void AllowInitialize()
            {
                allowInitialize = true;
                if (IsVisible && this.Size.Width > 0)
                    ActivateContent();
            }

            void ActivateContent()
            {
                if (!allowInitialize)
                    return;
                if (!isInitialized)
                {
                    isInitialized = true;
                    IPaneContent content = padDescriptor.PadContent;
                    if (content == null)
                        return;
                    Control control = content.Control;
                    control.Dock = DockStyle.Fill;
                    this.Control = control;
                    //Controls.Add(control);
                }
            }

            protected string GetPersistString()
            {
                return padDescriptor.Class;
            }

            protected override void OnDispose()
            {
                base.OnDispose();

                if (!Disposed)
                {
                    if (padDescriptor != null)
                    {
                        padDescriptor.Dispose();
                        padDescriptor = null;
                    }
                }
            }
        }

        public void CloseWindowEvent(object sender, EventArgs e)
        {
            WorkplaceWindow f = (WorkplaceWindow)sender;
            f.CloseEvent -= CloseWindowEvent;
            if (f.ViewContent != null)
            {
                ((IWorkbench)workplaceForm).CloseContent(f.ViewContent);
                if (f == oldSelectedWindow)
                {
                    oldSelectedWindow = null;
                }
                ActiveMdiChanged(this, null);
            }
        }

        private PadContentWrapper CreateContent(PaneDescriptor content)
        {
            if (contentHash.ContainsKey(content.Class))
            {
                return contentHash[content.Class];
            }
            //Properties properties = (Properties)PropertyService.Get("Workspace.ViewMementos", new Properties());

            PadContentWrapper newContent = new PadContentWrapper(content);
            //if (!string.IsNullOrEmpty(content.Icon))
            //{
            //    newContent.Icon = IconService.GetIcon(content.Icon);
            //}
            //newContent.Text = StringParser.Parse(content.Title);
            newContent.Text = content.Title;
            contentHash[content.Class] = newContent;
            return newContent;
        }

        public void ShowPad(PaneDescriptor content)
        {
            if (content == null)
            {
                return;
            }
            if (!contentHash.ContainsKey(content.Class))
            {
                DockableControlPane newContent = CreateContent(content);
                // TODO: read the default dock state from the PadDescriptor
                // we'll also need to allow for default-hidden (HideOnClose) contents
                // which seems to be not possible using any Show overload.
                newContent.Show();
                //newContent.Show(dockPanel);
            }
            else
            {
                if (contentHash[content.Class].IsVisible)
                    contentHash[content.Class].Close();
                else
                {
                    contentHash[content.Class].Show();
                    if (!contentHash[content.Class].Pinned)
                    {
                        contentHash[content.Class].Flyout(false, true);
                    }
                }
            }
        }

        public bool IsVisible(PaneDescriptor paneContent)
        {
            if (paneContent != null && contentHash.ContainsKey(paneContent.Class))
            {
                return contentHash[paneContent.Class].IsVisible;
            }
            return false;
        }

        public void HidePad(PaneDescriptor padContent)
        {
            if (padContent != null && contentHash.ContainsKey(padContent.Class))
            {
                contentHash[padContent.Class].Close();
            }
        }

        public void UnloadPad(PaneDescriptor padContent)
        {
            if (padContent != null && contentHash.ContainsKey(padContent.Class))
            {
                contentHash[padContent.Class].Close();
                contentHash[padContent.Class].Dispose();
                contentHash.Remove(padContent.Class);
            }
        }

        public void ActivatePad(PaneDescriptor padContent)
        {
            if (padContent != null && contentHash.ContainsKey(padContent.Class))
            {
                //contentHash[padContent.Class].ActivateContent();
                contentHash[padContent.Class].Show();
            }
        }
        public void ActivatePad(string fullyQualifiedTypeName)
        {
            //contentHash[fullyQualifiedTypeName].ActivateContent();
            contentHash[fullyQualifiedTypeName].Show();
        }

        public IWorkplaceWindow ShowView(IViewContent content)
        {
            if (content.WorkplaceWindow is WorkplaceWindow)
            {
                WorkplaceWindow oldWindow = (WorkplaceWindow)content.WorkplaceWindow;
                if (!oldWindow.IsDisposed)
                {
                    oldWindow.Show();
                    return oldWindow;
                }
            }

            if (!content.Control.Visible)
            {
                content.Control.Visible = true;
            }

            content.Control.Dock = DockStyle.Fill;
            WorkplaceWindow workplaceWindow = new WorkplaceWindow(content);
            workplaceWindow.CloseEvent += new EventHandler(CloseWindowEvent);
            workplaceWindow.Show();
            //workplaceWindow.Activate();

            return workplaceWindow;
        }

        private void ActiveMdiChanged(object sender, EventArgs e)
        {
            OnActiveWorkplaceWindowChanged(e);
        }

        private void ActiveContentChanged(object sender, EventArgs e)
        {
            OnActiveWorkplaceWindowChanged(e);
        }

        private IWorkplaceWindow oldSelectedWindow = null;
        
        internal virtual void OnActiveWorkplaceWindowChanged(EventArgs e)
        {
            IWorkplaceWindow newWindow = this.ActiveWorkplaceWindow;
            if (newWindow == null || newWindow.ViewContent != null)
            {
                if (ActiveWorkplaceWindowChanged != null)
                {
                    ActiveWorkplaceWindowChanged(this, e);
                }
            }
            else
            {
                // ignore window change to disposed window
            }
            if (oldSelectedWindow != null)
            {
                oldSelectedWindow.OnWindowDeselected(EventArgs.Empty);
            }
            oldSelectedWindow = newWindow;
            if (oldSelectedWindow != null && oldSelectedWindow.ViewContent != null)
            {
                oldSelectedWindow.OnWindowSelected(EventArgs.Empty);
                //oldSelectedWindow.ActiveViewContent.SwitchedTo();
            }
        }

        public event EventHandler ActiveWorkplaceWindowChanged;
    }
}
