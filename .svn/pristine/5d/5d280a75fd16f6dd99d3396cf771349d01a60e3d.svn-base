using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0002
{
    public partial class sgm_0002 : CustomReportPage
    {
        readonly DataTable dtChart = new DataTable();
        readonly DataTable dtGrid = new DataTable();
        
        string mapName = string.Empty;
        string diesCode = string.Empty;

        readonly Collection<string> years = new Collection<string>();
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        protected void CreateDataStructure(Collection<string> yearList)
        {
            DataColumn dataColumn = dtChart.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            for (int i = 0; i < yearList.Count; i++)
            {
                dataColumn = dtChart.Columns.Add();
                dataColumn.DataType = Type.GetType("System.Double");
                dataColumn.ColumnName = yearList[i];
            }
            dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            for (int j = 0; j < 4 * ComboGroup.SelectedValues.Count; j++)
            {
                dtGrid.Columns.Add();
                dtGrid.Columns[dtGrid.Columns.Count - 1].DataType = Type.GetType("System.Double");
            }

            for (int j = 0; j < yearList.Count; j++)
            {
                DataRow drGrid = dtGrid.Rows.Add();
                drGrid[0] = yearList[j];
            }
        }

        protected string ConvertGroupName(string groupName)
        {
            string groupCode = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            if (groupName == "Дети")
                groupCode = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtChild));
            if (groupName == "Взрослые")
                groupCode = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtMature));
            if (groupName == "Подростки")
                groupCode = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtTeen));
            return groupCode;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            base.Page_Load(sender, e);
            dataRotator.formNumber = 2;
            if (dataRotator.isSubjectReport) dataRotator.formNumber = 1;
            dataObject.InitObject();
            // Получение таблиц субъектов и ФО
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            int startYear = dataRotator.GetFirstYear();
            int lastYear = dataRotator.GetLastYear();
            if (!Page.IsPostBack)
            {
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, true);
                dataRotator.FillDeseasesList(ComboDesease, 0);
                supportClass.FillPeopleGroupList(ComboGroup);
                ComboGroup.SetСheckedState("Взрослые", true);
                ComboGroup.SetСheckedState("Дети", true);
                dataRotator.FillYearList(ComboYear);
                
                for (int i = startYear; i < lastYear + 1; i++)
                {
                    ComboYear.SetСheckedState(i.ToString(), true);
                }
                
                ComboPlan.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(lastYear + 1, lastYear + 6));                
                ComboPlan.SetСheckedState(Convert.ToString(lastYear + 1), true);
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, true);
            }

            int planYear = Convert.ToInt32(ComboPlan.SelectedValue);
            years.Clear();
            for (int i = startYear; i < planYear + 1; i++)
            {
                years.Add(i.ToString());
            }

            dataRotator.formNumber = 2;
            if (dataRotator.isSubjectReport) dataRotator.formNumber = 1;
            dataRotator.FillRelationValues();
            mapName = ComboMap.SelectedValue;

            diesCode = string.Empty;
            foreach (string diesName in ComboDesease.SelectedValues)
            {
                diesCode = string.Format("{0},{1}", diesCode, dataRotator.deseasesRelation[diesName]);  
            }
            diesCode = diesCode.TrimStart(',');

            CreateDataStructure(years);
            LabelFormula.Text = "Прогноз рассчитывается на основании линейной тенденции.<br>Коэффициенты уравнения линейной тенденции:";
            for (int i = 0; i < ComboGroup.SelectedValues.Count; i++)
            {
                var lineFact = new LineAppearance {Thickness = 3};
                lineFact.IconAppearance.Icon = SymbolIcon.Circle;
                lineFact.IconAppearance.IconSize = SymbolIconSize.Small;
                lineFact.IconAppearance.PE.Fill = Color.Black;
                chart.LineChart.LineAppearances.Add(lineFact);

                DataRow dr = dtChart.Rows.Add();
                dr[0] = ComboGroup.SelectedValues[i];

                string groupName = ConvertGroupName(ComboGroup.SelectedValues[i]);

                dataObject.ClearParams();
                dataObject.InitObject();
                dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
                dataObject.mainColumnRange = dataObject.GetMapCode(mapName);                                    
                for (int j = 0; j < years.Count; j++)
                {
                    dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                        years[j], string.Empty, string.Empty, groupName, diesCode);
                    dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                        Convert.ToString(j * 2 + 1));
                }
                DataTable dtTemp = dataObject.FillData();
                for (int j = 0; j < years.Count; j++)
                {
                    if (Convert.ToInt32(years[j]) <= lastYear) 
                        dr[j + 1] = dtTemp.Rows[0][j * 2 + 2];                         
                }

                double a = 0, b = 0;
                supportClass.CalculateLinearParams(dr, ComboYear.SelectedValues, years, 1, ref a, ref b);

                for (int j = 0; j < years.Count; j++)
                {
                    DataRow drGrid = dtGrid.Rows[j];
                    if (Convert.ToInt32(years[j]) <= lastYear)
                    {
                        drGrid[i * 4 + 1] = dtTemp.Rows[0][j * 2 + 1];
                        drGrid[i * 4 + 2] = dtTemp.Rows[0][j * 2 + 2];
                    }
                    drGrid[i * 4 + 4] = a * Convert.ToDouble(years[j]) + b;                        
                }             

                LabelFormula.Text = string.Format(
                    "{0}<br>Группа населения: <b>{1}</b><br> Прогноз = {2} * Год прогноза + {3}",
                    LabelFormula.Text, ComboGroup.SelectedValues[i], a, b);

                if ((a != 0) || (b != 0))
                {
                    var linePlan = new LineAppearance {LineStyle = {DrawStyle = LineDrawStyle.Dot}, Thickness = 3};
                    linePlan.IconAppearance.Icon = SymbolIcon.Circle;
                    linePlan.IconAppearance.IconSize = SymbolIconSize.Small;
                    linePlan.IconAppearance.PE.Fill = Color.Black;
                    chart.LineChart.LineAppearances.Add(linePlan);

                    dr = dtChart.Rows.Add();
                    dr[0] = string.Format("{0} (линейная)", ComboGroup.SelectedValues[i]);
                    for (int j = 0; j < years.Count; j++)
                    {
                        dr[j + 1] = a * Convert.ToDouble(years[j]) + b;
                    }
                }
            }

            for (int i = 0; i < ComboGroup.SelectedValues.Count; i++)
            {
                int cellIndex = 2 + i * 4;
                DataRow[] rows = dtGrid.Select(String.Empty, dtGrid.Columns[cellIndex].ColumnName + " desc ");
                int rank = 1;
                for (int j = 0; j < dtGrid.Rows.Count; j++)
                {
                    if (rows[j][cellIndex] != DBNull.Value)
                    {
                        DataRow tempRow = supportClass.GetTableRowValue(dtGrid, rows[j][0].ToString(), 0);
                        if (tempRow != null)
                        {
                            tempRow[cellIndex + 1] = rank++;
                        }
                    }
                }
            }

            dataRotator.FillMaxAndMin(dtGrid, 1);

            chart.TitleLeft.Extent = 30;
            chart.TitleLeft.Text = "На 100 тыс. населения";
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            chart.TitleLeft.Visible = true;

            chart.Border.Thickness = 0;
            chart.Axis.X.Extent = 30;
            chart.Axis.Y.Extent = 50;
            chart.Legend.Margins.Right = Convert.ToInt32(chart.Width.Value * 0.4);
            chart.Legend.Visible = true;
            chart.Legend.SpanPercentage = Convert.ToInt32(100 * (70 / chart.Height.Value));
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Tooltips.FormatString = "<SERIES_LABEL>\n <ITEM_LABEL> год\n заболеваемость <DATA_VALUE:N2> на 100 тыс. человек";
            chart.DataSource = dtChart;
            chart.DataBind();
            
            grid.Bands.Clear();

            grid.DataSource = dtGrid;
            grid.DataBind();

            Page.Title = "Годовая динамика и прогнозирование заболеваемости";
            LabelTitle.Text = string.Format("{0}", Page.Title);
            LabelSubTitle.Text = string.Format("{0} прогноз на {1} год (года расчета {2}) по заболеваниям: {3} {4}",
                ComboMap.SelectedValue.Trim(), 
                ComboPlan.SelectedValue,
                supportClass.GetMonthLabel(ComboYear, ComboYear.SelectedValues), 
                ComboDesease.SelectedValuesString, 
                dataRotator.GetFormHeader());
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            int dirtyHeigth = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 180) - 30;

            chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            chart.Height = Convert.ToInt32(0.55 * dirtyHeigth);
            grid.Height = Convert.ToInt32(0.4 * dirtyHeigth);
            grid.Width = chart.Width;

            SetExportHandlers();
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            grid.BorderStyle = BorderStyle.None;
            supportClass.SetColumnWidthAndCaption(grid, 0, "Год", 40, HorizontalAlign.Left, "");
            for (int i = 0; i < ComboGroup.SelectedValues.Count; i++)
            {
                supportClass.SetColumnWidthAndCaption(grid, 1 + i * 4, "Абс.", 95, HorizontalAlign.Right, "Абсолютное количество заболевших");
                CRHelper.FormatNumberColumn(grid.Columns[01 + i * 4], "N0");
                supportClass.SetColumnWidthAndCaption(grid, 2 + i * 4, "На 100 тыс. населения", 95, HorizontalAlign.Right, "Относительное количество заболевших на 100 тысяч населения территории");
                CRHelper.FormatNumberColumn(grid.Columns[02 + i * 4], "N2");
                supportClass.SetColumnWidthAndCaption(grid, 3 + i * 4, "Ранг", 50, HorizontalAlign.Right, "Ранг на 100 тысяч населения территории");
                CRHelper.FormatNumberColumn(grid.Columns[03 + i * 4], "N0");
                supportClass.SetColumnWidthAndCaption(grid, 4 + i * 4, "Прогноз", 95, HorizontalAlign.Right, "Прогноз на 100 тыс. населения территории");
                CRHelper.FormatNumberColumn(grid.Columns[04 + i * 4], "N2");
            }
            grid.Height = Unit.Empty;
            supportClass.CalculateGridColumnsWidth(grid, 1);

            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i += 4)
            {
                var ch = new ColumnHeader(true);

                if (i == 01) ch.Caption = ComboGroup.SelectedValues[0];
                if (i == 05) ch.Caption = ComboGroup.SelectedValues[1];
                if (i == 09) ch.Caption = ComboGroup.SelectedValues[2];
                if (i == 13) ch.Caption = ComboGroup.SelectedValues[3];

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 4;
                ch.RowLayoutColumnInfo.SpanX = 4;
                ch.Style.Wrap = true;
                ch.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (ComboYear.SelectedValues.IndexOf(e.Row.Cells[0].Value.ToString()) != -1)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            
            for (int i = 0; i < ComboGroup.SelectedValues.Count; i++)
            {
                supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, 3 + i * 4, 3 + i * 4, 3 + i * 4, true);
                if (e.Row.Index > 0)
                {
                    object cell1 = dtGrid.Rows[e.Row.Index - 0][2 + i * 4];
                    object cell2 = dtGrid.Rows[e.Row.Index - 1][2 + i * 4];
                    if (cell1.ToString() != string.Empty && cell2.ToString() != string.Empty)
                    {
                        double v1 = Convert.ToDouble(cell1);
                        double v2 = Convert.ToDouble(cell2);
                        double percent = 100 * (Math.Max(v1, v2) - Math.Min(v1, v2)) / Math.Max(v1, v2);
                        percent = Math.Round(percent, 2);

                        if (v1 < v2)
                        {
                            e.Row.Cells[1 + i * 4].Title = string.Format("Уменьшение заболеваемости  на {0:N2}% по сравнению с прошлым годом", percent);
                            e.Row.Cells[1 + i * 4].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        }
                        else
                        {
                            e.Row.Cells[1 + i * 4].Title = string.Format("Увеличение заболеваемости  на {0:N2}% по сравнению с прошлым годом", percent);
                            e.Row.Cells[1 + i * 4].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        }
                        e.Row.Cells[1 + i * 4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }
        }

        #region PDFExport

        protected virtual void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.PdfExporter.EndExport += PdfExporter_EndExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        protected virtual void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            InitializeExportLayout(e);

            exportClass.ExportCaptionText(e, LabelTitle.Text);
            exportClass.ExportSubCaptionText(e, LabelSubTitle.Text);
            exportClass.ExportChart(e, chart);
        }

        protected virtual void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = string.Format("0002.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        protected virtual void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            exportClass.ExportCommentText(e, LabelFormula.Text);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            chart.Width = (Unit)(chart.Width.Value - 150);
        }

        #endregion
    }
}
