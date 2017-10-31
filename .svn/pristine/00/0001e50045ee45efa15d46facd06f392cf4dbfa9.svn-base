using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models.StateTaskModel;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;
using Krista.FM.RIA.Extensions.E86N.Services.StateTaskService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTasks
{
    public class StateTaskController : SchemeBoundController
    {
        private readonly IStateTaskService stateTaskService;
        private readonly IChangeLogService logService;
        private readonly IDocService docService;

        public StateTaskController()
        {
            stateTaskService = Resolver.Get<IStateTaskService>();
            logService = Resolver.Get<IChangeLogService>();
            docService = Resolver.Get<IDocService>();
        }

        public RestResult Read(int parentId)
        {
            var data = from p in stateTaskService.GetItems<F_F_GosZadanie>()
                       where p.RefParametr.ID == parentId
                       select new
                                  {
                                      p.ID,
                                      RefParametr = p.RefParametr.ID,
                                      p.RazdelN,
                                      RefVedPch = p.RefVedPch.ID,
                                      RefVedPchOld = p.RefVedPch.ID,
                                      RefVedPchName = p.RefVedPch.Name,
                                      RefVedPchTip = p.RefVedPch.RefTipY.Name,
                                      RefVedPchTipID = p.RefVedPch.RefTipY.ID,
                                      RefVedPchCost = p.RefVedPch.RefPl.Name,
                                      RefVedPchCostID = p.RefVedPch.RefPl.Code,
                                      p.CenaEd
                                  };
            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult Save(string data, int parentId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);
               
                var validationError = ValidateData(dataUpdate, parentId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<F_F_GosZadanie>.DeserializeSingle(data.Replace("\"RazdelN\":\"\"", "\"RazdelN\":\"0\""));

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }
                else
                {
                    var oldId = JsonUtils.GetFieldOrDefault(dataUpdate, "RefVedPchOld", -1);
                    var newId = JsonUtils.GetFieldOrDefault(dataUpdate, "RefVedPch", -1);

                    if (!oldId.Equals(newId))
                    {
                        stateTaskService.GetItems<F_F_PNRZnach>().Where(p => p.RefFactGZ.ID.Equals(record.ID))
                            .Each(p => stateTaskService.Delete<F_F_PNRZnach>(p.ID));    
                    }
                }

                record.RefParametr = stateTaskService.GetItem<F_F_ParameterDoc>(parentId);
                record.RefVedPch = stateTaskService.GetItem<D_Services_VedPer>(record.RefVedPch.ID);
                record.RazdelN = default(int);

                stateTaskService.Save(record);
                logService.WriteChangeDocDetail(record.RefParametr);

                return new RestResult
                           {
                               Success = true,
                               Message = msg,
                               Data = from p in stateTaskService.GetItems<F_F_GosZadanie>()
                                      where p.ID == record.ID
                                      select new
                                                 {
                                                     p.ID,
                                                     RefParametr = p.RefParametr.ID,
                                                     RazdelN = default(int),
                                                     RefVedPch = p.RefVedPch.ID,
                                                     RefVedPchOld = p.RefVedPch.ID,
                                                     RefVedPchName = p.RefVedPch.Name,
                                                     RefVedPchTip = p.RefVedPch.RefTipY.Name,
                                                     RefVedPchTipID = p.RefVedPch.RefTipY.ID,
                                                     RefVedPchCost = p.RefVedPch.RefPl.Name,
                                                     RefVedPchCostID = p.RefVedPch.RefPl.Code,
                                                     p.CenaEd
                                                 }
                           };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
        
        [HttpDelete]
        public virtual RestResult Destroy(int id, int parentId)
        {
            try
            {
                stateTaskService.BeginTransaction();

                stateTaskService.DeleteDetails(id);

                stateTaskService.Delete<F_F_GosZadanie>(id);
               
                logService.WriteDeleteDocDetail(stateTaskService.GetItem<F_F_ParameterDoc>(parentId));

                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (stateTaskService.HaveTransaction)
                {
                    stateTaskService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteDocDetailAction: " + e.Message + " : " + e.ExpandException());

                if (stateTaskService.HaveTransaction)
                {
                    stateTaskService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public AjaxStoreResult GetServices(
            int limit,
            int start,
            int docId,
            string query)
        {
            var orgId = stateTaskService.GetItem<F_F_ParameterDoc>(docId).RefUchr.ID;
            var validService = stateTaskService.GetItems<F_F_VedPerProvider>().Where(p => p.RefProvider.ID == orgId);
            var data = stateTaskService.GetItems<D_Services_VedPer>().Where(p => validService.Any(x => x.RefService.ID == p.ID) && p.BusinessStatus == "801")
                .Where(p => (p.DataVkluch <= DateTime.Today || p.DataVkluch == null) && (p.DataIskluch >= DateTime.Today || p.DataIskluch == null) && p.Name.Contains(query))
                .Select(
                    p => new
                        {
                            p.ID,
                            p.Name,
                            TypeOfService = (p.RefTipY == null) ? string.Empty : p.RefTipY.Name,
                            TypeOfServiceID = (p.RefTipY == null) ? -1 : p.RefTipY.ID,
                            PaymentForServices = (p.RefPl == null) ? string.Empty : p.RefPl.Name,
                            PaymentForServicesCode = (p.RefPl == null) ? -1 : p.RefPl.Code,
                            PPO = (p.RefOrgPPO == null) ? -1 : p.RefOrgPPO.ID,
                            GRBS = (p.RefGRBSs == null) ? -1 : p.RefGRBSs.ID
                        });

            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
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

                var stateTasks = stateTaskService.GetRepository<F_F_GosZadanie>().FindAll()
                    .Where(
                        x =>
                        x.RefParametr.ID == recId).ToList();

                if (stateTasks.Count == 0)
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
            var formData = stateTaskService.GetItems<F_F_ParameterDoc>().First(x => x.ID == recId);

            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(recId).ID;
            try
            {
                var stateTasks = stateTaskService.GetItems<F_F_GosZadanie>()
                    .Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                if (stateTasks.Count > 0)
                {
                    foreach (var gosZadany in stateTasks)
                    {
                        var stateTask = new F_F_GosZadanie
                                            {
                                                SourceID = gosZadany.SourceID,
                                                TaskID = gosZadany.TaskID,
                                                CenaEd = gosZadany.CenaEd,
                                                RazdelN = gosZadany.RazdelN,
                                                RefVedPch = gosZadany.RefVedPch,
                                                RefParametr = formData
                                            };
                        foreach (var order in gosZadany.RenderOrders)
                        {
                            stateTask.RenderOrders.Add(order);
                        }

                        foreach (var limit in gosZadany.Limits)
                        {
                            stateTask.Limits.Add(limit);
                        }

                        foreach (var value in gosZadany.Indicators)
                        {
                            stateTask.Indicators.Add(value);
                        }

                        foreach (var procedure in gosZadany.InformingProcedures)
                        {
                            stateTask.InformingProcedures.Add(procedure);
                        }

                        stateTaskService.Save(stateTask);

                        // потребители услуги
                        var zadany = gosZadany;
                        var potrs = stateTaskService.GetItems<F_F_GZYslPotr>().Where(p => p.RefFactGZ.ID == zadany.ID);
                        foreach (var potr in potrs)
                        {
                            stateTaskService.Save(
                                new F_F_GZYslPotr
                                    {
                                        SourceID = potr.SourceID,
                                        TaskID = potr.TaskID,
                                        RefServicesCPotr = potr.RefServicesCPotr,
                                        RefFactGZ = stateTask
                                    });
                        }

                        // Показатеи оказания услуги
                        var zadany1 = gosZadany;
                        var pnrZnachs = stateTaskService.GetItems<F_F_PNRZnach>().Where(p => p.RefFactGZ.ID == zadany1.ID);
                        foreach (var pnrZnach in pnrZnachs)
                        {
                            stateTaskService.Save(
                                new F_F_PNRZnach
                                    {
                                        SourceID = pnrZnach.SourceID,
                                        TaskID = pnrZnach.TaskID,
                                        CurrentYear = pnrZnach.CurrentYear,
                                        ReportingYear = pnrZnach.ReportingYear,
                                        Protklp = pnrZnach.Protklp,
                                        ComingYear = pnrZnach.ComingYear,
                                        FirstPlanYear = pnrZnach.FirstPlanYear,
                                        SecondPlanYear = pnrZnach.SecondPlanYear,
                                        ActualValue = pnrZnach.ActualValue,
                                        RefIndicators = pnrZnach.RefIndicators,
                                        RefFactGZ = stateTask,
                                        Info = pnrZnach.Info,
                                        Source = pnrZnach.Source
                                    });
                        }

                        // НПА_устанавливающие цены
                        var zadany3 = gosZadany;
                        var npaCenas = stateTaskService.GetItems<F_F_NPACena>().Where(p => p.RefGZPr.ID == zadany3.ID);
                        foreach (var npaCena in npaCenas)
                        {
                            stateTaskService.Save(
                                new F_F_NPACena
                                    {
                                        SourceID = npaCena.SourceID,
                                        TaskID = npaCena.TaskID,
                                        RefGZPr = stateTask,
                                        Name = npaCena.Name,
                                        DataNPAGZ = npaCena.DataNPAGZ,
                                        NumNPA = npaCena.NumNPA,
                                        OrgUtvDoc = npaCena.OrgUtvDoc,
                                        VidNPAGZ = npaCena.VidNPAGZ
                                    });
                        }

                        // Нпа, регулирующий порядок оказания услуги
                        var gosZadany2 = gosZadany;
                        var npaRenderOrders = stateTaskService.GetItems<F_F_NPARenderOrder>().Where(p => p.RefFactGZ.ID == gosZadany2.ID);
                        foreach (var npaRenderOrder in npaRenderOrders)
                        {
                            stateTaskService.Save(
                                new F_F_NPARenderOrder
                                    {
                                        SourceID = npaRenderOrder.SourceID,
                                        TaskID = npaRenderOrder.TaskID,
                                        RenderEnact = npaRenderOrder.RenderEnact,
                                        RefFactGZ = stateTask,
                                        DateNpa = npaRenderOrder.DateNpa,
                                        NumberNpa = npaRenderOrder.NumberNpa,
                                        TypeNpa = npaRenderOrder.TypeNpa
                                    });
                        }

                        // Значения предельных цен
                        var gosZadany1 = gosZadany;
                        var limitPrices = stateTaskService.GetItems<F_F_LimitPrice>().Where(p => p.RefFactGZ.ID == gosZadany1.ID);
                        foreach (var limitPrice in limitPrices)
                        {
                            stateTaskService.Save(
                                new F_F_LimitPrice
                                    {
                                        SourceID = limitPrice.SourceID,
                                        TaskID = limitPrice.TaskID,
                                        Name = limitPrice.Name,
                                        Price = limitPrice.Price,
                                        RefFactGZ = stateTask
                                    });
                        }

                        // Порядок информирования потребителей
                        var zadany2 = gosZadany;
                        var infoProcedures = stateTaskService.GetItems<F_F_InfoProcedure>().Where(p => p.RefFactGZ.ID == zadany2.ID);
                        foreach (var infoProcedure in infoProcedures)
                        {
                            stateTaskService.Save(
                                new F_F_InfoProcedure
                                    {
                                        SourceID = infoProcedure.SourceID,
                                        TaskID = infoProcedure.TaskID,
                                        Method = infoProcedure.Method,
                                        Content = infoProcedure.Content,
                                        Rate = infoProcedure.Rate,
                                        RefFactGZ = stateTask
                                    });
                        }

                        stateTaskService.Save(stateTask);

                        stateTaskService.Save(formData);
                    }

                    // Порядок контроля за исполнением
                    var orderControls = stateTaskService.GetItems<F_F_OrderControl>().Where(p => p.RefFactGZ.ID == idOfLastDoc);
                    foreach (var orderControl in orderControls)
                    {
                        stateTaskService.Save(
                            new F_F_OrderControl
                                {
                                    SourceID = orderControl.SourceID,
                                    TaskID = orderControl.TaskID,
                                    Form = orderControl.Form,
                                    Supervisor = orderControl.Supervisor,
                                    Rate = orderControl.Rate,
                                    RefFactGZ = formData
                                });
                    }

                    // Требование к отчетности
                    var requestAccounts = stateTaskService.GetItems<F_F_RequestAccount>().Where(p => p.RefFactGZ.ID == idOfLastDoc);
                    foreach (var requestAccount in requestAccounts)
                    {
                        stateTaskService.Save(
                            new F_F_RequestAccount
                                {
                                    SourceID = requestAccount.SourceID,
                                    TaskID = requestAccount.TaskID,
                                    DeliveryTerm = requestAccount.DeliveryTerm,
                                    OtherInfo = requestAccount.OtherInfo,
                                    OtherRequest = requestAccount.OtherRequest,
                                    RefFactGZ = formData,
                                    ReportForm = requestAccount.ReportForm
                                });
                    }

                    // Основания для приостановления
                    var baseTerminations = stateTaskService.GetItems<F_F_BaseTermination>().Where(p => p.RefFactGZ.ID == idOfLastDoc);
                    foreach (var baseTermination in baseTerminations)
                    {
                        stateTaskService.Save(
                            new F_F_BaseTermination
                                {
                                    SourceID = baseTermination.SourceID,
                                    TaskID = baseTermination.TaskID,
                                    EarlyTerminat = baseTermination.EarlyTerminat,
                                    RefFactGZ = formData
                                });
                    }

                    var capFunds = stateTaskService.GetItems<F_Fin_CapFunds>().Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                    if (capFunds.Count > 0)
                    {
                        var capFund = new F_Fin_CapFunds
                                          {
                                              SourceID = capFunds.First().SourceID,
                                              TaskID = capFunds.First().TaskID,
                                              Name = capFunds.First().Name,
                                              funds = capFunds.First().funds,
                                              RefParametr = formData
                                          };

                        stateTaskService.Save(capFund);
                    }

                    var realAssets = stateTaskService.GetItems<F_Fin_realAssFunds>().Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                    if (realAssets.Count > 0)
                    {
                        var realAsset = new F_Fin_realAssFunds
                                            {
                                                SourceID = capFunds.First().SourceID,
                                                TaskID = capFunds.First().TaskID,
                                                Name = capFunds.First().Name,
                                                funds = capFunds.First().funds,
                                                RefParametr = formData
                                            };

                        stateTaskService.Save(realAsset);
                    }

                    var otherGrants = stateTaskService.GetItems<F_Fin_othGrantFunds>().Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                    if (otherGrants.Count > 0)
                    {
                        var otherGrant = new F_Fin_othGrantFunds
                                             {
                                                 SourceID = otherGrants.First().SourceID,
                                                 TaskID = otherGrants.First().TaskID,
                                                 RefOtherGrant = otherGrants.First().RefOtherGrant,
                                                 KOSGY = otherGrants.First().KOSGY,
                                                 funds = otherGrants.First().funds,
                                                 RefParametr = formData
                                             };

                        stateTaskService.Save(otherGrant);
                    }
                }

                logService.WriteChangeDocDetail(formData);
                
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

        /// <summary>
        /// Установка признака "Не доводить ГЗ"
        /// </summary>
        [Transaction]
        public ActionResult SetNotBring(int docId, bool value)
        {
            try
            {
                T_F_ExtHeader record;
                if (!stateTaskService.GetItems<T_F_ExtHeader>().Any(x => x.RefParametr.ID == docId))
                {
                    record = new T_F_ExtHeader
                    {
                        ID = 0,
                        RefParametr = stateTaskService.GetItem<F_F_ParameterDoc>(docId),
                        NotBring = value
                    };

                    stateTaskService.Save(record);
                }
                else
                {
                    record = stateTaskService.GetItems<T_F_ExtHeader>().SingleOrDefault(x => x.RefParametr.ID == docId);
                    if (record != null && record.NotBring != value)
                    {
                        record.NotBring = value;
                        stateTaskService.Save(record);
                    }
                }

                if (value)
                {
                    // если ставим галочку то данные из документа надо удалить
                    stateTaskService.DeleteDocForNotBring(docId);
                    docService.GetItems(docId).Each(x => docService.Delete(x.ID));
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

        public RestResult ExtHeaderRead(int docId)
        {
            var data = stateTaskService.GetItems<T_F_ExtHeader>().Where(x => x.RefParametr.ID.Equals(docId)).Select(p => new ExtHeaderModel
            {
                ID = p.ID,
                RefParameterID = p.RefParametr.ID,
                NotBring = p.NotBring
            });

            return new RestResult { Success = true, Data = data };
        }

        protected string ValidateData(JsonObject record, int parentId)
        {
            const string Msg1 = "У услуги \"{0}\" не указана платность. Обратитесь к {1} <br>";
            const string Msg2 = "\"Средневзвешенная цена за единицу услуги\" для бесплатной услуги(работы) \"{0}\" не должна иметь значения.<br>";
            const string Msg3 = "Не задана \"Средневзвешенная цена за единицу услуги\" для услуги \"{0}\".<br>";
            const string Msg4 = "Услуга(работа) \"{0}\" уже заведена.<br>";
            const string Msg5 = "Не указана \"Услуга(работа)\"";

            var message = string.Empty;
            var duplication = false;

            if (Convert.ToString(record["RefVedPch"]).IsNullOrEmpty())
            {
                message += Msg5;
            }
            else
            {
                var service = Convert.ToInt32(record["RefVedPch"]);
                var id = Convert.ToInt32(record["ID"]);

                if (stateTaskService.GetItems<F_F_GosZadanie>().Any(x => (x.ID != id) && (x.RefParametr.ID == parentId) && (x.RefVedPch.ID == service)))
                {
                    duplication = true;
                    message += Msg4.FormatWith(Convert.ToString(record["RefVedPchName"]));
                }

                // если дубль то не проверяем все остальное
                if (!duplication)
                {
                    // если не указана платность
                    if (Convert.ToString(record["RefVedPchCostID"]).IsNullOrEmpty() || Convert.ToInt32(record["RefVedPchCostID"]) == -1)
                    {
                        var ppo = stateTaskService.GetItem<F_F_ParameterDoc>(parentId).RefUchr.RefOrgPPO.Code;

                        var region = "{0}000000000".FormatWith(ConfigurationManager.AppSettings["ClientLocationOKATOCode"]);

                        if (ppo == region)
                        {
                            message += Msg1.FormatWith(Convert.ToString(record["RefVedPchName"]), "ГРБС");
                        }
                        else
                        {
                            message += Msg1.FormatWith(Convert.ToString(record["RefVedPchName"]), "ФО");
                        }
                    }
                    else
                    {
                        // для бесплатных работ услуг
                        if (Convert.ToInt32(record["RefVedPchCostID"]) == 1
                            && Convert.ToString(record["CenaEd"]).IsNotNullOrEmpty() &&
                            Convert.ToDecimal(record["CenaEd"]) != 0)
                        {
                            message += Msg2.FormatWith(Convert.ToString(record["RefVedPchName"]));
                        }

                        // проверять только для услуг 
                        if (Convert.ToInt32(record["RefVedPchTipID"]) == 1)
                        {
                            // если услуга платная то проверяем заполнение НПА и цены
                            if (Convert.ToInt32(record["RefVedPchCostID"]) == 2 ||
                                Convert.ToInt32(record["RefVedPchCostID"]) == 3)
                            {
                                if (Convert.ToString(record["CenaEd"]).IsNullOrEmpty() ||
                                    Convert.ToDecimal(record["CenaEd"]) == 0)
                                {
                                    message += Msg3.FormatWith(Convert.ToString(record["RefVedPchName"]));
                                }
                            }
                        }
                    }
                }
            }

            return message;
        }
    }
}