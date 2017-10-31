namespace Krista.FM.Domain
{
	public class F_F_RequestAccount : FactTable
	{
		public static readonly string Key = "7b2c5dcf-27fc-45f7-9be0-43c30e8c7717";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string DeliveryTerm { get; set; }
		public virtual string OtherRequest { get; set; }
		public virtual string OtherInfo { get; set; }
		public virtual F_F_ParameterDoc RefFactGZ { get; set; }
		public virtual string ReportForm { get; set; }
	}
}
