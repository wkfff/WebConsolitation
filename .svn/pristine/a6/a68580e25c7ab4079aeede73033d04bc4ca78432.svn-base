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
using Microsoft.AnalysisServices.AdomdClient;

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
using Infragistics.UltraChart.Resources.Appearance;


namespace Krista.FM.Server.Dashboards.reports.HMAO_ARC_001
{
    public partial class _default : CustomReportPage
    {
        int ScreenWidth { get { return ((int)Session[CustomReportConst.ScreenWidthKeyName]); } }
        int ScreenHeight { get { return ((int)Session[CustomReportConst.ScreenHeightKeyName]); } }

        string ReportTextNoCompareble = @"&nbsp&nbspКоличество умерших людей от всех видов болезней в целом по ХМАО-Югре в <b>{0}</b> составило <b>{1}</b> человек.
<br>&nbsp&nbspПо виду болезни ''{2}'' умерло  максимальное количество <b>{3}</b> человек.";

        string ReportTextCompareble = @"&nbsp&nbspКоличество умерших людей от всех видов болезней в целом по ХМАО-Югре в <b>{0}</b> по сравнению с <b>{1}</b>&nbsp {2} на <b>{3}</b> человек и составило <b>{4}</b> человек.
<br>&nbsp&nbspПо виду болезни ''{5}'' умерло  максимальное количество <b>{6}</b> человек.";

        ICustomizerSize CustomizerSize;

        CustomParam CurrentDate { get { return (UserParams.CustomParam("CurrentDate")); } }

        CustomParam CurrentDateLineChart { get { return (UserParams.CustomParam("CurrentDateLineChart")); } }



        CustomParam ComparisonDate { get { return (UserParams.CustomParam("ComparisonDate")); } }

        CustomParam SelectLevel { get { return (UserParams.CustomParam("SelectLevel")); } }

        CustomParam SelectUniqName { get { return (UserParams.CustomParam("SelectUniqName")); } }

        DataTable GridDataTable = new DataTable();

        DataTable TableChart = new DataTable();

        DataTable TablePointChart = new DataTable();

        IDataSetCombo DataComboPeriod = new DataSetComboTwoLevel();

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
                //String.Format(Width.

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
                    if (col.Header != null)
                        if (col.Header.Caption.Contains("Значение"))
                        {
                            col.Width = (int)(onePercent * 13);
                        }
                        else
                        {
                            col.Width = onePercent * 7;
                        }

                }
                Grid.Columns.FromKey("Disease").Width = (int)(onePercent * 20.0);
                Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header != null)
                        if (col.Header.Caption.Contains("Значение"))
                        {
                            col.Width = (int)(onePercent * 13);
                        }
                        else
                        {
                            col.Width = onePercent * 7;
                        }

                }
                Grid.Columns.FromKey("Disease").Width = (int)(onePercent * 20.0);
                Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header != null)
                        if (col.Header.Caption.Contains("Значение"))
                        {
                            col.Width = (int)(onePercent * 13);
                        }
                        else
                        {
                            col.Width = onePercent * 7;
                        }

                }
                Grid.Columns.FromKey("Disease").Width = (int)(onePercent * 25.5);
                Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
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
                    if (col.Header != null)
                        if (col.Header.Caption.Contains("Значение"))
                        {
                            col.Width = onePercent * 8;
                        }
                        else
                        {
                            col.Width = onePercent * 7;
                        }

                }
                Grid.Columns.FromKey("Disease").Width = onePercent * 37;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header != null)
                        if (col.Header.Caption.Contains("Значение"))
                        {
                            col.Width = onePercent * 8;
                        }
                        else
                        {
                            col.Width = onePercent * 7;
                        }

                }
                Grid.Columns.FromKey("Disease").Width = onePercent * 36 + 3;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                foreach (UltraGridColumn col in Grid.Columns)
                {
                    if (col.Header != null)
                        if (col.Header.Caption.Contains("Значение"))
                        {
                            col.Width = onePercent * 8;
                        }
                        else
                        {
                            col.Width = onePercent * 7;
                        }

                }
                Grid.Columns.FromKey("Disease").Width = onePercent * 38 + 3;
            }

            #endregion

        }
        #endregion

        static class DataBaseHelper
        {
            static DataProvider ActiveDataPorvider = DataProvidersFactory.SpareMASDataProvider;

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
                    string Child = row["Child"].ToString();

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
                    string Child = row["Child"].ToString();
                    string Parent = row["Parent"].ToString();

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

            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            string BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._1280x1024)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution._800x600)
            {
                CustomizerSize = new CustomizerSize_800x600(ICustomizerSize.GetBrouse(BN));
            }

            if (ICustomizerSize.GetScreenResolution(CRHelper.GetScreenWidth) == ICustomizerSize.ScreenResolution.Default)
            {
                CustomizerSize = new CustomizerSize_1280x1024(ICustomizerSize.GetBrouse(BN));
            }

            Grid.Width = CustomizerSize.GetGridWidth();
            Grid.Height = 500;

            Chart.Width = CustomizerSize.GetChartWidth();

            ChartPoint.Width = CustomizerSize.GetChartWidth();

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExportButton.Visible = false;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_Test);


        }

        enum typeValue { Value, DeviationValue, SpeedDeviationValue };

        #region DataBindGrid

        Dictionary<DataColumn, Dictionary<string, decimal>> SumLevel2 = new Dictionary<DataColumn, Dictionary<string, decimal>>();
        Dictionary<DataColumn, decimal> SumLevel1 = new Dictionary<DataColumn, decimal>();



        void saveSum(DataRow NewRow)
        {
            foreach (DataColumn col in NewRow.Table.Columns)
            {
                if (SumLevel1.ContainsKey(col))
                    if ((NewRow[col] != DBNull.Value))
                    {
                        string parentName = NewRow["parentName"].ToString();
                        if (!SumLevel2[col].ContainsKey(parentName))
                        {
                            SumLevel2[col].Add(parentName, (decimal)NewRow[col]);
                        }
                        else
                        {
                            SumLevel2[col][parentName] += (decimal)NewRow[col];
                        }

                    }
            }

        }

        bool GridCompareble()
        {
            string SelectDate = ComboPeriod.SelectedValue;
            string ComparisonDate = DateHelper.PrevDateGridCaption;
            return SelectDate != ComparisonDate;
        }

        private void BindTableColumn()
        {
            Grid.Bands.Clear();

            GridDataTable.Columns.Add("TypeValue", typeof(string));

            GridDataTable.Columns.Add("DiseaseLevel", typeof(string));

            GridDataTable.Columns.Add("DiseaseUniqueName", typeof(string));

            GridDataTable.Columns.Add("parentName", typeof(string));


            GridDataTable.Columns.Add("Disease", typeof(string));
            DataColumn LAddedCol =
            GridDataTable.Columns.Add("Death", typeof(decimal));
            SumLevel2.Add(LAddedCol, new Dictionary<string, decimal>());
            SumLevel1.Add(LAddedCol, 0);

            GridDataTable.Columns.Add("DeathPart", typeof(decimal));

            if (GridCompareble())
            {
                LAddedCol =
                GridDataTable.Columns.Add("DeathComparison", typeof(decimal));
                SumLevel2.Add(LAddedCol, new Dictionary<string, decimal>());
                SumLevel1.Add(LAddedCol, 0);


                GridDataTable.Columns.Add("DeathComparisonPart", typeof(decimal));
            }

            LAddedCol =
            GridDataTable.Columns.Add("DeathChildren", typeof(decimal));
            SumLevel2.Add(LAddedCol, new Dictionary<string, decimal>());
            SumLevel1.Add(LAddedCol, 0);



            GridDataTable.Columns.Add("DeathChildrenPart", typeof(decimal));

            if (GridCompareble())
            {

                LAddedCol =
                GridDataTable.Columns.Add("DeathChildrenComparison", typeof(decimal));
                SumLevel2.Add(LAddedCol, new Dictionary<string, decimal>());
                SumLevel1.Add(LAddedCol, 0);

                GridDataTable.Columns.Add("DeathChildrenComparisonPart", typeof(decimal));
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

        void GenerateText()
        {
            try
            {
                if (GridCompareble())
                {
                    decimal Deviation = SumLevel2[GridDataTable.Columns["Death"]]["Все виды заболеваний"] - SumLevel2[GridDataTable.Columns["DeathComparison"]]["Все виды заболеваний"];

                    bool up = Deviation > 0;
                    string imageName = "<img src='../../images/arrow" + (up ? "RedUp" : "GreenDown") + "BB.png'>";


                    ReportTextL.Text = string.Format(ReportTextCompareble,
                        CaseMounth.GetPrepositioneDate(ComboPeriod.SelectedValue.ToLower()),
                        CaseMounth.GetInstrumentativeDate(DateHelper.PrevDateGridCaption.ToLower()),
                        imageName + " " + (up ? "выросло" : "уменшилось"),
                        Math.Abs(Deviation),
                        SumLevel2[GridDataTable.Columns["Death"]]["Все виды заболеваний"],
                        maxDeathName,
                        maxDeath);
                }
                else
                {
                    ReportTextL.Text = string.Format(ReportTextNoCompareble,
                        CaseMounth.GetPrepositioneDate(ComboPeriod.SelectedValue.ToLower()),
                        SumLevel2[GridDataTable.Columns["Death"]]["Все виды заболеваний"],
                        maxDeathName,
                        maxDeath);

                }
                TableText.Visible = true; ;
            }
            catch
            {
                ReportTextL.Text = "";
                TableText.Visible = false;
            }
        }




        private DataRow NewGridTableRow()
        {
            DataRow NewRow = GridDataTable.NewRow();
            GridDataTable.Rows.Add(NewRow);
            return NewRow;
        }

        decimal maxDeath = -100;
        string maxDeathName = "";
        private void AddValueRow(DataRow BaseRow)
        {
            #region GetMax
            if (maxDeath < (decimal)BaseRow["Death"])
            {

                maxDeath = (decimal)BaseRow["Death"];
                maxDeathName = BaseRow["Disease"].ToString();


            }
            #endregion

            DataRow NewRow = NewGridTableRow();
            NewRow["TypeValue"] = typeValue.Value;
            foreach (DataColumn Col in BaseRow.Table.Columns)
            {
                if (GridDataTable.Columns.Contains(Col.ColumnName))
                {
                    NewRow[Col.ColumnName] = BaseRow[Col];
                }
            }

            saveSum(NewRow);

        }

        private void AddDeviationRow(DataRow BaseRow)
        {
            DataRow NewRow = NewGridTableRow();
            NewRow["TypeValue"] = typeValue.DeviationValue;
            if (GridCompareble())
            {
                if ((BaseRow["DeathChildren"] != DBNull.Value) && (BaseRow["DeathChildrenComparison"] != DBNull.Value))
                {
                    NewRow["DeathChildren"] = (decimal)(BaseRow["DeathChildren"]) - (decimal)(BaseRow["DeathChildrenComparison"]);
                }
                if ((BaseRow["Death"] != DBNull.Value) && (BaseRow["DeathComparison"] != DBNull.Value))
                {
                    NewRow["Death"] = (decimal)(BaseRow["Death"]) - (decimal)(BaseRow["DeathComparison"]);
                }
            }
        }

        private void AddSpeedDeviationRow(DataRow BaseRow)
        {
            DataRow NewRow = NewGridTableRow();
            NewRow["TypeValue"] = typeValue.SpeedDeviationValue;

            if (GridCompareble())
            {

                if ((BaseRow["DeathChildren"] != DBNull.Value) && (BaseRow["DeathChildrenComparison"] != DBNull.Value))
                {
                    if ((decimal)BaseRow["DeathChildrenComparison"] != 0)
                    {
                        NewRow["DeathChildren"] = (decimal)(BaseRow["DeathChildren"]) / (decimal)(BaseRow["DeathChildrenComparison"]) - 1;
                    }
                    else
                    {
                        NewRow["DeathChildren"] = (decimal)0;
                    }
                }
                if ((BaseRow["Death"] != DBNull.Value) && (BaseRow["DeathComparison"] != DBNull.Value))
                {
                    if ((decimal)BaseRow["DeathComparison"] != 0)
                    {
                        NewRow["Death"] = (decimal)(BaseRow["Death"]) / (decimal)(BaseRow["DeathComparison"]) - 1;
                    }
                    else
                    {
                        NewRow["Death"] = (decimal)0;
                    }
                }
            }
        }



        private void ExportRows(DataTable BaseTable)
        {
            foreach (DataRow BaseRow in BaseTable.Rows)
            {

                AddValueRow(BaseRow);
                AddDeviationRow(BaseRow);
                AddSpeedDeviationRow(BaseRow);
            }
        }

        void SetPart()
        {
            DataColumnCollection Cols = GridDataTable.Columns;

            foreach (DataRow row in GridDataTable.Rows)
            {
                if (row["TypeValue"].ToString() == "Value")
                {
                    int FirstNumericCol = GridDataTable.Columns.IndexOf("Death");
                    for (int i = FirstNumericCol; i < FirstNumericCol + 8; i += 2)
                    {
                        try
                        {
                            if (row[i] != DBNull.Value)
                            {
                                row[i + 1] = (decimal)row[i] / SumLevel2[GridDataTable.Columns[i]][row["parentName"].ToString()];
                            }
                        }
                        catch { }
                    }
                }
            }


        }

        private void DataBindGrid()
        {
            BindTableColumn();

            DataTable BaseTable = DataBaseHelper.ExecQueryByID("Grid", "Disease_");

            ExportRows(BaseTable);

            SetPart();

            Grid.DataSource = GridDataTable;

            Grid.DataBind();
        }
        #endregion

        #region SettingDisplayGrid
        private void SetColumnName()
        {
            Grid.Columns.FromKey("Disease").Header.Caption = "Виды болезней";

            Grid.Columns.FromKey("Death").Header.Caption = "Значение, чел.";

            Grid.Columns.FromKey("DeathPart").Header.Caption = "Доля %";

            if (GridCompareble())
            {
                Grid.Columns.FromKey("DeathComparison").Header.Caption = "Значение, чел.";

                Grid.Columns.FromKey("DeathComparisonPart").Header.Caption = "Доля %";
            }

            Grid.Columns.FromKey("DeathChildren").Header.Caption = "Значение, чел.";

            Grid.Columns.FromKey("DeathChildrenPart").Header.Caption = "Доля %";

            if (GridCompareble())
            {
                Grid.Columns.FromKey("DeathChildrenComparison").Header.Caption = "Значение, чел.";

                Grid.Columns.FromKey("DeathChildrenComparisonPart").Header.Caption = "Доля %";
            }
        }

        protected void SetDefaultStyleHeader(ColumnHeader header)
        {
            GridItemStyle HeaderStyle = header.Style;

            HeaderStyle.Wrap = true;

            HeaderStyle.VerticalAlign = VerticalAlign.Middle;

            HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        private void AddGridHeader(string Caption, int x, int y, int spanX, int SpanY)
        {
            ColumnHeader Header = new ColumnHeader();

            Header.Caption = Caption;
            Header.RowLayoutColumnInfo.OriginX = x;
            Header.RowLayoutColumnInfo.OriginY = y;
            Header.RowLayoutColumnInfo.SpanX = spanX;
            Header.RowLayoutColumnInfo.SpanY = SpanY;
            SetDefaultStyleHeader(Header);
            Grid.Bands[0].HeaderLayout.Add(Header);
        }

        private void SetHeaderPos(ColumnHeader Header, int x, int y, int spanX, int SpanY)
        {
            Header.RowLayoutColumnInfo.OriginX = x;
            Header.RowLayoutColumnInfo.OriginY = y;
            Header.RowLayoutColumnInfo.SpanX = spanX;
            Header.RowLayoutColumnInfo.SpanY = SpanY;

            Header.Style.HorizontalAlign = HorizontalAlign.Center;
        }

        private void SetDispalyHeader()
        {
            Grid.Columns.FromKey("TypeValue").Hidden = true;
            Grid.Bands[0].HeaderLayout.Remove(Grid.Columns.FromKey("TypeValue").Header);

            Grid.Columns.FromKey("DiseaseLevel").Hidden = true;
            Grid.Bands[0].HeaderLayout.Remove(Grid.Columns.FromKey("DiseaseLevel").Header);

            Grid.Columns.FromKey("DiseaseUniqueName").Hidden = true;
            Grid.Bands[0].HeaderLayout.Remove(Grid.Columns.FromKey("DiseaseUniqueName").Header);

            Grid.Columns.FromKey("parentName").Hidden = true;

            if (GridCompareble())
            {
                SetHeaderPos(Grid.Columns.FromKey("Disease").Header, 0, 0, 1, 3);

                SetHeaderPos(Grid.Columns.FromKey("Death").Header, 1, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathPart").Header, 2, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathComparison").Header, 3, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathComparisonPart").Header, 4, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathChildren").Header, 5, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathChildrenPart").Header, 6, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathChildrenComparison").Header, 7, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathChildrenComparisonPart").Header, 8, 2, 1, 1);
            }
            else
            {
                SetHeaderPos(Grid.Columns.FromKey("Disease").Header, 0, 0, 1, 3);

                SetHeaderPos(Grid.Columns.FromKey("Death").Header, 1, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathPart").Header, 2, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathChildren").Header, 3, 2, 1, 1);

                SetHeaderPos(Grid.Columns.FromKey("DeathChildrenPart").Header, 4, 2, 1, 1);
            }

            string SelectDate = ComboPeriod.SelectedValue;
            string ComparisonDate = DateHelper.PrevDateGridCaption;

            if (GridCompareble())
            {
                AddGridHeader("Число умерших", 1, 0, 4, 1);

                AddGridHeader(SelectDate, 1, 1, 2, 1);
                AddGridHeader(ComparisonDate, 3, 1, 2, 1);


                AddGridHeader("Число умерших детей в возрасте до 1 года", 5, 0, 4, 1);

                AddGridHeader(SelectDate, 5, 1, 2, 1);
                AddGridHeader(ComparisonDate, 7, 1, 2, 1);
            }
            else
            {
                AddGridHeader("Число умерших", 1, 0, 2, 1);

                AddGridHeader(SelectDate, 1, 1, 2, 1);
                //AddGridHeader(ComparisonDate, 2, 1, 2, 1);


                AddGridHeader("Число умерших детей в возрасте до 1 года", 3, 0, 2, 1);

                AddGridHeader(SelectDate, 3, 1, 2, 1);
                //AddGridHeader(ComparisonDate, 7, 1, 2, 1);
            }


        }

        private void SettingHeader()
        {
            SetColumnName();
            SetDispalyHeader();

        }

        private void SetCellStyle()
        {
            foreach (UltraGridColumn Col in Grid.Columns)
            {
                if (Col.Header.Caption.Contains("Доля"))
                {
                    CRHelper.FormatNumberColumn(Col, "### ### ##0.00%");
                }

                if (Col.Header.Caption.Contains("Значение"))
                {
                    CRHelper.FormatNumberColumn(Col, "### ### ##0");
                }
            }

            Grid.Columns.FromKey("Disease").CellStyle.Wrap = true;
        }

        void FormatCellLevel1(UltraGridCell cell)
        {
            cell.Style.Padding.Left = 1;
            cell.Style.Font.Bold = true;
        }

        void FormatCellLevel2(UltraGridCell cell)
        {
            cell.Style.Padding.Left = 15;
            cell.Style.Font.Italic = true;
        }

        private void SetValueRow(UltraGridRow Row)
        {
            if (int.Parse(Row.Cells.FromKey("DiseaseLevel").Value.ToString()) > 1)
            {
                FormatCellLevel2(Row.Cells.FromKey("Disease"));
            }
            else
            {
                FormatCellLevel1(Row.Cells.FromKey("Disease"));
            }

            foreach (UltraGridCell Cell in Row.Cells)
            {
                if (Cell.Column.Header != null)
                {
                    if (Grid.Rows[Cell.Row.Index + 1].Cells[Cell.Column.Index].Value == null)
                    {
                        Cell.RowSpan = 3;
                    }
                    else
                    {
                        Cell.Style.BorderDetails.ColorBottom = Color.Transparent;
                    }
                }

            }

        }

        string CaseTypeDeiation(string type)
        {
            if (type == "Предыдущий период")
            {
                return " предыдущего периода";
            }
            if (type == "На начало года")
            {
                return " начало года";
            }
            if (type == "На аналогичный период прошлого года")
            {
                return " аналогичного периода прошлого года";
            }
            return "";

        }

        string CaseTypeDeiation2(string type)
        {
            if (type == "Предыдущий период")
            {
                return " предыдущему периоду";
            }
            if (type == "На начало года")
            {
                return " началу года";
            }
            if (type == "На аналогичный период прошлого года")
            {
                return " аналогичному периоду прошлого года";
            }
            return "";

        }

        void SetImageFromCell(UltraGridCell Cell, bool up)
        {
            Cell.Title = (!up ? "снижение" : "прирост") + " относительно" + CaseTypeDeiation(ComboTypeDate.SelectedValue);

            string imageName = "arrow" + (up ? "RedUp" : "GreenDown") + "BB.png";
            Cell.Style.BackgroundImage = "~/images/" + imageName;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
        }

        private void SetDeviationRow(UltraGridRow Row)
        {
            foreach (UltraGridCell Cell in Row.Cells)
            {
                Cell.Style.BorderDetails.ColorTop = Color.Transparent;
                Cell.Style.BorderDetails.ColorBottom = Color.Transparent;
                if (Cell.Value != null)
                {
                    decimal valCell;
                    if (decimal.TryParse(Cell.Value.ToString(), out valCell))
                    {
                        if (valCell != 0)
                            SetImageFromCell(Cell, valCell > 0);
                    }
                }
            }
        }

        private void SetSpeedDeviationRow(UltraGridRow Row)
        {
            foreach (UltraGridCell Cell in Row.Cells)
            {
                Cell.Style.BorderDetails.ColorTop = Color.Transparent;
                if (Cell.Value != null)
                {
                    decimal valCell;
                    if (decimal.TryParse(Cell.Value.ToString(), out valCell))
                    {
                        Cell.Value = string.Format("{0:### ##0.00%}", valCell);
                        Cell.Title = "темп прироста к " + CaseTypeDeiation2(ComboTypeDate.SelectedValue);
                    }
                }
            }
        }

        void DispalyRow()
        {
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells.FromKey("TypeValue").Value.ToString() == typeValue.Value.ToString())
                {
                    SetValueRow(Row);
                }
                if (Row.Cells.FromKey("TypeValue").Value.ToString() == typeValue.DeviationValue.ToString())
                {
                    SetDeviationRow(Row);
                }
                if (Row.Cells.FromKey("TypeValue").Value.ToString() == typeValue.SpeedDeviationValue.ToString())
                {
                    SetSpeedDeviationRow(Row);
                }
                Row.Style.BackColor = Color.Transparent;
            }


        }

        private void SetColumnWidth()
        {
            CustomizerSize.ContfigurationGrid(Grid);


        }

        private void SetStyleGrid()
        {
            Grid.DisplayLayout.NullTextDefault = "-";
        }

        private void setGridEvent()
        {
            Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
        }

        void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            if (e.Row.Cells.FromKey("TypeValue").Value.ToString() == typeValue.DeviationValue.ToString())
            {
                ActiveRow(e.Row.PrevRow);

            }
            else
                if (e.Row.Cells.FromKey("TypeValue").Value.ToString() == typeValue.SpeedDeviationValue.ToString())
                {
                    ActiveRow(e.Row.PrevRow.PrevRow);
                }
                else
                {
                    ActiveRow(e.Row);
                }
        }

        private void ConfigurationDispalyGrid()
        {
            SettingHeader();
            SetCellStyle();
            DispalyRow();
            SetColumnWidth();
            SetStyleGrid();

            setGridEvent();

            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
        }







        #endregion

        class DateHelper
        {
            string prevMounth = "";
            string prevYear = "";

            public static string PrevDateGridCaption = "xnia";

            private static Dictionary<string, string> MounthDict = null;
            public static Dictionary<string, string> GetDictMounth()
            {
                if ((MounthDict == null) || (MounthDict.Count < 1))
                {
                    MounthDict = new Dictionary<string, string>();
                    MounthDict.Add("Январь".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Январь]");
                    MounthDict.Add("Февраль".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Февраль]");
                    MounthDict.Add("Март".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 1].[Март]");
                    MounthDict.Add("Апрель".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Апрель]");
                    MounthDict.Add("Май".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Май]");
                    MounthDict.Add("Июнь".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 1].[Квартал 2].[Июнь]");
                    MounthDict.Add("Июль".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Июль]");
                    MounthDict.Add("Август".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Август]");
                    MounthDict.Add("Сентябрь".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 3].[Сентябрь]");
                    MounthDict.Add("Октябрь".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Октябрь]");
                    MounthDict.Add("Ноябрь".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Ноябрь]");
                    MounthDict.Add("Декабрь".ToLower(), "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[{0}].[Полугодие 2].[Квартал 4].[Декабрь]");
                }
                return MounthDict;
            }

            string BaseDateUniqueName;
            string Year;
            string Mounth;

            public string GetFirstDateFromYear()
            {
                return GetUniqueDate(Year, "Январь".ToLower());
            }

            public string GetPrevYearUniq()
            {
                return GetUniqueDate(GetPrevYear(Year), Mounth.ToLower());
            }

            protected string GetPrevYear(string year)
            {
                return (int.Parse(Year) - 1).ToString();
            }

            public string GetPrevMounth(string Mounth, ref string year)
            {
                int mounthNum = CRHelper.MonthNum(Mounth);
                if (mounthNum == 1)
                {
                    mounthNum = 12;
                    year = GetPrevYear(year);
                }
                else
                {
                    mounthNum--;
                }
                return CRHelper.RusMonth(mounthNum);
            }

            public string GetPrevMounthUniq()
            {
                string year = this.Year;
                string mounth = GetPrevMounth(this.Mounth, ref year);

                return GetUniqueDate(year, mounth); ;
            }


            protected string GetUniqueDate(string year, string mounth)
            {
                string c = (mounth[0] + "").ToUpper();
                DateHelper.PrevDateGridCaption = c + mounth.Remove(0, 1) + " " + year + " года";

                return string.Format(GetDictMounth()[mounth], year);

            }

            public DateHelper(string year, string mounth)
            {
                this.Year = year;
                this.Mounth = mounth;
            }
        }

        #region Combo

        Dictionary<string, int> GenDictFromDateType()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("Предыдущий период", 0);
            dict.Add("На начало года", 0);
            dict.Add("На аналогичный период прошлого года", 0);

            return dict;
        }

        protected void GetReportParam()
        {
            SelectLevel.Value = CheckBox1.Checked ? "3" : "2";

            DataComboPeriod.SetFormatterText((parent, children) => { return children + " " + parent + " года"; });
            DataComboPeriod.SetFormatterTextParent((parent) => { return parent + " год"; });
            DataComboPeriod.LoadData(DataBaseHelper.ExecQueryByID("Period"));

            if (!Page.IsPostBack)
            {
                ComboPeriod.FillDictionaryValues(DataComboPeriod.DataForCombo);
                ComboPeriod.SetСheckedState(DataComboPeriod.LastAdededKey, true);

                ComboTypeDate.FillDictionaryValues(GenDictFromDateType());
            }

            ComboPeriod.Width = 250;
            ComboPeriod.Title = "Дата";
            ComboTypeDate.Width = 350;
            ComboTypeDate.Title = "Период для сравнения";
            SelectedPeriod();

        }

        private void SelectedPeriod()
        {
            CurrentDateLineChart.Value = DataComboPeriod.DataUniqeName[DataComboPeriod.LastAdededKey];

            CurrentDate.Value = DataComboPeriod.DataUniqeName[ComboPeriod.SelectedValue];

            DateHelper CurDate = new DateHelper(
                    DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Year"],
                    DataComboPeriod.OtherInfo[ComboPeriod.SelectedValue]["Mounth"]);

            if (ComboTypeDate.SelectedValue == "Предыдущий период")
            {
                ComparisonDate.Value = CurDate.GetPrevMounthUniq();
            }
            if (ComboTypeDate.SelectedValue == "На начало года")
            {
                ComparisonDate.Value = CurDate.GetFirstDateFromYear();
            }
            if (ComboTypeDate.SelectedValue == "На аналогичный период прошлого года")
            {
                ComparisonDate.Value = CurDate.GetPrevYearUniq();
            }
        }
        #endregion

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

                AxisY.Extent = 60;
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
                //Chart.Legend.Margins.Right = CusctomizerSize.GetChartLegendWidth(false);
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
                        LastRow[CurMunth] = Row["Death"];
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

                Chart.LineChart.NullHandling = NullHandling.DontPlot;
            }

            public override void DataBindChart()
            {
                Chart.ChartDrawItem += new ChartDrawItemEventHandler(Chart_ChartDrawItem);

                TableChart = DataBaseHelper.ExecQueryByID("ChartDynamic", "Mounth");

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
                Chart.Legend.SpanPercentage = 10;
                Chart.Legend.Margins.Right = 890;
                Chart.Legend.Visible = true;
            }

            protected override void ConfChartToolTips()
            {
                base.ConfChartToolTips();
                //Chart.Tooltips.FormatString = "<ITEM_LABEL> <SERIES_LABEL> года<br><b><DATA_VALUE:### ##0.00></b> " + Unit.ToLower();
                Chart.Tooltips.FormatString = "Число умерших за <SERIES_LABEL> год, <b><DATA_VALUE:### ##0></b>, человек";// +Unit.ToLower();
            }
        }


        private void GenerationChartDynamic()
        {

            IConfiguratoChart ConfiguratorChart =
                (IConfiguratoChart)new ConfiguratorDynamicChart(Chart, TableChart, "Человек", "");

            ConfiguratorChart.DataBindChart();

            ConfiguratorChart.ConfChart();

            ConfiguratorChart.ConfLegend(CustomizerSize);
        }
        #endregion


        static class CaseMounth
        {
            
            public static string MounthPreposition(string NominativeMounth)
            {
                string PrepositionMounth;

                if (NominativeMounth == "март"
                    ||
                    NominativeMounth == "август")
                {
                    PrepositionMounth = NominativeMounth + "е";
                }
                else
                {
                    PrepositionMounth = NominativeMounth.Remove(NominativeMounth.Length - 1) + "е";
                }
                return PrepositionMounth;
            }
            public static string MounthInstrumentative(string NominativeMounth)
            {
                int monthNum = CRHelper.MonthNum(NominativeMounth.ToLower());

                switch (monthNum)
                {
                    case 1:
                        return "январём";
                    case 2:
                        return "февралём";
                    case 3:
                        return "мартом";
                    case 4:
                        return "апрелем";
                    case 5:
                        return "маем";
                    case 6:
                        return "июнем";
                    case 7:
                        return "июлем";
                    case 8:
                        return "августом";
                    case 9:
                        return "сентябрём";
                    case 10:
                        return "октябрём";
                    case 11:
                        return "ноябрём";
                    case 12:
                        return "декабрём";
                    default:
                        return "январь";
                }

                return NominativeMounth;


                string InstrumentativeMounth;

                if (NominativeMounth == "март"
                    ||
                    NominativeMounth == "август")
                {
                    InstrumentativeMounth = NominativeMounth + "ом";
                }
                if (NominativeMounth == "апрель")
                {
                    InstrumentativeMounth = NominativeMounth.Remove(NominativeMounth.Length - 1) + "ем";
                }
                else
                {
                    InstrumentativeMounth = NominativeMounth.Remove(NominativeMounth.Length - 1) + "ём";
                }
                return InstrumentativeMounth;
            }

            public static string GetInstrumentativeDate(string Date)
            {
                string[] splitDate = Date.Split(' ');
                splitDate[0] = CaseMounth.MounthInstrumentative(splitDate[0]);

                return splitDate[0] + " " + splitDate[1] + " " + splitDate[2];

            }

            public static string GetPrepositioneDate(string Date)
            {
                string[] splitDate = Date.Split(' ');
                splitDate[0] = CaseMounth.MounthPreposition(splitDate[0]);

                return splitDate[0] + " " + splitDate[1] + " " + splitDate[2];

            }

        }

        string GetAllChildrenFromParent(string parent)
        {
            string Childrens = "";
            foreach (UltraGridRow row in Grid.Rows)
            {
                if (row.Cells.FromKey("parentName").Value.ToString() == parent)
                {
                    Childrens += row.Cells.FromKey("DiseaseUniqueName").Value.ToString() + ",";
                }
            }
            Childrens = Childrens.Remove(Childrens.Length - 1, 1);
            return Childrens;
        }

        void ActiveRow(UltraGridRow Row)
        {
            SelectUniqName.Value = Row.Cells.FromKey("DiseaseUniqueName").Value.ToString();

            GenerationChartDynamic();

            GenerationChartPoint();

            SetHeader();
        }

        string TextPointChart;
        private void GenerationChartPoint()
        {

            #region bindData
            TablePointChart = DataBaseHelper.ExecQueryByID("ChartPoint", "Disease");

            TextChartPoint.Text = "<table>";

            TextChartPoint0.Text = string.Format("<table>", (TablePointChart.Rows.Count / 2 + 2).ToString()); ;

            TextChartPoint0.Text = "<table>";

            for (int i = 0; i < TablePointChart.Rows.Count; i++)
            {
                if (i > TablePointChart.Rows.Count / 2)
                {
                    TextChartPoint0.Text += string.Format("<tr><td style='width:30px;text-align: right;vertical-align: top;'>{0}.<td><td>{1}</td></tr>", i + 1, TablePointChart.Rows[i]["Disease"]);
                }
                else
                {
                    TextChartPoint.Text += string.Format("<tr><td style='width:30px;text-align: right;vertical-align: top;'>{0}.<td><td>{1}</td></tr>", i + 1, TablePointChart.Rows[i]["Disease"]);
                }
                TablePointChart.Rows[i]["Disease"] = (i + 1).ToString();
            }
            TextChartPoint.Text += "</table>";
            TextChartPoint0.Text += "</table>";
            string SelectDate = ComboPeriod.SelectedValue;
            string ComparisonDate = DateHelper.PrevDateGridCaption;

            try
            {
                TablePointChart.Columns[1].ColumnName = SelectDate;
                TablePointChart.Columns[2].ColumnName = ComparisonDate;

            }
            catch
            {
                TablePointChart.Columns[1].ColumnName = SelectDate;
                TablePointChart.Columns.Remove(TablePointChart.Columns[2]);
            }

            ChartPoint.ChartType = ChartType.LineChart;
            ChartPoint.DataSource = TablePointChart;
            ChartPoint.DataBind();

            #endregion

            #region ConfChart

            ChartPoint.Axis.Y.NumericAxisType = NumericAxisType.Logarithmic;

            //ChartPoint.Axis.Y.RangeMax = 400;
            //ChartPoint.Axis.Y.RangeMin = 0;
            //ChartPoint.Axis.Y.RangeType = AxisRangeType.Custom;

            ChartPoint.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ### ### ##0>";

            ChartPoint.Axis.Y.LogZero = 0.4;

            ChartPoint.Axis.Y.LogBase = 1.0000001;

            ChartPoint.Axis.Y.Extent = Chart.Axis.Y.Extent;
            ChartPoint.Axis.Y.Labels.Font = Chart.Axis.Y.Labels.Font;
            //ChartPoint.Axis.Y.Labels.Font = Chart.Axis.X.Labels.Font;

            ChartPoint.Legend.FormatString = "<ITEM_LABEL>";
            ChartPoint.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ### ##0></b>, человек";

            ChartPoint.LineChart.LineAppearances.Clear();

            LineAppearance lineAppearance3 = new LineAppearance();
            lineAppearance3.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance3.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance3.Thickness = 0;
            ChartPoint.LineChart.LineAppearances.Add(lineAppearance3);

            LineAppearance lineAppearance1 = new LineAppearance();
            lineAppearance1.Thickness = 0;
            lineAppearance3.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance3.IconAppearance.IconSize = SymbolIconSize.Small;
            ChartPoint.LineChart.LineAppearances.Add(lineAppearance1);

            ChartPoint.Axis.Y.RangeType = AxisRangeType.Automatic;
            ChartPoint.Axis.X.RangeType = AxisRangeType.Automatic;

            ChartPoint.Legend.Visible = true;

            ChartPoint.Axis.X.Extent = 10;
            ChartPoint.Axis.X.Labels.Orientation = TextOrientation.Horizontal;

            ChartPoint.Axis.Y.Margin.Near.Value = 5;
            ChartPoint.Axis.Y.Margin.Near.MarginType = LocationType.Percentage;

            ChartPoint.Axis.X.Margin.Near.Value = 1;
            ChartPoint.Axis.X.Margin.Near.MarginType = LocationType.Percentage;

            ChartPoint.Axis.X.Margin.Far.Value = 1;
            ChartPoint.Axis.X.Margin.Far.MarginType = LocationType.Percentage;

            ChartPoint.Axis.Y.Margin.Far.Value = 5;
            ChartPoint.Axis.Y.Margin.Far.MarginType = LocationType.Percentage;

            ChartPoint.FillSceneGraph += new FillSceneGraphEventHandler(ChartPoint_FillSceneGraph);

            ChartPoint.Legend.SpanPercentage = 10;
            ChartPoint.Legend.Margins.Right = 890;

            #endregion
        }


        private void SetHeader()
        {
            Hederglobal.Text = string.Format("Мониторинг числа смертей от болезней (по состоянию на выбранную дату)", ComboPeriod.SelectedValue);
            Page.Title = Hederglobal.Text;

            PageSubTitle.Text = string.Format("Данные ежемесячного мониторинга числа смертей от болезней в ХМАО-Югре ({0})", ComboPeriod.SelectedValue);

            HeaderChart.Text = string.Format("Число умерших, человек, по виду болезни «{0}» в ХМАО-ЮГРЕ", UserComboBox.getLastBlock(SelectUniqName.Value));
            HeaderPointChart.Text = string.Format("Число умерших по классам болезней в ХМАО-ЮГРЕ (по состоянию на выбранную дату)");
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            GetReportParam();

            DataBindGrid();

            ConfigurationDispalyGrid();

            ActiveRow(Grid.Rows[0]);

            GenerateText();
        }


        #region Дорисовывалка диограмки
        void ChartPoint_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int count = 0;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.PointSet")
                {
                    PointSet ps = primitive as PointSet;
                    for (int j = 0; j < ps.points.Length; j++)
                    {
                        if (ps.points[j].Row == 0)
                        {
                            ps.points[j].Visible = false;
                            ps.points[j].hitTestRadius = 20;
                            DrawBox(e, ps.points[j].point, Color.Blue);
                        }

                        if (ps.points[j].Row == 1)
                        {

                            ps.points[j].Visible = false;
                            ps.points[j].hitTestRadius = 20;
                            DrawBox(e, ps.points[j].point, Color.Green);


                        }
                    }

                }
                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.Polyline")
                {
                    primitive.PE.Fill = Color.Blue;
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;

                    if (!(string.IsNullOrEmpty(box.Path)) && box.Path.EndsWith("Legend") &&
                        box.rect.Width == box.rect.Height)
                    {
                        Color color = Color.Aqua;
                        count++;

                        switch (count)
                        {
                            case 1:
                                {
                                    color = Color.Blue;
                                    break;
                                }
                            case 2:
                                {
                                    color = Color.Green;
                                    break;
                                }
                            case 3:
                                {
                                    color = Color.Red;
                                    break;
                                }
                        }

                        Box box1 = new Box(box.rect);
                        box1.PE.ElementType = PaintElementType.Gradient;
                        box1.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                        box1.PE.Fill = color;
                        box1.PE.FillStopColor = color;
                        box1.PE.Stroke = Color.Black;
                        box1.PE.StrokeWidth = 1;
                        box1.Row = 0;
                        box1.Column = 2;
                        box1.Value = 42;
                        box1.Layer = e.ChartCore.GetChartLayer();
                        box1.Chart = this.ChartPoint.ChartType;
                        e.SceneGraph.Add(box1);
                    }
                }

            }
        }

        private void DrawBox(FillSceneGraphEventArgs e, Point p, Color color)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            Box box = new Box(new Point(p.X - 6, p.Y - 6), 13, 13);

            box.PE.ElementType = PaintElementType.Gradient;
            box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
            box.PE.Fill = color;
            box.PE.FillStopColor = color;
            box.PE.Stroke = Color.Black;
            box.PE.StrokeWidth = 1;
            box.Row = 0;
            box.Column = 2;
            box.Value = 42;
            box.Layer = e.ChartCore.GetChartLayer();
            box.Chart = this.ChartPoint.ChartType;

            e.SceneGraph.Add(box);
        }
        #endregion

        #region TODO пЕРЕДЕЛАТЬ ЕКСПОРТ
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Hederglobal.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 12 * 20;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            ExportGridToExcel(Grid, e.CurrentWorksheet.Workbook.Worksheets["Таблица"], 3, true);
            try
            {
                //img = new Infragistics.Documents.Reports.Graphics.Image(Server.MapPath("~//reports//HMAO_ARC_001//Disease.JPG"));
                e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].Value = HeaderChart.Text;
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0], Chart);

                e.Workbook.Worksheets["Диаграммa2"].Rows[0].Cells[0].Value = HeaderPointChart.Text;
                ReportExcelExporter.ChartExcelExport(e.Workbook.Worksheets["Диаграммa2"].Rows[1].Cells[0], ChartPoint);

                //Infragistics.Documents.Reports.Graphics.Image img = new Infragistics.Documents.Reports.Graphics.Image(Server.MapPath("~//reports//HMAO_ARC_001//Disease.JPG"));
                //System.G
                //Worc
                System.Drawing.Image img = System.Drawing.Image.FromFile(Server.MapPath("~//reports//HMAO_ARC_001//Disease.JPG"));
                WorksheetImage excelImage = new WorksheetImage(img);
                WorksheetCell cell = e.Workbook.Worksheets["Диаграммa2"].Rows[27].Cells[0];
                excelImage.TopLeftCornerCell = cell;
                excelImage.BottomRightCornerCell = cell;
                cell.Worksheet.Shapes.Add(excelImage);
                excelImage.SetBoundsInTwips(cell.Worksheet,
                                            new Rectangle(
                                                excelImage.TopLeftCornerCell.GetBoundsInTwips().Left,
                                                excelImage.TopLeftCornerCell.GetBoundsInTwips().Top,
                                                 (int)(20 * ChartPoint.Width.Value), (int)(20 * ChartPoint.Height.Value)),
                                            true);


                //ReportExcelExporter.Ch
            }
            catch { }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            Worksheet sheet3 = workbook.Worksheets.Add("Диаграммa2");

            //if (IndexActiveRow.Value != "")
            //    ActionRowActive(Grid.Rows[int.Parse(IndexActiveRow.Value)]);
            GenerationChartDynamic();

            foreach (UltraGridColumn col in Grid.Columns)
            {
                if ((col.Hidden) || (col.Width.Value == 0))
                {
                    Grid.Bands[0].HeaderLayout.Remove(col.Header);
                    Grid.Columns.Remove(col);
                }
            }


            Chart.Width = (int)(Chart.Width.Value * 0.75);

            ChartPoint.Width = (int)(Chart.Width.Value * 0.75);


            Chart.Legend.Margins.Right = (int)(Chart.Legend.Margins.Right * 0.5);

            ReportExcelExporter1.ExcelExporter.ExcelStartRow = 7;
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
                sheet.Rows[i + startrow].CellFormat.FormatString = "### ### ##0.00";
            }
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
            title.AddContent(PageSubTitle.Text);

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


            GenerationChartDynamic();
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

            is_ = r.AddSection();
            is_.PageSize = new PageSize(950, 800);
            title = is_.AddText();
            title.Margins = new Infragistics.Documents.Reports.Report.Margins(40, 40, 40, 40);
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Left;

            GenerationChartPoint();
            title.AddContent(HeaderPointChart.Text);
            imageStream = new MemoryStream();
            ChartPoint.SaveTo(imageStream, ImageFormat.Png);
            img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);

            ima = is_.AddImage(img);
            ima.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 0, 5, 0);

            title = is_.AddText();
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;

            title.Alignment.Horizontal = Alignment.Center;

            imageStream = new MemoryStream();

            img = new Infragistics.Documents.Reports.Graphics.Image(Server.MapPath("~//reports//HMAO_ARC_001//Disease.JPG"));


            ima = is_.AddImage(img);
            ima.Margins = new Infragistics.Documents.Reports.Report.Margins(5, 0, 5, 0);

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

        int CountPDFheaderLevel = 3;

        int HeightLevel = 20;

        int[] PDFHeaderHeightsLevel = { 20, 20, 20 };


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

            ApplyHeader();
            ConfigurationDispalyGrid();

            int sumW = 0;

            ITableRow SelectorCol = RootRow.AddCell().AddTable().AddRow();

            AddTableCell(SelectorCol, ".", 16, PDFGetLevelHeight(0, 3));

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
        public void SetCellStyle(ITableCell headerCell)
        {
            headerCell.Alignment.Horizontal = Alignment.Center;
            headerCell.Alignment.Vertical = Alignment.Middle;
            headerCell.Borders = headerBorders;
            headerCell.Paddings.All = 2;
            headerCell.Background = headerBackground;
        }
        public static void SetFontStyle(IText t)
        {
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font(new System.Drawing.Font("Arial", 8));
            t.Style.Font = font;
            t.Style.Font.Bold = true;
            t.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Center;
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
        void setHederWidth(ITableCell tableCell, Double width)
        {
            tableCell.Width = new FixedWidth((int)width);
            tableCell.Parent.Parent.Width = new FixedWidth((int)width);
            object o = tableCell.Parent.Parent.Parent;
            ITableCell parentCell = (ITableCell)(o);
            parentCell.Width = new FixedWidth((int)width);

            //Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font(new System.Drawing.Font("Arial", 8));
            //t.Style.Font = font;
            //t.Style.Font.Bold = true;
            //t.Alignment = Infragistics.Documents.Reports.Report.TextAlignment.Center;

        }
        #endregion
        #endregion

    }
}
/* Здесь 
 * могла
 * бы 
 * быть
 * выша 
 * реклама
 */