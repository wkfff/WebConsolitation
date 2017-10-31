using System;
using System.Linq.Expressions;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel
{
    // Описания взяты из FX_FX_StateValue
    public class FinNFinAssetsViewModel : ViewModelBase
    {
        public int ID { get; set; }

        public int? AmountOfDamageCompensationID { get; set; }

        [DataBaseDescription("Общая сумма требований на возмещение ущерба по недостачам и хищениям материальных ценностей / денежных средств, а также порчи материальных ценностей (руб.)", 8)]
        public decimal? AmountOfDamageCompensation { get; set; }

        public int? InfAboutCarryingValueTotalID { get; set; }

        [DataBaseDescription("Сведения об изменении балансовой стоимости нефинансовых активов за отчетный год, ВСЕГО (%)", 0)]
        public decimal? InfAboutCarryingValueTotal { get; set; }

        public int? InfAboutCarryingValueTotalRefTypeIxm { get; set; }

        public string InfAboutCarryingValueTotalRefTypeIxmName { get; set; }

        public int? ImmovablePropertyID { get; set; }

        [DataBaseDescription("Балансовой стоимости недвижимого имущества (%)", 1)]
        public decimal? ImmovableProperty { get; set; }

        public int? ImmovablePropertyRefTypeIxm { get; set; }

        public string ImmovablePropertyRefTypeIxmName { get; set; }

        public int? ParticularlyValuablePropertyID { get; set; }

        [DataBaseDescription("Балансовой стоимости особо ценного движимого имущества (%)", 2)]
        public decimal? ParticularlyValuableProperty { get; set; }

        public int? ParticularlyValuablePropertyRefTypeIxm { get; set; }

        public string ParticularlyValuablePropertyRefTypeIxmName { get; set; }

        public int? ChangingArrearsTotalID { get; set; }

        [DataBaseDescription("Изменение дебиторской задолженности за отчетный год, ВСЕГО (%)", 3)]
        public decimal? ChangingArrearsTotal { get; set; }

        public int? ChangingArrearsRefTypeIxm { get; set; }

        public string ChangingArrearsRefTypeIxmName { get; set; }

        public int? IncomeID { get; set; }

        [DataBaseDescription("По доходам (поступлениям) (%)", 4)]
        public decimal? Income { get; set; }

        public int? IncomeRefTypeIxm { get; set; }

        public string IncomeRefTypeIxmName { get; set; }

        public int? ExpenditureID { get; set; }

        [DataBaseDescription("По расходам (выплатам) (%)", 5)]
        public decimal? Expenditure { get; set; }

        public int? ExpenditureRefTypeIxm { get; set; }

        public string ExpenditureRefTypeIxmName { get; set; }

        public int? IncreaseInAccountsPayableTotalID { get; set; }

        [DataBaseDescription("Изменение кредиторской задолженности за отчетный год, ВСЕГО (%)", 6)]
        public decimal? IncreaseInAccountsPayableTotal { get; set; }

        public int? IncreaseInAccountsPayableTotalRefTypeIxm { get; set; }

        public string IncreaseInAccountsPayableTotalRefTypeIxmName { get; set; }

        public int? OverduePayablesID { get; set; }

        [DataBaseDescription("Просроченной кредиторской задолженности (%)", 7)]
        public decimal? OverduePayables { get; set; }

        public int? OverduePayablesRefTypeIxm { get; set; }

        public string OverduePayablesRefTypeIxmName { get; set; }

        public int DescriptionIdOf<T>(Expression<Func<T>> expr)
        {
            return UiBuilders.DescriptionIdOf(expr);
        }
    }
}
