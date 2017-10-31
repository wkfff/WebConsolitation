using System;

namespace Krista.FM.Domain
{
	public class T_CD_Files : DomainObject
	{
		public static readonly string Key = "e4469d1d-a973-4a73-bde2-c796a274a1bb";

		public virtual string FileName { get; set; }
		public virtual string FileDescription { get; set; }
		public virtual byte[] FileBody { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime? ChangeDate { get; set; }
        public virtual string ChangeUser { get; set; }
		public virtual D_CD_Task RefTask { get; set; }
	}
}
