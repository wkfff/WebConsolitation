using System;
using System.Web;

namespace Krista.FM.RIA.Core.Progress
{
    /// <summary>
    /// Предоставляет серверное API для индикации процесса выполнения задач.
    /// </summary>
    public class ProgressManager : IProgressManager
    {
        /// <summary>
        /// Заголовок Http-запроса содержащий уникальный идентификатор задачи.
        /// </summary>
        public const string HeaderNameTaskId = "X-Progress-TaskId";

        private static readonly ProgressState InvalidProgressState = new ProgressState { Percentage = 0, Text = "..." };

        private readonly IProgressDataProvider dataProvider;

        public ProgressManager()
            : this(new AspnetProgressProvider())
        {
        }

        public ProgressManager(IProgressDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        /// <summary>
        /// Устанавливает процент выполнения задачи.
        /// </summary>
        /// <param name="percentage">Процент выполнения задачи.</param>
        /// <param name="taskId">ID задачи.</param>
        public void SetCompleted(double percentage, string taskId = null)
        {
            ////System.Threading.Thread.Sleep(1000 * 10);
            if (taskId == null)
            {
                taskId = GetTaskId();
            }

            if (String.IsNullOrEmpty(taskId))
            {
                return;
            }

            percentage = (percentage < 0) ? 0 : percentage;
            percentage = (percentage > 1) ? 1 : percentage;

            var ps = dataProvider.Get(taskId) ?? new ProgressState();
            ps.Percentage = Convert.ToInt32(Math.Round(percentage * 100));
            dataProvider.Set(taskId, ps);
        }

        /// <summary>
        /// Устанавливает описание выполняемого действия.
        /// </summary>
        /// <param name="step">Описание состояния задачи.</param>
        /// <param name="taskId">ID задачи.</param>
        public void SetCompleted(string step, string taskId = null)
        {
            if (taskId == null)
            {
                taskId = GetTaskId();
            }

            if (String.IsNullOrEmpty(taskId))
            {
                return;
            }

            var ps = dataProvider.Get(taskId) ?? new ProgressState();
            ps.Text = step;
            dataProvider.Set(taskId, ps);
        }

        public void SetCompleted(string step, double percentage, string taskId = null)
        {
            if (taskId == null)
            {
                taskId = GetTaskId();
            }

            if (String.IsNullOrEmpty(taskId))
            {
                return;
            }

            percentage = (percentage < 0) ? 0 : percentage;
            percentage = (percentage > 1) ? 1 : percentage;

            var ps = dataProvider.Get(taskId) ?? new ProgressState();
            ps.Percentage = Convert.ToInt32(Math.Round(percentage * 100));
            ps.Text = step;
            dataProvider.Set(taskId, ps);
        }

        /// <summary>
        /// Возвращает текущий статус задачи.
        /// </summary>
        /// <param name="taskId">ID задачи статус которой необходимо получить.</param>
        /// <returns>Текущий статус задачи.</returns>
        public ProgressState GetStatus(string taskId)
        {
            if (String.IsNullOrEmpty(taskId))
            {
                return InvalidProgressState;
            }

            var buffer = dataProvider.Get(taskId);
            return buffer ?? InvalidProgressState;
        }

        /// <summary>
        /// Извлекает ID текущей задачи из Http-заголовка запроса.
        /// </summary>
        /// <returns>ID задачи.</returns>
        private string GetTaskId()
        {
            var id = HttpContext.Current.Request.Headers[HeaderNameTaskId];
            id = id ?? HttpContext.Current.Request.Form[HeaderNameTaskId];
            return id ?? String.Empty;
        }
    }
}
