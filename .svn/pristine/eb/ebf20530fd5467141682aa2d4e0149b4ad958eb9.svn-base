using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    public class F_F_GosZadanie2016 : FactTable
    {
        public static readonly string Key = "4e6cc012-7861-43e0-a479-6104d2318702";

        private IList<F_F_GZYslPotr2016> customersCategory;
        private IList<F_F_NPARenderOrder2016> renderOrders;
        private IList<F_F_InfoProcedure2016> informingProcedures;
        private IList<F_F_NPACena2016> prices;
        private IList<F_F_PNRZnach2016> indicators;
        
        public F_F_GosZadanie2016()
        {
            customersCategory = new List<F_F_GZYslPotr2016>();
            renderOrders = new List<F_F_NPARenderOrder2016>();
            informingProcedures = new List<F_F_InfoProcedure2016>();
            prices = new List<F_F_NPACena2016>();
            indicators = new List<F_F_PNRZnach2016>();
        }

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual decimal? AveragePrice { get; set; }

        public virtual int OrdinalNumber { get; set; }

        public virtual D_Services_Service RefService { get; set; }

        public virtual F_F_ParameterDoc RefParameter { get; set; }

        public virtual bool? IsOtherSources { get; set; }
        
        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_GZYslPotr2016> CustomersCategory
        {
            get { return customersCategory; }
            set { customersCategory = value; }
        }
        
        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_NPARenderOrder2016> RenderOrders
        {
            get { return renderOrders; }
            set { renderOrders = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_InfoProcedure2016> InformingProcedures
        {
            get { return informingProcedures; }
            set { informingProcedures = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_NPACena2016> Prices
        {
            get { return prices; }
            set { prices = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_PNRZnach2016> Indicators
        {
            get { return indicators; }
            set { indicators = value; }
        }
    }
}
