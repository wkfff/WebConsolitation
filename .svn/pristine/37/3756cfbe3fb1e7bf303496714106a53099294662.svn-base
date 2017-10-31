using System;
using System.Data;
using System.Drawing;
using System.IO;
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
    public partial class FO_0035_0028 : CustomReportPage
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

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0028_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();

            lbDescription.Text = String.Format("Зарегистрированные бюджетные обязательства и остатки по лимитам бюджетных обязательств в разрезе ГРБС по состоянию на&nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>, тыс.руб.", date);
        }


        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0028_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            foreach (DataRow dataRow in dtGrid.Rows)
            {
                string grbsId = CustomParams.GetGrbsIdByName((dataRow[0].ToString()));

                if (grbsId == String.Empty)
                {
                    dataRow[1] = String.Empty;
                }
                else
                {
                    dataRow[1] =
                        String.Format(
                            "<div style='float: right'><a href='webcommand?showPinchReport=FO_0035_0030_grbs={0}'><img src='../../../images/detail.png'></a></div>", grbsId);
                }
            }

            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            UltraGridColumn col = e.Layout.Bands[0].Columns[1];

            e.Layout.Bands[0].Columns.RemoveAt(1);
            e.Layout.Bands[0].Columns.Insert(0, col);

            e.Layout.Bands[0].Columns[0].Width = 50;

            e.Layout.Bands[0].Columns[1].Width = 190;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[2].Width = 120;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");

            e.Layout.Bands[0].Columns[3].Width = 120;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            e.Layout.Bands[0].Columns[3].Header.Caption = String.Format("Поставлено на учет БО,<br/>в том числе {0:dd.MM.yyyy}", date);

            e.Layout.Bands[0].Columns[4].Width = 160;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            e.Layout.Bands[0].Columns[5].Width = 120;
            e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[7].Hidden = true;

            headerLayout.AddCell(" ");
            headerLayout.AddCell("ГРБС");            

            headerLayout.ApplyHeaderInfo();
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            DataTable dtChart = new DataTable();
            dtChart.Columns.Add(new DataColumn("1", typeof(double)));

            if (e.Row.Cells[6].Value != null)
            {
                e.Row.Cells[3].Value = String.Format("{0:N2}<br/>+{1:N2}", e.Row.Cells[3].Value, e.Row.Cells[6].Value);
            }
            if (e.Row.Cells[7].Value != null)
            {
                SetupChart();

                double value = Convert.ToDouble(e.Row.Cells[7].Value.ToString());
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

                e.Row.Cells[4].Style.BackgroundImage = String.Format("~/TemporaryImages/Chart_FO_0035_0028_{0}.png", value.ToString().Replace(',', '_').Replace('.', '_'));
                e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-left: 2px";

                e.Row.Cells[4].Value = String.Format("{0:N2}<br/>{1:P2}", e.Row.Cells[4].Value, e.Row.Cells[7].Value);
            }
            if (e.Row.Cells[1].Value.ToString() == "Всего")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
                e.Row.Cells[6].Style.Font.Size = 14;
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
