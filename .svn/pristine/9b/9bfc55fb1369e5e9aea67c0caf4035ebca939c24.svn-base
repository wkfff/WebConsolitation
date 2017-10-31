using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.Service2016Model
{
    public class Service2016ConsumersCategoryViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_ServiceConsumersCategory), "Code")]
        public string Code { get; set; }

        [Description("Категории потребителей")]
        [DataBaseBindingField(typeof(F_F_ServiceConsumersCategory), "Name")]
        public string Name { get; set; }
    }
}
