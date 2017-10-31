using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee.Wizards
{
    class GuaranteeDebtPenaltyWizard : GuaranteePenaltyWizard
    {
        internal GuaranteeDebtPenaltyWizard(DataRow activeMasterRow)
            : base()
        {
            DataRow principalContractRow = GuaranteeServer.GetPrincipalContract(activeMasterRow);
            principalContract = new PrincipalContract(principalContractRow, new Server.Guarantees.Guarantee(activeMasterRow));
            guaranteeServer = GuaranteeServer.GetGuaranteeServer(principalContract.RefOkv, FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
        }

        protected override void GetDetailsData(DataTable dtJournalPercent, ref DataTable dtPlan, ref DataTable dtFact,
            ref DataTable dtPenalties, ref Decimal currentPercent)
        {
            dtPlan = guaranteeServer.GetDebtPlan(principalContract.Guarantee.ID);
            dtFact = guaranteeServer.GetDebtFact(principalContract.Guarantee.ID);
            dtPenalties = guaranteeServer.GetDebtPenaltyTable(principalContract.Guarantee.ID);

            currentPercent = dtJournalPercent.Rows[dtJournalPercent.Rows.Count - 1]["PenaltyDebt"] is DBNull ?
                0 :
                Convert.ToDecimal(dtJournalPercent.Rows[dtJournalPercent.Rows.Count - 1]["PenaltyDebt"]);
            if (principalContract.PenaltyPercentRate != 0)
                penaltyRate = principalContract.PenaltyPercentRate;
            RenameWizardForMainDebt();
        }

        private void RenameWizardForMainDebt()
        {
            wizardWelcomePage1.Description = "Мастер начисления пени по основному долгу";
            wizardWelcomePage1.Title = "Мастер начисления пени по основному долгу";
            base.Text = "Начисление пени по основному долгу";
        }

        protected override void FillPenalty(DateTime endOfMonth, decimal penalty, decimal currencyPenalty, decimal exchangeRate)
        {
            guaranteeServer.AddDebtPenalty(principalContract.Guarantee.ID,
                endOfMonth, baseYear, currentPenaltyPayment, penalty,
                currencyPenalty, Convert.ToInt32(uteDaysCount.Text),
                Convert.ToDecimal(unePercent.Value),
                penaltyRate, exchangeRate, principalContract.Guarantee.RefOKV);
        }
    }
}
