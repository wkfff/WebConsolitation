using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Krista.FM.Domain.MappingAttributes;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
	public class F_Org_Passport : FactTable
	{
		public static readonly string Key = "0414c57c-93bf-493b-8f64-c245203ab7cd";
	    private IList<F_F_Filial> _branches;
	    private IList<F_F_OKVEDY> _activity;
	    private IList<F_F_Founder> _founders;

	    public F_Org_Passport()
        {
            _branches = new List<F_F_Filial>();
            _activity = new List<F_F_OKVEDY>();
            _founders = new List<F_F_Founder>();
        }

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string OGRN { get; set; }
		public virtual string Fam { get; set; }
		public virtual string NameRuc { get; set; }
		public virtual string Otch { get; set; }
		public virtual string Ordinary { get; set; }
		public virtual string Adr { get; set; }
		public virtual string Indeks { get; set; }
		public virtual string Website { get; set; }
		public virtual string OKPO { get; set; }
		public virtual string Phone { get; set; }
		public virtual string Mail { get; set; }
		public virtual D_OKOPF_OKOPF RefOKOPF { get; set; }
		public virtual D_OKATO_OKATO RefOKATO { get; set; }
		public virtual D_Org_Category RefCateg { get; set; }
		public virtual D_OKTMO_OKTMO RefOKTMO { get; set; }
		public virtual D_Org_VidOrg RefVid { get; set; }
		public virtual D_OKFS_OKFS RefOKFS { get; set; }
		public virtual D_OrgGen_Raspor RefRaspor { get; set; }
		public virtual F_F_ParameterDoc RefParametr { get; set; }
		
        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefPassport")]
        public virtual IList<F_F_Filial> Branches
        {
            get { return _branches; }
            set { _branches = value; }
        }

	    [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefPassport")]
        public virtual IList<F_F_OKVEDY> Activity
	    {
	        get { return _activity; }
	        set { _activity = value; }
	    }

	    [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefPassport")]
        public virtual IList<F_F_Founder> Founders
	    {
	        get { return _founders; }
	        set { _founders = value; }
	    }
	}
}
