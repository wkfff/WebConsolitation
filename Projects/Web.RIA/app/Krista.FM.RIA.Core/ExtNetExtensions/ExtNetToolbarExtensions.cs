using Ext.Net;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Core
{
    public static class ExtNetToolbarExtensions
    {
        public static Button AddIconButton(this Toolbar toolbar, string id, Icon icon, string toolTip)
        {
            return AddIconButton(toolbar, id, icon, toolTip, null);
        }

        public static Button AddIconButton(this ToolbarBase toolbar, string id, Icon icon, string toolTip, string handler)
        {
            Button saveButton = new Button
            {
                ID = id,
                Icon = icon,
                ToolTip = toolTip
            };

            if (handler.IsNotNullOrEmpty())
            {
                saveButton.Listeners.Click.Handler = handler;
            }

            toolbar.Add(saveButton);

            return saveButton;
        }

        public static Button SetDisabled(this Button button)
        {
            if (button != null)
            {
                button.Disabled = true;
            }

            return button;
        }
    }
}
