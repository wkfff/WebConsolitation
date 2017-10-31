using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;

using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    [DataContract]
    [JsonObject]
    public class D_Form_Part : ClassifierTable
	{
		public static readonly string Key = "fd0aaaf2-1494-46eb-b74b-89793197bca7";

        private IList<D_Form_Requisites> requisites;

        private IList<D_Form_TableColumn> columns;

        private IList<D_Form_TableRow> rows;

        private IList<D_Form_TableCell> cells;

        private IList<D_Form_Relations> relations;

        public D_Form_Part()
        {
            columns = new List<D_Form_TableColumn>();
            rows = new List<D_Form_TableRow>();
            cells = new List<D_Form_TableCell>();
            requisites = new List<D_Form_Requisites>();
            relations = new List<D_Form_Relations>();
        }

        [IgnoreDataMember]
        [JsonIgnore]
        public virtual int RowType { get; set; }
        
        [DataMember(Order = 1)]
        public virtual string Code { get; set; }

        [DataMember(Order = 2)]
        public virtual string Name { get; set; }
        
        [DataMember(Order = 3)]
        public virtual string InternalName { get; set; }

        [DataMember(Order = 4)]
        public virtual int Ord { get; set; }

        [DataMember(Order = 5)]
        [ReferenceField("RefPart")]
        public virtual IList<D_Form_Requisites> Requisites
        {
            get { return requisites; }
            set { requisites = value; }
        }

        [DataMember(Order = 6)]
        [ReferenceField("RefPart")]
        public virtual IList<D_Form_TableColumn> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        [DataMember(Order = 7)]
        [ReferenceField("RefPart")]
        public virtual IList<D_Form_TableRow> Rows
        {
            get { return rows; }
            set { rows = value; }
        }

        [DataMember(Order = 8)]
        [ReferenceField("RefSection")]
        public virtual IList<D_Form_TableCell> Cells
        {
            get { return cells; }
            set { cells = value; }
        }

        [DataMember(Order = 9)]
        [JsonIgnore]
        [ReferenceField("RefPart")]
        public virtual IList<D_Form_Relations> Relations
        {
            get { return relations; }
            set { relations = value; }
        }

        [DataMember(Order = 10)]
        [JsonIgnore]
        public virtual D_CD_Templates RefForm { get; set; }
    }
}
