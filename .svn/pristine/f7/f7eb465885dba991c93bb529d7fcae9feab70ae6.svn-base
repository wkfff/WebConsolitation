using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Models;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Servises;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Presentation.Controllers
{
    public class AreaDetailController : SchemeBoundController
    {
        private readonly IAreaService areaService;
        private readonly IFilesService fileService;
        private readonly IUserCredentials userCredentials;

        public AreaDetailController(
                                     IAreaService areaService,
                                     IFilesService fileService,
                                     IUserCredentials userCredentials)
        {
            this.areaService = areaService;
            this.fileService = fileService;
            this.userCredentials = userCredentials;
        }

        public ActionResult Load(int? areaId)
        {
            if (!userCredentials.IsInAnyRole(InvAreaRoles.Creator, InvAreaRoles.Coordinator))
            {
                throw new SecurityException("Недостаточно привилегий");
            }

            AreaDetailViewModel data;
            if (areaId == null)
            {
                data = this.areaService.GetInitialAreaModel();
            }
            else
            {
                data = this.areaService.GetAreaModel((int)areaId);
            }

            return new AjaxStoreResult(data, 1);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult Save(int? areaId, int? statusForSet, FormCollection values)
        {
            var result = new AjaxFormResult();
            StringBuilder script = new StringBuilder();

            // Заменяем - вместо null приходит 0
            if (areaId == 0)
            {
                areaId = null;
            }

            try
            {
                if (!userCredentials.IsInAnyRole(InvAreaRoles.Creator, InvAreaRoles.Coordinator))
                {
                    throw new SecurityException("Недостаточно привилегий");
                }
             
                var model = ConvertToModel(values);

                D_InvArea_Reestr entityNew;
                D_InvArea_Reestr entityOld;
                
                if (areaId != null)
                {
                    entityNew = areaService.GetProject((int)areaId);
                    entityOld = (D_InvArea_Reestr)Copy(entityNew);
                }
                else
                {
                    entityNew = new D_InvArea_Reestr();
                    entityOld = null;
                }

                InvAreaStatus newStatus = statusForSet == null ? (InvAreaStatus)model.RefStatusId : (InvAreaStatus)statusForSet;
                StuffProjectEntityFromModel(entityNew, model, newStatus);

                AutofillDependentParameters(entityNew, entityOld);

                // Сохраняем (пусть там сам движёк разбирается что надо сохранять, что не надо сохранять, и можно ли вообще сохранять)
                areaService.SaveProject(entityNew, entityOld, userCredentials);

                // Если запись создавали как новую, то на форме необходимо выполнить манипуляции
                if (areaId == null)
                {
                    script.AppendFormat("ID.setValue({0});\n", entityNew.ID);
                }

                if (statusForSet == null)
                {
                    script.AppendLine("ResetDirtyAttributeOnFormItems(areaDetailForm);");
                    script.AppendLine("parent.dsAreas.reload();");
                    script.AppendFormat("{0}.setDisabled(false);\n", FileListPanel.PanelId);
                    script.AppendFormat("{0}.reload();\n", FileListPanel.StoreId);
                }
                else
                {
                    script.AppendLine("parent.dsAreas.reload();");
                    script.AppendLine("parentAutoLoadControl.forceHide = true;");
                    script.AppendLine("parentAutoLoadControl.hide();");
                }
                
                result.Success = true;
                result.ExtraParams["msg"] = "Запись изменена.";
                result.Script = script.ToString();
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

        public ActionResult GetFileTable(int areaId)
        {
            var data = fileService.GetFileTable(areaId);
            return new AjaxStoreResult(data, data.Rows.Count);
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult CreateOrUpdateFileWithUploadBody(int areaId, int? fileId, string fileName)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                if (areaService.GetProject(areaId).RefStatus.ID != (int)InvAreaStatus.Edit)
                {
                    throw new InvalidOperationException("Редактирование файлов разрешено только для статуса проекта \"На редактировании\"");
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
                    fileService.InsertFile(areaId, fileName, fileBody);
                }
                else
                {
                    fileService.UpdateFile((int)fileId, areaId, fileName, fileBody);
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
        public AjaxFormResult SaveFileNames(int areaId, string storeChangedData)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                if (areaService.GetProject(areaId).RefStatus.ID != (int)InvAreaStatus.Edit)
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

        /// <summary>
        /// Преобразует кучу параметров, полученных с формы, в модель данной вьюхи
        /// </summary>
        private AreaDetailViewModel ConvertToModel(FormCollection parameters)
        {
            CheckParameters(parameters);

            var model = new AreaDetailViewModel();
           
            foreach (var field in typeof(AreaDetailViewModel).GetProperties())
            {
                var property = model.GetType().GetProperty(field.Name);
                
                if (String.IsNullOrEmpty(parameters[property.Name]))
                {
                    property.SetValue(model, null, null);
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(model, parameters[property.Name], null);
                }
                else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                {
                    property.SetValue(model, Convert.ToInt32(parameters[property.Name]), null);
                }
                else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                {
                    var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
                    ci.NumberFormat.NumberDecimalSeparator = ",";
                    string val = parameters[property.Name].Replace(".", ",");
                    property.SetValue(model, Decimal.Parse(val, ci), null);
                }
                else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    property.SetValue(model, Convert.ToDateTime(parameters[property.Name]), null);
                }
                else
                {
                    throw new KeyNotFoundException("Неизвестный тип");
                }
            }

            model.RefStatusId = Convert.ToInt32(parameters["comboRefStatus_Value"]);

            return model;
        }

        /// <summary>
        /// Проверяет корректное заполенние параметров, передаваемых с формы
        /// </summary>
        private void CheckParameters(FormCollection parameters)
        {
            // TODO: проверка  всех входящих параметров на заполненность и соответствие типу
            if (!String.IsNullOrEmpty(parameters["RegNumber"]))
            {
                if (parameters["RegNumber"].Length > 25)
                {
                    throw new Exception("Длина поля <Регистрационный номер> не может быть больше 25 символов");
                }
            }

            if (!String.IsNullOrEmpty(parameters["CoordinatesLat"]))
            {
                CheckCoordinatesValue(parameters["CoordinatesLat"], "Широта");
            }

            if (!String.IsNullOrEmpty(parameters["CoordinatesLng"]))
            {
                CheckCoordinatesValue(parameters["CoordinatesLng"], "Широта");
            }

            return;
        }

        private void CheckCoordinatesValue(string coordinate, string name)
        {
            decimal d;
            try
            {
                var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
                ci.NumberFormat.NumberDecimalSeparator = ",";
                string val = coordinate.Replace(".", ",");
                d = Decimal.Parse(val, ci);
            }
            catch (Exception)
            {
                throw new Exception("Неверное значение координаты {0}".FormatWith(name));
            }

            if (d > 90 || d < -90)
            {
                throw new Exception("Значение координаты {0} должно быть между -90 и +90 градусов".FormatWith(name));
            }
        }

        /// <summary>
        /// Заполняет сущность параметрами из модели, которыми надо в зависимости от предыдущего статуса
        /// </summary>
        private void StuffProjectEntityFromModel(D_InvArea_Reestr entity, AreaDetailViewModel model, InvAreaStatus newStatus)
        {
            foreach (var field in typeof(D_InvArea_Reestr).GetProperties())
            {
                if ((field.Name == "RowType")
                    || (field.Name == "SourceID")
                    || (field.Name == "RefTerritory")
                    || (field.Name == "RefStatus"))
                {
                    continue;
                }

                var fieldSource = typeof(AreaDetailViewModel).GetProperty(field.Name);
                var value = fieldSource.GetValue(model, null);
                field.SetValue(entity, value, null);
            }

            entity.RefTerritory = areaService.GetRefTerritory(model.RefTerritoryId);
            entity.RefStatus = areaService.GetRefStatus((int)newStatus);
        }

        /// <summary>
        /// Заполняет поля, которые должны заполняться по условию автоматически
        /// </summary>
        private void AutofillDependentParameters(D_InvArea_Reestr entityNew, D_InvArea_Reestr entityOld)
        {
            // Если вновь созданная запись, то автозаполнение автора
            entityNew.CreateUser = entityOld == null ? userCredentials.User.Name : entityOld.CreateUser;

            // В зависимости от статуса ставим даты
            switch ((InvAreaStatus)entityNew.RefStatus.ID)
            {
                case InvAreaStatus.Edit:
                    entityNew.CreatedDate = null;
                    entityNew.AdoptionDate = null;
                    break;
                case InvAreaStatus.Review:
                    entityNew.CreatedDate = entityOld == null || entityOld.RefStatus.ID == (int)InvAreaStatus.Edit ? DateTime.Now : entityOld.CreatedDate;
                    entityNew.AdoptionDate = null;
                    break;
                case InvAreaStatus.Accepted:
                    entityNew.CreatedDate = entityOld == null ? DateTime.Now : entityOld.CreatedDate;
                    entityNew.AdoptionDate = entityOld == null || entityOld.RefStatus.ID == (int)InvAreaStatus.Review ? DateTime.Now : entityOld.AdoptionDate;
                    entityNew.Note = null;
                    break;
                default:
                    throw new Exception("Неверное значение статуса.");
            }
        }

        private object Copy(object obj)
        {
            var result = obj.GetType().GetConstructor(new Type[0]).Invoke(null);
            foreach (var property in obj.GetType().GetProperties())
            {
                property.SetValue(result, property.GetValue(obj, null), null);
            }

            return result;
        }
    }
}