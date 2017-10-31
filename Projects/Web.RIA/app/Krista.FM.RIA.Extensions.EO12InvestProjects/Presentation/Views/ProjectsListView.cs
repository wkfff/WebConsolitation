using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Views
{
    public class ProjectsListView : View
    {
        private const string DatasourceProjectListID = "dsProjects";
        private const string GridpanelProjectListID = "gpProjects";
        
        public int RefPartId
        {
            get { return Convert.ToInt32(Params["refPart"]); }
        }

        public int ProjStatus
        {
            get { return Convert.ToInt32(Params["projStatus"]); }
        }
        
        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("ProjectsListView", Resource.ProjectsListView);
            } 
            
            var view = new Viewport
                            {
                                ID = "viewportMain",
                                Items = 
                                {
                                    new BorderLayout
                                    {   
                                        North = { Items = { CreateTopPanel() }, Margins = { Bottom = 5 } },
                                        Center = { Items = { CreateProjectListPanel() }, Margins = { Bottom = 3 } }
                                    }
                                }
                            };

            return new List<Component> { view };
        }

        private Panel CreateTopPanel()
        {
            var toolbar = new Toolbar();
            toolbar.Add(new Button
            {
                ID = "btnAdd",
                Icon = Icon.Add,
                ToolTip = "Добавить",
                Listeners = { Click = { Handler = "openProjectTab(null, {0}, 'Новый проект');".FormatWith(RefPartId) } },
                Hidden = RefPartId == (int)InvProjPart.Undefined
            });

            toolbar.Add(new Button
            {
                ID = "btnDelete",
                Icon = Icon.Delete,
                Disabled = true,
                ToolTip = "Удалить",
                Listeners = 
                { 
                    Click = 
                    {
                        Handler = "DeleteProjectSelectedRow('{0}');".FormatWith(GridpanelProjectListID)
                    } 
                }
            });

            toolbar.Add(new Button
            {
                ID = "btnMoveToProposedProjects",
                Icon = Icon.ArrowSwitchBluegreen,
                Disabled = true,
                Visible = (RefPartId == (int)InvProjPart.Part1) || (RefPartId == (int)InvProjPart.Part2),
                ToolTip = "Перенести в раздел {0}".FormatWith(RefPartId == (int)InvProjPart.Part1 ? "2" : "1"),
                Listeners =
                    {
                        Click =
                            {
                                Handler = "ChangeProjectPartSelectedRow('{0}');".FormatWith(GridpanelProjectListID)
                            }
                    },
                Hidden = RefPartId == (int)InvProjPart.Undefined
            });
            
            toolbar.Add(new ToolbarSpacer(50));
           
            toolbar.Add(new Button
            {
                ID = "btnFilterEditable",
                Icon = Icon.UserEdit,
                ToolTip = "Редактируемые",
                EnableToggle = true,
                Pressed = ProjStatus == (int)InvProjStatus.Undefined,
                ToggleHandler = "toggleFilter",
                Hidden = RefPartId == (int)InvProjPart.Undefined
            });

            toolbar.Add(new Button
            {
                ID = "btnFilterExecutable",
                Icon = Icon.UserTick,
                ToolTip = "Исполняемые",
                EnableToggle = true,
                Pressed = ProjStatus == (int)InvProjStatus.Undefined,
                ToggleHandler = "toggleFilter",
                Hidden = RefPartId == (int)InvProjPart.Undefined
            });

            toolbar.Add(new Button
            {
                ID = "btnFilterExcluded",
                Icon = Icon.UserCross,
                ToolTip = "Исключенные",
                EnableToggle = true,
                Pressed = true,
                ToggleHandler = "toggleFilter",
                Hidden = RefPartId == (int)InvProjPart.Undefined
            });

            toolbar.Add(new ToolbarFill());
            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceProjectListID) } }
            });

            toolbar.AddScript(@"function toggleFilter() {{ {0}.load(); }};".FormatWith(DatasourceProjectListID));

            return new Panel
            {
                ID = "topPanel",
                Height = 27,
                Border = false,
                TopBar = { toolbar }
            };
        }

        private List<Component> CreateProjectListPanel()
        {
            GridPanel table = new GridPanel
            {
                ID = GridpanelProjectListID, 
                Store = { GetProjectListStore() },
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
                                Commands  = { new ImageCommand { Icon = Icon.Pencil, CommandName  = "EditProject", ToolTip = { Text = "Открыть карточку проекта" } } }
                            },
                        new Column { DataIndex = "Status", Width = 30, Renderer = new Renderer("return getUrlForStatus(value);") },
                        new Column { Header = "Идентификационный номер", DataIndex = "Code", Width = 180, Align = Alignment.Left },
                        new Column { Header = "Наименование проекта", DataIndex = "Name", Width = 200, Align = Alignment.Left }
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
                                        RowSelect =
                                            {
                                                Handler = @"
btnDelete.enable();
if(Ext.getCmp('btnMoveToProposedProjects')!=undefined){
  btnMoveToProposedProjects.enable();
}"
                                            },
                                        RowDeselect =
                                            {
                                                Handler = @"
if(!gpProjects.hasSelection()){
  btnDelete.disable();
  if(Ext.getCmp('btnMoveToProposedProjects')!=undefined){
    btnMoveToProposedProjects.disable();
  }
}"
                                            }
                                    }
                            }
                    },
                Listeners = { Command = { Handler = "if (command != undefined && command=='EditProject'){{openProjectTab(record.data.ID, {0}, record.data.Name);}}".FormatWith(RefPartId) } }
            };

            switch (RefPartId)
            {
                case (int)InvProjPart.Part1:
                    table.Title = "Реализуемые приоритетные инвестиционные проекты";
                    table.ColumnModel.Columns.Add(new Column { Header = "Место реализации", DataIndex = "TerritoryName", Width = 200, Align = Alignment.Left });
                    table.ColumnModel.Columns.Add(new Column { Header = "Год начала реализации", DataIndex = "BeginYear", Width = 150, Align = Alignment.Center });
                    table.ColumnModel.Columns.Add(new Column { Header = "Год окончания реализации", DataIndex = "EndYear", Width = 150, Align = Alignment.Center });
                    break;
                case (int)InvProjPart.Part2:
                    table.Title = "Предлагаемые к реализации инвестиционные проекты";
                    table.ColumnModel.Columns.Add(new Column { Header = "Предполагаемое место реализации", DataIndex = "TerritoryName", Width = 250, Align = Alignment.Left });
                    break;
                default:
                    table.Title = "Исключенные инвестиционные проекты";
                    table.ColumnModel.Columns.Add(new Column { Header = "Причина исключения", DataIndex = "Exception", Width = 300, Align = Alignment.Left });
                    break;
            }

            return new List<Component>
                       {
                           new Hidden { ID = "UrlIconStatusEdit", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit), InvalidText = "Редактируется" },
                           new Hidden { ID = "UrlIconStatusExecut", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick), InvalidText = "Исполняется" },
                           new Hidden { ID = "UrlIconStatusExclude", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserCross), InvalidText = "Иключён" },
                           table
                       };
        }

        private Store GetProjectListStore()
        {
            var store = new Store
            {
                ID = DatasourceProjectListID,
                Restful = true,
                ShowWarningOnFailure = true,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };

            store.Proxy.Add(new HttpProxy
            {
                RestAPI =
                {
                    ReadUrl = "/ProjectsList/GetProjectsTable",
                    DestroyUrl = "/ProjectsList/DeleteProject",
                    CreateUrl = "/ProjectsList/Create", ////просто заглушка, метода нет
                    UpdateUrl = "/ProjectsList/Update", ////просто заглушка, метода нет
                }
            });
            store.BaseParams.Add(new Parameter("refPartId", RefPartId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("projStatusFilters", "getStatusFilters()", ParameterMode.Raw));
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Code"),
                    new RecordField("Name"),
                    new RecordField("Status"),
                    new RecordField("TerritoryName"),
                    new RecordField("BeginYear"),
                    new RecordField("EndYear"),
                    new RecordField("Exception")
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка проектов', response.responseText);";
            store.Listeners.SaveException.Handler = "Ext.Msg.alert('Ошибка при сохранение', response.responseText);";
            store.Listeners.Save.Handler = "Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Сохранение.', hideDelay  : 2500});";
            store.Listeners.Exception.Handler = @"
if (response.raw != undefined && response.raw.message != undefined){
   Ext.Msg.alert('Ошибка при сохранение', response.raw.message);
}else{
   Ext.Msg.alert('Ошибка', response.responseText);
}
";
            return store;
        }
    }
}