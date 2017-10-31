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


namespace Krista.FM.Server.Dashboards.ORG_0001_0009
{
    public partial class _default : CustomReportPage
    {
        IDataSetCombo SetCurDay;
        IDataSetCombo SetRegion;

        Day CurDay;
        Day PrevDay;
        Day FyDay;

        ICustomizerSize CustomizerSize;

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
        CustomParam ChosenCurGridPeriod { get { return (UserParams.CustomParam("ChosenCurGridPeriod")); } }

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
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
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
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
                    default:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 100).Value;
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
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 6) / (13);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 5) / (12);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1200 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 5) / (12);
                    }
                }

                Grid.Columns[0].Width = onePercent * 17;
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
            static DataProvider ActiveDataPorvider = DataProvidersFactory.SecondaryMASDataProvider;

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
                    string m = Row["Mounth"].ToString();
                    string y = Row["year"].ToString();
                    string uname = Row["period_uname"].ToString();

                    if (y != LYear)
                    {
                        this.AddItem(y + " год", 0, "");
                        LYear = y;
                    }

                    m = GetAlowableAndFormatedKey(m + " " + y + " года", "").ToLower();

                    this.AddItem(m, 1, uname);
                    this.addOtherInfo(Table, Row, m);
                }


            }

        }
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExportButton.Visible = false;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            UltraGridExporter1.PdfExportButton.Visible = false;

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
            Grid.Height = 500;

            ChartLine.Width = CustomizerSize.GetChartWidth();

            ComboCurDay.Width = 250;
            ComboRegion.Width = 420;


        }

        private void FillComboPeriodCur()
        {

            ComboCurDay.ParentSelect = false;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboMounth", "Mounth");

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
                ComboRegion.FillDictionaryValues(SetRegion.DataForCombo);
                String FoName = RegionSettings.Instance.Name;
                ComboRegion.SetСheckedState(FoName, true);
            }
        }

        private void FillComboTypePeriiod()
        {
            if (!Page.IsPostBack)
            {
                Dictionary<string, int> Dict = new System.Collections.Generic.Dictionary<string, int>();
                Dict.Add("к началу года", 0);
                Dict.Add("к предыдущему периоду", 0);
                Dict.Add("к аналогичному периоду предыдущего года", 0);
                ComboTypeCompare.FillDictionaryValues(Dict);
                ComboTypeCompare.Width = 300;
                ComboTypeCompare.Title = "Период для сравнения";

                ComboTypeCompare.SetСheckedState("к предыдущему периоду", true);
            }
        }

        bool IsPrevPeriod()
        {
            return ComboTypeCompare.SelectedValue == "к предыдущему периоду";
        }

        bool IsAnalogPrevYearPeiod()
        {
            return ComboTypeCompare.SelectedValue == "к аналогичному периоду предыдущего года";
        }

        bool IsFirstYearPeriod()
        {
            return ComboTypeCompare.SelectedValue == "к началу года";
        }

        private void FillCombo()
        {
            FillComboPeriodCur();
            FillComboRegion();
            FillComboTypePeriiod();
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

        private string GetGridPeriodString()
        {
            Stack<string> Uname = new System.Collections.Generic.Stack<string>();

            foreach (string Key in SetCurDay.DataForCombo.Keys)
            {
                if (Uname.Count > 12)
                {
                }
                if (SetCurDay.DataForCombo[Key] != 0)
                    if (Key != ComboCurDay.SelectedValue)
                    {
                        Uname.Push(SetCurDay.DataUniqeName[Key]);
                    }
                    else
                    {
                        Uname.Push(SetCurDay.DataUniqeName[Key]);
                        break;
                    }
            }

            string result = "";
            bool first = true;

            for (int i = 0; i < 14; i++)
            {
                try
                {
                    string unameperiod = Uname.Pop();
                    if (first)
                    {
                        result = unameperiod;
                        first = false;
                    }
                    else
                    {
                        result = unameperiod + ", " + result;
                    }
                }
                catch { }
            }
            return result;
        }

        string FormatgridHeader(string mounth, string year)
        {
            return mounth + " " + year + " года";
        }

        private Stack<string> GetGridHeader()
        {
            Stack<string> Uname = new System.Collections.Generic.Stack<string>();

            foreach (string Key in SetCurDay.DataForCombo.Keys)
            {
                if (Uname.Count > 13)
                {
                }
                if (SetCurDay.DataForCombo[Key] != 0)
                    if (Key != ComboCurDay.SelectedValue)
                    {
                        Uname.Push(FormatgridHeader(SetCurDay.OtherInfo[Key]["Mounth"], SetCurDay.OtherInfo[Key]["year"]));
                    }
                    else
                    {
                        Uname.Push(FormatgridHeader(SetCurDay.OtherInfo[Key]["Mounth"], SetCurDay.OtherInfo[Key]["year"]));
                        break;
                    }
                if (Uname.Count > 11)
                {
                }
            }

            Stack<string> s2 = new System.Collections.Generic.Stack<string>();
            for (int i = 0; i < 14; i++)
            {
                try
                {
                    s2.Push(Uname.Pop());
                }
                catch { }
            }

            return s2;
        }

        private void ChosenParam()
        {
            if (!Page.IsPostBack)
            {
                SelectDouwnLevel(ComboCurDay.SelectedNode, false, ComboCurDay);
            }

            ChosenCurPeriod.Value = SetCurDay.DataUniqeName[ComboCurDay.SelectedValue].ToString();

            ChosenCurGridPeriod.Value = GetGridPeriodString();

            ChosenRegionChart.Value = SetRegion.DataUniqeName["Российская  Федерация"] + ".datamember";

            if (ComboRegion.SelectedNode.Nodes.Count > 0)
            {
                ChosenRegionGrid.Value = SetRegion.DataUniqeName[ComboRegion.SelectedValue] + ".datamember";
                if (ComboRegion.SelectedNode.Level == 1)
                {
                    ChosenRegionChart.Value += "," + ChosenRegionGrid.Value + ".datamember";
                }
            }
            else
            {
                ChosenRegionGrid.Value = SetRegion.DataUniqeName[ComboRegion.SelectedValue];
                ChosenRegionChart.Value += "," + SetRegion.DataUniqeName[ComboRegion.SelectedNode.Parent.Text] + ".datamember";
                ChosenRegionChart.Value += "," + SetRegion.DataUniqeName[ComboRegion.SelectedNode.Text];
            }


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
            if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
            {
                return (decimal)CurValue - (decimal)PrevValue;
            }
            return DBNull.Value;
        }

        private object GetSpeedDeviation(object CurValue, object PrevValue)
        {
            if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
            {
                return ((decimal)CurValue / (decimal)PrevValue) - 1;
            }
            return DBNull.Value;
        }

        private DataRow GetRowFromField(string p, DataTable CompareTable)
        {
            foreach (DataRow Row in CompareTable.Rows)
            {
                if (Row["Field"].ToString() == p)
                {
                    return Row;
                }
            }
            return null;
        }

        string GetSelectYear()
        {
            return ComboCurDay.SelectedNode.Parent.Text.Replace(" год", "");
        }

        string SetPrevYear(string s)
        {
            string SelectYear = GetSelectYear();
            string CompareYear = (int.Parse(SelectYear) - 1).ToString();
            return s.Replace(SelectYear, CompareYear);
        }

        private void SetCompareCustoParam()
        {
            if (IsAnalogPrevYearPeiod())
            {
                string SelectYear = GetSelectYear();
                string CompareYear = (int.Parse(SelectYear) - 1).ToString();

                ChosenCurGridPeriod.Value = ChosenCurGridPeriod.Value.Replace((int.Parse(SelectYear) - 2).ToString(), (int.Parse(SelectYear) - 3).ToString());
                ChosenCurGridPeriod.Value = ChosenCurGridPeriod.Value.Replace((int.Parse(SelectYear) - 1).ToString(), (int.Parse(SelectYear) - 2).ToString());
                ChosenCurGridPeriod.Value = ChosenCurGridPeriod.Value.Replace(SelectYear, CompareYear);



            }
            else
            {
                ChosenCurGridPeriod.Value =
                    SetCurDay.OtherInfo[ComboCurDay.SelectedNode.Parent.Nodes[0].Text]["period_uname"];
            }
        }

        private void DataBindGrid()
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Field");

            DataTable CompareTable = new DataTable();

            if (!IsPrevPeriod())
            {
                SetCompareCustoParam();
                CompareTable = DataBaseHelper.ExecQueryByID("Grid", "Field");
            }


            CoolTable GridTable = new CoolTable();

            Stack<string> ColName = GetGridHeader();

            GridTable.Columns.Add("Field");

            for (; ColName.Count > 0; )
            {
                GridTable.Columns.Add(ColName.Pop(), typeof(decimal));
            }

            foreach (DataRow row in BaseTable.Rows)
            {
                DataRow VRow = GridTable.AddRow();
                DataRow DRow = GridTable.AddRow();
                DataRow SDRow = GridTable.AddRow();

                DataRow CompareRow = null;

                if (!IsPrevPeriod())
                {
                    CompareRow = GetRowFromField(row["Field"].ToString(), CompareTable);
                }

                VRow["Field"] = row["Field"];

                object PrevValue = DBNull.Value;

                for (int i = 1; i < GridTable.Columns.Count; i++)
                {
                    if (!IsPrevPeriod())
                    {
                        PrevValue = IsAnalogPrevYearPeiod() ? CompareRow[i] : CompareRow[1];
                    }

                    DataColumn GridCol = GridTable.Columns[i];
                    DataColumn BaseCol = BaseTable.Columns[i];

                    VRow[GridCol] = row[BaseCol];

                    if (GridTable.Columns[i].ColumnName.Contains("Январь"))
                    {
                        if (IsFirstYearPeriod())
                        {
                            continue;
                        }
                    }

                    DRow[GridCol] = GetDeviation(VRow[GridCol], PrevValue);
                    SDRow[GridCol] = GetSpeedDeviation(VRow[GridCol], PrevValue);

                    PrevValue = VRow[GridCol];
                }
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

        private void ConfHeader()
        {

            Grid.Columns.FromKey("Field").Header.Caption = "Наименование лекарственного препарата";
            foreach (UltraGridColumn Col in Grid.Columns)
            {
                if (!Col.BaseColumnName.Contains("Field"))
                {
                    if (!Col.Header.Caption.Contains("<br>"))
                    {
                        Col.Header.Caption = Col.Header.Caption.Replace("20", "<br>20");
                    }
                    SetDefaultStyleHeader(Col.Header);
                    Col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    Col.CellStyle.Padding.Right = 5;
                }
            }
            Grid.Columns[0].Header.Style.Wrap = true;
        }

        private UltraGridCell GetNextCell(UltraGridCell Cell)
        {
            return Cell.Row.NextRow.Cells.FromKey(Cell.Column.BaseColumnName);
        }

        private UltraGridCell GetPrevCell(UltraGridCell Cell)
        {
            return Cell.Row.PrevRow.Cells.FromKey(Cell.Column.BaseColumnName);
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            Cell.Style.BackgroundImage = ImagePath;
        }

        private void SetMaxRang(UltraGridCell ValueCell, bool Max, bool ReverceRang)
        {
            UltraGridCell RangCell = GetNextCell(ValueCell);
            if (RangCell.Value != null)
            {
                RangCell.Title = !ReverceRang ?
                    string.Format("Ранг по СФО на {1}: {0}\nСамое высокое значение показателя", RangCell.Value, RangCell.Column.Header.Caption) :
                    string.Format("Ранг по СФО на {1}: {0}\nСамое низкое значение показателя", RangCell.Value, RangCell.Column.Header.Caption);
                SetImageFromCell(RangCell, "starGrayBB.png");
            }
        }

        private void SetMinRang(UltraGridCell ValueCell, bool Max, bool ReverceRang)
        {
            UltraGridCell RangCell = GetNextCell(ValueCell);
            if (RangCell.Value != null)
            {
                RangCell.Title = ReverceRang ?
                    string.Format("Ранг по СФО на {1}: {0}\nСамое высокое значение показателя", RangCell.Value, RangCell.Column.Header.Caption) :
                    string.Format("Ранг по СФО на {1}: {0}\nСамое низкое значение показателя", RangCell.Value, RangCell.Column.Header.Caption);
                SetImageFromCell(RangCell, "starYellowBB.png");
                //:"starGrayBB.png");!ReverceRang 
            }
        }

        void SetFormatCell(UltraGridCell Cell, string format)
        {
            if (Cell.Value != null)
            {
                Cell.Text = string.Format("{0:" + format + "}", decimal.Parse(Cell.Text));
            }
        }

        decimal GetDecFromCell(UltraGridCell cell)
        {
            return decimal.Parse(cell.Text.ToString().Replace("%", ""));
        }

        void SetFormatCell1(UltraGridCell Cell, string format)
        {

        }
        UltraGridCell MaxSD = null;
        UltraGridCell MinSD = null;

        void SetMDCell(UltraGridCell cell)
        {
            if (MaxSD == null)
            {
                if (cell.Value != null)
                    MaxSD = cell;
            }
            if (MinSD == null)
            {
                if (cell.Value != null)
                    MinSD = cell;
            }

            try
            {
                if (GetDecFromCell(cell) > GetDecFromCell(MaxSD))
                {
                    MaxSD = cell;
                }
                if (GetDecFromCell(cell) < GetDecFromCell(MinSD))
                {
                    MinSD = cell;
                }
            }
            catch { }
        }

        private void SetDeviation(int IndexCol, UltraGridCell IndicatorCell, string TitleCaption)
        {
            string MounthName = Grid.Columns[IndexCol - 1].Header.Caption.Split(' ')[0];

            if (IsAnalogPrevYearPeiod())
            {
                MounthName = Grid.Columns[IndexCol].Header.Caption.Split(' ')[0];
            }
            if (IsFirstYearPeriod())
            {
                MounthName = ComboCurDay.SelectedNode.Parent.Nodes[0].Text.Split(' ')[0];
            }

            string year = Grid.Columns[IndexCol - (IsPrevPeriod() ? 1 : 0)].Header.Caption.Split(' ')[1].Replace("<br>", "");

            if (IsAnalogPrevYearPeiod())
            {
                year = SetPrevYear(year);
            }

            IndicatorCell.Title = string.Format("{0} к {1}", TitleCaption,
                CRHelper.RusMonthDat(CRHelper.MonthNum(MounthName)) + " " + year + " года");
        }

        private void SetTitleGridCell(UltraGridCell IndicatorCell, int IndexCol, decimal Value)
        {
            string TitleCaption = Value > 0 ? "прироста" : "снижения";

            string MounthName = Grid.Columns[IndexCol - 1].Header.Caption.Split(' ')[0];

            if (IsAnalogPrevYearPeiod())
            {
                MounthName = Grid.Columns[IndexCol].Header.Caption.Split(' ')[0];
            }
            if (IsFirstYearPeriod())
            {
                MounthName = ComboCurDay.SelectedNode.Parent.Nodes[0].Text.Split(' ')[0];
            }

            string year = Grid.Columns[IndexCol - (IsPrevPeriod() ? 1 : 0)].Header.Caption.Split(' ')[1].Replace("<br>", "");

            if (IsAnalogPrevYearPeiod())
            {
                year = SetPrevYear(year);
            }

            IndicatorCell.Title = string.Format("Темп {0} к {1}", TitleCaption,
                CRHelper.RusMonthDat(CRHelper.MonthNum(MounthName)) + " " + year + " года");
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

                    SetTitleGridCell(IndicatorCell, IndexCol, Value);

                    string Color = "";
                    if ((Value > 0))
                    {
                        Color = "Red";
                    }

                    if ((Value < 0))
                    {
                        Color = "Green";
                    }
                    if (!string.IsNullOrEmpty(Color))
                    {
                        SetImageFromCell(GetPrevCell(GetPrevCell(IndicatorCell)), "arrow" + Color + UpOrdouwn + "BB.png");
                    }
                }
            }
            SetMDCell(IndicatorCell);
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
                        string TitleCaption = Value > 0 ? "Прирост" : "Снижение";

                        SetDeviation(IndexCol, IndicatorCell, TitleCaption);

                    }
                }
            }
            catch { }
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

            SetFormatCell1(GetNextCell(GetNextCell(ValueCell)), "P2");

            SetIndicatorDeviationcell(ValueCell, false);
            SetIndicatorSpeedDeviationcell(ValueCell, false);
        }

        private void SetImageFromMaxMinSD()
        {
            try
            {

                foreach (UltraGridCell cell in MinSD.Row.Cells)
                {
                    if (cell.Text == MinSD.Text)
                    {
                        //cell.Title = "Самый высокий уровень тарифа";
                        //cell.Style.BackgroundImage = "~/images/starYellowBB.png";
                        //cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                foreach (UltraGridCell cell in MaxSD.Row.Cells)
                {
                    if (cell.Text == MaxSD.Text)
                    {
                        //cell.Title = "Самый низкий уровень тарифа";
                        //cell.Style.BackgroundImage = "~/images/starGrayBB.png";
                        //cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }
            catch { }
        }

        private void FormatRow()
        {

            foreach (UltraGridRow Row in Grid.Rows)
            {

                if (Row.Cells.FromKey("Field").Value != null)
                {
                    MaxSD = null;
                    MinSD = null;
                    foreach (UltraGridCell Cell in Row.Cells)
                    {
                        if (Cell.Column.BaseColumnName != "Field")
                        {
                            FormatValueCell(Cell);
                        }
                        else
                        {
                            try
                            {
                                Cell.RowSpan = 3;
                                Cell.Style.Wrap = true;
                                string basename = Cell.Text;
                                Cell.Value += ", " + DictEdizm[Cell.Text].ToLower(); ;
                                DictEdizm_re.Add(Cell.Text, basename);
                            }
                            catch { }
                        }
                    }
                    SetImageFromMaxMinSD();
                }
            }
        }

        private void CustomizeGrid()
        {

            FormatRow();
            if (Grid.Columns.Count > 14)
            {
                Grid.Columns.Remove(Grid.Columns[1]);
            }
            ConfHeader();
            CustomizerSize.ContfigurationGrid(Grid);
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }

        }

        private void GenerationGrid()
        {
            DataBindGrid();

            CustomizeGrid();

            Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);

            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
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
            GenChart();

        }

        DataTable TableChart = null;
        private void GenChart()
        {
            ChartLine.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(ChartLine_InvalidDataReceived);

            ChosenCurGridPeriod.Value = GetGridPeriodString();

            TableChart = DataBaseHelper.ExecQueryByID("ChartLine", "del");
            TableChart.Columns.Remove("del");

            
            if ((TableChart.Rows.Count > 0)&&(TableChart.Rows.Count>13))
                TableChart.Rows[0].Delete();

            foreach (DataColumn col in TableChart.Columns)
            {
                col.ColumnName = col.ColumnName.Replace("(", "").Replace("ДАННЫЕ)", "");
            }

            ChartLine.DataSource = TableChart;
            ChartLine.DataBind();

            {
                ChartLine.Data.SwapRowsAndColumns = true;
                ChartLine.ChartType = ChartType.LineChart;
                ChartLine.Data.SwapRowsAndColumns = true;
                ChartLine.Legend.FormatString = "<ITEM_LABEL>                .";
                ChartLine.Legend.Location = LegendLocation.Bottom;
                ChartLine.Legend.SpanPercentage = CustomizerSize is CustomizerSize_800x600 ? 20 : 10;
                ChartLine.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
                ChartLine.Axis.X.Margin.Far.Value = 3;
                ChartLine.Axis.X.Margin.Near.Value = 3;
                ChartLine.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
                ChartLine.Axis.Y.Extent = 60;
                ChartLine.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                ChartLine.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ChartLine_FillSceneGraph);
                ChartLine.Tooltips.FormatString = string.Format("<ITEM_LABEL> <br><b><DATA_VALUE:N2></b> рублей за {0}", DictEdizm[SelectField.Value].ToLower().ToLower().Replace("штука","штуку"));
                ChartLine.LineChart.LineAppearances.Clear();
                ChartLine.LineChart.Thickness = 10;
            }

            ChartLine.ColorModel.Skin.PEs.Clear();
            ChartLine.ColorModel.ModelStyle = ColorModels.CustomSkin; 
            ChartLine.ColorModel.Skin.ApplyRowWise = true;
            {
                PaintElement pe = new PaintElement();
                pe.Fill = Color.Red;

                ChartLine.ColorModel.Skin.PEs.Add(pe);

                LineAppearance lineAppearance = new LineAppearance();

                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.IconAppearance.PE = pe;

                ChartLine.LineChart.LineAppearances.Add(lineAppearance);

                pe = new PaintElement();
                pe.Fill = Color.Blue;

                ChartLine.ColorModel.Skin.PEs.Add(pe);

                lineAppearance = new LineAppearance();

                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.IconAppearance.PE = pe;

                ChartLine.LineChart.LineAppearances.Add(lineAppearance);

                pe = new PaintElement();
                pe.Fill = Color.Green;

                ChartLine.ColorModel.Skin.PEs.Add(pe);

                lineAppearance = new LineAppearance();

                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.IconAppearance.PE = pe;

                ChartLine.LineChart.LineAppearances.Add(lineAppearance);
            }
            ChartLine.BackColor = Color.White;

        }

        void ChartLine_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        void ChartLine_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

            List<Primitive> delp = new System.Collections.Generic.List<Primitive>();
            int i = 0;
            int priCount = 0;
            foreach (Primitive p in e.SceneGraph)
            {
                if ((p is Text) && (p.Path != null) && (p.Path.ToLower().Contains("legend")))
                {
                    i++;
                    Text t = (Text)p;
                    try
                    {
                        t.SetTextString(TableChart.Columns[i].ColumnName);
                    }
                    catch
                    {
                        delp.Add(p);
                        delp.Add(e.SceneGraph[priCount - 1]);
                    }
                }
                priCount++;
            }

            foreach (Primitive p in delp)
            {
                try
                {
                    e.SceneGraph.Remove(p);
                }
                catch { }
            }
        }

        Dictionary<string, string> DictEdizm = new System.Collections.Generic.Dictionary<string, string>();
        Dictionary<string, string> DictEdizm_re = new System.Collections.Generic.Dictionary<string, string>();
        private void FillDictEdIzm()
        {
            DataTable table = DataBaseHelper.ExecQueryByID("DictEdizm", "field");
            foreach (DataRow Row in table.Rows)
            {
                DictEdizm.Add(Row[0].ToString(), Row[1].ToString());
                //DictEdizm_re.Add(Row[1].ToString(), Row[0].ToString());
            }

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            FillCombo();
            ChosenParam();
            FillDictEdIzm();
            GenerationGrid();
            if (!Page.IsPostBack)
            {
                ActiveRow(Grid.Rows[0]);
            }
            else
            {
                foreach (UltraGridRow row in Grid.Rows)
                {
                    if (SelectLastRow.Value == row.Cells[0].Text)
                    {
                        ActiveRow(row);
                    }
                }
            }

            SetHeaderReport();
            GenChart();

            GenerationTextovka();

             
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

        string GetTextovkoAnalogPeriod(string text)
        {
            if (text == "к началу года")
            {
                return "началом года";
            }

            if (text.Contains("к предыдущем"))
            {
                return "предыдущим периодом";
            }

            return "аналогичным периодам прошлого года";
        }

        private void GenerationTextovka()
        {
            Textovka.Text = string.Format(
                ComboTypeCompare.SelectedValue.Contains("аналогичному") ?
                "По состоянию на <b>{0}</b> наблюдалось изменение средних розничных цен на лекарственные препараты по сравнению с аналогичным периодом прошлого года:" :
                "По состоянию на <b>{0}</b> наблюдалось изменение средних розничных цен на лекарственные препараты по сравнению с {1}:",
                ComboCurDay.SelectedValue.ToLower(),
                GetTextovkoAnalogPeriod(ComboTypeCompare.SelectedValue));
            string TopText = "";
            try
            {
                TopText = getbig2();
                if (!string.IsNullOrEmpty(TopText))
                {
                    Textovka.Text += string.Format("<br> - увеличение цен более чем на 2%: {0}", TopText);
                }
            }
            catch { }
            string BottomText = "";
            try
            {

                BottomText = getsmal2();
                if (!string.IsNullOrEmpty(BottomText))
                {

                    if (TopText != "")
                        Textovka.Text += ";";

                    Textovka.Text += string.Format("<br> - снижение цен более чем на 2%: {0}", BottomText);
                }
            }
            catch { }

            if ((BottomText == "") && (TopText == ""))
            {
                Textovka.Text = "";
            }
            else
            {
                Textovka.Text += ".";
            }

            tabletext.Visible = !string.IsNullOrEmpty(Textovka.Text);




        }

        private void SetHeaderReport()
        {
            Label4.Text = string.Format("Динамика средней розничных цены на лекарственный препарат «{0}, {1}», рубль",
                SelectField.Value,
                DictEdizm[SelectField.Value].ToLower());

            PageSubTitle.Text = string.Format("Ежемесячный мониторинг средних розничных цен на лекарственные препараты, {1}, по состоянию на {0}.",
                ComboCurDay.SelectedValue.ToLower(), ComboRegion.SelectedValue);

            Hederglobal.Text = "Анализ средних розничных цен на лекарственные препараты";
            Page.Title = Hederglobal.Text;
        }

        void ChartSZ_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Text t = new Text(
                new Point(12, 100),
                "Количество МО");
            t.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            t.labelStyle.Font = new Font("Verdana", 11);
            e.SceneGraph.Add(t);
        }

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 12 * 20;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].CellFormat.Font.Height = 11 * 20;

            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].CellFormat.Font.Name = "Verdana";
            e.CurrentWorksheet.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Height = 30 * 20;
            e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 5);

            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";

        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            ExportGridToExcel(Grid, e.CurrentWorksheet.Workbook.Worksheets["Таблица"], 4, true);

            e.CurrentWorksheet.Workbook.Worksheets["Таблица"].Rows[3].Cells[0].CellFormat.Font.Name
                =
                e.CurrentWorksheet.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Font.Name;

            e.CurrentWorksheet.Workbook.Worksheets["Таблица"].Rows[3].Cells[0].CellFormat.Font.Height
                =
                e.CurrentWorksheet.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Font.Height;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            foreach (UltraGridRow Row in Grid.Rows)
            {
                for (int i = 0; i < Row.Cells.Count; i++)
                {
                    if (Row.Cells[i].Value == null)
                    {
                        Row.Cells[i].Value = "-";
                    }
                }
            }

            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.Header.Caption = col.Header.Caption;
            }

            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 6;
            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";

            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.Header.Caption = col.Header.Caption.Replace("<br>", "").Replace("<br>", "");
            }

            ReportExcelExporter1.ExcelExporter.Export(Grid, sheet1);
        }
        #region Експор Грида
        protected void SetStyleHeadertableFromExcel(IWorksheetCellFormat CellFormat)
        {
            CellFormat.WrapText = ExcelDefaultableBoolean.True;
            CellFormat.FillPatternBackgroundColor = System.Drawing.Color.FromArgb(200, 200, 200);
            CellFormat.FillPatternForegroundColor = System.Drawing.Color.FromArgb(200, 200, 200);
            CellFormat.FillPattern = FillPatternStyle.Default;
            CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            CellFormat.Alignment = HorizontalCellAlignment.Center;

            CellFormat.FillPatternForegroundColor = Color.LightGray;
            CellFormat.FillPatternBackgroundColor = Color.LightGray;
            CellFormat.TopBorderColor = Color.Gray;
            CellFormat.BottomBorderColor = Color.Gray;
            CellFormat.LeftBorderColor = Color.Gray;
            CellFormat.RightBorderColor = Color.Gray;

            CellFormat.Font.Name = "Verdana";
            CellFormat.Font.Height = 11 * 20;
        }

        object FormatVal(object value)
        { return value; }

        Graphics graphics = Graphics.FromImage(new Bitmap(1000, 500));
        /// <summary>
        /// Получение высоты текста в области заданной ширины
        /// </summary>
        /// <param name="measuredString">текст</param>
        /// <param name="font">шрифт текста</param>
        /// <param name="rectangleWidth">ширина области</param>
        /// <returns>высота</returns>
        private int GetStringHeight(string measuredString, Font font, int rectangleWidth)
        {

            SizeF sizeF = graphics.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);

            return rect.Height;
        }

        int ExportHeaderGrid(HeadersCollection Headers, Worksheet WorkSheet, int StartRow)
        {
            int rowFirst = StartRow;

            int maxRow = 0;

            for (int i = 0; i < Headers.Count; i++)
            {
                int FirstAbscissaMearge = Headers[i].RowLayoutColumnInfo.OriginX;

                int FirstOrdinateMearge = Headers[i].RowLayoutColumnInfo.OriginY + StartRow;

                int LastAbscissaMearge = Headers[i].RowLayoutColumnInfo.SpanX + FirstAbscissaMearge - 1;

                int LastOrdinateMearge = Headers[i].RowLayoutColumnInfo.SpanY + FirstOrdinateMearge - 1;

                if (LastOrdinateMearge > maxRow)
                {
                    maxRow = LastOrdinateMearge;
                }

                try
                {
                    WorkSheet.MergedCellsRegions.Add(FirstOrdinateMearge, FirstAbscissaMearge, LastOrdinateMearge, LastAbscissaMearge);

                    SetStyleHeadertableFromExcel(WorkSheet.Rows[FirstOrdinateMearge].Cells[FirstAbscissaMearge].CellFormat);

                    WorkSheet.Rows[FirstOrdinateMearge].Cells[FirstAbscissaMearge].Value = FormatVal(Headers[i].Caption);

                }
                catch
                { }
            }

            maxRow++;

            for (int i = rowFirst; i < maxRow; i++)
            {
                WorkSheet.Rows[i].Height = 20 * 45;
            }

            return maxRow;

        }

        void ExportGridToExcel(UltraWebGrid G, Worksheet sheet, int startrow, bool RowZebra)
        {
            startrow = ExportHeaderGrid(G.Bands[0].HeaderLayout, sheet, startrow);
            for (int i = 0; i < G.Rows.Count; i++)
            {
                sheet.Rows[i + startrow].Height = 10 * 45;
                sheet.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                sheet.Rows[i + startrow].CellFormat.FormatString = "### ### ##0.00";
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    sheet.Rows[i + startrow].Cells[col.Index].CellFormat.Font.Name = "Verdana";
                    sheet.Rows[i + startrow].Cells[col.Index].CellFormat.Font.Height = 10 * 20;
                    sheet.Rows[i + startrow].Cells[col.Index].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                }

                if (G.Rows[i].Cells[0].RowSpan > 1)
                {
                    sheet.MergedCellsRegions.Add(startrow + i, 0, startrow + i + G.Rows[i].Cells[0].RowSpan - 1, 0);
                }
            }
            sheet.Columns[0].Width = 270 * 36;
            sheet.Rows[4].Height = 40 * 40;
            for (int i = 1; i < 20; i++)
            {
                sheet.Columns[i].Width = 90 * 50;
            }

            try
            {
                sheet.Workbook.Worksheets[1].Rows[0].Cells[0].Value = Label4.Text;
                sheet.Workbook.Worksheets[1].Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
                sheet.Workbook.Worksheets[1].Rows[0].Cells[0].CellFormat.Font.Height = 11 * 20;
                sheet.Workbook.Worksheets[1].Rows[0].Height = 13 * 30;
                ChartLine.Width = 800;
                ChartLine.Legend.SpanPercentage = 20;
                ReportExcelExporter.ChartExcelExport(sheet.Workbook.Worksheets[1].Rows[1].Cells[0], ChartLine);
            }
            catch { }

        }

        #endregion
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Page.Title;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            GridHeaderCell header;
            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.Header.Caption = col.Header.Caption.Replace("<br>", "").Replace("<br>", "");
            }

            foreach (UltraGridColumn col in Grid.Columns)
            {

                headerLayout.AddCell(col.Header.Caption.Replace("20", " 20"));
            }

            headerLayout.ApplyHeaderInfo();

            ReportPDFExporter1.HeaderCellHeight = 20;

            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[0].Value == null)
                {
                    Row.Cells[0].Value = Row.PrevRow.Cells[0].Value;
                    Row.PrevRow.Cells[0].Value = null;
                    Row.NextRow.Cells[0].Value = " ";

                    Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
                    Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                }
            }


            ReportPDFExporter1.Export(headerLayout, section1);

            ChartLine.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.85);
            ReportPDFExporter1.Export(ChartLine, Label4.Text, section2);

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
    }
}