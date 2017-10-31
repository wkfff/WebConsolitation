using System.Collections.Generic;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Core.ExtensionModule.Services
{
    public class ViewService
    {
        private readonly Dictionary<string, View> views = new Dictionary<string, View>();

        public void AddView(string id, View view)
        {
            views.Add(id, view);
        }

        public View GetView(string id)
        {
            return views[id];
        }

        public void Clear()
        {
            views.Clear();
        }
    }
}
