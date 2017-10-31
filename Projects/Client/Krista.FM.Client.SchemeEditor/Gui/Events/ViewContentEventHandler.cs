using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    public delegate void ViewContentEventHandler(object sender, ViewContentEventArgs e);

    public class ViewContentEventArgs : System.EventArgs
    {
        IViewContent content;

        public IViewContent Content
        {
            get { return content; }
            set { content = value; }
        }

        public ViewContentEventArgs(IViewContent content)
        {
            this.content = content;
        }
    }
}
