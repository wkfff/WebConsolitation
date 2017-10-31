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
    public class FinanceView : View
    {
        public const string Scope = "EO15AIP.View.Finance.Grid";

        private readonly string storeId = "FinanceStore";
        private readonly int objectId;
        private readonly D_ExcCosts_CObject curObject;
        private readonly bool canEdit;
        private readonly bool subMOVisible;
        private readonly bool investAOVisible;
        private readonly bool subAOVisible;

        public FinanceView(IEO15ExcCostsAIPExtension extension, IConstructionService constrRepository, int objectId)
        {
            this.objectId = objectId;
            curObject = constrRepository.GetOne(objectId);
            canEdit = User.IsInRole(AIPRoles.Coordinator) ||
                ((User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient)) && 
                extension.Client != null && 
                extension.Client.ID == curObject.RefClients.ID);

            subMOVisible = User.IsInRole(AIPRoles.Coordinator) || User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.User);
            investAOVisible = User.IsInRole(AIPRoles.Coordinator) || User.IsInRole(AIPRoles.GovClient) || User.IsInRole(AIPRoles.User);
            subAOVisible = User.IsInRole(AIPRoles.Coordinator) || User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.User);
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("EO15AIPFinance", Resources.EO15AIPFinance);
            resourceManager.RegisterClientScriptBlock("EO15AIPRegister", Resources.EO15AIPRegister);
            resourceManager.RegisterClientStyleBlock("EO15AIPStyle", Resources.EO15AIPStyles);

            resourceManager.AddScript(@"
            var idTab = parent.MdiTab.hashCode('/EO15AIPRegister/ShowFinance?objId=' + {0});
            var tab = parent.MdiTab.getComponent(idTab);    
            if (typeof(tab.events.beforeclose) == 'object'){{
                tab.events.beforeclose.clearListeners();
            }}
            tab.addListener('beforeclose', beforeCloseFinance);".FormatWith(curObject.ID));

            var isInAvailableRoles = User.IsInRole(AIPRoles.MOClient) ||
                User.IsInRole(AIPRoles.GovClient) ||
                User.IsInRole(AIPRoles.Coordinator) ||
                User.IsInRole(AIPRoles.User);

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
                            ID = "viewportFinance",
                            AutoScroll = true,
                            Items =
                                {
                                    new BorderLayout { Center = { Items = { CreateGrid(page) } } }
                                }
                        }
                };
}

        private void CreateHeaderGroups(GridPanelBase gp)
        {
            // верхний уровень заголовков
            var topGroup = new HeaderGroupRow();
            var colSpan = subMOVisible && investAOVisible ? 8 : 7;
            topGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });

            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Финансирование за отчетный период", Align = Alignment.Center, ColSpan = colSpan });
            if (subAOVisible)
            {
                topGroup.Columns.Add(new HeaderGroupColumn
                    {
                        Header = "Профинансировано МО за отчетный период за счет субсидий автономного округа",
                        Align = Alignment.Center,
                        ColSpan = 1
                    });
            }

            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Освоено за отчетный период в текущих ценах", Align = Alignment.Center, ColSpan = 1 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Освоено за отчетный период в базовых ценах", Align = Alignment.Center, ColSpan = 1 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Остаток средств в базовых ценах", Align = Alignment.Center, ColSpan = 1 });
            
            // средний уровень заголовков
            var middleGroup = new HeaderGroupRow();
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "Всего", Align = Alignment.Center, ColSpan = 1 });
            var colSpanM = subMOVisible && investAOVisible ? 7 : 6;
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "в том числе за счет средств", ColSpan = colSpanM });
            if (subAOVisible)
            {
                middleGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            }

            middleGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });

            // нижний уровень заголовков
            var bottomGroup = new HeaderGroupRow();
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            var colSpanB = subMOVisible && investAOVisible ? 2 : 1;
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = "бюджет округа", ColSpan = colSpanB });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = "бюджетов муниципальных образований", ColSpan = 1 });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = "других источников", ColSpan = 1 });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = "Объем средств из внебюджетных источников", ColSpan = 1 });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = "Бюджет РФ", Align = Alignment.Center, ColSpan = 1 });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = "Программа 'Сотрудничество'", Align = Alignment.Center, ColSpan = 1 });
            if (subAOVisible)
            {
                bottomGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            }

            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });
            bottomGroup.Columns.Add(new HeaderGroupColumn { Header = string.Empty, ColSpan = 1 });

            gp.View.Add(new GridView { HeaderGroupRows = { topGroup, middleGroup, bottomGroup }, ForceFit = true });
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

            ds.SetHttpProxy("/EO15AIPFinance/Read")
                .SetJsonReader()
                .AddField("CObjectId")
                .AddField("PeriodId")
                .AddField("PeriodName")
                .AddField("FactAOID")
                .AddField("FactAOValue")
                .AddField("FactSubMOID")
                .AddField("FactSubMOValue")
                .AddField("FactMOID")
                .AddField("FactMOValue")
                .AddField("FactOtherID")
                .AddField("FactOtherValue")
                .AddField("FactAll")
                .AddField("FactSubAOValue")
                .AddField("FactSubAOID")
                .AddField("FactMasterValue")
                .AddField("FactMasterID")
                .AddField("FactMasterBasePriceValue")
                .AddField("FactMasterBasePriceID")
                .AddField("FactBalanceBasePriceValue")
                .AddField("FactBalanceBasePriceID")
                .AddField("FactBudgetRFID")
                .AddField("FactBudgetRFValue")
                .AddField("FactOutOfBudgetID")
                .AddField("FactOutOfBudgetValue")
                .AddField("Editable")
                .AddField("FactProgramCooperationID")
                .AddField("FactProgramCooperationValue")
                .AddField("StatusDId")
                .AddField("StatusDName");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPFinance/Save",
                Method = HttpMethod.POST
            });

            ds.BaseParams.Add(new Parameter("objectId", objectId.ToString(), ParameterMode.Raw));

            ds.SetSaveLoadNotifications();

            return ds;
        }

        private GridPanel CreateGrid(ViewPage page)
        {
            var store = CreateStore();
            page.Controls.Add(store);

            var gp = new GridPanel
            {
                ID = "FinanceGrid",
                Icon = Icon.Table,
                Closable = false,
                Collapsible = false,
                Frame = true,
                StoreID = store.ID,
                AutoExpandMin = 100,
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                ColumnLines = true,
                SelectionModel = { new ExcelLikeSelectionModel() }
            };

            CreateHeaderGroups(gp);

            var rendererFn = Scope + ".rendererFn";

            var colState = gp.ColumnModel.AddColumn("StatusDName", String.Empty, DataAttributeTypes.dtString).SetWidth(40);
            gp.ColumnModel.AddColumn("PeriodName", "Период", DataAttributeTypes.dtString).SetWidth(100);
            var colAll = gp.ColumnModel.AddColumn("FactAll", string.Empty, DataAttributeTypes.dtString).SetWidth(40);
            colAll.Renderer.Fn = rendererFn;
            colAll.Align = Alignment.Right;
            
            ColumnBase colInvestAO = null;
            ColumnBase colSubMO = null;
            ColumnBase colSubAO = null;

            if (investAOVisible)
            {
                colInvestAO = gp.ColumnModel.AddColumn(
                        "FactAOValue", 
                        "бюджетные инвестиции автономного округа",
                        DataAttributeTypes.dtString)
                    .SetWidth(130);
                colInvestAO.Renderer.Fn = rendererFn;
            }

            if (subMOVisible)
            {
                colSubMO = gp.ColumnModel.AddColumn("FactSubMOValue", "субсидии муниципальным образованиям", DataAttributeTypes.dtString).SetWidth(135);
                colSubMO.Renderer.Fn = rendererFn;
            }

            var colMO = gp.ColumnModel.AddColumn("FactMOValue", string.Empty, DataAttributeTypes.dtString).SetWidth(160);
            colMO.Renderer.Fn = rendererFn;
            var colOther = gp.ColumnModel.AddColumn("FactOtherValue", string.Empty, DataAttributeTypes.dtString).SetWidth(105);
            colOther.Renderer.Fn = rendererFn;

            var colOutOdBudget = gp.ColumnModel.AddColumn("FactOutOfBudgetValue", string.Empty, DataAttributeTypes.dtString).SetWidth(105);
            colOutOdBudget.Renderer.Fn = rendererFn;
            
            var colBudget = gp.ColumnModel.AddColumn("FactBudgetRFValue", string.Empty, DataAttributeTypes.dtString).SetWidth(105);
            colBudget.Renderer.Fn = rendererFn;
            var colProgram = gp.ColumnModel.AddColumn("FactProgramCooperationValue", string.Empty, DataAttributeTypes.dtString).SetWidth(105);
            colProgram.Renderer.Fn = rendererFn;

            if (subAOVisible)
            {
                colSubAO = gp.ColumnModel.AddColumn("FactSubAOValue", string.Empty, DataAttributeTypes.dtString).SetWidth(125);
                colSubAO.Renderer.Fn = rendererFn;
            }

            var colMaster = gp.ColumnModel.AddColumn("FactMasterValue", string.Empty, DataAttributeTypes.dtString).SetWidth(105);
            colMaster.Renderer.Fn = rendererFn;

            var colMasterBasePrice = gp.ColumnModel.AddColumn("FactMasterBasePriceValue", string.Empty, DataAttributeTypes.dtString).SetWidth(105);
            colMasterBasePrice.Renderer.Fn = rendererFn;

            var colBalanceBasePrice = gp.ColumnModel.AddColumn("FactBalanceBasePriceValue", string.Empty, DataAttributeTypes.dtString).SetWidth(105);
            colBalanceBasePrice.Renderer.Fn = rendererFn;

            gp.AddRefreshButton();

            if (canEdit)
            {
                if (colInvestAO != null)
                {
                    colInvestAO.SetEditableDouble(2);
                }

                if (colSubMO != null)
                {
                    colSubMO.SetEditableDouble(2);
                }

                colMO.SetEditableDouble(2);
                colOther.SetEditableDouble(2);
                colOutOdBudget.SetEditableDouble(2);

                if (colSubAO != null)
                {
                    colSubAO.SetEditableDouble(2);
                }

                colMaster.SetEditableDouble(2);
                colMasterBasePrice.SetEditableDouble(2);
                colBalanceBasePrice.SetEditableDouble(2);

                if (User.IsInRole(AIPRoles.Coordinator))
                {
                    colBudget.SetEditableDouble(2);
                }
                else
                {
                    colBudget.Align = Alignment.Right;
                }

                colProgram.SetEditableDouble(2);

                gp.AddSaveButton();

                StatusDControl.AddStatusDButtons(gp, store, colState, page, User, false);
                
                gp.Listeners.BeforeEdit.AddAfter("return (e.record.get('Editable'))");
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
