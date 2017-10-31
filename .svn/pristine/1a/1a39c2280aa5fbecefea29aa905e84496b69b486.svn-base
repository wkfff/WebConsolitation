using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls.SearchCombobox;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class AnnualBalanceItfSettingsView : View
    {
        private const string IndicatorsGridID = "Indicators";
        private const string SettingsGridID = "Settings";

        private const string DetailsLpGridID = "DetailsLP";

        private const string ViewController = "AnnualBalanceView";

        private const string Scope = "E86n.View.AnnualBalanceItfSettingsView";

        private readonly ICommonDataService commonDataService;

        public AnnualBalanceItfSettingsView()
        {
            commonDataService = Resolver.Get<ICommonDataService>();
        }

        public ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            RestActions restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            ResourceManager.GetInstance(page)
                .RegisterClientScriptBlock("AnnualBalanceItfSettingsView", Resource.AnnualBalanceItfSettingsView);

            Store statesLp = CUtils.GetLookupStore(DetailsLpGridID, "/{0}/SettingsGetDetails".FormatWith(ViewController));

            statesLp.SetBaseParams("partDoc", Scope + ".getPartDoc()", ParameterMode.Raw);
            Page.Controls.Add(statesLp);

            var details = (TabPanel)UiBuilders.GetTabbedDetails(new List<Component>
                                                                    {
                                                                        GetIndicatorsGrid(),
                                                                        GetSettingsGrid()
                                                                    });

            var view = new Viewport
                           { 
                               Layout = LayoutType.Center.ToString(),
                               Items = { details }
                           };

            return new List<Component> { view };
        }

        private Store GetIndicatorsStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                                                        IndicatorsGridID + "Store",
                                                        true,
                                                        ViewController,
                                                        "{0}Read".FormatWith(IndicatorsGridID),
                                                        "{0}Save".FormatWith(IndicatorsGridID),
                                                        "{0}Save".FormatWith(IndicatorsGridID),
                                                        "{0}Delete".FormatWith(IndicatorsGridID));

            store.AddFieldsByEnum(new IndicatorsFields());

            store.Sort(IndicatorsFields.Code.ToString(), SortDirection.ASC);
            Page.Controls.Add(store);

            return store;
        }

        private Store GetSettingsStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                                                        SettingsGridID + "Store",
                                                        true,
                                                        ViewController,
                                                        "{0}Read".FormatWith(SettingsGridID),
                                                        "{0}Save".FormatWith(SettingsGridID),
                                                        "{0}Save".FormatWith(SettingsGridID),
                                                        "{0}Delete".FormatWith(SettingsGridID));

            store.AddFieldsByEnum(new SettingsFields());
            store.Sort(SettingsFields.LineCode.ToString(), SortDirection.ASC);
            Page.Controls.Add(store);

            return store;
        }

        private Component GetIndicatorsGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(IndicatorsGridID, GetIndicatorsStore());
            gp.Title = @"Показатели";
            
            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordWithConfirmButton();
            gp.AddSaveButton();

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = Scope + ".RowSelect('{0}');".FormatWith(IndicatorsGridID);

            gp.ColumnModel.AddColumn(IndicatorsFields.Code.ToString(), AnnualBalanceHelpers.IndicatorsFieldsNameMapping(IndicatorsFields.Code), DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(15);

            AddLineCodeColumn(
                                gp,
                                IndicatorsFields.LineCode.ToString(),
                                AnnualBalanceHelpers.IndicatorsFieldsNameMapping(IndicatorsFields.LineCode));

            gp.ColumnModel.AddColumn(IndicatorsFields.Name.ToString(), AnnualBalanceHelpers.IndicatorsFieldsNameMapping(IndicatorsFields.Name), DataAttributeTypes.dtString)
                .SetEditableString().SetWidth(500);

            gp.Plugins.Add(new GridFilters
            {
                Local = true,
                Filters =
                    {
                        new NumericFilter { DataIndex = IndicatorsFields.Code.ToString() },
                        new StringFilter { DataIndex = IndicatorsFields.LineCode.ToString() },
                        new StringFilter { DataIndex = IndicatorsFields.Name.ToString() }
                    }
            });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private Component GetSettingsGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(SettingsGridID, GetSettingsStore());
            gp.Title = @"Настройки";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordWithConfirmButton();
            gp.AddSaveButton();

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = Scope + ".RowSelect('{0}');".FormatWith(SettingsGridID);

            var url = "/Entity/DataWithCustomSearch?objectKey={0}&start=0&limit=-1&serverFilter=(ID IN ({1}, {2}, {3}, {4}, {5}, {6}, {7}))"
                .FormatWith(
                            FX_FX_PartDoc.Key,
                            FX_FX_PartDoc.AnnualBalanceF0503130Type,
                            FX_FX_PartDoc.AnnualBalanceF0503730Type,
                            FX_FX_PartDoc.AnnualBalanceF0503121Type,
                            FX_FX_PartDoc.AnnualBalanceF0503127Type,
                            FX_FX_PartDoc.AnnualBalanceF0503137Type,
                            FX_FX_PartDoc.AnnualBalanceF0503721Type,
                            FX_FX_PartDoc.AnnualBalanceF0503737Type);

            gp.ColumnModel.AddColumn(
                SettingsFields.RefPartDocName.ToString(),
                AnnualBalanceHelpers.SettingsFieldsNameMapping(SettingsFields.RefPartDocName),
                DataAttributeTypes.dtString)
                .SetWidth(200)
                .AddLookupEditorForColumn(
                    SettingsFields.RefPartDoc.ToString(),
                    SettingsFields.RefPartDocName.ToString(),
                    url,
                    true,
                    Page);

            ((ComboBox)gp.ColumnModel.GetColumnById(SettingsFields.RefPartDocName.ToString()).Editor[0]).Listeners.Select
                .AddAfter(Scope + ".reloadLP();");

            gp.ColumnModel.AddColumn(
                SettingsFields.SectionName.ToString(),
                AnnualBalanceHelpers.SettingsFieldsNameMapping(SettingsFields.SectionName),
                DataAttributeTypes.dtString)
                .SetWidth(200).SetLookup(SettingsFields.Section.ToString(), DetailsLpGridID, "ID", "Name");

            var referencerInfo = ComboboxEditorExtensions.BuildReferencerInfo(
                                        Resolver.Get<IScheme>().RootPackage.FindEntityByName(D_Line_Indicators.Key),
                                        new[] { "ID", "LineCode" },
                                        new[] { "Name" });
           
            gp.ColumnModel.AddColumn(
                SettingsFields.RefIndicatorsName.ToString(),
                AnnualBalanceHelpers.SettingsFieldsNameMapping(SettingsFields.RefIndicatorsName),
                DataAttributeTypes.dtString)
                .SetWidth(200)
                .AddLookupEditorForColumnHardCode1(
                                        SettingsFields.RefIndicatorsName.ToString(),
                                        new Dictionary<string, string[]>
                                            {
                                                { SettingsFields.RefIndicators.ToString(), new[] { "ID" } },
                                                { SettingsFields.RefIndicatorsName.ToString(), new[] { "LineCode", "Name" } }
                                            },
                                        referencerInfo,
                                        string.Format("/EntityExt/DataWithCustomSearch?objectKey={0}", D_Line_Indicators.Key),
                                        Page);

            gp.ColumnModel.AddColumn(SettingsFields.Additional.ToString(), AnnualBalanceHelpers.SettingsFieldsNameMapping(SettingsFields.Additional), DataAttributeTypes.dtString)
               .SetEditableString().SetWidth(500);

            gp.ColumnModel.AddColumn(SettingsFields.StartYear.ToString(), AnnualBalanceHelpers.SettingsFieldsNameMapping(SettingsFields.StartYear), DataAttributeTypes.dtInteger)
               .SetNullable().SetEditableInteger().SetWidth(500).SetMinLengthEdior(4).SetMaxLengthEdior(4);

            gp.ColumnModel.AddColumn(SettingsFields.EndYear.ToString(), AnnualBalanceHelpers.SettingsFieldsNameMapping(SettingsFields.EndYear), DataAttributeTypes.dtInteger)
               .SetNullable().SetEditableInteger().SetWidth(500).SetMinLengthEdior(4).SetMaxLengthEdior(4);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new ListFilter
                                    {
                                        DataIndex = SettingsFields.RefPartDocName.ToString(),
                                        Options = commonDataService.GetPartDocList().Where(
                                            x => (x.ID == FX_FX_PartDoc.AnnualBalanceF0503130Type)
                                                 || (x.ID == FX_FX_PartDoc.AnnualBalanceF0503121Type)
                                                 || (x.ID == FX_FX_PartDoc.AnnualBalanceF0503127Type)
                                                 || (x.ID == FX_FX_PartDoc.AnnualBalanceF0503137Type)
                                                 || (x.ID == FX_FX_PartDoc.AnnualBalanceF0503721Type)
                                                 || (x.ID == FX_FX_PartDoc.AnnualBalanceF0503737Type)
                                                 || (x.ID == FX_FX_PartDoc.AnnualBalanceF0503730Type)).Select(x => x.Name).ToArray()
                                    },
                                new StringFilter { DataIndex = SettingsFields.SectionName.ToString() },
                                new StringFilter { DataIndex = SettingsFields.RefIndicatorsName.ToString() },
                                new StringFilter { DataIndex = SettingsFields.Additional.ToString() },
                                new NumericFilter { DataIndex = SettingsFields.StartYear.ToString() },
                                new NumericFilter { DataIndex = SettingsFields.EndYear.ToString() },
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private void AddLineCodeColumn(GridPanel gp, string fieldID, string fieldName)
        {
            gp.ColumnModel.AddColumn(fieldID, fieldName, DataAttributeTypes.dtString)
                .SetWidth(20).Editor.Add(
                    new TextArea
                    {
                        AllowBlank = false,
                        MaxLength = 6,
                        MaskRe = @"[0-9\.]",
                        Regex = @"^\d{3}?(\.\d{1,2})?$",
                        RegexText =
                            @"Значение «{0}» должно состоять либо из 3 цифр,
                                  либо из 3х цифр и 1й или 2х после точки.<br>
                                  Пример: 123 или 123.1 или 123.12".FormatWith(fieldName)
                    });
        }
    }
}
