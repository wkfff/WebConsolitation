using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class DetailExpertiseView : View
    {
        private readonly string storeId = "detailExpertiseStore";
        private readonly int objectId;
        private readonly D_ExcCosts_CObject curObject;
        private readonly bool canEdit;

        public DetailExpertiseView(IEO15ExcCostsAIPExtension extension, IConstructionService constrRepository, int objectId)
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

            ds.SetHttpProxy("/EO15AIPDetailExpertise/Read")
                .SetJsonReader()
                .AddField("CObjectId")
                .AddField("StatusDId")
                .AddField("StatusDName")
                .AddField("TypeWorkId")
                .AddField("TypeWorkName")
                .AddField("AuditId")
                .AddField("Fact2001Id")
                .AddField("Fact2001Value")
                .AddField("FactCurId")
                .AddField("FactCurValue");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPDetailExpertise/Save",
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
                ID = "DetailExpertiseGrid",
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                Icon = Icon.Table,
                Title = @"Сметная стоимость по экспертизе",
                Closable = false,
                Collapsible = false,
                Frame = true,
                StoreID = store.ID,
                AutoExpandColumn = "TypeWorkName",
                ColumnLines = true,
                SelectionModel =
                                 {
                                     new ExcelLikeSelectionModel()
                                 }
            };

            var colState = gp.ColumnModel.AddColumn("StatusDName", "Статус данных", DataAttributeTypes.dtString).SetWidth(100);
            
            gp.ColumnModel.AddColumn("TypeWorkName", "Вид работ", DataAttributeTypes.dtString).SetWidth(300);

            var col2001 = gp.ColumnModel.AddColumn(
                "Fact2001Value", 
                "Сметная стоимость по экспертизе в ценах 2001 года", 
                DataAttributeTypes.dtString)
                .SetWidth(300);
            var colCur = gp.ColumnModel.AddColumn(
                "FactCurValue", 
                "Стоимость по экспертизе в текущих ценах с НДС",
                DataAttributeTypes.dtString)
                .SetWidth(300);

            gp.AddRefreshButton().Listeners.Click.Handler += "expertiseStore.reload();";

            if (canEdit)
            {
                col2001.SetEditableDouble(2);
                colCur.SetEditableDouble(2);
                gp.AddSaveButton();

                StatusDControl.AddStatusDButtons(gp, store, colState, page, User, true);
            }

            gp.Toolbar().Add(new ToolbarSeparator());
            CreateExpertiseCombo(page, gp);
            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }

        private void CreateExpertiseCombo(ViewPage page, GridPanel gp)
        {
            var comboPeriod = FilterControl.GetFilterExpertise(page);
            comboPeriod.Width = 300;
            comboPeriod.LabelWidth = 70;
            FilterControl.AddSelectListener(comboPeriod, gp.StoreID, "expertiseId");
            gp.Toolbar().Add(comboPeriod);

            if (canEdit)
            {
                var winId = @"ExpertiseModalWin";
                var winBookExpertise = CommonControl.GetBookWindow(
                    page,
                    gp.ID,
                    @"Экспертиза",
                    winId,
                    "Открытие списка экспертиз",
                    "{0}.hide(); expertiseStore.reload();".FormatWith(winId), 
                    @"Закрыть", 
                    true);
                var btnCancel = winBookExpertise.Buttons.Find(x => x.ID != null && x.ID.Equals("btnCancel"));
                if (btnCancel != null)
                {
                    btnCancel.Hidden = true;
                }

                var expertiseBookHandler = @"
                {0}
                {1}.autoLoad.url = '/Entity/Show?objectKey=e9dbe1ca-2679-41c3-8c3d-3029b358c3f0';
                {1}.show();
                ".FormatWith(winBookExpertise.ToScript(), winBookExpertise.ID, gp.ID);

                var expEditBtn = gp.Toolbar()
                    .AddIconButton(
                        "{0}EditExpertisedBtn".FormatWith(gp.ID),
                        Icon.ApplicationFormEdit,
                        string.Empty,
                        expertiseBookHandler);

                expEditBtn.Text = @"Редактировать список экспертиз";
            }
        }
    }
}
