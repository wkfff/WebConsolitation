using System;
using System.Data;
using System.IO;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.DebtBook.Services.Note;
using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.Controllers.Note
{
    public class DebtBookNoteController : SchemeBoundController
    {
        private const string FileName = "Note.xls";
        private const string FileMimeType = "application/vnd.ms-excel";

        private IDebtBookExtension debtBookExtension;
        private IDebtBookNoteService noteService;

        public DebtBookNoteController(
                                       IDebtBookExtension debtBookExtension,
                                       IDebtBookNoteService noteService)
        {
            this.debtBookExtension = debtBookExtension;
            this.noteService = noteService;
        }

        public ActionResult GetNotesTable()
        {
            if (this.debtBookExtension.UserRegionType != UserRegionType.Subject)
            {
                throw new SecurityException("Недостаточно привилегий");
            }

            var data = noteService.GetChildRegionsNotesList(
                                                             this.debtBookExtension.UserRegionId, 
                                                             this.debtBookExtension.Variant.Id, 
                                                             this.debtBookExtension.CurrentSourceId);

            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult DownloadNote(int id)
        {
            CheckSecurityReadPermission(id);

            // Получаем документ с сервера
            byte[] fileBody = noteService.GetFile(id);

            try
            {
                FileContentResult file = new FileContentResult(fileBody, FileMimeType);
                file.FileDownloadName = FileName;

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
        public AjaxFormResult UploadNote()
        {
            AjaxFormResult result = new AjaxFormResult();

            try
            {
                CheckSecurityWritePermission();

                HttpPostedFileBase uploadFile = Request.Files[0];
                if (uploadFile.ContentLength <= 0)
                {
                    throw new ArgumentNullException("Файл пустой!");
                }

                if (debtBookExtension.CurrentVariantBlocked())
                {
                    throw new Exception("Вариант заблокирован от изменений");
                }
                
                byte[] fileBody = new byte[uploadFile.ContentLength];
                uploadFile.InputStream.Read(fileBody, 0, uploadFile.ContentLength);
                
                CheckExcelDocument(fileBody);

                var variantId = this.debtBookExtension.Variant.Id;
                if (variantId < 0)
                {
                    throw new Exception("Вариант не выбран!");
                }

                noteService.UploadFile(
                                       this.debtBookExtension.UserRegionId,
                                       variantId,
                                       this.debtBookExtension.CurrentSourceId,
                                       fileBody);

                StringBuilder script = new StringBuilder();
                script.AppendLine("Ext.MessageBox.hide();");
                script.AppendLine("fldFileUpload.reset();");
                script.AppendFormat("dsNote.reload();");

                result.Success = true;
                result.ExtraParams["msg"] = "Файл успешно передан на сервер.";
                result.IsUpload = true;
                result.Script = script.ToString();
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка передачи файла.";
                result.ExtraParams["responseText"] = e.Message;
                result.IsUpload = true;
                return result;
            }
        }

        public ActionResult GetNote()
        {
            CheckSecurityReadPermission(null);

            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("ID", typeof(string), null));
            data.Columns.Add(new DataColumn("Editable", typeof(bool), null));
            
            int? noteId = noteService.GetNoteId(
                                                this.debtBookExtension.UserRegionId,
                                                this.debtBookExtension.Variant.Id,
                                                this.debtBookExtension.CurrentSourceId);

            DataRow row = data.NewRow();
            row["ID"] = noteId;
            row["Editable"] = !debtBookExtension.CurrentVariantBlocked();
            data.Rows.Add(row);

            return new AjaxStoreResult(data, data.Rows.Count);
        }

        public ActionResult DeleteNote()
        {
            AjaxFormResult result = new AjaxFormResult();

            try
            {
                CheckSecurityWritePermission();
                
                int? noteId = noteService.GetNoteId(
                    this.debtBookExtension.UserRegionId,
                    this.debtBookExtension.Variant.Id,
                    this.debtBookExtension.CurrentSourceId);

                if (noteId != null)
                {
                    if (debtBookExtension.CurrentVariantBlocked())
                    {
                        throw new Exception("Вариант заблокирован от изменений");
                    }
                
                    noteService.DeleteNote((int)noteId);
                }
                
                StringBuilder script = new StringBuilder();
                script.AppendLine("Ext.MessageBox.hide();");
                script.AppendFormat("dsNote.reload();");

                result.Success = true;
                result.ExtraParams["msg"] = "Пояснительная записка удалена.";
                result.IsUpload = true;
                result.Script = script.ToString();
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка передачи файла.";
                result.ExtraParams["responseText"] = e.Message;
                result.IsUpload = true;
                return result;
            }
        }

        private void CheckSecurityReadPermission(int? noteId)
        {
            if (this.debtBookExtension.UserRegionType == UserRegionType.Region
                    || this.debtBookExtension.UserRegionType == UserRegionType.Town)
            {
                if (noteId == null)
                {
                    return;
                }
                else
                {
                    var userNoteId = noteService.GetNoteId(
                                                           this.debtBookExtension.UserRegionId,
                                                           this.debtBookExtension.Variant.Id,
                                                           this.debtBookExtension.CurrentSourceId);
                    if (userNoteId == (int)noteId)
                    {
                        return;
                    }
                }
            }
            else if (this.debtBookExtension.UserRegionType == UserRegionType.Subject)
            {
                return;
            }

            throw new SecurityException("Недостаточно привилегий");
        }

        private void CheckSecurityWritePermission()
        {
            if (this.debtBookExtension.UserRegionType != UserRegionType.Region
                    && this.debtBookExtension.UserRegionType != UserRegionType.Town)
            {
                throw new SecurityException("Недостаточно привилегий");
            }
        }

        private void CheckExcelDocument(byte[] fileBody)
        {
            try
            {
                HSSFWorkbook workBook;
                using (var memoryStream = new MemoryStream(fileBody, false))
                {
                    workBook = new HSSFWorkbook(memoryStream);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Документ не является документом Excel", e);
            }
        }
    }
}
