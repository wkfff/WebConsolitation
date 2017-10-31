using Ext.Net;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.E86N.Extensions
{
    public static class ToolBarExtensions
    {
        public static ToolbarBase Toolbar(this PanelBase panel)
        {
            if (panel.TopBar.Count == 1 && (panel.TopBar[0] is Toolbar || panel.TopBar[0] is PagingToolbar))
            {
                return panel.TopBar[0];
            }

            var toolbar = new Toolbar { ID = "{0}Toolbar".FormatWith(panel.ID) };
            panel.TopBar.Add(toolbar);
            return toolbar;
        }
    }
}
