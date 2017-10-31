using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebNavigator;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0011_Novosib
{
    public partial class Default : CustomReportPage
    {
        ICustomizerSize CustomizerSize;

        IDataSetCombo DataSetComboDay;
        IDataSetCombo DataSetComboRegion;

        RadioButtons NaviagatorChart;

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
                        return 740;
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
                        return 740;
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
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 23) / 12;
                    //(Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / 12;
                    //(Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / 12;
                    //(Grid.Columns.Count - 1);
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
                    col.Width = onePercent * (100 - 26) / (12);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (12);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (12);
                }

                Grid.Columns[0].Width = onePercent * 19;
            }
            #endregion


            public override int GetMapHeight()
            {
                return 705;
            }
        }
        #endregion

        DataBaseHelper DBHelper;
        DataBaseHelper DBRFHelper;

        CustomParam LastSelectComboBox { get { return (UserParams.CustomParam("_")); } }
        CustomParam ChangeNavigatorChart { get { return (UserParams.CustomParam("ChangeNavigatorChart")); } }

        CustomParam DateForRang { get { return (UserParams.CustomParam("DateForRang")); } }
        CustomParam FieldForRang { get { return (UserParams.CustomParam("FieldForRang")); } }
        CustomParam ActiveRegion { get { return (UserParams.CustomParam("ActiveRegion")); } }

        CustomParam WithZoneDate { get { return (UserParams.CustomParam("WithZoneDate")); } }
        CustomParam Dates { get { return (UserParams.CustomParam("Dates")); } }

        CustomParam LastDate { get { return (UserParams.CustomParam("LastDate")); } }
        CustomParam LastDateCompare { get { return (UserParams.CustomParam("LastDateCompare")); } }
        

        class DataBaseHelper
        {
            public DataProvider ActiveDP;

            public DataTable ExecQueryByID(string QueryId)
            {
                return ExecQueryByID(QueryId, QueryId);
            }

            public DataTable ExecQueryByID(string QueryId, string FirstColName)
            {
                string QueryText = DataProvider.GetQueryText(QueryId);
                DataTable Table = ExecQuery(QueryText, FirstColName);
                return Table;
            }

            public DataTable ExecQuery(string QueryText, string FirstColName)
            {
                DataTable Table = new DataTable();
                ActiveDP.GetDataTableForChart(QueryText, FirstColName, Table);
                return Table;
            }

            public DataBaseHelper(DataProvider ActiveDataProvaider)
            {
                this.ActiveDP = ActiveDataProvaider;
            }

        }

        #region ComboClass
        abstract class IDataSetCombo
        {
            public enum TypeFillData
            {
                Linear, TwoLevel
            }

            public int LevelParent = 2;

            public string LastAdededKey { get; protected set; }
            public string PrevLastAdededKey { get; protected set; }

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
                this.PrevLastAdededKey = LastAdededKey;
                this.LastAdededKey = RealChild;
            }

            public abstract void LoadData(DataTable Table);

        }

        class SetComboDay : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LaseYear = "";
                string LastMounth = "";
                string PrevAddedKey = "";

                Table.Columns.Add("PrevDisplayName");

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();

                    string Day = row["Day"].ToString();

                    string Mounth = row["Mounth"].ToString();

                    string Year = row["Year"].ToString();

                    string DisplayNAme = this.GetAlowableAndFormatedKey(Day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(Mounth)) + " " + Year + " года", "");

                    if (LaseYear != Year)
                    {
                        this.AddItem(Year + " год", 0, string.Empty);
                    }
                    if (LastMounth != Mounth)
                    {
                        this.AddItem(Mounth + " " + Year + " годa", 1, string.Empty);
                    }

                    LaseYear = Year;
                    LastMounth = Mounth;

                    row["PrevDisplayName"] = PrevAddedKey;

                    PrevAddedKey = DisplayNAme;

                    this.AddItem(DisplayNAme, 2, UniqueName);

                    this.addOtherInfo(Table, row, DisplayNAme);
                }
            }
        }

        class SetComboRegion : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {   
                string LastParent = "";

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UName"].ToString();

                    string Parent = row["Parent"].ToString();

                    string DisplayName = row[0].ToString();
                    
                    if (LastParent != Parent)
                    { 
                        this.AddItem(Parent,0,string.Empty);
                    }
                    LastParent = Parent;

                    this.AddItem(DisplayName, 1, UniqueName);
                    
                    this.addOtherInfo(Table, row, DisplayName);
                }
            }
        }

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            DBHelper = new DataBaseHelper(DataProvidersFactory.PrimaryMASDataProvider);

            DBRFHelper = new DataBaseHelper(DataProvidersFactory.SecondaryMASDataProvider);

            #region ini CustomizerSize
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
            Grid.Height = Unit.Empty;

            UltraChart1.Width = CustomizerSize.GetChartWidth();
            UltraChart1.Height = 500;

            UltraChart2.Width = CustomizerSize.GetChartWidth();
            UltraChart2.Height = 500;
            
            #endregion

            #region Экспорт

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #endregion
        }

        class CoolDate
        {
            public DateTime dt;

            public CoolDate(DateTime dt)
            {
                this.dt = dt;
            }

            public CoolDate(string MDXName)
            {
                this.dt = ConvertToDateTime(MDXName);
            }

            private DateTime ConvertToDateTime(string MDXName)
            {   
                string[] firstsplit = MDXName.Replace("[", "").Replace("]", "").Split('.');
                int Year = int.Parse(firstsplit[3]);
                int Mounth = CRHelper.MonthNum(firstsplit[6]);
                int Day = int.Parse(firstsplit[7]);
                return new DateTime(Year, Mounth, Day);

            }

            public string ConvertToMDXName()
            {
                return string.Format(@"[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]",
                    dt.Year,
                CRHelper.HalfYearNumByMonthNum(dt.Month),
                CRHelper.QuarterNumByMonthNum(dt.Month),
                CRHelper.RusMonth(dt.Month),
                dt.Day);
            }

            public string ReportDate()
            {
                return string.Format("{0:00}.{1:00}.{2:00}", dt.Day, dt.Month, dt.Year);
            }

            public string ConvertToMDXNameRF()
            {
                DateTime dt_ = this.dt.AddMonths(-1);
                return string.Format(@"[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                    dt_.Year,
                CRHelper.HalfYearNumByMonthNum(dt_.Month),
                CRHelper.QuarterNumByMonthNum(dt_.Month),
                CRHelper.RusMonth(dt_.Month));
            }

        }

        private string GenStringFromAllChildrenNode(Node node)
        {
            string Result = node.Text;
            if (node.Nodes.Count > 0)
            {
                foreach(Node childNode in node.Nodes)
                {
                    Result += ","+ GenStringFromAllChildrenNode(childNode);
                }
            }
            return Result;
        }

        string GetAllLastNode()
        {
            ComboCurrentPeriod.SelectLastNode();
            Node Year = ComboCurrentPeriod.SelectedNode.Parent.Parent;
            return GenStringFromAllChildrenNode(Year);
        }        

        private void FillCombo()
        {
            ComboRegion.Title = "Субьект РФ";

            ComboCurrentPeriod.Title = "Период";
            ComboCurrentPeriod.PanelHeaderTitle = "Период:";
            ComboCurrentPeriod.ShowSelectedValue = true; 
            ComboTypePeriod.Title = "Период для сравнения"; 
             
            DataSetComboRegion = new SetComboRegion();
            DataSetComboRegion.LoadData(DBHelper.ExecQueryByID("ComboRegion"));
            if (!Page.IsPostBack)
            {
                ComboRegion.FillDictionaryValues(DataSetComboRegion.DataForCombo);
                ComboRegion.SetСheckedState(RegionSettingsHelper.Instance.Name, true);
            }

            ComboCurrentPeriod.MultiSelect = true;
            if (CustomizerSize is CustomizerSize_1280x1024)
            {
                ComboCurrentPeriod.Width = 300;
                ComboRegion.Width = 350;
                ComboTypePeriod.Width = 350;
            }
            else
            {
                ComboCurrentPeriod.Width = 170;
                ComboRegion.Width = 200;
                ComboTypePeriod.Width = 250 ; 
            }


            DataSetComboDay = new SetComboDay();
            DataSetComboDay.LoadData(DBHelper.ExecQueryByID("ComboDates", "Day"));
            if (!Page.IsPostBack)
            {
                ComboCurrentPeriod.FillDictionaryValues(DataSetComboDay.DataForCombo);
                //ComboCurrentPeriod.SetMultipleСheckedState(GetAllLastNode(), true);
                ComboCurrentPeriod.SetСheckedState("2011 год", true);
            }

            if (!Page.IsPostBack)
            {
                Dictionary<string, int> DictTypePeriod = new Dictionary<string, int>();
                DictTypePeriod.Add("К предыдущей дате", 0);
                DictTypePeriod.Add("К началу года", 0);
                DictTypePeriod.Add("К аналогичной дате предыдущего года", 0);

                ComboTypePeriod.FillDictionaryValues(DictTypePeriod);
            }
            

        }

        string RemoveLastChar(string s)
        {
            return s.Remove(s.Length - 1, 1);
        }

        private CoolDate GetCompareDate(string ComboName)
        {
            CoolDate CurDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboName]);

            try
            {
                if (ComboTypePeriod.SelectedValue == "К предыдущей дате")
                {
                    return new CoolDate(DataSetComboDay.DataUniqeName[DataSetComboDay.OtherInfo[ComboName]["PrevDisplayName"]]);
                }
                else
                    if (ComboTypePeriod.SelectedValue == "К началу года")
                    {
                        return new CoolDate(new DateTime(CurDate.dt.Year, 1, 1));
                    }
                    else
                    {
                        return new CoolDate(new DateTime(CurDate.dt.Year - 1, CurDate.dt.Month, CurDate.dt.Day));
                    }
            }
            catch { return new CoolDate(new DateTime(1990,10,10)); }
        }

        private string GenMemberSelect(CoolDate ReportDate)
        {
            return string.Format(@"[Период__Период].[Период__Период].[{0}] ", ReportDate.ReportDate());
        }

        private string GenMemberOfDate(CoolDate ReportDate)
        {
            return string.Format(@"member [Период__Период].[Период__Период].[{0}] as '{1}'", ReportDate.ReportDate(), ReportDate.ConvertToMDXName());
        }
        

        

        private void ChangeParam(bool Compare)
        {
            Dates.Value = "";
            WithZoneDate.Value = "";

            List<CoolDate> CoolDates = new List<CoolDate>();
            List<CoolDate> CoolCompareDates = new List<CoolDate>();
            
            foreach (Node SelectDate in ComboCurrentPeriod.SelectedNodes)
            {   
                //if (!string.IsNullOrEmpty(DataSetComboDay.DataUniqeName[SelectDate.Text]))
                if (SelectDate.Level == 2)
                {
                    CoolDates.Add(new CoolDate(DataSetComboDay.DataUniqeName[SelectDate.Text]));
                    CoolCompareDates.Add(GetCompareDate(SelectDate.Text));
                }
                if (SelectDate.Level == 1)
                {
                    foreach (Node SelectDateC in SelectDate.Nodes)
                    {
                        CoolDates.Add(new CoolDate(DataSetComboDay.DataUniqeName[SelectDateC.Text]));
                        CoolCompareDates.Add(GetCompareDate(SelectDateC.Text));
                    }
                }
                if (SelectDate.Level == 0)
                {
                    foreach (Node SelectDate_ in SelectDate.Nodes)
                    {
                        foreach (Node SelectDateC in SelectDate_.Nodes)
                        {
                            CoolDates.Add(new CoolDate(DataSetComboDay.DataUniqeName[SelectDateC.Text]));
                            CoolCompareDates.Add(GetCompareDate(SelectDateC.Text));
                        }
                    }
                }
            }

            for (int i = 0; i < CoolDates.Count; i++)
            {
                CoolDate ReportDate = CoolDates[i];
                CoolDate ComparedateDate = CoolCompareDates[i];

                if (Compare)
                {
                    WithZoneDate.Value += GenMemberOfDate(ComparedateDate);
                    Dates.Value += GenMemberSelect(ComparedateDate) + ",";
                }
                else
                {
                    WithZoneDate.Value += GenMemberOfDate(ReportDate);
                    Dates.Value += GenMemberSelect(ReportDate) + ",";
                }
            }

            Dates.Value = RemoveLastChar(Dates.Value);
        }

        class RadioButtons
        {
            public Dictionary<RadioButton, DataRow> Buttons = null;

            DataTable Table = null;

            string Scope = "";

            public DataRow ActiveRow = null;

            public EventHandler EventClick = null;

            void ConfigurateButton(RadioButton rb)
            {
                rb.AutoPostBack = true;
                rb.GroupName = this.ToString();// +" в сфере «" + Scope + "»";

                rb.Font.Name = "Verdana";
                rb.Font.Size = 10;

                rb.CheckedChanged += new EventHandler(EventClick);
            }



            void Bind()
            {
                Buttons = new Dictionary<RadioButton, DataRow>();
                foreach (DataRow Row in Table.Rows)
                {
                    RadioButton RB = new RadioButton();
                    this.ConfigurateButton(RB);
                    RB.Text = Row[0].ToString();
                    Buttons.Add(RB, Row);
                }
            }

            public void FillPLaceHolder(PlaceHolder pl)
            {
                bool b = false; ;
                foreach (RadioButton rb in Buttons.Keys)
                {
                    Label br = new Label();

                    if (b)
                    {
                        br.Text = "<br>";
                    }

                    b = true;

                    pl.Controls.Add(br);
                    pl.Controls.Add(rb);
                }
            }

            public RadioButtons(DataTable Table, EventHandler EventC)
            {
                this.EventClick = EventC;
                this.Table = Table;
                Bind();
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
                return 100 * ((decimal)CurValue / (decimal)PrevValue - 1);
            }
            return DBNull.Value;
        }
        int CountRegion = 0;

        private object GetRang(string Field, string Date, bool reverce)
        {
            DateForRang.Value = Date;
            FieldForRang.Value = Field;
            int index = 0;
            DataTable TableRang = DBHelper.ExecQueryByID("GridRang", "Region");

            CountRegion = TableRang.Rows.Count;

            foreach (DataRow Row in TableRang.Rows)
            {
                index++;
                if (ComboRegion.SelectedValue == Row["Region"].ToString())
                {
                    if (reverce)
                    {
                        return CountRegion - index+1;
                    }
                    else { return index; }
                }
            }
            return DBNull.Value;
        }

        class InfoRow
        {
            public string ReportDisplay;
            public string FormatString;
            public string Hint = "";
            public bool Revece;
            public bool ReveceRang;
            public bool Rang;
            public Color BeginColor;
            public Color CenterColor;
            public Color EndColor;
            public string RFString;
            public string Caption;
            public string EdIzm;

            public InfoRow(string BaseName)
            {
                switch (BaseName)
                {
                    case "Уровень регистрируемой безработицы, % от численности экономически активного населения ":
                        {
                            Caption = "Уровень регистрируемой безработицы";
                            EdIzm = "% от численности экономически активного населения";

                            ReportDisplay = "Уровень регистрируемой безработицы, % от численности экономически активного населения";
                            FormatString = "N2";
                            Rang = true;
                            Revece = true;
                            ReveceRang = false;
                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;

                            RFString = "Уровень зарегистрированной безработицы (от экономически активного населения)";


                            return;
                        }
                    case "Уровень регистрируемой безработицы, % от численности экономически активного населения":
                        {
                            Caption = "Уровень регистрируемой безработицы";
                            EdIzm = "% от численности экономически активного населения";

                            ReportDisplay = "Уровень регистрируемой безработицы, % от численности экономически активного населения";
                            FormatString = "N2";
                            Rang = true;
                            Revece = true;
                            ReveceRang = false;
                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;

                            RFString = "Уровень зарегистрированной безработицы (от экономически активного населения)";


                            return;
                        }
                    case "Уровень трудоустройства ":
                        {
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (с начала года), %";
                            FormatString = "N2";
                            Rang = true;
                            Revece = false;
                            ReveceRang = true;
                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;

                            RFString = "Уровень зарегистрированной безработицы (от экономически активного населения)";


                            return;
                        }

                    case "Уровень трудоустройства":
                        {
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (с начала года), %";
                            FormatString = "N2";
                            Rang = true;
                            Revece = false;
                            ReveceRang = true;
                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;

                            RFString = "Уровень зарегистрированной безработицы (от экономически активного населения)";
                            return;
                        }
                        
                    case "Уровень трудоустройства ищущих работу граждан (с начала года), %":
                        {
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (с начала года), %";
                            Caption = "Уровень трудоустройства ищущих работу граждан (с начала года)";//, %
                            EdIzm = "%"; 
                            FormatString = "N2";
                            Rang = true;
                            Revece = false;
                            ReveceRang = true;
                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;

                            RFString = "Уровень зарегистрированной безработицы (от экономически активного населения)";


                            return;
                        }

                    case "Численность официально зарегистрированных безработных":
                        {
                            ReportDisplay = "Численность официально зарегистрированных безработных, чел.";
                            FormatString = "N0";
                            Rang = false;
                            Revece = true;
                            return;
                        }

                    case "Численность экономически активного населения":
                        {
                            ReportDisplay = "Численность экономически активного населения, чел.";
                            FormatString = "N0";
                            Rang = false;
                            Revece = false;
                            return;
                        }

                    case "Численность незанятых граждан, состоящих на учете":
                        {
                            ReportDisplay = "Численность незанятых граждан, состоящих на учете";
                            FormatString = "N0";
                            Rang = false;
                            Revece = true;
                            return;
                        }
                    case "Число заявленных вакансий":
                        {
                            ReportDisplay = "Число заявленных вакансий, ед.";
                            FormatString = "N2";
                            Rang = false;
                            Revece = false;
                            return;
                        }
                    case "Уровень напряженности на рынке труда":
                        {
                            ReportDisplay = "Уровень напряженности на рынке труда, ед.";

                            Caption = "Уровень напряженности на рынке труда";
                            EdIzm = "ед.";

                            FormatString = "N2";
                            Rang = true;
                            ReveceRang = false;

                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;

                            RFString = "Уровень напряженности на рынке труда";

                            return;
                        }
                    case "Уровень трудоустройства ищущих работу граждан":
                        {
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (с начала года), %";
                            FormatString = "N2";
                            Rang = true;
                            Revece = false;
                            ReveceRang = true;
                            Caption = "Уровень трудоустройства ищущих работу граждан (с начала года)";//, %
                            EdIzm = "%";
                            BeginColor = Color.Red;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Green;

                            RFString = "Абра-Кадабра!";
                            return;
                        }
                };
            }
        }

        DataTable BaseTableCompare = null;
        DataTable BaseTableSelect = null;
        private void DataBindGrid()
        {
            ChangeParam(true);
            BaseTableCompare = DBHelper.ExecQueryByID("Grid", "Field");

            ChangeParam(false);
            BaseTableSelect = DBHelper.ExecQueryByID("Grid", "Field");

            CoolTable TableGrid = new CoolTable();

            foreach (DataColumn BaseCol in BaseTableSelect.Columns)
            {
                TableGrid.Columns.Add(BaseCol.ColumnName, BaseCol.DataType);
            }

            foreach (DataRow BaseRow in BaseTableSelect.Rows)
            {
                DataRow GridRowValue = TableGrid.AddRow();
                DataRow GridRowDeviation = TableGrid.AddRow();
                DataRow GridRowSpeedDeviation = TableGrid.AddRow();

                string Field = BaseRow["Field"].ToString();

                GridRowValue["Field"] = Field;

                DataRow CompareRow = GetRowFromValueFirstCell(BaseTableCompare,Field);
                
                InfoRow RowInfo = new InfoRow(Field);

                for (int i = 1; i < BaseTableSelect.Columns.Count; i++)
                {
                    
                        GridRowValue[i] = BaseRow[i];
                        if (BaseTableSelect.Columns[i].ColumnName != BaseTableCompare.Columns[i].ColumnName)
                        {
                            GridRowDeviation[i] = GetDeviation(BaseRow[i], CompareRow[i]);
                        }
                        
                        if (RowInfo.Rang)
                        {
                            GridRowDeviation[i] = GetRang(Field, BaseTableSelect.Columns[i].ColumnName,RowInfo.ReveceRang);
                        }

                        if (BaseTableSelect.Columns[i].ColumnName != BaseTableCompare.Columns[i].ColumnName)
                        {
                            GridRowSpeedDeviation[i] = GetSpeedDeviation(BaseRow[i], CompareRow[i]);
                        }
                }
            }

            Grid.DataSource = TableGrid;
            Grid.DataBind();
        }

        private DataRow GetRowFromValueFirstCell(DataTable BaseTable, string Field)
        {
            foreach(DataRow row in BaseTable.Rows)
            {
                if (row[0].ToString() == Field)
                {
                    return row;
                }
            }
            return null;
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

            Grid.Columns.FromKey("Field").Header.Caption = "Показатель";
            foreach (UltraGridColumn Col in Grid.Columns)
            {
                if (!Col.BaseColumnName.Contains("Field"))
                {
                    SetDefaultStyleHeader(Col.Header);
                    Col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    Col.CellStyle.Padding.Right = 5;
                }
            }
        }

        private UltraGridCell GetNextCell(UltraGridCell Cell)
        {
            return Cell.Row.NextRow.Cells.FromKey(Cell.Column.BaseColumnName);
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
                    string.Format("Ранг по СФО на {1}: {0} \nСамое высокое значение показателя", RangCell.Value,RangCell.Column.Header.Caption) :
                    string.Format("Ранг по СФО на {1}: {0} \nСамое низкое значение показателя", RangCell.Value, RangCell.Column.Header.Caption);
                SetImageFromCell(RangCell, "starGrayBB.png");
            }
        }

        private void SetMinRang(UltraGridCell ValueCell, bool Max, bool ReverceRang)
        {
            UltraGridCell RangCell = GetNextCell(ValueCell);
            if (RangCell.Value != null)
            {
                RangCell.Title = ReverceRang ?
                    string.Format("Ранг по СФО на {1}: {0} \nСамое высокое значение показателя", RangCell.Value, RangCell.Column.Header.Caption) :
                    string.Format("Ранг по СФО на {1}: {0} \nСамое низкое значение показателя", RangCell.Value, RangCell.Column.Header.Caption);
                SetImageFromCell(RangCell,"starYellowBB.png");
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
        void SetFormatRang(UltraGridCell cell)
        {
            if (cell.Value != null)
            {
                SetFormatCell(cell, "N0");
                if (string.IsNullOrEmpty(cell.Title))
                {
                    cell.Title = string.Format("Ранг по ФО на {0}: {1}", BaseTableSelect.Columns[cell.Column.Index].ColumnName, cell.Value);
                }
                cell.Text = "Ранг "+ cell.Text;
            }
        }

        void SetFormatCell1(UltraGridCell Cell, string format)
        {
            if (Cell.Value != null)
            {
                Cell.Text = string.Format("{0:" + format + "}%", decimal.Parse(Cell.Text));
            }
        }

        private void SetIndicatorSpeedDeviationcell(UltraGridCell ValueCell, bool reverce)
        {
            UltraGridCell IndicatorCell = GetNextCell(GetNextCell(ValueCell));
            if (IndicatorCell.Value != null)
            {

                decimal Value = decimal.Parse(IndicatorCell.Value.ToString().Replace("%",""));
                if (Value != 0)
                {
                    string UpOrdouwn = Value > 0 ? UpOrdouwn = "Up" : UpOrdouwn = "Down";

                    string TitleCaption = Value > 0 ? "прироста" : "снижения";

                    IndicatorCell.Title = string.Format("Темп {0} к {1}", TitleCaption, BaseTableCompare.Columns[ValueCell.Column.Index].ColumnName);

                    string Color = "Green";
                    if ((Value > 0) & (reverce))
                    {
                        Color = "Red";
                    }

                    if ((Value < 0) & (!reverce))
                    {
                        Color = "Red";
                    }
                    SetImageFromCell(IndicatorCell, "arrow" + Color + UpOrdouwn + "BB.png");
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
                UltraGridCell IndicatorCell = GetNextCell(ValueCell);
                if (IndicatorCell.Value != null)
                {

                    decimal Value = decimal.Parse(IndicatorCell.Value.ToString());
                    if (Value != 0)
                    {   
                        string TitleCaption = Value > 0 ? "Прирост" : "Снижение";

                        IndicatorCell.Title = string.Format("{0} к {1}", TitleCaption, BaseTableCompare.Columns[ValueCell.Column.Index].ColumnName);
                    }
                }
            }
            catch { }
        }

        private void FormatValueCell(UltraGridCell ValueCell, InfoRow RowInfo)
        {
            FormatTopCell(ValueCell);
            FormatCenterCell(GetNextCell(ValueCell));
            FormatBottomCell(GetNextCell(GetNextCell(ValueCell)));

            if (ValueCell.Value == null)
            {
                return;
            }

            SetFormatCell(ValueCell, RowInfo.FormatString);
            SetFormatCell(GetNextCell(ValueCell), RowInfo.FormatString);
            SetFormatCell(GetNextCell(GetNextCell(ValueCell)), RowInfo.FormatString);

            if (RowInfo.Rang)
            {
                
                
                if (GetNextCell(ValueCell).Text.Replace(",00","") == CountRegion.ToString())
                {
                    SetMaxRang(ValueCell, true, RowInfo.ReveceRang);
                }

                
                if (GetNextCell(ValueCell).Text.Replace(",00", "") == "1")
                {
                    SetMinRang(ValueCell, true, RowInfo.ReveceRang);
                }
                SetFormatRang(GetNextCell(ValueCell));                
                
                
            }
            
            SetFormatCell1(GetNextCell(GetNextCell(ValueCell)), "N2");

            string ColName = ValueCell.Column.BaseColumnName;
            string RegionName = ValueCell.Row.Cells[0].Text;

            
            SetIndicatorDeviationcell(ValueCell, false);
            SetIndicatorSpeedDeviationcell(ValueCell, RowInfo.Revece);
        }

        private void FormatRow()
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells.FromKey("Field").Value != null)
                {
                    InfoRow RowInfo = new InfoRow(Row.Cells.FromKey("Field").Text);
                    foreach (UltraGridCell Cell in Row.Cells)
                    {
                        if (Cell.Column.BaseColumnName != "Field")
                        {
                            FormatValueCell(Cell, RowInfo);
                        }
                        else
                        {
                            Cell.RowSpan = 3;
                            Cell.Style.Wrap = true;
                            Cell.Value = RowInfo.ReportDisplay;
                        }
                    }
                }
            }
        }

        private void CustomizeGrid()
        {
            ConfHeader();
            FormatRow();
            CustomizerSize.ContfigurationGrid(Grid);
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;
            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }
            
        }

        private void GenerationGrid()
        {
            DataBindGrid();
            CustomizeGrid();
        }

        InfoRow GlobalNavigationInfoRow;

        DataRow GetFirstDictRow()
        {
            foreach (DataRow Row in NaviagatorChart.Buttons.Values)
            {
                return Row;
            }
            return null;
        }
        private void ChangeMapAndChartParam()
        {
            if (!Page.IsPostBack)
            {
                if (NaviagatorChart.ActiveRow == null)
                {
                    NaviagatorChart.ActiveRow = GetFirstDictRow();
                    foreach (RadioButton rb in NaviagatorChart.Buttons.Keys)
                    {
                        rb.Checked = true;
                        break;
                    }
                }
            }
            else
            {
                if (NaviagatorChart.ActiveRow == null)
                {
                    foreach (RadioButton rb in NaviagatorChart.Buttons.Keys)
                    {
                        if (rb.Text == LastSelectComboBox.Value)
                        {
                            NaviagatorChart.ActiveRow = NaviagatorChart.Buttons[rb];
                        }
                    }
                }
            }            

                LastSelectComboBox.Value = NaviagatorChart.ActiveRow[0].ToString();
            ChangeNavigatorChart.Value = NaviagatorChart.ActiveRow["UName"].ToString(); ;

            GlobalNavigationInfoRow = new InfoRow(UserComboBox.getLastBlock(ChangeNavigatorChart.Value));
        }

        void rb_CheckedChanged(object sender, EventArgs e)
        {
            NaviagatorChart.ActiveRow = NaviagatorChart.Buttons[(RadioButton)sender];
            ChangeMapAndChartParam();
            GenFirstChart();
            SetHeader();
        }

        private void FillNaviagatorPanel()
        {
            CoolTable TableListForChart = new CoolTable();
            TableListForChart.Columns.Add("Field");
            TableListForChart.Columns.Add("UName");
            TableListForChart.AddOrGetRow("Уровень регистрируемой безработицы, % от численности экономически активного населения")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень регистрируемой безработицы, % от численности экономически активного населения]";
            TableListForChart.AddOrGetRow("Уровень трудоустройства ищущих работу граждан (с начала года), %")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень трудоустройства ищущих работу граждан]";
            NaviagatorChart = new RadioButtons(TableListForChart, rb_CheckedChanged);
            NaviagatorChart.FillPLaceHolder(PlaceHolder1);

            
        }

        private void GenFirstChart()
        {
            UltraChart1.ChartType = ChartType.ColumnChart;

            DataTable TableChart = DBHelper.ExecQueryByID("Chart", "Field");

            TableChart.Columns[1].ColumnName = ComboRegion.SelectedValue;
            TableChart.Columns[2].ColumnName = ComboRegion.SelectedNode.Parent.Text;

            UltraChart1.DataSource = TableChart;
            UltraChart1.DataBind();

            UltraChart1.Border.Color = Color.Transparent;

            UltraChart1.Axis.X.Extent = 80;
            UltraChart1.Axis.Y.Extent = 60;

            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);

            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>%";
            UltraChart1.Axis.X.Margin.Far.Value = 2;
            UltraChart1.Axis.X.Margin.Near.Value = 2;
            

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.SpanPercentage = CustomizerSize is CustomizerSize_800x600?14:8;
            UltraChart1.Legend.Location = LegendLocation.Top;
            //UltraChart1.Legend.Font = new Font("Verdana", 9);
            UltraChart1.Legend.Margins.Right = (int)UltraChart2.Width.Value / 2;

            UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL><br>{0}<br> на <SERIES_LABEL>: <b><DATA_VALUE:N2></b>", GlobalNavigationInfoRow.ReportDisplay, ComboRegion.SelectedValue);

            #region
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            PaintElement pe1 = new PaintElement();
            pe1.ElementType = PaintElementType.Gradient;
            pe1.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe1.Fill = Color.PowderBlue;
            pe1.FillStopColor = Color.SkyBlue;
            pe1.FillOpacity = 100;
            pe1.FillStopOpacity = 200;

            PaintElement pe2 = new PaintElement();
            pe2.ElementType = PaintElementType.Gradient;
            pe2.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe2.Fill = Color.MediumBlue;
            pe2.FillOpacity = 100;
            pe2.FillStopColor = Color.Navy;
            pe2.FillStopOpacity = 200;

            UltraChart1.ColorModel.Skin.PEs.Add(pe1);
            UltraChart1.ColorModel.Skin.PEs.Add(pe2);
            #endregion
        }
       
        private void GenSecondChart()
        {
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart2_InvalidDataReceived);
            ChangeNavigatorChart.Value = @"[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Численность официально зарегистрированных безработных],[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Численность незанятых граждан, состоящих на учете],[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Число заявленных вакансий]";

            UltraChart2.ChartType = ChartType.LineChart;
            UltraChart2.LineChart.NullHandling = NullHandling.DontPlot;

            DataTable TableChart = DBHelper.ExecQueryByID("Chart2", "Field");
            if (TableChart.Columns.Count == 2)
            {
                return;
            }

            UltraChart2.DataSource = TableChart;
            UltraChart2.DataBind();

            UltraChart2.Border.Color = Color.Transparent;

            UltraChart2.Axis.X.Extent = 80;
            UltraChart2.Axis.Y.Extent = 60;

            UltraChart2.Axis.X.Labels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 8);

            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Axis.X.Margin.Far.Value = 2;
            UltraChart2.Axis.X.Margin.Near.Value = 2; 

            //UltraChart2.LineChart.

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.SpanPercentage = CustomizerSize is CustomizerSize_800x600?16:8;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.FormatString = "CaptionLegend_CaptionLegend_CaptionLegend_CaptionLegend";
            //UltraChart2.Legend.Margins.Right = (int)UltraChart2.Width.Value;

            UltraChart2.Tooltips.FormatString = string.Format("{0}<br><SERIES_LABEL><br>По состоянию на <ITEM_LABEL> г.: <b><DATA_VALUE:N0></b> чел.    ", ComboRegion.SelectedValue);
            //UltraChart2.Tooltips.FormatString = string.Format("<SERIES_LABEL><br>По состоянию на {0} г.<br><b><DATA_VALUE:N2></b> " + GlobalNavigationInfoRow.EdIzm,
              //  .ReportDate());

            UltraChart2.LineChart.NullHandling = NullHandling.DontPlot;
        }

        void UltraChart2_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        Primitive GenLineFromBox(Box b)
        {
            int x = b.rect.X;
            int y = b.rect.Y+5;
            Line L = new Line(new Point(x, y), new Point(x + 12, y), new LineStyle(LineCapStyle.NoAnchor, LineCapStyle.NoAnchor, LineDrawStyle.Solid));
            L.PE.StrokeWidth = 4;
            L.PE.Stroke = b.PE.FillStopColor;
            return L;
        }

        void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            List<Primitive> DeletePrimitive = new List<Primitive>();
            List<Primitive> AdededPrimitive = new List<Primitive>();

            string[] CaptionLegend = new string[3] {
            "Численность официально зарегистрированных безработных",
            "Численность незанятых граждан, состоящих на учете",
            "Число заявленных вакансий"};

            int IndexOfPrimitive = 0;
            int IndexCaptioAdded = 0;

            foreach (Primitive p in e.SceneGraph)
            {
                if ((p is Text) && (((Text)p).GetTextString().Contains("CaptionLegend_CaptionLegend")))
                {
                    Text Lable = p as Text;
                    Box box = e.SceneGraph[IndexOfPrimitive - 1] as Box;
                    if (IndexCaptioAdded <= 2)
                    {
                        

                        AdededPrimitive.Add(GenLineFromBox(box));
                        DeletePrimitive.Add(box);

                        Lable.SetTextString(CaptionLegend[IndexCaptioAdded]);

                        IndexCaptioAdded++;
                    }
                    else
                    {
                        DeletePrimitive.Add(box);
                        DeletePrimitive.Add(Lable);
                    }
                }
                if ((p is Text) && (((Text)p).GetTextString().Contains("more")))
                {
                    DeletePrimitive.Add(p);
                }
                IndexOfPrimitive++;
            }
            foreach (Primitive p in DeletePrimitive)
            {
                e.SceneGraph.Remove(p);
            }

            foreach (Primitive p in AdededPrimitive)
            {
                e.SceneGraph.Add(p);
            }
            
        }

        private void GenerateTextovka()
        {
            LastDateCompare.Value = GetCompareDate(ComboCurrentPeriod.GetLastNode(2).Text).ConvertToMDXName();

            LastDate.Value = DataSetComboDay.DataUniqeName[ComboCurrentPeriod.GetLastNode(2).Text];

            CoolDate CurDate = new CoolDate(LastDate.Value);
            CoolDate CompareDate = new CoolDate(LastDateCompare.Value);

            DataTable table = DBHelper.ExecQueryByID("Textovka");

            string Textovka = string.Format("&nbsp;&nbsp;&nbsp;Ситуация на рынке труда на <b>{0}</b>: <b>{1}</b>", CurDate.ReportDate(), ComboRegion.SelectedValue);

            decimal suValue0 = (decimal)table.Rows[0][1];
            decimal suValue1 = (decimal)table.Rows[1][1];
            Textovka += string.Format("<br>&nbsp;&nbsp;&nbsp;Уровень зарегистрированной безработицы (в % от экономически активного населения) составляет <b>{0:N2}</b> %, что {1} {2} уровня по федеральному округу (<b>{3:N2}</b> %)",
                suValue0,
                GenCaptionUpOrDouwn(suValue0 - suValue1),
                GenBall(suValue1 - suValue0),
                suValue1);

            decimal suValue2 = (decimal)table.Rows[2][1];
            decimal suValue3 = (decimal)table.Rows[3][1];
            Textovka += string.Format("<br>&nbsp;&nbsp;&nbsp;Уровень трудоустройства ищущих работу граждан (с начала года) составил <b>{0:N2}</b> %, что {1} {2} уровня по федеральному округу (<b>{3:N2}</b> %)",
                suValue2,
                GenCaptionUpOrDouwn(suValue2 - suValue3),
                GenBall(suValue2 - suValue3),
                suValue3);
            try
            {
                decimal suValue4 = (decimal)table.Rows[4][1];
                decimal suValue4_prev = (decimal)table.Rows[4][2];

                Textovka = GenText(Textovka, suValue4, suValue4_prev, "Численность официально зарегистрированных безработных", true);
            }
            catch { }

            try
            {
                decimal suValue4 = (decimal)table.Rows[5][1];
                decimal suValue4_prev = (decimal)table.Rows[5][2];

                Textovka = GenText(Textovka, suValue4, suValue4_prev, "Численность незанятых граждан, состоящих на учете", true);
            }
            catch { }
            try
            {
                decimal suValue4 = (decimal)table.Rows[6][1];
                decimal suValue4_prev = (decimal)table.Rows[6][2];

                Textovka = GenText(Textovka, suValue4, suValue4_prev, "Число заявленных вакансий составляет", false);
            }
            catch { }

            Textovko.Text = Textovka;
        }

        private string GenText(string Textovka, decimal suValue4, decimal suValue4_prev, string field, bool reverce)
        {
            LastDateCompare.Value = GetCompareDate(ComboCurrentPeriod.GetLastNode(2).Text).ConvertToMDXName();

            LastDate.Value = DataSetComboDay.DataUniqeName[ComboCurrentPeriod.GetLastNode(2).Text];

            CoolDate CurDate = new CoolDate(LastDate.Value);
            CoolDate CompareDate = new CoolDate(LastDateCompare.Value);

            Textovka += string.Format("<br>&nbsp;&nbsp;&nbsp;{9} составляет <b>{0:N0}</b>  чел. {1} {2} значения показателя за период с <b>{3}</b> по <b>{4}</b> {5} <b>{6}</b> чел. (темп {7} <b>{8:N2}</b> %)",
                suValue4,
                GenSDUp(suValue4 - suValue4_prev),
                GenArrow(suValue4 - suValue4_prev,reverce),
                CompareDate.ReportDate(),
                CurDate.ReportDate(),
                GenSos(suValue4 - suValue4_prev),
                Math.Abs(suValue4 - suValue4_prev),
                GenSD(suValue4 - suValue4_prev),
                (suValue4 / suValue4_prev - 1) * 100,
                field);
            return Textovka;
        }

        private object GenArrowRe(decimal p)
        {
            return p < 0 ? "<img src='../../images/ArrowRedUpBB.png'>" : "<img src='../../images/ArrowGreenDownBB.png'>";
        }

        private object GenSDUp(decimal p)
        {
            return p < 0 ? "Снижение" : "Рост";
        }

        private object GenSD(decimal p)
        {
            return p > 0 ? "прироста" : "снижения";
        }
        private object GenSos(decimal p)
        {
            return p < 0 ? "составило" : "составил";
        }

        private object GenArrow(decimal p,bool reverce)
        {
            if (reverce)
            {
                return p < 0 ? "<img src='../../images/ArrowGreenDownBB.png'>" : "<img src='../../images/ArrowRedUpBB.png'>";
            }
            else
            {
                return p < 0 ? "<img src='../../images/ArrowRedDownBB.png'>" : "<img src='../../images/ArrowGreenUpBB.png'>";
            }
        }

        private string GenBall(decimal p)
        {
            return p < 0 ? "<img src='../../images/BallRedBB.png'>" : "<img src='../../images/BallGreenBB.png'>";
        }

        private string GenCaptionUpOrDouwn(decimal p)
        {
            return p > 0 ? "выше" : "ниже";
        }

        private void SetHeader()
        {
            PageTitle.Text = string.Format("Мониторинг ситуации на рынке труда ({0})",  
                //ComboTypePeriod.SelectedValue
                ComboRegion.SelectedValue
                );
                //ComboTypePeriod.SelectedValue);
            PageSubTitle.Text = string.Format("Данные ежемесячного мониторинга ситуации на рынке труда выбранного региона");
                //, ComboCurrentPeriod.SelectedValue);
            Page.Title = PageTitle.Text;
            LabelChart1.Text = string.Format("Сравнительная динамика показателя «{0}», {1}", GlobalNavigationInfoRow.Caption, GlobalNavigationInfoRow.EdIzm);
            LabelChart2.Text = string.Format("Сравнительная динамика численности незанятых граждан, безработных и числа заявленных вакансий ({0})", 
                ComboRegion.SelectedValue);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();
            ActiveRegion.Value = DataSetComboRegion.DataUniqeName[ComboRegion.SelectedValue];

            GenerationGrid();
            FillNaviagatorPanel();
            ChangeMapAndChartParam();
            GenFirstChart();
            GenSecondChart();

            GenerateTextovka();
            SetHeader();
        }
        

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();


            IText title = section1.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);


            title = section1.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = section1.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);

            GridHeaderLayout headerLayout = GenExportLayot();

            ReportPDFExporter1.Export(headerLayout, section1);

            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[0].Value != null)
                {
                    FormatTopCell(Row.Cells[0]);
                    FormatCenterCell(GetNextCell(Row.Cells[0]));
                    FormatBottomCell(GetNextCell(GetNextCell(Row.Cells[0])));
                }
            }

            section2.PageSize = new PageSize(section1.PageSize.Height, section1.PageSize.Width);

            

            //DundasMap1.Height = 700;
            ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text, section2);
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section3);
        }

        private GridHeaderLayout GenExportLayot()
        {
            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            foreach (UltraGridColumn Col in Grid.Columns)
            {
                //Col.Width = 200;
                headerLayout.AddCell(Col.Header.Caption);

            }

            headerLayout.ApplyHeaderInfo();

            //headerLayout.AddCell("Показатель");
            //headerLayout.AddCell(PrevDate.ReportDate());
            //headerLayout.AddCell(CurDate.ReportDate());
            return headerLayout;
        }

        #endregion

        #region Экспорт в Excel
        void MoveTitle(Worksheet w)
        {
            w.Rows[0].Cells[0].Value = w.Rows[0].Cells[0].Value;
            w.Rows[0].Cells[0].Value = "";
            
        }

        void ClearTitle(Worksheet w)
        {
            w.Rows[0].Cells[0].Value = "";
            w.Rows[1].Cells[0].Value = "";            
        }
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;            
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма 2");

            GridHeaderLayout headerLayout = GenExportLayot();
            ReportExcelExporter1.Export(headerLayout, sheet1, 7);
            UltraChart1.Width = 1000;
            UltraChart2.Width = 1000;            

            sheet4.Columns[0].Width = 300 * 100;
            sheet2.Columns[0].Width = 300 * 100;
            ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet4, 3);
            ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet2, 3);

            ClearTitle(sheet2);
            ClearTitle(sheet4);

            MoveTitle(sheet2);
            MoveTitle(sheet4);

            for(int i = 8; i<Grid.Rows.Count+8;i +=3)
            {
                sheet1.MergedCellsRegions.Add(i, 0, i + 2, 0);
            }
        }

        private void SetEmptyCellFormat(Worksheet sheet, int row, int column)
        {
            //WorksheetCell cell = sheet.Rows[9 + row * 3].Cells[column];
            //cell.CellFormat.Font.Name = "Verdana";
            //cell.CellFormat.Font.Height = 200;
            //cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            //cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.LeftBorderColor = Color.Black;
            //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.RightBorderColor = Color.Black;
            //cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.TopBorderColor = Color.Black;
            //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
            //cell.CellFormat.BottomBorderColor = Color.Black;

            //cell = sheet.Rows[10 + row * 3].Cells[column];
            //cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.LeftBorderColor = Color.Black;
            //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.RightBorderColor = Color.Black;
            //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
            //cell.CellFormat.BottomBorderColor = Color.Black;
            //cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
            //cell.CellFormat.TopBorderColor = Color.Black;

            //cell = sheet.Rows[11 + row * 3].Cells[column];
            //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.BottomBorderColor = Color.Black;
            //cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.LeftBorderColor = Color.Black;
            //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.RightBorderColor = Color.Black;
            //cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
            //cell.CellFormat.TopBorderColor = Color.Black;
        }

        private void SetCellFormat(WorksheetCell cell)
        {
            //cell.CellFormat.Font.Name = "Verdana";
            //cell.CellFormat.Font.Height = 200;
            //cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.BottomBorderColor = Color.Black;
            //cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.LeftBorderColor = Color.Black;
            //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.RightBorderColor = Color.Black;
            //cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
            //cell.CellFormat.TopBorderColor = Color.Black;
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            //e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            //e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            //e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            //e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            //e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            //e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            //string exportFontName = "Verdana";
            //int fontSize = 10;
            //double coeff = 1.0;
            //foreach (UltraGridColumn column in grid.Columns)
            //{
            //    //column.Width = Convert.ToInt32(column.Width.Value * coeff);
            //    column.CellStyle.Font.Name = exportFontName;
            //    column.Header.Style.Font.Name = exportFontName;
            //    column.CellStyle.Font.Size = fontSize;
            //    column.Header.Style.Font.Size = fontSize;
            //}
        }

        #endregion       
        
       
    }
}