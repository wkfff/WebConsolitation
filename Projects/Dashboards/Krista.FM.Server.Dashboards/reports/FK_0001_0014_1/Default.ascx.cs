using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0014_1
{ 
    public partial class Default : GadgetControlBase, IHotReport
    {
        private CustomParam bkkuDate;

        public int Width
        {
            get { return (int) CRHelper.GetGridWidth(230).Value; }
        }

        public int Height
        {
            get { return (int)CRHelper.GetGridHeight(600).Value; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 2);
            UltraWebGrid.Height = Unit.Empty;
            if (!Page.IsPostBack)
            {
                string query = DataProvider.GetQueryText("FK_0001_0014_1_date", Server.MapPath("~/reports/FK_0001_0014_1/"));

                DataTable dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();

                Page.Title = "Основные показатели консолидированных бюджетов субъектов РФ";

                int year = endYear;
                int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());

                DateTime date = new DateTime(year, CRHelper.MonthNum(UserParams.PeriodMonth.Value), 1);
                UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, date, 4);
                if (String.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    UserParams.StateArea.Value = "Ярославская область";
                }
                UserParams.Region.Value = RegionsNamingHelper.GetFoBySubject(UserParams.StateArea.Value);
               
                UserParams.Subject.Value = string.Format("[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);

                bkkuDate = UserParams.CustomParam("bkkuDate");

                if (date.Year == 2010)
                {
                    date.AddYears(-1);
                }

                bkkuDate.Value = CRHelper.PeriodMemberUName(String.Empty, date, 4);
                HyperLink1.Text = String.Format("Фактические&nbsp;показатели исполнения&nbsp;бюджета за&nbsp;{0}&nbsp;{1}&nbsp;{2}&nbsp;года ({3})", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year, UserParams.StateArea.Value);
                HyperLink1.Attributes.Add("target", "_blank");
                string navigateUrl = String.Format("~/reports/DashboardFederal/Dashboard.aspx");
                HyperLink1.NavigateUrl = navigateUrl;
                HyperLink2.Text = string.Format("Подробнее {0}", UserParams.StateArea.Value);
                HyperLink2.NavigateUrl = "~/reports/DashboardFederal/Dashboard.aspx";
                Label1.CssClass = "bujetText";
                UltraWebGrid.Style.Add("margin-top", "5px");
            }
            UltraWebGrid.DataBind();
            UltraWebGrid.Visible = false;
            
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0014_1_Grid_incomes", Server.MapPath("~/reports/FK_0001_0014_1/"));
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            query = DataProvider.GetQueryText("FK_0001_0014_1_Grid_outcomes", Server.MapPath("~/reports/FK_0001_0014_1/"));
            DataTable dtGrid1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid1);

            query = DataProvider.GetQueryText("FK_0001_0014_1_Grid_deficite", Server.MapPath("~/reports/FK_0001_0014_1/"));
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid2);

            query = DataProvider.GetQueryText("FK_0001_0014_1_Grid_bkku", Server.MapPath("~/reports/FK_0001_0014_1/"));
            DataTable dtGrid3 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid3);

            foreach (DataRow row in dtGrid1.Rows)
            {
                dtGrid.ImportRow(row);
            }

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            foreach (DataRow row in dtGrid3.Rows)
            {
                dtGrid.ImportRow(row);
            } 

            dtGrid.AcceptChanges();

            if (Request.Params["saveXml"] != null && Request.Params["saveXml"] == "y")
            {
                MemoryStream ms = new MemoryStream();
                dtGrid.WriteXml(ms);
                ms.Flush();
                StreamReader writer = new StreamReader(ms);
                ms.Position = 0;
                Response.Write(writer.ReadToEnd());
                writer.Close();
                Response.End();
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].HeaderLayout.Clear();
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.RowAlternateStyleDefault.CopyFrom(e.Layout.RowStyleDefault);
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = 200;
            e.Layout.Bands[0].Columns[1].Width = 150;
            e.Layout.Bands[0].Columns[1].Hidden = true;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().Contains("объем доходов"))
            {
                e.Row.Cells[0].Value = String.Format("<b>Общий объём доходов</b><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{0:N2}</b> тыс. руб.<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;в том числе за счет:", e.Row.Cells[1].Value);
            }
            else if (e.Row.Cells[0].Value.ToString().Contains("объем расходов"))
            {
                e.Row.Cells[0].Value = String.Format("<b>Общий объём расходов</b><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{0:N2}</b> тыс. руб.<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;в том числе:", e.Row.Cells[1].Value);
            }
            else if (e.Row.Cells[0].Value.ToString().Contains("Дефицит (-)/профицит (+)"))
            {
                if (Convert.ToDouble(e.Row.Cells[1].Value) < 0)
                {
                    e.Row.Cells[0].Style.BackgroundImage = "~/images/ballRedBB.png";
                    e.Row.Cells[0].Value = String.Format("<b>Дефицит (-)/профицит (+)</b><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b><span style=\"color:red \">{0:N2}</span> тыс. руб.</b>", e.Row.Cells[1].Value);
                }
                else
                {
                    e.Row.Cells[0].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    e.Row.Cells[0].Value = String.Format("<b>Дефицит (-)/профицит (+)</b><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{0:N2}</b> тыс. руб.", e.Row.Cells[1].Value);
                }
                e.Row.Cells[0].Title = "Плановый объем дефицита/профицита бюджета";
            }
            else if (e.Row.Cells[0].Value.ToString().Contains("объем государственного долга"))
            {
                e.Row.Cells[0].Value = String.Format("<b>{1}</b><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{0:N2}</b> тыс. руб.", e.Row.Cells[1].Value, e.Row.Cells[0].Value);
            }
            else if (e.Row.Index < 4)
            {
                e.Row.Cells[0].Title = "Утвержденный план на год";
                e.Row.Cells[0].Value = String.Format("{0}<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{1:N2}</b> тыс. руб.", e.Row.Cells[0].Value, e.Row.Cells[1].Value);
            }
            else
            {
                e.Row.Cells[0].Title = "Удельный вес социально-значимых расходов в расходах бюджета";
                e.Row.Cells[0].Value = String.Format("{0}<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{1:N2}</b> тыс. руб.", e.Row.Cells[0].Value, e.Row.Cells[1].Value);
            }
            if (e.Row.Cells[0].Value.ToString().Contains("санкции"))
            {
                e.Row.Hidden = true;
                if (Convert.ToDouble(e.Row.Cells[1].Value) == 1)
                {
                    Label1.Text = e.Row.Cells[0].Value.ToString();
                    Image1.ImageUrl = "~/images/ballRedBB.png";
                    Label1.Font.Bold = false;
                   // e.Row.Cells[0].Style.BackgroundImage = "~/images/ballRedBB.png";
                }
                else 
                {
                    Label1.Text =
                        String.Format("к субъекту РФ не применяются санкции, утвержденные пунктом 4 статьи 130 БК РФ");
                    Image1.ImageUrl = "~/images/ballGreenBB.png";
                    Label1.Font.Bold = false;
                   // e.Row.Cells[0].Style.BackgroundImage = "~/images/ballGreenBB.png";
                   // e.Row.Cells[1].Value = "Нет";
                }
                e.Row.Cells[0].Title = "Для субъектов РФ, в бюджетах которых доля межбюджетных трансфертов из федерального бюджета в течение двух из трех последних отчетных финансовых лет превышала 60 % объема собственных доходов";
                e.Row.Cells[0].Style.Padding.Right = 40;
                return;
            } 
            else if (e.Row.Cells[0].Value.ToString().Contains("Предельный объем государственного долга субъекта"))
            {
                e.Row.Cells[1].Title = "Предельный объем государственного долга, рассчитанный по утвержденным бюджетным назначениям";
            }
            else if (!e.Row.Cells[0].Value.ToString().Contains("в том числе"))
            {
                
            }
            string style = "";
            e.Row.Cells[0].Style.CustomRules = style;
            e.Row.Cells[0].Style.Padding.Bottom = 7;
            e.Row.Cells[0].Style.BorderStyle = BorderStyle.None;

            
            TableRow tableRow = new TableRow();
            TableCell cell = new TableCell();
            cell.Text = e.Row.Cells[0].Value.ToString();
            cell.CssClass = "tableCell";
            tableRow.Cells.Add(cell);
            Table1.Rows.Add(tableRow);
        }

        #endregion

        public override string Title
        {
            get { return ""; }
            set { }
        }


    }
}
