using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.Messages.Presentation.Controls
{
    public class MessageGridControl : Control
    {
        private readonly BasePrincipal principal;

        public MessageGridControl(BasePrincipal principal)
        {
            this.principal = principal;
            PagingSettings = new PagingSettings();
        }

        #region main
        
        public PagingSettings PagingSettings { get; set; }
        
        public override List<Component> Build(ViewPage page)
        {
            return new List<Component>
                {
                    BuildReciveMessagesPanel(page, "/MessagesNav/ReciveMessages")
                };
        }

        #endregion

        #region send window

        public Window CreateSendMsgWindow(ViewPage page, string windowID, string url)
        {
            var wndMessage = new Window
            {
                ID = windowID,
                Hidden = true,
                Width = 350,
                Height = 320,
                Modal = true,
                Title = "Отправка сообщения",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false,
                Resizable = false,
                MonitorResize = true,
                Padding = 10,
                Layout = LayoutType.Fit.ToString(),
                AutoHeight = true
            };

            var formPanel = new FormPanel
            {
                ID = "NewFormPanel",
                ButtonAlign = Alignment.Right,
                LabelAlign = LabelAlign.Top,
                Border = false,
                Layout = LayoutType.Form.ToString(),
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                MonitorValid = true,
                AutoHeight = true,
                BodyStyle = "padding:10px"
            };

            var anel = new Panel
            {
                ID = "anel",
                ButtonAlign = Alignment.Right,
                LabelAlign = LabelAlign.Top,
                Border = false,
                Layout = LayoutType.Form.ToString(),
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
            };

            formPanel.Items.Add(anel);

            string script = @"function panelLoaded() {
           Ext.getCmp('anel').add(new Ext.ux.form.SuperBoxSelect({
            allowBlank: false,
            msgTarget: 'under',
            allowAddNewData: true,
            id: 'selector1',
            xtype: 'superboxselect',
            addNewDataOnBlur: true,
            fieldLabel: 'Выберите получателей',
            emptyText: 'Выберите получателей',
            resizable: true,
            name: 'StoreUsers',
            anchor: '100%',
            store:'StoreUsers',
            mode: 'local',
            displayField: 'Text',
            displayFieldTpl: '{Text}',
            valueField: 'Value',
            forceSelection: false
        }))
}
";
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("panelLoaded", script);
            formPanel.AddScript("panelLoaded();");

            string uploadFieldName = "{0}uploadField".FormatWith(windowID);
            var uploadField = new FileUploadField
            {
                ID = uploadFieldName,
                EmptyText = "Выберите файл",
                Icon = Icon.Attach,
                ButtonText = String.Empty,
                ToolTip = "Прикрепить файл.",
                AnchorHorizontal = "100%",
                FieldLabel = "Вложение"
            };

            uploadField.DirectEvents.FileSelected.Method = HttpMethod.POST;
            uploadField.DirectEvents.FileSelected.Url = "/MessagesNav/Upload";
            uploadField.DirectEvents.FileSelected.IsUpload = true;
            uploadField.DirectEvents.FileSelected.CleanRequest = true;
            uploadField.DirectEvents.FileSelected.Before = @"
Ext.Msg.wait('Загрузка...', 'Загрузка');";
            uploadField.DirectEvents.FileSelected.Failure = "Ext.Msg.show({title:'Ошибка', msg:result.extraParams.responseText, minWidth:200, modal:true, icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });";
            uploadField.DirectEvents.FileSelected.Success = @"
Ext.net.Notification.show({iconCls : 'icon-information', html : result.extraParams.msg, title : 'Уведомление', hideDelay : 2500});";
            formPanel.Items.Add(uploadField);

            string txtAreaName = "{0}TxtArea".FormatWith(windowID);

            var txtArea = new TextArea
            {
                ID = txtAreaName,
                AutoScroll = true,
                Width = 300,
                Height = 100,
                FieldLabel = "Текст сообщения",
                AllowBlank = false,
                AnchorHorizontal = "100%",
                MaxLength = 255
            };

            formPanel.Items.Add(txtArea);

            var btnCancel = new Button
            {
                ID = "{0}btnCancel".FormatWith(windowID),
                Text = "Отмена"
            };

            btnCancel.Listeners.Click.Handler = @"
Ext.getCmp('{0}').hide();
".FormatWith(windowID);

            var btnOk = new Button
            {
                ID = "btnOk",
                Text = "Отправить",
                Icon = Icon.Mail,
                Disabled = true
            };

            btnOk.DirectEvents.Click.Url = url;
            btnOk.DirectEvents.Click.CleanRequest = true;
            btnOk.DirectEvents.Click.EventMask.ShowMask = false;
            btnOk.DirectEvents.Click.IsUpload = false;

            btnOk.DirectEvents.Click.ExtraParams.Add(
                new Parameter("msg", "{0}.getValue()".FormatWith(txtAreaName), ParameterMode.Raw));
            btnOk.DirectEvents.Click.ExtraParams.Add(
                new Parameter("recips", "Ext.getCmp('selector1').getValue()", ParameterMode.Raw));
            btnOk.DirectEvents.Click.ExtraParams.Add(
                new Parameter("fileName", "Ext.getCmp('{0}').getValue()".FormatWith(uploadFieldName), ParameterMode.Raw));
            btnOk.DirectEvents.Click.Success = @"Ext.getCmp('messagesGridPanel').reload()";

            btnOk.Listeners.Click.AddAfter(@"
Ext.getCmp('{0}').hide();".FormatWith(windowID));

            formPanel.Buttons.Add(btnOk);
            formPanel.Buttons.Add(btnCancel);

            formPanel.Listeners.ClientValidation.Handler = @"#{btnOk}.setDisabled(!valid);";
            wndMessage.Items.Add(formPanel);
            wndMessage.Listeners.Show.Handler = @"
    {0}.setValue('');
    {1}.reset();
    Ext.getCmp('selector1').removeAllItems().resetStore();
".FormatWith(txtAreaName, uploadFieldName);

            return wndMessage;
        }

        #endregion

        #region Build Store

        private Store CreateReciveStore(string id, string url)
        {
            var store = new Store
                {
                    ID = id,
                    AutoLoad = true,
                    SortInfo = { Field = "Date", Direction = SortDirection.DESC },
                    GroupField = "DateGroup",
                    RemoteGroup = true,
                    RemoteSort = true,
                    Restful = false
                };

            var reader = new JsonReader { Root = "data", IDProperty = "ID", TotalProperty = "total" };
            reader.Fields.Add("ID", RecordFieldType.Int);
            reader.Fields.Add("Sender", RecordFieldType.String);
            reader.Fields.Add("Date", RecordFieldType.Date);
            reader.Fields.Add("Status", RecordFieldType.Int);
            reader.Fields.Add("Importance", RecordFieldType.Int);
            reader.Fields.Add("Subject", RecordFieldType.String);
            reader.Fields.Add("MessageType", RecordFieldType.Int);
            reader.Fields.Add("DateGroup", RecordFieldType.String);
            reader.Fields.Add("DateCaption", RecordFieldType.String);
            reader.Fields.Add("RefMessageAttachment", RecordFieldType.Int);
            reader.Fields.Add("MessageTypeCaption", RecordFieldType.String);
            reader.Fields.Add("TransferLink", RecordFieldType.String);

            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
                {
                    Url = url,
                    Method = HttpMethod.POST
                });

            if (PagingSettings.Size < 1)
            {
                PagingSettings.Size = 10;
            }

            if (PagingSettings.Start < 0)
            {
                PagingSettings.Start = 0;
            }

            store.BaseParams.Add(new Parameter("limit", Convert.ToString(PagingSettings.Size), ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", Convert.ToString(PagingSettings.Start), ParameterMode.Raw));

            store.Listeners.Load.Handler = @"
Extensions.Messages.Resive.cloneStore(this, Ext.getCmp('ComboBoxType').getStore(), 'MessageType');
Extensions.Messages.Resive.cloneStore(this, Ext.getCmp('ComboBoxSender').getStore(), 'Sender');";

            return store;
        }

        private Store CreateStoreStatus(string id)
        {
            var store = new Store
                {
                    ID = id,
                    AutoLoad = true
                };

            var arrayReader = new ArrayReader();
            arrayReader.Fields.Add("Status", RecordFieldType.Int);
            arrayReader.Fields.Add("StatusCaption", RecordFieldType.String);
            store.Reader.Add(arrayReader);

            store.DataSource = new object[]
                {
                    new object[] { 1, "Новое" },
                    new object[] { 2, "Прочитано" }
                };
            store.DataBind();

            return store;
        }

        private Store CreateStoreImportance(string id)
        {
            var store = new Store { ID = id };

            var arrayReader = new ArrayReader();
            arrayReader.Fields.Add("Importance", RecordFieldType.Int);
            arrayReader.Fields.Add("ImportanceCaption", RecordFieldType.String);
            store.Reader.Add(arrayReader);

            store.DataSource = new object[]
                {
                    new object[] { 1, "Высокая важность" },
                    new object[] { 2, "Важное" },
                    new object[] { 3, "Обычное" },
                    new object[] { 4, "Неважное" }
                };
            store.DataBind();

            return store;
        }

        private Store CreateStoreMessageType(string id)
        {
            var store = new Store
            {
                ID = id
            };

            var arrayReader = new ArrayReader();
            arrayReader.Fields.Add("MessageType", RecordFieldType.Int);
            arrayReader.Fields.Add("MessageTypeCaption", RecordFieldType.String);
            store.Reader.Add(arrayReader);

            return store;
        }

        private Store CreateStoreMessageSender(string id)
        {
            var store = new Store
            {
                ID = id
            };

            var arrayReader = new ArrayReader();
            arrayReader.Fields.Add("Sender", RecordFieldType.String);
            store.Reader.Add(arrayReader);

            return store;
        }

        private Store CreateGroupAndUserStore(string id, string url)
        {
            var store = new Store
                {
                    ID = id,
                    AutoLoad = true
                };

            var reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("Value", RecordFieldType.String);
            reader.Fields.Add("Text", RecordFieldType.String);
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
                {
                    Url = url,
                    Method = HttpMethod.POST
                });

            return store;
        }

        #endregion

        #region recive grid panel

        private FormPanel BuildReciveMessagesPanel(ViewPage page, string url)
        {
            const string StoreId = "dsReciveMessages";

            page.Controls.Add(CreateReciveStore(StoreId, url));
            page.Controls.Add(CreateStoreStatus("StoreStatus"));
            page.Controls.Add(CreateStoreMessageType("StoreMessageType"));
            page.Controls.Add(CreateStoreMessageSender("StoreMessageSender"));
            page.Controls.Add(CreateStoreImportance("StoreMessageImportance"));
            page.Controls.Add(CreateGroupAndUserStore("StoreUsers", "/MessagesNav/GetUsersAndGroups"));

            ResourceManager.GetInstance(page).RegisterClientStyleBlock(
                "x-grid3-row-expanded",
                ".x-grid3-row-body p { margin : 5px 5px 10px 5px !important; padding: 10px 0px 10px 10px; background-color  : #E0E0E0; }");

            return CreateReciveMessagesPanelInner(page, StoreId);
        }

        private FormPanel InitializeMessagesFormPanel()
        {
            var formPanel = new FormPanel
                {
                    ID = "messagesFormPanel",
                    ButtonAlign = Alignment.Right,
                    LabelAlign = LabelAlign.Top,
                    Border = false,
                    Layout = LayoutType.Fit.ToString(),
                    BodyCssClass = "x-window-mc",
                    CssClass = "x-window-mc"
                };
            return formPanel;
        }

        private FormPanel CreateReciveMessagesPanelInner(ViewPage page, string storeId)
        {
            var formPanel = InitializeMessagesFormPanel();

            var gridPanel = BuildGridPanel(page, storeId);

            formPanel.Items.Add(gridPanel);

            return formPanel;
        }

        private GridPanel BuildGridPanel(ViewPage page, string storeId)
        {
            var gridPanel = new GridPanel
                {
                    ID = "messagesGridPanel",
                    StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                    ColumnLines = true,
                    StoreID = storeId,
                    Layout = LayoutType.Fit.ToString(),
                    AutoExpandColumn = "Subject",
                    StripeRows = true,
                    AutoScroll = true,
                    Stateful = true,
                    StateID = "messagesGridPanel"
                };

            gridPanel.View.Add(new GroupingView
                {
                    ////ForceFit = true,
                    MarkDirty = false,
                    IgnoreAdd = true,
                    HideGroupedColumn = true,
                });

            gridPanel.SelectionModel.Add(new CheckboxSelectionModel
                {
                    KeepSelectionOnClick = KeepSelectionMode.Always
                });

            BuildToolbox(gridPanel, storeId);
            BuildColumnModel(gridPanel, page);
            BuildHeaderRow(gridPanel);
            BuildListeners(gridPanel);

            return gridPanel;
        }

        private void BuildToolbox(GridPanel gridPanel, string storeId)
        {
            var buttonRefresh = new Button
                {
                    ID = "buttonRefresh",
                    Icon = Icon.TableRefresh,
                    ToolTip = "Обновить"
                };
            buttonRefresh.Listeners.Click.Handler = "Extensions.Messages.Resive.refresh();";
            gridPanel.Toolbar().Items.Add(buttonRefresh);

            var addNewMessageButton = new Button
                {
                    ID = "addNewMessage",
                    Icon = Icon.Add,
                    ToolTip = "Добавить новое сообщение",
                    Visible = principal.IsInRole("Администраторы")
                };
            addNewMessageButton.Listeners.Click.Handler = @"Ext.getCmp('addNewMessageWindow').show()";

            gridPanel.Toolbar().Items.Add(addNewMessageButton);

            var buttonDelete = new Button
                {
                    ID = "buttonDelete",
                    ToolTip = "Удалить",
                    Icon = Icon.Delete
                };
            buttonDelete.DirectEvents.Click.CleanRequest = true;
            buttonDelete.DirectEvents.Click.Success = "Extensions.Messages.Resive.refresh();";
            buttonDelete.DirectEvents.Click.ExtraParams.Add(new Parameter
                {
                    Name = "ids",
                    Mode = ParameterMode.Raw,
                    Value = @"messagesGridPanel.hasSelection() 
? messagesGridPanel.getSelectionModel().selections.keys.toString() 
: ''
"
                });
            buttonDelete.DirectEvents.Click.Before =
                @"var r = Ext.getCmp('messagesGridPanel').hasSelection();
if (!r){
    Ext.Msg.alert('Предупреждение', 'Для удаления выделите запись.');
}
return r;";
            buttonDelete.DirectEvents.Click.Url = "/MessagesNav/DeleteMessage";

            gridPanel.Toolbar().Items.Add(buttonDelete);

            var messageReadButton = new Button
                {
                    ID = "messageButtonRead",
                    Text = "Отметить как прочитанные",
                    Icon = Icon.EmailOpen
                };
            messageReadButton.DirectEvents.Click.CleanRequest = true;
            messageReadButton.DirectEvents.Click.Success = "Extensions.Messages.Resive.refresh();";
            messageReadButton.DirectEvents.Click.ExtraParams.Add(new Parameter
                {
                    Name = "ids",
                    Mode = ParameterMode.Raw,
                    Value = @"messagesGridPanel.hasSelection() 
? messagesGridPanel.getSelectionModel().selections.keys.toString() 
: ''
"
                });
            messageReadButton.DirectEvents.Click.Before =
                @"var r = Ext.getCmp('messagesGridPanel').hasSelection();
if (!r){
    Ext.Msg.alert('Предупреждение', 'Для того, что отметить запись как прочитанную, ее необходимо выделить.');
}
return r;";
            messageReadButton.DirectEvents.Click.After =
                @"messagesGridPanel.getSelectionModel().clearSelections();";
            messageReadButton.DirectEvents.Click.Url = "/MessagesNav/MakeMessagesRead";

            gridPanel.Toolbar().Items.Add(messageReadButton);

            gridPanel.Toolbar().Items.Add(new ToolbarSeparator());

            var button = new Button
                {
                    ID = "btnToggleGroups",
                    Text = "Развернуть / свернуть группы",
                    Icon = Icon.TableSort,
                    AutoPostBack = false
                };
            button.Listeners.Click.Handler = @"Ext.getCmp('messagesGridPanel').getView().toggleAllGroups();";
            gridPanel.Toolbar().Items.Add(button);

            var tb = new PagingToolbar
                {
                    ID = "pagingToolBar{0}".FormatWith(gridPanel.ID),
                    StoreID = storeId,
                    DisplayInfo = true,
                    PageSize = PagingSettings.Size,
                    PageIndex = PagingSettings.Start,
                    BorderWidth = 0,
                    DisplayMsg = "Записи с {0} по {1} из {2}",
                    EmptyMsg = "Нет данных"
                };

            gridPanel.BottomBar.Add(tb);
        }

        private void BuildColumnModel(GridPanel gridPanel, ViewPage page)
        {
            var colSender = gridPanel.ColumnModel.AddColumn(
                "Sender",
                "Sender",
                "Отправитель",
                DataAttributeTypes.dtString,
                Mandatory.NotNull).
                SetWidth(125);
            colSender.Renderer.Fn = @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('Status') == 1){
        return '<p style=\'white-space:normal;word-wrap:break-word;\'><b>' + value + '</p></b>'}
    else {
        return '<p style=\'white-space:normal;word-wrap:break-word;\'>' + value + '</p>';}
}";
            colSender.Groupable = false;
            colSender.Tooltip = "Отправитель сообщения";

            var dateColumn = new Column
                {
                    ColumnID = "Date",
                    DataIndex = "Date",
                    Header = "Дата отправки Date",
                    Wrap = true,
                    Width = 90,
                    Groupable = false,
                    Hidden = true,
                    Sortable = false,
                    MenuDisabled = true,
                    Hideable = false
                };

            gridPanel.ColumnModel.Columns.Add(dateColumn);

            var dateGroupColumn = new Column
                {
                    ColumnID = "DateGroup",
                    DataIndex = "DateGroup",
                    Header = "Дата отправки Group",
                    Groupable = false,
                    Hidden = true,
                    GroupName = "Отправлено",
                    MenuDisabled = true,
                    Sortable = false,
                    Hideable = false
                };
            gridPanel.ColumnModel.Columns.Add(dateGroupColumn);

            var dateCaptionColumn = new Column
                {
                    ColumnID = "DateCaption",
                    DataIndex = "DateCaption",
                    Header = "Дата отправки",
                    Groupable = false,
                    Tooltip = "Дата отправки сообщения",
                    Renderer = 
                    { 
                        Fn = @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('Status') == 1) {
        return '<b>' + value + '</b>';
    }
    else {
        return value;
    }
}" 
                    }
                };
            gridPanel.ColumnModel.Columns.Add(dateCaptionColumn);

            var colbaseStat =
                gridPanel.ColumnModel.AddColumn(
                "Status",
                "Status",
                "Статус",
                DataAttributeTypes.dtString,
                Mandatory.NotNull).
                    SetWidth(80);

            colbaseStat.RendererFn(@"function (value) {{ var tpl = '<img src=""{{0}}"" ext:qtip=""{{1}}""/>'; 
    if (value == {0}) {{
        return String.format(tpl, '{1}', '{2}');
    }} else if (value == {3}) {{
        return String.format(tpl, '{4}', '{5}');
    }} else if (value == {6}) {{
        return String.format(tpl, '{7}', '{8}');
    }} else {{
        return '';
    }}
}}".FormatWith(
                Convert.ToInt32(MessageStatus.New),
                ResourceManager.GetInstance(page).GetIconUrl(Icon.EmailAdd),
                "Новое",
                Convert.ToInt32(MessageStatus.Read),
                ResourceManager.GetInstance(page).GetIconUrl(Icon.EmailOpen),
                "Прочитано",
                Convert.ToInt32(MessageStatus.Deleted),
                ResourceManager.GetInstance(page).GetIconUrl(Icon.EmailDelete),
                "Удалено"));
            colbaseStat.Groupable = false;
            colbaseStat.Tooltip = "Статус сообщения";

            var colbaseImp = gridPanel.ColumnModel.AddColumn(
                "Importance",
                "Importance",
                "Важность",
                DataAttributeTypes.dtString,
                Mandatory.NotNull).SetWidth(75);

            colbaseImp.RendererFn(@"function (value) {{ var tpl = '<img src=""{{0}}"" ext:qtip=""{{1}}""/>'; 
    if (value == {0}) {{
        return String.format(tpl, '{1}', '{2}');
    }} else if (value == {3}) {{
        return String.format(tpl, '{4}', '{5}');
    }} else if (value == {6}) {{
        return String.format(tpl, '{7}', '{8}');
    }} else if (value == {9}) {{
        return String.format(tpl, '{10}', '{11}');
    }} else {{
        return '';
    }}
}}".FormatWith(
                Convert.ToInt32(MessageImportance.HighImportance),
                ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletRed),
                "Высокая важность!",
                Convert.ToInt32(MessageImportance.Importance),
                ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletYellow),
                "Важно!",
                Convert.ToInt32(MessageImportance.Regular),
                ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletGreen),
                "Обычное",
                Convert.ToInt32(MessageImportance.Unimportant),
                ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletWhite),
                "Не важное"));
            colbaseImp.Groupable = false;
            colbaseImp.Tooltip = "Важность сообщения";

            var colSub = gridPanel.ColumnModel.AddColumn(
                "Subject",
                "Subject",
                "Тема",
                DataAttributeTypes.dtString,
                Mandatory.NotNull).
                SetWidth(400);
            colSub.Tooltip = "Тема сообщения";

            colSub.Renderer.Fn = @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('Status') == 1){
        return '<p style=\'white-space:normal;word-wrap:break-word;\'>' + value + '</p>'}
    else {
        return '<p style=\'white-space:normal;word-wrap:break-word;\'>' + value + '</p>';}
}";
            colSub.Groupable = false;

            var colMessageType = gridPanel.ColumnModel.AddColumn(
                "MessageType",
                "MessageType",
                "Тип",
                DataAttributeTypes.dtString,
                Mandatory.NotNull)
                .SetWidth(45);
            colMessageType.Groupable = false;
            colMessageType.Tooltip = "Тип сообщения";
            colMessageType.Renderer.Fn = @"function(value) {{ var tpl = '<img src=""{{0}}"" ext:qtip=""{{1}}""/>';
    if (value == {0}) {{
        return String.format(tpl, '{1}', '{2}');
    }} else if (value == {3}) {{
        return String.format(tpl, '{4}', '{5}');
    }} else if (value == {6}) {{
        return String.format(tpl, '{7}', '{8}');
    }} else if (value == {9}) {{
        return String.format(tpl, '{10}', '{11}');
    }} else if (value == {12}) {{
        return String.format(tpl, '{13}', '{14}');
    }}
}}".FormatWith(
   Convert.ToInt32(MessageType.AdministratorMessage),
   ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletKey),
   "Сообщения от администратора системы",
   Convert.ToInt32(MessageType.CubesManagerMessage),
   ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletDatabaseYellow),
   "Сообщения от интерфейса расчета кубов",
   Convert.ToInt32(MessageType.PumpMessage),
   ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletGet),
   "Сообщения от подсистемы закачек",
   Convert.ToInt32(MessageType.ForecastMessage),
   ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletStart),
   "Сообщения от блока прогнозирование",
   Convert.ToInt32(MessageType.ConsolidationMessage),
   ResourceManager.GetInstance(page).GetIconUrl(Icon.BulletEdit),
   "Сообщение от форм сбора");

            var attachmentColumn = new Column
            {
                ColumnID = "RefMessageAttachment",
                DataIndex = "RefMessageAttachment",
                Sortable = false,
                MenuDisabled = true,
                Fixed = true,
                Groupable = false,
                Hideable = false,
                Hidden = true
            };
            gridPanel.ColumnModel.Columns.Add(attachmentColumn);

            var imageCommandColumn = new ImageCommandColumn
                {
                    ColumnID = "imageCommandColumn",
                    Width = 25,
                    Sortable = true,
                    MenuDisabled = true,
                    Fixed = true,
                    Groupable = false,
                    Hideable = false,
                    Tooltip = "Вложение",
                    Header = String.Format("<p style=\'font-size:16px\';>{0}</p>", ((char)9993).ToString()),
                };

            var imageCommand = new ImageCommand
                {
                    CommandName = "openAttachment",
                    Icon = Icon.Attach,
                    Text = String.Empty
                };

            imageCommandColumn.PrepareCommand.Fn = @"function(grid, command, record, row) {
if (!record.data.RefMessageAttachment) {
    command.hidden = true;
    command.hideMode = 'visibility';
}
}";

            imageCommandColumn.Commands.Add(imageCommand);
            gridPanel.ColumnModel.Columns.Add(imageCommandColumn);

            var transferCommandColumn = new ImageCommandColumn
                {
                    ColumnID = "transferCommandColumn",
                    Width = 25,
                    Sortable = true,
                    Fixed = true,
                    Tooltip = "Перейти к соответствующему интерфейсу", 
                    Header = String.Format("<p style=\'font-size:16px\';>{0}</p>", ((char)8679).ToString()),
                };
            var transferCommand = new ImageCommand
                {
                    CommandName = "transferCommand",
                    Icon = Icon.BulletGo,
                    Text = String.Empty
                };
            transferCommandColumn.PrepareCommand.Fn = @"function(grid, command, record, row) {
if (!record.data.TransferLink ||
    /[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}/.test(record.data.TransferLink) ||
        Extensions.Messages.Resive.isNumber(record.data.TransferLink)) {
        command.hidden = true;
        command.hideMode = 'visibility';
    }
command.qtitle = 'Перейти';
}
";
            transferCommandColumn.Commands.Add(transferCommand);
            gridPanel.ColumnModel.Columns.Add(transferCommandColumn);
                
            var column = new Column
                {
                    ColumnID = "ClFilter",
                    Header = "&nbsp;",
                    DataIndex = "Status",
                    Width = 28,
                    Sortable = false,
                    MenuDisabled = true,
                    Fixed = true,
                    Renderer = { Handler = @"return '';" },
                    Groupable = false,
                    Hideable = false,
                };
            gridPanel.ColumnModel.Columns.Add(column);

            var commandColumn = new CommandColumn
                {
                    Hidden = true
                };
            var gridCommand = new GridCommand
                {
                    Icon = Icon.TableRow,
                    CommandName = "SelectGroup",
                    ToolTip = { Text = "Выделить все сообщения в группе" }
                };
            commandColumn.GroupCommands.Add(gridCommand);
            gridPanel.ColumnModel.Columns.Add(commandColumn);

            gridPanel.AddScript(@"
var commandHandlerFiles = function (command, rowIndex, record) {
    switch (command) {
        case 'openAttachment':
            Ext.net.DirectMethod.request({
                url: '/MessagesNav/DownloadAttachment',
                isUpload: true,
                formProxyArg: 'messagesFormPanel',
                cleanRequest: true,
                params: {
                    attachmentId: record.data.RefMessageAttachment
                },
                success: successSaveHandler,
                failure: failureSaveHandler
            });
            break;
       case 'transferCommand':
        eval(record.data.TransferLink);
        break;
    }
}");

            gridPanel.AddScript(@"
function successSaveHandler(response, result) {
    if (result.extraParams != undefined && result.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: result.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });
    } else if (response.extraParams != undefined && response.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: response.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });

    } else {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: 'Ок',
            title: 'Уведомление',
            hideDelay: 2500
        });
    }
};

function failureSaveHandler(response, result) {
    if (result.extraParams != undefined && result.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', result.extraParams.responseText);
    } else {
        var responseParams = Ext.decode(result.responseText);
        if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
            Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
        } else {
            Ext.Msg.alert('Ошибка', 'Server failed');
        }
    }
};
");

            gridPanel.Listeners.Command.Handler = "commandHandlerFiles(command, rowIndex, record)";
        }

        private void BuildHeaderRow(GridPanel gridPanel)
        {
            GridView view = gridPanel.View[0];

            var headerRow = new HeaderRow();
            var headerColumnStatus = new HeaderColumn
                {
                    Cls = "x-small-editor"
                };
            var headerColumnCheck = new HeaderColumn();
            var headerColumnDate = new HeaderColumn();
            var headerColumnDateGroup = new HeaderColumn();
            var headerColumnDateCaption = new HeaderColumn();
            var headerColumnFrom = new HeaderColumn();
            var headerColumnImportance = new HeaderColumn();
            var headerColumnSubject = new HeaderColumn();
            var headerColumnType = new HeaderColumn();
            var headerColumnAttachment = new HeaderColumn
            {
                AutoWidthElement = false
            };
            var headerColumnTransfer = new HeaderColumn
            {
                AutoWidthElement = false
            };
            var headerColumnAttachmentImage = new HeaderColumn();
            var headerColumnClear = new HeaderColumn
                {
                    AutoWidthElement = false
                };

            var comboboxFrom = new ComboBox
                {
                    ID = "ComboBoxSender",
                    TriggerAction = TriggerAction.All,
                    Mode = DataLoadMode.Local,
                    StoreID = "StoreMessageSender",
                    ValueField = "Sender",
                    DisplayField = "Sender",
                    Resizable = true,
                    Editable = false,
                    ListWidth = 200,
                    Triggers = { new FieldTrigger { Icon = TriggerIcon.Clear, HideTrigger = true } }
                };
            SetComboBoxListeners(comboboxFrom);
            headerColumnFrom.Component.Add(comboboxFrom);

            var dateField = new DateField
            {
                ID = "DateFilter",
                Editable = false,
                Triggers = { new FieldTrigger { Icon = TriggerIcon.Clear, HideTrigger = true } }
            };
            dateField.Listeners.Select.Handler = @"this.triggers[0].show(); Extensions.Messages.Resive.applyFilter(this);";
            dateField.Listeners.TriggerClick.Handler = @"if (index == 0) { this.reset(); this.triggers[0].hide(); Extensions.Messages.Resive.applyFilter(this); }";
            headerColumnDateCaption.Component.Add(dateField);

            var comboboxStatus = new ComboBox
                {
                    ID = "ComboBoxStatus",
                    TriggerAction = TriggerAction.All,
                    Mode = DataLoadMode.Local,
                    ValueField = "Status",
                    StoreID = "StoreStatus",
                    DisplayField = "StatusCaption",
                    Triggers = { new FieldTrigger { Icon = TriggerIcon.Clear, HideTrigger = true } },
                };
            SetComboBoxListeners(comboboxStatus);

            headerColumnStatus.Component.Add(comboboxStatus);

            var comboboxType = new ComboBox
                {
                    ID = "ComboBoxType",
                    TriggerAction = TriggerAction.All,
                    Mode = DataLoadMode.Local,
                    ValueField = "MessageType",
                    StoreID = "StoreMessageType",
                    DisplayField = "MessageTypeCaption",
                    ListWidth = 200,
                    Editable = false,
                    Resizable = true,
                    Triggers = { new FieldTrigger { Icon = TriggerIcon.Clear, HideTrigger = true } }
                };

            SetComboBoxListeners(comboboxType);
            headerColumnType.Component.Add(comboboxType);

            var comboboxImportance = new ComboBox
                {
                    ID = "ComboBoxImportance",
                    TriggerAction = TriggerAction.All,
                    Mode = DataLoadMode.Local,
                    ValueField = "Importance",
                    StoreID = "StoreMessageImportance",
                    DisplayField = "ImportanceCaption",
                    ListWidth = 120,
                    Triggers = { new FieldTrigger { Icon = TriggerIcon.Clear, HideTrigger = true } }
                };
            SetComboBoxListeners(comboboxImportance);
            headerColumnImportance.Component.Add(comboboxImportance);

            var subjectTextField = new TextField
                {
                    ID = "SubjectTextField",
                    EnableKeyEvents = true
                };
            subjectTextField.Listeners.KeyUp.Handler = "Extensions.Messages.Resive.applyFilter(this);";
            headerColumnSubject.Component.Add(subjectTextField);

            var buttonClear = new Button
                {
                    ID = "buttonClear",
                    Icon = Icon.Cancel
                };
            var toolTipClear = new ToolTip
                {
                    Html = "Очистить фильтр"
                };
            buttonClear.ToolTips.Add(toolTipClear);
            buttonClear.Listeners.Click.Handler = @"
Ext.getCmp('ComboBoxStatus').triggers[0].hide();
Ext.getCmp('ComboBoxType').triggers[0].hide();
Ext.getCmp('ComboBoxSender').triggers[0].hide();
Ext.getCmp('ComboBoxImportance').triggers[0].hide();
Ext.getCmp('DateFilter').triggers[0].hide();
Extensions.Messages.Resive.clearFilter()";
            headerColumnClear.Component.Add(buttonClear);

            headerRow.Columns.AddRange(new[]
                {
                    headerColumnCheck,
                    headerColumnFrom,
                    headerColumnDate,
                    headerColumnDateGroup,
                    headerColumnDateCaption,
                    headerColumnStatus,
                    headerColumnImportance,
                    headerColumnSubject,
                    headerColumnType,
                    headerColumnAttachment,
                    headerColumnAttachmentImage,
                    headerColumnTransfer,
                    headerColumnClear
                });
            view.HeaderRows.Add(headerRow);
        }

        private void SetComboBoxListeners(ComboBox combobox)
        {
            combobox.Listeners.Select.Handler =
                @"this.triggers[0].show(); Extensions.Messages.Resive.applyFilter(this);";
            combobox.Listeners.TriggerClick.Handler =
                @"if (index == 0) { this.clearValue(); this.triggers[0].hide(); Extensions.Messages.Resive.applyFilter(this); }";
            combobox.Listeners.BeforeQuery.Handler =
                @"this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();";
        }

        private void BuildListeners(GridPanel gridPanel)
        {
            gridPanel.Listeners.GroupCommand.Handler =
                @"if(command === 'SelectGroup'){ this.getSelectionModel().selectRecords(records, true); return;};";
        }

        #endregion
    }
}