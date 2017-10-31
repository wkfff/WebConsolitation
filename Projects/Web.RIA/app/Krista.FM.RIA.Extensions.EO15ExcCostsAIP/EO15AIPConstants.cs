namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP
{
    /// <summary>
    /// Состояния объектов строительства
    /// </summary>
    public enum AIPStatus
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Строящийся объект
        /// </summary>
        UnderConstruction = 1,

        /// <summary>
        /// Введен в эксплуатацию
        /// </summary>
        Operated = 2,

        /// <summary>
        /// Планируемый к строительству
        /// </summary>
        PlannedForConstruction = 3,

        /// <summary>
        /// Заявлен на корректировку
        /// </summary>
        SentToCorrect = 4,

        /// <summary>
        /// В стадии незавершенного строительства
        /// </summary>
        UnderConstructionInProgress = 5
    }

    public enum AIPStatusD
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Данные На редактировании
        /// </summary>
        Edit = 1,

        /// <summary>
        /// Данные На рассмотрении
        /// </summary>
        Review = 2,

        /// <summary>
        /// Данные Утверждены
        /// </summary>
        Accepted = 3
    }

    /// <summary>
    /// Роли интерфейса
    /// </summary>
    internal static class AIPRoles
    {
        public const string MOClient = "ЭО15_Строительство МО";
        public const string GovClient = "ЭО15_Строительство Заказчики";
        public const string Coordinator = "ЭО15_Строительство Координаторы";
        public const string User = "ЭО15_Строительство Пользователи";
    }
}
