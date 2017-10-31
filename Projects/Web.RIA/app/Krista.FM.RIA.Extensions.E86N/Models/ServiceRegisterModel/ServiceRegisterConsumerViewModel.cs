using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.ServiceRegisterModel
{
    public class ServiceRegisterConsumerViewModel : ViewModelBase
    {
        public int ID { get; set; }

        public int RefConsumer { get; set; }

        [Description("Категории потребителей")]
        public string RefConsumerName { get; set; }
    }
}
