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
 
namespace Krista.FM.Server.Dashboards.reports.PROG_0003_Sahalin
{
    public partial class Default : CustomReportPage
    {

        ICustomizerSize CustomizerSize;

        IDataSetCombo DataSetComboYear;
        IDataSetCombo DataSetComboPath;

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
                    col.Width = onePercent * (100 - 26) / (12);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 26) / (12);
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

        CustomParam LastSelectGridCaption { get { return (UserParams.CustomParam("LastSelectGridCaption", true)); } }

        CustomParam QuartType { get { return (UserParams.CustomParam("QuartType", true)); } }

        CustomParam SelectProgramm { get { return (UserParams.CustomParam("SelectProgramm", true)); } }

        CustomParam SelectYear { get { return (UserParams.CustomParam("SelectYear", true)); } }

        CustomParam Comennt { get { return (UserParams.CustomParam("Comennt", true)); } }
        CustomParam LastSelectComboBox { get { return (UserParams.CustomParam("LastSelectComboBox", true)); } }
        CustomParam ChartParam { get { return (UserParams.CustomParam("ChartParam", true)); } }
        CustomParam ChartParamType { get { return (UserParams.CustomParam("ChartParamType", true)); } }
        CustomParam ChartParamTypeOnWall { get { return (UserParams.CustomParam("ChartParamTypeOnWall", true)); } }

        CustomParam SelectRegion { get { return (UserParams.CustomParam("SelectRegion", true)); } }

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
            string getQuartDispalyText(string Quart)
            {
                return new string[4] { "1-й квартал", "1-е полугодие", "9 месяцев", "Год" }[
                    int.Parse(Quart.Replace("Квартал ", "")) - 1];
            }

            public override void LoadData(DataTable Table)
            {

                string PrevYear = "";

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UName"].ToString();

                    string Quart = row["Quart"].ToString();

                    string Year = row["Year"].ToString();

                    if (PrevYear != Year)
                    {
                        PrevYear = Year;
                        this.AddItem(Year, 0, UniqueName);
                    }
                    
                    string DisplayName = GetAlowableAndFormatedKey(getQuartDispalyText(Quart), "");

                    this.AddItem(DisplayName, 1, UniqueName);

                    this.addOtherInfo(Table, row, DisplayName);
                }
            }
        }

        class SetLinearCombo : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                foreach(DataRow row in Table.Rows)
                {
                    string UniqueName = row["UName"].ToString();

                    string DisplayName = row[0].ToString();

                    this.AddItem(DisplayName, 0, UniqueName);
                    this.addOtherInfo(Table, row, DisplayName);
                }
            }
        }

        #endregion


        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
            //blackStyle = false;
            //if (Session["blackStyle"] != null)
            //{
            //    blackStyle = Convert.ToBoolean(((CustomParam)Session["blackStyle"]).Value);
            //}

            //CRHelper.SetPageTheme(this, blackStyle ? "MinfinBlackStyle" : "Minfin");
        }
        bool onWall = false;
        private int pageWidth;
        private int pageHeight;

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
            //ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #endregion

            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

            UltraChart2.BackColor = Color.Transparent;

            pageWidth = onWall ? 5600 : (int)Session["width_size"];
            pageHeight = onWall ? 2100: (int)Session["height_size"];

            GridTop.Visible = true;
            if (onWall)
            {
                ComprehensiveDiv.Style.Add("width", "5600px");
                ComprehensiveDiv.Style.Add("height", "2100px");
                GridTop.Visible = false;
                UltraChart2.Width = pageWidth - 50;
                UltraChart2.Height = pageHeight - 50;
            }

            int fontSizeMultiplier = onWall ? 5 : 1;

            if (onWall)
            {
                
                PageTitle.Font.Size = Convert.ToInt32(11 * fontSizeMultiplier);
                PageSubTitle.Font.Size = Convert.ToInt32(9.5 * fontSizeMultiplier);
                LabelChart2.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);
            }
            CheckBox1.Text = "Отображать квартальные значения";
            
        }


        private string GetYearDefault()
        {
            return DBHelper.ExecQueryByID("YearDefault").Rows[0][1].ToString();
        }

        string GetDijghtYear(string year)
        {
            return year.Replace(" год", "");
        }

        private void FillCombo()
        {
            ComboRegion.Width = 500;
            ComboRegion.Title = "Муниципальное образование";

            DataSetComboPath = new SetLinearCombo();

            DataSetComboPath.LoadData(DBHelper.ExecQueryByID("ComboRegion"));

            ComboRegion.ParentSelect = false;
            ComboRegion.MultiSelect = false;

            if (!Page.IsPostBack)
            {
                ComboRegion.FillDictionaryValues(DataSetComboPath.DataForCombo);
                ComboRegion.SelectLastNode();
            }

            SelectRegion.Value = DataSetComboPath.DataUniqeName[ComboRegion.SelectedValue];

            DataSetComboYear = new SetComboDay();
            DataSetComboYear.LoadData(DBHelper.ExecQueryByID("ComboDates", "Quart"));
            if (!Page.IsPostBack)
            {
                ComboCurrentPeriod.FillDictionaryValues(DataSetComboYear.DataForCombo);
                ComboCurrentPeriod.SelectLastNode();
            }

            ComboCurrentPeriod.Title = "Квартал";
            ComboCurrentPeriod.Width = 200;
            

            SelectYear.Value = DataSetComboYear.DataUniqeName[ComboCurrentPeriod.SelectedValue];
        }




        string RemoveLastChar(string s)
        {
            return s.Remove(s.Length - 1, 1);
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

        DataTable BaseTableCompare = null;
        DataTable BaseTableSelect = null;

        CoolTable TableGrid;

        private void ClearEmptyCol(CoolTable TableGrid)
        {

            List<DataColumn> DelColList = new List<DataColumn>();

            foreach (DataColumn col in TableGrid.Columns)
            {

                bool isEmptycol = true;

                foreach (DataRow row in TableGrid.Rows)
                {
                    if (row[col] != DBNull.Value)
                    {
                        if (row[col] is decimal)
                        {
                            if ((decimal)row[col] == 0)
                            {

                            }
                            else
                            {
                                isEmptycol = false;
                                break;
                            }
                        }
                        else
                        {
                            isEmptycol = false;
                            break;
                        }
                    }
                }
                if (isEmptycol)
                {
                    DelColList.Add(col);
                }
            }
            foreach (DataColumn col in DelColList)
            {
                TableGrid.Columns.Remove(col);
            }
        }


        Dictionary<string, int> FieldLevel = new Dictionary<string, int>();

        Dictionary<string, string> EdizmDict = new Dictionary<string, string>();

        private void DataBindGrid(UltraWebGrid Grid)
        {
            DataTable BaseTable = DBHelper.ExecQueryByID("Grid", "Field");

            TableGrid = new CoolTable();

            TableGrid.Columns.Add("Field");            
            foreach (DataColumn col in BaseTable.Columns)
            {
                if (col.ColumnName.Contains("План") ||
                    col.ColumnName.Contains("Факт") ||
                    col.ColumnName.Contains("% Исполнения"))
                {
                    TableGrid.Columns.Add(col.ColumnName, typeof(decimal));
                }
                if (col.ColumnName.Contains("EdIzm"))
                {
                    EdizmDict.Add(col.ColumnName.Split(';')[0], BaseTable.Rows[0][col.ColumnName].ToString());
                }

            }
            foreach (DataRow row in BaseTable.Rows)
            {
                DataRow RowGrid = TableGrid.AddRow();
                RowGrid["Field"] = row["Field"];               

                foreach (DataColumn col in BaseTable.Columns)
                {
                    if (col.ColumnName.Contains("План") ||
                        col.ColumnName.Contains("Факт") ||
                        col.ColumnName.Contains("% Исполнения"))
                    {
                        RowGrid[col.ColumnName] = row[col];
                    }
                }
            }

            ClearEmptyCol(TableGrid);

            Grid.DataSource = TableGrid;
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
        GridHeaderLayout GHL;

        private void GenYearHeder(UltraWebGrid Grid)
        {
            List<string> Children = new List<string>();
            string prevYear = "";
            string CurYear = "";
            string CurCaption = "";
            GHL = new GridHeaderLayout(Grid);
            GHL.AddCell("Показатель");
            //GHL.AddCell("Единица измерения");
            foreach (UltraGridColumn col in Grid.Columns)
            {

                if ((col.BaseColumnName.Contains("Факт"))
                    ||
                    (col.BaseColumnName.Contains("План"))
                    ||
                    (col.BaseColumnName.Contains("% Исполнения")))
                {
                    CurYear = col.BaseColumnName.Split(';')[0];
                    CurYear += ", " + EdizmDict[CurYear].ToLower();
                    CurCaption = col.BaseColumnName.Split(';')[1];
                    if (prevYear != "")
                        if (prevYear != CurYear)
                        {
                            GridHeaderCell cell = GHL.AddCell(prevYear);
                            foreach (string caption in Children)
                            {
                                cell.AddCell(caption);
                            }
                            Children = new List<string>();
                            Children.Add(CurCaption);
                        }
                        else
                        {
                            Children.Add(CurCaption);
                        }
                    else
                    {
                        Children.Add(CurCaption);
                    }
                    prevYear = CurYear;
                }
            }
            GridHeaderCell c = GHL.AddCell(prevYear);
            foreach (string caption in Children)
            {
                c.AddCell(caption);
            }



            GHL.ApplyHeaderInfo();
        }

        private void ConfHeader(UltraWebGrid Grid)
        {
            //return;
            Grid.Columns.FromKey("Field").Header.Caption = "Показатель";
            Grid.Columns.FromKey("Field").Header.Style.HorizontalAlign = HorizontalAlign.Center;
            GenYearHeder(Grid);

            foreach (ColumnHeader ch in Grid.Bands[0].HeaderLayout)
            {
                SetDefaultStyleHeader(ch);
            }

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

            Grid.Columns[0].CellStyle.Wrap = true;
            Grid.Columns[1].CellStyle.Wrap = true;
            Grid.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
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

        private static void OtherCustomizerGrid(UltraWebGrid Grid)
        {
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;

            Grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;

            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;

            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }
        }

        private void SetFormatCell(UltraGridCell cell)
        {
            if (cell.Column.BaseColumnName.Contains("% Исполнения"))
            {
                if (cell.Value != null)
                {
                    decimal value = (decimal)cell.Value;

                    cell.Text = string.Format("{0:N2}%", value); 

                    if (value <= 94)
                    {
                        SetImageFromCell(cell, "ballRedBB.png");
                        return;
                    }
                    if (value >= 99)
                    {
                        SetImageFromCell(cell, "ballGreenBB.png");
                        return;
                    }
                    SetImageFromCell(cell, "ballYellowBB.png");

                    
                }


            }
            else
            {
                if (cell.Column.BaseColumnName != "Field")
                {
                    cell.Text = string.Format("{0:N2}", cell.Value);
                }
            }
        }


        bool IsAllProg()
        {
            return true;
                //ComboRegion.SelectedNode.Level == 0;
        }

        private void FormatRow(UltraWebGrid Grid)
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {

                int level = 0;
                    //FieldLevel[Row.Cells[0].Text] - (IsAllProg() ? 2 : 3);

                if (level == 0)
                {
                    Row.Cells[0].Style.Font.Bold = true;
                    //Row.Cells[0].ColSpan = Grid.Columns.Count;
                }
                //if ((level == 1) & (IsAllProg()))
                //{
                //    Row.Cells[0].Style.Font.Bold = true;
                //    Row.Cells[0].ColSpan = Grid.Columns.Count;
                //}

                Row.Cells[0].Style.Padding.Left = level * 15;

                foreach (UltraGridCell cell in Row.Cells)
                {
                      SetFormatCell(cell);
                }
            }
        }

        private void FillColorCell(UltraWebGrid Grid)
        {
            foreach (UltraGridColumn col in Grid.Columns)
            {
                if (col.BaseColumnName.Contains(";"))
                {
                    int year = int.Parse(col.BaseColumnName.Split(';')[0].Replace(" ",""));
                    int SelectYearInt = int.Parse(SelectYear.Value);
                    if (year == SelectYearInt)
                    {
                        col.CellStyle.BackColor = Color.Blue;
                    }
                    if (year == SelectYearInt + 1)
                    {
                        col.CellStyle.BackColor = Color.GreenYellow;
                    }
                    if (year > SelectYearInt + 1)
                    {
                        col.CellStyle.BackColor = Color.Orange;
                    }

                     
                }
            }
        }

        private void CustomizeGrid(UltraWebGrid Grid)
        {
            //FillColorCell(Grid);

            ConfHeader(Grid);

            FormatRow(Grid);

            CustomizerSize.ContfigurationGrid(Grid);

            OtherCustomizerGrid(Grid);

            //Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
        }

        

        void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            LastSelectGridCaption.Value = e.Row.Cells[0].Text;
            ActiveRow(e.Row);
        }





        private void GenerationGrid(UltraWebGrid Grid)
        {
            DataBindGrid(Grid);
            CustomizeGrid(Grid);   
        }

        DataTable TableChart = null;

        private void GenSecondChart()
        {   
            if (!onWall)
            ChartParamType.Value = NaviagatorChart.ActiveRow["UName"].ToString();
            

            TableChart = DBHelper.ExecQueryByID("Chart", "Field");

            if (TableChart.Rows.Count <= 0)
            {
                UltraChart2.DataSource = null;
                UltraChart2.DataBind();
                return;
            }

            decimal max = decimal.MinValue;
            decimal min = decimal.MaxValue;

            foreach (DataRow Row in TableChart.Rows)
            {
                foreach (DataColumn col in TableChart.Columns)
                {
                    if (Row[col] != DBNull.Value)
                    {
                        if (Row[col] is decimal)
                        {
                            if ((decimal)Row[col] == 0)
                            {
                                Row[col] = DBNull.Value;
                            }
                            else
                            {
                                decimal value = (decimal)Row[col];
                                max = max > value ? max : value;
                                min = min < value ? min : value;
                            }
                        } 
                    }
                }
            }

            UltraChart2.Axis.Y.RangeMax = (double)max * 1.1;
            UltraChart2.Axis.Y.RangeMin = (double)min * 0.9;
            UltraChart2.Axis.Y.RangeType = AxisRangeType.Custom;

            //foreach9Data

            UltraChart2.DataSource = TableChart;
            UltraChart2.DataBind();

            //UltraChart2.Data.SwapRowsAndColumns = true;

            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.Axis.X.Extent = 50;
            UltraChart2.Axis.Y.Extent = 70;

            UltraChart2.Axis.X.Margin.Near.Value = 3;
            UltraChart2.Axis.X.Margin.Far.Value = 3;


            UltraChart2.Legend.Visible = false;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ### ### ##0.##>";

            UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 9);
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 9);

            UltraChart2.ColumnChart.ChartText.Add(
                new ChartTextAppearance(UltraChart2, -2, -2, true,
                    new Font("Verdana", 10), Color.Black, "<DATA_VALUE:### ### ### ##0.##>", StringAlignment.Far, StringAlignment.Center, 0));

            UltraChart2.Tooltips.FormatString = string.Format("{0} <br>{1} <br><SERIES_LABEL>, <ITEM_LABEL>: <DATA_VALUE:N2>", ComboCurrentPeriod.SelectedValue,ComboRegion.SelectedValue);

            UltraChart2.ColumnChart.NullHandling = NullHandling.DontPlot;
            //if (!Page.IsPostBack)
            {
                UltraChart2.FillSceneGraph -= new FillSceneGraphEventHandler(Chart_FillSceneGraph);
                UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(Chart_FillSceneGraph);
            }


            UltraChart2.ColorModel.ModelStyle = ColorModels.LinearRange;
            
            UltraChart2.ColorModel.ColorBegin = ChartParamType.Value == ">" ? Color.Blue : Color.Red;
            UltraChart2.ColorModel.ColorEnd = ChartParamType.Value == "<" ? Color.Blue : Color.Red;
            WallLink.Text = "Для&nbsp;видеостены";
            WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());
            
        }  

        void UltraChart2_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
            e.Text = "Отсутствуют";
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

        int GetSosed(int i)
        {
            return Math.Abs(i - 1);
        }

        private Text GetPrimText(int colempty, int rowEmpty, SceneGraph sceneGraph, string path, int x1, int x2)
        {
         
            
            foreach (Primitive p in sceneGraph)
            {
                if (p is Text)
                {
                    Text pText = (Text)p;

                    if (colempty != -1)
                    {
                        if ((colempty == p.Column) && (rowEmpty == p.Row))
                        {
                            return pText;
                        }                        
                    }
                    else                    
                    if (p.Path == path && pText.bounds.X > x1 && pText.bounds.X < x2)
                    {   
                        return pText;
                    }



                    //CRHelper.SaveToErrorLog(pText.
                }
            }
            throw new Exception("----");

        }

        Text getTextP(SceneGraph sceneGraph,  Text p_)
        {
            foreach (Primitive p in sceneGraph)
            {
                if (p is Text)
                {
                    Text pText = (Text)p;

                    if ((pText.bounds.X == p_.bounds.X) 
                        &&
                        (pText.bounds.Y == p_.bounds.Y)                         
                        && (p_ != pText))
                    {
                        return pText;
                    }
                }
            }
            return null;
        }


        void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

            //return;

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            foreach (Primitive p in e.SceneGraph)
            {
                if (p is Box)
                {

                    Box pBox = (Box)p;
                    if (pBox.Row == -1)
                    {
                        continue;
                    }
                    try
                    {
                        if (TableChart.Rows[GetSosed(pBox.Column)][pBox.Row + 1] == DBNull.Value)
                        {
                            int colempty = pBox.Column;
                            //GetSosed(pBox.Column);
                            int rowEmpty = pBox.Row;

                            try
                            {

                                Text emptylabel = GetPrimText(-1, -1, e.SceneGraph, null,
                                    pBox.rect.X - 10, pBox.rect.X + pBox.rect.Width + 10);


                                Text exo = getTextP(e.SceneGraph, emptylabel);
                                if (exo != null)
                                    exo.Visible = false;

                                emptylabel.bounds.X += pBox.rect.Width / 2;
                                //emptylabel.Visible = false;


                                Text emptylabelAxis = GetPrimText(colempty, rowEmpty, e.SceneGraph, "Border.Title.Grid.X", pBox.rect.X - 10, pBox.rect.X + pBox.rect.Width + 10);

                                emptylabelAxis.bounds.X += pBox.rect.Width / 2;

                                Text _emptylabelAxis = GetPrimText(GetSosed(colempty), rowEmpty, e.SceneGraph, "Border.Title.Grid.X", pBox.rect.X - 10, pBox.rect.X + pBox.rect.Width + 10);
                                _emptylabelAxis.Visible = false;

                            }
                            catch { }

                            pBox.rect.X += pBox.rect.Width / 2;
                            //pBox.rect.Width = 2;                        
                        }
                    }
                    catch { }
                }
                else
                {
                    //if (p.Row > 0)
                    //{
                    //    CRHelper.SaveToErrorLog(p.GetType().ToString());
                    //}
                    if (p is Text)
                    {
                        Text t = (Text)p;

                        //CRHelper.SaveToErrorLog(t.GetTextString()+"-"+p.Column.ToString()+"||"+p.Row.ToString()+" path - "+p.Path);

                    }
                }



            }
        }



        #endregion

        Dictionary<string, string> FillDictFormTextovka()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Минимальная заработная плата, установленная в Региональном соглашении (для работников организаций бюджетной сферы, финансируемых из областного и местных бюджетов)", "для работников организаций бюджетной сферы, финансируемых из областного и местных бюджетов");
            dict.Add("Минимальная заработная плата, установленная в Региональном соглашении (для работников организаций внебюджетной сферы, кроме организаций сельского хозяйства)", "для работников организаций внебюджетной сферы, кроме организаций сельского хозяйства");
            dict.Add("Минимальная заработная плата, установленная в Региональном соглашении (для работников организаций сельского хозяйства)", "для работников организаций сельского хозяйства");
            return dict;
        }

        private void SetHeader() 
        {
            PageTitle.Text = string.Format("Финансово-экономические показатели муниципальных унитарных предприятий ({0})", ComboRegion.SelectedValue);

            PageSubTitle.Text = string.Format("Сводный отчет об основных показателях финансово-хозяйственной деятельности муниципальных унитарных предприятий Сахалинской области, {0} ", ComboCurrentPeriod.SelectedValue);

            Page.Title = PageTitle.Text;

            LabelChart2.Text = string.Format("Финансовый результат деятельности муниципальных унитарных предприятий, {0}, тыс. руб.",NaviagatorChart.ActiveRow[0].ToString()); 

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
            //IsSubjectRF = 
            //    ComboRegion.SelectedValue == "Новосибирская область";

            CoolTable TableListForChart = new CoolTable();
            TableListForChart.Columns.Add("Field");
            TableListForChart.Columns.Add("UName");
            TableListForChart.AddOrGetRow("прибыльные")["Uname"] = ">";  
            TableListForChart.AddOrGetRow("убыточные")["Uname"] = "<";

            NaviagatorChart = new RadioButtons(TableListForChart, rb_CheckedChanged);

            PlaceHolder1.Controls.Clear();
            NaviagatorChart.FillPLaceHolder(PlaceHolder1);

            foreach (RadioButton rb in NaviagatorChart.Buttons.Keys)
            {
                if (rb.Text == LastSelectComboBox.Value)
                {
                    //rb.Checked = true;
                }
            }

        }

        private UltraGridRow GetActiveRow()
        {
            foreach (UltraGridRow Row in GridTop.Rows)
            {
                //CRHelper.SaveToErrorLog(
                //   Row.Cells[0].Text + "||" + UserComboBox.getLastBlock(LastSelectGridCaption.Value));
                if (Row.Cells[0].Text == LastSelectGridCaption.Value)
                {
                    return Row;
                }
            }
            return GridTop.Rows[0];
        }

        void rb_CheckedChanged(object sender, EventArgs e)
        {

            NaviagatorChart.ActiveRow = NaviagatorChart.Buttons[(RadioButton)sender];
            //ChangeMapAndChartParam();
            //ActiveRow(GetActiveRow());
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
                            rb.Checked = true;
                        }
                    }
                }
            }
            LastSelectComboBox.Value = NaviagatorChart.ActiveRow[0].ToString();
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

        }

        private void ChosenParm()
        {
            SelectYear.Value = GetDijghtYear(
                ComboCurrentPeriod.SelectedNode.Parent.Text);

            QuartType.Value = string.Format(
                CheckBox1.Checked ?
  @"member [Период__Период].[Период__Период].[Квартал 1]
    as '[Период__Период].[Период__Период].[Год].[{0}].[Полугодие 1].[Квартал 1]'
    member [Период__Период].[Период__Период].[Квартал 2]
    as 'iif(not [Организации__Показатели работы].[Организации__Показатели работы].currentmember is 
    [Организации__Показатели работы].[Организации__Показатели работы].[Все показатели].[Среднемесячная заработная плата одного работника],
    [Период__Период].[Период__Период].[Год].[{0}].[Полугодие 1].[Квартал 2] - [Период__Период].[Период__Период].[Квартал 1],
    [Период__Период].[Период__Период].[Год].[{0}].[Полугодие 1].[Квартал 2])'
   member [Период__Период].[Период__Период].[Квартал 3]
   as 'iif(not [Организации__Показатели работы].[Организации__Показатели работы].currentmember is 
    [Организации__Показатели работы].[Организации__Показатели работы].[Все показатели].[Среднемесячная заработная плата одного работника],
[Период__Период].[Период__Период].[Год].[{0}].[Полугодие 2].[Квартал 3] - 
       [Период__Период].[Период__Период].[Год].[{0}].[Полугодие 1].[Квартал 2],
[Период__Период].[Период__Период].[Год].[{0}].[Полугодие 2].[Квартал 3])'
   member [Период__Период].[Период__Период].[Квартал 4]
   as 'iif(not [Организации__Показатели работы].[Организации__Показатели работы].currentmember is 
    [Организации__Показатели работы].[Организации__Показатели работы].[Все показатели].[Среднемесячная заработная плата одного работника],
[Период__Период].[Период__Период].[Год].[{0}].[Полугодие 2].[Квартал 4] - 
       [Период__Период].[Период__Период].[Год].[{0}].[Полугодие 2].[Квартал 3], 
       [Период__Период].[Период__Период].[Год].[{0}].[Полугодие 2].[Квартал 4])'" :
    @"member [Период__Период].[Период__Период].[Квартал 1]
    as '[Период__Период].[Период__Период].[Год].[{0}].[Полугодие 1].[Квартал 1]'
    member [Период__Период].[Период__Период].[Квартал 2]
    as '[Период__Период].[Период__Период].[Год].[{0}].[Полугодие 1].[Квартал 2]'
    member [Период__Период].[Период__Период].[Квартал 3]
    as '[Период__Период].[Период__Период].[Год].[{0}].[Полугодие 2].[Квартал 3]'
    member [Период__Период].[Период__Период].[Квартал 4]
    as '[Период__Период].[Период__Период].[Год].[{0}].[Полугодие 2].[Квартал 4]'", SelectYear.Value);

            SelectYear.Value = string.Format("[Период__Период].[Период__Период].[{0}]", DataSetComboYear.OtherInfo[ComboCurrentPeriod.SelectedValue]["Quart"]);
            //DataSetComboYear.DataUniqeName[ComboCurrentPeriod.SelectedValue];

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            Comennt.Value = "";

            FillCombo();

            ChosenParm();

            GenerationGrid(GridTop);
            
            FillNaviagatorPanel();

            ChangeMapAndChartParam();

            GenSecondChart();

            SetHeader();

            SetEmptyChartStyle();
        }

        private UltraGridRow ChangeValueRow(UltraGridRow Row)
        {
            return Row;
        }

        private void UnSelectAllRow(UltraWebGrid GridTop)
        {
            foreach (UltraGridRow Row in GridTop.Rows)
            {
                Row.Selected = false;
                Row.Activated = false;
            }
        }

        private void ActiveRow(UltraGridRow Row)
        {
            Row = ChangeValueRow(Row);

            UnSelectAllRow(GridTop);

            Row.Activate();
            Row.Activated = true; 
            Row.Selected = true;
             
            ChartParam.Value = 
                string.Format("[Стратегия развития__Показатели].[Стратегия развития__Показатели].[{0}]",
                Row.Cells[0].Text);

            
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

            foreach(UltraGridRow row in GHL.Grid.Rows)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    try 
                    {
                        //this is magicka )))))))))))))
                        cell.Value = string.Format("{0:N2}", cell.Value).Replace(" ", " ");                         
                    }
                    catch { }
                    
                }
            }

            ReportPDFExporter1.Export(GHL, section1);            

            section2.PageSize = new PageSize(section1.PageSize.Height, section1.PageSize.Width);

            UltraChart2.Width = 1000;
            ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text+string.Format(", {0}, {1}",ComboCurrentPeriod.SelectedValue,ComboRegion.SelectedValue), section2);

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
            ReportExcelExporter1.Export(GHL, sheet1, 2);

            for (int i = 1; i < GridTop.Columns.Count; i++)
            {
                sheet1.Columns[i].Width = 150 * 30;
            }

            UltraChart2.Width = 800;

            sheet4.Columns[0].Width = 300 * 100;

            ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet4, 3);
        }


        #endregion


    }
}