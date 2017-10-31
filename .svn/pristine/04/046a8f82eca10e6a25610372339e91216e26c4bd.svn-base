using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.Region10MarksOIV.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Presentation.Views
{
    public class MarksOmsuView : View
    {
        private readonly IRegion10MarksOivExtension extension;

        public MarksOmsuView(IRegion10MarksOivExtension extension)
        {
            this.extension = extension;
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            resourceManager.RegisterScript("ViewPersistence", "/Content/js/ViewPersistence.js");
            
            // Проверяем есть ли у пользователя территория
            if (extension.UserTerritoryRf == null)
            {
                resourceManager.RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "У текущего пользоватя не указана территория.").ToScript());
                    return new List<Component>();
            }

            RegisterResources(resourceManager);

            FormPanel infoPanel = new FormPanel
            {
                Border = false,
                Layout = LayoutType.Form.ToString(),
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

            var marksGrid = new MarksTableControl("Region10MarksOmsu", extension.CurrentYearVal);
            layout.Center.Items.Add(marksGrid.Build(page));

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = LayoutType.Center.ToString() };
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
            resourceManager.RegisterClientScriptBlock("MarksOmsuView", Resources.Resource.MarksOmsuView);
        }

        private Toolbar CreateToolbar()
        {
            var toolbar = new Toolbar
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
                            ID = "totestButton",
                            Icon = Icon.Tick,
                            ToolTip = "Передать выделенные показатели на рассмотрение",
                            Listeners = { Click = { Fn = "toTestButtonClick" } }
                        }
                    }
            };

            toolbar.Items.Add(new ToolbarSeparator());
            toolbar.Items.Add(new Label { Text = "Фильтры:" });
            toolbar.Items.Add(new Button
                                  {
                                      ID = "filterEdit",
                                      ToolTip = "На редактировании",
                                      Icon = Icon.UserEdit,
                                      EnableToggle = true,
                                      Pressed = true,
                                      ToggleHandler = "toggleFilter"
                                  });
            toolbar.Items.Add(new Button
                                  {
                                      ID = "filterMagnify",
                                      ToolTip = "На рассмотрении",
                                      Icon = Icon.UserMagnify,
                                      EnableToggle = true,
                                      Pressed = true,
                                      ToggleHandler = "toggleFilter"
                                  });
            toolbar.Items.Add(new ToolbarSeparator());
            toolbar.Items.Add(new Button
                                  {
                                      ID = "exportReportButton",
                                      Icon = Icon.PageExcel,
                                      ToolTip = "Выгрузка в Excel",
                                      DirectEvents =
                                          {
                                              Click =
                                                {
                                                    Url = "/Region10Export/Omsu",
                                                    IsUpload = true,
                                                    CleanRequest = true
                                                }
                                          }
                                  });
            return toolbar;
        }

        private void CreateTopInfoPanel(FormPanel panel)
        {
            Label label = new Label();
            label.Text =
                "Форма ввода показателей для оценки эффективности деятельности органов местного самоуправления.";

            panel.Items.Add(label);

            label = new Label();
            label.Html = "<b>Наименование МО</b>: {0}".FormatWith(extension.UserTerritoryRf.Name);

            panel.Items.Add(label);

            label = new Label();
            label.ID = "labelYear";
            label.Html = "<b>Период</b>: {0} год".FormatWith(extension.CurrentYearVal);

            panel.Items.Add(label);
        }
    }
}
