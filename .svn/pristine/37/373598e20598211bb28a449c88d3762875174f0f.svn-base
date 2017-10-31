using System.Collections;
using System.Collections.Generic;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs
{
    public interface IFactService
    {
        /// <summary>
        /// Получаем данные из таблици фактов для конкретного отчёта
        /// </summary>
        IList<FormModel> LoadFactData(int taskId);

        /// <summary>
        /// Инициализация данных для конкретного отчёта
        /// </summary>
        IList<FormModel> GetInitialData(int taskId, GoodType goodType);

        /// <summary>
        /// Создание записей в таблице фактов
        /// </summary>
        void CreateData(int taskId, FormModel[] rows);

        /// <summary>
        /// Изменение записей в таблице фактов
        /// </summary>
        void UpdateData(int taskId, FormModel[] rows);

        /// <summary>
        /// Возвращает список зарегистрированных организаций, закрепленных за данным районом (определяется по задаче)
        /// </summary>
        IList GetOrganizations(int taskId, string filter, GoodType goodType);

        /// <summary>
        /// Добавляет в таблицу фактов все продукты для данной организации
        /// </summary>
        void IncludeOrganization(int taskId, int organizationId);

        /// <summary>
        /// Создает новую организацию и добавляет в таблицу фактов все продукты по ней
        /// </summary>
        void CreateAndIncludeOrganization(int taskId, string orgName, bool orgIsMarketGrid, GoodType goodType);

        /// <summary>
        /// Удаляет из таблицы фактов все подукты по данной организации
        /// </summary>
        void ExcludeOrganization(int taskId, int organizationId);

        /// <summary>
        /// Формирует список дат, соответствующих таким же отчетам за ранние периоды
        /// </summary>
        IList GetOldTaskDates(int taskId);

        /// <summary>
        /// Копирует все данные с другого отчета в данный. Существующие факты удаляются
        /// </summary>
        void CopyFromTask(int taskId, int sourceTaskId);
    }
}
