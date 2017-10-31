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


namespace Krista.FM.Server.Dashboards.reports.FO_0001_0006
{
    public partial class Default: CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private int firstYear = 2007; 
        private GridHeaderLayout headerLayout;

      #endregion

     
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 1);
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            CrossLink1.Visible = true;
            CrossLink1.Text = "Численность&nbsp;гос.&nbsp;служащих";
            CrossLink1.NavigateUrl = "~/reports/FO_0001_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Численность&nbsp;работников&nbsp;гос.&nbsp;органов";
            CrossLink2.NavigateUrl = "~/reports/FO_0001_0002/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Фактические&nbsp;затраты&nbsp;на&nbsp;содержание&nbsp;гос.&nbsp;служащих";
            CrossLink3.NavigateUrl = "~/reports/FO_0001_0005/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Фактические&nbsp;расходы&nbsp;на&nbsp;содержание&nbsp;гос.&nbsp;служащих";
            CrossLink4.NavigateUrl = "~/reports/FO_0001_0007/Default.aspx";


            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
      
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0006_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear; 
                if (dtDate.Rows[0][2].ToString() == "Квартал 4")
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                }
                else
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0])-1;
                }

                ComboYear.Title = "Год";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.SetСheckedState(endYear.ToString(),true);
                
            }

            Page.Title = "Утвержденная штатная численность государственных гражданских служащих в разрезе категорий и групп должностей";
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
                string query = DataProvider.GetQueryText("FO_0001_0006_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Категории и группы должностей", dtGrid);
                if (dtGrid.Rows.Count>0)
                {
                    for (int indRow = 0; indRow < dtGrid.Rows.Count; indRow++ )
                    {
                        dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("_", "");
                        dtGrid.Rows[indRow][0] = dtGrid.Rows[indRow][0].ToString().Replace("__", "");
                    }
                  
                    dtGrid.AcceptChanges();
                    UltraWebGrid1.DataSource = dtGrid;
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(225);

            bool flag = false;

            headerLayout.AddCell("Категории и группы должностей", "", 2);

            GridHeaderCell cell0_1 = headerLayout.AddCell("За счет средств обласного бюджета");
            GridHeaderCell cell0_2 = headerLayout.AddCell("За счет субвенций");
            
            GridHeaderCell cell1_1 = cell0_1.AddCell("Отдельные государственные органы");
            for (int i=1 ; i< 4; i++)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                captions[0] = captions[0].Replace("Костромская областная", "");
                captions[0] = captions[0].Replace("Костромской области", "");
                cell1_1.AddCell(string.Format("{0}",captions[0]));
            }
            
            GridHeaderCell cell1_2 = cell0_1.AddCell("Исполнительные органы государственной власти области по подразделам бюджетной классификации");
            GridHeaderCell cell1_3 = cell0_2.AddCell("Исполнительные органы государственной власти области по подразделам бюджетной классификации");
            for (int i=4; i<UltraWebGrid1.Columns.Count-1; i++)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                captions[0] = captions[0].Replace("Костромская областная", "");
                captions[0] = captions[0].Replace("Костромской области", "");
                if ( !flag && captions[0] != "Утвержденная штатная численность гражданских служащих в исполнительных органах, финансируемых за счет субвенций из федерального бюджета, всего")
                {
                    cell1_2.AddCell(string.Format("{0}",captions[0]));
                }
                else
                {
                    flag = true;
                    cell1_3.AddCell(string.Format("{0}", captions[0]));
                }
            }
            headerLayout.AddCell("Утвержденная штатная численность гражданских служащих в исполнительных органах, финансируемых за счет средств областного бюджета и за счет субвенций из федерального бюджета", "", 2);
           
            
            headerLayout.ApplyHeaderInfo();
            
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                }

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
            }
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[0].Style.Font.Size = 10;
                if (e.Row.Cells[0].Value.ToString() == "высшие" || e.Row.Cells[0].Value.ToString() == "главные" || e.Row.Cells[0].Value.ToString() == "ведущие" || e.Row.Cells[0].Value.ToString() == "старшие" || e.Row.Cells[0].Value.ToString() == "младшие")
                {
                    e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;
                }
                else
                {
                    e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                  
                }
                if ( Convert.ToInt32(e.Row.Cells[i].Value) == 0)
                {
                    e.Row.Cells[i].Value = "";
                }

            }


        }

        #endregion 
  

        #region экспорт

      
        #region экспорт в Excel

       
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            
            ReportExcelExporter1.HeaderCellHeight = 40;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
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
           
            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
         

        }

        #endregion

      #endregion


       }
}