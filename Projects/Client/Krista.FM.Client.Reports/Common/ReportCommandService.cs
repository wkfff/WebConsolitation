using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Common.Services;

namespace Krista.FM.Client.Reports.Common
{
    public static class ReportCommandService
    {
        /// <summary>
        /// Связывает элемент управления ctrl с командой cmd.
        /// </summary>
        public static void AttachCommandToControl(AbstractCommand cmd, Control ctrl)
        {
            ctrl.Tag = cmd;
            ctrl.Click += OnCommandToolClick;
        }

        /// <summary>
        /// Обработчик срабатывания команды.
        /// </summary>
        private static void OnCommandToolClick(object sender, EventArgs e)
        {
            // Обычная кнопка на форме или панели
            if (sender is Control && ((Control)sender).Tag != null)
            {
                ((AbstractCommand)((Control)sender).Tag).Run();
            }

            // Компонент тулбара или меню
            if (sender is ToolBase && ((ToolBase)sender).SharedProps.Tag != null)
            {
                ((AbstractCommand)((ToolBase)sender).SharedProps.Tag).Run();
            }
        }

        public static ButtonTool AttachToolbarTool(AbstractCommand command, UltraToolbar toolbar, string parentToolName)
        {
            var utm = toolbar.ToolbarsManager;

            if (utm.Tools.Exists(command.Key))
            {
                return (ButtonTool) utm.Tools[command.Key];
            }

            var tool = new ButtonTool(command.Key);
            var correctedCaption = command.Caption;
            
            if (command is CommonReportsCommand)
            {
                var cmd = (CommonReportsCommand)command;
                correctedCaption = cmd.GetCorrectedCaption();
            }

            tool.SharedProps.Caption = correctedCaption;
            tool.SharedProps.Shortcut = command.Shortcut;
            tool.SharedProps.Tag = command;
            var icon = ResourceService.GetIcon(command.IconKey);

            if (icon != null)
            {
                tool.CustomizedImage = new Icon(icon, 16, 16).ToBitmap();
            }

            utm.Tools.Add(tool);

            if (!String.IsNullOrEmpty(parentToolName) && utm.Tools.Exists(parentToolName))
            {
                ((PopupMenuTool)utm.Tools[parentToolName]).Tools.AddTool(tool.Key);
            }
            else
            {
                toolbar.Tools.AddTool(tool.Key);
            }

            tool.ToolClick += OnCommandToolClick;
            return tool;
        }
    }
}
