using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    public class InformingProcedureViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_InfoProcedure2016), "Method")]
        public string Method { get; set; }

        [DataBaseBindingField(typeof(F_F_InfoProcedure2016), "Content")]
        public string Content { get; set; }

        [DataBaseBindingField(typeof(F_F_InfoProcedure2016), "Rate")]
        public string Rate { get; set; }
    }
}
