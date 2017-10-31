using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;

using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    [JsonObject]
    public class D_CD_Templates : ClassifierTable
    {
        public static readonly string Key = "f27b352b-d3e5-441c-bc69-1fbce8c52e6f";

        private IList<D_Form_Requisites> requisites;

        private IList<D_Form_Part> parts;

        public D_CD_Templates()
        {
            requisites = new List<D_Form_Requisites>();
            parts = new List<D_Form_Part>();
        }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual int RowType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Class { get; set; }
        public virtual string ShortName { get; set; }
        public virtual string NameCD { get; set; }
        public virtual string InternalName { get; set; }
        public virtual string Code { get; set; }
        public virtual int FormVersion { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public virtual int Status { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public virtual byte[] TemplateFile { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public virtual byte[] TemplateMarkup { get; set; }

        [ReferenceField("RefForm")]
        public virtual IList<D_Form_Requisites> Requisites
        {
            get { return requisites; }
            set { requisites = value; }
        }

        [ReferenceField("RefForm")]
        public virtual IList<D_Form_Part> Parts
        {
            get { return parts; }
            set { parts = value; }
        }
    }
}
