using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Oil_0002_0001 : CustomReportPage
    {
        private DateTime startDate;
        private DateTime lastDate;
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0002_0001_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            startDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);
            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = dtDate.Rows[0][1].ToString();

            InitializeTable1();

            lbDescription.Text = String.Format("Розничные цены на нефтепродукты на&nbsp;<span class='DigitsValue'><b>{0:dd.MM.yyyy}</b></span>", currentDate);
        }

        private DataTable dt;

        private void InitializeTable1()
        {

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0002_0001_table1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 4; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                string gaugeValue = String.Format("{0:N2}", dt.Rows[i][1]);
                UltraGauge gauge = GetGauge(gaugeValue);
                PlaceHolder1.Controls.Add(gauge);
                row[0] = dt.Rows[i][0];
                row[1] = String.Format("<img src='../../../TemporaryImages/Oil_gauge{0}.png'>", gaugeValue);

                //row[0] = dt.Rows[i][0];
                //row[1] = String.Format("{0:N2}", dt.Rows[i][1]);

                double value = Convert.ToDouble(dt.Rows[i][2].ToString());
                string absoluteGrown = value > 0
                                           ? String.Format("+{0:N2}", value)
                                           : String.Format("-{0:N2}", Math.Abs(value));

                string img = String.Empty;
                if (value != 0)
                {
                    img = value > 0
                              ? "<img src='../../../images/arrowRedUpBB.png'>"
                              : "<img src='../../../images/arrowGreenDownBB.png'>";
                }

                row[2] = String.Format("{0}<br/>{2}&nbsp;{1:N2}%", absoluteGrown, dt.Rows[i][3], img);

                value = Convert.ToDouble(dt.Rows[i][4].ToString());
                absoluteGrown = value > 0
                                           ? String.Format("+{0:N2}", value)
                                           : String.Format("-{0:N2}", Math.Abs(value));

                if (value != 0)
                {
                    img = value > 0
                              ? "<img src='../../../images/arrowRedUpBB.png'>"
                              : "<img src='../../../images/arrowGreenDownBB.png'>";
                }

                row[3] = String.Format("{0}<br/>{2}&nbsp;{1:N2}%", absoluteGrown, dt.Rows[i][5], img);dtSource.Rows.Add(row);
            }


            UltraWebGrid1.DataSource = dtSource;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 3, 0, false);
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 5, 0, false);
        }

        private GridHeaderLayout headerLayout1;
        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);

            headerLayout1.AddCell("Товар");
            headerLayout1.AddCell(String.Format("Цена на {0:dd.MM.yy}, руб.", currentDate));

            headerLayout1.AddCell(String.Format("Динамика к {0:dd.MM.yy}", lastDate));
            headerLayout1.AddCell(String.Format("Динамика к {0:dd.MM.yy}", startDate));

            headerLayout1.ApplyHeaderInfo();

            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");

            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[3].Header.Style.Font.Size = FontUnit.Parse("14px");

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Width = 85;
            e.Layout.Bands[0].Columns[1].Width = 75;
            e.Layout.Bands[0].Columns[2].Width = 75;
            e.Layout.Bands[0].Columns[3].Width = 75;
        }

        #region Гейдж

        private UltraGauge GetGauge(string markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 70;
            gauge.Height = 35;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Oil_gauge{0}.png", markerValue);

            // Настраиваем гейдж
            SegmentedDigitalGauge SegmentedGauge = new SegmentedDigitalGauge();
            SegmentedGauge.BoundsMeasure = Measure.Percent;
            SegmentedGauge.Digits = 4;
            SegmentedGauge.InnerMarginString = "2, 2, 2, 2, Pixels";
            SegmentedGauge.MarginString = "2, 2, 2, 2, Pixels";
            SegmentedGauge.Text = markerValue.Replace(',', '.');

            SolidFillBrushElement brushUnlit = new SolidFillBrushElement();
            brushUnlit.Color = Color.FromArgb(10, 10, 10, 10);
            SegmentedGauge.UnlitBrushElements.Add(brushUnlit);

            SolidFillBrushElement brushFont = new SolidFillBrushElement();
            brushFont.Color = Color.PaleGoldenrod;
            SegmentedGauge.FontBrushElements.Add(brushFont);

            SolidFillBrushElement brush = new SolidFillBrushElement();
            brush.Color = Color.Black;
            SegmentedGauge.BrushElements.Add(brush);

            gauge.Gauges.Add(SegmentedGauge);
            return gauge;
        }

        #endregion
    }
}
