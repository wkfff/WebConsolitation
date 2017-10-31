using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using InitializeRowEventHandler=Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;

namespace Krista.FM.Server.Dashboards.reports.FO_0005_0004
{
    public partial class Default : CustomReportPage
    {
       
        #region Поля

        private DataTable dtGrid = new DataTable();
        private int firstYear = 2006;
        private int endYear;
        private string month = string.Empty;
        private int day;
        private DateTime date;


        private GridHeaderLayout headerLayout;
        private static MemberAttributesDigest rzprDigest;
        private static MemberAttributesDigest csrDigest;

        #endregion

        #region Параметры запроса

        private CustomParam AdminType;
        private CustomParam rzprType;
        private CustomParam csrType;
        private CustomParam vrType;
        private CustomParam Year;
        // имя выбранного раздела/подраздела
        private CustomParam selectedRzPrCaption;
        // имя выбранной целевой статьи
        private CustomParam selectedCSRCaption;
        // имя выбранного ГРБС
        private CustomParam selectedGRBSCaption;
        // имя выбранного вида расхода
        private CustomParam selectedVRCaption;
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/1.4);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
           
           #region Инициализация параметров
            if (AdminType == null)
            {
                AdminType = CustomParam.CustomParamFactory("admin_type");
            }

            if (rzprType == null)
            {
                rzprType = CustomParam.CustomParamFactory("rspr_type");
            }

            if (csrType == null)
            {
                csrType = CustomParam.CustomParamFactory("csr_type");
            }

            if (vrType == null)
            {
                vrType = CustomParam.CustomParamFactory("vr_type");
            }

            if (Year == null)
            {
                Year = CustomParam.CustomParamFactory("year");
            }

            selectedRzPrCaption = UserParams.CustomParam("selected_RzPr_caption");
            selectedCSRCaption = UserParams.CustomParam("selected_CSR_caption");
            selectedGRBSCaption = UserParams.CustomParam("selected_GRBS_caption");
            selectedVRCaption = UserParams.CustomParam("selected_VR_caption");

            #endregion
        }

      
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                string query = DataProvider.GetQueryText("FO_0005_0004_date");
                DataTable dtDate = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query,dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();
                day = Convert.ToInt32(dtDate.Rows[0][4]);
                ComboYear.Title = "Год";
                ComboYear.Width = 90;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                Year.Value = 2011.ToString();
                
                ComboBill.Width = 500;
                ComboBill.Title = "ГРБС";
                ComboBill.MultiSelect = false;
                ComboBill.ParentSelect = true;
                selectedGRBSCaption.Value = "Все ГРБС"; 

                ComboRzPr.Width = 500;
                ComboRzPr.Title ="РзПр";
                ComboRzPr.MultiSelect = false;
                ComboRzPr.ParentSelect = true;
                selectedRzPrCaption.Value = "Все разделы/подразделы";

                ComboCSR.Width = 550;
                ComboCSR.Title = "ЦСР";
                ComboCSR.MultiSelect = false;
                ComboCSR.ParentSelect = true;
                selectedCSRCaption.Value = "Все целевые статьи";

                ComboVR.Width = 550;
                ComboVR.Title = "ВР";
                ComboVR.MultiSelect = false;
                ComboVR.ParentSelect = true;
                selectedVRCaption.Value = "Все виды расходов";
            }

            Year.Value = ComboYear.SelectedValue;

            if (ComboBill.SelectedValue != String.Empty)
            {
                selectedGRBSCaption.Value = ComboBill.SelectedValue;
            }

            FillAdmin();
            ComboBill.ClearNodes();
            ComboBill.FillDictionaryValues(admin);
            ComboBill.SetСheckedState("Все ГРБС", true);
            ComboBill.SetСheckedState(selectedGRBSCaption.Value, true);

            if (ComboVR.SelectedValue != String.Empty)
            {
                selectedVRCaption.Value = ComboVR.SelectedValue;
            }

            FillKVR();
            ComboVR.ClearNodes();
            ComboVR.FillDictionaryValues(KVR);
            ComboVR.SetСheckedState("Все виды расходов", true);
            ComboVR.SetСheckedState(selectedVRCaption.Value, true);
            
            if (ComboRzPr.SelectedValue != String.Empty)
            {
                selectedRzPrCaption.Value = ComboRzPr.SelectedValue;
            }

            rzprDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "NovosibRZPRType");
            ComboRzPr.ClearNodes();
            ComboRzPr.FillDictionaryValues(
                CustomMultiComboDataHelper.FillMemberUniqueNameList(rzprDigest.UniqueNames, rzprDigest.MemberLevels));
            ComboRzPr.SetСheckedState("Все разделы/подразделы", true); 
            ComboRzPr.SetСheckedState(selectedRzPrCaption.Value, true);

            if (ComboCSR.SelectedValue != String.Empty)
            {
                selectedCSRCaption.Value = ComboCSR.SelectedValue;
            }

            csrDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "NovosibCSRType");
            ComboCSR.ClearNodes();
            ComboCSR.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(
            csrDigest.UniqueNames, csrDigest.MemberLevels));
            ComboCSR.SetСheckedState("Все целевые статьи", true);
            ComboCSR.SetСheckedState(selectedCSRCaption.Value, true);
           
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            
            rzprType.Value = rzprDigest.GetMemberUniqueName(ComboRzPr.SelectedValue);
            csrType.Value = csrDigest.GetMemberUniqueName(ComboCSR.SelectedValue);

            if (ComboBill.SelectedValue == "Все ГРБС")
             {
                AdminType.Value = string.Format("[ФО АС Бюджет - УФиНП {0}]", year);
             }
            else
            {
               string[] name = ComboBill.SelectedValue.Split(')');
               string fullName = ComboBill.SelectedValue.Replace(name[0],"");
               fullName = fullName.Replace(") ", "");
               AdminType.Value = string.Format("[ФО АС Бюджет - УФиНП {0}].[{1}]", year, fullName);
             }
 
            if (ComboVR.SelectedValue == "Все виды расходов")
            {
              vrType.Value = string.Format("[ФО АС Бюджет - УФиНП {0}]", year);
            }
            else
            {
              string[] name = ComboVR.SelectedValue.Split(')');
              string fullName = ComboVR.SelectedValue.Replace(name[0], "");
              fullName = fullName.Replace(") ", "");
              vrType.Value = string.Format("[ФО АС Бюджет - УФиНП {0}].[{1}]",year, fullName);
            }

            string queryMonth = DataProvider.GetQueryText("FO_0005_0004_month");
            DataTable dtMonth = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(queryMonth, dtMonth);
            endYear = Convert.ToInt32(dtMonth.Rows[0][0]);
            month = dtMonth.Rows[0][3].ToString();
            day = Convert.ToInt32(dtMonth.Rows[0][4]);

            date = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(month), day);

            PageTitle.Text = "Исполнение областного бюджета по расходам";
            Page.Title = "Исполнение областного бюджета по расходам";
            PageSubTitle.Text = String.Format("Данные за {1} год (по состоянию на {5:dd.MM.yyyy} г.), {0}, {2}, {3}, {4}", ComboBill.SelectedValue, ComboYear.SelectedValue, ComboRzPr.SelectedValue, ComboCSR.SelectedValue, ComboVR.SelectedValue, date);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

       protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0005_0004_Grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                for (int rowNum = 0; rowNum < dtGrid.Rows.Count; rowNum ++ )
                {
                    if (dtGrid.Rows[rowNum][6].ToString() != string.Empty && dtGrid.Rows[rowNum][6] != DBNull.Value && dtGrid.Rows[rowNum][5].ToString() != string.Empty && dtGrid.Rows[rowNum][5] != DBNull.Value )
                    {
                        if (Convert.ToDouble(dtGrid.Rows[rowNum][5]) != 0 && Convert.ToDouble(dtGrid.Rows[rowNum][5])>0)
                        {
                            dtGrid.Rows[rowNum][7] = Convert.ToDouble(dtGrid.Rows[rowNum][6])/
                                                     Convert.ToDouble(dtGrid.Rows[rowNum][5]);
                        }
                        else
                        {
                            dtGrid.Rows[rowNum][7] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dtGrid.Rows[rowNum][7] = DBNull.Value;
                    }
                  if (dtGrid.Rows[rowNum][0].ToString() != string.Empty)
                  {
                      dtGrid.Rows[rowNum][0] = TrimCode(dtGrid.Rows[rowNum][0].ToString());
                  }

                  
                }

                int sum = 0;
                int sumI = 0;
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    if (dtGrid.Rows[i][8].ToString() == "2")
                    {
                        if (dtGrid.Rows[i][5] != DBNull.Value && dtGrid.Rows[i][5].ToString() != string.Empty)
                        {
                            sum = sum + Convert.ToInt32(dtGrid.Rows[i][5]);
                        }
                        if (dtGrid.Rows[i][6] != DBNull.Value && dtGrid.Rows[i][6].ToString() != string.Empty)
                        {
                            sumI = sumI + Convert.ToInt32(dtGrid.Rows[i][6]);
                        }
                    }

                }

                DataRow row = dtGrid.NewRow();
                row[0] = "Итого";
                row[1] = DBNull.Value;
                row[2] = DBNull.Value;
                row[3] = DBNull.Value;
                row[4] = DBNull.Value;
                row[5] = sum;
                row[6] = sumI;
                if (row[5] != DBNull.Value && row[5].ToString() != string.Empty && row[6] != DBNull.Value && row[6].ToString() != string.Empty)
                {
                    row[7] = Convert.ToDouble(row[6]) / Convert.ToDouble(row[5]);
                }
                else
                {
                    row[7] = DBNull.Value;
                }
                dtGrid.Rows.Add(row);

                dtGrid.AcceptChanges();
                UltraWebGrid.DataSource = dtGrid;
            }
        }

       private string TrimCode(string name)
       {
           for (int i = 0; i < name.Length; i++)
           {
               decimal value;
               if (!Decimal.TryParse(name[i].ToString(), out value))
               {
                   return name.Substring(i);
               }
           }
           return name;
       }

       protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(400);
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            headerLayout.AddCell("Наименование расходов");
            headerLayout.AddCell("ГРБС","Код ГРБС");
            headerLayout.AddCell("РзПр","Код РзПр");
            headerLayout.AddCell("ЦСР", "Код ЦСР");
            headerLayout.AddCell("ВР", "Код ВР");
            headerLayout.AddCell("Утвержденный план, тыс. руб.", "Утвержденный годовой план по расходам по сводной бюджетной росписи");
            headerLayout.AddCell("Исполнено, тыс. руб.", "Кассовый расход нарастающим итогом с начала года");
            headerLayout.AddCell("% исполнения плана", "Процент исполнения плана по расходам");

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(60);
              
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "000");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "00 00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "000 00 00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "000");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(107);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N2");
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(107);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");
            e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(107);

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            double value;

            if (endYear.ToString() != ComboYear.SelectedValue)
            {
               value = 1;
            }
            else
            {
               int ch = CRHelper.MonthNum(month.ToLower());
               value = Convert.ToDouble(ch) / 12.0;
            }

            int level = Convert.ToInt32(e.Row.Cells[UltraWebGrid.Columns.Count - 1].Value);
            if (level == 2 || level == 0)
            {
                for (int i = 0; i < UltraWebGrid.Columns.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            if (e.Row.Cells[7].Value != null && e.Row.Cells[7].Value.ToString() != string.Empty)
            {
                if (100 * Convert.ToDouble(e.Row.Cells[7].Value) < value *100)
                {
                    e.Row.Cells[7].Style.BackgroundImage = "~/images/ballRedBB.png";
                    e.Row.Cells[7].Title = string.Format("Не соблюдается условие равномерности ({0:P2})",value);
                }
                else if (100 * Convert.ToDouble(e.Row.Cells[7].Value) >= value * 100)
                {
                    e.Row.Cells[7].Style.BackgroundImage = "~/images/ballGreenBB.png";
                    e.Row.Cells[7].Title = string.Format("Соблюдается условие равномерности ({0:P2})",value);
                }

                e.Row.Cells[7].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }

           
        }

       #endregion


        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            
            ReportExcelExporter1.HeaderCellHeight = 33;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            
        }

        #endregion


        #region Экспорт в PDF

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

        private Dictionary<string, int> admin = new Dictionary<string, int>();
        private void FillAdmin()
        {
            DataTable dtAdmin = new DataTable();
            string query = DataProvider.GetQueryText("FO_0005_0004_adminType");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtAdmin);

            admin.Add("Все ГРБС", 0);
            foreach (DataRow row in dtAdmin.Rows)
            {
                admin.Add(String.Format("({1}) {0}", row[0], row[1]), 0);
            }
        }

       private Dictionary<string, int> KVR = new Dictionary<string, int>();
       private void FillKVR()
       {
           DataTable dtKVR = new DataTable();
           string query = DataProvider.GetQueryText("FO_0005_0004_vrType");
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел/подраздел", dtKVR);

           KVR.Add("Все виды расходов", 0);
           foreach (DataRow row in dtKVR.Rows)
           {
               KVR.Add(string.Format("({1}) {0}", row[0],row[1]), 0);
           }

       }
    }
}