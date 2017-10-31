using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.Export;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Services.StateTaskService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTasks
{
    public class StateTaskViewController : SchemeBoundController
    {
        private readonly IAuthService auth;

        private readonly ILinqRepository<F_F_BaseTermination> baseTermination;

        private readonly ILinqRepository<F_F_GosZadanie> gosZadanie;

        private readonly ILinqRepository<F_F_ParameterDoc> headers;

        private readonly ILinqRepository<F_F_InfoProcedure> infoProcedure;

        private readonly ILinqRepository<F_F_LimitPrice> limitPrice;

        private readonly IChangeLogService logService;

        private readonly INewRestService newRestService;

        private readonly ILinqRepository<F_F_NPARenderOrder> npaRenderOrder;

        private readonly ILinqRepository<F_F_OrderControl> orderControl;

        public StateTaskViewController()
        {
            orderControl = Resolver.Get<ILinqRepository<F_F_OrderControl>>();
            headers = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();
            baseTermination = Resolver.Get<ILinqRepository<F_F_BaseTermination>>();
            npaRenderOrder = Resolver.Get<ILinqRepository<F_F_NPARenderOrder>>();
            gosZadanie = Resolver.Get<ILinqRepository<F_F_GosZadanie>>();
            limitPrice = Resolver.Get<ILinqRepository<F_F_LimitPrice>>();
            infoProcedure = Resolver.Get<ILinqRepository<F_F_InfoProcedure>>();
            auth = Resolver.Get<IAuthService>();
            logService = Resolver.Get<IChangeLogService>();
            newRestService = Resolver.Get<INewRestService>();
        }

        #region MonitoringExecution

        public RestResult MonitoringExecutionRead(int masterId)
        {
            var data = from p in orderControl.FindAll()
                       where p.RefFactGZ.ID == masterId
                       select new
                           {
                               p.ID, 
                               p.Form, 
                               p.Rate, 
                               p.Supervisor
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult MonitoringExecutionSave(string data, int masterId)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = MonitoringExecutionValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                F_F_OrderControl record = JavaScriptDomainConverter<F_F_OrderControl>.DeserializeSingle(data);

                string msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefFactGZ = headers.FindOne(masterId);
                record.Form = record.Form.Trim();
                record.Rate = record.Rate.Trim();
                record.Supervisor = record.Supervisor.Trim();

                orderControl.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ);

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = from p in orderControl.FindAll()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID, 
                                       p.Form, 
                                       p.Rate, 
                                       p.Supervisor
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult MonitoringExecutionDelete(int id, int masterId)
        {
            return newRestService.DeleteDocDetailAction<F_F_OrderControl>(id, masterId);
        }
        
        #endregion

        #region ReportingRequirements
        
        public RestResult ReportingRequirementsRead(int masterId)
        {
            var data = from p in newRestService.GetItems<F_F_RequestAccount>()
                       where p.RefFactGZ.ID == masterId
                       select new
                           {
                               p.ID, 
                               p.DeliveryTerm, 
                               p.OtherRequest, 
                               p.OtherInfo, 
                               p.ReportForm
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult ReportingRequirementsCreate(string data, int masterId)
        {
            try
            {
                var record = JavaScriptDomainConverter<F_F_RequestAccount>.DeserializeSingle(data);

                if (record.DeliveryTerm == string.Empty 
                    && record.OtherRequest == string.Empty 
                    && record.OtherInfo == string.Empty 
                    && record.ReportForm == string.Empty)
                {
                    return new RestResult
                        {
                            Success = false, 
                            Message = "Запись пуста"
                        };
                }

                record.ID = 0;
                record.RefFactGZ = newRestService.GetItem<F_F_ParameterDoc>(masterId);
                newRestService.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ);

                return new RestResult
                    {
                        Success = true, 
                        Message = "Новая запись добавлена", 
                        Data = from p in newRestService.GetItems<F_F_RequestAccount>()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID, 
                                       p.DeliveryTerm, 
                                       p.OtherRequest, 
                                       p.OtherInfo, 
                                       p.ReportForm
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction]
        public virtual RestResult ReportingRequirementsUpdate(int id, string data)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var record = JavaScriptDomainConverter<F_F_RequestAccount>.DeserializeSingle(data);
                if (record.DeliveryTerm == string.Empty 
                    && record.OtherRequest == string.Empty 
                    && record.OtherInfo == string.Empty 
                    && record.ReportForm == string.Empty)
                {
                    return new RestResult
                        {
                            Success = false, 
                            Message = "Запись пуста"
                        };
                }

                var recordDataUpdate = newRestService.GetItem<F_F_RequestAccount>(id);

                recordDataUpdate.DeliveryTerm = Convert.ToString(dataUpdate["DeliveryTerm"]);
                recordDataUpdate.OtherRequest = Convert.ToString(dataUpdate["OtherRequest"]);
                recordDataUpdate.OtherInfo = Convert.ToString(dataUpdate["OtherInfo"]);
                recordDataUpdate.ReportForm = Convert.ToString(dataUpdate[ReportingRequirementsFields.ReportForm.ToString()]);
                logService.WriteChangeDocDetail(recordDataUpdate.RefFactGZ);

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult ReportingRequirementsDelete(int id, int masterId)
        {
            return newRestService.DeleteDocDetailAction<F_F_RequestAccount>(id, masterId);
        }

        #endregion

        #region GroundsForTermination
        
        public RestResult GroundsForTerminationRead(int masterId)
        {
            var data = from p in baseTermination.FindAll()
                       where p.RefFactGZ.ID == masterId
                       select new
                           {
                               p.ID, 
                               p.EarlyTerminat
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult GroundsForTerminationCreate(string data, int masterId)
        {
            try
            {
                var record = JavaScriptDomainConverter<F_F_BaseTermination>.DeserializeSingle(data);

                if (record.EarlyTerminat == string.Empty)
                {
                    return new RestResult
                        {
                            Success = false, 
                            Message = "Запись пуста"
                        };
                }

                record.ID = 0;
                record.RefFactGZ = headers.FindOne(masterId);
                baseTermination.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ);

                return new RestResult
                    {
                        Success = true, 
                        Message = "Новая запись добавлена", 
                        Data = from p in baseTermination.FindAll()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID, 
                                       p.EarlyTerminat
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction]
        public virtual RestResult GroundsForTerminationUpdate(int id, string data)
        {
            try
            {
                var dataUpdate = JsonUtils.FromJsonRaw(data);

                var record = JavaScriptDomainConverter<F_F_BaseTermination>.DeserializeSingle(data);
                if (record.EarlyTerminat == string.Empty)
                {
                    return new RestResult
                        {
                            Success = false, 
                            Message = "Запись пуста"
                        };
                }

                var recordDataUpdate = baseTermination.FindOne(id);

                recordDataUpdate.EarlyTerminat = Convert.ToString(dataUpdate["EarlyTerminat"]);

                logService.WriteChangeDocDetail(recordDataUpdate.RefFactGZ);

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult GroundsForTerminationDelete(int id, int masterId)
        {
            return newRestService.DeleteDocDetailAction<F_F_BaseTermination>(id, masterId);
        }

        #endregion

        #region NPARegulatesService
        
        public RestResult NpaRegulatesServiceRead(int masterId)
        {
            var data = from p in npaRenderOrder.FindAll()
                       where p.RefFactGZ.ID == masterId
                       select new
                           {
                               p.ID, 
                               p.RenderEnact, 
                               p.DateNpa, 
                               p.TypeNpa, 
                               p.NumberNpa
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult NpaRegulatesServiceSave(string data, int masterId)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = NpaRegulatesServiceValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                F_F_NPARenderOrder record = JavaScriptDomainConverter<F_F_NPARenderOrder>.DeserializeSingle(data);

                string msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefFactGZ = gosZadanie.FindOne(masterId);
                record.RenderEnact = record.RenderEnact.Trim();
                record.TypeNpa = record.TypeNpa.Trim();
                record.NumberNpa = record.NumberNpa.Trim();

                npaRenderOrder.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ.RefParametr);

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = from p in npaRenderOrder.FindAll()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID, 
                                       p.RenderEnact, 
                                       p.DateNpa, 
                                       p.TypeNpa, 
                                       p.NumberNpa
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult NpaRegulatesServiceDelete(int id, int docId)
        {
            return newRestService.DeleteDocDetailAction<F_F_NPARenderOrder>(id, docId);
        }
        
        #endregion

        #region LimitValuesOfPrices

        public RestResult LimitValuesOfPricesRead(int masterId)
        {
            var data = from p in limitPrice.FindAll()
                       where p.RefFactGZ.ID == masterId
                       select new
                           {
                               p.ID, 
                               p.Name, 
                               p.Price
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult LimitValuesOfPricesCreate(string data, int masterId)
        {
            try
            {
                F_F_LimitPrice record = JavaScriptDomainConverter<F_F_LimitPrice>.DeserializeSingle(data);

                record.ID = 0;
                record.RefFactGZ = gosZadanie.FindOne(masterId);

                string validationError = LimitValuesOfPricesValidateData(record);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                limitPrice.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ.RefParametr);

                return new RestResult
                    {
                        Success = true, 
                        Message = "Новая запись добавлена", 
                        Data = from p in limitPrice.FindAll()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID, 
                                       p.Name, 
                                       p.Price
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction]
        public virtual RestResult LimitValuesOfPricesUpdate(int id, string data)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                F_F_LimitPrice recordDataUpdate = limitPrice.FindOne(id);

                recordDataUpdate.Name = Convert.ToString(dataUpdate["Name"]);
                recordDataUpdate.Price = Convert.ToString(dataUpdate["Price"]);
                logService.WriteChangeDocDetail(recordDataUpdate.RefFactGZ.RefParametr);

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult LimitValuesOfPricesDelete(int id, int docId)
        {
            return newRestService.DeleteDocDetailAction<F_F_LimitPrice>(id, docId);
        }
        
        #endregion

        #region InformConsumers

        public RestResult InformConsumersRead(int masterId)
        {
            var data = from p in infoProcedure.FindAll()
                       where p.RefFactGZ.ID == masterId
                       select new
                           {
                               p.ID, 
                               p.Content, 
                               p.Method, 
                               p.Rate
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult InformConsumersSave(string data, int masterId)
        {
            try
            {
                JsonObject dataUpdate = JsonUtils.FromJsonRaw(data);

                string validationError = InformConsumersValidateData(dataUpdate);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                F_F_InfoProcedure record = JavaScriptDomainConverter<F_F_InfoProcedure>.DeserializeSingle(data);

                string msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                record.RefFactGZ = gosZadanie.FindOne(masterId);
                record.Content = record.Content.Trim();
                record.Method = record.Method.Trim();
                record.Rate = record.Rate.Trim();

                infoProcedure.Save(record);
                logService.WriteChangeDocDetail(record.RefFactGZ.RefParametr);

                return new RestResult
                    {
                        Success = true, 
                        Message = msg, 
                        Data = from p in infoProcedure.FindAll()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID, 
                                       p.Content, 
                                       p.Method, 
                                       p.Rate
                                   }
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult InformConsumersDelete(int id, int docId)
        {
            return newRestService.DeleteDocDetailAction<F_F_InfoProcedure>(id, docId);
        }

        #endregion

        public ActionResult ExportToXml(int recId)
        {
            return File(
                ExportStateTaskService.Serialize(auth, headers.Load(recId)), 
                "application/xml", 
                "stateTask" + DateTime.Now.ToString("yyyymmddhhmmss") + ".xml");
        }

        private string MonitoringExecutionValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = string.Empty;

            if (Convert.ToString(record["Form"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Форма контроля");
            }

            if (Convert.ToString(record["Rate"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Периодичность");
            }

            if (Convert.ToString(record["Supervisor"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Орган исполнительной власти, осуществляющий контроль за оказанием услуги");
            }

            return message;
        }

        private string InformConsumersValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = string.Empty;

            if (Convert.ToString(record["Content"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Состав размещаемой информации");
            }

            if (Convert.ToString(record["Method"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Способ информирования");
            }

            if (Convert.ToString(record["Rate"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Частота обновления информации");
            }

            return message;
        }

        private string LimitValuesOfPricesValidateData(F_F_LimitPrice record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = string.Empty;

            if (record.Name.IsNullOrEmpty())
            {
                message += Msg.FormatWith("Наименование элемента услуги");
            }

            if (record.Price.IsNullOrEmpty())
            {
                message += Msg.FormatWith("Цена (тариф)");
            }

            return message;
        }

        private string NpaRegulatesServiceValidateData(JsonObject record)
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";
            const string Msg1 = "Дата НПА не должна быть больше текущей даты<br>";

            var message = string.Empty;

            if (Convert.ToString(record["RenderEnact"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Наименование НПА");
            }

            if (Convert.ToString(record["DateNpa"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Дата НПА");
            }
            else
            {
                if (Convert.ToDateTime(record["DateNpa"]) > DateTime.Now)
                {
                    message += Msg1;
                }
            }

            if (Convert.ToString(record["TypeNpa"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Вид НПА");
            }

            if (Convert.ToString(record["NumberNpa"]).Trim().IsNullOrEmpty())
            {
                message += Msg.FormatWith("Номер НПА");
            }

            return message;
        }
    }
}
