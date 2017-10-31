using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.DocsDetail;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Views;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    /// <summary>
    /// Детализация прикрепляемых документов
    /// </summary>
    public sealed class DocsDetailControl : Control
    {
        public const string Scope = "E86n.Control.DocsDetailControl";
        private readonly DocsDetailViewModel docsDetailViewModel = new DocsDetailViewModel();

        /// <summary>
        /// Показываем номер НПА только для паспортов АУ
        /// </summary>
        private readonly bool visibleNumberNPA;

        public DocsDetailControl(int docId)
        {
            DocId = docId.ToString();

            var doc = Resolver.Get<INewRestService>().GetItem<F_F_ParameterDoc>(docId);
            visibleNumberNPA = doc.RefPartDoc.ID.Equals(FX_FX_PartDoc.PassportDocTypeID) && doc.RefUchr.RefTipYc.ID.Equals(FX_Org_TipYch.AutonomousID);
        }

        public string DocId { get; set; }
        
        public override List<Component> Build(ViewPage page)
        {
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("DocsDetailControl", Resource.DocsDetailControl);

            Store docsStore = InstallAppliedDocumentsStore(page, "Docs{0}Store".FormatWith(DocId));
            Component docsDetail = GetAppliedDocumentsPanel(docsStore, DocId);

            // Устанавливаем обработчик на грид с типами прикрепляемых документов нужен для включения и отключения кнопки "добавить"
            var gp = (GridPanel)((ColumnLayout)((Panel)docsDetail).Items[0]).Columns[0].Items[0];
            ((ImageCommandColumn)gp.ColumnModel.GetColumnById("ComandColumn")).PrepareGroupCommand.Handler =
                GlobalConsts.ScopeStateToolBar + ".prepareCommand(command);";

            return new List<Component> { docsDetail };
        }
        
        public Component BuildComponent(ViewPage page)
        {
            return Build(page)[0];
        }

        private Component GetAppliedDocumentsPanel(Store docStore, string parentId)
        {
            ResourceManager.GetInstance(docStore.Page).RegisterClientScriptBlock("UIBuilders", Resource.UIBuilders);

            string docsPanelId = "gpDocs" + parentId;
            string docsFormId = "frmDocs" + parentId;
            string docsFormIdInner = "frmDocFile" + parentId;
            
            string docsFormDownloadBtnId = "adfDownloadBtn" + parentId;
            string docsFormDeleteBtnId = "adfDeleteBtn" + parentId;

            string docsFormUrlFieldId = "adf" + UiBuilders.NameOf(() => docsDetailViewModel.Url);
            string docsFormIdFieldId = "adf" + UiBuilders.NameOf(() => docsDetailViewModel.ID);
            string docsFormFileFieldId = "adfFileName" + parentId;
            string docsFormTotalDocsSizeFieldId = "adf" + UiBuilders.NameOf(() => docsDetailViewModel.TotalDocsSize);

            docStore.Listeners.Load.Handler = "{0}.setDisabled(1);".FormatWith(docsFormId);

            var gridPanel = new GridPanel
                                {
                                    Title = @"Список документов",
                                    ID = docsPanelId,
                                    StoreID = docStore.ID,
                                    TrackMouseOver = true,
                                    Icon = Icon.DatabaseTable,
                                    LoadMask = { ShowMask = true },
                                    Width = 650,
                                    View = {
                                                new GroupingView
                                                {
                                                    ShowGroupName = false,
                                                    StartCollapsed = true,
                                                    HideGroupedColumn = true,
                                                    EnableGrouping = true,
                                                    Enabled = true
                                                }
                                            },
                                    ColumnModel =
                                        {
                                            Columns =
                                                      {
                                                        new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.ID),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.ID),
                                                            Header = UiBuilders.NameOf(() => docsDetailViewModel.ID),
                                                            Hidden = true,
                                                            Groupable = false
                                                        },

                                                        new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.RefParent),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.RefParent),
                                                            Header = "Документ/Ссылка",
                                                            Hidden = true,
                                                            Groupable = false
                                                        },

                                                        new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.RefTypeDoc),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.RefTypeDoc),
                                                            Header = "ТипДокумента/Ссылка",
                                                            Hidden = true,
                                                            Groupable = false
                                                        },

                                                        new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.RefTypeDocName),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.RefTypeDocName),
                                                            Header = "Тип документа"
                                                        },

                                                        new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.Url),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.Url),
                                                            Header = "Локальное имя файла",
                                                            Hidden = true
                                                        },

                                                        new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.Name),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.Name),
                                                            Header = UiBuilders.DescriptionOf(() => docsDetailViewModel.Name),
                                                            Hidden = false,
                                                            Width = 600,
                                                            Groupable = false
                                                        },

                                                        new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.DocDate),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.DocDate),
                                                            Header = "Дата утверждения",
                                                            Hidden = true,
                                                            Groupable = false
                                                        },

                                                        new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.NumberNPA),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.NumberNPA),
                                                            Header = UiBuilders.DescriptionOf(() => docsDetailViewModel.NumberNPA),
                                                            Hidden = true,
                                                            Groupable = false
                                                        },
                                                    new Column
                                                        {
                                                            ColumnID = UiBuilders.NameOf(() => docsDetailViewModel.FileSize),
                                                            DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.FileSize),
                                                            Header = UiBuilders.DescriptionOf(() => docsDetailViewModel.FileSize),
                                                            Hidden = true,
                                                            Groupable = false
                                                        },

                                                        new ImageCommandColumn
                                                        {
                                                            ColumnID = "ComandColumn",
                                                            Width = 20,
                                                            Commands =
                                                                {
                                                                    new ImageCommand
                                                                            {
                                                                                CommandName = "DeleteDocument",
                                                                                Icon = Icon.Delete,
                                                                                ToolTip = { Text = "Удалить документ" }
                                                                            }
                                                                },
                                                            GroupCommands =
                                                                {
                                                                    new GroupImageCommand
                                                                            {
                                                                                CommandName = "AddDocumentWithType",
                                                                                Icon = Icon.Add,
                                                                                RightAlign = true,
                                                                                ToolTip = { Text = "Добавить документ" }
                                                                            }
                                                                }
                                                        }
                                                }
                                        },
                                    Listeners =
                                                {
                                                    Command = { Handler = Scope + ".Command(this, record);" },
                                                    GroupCommand = { Handler = Scope + ".GroupCommand(this, command, groupId, records);" }
                                                }
                                };

            var rowSelectHandler = Scope + ".RowSelect(record, #{{{0}}}, #{{{1}}}, #{{{2}}}, #{{{3}}}, #{{{4}}});".FormatWith(
                                        docsFormId,
                                        docsFormUrlFieldId,
                                        docsFormDownloadBtnId,
                                        docsFormDeleteBtnId,
                                        docsFormFileFieldId);

            gridPanel.SelectionModel.Add(
                new RowSelectionModel
                {
                    ID = "sm" + gridPanel.ID,
                    SingleSelect = true,
                    Listeners =
                        {
                            RowSelect = { Handler = rowSelectHandler },
                            RowDeselect = { Handler = @"{0}.setDisabled(true);".FormatWith(docsFormId) }
                        }
                });

            gridPanel.AddRefreshButton();
            var docsForm = new FormPanel
                               {
                                    ID = docsFormId,
                                    FormID = docsFormIdInner,
                                    Title = @"Выбранный документ",
                                    Frame = false,
                                    MonitorValid = true,
                                    FileUpload = true,
                                    AutoScroll = true,
                                    LabelWidth = 200,
                                    DefaultAnchor = "99%",
                                    Padding = 6,
                                    ColumnWidth = 1,
                                    Disabled = true,
                                    Layout = LayoutType.Form.ToString(),
                                    Items =
                                       {
                                            new NumberField
                                                {
                                                    ID = docsFormIdFieldId,
                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.ID),
                                                    FieldLabel = UiBuilders.NameOf(() => docsDetailViewModel.ID),
                                                    Hidden = true
                                               },

                                            new NumberField
                                               {
                                                    ID = "adfRefParameterID",
                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.RefParent),
                                                    FieldLabel = @"Документ/Ссылка",
                                                    Hidden = true
                                               },

                                            new NumberField
                                               {
                                                    ID = "adfRefTypeDocID",
                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.RefTypeDoc),
                                                    FieldLabel = @"ТипДокумента/Ссылка",
                                                    Hidden = true
                                               },

                                            new TextField
                                               {
                                                    ID = "adfRefTypeDoc_Name",
                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.RefTypeDocName),
                                                    FieldLabel = @"Тип документа (только чтение)",
                                                    ReadOnly = true
                                               },

                                            new TextField
                                               {
                                                    ID = "adfName",
                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.Name),
                                                    FieldLabel = @"Название",
                                                    AllowBlank = false,
                                                    MaxLength = 128
                                               },

                                            new DateField
                                               {
                                                    ID = "adfDocDate",
                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.DocDate),
                                                    Format = "d.m.Y",
                                                    FieldLabel = @"Дата утверждения (ДД-ММ-ГГГГ)",
                                                    AllowBlank = false
                                               },

                                            new TextField
                                               {
                                                   ID = "adf" + UiBuilders.NameOf(() => docsDetailViewModel.NumberNPA),
                                                   DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.NumberNPA),
                                                   FieldLabel = UiBuilders.DescriptionOf(() => docsDetailViewModel.NumberNPA),
                                                   Hidden = !visibleNumberNPA,
                                                   MaxLength = 64
                                               },
                                            new NumberField
                                               {
                                                    ID = docsFormTotalDocsSizeFieldId,
                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.TotalDocsSize),
                                                    FieldLabel = @"Размер всех файлов прикрепленных к документу",
                                                    Hidden = true
                                                },

                                            new Panel
                                               {
                                                    ID = "externalFiles",
                                                    Layout = LayoutType.Form.ToString(),
                                                    Border = false,
                                                    DefaultAnchor = "99%",
                                                    Items =
                                                        {
                                                            new HyperLink
                                                                {
                                                                    ID = "externalFileDownload",
                                                                    FieldLabel = @"Ссылка для загрузки файла-документа",
                                                                    Icon = Icon.DiskDownload,
                                                                    IsFormField = true,
                                                                    Text = string.Empty,
                                                                    NavigateUrl = string.Empty
                                                                }
                                                        }
                                               },

                                            new Panel
                                               {
                                                    ID = "internalFiles",
                                                    Layout = LayoutType.Form.ToString(),
                                                    Border = false,
                                                    DefaultAnchor = "99%",
                                                    Items =
                                                       {
                                                            new TextField
                                                                {
                                                                    ID = docsFormUrlFieldId,
                                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.Url),
                                                                    FieldLabel = UiBuilders.DescriptionOf(() => docsDetailViewModel.Url),
                                                                    Hidden = true
                                                               },

                                                    new Button
                                                               {
                                                                    ID = "adfSave",
                                                                    Text = @"Сохранить документ",
                                                                    ToolTip = @"Сохранить документ",
                                                                    Icon = Icon.Disk,
                                                                    Listeners =
                                                                       {
                                                                            Click =
                                                                                {
                                                                                    Handler =
@"rec = {0}.getSelectionModel().getSelected(); {1}.getForm().updateRecord(rec); {2}.save();".FormatWith(gridPanel.ID, docsFormId, docStore.ID)
                                                                                }
                                                                        }
                                                                },

                                                    new Label { Text = string.Empty },

                                                    new FileUploadField
                                                               {
                                                                    ID = docsFormFileFieldId,
                                                                    FieldLabel = @"Файл",
                                                                    ButtonText = @"Выбрать файл",
                                                                    DirectEvents =
                                                                       {
                                                                            FileSelected =
                                                                                {
                                                                                    Method = HttpMethod.POST,
                                                                                    CleanRequest = true,
                                                                                    Url = UiBuilders.GetUrl<DocController>("Upload"),
                                                                                    IsUpload = true,
                                                                                    FormID = docsFormIdInner,
                                                                                    ExtraParams =
                                                                                        {
                                                                                          new Parameter("id", "#{{{0}}}.getValue()".FormatWith(docsFormIdFieldId), ParameterMode.Raw)
                                                                                        },
                                                                                    Before = string.Concat(
                                                                                            "return ",
                                                                                            Scope,
                                            ".BeforeFileSelected(el, extraParams, #{{{0}}}, #{{{1}}});".FormatWith(docsFormTotalDocsSizeFieldId, docsFormUrlFieldId)),
                                                                                    Success = @"
Ext.MessageBox.hide();
Ext.net.Notification.show( {{ iconCls: 'icon-information', html: result.extraParams.msg, title: 'Уведомление', hideDelay: 2500 }} );
{0}.reload();
setTimeout(function() {{ {1}.selModel.selectRow({1}.selModel.last) }}, 1000);".FormatWith(docStore.ID, gridPanel.ID),

                                                                                    Failure = @"
if ( (result.extraParams != undefined) && (result.extraParams.responseText != undefined) )
    { Ext.Msg.show( { title: 'Ошибка', msg: result.extraParams.responseText, minWidth: 200, modal: true, icon: Ext.Msg.ERROR, buttons: Ext.Msg.OK } ); }
else
    { Ext.Msg.show( { title: 'Ошибка', msg: result.responseText, minWidth: 200, modal: true, icon: Ext.Msg.ERROR, buttons: Ext.Msg.OK } ); }
"
                                                                                }
                                                                       }
                                                               },
                                                            new TextField
                                                                {
                                                                    ID = "adf" + UiBuilders.NameOf(() => docsDetailViewModel.FileSize),
                                                                    DataIndex = UiBuilders.NameOf(() => docsDetailViewModel.FileSize),
                                                                    FieldLabel = UiBuilders.DescriptionOf(() => docsDetailViewModel.FileSize) + @"(байт)",
                                                                    ReadOnly = true
                                                                },
                                                           new Label { Text = @"ВНИМАНИЕ! Сохранять документ после загрузки файла не требуется." },
                                                           new Button
                                                               {
                                                                   StyleSpec = "margin-bottom: 4px",
                                                                   ID = docsFormDownloadBtnId,
                                                                   Text = @"Просмотреть загруженный документ",
                                                                   ToolTip = @"Просмотреть загруженный документ",
                                                                   Icon = Icon.DiskDownload,
                                                                   DirectEvents =
                                                                       {
                                                                           Click =
                                                                               {
                                                                                   Url = UiBuilders.GetUrl<DocController>("Download"),
                                                                                   CleanRequest = true,
                                                                                   IsUpload = true,
                                                                                   Before = Scope + ".BeforeDownload(extraParams, #{{{0}}}, #{{{1}}}, #{{{2}}});"
                                                                                                    .FormatWith(docsFormIdFieldId, docsFormFileFieldId, docsFormUrlFieldId),
                                                                                   Failure = @"
Ext.Msg.show( { title:'Ошибка', msg: response.responseText, buttons: Ext.Msg.OK, icon: Ext.MessageBox.ERROR, maxWidth: 1000 });
"
                                                                               }
                                                                       }
                                                               },
                                                           new Button
                                                               {
                                                                   StyleSpec = "margin-bottom: 4px",
                                                                   ID = docsFormDeleteBtnId,
                                                                   Text = @"Удалить загруженный документ",
                                                                   ToolTip = @"Удалить загруженный документ",
                                                                   Icon = Icon.Delete,
                                                                   DirectEvents =
                                                                       {
                                                                           Click =
                                                                               {
                                                                                   Url = UiBuilders.GetUrl<DocController>("DeleteFile"),
                                                                                   CleanRequest = true,
                                                                                   IsUpload = true,
                                                                                   Before = Scope + ".BeforeDelete(extraParams, #{{{0}}}, #{{{1}}});"
                                                                                                    .FormatWith(docsFormIdFieldId, docsFormUrlFieldId),
                                                                                   Success = @"
Ext.MessageBox.hide();
Ext.net.Notification.show( {{ iconCls: 'icon-information', html: result.extraParams.msg, title: 'Уведомление', hideDelay: 2500 }} );
{0}.reload();
setTimeout(function() {{ {1}.selModel.selectRow({1}.selModel.last) }}, 1000);
".FormatWith(docStore.ID, gridPanel.ID),
                                                                                   Failure = @"
Ext.Msg.show( { title:'Ошибка', msg: response.responseText, buttons: Ext.Msg.OK, icon: Ext.MessageBox.ERROR, maxWidth: 1000 });
"
                                                                               }
                                                                       }
                                                               }
                                                       }
                                               }
                                       }
                               };

            return new Panel
                       {
                           ID = docStore.ID.Replace("Store", string.Empty),
                           Icon = Icon.PageWhiteZip,
                           Title = @"Приложения",
                           Layout = LayoutType.Fit.ToString(),
                           Items =
                               {
                                   new ColumnLayout
                                       {
                                           Split = true,
                                           FitHeight = true,
                                           Columns = { new LayoutColumn { Items = { gridPanel } }, new LayoutColumn { Items = { docsForm } } }
                                       }
                               }
                       };
        }

        private Store InstallAppliedDocumentsStore(ViewPage page, string storeId)
        {
            Store store = StoreExtensions.StoreUrlCreateDefault(
                 storeId,
                 true,
                 UiBuilders.GetUrl<DocCommonController>("ReadAction"),
                 UiBuilders.GetUrl<DocCommonController>("SaveAction"),
                 UiBuilders.GetUrl<DocCommonController>("SaveAction"),
                 UiBuilders.GetUrl<DocController>("DeleteAction"));

            store.SetBaseParams("docId", DocId, ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocId, ParameterMode.Raw);

            store.SetBaseParams("modelType", docsDetailViewModel.GetType().AssemblyQualifiedName, ParameterMode.Value);
            store.SetWriteBaseParams("modelType", docsDetailViewModel.GetType().AssemblyQualifiedName, ParameterMode.Value);

            store.AddFieldsByClass(docsDetailViewModel);
            store.GroupField = UiBuilders.NameOf(() => docsDetailViewModel.RefTypeDocName);
            page.Controls.Add(store);
            return store;
        }
    }
}
