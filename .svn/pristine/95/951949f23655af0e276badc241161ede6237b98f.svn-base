using System;

namespace Krista.FM.RIA.Extensions.Consolidation.Presentation.ViewModel
{
    public class CollectingTaskViewModel
    {
        /// <summary>
        /// Первичный ключ.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата сбора.
        /// </summary>
        /// <remarks>
        /// Обязательная дата, по состоянию на которую собирается отчетность в рамках задачи. 
        /// Фактически отражает конец интервала данных, входящих в отчетность.
        /// </remarks>
        public DateTime Date { get; set; }

        /// <summary>
        /// Начало сбора.
        /// </summary>
        /// <remarks>
        /// Дата, в которую должна быть инициирована задача сбора отчетности. 
        /// От этой даты отсчитываются относительные сроки предоставления отчетов в рамках задачи сбора отчетности, 
        /// на основании трудоёмкости, указанной в регламенте.
        /// </remarks>
        public DateTime ProvideDate { get; set; }

        /// <summary>
        /// За период.
        /// </summary>
        /// <remarks>
        /// Ссылка на период отчетности, за который осуществляется сбор данных в рамках задачи сбора.
        /// </remarks>
        public int PeriodId { get; set; }

        public string PeriodName { get; set; }

        /// <summary>
        /// Автор (Пользователь отчетности).
        /// </summary>
        /// <remarks>
        /// Субъект отчетности, инициировавший задачу сбора отчетности, который может отклонять задачу. 
        /// Автор является владельцем задачи сбора.
        /// </remarks>
        public int AuthorId { get; set; }

        public string AuthorName { get; set; }
    }
}
