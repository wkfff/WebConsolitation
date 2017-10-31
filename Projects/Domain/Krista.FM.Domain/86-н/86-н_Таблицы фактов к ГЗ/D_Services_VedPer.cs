using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    public class D_Services_VedPer : ClassifierTable
    {
        public static readonly string Key = "1b3bb61a-72ea-45a9-9c1b-859716d780bd";
        private IList<F_F_PotrYs> customers;
        private IList<F_F_PNRysl> values;

        public D_Services_VedPer()
        {
            customers = new List<F_F_PotrYs>();
            values = new List<F_F_PNRysl>();
        }

        public virtual int RowType { get; set; }

        public virtual string Code { get; set; }
        
        public virtual string Name { get; set; }
        
        public virtual DateTime? DataVkluch { get; set; }
        
        public virtual DateTime? DataIskluch { get; set; }
        
        public virtual string Note { get; set; }
        
        public virtual string NumberService { get; set; }
        
        public virtual string TypeLow { get; set; }
        
        public virtual string NameLow { get; set; }
        
        public virtual string NumberLaw { get; set; }
        
        public virtual DateTime? DateLaw { get; set; }
        
        public virtual string AuthorLaw { get; set; }
        
        public virtual int? ParentID { get; set; }
        
        public virtual D_Services_Platnost RefPl { get; set; }
        
        public virtual D_Org_GRBS RefGRBSs { get; set; }
        
        public virtual D_Services_TipY RefTipY { get; set; }
        
        public virtual D_Org_PPO RefOrgPPO { get; set; }
        
        public virtual D_Services_SferaD RefSferaD { get; set; }
        
        public virtual D_Org_OrgYchr RefYchred { get; set; }
        
        public virtual string BusinessStatus { get; set; }
        
        public virtual string Founder { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefVedPP")]
        public virtual IList<F_F_PotrYs> Customers
        {
            get { return customers; }
            set { customers = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefPerV")]
        public virtual IList<F_F_PNRysl> Values
        {
            get { return values; }
            set { values = value; }
        }
    }
}
