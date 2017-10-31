using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.ServiceRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Models.ServisesModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public sealed class RegisterOfServices : View
    {
        /// <summary>
        ///  Реестр услуг
        /// </summary>
        #region Setting

        private const string Scope = "E86n.View.Services";
        private const string ServicesViewID = "Services";
        private const string ServicesStoreID = "ServicesStore";
        private const string ConsumerViewID = "Consumer";
        private const string ConsumerStoreID = "ConsumerStore";
        private const string CharacteristicViewID = "Characteristic";
        private const string CharacteristicStoreID = "CharacteristicStore";
        private const string ProviderID = "Provider";
        private const string ProviderStoreID = "ProviderStore";

        private readonly ServiceRegisterCharacteristicViewModel characteristicModel = new ServiceRegisterCharacteristicViewModel();
        private readonly ServiceRegisterConsumerViewModel consumerModel = new ServiceRegisterConsumerViewModel();
        private readonly ServiceRegisterProviderViewModel providerModel = new ServiceRegisterProviderViewModel();
        private readonly ServicesViewModel servicesModel = new ServicesViewModel();

        #endregion

        private readonly IAuthService auth;

        public RegisterOfServices(IAuthService auth)
        {
            this.auth = auth;
        }

        public ViewPage Page { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            var restActions = ResourceManager.GetInstance(Page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("ServicesView", Resource.ServicesView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            var view = new Viewport
                {
                    Items =
                        {
                            new BorderLayout
                                {
                                    Center = { Items = { GetMasterGrid() } }, 
                                    South = { Items = { GetDetailPanel() }, Split = true }
                                }
                        }
                };

            return new List<Component> { view };
        }

        #region MasterGrid

        private Store GetMasterStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                ServicesStoreID, 
                true, 
                typeof(ServicesController), 
                createActionName: "Save", 
                updateActionName: "Save");

            store.AddFieldsByClass(servicesModel);
            store.AddField(servicesModel.NameOf(() => servicesModel.BusinessStatus), new RecordField.Config { DefaultValue = "false" });
            store.BaseParams.Add(new Parameter("limit", "pageSizeServices.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));

            Page.Controls.Add(store);
            return store;
        }

        private IEnumerable<Component> GetMasterGrid()
        {
            var gp = UiBuilders.CreateGridPanel(ServicesViewID, GetMasterStore());

            gp.BottomBar.Add(GetMasterGridBottomToolBar(gp.StoreID));
            gp.Plugins.Add(GetMasterGridFilters());
            gp.Toolbar().Add(GetMasterGridToolbar());
            GetMasterGridColumnModel(gp.ColumnModel);

            var handler = string.Concat(Scope, ".UpdateView();");
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = handler;
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.Handler = handler;

            gp.AddRefreshButton();

            if (!auth.IsSpectator())
            {
                gp.AddNewRecordNoEditButton();
                gp.AddRemoveRecordWithConfirmButton();
                gp.AddSaveButton();

                gp.Toolbar().AddIconButton(
                    "CopyBtn",
                    Icon.PageCopy,
                    "Скопировать работу/услугу",
                    string.Concat(Scope, ".Copy();"));
            }

            gp.AddColumnsWrapStylesToPage(Page);
            return new List<Component> { gp };
        }

        private void GetMasterGridColumnModel(ColumnModel columnModel)
        {
            columnModel.AddColumn(() => servicesModel.BusinessStatus, DataAttributeTypes.dtBoolean)
                .SetEditableBoolean()
                .SetWidth(16);
            columnModel.AddColumn(() => servicesModel.RefTipYName, DataAttributeTypes.dtString)
                .SetWidth(80)
                .SetComboBoxEditor(D_Services_TipY.Key, Page, servicesModel.NameOf(() => servicesModel.RefTipY), null, null, false);
            columnModel.AddColumn(() => servicesModel.Code, DataAttributeTypes.dtInteger)
                .SetWidth(80)
                .Editor.Add(new TextField { AllowBlank = false, MaxLength = 20, MaskRe = @"[0-9]" });
            columnModel.AddColumn(() => servicesModel.Name, DataAttributeTypes.dtString)
                .SetWidth(250)
                .SetEditableString();
            columnModel.AddColumn(() => servicesModel.RefPlName, DataAttributeTypes.dtString)
                .SetWidth(80)
                .SetComboBoxEditor(D_Services_Platnost.Key, Page, servicesModel.NameOf(() => servicesModel.RefPl), null, null, false);

            var mapping = new Dictionary<string, string[]>
                {
                    { servicesModel.NameOf(() => servicesModel.RefGRBSs), new[] { "ID" } },
                    { servicesModel.NameOf(() => servicesModel.RefGRBSsName), new[] { "Code", "Name" } }
                };
            columnModel.AddColumn(() => servicesModel.RefGRBSsName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetEditable(auth.IsAdmin() || auth.IsPpoUser())
                .SetHidden(!(auth.IsAdmin() || auth.IsPpoUser()))
                .SetComboBoxEditor(D_Org_GRBS.Key, Page, mapping);
            columnModel.AddColumn(() => servicesModel.RefOrgPPOName, DataAttributeTypes.dtString)
                .SetWidth(100)
                .SetEditable(auth.IsAdmin())
                .SetHidden(!auth.IsAdmin())
                .SetComboBoxEditor(D_Org_PPO.Key, Page, servicesModel.NameOf(() => servicesModel.RefOrgPPO));
            columnModel.AddColumn(() => servicesModel.RefSferaDName, DataAttributeTypes.dtString)
                .SetWidth(100)
                .SetComboBoxEditor(D_Services_SferaD.Key, Page, servicesModel.NameOf(() => servicesModel.RefSferaD));
            columnModel.AddColumn(() => servicesModel.DataVkluch, DataAttributeTypes.dtDate)
               .SetWidth(100);
            columnModel.AddColumn(() => servicesModel.DataIskluch, DataAttributeTypes.dtDate)
               .SetWidth(100);
        }

        private IEnumerable<Component> GetMasterGridToolbar()
        {
            var t = new List<Component>();

            if (!auth.IsSpectator())
            {
                t.Add(
                    new UpLoadFileBtnControl
                        {
                            UploadController = UiBuilders.GetUrl<ImportsController>("ImportNsiSubjectService")
                        }

                        .Build(Page)[0]);
            }

            if (auth.IsKristaRu())
            {
                t.Add(
                    new UpLoadFileBtnControl
                        {
                            Id = "btnUploadYarServices",
                            Name = "Импорт услуги формат-Ярославль",
                            UploadController = UiBuilders.GetUrl<ImportsController>("ImportYaroslavlSubjectService")
                        }

                        .Build(Page)[0]);
            }

            return t;
        }

        private Plugin GetMasterGridFilters()
        {
            var gridFilters = new GridFilters { Local = false };

            gridFilters.Filters.Add(new ListFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefPlName), Options = new[] { "Бесплатная", "Платная", "Частично платная" } });
            gridFilters.Filters.Add(new BooleanFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefTipYName), YesText = @"Услуга", NoText = @"Работа" });
            gridFilters.Filters.Add(new BooleanFilter { DataIndex = servicesModel.NameOf(() => servicesModel.BusinessStatus), YesText = @"Включена", NoText = @"Исключена", Value = true });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.Name), EmptyText = "Полное или сокращенное название, ИНН, КПП" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefOrgPPOName), EmptyText = "Название ППО" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefGRBSsName), EmptyText = "Название ГРБС" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.RefSferaDName), EmptyText = "Название сферы деятельности" });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesModel.NameOf(() => servicesModel.Code), EmptyText = "Код" });
            gridFilters.Filters.Add(new DateFilter { DataIndex = servicesModel.NameOf(() => servicesModel.DataVkluch) });
            gridFilters.Filters.Add(new DateFilter { DataIndex = servicesModel.NameOf(() => servicesModel.DataIskluch) });

            return gridFilters;
        }

        private ToolbarBase GetMasterGridBottomToolBar(string storeID)
        {
            var toolbar = new PagingToolbar
                {
                    ID = "paggingServices", 
                    StoreID = storeID, 
                    PageSize = 100, 
                };

            toolbar.Items.Add(
                new NumberField
                    {
                        ID = "pageSizeServices", 
                        FieldLabel = @"Услуг на страницу", 
                        LabelWidth = 150, 
                        Width = 200, 
                        Number = 100, 
                        Listeners = { Change = { Handler = "#{paggingServices}.pageSize = parseInt(this.getValue());" } }
                    });

            toolbar.Listeners.Change.Handler = string.Concat(Scope, ".ChangePage();");

            return toolbar;
        }

        #endregion

        #region DetailPanel

        private IEnumerable<Component> GetDetailPanel()
        {
            var tabPanel = UiBuilders.GetTabbedDetails(new List<Component> { GetDetailConsumerGrid(), GetDetailCharacteristicGrid(), GetDetailProviderGrid() }) as TabPanel;
            if (tabPanel != null)
            {
                tabPanel.ID = "DetailTabPanel";
                tabPanel.Disabled = true;
                tabPanel.Height = 300;
                tabPanel.Collapsible = true;
                tabPanel.Listeners.TabChange.Handler = string.Concat(Scope, ".UpdateView();"); 
                return new List<Component> { tabPanel };
            }

            return null;
        }

        private Store GetDetailConsumerStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                ConsumerStoreID, 
                false, 
                typeof(ServicesController),
                string.Concat(ConsumerViewID, "Read"),
                string.Concat(ConsumerViewID, "Save"),
                string.Concat(ConsumerViewID, "Save"),
                string.Concat(ConsumerViewID, "Delete"));

            store.AddFieldsByClass(consumerModel);

            store.SetBaseParams("masterId", string.Concat(Scope, ".getSelectedServiceId()"), ParameterMode.Raw);
            store.SetWriteBaseParams("MasterId", string.Concat(Scope, ".getSelectedServiceId()"), ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Store GetDetailProviderStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                ProviderStoreID,
                false,
                typeof(ServicesController),
                string.Concat(ProviderID, "Read"),
                string.Concat(ProviderID, "Save"),
                string.Concat(ProviderID, "Save"),
                string.Concat(ProviderID, "Delete"));

            store.AddFieldsByClass(providerModel);

            store.SetBaseParams("masterId", string.Concat(Scope, ".getSelectedServiceId()"), ParameterMode.Raw);
            store.SetWriteBaseParams("MasterId",  string.Concat(Scope, ".getSelectedServiceId()"), ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private Store GetDetailCharacteristicStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                CharacteristicStoreID, 
                false,
                typeof(ServicesController),
                string.Concat(CharacteristicViewID, "Read"),
                string.Concat(CharacteristicViewID, "Save"),
                string.Concat(CharacteristicViewID, "Save"),
                string.Concat(CharacteristicViewID, "Delete"));

            store.AddFieldsByClass(characteristicModel);
            store.SetBaseParams("masterId", string.Concat(Scope, ".getSelectedServiceId()"), ParameterMode.Raw);
            store.SetWriteBaseParams("MasterId", string.Concat(Scope, ".getSelectedServiceId()"), ParameterMode.Raw);

            Page.Controls.Add(store);
            return store;
        }

        private GridPanel GetDetailCharacteristicGrid()
        {
            var table = GetDetailBaseGrid(CharacteristicViewID, GetDetailCharacteristicStore());
            table.Title = @"Показатели объема и качества услуг/работ";

            var mapping = new Dictionary<string, string[]>
                {
                    { characteristicModel.NameOf(() => characteristicModel.RefIndicators), new[] { "ID" } },
                    { characteristicModel.NameOf(() => characteristicModel.RefOKEIName), new[] { "RefOKEI" } },
                    { characteristicModel.NameOf(() => characteristicModel.RefTypeName), new[] { "RefCharacteristicType" } },
                    { characteristicModel.NameOf(() => characteristicModel.RefIndicatorsName), new[] { "Name", "RefOKEI", "RefCharacteristicType" } }
                };

            table.ColumnModel.AddColumn(() => characteristicModel.RefIndicatorsName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(D_Services_Indicators.Key, Page, mapping, UiBuilders.GetUrl<ServicesController>("GetIndicators"));

            table.ColumnModel.AddColumn(() => characteristicModel.RefOKEIName, DataAttributeTypes.dtString)
                .SetWidth(200);
            table.ColumnModel.AddColumn(() => characteristicModel.RefTypeName, DataAttributeTypes.dtString)
                .SetWidth(200);

            return table;
        }

        private GridPanel GetDetailProviderGrid()
        {
            var table = GetDetailBaseGrid(ProviderID, GetDetailProviderStore());
            table.Title = @"Поставщики услуг";

            var mapping = new Dictionary<string, string[]>
                {
                    { providerModel.NameOf(() => providerModel.RefProvider), new[] { "ID" } },
                    { providerModel.NameOf(() => providerModel.RefProviderName), new[] { "Name", "ShortName", "KPP", "INN" } },
                };

            table.ColumnModel.AddColumn(() => providerModel.RefProviderName, DataAttributeTypes.dtString)
                .SetWidth(500)
                .SetComboBoxEditor(D_Org_Structure.Key, Page, mapping, UiBuilders.GetUrl<ServicesController>("GetProvider"));
            return table;
        }

        private GridPanel GetDetailConsumerGrid()
        {
            var table = GetDetailBaseGrid(ConsumerViewID, GetDetailConsumerStore());
            table.Title = @"Категории потребителей";

            table.ColumnModel.AddColumn(() => consumerModel.RefConsumerName, DataAttributeTypes.dtString)
                .SetWidth(500)
                .SetComboBoxEditor(D_Services_CPotr.Key, Page, consumerModel.NameOf(() => consumerModel.RefConsumer));
            return table;
        }

        private GridPanel GetDetailBaseGrid(string gridID, Store store)
        {
            var table = UiBuilders.CreateGridPanel(gridID, store);

            table.AddRefreshButton();
            if (!auth.IsSpectator())
            {
                table.AddNewRecordNoEditButton();
                table.AddDeleteRecordWithConfirmButton();
                table.AddSaveButton();
            }

            return table;
        }

        #endregion
    }
}
