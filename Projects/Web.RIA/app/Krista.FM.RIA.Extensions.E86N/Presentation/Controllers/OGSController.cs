using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Data;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.InstitutionsRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Services.OGSService;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public sealed class OGSController : SchemeBoundController
    {
        private readonly IAuthService auth;
        private readonly IOGSService service;
        private readonly INewRestService restService;
        private readonly ILinqRepository<D_Org_Structure> repository;
        private readonly InstitutionsRegisterModel model = new InstitutionsRegisterModel();

        public OGSController()
        {
            auth = Resolver.Get<IAuthService>();
            service = Resolver.Get<IOGSService>();
            restService = Resolver.Get<INewRestService>();

            repository = new AuthRepositiory<D_Org_Structure>(
                restService.GetRepository<D_Org_Structure>(),
                auth,
                ppoIdExpr => ppoIdExpr.RefOrgPPO,
                grbsIdExpr => grbsIdExpr.RefOrgGRBS.ID,
                orgIdExpr => orgIdExpr.ID);
        }

        public AjaxStoreResult Read(int limit, int start, [FiltersBinder] FilterConditions filters)
        {
            var data = repository.FindAll()
                .Select(
                    p => new InstitutionsRegisterModel
                        {
                            ID = p.ID,
                            RefOrgPpo = p.RefOrgPPO.ID,
                            RefOrgPpoName = p.RefOrgPPO.Name,
                            RefTipYc = p.RefTipYc.ID,
                            RefTipYcName = p.RefTipYc.Name,
                            RefOrgGrbs = p.RefOrgGRBS.ID,
                            RefOrgGrbsName = p.RefOrgGRBS.Name,
                            Name = p.Name,
                            ShortName = p.ShortName,
                            Inn = p.INN,
                            Kpp = p.KPP,
                            Status = !p.CloseDate.HasValue,
                            OpenDate = p.OpenDate,
                            CloseDate = p.CloseDate
                        });

            if (!auth.IsAdmin())
            {
                data = data.Where(x => x.Status);
            }

            foreach (var filter in filters.Conditions)
            {
                if (filter.Name == model.NameOf(() => model.ID))
                {
                    switch (filter.Comparison)
                    {
                        case Comparison.Eq:
                            data = data.Where(v => v.ID == filter.ValueAsInt);
                            break;
                        case Comparison.Gt:
                            data = data.Where(v => v.ID > filter.ValueAsInt);
                            break;
                        case Comparison.Lt:
                            data = data.Where(v => v.ID < filter.ValueAsInt);
                            break;
                    }
                }

                if (filter.Name == model.NameOf(() => model.Status))
                {
                    data = data.Where(v => v.Status == filter.ValueAsBoolean);
                }

                if (filter.Name == model.NameOf(() => model.OpenDate))
                {
                    switch (filter.Comparison)
                    {
                        case Comparison.Eq:
                            data = data.Where(v => v.OpenDate == filter.ValueAsDate);
                            break;
                        case Comparison.Gt:
                            data = data.Where(v => v.OpenDate > filter.ValueAsDate);
                            break;
                        case Comparison.Lt:
                            data = data.Where(v => v.OpenDate < filter.ValueAsDate);
                            break;
                    }
                }

                if (filter.Name == model.NameOf(() => model.CloseDate))
                {
                    switch (filter.Comparison)
                    {
                        case Comparison.Eq:
                            data = data.Where(v => v.CloseDate == filter.ValueAsDate);
                            break;
                        case Comparison.Gt:
                            data = data.Where(v => v.CloseDate > filter.ValueAsDate);
                            break;
                        case Comparison.Lt:
                            data = data.Where(v => v.CloseDate < filter.ValueAsDate);
                            break;
                    }
                }

                var filterValue = filter.Value;
                if (filter.Name == model.NameOf(() => model.RefOrgGrbsName))
                {
                    data = data.Where(v => v.RefOrgGrbsName.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.RefOrgPpoName))
                {
                    data = data.Where(v => v.RefOrgPpoName.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.Name))
                {
                    data = data.Where(v => v.Name.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.ShortName))
                {
                    data = data.Where(v => v.ShortName.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.Inn))
                {
                    data = data.Where(v => v.Inn.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.Kpp))
                {
                    data = data.Where(v => v.Kpp.Contains(filterValue));
                }

                if (filter.Name == model.NameOf(() => model.RefTipYcName))
                {
                    data = data.Where(v => v.RefTipYcName.Contains(filterValue));
                }
            }

            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
        }

        [HttpPost]
        [Transaction]
        public RestResult Save(string data)
        {
            try
            {
                var formData = JavaScriptDomainConverter<InstitutionsRegisterModel>.DeserializeSingle(data);

                D_Org_Structure item;
                var msg = "Запись обновлена";
                if (formData.ID < 0)
                {
                    item = new D_Org_Structure { ID = 0 };
                    msg = "Новая запись добавлена";
                }
                else
                {
                    item = repository.Load(formData.ID);
                }

                // Сохранение типа учреждения только для новых учреждений, дальнейшее изменение по кнопке смены типа учреждения
                if (formData.ID < 0)
                {
                    item.RefTipYc = restService.GetItem<FX_Org_TipYch>(formData.RefTipYc);
                }
                
                item.Name = formData.Name;
                item.ShortName = formData.ShortName;
                item.INN = formData.Inn;
                item.KPP = formData.Kpp;
                item.OpenDate = formData.OpenDate;

                var profile = auth.Profile;
                if (auth.IsAdmin())
                {
                    item.RefOrgPPO = restService.GetItem<D_Org_PPO>(formData.RefOrgPpo);
                    item.RefOrgGRBS = restService.GetItem<D_Org_GRBS>(formData.RefOrgGrbs);
                }
                else if (auth.IsPpoUser())
                {
                    item.RefOrgPPO = restService.GetItem<D_Org_PPO>(profile.RefUchr.RefOrgPPO.ID);
                    item.RefOrgGRBS = restService.GetItem<D_Org_GRBS>(formData.RefOrgGrbs);
                    if (item.RefOrgGRBS.RefOrgPPO.ID != profile.RefUchr.RefOrgPPO.ID)
                    {
                        item.RefOrgGRBS = profile.RefUchr.RefOrgGRBS;
                    }
                }
                else if (auth.IsGrbsUser())
                {
                    item.RefOrgPPO = restService.GetItem<D_Org_PPO>(profile.RefUchr.RefOrgPPO.ID);
                    item.RefOrgGRBS = restService.GetItem<D_Org_GRBS>(profile.RefUchr.RefOrgGRBS.ID);
                }

                repository.Save(item);

                return new RestResult
                    {
                        Success = true,
                        Message = msg,
                        Data = repository.FindAll().Where(p => p.ID == item.ID)
                            .Select(
                                p => new InstitutionsRegisterModel
                                    {
                                        ID = p.ID,
                                        RefOrgPpo = p.RefOrgPPO.ID,
                                        RefOrgPpoName = p.RefOrgPPO.Name,
                                        RefTipYc = p.RefTipYc.ID,
                                        RefTipYcName = p.RefTipYc.Name,
                                        RefOrgGrbs = p.RefOrgGRBS.ID,
                                        RefOrgGrbsName = p.RefOrgGRBS.Name,
                                        Name = p.Name,
                                        ShortName = p.ShortName,
                                        Inn = p.INN,
                                        Kpp = p.KPP,
                                        Status = !p.CloseDate.HasValue,
                                        OpenDate = p.OpenDate,
                                        CloseDate = p.CloseDate
                                    })
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public RestResult Delete(int id)
        {
            try
            {
                if (restService.GetItems<F_F_ServiceInstitutionsInfo>().Any(p => p.RefStructure.ID == id) ||
                    restService.GetItems<F_F_ServiceProvider>().Any(p => p.RefProvider.ID == id) ||
                    restService.GetItems<F_F_VedPerProvider>().Any(p => p.RefProvider.ID == id) ||
                    restService.GetItems<F_F_ParameterDoc>().Any(p => p.RefUchr.ID == id) ||
                    restService.GetItems<D_Org_UserProfile>().Any(p => p.RefUchr.ID == id))
                {
                    return new RestResult
                        {
                            Success = false,
                            Message = "У данного учреждения существуют нормативные документы или учреждение указанно в качестве поствцика услуг. Удаление невозможно",
                            Data = null
                        };
                }
                
                return restService.DeleteAction<D_Org_Structure>(id);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "OGSController::Delete: Ошибка удаления записи: " + e.Message, Data = null };
            }
        }

        public ActionResult Export(int recId)
        {
            return File(
                ExportPassportService.Serialize(auth, restService.GetItem<F_F_ParameterDoc>(recId)),
                "application/xml",
                "institutionInfo_" + DateTime.Now.ToString("yyyymmddhhmmss") + ".xml");
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportFile(string fileName, int import)
        {
            var result = new AjaxFormResult();

            var file = Request.Files[0];
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var xmlfilereader = new XmlTextReader(file.InputStream);
                    switch (import)
                    {
                        // импорт из егрюл
                        case 1:
                            service.ImportFile(xmlfilereader);
                            break;

                        // импорт
                        case 2:
                            service.ImportFileOGSNew(xmlfilereader);
                            break;
                    }

                    result.Success = true;
                    result.ExtraParams["msg"] = "Файл '{0}' успешно импортирован.".FormatWith(fileName);
                    result.ExtraParams["name"] = fileName;
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
            result.ExtraParams["responseText"] = "Ошибка!";
            result.IsUpload = true;
            return result;
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportPPO(string fileName)
        {
            var result = new AjaxFormResult();

            var file = Request.Files[0];
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var xmlfile = XDocument.Load(new XmlTextReader(file.InputStream));

                    result.Success = true;
                    result.ExtraParams["msg"] = service.ImportFilePPO(xmlfile);
                    result.ExtraParams["name"] = fileName;
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
            result.ExtraParams["responseText"] = "Ошибка!";
            result.IsUpload = true;
            return result;
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportInstitutionType(string fileName)
        {
            var result = new AjaxFormResult();
            try
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var xmlfilereader = new XmlTextReader(file.InputStream);
                    service.ImportInstitutionType(xmlfilereader);
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Файл '{0}' успешно импортирован.".FormatWith(fileName);
                result.ExtraParams["name"] = fileName;
                result.IsUpload = true;
                return result;
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
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportOKVED(string fileName)
        {
            var result = new AjaxFormResult();

            try
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var xmlfilereader = new XmlTextReader(file.InputStream);
                    service.ImportOkved(xmlfilereader);
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Файл '{0}' успешно импортирован.".FormatWith(fileName);
                result.ExtraParams["name"] = fileName;
                result.IsUpload = true;
                return result;
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
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportOKTMO(string fileName)
        {
            var result = new AjaxFormResult();

            try
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var xmlfilereader = new XmlTextReader(file.InputStream);
                    service.ImportOKTMO(xmlfilereader);
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Файл '{0}' успешно импортирован.".FormatWith(fileName);
                result.ExtraParams["name"] = fileName;
                result.IsUpload = true;
                return result;
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
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportNsiOGS(string fileName)
        {
            var result = new AjaxFormResult();

            try
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var xmlfilereader = new XmlTextReader(file.InputStream);
                    service.ImportNsiOGS(xmlfilereader);
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Файл '{0}' успешно импортирован.".FormatWith(fileName);
                result.ExtraParams["name"] = fileName;
                result.IsUpload = true;
                return result;
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
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult ImportNsiBudget(string fileName)
        {
            var result = new AjaxFormResult();

            try
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var xmlfilereader = new XmlTextReader(file.InputStream);
                    service.ImportNsiBudget(xmlfilereader);
                }

                result.Success = true;
                result.ExtraParams["msg"] = "Файл '{0}' успешно импортирован.".FormatWith(fileName);
                result.ExtraParams["name"] = fileName;
                result.IsUpload = true;
                return result;
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
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult CloseOgs(int? recId, string closeDate, string note)
        {
            try
            {
                if (recId != null)
                {
                    using (new ServerContext())
                    {
                        Resolver.Get<IVersioningService>().CloseOgs(
                            (int)recId,
                            closeDate.IsNotNullOrEmpty() ? DateTime.Parse(closeDate) : DateTime.Now,
                            note);
                        DataTable users = Scheme.UsersManager.GetUsers();
                        var org = restService.GetItems<D_Org_UserProfile>().Single(t => t.RefUchr.ID == recId);
                        DataRow[] rows = users.Select(string.Concat("Name = ", org.UserLogin));
                        rows[0]["AllowPwdAuth"] = false;
                        org.IsActive = false;
                        var change = users.GetChanges();
                        Scheme.UsersManager.ApplayUsersChanges(change);
                    }
                }

                var result = new AjaxFormResult { Success = true };
                result.ExtraParams["msg"] = "Учреждение успешно закрыто";
                result.IsUpload = true;
                return result;
            }
            catch (Exception exception)
            {
                var result = new AjaxFormResult { Success = false };
                result.ExtraParams["msg"] = "При закрытии учреждения возникли ошибки";
                result.ExtraParams["responseText"] = "Ошибка закрытия документа: " + exception.Message;
                result.IsUpload = true;
                return result;
            }
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult OpenOgs(int? recId)
        {
            try
            {
                if (recId != null)
                {
                    using (new ServerContext())
                    {
                        Resolver.Get<IVersioningService>().OpenOgs((int)recId);
                        DataTable users = Scheme.UsersManager.GetUsers();
                        var org = restService.GetItems<D_Org_UserProfile>().Single(t => t.RefUchr.ID == recId);
                        DataRow[] rows = users.Select(string.Concat("Name = ", org.UserLogin));
                        rows[0]["AllowPwdAuth"] = true;
                        org.IsActive = true;
                        var change = users.GetChanges();
                        Scheme.UsersManager.ApplayUsersChanges(change);
                    }
                }

                var result = new AjaxFormResult { Success = true };
                result.ExtraParams["msg"] = "Учреждение успешно открыто";
                result.IsUpload = true;
                return result;
            }
            catch (Exception exception)
            {
                var result = new AjaxFormResult { Success = false };
                result.ExtraParams["msg"] = "При открытии учреждения возникли ошибки";
                result.ExtraParams["responseText"] = "Ошибка открытия документа: " + exception.Message;
                result.IsUpload = true;
                return result;
            }
        }

        public ActionResult GetPassportForOgs(int recId)
        {
            return new AjaxStoreResult(new { ID = recId, DocID = Resolver.Get<IVersioningService>().GetDocumentIdForOGS(recId) }, 1);
        }

        public AjaxStoreResult GetTypes()
        {
            var data = restService.GetItems<FX_Org_TipYch>();
            return new AjaxStoreResult(data, data.Count());
        }

        [HttpPost]
        [Transaction]
        public RestResult SetType(int recId, int typeId, string note, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                if (dateStart > DateTime.Now || dateEnd > DateTime.Now)
                {
                    throw new InvalidDataException("Даты открытия и завершения должны быть не позднее текущего числа");
                }

                if (dateStart > dateEnd)
                {
                    throw new InvalidDataException("Дата открытия должна быть раньше даты завершения");
                }

                var newType = restService.GetItem<FX_Org_TipYch>(typeId);
                var org = restService.GetItem<D_Org_Structure>(recId);
                
                restService.Save(new F_Org_TypeHistory
                    {
                        RefStructure = org,
                        RefTypeStructure = org.RefTipYc,
                        DateStart = dateStart,
                        DateEnd = dateEnd,
                        Note = note
                    });

                org.RefTipYc = newType;

                return new RestResult { Success = true };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public ActionResult GetLastTypeCloseDate(int recId)
        {
            var history = restService.GetItems<F_Org_TypeHistory>().Where(x => x.RefStructure.ID == recId);
            return history.Any() ? new AjaxStoreResult(history.Max(x => x.DateEnd)) : new AjaxStoreResult(null);
        }
    }
}