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


namespace Krista.FM.Server.Dashboards.Sahalin_ARC.investment
{
    public partial class _default : CustomReportPage
    {

        bool Beautiful_Lie = false;

        IDataSetCombo SetQuart;

        ICustomizerSize CustomizerSize;

        CustomParam ChosenPeriod { get { return (UserParams.CustomParam("ChosenPeriod")); } }
        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }

        CustomParam Group { get { return (UserParams.CustomParam("Group_")); } }
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
                int onePercent = (int)1280 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 27;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 27;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)1280 / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 19) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 27;
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

                Grid.Columns[0].Width = onePercent * 27;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 21) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 27;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = onePercent * (100 - 19) / (Grid.Columns.Count - 1);
                }

                Grid.Columns[0].Width = onePercent * 27;
            }

            #endregion

        }
        #endregion

        static class DataBaseHelper
        {
            static DataProvider ActiveDataPorvider = DataProvidersFactory.PrimaryMASDataProvider;

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

                    string UniqueName = row["UniqueName"].ToString();
                    string DisplayNAme = this.GetAlowableAndFormatedKey(row["DisplayName"].ToString(), "");

                    if (LastParent != row["ParentName"].ToString())
                    {
                        LastParent = row["ParentName"].ToString();
                        this.AddItem(LastParent, 0, "");
                    }

                    this.AddItem(DisplayNAme, 1, UniqueName);

                    this.addOtherInfo(Table, row, DisplayNAme);
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
            UltraGridExporter1.ExcelExportButton.Visible = true;

            //ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            //ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);



            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_Test);
            UltraGridExporter1.PdfExporter.RowExported += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportedEventArgs>(PdfExporter_RowExported);
            

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
            Grid.Height = 600;
        }

        private void FillComboTypePeriod()
        {


            ComboTypePeriod.Width = 450 * koef;
            Dictionary<string, int> Tp = new System.Collections.Generic.Dictionary<string, int>();
            Tp.Add("к предыдущему периоду", 0);
            Tp.Add("к началу года", 0);
            Tp.Add("к аналогичному периоду прошлого года", 0);

            ComboTypePeriod.FillDictionaryValues(Tp);
        }

        private void FillComboYear()
        {
            ComboQuart.Width = 250 * koef;
            ComboQuart.ParentSelect = true;

            DataTable Table = DataBaseHelper.ExecQueryByID("ComboYear", "DisplayName");

            SetQuart = new DataSetComboYearQuart();
            SetQuart.LoadData(Table);
            if (!Page.IsPostBack)
            {
                ComboQuart.FillDictionaryValues(SetQuart.DataForCombo);
                ComboQuart.SetСheckedState(SetQuart.LastAdededKey, 1 == 1);
            }
        }

        private void FillComboOKVED()
        {

            ComboOKVED_or_REGION.Width = 300 * koef;
            Dictionary<string, int> Tp = new System.Collections.Generic.Dictionary<string, int>();
            Tp.Add("ОКВЭД", 0);
            Tp.Add("Страны мира", 0);

            ComboOKVED_or_REGION.FillDictionaryValues(Tp);
        }
        double koef = 1;
        private void FillCombo()
        {
            if (CustomizerSize is CustomizerSize_800x600)
            {
                koef = 0.6;
            }

            FillComboYear();
            if (!Page.IsPostBack)
            {
                FillComboTypePeriod();
                FillComboOKVED();
            }

            ComboOKVED_or_REGION.Title = "Разрезность";
            ComboQuart.Title = "Период";
            ComboTypePeriod.Title = "Период для сравнения";
            

        }

        class PeriodPars
        {
            int Year;
            int HalgYear;
            int Quart;


            public static string bindDate(int y, int h, int q)
            {
                return string.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}]", y, h, q);
            }

            public string AnalogPrevPeriod()
            {

                int NewYear = Year - 1;
                int NewHalgYear = HalgYear;
                int NewQuart = Quart;

                return bindDate(NewYear, NewHalgYear, NewQuart);
            }

            public string FirstYearDate()
            {

                int NewYear = Year;
                int NewHalgYear = 1;
                int NewQuart = 1;

                return bindDate(NewYear, NewHalgYear, NewQuart);
            }

            public string PrevPeriodPars()
            {

                int NewYear = Year;
                int NewHalgYear = HalgYear;
                int NewQuart = Quart;

                if ((Quart == 2) || (Quart == 4))
                {
                    NewQuart--;
                }
                else
                {
                    NewQuart--;

                    if (NewQuart == 0)
                    {
                        NewQuart = 4;
                    }

                    if (NewHalgYear == 1)
                    {
                        NewHalgYear = 2;

                        NewYear--;
                    }
                    NewHalgYear--;

                }

                return bindDate(NewYear, NewHalgYear, NewQuart);
            }

            public string DisplayText()
            {
                return Quart.ToString() + " квартал " + Year.ToString() + " года";
            }

            public PeriodPars(string uniqueDate)
            {
                string[] b = { "].[" };

                string[] parsedate = uniqueDate.Split(b, StringSplitOptions.None);

                int C = parsedate.Length;
                //CRHelper.SaveToErrorLog(uniqueDate);
                Quart = (int.Parse(parsedate[C - 1].Split(' ')[1].Split(']')[0]));
                HalgYear = int.Parse(parsedate[C - 2].Split(' ')[1]);
                Year = int.Parse(parsedate[C - 3]);

            }
        }

        private string ChosePrevPeriod()
        {
            if (ComboTypePeriod.SelectedValue == "к предыдущему периоду")
            {
                return new PeriodPars(ChosenPeriod.Value).PrevPeriodPars();
            }
            else
                if (ComboTypePeriod.SelectedValue == "к началу года")
                {
                    return new PeriodPars(ChosenPeriod.Value).FirstYearDate();
                }
                else
                {
                    return new PeriodPars(ChosenPeriod.Value).AnalogPrevPeriod();
                }
        }

        private void ChosenParam()
        {
            if (ComboQuart.SelectedNode.Nodes.Count > 0)
            {
                ComboQuart.SetСheckedState(ComboQuart.SelectedNode.Nodes[0].Text, true);
            }

            ChosenPeriod.Value = SetQuart.DataUniqeName[ComboQuart.SelectedValue];

            ChosenPrevPeriod.Value = ChosePrevPeriod();

            if (ComboOKVED_or_REGION.SelectedValue == "ОКВЭД")
            {
                Fields.Value = @"Filter
            ([ОК__ОКВЭД].[ОК__ОКВЭД].members,
            not isempty((" + ChosenPeriod.Value + @",[Measures].[Нарастающий итог]))
            and (not [ОК__ОКВЭД].[ОК__ОКВЭД].currentmember.is_datamember))";
                //
                Group.Value = "По видам экономической деятельности, по видам инвестиций";
            }
            else
            {
                Group.Value = "По видам инвестиций, по странам";
                Fields.Value = @"filter(
                [ОК__Страны мира].[ОК__Страны мира].[Страна].members,
                not [ОК__Страны мира].[ОК__Страны мира].currentmember.is_datamember
                and
                [Инвестиции__Виды инвестиций].[Инвестиции__Виды инвестиций].[AllSum]>0)";
            }
        }

        string ExtractRootLevelName(string UnName)
        {
            string[] b = { "].[" };
            String.Format(UnName);
            return UnName.Split(b, StringSplitOptions.None)[3];
        }

        string ExtractLevelName(string LevelUNiqueName)
        {
            string[] b = new string[3] { "].[", "[", "]" };
            string LevelName = LevelUNiqueName.Split(b, StringSplitOptions.None)[3];
            LevelName = LevelName == "(All)" ? "" : LevelName + " - ";
            return LevelName;
        }

        string ExtractLevelName2(string LevelUNiqueName)
        {
            string[] b = new string[3] { "].[", "[", "]" };
            string LevelName = LevelUNiqueName.Split(b, StringSplitOptions.None)[3];

            LevelName = LevelName == "(All)" ? "" : LevelName + "";

            return LevelName;
        }

        DataRow FindRow(DataTable table, string colname, string value)
        {
            foreach (DataRow Row in table.Rows)
            {
                if (Row[colname].ToString() == value)
                {
                    return Row;
                }
            }
            return null;
        }

        DataTable UnionTable(DataTable Table1, DataTable Table2)
        {
            if (Table1 == null)
            {
                return Table2;
            }
            if (Table2 == null)
            {
                return Table1;
            }

            foreach (DataRow row in Table1.Rows)
            {
                DataRow rowT2 = FindRow(Table2, "cur-Field", row["cur-Field"].ToString());
                foreach (DataColumn col in Table1.Columns)
                {
                    
                    if ((col.ColumnName.Contains("cur")||(col.ColumnName.Contains("prev"))))
                    {
                        rowT2[col.ColumnName] = row[col.ColumnName];
                    }
                }
            }
            
            return Table2;
        }


        private void DataBindGrid()
        {
            Grid.Bands[0].Columns.Clear();
            Grid.Bands[0].HeaderLayout.Clear();


            DataTable BaseTable =UnionTable(
                ((ComboOKVED_or_REGION.SelectedValue == "ОКВЭД")?
                DataBaseHelper.ExecQueryByID("Grid_firstrow", "cur-Field"):null),
                DataBaseHelper.ExecQueryByID("Grid", "cur-Field"));
            if ((ComboOKVED_or_REGION.SelectedValue == "ОКВЭД"))
            {
                //BaseTable.Rows[1].Delete();
            }
            DataTable TableGrid = new DataTable();

            TableGrid.Columns.Add("Field");

            TableGrid.Columns.Add("in-subject-invest-sum", typeof(decimal));
            TableGrid.Columns.Add("in-subject-invest-line", typeof(decimal));
            TableGrid.Columns.Add("in-subject-invest-portfel", typeof(decimal));
            TableGrid.Columns.Add("in-subject-invest-other", typeof(decimal));

            TableGrid.Columns.Add("out-subject-invest-sum", typeof(decimal));
            TableGrid.Columns.Add("out-subject-invest-line", typeof(decimal));
            TableGrid.Columns.Add("out-subject-invest-portfel", typeof(decimal));
            TableGrid.Columns.Add("out-subject-invest-other", typeof(decimal));

            string PrevParent = "";


            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                {

                    DataRow GridRowValue = TableGrid.NewRow();
                    DataRow GridRowTemp = TableGrid.NewRow();
                    DataRow GridRowDeviation = TableGrid.NewRow();


                    TableGrid.Rows.Add(GridRowValue);                    
                    TableGrid.Rows.Add(GridRowTemp);
                    TableGrid.Rows.Add(GridRowDeviation);

                    foreach (DataColumn TableGridCol in TableGrid.Columns)
                    {
                        GridRowValue[TableGridCol] = BaseRow["cur-" + TableGridCol.ColumnName];

                        if (BaseTable.Columns.Contains("cur-" + TableGridCol.ColumnName)
                            &&
                            BaseTable.Columns.Contains("prev-" + TableGridCol.ColumnName)
                            &&
                            (BaseRow["cur-" + TableGridCol.ColumnName] != DBNull.Value)
                            &&
                            (BaseRow["prev-" + TableGridCol.ColumnName] != DBNull.Value)
                            &&
                            (decimal)BaseRow["prev-" + TableGridCol.ColumnName] != 0)
                        {
                            {
                                GridRowDeviation[TableGridCol] = (decimal)BaseRow["cur-" + TableGridCol.ColumnName] - (decimal)BaseRow["prev-" + TableGridCol.ColumnName];
                                GridRowTemp[TableGridCol] = 100 * (decimal)BaseRow["cur-" + TableGridCol.ColumnName] / (decimal)BaseRow["prev-" + TableGridCol.ColumnName];
                            }
                        }
                    }
                    int LevelNumber = BaseRow["Levelnumber"].ToString()==""?0:int.Parse(string.Format("{0:N0}", BaseRow["Levelnumber"]));
                    if ((BaseRow["Razdel"] != DBNull.Value)
                       &&
                       LevelNumber == 2)
                    {
                        //if ((BaseRow["Razdel"].ToString().Contains("C")||(BaseRow["Razdel"].ToString().Contains("F"))))
                        {
                            GridRowValue["Field"] = string.Format("{0:N0}|", BaseRow["Levelnumber"]) +
                                                       ExtractLevelName2(BaseRow["LevelName"].ToString()) + " " +
                                                       BaseRow["Razdel"].ToString() +
                                                       " - " +
                                                       BaseRow["cur-Field"].ToString().Replace("Подраздел - ", "");
                        }
                        //else
                        //{
                        //    TableGrid.Rows.Remove(GridRowValue);
                        //    TableGrid.Rows.Remove(GridRowTemp);
                        //    TableGrid.Rows.Remove(GridRowDeviation);
                        //}
                    }
                    else
                    if ((BaseRow["Razdel"] != DBNull.Value)
                        //&&
                        //((BaseRow["Razdel"].ToString().Contains("C")) || (BaseRow["Razdel"].ToString().Contains("D")))
                        &&
                        LevelNumber < 3)
                    {
                        GridRowValue["Field"] = string.Format("{0:N0}|", BaseRow["Levelnumber"]) +
                            ExtractLevelName2(BaseRow["LevelName"].ToString()) +
                            " " +
                            BaseRow["Razdel"].ToString() +
                            " - " +
                            BaseRow["cur-Field"].ToString().Replace("Подраздел - ", "");
                    }
                    else
                        
                        if ((BaseRow["Razdel"] != DBNull.Value)
                        &&
                        LevelNumber < 2)
                        {
                            GridRowValue["Field"] = string.Format("{0:N0}|", BaseRow["Levelnumber"]) +
                                 ExtractLevelName2(BaseRow["LevelName"].ToString()) +
                                " " +
                                BaseRow["Razdel"].ToString() +
                                " - " +
                                BaseRow["cur-Field"].ToString().Replace("Подраздел - ", "");
                        }
                        else
                        {

                            if (BaseRow["Razdel"] != DBNull.Value)
                            {
                                GridRowValue["Field"] = string.Format("{0:N0}|", BaseRow["Levelnumber"])
                                    //+BaseRow["Razdel"].ToString() +
                                    +
                                ExtractLevelName2(BaseRow["LevelName"].ToString()) + " - "

                                    + BaseRow["cur-Field"].ToString().Replace("Подраздел - ", "");
                                continue;
                            }

                            if (ComboOKVED_or_REGION.SelectedValue == "ОКВЭД")
                                GridRowValue["Field"] = string.Format("{0:N0}|", BaseRow["Levelnumber"])
                                    +
                                ExtractLevelName2(BaseRow["LevelName"].ToString())
                                    + 
                                    BaseRow["cur-Field"].ToString().Replace("Подраздел - ", "");
                            else
                                GridRowValue["Field"] = string.Format("{0:N0}|", BaseRow["Levelnumber"])
                                    + GridRowValue["Field"].ToString().Replace("Подраздел - ", "");
                        }
                }
            }

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

        private void SetHeader(bool Export)
        {
            ColumnsCollection cols = Grid.Columns;
            int add = 1;
            int add2 = Export ? 1 : 0;

            if (Export)
            {
                Grid.Columns.Add("CaptionExportCol");
                Grid.Columns.FromKey("CaptionExportCol").Move(1);

                for (int i = 0; i < Grid.Rows.Count; i += 3)
                {
                    UltraGridRow RowVal = Grid.Rows[i];
                    UltraGridRow RowDeviation = Grid.Rows[i + 1];
                    UltraGridRow RowSpeedDeviation = Grid.Rows[i + 2];

                    RowVal.Cells.FromKey("CaptionExportCol").Text = "Значение";
                    RowDeviation.Cells.FromKey("CaptionExportCol").Text = "Темп роста";
                    RowSpeedDeviation.Cells.FromKey("CaptionExportCol").Text = "Прирост";
                }

                Grid.Bands[0].HeaderLayout.Add(GenHeader(ComboOKVED_or_REGION.SelectedValue, 0, 0, 1 + add, 3 - add));
                ConfHeader(cols.FromKey("Field").Header, 0, 2, 1, 1, " ");
                ConfHeader(cols.FromKey("CaptionExportCol").Header, 1, 2, 1, 1, " ");
            }
            else
            {
                Grid.Columns.Add("CaptionExportCol");
                Grid.Columns.FromKey("CaptionExportCol").Move(1);

                for (int i = 0; i < Grid.Rows.Count; i += 3)
                {
                    UltraGridRow RowVal = Grid.Rows[i];
                    UltraGridRow RowDeviation = Grid.Rows[i + 1]; 
                    UltraGridRow RowSpeedDeviation = Grid.Rows[i + 2];

                    RowVal.Cells.FromKey("CaptionExportCol").Text = "Значение";
                    RowDeviation.Cells.FromKey("CaptionExportCol").Text = "Прирост";
                    RowSpeedDeviation.Cells.FromKey("CaptionExportCol").Text = "Темп прироста";
                }
                ConfHeader(cols.FromKey("Field").Header, 0, 0, 1, 3, ComboOKVED_or_REGION.SelectedValue);                
            }

            ConfHeader(cols.FromKey("in-subject-invest-sum").Header, 1 + add, 1, 1, 2, "Всего инвестиций");

            ConfHeader(cols.FromKey("in-subject-invest-line").Header, 2 + add, 2, 1, 1, "Прямые инвестиции");
            ConfHeader(cols.FromKey("in-subject-invest-portfel").Header, 3 + add, 2, 1, 1, "Портфельные инвестиции");
            ConfHeader(cols.FromKey("in-subject-invest-other").Header, 4 + add, 2, 1, 1, "Прочие инвестиции");

            Grid.Bands[0].HeaderLayout.Add(GenHeader("В том числе", 2 + add, 1, 3, 1));
            Grid.Bands[0].HeaderLayout.Add(GenHeader("Иностранные инвестиции из-за рубежа в субъект РФ, тыс. дол. США", 1 + add, 0, 4, 1));

            ConfHeader(cols.FromKey("out-subject-invest-sum").Header, 1 + 4 + add, 1, 1, 2, "Всего инвестиций");

            ConfHeader(cols.FromKey("out-subject-invest-line").Header, 2 + 4 + add, 2, 1, 1, "Прямые инвестиции");
            ConfHeader(cols.FromKey("out-subject-invest-portfel").Header, 3 + 4 + add, 2, 1, 1, "Портфельные инвестиции");
            ConfHeader(cols.FromKey("out-subject-invest-other").Header, 4 + 4 + add, 2, 1, 1, "Прочие инвестиции");

            Grid.Bands[0].HeaderLayout.Add(GenHeader("В том числе", 2 + 4 + add, 1, 3, 1));
            Grid.Bands[0].HeaderLayout.Add(GenHeader("Иностранные инвестиции из субъекта РФ за рубеж, тыс. дол. США", 1 + 4 + add, 0, 4, 1));
        }
        private string GetPrevDateHint()
        {
            int Year = int.Parse(ComboQuart.SelectedNode.Parent.Text);
            int NumberQuart = int.Parse(ComboQuart.SelectedValue.Split(' ')[1]);

            string Caption = "{0} кварталу {1} года";

            if (ComboTypePeriod.SelectedValue == "к предыдущему периоду")
            {
                //return string.Format(GetFirstMounthOfNumberQuart(NumberQuart), Year.ToString());
                if (NumberQuart == 1)
                {
                    Year--;
                    NumberQuart = 5;
                }
                return string.Format(Caption, NumberQuart - 1, Year);
            }
            else
                if (ComboTypePeriod.SelectedValue == "к началу года")
                {
                    //return string.Format(GetFirstMounthOfNumberQuart(1), Year);
                    return string.Format(Caption, 1, Year);
                }
                else
                {
                    //if (NumberQuart == 4)
                    //{
                    //    return string.Format(GetFirstMounthOfNumberQuart(1), (Year).ToString());
                    //}
                    //else
                    //{
                    //    return string.Format(GetFirstMounthOfNumberQuart(NumberQuart + 1), (--Year).ToString());
                    //}
                    return string.Format(Caption, NumberQuart, --Year);
                    
                }
        }
        public void SetImageFromGrowth(UltraGridCell cell, decimal value)
        {
            bool IsReverce = false;

            decimal valShift = value;

            if (valShift == 0)
            {
                return;
            }
            if (valShift.ToString() == "100")
            {
                return;
            }

            string UpOrDouwn = valShift > 00 ? "Up" : "Down";

            string GrenOrRed = (((UpOrDouwn == "Up") && (!IsReverce)) ||
                                ((UpOrDouwn == "Down") && (IsReverce))) ?
                                              "Green" : "Red";

            string ImageName = "arrow" + GrenOrRed + UpOrDouwn + "BB.png";

            string ImagePath = "~/images/" + ImageName;

            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
            cell.Style.BackgroundImage = ImagePath;

            UltraGridCell nextCell = cell.Row.NextRow.Cells.FromKey(cell.Column.BaseColumnName);
            nextCell.Title = (valShift < 0 ? "Cнижение относительно " : "Прирост к ") + GetPrevDateHint();

        }

        string ClearDataMemberCaption(string s)
        {
            if (s.Contains("ДАННЫЕ)"))
            {
                return s.Remove(0, 1).Replace("ДАННЫЕ)", "");
            }
            return s;

        }

        string GetFirstMounthOfNumberQuart(int QuartNumber)
        {
            return string.Format("01.{0:00}", 1 + (QuartNumber - 1) * 3) + ".{0}";
        }

        private void ConfRow()
        {
            Color newxtColor = Color.Transparent;
            foreach (UltraGridRow Row in Grid.Rows)
            {
                Row.Cells[0].Style.BorderDetails.ColorLeft = Color.FromArgb(241, 241, 242);
                if (Row.Cells.FromKey("Field").Value != null)
                {
                    Row.Style.BackColor = newxtColor;

                    string[] b = Row.Cells.FromKey("Field").Text.Split('|');

                    Row.Cells.FromKey("Field").Text = b[1];
                    Row.Cells.FromKey("Field").Style.Padding.Left = 10 * (int.Parse(b[0]));


                    if (Row.NextRow.Cells.FromKey("Field").Value == null)
                    {
                        Row.NextRow.Style.BackColor = newxtColor;
                        Row.NextRow.NextRow.Style.BackColor = newxtColor;

                        foreach (UltraGridCell cell in Row.Cells)
                        {
                            try
                            {
                                UltraGridCell nextCell = Row.NextRow.Cells.FromKey(cell.Column.BaseColumnName);
                                UltraGridCell nextnextCell = Row.NextRow.NextRow.Cells.FromKey(cell.Column.BaseColumnName);
                                if (Row.NextRow.Cells.FromKey(cell.Column.BaseColumnName).Value != null)
                                {
                                    SetImageFromGrowth(
                                        Row.NextRow.Cells.FromKey(cell.Column.BaseColumnName),
                                        (decimal)Row.NextRow.NextRow.Cells.FromKey(cell.Column.BaseColumnName).Value);
                                    Row.NextRow.Cells.FromKey(cell.Column.BaseColumnName).Text = string.Format("{0:N2}%",
                                        (decimal)Row.NextRow.Cells.FromKey(cell.Column.BaseColumnName).Value);
                                }
                             
                                if (cell.Column.BaseColumnName != "Field")
                                {
                                    cell.Style.Font.Bold = true;
                                    cell.Style.BorderDetails.ColorBottom = Color.Transparent;
                                    nextCell.Style.BorderDetails.ColorTop = Color.Transparent;
                                    nextCell.Style.BorderDetails.ColorBottom = Color.Transparent;
                                    nextnextCell.Style.BorderDetails.ColorTop = Color.Transparent;
                                }

                                if (Row.NextRow.Cells.FromKey(cell.Column.BaseColumnName).Value != null)
                                {
                                    nextCell.Title = "Темп роста к " + GetPrevDateHint();
                                }
                            }
                            catch { }
                        }
                        Row.Cells.FromKey("Field").RowSpan = 3;
                        Row.Cells.FromKey("Field").Text = ClearDataMemberCaption(Row.Cells.FromKey("Field").Text);
                    }
                    else
                    {
                        if (ComboOKVED_or_REGION.SelectedValue == "ОКВЭД")
                        {
                            Row.Cells.FromKey("Field").Style.Font.Bold = int.Parse(b[0]) == 3;
                            Row.Cells.FromKey("Field").ColSpan = 9;
                        }
                    }
                }
            }
        }

        private void ConfigurateGrid()
        {
            SetHeader(Beautiful_Lie);
            ConfRow();
            CustomizerSize.ContfigurationGrid(Grid);
            Grid.Columns[1].Hidden = true;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.NullTextDefault = "-";

            Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
        }

        private void ConfiugurationHeader()
        {
            Hederglobal.Text = "Объем иностранных инвестиций по видам экономической деятельности и странам-контрагентам";
            Page.Title = Hederglobal.Text;
        }

        PeriodPars curdate;
        PeriodPars Prevdate;
        private void SetHeaderReport()
        {
            curdate = new PeriodPars(SetQuart.DataUniqeName[ComboQuart.SelectedValue]);

            Prevdate = new PeriodPars(ChosePrevPeriod());

            PageSubTitle.Text = string.Format("Ежеквартальный мониторинг иностранных инвестиций в Сахалинской области за {0} по {1}",
                curdate.DisplayText(),
                (ComboOKVED_or_REGION.SelectedValue == "ОКВЭД" ? "видам экономической деятельности" : "странам мира"));
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo(); 

            ChosenParam();
            DataBindGrid();
            ConfigurateGrid();
            ConfiugurationHeader();
            SetHeaderReport();
        }

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
            ExportGridToExcel(Grid,
                e.CurrentWorksheet,
                4, true);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");

            foreach (UltraGridColumn col in Grid.Columns)
            {
                if ((col.Hidden) || (col.Width.Value == 0))
                {
                    Grid.Bands[0].HeaderLayout.Remove(col.Header);
                    Grid.Columns.Remove(col);
                }
            }

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 7;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            DataBindGrid();
            Grid.Bands[0].Hidden = false;

            SetHeader(true);
            ConfRow();
            CustomizerSize.ContfigurationGrid(Grid);

            UltraGridExporter1.ExcelExporter.Export(Grid, sheet1);
            //ExportGridToExcel(Grid,
            //    sheet1,
            //    3, true);
            
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
            CellFormat.TopBorderStyle = CellBorderLineStyle.Default;            

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
                    for(int j = FirstAbscissaMearge;j<LastAbscissaMearge+1;j++)
                        for (int k = FirstOrdinateMearge; k < LastOrdinateMearge+1; k++)
                        {
                            SetStyleHeadertableFromExcel(WorkSheet.Rows[k].Cells[j].CellFormat);
                        }
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
                    sheet.Rows[i + startrow].Cells[col.Index].CellFormat.Indent = (int)G.Rows[i].Cells[col.Index].Style.Padding.Left.Value/10;
                    
                }
                if (G.Rows[i].Cells[0].RowSpan > 1)
                {
                    sheet.MergedCellsRegions.Add(startrow + i, 0, startrow + i + G.Rows[i].Cells[0].RowSpan - 1, 0);
                }
            }
            sheet.Columns[0].Width = 270 * 36;
            //sheet.Rows[3].Height = 40 * 40;

            sheet.Rows[70 - 2-11].Height = 60 * 40;
            for (int i = 1; i < 20; i++)
            {
                sheet.Columns[i].Width = 90 * 50;
            }

        }

        #endregion
        #endregion

        #region Экспорт в PDF

        #region Рисовалка шапки у грида на каждой страничке в ПДФе
        Graphics graphics = Graphics.FromImage(new Bitmap(1000, 500));
        private int GetStringHeight(string measuredString, Font font, int rectangleWidth)
        {
            SizeF sizeF = graphics.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);

            if (rect.Height < 23)
            {
                return 23;
            }

            return rect.Height;
        }
        private Font GetSystemFont(FontInfo baseFont)
        {
            //FontInfo f;

            //FontStyle styleFont = FontStyle.Regular;
            //if(baseFont.Bold)
            //{
            //    styleFont = FontStyle.Bold;
            //}

            //baseFont = Grid.DisplayLayout.EditCellStyleDefault.Font;

            Font font = new Font(baseFont.Name, (int)baseFont.Size.Unit.Value);//, (int)baseFont.Size.Unit.Value,styleFont); 

            return font;
        }

        int PreExportedHeight = 120;

        int GetRowHeight(UltraGridRow row)
        {
            int maxHeight = 0;
            foreach (UltraGridCell cell in row.Cells)
            {
                int CurHeight = GetStringHeight(cell.Text, GetSystemFont(Grid.DisplayLayout.EditCellStyleDefault.Font), (int)cell.Column.Width.Value);
                maxHeight = CurHeight > maxHeight ? CurHeight : maxHeight;
            }

            return maxHeight;
        }

        void PdfExporter_RowExported(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportedEventArgs e)
        {
            e.ReportRow.KeepWithNext = true;
            if (PreExportedHeight > 800)
            {
                e.ReportRow.KeepWithNext = false; 

                PreExportedHeight = 60;

                ITableRow headerMainRow = e.ReportRow.Parent.AddRow();

                headerMainRow.KeepWithNext = true;

                CreateAllRootHeader(headerMainRow); 
                
            }

            e.ReportRow.Margins.All = 0;

            PreExportedHeight += GetRowHeight(e.GridRow);
        }
        #endregion
        private int CreateAllRootHeader(ITableRow RootRow)
        {
            int sumW = 0;
            foreach (HeaderBase Header in Grid.Bands[0].HeaderLayout)
            {
                if (HeaderIsRootLevel(Header))
                {
                    sumW += CreateHierarhyHeader(Header, RootRow);
                }
            }
            return sumW;
        }

        private void FormatGridXZ()
        {

            for (int i = 0; i < Grid.Rows.Count; i += 3)
            {
                UltraGridRow row = Grid.Rows[i];
                row.NextRow.Cells[0].Text = row.Cells[0].Text;
                row.Cells[0].Text = "";
                row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                row.NextRow.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                row.NextRow.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;


            }
        }
        

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


            Grid.Bands[0].HeaderLayout.Clear();
            Grid.Bands[0].Columns.Clear();

            DataBindGrid();
            Grid.Bands[0].Hidden = false;

            SetHeader(true);
            ConfRow();
            CustomizerSize.ContfigurationGrid(Grid);

            for (int i = 0; i < Grid.Rows.Count; i += 3)
            {
                UltraGridRow Row = Grid.Rows[i];
                UltraGridRow RowNext = Row.NextRow;


                RowNext.Cells[0].Value = Row.Cells[0].Value;
                Row.Cells[0].Value = null;

                Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                RowNext.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
                RowNext.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }

            UltraGridExporter1.PdfExporter.Export(Grid, e_);
        }




        Background headerBackground = null;
        Borders headerBorders = null;

        private bool HeaderIsChildren(HeaderBase Parent, HeaderBase Children)
        {
            if (Parent == Children)
            {
                return false;
            }

            if (((Parent.RowLayoutColumnInfo.OriginY + Parent.RowLayoutColumnInfo.SpanY) == Children.RowLayoutColumnInfo.OriginY)
                &&
                ((Parent.RowLayoutColumnInfo.OriginX <= Children.RowLayoutColumnInfo.OriginX) &&
                ((Parent.RowLayoutColumnInfo.OriginX + Parent.RowLayoutColumnInfo.SpanX) > Children.RowLayoutColumnInfo.OriginX)))
            {
                return true;
            }
            return false;
        }

        private List<HeaderBase> GetChildHeader(HeaderBase ParentHeder)
        {
            List<HeaderBase> ChildHeader = new List<HeaderBase>();

            foreach (HeaderBase Header in Grid.Bands[0].HeaderLayout)
            {

                if (HeaderIsChildren(ParentHeder, Header))
                {
                    ChildHeader.Add(Header);
                }

            }

            return ChildHeader;
        }

        protected bool HeaderIsRootLevel(HeaderBase Header)
        {
            return Header.RowLayoutColumnInfo.OriginY == 0;
        }

        ITableRow CreateChildrenRow(ITableCell row)
        {
            return row.Parent.Parent.AddRow();
        }

        int[] PDFHeaderHeightsLevel = { 15, 20, 30 };

        int PDFGetLevelHeight(int level, int span)
        {
            int sumHeightLevel = 0;
            for (int i = level; i < level + span; i++)
            {
                sumHeightLevel += PDFHeaderHeightsLevel[i];
            }
            return sumHeightLevel;
        }

        private int CreateHierarhyHeader(HeaderBase header, ITableRow row)
        {
            List<HeaderBase> ChildHeaders = GetChildHeader(header);
            row = row.AddCell().AddTable().AddRow();

            ITableCell ParentCell = row.AddCell();

            int width = AddTableCell(ParentCell, header, header.RowLayoutColumnInfo.SpanX, PDFGetLevelHeight(header.RowLayoutColumnInfo.OriginY, header.RowLayoutColumnInfo.SpanY));

            if (ChildHeaders.Count > 0)
            {
                width = 0;
                ITableRow ChildrenRow = row.Parent.AddRow();
                foreach (HeaderBase ChildHeader in ChildHeaders)
                {
                    width += CreateHierarhyHeader(ChildHeader, ChildrenRow);
                }
                setHederWidth(ParentCell, width);
            }
            return width;

        }


        private void ExportHeader(ITable Table)
        {
            String.Format("1");
            ITableRow RootRow = Table.AddRow();

            int sumW = 0;

            sumW = 16;

            foreach (HeaderBase Header in Grid.Bands[0].HeaderLayout)
            {
                if (HeaderIsRootLevel(Header))
                {
                    sumW += CreateHierarhyHeader(Header, RootRow);
                }
            }
            Table.Width = new FixedWidth(sumW);
        }

        private void ApplyHeader()
        {
            foreach (UltraGridColumn col in Grid.Columns)
            {
                Grid.Bands[0].HeaderLayout.Add(col.Header);
            }
        }



        private void PdfExporter_Test(object sender, MarginCellExportingEventArgs e)
        {
            if (headerBackground != null)
            {
                return;
            }
            PreProcessing(e);
            ITable Table = e.ReportCell.Parent.Parent;
            ExportHeader(Table);
        }



        private void PreProcessing(MarginCellExportingEventArgs e)
        {
            headerBackground = e.ReportCell.Background;
            headerBorders = e.ReportCell.Borders;
            e.ReportCell.Parent.Height = new FixedHeight(0);
        }

        #region Кривая н рабочая
        private void PdfExporter_HeaderCellExporting(object sender, MarginCellExportingEventArgs e)
        {
            if (headerBackground != null)
            {
                return;
            }
            headerBackground = e.ReportCell.Background;
            headerBorders = e.ReportCell.Borders;

            e.ReportCell.Parent.Height = new FixedHeight(0);


            ITable Table = e.ReportCell.Parent.Parent;

            ITableRow row = Table.AddRow();

            AddTableCell(row, "", 268, 30);


            Double WidthSubject =
                Grid.Columns.FromKey("ValueSubject").Width.Value * 0.75 +
                Grid.Columns.FromKey("GrowthSubject").Width.Value * 0.75 +
                Grid.Columns.FromKey("SpeedGrowthSubject").Width.Value * 0.75 +
                Grid.Columns.FromKey("RangFO").Width.Value * 0.75 +
                Grid.Columns.FromKey("RangRF").Width.Value * 0.75;

            Double WidthFO =
                Grid.Columns.FromKey("ValueFO").Width.Value * 0.75 +
                Grid.Columns.FromKey("GrowthFO").Width.Value * 0.75 +
                Grid.Columns.FromKey("SpeedGrowthFO").Width.Value * 0.75;

            Double WidthRF =
                Grid.Columns.FromKey("ValueRF").Width.Value * 0.75 +
                Grid.Columns.FromKey("GrowthRF").Width.Value * 0.75 +
                Grid.Columns.FromKey("SpeedGrowthRF").Width.Value * 0.75;

            row = Table.AddRow();
            AddTableCell(row, "Показатель", 268, 30);
            for (int i = Grid.Columns.FromKey("SEP").Index + 1; i < Grid.Columns.Count; i++)
            {
                AddTableCell(row, Grid.Columns[i].Header.Caption, Grid.Columns[i].Width.Value * 0.75, 30);
            }
        }
        #endregion

        void setHederWidth(ITableCell tableCell, Double width)
        {
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
        }

        private int AddTableCell(ITableCell tableCell, HeaderBase header, Double width, Double Height)
        {
            if (header.Column != null)
            {

                System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

                if (BN == "IE")
                {
                    width = 0.751 * (int)header.Column.Width.Value;
                }
                else
                {
                    if (BN == "FIREFOX")
                    {
                        width = 0.7535 * (int)header.Column.Width.Value;
                    }
                    else
                    {
                        width = 0.754 * (int)header.Column.Width.Value;
                    }
                }
            }

            SetCellStyle(tableCell);
            tableCell.Height = new FixedHeight((int)Height);
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
            IText text = tableCell.AddText();
            SetFontStyle(text);

            text.AddContent(header.Caption + " ");

            return (int)width;
        }
        private ITableCell AddTableCell(ITableRow row, string cellText, Double width, Double Height)
        {
            ITableCell tableCell = row.AddCell();


            SetCellStyle(tableCell);
            tableCell.Height = new FixedHeight((int)Height);
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);
            IText text = tableCell.AddText();

            text.Style.Font.Size = 1;
            text.Paddings.Left = 100;
            SetFontStyle(text);

            text.AddContent(cellText);

            return tableCell;
        }
        public static void SetFontStyle(IText t)
        {
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font(new System.Drawing.Font("Arial", 8));
            t.Style.Font = font;
            t.Style.Font.Bold = true;
            t.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Center;
        }
        public void SetCellStyle(ITableCell headerCell)
        {
            headerCell.Alignment.Horizontal = Alignment.Center;
            headerCell.Alignment.Vertical = Alignment.Middle;
            headerCell.Borders = headerBorders;
            headerCell.Paddings.All = 2;
            headerCell.Background = headerBackground;
        }

        #endregion



        #endregion

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