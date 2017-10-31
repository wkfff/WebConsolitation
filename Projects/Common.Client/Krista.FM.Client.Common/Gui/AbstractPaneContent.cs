using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.Common.Gui
{
    public abstract class AbstractPaneContent : IPaneContent
    {
        public abstract Control Control
        {
            get;
        }

        public virtual void RedrawContent()
        {
        }

        public virtual void Dispose()
        {
        }

        public bool IsVisible
        {
            get
            {
                return Control.Visible && Control.Width > 0;
            }
        }
    }
}
