    using System;

namespace Krista.FM.Domain
{
    public class F_S_SchBGuarantissued : DebtorBookFactBase
	{
		public const string Key = "6930d45e-89a3-4f28-b1c4-b28502593750";
        
		public virtual int PumpID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string RegNum { get; set; }
		public virtual DateTime? StartDate { get; set; }
		public virtual string Occasion { get; set; }
		public virtual Decimal Sum { get; set; }
		public virtual string DateDoc { get; set; }
		public virtual string EndDate { get; set; }
		public virtual string Collateral { get; set; }
		public virtual string Regress { get; set; }
		public virtual Decimal UpDebt { get; set; }
		public virtual Decimal UpService { get; set; }
		public virtual Decimal DownDebt { get; set; }
		public virtual Decimal DownService { get; set; }
		public virtual Decimal CapitalDebt { get; set; }
		public virtual Decimal StalePrincipalDebt { get; set; }
		public virtual Decimal StaleGarantDebt { get; set; }
		public virtual string Num { get; set; }
		public virtual Decimal? DownGarant { get; set; }
		public virtual Decimal TotalDebt { get; set; }
		public virtual Decimal ServiceDebt { get; set; }
		public virtual string FurtherConvention { get; set; }
		public virtual bool FromFinSource { get; set; }
		public virtual DateTime? PrincipalEndDate { get; set; }
		public virtual string PrincipalNum { get; set; }
		public virtual Decimal BgnYearDebt { get; set; }
		public virtual Decimal RemnsBgnMnthDbt { get; set; }
		public virtual Decimal RemnsEndMnthDbt { get; set; }
		public virtual DateTime? PrincipalStartDate { get; set; }
		public virtual DateTime? ChargeDate { get; set; }
		public virtual string Creditor { get; set; }
		public virtual string Principal { get; set; }
		public virtual string DateDemand { get; set; }
		public virtual string DatePerformance { get; set; }
		public virtual string PrincipalCondition { get; set; }
		public virtual Decimal? BgnPenalty { get; set; }
		public virtual Decimal? DownPrincipal { get; set; }
		public virtual Decimal? DownPenalty { get; set; }
		public virtual Decimal? CommisionUp { get; set; }
		public virtual Decimal? CommisionDown { get; set; }
		public virtual Decimal? StalePenalty { get; set; }
		public virtual Decimal? BgnInterest { get; set; }
		public virtual Decimal? UpPenalty { get; set; }
		public virtual Decimal? StaleCommision { get; set; }
		public virtual Decimal? StaleInterest { get; set; }
		public virtual Decimal? EndPenalty { get; set; }
		public virtual Decimal? EndInterest { get; set; }
		public virtual Decimal? ComissionEndPeriod { get; set; }
		public virtual string Purpose { get; set; }
		public virtual string CreditPercent { get; set; }
		public virtual DateTime? StartCreditDate { get; set; }
		public virtual DateTime? EndCreditDate { get; set; }
		public virtual string RenewalNum { get; set; }
		public virtual DateTime? RenewalDate { get; set; }
		public virtual Decimal? OldAgreementNum { get; set; }
		public virtual DateTime? OldAgreementDate { get; set; }
		public virtual string Garant { get; set; }
		public virtual Decimal? BgnStaleInterest { get; set; }
		public virtual Decimal? BgnStalePenalty { get; set; }
		public virtual Decimal? BgnStaleComission { get; set; }
		public virtual Decimal? BgnStaleDebt { get; set; }
		public virtual string OldAgrNum { get; set; }
		public virtual string PurposeGarantee { get; set; }
		public virtual string Note { get; set; }
		public virtual Decimal DownPenaltyGarant { get; set; }
		public virtual Decimal? DownServiceGarant { get; set; }
		public virtual int? IDDoc { get; set; }
		public virtual D_Organizations_Plan RefOrganizations { get; set; }
		public virtual D_Organizations_Plan RefOrganizationsPlan3 { get; set; }
		public virtual D_OKV_Currency RefOKV { get; set; }
		public virtual D_S_TypeContract RefTypeContract { get; set; }
	}
}
