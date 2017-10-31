using System;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controls
{
    public class MarkDetails : GridPanel
    {
        private readonly int markId;
        private readonly int categoryId;
        private readonly int periodId;
        private readonly int applicOGVId;
        private readonly int okeiCode;

        private readonly IFO41Extension extension;
        private readonly string markName;
        private readonly bool showEstimateAndForecast;
        private const int PercentCode = 744;

        private const string RendererFn = @"function (v, p, r) {{
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

        private const string RendererPartFn = @"function (v, p, r) {{
                    p.css = 'gray-cell';
                    var okei = r.data.OKEI;
                    var f;
                    f = Ext.util.Format.numberRenderer('0.000,00/i');
                    return f(v);
                }}";

        public MarkDetails(IFO41Extension extension, string markName, int markId, int applicOGVId, int okeiCode)
        {
            this.extension = extension;
            this.markName = markName;
            this.markId = markId;
            this.applicOGVId = applicOGVId;
            this.okeiCode = okeiCode;
            if (extension.OKTMO == FO41Extension.OKTMOHMAO)
            {
                this.applicOGVId = 0;
                categoryId = 0;
                periodId = 0;
                showEstimateAndForecast = false;
            }
            else
            {
                showEstimateAndForecast = true;
            }
        }

        public MarkDetails(IFO41Extension extension, string markName, int markId, int categoryId, int periodId, int okeiCode)
        {
            this.extension = extension;
            this.markName = markName;
            this.markId = markId;

            this.categoryId = categoryId;
            this.periodId = periodId;
            this.okeiCode = okeiCode;

            if (extension.OKTMO == FO41Extension.OKTMOHMAO)
            {
                applicOGVId = 0;
                showEstimateAndForecast = false;
            }
            else
            {
                showEstimateAndForecast = true;
            }
        }

        public void InitAll(ViewPage page)
        {
            ResourceManager.GetInstance(page).RegisterClientStyleBlock(
                "CustomStyle",
                ".gray-cell{background-color: #DCDCDC !important; border-right-color: #FFFFFF; !important;}");

            var ds = CreateStore();
            page.Controls.Add(ds);

            ID = "MarkDetails_{0}_{1}".FormatWith(markId, applicOGVId);
            Title = markName;
            Border = false;
            BodyCssClass = "x-window-mc";
            CssClass = "x-window-mc";
            Closable = false;
            StoreID = ds.ID;

            LoadMask.ShowMask = true;
            SaveMask.ShowMask = true;

            ID = "MDGrid_{0}_{1}".FormatWith(markId, applicOGVId);
            EnableColumnMove = false;
            ColumnLines = true;
            AutoExpandColumn = "TaxPayerName";

            var showPartColumn = okeiCode != PercentCode;

            var groupRow = new HeaderGroupRow();
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 2 });

            var groupsColSpan = showPartColumn ? 2 : 1;

            var year = DateTime.Today.Year;
            groupRow.Columns.Add(new HeaderGroupColumn
                                     {
                                         Header = "{0} (факт)".FormatWith(year - 2),
                                         ColSpan = groupsColSpan, 
                                         Align = Alignment.Center
                                     });
            groupRow.Columns.Add(new HeaderGroupColumn
                                     {
                                         Header = "{0} (факт)".FormatWith(year - 1),
                                         ColSpan = groupsColSpan, 
                                         Align = Alignment.Center
                                     });
            if (showEstimateAndForecast)
            {
                groupRow.Columns.Add(new HeaderGroupColumn
                                         {
                                             Header = "{0} (оценка)".FormatWith(year),
                                             ColSpan = groupsColSpan,
                                             Align = Alignment.Center
                                         });
                groupRow.Columns.Add(new HeaderGroupColumn
                                         {
                                             Header = "{0} (прогноз)".FormatWith(year + 1),
                                             ColSpan = groupsColSpan,
                                             Align = Alignment.Center
                                         });
            }

            View.Add(new GridView { HeaderGroupRows = { groupRow } });

            AddColumn("RefNumberString", "№ п/п").SetWidth(25);

            AddColumn("TaxPayerName", "Налогоплательщик").SetWidth(200);
            
            AddDoubleColumn("PreviousFactValue", "Значение").SetWidth(80).Renderer.Fn = RendererFn;
            if (showPartColumn)
            {
                AddDoubleColumn("PreviousFactPart", "Доля").SetWidth(50).Renderer.Fn = RendererPartFn;
            }

            AddDoubleColumn("FactValue", "Значение").SetWidth(80).Renderer.Fn = RendererFn;
            if (showPartColumn)
            {
                AddDoubleColumn("FactPart", "Доля").SetWidth(50).Renderer.Fn = RendererPartFn;
            }

            if (showEstimateAndForecast)
            {
                AddDoubleColumn("EstimateValue", "Значение").SetWidth(80).Renderer.Fn = RendererFn;
                if (showPartColumn)
                {
                    AddDoubleColumn("EstimatePart", "Доля").SetWidth(50).Renderer.Fn = RendererPartFn;
                }

                AddDoubleColumn("ForecastValue", "Значение").SetWidth(80).Renderer.Fn = RendererFn;
                if (showPartColumn)
                {
                    AddDoubleColumn("ForecastPart", "Доля").SetWidth(50).Renderer.Fn = RendererPartFn;
                }
            }

            this.AddColumnsWrapStylesToPage(page);
        }

        /// <summary>
        /// Добавляет столбец в грид
        /// </summary>
        /// <param name="columnId">Идентификатор столбца (имя в Store)</param>
        /// <param name="header">Заголовок столбца</param>
        private ColumnBase AddColumn(string columnId, string header)
        {
            var column = ColumnModel.AddColumn(columnId, header, DataAttributeTypes.dtString);

            column.Sortable = false;
            column.Hideable = false;

            return column;
        }

        /// <summary>
        /// Добавляет столбец в грид (для значений показателей)
        /// </summary>
        /// <param name="columnId">Идентификатор столбца (имя в Store)</param>
        /// <param name="header">Заголовок столбца</param>
        private ColumnBase AddDoubleColumn(string columnId, string header)
        {
            var column = ColumnModel.AddColumn(columnId, header, DataAttributeTypes.dtString);

            column.Sortable = false;
            column.Hideable = false;

            column.Renderer.Fn = RendererFn;

            // column.SetEditableDouble(2);
            return column;
        }

        private Store CreateStore()
        {
            var ds = new Store
            {
                ID = "Store_{0}_{1}".FormatWith(markId, applicOGVId),
                Restful = false,
                AutoDestroy = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };

            if (extension.OKTMO == FO41Extension.OKTMOHMAO)
            {
                ds.SetHttpProxy("/FO41HMAOMarkDetails/Read");
                ds.BaseParams.Add(new Parameter("categoryId", categoryId.ToString()));
                ds.BaseParams.Add(new Parameter("periodId", periodId.ToString()));
            }
            else
            {
                ds.SetHttpProxy("/FO41MarkDetails/Read");
                ds.BaseParams.Add(new Parameter("applicOGVId", applicOGVId.ToString()));
            }

            ds.BaseParams.Add(new Parameter("markId", markId.ToString()));

            ds.SetJsonReader()
                .AddField("RefNumberString")
                .AddField("PreviousFactValue")
                .AddField("PreviousFactPart")
                .AddField("FactValue")
                .AddField("FactPart")
                .AddField("RefName")
                .AddField("OKEI")
                .AddField("MarkName")
                .AddField("TaxPayerName")
                .AddField("TempID");
            if (showEstimateAndForecast)
            {
                ds.AddField("EstimateValue")
                    .AddField("EstimatePart")
                    .AddField("ForecastValue")
                    .AddField("ForecastPart");
            }

            return ds;
        }
    }
}
