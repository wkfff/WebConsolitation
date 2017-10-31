using System;
using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models
{
    [Description("Показатели объема и качества")]
    public class IndicatorsServiceModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(D_Services_Indicators), "Name")]
        public string Name { get; set; }

        public int RefCharacteristicType { get; set; }

        [DataBaseBindingField(typeof(FX_FX_CharacteristicType), "Name")]
        [Description("Тип показателя")]
        public string RefCharacteristicTypeName { get; set; }

        public int RefOKEI { get; set; }

        [DataBaseBindingField(typeof(D_Org_OKEI), "Name")]
        [Description("ОКЕИ")]
        public string RefOKEIName { get; set; }
    }
}
