namespace Krista.FM.Domain
{
    public class FX_Org_TipYch : ClassifierTable
    {
        /// <summary>
        /// Бюджетное учреждение
        /// </summary>
        public const int BudgetaryID = 3;

        /// <summary>
        /// Казенное учреждение
        /// </summary>
        public const int GovernmentID = 8;

        /// <summary>
        /// Автономное учреждение
        /// </summary>
        public const int AutonomousID = 10;
        
        public static readonly string Key = "64301354-e69b-4d5b-a8d4-92c79310e39b";

        public virtual int RowType { get; set; }
        
        public virtual string Name { get; set; }
    }
}
