using System;
using System.Windows.Forms;

using Krista.FM.Client.Common.Gui;


namespace Krista.FM.Client.Workplace.Gui
{
    public class WorkplaceWindow : Form, IWorkplaceWindow
    {
        private IViewContent content;

        public WorkplaceWindow(IViewContent content)
        {
            this.content = content;
            content.WorkplaceWindow = this;
            content.TitleChanged += SetTitleEvent;

            Controls.Add(content.Control);

            Icon = content.Icon;

            MdiParent = WorkplaceSingleton.MainForm;

            SetTitleEvent(this, EventArgs.Empty);

			Load += WorkplaceWindowOnLoad;
        }

		private void WorkplaceWindowOnLoad(object sender, EventArgs e)
		{
			Text = content.Control.Text;
		}

        #region IWorkplaceWindow Members

        public string Title
        {
            get { return Text; }
            set
            {
                Text = value;
                OnTitleChanged(EventArgs.Empty);
            }
        }

        public IViewContent ViewContent
        {
            get { return content; }
        }

        public IViewContent ActiveViewContent
        {
            get { return content; }
        }

        public void SelectWindow()
        {
            Show();
            Activate();
        }

        public event EventHandler TitleChanged;
        public event EventHandler WindowSelected;
        public event EventHandler WindowDeselected;
        public event EventHandler CloseEvent;

        #endregion

        public void SetTitleEvent(object sender, EventArgs e)
        {
            if (content == null)
            {
                return;
            }

            // здесь необходимо установить призкак того, 
            // что содержимое окна изменено, но не сохранено

        }

        public void DetachContent()
        {
            content.TitleChanged -= SetTitleEvent;
            content = null;
        }

        public bool CloseWindow(bool force)
        {
            if (!force && ViewContent != null /*&& ViewContent.IsDirty*/)
            {
                // TODO: Сделать сохранение содержимого
            }

            OnCloseEvent(null);
            Dispose();
            return true;
        }

        protected virtual void OnTitleChanged(EventArgs e)
        {
            if (TitleChanged != null)
            {
                TitleChanged(this, e);
            }
            //Workplace.Instance.OnActiveWindowChanged(EventArgs.Empty);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CloseWindow(false);
        }

        protected virtual void OnCloseEvent(EventArgs e)
        {
            OnWindowDeselected(e);
            if (CloseEvent != null)
            {
                CloseEvent(this, e);
            }
        }

        public virtual void OnWindowSelected(EventArgs e)
        {
            if (WindowSelected != null)
            {
                WindowSelected(this, e);
            }
        }

        public virtual void OnWindowDeselected(EventArgs e)
        {
            if (WindowDeselected != null)
            {
                WindowDeselected(this, e);
            }
        }
    }
}
