using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

using Ext.Net;
using Ext.Net.MVC;

using Krista.Diagnostics;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для представления описания систем переходов состояний
    /// </summary>
    public class StateSystemViewController : SchemeBoundController
    {
        private readonly IStateSystemService stateSystemService;

        public StateSystemViewController()
        {
            stateSystemService = Resolver.Get<IStateSystemService>();
        }

        #region SchemStateTransitions

        public RestResult SchemTransitionsRead()
        {
            var data = from p in stateSystemService.GetItems<D_State_SchemTransitions>()
                       select new
                           {
                               p.ID,
                               p.InitAction,
                               p.Name,
                               p.Note,
                               RefPartDoc = p.RefPartDoc.ID,
                               RefPartDocName = p.RefPartDoc.Name
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult SchemTransitionsCreate(string data)
        {
            try
            {
                D_State_SchemTransitions record =
                    JavaScriptDomainConverter<D_State_SchemTransitions>.DeserializeSingle(data);

                record.ID = 0;
                record.RefPartDoc = stateSystemService.GetItem<FX_FX_PartDoc>(record.RefPartDoc.ID);
                stateSystemService.Save(record);

                return new RestResult
                    {
                        Success = true,
                        Message = "Новая запись добавлена",
                        Data = from p in stateSystemService.GetItems<D_State_SchemTransitions>()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID,
                                       p.InitAction,
                                       p.Name,
                                       p.Note,
                                       RefPartDoc = p.RefPartDoc.ID,
                                       RefPartDocName = p.RefPartDoc.Name
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
        public virtual RestResult SchemTransitionsUpdate(int id, string data)
        {
            try
            {
                JsonObject jsonObject = JsonUtils.FromJsonRaw(data);

                D_State_SchemTransitions recordDataUpdate = stateSystemService.GetSchemStateTransitions(id);

                recordDataUpdate.RefPartDoc =
                    stateSystemService.GetItem<FX_FX_PartDoc>(Convert.ToInt32(jsonObject["RefPartDoc"]));
                recordDataUpdate.InitAction = Convert.ToString(jsonObject["InitAction"]);
                recordDataUpdate.Name = Convert.ToString(jsonObject["Name"]);
                recordDataUpdate.Note = Convert.ToString(jsonObject["Note"]);

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult SchemTransitionsDelete(int id)
        {
            try
            {
                stateSystemService.BeginTransaction();

                stateSystemService.GetItems<D_State_RightsTransition>()
                    .Where(x => x.RefOptionsTransition.RefSchemStates.RefSchemStateTransitions.ID == id)
                    .Each(x => stateSystemService.Delete<D_State_RightsTransition>(x.ID));

                stateSystemService.GetItems<D_State_OptionsTransition>()
                    .Where(x => x.RefSchemStates.RefSchemStateTransitions.ID == id)
                    .Each(x => stateSystemService.Delete<D_State_OptionsTransition>(x.ID));

                stateSystemService.GetTransitions(id).Each(x => stateSystemService.Delete<D_State_Transitions>(x.ID));

                stateSystemService.GetItems<D_State_SchemStates>()
                    .Where(x => x.RefSchemStateTransitions.ID == id)
                    .Each(x => stateSystemService.Delete<D_State_SchemStates>(x.ID));

                D_State_SchemTransitions record = stateSystemService.GetSchemStateTransitions(id);
                stateSystemService.Delete<D_State_SchemTransitions>(record.ID);

                if (stateSystemService.HaveTransaction)
                {
                    stateSystemService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("SchemTransitionsDelete: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));

                if (stateSystemService.HaveTransaction)
                {
                    stateSystemService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }

        #endregion

        #region States

        public RestResult StatesRead(int masterId)
        {
            var data = from p in stateSystemService.GetItems<D_State_SchemStates>()
                       where p.RefSchemStateTransitions.ID == masterId
                       select new
                           {
                               p.ID,
                               RefStates = p.RefStates.ID,
                               RefStatesName = p.RefStates.Name,
                               p.IsStart
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult StatesCreate(string data, int masterId)
        {
            try
            {
                D_State_SchemStates record = JavaScriptDomainConverter<D_State_SchemStates>
                                                .DeserializeSingle(data);

                record.ID = 0;
                record.RefSchemStateTransitions = stateSystemService.GetSchemStateTransitions(masterId);
                record.RefStates = stateSystemService.GetState(record.RefStates.ID);

                string validationError = StatesValidateData(record, record.IsStart != null && record.IsStart.Value);
                if (!string.IsNullOrEmpty(validationError))
                {
                    throw new InvalidDataException(validationError);
                }

                stateSystemService.Save(record);

                return new RestResult
                    {
                        Success = true,
                        Message = "Новая запись добавлена",
                        Data = from p in stateSystemService.GetItems<D_State_SchemStates>()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID,
                                       RefStates = p.RefStates.ID,
                                       RefStatesName = p.RefStates.Name,
                                       p.IsStart
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
        public virtual RestResult StatesUpdate(int id, string data)
        {
            try
            {
                JsonObject jsonObject = JsonUtils.FromJsonRaw(data);

                var recordDataUpdate = stateSystemService.GetItem<D_State_SchemStates>(id);

                bool isStart = Convert.ToBoolean(jsonObject["IsStart"]);

                string validationError = StatesValidateData(recordDataUpdate, isStart);
                if (!string.IsNullOrEmpty(validationError))
                {
                    throw new InvalidDataException(validationError);
                }

                recordDataUpdate.RefStates = stateSystemService
                                      .GetState(Convert.ToInt32(jsonObject["RefStates"]));
                recordDataUpdate.IsStart = isStart;

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult StatesDelete(int id)
        {
            try
            {
                stateSystemService.BeginTransaction();
                
                stateSystemService.GetItems<D_State_RightsTransition>()
                    .Where(x => x.RefOptionsTransition.RefSchemStates.ID == id)
                    .Each(x => stateSystemService.Delete<D_State_RightsTransition>(x.ID));

                stateSystemService.GetItems<D_State_OptionsTransition>()
                    .Where(x => x.RefSchemStates.ID == id)
                    .Each(x => stateSystemService.Delete<D_State_OptionsTransition>(x.ID));

                var record = stateSystemService.GetItem<D_State_SchemStates>(id);

                stateSystemService.GetTransitions(record.RefSchemStateTransitions.ID)
                    .Where(x => x.RefSchemStates.ID == id)
                    .Each(x => stateSystemService.Delete<D_State_Transitions>(x.ID));

                stateSystemService.Delete<D_State_SchemStates>(record.ID);

                if (stateSystemService.HaveTransaction)
                {
                    stateSystemService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("StatesDelete: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));

                if (stateSystemService.HaveTransaction)
                {
                    stateSystemService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }

        #endregion

        #region Transitions

        public RestResult TransitionsRead(int masterId)
        {
            var data = from p in stateSystemService.GetItems<D_State_Transitions>()
                       where p.RefSchemStateTransitions.ID == masterId
                       select new
                           {
                               p.ID,
                               p.Name,
                               p.Action,
                               p.Ico,
                               p.Note,
                               RefSchemStates = p.RefSchemStates.ID,
                               RefSchemStatesName = p.RefSchemStates.RefStates.Name,
                               p.TransitionClass,
                               TransitionClassName = p.TransitionClass == "Base"
                                                         ? "Базовый"
                                                         : p.TransitionClass == "Export"
                                                               ? "Экспорт"
                                                               : "С вызовом диалога"
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult TransitionsCreate(string data, int masterId)
        {
            try
            {
                D_State_Transitions record = JavaScriptDomainConverter<D_State_Transitions>
                                               .DeserializeSingle(data);

                record.ID = 0;
                record.RefSchemStateTransitions = stateSystemService.GetSchemStateTransitions(masterId);
                record.RefSchemStates = stateSystemService
                                              .GetItem<D_State_SchemStates>(record.RefSchemStates.ID);

                stateSystemService.Save(record);

                return new RestResult
                    {
                        Success = true,
                        Message = "Новая запись добавлена",
                        Data = from p in stateSystemService.GetItems<D_State_Transitions>()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID,
                                       p.Name,
                                       p.Action,
                                       p.Ico,
                                       p.Note,
                                       RefSchemStates = p.RefSchemStates.ID,
                                       RefSchemStatesName = p.RefSchemStates.RefStates.Name,
                                       p.TransitionClass,
                                       TransitionClassName = p.TransitionClass == "Base"
                                                                 ? "Базовый"
                                                                 : p.TransitionClass == "Export"
                                                                       ? "Экспорт"
                                                                       : "С вызовом диалога"
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
        public virtual RestResult TransitionsUpdate(int id, string data)
        {
            try
            {
                JsonObject jsonObject = JsonUtils.FromJsonRaw(data);

                var recordDataUpdate = stateSystemService.GetItem<D_State_Transitions>(id);

                recordDataUpdate.TransitionClass = Convert.ToString(jsonObject["TransitionClass"]);
                recordDataUpdate.Ico = Convert.ToString(jsonObject["Ico"]);
                recordDataUpdate.Name = Convert.ToString(jsonObject["Name"]);
                recordDataUpdate.Note = Convert.ToString(jsonObject["Note"]);
                recordDataUpdate.Action = Convert.ToString(jsonObject["Action"]);
                recordDataUpdate.RefSchemStates =
                    stateSystemService.GetItem<D_State_SchemStates>(Convert
                                                     .ToInt32(jsonObject["RefSchemStates"]));

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult TransitionsDelete(int id)
        {
            try
            {
                stateSystemService.BeginTransaction();

                stateSystemService.GetItems<D_State_RightsTransition>()
                    .Where(x => x.RefOptionsTransition.RefTransitions.ID == id)
                    .Each(x => stateSystemService.Delete<D_State_RightsTransition>(x.ID));

                stateSystemService.GetItems<D_State_OptionsTransition>()
                    .Where(x => x.RefTransitions.ID == id)
                    .Each(x => stateSystemService.Delete<D_State_OptionsTransition>(x.ID));

                var record = stateSystemService.GetItem<D_State_Transitions>(id);
                stateSystemService.Delete<D_State_Transitions>(record.ID);

                if (stateSystemService.HaveTransaction)
                {
                    stateSystemService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteDocDetailAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));

                if (stateSystemService.HaveTransaction)
                {
                    stateSystemService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public RestResult GetStatesLp(int masterId)
        {
            var data = from p in stateSystemService.GetItems<D_State_SchemStates>()
                       where p.RefSchemStateTransitions.ID == masterId
                       select new
                           {
                               p.ID,
                               p.RefStates.Name
                           };

            return new RestResult { Success = true, Data = data };
        }

        #endregion

        #region RightsTransition

        public RestResult RightsTransitionRead(int masterId)
        {
            var data = from p in stateSystemService.GetItems<D_State_RightsTransition>()
                       where p.RefOptionsTransition.ID == masterId
                       select new
                           {
                               p.ID,
                               p.AccountsRole,
                               AccountsRoleName = p.AccountsRole == "Admin"
                                                      ? "Администратор"
                                                      : p.AccountsRole == "GRBS"
                                                            ? "ГРБС"
                                                            : p.AccountsRole == "PPO" ? "ППО" : "Учреждение"
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult RightsTransitionCreate(string data, int masterId)
        {
            try
            {
                D_State_RightsTransition record =
                    JavaScriptDomainConverter<D_State_RightsTransition>.DeserializeSingle(data);

                record.ID = 0;
                record.RefOptionsTransition = stateSystemService.GetItem<D_State_OptionsTransition>(masterId);

                stateSystemService.Save(record);

                return new RestResult
                    {
                        Success = true,
                        Message = "Новая запись добавлена",
                        Data = from p in stateSystemService.GetItems<D_State_RightsTransition>()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID,
                                       p.AccountsRole,
                                       AccountsRoleName = p.AccountsRole == "Admin"
                                                              ? "Администратор"
                                                              : p.AccountsRole == "GRBS"
                                                                    ? "ГРБС"
                                                                    : p.AccountsRole == "PPO" ? "ППО" : "Учреждение"
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
        public virtual RestResult RightsTransitionUpdate(int id, string data)
        {
            try
            {
                JsonObject jsonObject = JsonUtils.FromJsonRaw(data);

                var recordDataUpdate = stateSystemService.GetItem<D_State_RightsTransition>(id);

                recordDataUpdate.AccountsRole = Convert.ToString(jsonObject["AccountsRole"]);

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult RightsTransitionDelete(int id)
        {
            return stateSystemService.DeleteAction<D_State_RightsTransition>(id);
        }

        #endregion

        #region OptionsTransition

        public RestResult OptionsTransitionRead(int masterId)
        {
            var data = from p in stateSystemService.GetItems<D_State_OptionsTransition>()
                       where p.RefSchemStates.ID == masterId
                       select new
                           {
                               p.ID,
                               RefTransitions = p.RefTransitions.ID,
                               RefTransitionsName = p.RefTransitions.Name
                           };

            return new RestResult { Success = true, Data = data };
        }

        [HttpPost]
        [Transaction]
        public RestResult OptionsTransitionCreate(string data, int masterId)
        {
            try
            {
                D_State_OptionsTransition record =
                    JavaScriptDomainConverter<D_State_OptionsTransition>.DeserializeSingle(data);

                record.ID = 0;
                record.RefSchemStates = stateSystemService.GetItem<D_State_SchemStates>(masterId);
                record.RefTransitions = stateSystemService
                        .GetItem<D_State_Transitions>(record.RefTransitions.ID);

                stateSystemService.Save(record);

                return new RestResult
                    {
                        Success = true,
                        Message = "Новая запись добавлена",
                        Data = from p in stateSystemService.GetItems<D_State_OptionsTransition>()
                               where p.ID == record.ID
                               select new
                                   {
                                       p.ID,
                                       RefTransitions = p.RefTransitions.ID,
                                       RefTransitionsName = p.RefTransitions.Name
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
        public virtual RestResult OptionsTransitionUpdate(int id, string data)
        {
            try
            {
                JsonObject jsonObject = JsonUtils.FromJsonRaw(data);

                var recordDataUpdate = stateSystemService.GetItem<D_State_OptionsTransition>(id);

                recordDataUpdate.RefTransitions = stateSystemService
                    .GetItem<D_State_Transitions>(Convert.ToInt32(jsonObject["RefTransitions"]));

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        public virtual RestResult OptionsTransitionDelete(int id)
        {
            return stateSystemService.DeleteAction<D_State_OptionsTransition>(id);
        }

        public RestResult TransitionLpStoreRead(int masterId)
        {
            var data = from p in stateSystemService.GetItems<D_State_Transitions>()
                       where p.RefSchemStateTransitions.ID == masterId
                       select new
                           {
                               p.ID,
                               p.Name
                           };

            return new RestResult { Success = true, Data = data };
        }

        #endregion

        #region Export/Import

        public ActionResult Export(int recId)
        {
            Stream stream = stateSystemService.Export(recId);
            string fileName = "StateSystem_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + recId + ".xml";
            return File(stream, "application/xml", fileName);
        }

        [HttpPost]
        [Transaction]
        public AjaxFormResult Import(string fileName)
        {
            var result = new AjaxFormResult();

            try
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file == null || file.ContentLength == 0)
                {
                    throw new ArgumentNullException("Файл" + fileName + " пустой!");
                }

                var xmlfilereader = new XmlTextReader(file.InputStream);
                stateSystemService.Import(xmlfilereader);

                result.Success = true;
                result.ExtraParams["msg"] = "Файл '{0}' успешно загружен.".FormatWith(fileName);
                result.ExtraParams["name"] = fileName;
                result.IsUpload = true;
                return result;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ExtraParams["msg"] = "Ошибка загрузки файла.";
                result.ExtraParams["responseText"] = e.Message;
                result.IsUpload = true;
                return result;
            }
        }

        #endregion

        private string StatesValidateData(D_State_SchemStates record, bool isStart)
        {
            string message = string.Empty;

            if (isStart
                && (stateSystemService.GetItems<D_State_SchemStates>()
                        .Count(x => (x.RefSchemStateTransitions.ID == record.RefSchemStateTransitions.ID) && (x.IsStart == true) && (x.ID != record.ID)) != 0))
            {
                message += "Нельзя задать два начальных состояния. <br/>";
            }

            return message;
        }
    }
}