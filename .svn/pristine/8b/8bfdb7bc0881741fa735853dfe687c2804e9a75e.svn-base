using System;
using System.Collections.Generic;
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
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controllers
{
    public class EO15AIPUploadFilesController : SchemeBoundController
    {
        private readonly IConstructionService constrRepository;

        private readonly ILinqRepository<D_ExcCosts_Document> filesRepository;

        public EO15AIPUploadFilesController(
            IConstructionService constrRepository,
            ILinqRepository<D_ExcCosts_Document> filesRepository)
        {
            this.constrRepository = constrRepository;
            this.filesRepository = filesRepository;
        }

         [AcceptVerbs(HttpVerbs.Get)]
        public RestResult GetFiles(int objId)
        {
            try
            {
                var data = from f in filesRepository.FindAll().Where(x => x.RefDoc.ID == objId).ToList()
                           select new
                                      {
                                          Name = f.DocName,
                                          f.ID,
                                          f.Doc,
                                          CObjectId = f.RefDoc.ID
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
        public AjaxFormResult UploadFile(int objId, int? fileId)
        {
            var result = new AjaxFormResult();

            var file = Request.Files[0];
            try
            {
                if (file.ContentLength > 0)
                {
                    var databaseFile = new D_ExcCosts_Document
                                           {
                                               RefDoc = constrRepository.GetOne(objId),
                                               DocName = Request.Form.Get(0)
                                                    .Substring(0, Request.Form.Get(0).IndexOf(',')),
                                               DocFileName = Request.Form.Get(0)
                                                    .Substring(0, Request.Form.Get(0).IndexOf(',')),
                                               Doc = new byte[file.ContentLength]
                                           };

                    file.InputStream.Read(databaseFile.Doc, 0, file.ContentLength);

                    filesRepository.Save(databaseFile);

                    result.Success = true;
                    result.ExtraParams["msg"] = "Файл успешно передан на сервер.";
                    result.ExtraParams["id"] = databaseFile.ID.ToString();
                    result.ExtraParams["name"] = databaseFile.DocName;
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
            var doc = filesRepository.FindOne(fileId);

            // Формируем ответ для клиента
            Response.Clear();

            var builder = new StringBuilder(255);
            builder.Append("attachment;filename=\"");
            builder.Append(FileHelper.GetDownloadableFileName(doc.DocName));
            builder.Append('"');

            Response.ContentType = GetContentMimeType(doc.DocName);
            Response.AddHeader("Content-Disposition", builder.ToString());
            Response.OutputStream.Write(doc.Doc, 0, doc.Doc.GetLength(0));
        }

        [Transaction]
        public AjaxStoreResult SaveFiles(int objId, string storeChangedData)
        {
            var result = new AjaxStoreResult();

            var dataHandler = new StoreDataHandler("{" + storeChangedData + "}");
            var data = dataHandler.ObjectData<Dictionary<string, string>>();

            foreach (var created in data.Created)
            {
                // 0 - нет ошибок
                // 1 - пустое имя файла
                // 2 - пустое описание
                int error = 0;
                if (String.IsNullOrEmpty(created["Name"]))
                {
                    error = 1;
                }

                if (error > 0)
                {
                    result.SaveResponse.Success = false;
                    result.ResponseFormat = StoreResponseFormat.Save;
                    result.SaveResponse.Message = "Поле 'Документ' должно быть заполнено";
                    return result; 
                }

                var newFile = new D_ExcCosts_Document
                                                   {
                                                       Code = 0,
                                                       RowType = 0,
                                                       DocFileName = created["Name"],
                                                       DocName = created["Name"],
                                                       RefDoc = constrRepository.GetOne(objId),
                                                       Doc = new byte[1]
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

                changedFile.DocName = updated["Name"];
                changedFile.DocFileName = updated["Name"];
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
