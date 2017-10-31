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
    public class DetailContractView : View
    {
        public const string ScopeCommon = "EO15AIP.View.Register.Grid";
        public const string Scope = "EO15AIP.View.Contract.Grid";

        private readonly string storeId = "detailContractStore";
        private readonly int objectId;
        private readonly D_ExcCosts_CObject curObject;
        private readonly bool canEdit;

        public DetailContractView(
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
            resourceManager.RegisterClientScriptBlock("EO15AIPDetailContract", Resources.EO15AIPDetailContract);

            return CreateGrid(page);
        }

        private static GroupingSummaryColumn AddColumnSum(GridPanel gp, string dataIndex, string header)
        {
            var colSumGroup = new GroupingSummaryColumn
            {
                ColumnID = dataIndex,
                DataIndex = dataIndex,
                Header = header,
                SummaryType = SummaryType.Sum
            };
            gp.ColumnModel.Columns.Add(colSumGroup);
            return colSumGroup;
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
                GroupField = "CObjectName",
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            var reader = ds.SetHttpProxy("/EO15AIPDetailContract/Read").SetJsonReader();
            reader.AddField("TypeWorkId")
                .AddField("TypeWorkName")
                .AddField("PeriodId")
                .AddField("Date")
                .AddField("ContractId")
                .AddField("ContractName")
                .AddField("PartnerId")
                .AddField("PartnerName")
                .AddField("StartPriceId")
                .AddField("StartPrice")
                .AddField("PriceId")
                .AddField("Price")
                .AddField("ExpectPriceId")
                .AddField("ExpectPrice")
                .AddField("StateId")
                .AddField("StateName")
                .AddField("StatusDId")
                .AddField("StatusDName");

            var field = new RecordField("CObjectName")
                            {
                                Type = RecordFieldType.String, 
                                DefaultValue = "'{0}'".FormatWith(curObject.Name)
                            };
            reader.AddField(field);

            var fieldId = new RecordField("CObjectId")
            {
                Type = RecordFieldType.Int,
                DefaultValue = curObject.ID.ToString()
            };
            reader.AddField(fieldId);

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPDetailContract/Save",
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
                ID = "DetailContractGrid",
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                Icon = Icon.Table,
                Title = @"Информация о контракте",
                Closable = false,
                Collapsible = false,
                Frame = true,
                StoreID = store.ID,
                ColumnLines = true,
                SelectionModel = { new ExcelLikeSelectionModel() },
                AutoExpandMin = 100
            };

            gp.View.Add(new GroupingView
                            {
                                ForceFit = true, 
                                ShowGroupName = false, 
                                EnableNoGroups = false, 
                                EnableGroupingMenu = false
                            });

            var colState = gp.ColumnModel.AddColumn("StatusDName", "Статус данных", DataAttributeTypes.dtString).SetWidth(100);
            CreateColumns(page, gp);

            gp.AddRefreshButton();

            if (canEdit)
            {
                gp.AddSaveButton();
                var stateId = User.IsInRole(AIPRoles.Coordinator) ? (int)AIPStatusD.Review : (int)AIPStatusD.Edit;
                var stateName = User.IsInRole(AIPRoles.Coordinator) ? "На рассмотрении" : "На редактировании";
                gp.Toolbar()
                    .AddIconButton(
                        "{0}NewRecordBtn".FormatWith(gp.ID),
                        Icon.Add,
                        "Добавить новую запись",
                        Scope + ".addRecordInGrid('{0}', {1}, '{2}', {3}, '{4}')".FormatWith(gp.ID, objectId, curObject.Name, stateId, stateName));

                gp.AddRemoveRecordButton().Listeners.Click.Handler =
                    ScopeCommon + ".deleteRecordInGrid('{0}', '{1}')".FormatWith(gp.ID, gp.ID);

                StatusDControl.AddStatusDButtons(gp, store, colState, page, User, true);
            }

            var gs = new GroupingSummary { Visible = true };
            gp.Plugins.Add(gs);

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }

        private void CreateColumns(ViewPage page, GridPanel gp)
        {
            var columnTypeWork = gp.ColumnModel.AddColumn("TypeWorkName", "Вид работ", DataAttributeTypes.dtString).SetWidth(100);
            //// Нужен только для группировки и подсчета итогов.
            gp.ColumnModel.AddColumn("CObjectName", "Объект строительства", DataAttributeTypes.dtString)
                .SetGroupable(true)
                .SetHidden(true);
            var colSumGroup = AddColumnSum(gp, "StartPrice", "Начальная (максимальная) цена контракта по согласованию РСТ");
            var colSumPrice = AddColumnSum(gp, "Price", "Стоимость в текущих ценах по контракту");
            var columnContract = gp.ColumnModel.AddColumn("ContractName", "Реквизиты контракта, протокола торгов", DataAttributeTypes.dtString).SetWidth(120);
            var columnPartner = gp.ColumnModel.AddColumn("PartnerName", "Наименование исполнителя работ (подрядчика)", DataAttributeTypes.dtString).SetWidth(120);
            var columnDate = gp.ColumnModel.AddColumn("Date", "Срок выполнения работ по контракту", DataAttributeTypes.dtInteger).SetWidth(120);
            var columnState = gp.ColumnModel.AddColumn("StateName", "Статус контракта", DataAttributeTypes.dtString).SetWidth(120);
            var columnExpectPrice = gp.ColumnModel.AddColumn("ExpectPrice", "Ожидаемая стоимость основных фондов", DataAttributeTypes.dtString).SetWidth(120);
            
            if (canEdit)
            {
                columnContract.SetEditableString();
                columnPartner.SetEditableString();
                colSumGroup.SetEditableDouble(2);
                colSumPrice.SetEditableDouble(2);
                columnExpectPrice.SetEditableDouble(2);

                var config = new PeriodFieldConfig
                                 {
                                     ShowYear = true,
                                     ShowQuarter = true,
                                     ShowMonth = true,
                                     ShowDay = true,
                                     YearSelectable = true,
                                     QuarterSelectable = true,
                                     MonthSelectable = true,
                                     DaySelectable = true,
                                     MinDate = curObject.StartConstruction,
                                     MaxDate = curObject.EndConstruction
                                 };

                columnDate.SetEditablePeriod(config);

                var storeId = "{0}Store{1}".FormatWith(gp.ID, columnState.ColumnID);
                var stateStore = CreateBookStore(storeId, "/Entity/DataWithCustomSearch?objectKey=2e8029b3-43a8-4644-ae61-c1786763239f&start=0&limit=999&serverFilter=(ID>0)");
                page.Controls.Add(stateStore);

                columnState.Editor.Add(
                    new ComboBox
                        {
                            TriggerAction = TriggerAction.All,
                            Store = { stateStore },
                            AutoShow = true,
                            MaxHeight = 60,
                            Editable = false,
                            AllowBlank = false,
                            ValueField = "NAME",
                            DisplayField = "NAME",
                            Listeners =
                                {
                                    Select =
                                        {
                                            Handler = Scope + @".selectNewValue({0}, record, 'StateId', 'StateName');".FormatWith(gp.ID)
                                        }
                                }
                        });

                var storeWorkTypeId = "{0}Store{1}".FormatWith(gp.ID, columnTypeWork.ColumnID);
                var stateWorkTypeStore = CreateBookStore(
                    storeWorkTypeId,
                    "/Entity/DataWithCustomSearch?objectKey=247aea04-5069-44b7-8270-8512ce18ae02&start=0&limit=999&serverFilter=(ID>0)");
                page.Controls.Add(stateWorkTypeStore);

                columnTypeWork.Editor.Add(
                    new ComboBox
                    {
                        TriggerAction = TriggerAction.All,
                        Store = { stateWorkTypeStore }, 
                        AutoShow = true,
                        MaxHeight = 60,
                        Editable = false,
                        AllowBlank = false,
                        ValueField = "NAME",
                        DisplayField = "NAME",
                        Width = 270,
                        AutoWidth = true,
                        Listeners =
                        {
                            Select =
                            {
                                Handler = Scope + @".selectNewValue({0}, record, 'TypeWorkId', 'TypeWorkName');".FormatWith(gp.ID)
                            }
                        }
                    });
            }
        }

        private Store CreateBookStore(string id, string url)
        {
            var ds = new Store { ID = id, AutoLoad = false };
            ds.SetHttpProxy(url).SetJsonReader();
            ds.BaseParams.Add(new Parameter("fields", "['ID','NAME']", ParameterMode.Raw));
            ds.AddField("ID"); 
            ds.AddField("NAME");
            
            return ds;
        }
    }
}
