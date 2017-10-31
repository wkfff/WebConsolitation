using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.Common.Gui
{
    public interface IPaneContent : IDisposable
    {
        Control Control { get; }
        void RedrawContent();
    }
}
