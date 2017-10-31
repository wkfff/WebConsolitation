using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTask2016;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.StateTasks;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    /// <summary>
    /// Результаты деятельности
    /// </summary>
    public sealed class ResultsOfActivityView : View
    {
        #region Setting
        private const string MembersOfStaffFormID = "MembersOfStaff";
        private const string FinNFinAssetsID = "FinNFinAssets";
        private const string CashReceiptsID = "CashReceipts";
        private const string CashPaymentsID = "CashPayments";
        private const string ServicesWorksID = "ServicesWorks";
        private const string ServicesWorks2016ID = "ServicesWorks2016";
        private const string PropertyUseID = "PropertyUse";
        private const string Scope = "E86n.View.ResultsOfActivityView.";
        private readonly CashPaymentsViewModel cashPaymentsModel = new CashPaymentsViewModel();
        private readonly CashReceiptsViewModel cashReceiptsModel = new CashReceiptsViewModel();
        private readonly FinNFinAssetsViewModel finNFinAssetsModel = new FinNFinAssetsViewModel();
        private readonly MembersOfStaffViewModel membersOfStaffModel = new MembersOfStaffViewModel();
        private readonly PropertyUseViewModel propertyUseModel = new PropertyUseViewModel();
        private readonly ServicesWorksViewModel servicesWorksModel = new ServicesWorksViewModel();
        private readonly ServicesWorks2016ViewModel servicesWorks2016Model = new ServicesWorks2016ViewModel();
        private readonly string viewController = UiBuilders.GetControllerID<ResultsOfActivityViewController>();
        #endregion

        private readonly IAuthService auth;
        private readonly ICommonDataService commonDataService;
        private int yearForm;
        private int type;
        private int docId;
        private string docIdStr;
       
        public ResultsOfActivityView(IAuthService auth, ICommonDataService commonDataService)
        {
            this.auth = auth;
            this.commonDataService = commonDataService;
        }

        private ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;
            docId = Params["docId"] == "null" ? -1 : Convert.ToInt32(Params["docId"]);
            docIdStr = docId.ToString(CultureInfo.InvariantCulture);
            yearForm = commonDataService.GetItem<F_F_ParameterDoc>(docId).RefYearForm.ID;
            type = commonDataService.GetTypeOfInstitution(docId).ID;

            var instance = ResourceManager.GetInstance(page);
            instance.RegisterClientScriptBlock("CodeMaskBuilder", Resource.CodeMaskBuilder);
            instance.RegisterClientScriptBlock("ResultsOfActivityView", Resource.ResultsOfActivityView);
            instance.RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            var restActions = instance.RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            var tb = new NewStateToolBarControl(Convert.ToInt32(docIdStr)).BuildComponent(page);
            tb.Add(new VersioningControl(Convert.ToInt32(docIdStr), UiBuilders.GetControllerID<ResultsOfActivityViewController>(), Scope + "Common.SetReadOnlyResultsOfActivity").Build(page));

            if (auth.IsAdmin())
            {
                var export = new UpLoadFileBtnControl
                {
                    Id = "btnExport",
                    Name = "Экспорт в XML",
                    Icon = Icon.DiskDownload,
                    Upload = false,
                    UploadController = UiBuilders.GetUrl<ResultsOfActivityViewController>("ExportToXml"),
                    Params = { { "recId", docIdStr } }
                };

                tb.Add(export.Build(Page));

                tb.Add(new ToolbarSeparator());

                tb.Add(new SetDocStateBtn(Convert.ToInt32(docIdStr)).Build(Page));
            }

            var paramDocPanel = new ParamDocPanelControl(docId, tb).BuildComponent(page);
            var formPanel = paramDocPanel as FormPanel;
            if (formPanel != null)
            {
                formPanel.Items.Find(x => x.ID == "RefYearFormID").FieldLabel = @"Отчетный год";
                formPanel.Items.Find(x => x.ID == "PlanThreeYear_Name").Visible = false;
            }

            var cash = new List<Component> { GetCashReceipts() };
            if (!(yearForm > 2015 && type.Equals(FX_Org_TipYch.GovernmentID)))
            {
                cash.Add(GetCashPayments());
            }

            var detailCash = (TabPanel)UiBuilders.GetTabbedDetails(cash);
            detailCash.Title = @"Кассовые поступления/выплаты";
            detailCash.ID = "Cash";

            var details = (TabPanel)UiBuilders.GetTabbedDetails(
                new List<Component>
                    {
                        GetMembersOfStaff(), 
                        GetFinNFinAssets(), 
                        yearForm < 2016
                            ? GetServicesWorks()
                            : GetServicesWorks2016(),
                        detailCash, 
                        GetPropertyUse(),
                        new DocsDetailControl(docId).BuildComponent(page)
                    });
            details.ID = "DetailTabPanel";
            details.Listeners.TabChange.Fn(string.Concat(Scope, "Common"), "reloadDetail");

            var view = new Viewport
                {
                    Items =
                        {
                            new BorderLayout
                                {
                                    North = { Items = { paramDocPanel } },
                                    Center = { Items = { details } }
                                }
                        }
                };

            return new List<Component> { view };
        }

        #region Служебные функции

        private Store GetStoreByModel(ViewModelBase model, string id)
        {
            var store = StoreExtensions.StoreCreateDefault(
                string.Concat(id, "Store"), 
                false, 
                viewController, 
                string.Concat(id, "Load"));
            store.SetBaseParams("recId", docIdStr, ParameterMode.Raw);
            store.Listeners.DataChanged.Fn(string.Concat(Scope, id), "StoreDataChanged");
            store.AddFieldsByClass(model);
            Page.Controls.Add(store);
            return store;
        }

        private FormPanel GetDefaltPanel()
        {
            return new FormPanel
                {
                    BaseParams = { new Parameter("recId", docIdStr) },
                    Border = false, 
                    AutoScroll = true, 
                    LabelWidth = 300, 
                    Padding = 6, 
                    MonitorValid = true
                };
        }

        private Toolbar GetToolbar(string id)
        {
            var tb = new Toolbar();
            tb.AddIconButton(
                string.Concat("btn", id, "Refresh"),
                Icon.PageRefresh, 
                "Обновить",
                string.Concat(Scope, id, ".Refresh();"));

            tb.AddIconButton(
                string.Concat("btn", id, "Save"),
                Icon.Disk, 
                "Сохранить изменения", 
                string.Concat(Scope, id, ".Save();"));
            return tb;
        }

        private NumberField GetNumberField<T>(Expression<Func<T>> expr)
        {
            var name = UiBuilders.NameOf(expr);
            return new NumberField
                {
                    ID = name, 
                    DataIndex = name, 
                    AllowBlank = false, 
                    AllowNegative = false, 
                    FieldLabel = UiBuilders.DescriptionOf(expr), 
                    DecimalPrecision = 2, 
                    DecimalSeparator = ","
                };
        }

        private NumberField GetNumberFieldNegative<T>(Expression<Func<T>> expr)
        {
            var name = UiBuilders.NameOf(expr);
            return new NumberField
            {
                ID = name,
                DataIndex = name,
                AllowBlank = false,
                FieldLabel = UiBuilders.DescriptionOf(expr),
                DecimalPrecision = 2,
                DecimalSeparator = ","
            };
        }

        private NumberField GetNumberFieldFlex1<T>(Expression<Func<T>> expr)
        {
            var id = UiBuilders.NameOf(expr);
            return new NumberField
                {
                    ID = id, 
                    DataIndex = id, 
                    AllowNegative = false, 
                    DecimalSeparator = ",", 
                    Flex = 1
                };
        }

        private NumberField GetNumberFieldFlex2<T>(Expression<Func<T>> expr)
        {
            var id = UiBuilders.NameOf(expr);
            return new NumberField
                {
                    ID = id,
                    DataIndex = id,
                    AllowBlank = false,
                    DecimalPrecision = 2,
                    DecimalSeparator = ",",
                    Flex = 1
                };
        }

        private Hidden GetHidden(string field)
        {
            return new Hidden { ID = field, DataIndex = field };
        }

        private CompositeField GetCompositeField<T>(Expression<Func<T>> expr)
        {
            return new CompositeField { FieldLabel = UiBuilders.DescriptionOf(expr), AnchorHorizontal = "100%" };
        }

        #endregion

        private FormPanel GetMembersOfStaff()
        {
            GetStoreByModel(membersOfStaffModel, MembersOfStaffFormID);

            var form = GetDefaltPanel();
            form.ID = MembersOfStaffFormID;
            form.Title = @"Штат сотрудников";
            form.Url = UiBuilders.GetUrl<ResultsOfActivityViewController>("MembersOfStaffSave");
            form.Listeners.ClientValidation.Handler = string.Concat(Scope, MembersOfStaffFormID, ".ClientValidation(valid);");

            form.TopBar.Add(GetToolbar(MembersOfStaffFormID));

            var hidden = GetHidden(membersOfStaffModel.NameOf(() => membersOfStaffModel.MembersOfStaffID));
            hidden.FieldLabel = membersOfStaffModel.DescriptionOf(() => membersOfStaffModel.MembersOfStaffID);
            form.Items.Add(hidden);

            var field = GetNumberField(() => membersOfStaffModel.BeginYear);
            field.MaxLength = 20;
            form.Items.Add(field);

            field = GetNumberField(() => membersOfStaffModel.EndYear);
            field.MaxLength = 20;
            form.Items.Add(field);

            form.Items.Add(GetNumberField(() => membersOfStaffModel.AvgSalary));

            return form;
        }

        private FormPanel GetFinNFinAssets()
        {
            GetStoreByModel(finNFinAssetsModel, FinNFinAssetsID).Listeners.Load.Fn(string.Concat(Scope, FinNFinAssetsID), "StoreLoad");

            var loadController = UiBuilders.GetUrl<ResultsOfActivityViewController>("GetTypeOfChange");
            var form = GetDefaltPanel();
            form.LabelWidth = 500;
            form.ID = FinNFinAssetsID;
            form.Title = @"Финансовые/Нефинансовые активы";
            form.Url = UiBuilders.GetUrl<ResultsOfActivityViewController>("FinNFinAssetsSave");
            form.Listeners.ClientValidation.Handler = string.Concat(Scope, FinNFinAssetsID, ".ClientValidation(valid);"); 

            form.TopBar.Add(GetToolbar(FinNFinAssetsID));

            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.AmountOfDamageCompensationID)));

            var copmositFields = GetCompositeField(() => finNFinAssetsModel.AmountOfDamageCompensation);
            var field = GetNumberFieldFlex1(() => finNFinAssetsModel.AmountOfDamageCompensation);
            field.MaxLength = 20;
            copmositFields.Items.Add(field);
            form.Items.Add(copmositFields);

            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotalID)));

            copmositFields = GetCompositeField(() => finNFinAssetsModel.InfAboutCarryingValueTotal);
            var combo = new DBComboBox
                {
                    ID = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotalRefTypeIxm),
                    LoadController = loadController,
                    DataIndex = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.InfAboutCarryingValueTotalRefTypeIxmName)
                };
            var nameSpace = string.Concat(Scope, FinNFinAssetsID);
            combo.Box.Listeners.Select.Fn(nameSpace, "SetRefTypeIxmValue");
            copmositFields.Items.Add(combo.Build(Page));
            copmositFields.Items.Add(GetNumberFieldFlex1(() => finNFinAssetsModel.InfAboutCarryingValueTotal));
            var group = new FieldSet
                {
                    Title = @"Сведения об изменении балансовой стоимости нефинансовых активов"
                };
            group.Items.Add(copmositFields);
            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovablePropertyID)));

            copmositFields = GetCompositeField(() => finNFinAssetsModel.ImmovableProperty);
            combo = new DBComboBox
                {
                    ID = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovablePropertyRefTypeIxm),
                    LoadController = loadController,
                    DataIndex = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ImmovablePropertyRefTypeIxmName)
                };
            combo.Box.Listeners.Select.Fn(nameSpace, "SetRefTypeIxmValue");
            copmositFields.Items.Add(combo.Build(Page));
            copmositFields.Items.Add(GetNumberFieldFlex1(() => finNFinAssetsModel.ImmovableProperty));
            group.Items.Add(copmositFields);
            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuablePropertyID)));

            copmositFields = GetCompositeField(() => finNFinAssetsModel.ParticularlyValuableProperty);
            combo = new DBComboBox
                {
                    ID = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuablePropertyRefTypeIxm),
                    LoadController = loadController,
                    DataIndex = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ParticularlyValuablePropertyRefTypeIxmName)
                };
            combo.Box.Listeners.Select.Fn(nameSpace, "SetRefTypeIxmValue");
            copmositFields.Items.Add(combo.Build(Page));
            copmositFields.Items.Add(GetNumberFieldFlex1(() => finNFinAssetsModel.ParticularlyValuableProperty));
            group.Items.Add(copmositFields);
            form.Items.Add(group);

            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsTotalID)));

            copmositFields = GetCompositeField(() => finNFinAssetsModel.ChangingArrearsTotal);
            combo = new DBComboBox
                {
                    ID = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsRefTypeIxm),
                    LoadController = loadController,
                    DataIndex = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ChangingArrearsRefTypeIxmName)
                };
            combo.Box.Listeners.Select.Fn(nameSpace, "SetRefTypeIxmValue");
            copmositFields.Items.Add(combo.Build(Page));
            copmositFields.Items.Add(GetNumberFieldFlex1(() => finNFinAssetsModel.ChangingArrearsTotal));
            group = new FieldSet
                {
                    Title = @"Изменение дебиторской и кредиторской задолжности за отчетный год"
                };

            group.Items.Add(copmositFields);
            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncomeID)));

            copmositFields = GetCompositeField(() => finNFinAssetsModel.Income);
            combo = new DBComboBox
                {
                    ID = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncomeRefTypeIxm),
                    LoadController = loadController,
                    DataIndex = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncomeRefTypeIxmName)
                };
            combo.Box.Listeners.Select.Fn(nameSpace, "SetRefTypeIxmValue");
            copmositFields.Items.Add(combo.Build(Page));
            copmositFields.Items.Add(GetNumberFieldFlex1(() => finNFinAssetsModel.Income));
            group.Items.Add(copmositFields);
            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ExpenditureID)));

            copmositFields = GetCompositeField(() => finNFinAssetsModel.Expenditure);
            combo = new DBComboBox
                {
                    ID = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ExpenditureRefTypeIxm),
                    LoadController = loadController,
                    DataIndex = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.ExpenditureRefTypeIxmName)
                };
            combo.Box.Listeners.Select.Fn(nameSpace, "SetRefTypeIxmValue");
            copmositFields.Items.Add(combo.Build(Page));
            copmositFields.Items.Add(GetNumberFieldFlex1(() => finNFinAssetsModel.Expenditure));
            group.Items.Add(copmositFields);
            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotalID)));

            copmositFields = GetCompositeField(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal);
            combo = new DBComboBox
                {
                    ID = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotalRefTypeIxm),
                    LoadController = loadController,
                    DataIndex = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.IncreaseInAccountsPayableTotalRefTypeIxmName)
                };
            combo.Box.Listeners.Select.Fn(nameSpace, "SetRefTypeIxmValue");
            copmositFields.Items.Add(combo.Build(Page));
            copmositFields.Items.Add(GetNumberFieldFlex1(() => finNFinAssetsModel.IncreaseInAccountsPayableTotal));
            group.Items.Add(copmositFields);
            form.Items.Add(GetHidden(finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayablesID)));

            copmositFields = GetCompositeField(() => finNFinAssetsModel.OverduePayables);
            combo = new DBComboBox
                {
                    ID = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayablesRefTypeIxm),
                    LoadController = loadController,
                    DataIndex = finNFinAssetsModel.NameOf(() => finNFinAssetsModel.OverduePayablesRefTypeIxmName)
                };
            combo.Box.Listeners.Select.Fn(nameSpace, "SetRefTypeIxmValue");
            copmositFields.Items.Add(combo.Build(Page));
            copmositFields.Items.Add(GetNumberFieldFlex1(() => finNFinAssetsModel.OverduePayables));
            group.Items.Add(copmositFields);
            form.Items.Add(group);

            return form;
        }

        private FormPanel GetCashReceipts()
        {
            GetStoreByModel(cashReceiptsModel, CashReceiptsID);

            var form = GetDefaltPanel();
            form.ID = CashReceiptsID;
            form.LabelWidth = 450;
            form.Title = @"Кассовые поступления";
            form.Url = UiBuilders.GetUrl<ResultsOfActivityViewController>("CashReceiptsSave");
            form.Listeners.ClientValidation.Handler = string.Concat(Scope, CashReceiptsID, ".ClientValidation(valid);");

            form.TopBar.Add(GetToolbar(CashReceiptsID));

            var hidden = GetHidden(cashReceiptsModel.NameOf(() => cashReceiptsModel.CashReceiptsID));
            hidden.FieldLabel = cashReceiptsModel.DescriptionOf(() => cashReceiptsModel.CashReceiptsID);
            form.Items.Add(hidden);

            form.Items.Add(GetNumberFieldNegative(() => cashReceiptsModel.Total));
            form.Items.Add(GetNumberFieldNegative(() => cashReceiptsModel.TaskGrant));
            form.Items.Add(GetNumberFieldNegative(() => cashReceiptsModel.ActionGrant));
            form.Items.Add(GetNumberFieldNegative(() => cashReceiptsModel.BudgetFunds));
            form.Items.Add(GetNumberFieldNegative(() => cashReceiptsModel.PaidServices));

            return form;
        }

        private GridPanel GetCashPayments()
        {
            Store store = StoreExtensions.StoreUrlCreateDefault(
                CashPaymentsID,
                false,
                Page,
                cashPaymentsModel,
                new Dictionary<string, string>
                    {
                        { servicesWorksModel.NameOf(() => cashPaymentsModel.RefParent), docIdStr }
                    });

            store.SetBaseParams("docId", docIdStr, ParameterMode.Raw);
            store.SetWriteBaseParams("docId", docIdStr, ParameterMode.Raw);

            var gp = UiBuilders.CreateGridPanel(CashPaymentsID, store);
            gp.Title = @"Кассовые выплаты";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordWithConfirmButton();
            gp.AddSaveButton();

            var handler = string.Concat(Scope, CashPaymentsID, ".Update()");
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = handler;
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.Handler = handler;
            
            var dictionary = new Dictionary<string, string[]>
                {
                    { cashPaymentsModel.NameOf(() => cashPaymentsModel.RefRazdPodr), new[] { "ID" } },
                    { cashPaymentsModel.NameOf(() => cashPaymentsModel.RefRazdPodrName), new[] { "Code", "Name" } }
                };

            if (type.Equals(FX_Org_TipYch.GovernmentID) || (type.Equals(FX_Org_TipYch.BudgetaryID) && yearForm > 2015))
            {
                gp.ColumnModel.AddColumn(() => cashPaymentsModel.RefRazdPodrName, DataAttributeTypes.dtString)
                    .SetWidth(200)
                    .SetComboBoxEditor(D_Fin_RazdPodr.Key, Page, dictionary, null, null, "Name");

                var castom = gp.ColumnModel.AddColumn(() => cashPaymentsModel.CelStatya, DataAttributeTypes.dtString)
                    .SetWidth(100);
                castom.Editor.Add(new TextField { AllowBlank = false, MaxLength = yearForm < 2016 ? 7 : 10, MaskRe = @"[0-9a-zA-Zа-яА-Я]" });
                castom.Renderer.Handler = "return buildMask(value, '{0}');".FormatWith(
                    yearForm < 2016
                    ? "###.##.##"
                    : "##.#.##.#####");

                gp.ColumnModel.AddColumn(() => cashPaymentsModel.RefVidRashName, DataAttributeTypes.dtString)
                    .SetWidth(200)
                    .SetHbLookup(D_Fin_VidRash.Key, "{0}.getVidRashFilter('{1}')".FormatWith(Scope + CashPaymentsID, docId));
            }

            dictionary = new Dictionary<string, string[]>
            {
                { cashPaymentsModel.NameOf(() => cashPaymentsModel.RefKosgy), new[] { "ID" } },
                { cashPaymentsModel.NameOf(() => cashPaymentsModel.RefKosgyName), new[] { "Code", "Name" } }
            };
           
            gp.ColumnModel.AddColumn(() => cashPaymentsModel.RefKosgyName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(D_KOSGY_KOSGY.Key, Page, dictionary, UiBuilders.GetUrl<ResultsOfActivityViewController>("GetKOSGY"))
                .SetCustomEditorFunction(string.Concat(Scope, CashPaymentsID, ".SelectRowGrid(item,record,'", CashPaymentsID, "')"));

            gp.ColumnModel.AddColumn(() => cashPaymentsModel.Payment, DataAttributeTypes.dtDouble)
                .SetWidth(100)
                .Editor.Add(
                    new NumberField
                        {
                            AllowBlank = false, 
                            AllowDecimals = true, 
                            DecimalPrecision = 2, 
                            DecimalSeparator = ","
                        });
            
            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = cashPaymentsModel.NameOf(() => cashPaymentsModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = cashPaymentsModel.NameOf(() => cashPaymentsModel.RefRazdPodrName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = cashPaymentsModel.NameOf(() => cashPaymentsModel.CelStatya) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = cashPaymentsModel.NameOf(() => cashPaymentsModel.RefKosgyName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = cashPaymentsModel.NameOf(() => cashPaymentsModel.Payment) });
            if (type.Equals(FX_Org_TipYch.GovernmentID) || (type.Equals(FX_Org_TipYch.BudgetaryID) && yearForm > 2015))
            {
                gridFilters.Filters.Add(new StringFilter { DataIndex = cashPaymentsModel.NameOf(() => cashPaymentsModel.RefVidRashName) });
            }
            
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetServicesWorks()
        {
            Store store = StoreExtensions.StoreUrlCreateDefault(
                ServicesWorksID,
                false,
                Page,
                servicesWorksModel,
                new Dictionary<string, string>
                    {
                        { servicesWorksModel.NameOf(() => servicesWorksModel.RefParent), docIdStr }
                    });

            store.SetBaseParams("docId", docIdStr, ParameterMode.Raw);
            store.SetWriteBaseParams("docId", docIdStr, ParameterMode.Raw);
            
            var gp = UiBuilders.CreateGridPanel(ServicesWorksID, store);
            gp.Title = @"Услуги/работы";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordWithConfirmButton();
            gp.AddSaveButton();

            var handler = string.Concat(Scope, ServicesWorksID, ".Update()");
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = handler;
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.Handler = handler;

            gp.Toolbar().Add(
                new UpLoadFileBtnControl
                    {
                        Upload = false, 
                        Icon = Icon.ArrowDown, 
                        Params = { { "docId", docIdStr } }, 
                        Name = "Импорт услуг из ГЗ", 
                        UploadController = UiBuilders.GetUrl<ResultsOfActivityViewController>("ServicesWorksImportFromStateTask"), 
                        SuccessHandler = @"
                                  Ext.MessageBox.hide();
                                  Ext.net.Notification.show({{iconCls : 'icon-information',
                                                                 html : result.extraParams.msg,
                                                              title : 'Уведомление', hideDelay : 2500}});
                                  {0}Store.reload();".FormatWith(ServicesWorksID)   
                    }

                    .Build(Page));

            gp.ColumnModel.AddColumn(() => servicesWorksModel.NRazdel, DataAttributeTypes.dtInteger)
                .SetEditableInteger().SetWidth(100).SetMaxLengthEdior(10).SetHidden(true);

            var param = new Dictionary<string, object> { { "docId", docIdStr } };

            var url = UiBuilders.GetUrl<StateTaskController>("GetServices", param);

            var dictionary = new Dictionary<string, string[]>
                {
                    { servicesWorksModel.NameOf(() => servicesWorksModel.RefVedPch), new[] { "ID" } },
                    { servicesWorksModel.NameOf(() => servicesWorksModel.RefVedPchName), new[] { "Name" } },
                    { servicesWorksModel.NameOf(() => servicesWorksModel.RefTipYName), new[] { "TypeOfService" } },
                    { servicesWorksModel.NameOf(() => servicesWorksModel.RefTipY), new[] { "TypeOfServiceID" } }
                };

            gp.ColumnModel.AddColumn(() => servicesWorksModel.RefVedPchName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(D_Services_VedPer.Key, Page, dictionary, url);

            gp.ColumnModel.AddColumn(() => servicesWorksModel.RefTipYName, DataAttributeTypes.dtString)
                .SetWidth(150).SetEditable(false);

            gp.ColumnModel.AddColumn(() => servicesWorksModel.Customers, DataAttributeTypes.dtInteger)
                .SetNullable().SetEditableInteger().SetWidth(100).SetMaxLengthEdior(9);

            gp.ColumnModel.AddColumn(() => servicesWorksModel.Complaints, DataAttributeTypes.dtInteger)
                .SetNullable().SetEditableInteger().SetWidth(100).SetMaxLengthEdior(9);

            gp.ColumnModel.AddColumn(() => servicesWorksModel.Reaction, DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = servicesWorksModel.NameOf(() => servicesWorksModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesWorksModel.NameOf(() => servicesWorksModel.RefVedPchName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesWorksModel.NameOf(() => servicesWorksModel.RefTipYName) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = servicesWorksModel.NameOf(() => servicesWorksModel.Customers) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = servicesWorksModel.NameOf(() => servicesWorksModel.Complaints) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesWorksModel.NameOf(() => servicesWorksModel.Reaction) });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetServicesWorks2016()
        {
            Store store = StoreExtensions.StoreUrlCreateDefault(
                 ServicesWorks2016ID,
                 false,
                 Page,
                 servicesWorks2016Model,
                 new Dictionary<string, string>
                     {
                        { servicesWorks2016Model.NameOf(() => servicesWorks2016Model.RefParent), docIdStr } 
                     });
            
            store.SetBaseParams("docId", docIdStr, ParameterMode.Raw);
            store.SetWriteBaseParams("docId", docIdStr, ParameterMode.Raw);
            
            var gp = UiBuilders.CreateGridPanel(ServicesWorks2016ID, store);
            gp.Title = @"Услуги/работы";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordWithConfirmButton();
            gp.AddSaveButton();

            var handler = string.Concat(Scope, ServicesWorks2016ID, ".Update()");
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = handler;
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.Handler = handler;

            gp.Toolbar().Add(
                new UpLoadFileBtnControl
                {
                    Upload = false,
                    Icon = Icon.ArrowDown,
                    Params = { { "docId", docIdStr } },
                    Name = "Импорт услуг из ГЗ",
                    UploadController = UiBuilders.GetUrl<ResultsOfActivityViewController>("ServicesWorks2016ImportFromStateTask"),
                    SuccessHandler = @"
                                  Ext.MessageBox.hide();
                                  Ext.net.Notification.show({{iconCls : 'icon-information',
                                                                 html : result.extraParams.msg,
                                                              title : 'Уведомление', hideDelay : 2500}});
                                  {0}Store.reload();".FormatWith(ServicesWorks2016ID)
                }

                    .Build(Page));

            gp.ColumnModel.AddColumn(() => servicesWorks2016Model.NRazdel, DataAttributeTypes.dtInteger)
                .SetEditableInteger().SetWidth(100).SetMaxLengthEdior(10).SetHidden(true);

            var param = new Dictionary<string, object> { { "docId", docIdStr }, { "isOtherSources", false } };

            var url = UiBuilders.GetUrl<StateTask2016Controller>("GetServices", param);

            var dictionary = new Dictionary<string, string[]>
                {
                    { servicesWorks2016Model.NameOf(() => servicesWorks2016Model.RefService), new[] { "RefService" } },
                    { servicesWorks2016Model.NameOf(() => servicesWorks2016Model.RefServiceName), new[] { "NameName", "SvcCntsName1Val", "SvcCntsName2Val", "SvcCntsName3Val", "SvcTermsName1Val", "SvcTermsName2Val" } },
                    { servicesWorks2016Model.NameOf(() => servicesWorks2016Model.RefServiceTypeName), new[] { "RefServiceTypeName" } },
                    { servicesWorks2016Model.NameOf(() => servicesWorks2016Model.RefServiceTypeCode), new[] { "RefServiceTypeCode" } }
                };

            gp.ColumnModel.AddColumn(() => servicesWorks2016Model.RefServiceName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(D_Services_Service.Key, Page, dictionary, url);

            gp.ColumnModel.AddColumn(() => servicesWorks2016Model.RefServiceTypeName, DataAttributeTypes.dtString)
                .SetWidth(150).SetEditable(false);

            gp.ColumnModel.AddColumn(() => servicesWorks2016Model.Customers, DataAttributeTypes.dtInteger)
                .SetNullable().SetEditableInteger().SetWidth(100).SetMaxLengthEdior(9);

            gp.ColumnModel.AddColumn(() => servicesWorks2016Model.Complaints, DataAttributeTypes.dtInteger)
                .SetNullable().SetEditableInteger().SetWidth(100).SetMaxLengthEdior(9);

            gp.ColumnModel.AddColumn(() => servicesWorks2016Model.Reaction, DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = servicesWorks2016Model.NameOf(() => servicesWorks2016Model.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesWorks2016Model.NameOf(() => servicesWorks2016Model.RefServiceName) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesWorks2016Model.NameOf(() => servicesWorks2016Model.RefServiceTypeName) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = servicesWorks2016Model.NameOf(() => servicesWorks2016Model.Customers) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = servicesWorks2016Model.NameOf(() => servicesWorks2016Model.Complaints) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = servicesWorks2016Model.NameOf(() => servicesWorks2016Model.Reaction) });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private FormPanel GetPropertyUse()
        {
            GetStoreByModel(propertyUseModel, PropertyUseID);

            var form = GetDefaltPanel();
            form.LabelWidth = 500;
            form.ID = PropertyUseID;
            form.Title = @"Использование имущества";
            form.Url = UiBuilders.GetUrl<ResultsOfActivityViewController>("PropertyUseSave");
            form.Listeners.ClientValidation.Handler = string.Concat(Scope, PropertyUseID, ".ClientValidation(valid);");

            form.TopBar.Add(GetToolbar(PropertyUseID));

            var copmositFields = new CompositeField { AnchorHorizontal = "100%" };
            copmositFields.Items.Add(new DisplayField { Text = @"Стоимость на начало отчетного года, руб.", Flex = 1 });
            copmositFields.Items.Add(new DisplayField { Text = @"Стоимость на конец отчетного года, руб.", Flex = 1 });
            form.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.AmountOfFundsFromDisposalID)));

            copmositFields = GetCompositeField(() => propertyUseModel.AmountOfFundsFromDisposalID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.AmountOfFundsFromDisposalBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.AmountOfFundsFromDisposalEndYear));
            form.Items.Add(copmositFields);

            var group = new FieldSet { Title = @"Сведения о балансовой стоимости имущества" };

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.BookValueOfRealEstateTotalID)));

            copmositFields = GetCompositeField(() => propertyUseModel.BookValueOfRealEstateTotalID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.BookValueOfRealEstateTotalBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.BookValueOfRealEstateTotalEndYear));
            group.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.ImmovablePropertyGivenOnRentID)));

            copmositFields = GetCompositeField(() => propertyUseModel.ImmovablePropertyGivenOnRentID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.ImmovablePropertyGivenOnRentBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.ImmovablePropertyGivenOnRentEndYear));
            group.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.ImmovablePropertyDonatedID)));

            copmositFields = GetCompositeField(() => propertyUseModel.ImmovablePropertyDonatedID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.ImmovablePropertyDonatedBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.ImmovablePropertyDonatedEndYear));
            group.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalID)));

            copmositFields = GetCompositeField(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.CarryingAmountOfMovablePropertyTotalEndYear));
            group.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.MovablePropertyGivenOnRentID)));

            copmositFields = GetCompositeField(() => propertyUseModel.MovablePropertyGivenOnRentID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.MovablePropertyGivenOnRentBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.MovablePropertyGivenOnRentEndYear));
            group.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.MovablePropertyDonatedID)));

            copmositFields = GetCompositeField(() => propertyUseModel.MovablePropertyDonatedID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.MovablePropertyDonatedBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.MovablePropertyDonatedEndYear));
            group.Items.Add(copmositFields);
            form.Items.Add(group);

            group = new FieldSet { Title = @"Сведения о площадях недвижимого имущества", Layout = LayoutType.Form.ToString() };
            copmositFields = new CompositeField { AnchorHorizontal = "100%" };
            copmositFields.Items.Add(new DisplayField { Text = @"Площадь на начало отчетного года, кв.м.", Flex = 1 });
            copmositFields.Items.Add(new DisplayField { Text = @"Площадь на конец отчетного года, кв.м.", Flex = 1 });
            group.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.AreaOfRealEstateTotalID)));

            copmositFields = GetCompositeField(() => propertyUseModel.AreaOfRealEstateTotalID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.AreaOfRealEstateTotalBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.AreaOfRealEstateTotalEndYear));
            group.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.GivenOnRentID)));

            copmositFields = GetCompositeField(() => propertyUseModel.GivenOnRentID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.GivenOnRentBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.GivenOnRentEndYear));
            group.Items.Add(copmositFields);

            form.Items.Add(GetHidden(propertyUseModel.NameOf(() => propertyUseModel.DonatedID)));

            copmositFields = GetCompositeField(() => propertyUseModel.DonatedID);
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.DonatedBeginYear));
            copmositFields.Items.Add(GetNumberFieldFlex2(() => propertyUseModel.DonatedEndYear));
            group.Items.Add(copmositFields);
            form.Items.Add(group);

            return form;
        }
    }
}
