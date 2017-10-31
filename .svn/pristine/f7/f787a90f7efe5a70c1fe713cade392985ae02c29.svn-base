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


namespace Krista.FM.Server.Dashboards.reports.EO_HAO_001
{
    public partial class _default : CustomReportPage
    {
        ICustomizerSize CustomizerSize;

        CustomParam ChosenDate;

        IDataSetCombo DataComboQuart = new DataSetComboTwoLevel();

        int ScreenWidth { get { return ((int)Session[CustomReportConst.ScreenWidthKeyName]); } }
        int ScreenHeight { get { return ((int)Session[CustomReportConst.ScreenHeightKeyName]); } }

        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;
        private bool onWall;
        private bool blackStyle;

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
                this.ContfigurationGridIE(Grid);
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
                Grid.Columns.FromKey("Field").Width = onePercent * 40;
                Grid.Columns.FromKey("plan").Width = onePercent * 9;
                Grid.Columns.FromKey("osvo").Width = onePercent * 9;
                Grid.Columns.FromKey("ispol").Width = onePercent * 10;
                Grid.Columns.FromKey("osvo%").Width = onePercent * 15;
                Grid.Columns.FromKey("ispol%").Width = onePercent * 15;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;
                int onePercent = (int)Grid.Width.Value / 100;
                Grid.Columns.FromKey("Field").Width = onePercent * 40;
                Grid.Columns.FromKey("plan").Width = onePercent * 9;
                Grid.Columns.FromKey("osvo").Width = onePercent * 9;
                Grid.Columns.FromKey("ispol").Width = onePercent * 10;
                Grid.Columns.FromKey("osvo%").Width = onePercent * 15;
                Grid.Columns.FromKey("ispol%").Width = onePercent * 15;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;
                int onePercent = (int)Grid.Width.Value / 100;
                Grid.Columns.FromKey("Field").Width = onePercent * 40;
                Grid.Columns.FromKey("plan").Width = onePercent * 9;
                Grid.Columns.FromKey("osvo").Width = onePercent * 9;
                Grid.Columns.FromKey("ispol").Width = onePercent * 10;
                Grid.Columns.FromKey("osvo%").Width = onePercent * 15;
                Grid.Columns.FromKey("ispol%").Width = onePercent * 15;
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

                //row.Cells.FromKey("Shift").Text = FormatShiftValue(row.Cells.FromKey("Shift").Value);
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
                    //FormatingText(Parent, BaseKey);

                while (DataForCombo.ContainsKey(Key))
                {
                    Key += " ";
                }

                return Key;
            }

            protected void AddItem(string Child, string Parent, string UniqueName)
            {
                string RealChild = Child;

                DataForCombo.Add(RealChild, 1);

                DataUniqeName.Add(RealChild, UniqueName);

                this.LastAdededKey = RealChild;
            }

            public abstract void LoadData(DataTable Table);

        }        

        class DataSetComboTwoLevel : IDataSetCombo
        {//показать полностью
            public override void LoadData(DataTable Table)
            {
                string LastParent = "";

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();
                    string Display = row["quart"].ToString();

                    string QuartNumber = Display.Split(' ')[1];
                    

                    string Parent = row["year"].ToString();

                    Display = QuartNumber + " квартал " + Parent + " года";

                    if (LastParent != Parent)
                    {   
                        DataForCombo.Add(Parent +" год", 0);
                        LastParent = Parent;
                    }

                    Display = this.GetAlowableAndFormatedKey(Display, Parent);

                    this.AddItem(Display, Parent, UniqueName);

                    this.addOtherInfo(Table, row, Display);
                }
            }
        }

        private void FillCombo()
        {
            DataComboQuart.LoadData(DataBaseHelper.ExecQueryByID("ComboQuart"));

            ComboPeriod.Width = 400;
            ComboPeriod.Title = "Выберите период";

            if (!Page.IsPostBack)
            {
                CheckBox1.Checked = false;
                ComboPeriod.FillDictionaryValues(DataComboQuart.DataForCombo);
                ComboPeriod.SelectLastNode();
            }
        }

        private void ChoseQueryParam()
        {
            ChosenDate.Value = DataComboQuart.DataUniqeName[ComboPeriod.SelectedValue];
        }

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
            blackStyle = false;
            if (Session["blackStyle"] != null)
            {
                blackStyle = Convert.ToBoolean(((CustomParam)Session["blackStyle"]).Value);
            }

            CRHelper.SetPageTheme(this, blackStyle ? "MinfinBlackStyle" : "Minfin");
        }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            ChosenDate =UserParams.CustomParam("ChosenDate",true);

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

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

            
            Grid.Height = 400*(onWall?4:1);
            //ReportPDFExporter1.PdfExportButton.Visible = false;
            //ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            //ReportPDFExporter1.PdfExportButton.Visible = false;
            //ReportExcelExporter1.ExcelExportButton.Visible = false;
            #region Bigresolution
            heightMultiplier = onWall ? 5 : 1;
            fontSizeMultiplier = onWall ? 5 : 1;
            primitiveSizeMultiplier = onWall ? 4 : 1;
            pageWidth = onWall ? 5600 : (int)Session["width_size"];
            pageHeight = onWall ? 2100 : (int)Session["height_size"];

            Grid.Width = pageWidth - 50 * (onWall?6:1);//CustomizerSize.GetGridWidth();

            widthMultiplier = 1;
            if (Session["width_size"] != null && (int)Session["width_size"] != 0)
            {
                widthMultiplier = onWall ? 1.08 * 5600 / (int)Session["width_size"] : 1;
            }

            Color fontColor = Color.Black;

            Grid.DisplayLayout.HeaderStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            Grid.DisplayLayout.HeaderStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            Grid.DisplayLayout.HeaderStyleDefault.BorderWidth = blackStyle ? 1 : onWall ? 3 : 1;

            Grid.DisplayLayout.RowStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            Grid.DisplayLayout.RowStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            Grid.DisplayLayout.RowStyleDefault.BorderWidth = 1;

            Hederglobal.Font.Size = Convert.ToInt32(14 * fontSizeMultiplier);
            PageSubTitle.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);
            textovka.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);

            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string bestStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starYellowBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string worseStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starGrayBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            if (onWall)
            {
                ComprehensiveDiv.Style.Add("width", "5600px");
                ComprehensiveDiv.Style.Add("height", "2100px");
            }

            if (onWall && Page.Master is IMasterPage)
            {
                ((IMasterPage)Page.Master).SetHeaderVisible(false);
            }

            PopupInformer1.Visible = !onWall;
            #endregion  

            CustomParam cp = UserParams.CustomParam("test", true);


            WallLink.Text = "Для&nbsp;видеостены";
            //WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());
            WallLink.NavigateUrl = String.Format("default.aspx?paramlist=onWall=true");
            RefreshButton1.Visible = !onWall;
            ComboPeriod.Visible = !onWall;
            CheckBox1.Visible = !onWall;

            if (onWall)
            {
                CheckBox1.Checked = false;
            }

            //ReportExcelExporter1.ExcelExportButton.Visible = !onWall;
            //ReportPDFExporter1.PdfExportButton.Visible = !onWall;
            
        }

        Dictionary<string, decimal> LevelFeild = new Dictionary<string, decimal>();

        private void DataBindGrid()
        {
            DataTable TableGrid = new DataTable();

            TableGrid.Columns.Add("Field");
            TableGrid.Columns.Add("plan", typeof(decimal));
            TableGrid.Columns.Add("ispol", typeof(decimal));
            TableGrid.Columns.Add("osvo", typeof(decimal));
            TableGrid.Columns.Add("ispol%", typeof(decimal));
            TableGrid.Columns.Add("osvo%", typeof(decimal));

            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid","Field");

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                DataRow GridRow = TableGrid.NewRow();
                TableGrid.Rows.Add(GridRow);

                if (!LevelFeild.ContainsKey(BaseRow["Field"].ToString()))
                    LevelFeild.Add(BaseRow["Field"].ToString(), (decimal)BaseRow["level"]);

                GridRow["Field"] = BaseRow["Field"];
                GridRow["plan"] = BaseRow["План"];
                GridRow["ispol"] = BaseRow["Исполнено"];

                GridRow["osvo"] = BaseRow["Освоено"];

                if (GridRow["plan"] != DBNull.Value)
                {

                    if (GridRow["ispol"] != DBNull.Value)
                    {
                        GridRow["ispol%"] = (decimal)GridRow["ispol"] / (decimal)GridRow["plan"];
                    }

                    if (GridRow["osvo"] != DBNull.Value)
                    {
                        GridRow["osvo%"] = (decimal)GridRow["osvo"] / (decimal)GridRow["plan"];
                    }
                }
            }
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

        private void ConfCol()
        {
            Grid.Columns.FromKey("Field").CellStyle.Wrap = true;
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("plan"),   "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("osvo"),   "N2");
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("ispol"),  "N2");
            //CRHelper.FormatNumberColumn(Grid.Columns.FromKey("osvo%"),  "P2");
            Grid.Columns.FromKey("osvo%").Format = "P2";
            Grid.Columns.FromKey("osvo%").CellStyle.Padding.Right = 4;
            Grid.Columns.FromKey("osvo%").CellStyle.HorizontalAlign = HorizontalAlign.Right;
            Grid.Columns.FromKey("ispol%").CellStyle.HorizontalAlign = HorizontalAlign.Right;
            Grid.Columns.FromKey("ispol%").CellStyle.Padding.Right = 4;
            Grid.Columns.FromKey("ispol%").Format = "P2";
            //CRHelper.FormatNumberColumn(Grid.Columns.FromKey("ispol%"), "P2");
        }

        /// <summary>
        /// </summary>
        /// <param name="value">Значение для гаджи</param>
        /// <param name="prefix">Префих ну чёт таипа ../../../../TemporaryImages/megaGadgz_mo_mo</param>
        /// <param name="width">высота</param>
        /// <param name="height">ширина</param>
        /// <returns> prefix+ value.tostring()+.png</returns>
        protected string GenGanga(decimal value,  string prefixPage, int width, int height)
        {           
            value = Math.Round(value * 100);
            string path = prefixPage + value.ToString() + ".png";
            System.Double V1 = (System.Double)value ;
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)UltraGauge1.Gauges[0]).Scales[0];
            scale.Markers[0].Value = V1;

            Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement BrushElement =
                (Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();    

            Color col;
            if (V1 > 80)
            {
                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else
            {
                if (V1 < 50)
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                    BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                    BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
                }
                else
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
                }
            }

            UltraGauge1.DataBind();

            System.Drawing.Size size = new Size(width * (onWall ? 4 : 1), height * (onWall ? 4 : 1));
            UltraGauge1.SaveTo(Server.MapPath("~/TemporaryImages/" + path), GaugeImageType.Png, size);
            return "~/TemporaryImages/" + path;             
        }

        private void FormatGridRow(UltraGridRow Row)
        {   
            decimal LevelRow = LevelFeild[Row.Cells.FromKey("Field").Text];
            Row.Cells.FromKey("Field").Style.Padding.Left = (int)LevelRow * 10;

            if (LevelRow == 2)
            {
                Row.Cells.FromKey("Field").Style.Font.Bold = true;
            }

            if (!CheckBox1.Checked) 
            {
                if (LevelRow > 2)
                {
                    Row.Hidden = true;
                }
            }

            if (Row.Cells.FromKey("osvo%").Value !=  null)
            {
                 Row.Cells.FromKey("osvo%").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                 Row.Cells.FromKey("osvo%").Style.BackgroundImage = GenGanga((decimal)Row.Cells.FromKey("osvo%").Value, "EO_HAO_001Gadga", 130, 30);
            }

            if (Row.Cells.FromKey("ispol%").Value != null)
            {
                Row.Cells.FromKey("ispol%").Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                Row.Cells.FromKey("ispol%").Style.BackgroundImage = GenGanga((decimal)Row.Cells.FromKey("ispol%").Value, "EO_HAO_001Gadga", 130, 30);
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
            ConfGrid();
            ConfCol();
            ConfRow();

            Grid.Rows[0].Cells[0].Text = "Всего по программам";
            Grid.Rows[0].Style.Font.Bold = true;

            Grid.Rows[0].Cells.FromKey("ispol%").Style.BackgroundImage = "";
            Grid.Rows[0].Cells.FromKey("osvo%").Style.BackgroundImage = "";
            Grid.Rows[0].Cells.FromKey("ispol%").Text = "";
            Grid.Rows[0].Cells.FromKey("osvo%").Text = "";
            Grid.Rows[1].Hidden = true;
            CustomizerSize.ContfigurationGrid(Grid);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            FillCombo();
            ChoseQueryParam();

            
            GenearateTextovka();

            Hederglobal.Text = "Отчет об исполнении целевых программ Ненецкого автономного округа";
            PageSubTitle.Text = "";
            
            Page.Title = Hederglobal.Text;

            GenerationGrid();

            #region Экспорт
            //ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            #endregion

            
        }
        
        private void GenearateTextovka()
        {
            textovka.Text = "";

            DataTable TableTextovka = DataBaseHelper.ExecQueryByID("Textovka");

            textovka.Text = string.Format(@"По состоянию на <b>{0}</b>  в <b>{1}</b> <br>
•	Объем финансирования целевых программ составил: <b>{2:N2}</b>  тыс.руб.  (<b>{3:P2}</b> % от запланированного объема)<br>
•	При выполнении мероприятий целевых программ освоено: <b>{4:N2}</b>  тыс.руб.  (<b>{5:P2}</b> %  от выделенных средств)",
                                                                                       ComboPeriod.SelectedValue,
                                                                                       "Ненецком автономном округе",
                                                                                       TableTextovka.Rows[0][1],
                                                                                       TableTextovka.Rows[1][1],
                                                                                       TableTextovka.Rows[2][1],
                                                                                       TableTextovka.Rows[3][1]);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            //ReportExcelExporter1.WorksheetTitle = Hederglobal.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            headerLayout.AddCell("Наименование программы");
            headerLayout.AddCell("План на год");
            headerLayout.AddCell("Исполнено с начала года");
            headerLayout.AddCell("Осовоено с начало года");
            headerLayout.AddCell("% исполнения от плана");
            headerLayout.AddCell("% освоения от плана");    

            //ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            //ReportExcelExporter1.WorksheetTitle = String.Empty;
            //ReportExcelExporter1.WorksheetSubTitle = String.Empty;
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

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {

            GridHeaderLayout headerLayout = new GridHeaderLayout(Grid);

            headerLayout.AddCell("Наименование программы");
            headerLayout.AddCell("План на год");
            headerLayout.AddCell("Исполнено с начала года");
            headerLayout.AddCell("Осовоено с начало года");
            headerLayout.AddCell("% исполнения от плана");
            headerLayout.AddCell("% освоения от плана");    

            UltraWebGrid grid = headerLayout.Grid;

            //ReportPDFExporter1.PageTitle = Hederglobal.Text;
            //ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
                //(PageSubTitle.Text + "\n\n" +
                //Regex.Replace(LabelDynamicText.Text.Replace("<br/>", "\n"), "<[\\s\\S]*?>", String.Empty).Replace("&nbsp;", String.Empty) + "\n").Replace("<b>", "").Replace("</b>", "");

            Report report = new Report();
            ISection section1 = report.AddSection();
            //ISection section2 = report.AddSection();

            ////UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            //ReportPDFExporter1.HeaderCellHeight = 70;

            //foreach (UltraGridRow row in headerLayout.Grid.Rows)
            //{
            //    if (row.Index % 3 != 0)
            //    {
            //        row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
            //        row.Cells[1].Style.BorderDetails.StyleTop = BorderStyle.None;
            //    }
            //    else
            //    {
            //        row.Cells[0].Value = null;
            //        row.Cells[1].Value = null;
            //    }
            //    if (row.Index % 3 != 2)
            //    {
            //        row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
            //        row.Cells[1].Style.BorderDetails.StyleBottom = BorderStyle.None;
            //    }
            //    else
            //    {
            //        row.Cells[0].Value = null;
            //        row.Cells[1].Value = null;
            //    }
            //}

            //headerLayout.childCells.Remove(headerLayout.GetChildCellByCaption("MDX имя"));
            //grid.Columns.Remove(grid.Columns.FromKey("Уникальное имя"));

            //ReportPDFExporter1.Export(headerLayout, section1);
            //ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
        }

        #endregion
    }
}
