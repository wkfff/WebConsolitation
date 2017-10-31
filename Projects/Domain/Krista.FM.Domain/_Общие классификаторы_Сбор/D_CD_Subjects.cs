using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain
{
	public class D_CD_Subjects : ClassifierTable
	{
		public static readonly string Key = "c2d46b94-cf54-4392-b226-f4a377dd9d78";

        private ICollection<D_CD_Subjects> childs;

        public D_CD_Subjects()
        {
            childs = new Collection<D_CD_Subjects>();
        }

		public virtual int RowType { get; set; }
		public virtual string Name { get; set; }
		public virtual string ShortName { get; set; }
		public virtual string Note { get; set; }
		public virtual int UserId { get; set; }
		public virtual D_CD_Subjects ParentID { get; set; }
		public virtual D_CD_Roles RefRole { get; set; }
		public virtual D_Regions_Analysis RefRegion { get; set; }
		public virtual D_Organizations_Analysis RefOrganizations { get; set; }
		public virtual D_CD_Level RefLevel { get; set; }

        [ReferenceField("ParentID")]
	    public virtual ICollection<D_CD_Subjects> Childs
	    {
	        get { return childs; }
	        set { childs = value; }
	    }
	}
}
