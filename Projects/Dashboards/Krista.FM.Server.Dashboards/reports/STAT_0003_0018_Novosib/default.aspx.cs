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

//Lego

namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0018_Novosib
{
    public partial class Default : CustomReportPage
    {
        ICustomizerSize CustomizerSize;

        string FieldTopGrid = @"[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Начисленная среднемесячная заработная плата в расчете на одного работника предприятий и организаций - всего],
[Показатели__Рынок труда].[Показатели__Рынок труда].[Среднемесячная номинальная начисленная заработная плата работников крупных и средних организаций, руб.],
[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Реальная начисленная заработная плата, в % к соответствующему периоду предыдущего года],
[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Номинальная начисленная заработная плата, в % к соответствующему периоду предыдущего года]";

        IDataSetCombo DataSetComboDay;
        IDataSetCombo DataSetComboRegion;

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

        CustomParam Comennt { get { return (UserParams.CustomParam("Comennt")); } } 

        CustomParam SelectUserYear { get { return (UserParams.CustomParam("SelectUserYear")); } }

        CustomParam SelectMapAndChartParam { get { return (UserParams.CustomParam("SelectMapAndChartParam")); } }

        CustomParam LastSelectComboBox { get { return (UserParams.CustomParam("_")); } }
        CustomParam ChangeNavigatorChart { get { return (UserParams.CustomParam("ChangeNavigatorChart")); } }

        CustomParam DateForRang { get { return (UserParams.CustomParam("DateForRang")); } }
        CustomParam FieldForRang { get { return (UserParams.CustomParam("FieldForRang")); } }
        CustomParam ActiveRegion { get { return (UserParams.CustomParam("ActiveRegion")); } }

        CustomParam WithZoneDate { get { return (UserParams.CustomParam("WithZoneDate")); } }
        CustomParam Dates { get { return (UserParams.CustomParam("Dates")); } }

        CustomParam Fields { get { return (UserParams.CustomParam("Fields")); } }
        CustomParam Field { get { return (UserParams.CustomParam("Field")); } }

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

                    string Year = row["Year"].ToString();

                    string DisplayName = Year + " год";

                    this.AddItem(DisplayName, 0, UniqueName);

                    this.addOtherInfo(Table, row, DisplayName);
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
                        this.AddItem(Parent, 0, "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Сибирский федеральный округ].[Новосибирская область].datamember");
                    }

                    LastParent = Parent;

                    if (!DisplayName.Contains("ДАННЫЕ)"))
                    {

                        this.AddItem(DisplayName, 1, UniqueName);

                        this.addOtherInfo(Table, row, DisplayName);
                    }
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

            GridTop.Width = CustomizerSize.GetGridWidth();
            GridTop.Height = Unit.Empty;

            UltraChart2.Width = CustomizerSize.GetChartWidth();
            UltraChart2.Height = 400;

            #endregion

            #region Экспорт

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #endregion

            UltraChart2.BackColor = Color.Transparent;
        }

        class CoolDate
        {
            public DateTime dt;
            bool oldReactor = false;

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

                //foreach (string s in firstsplit)
                //{
                //    String.Format(s);
                //}

                int Year = int.Parse(firstsplit[3]);


                int Mounth = CRHelper.MonthNum(firstsplit[6]);

                int Day = 1;
                if (MDXName.Contains("Заключительные обороты"))
                {
                    oldReactor = true;
                }
                else
                {
                    int.TryParse(firstsplit[7], out Day);
                }

                return new DateTime(Year, Mounth, Day);

            }

            public string ConvertToMDXName()
            {
                return string.Format(@"[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]",
                    dt.Year,
                CRHelper.HalfYearNumByMonthNum(dt.Month),
                CRHelper.QuarterNumByMonthNum(dt.Month),
                CRHelper.RusMonth(dt.Month),
                oldReactor ? "Заключительные обороты" : dt.Day.ToString());
            }

            public string ConvertToMDXNamefirstTable()
            {
                DateTime dt_ = dt.AddMonths(-1);
                return string.Format(@"[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                    dt.Year,
                CRHelper.HalfYearNumByMonthNum(dt.Month),
                CRHelper.QuarterNumByMonthNum(dt.Month),
                CRHelper.RusMonth(dt.Month)//,
                    //dt.Day
                );
            }

            public string ConvertToMDXNamefirstTableRF()
            {
                return string.Format(@"[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                    dt.Year,
                CRHelper.HalfYearNumByMonthNum(dt.Month),
                CRHelper.QuarterNumByMonthNum(dt.Month),
                CRHelper.RusMonth(dt.Month)//,
                    //dt.Day
                );
            }

            public string ReportDate()
            {
                return string.Format("{0:00}.{1:00}.{2:00}", dt.Day, dt.Month, dt.Year);
            }
            public string ReportDateRF()
            {
                string mounth = CRHelper.RusMonth(dt.Month);
                string YearSelect = dt.Year.ToString();

                if (mounth.ToLower() == "январь")
                {
                    return mounth + " " + YearSelect + " года"; ;
                }

                return "Январь - " + mounth + " " + YearSelect + " года";
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
            ComboRegion.Title = "Территория";
            ComboCurrentPeriod.Title = "Период";
            ComboCurrentPeriod.PanelHeaderTitle = "Дата:";
            ComboCurrentPeriod.ShowSelectedValue = true;
            ComboRegion.ParentSelect = true;

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
                ComboCurrentPeriod.Width = 200;
                ComboRegion.Width = 350;
            }
            else
            {
                ComboCurrentPeriod.Width = 170;
                ComboRegion.Width = 200;
            }

            DataSetComboDay = new SetComboDay();
            DataSetComboDay.LoadData(DBHelper.ExecQueryByID("ComboDates", "Day"));
            if (!Page.IsPostBack)
            {
                ComboCurrentPeriod.FillDictionaryValues(DataSetComboDay.DataForCombo);
                ComboCurrentPeriod.SetСheckedState("2011 год", true);
            }
        }

        string RemoveLastChar(string s)
        {
            return s.Remove(s.Length - 1, 1);
        }

        private CoolDate GetCompareDate(string ComboName)
        {
            CoolDate CurDate = new CoolDate(DataSetComboDay.DataUniqeName[ComboName]);
            return new CoolDate(new DateTime(CurDate.dt.Year - 1, CurDate.dt.Month, CurDate.dt.Day));
        }

        private string GenMemberSelect(CoolDate ReportDate)
        {
            return string.Format(@"[Период__Период].[Период__Период].[{0}] ", ReportDate.ReportDate());
        }

        private string GenMemberSelectRF(CoolDate ReportDate)
        {
            return string.Format(@"[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[{0}] ", ReportDate.ReportDateRF());
        }

        private string GenMemberOfDate(CoolDate ReportDate)
        {
            return string.Format(@"member [Период__Период].[Период__Период].[{0}] as '{1}'", ReportDate.ReportDate(), ReportDate.ConvertToMDXNamefirstTable());
        }

        private string GenMemberOfDateRF(CoolDate ReportDate)
        {
            return string.Format(@"member [Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[{0}] as '{1}'",
                ReportDate.ReportDateRF(),
                ReportDate.ConvertToMDXNamefirstTableRF());
        }

        string GetDisplayMounthFromTOPgrid(string mounth, string YearSelect)
        {
            if (mounth.ToLower() == "январь")
            {
                return mounth + " " + YearSelect + " года"; ;
            }

            return "Январь - " + mounth + " " + YearSelect + " года"; ; ;
        }

        private void ChangeParamFromtext(string YearSelect)
        {
            ChangeParam(false, false, YearSelect);
            WithZoneDate.Value = WithZoneDate.Value.Replace(".[Заключительные обороты]", "");
        }

        private void ChangeParam(bool Compare, bool DataMounth, string YearSelect)
        {
            Dates.Value = "";
            WithZoneDate.Value = "";

            string YearCompare = (int.Parse(YearSelect) - 1).ToString();

            SelectUserYear.Value = YearSelect;

            DataTable Table = DBHelper.ExecQueryByID("PeriodAlovalebleFirstGrid", "_");



            DataRow RowMounth = Table.Rows[0];
            DataRow RowUname = Table.Rows[1];

            foreach (DataColumn Col in Table.Columns)
            {
                if (Col.ColumnName != "_")
                {
                    if (DataMounth)
                    {
                        WithZoneDate.Value +=
                            string.Format(@"member [Период__Период].[Период__Период].[{0}] as '{1}'", GetDisplayMounthFromTOPgrid(RowMounth[Col].ToString(), YearSelect),
                            RowUname[Col].ToString().Replace(YearSelect, Compare ? YearCompare : YearSelect));
                        Dates.Value += string.Format("[Период__Период].[Период__Период].[{0}]", GetDisplayMounthFromTOPgrid(RowMounth[Col].ToString(), YearSelect)) + ",";
                    }
                    else
                    {

                        Dates.Value += string.Format("[Период__Период].[Период__Период].[{0}]",
                            new CoolDate(RowUname[Col].ToString()).ReportDate()) + ",";

                        WithZoneDate.Value +=
                            string.Format(@"member [Период__Период].[Период__Период].[{0}] as '{1}'",
                            new CoolDate(RowUname[Col].ToString()).ReportDate(),
                            RowUname[Col].ToString().Replace(YearSelect, Compare ? YearCompare : YearSelect));
                    }
                }
            }
            if (!string.IsNullOrEmpty(Dates.Value))
            {
                Dates.Value = RemoveLastChar(Dates.Value);
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
                        return CountRegion - index + 1;
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
            public bool OnlyRow;
            public bool Image = true;

            public InfoRow(string BaseName)
            {

                /*IsSubjectRF ?
                    "[СЭП].[Годовой сборник].[Среднемесячная номинальная начисленная заработная плата работников организаций]" :
                    "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Начисленная среднемесячная заработная плата в расчете на одного работника предприятий и организаций - всего] ";
            TableListForChart.AddOrGetRow("Реальная начисленная заработная плата, к предыдущему году")["Uname"]
                =
                IsSubjectRF ?
                "[СЭП].[Годовой сборник].[Реальная начисленная заработная плата, к предыдущему году]" :
                "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Реальная начисленная заработная плата, в % к соответствующему периоду предыдущего года]";*/

                switch (BaseName)
                {



                    case "Среднемесячная номинальная начисленная заработная плата работников организаций":
                        {
                            ReportDisplay = "Среднемесячная номинальная начисленная заработная плата работников организаций, руб.";
                            Caption = "Среднемесячная номинальная начисленная заработная плата работников организаций";
                            EdIzm = "руб.";
                            FormatString = "N2";
                            Revece = false;
                            OnlyRow = true;
                            Image = false;
                            return;
                        }
                    case "Среднемесячная номинальная начисленная заработная плата работников крупных и средних организаций, руб.":
                        {
                            ReportDisplay = "Среднемесячная номинальная начисленная заработная плата работников крупных и средних организаций, руб.";
                            Caption = "Среднемесячная номинальная начисленная заработная плата работников крупных и средних организаций, руб.";
                            EdIzm = "руб.";
                            FormatString = "N2";
                            Revece = false;
                            OnlyRow = true;
                            Image = false;
                            return;
                        }


                    case "Начисленная среднемесячная заработная плата в расчете на одного работника предприятий и организаций - всего":
                        {
                            ReportDisplay = "Среднемесячная номинальная начисленная заработная плата работников организаций, руб.";
                            Caption = "Среднемесячная номинальная начисленная заработная плата работников организаций";
                            EdIzm = "руб.";
                            FormatString = "N2";
                            Revece = false;
                            OnlyRow = true;
                            Image = false;
                            return;
                        }
                    case "Реальная начисленная заработная плата, к предыдущему году":
                        {
                            ReportDisplay = "Реальная начисленная заработная плата, в % к соответствующему периоду предыдущего года";
                            Caption = "Реальная начисленная заработная плата";
                            EdIzm = "в % к соответствующему периоду предыдущего года";
                            FormatString = "N2";
                            OnlyRow = true;
                            return;
                        }


                    case "Реальная начисленная заработная плата, в % к соответствующему периоду предыдущего года":
                        {
                            ReportDisplay = "Реальная начисленная заработная плата, в % к соответствующему периоду предыдущего года";
                            Caption = "Реальная начисленная заработная плата";
                            EdIzm = "в % к соответствующему периоду предыдущего года";
                            FormatString = "N2";
                            OnlyRow = true;
                            return;
                        }
                    case "Номинальная начисленная заработная плата, в % к соответствующему периоду предыдущего года":
                        {
                            ReportDisplay = "Номинальная начисленная заработная плата, в % к соответствующему периоду предыдущего года";
                            FormatString = "N2";
                            OnlyRow = true;
                            Revece = false;
                            return;
                        }
                    case "Численность работающих в режиме неполного рабочего времени":
                        {
                            ReportDisplay = "Численность работающих в режиме неполного рабочего времени, человек";
                            FormatString = "N0";
                            OnlyRow = true;
                            Revece = true;
                            return;
                        }

                };
            }
        }


        private void GenerationORAddGridColAndRow()
        {


            int StartCountCol = TableGrid.Columns.Count;
            if (StartCountCol > 0)
            {
                StartCountCol--;
            }

            foreach (DataColumn BaseCol in BaseTableSelect.Columns)
            {
                if (!TableGrid.Columns.Contains(BaseCol.ColumnName))
                {
                    TableGrid.Columns.Add(BaseCol.ColumnName, BaseCol.DataType);
                }
            }

            foreach (DataRow BaseRow in BaseTableSelect.Rows)
            {
                string Field = BaseRow["Field"].ToString();

                InfoRow RowInfo = new InfoRow(Field);

                DataRow GridRowValue = TableGrid.AddOrGetRow(Field);

                DataRow GridRowDeviation = null;
                DataRow GridRowSpeedDeviation = null;

                if (!RowInfo.OnlyRow)
                {
                    GridRowDeviation = TableGrid.AddOrGetRow(Field + "Deviation");
                    GridRowSpeedDeviation = TableGrid.AddOrGetRow(Field + "SpeedDeviation");
                }

                //GridRowValue["Field"] = Field;

                DataRow CompareRow = GetRowFromValueFirstCell(BaseTableCompare, Field);

                for (int i = 1; i < BaseTableSelect.Columns.Count; i++)
                {
                    int IndexReportGrid = i + StartCountCol;

                    GridRowValue[IndexReportGrid] = BaseRow[i];

                    if (!RowInfo.OnlyRow)
                    {
                        GridRowDeviation[IndexReportGrid] = GetDeviation(BaseRow[i], CompareRow[i]);

                        if (RowInfo.Rang)
                        {
                            GridRowDeviation[IndexReportGrid] = GetRang(Field, BaseTableSelect.Columns[i].ColumnName, RowInfo.ReveceRang);
                        }

                        GridRowSpeedDeviation[IndexReportGrid] = GetSpeedDeviation(BaseRow[i], CompareRow[i]);
                    }
                }
            }
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

        private void ChangeParamRf()
        {
            Dates.Value = "";
            foreach (string Date in ComboCurrentPeriod.SelectedValues)
            {
                Dates.Value = string.Format("or [Территории__Сопоставимый].[Территории__Сопоставимый].[year] = \"{0}\"", GetDijghtYear(Date));
            }
        }


        string GetDijghtYear(string year)
        {
            return year.Replace(" год", "");
        }

        DataTable BaseTableCompare = null;
        DataTable BaseTableSelect = null;

        CoolTable TableGrid;

        private void DataBindGrid(UltraWebGrid Grid)
        {
            Fields.Value = FieldTopGrid;

            TableGrid = new CoolTable();

            foreach (string SelectYear in ComboCurrentPeriod.SelectedValues)
            {
                try
                {
                    string Year = GetDijghtYear(SelectYear);
                    ChangeParam(true, Grid == GridTop, Year);
                    BaseTableCompare = DBHelper.ExecQueryByID("Grid", "Field");

                    ChangeParam(false, Grid == GridTop, Year);
                    BaseTableSelect = DBHelper.ExecQueryByID("Grid", "Field");

                    GenerationORAddGridColAndRow();
                }
                catch { }
            }

            Grid.DataSource = TableGrid;
            Grid.DataBind();
        }


        DataRow GenRow(DataTable Table)
        {
            DataRow Row = Table.NewRow();
            Table.Rows.Add(Row);
            return Row;
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
            DataRow Row = GenRow(BaseTable);
            Row[0] = Field;
            return Row;
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

        private void ConfHeader(UltraWebGrid Grid)
        {
            Grid.Columns.FromKey("Field").Header.Caption = "Показатель";
            Grid.Columns.FromKey("Field").Header.Style.HorizontalAlign = HorizontalAlign.Center;
            foreach (UltraGridColumn Col in Grid.Columns)
            {
                if (!Col.BaseColumnName.Contains("Field"))
                {
                    SetDefaultStyleHeader(Col.Header);
                    Col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    Col.CellStyle.Padding.Right = 5;
                    Col.Header.Caption = Col.Header.Caption.ToLower();
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
        void SetFormatRang(UltraGridCell cell)
        {
            if (cell.Value != null)
            {
                SetFormatCell(cell, "N0");
                if (string.IsNullOrEmpty(cell.Title))
                {
                    cell.Title = string.Format("Ранг по ФО на {0}: {1}", BaseTableSelect.Columns[cell.Column.Index].ColumnName, cell.Value);
                }
                cell.Text = "Ранг " + cell.Text;
            }
        }

        void SetFormatCell1(UltraGridCell Cell, string format)
        {
            if (Cell.Value != null)
            {
                //String.Format(Cell.Text);
                Cell.Text = string.Format("{0:" + format + "}%", decimal.Parse(Cell.Text.Replace("%", "")));
            }
        }

        private void SetIndicatorSpeedDeviationcell(UltraGridCell ValueCell, bool reverce)
        {
            UltraGridCell IndicatorCell = ValueCell;
            //GetNextCell(GetNextCell(ValueCell));
            if (IndicatorCell.Value != null)
            {
                decimal Value = decimal.Parse(IndicatorCell.Value.ToString().Replace("%", ""));
                if (Value != 0)
                {
                    string UpOrdouwn = Value > 0 ? UpOrdouwn = "Up" : UpOrdouwn = "Down";

                    string TitleCaption = Value > 0 ? "прироста" : "снижения";

                    IndicatorCell.Title = string.Format("Темп {0} в сравнении с аналогичным периодом предыдущего года", TitleCaption);

                    string Color = "Green";
                    //if ((Value > 100) & (reverce))
                    //{
                    //    Color = "Red";
                    //}

                    if ((Value < 100))// & (!reverce))
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

                        IndicatorCell.Title = string.Format("{0} в сравнении с аналогичным периодом предыдущего года", TitleCaption);
                    }
                }
            }
            catch { }
        }

        private void FormatValueCell(UltraGridCell ValueCell, InfoRow RowInfo)
        {
            if (!RowInfo.OnlyRow)
            {
                FormatTopCell(ValueCell);
                FormatCenterCell(GetNextCell(ValueCell));
                FormatBottomCell(GetNextCell(GetNextCell(ValueCell)));
            }

            if (ValueCell.Value == null)
            {
                return;
            }

            SetFormatCell(ValueCell, RowInfo.FormatString);

            if (!RowInfo.OnlyRow)
            {
                SetFormatCell(GetNextCell(ValueCell), RowInfo.FormatString);
                //SetFormatCell(GetNextCell(GetNextCell(ValueCell)), RowInfo.FormatString);


                if (RowInfo.Rang)
                {

                    if (GetNextCell(ValueCell).Text.Replace(",00", "") == CountRegion.ToString())
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

                //SetIndicatorDeviationcell(ValueCell, false);

            }
            if (RowInfo.Image)
            {
                SetIndicatorSpeedDeviationcell(ValueCell, RowInfo.Revece);
            }
        }

        private void FormatRow(UltraWebGrid Grid)
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (!Row.Cells.FromKey("Field").Text.Contains("Deviation"))
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
                            if (!RowInfo.OnlyRow)
                            {
                                Cell.RowSpan = 3;
                            }
                            Cell.Style.Wrap = true;
                            Cell.Value = RowInfo.ReportDisplay;
                        }
                    }
                }
            }
        }

        private static void OtherCustomizerGrid(UltraWebGrid Grid)
        {
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }
        }

        private void CustomizeGrid(UltraWebGrid Grid)
        {
            ConfHeader(Grid);

            FormatRow(Grid);

            CustomizerSize.ContfigurationGrid(Grid);

            OtherCustomizerGrid(Grid);
        }



        private void GenerationGrid(UltraWebGrid Grid)
        {
            DataBindGrid(Grid);

            CustomizeGrid(Grid);
        }

        InfoRow GlobalNavigationInfoRow;



        DataTable SumTable(DataTable MainTable, DataTable Table)
        {
            foreach (DataColumn col in Table.Columns)
            {
                if (!MainTable.Columns.Contains(col.ColumnName))
                {
                    MainTable.Columns.Add(col.ColumnName, col.DataType);
                }
            }
            foreach (DataRow Row in Table.Rows)
            {
                DataRow MainRow = GetRowFromValueFirstCell(MainTable, Row[0].ToString());
                foreach (DataColumn col in Table.Columns)
                {
                    MainRow[col.ColumnName] = Row[col];
                }
            }
            return MainTable;
        }

        string SetBR(string str)
        {
            if (str.Contains("\n"))
                return str;

            return str.Replace("20", "\n20").Replace("-", "\n-");//.Replace("года", "\nгода");
        }

        private void GenSecondChart()
        {
            UltraChart2.BackColor = Color.Transparent;
            DataTable TableChart = new DataTable();

            UltraChart2.ChartType = ChartType.AreaChart;

            DataBaseHelper DBRF = new DataBaseHelper(IsSubjectRF ? DataProvidersFactory.SecondaryMASDataProvider : DataProvidersFactory.PrimaryMASDataProvider);

            if (IsSubjectRF)
            {
                ChangeParamRf();
                TableChart = DBRF.ExecQueryByID(IsSubjectRF ? "Grid_Rf" : "Chart", "Field");
            }
            else
            {
                string s = "";

                foreach (string SelectYear in ComboCurrentPeriod.SelectedValues)
                {
                    s = SelectYear;
                }
                ChangeParam(false, true, GetDijghtYear(s));
                DataTable BufTable = DBRF.ExecQueryByID(IsSubjectRF ? "Grid_Rf" : "Chart", "Field");
                TableChart = SumTable(TableChart, BufTable);
            }

            if (IsSubjectRF)
            {
                TableChart.Columns.Remove(TableChart.Columns[0]);
            }
            if (IsSubjectRF)
                foreach (DataRow Row in TableChart.Rows)
                {
                    Row[0] = Row[0].ToString().ToLower();
                }
            else
                {
                    foreach (DataColumn col in TableChart.Columns)
                    {
                        col.ColumnName = col.ColumnName.ToLower();
                        col.Caption = col.Caption.ToLower();
                    }
                }   

            if (TableChart.Columns.Count == 2)
            {
                return;
            }
            if (TableChart.Rows.Count == 1)
            {
                return;
            }

            UltraChart2.Data.SwapRowsAndColumns = IsSubjectRF;

            

            UltraChart2.DataSource = TableChart;
            UltraChart2.DataBind();

            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 8);

            UltraChart2.Axis.X.Extent = 90;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;


            UltraChart2.Axis.Y.Extent = 50;

            UltraChart2.Axis.X.Margin.Far.Value = 3;
            UltraChart2.Axis.X.Margin.Near.Value = 3;

            UltraChart2.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart2.AreaChart.NullHandling = NullHandling.DontPlot;

            UltraChart2.AreaChart.LineStartCapStyle = LineCapStyle.Round;
            UltraChart2.AreaChart.LineEndCapStyle = LineCapStyle.Round;
            UltraChart2.Border.Color = Color.Transparent;

            UltraChart2.AreaChart.LineAppearances.Clear();

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 10;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 5;

            UltraChart2.AreaChart.LineAppearances.Add(lineAppearance);

            

            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            
            UltraChart2.Tooltips.FormatString =
                string.Format("{2}<br>{0}<br>По состоянию на {3}: <b><DATA_VALUE:N2></b> {1}    ", GlobalNavigationInfoRow.Caption, GlobalNavigationInfoRow.EdIzm, IsSubjectRF ? "<ITEM_LABEL>" : "<SERIES_LABEL>", !IsSubjectRF ? "<ITEM_LABEL>" : "<SERIES_LABEL>");

            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);

        }

        void UltraChart2_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        Primitive GenLineFromBox(Box b)
        {
            int x = b.rect.X;
            int y = b.rect.Y + 5;
            Line L = new Line(new Point(x, y), new Point(x + 12, y), new LineStyle(LineCapStyle.NoAnchor, LineCapStyle.NoAnchor, LineDrawStyle.Solid));
            L.PE.StrokeWidth = 4;
            L.PE.Stroke = b.PE.FillStopColor;
            return L;
        }

        void _UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            List<Primitive> DeletePrimitive = new List<Primitive>();

            string[] CaptionLegend = new string[2] {
            IsSubjectRF?"Российская федерация":"Новосибирская область"
            ,
            IsSubjectRF?"Новосибирская область":ComboRegion.SelectedValue};

            int IndexOfPrimitive = 0;
            int IndexCaptioAdded = 0;

            foreach (Primitive p in e.SceneGraph)
            {
                if ((p is Text) && (((Text)p).GetTextString().Contains("CaptionLegend_CaptionLegend")))
                {
                    Text Lable = p as Text;
                    Box box = e.SceneGraph[IndexOfPrimitive - 1] as Box;
                    if (IndexCaptioAdded <= 1)
                    {
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

        }

        #region Линии на диограмке
        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            //SelectMapAndChartParamRF.Value = GlobalNavigationInfoRow.RFString;
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = ColorLine;
            Line.PE.StrokeWidth = 3;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;

            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 11, FontStyle.Italic);
            textLabel.labelStyle.FontColor = Color.Black;
            //textLabel.PE.f

            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];

            if (TextUP)
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY - 17, 500, 15);
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
            try
            {
                if (!GlobalNavigationInfoRow.Caption.Contains("Среднемесячная "))
                    return;
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
                        RFheader = string.Format("{0}: {1:N2}", "Российская федерация", TableRFValue.Rows[0][1]);

                        RFStartLineX = xAxis.Map(xAxis.Minimum);
                        RFStartLineY = yAxis.Map((double)(decimal)TableRFValue.Rows[0][1]);

                        RFEndLineX = xAxis.Map(xAxis.Maximum);
                        RFEndLineY = yAxis.Map((double)(decimal)TableRFValue.Rows[0][1]);

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
                        FOheader = string.Format("{0}: {1:N2}, {2}", "Новосибирская обл. ", TableFOValue.Rows[0]["Value"], GlobalNavigationInfoRow.EdIzm);

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



        void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Text tl = null;
            try
            {
                foreach (Primitive p in e.SceneGraph)
                {
                    if (p is Text)
                    {
                        if (p.Path != null)
                        {
                            if (p.Path.Contains("Border.Title.Grid.X"))
                            {
                                Text t = p as Text;
                                t.bounds.X -= t.bounds.Width;
                                t.bounds.Width *= 3;
                                t.PE.Fill = Color.Gray;
                                t.labelStyle.Font = new Font("Arial", 9);
                                t.SetTextString(SetBR(t.GetTextString()));
                                tl = t;
                            }
                        }
                    }
                }
                tl.bounds.X -= 10;
                _UltraChart2_FillSceneGraph(sender, e);
            }
            catch { }

        }

        private static void RemoveAllOnlyFirstAndLast(DataTable TablePieChart)
        {
            for (; TablePieChart.Columns.Count > 2; )
                TablePieChart.Columns.Remove(TablePieChart.Columns[1]);
        }

        string GetLastYear()
        {
            string last = "";
            foreach (string s in DataSetComboDay.DataForCombo.Keys)
            {
                last = s;
            }
            return last;
        }

        private void GenCombinateChart()
        {
            Fields.Value = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Среднесписочная численность работников организаций и предприятий],[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Среднесписочная численность работников крупных и средних организаций]";

            string lastyear = GetLastYear();

            ChangeParam(false, true, GetDijghtYear(lastyear));

            DataTable BaseTable = DBHelper.ExecQueryByID("BottomChart", "Field");

            DataTable TablePieChart = BaseTable.Copy();

            DataTable TableStackChart = BaseTable.Copy();

            RemoveAllOnlyFirstAndLast(TablePieChart);

        }

        void LegendChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            List<Primitive> RemovList = new List<Primitive>();
            foreach (Primitive p in e.SceneGraph)
            {
                if (p.Path != null)
                {
                    if (!p.Path.ToLower().Contains("legend"))
                    {
                        RemovList.Add(p);
                    }
                }
                else
                {
                    RemovList.Add(p);
                }
            }
            foreach (Primitive p in RemovList)
            {
                e.SceneGraph.Remove(p);
            }
        }

        void StackChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            foreach (Primitive p in e.SceneGraph)
            {
                if (p is Text)
                {
                    if (p.Path != null)
                    {
                        if (p.Path.Contains("Border.Title.Grid.X"))
                        {
                            Text t = p as Text;
                            t.bounds.X -= t.bounds.Width;
                            t.bounds.Width *= 2;

                            t.labelStyle.Font = new Font("Arial", 9);
                            t.SetTextString(SetBR(t.GetTextString()));
                        }
                    }
                }
            }
        }

        Dictionary<string, string> FillDictFormTextovka()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Минимальная заработная плата, установленная в Региональном соглашении (для работников организаций бюджетной сферы, финансируемых из областного и местных бюджетов)", "для работников организаций бюджетной сферы, финансируемых из областного и местных бюджетов");
            dict.Add("Минимальная заработная плата, установленная в Региональном соглашении (для работников организаций внебюджетной сферы, кроме организаций сельского хозяйства)", "для работников организаций внебюджетной сферы, кроме организаций сельского хозяйства");
            dict.Add("Минимальная заработная плата, установленная в Региональном соглашении (для работников организаций сельского хозяйства)", "для работников организаций сельского хозяйства");

            return dict;

        }

        private string GenDisplayQuart(string Year, string Qurt)
        {
            string QuartNum = Qurt.Split(' ')[1];
            return string.Format("{0} квартал {1} года", QuartNum, Year);
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

        private object GenArrow(decimal p, bool reverce)
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
            return p > 0 ? "Выше" : "Ниже";
        }


        private string GenText(string Uname, string DisplayName, bool DataMounth)
        {
            string result = "";

            Field.Value = Uname;
            DataTable Table = DBHelper.ExecQueryByID(
                (!DataMounth ?
                    "Textovka2" :
                    "Textovka3"));
            if (Table.Rows.Count > 0)
            {
                DataRow CurRow = Table.Rows[0];

                if (Table.Rows.Count == 2)
                {
                    DataRow PrevRow = Table.Rows[1];
                    result = string.Format("{0} на  <b>{1}</b> составила <b>{2:N2}</b> руб. ",
                        DisplayName,
                            (!DataMounth ?
                                GenDisplayQuart(CurRow["year"].ToString(), CurRow[0].ToString()) :
                                new CoolDate(CurRow["Uname"].ToString()).ReportDate()),
                        CurRow[UserComboBox.getLastBlock(Uname)]);
                    decimal Deviation = (decimal)GetDeviation(CurRow[UserComboBox.getLastBlock(Uname)], PrevRow[UserComboBox.getLastBlock(Uname)]);
                    decimal SpeedDeviation = (decimal)GetSpeedDeviation(CurRow[UserComboBox.getLastBlock(Uname)], PrevRow[UserComboBox.getLastBlock(Uname)]);
                    result += string.Format("{0} {1} значения показателя в сравнении со значением за <b>{2}</b> {3} <b>{4:N2}</b> руб. (темп {5} составил <b>{6:N2}</b> %)",
                        GenSDUp(Deviation),
                        GenArrow(Deviation, false),
                        (!DataMounth ?
                            GenDisplayQuart(PrevRow["year"].ToString(), PrevRow[0].ToString()) :
                            new CoolDate(PrevRow["Uname"].ToString()).ReportDate()),
                         GenSos(Deviation),
                         Math.Abs(Deviation),
                         GenSD(Deviation),
                         SpeedDeviation);
                }
                else
                {
                    result = string.Format("{0} на  <b>{1}</b> составила <b>{2:N2}</b> руб. ",
                        DisplayName,
                        (!DataMounth ?
                            GenDisplayQuart(CurRow["year"].ToString(), CurRow[0].ToString()) :
                            new CoolDate(CurRow["Uname"].ToString()).ReportDate()),
                        CurRow[UserComboBox.getLastBlock(Uname)]);
                }
            }
            return result;

        }




        private void GenerateTextovka()
        {

            Textovko.Text = "";
            DataTable Table = DBHelper.ExecQueryByID("Textovka");
            if (Table.Rows.Count == 0)
                return;
            CoolDate LastDate =
                new CoolDate(Table.Rows[0]["Uname"].ToString());
            Textovko.Text = string.Format("Минимальная заработная плата, установленная в Региональном соглашении о минимальной заработной плате в Новосибирской области на <b>{0}</b> составляет:", LastDate.ReportDate());

            Dictionary<string, string> dictBaseDisplay = FillDictFormTextovka();

            for (int i = 2; i < Table.Columns.Count; i++)
            {
                if (dictBaseDisplay.ContainsKey(Table.Columns[i].Caption))
                {
                    if (Table.Rows[0][i] != DBNull.Value)
                        Textovko.Text += string.Format("<br> &nbsp;&nbsp;- {0} - <b>{1:N2}</b> руб.",
                            dictBaseDisplay[Table.Columns[i].Caption],
                            Table.Rows[0][i]);
                }
            }

            Textovko.Text += "<br>&nbsp;&nbsp;&nbsp;" + GenText("[Показатели__Рынок труда].[Показатели__Рынок труда].[Величина прожиточного минимума для трудоспособного населения]",
                "Величина прожиточного минимума для трудоспособного населения", false);

            Textovko.Text += "<br>&nbsp;&nbsp;&nbsp;" + GenText("[Показатели__Рынок труда].[Показатели__Рынок труда].[Величина минимального потребительского бюджета (для населения региона в целом)]",
                "Величина минимального потребительского бюджета (для населения региона в целом)", true);




        }



        private string GenTextFormField(string UnameField)
        {
            Field.Value = UnameField;
            Fields.Value = UnameField;
            string Text = UserComboBox.getLastBlock(UnameField);
            CRHelper.SaveToQueryLog("1");
            ChangeParam(false, false, GetDijghtYear(GetLastYear()));

            DataTable table = DBHelper.ExecQueryByID("Text2", "Field");
            CRHelper.SaveToQueryLog("2");
            decimal CurValue = (decimal)table.Rows[1][1];
            decimal PrevValue = (decimal)table.Rows[0][1];


            string CurDate = table.Rows[1][0].ToString();
            string PrevDate = table.Rows[0][0].ToString();

            string Res = string.Format("&nbsp;&nbsp;&nbsp;{0} составила <b>{1:N0}</b> чел. ", Text, CurValue);

            if (CurValue == PrevValue)
            {
                Res += string.Format("Значение показателя в сравнении со значением на <b>{0}</b> чел.  сталось неизменным <br>", PrevDate);
                return Res;
            }

            decimal Devination = CurValue - PrevValue;
            string Prefix0 = Devination > 0 ? "Прирост" : "Cнижение";


            string Prefix1 = Devination < 0 ? "составило" : "составил";

            string Prefix2 = Devination > 0 ? "прироста" : "снижения";

            string img = (string)GenArrow(Devination, "Трудоспособное население в трудоспособном возрасте, не занятое в экономике и обучением" == Text);

            decimal SpedDeviation = (decimal)GetSpeedDeviation(CurValue, PrevValue);

            Res += string.Format("{0} {1} значения показателя в сравнении со значением за <b>{2}</b> {3} <b>{4:N0}</b> чел. (темп {6} <b>{5:N2} %</b>)<br>", Prefix0, img, PrevDate, Prefix1, Math.Abs(Devination), SpedDeviation, Prefix2);

            return Res;

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
                GenArrow(suValue4 - suValue4_prev, reverce),
                CompareDate.ReportDate(),
                CurDate.ReportDate(),
                GenSos(suValue4 - suValue4_prev),
                Math.Abs(suValue4 - suValue4_prev),
                GenSD(suValue4 - suValue4_prev),
                (suValue4 / suValue4_prev - 1) * 100,
                field);
            return Textovka;
        }



        private void SetHeader()
        {
            PageTitle.Text = string.Format("Заработная плата работников организаций");//, ComboRegion.SelectedValue);

            PageSubTitle.Text = string.Format("Данные мониторинга заработной платы работников организаций Новосибирской области");

            Page.Title = PageTitle.Text;
            LabelChart2.Text = string.Format("Сравнительная динамика показателя «{0}», {1}",
                 GlobalNavigationInfoRow.Caption, GlobalNavigationInfoRow.EdIzm);

        }
        private void SetEmptyChartStyle()
        {
            UltraChart2.Border.Color = Color.Transparent;
            UltraChart2.BackColor = Color.White;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart1_InvalidDataReceived);
        }

        void UltraChart1_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        RadioButtons NaviagatorChart;

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
        bool IsSubjectRF = false;
        private void FillNaviagatorPanel()
        {
            IsSubjectRF = ComboRegion.SelectedValue == "Новосибирская область";
                //ComboRegion.SelectedNode.Nodes.Count > 0;

            CoolTable TableListForChart = new CoolTable();
            TableListForChart.Columns.Add("Field");
            TableListForChart.Columns.Add("UName");
            TableListForChart.AddOrGetRow("Среднемесячная номинальная начисленная заработная плата работников организаций, руб.")["Uname"]
                =
                IsSubjectRF ?
                    "[СЭП].[Годовой сборник].[Среднемесячная номинальная начисленная заработная плата работников организаций]" :
                    "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Начисленная среднемесячная заработная плата в расчете на одного работника предприятий и организаций - всего]";
            TableListForChart.AddOrGetRow("Реальная начисленная заработная плата, в % к соответствующему периоду предыдущего года")["Uname"]
                =
                IsSubjectRF ?
                "[СЭП].[Годовой сборник].[Реальная начисленная заработная плата, к предыдущему году]" :
                "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Реальная начисленная заработная плата, в % к соответствующему периоду предыдущего года]";

            NaviagatorChart = new RadioButtons(TableListForChart, rb_CheckedChanged);
            NaviagatorChart.FillPLaceHolder(PlaceHolder1);
        }

        //InfoRow GlobalNavigationInfoRow;

        void rb_CheckedChanged(object sender, EventArgs e)
        {
            NaviagatorChart.ActiveRow = NaviagatorChart.Buttons[(RadioButton)sender];
            ChangeMapAndChartParam();
            //GenerateMap();
            //GenChart();
            GenSecondChart();
            SetHeader();
        }

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

            Fields.Value = SelectMapAndChartParam.Value;

            GlobalNavigationInfoRow = new InfoRow(UserComboBox.getLastBlock(SelectMapAndChartParam.Value));


            //SelectMapAndChartParamRF.Value = GlobalNavigationInfoRow.RFString;
        }
        private string GetLastValFromList(Dictionary<string, int>.KeyCollection keyCollection)
        {
            string res = "";
            foreach (string s in keyCollection)
            {
                res = s;
            }
            return res;

        }   

        private void _ChangeParam()
        {
            if (ComboCurrentPeriod.SelectedNodes.Count <= 0)
            {
                ComboCurrentPeriod.SetСheckedState(GetLastValFromList(DataSetComboDay.DataForCombo.Keys), true);
            }
        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();
            _ChangeParam();
            ActiveRegion.Value = DataSetComboRegion.DataUniqeName[ComboRegion.SelectedValue];
            Comennt.Value = "";

            GenerationGrid(GridTop);



            FillNaviagatorPanel();

            ChangeMapAndChartParam();

            GenSecondChart();

            SetHeader();

            GenerateTextovka();

            tabletext.Visible = Textovko.Text != "";

            SetEmptyChartStyle();
        }

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

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

            //foreach (UltraGridRow Row in GridTop.Rows)
            //{
            //    if (!Row.Cells[0].Text.Contains("Deviation"))
            //    {
            //        if (Row.Index == 0 || Row.Index == 4)
            //        {
            //            try
            //            {
            //                GetNextCell(Row.Cells[0]).Text = Row.Cells[0].Text;
            //                FormatTopCell(Row.Cells[0]);
            //                FormatCenterCell(GetNextCell(Row.Cells[0]));
            //                FormatBottomCell(GetNextCell(GetNextCell(Row.Cells[0])));

            //                Row.Cells[0].Value = "";
            //                GetNextCell(GetNextCell(Row.Cells[0])).Value = "";
            //            }
            //            catch { }
            //        }

            //    }
            //}

            GridHeaderLayout headerLayout = GenExportLayot(GridTop);

            ReportPDFExporter1.Export(headerLayout, section1);



            section2.PageSize = new PageSize(section1.PageSize.Height, section1.PageSize.Width);


            UltraChart2.Width = 1000;
            ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text, section2);

        }

        private GridHeaderLayout GenExportLayot(UltraWebGrid grid)
        {
            GridHeaderLayout headerLayout = new GridHeaderLayout(grid);

            foreach (UltraGridColumn Col in grid.Columns)
            {
                headerLayout.AddCell(Col.Header.Caption);
            }

            headerLayout.ApplyHeaderInfo();
            return headerLayout;
        }

        #endregion

        #region Экспорт в Excel
        void MoveTitle(Worksheet w)
        {
            w.Rows[0].Cells[0].Value = w.Rows[2].Cells[0].Value;
            w.Rows[2].Cells[0].Value = "";

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
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица 1");

            Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма 1");

            //Worksheet sheet7 = workbook.Worksheets.Add("Диаграмма 5");

            GridHeaderLayout headerLayout = GenExportLayot(GridTop);
            ReportExcelExporter1.Export(headerLayout, sheet1, 7);


            //sheet1.MergedCellsRegions.Clear();
            //ReportExcelExporter1.Export(headerLayout, sheet1, 17);

            for (int i = 1; i < GridTop.Columns.Count; i++)
            {
                sheet1.Columns[i].Width = 150 * 30;
            }



            UltraChart2.Width = 800;

            sheet4.Columns[0].Width = 300 * 100;

            ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet4, 3);


            ClearTitle(sheet4);

            MoveTitle(sheet4);

            for (int i = 8; i < GridTop.Rows.Count + 8; i += 3)
            {
                if (i != 8)
                {
                    //sheet1.MergedCellsRegions.Add(i, 0, i + 2, 0);
                    //sheet1.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                }

                else
                {
                    i -= 2;
                }
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