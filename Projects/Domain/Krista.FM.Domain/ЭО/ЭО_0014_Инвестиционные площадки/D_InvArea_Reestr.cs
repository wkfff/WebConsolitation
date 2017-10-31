using System;

namespace Krista.FM.Domain
{
	public class D_InvArea_Reestr : ClassifierTable
	{
		public static readonly string Key = "225f20db-b79f-4a92-87e4-5790c976cfe9";

		public virtual int RowType { get; set; }
		public virtual int SourceID { get; set; }
		public virtual string RegNumber { get; set; }
		public virtual string Location { get; set; }
		public virtual string CadNumber { get; set; }
		public virtual Decimal? Area { get; set; }
		public virtual string Category { get; set; }
		public virtual string Head { get; set; }
		public virtual string Contact { get; set; }
		public virtual string Phone { get; set; }
		public virtual string Email { get; set; }
		public virtual string PermittedUse { get; set; }
		public virtual string ActualUse { get; set; }
		public virtual string Documentation { get; set; }
		public virtual string Limitation { get; set; }
		public virtual string PermConstr { get; set; }
		public virtual string Relief { get; set; }
		public virtual string Road { get; set; }
		public virtual string Station { get; set; }
		public virtual string Pier { get; set; }
		public virtual string Airport { get; set; }
		public virtual string Plumbing { get; set; }
		public virtual string Sewage { get; set; }
		public virtual string Gas { get; set; }
		public virtual string Electricity { get; set; }
		public virtual string Heating { get; set; }
		public virtual string Landfill { get; set; }
		public virtual string Telephone { get; set; }
		public virtual string Connectivity { get; set; }
		public virtual string Fee { get; set; }
		public virtual string DistanceZones { get; set; }
		public virtual string Buildings { get; set; }
		public virtual string Resources { get; set; }
		public virtual string Settlement { get; set; }
		public virtual string ObjectEducation { get; set; }
		public virtual string ObjectHealth { get; set; }
		public virtual string ObjectSocSphere { get; set; }
		public virtual string ObjectServices { get; set; }
		public virtual string Hotels { get; set; }
		public virtual string Owner { get; set; }
		public virtual string Note { get; set; }
		public virtual DateTime? CreatedDate { get; set; }
		public virtual DateTime? AdoptionDate { get; set; }
		public virtual Decimal? CoordinatesLat { get; set; }
		public virtual Decimal? CoordinatesLng { get; set; }
		public virtual D_Territory_RF RefTerritory { get; set; }
		public virtual FX_InvArea_Status RefStatus { get; set; }
		public virtual string CreateUser { get; set; }
	}
}
