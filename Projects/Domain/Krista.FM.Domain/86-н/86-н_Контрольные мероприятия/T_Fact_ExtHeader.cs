namespace Krista.FM.Domain
{
    public class T_Fact_ExtHeader : DomainObject
    {
        public static readonly string Key = "331c9e79-fa16-4e0f-aad5-bfd579728f87";

        public virtual bool NotInspectionActivity { get; set; }
        
        public virtual F_F_ParameterDoc RefParametr { get; set; }
    }
}
