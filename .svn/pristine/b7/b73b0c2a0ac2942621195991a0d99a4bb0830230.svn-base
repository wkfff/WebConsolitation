using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.ExtMaxExtensions;
using Krista.FM.RIA.Core.ExtNet.Extensions.BookCellSelectionModel;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Helpers;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;
using GridPanel = Ext.Net.GridPanel;
using Icon = Ext.Net.Icon;
using PagingToolbar = Ext.Net.PagingToolbar;

[assembly: WebResource("Krista.FM.RIA.Core.Extensions.css.CustomSearch.css", "text/javascript")]

namespace Krista.FM.RIA.Core.Gui
{
    public class GridControl : Control
    {
        private ViewPage page;

        public GridControl()
        {
            Params = new Dictionary<string, string>();
            StoreListeners = new Dictionary<string, string>();
            GridListeners = new Dictionary<string, string>();
            PagingSettings = new PagingSettings();

            Lookups = string.Empty;
            LookUpNames = new Dictionary<string, ReferencerInfo>();
            Columns = new List<ColumnBase>();
            RefFieldStyles = string.Empty;
            MetaData = string.Empty;
            Filters = new List<GridFilter>();
            RecordFields = new RecordFieldCollection();
            GridId = Id;

            IsBook = false;
            SourceId = -1;
            ParentId = string.Empty;
            IsDivided = false;
            MasterColumnId = MasterColumnName();
            AutoSaveOnDelete = false;
            ShowDataSourcesBox = true;

            SortDir = "ASC";
            SortField = "ID";
        }

        public string Title { get; set; }
        
        public IPresentation Presentation { get; set; }
        
        public IViewService ViewService { get; set; }
        
        public Dictionary<string, string> Params { get; private set; }
        
        public Dictionary<string, string> StoreListeners { get; private set; }
        
        public Dictionary<string, string> GridListeners { get; private set; }
        
        public RowEditorFormViewDescriptor RowEditorFormView { get; set; }
        
        public bool Readonly { get; set; }
        
        public PagingSettings PagingSettings { get; set; }

        public string ObjectKey { get; set; }

        /// <summary>
        /// Делится ли по источникам
        /// </summary>
        public bool IsDivided { get; set; }

        /// <summary>
        /// True, если отображается в качестве справочника
        /// </summary>
        public bool IsBook { get; set; }

        /// <summary>
        /// Идентификатор источника, если классификатор делится по источникам, иначе - -1
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// Задает видимость поля выбора источника данных.
        /// </summary>
        public bool ShowDataSourcesBox { get; set; }

        public string SortField { get; set; }

        public string SortDir { get; set; }
        
        /************************** Content ************************************/

        public string Lookups { get; set; }

        public List<ColumnBase> Columns { get; set; }

        public string RefFieldStyles { get; set; }

        public string MetaData { get; set; }

        public List<GridFilter> Filters { get; set; }

        public RecordFieldCollection RecordFields { get; set; }

        public string GridId { get; set; }

        /**************************** Hierarchy **********************************/

        /// <summary>
        /// Столбец, по которому производится иерархия
        /// </summary>
        public string MasterColumnId { get; set; }

        /// <summary>
        /// Признак - отображать ли иерархию
        /// </summary>
        public EntityBookViewModel.ShowModeType ShowMode { get; set; }

        /// <summary>
        /// Id родительского компонента
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Серверный фильтр для справочников
        /// </summary>
        protected string BookFilter { get; set; }
        
        protected Dictionary<string, ReferencerInfo> LookUpNames { get; set; }
        
        /// <summary>
        /// Сохранение изменений при удалении записи
        /// </summary>
        protected bool AutoSaveOnDelete { get; set; }

        private bool IsHierarchyGrid
        {
            get { return ParentId.IsNotNullOrEmpty(); }
        }

        public override List<Component> Build(ViewPage page)
        {
            if (Id.IsNullOrEmpty())
            {
                Id = "GridControl";
            }

            this.page = page;

            // скрипты для работы в качестве справочника
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript(
               "Extension.View", "/Content/js/Extension.View.js");
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript(
                "Entity.View", "/Content/js/Entity.Show.js");
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript(
                "Entity.Boook", "/Krista.FM.RIA.Extensions.Entity/Presentation/js/Entity.Book.js/extention.axd");
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript(
                "Entity.Source", "/Krista.FM.RIA.Extensions.Entity/Presentation/js/EntitySource.js/extention.axd");
            Ext.Net.ResourceManager.GetInstance(page).RegisterStyle(
                "CustomSearch.Style", "/Krista.FM.RIA.Core/Extensions/css/CustomSearch.css/extention.axd");
            Ext.Net.ResourceManager.GetInstance(page).RegisterScript(
                "Combobox.HotKeys", "/Krista.FM.RIA.Core/ExtNet.Extensions/ExcelLikeSelectionModel/js/HotKeysForCombobox.js/extention.axd");

            var ds = BuildStore();
            page.Controls.Add(ds);
            var grid = BuildGrid(ds);
            return new List<Component> { grid };
        }

        /// <summary>
        /// Обработчик выбора ячейки в справочнике
        /// </summary>
        /// <returns> Скрипт в виде строки</returns>
        protected virtual string GetBeforeCellSelectHandler(GridPanel grid)
        {
            return
                    @"
function (selModel, rowIndex, colIndex) {{
    // var r = #{{{0}}}.getSelectionModel().selection.record;
    var r = #{{{0}}}.getStore().getAt(rowIndex);
    Extension.entityBook.selectedRecord = r;
    Extension.entityBook.onRowSelect(r);
}}".FormatWith(grid.ID);
        }

        protected virtual string GetAddButtonHandler(string gridPanelId)
        {
            return @"
function(){{
        if ({0}.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
            r = {0}.getSelectionModel().getSelected();
        }}
        else {{
            if ({0}.getSelectionModel().selection == null)
                r = null;
            else
                r = {0}.getSelectionModel().selection.record;
        }}
        /*получаем список полей f*/
        var f = {0}.store.recordType.prototype.fields,
            dv = {{}};
        /*инициализируем поля значениями по умолчанию*/
        for (var i = 0; i < f.length; i++) {{
            if (f.items[i].defaultValue == null)
                dv[f.items[i].name] = '';
            else
                dv[f.items[i].name] = f.items[i].defaultValue;
        }}

        var comboS = Ext.getCmp('{1}');
        if (comboS != null) {{
            dv['SOURCEID'] = {1}.value;
        }}
        /*присваиваем временный идентификатор*/
        dv['ID'] = this.tempId;
    {0}.insertRecord(0, dv);{0}.getView().focusRow(0);{0}.startEditing(0, 0);
}}".FormatWith(gridPanelId, "comboSource" + Id);
        }

        protected virtual void CreateColumnModel(GridPanel grid)
        {
            foreach (var column in Columns)
            {
                grid.ColumnModel.Columns.Add(column);

                ReferencerInfo refInfo;

                if (LookUpNames.TryGetValue(column.ColumnID, out refInfo))
                {
                    string url = "/Entity/DataWithCustomSearch?objectKey={0}"
                        .FormatWith(refInfo.GetObjectKey());

                    column.AddLookupForColumn(
                        refInfo.GetPrimaryFields(),
                        refInfo.GetSecondaryFields(),
                        url,
                        page);
                }
            }

            if (Columns.Count == 1)
            {
                grid.AutoExpandColumn = Columns[0].ColumnID;
            }
            else
            {
                grid.AutoExpandColumn = MasterColumnId;
            }

            grid.AutoExpandMin = 250;
        }

        /// <summary>
        /// Определяет атрибут для иерархии
        /// </summary>
        /// <returns> Имя столбца, по которому производится иерархия</returns>        
        protected string MasterColumnName()
        {
            return Columns.Count > 0 ? Columns.First().DataIndex : string.Empty;
        }

        protected virtual Store BuildStore(string objectKey, string storeId)
        {
            var dataStore = IsHierarchyGrid ? new AdjacencyListStore(page) : new Store();
            dataStore.ID = storeId;
            dataStore.AutoLoad = !IsDivided;
            dataStore.RemoteSort = true;
            dataStore.RefreshAfterSaving = RefreshAfterSavingMode.Always;
            dataStore.DirtyWarningTitle = "Несохраненные изменения";
            dataStore.DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?";
            var proxyUrl = IsHierarchyGrid
                                  ? "/Entity/DataH?objectKey={0}".FormatWith(objectKey)
                                  : "/Entity/DataWithServerFilter?objectKey={0}".FormatWith(objectKey);
            dataStore.Restful = false;

            dataStore.Proxy.Add(new HttpProxy
            {
                Url = proxyUrl,
                Method = HttpMethod.POST
            });

            dataStore.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/Entity/Save?objectKey={0}"
                    .FormatWith(objectKey),
                Method = HttpMethod.POST
            });

            if (IsHierarchyGrid)
            {
                ((AdjacencyListStore)dataStore).ParentIdFieldName = ParentId;
            }

            var reader = new JsonReader { IDProperty = "ID", TotalProperty = "total", Root = "data" };
            foreach (RecordField recordField in RecordFields)
            {
                reader.Fields.Add(recordField);
            }

            dataStore.Reader.Add(reader);

            if (PagingSettings.Size < 1)
            {
                PagingSettings.Size = 10;
            }

            if (PagingSettings.Start < 0)
            {
                PagingSettings.Start = 0;
            }

            dataStore.BaseParams.Add(new Parameter("limit", Convert.ToString(PagingSettings.Size), ParameterMode.Raw));
            dataStore.BaseParams.Add(new Parameter("start", Convert.ToString(PagingSettings.Start), ParameterMode.Raw));
            dataStore.BaseParams.Add(new Parameter("dir", SortDir));
            dataStore.BaseParams.Add(new Parameter("sort", SortField));
            if (IsDivided)
            {
                dataStore.AutoLoadParams.Add(new Parameter("source", SourceId.ToString()));
            }

            if (ViewService != null)
            {
                var serverFilter = ViewService.GetDataFilter();
                if (serverFilter.IsNotNullOrEmpty())
                {
                    dataStore.BaseParams.Add(new Parameter(
                                                 "serverFilter",
                                                 "'{0}'".FormatWith(serverFilter),
                                                 ParameterMode.Raw));
                    dataStore.AutoLoadParams.Add(new Parameter(
                        "serverFilter",
                        " {0}".FormatWith(serverFilter)));
                }
            }

            dataStore.DirectEventConfig.Method = HttpMethod.POST;
            dataStore.DirectEventConfig.CleanRequest = true;

            dataStore.SortInfo.Direction = SortDir == "ASC" ? SortDirection.ASC : SortDirection.DESC;
            dataStore.SortInfo.Field = SortField;

            // Устанавливаем обработчики событий
            foreach (KeyValuePair<string, string> storeListener in StoreListeners)
            {
                dataStore.Listeners.AddListerer(storeListener.Key, storeListener.Value);
            }

            return dataStore;
        }

        /// <summary>
        /// Создает на тулбаре грида кнопки действий.
        /// </summary>
        /// <param name="gp">Родительский грид.</param>
        /// <param name="actions">Список действий.</param>
        private static void CreateActions(GridPanel gp, IEnumerable<ViewModel.ActionDescriptor> actions)
        {
            if (gp.TopBar.Count <= 0)
            {
                return;
            }

            var toolbar = gp.TopBar[0];

            foreach (ViewModel.ActionDescriptor action in actions)
            {
                // Создаем кнопки для отчетов
                if (action is ReportDescriptor)
                {
                    toolbar.Items.Add(new Button
                                          {
                                              Icon = Icon.Report,
                                              Handler = action.Handler,
                                              ToolTip = action.Title
                                          });
                }
            }
        }

        private static Window GetBookWindow(string gridId)
        {
            var win = new Window
            {
                ID = "{0}BookWindow".FormatWith(gridId),
                Width = 800,
                Height = 600,
                Icon = Icon.ApplicationFormEdit,
                Hidden = false,
                Modal = true,
                Constrain = true
            };
            win.AutoLoad.Url = "/";
            win.AutoLoad.Mode = LoadMode.IFrame;
            win.AutoLoad.TriggerEvent = "show";
            win.AutoLoad.ReloadOnEvent = true;
            win.AutoLoad.ShowMask = true;
            win.AutoLoad.MaskMsg = "Загрузка справочника...";
            win.AutoLoad.Params.Add(new Parameter("id", String.Empty, ParameterMode.Value));

            var button = new Button
            {
                ID = "btnOk",
                Text = "OK",
                Icon = Icon.Accept,
                Disabled = true
            };

            string bookValueSelectedHandler = @"    
if ({0}.selectedBookRecord) {{
    var grid = Ext.getCmp('{1}');
    var r = record;
        
    r.beginEdit();
    r.set({0}.activeRefField, {0}.selectedBookRecord.id);
    var lookupValue = {0}.getBody().Extension.entityBook.getLookupValue();
    if (lookupValue == '')
        lookupValue = {0}.selectedBookRecord.id;
    r.set({0}.activeLookupField, lookupValue);
    r.endEdit();
    {0}.selectedBookRecord = undefined; 
    btnOk.disable();
    {0}.hide();
    {1}.getView().focusCell(1,1);
}}
".FormatWith(win.ID, gridId);

            win.Listeners.Update.Handler =
                @"{0}.getBody().Extension.entityBook.onRowSelect = function(record){{btnOk.enable(); {0}.selectedBookRecord = record;}}
                  {0}.getBody().Extension.entityBook.bookEnterHandler =  function() {{
                    {1}
                  }}
                  {0}.getBody().Extension.entityBook.bookEscHandler =  function() {{
                    //Ext.getCmp('{2}').view.focusCell(r,c);
                    {0}.selectedBookRecord = undefined; 
                    btnOk.disable();
                    {0}.hide();
                    {2}.select(1,1);
                  }}
                  ".FormatWith(win.ID, bookValueSelectedHandler, gridId);

            button.Listeners.Click.Handler = bookValueSelectedHandler;

            win.Buttons.Add(button);

            return win;
        }

        private Store BuildStore()
        {
            return BuildStore(ObjectKey, "{0}Store".FormatWith(Id));
        }

        private void CreateFilters(GridPanel grid)
        {
            var filters = new GridFilters { FiltersText = "Фильтры" };
            foreach (var filter in Filters)
            {
                filters.Filters.Add(filter);
            }

            grid.Plugins.Add(filters);
        }

        private void CreateLookups()
        {
            if (Lookups.IsNotNullOrEmpty())
            {
                Ext.Net.ResourceManager.GetInstance(HttpContext.Current).RegisterOnReadyScript(Lookups);
            }
        }

        private GridPanel BuildGrid(Store ds)
        {
            var grid = IsHierarchyGrid ? new EditorGridPanel(page) : new GridPanel();
            grid.ID = Id;
            grid.Title = Title;
            grid.StripeRows = true;
            grid.TrackMouseOver = true;
            grid.AutoScroll = true;
            grid.MonitorResize = true;
            grid.Border = false;
            grid.Header = false;
            grid.Visible = true;
            grid.Stateful = true;
            grid.Layout = "fit";
            grid.StoreID = ds.ID;
            grid.Height = 700;
            grid.ClicksToEdit = 2;
            grid.LoadMask.ShowMask = true;
            grid.SaveMask.ShowMask = true;
            grid.FireSelectOnLoad = true;
            
            if (IsHierarchyGrid)
            {
                ((EditorGridPanel)grid).MasterColumnId = MasterColumnId;
            }

            grid.TopBar.Add(CreateToolbar(ds, grid.ID, grid.Title));

            // Устанавливаем обработчики событий
            SetListeners(grid);

            if (RowEditorFormView != null)
            {
                InitializeRowEditorFormAbility(page, grid);
            }

            CreateLookups();

            // Создаем колонки
            CreateColumnModel(grid);
            
            CreateRefFieldsStylesAndMetadata(page);

            CreateFilters(grid);

            CellSelectionModel sm;

            if (IsBook)
            {
                sm = new BookCellSelectionModel { ID = "BookCellSM_{0}".FormatWith(Id) };

                // sm.Listeners.BeforeRowSelect.Fn = GetBeforeRowSelectHandler();
                sm.Listeners.BeforeCellSelect.Fn = GetBeforeCellSelectHandler(grid);
            }
            else
            {
                sm = new ExcelLikeSelectionModel();
            }

            grid.SelectionModel.Add(sm);

            if (ViewService != null)
            {
                CreateActions(grid, ViewService.Actions);
            }

            return grid;
        }

        private PagingToolbar CreateToolbar(Store dataStore, string gridPanelId, string gridPanelTitle)
        {
            var tb = IsHierarchyGrid
                         ? new ExtMaxExtensions.PagingToolbar()
                         : new PagingToolbar();

            tb.ID = "pagingToolBar{0}".FormatWith(gridPanelId);
            tb.StoreID = dataStore.ID;
            tb.DisplayInfo = true;
            tb.PageSize = PagingSettings.Size;
            tb.PageIndex = PagingSettings.Start;
            tb.BorderWidth = 0;
            tb.DisplayMsg = "Записи с {0} по {1} из {2}";
            tb.EmptyMsg = "Нет данных";
            tb.PageSize = PagingSettings.Size;
            tb.PageIndex = PagingSettings.Start;
            tb.HideRefresh = IsHierarchyGrid;

            if (IsHierarchyGrid)
            {
                tb.Items.Add(new Button
                                 {
                                     ID = "{0}RefreshButton".FormatWith(gridPanelId),
                                     Icon = Icon.PageRefresh,
                                     Handler = @"function(){{{0}.rejectHandler();}}".FormatWith(gridPanelId),
                                     ToolTip = "Обновить",
                                     Disabled = Readonly
                                 });
            }

            if (!Readonly)
            {
                string addHandler;
                if (RowEditorFormView != null)
                {
                    string url = GetEditorFormUrl("-1");
                    addHandler =
                        @"
function(){{
    DetailsWindow.clearContent();
    DetailsWindow.autoLoad.url = '{0}';
    DetailsWindow.setTitle('{1}');
    DetailsWindow.show();
}}"
                            .FormatWith(url, gridPanelTitle);
                }
                else
                {
                    addHandler = GetAddButtonHandler(gridPanelId);
                }

                string addNewHandler;
                string deleteHandler;
                string saveHandler;
                if (IsHierarchyGrid)
                {
                    var saveChanges = AutoSaveOnDelete
                        ? "{0}.saveHandler('{1}');".FormatWith(gridPanelId, ObjectKey)
                        : string.Empty;
                    addNewHandler = @"function(){{{0}.addHandler();}}".FormatWith(gridPanelId);
                    deleteHandler = @"    
    function(){{
        {0}.deleteHandler();
        {1}
    }}".FormatWith(gridPanelId, saveChanges);
                    saveHandler = @"function(){{{0}.saveHandler('{1}');}}".FormatWith(gridPanelId, ObjectKey);
                }
                else
                {
                    addNewHandler = addHandler;
                    var saveChanges = AutoSaveOnDelete ? "{0}.save();".FormatWith(gridPanelId) : string.Empty;
                    deleteHandler = @"function(){{
    Ext.Msg.confirm('Предупреждение','Удалить выделенные записи?', function (btn) {{
        if(btn == 'yes') {{
            var sel;
            if (#{{{0}}}.getSelectionModel() instanceof Ext.grid.RowSelectionModel) {{
                sel = #{{{0}}}.getSelectionModel().getSelected();
            }}   
            else {{
                sel = #{{{0}}}.getSelectionModel().selection.record;
            }}
            #{{{1}}}.remove(sel);
            {2}
        }}
    }});    
}}"
.FormatWith(gridPanelId, dataStore.ID, saveChanges);

                    /*обработчик при сохранении изменений*/
                    saveHandler = @"
function(){{
    errorFields = '';
    errorFieldsCnt = 0;
    /*формируем список добавленных записей*/
    Ext.each({0}.data.items, function (record) {{
            if (record.phantom || record.dirty) {{
                recData = record.data;
                Ext.each({1}.colModel.columns, function (column) {{
                    if ((column.allowBlank == false) && ((recData[column.dataIndex] == null) || (recData[column.dataIndex] === ''))) {{
                        errorFields += '{2}' + column.header + '{2}, ';
                        errorFieldsCnt++;
                    }}
                }});
            }}
    }});

    if (errorFieldsCnt > 1) {{
            Ext.MessageBox.alert('Предупреждение', 'Поля ' + errorFields.substring(0, errorFields.length - 2) + ' должны иметь значение', 1);
    }}
    else if (errorFieldsCnt == 1) {{
            Ext.MessageBox.alert('Предупреждение', 'Поле ' + errorFields.substring(0, errorFields.length - 2) + ' должно иметь значение', 1);
    }}
    if (errorFieldsCnt < 1) {{
        {0}.save();
    }}
}}".FormatWith(dataStore.ID, gridPanelId, '"');
                }
                
                tb.Items.Add(new Button
                                 {
                                     ID = "{0}SaveButton".FormatWith(gridPanelId),
                                     Icon = Icon.Disk,
                                     Handler = saveHandler,
                                     ToolTip = "Сохранить данные",
                                     Disabled = Readonly
                                 });

                tb.Items.Add(new Button
                                 {
                                     ID = "{0}AddNewRowButton".FormatWith(gridPanelId),
                                     Icon = Icon.Add,
                                     Handler = addNewHandler,
                                     ToolTip = "Добавить новую запись",
                                     Disabled = Readonly
                                 });

                tb.Items.Add(new Button
                                 {
                                     ID = "{0}DeleteRowButton".FormatWith(gridPanelId),
                                     Icon = Icon.Delete,
                                     Disabled = Readonly,
                                     Handler = deleteHandler,
                                     ToolTip = "Удалить выделенные записи"
                                 });
            }

            if (IsDivided && ShowDataSourcesBox)
            {
                CreateSourceBox(tb, gridPanelId);
            }

            return tb;
        }

        private string GetEditorFormUrl(string recordIdValue)
        {
            var url = RowEditorFormView.Url.IsNullOrEmpty()
                          ? "/View/{0}".FormatWith(RowEditorFormView.Id)
                          : RowEditorFormView.Url;

            var sb = new StringBuilder();
            if (RowEditorFormView.Params.Count > 0)
            {
                sb.Append('&');
            }

            foreach (var param in RowEditorFormView.Params)
            {
                sb.Append(param.Name).Append('=');
                if (param.Mode == RowEditorFormViewParameterMode.Raw)
                {
                    sb.Append("' + ").Append(param.Value).Append(" + '");
                }
                else
                {
                    sb.Append(param.Value);
                }

                sb.Append('&');
            }

            sb.RemoveLastChar();

            url = "{0}?recordId={1}{2}".FormatWith(url, recordIdValue, sb.ToString());
            return url;
        }

        /// <summary>
        /// Создаем стили и метаданные для ссылочных колонок.
        /// </summary>
        private void CreateRefFieldsStylesAndMetadata(ViewPage page)
        {
            Ext.Net.ResourceManager.GetInstance(page)
                .RegisterOnReadyScript(MetaData);
            
            Ext.Net.ResourceManager.GetInstance(page)
                .RegisterClientStyleBlock("RefColumnStyles", RefFieldStyles);
        }

        private void SetListeners(GridPanel grid)
        {
            foreach (KeyValuePair<string, string> gridListener in GridListeners)
            {
                grid.Listeners.AddListerer(gridListener.Key, gridListener.Value);
            }

            grid.Listeners.Show.AddBefore(@"if(!{0}.viewReady){{ {0}.reload(); }}".FormatWith(grid.ID));

            if (!Readonly)
            {
                var bookWindow = GetBookWindow(Id);

                // Для Unit test контрол должен быть на странице иначе словим ошибку
                var dummyPage = new ViewPage();
                dummyPage.Controls.Add(new Ext.Net.ResourceManager());
                dummyPage.Controls.Add(bookWindow);

                // для 'должности для отчетов'
                var setBookFilter = BookFilter.IsNotNullOrEmpty() &&
                                    (ObjectKey == "4d192956-aced-4718-a87c-b2e5519c022a")
                                        ? "{0}.autoLoad.params.filter = '{1}';".FormatWith(bookWindow.ID, BookFilter)
                                        : string.Empty;
                var commandAfter =
                    @"
if (command == 'EditRefCell') {{
    var fieldName = this.getColumnModel().getDataIndex(colIndex);
    {0}
    var refFieldName = fieldName.substring(3, fieldName.length);
    {1}.activeRefField = refFieldName;
    {1}.activeLookupField = fieldName;
    {1}.autoLoad.params.id = record.id;
    {3}
    {1}.autoLoad.url = '/Entity/Book?objectKey=' + MetaData.{2}[refFieldName].objectKey;
    {1}.setTitle(MetaData.{2}[refFieldName].caption);
    btnOk.disable();
    {1}.show();
}}
".FormatWith(bookWindow.ToScript(), bookWindow.ID, grid.ID, setBookFilter);

                grid.Listeners.Command.AddAfter(commandAfter);
            }
        }

        private void InitializeRowEditorFormAbility(ViewPage page, GridPanel grid)
        {
            // Создаем колонку команд
            var cmdColumn = new CommandColumn { Width = 25, Hideable = false };
            var command = new GridCommand { CommandName = "edit", Icon = Icon.ApplicationFormEdit };
            command.ToolTip.Text = "Редактировать запись";
            cmdColumn.Commands.Add(command);
            grid.ColumnModel.Columns.Add(cmdColumn);

            // Устанавливаем обработчик события
            var url = GetEditorFormUrl("' + record.id + '");
            grid.Listeners.Command.AddAfter(@"
if (command == 'edit'){{
    DetailsWindow.clearContent();
    DetailsWindow.autoLoad.url = '{0}';
    DetailsWindow.setTitle(this.title);
    DetailsWindow.show();
}}".FormatWith(url));

            // Создаем окно для отображения формы
            var editFormWindowPresent = page.Controls.Cast<object>()
                .Any(control => ((System.Web.UI.Control)control).ID == "DetailsWindow");
            if (!editFormWindowPresent)
            {
                Window window = new GridViewEditFormWindowBuilder().Create(page);
                page.Controls.Add(window);
            }
        }

        /// <summary>
        /// Creates combobox to select source
        /// </summary>
        /// <param name="tb"> toolbar object </param>
        /// <param name="gridId">grid identifier</param>
        private void CreateSourceBox(PagingToolbar tb, string gridId)
        {
            var dataStore = new Store
                                {
                ID = "sourceStore" + Id,
                AutoLoad = false,
                AutoSave = false
            };
            dataStore.Proxy.Add(new HttpProxy
            {
                Method = HttpMethod.POST,
                Url = "/Entity/Sources"
            });
            var reader = new JsonReader { IDProperty = "ID", TotalProperty = "total", Root = "data", MessageProperty = "extraParams" };
            reader.Fields.Add(new RecordField("SOURCEID"));
            reader.Fields.Add(new RecordField("SOURCENAME"));
            dataStore.Reader.Add(reader);

            dataStore.Listeners.AddListerer("BeforeLoad", " store.baseParams = { objectkey: '" + ObjectKey + "' }; ");

            var sourceIdLabel = new Label
                               {
                ID = "sourceID" + Id,
                Text = "(ID Источника = )"
            };

            sourceIdLabel.Style["marginRight"] = "3px";
            sourceIdLabel.Style["marginLeft"] = "5px";
            sourceIdLabel.Text = "(ID Источника = {0})".FormatWith(SourceId); 
            
            tb.Add(sourceIdLabel);

            var comboSource = new ComboBox
                                  {
                ID = "comboSource" + Id,
                DisplayField = "SOURCENAME",
                ValueField = "SOURCEID",
                TypeAhead = true,
                StoreID = dataStore.ID,
                TriggerAction = TriggerAction.All,
                EmptyText = "Иточник...",
                Width = 200,
                ListWidth = 400,
                SelectOnFocus = true
            };
            if (SourceId != -1)
            {
                comboSource.SelectedItem.Value = SourceId.ToString();
            }

            comboSource.ToolTip = comboSource.SelectedItem.Text;
            tb.Add(comboSource);

            var applyServerFilter = string.Empty;
            if (ViewService != null && ViewService.GetDataFilter().IsNotNullOrEmpty())
            {
                applyServerFilter = "Ext.apply({0}.store.baseParams, {{ serverFilter: '{1}' }});"
                    .FormatWith(gridId, ViewService.GetDataFilter());
            }

            var sourceHandler = @"    if ({0}.selectedIndex == -1) {{ 
        if ({13} == -1) {{ 
                {1}.value = {2}.store.reader.jsonData.extraParams; 
            }} else {{ 
                {3}.value = {13}; 
            }} 
        }}
        {4}.setText('(ID Источника = ' + {5}.value + ')'); 
        {6}.show(); 
        {7}.initValue(); 
        {8}.getSelectionModel().clearSelections(false); 
        Ext.apply({9}.store.baseParams, {{ source: {10}.value }});
        {14}
        {11}.store.load(); 
         ".FormatWith(
                comboSource.ID, 
                comboSource.ID, 
                comboSource.ID, 
                comboSource.ID, 
                sourceIdLabel.ID,
                comboSource.ID,
                sourceIdLabel.ID, 
                comboSource.ID, 
                gridId, 
                gridId,
                comboSource.ID, 
                gridId, 
                gridId,
                SourceId,
                applyServerFilter);
            comboSource.Listeners.AddListerer("Select", sourceHandler + "{0}.getView().refresh();".FormatWith(gridId));
            var load =
                @" if ((this.hasloaded == null) || (this.hasloaded = false))
                {{ 
                    {0} 
                    this.hasloaded = true; 
                }}"
                    .FormatWith(sourceHandler);

             dataStore.Listeners.AddListerer("Load", load);

            dataStore.AutoLoad = true;
             page.Controls.Add(dataStore);
        }
    }
}
