using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Controllers
{
    public class VisualizationController : SchemeBoundController
    {
        private readonly IFilesService fileService;
        private readonly IProjectService projectService;

        public VisualizationController(
                                        IFilesService fileService,
                                        IProjectService projectService)
        {
            this.fileService = fileService;
            this.projectService = projectService;
        }

        public ActionResult GetFileTable(int refProjId)
        {
            var data = fileService.GetFileTable(refProjId);
            return new AjaxStoreResult(data, data.Rows.Count);
        }

        public ActionResult DownloadFile(int fileId)
        {
            // Получаем документ с сервера
            string fileName;
            byte[] fileBody;
            string fileMimeType;
            fileService.GetFile(fileId, out fileBody, out fileName, out fileMimeType);

            try
            {
                FileContentResult file = new FileContentResult(fileBody, fileMimeType);
                file.FileDownloadName = fileName;

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
        public AjaxFormResult CreateOrUpdateFileWithUploadBody(int refProjId, int? fileId, string fileName)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                if (projectService.GetProjectStatus(refProjId) != InvProjStatus.Edit)
                {
                    throw new InvalidOperationException("Редактирование разрешено только для статуса проекта \"На редактировании\"");
                }

                HttpPostedFileBase uploadFile = Request.Files[0];
                if (uploadFile.ContentLength <= 0)
                {
                    throw new ArgumentNullException("Файл пустой!");
                }

                byte[] fileBody = new byte[uploadFile.ContentLength];
                uploadFile.InputStream.Read(fileBody, 0, uploadFile.ContentLength);

                if (fileId == null)
                {
                    fileService.InsertFile(refProjId, fileName, fileBody);
                }
                else
                {
                    fileService.UpdateFile((int)fileId, refProjId, fileName, fileBody);
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Файл успешно передан на сервер.";
                result.IsUpload = true;
                result.Script = String.Format("{0}.reload();", FileListPanel.StoreId);
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

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult Save(int refProjId, string storeChangedData)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                if (projectService.GetProjectStatus(refProjId) != InvProjStatus.Edit)
                {
                    throw new InvalidOperationException("Редактирование разрешено только для статуса проекта \"На редактировании\"");
                }

                StoreDataHandler dataHandler = new StoreDataHandler("{" + storeChangedData + "}");
                ChangeRecords<Dictionary<string, string>> data = dataHandler.ObjectData<Dictionary<string, string>>();
                foreach (var created in data.Created)
                {
                    throw new ArgumentException("Создание записей в таблице напрямую недопустимо.");
                }

                foreach (var deleted in data.Deleted)
                {
                    int fileId = Convert.ToInt32(deleted["ID"]);
                    fileService.DeleteFile(fileId);
                }

                foreach (var updated in data.Updated)
                {
                    int fileId = Convert.ToInt32(updated["ID"]);
                    fileService.UpdateFileName(fileId, updated["FileName"]);
                }

                StringBuilder afterScript = new StringBuilder();
                afterScript.AppendFormat("{0}.reload();\n", FileListPanel.StoreId);
                
                result.Success = true;
                result.Script = afterScript.ToString();
                result.ExtraParams["msg"] = "Изменения сохранены.";
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
    }
}
