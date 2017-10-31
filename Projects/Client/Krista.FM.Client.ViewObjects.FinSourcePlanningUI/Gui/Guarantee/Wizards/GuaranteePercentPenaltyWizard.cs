using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee.Wizards
{
    class GuaranteePercentPenaltyWizard : GuaranteePenaltyWizard
    {
        internal GuaranteePercentPenaltyWizard(DataRow activeMasterRow)
            : base()
        {
            DataRow principalContractRow = GuaranteeServer.GetPrincipalContract(activeMasterRow);
            principalContract = new PrincipalContract(principalContractRow, new Server.Guarantees.Guarantee(activeMasterRow));
            guaranteeServer = GuaranteeServer.GetGuaranteeServer(principalContract.RefOkv, FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
        }

        protected override void GetDetailsData(DataTable dtJournalPercent, ref DataTable dtPlan, ref DataTable dtFact,
            ref DataTable dtPenalties, ref Decimal currentPercent)
        {
            dtPlan = guaranteeServer.GetServicePlan(principalContract.Guarantee.ID);
            dtFact = guaranteeServer.GetPercentFact(principalContract.Guarantee.ID);
            dtPenalties = guaranteeServer.GetPercentPenaltyTable(principalContract.Guarantee.ID);

            currentPercent = dtJournalPercent.Rows[dtJournalPercent.Rows.Count - 1]["PenaltyPercent"] is DBNull ?
                -1 :
                Convert.ToDecimal(dtJournalPercent.Rows[dtJournalPercent.Rows.Count - 1]["PenaltyPercent"]);
            if (principalContract.PenaltyPercentRate != 0)
                penaltyRate = principalContract.PenaltyPercentRate;
            RenameWizardForPercent();
        }

        private void RenameWizardForPercent()
        {
            wizardWelcomePage1.Description = "Мастер начисления пени по процентам";
            wizardWelcomePage1.Title = "Мастер начисления пени по процентам";
            base.Text = "Начисление пени по процентам";
        }

        protected override void FillPenalty(DateTime endOfMonth, decimal penalty, decimal currencyPenalty, decimal exchangeRate)
        {
            guaranteeServer.AddPercentPenalty(principalContract.Guarantee.ID,
                endOfMonth, baseYear, currentPenaltyPayment, penalty,
                currencyPenalty, Convert.ToInt32(uteDaysCount.Text),
                Convert.ToDecimal(unePercent.Value),
                penaltyRate, exchangeRate, principalContract.Guarantee.RefOKV);
        }
    }
}
