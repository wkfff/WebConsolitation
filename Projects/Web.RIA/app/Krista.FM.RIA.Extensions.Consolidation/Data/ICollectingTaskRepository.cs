using System;
using System.Collections.Generic;

using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Data
{
    public interface ICollectingTaskRepository
    {
        /// <summary>
        /// Возвращает задачи созданные указанным автором.
        /// </summary>
        /// <param name="subjects">Список субъектов в которых олицетворен текущий оператор.</param>
        ICollection<D_CD_CollectTask> GetSubjectTasks(IEnumerable<D_CD_Subjects> subjects);

        /// <summary>
        /// Возвращает все отчеты созданные в рамках задачи сбора отчетности.
        /// </summary>
        /// <param name="task">Задача сбора отчетности.</param>
        ICollection<D_CD_Report> GetReports(D_CD_CollectTask task);

        /// <summary>
        /// Создает новую задачу сбора.
        /// </summary>
        /// <param name="date">Дата конца периода сбора.</param>
        /// <param name="provideDate">Дата предоставления отчетности.</param>
        /// <param name="periodId">Период отчетности.</param>
        /// <param name="subjectId">Пользователь отчетности, который инициировал задачу.</param>
        D_CD_CollectTask Create(DateTime date, DateTime provideDate, int periodId, int subjectId);
    }
}