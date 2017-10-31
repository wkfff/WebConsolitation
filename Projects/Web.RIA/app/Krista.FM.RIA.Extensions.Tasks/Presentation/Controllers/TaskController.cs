using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Tasks
{
    public class TaskController : SchemeBoundController
    {
        private const string ViewRoot = "~/App_Resource/Krista.FM.RIA.Extensions.Tasks.dll/Krista.FM.RIA.Extensions.Tasks/Presentation/Views/Task/";
        //
        // GET: /Task/

        public ActionResult Show(int id)
        {
            ViewData["id"] = id;
            ITask task = Scheme.TaskManager.Tasks[id];
            return View(ViewRoot + "Show.aspx", task);
        }

        public AjaxStoreResult Get(int id)
        {
            ITask task = Scheme.TaskManager.Tasks[id];
            
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("TaskID", task.ID);
            data.Add("Headline", task.Headline);
            data.Add("Job", task.Job);
            data.Add("Description", task.Description);
            data.Add("FromDate", task.FromDate.ToString("dd.MM.yyyy hh:mm"));
            data.Add("ToDate", task.ToDate.ToString("dd.MM.yyyy hh:mm"));
            data.Add("Owner", task.Owner);
            string ownerName = Scheme.UsersManager.GetUserNameByID(task.Owner);
            data.Add("OwnerName", ownerName);
            data.Add("Doer", task.Doer);
            string doerName = Scheme.UsersManager.GetUserNameByID(task.Doer);
            data.Add("DoerName", doerName);
            data.Add("Curator", task.Curator);
            string curatorName = Scheme.UsersManager.GetUserNameByID(task.Curator);
            data.Add("CuratorName", curatorName);
            data.Add("State", task.State);
            data.Add("Editable", task.LockByCurrentUser());
            data.Add("Actions", task.GetActionsForState(task.State));
            data.Add("CashedAction", task.CashedAction);
            string currentUserName = User.Identity.Name.ToUpper();
            data.Add("CanDoAction", task.LockByUser < 0 && (currentUserName == ownerName.ToUpper() || currentUserName == doerName.ToUpper() || currentUserName == curatorName.ToUpper()));
            List<object> array = new List<object>();
            array.Add(data);
            bool b = task.PlacedInCacheOnly;
            return new AjaxStoreResult(array, array.Count());
        }

        public AjaxFormResult Save(int id, FormCollection values)
        {
            AjaxFormResult result = new AjaxFormResult();

            try
            {
                ITask task = Scheme.TaskManager.Tasks[id];
                task.Headline = values["fHeadline"];
                task.Job = String.IsNullOrEmpty(values["fTask"]) ? "Не указано" : values["fTask"];
                System.Data.DataTable udt = Scheme.UsersManager.GetUsers();
                task.Owner = Convert.ToInt32(udt.Select(String.Format("Name = '{0}'", values["fOwner"]))[0]["ID"]);
                task.Doer = Convert.ToInt32(udt.Select(String.Format("Name = '{0}'", values["fDoer"]))[0]["ID"]);
                task.Curator = Convert.ToInt32(udt.Select(String.Format("Name = '{0}'", values["fCurator"]))[0]["ID"]);
                task.Description = values["fDescription"];
                task.SaveStateIntoDatabase();
                task.EndUpdate();

                result.Success = true;
                return result;
            }
            catch (PermissionException e)
            {
                result.Success = false;
                result.Errors.Add(new FieldError("fFromDate", e.Message));
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Errors.Add(new FieldError(String.Empty, e.Message));
                return result;
            }
        }

        public AjaxResult Cancel(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                ITask task = Scheme.TaskManager.Tasks[id];
                task.CancelUpdate();
                if (task.IsNew)
                {
                    Scheme.TaskManager.Tasks.DeleteTask(task.ID);
                    task.Dispose();
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                return result;
            }
            return result;
        }

        public AjaxResult SetState(int id, string state)
        {
            ITask task = Scheme.TaskManager.Tasks[id];

            // определяем вид действия
            TaskActions action = Scheme.TaskManager.Tasks.FindActionsFromCaption(state);
            if (action == TaskActions.taDelete)
            {
                if (!task.PlacedInCacheOnly && task.InEdit)
                    task.CancelUpdate();
                try
                {
                    // удаляем только те задачи, которые находятся в основной таблице для задач
                    if (!task.PlacedInCacheOnly)
                        Scheme.TaskManager.Tasks.DeleteTask(task.ID);
                    task.Dispose();

                    return new AjaxResult("deleted") ;
                }
                catch (Exception e)
                {
                    return new AjaxResult
                        {
                           ErrorMessage = String.Format("Ошибка удаления задачи: {0}", e.Message)
                        };
                }
            }

            // определяем следующее состояние задачи
            string nextState = Scheme.TaskManager.Tasks.GetStateAfterAction(state);
            try
            {
                // пытаемся перевести задачу в режим редактирования
                task.BeginUpdate(state);
            }
            catch(Exception)
            {
                return new AjaxResult
                {
                    ErrorMessage = "Невозможно перейти в режим редактирования. Задача заблокирована другим пользователем."
                };
            }
            if (nextState != String.Empty)
            {
                task.State = nextState;
                task.SaveStateIntoDatabase();
            }
            return new AjaxResult();
        }

        public AjaxStoreResult GetDocuments(int id)
        {
            ITask task = Scheme.TaskManager.Tasks[id];

            System.Data.DataTable dt = new System.Data.DataTable();
            using (IDataUpdater du = task.GetTaskDocumentsAdapter())
            {
                du.Fill(ref dt);
            }

            dt.Columns.Add("DocumentTypeName", typeof(String));
            foreach (System.Data.DataRow row in dt.Rows)
            {
                row["SOURCEFILENAME"] = Path.GetExtension(Convert.ToString(row["SOURCEFILENAME"]));
                row["DocumentTypeName"] = TaskDocumentTypeToString((TaskDocumentType)Convert.ToInt32(row["DocumentType"]), Convert.ToString(row["SOURCEFILENAME"]));
            }

            return new AjaxStoreExtraResult(dt, dt.Rows.Count, null);
        }

        public void GetDocument(int taskId, int documentId)
        {
            // Получаем документ с сервера
            ITask task = Scheme.TaskManager.Tasks[taskId];
            System.Data.DataTable dt = new System.Data.DataTable();
            using (IDataUpdater du = task.GetTaskDocumentsAdapter())
            {
                du.Fill(ref dt);
            }
            DocumentFileInfo dfi = new DocumentFileInfo(
                Convert.ToString(dt.Select(String.Format("ID = {0}", documentId))[0]["NAME"]) +
                Path.GetExtension(Convert.ToString(dt.Select(String.Format("ID = {0}", documentId))[0]["SOURCEFILENAME"])),
                taskId, documentId);
            byte[] data = task.GetDocumentData(documentId);

            // Создаем временный каталог для хранения документов
            string folderPath = new[] { AppDomain.CurrentDomain.BaseDirectory, "Temp", Session.SessionID }.PathCombine();
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (!di.Exists)
            {
                di.Create();
            }
            
            // Сохраняем документ во временный каталог
            string tempFileName = new[] {folderPath, Path.GetFileName(dfi.FileName)}.PathCombine();
            using (FileStream fs = new FileStream(tempFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.Write(data, 0, data.Count());
                fs.Close();
            }

            // Устанавливаем контекст задачи
            TaskUtils.TaskUtils.SetTaskContext(tempFileName, task, documentId, Scheme);

            // Читаем файл из временного каталога и отдаем его клиенту
            FileInfo fi = new FileInfo(tempFileName);
            data = new byte[fi.Length];
            using (FileStream f = fi.OpenRead())
            {
                f.Read(data, 0, (int)fi.Length);
                f.Close();
            }

            Response.Clear();

            StringBuilder builder = new StringBuilder(255);
            builder.Append("attachment;filename=\"");
            builder.Append(taskId);
            builder.Append('_');
            builder.Append(documentId);
            builder.Append('_');
            builder.Append(FileHelper.GetDownloadableFileName(dfi.FileName));
            builder.Append('"');

            Response.ContentType = GetContentMimeType(dfi.FileName);
            Response.AddHeader("Content-Disposition", builder.ToString());
            Response.OutputStream.Write(data, 0, data.GetLength(0));
            GC.Collect();
            
            Trace.TraceVerbose("Объем памяти: {0}", GC.GetTotalMemory(true));
        }

        public string SaveDocument(int taskId, int documentId)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                using (new ServerContext())
                {
                    ITask task = Scheme.TaskManager.Tasks[taskId];

                    HttpPostedFileBase file = Request.Files["uploadField-file"];
                    if (file.ContentLength > 0)
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        using (IDataUpdater du = task.GetTaskDocumentsAdapter())
                        {
                            du.Fill(ref dt);
                        }

                        // Вытаскиваем из имени файла ID задачи, документа и имя документа
                        string sourceFileName = Convert.ToString(dt.Select(String.Format("ID = {0}", documentId))[0]["SOURCEFILENAME"]);
                        string documentName = Convert.ToString(dt.Select(String.Format("ID = {0}", documentId))[0]["NAME"]);
                        DocumentFileInfo dfi = new DocumentFileInfo(documentName + Path.GetExtension(sourceFileName), taskId, documentId);
                        try
                        {
                            dfi = new DocumentFileInfo(Path.GetFileName(file.FileName));
                        }
                        catch (Exception e)
                        {
                            throw new Common.ServerException(String.Format(
                                "Невозможно прикрепить загружаемый документ к текущей задаче, так как имя документа имеет неверный формат. Имя документа должно быть следующим: \"{0}\".", dfi.FormattedFileName), e);
                        }

                        if (dfi.TaskId != taskId)
                        {
                            throw new Common.ServerException(
                                "Невозможно прикрепить загружаемый документ к текущей задаче, так как документ принадлежит другой задаче.");
                        }

                        if (dfi.DocumentId != documentId)
                        {
                            throw new Common.ServerException(
                                "Невозможно прикрепить загружаемый документ к текущей задаче, так как у документа отличается его ID.");
                        }
                    
                        if (Path.GetExtension(dfi.FileName) != Path.GetExtension(sourceFileName))
                        {
                            throw new Common.ServerException(
                                "Тип загружаемого документа не соответствует типу исходного документа.");
                        }

                        byte[] data = new byte[file.ContentLength];
                        file.InputStream.Read(data, 0, file.ContentLength);
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();
                        task.SetDocumentData(documentId, data);
                        sw.Stop();
                    
                        Trace.TraceVerbose("Время загрузки документа в базу: {0}, размер документа: {1}", 
                            sw.Elapsed, file.ContentLength);
                    }
                }
                result.Success = true;
                return "{success:true,msg:'Файл успешно передан на сервер.'}";
            }
            catch (Common.ServerException e)
            {
                Trace.TraceError("Ошибка передачи файла: {0}", e.Message);
                return String.Format("{{success:false,msg:'Ошибка передачи файла: {0}'}}", e.Message);
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка передачи файла: {0}", e);
                return String.Format("{{success:false,msg:'Ошибка передачи файла: {0}'}}", e.Message);
            }
            finally
            {
                GC.Collect();
                Trace.TraceVerbose("Объем памяти: {0}", GC.GetTotalMemory(true));
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
            }
            return "";
        }

        public struct OfficeFileExt
        {
            public const string ofWordDocument = ".doc";
            public const string ofExcelDocument = ".xls";
            public const string ofWordDocumentNew = ".docx";
            public const string ofExcelDocumentNew = ".xlsx";
        }

        public static string TaskDocumentTypeToString(TaskDocumentType dt, string fileExt)
        {
            string fileTypeExt = String.Empty;
            switch (fileExt.ToLower())
            {
                case OfficeFileExt.ofExcelDocument:
                case OfficeFileExt.ofExcelDocumentNew:
                    fileTypeExt = "Надстройка MS Excel";
                    break;
                case OfficeFileExt.ofWordDocument:
                case OfficeFileExt.ofWordDocumentNew:
                    fileTypeExt = "Надстройка MS Word";
                    break;
            }

            switch (dt)
            {
                case TaskDocumentType.dtArbitraryDocument:
                    return "Произвольный документ";
                case TaskDocumentType.dtCalcSheet:
                    return fileTypeExt + " - Расчетный лист";
                case TaskDocumentType.dtDataCaptureList:
                    return fileTypeExt + " - Форма сбора данных";
                case TaskDocumentType.dtInputForm:
                    return fileTypeExt + " - Форма ввода";
                case TaskDocumentType.dtReport:
                    return fileTypeExt + " - Отчет";
                case TaskDocumentType.dtPlanningSheet:
                    return fileTypeExt;
                case TaskDocumentType.dtMDXExpertDocument:
                    return "Документ MDX Эксперт";
                case TaskDocumentType.dtWordDocument:
                    return "Документ MS Word";
                case TaskDocumentType.dtExcelDocument:
                    return "Документ MS Excel";
                case TaskDocumentType.dtDummyValue:
                    return String.Empty;
                default:
                    return "Неизвестный тип документа";
            }
        }

        private class DocumentFileInfo
        {
            public int TaskId { get; set; }
            public int DocumentId { get; set; }
            public string FileName { get; set; }

            public string FormattedFileName
            {
                get { return String.Format("{0}_{1}_{2}", TaskId, DocumentId, FileName); }
            }

            public DocumentFileInfo(string fileName)
            {
                // Вытаскиваем из имени файла ID задачи, документа и имя документа
                Regex ex = new Regex(@"^(?<taskId>\d+)_(?<documentId>\d+)_(?<fileName>[\w|\W]*)");
                if (ex.Match(fileName).Success)
                {
                    TaskId = Convert.ToInt32(ex.Match(fileName).Result("${taskId}"));
                    DocumentId = Convert.ToInt32(ex.Match(fileName).Result("${documentId}"));
                    FileName = ex.Match(fileName).Result("${fileName}");
                }
                else
                {
                    throw new Common.ServerException("Имя документа имеет неверный формат.");
                }
            }

            public DocumentFileInfo(string fileName, int taskId, int documentId)
            {
                // Вытаскиваем из имени файла ID задачи, документа и имя документа
                try
                {
                    Regex ex = new Regex(@"^(?<taskId>\d+)_(?<documentId>\d+)_(?<fileName>[\w|\W]*)");
                    if (ex.Match(fileName).Success)
                    {
                        TaskId = Convert.ToInt32(ex.Match(fileName).Result("${taskId}"));
                        DocumentId = Convert.ToInt32(ex.Match(fileName).Result("${documentId}"));
                        FileName = ex.Match(fileName).Result("${fileName}");
                    }
                    else
                    {
                        TaskId = taskId;
                        DocumentId = documentId;
                        FileName = fileName;
                    }
                }
                catch (Exception e)
                {
                    throw new Common.ServerException("Имя документа имеет неверный формат.", e);
                }
            }
        }
    }

}
