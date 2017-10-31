using System;
using System.Runtime.Serialization;

using Krista.FM.Domain.Serialization.Json;

using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    [JsonObject]
    public class D_Form_TableCell : ClassifierTable
	{
		public static readonly string Key = "5154828f-29b7-4b4f-afe3-832358b5234b";

        [IgnoreDataMember]
        public virtual int RowType { get; set; }
		public virtual bool Required { get; set; }
		public virtual bool ReadOnly { get; set; }
        [JsonIgnore]
        public virtual string InputMask { get; set; }
        [JsonIgnore]
        public virtual string DisplayMask { get; set; }
        public virtual string DefaultValue { get; set; }
        [JsonIgnore]
        public virtual string DereferenceFields { get; set; }
        [JsonIgnore]
        public virtual string SearchFields { get; set; }
        [JsonIgnore]
        public virtual string Filter { get; set; }
        
        [JsonIgnore]
        public virtual D_Form_Part RefSection { get; set; }

        [JsonProperty(PropertyName = "RefColumn")]
        [JsonConverter(typeof(ReferenceIdJsonConverter<D_Form_TableColumn>))]
        public virtual D_Form_TableColumn RefColumn { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(ReferenceIdJsonConverter<D_Form_TableRow>))]
        public virtual D_Form_TableRow RefRow { get; set; }
	}
}
