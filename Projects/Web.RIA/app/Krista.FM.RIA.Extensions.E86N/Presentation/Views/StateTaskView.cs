using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.StateTaskModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTasks;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls.SearchCombobox;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Services.StateTaskService;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class StateTaskView : View
    {
        private const string ViewController = "StateTaskView";
        private const string Scope = "E86n.View.StateTask";

        private const string ExtHeaderID = "StateTaskExtAttrCbNotBring";

        private readonly IAuthService auth;
        private readonly INewRestService newRestService;

        private readonly ConsumerCategoryModel consumerCategoryModel = new ConsumerCategoryModel();
        private readonly ExtHeaderModel extHeaderModel = new ExtHeaderModel();

        public StateTaskView(IAuthService auth, INewRestService newRestService)
        {
            this.auth = auth;
            this.newRestService = newRestService;
        }

        public int? DocId
        {
            get { return Params["docId"] == "null" ? -1 : Convert.ToInt32(Params["docId"]); }
        }

        public ViewPage Page { get; set; }

        private List<Component> StateTaskDetails { get; set; }

        private List<Component> ReportingRequirementsDetails { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StateTaskView", Resource.StateTaskView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            RestActions restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;
            
            Toolbar tb = new NewStateToolBarControl(Convert.ToInt32(DocId.ToString())).BuildComponent(page);

            tb.Add(new VersioningControl(Convert.ToInt32(DocId.ToString()), "StateTask", Scope + ".CloseStateTaskDoc").Build(page));

            if (auth.IsAdmin())
            {
                var btn = new UpLoadFileBtnControl
                {
                    Id = "btnExport",
                    Name = "Экспорт в XML",
                    Icon = Icon.DiskDownload,
                    Upload = false,
                    UploadController = "/StateTaskView/ExportToXml",
                    Params = { { "recId", DocId.ToString() } }
                };

                tb.Add(btn.Build(Page));

                tb.Add(new ToolbarSeparator());

                tb.Add(new SetDocStateBtn(Convert.ToInt32(DocId.ToString())).Build(Page));
            }

            var btnExcel = new UpLoadFileBtnControl
            {
                Id = "btnExportToExcel",
                Name = "Экспорт в Excel",
                Icon = Icon.PageExcel,
                Upload = false,
                UploadController = "/Reports/GetStateTaskForm",
                Params = { { "docId", DocId.ToString() } }
            };

            tb.Add(btnExcel.Build(Page));

            var paramDocPanel = new ParamDocPanelControl(DocId ?? -1, tb);
            paramDocPanel.ParamDocStore.Listeners.Load.AddAfter("E86n.View.StateTask.updateYearHeader(records[0]);");

            ReportingRequirementsDetails = new List<Component>
                                               {
                                                   GetReportingRequirementsGrid(),
                                                   GetGroundsForTerminationGrid()
                                               };

            StateTaskDetails = new List<Component>
                                   {
                                       GetConsumerCategoryGrid(),
                                       GetIndicatorsOfServiceGrid(),
                                       GetRequisitesNpaGrid(),
                                       GetNpaRegulatesServiceGrid(),
                                       GetLimitValuesOfPricesGrid(),
                                       GetInformConsumersGrid()
                                   };

            var rowLayout = new RowLayout();

            if (newRestService.GetItem<F_F_ParameterDoc>(DocId).RefUchr.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID))
            {
                rowLayout.Rows.Add(new LayoutRow
                {
                    Items =
                                        {
                                            GetAttrFormPanel()
                                        }
                });
            }

            rowLayout.Rows.Add(new LayoutRow
                               {
                                    RowHeight = 1,
                                    Items =
                                            {
                                                UiBuilders.GetTabbedDetails(
                                                        new List<Component>
                                                            {
                                                                GetDetailUi(),
                                                                GetMonitoringExecutionGrid(),
                                                                CreateReportingRequirementsDetailTabPanel(),
                                                                new DocsDetailControl(DocId ?? -1).BuildComponent(page)
                                                            })
                                            }
                               });

            var view = new Viewport
                           {
                               ID = "vpStateTask",
                               Items =
                                   {
                                       new BorderLayout
                                           {
                                               North = { Items = { paramDocPanel.BuildComponent(page) } },
                                               Center =
                                                   {
                                                       Items =
                                                           {
                                                              new Panel
                                                                  {
                                                                    Border = false,
                                                                    Items =
                                                                        {
                                                                            rowLayout
                                                                        }
                                                                  }
                                                           }
                                                   }
                                           }
                                   }
                           };

            return new List<Component> { view };
        }

        private void GetExtHeaderStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                 Details.StateTask + "ExtAttrStore",
                 true,
                 typeof(StateTaskController),
                 "ExtHeaderRead");

            store.Listeners.DataChanged.Handler = Scope + ".DataChanged('{0}', store)".FormatWith(ExtHeaderID);

            store.AddFieldsByClass(extHeaderModel);
            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            Page.Controls.Add(store);
        }

        private Component GetAttrFormPanel()
        {
            GetExtHeaderStore();

            var panel = new Panel
                            {
                                ID = Details.StateTask + "ExtAttrPanel",
                                BodyCssClass = "x-window-mc",
                                CssClass = "x-window-mc",
                                Height = 30,
                                Padding = 6,
                                Border = false,
                                LabelPad = 90
                            };
            
            var cb = new Checkbox
            {
                ID = ExtHeaderID,
                FieldLabel = extHeaderModel.DescriptionOf(() => extHeaderModel.NotBring)
            };

            cb.Listeners.Check.Handler = Scope + ".Check(item, checked, '{0}', {1})".FormatWith(
                                                         UiBuilders.GetUrl<StateTaskController>(
                                                             "SetNotBring",
                                                             new Dictionary<string, object>
                                                                 {
                                                                     { "docId", DocId }
                                                                 }),
                                                             DocId);

            panel.Items.Add(cb);

            return panel;
        }

        private Panel GetDetailUi()
        {
            return new Panel
                       {
                           Title = StateTaskHelpers.DatailsNameMapping(Details.StateTask),
                           Border = false,
                           Items =
                               {
                                   new RowLayout
                                       {
                                           Split = true,
                                           Rows =
                                               {
                                                   new LayoutRow
                                                       {
                                                           RowHeight = 0.65m,
                                                           Items =
                                                               {
                                                                   CreateStateTaskGridPanel()
                                                               }
                                                       },
                                                   new LayoutRow
                                                       {
                                                           RowHeight = 0.35m,
                                                           Items =
                                                               {
                                                                   CreateDetailTabPanel()
                                                               }
                                                       }
                                               }
                                       }
                               }
                       };
        }

        private TabPanel CreateDetailTabPanel()
        {
            var tabPanel = UiBuilders.GetTabbedDetails(StateTaskDetails) as TabPanel;
            if (tabPanel != null)
            {
                tabPanel.ID = "DetailTabPanel";
                tabPanel.Disabled = true;
                tabPanel.Listeners.TabChange.Handler = "reloadDetail();";
                return tabPanel;
            }

            return null;
        }

        private TabPanel CreateReportingRequirementsDetailTabPanel()
        {
            var tabPanel = UiBuilders.GetTabbedDetails(ReportingRequirementsDetails) as TabPanel;
            if (tabPanel != null)
            {
                tabPanel.Title = StateTaskHelpers.DatailsNameMapping(Details.RequirementForReportingOnPerformance);
                return tabPanel;
            }

            return null;
        }

        private Store GetStateTaskStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                Details.StateTask + "Store",
                true,
                typeof(StateTaskController),
                "Read",
                "Save",
                "Save",
                "Destroy");
            
            store.AddField("ID");
            store.AddField("RefParametr");
            store.AddField("RazdelN");
            store.AddField("RefVedPch");
            store.AddField("RefVedPchOld");
            store.AddField("RefVedPchName");
            store.AddField("RefVedPchTip");
            store.AddField("RefVedPchTipID");
            store.AddField("RefVedPchCost");
            store.AddField("RefVedPchCostID");
            store.AddField("CenaEd");

            store.SetBaseParams("parentId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("parentId", DocId.ToString(), ParameterMode.Value);
            
            Page.Controls.Add(store);

            return store;
        }

        private Store GetConsumerCategoryStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                                                        Details.ConsumerCategory + "Store",
                                                        true,
                                                        typeof(ConsumerCategoryController),
                                                        "Read",
                                                        "Save",
                                                        "Save");
            store.AddFieldsByClass(consumerCategoryModel);
            
            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetRequisitesNpaStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                Details.RequisitesNpa + "Store",
                true,
                typeof(StateTaskTab3Controller));

            store.AddField("ID");
            store.AddField("Name");
            store.AddField("DataNPAGZ");
            store.AddField("NumNPA");
            store.AddField("OrgUtvDoc");
            store.AddField("VidNPAGZ");

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetIndicatorsOfServiceStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                Details.IndicatorsOfService + "Store",
                true,
                typeof(StateTaskTab2Controller),
                createActionName: "Save",
                updateActionName: "Save");

            store.AddFieldsByEnum(new IndicatorsOfServiceFields());
            
            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);
           
            return store;
        }

        private Store GetMonitoringExecutionStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
               Details.MonitoringExecution + "Store",
                true,
                ViewController,
                "MonitoringExecutionRead",
                "MonitoringExecutionSave",
                "MonitoringExecutionSave",
                "MonitoringExecutionDelete");
            store.AddField("ID");
            store.AddField("Form");
            store.AddField("Rate");
            store.AddField("Supervisor");

            store.SetBaseParams("masterId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("masterId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetReportingRequirementsStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                Details.ReportingRequirements + "Store",
                true,
                ViewController,
                "ReportingRequirementsRead",
                "ReportingRequirementsCreate",
                "ReportingRequirementsUpdate",
                "ReportingRequirementsDelete");

            store.AddFieldsByEnum(new ReportingRequirementsFields());

            store.SetBaseParams("masterId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("masterId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetGroundsForTerminationStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                Details.GroundsForTermination + "Store",
                true,
                ViewController,
                "GroundsForTerminationRead",
                "GroundsForTerminationCreate",
                "GroundsForTerminationUpdate",
                "GroundsForTerminationDelete");
            store.AddField("ID");
            store.AddField("EarlyTerminat");

            store.SetBaseParams("masterId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("masterId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetNpaRegulatesServiceStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                Details.NpaRegulatesService + "Store",
                true,
                ViewController,
                "NpaRegulatesServiceRead",
                "NpaRegulatesServiceSave",
                "NpaRegulatesServiceSave",
                "NpaRegulatesServiceDelete");
            store.AddField("ID");
            store.AddField("RenderEnact");
            store.AddField("TypeNpa");
            store.AddField("DateNpa");
            store.AddField("NumberNpa");

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetLimitValuesOfPricesStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                Details.LimitValuesOfPrices + "Store",
                true,
                ViewController,
                "LimitValuesOfPricesRead",
                "LimitValuesOfPricesCreate",
                "LimitValuesOfPricesUpdate",
                "LimitValuesOfPricesDelete");
            store.AddField("ID");
            store.AddField("Name");
            store.AddField("Price");

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetInformConsumersStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                Details.InformConsumers + "Store",
                true,
                ViewController,
                "InformConsumersRead",
                "InformConsumersSave",
                "InformConsumersSave",
                "InformConsumersDelete");
            store.AddField("ID");
            store.AddField("Method");
            store.AddField("Content");
            store.AddField("Rate");

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(Details.StateTask.ToString()), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private GridPanel CreateStateTaskGridPanel()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.StateTask.ToString(), GetStateTaskStore());

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            var referencerInfo = ComboboxEditorExtensions.BuildReferencerInfo(
                Resolver.Get<IScheme>().RootPackage.FindEntityByName(D_Services_VedPer.Key), new[] { "Name" });
            
            var action = UiBuilders.GetUrl<StateTaskController>("GetServices", new Dictionary<string, object> { { "docId", DocId } });

            gp.ColumnModel
                .AddColumn("RefVedPchName", "Наименование услуги", DataAttributeTypes.dtString)
                .SetWidth(200)
                .AddLookupEditorForColumnHardCode1(
                    "Name",
                    new Dictionary<string, string[]>
                        {
                            { "RefVedPch", new[] { "ID" } },
                            { "RefVedPchName", new[] { "Name" } },
                            { "RefVedPchTip", new[] { "TypeOfService" } },
                            { "RefVedPchTipID", new[] { "TypeOfServiceID" } },
                            { "RefVedPchCost", new[] { "PaymentForServices" } },
                            { "RefVedPchCostID", new[] { "PaymentForServicesCode" } }
                        },
                    referencerInfo,
                    action,
                    Page);

            gp.ColumnModel.AddColumn("RefVedPchTip", "Тип услуги", DataAttributeTypes.dtString).SetWidth(150).SetEditable(false);
            gp.ColumnModel.AddColumn("RefVedPchCost", "Платность", DataAttributeTypes.dtString).SetWidth(150).SetEditable(false);
            gp.ColumnModel.AddColumn("CenaEd", "Средневзвешенная цена за единицу услуги", DataAttributeTypes.dtDouble).SetNullable()
                .SetWidth(300).SetMaxLengthEdior(20).SetEditableDouble(2);

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Fn(Scope, "RowSelect");

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }
        
        private GridPanel GetConsumerCategoryGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.ConsumerCategory.ToString(), GetConsumerCategoryStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.ConsumerCategory);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            var param = new Dictionary<string, string>
                {
                    { "service", string.Concat(Scope, ".getSelectedServiceId()") }
                };

            gp.ColumnModel.AddColumn(() => consumerCategoryModel.RefServicesCPotrName, DataAttributeTypes.dtString)
                .SetWidth(100)
                .SetComboBoxEditor(
                                    D_Services_CPotr.Key,
                                    Page,
                                    consumerCategoryModel.NameOf(() => consumerCategoryModel.RefServicesCPotr),
                                    UiBuilders.GetUrl<ConsumerCategoryController>("GetCategory"),
                                    null,
                                    true,
                                    false,
                                    param);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = consumerCategoryModel.NameOf(() => consumerCategoryModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = consumerCategoryModel.NameOf(() => consumerCategoryModel.RefServicesCPotrName) });
            
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetIndicatorsOfServiceGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.IndicatorsOfService.ToString(), GetIndicatorsOfServiceStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.IndicatorsOfService);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();
            
            var param = new Dictionary<string, string>
                {
                    { "service", string.Concat(Scope, ".getSelectedServiceId()") }
                };
            var mapping = new Dictionary<string, string[]>
                {
                    { IndicatorsOfServiceFields.RefIndicators.ToString(), new[] { "ID" } },
                    { IndicatorsOfServiceFields.PnrOkei.ToString(), new[] { "RefOKEI" } },
                    { IndicatorsOfServiceFields.PnrType.ToString(), new[] { "RefCharacteristicType" } },
                    { IndicatorsOfServiceFields.RefIndicatorsName.ToString(), new[] { "Name", "RefOKEI", "RefCharacteristicType" } }
                };

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.RefIndicatorsName.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.RefIndicatorsName),
                DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(D_Services_Indicators.Key, Page, mapping, UiBuilders.GetUrl<StateTaskTab2Controller>("GetIndicators"), null, null, true, false, param);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.PnrOkei.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.PnrOkei),
                DataAttributeTypes.dtString)
                .SetWidth(200).SetEditable(false);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.PnrType.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.PnrType),
                DataAttributeTypes.dtString)
                .SetWidth(200).SetEditable(false);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.ReportingYear.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.ReportingYear),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(100);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.CurrentYear.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.CurrentYear),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(100);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.ComingYear.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.ComingYear),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(100);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.FirstPlanYear.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.FirstPlanYear),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(100);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.SecondPlanYear.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.SecondPlanYear),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(100);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.ActualValue.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.ActualValue),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(100);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.Protklp.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.Protklp),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(200);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.Info.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.Info),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(200).SetMaxLengthEdior(4000);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.Source.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.Source),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(200).SetMaxLengthEdior(4000);

            gp.ColumnModel.AddColumn(
                IndicatorsOfServiceFields.SourceInfFact.ToString(),
                StateTaskHelpers.IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields.SourceInfFact),
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(200).SetMaxLengthEdior(2000);

            gp.Plugins.Add(
               new GridFilters
               {
                   Local = true,
                   Filters =
                            {
                                new NumericFilter { DataIndex = IndicatorsOfServiceFields.ID.ToString() },
                                new StringFilter { DataIndex = IndicatorsOfServiceFields.RefIndicatorsName.ToString() },
                                new StringFilter { DataIndex = IndicatorsOfServiceFields.PnrOkei.ToString() },
                                new StringFilter { DataIndex = IndicatorsOfServiceFields.PnrType.ToString() },
                                new NumericFilter { DataIndex = IndicatorsOfServiceFields.ReportingYear.ToString() },
                                new NumericFilter { DataIndex = IndicatorsOfServiceFields.CurrentYear.ToString() },
                                new NumericFilter { DataIndex = IndicatorsOfServiceFields.ComingYear.ToString() },
                                new NumericFilter { DataIndex = IndicatorsOfServiceFields.FirstPlanYear.ToString() },
                                new NumericFilter { DataIndex = IndicatorsOfServiceFields.SecondPlanYear.ToString() },
                                new NumericFilter { DataIndex = IndicatorsOfServiceFields.ActualValue.ToString() },
                                new StringFilter { DataIndex = IndicatorsOfServiceFields.Protklp.ToString() },
                                new StringFilter { DataIndex = IndicatorsOfServiceFields.Info.ToString() },
                                new StringFilter { DataIndex = IndicatorsOfServiceFields.Source.ToString() },
                                new StringFilter { DataIndex = IndicatorsOfServiceFields.SourceInfFact.ToString() }
                            }
               });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetRequisitesNpaGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.RequisitesNpa.ToString(), GetRequisitesNpaStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.RequisitesNpa);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn("Name", "Наименование НПА", DataAttributeTypes.dtString).SetEditableString().SetWidth(200);
            gp.ColumnModel.AddColumn("DataNPAGZ", "Дата НПА", DataAttributeTypes.dtDate).SetEditableDate().SetWidth(100);
            gp.ColumnModel.AddColumn("NumNPA", "Номер НПА", DataAttributeTypes.dtString).SetEditableString().SetWidth(200);
            gp.ColumnModel.AddColumn("OrgUtvDoc", "Орган утвердивший НПА", DataAttributeTypes.dtString).SetEditableString().SetWidth(200);
            gp.ColumnModel.AddColumn("VidNPAGZ", "Вид НПА", DataAttributeTypes.dtString).SetEditableString().SetWidth(200);

            gp.Plugins.Add(
               new GridFilters
               {
                   Local = true,
                   Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "Name" },
                                new DateFilter { DataIndex = "DataNPAGZ" },
                                new StringFilter { DataIndex = "NumNPA" },
                                new StringFilter { DataIndex = "OrgUtvDoc" },
                                new StringFilter { DataIndex = "VidNPAGZ" }
                            }
               });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }
        
        private GridPanel GetMonitoringExecutionGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.MonitoringExecution.ToString(), GetMonitoringExecutionStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.MonitoringExecution);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn("Form", "Форма контроля", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);
            gp.ColumnModel.AddColumn("Rate", "Периодичность", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(
                "Supervisor",
                "Орган исполнительной власти, осуществляющий контроль за оказанием услуги",
                DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "Form" },
                                new StringFilter { DataIndex = "Rate" },
                                new StringFilter { DataIndex = "Supervisor" }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetReportingRequirementsGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.ReportingRequirements.ToString(), /*store*/GetReportingRequirementsStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.ReportingRequirements);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn("DeliveryTerm", "Сроки предоставления отчетов", DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(300).SetMaxLengthEdior(1000);
            gp.ColumnModel.AddColumn("OtherRequest", "Иное требование к отчетности", DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(300).SetMaxLengthEdior(1000);
            gp.ColumnModel.AddColumn(
                "OtherInfo",
                "Иная информация, необходимая для исполнения",
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(300).SetMaxLengthEdior(1000);

            gp.ColumnModel.AddColumn(
                ReportingRequirementsFields.ReportForm.ToString(),
                StateTaskHelpers.ReportingRequirementsFieldsMapping(ReportingRequirementsFields.ReportForm),
                DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(300).SetMaxLengthEdior(1000);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "DeliveryTerm" },
                                new StringFilter { DataIndex = "OtherRequest" },
                                new StringFilter { DataIndex = "OtherInfo" },
                                new StringFilter { DataIndex = ReportingRequirementsFields.ReportForm.ToString() }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetGroundsForTerminationGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.GroundsForTermination.ToString(), GetGroundsForTerminationStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.GroundsForTermination);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(
                "EarlyTerminat",
                "Основание для досрочного прекращения",
                DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(500).SetMaxLengthEdior(4000);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "EarlyTerminat" }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetNpaRegulatesServiceGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.NpaRegulatesService.ToString(), GetNpaRegulatesServiceStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.NpaRegulatesService);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn("RenderEnact", "Наименование НПА", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            gp.ColumnModel.AddColumn("TypeNpa", "Вид НПА", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(200);

            gp.ColumnModel.AddColumn("DateNpa", "Дата НПА", DataAttributeTypes.dtDate)
                .SetEditableDate().SetWidth(100);

            gp.ColumnModel.AddColumn("NumberNpa", "Номер НПА", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(200).SetMaxLengthEdior(200);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "RenderEnact" },
                                new StringFilter { DataIndex = "TypeNpa" },
                                new DateFilter { DataIndex = "DateNpa" },
                                new StringFilter { DataIndex = "NumberNpa" }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetLimitValuesOfPricesGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.LimitValuesOfPrices.ToString(), GetLimitValuesOfPricesStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.LimitValuesOfPrices);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn("Name", "Наименование элемента услуги", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);
            gp.ColumnModel.AddColumn("Price", "Цена (тариф)", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(1000);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "Name" },
                                new StringFilter { DataIndex = "Price" }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetInformConsumersGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(Details.InformConsumers.ToString(), /*store*/GetInformConsumersStore());
            gp.Title = StateTaskHelpers.DatailsNameMapping(Details.InformConsumers);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn("Method", "Способ информирования", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn("Content", "Состав размещаемой информации", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(4000);
            gp.ColumnModel.AddColumn("Rate", "Частота обновления информации", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300).SetMaxLengthEdior(200);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "Method" },
                                new StringFilter { DataIndex = "Content" },
                                new StringFilter { DataIndex = "Rate" }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }
    }
}