using System;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class MrotFormController : SchemeBoundController
    {
        private readonly FormRepository repository;
        private readonly ITaskService taskService;
        private readonly ILinqRepository<D_Report_TrihedrAgr> reportRepository;
        private readonly ILinqRepository<D_Report_TAJobTitle> jobTitlesRepository;

        public MrotFormController(
            FormRepository repository,
            ITaskService taskService,
            ILinqRepository<D_Report_TrihedrAgr> reportRepository,
            ILinqRepository<D_Report_TAJobTitle> jobTitlesRepository)
        {
            this.repository = repository;
            this.taskService = taskService;
            this.reportRepository = reportRepository;
            this.jobTitlesRepository = jobTitlesRepository;
        }

        public ActionResult Load(int taskId)
        {
            var list = repository.GetFormData(taskId);

            return new AjaxStoreResult(list, list.Count);
        }

        [Transaction]
        public ActionResult Save(object data, int taskId)
        {
            var ss = JavaScriptDomainConverter<FormModel>
                .Deserialize(Convert.ToString(((string[])data)[0]));

            TaskViewModel task = taskService.GetTaskViewModel(taskId);
            if (task.RefStatus != (int)TaskViewModel.TaskStatus.Edit)
            {
                return new AjaxStoreResult(StoreResponseFormat.Save).Error("Изменять данные отчета запрещено.");
            }

            repository.Save(ss, taskId, task.Region);

            return new AjaxStoreResult(StoreResponseFormat.Save);
        }
    }
}
