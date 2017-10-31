using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Collections.ObjectModel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FNS_0004_0003
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate;
        private DataTable dtDebt;
        private DataTable dtData;
        private DataTable dtChart;
        private int firstYear = 2005;
        private int endYear = 2011;

        //параметры
        private CustomParam SelectedFO;
        private CustomParam LastMonth;
        private CustomParam LastHalfYear;
        private CustomParam LastQuarter;

        
        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.7);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров

           
            if (SelectedFO == null)
            {
                SelectedFO = UserParams.CustomParam("select_FO");
            }
            if (LastMonth == null)
            {
                LastMonth = UserParams.CustomParam("lastMonth");
            }
            if (LastHalfYear == null)
            {
                LastHalfYear = UserParams.CustomParam("lastHalfYear");
            }
            if (LastQuarter == null)
            {
                LastQuarter = UserParams.CustomParam("lastQuarter");
            }
            
            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExportButton.Visible = true;
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
       }
      
        protected override void Page_Load(Object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)   
            { //Инициализация элементов управления при первом обращении

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0004_0003_Date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                
                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                UserParams.Filter.Value = "Все федеральные округа";
                ComboFO.Title = "ФО";
                ComboFO.Width = 254;
                ComboFO.MultiSelect =false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState(UserParams.Filter.Value,true);

               if (!string.IsNullOrEmpty(UserParams.Region.Value))
                {
                    ComboFO.SetСheckedState(UserParams.Region.Value, true);
                }
               else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboFO.SetСheckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                }
          
        }
        Label1.Text = "Соотношение исполнения бюджета к задолженности";
        Label2.Text = "Соотношение консолидированного бюджета субъекта к задолженности по платежам в бюджет";  
          
         int year = Convert.ToInt32(ComboYear.SelectedValue);
        
         UserParams.PeriodYear.Value = year.ToString();
         UserParams.PeriodLastYear.Value = (year - 1).ToString();
         UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
         UserParams.PeriodHalfYear.Value =
            string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
         UserParams.PeriodQuater.Value =
                string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
         
         if (UserParams.PeriodMonth.Value=="Январь")
         {
             LastMonth.Value = "Декабрь";
             UserParams.PeriodLastYear.Value = (year - 1).ToString();
             LastHalfYear.Value =
               string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(LastMonth.Value)));
             LastQuarter.Value =
                    string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(LastMonth.Value)));
         }
         else
         {
           LastMonth.Value = CRHelper.RusMonth(ComboMonth.SelectedIndex);
           UserParams.PeriodLastYear.Value = UserParams.PeriodYear.Value;
           LastHalfYear.Value =
                          string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(LastMonth.Value)));
           LastQuarter.Value =
                  string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(LastMonth.Value)));
         }

         UltraWebGrid.Bands.Clear();
         UltraWebGrid.DataBind();
      
       }

        #region обработчик грида
       
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0004_0003_Grid");
            dtData = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtData);
           
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    for (int j = 2; j < dtData.Columns.Count; j = j + 6)
                    {
                        if (dtData.Rows[i][j] != DBNull.Value)
                        {
                            dtData.Rows[i][j] = Convert.ToDouble(dtData.Rows[i][j]) / 1000;

                        }
                        if (dtData.Rows[i][j + 2] != DBNull.Value)
                        {
                            dtData.Rows[i][j + 2] = Convert.ToDouble(dtData.Rows[i][j + 2]) / 1000;
                        }
                        if (dtData.Rows[i][j + 4] != DBNull.Value)
                        {
                            dtData.Rows[i][j + 4] = Convert.ToDouble(dtData.Rows[i][j + 4]) / 100;
                        }

                       
                    }
                }
            
           if (dtData.Columns.Count > 2)
            {
                dtData.Columns[1].ColumnName = "ФО";

            }

           UserParams.Filter.Value = ComboFO.SelectedValue;
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.DataSource = CRHelper.SetDataTableFilter(dtData, "ФО", RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            }
            else
            {
                UltraWebGrid.DataSource = dtData;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }
        
       protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i=2;i<e.Layout.Bands[0].Columns.Count; i+=6)
              {
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                  e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N2");
                  e.Layout.Bands[0].Columns[i + 2].Width = CRHelper.GetColumnWidth(100);
              }
            for (int i=3;i<e.Layout.Bands[0].Columns.Count; i+=6)
              {
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                  e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "P2");
                  e.Layout.Bands[0].Columns[i+2].Width = CRHelper.GetColumnWidth(100);
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 3], "P2");
                  e.Layout.Bands[0].Columns[i + 3].Width = CRHelper.GetColumnWidth(100);
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 4], "P2");
                  e.Layout.Bands[0].Columns[i + 4].Width = CRHelper.GetColumnWidth(100);
              }
            
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;  
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
         
            e.Layout.Bands[0].Columns[1].Header.Caption = "ФО";
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(45);
           
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0 || i == 1)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                    else
                    {
                       e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                }


           for (int i=2; i<UltraWebGrid.Columns.Count;i+=6)
           {
               string[] caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
               if (caption[0]=="Налог на доходы физических лиц")
               {
                   caption[0] = "НДФЛ";
               }

               CRHelper.SetHeaderCaption(UltraWebGrid, 0, i, "Сумма задолженности за период", "За период");
               CRHelper.SetHeaderCaption(UltraWebGrid, 0, i + 1, "Темп роста задолженности", "Темп роста задолженности к предыдущему периоду");
               CRHelper.SetHeaderCaption(UltraWebGrid, 0, i + 2, "Исполнено", "Исполнено за период");
               CRHelper.SetHeaderCaption(UltraWebGrid, 0, i + 3, "Темп роста исполнения", "Темп роста исполнения к предыдущему периоду");
               CRHelper.SetHeaderCaption(UltraWebGrid, 0, i + 4, "Соотношение исполнения к задолженности","");
               CRHelper.SetHeaderCaption(UltraWebGrid, 0, i + 5, "Темп роста соотношения исполнения к задолженности", "Темп роста соотношения исполнения к задолженности");

               CRHelper.AddHierarchyHeader(UltraWebGrid, 0, caption[0],i,0,6,1);
           }
        }

       protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
             }
           for (int i=3; i<e.Row.Cells.Count; i+=6)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double rate = 100 * Convert.ToDouble(e.Row.Cells[i].Value);
                        string hint = string.Empty;

                        if (rate >= 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            hint = "Рост к прошлому отчетному периоду";
                        }
                        else
                        {
                            if (rate < 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                hint = "Снижение к прошлому отчетному периоду";
                            }
                        }
                        e.Row.Cells[i].Title = hint;
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                    if (e.Row.Cells[i + 2].Value != null && e.Row.Cells[i + 2].Value.ToString() != string.Empty)
                    {
                        double rate = 100 * Convert.ToDouble(e.Row.Cells[i+2].Value);
                        string hint = string.Empty;

                        if (rate >= 100)
                        {
                            e.Row.Cells[i+2].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            hint = "Рост к прошлому отчетному периоду";
                        }
                        else
                        {
                            if (rate < 100)
                            {
                                e.Row.Cells[i+2].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                hint = "Снижение к прошлому отчетному периоду";
                            }
                        }
                        e.Row.Cells[i+2].Title = hint;
                        e.Row.Cells[i+2].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
               }
               foreach (UltraGridCell cell in e.Row.Cells)
               {
                   if (cell.Value != null && cell.Value.ToString() != string.Empty)
                   {
                       decimal value;
                       if (decimal.TryParse(cell.Value.ToString(), out value))
                       {
                           if (value < 0)
                           {
                               cell.Style.ForeColor = Color.Red;
                           }
                       }
                   }
               }
        }

        #endregion

   
        #region экспорт
          #region экспорт в PDF
        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
            {
                IText title = e.Section.AddText();
                System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
                title.Style.Font = new Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(Label1.Text);

                title = e.Section.AddText();
                font = new System.Drawing.Font("Verdana", 14);
                title.Style.Font = new Font(font);
                title.AddContent(Label2.Text);
                
               /* title = e.Section.AddText();
                font = new System.Drawing.Font("Verdana", 14);
                title.Style.Font = new Font(font);
                title.AddContent(string.Format("{0}.{1}", ComboYear.SelectedValue, ComboMonth.SelectedValue));
            */
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i+=6)
                {
                    e.Layout.Bands[0].Columns[i].Width = 120;
                    e.Layout.Bands[0].Columns[i+1].Width = 120;
                    e.Layout.Bands[0].Columns[i+2].Width = 120;
                    e.Layout.Bands[0].Columns[i+3].Width = 120;
                    e.Layout.Bands[0].Columns[i+4].Width = 120;
                }

                for (int i = 6; i < e.Layout.Bands[0].Columns.Count; i += 6)
                {
                    e.Layout.Bands[0].Columns[i].Width = 120;
                    e.Layout.Bands[0].Columns[i + 1].Width = 160;
                }
            }
        
       
        private void PdfExporter_EndExport(Object sender, EndExportEventArgs e)
            {
               
           }
          private void PdfExportButton_Click(Object sender, EventArgs e)
            {
                UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
        
                UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
               }
     #endregion
          #region экспорт в Excel
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
           // e.CurrentWorksheet.Rows[2].Cells[0].Value = string.Format("{0}.{1}", ComboYear.SelectedValue, ComboMonth.SelectedValue);
        }

        private int offset;
        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex+offset];
            if (col.Hidden)
            {
                offset++;
                
                col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            }
            else if (e.CurrentColumnIndex<2)
            {
                e.HeaderText = col.Header.Caption;
            }
            else if (e.CurrentColumnIndex<8)
            {
                e.HeaderText = "НДФЛ";
            }
            else
            {
                e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];    
            }
            
        }
          private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
          {
              int columnCount = UltraWebGrid.Columns.Count;

              e.CurrentWorksheet.Columns[0].Width = 200*37;
              e.CurrentWorksheet.Columns[1].Width = 50*37;
              
              e.CurrentWorksheet.Rows[3].Height = 15 * 37;
              e.CurrentWorksheet.Rows[4].Height = 25 * 37;
              
              int i;
                  for (i = 2; i < UltraWebGrid.Columns.Count; i+=6)
                  {
                      e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.000;[Red]-#,##0.000";
                      e.CurrentWorksheet.Columns[i].Width = 120 * 37;
                      e.CurrentWorksheet.Columns[i + 2].CellFormat.FormatString = "#,##0.000;[Red]-#,##0.000";
                      e.CurrentWorksheet.Columns[i + 2].Width = 130 * 37;
                  }
                  for (i=3;i<UltraWebGrid.Columns.Count;i+=6)
                  {
                      e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "0.00%";
                      e.CurrentWorksheet.Columns[i].Width = 110 * 37;
                      e.CurrentWorksheet.Columns[i + 2].CellFormat.FormatString = "0.00%";
                      e.CurrentWorksheet.Columns[i + 2].Width = 110 * 37;
                      e.CurrentWorksheet.Columns[i + 3].CellFormat.FormatString = "0.00%";
                      e.CurrentWorksheet.Columns[i + 3].Width = 110 * 37;
                      e.CurrentWorksheet.Columns[i + 4].CellFormat.FormatString = "0.00%";
                      e.CurrentWorksheet.Columns[i + 4].Width = 110 * 37;
                  }
              
             for ( i = 2; i < columnCount + 4; i++)
              {
                  e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                  e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                  e.CurrentWorksheet.Rows[4].Height = 30 * 37;
              }

              for ( i = 4; i < UltraWebGrid.Rows.Count + 4; i++)
              {
                  e.CurrentWorksheet.Rows[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                  e.CurrentWorksheet.Rows[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                  e.CurrentWorksheet.Rows[i].Height = 17 * 37;
              }

          }
          private void ExcelExportButton_Click(object sender, EventArgs e)
          {
              UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
              UltraGridExporter1.ExcelExporter.DownloadName = "reports.xls";
              UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
          }
    #endregion
         
#endregion

      }
}



