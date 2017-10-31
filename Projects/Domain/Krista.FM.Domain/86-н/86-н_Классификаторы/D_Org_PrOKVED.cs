namespace Krista.FM.Domain
{
    public class D_Org_PrOKVED : ClassifierTable
    {
        public const int MainID = 1;

        public const int OtherID = 2;

        public static readonly string Key = "7d64bb9a-3a38-4823-b7db-41e31c4157a7";

        public virtual int RowType { get; set; }

        public virtual int Code { get; set; }

        public virtual string Name { get; set; }
    }
}
