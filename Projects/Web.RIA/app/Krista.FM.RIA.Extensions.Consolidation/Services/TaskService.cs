using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class TaskService : ITaskService
    {
        private readonly ILinqRepository<D_CD_Task> taskRepository;

        public TaskService(ILinqRepository<D_CD_Task> taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        public TaskViewModel GetTaskViewModel(int taskId)
        {
            var queryable = from t in taskRepository.FindAll()
                            where t.ID == taskId
                            select new TaskViewModel
                                       {
                                           ID = t.ID,
                                           BeginDate = t.BeginDate,
                                           EndDate = t.EndDate,
                                           Deadline = t.Deadline,
                                           TemplateName = t.RefTemplate.Name,
                                           TemplateShortName = t.RefTemplate.ShortName,
                                           TemplateClass = t.RefTemplate.Class,
                                           Status = t.RefStatus.Name,
                                           RefStatus = t.RefStatus.ID,
                                           SubjectId = t.RefSubject.ID,
                                           SubjectShortName = t.RefSubject.ShortName,
                                           ParentSubjectId = t.RefSubject.ParentID != null ? t.RefSubject.ParentID.ID : (int?)null,
                                           Region = t.RefSubject.RefRegion
                                       };
            TaskViewModel task = queryable.First();

            return task;
        }
    }
}
