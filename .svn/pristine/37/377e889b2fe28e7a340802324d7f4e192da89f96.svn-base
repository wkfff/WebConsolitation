using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Common.Constants;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Views
{
    public class ProgramView : View
    {
        internal const string DatasourceProgramID = "dsProgram";

        private readonly IExtension extension;

        public ProgramView(IExtension extension)
        {
            this.extension = extension;
        }

        public int? ProgramId
        {
            get { return String.IsNullOrEmpty(Params["id"]) ? null : (int?)Convert.ToInt32(Params["id"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            var isInAvailableRoles = User.IsInAnyRoles(ProgramRoles.Creator, ProgramRoles.Viewer);
            if (!isInAvailableRoles)
            {
                return new List<Component> { new DisplayField { Text = "Данный пользователь не входит в группы 'ЭО Целевых программ' или 'Координаторы Целевых программ'. Представление недоступно." } };
            }

            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("ProgramView", Resource.ProgramView);
            }
            
            page.Controls.Add(GetProgramAttributeStore());

            bool editable = User.IsInRole(ProgramRoles.Creator);

            var view = new Viewport
            {
                ID = "viewportMain",
                AutoScroll = false,
                Layout = LayoutType.Row.ToString(),
            };

            view.Items.Add(CreateAttributeFormPanel(editable));
        
            view.Items.Add(new Button
                               {
                                   ID = "btSaveNewProgram", 
                                   Text = "Сохранить и перейти к детализации", 
                                   Height = 20, 
                                   Icon = Icon.BookmarkGo, 
                                   MaxWidth = 250, 
                                   Visible = ProgramId == null,
                                   Listeners = { Click = { Handler = "submitForm(programForm);" } }
                               });
            
            view.Items.Add(CreateProgramDetailTabPanel());

            if (ProgramId != null)
            {
                view.AddScript("TabNPA.reload();");
            }

            return new List<Component> { view, CreateApprovementWindow() };
        }

        private Component CreateAttributeFormPanel(bool editable)
        {
            var form = new FormPanel
            {
                ID = "programForm",
                Border = false,
                TopBar = { CreateToolbar(editable) },
                Collapsible = true,
                MaxHeight = 240,
                Url = "/Programs/SaveProgram",
                MonitorValid = true,
                AutoScroll = true,
                LabelSeparator = String.Empty,
                Padding = 15,
                LabelWidth = 200,
                DefaultAnchor = "100%"
            };
            form.Listeners.Collapse.Handler = "viewportMain.doLayout();";
            form.Listeners.Expand.Handler = "viewportMain.doLayout();";
            
            form.Listeners.AfterRender.Handler = "viewportMain.doLayout();";
            form.Listeners.AfterRender.Delay = 100;
            
            form.Listeners.ClientValidation.Handler = @"
btnSave.setDisabled(!(valid));
btnSave.setTooltip(valid ? 'Сохранить' : 'В карточке не все поля заполнены, либо имеют некорректные значения!');
var btnSaveNew = Ext.getCmp('btSaveNewProgram');
if (btnSaveNew !=undefined) {
   btnSaveNew.setDisabled(!valid);
}
";

            form.Items.Add(new TextField { ID = "ID", DataIndex = "ID", Hidden = true });
            form.Items.Add(new TextField { ID = "Name", FieldLabel = "Наименование программы", DataIndex = "Name", AllowBlank = false, ReadOnly = !editable, MaxLength = 4000 });
            form.Items.Add(new TextField { ID = "ShortName", FieldLabel = "Сокращенное наименование", DataIndex = "ShortName", AllowBlank = false, ReadOnly = !editable, MaxLength = 255 });

            var programType = new ComboBox
                               {
                                   ID = "comboRefPart", 
                                   FieldLabel = "Тип программы", 
                                   Items =
                                    {
                                        new ListItem { Value = ((int)ProgramType.Longterm).ToString(), Text = "Долгосрочная целевая программа" }, 
                                        new ListItem { Value = ((int)ProgramType.Department).ToString(), Text = "Ведомственная целевая программа" },
                                    }, 
                                   Editable = false, 
                                   ReadOnly = !editable, 
                                   HideMode = HideMode.Visibility, 
                                   TriggerAction = TriggerAction.All, 
                                   Mode = DataLoadMode.Local, 
                                   Resizable = false, 
                                   SelectOnFocus = false, 
                                   DataIndex = "RefTypeProgId", 
                                   MaxWidth = 300, 
                                   AllowBlank = false
                               };
            form.Items.Add(programType);

            if (extension.OKTMO == OKTMO.Sakhalin)
            {
                programType.Items.Clear();
                programType.Items.Add(new ListItem { Value = ((int)ProgramType.Municipal).ToString(), Text = "Муниципальная целевая программа" });
                programType.ReadOnly = true;
            }

            form.Items.Add(new TextField { ID = "Creator", FieldLabel = "Заказчик", DataIndex = "Creator", ReadOnly = true });
            form.Items.Add(new TextField { ID = "ApproveDate", FieldLabel = "Дата утверждения", DataIndex = "ApproveDate", ReadOnly = true, MaxWidth = 100 });
            form.Items.Add(new SpinnerField { ID = "RefBeginDateVal", FieldLabel = "Год начала реализации", DataIndex = "RefBeginDateVal", AllowBlank = false, ReadOnly = !editable, MaxWidth = 100 });
            form.Items.Add(new SpinnerField { ID = "RefEndDateVal", FieldLabel = "Год окончания реализации", DataIndex = "RefEndDateVal", AllowBlank = false, ReadOnly = !editable, MaxWidth = 100 });
            form.Items.Add(new TextField { ID = "NpaListCommaSeparated", FieldLabel = "Нормативное основание", DataIndex = "NpaListCommaSeparated", ReadOnly = true });
            
            var parentProgram = new ComboBox
                                    {
                                        ID = "comboRefParent",
                                        FieldLabel = "Родительская целевая программа",
                                        TriggerAction = TriggerAction.All,
                                        Store = { GetParentProgramLookupStore() },
                                        DataIndex = "ParentName",
                                        AutoShow = true,
                                        Height = 60,
                                        Resizable = true,
                                        Editable = false,
                                        ReadOnly = !editable,
                                        AllowBlank = true,
                                        ValueField = "Name",
                                        DisplayField = "Name",
                                        Triggers = { new FieldTrigger { Icon = TriggerIcon.Clear, Qtip = "Очистить", HideTrigger = !editable } },
                                        Listeners =
                                            {
                                                Select = { Handler = "ParentId.setValue(record.get('ID'));" },
                                                TriggerClick = { Handler = "this.clearValue(); ParentId.setValue(null);" }
                                            }
                                    };
            form.Items.Add(parentProgram); 
            form.Items.Add(new TextField { ID = "ParentId", Hidden = true, DataIndex = "ParentId" });
            
            return form;
        }
        
        private Toolbar CreateToolbar(bool editable)
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceProgramID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Hidden = !editable,
                Listeners = { Click = { Handler = "submitForm(programForm);" } }
            });

            toolbar.Add(new ToolbarSpacer(10));

            toolbar.Add(new Button
            {
                ID = "btnApprove",
                Icon = Icon.UserTick,
                ToolTip = "Утвердить",
                Disabled = true,
                Hidden = !editable,
                Listeners = { Click = { Handler = "wApproveDialog.show(); wApproveDialog.center();" } }
            });

            return toolbar;
        }
        
        private List<Component> CreateProgramDetailTabPanel()
        {
            var tabPanel = new TabPanel
            {
                ID = "ProgramDetailTabPanel",
                IDMode = IDMode.Explicit,
                Border = false,
                RowHeight = 1,
                Disabled = ProgramId == null,
                ActiveTabIndex = 0,
                EnableTabScroll = true,
                MonitorResize = true,
                LayoutOnTabChange = true,
                Closable = true
            };
            
            tabPanel.Items.Add(new Panel
            {
                ID = "TabNPA",
                Title = "НПА",
                AutoShow = false,
                MonitorResize = true,
                AutoLoad =
                {
                    Url = "/View/TrgProgDetailNPA",
                    Params = { new Parameter("programId", ProgramId.ToString(), ParameterMode.Value) },
                    Mode = LoadMode.IFrame,
                    ManuallyTriggered = true,
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(CreateTabView("TabTargets", "Цели", "/View/TrgProgDetailTargets"));
            tabPanel.Items.Add(CreateTabView("TabTasks", "Задачи", "/View/TrgProgDetailTasks"));
            tabPanel.Items.Add(CreateTabView("TabActions", "Мероприятия", "/View/TrgProgDetailActions"));
            tabPanel.Items.Add(CreateTabView("TabFinances", "Финансирование", "/View/TrgProgDetailFinances"));
            tabPanel.Items.Add(CreateTabView("TabTargetRatings", "Целевые показатели", "/View/TrgProgDetailTargetRatings"));
            tabPanel.Items.Add(CreateTabView("TabSubsidy", "Субсидии", "/View/TrgProgDetailSubsidy"));
            
            return new List<Component> { tabPanel };
        }

        private Store GetProgramAttributeStore()
        {
            var store = new Store { ID = DatasourceProgramID, AutoLoad = true };
            store.SetHttpProxy("/Programs/LoadProgram");
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID", RecordFieldType.Auto),
                    new RecordField("Name"),
                    new RecordField("ShortName"),
                    new RecordField("RefTypeProgId"),
                    new RecordField("Creator"),
                    new RecordField("ApproveDate"),
                    new RecordField("RefBeginDateVal"),
                    new RecordField("RefEndDateVal"),
                    new RecordField("NpaListCommaSeparated"),
                    new RecordField("ParentName"),
                    new RecordField("ParentId")
                }
            });
            store.Listeners.BeforeLoad.Handler = "Ext.apply(options.params, this.baseParams, options.params);";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке программы', response.responseText);";
            store.Listeners.DataChanged.Handler = @"
var record = this.getAt(0) || {};
programForm.getForm().loadRecord(record);
programForm.validate();
if (record.data.ApproveDate!=undefined && record.data.ApproveDate==='' && record.data.ID != undefined && record.data.ID > 0)
{
   btnApprove.setDisabled(false);
}
ResetDirtyAttributeOnFormItems(programForm);
";
            return store;
        }

        private Panel CreateTabView(string id, string title, string url)
        {
            var panel = new Panel
            {
                ID = id,
                Title = title,
                MonitorResize = true,
                AutoLoad =
                {
                    Url = url,
                    Params = { new Parameter("programId", ProgramId.ToString(), ParameterMode.Value) },
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            };

            return panel;
        }

        private Store GetParentProgramLookupStore()
        {
            var store = new Store { ID = "dsActions", AutoLoad = false };
            store.SetHttpProxy("/Programs/GetParentProgramListForLookup");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка программ', response.responseText);";
            return store;
        }

        private Window CreateApprovementWindow()
        {
            Window win = new Window
            {
                ID = "wApproveDialog",
                Title = "Утверждение целевой программы",
                Hidden = true,
                Modal = true,
                Width = 500,
                AutoHeight = true,
                Resizable = false,
                Border = false,
            };
            FormPanel form = new FormPanel();
            win.Add(form);

            form.ID = "fApproveDialog";
            form.Frame = true;
            form.MonitorValid = false;
            form.Border = false;
            form.BodyBorder = false;
            form.BodyStyle = "padding: 10px 10px 0 10px;background:none repeat scroll 0 0 #DFE8F6;";

            form.Defaults.Add(new Parameter { Name = "anchor", Value = "95%", Mode = ParameterMode.Value });
            form.Defaults.Add(new Parameter { Name = "msgTarget", Value = "side", Mode = ParameterMode.Value });

            form.Items.Add(new DateField
                               {
                                   ID = "ApproveDateDlg",
                                   FieldLabel = "Дата утверждения",
                                   AllowBlank = false,
                                   Value = DateTime.Now
                               });
            form.Items.Add(new TextField
            {
                ID = "ApproveNpaDlg",
                FieldLabel = "Нормативное основание",
                AllowBlank = false
            });
                          
            form.Items.Add(new FileUploadField
            {
                ID = "fileUploadFieldDlg",
                EmptyText = "Выберите файл...",
                FieldLabel = "Файл",
                AllowBlank = false,
                ButtonText = String.Empty,
                Icon = Icon.ImageAdd
            });
            
            Button saveButton = new Button
            {
                ID = "btSaveDlg",
                Text = "Сохранить",
                DirectEvents =
                {
                    Click =
                    {
                        Method = HttpMethod.POST,
                        CleanRequest = true,
                        Url = "/Programs/ApproveProgram",
                        IsUpload = true,
                        ExtraParams =
                                             {
                                                 new Parameter("programId", "ID.getValue()", ParameterMode.Raw),
                                                 new Parameter("approveDate", "ApproveDateDlg.getValue().format('Y-m-d')", ParameterMode.Raw),
                                                 new Parameter("npaName", "ApproveNpaDlg.getValue()", ParameterMode.Raw),
                                                 new Parameter("fileName", "fileUploadFieldDlg.getValue()", ParameterMode.Raw)
                                             },
                        Before = @"
if (!fApproveDialog.getForm().isValid()) {
  Ext.net.Notification.show({iconCls : 'icon-information', html : 'Не заполнены обязательные поля', title : 'Уведомление', hideDelay : 2500});
  return false;
}
Ext.Msg.wait('Загружается...', 'Загрузка');",
                        Failure = @"
if (result.extraParams != undefined && result.extraParams.responseText!=undefined){
  Ext.Msg.show({title:'Ошибка', msg:result.extraParams.responseText, minWidth:200, modal:true, icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });
}else{
  Ext.Msg.show({title:'Ошибка', msg:result.responseText, minWidth:200, modal:true, icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });
}
",
                        Success = @"
Ext.MessageBox.hide();
wApproveDialog.hide();
Ext.net.Notification.show({iconCls : 'icon-information', html : result.extraParams.msg, title : 'Уведомление', hideDelay : 2500});
"
                    }
                }
            };

            form.Buttons.Add(saveButton);

            return win;
        }
    }
}
