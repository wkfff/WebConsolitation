using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.DiverseInfo
{
    public class PaymentDetailsTargetsModel : ViewModelBase
    {
        [Description("ID")]
        public int ID { get; set; }

        [Description("Вид платежа")]
        public string PaymentType { get; set; }

        [Description("Назначение платежа")]
        public string PaymentTargetName { get; set; }

        [Description("КБК")]
        public string Kbk { get; set; }

        public int RefPaymentDetails { get; set; }
    }
}
