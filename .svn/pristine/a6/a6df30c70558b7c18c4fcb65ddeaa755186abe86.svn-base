using System;
using System.Data;
using System.Drawing;
using System.Web;
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
    public partial class Food_0002_0001_v : CustomReportPage
    {
        private DateTime lastDate;
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Камчатский край";
            }

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0002_0001_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[0][1].ToString();

            lbDescription.Text = String.Format("Розничные цены на продукты питания на&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>&nbsp;(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>)", lastDate, currentDate);

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.DataBind();

            Label1.Text = string.Format("данные на {0} {1} {2} года", currentDate.Day, CRHelper.RusMonthGenitive(currentDate.Month), currentDate.Year);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Width = 150;
            e.Layout.Bands[0].Columns[1].Width = 70;
            e.Layout.Bands[0].Columns[2].Width = 90;

            //for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
            //{
            //    e.Layout.Bands[0].Columns[i].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            //}

            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
        }

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("Food_0002_0001_table1_v");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            DataTable dtSource = new DataTable();
            dtSource.Columns.Add(new DataColumn("Наименование", typeof(string)));

            dtSource.Columns.Add(new DataColumn(String.Format("Цена на {0:dd.MM.yy}, руб.", currentDate), typeof(string)));
            dtSource.Columns.Add(new DataColumn(String.Format("Динамика к {0:dd.MM.yy}, руб.", lastDate), typeof(string)));
            
            for (int rowCount = 0; rowCount < dt.Rows.Count; rowCount++)
            {
                DataRow row = dtSource.NewRow();
                row[0] = dt.Rows[rowCount][0];
                int columnCount = 1;


                if (dt.Rows[rowCount][columnCount] != DBNull.Value)
                {
                    double grownValue = 0;
                    if (dt.Rows[rowCount][columnCount] != DBNull.Value)
                    {
                        grownValue = Convert.ToDouble(dt.Rows[rowCount][columnCount + 1].ToString());
                    }

                    string absoluteGrown = grownValue > 0
                                               ? String.Format("+{0:N2}", grownValue)
                                               : String.Format("{0:N2}", grownValue);

                    string img = String.Empty;
                    if (grownValue != 0)
                    {
                        img = grownValue > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }

                    row[1] = String.Format("{0:N2}", dt.Rows[rowCount][columnCount]);
                    row[2] = String.Format("{0}<br/>{2}&nbsp;{1:N2}%", absoluteGrown, dt.Rows[rowCount][columnCount + 2], img);
                }
                dtSource.Rows.Add(row);
            }

            UltraWebGrid1.DataSource = dtSource;
        }
    }
}

