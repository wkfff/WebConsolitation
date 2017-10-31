using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    public class ConsumersCategoryViewModel : ViewModelBase
    {
        public int ID { get; set; }

        public int RefFactGZ { get; set; }

        public int RefConsumersCategory { get; set; }

        [DataBaseBindingField(typeof(F_F_ServiceConsumersCategory), "Name")]
        [Description("Категория потребителей")]
        public string RefConsumersCategoryName { get; set; }
    }
}
