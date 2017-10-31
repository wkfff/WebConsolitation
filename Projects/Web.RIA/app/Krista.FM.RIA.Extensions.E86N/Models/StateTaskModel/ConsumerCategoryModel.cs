using System.ComponentModel;

using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTaskModel
{
    [Description("Категории потребителей")]
    public class ConsumerCategoryModel : ViewModelBase
    {
        public int ID { get; set; }

        public int RefFactGZ { get; set; }

        public int RefServicesCPotr { get; set; }

        [DataBaseBindingField(typeof(D_Services_CPotr), "Name")]
        [Description("Категория потребителей")]
        public string RefServicesCPotrName { get; set; }
    }
}
