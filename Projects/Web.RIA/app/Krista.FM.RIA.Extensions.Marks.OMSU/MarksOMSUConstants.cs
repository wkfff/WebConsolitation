namespace Krista.FM.RIA.Extensions.MarksOMSU
{
    /// <summary>
    /// Виды территорий
    /// </summary>
    public enum TerrytoryType : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Муниципальный район
        /// </summary>
        MR = 4,

        /// <summary>
        /// Городской округ
        /// </summary>
        GO = 7
    }

    /// <summary>
    /// Статусы данных ОМСУ
    /// </summary>
    public enum OMSUStatus : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// На редактировании
        /// </summary>
        OnEdit = 1,

        /// <summary>
        /// На рассмотрении
        /// </summary>
        OnReview = 2,

        /// <summary>
        /// Данные утверждены
        /// </summary>
        Approved = 3,

        /// <summary>
        /// Данные приняты
        /// </summary>
        Accepted = 4
    }

    /// <summary>
    /// Типы Показателей
    /// </summary>
    public enum TypeMark : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Тип - Сбор
        /// </summary>
        Gather = 1,

        /// <summary>
        /// Целевое значение
        /// </summary>
        TargetValue = 2,

        /// <summary>
        /// Тип - Норматив
        /// </summary>
        Normative = 3,

        /// <summary>
        /// Тип - Расчетный
        /// </summary>
        Calculated = 4
    }

    public class MarksOMSUConstants
    {
        /// <summary>
        /// Роль для просмотра неэффективных затрат ЖКХ
        /// </summary>
        public const string IneffGkhWatchRole = "Оценка ОМСУ_Просмотр оценки";

        /// <summary>
        /// Роль для пересчета неэффективных затрат ЖКХ
        /// </summary>
        public const string IneffGkhCalculateRole = "Оценка ОМСУ_Расчет оценки";
    }
}
