using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models.PfhdModel;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.PFHD
{
    public class CapFundsController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public CapFundsController()
        {
            newRestService = Resolver.Get<INewRestService>();
            logService = Resolver.Get<IChangeLogService>();
        }
        
        public ActionResult Read(int? parentId)
        {
            try
            {
                if (parentId == null)
                {
                    throw new InvalidDataException("Не указан родитель");
                }

                return new RestResult
                    {
                        Success = true, 
                        Data = newRestService.GetItems<F_Fin_CapFunds>()
                            .Where(x => x.RefParametr.ID == parentId)
                            .Select(
                                item => new CapFundsViewModel
                                    {
                                        ID = item.ID, 
                                        RefParameterID = item.RefParametr.ID, 
                                        Name = item.Name, 
                                        Funds = item.funds
                                    })
                    };
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "Ошибка загрузки Плана ФХД: " + e.Message, Data = null };
            }
        }
        
        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult Save(string data, int parentId)
        {
            try
            {
                F_Fin_CapFunds newPfhd = JavaScriptDomainConverter<F_Fin_CapFunds>.DeserializeSingle(data);
                newPfhd.RefParametr = newRestService.GetItem<F_F_ParameterDoc>(parentId);
                string msg = "Запись обновлена";
                if (newPfhd.ID < 0)
                {
                    newPfhd.ID = 0;
                    msg = "Новая запись добавлена";
                }

                var validationError = ValidateData(newPfhd);
                if (validationError == string.Empty)
                {
                    newRestService.Save(newPfhd);
                    logService.WriteChangeDocDetail(newPfhd.RefParametr);

                    return new RestResult
                        {
                            Success = true, 
                            Message = msg, 
                            Data = newRestService.GetItems<F_Fin_CapFunds>()
                                .Where(v => v.ID == newPfhd.ID)
                                .Select(
                                    item => new CapFundsViewModel
                                        {
                                            ID = item.ID, 
                                            RefParameterID = item.RefParametr.ID, 
                                            Name = item.Name, 
                                            Funds = item.funds
                                        })
                        };
                }

                throw new InvalidDataException(validationError);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = "Ошибка создания Плана ФХД: " + e.Message, Data = null };
            }
        }

        [HttpDelete]
        public RestResult Delete(int id, int parentId)
        {
            return newRestService.DeleteDocDetailAction<F_Fin_CapFunds>(id, parentId);
        }

        protected string ValidateData(F_Fin_CapFunds record)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(record.Name))
            {
                message += "Не указано наименование <br/>";
            }

            if (record.funds == null || record.funds == 0)
            {
                message += "Не указана сумма <br/>";
            }

            return message;
        }
    }
}
