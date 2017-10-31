using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Balance;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class CalculatePlanResultCommand : AbstractCommand
    {
        public CalculatePlanResultCommand()
        {
            key = "btnCalculatePlanResult";
            caption = "Расчет остатков средств бюджета";
        }

        public override void Run()
        {
            CalculatePlanResultWizard wizard = new CalculatePlanResultWizard();
            wizard.ShowDialog(WorkplaceSingleton.Workplace.WindowHandle);
            BalanceUI clsUI = (BalanceUI)WorkplaceSingleton.Workplace.ActiveContent;
            clsUI.Refresh();
        }
    }
}
