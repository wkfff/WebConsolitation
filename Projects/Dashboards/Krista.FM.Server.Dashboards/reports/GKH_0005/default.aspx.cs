﻿using System;

using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using System.Collections.Generic;


using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core.Primitives;
using Dundas.Maps.WebControl;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Table;
using System.IO;
using Infragistics.UltraChart.Resources.Appearance; 
using Infragistics.WebUI.UltraWebNavigator;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging; 
using System.Web;


namespace Krista.FM.Server.Dashboards.GKH_0005
{
    public partial class _default : CustomReportPage
    {
        IDataSetCombo SetCurDay;
        IDataSetCombo SetRegion;

        ICustomizerSize CustomizerSize;

        bool SelectCity;

        CustomParam IsHmao { get { return (UserParams.CustomParam("IsHmao")); } }

        CustomParam SelectCPP { get { return (UserParams.CustomParam("SelectCPP")); } }
        CustomParam SelectIIN { get { return (UserParams.CustomParam("SelectIIN")); } }

        CustomParam RegionBaseDimension { get { return (UserParams.CustomParam("RegionBaseDimension")); } }

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }

        CustomParam ChosenYear { get { return (UserParams.CustomParam("ChosenYear")); } }
        CustomParam ChosenCurQnum { get { return (UserParams.CustomParam("ChosenCurQnum")); } }

        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }
        CustomParam ChosenRegionGrid { get { return (UserParams.CustomParam("ChosenRegionGrid")); } }
        CustomParam ChosenRegionChart { get { return (UserParams.CustomParam("ChosenRegionChart")); } }

        CustomParam SelectField { get { return (UserParams.CustomParam("SelectField")); } }
        CustomParam SelectLastRow { get { return (UserParams.CustomParam("=)")); } }
        CustomParam lastActiveIndex { get { return (UserParams.CustomParam("lastActiveIndex")); } }

        #region Подонка размеров элементов, под браузер и разрешение
        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;

            public abstract int GetGridWidth();
            public abstract int GetChartWidth();

            public abstract int GetMapHeight();

            protected abstract void ContfigurationGridIE(UltraWebGrid Grid);
            protected abstract void ContfigurationGridFireFox(UltraWebGrid Grid);
            protected abstract void ContfigurationGridGoogle(UltraWebGrid Grid);

            public virtual void ContfigurationGrid(UltraWebGrid Grid)
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        this.ContfigurationGridIE(Grid);
                        return;
                    case BrouseName.FireFox:
                        this.ContfigurationGridFireFox(Grid);
                        return;
                    case BrouseName.SafariOrHrome:
                        this.ContfigurationGridGoogle(Grid);
                        return;
                    default:
                        throw new Exception("Охо!");
                }
            }

            public ICustomizerSize(BrouseName NameBrouse)
            {
                this.NameBrouse = NameBrouse;
            }

            public static BrouseName GetBrouse(string _BrouseName)
            {
                if (_BrouseName == "IE") { return BrouseName.IE; };
                if (_BrouseName == "FIREFOX") { return BrouseName.FireFox; };
                if (_BrouseName == "APPLEMAC-SAFARI") { return BrouseName.SafariOrHrome; };

                return BrouseName.IE;
            }

            public static ScreenResolution GetScreenResolution(int Width)
            {


                if (Width < 801)
                {
                    return ScreenResolution._800x600;
                }
                if (Width < 1281)
                {
                    return ScreenResolution._1280x1024;
                }
                return ScreenResolution.Default;
            }
        }

        class CustomizerSize_800x600 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 730;
                    case BrouseName.FireFox:
                        return 750;
                    case BrouseName.SafariOrHrome:
                        return 755;
                    default:
                        return 800;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 730;
                    case BrouseName.FireFox:
                        return 750;
                    case BrouseName.SafariOrHrome:
                        return 750;
                    default:
                        return 800;
                }
            }

            public CustomizerSize_800x600(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                int firstperccol = 40;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((100 -firstperccol) / (Grid.Columns.Count - 1) ) * onePercent;
                }

                Grid.Columns[0].Width = firstperccol * onePercent;
            } 

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                int firstperccol = 40;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((100 - firstperccol) / (Grid.Columns.Count - 1)) * onePercent;
                }

                Grid.Columns[0].Width = firstperccol * onePercent;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                int firstperccol = 40;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((100 - firstperccol) / (Grid.Columns.Count - 1)) * onePercent;
                }

                Grid.Columns[0].Width = firstperccol * onePercent;
            }

            #endregion


            public override int GetMapHeight()
            {
                return 705;
            }
        }

        class CustomizerSize_1280x1024 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)1200;
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    default:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                    default:
                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
                }
            }

            public CustomizerSize_1280x1024(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (98 / Grid.Columns.Count) * onePercent;
                }

                Grid.Columns[0].Width = 55 * onePercent;
                Grid.Columns[1].Width = 9 * onePercent;
                Grid.Columns[2].Width = 9 * onePercent;
                Grid.Columns[3].Width = 9 * onePercent;
                Grid.Columns[4].Width = 9 * onePercent; 
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (98 / Grid.Columns.Count) * onePercent;
                }
                Grid.Columns[0].Width = 62 * onePercent;
                Grid.Columns[1].Width = 9 * onePercent;
                Grid.Columns[2].Width = 9 * onePercent;
                Grid.Columns[3].Width = 9 * onePercent;
                Grid.Columns[4].Width = 9 * onePercent; 
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (99 / Grid.Columns.Count) * onePercent;
                }
                Grid.Columns[0].Width = 62 * onePercent;
                Grid.Columns[1].Width = 9 * onePercent;
                Grid.Columns[2].Width = 9 * onePercent;
                Grid.Columns[3].Width = 9 * onePercent;
                Grid.Columns[4].Width = 9 * onePercent; 

            }

            #endregion


            public override int GetMapHeight()
            {
                return 705;
            }
        }
        #endregion

        static class DataBaseHelper
        {
            static DataProvider ActiveDataPorvider = DataProvidersFactory.SpareMASDataProvider;

            public static DataTable ExecQueryByID(string QueryId)
            {
                return ExecQueryByID(QueryId, QueryId);
            }

            public static DataTable ExecQueryByID(string QueryId, string FirstColName)
            { 
                string QueryText = DataProvider.GetQueryText(QueryId);
                DataTable Table = ExecQuery(QueryText, FirstColName);
                return Table;
            }

            public static DataTable ExecQuery(string QueryText, string FirstColName)
            {
                DataTable Table = new DataTable();
                ActiveDataPorvider.GetDataTableForChart(QueryText, FirstColName, Table);
                return Table;
            }

        }

        #region Combo Binds
        abstract class IDataSetCombo
        {
            public enum TypeFillData
            {
                Linear, TwoLevel
            }

            public int LevelParent = 2;

            public string LastAdededKey { get; protected set; }

            public readonly Dictionary<string, string> DataUniqeName = new Dictionary<string, string>();

            public readonly Dictionary<string, int> DataForCombo = new Dictionary<string, int>();

            public readonly Dictionary<string, Dictionary<string, string>> OtherInfo = new Dictionary<string, Dictionary<string, string>>();

            protected void addOtherInfo(DataTable Table, DataRow Row, string Key)
            {
                Dictionary<string, string> Info = new Dictionary<string, string>();
                foreach (DataColumn col in Table.Columns)
                {
                    Info.Add(col.ColumnName, Row[col].ToString());
                }
                OtherInfo.Add(Key, Info);
            }

            public delegate string FormatedDispalyText(string ParentName, string ChildrenName);
            protected FormatedDispalyText FormatingText;
            public void SetFormatterText(FormatedDispalyText FormatterText)
            {
                FormatingText = FormatterText;
            }

            public delegate string FormatedDispalyTextParent(string ParentName);
            protected FormatedDispalyTextParent FormatingTextParent;
            public void SetFormatterTextParent(FormatedDispalyTextParent FormatterText)
            {
                FormatingTextParent = FormatterText;
            }

            protected string GetAlowableAndFormatedKey(string BaseKey, string Parent)
            {
                string Key = BaseKey;//FormatingText(Parent, BaseKey);

                while (DataForCombo.ContainsKey(Key))
                {
                    Key += " ";
                }

                return Key;
            }

            protected void AddItem(string Child, int level, string UniqueName)
            {

                string RealChild = Child;

                DataForCombo.Add(RealChild, level);

                DataUniqeName.Add(RealChild, UniqueName);

                this.LastAdededKey = RealChild;
            }

            public abstract void LoadData(DataTable Table);

        }

        class DataSetComboLinear : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();
                    string Child = "";// ParserDataUniqName.GetDouwnLevel(UniqueName);

                    DataForCombo.Add(Child, 0);

                    DataUniqeName.Add(Child, UniqueName);
                }
            }
        }

        class Day
        {
            DateTime Date;
            public int Year;
            public int Mounth;
            public int day;
            public string BaseCaptionFORF;
            public string BaseCaptionSU;

            public Day(string caption)
            {
                string[] bb = new string[3] { "].[", "[", "]" };

                if (caption[0] == '[')
                {
                    string[] splCa = caption.Split(bb, StringSplitOptions.None);
                    Year = int.Parse(splCa[4]);
                    Mounth = CRHelper.MonthNum(splCa[7]);
                    day = int.Parse(splCa[8]);
                    BaseCaptionSU = caption;
                }
                else
                {


                    string[] b = new string[1] { "- " };
                    string[] ddmmyy = caption.Split(b, StringSplitOptions.None)[1].Split('.', ',');

                    Year = int.Parse(ddmmyy[2]);
                    Mounth = int.Parse(ddmmyy[1]);
                    day = int.Parse(ddmmyy[0]);
                    BaseCaptionFORF = caption;
                }
                BaseCaptionSU = string.Format("[Период__День].[Период__День].[Год].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]",
                        Year,
                        CRHelper.HalfYearNumByMonthNum(Mounth),
                        CRHelper.QuarterNumByMonthNum(Mounth),
                        CRHelper.RusMonth(Mounth),
                        day);

                Date = new DateTime(Year, Mounth, day);


            }

            public string DisplayYear()
            {
                return Year.ToString() + " год";
            }

            public string DisplayMounth()
            {
                return CRHelper.RusMonth(Mounth) + " " + Year.ToString() + " года";
            }

            public string DisplayDay()
            {
                return day.ToString() + " " + CRHelper.RusMonth(Mounth).ToLower() + " " + Year.ToString() + " года";
                //return CRHelper.RusMonth(Mounth).ToLower() + " " + Year.ToString() + " года";
            }

            public string GridHeader()
            {
                return string.Format("{0:00}.{1:00}.{2:0000}", day, Mounth, Year);
                //return CRHelper.RusMonth(Mounth).ToLower() + " " + Year.ToString() + " года";
            }


            public string ChartLabel()
            {
                return string.Format("{0:00}.{1:00}.{2:0000}", day, Mounth, Year);
                //return CRHelper.RusMonth(Mounth).ToLower() + " " + Year.ToString() + " года";
            }

            public void RemoveDay(int count)
            {
                Date = Date.AddDays(-7);
                this.day = Date.Day;
                this.Mounth = Date.Month;
            }



        }

        class DataSetComboHierarhy : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();

                    string DisplayNAme = this.GetAlowableAndFormatedKey(row["DisplayName"].ToString(), "");
                    if (int.Parse(row["LevelNumber"].ToString()) == 0)
                    {

                        this.AddItem(DisplayNAme, int.Parse(row["LevelNumber"].ToString()), UniqueName);

                        this.addOtherInfo(Table, row, DisplayNAme);
                    }
                }
            }
        }

        class sortDay : System.Collections.Generic.IComparer<Day>
        {
            #region Члены IComparer<Day>

            public int Compare(Day x, Day y)
            {
                if (x.Year > y.Year)
                {
                    return 1;
                }
                else
                {
                    if (x.Year < y.Year)
                    {
                        return -1;
                    }
                    else
                    {
                        if (x.Mounth > y.Mounth)
                        {
                            return 1;
                        }
                        else
                        {
                            if (x.Mounth < y.Mounth)
                            {
                                return -1;
                            }
                            else
                            {
                                if (x.day > y.day)
                                {
                                    return 1;
                                }
                                else
                                {
                                    if (x.day == y.day)
                                        return 0;
                                    else
                                        return -1;
                                }
                            }
                        }

                    }
                }
            }

            #endregion
        }
        class DataSetComboPeriod : IDataSetCombo
        {
            string GetNumberQuart(string s)
            {
                return s.Split(' ')[1];
            }

            string GetCaptionChild(int NumberQuart, string year)
            {
                //красиво)
                string DisplayQuart = (new string[4] { "1 квартал", "1 полугодие", "9 месяцев", "год" })[NumberQuart];

                if (DisplayQuart != "год")
                {
                    DisplayQuart += " " + year + " года";
                }
                else
                {
                    DisplayQuart =  year + " год";
                }
                return DisplayQuart;
            }

            int GetMass(DataRow r)
            {
                return int.Parse(r["Year"].ToString() + GetNumberQuart(r["DisplayName"].ToString()));
            }

            int Compare(DataRow r1, DataRow r2)
            {
                int mas1 = GetMass(r1);
                int mas2 = GetMass(r2);

                if (mas1 > mas2)
                {
                    return 1;
                }
                if (mas1 < mas2)
                {
                    return -1;
                }
                return 0;

            }
            public override void LoadData(DataTable Table)
            {
                string LYear = "";
                Table.Columns.Add("OldCaptionChild");


                List<DataRow> RowsTable = new System.Collections.Generic.List<DataRow>();

                foreach (DataRow Row in Table.Rows)
                {

                    string Quart = Row["DisplayName"].ToString();

                    if (Row["Year"] == DBNull.Value)
                    {
                        Row["Year"] = Row["DisplayName"];
                        Row["DisplayName"] = "Квартал 4";
                    }

                    RowsTable.Add(Row);
                }

                RowsTable.Sort(new Comparison<DataRow>(Compare));

                foreach (DataRow Row in RowsTable)
                {
                    string Year = Row["Year"].ToString();
                    string Quart = Row["DisplayName"].ToString();
                    string uname = Row["UniqueName"].ToString();

                    if (Year != LYear)
                    {
                        this.AddItem(Year + " год", 0, string.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].datamember", Year));
                        LYear = Year;
                    }

                    Quart = GetAlowableAndFormatedKey(GetNumberQuart(Quart) + " квартал " + Year + " года", "");

                    Row["OldCaptionChild"] = Quart;
                    Quart = this.GetAlowableAndFormatedKey(this.GetCaptionChild(int.Parse(GetNumberQuart(Row["DisplayName"].ToString())) - 1, Year), "");

                    this.AddItem(Quart, 1, uname);
                    this.addOtherInfo(Table, Row, Quart);

                }
            }

        }
        #endregion

        private static void MoveContentCell(System.Web.UI.HtmlControls.HtmlTableCell outcell, System.Web.UI.HtmlControls.HtmlTableCell incell)
        {
            List<Control> lc = new System.Collections.Generic.List<Control>();
            foreach (Control c in outcell.Controls)
            {
                lc.Add(c);
            }

            foreach (Control c in lc)
            {
                outcell.Controls.Remove(c);
            }
            foreach (Control c in lc)
            {
                incell.Controls.Add(c);
            }
        }

        private void ConfCombo(bool IsSmall)
        {
            ComboCurDay.Width = IsSmall ? 250 : 350;
            ComboRegion.Width = IsSmall ? 370 : 420;

            if (false)
            {
                tableparam.Rows.Add(new System.Web.UI.HtmlControls.HtmlTableRow());
                tableparam.Rows[1].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[1].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[1].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[1].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                

                MoveContentCell(tableparam.Rows[0].Cells[1], tableparam.Rows[1].Cells[0]);

                tableparam.Rows.Add(new System.Web.UI.HtmlControls.HtmlTableRow());
                tableparam.Rows[2].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[2].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[2].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[2].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());



                MoveContentCell(tableparam.Rows[0].Cells[2], tableparam.Rows[2].Cells[0]);

                tableparam.Rows[0].Cells.RemoveAt(2);
                tableparam.Rows[0].Cells.RemoveAt(1);

                MoveContentCell(tableparam.Rows[0].Cells[1], tableparam.Rows[1].Cells[1]);
                for (int i = 2; i < tableparam.Rows[0].Cells.Count; i++)
                {
                    tableparam.Rows[0].Cells[i].RowSpan = 3;
                }
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            if (RegionSettingsHelper.Instance.Name == "ЯНАО")
            PopupInformer1.HelpPageUrl = "Help_YANAO.html";
            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(sad);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            #endregion

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;

            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            int add = 0;
            if (BN == "IE")
            {
                add = -50;
            }

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }
            else
                if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._800x600)
                {
                    CustomizerSize = new CustomizerSize_800x600(ICustomizerSize.GetBrouse(BN));
                }
                else
                {
                    CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
                }

            Grid.Width =600;
            Grid.Height =Unit.Empty; 

            GridB.Width = CustomizerSize.GetGridWidth(); ;
            GridB.Height = CustomizerSize is CustomizerSize_800x600 ? Unit.Empty : 400;
            
            ComboCurDay.Title = "Выберите период";
            ConfCombo(CustomizerSize is CustomizerSize_800x600);
        }





        private void FillComboPeriodCur()
        {

            ComboCurDay.ParentSelect = true;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboYear", "DisplayName");

            SetCurDay = new DataSetComboPeriod();
            SetCurDay.LoadData(Table);

            if (!Page.IsPostBack)
            {
                ComboCurDay.FillDictionaryValues(SetCurDay.DataForCombo);
                ComboCurDay.SetСheckedState(SetCurDay.LastAdededKey, 1 == 1);
            }
        }

        private void FillComboRegion()
        {
            ComboRegion.ParentSelect = true;

            ComboRegion.Title = "Территория";

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboRegion", "DisplayName");

            SetRegion = new DataSetComboHierarhy();
            SetRegion.LoadData(Table);
            
            
            if (!Page.IsPostBack)
            {   
                
                ChosenCurPeriod.Value = SetCurDay.DataUniqeName[ComboCurDay.SelectedValue];
                ComboRegion.FillDictionaryValues(SetRegion.DataForCombo);
                ComboRegion.SetСheckedState(DataBaseHelper.ExecQueryByID("GetRegionBuYear").Rows[0][0].ToString(), true);
            }
        }

        private void FillCombo()
        {
            RegionBaseDimension.Value = RegionSettingsHelper.Instance.RegionBaseDimension; 

            FillComboPeriodCur();
            
            FillComboRegion();
        }

        Node GetPrevNode(Node n)
        {
            if (n.PrevNode == null)
            {
                if (n.Parent.PrevNode == null)
                {
                    return n.Parent.Parent.Nodes[n.Parent.Parent.Nodes.Count - 1].Nodes[n.Parent.Parent.Nodes[n.Parent.Parent.Nodes.Count - 1].Nodes.Count - 1];
                }
                return n.Parent.PrevNode.Nodes[n.Parent.PrevNode.Nodes.Count - 1];
            }
            return n.PrevNode;

        }

        private void SelectDouwnLevel(Infragistics.WebUI.UltraWebNavigator.Node node, bool selectFirst, CustomMultiCombo combo)
        {
            if (node.Nodes != null)
                if (node.Nodes.Count > 0)
                {
                    SelectDouwnLevel(node.Nodes[0], true, combo);
                }
                else
                {
                    if (selectFirst)
                    {
                        combo.SetСheckedState(node.Text, true);
                    }
                }
        }

        private string GetYearMDX(int CurYear)
        {
            return string.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}]", CurYear.ToString());
        }

        string GetQuartNum(string SelectText)
        {
            string[] spl = new string[1] { "квартал" };

            return SelectText.Split(spl, StringSplitOptions.None)[0].Replace(" ", "");
        }

        string GetYear(string SelectText)
        {
            string[] spl = new string[1] { "квартал" };

            return SelectText.Split(spl, StringSplitOptions.None)[1].Replace(" ", "").Replace("год", "");
        }

        private void ChosenParam()
        {
            SelectCity = ComboRegion.SelectedValue.Contains("Город") || ComboRegion.SelectedNode.Level == 1;

            ChosenRegionGrid.Value = SetRegion.DataUniqeName[ComboRegion.SelectedValue] + (!SelectCity ? "" : "");

            try
            {
                ChosenCurQnum.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[q" + 
                    GetQuartNum(SetCurDay.OtherInfo[ComboCurDay.SelectedValue]["OldDisplayCaptiuon"])+"]";

                ChosenYear.Value = GetYear(ComboCurDay.SelectedValue);
            }
            catch 
            {
                ChosenCurQnum.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[q4]";
                ChosenYear.Value = ComboCurDay.SelectedValue.Replace(" год","").Replace(" ","");
            }
        }

        private void DataBindGrid(UltraWebGrid Grid)
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Field");

            if (BaseTable.Rows.Count > 0)
            {
                Grid.DataSource = BaseTable;
            }
            else
            {
                Grid.DataSource = null;
            }
            Grid.DataBind();
        }

        protected void SetDefaultStyleHeader(ColumnHeader header)
        {
            GridItemStyle HeaderStyle = header.Style;

            HeaderStyle.Wrap = true;

            HeaderStyle.VerticalAlign = VerticalAlign.Middle;

            HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        private ColumnHeader GenHeader(string Caption, int x, int y, int spanX, int SpanY)
        {
            ColumnHeader Header = new ColumnHeader();

            Header.Caption = Caption;
            Header.Style.Wrap = true;
            Header.RowLayoutColumnInfo.OriginX = x;
            Header.RowLayoutColumnInfo.OriginY = y;
            Header.RowLayoutColumnInfo.SpanX = spanX;
            Header.RowLayoutColumnInfo.SpanY = SpanY;
            SetDefaultStyleHeader(Header);

            return Header;
        }

        private void ConfigHeader(ColumnHeader ch, int x, int y, int spanX, int spanY, string Caption)
        {
            ch.RowLayoutColumnInfo.OriginX = x;
            ch.RowLayoutColumnInfo.OriginY = y;
            ch.RowLayoutColumnInfo.SpanX = spanX;
            ch.RowLayoutColumnInfo.SpanY = spanY;
            ch.Caption = Caption;
            SetDefaultStyleHeader(ch);
            if (ch.Column.Index != 0)
            {
                CRHelper.FormatNumberColumn(ch.Column, "N2");
            }
            else
            {
                ch.Column.CellStyle.Wrap = true;
            }
        }
        GridHeaderLayout HL;
        private void ConfHeader(UltraWebGrid Grid)
        {
            HL = new GridHeaderLayout(Grid);
            foreach (UltraGridColumn col in Grid.Columns)
            {
                if (col.BaseColumnName == "Field")
                {
                    HL.AddCell("Название организации");
                }
                else
                {
                    HL.AddCell(col.Header.Caption);
                }
                
                
                col.CellStyle.Wrap = true;
                col.Header.Style.Wrap = true;
            }
            HL.ApplyHeaderInfo();
            foreach (ColumnHeader ch in GridB.Bands[0].HeaderLayout)
            {
                
                ch.Style.HorizontalAlign = HorizontalAlign.Center;
                ch.Style.Wrap = true;
                
            }
        }

        void CustomizerOther(UltraWebGrid Grid)
        {
            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;

            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;
        }

        private void CustomizeGrid(UltraWebGrid Grid)
        {
            CustomizerOther(Grid);

            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }

            ConfHeader(Grid);

            //CustomizerSize.ContfigurationGrid(Grid);            

            Grid.Columns[0].Width = 300;
            Grid.Columns[1].Width = 100;
            Grid.Columns[2].Width = 100;

            Grid.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            Grid.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;


            Grid.ActiveRowChange +=new ActiveRowChangeEventHandler(Grid_ActiveRowChange);

        }

        private void GenerationGrid(UltraWebGrid Grid)
        {
            Grid.Bands.Clear();

            DataBindGrid(Grid);

            CustomizeGrid(Grid);
        }

        void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveRow(e.Row);
        }

        void ActiveRow(UltraGridRow Row)
        {
            SelectCPP.Value = Row.Cells[0].Text;
            //SelectIIN.Value = Row.Cells[1].Text;
            if (Grid.Rows.Count > 0)
            {
                GenerationGrid2();
            }
            SetHeaderReport();
        }

        Dictionary<string, string> DictEdizm = new System.Collections.Generic.Dictionary<string, string>();
        Dictionary<string, string> DictEdizm_re = new System.Collections.Generic.Dictionary<string, string>();
        private EndExportEventHandler ExcelExporter_EndExport_;
        private void FillDictEdIzm()
        {
            DataTable table = DataBaseHelper.ExecQueryByID("DictEdizm", "field");
            foreach (DataRow Row in table.Rows)
            {
                DictEdizm.Add(Row[0].ToString(), Row[1].ToString());
            }

        }


        private object GetDeviation(object CurValue, object PrevValue)
        {
            try
            {
                if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
                {
                    return (decimal)CurValue - (decimal)PrevValue;
                }
                return DBNull.Value;
            }
            catch { return DBNull.Value; }
        }

        private object GetSpeedDeviation(object CurValue, object PrevValue)
        {
            try
            {
                if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
                {
                    if ((decimal)PrevValue != 0)
                    {
                        return ((decimal)CurValue / (decimal)PrevValue) - 1;
                    }
                    else
                    {
                        return DBNull.Value;
                    }
                }
                return DBNull.Value;
            }
            catch { return DBNull.Value; }
        }
        Dictionary<string, int> LevelFieldGrid2 = new System.Collections.Generic.Dictionary<string, int>();
        private void DataBindgrid2()
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid2", "Field");

            DataTable TableGrid = new DataTable();

            TableGrid.Columns.Add("Field");
            TableGrid.Columns.Add("val1", typeof(decimal));
            TableGrid.Columns.Add("val2", typeof(decimal));
            TableGrid.Columns.Add("D", typeof(decimal));
            TableGrid.Columns.Add("SD", typeof(decimal));

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                //if (BaseRow[1] != DBNull.Value && BaseRow[2] != DBNull.Value) 
                {
                    DataRow ReportRow = TableGrid.NewRow();
                    TableGrid.Rows.Add(ReportRow);
                    ReportRow["Field"] = BaseRow["Field"];
                    ReportRow["val1"] = BaseRow[1];
                    ReportRow["val2"] = BaseRow[2];
                    ReportRow["D"] = GetDeviation(ReportRow["val2"], ReportRow["val1"]);
                    ReportRow["SD"] = GetSpeedDeviation(ReportRow["val2"], ReportRow["val1"]);
                    try
                    {
                        LevelFieldGrid2.Add(ReportRow["Field"].ToString(),
                            int.Parse(BaseRow[3].ToString()));
                    }
                    catch { }
                }
            }

            GridB.DataSource = TableGrid;
            GridB.DataBind();
        }
        GridHeaderLayout HL2;
        private void FormatHeader2()
        {
            HL2 = new GridHeaderLayout(GridB);
            HL2.AddCell("Наименование показателя");
            HL2.AddCell("На начало отчётного периода, тысяча рублей");
            HL2.AddCell("На конец отчётного периода, тысяча рублей");
            HL2.AddCell("Прирост за отчетный период, тысяча рублей");
            HL2.AddCell("Темп прироста, %");
            HL2.ApplyHeaderInfo();

            CRHelper.FormatNumberColumn(GridB.Columns.FromKey("val1"), "N2");
            CRHelper.FormatNumberColumn(GridB.Columns.FromKey("val2"), "N2");
            CRHelper.FormatNumberColumn(GridB.Columns.FromKey("D"), "N2");
            CRHelper.FormatNumberColumn(GridB.Columns.FromKey("SD"), "P2");

            foreach (UltraGridColumn col in GridB.Columns)
            {
                col.CellStyle.Wrap = true;
                col.Header.Style.Wrap = true;
                col.Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }
            foreach (HeaderBase hb in Grid.Bands[0].HeaderLayout)
            {
                hb.Style.HorizontalAlign = HorizontalAlign.Center;
                hb.Style.Wrap = true;
            }            
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center";
            Cell.Style.BackgroundImage = ImagePath;
        }
        private void SetcatorSpeedDeviationcell(UltraGridCell ValueCell, bool reverce)
        {

            UltraGridCell IndicatorCell = GetNextCell(GetNextCell(ValueCell));

            if (IndicatorCell.Value != null)
            {
                int IndexCol = ValueCell.Column.Index;
                decimal Value = decimal.Parse(IndicatorCell.Text.ToString().Replace("%", ""));
                if (Value != 0)
                {
                    string UpOrdouwn = Value > 100 ? "Up" : "Down";

                    string TitleCaption = Value > 100 ? "прироста" : "снижения";

                    //string MounthName = Grid.Columns[IndexCol - 1].Header.Caption.Split(' ')[0];
                    //string year = Grid.Columns[IndexCol - 1].Header.Caption.Split(' ')[1].Replace("<br>", "");
                    int CurYear = int.Parse(ComboCurDay.SelectedValue);
                    IndicatorCell.Title = string.Format("Темп {0} к {1} году", TitleCaption,
                        (CurYear - 1).ToString());

                    TitleCaption = Value > 100 ? "Прирост" : "Снижение";

                    GetNextCell(ValueCell).Title = string.Format("{0} к {1} году", TitleCaption, (CurYear - 1).ToString());

                    string Color = "";
                    if ((Value > 100))//& (reverce) 
                    {
                        Color = "Red";
                    }

                    if ((Value < 100))//& (!reverce)
                    {
                        Color = "Green";
                    }
                    if (!string.IsNullOrEmpty(Color))
                    {
                        SetImageFromCell(IndicatorCell, "arrow" + Color + UpOrdouwn + "BB.png");
                    }
                }
            }
        }

        void FormatTopCell(UltraGridCell cell)
        {
            cell.Style.BorderDetails.ColorBottom = Color.Transparent;
        }

        void FormatCenterCell(UltraGridCell cell)
        {
            cell.Style.BorderDetails.ColorBottom = Color.Transparent;
            cell.Style.BorderDetails.ColorTop = Color.Transparent;
        }

        void FormatBottomCell(UltraGridCell cell)
        {
            cell.Style.BorderDetails.ColorTop = Color.Transparent;
        }

        string RangMax = "";

        private void SetIndicatorDeviationcell(UltraGridCell ValueCell, bool p)
        {
            try
            {
                int IndexCol = ValueCell.Column.Index;

                UltraGridCell IndicatorCell = GetNextCell(ValueCell);
                if (IndicatorCell.Value != null)
                {

                    decimal Value = decimal.Parse(IndicatorCell.Value.ToString());
                    if (Value != 0)
                    {
                        string MounthName = Grid.Columns[IndexCol - 1].Header.Caption.Split(' ')[0];
                        string year = Grid.Columns[IndexCol - 1].Header.Caption.Split(' ')[1].Replace("<br>", "");
                    }
                }
            }
            catch { }
        }
        void SetFormatCell(UltraGridCell Cell, string format)
        {
            if (Cell.Value != null)
            {
                try
                {
                    Cell.Text = string.Format("{0:" + format + "}", decimal.Parse(Cell.Text));
                }
                catch { }
            }
        }
        private UltraGridCell GetNextCell(UltraGridCell Cell)
        {

            return Cell.Row.Cells[Cell.Column.Index + 1];
        }
        private UltraGridCell GetPrevCell(UltraGridCell Cell)
        {
            return Cell.Row.Cells[Cell.Column.Index + 2];
        }

        private void SetIndicatorSpeedDeviationcell(UltraGridCell ValueCell, bool reverce)
        {

            UltraGridCell IndicatorCell = GetNextCell(GetNextCell(ValueCell));

            if (IndicatorCell.Value != null)
            {
                int IndexCol = ValueCell.Column.Index;
                decimal Value = decimal.Parse(IndicatorCell.Text.ToString().Replace("%", ""));
                if (Value != 0)
                {
                    string UpOrdouwn = Value > 0 ? "Up" : "Down";

                    string TitleCaption = Value > 0 ? "прироста" : "снижения";

                    try
                    {
                        int CurYear = int.Parse(ComboCurDay.SelectedValue);
                        IndicatorCell.Title = string.Format("Темп {0} к {1} году", TitleCaption,
                            (CurYear - 1).ToString());
                    
                    TitleCaption = Value > 0 ? "Прирост" : "Снижение";

                    GetNextCell(ValueCell).Title = string.Format("{0} к {1} году", TitleCaption, (CurYear - 1).ToString());
                    }
                    catch { }
                    string Color = "";
                    string reColor = "";
                    if ((Value > 0))
                    {
                        Color = "Red";
                        reColor = "Green";
                    }

                    if ((Value < 0))
                    {
                        Color = "Green";
                        reColor = "Red";
                    }
                    if (reverce)
                    {
                        Color = reColor;
                    }

                    if (!string.IsNullOrEmpty(Color))
                    {
                        SetImageFromCell(IndicatorCell, "arrow" + Color + UpOrdouwn + "BB.png");
                    }
                }
            }
        }
         
        int AddLevel(string Field)
        {
            string MegoString = @"резервы, образованные в соответствии с законодательством
резервы, образованные в соответствии с учредительными документами
поставщики и подрядчики
задолженность перед персоналом организации
задолженность перед государственными внебюджетными фондами 
задолженность по налогам и сборам 
прочие кредиторы
сырье, материалы и другие аналогичные ценности 
животные на выращивании и откорме 
затраты в незавершенном производстве 
готовая продукция и товары для перепродажи 
товары отгруженные
расходы будущих периодов
прочие запасы и затраты
покупатели и заказчики
покупатели и заказчики
Дебиторская задолженность (платежи по которой ожидаются в течение 12 месяцев после отчетной даты, покупатели и заказчики
Дебиторская задолженность (платежи по которой ожидаются более чем через 12 месяцев после отчетной даты), покупатели и заказчики";
            if (MegoString.Contains(Field))
            {
                return 1;
            }
            return 0;
        }


        private void formatrowgrid2()
        {
            bool re = !false;
            foreach (UltraGridRow row in GridB.Rows)
            {

                if (row.Cells[0].Text.Contains("ПАССИВ"))
                {
                    re = !true;
                }
                    SetIndicatorSpeedDeviationcell(row.Cells.FromKey("val2"), re);

                    row.Cells.FromKey("Field").Style.Padding.Left = (
                         AddLevel(row.Cells.FromKey("Field").Text)+                        
                        LevelFieldGrid2[row.Cells.FromKey("Field").Text]) * 10;
                 
            }
        }

        private void CutomizerGrid2()
        {
            CustomizerOther(GridB);
            FormatHeader2();
            formatrowgrid2();

            CustomizerSize.ContfigurationGrid(GridB);
        }




        private void GenerationGrid2()
        {
            DataBindgrid2();
            CutomizerGrid2();
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            IsHmao.Value = RegionSettingsHelper.Instance.Name.Contains("Ямало") ? "--" : "";

            emptyReport(false);

            FillCombo();

            ChosenParam();

            try
            {
                GenerationGrid(Grid);

                ActiveRow(Grid.Rows[0]);
            }
            catch 
            {
                emptyReport(true);
            }
        }

        private void emptyReport(bool p)
        {
            HederTopGrid.Visible = !p;
            HederBottomGrid.Visible = !p;

            PageSubTitle.Text = string.Format("Анализ основных показателей, характеризующих финансовое состояние деятельности организаций в сфере ЖКХ на основе Формы №1 «Бухгалтерский баланс», {1}, за {0}",
            ComboCurDay.SelectedValue.ToLower(), ComboRegion.SelectedValue);

            Hederglobal.Text = "Ежегодный мониторинг финансового состояния организаций ЖКХ";
            Page.Title = Hederglobal.Text;

            Grid.DisplayLayout.NoDataMessage = "Нет данных";
            GridB.DisplayLayout.NoDataMessage = "Нет данных";

            GridB.DataSource = null;
            GridB.DataBind();
        }

        private void SetHeaderReport() 
        {
            PageSubTitle.Text = string.Format("Анализ основных показателей, характеризующих финансовое состояние деятельности организаций в сфере ЖКХ на основе Формы №1 «Бухгалтерский баланс», {1}, за {0}",
                ComboCurDay.SelectedValue.ToLower(), ComboRegion.SelectedValue);

            Hederglobal.Text = "Мониторинг финансового состояния организаций ЖКХ на основе Формы №1 «Бухгалтерский баланс»";
            Page.Title = Hederglobal.Text;

            HederTopGrid.Text = "Перечень организаций";
            HederBottomGrid.Text = "Основные показатели финансового состояния организации "+SelectCPP.Value;
        }

        void ChartSZ_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Text t = new Text(new Point(12, 100), "Количество МО");
            t.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            t.labelStyle.Font = new Font("Verdana", 11);
            e.SceneGraph.Add(t);
        }



        #region экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Hederglobal.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Таблица 2");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            
            ReportExcelExporter1.Export(HL,HederTopGrid.Text, sheet1, 4);

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            ReportExcelExporter1.Export(HL2,HederBottomGrid.Text, sheet2, 1);

            
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }


        private void MergRegion(int indexFirstMerg, int i, Worksheet worksheet, int p)
        {
            if (indexFirstMerg != i)
            {

                worksheet.MergedCellsRegions.Add(indexFirstMerg, p, i, p);
            }
        }

        private void MergRegion_(int indexFirstMerg, int i, Worksheet worksheet, int p)
        {
            if (indexFirstMerg != i)
            {

                worksheet.MergedCellsRegions.Add(p, indexFirstMerg, p, i);
            }
        }

        private void ExcellMergerRow(Worksheet worksheet, int p)
        {
            int indexFirstMerg = 3;

            foreach (UltraGridRow r in Grid.Rows)
            {
                if (r.Cells[p].RowSpan > 1)
                {
                    try
                    {
                        MergRegion(r.Index + 6, r.Index + 6 + r.Cells[p].RowSpan - 1, worksheet, p);
                    }
                    catch { }
                }
                if (r.Cells[p].ColSpan > 1)
                {
                    try
                    {
                        MergRegion_(p, p + r.Cells[p].ColSpan - 1, worksheet, r.Index + 6);
                    }
                    catch { }
                }
            }
            return;
            for (int i = 3; i < Grid.Rows.Count + 3; i++)
            {
                try
                {
                    if (worksheet.Rows[i].Cells[p].Value.ToString() == worksheet.Rows[indexFirstMerg].Cells[p].Value.ToString())
                    {

                    }
                    else
                    {
                        MergRegion(indexFirstMerg, i - 2, worksheet, p);
                        indexFirstMerg = i;
                    }
                }
                catch { indexFirstMerg = i; }
            }

        }




        void sad(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            ExcellMergerRow(e.CurrentWorksheet, 0);
            ExcellMergerRow(e.CurrentWorksheet, 1);
        }



        #endregion


        void PDFMergRow(UltraWebGrid Grid, UltraGridRow Firstrow, UltraGridRow LastRow, int col)
        {

            string text = Firstrow.Cells[col].Text;
            for (int i = Firstrow.Index; i < LastRow.Index + 1; i++)
            {
                Grid.Rows[i].Cells[col].Text = "";
            }

            for (int i = Firstrow.Index + 1; i < LastRow.Index; i++)
            {
                Grid.Rows[i].Cells[col].Style.BorderDetails.ColorTop = Color.White;
                Grid.Rows[i].Cells[col].Style.BorderDetails.ColorBottom = Color.White;
            }
            Firstrow.Cells[col].Style.BorderDetails.ColorBottom = Color.White;

            if (!(string.IsNullOrEmpty(text)))
                if (text.Contains("|"))
                {
                    text = text.Split('|')[1];
                }
            Grid.Rows[(Firstrow.Index + LastRow.Index) / 2].Cells[col].Text = text;
        }

        void MergeCellsGridFormPDF(UltraWebGrid Grid, int col)
        {
            UltraGridRow MerdgRow = Grid.Rows[0];
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[col].RowSpan > 1)
                {
                    PDFMergRow(Grid, Row, Grid.Rows[Row.Index + Row.Cells[col].RowSpan - 1], col);
                }
            }
        }

        private void MergeCellsGridFormPDFCol(UltraWebGrid ultraWebGrid, UltraGridRow row)
        {
            if (row.Cells[0].ColSpan > 1)
            {
                for (int i = 1; i < Grid.Columns.Count - 1; i++)
                {
                    row.Cells[i].Style.BackColor = row.Cells[0].Style.BackColor;
                    row.Cells[i].Style.BorderDetails.ColorLeft = row.Cells[0].Style.BackColor;
                    row.Cells[i].Style.BorderDetails.ColorRight = row.Cells[0].Style.BackColor;
                }
                row.Cells[Grid.Columns.Count - 1].Style.BorderDetails.ColorLeft = row.Cells[0].Style.BackColor;
                row.Cells[Grid.Columns.Count - 1].Style.BackColor = row.Cells[0].Style.BackColor;
                row.Cells[0].Style.BorderDetails.ColorRight = row.Cells[0].Style.BackColor;
            }
        }


        #region Экспорт в PDF



        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Page.Title;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            



            ReportPDFExporter1.HeaderCellHeight = 20;
            //MergeCellsGridFormPDF(HL.Grid, 0);
            //MergeCellsGridFormPDF(HL.Grid, 1);

            

            ReportPDFExporter1.Export(HL,"\n"+HederTopGrid.Text, section1);
            ReportPDFExporter1.Export(HL2, "\n"+HederBottomGrid.Text, section2);
        }





        #endregion

        #region Линии на диограмке

        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = ColorLine;
            Line.PE.StrokeWidth = 2;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;


            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            //textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 8);
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);

            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;

            if (TextUP)
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY - 15, 500, 15);
            }
            else
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY + 1, 500, 15);
            }


            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.SetTextString(Label);

            e.SceneGraph.Add(Line);
            e.SceneGraph.Add(textLabel);


        }


        void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            bool center = false;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Series != null && (box.Series.Label == "Cредняя цена" || box.Series.Label == "Медиана"))
                        {
                            box.PE.Fill = Color.Orange;
                            box.PE.FillStopColor = Color.OrangeRed;
                            center = true;
                        }
                        else
                        {
                            if (!center)
                            {
                                box.PE.Fill = Color.Green;
                                box.PE.FillStopColor = Color.Green;
                            }
                            else
                            {
                                box.PE.Fill = Color.Red;
                                box.PE.FillStopColor = Color.Red;
                            }
                        }
                    }
                }
            }


            return;

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double RFStartLineX = 0;
            double RFStartLineY = 0;

            double RFEndLineX = 0;
            double RFEndLineY = 0;
            string RFheader = "";

            //if (ReportTable.Rows[0]["CUR"] != DBNull.Value)
            //{
            //    RFheader = string.Format("{0}: {1:N2} {2}", ReportTable.Rows[0]["Field"], ReportTable.Rows[0]["CUR"], "рубль");

            //    RFStartLineX = xAxis.Map(xAxis.Minimum);
            //    RFStartLineY = yAxis.Map((decimal)ReportTable.Rows[0]["CUR"]);

            //    RFEndLineX = xAxis.Map(xAxis.Maximum);
            //    RFEndLineY = yAxis.Map((decimal)ReportTable.Rows[0]["CUR"]);


            //}



            //if (!string.IsNullOrEmpty(RFheader))
            //{

            GenHorizontalLineAndLabel((int)RFStartLineX, (int)RFStartLineY, (int)RFEndLineX, (int)RFEndLineY,
                    Color.Blue, RFheader, e, true);
            //}

        }


        //}
        #endregion




        public EndExportEventHandler ExcelExporter_EndExport { get; set; }
    }
}