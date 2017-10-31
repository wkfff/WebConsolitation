namespace Krista.FM.Domain
{
    public class FX_FX_PaymentDetailsType : ClassifierTable
    {
        /// <summary>
        /// Расчетный счет в КО
        /// </summary>
        public const string BankAccount = "01";

        /// <summary>
        /// Лицевой счет в ОрФК
        /// </summary>
        public const string OrfkAccount = "02";

        /// <summary>
        /// Лицевой счет в ФО
        /// </summary>
        public const string FoAccount = "03";

        public static readonly string Key = "381c1c96-aa64-45d0-847e-777de189d4b9";

        public virtual int RowType { get; set; }
        
        public virtual string Code { get; set; }
        
        public virtual string Name { get; set; }
    }
}
