using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls.SearchCombobox;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class StateSystemView : View
    {
        private const string MASTER_GRID_ID = "SchemStateTransitions";
        private const string STATES_GRID_ID = "States";
        private const string TRANSITIONS_GRID_ID = "Transitions";
        private const string RIGHTS_TRANSITION_GRID_ID = "RightsTransition";
        private const string OPTIONS_TRANSITION_GRID_ID = "OptionsTransition";

        private const string VIEW_CONTROLLER = "StateSystemView";

        private const string SCOPE = "E86n.View.StateSystem";

        public ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            # region Icons For ComboBox

            const string Style = @"
                    .icon-combo-item {
                    background-repeat: no-repeat ! important;
                    background-position: 3px 50% ! important;
                    padding-left: 24px ! important;
                    }
                  ";

            ResourceManager.GetInstance(page).RegisterClientStyleBlock("CustomStyle", Style);

            var iconStore = new Store
                {
                    ID = "IconStore",
                    Reader =
                        {
                            new ArrayReader
                                {
                                    Fields =
                                        {
                                            new RecordField {Name = "iconCls"},
                                            new RecordField {Name = "name"}
                                        }
                                }
                        }
                };

            var array = new object[Enum.GetNames(typeof (Icon)).GetLength(0)];
            int i = 0;

            foreach (string icoName in Enum.GetNames(typeof (Icon)))
            {
                var item = new object[]
                    {ResourceManager.GetIconClassName((Icon) Enum.Parse(typeof (Icon), icoName)), icoName};
                array[i++] = item;
                ResourceManager.GetInstance(page).RegisterIcon((Icon) Enum.Parse(typeof (Icon), icoName));
            }

            iconStore.DataSource = array;
            iconStore.DataBind();
            Page.Controls.Add(iconStore);

            # endregion

            var stateSystemService = Resolver.Get<IStateSystemService>();

            var roleStore = new Store
                {
                    ID = "RoleStore",
                    Reader =
                        {
                            new ArrayReader
                                {
                                    Fields =
                                        {
                                            new RecordField {Name = "role"},
                                            new RecordField {Name = "roleName"}
                                        }
                                }
                        },
                    DataSource = new object[]
                        {
                            new object[] {stateSystemService.Admin, "Администратор"},
                            new object[] {stateSystemService.Ppo, "ППО"},
                            new object[] {stateSystemService.Grbs, "ГРБС"},
                            new object[] {stateSystemService.User, "Учреждение"}
                        }
                };

            roleStore.DataBind();
            Page.Controls.Add(roleStore);

            var transitionClassStore = new Store
                {
                    ID = "TransitionClassStore",
                    Reader =
                        {
                            new ArrayReader
                                {
                                    Fields =
                                        {
                                            new RecordField {Name = "Class"},
                                            new RecordField {Name = "ClassName"}
                                        }
                                }
                        },
                    DataSource = new object[]
                        {
                            new object[] {"Base", "Базовый"},
                            new object[] {"Export", "Экспорт"},
                            new object[] {"WithDialog", "С вызовом диалога"}
                        }
                };

            transitionClassStore.DataBind();
            Page.Controls.Add(transitionClassStore);

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StateSystemView",
                                                                            Resource.StateSystemView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            RestActions restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            Store masterstore = GetMasterStore();
            Page.Controls.Add(masterstore);

            Store statesdetailstore = GetStatesDetailStore();
            Page.Controls.Add(statesdetailstore);

            Store statesLp = CUtils.GetLookupStore("StatesLPStore",
                                                   "/{0}/GetStatesLp".FormatWith(VIEW_CONTROLLER));

            statesLp.SetBaseParams("masterId", SCOPE + ".getMasterID('{0}')".FormatWith(MASTER_GRID_ID),
                                                                                        ParameterMode.Raw);
            Page.Controls.Add(statesLp);

            Store transitionsdetailstore = GetTransitionsDetailStore();
            Page.Controls.Add(transitionsdetailstore);

            Store rightstransition = GetRightsTransitionDetailStore();
            Page.Controls.Add(rightstransition);

            Store transitionsLp = CUtils.GetLookupStore("TransitionLPStore",
                                                        "/{0}/TransitionLpStoreRead".FormatWith(VIEW_CONTROLLER));

            transitionsLp.SetBaseParams("masterId", SCOPE + ".getMasterID('{0}')".FormatWith(MASTER_GRID_ID),
                                        ParameterMode.Raw);
            Page.Controls.Add(transitionsLp);

            Store optionstransition = GetOptionsTransitionDetailStore();
            Page.Controls.Add(optionstransition);

            Page.Controls.Add(GetOptionsTransitionWnd(GetOptionsTransitionDetail(optionstransition,
                                            rightstransition)));

            var details = new List<Component>
                {
                    GetStatesDetail(STATES_GRID_ID, statesdetailstore),
                    GetTransitionsDetail(transitionsdetailstore)
                };

            var tpDetails = UiBuilders.GetTabbedDetails(details) as TabPanel;
            if (tpDetails != null)
            {
                tpDetails.ID = "DetailsTabPanel";
                tpDetails.Disabled = true;
                tpDetails.Listeners.TabChange.Fn(SCOPE, "reloadDetail");
            }

            var view = new Viewport
            {
                Items =
                            {
                                new BorderLayout
                                    {
                                        North = {Items = {GetMasterGrid(masterstore)}, Split = true},
                                        Center = {Items = {tpDetails}}
                                    }
                            }
            };

            return new List<Component> {view};
        }

        private Store GetMasterStore()
        {
            Store masterStore = StoreExtensions.StoreCreateDefault(MASTER_GRID_ID + "Store", true, VIEW_CONTROLLER,
                                                              "SchemTransitionsRead",
                                                              "SchemTransitionsCreate",
                                                              "SchemTransitionsUpdate",
                                                              "SchemTransitionsDelete");
            masterStore.AddField("ID");
            masterStore.AddField("Name");
            masterStore.AddField("Note");
            masterStore.AddField("InitAction");
            masterStore.AddField("RefPartDoc");
            masterStore.AddField("RefPartDocName");

            return masterStore;
        }

        private Store GetStatesDetailStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(STATES_GRID_ID + "Store", true, VIEW_CONTROLLER,
                                                        "StatesRead",
                                                        "StatesCreate",
                                                        "StatesUpdate",
                                                        "StatesDelete");
            store.AddField("ID");
            store.AddField("RefStates");
            store.AddField("RefStatesName");
            store.AddField("IsStart", new RecordField.Config {DefaultValue = "false"});

            store.SetBaseParams("masterId", SCOPE + ".getMasterID('{0}')".FormatWith(MASTER_GRID_ID), 
                                    ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", SCOPE + ".getMasterID('{0}')".FormatWith(MASTER_GRID_ID),
                                        ParameterMode.Raw);

            return store;
        }

        private Store GetTransitionsDetailStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(TRANSITIONS_GRID_ID + "Store", true, VIEW_CONTROLLER,
                                                        "TransitionsRead",
                                                        "TransitionsCreate",
                                                        "TransitionsUpdate",
                                                        "TransitionsDelete");
            store.AddField("ID");
            store.AddField("Name");
            store.AddField("Ico");
            store.AddField("Action");
            store.AddField("Note");
            store.AddField("RefSchemStates");
            store.AddField("RefSchemStatesName");
            store.AddField("TransitionClass");
            store.AddField("TransitionClassName");

            store.SetBaseParams("masterId", SCOPE + ".getMasterID('{0}')".FormatWith(MASTER_GRID_ID),
                                    ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", SCOPE + ".getMasterID('{0}')".FormatWith(MASTER_GRID_ID),
                                     ParameterMode.Raw);

            return store;
        }

        private Store GetRightsTransitionDetailStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(RIGHTS_TRANSITION_GRID_ID + "Store", true, VIEW_CONTROLLER,
                                                        "RightsTransitionRead",
                                                        "RightsTransitionCreate",
                                                        "RightsTransitionUpdate",
                                                        "RightsTransitionDelete");
            store.AddField("ID");
            store.AddField("AccountsRole");
            store.AddField("AccountsRoleName");

            store.AutoLoad = false;

            store.SetBaseParams("masterId", SCOPE + ".getMasterID('{0}')".FormatWith(OPTIONS_TRANSITION_GRID_ID),
                                ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", SCOPE + ".getMasterID('{0}')"
                                        .FormatWith(OPTIONS_TRANSITION_GRID_ID), ParameterMode.Raw);

            return store;
        }

        private Store GetOptionsTransitionDetailStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(OPTIONS_TRANSITION_GRID_ID + "Store", true, VIEW_CONTROLLER,
                                                        "OptionsTransitionRead",
                                                        "OptionsTransitionCreate",
                                                        "OptionsTransitionUpdate",
                                                        "OptionsTransitionDelete");
            store.AddField("ID");
            store.AddField("RefTransitions");
            store.AddField("RefTransitionsName");

            store.AutoLoad = false;

            return store;
        }


        private Component GetMasterGrid(Store store)
        {
            GridPanel gp = CreateGridPanel(MASTER_GRID_ID, store);

            ToolbarBase tb = gp.Toolbar();

            var export = new UpLoadFileBtnControl
                {
                    Id = "btnExport",
                    Name = "Выгрузка в XML",
                    Icon = Icon.DiskDownload,
                    Upload = false,
                    UploadController = "/StateSystemView/Export",
                    Params = {{"recId", SCOPE + ".getMasterID('{0}')".FormatWith(MASTER_GRID_ID)}}
                };

            tb.Add(export.Build(Page));

            var import = new UpLoadFileBtnControl
                {
                    Name = "Загрузка из XML",
                    UploadController = "/StateSystemView/Import",
                    SuccessHandler = @"
                                                Ext.MessageBox.hide();
                                                Ext.net.Notification.show({iconCls : 'icon-information',
                                                                           html : result.extraParams.msg,
                                                                           title : 'Уведомление',
                                                                           hideDelay : 2500});
                                                SchemStateTransitionsStore.reload();"
                };
            tb.Add(import.Build(Page));

            tb.Add(new ToolbarSeparator());

            gp.AddRefreshButton();
            gp.AddNewRecordButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.Height = 500;
            gp.ColumnModel.AddColumn("Name", "Наименование схемы состояний", DataAttributeTypes.dtString).
                SetEditableString().SetWidth(300);
            gp.ColumnModel.AddColumn("InitAction", "Контроллер инициализации(URL)", DataAttributeTypes.dtString).
                SetEditableString().SetWidth(200);
            gp.ColumnModel.AddColumn("RefPartDocName", "Тип документа", DataAttributeTypes.dtString)
                .SetWidth(300)
                .AddLookupEditorForColumn(
                    "RefPartDoc",
                    "RefPartDocName",
                    "/Entity/DataWithCustomSearch?objectKey={0}"
                                         .FormatWith(FX_FX_PartDoc.Key),
                    true,
                    Page);
            gp.ColumnModel.AddColumn("Note", "Описание схемы состояний", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(500);

            gp.Plugins.Add(new GridFilters
                {
                    Local = true,
                    Filters =
                        {
                            new NumericFilter {DataIndex = "ID"},
                            new StringFilter {DataIndex = "Name"},
                            new StringFilter {DataIndex = "RefPartDocName"},
                            new StringFilter {DataIndex = "Note"},
                        }
                });

            gp.AddColumnsWrapStylesToPage(Page);

            ((RowSelectionModel) gp.GetSelectionModel()).Listeners.RowSelect.Handler = SCOPE +
                                                                    ".RowSelect('MasterGrid', record);";
            return gp;
        }

        private Component GetStatesDetail(string id, Store store)
        {
            GridPanel gp = CreateGridPanel(id, store);
            gp.Title = @"Состояния системы";

            gp.AddRefreshButton();
            gp.AddNewRecordButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.Columns.Add(new CommandColumn
                {
                    Width = 75,
                    Header = "Переходы",
                    ButtonAlign = Alignment.Center,
                    Commands =
                        {
                            new GridCommand
                                {
                                    Icon = Icon.Pencil,
                                    CommandName = "EditTransitions",
                                    ToolTip = {Text = "Редактировать переходы"}
                                }
                        },
                    PrepareToolbar = {Fn = SCOPE + ".prepareToolbar"}
                });

            gp.Listeners.Command.Fn(SCOPE, "cmdHandler");

            gp.ColumnModel.AddColumn("RefStatesName", "Наименование состояния", DataAttributeTypes.dtString)
                .SetWidth(200)
                .AddLookupEditorForColumn("RefStates", "RefStatesName",
                    "/Entity/DataWithCustomSearch?objectKey={0}".FormatWith(FX_Org_SostD.Key), true, Page);

            gp.ColumnModel.AddColumn("IsStart", "Начальное состояние", DataAttributeTypes.dtBoolean)
                .SetEditableBoolean().SetWidth(150);

            gp.Plugins.Add(new GridFilters
                {
                    Local = true,
                    Filters =
                        {
                            new NumericFilter {DataIndex = "ID"},
                            new StringFilter {DataIndex = "RefStatesName"},
                        }
                });
            return gp;
        }

        private Component GetTransitionsDetail(Store store)
        {
            GridPanel gp = CreateGridPanel(TRANSITIONS_GRID_ID, store);
            gp.Title = @"Переходы";

            gp.AddRefreshButton();
            gp.AddNewRecordButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn("Name", "Наименование перехода", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(150);
            gp.ColumnModel.AddColumn("Note", "Описание перехода", DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(300);
            gp.ColumnModel.AddColumn("Action", "Контроллер выполнения смены перехода(URL)",
                                        DataAttributeTypes.dtString).SetEditableString().SetWidth(265);
            gp.ColumnModel.AddColumn("TransitionClassName", "Тип перехода", DataAttributeTypes.dtString)
                .SetWidth(150).SetLookup("TransitionClass", "TransitionClassStore", "Class", "ClassName");
            gp.ColumnModel.AddColumn("RefSchemStatesName", "Конечное состояние перехода", 
                                      DataAttributeTypes.dtString).SetWidth(180)
                                      .SetLookup("RefSchemStates", "StatesLPStore", "ID", "Name");

            ComboBox editor = GetIconComboBox();

            gp.ColumnModel.Columns.Add(new Column
                {
                    ColumnID = "Ico",
                    DataIndex = "Ico",
                    Header = "Иконка",
                    Width = 160,
                    RightCommandAlign = false,
                    Commands = {new ImageCommand {CommandName = "Ico"}},
                    PrepareCommand = {Fn = SCOPE + ".prepareCellCommand"},
                    Editor = {editor}
                });

            gp.Plugins.Add(new GridFilters
                {
                    Local = true,
                    Filters =
                        {
                            new NumericFilter {DataIndex = "ID"},
                            new StringFilter {DataIndex = "Name"},
                            new StringFilter {DataIndex = "Note"},
                            new StringFilter {DataIndex = "Action"},
                            new StringFilter {DataIndex = "TransitionClassName"},
                            new StringFilter {DataIndex = "RefStatesName"}
                        }
                });
            return gp;
        }

        private Component GetOptionsTransitionDetail(Store optionsTransitionGridStore, 
                                                        Store rightsTransitionGridStore)
        {
            GridPanel optionsTransitionGrid = CreateGridPanel(OPTIONS_TRANSITION_GRID_ID, 
                                                                optionsTransitionGridStore);

            optionsTransitionGrid.AddRefreshButton();
            optionsTransitionGrid.AddNewRecordButton();
            optionsTransitionGrid.AddRemoveRecordButton();
            optionsTransitionGrid.AddSaveButton();

            optionsTransitionGrid.ColumnModel.AddColumn("RefTransitionsName", "Переходы", 
                                            DataAttributeTypes.dtString).SetWidth(150)
                                            .SetLookup("RefTransitions", "TransitionLPStore", "ID", "Name");

            ((RowSelectionModel) optionsTransitionGrid.GetSelectionModel()).Listeners.RowSelect
                                 .Handler = SCOPE + ".RowSelect('OptionsTransition', record);";

            GridPanel rightsTransitionGrid = CreateGridPanel(RIGHTS_TRANSITION_GRID_ID, rightsTransitionGridStore);
            rightsTransitionGrid.Disabled = true;
            rightsTransitionGrid.Title = @"Права на переход(если не указано то для всех разрешен)";

            rightsTransitionGrid.AddRefreshButton();
            rightsTransitionGrid.AddNewRecordButton();
            rightsTransitionGrid.AddRemoveRecordButton();
            rightsTransitionGrid.AddSaveButton();

            rightsTransitionGrid.ColumnModel.AddColumn("AccountsRoleName", "Роль пользователя",
                                                       DataAttributeTypes.dtString).SetWidth(150).SetLookup(
                                                           "AccountsRole", "RoleStore", "role", "roleName");

            return new RowLayout
                {
                    Split = true,
                    Rows =
                        {
                            new LayoutRow
                                {
                                    RowHeight = 0.5m,
                                    Items =
                                        {
                                            optionsTransitionGrid
                                        }
                                },
                            new LayoutRow
                                {
                                    RowHeight = 0.5m,
                                    Items =
                                        {
                                            rightsTransitionGrid
                                        }
                                }
                        }
                };
        }

        private Component GetOptionsTransitionWnd(Component grid)
        {
            Window wnd = GetWindow("wndOptionsTransition");
            wnd.Title = @"Настройка переходов для состояния";
            wnd.Width = 280;
            wnd.Height = 500;

            wnd.Items.Add(grid);

            var btn = new Button {ID = "btn" + wnd.ID, Text = @"Закрыть"};
            btn.Listeners.Click.Fn(SCOPE, "wndOptionsTransitionBtnOk");
            wnd.Buttons.Add(btn);

            return wnd;
        }

        #region Services

        private Window GetWindow(string id)
        {
            return new Window
                {
                    ID = id,
                    Hidden = true,
                    Title = "",
                    Width = 280,
                    MinWidth = 280,
                    Height = 300,
                    MinHeight = 200,
                    Modal = true,
                    Layout = LayoutType.Fit.ToString(),
                    MonitorResize = true,
                    CloseAction = CloseAction.Hide
                };
        }

        private ComboBox GetIconComboBox()
        {
            var cb = new ComboBox
                {
                    StoreID = "IconStore",
                    DisplayField = "name",
                    ValueField = "name",
                    Mode = DataLoadMode.Local,
                    TriggerAction = TriggerAction.All,
                    Editable = false,
                    EmptyText = @"Выберите иконку",
                };

            cb.Template.Html = @"<tpl for=""."">
                                    <div class=""x-combo-list-item icon-combo-item {iconCls}"">
                                     {name}
                                     </div>
                                    </tpl>";
            return cb;
        }

        private GridPanel CreateGridPanel(string id, Store store)
        {
            var table = new GridPanel
                {
                    ID = id,
                    StoreID = store.ID,
                    Border = false,
                    TrackMouseOver = true,
                    AutoScroll = true,
                    Icon = Icon.DatabaseTable,
                    LoadMask = {ShowMask = true, Msg = "Загрузка..."},
                    Layout = LayoutType.Fit.ToString(),
                    ClearEditorFilter = false,
                    StripeRows = true,
                    Stateful = true,
                };

            var selMod = new RowSelectionModel {SingleSelect = true};

            table.SelectionModel.Add(selMod);

            table.ColumnModel.AddColumn("ID", "ID", DataAttributeTypes.dtInteger).SetHidden(true);

            return table;
        }

        # endregion
    }
}