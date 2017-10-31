using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOResultTaxView : View
    {
        /// <summary>
        /// Репозиторий категорий
        /// </summary>
        private readonly ICategoryTaxpayerService categoryRepository;

        /// <summary>
        /// Идентификатор типа налога
        /// </summary>
        private readonly int taxTypeId;

        /// <summary>
        /// Идентификатор периода
        /// </summary>
        private readonly int periodId;

        /// <summary>
        /// Функция: как отображать поле с показателем
        /// </summary>
        private const string RendererFn =
        @"function (v, p, r) {
                    p.css = 'gray-cell';
                    var okei = r.data.OKEI;
                    var f;
                    if  (okei == 744)
                        f = Ext.util.Format.numberRenderer('0.000,00/i');
                    else
                    if  (okei == 383 || okei == 384)
                        f = Ext.util.Format.numberRenderer('0.000,0/i');
                    else if (okei == 792 || okei == 796)
                        f = Ext.util.Format.numberRenderer('0.000,/i');
                    else
                        f = Ext.util.Format.numberRenderer('0.000,/i');
                    return f(v);
                }";

        public HMAOResultTaxView(ICategoryTaxpayerService categoryRepository, int taxTypeId, int periodId)
         {
            this.categoryRepository = categoryRepository;
            this.taxTypeId = taxTypeId;
            this.periodId = periodId;
            Editable = true;
         }

        public bool Editable { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var store = CreateStore();
            page.Controls.Add(store);

            var estimatePanel = new Panel
            {
                ID = "ResultTaxTab{0}".FormatWith(taxTypeId),
                Title = @"Общая форма по налогу",
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
                             ID = "ResultTaxGrid{0}".FormatWith(taxTypeId),
                             LoadMask = { ShowMask = true },
                             SaveMask = { ShowMask = true },
                             StoreID = storeId,
                             AutoScroll = true,
                             AutoExpandMin = 150,
                             AutoExpandColumn = "RefName",
                             EnableColumnMove = false,
                             ColumnLines = true
                         };
            
            gp.ColumnModel
                .AddColumn("RefNumberString", "№ п/п", DataAttributeTypes.dtString)
                .SetWidth(30);
            gp.ColumnLines = true;
            gp.ColumnModel
                .AddColumn("RefName", "Наименование показателя", DataAttributeTypes.dtString)
                .SetWrap(true);

            var categories = categoryRepository.GetByTax(taxTypeId);
            foreach (var category in categories)
            {
                AddColumn(
                    gp,
                    "Fact{0}".FormatWith(category.ID),
                    "{0}".FormatWith(category.ShortName));
            }

            var toExcel = new Button
                              {
                                  ID = "resultTaxToExcel{0}".FormatWith(taxTypeId),
                                  Text = @"Выгрузка в Excel",
                                  Icon = Icon.PageExcel,
                                  Listeners =
                                      {
                                          Click =
                                              {
                                                  Handler = @"
                                                    Ext.net.DirectMethod.request({{
                                                        url: '/FO41Export/ReportResultTaxTypeHMAO',
                                                        isUpload: true,
                                                        formProxyArg: '{2}',
                                                        cleanRequest: true,
                                                        params: {{
                                                            taxTypeId: {0},
                                                            periodId: {1}
                                                        }},
                                                        success:function (response, options) {{
                                                        }},
                                                        failure: function (response, options) {{
                                                        }}
                                                    }});".FormatWith(taxTypeId, periodId, "HMAOTaxForm{0}".FormatWith(taxTypeId))
                                              }
                                      }
                              };

            gp.AddColumnsWrapStylesToPage(page); 
            gp.Listeners.BeforeEdit.AddAfter("return false;");
            gp.AddRefreshButton(); 
            gp.Toolbar().Add(toExcel);
            
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
                ID = "taxTypeStore{0}".FormatWith(taxTypeId),
                Restful = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные изменения. Перезагрузить данные?"
            };

            ds.SetRestController("FO41HMAOIndicators")
                .SetJsonReader()
                .AddField("RefName")
                .AddField("RefNumberString")
                .AddField("RefMarks")
                .AddField("OKEI")
                .AddField("TempID");

            var categories = categoryRepository.GetByTax(taxTypeId);
            foreach (var category in categories)
            {
                ds.AddField("Fact{0}".FormatWith(category.ID));
            }

            ds.Proxy.Proxy.RestAPI.ReadUrl =
                "/FO41HMAOIndicators/ReadByTax?&periodId={0}&taxTypeId={1}"
                .FormatWith(periodId, taxTypeId);

            return ds;
        }        
    }
}
