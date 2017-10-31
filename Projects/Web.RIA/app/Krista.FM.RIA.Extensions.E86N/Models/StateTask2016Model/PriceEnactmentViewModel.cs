using System;
using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    [Description("Нормативные правовые акты, устанавливающие цены (тарифы) на услугу либо порядок его (ее) установления")]
    public class PriceEnactmentViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_NPACena2016), "VidNPAGZ")]
        public string VidNPAGZ { get; set; }

        [DataBaseBindingField(typeof(F_F_NPACena2016), "OrgUtvDoc")]
        public string OrgUtvDoc { get; set; }

        [DataBaseBindingField(typeof(F_F_NPACena2016), "NumNPA")]
        public string NumNPA { get; set; }

        [DataBaseBindingField(typeof(F_F_NPACena2016), "DataNPAGZ")]
        public DateTime? DataNPAGZ { get; set; }

        [DataBaseBindingField(typeof(F_F_NPACena2016), "Name")]
        public string Name { get; set; }
    }
}
