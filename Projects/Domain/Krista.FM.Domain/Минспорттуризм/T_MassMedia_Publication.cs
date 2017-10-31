using System;

namespace Krista.FM.Domain
{
	public class T_MassMedia_Publication : DomainObject
	{
		public static readonly string Key = "00cf9450-1e4b-4c2a-8f19-c3c6de9392b4";

		public virtual int? Circulation { get; set; }
		public virtual int? Timing { get; set; }
		public virtual int? IssueCount { get; set; }
		public virtual T_MassMedia_List MassMedia { get; set; }
		public virtual FX_Date_YearDayUNV YearDayUNV { get; set; }
		public virtual D_MassMedia_Period Period { get; set; }
	}
}
