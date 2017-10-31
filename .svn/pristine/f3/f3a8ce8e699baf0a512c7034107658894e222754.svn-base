using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    public class D_Org_Structure : ClassifierTable
    {
        public static readonly string Key = "70be332f-acb1-438b-8cb5-d070fcfe4d83";
        private IList<F_F_ParameterDoc> documents;
        private IList<D_Org_UserProfile> profiles;
        private IList<F_Org_TypeHistory> typeHistories;

        public D_Org_Structure()
        {
            documents = new List<F_F_ParameterDoc>();
            profiles = new List<D_Org_UserProfile>();
            typeHistories = new List<F_Org_TypeHistory>();
        }

        public virtual int RowType { get; set; }

        public virtual string KPP { get; set; }

        public virtual string INN { get; set; }

        public virtual string ShortName { get; set; }

        public virtual string Name { get; set; }

        public virtual D_Org_PPO RefOrgPPO { get; set; }

        public virtual D_Org_GRBS RefOrgGRBS { get; set; }

        public virtual FX_Org_TipYch RefTipYc { get; set; }

        public virtual DateTime? OpenDate { get; set; }

        public virtual DateTime? CloseDate { get; set; }

        public virtual string Status { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefUchr")]
        public virtual IList<F_F_ParameterDoc> Documents
        {
            get { return documents; }
            set { documents = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefUchr")]
        public virtual IList<D_Org_UserProfile> Profiles
        {
            get { return profiles; }
            set { profiles = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefStructure")]
        public virtual IList<F_Org_TypeHistory> TypeHistories
        {
            get { return typeHistories; }
            set { typeHistories = value; }
        }
    }
}