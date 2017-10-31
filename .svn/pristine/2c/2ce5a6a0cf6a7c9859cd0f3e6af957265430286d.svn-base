using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;

namespace Krista.FM.RIA.Core.ExtensionModule.Services
{
    public class WindowService
    {
        private readonly List<Window> windows = new List<Window>();

        public void AddWindow(params Window[] wnds)
        {
            windows.AddRange(wnds);
        }

        public IEnumerable<Window> GetWindows()
        {
            return windows;
        }

        public void Clear()
        {
            windows.Clear();
        }
    }
}
