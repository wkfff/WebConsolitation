using System;

namespace Krista.FM.Domain
{
    public class T_F_LicenseDetails : DomainObject
    {
        public static readonly string Key = "98737278-da9f-42d7-b1b6-c8cb86ce026f";

        public virtual string LicenseAgencyName { get; set; }
        
        public virtual string LicenseName { get; set; }

        public virtual string LicenseNum { get; set; }

        public virtual DateTime LicenseDate { get; set; }

        public virtual DateTime? LicenseExpDate { get; set; }

        public virtual F_F_ParameterDoc RefParameterDoc { get; set; }
    }
}
