using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Presentation.Views
{
    public class MarksCompareView : View
    {
        private const string StoreId = "dsMarks";
        private const string GridId = "gpMarks";
        
        private readonly IRegion10MarksOivExtension extension;

        public MarksCompareView(IRegion10MarksOivExtension extension) 
        {
            this.extension = extension;
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            resourceManager.RegisterScript("ViewPersistence", "/Content/js/ViewPersistence.js");
            
            // Проверяем входит ли пользователь в группу ОИВ-ов
            if (extension.UserResponseOiv == null)
            {
                resourceManager.RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен ОИВ.").ToScript());
                return new List<Component>();
            }

            RegisterResources(resourceManager);
            resourceManager.RegisterOnReadyScript("ViewPersistence.refresh.call(window);");

            FormPanel infoPanel = new FormPanel
            {
                Border = false,
                Layout = LayoutType.Form.ToString(),
                AutoHeight = true,
                LabelWidth = 125,
                LabelAlign = LabelAlign.Right,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            infoPanel.TopBar.Add(CreateToolbar());

            infoPanel.Listeners.AfterRender.Handler = "viewportMain.doLayout()";

            CreateTopInfoPanel(infoPanel);

            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            layout.North.Items.Add(infoPanel);
            
            layout.Center.Items.Add(CreateGridPanel(page));

            Viewport viewport = new Viewport { ID = "viewportMain" };
            viewport.Items.Add(layout);

            var toolTip = new ToolTip
            {
                ID = "RowTip",
                Target = "={gpMarks.getView().mainBody}",
                Delegate = ".x-grid3-cell",
                TrackMouse = true,
                Listeners = { Show = { Fn = "showCellTip" } }
            };

            return new List<Component> { viewport, toolTip };
        }

        private static void RegisterResources(ResourceManager resourceManager)
        {
            resourceManager.RegisterIcon(Icon.UserEdit);
            resourceManager.RegisterIcon(Icon.UserMagnify);
            resourceManager.RegisterIcon(Icon.UserTick);
            resourceManager.RegisterIcon(Icon.Accept);

            resourceManager.RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");

            resourceManager.RegisterClientStyleBlock(
                "CustomStyle",
                ".disable-cell{background-color: #EDEDED !important;}");
            resourceManager.RegisterClientStyleBlock(
                "CustomLabelStyle",
                "label.x-form-item-label{ font-weight: bold;}  .x-form-group .x-form-group-header-text{background: url('/ux/extensions/formgroup/images/formcollapseicon-gif/ext.axd') no-repeat scroll 2px 0 #DFE8F6;}");

            resourceManager.RegisterClientScriptBlock("MarksCompareView", Resources.Resource.MarksCompareView);
        }

        private static Toolbar CreateToolbar()
        {
            return new Toolbar
            {
                ID = "toolbar",
                Items =
                    {
                        new Button
                        {
                            ID = "saveButton",
                            Icon = Icon.Disk,
                            ToolTip = "Сохранить данные",
                            Listeners = { Click = { Handler = "dsMarks.save();" } }
                        },
                        new Button
                        {
                            ID = "refreshButton",
                            Icon = Icon.PageRefresh,
                            ToolTip = "Обновить",
                            Listeners = { Click = { Handler = "ViewPersistence.refresh.call(window);" } }
                        },
                        new ToolbarSeparator(),
                        new Button
                        {
                            ID = "rejectButton",
                            Icon = Icon.Decline,
                            ToolTip = "Отправить выделенные показатели на доработку",
                            Listeners = { Click = { Fn = "rejectButtonClick" } }
                        },
                        new ToolbarSeparator(),
                        new Button
                                  {
                                      ID = "filterEdit",
                                      ToolTip = "На редактировании",
                                      Icon = Icon.UserEdit,
                                      EnableToggle = true,
                                      Pressed = true,
                                      ToggleHandler = "toggleFilter"
                                  },
                        new Button
                                  {
                                      ID = "filterMagnify",
                                      ToolTip = "На рассмотрении",
                                      Icon = Icon.UserMagnify,
                                      EnableToggle = true,
                                      Pressed = true,
                                      ToggleHandler = "toggleFilter"
                                  },
                        new ToolbarSeparator(),
                        new Button
                            {
                                ID = "exportReportButton",
                                Icon = Icon.PageExcel,
                                ToolTip = "Выгрузка в Excel",
                                DirectEvents =
                                    {
                                        Click =
                                            {
                                                Url = "/Region10Export/OmsuCompare",
                                                IsUpload = true,
                                                CleanRequest = true,
                                                ExtraParams =
                                                    {
                                                        new Parameter("markId", "marksCombo.getSelectedItem().value", ParameterMode.Raw)
                                                    },
                                                Before = "if (marksCombo.getSelectedItem().value == undefined || marksCombo.getSelectedItem().value == ''){return false;}"
                                            }
                                    }
                            }
                    }
            };
        }

        private static Store CreateMarksSelectStore()
        {
            var store = new Store { ID = "dsMarksLookup", AutoLoad = false };
            store.Proxy.Add(new HttpProxy { Url = "/Region10OmsuCompare/GetMarksListForCompare", Method = HttpMethod.POST });
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name")
                }
            });

            store.BaseParams.Add(new Parameter("filter", "marksCombo.getText()", ParameterMode.Raw));
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка показателей', response.responseText);";
            store.Listeners.Load.Handler = "eval(store.reader.jsonData.extraParams);";
            return store;
        }

        private static Store CreateStore(string storeId)
        {
            Store store = new Store { ID = storeId, AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("RefStatusData");
            reader.Fields.Add("Territory");
            reader.Fields.Add("Fact");
            reader.Fields.Add("Forecast");
            reader.Fields.Add("Forecast2");
            reader.Fields.Add("Forecast3");
            reader.Fields.Add("Note");
            reader.Fields.Add("RefTerritory");
            reader.Fields.Add("Symbol");
            reader.Fields.Add("Capacity");
            reader.Fields.Add("Readonly");
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("markId", "marksCombo.getSelectedItem().value", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Region10OmsuCompare/Load",
                Method = HttpMethod.POST
            });

            var httpWriteProxy = new HttpWriteProxy
            {
                Url = "/Region10OmsuCompare/Save",
                Method = HttpMethod.POST,
                Timeout = 500000
            };
            store.UpdateProxy.Add(httpWriteProxy);

            store.BaseParams.Add(new Parameter("filter", "getStateFilter()", ParameterMode.Raw));

            store.Listeners.DataChanged.Handler = @"
var year = parseInt(parent.cmbOivYearChooser.getValue());
gpMarks.getColumnModel().setColumnHeader(3, year);
gpMarks.getColumnModel().setColumnHeader(4, year+1);
gpMarks.getColumnModel().setColumnHeader(5, year+2);
gpMarks.getColumnModel().setColumnHeader(6, year+3);
labelYear.update(year +' год');
";
            return store;
        }

        private void CreateTopInfoPanel(FormPanel panel)
        {
            Label label = new Label();
            label.Html = "Форма вывода показателей для оценки эффективности деятельности органов исполнительной власти в разрезе муниципальных образований.";
            panel.Items.Add(label);

            label = new Label();
            label.FieldLabel = "Наименование MO";
            label.Html = "{0}".FormatWith(extension.UserResponseOiv.Name);
            panel.Items.Add(label);

            label = new Label();
            label.ID = "labelYear";
            label.FieldLabel = "Период";
            label.Html = "{0} год".FormatWith(extension.CurrentYear);
            panel.Items.Add(label);

            ComboBox comboBox = new ComboBox
            {
                ID = "marksCombo",
                Width = 800,
                FieldLabel = "Показатель",
                MinChars = 1,
                ValueField = "ID",
                DisplayField = "Name",
                LoadingText = "Поиск...",
                Store = { CreateMarksSelectStore() },
                TriggerAction = TriggerAction.All,
                AutoShow = true,
                Triggers = { new FieldTrigger { HideTrigger = true, Icon = TriggerIcon.Clear } },
                Listeners =
                {
                    TriggerClick = { Handler = "if(index == 0) { this.focus().clearValue(); trigger.hide(); dsMarks.reload(); clearMarkDescription(); }" },
                    BeforeQuery = { Handler = "this.triggers[0][this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" },
                    Select = { Handler = "this.triggers[0][this.getRawValue().toString().length == 0 ? 'hide' : 'show'](); dsMarks.reload();" },
                    Blur = { Handler = "if(Ext.isEmpty(this.getText())) { this.setValue(''); this.triggers[0].hide(); dsMarks.reload();};" }
                },
                DirectEvents =
                {
                    Select =
                    {
                        Url = "/Region10OmsuCompare/LoadMarkDescription",
                        CleanRequest = true,
                        EventMask = { ShowMask = false },
                        ExtraParams =
                        {
                            new Parameter("markId", "marksCombo.getSelectedItem().value", ParameterMode.Raw)
                        }
                    }
                }
            };

            panel.Items.Add(comboBox);

            FieldSet container = new FieldSet
            {
                Title = "Описание показателя",
                FormGroup = true,
                AutoHeight = true,
                Collapsed = true,
                Layout = "form",
                LabelAlign = LabelAlign.Left,
                Border = false,
                LabelWidth = 1,
                Padding = 2
            };
            container.Listeners.Expand.Handler = "viewportMain.doLayout();";
            container.Listeners.Collapse.Handler = "viewportMain.doLayout();";

            container.Items.Add(new Label { ID = "labelOKEI" });
            container.Items.Add(new Label { ID = "labelCompRep" });
            container.Items.Add(new Label { ID = "labelCodeRep" });
            container.Items.Add(new Label { ID = "labelSymbol" });
            container.Items.Add(new Label { ID = "labelFormula" });

            panel.Items.Add(container);
        }

        private GridPanel CreateGridPanel(ViewPage page)
        {
            GridPanel gp = new GridPanel
            {
                ID = GridId,
                Store = { CreateStore(StoreId) },
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Height = 600,
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true }
            };

            var view = new GroupingView { ForceFit = false, HideGroupedColumn = true };
            gp.View.Add(view);

            var groupRow = new HeaderGroupRow();
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 3 });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Отчет", ColSpan = 1, Align = Alignment.Center });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Прогноз", ColSpan = 3, Align = Alignment.Center });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });

            view.HeaderGroupRows.Add(groupRow);

            gp.ColumnModel.Columns.Add(new Column { ColumnID = "ID", DataIndex = "ID", Header = "Id", Hidden = true });

            var stateColumn = new Column { ColumnID = "State", DataIndex = "RefStatusData", Width = 30, Groupable = false };
            stateColumn.Renderer.Fn = "columnRenderStatus";
            gp.ColumnModel.Columns.Add(stateColumn);

            gp.ColumnModel.AddColumn("Territory", "Territory", "Наименование МО", DataAttributeTypes.dtString, Mandatory.NotNull)
                .SetWidth(300).SetGroupable(false);

            var year = this.extension.CurrentYearVal;

            var cl = gp.ColumnModel.AddColumn("Fact", "Fact", year.ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Отчет").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderValue()";

            gp.ColumnModel.AddColumn("Forecast", "Forecast", (year + 1).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
    .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4)
    .RendererFn("columnRenderValue()");

            gp.ColumnModel.AddColumn("Forecast2", "Forecast2", (year + 2).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4)
                .RendererFn("columnRenderValue()");

            gp.ColumnModel.AddColumn("Forecast3", "Forecast3", (year + 3).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4)
                .RendererFn("columnRenderValue()");

            gp.ColumnModel.AddColumn("Note", "Note", "Примечание", DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(200).SetGroupable(false).SetEditableString()
                .RendererFn("columnRenderNote()");

            gp.Listeners.BeforeEdit.Fn = "beforeEditCell";

            gp.Listeners.BeforeEdit.AddAfter(@"
if (e.record.get('RefStatusData') != 1 || e.record.get('Readonly')) {
    return false;
}
return true;");

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            return gp;
        }
    }
}
