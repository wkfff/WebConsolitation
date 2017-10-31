using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Properties;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class InfoView : View
    {        
        private readonly string storeId = "InfoStore";
        private readonly int objectId;
        private readonly D_ExcCosts_CObject curObject;
        private readonly bool canEdit;

        public InfoView(IEO15ExcCostsAIPExtension extension, IConstructionService constrRepository, int objectId)
        {
            this.objectId = objectId;
            curObject = constrRepository.GetOne(objectId);
            canEdit = User.IsInRole(AIPRoles.Coordinator) ||
                ((User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient)) && 
                extension.Client != null && 
                extension.Client.ID == curObject.RefClients.ID);
        }

        public override List<Component> Build(ViewPage page)
        {
            var isInAvailableRoles = User.IsInRole(AIPRoles.MOClient) ||
                User.IsInRole(AIPRoles.GovClient) ||
                User.IsInRole(AIPRoles.Coordinator) ||
                User.IsInRole(AIPRoles.User);

            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("EO15AIPRegister", Resources.EO15AIPRegister);
            resourceManager.AddScript(@"
            var idTab = parent.MdiTab.hashCode('/EO15AIPRegister/ShowInfo?objId=' + {0});
            var tab = parent.MdiTab.getComponent(idTab);    
            if (typeof(tab.events.beforeclose) == 'object'){{
                tab.events.beforeclose.clearListeners();
            }}
            tab.addListener('beforeclose', beforeCloseInfo);".FormatWith(curObject.ID));

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
                                           new BorderLayout { Center = { Items = { CreateGrid(page) } } }
                                       }
                               }
                       };
        }

        private Store CreateStore()
        {
            var ds = new Store
            {
                ID = storeId,
                Restful = false,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            ds.SetHttpProxy("/EO15AIPInfo/Read")
                .SetJsonReader()
                .AddField("CObjectId")
                .AddField("PeriodId")
                .AddField("PeriodName")
                .AddField("Mark11")
                .AddField("Mark12")
                .AddField("Mark13")
                .AddField("Mark14")
                .AddField("Mark15")
                .AddField("Mark16")
                .AddField("Mark17")
                .AddField("Mark18")
                .AddField("Mark11Id")
                .AddField("Mark12Id")
                .AddField("Mark13Id")
                .AddField("Mark14Id")
                .AddField("Mark15Id")
                .AddField("Mark16Id")
                .AddField("Mark17Id")
                .AddField("Mark18Id")
                .AddField("MarkReasonsNotCom")
                .AddField("MarkReasonsNotComId")
                .AddField("StatusDId")
                .AddField("StatusDName");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPInfo/Save",
                Method = HttpMethod.POST
            });

            ds.BaseParams.Add(new Parameter("objectId", objectId.ToString(), ParameterMode.Raw));

            ds.SetSaveLoadNotifications();

            return ds;
        }

        private void CreateHeaderGroups(GridPanelBase gp)
        {
            // Верхний уровень заголовков.
            var topGroup = new HeaderGroupRow();
            topGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Введено в эксплуатацию", Align = Alignment.Center, ColSpan = 3 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Разрешение на ввод объекта в эксплуатацию", Align = Alignment.Center, ColSpan = 1 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Использование старых мощностей", Align = Alignment.Center, ColSpan = 2 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Плановая численность персонала введенного объекта", Align = Alignment.Center, ColSpan = 2 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Причины не ввода объекта в эксплуатацию", Align = Alignment.Center, ColSpan = 1 });            
            
            // Средний уровень заголовков.
            var middleGroup = new HeaderGroupRow();
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "Мощностей", ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "Основных фондов", ColSpan = 2 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "выбыло мощностей", ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "перепро-филировано под новое назначение", ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "всего", ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "в т.ч. вновь созданные рабочие места", ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });

            gp.View.Add(new GridView { HeaderGroupRows = { topGroup, middleGroup }, ForceFit = true });
        }

        private GridPanel CreateGrid(ViewPage page)
        {
            var store = CreateStore();
            page.Controls.Add(store);

            var gp = new GridPanel
            {
                ID = "InfoGrid",
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                Icon = Icon.Table,
                Closable = false,
                Collapsible = false,
                Frame = true,
                StoreID = store.ID,
                AutoExpandMin = 100,
                ColumnLines = true,
                SelectionModel = { new ExcelLikeSelectionModel() }
            };

            CreateHeaderGroups(gp);
            var colState = gp.ColumnModel.AddColumn("StatusDName", String.Empty, DataAttributeTypes.dtString).SetWidth(40);
            gp.ColumnModel.AddColumn("PeriodName", "Период", DataAttributeTypes.dtString).SetWidth(100);
            gp.ColumnModel.AddColumn("Mark11", string.Empty, DataAttributeTypes.dtString).SetWidth(70);
            gp.ColumnModel.AddColumn("Mark12", "Всего (тыс. руб.)", DataAttributeTypes.dtString).SetWidth(70);
            gp.ColumnModel.AddColumn("Mark13", "в т.ч. за счет средств бюджета автономного округа", DataAttributeTypes.dtString).SetWidth(150);
            gp.ColumnModel.AddColumn("Mark14", string.Empty, DataAttributeTypes.dtString).SetWidth(120);
            gp.ColumnModel.AddColumn("Mark15", string.Empty, DataAttributeTypes.dtString).SetWidth(80);
            gp.ColumnModel.AddColumn("Mark16", string.Empty, DataAttributeTypes.dtString).SetWidth(120);
            gp.ColumnModel.AddColumn("Mark17", string.Empty, DataAttributeTypes.dtString).SetWidth(60);
            gp.ColumnModel.AddColumn("Mark18", string.Empty, DataAttributeTypes.dtString).SetWidth(120);
            gp.ColumnModel.AddColumn("MarkReasonsNotCom", string.Empty, DataAttributeTypes.dtString).SetWidth(120);

            gp.AddRefreshButton();

            if (canEdit)
            {
                foreach (var column in gp.ColumnModel.Columns)
                {
                    column.SetEditableString();
                }

                gp.AddSaveButton();

                StatusDControl.AddStatusDButtons(gp, store, colState, page, User, false);
            }

            gp.Toolbar().Add(new ToolbarSeparator());

            var comboPeriod = FilterControl.GetFilterYearPeriod(page, curObject.StartConstruction.Value.Year, curObject.EndConstruction.Value.Year);
            comboPeriod.Width = 300;
            comboPeriod.LabelWidth = 50;
            comboPeriod.Listeners.Select.AddAfter(
            @"
                var checked = [];
                Ext.each({2}.checkedRecords, function (record) {{
                    checked.push(record.id);
                }});
                {0}.baseParams.{1} = checked;
                {0}.reload();
                ".FormatWith(gp.StoreID, "periodId", comboPeriod.ID));

            gp.Toolbar().Add(comboPeriod);

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }
    }
}
