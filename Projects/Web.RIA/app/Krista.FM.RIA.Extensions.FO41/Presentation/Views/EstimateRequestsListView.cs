using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;
using Krista.FM.ServerLibrary;
using Control = System.Web.UI.Control;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class EstimateRequestsListView : View
    {
        /// <summary>
        /// Глобальные параметры
        /// </summary>
        private readonly IFO41Extension extension;

        private readonly int periodId;

        /// <summary>
        /// Функция: как отображать статус заявки
        /// </summary>
        private const string RendererFn =
            @"function (v, p, r, rowIndex, colIndex, ds) { 
                var tpl = '<img title=\'{1}\' src=\'{0}\' width=\'16\' height=\'16\' />';
                return String.format(
                    tpl, 
                    Ext.getCmp(String.format('UrlIconStatus{0}', r.data.RefStateID)).getValue(), r.data.RefState);
            }";

        public EstimateRequestsListView(
            IFO41Extension extension, 
            NHibernateLinqRepository<D_OMSU_ResponsOIV> ogvRepository,
            int periodId)
        {
            this.extension = extension;
            OGVs = ogvRepository.FindAll().ToList();
            this.periodId = periodId;
        }

        /// <summary>
        /// Список ОГВ, по которым группировать заявки
        /// </summary>
        public List<D_OMSU_ResponsOIV> OGVs { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            if (extension.ResponsOIV == null)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен ОГВ или ДФ.").ToScript());

                return new List<Component>();
            }

            page.Controls.Add(CreateCategoriesStore());

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportRequestsList",
                    Items = 
                    {
                        new BorderLayout
                        {
                            Center = { Items = { CreateRequestsGridPanel(page) } }
                        }
                    }
                }
            };
        }

        private static Control CreateRequestsStore(int periodId)
        {
            var ds = new Store
            {
                ID = "dsRequests",
                Restful = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                GroupField = "RefOGVId"
            };

            ds.SetRestController("FO41Estimate")
                .SetJsonReader()
                .AddField("ID")
                .AddField("RowType")
                .AddField("Executor")
                .AddField("RequestDate")
                .AddField("RefCategory")
                .AddField("RefCategoryShort")
                .AddField("RefStateID")
                .AddField("RefState")
                .AddField("RefOGVName")
                .AddField("RefCategoryId")
                .AddField("PeriodId")
                .AddField("RefOGVId");

            // параметр, с какими состояниями показывать заявки
            ds.BaseParams.Add(new Parameter("filter", "getStateFilter()", ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter("periodId", periodId.ToString(), ParameterMode.Raw));

            // функции обновления store для заявок; 
            // формирования массива признаков - с какими состояниями показывать заявки
            ds.AddScript(
                @"
                function toggleFilter(button, state) {{
                    {0}.load();
                }}

                function getStateFilter() {{
                    var filter = [true, true, true, true, true];
                    filter[0] = filterCreate.pressed;
                    filter[1] = filterEstimate.pressed;
                    filter[2] = filterMagnify.pressed;
                    filter[3] = filterAccept.pressed;
                    filter[4] = filterReject.pressed;

                    return filter;
                }}
                ".FormatWith(ds.ID));

            return ds;
        }

        private static Control CreateCategoriesStore()
        {
            var ds = new Store
                         {
                             ID = "dsCategories",
                             Restful = true,
                             ShowWarningOnFailure = false,
                             SkipIdForNewRecords = false,
                             RefreshAfterSaving = RefreshAfterSavingMode.None
                         };

            ds.SetRestController("FO41Categories")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Code")
                .AddField("Name")
                .AddField("ShortName")
                .AddField("CorrectIndex")
                .AddField("RowType");

            return ds;
        }

        /// <summary>
        ///  Формирует панель закладок с заявками
        /// </summary>
        /// <param name="page"> Родительский page</param>
        /// <returns>Панель закладок</returns>
        private IEnumerable<Component> CreateRequestsGridPanel(ViewPage page)
        {
            page.Controls.Add(CreateRequestsStore(periodId));
            var form = new FormPanel
            {
                ID = "RequestsOGVListFrom",
                Border = false,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                Layout = "RowLayout"
            };
            return new List<Component>
                       {
                            new Hidden { ID = "UrlIconStatus1", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit) },
                            new Hidden { ID = "UrlIconStatus3", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserMagnify) },
                            new Hidden { ID = "UrlIconStatus2", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick) },
                            new Hidden { ID = "UrlIconStatus4", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Accept) },
                            new Hidden { ID = "UrlIconStatus5", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Cancel) },
                           form, 
                           GetGridByOGV(page)
                       };
        }

        /// <summary>
        /// Формирует грид с закладками для определенного ОГВ
        /// </summary>
        /// <param name="page">Родительский page</param>
        /// <returns>Грид с закладками для ОГВ</returns>
        private GridPanel GetGridByOGV(ViewPage page)
        {
            var gp = new GridPanel
                         {
                             ID = "gpRequests",
                             Icon = Icon.Table,
                             Closable = false,
                             Frame = true,
                             StoreID = "dsRequests",
                             SelectionModel = { new RowSelectionModel() },
                             View = 
                             { 
                                 new GroupingView
                                 {
                                    HideGroupedColumn = false,
                                    ForceFit = true,
                                    GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Заявок' : 'Заявка']})",
                                    EnableRowBody = true,
                                    EnableNoGroups = true,
                                    EnableGrouping = true,
                                    EmptyGroupText = @"Пусто"
                                 } 
                            },
                         };

            // верхняя панель с фукнциями сортировки по состоянию, переходу состояний и тп
            var topBar = new RequestsListToolbar(extension, true);
            topBar.Build(-1, periodId);
            gp.TopBar.Add(topBar);

            // колонка на открытие заявки
            var command = new GridCommand { Icon = Icon.ApplicationGo, CommandName = "OpenRequest" };
            command.ToolTip.Text = "Открыть заявку"; 
            var column = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
            column.Commands.Add(command);
            column.Groupable = false;
            gp.ColumnModel.Columns.Add(column);

            // Колонка с состоянием заявки
            var columnState = new Column
            {
                ColumnID = "RefState",
                Header = string.Empty,
                Width = 40,
                Hideable = false,
                Groupable = false,
                Renderer =
                {
                    Fn = RendererFn
                }
            };

            gp.ColumnModel.Columns.Add(columnState);

            gp.ColumnModel.AddColumn("ID", "Номер заявки", DataAttributeTypes.dtString).SetWidth(90).SetGroupable(false);
            gp.ColumnModel.AddColumn("RefCategoryShort", "Категория", DataAttributeTypes.dtString).SetWidth(90).SetGroupable(false);
            gp.ColumnModel.AddColumn("RequestDate", "Дата создания заявки", DataAttributeTypes.dtString).SetWidth(120).SetGroupable(false);
            gp.AddColumn(new Column
                                {
                                    ColumnID = "RefOGVId",
                                    Hidden = true,
                                    DataIndex = "RefOGVId", 
                                    Header = "Ответственный ОГВ",
                                    
                                    GroupRenderer =
                                        {
                                            Handler =
                                                @"//function(value,metadata,record,rowIndex,colIndex,store)
return '<span style=""visibility: visible;"">' + record.data.RefOGVName + '</span>';
"
                                        }
                                });

            // Расширяем колонку наименование
            gp.AutoExpandColumn = "RefCategoryShort";

            if (extension.IsReqLastPeriod(periodId))
            {
                // по выбору строки с заявкой активизируем / делаем недоступными кнопки перехода состояний
                gp.Listeners.RowClick.Handler = extension.ResponsOIV.Role.Equals("ОГВ")
                                                    ? @"
                // если в состоянии Создана или На доработке у ОГВ - доступна кнопка - на оценку
                var state = this.store.getAt(rowIndex).get('RefStateID');
                {0}.hidden = !(state == 1 || state == 3);
                if ({0}.hidden) {0}.hide();
                else {0}.show();"
                                                          .FormatWith("toEstimate")
                                                    : @"
                // если в состоянии На оценке - доступны кнопки - Вернуть на доработку, Принять, Отклонить
                var state = this.store.getAt(rowIndex).get('RefStateID');
                if (state == 2) { toReEdit.show(); toAccept.show(); toReject.show(); }
                else { toReEdit.hide(); toAccept.hide(); toReject.hide(); }";

                gp.Listeners.KeyPress.AddAfter(
                    @"
                var k = e.charCode || e.keyCode;
                var rowIndex = this.selModel.last;
                if (k == e.PAGE_UP && rowInd > 0)
                    rowIndex--;
                if (k == e.PAGE_DOWN && rowInd < this.store.data.items.length - 1)
                    rowIndex++;
                {0}
                "
                        .FormatWith(gp.Listeners.RowClick.Handler));
            }

            // обработчик на открытие заявки
            gp.Listeners.Command.Handler = @"
            if (command == 'OpenRequest') {
                parent.MdiTab.addTab({ 
                    id: 'estReq_' + record.id,
                    title: 'Редактирование заявки (' + record.data.RefCategoryShort + ')', 
                    url: '/FO41Estimate/ShowRequest?appFromOGVId=' + record.id + 
                                '&categoryId=' + record.data.RefCategoryId + 
                                '&periodId=' + record.data.PeriodId
                });
            }";
            
            // ToolTip для строки (заявки) - отображаем полное название категории
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
                                            this.body.dom.innerHTML = '<b>Категория :</b> ' 
                                            + #{{{0}}}.store.getAt(rowIndex).get('RefCategory');".FormatWith(gp.ID)
                                                }
                                        }
                                });

            gp.AddColumnsWrapStylesToPage(page);
            return gp;
        }
    }
}
