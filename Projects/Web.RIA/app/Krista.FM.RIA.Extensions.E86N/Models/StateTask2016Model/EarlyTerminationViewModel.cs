using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    [Description("Основание для досрочного прекращения")]
    public class EarlyTerminationViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_BaseTermination2016), "EarlyTerminat")]
        public string EarlyTerminat { get; set; }
    }
}
