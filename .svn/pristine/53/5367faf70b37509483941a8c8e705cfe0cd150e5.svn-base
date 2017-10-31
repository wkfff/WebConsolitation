using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Helpers;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Models;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Presentation.Views
{
    public class AreasView : View
    {
        private const string DatasourceAreasListID = "dsAreas";
        private const string GridpanelAreasListID = "gpAreas";

        private IUserCredentials userCredentials;

        public AreasView(IUserCredentials userCredentials)
        {
            this.userCredentials = userCredentials;
        }

        public override List<Component> Build(ViewPage page)
        {
            var isInAvailableRoles = this.userCredentials.IsInRole(InvAreaRoles.Creator) || this.userCredentials.IsInRole(InvAreaRoles.Coordinator);
            if (!isInAvailableRoles)
            {
                return new List<Component> { new DisplayField { Text = "Данный пользователь не входит в группы 'Создатели' или 'Координаторы'. Представление недоступно." } };
            }

            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("AreasView", Resource.AreasView);
            }

            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                                {
                                    new BorderLayout
                                    {   
                                        North = { Items = { CreateTopPanel() }, Margins = { Bottom = 5 } },
                                        Center = { Items = { CreateAreasListPanel() }, Margins = { Bottom = 3 } }
                                    }
                                }
            };

            return new List<Component> { view, GetDetailWindow(), GetGoogleMapsWindow() };
        }

        private Panel CreateTopPanel()
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnAdd",
                Icon = Icon.Add,
                ToolTip = "Добавить",
                Visible = true,
                Listeners = { Click = { Handler = "editCard(null, 'новая');" } }
            });

            toolbar.Add(new Button
            {
                ID = "btnDelete",
                Icon = Icon.Delete,
                Disabled = true,
                Visible = true,
                ToolTip = "Удалить",
                Listeners = { Click = { Handler = "DeleteProjectSelectedRow('{0}');".FormatWith(GridpanelAreasListID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceAreasListID) } }
            });

            toolbar.Add(new ToolbarSpacer(50));

            toolbar.Add(new Button
            {
                ID = "btnFilterEditable",
                Icon = Icon.UserEdit,
                ToolTip = "Редактируемые",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });

            toolbar.Add(new Button
            {
                ID = "btnFilterReview",
                Icon = Icon.UserEarth,
                ToolTip = "На рассмотрении",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });

            toolbar.Add(new Button
            {
                ID = "btnFilterAccepted",
                Icon = Icon.UserTick,
                ToolTip = "Принят",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });

            toolbar.Add(new ToolbarSpacer(50));
            toolbar.Add(new Button
            {
                ID = "btnWatchGoogleMaps",
                Icon = Icon.World,
                Disabled = true,
                Text = "Посмотреть на карте",
                Listeners = { Click = { Handler = "ShowGoogleMap('{0}');".FormatWith(GridpanelAreasListID) } }
            });
            
            toolbar.AddScript(@"function toggleFilter() {{ {0}.load(); }};".FormatWith(DatasourceAreasListID));
            
            return new Panel
            {
                ID = "topPanel",
                Height = 27,
                Border = false,
                TopBar = { toolbar }
            };
        }

        private List<Component> CreateAreasListPanel()
        {
            GridPanel table = new GridPanel
            {
                ID = GridpanelAreasListID,
                Store = { GetAreasListStore() },
                Border = false,
                TrackMouseOver = true,
                Icon = Icon.DatabaseTable,
                LoadMask = { ShowMask = true },
                ColumnModel =
                {
                    Columns =
                    {   
                        new Column { Hidden = true, DataIndex = "ID", MenuDisabled = true },
                        new ImageCommandColumn
                            {
                                Width = 30,
                                Commands  = { new ImageCommand { Icon = Icon.Pencil, CommandName  = "EditCard", ToolTip = { Text = "Открыть карточку" } } }
                            },
                        new Column { DataIndex = "Status", Width = 30, Renderer = new Renderer("return getUrlForStatus(value);") },
                        new Column { Header = "Регистрационный номер", DataIndex = "RegNumber", Width = 180, Align = Alignment.Left },
                        new Column { Header = "Муниципальное образование", DataIndex = "TerritoryName", Width = 200, Align = Alignment.Left },
                        new Column { Header = "Местоположение", DataIndex = "Location", Width = 200, Align = Alignment.Left },
                        new Column { Header = "Кадастровый номер", DataIndex = "CadNumber", Width = 200, Align = Alignment.Left },
                        new DateColumn { Header = "Дата создания", DataIndex = "CreatedDate", Width = 200, Align = Alignment.Left, Format = "d-m-Y H:mm" },
                        new DateColumn { Header = "Дата принятия", DataIndex = "AdoptionDate", Width = 200, Align = Alignment.Left, Format = "d-m-Y H:mm" },
                        new Column { Header = "Автор", DataIndex = "CreateUser", Width = 100, Align = Alignment.Left, Css = "color:#AAAAAA;" }
                    }
                },
                SelectionModel =
                {
                    new RowSelectionModel
                        {
                            ID = "RowSelectionModel1", 
                            SingleSelect = true,
                            Listeners = 
                            {
                                RowSelect = { Handler = "btnDelete.enable(); btnWatchGoogleMaps.enable();" },
                                RowDeselect = { Handler = "if(!{0}.hasSelection()){{btnDelete.disable();btnWatchGoogleMaps.disable();}}".FormatWith(GridpanelAreasListID) }
                            }
                        }
                },
                Listeners = { Command = { Handler = "if (command != undefined && command=='EditCard'){{editCard(record.data.ID, record.data.RegNumber);}}" } }
            };

            return new List<Component>
                       {
                           new Hidden { ID = "UrlIconStatusEdit", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit), InvalidText = "На редактировании" },
                           new Hidden { ID = "UrlIconStatusReview", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEarth), InvalidText = "На рассмотрение" },
                           new Hidden { ID = "UrlIconStatusAcceptred", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick), InvalidText = "Принят" },
                           table
                       };
        }

        private Store GetAreasListStore()
        {
            var store = new Store
            {
                ID = DatasourceAreasListID,
                Restful = true,
                ShowWarningOnFailure = true,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };

            store.Proxy.Add(new HttpProxy
            {
                RestAPI =
                {
                    ReadUrl = "/Areas/GetAreasTable",
                    DestroyUrl = "/Areas/DeleteArea",
                    CreateUrl = "/Areas/Create", ////просто заглушка, метода нет
                    UpdateUrl = "/Areas/Update", ////просто заглушка, метода нет
                }
            });
            store.BaseParams.Add(new Parameter("projStatusFilters", "getStatusFilters()", ParameterMode.Raw));
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Status"),
                    new RecordField("RegNumber"),
                    new RecordField("TerritoryName"),
                    new RecordField("Location"),
                    new RecordField("CadNumber"),
                    new RecordField("CreatedDate", RecordFieldType.Date),
                    new RecordField("AdoptionDate", RecordFieldType.Date),
                    new RecordField("CreateUser")
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка площадок', response.responseText);";
            store.Listeners.SaveException.Handler = "Ext.Msg.alert('Ошибка при сохранение', response.responseText);";
            store.Listeners.Save.Handler = "Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Сохранение.', hideDelay  : 2500});";
            store.Listeners.Exception.Handler = @"
            if (response.raw != undefined && response.raw.message != undefined){
               Ext.Msg.alert('Ошибка при сохранение', response.raw.message);
            }else{
               Ext.Msg.alert('Ошибка', response.responseText);
            }
            ";
            store.Listeners.Load.Handler = "RowSelectionModel1.fireEvent('RowDeselect');";
            return store;
        }

        private Window GetDetailWindow()
        {
            Window win = new Window
                             {   
                                 ID = "wAreaDetail",
                                 Hidden = true,
                                 Title = "Карточка",
                                 Width = 500,
                                 MaxWidth = 1000,
                                 MinWidth = 200,
                                 Height = 500,
                                 MinHeight = 300,
                                 Modal = true,
                                 CloseAction = CloseAction.Hide,
                                 AutoLoad =
                                {
                                    Url = "/View/InvArDetail",
                                    Mode = LoadMode.IFrame,
                                    TriggerEvent = "show",
                                    Params = { new Parameter("id", "null", ParameterMode.Raw) },
                                    ShowMask = true,
                                    MaskMsg = "Загрузка..."
                                },
                                 Listeners =
                                {
                                    BeforeShow =
                                    {
                                        Handler = @"
var size = Ext.getBody().getSize(); 
var w = size.width * 0.9;
if (w>1000){w=1000};
this.setSize({ width: w, height: size.height * 0.9});
if (wAreaDetail.iframe != undefined){
  wAreaDetail.reload();
}
"
                                    },
                                    Hide = { Handler = "wAreaDetail.getBody().areaDetailForm.reset()" }
                                }
                             };
            return win;
        }

        private Window GetGoogleMapsWindow()
        {
            Window win = new Window
            {
                ID = "wGoogleMap",
                Hidden = true,
                Title = "Карта",
                Width = 800,
                MaxWidth = 1200,
                MinWidth = 200,
                Height = 500,
                MinHeight = 300,
                Modal = true,
                CloseAction = CloseAction.Hide,
                AutoLoad =
                {
                    Url = "/View/InvArMap",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ReloadOnEvent = true,
                    Params = { new Parameter("id", "null", ParameterMode.Raw) },
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                },
                Listeners =
                {
                    ////BeforeShow =
                    ////{
                    ////    Handler = "if (wGoogleMap.iframe != undefined){ wGoogleMap.reload();}"
                    ////},
                    Hide = { Handler = "wGoogleMap.clearContent();" }
                }
            };
            return win;
        }
    }
}
