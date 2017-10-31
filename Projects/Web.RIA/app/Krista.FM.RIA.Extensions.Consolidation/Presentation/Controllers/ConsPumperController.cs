using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers;

namespace Krista.FM.RIA.Extensions.Consolidation.Presentation.Controllers
{
    public class ConsPumperController : SchemeBoundController
    {
        private readonly ILinqRepository<D_CD_Task> taskRepository; 
        private readonly IProgressManager progressManager;

        public ConsPumperController(IProgressManager progressManager, ILinqRepository<D_CD_Task> taskRepository)
        {
            this.progressManager = progressManager;
            this.taskRepository = taskRepository;
        }

        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult PumpReport(int taskId)
        {
            var task = taskRepository.FindOne(taskId);
            if (task == null)
            {
                throw new InvalidOperationException("Не найдена задача");
            }

            var reportForm = Resolver.GetAll<IReportForm>().Where(x => x.ID == task.RefTemplate.Class).FirstOrDefault();
            if (reportForm == null)
            {
                throw new InvalidOperationException("Не найдена реализация формы с классом \"{0}\".".FormatWith(task.RefTemplate.Class));
            }

            try
            {
                reportForm.Pump(task, PamperActionsEnum.Pump);
            }
            catch (PumperPreconditionException e)
            {
                ViewData.ModelState.AddModelError("error", e);
                var result = new AjaxResult { ErrorMessage = e.Message };
                result.ExtraParamsResponse.Add(new Parameter("StackTrace", e.ExpandException()));
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("error", e);
                var result = new AjaxResult { ErrorMessage = "Критическая ошибка: " + e.Message };
                result.ExtraParamsResponse.Add(new Parameter("StackTrace", e.ExpandException()));
                return result;
            }

            return new AjaxResult();
        }

        public ActionResult PumpCollectTask(int taskId)
        {
            progressManager.SetCompleted("Подготовка...", 0);

            // Получаем все утвержденные задачи
            var tasks = taskRepository.FindAll()
                .Where(x => x.RefCollectTask.ID == taskId && x.RefStatus.ID == (int)TaskViewModel.TaskStatus.Accepted);

            int reportCount = tasks.Count();
            var reportIndx = 0.0;
            var sb = new StringBuilder();
            foreach (var task in tasks)
            {
                var reportForm = Resolver.GetAll<IReportForm>().FirstOrDefault(x => x.ID == task.RefTemplate.Class);
                if (reportForm == null)
                {
                    throw new InvalidOperationException("Не найдена реализация формы с классом \"{0}\".".FormatWith(task.RefTemplate.Class));
                }

                progressManager.SetCompleted("Обработка формы {0}".FormatWith(task.RefTemplate.InternalName));
                
                // TODO: begin transaction
                try
                {
                    reportForm.Pump(task, PamperActionsEnum.Pump);

                    progressManager.SetCompleted(reportIndx++ / reportCount);
                    sb.AppendFormat("Отчет обработан: \"{0} - {1}\".<br/><br/>", task.RefTemplate.Name, task.RefSubject.Name);
                }
                catch (PumperPreconditionException e)
                {
                    sb.AppendFormat(
                        "Отчет \"{0} - {1}\" обработан с ошибкой: {2}.",
                        task.RefTemplate.Name, 
                        task.RefSubject.Name,
                        e.ExpandException());
                    sb.Append("<br/><br/>");
                }
                catch (Exception e)
                {
                    sb.AppendFormat(
                        "Отчет \"{0} - {1}\" обработан с критической ошибкой: {2}.",
                        task.RefTemplate.Name,
                        task.RefSubject.Name,
                        e.ExpandException());
                    sb.Append("<br/><br/>");
                }
            }

            return new AjaxResult { Result = sb.ToString() };
        }
    }
}
