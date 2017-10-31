using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.Service2016Model
{
    public class Service2016OKPDViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_ServiceOKPD), "Code")]
        public string Code { get; set; }

        [DataBaseBindingField(typeof(F_F_ServiceOKPD), "Name")]
        public string Name { get; set; }
    }
}
