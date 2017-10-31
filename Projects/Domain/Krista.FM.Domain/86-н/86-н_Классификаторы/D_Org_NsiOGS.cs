using System;

namespace Krista.FM.Domain
{
    public class D_Org_NsiOGS : ClassifierTable
    {
        public static readonly string Key = "c92bd15a-3cb3-4024-8619-2ecb2078a58e";

        /// <summary>
        /// Бизнес статус 801
        /// </summary>
        public static readonly string Included = "801";

        /// <summary>
        /// Бизнес статус 865
        /// </summary>
        public static readonly string Excluded = "865";

        public virtual int RowType { get; set; }
        
        public virtual string regNum { get; set; }
        
        public virtual string FullName { get; set; }
        
        public virtual string ShortName { get; set; }
        
        public virtual string inn { get; set; }

        public virtual string kpp { get; set; }

        public virtual string Stats { get; set; }

        public virtual DateTime? CloseDate { get; set; }

        public virtual DateTime? OpenDate { get; set; }

        public virtual DateTime? changeDate { get; set; }
    }
}
