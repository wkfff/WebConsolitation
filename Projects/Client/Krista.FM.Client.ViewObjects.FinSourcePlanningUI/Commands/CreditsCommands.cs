using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits.CreditIssued;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Wizards;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{

    public class CalcPercentCommand : AbstractCommand
    {
        protected string detailKey;

        protected CalcPercentCommand()
        {
            key = "btnCalcPercent";
            caption = "Расчет ставок процентов";
        }

        public override void Run()
        {
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
            BaseClsView vo = (BaseClsView)content.ViewCtrl;
            UltraGridRow row = vo.ugeCls.ugData.ActiveRow;
            if (row != null)
            {
                try
                {
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Расчет ставок процентов";

                    DataRow[] dataRows = new DataRow[1];
                    dataRows[0] = content.GetActiveDataRow();

                    FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
                    planningServer.CalcPercents(dataRows, -1, false);

                    MessageBox.Show("Расчет ставок процентов успешно выполнен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    content.RefreshDetail(detailKey);
                }
                catch (FinSourcePlanningException ex)
                {
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                }
            }
        }
    }

    public class CalcPercentCommandCI : CalcPercentCommand
    {
        public CalcPercentCommandCI()
            : base()
        {
            detailKey = SchemeObjectsKeys.t_S_JournalPercentCI_Key;
        }
    }

    public class CalcPercentCommandCO : CalcPercentCommand
    {
        public CalcPercentCommandCO()
            : base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_JournalPercentCO;
        }
    }

	/// <summary>
	/// Расчет плана привлечения.
	/// </summary>
	public class CalcAttractionPlanCommand : AbstractCommand
	{
        protected string detailKey;

        protected CalcAttractionPlanCommand(string caption)
		{
			key = "btnCalcAttractionPlan";
            this.caption = caption;
		}

		public override void Run()
		{
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
			UltraGridRow row = ((BaseClsView)content.ViewCtrl).ugeCls.ugData.ActiveRow;
			if (row != null)
			{
				try
				{
					FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
					FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Формирование плана привлечения";

                    FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
                    planningServer.CalcAttractionPlan(planningServer.GetCredit(content.GetActiveDataRow()),
                        FinSourcePlanningNavigation.BaseYear);

					FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
					MessageBox.Show("План привлечения успешно заполнен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    content.RefreshDetail(detailKey);
				}
				catch (FinSourcePlanningException ex)
				{
					FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
					MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
				}
			}
		}
	}

    public class CalcAttractionPlanCommandCI : CalcAttractionPlanCommand
    {
        public CalcAttractionPlanCommandCI (string caption) 
            : base(caption)
        {
            detailKey = SchemeObjectsKeys.t_S_PlanAttractCI_Key;
        }
    }

    public class CalcAttractionPlanCommandCO : CalcAttractionPlanCommand
    {
        public CalcAttractionPlanCommandCO(string caption)
            : base(caption)
        {
            detailKey = CreditIssuedObjectsKeys.t_S_PlanAttractCO;
        }
    }

	/// <summary>
	/// Вычистение плана погашения основного долга.
	/// </summary>
	public class CalcAcquittanceMainPlanCommand : AbstractCommand
	{
        protected string detailKey;

        protected CalcAcquittanceMainPlanCommand()
		{
			key = "btnCalcAcquittanceMainPlan";
			caption = "План погашения основного долга";
		}

		public override void Run()
		{
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
		    DataRow activeRow = content.GetActiveDataRow();
            if (activeRow == null)
                return;
            if (activeRow.IsNull("EndDate"))
            {
                MessageBox.Show("Поле \"Дата окончательного погашения кредита\" не заполнено. Заполните это поле и повторите попытку", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
			if (vo.ugeCls.ugData.ActiveRow != null)
			{
                Credit credit = new Credit(content.GetActiveDataRow());
                int repayDebtMetod = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["RefSRepayDebt"].Value);
                PayPeriodicity payPeriodicity = repayDebtMetod == 0 ? PayPeriodicity.Single : credit.PeriodDebt;
                FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
                AcquittanceMainPlanWizard wizard = new AcquittanceMainPlanWizard(planningServer,
                    vo.ugeCls.ugData.ActiveRow, payPeriodicity, content.HasFactAttractionData());
				wizard.ShowDialog();
                content.RefreshDetail(detailKey);
                content.SetLastCalculation();
			}
		}
	}

    public class CalcDebtPlanCommandCI : CalcAcquittanceMainPlanCommand
    {
        public CalcDebtPlanCommandCI()
            : base()
        {
            detailKey = SchemeObjectsKeys.t_S_PlanDebtCI_Key;
        }
    }

    public class CalcDebtPlanCommandCO : CalcAcquittanceMainPlanCommand
    {
        public CalcDebtPlanCommandCO()
            : base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_PlanDebtCO;
        }
    }

	/// <summary>
	/// Расчет плана обслуживания долга.
	/// </summary>
	public class CalcServicePlanCommand : AbstractCommand
	{
        protected string detailKey;

        protected CalcServicePlanCommand()
		{
			key = "btnCalcDebtServicePlan";
			caption = "План обслуживания долга";
		}

		public override void Run()
		{
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
            DataRow activeRow = content.GetActiveDataRow();
            if (activeRow == null)
                return;
            if (activeRow.IsNull("EndDate"))
            {
                MessageBox.Show("Поле \"Дата окончательного погашения кредита\" не заполнено. Заполните это поле и повторите попытку", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (vo.ugeCls.ugData.ActiveRow != null)
            {
                Credit credit = new Credit(content.GetActiveDataRow());
                PayPeriodicity payPeriodicity = credit.PeriodRate;
                CreditPercentWizard wizard = new CreditPercentWizard(payPeriodicity);
                wizard.ShowDialog();
                content.FormDate = wizard.FormDate;
                content.FormComment = wizard.FormComment;
                content.RefreshDetail(detailKey);
                content.SetLastCalculation();
            }
		}
	}

    public class CalcServicePlanCommandCI : CalcServicePlanCommand
    {
        public CalcServicePlanCommandCI()
            :base()
        {
            detailKey = SchemeObjectsKeys.t_S_PlanServiceCI_Key;
        }
    }

    public class CalcServicePlanCommandCO : CalcServicePlanCommand
    {
        public CalcServicePlanCommandCO()
            :base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_PlanServiceCO;
        }
    }

    public class CreditVariantTransferCommand : AbstractCommand
    {
        protected string creditObjectKey;

        protected CreditVariantTransferCommand()
        {
            key = "VariantTransfer";
            caption = "Перевод договора из одного варианта в другой";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow guaranteeRow = content.GetActiveDataRow();
            if (guaranteeRow != null)
            {
                // спрашиваем, пер
                int variantID = Convert.ToInt32(guaranteeRow["RefVariant"]);
                string variantCaption = "действующие договора";
                string question = "Договор будет перенесен в действующие договора. Подолжить?";
                if (variantID == 0)
                {
                    variantCaption = "архив";
                    question = "Договор будет перенесен в архив. Подолжить?";
                }

                if (MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                   question, "Источники финансирования", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                // переводим в вариант в зависимости от текущего состояния и варианта
                IEntity entity =
                    WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(creditObjectKey);
                Utils.SetNewVariant(guaranteeRow, entity, WorkplaceSingleton.Workplace.ActiveScheme);
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                    string.Format("Договор переведен в {0}", variantCaption), "Источники финансирования", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    public class CreditCIVariantTransfer : CreditVariantTransferCommand
    {
        public CreditCIVariantTransfer()
            :base()
        {
            creditObjectKey = SchemeObjectsKeys.f_S_Сreditincome_Key;
        }
    }

    public class CreditCOVariantTransfer : CreditVariantTransferCommand
    {
        public CreditCOVariantTransfer()
            : base()
        {
            creditObjectKey = SchemeObjectsKeys.f_S_Creditissued_Key;
        }
    }

    public class FillDebtRemainderCommand : AbstractCommand
    {
        public FillDebtRemainderCommand()
        {
            key = "FillDebtRemainder";
            caption = "Расчет текущего остатка по основному долгу";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            try
            {
                WorkplaceSingleton.Workplace.OperationObj.Text = "Обработка данных";
                WorkplaceSingleton.Workplace.OperationObj.StartOperation();
                ((IСreditIncomeService)content.Service).FillDebtRemainder(FinSourcePlanningNavigation.Instance.CurrentVariantID);
                WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                   string.Format("Расчет текущего остатка завершен успешно. Для отображения результатов необходимо обновить данные"),
                   "Кредиты", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                throw new Exception(e.Message, e.InnerException);
            }
            
        }
    }

    public class MassCalculateServicePlanCommand : AbstractCommand
    {
        public MassCalculateServicePlanCommand()
        {
            key = "btnMassCalcPlanService";
            caption = "Сформировать план обслуживания долга по всем договорам";
        }

        public override void Run()
        {
            if (MessageBox.Show("Планы обслуживания долга по всем договорам будут удалены. Хотите произвести расчет?",
                "Кредиты предоставленные", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
                return;

            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
            try
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Формирование планов";

                BudgetCredit budCredit = content as BudgetCredit;

                CreditsTypes creditsTypes = budCredit != null
                                                ? CreditsTypes.BudgetOutcoming
                                                : CreditsTypes.OrganizationOutcoming;

                FinSourcePlanningServer.MassCalculatePlanService(FinSourcePlanningNavigation.Instance.CurrentVariantID,
                    FinSourcePlanningNavigation.BaseYear, creditsTypes, WorkplaceSingleton.Workplace.ActiveScheme);

                MessageBox.Show("Формирование планов обслуживания долга по всем договорам успешно завершено", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FinSourcePlanningException ex)
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
            }
        }
    }
}
