using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Views
{
    public class MarksOmsuView : View
    {
        private readonly IMarksOmsuExtension extension;
        private readonly MarksOmsuGridControl grid;

        public MarksOmsuView(IMarksOmsuExtension extension)
        {
            this.extension = extension;

            grid = new MarksOmsuGridControl(extension) { StoreController = "MarksOmsu" };
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            resourceManager.RegisterScript("ViewPersistence", "/Content/js/ViewPersistence.js");
            
            if (extension.UserRegionCurrent == null)
            {
                resourceManager.RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен район.").ToScript());

                return new List<Component>();
            }

            resourceManager.RegisterClientScriptBlock("MarksOmsuView", Resources.Resource.MarksOmsuView);

            FormPanel infoPanel = new FormPanel
            {
                Border = false,
                Layout = "form",
                Height = 100,
                LabelWidth = 1,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            infoPanel.TopBar.Add(CreateToolbar());

            CreateTopInfoPanel(infoPanel);

            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            layout.North.Items.Add(infoPanel);
            layout.Center.Items.Add(grid.Build(page));

            grid.Store.BaseParams.Add(new Parameter("filter", "getStateFilter()", ParameterMode.Raw));
            grid.Store.BaseParams.Add(new Parameter("showHierarhy", "showHierarhy.pressed", ParameterMode.Raw));
            grid.Store.BaseParams.Add(new Parameter("calculatePreviosResult", "calculatePreviousResultFilter.pressed", ParameterMode.Raw));
            
            grid.Store.Listeners.DataChanged.Handler = @"
var year = parseInt(parent.cmbOMSUYearChooser.getValue());
gpMarks.getColumnModel().setColumnHeader(7, year-1);
gpMarks.getColumnModel().setColumnHeader(8, year-1);
gpMarks.getColumnModel().setColumnHeader(9, year);
gpMarks.getColumnModel().setColumnHeader(10, year+1);
gpMarks.getColumnModel().setColumnHeader(11, year+2);
gpMarks.getColumnModel().setColumnHeader(12, year+3);
labelYear.update('<b>Период</b>: '+ year +' год');
";

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "Center" };
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
                            Listeners = { Click = { Fn = "saveButtonClick" } }
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
                                ID = "calculatePreviousResultFilter",
                                Icon = Icon.HourglassLink,
                                ToolTip = "Выводить данные доклада за предыдущий год",
                                EnableToggle = true,
                                Pressed = false,
                                ToggleHandler = "toggleFilter"
                            },
                        new ToolbarSeparator(),
                        new Button
                        {
                            ID = "totestButton",
                            Icon = Icon.Tick,
                            ToolTip = "Передать выделенные показатели на рассмотрение",
                            Listeners = { Click = { Fn = "toTestButtonClick" } }
                        },
                        new ToolbarSeparator(),
                        new Label { Text = "Фильтры:" },
                        new Button
                            {
                                ID = "filterEdit", 
                                ToolTip = "Показатели на редактировании МО и ОИВ",
                                Icon = Icon.UserEdit, 
                                EnableToggle = true, 
                                Pressed = true, 
                                ToggleHandler = "toggleFilter"
                            },
                        new Button
                            {
                                ID = "filterMagnify", 
                                ToolTip = "Показатели на рассмотрении УИМ",
                                Icon = Icon.UserMagnify, 
                                EnableToggle = true, 
                                Pressed = true, 
                                ToggleHandler = "toggleFilter"
                            },
                        new Button
                            {
                                ID = "filterTick", 
                                ToolTip = "Показатель утвержден УИМ",
                                Icon = Icon.UserTick, 
                                EnableToggle = true, 
                                Pressed = true, 
                                ToggleHandler = "toggleFilter"
                            },
                        new Button
                            {
                                ID = "filterAccept", 
                                ToolTip = "Доклад принят",
                                Icon = Icon.Accept, 
                                EnableToggle = true, 
                                Pressed = true, 
                                ToggleHandler = "toggleFilter"
                            },
                        new ToolbarSeparator(),
                        new Button
                            {
                                ID = "showHierarhy", 
                                ToolTip = "Отображать показатели в иерархическом виде",
                                Icon = Icon.TableRelationship, 
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
                                                Url = "/Export/Omsu",
                                                IsUpload = true,
                                                CleanRequest = true
                                            }
                                    }
                            }
                    }
            };
        }

        private void CreateTopInfoPanel(FormPanel panel)
        {
            Label label = new Label();
            label.Text =
                "Форма ввода показателей для оценки эффективности деятельности органов местного самоуправления городских округов и муниципальных районов.";

            panel.Items.Add(label);

            label = new Label();
            label.Html = "<b>Наименование МО</b>: {0}".FormatWith(extension.UserRegionCurrent.Name);

            panel.Items.Add(label);

            label = new Label();
            label.ID = "labelYear";
            label.Html = "<b>Период</b>: {0} год".FormatWith(extension.CurrentYear);

            panel.Items.Add(label);
        }
    }
}
