using System;

namespace Krista.FM.Domain
{
	public class T_People_Population : DomainObject
	{
		public static readonly string Key = "00d82809-882a-402b-b3a6-1328c28ae473";

		public virtual int Val { get; set; }
		public virtual D_Territory_RF Territory { get; set; }
		public virtual FX_Date_YearDayUNV YearDayUNV { get; set; }
	}
}
