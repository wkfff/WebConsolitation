using System;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Domain.Services.FinSourceDebtorBook;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Commands
{
    public class TransfertDataCommand : AbstractCommand
    {
        public TransfertDataCommand()
        {
            caption = "Перенос данных из источников финансирования";
            key = "TransfertDataCommand";
        }

        public override void Run()
        {
            DateTime calculateDate = DebtBookNavigation.Instance.CalculateDate;
            DebtBookNavigation.Instance.Workplace.OperationObj.Text = "Перенос данных из источников финансирования";
            DebtBookNavigation.Instance.Workplace.OperationObj.StartOperation();
            try
            {
                TransfertDataService transfertDataService = DebtBookNavigation.Instance.Services.TransfertDataService;
                transfertDataService.TransfertFinSourceData(
                    calculateDate, 
                    DebtBookNavigation.Instance.VariantYear, 
                    DebtBookNavigation.Instance.CurrentVariantID, 
                    DebtBookNavigation.Instance.CurrentSourceID, 
                    DebtBookNavigation.Instance.CurrentRegion);
                
                DebtBookNavigation.Instance.Workplace.OperationObj.StopOperation();
                
                MessageBox.Show(DebtBookNavigation.Instance.Workplace.WindowHandle,
                                "Перенос данных из источников финансирования закончился успешно", "Долговая книга",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DebtBookNavigation.Instance.Workplace.OperationObj.StopOperation();
            }
        }
    }

    /// <summary>
    /// кнопка копирования данных по варианту
    /// </summary>
    public class CopyRegionDataToVariantCommand : AbstractCommand
    {
        public CopyRegionDataToVariantCommand()
        {
            caption = "Копирование данных с предыдущего варианта";
            key = "CopyRegionDataToVariantCommand";
        }

        public override void Run()
        {
            DebtBookNavigation.Instance.Workplace.OperationObj.Text = "Обработка данных";
            DebtBookNavigation.Instance.Workplace.OperationObj.StartOperation();
            try
            {
                UserRegionType userRegionType =
                    DebtBookNavigation.Instance.GetUserRegionType(DebtBookNavigation.Instance.CurrentRegion);
                CopyRegionVariantService copyRegionVariant =
                    DebtBookNavigation.Instance.Services.CopyRegionVariantService;
                switch (userRegionType)
                {
                    case UserRegionType.Region:
                    case UserRegionType.Settlement:
                    case UserRegionType.Town:
                        DebtBookNavigation.Instance.Workplace.OperationObj.Text = "Копирование данных с варианта на вариант";
                        DebtBookNavigation.Instance.Workplace.OperationObj.StartOperation();
                        try
                        {
                            string message = copyRegionVariant.CopyRegionData(DebtBookNavigation.Instance.CurrentVariantID,
                                DebtBookNavigation.Instance.VariantYear,
                                DebtBookNavigation.Instance.CalculateDate,
                                DebtBookNavigation.Instance.CurrentRegion,
                                DebtBookNavigation.Instance.CurrentSourceID);
                            MessageBox.Show(DebtBookNavigation.Instance.Workplace.WindowHandle, message, "Долговая книга",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                            DebtBookNavigation.Instance.Workplace.OperationObj.StopOperation();
                        }
                        break;
                    case UserRegionType.Subject:
                        if (MessageBox.Show("Копировать данные по районам?", "Долговая книга", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DebtBookNavigation.Instance.Workplace.OperationObj.Text = "Копирование данных с варианта на вариант";
                            DebtBookNavigation.Instance.Workplace.OperationObj.StartOperation();
                            try
                            {
                                string message = copyRegionVariant.CopyAllData(DebtBookNavigation.Instance.CurrentVariantID,
                                    DebtBookNavigation.Instance.VariantYear,
                                    DebtBookNavigation.Instance.CalculateDate,
                                    DebtBookNavigation.Instance.CurrentRegion,
                                    DebtBookNavigation.Instance.CurrentSourceID);
                                MessageBox.Show(DebtBookNavigation.Instance.Workplace.WindowHandle, message, "Долговая книга",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            finally
                            {
                            DebtBookNavigation.Instance.Workplace.OperationObj.StopOperation();
                            }
                        }
                        else
                        {
                            DebtBookNavigation.Instance.Workplace.OperationObj.Text = "Копирование данных с варианта на вариант";
                            DebtBookNavigation.Instance.Workplace.OperationObj.StartOperation();
                            try
                            {
                                string message = copyRegionVariant.CopySubjectData(DebtBookNavigation.Instance.CurrentVariantID,
                                    DebtBookNavigation.Instance.VariantYear,
                                    DebtBookNavigation.Instance.CalculateDate,
                                    DebtBookNavigation.Instance.CurrentRegion,
                                    DebtBookNavigation.Instance.CurrentSourceID);
                                MessageBox.Show(DebtBookNavigation.Instance.Workplace.WindowHandle,
                                    message, "Долговая книга",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            finally
                            {
                                DebtBookNavigation.Instance.Workplace.OperationObj.StopOperation();
                            }
                        }
                        break;
                }
            }
            finally
            {
                DebtBookNavigation.Instance.Workplace.OperationObj.StopOperation();
            }
        }
    }
}
