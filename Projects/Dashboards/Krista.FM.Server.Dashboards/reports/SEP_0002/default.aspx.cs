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

namespace Krista.FM.Server.Dashboards.reports.SEP_0002
{
    public partial class _default : CustomReportPage
    {
        private CustomParam LD { get { return (UserParams.CustomParam("LD")); } }
        private CustomParam LD_L { get { return (UserParams.CustomParam("LD_L")); } }

        private CustomParam SEP { get { return (UserParams.CustomParam("SEP")); } }

        private CustomParam SEP_C { get { return (UserParams.CustomParam("SEP_C")); } }
        private CustomParam SEP_C_S { get { return (UserParams.CustomParam("SEP_C_S")); } }


        private CustomParam REGION { get { return (UserParams.CustomParam("REGION")); } }
        private CustomParam REGION_FO { get { return (UserParams.CustomParam("REGIONFO")); } }

        private CustomParam EDIZM_SEP { get { return (UserParams.CustomParam("kjkjk")); } }

        string[] GridColHeder = {"Территория","Значение, рубль","Абсолютное отклонение от АППГ","Темп роста к АППГ","Отклонение (от ФО)","Ранг по ФО","Отклонение (от РФ)","Ранг по РФ" };

        string FormatGridValue = "### ### ##0.##";

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "Нет данных");
            return dt;
        }

        #region Для юзер параметров
        Dictionary<string, string> Date_BaseName_for_Param;
        Dictionary<string, string> SEP_BaseName_for_Param;

        #region Для ФО и МО
        Dictionary<string, int> GenUserParamRegions(string q)
        {
            Dictionary<string, int> res = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(q));
            string lFO = "";
            

            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {
                string FO = cs.Cells[i].Value.ToString();
                string MO = cs.Axes[1].Positions[i].Members[0].Caption;
                if (FO != lFO)
                {
                    lFO = FO;
                    res.Add(FO, 0);
                }
                res.Add(MO, 1);


            }
            return res;


        }
        #endregion

        #region Для древовидного юзер парама

        string Last = "";
        /// <summary>
        /// вовращает словарик для юзерпарама(двухуровневый тока)
        /// в Last загоняет то что последнее вытащили
        /// </summary>
        /// <param name="q">иди запроса</param>
        /// <param name="Link">словарик сопостоления типа(имя в параме, уникальное имя для запроса)</param>
        /// <param name="l1">позицыя для вычлеки верхнего уровня</param>
        /// <param name="l2">позицыя для вычленки нижнего уровня</param>
        /// <returns>а сам как думаеш?</returns>
        Dictionary<string, int> GenUserParam(string q,out Dictionary<string, string> Link,int l1,int l2)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            Link = new Dictionary<string, string>();

            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(q));
            string LYear = "";
            for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
            {   
                string year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']','.')[l1];//[7];
                string mounth = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']','.')[l2];//[13];

                if (LYear != year)
                {
                    LYear = year;
                    d.Add(year, 0);
                }
                    string NewMounth = AID(d, mounth, 1);
                    Link.Add(NewMounth, cs.Axes[1].Positions[i].Members[0].UniqueName);
                    Last = NewMounth;  
            }
            return d;
        }
        #endregion

        #region Дописывалки пробелов
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
        #endregion

        #region Для линейного множества
        Dictionary<string, int> GenUserParam(string q)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Все", 0);
            DataTable dt = GetDSForChart(q);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;

        }
        #endregion
        
        #endregion

        #region лоады
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth-15);
            G.Height = 240;

            PanelChart.AddLinkedRequestTrigger(G);
            
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
            if (!Page.IsPostBack)
            {
                #region Иницализацыя юзерпарамов
                DATEPARAM.FillDictionaryValues(GenUserParam("LD",out Date_BaseName_for_Param,10,19));
                DATEPARAM.SetСheckedState(Last, 1 == 1);

                DATEPARAM.Width = 140;

                REGIONPARAM.FillDictionaryValues(GenUserParamRegions("REGIONS"));

                REGIONPARAM.Width = 500;

                SEPPARAM.FillDictionaryValues(GenUserParam("SEP"));
                SEPPARAM.Width = 500;
                #endregion
                SEP_C_S.Value = (true).ToString();
            }
            else
            {
                //повторная иницилизацыя словарика сопостоления
                GenUserParam("LD", out Date_BaseName_for_Param, 10, 19);
            }
            
                #region парамы
                LD.Value = Date_BaseName_for_Param[DATEPARAM.SelectedValue];
                LD_L.Value = LD.Value.Insert(58, ".lag(1)");

                SEP.Value = SEPPARAM.SelectedValue == "Все" ? "[СЭП].[Годовой сборник].[Уровень 1].members" : string.Format("[СЭП].[Годовой сборник].[Раздел].[{0}].Children", SEPPARAM.SelectedValue);

                REGION.Value = DelLastsChar(REGIONPARAM.SelectedValue, ' ');
                REGION_FO.Value = DelLastsChar(REGIONPARAM.SelectedNode.Parent.Text, ' ');
                #endregion

                G.DataBind();
                
                #region парамы для чарта
                SEP_C_S.Value = (G.Rows[1].Cells[0].Text[G.Rows[1].Cells[0].Text.Length - 1] == ' ').ToString();

                EDIZM_SEP.Value = _GetString_(G.Rows[1].Cells[0].Text);
                SEP_C.Value = GetString_(G.Rows[1].Cells[0].Text);

                #endregion
                
                    C.DataBind();
                    try
                    {
                        try
                    { }
                catch { }
            }
            catch 
            {
                CL.Text = "";
                G.Bands.Clear();
                G.Rows.Clear();
                G.DisplayLayout.NoDataMessage = "Нет данных";

            }
            Hederglobal.Text = string.Format("Мониторинг социально-экономического развития по состоянию на январь - {0} {1} года", DATEPARAM.SelectedNode.Text,DATEPARAM.SelectedNode.Parent.Text);
            Page.Title = Hederglobal.Text;
        }
        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {   
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

                    if (dt.Rows[j][ColVal] != DBNull.Value)
                    {
                        try
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
                        catch { }
                    }
                    else
                    {
                        //dt.Rows[j][ColVal] = -1;
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

        Dictionary<string, System.Decimal> SetRang_(string q,string RegionName)
        {
            DataTable dt_R = GetDSForChart(q);
            for (int i = 1; i < dt_R.Columns.Count - 1; i += 2)
            {
                SetRang(dt_R, i, i + 1, 0);
            }
            //REGION_FO.Value = RegionName;
            int indexRow_FO = -1;
            for (int i = 0; dt_R.Rows.Count > i; i++)
            {
                if (dt_R.Rows[i][0].ToString() == RegionName)
                {
                    indexRow_FO = i;
                    break;
                }
            }

            Dictionary<string, System.Decimal> Rangs = new Dictionary<string, System.Decimal>();
            for (int i = 1; dt_R.Columns.Count > i; i+=2)
            {
                try
                {
                    Rangs.Add(dt_R.Columns[i].Caption.Split(';')[0], System.Decimal.Parse(dt_R.Rows[indexRow_FO][i + 1].ToString()));
                }
                catch { }

            }
            return Rangs;
        }


        #endregion

        #region Создавалка грида
        protected void G_DataBinding(object sender, EventArgs e)
        {
            G.Bands.Clear();
            G.Rows.Clear();
            DataTable dt_ = GetDSForChart("G");
            CellSet cs_ = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G"));

            DataTable res = new DataTable();
            res.Columns.Add("Показатель");

            res.Columns.Add("Val_MO",typeof(System.Decimal));
            res.Columns.Add("A0_MO", typeof(System.Decimal));
            res.Columns.Add("A1_MO", typeof(System.Decimal));
            res.Columns.Add("R0_MO", typeof(System.Decimal));
            res.Columns.Add("R1_MO", typeof(System.Decimal));

            res.Columns.Add("Val_FO", typeof(System.Decimal));
            res.Columns.Add("A0_FO", typeof(System.Decimal));
            res.Columns.Add("A1_FO", typeof(System.Decimal));

            res.Columns.Add("Val_RF", typeof(System.Decimal));
            res.Columns.Add("A0_RF", typeof(System.Decimal));
            res.Columns.Add("A1_RF", typeof(System.Decimal));
            #region Ранжирование
            //REGION.Value = "Ивановская область";
            //REGION_FO.Value = "Центральный федеральный округ";
            Dictionary<string, System.Decimal> RangsRF = SetRang_("G_R1",REGION.Value);             
            Dictionary<string, System.Decimal> RangsFO = SetRang_("G_R2", REGION.Value);

            String sep = "";


            #endregion
            int HederInGr = 0;
                for (int i = 0; i < dt_.Rows.Count; i++)
                {

                    //String.Format(cs_.Axes[1].Positions[i].Members[0].UniqueName.Split('.','[',']')[7+6]);
                    string b = cs_.Axes[1].Positions[i].Members[0].UniqueName.Split('.', '[', ']')[7 + 6];
                    if (sep != b)
                    {
                        sep = b;
                        res.Rows.Add(sep, null, null, null, null, null, null, null, null, null, null);
                        HederInGr++;
                    }

                    res.Rows.Add(dt_.Rows[i][0], null, null, null, null, null, null, null, null, null, null);
                    
                    res.Rows[i+HederInGr]["Val_MO"] = dt_.Rows[i][1];
                    res.Rows[i + HederInGr]["A0_MO"] = dt_.Rows[i][2];
                    res.Rows[i + HederInGr]["A1_MO"] = dt_.Rows[i][3];

                    try
                    {
                        res.Rows[i + HederInGr]["R1_MO"] = RangsRF[dt_.Rows[i][0].ToString()];
                        res.Rows[i + HederInGr]["R0_MO"] = RangsFO[dt_.Rows[i][0].ToString()];
                    }
                    catch 
                    {
                        //ну это тяк на всякий пожарный
                    }

                    res.Rows[i + HederInGr]["Val_FO"] = dt_.Rows[i][6];
                    res.Rows[i + HederInGr]["A0_FO"] = dt_.Rows[i][7];
                    res.Rows[i + HederInGr]["A1_FO"] = dt_.Rows[i][8];

                    res.Rows[i + HederInGr]["Val_RF"] = dt_.Rows[i][11];
                    res.Rows[i + HederInGr]["A0_RF"] = dt_.Rows[i][12];
                    res.Rows[i + HederInGr]["A1_RF"] = dt_.Rows[i][13];

                    res.Rows[i + HederInGr][0] = res.Rows[i + HederInGr][0].ToString() + ", " + dt_.Rows[i][5].ToString().ToLower() + ";" + dt_.Rows[i][4].ToString();
                }
                G.DataSource = res;//res;

        }

        ColumnHeader GenColumnHeder(string Caption, int x, int y,int spanX,int spanY)
        {
            ColumnHeader CH = new ColumnHeader(1 == 1);
            CH.RowLayoutColumnInfo.OriginX = x;
            CH.RowLayoutColumnInfo.OriginY = y;
            CH.RowLayoutColumnInfo.SpanX = spanX;
            CH.RowLayoutColumnInfo.SpanY = spanY;
            CH.Caption = Caption;
            CH.Style.Wrap = 1 == 1;
            CH.Style.HorizontalAlign = HorizontalAlign.Center;
            return CH;
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ##0.##");
                e.Layout.Bands[0].Columns[i].Width = 77;
                e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY++;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;

            }
            e.Layout.Bands[0].Columns[1].Header.Caption = "Значение";
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Bold = 1 == 1;
            e.Layout.Bands[0].Columns[2].Header.Caption = "Абсолютное отклонение от АППГ";
            e.Layout.Bands[0].Columns[3].Header.Caption = "Темп роста к АППГ";
            e.Layout.Bands[0].Columns[4].Header.Caption = "Ранг по ФО";                        
            e.Layout.Bands[0].Columns[5].Header.Caption = "Ранг по РФ";

            e.Layout.Bands[0].Columns[6].Header.Caption = "Значение";
            e.Layout.Bands[0].Columns[6].Header.Style.Font.Bold = 1 == 1;
            e.Layout.Bands[0].Columns[7].Header.Caption = "Абсолютное отклонение от АППГ";
            e.Layout.Bands[0].Columns[8].Header.Caption = "Темп роста к АППГ";

            e.Layout.Bands[0].Columns[9].Header.Caption = "Значение";
            e.Layout.Bands[0].Columns[9].Header.Style.Font.Bold = 1 == 1;
            e.Layout.Bands[0].Columns[10].Header.Caption = "Абсолютное отклонение от АППГ";
            e.Layout.Bands[0].Columns[11].Header.Caption = "Темп роста к АППГ";
            

            e.Layout.Bands[0].Columns[0].Width = 320;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;

            e.Layout.Bands[0].HeaderLayout[0].RowLayoutColumnInfo.SpanY = 0; 

            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(REGION.Value, 1, 0, 5, 1));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder(REGION_FO.Value, 6, 0, 3, 1));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("Российская Федерация", 9, 0, 3, 1));
            e.Layout.Bands[0].HeaderLayout.Add(GenColumnHeder("Показатель", 0, 0, 1, 2));

        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Text.Split(';').Length > 1)
            {
                Reverce = (e.Row.Cells[0].Text.Split(';')[1][0] == '1');
                e.Row.Cells[0].Text = e.Row.Cells[0].Text.Split(';')[0] + (Reverce ? " " : "");


                #region Стрелочки
                if (e.Row.Cells[3].Value != null)
                {
                    e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                    if ((System.Decimal)(e.Row.Cells[3].Value) <= 100)
                    {
                        e.Row.Cells[3].Style.BackgroundImage = Reverce ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                    }
                    else
                    {
                        e.Row.Cells[3].Style.BackgroundImage = (Reverce ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png");
                    }

                }

                if (e.Row.Cells[8].Value != null)
                {
                    e.Row.Cells[8].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                    if ((System.Decimal)(e.Row.Cells[8].Value) <= 100)
                    {
                        e.Row.Cells[8].Style.BackgroundImage = Reverce ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                    }
                    else
                    {
                        e.Row.Cells[8].Style.BackgroundImage = (Reverce ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png");
                    }

                }

                if (e.Row.Cells[10].Value != null)
                {
                    e.Row.Cells[10].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                    if ((System.Decimal)(e.Row.Cells[10].Value) <= 100)
                    {
                        e.Row.Cells[10].Style.BackgroundImage = Reverce ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                    }
                    else
                    {
                        e.Row.Cells[10].Style.BackgroundImage = (Reverce ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png");
                    }

                }
                #endregion
                #region Звёздочки
                if (e.Row.Cells[4].Value != null)
                    if ((System.Decimal)(e.Row.Cells[4].Value) == 1)
                    {
                        e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                        e.Row.Cells[4].Style.BackgroundImage = !Reverce ? "~/images/starYellowBB.png" : "~/images/starGrayBB.png";
                    }
                #endregion
            }
            else
            {
                e.Row.Cells[0].ColSpan = 12;
                e.Row.Cells[0].Style.Font.Bold = 1 == 1;                
            }
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            UltraGridRow Row = e.Row;
            if (e.Row.Cells[0].Style.Font.Bold)
            {
                Row = Row.NextRow;
            }
         
            SEP_C_S.Value = (Row.Cells[0].Text[e.Row.Cells[0].Text.Length - 1] == ' ').ToString();
            SEP_C.Value = GetString_(Row.Cells[0].Text);
            EDIZM_SEP.Value = _GetString_(Row.Cells[0].Text);
            C.DataBind();
        }
        #endregion

        #region Создавалка диограмки
        bool sopos = 1 == 2;
        protected void C_DataBinding(object sender, EventArgs e)
        {
            LD.Value = "";
            
            foreach (string s in Date_BaseName_for_Param.Keys)
            {
                LD.Value += Date_BaseName_for_Param[s] + ",";
            }
            LD.Value = LD.Value.Remove(LD.Value.Length - 1, 1);

            DataTable dt = GetDSForChart("C_");
            String.Format(SEP_C_S.Value);
            if (bool.Parse(SEP_C_S.Value))
            {
                #region Биндинг диограмки

                C.ChartType = ChartType.LineChart;
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("C_"));
                System.Decimal max = System.Decimal.MinValue;
                System.Decimal min = System.Decimal.MaxValue;

                for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
                {
                    dt.Rows[i][0] = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']', '.')[10] + ", " +
                        cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']', '.')[19];
                    try
                    {
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            if ((System.Decimal)(dt.Rows[i][j]) > max)
                            {   
                                max = (System.Decimal)(dt.Rows[i][j]);
                            }
                            if ((System.Decimal)(dt.Rows[i][j]) < min)
                            {
                                min = (System.Decimal)(dt.Rows[i][j]);
                            }
                        }
                    }
                    catch { //на случай если тама нул будет
                    }
                }
                C.DataSource = dt;
                #endregion

                #region Кофнфа для первой вероятностной диограмки
                C.Axis.Y.RangeMax = (System.Double)(max);
                C.Axis.Y.RangeMin = (System.Double)(min);
                C.Axis.Y.RangeType = AxisRangeType.Custom;
                C.Data.SwapRowsAndColumns = 1 == 1;

                C.Axis.X.Extent = 90;

                C.Axis.X.Margin.Far.Value = 15;
                C.Axis.X.Margin.Far.MarginType = LocationType.Pixels;

                C.Axis.X.Margin.Near.Value = 15;
                C.Axis.X.Margin.Near.MarginType = LocationType.Pixels;

                C.Tooltips.FormatString = "<b><DATA_VALUE:##    # ### ### ##0.##></b>";

                C.LineChart.ChartText.Add(new ChartTextAppearance());
                C.LineChart.ChartText[0].Column = -2;
                C.LineChart.ChartText[0].Row = -2;
                C.LineChart.ChartText[0].Visible = 1 == 1;
                C.LineChart.ChartText[0].VerticalAlign = StringAlignment.Far;

                C.LineChart.Thickness = 6;

                

                #endregion
            }
            else
            {
                #region DataBind
                DataTable dt_c = new DataTable();
                dt_c.Columns.Add("Год");
                dt_c.Columns.Add("Январь",typeof(System.Decimal));
                dt_c.Columns.Add("Февраль", typeof(System.Decimal));
                dt_c.Columns.Add("Март", typeof(System.Decimal));
                dt_c.Columns.Add("Апрель", typeof(System.Decimal));
                dt_c.Columns.Add("Май", typeof(System.Decimal));
                dt_c.Columns.Add("Июнь", typeof(System.Decimal));
                dt_c.Columns.Add("Июль", typeof(System.Decimal));
                dt_c.Columns.Add("Август", typeof(System.Decimal));
                dt_c.Columns.Add("Сентябрь", typeof(System.Decimal));
                dt_c.Columns.Add("Октябрь", typeof(System.Decimal));
                dt_c.Columns.Add("Ноябрь", typeof(System.Decimal));
                dt_c.Columns.Add("Декабрь", typeof(System.Decimal));


                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("C_"));

                System.Decimal Max = System.Decimal.MinValue;
                System.Decimal Min = System.Decimal.MaxValue;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string Year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']', '.')[10];
                    int RowYear = -1;
                    for (int j = 0; j < dt_c.Rows.Count; j++)
                    {
                        if(dt_c.Rows[j][0].ToString() ==  Year)
                        {
                            RowYear = j;
                            break;
                        }
                    }
                    if (RowYear == -1)
                    {
                        
                        dt_c.Rows.Add(Year, null, null, null, null, null, null, null, null, null, null, null, null);
                        RowYear = dt_c.Rows.Count-1;
                    }
                    System.Decimal Val = (System.Decimal)(dt.Rows[i][1]);
                    dt_c.Rows[RowYear][cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']', '.')[19]] = Val;//dt.Rows[i][1];
                    
                    if (Max < (System.Decimal)(dt.Rows[i][1]))
                    {
                        Max = (System.Decimal)(dt.Rows[i][1]);
                    }
                    if (Min > (System.Decimal)(dt.Rows[i][1]))
                    {
                        Min = (System.Decimal)(dt.Rows[i][1]);
                    }
                }                
                C.DataSource = dt_c;
            #endregion

                C.ChartType = ChartType.AreaChart;//StackAreaChart;
                C.Axis.X.Extent = 40;
                C.AreaChart.NullHandling = NullHandling.DontPlot;

                C.Axis.Y.RangeMax = Max > 0 ? (System.Double)(Max) * 1.1 : (System.Double)(Max) * 0.9;
                C.Axis.Y.RangeMin = Min > 0 ? (System.Double)(Min) * 0.9 : (System.Double)(Min) * 1.1;
                C.Axis.Y.RangeType = AxisRangeType.Custom;

                C.Axis.X.Margin.Far.Value = 15;
                C.Axis.X.Margin.Far.MarginType = LocationType.Pixels;

                C.Axis.X.Margin.Near.Value = 15;
                C.Axis.X.Margin.Near.MarginType = LocationType.Pixels;

                C.AreaChart.ChartText.Add(new ChartTextAppearance());
                C.AreaChart.ChartText[0].Column = -2;
                C.AreaChart.ChartText[0].Row = -2;
                C.AreaChart.ChartText[0].Visible = 1 == 1;
                C.AreaChart.ChartText[0].VerticalAlign = StringAlignment.Far;
                C.AreaChart.ChartText[0].ItemFormatString = "<DATA_VALUE:### ### ### ##0.##>";

                C.Axis.X.Labels.Visible = 1 == 1;
                C.Axis.X.Labels.SeriesLabels.Visible = 1 == 1;
                C.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
                C.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
                C.Data.SwapRowsAndColumns = 1 == 2;
                C.Tooltips.FormatString = "<b><DATA_VALUE:### ### ### ##0.##></b>";                
            }

            CL.Text = string.Format("Динамика показателя «{0}», {1}",SEP_C.Value,EDIZM_SEP.Value);
            C.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
        }

        protected void C_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }
       #endregion

        #region hiden
        private string GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) ;
            for (int j = 0; j < i; j++)
            {
                res += s[j];
            }
            return res;


        }


        private string _GetString_(string s)
        {
            string res = "";
            int i = 0;
            for (i = s.Length - 1; s[i] != ','; i--) { res = s[i] + res; };

            return res;


        }
        #endregion

        


    }
}
