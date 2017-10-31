using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.Workplace
{
    public partial class Workplace : Form, IWorkbench
    {
        private List<PaneDescriptor> viewContentCollection = new List<PaneDescriptor>();
        private List<IViewContent> workbenchContentCollection = new List<IViewContent>();
        private bool closeAll = false;

        public List<PaneDescriptor> PaneContentCollection
        {
            get
            {
                System.Diagnostics.Debug.Assert(viewContentCollection != null);
                return viewContentCollection;
            }
        }

        /// <summary>
        /// Коллекция в которой содержатся все открытые окна.
        /// </summary>
        public List<IViewContent> ViewContentCollection
        {
            get
            {
                System.Diagnostics.Debug.Assert(workbenchContentCollection != null);
                return workbenchContentCollection;
            }
        }

		/// <summary>
		/// Возвращает открытое окно по ключу.
		/// </summary>
		/// <param name="key">Уникальный ключ.</param>
		/// <returns>Если содержимое по ключу не найдено, то возвращает null.</returns>
		public IViewContent GetOpenedContent(string key)
		{
			foreach (IViewContent item in ViewContentCollection)
			{
				if (item.Key == key)
				{
					return item;
				}
			}
			return null;
		}

		public object ActiveContent
        {
            get
            {
                if (layout == null)
                {
                    return null;
                }
                return layout.ActiveContent;
            }
        }

        public void CloseContent(IViewContent content)
        {
            if (content is IPersistenceSupport)
            {
				((IPersistenceSupport)content).SavePersistence();
            }

			/* TODO: Сохранить настройки объекта
            if (content is IMementoCapable)
            {
                StoreMemento(content);
            }*/
            
            if (ViewContentCollection.Contains(content))
            {
                ViewContentCollection.Remove(content);
            }
            
            OnViewClosed(new ViewContentEventArgs(content));
            content.Dispose();
        }

        /// <summary>
        /// Вставляет новый объект <see cref="IViewContent"/> в workplace.
        /// </summary>
        /// <param name="content"></param>
        public virtual void ShowView(IViewContent content)
        {
            System.Diagnostics.Debug.Assert(layout != null);
            ViewContentCollection.Add(content);

            // TODO: Сделать загрузку сохраненных свойств для объекта просмотра

            layout.ShowView(content);
            content.WorkplaceWindow.SelectWindow();
            OnViewOpened(new ViewContentEventArgs(content));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
			Trace.TraceVerbose("Workplace.OnClosing");
        	Trace.Indent();
			base.OnClosing(e);

			if (e is FormClosingEventArgs && 
                (((FormClosingEventArgs)e).CloseReason == CloseReason.UserClosing || ((FormClosingEventArgs)e).CloseReason == CloseReason.TaskManagerClosing))
			{
				// TODO: Сделать проверку на возможность выгрузки объекта просмотра
				foreach (BaseNavigationCtrl vo in Addins)
				{
					// если хотя бы один объект просмотра не хочет выгружаться - не закрываем приложение
					if (!vo.CanUnload)
					{
						closeAll = false;
						e.Cancel = true;
						return;
					}
				}
			}

			while (WorkplaceSingleton.Workplace.ViewContentCollection.Count > 0)
			{
				IViewContent content = WorkplaceSingleton.Workplace.ViewContentCollection[0];
				if (content.WorkplaceWindow == null)
				{
					WorkplaceSingleton.Workplace.ViewContentCollection.RemoveAt(0);
				}
				else
				{
					content.WorkplaceWindow.CloseWindow(false);
					if (WorkplaceSingleton.Workplace.ViewContentCollection.IndexOf(content) >= 0)
					{
						e.Cancel = true;
						return;
					}
				}
			}

			closeAll = true;

			layout.Detach();

			foreach (PaneDescriptor padDescriptor in PaneContentCollection)
			{
				padDescriptor.Dispose();
			}
			Trace.Unindent();
		}

        private void OnActiveWindowChanged(object sender, EventArgs e)
        {
            if (!closeAll && ActiveWorkplaceWindowChanged != null)
            {
                ActiveWorkplaceWindowChanged(this, e);
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
        public event EventHandler ActiveWorkplaceWindowChanged;
    }
}
