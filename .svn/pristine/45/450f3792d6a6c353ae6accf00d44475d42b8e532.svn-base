using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class EstimateView : View
    {
        /// <summary>
        /// Идентификатор заявки от ОГВ
        /// </summary>
        private readonly int appFromOGVId;

        /// <summary>
        /// Стиль для контролов
        /// </summary>
        private const string StyleAll = "margin: 0px 0px 5px 10px; font-size: 12px;";

        /// <summary>
        /// Глобальные параметры
        /// </summary>
        private readonly IFO41Extension extension;

        /// <summary>
        /// Репозиторий глобальных показателей (для величины прожиточного минимума)
        /// </summary>
        private readonly IRepository<D_Marks_NormPrivilege> normRepository;

        private readonly IAppFromOGVService appFromOGVRepository;

        /// <summary>
        /// Краткое наименование категории
        /// </summary>
        private readonly string categoryShortName;

        /// <summary>
        /// Заявка от ОГВ
        /// </summary>
        private readonly D_Application_FromOGV app;

        /// <summary>
        /// Год создания заявки
        /// </summary>
        private readonly int appYear;

        /// <summary>
        /// Функция: как отображать поле с показателем
        /// </summary>
        private const string RendererFn =
@"function (v, p, r) {
                    var okei = r.data.OKEI;
                    var f;
                    if (okei == 0)
                        f = Ext.util.Format.numberRenderer('0.000,00/i');
                    else    
                    if  (okei == 383 || okei == 384 || okei == 744)
                        f = Ext.util.Format.numberRenderer('0.000,0/i');
                    else if (okei == 792)
                        f = Ext.util.Format.numberRenderer('0.000,/i');
                    else
                        f = Ext.util.Format.numberRenderer('0.000,/i');
                    return f(v);
                }";

        public EstimateView(IFO41Extension extension, IRepository<D_Marks_NormPrivilege> normRepository, IAppFromOGVService appFromOGVRepository, int appFromOGVId, string categoryShortName)
        {
            this.extension = extension;
            this.normRepository = normRepository;
            this.appFromOGVRepository = appFromOGVRepository;
            this.appFromOGVId = appFromOGVId;
            app = appFromOGVRepository.FindOne(appFromOGVId);
            appYear = app.RefYearDayUNV.ID / 10000;
            this.categoryShortName = categoryShortName;
            ReadUrl = "/FO41ResultIndicators/Read?appFromOGV={0}".FormatWith(appFromOGVId);
        }

        /// <summary>
        /// Признак, можно ли редактировать
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// Url получения данных
        /// </summary>
        public string ReadUrl { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateStore());

            var tabDetails = new Panel
            {
                ID = "EstimateTab",
                Title = @"Оценка",
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Closable = false,
                Items =
                        {
                            new BorderLayout
                                {
                                    North = { Items = { CreateHeaderPanel() } },
                                    Center = { Items = { CreateGrid(page) } }
                                }
                        }
            };

            return new List<Component> { tabDetails };
        }

        /// <summary>
        /// Добавляет столбец в грид
        /// </summary>
        /// <param name="gp">Грид с показателями оценки</param>
        /// <param name="columnId">Идентификатор столбца (имя в Store)</param>
        /// <param name="header">Заголовок столбца</param>
        private void AddColumn(GridPanel gp, string columnId, string header)
        {
            var column = gp.ColumnModel
                .AddColumn(columnId, header, DataAttributeTypes.dtString);
            if (Editable)
            {
                column.SetEditableDouble(2);
            }

            column.Sortable = false;
            column.Hideable = false;
            column.Renderer.Fn = RendererFn;
        }

        /// <summary>
        /// Создание текстового поля для отображения велечин прожиточного минимума
        /// </summary>
        /// <param name="period">название параметра (prev, fact, estimate или forecast)</param>
        /// <param name="label">заголовок текстового поля</param>
        /// <param name="value">значение (год)</param>
        /// <param name="yearForSource">Идентификатор источника</param>
        /// <returns>Текстовое поле</returns>
        private NumberField CreateTextField(string period, string label, decimal value, int yearForSource)
        {
            return new NumberField
            {
                ID = "{0}Norm".FormatWith(period),
                FieldLabel = label,
                LabelAlign = LabelAlign.Top,
                StyleSpec = "margin: 0px 10px 5px 0px; font-size: 12px;",
                Value = value,
                AllowDecimals = true,
                Disabled = !Editable,
                Listeners =
                {
                    Change =
                    {
                        Handler = @"
                                                    Ext.Ajax.request(
                                                    {{
                                                        url: '/FO41ResultIndicators/SaveNorm?year={0}',
                                                        params: {{
                                                            {1}: newValue
                                                        }}
                                                    }});".FormatWith(yearForSource, period)
                    }
                },
                Width = 65
            };
        }

        private Panel CreateHeaderPanel()
        {
            var calc = new Button
            {
                ID = "calcEstimate",
                Text = @"Рассчитать по исходным данным",
                Icon = Icon.Calculator,
                StyleSpec = StyleAll,
                Hidden = !Editable 
            };

            // сохранить обобщенную форму, рассчитать показатели оценки, обновить грид
            calc.Listeners.Click.Handler = @"
                Ext.Ajax.request(
                {{
                    url: '/FO41ResultIndicators/SaveResultData?appFromOGVId={0}',
                    success: function (response, options) {{
                        Ext.Ajax.request(
                        {{
                            url: '/FO41ResultIndicators/CalculateIndicators?appFromOGVId={0}',
                                success: function (response, options) {{
                                    EstimateStore_{0}.reload();
                                }}
                        }});
                    }}
                }});
                
                ".FormatWith(appFromOGVId);

            var toExcel = new Button
            {
                ID = "estimateToExcel",
                Text = @"Выгрузка в Excel",
                Icon = Icon.PageExcel,
                StyleSpec = StyleAll,
                Listeners =
                    {
                        Click =
                            {
                                Handler = @"
                                    Ext.net.DirectMethod.request({{
                                        url: '/FO41Export/ReportResult',
                                        isUpload: true,
                                        formProxyArg: 'RequestsEstimateFrom',
                                        cleanRequest: true,
                                        params: {{
                                            appFromOGVId: {0},
                                            sourceId: {1},
                                            name: '{2}'
                                        }},
                                        success:function (response, options) {{
                                        }},
                                        failure: function (response, options) {{
                                        }}
                                    }});".FormatWith(
                                            appFromOGVId, 
                                            extension.DataSource(appYear).ID, 
                                            "Итоговые покзатели_{0}".FormatWith(categoryShortName))
                            }
                    }
            };

            var mainPanel = Editable
                                ? new Toolbar
                                      {
                                          AutoHeight = true,
                                          Layout = "ColumnLayout",
                                          Items =
                                              {
                                                  calc,
                                                  toExcel
                                              }
                                      }

                                : new Toolbar
                                      {
                                          AutoHeight = true,
                                          Layout = "ColumnLayout",
                                          Items =
                                              {
                                                  toExcel
                                              }
                                      };

            var panelWage = new Panel
                                {
                                    Border = false,
                                    BodyCssClass = "x-window-mc",
                                    CssClass = "x-window-mc",
                                    Closable = false,
                                    Layout = "ColumnLayout",
                                    AutoHeight = true
                                };

            // получение значений прожиточного минимума
            var norm = normRepository.GetAll()
                .FirstOrDefault(f => f.Symbol.Equals("MIN") && f.Year == appYear);

            panelWage.Add(new Label("Введите величину прожиточного минимума")
                              {
                                  StyleSpec = "margin: 0px 30px 5px 10px; font-size: 12px;"
                              });
            var year = app.RefYearDayUNV.ID / 10000;
            panelWage.Add(CreateTextField("prev", (year - 1).ToString(), norm == null ? 0 : norm.PreviousFact, appYear));
            panelWage.Add(CreateTextField("fact", year.ToString(), norm == null ? 0 : norm.Fact, appYear));
            panelWage.Add(CreateTextField("estimate", (year + 1).ToString(), norm == null ? 0 : norm.Estimate, appYear));
            panelWage.Add(CreateTextField("forecast", (year + 2).ToString(), norm == null ? 0 : norm.Forecast, appYear));

            var panelHeader = new Panel
            {
                ID = "EstimateHeader",
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Closable = false,
                AutoHeight = true
            };

            panelHeader.Add(mainPanel);
            panelHeader.Add(panelWage);

            return panelHeader;
        }

        /// <summary>
        /// Создает грид с показателями оценки
        /// </summary>
        /// <param name="page">Родительская страница</param>
        /// <returns>Грид с показателями оценки</returns>
        private GridPanel CreateGrid(ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = "EstimateGrid",
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true },
                StoreID = "EstimateStore_{0}".FormatWith(appFromOGVId),
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
            gp.ColumnModel
                .AddColumn("OKEIName", "Единица измерения", DataAttributeTypes.dtString);

            AddColumn(gp, "PreviousFact", "{0}".FormatWith(appYear - 1));
            AddColumn(gp, "Fact", "{0}".FormatWith(appYear));
            AddColumn(gp, "Estimate", "{0}".FormatWith(appYear + 1));
            AddColumn(gp, "Forecast", "{0}".FormatWith(appYear + 2));

            gp.AutoExpandColumn = "RefName";
            gp.AddColumnsWrapStylesToPage(page);

            if (Editable)
            {
                gp.AddSaveButton();
            }

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
                             ID = "EstimateStore_{0}".FormatWith(appFromOGVId),
                             Restful = true,
                             ShowWarningOnFailure = false,
                             SkipIdForNewRecords = false,
                             RefreshAfterSaving = RefreshAfterSavingMode.None
                         };

            ds.SetRestController("FO41ResultIndicators")
                .SetJsonReader()
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
                .AddField("TempID");

            ds.Proxy.Proxy.RestAPI.ReadUrl = ReadUrl;
            const string UrlCreate = "/FO41ResultIndicators/SaveEstimate";
            ds.Proxy.Proxy.RestAPI.CreateUrl = UrlCreate;
            ds.Proxy.Proxy.RestAPI.UpdateUrl = UrlCreate;
            ds.AddScript(@"isValid = function(store) {
                var recCnt = store.getCount();
                for (var i = 0; i < recCnt; i++){
                    data = store.getAt(i);
                    if (data.get('Fact') == '' || data.get('Fact') == null ||
                        data.get('PreviousFact') == '' || data.get('PreviousFact') == null ||
                        data.get('Estimate') == '' || data.get('Estimate') == null ||
                        data.get('Forecast') == '' || data.get('Forecast') == null)
                    return false;
                }   
                return true;
            };");

            return ds;
        }        
    }
}
