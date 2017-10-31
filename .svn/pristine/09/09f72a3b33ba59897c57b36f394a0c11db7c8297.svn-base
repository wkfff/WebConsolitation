using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    public class AveragePriceViewModel : ViewModelBase
    {
        public int ID { get; set; }
        
        public int RefVolumeIndex { get; set; }

        [Description("Наименование показателя")]
        [DataBaseBindingField(typeof(F_F_ServiceIndicators), "Name")]
        public string RefVolumeIndexName { get; set; }

        [DataBaseBindingTable(typeof(F_F_AveragePrice))]
        public virtual decimal? ReportYearDec { get; set; }

        [DataBaseBindingTable(typeof(F_F_AveragePrice))]
        public virtual decimal? CurrentYearDec { get; set; }

        [DataBaseBindingTable(typeof(F_F_AveragePrice))]
        public virtual decimal? NextYearDec { get; set; }

        [DataBaseBindingTable(typeof(F_F_AveragePrice))]
        public virtual decimal? PlanFirstYearDec { get; set; }

        [DataBaseBindingTable(typeof(F_F_AveragePrice))]
        public virtual decimal? PlanLastYearDec { get; set; }
    }
}
