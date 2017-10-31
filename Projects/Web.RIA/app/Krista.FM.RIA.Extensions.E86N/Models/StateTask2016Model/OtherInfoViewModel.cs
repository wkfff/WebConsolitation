using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    [Description("Иная информация")]
    public class OtherInfoViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_OtherInfo), "OtherInfo")]
        public string OtherInfo { get; set; }
    }
}
