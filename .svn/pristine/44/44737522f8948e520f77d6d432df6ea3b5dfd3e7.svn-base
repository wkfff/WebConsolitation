using System.Linq;
using System.Text;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.PfhdModel.Pfhd2017
{
    /// <summary>
    /// Плановые показатели поступлений и выплат
    /// </summary>
    public class PlanPaymentIndexViewModel : ViewModelBase
    {
        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public int ID { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public string Name { get; set; }
        
        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public string LineCode { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public string Kbk { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public decimal? Total { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public decimal? FinancialProvision { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public decimal? SubsidyFinSupportFfoms { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public decimal? SubsidyOtherPurposes { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public decimal? CapitalInvestment { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public decimal? HealthInsurance { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public decimal? ServiceTotal { get; set; }

        [DataBaseBindingTable(typeof(F_F_PlanPaymentIndex))]
        public decimal? ServiceGrant { get; set; }

        public override string ValidateData(int docId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg1 = "Колонка \"{0}\" должна быть больше или равна сумме показателей из колонок:"
                                + " «субсидии на финансовое обеспечение выполнения государственного (муниципального) задания», "
                                + "«субсидии на иные цели», " 
                                + "«субсидии на кап. вложения», "
                                + "«средства ОМС», "
                                + "«поступления от оказания платных услуг (выполнения работ) и от иной приносящей доход деятельности всего». <br>";

            // todo сделать контроль на обязательность по модели
            var message = new StringBuilder();

            var lineCodes = new[] { 100, 180, 200, 260, 300, 500, 600 };

            if (!lineCodes.Contains(int.Parse(LineCode)) && Kbk.IsNullOrEmpty())
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Kbk)));
            }
            
            if (Name.Trim().IsNullOrEmpty())
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Name)));
            }

            if (!Total.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionFromSchemeOf(() => Total)));
            }

            if (Total.GetValueOrDefault() < FinancialProvision.GetValueOrDefault() 
                                            + SubsidyOtherPurposes.GetValueOrDefault() 
                                            + CapitalInvestment.GetValueOrDefault()
                                            + HealthInsurance.GetValueOrDefault()
                                            + ServiceTotal.GetValueOrDefault())
            {
                message.Append(Msg1.FormatWith(DescriptionFromSchemeOf(() => Total)));
            }

            if (ServiceTotal.GetValueOrDefault() < ServiceGrant.GetValueOrDefault())
            {
                message.Append("Значение в колонке \"{0}\" не может быть меньше чем в \"{1}\""
                        .FormatWith(DescriptionFromSchemeOf(() => ServiceTotal), DescriptionFromSchemeOf(() => ServiceGrant)));
            }

            return message.ToString();
        }
    }
}
