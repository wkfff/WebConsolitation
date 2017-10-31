using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.DDE;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class DDECalculationCommand : AbstractCommand
    {
        public DDECalculationCommand()
        {
            key = "DDECalculation";
            caption = "Расчет доступной долговой емкости";
        }

        public override void Run()
        {
            int incomeVariant = -1;
            int outcomeVariant = -1;
            int ifVariant = FinSourcePlanningNavigation.Instance.CurrentVariantID > 0 ? FinSourcePlanningNavigation.Instance.CurrentVariantID : -1;
            string ifVariantCaption = ifVariant > 0 ? FinSourcePlanningNavigation.Instance.CurrentVariantCaption : string.Empty;
            if (VariantSelectForm.ShowVariantForm(WorkplaceSingleton.Workplace.WindowHandle, ifVariantCaption, ref incomeVariant, ref outcomeVariant, ref ifVariant))
            {
                WorkplaceSingleton.Workplace.OperationObj.Text = "Расчет доступной долговой емкости";
                WorkplaceSingleton.Workplace.OperationObj.StartOperation();
                try
                {
                    DDEIndicatorsUI clsUI = (DDEIndicatorsUI)WorkplaceSingleton.Workplace.ActiveContent;
                    //clsUI.ClearData();
                    DDEService server = new DDEService(WorkplaceSingleton.Workplace.ActiveScheme);
                    DataTable dtResults = server.CalculateDDE(ifVariant, incomeVariant, outcomeVariant, FinSourcePlanningNavigation.Instance.CurrentSourceID);
                    clsUI.SetCalculationData(dtResults);
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Расчет доступной долговой емкости успешно завершен",
                        "Определение доступной долговой емкости", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                    throw;
                }
            }
        }
    }
}
