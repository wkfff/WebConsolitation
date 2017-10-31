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
using Krista.FM.RIA.Extensions.E86N.Models.StartDocModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public sealed class StartDocView : View
    {
        #region Setting

        private const string InstitutionsStoreID = "StartDocInstitutionsStore";

        private const string InstitutionsGridID = "StartDocInstitutionsGrid";

        private const string DocumentStoreID = "StartDocDocumentStore";

        private const string DocumentGridID = "StartDocDocumentGrid";

        private const string Scope = "E86n.View.StartDoc.Grid";

        private readonly StartDocDocumentViewModel documentModel = new StartDocDocumentViewModel();

        private readonly StartDocInstitutionsViewModel institutionsModel = new StartDocInstitutionsViewModel();

        #endregion

        private readonly IAuthService auth;

        public StartDocView(IAuthService auth)
        {
            this.auth = auth;
        }

        public ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StartDocView.js", Resource.StartDocView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StateToolBarControl.js", Resource.StateToolBarControl);

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            GetPageControls();

            var view = new Viewport
                {
                    Items = 
                        {
                            GetRowLayout()
                        }
                };

            return new List<Component> { view };
        }

        private void GetPageControls()
        {
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusEdit", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit), 
                        InvalidText = @"На редактировании"
                    });
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusFix", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick), 
                        InvalidText = @"Заблокирован"
                    });
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusAdd", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserAdd), 
                        InvalidText = @"Создан"
                    });
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusRasm", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserCross), 
                        InvalidText = @"На рассмотрении"
                    });
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusReW", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserGo), 
                        InvalidText = @"На доработке"
                    });
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusUse", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserSuit), 
                        InvalidText = @"На выполнении"
                    });
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusEnd", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserHome), 
                        InvalidText = @"Завершено"
                    });
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusNot", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.None), 
                        InvalidText = @"Статус не указан"
                    });
            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusExported", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserGreen), 
                        InvalidText = @"Экспортировано"
                    });

            Page.Controls.Add(
                new Hidden
                    {
                        ID = "UrlIconStatusClosed", 
                        Value = ResourceManager.GetInstance().GetIconUrl(Icon.Lock), 
                        InvalidText = @"Документ закрыт"
                    });
        }

        private RowLayout GetRowLayout()
        {
            var rowLayout = new RowLayout { Split = true };
            rowLayout.Rows.Add(new LayoutRow { Items = { GetFilterPanel() } });

            if (!auth.IsAdmin() && !auth.IsGrbsUser() && !auth.IsPpoUser() && !auth.IsSpectator())
            {
                rowLayout.Rows.Add(new LayoutRow { RowHeight = 1m, Items = { GetDocumentPanel() } });
            }
            else
            {
                rowLayout.Rows.Add(new LayoutRow { RowHeight = 0.65m, Items = { GetInstitutionPanel() } });
                rowLayout.Rows.Add(new LayoutRow { RowHeight = 0.35m, Items = { GetDocumentPanel() } });
            }

            return rowLayout;
        }

        private Component GetFilterPanel()
        {
            var filterPnael = new Panel
                {
                    Title = @"Параметры фильтрации", 
                    BodyCssClass = "x-window-mc", 
                    CssClass = "x-window-mc", 
                    Border = false, 
                    Layout = "RowLayout", 
                    Padding = 6, 
                    LabelWidth = 350, 
                    LabelSeparator = string.Empty, 
                    Height = 115,
                    Collapsible = true
                };
            
            filterPnael.Items.Add(PpoFilter());
            filterPnael.Items.Add(GrbsFilter());
            filterPnael.Items.Add(YearFilter());

            return filterPnael;
        }

        private Component PpoFilter()
        {
            var ppoCb = new DBComboBox
                {
                    ID = "dbcbPpo",
                    FieldLabel = "ППО",
                    DisplayField = "Name",
                    ValueField = "ID",
                    LoadController = UiBuilders.GetUrl<StartDocController>("GetPpoInside")
                };
            ppoCb.Build(Page);

            var ppoFld = ppoCb.Box;
            ppoFld.ID = "RefOrgPPOFlt";
            ppoFld.Listeners.Select.Handler = "{0}.ChangeFilter();".FormatWith(Scope);

            if (!(auth.IsAdmin() || auth.IsSpectator()))
            {
                if (auth.Profile.RefUchr.RefOrgPPO != null)
                {
                    ppoFld.Call("setValue", auth.Profile.RefUchr.RefOrgPPO.ID);
                    ppoFld.Call("setRawValue", auth.Profile.RefUchr.RefOrgPPO.Name);
                }

                if (!auth.IsPpoUser())
                {
                    ppoFld.Disabled = true;
                }
            }

            AddClearTrigger(ppoFld);
            ppoFld.Listeners.TriggerClick.Handler += Scope + ".ChangeFilter();";

            return ppoFld;
        }

        private IEnumerable<Component> GrbsFilter()
        {
            var grbsFld = (TriggerField)CUtils.LookUpFld("RefOrgGRBSFltName", "ГРБС", string.Empty, D_Org_GRBS.Key, false, "'(REFORGPPO='+getFilter('PPO')+')'");
            var grbsIDFld = new Hidden { ID = "RefOrgGRBSFlt" };
            grbsIDFld.Listeners.Change.Handler = "{0}.ChangeFilter();".FormatWith(Scope);

            if (!(auth.IsAdmin() || auth.IsSpectator()))
            {
                if (auth.Profile.RefUchr.RefOrgGRBS != null)
                {
                    grbsFld.SetValue(auth.Profile.RefUchr.RefOrgGRBS.Name);
                    grbsIDFld.Value = auth.Profile.RefUchr.RefOrgGRBS.ID;
                }

                if (!auth.IsPpoUser())
                {
                    grbsFld.Disabled = true;
                }
            }

            return new Component[] { grbsFld, grbsIDFld };
        }

        private Component YearFilter()
        {
            var yearFormation = new DBComboBox
            {
                ID = "cbYearFormation",
                FieldLabel = "Год формирования",
                DataIndex = "YearFormation",
                DisplayField = "ID",
                ValueField = "ID",
                LoadController = UiBuilders.GetUrl<CommonDataController>("GetYearFormList")
            };
            yearFormation.Build(Page);

            var fld = yearFormation.Box;

            AddClearTrigger(fld);
            fld.Listeners.TriggerClick.Handler += Scope + ".Update();";
            
            return fld;
        }

        private void AddClearTrigger(ComboBox cb)
        {
            cb.Triggers.Add(new FieldTrigger { Icon = TriggerIcon.Clear, HideTrigger = true });
            cb.Listeners.Select.Handler = string.Concat("this.triggers[0].show(); ", Scope, ".Update();");
            cb.Listeners.BeforeQuery.Handler = "this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();";
            cb.Listeners.TriggerClick.Handler = "if (index == 0) { this.clearValue(); this.triggers[0].hide(); } ";
        }

        #region Institutions

        private Store GetInstitutionStore()
        {
            var store = StoreExtensions.StoreCreateDefault(InstitutionsStoreID, true, typeof(StartDocController), "ReadInstitutions");
            store.AddFieldsByClass(institutionsModel);

            store.SetBaseParams("ppo", "getFilter('PPO')", ParameterMode.Raw);
            store.SetBaseParams("grbs", "getFilter('GRBS')", ParameterMode.Raw);
            store.BaseParams.Add(new Parameter("limit", "pageSizeDocs.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));

            Page.Controls.Add(store);

            return store;
        }

        private Component GetInstitutionPanel()
        {
            var gp = UiBuilders.CreateGridPanel(InstitutionsGridID, GetInstitutionStore());
            gp.Title = @"Учреждения";

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Fn(Scope, "Update");
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.Fn(Scope, "Update");

            gp.AddRefreshButton();
            gp.Toolbar().Add(new ReportControl().Build(Page));
            gp.Toolbar().Add(new AnalReportControl().Build(Page));
            gp.Toolbar().Add(new NewAnalReportControl().Build(Page));

            GetInstitutionColumnModel(gp);
            
            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = false, 
                        Filters =
                            {
                                new NumericFilter { DataIndex = institutionsModel.NameOf(() => institutionsModel.ID) }, 
                                new StringFilter { DataIndex = institutionsModel.NameOf(() => institutionsModel.Name) }, 
                                new StringFilter { DataIndex = institutionsModel.NameOf(() => institutionsModel.ShortName) }, 
                                new StringFilter { DataIndex = institutionsModel.NameOf(() => institutionsModel.RefTipYcName) }
                            }
                    });

            gp.BottomBar.Add(GetInstitutionBottomToolBar());
            return gp;
        }

        private ToolbarBase GetInstitutionBottomToolBar()
        {
            var t = new PagingToolbar
                {
                    ID = "paggingDocs", 
                    StoreID = InstitutionsStoreID, 
                    PageSize = 50
                };
            t.Items.Add(
                new NumberField
                    {
                        ID = "pageSizeDocs", 
                        FieldLabel = @"Учреждений на страницу", 
                        LabelWidth = 150, 
                        Width = 200, 
                        Number = 50, 
                        Listeners = { Change = { Handler = @"#{paggingDocs}.pageSize = parseInt(this.getValue());" } }
                    });

            t.Listeners.Change.Handler = string.Concat(Scope, ".ChangePage();");
            return t;
        }

        private void GetInstitutionColumnModel(GridPanel gp)
        {
            gp.ColumnModel.AddColumn(() => institutionsModel.Name, DataAttributeTypes.dtString)
                .SetEditable(false).SetWidth(400);
            gp.ColumnModel.AddColumn(() => institutionsModel.ShortName, DataAttributeTypes.dtString)
                .SetEditable(false).SetWidth(300);
            gp.ColumnModel.AddColumn(() => institutionsModel.RefTipYcName, DataAttributeTypes.dtString)
                .SetEditable(false).SetWidth(200);

            gp.AddColumnsWrapStylesToPage(Page);
        }

        #endregion

        #region Documents

        private Store GetDocumentStore()
        {
            var store = StoreExtensions.StoreCreateDefault(DocumentStoreID, true, typeof(StartDocController), createActionName: "Save", updateActionName: "Save");
            store.AddFieldsByClass(documentModel);
            store.AddField(documentModel.NameOf(() => documentModel.PlanThreeYear), new RecordField.Config { DefaultValue = ConfigurationManager.AppSettings["Period"] });

            store.SetBaseParams("masterId", "getMasterID('{0}')".FormatWith(InstitutionsGridID), ParameterMode.Raw);
            store.SetBaseParams("yf", "getFilter('YF')", ParameterMode.Raw);
            store.SetWriteBaseParams("masterId", "getMasterID('{0}')".FormatWith(InstitutionsGridID), ParameterMode.Raw);
            store.SetWriteBaseParams("yf", "getFilter('YF')", ParameterMode.Raw);

            Page.Controls.Add(store);

            return store;
        }

        private Component GetDocumentPanel()
        {
            var gp = UiBuilders.CreateGridPanel(DocumentGridID, GetDocumentStore());
            gp.Title = @"Документы";

            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Fn(Scope, "RowSelectDocs");
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.Fn(Scope, "RowDeselectDocs");

            GetDocumentButtons(gp);
            GetDocumentColumnModel(gp.ColumnModel);
            gp.AutoExpandColumn = documentModel.NameOf(() => documentModel.Note);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = true, 
                        Filters =
                            {
                                new NumericFilter { DataIndex = documentModel.NameOf(() => documentModel.ID) }, 
                                new BooleanFilter { DataIndex = documentModel.NameOf(() => documentModel.PlanThreeYear) }, 
                                new StringFilter { DataIndex = documentModel.NameOf(() => documentModel.RefPartDocName) }, 
                                new NumericFilter { DataIndex = documentModel.NameOf(() => documentModel.RefYearForm) }, 
                                new StringFilter { DataIndex = documentModel.NameOf(() => documentModel.Note) }
                            }
                    });
            gp.Listeners.Command.Handler = "cmdHandler(command, record);";

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private void GetDocumentButtons(GridPanel gp)
        {
            gp.AddRefreshButton();

            if (!auth.IsSpectator())
            {
                gp.AddNewRecordNoEditButton();
            }

            if (auth.IsAdmin())
            {
                gp.AddRemoveRecordWithConfirmButton();
            }

            if (!auth.IsSpectator())
            {
                gp.AddSaveButton();
            }

            gp.Toolbar().Add(new ToolbarSeparator());

            var import = new UpLoadFileBtnControl
            {
                Id = "btnImport",
                Name = "Импорт из XML",
                Icon = Icon.DiskDownload,
                Upload = true,
                UploadController = UiBuilders.GetUrl<ImportsController>("ImportsFromGmuXml")
            };

            gp.Toolbar().Add(import.Build(Page));
        }

        private void GetDocumentColumnModel(ColumnModel cm)
        {
            cm.Columns.Add(
                new CommandColumn
                    {
                        Width = 30, 
                        Commands =
                            {
                                new GridCommand
                                    {
                                        Icon = Icon.Pencil, 
                                        CommandName = "EditDoc", 
                                        Text = "Открыть", 
                                        ToolTip = { Text = "Редактировать документ" }
                                    }
                            }, 
                        PrepareToolbar = { Fn = string.Concat(Scope, ".prepareToolbar") }
                    }

                    .SetWidth(15));

            cm.Columns.Add(
                new Column
                    {
                        Header = documentModel.DescriptionOf(() => documentModel.RefSost), 
                        DataIndex = documentModel.NameOf(() => documentModel.RefSost), 
                        Align = Alignment.Center, 
                        Renderer = new Renderer("return getUrlForStatus(value, record);")
                    }

                    .SetWidth(15));

            cm.AddColumn(() => documentModel.OpeningDate, DataAttributeTypes.dtDate)
                .SetWidth(100)
                .SetHidden(true);
            cm.AddColumn(() => documentModel.CloseDate, DataAttributeTypes.dtDate)
                .SetWidth(100)
                .SetHidden(true);
            cm.AddColumn(() => documentModel.PlanThreeYear, DataAttributeTypes.dtBoolean)
                .SetEditableBoolean()
                .SetWidth(100)
                .SetHidden(true);

            cm.AddColumn(() => documentModel.FormationDate, DataAttributeTypes.dtDate)
                .SetEditableDate();
            
            var param =
                new Dictionary<string, string>
                    {
                        { "masterId", "getMasterID('{0}')".FormatWith(InstitutionsGridID) },
                        { "formationDate", Scope + ".getSelectedFormationDate()" }
                    };
            var editor = cm.AddColumn(() => documentModel.RefPartDocName, DataAttributeTypes.dtString)
                .SetWidth(50)
                .SetWrap(true)
                .SetComboBoxEditor(FX_FX_PartDoc.Key, Page, documentModel.NameOf(() => documentModel.RefPartDoc), UiBuilders.GetUrl<CommonDataController>("GetPartDocList"), null, true, true, param);
            
            editor.EditorOptions.Listeners.BeforeStartEdit.Handler = "return E86n.View.StartDoc.Grid.getSelectedFormationDate() !== '';";

            cm.AddColumn(() => documentModel.RefYearForm, DataAttributeTypes.dtString)
                .SetLookup(documentModel.NameOf(() => documentModel.RefYearForm), "storecbYearFormation", "ID", "ID")
                .SetWidth(20);

            var col = new Column
                {
                    ColumnID = documentModel.NameOf(() => documentModel.Note), 
                    DataIndex = documentModel.NameOf(() => documentModel.Note), 
                    Header = documentModel.DescriptionOf(() => documentModel.Note)
                }

                .SetEditableString().SetWrap(true).SetMaxLengthEdior(1024);

            if (!auth.IsAdmin())
            {
                col.EditorOptions.Listeners.BeforeStartEdit.Handler =
                    "return {0}.SetEditbleNote({1});".FormatWith(Scope, auth.IsGrbsUser().ToString().ToLower());
            }

            cm.Columns.Add(col);
        }

        #endregion
    }
}