using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Common;
using Krista.FM.Domain.Services.FinSourceDebtorBook;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Commands.Reports
{
    public class MinfinReportCommand : AbstractCommand
    {
        public MinfinReportCommand()
        {
            key = "MinfinReport";
            caption = "Выгрузка данных в Минфин";
        }

        public override void Run()
        {
            DateTime reportDate = DateTime.Today;
            if (!DateSelectForm.ShowDateForm(DebtBookNavigation.Instance.Workplace.WindowHandle, ref reportDate))
                return;

            string fileName = string.Format("{0}690r0{1}",
                DebtBookNavigation.Instance.Workplace.ActiveScheme.GlobalConstsManager.Consts["RegionMFRF"].Value.ToString().Substring(0, 2), ((int)reportDate.DayOfWeek));
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.txt, true, ref fileName))
            {
                DebtBookNavigation.Instance.Workplace.OperationObj.Text = "Получение и обработка данных";
                DebtBookNavigation.Instance.Workplace.OperationObj.StartOperation();
                //try
                {
                    MinfinUnloadingService minfinService =
                        new MinfinUnloadingService(DebtBookNavigation.Instance.Workplace.ActiveScheme);
                    using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                        {
                            writer.Write(minfinService.GetMinfinData(reportDate));
                        }
                    }
                    Arj32Helper.ArchiveFile(fileName);
                    DebtBookNavigation.Instance.Workplace.OperationObj.StopOperation();
                    MessageBox.Show("Выгрузка данных завершилась успешно", "Долговая книга", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                //catch
                {
                    DebtBookNavigation.Instance.Workplace.OperationObj.StopOperation();
                    //throw;
                }
            }
        }
    }
}
