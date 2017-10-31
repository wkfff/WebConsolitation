using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.Service2016Model;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Service2016;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class Service2016View : View
    {
        /// <summary>
        /// Ведомственный перечень услуг
        /// </summary>
        #region Setting

        private const string Scope = "E86n.View.Service2016";

        private const string Service2016ViewID = "Service2016";
        private const string Service2016StoreID = "Service2016Store";

        private const string ConsumersCategoryViewID = "ServiceConsumersCategory";
        private const string ConsumersCategoryStoreID = "ServiceConsumersCategoryStore";

        private const string InstitutionsInfoViewID = "ServiceInstitutionsInfo";
        private const string InstitutionsInfoStoreID = "ServiceInstitutionsInfoStore";

        private const string IndicatorsViewID = "ServiceIndicators";
        private const string IndicatorsStoreID = "ServiceIndicatorsStore";

        private const string InstitutionTypeViewID = "ServiceInstitutionType";
        private const string InstitutionTypeStoreID = "ServiceInstitutionTypeStore";

        private const string LegalActViewID = "ServiceLegalAct";
        private const string LegalActStoreID = "ServiceLegalActStore";

        private const string OkpdViewID = "ServiceOKPD";
        private const string OkpdStoreID = "ServiceOKPDStore";

        private const string OKVEDViewID = "ServiceOKVED";
        private const string OKVEDStoreID = "ServiceOKVEDStore";

        private readonly Service2016ViewModel servicesModel = new Service2016ViewModel();
        private readonly Service2016ConsumersCategoryViewModel consumersCategoryModel = new Service2016ConsumersCategoryViewModel();
        private readonly Service2016InstitutionsInfoViewModel institutionsInfoModel = new Service2016InstitutionsInfoViewModel();
        private readonly Service2016IndicatorsViewModel indicatorsModel = new Service2016IndicatorsViewModel();
        private readonly Service2016InstitutionTypeViewModel institutionTypeModel = new Service2016InstitutionTypeViewModel();
        private readonly Service2016LegalActViewModel legalActModel = new Service2016LegalActViewModel();
        private readonly Service2016OKPDViewModel okpdModel = new Service2016OKPDViewModel();
        private readonly Service2016OKVEDViewModel okvedModel = new Service2016OKVEDViewModel();

        private readonly IAuthService auth;
        
        // Редактируемость перечня
        private bool editable;

        #endregion
        
        public Service2016View(IAuthService auth)
        {
            this.auth = auth;
        }

        public ViewPage Page { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            RestActions restActions = ResourceManager.GetInstance(Page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("CodeMaskBuilder", Resource.CodeMaskBuilder);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("Service2016", Resource.Service2016View);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);
            
            editable = Convert.ToBoolean(Params["editable"]);

            var view = new Viewport
            {
                Items =
                                   {
                                       new BorderLayout
                                           {
                                               Center = { Items = { GetMasterGrid() } },
                                               South = { Items = { GetDetailPanel() }, Split = true }
                                           }
                                   }
            };

            return new List<Component> { view };
        }

        #region MasterPanel

        private Store GetMasterStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                Service2016StoreID,
                true,
                typeof(Service2016Controller),
                updateActionName: "Create");

            store.AddFieldsByClass(servicesModel);
            store.AddField(servicesModel.NameOf(() => servicesModel.BusinessStatus), new RecordField.Config { DefaultValue = "false" });

            store.BaseParams.Add(new Parameter("limit", "pageSizeServices.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("editable", editable.ToString().ToLower(), ParameterMode.Raw));

            Page.Controls.Add(store);
            return store;
        }

        private IEnumerable<Component> GetMasterGrid()
        {
            var gp = UiBuilders.CreateGridPanel(Service2016ViewID, GetMasterStore());
            var rowSelectionModel = gp.SelectionModel[0] as RowSelectionModel;
            if (rowSelectionModel != null)
            {
                rowSelectionModel.ID = "ServiceRowSelectionModel";
                rowSelectionModel.MoveEditorOnEnter = false;
            }

            gp.BottomBar.Add(GetMasterGridBottomToolBar(gp.StoreID));
            gp.Plugins.Add(GetMasterGridFilters());
            GetMasterGridToolbar(gp);
            GetMasterGridColumnModel(gp.ColumnModel);
            gp.Listeners.Command.Handler = Scope + ".commandClick(command, record);";
            
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = Scope + ".UpdateView();";
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.Handler = Scope + ".UpdateView();";
            
            gp.AddColumnsWrapStylesToPage(Page);
            return new List<Component> { gp };
        }

        private void GetMasterGridColumnModel(ColumnModel columnModel)
        {
            columnModel.Columns.Add(
                new ImageCommandColumn
                {
                    DataIndex = "Action",
                    Width = 22,
                    Commands =
                            {
                                new ImageCommand
                                    {
                                        Icon = Icon.PageGo,
                                        CommandName = "SetActual",
                                        ToolTip = { Text = "Актуализировать услугу" },
                                    },
                                new ImageCommand
                                    {
                                        Icon = Icon.PageDelete,
                                        CommandName = "SetNotActual",
                                        ToolTip = { Text = "Исключить услугу" },
                                    },
                            },
                    PrepareCommand = { Handler = Scope + ".prepareCommand(command, record)" }
                });

            columnModel.AddColumn(() => servicesModel.BusinessStatus, DataAttributeTypes.dtBoolean).SetHidden(!editable);
            columnModel.AddColumn(() => servicesModel.RefTypeName, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.RefPayName, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.InstCode, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.RefYchrName, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.RefOKTMOName, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.RefActivityTypeCode, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.RefActivityTypeName, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.Regrnumber, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.NameCode, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.NameName, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.SvcCntsName1Val, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.SvcCntsName2Val, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.SvcCntsName3Val, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.SvcTermsName1Val, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.SvcTermsName2Val, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.EffectiveFrom, DataAttributeTypes.dtDate);
            columnModel.AddColumn(() => servicesModel.EffectiveBefore, DataAttributeTypes.dtDate);
            columnModel.AddColumn(() => servicesModel.GUID, DataAttributeTypes.dtString);
            columnModel.AddColumn(() => servicesModel.IsEditable, DataAttributeTypes.dtBoolean).SetHidden(true);
            columnModel.AddColumn(() => servicesModel.FromPlaning, true).SetHidden(true);
        }

        private void GetMasterGridToolbar(GridPanel gp)
        {
            if (auth.IsAdmin())
            {
                if (editable)
                {
                    gp.Toolbar()
                        .Add(
                            new UpLoadFileBtnControl
                                {
                                    Id = "btnUploadServicesPlanning",
                                    Name = "Импорт из Планирования",
                                    UploadController = UiBuilders.GetUrl<ImportsController>("ImportServicePlanning")
                                }.Build(Page)[0]);
                }
                else
                {
                    gp.Toolbar().Add(
                    new UpLoadFileBtnControl
                    {
                        Id = "btnUploadServices",
                        Name = "Импорт",
                        UploadController = UiBuilders.GetUrl<ImportsController>("ImportService2016")
                    }.Build(Page)[0]);
                }
            }

            gp.AddRefreshButton();
        }

        private Plugin GetMasterGridFilters()
        {
            var gridFilters = new GridFilters { Local = false };

            gridFilters.Filters.Add(new BooleanFilter { DataIndex = servicesModel.NameOf(() => servicesModel.BusinessStatus), YesText = @"Включена", NoText = @"Исключена" });
            gridFilters.Filters.Add(new BooleanFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefTypeName), YesText = @"Услуга", NoText = @"Работа" });
            gridFilters.Filters.Add(new BooleanFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefPayName), YesText = @"Бесплатная", NoText = @"Платная" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.InstCode), EmptyText = "Код учредителя" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefYchrName), EmptyText = "Наименование учредителя" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefOKTMOName), EmptyText = "ППО" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefActivityTypeCode), EmptyText = "Код вида деятельности" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefActivityTypeName), EmptyText = "Наименование вида деятельности" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.Regrnumber), EmptyText = "Реестровый номер" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.NameCode), EmptyText = "Код базовой услуги" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.NameName), EmptyText = "Наименование базовой услуги" });

            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.SvcCntsName1Val), EmptyText = "Наименование показателя характеристики сод1" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.SvcCntsName2Val), EmptyText = "Наименование показателя характеристики сод2" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.SvcCntsName3Val), EmptyText = "Наименование показателя характеристики сод3" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.SvcTermsName1Val), EmptyText = "Наименование показателя характеристики усл1" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.SvcTermsName2Val), EmptyText = "Наименование показателя характеристики усл2" });

            gridFilters.Filters.Add(new DateFilter { DataIndex = servicesModel.NameOf(() => servicesModel.EffectiveFrom) });
            gridFilters.Filters.Add(new DateFilter { DataIndex = servicesModel.NameOf(() => servicesModel.EffectiveBefore) });

            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.GUID), EmptyText = "Уникальный код" });
            
            gridFilters.Filters.Add(new BooleanFilter { DataIndex = servicesModel.NameOf(() => servicesModel.FromPlaning), YesText = @"Из планирования", NoText = @"Не из планирования" });

            return gridFilters;
        }

        private ToolbarBase GetMasterGridBottomToolBar(string storeID)
        {
            var toolbar = new PagingToolbar
            {
                ID = "paggingServices",
                StoreID = storeID,
                PageSize = 100,
            };

            toolbar.Items.Add(
                new NumberField
                {
                    ID = "pageSizeServices",
                    FieldLabel = @"Услуг на страницу",
                    LabelWidth = 150,
                    Width = 200,
                    Number = 100,
                    Listeners = { Change = { Handler = "#{paggingServices}.pageSize = parseInt(this.getValue());" } }
                });

            toolbar.Listeners.Change.Handler = Scope + ".ChangePage();";

            return toolbar;
        }

        #endregion

        #region DetailPanel

        private IEnumerable<Component> GetDetailPanel()
        {
            var tabPanel = UiBuilders.GetTabbedDetails(
                new List<Component>
                    {
                        GetDetailConsumersCategoryGrid(),
                        GetDetailInstitutionsInfoGrid(),
                        GetDetailIndicatorsGrid(),
                        GetDetailInstitutionTypeGrid(),
                        GetDetailLegalActGrid(),
                        GetDetailOkpdGrid(),
                        GetDetailOKVEDGrid(),
                    }) as TabPanel;
            if (tabPanel != null)
            {
                tabPanel.ID = "DetailTabPanel";
                tabPanel.Disabled = true;
                tabPanel.Height = 300;
                tabPanel.Collapsible = true;
                tabPanel.Listeners.TabChange.Handler = Scope + ".UpdateView();";
                return new List<Component> { tabPanel };
            }

            return null;
        }

        private Store GetDetailConsumersCategoryStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                ConsumersCategoryStoreID,
                false,
                typeof(Service2016ConsumersCategoryController),
                updateActionName: "Create");

            store.AddFieldsByClass(consumersCategoryModel);

            store.SetBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Store GetDetailInstitutionsInfoStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                InstitutionsInfoStoreID,
                false,
                typeof(Service2016InstitutionsInfoController),
                updateActionName: "Create");

            store.AddFieldsByClass(institutionsInfoModel);

            store.SetBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Store GetDetailIndicatorsStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                IndicatorsStoreID,
                false,
                typeof(Service2016IndicatorsController),
                updateActionName: "Create");

            store.AddFieldsByClass(indicatorsModel);

            store.SetBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Store GetDetailInstitutionTypeStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                InstitutionTypeStoreID,
                false,
                typeof(Service2016InstitutionTypeController),
                updateActionName: "Create");

            store.AddFieldsByClass(institutionTypeModel);

            store.SetBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Store GetDetailLegalActStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                LegalActStoreID,
                false,
                typeof(Service2016LegalActController),
                updateActionName: "Create");

            store.AddFieldsByClass(legalActModel);

            store.SetBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Store GetDetailOkpdStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                OkpdStoreID,
                false,
                typeof(Service2016OkpdController),
                updateActionName: "Create");

            store.AddFieldsByClass(okpdModel);

            store.SetBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Store GetDetailOKVEDStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                OKVEDStoreID,
                false,
                typeof(Service2016OKVEDController),
                updateActionName: "Create");

            store.AddFieldsByClass(okvedModel);

            store.SetBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", Scope + ".getSelectedServiceId()", ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private GridPanel GetDetailConsumersCategoryGrid()
        {
            var table = GetDetailBaseGrid(ConsumersCategoryViewID, GetDetailConsumersCategoryStore());
            table.Title = @"Категории потребителей";

            var code = table.ColumnModel.AddColumn(() => consumersCategoryModel.Code, DataAttributeTypes.dtString);
            var name = table.ColumnModel.AddColumn(() => consumersCategoryModel.Name, DataAttributeTypes.dtString);

            if (editable || auth.IsAdmin())
            {
                code.SetEditableString();
                name.SetEditableString();
            }
            
            return table;
        }

        private GridPanel GetDetailInstitutionsInfoGrid()
        {
            var table = GetDetailBaseGrid(InstitutionsInfoViewID, GetDetailInstitutionsInfoStore());
            table.Title = @"Информация об учреждениях";

            var refStructureName = table.ColumnModel.AddColumn(() => institutionsInfoModel.RefStructureName, DataAttributeTypes.dtString);
            table.ColumnModel.AddColumn(() => institutionsInfoModel.RefStructureInn, DataAttributeTypes.dtString);

            if (editable || auth.IsAdmin())
            {
                refStructureName.SetComboBoxEditor(
                    D_Org_Structure.Key,
                    Page,
                    new Dictionary<string, string[]>
                        {
                            { institutionsInfoModel.NameOf(() => institutionsInfoModel.RefStructure), new[] { "ID" } },
                            { institutionsInfoModel.NameOf(() => institutionsInfoModel.RefStructureInn), new[] { "INN" } },
                            { institutionsInfoModel.NameOf(() => institutionsInfoModel.RefStructureName), new[] { "Name" } },
                        },
                    UiBuilders.GetUrl<Service2016InstitutionsInfoController>("GetStructures"));
            }

            return table;
        }

        private GridPanel GetDetailIndicatorsGrid()
        {
            var table = GetDetailBaseGrid(IndicatorsViewID, GetDetailIndicatorsStore());
            table.Title = @"Показатели";

            var code = table.ColumnModel.AddColumn(() => indicatorsModel.Code, DataAttributeTypes.dtString);
            var name = table.ColumnModel.AddColumn(() => indicatorsModel.Name, DataAttributeTypes.dtString);
            var refIndTypeName = table.ColumnModel.AddColumn(() => indicatorsModel.RefIndTypeName, DataAttributeTypes.dtString);
            var refOkeiName = table.ColumnModel.AddColumn(() => indicatorsModel.RefOKEIName, DataAttributeTypes.dtString);

            if (editable || auth.IsAdmin())
            {
                code.SetEditableString().SetMaxLengthEdior(3);
                name.SetEditableString();

                refIndTypeName.SetComboBoxEditor(FX_FX_CharacteristicType.Key, Page, indicatorsModel.NameOf(() => indicatorsModel.RefIndType), null, null, false);

                refOkeiName.SetComboBoxEditor(
                    D_Org_OKEI.Key,
                    Page,
                    new Dictionary<string, string[]>
                        {
                            { indicatorsModel.NameOf(() => indicatorsModel.RefOKEI), new[] { "ID" } },
                            { indicatorsModel.NameOf(() => indicatorsModel.RefOKEIName), new[] { "Name" } },
                        },
                    UiBuilders.GetUrl<CommonDataController>("GetOkei"));
            }

            return table;
        }

        private GridPanel GetDetailInstitutionTypeGrid()
        {
            var table = GetDetailBaseGrid(InstitutionTypeViewID, GetDetailInstitutionTypeStore());
            table.Title = @"Виды учреждений";

            var code = table.ColumnModel.AddColumn(() => institutionTypeModel.Code, DataAttributeTypes.dtString);
            var name = table.ColumnModel.AddColumn(() => institutionTypeModel.Name, DataAttributeTypes.dtString);

            if (editable || auth.IsAdmin())
            {
                code.SetEditableString();
                name.SetEditableString();
            }

            return table;
        }

        private GridPanel GetDetailLegalActGrid()
        {
            var table = GetDetailBaseGrid(LegalActViewID, GetDetailLegalActStore());
            table.Title = @"НПА";

            var name = table.ColumnModel.AddColumn(() => legalActModel.Name, DataAttributeTypes.dtString);
            var kind = table.ColumnModel.AddColumn(() => legalActModel.Kind, DataAttributeTypes.dtString);
            var lanumber = table.ColumnModel.AddColumn(() => legalActModel.LANumber, DataAttributeTypes.dtString);
            var approvedby = table.ColumnModel.AddColumn(() => legalActModel.ApprovedBy, DataAttributeTypes.dtString);
            var apprvdat = table.ColumnModel.AddColumn(() => legalActModel.ApprvdAt, DataAttributeTypes.dtDate);
            var datetend = table.ColumnModel.AddColumn(() => legalActModel.DatetEnd, DataAttributeTypes.dtDate);
            var effectivefrom = table.ColumnModel.AddColumn(() => legalActModel.EffectiveFrom, DataAttributeTypes.dtDate);
            var mjnumber = table.ColumnModel.AddColumn(() => legalActModel.MJnumber, DataAttributeTypes.dtString);
            var mjregdate = table.ColumnModel.AddColumn(() => legalActModel.MJregdate, DataAttributeTypes.dtDate);

            if (editable || auth.IsAdmin())
            {
                name.SetEditableString();
                kind.SetEditableString();
                lanumber.SetEditableString();
                approvedby.SetEditableString();
                apprvdat.SetEditableDate();
                datetend.SetEditableDate();
                effectivefrom.SetEditableDate();
                mjnumber.SetEditableString();
                mjregdate.SetEditableDate();
            }

            return table;
        }

        private GridPanel GetDetailOkpdGrid()
        {
            var table = GetDetailBaseGrid(OkpdViewID, GetDetailOkpdStore());
            table.Title = @"ОКПД";

            var code = table.ColumnModel.AddColumn(() => okpdModel.Code, DataAttributeTypes.dtString);
            var name = table.ColumnModel.AddColumn(() => okpdModel.Name, DataAttributeTypes.dtString);

            if (editable || auth.IsAdmin())
            {
                code.SetEditableString();
                name.SetEditableString();
            }

            return table;
        }

        private GridPanel GetDetailOKVEDGrid()
        {
            var table = GetDetailBaseGrid(OKVEDViewID, GetDetailOKVEDStore());
            table.Title = @"ОКВЭД";

            var code = table.ColumnModel.AddColumn(() => okvedModel.Code, DataAttributeTypes.dtString);
            var name = table.ColumnModel.AddColumn(() => okvedModel.Name, DataAttributeTypes.dtString);

            if (editable || auth.IsAdmin())
            {
                code.SetEditableString();
                name.SetEditableString();
            }

            return table;
        }

        private GridPanel GetDetailBaseGrid(string gridID, Store store)
        {
            var table = UiBuilders.CreateGridPanel(gridID, store);

            table.AddRefreshButton();

            if ((!auth.IsSpectator() && editable) || auth.IsAdmin())
            {
                table.AddNewRecordNoEditButton();
                table.AddDeleteRecordWithConfirmButton();
                table.AddSaveButton();
            }

            return table;
        }

        #endregion
    }
}
