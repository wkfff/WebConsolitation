using System;
using System.Data;
using System.Drawing;
using System.IO;

using Infragistics.UltraGauge.Resources;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Prog_0001_0001 : CustomReportPage
    {

        CustomParam progDataSourceParam;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            HeraldImageContainer.InnerHtml = "<img style='margin-left: -30px; margin-right: 20px; height: 100px' src=\"../../../images/Heralds/33.png\"></a>";

            #region Инициализация параметров запроса

            progDataSourceParam = UserParams.CustomParam("prog_data_source");

            #endregion

            #region Получение дат

            DataTable dtMonth = new DataTable();
            string query = DataProvider.GetQueryText("Prog_0001_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Даты", dtMonth);
            UserParams.PeriodMonth.Value = dtMonth.Rows[0][1].ToString();
            progDataSourceParam.Value = String.Format("ЭО\\0005 Программы - вариант: {0:yyyy}, ДЦП, месяц: {0:MM}, территория: Ненецкий автономный округ",
                CRHelper.DateByPeriodMemberUName(UserParams.PeriodMonth.Value, 3));

            #endregion

            BindStatisticsText();
            BindData();
        }

        private void BindStatisticsText()
        {
            DataTable dtInfo = new DataTable();
            string query = DataProvider.GetQueryText("Prog_0001_0001_info");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtInfo);
            DataRow row = dtInfo.Rows[0];

            DateTime date = CRHelper.DateByPeriodMemberUName(UserParams.PeriodMonth.Value, 3);
            date = date.AddMonths(1);

            lbInfo.Text = String.Format(
                "По состоянию на&nbsp;<span class='DigitsValue'>{0:01.MM.yyyy}</span>&nbsp;года в Ненецком автономном округе<br/>" +
                "<span class='DigitsValue'>объем финансирования</span>&nbsp;целевых программ составил:<br/>" +
                "<span class='DigitsValueLarge'><b>{1:N0}</b></span>&nbsp;тыс.руб. (<span class='DigitsValueLarge'>{2:P0}</span>&nbsp;от запланированного объема)<br/>" +
                "<span class='DigitsValue'>освоено</span>&nbsp;при выполнении мероприятий целевых программ:<br/>" +
                "<span class='DigitsValueLarge'><b>{3:N0}</b></span>&nbsp;тыс.руб. (<span class='DigitsValueLarge'>{4:P0}</span>&nbsp;от выделенных средств)",
                date, row["Исполнено"], row["Исполнено (процент)"], row["Освоено"], row["Освоено (процент)"]);
        }

        private void BindData()
        {
            DateTime date = CRHelper.DateByPeriodMemberUName(UserParams.PeriodMonth.Value, 3);
            lbPrograms.Text = String.Empty;
            string separator = String.Empty;
            DataTable dtData = new DataTable();
            string query = DataProvider.GetQueryText("Prog_0001_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Программа", dtData);
            lbPrograms.Text += "<table Class=HtmlTable width=760px>";
            foreach (DataRow row in dtData.Rows)
            {
                AddTable(row, separator, date);
                separator = "<tr style=\"height: 10px;\"></tr>";
            }
            lbPrograms.Text += "</table>";
        }

        private void AddTable(DataRow row, string separator, DateTime date)
        {
            lbPrograms.Text += separator;
            lbPrograms.Text += "<tr style=\"height: 40px;\">";
            lbPrograms.Text += String.Format("<td width=300px rowspan=3><a style='color: White' href='webcommand?showReport=Prog_0001_0002_Prog={1}'>{0}</a></td>",
                row["Программа"], CustomParams.GetProgIdByName(row["Программа"].ToString()));
            lbPrograms.Text += String.Format("<td style=\"width: 125px\">План на {0:yyyy} г.</td>", date);
            lbPrograms.Text += String.Format("<td align=center style=\"width: 100px\">{0:N0} тыс.руб.</td>", row["План"]);
            lbPrograms.Text += "<td align=center style=\"width: 100px\">&nbsp;</td>";
            lbPrograms.Text += "<td align=center style=\"width: 135px\">&nbsp;</td>";
            lbPrograms.Text += "</tr>";
            lbPrograms.Text += "<tr style=\"height: 40px;\">";
            lbPrograms.Text += String.Format("<td style=\"width: 125px\">Исполнено на {0:01.MM.yyyy} г.</td>", date.AddMonths(1));
            lbPrograms.Text += String.Format("<td align=center style=\"width: 100px\">{0:N0} тыс.руб.</td>", row["Исполнено"]);
            lbPrograms.Text += String.Format("<td align=center style=\"width: 100px\">{0}</td>", GetGaugeUrl(row["Исполнено (процент)"]));
            lbPrograms.Text += String.Format("<td align=center style=\"width: 135px\">{0:P2}<br/>от плана</td>", row["Исполнено (процент)"]);
            lbPrograms.Text += "</tr>";
            lbPrograms.Text += "<tr style=\"height: 40px;\">";
            lbPrograms.Text += String.Format("<td style=\"width: 125px\">Освоено на {0:01.MM.yyyy} г.</td>", date.AddMonths(1));
            lbPrograms.Text += String.Format("<td align=center style=\"width: 100px\">{0:N0} тыс.руб.</td>", row["Освоено"]);
            lbPrograms.Text += String.Format("<td align=center style=\"width: 100px\">{0}</td>", GetGaugeUrl(row["Освоено (процент)"]));
            lbPrograms.Text += String.Format("<td align=center style=\"width: 135px\">{0:P2}<br/>от выд. средств</td>", row["Освоено (процент)"]);
            lbPrograms.Text += "</tr>";
        }

        protected string GetGaugeUrl(object oValue)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue) * 100;
            if (value > 100)
                value = 100;
            string path = "Prog_0001_0001_gauge_" + value.ToString("N0") + ".png";
            string returnPath = String.Format("<img style=\"FLOAT: left;\" src=\"../../../TemporaryImages/{0}\"/>", path);
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

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

    }
}
