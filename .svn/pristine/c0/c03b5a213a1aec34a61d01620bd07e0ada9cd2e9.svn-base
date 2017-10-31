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


namespace Krista.FM.Server.Dashboards.reports.SEP_002
{
    public partial class _default : CustomReportPage
    {

        int ScreenWidth { get { return ((int)Session[CustomReportConst.ScreenWidthKeyName]); } }
        int ScreenHeight { get { return ((int)Session[CustomReportConst.ScreenHeightKeyName]); } }

        ICustomizerSize CustomizerSize;

        IDataSetCombo DataComboRegion = new DataSetComboTwoLevel();
        IDataSetCombo DataComboPeriod = new DataSetComboTwoLevel();
        IDataSetCombo DataComboDirection = new DataSetComboTwoLevel();

        DataTable TableGrid = new DataTable();
        DataTable TableChart = new DataTable();


        CustomParam ChosenDate { get { return (UserParams.CustomParam("ChosenDate")); } }
        CustomParam ChosenRegionFO { get { return (UserParams.CustomParam("ChosenRegionFO")); } }
        CustomParam ChosenRegionSubject { get { return (UserParams.CustomParam("ChosenRegionSubject")); } }
        CustomParam ChosenSEP { get { return (UserParams.CustomParam("ChosenSEP")); } }

        CustomParam ChosenDetail { get { return (UserParams.CustomParam("ChosenDetail")); } }

        CustomParam ChosenYear { get { return (UserParams.CustomParam("ChosenYear")); } }

        CustomParam IndexActiveRow { get { return (UserParams.CustomParam("IndexActiveRow")); } }

        string CurrentSepDetail = "";
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

                int OnePercent = ((int)Grid.Width.Value - 6) / 100;

                ColsGrid.FromKey("SEP").Width = OnePercent * 28;

                ColsGrid.FromKey("ValueSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthSubject").Width = OnePercent * 6;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 4;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 4;

                ColsGrid.FromKey("ValueFO").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthFO").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthFO").Width = OnePercent * 6;

                ColsGrid.FromKey("ValueRF").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthRF").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthRF").Width = OnePercent * 6;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)Grid.Width.Value - 6) / 100;

                ColsGrid.FromKey("SEP").Width = OnePercent * 28;

                ColsGrid.FromKey("ValueSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthSubject").Width = OnePercent * 6;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 4;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 4;

                ColsGrid.FromKey("ValueFO").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthFO").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthFO").Width = OnePercent * 6;

                ColsGrid.FromKey("ValueRF").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthRF").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthRF").Width = OnePercent * 6;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                ColumnsCollection ColsGrid = Grid.Columns;

                int OnePercent = ((int)Grid.Width.Value - 6) / 100;

                ColsGrid.FromKey("SEP").Width = OnePercent * 28;

                ColsGrid.FromKey("ValueSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthSubject").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthSubject").Width = OnePercent * 6;

                ColsGrid.FromKey("RangFO").Width = OnePercent * 5;

                ColsGrid.FromKey("RangRF").Width = OnePercent * 5;

                ColsGrid.FromKey("ValueFO").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthFO").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthFO").Width = OnePercent * 6;

                ColsGrid.FromKey("ValueRF").Width = OnePercent * 7;

                ColsGrid.FromKey("GrowthRF").Width = OnePercent * 7;

                ColsGrid.FromKey("SpeedGrowthRF").Width = OnePercent * 6;
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
            static DataProvider ActiveDataPorvider = DataProvidersFactory.SecondaryMASDataProvider;

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

            public static  DataTable SetRang(DataTable Table, string ColumnValueName, string ColumnRangName, int StartRow, bool revrce)
            {
                Table = SortingDataTable(Table, ColumnValueName);
                int i = 0;

                decimal prevval = decimal.MinValue;

                foreach (DataRow row in Table.Rows)
                {
                    decimal CurVal = (decimal)row[ColumnValueName];
                    if (CurVal != prevval)
                    {
                        i++;
                    }
                    prevval = CurVal;
                    row[ColumnRangName] = i;
                }

                return Table;

                //int RowCount = Table.Rows.Count;
                //int IndexMax = StartRow;
                //int IndexMin = StartRow;
                //int rang = 0;
                //for (int i = StartRow; i < RowCount; i++)
                //{
                //    for (int j = StartRow; j < RowCount; j++)
                //    {
                //        if (Table.Rows[j][ColumnValueName] != System.DBNull.Value)
                //        {

                //            if (((decimal)(Table.Rows[j][ColumnValueName]) <= (decimal)(Table.Rows[IndexMax][ColumnValueName])) && (Table.Rows[j][ColumnRangName] == DBNull.Value))
                //            {
                //                IndexMax = j;
                //            }
                //            if (((decimal)(Table.Rows[j][ColumnValueName]) > (decimal)(Table.Rows[IndexMin][ColumnValueName])) && (Table.Rows[j][ColumnRangName] == DBNull.Value))
                //            {
                //                IndexMin = j;
                //            }

                //        }
                //    }
                //    if (revrce)
                //    {
                //        int b = IndexMax;
                //        IndexMax = IndexMin;
                //        IndexMin = b;
                //    }
                //    if (Table.Rows[IndexMin][ColumnValueName] != System.DBNull.Value)
                //        if (Table.Rows[IndexMin][ColumnRangName] == DBNull.Value)
                //        {
                //            Table.Rows[IndexMin][ColumnRangName] = ++rang;
                //        }
                //    IndexMin = IndexMax;
                //}

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
                //foreach (DataRow BaseRow in BaseTable.Rows)
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

                if (o1.GetType() == typeof(decimal))
                {
                    return ((decimal)(o1) > (decimal)(o2));
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
                            if (ComparedObject((BaseTable.Rows[j][ColSort]), BaseTable.Rows[IndexMaxRow][ColSort]))
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


            //public static void SetStarFromValue(UltraWebGrid G, int Col, int BaseValue, string Star, string Title)
            //{
            //    for (int i = 0; G.Rows.Count > i; i++)
            //    {
            //        if (G.Rows[i].Cells[Col].Text == BaseValue.ToString())
            //        {
            //            G.Rows[i].Cells[Col].Title = Title;
            //            G.Rows[i].Cells[Col].Style.BackgroundImage = Star;//"~/images/starYellowBB.png";
            //            G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            //            //"Самый высокий интегральный показатель"
            //        }

            //    }
            //}

            internal static void SetStarFromValue(UltraWebGrid G, int Col, Dictionary<string, int> MaxRangFromFiledFo, string Star, string Title)
            {
                for (int i = 0; G.Rows.Count > i; i++)
                {
                    if (G.Rows[i].Cells.FromKey("SEP").Value != null)
                    if (MaxRangFromFiledFo.ContainsKey(G.Rows[i].Cells.FromKey("SEP").Text))
                        if (G.Rows[i].Cells[Col].Text == MaxRangFromFiledFo[G.Rows[i].Cells.FromKey("SEP").Text].ToString())
                    {
                        G.Rows[i].Cells[Col].Title = Title;
                        G.Rows[i].Cells[Col].Style.BackgroundImage = Star;
                        G.Rows[i].Cells[Col].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

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

                        //DataForCombo.Add(Parent2, 0);
                        DataForCombo.Add(Parent2.Replace("Предприятия и организации", "Промышленное производство"), 0);
                        LastParent = Parent;
                    }

                    Child = this.GetAlowableAndFormatedKey(Child, Parent).Replace("Предприятия и организации", "Промышленное производство");

                    this.AddItem(Child, Parent, UniqueName);

                    this.addOtherInfo(Table, row, Child);
                }
            }
        }

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

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            //UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_Test);


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
            Grid.Height = 250;

            Chart.Width = CustomizerSize.GetChartWidth();
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
            if (!Page.IsPostBack)
            {
                SetSettingMultiCombo(ComboDirection);
                ComboDirection.Title = "Направление";
                ComboDirection.Width = CustomizerSize.GetComboDirectionWidth();
                ComboDirection.ParentSelect = true;

                SetSettingMultiCombo(ComboPeriod);
                ComboPeriod.Title = "Месяц";
                ComboPeriod.Width = CustomizerSize.GetComboPeriodWidth();


                SetSettingMultiCombo(ComboRegion);
                ComboRegion.Title = "Субъект РФ";
                ComboRegion.Width = CustomizerSize.GetComboRegonWidth();
            }

            DataComboDirection.SetFormatterText((parent, children) => { return children; });
            DataComboDirection.SetFormatterTextParent((Parent) => { return "Все"; });
            FillDataCombo(ComboDirection, "Direction", DataComboDirection);

            DataComboPeriod.SetFormatterText((parent, children) => { return children + " " + parent + " года"; });
            DataComboPeriod.SetFormatterTextParent((parent) => { return parent + " год"; });
            DataComboPeriod.LevelParent = 4;
            FillDataCombo(ComboPeriod, "Period", DataComboPeriod);

            DataComboRegion.SetFormatterText((parent, children) => { return children; });
            DataComboRegion.SetFormatterTextParent((Parent) => { return Parent; });
            FillDataCombo(ComboRegion, "Region", DataComboRegion);



            if (!Page.IsPostBack)
            {

                ComboPeriod.SetСheckedState(DataComboPeriod.LastAdededKey, true);
                ComboRegion.SetСheckedState(
                   RegionSettingsHelper.Instance.Name
                    , true);
                ComboDirection.SetСheckedState("Все", true);
            }
        }

        private void ChosenParam()
        {
            string SelectedDate = ComboPeriod.SelectedValue;
            ChosenDate.Value = DataComboPeriod.DataUniqeName[SelectedDate];

            string SelectedRegionFO = ComboRegion.SelectedNodeParent;
            ChosenRegionFO.Value = SelectedRegionFO;

            string SelectedRegionSubject = ComboRegion.SelectedValue;
            ChosenRegionSubject.Value = SelectedRegionSubject;

            if (ComboDirection.SelectedValue != "Все")
            {
                string SelectedSEP = ComboDirection.SelectedValue;
                ChosenSEP.Value = DataComboDirection.DataUniqeName[SelectedSEP] + ".children";

                


                /*
             
                 */

            }
            else
            {
                ChosenSEP.Value = "[СЭП].[Годовой сборник].[Уровень 1].members";
            }

//            ChosenSEP.Value = @"    
//            Generate  
//            (
//                "+ ChosenSEP.Value+@",
//                IIF  
//                (
//                    [СЭП__Годовой сборник].[СЭП__Годовой сборник].CurrentMember is [СЭП__Годовой сборник].[СЭП__Годовой сборник].[Индекс промышленного производства],
//                    {
//                        [СЭП__Годовой сборник].[СЭП__Годовой сборник].[Индекс промышленного производства: добыча полезных ископаемых],
//                        [СЭП__Годовой сборник].[СЭП__Годовой сборник].[Индекс промышленного производства: обрабатывающие производства],
//                        [СЭП__Годовой сборник].[СЭП__Годовой сборник].[Индекс промышленного производства: производство и распределение электроэнергии, газа и воды]
//                    },
//                    [СЭП__Годовой сборник].[СЭП__Годовой сборник].CurrentMember  
//                )  
//            )";
            string SelectYear = DataComboPeriod.OtherInfo[SelectedDate]["Year"];
            ChosenYear.Value = SelectYear;
        }

        #endregion

        #region DataBindGrid

        protected void GenerationColumnFromGrid()
        {
            TableGrid.Columns.Add("ParentName", typeof(string));

            TableGrid.Columns.Add("UniqueName", typeof(string));

            TableGrid.Columns.Add("Reverce", typeof(string));

            TableGrid.Columns.Add("Unit", typeof(string));

            TableGrid.Columns.Add("Comporable", typeof(string));

            TableGrid.Columns.Add("PrevValueSubject", typeof(double));

            TableGrid.Columns.Add("PrevValueFO", typeof(double));

            TableGrid.Columns.Add("PrevValueRF", typeof(double));

            TableGrid.Columns.Add("SEP", typeof(string));

            TableGrid.Columns.Add("ValueSubject", typeof(double));

            TableGrid.Columns.Add("GrowthSubject", typeof(double));

            TableGrid.Columns.Add("SpeedGrowthSubject", typeof(double));

            TableGrid.Columns.Add("RangFO", typeof(double));

            TableGrid.Columns.Add("RangRF", typeof(double));

            TableGrid.Columns.Add("ValueFO", typeof(double));

            TableGrid.Columns.Add("GrowthFO", typeof(double));

            TableGrid.Columns.Add("SpeedGrowthFO", typeof(double));

            TableGrid.Columns.Add("ValueRF", typeof(double));

            TableGrid.Columns.Add("GrowthRF", typeof(double));

            TableGrid.Columns.Add("SpeedGrowthRF", typeof(double));
        }

        private void SetGrowth(DataRow NewRow)
        {
            if ((NewRow["ValueSubject"] != DBNull.Value) && (NewRow["PrevValueSubject"] != DBNull.Value))
            {
                NewRow["GrowthSubject"] = (double)NewRow["ValueSubject"] - (double)NewRow["PrevValueSubject"];
            }

            if ((NewRow["ValueFO"] != DBNull.Value) && (NewRow["PrevValueFO"] != DBNull.Value))
            {
                NewRow["GrowthFO"] = (double)NewRow["ValueFO"] - (double)NewRow["PrevValueFO"];
            }

            if ((NewRow["ValueRF"] != DBNull.Value) && (NewRow["PrevValueRF"] != DBNull.Value))
            {
                NewRow["GrowthRF"] = (double)NewRow["ValueRF"] - (double)NewRow["PrevValueRF"];
            }
        }

        private void SetSpeedGrowth(DataRow NewRow)
        {
            if ((NewRow["ValueSubject"] != DBNull.Value) && (NewRow["PrevValueSubject"] != DBNull.Value))
            {
                if ((double)NewRow["PrevValueSubject"] != 0)
                    NewRow["SpeedGrowthSubject"] = ((double)NewRow["ValueSubject"] / (double)NewRow["PrevValueSubject"]);
            }

            if ((NewRow["ValueFO"] != DBNull.Value) && (NewRow["PrevValueFO"] != DBNull.Value))
            {
                if ((double)NewRow["PrevValueFO"] != 0)
                    NewRow["SpeedGrowthFO"] = ((double)NewRow["ValueFO"] / (double)NewRow["PrevValueFO"]);
            }

            if ((NewRow["ValueRF"] != DBNull.Value) && (NewRow["PrevValueRF"] != DBNull.Value))
            {
                if ((double)NewRow["PrevValueRF"] != 0)
                    NewRow["SpeedGrowthRF"] = ((double)NewRow["ValueRF"] / (double)NewRow["PrevValueRF"]);
            }

        }

        private void CopyValueRow(DataRow NewRow, DataRow BaseRow)
        {
            foreach (DataColumn Col in BaseRow.Table.Columns)
            {
                NewRow[Col.ColumnName] = BaseRow[Col];
            }
        }

        private void SetRangRowRF(DataRow Row)
        {

            ChosenSEP.Value = Row["UniqueName"].ToString();

            DataTable TableRang = DataBaseHelper.ExecQueryByID("RangRF", "Region");
            
            TableRang = DataTableHelper.SetRang(TableRang, "Value", "Rang", 0, Row["Reverce"].ToString() == "1");

            DataRow RangedRow = DataTableHelper.FindRow(TableRang, "Region", ChosenRegionSubject.Value);

            if (RangedRow != null)
            {
                Row["RangRF"] = RangedRow["Rang"];
            }
        }

        Dictionary<string, int> MaxRangFromFiledFo = new Dictionary<string, int>();

        private void SetRangRowFO(DataRow Row)
        {
            ChosenSEP.Value = Row["UniqueName"].ToString();

            DataTable TableRang = DataBaseHelper.ExecQueryByID("RangFO", "Region");

            TableRang =  DataTableHelper.SetRang(TableRang, "Value", "Rang", 0, Row["Reverce"].ToString() == "1");

            DataRow RangedRow = DataTableHelper.FindRow(TableRang, "Region", ChosenRegionSubject.Value);

                      

            if (RangedRow != null) 
            {

                if (!MaxRangFromFiledFo.ContainsKey(Row["SEP"].ToString()))
                {
                    int MaxRang = int.Parse(TableRang.Rows[TableRang.Rows.Count-1]["Rang"].ToString());
                    MaxRangFromFiledFo.Add(Row["SEP"].ToString(), MaxRang);
                    

                }

                Row["RangFO"] = RangedRow["Rang"];
            }

        }

        private void ExportTitleRow(DataRow NewRow, DataRow BaseRow)
        {
            NewRow["SEP"] = BaseRow["ParentName"];
        }

        private void ExportDataRow(DataRow NewRow, DataRow BaseRow)
        {
            CopyValueRow(NewRow, BaseRow);

            SetGrowth(NewRow);

            SetSpeedGrowth(NewRow);

            SetRangRowRF(NewRow);

            SetRangRowFO(NewRow);
        }

        private void ExportAllRow(DataTable BaseTable)
        {
            string LastParentName = "";

            foreach (DataRow BaseRow in BaseTable.Rows)
            {
                string ParentName = BaseRow["ParentName"].ToString();

                DataRow NewRow = null;

                if (LastParentName != ParentName)
                {
                    NewRow = TableGrid.NewRow();

                    TableGrid.Rows.Add(NewRow);

                    ExportTitleRow(NewRow, BaseRow);

                    LastParentName = ParentName;
                }

                NewRow = TableGrid.NewRow();

                TableGrid.Rows.Add(NewRow);

                ExportDataRow(NewRow, BaseRow);
            }
        }



        protected void FillRowsTableGrid()
        {
            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "SEP");

            ExportAllRow(BaseTable);

        }

        private void Filter()
        {
            List<string> RemovedKey = new List<string>();
            //RemovedKey.Add("Индекс промышленного производства по ОКВЭД, процент");
            RemovedKey.Add("Отгружено товаров собственного производства, выполнено работ и услуг собственными силами по ОКВЭД, миллион рублей");

            List<DataRow> RemovedRow = new List<DataRow>();

            foreach (DataRow row in TableGrid.Rows)
            {
                row["SEP"] = row["SEP"].ToString().Replace("Предприятия и организации", "Промышленное производство");

                foreach (string rk in RemovedKey)
                {
                    if (rk.Contains(row["SEP"].ToString()))
                    {
                        RemovedRow.Add(row);
                    }
                }
            }
            
            foreach (DataRow rr in RemovedRow)
            {
                try
                {
                    
                    TableGrid.Rows.Remove(rr);
                }
                catch { }
            }
        }

        protected void GenerationDataTable()
        {
            GenerationColumnFromGrid();

            FillRowsTableGrid();

            Filter();

        }



        protected void DataBindGrid()
        {
            GenerationDataTable();

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

            ColsGrid.FromKey("SEP").Header.Caption = "Показатель";

            ColsGrid.FromKey("ValueSubject").Header.Caption = "Значение";

            ColsGrid.FromKey("GrowthSubject").Header.Caption = "Абсолютное отклонение от АППГ";

            ColsGrid.FromKey("SpeedGrowthSubject").Header.Caption = "Темп роста к АППГ";

            ColsGrid.FromKey("RangFO").Header.Caption = "Ранг по ФО";

            ColsGrid.FromKey("RangRF").Header.Caption = "Ранг по РФ";


            ColsGrid.FromKey("ValueFO").Header.Caption = "Значение";

            ColsGrid.FromKey("GrowthFO").Header.Caption = "Абсолютное отклонение от АППГ";

            ColsGrid.FromKey("SpeedGrowthFO").Header.Caption = "Темп роста к АППГ";


            ColsGrid.FromKey("ValueRF").Header.Caption = "Значение";

            ColsGrid.FromKey("GrowthRF").Header.Caption = "Абсолютное отклонение от АППГ";

            ColsGrid.FromKey("SpeedGrowthRF").Header.Caption = "Темп роста к АППГ";

            foreach (ColumnHeader hb in Grid.Bands[0].HeaderLayout)
            {
                hb.TitleMode = CellTitleMode.Always;

                if (hb.Column != null)
                {
                    if (hb.Column.BaseColumnName.Contains("Value"))
                    {
                        hb.Title = "";
                        continue;
                    }

                    if (hb.Column.BaseColumnName.Contains("SpeedGrowth"))
                    {
                        hb.Title = "Темп роста к аналогичному периоду предыдущему года";
                        continue;
                    }

                    if (hb.Column.BaseColumnName.Contains("Growth"))
                    {
                        hb.Title = "Абсолютное отклонение от аналогичного периода предыдущего года";
                        continue;
                    }
                     


                    if (hb.Column.BaseColumnName.Contains("RangFO"))
                    {
                        hb.Title = "Ранг по Федеральному округу";
                        continue;
                    }

                    if (hb.Column.BaseColumnName.Contains("RangRF"))
                    {
                        hb.Title = "Ранг по Российской Федерации";
                        continue;
                    }
                }
            }
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
                header.RowLayoutColumnInfo.OriginY = 1;
            }
            #region Шапка грида
            Grid.Columns.FromKey("SEP").Header.RowLayoutColumnInfo.SpanY = 2;
            Grid.Columns.FromKey("SEP").Header.RowLayoutColumnInfo.OriginY = 0;
            Grid.Columns.FromKey("SEP").Header.RowLayoutColumnInfo.OriginX = 0;
            Grid.Columns.FromKey("SEP").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("ValueSubject").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("ValueSubject").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("ValueSubject").Header.RowLayoutColumnInfo.OriginX = 1;
            Grid.Columns.FromKey("ValueSubject").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("GrowthSubject").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("GrowthSubject").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("GrowthSubject").Header.RowLayoutColumnInfo.OriginX = 2;
            Grid.Columns.FromKey("GrowthSubject").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("SpeedGrowthSubject").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("SpeedGrowthSubject").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("SpeedGrowthSubject").Header.RowLayoutColumnInfo.OriginX = 3;
            Grid.Columns.FromKey("SpeedGrowthSubject").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("RangFO").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("RangFO").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("RangFO").Header.RowLayoutColumnInfo.OriginX = 4;
            Grid.Columns.FromKey("RangFO").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("RangRF").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("RangRF").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("RangRF").Header.RowLayoutColumnInfo.OriginX = 5;
            Grid.Columns.FromKey("RangRF").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("ValueFO").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("ValueFO").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("ValueFO").Header.RowLayoutColumnInfo.OriginX = 6;
            Grid.Columns.FromKey("ValueFO").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("GrowthFO").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("GrowthFO").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("GrowthFO").Header.RowLayoutColumnInfo.OriginX = 7;
            Grid.Columns.FromKey("GrowthFO").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("SpeedGrowthFO").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("SpeedGrowthFO").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("SpeedGrowthFO").Header.RowLayoutColumnInfo.OriginX = 8;
            Grid.Columns.FromKey("SpeedGrowthFO").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("ValueRF").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("ValueRF").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("ValueRF").Header.RowLayoutColumnInfo.OriginX = 9;
            Grid.Columns.FromKey("ValueRF").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("GrowthRF").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("GrowthRF").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("GrowthRF").Header.RowLayoutColumnInfo.OriginX = 10;
            Grid.Columns.FromKey("GrowthRF").Header.RowLayoutColumnInfo.SpanX = 1;

            Grid.Columns.FromKey("SpeedGrowthRF").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("SpeedGrowthRF").Header.RowLayoutColumnInfo.OriginY = 1;
            Grid.Columns.FromKey("SpeedGrowthRF").Header.RowLayoutColumnInfo.OriginX = 11;
            Grid.Columns.FromKey("SpeedGrowthRF").Header.RowLayoutColumnInfo.SpanX = 1;


            ColumnHeader HeaderSubject = GenHeader(ComboRegion.SelectedValue,
                  1,
                  0,
                  5, 1);
            Grid.Bands[0].HeaderLayout.Add(HeaderSubject);

            ColumnHeader HeaderFO = GenHeader(ComboRegion.SelectedNodeParent,
                   6,
                   0,
                   3, 1);
            Grid.Bands[0].HeaderLayout.Add(HeaderFO);

            ColumnHeader HeadeRF = GenHeader("Российская Федерация",
                    9,
                    0,
                    3, 1);
            Grid.Bands[0].HeaderLayout.Add(HeadeRF);
            #endregion
        }

        protected void SetHeaderGrid()
        {
            SetColumnNameGrid();

            SetHeaderStyle();
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

        protected void SetColumnDisplay()
        {
            ColumnsCollection ColsGrid = Grid.Columns;

            ColsGrid.FromKey("ParentName").Hidden = true;

            ColsGrid.FromKey("UniqueName").Hidden = true;

            ColsGrid.FromKey("Reverce").Hidden = true;

            ColsGrid.FromKey("Unit").Hidden = true;

            ColsGrid.FromKey("Comporable").Hidden = true;

            ColsGrid.FromKey("PrevValueSubject").Hidden = true;

            ColsGrid.FromKey("PrevValueFO").Hidden = true;

            ColsGrid.FromKey("PrevValueRF").Hidden = true;

            ColsGrid.FromKey("SEP").CellStyle.Wrap = true;


            CRHelper.FormatNumberColumn(ColsGrid.FromKey("ValueSubject"), "N2");
            ColsGrid.FromKey("ValueSubject").CellStyle.Font.Bold = true;


            CRHelper.FormatNumberColumn(ColsGrid.FromKey("GrowthSubject"), "N2");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("SpeedGrowthSubject"), "### ### ##0.00%");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("RangFO"), "#####");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("RangRF"), "#####");


            CRHelper.FormatNumberColumn(ColsGrid.FromKey("ValueFO"), "N2");
            ColsGrid.FromKey("ValueFO").CellStyle.Font.Bold = true;


            CRHelper.FormatNumberColumn(ColsGrid.FromKey("GrowthFO"), "N2");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("SpeedGrowthFO"), "### ### ##0.00%");


            CRHelper.FormatNumberColumn(ColsGrid.FromKey("ValueRF"), "N2");
            ColsGrid.FromKey("ValueRF").CellStyle.Font.Bold = true;


            CRHelper.FormatNumberColumn(ColsGrid.FromKey("GrowthRF"), "N2");

            CRHelper.FormatNumberColumn(ColsGrid.FromKey("SpeedGrowthRF"), "### ### ##0.00%");
        }

        protected void SetColumnWidth()
        {
            CustomizerSize.ContfigurationGrid(Grid);
        }


        private static void AddedUnit(UltraGridRow row)
        {
            bool NoaddedUnit = true;
            string[] split = row.Cells.FromKey("SEP").Text.Split(',');
            if (row.Cells.FromKey("Unit").Value != null)
            {
                if (split.Length > 1)
                {
                    NoaddedUnit = (split[split.Length - 1] != " " + row.Cells.FromKey("Unit").Value.ToString().ToLower());
                }


                if (NoaddedUnit)
                    row.Cells.FromKey("SEP").Text += ", " + row.Cells.FromKey("Unit").Value.ToString().ToLower();
            }
             
            
        }

        private void SetStyleSepRow(UltraGridRow row)
        {
            AddedUnit(row);
            
                
                string Mounth = MounthHelper.GetDateDativeCase(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"]).ToLower();

                string Year = (int.Parse(DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Year"]) - 1).ToString();
                //string Year = ComboPeriod.SelectedValue;
                //string Mounth = ""; 

                row.Cells.FromKey("SpeedGrowthSubject").Title = string.Format("Темп роста к {1} {0} года",
                    Year, Mounth);
                row.Cells.FromKey("SpeedGrowthFO").Title = string.Format("Темп роста к {1} {0} года",
                    Year, Mounth);
                row.Cells.FromKey("SpeedGrowthRF").Title = string.Format("Темп роста к {1} {0} года",
                    Year, Mounth);

                row.Cells.FromKey("GrowthSubject").Title = string.Format("Прирост к {1} {0} года",
                    Year, Mounth);
                row.Cells.FromKey("GrowthFO").Title = string.Format("Прирост к {1} {0} года",
                    Year, Mounth);
                row.Cells.FromKey("GrowthRF").Title = string.Format("Прирост к {1} {0} года",
                    Year, Mounth);


                try
                { }
            catch { }


            bool inverce = row.Cells.FromKey("Reverce").Text == "1";
            CustomizeUltraGridHelper.SetImageFromGrowth(row, "SpeedGrowthSubject", inverce);
            CustomizeUltraGridHelper.SetImageFromGrowth(row, "SpeedGrowthFO", inverce);
            CustomizeUltraGridHelper.SetImageFromGrowth(row, "SpeedGrowthRF", inverce);
        }


        private void SetStyleTitleSepRow(UltraGridRow row)
        {
            row.Style.Font.Bold = true;
            row.Cells.FromKey("SEP").ColSpan = 12;
        }

        private void SetSepRow()
        {
            foreach (UltraGridRow row in Grid.Rows)
            {
                if (row.Cells.FromKey("UniqueName").Value == null)
                {
                    SetStyleTitleSepRow(row);
                }
                else
                {
                    SetStyleSepRow(row);
                }
            }
        }

        protected void ConfigurationRows()
        {

            

            CustomizeUltraGridHelper.SetStarFromValue(Grid, Grid.Columns.FromKey("RangRF").Index, 1, "~/images/starYellowBB.png", "Наименьшее отклонение");

            CustomizeUltraGridHelper.SetStarFromValue(Grid, Grid.Columns.FromKey("RangRF").Index, 83, "~/images/starGrayBB.png", "Наибольшее отклонение");

            CustomizeUltraGridHelper.SetStarFromValue(Grid, Grid.Columns.FromKey("RangFO").Index, 1, "~/images/starYellowBB.png", "Наименьшее отклонение");

            CustomizeUltraGridHelper.SetStarFromValue(Grid, Grid.Columns.FromKey("RangFO").Index, 
                MaxRangFromFiledFo
                , "~/images/starGrayBB.png", "Наибольшее отклонение");
            SetSepRow();
        }

        protected void SetDisplayGrid()
        {
            SetHeaderGrid();

            SetColumnDisplay();

            SetColumnWidth();

            ConfigurationRows();

            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
        }

        private void SetEventGrid()
        {
            Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
        }
        #endregion

        void ActionRowActive(UltraGridRow ActiveRow)
        {

            if (ActiveRow.Cells.FromKey("UniqueName").Value == null)
            {
                ActiveRow = ActiveRow.NextRow;
            }

            CurrentSepDetail = ActiveRow.Cells.FromKey("SEP").Value.ToString();

            ChosenDetail.Value = ActiveRow.Cells.FromKey("UniqueName").Value.ToString();

            CurrentUnit = ActiveRow.Cells.FromKey("Unit").Value.ToString();

            if (ActiveRow.Cells.FromKey("Reverce").Value != null)
            {
                CurentInverce = ActiveRow.Cells.FromKey("Reverce").Value.ToString() == "1";
            }

            if (ActiveRow.Cells.FromKey("Comporable").Value != null)
            {
                CurrentComparable = ActiveRow.Cells.FromKey("Comporable").Value.ToString() == "1";
            }

            

            GenerationChart();

            SetHeader();
            //IndexActiveRow.Value = ActiveRow.Index.ToString();
        }

        #region Chart
        abstract class IConfiguratoChart
        {
            protected UltraChart Chart = null;
            protected DataTable TableChart = null;
            protected string Unit = "";

            protected virtual void ConfChartAxisY()
            {
                Infragistics.UltraChart.Resources.Appearance.AxisAppearance AxisY = Chart.Axis.Y;

                AxisY.Labels.ItemFormatString = "<DATA_VALUE:### ### ### ##0.00>";

                AxisY.Labels.Font = new Font("Arial", 10);

                AxisY.Extent = 100;
            }

            protected virtual void ConfChartAxisX()
            {
                Infragistics.UltraChart.Resources.Appearance.AxisAppearance AxisX = Chart.Axis.X;
                AxisX.Extent = 120;

                AxisX.Labels.Font = new Font("Arial", 10);
                AxisX.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            }

            protected virtual void ConfChartToolTips()
            {
                Infragistics.UltraChart.Resources.Appearance.WebTooltipAppearance Tooltips = Chart.Tooltips;

                Tooltips.FormatString = "<SERIES_LABEL><br> <b><DATA_VALUE:00.##></b> " + Unit.ToLower();
            }

            public virtual void ConfLegend(ICustomizerSize CustomizerSize)
            {
                Chart.Legend.Visible = true;
                Chart.Legend.Margins.Right = 900;

                Chart.Legend.Font = new Font("Arial", 10);

                Chart.Legend.SpanPercentage = 24;
            }

            protected virtual void ConfColorModel()
            {
                Chart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            }

            protected IConfiguratoChart(UltraChart Chart, DataTable TableChart, string Unit)
            {
                this.Chart = Chart;
                this.TableChart = TableChart;
                this.Unit = Unit;
            }

            protected virtual void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
            {
                CRHelper.UltraChartInvalidDataReceived(sender, e);
            }

            public void SetNoDataMessage()
            {
                this.Chart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(InvalidDataReceived);
            }

            protected abstract void SetChartType();

            public abstract void DataBindChart();

            public void ConfChart()
            {
                this.SetChartType();
                this.ConfColorModel();
                this.ConfChartAxisY();
                this.ConfChartAxisX();
                this.ConfChartToolTips();

                this.SetNoDataMessage();
            }

        }

        class ConfiguratorComparableChart : IConfiguratoChart
        {

            public ConfiguratorComparableChart(UltraChart Chart, DataTable TableChart, string Unit)
                : base(Chart, TableChart, Unit)
            {
                Chart.Data.SwapRowsAndColumns = true;
            }

            public override void DataBindChart()
            {
                this.TableChart = DataBaseHelper.ExecQueryByID("ChartComparable", "Date");

                DataTableHelper.ClearDataLevel(TableChart, "Date");

                TableChart.Columns.Remove("Date");

                Chart.DataSource = TableChart;

                Chart.DataBind();
            }

            protected override void SetChartType()
            {
                Chart.ChartType = ChartType.LineChart;
            }

            public override void ConfLegend(ICustomizerSize CusctomizerSize)
            {
                base.ConfLegend(CusctomizerSize);
                Chart.Legend.SpanPercentage = 24;
                Chart.Legend.Margins.Right = CusctomizerSize.GetChartLegendWidth(false);
            }

            protected override void ConfChartToolTips()
            {
                base.ConfChartToolTips();
                Chart.Tooltips.FormatString = "<ITEM_LABEL><br><SERIES_LABEL> года<br><b><DATA_VALUE:### ##0.00></b> " + Unit.ToLower();
            }
        }

        class ConfiguratorDynamicChart : IConfiguratoChart
        {
            string region = "";
            public ConfiguratorDynamicChart(UltraChart Chart, DataTable TableChart, string Unit, string region)
                : base(Chart, TableChart, Unit)
            {
                Chart.Data.SwapRowsAndColumns = false;
                this.region = region;
            }

            private DataTable TransformationDataTable(DataTable ConvertibleTable)
            {
                DataTable TransformedTable = new DataTable();
                TransformedTable.Columns.Add("Year");
                string LastYear = "";
                DataRow LastRow = null;

                TransformedTable.Columns.Add("Январь", typeof(decimal));
                TransformedTable.Columns.Add("Февраль", typeof(decimal));
                TransformedTable.Columns.Add("Март", typeof(decimal));
                TransformedTable.Columns.Add("Апрель", typeof(decimal));
                TransformedTable.Columns.Add("Май", typeof(decimal));
                TransformedTable.Columns.Add("Июнь", typeof(decimal));
                TransformedTable.Columns.Add("Июль", typeof(decimal));
                TransformedTable.Columns.Add("Август", typeof(decimal));
                TransformedTable.Columns.Add("Сентябрь", typeof(decimal));
                TransformedTable.Columns.Add("Октябрь", typeof(decimal));
                TransformedTable.Columns.Add("Ноябрь", typeof(decimal));
                TransformedTable.Columns.Add("Декабрь", typeof(decimal));

                foreach (DataRow Row in ConvertibleTable.Rows)
                {

                    string CurYear = Row["Year"].ToString();

                    if (CurYear != LastYear)
                    {
                        LastRow = TransformedTable.NewRow();
                        TransformedTable.Rows.Add(LastRow);

                        LastRow["Year"] = CurYear;
                        LastYear = CurYear;
                    }

                    string CurMunth = Row["Mounth"].ToString();
                    if (TransformedTable.Columns.Contains(CurMunth))
                    {
                        LastRow[CurMunth] = Row["ValueSubject"];
                    }

                }
                return TransformedTable;

            }

            protected override void ConfChartAxisX()
            {
                base.ConfChartAxisX();
                Chart.Axis.X.Extent = 80;
            }

            protected override void SetChartType()
            {
                Chart.ChartType = ChartType.LineChart;
            }

            public override void DataBindChart()
            {
                Chart.ChartDrawItem += new ChartDrawItemEventHandler(Chart_ChartDrawItem);

                TableChart = DataBaseHelper.ExecQueryByID("ChartDynamic", "Mounth");

                DataTableHelper.ClearDataLevel(TableChart, "Mounth");

                TableChart = TransformationDataTable(TableChart);

                Chart.DataSource = TableChart;

                Chart.DataBind();
            }

            void Chart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
            {
                Text text = e.Primitive as Text;
                if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
                {
                    int outb;
                    if (int.TryParse(text.GetTextString(), out outb))
                    {
                        text.SetTextString(text.GetTextString() + " год");
                        text.bounds.Width *= 2;
                    }
                }
            }

            public override void ConfLegend(ICustomizerSize CusctomizerSize)
            {
                base.ConfLegend(CusctomizerSize);
                Chart.Legend.SpanPercentage = 10;
                Chart.Legend.Margins.Right = 890;
            }

            protected override void ConfChartToolTips()
            {
                base.ConfChartToolTips();
                Chart.Tooltips.FormatString = region + "<br><ITEM_LABEL> <SERIES_LABEL> года<br><b><DATA_VALUE:### ##0.00></b> " + Unit.ToLower();
            }
        }

        private void GenerationChart()
        {
            //TODO применить патерн стратегия

            IConfiguratoChart ConfiguratorChart =
                CurrentComparable ?
                (IConfiguratoChart)new ConfiguratorComparableChart(Chart, TableChart, CurrentUnit) :
                (IConfiguratoChart)new ConfiguratorDynamicChart(Chart, TableChart, CurrentUnit, ComboRegion.SelectedValue);

            ConfiguratorChart.DataBindChart();

            ConfiguratorChart.ConfChart();

            ConfiguratorChart.ConfLegend(CustomizerSize);

            Chart.LineChart.NullHandling = NullHandling.DontPlot;
        }
        #endregion

        protected void SetHeader()
        {
            Hederglobal.Text = string.Format("Ежемесячный мониторинг социально-экономического развития территории");

            Page.Title = Hederglobal.Text;

            PageSubTitle.Text = string.Format(@"Анализ социально-экономического положения субъекта РФ по основным направлениям развития, <b>{2}</b>, по состоянию на <b>{1} {0} года</b>",
                            DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Year"],
                            DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"].ToLower(),
                            ComboRegion.SelectedValue);

            if (string.IsNullOrEmpty(ChosenDetail.Value))
            {
                HeaderChart.Text = "";
            }
            else
            {
                HeaderChart.Text = string.Format("Динамика показателя «{0}», {1} ({2})", UserComboBox.getLastBlock(ChosenDetail.Value), CurrentUnit.ToLower(), ComboRegion.SelectedValue);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            ConfigurationMultiCombo();

            ChosenParam();

            DataBindGrid();

            if (TableGrid.Rows.Count > 0)
            {
                SetDisplayGrid();

                SetEventGrid();

                if (Grid.Rows.Count > 0)
                {
                    ActionRowActive(Grid.Rows[0]);
                }
            }
            else
            {
                DataBindEmptyGrid();
                Grid.DisplayLayout.NoDataMessage = "Нет данных";
                Chart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(Chart_InvalidDataReceived);
            }
            SetHeader();
        }

        void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActionRowActive(e.Row);
            IndexActiveRow.Value = e.Row.Index.ToString();

        }

        #region TODO пЕРЕДЕЛАТЬ ЕКСПОРТ
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 12 * 20;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text.Replace("<b>", "").Replace("</b>", "");
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            ExportGridToExcel(Grid, e.CurrentWorksheet.Workbook.Worksheets["Таблица"], 3, true);
            try
            {
                e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].Value = HeaderChart.Text; 
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0], Chart);

                e.Workbook.Worksheets["Таблица"].Rows[0].Cells[0].Value = Hederglobal.Text;
            }
            catch { }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            if (IndexActiveRow.Value != "")
                ActionRowActive(Grid.Rows[int.Parse(IndexActiveRow.Value)]);
            GenerationChart();

            foreach (UltraGridColumn col in Grid.Columns)
            {
                if ((col.Hidden) || (col.Width.Value == 0))
                {
                    Grid.Bands[0].HeaderLayout.Remove(col.Header);
                    Grid.Columns.Remove(col);
                }
            }


            Chart.Width = (int)(Chart.Width.Value * 0.75);
            Chart.Legend.Margins.Right = (int)(Chart.Legend.Margins.Right * 0.5);

            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 6;
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
                WorkSheet.Rows[i].Height = 20 * 40;
            }

            return maxRow;

        }

        void ExportGridToExcel(UltraWebGrid G, Worksheet sheet, int startrow, bool RowZebra)
        {
            startrow = ExportHeaderGrid(G.Bands[0].HeaderLayout, sheet, startrow);
            for (int i = 0; i < G.Rows.Count; i++)
            {
                sheet.Rows[i + startrow].Height = 22 * 40;
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
                    
                }
            }
            sheet.Columns[4].CellFormat.FormatString = "### ### ##0";
            sheet.Columns[5].CellFormat.FormatString = "### ### ##0";

            sheet.Columns[0].Width = 200 * 36;
            for (int i = 1; i < 20; i++)
            {
                sheet.Columns[i].Width = 90 * 36;
            }
        }

        #endregion
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            Report r = new Report();

            ISection e_ = r.AddSection();
            e_.PageSize = new PageSize(1000, 600);
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
            title.AddContent(PageSubTitle.Text.Replace("<b>", "").Replace("</b>", ""));

            Grid.Bands[0].HeaderLayout.Clear();
            UltraGridExporter1.PdfExporter.Export(Grid, e_);
            ISection is_ = r.AddSection();
            is_.PageSize = new PageSize(950, 500);
            title = is_.AddText();
            title.Margins = new Infragistics.Documents.Reports.Report.Margins(40, 40, 40, 40);
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;

            Chart.Height = 400;

            //Infragistics.Documents.Graphics.Image ima = new Infragistics.Documents.Graphics.Image(new 
            //Chart.SaveTo(
            if (IndexActiveRow.Value != "")
                ActionRowActive(Grid.Rows[int.Parse(IndexActiveRow.Value)]);
            GenerationChart();
            title.AddContent(HeaderChart.Text);
            MemoryStream imageStream = new MemoryStream();
            Chart.SaveTo(imageStream, ImageFormat.Png);
            Infragistics.Documents.Reports.Graphics.Image img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);

            IImage ima = is_.AddImage(img);
            ima.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 0, 5, 0);

            title = is_.AddText();
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

            title.Alignment.Horizontal = Alignment.Center;


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

        int CountPDFheaderLevel = 2;

        int HeightLevel = 20;

        int[] PDFHeaderHeightsLevel = { 15, 30 };


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

            ITableRow RootRow = Table.AddRow();

            ApplyHeader();
            SetDisplayGrid();
            int sumW = 0;

            ITableRow SelectorCol = RootRow.AddCell().AddTable().AddRow();

            AddTableCell(SelectorCol, ".", 16, PDFGetLevelHeight(0, 2));

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
                if ((col.Hidden))
                {
                    continue;
                }
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

            AddTableCell(row, ComboRegion.SelectedValue, WidthSubject, 10);

            Double WidthFO =
                Grid.Columns.FromKey("ValueFO").Width.Value * 0.75 +
                Grid.Columns.FromKey("GrowthFO").Width.Value * 0.75 +
                Grid.Columns.FromKey("SpeedGrowthFO").Width.Value * 0.75;
            AddTableCell(row, ComboRegion.SelectedNodeParent, WidthFO, 10);


            Double WidthRF =
                Grid.Columns.FromKey("ValueRF").Width.Value * 0.75 +
                Grid.Columns.FromKey("GrowthRF").Width.Value * 0.75 +
                Grid.Columns.FromKey("SpeedGrowthRF").Width.Value * 0.75;
            AddTableCell(row, "Российская федерация", WidthRF, 10);



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
                width = 0.75 * (int)header.Column.Width.Value;
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

            text.AddContent(header.Caption);

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



            //cellText = cellText.Replace("&nbsp;", " ");
            //cellText = cellText.Replace("<br/>", Environment.NewLine);

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

        protected void Chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            CRHelper.UltraChartInvalidDataReceived(sender, e);
        }


    }
}
