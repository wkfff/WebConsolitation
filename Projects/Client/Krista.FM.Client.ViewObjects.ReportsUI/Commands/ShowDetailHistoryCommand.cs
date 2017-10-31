using System.Data;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.ReportsUI.Gui;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Commands
{
    internal class ShowDetailHistoryCommand : AbstractCommand
    {
        internal ShowDetailHistoryCommand()
        {
            key = "ShowDetailHistory";
            caption = "История изменений";
        }

        public override void Run()
        {
            var content = (EgrulClsUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow activeRow = content.GetActiveDetailRow();
            if (activeRow == null)
                return;
            //content.ShowDetailHistory(activeRow, ((StateButtonTool)Owner).Checked);
        }
    }
}
