using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders.Note
{
    public class DebtBookNoteFormView : View
    {
        private const string NoteStoreID = "dsNote";
        private const string NoteFormPanelID = "formNote";

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(GetNoteListStore());

            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                                {
                                    new BorderLayout
                                    {   
                                        North = { Items = { CreateTopPanel() }, Margins = { Bottom = 5 } },
                                        Center = { Items = { CreateNoteForm() }, Margins = { Bottom = 3 } }
                                    }
                                }
            };

            return new List<Component> { view };
        }

        private Panel CreateTopPanel()
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(NoteStoreID) } }
            });

            return new Panel
            {
                ID = "topPanel",
                Height = 25,
                Border = false,
                TopBar = { toolbar }
            };
        }

        private List<Component> CreateNoteForm()
        {
            var form = new FormPanel
                           {
                               ID = NoteFormPanelID,
                               Width = 400,
                               LoadMask = { ShowMask = true },
                               Padding = 6,
                               LabelWidth = 6
                           };

            form.Items.Add(new TextField { ID = "ID", DataIndex = "ID", Hidden = true });
            form.Items.Add(new TextField { ID = "Editable", DataIndex = "Editable", Hidden = true });

            form.Items.Add(new Label { ID = "FileExistIcon", Text = "Файл загружен", Icon = Icon.Accept, IconAlign = Alignment.Right, Hidden = true });
            form.Items.Add(new Label { ID = "FileEmptyIcon", Text = "Файл отсутствует", Icon = Icon.Cancel, IconAlign = Alignment.Right, Hidden = false });

            var panel1 = new Panel
                            {
                                Layout = LayoutType.Column.ToString(),
                                LayoutConfig = { new ColumnLayoutConfig { FitHeight = false, Margin = 5 } },
                                Padding = 6,
                                Border = false
                            };
            form.Items.Add(panel1);

            panel1.Items.Add(new Button
                               {
                                   ID = "btnDownload",
                                   Text = "Просмотр", 
                                   Icon = Icon.PageWhiteExcel, 
                                   ToolTip = "Посмотреть загруженный документ", 
                                   Width = 100,
                                   DirectEvents =
                                   {
                                        Click =
                                        {
                                            Url = "/DebtBookNote/DownloadNote",
                                            CleanRequest = true,
                                            IsUpload = true,
                                            ExtraParams = { new Parameter("id", "ID.getValue()", ParameterMode.Raw) }
                                        }
                                   },
                                   Disabled = true
                               });

            panel1.Items.Add(new FileUploadField
                               {
                                   ID = "fldFileUpload", 
                                   ButtonOnly = true,
                                   HideLabel = true,
                                   ButtonText = String.Empty,
                                   ToolTips = { new ToolTip { Html = "Загрузить документ", AutoShow = true, Target = "fldFileUpload-file" } },
                                   Icon = Icon.Attach,
                                   Hidden = true,
                                   DirectEvents =
                                   {
                                        FileSelected =
                                        {
                                            Url = "/DebtBookNote/UploadNote",
                                            CleanRequest = true,
                                            IsUpload = true,
                                            Before = @"
if(value.match('\.xls$') === null) {
  Ext.Msg.show({title:'Ошибка', msg:'Можно загружать только файлы Excel (*.xls)', minWidth:200, modal:true, icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });
  return false;
} 
Ext.Msg.wait('Загрузка...', 'Загрузка');",
                                            Failure = "Ext.Msg.show({title:'Ошибка', msg:result.extraParams.responseText, minWidth:200, modal:true, icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });",
                                            Success = "Ext.net.Notification.show({iconCls : 'icon-information', html : result.extraParams.msg, title : 'Уведомление', hideDelay : 2500});"
                                        }
                                   },
                               });

            panel1.Items.Add(new Button
                               {
                                   ID = "btnDelete",
                                   Text = String.Empty, 
                                   Icon = Icon.CutRed, 
                                   ToolTip = "Удалить загруженный документ",
                                   Hidden = true,
                                   DirectEvents =
                                   {
                                        Click =
                                        {
                                            Url = "/DebtBookNote/DeleteNote",
                                            CleanRequest = true,
                                            IsUpload = true,
                                            Confirmation = 
                                            {
                                                ConfirmRequest = true,
                                                Message = "Удалить пояснительную записку?",
                                                Title = "Предупреждение"
                                            },
                                            Before = "Ext.Msg.wait('Удаление...', 'Удаление');",
                                            Failure = "Ext.Msg.show({title:'Ошибка', msg:result.extraParams.responseText, minWidth:200, modal:true, icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });",
                                            Success = "Ext.net.Notification.show({iconCls : 'icon-information', html : result.extraParams.msg, title : 'Уведомление', hideDelay : 2500});"  
                                        }
                                   }
                               });
            
            form.AddScript(@"
var SetComponentsVisibility = function (id, editable){
  btnDownload.setDisabled( id == null || id == '');
  fldFileUpload.setVisible(editable);
  btnDelete.setVisible(id != null && id != '' && editable);
  FileExistIcon.setVisible(id != null && id != '');
  FileEmptyIcon.setVisible(id == null || id == '');
};
");

            return new List<Component> { form };
        }

        private Store GetNoteListStore()
        {
            var store = new Store
            {
                ID = NoteStoreID,
                AutoLoad = true
            };

            store.SetHttpProxy("/DebtBookNote/GetNote");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Editable")
                }
            });
            
            store.Listeners.DataChanged.Handler = @"
var record = this.getAt(0) || {{}};
{0}.getForm().loadRecord(record);
SetComponentsVisibility(ID.getValue(), Editable.getValue() != 0 );
".FormatWith(NoteFormPanelID);

            return store;
        }
    }
}
