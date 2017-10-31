using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models.PfhdModel;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.PFHD
{
    public class OtherGrantFundsController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public OtherGrantFundsController()
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
                        Data = newRestService.GetItems<F_Fin_othGrantFunds>()
                            .Where(x => x.RefParametr.ID == parentId)
                            .Select(
                                item => new OtherGrantFundsViewModel
                                    {
                                        ID = item.ID, 
                                        RefParameter = item.RefParametr.ID, 
                                        RefOtherGrant = item.RefOtherGrant.ID,
                                        RefOtherGrantCode = item.RefOtherGrant.Code,
                                        RefOtherGrantName = item.RefOtherGrant.Name,
                                        Funds = item.funds, 
                                        KOSGY = item.KOSGY ?? string.Empty
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
        public ActionResult Save(string data, int parentId)
        {
            try
            {
                F_Fin_othGrantFunds newPfhd = JavaScriptDomainConverter<F_Fin_othGrantFunds>.DeserializeSingle(data);
                newPfhd.RefParametr = newRestService.GetItem<F_F_ParameterDoc>(parentId);
                newPfhd.RefOtherGrant = newRestService.GetItem<D_Fin_OtherGant>(newPfhd.RefOtherGrant.ID);
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
                            Data = newRestService.GetItems<F_Fin_othGrantFunds>()
                                .Where(v => v.ID == newPfhd.ID)
                                .Select(
                                    item => new OtherGrantFundsViewModel
                                        {
                                            ID = item.ID, 
                                            RefParameter = item.RefParametr.ID, 
                                            RefOtherGrant = item.RefOtherGrant.ID,
                                            RefOtherGrantCode = item.RefOtherGrant.Code, 
                                            RefOtherGrantName = item.RefOtherGrant.Name,
                                            Funds = item.funds, 
                                            KOSGY = item.KOSGY ?? string.Empty
                                        })
                        };
                }

                throw new InvalidDataException(validationError);
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                return new RestResult { Success = false, Message = e.Message, Data = null };
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id, int parentId)
        {
            return newRestService.DeleteDocDetailAction<F_Fin_othGrantFunds>(id, parentId);
        }

        [Transaction]
        public ActionResult CalculateSumm(int docId)
        {
            try
            {
                var val = newRestService.GetItems<F_Fin_othGrantFunds>().Where(x => x.RefParametr.ID == docId).SumWithNull(x => x.funds) ?? 0;

                F_Fin_finActPlan pfhd = newRestService.GetItems<F_Fin_finActPlan>().First(x => (x.RefParametr.ID == docId) && (x.NumberStr == 0));

                if (pfhd.actionGrant != val)
                {
                    pfhd.actionGrant = val;
                    newRestService.Save(pfhd);
                }

                return new RestResult
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new RestResult
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }

        protected string ValidateData(F_Fin_othGrantFunds record)
        {
            var message = string.Empty;

            if (record.funds == null || record.funds == 0)
            {
                message += "Не указана сумма <br/>";
            }

            return message;
        }
    }
}
