using System;

namespace Krista.FM.Domain
{
	public class D_Org_Category : ClassifierTable
	{
		public static readonly string Key = "9e187428-bc74-48cf-b7db-468cb4b0c586";

		public virtual int RowType { get; set; }
		public virtual int Code { get; set; }
		public virtual string Name { get; set; }

        /// <summary> Не имеет филиалов </summary>
        public const int FX_FX_NOTHING = 2;
        /// <summary> Головное учреждение </summary>
        public const int FX_FX_HEAD_OFFICE = 3;
        /// <summary> Филиал </summary>
        public const int FX_FX_BRANCH = 4;
        /// <summary> Обособленное структурное подразделение </summary>
        public const int FX_FX_SEPARATE_UNIT = 5;
        /// <summary> Представительство </summary>
        public const int FX_FX_AGENCY = 6;
	}
}
