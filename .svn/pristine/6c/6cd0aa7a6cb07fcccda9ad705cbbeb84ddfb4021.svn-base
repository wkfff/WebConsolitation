using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.Service2016Model
{
    public class Service2016InstitutionsInfoViewModel : ViewModelBase
    {
        public int ID { get; set; }

        public int RefStructure { get; set; }

        [Description("Полное наименование учреждения")]
        [DataBaseBindingField(typeof(D_Org_Structure), "Name")]
        public string RefStructureName { get; set; }

        [Description("ИНН")]
        [DataBaseBindingField(typeof(D_Org_Structure), "INN")]
        public string RefStructureInn { get; set; }
    }
}
