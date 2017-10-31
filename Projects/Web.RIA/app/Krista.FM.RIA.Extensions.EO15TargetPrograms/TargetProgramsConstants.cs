namespace Krista.FM.RIA.Extensions.EO15TargetPrograms
{
    /// <summary>
    /// Статусы Целевых программ
    /// </summary>
    public enum ProgramStatus : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Не утверждена
        /// </summary>
        Unapproved = 1,

        /// <summary>
        /// Утверждена, но ещё не выполняется
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Выполняется, уже утверждена
        /// </summary>
        Running = 3,

        /// <summary>
        /// Завершена, утверждена
        /// </summary>
        Finished = 4
    }

    /// <summary>
    /// Типы Целевых программ
    /// </summary>
    public enum ProgramType : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Ведомственная целевая программа
        /// </summary>
        Department = 1,

        /// <summary>
        /// ДЦП - долгосрочная целевая программа
        /// </summary>
        Longterm = 2,

        /// <summary>
        /// Федеральная целевая программа
        /// </summary>
        Federal = 3,

        /// <summary>
        /// Областная целевая программа
        /// </summary>
        Regional = 4,

        /// <summary>
        /// Мунципальная целевая программа
        /// </summary>
        Municipal = 5
    }

    /// <summary>
    /// Стадии реализации проекта (Виды источников)
    /// </summary>
    public enum ProgramStage : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Стадия разработки концептуальных предложений
        /// </summary>
        Concept = 1,

        /// <summary>
        /// Стадия разработки проекта
        /// </summary>
        Design = 2,

        /// <summary>
        /// Стадия реализации
        /// </summary>
        Realization = 3
    }

    /// <summary>
    /// Роли интерфейса
    /// </summary>
    internal static class ProgramRoles
    {
        public const string Viewer = "ЭО15_ЦП Координаторы";
        public const string Creator = "ЭО15_ЦП Заказчики";
    }
}

