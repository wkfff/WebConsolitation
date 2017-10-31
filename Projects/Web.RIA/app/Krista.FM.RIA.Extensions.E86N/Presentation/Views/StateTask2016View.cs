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
using Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model;
using Krista.FM.RIA.Extensions.E86N.Models.StateTaskModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTask2016;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class StateTask2016View : View
    {
        private const string Scope = "E86n.View.StateTask2016";

        private const string StateTask2016ViewID = "StateTask2016";
        private const string StateTask2016GridID = "StateTask2016Grid";
        private const string StateTask2016StoreID = "StateTask2016Store";

        private const string EarlyTerminationViewID = "EarlyTermination";
        private const string EarlyTerminationStoreID = "EarlyTerminationStore";

        private const string OtherInfoViewID = "OtherInfo";
        private const string OtherInfoStoreID = "OtherInfoStore";

        private const string SupervisionProcedureViewID = "SupervisionProcedure";
        private const string SupervisionProcedureStoreID = "SupervisionProcedureStore";

        private const string ReportRequirementsViewID = "ReportRequirements";
        private const string ReportRequirementsStoreID = "ReportRequirementsStore";

        private const string ReportsViewID = "Reports";
        private const string ReportsStoreID = "ReportsStore";

        private const string ConsumersCategoryViewID = "ConsumersCategory";
        private const string ConsumersCategoryStoreID = "ConsumersCategoryStore";

        private const string QualityVolumeIndexesViewID = "QualityVolumeIndexes";
        private const string QualityVolumeIndexesStoreID = "QualityVolumeIndexesStore";

        private const string PriceEnactmentViewID = "PriceEnactment";
        private const string PriceEnactmentStoreID = "PriceEnactmentStore";

        private const string RenderEnactmentViewID = "RenderEnactment";
        private const string RenderEnactmentStoreID = "RenderEnactmentStore";

        private const string InformingProcedureViewID = "InformingProcedure";
        private const string InformingProcedureStoreID = "InformingProcedureStore";

        private const string AveragePriceViewID = "AveragePrice";
        private const string AveragePriceStoreID = "AveragePriceStore";

        private const string ExtHeaderStoreID = "StateTaskExtAttrCbNotBringStore";
        private const string ExtHeaderID = "StateTaskExtAttrCbNotBring";
        private const string ExtHeaderPanelID = "StateTaskExtAttrCbNotBringPanel";

        private const string AdditionalInformationID = "AdditionalInformationForm";

        private readonly IAuthService auth;
        private readonly INewRestService newRestService;

        private readonly StateTask2016ViewModel stateTask2016ViewModel = new StateTask2016ViewModel();

        private readonly EarlyTerminationViewModel earlyTerminationViewModel = new EarlyTerminationViewModel();
        private readonly OtherInfoViewModel otherInfoViewModel = new OtherInfoViewModel();
        private readonly SupervisionProcedureViewModel supervisionProcedureViewModel = new SupervisionProcedureViewModel();
        private readonly ReportRequirementsViewModel reportRequirementsViewModel = new ReportRequirementsViewModel();
        private readonly ReportsViewModel reportsViewModel = new ReportsViewModel();

        private readonly ConsumersCategoryViewModel consumersCategoryViewModel = new ConsumersCategoryViewModel();
        private readonly QualityVolumeIndexesViewModel qualityVolumeIndexesViewModel = new QualityVolumeIndexesViewModel();
        private readonly PriceEnactmentViewModel priceEnactmentViewModel = new PriceEnactmentViewModel();
        private readonly RenderEnactmentViewModel renderEnactmentViewModel = new RenderEnactmentViewModel();
        private readonly InformingProcedureViewModel informingProcedureViewModel = new InformingProcedureViewModel();
        private readonly AveragePriceViewModel averagePriceViewModel = new AveragePriceViewModel();

        private readonly ExtHeaderModel extHeaderModel = new ExtHeaderModel();

        private F_F_ParameterDoc doc;

        public StateTask2016View(IAuthService auth, INewRestService newRestService)
        {
            this.auth = auth;
            this.newRestService = newRestService;
        }
        
        public int? DocId
        {
            get { return Params["docId"] == "null" ? -1 : Convert.ToInt32(Params["docId"]); }
        }

        public ViewPage Page { get; set; }
        
        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            doc = newRestService.GetItem<F_F_ParameterDoc>(DocId);

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StateTask2016View", Resource.StateTask2016View);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            RestActions restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;
            
            Toolbar tb = new NewStateToolBarControl(DocId ?? -1).BuildComponent(page);

            tb.Add(new VersioningControl(Convert.ToInt32(DocId.ToString()), "StateTask2016", Scope + ".CloseStateTaskDoc").Build(page));

            if (auth.IsAdmin())
            {
                var btn = new UpLoadFileBtnControl
                {
                    Id = "btnExport",
                    Name = "Экспорт в XML",
                    Icon = Icon.DiskDownload,
                    Upload = false,
                    UploadController = UiBuilders.GetUrl<StateTask2016ViewController>("ExportToXml"),
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
                UploadController = UiBuilders.GetUrl<ReportsController>("GetStateTask2016Form"),
                Params = { { "docId", DocId.ToString() } }
            };

            tb.Add(btnExcel.Build(Page));

            var paramDocPanel = new ParamDocPanelControl(DocId ?? -1, tb);
            paramDocPanel.ParamDocStore.Listeners.Load.AddAfter("E86n.View.StateTask2016.updateYearHeader(records[0]);");
            
            var rowLayout = new RowLayout();

            if (doc.RefUchr.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID))
            {
                rowLayout.Rows.Add(new LayoutRow
                {
                    Items =
                                        {
                                            GetAttrFormPanel()
                                        }
                });
            }

            var detail = UiBuilders.GetTabbedDetails(new List<Component>
                                                        {
                                                            GetDetailUi(),
                                                            GetEarlyTerminationGrid(),
                                                            GetOtherInfoGrid(),
                                                            GetSupervisionProcedureGrid(),
                                                            GetReportRequirementsGrid(),
                                                            GetReportsGrid(),
                                                            GetAdditionalInformationForm(),
                                                            new DocsDetailControl(DocId ?? -1).BuildComponent(page)
                                                        });

            ((TabPanel)detail).Listeners.TabChange.Fn(Scope, "reloadDetail");

            rowLayout.Rows.Add(new LayoutRow
                                {
                                    RowHeight = 1,
                                    Items = { detail }
                                });

            var view = new Viewport
                           {
                               ID = "vpStateTask2016",
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
                 ExtHeaderStoreID,
                 true,
                 typeof(StateTask2016Controller),
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
                ID = ExtHeaderPanelID,
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
                                                         UiBuilders.GetUrl<StateTask2016Controller>(
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
                ID = StateTask2016ViewID,
                Title = @"Государственное задание",
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
                                                                   CreateStateTask2016GridPanel()
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
            var tabPanel = UiBuilders.GetTabbedDetails(new List<Component>
                                                           {
                                                               GetConsumersCategoryGrid(),
                                                               GetQualityVolumeIndexesGrid(),
                                                               GetPriceEnactmentGrid(),
                                                               GetRenderEnactmentGrid(),
                                                               GetAveragePriceGrid(),
                                                               GetInformingProcedureGrid()
                                                           }) as TabPanel;
            if (tabPanel != null)
            {
                tabPanel.ID = "DetailTabPanel";
                tabPanel.Disabled = true;
                tabPanel.Listeners.TabChange.Fn(Scope, "reloadDetail");
                return tabPanel;
            }

            return null;
        }

        private Store GetStateTask2016Store()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                StateTask2016StoreID,
                false,
                typeof(StateTask2016Controller),
                updateActionName: "Create");

            store.AddFieldsByClass(stateTask2016ViewModel);

            store.SetBaseParams("parentId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("parentId", DocId.ToString(), ParameterMode.Value);

            store.Listeners.Load.Fn = Scope + ".fillOtherSourcesCheckbox";

            Page.Controls.Add(store);

            return store;
        }

        private Store GetConsumersCategoryStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                ConsumersCategoryStoreID,
                false,
                typeof(ConsumersCategoryController),
                updateActionName: "Create");

            store.AddFieldsByClass(consumersCategoryViewModel);

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);
            store.SetBaseParams("isOtherSources", Scope + ".getOtherSourcesCheckboxValue()", ParameterMode.Raw);
            store.SetWriteBaseParams("isOtherSources", Scope + ".getOtherSourcesCheckboxValue()", ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetPriceEnactmentStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                PriceEnactmentStoreID,
                false,
                typeof(PriceEnactmentController),
                updateActionName: "Create");

            store.AddFieldsByClass(priceEnactmentViewModel);

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetQualityVolumeIndexesStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                QualityVolumeIndexesStoreID,
                false,
                typeof(QualityVolumeIndexesController),
                updateActionName: "Create");

            store.AddFieldsByClass(qualityVolumeIndexesViewModel);

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);
            store.SetBaseParams("isOtherSources", Scope + ".getOtherSourcesCheckboxValue()", ParameterMode.Raw);
            store.SetWriteBaseParams("isOtherSources", Scope + ".getOtherSourcesCheckboxValue()", ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetSupervisionProcedureStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
               SupervisionProcedureStoreID,
                false,
                typeof(SupervisionProcedureController),
                updateActionName: "Create");

            store.AddFieldsByClass(supervisionProcedureViewModel);

            store.SetBaseParams("masterId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("masterId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetReportRequirementsStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                ReportRequirementsStoreID,
                false,
                typeof(ReportRequirementsController),
                updateActionName: "Create");
            
            store.AddFieldsByClass(reportRequirementsViewModel);

            store.SetBaseParams("masterId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("masterId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetEarlyTerminationStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                EarlyTerminationStoreID,
                false,
                typeof(EarlyTerminationController),
                updateActionName: "Create");

            store.AddFieldsByClass(earlyTerminationViewModel);

            store.SetBaseParams("masterId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("masterId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetOtherInfoStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                OtherInfoStoreID,
                false,
                typeof(OtherInfoController),
                updateActionName: "Create");

            store.AddFieldsByClass(otherInfoViewModel);

            store.SetBaseParams("masterId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("masterId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetReportsStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                ReportsStoreID,
                false,
                typeof(StateTask2016ReportsController),
                updateActionName: "Create");

            store.AddFieldsByClass(reportsViewModel);

            store.SetBaseParams("masterId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("masterId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetRenderEnactmentStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                RenderEnactmentStoreID,
                false,
                typeof(RenderEnactmentController),
                updateActionName: "Create");

            store.AddFieldsByClass(renderEnactmentViewModel);
            
            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetInformingProcedureStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                InformingProcedureStoreID,
                false,
                typeof(InformingProcedureController),
                updateActionName: "Create");
            
            store.AddFieldsByClass(informingProcedureViewModel);

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private Store GetAveragePriceStrore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                AveragePriceStoreID,
                false,
                typeof(AveragePriceController),
                updateActionName: "Create");
            
            store.AddFieldsByClass(averagePriceViewModel);

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(StateTask2016GridID), ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Value);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Value);

            Page.Controls.Add(store);

            return store;
        }

        private GridPanel CreateStateTask2016GridPanel()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(StateTask2016GridID, GetStateTask2016Store());

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = Scope + ".RowSelect(record);";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.Toolbar().Add(new ToolbarSeparator());
            
            var cb = new Checkbox
            {
                ID = "OtherSourcesCheckbox",
                FieldLabel = @"Выбор услуг из иных источников",
                LabelPad = 100,
                Checked = false,
                Disabled = auth.IsPpoUser(),
                Listeners =
                                {
                                    Check =
                                        {
                                            Handler = Scope + ".changeService(item, checked, '{0}', {1})".FormatWith(
                                                UiBuilders.GetUrl<StateTask2016Controller>(
                                                    "ChangeService",
                                                    new Dictionary<string, object>
                                                        {
                                                            { "docId", DocId }
                                                        }),
                                                DocId)
                                        }
                                }
            };

            gp.Toolbar().Add(cb);
            gp.Toolbar().Add(new ToolbarSeparator());
            
            var param =
                new Dictionary<string, string>
                    {
                        { "docId", DocId.ToString() },
                        { "isOtherSources", Scope + ".getOtherSourcesCheckboxValue()" }
                    };

            var mapping =
                new Dictionary<string, string[]>
                    {
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefService),               new[] { "RefService" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServiceName),           new[] { "NameName", "Regrnumber", "NameCode", "SvcCntsName1Val", "SvcCntsName2Val", "SvcCntsName3Val", "SvcTermsName1Val", "SvcTermsName2Val" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServiceTypeName),       new[] { "RefServiceTypeName" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServiceTypeCode),       new[] { "RefServiceTypeCode" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServiceUniqueNumber),   new[] { "RefServiceUniqueNumber" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServiceRegNum),         new[] { "RefServiceRegNum" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServicePayName),        new[] { "RefServicePayName" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServicePayCode),        new[] { "RefServicePayCode" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServiceContentIndex),   new[] { "RefServiceContentIndex" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.RefServiceConditionIndex), new[] { "RefServiceConditionIndex" } },
                        { stateTask2016ViewModel.NameOf(() => stateTask2016ViewModel.IsOtherSources),           new[] { "IsOtherSources" } }
                    };

            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.RefServiceName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(D_Services_Service.Key, Page, mapping, UiBuilders.GetUrl<StateTask2016Controller>("GetServices"), null, null, true, false, param);

            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.RefService,               DataAttributeTypes.dtInteger).SetHidden(true).SetWidth(150).SetEditable(false);
            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.RefServiceTypeName,       DataAttributeTypes.dtString).SetWidth(150).SetEditable(false);
            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.RefServiceUniqueNumber,   DataAttributeTypes.dtString).SetWidth(200).SetEditable(false);
            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.RefServiceRegNum,         DataAttributeTypes.dtString).SetWidth(50).SetEditable(false);
            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.RefServicePayName,        DataAttributeTypes.dtString).SetWidth(150).SetEditable(false);
            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.RefServiceContentIndex,   DataAttributeTypes.dtString).SetWidth(2000).SetEditable(false);
            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.RefServiceConditionIndex, DataAttributeTypes.dtString).SetWidth(2000).SetEditable(false);
            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.AveragePrice,             DataAttributeTypes.dtDouble).SetNullable().SetWidth(300).SetMaxLengthEdior(20).SetEditableDouble(2);
            gp.ColumnModel.AddColumn(() => stateTask2016ViewModel.IsOtherSources,           DataAttributeTypes.dtBoolean).SetHidden(true).SetWidth(150).SetEditable(false);
            
            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetConsumersCategoryGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(ConsumersCategoryViewID, GetConsumersCategoryStore());
            gp.Title = @"Категории потребителей";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            var param = new Dictionary<string, string>
                {
                    { "Service", string.Concat(Scope, ".getSelectedServiceId()") },
                    { "isOtherSources", Scope + ".getOtherSourcesCheckboxValue()" }
                };

            gp.ColumnModel.AddColumn(() => consumersCategoryViewModel.RefConsumersCategoryName, DataAttributeTypes.dtString)
                .SetWidth(100)
                .SetComboBoxEditor(
                    F_F_ServiceConsumersCategory.Key,
                    Page,
                    consumersCategoryViewModel.NameOf(() => consumersCategoryViewModel.RefConsumersCategory),
                    UiBuilders.GetUrl<ConsumersCategoryController>("GetCategory"),
                    null,
                    true,
                    false,
                    param);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = consumersCategoryViewModel.NameOf(() => consumersCategoryViewModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = consumersCategoryViewModel.NameOf(() => consumersCategoryViewModel.RefConsumersCategoryName) });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetQualityVolumeIndexesGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(QualityVolumeIndexesViewID, GetQualityVolumeIndexesStore());
            gp.Title = @"Показатели";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            var param = new Dictionary<string, string>
                {
                    { "service", string.Concat(Scope, ".getSelectedServiceId()") },
                    { "isOtherSources", Scope + ".getOtherSourcesCheckboxValue()" }
                };
            var mapping = new Dictionary<string, string[]>
                {
                    { qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefIndicatorsName),      new[] { "Name", "RefType", "RefOKEI" } },
                    { qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefIndicators),          new[] { "ID" } },
                    { qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefIndicatorsOKEIName),  new[] { "RefOKEI" } },
                    { qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefIndicatorsTypeName),  new[] { "RefType" } }
                };

            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.RefIndicatorsName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(F_F_ServiceIndicators.Key, Page, mapping, UiBuilders.GetUrl<QualityVolumeIndexesController>("GetIndicators"), null, null, true, false, param);
            
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.RefIndicatorsOKEIName,  DataAttributeTypes.dtString).SetWidth(200).SetEditable(false);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.RefIndicatorsTypeName,  DataAttributeTypes.dtString).SetWidth(200).SetEditable(false);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.ReportingYear,          DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(100).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.CurrentYear,            DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(100).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.ComingYear,             DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(100).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.FirstPlanYear,          DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(100).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.SecondPlanYear,         DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(100).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.ActualValue,            DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(100).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.AveragePriceFact,       DataAttributeTypes.dtDouble).SetNullable().SetEditableDouble(2).SetWidth(100).SetMaxLengthEdior(20);

            param = new Dictionary<string, string>
                {
                    { "docId", DocId.ToString() }
                };

            mapping = new Dictionary<string, string[]>
                {
                    { qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefReportName),   new[] { "NameReport" } },
                    { qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefReport),       new[] { "ID" } }
                };

            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.RefReportName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(F_F_Reports.Key, Page, mapping, UiBuilders.GetUrl<QualityVolumeIndexesController>("GetReports"), null, "NameReport", true, true, param);

            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.Reject,                 DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(200).SetMaxLengthEdior(250);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.Protklp,                DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(200).SetMaxLengthEdior(500);
            gp.ColumnModel.AddColumn(() => qualityVolumeIndexesViewModel.Deviation,              DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(200).SetMaxLengthEdior(250);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.ID) },
                                new StringFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefIndicatorsName) },
                                new StringFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefIndicatorsOKEIName) },
                                new StringFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.RefIndicatorsType) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.ReportingYear) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.CurrentYear) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.ComingYear) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.FirstPlanYear) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.SecondPlanYear) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.ActualValue) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.Reject) },
                                new StringFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.Protklp) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.Deviation) },
                                new NumericFilter { DataIndex = qualityVolumeIndexesViewModel.NameOf(() => qualityVolumeIndexesViewModel.AveragePriceFact) }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetPriceEnactmentGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(PriceEnactmentViewID, GetPriceEnactmentStore());
            gp.Title = @"Реквизиты НПА, устанавливающих цены";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => priceEnactmentViewModel.Name,        DataAttributeTypes.dtString).SetEditableString().SetWidth(200);
            gp.ColumnModel.AddColumn(() => priceEnactmentViewModel.DataNPAGZ,   DataAttributeTypes.dtDate).SetEditableDate().SetWidth(100);
            gp.ColumnModel.AddColumn(() => priceEnactmentViewModel.NumNPA,      DataAttributeTypes.dtString).SetEditableString().SetWidth(200);
            gp.ColumnModel.AddColumn(() => priceEnactmentViewModel.OrgUtvDoc,   DataAttributeTypes.dtString).SetEditableString().SetWidth(200);
            gp.ColumnModel.AddColumn(() => priceEnactmentViewModel.VidNPAGZ,    DataAttributeTypes.dtString).SetEditableString().SetWidth(200);

            gp.Plugins.Add(
               new GridFilters
               {
                   Local = true,
                   Filters =
                            {
                                new NumericFilter { DataIndex = priceEnactmentViewModel.NameOf(() => priceEnactmentViewModel.ID) },
                                new StringFilter { DataIndex = priceEnactmentViewModel.NameOf(() => priceEnactmentViewModel.Name) },
                                new DateFilter { DataIndex = priceEnactmentViewModel.NameOf(() => priceEnactmentViewModel.DataNPAGZ) },
                                new StringFilter { DataIndex = priceEnactmentViewModel.NameOf(() => priceEnactmentViewModel.NumNPA) },
                                new StringFilter { DataIndex = priceEnactmentViewModel.NameOf(() => priceEnactmentViewModel.OrgUtvDoc) },
                                new StringFilter { DataIndex = priceEnactmentViewModel.NameOf(() => priceEnactmentViewModel.VidNPAGZ) }
                            }
               });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetSupervisionProcedureGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(SupervisionProcedureViewID, GetSupervisionProcedureStore());
            gp.Title = @"Порядок контроля за исполнением";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => supervisionProcedureViewModel.Form,          DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(4000);
            gp.ColumnModel.AddColumn(() => supervisionProcedureViewModel.Rate,          DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(4000);
            gp.ColumnModel.AddColumn(() => supervisionProcedureViewModel.Supervisor,    DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);
            
            gp.Plugins.Add(
                new GridFilters
                {
                    Local = true,
                    Filters =
                            {
                                new NumericFilter { DataIndex = supervisionProcedureViewModel.NameOf(() => supervisionProcedureViewModel.ID) },
                                new StringFilter { DataIndex = supervisionProcedureViewModel.NameOf(() => supervisionProcedureViewModel.Form) },
                                new StringFilter { DataIndex = supervisionProcedureViewModel.NameOf(() => supervisionProcedureViewModel.Rate) },
                                new StringFilter { DataIndex = supervisionProcedureViewModel.NameOf(() => supervisionProcedureViewModel.Supervisor) }
                            }
                });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetReportRequirementsGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(ReportRequirementsViewID, GetReportRequirementsStore());
            gp.Title = @"Требования к отчетности об исполнении";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => reportRequirementsViewModel.PeriodicityTerm).SetNotNullable().SetWidth(300);
            gp.ColumnModel.AddColumn(() => reportRequirementsViewModel.DeliveryTerm).SetWidth(300);
            gp.ColumnModel.AddColumn(() => reportRequirementsViewModel.OtherRequest).SetWidth(300);
            gp.ColumnModel.AddColumn(() => reportRequirementsViewModel.OtherIndicators).SetWidth(300);
            gp.ColumnModel.AddColumn(() => reportRequirementsViewModel.ReportForm).SetWidth(300);

            gp.Plugins.Add(
                new GridFilters
                {
                    Local = true,
                    Filters =
                            {
                                new NumericFilter { DataIndex = reportRequirementsViewModel.NameOf(() => reportRequirementsViewModel.ID) },
                                new StringFilter { DataIndex = reportRequirementsViewModel.NameOf(() => reportRequirementsViewModel.PeriodicityTerm) },
                                new StringFilter { DataIndex = reportRequirementsViewModel.NameOf(() => reportRequirementsViewModel.DeliveryTerm) },
                                new StringFilter { DataIndex = reportRequirementsViewModel.NameOf(() => reportRequirementsViewModel.OtherRequest) },
                                new StringFilter { DataIndex = reportRequirementsViewModel.NameOf(() => reportRequirementsViewModel.OtherIndicators) },
                                new StringFilter { DataIndex = reportRequirementsViewModel.NameOf(() => reportRequirementsViewModel.ReportForm) }
                            }
                });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetEarlyTerminationGrid()
        {
            var gp = UiBuilders.CreateGridPanel(EarlyTerminationViewID, GetEarlyTerminationStore());
            gp.Title = @"Основания для прекращения";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();
            
            gp.ColumnModel.AddColumn(() => earlyTerminationViewModel.EarlyTerminat, DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(500).SetMaxLengthEdior(2000);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = earlyTerminationViewModel.NameOf(() => earlyTerminationViewModel.ID) },
                                new StringFilter { DataIndex = earlyTerminationViewModel.NameOf(() => earlyTerminationViewModel.EarlyTerminat) }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetOtherInfoGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(OtherInfoViewID, GetOtherInfoStore());
            gp.Title = @"Иная информация";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => otherInfoViewModel.OtherInfo, DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            gp.Plugins.Add(
                new GridFilters
                {
                    Local = true,
                    Filters =
                            {
                                new NumericFilter { DataIndex = otherInfoViewModel.NameOf(() => otherInfoViewModel.ID) },
                                new StringFilter { DataIndex = otherInfoViewModel.NameOf(() => otherInfoViewModel.OtherInfo) }
                            }
                });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetReportsGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(ReportsViewID, GetReportsStore());
            gp.Title = @"Отчеты о выполнении";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();
            
            gp.ColumnModel.AddColumn(() => reportsViewModel.NameReport,     DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);
            gp.ColumnModel.AddColumn(() => reportsViewModel.HeadName,       DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);
            gp.ColumnModel.AddColumn(() => reportsViewModel.HeadPosition,   DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => reportsViewModel.DateReport,     DataAttributeTypes.dtDate).SetEditableDate().SetWidth(300);
            gp.ColumnModel.AddColumn(() => reportsViewModel.ReportGuid,     DataAttributeTypes.dtString);

            gp.Plugins.Add(
                new GridFilters
                {
                    Local = true,
                    Filters =
                            {
                                new NumericFilter { DataIndex = reportsViewModel.NameOf(() => reportsViewModel.ID) },
                                new StringFilter { DataIndex = reportsViewModel.NameOf(() => reportsViewModel.ReportGuid) },
                                new StringFilter { DataIndex = reportsViewModel.NameOf(() => reportsViewModel.NameReport) },
                                new StringFilter { DataIndex = reportsViewModel.NameOf(() => reportsViewModel.HeadName) },
                                new StringFilter { DataIndex = reportsViewModel.NameOf(() => reportsViewModel.HeadPosition) },
                                new DateFilter { DataIndex = reportsViewModel.NameOf(() => reportsViewModel.DateReport) }
                            }
                });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetRenderEnactmentGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(RenderEnactmentViewID, GetRenderEnactmentStore());
            gp.Title = @"НПА, регулирующий порядок оказания услуги";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();
            
            gp.ColumnModel.AddColumn(() => renderEnactmentViewModel.RenderEnact,    DataAttributeTypes.dtString).SetEditableString().SetMaxLengthEdior(4419);
            gp.ColumnModel.AddColumn(() => renderEnactmentViewModel.TypeNpa,        DataAttributeTypes.dtString).SetEditableString().SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => renderEnactmentViewModel.DateNpa,        DataAttributeTypes.dtDate).SetEditableDate();
            gp.ColumnModel.AddColumn(() => renderEnactmentViewModel.NumberNpa,      DataAttributeTypes.dtString).SetEditableString().SetMaxLengthEdior(200);
            gp.ColumnModel.AddColumn(() => renderEnactmentViewModel.Author,         DataAttributeTypes.dtString).SetEditableString().SetMaxLengthEdior(2000);
            
            gp.Plugins.Add(
                new GridFilters
                {
                    Local = true,
                    Filters =
                            {
                                new NumericFilter { DataIndex = renderEnactmentViewModel.NameOf(() => renderEnactmentViewModel.ID) },
                                new StringFilter { DataIndex = renderEnactmentViewModel.NameOf(() => renderEnactmentViewModel.RenderEnact) },
                                new StringFilter { DataIndex = renderEnactmentViewModel.NameOf(() => renderEnactmentViewModel.TypeNpa) },
                                new DateFilter { DataIndex = renderEnactmentViewModel.NameOf(() => renderEnactmentViewModel.DateNpa) },
                                new StringFilter { DataIndex = renderEnactmentViewModel.NameOf(() => renderEnactmentViewModel.NumberNpa) },
                                new StringFilter { DataIndex = renderEnactmentViewModel.NameOf(() => renderEnactmentViewModel.Author) }
                            }
                });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetInformingProcedureGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(InformingProcedureViewID, GetInformingProcedureStore());
            gp.Title = @"Порядок информирования потенциальных потребителей";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(() => informingProcedureViewModel.Method,      DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);
            gp.ColumnModel.AddColumn(() => informingProcedureViewModel.Content,     DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(4000);
            gp.ColumnModel.AddColumn(() => informingProcedureViewModel.Rate,        DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            gp.Plugins.Add(
                new GridFilters
                {
                    Local = true,
                    Filters =
                            {
                                new NumericFilter { DataIndex = informingProcedureViewModel.NameOf(() => informingProcedureViewModel.ID) },
                                new StringFilter { DataIndex = informingProcedureViewModel.NameOf(() => informingProcedureViewModel.Method) },
                                new StringFilter { DataIndex = informingProcedureViewModel.NameOf(() => informingProcedureViewModel.Content) },
                                new StringFilter { DataIndex = informingProcedureViewModel.NameOf(() => informingProcedureViewModel.Rate) }
                            }
                });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetAveragePriceGrid()
        {
            var gp = UiBuilders.CreateGridPanel(AveragePriceViewID, GetAveragePriceStrore());
            gp.Title = @"Среднегодовой размер платы";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();

            var param = new Dictionary<string, string>
                {
                    { "stateTask", "getMasterID('{0}')".FormatWith(StateTask2016GridID) }
                };
            var mapping = new Dictionary<string, string[]>
                {
                    { averagePriceViewModel.NameOf(() => averagePriceViewModel.RefVolumeIndexName),      new[] { "Name" } },
                    { averagePriceViewModel.NameOf(() => averagePriceViewModel.RefVolumeIndex),          new[] { "ID" } }
                };

            gp.ColumnModel.AddColumn(() => averagePriceViewModel.RefVolumeIndexName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(F_F_ServiceIndicators.Key, Page, mapping, UiBuilders.GetUrl<AveragePriceController>("GetVolumeIndexes"), null, null, true, false, param);

            gp.ColumnModel.AddColumn(() => averagePriceViewModel.ReportYearDec);
            gp.ColumnModel.AddColumn(() => averagePriceViewModel.CurrentYearDec);
            gp.ColumnModel.AddColumn(() => averagePriceViewModel.NextYearDec);
            gp.ColumnModel.AddColumn(() => averagePriceViewModel.PlanFirstYearDec);
            gp.ColumnModel.AddColumn(() => averagePriceViewModel.PlanLastYearDec);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = averagePriceViewModel.NameOf(() => averagePriceViewModel.ID) },
                                new NumericFilter { DataIndex = averagePriceViewModel.NameOf(() => averagePriceViewModel.ReportYearDec) },
                                new NumericFilter { DataIndex = averagePriceViewModel.NameOf(() => averagePriceViewModel.CurrentYearDec) },
                                new NumericFilter { DataIndex = averagePriceViewModel.NameOf(() => averagePriceViewModel.NextYearDec) },
                                new NumericFilter { DataIndex = averagePriceViewModel.NameOf(() => averagePriceViewModel.PlanFirstYearDec) },
                                new NumericFilter { DataIndex = averagePriceViewModel.NameOf(() => averagePriceViewModel.PlanLastYearDec) }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private FormPanel GetAdditionalInformationForm()
        {
            var store = StoreExtensions.StoreCreateDefault(
                                                            AdditionalInformationID,
                                                            false,
                                                            typeof(StateTask2016Controller),
                                                            extHeaderModel,
                                                            Page,
                                                            "ExtHeaderRead");
            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            store.Listeners.DataChanged.Handler = Scope + ".StoreDataChanged(store, '{0}');".FormatWith(AdditionalInformationID);

            var form = FormPanelExtensions.GetDefaultFormPanel(AdditionalInformationID);
            form.Title = @"Дополнительная информация о ГЗ";
            form.LabelWidth = 450;
            form.Url = UiBuilders.GetUrl<StateTask2016Controller>(AdditionalInformationID + "Save");
            form.BaseParams.Add(new Parameter("docId", DocId.ToString()));
            
            form.AddRefreshButton();
            form.AddSaveButton();

            form.Items.AddFormField(UiBuilders.SchemeOf(() => extHeaderModel.StatementTask));
            form.Items.AddFormField(UiBuilders.SchemeOf(() => extHeaderModel.StateTaskNumber));
            form.Items.AddFormField(UiBuilders.SchemeOf(() => extHeaderModel.ApproverLastName));
            form.Items.AddFormField(UiBuilders.SchemeOf(() => extHeaderModel.ApproverFirstName));
            form.Items.AddFormField(UiBuilders.SchemeOf(() => extHeaderModel.ApproverMiddleName));
            form.Items.AddFormField(UiBuilders.SchemeOf(() => extHeaderModel.ApproverPosition));
            
            return form;
        }
    }
}