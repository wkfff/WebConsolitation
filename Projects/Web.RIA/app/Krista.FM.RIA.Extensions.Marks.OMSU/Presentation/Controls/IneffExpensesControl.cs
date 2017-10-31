using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Principal;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls
{
    public class IneffExpensesControl
    {
        private readonly string itfCaptionShort;
        private readonly string targetMarkGetMethodName;

        public IneffExpensesControl(string targetMarkGetMethodName, string itfCaption)
        {
            this.itfCaptionShort = itfCaption;
            this.targetMarkGetMethodName = targetMarkGetMethodName;
        }

        public List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterScript("ViewPersistence", "/Content/js/ViewPersistence.js");
            resourceManager.RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");
            resourceManager.RegisterClientScriptBlock("IneffExpensesControl", Resources.Resource.IneffExpensesControl);

            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientStyleBlock("CustomStyle", @".red-text-cell { color: #FF0000 !important; }");
                resourceManager.RegisterIcon(Icon.UserEdit);
                resourceManager.RegisterIcon(Icon.UserMagnify);
                resourceManager.RegisterIcon(Icon.UserTick);
                resourceManager.RegisterIcon(Icon.Accept);
                resourceManager.RegisterIcon(Icon.Exclamation);
            }

            InstallTargetMarkStore(page);

            InstallTargetFactsStore(page);

            InstallSourceFactsStore(page);

            var viewport =
                new Viewport
                    {
                        ID = "viewportMain",
                        Items =
                            {
                                new BorderLayout
                                    {
                                        ID = "borderLayoutMain",
                                        North = { Items = { CreateTargetMarkPanel() } },
                                        Center = { Items = { CreateTargetFactsPanel() } },
                                        South = { Items = { CreateSourceFactsPanel(page) } }
                                    }
                            }
                    };

            return new List<Component> { viewport, CreateTargetFactsTooltip() };
        }

        private static void InstallTargetFactsStore(ViewPage page)
        {
            var targetFactsStore =
                new Store
                    {
                        AutoLoad = false,
                        ID = "targetFactsStore",
                        Proxy =
                            {
                                new HttpProxy
                                    {
                                        Url = "/IneffExpenses/GetTargetFacts", 
                                        Method = HttpMethod.POST
                                    }
                            },
                        UpdateProxy =
                            {
                                new HttpWriteProxy
                                    {
                                        Url = "/IneffExpenses/SaveTargetFacts", 
                                        Method = HttpMethod.POST, 
                                        Timeout = 500000
                                    }
                            },
                        BaseParams =
                        { 
                            new Parameter
                                {
                                    Name = "targetMarkId", 
                                    Value = "targetMarkID.getValue()", 
                                    Mode = ParameterMode.Raw
                                } 
                        },
                        WarningOnDirty = false,
                        Reader =
                            {
                                new JsonReader
                                    {
                                        IDProperty = "ID", 
                                        Root = "data", 
                                        Fields =
                                            {
                                                new RecordField("ID"), 
                                                new RecordField("RegionID"), 
                                                new RecordField("RefStatusData"),
                                                new RecordField("HasWarning"),
                                                new RecordField("NameMO"),

                                                new RecordField("HasWarnPrior"),
                                                new RecordField("PriorApproved"), 
                                                new RecordField("PriorCalc"),
                                                new RecordField("HasWarnCurrent"),
                                                new RecordField("CurrentApproved"), 
                                                new RecordField("CurrentCalc"),
                                            }
                                    }
                            },
                        Listeners =
                            {
                                LoadException = { Handler = "Ext.Msg.alert('Ошибка при загрузке списка значений целевых показателей', response.responseText);" },
                                Load = { Fn = "ShowDifferenceWarning" }
                            }
                    };
            page.Controls.Add(targetFactsStore);
        }

        private static void InstallSourceFactsStore(ViewPage page)
        {
            var sourceFactsStore =
                new Store
                    {
                        AutoLoad = false,
                        ID = "sourceFactsStore",
                        Proxy =
                            {
                                new HttpProxy
                                    {
                                        Url = "/IneffExpenses/GetSourceFacts", 
                                        Method = HttpMethod.POST
                                    }
                            },
                        BaseParams =
                        { 
                            new Parameter
                                {
                                    Name = "targetMarkId", 
                                    Value = "targetMarkID.getValue()", 
                                    Mode = ParameterMode.Raw
                                },
                            new Parameter
                                {
                                    Name = "regionId",
                                    Value = "targetFactsGrid.getSelectionModel().getSelected().get('RegionID')", 
                                    Mode = ParameterMode.Raw
                                } 
                        },
                        WarningOnDirty = false,
                        Reader =
                            {
                                new JsonReader
                                    {
                                        IDProperty = "ID", 
                                        Root = "data", 
                                        Fields =
                                            {
                                                new RecordField("ID"), 
                                                new RecordField("Symbol"),
                                                new RecordField("Formula"),
                                                new RecordField("Prior"),
                                                new RecordField("Current"),
                                                new RecordField("Name"),
                                                new RecordField("Level"),
                                                new RecordField("MeasureUnit"),
                                                new RecordField("Precision"),
                                            }
                                    }
                            },
                        Listeners =
                            {
                                LoadException = { Handler = "Ext.Msg.alert('Ошибка при загрузке списка значений исходных показателей', response.responseText);" },
                            }
                    };

            page.Controls.Add(sourceFactsStore);
        }

        private static Toolbar CreateTargetFactsToolbar()
        {
            var toolbar = new Toolbar();

            toolbar.Items.Add(
                new Button
                    {
                        ID = "expandDetails",
                        Icon = Icon.Help,
                        ToolTip = "Отобразить панель с исходными показателями",
                        Listeners = { Click = { Handler = "sourceFactsGrid.expand(true);" } }
                    });
           
            if (((BasePrincipal)System.Web.HttpContext.Current.User).IsInRole(MarksOMSUConstants.IneffGkhCalculateRole))
            {
                toolbar.Items.Add(new ToolbarSeparator());
                toolbar.Items.Add(
                    new Button
                        {
                            ID = "revertButton",
                            Icon = Icon.Pencil,
                            ToolTip = "Вернуть расчет показателя для выбранных МО на редактирование",
                            Listeners = { Click = { Fn = "FactsBackToEditHandler" } }
                        });
                toolbar.Items.Add(
                    new Button
                        {
                            ID = "approveButton",
                            Icon = Icon.Tick,
                            ToolTip = "Утвердить расчет показателя для выбранных МО",
                            Listeners = { Click = { Fn = "FactsApproveHandler" } }
                        });
            }

            return toolbar;
        }

        private static ToolTip CreateTargetFactsTooltip()
        {
            var toolTip = new ToolTip
            {
                ID = "RowTip",
                Target = "={targetFactsGrid.getView().mainBody}",
                Delegate = ".x-grid3-cell",
                TrackMouse = true,
                Listeners = { Show = { Fn = "TooltipShowHandler" } }
            };

            return toolTip;
        }

        private static IEnumerable<Component> CreateTargetFactsPanel()
        {
            var targetFactsGrid =
                new GridPanel
                    {
                        ID = "targetFactsGrid",
                        Title = "Значения итоговых показателей",
                        StoreID = "targetFactsStore",
                        Border = false,
                        MonitorResize = true,
                        AutoScroll = true,
                        ColumnLines = true,
                        LoadMask = { ShowMask = true, Msg = "Пересчет показателей..." },
                        SaveMask = { ShowMask = true, Msg = "Сохранение..." },

                        TopBar = { CreateTargetFactsToolbar() },

                        ColumnModel =
                            {
                                Columns =
                                    {
                                        new Column { ColumnID = "ID", DataIndex = "ID", Hidden = true, Fixed = true },
                                        new Column
                                            {
                                                ColumnID = "RefStatusData",
                                                DataIndex = "RefStatusData",
                                                Header = String.Empty,
                                                Width = 30,
                                                Fixed = true,
                                                Renderer = { Fn = "RefStatusDataRenderer" }
                                            },
                                        new Column
                                            {
                                                ColumnID = "HasWarning",
                                                DataIndex = "HasWarning",
                                                Header = String.Empty,
                                                Width = 30,
                                                Fixed = true,
                                                Renderer = { Fn = "HasWarningRenderer" }
                                            },
                                        new Column { ColumnID = "NameMO", DataIndex = "NameMO", Header = "Наименование МО", Width = 140 },                                        
                                        new Column { ColumnID = "HasWarnPrior", DataIndex = "HasWarnPrior", Header = String.Empty, Hidden = true },
                                        new Column
                                            {
                                                ColumnID = "PriorCalc", 
                                                DataIndex = "PriorCalc", 
                                                Header = "Рассчет",
                                                Css = "background-color: #EDEDED; text-align: right;", 
                                                Width = 85,
                                                Renderer = { Fn = "TargetFactPriorRenderer" }
                                            },
                                        new Column
                                            {
                                                ColumnID = "PriorApproved",
                                                DataIndex = "PriorApproved",
                                                Header = "Утверждено",
                                                Css = "background-color: #EDEDED; text-align: right;",
                                                Width = 85,
                                                Renderer = { Fn = "TargetFactPriorRenderer" }
                                            },
                                        new Column { ColumnID = "HasWarnCurrent", DataIndex = "HasWarnCurrent", Header = String.Empty, Hidden = true },
                                        new Column
                                            {
                                                ColumnID = "CurrentCalc", 
                                                DataIndex = "CurrentCalc", 
                                                Header = "Рассчет", 
                                                Width = 85,
                                                Css = "text-align: right;",
                                                Renderer = { Fn = "TargetFactCurrentRenderer" }
                                            },
                                        new Column
                                            {
                                                ColumnID = "CurrentApproved", 
                                                DataIndex = "CurrentApproved", 
                                                Header = "Утверждено", 
                                                Css = "text-align: right;",
                                                Width = 85,
                                                Renderer = { Fn = "TargetFactCurrentRenderer" }
                                            }
                                    }
                            },
                        SelectionModel =
                            {
                                new RowSelectionModel
                                    {
                                        SingleSelect = true,
                                        Listeners = { RowSelect = { Handler = @"sourceFactsStore.reload(); sourceFactsGrid.setDisabled(false);" } }
                                    }
                            },
                    };

            return new List<Component> { targetFactsGrid };
        }

        private static IEnumerable<Component> CreateSourceFactsPanel(ViewPage page)
        {
            var sourceFactsGrid =
                new GridPanel
                    {
                        ID = "sourceFactsGrid",
                        Title = "Значения исходных показателей по выбранному МО",
                        StoreID = "sourceFactsStore",
                        Border = false,
                        Height = 200,
                        Collapsed = true,
                        Collapsible = true,
                        Split = true,
                        MonitorResize = true,
                        AutoScroll = true,
                        ColumnLines = true,
                        LoadMask = { ShowMask = true, Msg = "Загрузка показателей..." },
                        Disabled = true,

                        ColumnModel =
                            {
                                Columns =
                                    {
                                        new Column { ColumnID = "srcFactsColID", DataIndex = "ID", Hidden = true },
                                        new Column { ColumnID = "srcFactsColLevel", DataIndex = "Level", Hidden = true },
                                        new Column
                                            {
                                                ColumnID = "srcFactsColName", 
                                                DataIndex = "Name", 
                                                Header = "Наименование", 
                                                Width = 400,
                                                Wrap = true,
                                                Renderer = { Fn = "SrcFactsNameRenderer" }
                                            },
                                        new Column { ColumnID = "srcFactsColSymbol", DataIndex = "Symbol", Header = "Обозначение", Width = 80 },
                                        new Column { ColumnID = "srcFactsColFormula", DataIndex = "Formula",  Header = "Формула", Width = 120 },
                                        new Column
                                            {
                                                ColumnID = "srcFactsColPrior", 
                                                DataIndex = "Prior", 
                                                Header = "(Загрузка...)", 
                                                Css = "background-color: #EDEDED; text-align: right;", 
                                                Width = 85,
                                                Renderer = { Fn = "SrcFactValueRenderer" }
                                            },
                                        new Column
                                            {
                                                ColumnID = "srcFactsColCurrent",
                                                DataIndex = "Current",
                                                Header = "(Загрузка...)",
                                                Css = "text-align: right;",
                                                Width = 85,
                                                Renderer = { Fn = "SrcFactValueRenderer" }
                                            },
                                        new Column { ColumnID = "srcFactsColMeasureUnit", DataIndex = "MeasureUnit",  Header = "Единица измерения", Width = 100 },
                                    }
                            },
                        SelectionModel = { new RowSelectionModel { SingleSelect = true } }
                    };

            sourceFactsGrid.AddColumnsWrapStylesToPage(page);
            return new List<Component> { sourceFactsGrid };
        }
        
        private IEnumerable<Component> CreateTargetMarkPanel()
        {
            var targetMarkPanel =
                new FormPanel
                    {
                        ID = "targetMarkForm",
                        Title = itfCaptionShort,
                        Border = false,
                        Padding = 5,
                        Layout = "form",
                        Collapsed = false,
                        Collapsible = true,
                        Height = 150,
                        LabelWidth = 130,
                        LabelAlign = LabelAlign.Right,
                        DefaultAnchor = "99%",
                        BodyCssClass = "x-window-mc",
                        CssClass = "x-window-mc",

                        TopBar =
                            {
                                new Toolbar
                                    {
                                        Items =
                                            {
                                                new Button
                                                    {
                                                        ID = "refreshButton",
                                                        Icon = Icon.PageRefresh,
                                                        ToolTip = "Обновить",
                                                        Listeners = { Click = { Handler = "ViewPersistence.refresh.call(window);" } }
                                                    },
                                                new Button
                                                    {
                                                        ID = "exportXlsButton",
                                                        Icon = Icon.PageExcel,
                                                        ToolTip = "Выгрузка в Excel",
                                                        DirectEvents =
                                                            {
                                                                Click =
                                                                    {
                                                                        Url = "/IneffExpenses/ExportTargetFactsToXls", 
                                                                        IsUpload = true, 
                                                                        CleanRequest = true,
                                                                        ExtraParams =
                                                                            {
                                                                                new Parameter("targetMarkId", "targetMarkID.getValue()", ParameterMode.Raw),
                                                                                new Parameter("itfCaption", "'{0}'".FormatWith(itfCaptionShort), ParameterMode.Raw),
                                                                            } 
                                                                    },
                                                            }
                                                    }
                                            }
                                    }
                            },

                        Items =
                            {
                                new Label { ID = "targetMarkDescriptionLabel", FieldLabel = "Описание", Text = "(Загрузка...)", LabelStyle = "font-weight:bold;" },
                                new Label { ID = "targetMarkCalcMarkLabel", FieldLabel = "Формула", Text = "(Загрузка...)", LabelStyle = "font-weight:bold;" },
                                new Label { ID = "targetMarkMeasureUnitLabel", FieldLabel = "Единица измерения", Text = "(Загрузка...)", LabelStyle = "font-weight:bold;" },
                                new TextField { ID = "targetMarkID", DataIndex = "ID", Hidden = true, ReadOnly = true },
                                new TextField { ID = "targetMarkDescription", DataIndex = "Description", Hidden = true, ReadOnly = true },
                                new TextField { ID = "targetMarkCalcMark", DataIndex = "CalcMark", Hidden = true, ReadOnly = true },
                                new TextField { ID = "currentYear", DataIndex = "Year", Hidden = true, ReadOnly = true },
                                new TextField { ID = "targetMarkMeasureUnit", DataIndex = "MeasureUnit", Hidden = true, ReadOnly = true },
                                new TextField { ID = "targetMarkPrecision", DataIndex = "Precision", Hidden = true, ReadOnly = true },
                            }
                    };
            return new List<Component> { targetMarkPanel };
        }
        
        private void InstallTargetMarkStore(ViewPage page)
        {
            var targetMarkStore =
                new Store
                    {
                        AutoLoad = true,
                        ID = "targetMarkStore",
                        Proxy =
                            {
                                new HttpProxy
                                    {
                                        Url = "/IneffExpenses/" + targetMarkGetMethodName,
                                        Method = HttpMethod.POST
                                    }
                            },
                        WarningOnDirty = false,
                        Reader =
                            {
                                new JsonReader
                                    {
                                        IDProperty = "ID", 
                                        Root = "data", 
                                        Fields =
                                            {
                                                new RecordField("ID"), 
                                                new RecordField("Name"),
                                                new RecordField("Description"),
                                                new RecordField("CalcMark"),
                                                new RecordField("Year"),
                                                new RecordField("MeasureUnit"),
                                                new RecordField("Precision"),
                                            }
                                    }
                            },
                        Listeners =
                            {
                                LoadException = { Handler = "Ext.Msg.alert('Ошибка при загрузке описания целевого показателя', response.responseText);" },
                                Load = { Handler = "TargetMarkStoreLoadHandler(); sourceFactsGrid.collapse(true);" }
                            }
                    };
            
            page.Controls.Add(targetMarkStore);        
        }
    }
}
