using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    public class T_F_PaymentDetails : DomainObject
    {
        public static readonly string Key = "b63ec193-5290-460a-9e85-65c3e21ac7fe";
        
        private IList<T_F_PaymentDetailsTargets> paymentDetailsTargets;

        public T_F_PaymentDetails()
        {
            paymentDetailsTargets = new List<T_F_PaymentDetailsTargets>();
        }
        
        public virtual string TofkName { get; set; }

        public virtual string BankName { get; set; }

        public virtual string BankCity { get; set; }

        public virtual string Bik { get; set; }

        public virtual string CorAccountCode { get; set; }

        public virtual string CalcAccountCode { get; set; }

        public virtual F_F_ParameterDoc RefParameterDoc { get; set; }

        public virtual FX_FX_PaymentDetailsType RefPaymentDetailsType { get; set; }

        public virtual string PersonalAccountCode { get; set; }

        public virtual string AccountName { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefPaymentDetails")]
        public virtual IList<T_F_PaymentDetailsTargets> PaymentDetailsTargets
        {
            get { return paymentDetailsTargets; }
            set { paymentDetailsTargets = value; }
        }
    }
}
