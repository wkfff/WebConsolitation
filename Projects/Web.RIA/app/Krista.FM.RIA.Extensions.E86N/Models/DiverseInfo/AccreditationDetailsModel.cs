using System;
using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.DiverseInfo
{
    public class AccreditationDetailsModel : ViewModelBase
    {
        [Description("ID")]
        public int ID { get; set; }

        [Description("Наименование органа выдавшего аккредитацию")]
        public string AccreditationAgencyName { get; set; }

        [Description("Наименование аккредитуемой деятельности")]
        public string AccreditationName { get; set; }

        [Description("Срок действия аккредитации")]
        public DateTime AccreditationExpDate { get; set; }

        public int RefParameterDoc { get; set; }
    }
}
