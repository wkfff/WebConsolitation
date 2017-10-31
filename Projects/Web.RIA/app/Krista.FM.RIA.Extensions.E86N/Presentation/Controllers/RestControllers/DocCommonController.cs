using System;
using System.IO;
using System.Reflection;
using System.Web.Mvc;

using Ext.Net.MVC;

using Krista.Diagnostics;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers
{
    /// <summary>
    /// Общий контроллер для grid ов работающих с моделями
    /// </summary>
    public class DocCommonController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly IChangeLogService logService;

        public DocCommonController(INewRestService newRestService, IChangeLogService logService)
        {
            this.newRestService = newRestService;
            this.logService = logService;
        }

        /// <summary>
        /// Чтение данных для грида
        /// </summary>
        public RestResult ReadAction(string modelType)
        {
            try
            {
                var type = Type.GetType(modelType);
                var model = Resolver.Get(type);
                var methodInfo = model.GetType().GetMethod("GetModelData");
                return new RestResult { Success = true, Data = methodInfo.Invoke(model, new object[] { Request.QueryString }) };
            }
            catch (Exception e)
            {
                Trace.TraceError("ReadAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Добавление или обновление записи грида
        /// </summary>
        [HttpPost]
        public RestResult SaveAction(string data, int? docId, string modelType)
        {
            try
            {
                newRestService.BeginTransaction();

                var type = Type.GetType(modelType);
                var model = Resolver.Get(type);
                MethodInfo methodInfo = type.GetMethod("Serialize");
                var dataModel = methodInfo.Invoke(model, new object[] { data });
                
                methodInfo = dataModel.GetType().GetMethod("ValidateData", Type.EmptyTypes);
                var validationError = (string)methodInfo.Invoke(dataModel, null);
                if (validationError.IsNotNullOrEmpty())
                {
                    throw new InvalidDataException(validationError);
                }

                methodInfo = dataModel.GetType().GetMethod("GetDomainByModel", Type.EmptyTypes);
                var record = (DomainObject)methodInfo.Invoke(dataModel, null);

                var msg = "Запись обновлена";

                if (record.ID < 0)
                {
                    record.ID = 0;
                    msg = "Новая запись добавлена";
                }

                newRestService.Save(record);
                if (docId.HasValue)
                {
                    logService.WriteChangeDocDetail(newRestService.GetItem<F_F_ParameterDoc>(docId));
                }
                
                methodInfo = dataModel.GetType().GetMethod("GetModelByDomain");
                var recData = methodInfo.Invoke(dataModel, new object[] { record });

                if (newRestService.HaveTransaction)
                {
                    newRestService.CommitTransaction();
                }

                return new RestResult
                {
                    Success = true,
                    Message = msg,
                    Data = recData
                };
            }
            catch (Exception e)
            {
                if (!(e is InvalidDataException))
                {
                    Trace.TraceError("SaveAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));
                }

                if (newRestService.HaveTransaction)
                {
                    newRestService.RollbackTransaction();
                }
                
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Экшин удаления записи грида
        /// </summary>
        [HttpDelete]
        public RestResult DeleteAction(int id, int? docId, string modelType)
        {
            try
            {
                newRestService.BeginTransaction();

                var type = Type.GetType(modelType);
                if (type != null)
                {
                    var model = Resolver.Get(type);
                    var methodInfo = model.GetType().GetMethod("GetDomainByModel", Type.EmptyTypes);
                    var record = (DomainObject)methodInfo.Invoke(model, null);
                    record.ID = id;
                    newRestService.Delete(record);
                }
                
                if (docId.HasValue)
                {
                  logService.WriteDeleteDocDetail(newRestService.GetItem<F_F_ParameterDoc>(docId));
                }
                 
                // todo считаю что не нужно в IChangeLogService работать с транзакциями!!!
                if (newRestService.HaveTransaction)
                {
                    newRestService.CommitTransaction();
                }

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                Trace.TraceError("DeleteAction: " + e.Message + " : " + KristaDiagnostics.ExpandException(e));

                if (newRestService.HaveTransaction)
                {
                    newRestService.RollbackTransaction();
                }

                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}