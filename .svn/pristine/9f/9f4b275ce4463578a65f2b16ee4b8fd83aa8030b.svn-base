using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;
using System.Drawing;
using System.Collections.ObjectModel;
using Infragistics.Documents.Reports.Report.Text;
using System.Data.OleDb;
using Infragistics.WebUI.UltraWebGrid;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
using System.DirectoryServices;

namespace Krista.FM.Server.Dashboards.reports.SGM.sgm_stat_0001
{
    public partial class sgm_stat_0001 : CustomReportPage
    {
        DataTable dtMain = new DataTable();
        bool baseFounded = false;

        string basePath = string.Empty;

       
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            basePath = string.Format(@"{0}\stat\mdb\dbstats.mdb", HttpContext.Current.Request.PhysicalApplicationPath);
            CRHelper.SaveToErrorLog(basePath);
            baseFounded = File.Exists(basePath);

            LabelTo.Text = "Конец периода:";
            LabelFrom.Text = "Начало периода:";

            if (!Page.IsPostBack)
            {
                dateFrom.WebCalendar.SelectedDate = new DateTime(2009, 1, 1);
                dateTo.WebCalendar.SelectedDate = DateTime.Now;

                warp.AddLinkedRequestTrigger(gridLogin);
                warp.AddRefreshTarget(gridDetail);
                warp.AddRefreshTarget(LabelTableCaption2);
            }

            if (!warp.IsAsyncPostBack)
            {
                FillDataTable();
            }

            Page.Title = string.Format("Статистика посещения сайта");
            LabelTitle.Text = string.Format("{0} за период с {1} до {2}",
                Page.Title, dateFrom.WebCalendar.SelectedDate,
                dateTo.WebCalendar.SelectedDate);
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int dirtyWidth = Convert.ToInt32(CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30).Value);
            int dirtyHeight = Convert.ToInt32(CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 300).Value);

            LabelTitle.Width = dirtyWidth - 50;

            gridLogin.Width = 1040;
            gridLogin.Height = Unit.Empty;

            gridDetail.Width = 930;
            gridDetail.Height = 200;
            warp.Height = 260;

            gridReferer.Height = 400;
            gridReferer.Width = dirtyWidth / 2 - 10;

            chartResolution.Height = 300;
            chartResolution.Width = dirtyWidth / 3;

            chartOS.Width = chartResolution.Width;
            chartOS.Height = chartResolution.Height;
            chartBrowser.Width = chartResolution.Width;
            chartBrowser.Height = chartResolution.Height;

            gridPage.Height = gridReferer.Height;
            gridPage.Width = gridReferer.Width;

            chartMonth.Width = LabelTitle.Width;
            chartMonth.Height = 250;

            chartYear.Width = chartMonth.Width;
            chartYear.Height = chartMonth.Height;

            chartDay.Width = chartMonth.Width;
            chartDay.Height = chartMonth.Height;

            gridDetail.CellPadding = 3;
            gridLogin.CellPadding = 3;
            gridReferer.CellPadding = 3;
            gridPage.CellPadding = 3;
        }

        // Глобально использующийся фильтр по диапазаону дат из параметров
        protected virtual string GetDateFilter()
        {
            int nextDay = dateTo.WebCalendar.SelectedDate.Day + 1;
            int nextMonth = dateTo.WebCalendar.SelectedDate.Month;
            int nextYear = dateTo.WebCalendar.SelectedDate.Year;

            if (nextDay > DateTime.DaysInMonth(nextYear, nextMonth))
            {
                nextDay = 1;
                nextMonth++;
                if (nextMonth > 12)
                {
                    nextMonth = 1;
                    nextYear++;
                }
            }

            return string.Format(" and (data > CDate('{0}/{1}/{2}') and data < CDate('{3}/{4}/{5}')) ",
                dateFrom.WebCalendar.SelectedDate.Day,
                dateFrom.WebCalendar.SelectedDate.Month,
                dateFrom.WebCalendar.SelectedDate.Year,
                nextDay,
                nextMonth,
                nextYear);
        }

        protected virtual void FillDataTable()
        {
            // Колонки выодимого в грид датасета
            DataColumn dataColumn = dtMain.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.Caption = "Логин";

            dataColumn = dtMain.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.Caption = "Визиты";

            dataColumn = dtMain.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.Caption = "Страницы";

            dataColumn = dtMain.Columns.Add();
            dataColumn.DataType = Type.GetType("System.DateTime");
            dataColumn.Caption = "Последнее подключение";

            dataColumn = dtMain.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.Caption = "Последний IP";

            dataColumn = dtMain.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.Caption = "Браузер";
            
            dataColumn = dtMain.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.Caption = "ОС";

            dataColumn = dtMain.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.Caption = "Разрешение";

            // Вытаскиваем все логины, под которыми заходили на сайт за историю его существования
            DataTable dtLogins = GetStatData(string.Format("SELECT distinct login FROM tblSt_Detail where login <> '' {0} order by login ", GetDateFilter()));

            for (int i = 0; i < dtLogins.Rows.Count; i++)
            {
                DataRow drMain = dtMain.Rows.Add();
                drMain[0] = dtLogins.Rows[i][0];

                // Дата последнего подключения и адрес для каждого логина
                DataTable dtLoginInfo = GetStatData(
                    string.Format("SELECT ip, data, browser, os, reso  FROM tblSt_Detail where login = '{0}' {1} order by data desc",
                        drMain[0].ToString(), GetDateFilter()));

                if (dtLoginInfo.Rows.Count > 0)
                {
                    drMain[3] = dtLoginInfo.Rows[0][1];
                    drMain[4] = dtLoginInfo.Rows[0][0];

                    drMain[5] = dtLoginInfo.Rows[0][2];
                    drMain[6] = dtLoginInfo.Rows[0][3];
                    drMain[7] = dtLoginInfo.Rows[0][4];
                }

                // Число посещений за период под этим логином
                DataTable dtLoginCountVisit = GetStatData(
                    string.Format("SELECT visitor_id FROM tblSt_Detail where login = '{0}' {1} group by visitor_id",
                        drMain[0].ToString(), GetDateFilter()));

                drMain[1] = dtLoginCountVisit.Rows.Count;
                // Число посещенных страниц за период под этим логином
                DataTable dtLoginCountPages = GetStatData(
                    string.Format("SELECT count(details_id) FROM tblSt_Detail where login = '{0}' {1}",
                        drMain[0].ToString(), GetDateFilter()));
                if (dtLoginCountPages.Rows.Count > 0)
                {
                    drMain[2] = dtLoginCountPages.Rows[0][0];
                }
            }
            dtMain = SGMDataObject.SortDataSet(dtMain, 3, true);

            gridLogin.DataSource = dtMain;
            gridLogin.DataBind();

            gridDetail.Height = 200;

            if (dtMain.Rows.Count > 0)
            {
                RefreshDetail(dtMain.Rows[0][0].ToString(), dtMain.Rows[0][4].ToString());
            }

            CRHelper.FillCustomColorModel(chartBrowser, 30, true);
            CRHelper.FillCustomColorModel(chartResolution, 30, true);
            CRHelper.FillCustomColorModel(chartOS, 30, true);

            // Заполняем графики данными
            FillChart(chartResolution, "reso");
            FillChart(chartBrowser, "browser");
            FillChart(chartOS, "os");

            // А теперь и гриды переходов и посещаемых страниц
            FillGrid(gridReferer, "referer", "Источник переходов", "Число переходов");
            FillGrid(gridPage, "page", "Имя страницы", "Число посещений");

            int curYear = DateTime.Now.Year;
            int prevYear = DateTime.Now.Year - 1;

            int curMonth = DateTime.Now.Month;
            int prevMonth = DateTime.Now.Month - 1;

            int curDay = DateTime.Now.Day;
            int curDay2 = curDay - 1;
            int curMonth2 = curMonth;
            int curYear2 = curYear;

            int nextDay = curDay + 1;
            int nextMonth = curMonth;
            int nextYear = curYear;

            int curYear3 = curYear;
            if (prevMonth == 0)
            {
                prevMonth = 12;
                curYear3 = prevYear;
            }

            if (curDay2 == 0)
            {
                curMonth2 = curMonth2 - 1;
                if (curMonth2 == 0)
                {
                    curMonth2 = 12;
                    curYear2 = prevYear;
                }
                curDay2 = DateTime.DaysInMonth(curYear2, curMonth2);
            }

            if (nextDay > DateTime.DaysInMonth(curYear, curMonth))
            {
                nextDay = 1;
                nextMonth++;
                if (nextMonth > 12)
                {
                    nextMonth = 1;
                    nextYear++;
                }
            }

            // день
            LabelPagesToday.Text = GetPagesCount(curDay, curMonth, curYear,
                nextDay, nextMonth, nextYear);
            LabelVisitorsToday.Text = GetVisitorCount(curDay, curMonth, curYear,
                nextDay, nextMonth, nextYear);
            LabelPagesYesterday.Text = GetPagesCount(curDay2, curMonth2, curYear2,
                curDay, curMonth, curYear);
            LabelVisitorsYesterday.Text = GetVisitorCount(curDay2, curMonth2, curYear2,
                curDay, curMonth, curYear);

            // месяц
            LabelPagesThisMonth.Text = GetPagesCount(1, curMonth, curYear,
                nextDay, nextMonth, nextYear);
            LabelVisitorsThisMonth.Text = GetVisitorCount(1, curMonth, curYear,
                nextDay, nextMonth, nextYear);

            LabelPagesLastMonth.Text = GetPagesCount(1, prevMonth, curYear3,
                1, curMonth, curYear);
            LabelVisitorsLastMonth.Text = GetVisitorCount(1, prevMonth, curYear3,
                1, curMonth, curYear);

            // год
            LabelPagesThisYear.Text = GetPagesCount(1, 1, curYear, 1, 1, curYear + 1);
            LabelVisitorsThisYear.Text = GetVisitorCount(1, 1, curYear, 1, 1, curYear + 1);

            LabelPagesLastYear.Text = GetPagesCount(1, 1, prevYear, 1, 1, curYear);
            LabelVisitorsLastYear.Text = GetVisitorCount(1, 1, prevYear, 1, 1, curYear);

            LabelPagesThisMonth.Font.Bold = true;
            LabelVisitorsThisMonth.Font.Bold = true;
            LabelPagesLastMonth.Font.Bold = true;
            LabelVisitorsLastMonth.Font.Bold = true;
            LabelPagesToday.Font.Bold = true;
            LabelVisitorsToday.Font.Bold = true;
            LabelPagesYesterday.Font.Bold = true;
            LabelVisitorsYesterday.Font.Bold = true;
            LabelPagesThisYear.Font.Bold = true;
            LabelVisitorsThisYear.Font.Bold = true;
            LabelPagesLastYear.Font.Bold = true;
            LabelVisitorsLastYear.Font.Bold = true;

            LabelCaptionPagesThisMonth.Text = string.Format("Страниц в {0}", CRHelper.RusMonthPrepositional(curMonth));
            LabelCaptionVisitorsThisMonth.Text = string.Format("Посетителей в {0}", CRHelper.RusMonthPrepositional(curMonth));
            LabelCaptionPagesLastMonth.Text = string.Format("Страниц в {0}", CRHelper.RusMonthPrepositional(prevMonth));
            LabelCaptionVisitorsLastMonth.Text = string.Format("Посетителей в {0}", CRHelper.RusMonthPrepositional(prevMonth));
            LabelCaptionPagesToday.Text = string.Format("Страниц {0} {1}", DateTime.Now.Day, CRHelper.RusMonthAblative(curMonth));
            LabelCaptionVisitorsToday.Text = string.Format("Посетителей {0} {1}", DateTime.Now.Day, CRHelper.RusMonthAblative(curMonth));
            LabelCaptionPagesYesterday.Text = string.Format("Страниц {0} {1}", curDay2, CRHelper.RusMonthAblative(curMonth2));
            LabelCaptionVisitorsYesterday.Text = string.Format("Посетителей {0} {1}", curDay2, CRHelper.RusMonthAblative(curMonth2));
            LabelCaptionPagesThisYear.Text = string.Format("Страниц в {0} году", curYear);
            LabelCaptionVisitorsThisYear.Text = string.Format("Посетителей в {0} году", curYear);
            LabelCaptionPagesLastYear.Text = string.Format("Страниц в {0} году", prevYear);
            LabelCaptionVisitorsLastYear.Text = string.Format("Посетителей в {0} году", prevYear);

            FillMonthChartData();
            FillYearChartData();
            FillDayChartData();

            chartMonth.Border.Thickness = 0;
            chartYear.Border.Thickness = 0;
            chartDay.Border.Thickness = 0;

            LabelCommonCaption.Text = "Общая информация";
            LabelTableCaption1.Text = "Информация по пользователям";
        }

        // Глобально использующийся фильтр по диапазону дат из параметров
        protected virtual string GetDateFilter(int sDay, int sMonth, int sYear, int eDay, int eMonth, int eYear)
        {
            return string.Format("(data > CDate('{0}/{1}/{2}') and data < CDate('{3}/{4}/{5}')) ",
                sDay, sMonth, sYear, eDay, eMonth, eYear);
        }

        protected virtual string GetPagesCount(int sDay, int sMonth, int sYear, int eDay, int eMonth, int eYear)
        {
            DataTable dt = GetStatData(
                string.Format("SELECT count(details_id) from tblSt_detail where {0} ",
                    GetDateFilter(sDay, sMonth, sYear, eDay, eMonth, eYear)));

            return dt.Rows[0][0].ToString();
        }

        protected virtual string GetVisitorCount(int sDay, int sMonth, int sYear, int eDay, int eMonth, int eYear)
        {
            DataTable dt = GetStatData(
                string.Format("SELECT distinct visitor_id from tblSt_detail where {0} ",
                    GetDateFilter(sDay, sMonth, sYear, eDay, eMonth, eYear)));
            
            return dt.Rows.Count.ToString();
        }

        protected virtual void FillMonthChartData()
        {
            // Колонки выодимого в грид датасета
            DataTable dtChartData = new DataTable();
            DataColumn dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Месяц";

            dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Посетители";

            dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Страницы";

            for (int i = 1; i < 13; i++ )
            {
                DataRow dr = dtChartData.Rows.Add();
                dr[0] = CRHelper.RusMonth(i);
                dr[1] = GetVisitorCount(1, i, DateTime.Now.Year, DateTime.DaysInMonth(DateTime.Now.Year, i), i, DateTime.Now.Year);
                dr[2] = GetPagesCount(1, i, DateTime.Now.Year, DateTime.DaysInMonth(DateTime.Now.Year, i), i, DateTime.Now.Year);
            }
            chartMonth.Tooltips.FormatString = string.Format("<SERIES_LABEL> {0} года <ITEM_LABEL> - <DATA_VALUE:N0>", DateTime.Now.Year);

            chartMonth.DataSource = dtChartData;
            chartMonth.DataBind();
            ConfigureChart(chartMonth);

            LabelChartMonthCaption.Text = string.Format("Месячная динамика {0} год", DateTime.Now.Year);
        }

        protected virtual void FillYearChartData()
        {
            // Колонки выодимого в грид датасета
            DataTable dtChartData = new DataTable();
            DataColumn dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Год";

            dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Посетители";

            dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Страницы";

            for (int i = 2009; i < DateTime.Now.Year + 1; i++)
            {
                DataRow dr = dtChartData.Rows.Add();
                dr[0] = i;
                dr[1] = GetVisitorCount(1, 1, i, 31, 12, i);
                dr[2] = GetPagesCount(1, 1, i, 31, 12, i);
            }
            for (int i = dtChartData.Rows.Count; i < 12; i++)
            {
                DataRow dr = dtChartData.Rows.Add();
                dr[0] = 2009 + i;
                dr[1] = 0;
                dr[2] = 0;
            }

            chartYear.Tooltips.FormatString = "<SERIES_LABEL> год <ITEM_LABEL> - <DATA_VALUE:N0>";

            chartYear.DataSource = dtChartData;
            chartYear.DataBind();
            ConfigureChart(chartYear);
            LabelChartYearCaption.Text = "Годовая динамика";
        }

        protected virtual void FillDayChartData()
        {
            // Колонки выодимого в грид датасета
            DataTable dtChartData = new DataTable();
            DataColumn dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Число";

            dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Посетители";

            dataColumn = dtChartData.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Страницы";

            for (int i = 1; i < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                DataRow dr = dtChartData.Rows.Add();
                dr[0] = i;
                dr[1] = GetVisitorCount(i, DateTime.Now.Month, DateTime.Now.Year, i + 1, DateTime.Now.Month, DateTime.Now.Year);
                dr[2] = GetPagesCount(i, DateTime.Now.Month, DateTime.Now.Year, i + 1, DateTime.Now.Month, DateTime.Now.Year);
            }

            chartDay.Tooltips.FormatString = string.Format("<SERIES_LABEL> {0} {1} года <ITEM_LABEL> - <DATA_VALUE:N0>", CRHelper.RusMonthAblative(DateTime.Now.Month), DateTime.Now.Year);

            chartDay.DataSource = dtChartData;
            chartDay.DataBind();
            ConfigureChart(chartDay);

            LabelChartDayCaption.Text = string.Format("Динамика {0} {1} года", CRHelper.RusMonth(DateTime.Now.Month), DateTime.Now.Year);
        }

        protected virtual void ConfigureChart(Infragistics.WebUI.UltraWebChart.UltraChart chart)
        {
            chart.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Right;
            chart.Legend.SpanPercentage = 10;
            chart.Legend.Margins.Bottom = 150;
            chart.Legend.Visible = true;
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.X.Extent = 30;
            chart.Axis.Y.Extent = 50;
        }

        protected virtual void RefreshDetail(string login, string ip)
        {
            // Т.к. все ходят под гостем, то ограничим многтысячную выборку только 50 записями
            DataTable dtDetail = GetStatData(
                string.Format("select top 50 ip, min(data), max(data), count(details_id), max(browser), max(os), max(reso)  from tblst_detail where login = '{0}' {1} group by visitor_id, ip order by 2 desc, 3 desc",
                    login, GetDateFilter()));

            dtDetail.Columns[0].ColumnName = "IP-Адрес";
            dtDetail.Columns[1].ColumnName = "Сеанс начат";
            dtDetail.Columns[2].ColumnName = "Сеанс закончен";
            dtDetail.Columns[3].ColumnName = "Cтраницы";
            dtDetail.Columns[4].ColumnName = "Браузер";
            dtDetail.Columns[5].ColumnName = "ОС";
            dtDetail.Columns[6].ColumnName = "Разрешение";

            gridDetail.DataSource = dtDetail;
            gridDetail.DataBind();

            gridDetail.Columns[0].Width = 90;
            gridDetail.Columns[1].Width = 115;
            gridDetail.Columns[2].Width = 115;
            gridDetail.Columns[3].Width = 70;
            gridDetail.Columns[4].Width = 170;
            gridDetail.Columns[5].Width = 170;
            gridDetail.Columns[6].Width = 80;

            gridDetail.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridDetail.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridDetail.Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridDetail.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            gridDetail.Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridDetail.Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridDetail.Columns[6].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            LabelTableCaption2.Text = string.Format("Визиты пользователя {0} (последние 50 посещений)", login);
        }

        protected virtual void FillGrid(UltraWebGrid grid, string fieldName, string columnCaption1, string columnCaption2)
        {
            DataTable dt = GetStatData(
                string.Format("SELECT {0}, count({0}) from tblSt_detail where {0} <> '' {1} group by {0} order by 2 desc ",
                    fieldName, GetDateFilter()));

            dt.Columns[0].ColumnName = columnCaption1;
            dt.Columns[1].ColumnName = columnCaption2;

            if (dt.Rows.Count > 0)
            {
                grid.Visible = true;
                grid.DataSource = dt;
                grid.DataBind();
            }
            else
            {
                grid.Visible = false;
            }
        }
        
        protected virtual void FillChart(Infragistics.WebUI.UltraWebChart.UltraChart chart, string fieldName)
        {
            DataTable dt = GetStatData(
                string.Format("SELECT {0}, count({0}) from tblSt_detail where {0} <> '' {1} group by {0} order by 2 desc ",
                    fieldName, GetDateFilter()));

            chart.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Right;
            chart.Legend.SpanPercentage = 45;
            chart.Legend.Visible = true;
            chart.PieChart.OthersCategoryPercent = 0.5;
            chart.Border.Thickness = 0;
            chart.Tooltips.FormatString = "<ITEM_LABEL> - <DATA_VALUE>";

            if (dt.Rows.Count > 0)
            {
                chart.Visible = true;
                chart.DataSource = dt;
                chart.DataBind();
            }
            else
            {
                chart.Visible = false;
            }
        }

        // Выполнятель запросов
        protected virtual DataTable GetStatData(string command)
        {
            string Connectionstring = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Persist Security Info=False", basePath);

            if (!baseFounded) return new DataTable();

            OleDbConnection con = new OleDbConnection(Connectionstring);
            con.Open();

            OleDbCommand cmd = new OleDbCommand(command, con);

            OleDbDataAdapter sda = new OleDbDataAdapter(cmd);

            DataSet dt = new DataSet();

            sda.Fill(dt);
            con.Close();

            return dt.Tables[0];
        }


        protected void grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            gridLogin.Columns[0].Width = 170;
            gridLogin.Columns[1].Width = 60;
            gridLogin.Columns[2].Width = 70;
            gridLogin.Columns[3].Width = 120;
            gridLogin.Columns[4].Width = 90;

            gridLogin.Columns[5].Width = 170;
            gridLogin.Columns[6].Width = 170;
            gridLogin.Columns[7].Width = 80;

            gridLogin.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridLogin.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridLogin.Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridLogin.Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            gridLogin.Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridLogin.Columns[6].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            gridLogin.Columns[7].CellStyle.HorizontalAlign = HorizontalAlign.Right;


            RowLayoutColumnInfo rlcInfo;
            for (int i = 0; i < gridLogin.Columns.Count; i++)
            {
                rlcInfo = gridLogin.Columns[i].Header.RowLayoutColumnInfo;
                if (i < 5)
                {
                    rlcInfo.OriginY = 0;
                    rlcInfo.SpanY = 2;
                }
                else
                {
                    rlcInfo.OriginY = 1;
                }
            }

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Характеристики подключения";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 5;
            ch.RowLayoutColumnInfo.SpanX = 3;
            ch.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }

        protected void gridLinks_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Grid.Columns[0].Width = (int)e.Layout.Grid.Width.Value - 140;
            e.Layout.Grid.Columns[1].Width = 70;
            e.Layout.Grid.Columns[1].Header.Caption = "Переходы";
            e.Layout.Grid.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
        }

        protected void gridLogin_ActiveRowChange(object sender, RowEventArgs e)
        {
            RefreshDetail(e.Row.Cells[0].ToString(), e.Row.Cells[4].ToString());
        }
        
        protected void gridLogin_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[4].TargetURL = string.Format("@[_blank]http://geotool.flagfox.net/?ip={0}", e.Row.Cells[4].Value);
        }

        protected void gridDetail_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].TargetURL = string.Format("@[_blank]http://geotool.flagfox.net/?ip={0}", e.Row.Cells[0].Value);
        }

        protected void gridDetail_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
        }
    }
}
