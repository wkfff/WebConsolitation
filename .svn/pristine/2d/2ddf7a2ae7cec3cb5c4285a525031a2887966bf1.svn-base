using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Xml;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class STAT_0001_0002 : CustomReportPage
    {
        private DataTable dt;

        // Дата
        private CustomParam periodDay;
        private CustomParam debtsDay;

        private DateTime date;
        private DateTime lastDate;
        private DateTime debtsDate;
        private DateTime debtsLastDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (UserParams.Region.Value != "Уральский федеральный округ")
            {
                Label17.Text =
                    String.Format(
                        "По выбранному субъекту Российской Федерации <b>&nbsp;{0}&nbsp;</b> отсутствуют сведения по показателям мониторинга ситуации на рынке труда.<br/>На данный момент информация присутствует только по субъектам Уральского федерального округа.",
                        UserParams.StateArea.Value);
                tableMain.Visible = false;
                return;
            }
            Label17.Visible = false;

            periodDay = UserParams.CustomParam("period_day");
            debtsDay = UserParams.CustomParam("debts_day");

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][5].ToString(), 3);
            lastDate = date.AddDays(-7);

            periodDay.Value = dtDate.Rows[0][5].ToString();
            Image6.ImageUrl = "../../../images/workers.png";
            // Label1.Text = string.Format("на {0:dd.MM.yyyy} по {1}", date, RegionsNamingHelper.ShortName(UserParams.StateArea.Value));

            query = DataProvider.GetQueryText("STAT_0001_0002_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            debtsDay.Value = dtDebtsDate.Rows[1][1].ToString();
            debtsDate = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
            debtsLastDate = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
            LabelsDataBind();
        }

        private void LabelsDataBind()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0002_FO");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lb1.Text = string.Format("Число безработных в {0}<br/>на {1:dd.MM.yyyy}&nbsp;", RegionsNamingHelper.ShortName(UserParams.StateArea.Value), date);
            lb2.Text = "Уровень безработицы&nbsp;";
            lb3.Text = "Зарегистрированных безработных в расчете ";
            Label12.Text = String.Format("Задолженность по выплате заработной платы на {0:dd.MM.yyyy}&nbsp;", debtsDate);
            
            lb1Value.Text = String.Format("{0:N0}&nbsp;", dt.Rows[0][1]);
            lb2Value.Text = String.Format("{0:N3}%<br/>", dt.Rows[2][1]);
            lb3Value.Text = String.Format("{0:N2}&nbsp;", dt.Rows[4][1]);

            Label8.Text = String.Format("{0:N3}%&nbsp;", dt.Rows[3][1]);
            Label9.Text = String.Format("{0:N2}&nbsp;", dt.Rows[5][1]);

            Label10.Text = String.Format("{0:N0}&nbsp;", dt.Rows[2][3]);
            Label11.Text = String.Format("{0:N0}&nbsp;", dt.Rows[4][3]);

            if (Convert.ToDouble(dt.Rows[6][1]) == 0)
            {
                Label12.Text += "отсутствует";
                Label13.Visible = false;
                Label14.Visible = false;
            }
            else
            {
                Label13.Text = String.Format("{0:N3}&nbsp;", dt.Rows[6][1]);
            }

            if (Convert.ToDouble(dt.Rows[0][2]) > 0)
            {
                lb1Percent.Text = String.Format("+{0:P2}&nbsp;", dt.Rows[0][2]);
                Label2.Text = String.Format("+{0:N0}&nbsp;", dt.Rows[0][3]);
            }
            else
            {
                lb1Percent.Text = String.Format("{0:P2}&nbsp;", dt.Rows[0][2]);
                Label2.Text = String.Format("{0:N0}&nbsp;", dt.Rows[0][3]);
            }
            if (Convert.ToDouble(dt.Rows[0][2]) < 0)
            {
                Image1.ImageUrl = "../../../images/arrowDownGreen.png";
                lbGrown1.Text = " снижение c&nbsp;";
            }
            else
            {
                Image1.ImageUrl = "../../../images/arrowUpRed.png";
                lbGrown1.Text = " прирост c&nbsp;";
            }

            Label16.Text = String.Format("&nbsp;{0:N3} ", dt.Rows[6][3]);
            if (Convert.ToDouble(dt.Rows[6][3]) < 0)
            {
                Image2.ImageUrl = "../../../images/arrowDownGreen.png";
                Label15.Text = String.Format(" снижение c {0:dd.MM}&nbsp;", debtsLastDate);
            }
            else if (Convert.ToDouble(dt.Rows[6][3]) > 0)
            {
                Image2.ImageUrl = "../../../images/arrowUpRed.png";
                Label15.Text = String.Format(" увеличение c {0:dd.MM}&nbsp;", debtsLastDate);
            }
            else
            {
                Label15.Visible = false;
                Image2.Visible = false;
                Label16.Visible = false;
                Label18.Visible = false;
            }

            lbGrown1.Text += lastDate.ToString("dd.MM.yyyy");

            if (Convert.ToDouble(dt.Rows[2][3]) == 6)
            {
                Image3.ImageUrl = "../../../images/StarYellow.png";
            }
            else if (Convert.ToDouble(dt.Rows[2][3]) == 1)
            {
                Image3.ImageUrl = "../../../images/StarGray.png";
            }
            else
            {
                Image3.Visible = false;
            }

            if (Convert.ToDouble(dt.Rows[4][3]) == 6)
            {
                Image5.ImageUrl = "../../../images/StarYellow.png";
            }
            else if (Convert.ToDouble(dt.Rows[4][3]) == 1)
            {
                Image5.ImageUrl = "../../../images/StarGray.png";
            }
            else
            {
                Image5.Visible = false;
            }

            Image1.Height = 20;
            Image2.Height = 20;
            Image3.Height = 20;
            Image5.Height = 20;


            /*lb31.Text = " Уровень в УФО ";
            lb4.Text = "Напряженность на рынке труда ";
            lb5.Text = "Ранг в УФО ";
            lb51.Text = " Напряженность в УФО ";

            lb1Value.Text = String.Format(" {0:N0} ", dt.Rows[0][1]);
            lb2Value.Text = String.Format(" {0:P2} ", dt.Rows[2][1]);
            lb3Value.Text = String.Format(" {0:N0} ", dt.Rows[3][3]);
            //lb4Value.Text = String.Format("{0:N0}", dt.Rows[4][1]);
            //lb5Value.Text = String.Format("{0:N0}", dt.Rows[4][3]);

            lb4Value.Text = String.Format(" {0:P2} ", dt.Rows[2][1]);
            lb5Value.Text = String.Format(" {0:N0} ", dt.Rows[3][3]);
                        
            lb2Percent.Text = String.Format(" {0:P2}<br/>", dt.Rows[2][2]);
            lb3Percent.Text = String.Format(" {0:P2}<br/>", dt.Rows[3][2]);
            lb4Percent.Text = String.Format(" {0:P2}<br/>", dt.Rows[2][2]);
            lb5Percent.Text = String.Format(" {0:P2}<br/>", dt.Rows[3][2]);

            
            

            

            Image1.Height = 20;
            Image2.Height = 20;
            Image3.Height = 20;
            Image4.Height = 20;
            Image5.Height = 20; */
        }
    }
}
