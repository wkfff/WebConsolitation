using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Control = System.Web.UI.Control;
using Panel = Ext.Net.Panel;
using Parameter = Ext.Net.Parameter;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class EstimateDetailView : View
    {
        private readonly IFO41Extension extension;

        /// <summary>
        /// Стиль контролов
        /// </summary>
        private const string StyleAll = "margin: 0px 0px 5px 10px; font-size: 12px;";

        public EstimateDetailView(IFO41Extension extension)
        {
            this.extension = extension;
        }
        
        /// <summary>
        /// Модель заявки от ОГВ
        /// </summary>
        public DetailsEstimateModel DetailsViewModel { get; set; }

        /// <summary>
        /// ИСполнитель (пользователь)
        /// </summary>
        public string Executor { get; set; }

        /// <summary>
        /// Идентификатор store для документов, для сохранения в предке
        /// </summary>
        public string DocsStoreId { get; set; }

        /// <summary>
        /// Признак, можно ли редактировать
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// Признак, можно ли добавлять комментарии и документы, если заявка не редактируема (Editable = false)
        /// </summary>
        public bool CanAddCommentsAndDocs { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            // Store для реквизитов заявки от ОГВ
            page.Controls.Add(GetRequestDetailStore(DetailsViewModel));

            // Store для комментариев от ОГВ
            page.Controls.Add(GetCommentsStore());

            // панель с реквизитами и комментариями
            var content = new FormPanel
            {
                ID = "DetailsForm{0}".FormatWith(DetailsViewModel.ID),
                Border = false,
                Url = "/FO41Estimate/Save?applicaionId={0}".FormatWith(DetailsViewModel.ID),
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                Layout = "RowLayout"
            };

            content.AddScript(@"
            var SaveAppFromOGV = function(state, tab) {{
                DetailsForm{0}.form.submit({{
                    waitMsg:'Сохранение...', 
                    success: function(form, action) {{
                        successSaveHandler(form, action);
                        if ((tab != null) && (tab != undefined)) {{
                            tab.forceClose = true; 
                            tab.ownerCt.closeTab(tab);
                        }}
                    }}, 
                    failure: failureSaveHandler, 
                    params: {{
                        OGVID : '{1}', 
                        categoryId: '{2}', 
                        stateOGV : state == null ? '{3}' : state, 
                        date: '{4}' 
                    }}
                }});
            }}
            ".FormatWith(
                DetailsViewModel.ID, 
                DetailsViewModel.OGVID, 
                DetailsViewModel.CategoryID, 
                DetailsViewModel.StateOGV, 
                DetailsViewModel.Date));

            var panelHeader = new Panel
                                  {
                                      StyleSpec = StyleAll,
                                      CssClass = "x-window-mc",
                                      BodyCssClass = "x-window-mc",
                                      Border = false
                                  };

            content.Add(new TextField
            {
                ID = "requestDateValue",
                DataIndex = "Date",
                Hidden = true
            });

            panelHeader.Add(new DisplayField
            {
                ID = "requestDateValueDisplay",
                StyleSpec = "margin: 10px 0px 5px 10px;",
                LabelAlign = LabelAlign.Left,
                FieldLabel = @"Дата создания",
                Value = DetailsViewModel.Date,
                ReadOnly = true,
                Enabled = false
            });

            panelHeader.Add(new DisplayField
            {
                ID = "requestNumberValue",
                FieldLabel = @"Номер",
                LabelAlign = LabelAlign.Left,
                StyleSpec = StyleAll,
                ReadOnly = true,
                DataIndex = "ID"
            });

            panelHeader.Add(new DisplayField
            {
                ID = "requestCategoryValue",
                FieldLabel = @"Категория",
                LabelAlign = LabelAlign.Left,
                StyleSpec = StyleAll,
                ReadOnly = true,
                DataIndex = "CategoryName"
            });

            panelHeader.Add(new DisplayField
            {
                ID = "requestOGVValue",
                FieldLabel = @"ОГВ",
                LabelAlign = LabelAlign.Left,
                StyleSpec = StyleAll,
                ReadOnly = true,
                DataIndex = "OGVName"
            });

            content.Add(panelHeader);

            // Грид с комментариями
            var commentsGrid = new CommentsGridControl(
                                        page,
                                        DetailsViewModel.CategoryID,
                                        Editable || CanAddCommentsAndDocs,
                                        extension.User.Name,
                                        DetailsViewModel.StateNameOGV)
                                   {
                                       StyleSpec = StyleAll
                                   };
            commentsGrid.InitAll();
            content.Add(commentsGrid);

            // Контрол для загрузки документов
            var fileListPanel = GetOrgFileListPanel(DetailsViewModel.ID > 0 && (Editable || CanAddCommentsAndDocs));
            if (DetailsViewModel.ID <= 0)
            {
                fileListPanel.ToolTips.Add(
                    new ToolTip
                        {
                            TargetControl = fileListPanel,
                            TrackMouse = true,
                            Listeners =
                                {
                                    Show =
                                        {
                                            Handler =
                                    @"this.body.dom.innerHTML = 'Добавить документы можно после сохранения заявки';"
                                        }
                                }
                        });
            }

            fileListPanel.StyleSpec = StyleAll;
            fileListPanel.AutoScroll = true;
            fileListPanel.Enabled = DetailsViewModel.ID > -1 && Editable;
            DocsStoreId = fileListPanel.StoreID;

            page.Controls.Add(fileListPanel.GetFileListStore());
            content.Add(fileListPanel);

            var tabDetails = new Panel
            {
                ID = "EstimateDetailsTab",
                Title = @"Реквизиты",
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                AutoScroll = true,
                Closable = false
            };

            tabDetails.Add(content);

            return new List<Component>
                       {
                           tabDetails, 
                           fileListPanel.GetFileUploadWindow(), 
                           commentsGrid.GetAddCommentWindow()
                       };
        }

        private static Control GetRequestDetailStore(DetailsEstimateModel detailsViewModel)
        {
            var store = new Store { ID = "dsDetail", AutoLoad = true };
            store.Reader.Add(new JsonReader
            {
                Fields =
                {
                    new RecordField("ID"),
                    new RecordField("CategoryID"),
                    new RecordField("CategoryName"),
                    new RecordField("OGVID"),
                    new RecordField("OGVName"),
                    new RecordField("Date"),
                    new RecordField("StateOGV")
                }
            });

            store.DataSource = new List<DetailsEstimateModel> { detailsViewModel };
            store.DataBind();
            store.AddScript("DetailsForm{0}.getForm().loadRecord(dsDetail.getAt(0));".FormatWith(detailsViewModel.ID));

            return store;
        }

        private FileListPanel GetOrgFileListPanel(bool editable)
        {
            var panelColumns = new List<FileListPanelColumn>
            {
                new FileListPanelColumn 
                    { 
                        Column = new Column
                                     {
                                         ColumnID = "fileName",
                                         Header = "Имя файла",
                                         DataIndex = "Name",
                                         MenuDisabled = true,
                                         Editable = Editable
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = true
                    },

                new FileListPanelColumn
                    {
                        Column = new Column
                                     {
                                         ColumnID = "fileDescription",
                                         Header = "Описание",
                                         DataIndex = "Note",
                                         MenuDisabled = true,
                                         Editable = Editable
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = true
                    },

                new FileListPanelColumn
                    {
                        Column = new Column
                                     {
                                         ColumnID = "changeUser",
                                         Header = "Кто добавил",
                                         DataIndex = "Executor",
                                         MenuDisabled = true,
                                         Editable = false
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = false
                    },

                new FileListPanelColumn
                    {
                        Column = new Column
                                     {
                                         ColumnID = "fileDate",
                                         Header = "Дата",
                                         DataIndex = "DateDoc",
                                         MenuDisabled = true,
                                         Editable = false
                                     },
                        AllowBlank = true,
                        VisibleInDialogWindow = false
                    }
            };

            var storeFields = new List<RecordField>
                                                {
                                                    new RecordField("Name"),
                                                    new RecordField("Note"),
                                                    new RecordField("DateDoc"),
                                                    new RecordField("Executor")
                                                };
            var loadController = new UrlWithParameters
            {
                Url = "/UploadOGVFiles/GetFiles",
                ParameterCollection = new ParameterCollection
                                          {
                                              new Parameter(
                                                  "applicationId", 
                                                  DetailsViewModel.ID.ToString(), 
                                                  ParameterMode.Value)
                                          }
            };

            var fileUploadController = new UrlWithParameters
            {
                Url = "/UploadOGVFiles/UploadFile",
                ParameterCollection = new ParameterCollection 
                                            {
                                                new Parameter(
                                                    "taskId", 
                                                    DetailsViewModel.ID.ToString(),
                                                    ParameterMode.Value),
                                            }
            };

            var fileDownloadController = new UrlWithParameters
            {
                Url = "/UploadOGVFiles/DownloadFile"
            };

            var fileUpdateController = new UrlWithParameters
            {
                Url = "/UploadOGVFiles/SaveFiles",
                ParameterCollection = new ParameterCollection 
                                            {
                                                new Parameter(
                                                    "taskId", 
                                                    DetailsViewModel.ID.ToString(), 
                                                    ParameterMode.Value),
                                            }
            };

            return new FileListPanel(
                panelColumns,
                editable,
                storeFields,
                loadController,
                fileUpdateController,
                fileUploadController,
                fileDownloadController,
                "DetailsForm{0}".FormatWith(DetailsViewModel.ID));
        }

        private Store GetCommentsStore()
        {
            var store = new Store
                            {
                                ID = "dsComments{0}".FormatWith(DetailsViewModel.CategoryID), 
                                AutoLoad = true, 
                                Restful = true, 
                                SkipIdForNewRecords = false
                            };

            store.SetRestController("FO41Comments")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Text")
                .AddField("StateName")
                .AddField("NoteDate")
                .AddField("Executor");

            store.Proxy.Proxy.RestAPI.ReadUrl = "/FO41Comments/OGVRead?applicationId={0}"
                .FormatWith(DetailsViewModel.ID);
            const string UrlCreate = "/FO41Comments/OGVSave";
            store.Proxy.Proxy.RestAPI.CreateUrl = UrlCreate;
            store.Proxy.Proxy.RestAPI.UpdateUrl = UrlCreate;
            store.Proxy.Proxy.RestAPI.DestroyUrl = "/FO41Comments/OGVDestroy"; 
            store.AddScript("var tempId = -2;");
            return store;
        }
    }
}
