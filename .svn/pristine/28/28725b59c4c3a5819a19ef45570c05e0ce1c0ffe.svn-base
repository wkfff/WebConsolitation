using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls; 
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel; 
using System.Text;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using Dundas.Maps.WebControl;
using System.Drawing.Imaging;
using Infragistics.UltraGauge.Resources;


namespace Krista.FM.Server.Dashboards.reports.EO.EO_0005.EO_0005_MP.EO_0005_MP_001._default.E01
{
    public partial class _default : CustomReportPage
    {
        #region Для этово, как там его, блин, аааа параметра ирархичного с датами!

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
                {
                }
                lev += " ";
            }
            return str + " " + lev;
        }

        Dictionary<string, int> GenDistonary1(string sql)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            
            string year = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[7];
            
            string poly = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[9];
            string qvart = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[11];

            string mounth = cs.Axes[1].Positions[0].Members[0].UniqueName.Split('[', ']')[13];
            
            

            AID(d, year, 0);

            AID(d, mounth + " " + year + " года", 1);
            

            for (int i = 1; i < cs.Axes[1].Positions.Count; i++)
            {
                if (year != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7])
                {
                    year = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[7];
                    AID(d, year, 0);
                    mounth = "";
                    qvart = "";
                    poly = "";
                }

                if (mounth != cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13])
                {
                    mounth = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('[', ']')[13];
                    ls = AID(d, mounth + " "+year +" года", 1);
                }
            }
            string subS = "";
            ls2 = cs.Axes[1].Positions[cs.Axes[1].Positions.Count - 1].Members[0].UniqueName;            
            return d;
        }
        string ls2 = "";
        string ls = "";
        Dictionary<string, int> GenDistonary(string sql)
        {
            return null;

        }
        
        #endregion

        static public class ForMarks
        {

            public static ArrayList Getmarks(string prefix)
            {
                ArrayList AL = new ArrayList();

                string CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + "1");
                int i = 2;
                while (!string.IsNullOrEmpty(CurMarks))
                {
                    AL.Add(CurMarks.ToString());

                    CurMarks = RegionSettingsHelper.Instance.GetPropertyValue(prefix + i.ToString());

                    i++;
                }

                return AL;
            }

            public static CustomParam SetMarks(CustomParam param, ArrayList AL, params bool[] clearParam)
            {
                if (clearParam.Length > 0 && clearParam[0]) { param.Value = ""; }
                int i;
                for (i = 0; i < AL.Count - 1; i++)
                {
                    param.Value += AL[i].ToString() + ",";
                }
                param.Value += AL[i].ToString();

                return param;
            }


        }

        protected ArrayList GenMounth()
        {
            ArrayList Mounth = new ArrayList();
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Январь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Февраль]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Март]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Апрель]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Май]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Июнь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Июль]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Август]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Сентябрь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Октябрь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Ноябрь]");
            Mounth.Add("[Период].[Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Декабрь]");
            return Mounth;
        }

        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            string s = DataProvider.GetQueryText(sql);
            DataProvidersFactory.PrimaryMASDataProvider.PopulateDataTableForChart(DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s), dt, "Наименование целевой программы");
            return dt;
        }


        CustomParam marks;

        private CustomParam p1;
        private CustomParam p2;
        private CustomParam p3;
        private CustomParam p4;
        private CustomParam p5;
        private CustomParam p6;
        private CustomParam p7;
        private CustomParam p8;

        string BN = "IE";


        Dictionary<string, int> GetParam(string sql)
        {
            DataTable dt = GetDSForChart(sql);
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Все заказчики", 0);
            foreach (DataRow dr in dt.Rows)
            {
                d.Add(dr.ItemArray[0].ToString(), 0);
            }
            return d;
        }


        /// <summary>
        /// Генерит гадж картинку в темпори имайже, занчение окуругляется шоб дубляжа менбше было
        /// использует гаджет каторый уже есть на страничке,
        /// да и госпада разработчики(если вам взгрустулось глянуть мой код конеш. Остальным не читать!) из соседнего отдела, судя по тендециии моей пастоновки гаджи они хотят активно использовать
        /// так что этот метод разумно было бы в ядро закинуть :)
        /// </summary>
        /// <param name="value">Значение для гаджи</param>
        /// <param name="prefix">Префих ну чёт таипа ../../../../TemporaryImages/megaGadgz_mo_mo</param>
        /// <param name="width">высота</param>
        /// <param name="height">ширина</param>
        /// <returns> prefix+ value.tostring()+.png</returns>
        protected string GenGanga(double value,string prefix,string prefixPage,int width,int height)
        {
            //крута! каментов больше чем кода!
            value = Math.Round(value);            
            string path = prefix+value.ToString()+".png";            
            System.Double V1 = value;
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)UltraGauge1.Gauges[0]).Scales[0];
            scale.Markers[0].Value = V1;

            Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement be =
                (Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            be.ColorStops.Clear();
            Color col;
            if (V1 > 80)
            {
                col = Color.Green;
            }
            else
            {
                if (V1 < 50)
                {
                    col = Color.Red;
                }
                else
                {
                    col = Color.Yellow;
                }
            }
            be.ColorStops.Add(col, 0);
            be.ColorStops.Add(col, 1); 


            System.Drawing.Size size = new Size(width,  height);            
            UltraGauge1.SaveTo(Server.MapPath("~/TemporaryImages" + path), GaugeImageType.Png, size);
            return "<img style=\"FLOAT: left;\" src =\"" + prefixPage + path + "\"/>";
        }



        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            p1 = UserParams.CustomParam("LD");
            p2 = UserParams.CustomParam("p2");
            p3 = UserParams.CustomParam("zakazchik");
            p4 = UserParams.CustomParam("region");
            p5 = UserParams.CustomParam("p5");
            p6 = UserParams.CustomParam("region");
            p7 = UserParams.CustomParam("programm");
            p8 = UserParams.CustomParam("kosgu");
            marks = UserParams.CustomParam("marks");

            System.Double Coef = 1;
            if (BN == "IE")
            {
                Coef = 1.01;
            }
            if (BN == "FIERFOX")
            {
                Coef = 1.01;
            }
            
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth*Coef-10);
            G.Height = Unit.Empty;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Visible = false;

        }

        

        protected string FilMount(ArrayList mount)
        {
            string res = ""; 
            int i = 0;
            for (i = 0; i < mount.Count - 1; i++)
            {
                
                res += string.Format(mount[i].ToString(), DelLastsChar(Year.SelectedNode.Parent.Text,' ') )+",";
            }
            res += string.Format(mount[i].ToString(), DelLastsChar(Year.SelectedNode.Parent.Text, ' '));

            return res;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                Kosgy = DetailKOSGY.Checked;
                Module = DetailModule.Checked;
                Year.Title = "Выберите месяц";
                Year.PanelHeaderTitle = "Выберите месяц";
                Year.ShowSelectedValue = 1 == 1;
                if (!Page.IsPostBack)
                {   
                    p4.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    marks.Value = "[Период].[Год Квартал Месяц].[Месяц].members";
                    Year.FillDictionaryValues(GenDistonary1("LDy"));
                    Year.SetСheckedState(ls, 1 == 1);
                    Year.ParentSelect = 1 == 2;
                    Year.Width = 350;                    
                    Month.Width = 150;
                    Month.Visible = 1 == 2;
                    marks.Value = "[Программы].[Заказчики].[Заказчик].Members";
                    Dictionary<string, int> d = GetParam("LDy");
                    Zakaz.FillDictionaryValues(d);
                    Zakaz.SetСheckedState("Все заказчики", 1 == 1);
                    Zakaz.Width = 600;

                }
                Label1.Text = string.Format("Отчет о выполнении окружных программ в муниципальном образовании {0} на {1} года ", UserComboBox.getLastBlock(p4.Value),Year.SelectedNode.Text.Split(' ')[0]+" "+Year.SelectedNode.Parent.Text);
                int num = CRHelper.MonthNum(DelLastsChar(Year.SelectedValue.Split(' ')[0],' '));
                    p1.Value = "[Период].[Год Квартал Месяц].[Год].[" + DelLastsChar(Year.SelectedNode.Parent.Text,' ') + "].[" + "Полугодие " + CRHelper.HalfYearNumByMonthNum(num).ToString() + "].[Квартал " + CRHelper.QuarterNumByMonthNum(num).ToString() + "].[" + DelLastsChar(Year.SelectedValue.Split(' ')[0],' ') + "]";
                if (Zakaz.SelectedValue != "Все заказчики")
                {
                    p2.Value = "[Программы].[Заказчики].[Заказчик].[" + Zakaz.SelectedValue + "],";
                }
                else
                {
                    p2.Value = "";
                } try
                {
                    HidenCount = 0;
                    G.DataBind();                    
                    FormatGrid();                                
                    G.Rows[0].Cells[0].Text = ("Всего по программам").ToUpper();
                    G.Rows[0].Cells[0].Style.Font.Bold = 1 == 1;
                    G.Columns[1].Header.Caption = "Процент исполнения";
                    G.Columns[G.Columns.Count - 1].Hidden = 1 == 1;
                    G.Columns[G.Columns.Count - 2].Hidden = 1 == 2;
                        //G.Rows[0].Hidden
                    G.Rows[0].Activate();
                    G.Rows[0].Selected = 1 == 1;
                    G.Rows[0].Activated = 1 == 1;
                    try
                    {
                        p5.Value = "";
                    }
                    catch { }                    
                    //G.Height = (int)((G.Rows.Count-HidenCount) * 40);
                    
                    Page.Title = Label1.Text;
                }
                catch { }
            }
            catch { }
            UltraGauge1.Visible = 1 == 2;
        }
        int HidenCount = 0;
        protected void FormatGrid()
        {
            G.Rows[0].Delete();
            try
            {
                double val = double.Parse(G.Rows[0].Cells[12].Text);

                G.Rows[0].Cells[1].Text = GenGanga(val, "/EO_MOP_GAdge", "../../../../../TemporaryImages", 100, 30) + "  <font style=\"FLOAT: Right;\">" + Math.Round(val).ToString() + "%&nbsp&nbsp</font>";

                G.Rows[0].Cells[12].Text = "";
            }
            catch { }
            string prev = G.Rows[0].Cells[0].Text;
            bool lev5Kosgy = Kosgy & !Module;
            for (int i = 1; i < G.Rows.Count; i++)
            {

                if (prev == G.Rows[i].Cells[0].Text)
                {   
                    //G.Rows[i].Hidden = !(((G.Rows[i].Cells[0].Text[0]=='5')&lev5Kosgy)
                    //                    || ((G.Rows[i].Cells[0].Text[0] != '5') & Kosgy));
                    if (!(((G.Rows[i].Cells[0].Text[0] == '5') & lev5Kosgy) || ((G.Rows[i].Cells[0].Text[0] != '5') & Kosgy)))
                    {
                        //G.Rows[i].Hidden = 1 == 1;
                        G.Rows[i].Delete();
                        i--;
                        HidenCount++;
                        continue;
                        
                    }



                    G.Rows[i].Cells[0].Text = G.Rows[i].Cells[1].Text;
                }
                else
                {
                    prev = G.Rows[i].Cells[0].Text;
                }

                if (G.Rows[i].Cells[0].Text[0] == '5')
                {
                    G.Rows[i].Cells[0].Style.Font.Bold = 1 == 1;
                }
                if (G.Rows[i].Cells[0].Text[0] == '6')
                {
                }
                if (G.Rows[i].Cells[0].Text[0] == '7')
                {
                    G.Rows[i].Cells[0].Style.Font.Underline = 1 == 1; ;
                }
                if (G.Rows[i].Cells[0].Text[0] == '8')
                {
                    G.Rows[i].Cells[0].Style.Font.Italic = 1 == 1; ;
                }


                G.Rows[i].Cells[1].Text = "";
                double val = double.Parse( G.Rows[i].Cells[12].Text);
                if (int.Parse(G.Rows[i].Cells[0].Text[0] + "") == 5)
                {
                    G.Rows[i].Cells[1].Text = GenGanga(val, "/EO_MOP_GAdge", "../../../../../TemporaryImages", 100, 30) + "  <font style=\"FLOAT: Right;\">" + Math.Round(val).ToString() + "%&nbsp&nbsp</font>";
                    
                }
                G.Rows[i].Cells[12].Text = "";
                G.Rows[i].Cells[0].Text = G.Rows[i].Cells[0].Text.Remove(0, 1);

            }
            G.Columns[12].Header.Caption = "";

        }


        protected void G_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("G"));
            DataTable ds = GetDSForChart("G0");

            dt.Columns.Add("Показатель", ds.Columns[0].DataType);
            dt.Columns.Add("КОСГУ", ds.Columns[0].DataType);
            int ColCount = cs.Axes[0].Positions.Count;
            int ii = 0;
            for (ii = 0; ii < ColCount - 1; ii++)
            {
                dt.Columns.Add(cs.Axes[0].Positions[ii].Members[0].Caption, ds.Columns[2].DataType);                
            }
            dt.Columns.Add(cs.Axes[0].Positions[ii].Members[0].Caption);
            dt.Columns.Add();
            dt.Rows.Add(ds.Rows[0].ItemArray);            
            {
                int rowCounter = 0;
                for (int i = 0; i < cs.Axes[1].Positions.Count; i++)
                {
                    object[] o = new object[ColCount + 3];
                    string ss = "";
                    string ss2 = "";
                    for (int j = 1; j < cs.Axes[1].Positions[i].Members.Count; j++)
                    {
                        ss += (cs.Axes[1].Positions[i].Members[j].UniqueName.Split('.').Length+3).ToString() + cs.Axes[1].Positions[i].Members[j].Caption;
                        ss2 += (cs.Axes[1].Positions[i].Members[j-1].UniqueName);
                    }
                    o[0] = cs.Axes[1].Positions[i].Members[0].UniqueName.Split('.').Length.ToString() + cs.Axes[1].Positions[i].Members[0].Caption;
                    o[1] = ss;
                    o[o.Length - 1] = ss2;                    
                    for (int k = 0; k < ColCount; k++)
                    {
                        try
                        {
                            o[k + 2] = cs.Cells[rowCounter * ColCount + k].Value;
                        }
                        catch { }
                    }   
                    dt.Rows.Add(o);
                    rowCounter++;
                }
            }            
            G.DataSource = dt;
        }


        bool Kosgy = 1 == 2;
        bool Module = 1 == 2;

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index > 1)
            {
                try
                {
                    string v = e.Row.Cells[1].Text[0] + "";
                    int iv = int.Parse(v) - 4;
                    e.Row.Cells[0].Style.Padding.Left = iv<3?1:iv * 10 + 1;
                    if (int.Parse(e.Row.Cells[0].Text[0] + "") < 5)
                    {
                        e.Row.Delete();
                    }




                    {
                        if (int.Parse(e.Row.Cells[0].Text[0] + "") > (Module ? 6 : 5))
                        {
                            e.Row.Delete();//Йа ПАКМАН.............. вака-вака-вака-вака-вака-вака-вака-вака-вака-вака-вака-вака-вака-вака-вака-вака
                        }
                    }
                }
                catch { }
                try
                {
	            if(e.Row.Cells[e.Row.Cells.Count - 2].Text.Length > 3)
                    {
                        e.Row.Cells[0].Style.BackgroundImage = "~/images/cornerGreen.gif";
                        e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                        e.Row.Cells[0].Title = e.Row.Cells[e.Row.Cells.Count - 2].Text;
                    }
                }
                catch { }
            }
        }

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                e.Layout.CellClickActionDefault = CellClickAction.NotSet;
                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();                
                Double Coef = 1.0;
                if (BN == "IE")
                {
                    Coef = 0.97;
                }
                if (BN == "APPLEMAC-SAFARI")
                {
                    Coef = 0.96;
                }
                if (BN == "FIREFOX")
                {
                    Coef = 0.96;
                }
                String.Format(BN);

                
                int c = e.Layout.Bands[0].HeaderLayout.Count;
                for (int i = 0; i < c; i++)
                {
                    e.Layout.Bands[0].HeaderLayout[i].RowLayoutColumnInfo.SpanY = 2;

                }
                {
                    ColumnHeader hb = new ColumnHeader();
                    hb.RowLayoutColumnInfo.OriginX = 3;
                    hb.RowLayoutColumnInfo.OriginY = 0;
                    hb.RowLayoutColumnInfo.SpanX = 4;
                    hb.Style.HorizontalAlign = HorizontalAlign.Center;
                    hb.Caption = "Втом числе и поквартально";
                    e.Layout.Bands[0].HeaderLayout.Add(hb);
                }
                e.Layout.Bands[0].HeaderLayout[1].Caption = "Процент исполнения";
                e.Layout.Bands[0].HeaderLayout[7].Caption = "Утвержденный лимит на отчетную дату";
                e.Layout.Bands[0].HeaderLayout[8].Caption = "Поступило средств с начала года";
                e.Layout.Bands[0].HeaderLayout[9].Caption = "Освоено средств с начала года ";
                e.Layout.Bands[0].HeaderLayout[10].Caption = "Освоено за отчетный месяц";
                e.Layout.Bands[0].HeaderLayout[11].Caption = "Сумма неосвоенных средств с начала года ";

                e.Layout.Bands[0].HeaderLayout[3].RowLayoutColumnInfo.OriginY = 1;
                e.Layout.Bands[0].HeaderLayout[3].Caption = "1";
                e.Layout.Bands[0].HeaderLayout[3].RowLayoutColumnInfo.SpanY = 1;
                e.Layout.Bands[0].HeaderLayout[4].RowLayoutColumnInfo.OriginY = 1;
                e.Layout.Bands[0].HeaderLayout[4].Caption = "2";
                e.Layout.Bands[0].HeaderLayout[4].RowLayoutColumnInfo.SpanY = 1;
                e.Layout.Bands[0].HeaderLayout[5].RowLayoutColumnInfo.OriginY = 1;
                e.Layout.Bands[0].HeaderLayout[5].Caption = "3";
                e.Layout.Bands[0].HeaderLayout[5].RowLayoutColumnInfo.SpanY = 1;
                e.Layout.Bands[0].HeaderLayout[6].RowLayoutColumnInfo.OriginY = 1;
                e.Layout.Bands[0].HeaderLayout[6].Caption = "4";
                e.Layout.Bands[0].HeaderLayout[6].RowLayoutColumnInfo.SpanY = 1;

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = 1 == 1;

                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth((120 + 140 - 6 * 4 ) * Coef);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(145 * Coef);

                e.Layout.Bands[0].Columns[0].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[1].Header.Style.Wrap = 1 == 1;

                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.0## ###");
                    e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(56 * Coef);
                }
                for (int i = 7; i < 13; i++)
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90 * Coef);
                for (int i = 3; i < 7; i++)
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth((50+10+5) * Coef);
                e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(95 * Coef);

                e.Layout.Bands[0].Columns[13].Width = CRHelper.GetColumnWidth(140 * Coef);
                
                e.Layout.Bands[0].Columns[13].Hidden = 1 == 1;

                e.Layout.Bands[0].Columns[13].CellStyle.Wrap = 1 == 1;

                e.Layout.Bands[0].Columns[1].Header.Caption = "";

                e.Layout.AllowSortingDefault = AllowSorting.No;
                e.Layout.Bands[0].Columns[12].Hidden = 1 == 1;
                e.Layout.Bands[0].Columns[13].Hidden = 1 == 1;
                e.Layout.Bands[0].Columns[12].Width = 0;
                e.Layout.Bands[0].Columns[13].Width = 0;
                e.Layout.Bands[0].Columns[13].Header.Caption = "";
                e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(95* Coef);
                 
                
                
            }
            catch { }
            
        }

        protected void LC_DataBinding(object sender, EventArgs e)
        {

        }

        protected void RC_DataBinding(object sender, EventArgs e)
        {
            try
            {
                string bld = p1.Value;
                p1.Value = FilMount(GenMounth());

                p1.Value = bld;
            }
            catch 
            { }
        }

        protected void G_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                int i = e.Row.Index;
                
                if (e.Row.Cells[0].Text ==  ("Всего по программам").ToUpper())
                {
                    p5.Value = "";
                }
                else
                {

                    for (; !G.Rows[i].Cells[0].Style.Font.Bold; i--)
                    { }
                    p5.Value = ", "+G.Rows[i].Cells[G.Rows[i].Cells.Count-1].Text;
                }
                
            }
            catch { }

        }

        protected void UltraGauge1_DataBinding(object sender, EventArgs e)
        {
            try
            {

            }
            catch
            { }
        }


        #region Экспорт Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Year.SelectedValue;
            e.CurrentWorksheet.Rows[1].Cells[1].Value = Month.SelectedValue;
            e.CurrentWorksheet.Rows[1].Cells[2].Value = Zakaz.SelectedValue;
           // e.CurrentWorksheet.Rows[1].Cells[3].Value = region.SelectedValue;

        }


        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            string formatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[0].Width = 650 * 37;
            e.CurrentWorksheet.Columns[1].Width = 300 * 37;

            for (int i = 2; i < G.Bands[0].Columns.Count; i++)
            {


                e.CurrentWorksheet.Columns[i].Width = 225 * 37;
                e.CurrentWorksheet.Columns[i].Hidden = false;
            }
            //ширина первого столбца

            for (int i = 0; i < G.Rows.Count+1; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = 500; try
                {
                    if (e.CurrentWorksheet.Rows[i].Cells[1].Value.ToString()[0] == '<')
                    {

                        e.CurrentWorksheet.Rows[i].Cells[1].Value = e.CurrentWorksheet.Rows[i].Cells[1].Value.ToString().Split('>')[2].Split('<')[0].Split('&')[0];
                        
                    }
                    
                }
                    catch { }

            }
            e.CurrentWorksheet.Columns[12].Width = 0;
            e.CurrentWorksheet.Rows[0].Cells[0].Value = "";
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            //  e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
            //  e.HeaderText = UltraWebGrid.DisplayLayout.Bands[1].Colmns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            //   Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 0;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(G, sheet1);
            //UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        protected void LC_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
        {

        }

        protected void LC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }


    }
}
