using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.ServiceRegisterModel
{
    public class ServiceRegisterProviderViewModel : ViewModelBase
    {
        public int ID { get; set; }

        public int RefProvider { get; set; }

        [Description("Наименование организации")]
        public string RefProviderName { get; set; }
    }
}
