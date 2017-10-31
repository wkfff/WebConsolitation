using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.InstitutionsRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public sealed class OGSView : View
    {
        private const string StoreId = "OGSStore";
        private const string ViewId = "OGSGrid";
        private const string Scope = "E86n.View.OGSView";
        private readonly InstitutionsRegisterModel model = new InstitutionsRegisterModel();
        private readonly IAuthService auth;

        public OGSView(IAuthService auth)
        {
            this.auth = auth;
        }

        public ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("OGSFunction", Resource.OGSView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StdHandBooks", Resource.StdHandBooks);

            var view = new Viewport
                {
                    ID = "viewportMain",
                    Items =
                        {
                            new RowLayout
                                {
                                    Rows =
                                        {
                                            new LayoutRow { Items = { CreateFilterPanel() } },
                                            new LayoutRow { RowHeight = 1m, Items = { CreateGridPanel() } },
                                        }
                                }
                        }
                };

            return new List<Component> { view };
        }

        private Panel CreateFilterPanel()
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
                Height = 100,
                Collapsible = true,
            };

            filterPnael.Items.Add(CUtils.LookUpFld("RefOrgPPOFltName", "Публично-правовое образование, создавшее учреждение", string.Empty, D_Org_PPO.Key));
            var hid = new Hidden { ID = "RefOrgPPOFlt" };
            hid.Listeners.Change.Handler = string.Concat(Scope, ".UpdateFlt();");
            filterPnael.Items.Add(hid);

            filterPnael.Items.Add(CUtils.LookUpFld("RefOrgGRBSFltName", "ГРБС", string.Empty, D_Org_GRBS.Key, false, "'(REFORGPPO='+getFilter('PPO')+')'"));
            hid = new Hidden { ID = "RefOrgGRBSFlt" };
            hid.Listeners.Change.Handler = string.Concat(Scope, ".UpdateFlt();");
            filterPnael.Items.Add(hid);

            return filterPnael;
        }

        private IEnumerable<Component> CreateButtons()
        {
            var toolbar = new List<Component>();

            if (auth.IsAdmin())
            {
                var importEgrul = new UpLoadFileBtnControl
                    {
                        Id = "btnUploadEGRUL",
                        Name = "Импорт ЕГРЮЛ",
                        UploadController = UiBuilders.GetUrl<OGSController>("ImportFile")
                    };
                importEgrul.Params.Add("Import", "1");

                toolbar.Add(importEgrul.Build(Page)[0]);
            }

            if (auth.IsAdmin())
            {
                var import = new UpLoadFileBtnControl
                    {
                        UploadController = UiBuilders.GetUrl<OGSController>("ImportFile")
                    };

                import.Params.Add("Import", "2");

                toolbar.Add(import.Build(Page)[0]);
            }

            var btnClose = new Button
                {
                    ID = "btnClose", 
                    Icon = Icon.Lock, 
                    ToolTip = @"Закрыть учреждение", 
                    Text = @"Закрыть", 
                    Disabled = true, 
                    Hidden = true
                };
            Page.Controls.Add(CreateCloseOgsWindow(UiBuilders.GetUrl<OGSController>("CloseOgs")));
            btnClose.Listeners.Click.Handler = "#{OGSCloseWindow}.show()";
            toolbar.Add(btnClose);

            var btnOpen = new Button
            {
                ID = "btnOpen",
                Icon = Icon.LockOpen,
                ToolTip = @"Открыть учреждение",
                Text = @"Открыть",
                Disabled = true,
                Hidden = true
            };

            btnOpen.DirectEvents.Click.Url = UiBuilders.GetUrl<OGSController>("OpenOgs");
            btnOpen.DirectEvents.Click.CleanRequest = true;
            btnOpen.DirectEvents.Click.IsUpload = true;
            btnOpen.DirectEvents.Click.FormID = "Form1";
            btnOpen.DirectEvents.Click.ExtraParams.Add(new Parameter("recId", string.Concat(ViewId, ".getSelectionModel().getSelected().data.ID"), ParameterMode.Raw));
            btnOpen.DirectEvents.Click.Before = @"if ( ! confirm('Выполнение этой операции приведет к открытию учреждения. Вы хотите продолжить?')) return false;";
            btnOpen.DirectEvents.Click.Success = @"Ext.Msg.show({
                                                           title:'Сообщение',
                                                           msg: 'Учреждение успешно открыто',
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.INFO,
                                                           maxWidth: 1000
                                                        });";
            btnOpen.DirectEvents.Click.Failure = @"Ext.Msg.show({
                                                           title:'Ошибка',
                                                           msg: 'При закрытии учреждения произошла ошибка: ' + response.responseText,
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.ERROR,
                                                           maxWidth: 1000
                                                        });";

            toolbar.Add(btnOpen);

            if (auth.IsAdmin())
            {
                btnClose.Hidden = false;
                btnOpen.Hidden = false;
            }

            var excelBtn = new Button
            {
                ID = "excel",
                Icon = Icon.PageExcel,
                ToolTip = @"Выгрузить реестр учреждений в excel",
            };
            excelBtn.DirectEvents.Click.Url = "/Reports/ImportInstitutionsGrid";
            excelBtn.DirectEvents.Click.CleanRequest = true;
            excelBtn.DirectEvents.Click.IsUpload = true;
            excelBtn.DirectEvents.Click.FormID = "Form1";
            excelBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("state", string.Concat(ViewId, ".getState()"), ParameterMode.Raw));
            excelBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("gridFilters", string.Format("{0}.filters.buildQuery({0}.filters.getFilterData())", ViewId), ParameterMode.Raw));
            excelBtn.DirectEvents.Click.Failure = @"Ext.Msg.show({
                                                           title:'Ошибка',
                                                           msg: result.responseText,
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.ERROR,
                                                           maxWidth: 1000
                                                        });";
            toolbar.Add(excelBtn);
            return toolbar;
        }

        private Component CreateGridPanel()
        {
            var gp = UiBuilders.CreateGridPanel(ViewId, GetStore());
            gp.Title = @"Учреждения";
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowSelect.Handler = Scope + ".Update()";
            ((RowSelectionModel)gp.GetSelectionModel()).Listeners.RowDeselect.Handler = Scope + ".Update()";
            gp.Listeners.Command.Handler = Scope + ".commandClick(command, record);";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordWithConfirmButton();
            gp.AddSaveButton();
            gp.Toolbar().Add(CreateButtons());

            gp.ColumnModel.Columns.Add(
                new ImageCommandColumn
                {
                    DataIndex = "Action",
                    Width = 22,
                    Commands =
                            {
                                new ImageCommand
                                    {
                                        Icon = Icon.PageEdit,
                                        CommandName = "EditTypeHistory",
                                        ToolTip = { Text = "Изменить тип учреждения" },
                                    },
                            }
                });

            gp.ColumnModel.Columns.Add(new RowNumbererColumn());
            if (auth.IsAdmin())
            {
                gp.ColumnModel.AddColumn(() => model.Status, DataAttributeTypes.dtBoolean).SetWidth(20);
            }

            gp.ColumnModel.AddColumn(() => model.Name, DataAttributeTypes.dtString)
                .SetEditableString()
                .SetWidth(400);
            gp.ColumnModel.AddColumn(() => model.ShortName, DataAttributeTypes.dtString)
                .SetEditableString()
                .SetWidth(300);
            gp.ColumnModel.AddColumn(() => model.RefTipYcName, DataAttributeTypes.dtString)
                .SetWidth(200)
                .SetComboBoxEditor(FX_Org_TipYch.Key, Page, model.NameOf(() => model.RefTipYc));
            var mapping = new Dictionary<string, string[]>
                {
                    { model.NameOf(() => model.RefOrgPpo), new[] { "ID" } },
                    { model.NameOf(() => model.RefOrgPpoName), new[] { "Code", "Name" } }
                };
            gp.ColumnModel.AddColumn(() => model.RefOrgPpoName, DataAttributeTypes.dtString)
                .SetWidth(300)
                .SetComboBoxEditor(D_Org_PPO.Key, Page, mapping, null, null, "Name");
            mapping = new Dictionary<string, string[]>
                {
                    { model.NameOf(() => model.RefOrgGrbs), new[] { "ID" } },
                    { model.NameOf(() => model.RefOrgGrbsName), new[] { "Code", "Name" } }
                };
            gp.ColumnModel.AddColumn(() => model.RefOrgGrbsName, DataAttributeTypes.dtString)
                .SetWidth(300)
                .SetComboBoxEditor(D_Org_GRBS.Key, Page, mapping, null, null, "Name");
            gp.ColumnModel.AddColumn(() => model.Inn, DataAttributeTypes.dtString)
                .SetEditableInteger()
                .SetWidth(100)
                .SetMaxLengthEdior(10);
            gp.ColumnModel.AddColumn(() => model.Kpp, DataAttributeTypes.dtString)
                .SetEditableInteger()
                .SetWidth(100)
                .SetMaxLengthEdior(9);
            gp.ColumnModel.AddColumn(() => model.OpenDate, DataAttributeTypes.dtDateTime)
                .SetEditableDate();
            gp.ColumnModel.AddColumn(() => model.CloseDate, DataAttributeTypes.dtDateTime)
                .SetEditable(false);

            gp.Plugins.Add(
                new GridFilters
                    {
                        Local = false,
                        Filters =
                            {
                                new NumericFilter { DataIndex = model.NameOf(() => model.ID) },
                                new BooleanFilter
                                    {
                                        DataIndex = model.NameOf(() => model.Status),
                                        YesText = @"Открыто",
                                        NoText = @"Закрыто",
                                        Value = true,
                                    },
                                new StringFilter { DataIndex = model.NameOf(() => model.Name) },
                                new StringFilter { DataIndex = model.NameOf(() => model.ShortName) },
                                new StringFilter { DataIndex = model.NameOf(() => model.RefTipYcName) },
                                new StringFilter { DataIndex = model.NameOf(() => model.Inn) },
                                new StringFilter { DataIndex = model.NameOf(() => model.Kpp) },
                                new StringFilter { DataIndex = model.NameOf(() => model.RefOrgGrbsName) },
                                new StringFilter { DataIndex = model.NameOf(() => model.RefOrgPpoName) },
                                new DateFilter { DataIndex = model.NameOf(() => model.OpenDate) },
                                new DateFilter { DataIndex = model.NameOf(() => model.CloseDate) },
                            }
                    });

            gp.BottomBar.Add(
                new PagingToolbar
                    {
                        ID = "ptbOGS",
                        StoreID = "OGSStore",
                        PageSize = 10,
                        Items =
                            {
                                new NumberField
                                    {
                                        ID = "nfSizePage",
                                        FieldLabel = @"Размер страницы",
                                        Number = 10,
                                        Listeners = { Change = { Handler = "#{ptbOGS}.pageSize = parseInt(this.getValue());" } }
                                    }
                            }
                    });

            gp.AddColumnsWrapStylesToPage(Page);
            return gp;
        }

        private Store GetStore()
        {
            var store = StoreExtensions.StoreCreateDefault(StoreId, true, typeof(OGSController), createActionName: "Save", updateActionName: "Save");
            store.AddFieldsByClass(model);
            store.AddField(model.NameOf(() => model.Status), new RecordField.Config { DefaultValue = "false" });

            store.BaseParams.Add(new Parameter("limit", "nfSizePage.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));

            Page.Controls.Add(store);
            return store;
        }

        private Window CreateCloseOgsWindow(string closeController)
        {
            var closeDate = new DateField
            {
                ID = "closeDate",
                FieldLabel = @"Дата закрытия",
                Width = 250,
                SelectedDate = DateTime.Now,
                MaxDate = DateTime.Now
            };

            var btnClose = new Button
            {
                ID = "btnCloseWin",
                Icon = Icon.Lock,
                ToolTip = @"Закрыть",
                Text = @"Закрыть",
            };

            btnClose.DirectEvents.Click.Url = closeController;
            btnClose.DirectEvents.Click.Method = HttpMethod.POST;
            btnClose.DirectEvents.Click.CleanRequest = true;
            btnClose.DirectEvents.Click.IsUpload = true;

            btnClose.DirectEvents.Click.ExtraParams.Add(new Parameter("closeDate", "closeDate.getValue()", ParameterMode.Raw));
            btnClose.DirectEvents.Click.ExtraParams.Add(new Parameter("recId", string.Concat(ViewId, ".getSelectionModel().getSelected().data.ID"), ParameterMode.Raw));
            btnClose.DirectEvents.Click.ExtraParams.Add(new Parameter("note", "Note.getValue()"));
            btnClose.DirectEvents.Click.Before = @"if ( ! confirm('Выполнение этой операции приведет к закрытию учреждения. Вы хотите продолжить?')) return false;";
            btnClose.DirectEvents.Click.Failure = @"Ext.Msg.show({
                                                           title:'Ошибка',
                                                           msg: result.extraParams.msg + result.extraParams.responseText,
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.ERROR,
                                                           maxWidth: 1000
                                                        });#{OGSCloseWindow}.hide()";
            btnClose.DirectEvents.Click.Success = @"Ext.Msg.show({
                                                           title:'Сообщение',
                                                           msg: result.extraParams.msg,
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.INFO,
                                                           maxWidth: 1000
                                                        });#{OGSCloseWindow}.hide()";

            var attachBtn = new Button
            {
                ID = "attachBtn",
                Icon = Icon.Attach,
                ToolTip = @"Прикрепить документ",
                Text = @"Прикрепить документ",
            };
            attachBtn.Listeners.Click.Handler =
                string.Concat(
                "var recId = ", 
                ViewId, 
@".getSelectionModel().getSelected().data.ID;
var myProxy = new Ext.data.HttpProxy
        ({
            url: '/OGS/',
            api:
            {
                read: { url: '/OGS/GetPassportForOgs', method: 'GET' },
            },
            timeout: 60000
        });

    var myReader = new Ext.data.JsonReader
    ({
        successProperty: 'success',
        idProperty: 'ID',
        root: 'data',
        fields: ['ID', 'DocID']
    });

    var myWriter = new Ext.data.JsonWriter
    ({
        encode: true,
        writeAllFields: false
    });

    var myStore = new Ext.data.Store
    ({
        id: 'oStore',
        proxy: myProxy,
        reader: myReader,
        writer: myWriter,
        idProperty: 'id',
        autoSave: true,
        listeners:
        {
            load: function (store, records, options) {
                var index = store.findExact('ID', recId);
                var record = store.getAt(index);
                var DocID = record.get('DocID');
                if (DocID != 0){
                 btnCloseWin.setDisabled(false);
                 window.parent.MdiTab.addTab({ title: 'Паспорт учреждения', url: '/View/PassportForm?RecId=' + DocID});
                }
                else{
                    Ext.Msg.show({
                                                           title:'Ошибка',
                                                           msg: 'Паспорт учреждения не найден',
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.ERROR,
                                                           maxWidth: 1000
                                                        });
                    btnCloseWin.setDisabled(false);
                }
            }
        }
    });
    myStore.setBaseParam('recId', recId);

    myStore.load();");
            var win = new Window
            {
                AutoRender = false,
                ID = "OGSCloseWindow",
                Hidden = true,
                Width = 320,
                Height = 320,
                Modal = true,
                Title = @"Закрытие учреждения",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false,
                Resizable = false,
                MonitorResize = true,
                Padding = 10,
                Layout = LayoutType.Fit.ToString(),
                AutoHeight = true,
                Items =
                        {
                            new FormPanel
                                {
                                    ID = "form", 
                                    MonitorValid = true, 
                                    Layout = LayoutType.Form.ToString(), 
                                    ButtonAlign = Alignment.Right, 
                                    LabelAlign = LabelAlign.Top, 
                                    Border = false, 
                                    BodyCssClass = "x-window-mc", 
                                    CssClass = "x-window-mc", 
                                    LabelWidth = 200, 
                                    DefaultAnchor = "99%", 
                                    AutoHeight = true, 
                                    BodyStyle = "padding:10px", 
                                    Items =
                                        {
                                            new Label
                                                {
                                                    LabelAlign = LabelAlign.Top, 
                                                    Text = @"Примечание:"
                                                }, 
                                            new TextArea
                                                {
                                                    ID = "Note"
                                                }, 
                                            closeDate, 
                                            btnClose
                                        }
                                }
                        },
            };

            return win;
        }
    }
}