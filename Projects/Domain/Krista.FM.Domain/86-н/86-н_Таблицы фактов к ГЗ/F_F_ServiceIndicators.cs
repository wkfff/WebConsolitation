using System;

namespace Krista.FM.Domain
{
    public class F_F_ServiceIndicators : FactTable
    {
        public static readonly string Key = "6e79a412-8e27-486b-980e-7cec7a14fdfe";

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual D_Services_Service RefService { get; set; }

        public virtual FX_FX_CharacteristicType RefType { get; set; }

        public virtual D_Org_OKEI RefOKEI { get; set; }
    }
}
