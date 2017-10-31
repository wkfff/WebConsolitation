using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Services.SmetaService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    public class SmetaController : RestDetailController<F_Fin_Smeta, SmetaViewModel>
    {
        private readonly IAuthService auth;

        private readonly ILinqRepository<F_F_ParameterDoc> headers;

        private readonly IChangeLogService logService;

        private readonly INewRestService newRestService;

        public SmetaController(ISmetaService smetaService)
            : base(smetaService)
        {
            headers = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();
            auth = Resolver.Get<IAuthService>();
            logService = Resolver.Get<IChangeLogService>();
            newRestService = Resolver.Get<INewRestService>();
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult Save(string data, int parentId)
        {
            try
            {   // todo нахрена в смете работа с прикрепляемыми документами?????
                var jo = JsonUtils.FromJsonRaw(data);
                jo["RefParameterID"] = parentId;
                var record = Service.DecodeJson(jo);
                var validationError = ValidateData(record);
                if (validationError == string.Empty)
                {
                    string msg = "Запись обновлена";

                    if (record.ID == 0)
                    {
                        record.ID = 0;
                        msg = "Новая запись добавлена";
                    }

                    record.CelStatya = record.CelStatya.ToUpper();

                    var item = Service.Save(record);
                    logService.WriteChangeDocDetail(record.RefParametr);
                    return new RestResult { Success = true, Message = msg, Data = Service.ConvertToView(item) };
                }

                throw new InvalidDataException(validationError);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = e.Message, Data = null };
            }
        }

        [HttpPost]
        [Transaction]
        public RestResult CheckIfCanDocumentCopy(int recId)
        {
            try
            {
                if (!Resolver.Get<IVersioningService>().CheckCloseDocs(recId))
                {
                    return new RestResult
                        {
                            Success = false, 
                            Message = "Нет закрытых документов"
                        };
                }

                var smetas = Service.GetRepository().FindAll()
                    .Where(
                        x =>
                        x.RefParametr.ID == recId).ToList();
                if (smetas.Count == 0)
                {
                    return new RestResult
                        {
                            Success = true, 
                            Message = "Документ пустой"
                        };
                }

                return new RestResult
                    {
                        Success = false, 
                        Message = "Документ непустой"
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction]
        public RestResult CopyContent(int recId)
        {
            var repository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();

            var formData = repository.FindAll().First(x => x.ID == recId);

            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(recId).ID;
            try
            {
                var smetas = Service.GetRepository().FindAll()
                    .Where(
                        x =>
                        x.RefParametr.ID == idOfLastDoc).ToList();
                if (smetas.Count > 0)
                {
                    foreach (var sm in smetas)
                    {
                        var smeta = new F_Fin_Smeta
                            {
                                SourceID = sm.SourceID, 
                                TaskID = sm.TaskID, 
                                Funds = sm.Funds, 
                                FundsOneYear = sm.FundsOneYear, 
                                FundsTwoYear = sm.FundsTwoYear, 
                                CelStatya = sm.CelStatya, 
                                RefBudget = sm.RefBudget, 
                                RefParametr = formData, 
                                RefKbkBudget = sm.RefKbkBudget, 
                                RefRazdPodr = sm.RefRazdPodr, 
                                RefVidRash = sm.RefVidRash, 
                                RefKosgy = sm.RefKosgy, 
                                Event = sm.Event
                            };

                        formData.Smetas.Add(smeta);
                        Service.GetRepository().Save(smeta);
                    }
                }

                repository.Save(formData);
                repository.DbContext.CommitChanges();
                logService.WriteChangeDocDetail(headers.Load(recId));

                return new RestResult
                    {
                        Success = true, 
                        Message = "Данные скопированы"
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public ActionResult ExportToXml(int recId)
        {
            return File(
                ExportBudgetaryCircumstancesService.Serialize(auth, headers.Load(recId)), 
                "application/xml", 
                "budgetaryCircumstances" + DateTime.Now.ToString("yyyymmddhhmmss") + ".xml");
        }

        [HttpDelete]
        public ActionResult Delete(int id, int parentId)
        {
            return newRestService.DeleteDocDetailAction<F_Fin_Smeta>(id, parentId);
            /*logService.WriteDeleteDocDetail(Service.GetItem(id).RefParametr);
            return base.Delete(id);*/
        }

        private string ValidateData(F_Fin_Smeta record)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(record.CelStatya) || !Regex.IsMatch(record.CelStatya, @"[^0\.]"))
            {
                message += "Не указана целевая статья <br/>";
            }

            if (record.RefParametr.PlanThreeYear)
            {
                if ((record.Funds == null || record.Funds == 0) &&
                    (record.FundsOneYear == 0) &&
                    (record.FundsTwoYear == null || record.FundsTwoYear == 0))
                {
                    message += "Не указаны суммы ни за один год <br/>";
                }
            }
            else
            {
                if (record.Funds == null || record.Funds == 0)
                {
                    message += "Не указана сумма за {0}г. <br/>".FormatWith(record.RefParametr.RefYearForm.ID);
                }
            }

            return message;
        }
    }
}
