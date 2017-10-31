using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class UploadOGVFilesController : SchemeBoundController
    {
        private readonly AppFromOGVService requestsRepository;

        private readonly ILinqRepository<T_Doc_ApplicationOGV> filesRepository;

        public UploadOGVFilesController(
            AppFromOGVService requestsRepository,
            ILinqRepository<T_Doc_ApplicationOGV> filesRepository)
        {
            this.requestsRepository = requestsRepository;
            this.filesRepository = filesRepository;
        }

         [AcceptVerbs(HttpVerbs.Get)]
        public RestResult GetFiles(int applicationId)
        {
            try
            {
                var data = from f in filesRepository.FindAll().Where(x => x.RefApplicOGV.ID == applicationId)
                           select new
                                      {
                                          f.Name,
                                          f.Note,
                                          f.ID,
                                          f.Doc,
                                          DateDoc = f.DateDoc
                                            .ToString("d", CultureInfo.CreateSpecificCulture("de-DE")),
                                          f.Executor
                                      };

                return new RestResult { Success = true, Data = data };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult UploadFile(int taskId, int? fileId)
        {
            var result = new AjaxFormResult();

            var file = Request.Files[0];
            try
            {
                if (file.ContentLength > 0)
                {
                    var databaseFile = new T_Doc_ApplicationOGV
                                           {
                                               DateDoc = DateTime.Now,
                                               Executor = User.Identity.Name,
                                               Name = Request.Form.Get(0)
                                                    .Substring(0, Request.Form.Get(0).IndexOf(',')),
                                               Note = Request.Form.Get(1)
                                                    .Substring(0, Request.Form.Get(1).IndexOf(',')),
                                               RefApplicOGV = requestsRepository.Get(taskId),
                                               Doc = new byte[file.ContentLength]
                                           };

                    file.InputStream.Read(databaseFile.Doc, 0, file.ContentLength);

                    filesRepository.Save(databaseFile);

                    result.Success = true;
                    result.ExtraParams["msg"] = "Файл успешно передан на сервер.";
                    result.ExtraParams["date"] = databaseFile.DateDoc
                        .ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
                    result.ExtraParams["executor"] = databaseFile.Executor;
                    result.ExtraParams["id"] = databaseFile.ID.ToString();
                    result.ExtraParams["name"] = databaseFile.Name;
                    result.ExtraParams["note"] = databaseFile.Note;
                    result.IsUpload = true;
                    return result;
                }
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка передачи файла.";
                result.ExtraParams["responseText"] = e.Message;
                result.IsUpload = true;
                return result;
            }

            result.Success = false;
            result.ExtraParams["msg"] = "Ошибка передачи файла.";
            result.ExtraParams["responseText"] = string.Empty;
            result.IsUpload = true;
            return result;
        }

        public void DownloadFile(int fileId)
        {
            // Получаем документ с сервера
            var taskFile = filesRepository.FindOne(fileId);

            // Формируем ответ для клиента
            Response.Clear();

            var builder = new StringBuilder(255);
            builder.Append("attachment;filename=\"");
            builder.Append(FileHelper.GetDownloadableFileName(taskFile.Name));
            builder.Append('"');

            Response.ContentType = GetContentMimeType(taskFile.Name);
            Response.AddHeader("Content-Disposition", builder.ToString());
            Response.OutputStream.Write(taskFile.Doc, 0, taskFile.Doc.GetLength(0));
        }

        [Transaction]
        public AjaxStoreResult SaveFiles(int taskId, string storeChangedData)
        {
            var result = new AjaxStoreResult();

            var dataHandler = new StoreDataHandler("{" + storeChangedData + "}");
            var data = dataHandler.ObjectData<Dictionary<string, string>>();

            foreach (var created in data.Created)
            {
                // 0 - нет ошибок
                // 1 - пустое имя файла
                // 2 - пустое описание
                var error = 0;
                if (String.IsNullOrEmpty(created["Name"]))
                {
                    error = 1;
                }

                if (String.IsNullOrEmpty(created["Note"]))
                {
                    error = 2;
                }

                if (error > 0)
                {
                    result.SaveResponse.Success = false;
                    result.ResponseFormat = StoreResponseFormat.Save;
                    result.SaveResponse.Message = (error == 1)
                                              ? "Поле 'Наименование' должно быть заполнено"
                                              : "Поле 'Описание' должно быть заполнено";
                    return result; 
                }

                var newFile = new T_Doc_ApplicationOGV
                                                   {
                                                       Executor = User.Identity.Name,
                                                       DateDoc = DateTime.Now,
                                                       Name = created["Name"],
                                                       Note = created["Note"],
                                                       Doc = new byte[1],
                                                       RefApplicOGV = requestsRepository.Get(taskId)
                                                   };
                filesRepository.Save(newFile);
            }

            foreach (var deleted in data.Deleted)
            {
                var fileId = Convert.ToInt32(deleted["ID"]);
                var oldFile = filesRepository.FindOne(fileId);
                filesRepository.Delete(oldFile);
            }

            foreach (var updated in data.Updated)
            {
                var fileId = Convert.ToInt32(updated["ID"]);
                var changedFile = filesRepository.FindOne(fileId);

                changedFile.Name = updated["Name"];
                changedFile.Note = updated["Note"];
                changedFile.Executor = User.Identity.Name;
                changedFile.DateDoc = DateTime.Now;
                filesRepository.Save(changedFile);
            }

            result.SaveResponse.Success = true;
            result.SaveResponse.Message = "Информация о документах изменена";
            result.ResponseFormat = StoreResponseFormat.Save;
            result.Data = data.Updated;
            result.Total = data.Updated.Count;
            return result;
        }

        private static string GetContentMimeType(string fileName)
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

            return String.Empty;
        }
    }
}
