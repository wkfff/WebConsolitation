using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using System.Xml;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0001 : CustomReportPage
    {
        private DataTable LoadPersons()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/iphone/fo_0003_0001/Default.settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["table"];
        }

        DateTime date;
        DateTime debtDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            CustomParam periodDebtDate = UserParams.CustomParam("period_debt_date");
            CustomParam periodLastLastYear = UserParams.CustomParam("period_last_last_year");
            CustomParam periodThreeYearAgo = UserParams.CustomParam("period_3_last_year");
            CustomParam periodFourYearAgo = UserParams.CustomParam("period_4_last_year");

            HeraldImageContainer.InnerHtml = String.Format("<a href ='{1}'><img style='margin-right: 20px; height: 65px' src=\"../../../images/Heralds/{0}.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"], HttpContext.Current.Session["CurrentSiteRef"]);
            PersonImageContainer.InnerHtml = String.Format("<a href ='webcommand?showPopoverReport=fo_0003_0003_{0}&width=500&height=600&fitByHorizontal=true'><img src=\"../../../images/person.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"]);

            DataTable dtPerson = LoadPersons();

            DataRow[] persons = dtPerson.Select(String.Format("territory = '{0}'", UserParams.StateArea.Value));

            if (persons.Length > 0 &&
                persons[0]["twitter"] != DBNull.Value &&
                persons[0]["twitter"].ToString() != "")
            {
                TwitterImageContainer.InnerHtml = String.Format("<a href ='http://twitter.com/#!/{0}'><img src=\"../../../images/twitter.png\"></a>", persons[0]["twitter"]);
            }

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("fo_0003_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            UserParams.PeriodYear.Value = date.Year.ToString();
            UserParams.PeriodLastYear.Value = date.AddYears(-1).Year.ToString();
            periodLastLastYear.Value = date.AddYears(-2).Year.ToString();
            periodThreeYearAgo.Value = date.AddYears(-3).Year.ToString();
            periodFourYearAgo.Value = date.AddYears(-4).Year.ToString();

            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", date.AddYears(-1), 4);

            UltraWebGridIncomes.InitializeRow += new InitializeRowEventHandler(UltraWebGridIncomes_InitializeRow);
            UltraWebGridOutcomes.InitializeRow += new InitializeRowEventHandler(UltraWebGridOutcomes_InitializeRow);
            UltraWebGridSources.InitializeRow += new InitializeRowEventHandler(UltraWebGridSources_InitializeRow);

            SetupGrid(UltraWebGridIncomes, "fo_0003_0001_incomes");
            SetupGrid(UltraWebGridOutcomes, "fo_0003_0001_outcomes");
            SetupGrid(UltraWebGridSources, "fo_0003_0001_sources");

            UltraWebGridOutcomes.Bands[0].HeaderLayout.Clear();
            UltraWebGridSources.Bands[0].HeaderLayout.Clear();

            UltraWebGridOthers.Width = Unit.Empty;
            UltraWebGridOthers.Height = Unit.Empty;
            UltraWebGridOthers.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGridOthers_InitializeLayout);
            UltraWebGridOthers.InitializeRow += new InitializeRowEventHandler(UltraWebGridOthers_InitializeRow);

            DataTable dtDateDebts = new DataTable();
            query = DataProvider.GetQueryText("fo_0003_0001_date_debts");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDateDebts);

            periodDebtDate.Value = dtDateDebts.Rows[0][1].ToString();
            debtDate = CRHelper.DateByPeriodMemberUName(dtDateDebts.Rows[0][1].ToString(), 3);

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("fo_0003_0001_others");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

            UltraWebGridOthers.DataSource = dt;
            if (debtDate != date)
            {
                dt.Rows[1][0] = String.Format("{0} (на {1:dd.MM.yyyy})", dt.Rows[1][0], debtDate.AddMonths(1));
            }
            UltraWebGridOthers.DataBind();

            lbDescription.Text = String.Format("Показатели исполнения бюджетов субъекта РФ&nbsp;<span class='DigitsValue'>на {0:dd.MM.yyyy}</span>, тыс.руб.", date.AddMonths(1));

            GetIndicatorImageIncomes();
            GetIndicatorImageOutcomes();

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/FO_0003_0001/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/FO_0003_0001/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"FO_0003_0002_{0}\" bounds=\"x=0;y=120;width=768;height=260\" openMode=\"incomes\"/><element id=\"FO_0003_0004_{0}\" bounds=\"x=0;y=380;width=768;height=230\" openMode=\"outcomes\"/><element id=\"FO_0003_0005_{0}\" bounds=\"x=0;y=610;width=768;height=100\" openMode=\"rests\"/><element id=\"FO_0003_0006_{0}\" bounds=\"x=0;y=710;width=768;height=130\" openMode=\"\"/></touchElements>", CustomParams.GetSubjectIdByName(UserParams.StateArea.Value)));
        }

        void UltraWebGridOthers_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = String.Format("<div style='margin-bottom: -50px'><a href='webcommand?showPinchReport=FO_0003_0006_{0}'><img style='padding-top: 5px' src='../../../images/TableDetail.png'/></a></div>", HttpContext.Current.Session["CurrentSubjectID"]);
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index != 3)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }
        }

        void UltraWebGridSources_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = String.Format("<div style='margin-bottom: -50px;'><a href='webcommand?showPinchReport=FO_0003_0005_{0}'><img style='' src='../../../images/TableDetail.png'/></a></div>", HttpContext.Current.Session["CurrentSubjectID"]);
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index != 2)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }

            if (e.Row.Cells[5].Value.ToString() == "1")
            {
                e.Row.Cells[1].Style.Font.Bold = true;
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "2")
            {
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "3")
            {
                e.Row.Cells[1].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[5].Value.ToString() == "4")
            {
                e.Row.Cells[1].Style.Font.Italic = true;
                e.Row.Cells[1].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[1].Value.ToString().Contains("Недостаток") ||
                e.Row.Cells[1].Value.ToString().Contains("Дефицит"))
            {
                SetConditionCorner(e, 2, 0);
            }
        }

        void UltraWebGridOutcomes_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = String.Format("<div style='margin-bottom: -100px'><img src='../../../images/Outcomes.png'/><a href='webcommand?showPinchReport=FO_0003_0004_{0}'><img style='padding-top: 5px' src='../../../images/TableDetail.png'/></a><br/><br/><a href ='webcommand?showPopoverReport=fo_0003_0009_{0}&width=690&height=450&fitByHorizontal=true'><img src='../../../images/ChartDetail.png'/></a></div>", HttpContext.Current.Session["CurrentSubjectID"]);
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index != 6)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }

            if (e.Row.Cells[5].Value != null)
            {
                if (e.Row.Cells[5].Value.ToString() == "1")
                {
                    e.Row.Cells[1].Style.Font.Bold = true;
                    e.Row.Cells[1].Style.ForeColor = Color.White;
                }
                if (e.Row.Cells[5].Value.ToString() == "2")
                {
                    e.Row.Cells[1].Style.ForeColor = Color.White;
                }
                if (e.Row.Cells[5].Value.ToString() == "3")
                {
                    e.Row.Cells[1].Style.Padding.Left = 20;
                }
                if (e.Row.Cells[5].Value.ToString() == "4")
                {
                    e.Row.Cells[1].Style.Font.Italic = true;
                    e.Row.Cells[1].Style.Padding.Left = 20;
                }
                if (e.Row.Cells[1].Value.ToString().Contains("Недостаток") ||
                    e.Row.Cells[1].Value.ToString().Contains("Дефицит"))
                {
                    SetConditionCorner(e, 2, 0);
                }
            }
        }

        public void SetConditionCorner(RowEventArgs e, int index, int borderValue)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Math.Round(Convert.ToDouble(e.Row.Cells[index].Value.ToString()));                
                string img = string.Empty;
                if (value < borderValue)
                {
                    img = "~/images/cornerRed.gif";

                }
                else if (value > borderValue)
                {
                    img = "~/images/cornerGreen.gif";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-left: 2px";
            }
        }

        void UltraWebGridIncomes_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = String.Format("<div style='margin-bottom: -100px'><img src='../../../images/Incomes.png'/><a href='webcommand?showPinchReport=FO_0003_0002_{0}'><img style='padding-top: 5px' src='../../../images/TableDetail.png'/></a><br/><br/><a href ='webcommand?showPopoverReport=fo_0003_0008_{0}&width=690&height=770&fitByHorizontal=true'><img src='../../../images/ChartDetail.png'/></a></div>", HttpContext.Current.Session["CurrentSubjectID"]);
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index != 9)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }

            if (e.Row.Cells[5].Value.ToString() == "1")
            {
                e.Row.Cells[1].Style.Font.Bold = true;
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "2")
            {
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "3")
            {
                e.Row.Cells[1].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[5].Value.ToString() == "4")
            {
                e.Row.Cells[1].Style.Font.Italic = true;
                e.Row.Cells[1].Style.Padding.Left = 20;
            }

        }

        void UltraWebGridOthers_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Columns.Insert(0, new UltraGridColumn());

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 53;
            e.Layout.Bands[0].Columns[1].Width = 425;
            e.Layout.Bands[0].Columns[2].Width = 285;

            e.Layout.Bands[0].Columns[2].CellStyle.Padding.Right = 3;
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Исполнение на {0:dd.MM.yyyy}", date.AddMonths(1));

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
        }

        private void SetupGrid(UltraWebGrid grid, string queryName)
        {
            grid.Width = Unit.Empty;
            grid.Height = Unit.Empty;
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

            dt.Columns.RemoveAt(0);
            grid.DataSource = dt;
            grid.DataBind();
        }

        void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Columns.Insert(0, new UltraGridColumn());

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 53;
            e.Layout.Bands[0].Columns[1].Width = 370;
            e.Layout.Bands[0].Columns[2].Width = 110;
            e.Layout.Bands[0].Columns[3].Width = 110;
            e.Layout.Bands[0].Columns[4].Width = 120;

            e.Layout.Bands[0].Columns[2].CellStyle.Padding.Right = 3;
            e.Layout.Bands[0].Columns[3].CellStyle.Padding.Right = 3;
            e.Layout.Bands[0].Columns[4].CellStyle.Padding.Right = 3;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P0");

            e.Layout.Bands[0].Columns[5].Hidden = true;

            e.Layout.Bands[0].Columns[1].CellStyle.Padding.Top = 1;
            e.Layout.Bands[0].Columns[0].CellStyle.Padding.Bottom = 1;
            e.Layout.Bands[0].Columns[1].CellStyle.Padding.Bottom = 1;
            e.Layout.Bands[0].Columns[2].CellStyle.Padding.Bottom = 1;
            e.Layout.Bands[0].Columns[3].CellStyle.Padding.Bottom = 1;
            e.Layout.Bands[0].Columns[4].CellStyle.Padding.Bottom = 1;

            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("Исполнение на {0:dd.MM.yyyy}", date.AddMonths(1));
        }

        private void GetIndicatorImageOutcomes()
        {
            DataTable dt = new DataTable();
            int i = 13;
            UserParams.Filter.Value = GetIndicatorName(i.ToString());
            string query = DataProvider.GetQueryText("fo_0003_0001_indicator_outcomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);
            if (dt.Rows[0]["Внимание"].ToString() != "")
            {
                outcomesIndicatorsDiv.InnerHtml += String.Format("<a href='webcommand?showPopoverReport=fo_0003_0011_Control={1}_{0}&width=690&height=300'><img style='margin-right: 7px' src='../../../images/IndicatorSigns/{1}.png'/></a>", HttpContext.Current.Session["CurrentSubjectID"], i);
            }

            dt = new DataTable();
            i = 14;
            UserParams.Filter.Value = GetIndicatorName(i.ToString());
            query = DataProvider.GetQueryText("fo_0003_0001_indicator_outcomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);
            if (dt.Rows[0]["Внимание"].ToString() != "")
            {
                outcomesIndicatorsDiv.InnerHtml += String.Format("<a href='webcommand?showPopoverReport=fo_0003_0011_Control={1}_{0}&width=600&height=500'><img style='margin-right: 7px' src='../../../images/IndicatorSigns/{1}.png'/></a>", HttpContext.Current.Session["CurrentSubjectID"], i);
            }

            i = 15;
            dt = new DataTable();
            query = DataProvider.GetQueryText("fo_0003_0001_241");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);
            if (dt.Rows[0]["Внимание "].ToString() != "0")
            {
                outcomesIndicatorsDiv.InnerHtml += String.Format("<a href='webcommand?showPopoverReport=fo_0003_0011_Control={1}_{0}&width=600&height=500'><img style='margin-right: 7px' src='../../../images/IndicatorSigns/{1}.png'/></a>", HttpContext.Current.Session["CurrentSubjectID"], i);
            }
        }

        private void GetIndicatorImageIncomes()
        {
            string indicator = string.Empty;

            for (int i = 1; i < 13; i++)
            {
                if (i != 8)
                {
                    UserParams.Filter.Value = GetIndicatorName(i.ToString());

                    DataTable dt = new DataTable();
                    string query = DataProvider.GetQueryText("fo_0003_0001_indicator");
                    DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);
                    if (dt.Rows[0]["Внимание"].ToString() != "")
                    {
                        incomesIndicatorsDiv.InnerHtml += String.Format("<a href='webcommand?showPopoverReport=fo_0003_0011_Control={1}_{0}&width=600&height=500'><img style='margin-right: 7px' src='../../../images/IndicatorSigns/{1}.png'/></a>", HttpContext.Current.Session["CurrentSubjectID"], i);
                    }
                }
            }

        }

        private static string GetIndicatorName(string id)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/ControlIndicators.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            // Ищем узел регионов
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "ControlIndicators")
                {
                    foreach (XmlNode regionNode in rootNode.ChildNodes)
                    {
                        if (regionNode.Attributes["id"].Value == id)
                        {
                            return regionNode.Attributes["name"].Value;
                        }

                    }
                }
            }
            return String.Empty;
        }
    }
}
