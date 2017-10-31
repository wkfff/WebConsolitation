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


namespace Krista.FM.Server.Dashboards.GKH_0003
{
    public partial class _default : CustomReportPage
    {
        IDataSetCombo SetCurDay;
        IDataSetCombo SetYslyg;

        IDataSetCombo SetRegionLevel;

        ICustomizerSize CustomizerSize;

        bool SelectCity;

        CustomParam RegionBaseDimension { get { return (UserParams.CustomParam("RegionBaseDimension")); } }

        CustomParam IsHmao { get { return (UserParams.CustomParam("IsHmao")); } }

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }

        CustomParam ChosenYslyg { get { return (UserParams.CustomParam("ChosenYslyg")); } }
        CustomParam ChosenBlago { get { return (UserParams.CustomParam("ChosenBlago")); } }

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
                int FirstPer = 20;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((100-FirstPer) / (Grid.Columns.Count-1)) * onePercent;
                }
                Grid.Columns[0].Width = FirstPer * onePercent;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                int FirstPer = 20;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((100-FirstPer) / (Grid.Columns.Count-1)) * onePercent;
                }
                Grid.Columns[0].Width = FirstPer * onePercent;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;

                int FirstPer = 20;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((100-FirstPer) / (Grid.Columns.Count-1)) * onePercent;
                }
                Grid.Columns[0].Width = FirstPer * onePercent;
                
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
                        return //1100;
                            (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
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
                int onePercent = (int)1280 / 98;

                int PercFirstCol = 30;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((96 - PercFirstCol) / Grid.Columns.Count - 1) * onePercent;
                }
                Grid.Columns[0].Width = PercFirstCol * onePercent;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 98;

                int PercFirstCol = 30;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((96 - PercFirstCol) / Grid.Columns.Count - 1) * onePercent;
                }
                Grid.Columns[0].Width = PercFirstCol * onePercent;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 98;

                int PercFirstCol = 30;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = ((96 - PercFirstCol) / Grid.Columns.Count - 1) * onePercent;
                }
                Grid.Columns[0].Width = PercFirstCol * onePercent;

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

                    string DisplayNAme = this.GetAlowableAndFormatedKey(row["DisplayName"].ToString(), "").ToLower();

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
            public override void LoadData(DataTable Table)
            {
                string LYear = "";


                foreach (DataRow Row in Table.Rows)
                {
                    string m = Row["mounth"].ToString();
                    string y = Row["year"].ToString();
                    string uname = Row["uname"].ToString();

                    if (y != LYear)
                    {
                        this.AddItem(y + " год", 0, "");
                        LYear = y;
                    }

                    m = GetAlowableAndFormatedKey(m + " " + y + " года", "");

                    this.AddItem(m, 1, uname);
                    this.addOtherInfo(Table, Row, m);
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
            ComboCurDay.Width = IsSmall ? 430 : 500;
            ComboYslyga.Width = IsSmall ? 430 : 500;
            ComboComapraDate.Width = IsSmall ? 430 : 500;

            //ComboCurDay.Width = IsSmall ? 430 : 250;
            //ComboYslyga.Width = IsSmall ? 430 : 400;
            //ComboComapraDate.Width = IsSmall ? 430 : 320;


            ComboCurDay.Title = "Выберите период";
            ComboCurDay.ParentSelect = false;

            ComboYslyga.Title = "Вид услуги";
            ComboYslyga.ParentSelect = true;

            ComboComapraDate.Title = "Выберите период для сравнения";

            Link1.Text = "Мониторинг тарифов и нормативов на коммунальные услуги в разрезе организаций, предоставляющих услуги ЖКХ";


            if (true)//(IsSmall)
            {
                Link1.Text = "Мониторинг тарифов и нормативов на коммунальные услуги в разрезе организаций, предоставляющих услуги ЖКХ";

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
        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            ConfCombo(false);
        }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            ColChart.BackColor = Color.White;
            if (RegionSettingsHelper.Instance.Name == "Ямало-Ненецкий автономный округ")
                PopupInformer1.HelpPageUrl = "Help_YANAO.html";

            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(sad);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            #endregion
            
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;

            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

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

            Grid.Width = CustomizerSize.GetGridWidth();
            ColChart.Width = CustomizerSize.GetChartWidth();

            Grid.Height = 570;

            DundasMap.Width = CustomizerSize.GetChartWidth();
            DundasMap.Height = 600;

            Grid.Height = Unit.Empty;
            
            
            ComboCurDay.Title = "Выберите период";

            

        }



        private void FillComboPeriodCur()
        {

            ComboCurDay.ParentSelect = false;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboYear", "DisplayName");

            SetCurDay = new DataSetComboPeriod();
            //DataSetComboHierarhy();
            SetCurDay.LoadData(Table);

            if (!Page.IsPostBack)
            {
                ComboCurDay.FillDictionaryValues(SetCurDay.DataForCombo);
                ComboCurDay.SetСheckedState(SetCurDay.LastAdededKey, 1 == 1);
            }
        }

        private void FillComboRegion()
        {
            ComboYslyga.ParentSelect = true;


            ComboYslyga.Title = "Вид услуги";

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboRegion", "---");

            SetYslyg = new DataSetComboHierarhy(); 
            SetYslyg.LoadData(Table);

            if (!Page.IsPostBack)
            {
                ComboYslyga.FillDictionaryValues(SetYslyg.DataForCombo);
            }
        }

        private void FillComboCompare()
        {
            if (!Page.IsPostBack)
            {
                Dictionary<string, int> dict = new System.Collections.Generic.Dictionary<string, int>();
                dict.Add("к предыдущему периоду", 0);
                dict.Add("к аналогичному периоду предыдущего года", 0);
                dict.Add("к началу года", 0);
                ComboComapraDate.FillDictionaryValues(dict);
                ComboComapraDate.Title = "Выберите период для сравнения";
                
            }
        }

        private void FillCombo()
        {
            RegionBaseDimension.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

            FillComboPeriodCur();

            FillComboRegion();

            FillComboCompare();
        }

        Node GetPrevNode(Node n)
        { 
            if (n.PrevNode == null)
            {
                if (n.Parent.PrevNode == null)
                {
                    //return n.Parent.Parent.Nodes[n.Parent.Parent.Nodes.Count - 1].Nodes[n.Parent.Parent.Nodes[n.Parent.Parent.Nodes.Count - 1].Nodes.Count - 1];
                    return n;
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



        private void FillSetLevelRegion()
        {
            DataTable Table = DataBaseHelper.ExecQueryByID("RegionLevel", "DisplayName");

            SetRegionLevel = new DataSetComboHierarhy();
            SetRegionLevel.LoadData(Table);


        }

        string GetYearFromUname(string Uname)
        {
            Uname = Uname.Replace("[", "").Replace("]", "");
            return Uname.Split('.')[3];
        }

        private string GetCompareDate()
        {
            if (!ComboComapraDate.SelectedValue.Contains("предыдущему"))
            {
                string year = GetYearFromUname(ChosenCurPeriod.Value);
                if (ComboComapraDate.SelectedValue.Contains("началу"))
                {
                    string FirstYearDate = string.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 1].[Январь]", year);

                    if (FirstYearDate == ChosenCurPeriod.Value)
                    {
                        return "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[empty]";
                    }

                    return FirstYearDate;
                }
                else
                {
                    string prevYear = (int.Parse(year) - 1).ToString();
                    return ChosenCurPeriod.Value.Replace(year, prevYear);
                }
            }
            return GetPrevNode(ComboCurDay.SelectedNode).Text == ComboCurDay.SelectedNode.Text ?
               "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[empty]" :
               SetCurDay.DataUniqeName[GetPrevNode(ComboCurDay.SelectedNode).Text];
        }

        private void ChosenParam()
        {
            if (!Page.IsPostBack)
            {
                //SelectDouwnLevel(ComboCurDay.SelectedNode, false, ComboCurDay);
            }

            ChosenCurPeriod.Value = SetCurDay.DataUniqeName[ComboCurDay.SelectedNode.Text];
            ChosenPrevPeriod.Value = GetCompareDate();
            ChosenYslyg.Value = SetYslyg.DataUniqeName[ComboYslyga.SelectedValue];
            ChosenBlago.Value = SetYslyg.OtherInfo[ComboYslyga.SelectedValue]["UniqueName_"];

            FillSetLevelRegion();

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
                        return ((decimal)CurValue / (decimal)PrevValue);
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
        private void fillStrRow(DataRow GridRow, string[] ParsRow)
        {
            GridRow["Yslyga"] = ParsRow[0];
        }


        private object GetSum(object p, object p_2)
        {
            decimal val1 = 0;
            decimal val2 = 0;
            if ((p == DBNull.Value) && (p_2 == DBNull.Value))
            {
                return DBNull.Value;
            }
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

            if ((val1 + val2) > 0)
            {
                return val1 + val2;
            }
            return DBNull.Value;

            

        }
        private void SumCol(DataTable BaseTable, string val1, string val2, string val3)
        {
            foreach (DataRow Row in BaseTable.Rows)
            {
                Row[val3] = GetSum(Row[val1], Row[val2]);
            }
        }

        private bool ContainCurRegion(DataTable Table, string reg)
        {
            foreach (DataRow row in Table.Rows)
            {
                string CurRegionName = row[0].ToString().Split(';')[0];
                if (CurRegionName == reg)
                {
                    return true;
                }
            }
            return false;
        }

        DataTable RemoveAllOrg(DataTable BaseTable)
        {
            DataTable NewBaseTable = new DataTable();
            foreach (DataColumn col in BaseTable.Columns)
            {
                NewBaseTable.Columns.Add(col.ColumnName, col.DataType);
            }

            foreach (DataRow row in BaseTable.Rows)
            {
                string CurRegionName = row[0].ToString().Split(';')[0];
                if ((SetRegionLevel.OtherInfo[CurRegionName.ToLower()]["LevelNumber"] == "0")
                    &&
                    (ContainCurRegion(NewBaseTable, CurRegionName)))
                {

                }
                else
                {
                    NewBaseTable.Rows.Add(row.ItemArray);
                }
            }

            return NewBaseTable;
        }

        DataTable FormatBaseTable(DataTable BaseTable)
        {
            string CurMounth = SetCurDay.OtherInfo[ComboCurDay.SelectedValue]["mounth"];
            string PrevMounth = UserComboBox.getLastBlock(ChosenPrevPeriod.Value);
            
            SumCol(BaseTable,
                CurMounth + "; по приборам учета; Тариф ",
                CurMounth + "; при наличии приборов учёта; Тариф ",
                CurMounth + "; при наличии приборов учёта; Тариф ");

            SumCol(BaseTable,
                PrevMounth + "; по приборам учета; Тариф ",
                PrevMounth + "; при наличии приборов учёта; Тариф ",
                PrevMounth + "; при наличии приборов учёта; Тариф ");

            SumCol(BaseTable,
                PrevMounth + "; по приборам учета; Норматив ",
                PrevMounth + "; при отсутствии приборов учёта ; Норматив ",
                PrevMounth + "; при отсутствии приборов учёта ; Норматив ");

            SumCol(BaseTable,
                CurMounth + "; по приборам учета; Норматив ",
                CurMounth + "; при отсутствии приборов учёта ; Норматив ",
                CurMounth + "; при отсутствии приборов учёта ; Норматив ");

            SumCol(BaseTable,
                PrevMounth + "; по приборам учета; Тариф ",
                PrevMounth + "; при отсутствии приборов учёта ; Тариф ",
                PrevMounth + "; при отсутствии приборов учёта ; Тариф ");

            SumCol(BaseTable,
                CurMounth + "; по приборам учета; Тариф ",
                CurMounth + "; при отсутствии приборов учёта ; Тариф ",
                CurMounth + "; при отсутствии приборов учёта ; Тариф ");

            BaseTable.Columns.Remove(CurMounth + "; по приборам учета; Тариф ");
            BaseTable.Columns.Remove(CurMounth + "; по приборам учета; Норматив ");
            if (!BaseTable.Columns.Contains(PrevMounth + "; по приборам учета; Тариф "))
            {
                BaseTable.Columns.Remove(PrevMounth + "; по приборам учета; Тариф  ");
            }
            else
            {
                BaseTable.Columns.Remove(PrevMounth + "; по приборам учета; Тариф "); 
            }
            if (!BaseTable.Columns.Contains(PrevMounth + "; по приборам учета; Норматив "))
            {
                BaseTable.Columns.Remove(PrevMounth + "; по приборам учета; Норматив  ");
            }
            else
            {
                BaseTable.Columns.Remove(PrevMounth + "; по приборам учета; Норматив ");
            }

            return BaseTable;
        }
        bool IsParentRegion(string s)
        {
            return ((s.Contains("Город") ||
                   (s.Contains("район")))) ||
                   (s.Contains("Город Надым"));
        }

        private void CalcRang(List<DataRow> ListRow, string col)
        {
            if (ListRow.Count == 0)
                return;
            RankCalculator rc = new RankCalculator(RankDirection.Desc);
            foreach (DataRow Row in ListRow)
            {
                try
                {
                    rc.AddItem((double)(decimal)Row[col]);
                }
                catch { }
            }
            if (rc.GetMaxValue() == rc.GetMinValue())
            {
                foreach (DataRow Row in ListRow)
                {
                    Row[col] = DBNull.Value;
                }
                return;

            }
            foreach (DataRow Row in ListRow)
            {
                try
                {
                    Row[col] = rc.GetRank((double)(decimal)Row[col]);
                }
                catch { Row[col] = DBNull.Value; }
            }

        }

        private void EvalRang(List<DataRow> ForRankNoMO)
        {
            CalcRang(ForRankNoMO, "val1-Rank");
            CalcRang(ForRankNoMO, "val2-Rank");
            //CalcRang(ForRankNoMO, 3);
        }
        int CountMO = 0;
        Dictionary<string, int> CountChild = new Dictionary<string, int>();

        private void ClearEmpty(DataTable BaseTable)
        {
            foreach (DataRow row in BaseTable.Rows)
            {
                foreach (DataColumn col in BaseTable.Columns)
                {

                    if (row[col] != DBNull.Value)
                    {
                        if (row[col] is decimal)
                        {
                            if ((decimal)row[col] == -1)
                            {
                                row[col] = DBNull.Value;
                            }
                        }
                    }
                }
            }
        }
        DataColumn GetColFromContainsCaption(DataTable Table, string caption)
        {
            foreach (DataColumn col in Table.Columns)
            {
                if (col.Caption.Contains(caption))
                {
                    return col;
                }
            }
            return null;
        }

        List<string> children = new List<string>();
        List<string> parents = new List<string>();

        decimal CharrtVal1 = -1;
        decimal CharrtVal2 = -1;

        private void DataBindGrid()
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Field");

            ClearEmpty(BaseTable);

            if (BaseTable.Rows.Count > 0)
            {
                Grid.DataSource = null;
                Grid.DataBind();
            }

            BaseTable = FormatBaseTable(BaseTable);

            CoolTable GridTable = new CoolTable();

            GridTable.Columns.Add("Yslyga");
            GridTable.Columns.Add("val1", typeof(decimal));
            GridTable.Columns.Add("val1-Deviation", typeof(decimal));
            GridTable.Columns.Add("val1-SpeedDeviation", typeof(decimal));
            GridTable.Columns.Add("val1-Rank", typeof(decimal));

            GridTable.Columns.Add("val2", typeof(decimal));
            GridTable.Columns.Add("val2-Deviation", typeof(decimal));
            GridTable.Columns.Add("val2-SpeedDeviation", typeof(decimal));
            GridTable.Columns.Add("val2-Rank", typeof(decimal));
            GridTable.Columns.Add("val3", typeof(decimal));

            List<DataRow> ForRankMO = new List<DataRow>();
            List<DataRow> ForRankNoMO = new List<DataRow>();

            string lastparentRegion = "";

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                string[] ParsRow = BaseRow["Field"].ToString().Split(';');

                string ysl = ParsRow[1];
                string Reg = ParsRow[0];

                DataRow GridRow = GridTable.AddRow();
                fillStrRow(GridRow, ParsRow);

                string parent = BaseRow[GetColFromContainsCaption(BaseTable, "ParentRegion")].ToString();//.Replace("округом","");

                if (GridRow["Yslyga"].ToString().Contains("округ"))
                {
                    continue;
                }

                if (parent.Contains("округ"))
                {
                    children.Add(Reg);
                    if (!parents.Contains(Reg))
                    {
                        parents.Add(Reg);
                    }
                }
                if (!parents.Contains(parent))
                {
                    parents.Add(parent);
                }

                if (parent.Contains("округ"))
                {
                    if (lastparentRegion != Reg)
                    {
                        CountChild.Add(lastparentRegion, ForRankNoMO.Count);
                        EvalRang(ForRankNoMO);
                        ForRankNoMO = new List<DataRow>();
                        lastparentRegion = Reg;
                    }
                    ForRankMO.Add(GridRow);
                    CountMO++;
                }
                else
                {
                    ForRankNoMO.Add(GridRow);
                }

                int CountOtherCol = 1;

                GridRow["val1"] = BaseRow[1 + CountOtherCol];
                GridRow["val2"] = BaseRow[2 + CountOtherCol];
                GridRow["val3"] = BaseRow[3 + CountOtherCol];

                GridRow["val1-Rank"] = BaseRow[1 + CountOtherCol];
                GridRow["val2-Rank"] = BaseRow[2 + CountOtherCol];


                int adededPrevColNumber = 3;

                GridRow["val1-Deviation"] = GetDeviation(BaseRow[1 + CountOtherCol], BaseRow[1 + adededPrevColNumber + CountOtherCol]);
                GridRow["val2-Deviation"] = GetDeviation(BaseRow[2 + CountOtherCol], BaseRow[2 + adededPrevColNumber + CountOtherCol]);

                GridRow["val1-SpeedDeviation"] =
                    GetSpeedDeviation(BaseRow[1 + CountOtherCol], BaseRow[1 + adededPrevColNumber + CountOtherCol]);
                GridRow["val2-SpeedDeviation"] =
                    GetSpeedDeviation(BaseRow[2 + CountOtherCol], BaseRow[2 + adededPrevColNumber + CountOtherCol]);
            }

            CountChild.Add(lastparentRegion, ForRankNoMO.Count);

            

            EvalRang(ForRankNoMO);
            EvalRang(ForRankMO);

            GridTable.Columns.Remove("val3");

            SaveYanaoValue(GridTable);

            Grid.DataSource = GridTable;
            Grid.DataBind();
        }

        private void SaveYanaoValue(CoolTable GridTable)
        {
            try
            {
                if (GridTable.Rows[0]["val1"] != DBNull.Value)
                {
                    CharrtVal1 = (decimal)GridTable.Rows[0]["val1"];
                }
                if (GridTable.Rows[0]["val2"] != DBNull.Value)
                {
                    CharrtVal2 = (decimal)GridTable.Rows[0]["val2"];
                }
                GridTable.Rows[0].Delete();
            }
            catch { }
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

        string SetEdizm(string str)
        {
            return string.Format("{0}, рублей за {1}", str, SetYslyg.OtherInfo[ComboYslyga.SelectedValue]["EdIzm"]);
        }

        GridHeaderLayout HL;
        private void ConfHeader()
        {
            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.CellStyle.Wrap = true;

            }
            HL = new GridHeaderLayout(Grid);
            HL.AddCell("Территории");
            GridHeaderCell yearhe = HL.AddCell(string.Format("{0}", ComboCurDay.SelectedValue));
            GridHeaderCell cell = yearhe.AddCell("при наличии приборов учёта");
            cell.AddCell(SetEdizm("Средний тариф"));
            cell.AddCell("Абсолютное отклонение, рубль");
            cell.AddCell("Темп роста, %");
            cell.AddCell("Ранг");

            cell = yearhe.AddCell("при отсутствии приборов учёта ");
            cell.AddCell(SetEdizm("Средний тариф"));
            cell.AddCell("Абсолютное отклонение, рублей");
            cell.AddCell("Темп роста, %");
            cell.AddCell("Ранг");
            //cell.AddCell(SetEdizm("Норматив"));

            HL.ApplyHeaderInfo();
            foreach (ColumnHeader ch in Grid.Bands[0].HeaderLayout)
            {
                ch.Style.Wrap = true;
                ch.Style.HorizontalAlign = HorizontalAlign.Center;
            }
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val1"), "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val1-Deviation"), "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val1-SpeedDeviation"), "P2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val1-Rank"), "N0");

            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val2"), "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val2-Deviation"), "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val2-SpeedDeviation"), "P2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val2-Rank"), "N0");
            //CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val3"), "N2");
        }

        private UltraGridCell GetNextCell(UltraGridCell Cell)
        {
            //return Cell.Row.NextRow.Cells.FromKey(Cell.Column.BaseColumnName);
            return Cell.Row.Cells[Cell.Column.Index + 1];
        }
        private UltraGridCell GetPrevCell(UltraGridCell Cell)
        {
            //return Cell.Row.PrevRow.Cells.FromKey(Cell.Column.BaseColumnName);
            return Cell.Row.Cells[Cell.Column.Index - 1];
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
            }
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
            Cell.Style.BackgroundImage = ImagePath;
        }
        private void SetIndicatorSpeedDeviationcell(UltraGridCell ValueCell, bool reverce)
        {

            UltraGridCell IndicatorCell = GetNextCell(GetNextCell(ValueCell));
            
            if (IndicatorCell.Value != null)
            {
                int IndexCol = ValueCell.Column.Index;
                decimal Value = decimal.Parse(IndicatorCell.Text.ToString().Replace("%", ""));
                //if (Value != 1)
                {
                    string UpOrdouwn = Value > 1 ? "Up" : "Down";

                    string TitleCaption = Value > 1 ? "прироста" : "снижения";

                    //int CurYear = int.Parse(ComboCurDay.SelectedValue);
                    string CurMounth = SetCurDay.OtherInfo[ComboCurDay.SelectedValue]["mounth"];
                    string PrevMounth = CRHelper.RusMonthDat(
                    CRHelper.MonthNum(SetCurDay.OtherInfo[GetPrevNode(ComboCurDay.SelectedNode).Text]["mounth"]));

                    string PrevYear = 
                    SetCurDay.OtherInfo[GetPrevNode(ComboCurDay.SelectedNode).Text]["year"];

                    // if (Value != 100)
                    {
                        IndicatorCell.Title = string.Format("Темп роста к {0} {1} года", PrevMounth, PrevYear);
                    }

                    TitleCaption = Value > 1 ? "Прирост" : "Снижение";
                    if (Value != 1)
                        GetNextCell(ValueCell).Title = string.Format("{0} к {1} {2} года", TitleCaption, PrevMounth, PrevYear);
                    else
                        GetNextCell(ValueCell).Title = string.Format("Абсолютное отклонение к {0} {1} года", PrevMounth, PrevYear);

                    string Color = "";
                    if ((Value > 1))
                    {
                        Color = "Red";
                    }

                    if ((Value < 1))
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

        private string GetParentRegion(UltraGridRow ChildRow)
        {
            UltraGridRow Row = ChildRow;
            for (; !IsParentRegion(Row.Cells[0].Text); )
            {
                Row = Row.PrevRow;
            }
            return Row.Cells[0].Text;

        }


        int getMaxValueFromCol(UltraGridColumn col)
        {
            int max = int.MinValue;

            foreach (UltraGridRow row in Grid.Rows)
            {

                if (row.Cells.FromKey(col.BaseColumnName).Value != null)
                {
                    try
                    {
                        int value = int.Parse(row.Cells.FromKey(col.BaseColumnName).Text);
                        if (value > max)
                        {
                            max = value;
                        }
                    }
                    catch { }
                }
            }
            return max;
        }

        private void setRangformCell(UltraGridCell RangCell, int Max, int Min, string MegoParent)
        {
            if (RangCell.Value == null)
                return;
            int Rang = (int)(decimal)RangCell.Value;

            if (Max == Min)
            {
                RangCell.Value = null;
                return;
            }

            if (Rang == 0)
            {
                RangCell.Text = "";
                return;
            }
            //RangCell.Text = string.Format("Ранг по {0}:{1}", MegoParent.Replace("ий", "ому").Replace("муниципальный район", "м-р"), Rang);


            if (Rang == Min)
            {
                SetImageFromCell(RangCell, "starYellowBB.png");
                RangCell.Title = "Самый низкий уровень тарифа";
            }

            if (Rang == Max)
            {
                RangCell.Title = "Самый высокий уровень тарифа";
                SetImageFromCell(RangCell, "starGrayBB.png");

            }

        }

        private void FormatRangCell(UltraGridCell ValueCell)
        {
            UltraGridCell RangCell = GetNextCell(GetNextCell(GetNextCell(ValueCell)));
            // if ((IsParentRegion(ValueCell.Row.Cells[0].Text)) || (ValueCell.Row.Cells[0].Text.Contains("Город Надым")))
            if(parents.Contains(ValueCell.Row.Cells[0].Text))
            {
                setRangformCell(RangCell, getMaxValueFromCol(RangCell.Column), 1, RegionSettingsHelper.Instance.GetPropertyValue("ShortName"));
                RangCell.Style.Font.Bold = true;
            }
            else
            {
                //string parentRegion = GetParentRegion(ValueCell.Row);
                //RangCell.Value = null;
                //if (CountChild.ContainsKey(parentRegion))
                {
                    //setRangformCell(RangCell, CountChild[parentRegion], 1, parentRegion);
                }

            }

        }





        private void FormatValueCell(UltraGridCell ValueCell)
        {
            //FormatTopCell(ValueCell);
            //FormatCenterCell(GetNextCell(ValueCell));
            //FormatCenterCell(GetNextCell(GetNextCell(ValueCell)));
            //FormatBottomCell(GetNextCell(GetNextCell(GetNextCell(ValueCell))));

            if (ValueCell.Value == null)
            {
                return;
            }

            //SetFormatCell(ValueCell, "N2");
            //SetFormatCell(GetNextCell(ValueCell), "N2");
            //SetFormatCell(GetNextCell(GetNextCell(ValueCell)), "P2");
            if (!ValueCell.Column.BaseColumnName.Contains("val3"))
            {
                SetIndicatorDeviationcell(ValueCell, false);
                SetIndicatorSpeedDeviationcell(ValueCell, false);
                FormatRangCell(ValueCell);
            }
        }

        private string ClearDataMember(string p)
        {
            return p.Replace("ДАННЫЕ)", "").Replace("(", "");
        }
        private void FormatRow()
        {
            MergeCellsGrid(Grid, 0);
            FormatRowYsly(Grid, "Yslyga");
            foreach (UltraGridRow Row in Grid.Rows)
            {
                Row.Cells[0].Text = ClearDataMember(Row.Cells[0].Text);
                if (children.Contains(Row.Cells[0].Text))
                {
                    Row.Cells[0].Style.Padding.Left = 0 * 15;
                    if (!Row.Cells[0].Text.Contains("Город"))
                    {
                        Row.Style.BackColor = Color.FromArgb(240, 240, 240);
                        if (!parents.Contains(Row.Cells[0].Text))
                        {
                            //Row.Hidden = true;
                        }
                    }
                    Row.Cells[0].Style.Font.Bold = true;

                }
                else
                {
                    Row.Cells[0].Style.Padding.Left = 1 * 15;
                }

                //if (Row.Cells.FromKey("Yslyga").RowSpan == 4)
                {
                    FormatValueCell(Row.Cells.FromKey("val1"));
                    FormatValueCell(Row.Cells.FromKey("val2"));
                    //FormatValueCell(Row.Cells.FromKey("val3"));
                }
            }
        }


        void CustomizerOther()
        {
            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.OnClient;

            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;
        }


        private void CustomizeGrid()
        {
            CustomizerOther();

            FormatRow();

            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }

            ConfHeader();

            CustomizerSize.ContfigurationGrid(Grid);


        }

        private void GenerationGrid()
        {
            Grid.Bands.Clear();

            DataBindGrid();
            
            if (Grid.Rows.Count > 0)
            {
                CustomizeGrid();
            }
            else
            {
                Grid.DisplayLayout.NoDataMessage = "Нет данных";
                Grid.Bands.Clear();
            }
        }

        RankCalculator rcChart = new RankCalculator(RankDirection.Desc);

        decimal HMAOVALCHART = 0;

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

        private void DataBindColChart()
        {
            string indexActiveCol = RadioButton1.Checked ? "val1" : "val2";

            CoolTable TableChart = new CoolTable();

            TableChart.Columns.Add("Region");
            TableChart.Columns.Add("Value", typeof(decimal));

            foreach (UltraGridRow Row in Grid.Rows)
            {
                //if ((Row.Cells[0].RowSpan == 4))
                //&& (IsParentRegion(Row.Cells[0].Text)))
                {
                    if (Row.Cells.FromKey(indexActiveCol).Value != null)
                    {
                        try
                        {
                            //if (parents.Contains(Row.Cells[0].Text))
                            if (children.Contains(Row.Cells[0].Text))
                            {
                                DataRow rowChart = TableChart.AddRow();
                                rowChart["Region"] = Row.Cells[0].Text.Replace("муниципальный район", "м-р");
                                rowChart["Value"] = GetVal(indexActiveCol, Row);
                                rcChart.AddItem((double)GetVal(indexActiveCol, Row));
                                HMAOVALCHART += GetVal(indexActiveCol, Row);
                            }

                        }
                        catch { }
                    }
                }
            }

            if (TableChart.Rows.Count == 0)
            {
                return;
            }

            HMAOVALCHART = HMAOVALCHART / TableChart.Rows.Count;
            rcChart.AddItem((double)HMAOVALCHART);
            ColChart.DataSource = SortTable(TableChart);
            ColChart.DataBind();

            ColChart.Axis.Y.RangeMax = rcChart.GetMaxValue() * 1.1;
            ColChart.Axis.Y.RangeMin = rcChart.GetMinValue() * 0.9;
            ColChart.Axis.Y.RangeType = AxisRangeType.Custom;

            ColChart.Axis.Y.Labels.ItemFormat = AxisItemLabelFormat.Custom;
            ColChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
        }

        private static decimal GetVal(string indexActiveCol, UltraGridRow Row)
        {

            return decimal.Parse(Row.Cells.FromKey(indexActiveCol).Text);
        }

        private void FillColorModelChart()
        {
            for (int i = 0; i < 1; ++i)
            {
                PaintElement pe = new PaintElement();

                switch (i)
                {
                    case 0:
                        {
                            pe.Fill = Color.Orange;
                            break;
                        }
                }

                pe.FillStopColor = pe.Fill;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 200;
                pe.FillStopOpacity = 100;

                ColChart.ColorModel.Skin.PEs.Add(pe);
            }
        }


        private void ConfColChart()
        {
            ColChart.BackColor = Color.White;

            ColChart.ChartType = ChartType.ColumnChart;

            ColChart.Legend.Visible = false;
            ColChart.Axis.X.Labels.Visible = false;

            ColChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ### ##0.00>";

            ColChart.Axis.X.Margin.Near.Value = 3;
            ColChart.Axis.X.Margin.Far.Value = 3;
            ColChart.Axis.Y.Extent = 40;
            ColChart.Axis.X.Extent = 40;


            ColChart.Tooltips.FormatString = "<SERIES_LABEL><br><b><DATA_VALUE:N2></b>, рубль";

            ColChart.BackColor = Color.White;
                //Color.Transparent;

            #region Настройка диаграммы

            ColChart.Height = Unit.Empty;

            ColChart.ChartType = ChartType.ColumnChart;
            ColChart.Border.Thickness = 0;

            ColChart.ColumnChart.SeriesSpacing = 1;
            ColChart.ColumnChart.ColumnSpacing = 1;

            ColChart.Axis.X.Extent = 150;

            ColChart.Axis.X.Labels.SeriesLabels.Visible = true;
            ColChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;


            ColChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            ColChart.Axis.Y.Extent = 35;



            ColChart.ColorModel.ColorBegin = Color.Orange;
            ColChart.ColorModel.ColorEnd = Color.Red;


            ColChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            ColChart.ColorModel.Skin.ApplyRowWise = true;
            ColChart.ColorModel.Skin.PEs.Clear();

            FillColorModelChart();

            ColChart.BackColor = //Color.Transparent;
                Color.White; 
            ColChart.ChartType = ChartType.ColumnChart;

            #endregion

            ColChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ColChart_FillSceneGraph);

            ColChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(ColChart_InvalidDataReceived);

            ColChart.Axis.Y.Labels.ItemFormat = AxisItemLabelFormat.Custom;
            ColChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";

        }



        void ColChart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }







        private void GenerateColChart()
        {
            ConfColChart();

            ColChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            DataBindColChart();
        }

        #region DundasMap
        protected void CustomizeMapPanel()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = 1 == 1;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap.NavigationPanel.Visible = 1 == 1;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.Viewport.EnablePanning = true;
        }

        protected void AddShapeField()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Value");
            DundasMap.ShapeFields["Value"].Type = typeof(double);
            DundasMap.ShapeFields["Value"].UniqueIdentifier = false;
        }

        protected string SetHyphen(string BaseString)
        {


            string[] Word = BaseString.Split(' ');
            int Median = Word.Length / 2;

            string res = "";
            for (int i = 0; i < Median; i++)
            {
                res += Word[i] + " ";
            }
            res += "\n";
            for (int i = Median; i < Word.Length; i++)
            {
                res += Word[i] + " ";
            }

            return res;

        }

        protected void AddLegend()
        {
            DundasMap.Legends.Clear();
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Title = "Средний тариф, " + GetUnit();
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Right;
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

            legend1.AutoFitMinFontSize = 7;

            DundasMap.Legends.Add(legend1);
        }

        protected void FillRule()
        {

            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorValueRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Value";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.MiddleColor = Color.Yellow;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";

            rule.LegendText = "#FROMVALUE{N1} - #TOVALUE{N1}";//"LegendText";
            DundasMap.ShapeRules.Add(rule);
        }

        protected string ShorteningFO(string LongName)
        {
            switch (LongName)
            {
                case "Центральный федеральный округ": { return "ЦФО"; }
                case "Северо-Западный федеральный округ": { return "СФО"; }
                case "Южный федеральный округ": { return "ЮФО"; }
                case "Северо-Кавказский федеральный округ": { return "СКФО"; }
                case "Приволжский федеральный округ": { return "ПФО"; }
                case "Уральский федеральный округ": { return "УрФО"; }
                case "Сибирский федеральный округ": { return "СФО"; }
                case "Дальневосточный федеральный округ": { return "ДФО"; }
            }

            throw new Exception("Неизвестный регион");
        }

        protected void LoadMap()
        {

            string mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapPath");

            //что нють то точно загрузится ;)
            try
            {
                AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
            }
            catch { }
            //ии
            try
            {
                AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.Areas);
            }
            catch { }
            try
            {
                AddMapLayer(DundasMap, mapFolderName, "Граница", CRHelper.MapShapeType.SublingAreas);
            }
            catch { }

        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        protected void ConfigurationMap()
        {
            CustomizeMapPanel();

            AddShapeField();

            AddLegend();

            FillRule();

            LoadMap();

            DataBindMap();
        }

        protected Shape GetShapefromRegionName(string RegionShortName, string RegionFullName)
        {
            foreach (Shape shape in DundasMap.Shapes)
            {
                if ((RegionShortName == shape["NAME"].ToString()) || (RegionFullName == shape["NAME"].ToString()))
                {
                    return shape;
                }
            }
            return null;
        }
        protected void SetShapeTooltip(Shape shape, DataRow dataRow)
        {
            if (dataRow["RangFO"] != DBNull.Value)
            {
                shape.ToolTip = string.Format("{0}\n{1:########0.00}, {2}\nРанг по ФО:{3}",
                    dataRow["Region"],
                    dataRow["Value"],
                    "Единица измерения".ToLower(),
                    dataRow["RangFO"]);
            }
            else
            {
                shape.ToolTip = string.Format("{0}\n{1:### ### ##0.00}, {2}",
                    dataRow["Region"],
                    dataRow["Value"],
                    "Единица измерения");
            }
        }

        private void FillShape(Shape shape, DataRow dataRow)
        {
            if (shape == null)
            {
                return;
            }

            if (dataRow["Value"] == DBNull.Value)
            {
                return;
            }


            //shape["Value"] = dataRow["Value"];
            //shape.Text = string.Format("{0}\n{1:### ### ##0.##}", dataRow["Region"], dataRow["Value"]);

            //shape.Visible = true;

            //shape.TextVisibility = TextVisibility.Shown;

            ////SetShapeTooltip(shape, dataRow);
        }

        string GenShortNameRegion_(string NameRegion)
        {
            return NameRegion
                .ToLower()
                .Replace("город", "г.")
                .Replace("муниципальный район", "р-н")
                .Replace(" ", "")
                .Replace("г.", "")
                .Replace("р-н", "")
                .Replace("_callout","");
        }
        string GenShortNameRegion(string NameRegion)
        {
            return NameRegion
                .Replace("Город", "г.")
                .Replace("муниципальный район", "р-н");
        }

        UltraGridRow GetValueFromNameRegion(UltraWebGrid Table, string Region)
        {
            int indexActiveCol = RadioButton1.Checked ? 1 : 5;
            foreach (UltraGridRow row in Table.Rows)
            {
                {
                    if (GenShortNameRegion_(row.Cells[0].ToString()) == GenShortNameRegion_(Region))
                    {
                        if (row.Cells[indexActiveCol].Value != null)
                        {
                            return row;
                        }
                    }
                }
            }
            return null;

        }

        enum ShapeType
        {
            City, Region, City_out
        }

        ShapeType GetShapeType(string region)
        {
            if (region[0] == 'г')
            {
                if (region.Split('_').Length == 1)
                {
                    return ShapeType.City;
                }
                else
                {
                    return ShapeType.City_out;
                }
            }
            else
            {
                return ShapeType.Region;
            }
        }

        string GetNormalName(string str)
        {

            if (str[0] == 'г')
            {
                return str.Replace(" ", "");
            }
            else
            {
                return str.Replace(" р-н", "");
            }

        }

        string GetNormalDisplay(string str)
        {

            if (str[0] == 'г')
            {
                return str;
            }
            else
            {

                return str + " р-н";
            }

        }

        int ExjectRangValue(UltraGridCell cell)
        {
            UltraGridCell RangCell = GetNextCell(GetNextCell(GetNextCell(cell)));
            if (RangCell.Value != null)
            {
                return int.Parse(RangCell.Text);
                    //int.Parse(RangCell.Text.Split(':')[1]);
            }
            return -1;
        }

        protected void DataBindMap()
        {
            foreach (Shape sh in DundasMap.Shapes)
            {
                string indexActiveCol = RadioButton1.Checked ? "val1" : "val2";

                UltraGridRow ValueShape = GetValueFromNameRegion(Grid, sh.Name);

                if (ValueShape == null)
                {
                    continue;
                }

                sh["Value"] = (double)GetVal(indexActiveCol, ValueShape);

                if (sh.Name.Contains("г.") && (!sh.Name.Contains("_callout")))
                {
                    continue;
                }

                ShapeType TypeShape = GetShapeType(sh.Name);

                string TollTips = string.Format("{0} \n{1:### ##0.##}, рубль", ValueShape.Cells[0], sh["Value"]);
                sh.TextVisibility = TextVisibility.Shown;
                sh.Text = GenShortNameRegion(ValueShape.Cells[0].ToString()) + "\n" + string.Format("{0:### ### ##0.##}", sh["Value"]);
                try
                {
                    int Rang = ExjectRangValue(ValueShape.Cells.FromKey(indexActiveCol));
                    if (Rang > 0)
                    {
                        TollTips = string.Format("{0} \n{1:### ##0.##}, рубль \nРанг: {2} ", ValueShape.Cells[0], sh["Value"], Rang);
                    }
                    sh.ToolTip = TollTips;
                }
                catch
                { }
            }
        }

        #endregion

        private bool IsEmptyCol(string p)
        {
            foreach (UltraGridRow row in Grid.Rows)
            {
                if (row.Cells.FromKey(p).Value != null)
                {
                    return false;
                }
            }
            return true;
        }

        private void ChosenNoEmptyRadioButtons()
        {
            if (IsEmptyCol("val1"))
            {
                RadioButton2.Checked = true;
                RadioButton1.Checked = false;
            }
        }

        private object GetAnalogForHeader()
        {
            if (ComboComapraDate.SelectedValue.Contains("предыдущему"))
            {
                return "предыдущим периодом";
            }
            if (ComboComapraDate.SelectedValue.Contains("к аналогичному периоду предыдущего года"))
            {
                return "аналогичным периодом предыдущего года";
            }
            if (ComboComapraDate.SelectedValue.Contains("к началу года"))
            {
                return "началом года";
            }

            return "О, кто вы!??? как вы поплаи в мой код!?????";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            IsHmao.Value = RegionSettingsHelper.Instance.Name.Contains("Ямало") ? "--" : "";

            FillCombo(); 
            
            ChosenParam();

            GenerationGrid();

            if (!Page.IsPostBack)
            {
                ChosenNoEmptyRadioButtons();
            }

            SetHeaderReport();

            GenerateColChart();

            ConfigurationMap();
        }

        



        string ReplaceIA(string s)
        {
            return s.Remove(s.Length - 1) + "я";
        }

        string GetUnit()
        {
            return string.Format("рублей за {0}", SetYslyg.OtherInfo[ComboYslyga.SelectedValue]["EdIzm"]);
        }


        private void SetHeaderReport()
        {
            if (ComboYslyga.SelectedNode.Parent != null)
            {

                //      PageSubTitle.Text = string.Format("Анализ средних тарифов для населения с учётом надбавки, НДС и прочих выплат, применяемых при начислении платежа гражданину, на услуги {0} по всем видам благоустройства, {3}, за {2} год{1}",
                //ReplaceIA(ComboYslyga.SelectedNode.Parent.Text),
                //ComboCurDay.SelectedValue,
                //RegionSettingsHelper.Instance.Name);

                Hederglobal.Text = "Мониторинг средних тарифов на коммунальные услуги в разрезе муниципальных образований";
                Page.Title = Hederglobal.Text;

                Label4.Text = string.Format("Распределение территорий по уровню средних тарифов на {0} по виду благоустройства «{1}»",
                ComboYslyga.SelectedNode.Parent.Text,
                ComboYslyga.SelectedValue);

                Label1.Text = string.Format("Средний тариф на {0} по виду благоустройства «{1}», {2}", ComboYslyga.SelectedNode.Parent.Text, ComboYslyga.SelectedValue, GetUnit());
            }
            else
            {
                PageSubTitle.Text =
                    string.Format("Анализ средних тарифов для населения с учётом надбавки, НДС и прочих выплат, применяемых при начислении платежа гражданину, на услуги {0}, Ямало-Ненецкий автономный округ, по состоянию на {1} по сравнению с {2}",
                    ComboYslyga.SelectedValue.ToLower().Replace("горячее водоснабжение",
                    "горячего водоснобжения").Replace("ие", "ия"), ComboCurDay.SelectedValue.ToLower(),
                    GetAnalogForHeader());

                //string.Format("Анализ средних тарифов на услуги {0} по всем видам благоустройства,{1} {3}, за {2} год",
                //ReplaceIA(ComboYslyga.SelectedNode.Text),
                //,
                //ComboYslyga.SelectedValue,
                //"",
                //ComboCurDay.SelectedValue,
                //RegionSettingsHelper.Instance.Name);

                Hederglobal.Text = "Мониторинг средних тарифов на коммунальные услуги в разрезе муниципальных образований";
                Page.Title = Hederglobal.Text;

                Label4.Text = string.Format("Распределение территорий по уровню средних тарифов на {0}, {1}", ComboYslyga.SelectedNode.Text,
                    GetUnit());
                Label1.Text = string.Format("Средний тариф на {0}{1}, {2}", ComboYslyga.SelectedNode.Text, "", GetUnit());

            }

            Link1.Text = "Мониторинг средних тарифов на коммунальные<br>услуги муниципального образования";
            Link1.NavigateUrl = "../GKH_0004/default.aspx";

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
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);


            foreach (UltraGridRow r in HL.Grid.Rows)
            {
                try
                {
                    r.Cells[1].Text = r.Cells[1].Text.Split('|')[1];
                }
                catch { }
            }

            ReportExcelExporter1.Export(HL, sheet1, 3);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            ColChart.Width = 800;
            ReportExcelExporter1.Export(ColChart, Label4.Text, sheet2, 2);
            sheet2.Columns[0].Width = 34 * 1000;

            DundasMap.Width = 800;
            DundasMap.Height = 400;
            ReportExcelExporter1.Export(DundasMap, Label1.Text, sheet3, 2);
            sheet3.Columns[0].Width = 34 * 1000;
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

            e.CurrentWorksheet.Rows[1].Height = 24 * 50; 
        }



        #endregion

        #region Экспорт в PDF
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


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Page.Title;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ISection section2 = report.AddSection();

            ISection section3 = report.AddSection();


            foreach (UltraGridRow r in HL.Grid.Rows)
            {
                try
                {
                    //r.Cells[1].Text = r.Cells[1].Text.Split('|')[1];
                }
                catch { }
            }

            ReportPDFExporter1.HeaderCellHeight = 35;
            MergeCellsGridFormPDF(HL.Grid, 0);
            //MergeCellsGridFormPDF(HL.Grid, 1);

            foreach (UltraGridRow row in Grid.Rows)
            {
                MergeCellsGridFormPDFCol(HL.Grid, row);
            }

            ReportPDFExporter1.Export(HL, section1);
            ColChart.Width = 900;
            DundasMap.Width = 900;
            ReportPDFExporter1.Export(ColChart, Label1.Text, section2);
            ReportPDFExporter1.Export(DundasMap, Label4.Text, section3);
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

        void ColChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double RFStartLineX = 0;
            double RFStartLineY = 0;

            double RFEndLineX = 0;
            double RFEndLineY = 0;
            string RFheader = "";
            
            HMAOVALCHART = RadioButton1.Checked ? CharrtVal1 : CharrtVal2;
            if (HMAOVALCHART < 0)
            {
                return;
            }
            if (HMAOVALCHART > 0)
            {
                RFheader = string.Format("{0}: {1:N2} {2}", RegionSettingsHelper.Instance.Name, HMAOVALCHART, "рубль");
                RFStartLineX = xAxis.Map(xAxis.Minimum);
                RFStartLineY = yAxis.Map(HMAOVALCHART);

                RFEndLineX = xAxis.Map(xAxis.Maximum);
                RFEndLineY = yAxis.Map(HMAOVALCHART);


            }

            GenHorizontalLineAndLabel((int)RFStartLineX, (int)RFStartLineY, (int)RFEndLineX, (int)RFEndLineY,
                    Color.Blue, RFheader, e, true);


        }


        #endregion

        public EndExportEventHandler ExcelExporter_EndExport { get; set; }
        void SaveTableToErrorLog(DataTable Table)
        {

            string saved = "";

            foreach (DataColumn col in Table.Columns)
            {
                saved += "|     " + col.ColumnName;
            }
            String.Format(saved);

            foreach (DataRow Row in Table.Rows)
            {
                saved = "";
                foreach (DataColumn col in Table.Columns)
                {
                    saved += "|     " + Row[col].ToString();
                }
                String.Format(saved);
            }

        }

        void SaveTableToErrorLog(UltraWebGrid grid)
        {

            string saved = "";

            foreach (UltraGridColumn col in grid.Columns)
            {
                saved += "|     " + col.BaseColumnName;
            }
            String.Format(saved);

            foreach (UltraGridRow Row in grid.Rows)
            {
                saved = "";
                foreach (UltraGridColumn col in grid.Columns)
                {

                    if (Row.Cells.FromKey(col.BaseColumnName).Value == null)
                    {
                        saved += "|     null";
                    }
                    else
                    {
                        saved += "|     " + Row.Cells.FromKey(col.BaseColumnName).Value.ToString();
                    }
                }
                String.Format(saved);
            }

        }


    }




}