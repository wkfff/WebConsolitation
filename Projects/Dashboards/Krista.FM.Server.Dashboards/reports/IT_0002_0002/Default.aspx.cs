using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.IT_0002_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;

        private CustomParam filial;

        #endregion

        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        private static int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private static int MinScreenHeight
        {
            get { return CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            filial = CustomParam.CustomParamFactory("filial");

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid2.Width = Unit.Empty;

            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid2.Height = Unit.Empty;

            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            //UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraGridExporter1.Visible = false;
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboRegion.FillDictionaryValues(Regions);
                ComboRegion.Title = "Бюджет";
                ComboRegion.Width = 400;
                ComboRegion.SetСheckedState("Консолидированный", true);
            }
            filial.Value = regionsDictionary[ComboRegion.SelectedValue];

            Page.Title = String.Format("Доходы и расходы ({0})", ComboRegion.SelectedValue);
            PageTitle.Text = String.Format("Доходы и расходы ({0})", ComboRegion.SelectedValue);
            PageSubTitle.Text = "Исполнение бюджета по доходам и расходам за 1 полугодие 2010 года";

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();
        }


        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("IT_0002_0002_incomes"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("IT_0002_0002_outcomes"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150, 1280);

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }


            ColumnHeader ch = new ColumnHeader(true);
            string[] captions = e.Layout.Bands[0].Columns[1].Header.Caption.Split(';');
            ch.Caption = captions[0];
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 4;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            captions = e.Layout.Bands[0].Columns[5].Header.Caption.Split(';');
            ch.Caption = captions[0];
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 5;
            ch.RowLayoutColumnInfo.SpanX = 8;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, captions[1], GetColumnHeaderTooltip(i, captions[1]));
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(85, 1280);
            }

            //
            //e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            //e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(180, 1280);
            //e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(180, 1280);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[12], "P2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            e.Row.Cells[5].Style.BorderDetails.ColorLeft = Color.FromArgb(192, 192, 192);

            SetConditionBall(e, 4);
            SetConditionBall(e, 8);
            SetConditionArrow(e, 9);
            SetConditionArrow(e, 10);
            SetConditionArrow(e, 11);
        }

        private string GetColumnHeaderTooltip(int index, string caption)
        {
            switch (index)
            {
                case 1:
                    {
                        return "Первоначальный план";
                    }
                case 2:
                    {
                        return "Уточненный план";
                    }
                case 3:
                    {
                        return "Фактическое исполнение бюджета за аналогичный период предыдущего года";
                    }
                case 4:
                    {
                        return "Исполнение первоначального плана";
                    }
                case 5:
                    {
                        return "Первоначальный план";
                    }
                case 6:
                    {
                        return "Уточненный план";
                    }
                case 7:
                    {
                        return "Фактическое исполнение бюджета за аналогичный период предыдущего года";
                    }
                case 8:
                    {
                        return "Исполнение первоначального плана";
                    }
                case 9:
                    {
                        return "Фактический темп роста к аналогичному периоду предыдущего года";
                    }
                case 10:
                    {
                        return "Темп роста к аналогичному периоду предыдущего года по первоначальному плану";
                    }
                case 11:
                    {
                        return "Темп роста к аналогичному периоду предыдущего года по первоначальному плану";
                    }
                case 12:
                    {
                        return caption;
                    }
            }
            return String.Empty;
        }

        private static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (value > 1)
                {
                    img = "~/images/arrowGreenUpBB.png";

                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px";
                //   e.Row.Cells[3].Title = title;
            }
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
        }

        private static void SetConditionBall(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (value < 1)
                {
                    img = "~/images/BallRedBB.png";

                }
                else
                {
                    img = "~/images/BallGreenBB.png";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 5px center; padding-left: 2px";
                //   e.Row.Cells[3].Title = title;
            }
        }

        #endregion

        
        private static Dictionary<string, string> regionsDictionary;
        private static Dictionary<string, int> regions;

        public static Dictionary<string, int> Regions
        {
            get
            {
                // если словарь пустой
                if (regions == null || regions.Count == 0)
                {
                    // заполняем его
                    FillRegions();
                }
                return regions;
            }
        }

        private static void FillRegions()
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("IT_0002_0002_Regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            regionsDictionary = new Dictionary<string, string>();
            regions = new Dictionary<string, int>();

            foreach (DataRow row in dt.Rows)
            {
               
                regionsDictionary.Add(row[0].ToString(), row[1].ToString());
                regions.Add(row[0].ToString(), 0);
            }
        }


        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
            }

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            while (col.Hidden)
            {
                offset++;
                col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            }
            e.HeaderText = col.Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(PageSubTitle.Text);

            e.Section.AddPageBreak();
        }

        #endregion
    }
}
