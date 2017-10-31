using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Views
{
    public class MarksOivView : View
    {
        private readonly IMarksOmsuExtension extension;
        private readonly MarksOivGridControl grid;

        public MarksOivView(IMarksOmsuExtension extension)
        {
            this.extension = extension;

            grid = new MarksOivGridControl();
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            resourceManager.RegisterScript("ViewPersistence", "/Content/js/ViewPersistence.js");
            
            if (extension.ResponsOIV == null)
            {
                resourceManager.RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен ОИВ.").ToScript());

                return new List<Component>();
            }

            resourceManager.RegisterClientScriptBlock("MarksOivView", Resources.Resource.MarksOivView);
            resourceManager.RegisterClientStyleBlock(
                    "CustomLabelStyle",
                    "label.x-form-item-label{ font-weight: bold;} .x-form-group .x-form-group-header-text{background: url('/ux/extensions/formgroup/images/formcollapseicon-gif/ext.axd') no-repeat scroll 2px 0 #DFE8F6;}");

            resourceManager.RegisterOnReadyScript("ViewPersistence.refresh.call(window);");

            FormPanel infoPanel = new FormPanel
            {
                Border = false,
                Layout = "form",
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
            layout.Center.Items.Add(grid.Build(page));

            grid.Store.AutoLoad = false;
            grid.Store.BaseParams.Add(new Parameter("calculatePreviosResult", "calculatePreviousResultFilter.pressed", ParameterMode.Raw));
            grid.Store.Listeners.DataChanged.Handler = @"
var year = parseInt(parent.cmbOMSUYearChooser.getValue());
gpMarks.getColumnModel().setColumnHeader(3, year-1);
gpMarks.getColumnModel().setColumnHeader(4, year-1);
gpMarks.getColumnModel().setColumnHeader(5, year);
labelYear.update(year +' год');
";

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

        private static Toolbar CreateToolbar()
        {
            Toolbar toolbar = new Toolbar { ID = "toolbar" };

            Button saveButton = new Button
            {
                ID = "saveButton",
                Icon = Icon.Disk,
                ToolTip = "Сохранить данные"
            };
            saveButton.Listeners.Click.Handler = "dsMarks.save()";
            toolbar.Add(saveButton);

            Button refreshButton = new Button
            {
                ID = "refreshButton",
                Icon = Icon.PageRefresh,
                ToolTip = "Обновить"
            };
            refreshButton.Listeners.Click.Handler = "ViewPersistence.refresh.call(window);";
            toolbar.Add(refreshButton);

            toolbar.Add(new ToolbarSeparator());

            Button exportButton = new Button
            {
                ID = "exportButton",
                Icon = Icon.PageExcel,
                ToolTip = "Выгрузка в Excel",
                DirectEvents =
                    {
                        Click =
                            {
                                Url = "/Export/OivInputData",
                                IsUpload = true,
                                CleanRequest = true,
                                ExtraParams =
                                    {
                                        new Parameter("markId", "marksCombo.getSelectedItem().value", ParameterMode.Raw)
                                    } 
                            }
                    }
            };
            toolbar.Add(exportButton);

            toolbar.Add(new ToolbarSeparator());

            toolbar.Add(new Button
                            {
                                ID = "calculatePreviousResultFilter",
                                Icon = Icon.HourglassLink,
                                ToolTip = "Выводить данные доклада за предыдущий год",
                                EnableToggle = true,
                                Pressed = false,
                                ToggleHandler = "toggleFilter"
                            });
            return toolbar;
        }

        private void CreateTopInfoPanel(FormPanel panel)
        {
            Label label = new Label();
            label.Html =
                "Форма ввода показателей для оценки эффективности деятельности органов местного самоуправления городских округов и муниципальных районов автономного округа.";
            panel.Items.Add(label);

            label = new Label();
            label.FieldLabel = "Наименование ОИВ";
            label.Html = "{0}".FormatWith(extension.ResponsOIV.Name);
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
                        Url = "/MarksOiv/LoadMarkDescription",
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
            container.Items.Add(new Label { ID = "labelCodeRep" });
            container.Items.Add(new Label { ID = "labelDescription" });
            container.Items.Add(new Label { ID = "labelCalcMark" });
            container.Items.Add(new Label { ID = "labelInfoSource" });
            container.Items.Add(new Label { ID = "labelSymbol" });
            container.Items.Add(new Label { ID = "labelFormula" });

            panel.Items.Add(container);
        }

        private Store CreateMarksSelectStore()
        {
            var store = new Store { ID = "dsMarksLookup", AutoLoad = false };
            store.Proxy.Add(new HttpProxy { Url = "/MarksOiv/GetMarksListForOiv", Method = HttpMethod.POST });
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
    }
}

