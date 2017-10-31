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


namespace Krista.FM.Server.Dashboards.ORG_0001_0008
{
    public partial class _default : CustomReportPage
    {

        IDataSetCombo SetCurDay;
        IDataSetCombo SetPrevDay;

        Day CurDay;
        Day PrevDay;
        Day FyDay;

        ICustomizerSize CustomizerSize;

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }
        CustomParam ChosenFYPeriod { get { return (UserParams.CustomParam("ChosenFirstPeriod")); } }

        CustomParam ChosenCurPeriodSU { get { return (UserParams.CustomParam("ChosenCurPeriodSU")); } }
        CustomParam ChosenPrevPeriodSU { get { return (UserParams.CustomParam("ChosenPrevPeriodSU")); } }
        CustomParam ChosenFYPeriodSU { get { return (UserParams.CustomParam("ChosenFirstPeriodSU")); } }

        CustomParam SelectField { get { return (UserParams.CustomParam("SelectField")); } }

        CustomParam Group { get { return (UserParams.CustomParam("Group")); } }
        CustomParam Fields { get { return (UserParams.CustomParam("Fields")); } }

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
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 6) / (Grid.Columns.Count - 1);
                    }
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 5) / (Grid.Columns.Count - 1);
                    }
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header.Caption.Contains("Ранг"))
                    {
                        col.Width = onePercent * 5;
                    }
                    else
                    {
                        col.Width = onePercent * (100 - 21 - 5) / (Grid.Columns.Count - 1);
                    }
                }

                Grid.Columns[0].Width = onePercent * 18;
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
                string LastParent = "";

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
        class DataSetComboYearQuart : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LastYear = "";
                string LastMounth = "";

                List<Day> ld = new System.Collections.Generic.List<Day>();


                foreach (DataRow row in Table.Rows)
                {
                    Day day = new Day(row["UniqueName"].ToString());
                    ld.Add(day);
                }

                ld.Sort(new sortDay());

                foreach (Day day in ld.ToArray())
                {
                    if (LastYear != day.DisplayYear())
                    {
                        LastYear = day.DisplayYear();
                        this.AddItem(LastYear, 0, "noname");

                        LastMounth = day.DisplayMounth();
                        this.AddItem(LastMounth, 1, day.BaseCaptionSU);

                        this.AddItem(day.DisplayDay(), 2, day.BaseCaptionSU);
                    }
                    else
                    {
                        if (LastMounth != day.DisplayMounth())
                        {
                            LastMounth = day.DisplayMounth();
                            this.AddItem(LastMounth, 1, day.BaseCaptionSU);

                            this.AddItem(day.DisplayDay(), 2, day.BaseCaptionSU);
                        }
                        else
                        {
                            this.AddItem(day.DisplayDay(), 2, day.BaseCaptionSU);
                        }
                    }
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
            Grid.Height = Unit.Empty;

            ChartColumn.Width = CustomizerSize.GetChartWidth();
            ChartLine.Width = CustomizerSize.GetChartWidth();
            
            ComboCurDay.Width = 350;
            TypeField.Width = 400;


        }

        private void FillComboPeriodCur()
        {

            ComboCurDay.ParentSelect = false;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboPeriodDay", "name");

            SetCurDay = new DataSetComboYearQuart();
            SetCurDay.LoadData(Table);

            if (!Page.IsPostBack)
            {
                ComboCurDay.FillDictionaryValues(SetCurDay.DataForCombo);
                ComboCurDay.SetСheckedState(SetCurDay.LastAdededKey, 1 == 1);
            }
        }

        private void FillComboTypeField()
        {
            TypeField.Width = 300;
            TypeField.Visible = false;
            if (!Page.IsPostBack)
            {
                Dictionary<string, int> d = new System.Collections.Generic.Dictionary<string, int>();

                foreach (DataRow Row in DataBaseHelper.ExecQueryByID("GSM_USER_PARAM", "Param").Rows)
                {
                    d.Add(Row["Param"].ToString(), 0);
                }
                TypeField.FillDictionaryValues(d);
            }

        }

        private void FillCombo()
        {
            FillComboPeriodCur();
            FillComboTypeField();
        }

        string SelectCurDate()
        {
            return ComboCurDay.SelectedValue;
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

        private bool IsFirstNode()
        {
            Node node = ComboCurDay.SelectedNode;
            return (node.Parent.Index == 0) && (node.Index == 0);
        }

        string SelectPrevDate()
        {
            return GetPrevNode(ComboCurDay.SelectedNode).Text;
        }



        string SelectFyDate()
        {
            return ComboCurDay.SelectedNode.Parent.Parent.Nodes[0].Nodes[0].Text;            
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

        private void RecheckDownLevel()
        {
            if (ComboCurDay.SelectedNode.Nodes.Count > 0)
            {
                ComboCurDay.SetСheckedState(ComboCurDay.SelectedNode.Nodes[0].Text,true);
            }
        }

        private void ChosenParam() 
        {
            if (!Page.IsPostBack)
            {
                SelectDouwnLevel(ComboCurDay.SelectedNode, false, ComboCurDay);
            }

            RecheckDownLevel();

            CurDay = new Day(SetCurDay.DataUniqeName[SelectCurDate()]);
            if (IsFirstNode())
            {
                PrevDay = new Day(SetCurDay.DataUniqeName[SelectCurDate()]); 
                PrevDay.RemoveDay(7);
            }
            else
            {
                PrevDay = new Day(SetCurDay.DataUniqeName[SelectPrevDate()]);
            }

            FyDay = new Day(SetCurDay.DataUniqeName[SelectFyDate()]);

            ChosenCurPeriod.Value = CurDay.BaseCaptionFORF;
            ChosenPrevPeriod.Value = PrevDay.BaseCaptionFORF;

            ChosenCurPeriodSU.Value = CurDay.BaseCaptionSU;
            ChosenPrevPeriodSU.Value = PrevDay.BaseCaptionSU;

            ChosenFYPeriod.Value = FyDay.BaseCaptionFORF;
            ChosenFYPeriodSU.Value = FyDay.BaseCaptionSU;

            Fields.Value = TypeField.SelectedValue;


        }

        

        DataTable BaseTable = null;

        #region Grid

        DataRow FindRow(string colname, string value, DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                if (row[colname].ToString() == value)
                {
                    return row;
                }
            }
            return null;
        }

        DataRow CreateRow(DataTable Table)
        {
            DataRow Row = Table.NewRow();
            Table.Rows.Add(Row);
            return Row;
        }

        struct RegionValue
        {
            public string NameRegion;
            public decimal Value;
        }

        RegionValue NewRV(string R, decimal v)
        {
            RegionValue rv = new RegionValue();
            rv.NameRegion = R;
            rv.Value = v;
            return rv;
        }

        class SortRV : System.Collections.Generic.IComparer<RegionValue>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(RegionValue x, RegionValue y)
            {
                if (x.Value == y.Value)
                {
                    return 0;
                }
                if (x.Value > y.Value)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            #endregion
        }

        int GetRangFromRegion(string r, List<RegionValue> lRv)
        {
            int i = 1;
            foreach (RegionValue rv in lRv)
            {
                if (rv.NameRegion == r)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        DataTable ReportTable = null;


        object GetSumCol(DataColumn col)
        {
            DataTable Table = col.Table;

            Decimal sum = 0;
            int Colunt = 0;

            foreach (DataRow row in Table.Rows)
            {
                if (row[col] != DBNull.Value)
                {
                    sum += (decimal)row[col];
                    Colunt++;
                }
            }
            if (sum == 0)
                return DBNull.Value;
            return sum / Colunt;
        }


        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(DataRow x, DataRow y)
            {
                return Compare_(x, y);
            }

            public int Compare_(DataRow x, DataRow y)
            {
                
                decimal X = (decimal)x["CUR"];
                decimal Y = (decimal)y["CUR"];

                if (X > Y)
                {
                    return 1;
                }
                else
                {
                    if (X < Y)
                        return -1;
                    else
                        return 0;
                }


                
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
                if (row["CUR"] != DBNull.Value)
                    LR.Add(row);
            }

            LR.Sort(new SortDataRow());

            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);                
            }
            return TableSort;
        }

        class RankingField
        {
            class SortKeyVal : System.Collections.Generic.IComparer<KeyVal>
            {
                #region Члены IComparer<KeyVal>

                public int Compare(KeyVal x, KeyVal y)
                {
                    if (x.Val > y.Val)
                    {
                        return 1;
                    }
                    if (x.Val < y.Val)
                    {
                        return -1;
                    }
                    return 0;
                }

                #endregion
            }

            struct KeyVal
            {
                public string Key;
                public decimal Val;
            }

            List<KeyVal> Fields = new List<KeyVal>();

            public int Count
            {
                get { return Fields.Count; }
            }

            public void AddItem(string Key, decimal Val)
            {
                KeyVal NewFild = new KeyVal();
                NewFild.Key = Key;
                NewFild.Val = Val;
                Fields.Add(NewFild);
            }

            void ClearDoubleVal()
            {
                List<KeyVal> RemoveList = new List<KeyVal>();
                for (int i = 0; i < Fields.Count - 1; i++)
                {
                    for (int j = i + 1; j < Fields.Count; j++)
                    {
                        if (Fields[i].Key == Fields[j].Key)
                        {
                            //RemoveList.Add(Fields[j]);
                            Fields.Remove(Fields[j]);
                        }
                    }
                }

                foreach (KeyVal kv in RemoveList)
                {
                    Fields.Remove(kv);
                }
            }

            public object GetRang(string Key)
            {
                ClearDoubleVal();
                Fields.Sort(new SortKeyVal());

                int i = 0;
                foreach (KeyVal kv in Fields)
                {
                    i++;

                    if (kv.Key.Split(';')[0] == Key.Split(';')[0])
                    {
                        return i;
                    }
                }
                return DBNull.Value;
            }

        }
        RankingField RF = new RankingField();
        private void DataBindGrid()
        {

            DataTable SUTABLE = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("TableSU"), "Field", SUTABLE);

            ReportTable = new DataTable();

            ReportTable.Columns.Add("Field");
            ReportTable.Columns.Add("CUR", typeof(decimal));
            ReportTable.Columns.Add("PREV", typeof(decimal));
            ReportTable.Columns.Add("FY", typeof(decimal));
            //ReportTable.Columns.Add("RANG");

            ReportTable.Columns.Add("PREVDeviation", typeof(decimal));
            ReportTable.Columns.Add("PREVSpeedDeviation", typeof(decimal));

            ReportTable.Columns.Add("FYDeviation", typeof(decimal));
            ReportTable.Columns.Add("FYSpeedDeviation", typeof(decimal));

            DataRow RowHMAO = ReportTable.NewRow();
            ReportTable.Rows.Add(RowHMAO);

            RowHMAO["Field"] = "Cредняя цена";
            RowHMAO["FY"] = GetSumCol(SUTABLE.Columns["FY"]);
            if (!IsFirstNode())
                RowHMAO["PREV"] = GetSumCol(SUTABLE.Columns["PREV"]);
            RowHMAO["CUR"] = GetSumCol(SUTABLE.Columns["CUR"]);


            List<RegionValue> RVal = new System.Collections.Generic.List<RegionValue>();

            //RankingField RF = new RankingField();


            foreach (DataRow Row in SUTABLE.Rows)
            {

                if (Row["CUR"] != DBNull.Value)
                {
                    RF.AddItem(Row["CUR"].ToString(), (decimal)Row["CUR"]);
                }


                DataRow SURow = CreateRow(ReportTable);
                foreach (DataColumn col in SUTABLE.Columns)
                {
                    if ((col.ColumnName == "PREV") && IsFirstNode())
                        continue;

                    SURow[col.ColumnName] = Row[col];
                }
               
            }

            RVal.Sort(new SortRV());
            bool FirstRow = true;
            foreach (DataRow Row in ReportTable.Rows)
            {
                if (Row["CUR"] != DBNull.Value)
                {
                    try
                    {
                        if (!FirstRow)
                        {
                            //int Rang = (int)RF.GetRang(Row["CUR"].ToString());
                            ////GetRangFromRegion(Row["Field"].ToString(), RVal);
                            //if (Rang > 0)
                            //{
                            //    Row["RANG"] = Rang.ToString();
                            //}
                        }
                        FirstRow = false;
                    }
                    catch { }
                    if (Row["PREV"] != DBNull.Value)
                    {
                        Row["PREVDeviation"] = (decimal)(Row["CUR"]) - (decimal)(Row["PREV"]);
                        Row["PREVSpeedDeviation"] = ((decimal)(Row["CUR"]) / (decimal)(Row["PREV"]) - 1);
                    }
                    if (Row["FY"] != DBNull.Value)
                    {
                        Row["FYSpeedDeviation"] = ((decimal)(Row["CUR"]) / (decimal)(Row["FY"]) - 1);
                        Row["FYDeviation"] = (decimal)(Row["CUR"]) - (decimal)(Row["FY"]);
                    }
                }
            }


            Grid.DataSource = ReportTable;
                //SortTable(ReportTable);
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
            Header.RowLayoutColumnInfo.OriginX = x;
            Header.RowLayoutColumnInfo.OriginY = y;
            Header.RowLayoutColumnInfo.SpanX = spanX;
            Header.RowLayoutColumnInfo.SpanY = SpanY;
            SetDefaultStyleHeader(Header);

            return Header;
        }

        private void ConfHeader(ColumnHeader ch, int x, int y, int spanX, int spanY, string Caption)
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

        private void SetHeader()
        {
            ColumnsCollection cols = Grid.Columns;

            ConfHeader(cols.FromKey("Field").Header, 0, 0, 1, 2, "Предприятия");

            ConfHeader(cols.FromKey("CUR").Header, 1, 1, 1, 1, CurDay.GridHeader() + "  ");
            ConfHeader(cols.FromKey("PREV").Header, 2, 1, 1, 1, PrevDay.GridHeader() + "  ");
            ConfHeader(cols.FromKey("FY").Header, 3, 1, 1, 1, FyDay.GridHeader() + "  ");

            ConfHeader(cols.FromKey("PREVDeviation").Header, 5-1, 1, 1, 1, "Абсолютное отклонение, рубль");
            ConfHeader(cols.FromKey("PREVSpeedDeviation").Header, 6-1, 1, 1, 1, "Темп прироста, %");

            ConfHeader(cols.FromKey("FYDeviation").Header, 7-1, 1, 1, 1, "Абсолютное отклонение, рубль");
            ConfHeader(cols.FromKey("FYSpeedDeviation").Header, 8-1, 1, 1, 1, "Темп прироста, %");

            Grid.Bands[0].HeaderLayout.Add(GenHeader("Средняя розничная цена, рублей за тонну", 1, 0, 3, 1));
            Grid.Bands[0].HeaderLayout.Add(GenHeader("Динамика к предыдущему отчётному периоду", 5-1, 0, 2, 1));
            Grid.Bands[0].HeaderLayout.Add(GenHeader("Динамика за период с начала года", 7-1, 0, 2, 1));

            Grid.Bands[0].HeaderLayout[0].Style.BorderDetails.ColorLeft = Color.LightGray;

            CRHelper.FormatNumberColumn(cols.FromKey("PREVSpeedDeviation"), "### ### ##0.00 %");
            CRHelper.FormatNumberColumn(cols.FromKey("FYSpeedDeviation"), "### ### ##0.00 %");

        }

        public static void SetImageFromGrowth(UltraGridCell cell, decimal value)
        {
            bool IsReverce = true;

            decimal valShift = value;

            if (valShift == 0)
            {
                return;
            }

            string UpOrDouwn = valShift > 0 ? "Up" : "Down";

            string GrenOrRed = (((UpOrDouwn == "Up") && (!IsReverce)) ||
                                ((UpOrDouwn == "Down") && (IsReverce))) ?
                                              "Green" : "Red";

            string ImageName = "arrow" + GrenOrRed + UpOrDouwn + "BB.png";

            string ImagePath = "~/images/" + ImageName;

            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
            cell.Style.BackgroundImage = ImagePath;



        }
        string ClearDataMemberCaption(string s)
        {
            if (s.Contains("ДАННЫЕ)"))
            {
                return s.Remove(0, 1).Replace("ДАННЫЕ)", "");
            }
            return s;
        }

        private static void _SetStar(UltraGridCell cell, bool Max)
        {
            if (!Max)
            {
                cell.Title = "Самый низкий уровень цены";
                cell.Style.BackgroundImage = "~/images/starYellowBB.png";
                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }
            else
            {
                cell.Title = "Самый высокий уровень цены";
                cell.Style.BackgroundImage = "~/images/starGrayBB.png";
                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }
        }

        private void SetStar(UltraGridRow Row)
        {
            return;
            if (Row.Cells.FromKey("RANG").Value != null)
            {

                if (Row.Cells.FromKey("RANG").Text == "1")
                {
                    //_SetStar(Row.ce, true);
                }
                if (Row.Cells.FromKey("RANG").Text == (RF.Count).ToString())
                {
                    //_SetStar(Row, false);
                }
            }
        }

        

        private void SetStarChar(UltraGridCell Cell)
        {
            string NameRegion = Cell.Text;

            string[] StarRegions = new string[12] { "Ханты-Мансийский автономный округ", "Советск", "Сургутск", "Когал", "Ланге", "Мегион", "Нефтеюганск", "Нижневартовский-", "Нягань", "Сургут", "Пыть", "Югорск" };
            foreach (string R in StarRegions)
            {
                if (NameRegion.Contains(R))
                {
                    return;
                }
            }
            Cell.Text += "";
        }

        private void FormatGridValueRow(UltraGridRow Row)
        {
            if ((Row.Cells.FromKey("Field").Text.Contains("Белоярский муниципальный район")) && (TypeField.SelectedValue.Contains("Газ")))
            {
                Row.Cells[0].Style.BackgroundImage = "~/images/cornerGreen.gif";
                Row.Cells[0].Title = "Единица измерения: куб.метр";
                Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: right top;  margin: 2px";
            }


            if (Row.Cells.FromKey("PREVSpeedDeviation").Value != null)
                SetImageFromGrowth(Row.Cells.FromKey("PREVSpeedDeviation"), (decimal)Row.Cells.FromKey("PREVDeviation").Value);
            if (Row.Cells.FromKey("FYSpeedDeviation").Value != null)
                SetImageFromGrowth(Row.Cells.FromKey("FYSpeedDeviation"), (decimal)Row.Cells.FromKey("FYDeviation").Value);

            SetStar(Row);

            SetStarChar(Row.Cells.FromKey("Field"));

            Row.Cells.FromKey("PREVDeviation").Title = "Изменение в руб. к " + PrevDay.GridHeader();
            Row.Cells.FromKey("FYDeviation").Title = "Изменение в руб. к " + FyDay.GridHeader();

            Row.Cells.FromKey("PREVSpeedDeviation").Title = "Изменение в % к " + PrevDay.GridHeader();
            Row.Cells.FromKey("FYSpeedDeviation").Title = "Изменение в % к " + FyDay.GridHeader();
        }

        void addOrAdd(Dictionary<string, UltraGridCell> val, string key, UltraGridCell value, bool max)
        {
            if (val.ContainsKey(key))
            {
                val[key] = (max ? (decimal)val[key].Value > (decimal)value.Value : (decimal)val[key].Value < (decimal)value.Value) ? val[key] : value;
            }
            else
            {
                val.Add(key, value);
            }
        }

        private void SetStarFromVal(bool max)
        {
            Dictionary<string, UltraGridCell> val = new System.Collections.Generic.Dictionary<string, UltraGridCell>();
            for (int i = 1; i < Grid.Rows.Count; i++)
            {
                for (int j = 1; j < Grid.Rows[i].Cells.Count; j++)
                {
                    if (Grid.Rows[i].Cells[j].Value != null)
                    {
                        addOrAdd(val, Grid.Columns[j].BaseColumnName, Grid.Rows[i].Cells[j],max);
                    }
                }
            }
            try
            {
                _SetStar(val["CUR"], max);
                _SetStar(val["FY"], max);
                _SetStar(val["PREV"], max);
                
            }
            catch { }

        }


        private void ConfRow()
        {
            Color newxtColor = Color.Transparent;

            UltraGridRow LastRow = null;

            Grid.Rows[0].Style.Font.Bold = true;

            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells.FromKey("Field").Value != null)
                {
                    FormatGridValueRow(Row);
                }
            }
            SetStarFromVal(true);
            SetStarFromVal(false);

        }
        private void ActiverRowFromValueFirstCell(string p)
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[0].Text == p)
                {
                    Row.Activate();
                    Row.Activated = true;
                    Row.Selected = true;
                }
            }
        }
        private void ConfigurateGrid()
        {
            SetHeader();
            ConfRow();
            CustomizerSize.ContfigurationGrid(Grid);
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.NullTextDefault = "-";
            Grid.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;            

            Grid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            if (!string.IsNullOrEmpty(SelectField.Value))
            {
                ActiverRowFromValueFirstCell(SelectField.Value);
            }
        }

        

        void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            SelectField.Value = e.Row.Cells[0].Text;

            foreach (UltraGridRow Row in Grid.Rows)
            {
                Row.Activated = false;
                Row.Selected = false;
            }

            if (e.Row.Index == 0)
            {
                e.Row.NextRow.Activate();
                e.Row.NextRow.Selected = true;
                e.Row.NextRow.Activated = true;
                SelectField.Value = e.Row.NextRow.Cells[0].Text;
            }
            else
            {
                e.Row.Activate();
                e.Row.Selected = true;
                e.Row.Activated = true;
            }
            
            GenerationLineChart();
            SetHeaderReport();
        }

        #endregion

        #region Chart
        private void ConfColChart()
        {
            ChartColumn.ChartType = ChartType.ColumnChart;

            ChartColumn.Legend.Visible = false; 
            ChartColumn.Axis.X.Labels.Visible = false;

            ChartColumn.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ### ##0.##>";

            ChartColumn.Axis.X.Margin.Near.Value = 3;
            ChartColumn.Axis.X.Margin.Far.Value = 3;
            ChartColumn.Axis.Y.Extent = 60;
            ChartColumn.Axis.X.Extent = 60; 
            ChartColumn.Tooltips.FormatString = "«<SERIES_LABEL>»<br>На <b>" + CurDay.GridHeader() + " г.</b><br><b><DATA_VALUE:N2></b>, рублей за тонну";

            ChartColumn.BackColor = Color.Transparent;

            #region Настройка диаграммы

            ChartColumn.Height = Unit.Empty;

            ChartColumn.ChartType = ChartType.ColumnChart;
            ChartColumn.Border.Thickness = 0;

            ChartColumn.ColumnChart.SeriesSpacing = 1;
            ChartColumn.ColumnChart.ColumnSpacing = 1;

            ChartColumn.Axis.X.Extent = 180;
            ChartColumn.Axis.X.Labels.Visible = false;
            ChartColumn.Axis.X.Labels.SeriesLabels.Visible = true;
            ChartColumn.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            ChartColumn.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            ChartColumn.Axis.Y.Extent = 60;
            ChartColumn.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            ChartColumn.ColorModel.ModelStyle = ColorModels.PureRandom;

            //ChartColumn.Tooltips.FormatString = "<SERIES_LABEL>\nРозничная цена: <b><DATA_VALUE:N2></b>, рубль";
            #endregion

            ChartColumn.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(Chart_FillSceneGraph);
        }

        string GenShortNameRegion(string NameRegion)
        {
            if (NameRegion[0] != 'Г')
            {
                return NameRegion.Split(' ')[0] + " р-н";
            }
            else
            {
                return NameRegion.Replace("Город", "г.");
            }
            return NameRegion;
        }

        decimal GetMaxFormCol(DataTable Table,bool max)
        {
            decimal mv = max?decimal.MinValue:decimal.MaxValue;
            foreach (DataRow Row in Table.Rows)
            {
                if (Row["CUR"] != DBNull.Value)
                {

                    if (max)
                    {
                        mv = mv > (decimal)Row["CUR"] ? mv : (decimal)Row["CUR"];
                    }
                    else
                    {
                        mv = mv < (decimal)Row["CUR"] ? mv : (decimal)Row["CUR"];
                    }
                }
            }
            return mv;
        }

        private void DataBindColChart() 
        {
            DataTable TableChart = new DataTable();
            TableChart.Columns.Add("Region");
            TableChart.Columns.Add("CUR", typeof(decimal));


            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DataRow Row = CreateRow(TableChart);
                Row["Region"] = ReportTable.Rows[i]["Field"].ToString().Replace("Березовский филиал Аэропорт Сургут","Березовский филиал\n Аэропорт Сургут");
                    //GenShortNameRegion(ReportTable.Rows[i]["Field"].ToString());
                Row["CUR"] = ReportTable.Rows[i]["CUR"];
            }

            decimal max = GetMaxFormCol(TableChart, true);
            decimal min = GetMaxFormCol(TableChart, false);

            ChartColumn.Axis.Y.RangeType = AxisRangeType.Custom;
            ChartColumn.Axis.Y.RangeMax = (double)max * 1.1;
            ChartColumn.Axis.Y.RangeMin = (double)min * 0.9;
            

            ChartColumn.DataSource = SortTable(TableChart);
            ChartColumn.DataBind();
        }

        private void GenerationColChart()
        {
            ConfColChart();
            DataBindColChart();
        }





        #endregion

        private void DataBindLineChart()
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    SelectField.Value = Grid.Rows[1].Cells[0].Text;
                }

                DataTable TableChart = DataBaseHelper.ExecQueryByID("LineChart", "Day");

                foreach (DataRow Row in TableChart.Rows)
                {
                    Day day = new Day(Row["UName"].ToString());
                    Row["Day"] = day.ChartLabel();

                }
                TableChart.Columns.Remove("UName");
                ChartLine.DataSource = TableChart;
                ChartLine.DataBind();
            }
            catch { }
        }

        private void ConfLineChart()
        {
            ChartLine.ChartType = ChartType.LineChart;
            ChartLine.Data.SwapRowsAndColumns = true;
            ChartLine.Legend.FormatString = "<ITEM_LABEL>";
            ChartLine.Legend.Location = LegendLocation.Bottom;
            ChartLine.Legend.SpanPercentage = 10;

            ChartLine.Axis.Y.Extent = 60;
            ChartLine.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            ChartLine.Axis.X.Margin.Near.Value = 3;
            ChartLine.Axis.X.Margin.Far.Value = 3;

            ChartLine.Axis.Y.Margin.Near.Value = 3;
            ChartLine.Axis.Y.Margin.Far.Value = 3;

            ChartLine.LineChart.NullHandling = NullHandling.DontPlot;

            ChartLine.Tooltips.FormatString = "«<ITEM_LABEL>»<br>На <b>" + "<SERIES_LABEL> г." + "</b><br><b><DATA_VALUE:N2></b>, рублей за тонну";
            ChartLine.FillSceneGraph 
                += 
                new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ChartLine_FillSceneGraph);

            ChartLine.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(ChartLine_InvalidDataReceived);

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
            }
        }

        void ChartLine_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        void ChartLine_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Text t = new Text(new Point(12, 90), "Рублей за тонну");
            t.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            t.labelStyle.Font = new Font("Verdana", 10);
            t.labelStyle.FontColor = Color.Gray;
            e.SceneGraph.Add(t);
        }

        private void GenerationLineChart()
        {
            DataBindLineChart();
            ConfLineChart();
        }
        
        public static string GetCurrentDateTime(HttpContext context)
        {

            return DateTime.Now.ToString();
        }


        public void WriteSubstitution(
    HttpResponseSubstitutionCallback callback
)
        {

        }

        void test()
        {
            Response.WriteSubstitution(GetCurrentDateTime);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            FillCombo();

            ChosenParam();
            DataBindGrid();
            ConfigurateGrid();

            GenerationColChart();
            GenerationLineChart();            
            SetHeaderReport();

            ChartLine.BackColor = Color.White;
            ChartColumn.BackColor = Color.White;
        }



        private void SetHeaderReport()
        {
            //Динамика стоимости авиационного керосина предприятия «Выбранная в таблице строка», рублей за тонну
            Label3.Text = string.Format("Распределение территорий по средней розничной цене на «{0}», рубль, {1}", Fields.Value, CurDay.GridHeader());

            Label4.Text = string.Format("Динамика стоимости авиационного керосина предприятия «{0}», рублей за тонну", SelectField.Value);

            Label3.Text = string.Format("Распределение предприятий по стоимости авиационного керосина по состоянию на {0}, рублей за тонну",ComboCurDay.SelectedValue);

            PageSubTitle.Text = string.Format("Еженедельный мониторинг цен на авиационный керосин в разрезе предприятий торговли, {1}, по состоянию на {0} года.", CurDay.GridHeader(), RegionSettingsHelper.Instance.Name);

            Hederglobal.Text = "Анализ стоимости авиационного керосина";


            Page.Title = Hederglobal.Text;                
            
            Page.Title = Hederglobal.Text;
            
        }

        void ChartSZ_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Text t = new Text(new Point(12, 100), "Количество МО");
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
            
            e.CurrentWorksheet.Workbook.Worksheets["Таблица"].Rows[3].Cells[0].CellFormat.Font.Name = e.CurrentWorksheet.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Font.Name;

            e.CurrentWorksheet.Workbook.Worksheets["Таблица"].Rows[3].Cells[0].CellFormat.Font.Height = e.CurrentWorksheet.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Font.Height;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");


            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");

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
            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 7;
            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";
            ReportExcelExporter1.ExcelExporter.Export(Grid, sheet1);

            //ReportExcelExporter1.Export(ChartLine, Label4.Text, sheet2, 2);

            //ReportExcelExporter1.Export(ChartColumn, Label3.Text, sheet3, 2);
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
                    //int Height = GetStringHeight(

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
                sheet.Workbook.Worksheets[2].Rows[0].Cells[0].Value = Label3.Text;
                sheet.Workbook.Worksheets[2].Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
                sheet.Workbook.Worksheets[2].Rows[0].Cells[0].CellFormat.Font.Height = 11 * 20;
                sheet.Workbook.Worksheets[2].Rows[0].Height = 13 * 30;
                ChartColumn.Width = 800;
                ReportExcelExporter.ChartExcelExport(sheet.Workbook.Worksheets[2].Rows[1].Cells[0], ChartColumn);
                sheet.Workbook.Worksheets[1].Rows[0].Cells[0].Value = Label4.Text;
                sheet.Workbook.Worksheets[1].Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
                sheet.Workbook.Worksheets[1].Rows[0].Cells[0].CellFormat.Font.Height = 11 * 20;
                sheet.Workbook.Worksheets[1].Rows[0].Height = 13 * 30;
                ChartLine.Width = 800;
                ReportExcelExporter.ChartExcelExport(sheet.Workbook.Worksheets[1].Rows[1].Cells[0],ChartLine);
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
            ISection section3 = report.AddSection();

            DataBindGrid();
            ConfigurateGrid();

            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            GridHeaderCell header;
            headerLayout.AddCell("Наименование МО");
            header = headerLayout.AddCell("Средняя розничная цена, рубль");
            header.AddCell(String.Format("{0}", FyDay.GridHeader()));
            header.AddCell(String.Format("{0}", PrevDay.GridHeader()));
            header.AddCell(String.Format("{0}", CurDay.GridHeader()));

            //headerLayout.AddCell("Ранг");

            header = headerLayout.AddCell("Динамика за период с начала года");
            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");

            header = headerLayout.AddCell("Динамика к предыдущему отчетному периоду");
            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");

            headerLayout.ApplyHeaderInfo(); 
            
            ReportPDFExporter1.HeaderCellHeight = 20;

            ReportPDFExporter1.Export(headerLayout, section1);

            ChartLine.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.85);
            ReportPDFExporter1.Export(ChartLine, Label4.Text, section2);


            ChartColumn.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.85);
            ReportPDFExporter1.Export(ChartColumn, Label3.Text, section3);



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
                            box.PE.Fill = Color.Yellow;
                            box.PE.FillStopColor = Color.Yellow;
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

            Text t = new Text(new Point(12, 60), "Рублей за тонну");
            t.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            t.labelStyle.Font = new Font("Verdana", 10);
            t.labelStyle.FontColor = Color.Gray;
            e.SceneGraph.Add(t);

            //  =^_^=
            return;

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double RFStartLineX = 0;
            double RFStartLineY = 0;

            double RFEndLineX = 0;
            double RFEndLineY = 0;
            string RFheader = "";

            if (ReportTable.Rows[0]["CUR"] != DBNull.Value)
            {
                RFheader = string.Format("{0}: {1:N2} {2}", ReportTable.Rows[0]["Field"], ReportTable.Rows[0]["CUR"], "рубль");

                RFStartLineX = xAxis.Map(xAxis.Minimum);
                RFStartLineY = yAxis.Map((decimal)ReportTable.Rows[0]["CUR"]);

                RFEndLineX = xAxis.Map(xAxis.Maximum);
                RFEndLineY = yAxis.Map((decimal)ReportTable.Rows[0]["CUR"]);


            }



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