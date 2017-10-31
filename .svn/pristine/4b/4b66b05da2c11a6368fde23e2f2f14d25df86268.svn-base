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

/**
 *  Мониторинг ситуации на рынке труда в субъекте РФ по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0010_Novosib
{
    public partial class Default : CustomReportPage
    {
        ICustomizerSize CustomizerSize;

        IDataSetCombo DataSetComboDay;

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
                int onePercent = (int)1284 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 24) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 16;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1284 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1284 / 100;
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
                    col.Width = onePercent * (100 - 24) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 16;
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
        #endregion

        DataBaseHelper DBHelper;
        DataBaseHelper DBRFHelper;

        CustomParam SelectMapAndChartParam { get { return (UserParams.CustomParam("SelectMapAndChartParam")); } }
        CustomParam SelectMapAndChartParamRF { get { return (UserParams.CustomParam("SelectMapAndChartParamRF")); } }

        CustomParam ChosenCurPeriodRF { get { return (UserParams.CustomParam("ChosenCurPeriodRF")); } }

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }
        CustomParam ChosenPrevPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPrevPeriod")); } }

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

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();

                    string Day = row["Day"].ToString();

                    string Mounth = row["Mounth"].ToString();

                    string Year = row["Year"].ToString();

                    string DisplayNAme = this.GetAlowableAndFormatedKey(Day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(Mounth)) + " " + Year + " года", "");

                    if (LaseYear != Year)
                    {
                        this.AddItem(Year + " год", 0, UniqueName);
                    }

                    LaseYear = Year;

                    this.AddItem(DisplayNAme, 1, UniqueName);

                    this.addOtherInfo(Table, row, DisplayNAme);
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
            Grid.Height = 600;
            DundasMap1.Width = CustomizerSize.GetChartWidth();
            DundasMap1.Height = 600;

            UltraChart1.Width = CustomizerSize.GetChartWidth();
            UltraChart1.Height = 500;
            
            #endregion

            #region Экспорт

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #endregion
        }

        private void FillComboPeriod()
        {
            ComboCurrentPeriod.ParentSelect = false;
            ComboCurrentPeriod.Title = "Выберите дату";
            if (CustomizerSize is CustomizerSize_800x600)
            {
                ComboCurrentPeriod.Width = 300 - 100;
                ComboComparePeriod.Width = 400 - 100;
            }
            else
            {
                ComboCurrentPeriod.Width = 300;
                ComboComparePeriod.Width = 400;
            }
            


            ComboComparePeriod.ParentSelect = false;
            ComboComparePeriod.Title = "Выберите дату для сравнения";
            

            DataTable Table = DBHelper.ExecQueryByID("ComboDates", "Day");

            DataSetComboDay = new SetComboDay();
            DataSetComboDay.LoadData(Table);

            if (!Page.IsPostBack)
            {
                ComboCurrentPeriod.FillDictionaryValues(DataSetComboDay.DataForCombo);
                ComboCurrentPeriod.SetСheckedState(DataSetComboDay.LastAdededKey, 1 == 1);
                ComboComparePeriod.FillDictionaryValues(DataSetComboDay.DataForCombo);
                ComboComparePeriod.SetСheckedState(DataSetComboDay.PrevLastAdededKey, 1 == 1);
            }
        }

        private void FillCombo()
        {
            FillComboPeriod();
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
                    //this.dt;
                    
                return string.Format(@"[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                    dt_.Year,
                CRHelper.HalfYearNumByMonthNum(dt_.Month),
                CRHelper.QuarterNumByMonthNum(dt_.Month),
                CRHelper.RusMonth(dt_.Month));
            }

        }

        Node GetPrevNode(Node n)
        {
            if (n.PrevNode == null)
            {
                if (n.Parent.PrevNode == null)
                {
                    return null;
                }
                return n.Parent.PrevNode.Nodes[n.Parent.PrevNode.Nodes.Count - 1];
            }
            return n.PrevNode;

        }
        CoolDate CurDate;
        CoolDate CompareDate;

        #region GetCompareDate
        private CoolDate ConstructPrevDate(CoolDate cd)
        {
            return new CoolDate(cd.dt.AddMonths(-1));
        }

        private void GetCompareDate()
        {
            CurDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboCurrentPeriod.SelectedValue]);
            CompareDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboComparePeriod.SelectedValue]);
            if (ComboComparePeriod.SelectedIndex > ComboCurrentPeriod.SelectedIndex)
            {
                CurDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboComparePeriod.SelectedValue]);
                CompareDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboCurrentPeriod.SelectedValue]);
            }
        }
        #endregion

        private void SetQueryParam()
        {
            ChosenCurPeriod.Value = CurDate.ConvertToMDXName();
            ChosenPrevPeriod.Value = CompareDate.ConvertToMDXName();
            ChosenCurPeriodRF.Value = CurDate.ConvertToMDXNameRF();
        }

        #region Grid
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

        private object GetDeviation(DataRow BaseRow, DataColumn ColCurValue, DataColumn ColPrevValue)
        {
            object CurValue = BaseRow[ColCurValue];
            object PrevValue = BaseRow[ColPrevValue];

            if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
            {
                return (decimal)CurValue - (decimal)PrevValue;
            }
            return DBNull.Value;
        }

        private object GetSpeedDeviation(DataRow BaseRow, DataColumn ColCurValue, DataColumn ColPrevValue)
        {
            object CurValue = BaseRow[ColCurValue];
            object PrevValue = BaseRow[ColPrevValue];

            if ((CurValue != DBNull.Value) && (PrevValue != DBNull.Value))
            {
                return 100 * ((decimal)CurValue / (decimal)PrevValue - 1);
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
                    case "Численность экономически активного населения":
                        {
                            ReportDisplay = "Численность экономически активного населения, чел.";
                            FormatString = "N0";
                            Rang = false;
                            Revece = false;
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

                            RFString = "[Труд__Трудовые ресурсы].[Труд__Трудовые ресурсы].[Все показатели].[Уровень зарегистрированной безработицы (от экономически активного населения)]*100";                            
                            

                            return;
                        }

                    case "Численность незанятых граждан, состоящих на учете":
                        {
                            ReportDisplay = "Численность незанятых граждан, состоящих на учете, чел.";
                            FormatString = "N0";
                            Rang = false;
                            Revece = true;
                            return;
                        }
                    case "Число заявленных вакансий":
                        {
                            ReportDisplay = "Число заявленных вакансий, ед.";
                            FormatString = "N0";
                            Rang = false;
                            Revece = false;
                            return;
                        }
                    case "Уровень напряженности на рынке труда ":
                        {
                            ReportDisplay = "Уровень напряженности на рынке труда, ед.";

                            Caption = "Уровень напряженности на рынке труда";
                            EdIzm = "ед.";

                            FormatString = "N2";
                            Rang = true;
                            Revece = true;
                            ReveceRang = false;

                            BeginColor = Color.Green;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Red;

                            RFString = "[Труд__Трудовые ресурсы].[Труд__Трудовые ресурсы].[Все показатели].[Уровень напряжённости на рынке труда]";

                            return;
                        }
                    case "Уровень трудоустройства ищущих работу граждан ":
                        {
                            ReportDisplay = "Уровень трудоустройства ищущих работу граждан (с начала года), %";

                            Caption = "Уровень трудоустройства ищущих работу граждан (с начала года)";
                            EdIzm = "%";
                              
                            FormatString = "N2";
                            Rang = true;
                            Revece = false;
                            ReveceRang = true;

                            BeginColor = Color.Red;
                            CenterColor = Color.Yellow;
                            EndColor = Color.Green;

                            RFString = "Ля ля ля";
                            return;
                        }
                };
            }
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
                        return -1;
                    }
                    if (x.Val < y.Val)
                    {
                        return 1;
                    }
                    return 0;
                }

                #endregion
            }

            class SortKeyValReverce : System.Collections.Generic.IComparer<KeyVal>
            {
                #region Члены IComparer<KeyVal>

                public int Compare(KeyVal x, KeyVal y)
                {
                    if (x.Val < y.Val)
                    {
                        return -1;
                    }
                    if (x.Val > y.Val)
                    {
                        return 1;
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

            public void AddItem(string Key, decimal Val)
            {
                KeyVal NewFild = new KeyVal();
                NewFild.Key = Key;
                NewFild.Val = Val;
                Fields.Add(NewFild);
            }

            public string GetMinRang()
            {
                KeyVal Min = Fields[0];
                foreach (KeyVal kv in Fields)
                {
                    if (Min.Val < kv.Val)
                    {
                        Min = kv;
                    }
                }
                return Min.Key;
            }

            public string GetMaxRang()
            {
                KeyVal Max = Fields[0];
                foreach (KeyVal kv in Fields)
                {
                    if (Max.Val > kv.Val)
                    {
                        Max = kv;
                    }
                }

                return Max.Key;
            }

            public object GetRang(string Key, bool reverce)
            {
                if (reverce)
                    Fields.Sort(new SortKeyVal());
                else
                    Fields.Sort(new SortKeyValReverce());

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

        string ClearDataMemberStr(string str)
        { 
            if (str.Contains("ДАННЫЕ)")) 
            {
                return str.Replace("ДАННЫЕ)", "").Replace("(","");
            }
            return str;
        }

        private void ClearDataMember(CoolTable GridTable)
        {
            foreach (DataRow row in GridTable.Rows)
            {
                row[0] = ClearDataMemberStr(row[0].ToString());
            }
        }

        Dictionary<string, RankingField> ColRanger = new Dictionary<string, RankingField>();

        private void DataBindGrid()
        {
            DataTable Basetable = DBHelper.ExecQueryByID("Grid", "Region");

            DataTable FOTable = DBHelper.ExecQueryByID("GridFO", "FO");
            FOTable.Rows[0]["FO"] = "Сибирский федеральный округ";
            DataRow RowFO = Basetable.NewRow();
            RowFO.ItemArray = FOTable.Rows[0].ItemArray;
            Basetable.Rows.InsertAt(RowFO, 0);

            CoolTable GridTable = new CoolTable();

            GridTable.Columns.Add("Территория");

            foreach (DataColumn BaseCol in Basetable.Columns)
            {
                if (BaseCol.ColumnName.Contains("Region"))
                {
                    continue;
                }

                if (!BaseCol.ColumnName.Contains(" ComparePeriod"))
                {
                    string ColName = BaseCol.ColumnName.Split(';')[0];

                    InfoRow RowInfo = new InfoRow(ColName);

                    RankingField Ranker = new RankingField();

                    DataColumn BaseColCompare = Basetable.Columns[ColName + "; ComparePeriod"];

                    DataColumn GridValueColumn = GridTable.Columns.Add(ColName, typeof(decimal));

                    bool FORow = true;

                    foreach (DataRow BaseRow in Basetable.Rows)
                    {
                        DataRow ValueRow = GridTable.AddOrGetRow(BaseRow[0].ToString() + ";top");
                        DataRow SpeedDeviationRow = GridTable.AddOrGetRow(BaseRow[0].ToString() + ";douwn");
                        DataRow DeviationRow = GridTable.AddOrGetRow(BaseRow[0].ToString() + ";center");

                        ValueRow[GridValueColumn] = BaseRow[BaseCol];
                        SpeedDeviationRow[GridValueColumn] = GetSpeedDeviation(BaseRow, BaseCol, BaseColCompare);
                        if (BaseRow[BaseCol] != DBNull.Value)
                        {
                            if ((RowInfo.Rang)&(!FORow))
                            {
                                decimal ValueCol = (decimal)BaseRow[BaseCol];
                                Ranker.AddItem(ValueRow[0].ToString(), ValueCol);
                            }
                        }
                        FORow = false;

                        DeviationRow[GridValueColumn] = GetDeviation(BaseRow, BaseCol, BaseColCompare);


                    }

                    ColRanger.Add(ColName, Ranker);

                    if (RowInfo.Rang)
                    {
                        foreach (DataRow GridTableRow in GridTable.Rows)
                        {
                            if (GridTableRow[0].ToString().Contains(";top"))
                            {
                                if (!GridTableRow[0].ToString().Contains("Сибирский федеральный округ"))
                                {
                                    GridTable.GetRowFormValueFirstCell(GridTableRow[0].ToString().Replace(";top", "") + ";center")[GridValueColumn] =
                                        Ranker.GetRang(GridTableRow[0].ToString(),RowInfo.ReveceRang);
                                }
                                else
                                {
                                    GridTable.GetRowFormValueFirstCell(GridTableRow[0].ToString().Replace(";top", "") + ";center")[GridValueColumn] = DBNull.Value;
                                }
                            }
                        }
                    }

                }
            }

            ClearDataMember(GridTable);

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
            foreach (UltraGridColumn Col in Grid.Columns)
            {
                SetDefaultStyleHeader(Col.Header);
                if (!Col.BaseColumnName.Contains("Территория"))
                {
                    
                    Col.Header.Caption = new InfoRow(Col.BaseColumnName).ReportDisplay;
                    Col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    Col.CellStyle.Padding.Right = 5;
                }
            }
        }

        private void ConfCol()
        {
            Grid.Columns.FromKey("Field").CellStyle.Wrap = true;
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("SelectDate"), "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("CompareDate"), "N2");
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
            UltraGridCell RangCell = GetNextCell(GetNextCell(ValueCell));
            if (RangCell.Value != null)
            {
                RangCell.Title = 
                    //true
                    ReverceRang
                    ?
                    string.Format("Ранг по ФО на {1} г.: {0} \nСамое высокое значение показателя", RangCell.Value.ToString().Replace("Ранг ",""), CurDate.ReportDate()) :
                    string.Format("Ранг по ФО на {1} г.: {0} \nСамое низкое значение показателя", RangCell.Value.ToString().Replace("Ранг ", ""), CurDate.ReportDate());
                SetImageFromCell(RangCell, "starYellowBB.png");
            }
        }

        private void SetMinRang(UltraGridCell ValueCell, bool Max, bool ReverceRang)
        {
            UltraGridCell RangCell = GetNextCell(GetNextCell(ValueCell));
            if (RangCell.Value != null)
            {
                RangCell.Title = 
                    //!true
                    !ReverceRang
                    ?
                    string.Format("Ранг по ФО на {1} г.: {0} \nСамое высокое значение показателя", RangCell.Value.ToString().Replace("Ранг ", ""), CurDate.ReportDate()) :
                    string.Format("Ранг по ФО на {1} г.: {0}\nСамое низкое значение показателя", RangCell.Value.ToString().Replace("Ранг ", ""), CurDate.ReportDate());
                SetImageFromCell(RangCell, "starGrayBB.png");
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
            SetFormatCell(cell, "N0");
            if (!string.IsNullOrEmpty(cell.Text))
            {
                cell.Text = "Ранг " + cell.Text;
                cell.Title = string.Format("Ранг по ФО на {0} г.: {1}", CurDate.ReportDate(), cell.Value.ToString().Replace("Ранг ", ""));
            }
        }
        private void SetIndicatorDeviationcell(UltraGridCell ValueCell, bool reverce)
        {
            UltraGridCell IndicatorCell = GetNextCell(GetNextCell(ValueCell));
            if (IndicatorCell.Value != null)
            {

                decimal Value = decimal.Parse(IndicatorCell.Value.ToString());
                if (Value != 0)
                {
                    string UpOrdouwn = Value > 0 ? UpOrdouwn = "Up" : UpOrdouwn = "Down";
                    string TitleCaption = Value > 0 ? "Прирост к" : "Снижение к";
                    IndicatorCell.Title = string.Format("{0} {1}", TitleCaption, CompareDate.ReportDate());
                }
                else
                {
                    IndicatorCell.Title = string.Format("За период с {0} по {1} изменений не произошло",CompareDate.ReportDate(), CurDate.ReportDate());
                }
            }
        }

        private void SetIndicatorSpeedDeviationcell(UltraGridCell ValueCell, bool reverce)
        {
            UltraGridCell IndicatorCell = GetNextCell(ValueCell);
            if (IndicatorCell.Value != null)
            {
                
                decimal Value = decimal.Parse(IndicatorCell.Value.ToString());
                if (Value != 0)
                {
                    string UpOrdouwn = Value > 0 ? UpOrdouwn = "Up" : UpOrdouwn = "Down";

                    string TitleCaption = Value > 0 ? "прироста" : "снижения";

                    IndicatorCell.Title = string.Format("Темп {0} к {1}", TitleCaption, CompareDate.ReportDate());

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
                else
                {
                    IndicatorCell.Title = string.Format("За период с {0} по {1} изменений не произошло", CompareDate.ReportDate(), CurDate.ReportDate());
                }
                IndicatorCell.Text += "%";
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
            SetFormatCell(GetNextCell(ValueCell), "N2");
                //RowInfo.FormatString);
            SetFormatCell(GetNextCell(GetNextCell(ValueCell)), RowInfo.FormatString);

            string ColName = ValueCell.Column.BaseColumnName;
            string RegionName = ValueCell.Row.Cells[0].Text;

            if (RowInfo.Rang)
            {
                SetFormatRang(GetNextCell(GetNextCell(ValueCell)));
                String.Format (ColRanger[ColName].GetMaxRang().Replace(";top", "") +"|"+ RegionName.Replace(";top", ""));
                
                if (ClearDataMemberStr(ColRanger[ColName].GetMaxRang().Replace(";top", "")) == RegionName.Replace(";top", ""))
                {
                    if (!RowInfo.ReveceRang)
                    {
                        SetMaxRang(ValueCell, true, RowInfo.ReveceRang);
                    }
                    else
                    {
                        SetMinRang(ValueCell, true, RowInfo.ReveceRang);
                    }
                }
                if (ClearDataMemberStr(ColRanger[ColName].GetMinRang().Replace(";top", "")) == RegionName.Replace(";top", ""))
                {
                    if (RowInfo.ReveceRang)
                    {
                        SetMaxRang(ValueCell, true, RowInfo.ReveceRang);
                    }
                    else
                    {
                        SetMinRang(ValueCell, true, RowInfo.ReveceRang);
                    }
                }

            }
            else
            {
                SetIndicatorDeviationcell(ValueCell, RowInfo.Revece);
            }

            SetIndicatorSpeedDeviationcell(ValueCell, RowInfo.Revece);
        }



        private void ConfRow()
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells.FromKey("Территория").Text.Contains(";top"))
                {
                    Row.Cells.FromKey("Территория").RowSpan = 3;


                    foreach (UltraGridColumn Col in Grid.Columns)
                    {
                        if (Col.BaseColumnName != "Территория")
                        {
                            InfoRow IRow = new InfoRow(Col.BaseColumnName);

                            FormatValueCell(Row.Cells.FromKey(Col.BaseColumnName), IRow);
                        }
                    }

                    Row.Cells.FromKey("Территория").Text = Row.Cells.FromKey("Территория").Text.Replace(";top", "");
                }
            }
        }

        private void OtherCustomizeGrid()
        {
            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;
            Grid.DisplayLayout.NoDataMessage = "Нет данных";
            Grid.DisplayLayout.NullTextDefault = "";
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
        }

        private void SetDisplayGrid()
        {
            ConfHeader();
            //ConfCol();
            ConfRow();
            OtherCustomizeGrid();
            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }
            CustomizerSize.ContfigurationGrid(Grid);
        }

        private void GenerateGrid()
        {
            DataBindGrid();
            SetDisplayGrid();
        }
        #endregion        

        private void SetHeader()
        {
            PageTitle.Text = "Мониторинг ситуации на рынке труда (Сибирский федеральный округ)";
            PageSubTitle.Text = string.Format("Данные ежемесячного мониторинга ситуации на рынке труда по состоянию на {0}", CurDate.ReportDate());
            Page.Title = PageTitle.Text;

            LabelMap2.Text = string.Format("«{0}», {1} на {2}", GlobalNavigationInfoRow.Caption, GlobalNavigationInfoRow.EdIzm, CurDate.ReportDate());

            LabelChart1.Text = string.Format("Распределение территорий по показателю «{0}», {1} на {2}", GlobalNavigationInfoRow.Caption, GlobalNavigationInfoRow.EdIzm, CurDate.ReportDate());  
        }

        #region NavigatorRadioBox

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

        #endregion

        private void FillNaviagatorPanel() 
        {
            CoolTable TableListForChart = new CoolTable();
            TableListForChart.Columns.Add("Field");
            TableListForChart.Columns.Add("UName");
            TableListForChart.AddOrGetRow("Уровень регистрируемой безработицы, % от численности экономически активного населения")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень регистрируемой безработицы, % от численности экономически активного населения ]";
            TableListForChart.AddOrGetRow("Уровень напряженности на рынке труда, ед.")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень напряженности на рынке труда ]";
            TableListForChart.AddOrGetRow("Уровень трудоустройства ищущих работу граждан (с начала года), %")["Uname"]
                = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень трудоустройства ищущих работу граждан ]";

            NaviagatorChart = new RadioButtons(TableListForChart, rb_CheckedChanged);
            NaviagatorChart.FillPLaceHolder(PlaceHolder1);
        }

        InfoRow GlobalNavigationInfoRow;

        void rb_CheckedChanged(object sender, EventArgs e)
        {
            NaviagatorChart.ActiveRow = NaviagatorChart.Buttons[(RadioButton)sender];
            SetQueryParam();
            ChangeMapAndChartParam();
            GenerateMap();
            GenChart();
            SetHeader();
        }

        #region Map
        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        string SetBr(string s, int limit)
        {
            for (int i = 0; i < s.Length; i += limit)
            {
                for (int j = i; (j < i + limit) & (j < s.Length); j++)
                {
                    if (s[j] == ' ')
                    {
                        i = j;
                        s = s.Insert(j, '\n' + "");
                        break;
                    }
                }
            }
            return s;
        }

        private void CustomaizeMapko()
        {
            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = true;
            DundasMap1.NavigationPanel.Visible = true;
            DundasMap1.Viewport.EnablePanning = true;
            DundasMap1.Viewport.Zoom = 100;
            DundasMap1.ColorSwatchPanel.Visible = false;

            DundasMap1.Legends.Clear();
            // добавляем легенду
            Legend legend = new Legend("CostLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.Dock = PanelDockStyle.Right;
            legend.DockAlignment = DockAlignment.Near;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;  
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;

            legend.Title = SetBr(NaviagatorChart.ActiveRow[0].ToString(), 30);
                //"Уровень регистрируемой\nбезработицы";
            legend.AutoFitMinFontSize = 7;

            // добавляем легенду с символами
            Legend legend2 = new Legend("SymbolLegend");
            legend2.Visible = true;
            legend2.Dock = PanelDockStyle.Right;
            legend2.DockAlignment = DockAlignment.Far;
            legend2.BackColor = Color.White;
            legend2.BackSecondaryColor = Color.Gainsboro;
            legend2.BackGradientType = GradientType.DiagonalLeft;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Gray;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.Black;
            legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.Title = "Общая численность\n зарегистрированных безработных граждан";
            legend2.AutoFitMinFontSize = 7;

            DundasMap1.Legends.Add(legend2);
            DundasMap1.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CostRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Cost";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;

            rule.FromColor = GlobalNavigationInfoRow.BeginColor;
            rule.MiddleColor = GlobalNavigationInfoRow.CenterColor;
            rule.ToColor = GlobalNavigationInfoRow.EndColor;

            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInLegend = "CostLegend";
            rule.LegendText = "#FROMVALUE{N1}% - #TOVALUE{N1}%";
            DundasMap1.ShapeRules.Add(rule);

            // добавляем поля
            DundasMap1.Shapes.Clear();
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("Cost");
            DundasMap1.ShapeFields["Cost"].Type = typeof(double);
            DundasMap1.ShapeFields["Cost"].UniqueIdentifier = false;

            string mapFolderName = "Субъекты\\Новособл";

            DundasMap1.Layers.Clear();
            AddMapLayer(DundasMap1, "СФО", "СФО", CRHelper.MapShapeType.CalloutTowns);
            #endregion
        }

        enum ShapeType
        {
            CallOutCity, MO, City
        }

        string GetUniversalRegionName(string Name)
        {
            return Name.Replace(" область", "").Replace("Республика ", "").Replace(" обл.", "").Replace(" автономный округ", "").Replace(" край", "").Replace("Р. ", "");
        }

        ShapeType GetShapeType(Shape shape)
        {
            if (shape.Name.Contains("_callout"))
            {
                return ShapeType.CallOutCity;
            }
            if (shape.Name.Contains("район"))
            {
                return ShapeType.MO;
            }
            return ShapeType.City;
        }

        DataRow GetShapeValue(DataTable BaseTable, Shape Shape)
        {
            string ShapeName = Shape.Name;
            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                if (GetUniversalRegionName(BaseRow["Region"].ToString()) == GetUniversalRegionName(ShapeName))
                {
                    if (BaseRow[0] != DBNull.Value)
                    {
                        return BaseRow;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        string GetMapDisplayRegionName(string Name)
        {
            return Name.Replace("муницыпальный ", "").Replace("_callout", "");
        }

        DataTable TableChart = null;

        public void FillMap1Data()
        {
            string query = DataProvider.GetQueryText("STAT_0003_0001_map1");
            DataTable TableMap = DBHelper.ExecQueryByID("Map", "Region");
            TableChart = TableMap;
            InfoRow Info = new InfoRow(UserComboBox.getLastBlock(NaviagatorChart.ActiveRow[1].ToString()));

            string Hint = @"{0} \n{3} \nна {1} г.: {2:" + Info.FormatString + "} ";// +Info.EdIzm;

            foreach (Shape shape in DundasMap1.Shapes)
            {
                try
                { 
                    DataRow ShapeRow = GetShapeValue(TableMap, shape);


                    string ShapeReportName = shape.Name;

                    ShapeType TypeShape = GetShapeType(shape);

                    shape.ToolTip = string.Format(Hint,
                        ShapeRow[0],
                        CurDate.ReportDate(),
                        ShapeRow[1],
                        NaviagatorChart.ActiveRow[0].ToString());

                    shape.TextVisibility = TextVisibility.Shown;
                    shape["Cost"] = double.Parse(ShapeRow[1].ToString());
                    shape.Text = ShapeReportName.Replace(" ", "\n") + "\n" + string.Format("{0:" + Info.FormatString + "}", ShapeRow[1]);
                    if (TypeShape == ShapeType.MO)
                    {

                    }
                }
                catch { }
            }          
        }

        private void GenerateMap()
        {
            CustomaizeMapko();
            FillMap1Data();
        }

        #endregion

        CustomParam LastSelectComboBox { get { return (UserParams.CustomParam("_")); } }

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
            SelectMapAndChartParam.Value = NaviagatorChart.ActiveRow["UName"].ToString();
            
            GlobalNavigationInfoRow = new InfoRow(UserComboBox.getLastBlock(SelectMapAndChartParam.Value));
            SelectMapAndChartParamRF.Value = GlobalNavigationInfoRow.RFString;
        }

        DataTable SortDataTable(DataTable Table)
        {
            DataTable SortTable = new DataTable();
            foreach (DataColumn col in Table.Columns)
            {
                SortTable.Columns.Add(col.ColumnName, col.DataType);
            }
            DataRow[] Rows = Table.Select("", "Value ASC");

            foreach (DataRow Row in Rows)
            {
                SortTable.Rows.Add(Row.ItemArray);
            }
            SortTable.Columns.Remove(SortTable.Columns[0]);
            return SortTable;
        }

        private void GenChart()
        {   
            UltraChart1.ChartType = ChartType.ColumnChart;
            TableChart = SortDataTable(TableChart);
            String.Format(TableChart.Rows[TableChart.Rows.Count - 1][0].ToString());
            decimal LastValuyFromChart = (decimal)TableChart.Rows[TableChart.Rows.Count - 1][0];

            UltraChart1.Axis.Y.RangeMax = (double)LastValuyFromChart * 1.1;
            UltraChart1.Axis.Y.RangeMin = 0;
            UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;

            UltraChart1.DataSource = TableChart;
            UltraChart1.DataBind();

            UltraChart1.Border.Color = Color.Transparent;

            UltraChart1.Axis.X.Extent = 200;
            UltraChart1.Axis.Y.Extent = 60;

            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana",9);

            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart1.Axis.X.Margin.Far.Value = 2;
            UltraChart1.Axis.X.Margin.Near.Value = 2;

            UltraChart1.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart1.ColorModel.ColorBegin = Color.LimeGreen;
            UltraChart1.ColorModel.ColorEnd = Color.Lime;
            UltraChart1.Tooltips.FormatString = string.Format("<SERIES_LABEL><br>По состоянию на {0} г.<br><b><DATA_VALUE:N2></b> "+GlobalNavigationInfoRow.EdIzm, 
                CurDate.ReportDate());
            
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(Chart_FillSceneGraph);
        }

        #region Линии на диограмке
        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            SelectMapAndChartParamRF.Value = GlobalNavigationInfoRow.RFString;
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = ColorLine;
            Line.PE.StrokeWidth = 2;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;

            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);
            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;

            if (TextUP)
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY - 15, 800, 15);
            }
            else
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY + 1, 800, 15);
            }


            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.SetTextString(Label);

            e.SceneGraph.Add(Line);
            e.SceneGraph.Add(textLabel);


        }


        void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            try
            {
                DataTable TableRFValue = DBRFHelper.ExecQueryByID("ChartRFLine");

                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                double RFStartLineX = 0;
                double RFStartLineY = 0;

                double RFEndLineX = 0;
                double RFEndLineY = 0;
                string RFheader = "";
                try
                {
                    

                    if (TableRFValue.Rows[0][1] != DBNull.Value)
                    {
                        RFheader = string.Format("{0}: {1:N2} "+GlobalNavigationInfoRow.EdIzm, "Российская федерация", TableRFValue.Rows[0][1]);

                        if (GlobalNavigationInfoRow.Caption != "Уровень напряженности на рынке труда, ед.")
                        {
                            RFStartLineX = xAxis.Map(xAxis.Minimum);
                            RFStartLineY = yAxis.Map((double)(decimal)TableRFValue.Rows[0][1]);

                            RFEndLineX = xAxis.Map(xAxis.Maximum);
                            RFEndLineY = yAxis.Map((double)(decimal)TableRFValue.Rows[0][1]);
                        }
                        else
                        {
                            RFStartLineX = xAxis.Map(xAxis.Minimum);
                            RFStartLineY = yAxis.Map((double)(decimal)TableRFValue.Rows[0][1] / 100);

                            RFEndLineX = xAxis.Map(xAxis.Maximum);
                            RFEndLineY = yAxis.Map((double)(decimal)TableRFValue.Rows[0][1] / 100); 
                        }

                    }

                }
                catch { }
                double FOStartLineX = 0;
                double FOStartLineY = 0;
                double FOEndLineX = 0;
                double FOEndLineY = 0;
                string FOheader = "";

                DataTable TableFOValue = DBHelper.ExecQueryByID("ChartFOLine");
                try
                {
                    if (TableFOValue.Rows[0]["Value"] != DBNull.Value)
                    {
                        FOheader = string.Format("{0}: {1:N2} " + GlobalNavigationInfoRow.EdIzm, "Сибирский федеральный округ", TableFOValue.Rows[0]["Value"]);

                        FOStartLineX = xAxis.Map(xAxis.Minimum);
                        FOStartLineY = yAxis.Map((double)(decimal)TableFOValue.Rows[0]["Value"]);

                        FOEndLineX = xAxis.Map(xAxis.Maximum);
                        FOEndLineY = yAxis.Map((double)(decimal)TableFOValue.Rows[0]["Value"]);


                    }
                }
                catch { }


                bool RFUP = true;

                bool FOUP = true;

                if ((Math.Abs(FOStartLineY - RFStartLineY) < 22))
                {
                    RFUP = RFStartLineY < FOStartLineY;

                    FOUP = FOStartLineY < RFStartLineY;

                }

                if (!string.IsNullOrEmpty(RFheader))
                {
                    GenHorizontalLineAndLabel((int)RFStartLineX, (int)RFStartLineY, (int)RFEndLineX, (int)RFEndLineY,
                            Color.Red, RFheader, e, RFUP);
                }

                if (!string.IsNullOrEmpty(FOheader))
                {
                    GenHorizontalLineAndLabel((int)FOStartLineX, (int)FOStartLineY, (int)FOEndLineX, (int)FOEndLineY,
                            Color.Blue, FOheader, e, FOUP);
                }
            }
            catch { }

        }

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            FillCombo();    
            GetCompareDate();
            SetQueryParam();

            GenerateGrid();           

            FillNaviagatorPanel();

            ChangeMapAndChartParam();

            GenerateMap();

            GenChart();

            SetHeader();

            DundasMap1.RenderType = RenderType.InteractiveImage;
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
            ReportPDFExporter1.HeaderCellHeight = 100;
            ReportPDFExporter1.Export(headerLayout, section1);

            section2.PageSize = new PageSize(section1.PageSize.Height, section1.PageSize.Width);
            DundasMap1.Height = 700;
            DundasMap1.Width = 1000;
            UltraChart1.Width = 1000;
            ReportPDFExporter1.Export(DundasMap1,LabelMap2.Text,section2);
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

            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[0].Text.Contains(";"))
                {
                    Row.Cells[0].Value = null;
                }
                
            }

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
            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма");

            ReportExcelExporter1.HeaderCellHeight = 80;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 0;

            GridHeaderLayout headerLayout = GenExportLayot();
            ReportExcelExporter1.Export(headerLayout, sheet1, 7);

            UltraChart1.Width = 800-100;
            DundasMap1.Width = 900-100;
            DundasMap1.Width = 500;

            ReportExcelExporter1.Export(DundasMap1, LabelMap2.Text, sheet2, 4);
            ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet4, 4);

            sheet2.Rows[0].Cells[0].Value = LabelMap2.Text;
            sheet4.Rows[0].Cells[0].Value = LabelChart1.Text;

            sheet2.Rows[1].Cells[0].Value = "";  
            sheet4.Rows[1].Cells[0].Value = "";

            sheet2.Columns[0].Width = 250 * 100;
            sheet4.Columns[0].Width = 250 * 100;

            sheet2.Rows[0].Height *= 2;
            sheet4.Rows[0].Height *= 2;
            sheet4.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet2.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;

            sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

            sheet4.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet2.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

            sheet2.Rows[3].Cells[0].Value = "";
            sheet4.Rows[3].Cells[0].Value = "";
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