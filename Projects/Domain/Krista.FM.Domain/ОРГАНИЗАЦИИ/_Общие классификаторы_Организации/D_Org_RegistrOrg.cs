using System;

namespace Krista.FM.Domain
{
	public class D_Org_RegistrOrg : ClassifierTable
	{
		public static readonly string Key = "c924d846-afdc-4f08-8e64-c572eec75405";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string NameOrg { get; set; }
		public virtual string ShName { get; set; }
		public virtual string LegalAddress { get; set; }
		public virtual string FactAddress { get; set; }
		public virtual string Phone { get; set; }
		public virtual string Fax { get; set; }
		public virtual string Email { get; set; }
		public virtual string ShareHold { get; set; }
		public virtual string NumRegistr { get; set; }
		public virtual DateTime? DatRegistr { get; set; }
		public virtual string PlaceRegitr { get; set; }
		public virtual string TypeAction { get; set; }
		public virtual string INN { get; set; }
		public virtual string KPP { get; set; }
		public virtual string Fond { get; set; }
		public virtual string State { get; set; }
		public virtual string Note { get; set; }
		public virtual string Branch { get; set; }
		public virtual string Submission { get; set; }
		public virtual string OKPO { get; set; }
		public virtual string OGRN { get; set; }
		public virtual string DateN { get; set; }
		public virtual string CodeOKATO { get; set; }
		public virtual string Site { get; set; }
		public virtual string Contact { get; set; }
		public virtual string Login { get; set; }
		public virtual bool? SignCN { get; set; }
		public virtual D_OK_OKOPF RefOK { get; set; }
		public virtual D_OK_OKFS RefOKOKFS { get; set; }
		public virtual D_Org_TypeOrg RefOrg { get; set; }
		public virtual D_Regions_Analysis RefRegionAn { get; set; }
		public virtual B_Org_OrgBridge RefOrgBridge { get; set; }
		public virtual FX_Org_ProdGroup RefProdGroup { get; set; }
	}
}
