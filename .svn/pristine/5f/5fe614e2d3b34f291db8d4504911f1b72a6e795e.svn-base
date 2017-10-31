using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Controllers
{
    public class DetailSubsidyListController : SchemeBoundController
    {
        private readonly IProgramService programService;
        private readonly ISubsidyService subsidyService;

        public DetailSubsidyListController(
                                   IProgramService programService,
                                   ISubsidyService subsidyService)
        {
            this.programService = programService;
            this.subsidyService = subsidyService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSubsidyListTable(int programId)
        {
            if (!User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer))
            {
                throw new SecurityException("Привилегий недостаточно.");
            }

            var data = this.subsidyService.GetSubsidyListTable(programId);

            return new AjaxStoreResult { Data = data, Total = data.Count };
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult CreateOrUpdateFileWithUploadBody(int programId, int? fileId, string fileName, string subsidyName)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                if (!User.IsInRole(ProgramRoles.Creator))
                {
                    throw new SecurityException("Редактирование файлов запрещено. Привилегий недостаточно.");
                }

                HttpPostedFileBase uploadFile = Request.Files[0];
                if (uploadFile.ContentLength <= 0)
                {
                    throw new ArgumentNullException("Файл пустой!");
                }

                byte[] fileBody = new byte[uploadFile.ContentLength];
                uploadFile.InputStream.Read(fileBody, 0, uploadFile.ContentLength);

                var program = programService.GetProgram(programId);

                if (fileId == null)
                {
                    subsidyService.InsertFile(program, fileName, fileBody, subsidyName);
                }
                else
                {
                    var editable = new PermissionSettings(User, program).CanEditDetail;
                    if (editable)
                    {
                        subsidyService.UpdateFile((int)fileId, program, fileName, fileBody, subsidyName);
                    }
                    else
                    {
                        throw new SecurityException("Недостаточно привилегий для изменения документа");
                    }
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

        public ActionResult DownloadFile(int fileId)
        {
            // Получаем документ с сервера
            string fileName;
            byte[] fileBody;
            string fileMimeType;
            subsidyService.GetFile(fileId, out fileBody, out fileName, out fileMimeType);

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
        public AjaxFormResult Save(int programId, string storeChangedData)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                var program = programService.GetProgram(programId);
                var editable = new PermissionSettings(User, program).CanEditDetail;
                if (!editable)
                {
                    throw new SecurityException("Недостаточно привилегий");
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
                    subsidyService.DeleteSubsidy(fileId);
                }

                foreach (var updated in data.Updated)
                {
                    int fileId = Convert.ToInt32(updated["ID"]);
                    string subsudyName = updated["SubsidyName"];

                    subsidyService.UpdateSubsidyAttributes(fileId, subsudyName);
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
