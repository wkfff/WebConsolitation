using System;

namespace Krista.FM.Domain
{
	public class F_F_RequestAccount2016 : FactTable
	{
		public static readonly string Key = "4655fe67-4a30-4401-8fd9-f335284cca98";

		public virtual int SourceID { get; set; }
		public virtual int TaskID { get; set; }
		public virtual string DeliveryTerm { get; set; }
		public virtual string OtherRequest { get; set; }
		public virtual string ReportForm { get; set; }
		public virtual string PeriodicityTerm { get; set; }
		public virtual string OtherIndicators { get; set; }
		public virtual F_F_ParameterDoc RefFactGZ { get; set; }
	}
}
