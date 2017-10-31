using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Common;

namespace Krista.FM.Client.Common.Gui
{
    public abstract class AbstractViewContent : DisposableObject, IViewContent
    {
        IWorkplaceWindow workplaceWindow = null;

        #region IViewContent Members

        public abstract String Key { get;}

        public abstract Control Control { get;}

        public IWorkplaceWindow WorkplaceWindow
        {
            get { return workplaceWindow; }
            set { workplaceWindow = value; }
        }

        public virtual Icon Icon
        {
            get { return null; }
        }

        public event EventHandler TitleChanged;

        #endregion

		#region IDisposable Members

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				workplaceWindow = null;
			}
			base.Dispose(disposing);
		}

        #endregion
    }
}
