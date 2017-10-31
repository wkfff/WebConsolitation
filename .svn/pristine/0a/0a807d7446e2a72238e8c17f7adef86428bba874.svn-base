using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Common.Constants;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Helpers;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.DebtBook.Presentation.Filters;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.Controllers
{
    public class BebtBookVariantController : SchemeBoundController
    {
        private readonly IDebtBookExtension extension;
        private readonly VariantCopyService variantCopyService;
        private readonly IEntityDataService dataService;
        private readonly VariantService variantService;

        public BebtBookVariantController(
                                          IDebtBookExtension extension, 
                                          VariantCopyService variantCopyService, 
                                          IEntityDataService dataService,
                                          VariantService variantService)
        {
            this.extension = extension;
            this.variantCopyService = variantCopyService;
            this.dataService = dataService;
            this.variantService = variantService;
        }

        /// <summary>
        /// Общий алгоритм копирования варианта в новый вариант.
        /// </summary>
        [Transaction]
        [ScriptResultMessage]
        public ActionResult Copy(int variantId)
        {
            var lockCopiedData = new Params.OKTMOValueProvider(Scheme).GetValue() == OKTMO.Yaroslavl;
            variantCopyService.Copy(variantId, extension.CurrentAnalysisSourceId, lockCopiedData);
            return null;
        }

        /// <summary>
        /// Для Вологды. Копирует данные субъекта либо данные района и его поселений 
        /// в выбранный вариант из варианта предыдущего месяца.
        /// </summary>
        [Transaction]
        [ScriptResultMessage]
        public ActionResult CopyVologda(int variantId)
        {
            if (extension.UserRegionType == UserRegionType.Subject)
            {
                variantCopyService.CopySubjectData(variantId, extension.CurrentAnalysisSourceId, extension.SubjectRegionId, false);
            }
            else
            {
                variantCopyService.CopyVariantData(variantId, extension.CurrentAnalysisSourceId, extension.UserRegionId, true, false);
            }

            return null;
        }

        public AjaxStoreResult Load()
        {
            return new AjaxStoreResult();
        }

        public ActionResult SetCurrentVariant(int variantId)
        {
            try
            {
                var variant = variantService.GetVariant(variantId);
                extension.SetCurrentVariant(variant);

                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Установка варианта.",
                    Html = "Вариант выбран успешно.",
                    HideDelay = 1000
                });
                
                return new AjaxResult { Script = message.ToScript() };
            }
            catch (Exception e)
            {
                var errorWnd = new ErrorWindow
                {
                    Title = "Установка варианта.",
                    Text = e.ToString()
                };

                return new AjaxResult { Script = errorWnd.Build(new ViewPage())[0].ToScript() }; 
            }
        }

        public ActionResult Save(string objectKey)
        {
            try
            {
                IEntity entity = Scheme.RootPackage.FindEntityByName(objectKey);
                if (entity == null)
                {
                    return new AjaxStoreResult(null, 0);
                }

                StoreDataHandler dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
                var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

                // TODO: проверка на отсутствие одновременной установки более 1-го текущего варианта (CURRENTVARIANT)
                if (dataSet.ContainsKey("Updated"))
                {
                    entity.CanEditRecord(Scheme.UsersManager, true);

                    var table = dataSet["Updated"];
                    dataService.Update(entity, table);
                }

                List<long> newIdList = null;
                if (dataSet.ContainsKey("Created"))
                {
                    entity.CanAddRecord(Scheme.UsersManager, true);

                    var table = dataSet["Created"];

                    /* Используется только для иерархических данных */
                    Dictionary<long, long> ids = new Dictionary<long, long>();
                    /* признак иерархических данных */
                    if (dataSet.ContainsKey("hierarchy"))
                    {
                        foreach (var item in table)
                        {
                            ids.Add((long)item["ID"], (long)item["ID"]);
                        }
                    }

                    newIdList = dataService.Create(entity, table, ids);
                }

                if (dataSet.ContainsKey("Deleted"))
                {
                    entity.CanDeleteRecord(Scheme.UsersManager, true);

                    var table = dataSet["Deleted"];
                    
                    // сначала удаляем подчиненные записи
                    foreach (var record in table)
                    {
                        int id = Convert.ToInt32(record["ID"]);
                        dataService.DeleteDependentData(entity, id);
                    }

                    dataService.Delete(entity, table);
                }

                var result = new AjaxStoreResult(StoreResponseFormat.Save);

                // TODO : сформировать список новых id-шников
                if (newIdList != null)
                {
                    result.SaveResponse.Message = String.Format("{{newId:{0}}}", newIdList.Last());
                }

                return result;
            }
            catch (Exception e)
            {
                AjaxStoreResult ajaxStoreResult = new AjaxStoreResult(StoreResponseFormat.Save);
                ajaxStoreResult.SaveResponse.Success = false;
                ajaxStoreResult.SaveResponse.Message = e.Message;
                return ajaxStoreResult;
            }
        }
    }
}
