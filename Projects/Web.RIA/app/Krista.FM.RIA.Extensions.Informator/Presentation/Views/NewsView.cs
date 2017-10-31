using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.Informator.Presentation.Views
{
    public class NewsView : View
    {
        private const string Scope = "Informator.View.News.Grid";

        public ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            const string Style = @".new_message-row {
            font-weight: bolder;

            }
            .read_message-row {
            font-weight: bold;

            }
            .delete_message-row {
            font-weight: bold;

            }
       
";
            ResourceManager.GetInstance(page).RegisterClientStyleBlock("CustomStyle", Style);

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("NewsView.js", Resource.NewsView);

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.PUT;
            restActions.Destroy = HttpMethod.DELETE;

            var panel = new GridPanel
            {
                ID = "News",
                Store = { GetStore() },
                LoadMask = { ShowMask = true },
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                BottomBar =
                                    {
                                        new PagingToolbar
                                            {
                                                ID = "paggingNews",
                                                StoreID = "dsinbox",
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
                                                        new BooleanFilter { DataIndex = "new_message" },
                                                        new BooleanFilter { DataIndex = "read_message" },
                                                        new BooleanFilter { DataIndex = "exclamation_message" },
                                                        new BooleanFilter { DataIndex = "regular_message" },
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
                                                                DataIndex = FieldsId.Sender,
                                                                EmptyText = "Отправитель",
                                                            },
                                                    },
                                            },
                                    },
            };

            panel.ColumnModel.Columns.Add(
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
                    PrepareCommand = { Fn = Scope + ".prepareCommand" }
                });

            panel.ColumnModel.Columns.Add(
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
                    PrepareCommand = { Fn = Scope + ".prepareCommand" }
                });

            panel.ColumnModel.AddColumn(FieldsId.ID, "ID", DataAttributeTypes.dtInteger).SetWidth(100).SetHidden(true);
            panel.ColumnModel.AddColumn(FieldsId.Attachment, "Вложение", DataAttributeTypes.dtBoolean).SetWidth(100).SetHidden(true);
            panel.ColumnModel.AddColumn(FieldsId.Date, "Дата", DataAttributeTypes.dtDateTime).SetWidth(150);
            var stateColumn = new Column
            {
                ColumnID = FieldsId.State,
                DataIndex = FieldsId.State,
                Header = "Статус",
                Groupable = false,
                Tooltip = "Статус",
                Renderer =
                {
                    Fn =
                        @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('MessageStatus') == 'Новая') {
        return '<b>' + value + '</b>';
    }
    else {
        return value;
    }
}"
                }
            };
            panel.ColumnModel.Columns.Add(stateColumn);
            var subjectColumn = new Column
            {
                ColumnID = FieldsId.Subject,
                DataIndex = FieldsId.Subject,
                Header = "Тема",
                Groupable = false,
                Tooltip = "Тема",
                Width = 300,
                Wrap = true,
                Renderer =
                {
                    Fn =
                        @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('MessageStatus') == 'Новая') {
        return '<b>' + value + '</b>';
    }
    else {
        return value;
    }
}"
                }
            };
            panel.ColumnModel.Columns.Add(subjectColumn);
            var senderColumn = new Column
            {
                ColumnID = FieldsId.Sender,
                DataIndex = FieldsId.Sender,
                Width = 300,
                Header = "Отправитель",
                Groupable = false,
                Tooltip = "Отправитель",
                Renderer =
                {
                    Fn =
                        @"function(value,metadata,record,rowIndex,colIndex,store) {
    if (record.get('MessageStatus') == 'Новая') {
        return '<b>' + value + '</b>';
    }
    else {
        return value;
    }
}"
                }
            };
            panel.ColumnModel.Columns.Add(senderColumn);
            panel.AddColumnsHeaderAlignStylesToPage(page);
            panel.AddColumnsWrapStylesToPage(Page);

            panel.Listeners.CellClick.Fn = Scope + ".actionHandler1";

            panel.Toolbar().Add(new Button
            {
                ID = "btnNewsRefresh",
                Icon = Icon.EmailTransfer,
                ToolTip = "Обновить данные",
                Listeners = { Click = { Handler = "News.reload();" } },
            });
            panel.SelectionModel.Add(new CheckboxSelectionModel
            {
                ID = "checkSel",
            });

            var filterNew = new Button
            {
                ID = "filterNew",
                Icon = Icon.Email,
                ToolTip = "Отобрать все непрочитанные новости",
                EnableToggle = true,
            };
            filterNew.SetHandler("Informator.View.News.Grid.toggleFilter", Scope);
            var filterRead = new Button
            {
                ID = "filterRead",
                Icon = Icon.EmailOpen,
                ToolTip = "Отобрать все прочитанные новости",
                EnableToggle = true,
            };
            filterRead.SetHandler("Informator.View.News.Grid.toggleFilter", Scope);

            var filterExclamation = new Button
            {
                ID = "filterExclamation",
                Icon = Icon.Exclamation,
                ToolTip = "Отобрать важные новости",
                EnableToggle = true,
            };
            filterExclamation.SetHandler("Informator.View.News.Grid.toggleFilter", Scope);

            var filterRegular = new Button
            {
                ID = "filterRegular",
                Icon = Icon.Feed,
                ToolTip = "Отобрать обычные новости",
                EnableToggle = true,
            };
            filterRegular.SetHandler("Informator.View.News.Grid.toggleFilter", Scope);

            panel.Toolbar().Add(filterNew);
            panel.Toolbar().Add(filterRead);
            panel.Toolbar().Add(filterExclamation);
            panel.Toolbar().Add(filterRegular);

            var filters = new GridFilters { Local = true };
            filters.Filters.Add(new NumericFilter { DataIndex = "ID" });

            panel.AddColumnsWrapStylesToPage(page);

            panel.View.Add(new GridView
            {
                EmptyText = "<center>Нет доступных новостей</center>"
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
                                                   Center = { Items = { panel }, Margins = { Bottom = 3 } }
                                               },
                                       }
                               },
                       };
        }

        private static Store GetStore()
        {
            var store = new Store
            {
                ID = "dsinbox",
                Restful = true,
                AutoLoad = true,
                ShowWarningOnFailure = true,
                WarningOnDirty = false,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };
            store
                .SetRestController("News/Read").SetJsonReader()
                .AddField(FieldsId.ID)
                .AddField(FieldsId.Date)
                .AddField(FieldsId.State)
                .AddField(FieldsId.Importance)
                .AddField(FieldsId.Subject)
                .AddField(FieldsId.Sender)
                .AddField(FieldsId.Attachment)
                .AddField(FieldsId.DocumentFileName)
                ////.AddField(FieldsId.Document)
                .AddField(FieldsId.RefMessageAttachment)
                .AddField("new_message")
                .AddField("read_message");
            store.SetBaseParams("type", "inbox", ParameterMode.Value);
            store.BaseParams.Add(new Parameter("limit", "pageSizeNews.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));

            return store;
        }

        #region Nested type: FieldsId

        private static class FieldsId
        {
            public const string ID = "ID";
            public const string Date = "ReceivedDate";
            public const string State = "MessageStatus";
            public const string Importance = "MessageImportance";
            public const string Subject = "Subject";
            public const string Sender = "RefUserSender";
            public const string RefMessageAttachment = "RefMessageAttachment";
            public const string Attachment = "MessageAttachment";
            ////public const string Document = "Document";
            public const string DocumentFileName = "DocumentFileName";
        }

        #endregion
    }
}