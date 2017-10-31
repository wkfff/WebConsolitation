using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models
{
    public class OKVEDYViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [Description("Наименование вида деятельности (по уставу)")]
        public string Name { get; set; }

        public int? RefOkved { get; set; }

        public string RefOkvedName { get; set; }

        public int? RefPrOkved { get; set; }

        public string RefPrOkvedName { get; set; }
    }
}
