using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Common;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders;
using Krista.FM.RIA.Extensions.DebtBook.Services.ControlRelationships;
using Krista.FM.RIA.Extensions.DebtBook.Services.DAL;
using Krista.FM.ServerLibrary;
using XControl = Krista.FM.RIA.Core.Gui.XControl;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.Controllers
{
    public class BebtBookDataController : SchemeBoundController
    {
        private readonly IScheme scheme;
        private readonly IEntityDataService dataService;
        private readonly IDebtBookExtension bebtBookExtension;
        private readonly Core.ExtensionModule.Services.ViewService viewService;
        private readonly IParametersService parametersService;
        private readonly IChangesCalcService changesCalcService;
        private readonly IObjectRepository objectRepository;

        public BebtBookDataController(
            IScheme scheme, 
            IEntityDataService dataService, 
            IDebtBookExtension bebtBookExtension,
            Core.ExtensionModule.Services.ViewService viewService,
            IParametersService parametersService,
            IChangesCalcService changesCalcService,
            IObjectRepository objectRepository)
        {
            this.scheme = scheme;
            this.dataService = dataService;
            this.bebtBookExtension = bebtBookExtension;
            this.viewService = viewService;
            this.parametersService = parametersService;
            this.changesCalcService = changesCalcService;
            this.objectRepository = objectRepository;
        }

        public AjaxStoreResult GetRecord(string objectKey, int rid)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(objectKey);
            if (entity == null)
            {
                return new AjaxStoreResult(null, 0);
            }

            const string Filter = "(ID = ?)";
            IDbDataParameter[] prms = new IDbDataParameter[1];
            prms[0] = new DbParameterDescriptor("ID", rid, DbType.String);

            if (rid == -1)
            {
                var ajaxStoreResult = dataService.GetData(entity, 0, 1, String.Empty, String.Empty, Filter, prms, new DefaultValuesQueryBuilder());

                InitNewRow(ajaxStoreResult);

                return ajaxStoreResult;
            }

            return dataService.GetData(entity, 0, 1, String.Empty, String.Empty, Filter, prms);
        }

        public ActionResult Recalc(int documentId, string viewId)
        {
            if (RecalcNeed(viewId))
            {
                var view = viewService.GetView(viewId);
                XControl control = new XBuilderFactory(Scheme, parametersService)
                                         .Create(view);
                Control component = control.Create();

                if (component is BebtBookFormView)
                {
                    BebtBookFormView form = (BebtBookFormView)component;
                    changesCalcService.Recalc(form.Entity, documentId, form.Fields);
                }
                else
                {
                    throw new Exception("Неизвестный тип компонента");
                }
            }

            return new AjaxResult();
        }
        
        // Проверка контрольных соотношений при сохранении карточки.
        public ActionResult CheckControlRelationships(int documentId, string viewId)
        {
            var result = new AjaxResult();

            var view = viewService.GetView(viewId);
            XControl control = new XBuilderFactory(Scheme, parametersService)
                                     .Create(view);
            
            var component = control.Create();

            IList<ControlRelationship> controlRelatioships = new List<ControlRelationship>();
            string entityDbName = null;
            if (component is BebtBookGridView)
            {
                var cmp = (BebtBookGridView)component;
                controlRelatioships = cmp.ControlRelationships;
                entityDbName = cmp.Entity.FullDBName;
            }
            else if (component is BebtBookFormView)
            {
                var cmp = (BebtBookFormView)component;
                controlRelatioships = cmp.ControlRelationships;
                entityDbName = cmp.Entity.FullDBName;
            }

            if (controlRelatioships.Count > 0)
            {
                // Строка, которую тестируем по всем контрольным соотношениям
                object row = objectRepository.GetRow(entityDbName, documentId);

                var resultMessage = new StringBuilder();

                // Проверяем контрольные соотношения.
                foreach (var controlRelatioship in controlRelatioships)
                {
                    var isCorrect = controlRelatioship.Check(row, objectRepository).IsCorrect;
                    if (!isCorrect)
                    {
                        resultMessage.AppendLine(String.Format("- {0}<br>", controlRelatioship.GetResultMessage()));
                    }
                }

                // Формируем окно с результатом проверки всех соотношений
                if (resultMessage.IsNullOrEmpty())
                {
                    var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                                                                  {
                                                                      Title = "Результат проверки контрольных соотношений.",
                                                                      Html = "Ок. Контрольные соотношения выполнены.",
                                                                      HideDelay = 2000
                                                                  });
                    result.Script = message.ToScript();
                }
                else
                {
                    var errorWnd = new ErrorWindow
                    {
                        Title = "Результат проверки контрольных соотношений.",
                        Text = resultMessage.ToString()
                    };    
                    result.Script = errorWnd.Build(new ViewPage())[0].ToScript();
                }
            }

            return result;
        }

        public ActionResult CheckControlRelationshipsForAll(string viewId, string tabId, string serverFilter, int? variantID, int? sourceID)
        {
            var result = new AjaxResult();

            var view = viewService.GetView(viewId);
            XControl control = new XBuilderFactory(Scheme, parametersService)
                                     .Create(view);

            var component = control.Create();
            var tabbedView = (TabbedView)component;
            var tab = tabbedView.Tabs.Find(x => x.Id == tabId);
            
            IList<ControlRelationship> controlRelatioships = new List<ControlRelationship>();
            string entityDbName = null;
            if (tab is BebtBookGridView)
            {
                var cmp = (BebtBookGridView)tab;
                controlRelatioships = cmp.ControlRelationships;
                entityDbName = cmp.Entity.FullDBName;
            }
            
            if (controlRelatioships.Count > 0)
            {
                // Строки, которые тестируем по всем контрольным соотношениям
                IList<object> rows = objectRepository.GetRows(entityDbName, serverFilter, variantID, sourceID);

                var resultMessage = new StringBuilder();
                
                foreach (var row in rows)
                {
                    // Проверяем контрольные соотношения для данной стоки.
                    var subresult = new StringBuilder();
                    foreach (var controlRelatioship in controlRelatioships)
                    {
                        var isCorrect = controlRelatioship.Check(row, objectRepository).IsCorrect;
                        if (!isCorrect)
                        {
                            subresult.AppendFormat("   - {0}\n", controlRelatioship.GetResultMessage());
                        }
                    }

                    if (subresult.IsNotNullOrEmpty())
                    {
                        resultMessage.AppendFormat("• {0}\n", GetDocumentNum(row));
                        resultMessage.AppendFormat("{0}\n", subresult);
                    }
                }

                // Формируем окно с результатом проверки всех соотношений
                if (resultMessage.IsNullOrEmpty())
                {
                    var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                    {
                        Title = "Результат проверки контрольных соотношений.",
                        Html = "Ок. Все контрольные соотношения по всем договорам выполнены.",
                        HideDelay = 2000
                    });
                    result.Script = message.ToScript();
                }
                else
                {
                    var errorWnd = new ErrorWindow
                    {
                        Title = "Результат проверки контрольных соотношений.",
                        Text = resultMessage.ToString().Replace("\n", "<br/>")
                    };
                    result.Script = errorWnd.Build(new ViewPage())[0].ToScript();
                }
            }

            return result;
        }

        private string GetDocumentNum(object obj)
        {
            var property = obj.GetType().GetProperty("Num");
            if (property == null)
            {
                property = obj.GetType().GetProperty("OfficialNumber");
            }

            var val = property.GetValue(obj, null);
            return Convert.ToString(val);
        }

        /// <summary>
        /// Необходимость делать пересчет для отдельных представлений
        /// </summary>
        private bool RecalcNeed(string viewId)
        {
            switch (viewId)
            {
                case "CapitalFormSubject":
                case "CapitalFormRegion":
                case "CapitalFormSetl":

                case "OrganizationCreditFormSubject":
                case "OrganizationCreditFormRegion":
                case "OrganizationCreditFormSetl":

                case "BudgetCreditFormSubject":
                case "BudgetCreditFormRegion":
                case "BudgetCreditFormSetl":

                case "OtherCreditFormSubject":
                case "OtherCreditFormRegion":
                case "OtherCreditFormSetl":

                case "GuaranteeFormSubject":
                case "GuaranteeFormRegion":
                case "GuaranteeFormSetl":
                    return true;
                
                default:
                    return false;
            }
        }

        private void InitNewRow(AjaxStoreResult ajaxStoreResult)
        {
            var data = (DataTable)ajaxStoreResult.Data;

            // Устанавливаем значения по умолчанию для новой записи
            data.Columns["REFREGION"].ReadOnly = false;
            data.Columns["REFREGION"].AllowDBNull = true;
            data.Rows[0]["REFREGION"] = DBNull.Value;
            data.Columns["LP_REFREGION"].ReadOnly = false;
            data.Columns["LP_REFREGION"].AllowDBNull = true;
            data.Rows[0]["LP_REFREGION"] = DBNull.Value;

            data.Columns["REFVARIANT"].ReadOnly = false;
            if (data.Columns["REFVARIANT"].MaxLength != -1)
            {
                data.Columns["REFVARIANT"].MaxLength = 10;
            }

            data.Rows[0]["REFVARIANT"] = bebtBookExtension.Variant.Id;
            data.Columns["SOURCEID"].ReadOnly = false;
            if (data.Columns["SOURCEID"].MaxLength != -1)
            {
                data.Columns["SOURCEID"].MaxLength = 10;
            }

            data.Rows[0]["SOURCEID"] = bebtBookExtension.CurrentSourceId;
        }
    }
}
