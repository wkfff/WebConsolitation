using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Properties;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class DetailPlanView : View 
    {
        public const string ScopeCommon = "EO15AIP.View.Register.Grid";
        public const string Scope = "EO15AIP.View.Plan.Grid";

        private readonly string storeId = "detailPlanStore";
        private readonly int objectId;
        private readonly D_ExcCosts_CObject curObject;
        private readonly bool canEdit;

        public DetailPlanView(IEO15ExcCostsAIPExtension extension, IConstructionService constrRepository, int objectId)
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
            resourceManager.RegisterClientScriptBlock("EO15AIPDetailPlan", Resources.EO15AIPDetailPlan);
            return CreateGrid(page);
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

            ds.SetHttpProxy("/EO15AIPDetailPlan/Read")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Year")
                .AddField("StartConstruction")
                .AddField("EndConstruction")
                .AddField("SourceFinance")
                .AddField("SourceFinanceId")
                .AddField("CObjectId")
                .AddField("StatusDId")
                .AddField("StatusDName");

            var start = curObject.StartConstruction == null ? 0 : curObject.StartConstruction.Value.Year;
            var end = curObject.EndConstruction == null ? 0 : curObject.EndConstruction.Value.Year;
            if (start != 0 && end != 0)
            {
                for (var i = start; i <= end; i++)
                {
                    ds.AddField("Year{0}".FormatWith(i));
                    ds.AddField("Fact{0}Id".FormatWith(i));
                }
            }

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPDetailPlan/Save",
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
                ID = "DetailPlanGrid",
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                Icon = Icon.Table,
                Title = @"Планируемое финансирование",
                Closable = false,
                Collapsible = false,
                AutoExpandColumn = "SourceFinance",
                AutoExpandMin = 200,
                Frame = true,
                StoreID = store.ID,
                ColumnLines = true,
                SelectionModel =
                    {
                        new ExcelLikeSelectionModel()
                    }
            };

            var colState = gp.ColumnModel.AddColumn("StatusDName", "Статус данных", DataAttributeTypes.dtString).SetWidth(100);
            var colSourceFinance = gp.ColumnModel.AddColumn("SourceFinance", "Источник финансирования", DataAttributeTypes.dtString).SetWidth(80);

            var start = curObject.StartConstruction == null ? 0 : curObject.StartConstruction.Value.Year;
            var end = curObject.EndConstruction == null ? 0 : curObject.EndConstruction.Value.Year;
            if (start != 0 && end != 0)
            {
                for (var i = start; i <= end; i++)
                {
                    gp.ColumnModel.AddColumn("Year{0}".FormatWith(i), i.ToString(), DataAttributeTypes.dtString).SetWidth(80);
                }
            }

            gp.AddRefreshButton(); 

            if (canEdit)
            {
                colSourceFinance.AddLookupIdNameForColumn(
                    "SourceFinanceId",
                    "SourceFinance",
                    "/Entity/DataWithCustomSearch?objectKey=8befa9da-279a-457f-bec3-7c2275fe15ca&serverFilter=(NOTE is not null)",
                    false,
                    page);

                for (var i = start; i <= end; i++)
                {
                    var colName = "Year{0}".FormatWith(i);
                    var colYearI = gp.ColumnModel.GetColumnById(colName);
                    if (colYearI != null)
                    {
                        colYearI.SetEditableDouble(2);
                    }
                }

                var stateId = User.IsInRole(AIPRoles.Coordinator) ? (int)AIPStatusD.Review : (int)AIPStatusD.Edit;
                var stateName = User.IsInRole(AIPRoles.Coordinator) ? "На рассмотрении" : "На редактировании";

                gp.AddNewRecordButton().Listeners.Click.Handler = 
                    Scope + @".addRecordInGrid({0}, {1}, {2}, '{3}');".FormatWith(gp.ID, curObject.ID, stateId, stateName);
                gp.AddRemoveRecordButton().Listeners.Click.Handler =
                    ScopeCommon + ".deleteRecordInGrid('{0}', '{1}')".FormatWith(gp.ID, gp.ID);
                gp.AddSaveButton();

                StatusDControl.AddStatusDButtons(gp, store, colState, page, User, false);
            }
            
            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }
    }
}
