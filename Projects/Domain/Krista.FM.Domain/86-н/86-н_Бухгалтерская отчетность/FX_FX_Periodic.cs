namespace Krista.FM.Domain
{
    public class FX_FX_Periodic : ClassifierTable
    {
        /// <summary>
        /// Годовая периодичность
        /// </summary>
        public const int AnualID = 1;

        public static readonly string Key = "7a7ba65d-8545-4307-ba29-2ad4e9c17031";

        public virtual int RowType { get; set; }
        
        public virtual string Name { get; set; }
        
        public virtual string NameEng { get; set; }
    }
}
