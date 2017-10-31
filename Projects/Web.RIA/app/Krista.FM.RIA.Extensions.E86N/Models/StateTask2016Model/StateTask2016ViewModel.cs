using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    public class StateTask2016ViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_GosZadanie2016), "AveragePrice")]
        public decimal? AveragePrice { get; set; }

        [DataBaseBindingField(typeof(F_F_GosZadanie2016), "OrdinalNumber")]
        public int OrdinalNumber { get; set; }

        public int RefParameter { get; set; }

        public int RefService { get; set; }

        public int RefServiceOld { get; set; }

        [Description("Наименование услуги")]
        [DataBaseBindingField(typeof(D_Services_Service), "NameName")]
        public string RefServiceName { get; set; }

        [Description("Код услуги")]
        [DataBaseBindingField(typeof(D_Services_Service), "NameCode")]
        public string RefServiceCode { get; set; }

        public int RefServiceType { get; set; }

        [Description("Услуга/работа")]
        [DataBaseBindingField(typeof(FX_FX_ServiceType), "Name")]
        public string RefServiceTypeName { get; set; }

        [Description("Код вида услуги")]
        [DataBaseBindingField(typeof(FX_FX_ServiceType), "Code")]
        public string RefServiceTypeCode { get; set; }

        public int RefServicePay { get; set; }

        [Description("Платность")]
        [DataBaseBindingField(typeof(FX_FX_ServicePayType2), "Name")]
        public string RefServicePayName { get; set; }

        [Description("Код вида платности")]
        [DataBaseBindingField(typeof(FX_FX_ServicePayType2), "Code")]
        public string RefServicePayCode { get; set; }

        [DataBaseBindingField(typeof(D_Services_Service), "Regrnumber")]
        public string RefServiceRegNum { get; set; }

        [Description("Уникальный номер по базовому перечню")]
        public string RefServiceUniqueNumber { get; set; }

        [Description("Наименование показателя, характеризующего содержание услуги")]
        public string RefServiceContentIndex { get; set; }

        [Description("Наименование показателя, характеризующего условия услуги")]
        public string RefServiceConditionIndex { get; set; }

        public bool IsOtherSources { get; set; }
    }
}
