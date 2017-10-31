using System;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class DetailLimitView : View
    {
        private readonly string storeId = "detailLimitStore";
        private readonly int objectId;
        private readonly D_ExcCosts_CObject curObject;
        private readonly bool canEdit;

        public DetailLimitView(IEO15ExcCostsAIPExtension extension, IConstructionService constrRepository, int objectId)
        {
            this.objectId = objectId;
            curObject = constrRepository.GetOne(objectId);
            canEdit = User.IsInRole(AIPRoles.Coordinator) ||
                ((User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient)) 
                    && extension.Client != null && extension.Client.ID == curObject.RefClients.ID);
        }

        public new GridPanel Build(ViewPage page)
        {
            return CreateGrid(page);
        }

        private static void CreateHeaderGroups(GridPanelBase gp)
        {
            var topGroup = new HeaderGroupRow();
            topGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Вид работ", Align = Alignment.Center, ColSpan = 1 });
            topGroup.Columns.Add(new HeaderGroupColumn { Header = "Лимит бюджетных ассигнований", Align = Alignment.Center, ColSpan = 6 });

            var middleGroup = new HeaderGroupRow();
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "Всего", Align = Alignment.Center, ColSpan = 1 });
            middleGroup.Columns.Add(new HeaderGroupColumn { Header = "в том числе за счет средств", ColSpan = 5 });

            gp.View.Add(new GridView { HeaderGroupRows = { topGroup, middleGroup }, ForceFit = true });
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

            ds.SetHttpProxy("/EO15AIPDetailLimit/Read")
                .SetJsonReader()
                .AddField("CObjectId")
                .AddField("PeriodId")
                .AddField("TypeWorkId")
                .AddField("TypeWorkName")
                .AddField("FactAOID")
                .AddField("FactAOValue")
                .AddField("FactSubMOID")
                .AddField("FactSubMOValue")
                .AddField("FactMOID")
                .AddField("FactMOValue")
                .AddField("FactOtherID")
                .AddField("FactOtherValue")
                .AddField("FactBudgetRFID")
                .AddField("FactBudgetRFValue")
                .AddField("FactProgramCooperationID")
                .AddField("FactProgramCooperationValue")
                .AddField("FactAll")
                .AddField("StatusDId")
                .AddField("StatusDName");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPDetailLimit/Save",
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
                             ID = "DetailLimitGrid",
                             LoadMask = { ShowMask = true, Msg = "Загрузка" },
                             SaveMask = { ShowMask = true, Msg = "Сохранение" },
                             Icon = Icon.Table,
                             Title = @"Лимит бюджетных ассигнований",
                             Closable = false,
                             Collapsible = false,
                             Frame = true,
                             StoreID = store.ID,
                             AutoExpandColumn = "TypeWorkName",
                             AutoExpandMin = 120,
                             ColumnLines = true,
                             SelectionModel =
                                 {
                                     new ExcelLikeSelectionModel()
                                 }
                         };

            var colState = gp.ColumnModel.AddColumn("StatusDName", String.Empty, DataAttributeTypes.dtString).SetWidth(40);
            CreateHeaderGroups(gp);

            gp.ColumnModel.AddColumn("TypeWorkName", String.Empty, DataAttributeTypes.dtString).SetWidth(120);
            gp.ColumnModel.AddColumn("FactAll", String.Empty, DataAttributeTypes.dtString).SetWidth(40);

            var colBudgetRF = gp.ColumnModel
                .AddColumn("FactBudgetRFValue", "Бюджет РФ", DataAttributeTypes.dtString)
                .SetWidth(60);
            var colCooperation = gp.ColumnModel
                .AddColumn("FactProgramCooperationValue", "Программа 'Сотрудничество'", DataAttributeTypes.dtString)
                .SetWidth(140);

            var isGovObj = curObject.RefClients.TypeClient == 1;

            ColumnBase colAO = null;
            ColumnBase colSubMO = null;
            if (isGovObj)
            {
                colAO = gp.ColumnModel
                    .AddColumn("FactAOValue", "бюджетные инвестиции автономного округа", DataAttributeTypes.dtString)
                    .SetWidth(140);
            }
            else
            {
                colSubMO = gp.ColumnModel
                    .AddColumn("FactSubMOValue", "субсидии муниципальным образованиям", DataAttributeTypes.dtString)
                    .SetWidth(140);
            }

            var colMO = gp.ColumnModel
                .AddColumn("FactMOValue", "бюджетов муниципальных образований", DataAttributeTypes.dtString)
                .SetWidth(140);
            var colOther = gp.ColumnModel
                .AddColumn("FactOtherValue", "прочих источников", DataAttributeTypes.dtString)
                .SetWidth(105);

            var comboPeriod = FilterControl.GetFilterPeriod(page, "/EO15AIPDetailLimit/LookupPeriods", "objId", objectId.ToString());
            gp.AddRefreshButton();
            
            if (canEdit)
            {
                if (isGovObj)
                {
                    if (colAO != null)
                    {
                        colAO.SetEditableDouble(2);
                    }
                }
                else
                {
                    if (colSubMO != null)
                    {
                        colSubMO.SetEditableDouble(2);
                    }
                }

                if (User.IsInRole(AIPRoles.Coordinator))
                {
                    colBudgetRF.SetEditableDouble(2);
                }

                colCooperation.SetEditableDouble(2);
                colMO.SetEditableDouble(2);
                colOther.SetEditableDouble(2);

                gp.AddSaveButton();

                StatusDControl.AddStatusDButtons(gp, store, colState, page, User, true);
            } 

            gp.Toolbar().Add(new ToolbarSeparator());
            
            comboPeriod.Width = 300;
            comboPeriod.LabelWidth = 50;
            FilterControl.AddSelectListener(comboPeriod, gp.StoreID, "periodId");
            gp.Toolbar().Add(comboPeriod);

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }
    }
}
