using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.EO_0001_0005
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private static MemberAttributesDigest programDigest;
        private static MemberAttributesDigest regionDigest;
        private static MemberAttributesDigest customerDigest;
        private static MemberAttributesDigest statusDigest;

        #endregion

        #region Параметры запроса

        // выбранная программа
        private CustomParam selectedProgram;
        // выбранный МО
        private CustomParam selectedRegion;
        // выбранный заказчик
        private CustomParam selectedCustomer;
        // выбранный статус
        private CustomParam selectedStatus;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            //GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight - 230);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow +=new InitializeRowEventHandler(Grid_InitializeRow);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);

            #endregion

            #region Инициализация параметров запроса

            selectedProgram = UserParams.CustomParam("selected_program");
            selectedRegion = UserParams.CustomParam("selected_region");
            selectedCustomer = UserParams.CustomParam("selected_customer");
            selectedStatus = UserParams.CustomParam("selected_status");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected void Grid_DataBound(object sender, EventArgs e)
        {
            int width = 0;
            foreach (UltraGridColumn column in GridBrick.Grid.Columns)
            {
                width += Convert.ToInt32(column.Width.Value);
            }

            GridBrick.Grid.Width = width + 50;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboProgram.Title = "Целевая программа";
                ComboProgram.Width = 300;
                ComboProgram.MultiSelect = false;
                ComboProgram.ParentSelect = true;
                ComboProgram.TooltipVisibility = TooltipVisibilityMode.Shown;
                programDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "EO_0001_0005_programDigest");
                ComboProgram.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(programDigest.UniqueNames, programDigest.MemberLevels));
                ComboProgram.SetСheckedState("Все программы", true);

                ComboRegion.Title = "Территория";
                ComboRegion.Width = 250;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                regionDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "EO_0001_0005_regionDigest");
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(regionDigest.UniqueNames, regionDigest.MemberLevels));
                ComboRegion.SetСheckedState("Все территории", true);

                ComboCustomer.Title = "Заказчик";
                ComboCustomer.Width = 250;
                ComboCustomer.MultiSelect = false;
                ComboCustomer.ParentSelect = true;
                customerDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "EO_0001_0005_customerDigest");
                ComboCustomer.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(customerDigest.UniqueNames, customerDigest.MemberLevels));
                ComboCustomer.SetСheckedState("Все заказчики", true);

                ComboStatus.Title = "Статус";
                ComboStatus.Width = 250;
                ComboStatus.MultiSelect = false;
                ComboStatus.ParentSelect = true;
                statusDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "EO_0001_0005_statusDigest");
                ComboStatus.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(statusDigest.UniqueNames, statusDigest.MemberLevels));
                ComboStatus.SetСheckedState("Все статусы", true);
            }
 
            Page.Title = "Реестр объектов капитального строительства в Ханты-Мансийском автономном округе - Югре";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Перечень объектов государственной и муниципальной собственности ({0}, {1}, {2}, {3})",
                ComboProgram.SelectedValue, ComboRegion.SelectedValue, ComboCustomer.SelectedValue, ComboStatus.SelectedValue);

            selectedProgram.Value = programDigest.GetMemberUniqueName(ComboProgram.SelectedValue);
            selectedRegion.Value = regionDigest.GetMemberUniqueName(ComboRegion.SelectedValue);
            selectedCustomer.Value = customerDigest.GetMemberUniqueName(ComboCustomer.SelectedValue);
            selectedStatus.Value = statusDigest.GetMemberUniqueName(ComboStatus.SelectedValue);

            GridDataBind();
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0001_0005_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатели", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[2].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[3].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[4].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(220);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(310);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(160);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(220);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(220);

            GridBrick.GridHeaderLayout.AddCell("Объекты строительства");
            GridBrick.GridHeaderLayout.AddCell("Общая информация");
            GridBrick.GridHeaderLayout.AddCell("Стоимость объектов в текущих ценах, тыс.руб.");
            GridBrick.GridHeaderLayout.AddCell("Профинансировано с начала строительства, тыс.руб.");
            GridBrick.GridHeaderLayout.AddCell("Освоено с начала строительства, тыс.руб.");
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool infoColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Общая информация");
                bool valueColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Стоимость");
                bool financeColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Профинансировано");
                bool completeColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Освоено");

                if (infoColumn)
                {
                    string[] valueParts = e.Row.Cells[i].Value.ToString().Split(';');
                    if (valueParts.Length > 4)
                    {
                        e.Row.Cells[i].Value = String.Format(@"
                            <b> - целевая программа</b>: {0}<br/>
                            <b> - территория</b>: {1}<br/>
                            <b> - заказчик</b>: {2}<br/>
                            <b> - сроки строительства</b>: {3}<br/>
                            <b> - статус</b>: {4}<br/>", valueParts[0], valueParts[1], valueParts[2], GetYearInterval(valueParts[3]), valueParts[4]);
                    }
                }
                if (valueColumn && e.Row.Cells[i].Value != null)
                {
                    double value = GetDoubleValue(e.Row.Cells[i].Value.ToString());
                    e.Row.Cells[i].Value = String.Format("<span style=\"font-size: 14px;\">{0:N2}</span>", value);
                }
                if (financeColumn && e.Row.Cells[i].Value != null)
                {
                    string[] valueParts = e.Row.Cells[i].Value.ToString().Split(';');
                    if (valueParts.Length > 1)
                    {
                        double value1 = GetDoubleValue(valueParts[0]);
                        double value2 = GetDoubleValue(valueParts[1]);

                        e.Row.Cells[i].Value = String.Format("<span style=\"font-size: 14px;\">{0:N2}</span><br/>{1}", value1, GetGaugeTable(value2, "от стоимости объекта в текущих ценах"));
                    }
                }
                if (completeColumn && e.Row.Cells[i].Value != null)
                {
                    string[] valueParts = e.Row.Cells[i].Value.ToString().Split(';');
                    if (valueParts.Length > 2)
                    {
                        double value1 = GetDoubleValue(valueParts[0]);
                        double value2 = GetDoubleValue(valueParts[1]);
                        double value3 = GetDoubleValue(valueParts[2]);

                        e.Row.Cells[i].Value = String.Format("<span style=\"font-size: 14px;\">{0:N2}</span><br/>{1}<br/>{2}", 
                            value1, GetGaugeTable(value2, "от стоимости объекта в текущих ценах"), GetGaugeTable(value3, "от финансирования"));
                    }
                }
            }
        }

        private static string GetYearInterval(string dateStr)
        {
            string[] strParts = dateStr.Split('-');
            if (strParts.Length > 1)
            {
                DateTime beginDate = DateTime.Parse(strParts[0]);
                DateTime endDate = DateTime.Parse(strParts[1]);

                return String.Format("{0} - {1}", beginDate.Year, endDate.Year);
            }
            return dateStr;
        }

        private string GetGaugeTable(double value, string caption)
        {
            if (value != Double.MinValue)
            {
                return String.Format("<span style=\"font-size: 12px;\"><b>{0}</b><br/>{1:P2}&nbsp;{2}</span>", GetGaugeUrl(100 * value), value, caption);
            }
            return String.Empty;
        }

        protected string GetGaugeUrl(object oValue)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue);
            if (value > 100)
                value = 100;
            string path = "Prog_0001_0001_gauge" + Convert.ToInt32(value) + ".png";
            string returnPath = String.Format("<img style=\"FLOAT: center;\" src=\"../../TemporaryImages/{0}\"/>", path);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
                return returnPath;
            LinearGaugeScale scale = ((LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (value > 90)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else
            {
                if (value < 50)
                {

                    BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                    BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                    BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
                }
                else
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
                }
            }

            Size size = new Size(100, 40);
            Gauge.SaveTo(Server.MapPath(serverPath), GaugeImageType.Png, size);
            return returnPath;
        }

        private Double GetDoubleValue(object strValue)
        {
            if (strValue != null)
            {

                decimal value;
                if (Decimal.TryParse(strValue.ToString(), out value))
                {
                    return Convert.ToDouble(value);
                }
            }

            return Double.MinValue;
        }

        #endregion
        
        #region Экспорт в Excel

        private void RemoveTags(UltraWebGrid grid)
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    if (row.Cells[i].Value != null)
                    {
                        string cellValue = row.Cells[i].Value.ToString();

                        cellValue = cellValue.Replace("&nbsp;", " ");
                        cellValue = Regex.Replace(cellValue, "<br[^>]*?>", Environment.NewLine);
                        cellValue = Regex.Replace(cellValue, "<[^>]*?>", String.Empty);

                        row.Cells[i].Value = cellValue;
                    }
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            RemoveTags(GridBrick.Grid);
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            RemoveTags(GridBrick.Grid);
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout);
        }

        #endregion
    }
}