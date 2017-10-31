namespace Krista.FM.Domain
{
    public class D_Services_Platnost : ClassifierTable
    {
        /// <summary>
        /// Бесплатная услуга\работа
        /// </summary>
        public const int Free = 1;

        /// <summary>
        /// Платная услуга\работа
        /// </summary>
        public const int Paid = 2;

        /// <summary>
        /// Частично платная услуга\работа
        /// </summary>
        public const int PartiallyPaid = 3;

        public static readonly string Key = "fa47caf7-1220-46dd-b0c3-8b1a1d66a1ef";

        public virtual int RowType { get; set; }
        
        public virtual int Code { get; set; }

        public virtual string Name { get; set; }
    }
}
