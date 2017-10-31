using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.Informator.Presentation.Views;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Informator.Presentation.Controls
{
    public class SendMessage : Control
    {
        private const string Scope = "Informator.View.News.Grid";

        public override List<Component> Build(ViewPage page)
        {
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("NewsView.js", Resource.NewsView);

            var adressBookStore = new Store
            {
                ID = "adressBookStore",
                Restful = true,
                ShowWarningOnFailure = true,
                WarningOnDirty = false,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                AutoLoad = true,
            };
            adressBookStore.SetHttpProxy("/News/GetAdressBook").SetJsonReader();
            adressBookStore.AddField("ID");
            adressBookStore.AddField("Name");

            var adressGrbsBookStore = new Store
            {
                ID = "adressGrbsBookStore",
                Restful = true,
                ShowWarningOnFailure = true,
                WarningOnDirty = false,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                AutoLoad = false,
            };
            adressGrbsBookStore.Proxy.Add(new HttpProxy { Url = "/News/GetGrbsAdressBook", Method = HttpMethod.GET, Timeout = 50000000 });
            var jsonReader = new JsonReader { IDProperty = "ID", Root = "data" };
            jsonReader.Fields.AddRange(new RecordFieldCollection { new RecordField("ID"), new RecordField("Name") });
            adressGrbsBookStore.Reader.Add(jsonReader);
            adressGrbsBookStore.AddField("ID")
                               .AddField("Name");
            adressGrbsBookStore.Listeners.BeforeLoad.Handler = @"Ext.net.Mask.show({msg: 'Загрузка..'});";
            adressGrbsBookStore.Listeners.Load.Handler = @"Ext.net.Mask.hide();";

            var adressPposBookStore = new Store
            {
                ID = "adressPposBookStore",
                Restful = true,
                ShowWarningOnFailure = true,
                WarningOnDirty = false,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                AutoLoad = false,
            };
            adressPposBookStore.Proxy.Add(new HttpProxy { Url = "/News/GetPpoAdressBook", Method = HttpMethod.GET, Timeout = 50000000 });
            var ppoJsonReader = new JsonReader { IDProperty = "ID", Root = "data" };
            ppoJsonReader.Fields.AddRange(new RecordFieldCollection { new RecordField("ID"), new RecordField("Name") });
            adressPposBookStore.Reader.Add(ppoJsonReader);
            adressPposBookStore.AddField("ID")
                               .AddField("Name");
            adressPposBookStore.Listeners.BeforeLoad.Handler = @"Ext.net.Mask.show({msg: 'Загрузка..'});";
            adressPposBookStore.Listeners.Load.Handler = @"Ext.net.Mask.hide();";

            var comboBox = new ComboBox
            {
                ID = "cb",
                TriggerAction = TriggerAction.All,
                SelectOnFocus = true,
                ValueField = "ID",
                DisplayField = "Name",
                FieldLabel = "Получатели",
                AnchorHorizontal = "75%",
            };
            comboBox.Store.Add(adressBookStore);
            comboBox.Listeners.KeyUp.Fn = @"function (combo, e) {           
            var v = combo.getRawValue();
            combo.store.filter(combo.displayField, new RegExp(v));
            combo.onLoad();
        }";

            var groupCombo = new MultiCombo
            {
                TriggerAction = TriggerAction.All,
                ID = "group",
                SelectOnFocus = true,
                TypeAhead = true,
                FieldLabel = "Отправить группе",
                AnchorHorizontal = "100%"
            };
            groupCombo.Items.AddRange(new ListItemCollection<ListItem>() { new ListItem("ГРБС"), new ListItem("ФО"), new ListItem("Бюджетные учреждения"), new ListItem("Автономные учреждения"), new ListItem("Казенные учреждения") });
            groupCombo.Listeners.Change.Fn = @"function (grp){
                                            if(grp.getValue().indexOf('ГРБС') != -1){                                            
                                                #{grbsField}.setDisabled(true);
                                            }
                                            else{
                                                #{grbsField}.setDisabled(false)
                                            };
                                            if(grp.getValue().indexOf('ФО') != -1){
                                                #{pposField}.setDisabled(true)
                                            }
                                            else{
                                                #{pposField}.setDisabled(false)};
                                            }";

            var grbsPanel = new GridPanel
            {
                ID = "grbsPanel",
                Height = 520,
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                AutoScroll = true,
                Plugins =
                                    {
                                        new GridFilters
                                            {
                                                Local = false,
                                                Filters =
                                                    {
                                                        new StringFilter
                                                            {
                                                                DataIndex = "Name",
                                                                EmptyText = "Наименование или инн",
                                                            }
                                                    },
                                            },
                                    },
                Buttons =
                                    {
                                        new Button
                                            {
                                                Text = "Выбрать",
                                                Listeners = 
                                                {
                                                    Click = 
                                                {
                                                                 Handler = 
@"var count = Ext.getCmp('grbsPanel').getSelectionModel().getCount();
var rows = Ext.getCmp('grbsPanel').getSelectionModel().getSelections();
var value = '';
for (var i = 0; i < count; i++) {
    if (i != count - 1)
    {
        value += rows[i].data.Name + ', ';
    }
    else
    {
        value += rows[i].data.Name;
    }
}
#{grbsField}.setValue(value);
"
                                                }
                                                }
                                            }
                                    },
                Listeners =
                {
                    BeforeRender = { Handler = @"adressGrbsBookStore.load()" }
                }
            };

            grbsPanel.Store.Add(adressGrbsBookStore);

            grbsPanel.ColumnModel.AddColumn("ID", "ID", DataAttributeTypes.dtInteger).SetWidth(100).SetHidden(true);
            grbsPanel.ColumnModel.AddColumn("Name", "Наименование", DataAttributeTypes.dtString).SetWidth(700);

            grbsPanel.SelectionModel.Add(new CheckboxSelectionModel
            {
                ID = "checkSelModel",
            });

            var grbsField = new DropDownField
            {
                ID = "grbsField",
                FieldLabel = "Выбрать ГРБС",
                Name = "выбрать",
                AnchorHorizontal = "100%",
                Editable = false,
                Icon = Icon.ArrowDown,
                RowHeight = 1
            };
            grbsField.Component.Add(grbsPanel);
            grbsField.AllowBlur = true;

            var pposPanel = new GridPanel
            {
                ID = "pposPanel",
                Height = 500,
                AutoScroll = true,
                Plugins =
                                    {
                                        new GridFilters
                                            {
                                                Local = false,
                                                Filters =
                                                    {
                                                        new StringFilter
                                                            {
                                                                DataIndex = "Name",
                                                                EmptyText = "Наименование",
                                                            }
                                                    },
                                            },
                                    },
                Buttons =
                                    {
                                        new Button
                                            {
                                                Text = "Выбрать",
                                                Listeners = 
                                                {
                                                    Click = 
                                                {
                                                    Handler = 
@"var count = Ext.getCmp('pposPanel').getSelectionModel().getCount();
var rows = Ext.getCmp('pposPanel').getSelectionModel().getSelections();
var value = '';
for (var i = 0; i < count; i++) {
    if (i != count - 1)
    {
        value += rows[i].data.Name + ', ';
    }
    else
    {
        value += rows[i].data.Name;
    }
}
#{pposField}.setValue(value);
"
                                                }
                                                }
                                            }
                                    },
                Listeners = { BeforeRender = { Handler = @"adressPposBookStore.load()" } }
            };

            pposPanel.Store.Add(adressPposBookStore);

            pposPanel.ColumnModel.AddColumn("ID", "ID", DataAttributeTypes.dtInteger).SetWidth(100).SetHidden(true);
            pposPanel.ColumnModel.AddColumn("Name", "Наименование", DataAttributeTypes.dtString).SetWidth(700);

            pposPanel.SelectionModel.Add(new CheckboxSelectionModel
            {
                ID = "checkSelModel2",
            });

            var pposField = new DropDownField
            {
                ID = "pposField",
                FieldLabel = "Выбрать ФО",
                Name = "выбрать",
                AnchorHorizontal = "100%",
                Editable = false,
                Icon = Icon.ArrowDown,
                RowHeight = 1
            };
            pposField.Component.Add(pposPanel);
            pposField.AllowBlur = true;

            var subjectField = new TextField
            {
                ID = "subjectField",
                DataIndex = "message",
                FieldLabel = "Введите тему",
                AnchorHorizontal = "100%",
                AllowBlank = false,
                MaxLength = 43,
            };

            var messageField = new TextArea
            {
                ID = "messageField",
                DataIndex = "message",
                HideLabel = true,
                AnchorHorizontal = "100%",
                AnchorVertical = "40%",
                AllowBlank = false,
                MaxLength = 200,
            };

            var fileNameField = new TextArea
            {
                ID = "fileNameField",
                DataIndex = "fileNameField",
                Hidden = true
            };

            var impComboBox = new ComboBox { ID = "imbCb", FieldLabel = "  Важность" };
            impComboBox.Items.AddRange(new ListItemCollection<ListItem> { new ListItem("Важная"), new ListItem("Обычная") });
            impComboBox.SelectedIndex = 0;
            impComboBox.AnchorHorizontal = "100%";

            var writeButton = new Button
            {
                ID = "writeButton",
                Icon = Icon.Accept,
                Text = "Опубликовать",
                ToolTip = "Опубликовать новость",
                Disabled = false,
            };

            writeButton.DirectEvents.Click.Url = "/News/SendMessage";
            writeButton.DirectEvents.Click.EventMask.ShowMask = true;
            writeButton.DirectEvents.Click.EventMask.Msg = "Отправка...";
            writeButton.DirectEvents.Click.CleanRequest = true;
            writeButton.DirectEvents.Click.IsUpload = true;
            writeButton.DirectEvents.Click.FormID = "Form1";
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("Subject", "{0}.getValue()".FormatWith("subjectField"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("Message", "{0}.getValue()".FormatWith("messageField"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("grops", "{0}.getValue()".FormatWith("group"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("singleGrbs", Scope + ".getIds('{0}')".FormatWith("grbsPanel"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("singlePpos", Scope + ".getIds('{0}')".FormatWith("pposPanel"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("isSendAll", "{0}.getValue()".FormatWith("check"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("fileName", "{0}.getValue()".FormatWith("fileNameField"), ParameterMode.Raw));

            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("Importance", "{0}.getValue()".FormatWith("imbCb"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("numberAct", "{0}.getValue()".FormatWith("number"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.ExtraParams.Add(new Parameter("typeAct", "{0}.getValue()".FormatWith("combo"), ParameterMode.Raw));
            writeButton.DirectEvents.Click.Success =
                @"#{Window}.hide();outboxNews.reload();";

            writeButton.DirectEvents.Click.Failure =
                @"#{Window}.hide();";

            var escapeButton = new Button
            {
                ID = "escapeWrite",
                Icon = Icon.Cancel,
                Text = "Отмена",
                ToolTip = "Отмена"
            };
            escapeButton.Listeners.Click.Handler = @"#{Window}.hide();";

            var fileUploadField = new FileUploadField
            {
                ID = "file",
                ButtonText = "Выбрать файл",
                EmptyText = "Выберите приложение",
                Icon = Icon.Attach,
                AnchorHorizontal = "100%",
                DirectEvents =
                {
                    FileSelected =
                    {
                        Method = HttpMethod.POST,
                        CleanRequest = true,
                        Url = "/News/Upload",
                        IsUpload = true,
                        Before =
                            @"
fileName = {0}.getValue();
extraParams.fileName = fileName;".FormatWith("file"),
                        Success = @"
#{fileNameField}.setValue(result.extraParams.msg);
Ext.Msg.show({ title: 'Уведомление', msg: 'Документ загружен.', minWidth: 200, modal: true, icon: Ext.Msg.INFO, buttons: Ext.Msg.OK });
",

                        Failure = @"
if ( (result.extraParams != undefined) && (result.extraParams.responseText != undefined) )
    { Ext.Msg.show( { title: 'Ошибка', msg: result.extraParams.responseText, minWidth: 200, modal: true, icon: Ext.Msg.ERROR, buttons: Ext.Msg.OK } ); }
else
    { Ext.Msg.show( { title: 'Ошибка', msg: result.responseText, minWidth: 200, modal: true, icon: Ext.Msg.ERROR, buttons: Ext.Msg.OK } ); }
"
                    }
                }
            };
            fileUploadField.DirectEvents.FileSelected.EventMask.ShowMask = true;
            fileUploadField.DirectEvents.FileSelected.EventMask.Msg = "Загрузка файла...";

            var combo = new ComboBox { ID = "combo" };
            combo.Items.AddRange(new ListItemCollection<ListItem> { new ListItem("дней"), new ListItem("часов"), new ListItem("минут") });
            combo.SelectedIndex = 0;
            combo.AnchorHorizontal = "50%";

            var fieldSet = new FieldSet
            {
                AnchorHorizontal = "100%",
                Layout = LayoutType.HBox.ToString(),
                FieldLabel = "Актуально",
                Border = false,
            };

            fieldSet.Items.Add(new NumberField
            {
                ID = "number",
                AnchorHorizontal = "50%",
                AllowBlank = false,
                Value = 7
            });
            fieldSet.Items.Add(combo);

            var checkMenuItem = new Checkbox { ID = "check", FieldLabel = "Выбрать всех", Checked = false };
            checkMenuItem.Listeners.Change.Fn = @"function (check){#{group}.setDisabled(check.checked);
                                                       #{grbsField}.setDisabled(check.checked);
                                                       #{pposField}.setDisabled(check.checked);}";

            var win = new Window
            {
                ID = "Window",
                Title = "Новости",
                Width = 500,
                Height = 500,
                Modal = true,
                AutoRender = false,
                Hidden = true,
                Layout = LayoutType.Fit.ToString(),
                Items =
                                  {
                                      new FormPanel
                                          {
                                              ID = "form",
                                              MonitorValid = true,
                                              Layout = LayoutType.Anchor.ToString(),
                                              Items =
                                                  {
                                                      groupCombo,
                                                      grbsField,
                                                      pposField,
                                                      fileNameField,
                                                      checkMenuItem,
                                                      subjectField,
                                                      messageField,
                                                      impComboBox,
                                                      fileUploadField,
                                                      fieldSet,
                                                      new CompositeField
                                                          {
                                                              AnchorHorizontal = "100%",
                                                              Items =
                                                                  {
                                                                      writeButton,
                                                                      escapeButton
                                                                  }
                                                          },
                                                 },
                                             Listeners =
                                                 {
                                                    ClientValidation = 
                                                        {
                                                            Handler =
                                                            @"var val=true;if ((#{group}.getValue() == '')&&(#{grbsField}.getValue() == '' || #{grbsField}.getValue() == 'Выбрать ГРБС')
                                                            &&(#{pposField}.getValue() == '' || #{pposField}.getValue() == 'Выбрать ФО')&&(#{check}.getValue() == false))
                                                            {val = false;}
                                                            if(val)
                                                            {
                                                            #{writeButton}.setDisabled(!valid);}
                                                            else{#{writeButton}.setDisabled(true);}"
                                                        },
                                                     BeforeHide = 
                                                         {
                                                             Handler =
                                                                @"{0}.value='';".FormatWith("messageField")
                                                         },
                                                 }
                                          }
                                },
            };

            return new List<Component> { win };
        }
    }
}
