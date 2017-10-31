using System.Data;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.ReportsUI.Gui;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Commands
{
    internal class ShowHistoryCommand : AbstractCommand
    {
        internal ShowHistoryCommand()
        {
            key = "ShowHistory";
            caption = "История изменений";
        }

        public override void Run()
        {
            var content = (ReportsClsUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow activeRow = content.GetActiveDataRow();
            if (activeRow == null)
                return;
            content.ShowMasterHistory(activeRow, ((StateButtonTool)Owner).Checked);
        }
    }
}
