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


namespace Krista.FM.Server.Dashboards.CTAT_ecology
{
    public partial class _default : CustomReportPage
    {
        string PrevDate = "2009";
        string CurDate = "2010";

        IDataSetCombo SetQuart;

        IPeriodPars PeriodPars;

        ICustomizerSize CustomizerSize;

        CustomParam ChosenPeriod { get { return (UserParams.CustomParam("ChosenPeriod")); } }
        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }

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
                ////String.Format(Width.

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
                    col.Width = onePercent * (100 - 19) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 18;
            }

            #endregion

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

        class DataSetComboYearQuart : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LastParent = "";

                foreach (DataRow row in Table.Rows)
                {
                    int level = int.Parse(row["Level"].ToString()) / 2 - 1;

                    string caption = row["DisplayName"].ToString().Replace("ДАННЫЕ)", "").Replace("(", "");

                    this.AddItem(caption, level, row["UniqueName"].ToString());
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

            UltraGridExporter1.PdfExportButton.Visible = false;//Click += new EventHandler(PdfExportButton_Click);

            //UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_Test);
            //UltraGridExporter1.PdfExporter.RowExported += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportedEventArgs>(PdfExporter_RowExported);


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

            ChartIZO.Width = CustomizerSize.GetChartWidth();

            ChartSZCur.Width = CustomizerSize.GetChartWidth()/2-30;
            ChartSZPrev.Width = CustomizerSize.GetChartWidth()/2-30;
            ChartSZLegend.Width = CustomizerSize.GetChartWidth();
            ChartSZLegend.Height = 40;
        }

        private void FillComboTypePeriod()
        {

            ComboTypePeriod.Width = 400;
            Dictionary<string, int> Tp = new System.Collections.Generic.Dictionary<string, int>();
            Tp.Add("к предыдущему периоду", 0);
            Tp.Add("к началу года", 0);
            Tp.Add("к аналогичному периоду прошлого года", 0);

            ComboTypePeriod.FillDictionaryValues(Tp);
        }

        private void FillComboYear()
        {
            ComboQuart.Width = 200;
            ComboQuart.ParentSelect = true;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboPeriod", "DisplayName");

            SetQuart = new DataSetComboYearQuart();
            SetQuart.LoadData(Table);
            if (!Page.IsPostBack)
            {
                ComboQuart.FillDictionaryValues(SetQuart.DataForCombo);
                ComboQuart.SetСheckedState(SetQuart.LastAdededKey, 1 == 1);
            }
        }

        private void FillCombo()
        {
            FillComboYear();
            if (!Page.IsPostBack)
            {
                FillComboTypePeriod();
            }
        }

        interface IPeriodPars
        {
            string AnalogPrevPeriod();
            string DisplayText();
            string DisplayLegendChartText();

            string FirstYearDate();
            string PrevPeriodPars();
        }

        class PeriodYearPars : IPeriodPars
        {
            int Year;

            public static string bindDate(int y)
            {
                return string.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].DATAMEMBER", y);
            }

            public string AnalogPrevPeriod()
            {

                int NewYear = Year - 1;

                return bindDate(NewYear);
            }

            public string FirstYearDate()
            {

                int NewYear = Year - 10;

                return bindDate(NewYear);

                //throw new Exception("Невзможная дата");
            }

            public string PrevPeriodPars()
            {
                int NewYear = Year - 1;

                return bindDate(NewYear);
            }

            public string DisplayText()
            {
                return Year.ToString() + " году";
            }
            public string DisplayLegendChartText()
            {
                return Year.ToString() + " год";
            }

            public PeriodYearPars(string uniqueDate)
            {
                string[] b = { "].[" };

                string[] parsedate = uniqueDate.Split(b, StringSplitOptions.None);

                int C = parsedate.Length - 1;

                Year = int.Parse(parsedate[C].Split(']')[0]);
            }
        }

        class PeriodMounthPars : IPeriodPars
        {
            int Year;
            int HalgYear;
            int Quart;
            int Mounth;


            public static string bindDate(int y, int h, int q, int m)
            {
                return string.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", y, h, q, CRHelper.RusMonth(m));
            }

            public string AnalogPrevPeriod()
            {

                int NewYear = Year - 1;
                int NewHalgYear = HalgYear;
                int NewQuart = Quart;
                int NewMounth = Mounth;

                return bindDate(NewYear, NewHalgYear, NewQuart, NewMounth);
            }

            public string FirstYearDate()
            {

                int NewYear = Year;
                int NewHalgYear = 1;
                int NewQuart = 1;
                int NewMounth = 0;
                
                if ((0 == Mounth) || (Mounth == 1))
                {
                    return bindDate(NewYear - 10, NewHalgYear, NewQuart, NewMounth);
                }

                return bindDate(NewYear, NewHalgYear, NewQuart, NewMounth);
            }

            public string PrevPeriodPars()
            {
                int NewYear = Year;
                int NewMounth = Mounth - 1;
                if (NewMounth <= 0)
                {
                    NewMounth = 12;
                    NewYear--;

                }

                int NewHalgYear = CRHelper.HalfYearNumByMonthNum(NewMounth);
                int NewQuart = CRHelper.QuarterNumByMonthNum(NewMounth);

                return bindDate(NewYear, NewHalgYear, NewQuart, NewMounth);
            }


            public string DisplayLegendChartText()
            {
                return CRHelper.RusMonth(Mounth) + " " + Year.ToString() + " годa";
            }

            public string DisplayText()
            {
                return CRHelper.RusMonthPrepositional(Mounth) + " " + Year.ToString() + " года";
            }


            public PeriodMounthPars(string uniqueDate)
            {
                string[] b = { "].[" };

                string[] parsedate = uniqueDate.Split(b, StringSplitOptions.None);

                int C = parsedate.Length - 1;

                Mounth = (CRHelper.MonthNum(parsedate[C].Split(']')[0]));
                Quart = (int.Parse(parsedate[C - 1].Split(' ')[1].Split(']')[0]));
                HalgYear = int.Parse(parsedate[C - 2].Split(' ')[1]);
                Year = int.Parse(parsedate[C - 3]);
            }
        }


        private string ChosePrevPeriod()
        {

            if ((ChosenPeriod.Value.Contains("DATAMEMBER")))
            {
                PeriodPars = new PeriodYearPars(ChosenPeriod.Value);
            }
            else
            {
                PeriodPars = new PeriodMounthPars(ChosenPeriod.Value);
            }


            if (ComboTypePeriod.SelectedValue == "к предыдущему периоду")
            {
                return PeriodPars.PrevPeriodPars();
            }
            else
                if (ComboTypePeriod.SelectedValue == "к началу года")
                {
                    return PeriodPars.FirstYearDate();
                }
                else
                {
                    return PeriodPars.AnalogPrevPeriod();
                }
        }

        IPeriodPars GetPrevPeriod()
        {
            IPeriodPars x;

            if ((ChosenPeriod.Value.Contains("DATAMEMBER")))
            {
                x = new PeriodYearPars(ChosenPeriod.Value);

                if (ComboTypePeriod.SelectedValue == "к предыдущему периоду")
                {
                    return new PeriodYearPars(x.PrevPeriodPars());
                }
                else
                    if (ComboTypePeriod.SelectedValue == "к началу года")
                    {
                        return new PeriodYearPars(x.FirstYearDate());
                    }
                    else
                    {
                        return new PeriodYearPars(x.AnalogPrevPeriod());
                    }

            }
            else
            {
                x = new PeriodMounthPars(ChosenPeriod.Value);

                if (ComboTypePeriod.SelectedValue == "к предыдущему периоду")
                {
                    return new PeriodMounthPars(x.PrevPeriodPars());
                }
                else
                    if (ComboTypePeriod.SelectedValue == "к началу года")
                    {
                        return new PeriodMounthPars(x.FirstYearDate());
                    }
                    else
                    {
                        return new PeriodMounthPars(x.AnalogPrevPeriod());
                    }
            }
        }

        private void ChosenParam()
        {
            ChosenPeriod.Value = SetQuart.DataUniqeName[ComboQuart.SelectedValue];
            ChosenPrevPeriod.Value = ChosePrevPeriod();
        }

        DataTable BaseTable = null;

        string[] SZ_CAPTION_CHART = new string[5] { "несмари сюда!!!!!", "Низкая", "Повышенная", "Высокая", "Очень высокая" };

        string[] SZ_CAPTION = new string[5] { "отведи глаза влево!", "низкое", "повышенное", "высокое", "очень высокое" };
        string[] substance_CAPTION = new string[7] { "отведи глаза вправо!", "ВВ, NO2, NO, ФНЛ, Ф;", "CO, NO2, NO, ФНЛ, Ф;", "Ф, ФНЛ, NO2;", "Ф, ФНЛ, NO3;", "Ф, ФНЛ;", "Ф." };

        #region Grid

        private void DataBindGrid()
        {

            BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Field");

            DataTable TableGrid = new DataTable();

            DataColumnCollection ReportCols = TableGrid.Columns;

            ReportCols.Add("Field");

            ReportCols.Add("Cur-IZA", typeof(Decimal));
            ReportCols.Add("Prev-IZA", typeof(Decimal));

            ReportCols.Add("Cur-SZ");
            ReportCols.Add("Prev-SZ");



            ReportCols.Add("substance");

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                DataRow ValRow = TableGrid.NewRow();
                DataRow DeviationRow = TableGrid.NewRow();
                DataRow SpeedDeviationRow = TableGrid.NewRow();

                ValRow["Field"] = BaseRow["Field"];
                ValRow["Cur-IZA"] = BaseRow["Cur-IZA"];

                ValRow["Prev-IZA"] = BaseRow["Prev-IZA"];

                if (BaseRow["Cur-SZ"] != DBNull.Value)
                {
                    ValRow["Cur-SZ"] = SZ_CAPTION[int.Parse(string.Format("{0:N0}", BaseRow["Cur-SZ"]))];
                }

                if (BaseRow["Prev-SZ"] != DBNull.Value)
                {

                    ValRow["Prev-SZ"] = SZ_CAPTION[int.Parse(string.Format("{0:N0}", BaseRow["Prev-SZ"]))];
                }

                if (BaseRow["substance"] != DBNull.Value)
                {
                    ValRow["substance"] = substance_CAPTION[int.Parse(string.Format("{0:N0}", BaseRow["substance"]))];
                }

                if ((ValRow["Cur-IZA"] != DBNull.Value) && (ValRow["Prev-IZA"] != DBNull.Value))
                {
                    DeviationRow["Cur-IZA"] = (System.Decimal)ValRow["Cur-IZA"] - (System.Decimal)ValRow["Prev-IZA"];
                    SpeedDeviationRow["Cur-IZA"] = 100 * (System.Decimal)ValRow["Cur-IZA"] / (System.Decimal)ValRow["Prev-IZA"];
                }

                TableGrid.Rows.Add(ValRow);
                TableGrid.Rows.Add(DeviationRow);
                TableGrid.Rows.Add(SpeedDeviationRow);
            }


            int gorCount = 0, moCount = 0;
            for (int i = 0; i < TableGrid.Rows.Count; i+=3)
            {
                if (TableGrid.Rows[i][0].ToString().StartsWith("Город"))
                {
                    gorCount += 1;
                }
                else
                {
                    moCount += 1;
                }
            }
            string[] goroda = new string[gorCount];
            string[] mo = new string[moCount];
            int k = 0, l = 0;
            for (int i = 0; i < TableGrid.Rows.Count; i+=3)
            {
                if (TableGrid.Rows[i][0].ToString().StartsWith("Город"))
                {
                    goroda[k] = TableGrid.Rows[i][0].ToString().Replace("Город", String.Empty).Replace(" ", String.Empty);
                    k += 1;
                }
                else
                {
                    mo[l] = TableGrid.Rows[i][0].ToString();
                    l += 1;
                }
            }
            Array.Sort(goroda);
            Array.Sort(mo);
            DataTable normDt = new DataTable();
            for (int i = 0; i < TableGrid.Columns.Count; i++)
            {
                normDt.Columns.Add(TableGrid.Columns[i].ColumnName, TableGrid.Columns[i].DataType);
            }

            for (int j = 0; j < TableGrid.Rows.Count; j+=3)
            {
                if (TableGrid.Rows[j][0].ToString() == "Город Ханты-Мансийск")
                {
                    normDt.Rows.Add(TableGrid.Rows[j].ItemArray);
                    normDt.Rows.Add(TableGrid.Rows[j+1].ItemArray);
                    normDt.Rows.Add(TableGrid.Rows[j+2].ItemArray);
                }
            }

            for (int i = 0; i < goroda.Length; i++)
            {
                for (int j = 0; j < TableGrid.Rows.Count; j+=3)
                {
                    if (TableGrid.Rows[j][0].ToString() == "Город " + goroda[i] && TableGrid.Rows[j][0].ToString() != "Город Ханты-Мансийск")
                    {
                        normDt.Rows.Add(TableGrid.Rows[j].ItemArray);
                        normDt.Rows.Add(TableGrid.Rows[j+1].ItemArray);
                        normDt.Rows.Add(TableGrid.Rows[j+2].ItemArray);
                    }
                }
            }

            for (int i = 0; i < mo.Length; i++)
            {
                for (int j = 0; j < TableGrid.Rows.Count; j+=3)
                {
                    if (TableGrid.Rows[j][0].ToString() == mo[i])
                    {
                        normDt.Rows.Add(TableGrid.Rows[j].ItemArray);
                        normDt.Rows.Add(TableGrid.Rows[j+1].ItemArray);
                        normDt.Rows.Add(TableGrid.Rows[j+2].ItemArray);
                    }
                }
            }
            Grid.DataSource = normDt; 
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
            ConfHeader(cols.FromKey("Field").Header, 0, 0, 1, 2, "МР ГО");

            ConfHeader(cols.FromKey("Cur-IZA").Header, 1, 1, 1, 1, "Отчетный период");
            ConfHeader(cols.FromKey("Prev-IZA").Header, 2, 1, 1, 1, UserComboBox.getLastBlock(ChosenPrevPeriod.Value) + " " + ChosenPrevPeriod.Value.Split(']')[3].Remove(0,2)+" года");

            ConfHeader(cols.FromKey("Cur-SZ").Header, 3, 1, 1, 1, "Отчетный период");
            ConfHeader(cols.FromKey("Prev-SZ").Header, 4, 1, 1, 1, UserComboBox.getLastBlock(ChosenPrevPeriod.Value) + " " + ChosenPrevPeriod.Value.Split(']')[3].Remove(0, 2) + " года");
            ConfHeader(cols.FromKey("substance").Header, 5, 0, 1, 2, "Вещества, вносящие основной вклад в загрязнение");

            Grid.Bands[0].HeaderLayout.Add(GenHeader("ИЗА", 1, 0, 2, 1));
            Grid.Bands[0].HeaderLayout.Add(GenHeader("Степень загрязнения", 3, 0, 2, 1));

            Grid.Bands[0].HeaderLayout[0].Style.BorderDetails.ColorLeft = Color.LightGray;

            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.DataType = "string";
            }
        }


        public static void SetImageFromGrowth(UltraGridCell cell, decimal value)
        {
            bool IsReverce = false;

            decimal valShift = value;

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

        void setImageSZ(UltraGridCell cell)
        {


            if (cell.Text == "низкое")
            {
                cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
            }

            if (cell.Text == "повышенное")
            {
                cell.Style.BackgroundImage = "~/images/ballYellowBB.png";
            }

            if (cell.Text == "высокое")
            {
                cell.Style.BackgroundImage = "~/images/ballOrangeBB.png";
            }

            if (cell.Text == "очень высокое")
            {
                cell.Style.BackgroundImage = "~/images/ballRedBB.png";
            }

            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
        }

        private void FormatGridValueRow(UltraGridRow Row)
        {
            setImageSZ(Row.Cells.FromKey("Cur-SZ"));
            setImageSZ(Row.Cells.FromKey("Prev-SZ"));



            foreach (UltraGridCell cell in Row.Cells)
            {
                string ColName = cell.Column.BaseColumnName;
                if ((Row.NextRow.Cells.FromKey(ColName).Value == null) && (Row.NextRow.NextRow.Cells.FromKey(ColName).Value == null))
                {
                    cell.RowSpan = 3;
                }
            }
            if (Row.NextRow.Cells.FromKey("Cur-IZA").Value != null)
                Row.Cells.FromKey("Cur-IZA").Style.BorderDetails.ColorBottom = Color.Transparent;



        }

        private void FormatGridDeviationRow(UltraGridRow ultraGridRow)
        {
            if (ultraGridRow.Cells.FromKey("Cur-IZA").Value != null)
            {
                SetImageFromGrowth(ultraGridRow.Cells.FromKey("Cur-IZA"), (decimal)ultraGridRow.Cells.FromKey("Cur-IZA").Value);
                ultraGridRow.Cells.FromKey("Cur-IZA").Title = "отклонение от предыдущего периода";
            }
            if (ultraGridRow.Cells.FromKey("Cur-IZA").Value != null)
            {
                ultraGridRow.Cells.FromKey("Cur-IZA").Style.BorderDetails.ColorTop = Color.Transparent;
                ultraGridRow.Cells.FromKey("Cur-IZA").Style.BorderDetails.ColorBottom = Color.Transparent;
            }
        }

        private void FormatGridSpeedDeviationRow(UltraGridRow ultraGridRow)
        {
            if (ultraGridRow.Cells.FromKey("Cur-IZA").Value != null)
            {
                ultraGridRow.Cells.FromKey("Cur-IZA").Title = "темп роста к предыдущему периоду";

                ultraGridRow.Cells.FromKey("Cur-IZA").Text = string.Format("{0:### ##0.00}%", ultraGridRow.Cells.FromKey("Cur-IZA").Value);
            }
            if (ultraGridRow.Cells.FromKey("Cur-IZA").Value != null)
            {
                ultraGridRow.Cells.FromKey("Cur-IZA").Style.BorderDetails.ColorTop = Color.Transparent;
            }

        }


        private void ConfRow()
        {
            Color newxtColor = Color.Transparent;

            UltraGridRow LastRow = null;

            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells.FromKey("Field").Value != null)
                {
                    FormatGridValueRow(Row);
                    FormatGridDeviationRow(Row.NextRow);
                    FormatGridSpeedDeviationRow(Row.NextRow.NextRow);
                }

                Row.Style.BackColor = Color.Transparent;
            }
        }

        private void ConfigurateGrid()
        {
            SetHeader();
            ConfRow();
            CustomizerSize.ContfigurationGrid(Grid);
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.NullTextDefault = "-";

            Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
        }


        #endregion

        private void ConfIZOCHART(Infragistics.WebUI.UltraWebChart.UltraChart ChartIZO)
        {
            ChartIZO.ChartType = ChartType.ColumnChart;

            ChartIZO.Legend.Visible = true;
            ChartIZO.Legend.Location = LegendLocation.Bottom;
            ChartIZO.Legend.SpanPercentage = 11 ; 

            ChartIZO.Legend.Margins.Right = CustomizerSize.GetChartWidth() - 400;

            ChartIZO.Axis.X.Labels.Visible = false;
            ChartIZO.Axis.X.Extent = 20;
            ChartIZO.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
            ChartIZO.Axis.X.Labels.SeriesLabels.FontColor = Color.Black;
            //ChartIZO.Axis.X.MajorGridLines.Visible = false;

            ChartIZO.Tooltips.FormatString = @"Индекс загрязненности атмосферы в <ITEM_LABEL> в пгт. <SERIES_LABEL><b> <DATA_VALUE:0></b> %";

            ChartIZO.Axis.Y.Extent = 30;
            //ChartIZO.Axis.Y.MajorGridLines.Visible = false;

            ChartIZO.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ##0.##>";
        }

        private void DataBindIZOCHART()
        {
            if (BaseTable == null)
                return;

            DataTable TableChart = new DataTable();

            TableChart.Columns.Add("Field");

            if (!ColIsEmpty("Prev-IZA"))
            {
                TableChart.Columns.Add(GetPrevPeriod().DisplayText(), typeof(System.Decimal));
            }

            TableChart.Columns.Add(PeriodPars.DisplayText(), typeof(System.Decimal));

            foreach (DataRow row in BaseTable.Rows)
            {
                DataRow newRow = TableChart.NewRow();
                TableChart.Rows.Add(newRow);
                newRow["Field"] = row["Field"];
                newRow[PeriodPars.DisplayText()] = row["Cur-IZA"];
                if (TableChart.Columns.Contains(GetPrevPeriod().DisplayText()))
                {
                    newRow[GetPrevPeriod().DisplayText()] = row["Prev-IZA"];
                }
            }

            int gorCount = 0, moCount = 0;
            for (int i = 0; i < TableChart.Rows.Count; i++)
            {
                if (TableChart.Rows[i][0].ToString().StartsWith("Город"))
                {
                    gorCount += 1;
                }
                else
                {
                    moCount += 1;
                }
            }
            string[] goroda = new string[gorCount];
            string[] mo = new string[moCount];
            int k = 0, l = 0;
            for (int i = 0; i < TableChart.Rows.Count; i++)
            {
                if (TableChart.Rows[i][0].ToString().StartsWith("Город"))
                {
                    goroda[k] = TableChart.Rows[i][0].ToString().Replace("Город", String.Empty).Replace(" ", String.Empty);
                    k += 1;
                }
                else
                {
                    mo[l] = TableChart.Rows[i][0].ToString();
                    l += 1;
                }
            }
            Array.Sort(goroda);
            Array.Sort(mo);
            DataTable normDt = new DataTable();
            for (int i = 0; i < TableChart.Columns.Count; i++)
            {
                normDt.Columns.Add(TableChart.Columns[i].ColumnName, TableChart.Columns[i].DataType);
            }
            for (int j = 0; j < TableChart.Rows.Count; j++)
            {
                if (TableChart.Rows[j][0].ToString() == "Город Ханты-Мансийск")
                {
                    normDt.Rows.Add(TableChart.Rows[j].ItemArray);
                }
            }

            for (int i = 0; i < goroda.Length; i++)
            {
                for (int j = 0; j < TableChart.Rows.Count; j++)
                {
                    if (TableChart.Rows[j][0].ToString() == "Город " + goroda[i] && TableChart.Rows[j][0].ToString() != "Город Ханты-Мансийск")
                    {
                        normDt.Rows.Add(TableChart.Rows[j].ItemArray);
                    }
                }
            }

            for (int i = 0; i < mo.Length; i++)
            {
                for (int j = 0; j < TableChart.Rows.Count; j++)
                {
                    if (TableChart.Rows[j][0].ToString() == mo[i])
                    {
                        normDt.Rows.Add(TableChart.Rows[j].ItemArray);
                    }
                }
            }

                ChartIZO.DataSource = normDt;
            ChartIZO.DataBind();
        }

        private void GenChartIZO()
        {
            ConfIZOCHART(ChartIZO);
            DataBindIZOCHART();

            ChartIZO.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ChartIZO_FillSceneGraph);
        }

        bool ColIsEmpty(string col)
        {
            foreach (DataRow row in BaseTable.Rows)
            {
                if (row[col] != DBNull.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private void DataBindSZCHART(Infragistics.WebUI.UltraWebChart.UltraChart ChartSZ, string MainCol)
        {
            if (BaseTable == null)
                return;

            if (ColIsEmpty(MainCol))
                return;
            DataTable TableChart = new DataTable();

            TableChart.Columns.Add("Field");

            TableChart.Columns.Add(SZ_CAPTION[1], typeof(decimal));
            TableChart.Columns.Add(SZ_CAPTION[2], typeof(decimal));
            TableChart.Columns.Add(SZ_CAPTION[3], typeof(decimal));
            TableChart.Columns.Add(SZ_CAPTION[4], typeof(decimal));

            DataRow Cur_row = TableChart.NewRow();
            TableChart.Rows.Add(Cur_row);
            Cur_row[0] = PeriodPars.DisplayText();

            for (int i = 1; i < 5; i++)
            {
                Cur_row[SZ_CAPTION[i]] = 0;
                foreach (DataRow baseRow in BaseTable.Rows)
                {
                    if (baseRow[MainCol] != DBNull.Value)
                    {
                        if (int.Parse(baseRow[MainCol].ToString()) == i)
                        {
                            Cur_row[SZ_CAPTION[i]] = int.Parse(Cur_row[SZ_CAPTION[i]].ToString()) + 1;
                        }
                    }
                }
            }
            TableChart.Columns[1].ColumnName = SZ_CAPTION_CHART[1];
            TableChart.Columns[2].ColumnName = SZ_CAPTION_CHART[2];
            TableChart.Columns[3].ColumnName = SZ_CAPTION_CHART[3];
            TableChart.Columns[4].ColumnName = SZ_CAPTION_CHART[4];
            
            

            ChartSZ.DataSource = TableChart;
            ChartSZ.DataBind();

        }

        private void ConfSZCHART(Infragistics.WebUI.UltraWebChart.UltraChart ChartSZ)
        {
            ChartSZ.ChartType = ChartType.PieChart; 
            ChartSZ.Legend.Visible = false;

            ChartSZ.ColorModel.ModelStyle = ColorModels.CustomSkin;
            ChartSZ.ColorModel.Skin.ApplyRowWise = true;
            ChartSZ.ColorModel.Skin.PEs.Clear();

            for (int i = 0; i < 4; ++i)
            {
                PaintElement pe = new PaintElement();

                switch (i)
                {
                    case 0:
                        {
                            pe.Fill = Color.Green;
                            break;
                        }
                    case 1:
                        {
                            pe.Fill = Color.Yellow;
                            break;
                        }
                    case 2:
                        {
                            pe.Fill = Color.Orange;
                            break;
                        }
                    case 3:
                        {
                            pe.Fill = Color.Red;
                            break;
                        }

                }

                pe.FillStopColor = pe.Fill;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 200;
                pe.FillStopOpacity = 100;

                ChartSZ.ColorModel.Skin.PEs.Add(pe);

                LineAppearance lineAppearance = new LineAppearance();

                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
                lineAppearance.IconAppearance.PE = pe;

                ChartSZ.LineChart.LineAppearances.Add(lineAppearance);
            }
        }

        private void GenChartSZ(Infragistics.WebUI.UltraWebChart.UltraChart ChartSZ,string col)
        {
            ChartSZ.Data.SwapRowsAndColumns = true;

            ConfSZCHART(ChartSZ);
            if (col == "CUR-SZ")
            {
             //   ChartSZ.Tooltips.FormatString = "<ITEM_LABEL> степень загрязнения в " + PeriodPars.DisplayText() + " наблюдается у <b><DATA_VALUE:0></b> МО";
            }
            else
            {
              //  ChartSZ.Tooltips.FormatString = "<ITEM_LABEL> степень загрязнения в " + GetPrevPeriod().DisplayText() + " наблюдается у <b><DATA_VALUE:0></b> МО";
            }


            ChartSZ.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(ChartSZ_InvalidDataReceived);
            //ChartSZ.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ChartSZ_FillSceneGraph);

            DataBindSZCHART(ChartSZ, col);            
        }

        void ChartSZ_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }
        private void ConfLegendChart(Infragistics.WebUI.UltraWebChart.UltraChart ChartSZLegend)
        {
            ChartSZLegend.Legend.SpanPercentage = 100;
            ChartSZLegend.Legend.Visible = true;
            ChartSZLegend.Legend.Location = LegendLocation.Top;
            ChartSZLegend.Legend.Font = new Font("Verdana", 10);

            ChartIZO.Legend.Font = new Font("Verdana", 10);

            ChartSZLegend.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ChartSZLegend_FillSceneGraph);
            
        }

        
        

        private void SetHeaderReport()
        {

            Label3.Text = string.Format("Индекс загрязненности атмосферы (ИЗА) в {0}, %", PeriodPars.DisplayText());
            Label5.Text = string.Format("Степень загрязнения атмосферы в {0}", PeriodPars.DisplayText());

            PageSubTitle.Text = string.Format("Данные ежегодного и ежемесячного мониторинга качества атмосферного воздуха в муниципальных образованиях ХМАО-Югры, на {0}", PeriodPars.DisplayLegendChartText());

            if (!ColIsEmpty("PREV-SZ"))
                Label6.Text = string.Format("Степень загрязнения атмосферы в {0}", GetPrevPeriod().DisplayText());
            else
                Label6.Text = string.Empty;
            Page.Title = Hederglobal.Text;
        }

        private string ChartColumn = "";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            FillCombo();

            ChosenParam();
            DataBindGrid();
            ConfigurateGrid();

            GenChartIZO();
            ChartColumn = "CUR-SZ";
            GenChartSZ(ChartSZCur,"CUR-SZ");
            ChartColumn = "PREV-SZ";
            GenChartSZ(ChartSZPrev, "PREV-SZ");
            GenChartSZ(ChartSZLegend, "CUR-SZ");
            ConfLegendChart(ChartSZLegend);

            SetHeaderReport();
        }

        

        void ChartIZO_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            bool firstLegendText = false;

            foreach (Primitive p in e.SceneGraph)
            {
                if (p.Path != null)
                {
                    if (p.Path == "Border.Title.Legend")
                    {
                        if (p is Text)
                        {
                            Text t = (Text)p;
                            if (ColIsEmpty("Prev-IZA"))
                            {
                                t.SetTextString(PeriodPars.DisplayLegendChartText());
                            }
                            else
                            {

                                if (firstLegendText)
                                {
                                    t.SetTextString(PeriodPars.DisplayLegendChartText());
                                }
                                else
                                {
                                    t.SetTextString(GetPrevPeriod().DisplayLegendChartText());
                                }
                                firstLegendText = !firstLegendText;
                            }
                        }
                    }
                }
            }
        }

        void ChartSZLegend_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            List<Primitive> lp = new System.Collections.Generic.List<Primitive>();

            foreach (Primitive p in e.SceneGraph)
            {
                if (p.Path != null)
                {
                    if (p.Path.Contains("Legend"))
                    {
                        continue;
                    }
                }
                lp.Add(p);
            }

            foreach (Primitive p in lp)
            {
                e.SceneGraph.Remove(p);
            }
        }


       /* void ChartSZ_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i=0; i<e.SceneGraph.Count;i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Wedge)
                {
                    Wedge polygon = (Wedge)primitive;
                    if (polygon.DataPoint != null)
                    {
                        
                        polygon.DataPoint.Label = "dfdf";//+ " степень загрязнения в "+UserComboBox.getLastBlock(ChosenPrevPeriod.Value) +" "+ChosenPrevPeriod.Value.Split(']')[3].Remove(0,2) +" года наблюдается у " + polygon.Value+ " МО";
                        
                        
                    }
                }
            }
            
            //Text t = new Text(new Point(12, 100), "Количество МО");
            //t.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            //t.labelStyle.Font = new Font("Verdana", 11);
            //e.SceneGraph.Add(t);
        }
        */
        #region Осторожно! бЫдлА-кот =^_^=
        //todo
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 12 * 20;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].CellFormat.Font.Name = "Verdana";
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            ExportGridToExcel(Grid, e.CurrentWorksheet.Workbook.Worksheets["Таблица"], 3, true);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");


            Worksheet sheet2 = workbook.Worksheets.Add("ДиАграмма_1");
            Worksheet sheet3 = workbook.Worksheets.Add("ДиАграмма_2");

            foreach (UltraGridColumn col in Grid.Columns)
            {
                if ((col.Hidden) || (col.Width.Value == 0))
                {
                    Grid.Bands[0].HeaderLayout.Remove(col.Header);
                    Grid.Columns.Remove(col);
                }
            }

            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 6;
            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";
            ReportExcelExporter1.ExcelExporter.Export(Grid, sheet1);

            //ReportExcelExporter1.Export(ChartIZO, Label3.Text, sheet2, 2);
            //ReportExcelExporter1.Export(ChartSZ, Label4.Text, sheet3, 3);
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

                    try
                    {
                        if (sheet.Rows[i + startrow].Cells[col.Index].Value.ToString().Contains("%"))
                        {
                            sheet.Rows[i + startrow].Cells[col.Index].Value = decimal.Parse(sheet.Rows[i + startrow].Cells[col.Index].Value.ToString().Replace("%", ""))/100;//sheet.Rows[i + startrow].Cells[col.Index].Value.ToString().Replace("%", "").Replace(".","");
                            sheet.Rows[i + startrow].Cells[col.Index].CellFormat.FormatString = "0.00%";
                        }
                    }
                    catch { }
                }
                if (G.Rows[i].Cells[0].RowSpan > 1)
                {
                    sheet.MergedCellsRegions.Add(startrow + i, 0, startrow + i + G.Rows[i].Cells[0].RowSpan - 1, 0);
                }
                
            }
            sheet.Columns[0].Width = 270 * 36;
            sheet.Rows[3].Height = 40 * 40;
            for (int i = 1; i < 20; i++)
            {
                sheet.Columns[i].Width = 90 * 50;
            }

            try
            {
                ChartSZCur.Width = 433;
                ChartSZCur.Height = 430;
                ChartSZPrev.Width = 433;
                ChartSZPrev.Height = 430;
                ChartSZLegend.Width = 960;

                sheet.Workbook.Worksheets[2].Rows[0].Cells[1].Value = Label5.Text;
                ReportExcelExporter.ChartExcelExport(sheet.Workbook.Worksheets[2].Rows[2].Cells[1], ChartSZCur);

                sheet.Workbook.Worksheets[2].Rows[0].Cells[10].Value = Label6.Text;
                ReportExcelExporter.ChartExcelExport(sheet.Workbook.Worksheets[2].Rows[2].Cells[10], ChartSZPrev);
                
                ReportExcelExporter.ChartExcelExport(sheet.Workbook.Worksheets[2].Rows[35].Cells[1], ChartSZLegend);

                sheet.Workbook.Worksheets[1].Rows[0].Cells[0].Value = Label3.Text;
                ReportExcelExporter.ChartExcelExport(sheet.Workbook.Worksheets[1].Rows[2].Cells[1], ChartIZO);
            }
            catch { }

        }

        #endregion
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            Report r = new Report();

            ISection e_ = r.AddSection();
            e_.PageSize = new PageSize(800, 600);
            IText title = e_.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;
            title.AddContent(Hederglobal.Text);

            title = e_.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;
            title.AddContent(PageSubTitle.Text);


            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            GridHeaderCell header;
            headerLayout.AddCell("МРГО");
            header = headerLayout.AddCell("ИЗА");
            header.AddCell("Отчётный период");
            header.AddCell("Предыдущий период");
            

            header = headerLayout.AddCell("Степень загрязнения");
            header.AddCell("Отчётный период");
            header.AddCell("Предыдущий период");

            header = headerLayout.AddCell("Вещества внисяцие основной вклад в загрязнения");

            headerLayout.ApplyHeaderInfo();


            ReportPDFExporter1.HeaderCellHeight = 20;
            ReportPDFExporter1.Export(headerLayout, e_);
            
            ISection is_ = r.AddSection();
            is_.PageSize = new PageSize(950, 500);
            title = is_.AddText();
            title.Margins = new Infragistics.Documents.Reports.Report.Margins(40, 40, 40, 40);
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;

            ChartIZO.Height = 400;

            GenChartIZO();
            title.AddContent(Label3.Text);
            MemoryStream imageStream = new MemoryStream();
            ChartIZO.SaveTo(imageStream, Infragistics.UltraChart.Shared.Styles.RenderingType.Image);
            Infragistics.Documents.Reports.Graphics.Image img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);

            IImage ima = is_.AddImage(img);
            ima.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 0, 5, 0);

            title = is_.AddText();
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

            title.Alignment.Horizontal = Alignment.Center;

            is_ = r.AddSection();
            is_.PageSize = new PageSize(950, 500);
            title = is_.AddText();
            title.Margins = new Infragistics.Documents.Reports.Report.Margins(40, 40, 40, 40);
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;

            ITable TableChart = is_.AddTable();
            ITableRow RowLabel = TableChart.AddRow();

            IText _Label = RowLabel.AddCell().AddText();
            _Label.Alignment.Horizontal = Alignment.Center;
            
            _Label.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            _Label.Style.Font.Bold = true;
            _Label.AddContent(Label5.Text);


            IText __Label = RowLabel.AddCell().AddText();
            __Label.Alignment.Horizontal = Alignment.Center;

            __Label.Style.Font = _Label.Style.Font;//new Infragistics.Documents.Reports.Graphics.Font(font);
            __Label.Style.Font.Bold = true;
            __Label.AddContent(Label6.Text);


            ITableRow RowImage = TableChart.AddRow();
            ITableRow RowLegend = TableChart.AddRow();

            RowImage.AddCell().AddImage(UltraGridExporter.GetImageFromChart(ChartSZCur));
            RowImage.AddCell().AddImage(UltraGridExporter.GetImageFromChart(ChartSZPrev));

            RowLegend.AddCell().AddImage(UltraGridExporter.GetImageFromChart(ChartSZLegend));
            

            //GenChartSZ();
            //title.AddContent(Label4.Text);
            //imageStream = new MemoryStream();
            //ChartSZ.SaveTo(imageStream, Infragistics.UltraChart.Shared.Styles.RenderingType.Image);
            //img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);

            //ima = is_.AddImage(img);
            //ima.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 0, 5, 0);

            title = is_.AddText();
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

            title.Alignment.Horizontal = Alignment.Center;
        }


        #endregion

        protected void ChartSZCur_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Wedge)
                {
                    Wedge polygon = (Wedge)primitive;
                    if (polygon.DataPoint != null)
                    {
                        string buf = "";
                        if (polygon.DataPoint.Label == "Низкая")
                        {
                            for (int j = 0; j < BaseTable.Rows.Count; j++)
                            {
                                if (BaseTable.Rows[j]["Cur-SZ"].ToString() == "1")
                                {
                                    buf += BaseTable.Rows[j][0].ToString() + ", ";
                                }
                            }
                        }
                        if (polygon.DataPoint.Label == "Повышенная")
                        {
                            for (int j = 0; j < BaseTable.Rows.Count; j++)
                            {
                                if (BaseTable.Rows[j]["Cur-SZ"].ToString() == "2")
                                {
                                    buf += BaseTable.Rows[j][0].ToString() + ", ";
                                }
                            }
                        }
                        if (polygon.DataPoint.Label == "Высокая")
                        {
                            for (int j = 0; j < BaseTable.Rows.Count; j++)
                            {
                                if (BaseTable.Rows[j]["Cur-SZ"].ToString() == "3")
                                {
                                    buf += BaseTable.Rows[j][0].ToString() + ", ";
                                }
                            }
                        }
                        if (polygon.DataPoint.Label == "Очень высокая")
                        {
                            for (int j = 0; j < BaseTable.Rows.Count; j++)
                            {
                                if (BaseTable.Rows[j]["Cur-SZ"].ToString() == "4")
                                {
                                    buf += BaseTable.Rows[j][0].ToString() + ", ";
                                }
                            }
                        }
                        buf = buf.TrimEnd(' ').TrimEnd(',');
                        polygon.DataPoint.Label += " степень загрязнения в " + UserComboBox.getLastBlock(ChosenPeriod.Value).ToLower() + " " + ChosenPeriod.Value.Split(']')[3].Remove(0, 2) + " года наблюдается у " + polygon.Value + " МО:<br>"+buf;
                    }
                }
            }
        }

        #endregion

        protected void ChartSZPrev_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Wedge)
                {
                    Wedge polygon = (Wedge)primitive;
                    if (polygon.DataPoint != null)
                    {
                        string buf = "";
                        if (polygon.DataPoint.Label == "Низкая")
                        {
                            for (int j = 0; j < BaseTable.Rows.Count; j++)
                            {
                                if (BaseTable.Rows[j]["Prev-SZ"].ToString() == "1")
                                {
                                    buf += BaseTable.Rows[j][0].ToString() + ", ";
                                }
                            }
                        }
                        if (polygon.DataPoint.Label == "Повышенная")
                        {
                            for (int j = 0; j < BaseTable.Rows.Count; j++)
                            {
                                if (BaseTable.Rows[j]["Prev-SZ"].ToString() == "2")
                                {
                                    buf += BaseTable.Rows[j][0].ToString() + ", ";
                                }
                            }
                        }
                        if (polygon.DataPoint.Label == "Высокая")
                        {
                            for (int j = 0; j < BaseTable.Rows.Count; j++)
                            {
                                if (BaseTable.Rows[j]["Prev-SZ"].ToString() == "3")
                                {
                                    buf += BaseTable.Rows[j][0].ToString() + ", ";
                                }
                            }
                        }
                        if (polygon.DataPoint.Label == "Очень высокая")
                        {
                            for (int j = 0; j < BaseTable.Rows.Count; j++)
                            {
                                if (BaseTable.Rows[j]["Prev-SZ"].ToString() == "4")
                                {
                                    buf += BaseTable.Rows[j][0].ToString() + ", ";
                                }
                            }
                        }
                        buf = buf.TrimEnd(' ').TrimEnd(',');
                        polygon.DataPoint.Label += " степень загрязнения в " + UserComboBox.getLastBlock(ChosenPrevPeriod.Value) + " " + ChosenPrevPeriod.Value.Split(']')[3].Remove(0, 2) + " года наблюдается у " + polygon.Value+ " МО:<br>"+buf;
                    }
                }
            }
        }

        #region -_-
        #region o_-
        #region O_-
        #region O_o
        #region o_O
        #region -_O
        #region -_o
        #region -_-
        #region o_o
        #region p_p
        #region q_q
        #region Q_Q
        #region O_O
        #region *_*
        #region &_&
        #region 6_6
        #region !_!
        #region ~_~
        #region >_<
        #region э_e
        #region b_d
        #region ё_ё
        #region $_$
        #region o,O
        #region o/O
        #region -/O
        #region v_v
        #region W_W
        #region ^.^
        #region @_@
        #region /_\
        #region "_"
        #region М_М
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion
        #endregion








    }
}