namespace Krista.FM.Domain
{
    public class T_F_IndicatorsPfhd : DomainObject
    {
        public static readonly string Key = "51cf3da8-c7b2-437b-8a40-edaca9911ced";

        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual int Detail { get; set; }
    }
}
