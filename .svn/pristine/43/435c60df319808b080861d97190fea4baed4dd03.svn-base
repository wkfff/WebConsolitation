using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Oil_0005_0001 : CustomReportPage
    {

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "День", dtDate);

            if (dtDate.Rows.Count < 2)
            {
                LabelInfo.Text = String.Format("Данные по розничным ценам на нефтепродукты отсутствуют");
                return;
            }

            CustomParam dateParam = UserParams.CustomParam("date");
            dateParam.Value = dtDate.Rows[0]["Дата"].ToString();
            DateTime lastDate = CRHelper.DateByPeriodMemberUName(dateParam.Value, 3);
            LabelInfo.Text = String.Format("Розничные цены на нефтепродукты на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", lastDate);
            DataTable dtLastDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Организация", dtLastDate);

            dateParam.Value = dtDate.Rows[1]["Дата"].ToString();
            DateTime prevDate = CRHelper.DateByPeriodMemberUName(dateParam.Value, 3);
            LabelInfo.Text += String.Format("(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)", prevDate);
            DataTable dtPrevDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Организация", dtPrevDate);

            string fuelUp = String.Empty, fuelDown = String.Empty, fuelConst = String.Empty;
            foreach (DataColumn column in dtLastDate.Columns)
            {
                if (column.ColumnName == "Организация")
                    continue;
                string fuel = column.ColumnName;
                object price = MathHelper.GeoMean(column, 0);
                object prevPrice = MathHelper.GeoMean(dtPrevDate.Columns[fuel], 0);
                object value = MathHelper.Grown(price, prevPrice);
                if (MathHelper.AboveZero(value))
                {
                    fuelUp += String.Format("<br/>{0} на&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>(цена - {2:N2} руб.)", fuel, value, price);
                }
                else if (MathHelper.SubZero(value))
                {
                    fuelDown += String.Format("<br/>{0} на&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>(цена - {2:N2} руб.)", fuel, MathHelper.Abs(value), price);
                }
                else
                {
                    fuelConst += String.Format("<br/>{0}", fuel);
                }
            }
            if (fuelUp == fuelDown)
            {
                LabelInfo.Text += "<br/>Цены в Югре на бензин всех марок и дизельное топливо остались без изменений.";
                return;
            }

            if (!String.IsNullOrEmpty(fuelUp))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Отмечено увеличение цен{0}на:</span>{1}", "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;", fuelUp);
            }

            if (!String.IsNullOrEmpty(fuelDown))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Отмечено снижение цен{0}на:</span>{1}", "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;", fuelDown);
            }

            if (!String.IsNullOrEmpty(fuelConst))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Цены стабильны на:</span>{0}", fuelConst);
            }

        }
    }
}
