using System.Text;

using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.PfhdModel.Pfhd2017
{
    /// <summary>
    /// Показатели финансового состояния учреждения
    /// </summary>
    public class FinancialIndexViewModel : ViewModelBase
    {
        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? NonFinancialAssets { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? RealAssets { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? RealAssetsDepreciatedCost { get; set; }
        
        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? HighValuePersonalAssets { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? HighValuePADepreciatedCost { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? FinancialAssets { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? MoneyInstitutions { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? FundsAccountsInstitution { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? FundsPlacedOnDeposits { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? OtherFinancialInstruments { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? DebitIncome { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? DebitExpense { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? FinancialCircumstanc { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? Debentures { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? AccountsPayable { get; set; }

        [DataBaseBindingTable(typeof(F_F_FinancialIndex))]
        public decimal? KreditExpired { get; set; }

        public override string ValidateData(int docId)
        {
            const string Msg = "Строка 2.1 должна быть больше или равна сумме входящих в нее строк 2.1.1 + 2.1.2.";

            var message = new StringBuilder();

            if (MoneyInstitutions.GetValueOrDefault() < FundsAccountsInstitution.GetValueOrDefault() + FundsPlacedOnDeposits.GetValueOrDefault())
            {
                message.Append(Msg);
            }

            return message.ToString();
        }
        
        public override ViewModelBase GetModelByDomain(DomainObject domain)
        {
            var domModel = (F_F_FinancialIndex)domain;
            return new FinancialIndexViewModel
                       {
                           NonFinancialAssets = domModel.NonFinancialAssets,
                           RealAssets = domModel.RealAssets,
                           RealAssetsDepreciatedCost = domModel.RealAssetsDepreciatedCost,
                           HighValuePersonalAssets = domModel.HighValuePersonalAssets,
                           HighValuePADepreciatedCost = domModel.HighValuePADepreciatedCost,
                           FinancialAssets = domModel.FinancialAssets,
                           MoneyInstitutions = domModel.MoneyInstitutions,
                           FundsAccountsInstitution = domModel.FundsAccountsInstitution,
                           FundsPlacedOnDeposits = domModel.FundsPlacedOnDeposits,
                           OtherFinancialInstruments = domModel.OtherFinancialInstruments,
                           DebitIncome = domModel.DebitIncome,
                           DebitExpense = domModel.DebitExpense,
                           FinancialCircumstanc = domModel.FinancialCircumstanc,
                           Debentures = domModel.Debentures,
                           AccountsPayable = domModel.AccountsPayable,
                           KreditExpired = domModel.KreditExpired
                       };
        }
    }
}
