using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls.SearchCombobox;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Services.SmetaService;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class SmetaView : View
    {
        private const string SmetaStoreID = "dsSmetaDetail";
        private const string SmetaGridPanelID = "gpSmetaDetail";

        private const string FundsID = "Funds";
        private const string FundsOneYearID = "FundsOneYear";
        private const string FundsTwoYearID = "FundsTwoYear";

        private const string Scope = "E86n.View.SmetaView";

        private readonly IAuthService auth;
        private readonly INewRestService newRestService;
        
        public SmetaView(IAuthService auth, INewRestService newRestService)
        {
            this.auth = auth;
            this.newRestService = newRestService;
        }

        public ViewPage Page { get; set; }

        public int? DocId
        {
            get { return (Params["docId"] == "null") ? null : (int?)Convert.ToInt32(Params["docId"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;
            
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("CodeMaskBuilder", Resource.CodeMaskBuilder);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("SmetaView", Resource.SmetaView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            RestActions restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            Toolbar tb =
                new NewStateToolBarControl(DocId ?? 0).BuildComponent(Page);

            tb.Add(new VersioningControl(Convert.ToInt32(DocId.ToString()), "Smeta", Scope + ".SetReadOnlySmeta").Build(page));

            if (auth.IsAdmin())
            {
                var export = new UpLoadFileBtnControl
                {
                    Id = "btnExport",
                    Name = "Экспорт в XML",
                    Icon = Icon.DiskDownload,
                    Upload = false,
                    UploadController = "/Smeta/ExportToXml",
                    Params = { { "recId", DocId.ToString() } }
                };

                tb.Add(export.Build(Page));

                tb.Add(new ToolbarSeparator());

                tb.Add(new SetDocStateBtn(Convert.ToInt32(DocId.ToString())).Build(Page));
            }

            var paramDocPanel = new ParamDocPanelControl(DocId ?? -1, tb);
            paramDocPanel.ParamDocStore.Listeners.Load.AddAfter(Scope + ".docStoreLoad(store);");

            Store smetaStore = GetStore();
            page.Controls.Add(smetaStore);
            
            var details = new List<Component>
                              {
                                GetDetailUi(smetaStore),
                                new DocsDetailControl(DocId ?? -1).BuildComponent(page)
                              };

            var view =
                new Viewport
                    {
                        Items =
                            {
                                new BorderLayout
                                    {
                                        North = { Items = { paramDocPanel.BuildComponent(page) } },
                                        Center = { Items = { UiBuilders.GetTabbedDetails(details) } }
                                    }
                            }
                    };

            return new List<Component> { view };
        }

        public Store GetStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(SmetaStoreID, true, "Smeta", "Index", "Save", "Save");
            store.SetBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("parentId", DocId.ToString(), ParameterMode.Raw);
            SmetaHelpers.ExportMetadataTo((JsonReader)store.Reader.Reader);
            return store;
        }

        private Component GetDetailUi(Store store)
        {
            GridPanel gridPanel = UiBuilders.CreateGridPanel(SmetaGridPanelID, store);
            gridPanel.Title = @"Бюджетные обязательства учреждения";

            gridPanel.AddRefreshButton();
            gridPanel.AddNewRecordNoEditButton();
            gridPanel.AddRemoveRecordButton();
            gridPanel.AddSaveButton();

            var referencerInfo = ComboboxEditorExtensions.BuildReferencerInfo(
                                    Resolver.Get<IScheme>().RootPackage.FindEntityByName(D_Fin_nsiBudget.Key),
                                    new[] { "ID", "Code" },
                                    new[] { "Name" });

            gridPanel.ColumnModel.AddColumn("RefBudgetIDName", "Бюджет", DataAttributeTypes.dtString)
                .SetWidth(220)
                .AddLookupEditorForColumnHardCode1(
                    "RefBudgetIDName",
                    new Dictionary<string, string[]>
                        {
                            { "RefBudgetID", new[] { "ID" } },
                            { "RefBudgetIDName", new[] { "Code", "Name" } }
                        },
                    referencerInfo,
                    "/EntityExt/DataWithCustomSearch?objectKey={0}".FormatWith(D_Fin_nsiBudget.Key),
                    Page);

            gridPanel.ColumnModel.AddColumn("RefRazdPodrIDName", "Раздел / Подраздел", DataAttributeTypes.dtInteger)
                .SetHbLookup(D_Fin_RazdPodr.Key).SetWidth(120);

            var yearForm = newRestService.GetItem<F_F_ParameterDoc>(DocId).RefYearForm.ID;

            gridPanel.ColumnModel.Columns.Add(
                new Column
                    {
                        ColumnID = "CelStatya",
                        DataIndex = "CelStatya",
                        Header = "Целевая статья расходов",
                        Width = 100,
                        Editor =
                            {
                                new TextField
                                    {
                                        AllowBlank = false, MaxLength = yearForm < 2016 ? 7 : 10, MaskRe = @"[0-9a-zA-Zа-яА-Я]" 
                                    }
                            },
                        Renderer =
                            {
                                Handler =
                                    "return buildMask(value, '{0}');".FormatWith(
                                        yearForm < 2016
                                        ? "###.##.##"
                                        : "##.#.##.#####")
                            }
                    });

            gridPanel.ColumnModel.AddColumn("RefVidRashIDName", "Вид расходов", DataAttributeTypes.dtInteger)
                .SetHbLookup(D_Fin_VidRash.Key, "{0}.getVidRashFilter('{1}')".FormatWith(Scope, DocId)).SetWidth(100);

            gridPanel.ColumnModel.AddColumn("RefKosgyIDName", "КОСГУ", DataAttributeTypes.dtInteger).SetHbLookup(D_KOSGY_KOSGY.Key).SetWidth(200);
            
            gridPanel.ColumnModel.Columns.Add(
                new Column
                    {
                        ColumnID = "Event",
                        DataIndex = "Event",
                        Header = "Мероприятие",
                        Width = 100,
                        Editor =
                            {
                                new TextField
                                    {
                                        MaxLength = 6, MaskRe = @"[0-9]" 
                                    }
                            },
                        Renderer =
                            {
                                Handler =
                                    "return buildMask(value, '{0}');".FormatWith(
                                        CUtils.GetMaskByTableField(
                                            F_Fin_Smeta.Key,
                                            "Event"))
                            }
                    });

            gridPanel.ColumnModel.AddColumn(FundsID, "Очередной финансовый год,руб", DataAttributeTypes.dtDouble)
                .SetWidth(100).Editor.Add(
                    new NumberField
                        {
                            AllowBlank = false,
                            AllowDecimals = true,
                            DecimalPrecision = 2,
                            DecimalSeparator = ","
                        });

            gridPanel.ColumnModel.AddColumn(
                FundsOneYearID,
                "Первый год планового периода,руб",
                DataAttributeTypes.dtDouble).SetWidth(100).Editor.Add(
                    new NumberField
                        {
                            AllowBlank =
                                false,
                            AllowDecimals
                                = true,
                            DecimalPrecision
                                = 2,
                            DecimalSeparator
                                = ","
                        });

            gridPanel.ColumnModel.AddColumn(
                FundsTwoYearID,
                "Второй год планового периода,руб",
                DataAttributeTypes.dtDouble).SetWidth(100).Editor.Add(
                    new NumberField
                        {
                            AllowBlank =
                                false,
                            AllowDecimals
                                = true,
                            DecimalPrecision
                                = 2,
                            DecimalSeparator
                                = ","
                        });

            gridPanel.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "RefBudgetIDName" },
                                new StringFilter { DataIndex = "RefKosgyIDName" },
                                new StringFilter { DataIndex = "RefRazdPodrIDCode" },
                                new StringFilter { DataIndex = "RefVidRashIDCode" },
                                new StringFilter { DataIndex = "CelStatya" },
                                new NumericFilter { DataIndex = FundsID },
                                new NumericFilter { DataIndex = FundsOneYearID },
                                new NumericFilter { DataIndex = FundsTwoYearID },
                            }
                    });

            gridPanel.AddColumnsWrapStylesToPage(Page);

            var summ1 = new SummFieldControl("Summ1", store, FundsID, "Cумма очередной год");
            var summ2 = new SummFieldControl("Summ2", store, FundsOneYearID, "Cумма первый год");
            var summ3 = new SummFieldControl("Summ3", store, FundsTwoYearID, "Cумма второй год");

            gridPanel.BottomBar.Add(
                new Toolbar
                    {
                        Items =
                            {
                                summ1.BuildComponent(Page),
                                summ2.BuildComponent(Page),
                                summ3.BuildComponent(Page),
                            }
                    });
            return gridPanel;
        }
    }
}