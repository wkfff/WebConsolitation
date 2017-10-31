using System.ComponentModel;

namespace Krista.FM.Common.Constants
{
    /// <summary>
    /// Виды территорий
    /// </summary>
    public enum TerritoryType : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Российская федерация
        /// </summary>
        [Description("Российская федерация")]
        RF = 1,

        /// <summary>
        /// Федеральный округ
        /// </summary>
        [Description("Федеральный округ")]
        FO = 2,

        /// <summary>
        /// Субъект РФ
        /// </summary>
        [Description("Субъект РФ")]
        SB = 3,

        /// <summary>
        /// Муниципальный район
        /// </summary>
        [Description("Муниципальный район")]
        MR = 4,

        /// <summary>
        /// Городское поселение
        /// </summary>
        [Description("Городское поселение")]
        GP = 5,
        
        /// <summary>
        /// Сельское поселение
        /// </summary>
        [Description("Сельское поселение")]
        SP = 6,

        /// <summary>
        /// Городской округ
        /// </summary>
        [Description("Городской округ")]
        GO = 7,

        /// <summary>
        /// Внутригородская территория города федерального значения
        /// </summary>
        [Description("Внутригородская территория города федерального значения")]
        VGTFZ = 8,

        /// <summary>
        /// Межселенные территории
        /// </summary>
        [Description("Межселенные территории")]
        MejST = 9,

        /// <summary>
        /// Районный центр
        /// </summary>
        [Description("Районный центр")]
        RC = 10,

        /// <summary>
        /// Поселение
        /// </summary>
        [Description("Поселение")]
        Pos = 11
    }
}