using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    [JsonObject]
	public class D_Form_Requisites : ClassifierTable
	{
		public static readonly string Key = "5cdcdc06-0efd-4e4d-80bf-f043ed826eff";

        [DataMember]
        [JsonIgnore]
        public virtual int RowType { get; set; }
        [DataMember]
        public virtual bool IsHeader { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Code { get; set; }
        [DataMember]
        public virtual int Ord { get; set; }
        [DataMember]
        public virtual bool Required { get; set; }
        [DataMember]
        public virtual bool ReadOnly { get; set; }
        [DataMember]
        public virtual string DefaultValue { get; set; }
        [DataMember]
        public virtual string DataType { get; set; }
        [DataMember]
        public virtual int? DataTypeSize { get; set; }
        [DataMember]
        public virtual int? DataTypeScale { get; set; }
        [DataMember]
        [JsonIgnore]
        public virtual string DereferenceFields { get; set; }
        [DataMember]
        [JsonIgnore]
        public virtual string SearchFields { get; set; }
        [DataMember]
        [JsonIgnore]
        public virtual string Filter { get; set; }
        [DataMember]
        [JsonIgnore]
        public virtual string InputMask { get; set; }
        [DataMember]
        [JsonIgnore]
        public virtual string DisplayMask { get; set; }
        [DataMember]
        public virtual string InternalName { get; set; }
        [JsonIgnore]
        public virtual D_CD_Templates RefForm { get; set; }
        [JsonIgnore]
        public virtual D_Form_Part RefPart { get; set; }
	}
}
