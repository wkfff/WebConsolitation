using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Common.Constants;
using Krista.FM.Domain;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.RIA.Extensions.DebtBook.Services.ControlRelationships;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.ViewBuilders
{
    public class BebtBookFormView : View
    {
        private readonly IDebtBookExtension bebtBookExtension;
        private readonly IParametersService parametersService;

        public BebtBookFormView(IVariantProtocolService protocolService, IDebtBookExtension bebtBookExtension, IParametersService parametersService)
        {
            this.bebtBookExtension = bebtBookExtension;
            this.parametersService = parametersService;
            Fields = new Dictionary<string, ColumnState>();
            ControlRelationships = new List<ControlRelationship>();

            Readonly = !CheckEditable(protocolService, bebtBookExtension);
        }
        
        public int TabRegionType { get; set; }

        /// <summary>
        /// Набор контрольных соотношений
        /// </summary>
        public IList<ControlRelationship> ControlRelationships { get; set; }
        
        /// <summary>
        /// Преднастроенные свойства отображения полей.
        /// </summary>
        public Dictionary<string, ColumnState> Fields { get; set; }
        
        /// <summary>
        /// Количество полей в первом столбце формы.
        /// </summary>
        public int FieldsPerColumn { get; set; }

        public new string Title { get; set; }

        public IEntity Entity { get; set; }
        
        public IPresentation Presentation { get; set; }
        
        public IViewService ViewService { get; set; }
        
        public bool Readonly { get; set; }
        
        public int RecordId { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            RegisterResources(page);

            // Разрешаем редактирование, если субъект явно запросил изменение договора
            if (IsForceUnlock())
            {
                Readonly = false;
            }

            //------------------------------------------------------------------
            // Настраиваем источники данных
            Store dstoreEntity = InitializeStore();
            page.Controls.Add(dstoreEntity);

            Store dstoreChilds = null;

            // Для интерфейса "ИФ.Предельные объемы по муниципальным долговым обязательствам.Оренбург"
            // не использовать данные детализации (по идее детализация нужна только для Ярославля));););
            if (NeedCreateChilds())
            {
                dstoreChilds = InitializeStoreChilds();
                page.Controls.Add(dstoreChilds);
            }

            FormPanel detailsForm = new FormPanel
            {
                ID = "DetailsForm",
                Border = false,
                Url = "/Entity/Save/",
                Layout = "form",
                LabelWidth = 230,
                Padding = 5,
                TrackResetOnLoad = true,
                MonitorValid = true
            };

            if (!Readonly)
            {
                detailsForm.Listeners.ClientValidation.Handler =
                    "#{btnSave}.setDisabled(!valid);#{btnSave}.setTooltip(valid ? 'Сохранить договор' : 'В карточке не все поля заполнены либо имеют некорректные значения');";
            }

            detailsForm.Items.Add(new TextField { ID = "ID", Hidden = true });
            detailsForm.Items.Add(new TextField { ID = "Unlock", Hidden = true, Value = IsForceUnlock() });

            //------------------------------------------------------------------
            // Формируем форму карточки
            IPresentation presentation = Presentation;
            if (presentation != null)
            {
                CreareForm(presentation, dstoreEntity, dstoreChilds, detailsForm);
            }
            else
            {
                CreateFormColumns(dstoreEntity, detailsForm);
            }

            ResourceManager.GetInstance(page).RegisterOnReadyScript(
                ViewService.GetClientScript());

            // Устанавливаем тип закладки
            ResourceManager.GetInstance(page).RegisterOnReadyScript(
                "Extension.DebtBookShowDetailsView.regionType = {0};".FormatWith(TabRegionType));

            var pagingToolbar = new PagingToolbar
            {
                ID = "ToolbarsPanel",
                PageSize = 1,
                DisplayMsg = String.Empty,
                BeforePageText = "Договор",
                StoreID = dstoreEntity.ID
            };

            var idFilter = new Hidden
            {
                ID = "idFilter",
                Text = Convert.ToString(RecordId),
                AutoDataBind = true
            };
            idFilter.Listeners.Change.Handler = "#{ToolbarsPanel}.changePage(1);";
            idFilter.Listeners.Change.Delay = 30;
            pagingToolbar.Items.Add(idFilter);

            var txtFilter = new Hidden
            {
                ID = "txtFilter",
                Text = String.Empty,
                AutoDataBind = true
            };
            txtFilter.Listeners.Change.Handler = "#{ToolbarsPanel}.changePage(1);";
            txtFilter.Listeners.Change.Delay = 30;
            pagingToolbar.Items.Add(txtFilter);

            var btnSave = new Button
            {
                ID = "btnSave",
                ToolTip = "Сохранить договор",
                Icon = Icon.Disk,
                Disabled = Readonly
            };
            btnSave.Listeners.Click.Handler = "ViewPersistence.save.call(window, true);";
            pagingToolbar.Items.Add(btnSave);

            if (bebtBookExtension.UserRegionType == UserRegionType.Subject)
            {
                var btnUnlock = new Button
                {
                    ID = "btnUnlock",
                    ToolTip = "Разблокировать договор",
                    Icon = Icon.LockEdit,
                    Disabled = Readonly
                };
                btnUnlock.Listeners.Click.Fn = "unlock";
                pagingToolbar.Items.Add(btnUnlock);
            }

            pagingToolbar.Items.Add(new ToolbarFill());

            var mainPanel = new Panel
            {
                Title = Title,
                Border = false,
                AutoWidth = false,
                AnchorHorizontal = "93%",
                AutoScroll = true
            };

            mainPanel.TopBar.Add(pagingToolbar);

            mainPanel.Items.Add(detailsForm);

            page.Controls.Add(CreateBookWindow());

            return new List<Component> { mainPanel };
        }

        protected virtual bool NeedCreateChilds()
        {
            return Entity.ObjectKey != F_S_SchBLimit.Key && Entity.ObjectKey != F_S_ContrDebt.Key;
        }

        private static bool CheckEditable(IVariantProtocolService protocolService, IDebtBookExtension debtBookExtension)
        {
            T_S_ProtocolTransfer status = protocolService.GetStatus(
                debtBookExtension.Variant.Id,
                debtBookExtension.UserRegionId);

            return status.RefStatusSchb.ID == 1
                || debtBookExtension.UserRegionType == UserRegionType.Subject;
        }

        private void CreareForm(IPresentation presentation, Store dstoreEntity, Store dstoreChilds, FormPanel detailsForm)
        {
            var groups = new Dictionary<string, FieldSet>();

            //--------------------------------------------------------------
            // Информация по договору
            var formFields = new Dictionary<string, Field>();
            foreach (IDataAttribute attribute in presentation.Attributes.Values)
            {
                if (attribute.Name == "ID")
                {
                    continue;
                }

                // Если отрибут входит в групу, то создаем для него группу
                FieldSet container = null;
                if (attribute.GroupTags.IsNotNullOrEmpty())
                {
                    container = GetContainer(attribute, groups);
                }
                else
                {
                    // Добавляем в источник данных скрытые обязательные поля
                    if (!attribute.IsNullable)
                    {
                        dstoreEntity.Reader[0].Fields.AddRecordField(attribute);
                        if (dstoreChilds != null)
                        {
                            dstoreChilds.Reader[0].Fields.AddRecordField(attribute);
                        }

                        // Добавляем скрытое поле в форму
                        detailsForm.Items.AddHiddenFormField(attribute);
                    }
                }

                if (container != null)
                {
                    // Добавляем поле в источники данных
                    dstoreEntity.Reader[0].Fields.AddRecordField(attribute);
                    if (dstoreChilds != null)
                    {
                        var recordField = dstoreChilds.Reader[0].Fields.AddRecordField(attribute);

                        if (attribute.Name.ToUpper() == "CHARGEDATE")
                        {
                            recordField.AllowBlank = false;
                        }
                    }

                    // Для ссылочных полей создаем скрытое поле для хранения значения)
                    if (attribute.Class == DataAttributeClassTypes.Reference)
                    {
                        container.Items.Add(new TextField
                        {
                            ID = attribute.Name.ToUpper(),
                            FieldLabel = attribute.Name.ToUpper(),
                            Hidden = true
                        });
                    }

                    Field field = container.Items.AddFormField(attribute, new ColumnState { Caption = attribute.Caption, Width = 300 });
                    field.ReadOnly = Readonly || field.ReadOnly;

                    // Для поселений поле "Район" доступно только для чтения
                    if (attribute.Name == "RefRegion" && bebtBookExtension.UserRegionType == UserRegionType.Settlement)
                    {
                        field.ReadOnly = true;
                    }

                    formFields.Add(field.ID.ToUpper(), field);
                }
            }

            bool changesExists = false;

            // Размещаем группы на форме
            foreach (KeyValuePair<string, FieldSet> pair in groups)
            {
                if (pair.Key.StartsWith("Изменение;"))
                {
                    changesExists = true;
                    Panel changesPanel = detailsForm.Items.Find(x => x.ID == "changesPanel") as Panel;
                    if (changesPanel == null)
                    {
                        changesPanel = new Panel
                        {
                            ID = "changesPanel",
                            Border = false,
                            Hidden = true,
                            StyleSpec = "margin-top: 2px; margin-bottom: 2px;"
                        };
                       
                        detailsForm.Items.Add(changesPanel);
                    }

                    changesPanel.Items.Add(pair.Value);
                }
                else
                {
                    detailsForm.Items.Add(pair.Value);
                }
            }

            // Настраиваем вычисляемые поля
            foreach (var item in Fields)
            {
                if (formFields.ContainsKey(item.Key.ToUpper()))
                {
                    var field = formFields[item.Key.ToUpper()];
                    field.ReadOnly = true;

                    var keySigns = new char[] { '+', '-', '*', '/', '(', ')' };
                    
                    string formula = item.Value.CalcFormula.Replace(" ", String.Empty);
                    var lexema = new StringBuilder();
                    var jscrFormula = new StringBuilder();

                    // Проходим по формуле и выделяем переменные, разграниченные ключевыми символами
                    // Добавлен лишний ключевой знак - чтобы не потерять последнюю лексему в формуле
                    foreach (char c in formula + keySigns[0]) 
                    {
                        if (keySigns.Contains(c))
                        {
                            if (lexema.Length > 0)
                            {
                                if (formFields.ContainsKey(lexema.ToString()))
                                {
                                    jscrFormula.AppendFormat("{0}.getValue()", lexema);
                                }
                                else
                                {
                                    jscrFormula.Append(lexema);
                                }

                                lexema.Remove(0, lexema.Length);
                            }
                            
                            jscrFormula.Append(c);
                        }
                        else
                        {
                            lexema.Append(c);
                        }
                    }

                    jscrFormula.RemoveLastChar();

                    string[] expParts = item.Value.CalcFormula.Split(keySigns, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string expPart in expParts)
                    {
                        var expPartTrimmed = expPart.Trim();
                        if (formFields.ContainsKey(expPartTrimmed))
                        {
                            var sourceField = formFields[expPartTrimmed];
                            sourceField.AddListener("change", new JFunction("{0}.setValue({1})".FormatWith(item.Key, jscrFormula.ToString())));
                        }
                    }
                }
            }

            // Устанавливаем серый цвет у ReadOnly-полей
            foreach (var field in formFields.Values)
            {
                if (field.ReadOnly)
                {
                    field.StyleSpec = "background: LightGray;";
                }
            }

            // Добавляем системные атрибуты в грид изменений
            if (changesExists)
            {
                if (dstoreChilds != null)
                {
                    dstoreChilds.Reader[0].Fields.AddRecordField(Entity.Attributes["ParentID"]);
                }
            }

            if (dstoreChilds != null)
            {
                if (ViewService is CreditViewService)
                {
                    // По умолчанию указываем тип кредита
                    dstoreChilds.Reader[0].Fields.First(x => x.Name == "REFTYPECREDIT").DefaultValue =
                        Convert.ToString(((CreditViewService)ViewService).CreditTypeId);
                }

                // По умолчанию указываем район пользователя
                if (TabRegionType == (int)bebtBookExtension.UserRegionType
                    || (TabRegionType == 2 && bebtBookExtension.UserRegionType == UserRegionType.Town))
                {
                    dstoreChilds.Reader[0].Fields.First(x => x.Name == "REFREGION").DefaultValue =
                        Convert.ToString(bebtBookExtension.UserRegionId);
                
                    dstoreChilds.Reader[0].Fields.First(x => x.Name == "LP_REFREGION").DefaultValue =
                        "'" + Convert.ToString(bebtBookExtension.UserRegionName) + "'";
                }
                else
                {
                    dstoreChilds.Reader[0].Fields.First(x => x.Name == "REFREGION").DefaultValue = String.Empty;
                    dstoreChilds.Reader[0].Fields.First(x => x.Name == "LP_REFREGION").DefaultValue = String.Empty;
                }

                // По умолчанию указываем текущий выбранный вариант
                dstoreChilds.Reader[0].Fields.First(x => x.Name == "REFVARIANT").DefaultValue =
                    Convert.ToString(bebtBookExtension.Variant.Id);

                // По умолчанию указываем текущий выбранный источник
                dstoreChilds.Reader[0].Fields.First(x => x.Name == "SOURCEID").DefaultValue =
                    Convert.ToString(bebtBookExtension.CurrentSourceId);
            }

            // function(store,params)
            if (Entity.ObjectKey == F_S_SchBCreditincome.Key)
            {
                dstoreEntity.Listeners.BeforeSave.AddAfter(
                    @"
try{
    var n = new Date(store.getAt(0).get('CONTRACTDATE'));
    var k = new Date(store.getAt(0).get('DEBTENDDATE'));
    var ny = n.format('Y');
    var ky = k.format('Y');
    var nd = n.getDayOfYear();
    var kd = k.getDayOfYear();
    var periodType = -1;

    if ((ny == ky) || ky - ny == 1 && nd > kd){
	    periodType = 1;
    }else if (((ky - ny) == 1 && (nd <= kd)) || ((ky - ny) >= 2 && (ky - ny) <= 4) || ((ky - ny) == 5 && (nd >= kd))){
	    periodType = 2;
    }else if ((ky - ny) >= 5 && (nd < kd)){
	    periodType = 2;
    } else {
	    periodType = -1;
    }

    store.getAt(0).set('REFSCREDITPERIOD', periodType);
}catch(err){
}
");
            }

            if (changesExists && NeedCreateChilds())
            {
                //--------------------------------------------------------------
                // Действие "Добавить изменения"
                var addChangesButton = new Button
                {
                    ID = "addChangesButton",
                    Text = "Добавить изменения",
                    Icon = Icon.ApplicationFormEdit,
                    Disabled = Readonly,
                    Hidden = true,
                    StyleSpec = "margin-top: 2px; margin-bottom: 2px;",
                    Handler = "function (){ setVisibleChangesGrid(true); }"
                };
                detailsForm.Items.Add(addChangesButton);

                detailsForm.Items.Add(
                    new BebtBookFormChangesGridControl(presentation, Readonly, Fields)
                        .Build(null));
            }
        }

        private bool IsForceUnlock()
        {
            return bebtBookExtension.UserRegionType == UserRegionType.Subject && Params.ContainsKey("ForceUnlock");
        }

        private FieldSet GetContainer(IDataAttribute attribute, Dictionary<string, FieldSet> groups)
        {
            FieldSet container;
            if (groups.ContainsKey(attribute.GroupTags))
            {
                container = groups[attribute.GroupTags];
            }
            else
            {
                // создаём новую группу
                string[] groupNames = attribute.GroupTags.Split(new[] { ';' }, 2);
                container = new FieldSet
                                {
                                    Title = groupNames.GetLength(0) == 2
                                                ? groupNames[1]
                                                : groupNames[0],
                                    FormGroup = true,
                                    AutoHeight = true,
                                    Collapsed = groups.Count > 0,
                                    Layout = "form",
                                    LabelAlign = LabelAlign.Left,
                                    Border = false,
                                    LabelSeparator = " ",
                                    LabelWidth = 200,
                                    Padding = 2,
                                    StyleSpec = "margin-top: 2px; margin-bottom: 4px;"
                                };
                container.Listeners.Expand.Handler = "ToolbarsPanel.doLayout();";
                container.Listeners.Collapse.Handler = "ToolbarsPanel.doLayout();";
                groups.Add(attribute.GroupTags, container);
            }

            return container;
        }

        private Store InitializeStore()
        {
            Store store = new Store
            {
                ID = "dsEntity",
                ShowWarningOnFailure = true,
                AutoLoad = false,
                WarningOnDirty = false
            };

            HttpProxy proxy = new HttpProxy
            {
                Url = "/BebtBookData/GetRecord?objectKey={0}".FormatWith(Entity.ObjectKey),
                Method = HttpMethod.GET
            };
            store.Proxy.Add(proxy);

            if (!Readonly)
            {
                HttpWriteProxy writeProxy = new HttpWriteProxy
                {
                    Url = "/Entity/Save?objectKey={0}".FormatWith(Entity.ObjectKey),
                    Method = HttpMethod.POST
                };
                store.UpdateProxy.Add(writeProxy);
            }

            JsonReader reader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                TotalProperty = "total"
            };
            reader.Fields.Add("ID");
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("rid", "#{idFilter}.getValue()", ParameterMode.Raw));

            store.Listeners.BeforeLoad.Handler = "#{DetailsForm}.el.mask('Загрузка данных...', 'x-mask-loading');";
            store.Listeners.LoadException.Handler = "#{DetailsForm}.el.unmask();alert('LoadException');";
            store.Listeners.Load.Fn = "entityLoaded";
            store.Listeners.BeforeSave.Handler = "#{DetailsForm}.el.mask('Сохранение данных...', 'x-mask-loading');";
            store.Listeners.Save.Handler = @"
if(arg.message){
    if (arg.message.indexOf('newId', 0) == 1){
        var result = eval('(' + arg.message + ')');
        #{idFilter}.setValue(result.newId);
    }
}
ViewPersistence.onSave();
";
            store.Listeners.CommitDone.Handler = "#{DetailsForm}.el.unmask();";
            store.Listeners.CommitFailed.Handler = "#{DetailsForm}.el.unmask();";

            return store;
        }

        private Store InitializeStoreChilds()
        {
            Store store = new Store
            {
                ID = "dsChilds",
                ShowWarningOnFailure = true,
                AutoLoad = false,
                WarningOnDirty = false
            };

            HttpProxy proxy = new HttpProxy
            {
                Url = "/Entity/DataWithServerFilter?objectKey={0}".FormatWith(Entity.ObjectKey),
                Method = HttpMethod.GET
            };
            store.Proxy.Add(proxy);

            if (!Readonly)
            {
                HttpWriteProxy writeProxy = new HttpWriteProxy
                {
                    Url = "/Entity/Save?objectKey={0}".FormatWith(Entity.ObjectKey),
                    Method = HttpMethod.POST
                };
                store.UpdateProxy.Add(writeProxy);
            }

            JsonReader reader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                TotalProperty = "total"
            };
            reader.Fields.Add("ID");
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("limit", "15", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("dir", "ASC", ParameterMode.Value));
            store.BaseParams.Add(new Parameter("sort", "CHARGEDATE", ParameterMode.Value));
            store.BaseParams.Add(new Parameter("serverFilter", "'(PARENTID = ' + #{idFilter}.getValue() + ')'", ParameterMode.Raw));

            store.Listeners.Load.Handler = "setVisibleChangesGrid(#{dsChilds}.getCount() != 0);";

            return store;
        }

        private void CreateFormColumns(Store dstoreEntity, FormPanel detailsForm)
        {
            var columnPanel = new Panel { Border = false, Layout = "column", Anchor = "99% -5" };
            var groupPanel = new Panel { Border = false, Layout = "form", ColumnWidth = 0.5, LabelSeparator = " ", LabelWidth = 230, Padding = 2 };
            int i = 1;
            foreach (IDataAttribute attribute in Entity.Attributes.Values)
            {
                if (attribute.Class == DataAttributeClassTypes.Typed || attribute.Class == DataAttributeClassTypes.Reference)
                {
                    dstoreEntity.Reader[0].Fields.AddRecordField(attribute);

                    ColumnState columnState;
                    if (Fields != null)
                    {
                        Fields.TryGetValue(attribute.Name.ToUpper(), out columnState);
                    }
                    else
                    {
                        columnState = null;
                    }

                    // пропускаем скрытые поля
                    if (columnState.Return(x => x.Hiden, false))
                    {
                        continue;
                    }

                    // Для ссылочных полей создаем скрытое поле для хранения значения
                    if (attribute.Class == DataAttributeClassTypes.Reference)
                    {
                        groupPanel.Items.Add(new TextField
                        {
                            ID = attribute.Name.ToUpper(),
                            FieldLabel = attribute.Name.ToUpper(),
                            Hidden = true
                        });
                    }

                    groupPanel.Items.AddFormField(attribute, columnState)
                        .ReadOnly = Readonly;

                    if (i++ == FieldsPerColumn)
                    {
                        // Создаем вторую колонку свойств формы
                        columnPanel.Items.Add(groupPanel);
                        groupPanel = new Panel
                        {
                            Border = false,
                            Layout = "form",
                            ColumnWidth = 0.5,
                            LabelSeparator = " ",
                            LabelWidth = 230,
                            Padding = 2
                        };
                    }
                }
            }

            columnPanel.Items.Add(groupPanel);
            detailsForm.Items.Add(columnPanel);
        }

        private void RegisterResources(ViewPage page)
        {
            ResourceManager.GetInstance(page).RegisterStyle(
                "ShowDetails", "/Content/css/DebtBook/ShowDetails.css");
            ResourceManager.GetInstance(page).RegisterScript(
                "Format.number.Hack", "/Content/js/Ext.util.Format.number.Hack.js");
            ResourceManager.GetInstance(page).RegisterScript(
                "Extension.View", "/Content/js/Extension.View.js");
            ResourceManager.GetInstance(page).RegisterScript(
                "ViewPersistence", "/Content/js/ViewPersistence.js");

            if (parametersService.GetParameterValue("OKTMO") == OKTMO.Yaroslavl)
            {
                var script = @"
function validateChanges(records) {
    for (var i = 0; i < records.length; i++) {
        if (records[i].get('CHARGEDATE') == '') {
            return false;
        }
    }
    return true;
}";
                ResourceManager.GetInstance(page).RegisterClientScriptBlock("DebtBook.ShowDetails.Validate", script);
            }
            else
            {
                ResourceManager.GetInstance(page).RegisterClientScriptBlock("DebtBook.ShowDetails.Validate", @"function validateChanges(records) { return true; }");
            }
            
            ResourceManager.GetInstance(page).RegisterScript(
                "DebtBook.ShowDetails", "/Content/js/DebtBook.ShowDetails.js");
        }

        private Window CreateBookWindow()
        {
            var window = new Window
            {
                ID = "BookWindow",
                Title = "Параметры отчета",
                Width = 700,
                Height = 500,
                Hidden = true,
                Modal = true,
                Constrain = true,
                Icon = Icon.ApplicationFormEdit
            };

            window.AutoLoad.ShowMask = true;
            window.AutoLoad.ReloadOnEvent = true;
            window.AutoLoad.TriggerEvent = "show";
            window.AutoLoad.Url = "/";
            window.AutoLoad.Mode = LoadMode.IFrame;
            window.AutoLoad.MaskMsg = "Загрузка справочника...";

            window.AutoLoad.Params.Add(new Parameter("id", String.Empty, ParameterMode.Value));
            window.AutoLoad.Params.Add(new Parameter("sourceId", String.Empty, ParameterMode.Value));
            window.AutoLoad.Params.Add(new Parameter("regionId", String.Empty, ParameterMode.Value));
            window.AutoLoad.Params.Add(new Parameter("filter", String.Empty, ParameterMode.Value));

            window.Listeners.Update.Handler =
                "BookWindow.getBody().Extension.entityBook.onRowSelect = Extension.DebtBookShowDetailsView.onBookRowSelect;";

            var btnOk = new Button 
            {
                ID = "btnOk",
                Text = "OK",
                Icon = Icon.Accept,
                Disabled = true
            };
            btnOk.Listeners.Click.Fn = "bookClose";
            window.Buttons.Add(btnOk);

            return window;
        }
    }
}
