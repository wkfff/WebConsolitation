using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    public class D_Services_Service : ClassifierTable
    {
        public static readonly string Key = "e7dcd5fc-e019-4f45-8a07-0631deecbb74";
        
        /// <summary>
        /// Статус Включена
        /// </summary>
        public static readonly string Included = "801";

        /// <summary>
        /// Статус Исключена
        /// </summary>
        public static readonly string Excluded = "866";

        private IList<F_F_ServiceConsumersCategory> consumers;
        private IList<F_F_ServiceOKVED> okveds;
        private IList<F_F_ServiceConsumersCategory> consumersCategoryes;

        public D_Services_Service()
        {
            consumers = new List<F_F_ServiceConsumersCategory>();
            okveds = new List<F_F_ServiceOKVED>();
            consumersCategoryes = new List<F_F_ServiceConsumersCategory>();
        }

        public virtual int RowType { get; set; }

        public virtual string Regrnumber { get; set; }

        public virtual DateTime EffectiveFrom { get; set; }

        public virtual DateTime? EffectiveBefore { get; set; }

        public virtual string InstCode { get; set; }

        public virtual string NameCode { get; set; }

        public virtual string NameName { get; set; }

        public virtual string SvcCnts1CodeVal { get; set; }

        public virtual string SvcCnts2CodeVal { get; set; }

        public virtual string SvcCnts3CodeVal { get; set; }

        public virtual string SvcCntsName1Val { get; set; }

        public virtual string SvcCntsName2Val { get; set; }

        public virtual string SvcCntsName3Val { get; set; }

        public virtual string SvcTerms1CodeVal { get; set; }

        public virtual string SvcTerms2CodeVal { get; set; }

        public virtual string SvcTermsName1Val { get; set; }

        public virtual string SvcTermsName2Val { get; set; }

        public virtual string GUID { get; set; }

        public virtual bool? IsActual { get; set; }

        public virtual FX_FX_ServiceType RefType { get; set; }

        public virtual FX_FX_ServicePayType2 RefPay { get; set; }

        public virtual D_OKTMO_OKTMO RefOKTMO { get; set; }

        public virtual D_Org_Structure RefYchr { get; set; }

        public virtual D_Services_ActivityType RefActivityType { get; set; }

        public virtual bool? IsEditable { get; set; }

        public virtual string BusinessStatus { get; set; }

        public virtual bool? FromPlaning { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefService")]
        public virtual IList<F_F_ServiceConsumersCategory> Customers
        {
            get { return consumers; }
            set { consumers = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefService")]
        public virtual IList<F_F_ServiceOKVED> Okveds
        {
            get { return okveds; }
            set { okveds = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefService")]
        public virtual IList<F_F_ServiceConsumersCategory> ConsumersCategoryes
        {
            get { return consumersCategoryes; }
            set { consumersCategoryes = value; }
        }
    }
}
