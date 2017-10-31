using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    [Description("Порядок контроля за исполнением")]
    public class SupervisionProcedureViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_OrderControl2016), "Form")]
        public string Form { get; set; }

        [DataBaseBindingField(typeof(F_F_OrderControl2016), "Rate")]
        public string Rate { get; set; }

        [DataBaseBindingField(typeof(F_F_OrderControl2016), "Supervisor")]
        public string Supervisor { get; set; }
    }
}
