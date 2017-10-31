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
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebChart;

using System.Globalization;

using Infragistics.Documents.Reports.Report.Text;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Infragistics.WebUI.UltraWebNavigator;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using Orientation = Infragistics.Documents.Excel.Orientation;
using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;

using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.SEP_0001
{
    public partial class _default : CustomReportPage
    {
        private CustomParam LD { get { return (UserParams.CustomParam("LD")); } }
        private CustomParam LD_L { get { return (UserParams.CustomParam("LD_L")); } }
        private CustomParam SEP { get { return (UserParams.CustomParam("SEP")); } }
        private CustomParam REGION { get { return (UserParams.CustomParam("REGION")); } }

        string[] GridColHeder = {"Территория","Значение, рубль","Абсолютное отклонение от АППГ","Темп роста к АППГ","Отклонение (от ФО)","Ранг по ФО","Отклонение (от РФ)","Ранг по РФ" };

        string FormatGridValue = "### ### ##0.##";

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "Нет данных");
            return dt;
        }

        #region Для ерархичных параметров
        Dictionary<string, string> Date_BaseName_for_Param;
        Dictionary<string, string> SEP_BaseName_for_Param;

        Dictionary<string, int> GenUserParam(string q,out Dictionary<string, string> Link,int l1,int l2)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            Link = new Dictionary<string, string>();

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(q));
            string LYear = "";
            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {   
                string year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[l1];//[7];
                string mounth = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[l2];//[13];

                if (LYear != year)
                {
                    LYear = year;
                    d.Add(year, 0);
                }
                    string NewMounth = AID(d, mounth, 1);
                    Link.Add(NewMounth, cs.Axes[1].Positions[i].Members[0].UniqueName);
                    SEP.Value = NewMounth;
                    LD.Value = NewMounth;

                
                
                
            }
            return d;
        }




        string AID(Dictionary<string, int> d, string str, int level)
        {
            string lev = "";
            for (; ; )
            {
                try
                {
                    d.Add(str + " " + lev, level);
                    break;
                }
                catch
                { }
                lev += " ";
            }
            return str + " " + lev;
        }

        string DelLastsChar(string s, Char c)
        {
            for (int i = s.Length - 1; i > 0; i--)
            {
                if (s[i] == c)
                {
                    s = s.Remove(i, 1);
                }
                else
                {
                    break;
                }
            }
            return s;

        }
        Dictionary<string, int> GenUserParam(string q)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();

            DataTable dt = GetDSForChart(q);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;

        }
        #endregion
        
        #region лоады
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5-5);
            G.Height = Unit.Empty;

            C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 18-5);

            map.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15-5);
            map.Height = CustomReportConst.minScreenWidth/2 - 15;

            #region toExcel
            UltraGridExporter1.PdfExportButton.Visible = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            #endregion 
        }

        bool Reverce = 1 == 2;
        bool IsCompare = 1 == 2;
        string ezm = "";


        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FOPARAM.ParentSelect = 1 == 2;
                    SEPPARAM.ParentSelect = 1 == 2;
                    REGIONPARAM.ParentSelect = 1 == 2;

                    FOPARAM.FillDictionaryValues(GenUserParam("AllMount", out Date_BaseName_for_Param, 7, 13));
                    FOPARAM.Width = 140;
                    FOPARAM.SetСheckedState(LD.Value, 1 == 1);

                    LD.Value = "[Период].[Год Квартал Месяц].[Данные всех периодов]";
                    SEPPARAM.FillDictionaryValues(GenUserParam("AllSEP", out SEP_BaseName_for_Param, 9, 11));
                    SEPPARAM.Width = 700;


                    LD.Value = Date_BaseName_for_Param[FOPARAM.SelectedValue];

                    SEPPARAM.SetСheckedState(GetDSForChart("AllSEP").Rows[0][0].ToString() + " ", 1 == 1);

                    REGIONPARAM.FillDictionaryValues(GenUserParam("FO"));
                    REGIONPARAM.Width = 300;


                }
                else
                {
                    GenUserParam("AllMount", out Date_BaseName_for_Param, 7, 13);
                    LD.Value = Date_BaseName_for_Param[FOPARAM.SelectedValue];
                    GenUserParam("AllSEP", out SEP_BaseName_for_Param, 9, 11);
                }

                LD.Value = Date_BaseName_for_Param[FOPARAM.SelectedValue];
                LD_L.Value = LD.Value.Insert(58, ".lag(1)");
                SEP.Value = SEP_BaseName_for_Param[SEPPARAM.SelectedValue];

                REGION.Value = REGIONPARAM.SelectedValue;

                //Определяем, как ща показатель(сопостовимый, обрантный и единичку измерения)
                DataTable dt = GetDSForChart("Compare_And_Reverce");
                {
                    Reverce = dt.Rows[0][1].ToString() == "1";
                    IsCompare = dt.Rows[0][2].ToString() == "1";
                    ezm = dt.Rows[0][3].ToString().ToLower();
                }
                Hederglobal.Text = string.Format("Мониторинг социально-экономического развития по состоянию на {0} - {1} года (в разрезе субъектов РФ)", FOPARAM.SelectedValue, FOPARAM.SelectedNode.Parent.Text);
                Page.Title = Hederglobal.Text;
                CL.Text = string.Format("Распределение субъектов РФ по показателю «{0}», {1}", SEPPARAM.SelectedValue, ezm);
                Label1.Text = string.Format("{0}, {1}", SEPPARAM.SelectedValue, ezm);


                SetMapSettings();
                G.DataBind();
                C.DataBind();
            }
            catch {
                G.Bands.Clear();
                G.DisplayLayout.NoDataMessage = "Нету данных";
                C.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(C_InvalidDataReceived);
                map.Shapes.Clear();
            }
            

        }
        #endregion

        #region Собиралка грида +загонялка данных на карту
        #region Ранжирование для грида
        void SetRang(DataTable dt, int ColVal, int ColRang, int StartRow)
        {
            int RowCount = dt.Rows.Count;
            int L_max = StartRow;
            int L_min = StartRow;
            for (int i = StartRow; i < RowCount; i++)
            {
                for (int j = StartRow; j < RowCount; j++)
                {
                    if (((System.Decimal)(dt.Rows[j][ColVal]) >= (System.Decimal)(dt.Rows[L_max][ColVal])) && (dt.Rows[j][ColRang] == DBNull.Value))
                    {
                        L_max = j;
                    }
                    if (((System.Decimal)(dt.Rows[j][ColVal]) < (System.Decimal)(dt.Rows[L_min][ColVal])) && (dt.Rows[j][ColRang] == DBNull.Value))
                    {
                        L_min = j;
                    }
                }
                if (Reverce)
                {
                    dt.Rows[L_max][ColRang] = RowCount - (i - StartRow + 2);
                }
                else
                {
                    dt.Rows[L_max][ColRang] = i - StartRow + 1;
                }

                L_max = L_min;
            }





        }

        #endregion
        DataTable AllData = null;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt_data = GetDSForChart("G");
            DataTable dt_res = new DataTable();

            //в сенеграфе пригадится, небудем его терять
            AllData = dt_data.DefaultView.ToTable();

            dt_res.Columns.Add("NAME");
            dt_res.Columns.Add("VALUE", typeof(decimal));

            dt_res.Columns.Add("APPG", typeof(decimal));
            dt_res.Columns.Add("APPG2", typeof(decimal));

            dt_res.Columns.Add("-FO", typeof(decimal));
            dt_res.Columns.Add("RFO", typeof(decimal));

            dt_res.Columns.Add("-RF", typeof(decimal));
            dt_res.Columns.Add("RRF", typeof(decimal));

            dt_res.Columns.Add("miniName", typeof(string));

            //строка с РФ
            dt_res.Rows.Add(dt_data.Rows[0][0], dt_data.Rows[0][2], dt_data.Rows[0][3], dt_data.Rows[0][4], null, null, null, null);
            //строка с фо
            dt_res.Rows.Add(dt_data.Rows[1][0], dt_data.Rows[1][2], dt_data.Rows[1][3], dt_data.Rows[1][4], null, null, null, null);

            for (int i = 2; i < dt_data.Rows.Count; i++)
            {
                dt_res.Rows.Add(null, null, null, null, null, null, null, null);                

                dt_res.Rows[i]["NAME"] = dt_data.Rows[i][0];
                dt_res.Rows[i]["VALUE"] = dt_data.Rows[i][2];
                dt_res.Rows[i]["APPG"] = dt_data.Rows[i][3];
                dt_res.Rows[i]["APPG2"] = dt_data.Rows[i][4];
                dt_res.Rows[i]["miniName"] = dt_data.Rows[i][5];               
                dt_res.Rows[i]["RFO"] = DBNull.Value;

                if (IsCompare)
                {
                    dt_res.Rows[i]["-FO"] = (System.Decimal)(dt_res.Rows[1]["VALUE"]) - (System.Decimal)(dt_res.Rows[i]["VALUE"]);
                    dt_res.Rows[i]["-RF"] = (System.Decimal)(dt_res.Rows[0]["VALUE"]) - (System.Decimal)(dt_res.Rows[i]["VALUE"]);
                }
                
                dt_res.Rows[i]["RRF"] = DBNull.Value;
            }
            //Ранги по ФО
            SetRang(dt_res, 1, 5, 2);
            
            for (int i = 0; i < dt_res.Rows.Count; i++)            
            {
                SetShape(dt_res.Rows[i][0].ToString(), dt_res.Rows[i]["miniName"].ToString(), (System.Decimal)(dt_res.Rows[i][1]), dt_res.Rows[i][5]);    
            }

            //Усё, болше мини нейм нам не пригадится =(
            dt_res.Columns.Remove("miniName");
            DataTable dt_rl = GetDSForChart("G_lr");


            //Ранги по РФ
            SetRang(dt_rl, 1, 2, 2);
            for (int i = 0; i < dt_res.Rows.Count; i++)
            {
                for (int j = 0; j < dt_rl.Rows.Count; j++)
                {
                    if (dt_res.Rows[i][0].ToString() == dt_rl.Rows[j][0].ToString())
                    {
                        dt_res.Rows[i][7] = dt_rl.Rows[j][2];
                    }
                }
            }


            maxRang = dt_res.Rows.Count - 2;

            G.DataSource = dt_res;
        }
        int maxRang = 0;
        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {

            foreach (UltraGridColumn col in e.Layout.Bands[0].Columns)
            {
                col.Header.Caption = GridColHeder[col.Index];
                col.Header.Style.Wrap = 1 == 1;
                col.Header.Style.HorizontalAlign = HorizontalAlign.Center;
                if (col.Index == 0) continue;
                CRHelper.FormatNumberColumn(col, FormatGridValue);
                col.CellStyle.Wrap = 1 == 1;
                col.Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.66 / (e.Layout.Bands[0].Columns.Count - 1));
            }
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.26);
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("{0}, {1}","Значение", ezm);

        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.BorderDetails.ColorLeft = Color.LightGray;
            if (e.Row.Index < 2)
            {
                e.Row.Style.Font.Bold = 1 == 1;
            }
            else
            {
                e.Row.Cells[1].Style.Font.Bold = 1 == 1;
            }

            #region Картинки в студию!
            e.Row.Cells[3].Style.CustomRules = e.Row.Cells[5].Style.CustomRules = e.Row.Cells[7].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            if (e.Row.Cells[5].Value != null)
            {
                if ((System.Decimal)(e.Row.Cells[5].Value) == 1)
                {
                    e.Row.Cells[5].Style.BackgroundImage = !Reverce ? "~/images/starYellowBB.png" : "~/images/starGrayBB.png";
                }
                else
                    if ((System.Decimal)(e.Row.Cells[5].Value) == maxRang)
                    {
                        e.Row.Cells[5].Style.BackgroundImage = Reverce ? "~/images/starYellowBB.png" : "~/images/starGrayBB.png";
                    }
                if ((System.Decimal)(e.Row.Cells[7].Value) == 1)
                {
                    e.Row.Cells[7].Style.BackgroundImage = !Reverce ? "~/images/starYellowBB.png" : "~/images/starGrayBB.png";
                }
                else
                    if ((System.Decimal)(e.Row.Cells[7].Value) == maxRang)
                    {
                        e.Row.Cells[7].Style.BackgroundImage = Reverce ? "~/images/starYellowBB.png" : "~/images/starGrayBB.png";
                    }
            }
            else
            {
             //   e.Row.Cells[4].Value = e.Row.Cells[5].Value = e.Row.Cells[6].Value = e.Row.Cells[7].Value = "-";
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Value == null)
                {
                    e.Row.Cells[i].Value = "-";
                }

            }


                if ((System.Decimal)(e.Row.Cells[3].Value) > 0)
                {
                    e.Row.Cells[3].Style.BackgroundImage = (Reverce ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png");
                    e.Row.Cells[3].Value = "+" + string.Format("{0:" + FormatGridValue + "}%", e.Row.Cells[3].Value);
                }
                else
                    if ((System.Decimal)(e.Row.Cells[3].Value) < 0)
                    {
                        e.Row.Cells[3].Style.BackgroundImage = (Reverce ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png");
                    }
                    else { }
            #endregion

        }
        #endregion

        #region Диограма
        protected void C_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("region",typeof(string));
            dt.Columns.Add("value", typeof(System.Decimal));

            System.Decimal max = System.Decimal.MinValue;
            System.Decimal min = System.Decimal.MaxValue;
            for (int i = 0; i < AllData.Rows.Count; i++)
            {
                if (i > 1)
                {
                    dt.Rows.Add(AllData.Rows[i][0], AllData.Rows[i][2]);
                }
                if ((System.Decimal)(AllData.Rows[i][2]) > max)
                {
                    max = (System.Decimal)(AllData.Rows[i][2]);
                }
                if ((System.Decimal)(AllData.Rows[i][2]) < min)
                {
                    min = (System.Decimal)(AllData.Rows[i][2]);
                }                
            
            }
            C.DataSource = dt.DefaultView;
            if (IsCompare)
            {
                C.Axis.Y.RangeMax = (System.Double)(max) * 1.1;
                C.Axis.Y.RangeMin = (System.Double)(min) * 0.9;
                C.Axis.Y.RangeType = AxisRangeType.Custom;
            }
            else
            {
                C.Axis.Y.RangeType = AxisRangeType.Automatic;
            }

            C.Tooltips.FormatString = "<ITEM_LABEL><br><b><DATA_VALUE:### ##0.##></b>, " + ezm;
            

            
        }
        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        protected void C_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (IsCompare)
            {
                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                Infragistics.UltraChart.Core.Primitives.Line l = new Infragistics.UltraChart.Core.Primitives.Line(
                    new Point((int)xAxis.Map(xAxis.Minimum), (int)yAxis.Map((System.Decimal)(AllData.Rows[0][2]))),
                    new Point((int)xAxis.Map(xAxis.Maximum), (int)(yAxis.Map((System.Decimal)(AllData.Rows[0][2])))));
                l.lineStyle.DrawStyle = LineDrawStyle.Dot;
                l.PE.Stroke = Color.Red;
                l.PE.StrokeWidth = 2;

                e.SceneGraph.Add(l);
                Text textLabel = new Text();
                textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
                textLabel.PE.Fill = System.Drawing.Color.Black;
                textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 8);
                textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
                textLabel.labelStyle.VerticalAlign = StringAlignment.Near;
                textLabel.bounds = new System.Drawing.Rectangle( (int)(xAxis.MapMaximum)-(220)//(int)xAxis.Map(xAxis.Minimum)
                    , (int)yAxis.Map((System.Decimal)(AllData.Rows[0][2]))-13,300,13);
                textLabel.SetTextString(String.Format("   {2} {0:### ### ##0.##}, {1}", AllData.Rows[0][2], ezm, "Российская Федерация"));

                e.SceneGraph.Add(textLabel);

                l = new Infragistics.UltraChart.Core.Primitives.Line(
                new Point((int)xAxis.Map(xAxis.Minimum), (int)yAxis.Map((System.Decimal)(AllData.Rows[1][2]))),
                new Point((int)xAxis.Map(xAxis.Maximum), (int)(yAxis.Map((System.Decimal)(AllData.Rows[1][2])))));
                l.lineStyle.DrawStyle = LineDrawStyle.Dot;
                l.PE.Stroke = Color.Blue;
                l.PE.StrokeWidth = 2;

                e.SceneGraph.Add(l);
                textLabel = new Text();
                textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
                textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
                textLabel.PE.Fill = System.Drawing.Color.Black;
                textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 8);
                textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
                textLabel.labelStyle.VerticalAlign = StringAlignment.Near;
                textLabel.bounds = new System.Drawing.Rectangle((int)xAxis.Map(xAxis.Minimum), (int)yAxis.Map((System.Decimal)(AllData.Rows[1][2])) - 13, 300, 13);
                textLabel.SetTextString(String.Format("   {2} {0:### ### ##0.##}, {1}", AllData.Rows[1][2], ezm,REGIONPARAM.SelectedValue));
                e.SceneGraph.Add(textLabel);

            }
        }
        #endregion

        #region mapa

        void SetMapSettings()
        {

            #region Настройка карты

            

            map.Meridians.Visible = false;
            map.Parallels.Visible = false;
            map.ZoomPanel.Visible = 1==1;
            map.ZoomPanel.Dock = PanelDockStyle.Right;
            map.NavigationPanel.Visible = 1 == 1;
            map.NavigationPanel.Dock = PanelDockStyle.Right;
            map.Viewport.EnablePanning = true;

            

            // добавляем поля для раскраски
            map.ShapeFields.Clear();
            map.ShapeFields.Add("Name");
            map.ShapeFields["Name"].Type = typeof(string);
            map.ShapeFields["Name"].UniqueIdentifier = true;
            map.ShapeFields.Add("IndicatorValue");
            map.ShapeFields["IndicatorValue"].Type = typeof(decimal);
            map.ShapeFields["IndicatorValue"].UniqueIdentifier = false;

            #endregion

            // добавляем легенду
            map.Legends.Clear();
            // добавляем легенду раскраски
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Title = "Значение";
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            //legend1.Size.Width = 500f;
            legend1.AutoFitMinFontSize = 7;

            map.Legends.Add(legend1);

            // добавляем правила раскраски
            map.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorValueRule";
            rule.Category = String.Empty;
            rule.ShapeField = "IndicatorValue";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Reverce ? Color.Green : Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = !Reverce ? Color.Green : Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";

            rule.LegendText = "#FROMVALUE{N1} - #TOVALUE{N1}";//"LegendText";
            map.ShapeRules.Add(rule);




            map.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", CaseRegion(REGIONPARAM.SelectedValue))), "NAME", true);

            
        }

        string CaseRegion(string _)
        {
            switch (_)
            {
                    //Если бы каталог назывался не УрФо то можно было бы нормаьлно реализвать
                case "Центральный федеральный округ":{ return "ЦФО";}
                case "Северо-Западный федеральный округ":{ return "СФО";}
                case "Южный федеральный округ":{ return "ЮФО";}
                case "Северо-Кавказский федеральный округ":{ return "СКФО";}
                case "Приволжский федеральный округ":{ return "ПФО";}
                case "Уральский федеральный округ":{ return "УрФО";}
                case "Сибирский федеральный округ":{ return "СФО";}
                case "Дальневосточный федеральный округ":{ return "ДФО";}
            }
            return _;


        }
        
        public Shape FindMapShape(string nameFO)
        {
            //nameFO = nameFO.Split(' ')[0];
            for (int i = 0; i < map.Shapes.Count;i++ )
            {
                
                
                    if (nameFO == map.Shapes[i]["NAME"].ToString())
                    {
                        return map.Shapes[i];
                    }
                

            }
            return null;
        }

        public void SetShape(string nameFO,string nameMini, System.Decimal val, object rang)
        {


            Shape sh = FindMapShape(nameMini);
            if (sh == null) return;
            
            sh["IndicatorValue"] = val;
            sh.Text = string.Format("{0}\n{1:### ### ##0.##}", nameFO,val);
            sh.Visible = 1 == 1;
            sh.TextVisibility = TextVisibility.Shown;

            if (rang != DBNull.Value)
            {
                sh.ToolTip = string.Format("{0}\n{1:### ### ##0.##}, {2}\nРанг по ФО:{3}", nameFO, val, ezm, rang);
            }
            else
            {
                sh.ToolTip = string.Format("{0}\n{1:### ### ##0.##}, {2}", nameFO, val, ezm);
            }

            
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            //int width = 100;
            //e.CurrentWorksheet.Columns[0].Width = 50 * 37;
            //e.CurrentWorksheet.Columns[1].Width = 250 * 37;
            //e.CurrentWorksheet.Columns[2].Width = width * 37;
            //e.CurrentWorksheet.Columns[3].Width = width * 37;
            //e.CurrentWorksheet.Columns[4].Width = width * 37;
            //e.CurrentWorksheet.Columns[5].Width = width * 37;
            //e.CurrentWorksheet.Columns[6].Width = width * 37;
            //e.CurrentWorksheet.Columns[7].Width = width * 37;
            //e.CurrentWorksheet.Columns[8].Width = width * 37;
            //e.CurrentWorksheet.Columns[9].Width = width * 37;
            //e.CurrentWorksheet.Columns[10].Width = width * 37;
            //e.CurrentWorksheet.Columns[11].Width = width * 37;
            //e.CurrentWorksheet.Columns[12].Width = width * 37;
            //e.CurrentWorksheet.Columns[13].Width = width * 37;
            //e.CurrentWorksheet.Columns[14].Width = width * 37;

            //e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "0";
            //e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "";

            //int columnCount = G.Columns.Count;
            //int rowsCount = 100 + 5;

            //// расставляем стили у начальных колонок
            //for (int i = 4; i < rowsCount; i++)
            //{
            //    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            //    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;

            //    e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
            //    e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            //    e.CurrentWorksheet.Rows[i].Height = 20 * 37;
            //}

            // расставляем стили у ячеек
        //    for (int i = 2; i < columnCount; i += 2)
        //    {
        //        for (int j = 4; j < rowsCount; j++)
        //        {
        //            e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
        //            e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
        //            e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FormatString = "#,##0.####;[Red]-#,##0.####";
        //            double value;
        //            if (e.CurrentWorksheet.Rows[j].Cells[i].Value != null && Double.TryParse(e.CurrentWorksheet.Rows[j].Cells[i].Value.ToString(), out value))
        //            {
        //                e.CurrentWorksheet.Rows[j].Cells[i].Value = value;
        //            }

        //            e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.Alignment = HorizontalCellAlignment.Right;
        //            e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
        //            e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.FormatString = "#,##0";

        //            int value1;
        //            if (e.CurrentWorksheet.Rows[j].Cells[i + 1].Value != null && Int32.TryParse(e.CurrentWorksheet.Rows[j].Cells[i + 1].Value.ToString(), out value1))
        //            {
        //                e.CurrentWorksheet.Rows[j].Cells[i + 1].Value = value1;
        //            }

        //            //e.CurrentWorksheet.Workbook.Styles.
        //        }
        //    }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 2;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            
        }

        #endregion

    }
}
