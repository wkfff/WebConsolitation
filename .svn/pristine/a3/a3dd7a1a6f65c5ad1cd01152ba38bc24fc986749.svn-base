using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    public class F_F_PNRZnach2016 : FactTable
    {
        public static readonly string Key = "17cd6332-b391-41ec-bb6f-3667f95cbf0e";
        
        private IList<F_F_AveragePrice> averagePrices;

        public F_F_PNRZnach2016()
        {
            averagePrices = new List<F_F_AveragePrice>();
        }

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Protklp { get; set; }

        public virtual string ReportingYear { get; set; }

        public virtual string CurrentYear { get; set; }

        public virtual string ComingYear { get; set; }

        public virtual string FirstPlanYear { get; set; }

        public virtual string SecondPlanYear { get; set; }

        public virtual string ActualValue { get; set; }

        public virtual string Deviation { get; set; }

        public virtual string Reject { get; set; }

        public virtual F_F_Reports RefReport { get; set; }

        public virtual F_F_ServiceIndicators RefIndicators { get; set; }

        public virtual F_F_GosZadanie2016 RefFactGZ { get; set; }

        public virtual Decimal? AveragePriceFact { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefVolumeIndex")]
        public virtual IList<F_F_AveragePrice> AveragePrices
        {
            get { return averagePrices; }
            set { averagePrices = value; }
        }
    }
}
