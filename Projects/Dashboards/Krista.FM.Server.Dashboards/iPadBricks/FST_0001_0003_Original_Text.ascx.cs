using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.IO;
using System.Drawing.Imaging;
using System.Web;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FST_0001_0003_Original_Text : UserControl
    {
        private DataTable dtChart;
        public DataTable DtChart
        {
            get
            {
                return dtChart;
            }
            set
            {
                dtChart = value;
            }
        }

        private string taxName = "среднеотпускной тариф";

        public string TaxName
        {
            get
            {
                return taxName;
            }
            set
            {
                taxName = value;
            }
        }

        private DataTable dtText;
        public DataTable DtText
        {
            get
            {
                return dtText;
            }
            set
            {
                dtText = value;
            }
        }

        int currentYear = 2011;

        private int GetBoundX(string stateArea)
        {
            if (stateArea.Length < 19)
            {
                return 61;
            }
            if (stateArea.Length < 22)
            {
                return 58;
            }
            if (stateArea.Length < 25)
            {
                return 57;
            }
            if (stateArea.Length < 27)
            {
                return 55;
            }
            if (stateArea.Length < 32)
            {
                return 53;
            }
            if (stateArea.Length < 34)
            {
                return 50;
            }
            return 58;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RadialGauge gauge = (RadialGauge)UltraGauge2.Gauges[0];
            RadialGaugeScale scale = (RadialGaugeScale)gauge.Scales[0];

            Infragistics.UltraGauge.Resources.BoxAnnotation annotation = (Infragistics.UltraGauge.Resources.BoxAnnotation)UltraGauge2.Annotations[7];
            annotation.Label.FormatString = CRHelper.ToUpperFirstSymbol(taxName);
            annotation.Bounds = new Rectangle(GetBoundX(taxName),
                annotation.Bounds.Y,
                annotation.Bounds.Width,
                annotation.Bounds.Height);

            double summ = 0;
            double summGrown = 0;
            int nonZeroGrown = 0;

            double summGrownDemos = 0;
            int nonZeroGrownDemos = 0;

            scale.Markers[0].Value = Convert.ToDouble(dtChart.Rows[0][1]);

            string currentState = ((CustomParam)(HttpContext.Current.Session["state_area"])).Value;

            scale.Ranges[0].StartValue = Convert.ToDouble(dtChart.Rows[0][1]);
            for (int i = 0; i < 5; i++)
            {
                summ += Convert.ToDouble(dtChart.Rows[i][1]);
                if (Convert.ToDouble(dtChart.Rows[i][2]) != -1)
                {
                    summGrown += Convert.ToDouble(dtChart.Rows[i][2]);
                    nonZeroGrown++;
                }
                if (Convert.ToDouble(dtChart.Rows[i][3]) != -1)
                {
                    summGrownDemos += Convert.ToDouble(dtChart.Rows[i][3]);
                    nonZeroGrownDemos++;
                }
                if (currentState == dtChart.Rows[i][0].ToString())
                {
                    scale.Markers[0].Value = Convert.ToDouble(dtChart.Rows[i][1]);
                }
            }

            scale.Ranges[0].EndValue = Convert.ToDouble(dtChart.Rows[4][1]);
            scale.Ranges[1].StartValue = Convert.ToDouble(dtChart.Rows[4][1]);

            for (int i = 5; i < dtChart.Rows.Count - 5; i++)
            {
                summ += Convert.ToDouble(dtChart.Rows[i][1]);
                if (Convert.ToDouble(dtChart.Rows[i][2]) != -1)
                {
                    summGrown += Convert.ToDouble(dtChart.Rows[i][2]);
                    nonZeroGrown++;
                }
                if (Convert.ToDouble(dtChart.Rows[i][3]) != -1)
                {
                    summGrownDemos += Convert.ToDouble(dtChart.Rows[i][3]);
                    nonZeroGrownDemos++;
                }
                if (currentState == dtChart.Rows[i][0].ToString())
                {
                    scale.Markers[0].Value = Convert.ToDouble(dtChart.Rows[i][1]);
                }
            }

            scale.Ranges[1].EndValue = Convert.ToDouble(dtChart.Rows[dtChart.Rows.Count - 5][1]);
            scale.Ranges[2].StartValue = Convert.ToDouble(dtChart.Rows[dtChart.Rows.Count - 5][1]);

            for (int i = dtChart.Rows.Count - 5; i < dtChart.Rows.Count; i++)
            {
                summ += Convert.ToDouble(dtChart.Rows[i][1]);
                if (Convert.ToDouble(dtChart.Rows[i][2]) != -1)
                {
                    summGrown += Convert.ToDouble(dtChart.Rows[i][2]);
                    nonZeroGrown++;
                }
                if (Convert.ToDouble(dtChart.Rows[i][3]) != -1)
                {
                    summGrownDemos += Convert.ToDouble(dtChart.Rows[i][3]);
                    nonZeroGrownDemos++;
                }
                if (currentState == dtChart.Rows[i][0].ToString())
                {
                    scale.Markers[0].Value = Convert.ToDouble(dtChart.Rows[i][1]);
                }
            }

            scale.Ranges[2].EndValue = Convert.ToDouble(dtChart.Rows[dtChart.Rows.Count - 1][1]);

            scale.Axis.SetTickmarkInterval((Convert.ToDouble(dtChart.Rows[dtChart.Rows.Count - 1][1]) - Convert.ToDouble(dtChart.Rows[0][1])) / 10);
            scale.MinorTickmarks.Frequency = 1;
            scale.Labels.Frequency = 2.5;
            scale.Axis.SetStartValue(Convert.ToDouble(dtChart.Rows[0][1]));
            scale.Axis.SetEndValue(Convert.ToDouble(dtChart.Rows[dtChart.Rows.Count - 1][1]));

            scale.Markers[0].Precision = 0.1;

            UltraGauge2.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/{0}_{1}_{2}.png", scale.Ranges[1].StartValue, scale.Ranges[2].StartValue, scale.Markers[0].Value);

            double avgRfGrown = (summGrown / nonZeroGrown);
            double avgRfGrownDemos = (summGrownDemos / nonZeroGrownDemos);

            DataRow row = dtText.Rows[0];

            double currentGrown = row[2] != DBNull.Value ? Convert.ToDouble(row[2]) : 0;
            double currentGrownDemos = row[4] != DBNull.Value ? Convert.ToDouble(row[4]) : 0;

            //string rfMiddleLevelDescription = currentGrown > avgRfGrown ? "&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;выше" : "&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;ниже";
            string rfMiddleLevelDescription = currentGrown > avgRfGrown ? "выше" : "ниже";
            string rfMiddleLevelDescriptionDemos = currentGrownDemos > avgRfGrownDemos ? "выше" : "ниже";



            if ((row[1] == DBNull.Value) &&
                (row[3] == DBNull.Value))
            {
                Label1.Text = "Данные уточняются";
                UltraGauge2.Visible = false;
            }
            else
            {
                if (taxName == "среднеотпускной тариф")
                {
                    Label1.Text = String.Format("<span class='DigitsValue'>{1}</span>&nbsp;в&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;году:<br/>",
                        currentYear, row[0]);

                    Label1.Text += "<table style='margin-left: 20px; border-collapse: collapse'>";

                    if (row[1] != DBNull.Value &&
                        row[1].ToString() != String.Empty &&
                        Convert.ToDouble(row[1].ToString()) != 0)
                    {
                        Label1.Text += String.Format("<tr><td>{0}</td><td align='right'>&nbsp;<b><span class='DigitsValueXLarge'>{1:N2}</span></b>&nbsp;{2}</td></tr>",
                             taxName, row[1], row[13]);
                    }

                    if (row[3] != DBNull.Value &&
                        row[3].ToString() != String.Empty &&
                        Convert.ToDouble(row[3].ToString()) != 0)
                    {
                        Label1.Text += String.Format("<tr style='margin-top: -100px'><td>тариф для населения</td><td align='right'>&nbsp;<b><span class='DigitsValueXLarge'>{1:N2}</span></b>&nbsp;{0}</td></tr>",
                            row[13], row[3]);

                    }
                    Label1.Text += "</table>";

                    Label1.Text += String.Format("<br/>По сравнению с&nbsp;<b><span class='DigitsValue'>{0}</span></b>&nbsp;годом темп прироста составил:<br/>",
                        currentYear - 1);

                    Label1.Text += "<table style='margin-left: 20px; border-collapse: collapse'>";

                    if (row[1] != DBNull.Value &&
                        row[1].ToString() != String.Empty &&
                        Convert.ToDouble(row[1].ToString()) != 0)
                    {
                        Label1.Text += String.Format("<tr><td>{3}</td><td align='right'>&nbsp;<b><span class='DigitsValueXLarge'>{0:P0}</span></b></td><td>&nbsp;({1} чем РФ</td><td>&nbsp;<b><span class='DigitsValue'>{2:P2}</span></b>)</td></tr>",
                            currentGrown, rfMiddleLevelDescription, avgRfGrown, taxName);
                    }

                    if (row[3] != DBNull.Value &&
                        row[3].ToString() != String.Empty &&
                        Convert.ToDouble(row[3].ToString()) != 0)
                    {
                        Label1.Text += String.Format("<tr><td>тариф для населения</td><td align='right'>&nbsp;<b><span class='DigitsValueXLarge'>{0:P0}</span></b></td><td>&nbsp;({1} чем РФ</td><td>&nbsp;<b><span class='DigitsValue'>{2:P2}</span></b>)</td></tr>",
                            currentGrownDemos, rfMiddleLevelDescriptionDemos, avgRfGrownDemos);

                    }
                    Label1.Text += "</table>";
                }
                else if (row[3] != DBNull.Value &&
                        row[3].ToString() != String.Empty &&
                        Convert.ToDouble(row[3].ToString()) != 0)
                {
                    Label1.Text = String.Format("<span class='DigitsValue'>{1}</span>&nbsp;в&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;году {2} составил&nbsp;<b><span class='DigitsValueXLarge'>{3:N2}</span></b>&nbsp;{4}",
                            currentYear, row[0], taxName, row[3], row[13]);

                    if (row[2] != DBNull.Value && Convert.ToDouble(row[2]) != -1)
                    {
                        Label1.Text += String.Format("<br/><br/>По сравнению с&nbsp;<b><span class='DigitsValue'>{0}</span></b>&nbsp;годом темп прироста составил<br/><b><span class='DigitsValueXLarge'>{1:P0}</span></b>&nbsp;({2}, чем РФ&nbsp;<b><span class='DigitsValue'>{3:P2}</span></b>)",
                            currentYear - 1,
                            currentGrownDemos, rfMiddleLevelDescriptionDemos, avgRfGrownDemos);
                    }
                }


                string tooltip = String.Format("<span style='font-family: Arial; font-size: 14pt'>{0}<br/>{1}<br/>на&nbsp;<b>{10}</b>&nbsp;год&nbsp;<b>{9}</b>&nbsp;{2}<br/>Темп прироста тарифа&nbsp;<b>{6:P2}</b></span>",
                                RegionsNamingHelper.FullName(row[0].ToString().Replace("*", "")),
                                iPadBricks.iPadBricks.iPadBricksHelper.GetWarpedHint(taxName),
                                row[11],
                                RegionsNamingHelper.ShortName(RegionsNamingHelper.GetFoBySubject(RegionsNamingHelper.FullName(row[0].ToString().Replace("*", "")))),
                                row["Ранг среднеотпускной тариф ФО"],
                                row["Ранг среднеотпускной тариф РФ"],
                                currentGrown,
                                row["Ранг прирост тарифа ФО"],
                                row["Ранг прирост тарифа РФ"],
                                row[1],
                                currentYear,
                                GetRankImg(row, "Ранг среднеотпускной тариф ФО", "Худший среднеотпускной тариф ФО"),
                                GetRankImg(row, "Ранг прирост тарифа ФО", "Худший прирост тарифа ФО"),
                                GetRankImg(row, "Ранг среднеотпускной тариф РФ", "Худший среднеотпускной тариф РФ"),
                                GetRankImg(row, "Ранг прирост тарифа РФ", "Худший прирост тарифа РФ")).Replace("'", "\"");

                TooltipHelper.AddToolTip(div, tooltip, this.Page);
            }
        }


        private static string GetRankImg(DataRow row, string columnName, string worseColumnName)
        {
            string foFirstRankImg = string.Empty;
            if (row[columnName] != DBNull.Value)
            {
                if (row[columnName].ToString() == "1")
                {
                    foFirstRankImg = "<img src='../../../images/starGray.png'>";
                }
                else if (row[columnName].ToString() == row[worseColumnName].ToString())
                {
                    foFirstRankImg = "<img src='../../../images/starYellow.png'>";
                }
            }
            return foFirstRankImg;
        }
    }
}