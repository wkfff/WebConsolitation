using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment=System.Drawing.ContentAlignment;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0048
{
    public partial class Default : CustomReportPage
    {
        #region поля

        private DataTable dtDate = new DataTable();
        private string query;
        private int firstYear = 2011;
        private int endYear;
        private string quarter;
        private int year;
        private string adminName;
        private DateTime currentDate;

        private static MemberAttributesDigest grbsDigest;
        #endregion

        #region параметры запроса

        private CustomParam adminType;
        private CustomParam typeDebts;
        private CustomParam currentFin;
        private CustomParam listMeans;
        private CustomParam socialSecurity;
        

        #endregion


        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;
        private GridHeaderLayout headerLayout3;
        private GridHeaderLayout headerLayout4;
        private GridHeaderLayout headerLayout5;
        private GridHeaderLayout headerLayout6;
        private GridHeaderLayout headerLayout7;
        private GridHeaderLayout headerLayout8;
        private GridHeaderLayout headerLayout9;

       protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid3.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid4.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid5.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid6.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid7.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid8.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid9.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            
            #region Инициализация параматров
           
             adminType = UserParams.CustomParam("admin_type");
             typeDebts = UserParams.CustomParam("type_debit");
             currentFin = UserParams.CustomParam("current_fin");
             listMeans = UserParams.CustomParam("list_means");
             socialSecurity = UserParams.CustomParam("soc_security");

            #endregion
           
           ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
           ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
           
        }

      protected override void Page_Load(object sender, EventArgs e)
       {
           base.Page_PreLoad(sender, e);

           if (!Page.IsPostBack)
           {
               dtDate = new DataTable();
               query = DataProvider.GetQueryText("FO_0002_0048_date");
               DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
               endYear = Convert.ToInt32(dtDate.Rows[0][0]);
               quarter = dtDate.Rows[0][2].ToString();

               ComboYear.Title = "Год";
               ComboYear.Width = 100;
               ComboYear.MultiSelect = false;
               ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
               ComboYear.SetСheckedState(endYear.ToString(), true);

               Dictionary <string, int> quarters = new Dictionary<string, int>();
               quarters.Add("Квартал 2",0);
               quarters.Add("Квартал 3",0);
               quarters.Add("Квартал 4",0);

               ComboQuarter.Title = "Квартал";
               ComboQuarter.Width = 150;
               ComboQuarter.MultiSelect = false;
               ComboQuarter.FillDictionaryValues(quarters);
               ComboQuarter.SetСheckedState(quarter, true);

               Dictionary<string,int> means = new Dictionary<string, int>();
               means.Add("Бюджетные средства",0);
               means.Add("Внебюджетные средства",0);
               means.Add("Всего по бюджетным и внебюджетным средствам", 0);

               ComboMeans.Title = "Тип средств";
               ComboMeans.Width = 250;
               ComboMeans.MultiSelect = true;
               ComboMeans.FillDictionaryValues(means);
               ComboMeans.SetСheckedState("Бюджетные средства", true);
               ComboMeans.SetСheckedState("Внебюджетные средства", true);
               ComboMeans.SetСheckedState("Всего по бюджетным и внебюджетным средствам", true);

               Dictionary<string,int> typeKredit = new Dictionary<string, int>();
               typeKredit.Add("Дебиторская задолженность",0);
               typeKredit.Add("Просроченная дебиторская задолженность", 0);
               typeKredit.Add("Кредиторская задолженность", 0);
               typeKredit.Add("Просроченная кредиторская задолженность", 0);

               ComboKredit.Title = "Задолженность";
               ComboKredit.Width = 300;
               ComboKredit.MultiSelect = false;
               ComboKredit.FillDictionaryValues(typeKredit);
               ComboKredit.SetСheckedState("Дебиторская задолженность",true);

               grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "VologdaGRBSType");
               ComboGRBS.Title = "ГРБС";
               ComboGRBS.Width = 600;
               ComboGRBS.MultiSelect = false;
               ComboGRBS.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(grbsDigest.UniqueNames, grbsDigest.MemberLevels));
               ComboGRBS.SetСheckedState("Законодательное Собрание области", true);

           }

          string kreditType = string.Empty;
          switch (ComboKredit.SelectedIndex)
          {
              case 0:
                  {
                      kreditType = "дебиторской задолженности";
                      typeDebts.Value = "[Задолженность].[Вид задолженности].[Все].[Дебиторская задолженность]";
                      break;
                  }
              case 1:
                  {
                      kreditType = "просроченной дебиторской задолженности";
                      typeDebts.Value ="[Задолженность].[Вид задолженности].[Все].[Просроченная дебиторская задолженность]";
                      break;
                  }
              case 2:
                  {
                      kreditType = "кредиторской задолженности";
                      typeDebts.Value = "[Задолженность].[Вид задолженности].[Все].[Кредиторская задолженность]";
                      break;
                  }
             case 3:
                  {
                      kreditType = "просроченной кредиторской задолженности";
                      typeDebts.Value ="[Задолженность].[Вид задолженности].[Все].[Просроченная кредиторская задолженность]";
                      break;
                  }
          }

            currentDate = new DateTime(ComboQuarter.SelectedIndex == 2 ? Convert.ToInt32(ComboYear.SelectedValue)+1 : Convert.ToInt32(ComboYear.SelectedValue), ComboQuarter.SelectedIndex == 0 ? 7 : ComboQuarter.SelectedIndex == 1 ? 10 : 1, 1);
            Page.Title = string.Format("Детализация информации о состоянии {0} в разрезе разделов ФКР по направлениям финансирования на {1:dd.MM.yyyy} г., тыс. руб.", kreditType, currentDate );
            Label1.Text = Page.Title;
            Label2.Text = string.Empty;

            year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodQuater.Value = ComboQuarter.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(ComboQuarter.SelectedIndex+2));

            adminType.Value = grbsDigest.GetMemberUniqueName(ComboGRBS.SelectedValue);
            adminName = ComboGRBS.SelectedValue;

            socialSecurity.Value = ComboYear.SelectedValue != "2011"
                                     ? "  [Задолженность].[Показатели].[Задолженность Всего].[Задолженность всего, в т.ч.:].[Социальное обеспечение],"
                                     : string.Empty;
            listMeans.Value = string.Empty;
            if (ComboMeans.SelectedValues.Count>0)
            {
                string str = string.Empty;
                for (int i=0; i<ComboMeans.SelectedValues.Count; i++)
                {
                    str += string.Format("[Тип средств].[СКИФ].[Все].[{0}] ,", ComboMeans.SelectedValues[i]);
                }
                listMeans.Value = str.TrimEnd(',');
            }

            Label3.Text = string.Empty;
            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0048_Lable");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count > 0)
            {
              double sum = Convert.ToDouble(dtDate.Rows[0][0]);
              Label3.Text = string.Format("Задолженность, всего: {0:N2} тыс. руб.", sum);
            }

            grid1Captions.Text = string.Format("Информация о состоянии {0} по аппарату управления", kreditType);
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            grid2Captions.Text = string.Format("Информация о состоянии {0} по подведомственным учреждениям, Всего", kreditType);
            headerLayout2 = new GridHeaderLayout(UltraWebGrid2);
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            grid7Captions.Text = string.Format("Информация о состоянии {0} по бюджетным учреждениям", kreditType);
            headerLayout7 = new GridHeaderLayout(UltraWebGrid7);
            UltraWebGrid7.Bands.Clear();
            UltraWebGrid7.DataBind();

            grid8Captions.Text = string.Format("Информация о состоянии {0} по автономным учреждениям", kreditType);
            headerLayout8 = new GridHeaderLayout(UltraWebGrid8);
            UltraWebGrid8.Bands.Clear();
            UltraWebGrid8.DataBind();

            grid9Captions.Text = string.Format("Информация о состоянии {0} по казенным учреждениям", kreditType);
            headerLayout9 = new GridHeaderLayout(UltraWebGrid9);
            UltraWebGrid9.Bands.Clear();
            UltraWebGrid9.DataBind();

            grid3Captions.Text = string.Format("Информация о состоянии {0} по капительному строительству", kreditType);
            headerLayout3 = new GridHeaderLayout(UltraWebGrid3);
            UltraWebGrid3.Bands.Clear();
            UltraWebGrid3.DataBind();

            grid4Captions.Text = string.Format("Информация о состоянии {0} по мероприятиям", kreditType);
            headerLayout4 = new GridHeaderLayout(UltraWebGrid4);
            UltraWebGrid4.Bands.Clear();
            UltraWebGrid4.DataBind();

            grid5Captions.Text = string.Format("Информация о состоянии {0} по целевым программам", kreditType);
            headerLayout5 = new GridHeaderLayout(UltraWebGrid5);
            UltraWebGrid5.Bands.Clear();
            UltraWebGrid5.DataBind();

            grid6Captions.Text = string.Format("Информация о состоянии {0} по прочим расходам", kreditType);
            headerLayout6 = new GridHeaderLayout(UltraWebGrid6);
            UltraWebGrid6.Bands.Clear();
            UltraWebGrid6.DataBind();
          
          }

       #region Обработчики грида

       protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[аппарат управления]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count >0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid1.DataSource = dtGrid1;
           }

       }

       protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[подведомственные учреждения]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count > 0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid2.DataSource = dtGrid1;
           }

       }

       protected void UltraWebGrid3_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[капитальное строительство]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count > 0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid3.DataSource = dtGrid1;
           }

       }
       protected void UltraWebGrid4_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[мероприятия]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count > 0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid4.DataSource = dtGrid1;
           }

       }
       protected void UltraWebGrid5_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[целевые программы]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count > 0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid5.DataSource = dtGrid1;
           }

       }
       protected void UltraWebGrid6_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[прочие расходы]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count > 0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid6.DataSource = dtGrid1;
           }
       }

       protected void UltraWebGrid7_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[подведомственные учреждения].[бюджетные учреждения]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count > 0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid7.DataSource = dtGrid1;
           }
       }

       protected void UltraWebGrid8_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[подведомственные учреждения].[автономные учреждения]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count > 0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid8.DataSource = dtGrid1;
           }
       }

       protected void UltraWebGrid9_DataBinding(object sender, EventArgs e)
       {
           currentFin.Value = "[Задолженность].[Направления финансирования].[Все].[подведомственные учреждения].[казенные учреждения]";
           string query = DataProvider.GetQueryText("FO_0002_0048_Grid1");
           DataTable dtGrid1 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Раздел ФКР", dtGrid1);
           if (dtGrid1.Rows.Count > 0)
           {
               dtGrid1.Rows[0][0] = adminName;
               dtGrid1.AcceptChanges();
               UltraWebGrid9.DataSource = dtGrid1;
           }
       }

       void UltraWebGrid1_DataBound(object sender, EventArgs e)
       {
           ((UltraWebGrid) sender).Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
           ((UltraWebGrid)sender).Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2);
           ((UltraWebGrid)sender).DisplayLayout.NoDataMessage = "Задолженность отсутствует";

           if (((UltraWebGrid) sender ).Rows.Count <= 10 )
           {
             ((UltraWebGrid) sender).Height = Unit.Empty;
           }
       }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid1.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid1.Columns.Count-1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid1.Columns.Count/2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid1.Columns.Count/3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);
          

            headerLayout1.AddCell("Разделы ФКР");
            for (int i = 1; i < UltraWebGrid1.Columns.Count; i+=countColumns )
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout1.AddCell(string.Format("{0} по аппарату управления",caption0[0]));

                for (int j = i; j < i + countColumns-1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]),"");
                    
                    if (dtGridCod.Rows.Count > 0)
                    {
                      for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++ )
                       {
                         if (caption1.Contains(string.Format("{0}",dtGridCod.Rows[rowNum][0])))
                           {
                             cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на аппарат управления по состоянию на {0:dd.MM.yyyy} г. ({1})",  currentDate, caption0[0].ToLower()));
                           }
                      }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на аппарат управления по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
            }

            headerLayout1.ApplyHeaderInfo();

        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        { 
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid2.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid2.Columns.Count - 1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid2.Columns.Count / 2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid2.Columns.Count / 3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);


            headerLayout2.AddCell("Разделы ФКР");
            for (int i = 1; i < UltraWebGrid2.Columns.Count; i += countColumns)
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout2.AddCell(string.Format("{0} по подведомственным учреждениям, всего", caption0[0]));

                for (int j = i; j < i + countColumns - 1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]), "");
                    
                    if (dtGridCod.Rows.Count > 0)
                    {
                        for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++)
                        {
                            if (caption1.Contains(string.Format("{0}", dtGridCod.Rows[rowNum][0])))
                            {
                                cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на подведомственные учреждения по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
                            }
                        }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на подведомственные учреждения по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
            }

            headerLayout2.ApplyHeaderInfo();

        }

        protected void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid3_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid3.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid3.Columns.Count - 1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid3.Columns.Count / 2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid3.Columns.Count / 3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);


            headerLayout3.AddCell("Разделы ФКР");
            for (int i = 1; i < UltraWebGrid3.Columns.Count; i += countColumns)
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout3.AddCell(string.Format("{0} по капительному строительству", caption0[0]));

                for (int j = i; j < i + countColumns - 1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]), "");
                    
                    if (dtGridCod.Rows.Count > 0)
                    {
                        for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++)
                        {
                            if (caption1.Contains(string.Format("{0}", dtGridCod.Rows[rowNum][0])))
                            {
                                cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на капитальное строительство по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
                            }
                        }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на капитальное строительство по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
            }

            headerLayout3.ApplyHeaderInfo();

        }

        protected void UltraWebGrid3_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid4_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid4.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid4.Columns.Count - 1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid4.Columns.Count / 2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid4.Columns.Count / 3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);


            headerLayout4.AddCell("Разделы ФКР");
            for (int i = 1; i < UltraWebGrid4.Columns.Count; i += countColumns)
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout4.AddCell(string.Format("{0} по мероприятиям", caption0[0]));

                for (int j = i; j < i + countColumns - 1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]), "");

                    if (dtGridCod.Rows.Count > 0)
                    {
                        for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++)
                        {
                            if (caption1.Contains(string.Format("{0}", dtGridCod.Rows[rowNum][0])))
                            {
                                cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на мероприятия по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
                            }
                        }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на мероприятия по состоянию на {0:dd.MM.yyyy} г. ({1})",  currentDate, caption0[0].ToLower()));
            }

            headerLayout4.ApplyHeaderInfo();

        }

        protected void UltraWebGrid4_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid5_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid5.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid5.Columns.Count - 1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid5.Columns.Count / 2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid5.Columns.Count / 3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);


            headerLayout5.AddCell("Разделы ФКР");
            for (int i = 1; i < UltraWebGrid5.Columns.Count; i += countColumns)
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout5.AddCell(string.Format("{0} по целевым программам", caption0[0]));

                for (int j = i; j < i + countColumns - 1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]), "");

                    if (dtGridCod.Rows.Count > 0)
                    {
                        for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++)
                        {
                            if (caption1.Contains(string.Format("{0}", dtGridCod.Rows[rowNum][0])))
                            {
                                cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на целевые программы по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
                            }
                        }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на целевые программы по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
            }

            headerLayout5.ApplyHeaderInfo();

        }

        protected void UltraWebGrid5_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid6_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid6.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid6.Columns.Count - 1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid6.Columns.Count / 2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid6.Columns.Count / 3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);


            headerLayout6.AddCell("Разделы ФКР");
          for (int i = 1; i < UltraWebGrid6.Columns.Count; i += countColumns)
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout6.AddCell(string.Format("{0} по прочим расходам", caption0[0]));

                for (int j = i; j < i + countColumns - 1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]), "");

                    if (dtGridCod.Rows.Count > 0)
                    {
                        for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++)
                        {
                            if (caption1.Contains(string.Format("{0}", dtGridCod.Rows[rowNum][0])))
                            {
                                cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на прочие расходы по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
                            }
                        }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на прочие расходы по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
            }
            
            headerLayout6.ApplyHeaderInfo();

        }

        protected void UltraWebGrid6_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid7_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid7.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid7.Columns.Count - 1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid7.Columns.Count / 2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid7.Columns.Count / 3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);


            headerLayout7.AddCell("Разделы ФКР");
            for (int i = 1; i < UltraWebGrid7.Columns.Count; i += countColumns)
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout7.AddCell(string.Format("{0} по бюджетным учреждениям", caption0[0]));

                for (int j = i; j < i + countColumns - 1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]), "");

                    if (dtGridCod.Rows.Count > 0)
                    {
                        for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++)
                        {
                            if (caption1.Contains(string.Format("{0}", dtGridCod.Rows[rowNum][0])))
                            {
                                cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на бюджетные учреждения по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
                            }
                        }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на бюджетные учреждения по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
            }

            headerLayout7.ApplyHeaderInfo();

        }

        protected void UltraWebGrid7_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid8_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid8.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid8.Columns.Count - 1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid8.Columns.Count / 2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid8.Columns.Count / 3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);


            headerLayout8.AddCell("Разделы ФКР");
            for (int i = 1; i < UltraWebGrid8.Columns.Count; i += countColumns)
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout8.AddCell(string.Format("{0} по автономным учреждениям", caption0[0]));

                for (int j = i; j < i + countColumns - 1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]), "");

                    if (dtGridCod.Rows.Count > 0)
                    {
                        for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++)
                        {
                            if (caption1.Contains(string.Format("{0}", dtGridCod.Rows[rowNum][0])))
                            {
                                cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на автономные учреждения по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
                            }
                        }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на автономные учреждения по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
            }

            headerLayout8.ApplyHeaderInfo();

        }

        protected void UltraWebGrid8_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }

        protected void UltraWebGrid9_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;


            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < UltraWebGrid9.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
            }

            int countColumns = 0;
            if (ComboMeans.SelectedValues.Count > 0)
            {
                switch (ComboMeans.SelectedValues.Count)
                {
                    case 1:
                        {
                            countColumns = UltraWebGrid9.Columns.Count - 1;
                            break;
                        }
                    case 2:
                        {
                            countColumns = UltraWebGrid9.Columns.Count / 2;
                            break;
                        }
                    case 3:
                        {
                            countColumns = UltraWebGrid9.Columns.Count / 3;
                            break;
                        }
                }
            }

            string query = DataProvider.GetQueryText("FO_0002_0048_Cod");
            DataTable dtGridCod = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Коды КОСГУ", dtGridCod);


            headerLayout9.AddCell("Разделы ФКР");
            for (int i = 1; i < UltraWebGrid9.Columns.Count; i += countColumns)
            {
                string[] caption0 = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell = headerLayout9.AddCell(string.Format("{0} по казенным учреждениям", caption0[0]));

                for (int j = i; j < i + countColumns - 1; j++)
                {
                    string caption1 = e.Layout.Bands[0].Columns[j].Header.Caption.Replace(string.Format("{0};", caption0[0]), "");

                    if (dtGridCod.Rows.Count > 0)
                    {
                        for (int rowNum = 0; rowNum < dtGridCod.Rows.Count; rowNum++)
                        {
                            if (caption1.Contains(string.Format("{0}", dtGridCod.Rows[rowNum][0])))
                            {
                                cell.AddCell(string.Format("{0} ({1})", caption1, dtGridCod.Rows[rowNum][1]), string.Format("задолженность по расходам на казенные учреждения по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
                            }
                        }
                    }
                    else
                    {
                        cell.AddCell(string.Format("{0}", caption1));
                    }

                }
                cell.AddCell(string.Format("Задолженность всего, в т.ч.:"), string.Format("задолженность по расходам на казенные учреждения по состоянию на {0:dd.MM.yyyy} г. ({1})", currentDate, caption0[0].ToLower()));
            }

            headerLayout9.ApplyHeaderInfo();

        }

        protected void UltraWebGrid9_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
        }
   #endregion 

       #region Экспорт
       #region Экспорт в Excel
      private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            Worksheet sheet4 = workbook.Worksheets.Add("sheet4");
            Worksheet sheet5 = workbook.Worksheets.Add("sheet5");
            Worksheet sheet6 = workbook.Worksheets.Add("sheet6");
            Worksheet sheet7 = workbook.Worksheets.Add("sheet7");
            Worksheet sheet8 = workbook.Worksheets.Add("sheet8");
            Worksheet sheet9 = workbook.Worksheets.Add("sheet9");
            

            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.GridColumnWidthScale = 1.5;

            ReportExcelExporter1.Export(headerLayout1,grid1Captions.Text ,sheet1, 3);
            ReportExcelExporter1.Export(headerLayout2, grid2Captions.Text, sheet2, 3);
            ReportExcelExporter1.Export(headerLayout7, grid7Captions.Text, sheet3, 3);
            ReportExcelExporter1.Export(headerLayout8, grid8Captions.Text, sheet4, 3);
            ReportExcelExporter1.Export(headerLayout9, grid9Captions.Text, sheet5, 3);
            ReportExcelExporter1.Export(headerLayout3, grid3Captions.Text, sheet6, 3);
            ReportExcelExporter1.Export(headerLayout4, grid4Captions.Text, sheet7, 3);
            ReportExcelExporter1.Export(headerLayout5, grid5Captions.Text, sheet8, 3);
            ReportExcelExporter1.Export(headerLayout6, grid6Captions.Text, sheet9, 3);
           
            
        }

    #endregion

       #region Экспорт в Pdf
     private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            ISection section4 = report.AddSection();
            ISection section5 = report.AddSection();
            ISection section6 = report.AddSection();
            ISection section7 = report.AddSection();
            ISection section8 = report.AddSection();
            ISection section9 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout1, grid1Captions.Text ,section1);
            ReportPDFExporter1.Export(headerLayout2, grid2Captions.Text,section2);
            ReportPDFExporter1.Export(headerLayout7, grid7Captions.Text, section3);
            ReportPDFExporter1.Export(headerLayout8, grid8Captions.Text, section4);
            ReportPDFExporter1.Export(headerLayout9, grid9Captions.Text, section5);
            ReportPDFExporter1.Export(headerLayout3, grid3Captions.Text, section6);
            ReportPDFExporter1.Export(headerLayout4, grid4Captions.Text, section7);
            ReportPDFExporter1.Export(headerLayout5, grid5Captions.Text, section8);
            ReportPDFExporter1.Export(headerLayout6, grid6Captions.Text, section9);
            
            
        }

   #endregion
    #endregion

    }
}
