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
using Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model;
using Krista.FM.RIA.Extensions.E86N.Models.StateTaskModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;
using Krista.FM.RIA.Extensions.E86N.Services.StateTaskService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTask2016
{
    public class StateTask2016Controller : SchemeBoundController
    {
        private readonly IStateTask2016Service stateTask2016Service;
        private readonly IChangeLogService logService;
        private readonly IDocService docService;

        public StateTask2016Controller()
        {
            stateTask2016Service = Resolver.Get<IStateTask2016Service>();
            logService = Resolver.Get<IChangeLogService>();
            docService = Resolver.Get<IDocService>();
        }

        public RestResult Read(int parentId)
        {
            var data = from p in stateTask2016Service.GetItems<F_F_GosZadanie2016>()
                       where p.RefParameter.ID == parentId
                       select new StateTask2016ViewModel
                       {
                           ID = p.ID,
                           RefParameter = p.RefParameter.ID,
                           RefService = p.RefService.ID,
                           RefServiceOld = p.RefService.ID,
                           RefServiceName = p.RefService.NameName,
                           RefServiceTypeName = p.RefService.RefType.Name,
                           RefServiceTypeCode = p.RefService.RefType.Code,
                           RefServicePayName = p.RefService.RefPay.Name,
                           RefServicePayCode = p.RefService.RefPay.Code,
                           RefServiceRegNum = p.RefService.Regrnumber,
                           RefServiceUniqueNumber = p.RefService.Regrnumber.Substring(19, 23),
                           RefServiceContentIndex = p.RefService.SvcCntsName1Val + ", " + p.RefService.SvcCntsName2Val + ", " + p.RefService.SvcCntsName3Val,
                           RefServiceConditionIndex = p.RefService.SvcTermsName1Val + ", " + p.RefService.SvcTermsName2Val,
                           OrdinalNumber = p.OrdinalNumber,
                           AveragePrice = p.AveragePrice,
                           IsOtherSources = p.IsOtherSources.GetValueOrDefault()
                       };
            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult Create(string data, int parentId)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var validationError = ValidateData(dataUpdate, parentId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                var record = JavaScriptDomainConverter<F_F_GosZadanie2016>.DeserializeSingle(
                    data.Replace("\"OrdinalNumber\":\"\"", "\"OrdinalNumber\":\"0\""));

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }
                else
                {
                    // удяляем детализации при смене услуги\работы
                    var oldServiceID = dataUpdate["RefServiceOld"];
                    if (!oldServiceID.Equals(record.RefService.ID))
                    {
                        stateTask2016Service.DeleteDetails(record.ID);
                    }
                }

                record.RefParameter = stateTask2016Service.GetItem<F_F_ParameterDoc>(parentId);
                record.RefService = stateTask2016Service.GetItem<D_Services_Service>(record.RefService.ID);

                stateTask2016Service.Save(record);
                logService.WriteChangeDocDetail(record.RefParameter);

                var returnData = from p in stateTask2016Service.GetItems<F_F_GosZadanie2016>()
                    where p.ID == record.ID
                    select new StateTask2016ViewModel
                    {
                        ID = p.ID,
                        RefParameter = p.RefParameter.ID,
                        RefService = p.RefService.ID,
                        RefServiceOld = p.RefService.ID,
                        RefServiceName = p.RefService.NameName,
                        RefServiceTypeName = p.RefService.RefType.Name,
                        RefServiceTypeCode = p.RefService.RefType.Code,
                        RefServicePayName = p.RefService.RefPay.Name,
                        RefServicePayCode = p.RefService.RefPay.Code,
                        RefServiceRegNum = p.RefService.Regrnumber,
                        RefServiceUniqueNumber = p.RefService.Regrnumber.Substring(19, 23),
                        RefServiceContentIndex = p.RefService.SvcCntsName1Val + ", " + p.RefService.SvcCntsName2Val + ", " + p.RefService.SvcCntsName3Val,
                        RefServiceConditionIndex = p.RefService.SvcTermsName1Val + ", " + p.RefService.SvcTermsName2Val,
                        OrdinalNumber = p.OrdinalNumber,
                        AveragePrice = p.AveragePrice,
                        IsOtherSources = p.IsOtherSources.GetValueOrDefault()
                    };

                return new RestResult
                           {
                               Success = true,
                               Message = msg,
                               Data = returnData
                           };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult Delete(int id, int parentId)
        {
            try
            {
                stateTask2016Service.BeginTransaction();

                stateTask2016Service.DeleteDetails(id);
                stateTask2016Service.Delete<F_F_GosZadanie2016>(id);
                
                logService.WriteDeleteDocDetail(stateTask2016Service.GetItem<F_F_ParameterDoc>(parentId));

                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (stateTask2016Service.HaveTransaction)
                {
                    stateTask2016Service.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteDocDetailAction: " + e.Message + " : " + e.ExpandException());

                if (stateTask2016Service.HaveTransaction)
                {
                    stateTask2016Service.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }
        
        public AjaxStoreResult GetServices(
           int? limit,
           int? start,
           int docId,
           string query,
           bool isOtherSources)
        {
            var doc = stateTask2016Service.GetItem<F_F_ParameterDoc>(docId);
            var orgId = doc.RefUchr.ID;
            var year = doc.RefYearForm.ID;
            var validService = stateTask2016Service.GetItems<F_F_ServiceInstitutionsInfo>().Where(p => p.RefStructure.ID == orgId);
            
            var data = stateTask2016Service.GetItems<D_Services_Service>()
                        .Where(p => validService.Any(x => x.RefService.ID == p.ID)
                                    && p.BusinessStatus.Equals(D_Services_Service.Included)
                                    && (!p.EffectiveBefore.HasValue || (p.EffectiveBefore.HasValue && p.EffectiveBefore.Value.Year >= year)));
            
            if (isOtherSources)
            {
                data = data.Where(p => p.IsEditable.HasValue && p.IsEditable.Value);
                data = year == 2016 
                        ? data.Where(p => !(p.FromPlaning.HasValue && p.FromPlaning.Value)) 
                        : data.Where(p => p.FromPlaning.HasValue && p.FromPlaning.Value);
            }
            else
            {
                data = data.Where(p => !p.IsEditable.HasValue || !p.IsEditable.Value);
            }

            if (query.IsNotNullOrEmpty())
            {
                data = data.Where(x => x.NameName.Contains(query) || x.Regrnumber.Contains(query) || x.NameCode.Contains(query));
            }
            
            var dataNew = data.Select(
                                        p =>
                                            new 
                                            {
                                                RefService = p.ID,
                                                p.NameName,
                                                p.Regrnumber,
                                                p.NameCode,
                                                RefServiceTypeName = p.RefType.Name,
                                                RefServiceTypeCode = p.RefType.Code,
                                                RefServicePayName = p.RefPay.Name,
                                                RefServicePayCode = p.RefPay.Code,
                                                RefServiceRegNum = p.Regrnumber,
                                                RefServiceUniqueNumber = p.Regrnumber.Substring(19, 5),
                                                RefServiceContentIndex = p.SvcCntsName1Val + ", " + p.SvcCntsName2Val + ", " + p.SvcCntsName3Val,
                                                RefServiceConditionIndex = p.SvcTermsName1Val + ", " + p.SvcTermsName2Val,
                                                p.SvcCntsName1Val,
                                                p.SvcCntsName2Val,
                                                p.SvcCntsName3Val,
                                                p.SvcTermsName1Val,
                                                p.SvcTermsName2Val,
                                                IsOtherSources = isOtherSources
                                            });

            return new AjaxStoreResult(dataNew.Skip(start ?? 0).Take(limit ?? 10), dataNew.Count());
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

                var stateTasks = stateTask2016Service.GetRepository<F_F_GosZadanie2016>().FindAll()
                    .Where(
                        x =>
                        x.RefParameter.ID == recId).ToList();

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
            var formData = stateTask2016Service.GetItems<F_F_ParameterDoc>().First(x => x.ID == recId);

            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(recId).ID;

            if (stateTask2016Service.GetItem<F_F_ParameterDoc>(idOfLastDoc).RefYearForm.ID < 2016)
            {
                return new RestResult
                {
                    Success = false,
                    Message = "Не найден подходящий документ для копирования"
                };
            }

            try
            {
                var stateTasks = stateTask2016Service.GetItems<F_F_GosZadanie2016>()
                    .Where(x => x.RefParameter.ID == idOfLastDoc).ToList();

                if (stateTasks.Count <= 0)
                {
                    return new RestResult
                        {
                            Success = true,
                            Message = "Нет данных для копирования"
                        };
                }

                foreach (var st in stateTasks)
                {
                    var newStateTask = new F_F_GosZadanie2016
                        {
                            SourceID = st.SourceID,
                            TaskID = st.TaskID,
                            AveragePrice = st.AveragePrice,
                            OrdinalNumber = st.OrdinalNumber,
                            IsOtherSources = st.IsOtherSources,
                            RefService = st.RefService,
                            RefParameter = formData
                        };
                    
                    stateTask2016Service.Save(newStateTask);

                    // потребители услуги
                    var potrs = stateTask2016Service.GetItems<F_F_GZYslPotr2016>().Where(p => p.RefFactGZ.ID == st.ID);
                    foreach (var potr in potrs)
                    {
                        stateTask2016Service.Save(
                            new F_F_GZYslPotr2016
                                {
                                    SourceID = potr.SourceID,
                                    TaskID = potr.TaskID,
                                    RefConsumersCategory = potr.RefConsumersCategory,
                                    RefFactGZ = newStateTask
                                });
                    }

                    // Показатеи оказания услуги
                    var pnrZnachs = stateTask2016Service.GetItems<F_F_PNRZnach2016>().Where(p => p.RefFactGZ.ID == st.ID);
                    foreach (var pnrZnach in pnrZnachs)
                    {
                        stateTask2016Service.Save(
                            new F_F_PNRZnach2016
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
                                    RefFactGZ = newStateTask,
                                    RefReport = pnrZnach.RefReport,
                                    Deviation = pnrZnach.Deviation,
                                    Reject = pnrZnach.Reject
                                });

                        // Средневзвешенная цена
                        foreach (var ap in pnrZnach.AveragePrices)
                        {
                            stateTask2016Service.Save(
                                new F_F_AveragePrice
                                    {
                                        NextYearDec = ap.NextYearDec,
                                        PlanFirstYearDec = ap.PlanFirstYearDec,
                                        PlanLastYearDec = ap.PlanLastYearDec,
                                        ReportYearDec = ap.ReportYearDec,
                                        CurrentYearDec = ap.CurrentYearDec,
                                        RefVolumeIndex = pnrZnach
                                    });
                        }
                    }

                    // НПА устанавливающие цены
                    var npaCenas = stateTask2016Service.GetItems<F_F_NPACena2016>().Where(p => p.RefFactGZ.ID == st.ID);
                    foreach (var npaCena in npaCenas)
                    {
                        stateTask2016Service.Save(
                            new F_F_NPACena2016
                                {
                                    SourceID = npaCena.SourceID,
                                    TaskID = npaCena.TaskID,
                                    RefFactGZ = newStateTask,
                                    Name = npaCena.Name,
                                    DataNPAGZ = npaCena.DataNPAGZ,
                                    NumNPA = npaCena.NumNPA,
                                    OrgUtvDoc = npaCena.OrgUtvDoc,
                                    VidNPAGZ = npaCena.VidNPAGZ
                                });
                    }

                    // Нпа, регулирующий порядок оказания услуги
                    var npaRenderOrders = stateTask2016Service.GetItems<F_F_NPARenderOrder2016>().Where(p => p.RefFactGZ.ID == st.ID);
                    foreach (var npaRenderOrder in npaRenderOrders)
                    {
                        stateTask2016Service.Save(
                            new F_F_NPARenderOrder2016
                                {
                                    SourceID = npaRenderOrder.SourceID,
                                    TaskID = npaRenderOrder.TaskID,
                                    RenderEnact = npaRenderOrder.RenderEnact,
                                    RefFactGZ = newStateTask,
                                    DateNpa = npaRenderOrder.DateNpa,
                                    NumberNpa = npaRenderOrder.NumberNpa,
                                    TypeNpa = npaRenderOrder.TypeNpa
                                });
                    }

                    // Порядок информирования потребителей
                    var infoProcedures = stateTask2016Service.GetItems<F_F_InfoProcedure2016>().Where(p => p.RefFactGZ.ID == st.ID);
                    foreach (var infoProcedure in infoProcedures)
                    {
                        stateTask2016Service.Save(
                            new F_F_InfoProcedure2016
                                {
                                    SourceID = infoProcedure.SourceID,
                                    TaskID = infoProcedure.TaskID,
                                    Method = infoProcedure.Method,
                                    Content = infoProcedure.Content,
                                    Rate = infoProcedure.Rate,
                                    RefFactGZ = newStateTask
                                });
                    }

                    stateTask2016Service.Save(newStateTask);
                }

                // Порядок контроля за исполнением
                var orderControls = stateTask2016Service.GetItems<F_F_OrderControl2016>().Where(p => p.RefFactGZ.ID == idOfLastDoc);
                foreach (var orderControl in orderControls)
                {
                    stateTask2016Service.Save(
                        new F_F_OrderControl2016
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
                var requestAccounts = stateTask2016Service.GetItems<F_F_RequestAccount2016>().Where(p => p.RefFactGZ.ID == idOfLastDoc);
                foreach (var requestAccount in requestAccounts)
                {
                    stateTask2016Service.Save(
                        new F_F_RequestAccount2016
                            {
                                SourceID = requestAccount.SourceID,
                                TaskID = requestAccount.TaskID,
                                DeliveryTerm = requestAccount.DeliveryTerm,
                                OtherRequest = requestAccount.OtherRequest,
                                RefFactGZ = formData,
                                ReportForm = requestAccount.ReportForm,
                                OtherIndicators = requestAccount.OtherIndicators,
                                PeriodicityTerm = requestAccount.PeriodicityTerm
                            });
                }

                // Основания для приостановления
                var baseTerminations = stateTask2016Service.GetItems<F_F_BaseTermination2016>().Where(p => p.RefFactGZ.ID == idOfLastDoc);
                foreach (var baseTermination in baseTerminations)
                {
                    stateTask2016Service.Save(
                        new F_F_BaseTermination2016
                            {
                                SourceID = baseTermination.SourceID,
                                TaskID = baseTermination.TaskID,
                                EarlyTerminat = baseTermination.EarlyTerminat,
                                RefFactGZ = formData
                            });
                }

                // Иная информация
                var otherInfos = stateTask2016Service.GetItems<F_F_OtherInfo>().Where(p => p.RefFactGZ.ID == idOfLastDoc);
                foreach (var otherInfo in otherInfos)
                {
                    stateTask2016Service.Save(
                        new F_F_OtherInfo
                            {
                                OtherInfo = otherInfo.OtherInfo,
                                RefFactGZ = formData
                            });
                }

                // Отчеты
                var reports = stateTask2016Service.GetItems<F_F_Reports>().Where(p => p.RefFactGZ.ID == idOfLastDoc);
                foreach (var report in reports)
                {
                    stateTask2016Service.Save(
                        new F_F_Reports
                            {
                                DateReport = report.DateReport,
                                HeadName = report.HeadName,
                                HeadPosition = report.HeadPosition,
                                NameReport = report.NameReport,
                                ReportGuid = report.ReportGuid,
                                RefFactGZ = formData
                            });
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
                if (!stateTask2016Service.GetItems<T_F_ExtHeader>().Any(x => x.RefParametr.ID == docId))
                {
                    record = new T_F_ExtHeader
                    {
                        ID = 0,
                        RefParametr = stateTask2016Service.GetItem<F_F_ParameterDoc>(docId),
                        NotBring = value
                    };

                    stateTask2016Service.Save(record);
                }
                else
                {
                    record = stateTask2016Service.GetItems<T_F_ExtHeader>().SingleOrDefault(x => x.RefParametr.ID == docId);
                    if (record != null && record.NotBring != value)
                    {
                        record.NotBring = value;
                        stateTask2016Service.Save(record);
                    }
                }

                if (value)
                {
                    // если ставим галочку то данные из документа надо удалить
                    stateTask2016Service.DeleteDocForNotBring(docId);
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
            var data = stateTask2016Service.GetItems<T_F_ExtHeader>().Where(x => x.RefParametr.ID.Equals(docId)).Select(p => new ExtHeaderModel
            {
                ID = p.ID,
                RefParameterID = p.RefParametr.ID,
                NotBring = p.NotBring,
                StatementTask = p.StatementTask.HasValue ? p.StatementTask.Value.ToString("dd.MM.yyyy") : null,
                
                StateTaskNumber = p.StateTaskNumber,
                ApproverFirstName = p.ApproverFirstName,
                ApproverLastName = p.ApproverLastName,
                ApproverMiddleName = p.ApproverMiddleName,
                ApproverPosition = p.ApproverPosition
            });

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public AjaxFormResult AdditionalInformationFormSave(FormCollection values, int docId)
        {
            ExtHeaderModel extHeaderModel = new ExtHeaderModel();
            var result = new AjaxFormResult();

            try
            {
                var record = stateTask2016Service.GetItem<F_F_ParameterDoc>(docId).StateTasksExtHeader.FirstOrDefault()
                                            ?? new T_F_ExtHeader
                                            {
                                                ID = 0,
                                                RefParametr = stateTask2016Service.Load<F_F_ParameterDoc>(docId)
                                            };

                // todo наваять конвертер преобразующий FormCollection в Domain
                record.NotBring = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => extHeaderModel.NotBring)], false);
                record.StatementTask = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => extHeaderModel.StatementTask)], (DateTime?)null);
                record.StateTaskNumber = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => extHeaderModel.StateTaskNumber)], (string)null);
                record.ApproverFirstName = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => extHeaderModel.ApproverFirstName)], (string)null);
                record.ApproverLastName = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => extHeaderModel.ApproverLastName)], (string)null);
                record.ApproverMiddleName = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => extHeaderModel.ApproverMiddleName)], (string)null);
                record.ApproverPosition = JsonUtils.ValueOrDefault(values[UiBuilders.NameOf(() => extHeaderModel.ApproverPosition)], (string)null);

                stateTask2016Service.Save(record);
                logService.WriteChangeDocDetail(stateTask2016Service.GetItem<F_F_ParameterDoc>(docId));

                result.Success = true;
                result.ExtraParams["msg"] = "Сохранено";
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = e.Message;
                return result;
            }
        }

        [Transaction]
        public ActionResult ChangeService(int docId)
        {
            try
            {
                stateTask2016Service.GetItems<F_F_GosZadanie2016>().Where(x => x.RefParameter.ID == docId)
                    .Each(
                        x =>
                        {
                            stateTask2016Service.DeleteDetails(x.ID);
                            stateTask2016Service.Delete<F_F_GosZadanie2016>(x.ID);
                        });
                return new RestResult { Success = true, Message = "Данные успешно удалены" };
            }
            catch
            {
                return new RestResult { Success = false, Message = "Ошибка удаления данных" };
            }
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

            if (Convert.ToString(record["RefService"]).IsNullOrEmpty())
            {
                message += Msg5;
            }
            else
            {
                var service = Convert.ToInt32(record["RefService"]);
                var id = Convert.ToInt32(record["ID"]);

                if (stateTask2016Service.GetItems<F_F_GosZadanie2016>().Any(x => (x.ID != id) && (x.RefParameter.ID == parentId) &&
                        x.RefService.ID == service))
                {
                    duplication = true;
                    message += Msg4.FormatWith(Convert.ToString(record["RefServiceName"]));
                }

                // если дубль то не проверяем все остальное
                if (!duplication)
                {
                    // если не указана платность
                    if (Convert.ToString(record["RefServicePayCode"]).IsNullOrEmpty() || Convert.ToInt32(record["RefServicePayCode"]) == -1)
                    {
                        var ppo = stateTask2016Service.GetItem<F_F_ParameterDoc>(parentId).RefUchr.RefOrgPPO.Code;

                        var region = "{0}000000000".FormatWith(ConfigurationManager.AppSettings["ClientLocationOKATOCode"]);

                        if (ppo == region)
                        {
                            message += Msg1.FormatWith(Convert.ToString(record["RefServiceName"]), "ГРБС");
                        }
                        else
                        {
                            message += Msg1.FormatWith(Convert.ToString(record["RefServiceName"]), "ФО");
                        }
                    }
                    else
                    {
                        // для бесплатных работ услуг
                        if (Convert.ToInt32(record["RefServicePayCode"]) == 2
                            && Convert.ToString(record["AveragePrice"]).IsNotNullOrEmpty() &&
                            Convert.ToDecimal(record["AveragePrice"]) != 0)
                        {
                            message += Msg2.FormatWith(Convert.ToString(record["RefServiceName"]));
                        }

                        // проверять только для услуг 
                        if (Convert.ToInt32(record["RefServiceTypeCode"]) == 0)
                        {
                            // если услуга платная то проверяем заполнение НПА и цены
                            if (Convert.ToInt32(record["RefServicePayCode"]) == 1)
                            {
                                if (Convert.ToString(record["AveragePrice"]).IsNullOrEmpty() ||
                                    Convert.ToDecimal(record["AveragePrice"]) == 0)
                                {
                                    message += Msg3.FormatWith(Convert.ToString(record["RefServiceName"]));
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