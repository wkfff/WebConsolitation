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
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using System.Xml;
using System.IO;
using Dundas.Maps.WebControl;
using System.Collections;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebChart;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FW_0003_0002_Horizontal : CustomReportPage
    {
        private GridHeaderLayout headerLayout;
        private DateTime date;
        private Collection<string> moCollection = new Collection<string>();

        private DataSet LoadData()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/iphone/FW_0003_0002/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //DataSet ds = LoadData();
            //DataTable dtDate = ds.Tables["date"];
            //DataTable dtForestry = ds.Tables["forestry"];

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FW_0003_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();

            date = CRHelper.PeriodDayFoDate(UserParams.PeriodCurrentDate.Value);

            DataTable dtForestry = new DataTable();
            query = DataProvider.GetQueryText("FW_0003_0002_data_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtForestry);

            //date = Convert.ToDateTime(dtDate.Rows[0]["curretnDate"]);

            foreach (DataRow row in dtForestry.Rows)
            {
                string mo = row["stateId"].ToString();
                if (!(moCollection.Contains(mo)))
                {
                    moCollection.Add(mo);
                }
            }

            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            DataTable source = dtForestry;
            //source.Columns.RemoveAt(0);

            for (int i = source.Rows.Count - 1; i > source.Rows.Count - 7; i--)
            {
                source.Rows[i][0] = String.Format("В {0} г. на {1:dd.MM}", date.AddYears(source.Rows.Count - i - 7).Year, date);
            }

            DataRow totalRow = source.Rows[source.Rows.Count - 7];

            double total = 0;
            if (totalRow[4] != DBNull.Value &&
                totalRow[4].ToString() != String.Empty)
            {
                total += Convert.ToDouble(totalRow[4]);
            }

            if (totalRow[6] != DBNull.Value &&
                totalRow[6].ToString() != String.Empty)
            {
                total += Convert.ToDouble(totalRow[6]);
            }

            //if (total < 1)
            //{
            //    mainDiv.Style.Add("background-image", "url(../../../images/ForestTree.png)");
            //}
            //else if (total < 10)
            //{
            //    mainDiv.Style.Add("background-image", "url(../../../images/ForestFireMiddle.png)");
            //}
            //else
            //{
            //    mainDiv.Style.Add("background-image", "url(../../../images/flame.png)");
            //}
            source.AcceptChanges();

            SetupGrid(UltraWebGrid1, source);
            //UltraWebGrid1.DisplayLayout.Bands[0].HeaderLayout.Clear();

            lbDescription.Text = String.Format("Ежедневная сводная ведомость лесных пожаров в лесном фонде Ханты-Мансийского авт. округа – Югры в разрезе территориальных отделов по состоянию на 9:00 местного времени&nbsp;<span class='DigitsValue'>{0}</span>", date.AddDays(1).ToShortDateString());

            DataRow rowTotal = dtForestry.Rows[dtForestry.Rows.Count - 7];

            //Label1.Text = String.Format("действующие пожары<br/>на 9:00 {0:dd.MM}:&nbsp;<div style='position: relative; top: -22px; text-align: center; left: 123px;  width: 40px; height: 23px; background-image: url(../../../images/jawRed.png); background-position: left top; background-repeat: no-repeat'>{1:N0}</div>", date.AddDays(1), rowTotal["Действует пожаров "]);
            //Label2.Text = String.Format("в т.ч. локализовано<br/>за {0:dd.MM}:&nbsp;<div style='position: relative; top: -22px; text-align: center; left: 80px;  width: 40px; height: 23px; background-image: url(../../../images/jawYellow.png); background-position: left top; background-repeat: no-repeat'>{1:N0}</div>", date, rowTotal["Локализовано пожаров "]);
            //Label3.Text = String.Format("ликвидировано<br/>с начала года:&nbsp;<div style='position: relative; top: -22px; text-align: center; left: 133px;  width: 40px; height: 23px; background-image: url(../../../images/jawGreen.png); background-position: left top; background-repeat: no-repeat'>{1:N0}</div>", date, rowTotal["Ликвидировано пожаров "]);

            //string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/FW_0003_0002/") + "TouchElementBounds.xml";
            //Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/FW_0003_0002/"));
            //File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"FO_0003_0002_{0}\" bounds=\"x=0;y=120;width=768;height=260\" openMode=\"incomes\"/><element id=\"FO_0003_0004_{0}\" bounds=\"x=0;y=380;width=768;height=230\" openMode=\"outcomes\"/><element id=\"FO_0003_0005_{0}\" bounds=\"x=0;y=610;width=768;height=100\" openMode=\"rests\"/><element id=\"FO_0003_0006_{0}\" bounds=\"x=0;y=710;width=768;height=130\" openMode=\"\"/></touchElements>", CustomParams.GetSubjectIdByName(UserParams.StateArea.Value)));

        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().Contains("Итого"))
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            else if (e.Row.Cells[0].Value.ToString().Contains(" г. на "))
            {
                e.Row.Cells[4].ColSpan = 12;
            }
            else if (!(e.Row.Cells[16].Value == null ||
                e.Row.Cells[16].Value.ToString() == String.Empty))
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            else
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                    e.Row.Style.Font.Bold = true;
                    e.Row.Style.ForeColor = Color.White;
                }
            }

            //if (e.Row.Index < 7)
            //{
            //    e.Row.Cells[0].Style.ForeColor = Color.Black;
            //}
        }

        private void SetupGrid(UltraWebGrid grid, DataTable dt)
        {
            grid.Width = Unit.Empty;
            grid.Height = Unit.Empty;
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);

            grid.DisplayLayout.HeaderStyleDefault.BackColor = Color.Transparent;
            grid.DisplayLayout.RowStyleDefault.BackColor = Color.Transparent;
            grid.DisplayLayout.FrameStyle.BackColor = Color.Transparent;

            grid.DataSource = dt;
            grid.DataBind();
        }

        void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            e.Layout.CellClickActionDefault = CellClickAction.NotSet;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.White;

            //e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.White;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "N1");

            e.Layout.Bands[0].Columns[0].Width = 174;
            e.Layout.Bands[0].Columns[1].Width = 54;
            e.Layout.Bands[0].Columns[2].Width = 78;
            e.Layout.Bands[0].Columns[3].Width = 78;
            e.Layout.Bands[0].Columns[4].Width = 54;

            e.Layout.Bands[0].Columns[5].Width = 78;
            e.Layout.Bands[0].Columns[6].Width = 54;
            e.Layout.Bands[0].Columns[7].Width = 78;
            e.Layout.Bands[0].Columns[8].Width = 55;
            e.Layout.Bands[0].Columns[9].Width = 79;

            e.Layout.Bands[0].Columns[10].Width = 110;
            e.Layout.Bands[0].Columns[11].Width = 110;
            e.Layout.Bands[0].Columns[12].Width = 110;
            e.Layout.Bands[0].Columns[13].Width = 110;
            e.Layout.Bands[0].Columns[14].Width = 110;
            e.Layout.Bands[0].Columns[15].Width = 110;

            e.Layout.Bands[0].Columns[16].Hidden = true;

            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[6].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[7].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[8].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[9].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[10].CellStyle.BorderDetails.WidthLeft = 3;

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("Территориальный отдел – лесничество");

            GridHeaderCell cell = headerLayout.AddCell("Всего с начала года");
            cell.AddCell("кол-во");
            GridHeaderCell childCell = cell.AddCell("охваченная площадь");
            childCell.AddCell("лес (га)");
            childCell.AddCell("нелес (га)");

            cell = headerLayout.AddCell(String.Format("Из них за {0}", date.ToShortDateString()));

            childCell = cell.AddCell("действует");
            childCell.AddCell("кол-во");
            childCell.AddCell("лес (га)");

            childCell = cell.AddCell("в т.ч. локализовано");
            childCell.AddCell("кол-во");
            childCell.AddCell("лес (га)");

            childCell = cell.AddCell("ликвидировано");
            childCell.AddCell("кол-во");
            childCell.AddCell("лес (га)");

            cell = headerLayout.AddCell("Работает на день составления сводки<span style=\"color: black\"><span>");
            cell.AddCell("Воздушные суда");
            cell.AddCell("Пожарно-химические станции");
            cell.AddCell("Авиационно-пожарные службы");
            cell.AddCell("Бульдозеры");
            cell.AddCell("Тралы");
            cell.AddCell("Автомобили");

            headerLayout.ApplyHeaderInfo();
        }
    }
}
