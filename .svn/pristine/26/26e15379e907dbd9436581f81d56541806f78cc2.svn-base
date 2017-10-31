using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Views
{
    public class MarksOmsuEstimateView : View
    {
        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterScript("ViewPersistence", "/Content/js/ViewPersistence.js");
            resourceManager.RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");
            resourceManager.RegisterClientScriptBlock("MarksOmsuEstimateView", Resources.Resource.MarksOmsuEstimateView);

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

            InstallSourceDataStore(page);

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
                                        South = { Items = { CreateSourceDataPanel(page) } },
                                    }
                            }
                    };

            return new List<Component> { viewport };
        }

        private static void InstallTargetMarkStore(ViewPage page)
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
                                        Url = "/OmsuEstimate/GetTargetMark",
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
                                        Url = "/OmsuEstimate/GetTargetFacts", 
                                        Method = HttpMethod.POST
                                    }
                            },
                        UpdateProxy =
                            {
                                new HttpWriteProxy
                                    {
                                        Url = "/OmsuEstimate/SaveTargetFacts", 
                                        Method = HttpMethod.POST, 
                                        Timeout = 500000
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

        private static void InstallSourceDataStore(ViewPage page)
        {
            var sourceFactsStore =
                new Store
                    {
                        AutoLoad = false,
                        ID = "sourceDataStore",
                        Proxy =
                            {
                                new HttpProxy
                                    {
                                        Url = "/OmsuEstimate/GetSourceFacts", 
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
                        Listeners = { Click = { Fn = "ToggleSourceDataPanel" } }
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

        private static IEnumerable<Component> CreateTargetMarkPanel()
        {
            var targetMarkPanel =
                new FormPanel
                    {
                        ID = "targetMarkForm",
                        Title = "Расчет оценки ОМСУ",
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
                                                                        Url = "/OmsuEstimate/ExportTargetFactsToXls", 
                                                                        IsUpload = true, 
                                                                        CleanRequest = true,
                                                                        ExtraParams =
                                                                            {
                                                                                new Parameter("targetMarkId", "targetMarkID.getValue()", ParameterMode.Raw),
                                                                            } 
                                                                    },
                                                            }
                                                    }
                                            }
                                    }
                            },

                        Items =
                            {
                                new TextField { ID = "targetMarkID", DataIndex = "ID", Hidden = true, ReadOnly = true },
                            }
                    };
            return new List<Component> { targetMarkPanel };
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
                                        new Column { ColumnID = "trgID", DataIndex = "ID", Hidden = true, Fixed = true },
                                    }
                            },
                        SelectionModel =
                            {
                                new RowSelectionModel
                                    {
                                        SingleSelect = true,
                                        Listeners = { RowSelect = { Handler = @"sourceDataStore.reload(); sourceDataGrid.setDisabled(false);" } }
                                    }
                            },
                    };

            return new List<Component> { targetFactsGrid };
        }

        private static IEnumerable<Component> CreateSourceDataPanel(ViewPage page)
        {
            var sourceFactsGrid =
                new GridPanel
                    {
                        ID = "sourceDataGrid",
                        Title = "Значения исходных показателей по выбранному МО",
                        StoreID = "sourceDataStore",
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
                                        new Column { ColumnID = "srcID", DataIndex = "ID", Hidden = true },
                                    }
                            },
                        SelectionModel = { new RowSelectionModel { SingleSelect = true } }
                    };

            sourceFactsGrid.AddColumnsWrapStylesToPage(page);
            return new List<Component> { sourceFactsGrid };        
        }
    }
}
