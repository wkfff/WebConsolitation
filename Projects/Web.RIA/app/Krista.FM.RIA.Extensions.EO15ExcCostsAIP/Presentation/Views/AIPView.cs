using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.ExtNet.Extensions.PeriodField;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Properties;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class AIPView : View
    {
        public const string Scope = "EO15AIP.View.AIP";

        private readonly string CObjectsStoreName = "CObjectsStore";

        public override List<Component> Build(ViewPage page)
        {
            var isInAvailableRoles = User.IsInRole(AIPRoles.Coordinator);
           
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("EO15AIP", Resources.EO15AIP);
            resourceManager.AddScript(@"
            var idTab = parent.MdiTab.hashCode('/View/EO15ExcCostsAIP');
            var tab = parent.MdiTab.getComponent(idTab);    
            if (typeof(tab.events.beforeclose) == 'object'){
                tab.events.beforeclose.clearListeners();
            }
            tab.addListener('beforeclose', beforeCloseAIP);");

            if (!isInAvailableRoles)
            {
                return new List<Component>
                           {
                               new DisplayField
                                   {
                                       Text = @"Данный пользователь не входит в группы 'МО', 'Заказчики', 'Координаторы' или 'Пользователи'. Представление недоступно."
                                   }
                           };
            }

            return new List<Component>
                       {
                           new Viewport
                               {
                                   ID = "viewportInfo",
                                   AutoScroll = true,
                                   Items =
                                       {
                                           new BorderLayout
                                               {
                                                   Center = { Items = { CreateGridAIP(page) } },
                                                   South = { Items = { CreatePanelCObject(page) } }
                                               }
                                       }
                               }
                      };
        }

        private Panel CreatePanelCObject(ViewPage page)
        {
            return new Panel
                       {
                           Split = true,
                           Collapsible = true,
                           Height = 400,
                           Items =
                               {
                                   new BorderLayout
                                       {
                                           Center = { Items = { CreateGridCObject(page) } },
                                           East = { Items = { CreateGridCObjectAll(page) } }
                                       }
                               }
                       };
        }

        private Store CreateStoreAIP()
        {
            var ds = new Store
            {
                ID = "AIPStore",
                Restful = false,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            ds.SetHttpProxy("/EO15AIP/ReadAIP")
                .SetJsonReader()
                .AddField("AIPId")
                .AddField("AIPName")
                .AddField("AIPYearId");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIP/SaveAIP",
                Method = HttpMethod.POST
            });

            ds.SetSaveLoadNotifications();

            return ds;
        }

        private GridPanel CreateGridAIP(ViewPage page)
        {
            var store = CreateStoreAIP();
            page.Controls.Add(store);

            var gp = new GridPanel
            {
                ID = "AIPGrid",
                Icon = Icon.Table,
                Closable = false,
                Collapsible = false,
                Title = @"АИП",
                Frame = true,
                StoreID = store.ID,
                AutoExpandColumn = "AIPName",
                AutoExpandMin = 100,
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                ColumnLines = true,
                SelectionModel = { new ExcelLikeSelectionModel() }
            };

            gp.ColumnModel.AddColumn("AIPName", "Наименование", DataAttributeTypes.dtString).SetWidth(200).SetEditableString();
            var config = new PeriodFieldConfig
                             {
                                 DaySelectable = false,
                                 QuarterSelectable = false,
                                 MonthSelectable = false,
                                 YearSelectable = true,
                                 ShowDay = false,
                                 ShowMonth = false,
                                 ShowQuarter = false,
                                 ShowYear = true
                             };
            gp.ColumnModel.AddColumn("AIPYearId", "Год", DataAttributeTypes.dtString).SetWidth(200).SetEditablePeriod(config);

            gp.AddRefreshButton();
            gp.AddNewRecordButton();
            var removeBtn = gp.AddRemoveRecordButton();
            removeBtn.Listeners.Click.AddAfter("{0}.baseParams.aipId = 0; {0}.reload();".FormatWith(CObjectsStoreName));
            gp.AddSaveButton();

            gp.Listeners.CellClick.AddAfter(Scope + ".updateObjectsList({0}, {1}, rowIndex);".FormatWith(store.ID, CObjectsStoreName));

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }

        private GridPanel CreateGridCObject(ViewPage page)
        {
            var store = CreateStoreCObjects();
            page.Controls.Add(store);

            var gp = new GridPanel
            {
                ID = "CObjectGrid",
                Icon = Icon.Table,
                Closable = false,
                Collapsible = false,
                Frame = true,
                Title = @"Реестр объектов АИП",
                StoreID = store.ID,
                AutoExpandMin = 100,
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                ColumnLines = true,
                SelectionModel = { new RowSelectionModel() },
                DDGroup = "objectsGroup",
                EnableDragDrop = true,
                Draggable = true
            };

            gp.ToolTips.Add(new ToolTip
            {
                TargetControl = gp,
                Delegate = ".x-grid3-row",
                TrackMouse = true,
                Listeners =
                {
                    Show =
                    {
                        Handler = @"
                        this.body.dom.innerHTML = 'Для удаления объектов из АИП перетащите их в перечень справа.';"
                            .FormatWith(gp.ID)
                    }
                }
            });

            gp.ColumnModel.AddColumn("CObjectName", "Объект", DataAttributeTypes.dtString).SetWidth(300);
            gp.ColumnModel.AddColumn("ClientName", "Заказчик", DataAttributeTypes.dtString).SetWidth(300);
            gp.AddRefreshButton();
            var saveBtn = gp.AddSaveButton();
            saveBtn.Listeners.Click.AddBefore("if ({0}.baseParams.aipId < 1) return;".FormatWith(store.ID));

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            var dropTarget = new DropTarget
            {
                Target = "={CObjectGrid.view.scroller.dom}",
                Group = "objectsAllGroup",
                NotifyDrop = { Fn = Scope + @".objListDropFn" }
            };

            page.Controls.Add(dropTarget);

            return gp;
        }

        private Store CreateStoreCObjects()
        {
            var ds = new Store
            {
                ID = CObjectsStoreName,
                Restful = false,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные в реестре объектов. Вы уверены, что хотите обновить?"
            };

            ds.SetHttpProxy("/EO15AIP/ReadCObjectsAIP")
                .SetJsonReader()
                .AddField("AIPId")
                .AddField("CObjectId")
                .AddField("CObjectName")
                .AddField("ClientId")
                .AddField("ClientName");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIP/SaveCObject",
                Method = HttpMethod.POST
            });

            ds.SetSaveLoadNotifications();

            return ds;
        }

        private GridPanel CreateGridCObjectAll(ViewPage page)
        {
            var store = CreateStoreCObjectsAll();
            page.Controls.Add(store);

            var gp = new GridPanel
            {
                ID = "CObjectGridAll",
                Icon = Icon.Table,
                Closable = false,
                Collapsible = true,
                Collapsed = false,
                Frame = true,
                Width = 400,
                Split = true,
                Title = @"Перечень всех объектов строительства",
                AutoExpandColumn = "CObjectName",
                Draggable = true,
                EnableDragDrop = true,
                DDGroup = "objectsAllGroup",
                StoreID = store.ID,
                AutoExpandMin = 100,
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                ColumnLines = true,
                SelectionModel = { new RowSelectionModel() }
            };

            gp.ToolTips.Add(new ToolTip
            {
                TargetControl = gp,
                Delegate = ".x-grid3-row",
                TrackMouse = true,
                Listeners =
                {
                    Show =
                    {
                        Handler = @"
                        this.body.dom.innerHTML = 'Для добавления объектов в АИП перетащите их в реестр объектов АИП.';"
                            .FormatWith(gp.ID)
                    }
                }
            });

            var dropAllTarget = new DropTarget
            {
                Target = "={CObjectGridAll.view.scroller.dom}",
                Group = "objectsGroup",
                NotifyDrop = { Fn = Scope + @".objAllListDropFn" }
            };

            page.Controls.Add(dropAllTarget);

            gp.ColumnModel.AddColumn("CObjectName", "Объект", DataAttributeTypes.dtString).SetWidth(200);
            gp.ColumnModel.AddColumn("ClientName", "Заказчик", DataAttributeTypes.dtString).SetWidth(200);
            gp.AddRefreshButton();

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }

        private Store CreateStoreCObjectsAll()
        {
            var ds = new Store
            {
                ID = "CObjectsStoreAll",
                Restful = false,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            ds.SetHttpProxy("/EO15AIP/ReadCObjects")
                .SetJsonReader()
                .AddField("AIPId")
                .AddField("CObjectId")
                .AddField("CObjectName")
                .AddField("ClientId")
                .AddField("ClientName");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIP/SaveCObject",
                Method = HttpMethod.POST
            });

            ds.SetSaveLoadNotifications();

            return ds;
        }
    }
}
