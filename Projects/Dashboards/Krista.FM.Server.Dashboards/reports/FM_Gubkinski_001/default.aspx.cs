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

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using System.Collections.Generic;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using System.Web.SessionState;

using Krista.FM.ServerLibrary;

using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using Krista.FM.Common;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;

namespace Krista.FM.Server.Dashboards.FM_Gubkinski_001
{
    public partial class _default : CustomReportPage
    {

        int ScreenWidth { get { return ((int)Session[CustomReportConst.ScreenWidthKeyName]); } }


        ICustomizerSize CustomizerSize;

        CustomParam SourceID { get { return (UserParams.CustomParam("SourceID")); } }

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
                Grid.Columns.FromKey("Number").Width = onePercent * 5;
                Grid.Columns.FromKey("Task").Width = onePercent * 45;
                Grid.Columns.FromKey("Quarters").Width = onePercent * 45;
                
                Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                Grid.Columns.FromKey("Number").Width = onePercent * 5;
                Grid.Columns.FromKey("Task").Width = onePercent * 45;
                Grid.Columns.FromKey("Quarters").Width = onePercent * 45;
                Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                Grid.Columns.FromKey("Number").Width = onePercent * 5;
                Grid.Columns.FromKey("Task").Width = onePercent * 45;
                Grid.Columns.FromKey("Quarters").Width = onePercent * 45;
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
                Grid.Columns.FromKey("Number").Width = onePercent * 5;
                Grid.Columns.FromKey("Task").Width = onePercent * 47;
                Grid.Columns.FromKey("Quarters").Width = onePercent * 47;
            }

            protected override void ContfigurationGridFireFox(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                Grid.Columns.FromKey("Number").Width = onePercent * 5;
                Grid.Columns.FromKey("Task").Width = onePercent * 47;
                Grid.Columns.FromKey("Quarters").Width = onePercent * 47;
            }

            protected override void ContfigurationGridGoogle(UltraWebGrid Grid)
            {
                int onePercent = (int)Grid.Width.Value / 100;
                Grid.Columns.FromKey("Number").Width = onePercent * 4;
                Grid.Columns.FromKey("Task").Width = onePercent * 48;
                Grid.Columns.FromKey("Quarters").Width = onePercent * 48;
            }

            #endregion

        }
        #endregion


        #region Combo

        Dictionary<string, string> IDByDate = new Dictionary<string, string>();

        void FillComboYear()
        {
            ComboYear.Width = 100;
            ComboYear.Title = "Год";


            DataTable table = GetDBWar(DataProvider.GetQueryText("ComboYear"));
            Dictionary<string, int> DictYear = new Dictionary<string, int>();
            foreach (DataRow row in table.Rows)
            {
                DictYear.Add(row["Year"].ToString(), 0);
                IDByDate.Add(row["Year"].ToString(), row["SourceID"].ToString());
            }
            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(DictYear);
            }
        }   

        #endregion


        #region Loaders
        private static IDatabase GetDataBase()
        {
            HttpSessionState sessionState = HttpContext.Current.Session;
            LogicalCallContextData cnt =
                sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
            if (cnt != null)
                LogicalCallContextData.SetContext(cnt);
            IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];

            return scheme.SchemeDWH.DB;

        }

        private DataTable GetDBWar(string Query)
        {
            IDatabase db = GetDataBase();
            DataTable dt = null;

            try
            {
                dt = (DataTable)db.ExecQuery(Query, QueryResultTypes.DataTable);
                CRHelper.SaveToQueryLog(Query);
            }
            //catch (Exception e)
            //{
            //    String.Format(CRHelper.GetExceptionInfo(e));
            //}
            finally
            {
                db.Dispose();
            }
            return dt;

        }


        DataTable GetDataTableFromChart(string QueryId)
        {
            string Query = DataProvider.GetQueryText(QueryId);

            DataTable NewTable = new DataTable(QueryId);

            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(Query, QueryId, NewTable);

            return NewTable;
        }
        #endregion

        #region DataBindGrid

        private void CustomizeReportTableColumn(DataTable Table)
        {
            DataColumnCollection Cols = Table.Columns;

            Cols.Add("TypeRow");

            Cols.Add("Number");

            Cols.Add("Task");

            Cols.Add("Quarters");
        }

        Dictionary<Int32, List<DataRow>> TableTree = new Dictionary<Int32, List<DataRow>>();
        private void SaveToHierarhyDict(DataTable BaseTable)
        {


            foreach (DataRow row in BaseTable.Rows)
            {

                Int32 IDRow = (Int32)row["ID"];

                Int32? ParentID = row["ParentID"] == DBNull.Value ? null : (Int32?)row["ParentID"];

                if (ParentID == null)
                {
                    List<DataRow> ChildrenRow = new List<DataRow>();
                    ChildrenRow.Add(row);                        
                    TableTree.Add(IDRow, ChildrenRow);
                }
                else
                {
                    if (TableTree.ContainsKey((Int32)(ParentID)))
                    {
                        List<DataRow> ChildrenRow = TableTree[(Int32)ParentID];
                        ChildrenRow.Add(row);                        
                    }
                }
            }
        }

        private void LoadInHierarhyDict(DataTable BaseTable, DataTable ReportTable)
        {
            foreach (List<DataRow> ListRow in TableTree.Values)
            {
                foreach(DataRow BaseRow in ListRow)
                {
                    DataRow NewRow = ReportTable.NewRow();
                    ReportTable.Rows.Add(NewRow);
                    CopyValueFromRow(NewRow, BaseRow);
                }
                
            }
        }

        private string ExtractQuaters(DataRow BaseRow)
        {
            string[] NameQuarts = { "В первом квартале", "Во втором квартале","В третем квартале", "В четвёртом квартале"};
            object[] ValQarts = { BaseRow["FirstQuarter"], BaseRow["SecondQuarter"], BaseRow["ThirdQuarter"], BaseRow["ThirdQuarter"]};

            string Res = "";


            for (int i = 0; i < NameQuarts.Length; i++)
            {
                if (ValQarts[i] != DBNull.Value)
                {
                    Res += NameQuarts[i] + ": " + ValQarts[i] + "<br><br>";
                }
            }
            return Res;
        }

        int CountChild = 1;
        int CountParent = 0;
        private void CopyValueFromRow(DataRow NewRow, DataRow BaseRow)
        {
            

            NewRow["TypeRow"] = BaseRow["ParentID"] == DBNull.Value ? "Parent" : "Children";


            if (NewRow["TypeRow"].ToString() == "Children")
            {
                
                NewRow["Number"] = string.Format("{0:00}.{1:00}",CountParent,CountChild);
                CountChild++;

                NewRow["Task"] = BaseRow["Task"].ToString() + "<br><br>Срок исполнения: "
                    + BaseRow["Time"].ToString() + "<br>Исполнитель: " + BaseRow["Executors"].ToString();
                //Grid.Style.Add(HtmlTextWriterStyle.
                NewRow["Quarters"] = ExtractQuaters(BaseRow);
            }
            else
            {
                CountParent++;
                NewRow["Number"] = string.Format("{0:00}.00", CountParent);
                NewRow["Task"] = BaseRow["Task"];
                
                CountChild = 1;
            }

            

            
        }

        

        private void ImportRow(DataTable BaseTable, DataTable ReportTable)
        {
            SaveToHierarhyDict(BaseTable);
            LoadInHierarhyDict(BaseTable, ReportTable);

        }

        

        




        void Grid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("Grid");

            DataTable BaseTable = GetDBWar(query);

            DataTable ReportTable = new DataTable("reportGrid");

            CustomizeReportTableColumn(ReportTable);

            //Выстраиваем результирующую выборку, в порядке поля код, тоесть сохраняет визуальную зависимость дочерних задач от родительских
            ImportRow(BaseTable, ReportTable);

            Grid.DataSource = ReportTable;
        }



        private void GenerationGrid()
        {
            Grid.DataBinding += new EventHandler(Grid_DataBinding);
            Grid.DataBind();
        }

        #endregion        

        #region Dispalygrid


        private void CustomizeGrid()
        {
            SetColName();
            SetColStyle();
            SetColSize();
            FormatRow();
            Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
        }

        private void FormatRow()
        {
            foreach (UltraGridRow row in Grid.Rows)
            {
                if (row.Cells.FromKey("TypeRow").Value.ToString() == "Children")
                {
                    FormatChildrenRow(row);
                }
                else
                {
                    FormatParentRow(row);
                }
                row.Cells.FromKey("Number").Style.BorderDetails.ColorLeft = Color.LightGray;
            }
        }

        private void FormatParentRow(UltraGridRow row)
        {
            row.Style.BackColor = Color.FromArgb(241, 241, 242);
            row.Cells.FromKey("Task").ColSpan = 2;
            row.Cells.FromKey("Task").Style.Font.Bold = true;
        }

        private void FormatChildrenRow(UltraGridRow row)
        {
            row.Style.BackColor = Color.Transparent;
            row.Cells.FromKey("Task").Style.VerticalAlign = VerticalAlign.Top;
            row.Cells.FromKey("Quarters").Style.VerticalAlign = VerticalAlign.Top;
            row.Cells.FromKey("Task").Text = row.Cells.FromKey("Task").Text.Replace("\n", "<br>");

        }

        private void SetColSize()
        {
            CustomizerSize.ContfigurationGrid(Grid);   
        }

        private void SetColStyle()
        {
            Grid.Columns.FromKey("Number").CellStyle.HorizontalAlign = HorizontalAlign.Center;

            Grid.Columns.FromKey("Task").CellStyle.HorizontalAlign = HorizontalAlign.Left;

            Grid.Columns.FromKey("Quarters").CellStyle.HorizontalAlign = HorizontalAlign.Left;

            Grid.Columns.FromKey("TypeRow").Hidden = true;

            foreach (UltraGridColumn col in Grid.Columns)
            {
                col.CellStyle.Wrap = true;
                col.Header.Style.Wrap = true;
                col.Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        private void SetColName()
        {
            Grid.Columns.FromKey("Number").Header.Caption = "№п/п";
            Grid.Columns.FromKey("Task").Header.Caption = "Задачи, мероприятия, результаты выполнения";
            Grid.Columns.FromKey("Task").Header.RowLayoutColumnInfo.SpanX = 2;
            Grid.Columns.FromKey("Task").Header.RowLayoutColumnInfo.SpanY = 1;
            Grid.Columns.FromKey("Task").Header.RowLayoutColumnInfo.OriginX = 1;
            Grid.Columns.FromKey("Task").Header.RowLayoutColumnInfo.OriginY = 0;

            Grid.Columns.FromKey("Number").Header.RowLayoutColumnInfo.SpanX = 1;
            Grid.Columns.FromKey("Number").Header.RowLayoutColumnInfo.SpanX = 1;
            Grid.Columns.FromKey("Number").Header.RowLayoutColumnInfo.OriginX = 0;
            Grid.Columns.FromKey("Number").Header.RowLayoutColumnInfo.OriginY = 0;

            Grid.Columns.FromKey("Quarters").Header.RowLayoutColumnInfo.SpanX = 0;
            Grid.Columns.FromKey("Quarters").Header.RowLayoutColumnInfo.SpanX = 1;
            Grid.Columns.FromKey("Quarters").Header.RowLayoutColumnInfo.OriginX = 2;
            Grid.Columns.FromKey("Quarters").Header.RowLayoutColumnInfo.OriginY = 0;



            Grid.Bands[0].HeaderLayout.Remove(Grid.Columns.FromKey("Quarters").Header);
            
        }

        #endregion 

        #region SettingHeader

        protected void SettingHeader()
        {
            HeaderGlobal.Text = "Отчет по исполнению комплексного плана мероприятий по реализации послания Президента РФ Федеральному Собранию РФ в  г. Губкинский";
            //SubHeaderGlobal.Text = string.Format("Показатели эффективности деятельности органов местного самоуправления за 2010 год, {0}", ComboRegion.SelectedValue);

            Page.Title = HeaderGlobal.Text;
        }
        

        #endregion

        private void CustomizeAndGenerationGrid()
        {
            GenerationGrid();
            CustomizeGrid();
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
            Grid.Height = Unit.Empty;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_Test);
            //UltraGridExporter1.PdfExporter.RowExported += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportedEventArgs>(PdfExporter_RowExported);            
               
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);


            FillComboYear();
            SourceID.Value = IDByDate[ComboYear.SelectedValue];

            CustomizeAndGenerationGrid();
            SettingHeader();
        }

        private void BindTestGrid()
        {
            string query = DataProvider.GetQueryText("GridSqlQ");

            DataTable BaseTable = GetDBWar(query);

            Grid.DataSource = BaseTable;
            Grid.DataBind();
        }

        #region Экспорт в Excel
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;

            e.CurrentWorksheet.PrintOptions.PrintRowAndColumnHeaders = true;//
            e.CurrentWorksheet.PrintOptions.Header = "2:2";
            
            
            CustomView alternateView = e.Workbook.CustomViews.Add("Alternate Settings", true, true);
            
            PrintOptions po = alternateView.GetPrintOptions(e.CurrentWorksheet);
            po.PrintGridlines = 1 == 1;

            //ExcelDefaultableBoolean

            e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 3);
            e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 3);

            e.CurrentWorksheet.Rows[0].Cells[0].Value = "Отчет по исполнению комплексного плана мероприятий по реализации послания Президента РФ Федеральному Собранию РФ в г. Губкинский";//HeaderGlobal.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True; ;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Height = 50 * 5;
            e.CurrentWorksheet.Rows[0].Height = 20 * 40;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            e.CurrentWorksheet.MergedCellsRegions.Add(2, 0, 2, 8);
            e.CurrentWorksheet.MergedCellsRegions.Add(3, 0, 3, 8);

            //e.CurrentWorksheet.Rows[2].Cells[0].Value = ComboRegion.SelectedValue;//SubHeaderGlobal.Text;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.False;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Italic = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Font.Height = 50 * 4;
            e.CurrentWorksheet.Rows[2].Height = 10 * 40;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            ExportGrid(Grid, e.CurrentWorksheet, 4);

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            ClearBr();
            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 5;

            UltraGridExporter1.ExcelExporter.Export(Grid, sheet1);
            
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        #endregion

        #region Експор Грида

        protected void SetStyleHeadertableFromExcel(IWorksheetCellFormat CellFormat)
        {

            CellFormat.WrapText = ExcelDefaultableBoolean.True;

            CellFormat.FillPatternBackgroundColor = System.Drawing.Color.FromArgb(200, 200, 200);

            CellFormat.FillPatternForegroundColor = System.Drawing.Color.FromArgb
(200, 200, 200);

            CellFormat.FillPattern = FillPatternStyle.Default;

            CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

            CellFormat.Alignment = HorizontalCellAlignment.Center;

            
        }

        object FormatVal(object value)
        {
            return value;
            object resValue = value;

            if (typeof(string) == value.GetType())
            {

                resValue = value.ToString().Replace("<br>", "\n");

            }

            return resValue;
        }

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
                {
                    //TODO разобратся с левыми хедерами
                }
            }

            maxRow++;

            for (int i = rowFirst; i < maxRow; i++)
            {
                WorkSheet.Rows[i].Height = 20 * 40;
            }

            return maxRow;

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
                    G.Columns.Remove(G.Columns[i]);
                    i--;
                }
                
                
            }
            for (int i = 0; i < G.Columns.Count; i++)
            {
                sheet.Columns[i].Width = (int)G.Columns[i].Width.Value * 36;
                
                sheet.Columns[i].CellFormat.FormatString = G.Columns[i].Format;
            }
        }

        void ExportGridToExcel(UltraWebGrid G, Worksheet sheet, int startrow, bool RowZebra)
        {


            foreach (UltraGridColumn col in G.Columns)
            {
                if ((col.Hidden)||(col.Width.Value == 0))
                {
                    G.Bands[0].HeaderLayout.Remove(col.Header);
                    G.Columns.Remove(col);
                }
            }
            G.Columns.Remove(G.Columns.FromKey("Code"));
            ExportWidthGrid(G, sheet);

            startrow = ExportHeaderGrid(G.Bands[0].HeaderLayout, sheet, startrow);

            for (int i = 0; i < G.Rows.Count; i++)
            {
                sheet.Rows[i + startrow].Height = 22 * 40;
                sheet.Rows[i + startrow].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                sheet.Rows[i + startrow].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                for (int j = 0; j < 10;j++ )
                    try
                    {
                        sheet.Rows[i + startrow].Cells[j].Value = sheet.Rows[i + startrow].Cells[j].Value.ToString() + "";
                    }
                    catch { }
                
            }
        }
        
        #endregion

        #region Экспорт в PDF


        void PDFAddTitleReport(ISection Sectin, string TextHeader)
        {
            IText title = Sectin.AddText();

            Font font = new Font("Verdana", 16);

            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.Alignment.Horizontal = Alignment.Center;
            title.Alignment.Vertical = Alignment.Middle;

            title.AddContent("Отчет по исполнению комплексного плана мероприятий по реализации послания Президента РФ Федеральному Собранию РФ в  г. Губкинский");
            //title.AddContent(TextHeader);
            title.Height = new FixedHeight(60); 
        }

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
            title.Alignment.Horizontal = Alignment.Center;
            title.Alignment.Vertical = Alignment.Middle;
            title.AddContent("Отчет по исполнению комплексного плана мероприятий по реализации послания Президента РФ Федеральному Собранию РФ в  г. Губкинский");
            title.Height = new FixedHeight(60);

            title = e_.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Italic = true;
            title.Alignment.Horizontal = Alignment.Center;
            title.Alignment.Vertical = Alignment.Top;
            //title.AddContent(ComboRegion.SelectedValue+"\n");
            title.Height = new FixedHeight(30);

            Grid.Bands[0].HeaderLayout.Clear();
            UltraGridExporter1.PdfExporter.Export(Grid, e_);
        }

        #region Рисовалка иерархичного хидера
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

        private void ExportHeader(ITable Table)
        {
            ITableRow RootRow = Table.AddRow();

            //ITableRow SelectorCol = RootRow.AddCell().AddTable().AddRow();            

            int sumW = CreateAllRootHeader(RootRow);
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

        class sortHeder : IComparer
        {
            public int Compare(object x, object y)
            {

                if (((HeaderBase)x).RowLayoutColumnInfo.OriginX > ((HeaderBase)y).RowLayoutColumnInfo.OriginX)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }                
            }
        }
        private void PreProcessing(MarginCellExportingEventArgs e)
        {
            headerBackground = e.ReportCell.Background;
            headerBorders = e.ReportCell.Borders;

            //Скрываем хедер рисуемый по умолчанию
            e.ReportCell.Parent.Height = new FixedHeight(0);

            SetColName();
            SetColStyle();

            ApplyHeader();

            //SettingHeaderGrid();
            

            //SettingColWidth();

            Grid.Bands[0].HeaderLayout.Sort(new sortHeder());
            ClearBr();
            

        }

        void ClearBr()
        {
            foreach (UltraGridRow row in Grid.Rows)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    try
                    {
                        cell.Text = cell.Text.Replace("<br>", "\n");
                        cell.Text = cell.Text.Replace("</div>", "");
                        cell.Text = cell.Text.Replace("<div style=\"vertical-align:bottom\">", "");
                    }
                    catch { }
                }
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

        #region UtilsFromExportGridToPDF
        private int AddTableCell(ITableCell tableCell, HeaderBase header, Double width, Double Height)
        {
            if (header.Column != null)
            {
                width = 0.75 * (int)header.Column.Width.Value * width;
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
        }
        #endregion
        #endregion 
         
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

            baseFont = Grid.DisplayLayout.EditCellStyleDefault.Font;

            Font font = new Font(baseFont.Name, (float)8.5);//, (int)baseFont.Size.Unit.Value,styleFont); 

            return font;
        }

        int PreExportedHeight = 240;

        int GetRowHeight(UltraGridRow row)
        {
            int maxHeight = 0;
            foreach (UltraGridCell cell in row.Cells)
            {
                int CurHeight = GetStringHeight(cell.Text, GetSystemFont(cell.Column.CellStyle.Font), (int)cell.Column.Width.Value);
                maxHeight = CurHeight > maxHeight ? CurHeight : maxHeight;
            }

            return maxHeight;
        }

        void PdfExporter_RowExported(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportedEventArgs e)
        {
            e.ReportRow.KeepWithNext = true;
            if (PreExportedHeight > 1000)
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

        protected void Grid_DataBinding1(object sender, EventArgs e)
        {

        }

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
