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
    public partial class Food_0005_0001 : CustomReportPage
    {

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "День", dtDate);

            if (dtDate.Rows.Count < 2)
            {
                LabelInfo.Text = String.Format("Данные по розничным ценам на продукты питания отсутствуют");
                return;
            }

            CustomParam lastDateParam = UserParams.CustomParam("last_date");
            lastDateParam.Value = dtDate.Rows[0]["Дата"].ToString(); ;
            DateTime lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);
            LabelInfo.Text = String.Format("Розничные цены на продукты питания на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", lastDate);

            CustomParam prevDateParam = UserParams.CustomParam("prev_date");
            prevDateParam.Value = dtDate.Rows[1]["Дата"].ToString();
            DateTime prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);
            LabelInfo.Text += String.Format("(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)", prevDate);

            DataTable dtData = new DataTable();
            query = DataProvider.GetQueryText("Food_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование", dtData);

            string foodUp = String.Empty, foodDown = String.Empty, foodConst = String.Empty;
            foreach (DataRow row in dtData.Rows)
            {
                if (!MathHelper.IsDouble(row["Темп прироста"]))
                    continue;
                string food = ConvertFoodName(row["Наименование"].ToString());
                double value = Convert.ToDouble(row["Темп прироста"]);
                double price = Convert.ToDouble(row["Цена на последнюю дату"]);
                if (value > 0)
                {
                    foodUp += String.Format("<br/>{0} на&nbsp;<span class='DigitsValue'>{1:P2} </span>(цена - {2:N2} руб.)", food, value, price);
                }
                else if (value < 0)
                {
                    foodDown += String.Format("<br/>{0} на&nbsp;<span class='DigitsValue'>{1:P2} </span>(цена - {2:N2} руб.)", food, -value, price);
                }
                else
                {
                    foodConst += String.Format("<br/>{0}", food);
                }
            }
            if (foodUp == foodDown)
            {
                LabelInfo.Text += "<br/>Изменений цен на продовольственные товары не наблюдалось.";
                return;
            }

            if (!String.IsNullOrEmpty(foodUp))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Отмечено увеличение цен{0}на:</span>{1}", "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;", foodUp);
            }

            if (!String.IsNullOrEmpty(foodDown))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Отмечено снижение цен{0}на:</span>{1}", "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;", foodDown);
            }
            /*
            if (!String.IsNullOrEmpty(foodConst))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Цены стабильны на:</span>{0}", foodConst);
            }*/

        }

        private string ConvertFoodName(string food)
        {
            return food.Replace("Рыба мороженная неразделенная", "Рыба замороженная неразделанная").Replace("(2,5-3,2 % жирн.)", "2,5-3,2 % жирности");
        }

    }
}
