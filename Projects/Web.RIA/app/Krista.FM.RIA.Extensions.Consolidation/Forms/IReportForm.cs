using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public interface IReportForm
    {
        /// <summary>
        /// Идентификатор формы.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Метод валидации формы.
        /// </summary>
        /// <param name="taskId">ID задачи связанной с отчетом.</param>
        /// <returns>true - ошибок нет, false - есть ошибки.</returns>
        bool Validate(int taskId);

        /// <summary>
        /// Создает пустой отчет для задачи.
        /// </summary>
        /// <param name="task">Задача связанная с отчетом.</param>
        void CreateReport(D_CD_Task task);

        /// <summary>
        /// Возвращает true, если для отчета есть процедура переноса данных в таблицы фактов.
        /// </summary>
        bool HasPampers(D_CD_Task task);

        /// <summary>
        /// Запускает процедуру переноса данных в таблицы фактов.
        /// </summary>
        /// <param name="task">Обрабатываемая задача.</param>
        /// <param name="actions">Действия для выполнения.</param>
        void Pump(D_CD_Task task, PamperActionsEnum actions);
    }
}
