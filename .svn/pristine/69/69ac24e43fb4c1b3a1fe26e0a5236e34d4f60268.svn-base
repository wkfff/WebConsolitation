using System;
using System.IO;
using System.Web.Mvc;

using Ext.Net.MVC;

using Krista.Diagnostics;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    // todo: переделать старый контроллер
    public sealed class DocController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;
        private readonly IDocService docService;

        public DocController(IDocService docService, INewRestService newRestService, IChangeLogService logService)
        {
            this.newRestService = newRestService;
            this.logService = logService;
            this.docService = docService;
        }
        
        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult Upload(int? id, string fileName, int size)
        {
            var result = new AjaxFormResult();

            if (!id.HasValue)
            {
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка передачи файла";
                result.ExtraParams["responseText"] = "Не указан идентификатор документа";
                result.IsUpload = true;
                return result;
            }

            F_Doc_Docum doc;

            try
            {
                doc = newRestService.GetItem<F_Doc_Docum>(id.GetValueOrDefault());
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка передачи файла";
                result.ExtraParams["responseText"] = string.Concat("Ошибка получения документа: ", e.Message);
                result.IsUpload = true;
                return result;
            }

            var file = Request.Files[0];

            try
            {
                var localFileName = string.Concat("doc", id, "_", DateTime.Now.ToString("yyyyMMdd_HHmmss"), "_", fileName);
                var localPath = DocHelpers.GetLocalFilePath(doc);
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }

                if (file != null)
                {
                    file.SaveAs(string.Concat(localPath, localFileName));
                }

                doc.Url = localFileName; ////note: Ограничение на повторную загрузку сделано на стороне клиента; если вызвать напрямую - старый файл повиснет.
                doc.FileSize = size;
                newRestService.Save(doc);

                result.Success = true;
                result.ExtraParams["msg"] = string.Concat("Файл '", fileName, "' успешно импортирован.");
                result.IsUpload = true;
                Resolver.Get<IChangeLogService>().WriteCreateFile(doc);
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка передачи файла";
                result.ExtraParams["responseText"] = string.Concat("Ошибка передачи файла: ", e.Message);
                result.IsUpload = true;
                return result;
            }
        }

/*
        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult CopyFiles(int RefParameterID)
        {
            var result = new AjaxFormResult();

            if ((Service as IDocService).CopyFiles(RefParameterID))
            {
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка копирования файла";
                result.ExtraParams["responseText"] = "Ошибка копирования файла: ";
                result.IsUpload = true;
                return result;
            }

            result.Success = true;
            result.ExtraParams["msg"] = "Файлы-приложения скопированы";
            result.ExtraParams["responseText"] = "Файлы-приложения скопированы";
            result.IsUpload = true;
            return result;
        }
*/

        public ActionResult Download(int? id)
        {
            if (id == null)
            {
                return Content("Ошибка выгрузки файла: Не указан документ-приложение", "application/octet-stream");
            }

            F_Doc_Docum doc;

            try
            {
                doc = newRestService.GetItem<F_Doc_Docum>(id.Value);
            }
            catch (Exception e)
            {
                return Content(string.Concat("Ошибка выгрузки файла: Документ-приложение не найден. ", e.Message), "application/octet-stream");
            }

            try
            {
                var fileName = docService.FindFile(doc);
                if (fileName == string.Empty)
                {
                    return Content("Ошибка выгрузки файла: Файл не найден на сервере.", "application/octet-stream");
                }

                var stream = new FileStream(fileName, FileMode.Open);
                return File(stream, "application/octet-stream", doc.Url);
            }
            catch (Exception e)
            {
                return Content(string.Concat("Ошибка выгрузки файла: ", e.Message), "application/octet-stream");
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult DeleteFile(int? id)
        {
            var result = new AjaxFormResult();

            if (id == null)
            {
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка удаления файла";
                result.ExtraParams["responseText"] = "Не указан идентификатор документа";
                result.IsUpload = true;
                return result;
            }

            F_Doc_Docum doc;

            try
            {
                doc = newRestService.GetItem<F_Doc_Docum>(id.Value);
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка удаления файла";
                result.ExtraParams["responseText"] = string.Concat("Ошибка получения документа: ", e.Message);
                result.IsUpload = true;
                return result;
            }

            try
            {
                docService.DeleteFile(doc);
                Resolver.Get<IChangeLogService>().WriteDeleteFile(doc);

                result.Success = true;
                result.ExtraParams["msg"] = "Загруженный ранее файл удален.";
                result.IsUpload = true;
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка удаления файла";
                result.ExtraParams["responseText"] = string.Concat("Ошибка удаления файла: ", e.Message);
                result.IsUpload = true;
                return result;
            }
        }

        /// <summary>
        /// Экшин удаления записи грида
        /// </summary>
        [HttpDelete]
        public RestResult DeleteAction(int id, int? docId, string modelType)
        {
            try
            {
                newRestService.BeginTransaction();

                var rec = newRestService.GetItem<F_Doc_Docum>(id);

                docService.Delete(id);
                logService.WriteDeleteFile(rec);
                
                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (newRestService.HaveTransaction)
                {
                    newRestService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DocDeleteAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));

                if (newRestService.HaveTransaction)
                {
                    newRestService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
