using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using System.Data;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class PenaltyWizardCommand : AbstractCommand
    {
        protected string detailKey;

        public override void Run()
        {
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
            BaseClsView vo = (BaseClsView)content.ViewCtrl;
            DataRow activeRow = content.GetActiveDataRow();
            if (activeRow == null)
                return;
            FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
            
            if (activeRow.IsNull("EndDate"))
            {
                MessageBox.Show("Поле \"Дата окончательного погашения кредита\" не заполнено. Заполните это поле и повторите попытку", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (vo.ugeCls.ugData.ActiveRow != null)
            {
                DataTable dtJournalPercent = planningServer.GetCreditPercents(Convert.ToInt32(activeRow["ID"]));
                if (dtJournalPercent.Rows.Count == 0)
                {
                    MessageBox.Show(FinSourcePlanningNavigation.Instance.Workplace.WindowHandle,
                        "Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"",
                        "Расчет пени", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ShowWizard(planningServer, activeRow);
                content.RefreshDetail(detailKey);
            }
        }

        protected virtual void ShowWizard(FinSourcePlanningServer planningServer, DataRow activeRow)
        {

        }
    }

    public class DebtPenaltyWizardCommand : PenaltyWizardCommand
    {
        protected DebtPenaltyWizardCommand()
		{
            key = "DebtPenaltyWizardCommand";
			caption = "Мастер начисления пени по основному долгу";
		}
        /*
        public override void Run()
        {
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
            FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
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
                DataTable dtJournalPercent = planningServer.GetCreditPercents(Convert.ToInt32(content.GetActiveDataRow()["ID"]));
                if (dtJournalPercent.Rows.Count == 0)
                {
                    MessageBox.Show(FinSourcePlanningNavigation.Instance.Workplace.WindowHandle,
                        "Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"",
                        "Расчет пени", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CalculatePenaltiesWizard wizard = new CalculatePenaltiesWizard(planningServer, content.GetActiveDataRow(), FinSourcePlanningNavigation.BaseYear, true);
                wizard.ShowDialog();
                content.RefreshDetail(detailKey);                                          
            }
        }*/
        
        protected override void ShowWizard(FinSourcePlanningServer planningServer, DataRow activeRow)
        {
            CalculatePenaltiesWizard wizard = new CalculatePenaltiesWizard(planningServer, activeRow, FinSourcePlanningNavigation.BaseYear, true);
            wizard.ShowDialog();
        }
    }

    public class DebtPenaltyWizardCommandCI : DebtPenaltyWizardCommand
    {
        public DebtPenaltyWizardCommandCI()
            : base()
        {
            detailKey = SchemeObjectsKeys.t_S_ChargePenaltyDebtCI_Key;
        }
    }

    public class DebtPenaltyWizardCommandCO : DebtPenaltyWizardCommand
    {
        public DebtPenaltyWizardCommandCO()
            : base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_ChargePenaltyDebtCO;
        }
    }

    public class PercentPenaltyWizardCommand : PenaltyWizardCommand
    {
        protected bool IsMainDebt
        {
            get; set;
        }

        protected PercentPenaltyWizardCommand()
        {
            key = "PercentPenaltyWizardCommand";
            caption = "Мастер начисления пени по процентам";
        }
        /*
        public override void Run()
        {
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
            BaseClsView vo = (BaseClsView)content.ViewCtrl;
            if (vo.ugeCls.ugData.ActiveRow != null)
            {
                FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
                DataTable dtJournalPercent = planningServer.GetCreditPercents(Convert.ToInt32(content.GetActiveDataRow()["ID"]));
                if (dtJournalPercent.Rows.Count == 0)
                {
                    MessageBox.Show(FinSourcePlanningNavigation.Instance.Workplace.WindowHandle,
                        "Для выполнения операции необходимо заполнить деталь \"Журнал ставок процентов\"",
                        "Расчет пени", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CalculatePenaltiesWizard wizard = new CalculatePenaltiesWizard(planningServer, content.GetActiveDataRow(), FinSourcePlanningNavigation.BaseYear, false);
                wizard.ShowDialog();
                content.RefreshDetail(detailKey);
            }
        }*/

        protected override void ShowWizard(FinSourcePlanningServer planningServer, DataRow activeRow)
        {
            CalculatePenaltiesWizard wizard = new CalculatePenaltiesWizard(planningServer, activeRow, FinSourcePlanningNavigation.BaseYear, IsMainDebt);
            wizard.ShowDialog();
        }
    }

    public class CIPercentPenaltyWizardCommand : PercentPenaltyWizardCommand
    {
        public CIPercentPenaltyWizardCommand()
            : base()
        {
            detailKey = SchemeObjectsKeys.t_S_ChargePenaltyPercentCI_Key;
            IsMainDebt = false;
        }
    }

    public class COPercentPenaltyWizardCommand : PercentPenaltyWizardCommand
    {
        public COPercentPenaltyWizardCommand()
            : base()
        {
            detailKey = CreditIssuedObjectsKeys.t_S_ChargePenaltyPercentCO;
            IsMainDebt = false;
        }
    }
}
