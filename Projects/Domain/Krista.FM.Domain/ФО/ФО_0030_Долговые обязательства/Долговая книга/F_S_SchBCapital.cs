using System;

namespace Krista.FM.Domain
{
    public class F_S_SchBCapital : DebtorBookFactBase
	{
		public const string Key = "328a93cf-9769-4980-97e3-32570636b125";

		public virtual int PumpID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string CodeCapital { get; set; }
		public virtual string OfficialNumber { get; set; }
		public virtual string RegNumber { get; set; }
		public virtual DateTime? RegEmissionDate { get; set; }
		public virtual string NameNPA { get; set; }
		public virtual string NameOrg { get; set; }
		public virtual DateTime? DateNPA { get; set; }
		public virtual string NumberNPA { get; set; }
		public virtual string Owner { get; set; }
		public virtual Decimal? Nominal { get; set; }
		public virtual Decimal? NominalCurrency { get; set; }
		public virtual Decimal? Sum { get; set; }
		public virtual Decimal? CurrencySum { get; set; }
		public virtual DateTime? StartDate { get; set; }
		public virtual Decimal? ExstraIssueSum { get; set; }
		public virtual DateTime? DatePartDischarge { get; set; }
		public virtual int? CodeTransh { get; set; }
		public virtual DateTime? DateDischarge { get; set; }
		public virtual Decimal? IssueSum { get; set; }
		public virtual DateTime? DueDate { get; set; }
		public virtual Decimal? Coupon { get; set; }
		public virtual Decimal? Income { get; set; }
		public virtual Decimal? FactServiceSum { get; set; }
		public virtual Decimal? DebtFactService { get; set; }
		public virtual Decimal? Discount { get; set; }
		public virtual string Depository { get; set; }
		public virtual string Trade { get; set; }
		public virtual Decimal? FactDiscountSum { get; set; }
		public virtual Decimal? TotalSum { get; set; }
		public virtual bool FromFinSource { get; set; }
		public virtual Decimal BgnYearDbt { get; set; }
		public virtual Decimal BgnYearInterest { get; set; }
		public virtual Decimal BgnYearPenlt { get; set; }
		public virtual Decimal RemnsBgnMnthDbt { get; set; }
		public virtual Decimal RemnsEndMnthDbt { get; set; }
		public virtual DateTime? ChargeDate { get; set; }
		public virtual Decimal Discharge { get; set; }
		public virtual Decimal RemnsBgnMnthInterest { get; set; }
		public virtual Decimal PlanService { get; set; }
		public virtual Decimal FactService { get; set; }
		public virtual Decimal RemnsEndMnthInterest { get; set; }
		public virtual Decimal Attract { get; set; }
		public virtual string GenAgent { get; set; }
		public virtual string Collateral { get; set; }
		public virtual Decimal StaleDebt { get; set; }
		public virtual Decimal StaleInterest { get; set; }
		public virtual Decimal StalePenlt { get; set; }
		public virtual Decimal RemnsBgnMnthPenlt { get; set; }
		public virtual Decimal ChargePenlt { get; set; }
		public virtual Decimal FactPenlt { get; set; }
		public virtual Decimal RemnsEndMnthPenlt { get; set; }
		public virtual string FormCap { get; set; }
		public virtual string PeriodIncome { get; set; }
		public virtual string Reason { get; set; }
		public virtual DateTime? AddDate { get; set; }
		public virtual DateTime? GetBackDate { get; set; }
		public virtual Decimal? StaleBgnYear { get; set; }
		public virtual DateTime? DebtStartDate { get; set; }
		public virtual DateTime? PaymentDate { get; set; }
		public virtual string Purpose { get; set; }
        public virtual int? Count { get; set; }
		public virtual string Note { get; set; }
		public virtual Decimal? DischargePrice { get; set; }
		public virtual D_Organizations_Plan RefOrg { get; set; }
		public virtual D_OKV_Currency RefOKV { get; set; }
		public virtual D_S_Capital RefSCap { get; set; }
		public virtual FX_S_KindCapital RefKindCapital { get; set; }
		public virtual FX_S_FormCapital RefFofmCapital { get; set; }
    }
}
