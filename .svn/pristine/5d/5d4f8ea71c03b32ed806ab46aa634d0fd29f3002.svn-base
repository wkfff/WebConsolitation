using System;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.ExtNet.Extensions.PeriodField;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Properties;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class DetailReviewView : View
    {
        public const string ScopeCommon = "EO15AIP.View.Register.Grid";
        public const string Scope = "EO15AIP.View.Review.Grid";
        
        private readonly string storeId = "detailReviewStore";
        private readonly int objectId;
        private readonly D_ExcCosts_CObject curObject;
        private readonly bool canEdit;

        public DetailReviewView(
            IEO15ExcCostsAIPExtension extension, 
            IConstructionService constrRepository, 
            int objectId)
        {
            this.objectId = objectId;
            curObject = constrRepository.GetOne(objectId);
            canEdit = User.IsInRole(AIPRoles.Coordinator) ||
                ((User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient))
                    && extension.Client != null && extension.Client.ID == curObject.RefClients.ID);
        }

        public new GridPanel Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("EO15AIPDetailReview", Resources.EO15AIPDetailReview);

            return CreateGrid(page);
        }

        private void CreatePeriodCombo(ViewPage page, GridPanel gp)
        {
            var oneYearUndefined = curObject == null || curObject.StartConstruction == null || curObject.EndConstruction == null;
            var yearFrom = oneYearUndefined ? 1 : curObject.StartConstruction.Value.Year;
            var yearTo = oneYearUndefined ? 0 : curObject.EndConstruction.Value.Year;
            var comboPeriod = FilterControl.GetFilterQuarterPeriod(page, yearFrom, yearTo);
            comboPeriod.Width = 300;
            comboPeriod.LabelWidth = 70;
            FilterControl.AddSelectListener(comboPeriod, gp.StoreID, "periodId");
            gp.Toolbar().Add(comboPeriod);
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

            ds.SetHttpProxy("/EO15AIPDetailReview/Read")
                .SetJsonReader()
                .AddField("Result")
                .AddField("PeriodId")
                .AddField("ID")
                .AddField("CObjectId")
                .AddField("StatusDId")
                .AddField("StatusDName");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPDetailReview/Save",
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
                ID = "DetailReviewGrid",
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                Icon = Icon.Table,
                Title = @"Конъюнктурный обзор",
                Closable = false,
                Collapsible = false,
                Frame = true,
                StoreID = store.ID,
                AutoExpandColumn = "Result",
                AutoExpandMin = 150,
                ColumnLines = true,
                SelectionModel =
                                 {
                                     new ExcelLikeSelectionModel()
                                 }
            };

            var colState = gp.ColumnModel.AddColumn("StatusDName", "Статус данных", DataAttributeTypes.dtString).SetWidth(100);
            var colPeriod = gp.ColumnModel.AddColumn("PeriodId", "Период", DataAttributeTypes.dtString).SetWidth(100);
            var colResult = gp.ColumnModel
                .AddColumn("Result", "Конъюнктурный обзор", DataAttributeTypes.dtString)
                .SetWidth(200);

            gp.AddRefreshButton();
            if (canEdit)
            {
                colResult.SetEditableString();
                var config = new PeriodFieldConfig
                {
                    ShowYear = false,
                    ShowQuarter = true,
                    ShowMonth = false,
                    ShowDay = false,
                    YearSelectable = false,
                    QuarterSelectable = true,
                    MonthSelectable = false,
                    DaySelectable = false,
                    MinDate = curObject.StartConstruction,
                    MaxDate = curObject.EndConstruction
                };

                colPeriod.SetEditablePeriod(config);

                gp.AddSaveButton();

                var stateId = User.IsInRole(AIPRoles.Coordinator) ? (int)AIPStatusD.Review : (int)AIPStatusD.Edit;
                var stateName = User.IsInRole(AIPRoles.Coordinator) ? "На рассмотрении" : "На редактировании";

                gp.Toolbar().AddIconButton(
                    "{0}NewRecordBtn".FormatWith(gp.ID),
                    Icon.Add,
                    "Добавить новую запись",
                    Scope + ".addRecordInGrid('{0}', {1}, {2}, '{3}')".FormatWith(gp.ID, objectId, stateId, stateName));

                gp.AddRemoveRecordButton().Listeners.Click.Handler =
                    ScopeCommon + ".deleteRecordInGrid('{0}', '{1}')".FormatWith(gp.ID, gp.ID);

                StatusDControl.AddStatusDButtons(gp, store, colState, page, User, true);
            }

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }
    }
}
