using System;
using System.Collections;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs
{
    public class Org3PriceAndTariffsController : SchemeBoundController
    {
        private IFactService factService;
        private ITaskService taskService;
        
        public Org3PriceAndTariffsController(IFactService factService, ITaskService taskService)
        {
            this.factService = factService;
            this.taskService = taskService;
        }

        public AjaxStoreExtraResult Load(int taskId, GoodType goodType)
        {
            bool isDirtyData = false;
            var list = factService.LoadFactData(taskId);

            // первый раз открыли?
            if (list.Count == 0)
            {
                list = factService.GetInitialData(taskId, goodType);
                isDirtyData = true;
            }

            return new AjaxStoreExtraResult(list, list.Count, isDirtyData ? "MarkLoadedDataAsDirty({0});".FormatWith(PricesAndTariffsView.DatasourceID) : string.Empty);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult Save(object data, int taskId)
        {
            try
            {
                CheckTaskStatus(taskId);

                var ss = JavaScriptDomainConverter<FormModel>.Deserialize(Convert.ToString(((string[])data)[0]));

                if (ss.Created != null)
                {
                    factService.CreateData(taskId, ss.Created);
                }

                if (ss.Updated != null)
                {
                    factService.UpdateData(taskId, ss.Updated);
                }

                return new AjaxStoreResult(StoreResponseFormat.Save);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new AjaxStoreResult(StoreResponseFormat.Save).Error(e.Message);
            }
        }

        [HttpPost]
        public AjaxStoreResult GetOrganizations(int taskId, string filter, GoodType goodType)
        {
            IList data = factService.GetOrganizations(taskId, filter, goodType);
            
            return new AjaxStoreResult(data, data.Count);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult IncludeOrganization(int taskId, int organizationId)
        {
            var result = new AjaxFormResult();
            try
            {
                CheckTaskStatus(taskId);

                factService.IncludeOrganization(taskId, organizationId);

                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Информация",
                    Html = "Организация добавлена.",
                    HideDelay = 2500
                });

                result.Script = message.ToScript();
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка при создании организации.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult CreateAndIncludeOrganization(int taskId, string orgName, bool orgIsMarketGrid, GoodType goodType)
        {
            var result = new AjaxFormResult();
            try
            {
                CheckTaskStatus(taskId);

                factService.CreateAndIncludeOrganization(taskId, orgName, orgIsMarketGrid, goodType);

                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Информация",
                    Html = "Организация создана.",
                    HideDelay = 2500
                });

                result.Script = message.ToScript();
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка при создании организации.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult ExcludeOrganization(int taskId, int organizationId)
        {
            var result = new AjaxFormResult();
            try
            {
                CheckTaskStatus(taskId);

                factService.ExcludeOrganization(taskId, organizationId);

                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Информация",
                    Html = "Организация исключена.",
                    HideDelay = 2500
                });

                result.Script = message.ToScript();
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка при удалении организации.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }

        [HttpPost]
        public AjaxStoreResult GetOldReportDates(int taskId)
        {
            IList data = factService.GetOldTaskDates(taskId);
            
            return new AjaxStoreResult(data, data.Count);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult CopyFromReport(int taskId, int sourceTaskId)
        {
            var result = new AjaxFormResult();
            try
            {
                CheckTaskStatus(taskId);

                factService.CopyFromTask(taskId, sourceTaskId);
                
                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Информация",
                    Html = "Данные скопированы.",
                    HideDelay = 2500
                });

                result.Script = message.ToScript();
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка при копировании данных с другого отчета.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }

        public ActionResult ExportToExcel(int taskId, GoodType goodType)
        {
            IExportService exportService = Resolver.Get<IExportService>("{0}ExportService".FormatWith(goodType));
            var stream = exportService.GetExcelReport(taskId);
            string fileName = "Отчет.xls";
            return File(stream, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// Проверка статуса задачи на возможность редактирования
        /// </summary>
        private void CheckTaskStatus(int taskId)
        {
            TaskViewModel task = taskService.GetTaskViewModel(taskId);
            if (task.RefStatus != (int)TaskViewModel.TaskStatus.Edit)
            {
                throw new Exception("Изменять данные отчета запрещено.");
            }
        }
    }
}
