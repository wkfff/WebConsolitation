using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.Service2016Model
{
    public class Service2016InstitutionTypeViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_ServiceInstitutionType), "Code")]
        public string Code { get; set; }

        [DataBaseBindingField(typeof(F_F_ServiceInstitutionType), "Name")]
        public string Name { get; set; }
    }
}
