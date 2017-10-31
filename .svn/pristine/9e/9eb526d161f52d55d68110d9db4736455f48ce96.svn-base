using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controls
{
    public class IndicatorsGridControl : GridControl
    {
        /// <summary>
        /// Функция: как отображать поле с кнопкой детализации
        /// </summary>
        protected const string RendererDetailFn =
            "function (v, p, r, rowIndex, colIndex, ds) { if (r.data.HasDetail) { var s = \"<img title='Добавить показатель' src='/icons/add-png/ext.axd' width='16' height='16' class='controlBtn'>\"; return s; } else if (r.data.DetailMark > 0) { var s = \"<img title='Удалить показатель' src='/icons/delete-png/ext.axd' width='16' height='16' class='controlDelBtn'>\"; return s; } else return '';}";

        private bool editable;

        private int requestYear;

        public IndicatorsGridControl(int applicationId, int requestYear)
        {
            GridId = "IndicatorsGrid";
            ApplicationId = applicationId;
            this.requestYear = requestYear;
            editable = true;
            AddToolTipAutoCalc = true;
            ShowAddRemoveDetailsButtons = true;
            ShowOKEI = false;
            ReadUrl = "/FO41Indicators/Read?applicationId={0}".FormatWith(ApplicationId);
            UpdateUrl = "/FO41Indicators/Create";
            UpdateUrl = "/FO41Indicators/Save?applicationId={0}".FormatWith(ApplicationId);
            RendererFnPart =
                @"function (v, p, r) {{
                    if (r.data.IsFormula || {0}) 
                        p.css = 'gray-cell';
                    var okei = r.data.OKEI;
                    var f;
                    if  (okei == 383 || okei == 384)
                        f = Ext.util.Format.numberRenderer('0.000,0/i');
                    else if (okei == 792)
                        f = Ext.util.Format.numberRenderer('0.000,/i');
                    else
                        f = Ext.util.Format.numberRenderer('0.000,/i');
                    return f(v);
                }}";

            RendererFn = RendererFnPart.FormatWith(editable ? "false" : "true");

            BeforeEditHandler = @"
                return !(e.field == 'RefName' && 
                    (e.record.get('DetailMark') == null || e.record.get('DetailMark') == 0));";
        }

        /// <summary>
        /// Идентификатор заявки
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Идентификатор Store для показателей
        /// </summary>
        public string IndicatorsStoreId { get; set; }

        /// <summary>
        /// Признак копии заявки
        /// </summary>
        public bool IsCopy { get; set; }

        /// <summary>
        /// Отображать ли единицы измерения
        /// </summary>
        public bool ShowOKEI { get; set; }

        /// <summary>
        /// Url получения данных
        /// </summary>
        public string ReadUrl { get; set; }

        /// <summary>
        /// Url для записи данных
        /// </summary>
        public string UpdateUrl { get; set; }

        /// <summary>
        /// Признак для отображения значений по показателю в итоговой таблице в разрезе заявок от налогоплательщиков
        /// </summary>
        public bool ShowDetailsMark { get; set; }

        /// <summary>
        /// Признак для отображения поля с кнопками добавления/удаления детализирующих комментариев 
        /// (если Editable = false колонка не отображается в любом случае)
        /// </summary>
        public bool ShowAddRemoveDetailsButtons { get; set; }

        /// <summary>
        /// Признак, можно ли редактировать
        /// </summary>
        public bool Editable
        {
            get
            {
                return editable;
            }

            set
            {
                editable = value;
                RendererFn = RendererFnPart.FormatWith(editable ? "false" : "true");
            }
        }

        protected bool AddToolTipAutoCalc { get; set; }

        /// <summary>
        /// Функция: как отображать поле с показателем
        /// </summary>
        protected string RendererFn { get; set; }

        protected HeaderGroupColumn GroupValueColumns { get; set; }

        protected string BeforeEditHandler { get; set; }
        
        protected string RendererFnPart { get; set; }

        protected string AfterLoadListener { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            RendererFn = RendererFnPart.FormatWith(editable ? "false" : "true");

            ResourceManager.GetInstance(page).RegisterClientStyleBlock(
                "CustomStyle",
                ".gray-cell{background-color: #DCDCDC !important; border-right-color: #FFFFFF; !important;}");
            ResourceManager.GetInstance(page)
                .RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");

            page.Controls.Add(CreateStore());

            var gp = new GridPanel
                         {
                             ID = GridId,
                             LoadMask = { ShowMask = true },
                             SaveMask = { ShowMask = true },
                             StoreID = IndicatorsStoreId,
                             EnableColumnMove = false,
                             ColumnLines = true,
                             AutoExpandColumn = "RefName",
                             SelectionModel = { new ExcelLikeSelectionModel() }
                         };
            if (editable && AddToolTipAutoCalc)
            {
                gp.ToolTips.Add(new ToolTip
                                    {
                                        TargetControl = gp,
                                        Delegate = ".x-grid3-row",
                                        TrackMouse = true,
                                        Listeners =
                                            {
                                                Show =
                                                    {
                                                        Handler = @"
                        var rowIndex = #{{{0}}}.view.findRowIndex(this.triggerElement);
                        if (#{{{0}}}.store.getAt(rowIndex).data.IsFormula)
                            this.body.dom.innerHTML = 'Рассчетные показатели вычисляются при сохранении заявки';
                        else 
                            this.body.dom.innerHTML = 'Показатель не является рассчетным';"
                                                            .FormatWith(gp.ID)
                                                    }
                                            }
                                    });
            }

            var groupRow = new HeaderGroupRow();
            var firstGroupColumnsCnt = editable ? 3 : 2;
            if (ShowDetailsMark)
            {
                firstGroupColumnsCnt++;
            }

            if (ShowOKEI)
            {
                firstGroupColumnsCnt++;
            }

            if (editable && !ShowAddRemoveDetailsButtons)
            {
                firstGroupColumnsCnt--;
            }

            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = firstGroupColumnsCnt });

            GroupValueColumns = new HeaderGroupColumn { Header = "Периоды", ColSpan = 4, Align = Alignment.Center };
            groupRow.Columns.Add(GroupValueColumns);
            gp.View.Add(new GridView { HeaderGroupRows = { groupRow } });

            AddColumn(gp, "RefNumberString", "№ п/п").SetWidth(30);

            if (editable && ShowAddRemoveDetailsButtons)
            {
                AddColumn(gp, "HasDetail", string.Empty)
                    .SetWidth(30)
                    .Renderer.Fn = RendererDetailFn;
            }

            var command = new GridCommand { Icon = Icon.ApplicationViewDetail, CommandName = "AddDetailIndicator" };
            var commandColumn = new CommandColumn { Header = string.Empty, ButtonAlign = Alignment.Center };
            commandColumn.Commands.Add(command);

            var columnName = AddColumn(gp, "RefName", "Наименование показателя").SetWidth(200);
            columnName.Wrap = true;
        
            if (ShowDetailsMark)
            {
                var commandDetails = new GridCommand { Icon = Icon.Information, CommandName = "ViewMarkDetails" };
                commandDetails.ToolTip.Text = "Показать детализацию по данному показателю";
                var column = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
                column.Commands.Add(commandDetails);
                gp.ColumnModel.Columns.Add(column);

                // на открытие детализации показателя в разрезе налогоплательщиков
                AddDetailMarkHandler(gp);
            }

            if (editable)
            {
                columnName.SetEditableString();
            }

            if (ShowOKEI)
            {
                gp.ColumnModel.AddColumn("OKEIName", "Единица измерения", DataAttributeTypes.dtString).Align = Alignment.Center;
            }

            CreateValueColumns(gp);

            gp.AddColumnsWrapStylesToPage(page);
            gp.AddColumnsHeaderAlignStylesToPage(page);
            
            gp.Listeners.CellMouseDown.Handler = @"
                var fName = this.getColumnModel().getDataIndex(columnIndex);
                if (fName == 'HasDetail') {
                    var t = e.getTarget();
                    if (t.className == 'controlBtn') {
                        var myNewRecord = new IndicatorsGrid.store.recordType();
                        myNewRecord.newRecord = true;
                        myNewRecord.data.phantom = true;
                        myNewRecord.data.TempID = tempIndId;
                        myNewRecord.data.DetailMark = IndicatorsGrid.store.getAt(rowIndex, columnIndex).data.RefMarks;
                        myNewRecord.data.OKEI = IndicatorsGrid.store.getAt(rowIndex, columnIndex).data.OKEI;
                        tempIndId--;
                        /*добавляем новую запись к выбранному узлу*/
                        IndicatorsGrid.stopEditing();
                        var rowToInsert = rowIndex+1;
                        while(IndicatorsGrid.store.data.length > rowToInsert && IndicatorsGrid.store.getAt(rowToInsert).data.DetailMark > 0)
                            rowToInsert++;
                        IndicatorsGrid.store.insert(rowToInsert, myNewRecord); // insert a new record into the store
                    }
                    else
                    if (t.className == 'controlDelBtn') {
                        var recToRmv = IndicatorsGrid.store.getAt(rowIndex, columnIndex);
                        IndicatorsGrid.store.removed.push(recToRmv);
                        IndicatorsGrid.store.remove(recToRmv);    
                    }
                    else if (t.className == 'controlDetailsBtn') {
                            
                        }
                }";

            gp.Listeners.BeforeEdit.AddAfter(BeforeEditHandler);

           return new List<Component> { gp };
        }

        /// <summary>
        /// Создание модального окна для формы с детализацией показателя
        /// </summary>
        /// <returns>Модальное окно</returns>
        protected static Window GetDetailsWindow()
        {
            var win = new Window
            {
                ID = "detailsWindow",
                Width = 600,
                Height = 340,
                Title = @"Детализация показателя",
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
            win.AutoLoad.MaskMsg = @"Открытие формы с детализацией показателя...";
            win.AutoLoad.Params.Add(new Parameter("id", String.Empty, ParameterMode.Value));

            win.Listeners.BeforeShow.Handler =
                @"
                var size = Ext.getBody().getSize(); 
                this.setSize({ width: size.width * 0.8, height: size.height * 0.8});";

            var buttonSave = new Button
            {
                ID = "btnOk",
                Text = @"ОК",
                Icon = Icon.Accept,
                Listeners =
                {
                    Click =
                    {
                        Handler = @"{0}.hide();".FormatWith(win.ID)
                    }
                }
            };

            win.Buttons.Add(buttonSave);

            return win;
        }
        
        protected virtual void AddDetailMarkHandler(GridPanel gp)
        {
            var bookWindowEdit = GetDetailsWindow(); 
            gp.Listeners.Command.Handler = @"
            if (command == 'ViewMarkDetails') {{
                if (record != null) {{
                    {0}
                    {1}.autoLoad.url = '/FO41Indicators/ViewDetails?markId=' + record.data.RefMarks + 
                        '&applicOGVId={3}';
                    {1}.show();
                }}
            }}".FormatWith(bookWindowEdit.ToScript(), bookWindowEdit.ID, gp.ID, ApplicationId);
        }

        protected virtual void CreateValueColumns(GridPanel gp)
        {
            AddDoubleColumn(gp, "PreviousFact", "{0} (факт)".FormatWith(requestYear - 1));
            AddDoubleColumn(gp, "Fact", "{0} (факт)".FormatWith(requestYear));
            AddDoubleColumn(gp, "Estimate", "{0} (оценка)".FormatWith(requestYear + 1));
            AddDoubleColumn(gp, "Forecast", "{0} (прогноз)".FormatWith(requestYear + 2));
            GroupValueColumns.ColSpan = 4;
        }

        /// <summary>
        /// Добавляет столбец в грид (для значений показателей)
        /// </summary>
        /// <param name="gp">Грид с показателями</param>
        /// <param name="columnId">Идентификатор столбца (имя в Store)</param>
        /// <param name="header">Заголовок столбца</param>
        protected ColumnBase AddDoubleColumn(GridPanel gp, string columnId, string header)
        {
            var column = AddColumn(gp, columnId, header);
            column.Renderer.Fn = RendererFn;
            if (editable)
            {
                column.SetEditableDouble(2);
            }

            return column;
        }

        /// <summary>
        /// Добавляет столбец в грид
        /// </summary>
        /// <param name="gp">Грид с показателями</param>
        /// <param name="columnId">Идентификатор столбца (имя в Store)</param>
        /// <param name="header">Заголовок столбца</param>
        private static ColumnBase AddColumn(GridPanel gp, string columnId, string header)
        {
            var column = gp.ColumnModel
                .AddColumn(columnId, header, DataAttributeTypes.dtString);

            column.Sortable = false;
            column.Hideable = false;

            return column;
        }

        private Store CreateStore()
        {
            var ds = new Store
                         {
                             ID = IndicatorsStoreId,
                             Restful = false,
                             AutoDestroy = true,
                             ShowWarningOnFailure = false,
                             SaveAllFields = true,
                             SkipIdForNewRecords = false,
                             RefreshAfterSaving = RefreshAfterSavingMode.None
                         };

            ds.SetHttpProxy(ReadUrl);
            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = UpdateUrl,
                Method = HttpMethod.POST
            });
            ds.BaseParams.Add(new Parameter("applicationId", ApplicationId.ToString()));

            ds.SetJsonReader()
                .AddField("PreviousFact")
                .AddField("Fact")
                .AddField("Forecast")
                .AddField("Estimate")
                .AddField("RefName")
                .AddField("RefNumberString")
                .AddField("RowType")
                .AddField("RefMarks")
                .AddField("OKEI")
                .AddField("OKEIName")
                .AddField("PrevFactFormula")
                .AddField("FactFormula")
                .AddField("EstimateFormula")
                .AddField("ForecastFormula")
                .AddField("IsFormula")
                .AddField("Symbol")
                .AddField("HasDetail")
                .AddField("DetailMark")
                .AddField("TempID");

            if (IsCopy)
            {
                ApplicationId = -1;
                ds.Listeners.Load.AddAfter(@"
                    Ext.each({0}.data.items, function (record) {{ 
                        record.data.TempID = -record.data.TempID; 
                        record.dirty = true; 
                        record.modified = []; 
                        {0}.modified.push(record); 
                    }});".FormatWith(ds.ID));
            }

            ds.AddScript(@"
                var tempIndId = -2;
                isValid = function(store) {
                var recCnt = store.getCount();
                for (var i = 0; i < recCnt; i++){
                    data = store.getAt(i);
                    if (data.get('RefName') == '' || 
                        data.get('RefName') == null ||
                        (data.get('Fact') != 0 && (data.get('Fact') == '' || data.get('Fact') == null)) ||
                        (data.get('PreviousFact') != 0 && 
                            (data.get('PreviousFact') == '' || data.get('PreviousFact') == null)) ||
                        (data.get('Estimate') != 0 && (data.get('Estimate') == '' || data.get('Estimate') == null)) ||
                        (data.get('Forecast') != 0 && (data.get('Forecast') == '' || data.get('Forecast') == null)))
                    return false;
                }   
                return true;
            };");

            ds.Listeners.Load.AddAfter(AfterLoadListener);

            return ds;
        }
    }
}