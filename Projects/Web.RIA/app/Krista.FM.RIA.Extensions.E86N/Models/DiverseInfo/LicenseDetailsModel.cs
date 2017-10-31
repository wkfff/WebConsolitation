using System;
using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Models.DiverseInfo
{
    public class LicenseDetailsModel : ViewModelBase
    {
        [Description("ID")]
        public int ID { get; set; }

        [Description("Наименование органа выдавшего лицензию")]
        public string LicenseAgencyName { get; set; }

        [Description("Наименование лицензируемого вида деятельности")]
        public string LicenseName { get; set; }

        [Description("Номер лицензии")]
        public string LicenseNum { get; set; }

        [Description("Дата регистрации")]
        public DateTime LicenseDate { get; set; }

        [Description("Срок действия лицензии")]
        public DateTime? LicenseExpDate { get; set; }

        public int RefParameterDoc { get; set; }
    }
}
