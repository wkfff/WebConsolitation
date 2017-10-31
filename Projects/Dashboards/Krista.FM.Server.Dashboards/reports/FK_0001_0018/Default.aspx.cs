using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
namespace Krista.FM.Server.Dashboards.reports.FK_0001_0018
{ 
    public partial class Default : CustomReportPage
    {
        private DataTable dtData = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";

       #region  Объявление параметров
         
       #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров
            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExportButton.Visible = true;
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            //UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            //UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            { // инициализация элементов управления при первом обращении
                Label1.Text = String.Empty;
                Label2.Text = String.Empty;
                //
                
                dtData = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0018_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtData);
                endYear = Convert.ToInt32(dtData.Rows[0][0]);
                month = dtData.Rows[0][3].ToString();
                
                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
                //
                ComboYear.Title = "Год";
                ComboYear.Width = 90;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear,endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 115;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboSubject.Title = "Субъект РФ";
                ComboSubject.Width = 300;
                ComboSubject.MultiSelect = false;
                ComboSubject.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames,
                                                                                         RegionsNamingHelper.RegionsFoDictionary));
              
                //
                ComboSubject.ParentSelect = false;
                if (!string.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    ComboSubject.SetСheckedState(UserParams.StateArea.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboSubject.SetСheckedState(RegionSettings.Instance.Name, true);
                }
                //
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState("Консолидированный бюджет субъекта",true);
            }
          // 
            UserParams.Region.Value = ComboSubject.SelectedNodeParent;
            UserParams.StateArea.Value = ComboSubject.SelectedValue;
         //

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);


            Page.Title = string.Format("Источники финансирования дефицита: " +"{0}",ComboSubject.SelectedValue);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Источники финансирования дефицита бюджета субъекта РФ " + "({0}) " + "за " + "{1} " + "{2} " + "{3} " + "года в разрезе классификации источников финансирования", ComboSKIFLevel.SelectedValue, monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            UltraWebGrid.DataBind();
        }

        #region обработчик грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0018_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Таблица", dtGrid);
            UltraWebGrid.DataSource = dtGrid;  

        }

      protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
           UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }
        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (Page.IsPostBack)
              return;

            e.Layout.GroupByBox.Hidden = true;
           

           if (e.Layout.Bands[0].Columns.Count > 10)
           {
 
               e.Layout.Bands[0].HeaderStyle.Wrap = true;
               e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
               e.Layout.Bands[0].Columns[10].Hidden =true;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "КИФ","");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Код", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Исполнено, млн.руб.", "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Исполнено прошлый год, млн.руб.", "Исполнено за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Назначено, млн. руб.", "Плановые назначения на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Назначено прошлый год, млн. руб.", "Назначения в аналогичном периоде прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Темп роста назначений к прошлому году", "Темп роста назначения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Исполнено, %", "Процент выполнения назначений / Оценка равномерности исполнения (1/12 годового плана в месяц)");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Исполнено прошлый год, %", "Процент выполнения назначений за аналогичный период прошлого года");
                
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                //
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
               e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;                                                                                 
                    e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
                    e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(117);
                    e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(96);
                    e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(99);
                    e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(99);
                    e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(97);
                    e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(97);
                    e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(99);
                    e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(96);
                    e.Layout.Bands[0].Columns[9].Width = CRHelper.GetColumnWidth(99);

                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N3");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N3");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N3");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N3");
                    
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P2");
            }
 
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
           for (int i = 0; i < e.Row.Cells.Count; i++)
              {
                  if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                  { 
                   if (i == 8 )
                   {
                       // проверить правильно ли записала формулу
                       double percent = (ComboMonth.SelectedIndex + 1)*100/12;
                       if ((100 * Convert.ToDouble(e.Row.Cells[i].Value) )< percent)
                       {
                           e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                           e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                       }
                       else
                       {
                           e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                          e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                       }
                       e.Row.Cells[i].Style.Padding.Right = 2;
                   }

                  if (i == 4 || i==7) 
                  {
                      if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                      {
                          e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                          e.Row.Cells[i].Title = "Рост к прошлому году";
                      }
                      else
                       if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                      {
                          e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                          e.Row.Cells[i].Title = "Снижение к прошлому году";
                      }
                   }
                  }
              
               if (e.Row.Cells[10]!= null && e.Row.Cells[10].Value.ToString()!= string.Empty && i!=1)
               {
                   string level = e.Row.Cells[10].Value.ToString();
                   int fontSize = 8;
                  // bool bold = false;
                   bool italic = false;
                   switch (level)
                   {
                       case "Группа":
                           {
                               fontSize = 10;
                               italic = false;
                               break;
                           }
                       case "Подгруппа":
                           {
                               fontSize = 10;
                               italic = true;
                               break;
                           }
                       case "Статья":
                           {
                               fontSize = 8;
                               italic = false;
                               break;
                           }
                   }

                   e.Row.Cells[i].Style.Font.Size = fontSize;
                   //e.Row.Cells[i].Style.Font.Bold = bold;
                   e.Row.Cells[i].Style.Font.Italic = italic;
                   e.Row.Cells[i].Style.CustomRules =
                         "background-repeat: no-repeat; background-position: left center; margin: 2px";
               }
                  UltraGridCell cell = e.Row.Cells[i];
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
        #region экспорт

         #region экспорт в Excel

          private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
          {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
          }

          private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
          {
             e.CurrentWorksheet.Columns[0].Width = 300 * 37;
             e.CurrentWorksheet.Columns[1].Width = 150 * 37;
             e.CurrentWorksheet.Columns[2].Width = 130 * 37;
             e.CurrentWorksheet.Columns[3].Width = 130 * 37;
             e.CurrentWorksheet.Columns[4].Width = 130 * 37;
             e.CurrentWorksheet.Columns[5].Width = 130 * 37;
             e.CurrentWorksheet.Columns[6].Width = 130 * 37;
             e.CurrentWorksheet.Columns[7].Width = 130 * 37;
             e.CurrentWorksheet.Columns[8].Width = 130 * 37;
             e.CurrentWorksheet.Columns[9].Width = 130 * 37;

             e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "0";
             e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
             e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
             e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
             e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
             e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";

             e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "0.00%";
             e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "0.00%";
             e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "0.00%";
             e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "0.00%";

              int rowsCount = UltraWebGrid.Rows.Count+4;
              for (int i = 1; i < rowsCount; i++)
              {
                  if (i != 3 && i != 4)
                  {
                      e.CurrentWorksheet.Rows[i].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                      e.CurrentWorksheet.Rows[i].CellFormat.VerticalAlignment = Infragistics.Documents.Excel.VerticalCellAlignment.Center;
                      e.CurrentWorksheet.Rows[i].Height = 22 * 37;
                  }
              }
              e.CurrentWorksheet.Rows[1].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.False;
              e.CurrentWorksheet.Rows[1].Height = 15 * 37;

              int columnCount = UltraWebGrid.Columns.Count;
              for (int i=3; i<columnCount; i++)
              {
                  e.CurrentWorksheet.Columns[i].CellFormat.WrapText = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
                  e.CurrentWorksheet.Columns[i].CellFormat.VerticalAlignment = Infragistics.Documents.Excel.VerticalCellAlignment.Center;
              }
              e.CurrentWorksheet.Rows[3].Height = 10*37;
              e.CurrentWorksheet.Rows[4].Height = 10*37;

          }

          private void ExcelExportButton_Click(object sender, EventArgs e)
           {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
           }

         #endregion

         #region экспорт в PDF
          private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
          {
              IText title = e.Section.AddText();
              Font font = new Font("Verdana",16);
              title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
              title.Style.Font.Bold = true;
              title.AddContent(Label1.Text);

              title = e.Section.AddText();
              font = new Font("Verdana",14);
              title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
              title.AddContent(Label2.Text);
          }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }
         #endregion
        #endregion
      #endregion


    }
}