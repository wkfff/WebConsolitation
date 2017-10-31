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
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using System.Drawing;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid;
using Microsoft.AnalysisServices.AdomdClient;


namespace Krista.FM.Server.Dashboards.FO_BOR_0001_003_0001.Default
{
    public partial class _default : CustomReportPage
    {
        #region Всякие разные заголовки для гридов
        string[] HT = { "Лимит финансирования, млн.р.", "Исполнено, млн.р.", "Индекс затрат", "Общая результативность достижения целей" };
        string[] HB = { "Пгрбс", "Фгрбс", "ИЗ = Фгрбс / Пгрбс", "ОРц" };
        #endregion

        #region Парамы для запросов
        private CustomParam LD { get { return (UserParams.CustomParam("LD")); } }
        private CustomParam GRBS { get { return (UserParams.CustomParam("GRBS")); } }
        #endregion

        #region Словари о гридах и их заголовках
        System.Collections.Generic.Dictionary<int, UltraWebGrid> AllGrid = new System.Collections.Generic.Dictionary<int,UltraWebGrid>();

        System.Collections.Generic.Dictionary<int, string> RightGridHeder = new System.Collections.Generic.Dictionary<int, string>();
        System.Collections.Generic.Dictionary<int, string> LeftGridHeder = new System.Collections.Generic.Dictionary<int, string>();

        System.Collections.Generic.Dictionary<int, string> BRightGridHeder = new System.Collections.Generic.Dictionary<int, string>();
        System.Collections.Generic.Dictionary<int, string> BLeftGridHeder = new System.Collections.Generic.Dictionary<int, string>();

        System.Collections.Generic.Dictionary<int, string> PercentGridHeder = new System.Collections.Generic.Dictionary<int, string>();

        System.Collections.Generic.Dictionary<int, string[]> TopHeders_ = new System.Collections.Generic.Dictionary<int, string[]>();
        System.Collections.Generic.Dictionary<int, string[]> BottomHeders_ = new System.Collections.Generic.Dictionary<int, string[]>();

        System.Collections.Generic.Dictionary<int, Decimal[]> Values_ = new System.Collections.Generic.Dictionary<int, Decimal[]>();
        #endregion

        #region Генератор ультра-табеля и табеля "обыкновенного"
        protected Table GenCrazyGrid(string[] TopHeders, string[] BottomHeders, Decimal[] Values, string H0, string H1, string H2, string H3)
        {
            Table t = new Table();

            TopHeders_.Add(TopHeders_.Count, TopHeders);
            BottomHeders_.Add(BottomHeders_.Count, BottomHeders);
            Values_.Add(Values_.Count, Values);


            t.Rows.Add(new TableRow());
            t.Rows.Add(new TableRow());
            t.Rows.Add(new TableRow());
            t.BorderColor = Color.LightGray;
            t.BorderWidth = 1;
            t.BorderStyle = BorderStyle.Solid;
            t.Style["BORDER-COLLAPSE"] = "collapse";

            t.Rows[0].Cells.Add(new TableCell());
            t.Rows[0].Cells.Add(new TableCell());
            t.Rows[0].Cells[1].ColumnSpan = 2;
            t.Rows[0].Cells[0].BorderStyle = BorderStyle.Solid;

            t.Rows[0].Cells[0].Style["BORDER-BOTTOM-COLOR"] = "LightGray";
            t.Rows[0].Cells[0].Style["BORDER-BOTTOM-STYLE"] = "solid";
            t.Rows[0].Cells[0].Style["BORDER-BOTTOM-WIDTH"] = "1px";

            t.Rows[0].Cells[0].Style["BORDER-TOP-COLOR"] = "LightGray";
            t.Rows[0].Cells[0].Style["BORDER-TOP-STYLE"] = "solid";
            t.Rows[0].Cells[0].Style["BORDER-TOP-WIDTH"] = "1px";

            t.Rows[0].Cells[0].Style["BORDER-RIGHT-COLOR"] = "LightGray";
            t.Rows[0].Cells[0].Style["BORDER-RIGHT-STYLE"] = "solid";
            t.Rows[0].Cells[0].Style["BORDER-RIGHT-WIDTH"] = "1px";

            t.Rows[0].Cells[0].Style["BORDER-LEFT-COLOR"] = "LightGray";
            t.Rows[0].Cells[0].Style["BORDER-LEFT-STYLE"] = "solid";
            t.Rows[0].Cells[0].Style["BORDER-LEFT-WIDTH"] = "1px";

            t.Rows[0].Cells[1].Style["BORDER-BOTTOM-COLOR"] = "LightGray";
            t.Rows[0].Cells[1].Style["BORDER-BOTTOM-STYLE"] = "solid";
            t.Rows[0].Cells[1].Style["BORDER-BOTTOM-WIDTH"] = "1px";

            t.Rows[0].Cells[1].Style["BORDER-LEFT-COLOR"] = "LightGray";
            t.Rows[0].Cells[1].Style["BORDER-LEFT-STYLE"] = "solid";
            t.Rows[0].Cells[1].Style["BORDER-LEFT-WIDTH"] = "1px";
            

            t.Rows[1].Cells.Add(new TableCell());            
            t.Rows[1].Cells.Add(new TableCell());
            t.Rows[1].Cells[1].ColumnSpan = 2;

            t.Rows[1].Cells[0].Style["BORDER-BOTTOM-COLOR"] = "LightGray";
            t.Rows[1].Cells[0].Style["BORDER-BOTTOM-STYLE"] = "solid";
            t.Rows[1].Cells[0].Style["BORDER-BOTTOM-WIDTH"] = "1px";

            t.Rows[1].Cells[1].Style["BORDER-BOTTOM-COLOR"] = "LightGray";
            t.Rows[1].Cells[1].Style["BORDER-BOTTOM-STYLE"] = "solid";
            t.Rows[1].Cells[1].Style["BORDER-BOTTOM-WIDTH"] = "1px";

            t.Rows[1].Cells[1].Style["BORDER-LEFT-COLOR"] = "LightGray";
            t.Rows[1].Cells[1].Style["BORDER-LEFT-STYLE"] = "solid";
            t.Rows[1].Cells[1].Style["BORDER-LEFT-WIDTH"] = "1px";

            t.Rows[2].Cells.Add(new TableCell());
            t.Rows[2].Cells.Add(new TableCell());
            t.Rows[2].Cells.Add(new TableCell());

            t.Rows[2].Cells[1].Style["BORDER-LEFT-COLOR"] = "LightGray";
            t.Rows[2].Cells[1].Style["BORDER-LEFT-STYLE"] = "solid";
            t.Rows[2].Cells[1].Style["BORDER-LEFT-WIDTH"] = "1px";

            t.Rows[0].Cells[0].Text = H0;
            t.Rows[0].Cells[0].Font.Bold = 1 == 1;
            t.Rows[0].Cells[0].Font.Size = 11;
            t.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            t.Rows[0].Cells[0].Font.Name = "Arial";

            t.Rows[0].Cells[1].Text = H1;
            t.Rows[0].Cells[1].HorizontalAlign = HorizontalAlign.Center;
            t.Rows[0].Cells[1].Font.Bold = 1 == 1;
            t.Rows[0].Cells[1].Font.Size = 11;
            t.Rows[0].Cells[1].Font.Name = "Arial";

            t.Rows[1].Cells[0].Text = H2;
            //t.Rows[1].Cells[0].Font.Size = 11;
            //t.Rows[1].Cells[0].HorizontalAlign = HorizontalAlign.Center;

            

            LeftGridHeder.Add(LeftGridHeder.Count, H0);
            RightGridHeder.Add(RightGridHeder.Count, H1);

            BLeftGridHeder.Add(BLeftGridHeder.Count, H2);
            BRightGridHeder.Add(BRightGridHeder.Count, H3);

            PercentGridHeder.Add(PercentGridHeder.Count, string.Format("{0:##0%}", Values[4]));

            t.Rows[1].Cells[1].Text = H3;
            t.Rows[1].Cells[1].HorizontalAlign = HorizontalAlign.Center;
            t.Rows[1].Cells[1].Font.Bold = 1 == 1;
            t.Rows[1].Cells[1].Font.Size = 10;
            

            t.Rows[2].Cells[1].Text = string.Format("{0:##0%}",Values[4]);
            t.Rows[2].Cells[1].Font.Size = 24;
            t.Rows[2].Cells[1].Style["COLOR"] = ((Values[4] >= 1)?"green":"red");
            t.Rows[2].Cells[1].HorizontalAlign = HorizontalAlign.Center;
            
            t.Rows[2].Cells[2].Text = ((Values[4] >= 1) ? "<img style=\"vertical-align:baseline\" src=\"../../images/GreenAllert.gif\">" : "<img style=\"vertical-align:baseline\" src=\"../../images/RedAllert.gif\">");
            t.Rows[2].Cells[2].HorizontalAlign = HorizontalAlign.Center;

            UltraWebGrid CurGrid = GenGrid(TopHeders, BottomHeders, Values);

            PlaceHolder2.Controls.Add(CurGrid);
            PlaceHolder2.Visible = 1 == 2;
            t.Rows[2].Cells[0].Controls.Add(GenGrid2(TopHeders, BottomHeders, Values));

            t.Rows[0].Cells[0].Width = 890;
            t.Rows[1].Cells[0].Width = 890;

            t.Rows[0].Cells[1].Width = 360;
            t.Rows[1].Cells[1].Width = 360;

            t.Rows[2].Cells[1].Width = 180;
            t.Rows[2].Cells[2].Width = 180;
            t.Rows[2].Cells[0].Width = 890;

            CurGrid.Width = 800;
            

            return t;

        }
        Random r = new Random();
        Table GenGrid2(string[] TopHeders, string[] BottomHeders, Decimal[] Values)
        {
            Table t = new Table();
            t.ID = "Table" + r.Next().ToString();
            t.Style["WIDTH"] = "100%";
            t.Style["HEIGHT"] = "100%";

            t.BorderColor =  Color.LightGray;

            t.BorderWidth = 1;
            t.BorderStyle = BorderStyle.Solid;
            t.Style["BORDER-COLLAPSE"] = "collapse";


            t.Rows.Add(new TableRow());
            t.Rows.Add(new TableRow());
            t.Rows.Add(new TableRow());

            t.Rows[0].Cells.Add(new TableCell());
            t.Rows[0].Cells.Add(new TableCell());
            t.Rows[0].Cells.Add(new TableCell());
            t.Rows[0].Cells.Add(new TableCell());

            t.Rows[1].Cells.Add(new TableCell());
            t.Rows[1].Cells.Add(new TableCell());
            t.Rows[1].Cells.Add(new TableCell());
            t.Rows[1].Cells.Add(new TableCell());

            t.Rows[2].Cells.Add(new TableCell());
            t.Rows[2].Cells.Add(new TableCell());
            t.Rows[2].Cells.Add(new TableCell());
            t.Rows[2].Cells.Add(new TableCell());

            

            for (int i = 0; i < TopHeders.Length; i++)
            {
                SetFormatTableCell(t.Rows[0].Cells[i], TopHeders[i]);
            }

            for (int i = 0; i < BottomColumnHeder.Length; i++)
            {
                SetFormatTableCell(t.Rows[1].Cells[i], BottomColumnHeder[i]);
            }

            for (int i = 0; i < Values.Length-1; i++)
            {
                if (i > 1)
                {
                    SetFormatTableCell_(t.Rows[2].Cells[i], string.Format("{0:### ### ### ##0.##%}", Values[i]));
                }
                else
                {
                    SetFormatTableCell_(t.Rows[2].Cells[i], string.Format("{0:### ### ### ##0.##}", Values[i]));
                }
            }
                return t;

        }

        void SetFormatTableCell(TableCell C, string hedere)
        {
            C.BackColor = Color.DarkGray;
            C.Text = hedere;
            C.HorizontalAlign = HorizontalAlign.Center;
            C.Style["WIDTH"] = "25%";
            C.Font.Bold = 1 == 1;
            C.Font.Size = 10;
            C.Style["BORDER-BOTTOM-COLOR"] = "White";
            C.Style["BORDER-BOTTOM-STYLE"] = "solid";
            C.Style["BORDER-BOTTOM-WIDTH"] = "1px";

            C.Style["BORDER-LEFT-COLOR"] = "White";
            C.Style["BORDER-LEFT-STYLE"] = "solid";
            C.Style["BORDER-LEFT-WIDTH"] = "1px";
        }
        void SetFormatTableCell_(TableCell C, string hedere)
        {
            C.BackColor = Color.White;
            C.Text = hedere;
            C.HorizontalAlign = HorizontalAlign.Center;
            C.Style["WIDTH"] = "25%";
            //C.Font.Bold = 1 == 1;
            C.Font.Size = 10;
            C.Style["BORDER-BOTTOM-COLOR"] = "LightGray";
            C.Style["BORDER-BOTTOM-STYLE"] = "solid";
            C.Style["BORDER-BOTTOM-WIDTH"] = "1px";

            C.Style["BORDER-LEFT-COLOR"] = "LightGray";
            C.Style["BORDER-LEFT-STYLE"] = "solid";
            C.Style["BORDER-LEFT-WIDTH"] = "1px";
        }
        


        

        string[] TopColumnHeder = {"","","",""};
        string[] BottomColumnHeder = { "", "", "", ""};

        UltraWebGrid GenGrid(string[] TopHeders,string[] BottomHeders, Decimal[] Values)
        {
            UltraWebGrid grid = new UltraWebGrid();
            grid.EnableViewState = false;
            grid.DisplayLayout.Reset();
            grid.Bands.Clear();
            grid.SkinID = "UltraWebGrid";
            grid.EnableAppStyling = Infragistics.WebUI.Shared.DefaultableBoolean.True;
            grid.EnableTheming = false;

            grid.DisplayLayout.CopyFrom(G.DisplayLayout);
            grid.StyleSetName = "Office2007Blue";
            grid.DisplayLayout.GroupByBox.Hidden = 1 == 1;
            grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            grid.Height = Unit.Empty;

            TopColumnHeder = TopHeders;
            BottomColumnHeder = BottomHeders;

            grid.DataBinding += new EventHandler(G_DataBinding);
            grid.InitializeLayout += new InitializeLayoutEventHandler(G_InitializeLayout);
            grid.DataBind();

            grid.Rows[0].Cells[0].Text = string.Format("{0:### ### ##0.##}", Values[0]);
            grid.Rows[0].Cells[1].Text = string.Format("{0:### ### ##0.##}", Values[1]);
            grid.Rows[0].Cells[2].Text = string.Format("{0:### ### ##0.##%}", Values[2]);
            grid.Rows[0].Cells[3].Text = string.Format("{0:### ### ##0.##%}", Values[3]);
            grid.Rows[0].Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;

            AllGrid.Add(AllGrid.Count, grid);
            
            return grid;


        }
        ColumnHeader GenColumnHeder(string Caption, int x, int y)
        {
            ColumnHeader CH = new ColumnHeader(1 == 1);
            CH.RowLayoutColumnInfo.OriginX = x;
            CH.RowLayoutColumnInfo.OriginY = y;
            CH.RowLayoutColumnInfo.SpanX = 1;
            CH.RowLayoutColumnInfo.SpanY = 1;
            CH.Caption = Caption;
            CH.Style.Wrap = 1 == 1;
            CH.Style.HorizontalAlign = HorizontalAlign.Center;
            return CH;
        }

        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(TopColumnHeder[0]);
            dt.Columns.Add(TopColumnHeder[1]);
            dt.Columns.Add(TopColumnHeder[2]);
            dt.Columns.Add(TopColumnHeder[3]);
            dt.Rows.Add("1", "2", "3", "4");
            ((UltraWebGrid)(sender)).DataSource = dt;
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200-15);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(200 - 15);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(200 - 15);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(200 - 15);

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].HeaderLayout.Clear();
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(TopColumnHeder[0], 0, 0));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(TopColumnHeder[1], 1, 0));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(TopColumnHeder[2], 2, 0));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(TopColumnHeder[3], 3, 0));

            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(BottomColumnHeder[0], 0, 1));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(BottomColumnHeder[1], 1, 1));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(BottomColumnHeder[2], 2, 1));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(BottomColumnHeder[3], 3, 1));
        }
        #endregion

        #region Верхний грид
        void GenTopGrid()
        {
            DataTable dt = GetDS("TG");
            if (dt.Rows.Count == 0)
            {
                Table t_ = new Table();
                t_.Rows.Add(new TableRow());
                t_.Rows[0].Cells.Add(new TableCell());
                t_.Rows[0].Cells[0].Text = "Нет данных";
                panelGrid.Controls.Add(t_);
                return;
            }

            try
            {
                Decimal[] D = { (System.Decimal)(dt.Rows[0][0]), (System.Decimal)(dt.Rows[0][1]), (System.Decimal)(dt.Rows[0][2]), (System.Decimal)(dt.Rows[0][3]), (System.Decimal)(dt.Rows[0][4]) };

                Table t = GenCrazyGrid(HT, HB, D, "", "", "Общая результативность деятельности ГРБС", "ОРДгрбс = ОРц / ИЗ");
                panelGrid.Controls.Add(t);
                t.Rows.Remove(t.Rows[0]);
                t.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                 }
            catch {
                Table t_ = new Table();
                t_.Rows.Add(new TableRow());
                t_.Rows[0].Cells.Add(new TableCell());
                t_.Rows[0].Cells[0].Text = "Нет данных";
                panelGrid.Controls.Add(t_);
                return;
            }

        }
        #endregion

        #region Табель для первого параметра

        void GenOtherGrid_0(string q)
        {
            DataTable dt = GetDS(q);
            if (dt.Rows.Count <=1)
            {
                Table t = new Table();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell());
                t.Rows[0].Cells[0].Text = "Нет данных";
                PlaceHolder1.Controls.Add(t);
                return;
            }
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                
                    string[] HBP = { "Пц", "Фц", "ИЗ = Фц / Пц", "Рц" };
                    Decimal[] D = { (System.Decimal)(dt.Rows[i][1]), (System.Decimal)(dt.Rows[i][2]), (System.Decimal)(dt.Rows[i][3]), (System.Decimal)(dt.Rows[i][4]), (System.Decimal)(dt.Rows[i][5]) };
                    PlaceHolder1.Controls.Add(GenCrazyGrid(HT, HBP, D, "Цель", "Результативность деятельности ГРБС по достижению цели",
                        i.ToString() + ". " + dt.Rows[i][0].ToString(), "ОРДц = Рц / ИЗ"));

                    Table t = new Table();
                    t.Rows.Add(new TableRow());
                    t.Rows[0].Cells.Add(new TableCell());
                    t.Rows[0].Cells[0].Height = 20;
                    PlaceHolder1.Controls.Add(t);
            }
        }
        #endregion

        #region Табель для второго параметра
        void GenOtherGrid_1(string q)
        {
            DataTable dt = GetDS(q);
            if (dt.Rows.Count <=1)
            {
                Table t = new Table();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell());
                t.Rows[0].Cells[0].Text = "Нет данных";
                PlaceHolder1.Controls.Add(t);
                return;
            }
            for (int i = 1; i < dt.Rows.Count; i++)
            {

                string[] HBP = { "Пц", "Фц", "ИЗ = Фц / Пц", "ОРтз" };
                string[] HTB = { "Лимит финансирования, млн.р.", "Исполнено, млн.р.", "Индекс затрат", "Результативность решения тактических задач" };
                Decimal[] D = { (System.Decimal)(dt.Rows[i][1]), (System.Decimal)(dt.Rows[i][2]), (System.Decimal)(dt.Rows[i][3]), (System.Decimal)(dt.Rows[i][4]), (System.Decimal)(dt.Rows[i][5]) };
                PlaceHolder1.Controls.Add(GenCrazyGrid(HTB, HBP, D, "Цель", "Общая результативность деятельности ГРБС по решению тактических задач цели",
                    i.ToString() + ". " + dt.Rows[i][0].ToString(), "ОРДтз = ОРтз / ИЗ"));
                Table t = new Table();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell());
                t.Rows[0].Cells[0].Height = 20;
                PlaceHolder1.Controls.Add(t);
            }
        }
        #endregion

        #region Табель для третего параметра
        void GenOtherGrid_2(string q)
        {
            DataTable dt = GetDS(q);
            if (dt.Rows.Count <=1)
            {
                Table t_ = new Table();
                t_.Rows.Add(new TableRow());
                t_.Rows[0].Cells.Add(new TableCell());
                t_.Rows[0].Cells[0].Text = "Нет данных";
                PlaceHolder1.Controls.Add(t_);
                return;
            }
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(q));

            int CountGoal = 0;
            int CountTask = 0;
            Table t = new Table();
            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                string[] ololo = {"].["};
                
                if (cs.Axes[1].Positions[i].Members[0].UniqueName.Split(ololo, StringSplitOptions.None).Length == 5)
                {
                    if (i != 1)
                    {
                        Table tb = new Table();
                        tb.Rows.Add(new TableRow());
                        tb.Rows[0].Cells.Add(new TableCell());
                        tb.Rows[0].Cells[0].Height = 20;
                        PlaceHolder1.Controls.Add(tb);
                    }


                    CountGoal++;
                    CountTask = 0;
                    t = new Table();
                    t.ID = "table_" + CountGoal.ToString();
                    t.Rows.Add(new TableRow());
                    t.Rows.Add(new TableRow());

                    t.Rows[0].Cells.Add(new TableCell());
                    t.Rows[1].Cells.Add(new TableCell());

                    t.Rows[0].Cells[0].Text = "Цель";
                    t.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    t.Rows[0].Cells[0].Font.Bold = 1 == 1;

                    t.Rows[0].Cells[0].Style["BORDER-TOP-COLOR"] = "LightGray";
                    t.Rows[0].Cells[0].Style["BORDER-TOP-STYLE"] = "solid";
                    t.Rows[0].Cells[0].Style["BORDER-TOP-WIDTH"] = "1px";

                    t.Rows[0].Cells[0].Style["BORDER-RIGHT-COLOR"] = "LightGray";
                    t.Rows[0].Cells[0].Style["BORDER-RIGHT-STYLE"] = "solid";
                    t.Rows[0].Cells[0].Style["BORDER-RIGHT-WIDTH"] = "1px";

                    t.Rows[0].Cells[0].Style["BORDER-LEFT-COLOR"] = "LightGray";
                    t.Rows[0].Cells[0].Style["BORDER-LEFT-STYLE"] = "solid";
                    t.Rows[0].Cells[0].Style["BORDER-LEFT-WIDTH"] = "1px";


                    t.Rows[1].Cells[0].Style["BORDER-BOTTOM-COLOR"] = "LightGray";
                    t.Rows[1].Cells[0].Style["BORDER-BOTTOM-STYLE"] = "solid";
                    t.Rows[1].Cells[0].Style["BORDER-BOTTOM-WIDTH"] = "1px";

                    t.Rows[1].Cells[0].Style["BORDER-TOP-COLOR"] = "LightGray";
                    t.Rows[1].Cells[0].Style["BORDER-TOP-STYLE"] = "solid";
                    t.Rows[1].Cells[0].Style["BORDER-TOP-WIDTH"] = "1px";

                    t.Rows[1].Cells[0].Style["BORDER-RIGHT-COLOR"] = "LightGray";
                    t.Rows[1].Cells[0].Style["BORDER-RIGHT-STYLE"] = "solid";
                    t.Rows[1].Cells[0].Style["BORDER-RIGHT-WIDTH"] = "1px";

                    t.Rows[1].Cells[0].Style["BORDER-LEFT-COLOR"] = "LightGray";
                    t.Rows[1].Cells[0].Style["BORDER-LEFT-STYLE"] = "solid";
                    t.Rows[1].Cells[0].Style["BORDER-LEFT-WIDTH"] = "1px";

                    if (CountGoal > 1)
                    {
                       // t.Rows[0].Cells[0].Height = 50;
                        t.Rows[0].Cells[0].VerticalAlign = VerticalAlign.Bottom;

                    }
                    else
                    {
                        t.Rows[0].Cells[0].VerticalAlign = VerticalAlign.Bottom;
                    }

                    t.Rows[1].Cells[0].Text = CountGoal.ToString() + ". " + dt.Rows[i][0].ToString();                   

                    t.Rows[0].Cells[0].Width = 1230;
                    t.Rows[1].Cells[0].Width = 1230;
                    t.Rows[0].Cells[0].ColumnSpan = 3;
                    t.Rows[1].Cells[0].ColumnSpan = 3;

                    PlaceHolder1.Controls.Add(t);
                }
                else
                {
                    CountTask++;
                    string[] HTB = {"Птз"	,"Фтз"	,"ИЗ = Фтз / Птз",	"Ртз" };
                    string[] HTB_ = { "Лимит финансирования, млн.р.", "Исполнено, млн.р.", "Индекс затрат", "Результативность решения тактической задачи" };
                    Decimal[] D = { (System.Decimal)(dt.Rows[i][1]), (System.Decimal)(dt.Rows[i][2]), (System.Decimal)(dt.Rows[i][3]), (System.Decimal)(dt.Rows[i][4]), (System.Decimal)(dt.Rows[i][5]) };
                    Table tt = GenCrazyGrid(HTB_, HTB, D, "Задача", "Результативность деятельности ГРБС по решению тактической задачи", CountGoal.ToString() + "." + CountTask.ToString() + ". " + dt.Rows[i][0].ToString(), "РДтз = Ртз / ИЗ");
                    if (t.Rows.Count > 0)
                    {
                        try
                        {
                            tt.Rows.AddAt(0, t.Rows[1]);
                            tt.Rows.AddAt(0, t.Rows[0]);
                            
                        }
                        catch { }
                    }
                    t.Rows.Clear();
                    PlaceHolder1.Controls.Add(tt);
                    Table tb = new Table();
                    tb.Rows.Add(new TableRow());
                    tb.Rows[0].Cells.Add(new TableCell());
                    tb.Rows[0].Cells[0].Height = 5;
                    PlaceHolder1.Controls.Add(tb);
                }
                
            }
        }
        #endregion

        #region Пуск!
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraGridExporter1.PdfExportButton.Visible = 1 == 2;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
 
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(GenUserParam("LD_"));
                ComboYear.SetСheckedState(LD.Value, 1 == 1);
                ComboYear.Title = "Период";
                ComboYear.Width = 135;

                comboFo.FillDictionaryValues(GenUserParam("Fo_BOR"));
                comboFo.Title = "ГРБС";
                comboFo.Width = 1000;

                System.Collections.Generic.Dictionary<string, int> oParam = new System.Collections.Generic.Dictionary<string, int>();
                oParam.Add("Результативность деятельности ГРБС по достижению целей", 0);
                oParam.Add("Общая результативность деятельности ГРБС по решению тактических задач каждой цели", 0);
                oParam.Add("Результативность деятельности ГРБС по решению каждой тактической задачи", 0);                
                OtherParam.FillDictionaryValues(oParam);
                OtherParam.Title = "Показатель";                
            }
            string PrevReport = CustomParam.CustomParamFactory("CRepoort").Value;

            //Удалаяяем этот папраметр, шоб в следущий ра
            Session.Remove("CRepoort");
            //если пусто значит не с какого
            if (!string.IsNullOrEmpty(PrevReport))
            {
                comboFo.SetСheckedState(PrevReport.Split(':')[0].Replace('_', ' '), 1 == 1);
                ComboYear.SetСheckedState(PrevReport.Split(':')[1], 1 == 1);
            }

            LD.Value = ComboYear.SelectedValue;
            GRBS.Value = comboFo.SelectedValue;

            GenTopGrid();

            if (OtherParam.SelectedValue == "Результативность деятельности ГРБС по достижению целей")
            {
                GenOtherGrid_0("OG_0");
            }
            if (OtherParam.SelectedValue == "Общая результативность деятельности ГРБС по решению тактических задач каждой цели")
            {
                GenOtherGrid_1("OG_1");
            }
            if (OtherParam.SelectedValue == "Результативность деятельности ГРБС по решению каждой тактической задачи")
            {
                GenOtherGrid_2("OG_2");
            }

            Label1.Text = string.Format("Общая результативность деятельности ГРБС: «{0}»",comboFo.SelectedValue);
            Label3.Text = string.Format("Результативность деятельности ГРБС в {0} году ({1})",ComboYear.SelectedValue,comboFo.SelectedValue);
            Page.Title = Label3.Text;
        }
        #endregion

        #region Экпорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {

        }

        void ExportGrid(Worksheet sheet)
        {
            sheet.Rows[0].Cells[0].Value = LeftGridHeder[sheet.Index];
            sheet.Rows[0].Cells[4].Value = RightGridHeder[sheet.Index];

            sheet.Rows[0].Height = 37 * 37;
            sheet.Rows[0].CellFormat.Alignment = HorizontalCellAlignment.Center;
            sheet.Rows[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;


            sheet.Rows[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            sheet.Rows[1].Cells[0].Value = BLeftGridHeder[sheet.Index];
            sheet.Rows[1].Cells[4].Value = BRightGridHeder[sheet.Index];

            sheet.Rows[1].Height = 37 * 37;
            sheet.Rows[1].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet.Rows[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            sheet.Rows[2].Cells[4].Value = PercentGridHeder[sheet.Index];

            sheet.Rows[2].Cells[0].Value = TopHeders_[sheet.Index][0];
            sheet.Rows[2].Cells[1].Value = TopHeders_[sheet.Index][1];
            sheet.Rows[2].Cells[2].Value = TopHeders_[sheet.Index][2];
            sheet.Rows[2].Cells[3].Value = TopHeders_[sheet.Index][3];

            sheet.Rows[2].Height = 15 * 37;
            sheet.Rows[2].CellFormat.Alignment = HorizontalCellAlignment.Center;
            sheet.Rows[2].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            sheet.Rows[2].Cells[0].CellFormat.FillPatternBackgroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[2].Cells[0].CellFormat.FillPatternForegroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[2].Cells[1].CellFormat.FillPatternBackgroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[2].Cells[1].CellFormat.FillPatternForegroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[2].Cells[2].CellFormat.FillPatternBackgroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[2].Cells[2].CellFormat.FillPatternForegroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[2].Cells[3].CellFormat.FillPatternBackgroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[2].Cells[3].CellFormat.FillPatternForegroundColor = Color.FromArgb(150, 150, 150);

            sheet.Rows[3].Cells[0].Value = BottomHeders_[sheet.Index][0];
            sheet.Rows[3].Cells[1].Value = BottomHeders_[sheet.Index][1];
            sheet.Rows[3].Cells[2].Value = BottomHeders_[sheet.Index][2];
            sheet.Rows[3].Cells[3].Value = BottomHeders_[sheet.Index][3];
            sheet.Rows[3].Cells[0].CellFormat.FillPatternBackgroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[3].Cells[0].CellFormat.FillPatternForegroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[3].Cells[1].CellFormat.FillPatternBackgroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[3].Cells[1].CellFormat.FillPatternForegroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[3].Cells[2].CellFormat.FillPatternBackgroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[3].Cells[2].CellFormat.FillPatternForegroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[3].Cells[3].CellFormat.FillPatternBackgroundColor = Color.FromArgb(150, 150, 150);
            sheet.Rows[3].Cells[3].CellFormat.FillPatternForegroundColor = Color.FromArgb(150, 150, 150);


            sheet.Rows[3].Height = 15 * 37;
            sheet.Rows[3].CellFormat.Alignment = HorizontalCellAlignment.Center;
            sheet.Rows[3].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;


            sheet.Rows[4].Cells[0].Value = Values_[sheet.Index][0];
            sheet.Rows[4].Cells[1].Value = Values_[sheet.Index][1];
            sheet.Rows[4].Cells[2].Value = Values_[sheet.Index][2];
            sheet.Rows[4].Cells[3].Value = Values_[sheet.Index][3];

            sheet.Rows[4].Height = 20 * 37;
            sheet.Rows[4].CellFormat.Alignment = HorizontalCellAlignment.Center;
            sheet.Rows[4].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            try
            {
                sheet.MergedCellsRegions.Add(0, 0, 0, 3);
                sheet.MergedCellsRegions.Add(1, 0, 1, 3);
                sheet.MergedCellsRegions.Add(2, 4, 4, 4);
            }
            catch { }
        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            foreach (Worksheet sheet in e.Workbook.Worksheets)
            {
                ExportGrid(sheet);
                if (sheet.Index==0)
                {
                    sheet.Rows[0].Cells[0].Value = Label1.Text;//"Общая результативность деятельности ГРБС: «Министерство имущественных отношений Самарской области»";
                }


                
                sheet.Columns[0].Width = 200 * 37;
                sheet.Columns[1].Width = 200 * 37;
                sheet.Columns[2].Width = 200 * 37;
                sheet.Columns[3].Width = 200 * 37;
                sheet.Columns[4].Width = 200 * 37;

                ////sheet.Rows[0].Cells[0].Value = AllGridHeder[sheet.Index];

                sheet.Columns[1].CellFormat.FormatString = "#,##0";
                sheet.Columns[2].CellFormat.FormatString = "#,##0.0##";
                sheet.Columns[3].CellFormat.FormatString = "#,##0.0##";
                sheet.Columns[4].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;

                sheet.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Columns[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Columns[2].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Columns[3].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Columns[4].CellFormat.WrapText = ExcelDefaultableBoolean.True;

            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();

            for (int i = 0; i < AllGrid.Count; i++)
            {
                Worksheet sheet = workbook.Worksheets.Add("Таблица"+(i+1).ToString());
                UltraGridExporter1.ExcelExporter.ExcelStartRow = 3; 
                UltraGridExporter1.ExcelExporter.Export(G, sheet); 
            }

        }

        #endregion

        #region Прочая шляпа непапвшая в другие разделы :)
        protected DataTable GetDS(string q)
        {
            DataTable dt = new DataTable(q);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(q), q, dt);
            return dt;

        }

        System.Collections.Generic.Dictionary<string, int> GenUserParam(string query)
        {
            DataTable dt = GetDS(query);
            System.Collections.Generic.Dictionary<string, int> d = new System.Collections.Generic.Dictionary<string, int>();
            foreach (DataColumn Column in dt.Columns)
            {
                if ((Column.Caption != "LD_") && (Column.Caption != "Fo_BOR"))
                {
                    LD.Value = Column.Caption;
                    d.Add(LD.Value, 0);
                }
            }
            return d;
        }
        #endregion
    }
}
