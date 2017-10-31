using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.DiverseInfo
{
    public class PaymentDetailsModel : ViewModelBase
    {
        [Description("ID")]
        public int ID { get; set; }

        [Description("Наименование ТОФК")]
        public string TofkName { get; set; }

        [Description("Наименование банка")]
        public string BankName { get; set; }

        [Description("Город в котором расположен банк")]
        public string BankCity { get; set; }

        [Description("БИК")]
        public string Bik { get; set; }

        [Description("Номер корреспондентского счета")]
        public string CorAccountCode { get; set; }

        [Description("Номер расчетного счета")]
        public string CalcAccountCode { get; set; }

        [Description("Номер лицевого счета")]
        public string PersonalAccountCode { get; set; }

        public int RefParameterDoc { get; set; }

        public int RefPaymentDetailsType { get; set; }

        [Description("Тип информации о платежных реквизитах")]
        public string RefPaymentDetailsTypeName { get; set; }

        [DataBaseBindingField(typeof(T_F_PaymentDetails), "AccountName")]
        public string AccountName { get; set; }
    }
}
