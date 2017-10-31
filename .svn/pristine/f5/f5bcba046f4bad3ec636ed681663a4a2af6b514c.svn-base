using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.UltraChart.Core.Primitives;


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0005_XMAO
{
    public partial class Default: CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private int firstYear = 2008;
        private int endYear;
       
        private GridHeaderLayout headerLayout;

      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -10);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/1.5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            CrossLink1.Visible = true;
            CrossLink1.Text = "Численность&nbsp;работников&nbsp;ОГВ&nbsp;и&nbsp;ОМСУ";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0003/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Штатная&nbsp;численность&nbsp;ОГВ&nbsp;и&nbsp;ОМСУ";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0006_XMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Анализ&nbsp;среднемесячной&nbsp;зарплаты&nbsp;ОГВ&nbsp;и&nbsp;ОМСУ";
            CrossLink3.NavigateUrl = "~/reports/FO_0001_0007_XMAO/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Анализ&nbsp;расходов&nbsp;на&nbsp;ОГВ&nbsp;и&nbsp;ОМСУ";
            CrossLink4.NavigateUrl = "~/reports/FO_0001_0003_XMAO/Default.aspx";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0005_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                string quarter = dtDate.Rows[0][2].ToString();
                
                if (dtDate.Rows[0][2].ToString() == "Квартал 4")
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                }
                else
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]) - 1;
                }
                
                ComboYear.Title = "Год";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.SetСheckedState(endYear.ToString(), true);
                
            }

            if (DebtKindButtonList.SelectedIndex == 0)
            {
                Page.Title = "Фактические затраты на заработную плату работников органов государственной власти Ханты-Мансийского автономного округа-Югры";
            }
            else
            {
                Page.Title = "Фактические затраты на заработную плату работников органов местного самоуправления и избирательных комиссий МО Ханты-Мансийского автономного округа-Югры";
            }
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("Данные за {0} г.", ComboYear.SelectedValue);

            UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
       
           
        }

        #region Обработчик грида

       

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            if (DebtKindButtonList.SelectedIndex == 0) // Форма_14
            {
                string query = DataProvider.GetQueryText("FO_0001_0005_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "ГРБС",
                                                                                 dtGrid);
                if (dtGrid.Rows.Count > 1)
                {
                   
                   /* int colLastYear = 9;
                    int colCurYear = 10;
                    for (int indRow = 0; indRow < dtGrid.Rows.Count; indRow++)
                    {
                        if (dtGrid.Rows[indRow][2].ToString() != string.Empty && dtGrid.Rows[indRow][2] != DBNull.Value &&
                            dtGrid.Rows[indRow][6].ToString() != string.Empty &&
                            dtGrid.Rows[indRow][6] != DBNull.Value)
                        {
                            dtGrid.Rows[indRow][colCurYear] = (Convert.ToDouble(dtGrid.Rows[indRow][2]) /
                                                               Convert.ToDouble(dtGrid.Rows[indRow][6]) / 12);
                        }
                        if (dtGrid.Rows[indRow][1].ToString() != string.Empty && dtGrid.Rows[indRow][1] != DBNull.Value &&
                            dtGrid.Rows[indRow][5].ToString() != string.Empty &&
                            dtGrid.Rows[indRow][5] != DBNull.Value)
                        {
                            dtGrid.Rows[indRow][colLastYear] = (Convert.ToDouble(dtGrid.Rows[indRow][1]) /
                                                                Convert.ToDouble(dtGrid.Rows[indRow][5])) / 12;
                        }
                        if (dtGrid.Rows[indRow][colCurYear].ToString() != string.Empty &&
                            dtGrid.Rows[indRow][colCurYear] != DBNull.Value &&
                            dtGrid.Rows[indRow][colLastYear].ToString() != string.Empty &&
                            dtGrid.Rows[indRow][colLastYear] != DBNull.Value)
                        {
                            dtGrid.Rows[indRow][colCurYear + 1] = Convert.ToDouble(dtGrid.Rows[indRow][colCurYear]) -
                                                                  Convert.ToDouble(dtGrid.Rows[indRow][colLastYear]);
                        }

                    }

                    for (int indCol = 4; indCol < dtGrid.Columns.Count; indCol += 4)
                    {
                        for (int indRow = 0; indRow < dtGrid.Rows.Count; indRow++)
                        {
                            if (dtGrid.Rows[indRow][indCol - 2].ToString() != string.Empty &&
                                dtGrid.Rows[indRow][indCol - 2] != DBNull.Value &&
                                dtGrid.Rows[indRow][indCol - 3].ToString() != string.Empty &&
                                dtGrid.Rows[indRow][indCol - 3] != DBNull.Value)
                            {
                                dtGrid.Rows[indRow][indCol] = Convert.ToDouble(dtGrid.Rows[indRow][indCol - 2]) /
                                                              Convert.ToDouble(dtGrid.Rows[indRow][indCol - 3]);
                            }
                        }
                    }*/

                    dtGrid.AcceptChanges();
                    UltraWebGrid1.DataSource = dtGrid;
                }
            }
            else // Форма_14 МО
            {
                string query = DataProvider.GetQueryText("FO_0001_0005_Grid_MO");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                                 "МО",
                                                                                 dtGrid);
                if (dtGrid.Rows.Count>1)
                {
                    UltraWebGrid1.DataSource = dtGrid;
                }

            }
        }

      
        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);

            if (DebtKindButtonList.SelectedIndex == 0)
            {
                headerLayout.AddCell("Наименование государственного органа", "", 2);

                GridHeaderCell cell0 = headerLayout.AddCell("Фактически начисленная заработная плата работников государственного органа, тыс.руб");
                cell0.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Фактически начислено в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell0.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Фактически начислено в {0} году", ComboYear.SelectedValue));
                cell0.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell1 = headerLayout.AddCell("в т.ч. фактически начисленная заработная плата лиц, замещающих должности государственной гражданской службы, тыс.руб.");
                cell1.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Фактически начислено в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell1.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Фактически начислено в {0} году", ComboYear.SelectedValue));
                cell1.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell2 = headerLayout.AddCell("Среднесписочная численность работников государственного органа, чел.");
                cell2.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднесписочная численность в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell2.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднесписочная численность в {0} году", ComboYear.SelectedValue));
                cell2.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell3 = headerLayout.AddCell("в т.ч. среднесписочная численность лиц, замещающих должности государственной гражданской службы, чел.");
                cell3.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднесписочная численность в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell3.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднесписочная численность в {0} году", ComboYear.SelectedValue));
                cell3.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell4 = headerLayout.AddCell("Среднемесячная начисленная заработная плата работников государственного органа, тыс.руб./чел.");
                cell4.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднемесячная заработная плата в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell4.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднемесячная заработная плата в {0} году", ComboYear.SelectedValue));
                cell4.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell5 = headerLayout.AddCell("Среднемесячная начисленная заработная плата лиц, замещающих должности государственной гражданской службы, тыс.руб./чел.");
                cell5.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднемесячная заработная плата в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell5.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднемесячная заработная плата в {0} году", ComboYear.SelectedValue));
                cell5.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

            }
            else
            {
                headerLayout.AddCell("Наименование муниципального образования", "", 2);

                GridHeaderCell cell0 = headerLayout.AddCell("Фактически начисленная заработная плата работников органа местного самоуправления, избирательной комиссии МО, тыс.руб");
                cell0.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Фактически начислено в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell0.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Фактически начислено в {0} году", ComboYear.SelectedValue));
                cell0.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell1 = headerLayout.AddCell("в т.ч. фактически начисленная заработная плата лиц, замещающих должности муниципальной службы, тыс.руб.");
                cell1.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Фактически начислено в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell1.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Фактически начислено в {0} году", ComboYear.SelectedValue));
                cell1.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell2 = headerLayout.AddCell("Среднесписочная численность работников органа местного самоуправления, избирательной комиссии МО, чел.");
                cell2.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднесписочная численность в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell2.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднесписочная численность в {0} году", ComboYear.SelectedValue));
                cell2.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell3 = headerLayout.AddCell("в т.ч. среднесписочная численность лиц, замещающих должности муниципальной службы, чел.");
                cell3.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднесписочная численность в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell3.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднесписочная численность в {0} году", ComboYear.SelectedValue));
                cell3.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell4 = headerLayout.AddCell("Среднемесячная начисленная заработная плата работников органа местного самоуправления, избирательной комиссии МО, тыс.руб./чел.");
                cell4.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднемесячная заработная плата в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell4.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднемесячная заработная плата в {0} году", ComboYear.SelectedValue));
                cell4.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

                GridHeaderCell cell5 = headerLayout.AddCell("Среднемесячная начисленная заработная плата лиц, замещающих должности муниципальной службы, тыс.руб./чел.");
                cell5.AddCell(string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1), string.Format("Среднемесячная заработная плата в {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1));
                cell5.AddCell(string.Format("{0}", ComboYear.SelectedValue), string.Format("Среднемесячная заработная плата в {0} году", ComboYear.SelectedValue));
                cell5.AddCell("Отклонение", "Прирост/снижение относительно прошлого отчетного года");

            }
            
            headerLayout.ApplyHeaderInfo();

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(73);
            }
            for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i+=3 )
            {
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P1");
               e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(78);
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");
            e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(55);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N0");
            e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(55);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N0");
            e.Layout.Bands[0].Columns[10].Width = CRHelper.GetColumnWidth(55);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "N0");
            e.Layout.Bands[0].Columns[11].Width = CRHelper.GetColumnWidth(55);

            e.Layout.Bands[0].Columns[13].Width = CRHelper.GetColumnWidth(55);
            e.Layout.Bands[0].Columns[14].Width = CRHelper.GetColumnWidth(55);
            e.Layout.Bands[0].Columns[16].Width = CRHelper.GetColumnWidth(55);
            e.Layout.Bands[0].Columns[17].Width = CRHelper.GetColumnWidth(55);
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            e.Row.Cells[i].Style.ForeColor = Color.Red;
                        }
                    }
                }

                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;

                if (e.Row.Cells[0].Value.ToString() == "Итого" || e.Row.Cells[0].Value.ToString() == "Cреднее значение" || e.Row.Cells[0].Value.ToString() == "Среднее значение")
                {
                    
                    e.Row.Cells[0].Style.Font.Bold = true;
                }

                if (i == 3 || i==6)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                      {
                          if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyDownBB.png";
                              e.Row.Cells[i].Title = "Затраты сократились к прошлому году";
                          }
                          else if (100*Convert.ToDouble(e.Row.Cells[i].Value) >0)
                          {
                              e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyUpBB.png";
                              e.Row.Cells[i].Title = "Затраты увеличились к прошлому году";
                          }

                          e.Row.Cells[i].Style.CustomRules =
                              "background-repeat: no-repeat; background-position: left center; margin: 2px";
                      }
                }

                if (i == 9 || i==12)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyDownBB.png";
                            e.Row.Cells[i].Title = "Среднесписочная численность сократилась к прошлому году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyUpBB.png";
                            e.Row.Cells[i].Title = "Среднесписочная численность увеличилась к прошлому году";
                        }

                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (i == 15 || i==18 )
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому отчетному году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreyUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому отчетному году";
                        }

                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }
              
        }

        #endregion 
  

        #region экспорт

      
        #region экспорт в Excel

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            
        }
        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 5; j < rowsCount + 5; j++)
                {
                   e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Font.Height = 200;
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в Pdf

        
        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
           
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
         

        }

        #endregion

      #endregion


       }
}