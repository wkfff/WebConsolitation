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

/**
 *  Мониторинг ситуации на рынке труда в субъекте РФ по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0009_Novosib
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
                    col.Width = onePercent * (100 - 21) / 12;
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
                    col.Width = onePercent * (100 - 21) / (12);
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
                string Key = BaseKey;

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
                        this.AddItem(Parent, 0, string.Empty);
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
                foreach (Node childNode in node.Nodes)
                {
                    Result += "," + GenStringFromAllChildrenNode(childNode);
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
            //ComboRegion.Title = "Субьект РФ";



            //ComboCurrentPeriod.ShowSelectedValue = false;
            
            ComboTypePeriod.Title = "Период для сравнения";

            //DataSetComboRegion = new SetComboRegion();
            //DataSetComboRegion.LoadData(DBHelper.ExecQueryByID("ComboRegion"));
            //if (!Page.IsPostBack)
            //{
            //    ComboRegion.FillDictionaryValues(DataSetComboRegion.DataForCombo);
            //    ComboRegion.SetСheckedState(RegionSettingsHelper.Instance.Name, true);
            //}

            ComboCurrentPeriod.ShowSelectedValue = true;

            

            ComboCurrentPeriod.MultiSelect = true;


            ComboCurrentPeriod.Width = 250;
            ComboTypePeriod.Width = 350;

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

            ComboCurrentPeriod.Title = "Период";
            ComboCurrentPeriod.PanelHeaderTitle = "Период:";
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
            catch { return new CoolDate(new DateTime(1990, 10, 10)); }
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

        private object GetRang(string Field, string Date)
        {
            DateForRang.Value = Date;
            FieldForRang.Value = Field;
            int index = 0;
            DataTable TableRang = DBHelper.ExecQueryByID("GridRang", "Region");
            foreach (DataRow Row in TableRang.Rows)
            {
                index++;
                //if (ComboRegion.SelectedValue == Row["Region"].ToString())
                //{
                //    return index;
                //}
            }
            return DBNull.Value;
        }

        class InfoRow
        {
            public string ReportDisplay;
            public string FormatString;
            public string Hint = "";
            public bool Bul;

            public InfoRow(string BaseName)
            {
                switch (BaseName)
                {
                    case "Численность пенсионеров":
                        {
                            ReportDisplay = "Численность пенсионеров, чел.";
                            FormatString = "N0";
                            Bul = false;
                            return;
                        }
                    case "Численность пенсионеров, получающих трудовые пенсии":
                        {
                            ReportDisplay = "Численность пенсионеров, получающих трудовые пенсии, чел.";
                            FormatString = "N0";
                            Bul = false;
                            return;
                        }

                    case "Численность пенсионеров, получающих социальные пенсии":
                        {
                            ReportDisplay = "Численность пенсионеров, получающих социальные пенсии, чел.";
                            FormatString = "N0";
                            Bul = false;
                            return;
                        }
                    case "Средний размер пенсии":
                        {
                            ReportDisplay = "Средний размер пенсии, руб.";
                            FormatString = "N2";
                            Bul = false;
                            return;
                        }
                    case "Средний размер трудовой пенсии":
                        {
                            ReportDisplay = "Средний размер трудовой пенсии, руб.";
                            FormatString = "N2";
                            Bul = false;
                            return;
                        }
                    case "Средний размер социальной пенсии":
                        {
                            ReportDisplay = "Средний размер социальной пенсии, руб.";
                            FormatString = "N2";
                            Bul = false;
                            return;
                        }
                    case "Соотношение среднего размера пенсии и величины прожиточного минимума пенсионера":
                        {
                            ReportDisplay = "Соотношение среднего размера пенсии и величины прожиточного минимума пенсионера, %";
                            FormatString = "N2";
                            Bul = true;
                            Hint = "Средний размер пенсии {0} величины прожиточного минимума пенсионера на {1}%";
                            return;
                        }
                    case "Соотношение среднего размера пенсии и средней заработной платы":
                        {
                            ReportDisplay = "Соотношение среднего размера пенсии и средней заработной платы, %";
                            FormatString = "N2";
                            Hint = "Средний размер пенсии {0} средней заработной платы на {1}%";
                            Bul = true;
                            return;
                        }
                };
            }
        }

        class InfoRow_
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

            public InfoRow_(string BaseName)
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
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (за предшествующий период года), %";
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
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (за предшествующий период года), %";
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

                    case "Уровень трудоустройства ищущих работу граждан (за предшествующий период года), %":
                        {
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (за предшествующий период года), %";
                            Caption = "Уровень трудоустройства ищущих работу граждан (за предшествующий период года)";//, %
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
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (за предшествующий период года), %";
                            FormatString = "N2";
                            Rang = true;
                            Revece = false;
                            ReveceRang = true;
                            Caption = "Уровень трудоустройства ищущих работу граждан (за предшествующий период года)";//, %
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

                DataRow CompareRow = GetRowFromValueFirstCell(BaseTableCompare, Field);

                InfoRow RowInfo = new InfoRow(Field);

                for (int i = 1; i < BaseTableSelect.Columns.Count; i++)
                {
                    
                    GridRowValue[i] = BaseRow[i];

                    if (BaseTableSelect.Columns[i].Caption != BaseTableCompare.Columns[i].Caption)
                    {
                        GridRowDeviation[i] = GetDeviation(BaseRow[i], CompareRow[i]);

                        GridRowSpeedDeviation[i] = GetSpeedDeviation(BaseRow[i], CompareRow[i]);
                    }
                }
            }

            Grid.DataSource = TableGrid;
            Grid.DataBind();
        }

        private DataRow GetRowFromValueFirstCell(DataTable BaseTable, string Field)
        {
            foreach (DataRow row in BaseTable.Rows)
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
                RangCell.Title = ReverceRang ?
                    string.Format("Ранг по ФО: {0}\nСамое высокое значение показателя", RangCell.Value) :
                    string.Format("Ранг по ФО: {0}\nСамый низкое значение показателя", RangCell.Value);
                SetImageFromCell(RangCell, "starYellowBB.png");
            }
        }

        private void SetMinRang(UltraGridCell ValueCell, bool Max, bool ReverceRang)
        {
            UltraGridCell RangCell = GetNextCell(ValueCell);
            if (RangCell.Value != null)
            {
                RangCell.Title = !ReverceRang ?
                    string.Format("Ранг по ФО: {0}\nСамое высокое значение показателя", RangCell.Value) :
                    string.Format("Ранг по ФО: {0}\nСамый низкое значение показателя", RangCell.Value);
                SetImageFromCell(RangCell, "starGrayBB.png");
            }
        }
        void SetFormatCell1(UltraGridCell Cell, string format)
        {
            if (Cell.Value != null)
            {
                Cell.Text = string.Format("{0:" + format + "}%", decimal.Parse(Cell.Text));
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
                cell.Title = string.Format("Ранг по ФО на {0}: {1}", BaseTableCompare.Columns[cell.Column.Index].ColumnName, cell.Value);
                cell.Text = "Ранг: "+cell.Text;
            }
        }
        private void SetIndicatorDeviationcell(UltraGridCell ValueCell, bool p)
        {
            UltraGridCell IndicatorCell = GetNextCell(ValueCell);
            if (IndicatorCell.Value != null)
            {

                decimal Value = decimal.Parse(IndicatorCell.Value.ToString());
                if (Value != 0)
                {
                    //.if.string UpOrdouwn = Value > 0 ? UpOrdouwn = "Up" : UpOrdouwn = "Down";

                    string TitleCaption = Value > 0 ? "Прирост" : "Снижение";

                    IndicatorCell.Title = string.Format("{0} к {1}", TitleCaption, BaseTableCompare.Columns[ValueCell.Column.Index].ColumnName);

                    //string Color = "Grey";
                    //if ((Value > 0) & (reverce))
                    //{
                    //    Color = "Grey";
                    //}

                    //if ((Value < 0) & (!reverce))
                    //{
                    //    Color = "Grey";
                    //}
                    //SetImageFromCell(IndicatorCell, "arow" + Color + UpOrdouwn + ".png");
                }
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

                    string Color = "Grey";
                    if ((Value > 0) & (reverce))
                    {
                        Color = "Grey";
                    }

                    if ((Value < 0) & (!reverce))
                    {
                        Color = "Grey";
                    }
                    SetImageFromCell(IndicatorCell, "arow" + Color + UpOrdouwn + ".png");
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
            
            
            SetFormatCell1(GetNextCell(GetNextCell(ValueCell)), "N2");

            SetIndicatorDeviationcell(ValueCell, false);
            SetIndicatorSpeedDeviationcell(ValueCell, false);
            //GetNextCell(GetNextCell(ValueCell)).Text +="%";
                //RowInfo.FormatString);

            //string ColName = ValueCell.Column.BaseColumnName;
            //string RegionName = ValueCell.Row.Cells[0].Text;

            //if (RowInfo.Rang)
            //{
            //    SetFormatRang(GetNextCell(ValueCell));

            //    //if (GetNextCell(ValueCell).Text == ComboRegion.SelectedNode.Parent.Nodes.Count.ToString())
            //    //{
            //    //    SetMaxRang(ValueCell, true, RowInfo.ReveceRang);
            //    //}
            //    if (GetNextCell(ValueCell).Text == "1")
            //    {
            //        SetMinRang(ValueCell, true, RowInfo.ReveceRang);
            //    }
            //}

            
                //RowInfo.Revece);
        }

        

        private void FormatBall(UltraGridCell Cell, InfoRow RowInfo)
        {
            string GreenOrRed = "Red";
            string Hintcaption = "ниже";

            decimal HintValue = 100 - (decimal)Cell.Value;
            if ((decimal)Cell.Value > 100)
            {
                GreenOrRed = "Green";
                Hintcaption = "выше";
                HintValue = (decimal)Cell.Value - 100;
            }


            SetFormatCell(Cell, RowInfo.FormatString);

            string ImageName = "ball" + GreenOrRed + "BB.png";

            string ImagePath = "~/images/" + ImageName;

            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            Cell.Style.BackgroundImage = ImagePath;

            Cell.Title = string.Format(RowInfo.Hint, Hintcaption, HintValue);
        }


        private void FormatRow()
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells.FromKey("Field").Value != null)
                {
                    InfoRow RowInfo = new InfoRow(Row.Cells.FromKey("Field").Text);
                    Row.Cells[0].Text = RowInfo.ReportDisplay;
                    if (RowInfo.Bul)
                    {
                        Row.NextRow.NextRow.Hidden = true;
                        Row.NextRow.Hidden = true;
                    }
                    foreach (UltraGridCell Cell in Row.Cells)
                    {
                        
                        if (RowInfo.Bul)
                        {
                            if (Cell.Column.BaseColumnName != "Field")
                            {
                                FormatBall(Cell, RowInfo);
                            }
                            else
                            {
                                Cell.Style.Wrap = true;
                            }
                        }
                        else
                        {
                            if (Cell.Column.BaseColumnName != "Field")
                            {
                                FormatValueCell(Cell, RowInfo);
                            }
                            else
                            {
                                Cell.RowSpan = 3;
                                Cell.Style.Wrap = true;
                            }
                        }
                    }
                }
            }
        }

        private void CustomizeOtherGridProperty()
        {
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;
            //Grid.DisplayLayout.CellClickActionDefault = CellClickAction.Edit;
        }

        private void CustomizeGrid()
        {
            ConfHeader();
            FormatRow();
            CustomizeOtherGridProperty();
            CustomizerSize.ContfigurationGrid(Grid);
            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }

            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;

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
            SetHeader();
        }

        private void FillNaviagatorPanel()
        {
            CoolTable TableListForChart = new CoolTable();
            TableListForChart.Columns.Add("Field");
            TableListForChart.Columns.Add("UName");
            TableListForChart.AddOrGetRow("Уровень регистрируемой безработицы, % от численности экономически активного населения")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень регистрируемой безработицы, % от численности экономически активного населения]";
            TableListForChart.AddOrGetRow("Уровень трудоустройства ищущих работу граждан (за предшествующий период года), %")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень трудоустройства ищущих работу граждан]";
            NaviagatorChart = new RadioButtons(TableListForChart, rb_CheckedChanged);
        }

        
        private void SetHeader()
        {
            PageTitle.Text = string.Format("Пенсионное обеспечение граждан");
            PageSubTitle.Text = string.Format("Данные ежеквартального мониторинга пенсионного обеспечения граждан Новосибирской области");
            Page.Title = PageTitle.Text;

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
            }
            catch { }
                FillCombo();
                GenerationGrid();
                FillNaviagatorPanel();
                ChangeMapAndChartParam();

                GenerateText();
                SetHeader();         


        }


        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();
            //ISection section2 = report.AddSection();
            //ISection section3 = report.AddSection();


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

            //section2.PageSize = new PageSize(section1.PageSize.Height, section1.PageSize.Width);
            //DundasMap1.Height = 700;
            
        }

        private GridHeaderLayout GenExportLayot()
        {
            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            foreach (UltraGridColumn Col in Grid.Columns)
            {
                //Col.Width = 200;
                headerLayout.AddCell(Col.Header.Caption);

            }

            //foreach (UltraGridRow Row in Grid.Rows)
            //{
            //    if (Row.Cells[0].Text.Contains(";"))
            //    {
            //        Row.Cells[0].Value = null;
            //    }

            //}

            headerLayout.ApplyHeaderInfo();

            //headerLayout.AddCell("Показатель");
            //headerLayout.AddCell(PrevDate.ReportDate());
            //headerLayout.AddCell(CurDate.ReportDate());
            return headerLayout;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();    
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            //Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            //Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма");

            //ReportExcelExporter1.HeaderCellHeight = 80;
            //ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            //ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            //ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            //ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            //ReportExcelExporter1.TitleStartRow = 0;

            GridHeaderLayout headerLayout = GenExportLayot();
            ReportExcelExporter1.Export(headerLayout, sheet1, 7);
            //UltraChart1.Width = 1000;
            //UltraChart2.Width = 1000;


            //ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet4, 3);
            //ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet2, 3);
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

        #region old
        //        ICustomizerSize CustomizerSize;

//        IDataSetCombo DataSetComboDay;

//        #region Подонка размеров элементов, под браузер и разрешение
//        abstract class ICustomizerSize
//        {
//            public enum BrouseName { IE, FireFox, SafariOrHrome }

//            public enum ScreenResolution { _800x600, _1280x1024, Default }

//            public BrouseName NameBrouse;

//            public abstract int GetGridWidth();
//            public abstract int GetChartWidth();

//            public abstract int GetMapHeight();

//            protected abstract void ContfigurationGridIE(UltraWebGrid Grid);
//            protected abstract void ContfigurationGridFireFox(UltraWebGrid Grid);
//            protected abstract void ContfigurationGridGoogle(UltraWebGrid Grid);

//            public virtual void ContfigurationGrid(UltraWebGrid Grid)
//            {
//                switch (this.NameBrouse)
//                {
//                    case BrouseName.IE:
//                        this.ContfigurationGridIE(Grid);
//                        return;
//                    case BrouseName.FireFox:
//                        this.ContfigurationGridFireFox(Grid);
//                        return;
//                    case BrouseName.SafariOrHrome:
//                        this.ContfigurationGridGoogle(Grid);
//                        return;
//                    default:
//                        throw new Exception("Охо!");
//                }
//            }

//            public ICustomizerSize(BrouseName NameBrouse)
//            {
//                this.NameBrouse = NameBrouse;
//            }

//            public static BrouseName GetBrouse(string _BrouseName)
//            {
//                if (_BrouseName == "IE") { return BrouseName.IE; };
//                if (_BrouseName == "FIREFOX") { return BrouseName.FireFox; };
//                if (_BrouseName == "APPLEMAC-SAFARI") { return BrouseName.SafariOrHrome; };

//                return BrouseName.IE;
//            }

//            public static ScreenResolution GetScreenResolution(int Width)
//            {


//                if (Width < 801)
//                {
//                    return ScreenResolution._800x600;
//                }
//                if (Width < 1281)
//                {
//                    return ScreenResolution._1280x1024;
//                }
//                return ScreenResolution.Default;
//            }
//        }

//        class CustomizerSize_800x600 : ICustomizerSize
//        {
//            public override int GetGridWidth()
//            {
//                switch (this.NameBrouse)
//                {
//                    case BrouseName.IE:
//                        return 740;
//                    case BrouseName.FireFox:
//                        return 750;
//                    case BrouseName.SafariOrHrome:
//                        return 755;
//                    default:
//                        return 800;
//                }
//            }

//            public override int GetChartWidth()
//            {
//                switch (this.NameBrouse)
//                {
//                    case BrouseName.IE:
//                        return 740;
//                    case BrouseName.FireFox:
//                        return 750;
//                    case BrouseName.SafariOrHrome:
//                        return 750;
//                    default:
//                        return 800;
//                }
//            }

//            public CustomizerSize_800x600(BrouseName NameBrouse) : base(NameBrouse) { }

//            #region Настрока размера колонок для грида
//            protected override void ContfigurationGridIE(UltraWebGrid Grid)
//            {
//                int onePercent = (int)Grid.Width.Value / 100;
//                foreach (UltraGridColumn col in Grid.Columns)
//                {
//                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
//                }

//                Grid.Columns[0].Width = onePercent * 18;
//            }

//            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
//            {
//                int onePercent = (int)Grid.Width.Value / 100;
//                foreach (UltraGridColumn col in Grid.Columns)
//                {
//                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
//                }

//                Grid.Columns[0].Width = onePercent * 18;
//            }

//            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
//            {
//                int onePercent = (int)Grid.Width.Value / 100;
//                foreach (UltraGridColumn col in Grid.Columns)
//                {
//                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
//                }

//                Grid.Columns[0].Width = onePercent * 18;
//            }

//            #endregion


//            public override int GetMapHeight()
//            {
//                return 705;
//            }
//        }

//        class CustomizerSize_1280x1024 : ICustomizerSize
//        {
//            public override int GetGridWidth()
//            {
//                switch (this.NameBrouse)
//                {
//                    case BrouseName.IE:
//                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
//                    case BrouseName.FireFox:
//                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
//                    case BrouseName.SafariOrHrome:
//                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20).Value;
//                    default:
//                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 100).Value;
//                }
//            }

//            public override int GetChartWidth()
//            {
//                switch (this.NameBrouse)
//                {
//                    case BrouseName.IE:
//                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
//                    case BrouseName.FireFox:
//                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
//                    case BrouseName.SafariOrHrome:
//                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
//                    default:
//                        return (int)CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
//                }
//            }

//            public CustomizerSize_1280x1024(BrouseName NameBrouse) : base(NameBrouse) { }

//            #region Настрока размера колонок для грида
//            protected override void ContfigurationGridIE(UltraWebGrid Grid)
//            {
//                int onePercent = (int)Grid.Width.Value / 100;
//                Grid.Columns[0].Width = onePercent * 30;
//                Grid.Columns[1].Width = onePercent * 10;
//                Grid.Columns[2].Width = onePercent * 10;
//            }

//            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
//            {
//                int onePercent = (int)Grid.Width.Value / 100;
//                Grid.Columns[0].Width = onePercent * 30;
//                Grid.Columns[1].Width = onePercent * 10;
//                Grid.Columns[2].Width = onePercent * 10;
//            }

//            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
//            {
//                int onePercent = (int)Grid.Width.Value / 100;
//                Grid.Columns[0].Width = onePercent * 30;
//                Grid.Columns[1].Width = onePercent * 10;
//                Grid.Columns[2].Width = onePercent * 10;
//            }

//            #endregion


//            public override int GetMapHeight()
//            {
//                return 705;
//            }
//        }
//        #endregion

//        DataBaseHelper DBHelper;

//        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
//        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }
//        CustomParam ChosenPrevPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPrevPeriod")); } }

//        class DataBaseHelper
//        {
//            public DataProvider ActiveDP;

//            public DataTable ExecQueryByID(string QueryId)
//            {
//                return ExecQueryByID(QueryId, QueryId);
//            }

//            public DataTable ExecQueryByID(string QueryId, string FirstColName)
//            {
//                string QueryText = DataProvider.GetQueryText(QueryId);
//                DataTable Table = ExecQuery(QueryText, FirstColName);
//                return Table;
//            }

//            public DataTable ExecQuery(string QueryText, string FirstColName)
//            {
//                DataTable Table = new DataTable();
//                ActiveDP.GetDataTableForChart(QueryText, FirstColName, Table);
//                return Table;
//            }

//            public DataBaseHelper(DataProvider ActiveDataProvaider)
//            {
//                this.ActiveDP = ActiveDataProvaider;
//            }

//        }

//        #region ComboClass
//        abstract class IDataSetCombo
//        {
//            public enum TypeFillData
//            {
//                Linear, TwoLevel
//            }

//            public int LevelParent = 2;

//            public string LastAdededKey { get; protected set; }

//            public readonly Dictionary<string, string> DataUniqeName = new Dictionary<string, string>();

//            public readonly Dictionary<string, int> DataForCombo = new Dictionary<string, int>();

//            public readonly Dictionary<string, Dictionary<string, string>> OtherInfo = new Dictionary<string, Dictionary<string, string>>();

//            protected void addOtherInfo(DataTable Table, DataRow Row, string Key)
//            {
//                Dictionary<string, string> Info = new Dictionary<string, string>();
//                foreach (DataColumn col in Table.Columns)
//                {
//                    Info.Add(col.ColumnName, Row[col].ToString());
//                }
//                OtherInfo.Add(Key, Info);
//            }

//            public delegate string FormatedDispalyText(string ParentName, string ChildrenName);
//            protected FormatedDispalyText FormatingText;
//            public void SetFormatterText(FormatedDispalyText FormatterText)
//            {
//                FormatingText = FormatterText;
//            }

//            public delegate string FormatedDispalyTextParent(string ParentName);
//            protected FormatedDispalyTextParent FormatingTextParent;
//            public void SetFormatterTextParent(FormatedDispalyTextParent FormatterText)
//            {
//                FormatingTextParent = FormatterText;
//            }

//            protected string GetAlowableAndFormatedKey(string BaseKey, string Parent)
//            {
//                string Key = BaseKey;//FormatingText(Parent, BaseKey);

//                while (DataForCombo.ContainsKey(Key))
//                {
//                    Key += " ";
//                }

//                return Key;
//            }

//            protected void AddItem(string Child, int level, string UniqueName)
//            {

//                string RealChild = Child;

//                DataForCombo.Add(RealChild, level);

//                DataUniqeName.Add(RealChild, UniqueName);

//                this.LastAdededKey = RealChild;
//            }

//            public abstract void LoadData(DataTable Table);

//        }

//        class SetComboDay : IDataSetCombo
//        {
//            public override void LoadData(DataTable Table)
//            {
//                string LaseYear = "";

//                foreach (DataRow row in Table.Rows)
//                {
//                    string UniqueName = row["UniqueName"].ToString();

//                    string Day = row["Day"].ToString();

//                    string Mounth = row["Mounth"].ToString();

//                    string Year = row["Year"].ToString();

//                    string DisplayNAme = this.GetAlowableAndFormatedKey(Day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(Mounth)) + " " + Year + " года", "");

//                    if (LaseYear != Year)
//                    {
//                        this.AddItem(Year + " год", 0, UniqueName);
//                    }

//                    LaseYear = Year;

//                    this.AddItem(DisplayNAme, 1, UniqueName);

//                    this.addOtherInfo(Table, row, DisplayNAme);
//                }
//            }
//        }

//        #endregion

//        protected override void Page_PreLoad(object sender, EventArgs e)
//        {
//            base.Page_PreLoad(sender, e);

//            DBHelper = new DataBaseHelper(DataProvidersFactory.PrimaryMASDataProvider);

//            #region ini CustomizerSize
//            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;

//            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

//            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
//            {
//                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
//            }
//            else

//                if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._800x600)
//                {
//                    CustomizerSize = new CustomizerSize_800x600(ICustomizerSize.GetBrouse(BN));
//                }
//                else
//                {
//                    CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
//                }

//            Grid.Width = CustomizerSize.GetGridWidth();
//            Grid.Height = Unit.Empty;
//            #endregion

//            #region Экспорт

//            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
//            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
//            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

//            #endregion
//        }

//        private void FillComboPeriod()
//        {
//            ComboDay.ParentSelect = false;

//            DataTable Table = DBHelper.ExecQueryByID("ComboDates", "Day");

//            DataSetComboDay = new SetComboDay();
//            DataSetComboDay.LoadData(Table);

//            if (!Page.IsPostBack)
//            {
//                ComboDay.FillDictionaryValues(DataSetComboDay.DataForCombo);
//                ComboDay.SetСheckedState(DataSetComboDay.LastAdededKey, 1 == 1);
//            }
//        }

//        private void FillComboTypePeriod()
//        {
//            if (!Page.IsPostBack)
//            {
//                Dictionary<string, int> DictTypePeriod = new Dictionary<string, int>();
//                DictTypePeriod.Add("К предыдущей дате", 0);
//                DictTypePeriod.Add("К началу года", 0);
//                DictTypePeriod.Add("К аналогичной дате предыдущего года", 0);

//                ComboTypePeriod.FillDictionaryValues(DictTypePeriod);
//            }
//        }

//        private void FillCombo()
//        {
//            FillComboPeriod();
//            FillComboTypePeriod();
//        }

//        class CoolDate
//        {
//            public DateTime dt;

//            public CoolDate(DateTime dt)
//            {
//                this.dt = dt;
//            }

//            public CoolDate(string MDXName)
//            {
//                this.dt = ConvertToDateTime(MDXName);
//            }

//            private DateTime ConvertToDateTime(string MDXName)
//            {
//                string[] firstsplit = MDXName.Replace("[", "").Replace("]", "").Split('.');
//                int Year = int.Parse(firstsplit[3]);
//                int Mounth = CRHelper.MonthNum(firstsplit[6]);
//                int Day = int.Parse(firstsplit[7]);
//                return new DateTime(Year, Mounth, Day);

//            }

//            public string ConvertToMDXName()
//            {
//                return string.Format(@"[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]",
//                    dt.Year,
//                CRHelper.HalfYearNumByMonthNum(dt.Month),
//                CRHelper.QuarterNumByMonthNum(dt.Month),
//                CRHelper.RusMonth(dt.Month),
//                dt.Day);


//            }

//            public string ReportDate()
//            {
//                return string.Format("{0:00}.{1:00}.{2:00}", dt.Day, dt.Month, dt.Year);
//            }
//        }

//        Node GetPrevNode(Node n)
//        {
//            if (n.PrevNode == null)
//            {
//                if (n.Parent.PrevNode == null)
//                {
//                    return null;
//                }
//                return n.Parent.PrevNode.Nodes[n.Parent.PrevNode.Nodes.Count - 1];
//            }
//            return n.PrevNode;

//        }
//        CoolDate CurDate;
//        CoolDate PrevDate;
//        CoolDate PrevPrevDate;

//        #region GetCompareDate
//        private CoolDate ConstructPrevDate(CoolDate cd)
//        {
//            return new CoolDate(cd.dt.AddMonths(-1));
//        }

//        private void GetCompareDate()
//        {
//            string UNameCurDate = DataSetComboDay.DataUniqeName[ComboDay.SelectedValue];
//            CurDate = new CoolDate(UNameCurDate);
//            if (ComboTypePeriod.SelectedValue == "К предыдущей дате")
//            {
//                Node PrevNode = GetPrevNode(ComboDay.SelectedNode);
//                if (PrevNode != null)
//                {
//                    PrevDate = new CoolDate(DataSetComboDay.DataUniqeName[PrevNode.Text]);
//                    PrevNode = GetPrevNode(PrevNode);
//                    if (PrevNode != null)
//                    {
//                        PrevPrevDate = new CoolDate(DataSetComboDay.DataUniqeName[PrevNode.Text]);
//                    }
//                    else
//                    {
//                        PrevPrevDate = ConstructPrevDate(PrevDate);
//                    }
//                }
//                else
//                {
//                    PrevDate = ConstructPrevDate(CurDate);
//                    PrevPrevDate = ConstructPrevDate(PrevDate);
//                }
//            }
//            else
//                if (ComboTypePeriod.SelectedValue == "К началу года")
//                {
//                    PrevDate = new CoolDate(new DateTime(CurDate.dt.Year, 1, 1));
//                    PrevPrevDate = new CoolDate(new DateTime(CurDate.dt.Year - 20, CurDate.dt.Month, CurDate.dt.Day));
//                }
//                else
//                {
//                    PrevDate = new CoolDate(new DateTime(CurDate.dt.Year - 1, CurDate.dt.Month, CurDate.dt.Day));
//                    PrevPrevDate = new CoolDate(new DateTime(CurDate.dt.Year - 2, CurDate.dt.Month, CurDate.dt.Day));
//                }
//        }
//        #endregion

//        private void SetQueryParam()
//        {
//            ChosenCurPeriod.Value = CurDate.ConvertToMDXName();
//            ChosenPrevPeriod.Value = PrevDate.ConvertToMDXName();
//            ChosenPrevPrevPeriod.Value = PrevPrevDate.ConvertToMDXName();
//        }

//        class CoolTable : DataTable
//        {
//            public DataRow AddRow()
//            {
//                DataRow NewRow = this.NewRow();
//                this.Rows.Add(NewRow);
//                return NewRow;
//            }
//        }

//        private object GetDeviation(DataRow BaseRow, DataColumn ColCurValue, DataColumn ColPrevValue)
//        {
//            object CurValue = BaseRow[ColCurValue];
//            object PrevValue = BaseRow[ColPrevValue];

//            if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
//            {
//                return (decimal)CurValue - (decimal)PrevValue;
//            }
//            return DBNull.Value;
//        }

//        private object GetSpeedDeviation(DataRow BaseRow, DataColumn ColCurValue, DataColumn ColPrevValue)
//        {
//            object CurValue = BaseRow[ColCurValue];
//            object PrevValue = BaseRow[ColPrevValue];

//            if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
//            {
//                return 100 * ((decimal)CurValue / (decimal)PrevValue - 1);
//            }
//            return DBNull.Value;
//        }

//        class InfoRow
//        {
//            public string ReportDisplay;
//            public string FormatString;
//            public string Hint = "";
//            public bool Bul;

//            public InfoRow(string BaseName)
//            {
//                switch (BaseName)
//                {
//                    case "Численность пенсионеров":
//                        {
//                            ReportDisplay = "Численность пенсионеров, чел.";
//                            FormatString = "N0";
//                            Bul = false;
//                            return;
//                        }
//                    case "Численность пенсионеров, получающих трудовые пенсии":
//                        {
//                            ReportDisplay = "Численность пенсионеров, получающих трудовые пенсии, чел.";
//                            FormatString = "N0";
//                            Bul = false;
//                            return;
//                        }

//                    case "Численность пенсионеров, получающих социальные пенсии":
//                        {
//                            ReportDisplay = "Численность пенсионеров, получающих социальные пенсии, чел.";
//                            FormatString = "N0";
//                            Bul = false;
//                            return;
//                        }
//                    case "Средний размер пенсии":
//                        {
//                            ReportDisplay = "Средний размер пенсии, руб.";
//                            FormatString = "N2";
//                            Bul = false;
//                            return;
//                        }
//                    case "Средний размер трудовой пенсии":
//                        {
//                            ReportDisplay = "Средний размер трудовой пенсии, руб.";
//                            FormatString = "N2";
//                            Bul = false;
//                            return;
//                        }
//                    case "Средний размер социальной пенсии":
//                        {
//                            ReportDisplay = "Средний размер социальной пенсии, руб.";
//                            FormatString = "N2";
//                            Bul = false;
//                            return;
//                        }
//                    case "Соотношение среднего размера пенсии и величины прожиточного минимума пенсионера":
//                        {
//                            ReportDisplay = "Соотношение среднего размера пенсии и величины прожиточного минимума пенсионера, %";
//                            FormatString = "N2";
//                            Bul = true;
//                            Hint = "Средний размер пенсии {0} величины прожиточного минимума пенсионера на {1}%";
//                            return;
//                        }
//                    case "Соотношение среднего размера пенсии и средней заработной платы":
//                        {
//                            ReportDisplay = "Соотношение среднего размера пенсии и средней заработной платы, %";
//                            FormatString = "N2";
//                            Hint = "Средний размер пенсии {0} средней заработной платы на {1}%";
//                            Bul = true;
//                            return;
//                        }
//                };
//            }
//        }

//        private void DataBindGrid()
//        {
//            DataTable Basetable = DBHelper.ExecQueryByID("Grid", "Field");
//            CoolTable GridTable = new CoolTable();

//            GridTable.Columns.Add("Field", typeof(string));
//            GridTable.Columns.Add("IndicatorCompare", typeof(decimal));
//            GridTable.Columns.Add("CompareDate", typeof(decimal));
//            GridTable.Columns.Add("IndicatorSelect", typeof(decimal));
//            GridTable.Columns.Add("SelectDate", typeof(decimal));


//            foreach (DataRow BaseRow in Basetable.Rows)
//            {
//                DataRow ValueRow = GridTable.AddRow();

//                ValueRow["Field"] = BaseRow["Field"];
//                ValueRow["SelectDate"] = BaseRow["CurDate"];
//                ValueRow["CompareDate"] = BaseRow["PrevDate"];

//                if (!new InfoRow(BaseRow["Field"].ToString()).Bul)
//                {
//                    DataRow DeviationRow = GridTable.AddRow();
//                    DataRow SpeedDeviationRow = GridTable.AddRow();

//                    DeviationRow["Field"] = DBNull.Value;
//                    DeviationRow["SelectDate"] = GetDeviation(BaseRow, Basetable.Columns["CurDate"], Basetable.Columns["PrevDate"]);
//                    DeviationRow["CompareDate"] = GetDeviation(BaseRow, Basetable.Columns["PrevDate"], Basetable.Columns["PrevPrevDate"]);

//                    SpeedDeviationRow["Field"] = DBNull.Value;
//                    SpeedDeviationRow["SelectDate"] = GetSpeedDeviation(BaseRow, Basetable.Columns["CurDate"], Basetable.Columns["PrevDate"]);
//                    SpeedDeviationRow["CompareDate"] = GetSpeedDeviation(BaseRow, Basetable.Columns["PrevDate"], Basetable.Columns["PrevPrevDate"]);
//                }
//            }

//            Grid.DataSource = GridTable;
//            Grid.DataBind();
//        }

//        protected void SetDefaultStyleHeader(ColumnHeader header)
//        {
//            GridItemStyle HeaderStyle = header.Style;

//            HeaderStyle.Wrap = true;

//            HeaderStyle.VerticalAlign = VerticalAlign.Middle;

//            HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
//        }

//        private ColumnHeader GenHeader(string Caption, int x, int y, int spanX, int SpanY)
//        {
//            ColumnHeader Header = new ColumnHeader();

//            Header.Caption = Caption;
//            Header.RowLayoutColumnInfo.OriginX = x;
//            Header.RowLayoutColumnInfo.OriginY = y;
//            Header.RowLayoutColumnInfo.SpanX = spanX;
//            Header.RowLayoutColumnInfo.SpanY = SpanY;
//            SetDefaultStyleHeader(Header);

//            return Header;
//        }

//        private void ConfigHeader(ColumnHeader ch, int x, int y, int spanX, int spanY, string Caption)
//        {
//            ch.RowLayoutColumnInfo.OriginX = x;
//            ch.RowLayoutColumnInfo.OriginY = y;
//            ch.RowLayoutColumnInfo.SpanX = spanX;
//            ch.RowLayoutColumnInfo.SpanY = spanY;
//            ch.Caption = Caption;
//            SetDefaultStyleHeader(ch);
//            if (ch.Column.Index != 0)
//            {
//                CRHelper.FormatNumberColumn(ch.Column, "N2");
//            }
//            else
//            {
//                ch.Column.CellStyle.Wrap = true;
//            }
//        }

//        private void ConfHeader()
//        {
//            ConfigHeader(Grid.Columns.FromKey("Field").Header, 0, 0, 1, 1, "Показатель");
//            ConfigHeader(Grid.Columns.FromKey("IndicatorCompare").Header, 1, 0, 2, 1, PrevDate.ReportDate());
//            ConfigHeader(Grid.Columns.FromKey("IndicatorSelect").Header, 3, 0, 2, 1, CurDate.ReportDate());
//        }

//        private void ConfCol()
//        {
//            Grid.Columns.FromKey("Field").CellStyle.Wrap = true;
//            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("SelectDate"), "N2");
//            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("CompareDate"), "N2");
//        }

//        private UltraGridCell GetNextCell(UltraGridCell Cell)
//        {
//            return Cell.Row.NextRow.Cells.FromKey(Cell.Column.BaseColumnName);
//        }

//        private void FormatValueCell(UltraGridCell ValueCell, InfoRow RowInfo, UltraGridCell IndicatorCell)
//        {
//            if (ValueCell.Value == null)
//            {
//                return;
//            }

//            if (RowInfo.Bul)
//            {
//                string GreenOrRed = "Red";
//                string Hintcaption = "ниже";
                
//                decimal HintValue = 100 - (decimal)ValueCell.Value;
//                if ((decimal)ValueCell.Value > 100)
//                {
//                    GreenOrRed = "Green";
//                    Hintcaption = "выше";
//                    HintValue = (decimal)ValueCell.Value - 100;
//                }

//                string ImageName = "ball" + GreenOrRed + "BB.png";

//                string ImagePath = "~/images/" + ImageName;

//                IndicatorCell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center";
//                IndicatorCell.Style.BackgroundImage = ImagePath;

//                IndicatorCell.Title = string.Format(RowInfo.Hint, Hintcaption, HintValue);

//            }
//            else
//            {

//                UltraGridCell DeviationCell = GetNextCell(ValueCell);
//                UltraGridCell SpeedDeviationCell = GetNextCell(DeviationCell);

//                if (SpeedDeviationCell.Value == null)
//                {
//                    return;
//                }

//                ValueCell.Text = string.Format("{0:" + RowInfo.FormatString + "}", ValueCell.Value);

//                string UpOrDown = "Down";
//                string CaptionDeviation = "Снижение";
//                string CaptionSpeedDeviation = "снижения";

//                if ((decimal)SpeedDeviationCell.Value > 0)
//                {
//                    UpOrDown = "Up";
//                    CaptionDeviation = "Прирост";
//                    CaptionSpeedDeviation = "прироста";

//                }
//                string ImageName = "arowGrey" + UpOrDown + ".png";

//                string ImagePath = "~/images/" + ImageName;

//                DeviationCell.Title = string.Format("{1} к {0}",
//    ValueCell.Column.BaseColumnName == "SelectDate" ?
//    PrevDate.ReportDate() : PrevPrevDate.ReportDate(), CaptionDeviation);

//                DeviationCell.Text = string.Format("{0:" + RowInfo.FormatString + "}", DeviationCell.Value);

//                SpeedDeviationCell.Title = string.Format("Темп {1} к {0}",
//    ValueCell.Column.BaseColumnName == "SelectDate" ?
//    PrevDate.ReportDate() : PrevPrevDate.ReportDate(), CaptionSpeedDeviation);

//                IndicatorCell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center";
//                IndicatorCell.Style.BackgroundImage = ImagePath;
//                IndicatorCell.Title = SpeedDeviationCell.Title;
//                IndicatorCell.RowSpan = 3;

//                SpeedDeviationCell.Value = string.Format("{0:N2}%", SpeedDeviationCell.Value);

//            }
//        }

//        private void ConfRow()
//        {
//            foreach (UltraGridRow Row in Grid.Rows)
//            {
//                if (Row.Cells.FromKey("Field").Value != null)
//                {
//                    Row.Cells.FromKey("Field").RowSpan = 3;

//                    InfoRow RowInfo = new InfoRow(Row.Cells.FromKey("Field").Text);

//                    Row.Cells.FromKey("Field").Text = RowInfo.ReportDisplay;

//                    FormatValueCell(Row.Cells.FromKey("CompareDate"), RowInfo, Row.Cells.FromKey("IndicatorCompare"));
//                    FormatValueCell(Row.Cells.FromKey("SelectDate"), RowInfo, Row.Cells.FromKey("IndicatorSelect"));
//                }
//            }
//        }

//        private void OtherCustomizeGrid()
//        {
//            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;
//            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
//            Grid.DisplayLayout.NoDataMessage = "Нет данных";
//            Grid.DisplayLayout.NullTextDefault = "";
//        }

//        private void SetDisplayGrid()
//        {
//            ConfHeader();
//            ConfCol();
//            ConfRow();
//            OtherCustomizeGrid();
//            CustomizerSize.ContfigurationGrid(Grid);

//        }



//        private void GenerateGrid()
//        {
//            DataBindGrid();
//            SetDisplayGrid();
//        }

        string GetYear(string Uname)
        {
            string[] sUname = Uname.Replace("[", "").Replace("]", "").Split('.');
            return sUname[3];
        }

        private void GenerateText()
        {

            Textovko.Text = ""; 
            try
            { 
            
                DataTable TableField1 = DBHelper.ExecQueryByID("TextField1");
                string LastYear = GetYear(TableField1.Rows[0]["UNameDate"].ToString());
                decimal ValueFirstField = (decimal)TableField1.Rows[0]["Field"];

                string AbzacFirst = string.Format(@"Величина прожиточного минимума пенсионера для установления социальных доплат к пенсии на <b>{0}</b> год - <b>{1:N0}</b>  руб.", LastYear, ValueFirstField);
                Textovko.Text = AbzacFirst;
            }
            catch { }
            try
            {
            DataTable TableField2 = DBHelper.ExecQueryByID("TextField2");

            DataRow FirstRow = TableField2.Rows[1];
            DataRow LastRow = TableField2.Rows[0];
            
                string LastQuart = string.Format("{0} квартал {1} года", FirstRow["quart"].ToString().Replace("Квартал ", ""), FirstRow["year"].ToString());
                string FirstQuart = string.Format("{0} квартал {1} года", LastRow["quart"].ToString().Replace("Квартал ", ""), LastRow["year"].ToString());
                decimal ValLast = (decimal)LastRow["Величина прожиточного минимума для пенсионеров"];
                string AbzacSecond = string.Format("Величина прожиточного минимума для пенсионеров за <b>{0}</b> составила <b>{1:N0}</b> руб.", FirstQuart, ValLast);

                Textovko.Text += "<br>" + AbzacSecond; 

                try
                {
                    
                    decimal ValFirst = (decimal)FirstRow["Величина прожиточного минимума для пенсионеров"];

                    decimal Deviation = ValLast - ValFirst;
                    decimal SpeedDeviation = (ValLast / ValFirst - 1) * 100;

                    string CaptionDeviation = SpeedDeviation < 0 ? "Снижение <img src='../../images/ArrowRedDownBB.png'>" : "Прирост <img src='../../images/ArrowGreenUpBB.png'>";
                    string CaptionDeviation2 = SpeedDeviation < 0 ? "снижения" : "прироста";

                    string AbzacLast = string.Format("{0} значения показателя в сравнении со значением за <b>{1:N0}</b> составил <b>{2}</b> руб. (темп {3} <b>{4:N2}%</b>)",
                        CaptionDeviation ,
                        LastQuart,
                        Math.Abs(Deviation),
                        CaptionDeviation2,
                        Math.Abs(SpeedDeviation));

                    Textovko.Text += " " + AbzacLast;
                }
                catch { }
            }
            catch { };
        }


        #endregion
    }
}