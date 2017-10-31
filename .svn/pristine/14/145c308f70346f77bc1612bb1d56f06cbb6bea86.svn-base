using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Models;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Presentation.Views
{
    public class AreaDetailView : View
    {
        private const string DatasourceAreaId = "dsArea";

        public int? AreaId
        {
            get { return String.IsNullOrEmpty(Params["id"]) ? null : (int?)Convert.ToInt32(Params["id"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("AreaDetialView", Resource.AreaDetialView);
            }

            // Регистрируем событие на закрытие окна
            resourceManager.AddScript(@"
var win = parentAutoLoadControl;
if (typeof(win.events.beforehide) == 'object'){{
    win.events.beforehide.clearListeners();
}}
win.addListener('beforehide', onBeforeWindowHideEventHandler);
");
            
            page.Controls.Add(GetAreaAttributeStore());

            FileListPanel fileListPanel = CreateFilesPanel();

            var filesStore = fileListPanel.GetFileListStore();
            filesStore.AutoLoad = false;
            page.Controls.Add(filesStore);

            var view = new Viewport
                           {
                               ID = "viewPortMain",
                               Items =
                                   {
                                       new BorderLayout
                                           {
                                               North = { Items = { CreateTopPanel() } },
                                               Center = { Items = { CreateAttributePanel() } },
                                               South = { Items = { fileListPanel }, Collapsible = true, Split = true }
                                           }
                                   }
                           };
            return new List<Component> { view, GetTerritoryLookupWindow(), fileListPanel.GetFileUploadWindow() };
        }

        private Panel CreateTopPanel()
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceAreaId) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Listeners = { Click = { Handler = "SaveAreaAttributes();" } }
            });

            toolbar.Add(new ToolbarSpacer(50));

            toolbar.Add(new Button
            {
                ID = "btnToEdit",
                Icon = Icon.UserEdit,
                Text = "Передать на редактирование",
                Disabled = true,
                Listeners = { Click = { Handler = "SaveAreaAttributes({0});".FormatWith((int)InvAreaStatus.Edit) } }            
            });

            toolbar.Add(new Button
            {
                ID = "btnToReview",
                Icon = Icon.UserEarth,
                Text = "Передать на рассмотрение",
                Disabled = true,
                Listeners = { Click = { Handler = "SaveAreaAttributes({0});".FormatWith((int)InvAreaStatus.Review) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnToAccepted",
                Icon = Icon.UserTick,
                Text = "Принять",
                Disabled = true,
                Listeners = { Click = { Handler = "SaveAreaAttributes({0});".FormatWith((int)InvAreaStatus.Accepted) } }
            });

            return new Panel
            {
                ID = "topPanel",
                Height = 25,
                Border = false,
                TopBar = { toolbar }
            };
        }

        private Component CreateAttributePanel()
        {
            var form = new FormPanel
            {
                ID = "areaDetailForm",
                Border = false,
                Url = "/AreaDetail/Save",
                MonitorValid = true,
                AutoScroll = true,
                LabelSeparator = String.Empty,
                LabelPad = 10,
                Padding = 5,
                LabelWidth = 200,
                DefaultAnchor = "90%"
            };

            form.Listeners.ClientValidation.Handler = @"
btnSave.setDisabled(!(valid && comboRefStatus.getValue()==1));
btnSave.setTooltip(valid ? (comboRefStatus.getValue()==1 ? 'Сохранить': 'Можно только Принять или Отправить на доработку') : 'В карточке не все поля заполнены, либо имеют некорректные значения!');
";

            form.Items.Add(new TextField { ID = "ID", DataIndex = "ID", Hidden = true });

            form.Items.Add(new TextField { ID = "RegNumber", FieldLabel = "Регистрационный номер", DataIndex = "RegNumber", AllowBlank = true, MaxLength = 25 });

            var comboboxRefStatus = new ComboBox
            {
                ID = "comboRefStatus",
                FieldLabel = "Состояние",
                Items =
                    {
                      new ListItem { Value = "1", Text = "На редактировании" },
                      new ListItem { Value = "2", Text = "На рассмотрении у координатора" },
                      new ListItem { Value = "3", Text = "Принят" },
                    },
                Editable = false,
                ReadOnly = true,
                StyleSpec = "font-weight:bold;",
                HideMode = HideMode.Visibility,
                TriggerAction = TriggerAction.All,
                Mode = DataLoadMode.Local,
                Resizable = false,
                SelectOnFocus = false,
                DataIndex = "RefStatusId",
                AllowBlank = false
            };
            form.Items.Add(comboboxRefStatus);
            
            var lookupRefTerritory = new TriggerField
            {
                ID = "lfRefTerritoryName",
                Editable = false,
                FieldLabel = "Муниципальное образование",
                DataIndex = "RefTerritoryName",
                AllowBlank = false,
                TriggerIcon = TriggerIcon.Ellipsis,
                Listeners = { TriggerClick = { Handler = "wTerritoryLookup.show('lfRefTerritoryName');" } }
            };

            form.Items.Add(lookupRefTerritory);
            form.Items.Add(new TextField { ID = "RefTerritoryId", Hidden = true, DataIndex = "RefTerritoryId" });

            form.Items.Add(new TextField { ID = "Location",     DataIndex = "Location", AllowBlank = true, FieldLabel = "Местоположение" });
            form.Items.Add(new TextField { ID = "CadNumber",    DataIndex = "CadNumber", AllowBlank = true, FieldLabel = "Кадастровый номер", MaxLength = 25 });
            form.Items.Add(new NumberField { ID = "Area",       DataIndex = "Area", AllowBlank = true, AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = ".", FieldLabel = "Площадь, км.кв." });
            form.Items.Add(new TextField { ID = "Category",     DataIndex = "Category", AllowBlank = true, FieldLabel = "Категория земель" });

            var container1 = new FieldSet
                                {
                                    Title = "Собственник (пользователь) земельного участка:",
                                    Collapsible = true,
                                    Collapsed = false,
                                    AutoHeight = true,
                                    Border = false,
                                    StyleSpec = "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                                    Layout = LayoutType.Form.ToString(),
                                    
                                    LabelSeparator = String.Empty,
                                    LabelPad = 10,
                                    Padding = 5,
                                    LabelWidth = 200,
                                    DefaultAnchor = "0"
                                };
            form.Items.Add(container1);
            container1.Items.Add(new TextField { ID = "Owner", DataIndex = "Owner", AllowBlank = true, FieldLabel = "Организация" }); 
            container1.Items.Add(new TextField { ID = "Head", DataIndex = "Head", AllowBlank = true, FieldLabel = "ФИО руководителя" });
            container1.Items.Add(new TextField { ID = "Contact", DataIndex = "Contact", AllowBlank = true, FieldLabel = "ФИО ответственного лица" });
            container1.Items.Add(new TextField { ID = "Email", DataIndex = "Email", AllowBlank = true, FieldLabel = "E-mail ответственного лица", });
            container1.Items.Add(new TextField { ID = "Phone", DataIndex = "Phone", AllowBlank = true, FieldLabel = "Телефон ответственного лица" });

            form.Items.Add(new TextField { ID = "PermittedUse", DataIndex = "PermittedUse", AllowBlank = true, FieldLabel = "Разрешенное использование земельного участка", ToolTips = { new ToolTip { Html = "(в соответствии с Правилами землепользования и застройки муниципального образования)" } } });
            form.Items.Add(new TextField { ID = "ActualUse",    DataIndex = "ActualUse", AllowBlank = true, FieldLabel = "Обременения", ToolTips = { new ToolTip { Html = "(фактическое использование земельного участка)" } } });
            form.Items.Add(new TextField { ID = "Documentation", DataIndex = "Documentation", AllowBlank = true, FieldLabel = "Наличие градостроительного плана земельного участка", ToolTips = { new ToolTip { Html = "Обеспеченность территории, в которую входит земельный участок, документами территориального планирования, \nправилами землепользования и застройки (градостроительные регламенты), \nдокументацией по планировке территории, инженерным изысканиям (в том числе топографические съемки различных масштабов)" } } });
            form.Items.Add(new TextField { ID = "Limitation",   DataIndex = "Limitation", AllowBlank = true, FieldLabel = "Ограничения использования земельного участка", ToolTips = { new ToolTip { Html = "(санитарно-защитные зоны, охранные зоны и др.)" } } });
            form.Items.Add(new TextField { ID = "PermConstr",   DataIndex = "PermConstr", AllowBlank = true, FieldLabel = "Параметры разрешенного строительства объекта капитального строительства" });
            form.Items.Add(new TextField { ID = "Relief",       DataIndex = "Relief", AllowBlank = true, FieldLabel = "Особенности рельефа территории участка", ToolTips = { new ToolTip { Html = "Наличие на земельном участке водоемов, зеленых насаждений (деревья, кустарники, особо ценные породы), степень заболоченности и залесенности" } } });

            var container2 = new FieldSet
            {
                Title = "Наличие (удаленность от земельного участка) объектов транспортной инфраструктуры, в т.ч.:",
                Collapsible = true,
                Collapsed = false,
                AutoHeight = true,
                Border = false,
                StyleSpec = "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                Layout = LayoutType.Form.ToString(),
                FormGroup = true,
                LabelSeparator = String.Empty,
                LabelPad = 10,
                Padding = 5,
                LabelWidth = 200,
                DefaultAnchor = "0"
            };
            form.Items.Add(container2);
            container2.Items.Add(new TextField { ID = "Road", DataIndex = "Road", AllowBlank = true, FieldLabel = "Автомобильные дороги", ToolTips = { new ToolTip { Html = "Автомобильные дороги с твердым покрытием (асфальтобетон, бетон), муниципальный транспорт (краткая характеристика)" } } });
            container2.Items.Add(new TextField { ID = "Station", DataIndex = "Station", AllowBlank = true, FieldLabel = "Железные дороги", ToolTips = { new ToolTip { Html = "Железнодорожная магистраль, станция, тупик, ветка, подкрановые пути, краткая характеристика (в том числе электрифицированные, неэлектрифицированные)" } } });
            container2.Items.Add(new TextField { ID = "Pier", DataIndex = "Pier", AllowBlank = true, FieldLabel = "Водный транспорт", ToolTips = { new ToolTip { Html = "Водный транспортный путь, пристань, причальная стенка и др. (краткая характеристика);" } } });
            container2.Items.Add(new TextField { ID = "Airport", DataIndex = "Airport", AllowBlank = true, FieldLabel = "Аэропорт", ToolTips = { new ToolTip { Html = "(грузовые и пассажирские перевозки), краткая характеристика " } } });

            var container3 = new FieldSet
            {
                Title = "Наличие (удаленность от земельного участка) сетей ИТО и объектов инженерной инфраструктуры, в т.ч.:",
                Collapsible = true,
                Collapsed = false,
                AutoHeight = true,
                Border = false,
                StyleSpec = "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                Layout = LayoutType.Form.ToString(),
                FormGroup = true,
                LabelSeparator = String.Empty,
                LabelPad = 10,
                Padding = 5,
                LabelWidth = 200,
                DefaultAnchor = "0"
            };
            form.Items.Add(container3);
            container3.Items.Add(new TextField { ID = "Plumbing", DataIndex = "Plumbing", AllowBlank = true, FieldLabel = "Объекты водоснабжения", ToolTips = { new ToolTip { Html = "(артезианские скважины, насосные станции, водонапорные башни, магистральные сети), тип, мощность объектов водоснабжения, возможность и условия подключения;" } } });
            container3.Items.Add(new TextField { ID = "Sewage", DataIndex = "Sewage", AllowBlank = true, FieldLabel = "Канализация", ToolTips = { new ToolTip { Html = "тип: бытовая, ливневая, канализационная насосная станция, очистные сооружения, мощность, возможность и условия подключения" } } });
            container3.Items.Add(new TextField { ID = "Gas", DataIndex = "Gas", AllowBlank = true, FieldLabel = "Объекты газоснабжения", ToolTips = { new ToolTip { Html = "магистральные сети, распределительные устройства: тип, мощность, возможность и условия подключения" } } });
            container3.Items.Add(new TextField { ID = "Electricity", DataIndex = "Electricity", AllowBlank = true, FieldLabel = "Объекты электроснабжения", ToolTips = { new ToolTip { Html = "электрические линии, подстанции: тип и мощность, возможность и условия подключения" } } });
            container3.Items.Add(new TextField { ID = "Heating", DataIndex = "Heating", AllowBlank = true, FieldLabel = "Объекты теплоснабжения", ToolTips = { new ToolTip { Html = "центральные тепловые подстанции, сети: тип и мощность, возможность и условия подключения" } } });
            container3.Items.Add(new TextField { ID = "Landfill", DataIndex = "Landfill", AllowBlank = true, FieldLabel = "Полигон для отходов", ToolTips = { new ToolTip { Html = "полигон для размещения бытовых, промышленных и производственных отходов, тип, мощность, возможность и условия дополнительного размещения отходов" } } });
            container3.Items.Add(new TextField { ID = "Telephone", DataIndex = "Telephone", AllowBlank = true, FieldLabel = "Телефонизация площадки" });

            ////form.Items.Add(new Container { Items = { new DisplayField { Text = String.Empty } } });
            form.Items.Add(new TextField { ID = "Connectivity", DataIndex = "Connectivity", AllowBlank = true, FieldLabel = "Условия подключения к сетям ИТО", ToolTips = { new ToolTip { Html = "Технические условия подключения объекта капитального строительства к сетям инженерно-технического обеспечения" } } });
            form.Items.Add(new TextField { ID = "Fee",          DataIndex = "Fee", AllowBlank = true, FieldLabel = "Плата за подключение к сетям ИТО", ToolTips = { new ToolTip { Html = "Информация о плате за подключение объекта капитального строительства к сетям инженерно - технического обеспечения" } } });
            form.Items.Add(new TextField { ID = "DistanceZones", DataIndex = "DistanceZones", AllowBlank = true, FieldLabel = "Расстояния", ToolTips = { new ToolTip { Html = "Расстояние от земельного участка до жилых массивов, водоемов, природоохранных и санитарно-защитных зон" } } });
            form.Items.Add(new TextField { ID = "Buildings",    DataIndex = "Buildings", AllowBlank = true, FieldLabel = "Строения", ToolTips = { new ToolTip { Html = "Перечень и характеристика зданий, сооружений и других объектов, находящихся на земельном участке" } } });
            form.Items.Add(new TextField { ID = "Resources",    DataIndex = "Resources", AllowBlank = true, FieldLabel = "Ресурсы", ToolTips = { new ToolTip { Html = "Наличие (удаленность от земельного участка) природных, лесных ресурсов, месторождений полезных ископаемых, их характеристика" } } });
            form.Items.Add(new TextField { ID = "Settlement",   DataIndex = "Settlement", AllowBlank = true, FieldLabel = "Население", ToolTips = { new ToolTip { Html = "Близость земельного участка к населенным пунктам, количество жителей, уровень занятости населения, демографическая ситуация, основные градообразующие отрасли экономики" } } });
            form.Items.Add(new TextField { ID = "ObjectEducation", DataIndex = "ObjectEducation", AllowBlank = true, FieldLabel = "Образовательные учреждения", ToolTips = { new ToolTip { Html = "Близость земельного участка к объектам среднего специального и высшего образования, наличие кадрового потенциала территории и источники его формирования" } } });

            var container4 = new FieldSet
            {
                Title = "Близость земельного участка к объектам:",
                Collapsible = true,
                Collapsed = false,
                AutoHeight = true,
                Border = false,
                StyleSpec = "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                Layout = LayoutType.Form.ToString(),
                FormGroup = true,
                LabelSeparator = String.Empty,
                LabelPad = 10,
                Padding = 5,
                LabelWidth = 200,
                DefaultAnchor = "0"
            };
            form.Items.Add(container4);
            container4.Items.Add(new TextField { ID = "ObjectHealth", DataIndex = "ObjectHealth", AllowBlank = true, FieldLabel = "здравоохранения", ToolTips = { new ToolTip { Html = "поликлиники, больницы общего и специализированного профиля, здравпункты, аптеки, фельдшерско-акушерские пункты и т.д." } } });
            container4.Items.Add(new TextField { ID = "ObjectSocSphere", DataIndex = "ObjectSocSphere", AllowBlank = true, FieldLabel = "социальной сферы", ToolTips = { new ToolTip { Html = "детские сады, школы, места проведения досуга населения и т.д." } } });
            container4.Items.Add(new TextField { ID = "ObjectServices", DataIndex = "ObjectServices", AllowBlank = true, FieldLabel = "сферы услуг", ToolTips = { new ToolTip { Html = "магазины, кафе, столовые, бытовое обслуживание населения и т.д." } } });
            container4.Items.Add(new TextField { ID = "Hotels", DataIndex = "Hotels", AllowBlank = true, FieldLabel = "гостинично-деловой сферы", ToolTips = { new ToolTip { Html = "(гостиницы, бизнес-центры, офисы компаний и т.д." } } });
            
            ////form.Items.Add(new Container { Items = { new DisplayField { Text = String.Empty } } });
            form.Items.Add(new NumberField { ID = "CoordinatesLat", DataIndex = "CoordinatesLat", AllowBlank = true, AllowDecimals = true, DecimalSeparator = ".", DecimalPrecision = 6, FieldLabel = "Координаты на карте GoogleMaps, широта, град." });
            form.Items.Add(new NumberField { ID = "CoordinatesLng", DataIndex = "CoordinatesLng", AllowBlank = true, AllowDecimals = true, DecimalSeparator = ".", DecimalPrecision = 6, FieldLabel = "Координаты на карте GoogleMaps, долгота, град." });

            //// TODO: прикрутить выбор координат как тут: http://www.3planeta.com/googlemaps/karty-google-maps.html

            ////form.Items.Add(new Container { Items = { new DisplayField { Text = String.Empty } } });
            form.Items.Add(new TextField { ID = "Note",         DataIndex = "Note", AllowBlank = true, FieldLabel = "Причина возврата на доработку" });
            form.Items.Add(new DateField { ID = "CreatedDate", DataIndex = "CreatedDate", AllowBlank = true, FieldLabel = "Дата создания", Format = "d-m-Y H:mm", ReadOnly = true });
            form.Items.Add(new DateField { ID = "AdoptionDate", DataIndex = "AdoptionDate", AllowBlank = true, FieldLabel = "Дата принятия", Format = "d-m-Y H:mm", ReadOnly = true });

            form.Items.Add(new TextField { ID = "CreateUser", DataIndex = "CreateUser", AllowBlank = true, FieldLabel = "Автор", StyleSpec = "color:#AAAAAA;", ReadOnly = true });
            
            return form;
        }

        private FileListPanel CreateFilesPanel()
        {
            List<FileListPanelColumn> panelColumns = new List<FileListPanelColumn>
            {
                new FileListPanelColumn 
                    { 
                        Column = new Column
                                     {
                                         ColumnID = "fileType",
                                         Header = "Тип файла",
                                         DataIndex = "FileType",
                                         Width = 200,
                                         MenuDisabled = true,
                                         Editable = true
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = false
                    },
                new FileListPanelColumn 
                    { 
                        Column = new Column
                                     {
                                         ColumnID = "fileName",
                                         Header = "Имя файла",
                                         DataIndex = "FileName",
                                         Width = 200,
                                         MenuDisabled = true,
                                         Editable = true
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = false
                    }
            };

            List<RecordField> storeFields = new List<RecordField>
                                                {
                                                    new RecordField("ID"),
                                                    new RecordField("FileType"),
                                                    new RecordField("FileName")
                                                };

            UrlWithParameters loadController = new UrlWithParameters
            {
                Url = "/AreaDetail/GetFileTable",
                ParameterCollection = new ParameterCollection { new Parameter("areaId", "ID.getValue()", ParameterMode.Raw) }
            };

            UrlWithParameters fileUploadController = new UrlWithParameters
            {
                Url = "/AreaDetail/CreateOrUpdateFileWithUploadBody",
                ParameterCollection = new ParameterCollection { new Parameter("areaId", "ID.getValue()", ParameterMode.Raw) }
            };

            UrlWithParameters fileDownloadController = new UrlWithParameters
            {
                Url = "/AreaDetail/DownloadFile"
            };

            UrlWithParameters updateController = new UrlWithParameters
            {
                Url = "/AreaDetail/SaveFileNames",
                ParameterCollection = new ParameterCollection { new Parameter("areaId", "ID.getValue()", ParameterMode.Raw) }
            };

            var fileListPanel = new FileListPanel(
                panelColumns,
                true,
                storeFields,
                loadController,
                updateController,
                fileUploadController,
                fileDownloadController,
                "areaDetailForm");

            fileListPanel.Title = "Визуализация площадки";
            fileListPanel.Collapsible = false;
            fileListPanel.Height = 150;
            fileListPanel.Border = false;
            fileListPanel.Disabled = true;

            var toolbar = fileListPanel.TopBar[0];

            // Допиливаем существующий функционал у Toolbar-а
            Button btnAdd = (Button)toolbar.Items[0];
            btnAdd.Text = null;

            var btnRefresh = new Button
            {
                ID = "btnRefreshFiles",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Visible = true,
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(FileListPanel.StoreId) } }
            };
            toolbar.Items.Insert(0, btnRefresh);

            toolbar.Add(new Button
            {
                ID = "btnSaveFiles",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Visible = true,
                Listeners =
                {
                    Click =
                    {
                        Handler = "{0}.save();".FormatWith(FileListPanel.StoreId)
                    }
                }
            });

            return fileListPanel;
        }

        private Store GetAreaAttributeStore()
        {
            var store = new Store { ID = DatasourceAreaId, AutoLoad = true };
            store.SetHttpProxy("/AreaDetail/Load");
            store.BaseParams.Add(new Parameter("areaId", AreaId.ToString(), ParameterMode.Value));
            var reader = new JsonReader
                             {
                                 IDProperty = "ID",
                                 Root = "data"
                             };
            
            foreach (var field in typeof(AreaDetailViewModel).GetProperties())
            {
                if (field.PropertyType == typeof(DateTime) || field.PropertyType == typeof(DateTime?))
                {
                    reader.Fields.Add(new RecordField(field.Name, RecordFieldType.Date, "Y-m-dTH:i:s.u"));
                }
                else
                {
                    reader.Fields.Add(new RecordField(field.Name));    
                }
            }

            store.Reader.Add(reader);

            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке карточки', response.responseText);";
            store.Listeners.DataChanged.Handler = @"
var record = this.getAt(0) || {};
var form = areaDetailForm.getForm();
form.loadRecord(record);
EnableChangeStatusButtons(comboRefStatus.getValue());
areaDetailForm.validate();
//comboRefStatus.fireEvent('select');
SetFieldsEditableOption(comboRefStatus.getValue());
ResetDirtyAttributeOnFormItems(areaDetailForm);
";
            store.Listeners.Load.Handler = @"
if (ID.getValue()>0){{
  {0}.setDisabled(false);
  {1}.reload();
}}
".FormatWith(FileListPanel.PanelId, FileListPanel.StoreId);

            return store;
        }

        private Window GetTerritoryLookupWindow()
        {
            var w = new Window();

            w.ID = "wTerritoryLookup";
            w.Title = "Выбор варианта";
            w.Width = 600;
            w.Height = 400;
            w.Hidden = true;

            w.AutoLoad.TriggerEvent = "show";
            w.AutoLoad.Url = "/Entity/Book?objectKey={0}".FormatWith(D_Territory_RF.Key);
            w.AutoLoad.Mode = LoadMode.IFrame;
            var btn = new Button { ID = "btTerritorySelect", Text = "Выбрать", Disabled = true };
            btn.Listeners.Click.Handler = @"
 var record = wTerritoryLookup.getBody().Extension.entityBook.selectedRecord;
 lfRefTerritoryName.setValue(record.data.NAME);
 RefTerritoryId.setValue(record.data.ID);
 #{wTerritoryLookup}.hide();";
            w.Buttons.Add(btn);

            // Установка обработчика выбора варианта
            w.Listeners.Update.Handler = @"
function rowSelected(record) { 
    wTerritoryLookup.getBody().Extension.entityBook.selectedRecord = record;
    btTerritorySelect.setDisabled(false); 
}
wTerritoryLookup.getBody().Extension.entityBook.onRowSelect = rowSelected;
";

            w.AddAfterClientInitScript("wTerritoryLookup.MetaData = {};");
            return w;
        }
    }
}
