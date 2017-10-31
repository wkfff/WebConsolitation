using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;

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
using Infragistics.WebUI.UltraWebChart;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Events;
using System.IO;
using System.Drawing.Imaging;
using Infragistics.UltraGauge.Resources;


namespace Krista.FM.Server.Dashboards.reports.EO_HAO_001_Dasboard
{
    public partial class _default : CustomReportPage
    {
        ICustomizerSize CustomizerSize;
        
        CustomParam ChosenDate { get { return (UserParams.CustomParam("ChosenDate")); } }
        CustomParam Code { get { return (UserParams.CustomParam("code")); } }

        IDataSetCombo DataComboQuart = new DataSetComboTwoLevel();

        int ScreenWidth { get { return ((int)Session[CustomReportConst.ScreenWidthKeyName]); } }
        int ScreenHeight { get { return ((int)Session[CustomReportConst.ScreenHeightKeyName]); } }

        #region Подонка размеров элементов, под браузер и разрешение
        abstract class ICustomizerSize
        {
            public enum BrouseName { IE, FireFox, SafariOrHrome }

            public enum ScreenResolution { _800x600, _1280x1024, Default }

            public BrouseName NameBrouse;

            public abstract int GetGridWidth();
            public abstract int GetChartWidth();

            public abstract int GetComboPeriodWidth();
            public abstract int GetComboDirectionWidth();
            public abstract int GetComboRegonWidth();

            public abstract int GetChartLegendWidth(bool Dynamic);

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
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)1200 - 6) / 100;

                ColsGrid.FromKey("SEP").Width = OnePercent * 18;

                ColsGrid.FromKey("ValueSubject").Width = OnePercent * 9;

                ColsGrid.FromKey("GrowthSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthSubject").Width = OnePercent * 6;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 5;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 5;

                ColsGrid.FromKey("ValueFO").Width = OnePercent * 10;

                ColsGrid.FromKey("GrowthFO").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthFO").Width = OnePercent * 6;

                ColsGrid.FromKey("ValueRF").Width = OnePercent * 9;

                ColsGrid.FromKey("GrowthRF").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthRF").Width = OnePercent * 6;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)1200 - 6) / 100;

                ColsGrid.FromKey("SEP").Width = OnePercent * 20;

                ColsGrid.FromKey("ValueSubject").Width = OnePercent * 9;

                ColsGrid.FromKey("GrowthSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthSubject").Width = OnePercent * 6;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 5;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 5;

                ColsGrid.FromKey("ValueFO").Width = OnePercent * 10;

                ColsGrid.FromKey("GrowthFO").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthFO").Width = OnePercent * 6;

                ColsGrid.FromKey("ValueRF").Width = OnePercent * 9;

                ColsGrid.FromKey("GrowthRF").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthRF").Width = OnePercent * 6;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)1200 - 6) / 100;

                ColsGrid.FromKey("SEP").Width = OnePercent * 22;

                ColsGrid.FromKey("ValueSubject").Width = OnePercent * 9;

                ColsGrid.FromKey("GrowthSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthSubject").Width = OnePercent * 6;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 5;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 5;

                ColsGrid.FromKey("ValueFO").Width = OnePercent * 10;

                ColsGrid.FromKey("GrowthFO").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthFO").Width = OnePercent * 6;

                ColsGrid.FromKey("ValueRF").Width = OnePercent * 9;

                ColsGrid.FromKey("GrowthRF").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthRF").Width = OnePercent * 6;
            }
            #endregion

            public override int GetChartLegendWidth(bool Dynamic)
            {
                return Dynamic ? 600 : 400;
            }

            public override int GetComboPeriodWidth()
            {
                return 150;
            }

            public override int GetComboDirectionWidth()
            {
                return 240;
            }

            public override int GetComboRegonWidth()
            {
                return 180;
            }
        }

        class CustomizerSize_1280x1024 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5).Value;
                    case BrouseName.FireFox:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10).Value;
                    case BrouseName.SafariOrHrome:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5).Value;
                    default:
                        return (int)CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10).Value;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
                    case BrouseName.FireFox:
                        return CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
                    case BrouseName.SafariOrHrome:
                        return CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
                    default:
                        return CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
                }
            }

            public CustomizerSize_1280x1024(BrouseName NameBrouse) : base(NameBrouse) { }

            #region Настрока размера колонок для грида
            protected override void ContfigurationGridIE(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in  Grid.Columns)
                {
                    col.Width = (75 / (Grid.Columns.Count - 1)) * onePercent;
                }
                Grid.Columns[0].Width = 25 * onePercent;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (75 / (Grid.Columns.Count - 1)) * onePercent;
                }
                Grid.Columns[0].Width = 25 * onePercent;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    col.Width = (75 / (Grid.Columns.Count - 1)) * onePercent;
                }
                Grid.Columns[0].Width = 25*onePercent;
            }

            public override int GetChartLegendWidth(bool Dynamic)
            {
                return Dynamic ? 1120 : 850;
            }

            #endregion

            public override int GetComboPeriodWidth()
            {
                return 250;
            }

            public override int GetComboDirectionWidth()
            {
                return 240;
            }

            public override int GetComboRegonWidth()
            {
                return 380;
            }
        }
        #endregion

        static class DataBaseHelper
        {
            static DataProvider ActiveDataPorvider = DataProvidersFactory.PrimaryMASDataProvider;

            public static DataTable ExecQueryByID(string QueryId)
            {
                return ExecQueryByID(QueryId, "FirstColumn");
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

        private static class DataTableHelper
        {
            public static DataRow FindRow(DataTable Table, string Col, string value)
            {
                foreach (DataRow row in Table.Rows)
                {
                    string curVal = row[Col].ToString();
                    if (curVal == value)
                    {
                        return row;
                    }
                }
                return null;
            }

            public static void SetRang(DataTable Table, string ColumnValueName, string ColumnRangName, int StartRow)
            {
                SetRang(Table, ColumnValueName, ColumnRangName, StartRow, true);
            }

            public static void SetRang(DataTable Table, string ColumnValueName, string ColumnRangName, int StartRow, bool revrce)
            {
                int RowCount = Table.Rows.Count;
                int IndexMax = StartRow;
                int IndexMin = StartRow;
                int rang = 0;
                for (int i = StartRow; i < RowCount; i++)
                {
                    for (int j = StartRow; j < RowCount; j++)
                    {
                        if (Table.Rows[j][ColumnValueName] != System.DBNull.Value)
                        {

                            if (((decimal)(Table.Rows[j][ColumnValueName]) <= (decimal)(Table.Rows[IndexMax][ColumnValueName])) && (Table.Rows[j][ColumnRangName] == DBNull.Value))
                            {
                                IndexMax = j;
                            }
                            if (((decimal)(Table.Rows[j][ColumnValueName]) > (decimal)(Table.Rows[IndexMin][ColumnValueName])) && (Table.Rows[j][ColumnRangName] == DBNull.Value))
                            {
                                IndexMin = j;
                            }

                        }
                    }
                    if (revrce)
                    {
                        int b = IndexMax;
                        IndexMax = IndexMin;
                        IndexMin = b;
                    }
                    if (Table.Rows[IndexMin][ColumnValueName] != System.DBNull.Value)
                        if (Table.Rows[IndexMin][ColumnRangName] == DBNull.Value)
                        {
                            Table.Rows[IndexMin][ColumnRangName] = ++rang;
                        }
                    IndexMin = IndexMax;
                }

            }

            public static void ClearEmtyRow(DataTable Table, int NumberCol)
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    DataRow row = Table.Rows[i];
                    if (row[NumberCol] == DBNull.Value)
                    {
                        row.Delete();
                    }
                }
            }

            public static void CopyRows(DataTable BaseTable, DataTable NewTable, int start, int end)
            {
                for (int i = start; i < end; i++)
                {
                    DataRow BaseRow = BaseTable.Rows[i];
                    DataRow newRow = NewTable.NewRow();
                    foreach (DataColumn BaseCol in BaseTable.Columns)
                    {
                        if (NewTable.Columns.Contains(BaseCol.ColumnName))
                        {
                            newRow[BaseCol.ColumnName] = BaseRow[BaseCol];
                        }
                    }
                    NewTable.Rows.Add(newRow);
                }
            }

            public static void CopyRows(DataTable BaseTable, DataTable NewTable)
            {
                CopyRows(BaseTable, NewTable, 0, BaseTable.Rows.Count);
            }

            private static void CopyPasteColumn(DataTable TableBase, DataTable TableNew, int firstColumn, int lastColumn)
            {
                for (int i = firstColumn; i <= lastColumn; i++)
                {
                    TableNew.Columns.Add(TableBase.Columns[i].ColumnName, TableBase.Columns[i].DataType);
                }
            }


            public enum Extrmum { Max, Min }
            public static double GetExtremum(DataTable Table, DataColumn Column, Extrmum TypeExtremum)
            {
                double MaxOrMin = TypeExtremum == Extrmum.Max ? double.NegativeInfinity : double.PositiveInfinity;

                foreach (DataRow row in Table.Rows)
                {
                    if (row[Column] != DBNull.Value)
                    {
                        double Value = (double)(row[Column]);

                        if (TypeExtremum == Extrmum.Max)
                        {
                            if (MaxOrMin < Value)
                            {
                                MaxOrMin = Value;
                            }
                        }
                        else
                        {
                            if (MaxOrMin > Value)
                            {
                                MaxOrMin = Value;
                            }
                        }

                    }
                }

                return MaxOrMin;
            }

            public static DataTable SortingDataTable(DataTable BaseTable, string ColSort)
            {
                return SortingDataTable(BaseTable, ColSort, 0);
            }

            private static bool ComparedObject(object o1, object o2)
            {
                if (o1.GetType() == typeof(string))
                {
                    return string.Compare(o1.ToString(), o2.ToString()) > 0;
                }
                if (o1.GetType() == typeof(double))
                {
                    return ((double)(o1) > (double)(o2));
                }
                throw new Exception("Неизвестный тип");

            }

            public static DataTable SortingDataTable(DataTable BaseTable, string ColSort, int StartRow)
            {
                DataTable SortTabe = new DataTable();
                CopyPasteColumn(BaseTable, SortTabe, 0, BaseTable.Columns.Count - 1);

                CopyRows(BaseTable, SortTabe, 0, StartRow);

                for (; BaseTable.Rows.Count > StartRow; )
                {
                    int IndexMaxRow = StartRow;

                    for (int j = StartRow; j < BaseTable.Rows.Count; j++)
                    {
                        if ((BaseTable.Rows[IndexMaxRow][ColSort] != DBNull.Value) &&
                            (BaseTable.Rows[j][ColSort]) != DBNull.Value)
                            if (ComparedObject(BaseTable.Rows[IndexMaxRow][ColSort], (BaseTable.Rows[j][ColSort])))
                            {
                                IndexMaxRow = j;
                            }
                    }

                    DataRow newRow = SortTabe.NewRow();
                    DataRow BaseRow = BaseTable.Rows[IndexMaxRow];

                    foreach (DataColumn BaseCol in BaseTable.Columns)
                    {
                        if (SortTabe.Columns.Contains(BaseCol.ColumnName))
                        {
                            newRow[BaseCol.ColumnName] = BaseRow[BaseCol];
                        }
                    }
                    SortTabe.Rows.Add(newRow);

                    BaseTable.Rows[IndexMaxRow].Delete();
                }
                return SortTabe;
            }

            public static void ClearDataLevel(DataTable Table, string ColFilring)
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    if (Table.Rows[i][ColFilring].ToString().Contains("ДАННЫЕ)"))
                    {
                        Table.Rows[i].Delete();
                        i--;
                    }
                }

            }

        }

        private static class CustomizeUltraGridHelper
        {
            private static int GetMaxRowFromCol(UltraWebGrid dt, int col)
            {
                int MaxIndex = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((dt.Rows[MaxIndex].Cells[col].Value) == null)
                    {
                        MaxIndex = i;
                    }
                    if (null != dt.Rows[i].Cells[col].Value)
                        if ((double)(dt.Rows[i].Cells[col].Value) > (double)(dt.Rows[MaxIndex].Cells[col].Value))
                        {
                            MaxIndex = i;
                        }



                }
                return MaxIndex;
            }

            private static int GetMinRowFromCol(UltraWebGrid dt, int col)
            {
                int MaxIndex = 1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((dt.Rows[MaxIndex].Cells[col].Value) == null)
                    {
                        MaxIndex = i;
                    }
                    if (null != dt.Rows[i].Cells[col].Value)
                        if ((double)(dt.Rows[i].Cells[col].Value) < (double)(dt.Rows[MaxIndex].Cells[col].Value))
                        {
                            MaxIndex = i;
                        }

                }
                return MaxIndex;
            }

            private static void SetStar(UltraWebGrid G, int Col, int RowBaseVaslue, string Star, string Title)
            {
                for (int i = 0; G.Rows.Count > i; i++)
                {
                    if (G.Rows[i].Cells[Col].Text == G.Rows[RowBaseVaslue].Cells[Col].Text)
                    {
                        G.Rows[i].Cells[Col].Title = Title;
                        G.Rows[i].Cells[Col].Style.BackgroundImage = Star;//"~/images/starYellowBB.png";
                        G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        //"Самый высокий интегральный показатель"
                    }

                }
            }

            public static void SetStarFromValue(UltraWebGrid G, int Col, int BaseValue, string Star, string Title)
            {
                for (int i = 0; G.Rows.Count > i; i++)
                {
                    if (G.Rows[i].Cells[Col].Text == BaseValue.ToString())
                    {
                        G.Rows[i].Cells[Col].Title = Title;
                        G.Rows[i].Cells[Col].Style.BackgroundImage = Star;//"~/images/starYellowBB.png";
                        G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        //"Самый высокий интегральный показатель"
                    }

                }
            }

            public static void SetImageFromGrowth(UltraGridRow row, string ColumnGrowthName, bool inverce)
            {

                if ((row.Cells.FromKey(ColumnGrowthName).Value == null) || ((double)row.Cells.FromKey(ColumnGrowthName).Value == 100))
                {
                    return;
                }

                bool IsReverce = inverce;

                double valShift = (double)row.Cells.FromKey(ColumnGrowthName).Value;

                string UpOrDouwn = valShift > 1 ? "Up" : "Down";

                string GrenOrRed = (((UpOrDouwn == "Up") && (!IsReverce)) ||
                                    ((UpOrDouwn == "Down") && (IsReverce))) ?
                                                  "Green" : "Red";

                string ImageName = "arrow" + GrenOrRed + UpOrDouwn + "BB.png";

                string ImagePath = "~/images/" + ImageName;

                row.Cells.FromKey(ColumnGrowthName).Style.CustomRules = "background-repeat: no-repeat; background-position: center left";
                row.Cells.FromKey(ColumnGrowthName).Style.BackgroundImage = ImagePath;
            }

            public static void SetStar(UltraWebGrid Grid, int ColFromImage)
            {
                int LastMaxIndex = CustomizeUltraGridHelper.GetMaxRowFromCol(Grid, ColFromImage);
                int LastMinIndex = CustomizeUltraGridHelper.GetMinRowFromCol(Grid, ColFromImage);
                CustomizeUltraGridHelper.SetStar(Grid, ColFromImage, LastMinIndex, "~/images/starYellowBB.png", "Наименьшее отклонение");
                CustomizeUltraGridHelper.SetStar(Grid, ColFromImage, LastMaxIndex, "~/images/starGrayBB.png", "Наибольшее отклонение");
            }
        }

        private static class ParserDataUniqName
        {
            private static string[] Split(string UniqName)
            {
                string[] Separators = { "].[" };
                string[] AllLevel = UniqName.Split(Separators, StringSplitOptions.None);
                return AllLevel;
            }

            private static string GetLevelFromIndex(string UniqName, int index)
            {
                //индекс отчитывается с конца
                string[] AllLevel = Split(UniqName);
                int CountLevel = AllLevel.Length;
                string Level = AllLevel[CountLevel - index];
                return Level;
            }

            public static string GetDouwnLevel(string UniqName)
            {
                string DownLevel = GetLevelFromIndex(UniqName, 1);
                DownLevel = DownLevel.Replace("]", "");
                return DownLevel;
            }

            public static string GetParentLevel(string UniqName, int LevelParent)
            {
                string ParentLevel = GetLevelFromIndex(UniqName, LevelParent);
                return ParentLevel;
            }
        }

        private static class MounthHelper
        {
            public static string GetDateDativeCase(string Mounth)
            {
                Mounth = Mounth.ToLower();

                if (Mounth == "январь")
                {
                    return "Январю";
                }
                if (Mounth == "февраль")
                {
                    return "февралю";
                }
                if (Mounth == "март")
                {
                    return "Марту";
                }
                if (Mounth == "апрель")
                {
                    return "Апрелю";
                }
                if (Mounth == "май")
                {
                    return "Маю";
                }
                if (Mounth == "июнь")
                {
                    return "Июню";
                }
                if (Mounth == "июль")
                {
                    return "Июлю";
                }
                if (Mounth == "август")
                {
                    return "Августу";
                }
                if (Mounth == "Сентябрь")
                {
                    return "Сентябрю";
                }
                if (Mounth == "Октябрь")
                {
                    return "Октябрю";
                }
                if (Mounth == "Ноябрь")
                {
                    return "Ноябрю";
                }
                if (Mounth == "Декарь")
                {
                    return "Декабрю";
                }
                throw new Exception("Неизвестный месяц");
            }
        }

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
                string Key = BaseKey;

                while (DataForCombo.ContainsKey(Key))
                {
                    Key += " ";
                }

                return Key;
            }

            protected void AddItem(string Child, string Parent, int level,string UniqueName)
            {
                string RealChild = Child;

                DataForCombo.Add(RealChild, level);

                DataUniqeName.Add(RealChild, UniqueName);

                this.LastAdededKey = RealChild;
            }

            public abstract void LoadData(DataTable Table);
        }        

        class DataSetComboTwoLevel : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LastParent = "";
                foreach(DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();

                    string Display = row[0].ToString();

                    int level = int.Parse(row["level"].ToString());

                    Display = this.GetAlowableAndFormatedKey(Display, "");

                    this.AddItem(Display, "", level ,UniqueName);

                    this.addOtherInfo(Table, row, Display);
                }
            }
        }
           
        private void FillCombo()
        {
            DataTable TableCombo = DataBaseHelper.ExecQueryByID("ComboProgram");
            try
            {
                TableCombo.Rows[0].Delete();
                TableCombo.Rows[0].Delete();
            }
            catch { }

            DataComboQuart.LoadData(TableCombo);

            ComboPeriod.Width = 800;            


            ComboPeriod.Title = "Программа";

            if (!Page.IsPostBack)
            {
                ComboPeriod.FillDictionaryValues(DataComboQuart.DataForCombo);
                ComboPeriod.SelectLastNode();
            }
            
        }

        private void ChoseQueryParam()
        {
            ChosenDate.Value = DataComboQuart.DataUniqeName[ComboPeriod.SelectedValue];
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            if (ICustomizerSize.GetScreenResolution(ScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }

            if (ICustomizerSize.GetScreenResolution(ScreenWidth) == ICustomizerSize.ScreenResolution._800x600)
            {
                CustomizerSize = new CustomizerSize_800x600(ICustomizerSize.GetBrouse(BN));
            }
            else
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }

            Grid.Width = CustomizerSize.GetGridWidth();
            Grid.Height = Unit.Empty;
            UltraChart.Width = CustomizerSize.GetChartWidth() / 4;
            UltraChart0.Width = CustomizerSize.GetChartWidth() / 4;

            ReportExcelExporter1.ExcelExportButton.Visible = false;

            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart_InvalidDataReceived);
            UltraChart0.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart_InvalidDataReceived);

        }

        void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }

        Dictionary<string, decimal> LevelFeild = new Dictionary<string, decimal>();

        private void DataBindGrid()
        {
            Code.Value = DataComboQuart.OtherInfo[ComboPeriod.SelectedValue]["Код"].Replace("!","");

            Code.Value = DataBaseHelper.ExecQueryByID("UnameProgram").Rows[0]["uniqueName"].ToString();
            //[Программы__Реестр программ].[Программы__Реестр програм].[Данные всех источников].[ЭО\0005 Программы - вариант: 2010, ДЦП, месяц: 12, территория: Ненецкий автономный округ].[ДЦП "Сохранение и развитие коренных малочисленных народов Севера в НАО на 2009-2010 годы"]
            DataTable TableGrid = DataBaseHelper.ExecQueryByID("Grid", "Field");

            foreach (DataRow r in TableGrid.Rows)
            {
                try
                {
                    LevelFeild.Add(r["Field"].ToString(), decimal.Parse(r["level"].ToString()));
                }
                catch { } 
            }

            TableGrid.Columns.Remove("level");

            Grid.DataSource = TableGrid;
            Grid.DataBind();
        }

        protected void SetDefaultStyleHeader(ColumnHeader header,string Caption)
        {
            GridItemStyle HeaderStyle = header.Style;

            HeaderStyle.Wrap = true;

            HeaderStyle.VerticalAlign = VerticalAlign.Middle;

            HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            header.Caption = Caption;
        }

        private void ConfGrid()
        {
            SetDefaultStyleHeader(Grid.Columns.FromKey("Field").Header, "Наименование программы");
            SetDefaultStyleHeader(Grid.Columns.FromKey("plan").Header, "План на год");
            SetDefaultStyleHeader(Grid.Columns.FromKey("ispol").Header, "Исполнено с начала года");
            SetDefaultStyleHeader(Grid.Columns.FromKey("osvo").Header, "Освоено с начала года");
            SetDefaultStyleHeader(Grid.Columns.FromKey("ispol%").Header, "% исполнения от плана");
            SetDefaultStyleHeader(Grid.Columns.FromKey("osvo%").Header, "% освоения от плана");
        }

        GridHeaderLayout HeaderLayout;
        private void ConfCol()
        {
            HeaderLayout = new GridHeaderLayout(Grid);

            HeaderLayout.AddCell("Шапка");
            GridHeaderCell cell = HeaderLayout.AddCell("План финансирования на год, тыс. руб.");
            cell.AddCell("Всего");
            cell.AddCell("Бюджет субъекта");
            cell.AddCell("Бюджет МО");

            cell = HeaderLayout.AddCell("Выделенное финансирование, с начала года, тыс.руб.");
            cell.AddCell("Всего");
            cell.AddCell("Бюджет субъекта");
            cell.AddCell("Бюджет МО");

            cell = HeaderLayout.AddCell("Освоено средств с начала года, тыс.руб.");
            cell.AddCell("Всего");
            cell.AddCell("Бюджет субъекта");
            cell.AddCell("Бюджет МО");
            //HeaderLayout.
            HeaderLayout.ApplyHeaderInfo(); 

            
            foreach (HeaderBase he in Grid.Bands[0].HeaderLayout)
            {
                he.Style.HorizontalAlign = HorizontalAlign.Center;
            }

            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.CellStyle.Wrap = true;

                if (col.BaseColumnName != "Field")
                {
                    CRHelper.FormatNumberColumn(col, "### ### ##0.##");
                }
            }

            foreach (HeaderBase hb in Grid.Bands[0].HeaderLayout)
            {
                hb.Style.Wrap = true;
            }
                

        }

        private void FormatGridRow(UltraGridRow Row)
        {   
            decimal LevelRow = LevelFeild[Row.Cells.FromKey("Field").Text];
            Row.Cells.FromKey("Field").Style.Padding.Left = (int)LevelRow * 10;
            
            if (LevelRow == 0)
            {
                Row.Cells.FromKey("Field").Style.Font.Bold = true;
            }
        }

        private void ConfRow()
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                FormatGridRow(Row);
            }
        }

        private void GenerationGrid()
        {
            DataBindGrid();
            ConfCol();
            ConfRow();
            CustomizerSize.ContfigurationGrid(Grid);

            
        }

        void DataBindChart1()
        {
            UltraChart0.Border.Color = Color.Transparent;

            UltraChart0.Axis.X.Labels.Visible = false;
            UltraChart0.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart0.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart.ColumnChart.ColumnSpacing = 0;
            UltraChart.ColumnChart.SeriesSpacing = 0;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart0.ColumnChart.ColumnSpacing = 0;
            UltraChart0.ColumnChart.SeriesSpacing = 0;
            UltraChart0.ColorModel.ModelStyle = ColorModels.PureRandom;

            string s = GetProgramPropertyNoBr("Объем и источники финансирования");
            if (s.Contains("в том числе:"))
            {
                string ValueYearString = s.Split(new string[1] {"в том числе:"},StringSplitOptions.None)[1];
                string[] YearValueArr = ValueYearString.Split(';');

                DataTable TableChart = new DataTable();

                TableChart.Columns.Add("Year");
                TableChart.Columns.Add("Value",typeof(decimal));

                foreach (string YearVal in YearValueArr)
                {
                    string[] parsstr = new string[2] { "-", "–" };
                    string Year = YearVal.Split(parsstr,StringSplitOptions.None)[0];
                    String.Format(YearVal);
                    string Value = YearVal.Split(parsstr, StringSplitOptions.None)[1].Replace("тыс.руб.", "").Replace(" ", "");

                    DataRow Row = TableChart.NewRow();
                    TableChart.Rows.Add(Row);
                    
                    Row["Value"] = decimal.Parse(Value.Replace("тыс.руб.", ""));
                    Row["Year"] = Year;
                }
                UltraChart0.DataSource = TableChart;
                UltraChart0.DataBind();
            }

            UltraChart0.Axis.X.Extent = 50;
            UltraChart0.Axis.Y.Extent = 50;
            

        }

        string GetProgramPropertyNoBr(string p)
        {
            return DataComboQuart.OtherInfo[ComboPeriod.SelectedValue][p];
        }

        string GetProgramProperty(string p)
        {
           return DataComboQuart.OtherInfo[ComboPeriod.SelectedValue][p].Replace(";","<br>");
        }

        private void DataBindChart0()
        {
            Code.Value = GetProgramPropertyNoBr("Код").Replace("!", "");
            
            UltraChart.Border.Color = Color.Transparent;

            UltraChart.ChartType = ChartType.PieChart;

            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 30;
            UltraChart.Legend.Visible = true;

            DataTable TableChart = DataBaseHelper.ExecQueryByID("Lepochka");
            TableChart.Rows.Remove(TableChart.Rows[0]);

            UltraChart.DataSource = TableChart;
            UltraChart.DataBind();

            UltraChart.ColumnChart.ColumnSpacing = 0;
            UltraChart.ColumnChart.SeriesSpacing = 0;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
        }

        private void GenGanja()
        {
            Code.Value = GetProgramPropertyNoBr("Код").Replace("!", "");
            DataTable TableChart = DataBaseHelper.ExecQueryByID("gauj");

            Infragistics.UltraGauge.Resources.RadialGaugeScale scale = ((Infragistics.UltraGauge.Resources.RadialGauge)UltraGauge1.Gauges[0]).Scales[0];
            scale.Markers[0].Value = (decimal)TableChart.Rows[0][1] * 100;

            scale = ((Infragistics.UltraGauge.Resources.RadialGauge)UltraGauge2.Gauges[0]).Scales[0];
            scale.Markers[0].Value = (decimal)TableChart.Rows[1][1] * 100;
        }
        
        string InPanel(string content)
        {
            return string.Format("<table style=\"width: 100%; border-collapse: collapse; background-color: white; height: 100%;\"><tr><td class=\"topleft\"></td><td class=\"top\"></td><td class=\"topright\"></td></tr><tr><td class=\"left\"></td><td style=\"vertical-align: top; \">{0}</td><td class=\"right\"></td></tr><tr><td class=\"bottomleft\"></td><td class=\"bottom\"></td><td class=\"bottomright\"></td></tr></table>", content);
        }

        private void GenearateTextovka()
        {
            textovka.Text = "";

            try
            {
                textovka.Text = 
      string.Format(InPanel(@"<b>Основание для разработки</b>: <br>" + GetProgramProperty("Основание для разработки программы")) +
                    InPanel("<b>Ожидаемый результат:</b> <br>" + GetProgramProperty("Ожидаемый результат реализации")) +
                    InPanel("<b>Контроль за исполнением:</b> <br>" + GetProgramProperty("Система реализации контроля за исполнением")) +
                    InPanel("<b>ЦЕЛИ ПРОГРАММЫ</b> <br>" + GetProgramProperty("Цель программы")) +
                    InPanel("<b>ЗАДАЧИ ПРОГРАММЫ</b> <br>" + GetProgramProperty("Задачи программы")) +
                    InPanel("<b>МЕРОПРИЯТИЯ ПРОГРАММЫ</b> <br>" + GetProgramProperty("Перечень основных мероприятий")));

            }
            catch { }
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();
            ChoseQueryParam();
            GenerationGrid();
            GenearateTextovka();
            DataBindChart1();
            DataBindChart0();
            GenGanja();

            Hederglobal.Text = "Отчет об исполнении целевых программ Ненецкого автономного округа";
            PageSubTitle.Text = "";
            
            Page.Title = Hederglobal.Text;   

            LabelChart.Text = "План финансирования, тыс.руб.";
            LabelChart1.Text = "Источники финансирования, тыс.руб.";
            LabelChart2.Text = "Исполнено, в % от плана";
            LabelChart3.Text = "Освоено, в % от плана";
                
            ComboPeriod.Width = 300;

            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            #endregion


        }       

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Hederglobal.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            headerLayout.AddCell("Наименование программы");
            headerLayout.AddCell("План на год");
            headerLayout.AddCell("Исполнено с начала года");
            headerLayout.AddCell("Осовоено с начало года");
            headerLayout.AddCell("% исполнения от плана");
            headerLayout.AddCell("% освоения от плана");

            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }
        /*  
        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraWebGrid grid = headerLayout.Grid;

            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = (PageSubTitle.Text + "\n\n" +
                Regex.Replace(LabelDynamicText.Text.Replace("<br/>", "\n"), "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", String.Empty) + "\n").Replace("<b>", "").Replace("</b>", "");

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.HeaderCellHeight = 70;

            foreach (UltraGridRow row in headerLayout.Grid.Rows)
            {
                if (row.Index % 3 != 0)
                {
                    row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
                    row.Cells[1].Style.BorderDetails.StyleTop = BorderStyle.None;
                }
                else
                {
                    row.Cells[0].Value = null;
                    row.Cells[1].Value = null;
                }
                if (row.Index % 3 != 2)
                {
                    row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    row.Cells[1].Style.BorderDetails.StyleBottom = BorderStyle.None;
                }
                else
                {
                    row.Cells[0].Value = null;
                    row.Cells[1].Value = null;
                }
            }

            headerLayout.childCells.Remove(headerLayout.GetChildCellByCaption("MDX имя"));
            grid.Columns.Remove(grid.Columns.FromKey("Уникальное имя"));

            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
        }

        #endregion*/
    }
}
    