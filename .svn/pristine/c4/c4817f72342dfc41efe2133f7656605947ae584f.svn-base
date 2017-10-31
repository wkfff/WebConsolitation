using System;
using System.Collections.Generic;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Consolidation.Data
{
    public class CollectingTaskRepository : ICollectingTaskRepository
    {
        private readonly ILinqRepository<D_CD_CollectTask> taskRepository;
        private readonly IRepository<D_CD_Period> periodRepository;
        private readonly IRepository<D_CD_Subjects> subjectRepository;
        private readonly ILinqRepository<D_CD_Report> reportRepository;

        public CollectingTaskRepository(
            ILinqRepository<D_CD_CollectTask> taskRepository,
            IRepository<D_CD_Period> periodRepository,
            IRepository<D_CD_Subjects> subjectRepository,
            ILinqRepository<D_CD_Report> reportRepository)
        {
            this.taskRepository = taskRepository;
            this.periodRepository = periodRepository;
            this.subjectRepository = subjectRepository;
            this.reportRepository = reportRepository;
        }

        /// <summary>
        /// Возвращает задачи созданные указанным автором.
        /// </summary>
        /// <param name="subjects">Список субъектов в которых олицетворен текущий оператор.</param>
        public ICollection<D_CD_CollectTask> GetSubjectTasks(IEnumerable<D_CD_Subjects> subjects)
        {
            return (from t in taskRepository.FindAll()
                    where subjects.Contains(t.RefSubject)
                    select t).ToList();
        }

        /// <summary>
        /// Возвращает все отчеты созданные в рамках задачи сбора отчетности.
        /// </summary>
        /// <param name="task">Задача сбора отчетности.</param>
        public ICollection<D_CD_Report> GetReports(D_CD_CollectTask task)
        {
            return reportRepository.FindAll().Where(x => x.RefTask.RefCollectTask == task).ToList();
        }

        public D_CD_CollectTask Create(DateTime date, DateTime provideDate, int periodId, int subjectId)
        {
            var task = new D_CD_CollectTask
            {
                EndPeriod = date,
                ProvideDate = provideDate,
                RefPeriod = periodRepository.Get(periodId),
                RefSubject = subjectRepository.Get(subjectId)
            };

            taskRepository.Save(task);

            return task;
        }
    }
}
