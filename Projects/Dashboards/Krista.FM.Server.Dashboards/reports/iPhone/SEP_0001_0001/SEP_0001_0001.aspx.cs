using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.IO;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;  
using Infragistics.WebUI.UltraWebGrid;
using System.Web.UI.HtmlControls; 
using Primitive = Infragistics.UltraChart.Core.Primitives.Primitive;
using Text = Infragistics.UltraChart.Core.Primitives.Text;
using System.Web; 
namespace Krista.FM.Server.Dashboards.reports.iPad
{ 
    public partial class SEP_0001_0001 : CustomReportPage
    {
        private string date = ""; 
        protected override void Page_Load(object sender, EventArgs e)
        { 
            base.Page_Load(sender, e); 

            string multitouchIcon = String.Empty; 
             
            multitouchIcon = "<img src='../../../images/detail.png'>";
            detalizationIconDiv.InnerHtml = String.Format("<a href='https://ias-mo.admsakhalin.ru/reports/SEP_0001_ComplexSahalin/default.aspx?login=test&password=test'>{0}</a>", multitouchIcon);
            lbUrl.Text = "Исходные показатели, используемые для расчета комплексной оценки социально-экономического развития";
            lbDescription.Text = @"<center>Комплексная оценка социально-экономического развития</center>
   <center>&nbsp;А Высокая&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;В Средняя&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;С Низкая</center>";
            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Сахалинская область";
            }

            HeraldImageContainer.InnerHtml = "<img style='margin-left: -30px; margin-right: 20px; height: 100px' src=\"../../../images/Heralds/65.png\"></a>";

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("SEP_0001_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);
            if (Krista.FM.Server.Dashboards.Components.UserComboBox.getLastBlock(dtDate.Rows[0][1].ToString()) == "Квартал 4")
            {
                UserParams.Subject.Value = "Годовой";
            } 
            else
            {
                UserParams.Subject.Value = "Квартальный";
            }
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            date = Krista.FM.Server.Dashboards.Components.UserComboBox.getLastBlock(dtDate.Rows[0][1].ToString()).Split(' ')[1] + " квартал " + dtDate.Rows[0][1].ToString().Split(']')[3].TrimStart('.').TrimStart('[') + " года";
            FillShortRegionsNames();
            InitializeTable1();
            InitializeTable3();
            InitializeTable2();
            MakeHtmlTableDetailTable();
            BindStatisticsText();
    
        }
         
        private void BindStatisticsText()
        {
            string buf = "";
            string buf1 = "";
            buf = date.Split(' ')[0];
            switch (buf)
            {
                case "1": buf1 = "1 квартал "+date.Split(' ')[2];  break;
                case "2": buf1 = "2 квартала " + date.Split(' ')[2]; break;
                case "3": buf1 = "3 квартала " + date.Split(' ')[2]; break;
                case "4": buf1 = "4 квартала " + date.Split(' ')[2]; break;
            }
            string buf2 = dtGrid2.Rows[0][dtGrid2.Rows[1].ItemArray.Length - 1].ToString().Replace("<br>", ",").TrimEnd(',').Replace("(", String.Empty).Replace(")", String.Empty).Replace(";", String.Empty).Replace("1", String.Empty).Replace("2", String.Empty).Replace("3", String.Empty).Replace("4", String.Empty).Replace("5", String.Empty).Replace("6", String.Empty).Replace("7", String.Empty).Replace("8", String.Empty).Replace("9", String.Empty);
            string buf3 = dtGrid2.Rows[dtGrid2.Rows.Count - 1][1].ToString().Replace("<br>", ",").TrimEnd(',').Replace("(", String.Empty).Replace(")", String.Empty).Replace(";", String.Empty).Replace("1", String.Empty).Replace("2", String.Empty).Replace("3", String.Empty).Replace("4", String.Empty).Replace("5", String.Empty).Replace("6", String.Empty).Replace("7", String.Empty).Replace("8", String.Empty).Replace("9", String.Empty);
             
            
            lbInfo.Text = String.Format(
                "По результатам комплексной оценки социально-экономического развития за<br><b>{0}</b> года наблюдалось<br>" +
                "<img src='../../../images/ballGreenBB.png'>&nbsp;&nbsp;высокая оценка в следующих муниципальных образованиях: <br><b>{1}</b><br>" +
                "<img src='../../../images/ballRedBB.png'>&nbsp;&nbsp;низкая оценка в следующих муниципальных образованиях: <br><b>{2}</b>",
                buf1, buf2, buf3).Replace(" ,", ", ");

        }

        #region Таблица1 
        private DataTable dtGrid1;

        private void InitializeTable1()
        {
            
            UltraWebGrid1.InitializeLayout += new  InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            dtGrid1 = new DataTable();
            string query = DataProvider.GetQueryText("SEP_0001_0001_table1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid1);
            calculateRank(dtGrid1, 1, 2);
           // calculateRank(dt, 3, 4);
            for (int i = 0; i < dtGrid1.Rows.Count; i++)
            {
                if (GetNumber(dtGrid1.Rows[i][2].ToString()) >= 1 && GetNumber(dtGrid1.Rows[i][2].ToString()) <= 6)
                {
                    dtGrid1.Rows[i][4] = "A";
                }
                else
                {
                    if (GetNumber(dtGrid1.Rows[i][2].ToString()) >= 7 && GetNumber(dtGrid1.Rows[i][2].ToString()) <= 13)
                    {
                        dtGrid1.Rows[i][4] = "B";
                    }
                    else
                    {
                        if (GetNumber(dtGrid1.Rows[i][2].ToString()) >= 14 && GetNumber(dtGrid1.Rows[i][2].ToString()) <= 19)
                        {
                            dtGrid1.Rows[i][4] = "C";
                        }
                    }
                }

            }

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 4; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dtGrid1.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                string gaugeValue = String.Format("{0:N2}", dtGrid1.Rows[i][1]);
                row[0] = dtGrid1.Rows[i][0];
                row[1] = String.Format("Итоговая сводная оценка {0}<br>Ранг {1}", String.Format("{0:##0.000}", Convert.ToDouble(dtGrid1.Rows[i][1])), dtGrid1.Rows[i][2]);
                row[2] = GetGaugeUrl(dtGrid1.Rows[i][1], dtGrid1.Rows[i][2]) + "<span style=\'float:left\'>0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1</span>";
                row[3] = dtGrid1.Rows[i][4].ToString();
                dtSource.Rows.Add(row);
            }


            UltraWebGrid1.DataSource = dtSource;
            UltraWebGrid1.DataBind();
        }

        

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {

        } 

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;
            

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 16;
            //e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[0].Width = 310;
            e.Layout.Bands[0].Columns[1].Width = 250;
            e.Layout.Bands[0].Columns[2].Width = 130;
            e.Layout.Bands[0].Columns[3].Width = 50;

            e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].Columns[0].Header);
            e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].Columns[1].Header);
            e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].Columns[2].Header);
            e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].Columns[3].Header);
        }

        #endregion

        #region Таблица3
        DataTable dtGrid3;
        private void InitializeTable3()
        {

            UltraWebGrid3.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid3_InitializeLayout);
            //UltraWebGrid3.InitializeRow += new InitializeRowEventHandler(UltraWebGrid3_InitializeRow);

            dtGrid3 = new DataTable();
            string query = DataProvider.GetQueryText("SEP_0001_0001_table3");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid3);
            calculateRank(dtGrid3, 1, 2);
            for (int i = 0; i < dtGrid3.Rows.Count; i++)
            {

                if (GetNumber(dtGrid3.Rows[i][2].ToString()) >= 1 && GetNumber(dtGrid3.Rows[i][2].ToString()) <= 6)
                {
                    dtGrid3.Rows[i][4] = "+";

                }
                else
                {
                    if (GetNumber(dtGrid3.Rows[i][2].ToString()) >= 7 && GetNumber(dtGrid3.Rows[i][2].ToString()) <= 13)
                    {
                        dtGrid3.Rows[i][4] = "±";
                    }
                    else
                    {
                        if (GetNumber(dtGrid3.Rows[i][2].ToString()) >= 14 && GetNumber(dtGrid3.Rows[i][2].ToString()) <= 19)
                        {
                            dtGrid3.Rows[i][4] = "-";
                        }
                    }
                }
            }

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 4; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dtGrid3.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                string gaugeValue = String.Format("{0:N2}", dtGrid3.Rows[i][1]);
                row[0] = dtGrid3.Rows[i][0];
                row[1] = String.Format("Интегральная оценка населением {0}<br>Ранг {1}", String.Format("{0:##0.000}", Convert.ToDouble(dtGrid3.Rows[i][1])), dtGrid3.Rows[i][2]);
                row[2] = GetGaugeUrl(dtGrid3.Rows[i][1], dtGrid3.Rows[i][2]) +  "<span style=\'float:left\'>0&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1</span>";
                row[3] = dtGrid3.Rows[i][4].ToString();
                dtSource.Rows.Add(row);
            }
            UltraWebGrid3.DataSource = dtSource;
            UltraWebGrid3.DataBind();
        }

        void UltraWebGrid3_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;


            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 16;
            //e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[0].Width = 310;
            e.Layout.Bands[0].Columns[1].Width = 250;
            e.Layout.Bands[0].Columns[2].Width = 130;
            e.Layout.Bands[0].Columns[3].Width = 50;

            e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].Columns[0].Header);
            e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].Columns[1].Header);
            e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].Columns[2].Header);
            e.Layout.Bands[0].HeaderLayout.Remove(e.Layout.Bands[0].Columns[3].Header);
        }

        #endregion

        #region Таблица2

        DataTable dtGrid2;
        private void InitializeTable2()
        {
            DataTable dtSource = new DataTable();
            dtGrid2 = new DataTable();
            dtSource.Columns.Add(" ", typeof(string));
            dtSource.Columns.Add("-", typeof(string));
            dtSource.Columns.Add("±", typeof(string));
            dtSource.Columns.Add("+", typeof(string));

            object[] o = new object[dtSource.Columns.Count];
            o[0] = "A";
            dtSource.Rows.Add(o);
            o[0] = "B";
            dtSource.Rows.Add(o);
            o[0] = "C";
            dtSource.Rows.Add(o);

            for (int i = 0; i < dtGrid1.Rows.Count; i++)
            {
                string regionName = "";
                regionName = ShortName(dtGrid1.Rows[i][0].ToString());
                int k = 0;
                for (int j = 0; j < dtGrid3.Rows.Count; j++)
                {
                    if (dtGrid3.Rows[j][0].ToString() == dtGrid1.Rows[i][0].ToString())
                    {
                        k = j;
                    }
                }
                if (dtGrid1.Rows[i][4].ToString() == "A")
                {
                    switch (dtGrid3.Rows[k][4].ToString())
                    {
                        case "-": dtSource.Rows[0][1] += regionName +" ("+dtGrid1.Rows[i][2].ToString()+";"+dtGrid3.Rows[k][2].ToString()+")"+ "<br>"; break;
                        case "±": dtSource.Rows[0][2] += regionName + " (" + dtGrid1.Rows[i][2].ToString() + ";" + dtGrid3.Rows[k][2].ToString() + ")" + "<br>"; break;
                        case "+": dtSource.Rows[0][3] += regionName + " (" + dtGrid1.Rows[i][2].ToString() + ";" + dtGrid3.Rows[k][2].ToString() + ")" + "<br>"; break;
                    }
                }
                if (dtGrid1.Rows[i][4].ToString() == "B")
                {
                    switch (dtGrid3.Rows[k][4].ToString())
                    {
                        case "-": dtSource.Rows[1][1] += regionName + " (" + dtGrid1.Rows[i][2].ToString() + ";" + dtGrid3.Rows[k][2].ToString() + ")" + "<br>"; break;
                        case "±": dtSource.Rows[1][2] += regionName + " (" + dtGrid1.Rows[i][2].ToString() + ";" + dtGrid3.Rows[k][2].ToString() + ")" + "<br>"; break;
                        case "+": dtSource.Rows[1][3] += regionName + " (" + dtGrid1.Rows[i][2].ToString() + ";" + dtGrid3.Rows[k][2].ToString() + ")" + "<br>"; break;
                    }
                }
                if (dtGrid1.Rows[i][4].ToString() == "C")
                {
                    switch (dtGrid3.Rows[k][4].ToString())
                    {
                        case "-": dtSource.Rows[2][1] += regionName + " (" + dtGrid1.Rows[i][2].ToString() + ";" + dtGrid3.Rows[k][2].ToString() + ")" + "<br>"; break;
                        case "±": dtSource.Rows[2][2] += regionName + " (" + dtGrid1.Rows[i][2].ToString() + ";" + dtGrid3.Rows[k][2].ToString() + ")" + "<br>"; break;
                        case "+": dtSource.Rows[2][3] += regionName + " (" + dtGrid1.Rows[i][2].ToString() + ";" + dtGrid3.Rows[k][2].ToString() + ")" + "<br>"; break;
                    }
                }
            }
            dtGrid2 = dtSource.Copy();
        }

        private void MakeHtmlTableDetailTable()
        {
            string color = "";
            detailTable.CssClass = "HtmlTableCompact";

            AddHeaderRow(detailTable);

            int k = 1;
            for (int i = 0; i < dtGrid2.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell= new TableCell();
                HtmlGenericControl div = new HtmlGenericControl("div"+k.ToString());
                div.ID = "div" + k.ToString();
                StringBuilder html = new StringBuilder();
                if (dtGrid2.Rows[i][0].ToString() == "A")
                {
                    html.AppendFormat("<span class=\"TextWhite TextXXLarge ShadowGreen\">{0}</span>", dtGrid2.Rows[i][0].ToString());
                    div.InnerHtml = html.ToString();
                }
                else if (dtGrid2.Rows[i][0].ToString() == "C")
                {
                    html.AppendFormat("<span class=\"TextWhite TextXXLarge ShadowRed\">{0}</span>", dtGrid2.Rows[i][0].ToString());
                    div.InnerHtml = html.ToString();
                }
                else if (dtGrid2.Rows[i][0].ToString() == "B")
                {
                    html.AppendFormat("<span class=\"TextWhite TextXXLarge ShadowYellow\">{0}</span>", dtGrid2.Rows[i][0].ToString());
                    div.InnerHtml = html.ToString();
                }
                cell.Controls.Add(div);
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.VerticalAlign = VerticalAlign.Middle;
                row.Cells.Add(cell);

                cell = GetNameCell(dtGrid2.Rows[i][1].ToString(), false,k);
                switch (k) 
                {
                    case 1: color = "rgba(209,209,0,1.0)"; break;
                    case 2: color = "rgba(62,186,0,1.0)"; break;
                    case 3: color = "rgba(59,118,0,1.0)"; break;
                    case 4: color = "rgba(255,67,67,1.0)"; break;
                    case 5: color = "rgba(209,174,0,1.0)"; break;
                    case 6: color = "rgba(62,186,0,1.0)"; break;
                    case 7: color = "rgba(164,27,0,1.0)"; break;
                    case 8: color = "rgba(255,67,67,1.0)"; break;
                    case 9: color = "rgba(209,209,0,1.0)"; break;
                } 
                
                cell.Style.Add("background-color", color);
                row.Cells.Add(cell);
                k += 1;

                cell = GetNameCell(dtGrid2.Rows[i][2].ToString(), false,k);
                switch (k)
                { 
                    case 1: color = "rgba(209,209,0,1.0)"; break;
                    case 2: color = "rgba(62,186,0,1.0)"; break;
                    case 3: color = "rgba(59,118,0,1.0)"; break;
                    case 4: color = "rgba(255,67,67,1.0)"; break;
                    case 5: color = "rgba(209,174,0,1.0)"; break;
                    case 6: color = "rgba(62,186,0,1.0)"; break; 
                    case 7: color = "rgba(164,27,0,1.0)"; break;
                    case 8: color = "rgba(255,67,67,1.0)"; break;
                    case 9: color = "rgba(209,209,0,1.0)"; break;
                }
                cell.Style.Add("background-color", color);
                row.Cells.Add(cell);
                k += 1;

                cell = GetNameCell(dtGrid2.Rows[i][3].ToString(), false,k);
                switch (k)
                {
                    case 1: color = "rgba(209,209,0,1.0)"; break;
                    case 2: color = "rgba(62,186,0,1.0)"; break;
                    case 3: color = "rgba(59,118,0,1.0)"; break;
                    case 4: color = "rgba(255,67,67,1.0)"; break;
                    case 5: color = "rgba(209,174,0,1.0)"; break;
                    case 6: color = "rgba(62,186,0,1.0)"; break;
                    case 7: color = "rgba(164,27,0,1.0)"; break;
                    case 8: color = "rgba(255,67,67,1.0)"; break;
                    case 9: color = "rgba(209,209,0,1.0)"; break;
                }
                cell.Style.Add("background-color", color);
                row.Cells.Add(cell);
                k += 1;

                detailTable.Rows.Add(row);
            }
        }

        private void AddHeaderRow(Table table)
        {
            TableCell cell;
            TableRow row;

            

            row = new TableRow();
            cell = new TableCell();
            cell.Width = 50;
            
            cell.Text = " ";
            cell.RowSpan = 2;

            SetupHeaderCell(cell);

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.ColumnSpan = 3;
            cell.Text = "Интегральная оценка населением ситуации в ключевых сферах деятельости ОМСУ";
            SetupHeaderCell(cell);
            row.Cells.Add(cell);
            table.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();
            cell.Width = 230;

            SetupHeaderCell(cell);
            HtmlGenericControl div = new HtmlGenericControl("div1");
            div.ID = "div1";
            StringBuilder html = new StringBuilder();
            html.AppendFormat("Низкая<br><span class=\"TextWhite TextXXLarge ShadowRed\">{0}</span>", "-");
            div.InnerHtml = html.ToString();
            cell.Controls.Add(div);

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 230;
            SetupHeaderCell(cell);
            div = new HtmlGenericControl("div2");
            div.ID = "div2";
            html = new StringBuilder();
            html.AppendFormat("Средняя<br><span class=\"TextWhite TextXXLarge ShadowYellow\">{0}</span>", "±");
            div.InnerHtml = html.ToString();
            cell.Controls.Add(div);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 230;
            SetupHeaderCell(cell);
            div = new HtmlGenericControl("div3");
            div.ID = "div3";
            html = new StringBuilder();
            html.AppendFormat("Высокая<br><span class=\"TextWhite TextXXLarge ShadowGreen\">{0}</span>", "+");
            div.InnerHtml = html.ToString();
            cell.Controls.Add(div);
            row.Cells.Add(cell);

            table.Rows.Add(row);
        }

        private TableCell GetNameCell(string name)
        {
            return GetNameCell(name, false,1);
        }

        private TableCell GetNameCell(string name, bool child, int k)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = String.Format("{0}", name);

            if (child)
            {
                lb.CssClass = "TableFontGrey"; 
                cell.Style.Add("padding-left", "10px");
            }
            else
            {
                lb.CssClass = "TableFont";
                cell.Style.Add("padding-left", "3px");
            }
            cell.Controls.Add(lb);
            lb.Style.Add("font-color", "white");
            lb.Style.Add("opacity", "1.0");
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Style.Add("padding-top", "5px");
            cell.Style.Add("padding-bottom", "5px");
            return cell;
        }

        private TableCell GetValueCell(string value)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = value;
            lb.CssClass = "TableFont";
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.Style.Add("padding-right", "3px");
            return cell;
        }

        private TableCell GetGreyValueCell(string value)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = value;
            lb.CssClass = "TableFontGrey";
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.Style.Add("padding-right", "3px");
            return cell;
        }

        private TableCell GetCell(int i, int column)
        {
            TableCell cell;
            cell = new TableCell();

            cell.Style.Add("font-size", "16px");
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            return cell;
        }

        private void SetupHeaderCell(TableCell cell)
        {
            cell.CssClass = "HtmlTableHeader";
            cell.Style.Add("font-size", "16px");
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
        }

        void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 3, 0, false);
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 5, 0, false);
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.ForeColor = Color.Black;
            }
           /* for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Value != null)
                {
                    string s = "";
                    string s1=e.Row.Cells[i].Text.Replace("<br>",",").Replace("г.","Город").Replace("р-н","район");
                    for (int j = 0; j < s1.Split(',').Length;j++ )
                    { 
                        s+= s1.Split(',')[i]+"<br>";
                        int l = 0;
                        for (int k = 0; k < dtGrid1.Rows.Count; k++)
                        {
                            if (dtGrid1.Rows[k][0].ToString().Contains(s1.Split(',')[i]))
                            {
                                l = k;
                            }
                        }
                        s += dtGrid1.Rows[l][1].ToString().Split('<')[0]+"<br>";
                        for (int k = 0; k < dtGrid3.Rows.Count; k++)
                        {
                            if (dtGrid3.Rows[k][0].ToString().Contains(s1.Split(',')[i]))
                            {
                                l = k;
                            }
                        }
                        s += dtGrid3.Rows[l][1].ToString().Split('<')[0] + "<br>";
                    }

                }
            }*/
        }

        void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;
            e.Layout.Bands[0].Grid.DisplayLayout.HeaderStyleDefault.Dispose();

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 16;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 14;
            

            e.Layout.Bands[0].Columns[0].Width = 50;
            e.Layout.Bands[0].Columns[1].Width = 230;
            e.Layout.Bands[0].Columns[2].Width = 230;
            e.Layout.Bands[0].Columns[3].Width = 230;
        }
        #endregion


        #region Гейдж

        protected string GetGaugeUrl(object oValue, object oRank)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue) * 100;
            double rank = Convert.ToDouble(oRank);
            if (value > 100)
                value = 100;
            string path = "SEP_0001_0001_gauge_" + value.ToString("N0") + "_"+rank.ToString("N0")+".png";
            string returnPath = String.Format("<img style=\"FLOAT: left;\" src=\"../../../TemporaryImages/{0}\"/>", path);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath))) 
            return returnPath;   
            ((LinearGauge)Gauge.Gauges[0]).Scales[0].Markers[0].Value = value;
            LinearGaugeScale scale = ((LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (rank >=1 && rank<=6)
            { 
                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else
            {
                if (rank >= 7 && rank <= 13)
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
                }
                else
                {
                    if (rank >= 14 && rank <= 19)
                    {
                        BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                        BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                        BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                        BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
                    }
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

        #endregion

        protected double GetNumber(string s)
        {
            try
            {
                if (!String.IsNullOrEmpty(s))
                {
                    return double.Parse(s);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }

        protected void calculateRank(DataTable Grid, int colNumber, int colRankNumber)
        {
            int m = 0;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                if (MathHelper.IsDouble(Grid.Rows[i][colNumber]) == true)
                {
                    m += 1;

                }
                Grid.Rows[i][colRankNumber] = 0;
            }

            if (m != 0)
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 0; i < Grid.Rows.Count; i++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[i][colNumber]) == true)
                    {
                        rank[m] = Convert.ToDouble(Grid.Rows[i][colNumber]);
                        m += 1;
                        Grid.Rows[i][colRankNumber] = 0;
                    }
                    else
                    {
                        Grid.Rows[i][colRankNumber] = String.Empty;
                    }

                }
                Array.Sort(rank);

                m = 1;
                for (int i = rank.Length - 1; i >= 0; i--)
                {

                    for (int j = 0; j < Grid.Rows.Count; j++)
                    {
                        if (rank[i] == GetNumber(Grid.Rows[j][colNumber].ToString()))
                        {
                            if (Grid.Rows[j][colRankNumber].ToString() == "0")
                            {
                                Grid.Rows[j][colRankNumber] =  m;
                            }
                        }
                    }
                    if (i != 0)
                    {
                        if (rank[i] != rank[i - 1])
                        {
                            m += 1;
                        }
                    }
                    else
                    { m += 1; }

                }
            }

        }

        private System.Collections.Generic.Dictionary<string, string> ShortRegionsNames
        {
            get
            {
                // если короткие имена регионов еще не получены
                if (shortRegionsNames == null || shortRegionsNames.Count == 0)
                {
                    // заполняем словарик
                    FillShortRegionsNames();
                }
                return shortRegionsNames;
            }
        }

        private System.Collections.Generic.Dictionary<string, string> shortRegionsNames;

        private void FillShortRegionsNames()
        {
            shortRegionsNames = new System.Collections.Generic.Dictionary<string, string>();
            string query = DataProvider.GetQueryText("shortRegionsNames");
            DataTable dt = new DataTable();

            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Регион", dt);
            foreach (DataRow row in dt.Rows)
            {
                // пока нет нормального запроса с именами ФО, будем делать глупо.
                string key = row[0].ToString();
                key = key.Trim('(');
                key = key.Replace(" ДАННЫЕ)", string.Empty);
                shortRegionsNames.Add(key, row[1].ToString());
            }
        }

        private string ShortName(string fullName)
        {
            if (fullName == null)
            {
                return String.Empty;
            }

            if (fullName == "Российская Федерация")
            {
                return "РФ";
            }
            if (ShortRegionsNames.ContainsKey(fullName))
            {
                if ((ShortRegionsNames[fullName] != "Углегорский р-н") && (ShortRegionsNames[fullName] != "г. Южно-Сахалинск"))
                {
                    return "г.о. "+ShortRegionsNames[fullName].Replace("р-н",String.Empty);
                }
                else
                {
                    return ShortRegionsNames[fullName];
                }
                
            }

            return fullName;
        }
    }
}