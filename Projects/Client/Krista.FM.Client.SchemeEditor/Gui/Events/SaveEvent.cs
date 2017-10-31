using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    public delegate void SaveEventHandler(object sender, SaveEventArgs e);

    public class SaveEventArgs : System.EventArgs
    {
        bool successful;

        public bool Successful
        {
            get { return successful; }
        }

        public SaveEventArgs(bool successful)
        {
            this.successful = successful;
        }
    }
}
