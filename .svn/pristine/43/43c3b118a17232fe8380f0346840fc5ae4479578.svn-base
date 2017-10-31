using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;
using Infragistics.UltraChart.Shared.Events;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0006
{
    public partial class sgm_0006 : CustomReportPage
    {
        string year, months;
        int selectedYear;
        const int deseaseCount = 7;
        int screenWidth;
        private string mapName;
        private string rfName;
        private string monthText;
        private string yearsCmp;

        private DataTable tblFullDataDes;

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            double dirtyHeight = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 180).Value;
            grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 35);
            screenWidth = (int)grid.Width.Value;
            grid.Height = (Unit)dirtyHeight;
            chart.Width = CustomReportConst.minScreenWidth - 735;
            grid.Width = 700;
            chart.Height = grid.Height;
            SetExportHandlers();
        }

        private void FillReportParams()
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            dataRotator.formNumber = 1;
            dataObject.InitObject();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);

            if (!Page.IsPostBack)
            {
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillYearList(ComboCompare);
                year = ComboYear.SelectedValue;
                ComboCompare.SetСheckedState(year, false);
                ComboCompare.SetСheckedState(Convert.ToString(Convert.ToInt32(year) - 1), true);
                dataRotator.FillMonthListEx(ComboMonth, year);
                supportClass.FillPeopleGroupList(ComboGroup);
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, true);

                if (dataRotator.fullMapList.Count > 0)
                {
                    ComboMap.SetСheckedState(dataRotator.fullMapList[0], true);
                }
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, true);
            }

            dataRotator.FillDeseasesList(null, 0);
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            year = ComboYear.SelectedValue;
            months = dataRotator.GetMonthParamString(ComboMonth, year);
            mapName = ComboMap.SelectedValue;
            dataRotator.CheckFormNumber(selectedYear, ref months);
            rfName = supportClass.GetRootMapName(dataObject.dtAreaShort);
            monthText = dataRotator.GetMonthParamLabel(ComboMonth, year);
            yearsCmp = ComboCompare.SelectedValuesString;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            FillReportParams();
            FillData();
            ConfigureChart();

            tblFullDataDes.Columns.RemoveAt(8);
            tblFullDataDes.Columns.RemoveAt(8);

            grid.DataBind();

            Page.Title = String.Format("Превышение контрольного ({0}) уровня заболеваемости", rfName);
            LabelTitle.Text = Page.Title;
            LabelSubTitle.Text = String.Format("{0}, за {1} {2} год{4}, группа населения: {3}{5}",
                                               mapName,
                                               monthText,
                                               year, 
                                               ComboGroup.SelectedValue,
                                               dataRotator.GetYearAppendix(), 
                                               dataRotator.GetFormHeader());
        }

        private void FillData()
        {
            string deseasesCodes = dataRotator.deseasesCodes[0];
            const string groupName = "0";
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.mainColumnRange = deseasesCodes;
            dataObject.useLongNames = true;
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                 year, months, mapName, groupName, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                 "1");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                 yearsCmp, months, mapName, groupName, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                 "3");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                 year, months, dataRotator.mapList[0], groupName, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                 "5");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                                 "2", "6");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                                 "4", "6");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctShortDeseaseName,
                                 String.Empty);

            tblFullDataDes = dataObject.FillData();
            tblFullDataDes = dataObject.FilterDataSet(tblFullDataDes,
                String.Format("{0} > 0 and {1} > 0", 
                tblFullDataDes.Columns[1].ColumnName, 
                tblFullDataDes.Columns[5].ColumnName));
            tblFullDataDes = dataObject.SortDataSet(tblFullDataDes, 7, true);

            foreach (DataRow rowDies in tblFullDataDes.Rows)
            {
                rowDies[7] = 100 * Convert.ToDouble(rowDies[7]);
            }

            grid.Bands.Clear();
            grid.DataSource = tblFullDataDes;
        }

        private void ConfigureChart()
        {
            chart.Tooltips.FormatString = "<SERIES_LABEL> <ITEM_LABEL> <DATA_VALUE>%";
            chart.Border.Thickness = 0;
            chart.Axis.X.Extent = 1;
            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.Font = new Font("Verdana", 10);
            chart.Legend.SpanPercentage = Convert.ToInt32(100 * 78 / chart.Height.Value);
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE>%";
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.Axis.Y.Labels.Visible = false;
            chart.Axis.Y.Extent = 150;
            chart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 15, FontStyle.Regular);
            GetChartData();
            chart.DataBind();          
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Header.Caption = "Наименование заболеваний";
            e.Layout.Bands[0].Columns[0].Width = 125;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 68;
                CRHelper.FormatNumberColumn(grid.Columns[i], "N0");
            }

            CRHelper.FormatNumberColumn(grid.Columns[02], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[04], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[06], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[07], "N2");

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

            const string asbColumnName = "абс.";
            const string asbColumnTips = "Абсолютный показатель заболеваемости за {0} {1} года";
            const string relColumnName = "на 100 тыс. населения";
            const string relColumnTips = "Относительный показатель заболеваемости на 100 тысяч населения за {0} {1} года";
            const string pctColumnName = "%";

            CRHelper.SetHeaderCaption(grid, 0, 1, asbColumnName, String.Format(asbColumnTips, monthText, year));
            CRHelper.SetHeaderCaption(grid, 0, 2, relColumnName, String.Format(relColumnTips, monthText, year));
            CRHelper.SetHeaderCaption(grid, 0, 3, asbColumnName, String.Format(asbColumnTips, monthText, yearsCmp));
            CRHelper.SetHeaderCaption(grid, 0, 4, relColumnName, String.Format(relColumnTips, monthText, yearsCmp));

            string levelCaption1 = String.Format("Уровень заболеваемости в {0}", rfName);
            string levelCaption2 = String.Format("Уровень заболеваемости в {0} на 100 тыс. населения", rfName);
            string levelCaption3 = String.Format("Процент от уровня заболеваемости в {0}", rfName);

            CRHelper.SetHeaderCaption(grid, 0, 5, asbColumnName, levelCaption1);
            CRHelper.SetHeaderCaption(grid, 0, 6, relColumnName, levelCaption2);
            CRHelper.SetHeaderCaption(grid, 0, 7, pctColumnName, levelCaption3);

            var ch = new ColumnHeader(true)
                         {
                             Caption = String.Format("За {0} {1} года", monthText, year),
                             RowLayoutColumnInfo =
                                 {
                                     OriginY = 0,
                                     OriginX = 1,
                                     SpanX = 2
                                 }
                         };

            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            if (ComboCompare.SelectedValues.Count == 1)
            {
                ch.Caption = String.Format("За {0} {1} года", monthText, yearsCmp);
            }
            else
            {
                ch.Caption = String.Format("Среднемноголетний уровень ({0} {1})", monthText, yearsCmp);
            }

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 3;
            ch.RowLayoutColumnInfo.SpanX = 2;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true)
                     {
                         Caption = String.Format("Уровень {0}", rfName),
                         RowLayoutColumnInfo =
                             {
                                 OriginY = 0,
                                 OriginX = 5,
                                 SpanX = 3
                             }
                     };

            e.Layout.Bands[0].HeaderLayout.Add(ch);

            if (HttpContext.Current.Request.UserAgent != null)
            {
                if (grid.Height.Value > 80)
                {
                    grid.Height = (Unit) (grid.Height.Value - 80);
                }
            }
        }

        protected virtual void ClearCellStyle(UltraGridCell cell, double v1, double v2)
        {
            double difValue = Math.Abs(v1 - v2);

            if (difValue < 11)
            {
                if (difValue == 0) cell.Style.CssClass = "";
            }
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row == null) return;

            double iv1 = Convert.ToDouble(e.Row.Cells[2].Value);
            double iv3 = Convert.ToDouble(e.Row.Cells[6].Value);

            UltraGridCell cell = e.Row.Cells[7];

            if (iv1 > iv3)
            {
                if (iv3 != 0)
                {
                    cell.Title = "Максимальный уровень заболеваемости превышен";
                    cell.Style.BackgroundImage = "~/images/ballRedBB.png";
                }
            }
            else
            {
                cell.Title = "Максимальный уровень заболеваемости не превышен";
                cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
            }

            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
        }

        protected DataTable GetChartData()
        {
            var dtResult = new DataTable();

            DataColumn dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");

            for (int i = 0; i < deseaseCount; i++)
            {
                dataColumn = dtResult.Columns.Add();
                dataColumn.DataType = Type.GetType("System.Double");
                dataColumn.ColumnName = Convert.ToString(tblFullDataDes.Rows[i][0]).Trim();
            }

            DataRow dr = dtResult.Rows.Add();
            dr[0] = yearsCmp;
            dr = dtResult.Rows.Add();
            dr[0] = selectedYear;
            dr = dtResult.Rows.Add();
            dr[0] = String.Format("Уровень {0}", rfName);

            for (int i = 0; i < deseaseCount; i++)
            {
                dtResult.Rows[0][i + 1] = Math.Round(Convert.ToDouble(tblFullDataDes.Rows[i][8]) * 100, 3);
                dtResult.Rows[1][i + 1] = Math.Round(Convert.ToDouble(tblFullDataDes.Rows[i][7]), 3);
                dtResult.Rows[2][i + 1] = 100;
            }

            for (int i = 0; i < deseaseCount; i++)
            {
                int rowIndex = deseaseCount - i - 1;
                NumericSeries series1 = CRHelper.GetNumericSeries(deseaseCount - i, dtResult);
                series1.Label = Convert.ToString(tblFullDataDes.Rows[rowIndex][9]).Trim();
                chart.Series.Add(series1);
            }
            return dtResult;
        }

        protected void chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            var colors = new Color[5];
            colors[0] = Color.Green;
            colors[1] = Color.Coral;
            colors[2] = Color.DarkBlue;
            colors[3] = Color.Firebrick;
            colors[4] = Color.Ivory;

            string captionLevel = String.Format("уровня {0}", rfName);
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                var primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    var box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        int cellIndex;
                        DataRow primitiveRow = tblFullDataDes.Rows[deseaseCount - box.Row - 1];
                        if (box.DataPoint.Label == String.Format("Уровень {0}", rfName))
                        {
                            box.PE.FillStopColor = Color.SteelBlue;
                            box.PE.Fill = Color.DodgerBlue;
                            if (primitiveRow != null)
                            {
                                box.DataPoint.Label = String.Format("{0} {1:N2} на 100 тыс. {2} год",
                                    box.DataPoint.Label, primitiveRow[6], year);
                            }
                            
                            cellIndex = 6;
                        }
                        else
                        {
                            if (box.DataPoint.Label == ComboYear.SelectedValue)
                            {
                                if (Convert.ToDouble(primitiveRow[7]) <= 100)
                                {
                                    box.DataPoint.Label = String.Format("(ниже {1}) {0} год", 
                                        box.DataPoint.Label, 
                                        captionLevel);

                                    box.PE.Fill = Color.LightGreen;
                                    box.PE.FillStopColor = Color.DarkGreen;
                                }
                                else
                                {
                                    box.DataPoint.Label = String.Format("(выше {1}) {0} год", 
                                        box.DataPoint.Label, 
                                        captionLevel);
                                    box.PE.Fill = Color.Red;
                                    box.PE.FillStopColor = Color.Maroon;
                                }
                                cellIndex = 2;
                            }
                            else
                            {
                                box.PE.Fill = Color.LightGray;
                                box.PE.FillStopColor = Color.DarkGray;
                                cellIndex = 4;
                            }
                        }
                        var text = new Text
                                       {
                                           PE = {Fill = Color.Black},
                                           bounds =
                                               new Rectangle(box.rect.X, box.rect.Y, box.rect.Width, box.rect.Height)
                                       };
                        if (primitiveRow != null)
                        {
                            text.SetTextString(String.Format("{0:N2}", primitiveRow[cellIndex]));
                        }
                        text.labelStyle.Orientation = TextOrientation.Horizontal;
                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;

                        text.bounds = box.rect.Width > 40 ? 
                            new Rectangle(box.rect.X, box.rect.Y, box.rect.Width, box.rect.Height) : 
                            new Rectangle(box.rect.Right + 2, box.rect.Y, text.GetTextString().Length * 7, box.rect.Height);

                        e.SceneGraph.Add(text);
                    }
                    else if (i != 0 && box.Path == "Legend")
                    {
                        Primitive primitive1 = e.SceneGraph[i - 0];
                        Primitive primitive2 = e.SceneGraph[i - 1];
                        if (primitive2 is Text && primitive1 is Box)
                        {
                            var text = (Text)primitive2;
                            if (text.GetTextString() != ComboYear.SelectedValue)
                            {
                                if (text.GetTextString() == String.Format("Уровень {0}", rfName))
                                {
                                    box.PE.FillStopColor = Color.SteelBlue;
                                    box.PE.Fill = Color.DodgerBlue;
                                    text.SetTextString(String.Format("{0} {1}", text.GetTextString(), year));
                                    text.bounds.Width = 300;
                                }
                                else
                                {
                                    box.PE.Fill = Color.LightGray;
                                    box.PE.FillStopColor = Color.DarkGray;
                                    text.SetTextString(String.Format("{0} {1}", ComboMap.SelectedValue.Trim(), text.GetTextString()));
                                    text.bounds.Width = 300;
                                }
                            }
                            else
                            {
                                box.PE.Fill = Color.LightGreen;
                                box.PE.FillStopColor = Color.Maroon;
                                text.SetTextString(String.Format("{0} {1}", ComboMap.SelectedValue.Trim(), text.GetTextString()));
                                text.bounds.Width = 300;
                            }

                        }
                    }
                }
            }
        }

        #region PDFExport

        protected virtual void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
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
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0006.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            grid.Width = screenWidth - 200;
            chart.Width = grid.Width;
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 100;
            }
        }

        #endregion
    }
}
