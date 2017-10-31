using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Passport;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls.SearchCombobox;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public sealed class PassportView : View
    {
        private const string FormID = "InstitutionInfo";
        private const string BranchesGridID = "Branches";
        private const string TypesOfActivitiesGridID = "TypesOfActivities";
        private const string FoundersGridID = "Founders";

        private const string ScopePassport = "E86n.View.PassportView";

        private readonly IAuthService auth;

        public PassportView(IAuthService auth)
        {
            this.auth = auth;
        }

        [Dependency]
        public INewRestService NewRestService { get; set; }

        private int? DocId
        {
            get { return Params["docId"] == "null" ? -1 : Convert.ToInt32(Params["docId"]); }
        }

        private ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;
           
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("PassportView", Resource.PassportView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            page.Controls.Add(GetInstitutionInfoStore());
            
            var ds = CUtils.GetLookupStore("dsRefTipFi", "/CommonData/GetTipFilList");
            Page.Controls.Add(ds);

            ds = CUtils.GetLookupStore("dsRefPrOkved", "/CommonData/GetPrOkvedList");
            Page.Controls.Add(ds);

            var branchesStore = GetBranchesStore();
            Page.Controls.Add(branchesStore);

            var typesOfActivitiesStore = GetTypesOfActivitiesStore();
            Page.Controls.Add(typesOfActivitiesStore);

            var docsDetail = (Panel)new DocsDetailControl(DocId ?? -1).BuildComponent(page);
            docsDetail.Title = @"Документы основания*";

            var details = new List<Component>
                              {
                                  GetInstitutionInfoForm(),
                                  GetFoundersGrid(),
                                  GetBranchesGrid(branchesStore),
                                  GetTypesOfActivitiesGrid(typesOfActivitiesStore),
                                  docsDetail
                              };

            var view = new Viewport
                           {
                               Items =
                                   {
                                       new BorderLayout
                                           {
                                               North =
                                                   {
                                                       Items =
                                                           {
                                                               new Panel
                                                                   {
                                                                       TopBar = { CreateToolbar(2) },
                                                                       Height = 27,
                                                                       Border = false
                                                                   }
                                                           }
                                                   },
                                               Center = { Items = { UiBuilders.GetTabbedDetails(details) } }
                                           }
                                   }
                           };

            return new List<Component> { view };
        }

        private Store GetInstitutionInfoStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(FormID + "Store", true, "Passport", "Load");
            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);

            store.Listeners.DataChanged.Fn(ScopePassport, "StoreDataChanged");

            store.AddField("ID");
            store.AddField("RefOrgPpoName");
            store.AddField("RefTipYcName");
            store.AddField("RefOrgGrbsName");
            store.AddField("Name");
            store.AddField("ShortName");
            store.AddField("Inn");
            store.AddField("Kpp");

            store.AddField("RefRaspor");
            store.AddField("RefRasporName");
            store.AddField("Ogrn");
            store.AddField("RefCatYh");
            store.AddField("RefCatYhName");
            store.AddField("RefVid");
            store.AddField("RefVidName");
            store.AddField("RefOkato");
            store.AddField("RefOkatoName");
            store.AddField("RefOkfs");
            store.AddField("RefOkfsName");
            store.AddField("RefOktmo");
            store.AddField("RefOktmoName");
            store.AddField("RefOkopf");
            store.AddField("RefOkopfName");
            store.AddField("Okpo");
            store.AddField("Adr");
            store.AddField("Website");
            store.AddField("Phone");
            store.AddField("Mail");
            store.AddField("Fam");
            store.AddField("NameRuc");
            store.AddField("Otch");
            store.AddField("Ordinary");
            store.AddField("Indeks");
            store.AddField("RefSost");
            store.AddField("RefSostName");
            store.AddField("OpeningDate");
            store.AddField("CloseDate");

            store.Listeners.Load.Handler = ScopePassport + ".Load({0}, records);".FormatWith(DocId);

            return store;
        }

        private Store GetBranchesStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                BranchesGridID + "Store", false, "Branches", createActionName: "Save", updateActionName: "Save");

            store.AddField("ID");
            store.AddField("RefTipFi");
            store.AddField("RefTipFiName");
            store.AddField("Code");
            store.AddField("Name");
            store.AddField("Nameshot");
            store.AddField("INN");
            store.AddField("KPP");

            store.SetBaseParams("recId", "InstitutionInfoStore.getAt(0).data.ID", ParameterMode.Raw);
            store.SetWriteBaseParams("recId", "InstitutionInfoStore.getAt(0).data.ID", ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Raw);

            return store;
        }

        private Store GetTypesOfActivitiesStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                TypesOfActivitiesGridID + "Store",
                false,
                "TypesOfActivities");
            store.AddField("ID");
            store.AddField("Name");
            store.AddField("RefOkved");
            store.AddField("RefOkvedName");
            store.AddField("RefPrOkved");
            store.AddField("RefPrOkvedName");

            store.SetBaseParams("recId", "InstitutionInfoStore.getAt(0).data.ID", ParameterMode.Raw);
            store.SetWriteBaseParams("recId", "InstitutionInfoStore.getAt(0).data.ID", ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Raw);

            return store;
        }

        private Store GetFounderStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(FoundersGridID + "Store", false, "Founders", createActionName: "Save", updateActionName: "Save");
            store.AddField("ID");
            store.AddField("RefPassport");
            store.AddField("RefYchred");
            store.AddField("RefYchredName");
            store.AddField("formative", new RecordField.Config { DefaultValue = "false" });
            store.AddField("stateTask", new RecordField.Config { DefaultValue = "false" });
            store.AddField("supervisoryBoard", new RecordField.Config { DefaultValue = "false" });
            store.AddField("manageProperty", new RecordField.Config { DefaultValue = "false" });
            store.AddField("financeSupply", new RecordField.Config { DefaultValue = "false" });

            store.SetBaseParams("parentID", "InstitutionInfoStore.getAt(0).data.ID", ParameterMode.Raw);
            store.SetWriteBaseParams("parentID", "InstitutionInfoStore.getAt(0).data.ID", ParameterMode.Raw);

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Raw);

            Page.Controls.Add(store);

            return store;
        }

        private Toolbar CreateToolbar(int index)
        {
            var toolbar = new Toolbar();
            switch (index)
            {
                case 1:
                    toolbar.Add(
                        new Button
                            {
                                ID = "btnRefresh",
                                Icon = Icon.PageRefresh,
                                ToolTip = @"Обновить",
                                Listeners =
                                    {
                                        Click =
                                            {
                                                Handler = "InstitutionInfoStore.reload();"
                                            }
                                    }
                            });

                    toolbar.Add(
                        new Button
                            {
                                ID = "btnSave",
                                Icon = Icon.TableSave,
                                ToolTip = @"Сохранить",
                                Listeners =
                                    {
                                        Click =
                                            {
                                                Handler = ScopePassport + ".SaveForm(0, {0})".FormatWith(DocId)
                                            }
                                    }
                            });
                    break;

                case 2:

                    if (DocId.HasValue)
                    {
                        toolbar = new NewStateToolBarControl(DocId.Value).BuildComponent(Page);
                        toolbar.Add(new ToolbarSeparator());
                    }

                    if (auth.IsAdmin())
                    {
                        var export = new UpLoadFileBtnControl
                        {
                            Id = "btnExport",
                            Name = "Экспорт в XML",
                            Icon = Icon.DiskDownload,
                            Upload = false,
                            UploadController = "/OGS/Export",
                            Params = { { "RecId", DocId.ToString() } }
                        };
                       
                        toolbar.Add(export.Build(Page));
                        toolbar.Add(new ToolbarSeparator());
                    }

                    toolbar.Add(new VersioningControl(Convert.ToInt32(DocId.ToString()), "Passport", ScopePassport + ".SetReadOnlyPassport").Build(Page));

                    toolbar.Add(new ToolbarSeparator());

                    toolbar.Add(new SetDocStateBtn(Convert.ToInt32(DocId.ToString())).Build(Page));

                    break;
            }

            return toolbar;
        }

        private Component GetInstitutionInfoForm()
        {
            var form = new FormPanel
                {
                    ID = FormID,
                    Border = false,
                    Url = "/Passport/Save",
                    BaseParams =
                        {
                            new Parameter("docId", DocId.ToString())
                        },
                    AutoScroll = true,
                    LabelWidth = 250,
                    Padding = 6,
                    DefaultAnchor = "95%",
                    MonitorValid = true
                };

            form.Items.Add(new Hidden { ID = "ID", DataIndex = "ID" });

            form.Items.Add(new Hidden { ID = "RefSostID", DataIndex = "RefSost" });

            form.Items.Add(
                new TextField
                    {
                        ID = "RefOrgPpoName",
                        FieldLabel = @"Публично-правовое образование, создавшее учреждение*",
                        DataIndex = "RefOrgPpoName",
                        ReadOnly = true
                    });

            form.Items.Add(
                new TextField
                    {
                        ID = "RefTipYcName",
                        FieldLabel = @"Тип учреждения",
                        DataIndex = "RefTipYcName",
                        ReadOnly = true
                    });

            form.Items.Add(
                new TextField
                    {
                        ID = "RefOrgGrbsName",
                        FieldLabel = @"ГРБС*",
                        DataIndex = "RefOrgGrbsName",
                        ReadOnly = true
                    });

            form.Items.Add(
                new TextField
                    {
                        ID = "Name",
                        FieldLabel = @"Полное наименование",
                        DataIndex = "Name",
                        ReadOnly = true
                    });

            form.Items.Add(
                new TextField
                    {
                        ID = "ShortName",
                        FieldLabel = @"Сокращенное наименование",
                        DataIndex = "ShortName",
                        ReadOnly = true
                    });

            form.Items.Add(
                new NumberField
                    {
                        ID = "Inn",
                        FieldLabel = @"ИНН",
                        DataIndex = "Inn",
                        ReadOnly = true,
                        MaxLength = 10
                    });

            form.Items.Add(
                new NumberField
                    {
                        ID = "Kpp",
                        FieldLabel = @"КПП",
                        DataIndex = "Kpp",
                        ReadOnly = true,
                        MaxLength = 9
                    });

            // Распорядител временно убрать
            Component fld = CUtils.LookUpFld("RefRasporName", "Распорядитель", "RefRasporName", D_OrgGen_Raspor.Key, true);
            fld.Hidden = true;
            form.Items.Add(fld);
            form.Items.Add(new TextField { ID = "RefRaspor", Hidden = true, DataIndex = "RefRaspor" });

            // Распорядител временно убрать
            form.Items.Add(
                new NumberField
                    {
                        ID = "Ogrn",
                        FieldLabel = @"ОГРН*",
                        DataIndex = "Ogrn",
                        AllowBlank = false,
                        MaxLength = 13
                    });

            form.Items.Add(
                new DBComboBox
                    {
                        ID = "comboRefCatYh",
                        DataIndex = "RefCatYhName",
                        LoadController = UiBuilders.GetUrl<CommonDataController>("GetOrgCategoryList"),
                        FieldLabel = "Категория учреждения"
                    }.Build(Page));

            form.Items.Add(new TextField { ID = "RefVid", Hidden = true, DataIndex = "RefVid" });
            var comboRefVidName = new DBComboBox
                {
                    ID = "comboRefVidName",
                    DataIndex = "RefVidName",
                    LoadController = UiBuilders.GetUrl<PassportController>("InstitutionSorts"),
                    FieldLabel = "Вид учреждения*",
                    Mapping = new Dictionary<string, string[]>
                        {
                            { "RefVid", new[] { "ID" } },
                            { "RefVidName", new[] { "Name", "Code" } }
                        },
                    DisplayField = "RefVidName",
                    Editable = true,
                    Key = D_Org_VidOrg.Key
                };
            comboRefVidName.Box.Listeners.Select.Fn(ScopePassport, "SelectInstitutionSorts");
            form.Items.Add(comboRefVidName.Build(Page));

            form.Items.Add(
                new TextField
                    {
                        ID = "OpeningDate",
                        DataIndex = "OpeningDate",
                        FieldLabel = @"Дата открытия",
                        ReadOnly = true,
                        Disabled = true
                    });

            form.Items.Add(
                new TextField
                    {
                        ID = "CloseDate",
                        DataIndex = "CloseDate",
                        FieldLabel = @"Дата закрытия",
                        ReadOnly = true,
                        Disabled = true
                    });

            form.Items.Add(
                new TextField
                    {
                        ID = "RefSostName",
                        DataIndex = "RefSostName",
                        FieldLabel = @"Состояние документа:",
                        ReadOnly = true,
                        AutoWidth = true
                    });

            var group1 = new FieldSet
                {
                    Title = @"Классификаторы",
                    Collapsible = true,
                    Collapsed = true,
                    AutoHeight = true,
                    Border = false,
                    StyleSpec =
                        "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                    Layout = LayoutType.Form.ToString(),
                    FormGroup = true,
                    LabelWidth = 250,
                    LabelSeparator = string.Empty,
                    LabelPad = 10,
                    Padding = 6,
                    DefaultAnchor = "0"
                };
            form.Items.Add(group1);

            group1.Items.Add(
                new DBComboBox
                    {
                        Mapping = new Dictionary<string, string[]>
                            {
                                { "RefOkato", new[] { "ID" } },
                                { "Okato", new[] { "Code", "Name" } }
                            },
                        DataIndex = "RefOkatoName",
                        DisplayField = "Okato",
                        ID = "RefOkatoName",
                        Key = D_OKATO_OKATO.Key,
                        Editable = true,
                        FieldLabel = "Код по ОКАТО (по месту регистрации учреждения)*"
                    }
                    
                    .Build(Page));

            group1.Items.Add(
                new TextField
                    {
                        ID = "RefOkato",
                        Hidden = true,
                        DataIndex = "RefOkato"
                    });

            group1.Items.Add(CUtils.LookUpFld("RefOkfsName", "Код по ОКФС*", "RefOkfsName", D_OKFS_OKFS.Key));
            group1.Items.Add(
                new TextField
                    {
                        ID = "RefOkfs",
                        Hidden = true,
                        DataIndex = "RefOkfs"
                    });

            group1.Items.Add(
                new DBComboBox
                    {
                        Mapping = new Dictionary<string, string[]>
                            {
                                { "RefOktmo", new[] { "ID" } },
                                { "Oktmo", new[] { "Code", "Name" } }
                            },
                        DataIndex = "RefOktmoName",
                        DisplayField = "Oktmo",
                        ID = "RefOktmoName",
                        Key = D_OKTMO_OKTMO.Key,
                        Editable = true,
                        FieldLabel = "Код по ОКТМО (по месту регистрации учреждения)*"
                    }
                    
                    .Build(Page));

            group1.Items.Add(
                new TextField
                    {
                        ID = "RefOktmo",
                        Hidden = true,
                        DataIndex = "RefOktmo"
                    });

            group1.Items.Add(
                CUtils.LookUpFld(
                    "RefOkopfName",
                    "Код по ОКОПФ*",
                    "RefOkopfName",
                    D_OKOPF_OKOPF.Key,
                    allowblank: true,
                    filter: "'((businessStatus=801) AND (Code5Zn IN (75201, 75203, 75204, 75401, 75403, 75404)))'"));
            group1.Items.Add(
                new TextField
                    {
                        ID = "RefOkopf",
                        Hidden = true,
                        DataIndex = "RefOkopf",
                        AllowBlank = true
                    });

            group1.Items.Add(
                new TextField
                    {
                        ID = "Okpo",
                        FieldLabel = @"Код по ОКПО*",
                        DataIndex = "Okpo",
                        AllowBlank = false,
                        MaxLength = 10,
                        Regex = @"^\d{8}$|^\d{10}$",
                        InvalidText = @"Неверный формат ОКПО",
                        MaskRe = @"[0-9]"
                    });

            var group2 = new FieldSet
                {
                    Title = @"Адресные данные",
                    Collapsible = true,
                    Collapsed = true,
                    AutoHeight = true,
                    Border = false,
                    StyleSpec =
                        "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                    Layout = LayoutType.Form.ToString(),
                    FormGroup = true,
                    LabelWidth = 250,
                    LabelSeparator = string.Empty,
                    LabelPad = 10,
                    Padding = 6,
                    DefaultAnchor = "0"
                };
            form.Items.Add(group2);

            group2.Items.Add(
                new NumberField
                    {
                        ID = "Indeks",
                        FieldLabel = @"Индекс*",
                        DataIndex = "Indeks",
                        AllowBlank = false,
                        MaxLength = 6
                    });

            group2.Items.Add(
                new TextField
                    {
                        ID = "Adr",
                        FieldLabel = @"Адрес местонахождения",
                        DataIndex = "Adr",
                        AllowBlank = false
                    });

            group2.Items.Add(
                new TextField
                    {
                        ID = "Website",
                        FieldLabel = @"Сайт учреждения (при наличии)*",
                        DataIndex = "Website"
                    });

            group2.Items.Add(
                new TextField
                    {
                        ID = "Phone",
                        MaskRe = @"[0-9\-]",
                        Regex = @"^8-\d{4}-\d{6}$",
                        InvalidText =
                            @"Контактный телефон должен иметь формат 8-ХХХХ-ХХХХХХ",
                        FieldLabel = @"Контактный телефон*",
                        DataIndex = "Phone",
                        AllowBlank = false,
                        MaxLength = 13
                    });

            group2.Items.Add(
                new TextField
                    {
                        ID = "Mail",
                        FieldLabel = @"Адрес электронной почты (при наличии)*",
                        DataIndex = "Mail"
                    });

            var group3 = new FieldSet
                {
                    Title = @"Руководитель учреждения",
                    Collapsible = true,
                    Collapsed = true,
                    AutoHeight = true,
                    Border = false,
                    StyleSpec =
                        "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                    Layout = LayoutType.Form.ToString(),
                    FormGroup = true,
                    LabelWidth = 250,
                    LabelSeparator = string.Empty,
                    LabelPad = 10,
                    Padding = 6,
                    DefaultAnchor = "0"
                };
            form.Items.Add(group3);

            group3.Items.Add(
                new TextField
                    {
                        ID = "Fam",
                        FieldLabel = @"Фамилия",
                        DataIndex = "Fam",
                        AllowBlank = false
                    });

            group3.Items.Add(
                new TextField
                    {
                        ID = "NameRuc",
                        FieldLabel = @"Имя",
                        DataIndex = "NameRuc",
                        AllowBlank = false
                    });

            group3.Items.Add(
                new TextField
                    {
                        ID = "Otch",
                        FieldLabel = @"Отчество",
                        DataIndex = "Otch",
                        AllowBlank = false
                    });

            group3.Items.Add(
                new TextField
                    {
                        ID = "Ordinary",
                        FieldLabel = @"Должность",
                        DataIndex = "Ordinary",
                        AllowBlank = false
                    });

            var panel = new Panel
                {
                    Title = @"Информация по учреждению",
                    Border = false,
                    AutoScroll = true,
                    TopBar =
                        {
                            CreateToolbar(1)
                        },
                    Items =
                        {
                            form
                        },
                    Icon = Icon.DatabaseTable
                };

            return panel;
        }

        private GridPanel GetBranchesGrid(Store store)
        {
            GridPanel gp = UiBuilders.CreateGridPanel(BranchesGridID, store);
            gp.Title = @"Филиалы и представительства*";
            gp.Disabled = true;

            gp.AddRefreshButton();
            gp.AddNewRecordButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners
                .RowSelect.Handler = ScopePassport + ".RowSelect('{0}')".FormatWith(BranchesGridID);

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners
                .RowDeselect.Handler = ScopePassport + ".RowDeselect('{0}')".FormatWith(BranchesGridID);

            gp.ColumnModel.AddColumn("RefTipFiName", "Тип филиала", DataAttributeTypes.dtString).SetWidth(150)
                .SetLookup("RefTipFi", "dsRefTipFi", "ID", "Name");

            gp.ColumnModel.AddColumn("Code", "Код подразделения", DataAttributeTypes.dtString).SetNullable()
                .SetEditableInteger().SetWidth(100).SetMaxLengthEdior(4);

            gp.ColumnModel.AddColumn("Name", "Полное наименование", DataAttributeTypes.dtString).SetNullable()
                .SetEditableString().SetWidth(200);

            gp.ColumnModel.AddColumn("Nameshot", "Сокращенное наименование учреждения", DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(200);

            gp.ColumnModel.AddColumn("INN", "ИНН", DataAttributeTypes.dtString).SetNullable().SetEditableInteger()
                .SetWidth(100).SetMaxLengthEdior(10);

            gp.ColumnModel.AddColumn("KPP", "КПП", DataAttributeTypes.dtString).SetNullable().SetEditableInteger()
                .SetWidth(100).SetMaxLengthEdior(9);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "RefTipFiName" },
                                new StringFilter { DataIndex = "Code" },
                                new StringFilter { DataIndex = "Name" },
                                new StringFilter { DataIndex = "Nameshot" },
                                new NumericFilter { DataIndex = "INN" },
                                new NumericFilter { DataIndex = "KPP" }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetTypesOfActivitiesGrid(Store store)
        {
            GridPanel gp = UiBuilders.CreateGridPanel(TypesOfActivitiesGridID, store);
            gp.Title = @"Виды деятельности*";
            gp.Disabled = true;

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners
                .RowSelect.Handler = ScopePassport + ".RowSelect('{0}')".FormatWith(BranchesGridID);

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners
                .RowDeselect.Handler = ScopePassport + ".RowDeselect('{0}')".FormatWith(BranchesGridID);

            var referencerInfo = ComboboxEditorExtensions.BuildReferencerInfo(
                                    Resolver.Get<IScheme>().RootPackage.FindEntityByName(D_OKVED_OKVED.Key),
                                    new[] { "ID", "Code" },
                                    new[] { "Name" });

            var url = UiBuilders.GetUrl<CommonDataController>(
                "GetOkvedListExt",
                new Dictionary<string, object>
                    {
                        { "dateBegin", NewRestService.GetItem<F_F_ParameterDoc>(DocId).OpeningDate },
                        { "dateEnd", NewRestService.GetItem<F_F_ParameterDoc>(DocId).CloseDate }
                    });

            gp.ColumnModel.AddColumn("RefOkvedName", "ОКВЭД", DataAttributeTypes.dtString)
                .SetWidth(350)
                .AddLookupEditorForColumnHardCode1(
                    "RefOkvedName",
                    new Dictionary<string, string[]>
                        {
                            { "RefOkved", new[] { "ID" } },
                            { "RefOkvedName", new[] { "Code", "Name" } }
                        },
                    referencerInfo,
                    url,
                    Page);

            gp.ColumnModel.AddColumn("RefPrOkvedName", "Признак вида деятельности", DataAttributeTypes.dtString)
                .SetWidth(200).SetLookup("RefPrOkved", "dsRefPrOkved", "ID", "Name");

            gp.ColumnModel.AddColumn("Name", "Наименование вида деятельности (по уставу)", DataAttributeTypes.dtString)
               .SetEditableString().SetWidth(200).SetMaxLengthEdior(2000);

            gp.AddRefreshButton();
            if (!auth.IsAdmin())
            {
                gp.SetReadonly();
            }
            else
            {
                gp.AddNewRecordNoEditButton();
                gp.AddRemoveRecordButton();
                gp.AddSaveButton();
            }

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "RefOkvedName" },
                                new StringFilter { DataIndex = "RefPrOkvedName" },
                                new StringFilter { DataIndex = "Name" }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private GridPanel GetFoundersGrid()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(FoundersGridID, GetFounderStore());
            gp.Title = @"Учредители";
            gp.Disabled = true;

            bool editble;

            if (auth.IsAdmin())
            {
                editble = true;
            }
            else
            {
                if (ConfigurationManager.AppSettings["FoundersEdit"] == null ||
                    ConfigurationManager.AppSettings["FoundersEdit"] == "true")
                {
                    editble = true;
                }
                else
                {
                    editble = false;
                }
            }

            gp.AddRefreshButton();

            if (editble)
            {
                gp.AddNewRecordNoEditButton();
                gp.AddDeleteRecordWithConfirmButton();
                gp.AddSaveButton();
            }

            var headerGroupRow = new HeaderGroupRow();
            headerGroupRow.Columns.Add(new HeaderGroupColumn { ColSpan = 2 });
            headerGroupRow.Columns.Add(
                new HeaderGroupColumn
                    {
                        Header = "Полномочия учредителя <br> Решение о",
                        ColSpan = 5,
                        Align = Alignment.Center
                    });

            gp.ColumnModel.AddColumn("RefYchredName", "Наименование учредителя", DataAttributeTypes.dtString)
                .SetWidth(200)
                .AddLookupEditorForColumn(
                    "RefYchred",
                    "RefYchredName",
                    "/Entity/DataWithCustomSearch?objectKey={0}".FormatWith(D_Org_OrgYchr.Key),
                    true,
                    Page);

            gp.ColumnModel.AddColumn(
                "formative",
                "создании, реорганизации, ликвидации, изменении типа учреждения (его филиалов)",
                DataAttributeTypes.dtBoolean).SetEditableBoolean().SetWidth(150);

            gp.ColumnModel.AddColumn(
                "stateTask",
                "формировании и утверждении государственного (муниципального) задания",
                DataAttributeTypes.dtBoolean).SetEditableBoolean().SetWidth(150);

            gp.ColumnModel.AddColumn(
                "supervisoryBoard",
                "назначении членов наблюдательного совета",
                DataAttributeTypes.dtBoolean).SetEditableBoolean().SetWidth(150);

            gp.ColumnModel.AddColumn(
                "manageProperty",
                "управлении имуществом организации",
                DataAttributeTypes.dtBoolean).SetEditableBoolean().SetWidth(150);

            gp.ColumnModel.AddColumn(
                "financeSupply",
                "финансовом обеспечении организации",
                DataAttributeTypes.dtBoolean).SetEditableBoolean().SetWidth(150);

            gp.View[0].HeaderGroupRows.Add(headerGroupRow);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true,
                        Filters =
                            {
                                new NumericFilter { DataIndex = "ID" },
                                new StringFilter { DataIndex = "RefYchredName" }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);

            gp.SetReadonly(!editble);

            return gp;
        }
    }
}
