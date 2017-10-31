using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controls
{
    public class CommentsGridControl : GridPanel
    {
        private readonly ViewPage page;
        private readonly int categoryId;
        private readonly bool editable;
        private readonly string userName;
        private readonly string stateName;
        private Window window;

        public CommentsGridControl(ViewPage page, int categoryId, bool editable, string userName, string stateName)
        {
            this.page = page;
            this.categoryId = categoryId;
            this.editable = editable;
            this.userName = userName;
            this.stateName = stateName;

            InitializeWindow();
        }

        public Window GetAddCommentWindow()
        {
            return window;
        }

        public void InitAll()
        {
            ID = "CommentsPanel_{0}".FormatWith(categoryId);
            StoreID = "dsComments{0}".FormatWith(categoryId);
            Title = @"Список комментариев";
            Height = 200;
            Flex = 1;
            LabelAlign = LabelAlign.Top;
            AutoScroll = true;
            Collapsible = true;
            AnimCollapse = true;
            TrackMouseOver = true;
            Icon = Icon.Comments;
            LoadMask.ShowMask = true;

            TopBar.Add(
                new Toolbar
                {
                    Items =
                            {
                                new Button
                                    {
                                        ID = "btnAddComment",
                                        Icon = Icon.Add,
                                        Text = @"Добавить",
                                        ToolTip = @"Добавить новый комментарий",
                                        Listeners =
                                            {
                                                Click =
                                                    {
                                                        Handler = "wAddCommentDialog.show('{0}');".FormatWith(ID)
                                                    }
                                    },
                                    Disabled = !editable,
                                    Enabled = editable
                                }
                        }
                });

            ColumnModel.AddColumn("Text", "Комментарий", DataAttributeTypes.dtString).MenuDisabled = true;
            ColumnModel.AddColumn("StateName", "Статус", DataAttributeTypes.dtString).MenuDisabled = true;
            ColumnModel.AddColumn("Executor", "Кто", DataAttributeTypes.dtString).MenuDisabled = true;
            ColumnModel.AddColumn("NoteDate", "Дата", DataAttributeTypes.dtString).MenuDisabled = true;

            AutoExpandColumn = "Text";
            AutoScroll = true;
            this.AddColumnsWrapStylesToPage(page);

            ColumnModel.Columns.Add(
                new ImageCommandColumn
                {
                    Commands =
                        {
                            new ImageCommand
                                {
                                    Icon = Icon.Delete,
                                    CommandName = "deleteComment",
                                    ToolTip = { Text = "Удалить" },
                                    Hidden = !editable
                                }
                        }
                });

            SelectionModel.Add(new RowSelectionModel { SingleSelect = true });
            Listeners.Command.Handler = "commandHandlerCommants(command, rowIndex, record)";

            AddScript(@"
var commandHandlerCommants = function (command, rowIndex, record) {{
    switch (command) {{
        case 'deleteComment':
            Ext.Msg.confirm('Предупреждение', 'Удалить комментарий?', function (btn) {{
                if (btn == 'yes') {{
                    Ext.getCmp('{0}').store.removed.push(Ext.getCmp('{0}').store.getAt(rowIndex));
                    Ext.getCmp('{0}').store.removeAt(rowIndex);
                }}
            }}
            );
            break;
    }}
}};
".FormatWith(ID));

            View.Add(
                new GridView
                {
                    ID = "GridViewComments",
                    ForceFit = true,
                    MarkDirty = true,
                });
        }

        private void InitializeWindow()
        {
            var win = new Window
            {
                ID = "wAddCommentDialog",
                Title = @"Форма добавления комментария",
                Hidden = true,
                Modal = true,
                Width = 500,
                AutoHeight = true,
                Resizable = false,
                Border = false
            };
            var form = new FormPanel();
            win.Add(form);

            form.ID = "fAddCommentDialog";
            form.Frame = true;
            form.MonitorValid = true;
            form.Border = false;
            form.BodyBorder = false;
            form.BodyStyle = "padding: 10px 10px 0 10px;background:none repeat scroll 0 0 #DFE8F6;";

            form.Defaults.Add(new Parameter { Name = "anchor", Value = "95%", Mode = ParameterMode.Value });
            form.Defaults.Add(new Parameter { Name = "msgTarget", Value = "side", Mode = ParameterMode.Value });

            form.Items.Add(new TextArea
            {
                ID = "Text",
                Width = 700,
                Height = 40,
                AllowBlank = true,
                FieldLabel = @"Текст комментария"
            });

            var addHandler =
                   @"
                        var text = Text.getValue();
                        if (text != '') {{
                            dv = {{}};
                            dv['Text'] = text;
                            var date = new Date();
                            var d = date.getDate();
                            var m = date.getMonth();
                            var y = date.getFullYear();
                            dv['NoteDate'] = ((d < 10) ? '0' : '') + d + '.' + 
                                    ((m < 10) ? '0' : '') + (m + 1) + '.' + date.getFullYear();
                            dv['Executor'] = '{1}';
                            dv['StateName'] = '{2}';
                            dv.phantom = true;
                            /*формируем новую запись*/
                            var myNewRecord = new CommentsPanel_{0}.store.recordType(dv, tempId);
                            tempId--;
                            /*добавляем новую запись к выбранному узлу*/
                            CommentsPanel_{0}.store.add(myNewRecord);
                            CommentsPanel_{0}.store.modified.push(myNewRecord);
                            #{{fAddCommentDialog}}.getForm().reset();
                            wAddCommentDialog.hide();
                        }}
                        ".FormatWith(categoryId, userName, stateName ?? string.Empty);

            form.Buttons.Add(new Button
            {
                ID = "requestCommentsButton",
                Width = 50,
                Text = @"Сохранить",
                Listeners = { Click = { Handler = addHandler } }
            });

            form.Buttons.Add(new Button
            {
                ID = "btResetAdd",
                Text = @"Сброс",
                Listeners = 
                { 
                    Click = 
                    { 
                        Handler = @"#{fAddCommentDialog}.getForm().reset(); wAddCommentDialog.hide();" 
                    } 
                }
            });

            window = win;
        }
    }
}
