using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.Informator.Presentation.Controls;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.Informator.Presentation.Views
{
    public class OutboxNewsView : View
    {
        private const string Scope = "Informator.View.News.Grid";

        public ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("NewsView.js", Resource.NewsView);

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.PUT;
            restActions.Destroy = HttpMethod.DELETE;

            var sendMessage = new SendMessage();

            var sendBtn = new Button
            {
                ID = "btnWrite",
                Icon = Icon.EmailAdd,
                ToolTip = "Опубликовать новость"
            };
            sendBtn.Listeners.Click.Handler = @"Ext.getCmp('Window').show()";
            sendBtn.DirectEvents.Click.CleanRequest = true;
            sendBtn.DirectEvents.Click.IsUpload = true;
            sendBtn.DirectEvents.Click.FormID = "Form1";
            sendBtn.DirectEvents.Click.Failure =
                @"Ext.Msg.show({
                                                           title:'Ошибка',
                                                           msg: response.responseText,
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.ERROR,
                                                           maxWidth: 1000
                                                        });";

            var filters = new GridFilters { Local = true };
            filters.Filters.Add(new NumericFilter { DataIndex = "ID" });

            var sendPanel = new GridPanel
            {
                ID = "outboxNews",
                Store = { GetStore() },
                LoadMask = { ShowMask = true },
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                BottomBar =
                                    {
                                        new PagingToolbar
                                            {
                                                ID = "paggingNews",
                                                StoreID = "dsoutbox",
                                                PageSize = 200,
                                                Items =
                                                    {
                                                        new NumberField
                                                            {
                                                                ID = "pageSizeNews",
                                                                FieldLabel = "Новостей на страницу",
                                                                LabelWidth = 150,
                                                                Width = 200,
                                                                Number = 200,
                                                                Listeners =
                                                                    {
                                                                        Change =
                                                                            {
                                                                                Handler =
                                                                                    "#{paggingNews}.pageSize = parseInt(this.getValue());"
                                                                            }
                                                                    }
                                                            },
                                                    },
                                            },
                                    },
                Plugins =
                {
                    new GridFilters
                        {
                            Local = false,
                            Filters =
                                {
                                    new BooleanFilter { DataIndex = "Прочитана" },
                                    new BooleanFilter { DataIndex = "Новая" },
                                    new BooleanFilter { DataIndex = "Удалена" },
                                    new BooleanFilter { DataIndex = "exclamation_message" },
                                    new BooleanFilter { DataIndex = "regular_message" },
                                    new DateFilter
                                        {
                                            DataIndex = FieldsId.Date,
                                        },
                                    new ListFilter
                                        {
                                            DataIndex = FieldsId.State,
                                            Options =
                                                new[]
                                                    {
                                                        "Новая",
                                                        "Прочитана"
                                                        }
                                        },
                                    new ListFilter
                                    {
                                        DataIndex = FieldsId.Importance,
                                        Options =
                                            new[]
                                                {
                                                    "Важная",
                                                    "Обычная"
                                                },
                                    },
                                    new BooleanFilter { DataIndex = FieldsId.Attachment },
                                    new StringFilter
                                        {
                                            DataIndex = FieldsId.Recipient,
                                            EmptyText = "Получатель",
                                        },
                                },
                        },
                },
            };

            sendPanel.Items.Add(sendMessage.Build(page));

            sendPanel.Toolbar().Add(sendBtn);

            sendPanel.Toolbar().Add(new Button
            {
                ID = "btnSendNewsRefresh",
                Icon = Icon.EmailTransfer,
                ToolTip = "Обновить данные",
                Listeners = { Click = { Handler = "outboxNews.reload();" } },
            });

            var delBtn = new Button
            {
                ID = "btnDelete",
                Icon = Icon.EmailDelete,
                ToolTip = "Удалить новость"
            };
            delBtn.DirectEvents.Click.Url = "/News/DeleteMessage";
            delBtn.DirectEvents.Click.CleanRequest = true;
            delBtn.DirectEvents.Click.IsUpload = true;
            delBtn.DirectEvents.Click.FormID = "Form1";
            delBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("id", "Informator.View.News.Grid.getIds('{0}')".FormatWith("outboxNews"), ParameterMode.Raw));
            delBtn.DirectEvents.Click.Success = @"outboxNews.reload();";
            delBtn.DirectEvents.Click.Failure =
                @"Ext.Msg.show({
                                                           title:'Ошибка',
                                                           msg: result.responseText,
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.ERROR,
                                                           maxWidth: 1000
                                                        });";

            sendPanel.Toolbar().Add(delBtn);

            var filterExclamation = new Button
            {
                ID = "filterExclamation",
                Icon = Icon.Exclamation,
                ToolTip = "Отобрать важные новости",
                EnableToggle = true,
            };
            filterExclamation.SetHandler("Informator.View.News.Grid.sendtoggleFilter", Scope);

            var filterRegular = new Button
            {
                ID = "filterRegular",
                Icon = Icon.Feed,
                ToolTip = "Отобрать обычные новости",
                EnableToggle = true,
            };
            filterRegular.SetHandler("Informator.View.News.Grid.sendtoggleFilter", Scope);

            sendPanel.Toolbar().Add(filterExclamation);
            sendPanel.Toolbar().Add(filterRegular);

            sendPanel.ColumnModel.Columns.Add(
                new ImageCommandColumn
                {
                    DataIndex = "Action",
                    Width = 60,
                    Commands =
                    {
                        new ImageCommand
                        {
                            Icon = Icon.EmailGo,
                            CommandName = "OpenNews",
                            ToolTip = { Text = "Открыть новость" },
                        },
                        new ImageCommand
                        {
                            Icon = Icon.Attach,
                            CommandName = "OpenAttach",
                            ToolTip = { Text = "Открыть вложение" },
                        }
                    },
                    PrepareCommand = { Fn = Scope + ".prepareCommand", }
                });

            sendPanel.ColumnModel.Columns.Add(
                new ImageCommandColumn
                {
                    DataIndex = "Action",
                    ColumnID = "Важная новость",
                    Width = 40,
                    Commands =
                            {
                                new ImageCommand
                                {
                                    Icon = Icon.Exclamation,
                                    CommandName = "ImpNews",
                                    ToolTip = { Text = "Важная новость" },
                                },
                            },

                    PrepareCommand = { Fn = Scope + ".prepareCommand", }
                });

            sendPanel.ColumnModel.AddColumn(FieldsId.ID, "ID", DataAttributeTypes.dtInteger).SetWidth(100).SetHidden(true);
            sendPanel.ColumnModel.AddColumn(FieldsId.Attachment, "Вложение", DataAttributeTypes.dtBoolean).SetWidth(100).SetHidden(true);
            sendPanel.ColumnModel.AddColumn(FieldsId.Date, "Дата", DataAttributeTypes.dtDateTime).SetWidth(150);
            var stateColumn = new Column
            {
                ColumnID = FieldsId.State,
                DataIndex = FieldsId.State,
                Header = "Статус",
                Groupable = false,
                Tooltip = "Статус",
                Renderer =
                {
                    Fn = @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('MessageStatus') == 'Новая') {
        return '<b>' + value + '</b>';
    }
    else {
        return value;
    }
}"
                }
            };
            sendPanel.ColumnModel.Columns.Add(stateColumn);
            var subjectColumn = new Column
            {
                ColumnID = FieldsId.Subject,
                DataIndex = FieldsId.Subject,
                Header = "Тема",
                Width = 300,
                Groupable = false,
                Tooltip = "Тема новости",
                Wrap = true,
                Renderer =
                {
                    Fn = @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('MessageStatus') == 'Новая') {
        return '<b>' + value + '</b>';
    }
    else {
        return value;
    }
}"
                }
            };
            sendPanel.ColumnModel.Columns.Add(subjectColumn);
            var recipColumn = new Column
            {
                ColumnID = FieldsId.Recipient,
                DataIndex = FieldsId.Recipient,
                Header = "Получатели",
                Width = 300,
                Groupable = false,
                Tooltip = "Получатели",
                Renderer =
                {
                    Fn = @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('MessageStatus') == 'Новая') {
        return '<b>' + value + '</b>';
    }
    else {
        return value;
    }
}"
                }
            };
            sendPanel.ColumnModel.Columns.Add(recipColumn);

            sendPanel.SelectionModel.Add(new CheckboxSelectionModel
            {
                ID = "checkSel",
            });

            sendPanel.Listeners.CellClick.Fn = Scope + ".actionHandler1";

            sendPanel.View.Add(new GridView
            {
                EmptyText = "<center>Нет опубликованных новостей<center>"
            });

            return new List<Component>
                       {
                           new Viewport
                               {
                                   ID = "viewportMain",
                                   Items =
                                       {
                                           new BorderLayout
                                               {
                                                   Center = { Items = { sendPanel }, Margins = { Bottom = 3 } },
                                               },
                                           }
                               },
                       };
        }

        private static Store GetStore()
        {
            var store = new Store
            {
                ID = "dsoutbox",
                Restful = true,
                AutoLoad = true,
                ShowWarningOnFailure = true,
                WarningOnDirty = false,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };
            store.Proxy.Add(new HttpProxy { Url = "/News/Read", Method = HttpMethod.GET, Timeout = 50000000 });
            store
                .AddField(FieldsId.ID)
                .AddField(FieldsId.Date)
                .AddField(FieldsId.State)
                .AddField(FieldsId.Importance)
                .AddField(FieldsId.Subject)
                .AddField(FieldsId.Recipient)
                .AddField(FieldsId.Attachment)
                .AddField(FieldsId.RefMessageAttachment)
                .AddField(FieldsId.DocumentFileName)
                .AddField("new_message")
                .AddField("read_message");
            var jsonReader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
            };
            jsonReader.Fields.Add(new RecordField(FieldsId.ID));
            jsonReader.Fields.Add(new RecordField(FieldsId.Date));
            jsonReader.Fields.Add(new RecordField(FieldsId.State));
            jsonReader.Fields.Add(new RecordField(FieldsId.Importance));
            jsonReader.Fields.Add(new RecordField(FieldsId.Subject));
            jsonReader.Fields.Add(new RecordField(FieldsId.Recipient));
            jsonReader.Fields.Add(new RecordField(FieldsId.Attachment));
            jsonReader.Fields.Add(new RecordField(FieldsId.DocumentFileName));

            store.Reader.Add(jsonReader);
            store.SetBaseParams("type", "outbox", ParameterMode.Value);
            store.BaseParams.Add(new Parameter("limit", "pageSizeNews.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));
            return store;
        }

        private static class FieldsId
        {
            public const string ID = "ID";
            public const string Date = "ReceivedDate";
            public const string State = "MessageStatus";
            public const string Importance = "MessageImportance";
            public const string Subject = "Subject";
            public const string Recipient = "RefUserRecipient";
            public const string RefMessageAttachment = "RefMessageAttachment";
            public const string Attachment = "MessageAttachment";
            public const string DocumentFileName = "DocumentFileName";
        }
    }
}
