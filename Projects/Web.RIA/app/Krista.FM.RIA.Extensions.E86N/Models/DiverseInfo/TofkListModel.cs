using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.DiverseInfo
{
    public class TofkListModel : ViewModelBase
    {
        [Description("ID")]
        public int ID { get; set; }

        [Description("Наименование ТОФК в котором обслуживается лицевой счет")]
        public string TofkName { get; set; }

        [Description("Адрес ТОФК")]
        public string TofkAddress { get; set; }

        public int RefParameterDoc { get; set; }
    }
}
