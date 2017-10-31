namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Models
{
    /// <summary>
    /// Типы проектов
    /// </summary>
    public enum InvProjPart : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Раздел 1 (Реализуемые приоритетные)
        /// </summary>
        Part1 = 1,

        /// <summary>
        /// Раздел 2 (Предлагаемые к реализации)
        /// </summary>
        Part2 = 2,
    }

    /// <summary>
    /// Виды статусов проекта
    /// </summary>
    public enum InvProjStatus : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// На редактировании
        /// </summary>
        Edit = 1,

        /// <summary>
        /// Исполняется в данный момент
        /// </summary>
        Executing = 2,

        /// <summary>
        /// Исключен в данный момент
        /// </summary>
        Excluded = 3
    }

    /// <summary>
    /// Типы показателей
    /// </summary>
    public enum InvProjInvestType : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Плановые объемы инвестиций
        /// </summary>
        Investment = 1,

        /// <summary>
        /// Плановые формы и объемы господдержки
        /// </summary>
        GosSupport = 2,

        /// <summary>
        /// Целевые показатели
        /// </summary>
        TargetRatings = 3
    }
}