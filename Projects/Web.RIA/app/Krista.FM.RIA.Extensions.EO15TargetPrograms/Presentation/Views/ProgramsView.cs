using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Common.Constants;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Views
{
    public class ProgramsView : View
    {
        private const string DatasourceProgramsID = "dsPrograms";
        private const string GridpanelProgramsID = "gpPrograms";

        private readonly IExtension extension;

        public ProgramsView(IExtension extension)
        {
            this.extension = extension;
        }

        public override List<Component> Build(ViewPage page)
        {
            var isInAvailableRoles = User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer);
            if (!isInAvailableRoles)
            {
                return new List<Component> { new DisplayField { Text = "Данный пользователь не входит в группы 'ЭО Целевых программ' или 'Координаторы Целевых программ'. Представление недоступно." } };
            }
            
            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("ProgramsView", Resource.ProgramsView);
            }
            
            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                                {
                                    new BorderLayout
                                    {   
                                        North = { Items = { CreateTopPanel() }, Margins = { Bottom = 5 } },
                                        Center = { Items = { CreateProgramsListPanel() }, Margins = { Bottom = 3 } }
                                    }
                                }
            };

            return new List<Component> { view, GetProgramWindow(), GetFinanceReportWindow(), GetProgramExecutingReportWindow(), GetEffectivityEstimateReportWindow() };
        }

        private Panel CreateTopPanel()
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceProgramsID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnAdd",
                Icon = Icon.Add,
                ToolTip = "Добавить",
                Hidden = !User.IsInRole(ProgramRoles.Creator),
                Listeners = { Click = { Handler = "openProgram(null, 'новая');" } }
            });

            toolbar.Add(new Button
            {
                ID = "btnDelete",
                Icon = Icon.Delete,
                Disabled = true,
                Hidden = !User.IsInRole(ProgramRoles.Creator),
                ToolTip = "Удалить",
                Listeners = { Click = { Handler = "deleteProgramSelectedRow('{0}');".FormatWith(GridpanelProgramsID) } }
            });

            toolbar.Add(new ToolbarSpacer(50));

            var regionVisibility = extension.OKTMO != OKTMO.Sakhalin;

            toolbar.Add(new Button
            {
                ID = "btnFilterTypeDCP",
                Text = "ЦП",
                ToolTip = "Долгосрочные целевые программы",
                Width = 30,
                Hidden = !regionVisibility,
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });
            
            toolbar.Add(new Button
            {
                ID = "btnFilterTypeVCP",
                Text = "ВЦП",
                ToolTip = "Ведомственные целевые программы",
                Width = 30,
                Hidden = !regionVisibility,
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });

            toolbar.Add(new ToolbarSpacer
            {
                Width = 30,
                Hidden = !regionVisibility,
            });

            toolbar.Add(new Button
            {
                ID = "btnFilterUnapproved",
                Icon = Icon.UserEdit,
                ToolTip = "Проект",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });

            toolbar.Add(new Button
            {
                ID = "btnFilterApproved",
                Icon = Icon.UserEarth,
                ToolTip = "Утверждена",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });
            toolbar.Add(new Button
            {
                ID = "btnFilterRunning",
                Icon = Icon.UserStar,
                ToolTip = "Действующая",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });

            toolbar.Add(new Button
            {
                ID = "btnFilterFinished",
                Icon = Icon.UserTick,
                ToolTip = "Завершена",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter"
            });

            toolbar.AddScript(@"
function toggleFilter() {{ 
  {0}.load(); 
}};
".FormatWith(DatasourceProgramsID));

            return new Panel
            {
                ID = "topPanel",
                Height = 27,
                Border = false,
                TopBar = { toolbar }
            };
        }

        private List<Component> CreateProgramsListPanel()
        {
            var table = new GridPanel
            {
                ID = GridpanelProgramsID,
                Icon = Icon.DatabaseTable,
                Store = { GetProgramsListStore() },
                Border = false,
                TrackMouseOver = true,
                AutoScroll = true,
                LoadMask = { ShowMask = true },
                ColumnModel = 
                { 
                    Columns = 
                    {
                        new Column { Hidden = true, DataIndex = "ID" },
                        new Column { Hidden = true, DataIndex = "ParentId" },
                        new ImageCommandColumn
                            {
                                ColumnID = "ProgramOptions",
                                Width = 100,
                                Commands  = 
                                { 
                                    new ImageCommand { Icon = Icon.Pencil, CommandName  = "EditProgram", ToolTip = { Text = "Паспорт целевой программы" } },
                                    new ImageCommand { Icon = Icon.MonitorEdit, CommandName  = "ReportFinance", ToolTip = { Text = "Отчет об использовании финансовых средств" } },
                                    new ImageCommand { Icon = Icon.CalendarStar, CommandName  = "ReportExecuting", ToolTip = { Text = "Отчет о ходе реализации" } },
                                    new ImageCommand { Icon = Icon.BulletSparkle, CommandName  = "ReportDcpVcp", ToolTip = { Text = "Оценка целевой программы" } } 
                                }
                            },
                        new Column { Header = "Наименование программы", DataIndex = "Name", Width = 180 },
                        new Column { Header = "Нормативные документы", DataIndex = "NPAListCommaSeparated", Width = 200 },
                        new Column { Header = "Год начала реализации ", DataIndex = "BeginYearString", Width = 100 },
                        new Column { Header = "Год окончания реализации ", DataIndex = "EndYearString", Width = 100 },
                        new Column { Header = "Заказчик", DataIndex = "RefCreatorName", Width = 100 },
                        new Column { Header = "Родительская программа", DataIndex = "ParentName", Width = 200 },
                        new ImageCommandColumn
                            {
                                ColumnID = "ParentProgramOptions",
                                Width = 40,
                                Commands  = { new ImageCommand { Icon = Icon.Pencil, CommandName  = "OpenParentProgram", ToolTip = { Text = "Родительская программа" } } },
                                PrepareCommand = { Handler = "if (record.data.ParentId === null){command.hidden = true;command.hideMode = 'visibility';}; " }
                            },
                        new Column { Header = "Статус", DataIndex = "Status", Width = 50, Renderer = new Renderer("return getUrlForStatus(value);") },
                    },
                },
                SelectionModel = 
                {
                    new RowSelectionModel
                        {
                            ID = "RowSelectionModel1", 
                            Listeners = 
                            { 
                                RowSelect = { Handler = "btnDelete.enable();" },
                                RowDeselect = { Handler = "if(!{0}.hasSelection()){{btnDelete.disable();}}".FormatWith(GridpanelProgramsID) }
                            }
                        }
                },
                Listeners =
                    {
                        Command =
                            {
                                Handler = @"
if (command != undefined) {
   if (command=='EditProgram') {
      openProgram(record.data.ID, record.data.Name);
   } else if (command=='OpenParentProgram') {
      openProgram(record.data.ParentId, record.data.ParentName);
   } else if (command=='ReportFinance') {
      openReport(wReportFianceUsage, record.data.ID, 'Отчет об использовании финансовых средств', record.data.Name);
   }else if (command=='ReportExecuting') {
      openReport(wReportProgramExecuting, record.data.ID, 'Отчет о ходе реализации программы', record.data.Name);
   }else if (command=='ReportDcpVcp') {
      openReport(wReportEffectivityEstimate, record.data.ID, 'Оценка целевой программы', record.data.Name);
   }
}
"
                            }
                    }
            };

            var regionVisibility = extension.OKTMO != OKTMO.Sakhalin;
            if (!regionVisibility)
            {
                var imageCommands = table.ColumnModel.Columns.OfType<ImageCommandColumn>().FirstOrDefault(x => x.ColumnID == "ProgramOptions");
                var command = imageCommands.Commands.FirstOrDefault(x => x.CommandName == "ReportDcpVcp");
                imageCommands.Commands.Remove(command);
            }

            return new List<Component>
                       {
                           new Hidden { ID = "UrlIconStatusUnapproved", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit), InvalidText = "Проект" },
                           new Hidden { ID = "UrlIconStatusApproved", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEarth), InvalidText = "Утверждена" },
                           new Hidden { ID = "UrlIconStatusRunning", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserStar), InvalidText = "Действующая" },
                           new Hidden { ID = "UrlIconStatusFinished", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick), InvalidText = "Завершена" },
                           table
                       };
        }
       
        private Store GetProgramsListStore()
        {
            var store = new Store
            {
                ID = DatasourceProgramsID,
                Restful = true,
                ShowWarningOnFailure = true,
                RefreshAfterSaving = RefreshAfterSavingMode.Auto
            };

            store.Proxy.Add(new HttpProxy
            {
                RestAPI =
                {
                    ReadUrl = "/Programs/GetProgramsTable",
                    DestroyUrl = "/Programs/DeleteProgram",
                    CreateUrl = "/Programs/Create", ////просто заглушка, метода нет
                    UpdateUrl = "/Programs/Update", ////просто заглушка, метода нет
                }
            });
            store.BaseParams.Add(new Parameter("filters", "getFilters()", ParameterMode.Raw));
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                    new RecordField("ParentId"),
                    new RecordField("ParentName"),
                    new RecordField("NPAListCommaSeparated"),
                    new RecordField("BeginYearString"),
                    new RecordField("EndYearString"),
                    new RecordField("RefCreatorName"),
                    new RecordField("Status")
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка программ', response.responseText);";
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

        private Window GetProgramWindow()
        {
            Window win = new Window
            {
                ID = "wProgram",
                Hidden = true,
                Title = "Карточка",
                Width = 500,
                MinWidth = 200,
                Height = 500,
                MinHeight = 300,
                Modal = true,
                MonitorResize = true,
                CloseAction = CloseAction.Hide,
                AutoLoad =
                {
                    Url = "/View/TrgProgProgram",
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
if (w>1200){w=1200};
this.setSize({ width: w, height: size.height * 0.9});
if (wProgram.iframe != undefined){
  wProgram.reload();
}
"
                    },
                    Hide = { Handler = "wProgram.getBody().programForm.reset()" }
                }
            };
            return win;
        }

        private Window GetFinanceReportWindow()
        {
            Window win = new Window
            {
                ID = "wReportFianceUsage",
                Hidden = true,
                Title = "Отчет об использованию финансовых средств",
                Width = 500,
                MinWidth = 200,
                Height = 500,
                MinHeight = 300,
                Modal = true,
                MonitorResize = true,
                CloseAction = CloseAction.Hide,
                AutoLoad =
                {
                    Url = "/View/TrgProgReportFinanceUsage",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    Params = { new Parameter("programId", "null", ParameterMode.Raw) },
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                },
                Listeners =
                {
                    BeforeShow =
                    {
                        Handler = @"
var size = Ext.getBody().getSize(); 
this.setSize({ width: size.width * 0.95, height: size.height * 0.95});
if (wReportFianceUsage.iframe != undefined){
  wReportFianceUsage.reload();
}
"
                    },
                }
            };
            return win;
        }

        private Window GetProgramExecutingReportWindow()
        {
            Window win = new Window
            {
                ID = "wReportProgramExecuting",
                Hidden = true,
                Title = "Отчет о ходе реализации программы",
                Width = 500,
                MinWidth = 200,
                Height = 500,
                MinHeight = 300,
                Modal = true,
                MonitorResize = true,
                CloseAction = CloseAction.Hide,
                AutoLoad =
                {
                    Url = "/View/TrgProgReportProgramExecuting",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    Params = { new Parameter("programId", "null", ParameterMode.Raw) },
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                },
                Listeners =
                {
                    BeforeShow =
                    {
                        Handler = @"
var size = Ext.getBody().getSize(); 
this.setSize({ width: size.width * 0.95, height: size.height * 0.95});
if (wReportProgramExecuting.iframe != undefined){
  wReportProgramExecuting.reload();
}
"
                    },
                }
            };
            return win;
        }

        private Window GetEffectivityEstimateReportWindow()
        {
            Window win = new Window
            {
                ID = "wReportEffectivityEstimate",
                Hidden = true,
                Title = "Оценка целевой программы",
                Width = 500,
                MinWidth = 200,
                Height = 500,
                MinHeight = 300,
                Modal = true,
                MonitorResize = true,
                CloseAction = CloseAction.Hide,
                AutoLoad =
                {
                    Url = "/View/TrgProgReportEffectivityEstimate",
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    Params = { new Parameter("programId", "null", ParameterMode.Raw) },
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                },
                Listeners =
                {
                    BeforeShow =
                    {
                        Handler = @"
var size = Ext.getBody().getSize(); 
this.setSize({ width: size.width * 0.95, height: size.height * 0.95});
if (wReportEffectivityEstimate.iframe != undefined){
  wReportEffectivityEstimate.reload();
}
"
                    },
                }
            };
            return win;
        }
    }
}
