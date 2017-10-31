using System;

using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
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


namespace Krista.FM.Server.Dashboards.reports.SEP_001_HMAO
{
    public partial class _default : CustomReportPage
    {
        GridHeaderLayout headerLayout;
        
        int ScreenWidth { get { return ((int)Session[CustomReportConst.ScreenWidthKeyName]); } }
        int ScreenHeight { get { return ((int)Session[CustomReportConst.ScreenHeightKeyName]); } }

        IDataSetCombo DataComboRegion = new DataSetComboLinear();
        IDataSetCombo DataComboPeriod = new DataSetComboLinear();//new DataSetComboTwoLevel();
        IDataSetCombo DataComboDirection = new DataSetComboTwoLevel();

        DataTable TableGrid = new DataTable();
        DataTable TableChart = new DataTable();

        ICustomizerSize CustomizerSize = null;

        CustomParam ChosenDate { get { return (UserParams.CustomParam("ChosenDate")); } }
        CustomParam ChosenRegion { get { return (UserParams.CustomParam("ChosenRegion")); } }
        CustomParam ChosenSEP { get { return (UserParams.CustomParam("ChosenSEP")); } }
        CustomParam RegionBaseDimention { get { return (UserParams.CustomParam("RegionBaseDimention")); } }

        string CurrentUnit = "Рубль";
        bool CurentInverce = false;
        bool CurrentComparable = false;

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
                if (Width < 901)
                {
                    return ScreenResolution._800x600;
                }
                if (Width < 1281)
                {
                    return ScreenResolution._1280x1024;
                }
                return ScreenResolution._1280x1024;
            }
        }

        class CustomizerSize_800x600 : ICustomizerSize
        {
            public override int GetGridWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 730;
                    case BrouseName.FireFox:
                        return 750;
                    case BrouseName.SafariOrHrome:
                        return 750;
                    default:
                        return 800;
                }
            }

            public override int GetChartWidth()
            {
                switch (this.NameBrouse)
                {
                    case BrouseName.IE:
                        return 730;
                    case BrouseName.FireFox:
                        return 730;
                    case BrouseName.SafariOrHrome:
                        return 730;
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

                ColsGrid.FromKey("Region").Width = OnePercent * 20;

                ColsGrid.FromKey("Value").Width = OnePercent * 10;

                ColsGrid.FromKey("Growth").Width = OnePercent * 11;

                ColsGrid.FromKey("SpeedGrowth").Width = OnePercent * 10;

                ColsGrid.FromKey("DeviationFO").Width = OnePercent * 8;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 6;

                ColsGrid.FromKey("DeviationRF").Width = OnePercent * 8;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 6;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)1200 - 6) / 100;

                ColsGrid.FromKey("Region").Width = OnePercent * 20;

                ColsGrid.FromKey("Value").Width = OnePercent * 10;

                ColsGrid.FromKey("Growth").Width = OnePercent * 11;

                ColsGrid.FromKey("SpeedGrowth").Width = OnePercent * 10;

                ColsGrid.FromKey("DeviationFO").Width = OnePercent * 8;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 6;

                ColsGrid.FromKey("DeviationRF").Width = OnePercent * 8;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 6;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)1200 - 6) / 100;

                ColsGrid.FromKey("Region").Width = OnePercent * 20;

                ColsGrid.FromKey("Value").Width = OnePercent * 10;

                ColsGrid.FromKey("Growth").Width = OnePercent * 11;

                ColsGrid.FromKey("SpeedGrowth").Width = OnePercent * 10;

                ColsGrid.FromKey("DeviationFO").Width = OnePercent * 8;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 6;

                ColsGrid.FromKey("DeviationRF").Width = OnePercent * 8;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 6;

            }

            public override int GetChartLegendWidth(bool Dynamic)
            {
                return Dynamic ? 1120 : 850;
            }

            #endregion

            public override int GetComboPeriodWidth()
            {
                return 120;
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

                int OnePercent = ((int)Grid.Width.Value - 6) / 100;

                ColsGrid.FromKey("Region").Width = OnePercent * 35;

                ColsGrid.FromKey("Value").Width = OnePercent * 10;

                ColsGrid.FromKey("Growth").Width = OnePercent * 11;

                ColsGrid.FromKey("SpeedGrowth").Width = OnePercent * 10;

                ColsGrid.FromKey("DeviationFO").Width = OnePercent * 8;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 6;

                ColsGrid.FromKey("DeviationRF").Width = OnePercent * 8;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 6;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)Grid.Width.Value - 6) / 100;

                ColsGrid.FromKey("Region").Width = OnePercent * 36;

                ColsGrid.FromKey("Value").Width = OnePercent * 10;

                ColsGrid.FromKey("Growth").Width = OnePercent * 11;

                ColsGrid.FromKey("SpeedGrowth").Width = OnePercent * 10;

                ColsGrid.FromKey("DeviationFO").Width = OnePercent * 8;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 6;

                ColsGrid.FromKey("DeviationRF").Width = OnePercent * 8;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 6;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)Grid.Width.Value - 6) / 100;

                ColsGrid.FromKey("Region").Width = OnePercent * 39;

                ColsGrid.FromKey("Value").Width = OnePercent * 10;

                ColsGrid.FromKey("Growth").Width = OnePercent * 11;

                ColsGrid.FromKey("SpeedGrowth").Width = OnePercent * 10;

                ColsGrid.FromKey("DeviationFO").Width = OnePercent * 8;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 6;

                ColsGrid.FromKey("DeviationRF").Width = OnePercent * 8;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 6;

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
                return 550;
            }

            public override int GetComboRegonWidth()
            {
                return 280;
            }
        }
        #endregion

        static class DataBaseHelper
        {
            static DataProvider ActiveDataPorvider = DataProvidersFactory.SecondaryMASDataProvider;

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

        private static class DataTableHelper
        {
            public static void ClearEmtyRow(DataTable Table, string ColName)
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    DataRow row = Table.Rows[i];
                    if (row[ColName] == DBNull.Value)
                    {
                        row.Delete();
                        i--;
                    }
                }
            }


            public static void FilterTable(DataTable Table, int StartRow, string FilterableColumn, string value, bool exactly)
            {
                for (int i = StartRow; i < Table.Rows.Count; i++)
                {
                    DataRow Row = Table.Rows[i];

                    if (((Row[FilterableColumn].ToString() == value) & !exactly) ||
                        ((Row[FilterableColumn].ToString() != value) & exactly))
                    {
                        Row.Delete();
                        i--;
                    }
                }
            }
            public static void FilterTable2(DataTable Table, int StartRow, string FilterableColumn, string value, bool exactly)
            {
                for (int i = StartRow; i < Table.Rows.Count; i++)
                {
                    DataRow Row = Table.Rows[i];

                    if (((Row[FilterableColumn].ToString().IndexOf(value)>0) & !exactly) ||
                        ((!(Row[FilterableColumn].ToString().IndexOf(value)>0)) & exactly))
                    {
                        Row.Delete();
                        i--;
                    }
                }
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
                            if (revrce)
                            {
                                if (((double)(Table.Rows[j][ColumnValueName]) <= (double)(Table.Rows[IndexMax][ColumnValueName])) && (Table.Rows[j][ColumnRangName] == DBNull.Value))
                                {
                                    IndexMax = j;
                                }
                                if (((double)(Table.Rows[j][ColumnValueName]) > (double)(Table.Rows[IndexMin][ColumnValueName])) && (Table.Rows[j][ColumnRangName] == DBNull.Value))
                                {
                                    IndexMin = j;
                                }
                            }
                            else
                            {
                                if (((double)(Table.Rows[j][ColumnValueName]) >= (double)(Table.Rows[IndexMax][ColumnValueName])) && (Table.Rows[j][ColumnRangName] == DBNull.Value))
                                {
                                    IndexMax = j;
                                }
                                if (((double)(Table.Rows[j][ColumnValueName]) < (double)(Table.Rows[IndexMin][ColumnValueName])) && (Table.Rows[j][ColumnRangName] == DBNull.Value))
                                {
                                    IndexMin = j;
                                }
                            }
                        }
                    }
                    if (Table.Rows[IndexMin][ColumnValueName] != System.DBNull.Value)
                    {
                        if (Table.Rows[IndexMin][ColumnRangName] == DBNull.Value)
                        {
                            Table.Rows[IndexMin][ColumnRangName] = ++rang;
                            for(int k =StartRow;k<RowCount;k++)
                            {
                                if (Table.Rows[k][ColumnValueName] != DBNull.Value)
                                if ((double)Table.Rows[k][ColumnValueName] == (double)Table.Rows[IndexMin][ColumnValueName])
                                {
                                    Table.Rows[k][ColumnRangName] = rang;
                                }
                            }
                        }
                    }
                    IndexMin = IndexMax;
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
                if (o1 is string)
                {
                    return string.Compare(o1.ToString(), o2.ToString()) > 0;
                }
                if (o1 is double)
                {
                    return ((double)(o1) > (double)(o2));
                }
                if (o1 is decimal)
                {
                    return ((double)(o1) > (double)(o2));
                }

                throw new Exception("Непредусмотренный тип");

            }

            public static DataTable SortingDataTable(DataTable BaseTable, string ColSort, int StartRow)
            {
                if (BaseTable.Rows.Count < 1)
                {
                    return BaseTable;
                }
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
                        G.Rows[i].Cells[Col].Style.BackgroundImage = Star;
                        G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                }


            }

            public static void SetImageFromGrowth(UltraGridRow row, string ColumnGrowthName, bool inverce)
            {
                if ((row.Cells.FromKey(ColumnGrowthName).Value == null)
                    || ((double)row.Cells.FromKey(ColumnGrowthName).Value == 1))
                {
                    //Неставим картинки для пустых, и 100%ых
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

            public static int? FindIndexRowFromValue(UltraWebGrid Grid, string FinderCol, string Value)
            {
                foreach (UltraGridRow Row in Grid.Rows)
                {
                    if (Row.Cells.FromKey(FinderCol).Value != null)
                        if (Row.Cells.FromKey(FinderCol).Value.ToString() == Value)
                        {
                            return Row.Index;
                        }
                }

                return null;
            }

            public static void SetStarGrayFromValue(UltraWebGrid Grid, int ColFromImage, bool reverce, string Value)
            {
                int? IndexRow = CustomizeUltraGridHelper.FindIndexRowFromValue(Grid, Grid.Columns[ColFromImage].Key, Value);
                if (IndexRow != null)
                {
                    CustomizeUltraGridHelper.SetStar(Grid, ColFromImage, (int)IndexRow, "~/images/starGrayBB.png",
                        reverce ? "Наименьшее отклонение" : "Наибольшее отклонение"); 
                }
            }

            public static void SetStarYellowFromValue(UltraWebGrid Grid, int ColFromImage, bool reverce, string Value)
            {
                int? IndexRow = CustomizeUltraGridHelper.FindIndexRowFromValue(Grid, Grid.Columns[ColFromImage].Key, Value);
                if (IndexRow != null)
                {
                    CustomizeUltraGridHelper.SetStar(Grid, ColFromImage, (int)IndexRow, "~/images/starYellowBB.png", reverce ? "Наибольшее отклонение" : "Наименьшее отклонение");
                }
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
                if (Mounth == "сентябрь")
                {
                    return "Сентябрю";
                }
                if (Mounth == "октябрь")
                {
                    return "Октябрю";
                }
                if (Mounth == "ноябрь")
                {
                    return "Ноябрю";
                }
                if (Mounth == "декабрь")
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

            public string FirstAdededKey = "";

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
                string Key = FormatingText(Parent, BaseKey);

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
                this.FirstAdededKey = string.IsNullOrEmpty(this.FirstAdededKey) ? RealChild : this.FirstAdededKey;
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
                    string Child = ParserDataUniqName.GetDouwnLevel(UniqueName);
                    this.LastAdededKey = Child;
                    DataForCombo.Add(Child, 0);

                    DataUniqeName.Add(Child, UniqueName);
                }
            }
        }

        class DataSetComboTwoLevel : IDataSetCombo
        {
            public override void LoadData(DataTable Table)
            {
                string LastParent = "";

                foreach (DataRow row in Table.Rows)
                {
                    string UniqueName = row["UniqueName"].ToString();
                    string Child = ParserDataUniqName.GetDouwnLevel(UniqueName);
                    string Parent = ParserDataUniqName.GetParentLevel(UniqueName, this.LevelParent);

                    if (LastParent != Parent)
                    {
                        string Parent2 = this.FormatingTextParent(Parent);
                        DataForCombo.Add(Parent2, 0);
                        LastParent = Parent;
                    }

                    Child = this.GetAlowableAndFormatedKey(Child, Parent);

                    this.AddItem(Child, Parent, UniqueName);

                    this.addOtherInfo(Table, row, Child);
                }
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.EndExport +=new EndExportEventHandler(ExcelExporter_EndExport);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.ExcelExportButton.Visible = false;
            
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }else

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

            Chart.Width = CustomizerSize.GetChartWidth();

            DundasMap.Width = CustomizerSize.GetChartWidth();

            DundasMap.Height = 700;
        }

        #region ConfigurationMultiCombo
        private void SetSettingMultiCombo(CustomMultiCombo Combo)
        {
            int WidthDefault = 300;
            Combo.ParentSelect = false;
            Combo.MultiSelect = false;
            Combo.Width = WidthDefault;
        }

        private void FillDataCombo(CustomMultiCombo Combo, string QueryID, IDataSetCombo dataSetCombo)
        {
            DataTable Table = DataBaseHelper.ExecQueryByID(QueryID);

            DataTableHelper.FilterTable2(Table, 0, "UniqueName", "ОКВЭД", false);
            dataSetCombo.LoadData(Table);

            if (!Page.IsPostBack)
            {
                Combo.FillDictionaryValues(dataSetCombo.DataForCombo);
            }
        }

        private string GetDefaultDirection()
        {
            ChosenDate.Value = DataComboPeriod.DataUniqeName[ComboPeriod.SelectedValue];
            return DataBaseHelper.ExecQueryByID("DefaultParam").Rows[0][0].ToString();
        }

        private void ConfigurationMultiCombo()
        {
            RegionBaseDimention.Value = RegionSettingsHelper.Instance.RegionBaseDimension.Replace("[Территории__РФ].[Территории__РФ]","[Территории__СЭП].[Территории__СЭП]");

            if (!Page.IsPostBack)
            {
                SetSettingMultiCombo(ComboDirection);
                ComboDirection.Title = "Показатель";
                ComboDirection.Width = CustomizerSize.GetComboDirectionWidth();
                 
                SetSettingMultiCombo(ComboPeriod);
                ComboPeriod.Title = "Год";
                ComboPeriod.Width = CustomizerSize.GetComboPeriodWidth();


                SetSettingMultiCombo(ComboRegion);
                ComboRegion.Title = "ФО";
                ComboRegion.Width = CustomizerSize.GetComboRegonWidth();
            }

            DataComboDirection.SetFormatterText((parent, children) => { return children; });
            DataComboDirection.SetFormatterTextParent((Parent) => { return Parent; });
            FillDataCombo(ComboDirection, "Direction", DataComboDirection);


            DataComboPeriod.SetFormatterText((parent, children) => { return children + " " + parent + " года"; });
            //DataComboPeriod.SetFormatterTextParent((parent) => { return parent + " год"; });
            DataComboPeriod.LevelParent = 4;
            FillDataCombo(ComboPeriod, "Period", DataComboPeriod);


            DataComboDirection.SetFormatterText((parent, children) => { return children; });
            DataComboDirection.SetFormatterTextParent((Parent) => { return Parent; });
            FillDataCombo(ComboRegion, "Region", DataComboRegion);
            
            if (!Page.IsPostBack)
            {                
                ComboPeriod.SetСheckedState(DataComboPeriod.LastAdededKey, true);
                ComboRegion.SetСheckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                //ComboRegion.SetСheckedState(RegionSettingsHelper.Instance.Name, true);
                try
                {
                    string DefaultDirection = GetDefaultDirection();
                    ComboDirection.SetСheckedState(DefaultDirection, true);
                }
                catch { }
            }
        }



        private void ChosenParam()
        {
            string SelectedDate = ComboPeriod.SelectedValue;
            ChosenDate.Value = DataComboPeriod.DataUniqeName[SelectedDate];

            string SelectedRegion = ComboRegion.SelectedValue;
            ChosenRegion.Value = SelectedRegion;

            string SelectedSEP = ComboDirection.SelectedValue;
            ChosenSEP.Value = DataComboDirection.DataUniqeName[SelectedSEP];


            CurentInverce = DataComboDirection.OtherInfo[SelectedSEP]["reverce"].ToLower() == "0";

            CurrentUnit = DataComboDirection.OtherInfo[SelectedSEP]["Unit"];

            CurrentComparable = DataComboDirection.OtherInfo[SelectedSEP]["comparable"] == "1";
        }

        #endregion

        #region DataBindGrid

        protected void GenerationColumnFromGrid()
        {
            TableGrid.Columns.Add("ShortName", typeof(string));

            TableGrid.Columns.Add("accessory", typeof(string));

            TableGrid.Columns.Add("Region", typeof(string));

            TableGrid.Columns.Add("PrevValue", typeof(double));

            TableGrid.Columns.Add("Value", typeof(double));

            TableGrid.Columns.Add("Growth", typeof(double));

            TableGrid.Columns.Add("SpeedGrowth", typeof(double));

            TableGrid.Columns.Add("DeviationFO", typeof(double));

            TableGrid.Columns.Add("RangFO", typeof(double));

            TableGrid.Columns.Add("DeviationRF", typeof(double));

            TableGrid.Columns.Add("RangRF", typeof(double));
        }

        private void SetGrowth(DataRow NewRow)
        {
            if ((NewRow["Value"] != DBNull.Value) && (NewRow["PrevValue"] != DBNull.Value))
            {                
                NewRow["Growth"] = (double)NewRow["Value"] - (double)NewRow["PrevValue"];
            }
        }

        private void SetSpeedGrowth(DataRow NewRow)
        {
            if ((NewRow["Value"] != DBNull.Value) && (NewRow["PrevValue"] != DBNull.Value))
            {
                if ((double)NewRow["PrevValue"]!=0)
                NewRow["SpeedGrowth"] = ((double)NewRow["Value"] / (double)NewRow["PrevValue"]);
            }
        }

        private void CopyValueRow(DataRow NewRow, DataRow BaseRow)
        {
            NewRow["accessory"] = BaseRow["accessory"];

            NewRow["ShortName"] = BaseRow["ShortName"];

            NewRow["Region"] = BaseRow["Region"];

            NewRow["PrevValue"] = BaseRow["PrevValue"];

            NewRow["Value"] = BaseRow["Value"];
        }

        private void ExportRFRow(DataTable BaseTable)
        {
            DataRow BaseRow = BaseTable.Rows[0];
            DataRow NewRow = TableGrid.NewRow();

            CopyValueRow(NewRow, BaseRow);

            SetGrowth(NewRow);

            SetSpeedGrowth(NewRow);

            TableGrid.Rows.Add(NewRow);
        }

        private void ExportFORow(DataTable BaseTable)
        {
            DataRow BaseRow = BaseTable.Rows[1];
            DataRow NewRow = TableGrid.NewRow();

            CopyValueRow(NewRow, BaseRow);

            SetGrowth(NewRow);

            SetSpeedGrowth(NewRow);

            SetDeviationFromParentSubject(NewRow, "DeviationRF", 0);

            TableGrid.Rows.Add(NewRow);
        }

        private void SetDeviationFromParentSubject(DataRow NewRow, string ColDeviation, int indexRowParentSubject)
        {
            DataRow RowParentSubject = TableGrid.Rows[indexRowParentSubject];

            if ((RowParentSubject["Value"] == DBNull.Value) || (NewRow["Value"] == DBNull.Value))
            {
                return;
            }

            NewRow[ColDeviation] = (double)NewRow["Value"] - (double)RowParentSubject["Value"];
        }

        private void ExportChildrenSubjectRow(DataTable BaseTable)
        {
            for (int i = 2; i < BaseTable.Rows.Count; i++)
            {
                DataRow BaseRow = BaseTable.Rows[i];
                DataRow NewRow = TableGrid.NewRow();

                CopyValueRow(NewRow, BaseRow);

                SetGrowth(NewRow);

                SetSpeedGrowth(NewRow);

                if (CurrentComparable)
                {
                    SetDeviationFromParentSubject(NewRow, "DeviationRF", 0);

                    SetDeviationFromParentSubject(NewRow, "DeviationFO", 1);
                }

                TableGrid.Rows.Add(NewRow);
            }
        }

        protected void FillRowsTableGrid()
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Region");

            ExportRFRow(BaseTable);

            ExportFORow(BaseTable);

            ExportChildrenSubjectRow(BaseTable);


            DataTable RangFO = TableGrid.Copy();

            DataTableHelper.FilterTable(RangFO, 2, "accessory", "РФ", true);

            DataTableHelper.SetRang(RangFO, "Value", "RangRF", 2, CurentInverce);

            DataTableHelper.FilterTable(RangFO, 2, "Region", ComboRegion.SelectedValue, true);


            DataTableHelper.FilterTable(TableGrid, 2, "accessory", "РФ", false);

            DataTableHelper.SetRang(TableGrid, "Value", "RangRF", 2, CurentInverce);

            DataTableHelper.FilterTable(TableGrid, 2, "accessory", ComboRegion.SelectedValue, true);

            DataTableHelper.SetRang(TableGrid, "Value", "RangFO", 2, CurentInverce);

            TableGrid.Rows[1]["RangRF"] = RangFO.Rows[2]["RangRF"];

            DataTableHelper.ClearEmtyRow(TableGrid, "Value");

        }

        protected void GenerationDataTable()
        {
            GenerationColumnFromGrid();

            FillRowsTableGrid();
        }

        protected void DataBindGrid()
        {
            GenerationDataTable();

            TableGrid = DataTableHelper.SortingDataTable(TableGrid, "Region", 2);

            Grid.DataSource = TableGrid;

            Grid.DataBind();
        }

        protected void DataBindEmptyGrid()
        {
            Grid.DataSource = null;
            Grid.DataBind();
        }
        #endregion

        #region SetDisplayGrid

        protected void SetColumnNameGrid()  
        {
            ColumnsCollection ColsGrid = Grid.Columns;

            ColsGrid.FromKey("Region").Header.Caption = "Территория";

            ColsGrid.FromKey("Value").Header.Caption = "Значение, " + CurrentUnit.ToLower();

            ColsGrid.FromKey("Growth").Header.Caption = "Абсолютное отклонение от ПП";
            ColsGrid.FromKey("Growth").Header.Title = "Абсолютное отклонение от предыдущего периода";

            ColsGrid.FromKey("SpeedGrowth").Header.Caption = "Темп роста к ПП";
            ColsGrid.FromKey("SpeedGrowth").Header.Title = "Темп роста к предыдущему периоду";

            ColsGrid.FromKey("DeviationFO").Header.Caption = "Отклонение (от ФО)";            

            ColsGrid.FromKey("RangFO").Header.Caption = "Ранг по ФО";
            ColsGrid.FromKey("RangFO").Header.Title = "Ранг по Федеральному округу";
             
            ColsGrid.FromKey("DeviationRF").Header.Caption = "Отклонение (от РФ)";            
               
            ColsGrid.FromKey("RangRF").Header.Caption = "Ранг по РФ";
            ColsGrid.FromKey("RangRF").Header.Title = "Ранг по Российской Федерации";
        }

        protected void SetDefaultStyleHeader(ColumnHeader header)
        {
            GridItemStyle HeaderStyle = header.Style;

            HeaderStyle.Wrap = true;

            HeaderStyle.VerticalAlign = VerticalAlign.Middle;

            HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void SetHeaderStyle()
        {
            foreach (ColumnHeader header in Grid.Bands[0].HeaderLayout)
            {
                SetDefaultStyleHeader(header);
            }
        }

        protected void SetHeaderGrid()
        {
            SetColumnNameGrid();

            SetHeaderStyle();
        }

        protected void SetColumnDisplay()
        {
            ColumnsCollection ColsGrid = Grid.Columns;

            ColsGrid.FromKey("ShortName").Hidden = true;

            ColsGrid.FromKey("PrevValue").Hidden = true;

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("Value"), "N2");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("Growth"), "N2");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("SpeedGrowth"), "### ### ##0.00%");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("DeviationFO"), "N2");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("RangFO"), "#####");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("DeviationRF"), "N2");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("RangRF"), "#####");

            ColsGrid.FromKey("DeviationFO").Hidden = !CurrentComparable;

            ColsGrid.FromKey("DeviationRF").Hidden = !CurrentComparable;

        }

        protected void SetColumnWidth()
        {

            Grid.Columns.FromKey("accessory").Hidden = true;
            CustomizerSize.ContfigurationGrid(Grid);
        }
            
        private void SetParentSubjectRow(UltraGridRow row)
        {
            foreach (UltraGridCell Cell in row.Cells)
            {
                Cell.Style.Font.Bold = true;
            }

            row.Cells.FromKey("SpeedGrowth").Title = string.Format("Темп роста к {0} году",
                (int.Parse(ComboPeriod.SelectedValue)-1).ToString());
            //,   MounthHelper.GetDateDativeCase(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"]).ToLower());

            row.Cells.FromKey("Growth").Title = string.Format("Прирост к {0} году",
                  (int.Parse(ComboPeriod.SelectedValue) - 1).ToString());//int.Parse(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Year"]) - 1);
            //,   MounthHelper.GetDateDativeCase(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"]).ToLower());
        } 


        private void SetParentSubjectRows()
        {
            SetParentSubjectRow(Grid.Rows[0]);
            SetParentSubjectRow(Grid.Rows[1]);
        }

        private void SetStyleChildrenRow(UltraGridRow row)
        {
            CustomizeUltraGridHelper.SetImageFromGrowth(row, "SpeedGrowth", !CurentInverce);
            row.Cells.FromKey("RangFO").Style.Font.Bold = true;
            row.Cells.FromKey("RangRF").Style.Font.Bold = true;

            row.Cells.FromKey("SpeedGrowth").Title = string.Format("Темп роста к {0} году",
                (int.Parse(ComboPeriod.SelectedValue) - 1).ToString());
            //,   MounthHelper.GetDateDativeCase(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"]).ToLower());

            row.Cells.FromKey("Growth").Title = string.Format("Прирост к {0} году",
                  (int.Parse(ComboPeriod.SelectedValue) - 1).ToString());//int.Parse(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Year"]) - 1);
            //,   MounthHelper.GetDateDativeCase(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"]).ToLower());

            //row.Cells.FromKey("Growth").Title = string.Format("Прирост к {1} {0} года",
            //    int.Parse(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Year"]) - 1, 
            //    MounthHelper.GetDateDativeCase(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"]).ToLower());

            //row.Cells.FromKey("SpeedGrowth").Title = string.Format("Темп роста к {1} {0} года",
            //    int.Parse(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Year"]) - 1, 
            //    MounthHelper.GetDateDativeCase(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"]).ToLower());
        }

        private void SetChildrenSubjectRow()
        {
            foreach (UltraGridRow row in Grid.Rows)
            {
                SetStyleChildrenRow(row);
            }
        }

        protected void ConfigurationRows()
        {
            SetParentSubjectRows();
            SetChildrenSubjectRow();

            CustomizeUltraGridHelper.SetStarGrayFromValue(Grid, Grid.Columns.FromKey("RangFO").Index, CurentInverce, (Grid.Rows.Count - 2).ToString());
            CustomizeUltraGridHelper.SetStarGrayFromValue(Grid, Grid.Columns.FromKey("RangRF").Index, CurentInverce, "83");

            CustomizeUltraGridHelper.SetStarYellowFromValue(Grid, Grid.Columns.FromKey("RangFO").Index, CurentInverce, (1).ToString());
            CustomizeUltraGridHelper.SetStarYellowFromValue(Grid, Grid.Columns.FromKey("RangRF").Index, CurentInverce, "1");

            if (Grid.Rows[1].Cells.FromKey("RangRF").ToString() == "8")
            {
                CustomizeUltraGridHelper.SetStarGrayFromValue(Grid, Grid.Columns.FromKey("RangRF").Index, CurentInverce, "8");
            }
        }

        private void SetOtherDispaly()
        {
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
        }

        protected void SetDisplayGrid()
        {
            SetHeaderGrid();

            SetColumnDisplay();

            SetColumnWidth();

            headerLayout = new GridHeaderLayout(Grid);

            ConfigurationRows();

            SetOtherDispaly();
        }
        #endregion

        #region Chart
        protected void DeleteValueFOAndRF(DataTable Table)
        {
            Table.Rows[0].Delete();
            Table.Rows[0].Delete();
        }

        protected void DataBindChart()
        {
            TableChart.Columns.Add("ShortName", typeof(string));
            TableChart.Columns.Add("Value", typeof(double));

            DataTableHelper.CopyRows(TableGrid, TableChart);

            DeleteValueFOAndRF(TableChart);

            TableChart = DataTableHelper.SortingDataTable(TableChart, "Value");

            Chart.DataSource = TableChart;

            Chart.DataBind();
        }

        private void ConfChartAxisY()
        {
            Infragistics.UltraChart.Resources.Appearance.AxisAppearance AxisY = Chart.Axis.Y;

            double ChartMaxValue = DataTableHelper.GetExtremum(TableChart, TableChart.Columns["Value"], DataTableHelper.Extrmum.Max);
            double ChartMinValue = DataTableHelper.GetExtremum(TableChart, TableChart.Columns["Value"], DataTableHelper.Extrmum.Min);

            if ((ChartMaxValue == double.NegativeInfinity) || (ChartMinValue == double.PositiveInfinity))
            { }
            else
            {
                AxisY.RangeMax = ChartMaxValue * 1.1;
                AxisY.RangeMin = ChartMinValue * 0.9;
                AxisY.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
            }

            AxisY.Labels.ItemFormatString = "<DATA_VALUE:### ### ### ##0.00>";

            AxisY.Labels.Font = new Font("Arial", 10);

            AxisY.Extent = 80;

        }

        private void ConfChartAxisX()
        {
            Infragistics.UltraChart.Resources.Appearance.AxisAppearance AxisX = Chart.Axis.X;
            AxisX.Extent = 160;
            AxisX.Labels.Font = new Font("Arial", 10);
        }

        private void ConfChartToolTips()
        {
            Infragistics.UltraChart.Resources.Appearance.WebTooltipAppearance Tooltips = Chart.Tooltips;
            Tooltips.FormatString = "<ITEM_LABEL><br> <b><DATA_VALUE:### ### ##0.00></b> " + CurrentUnit.ToLower();
        }

        private void ConfigurationChart()
        {
            ConfChartAxisY();
            ConfChartAxisX();
            ConfChartToolTips();
        }
        #endregion

        #region DundasMap
        protected void CustomizeMapPanel()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = 1 == 1;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap.NavigationPanel.Visible = 1 == 1;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.Viewport.EnablePanning = true;
        }

        protected void AddShapeField()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Value");
            DundasMap.ShapeFields["Value"].Type = typeof(double);
            DundasMap.ShapeFields["Value"].UniqueIdentifier = false;
        }

        protected string SetHyphen(string BaseString)
        {


            string[] Word = BaseString.Split(' ');
            int Median = Word.Length / 2;

            string res = "";
            for (int i = 0; i < Median; i++)
            {
                res += Word[i] + " ";
            }
            res += "\n";
            for (int i = Median; i < Word.Length; i++)
            {
                res += Word[i] + " ";
            }

            return res;

        }

        protected void AddLegend()
        {
            DundasMap.Legends.Clear();
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Title = string.Format("{0}:\n{1}\n(по состоянию на {2} год)", ComboDirection.SelectedNodeParent, SetHyphen(ComboDirection.SelectedValue),ComboPeriod.SelectedValue);
            //"Значение";
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            //legend1.Size.Width = 500f;
            legend1.AutoFitMinFontSize = 7;

            DundasMap.Legends.Add(legend1);
        }

        protected void FillRule()
        {
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorValueRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Value";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = !CurentInverce ? Color.Green : Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = CurentInverce ? Color.Green : Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";

            rule.LegendText = "#FROMVALUE{N1} - #TOVALUE{N1}";//"LegendText";
            DundasMap.ShapeRules.Add(rule);
        }

        protected string ShorteningFO(string LongName)
        {
            switch (LongName)
            {
                case "Центральный федеральный округ": { return "ЦФО"; }
                case "Северо-Западный федеральный округ": { return "СФО"; }
                case "Южный федеральный округ": { return "ЮФО"; }
                case "Северо-Кавказский федеральный округ": { return "СКФО"; }
                case "Приволжский федеральный округ": { return "ПФО"; }
                case "Уральский федеральный округ": { return "УрФО"; }
                case "Сибирский федеральный округ": { return "СФО"; }
                case "Дальневосточный федеральный округ": { return "ДФО"; }
            }

            throw new Exception("Неизвестный регион");
        }

        protected void LoadMap()
        {
            string RegionName = ComboRegion.SelectedValue;

            string DirectoryName = ShorteningFO(RegionName);

            string FileName = ShorteningFO(RegionName);

            string MapPath = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", DirectoryName, FileName));

            DundasMap.LoadFromShapeFile(MapPath, "NAME", true);
        }

        protected void ConfigurationMap()
        {
            CustomizeMapPanel();

            AddShapeField();

            AddLegend();

            FillRule();

            LoadMap();
        }

        protected Shape GetShapefromRegionName(string RegionShortName, string RegionFullName)
        {
            foreach (Shape shape in DundasMap.Shapes)
            {
                if ((RegionShortName == shape["NAME"].ToString()) || (RegionFullName == shape["NAME"].ToString()))
                {
                    return shape;
                }
            }
            return null;
        }
        protected void SetShapeTooltip(Shape shape, DataRow dataRow)
        {
            if (dataRow["RangFO"] != DBNull.Value)
            {
                shape.ToolTip = string.Format("{0}\n{1:########0.00}, {2}\nРанг по ФО:{3}",
                    dataRow["Region"],
                    dataRow["Value"],
                    CurrentUnit.ToLower(),
                    dataRow["RangFO"]);
            }
            else
            {
                shape.ToolTip = string.Format("{0}\n{1:### ### ##0.00}, {2}",
                    dataRow["Region"],
                    dataRow["Value"],
                    CurrentUnit);
            }
        }

        private void FillShape(Shape shape, DataRow dataRow)
        {
            if (shape == null)
            {
                return;
            }

            if (dataRow["Value"] == DBNull.Value)
            {
                return;
            }


            shape["Value"] = dataRow["Value"];
            shape.Text = string.Format("{0}\n{1:### ### ##0.##}", dataRow["Region"].ToString().Replace(" ","\n"), dataRow["Value"]);

            shape.Visible = true;

            shape.TextVisibility = TextVisibility.Shown;

            SetShapeTooltip(shape, dataRow);
        }

        protected void DataBindMap()
        {
            for (int i = 2; i < TableGrid.Rows.Count; i++)
            {
                Shape shape = GetShapefromRegionName(TableGrid.Rows[i]["ShortName"].ToString(), TableGrid.Rows[i]["Region"].ToString());
                FillShape(shape, TableGrid.Rows[i]);
            }
        }

        #endregion

        protected void SetHeader()
        {
            Hederglobal.Text = "Ежегодный мониторинг социально-экономического развития (в разрезе субъектов РФ)";

            PageSubTitle.Text = string.Format(@"Анализ социально-экономического положения территории по выбранному показателю в разрезе субъектов РФ, {1}, за {0} год",
                "<b>"+ComboPeriod.SelectedValue+"</b>",
                "<b>"+ComboRegion.SelectedValue+"</b>");

            Page.Title = Hederglobal.Text;
             
            HeaderChart.Text = string.Format("Распределение субъектов РФ по показателю «{0}», {1}, за {2} год ({3})", 
                ComboDirection.SelectedValue, 
                CurrentUnit.ToLower(),
                "<b>"+ComboPeriod.SelectedValue+"</b>",
                ComboRegion.SelectedValue);
            HeaderGrid.Text = ComboDirection.SelectedValue;

            HeaderMap.Text = string.Format("{0}, {1} ({2})",ComboDirection.SelectedValue,CurrentUnit.ToLower(),ComboRegion.SelectedValue);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            ConfigurationMultiCombo();
            
            ChosenParam();
            try
            {
                DataBindGrid();
                if (TableGrid.Rows.Count > 0)
                {
                    SetDisplayGrid();

                    DataBindChart();

                    ConfigurationChart();
                }
                else
                {
                    EmptyDate();
                }
            }
            catch { EmptyDate(); }

                ConfigurationMap();

                DataBindMap();

                SetHeader();
        }

        private void EmptyDate()
        {
            DataBindEmptyGrid();
            Grid.DisplayLayout.NoDataMessage = "Нет данных";
            Chart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(Chart_InvalidDataReceived);
        }

        void Chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);   
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
            textLabel.PE.Fill = System.Drawing.Color.Black;
            //textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 8);
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);

            textLabel.labelStyle.FontColor = Color.LightGray;

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


            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.SetTextString(Label);

            e.SceneGraph.Add(Line);
            e.SceneGraph.Add(textLabel);


        }


        void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if ((!CurrentComparable)||(TableGrid.Rows.Count == 0))
            {
                return;
            }
            

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double RFStartLineX = 0;
            double RFStartLineY = 0;

            double RFEndLineX = 0;
            double RFEndLineY = 0;
            string RFheader = "";

            if (TableGrid.Rows[0]["Value"] != DBNull.Value)
            {
                RFheader = string.Format("{0}: {1:N2} {2}", TableGrid.Rows[0]["Region"], TableGrid.Rows[0]["Value"], CurrentUnit.ToLower());

                RFStartLineX = xAxis.Map(xAxis.Minimum);
                RFStartLineY = yAxis.Map((double)TableGrid.Rows[0]["Value"]);

                RFEndLineX = xAxis.Map(xAxis.Maximum);
                RFEndLineY = yAxis.Map((double)TableGrid.Rows[0]["Value"]);


            }

            double FOStartLineX = 0;
            double FOStartLineY = 0;
            double FOEndLineX = 0;
            double FOEndLineY = 0;
            string FOheader = "";

            if (TableGrid.Rows[1]["Value"] != DBNull.Value)
            {
                FOheader = string.Format("{0}: {1:N2} {2}", TableGrid.Rows[1]["Region"], TableGrid.Rows[1]["Value"], CurrentUnit.ToLower());

                FOStartLineX = xAxis.Map(xAxis.Minimum);
                FOStartLineY = yAxis.Map((double)TableGrid.Rows[1]["Value"]);

                FOEndLineX = xAxis.Map(xAxis.Maximum);
                FOEndLineY = yAxis.Map((double)TableGrid.Rows[1]["Value"]);


            }


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

        protected void Chart_FillSceneGraph1(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Chart_FillSceneGraph(sender, e);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text.Replace("<b>", "").Replace("</b>", "").Replace("<b>", "").Replace("<b>", "");
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 12*20;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text.Replace("<b>", "").Replace("</b>", "").Replace("<b>", "").Replace("<b>", "");
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                //e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "### ### ##0.##";
            }

            

            ExportGridToExcel(Grid, e.Workbook.Worksheets["Таблица"], 4, false);
            
            try
            {
                e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].Value = HeaderChart.Text.Replace("<b>", "").Replace("</b>", "").Replace("<b>", "").Replace("<b>", "");
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0], Chart);


                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].Value = HeaderMap.Text.Replace("<b>", "").Replace("</b>", "").Replace("<b>", "").Replace("<b>", "");

                ReportExcelExporter.MapExcelExport(e.Workbook.Worksheets["Карта"].Rows[1].Cells[0], DundasMap);

                e.Workbook.Worksheets["Таблица"].Rows[3].Cells[0].Value = HeaderGrid.Text;     
                            
            }
            catch { }
        }

        private void SetSizeMapAndChart()
        {
            Chart.Width = (int)(Chart.Width.Value * 0.75);

            DundasMap.Width = (int)(Chart.Width.Value * 0.85);
            DundasMap.Height = (int)(DundasMap.Height.Value * 0.75);
        }

        private void CleraHidenColGrid()
        {
            foreach (UltraGridColumn col in Grid.Columns)
            {
                if ((col.Hidden) || (col.Width.Value == 0))
                {
                    Grid.Bands[0].HeaderLayout.Remove(col.Header);
                    Grid.Columns.Remove(col);
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");

            SetSizeMapAndChart();

            CleraHidenColGrid();

            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 5;
            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";
            ReportExcelExporter1.ExcelExporter.Export(Grid, sheet1);
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


        }

        object FormatVal(object value)
        { return value;}

        int ExportHeaderGrid(HeadersCollection Headers, Worksheet WorkSheet, int StartRow)
        {

            for(int i =0;i<Grid.Columns.Count;i++)
            {
                if(Grid.Columns[i].Hidden)
                {
                }
                else
                {
                    try
                    {
                        WorkSheet.Rows[StartRow].Cells[i-1  ].Value = Grid.Columns[i].Header.Caption;
                        SetStyleHeadertableFromExcel(WorkSheet.Rows[StartRow].Cells[i-1].CellFormat);
                    }
                    catch { }
                }
            }

            for (int i = StartRow; i < StartRow; i++)
            {
                WorkSheet.Rows[i].Height = 20 * 40;
            }

            return StartRow;
        }

        void ExportGrid(UltraWebGrid G, Worksheet sheet, int startrow)
        {
            ExportGridToExcel(G, sheet, startrow, 1 == 1);
        }

        void ExportWidthGrid(UltraWebGrid G, Worksheet sheet)
        {
            for (int i = 0; i < G.Columns.Count; i++)
            {
                if (G.Columns[i].Hidden)
                {
                    sheet.Columns[i].Width = 0;
                }
                else
                {
                    sheet.Columns[i].Width = (int)G.Columns[i].Width.Value * 36;
                }
                sheet.Columns[i].CellFormat.FormatString = G.Columns[i].Format;
            }
        }

        void ExportGridToExcel(UltraWebGrid G, Worksheet sheet, int startrow, bool RowZebra)
        {

            startrow = ExportHeaderGrid(G.Bands[0].HeaderLayout, sheet, startrow);

            for (int i = 0; i < G.Rows.Count; i++)
            {
                sheet.Rows[i + startrow].Height = 32 * 40;
                sheet.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                //sheet.Rows[i + startrow].CellFormat.FormatString = "### ### ##0.00";
            }

            for (int i = 0; i < Grid.Columns.Count; i++)
            {
                if (!Grid.Columns[i].Header.Caption.Contains("по"))
                    sheet.Columns[i].CellFormat.FormatString = "### ### ##0.00";
                else
                {
                    sheet.Columns[i].CellFormat.FormatString = "### ### ##0";
                    sheet.Columns[i - 1].CellFormat.FormatString = "### ### ##0";
                }

            }

            for (int i = 1; i < G.Rows.Count; i++)
            {
                sheet.Rows[i + startrow].Height = 12 * 40;
            }

            sheet.Columns[0].Width = 200 * 36;

            for (int i = 1; i < 10; i++)
            {
                sheet.Columns[i].Width = 100 * 36;
            }
        }

        #endregion
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection is_ = section2;
            is_.PageSize = new PageSize(950, 500);
            IText title = is_.AddText();
            title.Margins = new Infragistics.Documents.Reports.Report.Margins(40, 40, 40, 40);
            Font font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;
            title.AddContent(HeaderChart.Text.Replace("<b>", "").Replace("</b>", "").Replace("<b>", "").Replace("<b>", ""));
            Chart.Height = 400;
            IImage ima = is_.AddImage(UltraGridExporter.GetImageFromChart(Chart));
            ima.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 0, 5, 0);
            title = section1.AddText();
            font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Hederglobal.Text.Replace("<b>", "").Replace("</b>", "").Replace("<b>", "").Replace("<b>", ""));
            title = section1.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text.Replace("<b>", "").Replace("</b>", ""));

            title = section1.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(HeaderGrid.Text.Replace("<b>", "").Replace("</b>", ""));

            UltraGridExporter1.PdfExporter.Export(Grid, section1);
            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 1.0);
            DundasMap.Height = 800;
            ISection section3 = report.AddSection();
            title = section3.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent( HeaderMap.Text);

            section3.PageSize = new PageSize(900, 1000);
            section3.AddImage(ReportPDFExporter.GetImageFromMap(DundasMap));

        }
        #endregion
    }

}
