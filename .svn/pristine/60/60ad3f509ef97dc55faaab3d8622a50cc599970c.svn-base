using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0030 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {           
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            CustomParams.MakeGrbsParams("122", "id");

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0030_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();

            Label1.Text = String.Format("<b><span class='ImportantText'>{0}</span></b>", UserParams.Grbs.Value);
            lbDescription.Text = String.Format("Данные по состоянию на &nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>, тыс.руб.", date);

            DataTable dtPerson = new DataTable();
            query = DataProvider.GetQueryText("person");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "name", dtPerson);

            HyperLinkSite.NavigateUrl = dtPerson.Rows[0][2].ToString();
            HyperLinkSite.Text = dtPerson.Rows[0][2].ToString();
            lbFIO.Text = dtPerson.Rows[0][3].ToString();
            lbPhone.Text = dtPerson.Rows[0][4].ToString();
            HyperLinkMail.NavigateUrl = "mailto:" + dtPerson.Rows[0][5];
            HyperLinkMail.Text = dtPerson.Rows[0][5].ToString();

            Image1.ImageUrl = String.Format("../../../images/novosibPhotos/{0}.jpg", HttpContext.Current.Session["CurrentGrbsID"]);
        }
        

        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0030_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;
           
            e.Layout.Bands[0].Columns[0].Width = 240;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 120;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 120;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Поставлено на учет БО,<br/>в том числе {0:dd.MM.yyyy}", date);

            e.Layout.Bands[0].Columns[3].Width = 160;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            e.Layout.Bands[0].Columns[4].Width = 120;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[7].Hidden = true;
            
            headerLayout.AddCell("КОСГУ");
            
            headerLayout.ApplyHeaderInfo();
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[5].Value != null)
            {
                e.Row.Cells[2].Value = String.Format("{0:N2}<br/>+{1:N2}", e.Row.Cells[2].Value, e.Row.Cells[5].Value);
            }
            if (e.Row.Cells[6].Value != null)
            {
                SetupChart();
                DataTable dtChart = new DataTable();
                dtChart.Columns.Add(new DataColumn("1", typeof(double)));

                double value = Convert.ToDouble(e.Row.Cells[6].Value.ToString());
                DataRow row = dtChart.NewRow();
                row[0] = value;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 0;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 0;
                dtChart.Rows.Add(row);

                row = dtChart.NewRow();
                row[0] = 1 - value;
                dtChart.Rows.Add(row);

                UltraChart1.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Chart_FO_0035_0028_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));

                UltraChart1.DataSource = dtChart;
                UltraChart1.DataBind();

                StringWriter writer = new StringWriter();
                HtmlTextWriter output = new HtmlTextWriter(writer);

                UltraChart1.RenderControl(output);

                e.Row.Cells[3].Style.BackgroundImage = String.Format("~/TemporaryImages/Chart_FO_0035_0028_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-left: 2px";
                e.Row.Cells[3].Value = String.Format("{0:N2}<br/>{1:P2}", e.Row.Cells[3].Value, e.Row.Cells[6].Value);
            }
            if (e.Row.Cells[0].Value.ToString() == "Всего")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
                e.Row.Cells[5].Style.Font.Size = 14;
            }
            if (e.Row.Cells[0].Value.ToString() == "Всего")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
                e.Row.Cells[5].Style.Font.Size = 14;
            }
            else if (e.Row.Cells[7].Value.ToString() == "Подстатья")
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.ForeColor = Color.FromArgb(192, 192, 192);
                }
            }
        }

        #endregion

        private void SetupChart()
        {
            UltraChart1.Width = 35;
            UltraChart1.Height = 35;

            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.PieChart.OthersCategoryText = "Прочие";
            UltraChart1.PieChart.Labels.Visible = false;
            UltraChart1.PieChart.Labels.LeaderLinesVisible = false;
            UltraChart1.PieChart.Labels.FontColor = Color.White;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";

            UltraChart1.Legend.Visible = false;
            UltraChart1.PieChart.StartAngle = 270;
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.ForestGreen;
                    }
                case 2:
                    {
                        return Color.Red;
                    }
                default:
                    {
                        return Color.White;
                    }
            }
        }
    }
}
