using System;

namespace Krista.FM.RIA.Extensions.E86N.Models
{
    public class EnactmentViewModel : ViewModelBase
    {
        public string type { get; set; }

        public string name { get; set; }

        public string number { get; set; }

        public DateTime? date { get; set; }
    }
}
