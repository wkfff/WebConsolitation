using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.Service2016Model
{
    public class Service2016IndicatorsViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingTable(typeof(F_F_ServiceIndicators))]
        public string Code { get; set; }

        [DataBaseBindingTable(typeof(F_F_ServiceIndicators))]
        public string Name { get; set; }

        // Из-за неккоректной работы SetComboBoxEditor, произошло несогласование имени ссылки на тип показателя
        public int RefIndType { get; set; }
        
        [Description("Тип показателя")]
        [DataBaseBindingField(typeof(FX_FX_CharacteristicType), "Name")]
        public string RefIndTypeName { get; set; }

        public int RefOKEI { get; set; }

        [Description("Наименование ед. по ОКЕИ")]
        [DataBaseBindingField(typeof(D_Org_OKEI), "Name")]
        public string RefOKEIName { get; set; }
    }
}
