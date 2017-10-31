using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Models;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsTaskController : SchemeBoundController
    {
        private readonly ILinqRepository<D_CD_Task> taskRepository;
        private readonly ILinqRepository<D_CD_Protocol> taskProtocolRepository;
        private readonly ILinqRepository<D_CD_ProtocolDetail> taskProtocolDetailRepository;
        private readonly ILinqRepository<T_CD_Files> taskFilesRepository;
        private readonly IRepository<FX_FX_FormStatus> statusRepository;
        private readonly ITaskService taskService;
        private readonly IUserSessionState sessionState;

        public ConsTaskController(
            ILinqRepository<D_CD_Task> taskRepository,
            ILinqRepository<D_CD_Protocol> taskProtocolRepository,
            ILinqRepository<D_CD_ProtocolDetail> taskProtocolDetailRepository,
            ILinqRepository<T_CD_Files> taskFilesRepository,
            IRepository<FX_FX_FormStatus> statusRepository,
            ITaskService taskService,
            IUserSessionState sessionState)
        {
            this.taskRepository = taskRepository;
            this.taskProtocolRepository = taskProtocolRepository;
            this.taskProtocolDetailRepository = taskProtocolDetailRepository;
            this.taskFilesRepository = taskFilesRepository;
            this.statusRepository = statusRepository;
            this.taskService = taskService;
            this.sessionState = sessionState;
        }

        public ActionResult GetProtocolTable(int taskId)
        {
            var data = new List<TaskProtocolViewModel>(
                       from p in taskProtocolRepository.FindAll()
                       where p.RefTask.ID == taskId
                       orderby p.ChangeDate descending
                       select new TaskProtocolViewModel
                              {
                                  ID = p.ID,
                                  Commentary = p.Commentary,
                                  ChangeDate = p.ChangeDate,
                                  ChangeUser = p.ChangeUser
                              });

            foreach (TaskProtocolViewModel taskProtocol in data)
            {
                var protocolDetails = new List<TaskProtocolDetailViewModel>(
                                      from d in taskProtocolDetailRepository.FindAll()
                                      where d.RefProtocol.ID == taskProtocol.ID
                                      select new TaskProtocolDetailViewModel()
                                             {
                                                 ID = d.ID,
                                                 Attribute = d.Attribute,
                                                 AttributeType = d.AttributeType,
                                                 OldValue = d.OldValue,
                                                 NewValue = d.NewValue
                                             });
                taskProtocol.ProtocolDetail = protocolDetails;
            }

            return new AjaxStoreResult(data, data.Count());
        }

        public ActionResult GetFileTable(int taskId)
        {
            var data = from f in taskFilesRepository.FindAll()
                      where f.RefTask.ID == taskId
                     select new
                      {
                          f.ID,
                          f.FileName,
                          f.FileDescription,
                          f.CreateDate,
                          f.ChangeDate,
                          f.ChangeUser
                      };
            return new AjaxStoreResult(data, data.Count());
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult Save(int taskId, int refStatusId, string deadlineStr, string newComment, string taskFilesChangedData)
        {
            AjaxFormResult result = new AjaxFormResult();

            try
            {
                StringBuilder afterScript = new StringBuilder();
                
                D_CD_Protocol protocol = new D_CD_Protocol();
                List<D_CD_ProtocolDetail> protocolDetailList = new List<D_CD_ProtocolDetail>();
                bool protocolExist = false;

                var task = taskRepository.FindOne(taskId);
                bool statusChanged = task.RefStatus.ID != refStatusId;

                var taskViewModel = taskService.GetTaskViewModel(taskId);
                TaskPermisionSettings taskPermisionSettings = new TaskPermisionSettings(sessionState, taskViewModel);

                // Изменение статуса
                string resultMessage = "Изменения сохранены.";
                if (((task.RefStatus.ID == (int)TaskViewModel.TaskStatus.Edit) && taskPermisionSettings.CanEditTask && statusChanged)
                    ||
                    ((task.RefStatus.ID == (int)TaskViewModel.TaskStatus.OnTest) && taskPermisionSettings.CanSetVise && statusChanged))
                {
                    if (refStatusId == (int)TaskViewModel.TaskStatus.OnTest)
                    {
                        // Валидация отчета перед отправкой на рассмотрение
                        var form = Resolver.GetAll<IReportForm>().Where(x => x.ID == task.RefTemplate.Class).FirstOrDefault();
                        if (form != null && !form.Validate(task.ID))
                        {
                            result.Success = false;
                            result.ExtraParams["responseText"] = "Форма отчета не заполнена или имеются ошибки.";
                            return result;
                        }
                    }

                    var newStatus = statusRepository.Get(refStatusId);
                    protocolDetailList.Add(new D_CD_ProtocolDetail { Attribute = "Статус", OldValue = task.RefStatus.Name, NewValue = newStatus.Name, RefProtocol = protocol });
                    task.RefStatus = newStatus;
                    protocolExist = true;
                    resultMessage = "Задача переведена в состояние \"{0}\".".FormatWith(newStatus.Name);
                }

                // Изменение Даты сдачи(Deadline)
                if (deadlineStr.IsNotNullOrEmpty())
                {
                    DateTime deadline = JSON.Deserialize<DateTime>(deadlineStr);

                    if (taskPermisionSettings.CanChangeDeadline && (deadline != task.Deadline))
                    {
                        protocolDetailList.Add(new D_CD_ProtocolDetail { Attribute = "Дата сдачи", OldValue = task.Deadline.ToString("dd.MM.yyyy"), NewValue = deadline.ToString("dd.MM.yyyy"), RefProtocol = protocol });
                        task.Deadline = deadline;
                        protocolExist = true;
                    }
                }
                
                // Новый комментарий
                if (newComment.IsNotNullOrEmpty() && taskPermisionSettings.CanEditTask)
                {
                    protocol.Commentary = newComment;
                    protocolExist = true;

                    // На форме надо будет сбросить комментарий
                    afterScript.AppendLine("newComment.setValue('')");
                }

                // Сохраняем изменения таблицы файлов
                if (taskPermisionSettings.CanEditTask)
                {
                    StoreDataHandler dataHandler = new StoreDataHandler("{" + taskFilesChangedData + "}");
                    ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();
                    foreach (var created in data.Created)
                    {
                        throw new ArgumentException("Создание записей в таблице напрямую недопустимо.");
                    }

                    foreach (var deleted in data.Deleted)
                    {
                        int fileId = Convert.ToInt32(deleted["ID"]);
                        T_CD_Files oldFile = taskFilesRepository.FindOne(fileId);
                        taskFilesRepository.Delete(oldFile);

                        protocolDetailList.Add(new D_CD_ProtocolDetail
                                                   {
                                                       Attribute = "Файл",
                                                       AttributeType = (int)TaskProtocolDetailViewModel.ProtocolDetailAttributeType.File, 
                                                       OldValue = oldFile.FileName,
                                                       NewValue = null,
                                                       RefProtocol = protocol
                                                   });
                        protocolExist = true;
                    }

                    foreach (var updated in data.Updated)
                    {
                        int fileId = Convert.ToInt32(updated["ID"]);
                        T_CD_Files changedFile = taskFilesRepository.FindOne(fileId);

                        protocolDetailList.Add(new D_CD_ProtocolDetail
                        {
                            Attribute = "Файл",
                            AttributeType = (int)TaskProtocolDetailViewModel.ProtocolDetailAttributeType.File, 
                            OldValue = changedFile.FileName,
                            NewValue = updated["FileName"],
                            RefProtocol = protocol
                        });
                        protocolExist = true;

                        changedFile.FileName = updated["FileName"];
                        changedFile.FileDescription = updated["FileDescription"];
                        changedFile.ChangeUser = User.Identity.Name;
                        changedFile.ChangeDate = DateTime.Now;
                        taskFilesRepository.Save(changedFile);
                    }

                    if (data.Created.Count > 0 || data.Deleted.Count > 0 || data.Updated.Count > 0)
                    {
                        afterScript.AppendLine("{0}.commitChanges();".FormatWith(FileListPanel.StoreId));
                        afterScript.AppendLine("{0}.reload();".FormatWith(FileListPanel.StoreId));
                    }
                }

                if (protocolExist)
                {
                    // Добавляем запись в протокол
                    protocol.RefTask = task;
                    protocol.ChangeDate = DateTime.Now;
                    protocol.ChangeUser = User.Identity.Name;
                    taskProtocolRepository.Save(protocol);
                    foreach (D_CD_ProtocolDetail protocolDetail in protocolDetailList)
                    {
                        taskProtocolDetailRepository.Save(protocolDetail);
                    }

                    afterScript.AppendLine("if (this.dsProtocol != undefined) dsProtocol.reload();");
                }

                // Обновляем права доступа в соответсвии с новым статусом
                taskViewModel = taskService.GetTaskViewModel(taskId);
                taskPermisionSettings = new TaskPermisionSettings(sessionState, taskViewModel);

                // Устанавливаем видимость кнопок с действиями в соответстии с новым статусом и правами
                afterScript.AppendLine(taskPermisionSettings.CanEditTask ? "btnSave.show();" : "btnSave.hide();");
                afterScript.AppendLine(taskPermisionSettings.CanEditTask && taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.Edit
                    ? "btnToTest.show();"
                    : "btnToTest.hide();");
                afterScript.AppendLine(taskPermisionSettings.CanSetVise && taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.OnTest
                    ? "btnReject.show();btnAccept.show();"
                    : "btnReject.hide();btnAccept.hide();");
                afterScript.AppendLine(taskPermisionSettings.CanPumpReport && taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.Accepted
                    ? "btnPump.show();"
                    : "btnPump.hide();");
                afterScript.AppendLine("if (this.StatusName != undefined) StatusName.setValue('{0}')".FormatWith(taskViewModel.Status));
                afterScript.AppendLine("if (this.dsTask != undefined) dsTask.reload();");

                result.Success = true;
                result.Script = afterScript.ToString();
                if (protocolExist)
                {
                    result.ExtraParams["msg"] = resultMessage;
                }
                else
                {
                    result.ExtraParams["msg"] = "Изменений не было.";
                }

                // Обновляем имформацию о последнем изменении
                task.LastChangeDate = DateTime.Now;
                task.LastChangeUser = User.Identity.Name;

                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка сохранения.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
       }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult DeleteProtocolRow(int? protocolId)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                var protocol = taskProtocolRepository.FindOne((int)protocolId);

                // Проверяем security
                var taskViewModel = taskService.GetTaskViewModel(protocol.RefTask.ID);
                TaskPermisionSettings taskPermisionSettings = new TaskPermisionSettings(sessionState, taskViewModel);
                
                if (taskPermisionSettings.CanEditTask)
                {
                    ////foreach (var protocolDetail in taskProtocolDetailRepository.FindAll( f => f.RefProtocol.id == protocol.ID))
                    foreach (D_CD_ProtocolDetail protocolDetail in from d in taskProtocolDetailRepository.FindAll()
                                                                  where d.RefProtocol.ID == protocol.ID
                                                                 select new D_CD_ProtocolDetail { ID = d.ID })
                    {
                        taskProtocolDetailRepository.Delete(protocolDetail);
                    }

                    taskProtocolRepository.Delete(protocol);
                    result.Script = "dsProtocol.reload()";
                }
                else
                {
                    throw new SecurityException("Недостаточно прав.");
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Запись удалена.";
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка сохранения.";
                result.ExtraParams["responseText"] = e.Message;
                return result;
            }
        }

        public ActionResult DownloadFile(int fileId)
        {
            // Получаем документ с сервера
            var taskFile = taskFilesRepository.FindOne(fileId);
            
            // Проверяем security
            var taskViewModel = taskService.GetTaskViewModel(taskFile.RefTask.ID);
            TaskPermisionSettings taskPermisionSettings = new TaskPermisionSettings(sessionState, taskViewModel);
            if (!taskPermisionSettings.CanViewTask)
            {
                AjaxFormResult result = new AjaxFormResult();
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка передачи файла.";
                result.ExtraParams["responseText"] = "Недостаточно привилегий";
                result.IsUpload = true;
                return result;
            }

            try
            {
                FileContentResult file = new FileContentResult(taskFile.FileBody, GetContentMimeType(taskFile.FileName));
                file.FileDownloadName = taskFile.FileName;
                return file;
            }
            catch (Exception e)
            {
                AjaxFormResult result = new AjaxFormResult();
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка чтения файла.";
                result.ExtraParams["responseText"] = String.Format("Ошибка чтения файла из БД:{0}", e.Message);
                result.IsUpload = true;
                return result;
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult CreateOrUpdateFileWithUploadBody(int taskId, int? fileId, string fileName, string fileDescription)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                // Проверяем security
                var taskViewModel = taskService.GetTaskViewModel(taskId);
                TaskPermisionSettings taskPermisionSettings = new TaskPermisionSettings(sessionState, taskViewModel);
                if (!taskPermisionSettings.CanEditTask)
                {
                    throw new SecurityException("Недостаточно прав.");
                }
                
                HttpPostedFileBase file = Request.Files[0];
                D_CD_Task task = taskRepository.FindOne(taskId);
                var protocolDetail = new D_CD_ProtocolDetail();
                
                if (file.ContentLength > 0)
                {
                    T_CD_Files databaseFile = null;
                    if (fileId != null)
                    {
                        databaseFile = taskFilesRepository.FindOne((int)fileId);
                        if (databaseFile.RefTask.ID != taskId)
                        {
                            throw new Common.ServerException("Невозможно прикрепить загружаемый документ к текущей задаче, так как документ принадлежит другой задаче.");
                        }

                        protocolDetail.Attribute = "Файл";
                        protocolDetail.AttributeType = (int)TaskProtocolDetailViewModel.ProtocolDetailAttributeType.File; 
                        protocolDetail.OldValue = databaseFile.FileName;
                        protocolDetail.NewValue = fileName;

                        databaseFile.FileName = fileName;
                        databaseFile.FileDescription = fileDescription;
                        databaseFile.ChangeDate = DateTime.Now;
                        databaseFile.ChangeUser = User.Identity.Name;
                        databaseFile.FileBody = new byte[file.ContentLength];
                        file.InputStream.Read(databaseFile.FileBody, 0, file.ContentLength);
                    }
                    else
                    {
                        databaseFile = new T_CD_Files();
                        databaseFile.RefTask = task;
                        databaseFile.FileName = fileName;
                        databaseFile.FileDescription = fileDescription;
                        databaseFile.CreateDate = DateTime.Now;
                        databaseFile.ChangeUser = User.Identity.Name;
                        taskFilesRepository.Save(databaseFile);
                        databaseFile.FileBody = new byte[file.ContentLength];
                        file.InputStream.Read(databaseFile.FileBody, 0, file.ContentLength);
              
                        protocolDetail.Attribute = "Файл";
                        protocolDetail.AttributeType = (int)TaskProtocolDetailViewModel.ProtocolDetailAttributeType.File; 
                        protocolDetail.OldValue = null;
                        protocolDetail.NewValue = fileName;
                    }
                }

                // Добавляем запись в протокол
                var protocol = new D_CD_Protocol();
                protocol.RefTask = task;
                protocol.ChangeDate = DateTime.Now;
                protocol.ChangeUser = User.Identity.Name;
                taskProtocolRepository.Save(protocol);
                protocolDetail.RefProtocol = protocol;
                taskProtocolDetailRepository.Save(protocolDetail);
                
                result.Success = true;
                result.ExtraParams["msg"] = "Файл успешно передан на сервер.";
                result.IsUpload = true;
                result.Script = "dsTask.reload();dsProtocol.reload();";
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка передачи файла.";
                result.ExtraParams["responseText"] = e.Message;
                result.IsUpload = true;
                return result;
            }
        }
        
        private string GetContentMimeType(string fileName)
        {
            switch (Path.GetExtension(fileName))
            {
                case ".doc": return "application/msword";
                case ".dot": return "application/msword";
                case ".xls": return "application/vnd.ms-excel";
                case ".xla": return "application/vnd.ms-excel";
                case ".xlt": return "application/vnd.ms-excel";
                case ".xlc": return "application/vnd.ms-excel";
                case ".xlm": return "application/vnd.ms-excel";
                case ".xlw": return "application/vnd.ms-excel";
                case ".txt": return "text/plain";
                default: return "application/octet-stream";
            }
        }
    }
}
