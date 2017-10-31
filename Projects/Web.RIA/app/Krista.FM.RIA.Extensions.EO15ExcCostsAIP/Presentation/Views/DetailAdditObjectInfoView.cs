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

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class DetailAdditObjectInfoView : View
    {
        public const string ScopeCommon = "EO15AIP.View.Register.Grid";
        public const string Scope = "EO15AIP.View.AdditObjectInfo.Grid";

        private readonly string storeId = "detailAdditObjectInfoStore";
        private readonly int objectId;
        private readonly D_ExcCosts_CObject curObject;
        private readonly bool canEdit;

        public DetailAdditObjectInfoView(IEO15ExcCostsAIPExtension extension, IConstructionService constrRepository, int objectId)
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
            resourceManager.RegisterClientScriptBlock("EO15AIPDetailAdditObjectInfo", Resources.EO15AIPDetailAdditObjectInfo);

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

            ds.SetHttpProxy("/EO15AIPDetailAdditObjectInfo/Read")
                .SetJsonReader()
                .AddField("MarkName")
                .AddField("MarkValue")
                .AddField("MarkId")
                .AddField("CObjectId")
                .AddField("PeriodId");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPDetailAdditObjectInfo/Save",
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
                ID = "DetailAdditObjectInfoGrid",
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                Icon = Icon.Table,
                Title = @"Дополнительная информация по объекту",
                Closable = false,
                Collapsible = false,
                Frame = true,
                StoreID = store.ID,
                AutoExpandColumn = "MarkName",
                ColumnLines = true,
                SelectionModel = { new ExcelLikeSelectionModel() },
                AutoExpandMin = 100
            };

            var columnName = gp.ColumnModel.AddColumn("MarkName", "Наименование показателя", DataAttributeTypes.dtString).SetWidth(100);
            var columnValue = gp.ColumnModel.AddColumn("MarkValue", "Значение показателя", DataAttributeTypes.dtString).SetWidth(120);
            
            gp.AddRefreshButton();

            if (canEdit)
            {
                columnName.SetEditableString();
                columnValue.SetEditableString();
                gp.AddSaveButton();

                gp.Toolbar()
                    .AddIconButton(
                        "{0}NewRecordBtn".FormatWith(gp.ID),
                        Icon.Add,
                        "Добавить новую запись",
                        Scope + ".addRecordInGrid('{0}', {1})".FormatWith(gp.ID, objectId));

                gp.AddRemoveRecordButton().Listeners.Click.Handler =
                    ScopeCommon + ".deleteRecordInGrid('{0}', '{1}')".FormatWith(gp.ID, gp.ID); 
            }

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            CreateExpertiseCombo(page, gp, objectId);

            return gp;
        }

        private void CreateExpertiseCombo(ViewPage page, GridPanel gp, int objectId)
        {
            var periodStore = FilterControl.CreateFilterStore("periodsObjectStore", "/EO15AIPDetailAdditObjectInfo/LookupPeriods", "objectId", objectId.ToString());
            page.Controls.Add(periodStore);
            var comboPeriod = new ComboBox
            {
                EmptyText = @"Выберите значение",
                StoreID = periodStore.ID,
                ID = "periodsObject",
                Editable = false,
                FieldLabel = @"Период",
                TriggerAction = TriggerAction.All,
                ValueField = "ID",
                Width = 100,
                Disabled = false,
                DisplayField = "Name"
            };
            comboPeriod.Width = 300;
            comboPeriod.LabelWidth = 70;
            FilterControl.AddSelectListener(comboPeriod, gp.StoreID, "PeriodId");
            gp.Toolbar().Add(comboPeriod);
        }
    }
}
