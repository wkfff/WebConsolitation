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

namespace Krista.FM.Server.Dashboards.reports.org_gkh_novosib_002
{
    public partial class Default : CustomReportPage
    {
        ICustomizerSize CustomizerSize;
         
        IDataSetCombo DataSetComboDay;
        IDataSetCombo DataSetComboRegion;
        IDataSetCombo DataSetComboRealRegion;

        #region Подонка размеров элементов, под браузер и разрешение
        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;

            public abstract int GetGridWidth();
            public abstract int GetGridHeight();

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

            public override int GetGridHeight()
            {
                //throw new NotImplementedException();
                return 400;
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
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth + 15).Value;
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth + 15).Value;
                    default:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 100).Value;
                }
            }

            public override int GetGridHeight()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 550;
                    case BrouseName.FireFox:
                        return 640;
                    case BrouseName.SafariOrHrome:
                        return 570;
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
                if (Grid.Columns.Count < 10)
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (int)(onePercent * (100 / Grid.Columns.Count))-5;
                }
                else
                {
                    foreach (UltraGridColumn col in Grid.Columns)
                    {
                        col.Width = onePercent * 8;
                    }
                    Grid.Columns[0].Width = onePercent * 12;
                }
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                if (Grid.Columns.Count < 10)
                {
                    foreach (UltraGridColumn col in Grid.Columns)
                    {
                        col.Width = (int)(onePercent * (100 / Grid.Columns.Count)) + 4;
                    }
                }
                else
                {
                    foreach (UltraGridColumn col in Grid.Columns)
                    {
                        col.Width = onePercent * 8;
                    }
                    Grid.Columns[0].Width = onePercent * 12;
                }
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                if (Grid.Columns.Count < 10)
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (int)(onePercent * (100/Grid.Columns.Count));
                }
                else
                {
                    foreach (UltraGridColumn col in Grid.Columns)
                    {
                        col.Width = onePercent * 8;
                    }
                    Grid.Columns[0].Width = onePercent * 12;
                }
            }
            #endregion


            public override int GetMapHeight()
            {
                return 640;
            }


        }
        #endregion

        DataBaseHelper DBHelper;
        DataBaseHelper DBRFHelper;
        CustomParam Date { get { return (UserParams.CustomParam("Date")); } }
        CustomParam PrevDate { get { return (UserParams.CustomParam("PrevDate")); } }
        CustomParam SelectRegion { get { return (UserParams.CustomParam("SelectRegion")); } }

        CustomParam INN { get { return (UserParams.CustomParam("INN")); } }
        CustomParam childreg { get { return (UserParams.CustomParam("childreg")); } }

        CustomParam MegaParam { get { return (UserParams.CustomParam("MegaParam", true)); } }

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
                    string UniqueName = row["UnamePeriod"].ToString();

                    string Mounth = row["Mounth"].ToString();

                    string Year = row["Year"].ToString();

                    string DisplayNAme = this.GetAlowableAndFormatedKey(Mounth + " " + Year + " года", "");

                    if (LaseYear != Year)
                    {
                        this.AddItem(Year + " год", 0, string.Empty);
                    }

                    LaseYear = Year;
                    LastMounth = Mounth;

                    row["PrevDisplayName"] = PrevAddedKey;

                    PrevAddedKey = DisplayNAme;

                    this.AddItem(DisplayNAme, 1, UniqueName);

                    this.addOtherInfo(Table, row, DisplayNAme);
                }
            }
        }

        class SetComboRegion : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                Table.Columns.Add("DisplayNameReport");
                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();

                    string DisplayName = row["DisplayName"].ToString();
                    int Level = int.Parse(row["level"].ToString());

                    DisplayName = GetAlowableAndFormatedKey(DisplayName, "");

                    row["DisplayNameReport"] = DisplayName;

                    this.AddItem(DisplayName, Level, UniqueName);

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

            GridTop.Width = CustomizerSize.GetGridWidth();
            GridTop.Height = Unit.Empty;//CustomizerSize.GetGridHeight();
            #endregion

            #region Экспорт

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #endregion
        }

        void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            mergeRegion(0, 7, GridTop.Rows.Count + 6, e.CurrentWorksheet);
            mergeRegion(1, 7, GridTop.Rows.Count + 6, e.CurrentWorksheet);
            mergeRegion(2, 7, GridTop.Rows.Count + 6, e.CurrentWorksheet);
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
                int Day =
                int.Parse(firstsplit[7]);
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

        Node GetPrevNode(Node n)
        {
            if (n.PrevNode != null)
            {
                return n.PrevNode;
            }
            if (n.Parent.PrevNode != null)
            {
                return n.Parent.Nodes[n.Parent.Nodes.Count - 1];
            }
            return null;
        }

        bool isDetail()
        {   
            return !string.IsNullOrEmpty(MegaParam.Value);
        }

        string GetDetailDate()
        {
            
            return MegaParam.Value.Split('|')[0];
        }

        string GetDetailOrg()
        {
            
            return MegaParam.Value.Split('|')[1];
        }

        string GetDetailInn()
        {
            return MegaParam.Value.Split('|')[2];
        }

        string GetDetailRegion()
        {
            return MegaParam.Value.Split('|')[3];
        }
        
        private void FillComoPeriod()
        {
            ComboRegion.Title = "Отчётный период";

            DataSetComboRegion = new SetComboDay();
            DataSetComboRegion.LoadData(DBHelper.ExecQueryByID("ComboMounth"));

            ComboRegion.Width = 300;

            ComboRegion.ParentSelect = false;

            if (!Page.IsPostBack)
            {
                ComboRegion.FillDictionaryValues(DataSetComboRegion.DataForCombo);
                if (isDetail())
                {
                    ComboRegion.SetСheckedState(GetDetailDate(),true);
                }
                else
                {
                    ComboRegion.SelectLastNode();
                }
            }

            Date.Value = DataSetComboRegion.DataUniqeName[ComboRegion.SelectedValue] + ".[Данные месяца]";
            try
            {
                PrevDate.Value = DataSetComboRegion.DataUniqeName[GetPrevNode(ComboRegion.SelectedNode).Text] + ".[Данные месяца]";
            }
            catch
            {
                PrevDate.Value = "null";
            }
        }

        private void FillComboRegion()
        {
            DataSetComboRealRegion = new SetComboRegion();
            DataSetComboRealRegion.LoadData(DBHelper.ExecQueryByID("ComboRegion"));

            if (!Page.IsPostBack)
            {
                ComboRealRegion.FillDictionaryValues(DataSetComboRealRegion.DataForCombo);
                if (isDetail())
                {
                    string Key = GetRealRegion();                    
                    ComboRealRegion.SetСheckedState(Key, true);
                }
            }
            SelectRegion.Value = DataSetComboRealRegion.DataUniqeName[ComboRealRegion.SelectedValue];
            INN.Value = DataSetComboRealRegion.OtherInfo[ComboRealRegion.SelectedValue]["иин"];
            childreg.Value = DataSetComboRealRegion.OtherInfo[ComboRealRegion.SelectedValue]["RefRegionID"]; //dict["RefRegionID"];
        }

        private string GetRealRegion()
        {
            foreach(Dictionary<string, string> dict in DataSetComboRealRegion.OtherInfo.Values)
            {   
                if ((dict["иин"] == GetDetailInn()) && (dict["RefRegion"] == GetDetailRegion()) && (dict["Name"] == GetDetailOrg()))
                {                    
                    return dict["DisplayNameReport"];
                }
            }
            return "";
        }

        private void FillCombo()
        {
            ComboRealRegion.Width = 550;
            ComboRealRegion.Title = "Организация";
            FillComoPeriod();
            FillComboRegion();

        }

        string RemoveLastChar(string s)
        {
            return s.Remove(s.Length - 1, 1);
        }

        private string GenMemberSelect(CoolDate ReportDate)
        {
            return string.Format(@"[Период__Период].[Период__Период].[{0}] ", ReportDate.ReportDate());
        }

        private string GenMemberOfDate(CoolDate ReportDate)
        {
            return string.Format(@"member [Период__Период].[Период__Период].[{0}] as '{1}'", ReportDate.ReportDate(), ReportDate.ConvertToMDXNamefirstTable());
        }

        string GetDisplayMounthFromTOPgrid(string mounth)
        {
            if (mounth.ToLower() == "январь")
            {
                return mounth + " " + YearSelect + " года"; ;
            }

            return "Январь - " + mounth + " " + YearSelect + " года"; ; ;
        }

        string YearSelect = "2011";

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

            public InfoRow(string BaseName)
            {
                switch (BaseName)
                {
                    case "Среднесписочная численность работников крупных и средних организаций, человек":
                        {
                            ReportDisplay = "Среднесписочная численность работников организаций и предприятий, человек";
                            FormatString = "N0";
                            Revece = false;
                            OnlyRow = false;
                            return;
                        }
                    case "   в % к трудовым ресурсам":
                        {
                            ReportDisplay = "   в % к трудовым ресурсам";
                            FormatString = "P2";
                            OnlyRow = true;
                            return;
                        }
                    case "Среднесписочная численность работников организаций и предприятий, человек в % к трудовым ресурсам":
                        {
                            ReportDisplay = "Среднесписочная численность работников организаций и предприятий, человек в % к трудовым ресурсам";
                            FormatString = "N0";
                            OnlyRow = false;
                            Revece = false;
                            return;
                        }
                    case "Численность работающих в режиме неполного рабочего времени":
                        {
                            ReportDisplay = "Численность работающих в режиме неполного рабочего времени, человек";
                            FormatString = "N0";
                            OnlyRow = false;
                            Revece = true;
                            return;
                        }

                };
            }
        }

        private void FillCap(DataRow Row, string[] fieldSplit)
        {
            try
            {
                Row["cap1"] = fieldSplit[0];
                Row["cap2"] = fieldSplit[1];
                Row["cap3"] = fieldSplit[2];
            }
            catch 
            { 
            }
        }

        private void DataBindGrid(UltraWebGrid Grid)
        {
            Grid.Bands.Clear();

            DataTable BaseTable = DBHelper.ExecQueryByID(   
                RadioButton1.Checked ?
                "Grid_1" :
                RadioButton2.Checked ? "Grid_2" : "Grid_3",
                "field");

            if (RadioButton3.Checked)
            {
                Grid.DataSource = BaseTable;
                Grid.DataBind();
                return;
            }

            CoolTable TableGrid = new CoolTable();
            TableGrid.Columns.Add("cap1");
            TableGrid.Columns.Add("cap2");
            TableGrid.Columns.Add("cap3");

            foreach (DataColumn col in BaseTable.Columns)
            {
                if (col.ColumnName != "field")
                {
                    TableGrid.Columns.Add(col.ColumnName, col.DataType);
                }
            }

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                DataRow GridRow = TableGrid.AddRow();

                string[] fieldSplit = BaseRow["field"].ToString().Split('_');
                FillCap(GridRow, fieldSplit);
                foreach (DataColumn col in BaseTable.Columns)
                {
                    if (col.ColumnName != "field")
                    {
                        GridRow[col.ColumnName] = BaseRow[col];
                    }
                }
            }

            Grid.DataSource = TableGrid;
            Grid.DataBind();
        }

        private void FormatFirstHeader(HeadersCollection headersCollection)
        {
            List<HeaderBase> RemovedHeader = new List<HeaderBase>();
            HeaderBase mainhb = null;
            foreach (HeaderBase hb in headersCollection)
            {
                if ((hb.Caption == " "))
                    RemovedHeader.Add(hb);
                if (hb.Caption == "Наименование")
                    mainhb = hb;
            }

            foreach (HeaderBase h in RemovedHeader)
            {
                headersCollection.Remove(h);
            }
            mainhb.RowLayoutColumnInfo.SpanY = 3;



        }

        GridHeaderLayout HL;
        private void ConfHeader(UltraWebGrid Grid)
        {

            HL = new GridHeaderLayout(Grid);
            GridHeaderCell cell = HL.AddCell("Наименование");
            if (!RadioButton3.Checked)
            {
                cell.AddCell(" ",2);
                cell.AddCell(" ",2);
                cell.AddCell(" ",2);
            }
            if (RadioButton1.Checked)
            {
                GridHeaderCell mainCell = HL.AddCell("ДОХОДЫ, тыс. рублей");
                mainCell.AddCell("Всего");
                cell = mainCell.AddCell("За тепло, в т.ч.:");
                cell.AddCell("от населения");
                cell.AddCell("от бюд. фин. организаций");
                cell.AddCell("от прочих");
                mainCell.AddCell("За водоснабжение и канализование");
                mainCell.AddCell("За прочее");


            }
            else
            {

                if (RadioButton2.Checked)
                {
                    GridHeaderCell mainCell = HL.AddCell("РАСХОДЫ, тыс. рублей");
                    mainCell.AddCell("Всего");
                    cell = mainCell.AddCell("На топливо, в т.ч. на: ");
                    cell.AddCell("уголь");
                    cell.AddCell("газ");
                    cell.AddCell("прочее");
                    mainCell.AddCell("На электроэнергию");
                    mainCell.AddCell("На оплату труда");
                    mainCell.AddCell("На ремонт");
                    mainCell.AddCell("На прочее");
                }
                else
                {
                    GridHeaderCell mainCell = HL.AddCell("ЗАДОЛЖЕННОСТЬ, тыс. рублей");
                    GridHeaderCell mainCell_ = mainCell.AddCell("Дебиторская, в т.ч.");
                    mainCell_.AddCell("Всего");
                    cell = mainCell_.AddCell("За тепло");
                    cell.AddCell("По расчетам с населением");
                    cell.AddCell("По расчетам с бюд. фин. организациями");
                    cell.AddCell("По расчетам с прочимим");
                    mainCell_.AddCell("За водоснабжение и канализование");
                    mainCell_.AddCell("За прочее");
                    mainCell_ = mainCell.AddCell("Кредиторская, в т.ч.");
                    mainCell_.AddCell("Всего");
                    cell = mainCell_.AddCell("За топливо, в т.ч.:");
                    cell.AddCell("За уголь");
                    cell.AddCell("За газ");
                    cell.AddCell("За прочие");
                    mainCell_.AddCell("За электроэнергию");
                    mainCell_.AddCell("По оплате труда");
                    mainCell_.AddCell("По ремонтам");
                    mainCell_.AddCell("За прочее");


                }


            }
            HL.ApplyHeaderInfo();

            foreach (HeaderBase hb in Grid.Bands[0].HeaderLayout)
            {
                hb.Style.HorizontalAlign = HorizontalAlign.Center;
                hb.Style.Wrap = true;
            }
            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.CellStyle.Wrap = true;
                if (col.BaseColumnName.Contains("cap"))
                { }
                else
                {
                    CRHelper.FormatNumberColumn(col, "########0.############");
                    
                }
            }
            if (!RadioButton3.Checked)
            FormatFirstHeader(Grid.Bands[0].HeaderLayout);

            Grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

        }

        private UltraGridCell GetNextCell(UltraGridCell Cell)
        {
            return Cell.Row.NextRow.Cells.FromKey(Cell.Column.BaseColumnName);
        }

        void SetFormatCell(UltraGridCell Cell, string format)
        {
            if (Cell.Value != null)
            {
                Cell.Text = string.Format("{0:" + format + "}", decimal.Parse(Cell.Text));
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

        private bool IsPercentCell(UltraGridCell cell)
        {
            return (cell.Row.Cells.FromKey(RadioButton3.Checked ? "field" : "cap2").Value != null) &&
                   (cell.Row.Cells.FromKey(RadioButton3.Checked ? "field" : "cap2").Text.Contains("%"));
        }

        private void FormatPercentCell(UltraGridCell cell)
        {
            cell.Style.HorizontalAlign = HorizontalAlign.Right;
            cell.Text = string.Format("{0:N2}%", decimal.Parse(cell.Text));
        }

        private void FormatValueCell(UltraGridCell cell)
        {
            cell.Text = string.Format("{0:N2}", decimal.Parse(cell.Text));
        }

        private void FormatRow(UltraWebGrid Grid)
        {
            MergeCellsGrid(Grid, 0);
            if (!RadioButton3.Checked)
            {
                MergeCellsGrid(Grid, 1);
                MergeCellsGrid(Grid, 2);
                Grid.Rows[0].Cells[0].Style.BorderDetails.ColorRight = Color.Transparent;
                Grid.Rows[0].Cells[1].Style.BorderDetails.ColorLeft = Color.Transparent;
                Grid.Rows[0].Cells[1].Style.BorderDetails.ColorRight = Color.Transparent;
                Grid.Rows[0].Cells[2].Style.BorderDetails.ColorLeft = Color.Transparent;

                Grid.Rows[0].Cells[1].Value = "";
                Grid.Rows[0].Cells[2].Value = "";
            }

            foreach (UltraGridRow Row in Grid.Rows)
            {
                foreach (UltraGridCell cell in Row.Cells)
                {
                    string ColName = cell.Column.BaseColumnName;
                    if ((!ColName.Contains("cap")) && (!ColName.Contains("field")))
                    {
                        cell.Style.HorizontalAlign = HorizontalAlign.Right;
                        if (cell.Value != null)
                        {
                            if (IsPercentCell(cell))
                            {
                                FormatPercentCell(cell);
                            }
                            else
                            {
                                FormatValueCell(cell);
                            }
                        }
                    }
                }
            }
        }

        private void OtherCustomizerGrid(UltraWebGrid Grid)
        {
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.NullTextDefault = "-";
            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.Transparent;

            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";

            }
            if (!RadioButton3.Checked)
            {
                //Grid.Columns.FromKey("cap1").MergeCells = true;
                //Grid.Columns.FromKey("cap2").MergeCells = true;
                //Grid.Columns.FromKey("cap3").MergeCells = true;
            }
        }

        private void CustomizeGrid(UltraWebGrid Grid)
        {
            ConfHeader(Grid);
            OtherCustomizerGrid(Grid);
            CustomizerSize.ContfigurationGrid(Grid);
            FormatRow(Grid);
        }

        private void GenerationGrid(UltraWebGrid Grid)
        {
            DataBindGrid(Grid);            
            CustomizeGrid(Grid);
        }

        private void SetHeader()
        {
            PageTitle.Text = "Мониторинг доходов, расходов и задолженности организаций ЖКХ";
            
            PageSubTitle.Text = string.Format("Данные ежемесячного мониторинга деятельности организаций ЖКХ {0} за {1}", "Новосибирской области", ComboRegion.SelectedValue.ToLower());
            Page.Title = PageTitle.Text;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            FillCombo();

            GenerationGrid(GridTop);
            
            SetHeader();

        }

        #region Экспорт в PDF 

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
                for (int i = 1; i < GridTop.Columns.Count - 1; i++)
                {
                    row.Cells[i].Style.BackColor = row.Cells[0].Style.BackColor;
                    row.Cells[i].Style.BorderDetails.ColorLeft = row.Cells[0].Style.BackColor;
                    row.Cells[i].Style.BorderDetails.ColorRight = row.Cells[0].Style.BackColor;
                }
                row.Cells[GridTop.Columns.Count - 1].Style.BorderDetails.ColorLeft = row.Cells[0].Style.BackColor;
                row.Cells[GridTop.Columns.Count - 1].Style.BackColor = row.Cells[0].Style.BackColor;
                row.Cells[0].Style.BorderDetails.ColorRight = row.Cells[0].Style.BackColor;
            }
        }


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();

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

            //GridHeaderLayout headerLayout = GenExportLayot();

            MergeCellsGrid(HL.Grid, 0);
            MergeCellsGridFormPDF(HL.Grid, 0);
            if (!RadioButton3.Checked)
            {
                MergeCellsGrid(HL.Grid, 1);
                MergeCellsGridFormPDF(HL.Grid, 1);
                MergeCellsGrid(HL.Grid, 2);
                MergeCellsGridFormPDF(HL.Grid, 2);
            }

            ReportPDFExporter1.Export(HL, section1);

        }

        private GridHeaderLayout GenExportLayot()
        {
            GridHeaderLayout headerLayout = new GridHeaderLayout(GridTop);

            foreach (UltraGridColumn Col in GridTop.Columns)
            {
                //Col.Width = 200;
                headerLayout.AddCell(Col.Header.Caption);

            }

            headerLayout.ApplyHeaderInfo();
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

            foreach (UltraGridRow Row in GridTop.Rows)
            {
                Row.Cells[0].Text = Row.Cells[0].Text.Replace("<b>", "").Replace("</b>", "");
            }

            ReportExcelExporter1.Export(HL, sheet1, 4);            

            sheet1.Rows[4].Height = 25 * 20;
        }

        private void mergeRegion(int ColumnNumber, int StartRow, int EndRow, Worksheet sheet1)
        {
            string PrevCaption = sheet1.Rows[StartRow].Cells[ColumnNumber].Value.ToString();
            int StartMerge = StartRow;

            for (int i = StartRow+1; i < EndRow+1; i++)
            {
                string CurCaption = sheet1.Rows[i].Cells[ColumnNumber].Value.ToString();
                
                if (CurCaption != PrevCaption)
                {
                    sheet1.MergedCellsRegions.Add(StartMerge, ColumnNumber, i - 1, ColumnNumber);
                    PrevCaption = CurCaption;
                    StartMerge = i;
                }
            }
            try
            {
                sheet1.MergedCellsRegions.Add(StartMerge, ColumnNumber, EndRow, ColumnNumber);
            }
            catch { }



        }

        private void SetEmptyCellFormat(Worksheet sheet, int row, int column)
        {
        }

        private void SetCellFormat(WorksheetCell cell)
        {
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
        }

        private static void SetExportGridParams(UltraWebGrid grid)
        {
        }

        #endregion
    }
}