using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class FillTranshCommand : AbstractCommand
    {
        private const string detailKey = CapitalObjectKeys.t_S_CPTransh_Key;

        public FillTranshCommand()
        {
            key = "btnFillTransh";
            caption = "Сформировать транши";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            UltraGridRow masterRow = ((BaseClsView) content.ViewCtrl).ugeCls.ugData.ActiveRow;
            if (masterRow == null || content.GetActiveDataRow().RowState == DataRowState.Added)
            {
                MessageBox.Show("Активная запись мастер-таблицы не задана или не сохранена в базу данных",
                    "Формирование детали траншей", MessageBoxButtons.OK);
                return;
            }
            CapitalServer capitalServer = new CapitalServer();
            CapitalTranshCreateWizard wizard = new CapitalTranshCreateWizard(capitalServer, content.GetActiveDataRow());
            wizard.ShowDialog();
            content.RefreshDetail(detailKey);
        }
    }

    public class FillServicePlanCommand : AbstractCommand
    {
        private const string detailKey = CapitalObjectKeys.t_S_CPPlanService;

        public FillServicePlanCommand()
        {
            key = "btnFillServicePlan";
            caption = "Сформировать план выплаты дохода";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow masterRow = content.GetActiveDataRow();
            if (masterRow == null || masterRow.RowState == DataRowState.Added)
            {
                MessageBox.Show("Активная запись мастер-таблицы не задана или не сохранена в базу данных",
                    "Формирование детали плана выплаты дохода", MessageBoxButtons.OK);
                return;
            }

            if (Utils.GetRowsCount(CapitalObjectKeys.t_S_CPJournalPercent,
                FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme, string.Format("RefCap = {0}", masterRow["ID"])) == 0)
            {
                MessageBox.Show("Деталь 'Журнале процентов' не заполнена. Пожалуйста запустите мастер для заполнения этой детали",
                    "Формирование детали плана выплаты дохода", MessageBoxButtons.OK);
                return;
            }

            if (Utils.GetRowsCount(CapitalObjectKeys.t_S_CPPlanCapital_Key,
                FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme, string.Format("RefCap = {0}", masterRow["ID"])) == 0)
            {
                MessageBox.Show("Деталь 'План размещения' не заполнена",
                    "Формирование детали плана выплаты дохода", MessageBoxButtons.OK);
                return;
            }

            CapitalServer server = new CapitalServer();

            server.FillPaymentIncomePlan(new Stock(masterRow), FinSourcePlanningNavigation.Instance.CurrentSourceID,
                                         FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
            content.RefreshDetail(detailKey);
        }
    }

    public class PayingOffNominalPlanCommand : AbstractCommand
    {
        private const string detailKey = CapitalObjectKeys.t_S_CPPlanDebt_Key;

        public PayingOffNominalPlanCommand()
        {
            key = "btnPayingOffNominalPlan";
            caption = "Сформировать план погашения номинальной стоимости";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow activeRow = content.GetActiveDataRow();
            if (activeRow == null || activeRow.RowState == DataRowState.Added)
            {
                MessageBox.Show("Активная запись мастер-таблицы не задана или не сохранена в базу данных",
                    "Формирование детали плана погашения номинальной стоимости", MessageBoxButtons.OK);
                return;
            }
            NominalValueRepaymentPlanWizard wizard = new NominalValueRepaymentPlanWizard(activeRow);
            wizard.ShowDialog();
            content.RefreshDetail(detailKey);
        }
    }

    public class AllocationPlanCommand : AbstractCommand
    {
        private const string detailKey = CapitalObjectKeys.t_S_CPPlanCapital_Key;

        public AllocationPlanCommand()
        {
            key = "btnAllocationPlan";
            caption = "Сформировать план размещения";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            UltraGridRow masterRow = ((BaseClsView)content.ViewCtrl).ugeCls.ugData.ActiveRow;
            if (masterRow == null || content.GetActiveDataRow().RowState == DataRowState.Added)
            {
                MessageBox.Show("Активная запись мастер-таблицы не задана или не сохранена в базу данных",
                    "Формирование детали плана размещения", MessageBoxButtons.OK);
                return;
            }
            CapitalServer server = new CapitalServer();
            server.FillAllocationPlan(content.GetActiveDataRow(), FinSourcePlanningNavigation.BaseYear);
            content.RefreshDetail(detailKey);
        }
    }

    public class FillPercentsCommand : AbstractCommand
    {
        private const string detailKey = CapitalObjectKeys.t_S_CPJournalPercent;

        public FillPercentsCommand()
        {
            key = "btnPercents";
            caption = "Сформировать журнал ставок процентов";
        }

        public override void Run()
        {
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow masterRow = content.GetActiveDataRow();
            if (masterRow == null || masterRow.RowState == DataRowState.Added)
            {
                MessageBox.Show("Активная запись мастер-таблицы не задана или не сохранена в базу данных",
                    "Формирование детали журнала ставок процентов", MessageBoxButtons.OK);
                return;
            }

            StringBuilder messages = new StringBuilder();

            if (Utils.GetRowsCount(CapitalObjectKeys.t_S_CPPlanCapital_Key,
                FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme, string.Format("RefCap = {0}", masterRow["ID"])) == 0)
            {
                messages.AppendLine("Деталь 'План размещения' не заполнена");
            }

            if (Utils.GetRowsCount(CapitalObjectKeys.t_S_CPPlanDebt_Key,
                FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme, string.Format("RefCap = {0}", masterRow["ID"])) == 0)
            {
                messages.AppendLine("Деталь 'План погашения номинальной стоимости' не заполнена");
            }
            if (messages.Length > 0)
            {
                messages.AppendLine("Продолжить формирование детали?");
                if (MessageBox.Show(messages.ToString(),
                    "Формирование детали журнала ставок процентов", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                    return;
            }

            PercentsWizard wizard = new PercentsWizard(masterRow);
            wizard.ShowDialog();
            content.RefreshDetail(detailKey);
        }
    }

    public class CapitalVariantTransfer : CreditVariantTransferCommand
    {
        public CapitalVariantTransfer()
            : base()
        {
            creditObjectKey = SchemeObjectsKeys.f_S_Capital_Key;
        }
    }
}
