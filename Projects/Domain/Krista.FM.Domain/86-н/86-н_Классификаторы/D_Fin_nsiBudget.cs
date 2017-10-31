using System;
using Krista.FM.Domain.Serialization.Json;
using Newtonsoft.Json;

namespace Krista.FM.Domain
{
	public class D_Fin_nsiBudget : ClassifierTable
	{
		public static readonly string Key = "7f368554-cac7-4d1a-9158-4c0a12f79a6c";

		public virtual int RowType { get; set; }
		public virtual string Code { get; set; }
		public virtual string Name { get; set; }
		public virtual int? ParentID { get; set; }
        [JsonProperty]
        [JsonConverter(typeof(ReferenceIdJsonConverter<D_Org_PPO>))]
        public virtual D_Org_PPO RefOrgPPO { get; set; }
	}
}
