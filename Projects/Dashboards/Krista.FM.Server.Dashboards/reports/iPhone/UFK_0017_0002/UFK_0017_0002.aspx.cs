using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class UFK_0017_0002 : CustomReportPage
    {
        private double income = 1486112;
        private double outcome = 0;
        private double borrow = 0;
        private double otherFS = 0;
        private double rest;

        private double maxValue;
        private double minValue = 0;
        private double avgValue;
        private double currentValue;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string query = DataProvider.GetQueryText("UFK_0017_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            lbDate.Text =
            string.Format("{0} {1}", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            lbRestDate.Text =
                string.Format("�� ����� {0} �������",
                              CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));

            UserParams.PeriodDayFO.Value = string.Format("{0}", dtDate.Rows[0][5]);
            UserParams.PeriodYear.Value =
                string.Format("[������].[����].[������ ���� ��������].[{0}]", dtDate.Rows[0][0]);

            UserParams.PeriodMonth.Value = string.Format("[������].[����].[������ ���� ��������].[{0}].[{1}].[{2}].[{3}]",
                dtDate.Rows[0][0], dtDate.Rows[0][1], dtDate.Rows[0][2], dtDate.Rows[0][3]);

            query = DataProvider.GetQueryText("UFK_0017_0001_stats");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtStats);

            query = DataProvider.GetQueryText("UFK_0017_0001");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtGaudes);

            query = DataProvider.GetQueryText("UFK_0017_0002");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtRests);

            //            Double.TryParse(dtRests.Rows[0][1].ToString(), out income);
            //Double.TryParse(dtRests.Rows[0][0].ToString(), out outcome);
            currentValue = Convert.ToDouble(dtGaudes.Rows[0][3]) / 1000;

            maxValue = Convert.ToDouble(dtStats.Rows[0][6]) / 1000;
            avgValue = Convert.ToDouble(dtStats.Rows[0][8]) / 1000;
            minValue = Convert.ToDouble(dtStats.Rows[0][9]) / 1000;

            rest = currentValue + income + borrow + otherFS - outcome;

            SetUpStatLables();
            SetUpGaudes((LinearGauge)ugTarget.Gauges[0]);
        }

        private DataTable dtStats = new DataTable();
        private DataTable dtGaudes = new DataTable();
        private DataTable dtDate = new DataTable();
        private DataTable dtRests = new DataTable();

        private void SetUpGaudes(LinearGauge gaude)
        {
            gaude.Scales[0].Markers[0].Value = currentValue / 1000;
            double topValue = Math.Max(currentValue / 1000, maxValue / 1000);
            gaude.Scales[0].Axes[0].SetEndValue(topValue);
            gaude.Scales[0].Labels.Frequency = topValue / 2;

            LinearGaugeNeedle needle = new LinearGaugeNeedle();
            needle.StartExtent = 2;
            needle.MidExtent = 5;
            needle.EndExtent = 9;
            needle.StartWidth = 9;
            needle.MidWidth = 9;
            needle.EndWidth = 0;
            needle.Value = avgValue / 1000;
            SolidFillBrushElement fill = new SolidFillBrushElement();
            fill.Color = Color.Gray;
            //            fill.Color = Color.White;
            needle.BrushElements.Add(fill);

            gaude.Scales[0].Markers.Add(needle);

            needle = new LinearGaugeNeedle();
            needle.StartExtent = 2;
            needle.MidExtent = 5;
            needle.EndExtent = 9;
            needle.StartWidth = 9;
            needle.MidWidth = 9;
            needle.EndWidth = 0;
            needle.Value = minValue / 1000;
            fill = new SolidFillBrushElement();
            fill.Color = Color.Gray;
            //            fill.Color = Color.White;
            needle.BrushElements.Add(fill);

            gaude.Scales[0].Markers.Add(needle);
//            if (rest > 0)
//            {
//                needle = new LinearGaugeNeedle();
//                needle.StartExtent = 10;
//                needle.MidExtent = 30;
//                needle.EndExtent = 50;
//                needle.StartWidth = 2;
//                needle.EndWidth = 2;
//                needle.MidWidth = 2;
//                needle.Value = rest / 1000;
//                HatchBrushElement hatch = new HatchBrushElement();
//                hatch.HatchStyle = HatchStyle.DarkVertical;
//                hatch.ForeColor = Color.Gray;
//                //                hatch.ForeColor = Color.White;
//                needle.BrushElements.Add(hatch);
//                gaude.Scales[0].Markers.Add(needle);
//            }

            BoxAnnotation ba = new BoxAnnotation();
            ba.Bounds = new Rectangle(5, (int)gaude.Scales[0].Axes[0].Map(avgValue / 1000, 135, 11), 15, 10);
            ba.Label.FormatString = "����";
            ba.Label.Font = new Font("Arial", 8);
            ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.Gray));
            //            ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.White));
            gaude.GaugeComponent.Annotations.Add(ba);

            ba = new BoxAnnotation();
            ba.Bounds = new Rectangle(7, (int)gaude.Scales[0].Axes[0].Map(minValue / 1000, 135, 11), 15, 10);
            ba.Label.FormatString = "���";
            ba.Label.Font = new Font("Arial", 8);
            ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.Gray));
            //            ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.White));
            gaude.GaugeComponent.Annotations.Add(ba);
        }

        private void SetUpStatLables()
        {
            string[] dateStat = dtStats.Rows[0][5].ToString().Split('[');
            lbNoTargetMaxDate.Text = string.Format("{0:dd.MM.yyyy}",
                new DateTime(Convert.ToInt32(dateStat[4].Trim('.').Trim(']')),
                Convert.ToInt32(CRHelper.MonthNum(dateStat[7].Trim('.').Trim(']'))),
                Convert.ToInt32(dateStat[8].Trim(']'))));
            lbNoTargetMax.Text = string.Format("{0:N0}", maxValue);

            lbNoTargetAvg.Text = string.Format("{0:N0}", avgValue);

            dateStat = dtStats.Rows[0][7].ToString().Split('[');
            lbNoTargetMinDate.Text = string.Format("{0:dd.MM.yyyy}",
                new DateTime(Convert.ToInt32(dateStat[4].Trim('.').Trim(']')),
                Convert.ToInt32(CRHelper.MonthNum(dateStat[7].Trim('.').Trim(']'))),
                Convert.ToInt32(dateStat[8].Trim(']'))));
            lbNoTargetMin.Text = string.Format("{0:N0}", minValue);

            double offset = (Convert.ToDouble(dtGaudes.Rows[0][4]) / 1000);
            double offsetPercents = Convert.ToDouble(dtGaudes.Rows[0][5]);

            lbNoTargetSum.Text = string.Format("{0}", (Convert.ToDouble(dtGaudes.Rows[0][3]) / 1000).ToString("N0"));

            lbNotargetOffset.Text = offset > 0 ? string.Format("+{0:N0}", offset) : string.Format("{0:N0}", offset);
            lbNotargetOffsetPercents.Text = offsetPercents > 0
                                                ? string.Format("+{0:P2}", offsetPercents)
                                                : string.Format("{0:P2}", offsetPercents);
            if (Convert.ToDouble(dtGaudes.Rows[0][4]) > 0)
                imageNoTarget.ImageUrl = "~/images/ArrowUpGreen.png";
            else
                imageNoTarget.ImageUrl = "~/images/ArrowDownRed.png";

           foreach(DataRow row in dtRests.Rows)
           {
               TableRow tableRow = new TableRow();

               TableCell cell = new TableCell();

               Label lb = new Label();
               lb.CssClass = "InformationText";
               lb.Text = row[0].ToString();
               cell.Controls.Add(lb);

               tableRow.Cells.Add(cell);

               cell = new TableCell();
               lb = new Label();
               lb.CssClass = "TableFont";
               lb.Text = String.Format("{0:N2}", row[1]);
               cell.Controls.Add(lb);
               cell.HorizontalAlign = HorizontalAlign.Right;
               tableRow.Cells.Add(cell);
               tableStat.Rows.Add(tableRow);
           }
        }
    }
}