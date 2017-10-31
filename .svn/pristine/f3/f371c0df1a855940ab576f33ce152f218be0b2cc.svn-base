using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;
using TreeNode = Ext.Net.TreeNode;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class TasksView : View
    {
        private readonly SubjectTreeRepository subjectTreeRepository;
        private readonly IUserSessionState sessionState;

        public TasksView(SubjectTreeRepository subjectTreeRepository, IUserSessionState sessionState)
        {
            this.subjectTreeRepository = subjectTreeRepository;
            this.sessionState = sessionState;
        }

        public override List<Component> Build(ViewPage page)
        {
            RegisterResources(page);

            CreateTaskStore(page);

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportMain",
                    Items = 
                    {
                        new BorderLayout
                        {
                            North = { Items = { CreateTopPanel() }, Margins = { Bottom = 5 } },
                            West = { Items = { CreateSubjectsTreePanel(page) }, Margins = { Bottom = 3 } },
                            Center = { Items = { CreateTasksGridPanel(page) }, Margins = { Bottom = 3 } }
                        }
                    }
                },
                new ToolTip
                {
                    ID = "CellTip",
                    Target = "={gpTask.getView().mainBody}",
                    Delegate = ".x-grid3-col-SubjectShortName",
                    TrackMouse = true,
                    Listeners = { Show = { Fn = "showCellTip" } }
                },
                new ToolTip
                {
                    ID = "SubjectTreeTip",
                    Target = "={tgSubjects.innerCt}",
                    Delegate = ".x-tree-node-el",
                    TrackMouse = true,
                    Listeners = { Show = { Fn = "showTreeNodeTip" } }
                }
            };
        }

        private static Panel CreateTopPanel()
        {
            var topPanel = new Panel { ID = "topPanel", Height = 27, Border = false };
            topPanel.TopBar.Add(new Toolbar
            {
                Items =
                    {
                        new Label { Text = "Фильтры:" },
                        new Button
                            {
                                ID = "filterEdit", 
                                Text = "На редактировании",
                                Icon = Icon.UserEdit, 
                                EnableToggle = true, 
                                Pressed = true, 
                                ToggleHandler = "toggleFilter"
                            },
                        new Button
                            {
                                ID = "filterUnderConsideration", 
                                Text = "На рассмотрении",
                                Icon = Icon.UserMagnify, 
                                EnableToggle = true, 
                                Pressed = true, 
                                ToggleHandler = "toggleFilter"
                            },
                        new Button
                            {
                                ID = "filterAccepted", 
                                Text = "Утвержденные",
                                Icon = Icon.UserTick, 
                                EnableToggle = true, 
                                Pressed = false, 
                                ToggleHandler = "toggleFilter"
                            },
                        new ToolbarSeparator(),
                        new Button
                            {
                                ID = "filterDeadline", 
                                Text = "Просроченные",
                                Icon = Icon.Exclamation, 
                                EnableToggle = true, 
                                Pressed = true, 
                                ToggleHandler = "toggleFilter"
                            },
                        new Button
                            {
                                ID = "filterToDo", 
                                Text = "К исполнению",
                                Icon = Icon.DateEdit, 
                                EnableToggle = true, 
                                Pressed = true, 
                                ToggleHandler = "toggleFilter"
                            },
                        new Button
                            {
                                ID = "filterFuture", 
                                Text = "Текущий период",
                                Icon = Icon.DateNext, 
                                EnableToggle = true, 
                                Pressed = false, 
                                ToggleHandler = "toggleFilter"
                            },
                        new Button
                            {
                                ID = "filterCurrentYear", 
                                Text = "Текущий год",
                                Icon = Icon.DateMagnify, 
                                EnableToggle = true, 
                                Pressed = false, 
                                ToggleHandler = "toggleFilter"
                            },
                        new ToolbarSeparator(),
                        new Button
                            {
                                ID = "filterMyTasks", 
                                Text = "Показывать подчиненные задачи",
                                Icon = Icon.ChartOrganisation,
                                EnableToggle = true, 
                                Pressed = true,
                                ToggleHandler = "toggleFilter"
                            }  
                    }
            });

            topPanel.AddScript(@"function toggleFilter() { dsTask.load(); };");
            topPanel.AddScript(@"
function getStateFilter() { 
    var filter = [true, true, true, true, true, true, true];

    filter[0] = filterEdit.pressed;
    filter[1] = filterUnderConsideration.pressed;
    filter[2] = filterAccepted.pressed;
    filter[3] = filterDeadline.pressed;
    filter[4] = filterToDo.pressed;
    filter[5] = filterFuture.pressed;
    filter[6] = filterCurrentYear.pressed;

    return filter;
};");

            return topPanel;
        }

        private static void FillTree(TreeNode root, IEnumerable<LinqExtensions.Node<SubjectsTree>> nodes)
        {
            foreach (var node in nodes)
            {
                var treeNode = new TreeNode { Expanded = true };
                treeNode.CustomAttributes.Add(new ConfigItem("Id", Convert.ToString(node.Item.ID)));
                treeNode.CustomAttributes.Add(new ConfigItem("Subject", node.Item.Subject, ParameterMode.Value));
                treeNode.CustomAttributes.Add(new ConfigItem("SubjectShortName", node.Item.SubjectShortName, ParameterMode.Value));
                treeNode.CustomAttributes.Add(new ConfigItem("Role", node.Item.Role, ParameterMode.Value));
                treeNode.CustomAttributes.Add(new ConfigItem("Level", node.Item.ReportLevel, ParameterMode.Value));
                root.Nodes.Add(treeNode);

                FillTree(treeNode, node.Nodes);
            }
        }

        private static IEnumerable<Component> CreateTasksGridPanel(ViewPage page)
        {
            var gp = new MultiGroupingPanel { ID = "gpTask", StoreID = "dsTask", LoadMask = { ShowMask = true } };

            gp.ColumnModel.Columns.Add(new ImageCommandColumn 
            { 
                DataIndex = "Action",
                Width = 56,
                Commands = 
                { 
                    new ImageCommand { Icon = Icon.PlayGreen, CommandName = "OpenTask", ToolTip = { Text = "Открыть задачу" } },
                    new ImageCommand { Icon = Icon.TableGo, CommandName = "OpenForm", ToolTip = { Text = "Открыть форму сбора" } } 
                } 
            });
            
            gp.ColumnModel.AddColumn("Year", "Год", DataAttributeTypes.dtString)
                .SetWidth(150);
            gp.ColumnModel.AddColumn("Period", "Отчетный период", DataAttributeTypes.dtString).SetHidden(true);
            gp.ColumnModel.AddColumn("FormName", "Вид отчетности", DataAttributeTypes.dtString).SetHidden(true);
            gp.ColumnModel.AddColumn("TemplateName", "Форма отчета", DataAttributeTypes.dtString)
                .SetWidth(200);
            gp.ColumnModel.AddColumn("TemplateGroup", "Название сбора", DataAttributeTypes.dtString)
                .SetWidth(200);
            gp.ColumnModel.AddColumn("Role", "Роль субъекта", DataAttributeTypes.dtString).SetHidden(true);
            gp.ColumnModel.AddColumn("ReportLevel", "Уровень отчетности", DataAttributeTypes.dtString);
            gp.ColumnModel.AddColumn("SubjectShortName", "Субъект", DataAttributeTypes.dtString);

            gp.ColumnModel.AddColumn("BeginDate", "Начало отчетного периода", DataAttributeTypes.dtDateTime).SetHidden(true);
            gp.ColumnModel.AddColumn("EndDate", "Конец отчетного периода", DataAttributeTypes.dtDateTime).SetHidden(true);
            gp.ColumnModel.AddColumn("Deadline", "Дата сдачи отчета", DataAttributeTypes.dtDateTime);

            gp.ColumnModel.AddColumn("LastChangeDate", "Дата последнего изменения", DataAttributeTypes.dtDateTime);
            gp.ColumnModel.AddColumn("LastChangeUser", "Автор последнего изменения", DataAttributeTypes.dtString);

            gp.ColumnModel.AddColumn("Status", "Статус", DataAttributeTypes.dtString);

            gp.AddColumnsWrapStylesToPage(page);

            gp.View.Add(new MultiGroupingView { GroupTextTpl = "{text}: {gvalue}", HideGroupedColumn = true, ForceFit = true });

            gp.Listeners.Command.Handler = "Ext.Msg.alert(command, record.data.id);";
            gp.Listeners.CellMouseDown.Handler = @"
var r = item.getStore().getAt(rowIndex);
var fName = item.getColumnModel().getDataIndex(columnIndex);
if (fName == 'Action') {
    var t = e.getTarget();
    var taskTitle = r.data.TemplateShortName + '_' + r.data.SubjectShortName + '_' + r.data.Period + '_' + r.data.Year;
    if (t.attributes.cmd.value == 'OpenTask') {
        parent.MdiTab.addTab({ id: 'consTask_' + r.data.ID, title: taskTitle, url: '/View/ConsTask?id=' + r.data.ID, icon: 'icon-mark-active' });
    }
    if (t.attributes.cmd.value == 'OpenForm') {
        parent.MdiTab.addTab({ id: 'consReport_' + r.data.ID, title: taskTitle, url: '/ConsReport/' + r.data.TemplateClass + '?taskId=' + r.data.ID, icon: 'icon-report' });
    }
}
";

            return new List<Component> { gp };
        }

        private static void CreateTaskStore(ViewPage page)
        {
            var store = new MultiGroupingStore { ID = "dsTask", AutoLoad = false };

            store.Proxy.Add(new HttpProxy { Url = "/ConsTasks/Load", Method = HttpMethod.GET });

            store.SetJsonReader()
                .AddField("ID")
                .AddField("Year")
                .AddField("Period")
                .AddField("FormName")
                .AddField("TemplateName")
                .AddField("TemplateShortName")
                .AddField("TemplateClass")
                .AddField("TemplateGroup")
                .AddField("Role")
                .AddField("ReportLevel")
                .AddField("Subject")
                .AddField("SubjectShortName")
                .AddField("BeginDate")
                .AddField("EndDate")
                .AddField("Deadline")
                .AddField("Status")
                .AddField("LastChangeDate")
                .AddField("LastChangeUser");

            store.BaseParams.Add(new Parameter("SubjectId", "tgSubjects.getSelectionModel().getSelectedNode().attributes.Id", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("filters", "getStateFilter()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("showChilds", "filterMyTasks.pressed", ParameterMode.Raw));

            store.SortInfo.Field = "ID";

            page.Controls.Add(store);
            store.ResourceManager.RegisterOnReadyScript("dsTask.groupField = ['Year', 'TemplateGroup', 'TemplateName'];");
        }

        private static void RegisterResources(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            resourceManager.RegisterClientStyleBlock("CustomStyle", Presentation.Views.Resource.TasksViewCss);
            
            var script = @"
function showCellTip() {
    var rowIndex = gpTask.getView().findRowIndex(this.triggerElement),
    cellIndex = gpTask.getView().findCellIndex(this.triggerElement),
    row = dsTask.getAt(rowIndex),
    col = gpTask.getColumnModel().getDataIndex(cellIndex),
    data = '';

    if (col == 'SubjectShortName'){
        data = row.get('Subject');
    }

    if (data) {
        this.body.dom.innerHTML = data;
    }
    else {
        this.hide();
        return false;
    }
};

function showTreeNodeTip(tip){
    var e = Ext.fly(tip.triggerElement).findParent('.x-tree-node-el', null, true);
    var node = e ? tgSubjects.getNodeById(e.getAttribute('tree-node-id', 'ext')) : null;
    
    if(node){
        var data = node.attributes.Subject + ' (' + node.attributes.Role + ')';
                 
        if (tip.rendered) {
            tip.update(data);
        } else {
            tip.html = data;
        }
    } else {
        return false;
    }
};";
            resourceManager.RegisterClientScriptBlock("Scripts", script);
        }

        private IEnumerable<Component> CreateSubjectsTreePanel(ViewPage page)
        {
            TreeGrid tg = new TreeGrid
            {
                ID = "tgSubjects",
                Title = "Участники сбора",
                Width = 220,
                NoLeafIcon = true, 
                Collapsible = true,
                Split = true,
                Layout = "VBoxLayout"
            };

            tg.Columns.Add(new TreeGridColumn { Header = "Субъект", Width = 200, DataIndex = "SubjectShortName" });
            tg.Columns.Add(new TreeGridColumn { Header = "Роль", Width = 70, DataIndex = "Role", Hidden = true });
            tg.Columns.Add(new TreeGridColumn { Header = "Уровень", Width = 70, DataIndex = "Level", Hidden = true });
            tg.Columns.Add(new TreeGridColumn { Header = "Субъект1", Width = 70, DataIndex = "Subject", Hidden = true });

            tg.SelectionModel.Add(new DefaultSelectionModel
                {
                    Listeners = { SelectionChange = { Handler = "dsTask.reload();" } }
                });

            var root = new TreeNode();
            
            // Добавляем дерево субъектов
            PopulateTree(root);

            tg.Root.Add(root);

            ResourceManager.GetInstance(page).RegisterOnReadyScript("tgSubjects.getRootNode().firstChild.select();");

            return new List<Component> { tg };
        }

        private void PopulateTree(TreeNode root)
        {
            IEnumerable<SubjectsTree> list = new List<SubjectsTree>();
            list = sessionState.Subjects
                .Aggregate(
                    list, 
                    (current, subject) => current.Union(subjectTreeRepository.GetTree(subject.ID)));

            var tree = list.ByHierarchy(t => t.ParentId == null, (parent, child) => parent.ID == child.ParentId).ToList();

            FillTree(root, tree.Where(x => x.Parent == null));
        }
    }
}
