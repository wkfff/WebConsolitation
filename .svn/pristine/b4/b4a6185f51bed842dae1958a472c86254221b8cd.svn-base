using System;
using System.Collections.Generic;
using Ext.Net;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Core.Gui
{
    /// <summary>
    /// Описание колонки
    /// </summary>
    public struct FileListPanelColumn
    {
        /// <summary>
        /// Собственно колонка в таблице, незабудьте установить редактируемая или нет
        /// </summary>
        public ColumnBase Column;

        /// <summary>
        /// Компонент, который будет отображаться в окне для добавления нового документа.
        /// </summary>
        public Component ComponentViewer;

        /// <summary>
        /// Показывать ли это поле в диалоговом окне при upload-файла
        /// </summary>
        public bool VisibleInDialogWindow;
        
        /// <summary>
        /// Допускает ли это поле пустые значения
        /// </summary>
        public bool AllowBlank;
    }
    
    /// <summary>
    /// привязка метода контроллера
    /// </summary>
    public struct UrlWithParameters
    {
        /// <summary>
        /// адрес метода контроллера
        /// </summary>
        public string Url;

        /// <summary>
        /// Список параметров, которые необходимо передавать контроллеру (будут добавлены в DirectMethod)
        /// </summary>
        public ParameterCollection ParameterCollection;
    }

    public class FileListPanel : Ext.Net.GridPanel
    {
        /// <summary>
        /// id таблицы со списком файлов
        /// </summary>
        public const string PanelId = "gpTaskFiles";

        /// <summary>
        /// id store, поставщика данных для таблицы
        /// </summary>
        public const string StoreId = "dsFiles";

        /// <summary>
        /// id поля выбора файла в диалоговом окне. 
        /// </summary>
        public const string FileUploadFieldId = "fileUploadField";

        private List<FileListPanelColumn> panelColumns;
        private bool editable;
        private bool allowDelete;
        private List<RecordField> storeFields;
        private UrlWithParameters loadController;
        private UrlWithParameters updateController;
        private UrlWithParameters fileUploadController;
        private UrlWithParameters fileDownloadController;
        private string formId;
        private bool reloadAfterUpload;

        private Store store;
        private Window window;

        /// <summary>
        /// Initializes a new instance of the FileListPanel class.
        /// </summary>
        /// <param name="panelColumns"> Список колонок таблицы с дополнительными признаками</param>
        /// <param name="editable">Признак редактируемости, влияет также на наличие кнопки "Добавить файл"</param>
        /// <param name="storeFields">Список полей в store, данные из которого мапятся на колонки</param>
        /// <param name="loadController">Поставщик данных для таблицы файлов</param>
        /// <param name="updateController">Реализует механизм обновления данных, измененных в таблице (при вызове метода store.save())</param>
        /// <param name="fileUploadController">Реализует загрузку файла. Обязательные поля контоллера - (int? fileId, string fileName). Остальные по усмотрению.</param>
        /// <param name="fileDownloadController">Реализует выгрузку файла на клиент. Обязательное поле контроллера - (int fileId)</param>
        /// <param name="formId">id формы(FormPanel), на которой расположена таблица с файлами. Используется при download файла</param>
        /// <param name="reloadAfterUpload">Признак, обновлять ли store после загрузки документа</param>
        /// <param name="allowDelete">Признак, предоставлять ли возможность удалять документы</param>
        public FileListPanel(
                             List<FileListPanelColumn> panelColumns,
                             bool editable,
                             List<RecordField> storeFields,
                             UrlWithParameters loadController, 
                             UrlWithParameters? updateController,
                             UrlWithParameters fileUploadController,
                             UrlWithParameters fileDownloadController,
                             string formId, 
                             bool reloadAfterUpload = false,
                             bool? allowDelete = null)
        {
            this.panelColumns = panelColumns;
            this.editable = editable;
            this.storeFields = storeFields;
            this.loadController = loadController;
            this.updateController = (updateController == null) ? new UrlWithParameters() : (UrlWithParameters)updateController;
            this.fileUploadController = fileUploadController;
            this.fileDownloadController = fileDownloadController;
            this.formId = formId;
            this.reloadAfterUpload = reloadAfterUpload;
            this.allowDelete = allowDelete == null ? editable : this.allowDelete = (bool)allowDelete;

            InitializeGridPanel();
            InitializeStore();
            InitializeWindow();
        }

        public Store GetFileListStore()
        {
            return this.store;
        }

        public Window GetFileUploadWindow()
        {
            return this.window;
        }

        private void InitializeStore()
        {
            Store storeFiles = new Store 
            { 
                ID = StoreId, 
                AutoLoad = true, 
                RefreshAfterSaving = RefreshAfterSavingMode.Always,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            storeFiles.Proxy.Add(new HttpProxy { Url = loadController.Url, Method = HttpMethod.GET });
            storeFiles.BaseParams.AddRange(loadController.ParameterCollection);

            storeFiles.UpdateProxy.Add(new HttpWriteProxy { Url = updateController.Url, Method = HttpMethod.POST, Timeout = 500000 });
            storeFiles.WriteBaseParams.Add(new Parameter("storeChangedData", "#{{{0}}}.store.getChangedData()".FormatWith(PanelId), ParameterMode.Raw));
            if (updateController.ParameterCollection != null)
            {
                storeFiles.WriteBaseParams.AddRange(updateController.ParameterCollection);
            }

            JsonReader jsonReader = new JsonReader();
            jsonReader.IDProperty = "ID";
            jsonReader.Root = "data";
            jsonReader.Fields.Add(new RecordField("ID"));
            jsonReader.Fields.AddRange(storeFields);

            storeFiles.Reader.Add(jsonReader);
            storeFiles.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке информации о файлах', response.responseText);";
            storeFiles.Listeners.SaveException.Handler = "Ext.Msg.alert('Ошибка при сохранении информации о файлах', e.message || response.statusText);";
            storeFiles.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            this.store = storeFiles;
        }

        private void InitializeWindow()
        {
            Window win = new Window
            {
                ID = "wUploadFileDialog",
                Title = "Форма загрузки документа",
                Hidden = true,
                Modal = true,
                Width = 500,
                AutoHeight = true,
                Resizable = false,
                Border = false,
                Listeners =
                    {
                        BeforeShow = { Handler = ConvertToParameters(panelColumns) },
                        BeforeHide = { Handler = "btResetUploadField.fireEvent('click');" },
                    }
            };
            FormPanel form = new FormPanel();
            win.Add(form);

            form.ID = "fUploadFileDialog";
            form.Frame = true;
            form.MonitorValid = true;
            form.Border = false;
            form.BodyBorder = false;
            form.BodyStyle = "padding: 10px 10px 0 10px;background:none repeat scroll 0 0 #DFE8F6;";

            form.Defaults.Add(new Parameter { Name = "anchor", Value = "95%", Mode = ParameterMode.Value });
            form.Defaults.Add(new Parameter { Name = "msgTarget", Value = "side", Mode = ParameterMode.Value });

            foreach (var column in panelColumns)
            {
                if (column.VisibleInDialogWindow)
                {
                    if (column.ComponentViewer != null)
                    {
                        form.Items.Add(column.ComponentViewer);
                    }
                    else
                    {
                        if (column.Column.GetType() == typeof(Column))
                        {
                            form.Items.Add(new TextField
                                               {
                                                   ID = column.Column.ColumnID,
                                                   FieldLabel = column.Column.Header,
                                                   AllowBlank = column.AllowBlank
                                               });
                        }
                        else if (column.Column.GetType() == typeof(CheckColumn))
                        {
                            form.Items.Add(new Checkbox
                                               {
                                                   ID = column.Column.ColumnID,
                                                   FieldLabel = column.Column.Header
                                               });
                        }
                        else if (column.Column.GetType() == typeof(DateColumn))
                        {
                            form.Items.Add(new DateField
                            {
                                ID = column.Column.ColumnID,
                                FieldLabel = column.Column.Header,
                                AllowBlank = column.AllowBlank
                            });
                        }
                        else
                        {
                            throw new Exception("Неподдерживаемый тип колонки");
                        }
                    }
                }
            }

            form.Items.Add(new FileUploadField
            {
                ID = FileUploadFieldId,
                EmptyText = "Выберите файл...",
                FieldLabel = "Файл",
                AllowBlank = false,
                ButtonText = String.Empty,
                Icon = Icon.ImageAdd
            });
            form.Listeners.ClientValidation.Handler = "#{btSave}.setDisabled(!valid);";

            Button saveButton = new Button
            {
                ID = "btSave",
                Text = "Сохранить",
                DirectEvents =
                {
                    Click =
                    {
                        Method = HttpMethod.POST,
                        CleanRequest = true,
                        Url = fileUploadController.Url,
                        IsUpload = true,
                        Timeout = 60 * 60 * 1000,
                        ExtraParams =
                                             {
                                                 new Parameter("fileId", "wUploadFileDialog.documentParameters.id", ParameterMode.Raw),
                                                 new Parameter("fileName", "fileUploadField.getValue()", ParameterMode.Raw)
                                             },
                        Before = "if (!#{fUploadFileDialog}.getForm().isValid()) { return false; } Ext.Msg.wait('Загружается...', 'Загрузка');",
                        Failure = @"
if (result.extraParams != undefined && result.extraParams.responseText!=undefined){
  Ext.Msg.show({title:'Ошибка', msg:result.extraParams.responseText, minWidth:200, modal:true, icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });
}else{
  Ext.Msg.show({title:'Ошибка', msg:result.responseText, minWidth:200, modal:true, icon:Ext.Msg.ERROR, buttons:Ext.Msg.OK });
}
",
                        Success = @"
Ext.MessageBox.hide();
wUploadFileDialog.hide();
btResetUploadField.fireEvent('click');
Ext.net.Notification.show({{iconCls : 'icon-information', html : result.extraParams.msg, title : 'Уведомление', hideDelay : 2500}});
{0}
".FormatWith(reloadAfterUpload ? "{0}.getStore().reload();".FormatWith(PanelId) : string.Empty)
                    }
                }
            };

            saveButton.DirectEvents.Click.ExtraParams.AddRange(fileUploadController.ParameterCollection);

            foreach (var column in panelColumns)
            {
                if (column.VisibleInDialogWindow && column.Column.ColumnID != "fileName")
                {
                    saveButton.DirectEvents.Click.ExtraParams.Add(new Parameter(column.Column.ColumnID, "{0}.getValue()".FormatWith(column.Column.ColumnID), ParameterMode.Raw));
                }
                else if (column.VisibleInDialogWindow && column.Column.ColumnID == "fileName")
                {
                    saveButton.DirectEvents.Click.ExtraParams.GetParameter("fileName").Value = "{0}.getValue()".FormatWith(column.Column.ColumnID);
                }
            }

            form.Buttons.Add(saveButton);

            form.Buttons.Add(new Button
            {
                ID = "btResetUploadField",
                Text = "Сброс",
                Listeners = { Click = { Handler = "#{fUploadFileDialog}.getForm().reset();" } }
            });

            this.window = win;
        }

        private void InitializeGridPanel()
        {
            ID = PanelId;
            StoreID = StoreId;
            Title = "Список документов";
            Height = 400;
            Flex = 1;
            AutoScroll = true;
            Collapsible = true;
            AnimCollapse = true;
            TrackMouseOver = true;
            Icon = Icon.DatabaseTable;
            LoadMask.ShowMask = true;
            TopBar.Add(
                new Toolbar
                    {
                        Items =
                            {
                                new Button
                                    {
                                        ID = "btnAddFile",
                                        Icon = Icon.Add,
                                        Text = "Добавить",
                                        ToolTip = "Добавить новый файл",
                                        Listeners =
                                            {
                                                Click =
                                                    {
                                                        Handler =
                                                            "wUploadFileDialog.documentParameters = {{ ID:'', {0} }}; wUploadFileDialog.show('{1}');"
                                                            .FormatWith(ConvertToEmptyParameters(panelColumns), PanelId)
                                                    }
                                    },
                                    Hidden = !editable,
                                }
                        }
                    });

            ColumnModel.Columns.Add(
                new Column
                    {
                        Hidden = true,
                        DataIndex = "ID",
                        MenuDisabled = true
                    });

            ColumnModel.Columns.Add(
                new ImageCommandColumn
                    {
                        Width = 50,
                        Commands =
                        {
                            new ImageCommand
                                {
                                    Icon = Icon.BookOpen,
                                    CommandName = "openDocument",
                                    ToolTip =
                                    {
                                        Text =
                                            "Открыть для просмотра"
                                    }
                                },
                            new ImageCommand
                                {
                                    Icon = Icon.Attach,
                                    CommandName = "reloadDocument",
                                    ToolTip = { Text = "Перезагрузить файл" },
                                    Hidden = !editable
                                }
                        }
                    });
            
            // Рисуем колонки пользователя
            foreach (var column in panelColumns)
            {
                if (!editable)
                {
                    column.Column.Editable = false;
                }
                
                if (column.Column.Editor.Count == 0)
                {
                 column.Column.Editor.Add(new TextField());   
                }

                ((TextFieldBase)column.Column.Editor.Editor).AllowBlank = column.AllowBlank;
                ColumnModel.Columns.Add(column.Column);
            }
            
            ColumnModel.Columns.Add(
                new ImageCommandColumn
                    {
                        Commands =
                        {
                            new ImageCommand
                                {
                                    Icon = Icon.Delete,
                                    CommandName = "deleteFile",
                                    ToolTip = { Text = "Удалить" },
                                    Hidden = !editable || !allowDelete
                                }
                        }
                    });

            SelectionModel.Add(new RowSelectionModel { ID = "RowSelectionModel2", SingleSelect = true });

            Listeners.Command.Handler = "commandHandlerFiles(command, rowIndex, record)";

            View.Add(
                new Ext.Net.GridView
                    {
                        ID = "GridViewFiles",
                        ForceFit = true,
                        MarkDirty = true,
                    });

            AddScript(@"
var commandHandlerFiles = function (command, rowIndex, record) {{
    switch (command) {{
        case 'openDocument':
            Ext.net.DirectMethod.request({{
                url: '{0}',
                isUpload: true,
                formProxyArg: '{1}',
                cleanRequest: true,
                params: {{
                    fileId: record.data.ID
                    {2}
                }},
                success: successSaveHandler,
                failure: failureSaveHandler
            }});
            break;

        case 'reloadDocument':
            wUploadFileDialog.documentParameters = Object();
            wUploadFileDialog.documentParameters.id = record.data.ID;
            {3}
            wUploadFileDialog.show('{4}');
            break;

        case 'deleteFile':
            Ext.Msg.confirm('Предупреждение', 'Удалить документ?', function (btn) {{
                if (btn == 'yes') {{
                    Ext.getCmp('{4}').store.removeAt(rowIndex);
                    Ext.net.Notification.show({{ iconCls: 'icon-information', html: 'Не забудьте сохранить изменения в таблице.', title: 'Уведомление', hideDelay: 2500 }});
                }}
            }}
            );
            break;
    }}
}};
".FormatWith(fileDownloadController.Url, formId, ConvertDownloadParams(fileDownloadController.ParameterCollection), ConvertToParametersDialog(panelColumns), PanelId));

            AddScript(@"
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
        }

        private string ConvertToEmptyParameters(List<FileListPanelColumn> columns)
        {
            List<string> args = new List<string>();
            for (int index = 0; index < columns.Count; index++)
            {
                if (columns[index].VisibleInDialogWindow)
                {
                    args.Add("{0}:null".FormatWith(columns[index].Column.ColumnID));
                }
            }

            string result = String.Join(",", args.ToArray());
            
            return result;
        }

        private string ConvertDownloadParams(ParameterCollection parameterCollection)
        {
            string result = String.Empty;
            if (parameterCollection != null)
            {
                foreach (var param in parameterCollection)
                {
                    result = String.Concat(result, ", {0}:{1}".FormatWith(param.Name, param.Value));
                }
            }

            return result;
        }
        
        private string ConvertToParameters(List<FileListPanelColumn> columns)
        {
            string result = String.Empty;
            foreach (var column in columns)
            {
                if (column.VisibleInDialogWindow)
                {
                    result = String.Concat(result, "{0}.setValue(wUploadFileDialog.documentParameters.{0});".FormatWith(column.Column.ColumnID));
                }
            }

            return result;
        }
        
        private string ConvertToParametersDialog(List<FileListPanelColumn> columns)
        {
            string result = String.Empty;
            foreach (var column in columns)
            {
                if (column.VisibleInDialogWindow)
                {
                    result = String.Concat(result, "wUploadFileDialog.documentParameters.{0} = record.data.{1};".FormatWith(column.Column.ColumnID, column.Column.DataIndex));
                }
            }
            
            return result;
        }
    }
}
