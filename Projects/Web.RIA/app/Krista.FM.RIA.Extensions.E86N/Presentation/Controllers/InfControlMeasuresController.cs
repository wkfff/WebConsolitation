using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;
using Ext.Net.MVC;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Models.InfControlMeasures;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Services.InfControlMeasures;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для представления "Сведения о проведенных контрольных мероприятиях и их результатах"
    /// </summary>
    public class InfControlMeasuresController : SchemeBoundController
    {
        private readonly IInfControlMeasuresService infControlMeasures;
        private readonly IAuthService auth;
        private readonly IDocService docService;
        private readonly IChangeLogService logService;

        public InfControlMeasuresController()
        {
            infControlMeasures = Resolver.Get<IInfControlMeasuresService>();
            auth = Resolver.Get<IAuthService>();
            docService = Resolver.Get<IDocService>();
            logService = Resolver.Get<IChangeLogService>();
        }

        [Transaction]
        public ActionResult SetNotInspectionActivity(int docId, bool value)
        {
            try
            {
                T_Fact_ExtHeader record;
                if (!infControlMeasures.GetItems<T_Fact_ExtHeader>().Any(x => x.RefParametr.ID == docId))
                {
                    record = new T_Fact_ExtHeader
                    {
                        ID = 0,
                        RefParametr = infControlMeasures.GetItem<F_F_ParameterDoc>(docId),
                        NotInspectionActivity = value
                    };

                    infControlMeasures.Save(record);
                }
                else
                {
                    record = infControlMeasures.GetItems<T_Fact_ExtHeader>().SingleOrDefault(x => x.RefParametr.ID == docId);
                    if (record != null && record.NotInspectionActivity != value)
                    {
                        record.NotInspectionActivity = value;
                        infControlMeasures.Save(record);
                    }
                }
                
                if (value)
                {
                    // если ставим галочку то данные из документа надо удалить
                    infControlMeasures.GetItems<F_Fact_InspectionEvent>().Where(x => x.RefParametr.ID == docId)
                        .Each(x => infControlMeasures.Delete<F_Fact_InspectionEvent>(x.ID));

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
            var data = from p in infControlMeasures.GetItems<T_Fact_ExtHeader>()
                       where p.RefParametr.ID == docId
                       select new ExtHeaderModel
                       {
                           ID = p.ID,
                           RefParameterID = p.RefParametr.ID,
                           NotInspectionActivity = p.NotInspectionActivity
                       };

            return new RestResult { Success = true, Data = data };
        }

        public RestResult InfControlMeasuresRead(int docId)
        {
            var data = from p in infControlMeasures.GetItems<F_Fact_InspectionEvent>()
                       where p.RefParametr.ID == docId
                       select new
                           {
                               p.ID,
                               RefParametr = p.RefParametr.ID,
                               p.Supervisor,
                               p.Topic,
                               p.EventBegin,
                               p.EventEnd,
                               p.Violation,
                               p.ResultActivity
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult InfControlMeasuresSave(string data, int docId)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = InfControlMeasuresValidateData(dataUpdate, docId);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                F_Fact_InspectionEvent record =
                    JavaScriptDomainConverter<F_Fact_InspectionEvent>.DeserializeSingle(data);

                string msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefParametr = infControlMeasures.GetItem<F_F_ParameterDoc>(docId);
                record.Supervisor = record.Supervisor.Trim();
                record.Topic = record.Topic.Trim();

                infControlMeasures.Save(record);
                logService.WriteChangeDocDetail(record.RefParametr);

                return new RestResult
                    {
                        Success = true,
                        Message = msg,
                        Data = from p in infControlMeasures.GetItems<F_Fact_InspectionEvent>()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID,
                                       RefParametr = p.RefParametr.ID,
                                       p.Supervisor,
                                       p.Topic,
                                       p.EventBegin,
                                       p.EventEnd,
                                       p.Violation,
                                       p.ResultActivity
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult InfControlMeasuresDelete(int id, int docId)
        {
            return infControlMeasures.DeleteDocDetailAction<F_Fact_InspectionEvent>(id, docId);
        }

        #region Versions

        [HttpPost]
        [Transaction]
        public RestResult CheckIfCanDocumentCopy(int recId)
        {
            if (!Resolver.Get<IVersioningService>().CheckDocs(recId))
            {
                return new RestResult
                {
                    Success = false,
                    Message = "Нет закрытых документов"
                };
            }

            var factInspectionEvents = infControlMeasures.GetItems<F_Fact_InspectionEvent>()
                .Where(
                    x =>
                    x.RefParametr.ID == recId).ToList();

            if (factInspectionEvents.Count == 0)
            {
                return new RestResult
                {
                    Success = true,
                    Message = "Документ пуст"
                };
            }

            return new RestResult
                {
                Success = false,
                Message = "Документ не пуст"
            };
        }

        [HttpPost]
        [Transaction]
        public RestResult CopyContent(int recId)
        {
            var formData = infControlMeasures.GetItem<F_F_ParameterDoc>(recId);
            var idOfLastDoc = Resolver.Get<IVersioningService>().GetDocumentForCopy(recId).ID;
            try
            {
                var extHeader = infControlMeasures.GetItems<T_Fact_ExtHeader>()
                                    .Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                if (extHeader.Count > 0)
                {
                    foreach (var item in extHeader.Select(x => new T_Fact_ExtHeader
                                                                        {
                                                                           NotInspectionActivity = x.NotInspectionActivity,
                                                                           RefParametr = formData
                                                                        }))
                    {
                        infControlMeasures.Save(item);
                    }
                }
                
                var factInspectionEvents = infControlMeasures.GetItems<F_Fact_InspectionEvent>()
                                                    .Where(x => x.RefParametr.ID == idOfLastDoc).ToList();

                if (factInspectionEvents.Count > 0)
                {
                    foreach (var factInspectionEvent in factInspectionEvents.Select(factInspectionEvent => new F_Fact_InspectionEvent
                                                      {
                                                          SourceID = factInspectionEvent.SourceID,
                                                          TaskID = factInspectionEvent.TaskID,
                                                          Topic = factInspectionEvent.Topic,
                                                          EventBegin = factInspectionEvent.EventBegin,
                                                          EventEnd = factInspectionEvent.EventEnd,
                                                          Violation = factInspectionEvent.Violation,
                                                          ResultActivity = factInspectionEvent.ResultActivity,
                                                          Supervisor = factInspectionEvent.Supervisor,
                                                          RefParametr = formData
                                                      }))
                    {
                        infControlMeasures.Save(factInspectionEvent);
                    }
                }

                infControlMeasures.Save(formData);
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

        #endregion

        public ActionResult ExportToXml(int recId)
        {
            return File(
                ExportInspectionActivityService.Serialize(auth, infControlMeasures.Load<F_F_ParameterDoc>(recId)),
                "application/xml",
                "inspectionActivity" + DateTime.Now.ToString("yyyymmddhhmmss") + ".xml");
        }

        private string InfControlMeasuresValidateData(JsonObject record, int docId)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg1 = "Дата окончания не может быть раньше даты начала!<br>";
            const string Msg2 = "Неправильно задан год \"{0}\" Должен быть {1} <br>";

            string message = string.Empty;

            bool begin = true, end = true;

            if (Convert.ToString(record[InfControlMeasures.Supervisor.ToString()]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith(InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.Supervisor));
            }

            if (Convert.ToString(record[InfControlMeasures.Topic.ToString()]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith(InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.Topic));
            }

            if (Convert.ToString(record[InfControlMeasures.EventBegin.ToString()]).IsNullOrEmpty())
            {
                begin = false;
                message += Msg.FormatWith(InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.EventBegin));
            }

            if (Convert.ToString(record[InfControlMeasures.EventEnd.ToString()]).IsNullOrEmpty())
            {
                end = false;
                message += Msg.FormatWith(
                    InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.EventEnd));
            }

            if (Convert.ToString(record[InfControlMeasures.EventBegin.ToString()]).IsNotNullOrEmpty() &&
                Convert.ToString(record[InfControlMeasures.EventEnd.ToString()]).IsNotNullOrEmpty())
            {
                if (Convert.ToDateTime(record[InfControlMeasures.EventBegin.ToString()]).Date >
                    Convert.ToDateTime(record[InfControlMeasures.EventEnd.ToString()]).Date)
                {
                    message += Msg1;
                }
            }

            var doc = infControlMeasures.Load<F_F_ParameterDoc>(docId);

            if (begin)
            {
                if (Convert.ToDateTime(record[InfControlMeasures.EventBegin.ToString()]).Year != doc.RefYearForm.ID)
                {
                    message += Msg2.FormatWith(InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.EventBegin), doc.RefYearForm.ID);
                }
            }

            if (end)
            {
                var y = Convert.ToDateTime(record[InfControlMeasures.EventEnd.ToString()]).Year;
                if ((y != doc.RefYearForm.ID) && (y != doc.RefYearForm.ID + 1))
                {
                    message += Msg2.FormatWith(InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.EventEnd), doc.RefYearForm.ID)
                                        + "или {0}".FormatWith(doc.RefYearForm.ID + 1);
                }
            }

            return message;
        }
    }
}