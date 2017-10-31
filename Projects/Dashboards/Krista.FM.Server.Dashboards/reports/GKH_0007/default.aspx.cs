using System;
 
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


namespace Krista.FM.Server.Dashboards.GKH_0007
{
    public partial class _default : CustomReportPage
    {
        IDataSetCombo SetCurDay;
        IDataSetCombo SetRegion;

        ICustomizerSize CustomizerSize;

        bool SelectCity;

        CustomParam RegionBaseDimension { get { return (UserParams.CustomParam("RegionBaseDimension")); } }

        CustomParam ChosenYear { get { return (UserParams.CustomParam("ChosenYear")); } }
        CustomParam ChosenCurQnum { get { return (UserParams.CustomParam("ChosenCurQnum")); } }

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
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
                int onePercent = (int)1280 / 100;
                int PercFirstCol = 17;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.BaseColumnName != "Field")
                    {
                        col.Width = (int)(((98.0 - PercFirstCol) / (Grid.Columns.Count - 1)) * onePercent);
                    }
                    else
                    {
                        col.Width = PercFirstCol * onePercent;
                    }
                }
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                int PercFirstCol = 17;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.BaseColumnName != "Field")
                    {
                        col.Width = (int)(((98.0 - PercFirstCol) / (Grid.Columns.Count - 1)) * onePercent);
                    }
                    else
                    {
                        col.Width = PercFirstCol * onePercent;
                    }
                }
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                int PercFirstCol = 17;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.BaseColumnName != "Field")
                    {
                        col.Width = (int)(((98.0 - PercFirstCol) / (Grid.Columns.Count - 1)) * onePercent);
                    }
                    else
                    {
                        col.Width = PercFirstCol * onePercent;
                    }
                }
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
                String.Format(CustomReportConst.minScreenWidth.ToString());
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
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
                int PercFirstCol = 13;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.BaseColumnName != "Field")
                    {
                        col.Width = (int)(((98.0 - PercFirstCol) / (Grid.Columns.Count - 1)) * onePercent);
                    }
                    else
                    {
                        col.Width = PercFirstCol * onePercent;
                    }
                }
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                int PercFirstCol = 13;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.BaseColumnName != "Field")
                    {
                        col.Width = (int)(((98.0 - PercFirstCol) / (Grid.Columns.Count - 1)) * onePercent);
                    }
                    else
                    { 
                        col.Width = PercFirstCol * onePercent;
                    }
                }
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                int PercFirstCol = 13;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.BaseColumnName != "Field")
                    {
                        col.Width = (int)(((98.0 - PercFirstCol) / (Grid.Columns.Count - 1)) * onePercent);
                    }
                    else
                    {
                        col.Width = PercFirstCol * onePercent;
                    }
                }
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

                    this.AddItem(DisplayNAme, int.Parse(row["LevelNumber"].ToString()), UniqueName);

                    this.addOtherInfo(Table, row, DisplayNAme);
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
                    DisplayQuart = year + " год";
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

        private static void MoveContentCell(System.Web.UI.HtmlControls.HtmlTableCell outcell, 
                                             System.Web.UI.HtmlControls.HtmlTableCell incell)
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
            if (IsSmall)
            {
                ComboCurDay.Width = 500;
                ComboRegion.Width = 500;

                tableparam.Rows.Add(new System.Web.UI.HtmlControls.HtmlTableRow());
                tableparam.Rows[1].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[1].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[1].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());
                tableparam.Rows[1].Cells.Add(new System.Web.UI.HtmlControls.HtmlTableCell());


                MoveContentCell(tableparam.Rows[0].Cells[1], tableparam.Rows[1].Cells[0]);

                tableparam.Rows[0].Cells.RemoveAt(1);

                MoveContentCell(tableparam.Rows[0].Cells[1], tableparam.Rows[1].Cells[1]);
                for (int i = 2; i < tableparam.Rows[0].Cells.Count; i++)
                {
                    tableparam.Rows[0].Cells[i].RowSpan = 2;
                }
            }
        }


       
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

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

            Grid.Width =// CustomizerSize is CustomizerSize_800x600?
                CustomizerSize.GetGridWidth();            

            
            ComboCurDay.Width =  CustomizerSize is CustomizerSize_800x600?280:380;

            ComboRegion.Width = CustomizerSize is CustomizerSize_800x600 ? 280 : 420;
            Grid.Height = CustomizerSize is CustomizerSize_800x600? Unit.Empty: 440;

            ConfCombo(CustomizerSize is CustomizerSize_800x600);

            ComboCurDay.Title = "Выберите период"; 
        }

        private void FillComboPeriodCur()
        {
            ComboCurDay.ParentSelect =  true;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboYear", "DisplayName");

            ComboCurDay.ParentSelect = false;

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
                //ComboRegionFirst
                ChosenCurPeriod.Value = getMdxQuart(ComboCurDay.SelectedNode);
                ComboRegion.FillDictionaryValues(SetRegion.DataForCombo);
                ComboRegion.SetСheckedState( DataBaseHelper.ExecQueryByID("ComboRegionFirst").Rows[0][0].ToString() , true);
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
                if (n.Parent == null)
                {
                    return null;
                }
                if (n.Parent.PrevNode == null)
                {
                    return null;
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

        private string getMdxQuart(Node PrevNode)
        {
            if (PrevNode == null)
            {
                return "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[null]";
            }
            else
            {
                return SetCurDay.DataUniqeName[PrevNode.Text];
            }
        }

        string GetQuartNum(string SelectText)
        {
            string[] spl = new string[1] { "квартал" };

            return SelectText.Split(spl, StringSplitOptions.None)[0].Replace(" ", "");
        }

        string GetYear(string SelectText)
        {
            return SelectText.Replace(" год", "");
        }

        private void ChosenParam()
        {
            SelectCity = ComboRegion.SelectedValue.Contains("Город") || ComboRegion.SelectedNode.Level == 1;

            ChosenRegionGrid.Value = SetRegion.DataUniqeName[ComboRegion.SelectedValue] + (!SelectCity ? "" : "");

            ChosenCurPeriod.Value = SetCurDay.DataUniqeName[ComboCurDay.SelectedValue];

            ChosenCurQnum.Value = "q" + GetQuartNum(ComboCurDay.SelectedValue);

            ChosenYear.Value = GetYear(ComboCurDay.SelectedNode.Parent.Text);


            ChosenCurPeriod.Value = getMdxQuart(ComboCurDay.SelectedNode);
            ChosenPrevPeriod.Value = 
                getMdxQuart(ComboCurDay.SelectedNode).Replace(
                    ChosenYear.Value, 
                    (int.Parse(ChosenYear.Value)-1).ToString());

        }

        class CoolTable : DataTable
        {
            public DataRow AddRow()
            {
                DataRow NewRow = this.NewRow();
                this.Rows.Add(NewRow);
                return NewRow;
            }

            public DataRow GetRowFormValueFirstCell(string caption)
            {
                foreach (DataRow Row in this.Rows)
                {
                    if (Row[0].ToString() == caption)
                    {
                        return Row;
                    }
                }
                return null;
            }

            public DataRow AddOrGetRow(string caption)
            {
                DataRow Row = this.GetRowFormValueFirstCell(caption);
                if (Row != null)
                {
                    return Row;
                }

                DataRow NewRow = this.AddRow();
                NewRow[0] = caption;
                return NewRow;
            }


        }

        private object GetDeviation(object CurValue, object PrevValue)
        {
            try
            {
                if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
                {
                    return (decimal)CurValue - ((decimal)PrevValue);
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
                        return ((decimal)CurValue / ((decimal)PrevValue));
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
        private static void fillStrRow(DataRow GridRow, string ParsRow)
        {
            //GridRow["Region"] = ParsRow[0];
            GridRow["Field"] = ParsRow;
            //GridRow["Org"] = GridRow["Yslyga"].ToString() +"|"+ ParsRow[3];
        }


        private object GetSum(object p, object p_2)
        {
            decimal val1 = 0;
            decimal val2 = 0;
            try
            {
                val1 = (decimal)p;
            }
            catch { }
            try
            {
                val2 = (decimal)p_2;
            }
            catch { }
            return val1 + val2;

        }
        private void SumCol(DataTable BaseTable, string val1, string val2, string val3)
        {
            foreach (DataRow Row in BaseTable.Rows)
            {
                Row[val3] = GetSum(Row[val1], Row[val2]);
            }
        }

        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(DataRow x, DataRow y)
            {
                return -Compare_(x, y);
            }



            public int Compare_(DataRow x, DataRow y)
            {
                decimal X = (decimal)x["Value"];
                decimal Y = (decimal)y["Value"];
                if (X > Y)
                {
                    return -1;
                }

                if (X < Y)
                {
                    return 1;
                }

                return 0;
            }

            #endregion
        }

        DataTable SortTable(DataTable Table)
        {
            DataTable TableSort = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }

            List<DataRow> LR = new System.Collections.Generic.List<DataRow>();

            foreach (DataRow row in Table.Rows)
            {
                LR.Add(row);
            }

            LR.Sort(new SortDataRow());

            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);
            }
            return TableSort;
        }


        bool Isemptytable(DataTable Table)
        {
            foreach (DataColumn col in Table.Columns)
            {
                if (col.ColumnName != "Field")
                {
                    foreach (DataRow row in Table.Rows)
                    {
                        if ((row[col] != DBNull.Value) && ((decimal)row[col]!= 0))
                        {    
                            return false; 
                        }
                    }
                }
            }
            return true;

        }

        private void DataBindGrid()
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Field");

            CoolTable GridTable = new CoolTable();

            GridTable.Columns.Add("Field");

            foreach (DataColumn Col in BaseTable.Columns)
            {   
                if (Col.Caption.Contains("CurYear"))
                {
                    string[] ParsCol = Col.ColumnName.Split(';');
                    if (ParsCol.Length > 0)
                    {
                        string ColRealname = ParsCol[1];
                        GridTable.Columns.Add(ColRealname, typeof(decimal));
                    }
                }
            }


            string CurYear = UserComboBox.getLastBlock(ChosenCurPeriod.Value);
            string PrevYear = UserComboBox.getLastBlock(ChosenPrevPeriod.Value);

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                string[] ParsRow = BaseRow["Field"].ToString().Split(';');

                DataRow GridRow = GridTable.AddRow();
                DataRow GridRowD = GridTable.AddRow();
                DataRow GridRowSD = GridTable.AddRow();

                fillStrRow(GridRow, BaseRow[0].ToString());
                fillStrRow(GridRowD, BaseRow[0].ToString());
                fillStrRow(GridRowSD, BaseRow[0].ToString());

                foreach (DataColumn col in GridTable.Columns)
                {
                    string ReportColName = col.ColumnName;
                    if (ReportColName != "Field")
                    {
                        string CurBaseColName = "CurYear" + ";" + ReportColName;
                        string PrevBaseColName = "PrevYear" + ";" + ReportColName;

                        GridRow[ReportColName] = BaseRow[CurBaseColName];
                        if (!col.ColumnName.Contains("Процент")) 
                        {
                            GridRowD[ReportColName] =
                                GetDeviation(
                                BaseRow[CurBaseColName],
                                BaseRow[PrevBaseColName]);
                            GridRowSD[ReportColName] =
                                GetSpeedDeviation(
                                BaseRow[CurBaseColName],
                                BaseRow[PrevBaseColName]);
                        }
                    }
                }
            }

            if (!Isemptytable(GridTable))
            {
                Grid.DataSource = GridTable;
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
        public void ConfHeader()
        {
            HL = new GridHeaderLayout(Grid);
            foreach (UltraGridColumn col in Grid.Columns)
            {

                col.CellStyle.Wrap = true;
                if (col.BaseColumnName != "Field")
                {
                    col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    col.CellStyle.Padding.Right = 5;
                }

            }

            HL.AddCell("Виды услуг");
            HL.AddCell("Начислено (предъявлено) жилищно-коммунальных платежей населению,тыс руб");
            HL.AddCell("Фактически оплачено,тыс. руб.");
            HL.AddCell("Процент неоплаченных коммунальных услуг");
            GridHeaderCell cell = HL.AddCell("Фактические объемы финансирования из бюджетов всех уровней на предоставление отдельным категориям граждан, тыс руб.");
            cell.AddCell("социальной поддержки по оплате жилищно-коммунальных услуг");
            cell.AddCell("субсидий по оплате жилищно-коммунальных услуг");
            HL.AddCell("Стоимость предоставлен-ных населению услуг, рассчитанная по экономически обоснованным тарифам,тыс руб");
            cell = HL.AddCell("Возмещение населением затрат за предоставление услуг, тыс руб.");
            cell.AddCell("по установленным для населения тарифам");
            cell.AddCell("Фактическое");

            HL.AddCell("Обслуживаемый жилищный фонд, м2");
            HL.AddCell("Число проживающих в обслуживаемом жилищном фонде, которым оказываются ЖКУ, чел");

            HL.ApplyHeaderInfo();

            foreach (HeaderBase hb in Grid.Bands[0].HeaderLayout)
            {
                hb.Style.Wrap = true;
                hb.Style.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        private UltraGridCell GetNextCell(UltraGridCell Cell)
        {
            return Cell.Row.NextRow.Cells.FromKey(Cell.Column.BaseColumnName);
        }
        private UltraGridCell GetPrevCell(UltraGridCell Cell)
        {
            return Cell.Row.PrevRow.Cells.FromKey(Cell.Column.BaseColumnName);
        }

        void MergeCellsGrid(UltraWebGrid Grid, int col)
        {
            UltraGridRow MerdgRow = Grid.Rows[0];
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[col].Text == MerdgRow.Cells[col].Text)
                {

                }
                else
                {
                    MerdgRow.Cells[col].RowSpan = Row.Index - MerdgRow.Index;
                    MerdgRow = Row;
                }
            }
            MerdgRow.Cells[col].RowSpan = Grid.Rows.Count - MerdgRow.Index;
        }
        UltraGridCell GetNextVertCell(UltraGridCell cell)
        {
            return cell.Row.Cells[cell.Column.Index + 1];
        }

        private void FormatRowYsly(UltraWebGrid Grid, string p)
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                UltraGridCell cell = Row.Cells.FromKey(p);
                if (GetNextVertCell(cell).Value == null)
                {
                    if (SetRegion.DataForCombo.ContainsKey(cell.Text))
                    {
                        cell.Style.Font.Bold = true;
                        cell.Style.BackColor = Color.FromArgb(200, 200, 200);
                        cell.ColSpan = 4;
                    }
                    else
                    {
                        cell.Style.Padding.Left = 10;
                        cell.Style.BackColor = Color.FromArgb(222, 222, 222);
                        cell.ColSpan = 4;
                    }
                }
                else
                {
                    cell.Style.Padding.Left = 20;
                }
            }
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center";
            Cell.Style.BackgroundImage = ImagePath;
        }

        string FormatstringHint(string h)
        {
            return h.Replace("квартал", "кварталу");
        }

        string FormatTitle(string title)
        {
            if (title.Contains("полугодие") ||(title.Contains("месяцев"))|| (title.Contains("квартал")))
            {
                return title.Replace("полугодие", "полугодию").Replace("квартал", "кварталу").Replace("месяцев","месяцам");
            }
            else
            {
                return title.Replace("год", "году");
            }
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
                    string UpOrdouwn = Value > 100 ? "Up" : "Down";

                    string TitleCaption = Value >= 100 ? "роста" : "снижения";

                    
                    try
                    {
                        IndicatorCell.Title =
                            FormatTitle(string.Format("Темп {0} к {1}", TitleCaption,
                            ((ComboCurDay.SelectedNode).Text)).Replace(
                    ChosenYear.Value,
                    (int.Parse(ChosenYear.Value) - 1).ToString()));

                        TitleCaption = Value > 100 ? "Прирост" : "Снижение";

                        if (Value == 100)
                        {
                            TitleCaption = "Абсолютное отклонение";
                        }

                        GetNextCell(ValueCell).Title = FormatTitle(string.Format("{0} к {1}", TitleCaption, ((ComboCurDay.SelectedNode).Text)).Replace(
                    ChosenYear.Value,
                    (int.Parse(ChosenYear.Value) - 1).ToString())); ;
                    }
                    catch { }

                    


                    string Color = "";
                    string revColor = "";
                    if ((Value > 100))
                    {
                        Color = "Red";
                        revColor = "Green";
                    }

                    if ((Value < 100))
                    {
                        Color = "Green";
                        revColor = "Red";
                    }

                    if (reverce)
                    {
                        Color = revColor;
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


        void SetFormatCell(UltraGridCell Cell, string format)
        {
            if (Cell.Value != null)
            {
                try
                {
                    Cell.Text = string.Format("{0:" + format + "}", decimal.Parse(Cell.Text.Replace("%", "")));
                }
                catch { }
            }
        }

        private bool IsReverceCol(string p)
        {
            return p.Contains("Начислено (предъявлено) жилищно-коммунальных платежей населению") || p.Contains("Процент неоплаченных коммунальных услуг") | p.Contains("Стоимость предоставленных населению услуг, рассчитанная по экономически обоснованным тарифам");
        }

        private void FormatValueCell(UltraGridCell ValueCell)
        {
            FormatTopCell(ValueCell);
            FormatCenterCell(GetNextCell(ValueCell));
            FormatBottomCell(GetNextCell(GetNextCell(ValueCell)));

            if (ValueCell.Value == null)
            {
                return;
            }
            if (ValueCell.Column.BaseColumnName == " Процент неоплаченных коммунальных услуг")
            {
                SetFormatCell(ValueCell, "P2");
            }
            else
            {
                SetFormatCell(ValueCell, "N2");
            }
            SetFormatCell(GetNextCell(ValueCell), "N2");
            SetFormatCell(GetNextCell(GetNextCell(ValueCell)), "P2");

            //SetIndicatorDeviationcell(ValueCell, false);
            SetIndicatorSpeedDeviationcell(ValueCell, !IsReverceCol(ValueCell.Column.BaseColumnName));
        }



        private int GetLevelFromField(string Field)
        {
            switch (Field.ToLower())
            {
                case "жилищные":
                    {
                        return 0;
                    }
                case "найм жилого помещения":
                    {
                        return 1;
                    }
                case "ремонт жилого помещения":
                    {
                        return 1;
                    }
                case "ремонт жилого помещения со всеми видами благоустройства,включая лифты и мусопроводы":
                    {
                        return 2;
                    }
                case "ремонт жилого помещения со всеми видами благоустройства, кроме лифтов и мусопроводов":
                    {
                        return 2;
                    }
                case "вывоз тбо":
                    {
                        return 2;
                    }
                case "коммунальные":
                    {
                        return 0;
                    }
                case "водоснабжение":
                    {
                        return 1;
                    }
                case "электроснабжение":
                    {
                        return 1;
                    }
                case "электроснабжение в домах с газовыми плитами":
                    {
                        return 2;
                    }
                case "электроснабжение в домах с электропитанием":
                    {
                        return 2;
                    }
                case "водоотведение":
                    {
                        return 1;
                    }
                case "газоснабжение сетевым газом":
                    {
                        return 1;
                    }
                case "газоснабжение сжиженным газом":
                    {
                        return 1;
                    }
                case "поставка бытового газа в баллонах":
                    {
                        return 2;
                    }
                case "отопление":
                    {
                        return 1;
                    }
                case "горячее водоснабжение":
                    {
                        return 1;
                    }
                case "поставка твердого топлива (уголь)":
                    {
                        return 1;
                    }
                case "поставка твердого топлива (дрова)":
                    {
                        return 1;
                    }
                case "жилищно-коммунальные услуги":
                    {
                        return 0;
                    }
                case "норма площади жилого помещения на 1 гражданина, м2":
                    {
                        return 0;
                    }
            }
            return 0;
        }

        private void FormatRow()
        {
            {
                MergeCellsGrid(Grid, 0);
                foreach (UltraGridRow Row in Grid.Rows)
                {
                    {
                        if (Row.Cells.FromKey("Field").RowSpan == 3)
                        {
                            Row.Cells.FromKey("Field").Style.Padding.Left =
                                15 *
                                GetLevelFromField(Row.Cells.FromKey("Field").Text);
                            for (int i = 1; i < Grid.Columns.Count; i++)
                            {
                                try
                                { }
                                catch { }
                                FormatValueCell(Row.Cells[i]);

                            }
                        }
                    }
                }
            }
        }


        void CustomizerOther()
        {
            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;

            Grid.DisplayLayout.NoDataMessage = "Нет данных";

            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;
        }


        private void CustomizeGrid()
        {
            FormatRow();

            ConfHeader();

            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }

            CustomizerSize.ContfigurationGrid(Grid);

            CustomizerOther();
        }

        private void GenerationGrid()
        {
            Grid.Columns.Clear();
            Grid.Bands.Clear();
            Grid.Rows.Clear();
            DataBindGrid();
             
            if (Grid.Rows.Count > 0)
            {
                CustomizeGrid();
            }
        }

        void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveRow(e.Row);
        }

        void ActiveRow(UltraGridRow Row)
        {
            if (Row.Cells[0].Value != null)
            {

                SelectField.Value = DictEdizm_re[Row.Cells[0].Text];
                SelectLastRow.Value = Row.Cells[0].Text;
            }
            else
            {
                if (Row.PrevRow.Cells[0].Value != null)
                {
                    SelectField.Value = DictEdizm_re[Row.PrevRow.Cells[0].Text];
                    SelectLastRow.Value = Row.PrevRow.Cells[0].Text;
                }
                else
                {
                    SelectField.Value = DictEdizm_re[Row.PrevRow.PrevRow.Cells[0].Text];
                    SelectLastRow.Value = Row.PrevRow.PrevRow.Cells[0].Text;
                }
            }


            foreach (UltraGridRow row in Grid.Rows)
            {
                row.Activated = false;
                row.Selected = false;
            }
            Row.Activate();
            Row.Activated = true;
            Row.Selected = true;

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
                DictEdizm.Add(Row[0].ToString(), "рублей за "+ Row[1].ToString());
            }

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();

            ChosenParam();

            GenerationGrid();

            SetHeaderReport();
        }




        private string getbig2()
        {

            string res = "  ";
            int IndexLastCol = Grid.Columns.Count - 1;
            for (int i = 2; i < Grid.Rows.Count; i += 3)
            {
                if ((Grid.Rows[i].Cells[IndexLastCol].Value != null) &&
                    (decimal.Parse(Grid.Rows[i].Cells[IndexLastCol].Value.ToString().Replace("%", "")) >= 2))
                {
                    res += string.Format("«{0}» (<b>{1}</b>), ", Grid.Rows[i].PrevRow.PrevRow.Cells[0].Text,
                        Grid.Rows[i].Cells[IndexLastCol].Text);
                }
            }
            return res.Remove(res.Length - 2);

        }

        private string getsmal2()
        {

            string res = "  ";
            int IndexLastCol = Grid.Columns.Count - 1;
            for (int i = 2; i < Grid.Rows.Count; i += 3)
            {

                if ((Grid.Rows[i].Cells[IndexLastCol].Value != null) &&
                    (decimal.Parse(Grid.Rows[i].Cells[IndexLastCol].Value.ToString().Replace("%", "")) <= -2))
                {
                    res += string.Format("«{0}» (<b>{1}</b>), ", Grid.Rows[i].PrevRow.PrevRow.Cells[0].Text, Grid.Rows[i].Cells[IndexLastCol].Text);
                }
            }

            return res.Remove(res.Length - 2);


        }





        private void SetHeaderReport()
        {
            PageSubTitle.Text = string.Format("Анализ оплаты населением жилищно-коммунальных услуг по данным Формы №22-ЖКХ, {1}, за {0}",
                ComboCurDay.SelectedValue.ToLower(), ComboRegion.SelectedValue);

            Hederglobal.Text = "Платежи населения";
            Page.Title = Hederglobal.Text;
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
            try
            {
                ReportExcelExporter1.WorksheetTitle = Hederglobal.Text;
                ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

                ReportExcelExporter1.RowsAutoFitEnable = false;
                ReportExcelExporter1.HeaderCellHeight = 60;

                Workbook workbook = new Workbook();
                Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
                ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
                ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
                ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);

                //Grid.Columns.FromKey("Стоимость предоставлен-ных населению услуг, рассчитанная по экономически обоснованным тарифам,тыс руб"

                ReportExcelExporter1.Export(HL, sheet1, 3);

                for (int i = 3; i < Grid.Rows.Count; i++)
                {
                    //  if (sheet1.Rows[i].Cells[0].Value != null)
                    {

                        sheet1.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                        sheet1.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                        sheet1.Rows[i].Cells[0].CellFormat.Indent = 3;
                        //GetMarginFromGridField(sheet1.Rows[i].Cells[0].Value.ToString());
                    }
                }

                ReportExcelExporter1.WorksheetTitle = String.Empty;
                ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            }
            catch { }
        }

        int GetMarginFromGridField(string f)
        {
            foreach (UltraGridRow row in Grid.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    if (row.Cells[0].Text == f)
                    {                        
                        
                        return (int)row.Cells[0].Style.Padding.Left.Value/20;
                    }
                }
            }
            return 0;
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
                        MergRegion(r.Index + 5, r.Index + 5 + r.Cells[p].RowSpan - 1, worksheet, p);
                    }
                    catch { }
                }
                if (r.Cells[p].ColSpan > 1)
                {
                    try
                    {
                        MergRegion_(p, p + r.Cells[p].ColSpan - 1, worksheet, r.Index + 5);
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
            //ExcellMergerRow(e.CurrentWorksheet, 1);
        }



        #endregion


        void PDFMergRow(UltraWebGrid Grid, UltraGridRow Firstrow, UltraGridRow LastRow, int col)
        {

            string text = Firstrow.Cells[col].Text;
            for (int i = Firstrow.Index; i < LastRow.Index + 1; i++)
            {
                Grid.Rows[i].Cells[col].Text = " ";
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
            //if (row.Cells[0].ColSpan > 1)
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
            try
            {
                ReportPDFExporter1.PageTitle = Page.Title;
                ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

                Report report = new Report();
                ISection section1 = report.AddSection();


                foreach (UltraGridRow r in HL.Grid.Rows)
                {
                    try
                    {
                        //r.Cells[1].Text = r.Cells[1].Text.Split('|')[1];
                    }
                    catch { }
                }

                ReportPDFExporter1.HeaderCellHeight = 50;
                MergeCellsGridFormPDF(HL.Grid, 0);
                //MergeCellsGridFormPDF(HL.Grid, 1);

                //foreach (UltraGridRow row in Grid.Rows)
                //{
                //    MergeCellsGridFormPDFCol(HL.Grid, row);
                //}

                ReportPDFExporter1.Export(HL, section1);
            }
            catch { }
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