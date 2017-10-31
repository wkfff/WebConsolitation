using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    public class F_F_GosZadanie : FactTable
    {
        public static readonly string Key = "5e89ca1c-f287-414d-b216-0e7d14369079";
        private IList<F_F_PNRZnach> indicators;
        private IList<F_F_NPACena> prices;
        private IList<F_F_LimitPrice> limits;
        private IList<F_F_NPARenderOrder> renderOrders;
        private IList<F_F_InfoProcedure> informingProcedures;

        public F_F_GosZadanie()
        {
            renderOrders = new List<F_F_NPARenderOrder>();
            limits = new List<F_F_LimitPrice>();
            prices = new List<F_F_NPACena>();
            indicators = new List<F_F_PNRZnach>();
            informingProcedures = new List<F_F_InfoProcedure>();
        }

        public virtual int SourceID { get; set; }
        
        public virtual int TaskID { get; set; }
        
        public virtual Decimal? CenaEd { get; set; }
        
        public virtual int RazdelN { get; set; }
        
        public virtual D_Services_VedPer RefVedPch { get; set; }
        
        public virtual F_F_ParameterDoc RefParametr { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_PNRZnach> Indicators
        {
            get { return indicators; }
            set { indicators = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefGZPr")]
        public virtual IList<F_F_NPACena> Prices
        {
            get { return prices; }
            set { prices = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_LimitPrice> Limits
        {
            get { return limits; }
            set { limits = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_NPARenderOrder> RenderOrders
        {
            get { return renderOrders; }
            set { renderOrders = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_InfoProcedure> InformingProcedures
        {
            get { return informingProcedures; }
            set { informingProcedures = value; }
        }
    }
}
