using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Primitive = Infragistics.UltraChart.Core.Primitives.Primitive;
using Text = Infragistics.UltraChart.Core.Primitives.Text;
using System.Web;
using System.IO;
using Box = Infragistics.UltraChart.Core.Primitives.Box;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class ORG_0001_0008 : CustomReportPage
    {
        IDataSetCombo SetCurDay;
        IDataSetCombo SetPrevDay;

        Day CurDay;
        Day PrevDay;
        Day FyDay;

        CustomParam ChosenCurPeriod { get { return (UserParams.CustomParam("ChosenCurPeriod")); } }
        CustomParam ChosenPrevPeriod { get { return (UserParams.CustomParam("ChosenPrevPeriod")); } }
        CustomParam ChosenFYPeriod { get { return (UserParams.CustomParam("ChosenFirstPeriod")); } }

        CustomParam ChosenCurPeriodSU { get { return (UserParams.CustomParam("ChosenCurPeriodSU")); } }
        CustomParam ChosenPrevPeriodSU { get { return (UserParams.CustomParam("ChosenPrevPeriodSU")); } }
        CustomParam ChosenFYPeriodSU { get { return (UserParams.CustomParam("ChosenFirstPeriodSU")); } }

        CustomParam SelectField { get { return (UserParams.CustomParam("SelectField")); } }

        CustomParam Group { get { return (UserParams.CustomParam("Group")); } }
        CustomParam Fields { get { return (UserParams.CustomParam("Fields")); } }

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

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;

            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            Grid.Width = 860;
            Grid.Height = Unit.Empty;
            Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            UltraChartColumn.Width = 750;
            UltraChartLine.Width = 750;
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
        }

        #region Combo

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
                ComboCurDay.SetСheckedState(ComboCurDay.SelectedNode.Nodes[0].Text, true);
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

        #endregion

        #region Grid

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

           // ReportTable.Columns.Add("PREVDeviation", typeof(decimal));
            ReportTable.Columns.Add("PREVSpeedDeviation", typeof(decimal));

          //  ReportTable.Columns.Add("FYDeviation", typeof(decimal));
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
                        }
                        FirstRow = false;
                    }
                    catch { }
                    if (Row["PREV"] != DBNull.Value)
                    {
                        Row["PREVSpeedDeviation"] = ((decimal)(Row["CUR"]) / (decimal)(Row["PREV"]) - 1);
                    }
                    if (Row["FY"] != DBNull.Value)
                    {
                        Row["FYSpeedDeviation"] = ((decimal)(Row["CUR"]) / (decimal)(Row["FY"]) - 1);
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

        private void SetHeader()
        {
            ColumnsCollection cols = Grid.Columns;

            cols[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            cols[1].CellStyle.Font.Bold = true;
            cols[1].CellStyle.ForeColor = Color.White;
            cols[1].CellStyle.Font.Size = 14;
            
            CRHelper.FormatNumberColumn(cols.FromKey("CUR"), "### ### ##0.00");
            CRHelper.FormatNumberColumn(cols.FromKey("PREV"), "### ### ##0.00");
            CRHelper.FormatNumberColumn(cols.FromKey("FY"), "### ### ##0.00");

            CRHelper.FormatNumberColumn(cols.FromKey("PREVSpeedDeviation"), "### ### ##0.00 %");
            CRHelper.FormatNumberColumn(cols.FromKey("FYSpeedDeviation"), "### ### ##0.00 %");

            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            headerLayout.AddCell("Предприятия");
            GridHeaderCell avgCell = headerLayout.AddCell("Средняя розничная цена, рублей за тонну");
            avgCell.AddCell(CurDay.GridHeader() + "  ");
            avgCell.AddCell(PrevDay.GridHeader() + "  ");
            avgCell.AddCell(FyDay.GridHeader() + "  ");
            headerLayout.AddCell(String.Format("Темп прироста к {0}", PrevDay.GridHeader()));
            headerLayout.AddCell("Темп прироста к началу года");

            headerLayout.ApplyHeaderInfo();
        }

        private void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            foreach (UltraGridColumn column in Grid.Columns)
            {
                column.Width = GetColumnWidth(column.Header.Caption);
            }
        }

        private int GetColumnWidth(string columnName)
        {
            if (columnName.Contains("Field"))
            {
                return 165;
            }
            if (columnName.Contains("CUR"))
            {
                return 130;
            }
            if (columnName.Contains("SpeedDeviation"))
            {
                return 110;
            }
            return 120;
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

            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center left; padding-left: 5px";
            cell.Style.BackgroundImage = ImagePath;



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
                SetImageFromGrowth(Row.Cells.FromKey("PREVSpeedDeviation"), (decimal)Row.Cells.FromKey("PREVSpeedDeviation").Value);
            if (Row.Cells.FromKey("FYSpeedDeviation").Value != null)
                SetImageFromGrowth(Row.Cells.FromKey("FYSpeedDeviation"), (decimal)Row.Cells.FromKey("FYSpeedDeviation").Value);

            SetStar(Row);

            SetStarChar(Row.Cells.FromKey("Field"));

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
                        addOrAdd(val, Grid.Columns[j].BaseColumnName, Grid.Rows[i].Cells[j], max);
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
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            Grid.DisplayLayout.NullTextDefault = "-";
            Grid.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;

            Grid.DisplayLayout.SelectTypeRowDefault = SelectType.Single;
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            //Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            if (!string.IsNullOrEmpty(SelectField.Value))
            {
                ActiverRowFromValueFirstCell(SelectField.Value);
            }
        }

        #endregion

        #region Chart

        private void ConfColChart()
        {
            UltraChartColumn.ChartType = ChartType.ColumnChart;

            UltraChartColumn.Width = 760;
            UltraChartColumn.Height = 400;

            UltraChartColumn.TitleLeft.Margins.Bottom = UltraChartColumn.Axis.X.Extent;

            UltraChartColumn.ColumnChart.ColumnSpacing = 0;

            UltraChartColumn.Legend.Visible = false;
            UltraChartColumn.Axis.X.Labels.Visible = false;
            
            UltraChartColumn.Axis.X.Margin.Near.Value = 2;
            UltraChartColumn.Axis.X.Margin.Far.Value = 2;
            UltraChartColumn.Tooltips.FormatString = "«<SERIES_LABEL>»<br>На <b>" + CurDay.GridHeader() + " г.</b><br><b><DATA_VALUE:N2></b>, рублей за тонну";
            UltraChartColumn.Tooltips.Font.Name = "Verdana";
            UltraChartColumn.Tooltips.Font.Size = 11;

            UltraChartColumn.BackColor = Color.Transparent;

            #region Настройка диаграммы

            UltraChartColumn.Height = Unit.Empty;

            UltraChartColumn.ChartType = ChartType.ColumnChart;
            UltraChartColumn.Border.Thickness = 0;

            UltraChartColumn.ColumnChart.SeriesSpacing = 0;
            UltraChartColumn.ColumnChart.ColumnSpacing = 1;

            UltraChartColumn.Axis.X.Extent = 170;
            UltraChartColumn.Axis.X.Labels.Visible = false;
            UltraChartColumn.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChartColumn.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChartColumn.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);

            UltraChartColumn.Axis.Y.Visible = true;
            UltraChartColumn.Axis.Y.Extent = 90;
            UltraChartColumn.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChartColumn.Axis.Y.Labels.Font = new Font("Verdana", 10);

            UltraChartColumn.ColorModel.ModelStyle = ColorModels.PureRandom;

            //UltraChartColumn.Tooltips.FormatString = "<SERIES_LABEL>\nРозничная цена: <b><DATA_VALUE:N2></b>, рубль";
            #endregion

            UltraChartColumn.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(Chart_FillSceneGraph);
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

        decimal GetMaxFormCol(DataTable Table, bool max)
        {
            decimal mv = max ? decimal.MinValue : decimal.MaxValue;
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
                Row["Region"] = ReportTable.Rows[i]["Field"].ToString().Replace("Березовский филиал Аэропорт Сургут", "Березовский филиал\n Аэропорт Сургут");
                //GenShortNameRegion(ReportTable.Rows[i]["Field"].ToString());
                Row["CUR"] = ReportTable.Rows[i]["CUR"];
            }

            decimal max = GetMaxFormCol(TableChart, true);
            decimal min = GetMaxFormCol(TableChart, false);

            UltraChartColumn.Axis.Y.RangeType = AxisRangeType.Custom;
            UltraChartColumn.Axis.Y.RangeMax = (double)max * 1.1;
            UltraChartColumn.Axis.Y.RangeMin = (double)min * 0.9;


            UltraChartColumn.DataSource = SortTable(TableChart);
            UltraChartColumn.DataBind();
        }

        private void GenerationColChart()
        {
            ConfColChart();
            DataBindColChart();
        }





        #endregion

        #region LineChart

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
                UltraChartLine.DataSource = TableChart;
                UltraChartLine.DataBind();
            }
            catch { }
        }

        private void ConfLineChart()
        {
            AddLineAppearencesUltraChart1(UltraChartLine);
            SetupDynamicChart(UltraChartLine, "<ITEM_LABEL><br>На <b>" + "<SERIES_LABEL> г." + "</b><br><b><DATA_VALUE:N2></b>, рублей за тонну", "<DATA_VALUE:N0>");
        }

        private void AddLineAppearencesUltraChart1(UltraChart chart)
        {
            Color color = Color.Green;

            chart.Width = 760;
            chart.Height = 400;
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();

            PaintElement pe = new PaintElement();

            pe.Fill = color;
            pe.FillStopColor = color;
            pe.StrokeWidth = 0;
            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe.FillOpacity = 255;
            pe.FillStopOpacity = 200;
            chart.ColorModel.Skin.PEs.Add(pe);
            pe.Stroke = Color.Black;
            pe.StrokeWidth = 0;

            LineAppearance lineAppearance2 = new LineAppearance();
            lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance2.IconAppearance.PE = pe;

            chart.AreaChart.LineAppearances.Add(lineAppearance2);
        }

        private void SetupDynamicChart(UltraChart chart, string tooltipsFormatString, string axisYLabelsFormatString)
        {
            chart.ChartType = ChartType.AreaChart;
            chart.Border.Thickness = 0;

            chart.Tooltips.Font.Name = "Verdana";
            chart.Tooltips.Font.Size = 11;
            chart.Tooltips.FormatString = tooltipsFormatString;
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            chart.Axis.X.Extent = 80;
            chart.Axis.X.Labels.Font = new Font("Verdana", 10);
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.Y.Labels.SeriesLabels.Visible = false;

            chart.Axis.Y.Extent = 60;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 10);

            chart.Axis.X.Labels.HorizontalAlign = StringAlignment.Near;

            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = "Рублей за тонну";
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.VerticalAlign = StringAlignment.Near;
            chart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            chart.TitleLeft.Margins.Top = 0;
            chart.TitleLeft.Margins.Bottom = chart.Axis.X.Extent;
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.TitleLeft.FontColor = Color.White;

            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;

            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;
            chart.Axis.Y2.MinorGridLines.Color = Color.Black;
            chart.Axis.Y2.MajorGridLines.Color = Color.Black;

            chart.Data.EmptyStyle.Text = " ";
            chart.EmptyChartText = " ";

            chart.AreaChart.NullHandling = NullHandling.DontPlot;
            chart.Data.ZeroAligned = false;

            chart.Legend.Visible = false;

            chart.InvalidDataReceived +=
                new ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            chart.FillSceneGraph += new FillSceneGraphEventHandler(chart_FillSceneGraph);
            chart.Data.SwapRowsAndColumns = true;

            chart.Axis.X.Margin.Near.Value = 2;
            chart.Axis.X.Margin.Far.Value = 2;
        }

        void chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];

                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.PointSet")
                {
                    foreach (Primitive child in primitive.GetPrimitives())
                    {
                        if (child.ToString() == "Infragistics.UltraChart.Core.Primitives.DataPoint")
                        {
                            DataPoint dataPoint = (DataPoint)child;
                            if (dataPoint.DataPoint == null)
                            {
                                dataPoint.Visible = false;
                            }
                        }
                    }
                }
                if (primitive is PointSet)
                {
                    var pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 20;
                    }
                }
            }
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

        #endregion


        public void WriteSubstitution(HttpResponseSubstitutionCallback callback
)
        {

        }

        private void SetHeaderReport()
        {
            elementCaption.Text = string.Format("Распределение предприятий по стоимости авиационного керосина по состоянию на&nbsp;<span class='DigitsValue'>{0}</span>, рублей за тонну", CurDay.GridHeader());
            IPadElementHeader4.Text = "Динамика средней стоимости авиационного керосина по ХМАО-Югре, рублей за тонну";
            ReportTitle.Text = String.Format("Еженедельный мониторинг цен на авиационный керосин в разрезе предприятий торговли,<br/> Ханты-Мансийский автономный округ, по состоянию на&nbsp;<span class='DigitsValue'>{0}</span> года", CurDay.GridHeader());
        }

        #region Линии на диограмке

        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = ColorLine;
            Line.PE.StrokeWidth = 2;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;


            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
             textLabel.PE.Fill = System.Drawing.Color.White;
            //textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 8);
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);

            textLabel.labelStyle.FontColor = Color.White;

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


            //textLabel.labelStyle.FontColor = Color.LightGray;

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

                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds = new Rectangle(text.bounds.Left - 5, text.bounds.Top, text.bounds.Width, text.bounds.Height);
                    text.bounds.Width = 50;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 10);
                    text.labelStyle.WrapText = true;
                }

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
            t.labelStyle.FontColor = Color.White;
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
