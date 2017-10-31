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


namespace Krista.FM.Server.Dashboards.GKH_0002
{
    public partial class _default : CustomReportPage
    {
        IDataSetCombo SetCurDay;
        IDataSetCombo SetYslyg;

        IDataSetCombo SetRegionLevel;

        ICustomizerSize CustomizerSize;

        bool SelectCity;

        CustomParam IsHmao { get { return (UserParams.CustomParam("IsHmao")); } }

        CustomParam RegionBaseDimension
        { get { return (UserParams.CustomParam("RegionBaseDimension")); } }

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
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (100 / Grid.Columns.Count) * onePercent;
                }
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (100 / Grid.Columns.Count) * onePercent;
                }
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (100 / Grid.Columns.Count) * onePercent;
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
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)//1100; 
                    CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
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
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (98 / Grid.Columns.Count) * onePercent;
                }
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (99 / Grid.Columns.Count) * onePercent;
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
            //ComboCurDay.Width = IsSmall ? 430 : 280;
            //ComboYslyga.Width = IsSmall ? 430 : 300;
            //ComboComapraDate.Width = IsSmall ? 430 : 240;


            ComboCurDay.Width = IsSmall ? 550 : 280;
            ComboYslyga.Width = IsSmall ? 550 : 300;
            ComboComapraDate.Width = IsSmall ? 550 : 240;

            ComboCurDay.Title = "Выберите период";
            ComboCurDay.ParentSelect = false;

            ComboYslyga.Title = "Вид услуги";
            ComboYslyga.ParentSelect = true;

            ComboComapraDate.Title = "Выберите период для сравнения";

            Link1.Text = "Мониторинг тарифов и нормативов на коммунальные услуги<br>в разрезе организаций, предоставляющих услуги ЖКХ";


            if (IsSmall)
            {
                Link1.Text = "Мониторинг тарифов и нормативов на коммунальные услуги<br>в разрезе организаций, предоставляющих услуги ЖКХ";

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
            ConfCombo(true);
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
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

            Grid.Width = CustomizerSize.GetGridWidth();
            Grid.Height =CustomizerSize is CustomizerSize_800x600?  Unit.Empty: 530;
            
            ComboCurDay.Title = "Выберите период";

            
        }

        private void FillComboPeriodCur()
        {

            ComboCurDay.ParentSelect = false;

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
            ComboYslyga.ParentSelect = false;


            

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
            ChosenCurPeriod.Value = SetCurDay.DataUniqeName[ComboCurDay.SelectedNode.Text];
            ChosenYslyg.Value = SetYslyg.DataUniqeName[ComboYslyga.SelectedValue];
            ChosenBlago.Value = SetYslyg.OtherInfo[ComboYslyga.SelectedValue]["UniqueName_"];

            ChosenPrevPeriod.Value = GetCompareDate();

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

            GridRow["Org"] = ParsRow[0] + "|" + ParsRow[3];
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
                if ((SetRegionLevel.OtherInfo[CurRegionName]["LevelNumber"] == "0") &&
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
            try
            {
                BaseTable.Columns.Remove(CurMounth + "; по приборам учета; Тариф ");
                BaseTable.Columns.Remove(CurMounth + "; по приборам учета; Норматив ");
                BaseTable.Columns.Remove(PrevMounth + "; по приборам учета; Тариф ");
                BaseTable.Columns.Remove(PrevMounth + "; по приборам учета; Норматив ");
            }
            catch { }

            return BaseTable;
        }

        DataTable RemoveColIfContainCaptionName(DataTable Table, string caption)
        {
            List<DataColumn> RemoveCols = new List<DataColumn>();
            foreach (DataColumn col in Table.Columns)
            {
                if (col.ColumnName.Contains(caption))
                {
                    RemoveCols.Add(col);
                }
            }
            foreach (DataColumn col in RemoveCols)
            {
                Table.Columns.Remove(col);
            }
            return Table;
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

        List<string> SP = new List<string>();

        private void DataBindGrid()
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Field");

            if (BaseTable.Rows.Count == 0)
            {
                Grid.DataSource = null;
                Grid.DataBind();
            }
            
            BaseTable = FormatBaseTable(BaseTable);

            CoolTable GridTable = new CoolTable();

            GridTable.Columns.Add("Yslyga");
            GridTable.Columns.Add("Org");
            GridTable.Columns.Add("val1", typeof(decimal));
            GridTable.Columns.Add("val2", typeof(decimal));
            GridTable.Columns.Add("val3", typeof(decimal));

            string LastParentRegion = "";            

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                string[] ParsRow = BaseRow["Field"].ToString().Split(';');

                string ysl = ParsRow[1];
                string Reg = ParsRow[0];

                string parent = BaseRow[GetColFromContainsCaption(BaseTable, "ParentRegion")].ToString();

                if (parent.Contains("муниципальный район"))
                {
                    if (parent != LastParentRegion)
                    {

                        DataRow TitleMORow = GridTable.AddRow();
                        TitleMORow["Yslyga"] = parent;
                        LastParentRegion = parent;
                    }
                    SP.Add(Reg);
                }

                DataRow GridRow = GridTable.AddRow();
                DataRow GridRowD = GridTable.AddRow();
                DataRow GridRowSD = GridTable.AddRow();

                fillStrRow(GridRow, ParsRow);
                fillStrRow(GridRowD, ParsRow);
                fillStrRow(GridRowSD, ParsRow);

                GridRow["val1"] = BaseRow[2];
                GridRow["val2"] = BaseRow[3];
                GridRow["val3"] = BaseRow[4];

                int adededPrevColNumber = 3;

                GridRowD["val1"] = GetDeviation(BaseRow[2], BaseRow[2 + adededPrevColNumber]);
                GridRowD["val2"] = GetDeviation(BaseRow[3], BaseRow[3 + adededPrevColNumber]);
                //GridRowD["val3"] = GetDeviation(BaseRow[3], BaseRow[3 + adededPrevColNumber]);

                GridRowSD["val1"] = GetSpeedDeviation(BaseRow[2], BaseRow[2 + adededPrevColNumber]);
                GridRowSD["val2"] = GetSpeedDeviation(BaseRow[3], BaseRow[3 + adededPrevColNumber]);
                //GridRowSD["val3"] = GetSpeedDeviation(BaseRow[3], BaseRow[3 + adededPrevColNumber]);
            }

            Grid.DataSource = GridTable;
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

        string SetEdIzm(string str)
        {
            return string.Format("{0}, рублей за {1}",
                str,
                SetYslyg.OtherInfo[ComboYslyga.SelectedValue]["EdIzm"]);
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
            HL.AddCell("Организация");
            GridHeaderCell yearhe = HL.AddCell(string.Format("{0}", ComboCurDay.SelectedValue));
            yearhe.AddCell("При наличии приборов учета").AddCell(SetEdIzm("Тариф"));
            GridHeaderCell ghc = yearhe.AddCell("при отсутствии приборов учёта ");
            ghc.AddCell(SetEdIzm("Тариф"));
            ghc.AddCell(SetEdIzm("Норматив"));
            HL.ApplyHeaderInfo();
            foreach (ColumnHeader ch in Grid.Bands[0].HeaderLayout)
            {
                ch.Style.Wrap = true;
                ch.Style.HorizontalAlign = HorizontalAlign.Center;
            }
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val1"), "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val2"), "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("val3"), "N2");
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
                    cell.ColSpan = Grid.Columns.Count;
                    cell.Style.Font.Bold = true;
                }
                else
                {
                    cell.Style.Padding.Left = 0;
                    //if (!cell.Text.Contains("Город"))
                    {
                      //  if (!cell.Text.Contains("район"))
                        if (SP.Contains(cell.Text))
                        {
                            cell.Style.Padding.Left = 20;
                        }
                        else
                        {
                            cell.Style.Font.Bold = true;
                        }
                    }
                }
            }
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center";
            Cell.Style.BackgroundImage = ImagePath;
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

                    string TitleCaption = Value > 100 ? "прироста" : "снижения";

                    //int CurYear = int.Parse(ComboCurDay.SelectedValue);
                    string CurMounth = SetCurDay.OtherInfo[ComboCurDay.SelectedValue]["mounth"];
                    string PrevMounth = CRHelper.RusMonthDat(CRHelper.MonthNum(SetCurDay.OtherInfo[GetPrevNode(ComboCurDay.SelectedNode).Text]["mounth"]));
                    string PrevYear = SetCurDay.OtherInfo[GetPrevNode(ComboCurDay.SelectedNode).Text]["year"];

                    if (Value != 100)
                    {
                        IndicatorCell.Title = string.Format("Темп роста к {0} {1} года", (PrevMounth).ToString(), PrevYear);
                    }
                    else
                    {
                        IndicatorCell.Title = string.Format("Темп роста к {0} {1} года", (PrevMounth).ToString(), PrevYear);
                    }

                    TitleCaption = Value > 100 ? "Прирост" : "Снижение";
                    if (Value != 100)
                        GetNextCell(ValueCell).Title = 
                            string.Format("{0} к {1} {2} года", TitleCaption, (PrevMounth).ToString(), PrevYear);
                    else
                    {
                        GetNextCell(ValueCell).Title = string.Format("Абсолютное отклонение к {0} {1} года",(PrevMounth).ToString(), PrevYear);
                    }
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

                        //IndicatorCell.Title = string.Format("{0} к {1}", TitleCaption,
                        //    CRHelper.RusMonthDat(CRHelper.MonthNum(MounthName)) + " " + year + " года");

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
        private void FormatValueCell(UltraGridCell ValueCell)
        {
            FormatTopCell(ValueCell);
            FormatCenterCell(GetNextCell(ValueCell));
            FormatBottomCell(GetNextCell(GetNextCell(ValueCell)));

            if (ValueCell.Value == null)
            {
                return;
            }

            SetFormatCell(ValueCell, "N2");
            SetFormatCell(GetNextCell(ValueCell), "N2");
            SetFormatCell(GetNextCell(GetNextCell(ValueCell)), "P2");

            SetIndicatorDeviationcell(ValueCell, false);
            SetIndicatorSpeedDeviationcell(ValueCell, false);
        }
        private string ClearDataMember(string p)
        {
            return p.Replace("ДАННЫЕ)", "").Replace("(", "");
        }
        private void FormatRow()
        {
            MergeCellsGrid(Grid, 0);
            MergeCellsGrid(Grid, 1);

            foreach (UltraGridRow Row in Grid.Rows)
            {
                Row.Cells[0].Style.Padding.Left = int.Parse(SetRegionLevel.OtherInfo[Row.Cells[0].Text]["LevelNumber"]) * 15;

                Row.Cells[0].Text = ClearDataMember(Row.Cells[0].Text);

                if ((Grid.Rows[0].Cells[0].Text.Contains("Город") || (Grid.Rows[0].Cells[0].Text.Contains("район"))))
                {
                    //Row.Cells[0].Style.Font.Bold = true;
                }

                if ((Row.Cells.FromKey("Org").RowSpan == 3) ||
                    (Row.Cells.FromKey("Yslyga").Text.Contains("Город") & Row.Cells.FromKey("Yslyga").RowSpan == 3))
                {
                    
                        FormatValueCell(Row.Cells.FromKey("val1"));
                        FormatValueCell(Row.Cells.FromKey("val2"));
                        FormatValueCell(Row.Cells.FromKey("val3"));
                        Row.Cells.FromKey("Org").Value =
                            Row.Cells.FromKey("Org").Text =
                                Row.Cells.FromKey("Org").Text.Split('|')[1];
                }

            }
            FormatRowYsly(Grid, "Yslyga");
        }


        void CustomizerOther()
        {
            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;

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

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            IsHmao.Value = RegionSettingsHelper.Instance.Name.Contains("Ямало") ? "--" : "";

            FillCombo();

            ChosenParam();

            GenerationGrid();

            SetHeaderReport();
        }

        string ReplaceIA(string s)
        {
            return s.Remove(s.Length - 1) + "я";
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

        private void SetHeaderReport()
        {
            string text = @"Анализ тарифов и нормативов для населения с учётом надбавки, НДС и прочих выплат, применяемых при начислении платежа гражданину на услуги {0} по виду благоустройства «{1}», {3}, по состоянию на {2} по сравнению с {4}";

            PageSubTitle.Text = string.Format(
                //"Анализ тарифов и нормативов для населения с учётом надбавки, НДС и прочих выплат, применяемых при начислении платежа гражданину на услуги {0} по виду благоустройства «{1}», {3}, за {2} год",
            text,
            ReplaceIA(ComboYslyga.SelectedNode.Parent.Text).ToLower(),
            ComboYslyga.SelectedValue,
            ComboCurDay.SelectedValue.ToLower(),
            RegionSettingsHelper.Instance.Name,
            GetAnalogForHeader());

            Hederglobal.Text = " Мониторинг тарифов и нормативов на коммунальные услуги в разрезе муниципальных образований";
            Page.Title = Hederglobal.Text;


            Link1.NavigateUrl = "../GKH_0001/default.aspx";
            
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


        private void PdfExportButton_Click(object sender, EventArgs e)
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

            ReportPDFExporter1.HeaderCellHeight = 20;
            MergeCellsGridFormPDF(HL.Grid, 0);
            MergeCellsGridFormPDF(HL.Grid, 1);

            foreach (UltraGridRow row in Grid.Rows)
            {
                MergeCellsGridFormPDFCol(HL.Grid, row);
            }

            ReportPDFExporter1.Export(HL, section1);
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