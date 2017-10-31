using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.PfhdModel
{
    public class CapFundsViewModel : ViewModelBase
    {
        [Description("ID")]
        public int ID { get; set; }

        [Description("ШапкаДокумента/Ссылка")]
        public int? RefParameterID { get; set; }

        [Description("Наименование объекта капитального строительства")]
        public string Name { get; set; }

        [Description("Сумма планируемых поступлений, руб")]
        public decimal? Funds { get; set; }
    }
}
