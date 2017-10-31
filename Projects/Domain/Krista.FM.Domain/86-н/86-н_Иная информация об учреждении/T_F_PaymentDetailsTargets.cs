namespace Krista.FM.Domain
{
    public class T_F_PaymentDetailsTargets : DomainObject
    {
        public static readonly string Key = "44c3298d-c819-463b-8a78-d4536f5b1212";

        public virtual string PaymentType { get; set; }
        
        public virtual string PaymentTargetName { get; set; }

        public virtual string Kbk { get; set; }

        public virtual T_F_PaymentDetails RefPaymentDetails { get; set; }
    }
}
