using System;
using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    public class ReportsViewModel : ViewModelBase
    {
        public int ID { get; set; }
        
        [DataBaseBindingField(typeof(F_F_Reports), "ReportGuid")]
        public string ReportGuid { get; set; }
        
        [Description("Сведения об отчетном периоде, за который представлен отчет")]
        [DataBaseBindingField(typeof(F_F_Reports), "NameReport")]
        public string NameReport { get; set; }

        [DataBaseBindingField(typeof(F_F_Reports), "HeadName")]
        public string HeadName { get; set; }

        [DataBaseBindingField(typeof(F_F_Reports), "HeadPosition")]
        public string HeadPosition { get; set; }

        [DataBaseBindingField(typeof(F_F_Reports), "DateReport")]
        public DateTime DateReport { get; set; }
    }
}
