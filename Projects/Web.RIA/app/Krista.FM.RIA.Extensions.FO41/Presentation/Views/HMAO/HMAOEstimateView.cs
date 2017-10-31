using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOEstimateView : View
    {
        private readonly int categoryId;
        private readonly int taxTypeId;
        private readonly int periodId;

        /// <summary>
        /// Функция: как отображать поле с показателем
        /// </summary>
        private const string RendererFn =
        @"function (v, p, r) {
                    f = Ext.util.Format.numberRenderer('0.000,00/i');
                    return f(v);
                }";

        public HMAOEstimateView(int categoryId, int taxTypeId, int periodId)
         {
            this.categoryId = categoryId;
            this.taxTypeId = taxTypeId;
            this.periodId = periodId;
            Editable = true;
         }

        /// <summary>
        /// Признак, редактируема ли вкладка с оценкой
        /// </summary>
        public bool Editable { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var store = CreateStore();
            page.Controls.Add(store);

            var estimatePanel = new Panel
            {
                ID = "EstimateTab{0}".FormatWith(categoryId),
                Title = @"Оценка",
                Border = false,
                Height = 100,
                LabelWidth = 1,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Items = { new BorderLayout { Center = { Items = { CreateGrid(page, store.ID) } } } }
            };

            return new List<Component> { estimatePanel };
        }

        /// <summary>
        /// Добавляет столбец в грид
        /// </summary>
        /// <param name="gp">Грид с показателями оценки</param>
        /// <param name="columnId">Идентификатор столбца (имя в Store)</param>
        /// <param name="header">Заголовок столбца</param>
        private static void AddColumn(GridPanel gp, string columnId, string header)
        {
            var column = gp.ColumnModel
                .AddColumn(columnId, header, DataAttributeTypes.dtString);
            column.SetEditableDouble(2);
            column.Sortable = false;
            column.Hideable = false;
            column.Renderer.Fn = RendererFn;
        }

        /// <summary>
        /// Создает грид с показателями оценки
        /// </summary>
        /// <param name="page">Родительская страница</param>
        /// <param name="storeId">Идентификатор Store</param>
        /// <returns>Грид с показателями оценки</returns>
        private GridPanel CreateGrid(ViewPage page, string storeId)
        {
            var gp = new GridPanel
            {
                ID = "EstimateGrid{0}".FormatWith(categoryId),
                LoadMask = { ShowMask = true, Msg = "Загрузка" },
                SaveMask = { ShowMask = true, Msg = "Сохранение" },
                StoreID = storeId,
                AutoScroll = true,
                EnableColumnMove = false,
                SelectionModel = { new ExcelLikeSelectionModel() }
            };

            gp.ColumnModel
                .AddColumn("RefNumberString", "№ п/п", DataAttributeTypes.dtString)
                .SetWidth(25);
            gp.ColumnLines = true;
            gp.ColumnModel
                .AddColumn("RefName", "Наименование показателя", DataAttributeTypes.dtString)
                .SetWidth(200);

            AddColumn(gp, "Fact", "{0} За отчетный (налоговый) период".FormatWith(periodId / 10000));

            gp.AutoExpandColumn = "RefName";
            gp.AddColumnsWrapStylesToPage(page);

            if (Editable)
            {
                var calc = new Button
                               {
                                   ID = "calcEstimate{0}".FormatWith(categoryId),
                                   Text = @"Рассчитать по исходным данным",
                                   Icon = Icon.Calculator,
                                   Hidden = !Editable
                               };

                gp.Toolbar().Add(calc);

                // сохранить обобщенную форму, рассчитать показатели оценки, обновить грид
                calc.Listeners.Click.Handler =
                    @"
                Ext.Ajax.request(
                {{
                    url: '/FO41HMAOEstimate/SaveResultData?categoryId={0}&periodID={1}&taxTypeID={2}',
                    success: function (response, options) {{
                        Ext.Ajax.request(
                        {{
                            url: '/FO41HMAOEstimate/CalculateIndicators?categoryId={0}&periodID={1}&taxTypeID={2}',
                            success: function (response, options) {{
                                {3}.reload();
                            }},
                            failure: function (response, options) {{}}
                        }});
                    }},
                    failure: function (response, options) {{}}
                }});"
                        .FormatWith(categoryId, periodId, taxTypeId, storeId);

                gp.Toolbar().Add(new Button
                           {
                               ID = "exportEstimateCategoryButton{0}".FormatWith(categoryId),
                               Icon = Icon.PageExcel,
                               ToolTip = @"Выгрузка в Excel итоговых показателей оценки по категории",
                               Text = @"Выгрузка в Excel показателей оценки",
                               Listeners =
                                   {
                                       Click =
                                           {
                                               Handler =
                                                   @"
                                                    Ext.net.DirectMethod.request({{
                                                        url: '/FO41Export/ReportEstimateCategoryHMAO',
                                                        isUpload: true,
                                                        formProxyArg: '{0}',
                                                        cleanRequest: true,
                                                        params: {{
                                                            categoryId: {1}, periodId: {2}
                                                        }},
                                                        success:function (response, options) {{
                                                        }},
                                                        failure: function (response, options) {{
                                                        }}
                                                    }});"
                                                   .FormatWith("HMAOTaxForm{0}".FormatWith(taxTypeId), categoryId, periodId)
                                           }
                                   }
                           });
            }

            gp.AddRefreshButton();
            var saveBtn = gp.AddSaveButton();
            saveBtn.Hidden = !Editable;

            return gp;
        }

        /// <summary>
        /// Создает Store для грида с показателями оценки
        /// </summary>
        /// <returns>Store для показателей оценки</returns>
        private Store CreateStore()
        {
            var ds = new Store
            {
                ID = "EstimateStore{0}".FormatWith(categoryId),
                Restful = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные изменения. Перезагрузить данные?"
            };

            ds.SetRestController("FO41ResultIndicators")
                .SetJsonReader()
                .AddField("Fact")
                .AddField("RefName")
                .AddField("RefNumberString")
                .AddField("RefMarks")
                .AddField("TempID");

            ds.Proxy.Proxy.RestAPI.ReadUrl = 
                "/FO41HMAOEstimate/Read?categoryId={0}&periodId={1}&taxTypeId={2}"
                .FormatWith(categoryId, periodId, taxTypeId);
            const string UrlCreate = "/FO41ResultIndicators/SaveEstimate";
            ds.Proxy.Proxy.RestAPI.CreateUrl = UrlCreate;
            ds.Proxy.Proxy.RestAPI.UpdateUrl = UrlCreate;

            return ds;
        }        
     }
}
