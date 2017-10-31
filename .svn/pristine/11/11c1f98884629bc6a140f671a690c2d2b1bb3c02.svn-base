using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views
{
    public class FO51FormSborView : View
    {
        /// <summary>
        /// Список месяцев.
        /// </summary>
        private readonly List<string> months = new List<string>(12)
        {
            " января", 
            " февраля", 
            " марта", 
            " апреля", 
            " мая", 
            " июня", 
            " июля", 
            " августа", 
            " сентября", 
            " октября", 
            " ноября", 
            " декабря"
        };

        /// <summary>
        /// Функция: как отображать ИСполнено за отч.месяц.
        /// </summary>
        private const string RendererFactPeriod =
        @"function (v, p, r) {{
                    if (r.data.IsLeaf != true) 
                        p.css = 'gray-cell';
                    if (!r.data.Editable3456)
                        p.css = 'gray-cell';
                    var f = Ext.util.Format.numberRenderer('0.000,00/i');
                    return f(v);
                }}";

        /// <summary>
        /// Функция: как отображать поле с показателем (3 4 5 6).
        /// </summary>
        private const string Renderer3456Fn =
        @"function (v, p, r) {{
                    if ((r.data.IsLeaf != true && r.data.MarkCode != '3.8.2') || r.data.MarkCode == '3.8.2.1' || r.data.MarkCode == '3.8.2.2')
                        p.css = 'gray-cell';
                    if (!r.data.Editable3456)
                        p.css = 'gray-cell';
                    var f = Ext.util.Format.numberRenderer('0.000,00/i');
                    return f(v);
                }}";

        /// <summary>
        /// Функция: как отображать поле с показателем (остальные)
        /// </summary>
        private const string RendererFn =
        @"function (v, p, r) {{
                    if ((r.data.IsLeaf != true && r.data.MarkCode != '3.8.2') || r.data.MarkCode == '3.8.2.1' || r.data.MarkCode == '3.8.2.2') 
                        p.css = 'gray-cell';
                    var f = Ext.util.Format.numberRenderer('0.000,00/i');
                    return f(v);
                }}";

        /// <summary>
        /// Функция: как отображать поле с показателем.
        /// </summary>
        private const string RendererNameFn =
        @"function (v, p, r) {
                    var level = r.data.Level;
                    p.style = ' padding-left: ' + 10 * level + 'px;';
                    return v;
                }";

        private readonly string CheckEdit;
        private readonly D_Marks_PassportMO mark;
        private readonly int periodId;
        private readonly int userGroup;
        private readonly D_Regions_Analysis region;
        private Store ds;

        public FO51FormSborView(D_Marks_PassportMO mark, int periodId, D_Regions_Analysis region, int userGroup)
        {
            Id = "TabSbor_{0}".FormatWith(mark.ID);

            this.mark = mark;
            this.periodId = periodId;
            this.region = region;
            this.userGroup = userGroup;
            GridId = "GridPanel_{0}".FormatWith(mark.ID);
            var chekEditForMO = @"
                if(e.field == 'ScoreMOQuarter1' || e.field == 'ScoreMOQuarter2' || e.field == 'ScoreMOQuarter3' || e.field == 'ScoreMOQuarter4')
                    return false;
                if (e.record.get('MarkCode') == '3.8.2' && !(e.field == 'FactPeriod' || e.field == 'FactLastYear'))
                    return {0}.store.dataState == {1};
                if (e.record.get('MarkCode') == '3.8.2.1' || e.record.get('MarkCode') == '3.8.2.2') 
                    if (e.field == 'FactPeriod' || e.field == 'FactLastYear')
                        return {0}.store.dataState == {1};
                    else
                        return false;

                var editable = ((e.record.get('IsLeaf') == true) &&
                                {0}.store.dataState == {1});
                if (e.field == 'FactLastYear' || e.field == 'RefinPlan') {{
                    if  ({2}) {{
                        return editable;
                    }}
                    else {{
                        editable = editable && e.record.get('Editable3456');
                    }}
                }}
                else {{
                    if (e.field == 'FactPeriod' || e.field == 'PlanPeriod')
                    {{
                        editable = editable && e.record.get('Editable3456');
                    }}
                }}
                return editable;"
                .FormatWith(GridId, userGroup == FO51Extension.GroupMo ? FO51Extension.StateEdit : FO51Extension.StateConsider, mark.Name.Equals("Расходы") ? "true" : "false");
            CheckEdit = region.ID > 0 ? chekEditForMO : "return false;";
        }

        public bool Hidden { get; set; }

        public string GridId { get; private set; }

        public string BeforeLoadHandler
        {
            set
            {
                ds.Listeners.BeforeLoad.AddAfter(@"
                if ({0}.store.dataState == undefined || {0}.store.dataState == null) {{
                    {1}
                }}"
                    .FormatWith(GridId, value));
            }
        }

        public new List<Component> Build(ViewPage page)
        {
            var style = @"
            .gray-cell
            {
                background-color: #DCDCDC !important; 
                border-right-color: #FFFFFF; !important;
            }";
            ResourceManager.GetInstance(page).RegisterClientStyleBlock("CustomStyle", style);
            ResourceManager.GetInstance(page).RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");

            // Store для формы сбора.
            var store = CreateStore();
            page.Controls.Add(store);

            if (mark.Name.Equals("Доходы") || mark.Name.Equals("Источники финансирования дефицита бюджета") || mark.Name.Equals("Расходы"))
            {
                var storeCopy = CreateStoreCopyPrevEstimate();
                page.Controls.Add(storeCopy);
            }

            return new List<Component> { CreateFormGrid(page, store.ID) };
        }

        private static ColumnBase CreateColumn(
            string columnId, 
            string header, 
            DataAttributeTypes attributeType, 
            string renderer, 
            int width)
        {
            var column = ColumnFactory(attributeType);
            column.ColumnID = columnId;
            column.DataIndex = columnId;
            column.Header = header;
            column.Wrap = true;
            column.Renderer.Fn = renderer;
            column.SetEditableDouble(2);
            column.Sortable = false;
            column.Width = width;
            return column;
        }

        private static ColumnBase ColumnFactory(DataAttributeTypes attributeType)
        {
            switch (attributeType)
            {
                case DataAttributeTypes.dtBoolean:
                    return new CheckColumn();
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    return new Column().IsDate();
                case DataAttributeTypes.dtDouble:
                    return new NumberColumn();
                default:
                    return new Column();
            }
        }

        private GridPanel CreateFormGrid(ViewPage page, string storeId)
        {
            var gp = new GridPanel
                                {
                                    ID = GridId,
                                    LoadMask = { ShowMask = true },
                                    SaveMask = { ShowMask = true },
                                    Closable = false,
                                    Collapsible = false,
                                    Frame = true,
                                    StoreID = storeId,
                                    EnableColumnHide = false,
                                    ColumnLines = true,
                                    AutoExpandColumn = "MarkName",
                                    AutoExpandMin = 200,
                                    EnableColumnMove = false,
                                    SelectionModel = { new ExcelLikeSelectionModel() },
                                    View = { new GridView() },
                                    Title = mark.Name,
                                    Border = false,
                                    AutoScroll = true,
                                    Hidden = Hidden
                                };

            gp.SaveMask.Msg = "Сохранение";
            gp.SaveMask.ShowMask = true;
            gp.LoadMask.Msg = "Загрузка";
            gp.LoadMask.ShowMask = true;
            gp.Toolbar().Add(new DisplayField
                                 {
                                     Text = @"руб.", 
                                     StyleSpec = "font-weight: bold; padding-left: 5px; padding-right: 30px;"
                                 });

            CreateColumns(gp);

            gp.Listeners.BeforeEdit.AddAfter(CheckEdit);
            gp.AddRefreshButton();

            if (mark.Name.Equals("Расходы") && region.ID > 0 && region.RefBridgeRegions != null && region.RefBridgeRegions.ID != FO51Extension.RegionFictID)
            {
                AddMesOtchBtn(gp);
                AddDefectsBtn(gp);
            }

            if (mark.Name.Equals("Доходы") || mark.Name.Equals("Источники финансирования дефицита бюджета") || mark.Name.Equals("Расходы"))
            {
                AddCopyBtn(gp);
            }

            gp.AddColumnsWrapStylesToPage(page);
            gp.AddColumnsHeaderAlignStylesToPage(page);
            return gp;
        }

        private void AddCopyBtn(GridPanel gp)
        {
            var stateEditable = userGroup == FO51Extension.GroupMo ? FO51Extension.StateEdit : FO51Extension.StateConsider;
            var copyBtn = gp.Toolbar()
                .AddIconButton(
                    "{0}CopyPrevEstimate".FormatWith(gp.ID),
                    Icon.PageCode,
                    "Скопировать оценки по месяцам с прошлого отчетного периода",
                    @"if ({0}.store.dataState == {1}) {{ MarksStoreCopy_{2}.load(); }}".FormatWith(GridId, stateEditable, mark.ID));

            copyBtn.Hidden = (periodId % 10000) / 100 == 1;
            copyBtn.DirectEvents.Click.ExtraParams.Add(new ProgressConfig("Экспорт утвержденных отчетов..."));
            copyBtn.Text = @"Скопировать оценки";
        }

        private void CreateColumns(GridPanel gp)
        {
            var width = region.ID > 0 ? 100 : 130;
            var colOrigPlan = CreateColumn(
                "OrigPlan", 
                "План на год (первоначально утвержденный)",
                DataAttributeTypes.dtString, 
                RendererFn,
                width);
            var colScoreMO = new List<ColumnBase>();
            for (int i = 1; i < 13; i++)
            {
                colScoreMO.Add(CreateColumn("ScoreMO{0}".FormatWith(i), "Оценка исполнения{0}".FormatWith(months[i - 1]), DataAttributeTypes.dtString, RendererFn, width));
            }

            var colFactPeriod = CreateColumn("FactPeriod", "Исполнено за отчетный месяц", DataAttributeTypes.dtString, RendererFactPeriod, width);
            var colPlanPeriod = CreateColumn("PlanPeriod", "Уточненный план на год (по месячному отчету)", DataAttributeTypes.dtString, Renderer3456Fn, width);
            var rendererLastYear = mark.Name.Equals("Расходы") ? RendererFn : (mark.Name.Equals("Источники финансирования дефицита бюджета") ? RendererFactPeriod : Renderer3456Fn);
            var rendererRefinPlan = mark.Name.Equals("Расходы") ? RendererFn : Renderer3456Fn;

            var colFactLastYear = CreateColumn("FactLastYear", "Исполнено за отчетный год (по годовому отчету)", DataAttributeTypes.dtString, rendererLastYear, width);
            var colRefinPlan = CreateColumn("RefinPlan", "Уточненный план на год (по годовому отчету)", DataAttributeTypes.dtString, rendererRefinPlan, width);

            var columnName = CreateColumn("MarkName", "Показатели", DataAttributeTypes.dtString, RendererNameFn, 300);
            columnName.Fixed = true;
            columnName.Align = Alignment.Center;
            columnName.SetEditable(false);
            gp.ColumnModel.Columns.Add(columnName);
            gp.ColumnModel.GetColumnById("MarkName").Resizable = false;

            var month = (periodId / 100) % 100;

            var scriptCheckJanuary =
                @"
                    var colOrigPlan = gp.colModel.getColumnById('{0}');
                    if (month != 1)
                        colOrigPlan.hidden = true;
                    else
                        colOrigPlan.hidden = false;";

            var scriptCheckQuarters = @"
                /*gp.colModel.getColumnById('ScoreMOQuarter1').hidden = month > 2;
                gp.colModel.getColumnById('ScoreMOQuarter2').hidden = month > 5;
                gp.colModel.getColumnById('ScoreMOQuarter3').hidden = month > 8;
                gp.colModel.getColumnById('ScoreMOQuarter4').hidden = month > 11;*/
            ";

            var scriptCheckScoreMO = String.Empty;
            for (var i = 1; i < 13; i++)
            {
                scriptCheckScoreMO +=
                    @"
                        var col{0} = gp.colModel.getColumnById('ScoreMO{0}'); 
                        if ({0} > month) col{0}.hidden = false;
                        else col{0}.hidden = true;"
                        .FormatWith(i);
            }

            if (mark.Name.Equals("Доходы") || mark.Name.Equals("Источники финансирования дефицита бюджета"))
            {
                gp.AddScript(@"
                function CheckHidden_{0}(gp){{
                    var month = cbPeriodMonth.value / 100 % 100;
                    {1}
                    {2}
                    {3}
                    {4}
                    {5}
                    gp.show();
                 }};".FormatWith(
                    mark.ID, 
                    scriptCheckJanuary.FormatWith("OrigPlan"),
                    scriptCheckJanuary.FormatWith("FactLastYear"),
                    scriptCheckJanuary.FormatWith("RefinPlan"), 
                    scriptCheckScoreMO,
                    mark.Name.Equals("Источники финансирования дефицита бюджета") ? String.Empty : scriptCheckQuarters));

                gp.ColumnModel.Columns.Add(colOrigPlan);
                gp.ColumnModel.Columns.Add(colFactPeriod);
                gp.ColumnModel.Columns.Add(colFactLastYear.SetHidden(month != 1));
                gp.ColumnModel.Columns.Add(colPlanPeriod);
                gp.ColumnModel.Columns.Add(colRefinPlan.SetHidden(month != 1));

                // Виден только в январе.
                colOrigPlan.SetHidden(month != 1);

                CreateScoreMOColumns(gp, colScoreMO, month);
            }
            else
                if (mark.Name.Equals("Расходы"))
                {
                    gp.AddScript(@"function CheckHidden_{0}(gp){{
                        var month = cbPeriodMonth.value / 100 % 100;
                        {1}
                        {2}
                        {3}
                        {4}
                        {5}
                        gp.show();
                    }};".FormatWith(
                        mark.ID,
                        scriptCheckJanuary.FormatWith("OrigPlan"),
                        scriptCheckJanuary.FormatWith("FactLastYear"),
                        scriptCheckJanuary.FormatWith("RefinPlan"),
                        scriptCheckScoreMO,
                        scriptCheckQuarters));

                    gp.ColumnModel.Columns.Add(colOrigPlan.SetHidden(month != 1));
                    gp.ColumnModel.Columns.Add(colFactPeriod);
                    gp.ColumnModel.Columns.Add(colFactLastYear.SetHidden(month != 1));
                    gp.ColumnModel.Columns.Add(colPlanPeriod);
                    gp.ColumnModel.Columns.Add(colRefinPlan.SetHidden(month != 1));

                    CreateScoreMOColumns(gp, colScoreMO, month);
                }
                else
                    if (mark.Name.Equals("Кредиторская задолженность"))
                    {
                        gp.AddScript(@"function CheckHidden_{0}(gp){{
                        }};".FormatWith(mark.ID));

                        gp.ColumnModel.Columns.Add(colFactPeriod);
                    }
                    else
                        if (mark.Name.Equals("Справочно"))
                        {
                            gp.AddScript(@"function CheckHidden_{0}(gp){{
                                var month = cbPeriodMonth.value / 100 % 100;
                                {1}
                                if (month == 1) {{
                                    {2}.setDisabled(false);
                                }}
                                else {{
                                    {2}.setDisabled(true);
                                }}
                            }};".FormatWith(mark.ID, scriptCheckJanuary.FormatWith("FactLastYear"), GridId));

                            gp.ColumnModel.Columns.Add(colFactLastYear.SetHidden(month != 1));
                        }
        }

        private void AddDefectsBtn(GridPanel gp)
        {
            var defectsBtn = new Button
                                 {
                                     ToolTip = @"Просмотреть нарушения от месячной отчетности", 
                                     Text = @"Сверка данных", Icon = Icon.CheckError
                                 };

            defectsBtn.Listeners.Click.Handler = @"  
                    var month = (({1}.store.baseParams.periodId - {1}.store.baseParams.periodId % 100) / 100) % 100;
                    var year = (({1}.store.baseParams.periodId - {1}.store.baseParams.periodId % 10000) / 10000);
                    parent.MdiTab.addTab({{ 
                        title: 'Сверка данных {0} (' + month + '.' + year + ')', 
                        url: '/FO51FormSbor/Book?periodId=' + {1}.store.baseParams.periodId + '&regionId={2}', 
                        icon: 'icon-report'
                    }});".FormatWith(region.Name, GridId, region.ID, ((periodId / 100) % 100).ToString() + '.' + (periodId / 10000));
            gp.Toolbar().Add(defectsBtn);
        }

        private void AddMesOtchBtn(GridPanel gp)
        {
            var defectsBtn = new Button
            {
                ToolTip = @"Получить данные месячной отчетности",
                Icon = Icon.PageAdd
            };
            var addMesOtchHandler = @"saveMesOtchHandler({0}, 'Получение данных', 'Получение данных месячной отчетности...', {1}, {2});"
                .FormatWith(region.ID, gp.ID, mark.ID);

            defectsBtn.Listeners.Click.Handler = addMesOtchHandler;

            gp.Toolbar().Add(defectsBtn);
        }

        private void CreateScoreMOColumns(GridPanelBase gp, IList<ColumnBase> colScoreMO, int month)
        {
            for (var i = 0; i < 12; i++)
            {
                gp.ColumnModel.Columns.Add(colScoreMO[i].SetHidden(month > i));
            }
        }

        /// <summary>
        /// Формирование Store для форм сбора.
        /// </summary>
        /// <returns>Store для форм сбора.</returns>
        private Store CreateStore()
        {
            ds = new Store
                         {
                             ID = "MarksStore_{0}".FormatWith(mark.ID),
                             Restful = false,
                             ShowWarningOnFailure = false,
                             SkipIdForNewRecords = false,
                             RefreshAfterSaving = RefreshAfterSavingMode.Always,
                             DirtyWarningTitle = @"Несохраненные изменения",
                             DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
                         };

            ds.Proxy.Add(new HttpProxy { Url = "/FO51FormSbor/Read?markId={0}".FormatWith(mark.ID), Timeout = 5000000 });

            ds.SetJsonReader()
                .AddField("ID")
                .AddField("MarkName")
                .AddField("MarkCode")
                .AddField("OrigPlan")
                .AddField("ScoreMO1")
                .AddField("ScoreMO2")
                .AddField("ScoreMO3")
                .AddField("ScoreMO4")
                .AddField("ScoreMO5")
                .AddField("ScoreMO6")
                .AddField("ScoreMO7")
                .AddField("ScoreMO8")
                .AddField("ScoreMO9")
                .AddField("ScoreMO10")
                .AddField("ScoreMO11")
                .AddField("ScoreMO12")
                .AddField("ScoreMOQuarter1")
                .AddField("ScoreMOQuarter2")
                .AddField("ScoreMOQuarter3")
                .AddField("ScoreMOQuarter4")
                .AddField("IsLeaf")
                .AddField("Level")

                .AddField("State")
                .AddField("Editable3456")
                .AddField("FactPeriod")
                .AddField("FactLastYear")
                .AddField("PlanPeriod")
                .AddField("RefinPlan");

            var urlSave = "/FO51FormSbor/Save";

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = urlSave,
                Method = HttpMethod.POST,
                Timeout = 5000000 
            });

            ds.Listeners.BeforeSave.AddAfter(@"
                {0}.store.updateProxy.conn.url = '{1}?periodId=' + {0}.store.baseParams.periodId + '&parentMarkId={2}&regionId={3}';
                if ({0}.store.dataState != undefined && {0}.store.dataState != null) 
                    {0}.store.updateProxy.conn.url += '&state=' + {0}.store.dataState;"
                .FormatWith(GridId, urlSave, mark.ID, region.ID, mark.Name));

            ds.Listeners.BeforeLoad.AddAfter(@"
                Ext.apply({0}.store.baseParams, {{ periodId: {2}, regionId: {1} }}); 
                ".FormatWith(GridId, region.ID, userGroup == FO51Extension.GroupMo ? "(cbPeriodMonth.value || -1)" : periodId.ToString()));

            ds.Listeners.SaveException.Handler = @" 
                Ext.MessageBox.hide();
                if (response.responseText != null && response.responseText != undefined) {
                    var fi = response.responseText.indexOf('message:') + 9;
                    var li = response.responseText.lastIndexOf('" + '"' + @"');
                    var textToShow = response.responseText.substring(li, fi);
                    Ext.net.Notification.show({
                        iconCls    : 'icon-information', 
                        html       : textToShow, 
                        title      : 'Внимание', 
                        hideDelay  : 10000
                    });
                }";

            ds.Listeners.Save.Handler = @"  
                Ext.MessageBox.hide();
                Ext.net.Notification.show({
                    iconCls    : 'icon-information', 
                    html       : arguments[2].message, 
                    title      : 'Внимание', 
                    hideDelay  : 10000
                });";

            return ds;
        }

        /// <summary>
        /// Формирование Store для копирования оценок с прошлого месяца.
        /// </summary>
        /// <returns> Store с оценами с прошлого месяца.</returns>
        private Store CreateStoreCopyPrevEstimate()
        {
            ds = new Store
            {
                ID = "MarksStoreCopy_{0}".FormatWith(mark.ID),
                Restful = false,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                AutoLoad = false
            };

            ds.Proxy.Add(new HttpProxy { Url = "/FO51FormSbor/ReadCopy?markId={0}".FormatWith(mark.ID), Timeout = 5000000 });

            ds.SetJsonReader()
                .AddField("ID")
                .AddField("ScoreMO1")
                .AddField("ScoreMO2")
                .AddField("ScoreMO3")
                .AddField("ScoreMO4")
                .AddField("ScoreMO5")
                .AddField("ScoreMO6")
                .AddField("ScoreMO7")
                .AddField("ScoreMO8")
                .AddField("ScoreMO9")
                .AddField("ScoreMO10")
                .AddField("ScoreMO11")
                .AddField("ScoreMO12")
                .AddField("ScoreMOQuarter1")
                .AddField("ScoreMOQuarter2")
                .AddField("ScoreMOQuarter3")
                .AddField("ScoreMOQuarter4")
                .AddField("IsLeaf")
                .AddField("Level");

            ds.Listeners.BeforeLoad.AddAfter(@"
                Ext.apply({0}.baseParams, {{ periodId: {2}, regionId: {1} }}); 
                {3}.loadMask.show();
                ".FormatWith(ds.ID, region.ID, userGroup == FO51Extension.GroupMo ? "(cbPeriodMonth.value || -1)" : periodId.ToString(), GridId));

            ds.Listeners.Load.Handler = @"
                copyPrevEstimate(MarksStoreCopy_{0}, {1})
                {1}.loadMask.hide();".FormatWith(mark.ID, GridId);

            return ds;
        }
    }
}
