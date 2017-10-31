using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.PfhdModel;
using Krista.FM.RIA.Extensions.E86N.Models.PfhdModel.Pfhd2017;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.PFHD;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    // todo избавиться от дублирования кода по одинаковым деталям!!!

    /// <summary>
    ///  ПФХД 2017
    /// </summary>
    public sealed class Pfhd2017View : DocBaseView
    {
        #region Setting

            private const string Scope = "E86n.View.Pfhd2017View";
            private const string FinancialIndexFormID = "FinancialIndexForm";
            private const string TemporaryResourcesFormID = "TemporaryResourcesForm";
            private const string ReferenceFormID = "ReferenceForm";
            private const string ExpensePaymentGridID = "ExpensePaymentGrid";
            private const string PlanPaymentIndexID = "PlanPaymentIndexGrid";
            private const string CapitalConstructionFundsID = "CapitalConstructionFundsGrid";
            private const string RealAssetFundsID = "RealAssetFundsGrid";
            private const string OtherGrantFundsID = "OtherGrantFundsGrid";

            private readonly FinancialIndexViewModel financialIndexViewModel = new FinancialIndexViewModel();
            private readonly TemporaryResourcesViewModel temporaryResourcesViewModel = new TemporaryResourcesViewModel();
            private readonly ReferenceViewModel referenceViewModel = new ReferenceViewModel();
            private readonly ExpensePaymentIndexViewModel expensePaymentIndexViewModel = new ExpensePaymentIndexViewModel();
            private readonly PlanPaymentIndexViewModel planPaymentIndexViewModel = new PlanPaymentIndexViewModel();
            private readonly RealAssetFundsViewModel realAssetModel = new RealAssetFundsViewModel();
            private readonly OtherGrantFundsViewModel otherGrantModel = new OtherGrantFundsViewModel();
            private readonly CapFundsViewModel capFundsModel = new CapFundsViewModel();
        #endregion

        public override List<Component> Build(ViewPage page)
        {
            ViewController = UiBuilders.GetControllerID<Pfhd2017ViewController>();
            ReadOnlyDocHandler = Scope + ".SetReadOnlyDoc";
            var components = base.Build(page);

            ResourceManager.RegisterClientScriptBlock("Pfhd2017View", Resource.Pfhd2017View);
            
            Detail = UiBuilders.GetTabbedDetails(new List<Component>
                                          {
                                              GetFinancialIndex(),
                                              GetPlanPaymentIndex(),
                                              GetExpensePaymentIndex(),
                                              GetTemporaryResources(),
                                              GetReference(),
                                              GetActionGrant(),
                                              new DocsDetailControl(DocId).BuildComponent(page)
                                          });

            Detail.ID = "DetailTabPanel";
            ((TabPanel)Detail).Listeners.TabChange.Fn(Scope, "reloadDetail");

            return components;
        }

        private Component GetActionGrant()
        {
            var tabs = UiBuilders.GetTabbedDetails(new List<Component>
                                          {
                                                GetCapitalConstructionFunds(),
                                                GetRealAssetsFunds(),
                                                GetOtherGrantFunds()
                                          });

            tabs.ID = "ActionGrantTabPanel";
            ((TabPanel)tabs).Title = @"Операции с целевыми средствами";
            ((TabPanel)tabs).Listeners.TabChange.Fn(Scope, "reloadDetail");
         
            return tabs;
        }

        private Component GetOtherGrantFunds()
        {
            Store store = StoreExtensions.StoreCreateDefault(
               string.Concat(OtherGrantFundsID, "Store"),
               false,
               typeof(OtherGrantFundsController),
               "Read",
               "Save",
               "Save");

            store.SetBaseParams("parentId", DocIdStr, ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", DocIdStr, ParameterMode.Raw);
            store.AddFieldsByClass(otherGrantModel);
            Page.Controls.Add(store);
            
            GridPanel gridPanel = UiBuilders.CreateGridPanel(OtherGrantFundsID, store);
            gridPanel.Title = @"Информация об операциях с субсидиями на иные цели";

            gridPanel.AddRefreshButton();
            gridPanel.AddNewRecordNoEditButton();
            gridPanel.AddRemoveRecordButton();
            gridPanel.AddSaveButton();

            gridPanel.ColumnModel.AddColumn(() => otherGrantModel.RefOtherGrantCode, DataAttributeTypes.dtInteger)
                .SetWidth(100).SetEditable(false).Renderer.Handler = "return buildMask(value, '###.##.####');";

            gridPanel.ColumnModel.AddColumn(() => otherGrantModel.RefOtherGrantName, DataAttributeTypes.dtInteger)
                .SetWidth(400).SetHbLookup(D_Fin_OtherGant.Key, "{0}.getOtherGrantFilter('{1}')".FormatWith(Scope, DocIdStr));

            gridPanel.ColumnModel.AddColumn(() => otherGrantModel.Funds, DataAttributeTypes.dtDouble)
                .SetEditableDouble(2).SetWidth(250);

            var temp = gridPanel.ColumnModel.AddColumn(() => otherGrantModel.KOSGY, DataAttributeTypes.dtInteger);
            temp.SetWidth(50).Editor.Add(new TextField { AllowBlank = false, MaxLength = 3, MaskRe = @"[0-9]" });
            temp.Renderer.Handler = "return buildMask(value, '#.#.#');";

            var summ = new SummFieldControl("Summ1", store, otherGrantModel.NameOf(() => otherGrantModel.Funds), "Сумма по всем субсидиям");

            gridPanel.BottomBar.Add(new Toolbar { Items = { summ.BuildComponent(Page) } });

            return gridPanel;
        }

        private Component GetRealAssetsFunds()
        {
            Store store = StoreExtensions.StoreCreateDefault(
               string.Concat(RealAssetFundsID, "Store"),
               false,
               typeof(RealAssetFundsController),
               "Read",
               "Save",
               "Save");

            store.SetBaseParams("parentId", DocIdStr, ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", DocIdStr, ParameterMode.Raw);
            store.AddFieldsByClass(realAssetModel);
            Page.Controls.Add(store);
            
            GridPanel gridPanel = UiBuilders.CreateGridPanel(RealAssetFundsID, store);
            gridPanel.Title = @"Информация об объектах приобретаемого недвижимого имущества";

            gridPanel.AddRefreshButton();
            gridPanel.AddNewRecordNoEditButton();
            gridPanel.AddRemoveRecordButton();
            gridPanel.AddSaveButton();

            gridPanel.ColumnModel.AddColumn(() => realAssetModel.ID, DataAttributeTypes.dtInteger).SetHidden(true);
            gridPanel.ColumnModel.AddColumn(() => realAssetModel.RefParameterID, DataAttributeTypes.dtInteger)
                .SetHidden(true);
            gridPanel.ColumnModel.AddColumn(() => realAssetModel.Name, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(500).SetMaxLengthEdior(255);

            gridPanel.ColumnModel.AddColumn(() => realAssetModel.Funds, DataAttributeTypes.dtDouble)
                .SetEditableDouble(2).SetWidth(250);

            var summ = new SummFieldControl("Summ2", store, realAssetModel.NameOf(() => realAssetModel.Funds), "Общая сумма");

            gridPanel.BottomBar.Add(new Toolbar { Items = { summ.BuildComponent(Page) } });

            return gridPanel;
        }

        private Component GetCapitalConstructionFunds()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                string.Concat(CapitalConstructionFundsID, "Store"),
                false,
                typeof(CapFundsController),
                "Read",
                "Save",
                "Save");

            store.SetBaseParams("parentId", DocIdStr, ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", DocIdStr, ParameterMode.Raw);
            store.AddFieldsByClass(capFundsModel);
            Page.Controls.Add(store);

            GridPanel gridPanel = UiBuilders.CreateGridPanel(CapitalConstructionFundsID, store);
            gridPanel.Title = @"Информация об объектах капитального строительства";

            gridPanel.AddRefreshButton();
            gridPanel.AddNewRecordNoEditButton();
            gridPanel.AddRemoveRecordButton();
            gridPanel.AddSaveButton();

            gridPanel.ColumnModel.AddColumn(() => capFundsModel.ID, DataAttributeTypes.dtInteger)
                .SetHidden(true);
            gridPanel.ColumnModel.AddColumn(() => capFundsModel.RefParameterID, DataAttributeTypes.dtInteger)
                .SetHidden(true);
            gridPanel.ColumnModel.AddColumn(() => capFundsModel.Name, DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(500).SetMaxLengthEdior(255);

            gridPanel.ColumnModel.AddColumn(() => capFundsModel.Funds, DataAttributeTypes.dtDouble)
                .SetEditableDouble(2).SetWidth(250);

            var summ = new SummFieldControl("Summ3", store, capFundsModel.NameOf(() => capFundsModel.Funds), "Общая сумма");

            gridPanel.BottomBar.Add(new Toolbar { Items = { summ.BuildComponent(Page) } });

            return gridPanel;
        }

        /// <summary>
        /// Плановые показатели поступлений и выплат
        /// </summary>
        private Component GetPlanPaymentIndex()
        {
            var tabs = UiBuilders.GetTabbedDetails(new List<Component>
                                          {
                                              GetPlanPaymentIndexTab(0),
                                              GetPlanPaymentIndexTab(1),
                                              GetPlanPaymentIndexTab(2)
                                          });

            tabs.ID = "PlanPaymentIndexTabPanel";
            ((TabPanel)tabs).Title = @"Плановые показатели поступлений и выплат";
            ((TabPanel)tabs).Listeners.TabChange.Fn(Scope, "reloadDetail");

            return tabs;
        }

        private Component GetPlanPaymentIndexTab(int period)
        {
            var year = NewRestService.GetItem<F_F_ParameterDoc>(DocId).RefYearForm.ID;

            var store = StoreExtensions.StoreCreateDefault(
                string.Concat(PlanPaymentIndexID + period, "Store"),
                false,
                ViewController,
                string.Concat(PlanPaymentIndexID, "Read"),
                string.Concat(PlanPaymentIndexID, "Save"),
                string.Concat(PlanPaymentIndexID, "Save"),
                string.Concat(PlanPaymentIndexID, "Delete"));
            store.SetBaseParams("docId", DocIdStr, ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocIdStr, ParameterMode.Raw);
            store.SetBaseParams("period", period.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("period", period.ToString(), ParameterMode.Raw);
            store.AddFieldsByClass(planPaymentIndexViewModel);
            Page.Controls.Add(store);
            
            var gp = UiBuilders.CreateGridPanel(PlanPaymentIndexID + period, store);
            gp.Title = (year + period) + @"год";

            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.Name, DataAttributeTypes.dtString);
            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.LineCode, DataAttributeTypes.dtString);
            var kbk = gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.Kbk);
            kbk.EditorOptions.Listeners.BeforeStartEdit.Handler = "return {0}.PlanPaymentIndexGridBeforeStartEdit(editor, '{1}');".FormatWith(Scope, gp.ID);
            var editorKbk = (TextArea)kbk.Editor[0];
            editorKbk.Regex = @"\S{3}";
            editorKbk.RegexText =
                @"Значение «{0}» должно быть равно 3 символам и не содержать пробелов".FormatWith(UiBuilders.DescriptionOf(() => planPaymentIndexViewModel.Kbk));

            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.Total)
                .EditorOptions.Listeners.BeforeStartEdit.Handler = "return {0}.PlanPaymentIndexGridBeforeStartEdit(editor, '{1}');".FormatWith(Scope, gp.ID);
            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.FinancialProvision)
                .EditorOptions.Listeners.BeforeStartEdit.Handler = "return {0}.PlanPaymentIndexGridBeforeStartEdit(editor, '{1}');".FormatWith(Scope, gp.ID);
            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.SubsidyOtherPurposes)
                .EditorOptions.Listeners.BeforeStartEdit.Handler = "return {0}.PlanPaymentIndexGridBeforeStartEdit(editor, '{1}');".FormatWith(Scope, gp.ID);
            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.CapitalInvestment)
                .EditorOptions.Listeners.BeforeStartEdit.Handler = "return {0}.PlanPaymentIndexGridBeforeStartEdit(editor, '{1}');".FormatWith(Scope, gp.ID);
            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.HealthInsurance)
                .EditorOptions.Listeners.BeforeStartEdit.Handler = "return {0}.PlanPaymentIndexGridBeforeStartEdit(editor, '{1}');".FormatWith(Scope, gp.ID);
            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.ServiceTotal)
                .EditorOptions.Listeners.BeforeStartEdit.Handler = "return {0}.PlanPaymentIndexGridBeforeStartEdit(editor, '{1}');".FormatWith(Scope, gp.ID);
            gp.ColumnModel.AddColumn(() => planPaymentIndexViewModel.ServiceGrant)
                .EditorOptions.Listeners.BeforeStartEdit.Handler = "return {0}.PlanPaymentIndexGridBeforeStartEdit(editor, '{1}');".FormatWith(Scope, gp.ID);

            gp.AddRefreshButton();
            gp.AddSaveButton();

            var headerGroupRow = new HeaderGroupRow();
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 4 });
            headerGroupRow.Columns.Add(
                new HeaderGroupColumn
                {
                    Header = "Объем финансового обеспечения, руб. (с точностью до двух знаков после запятой - 0,00)",
                    ColSpan = 7,
                    Align = Alignment.Center
                });

            gp.View[0].HeaderGroupRows.Add(headerGroupRow);

            headerGroupRow = new HeaderGroupRow();
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 4 });
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 1 });
            headerGroupRow.Columns.Add(
                new HeaderGroupColumn
                {
                    Header = "в том числе:",
                    ColSpan = 6,
                    Align = Alignment.Center
                });

            gp.View[0].HeaderGroupRows.Add(headerGroupRow);

            headerGroupRow = new HeaderGroupRow();
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 4 });
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 1 });
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 4 });
            headerGroupRow.Columns.Add(
                new HeaderGroupColumn
                {
                    Header = "поступления от оказания платных услуг (выполнения работ) и от иной приносящей доход деятельности",
                    ColSpan = 2,
                    Align = Alignment.Center
                });

            gp.View[0].HeaderGroupRows.Add(headerGroupRow);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }
        
        /// <summary>
        /// Показатели выплат по расходам на закупку 
        /// </summary>
        private Component GetExpensePaymentIndex()
        {
            var year = NewRestService.GetItem<F_F_ParameterDoc>(DocId).RefYearForm.ID;

            var store = GetStoreByModel(expensePaymentIndexViewModel, ExpensePaymentGridID);
            
            var gp = UiBuilders.CreateGridPanel(ExpensePaymentGridID, store);
            gp.Title = @"Показатели выплат по расходам на закупку";
            
            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddDeleteRecordWithConfirmButton();
            gp.AddSaveButton();
            
            gp.Toolbar().Add(new ToolbarSeparator());
            
            var btnSumm = new Button
            {
                ID = string.Concat(gp.ID, "SummBtn"),
                Icon = Icon.Sum,
                Text = @"Подсчет итоговых строк",
                ToolTip = @"Вычислить суммы",
                DirectEvents =
                        {
                            Click =
                                {
                                    Url = UiBuilders.GetUrl<Pfhd2017ViewController>("ExpensePaymentGridCalculateSumm"),
                                    CleanRequest = true,
                                    ExtraParams =
                                        {
                                            new Parameter("docId", DocIdStr, ParameterMode.Raw),
                                        },
                                    Success = "{0}.reload();".FormatWith(gp.StoreID),
                                    Before = string.Concat("return ", Scope, ".BeforeSumm('{0}');".FormatWith(gp.ID))
                                }
                        }
            };

            gp.Toolbar().Add(btnSumm);

            gp.ColumnModel.AddColumn(() => expensePaymentIndexViewModel.Name).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(() => expensePaymentIndexViewModel.LineCode).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(() => expensePaymentIndexViewModel.Year).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");
            
            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.TotalSumNextYear),
                                    "на {0}г.<br>очередной финансовый год".FormatWith(year),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.TotalSumFirstPlanYear),
                                    "на {0}г.<br>1-ый год планового периода".FormatWith(year + 1),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.TotalSumSecondPlanYear),
                                    "на {0}г.<br>2-ой год планового периода".FormatWith(year + 2),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz44SumNextYear),
                                    "на {0}г.<br>очередной финансовый год".FormatWith(year),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz44SumFirstPlanYear),
                                    "на {0}г.<br>1-ый год планового периода".FormatWith(year + 1),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz44SumSecondPlanYear),
                                    "на {0}г.<br>2-ой год планового периода".FormatWith(year + 2),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz223SumNextYear),
                                    "на {0}г.<br>очередной финансовый год".FormatWith(year),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz223SumFirstPlanYear),
                                    "на {0}г.<br>1-ый год планового периода".FormatWith(year + 1),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");

            gp.ColumnModel.AddColumn(
                                    UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz223SumSecondPlanYear),
                                    "на {0}г.<br>2-ой год планового периода".FormatWith(year + 2),
                                    DataAttributeTypes.dtDouble).SetEditableDouble(2).EditorOptions.Listeners.BeforeStartEdit.Fn(Scope, "ExpensePaymentGridBeforeStartEdit");
            
            var headerGroupRow = new HeaderGroupRow();
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 4 });
            headerGroupRow.Columns.Add(
                new HeaderGroupColumn
                {
                    Header = "Сумма выплат по расходам на закупку товаров, работ и услуг, руб. (с точностью до двух знаков после запятой - 0,00)",
                    ColSpan = 9,
                    Align = Alignment.Center
                });
            
            gp.View[0].HeaderGroupRows.Add(headerGroupRow);

            headerGroupRow = new HeaderGroupRow();
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 4 });
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 3 });
            headerGroupRow.Columns.Add(
                new HeaderGroupColumn
                {
                    Header = "в том числе:",
                    ColSpan = 6,
                    Align = Alignment.Center
                });

            gp.View[0].HeaderGroupRows.Add(headerGroupRow);

            headerGroupRow = new HeaderGroupRow();
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 4 });
            headerGroupRow.Columns.Add(new HeaderGroupColumn
                                            {
                                                Header = "всего на закупки",
                                                ColSpan = 3,
                                                Align = Alignment.Center
                                            });
            headerGroupRow.Columns.Add(new HeaderGroupColumn
                                            {
                                                Header = "в соответствии с 44-ФЗ",
                                                ColSpan = 3,
                                                Align = Alignment.Center
                                            });
            headerGroupRow.Columns.Add(new HeaderGroupColumn
                                            {
                                                Header = "в соответствии с 223-ФЗ",
                                                ColSpan = 3,
                                                Align = Alignment.Center
                                            });

            gp.View[0].HeaderGroupRows.Add(headerGroupRow);
            
            var gridFilters = new GridFilters { Local = true };
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.ID) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.Name) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.Year) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.TotalSumNextYear) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.TotalSumFirstPlanYear) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.TotalSumSecondPlanYear) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz44SumNextYear) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz44SumFirstPlanYear) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz44SumSecondPlanYear) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz223SumNextYear) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz223SumFirstPlanYear) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => expensePaymentIndexViewModel.Fz223SumSecondPlanYear) });
            
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        /// <summary>
        /// Справочная информация
        /// </summary>
        private Component GetReference()
        {
            var store = GetStoreByModel(referenceViewModel, ReferenceFormID);
            store.Listeners.DataChanged.Handler = Scope + ".StoreDataChanged(store, '{0}');".FormatWith(ReferenceFormID);

            var form = FormPanelExtensions.GetDefaultFormPanel(ReferenceFormID);
            form.Title = @"Справочная информация";
            form.LabelWidth = 500;
            form.Url = UiBuilders.GetUrl<Pfhd2017ViewController>(ReferenceFormID + "Save");
            form.BaseParams.Add(new Parameter("docId", DocIdStr));
            form.Listeners.ClientValidation.Fn(Scope, "ClientValidation");

            form.AddRefreshButton();
            form.AddSaveButton();

            var copmositFields = FormPanelExtensions.GetCompositeField(() => referenceViewModel.AmountPublicLiabilities);
            var field = new Label
            {
                Text = @"010",
                Width = 100
            };
            copmositFields.Items.Add(field);
            copmositFields.Items.AddFormField(UiBuilders.SchemeOf(() => referenceViewModel.AmountPublicLiabilities)).Flex = 1;
            form.Items.Add(copmositFields);

            copmositFields = FormPanelExtensions.GetCompositeField(() => referenceViewModel.VolumeBudgetIinvestments);
            field = new Label
            {
                Text = @"020",
                Width = 100
            };
            copmositFields.Items.Add(field);
            copmositFields.Items.AddFormField(UiBuilders.SchemeOf(() => referenceViewModel.VolumeBudgetIinvestments)).Flex = 1;
            form.Items.Add(copmositFields);

            copmositFields = FormPanelExtensions.GetCompositeField(() => referenceViewModel.AmountTemporaryOrder);
            field = new Label
            {
                Text = @"030",
                Width = 100
            };
            copmositFields.Items.Add(field);
            copmositFields.Items.AddFormField(UiBuilders.SchemeOf(() => referenceViewModel.AmountTemporaryOrder)).Flex = 1;
            form.Items.Add(copmositFields);

            return form;
        }

        /// <summary>
        /// Сведения о средствах, поступающих во временное распоряжение учреждения
        /// </summary>
        private Component GetTemporaryResources()
        {
            var store = GetStoreByModel(temporaryResourcesViewModel, TemporaryResourcesFormID);
            store.Listeners.DataChanged.Handler = Scope + ".StoreDataChanged(store, '{0}');".FormatWith(TemporaryResourcesFormID);

            var form = FormPanelExtensions.GetDefaultFormPanel(TemporaryResourcesFormID);
            form.Title = @"Сведения о средствах, поступающих во временное распоряжение учреждения";
            form.LabelWidth = 300;
            form.Url = UiBuilders.GetUrl<Pfhd2017ViewController>(TemporaryResourcesFormID + "Save");
            form.BaseParams.Add(new Parameter("docId", DocIdStr));
            form.Listeners.ClientValidation.Fn(Scope, "ClientValidation");

            form.AddRefreshButton();
            form.AddSaveButton();

            var copmositFields = FormPanelExtensions.GetCompositeField(() => temporaryResourcesViewModel.BalanceBeginningYear);
            var field = new Label
                            {
                                Text = @"010",
                                Width = 100
                            };
            copmositFields.Items.Add(field);
            copmositFields.Items.AddFormField(UiBuilders.SchemeOf(() => temporaryResourcesViewModel.BalanceBeginningYear)).Flex = 1;
            form.Items.Add(copmositFields);

            copmositFields = FormPanelExtensions.GetCompositeField(() => temporaryResourcesViewModel.BalanceEndYear);
            field = new Label
            {
                Text = @"020",
                Width = 100
            };
            copmositFields.Items.Add(field);
            copmositFields.Items.AddFormField(UiBuilders.SchemeOf(() => temporaryResourcesViewModel.BalanceEndYear)).Flex = 1;
            form.Items.Add(copmositFields);

            copmositFields = FormPanelExtensions.GetCompositeField(() => temporaryResourcesViewModel.Income);
            field = new Label
            {
                Text = @"030",
                Width = 100
            };
            copmositFields.Items.Add(field);
            copmositFields.Items.AddFormField(UiBuilders.SchemeOf(() => temporaryResourcesViewModel.Income)).Flex = 1;
            form.Items.Add(copmositFields);

            copmositFields = FormPanelExtensions.GetCompositeField(() => temporaryResourcesViewModel.Disposals);
            field = new Label
            {
                Text = @"040",
                Width = 100
            };
            copmositFields.Items.Add(field);
            copmositFields.Items.AddFormField(UiBuilders.SchemeOf(() => temporaryResourcesViewModel.Disposals)).Flex = 1;
            form.Items.Add(copmositFields);

            return form;
        }

        /// <summary>
        /// Показатели финансового состояния учреждения
        /// </summary>
        private FormPanel GetFinancialIndex()
        {
            var store = GetStoreByModel(financialIndexViewModel, FinancialIndexFormID);
            store.Listeners.DataChanged.Handler = Scope + ".StoreDataChanged(store, '{0}');".FormatWith(FinancialIndexFormID);

            var form = FormPanelExtensions.GetDefaultFormPanel(FinancialIndexFormID);
            form.Title = @"Показатели финансового состояния учреждения";
            form.LabelWidth = 600;
            form.Url = UiBuilders.GetUrl<Pfhd2017ViewController>(FinancialIndexFormID + "Save");
            form.BaseParams.Add(new Parameter("docId", DocIdStr));
            form.Listeners.ClientValidation.Fn(Scope, "ClientValidation");

            form.AddRefreshButton();
            var handler = @"
var msg = window.E86n.View.Pfhd2017View.FinancialIndexFormValidation();
if (msg !== '') {{
    window.Ext.Msg.show({{
                title: 'Ошибки контроля',
                msg: msg,
                buttons: window.Ext.Msg.OK,
                icon: window.Ext.MessageBox.ERROR,
                maxWidth: 1000
            }});
}}
else {{
        window.{0}.form.submit({{
            waitMsg: 'Сохранение...',
            success: function (form, action) {{
                if (action.result && action.result.extraParams && action.result.extraParams.msg)
                    window.Ext.net.Notification.show({{
                        iconCls: 'icon-information',
                        html: action.result.extraParams.msg,
                        title: 'Сохранение',
                        hideDelay: 2000
                    }});
            }},
            failure: function (form, action) {{
                if (action.result && action.result.extraParams && action.result.extraParams.msg)
                    window.Ext.Msg.show({{
                        title: 'Ошибка сохранения',
                        msg: action.result.extraParams.msg,
                        buttons: window.Ext.Msg.OK,
                        icon: window.Ext.MessageBox.ERROR,
                        maxWidth: 1000
                    }});
                window.{0}Store.reload();
            }}
        }})
     }};".FormatWith(form.ID);

            form.Toolbar().AddIconButton("{0}SaveBtn".FormatWith(form.ID), Icon.Disk, "Сохранить изменения", handler);
            
            var group = new FieldSet
                        {
                            Title = @"Нефинансовые активы",
                            DefaultAnchor = "0",
                            Layout = LayoutType.Form.ToString(),
                            StyleSpec = "padding-bottom: 0;"
            };
            
            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.NonFinancialAssets));
            
            group.Items.Add(
               new Label
               {
                   Text = @"Из них:"
               });

            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.RealAssets));
            
            group.Items.Add(
              new Label
              {
                  Text = @"в том числе:"
              });

            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.RealAssetsDepreciatedCost));
            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.HighValuePersonalAssets));

            group.Items.Add(
              new Label
              {
                  Text = @"в том числе:"
              });

            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.HighValuePADepreciatedCost));
            form.Items.Add(group);

            group = new FieldSet
            {
                Title = @"Финансовые активы",
                DefaultAnchor = "0",
                Layout = LayoutType.Form.ToString(),
                StyleSpec = "padding-bottom: 0;"
            };

            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.FinancialAssets));

            group.Items.Add(
               new Label
               {
                   Text = @"Из них:"
               });

            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.MoneyInstitutions));

            group.Items.Add(
              new Label
              {
                  Text = @"в том числе:"
              });

            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.FundsAccountsInstitution));
            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.FundsPlacedOnDeposits));

            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.OtherFinancialInstruments));
            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.DebitIncome));
            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.DebitExpense));
            form.Items.Add(group);

            group = new FieldSet
            {
                Title = @"Обязательства",
                Layout = LayoutType.Form.ToString(),
                DefaultAnchor = "0",
                StyleSpec = "padding-bottom: 0;"
            };
            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.FinancialCircumstanc));

            group.Items.Add(
               new Label
               {
                   Text = @"Из них:"
               });

            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.Debentures));
            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.AccountsPayable));
            group.Items.Add(
              new Label
              {
                  Text = @"в том числе:"
              });
            group.Items.AddFormField(UiBuilders.SchemeOf(() => financialIndexViewModel.KreditExpired));
            form.Items.Add(group);
            return form;
        }
    }
}