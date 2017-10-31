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


namespace Krista.FM.Server.Dashboards.reports.org_gkh_novosib_001
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

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * 8;
                }
                Grid.Columns[0].Width = onePercent * 18;

                Grid.Columns[2].Width = onePercent * 9;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * 8;
                }
                Grid.Columns[0].Width = onePercent * 18;

                Grid.Columns[2].Width = onePercent * 9;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;

                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * 8;
                }
                Grid.Columns[0].Width = onePercent * 18;

                Grid.Columns[2].Width = onePercent * 9;
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
                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UName"].ToString();

                    string DisplayName = row[0].ToString();

                    this.AddItem(DisplayName, 0, UniqueName);

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
            GridTop.Height = CustomizerSize.GetGridHeight();
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
                ComboRegion.SelectLastNode();
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
            }
            SelectRegion.Value = DataSetComboRealRegion.DataUniqeName[ComboRealRegion.SelectedValue];
        }

        private void FillCombo()
        {
            ComboRealRegion.Width = 250;
            ComboRealRegion.Title = "Территория";
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

        DataTable BaseTableCompare = null;
        DataTable BaseTableSelect = null;

        CoolTable TableGrid;

        private void DataBindGrid(UltraWebGrid Grid)
        {
            #region
            //Fields.Value = Grid == GridTop ? FieldTopGrid : FieldBootomGrid;

            //ChangeParam(true, Grid == GridTop);
            //BaseTableCompare = DBHelper.ExecQueryByID("Grid", "Field");

            //ChangeParam(false, Grid == GridTop);
            //BaseTableSelect = DBHelper.ExecQueryByID("Grid", "Field");

            //TableGrid = new CoolTable();

            //foreach (DataColumn BaseCol in BaseTableSelect.Columns)
            //{
            //    TableGrid.Columns.Add(BaseCol.ColumnName, BaseCol.DataType);
            //}

            //foreach (DataRow BaseRow in BaseTableSelect.Rows)
            //{
            //    string Field = BaseRow["Field"].ToString();

            //    InfoRow RowInfo = new InfoRow(Field);

            //    DataRow GridRowValue = TableGrid.AddRow();

            //    DataRow GridRowDeviation = null;
            //    DataRow GridRowSpeedDeviation = null;

            //    if (!RowInfo.OnlyRow)
            //    {
            //        GridRowDeviation = TableGrid.AddRow();
            //        GridRowSpeedDeviation = TableGrid.AddRow();

            //    }



            //    GridRowValue["Field"] = Field;

            //    DataRow CompareRow = GetRowFromValueFirstCell(BaseTableCompare, Field);

            //    for (int i = 1; i < BaseTableSelect.Columns.Count; i++)
            //    {
            //        GridRowValue[i] = BaseRow[i];

            //        if (!RowInfo.OnlyRow)
            //        {
            //            //if (BaseTableSelect.Columns[i].ColumnName != BaseTableCompare.Columns[i].ColumnName)
            //            {
            //                GridRowDeviation[i] = GetDeviation(BaseRow[i], CompareRow[i]);
            //            }

            //            if (RowInfo.Rang)
            //            {
            //                GridRowDeviation[i] = GetRang(Field, BaseTableSelect.Columns[i].ColumnName, RowInfo.ReveceRang);
            //            }

            //            //if (BaseTableSelect.Columns[i].ColumnName != BaseTableCompare.Columns[i].ColumnName)
            //            {
            //                GridRowSpeedDeviation[i] = GetSpeedDeviation(BaseRow[i], CompareRow[i]);
            //            }
            //        }
            //    }
            //}

            //Grid.DataSource = TableGrid;
            //Grid.DataBind();
            #endregion
            DataTable TableGrid = DBHelper.ExecQueryByID("Grid", "org");
            string lastReg = "";
            foreach (DataRow Row in TableGrid.Rows)
            {
                string CurReg = Row["org"].ToString().Split(';')[0];
                string CurOrg = Row["org"].ToString().Split(';')[1];

                if (CurOrg.Contains("Все организации"))
                {
                    Row["org"] = "Итого <b>" + CurReg + "</b>";
                }
                else
                {
                    Row["org"] = CurOrg;
                }
                if (CurOrg.Contains("Данные всех источников"))
                {
                    Row["org"] = "<b>ВСЕГО<b>";
                }
            }

            Grid.DataSource = TableGrid;
            Grid.DataBind();
        }

        GridHeaderLayout HL;
        private void ConfHeader(UltraWebGrid Grid)
        {
            HL = new GridHeaderLayout(Grid);
            HL.AddCell("Наименование организации ЖКХ");
            HL.AddCell("ИНН", 2);
            HL.AddCell("Муниципальное образование", 2);


            GridHeaderCell cell = HL.AddCell("ДОХОДЫ, тыс. рублей");
            GridHeaderCell cell1 = cell.AddCell("План");
            cell1.AddCell("на год");
            cell1.AddCell("на текущий месяц");

            GridHeaderCell cell2 = cell.AddCell("Факт");
            cell2.AddCell("с начала года");
            cell2.AddCell("% исполнения плана с начала года");
            cell2.AddCell("за текущий месяц");
            cell2.AddCell("% исполнения плана на месяц");
            cell2.AddCell("Темп роста к предыдущему месяцу");

            cell = HL.AddCell("РАСХОДЫ, тыс. рублей");
            cell1 = cell.AddCell("План");
            cell1.AddCell("на год");
            cell1.AddCell("на текущий месяц");

            cell2 = cell.AddCell("Факт");
            cell2.AddCell("с начала года");
            cell2.AddCell("% исполнения плана с начала года");
            cell2.AddCell("за текущий месяц");
            cell2.AddCell("% исполнения плана на месяц");
            cell2.AddCell("Темп роста к предыдущему месяцу");

            cell = HL.AddCell("Тепла отпущено в натуральном выражении, Гкал");
            cell1 = cell.AddCell("План");
            cell1.AddCell("на год");
            cell1.AddCell("на текущий месяц");

            cell2 = cell.AddCell("Факт");
            cell2.AddCell("с начала года");
            cell2.AddCell("% исполнения плана с начала года");
            cell2.AddCell("за текущий месяц");
            cell2.AddCell("% исполнения плана на месяц");
            cell2.AddCell("Темп роста к предыдущему месяцу");

            cell = HL.AddCell("Себестоимость, рублей / 1 Гкал");
            cell1 = cell.AddCell("План");
            cell1.AddCell("на год");
            cell1.AddCell("на текущий месяц");

            cell2 = cell.AddCell("Факт");
            cell2.AddCell("с начала года");
            cell2.AddCell("% исполнения плана с начала года");
            cell2.AddCell("за текущий месяц");
            cell2.AddCell("% исполнения плана на месяц");
            cell2.AddCell("Темп роста к предыдущему месяцу");

            HL.AddCell("Тариф на год, рублей / 1 Гкал");
            HL.AddCell("Топливная составляющая на год, %");

            cell1 = HL.AddCell("Фактическая задолженность, тыс. руб.");
            cell1.AddCell("Дебиторская", 2);
            cell1.AddCell("Кредиторская", 2);

            cell = HL.AddCell("Доля расходов на топливо в общем объеме доходов нарастающим итогом за год");

            HL.ApplyHeaderInfo();

            foreach (HeaderBase hb in Grid.Bands[0].HeaderLayout)
            {
                hb.Style.HorizontalAlign = HorizontalAlign.Center;
                hb.Style.Wrap = true;
            }

            foreach (UltraGridColumn Col in Grid.Columns)
            {
                if (Col.Index > 2)
                    CRHelper.FormatNumberColumn(Col, "N2");
                Col.CellStyle.Wrap = true;
            }

            Grid.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            Grid.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            int adeded = 2;

            CRHelper.FormatNumberColumn(Grid.Columns[4 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[6 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[7 + adeded], "P2");

            CRHelper.FormatNumberColumn(Grid.Columns[4 + 7 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[6 + 7 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[7 + 7 + adeded], "P2");

            CRHelper.FormatNumberColumn(Grid.Columns[4 + 7 + 7 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[6 + 7 + 7 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[7 + 7 + 7 + adeded], "P2");


            CRHelper.FormatNumberColumn(Grid.Columns[4 + 7 + 7 + 7 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[6 + 7 + 7 + 7 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[7 + 7 + 7 + 7 + adeded], "P2");


            CRHelper.FormatNumberColumn(Grid.Columns[6 + 7 + 7 + 7 + 3 + adeded], "P2");
            CRHelper.FormatNumberColumn(Grid.Columns[7 + 7 + 7 + 7 + 2 + 3 + adeded], "P2");

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
            UltraGridCell IndicatorCell = GetNextCell(GetNextCell(ValueCell));
            if (IndicatorCell.Value != null)
            {

                decimal Value = decimal.Parse(IndicatorCell.Value.ToString().Replace("%", ""));
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
                SetFormatCell(GetNextCell(GetNextCell(ValueCell)), RowInfo.FormatString);


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

                string ColName = ValueCell.Column.BaseColumnName;
                string RegionName = ValueCell.Row.Cells[0].Text;

                SetIndicatorDeviationcell(ValueCell, false);
                SetIndicatorSpeedDeviationcell(ValueCell, RowInfo.Revece);
            }
        }

        private void setImageSZ(UltraGridRow Row)
        {
            try
            {
                int lastCell = Row.Cells.Count;
                decimal CompareValue = (decimal)Row.Cells[lastCell - 1].Value;
                decimal CompareValue2 = (decimal)Row.Cells[lastCell - 4].Value;
                if (CompareValue > CompareValue2)
                {
                    Row.Cells[lastCell - 1].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    Row.Cells[lastCell - 1].Title = "Доля расходов на топливо в общем объеме доходов больше установленного размера топливной составляющей";

                }
                else

                    if (CompareValue < CompareValue2)
                    {
                        Row.Cells[lastCell - 1].Style.BackgroundImage = "~/images/ballRedBB.png";
                        Row.Cells[lastCell - 1].Title = "Доля расходов на топливо в общем объеме доходов меньше установленного размера топливной составляющей";
                    }
                    else
                    {
                        Row.Cells[lastCell - 1].Style.BackgroundImage = "~/images/ballOrangeBB.png";
                        Row.Cells[lastCell - 1].Title = "Доля расходов на топливо в общем объеме доходов равна установленному размеру топливной составляющей";
                    }
                Row.Cells[lastCell - 1].Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
            }
            catch { }

        }

        private string GenerateUrlRow(string p, string p2, string p3)
        {
            MegaParam.Value = ComboRegion.SelectedValue + "|" + p.Remove(0, 1) + "|" + p2 + "|" + p3;
            return string.Format("<a href = \"{1}\">{0}</a>", p, UserParams.GetCurrentReportParamList()).Replace("001", "002");
        }

        private void FormatRow(UltraWebGrid Grid)
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                setImageSZ(Row);

                if ((Row.Cells[0].Text.Contains("Итог")) ||
                    (Row.Cells[0].Text.Contains("ВСЕГО")))
                {
                    Row.Style.BackColor = Color.FromArgb(241, 241, 242);
                }
                else
                {
                    Row.Style.BackColor = Color.Transparent;
                }
                if (!Row.Cells[0].Text.Contains("Итог"))
                    Row.Cells[0].Text = GenerateUrlRow(Row.Cells[0].Text, Row.Cells[1].Text, Row.Cells[2].Text);

            }
        }

        private static void OtherCustomizerGrid(UltraWebGrid Grid)
        {
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.NullTextDefault = "-";

            for (int i = 1; i < Grid.Columns.Count; i++)
            {
                Grid.Columns[i].DataType = "decimal";
            }
        }

        private void ClearSpecifickCell(UltraWebGrid Grid)
        {
            UltraGridRow LAstrow = Grid.Rows[Grid.Rows.Count - 1];
            LAstrow.Cells[29].Value = null;
            LAstrow.Cells[30].Value = null;
            LAstrow.Cells[33].Value = null;
            LAstrow.Cells[33].Style.BackgroundImage = "";
        }

        private void CustomizeGrid(UltraWebGrid Grid)
        {
            ConfHeader(Grid);

            FormatRow(Grid);

            CustomizerSize.ContfigurationGrid(Grid);

            OtherCustomizerGrid(Grid);

            ClearSpecifickCell(Grid);
        }

        private void GenerationGrid(UltraWebGrid Grid)
        {
            DataBindGrid(Grid);
            GBT.Text = "";
            if (GridTop.Rows.Count <= 1)
            {
                Grid.DataSource = null;
                Grid.DataBind();
                new Exception("exeption");
            }
            else
            {
                GBT.Text = "";
            }
            CustomizeGrid(Grid);
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            foreach (Primitive p in e.SceneGraph)
            {
                if (p is Box)
                {
                    try
                    {
                        Box b = (Box)p;
                        if (b.PE.FillStopColor == Color.Coral)
                        {

                            if (string.IsNullOrEmpty(b.Path))
                            {
                                if (b.Column == 0)
                                {
                                    b.rect.X += b.rect.Width / 2;
                                }
                                else
                                {
                                    b.rect.X -= b.rect.Width / 2;
                                }
                            }

                        }

                    }
                    catch { }
                }
            }
        }

        private void GenSecondChart()
        {

            DataTable TableChart = DBHelper.ExecQueryByID("Grid", "Field");
            if (TableChart.Columns.Count == 2)
            {
                return;
            }
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

        private void SetHeader()
        {
            PageTitle.Text = "";
            PageSubTitle.Text = "";

            PageTitle.Text = string.Format("Мониторинг деятельности организаций ЖКХ");
            Page.Title = PageTitle.Text;
            if (!string.IsNullOrEmpty(ComboRegion.SelectedValue.ToLower()))
                PageSubTitle.Text = string.Format("Данные ежемесячного мониторинга деятельности организаций ЖКХ {0} за {1}", "Новосибирской области", ComboRegion.SelectedValue.ToLower());
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
                try
                {
                    SetHeader();
                }
                catch { }

                FillCombo();

                try
                {
                    SetHeader();
                }
                catch { }
                GenerationGrid(GridTop);


            }
            catch
            {

                GridTop.DisplayLayout.NoDataMessage = "В настоящий момент данные отсутствуют";
            }
        }

        #region Экспорт в PDF

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


            foreach (UltraGridRow Row in HL.Grid.Rows)
            {
                Row.Cells[0].Text = Row.Cells[0].Text.Replace("<b>", "").Replace("</b>", "");
                try
                {
                    Row.Cells[0].Text = Row.Cells[0].Text.Split('>')[1].Split('<')[0];
                }
                catch { }
                foreach (UltraGridCell cell in Row.Cells)
                {
                    if (cell.Value == null)
                    {
                        cell.Text = "-";
                    }

                }
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
                if (Row.Cells[0].Text.Contains(">"))
                {
                    Row.Cells[0].Text = Row.Cells[0].Text.Split('>')[1].Split('<')[0];  
                }
            }
            ReportExcelExporter1.Export(HL, sheet1, 4);
            sheet1.Rows[4].Height = 25 * 20;
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