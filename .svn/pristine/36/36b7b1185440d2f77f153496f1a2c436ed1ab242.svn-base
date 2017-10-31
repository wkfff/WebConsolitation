using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Views
{
    public class MarksOmsuReportView : View
    {
        private readonly IMarksOmsuExtension extension;
        private readonly MarksOmsuGridControl grid;

        public MarksOmsuReportView(IMarksOmsuExtension extension)
        {
            this.extension = extension;

            grid = new MarksOmsuGridControl(extension) { StoreController = "MarksOmsuReport" };
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            
            resourceManager.RegisterScript("ViewPersistence", "/Content/js/ViewPersistence.js");
            
            if (extension.User.Name != "econom1")
            {
                resourceManager.RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен район.").ToScript());

                return new List<Component>();
            }

            resourceManager.RegisterClientScriptBlock("MarksOmsuReportView", Resources.Resource.MarksOmsuReportView);
            resourceManager.RegisterClientStyleBlock(
                "CustomLabelStyle",
                "label.x-form-item-label{ font-weight: bold;}");

            resourceManager.RegisterOnReadyScript(@"
viewportMain.doLayout(); 
ViewPersistence.refresh.call(window);");

            Viewport viewport = new Viewport
            {
                ID = "viewportMain",
                Items =
                    {
                        new BorderLayout
                            {
                                North = { Items = { CreateTopInfoPanel() } },
                                Center = { Items = { grid.Build(page) } }
                            }
                    }
            };

            grid.Store.AutoLoad = false;
            grid.Store.BaseParams.Add(new Parameter("regionId", "regionsCombo.getSelectedItem().value", ParameterMode.Raw));
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
labelYear.update(year +' год');
";

            grid.GridPanel.Listeners.BeforeEdit.Fn = "beforeEditCellReadonly";

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
                            ID = "refreshButton",
                            Icon = Icon.PageRefresh,
                            ToolTip = "Обновить",
                            Listeners = { Click = { Handler = "ViewPersistence.refresh.call(window);" } }
                        },
                        new ToolbarSeparator(),
                        new Button
                        {
                            ID = "acceptButton",
                            Icon = Icon.Accept,
                            ToolTip = "Утвердить выделенные показатели",
                            Listeners = { Click = { Fn = "acceptButtonClick" } }
                        },
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
                            ID = "acceptReportButton",
                            Icon = Icon.ReportKey,
                            ToolTip = "Утвердить доклад",
                            DirectEvents =
                                {
                                    Click =
                                        {
                                            Url = "/MarksOmsuReport/AcceptReport",
                                            CleanRequest = true,
                                            Success = "dsMarks.load();",
                                            EventMask = { ShowMask = true },
                                            ExtraParams =
                                                {
                                                    new Parameter("regionId", "regionsCombo.getSelectedItem().value", ParameterMode.Raw)
                                                }
                                        }
                                }
                        },
                        new Button
                        {
                            ID = "rejectReportButton",
                            Icon = Icon.ReportEdit,
                            ToolTip = "Вернуть доклад на утверждение",
                            DirectEvents =
                                {
                                    Click =
                                        {
                                            Url = "/MarksOmsuReport/RejectReport",
                                            CleanRequest = true,
                                            Success = "dsMarks.load();",
                                            EventMask = { ShowMask = true },
                                            ExtraParams =
                                                {
                                                    new Parameter("regionId", "regionsCombo.getSelectedItem().value", ParameterMode.Raw)
                                                }
                                        }
                                }
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
                                ID = "calculatePreviousResultFilter",
                                Icon = Icon.HourglassLink,
                                ToolTip = "Выводить данные доклада за предыдущий год",
                                EnableToggle = true,
                                Pressed = false,
                                ToggleHandler = "toggleFilter"
                            }
                    }
            };
        }

        private ComboBox CreateRegionsCombo()
        {
            ComboBox comboBox = new ComboBox
            {
                ID = "regionsCombo",
                Width = 400,
                FieldLabel = "Наименование МО",
                Editable = false,
                ValueField = "ID",
                DisplayField = "Name",
                Store = { CreateRegionsSelectStore() },
                TriggerAction = TriggerAction.All,
                AutoShow = true,
                Listeners =
                    { 
                      Select = { Handler = "dsMarks.reload();" }
                    }
            };
            
            return comboBox;
        }

        private FormPanel CreateTopInfoPanel()
        {
            return new FormPanel
            {
                Border = false,
                Layout = "form",
                AutoHeight = true,
                LabelWidth = 125,
                LabelAlign = LabelAlign.Right,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                TopBar = { CreateToolbar() },
                Items =
                {
                    new Label { Text = "Форма утверждения показателей для оценки эффективности деятельности органов местного самоуправления городских округов и муниципальных районов." },
                    new DisplayField { ID = "labelYear", FieldLabel = "Период", Text = Convert.ToString(extension.CurrentYear) },
                    CreateRegionsCombo()
                }
            };
        }

        private Store CreateRegionsSelectStore()
        {
            var store = new Store { ID = "dsRegionsLoockup", AutoLoad = false };
            store.SetHttpProxy("/MarksOmsuReport/GetRegionList");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                    new RecordField("RegionCodeLine")
                }
            });

            store.SortInfo.Field = "RegionCodeLine";
            store.SortInfo.Direction = SortDirection.ASC;

            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка территорий', response.responseText);";
            store.Listeners.Load.Handler = @"
eval(store.reader.jsonData.extraParams);
regionsCombo.lastQuery = ''; 

if (regionsCombo.store.data.length > 0) {
   regionsCombo.selectByIndex(0);
} else {
   regionsCombo.clear();
}
regionsCombo.fireEvent('select');
";
            return store;
        }
    }
}
