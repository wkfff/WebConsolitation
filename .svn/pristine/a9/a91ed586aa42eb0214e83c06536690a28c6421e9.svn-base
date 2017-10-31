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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0047
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtGridMO;
        private DataTable dtChart1;
        private DataTable dtChart2;
       
        private int firstYear = 2010;
        private int endYear;
        private string quarter;
        private int year;
        int index;
        private DateTime currentMember;
        private DateTime prevCurDate;

        private CustomParam typeDebit;
        private CustomParam typeBudget;
        private CustomParam list;
        private CustomParam adminType;
        private CustomParam measures;
        private CustomParam dimentions;
        private CustomParam municipalUnion;
        private CustomParam firstSTR;
        private CustomParam typeKredit;
        private CustomParam listMeasures;



        private GridHeaderLayout headerLayout;

       protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);

            
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);
            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);

            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 1.8);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 1.8);
           
            #region Инициализация параматров
           
            typeDebit = UserParams.CustomParam("type_debit");
            list = UserParams.CustomParam("list");
            adminType = UserParams.CustomParam("admin_type");
            measures = UserParams.CustomParam("measures");
            typeBudget = UserParams.CustomParam("type_budget");
            dimentions = UserParams.CustomParam("dimentions");
            municipalUnion = UserParams.CustomParam("mun_union");
            firstSTR = UserParams.CustomParam("first_str");
            typeKredit = UserParams.CustomParam("type_kredit");
            listMeasures = UserParams.CustomParam("list_measures");

            #endregion
           
            #region Настройка диаграмм
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Extent = 150;
            UltraChart1.Axis.Y.Extent = 60;
           // UltraChart1.Tooltips.FormatString = " <DATA_VALUE:N2> тыс. руб.";
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Data.SwapRowsAndColumns = true;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "Тыс.руб.";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 12;
           
            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart2.Data.ZeroAligned = true;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Extent = 180;
            UltraChart2.Axis.Y.Extent = 60;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> \n <SERIES_LABEL> \n <DATA_VALUE:N2> тыс. руб. ";
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Data.SwapRowsAndColumns = true;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = "Тыс.руб.";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 12;
        
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
         
            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid1.ClientID;
           
        }

   
        
       protected override void Page_Load(object sender, EventArgs e)
       {
           base.Page_PreLoad(sender, e);

           if (!Page.IsPostBack)
           {
               CheckBox1.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", CheckBox2.ClientID));
               CheckBox2.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", CheckBox1.ClientID));

               CheckBox4.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", CheckBox5.ClientID));
               CheckBox5.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", CheckBox4.ClientID));

               dtDate = new DataTable();
               string query = DataProvider.GetQueryText("FO_0002_0047_date");
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

              }

            Page.Title = "Динамика кредиторской и дебиторской задолженности";
            Label1.Text = "Динамика кредиторской и дебиторской задолженности";

            currentMember = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), 7, 1);
            int numberQuarter=2;
            switch (ComboQuarter.SelectedIndex)
             {
                case 0:
                   {
                     currentMember = new DateTime(Convert.ToInt32(ComboYear.SelectedValue),7,1);
                     prevCurDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue) - 1, 7, 1);
                     numberQuarter = 2;
                     break;
                   }
               case 1:
                   {
                     currentMember = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), 10, 1);
                     prevCurDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue)-1, 10, 1);
                     numberQuarter = 3;
                       break;
                   }

              case 2:
                   {
                       currentMember = new DateTime(Convert.ToInt32(ComboYear.SelectedValue)+1, 1, 1);
                       prevCurDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), 1, 1);
                       numberQuarter = 4;
                       break;
                   }
             }

            Label2.Text = string.Format("Информация по состоянию на {0:dd.MM.yyyy} г., тыс. руб.",currentMember);
            year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodHalfYear.Value =string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(numberQuarter));
            UserParams.PeriodQuater.Value = ComboQuarter.SelectedValue;

            if (DebtKindButtonList2.SelectedIndex == 0)
            {
                list.Value = "[ГРБС]";
                dimentions.Value = "[Администратор].[Сопоставим]";
            }
            else
            {
                list.Value = "[Разделы]";
                dimentions.Value = "[РзПр].[Сопоставимый]";
            }

            if (RadioButtonList1.SelectedIndex == 0)
            {
                measures.Value = "[Задолженность].[Вид задолженности].[Все].[Дебиторская задолженность]";

              if (CheckBox6.Checked)
               {
                    measures.Value = "[Задолженность].[Вид задолженности].[Все].[Просроченная дебиторская задолженность]";
               }
            }
            else
            {
                measures.Value = "[Задолженность].[Вид задолженности].[Все].[Кредиторская задолженность]";

                if (CheckBox6.Checked)
                {
                    measures.Value = "[Задолженность].[Вид задолженности].[Все].[Просроченная кредиторская задолженность]";
                }
            }
            if (CheckBox4.Checked && CheckBox5.Checked)
            {
                typeBudget.Value = "[Тип средств].[СКИФ].[Все].[Бюджетные и внебюджетные средства]";
            }
            else if (CheckBox4.Checked)
            {
                typeBudget.Value = "[Тип средств].[СКИФ].[Все].[Бюджетные средства]";
            }
            else
            {
                typeBudget.Value = "[Тип средств].[СКИФ].[Все].[Внебюджетные средства]";
            }


            if (ComboQuarter.SelectedValue == "Квартал 4")
            {
                listMeasures.Value = "[Measures].[Аналогичный период прошлого года], [Measures].[Выбранный период], [Measures].[Темп роста к аналогичному периоду предыдущего года]";
            }
            else
            {
                listMeasures.Value = "[Measures].[На первый квартал] , [Measures].[Аналогичный период прошлого года], [Measures].[Выбранный период], [Measures].[Темп роста к аналогичному периоду предыдущего года], [Measures].[Темп роста к показателям на начало года]";
            }

            index = ComboQuarter.SelectedValue == "Квартал 4" ? 3 : 5;

            chart1ElementCaption.Text = string.Format("Динамика {0} {1} задолженности в разрезе муниципальных образований в сравнении с началом отчетного года ({2} средства)", CheckBox6.Checked ? "просроченной" : "", RadioButtonList1.SelectedIndex == 0 ? "дебиторской" : "кредиторской", CheckBox4.Checked && CheckBox5.Checked ? "Бюджетные и внебюджетные" : CheckBox4.Checked ? "Бюджетные" : "Внебюджетные");
            chart2ElementCaption.Text = string.Format("Динамика {0} {1} задолженности в разрезе {2} в сравнении с началом отчетного года ({3} средства)", CheckBox6.Checked ? "просроченной" : "", RadioButtonList1.SelectedIndex == 0 ? "дебиторской" : "кредиторской", DebtKindButtonList2.SelectedIndex == 0 ? "главных распорядителей бюджетных средств " : " разделов ФКР", CheckBox4.Checked && CheckBox5.Checked ? "Бюджетные и внебюджетные" : CheckBox4.Checked ? "Бюджетные" : "Внебюджетные");

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            UltraChart1.DataBind();
            UltraChart2.DataBind();

          }

       #region Обработчики грида

       protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
       {
          if (CheckBox1.Checked && CheckBox2.Checked) // выбрана кредиторская и дебиторская задолженности 
           {
               typeDebit.Value = "[Задолженность].[Вид задолженности].[Все].[Дебиторская задолженность]";
               adminType.Value = string.Format("{0}.[ ДЕБИТОРСКАЯ ЗАДОЛЖЕННОСТЬ, ВСЕГО]",dimentions.Value);
               firstSTR.Value = string.Format("{0}.[Строка с произвольными данными для корректной работы формата],", dimentions.Value);
               typeKredit.Value = "[Задолженность].[Вид задолженности].[Все].[Просроченная дебиторская задолженность]";

               string query = DataProvider.GetQueryText("FO_0002_0047_Grid");
               DataTable dtGridDebit = new DataTable();
               DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridDebit);

               if (dtGridDebit.Rows.Count > 0)
               {
                   if (!CheckBox3.Checked) // выводим МО
                   {
                       municipalUnion.Value = "+ { [МО] } ";
                   }
                   else
                   {
                       municipalUnion.Value = string.Empty;
                   }
                       query = DataProvider.GetQueryText("FO_0002_0047_GridMO");
                       dtGridMO = new DataTable();
                       DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridMO);
                       if (dtGridMO.Rows.Count > 0)
                       {
                           foreach (DataRow row in dtGridMO.Rows)
                           {
                               dtGridDebit.ImportRow(row);
                           }
                           dtGridDebit.AcceptChanges();
                       }

                       for (int i = 1; i < dtGridDebit.Columns.Count - 1; i += index)
                       {
                           if (dtGridDebit.Rows[1][i] != DBNull.Value && dtGridDebit.Rows[1][i].ToString() != string.Empty && dtGridMO.Rows[0][i] != DBNull.Value && dtGridMO.Rows[0][i].ToString() != string.Empty)
                           {
                               dtGridDebit.Rows[1][i] = Convert.ToDouble(dtGridDebit.Rows[1][i]) + Convert.ToDouble(dtGridMO.Rows[0][i]);
                           }
                           else if (dtGridDebit.Rows[1][i] == DBNull.Value && dtGridDebit.Rows[1][i].ToString() == string.Empty && dtGridMO.Rows[0][i] != DBNull.Value && dtGridMO.Rows[0][i].ToString() != string.Empty)
                           {
                               dtGridDebit.Rows[1][i] = Convert.ToDouble(dtGridMO.Rows[0][i]);
                           }
                           if (dtGridDebit.Rows[1][i + 1] != DBNull.Value && dtGridDebit.Rows[1][i + 1].ToString() != string.Empty && dtGridMO.Rows[0][i + 1] != DBNull.Value && dtGridMO.Rows[0][i + 1].ToString() != string.Empty)
                           {
                               dtGridDebit.Rows[1][i + 1] = Convert.ToDouble(dtGridDebit.Rows[1][i + 1]) + Convert.ToDouble(dtGridMO.Rows[0][i + 1]);
                           }
                           else if (dtGridDebit.Rows[1][i + 1] == DBNull.Value && dtGridDebit.Rows[1][i + 1].ToString() == string.Empty && dtGridMO.Rows[0][i + 1] != DBNull.Value && dtGridMO.Rows[0][i + 1].ToString() != string.Empty)
                           {
                               dtGridDebit.Rows[1][i + 1] = Convert.ToDouble(dtGridMO.Rows[0][i + 1]);
                           }
                           if (ComboQuarter.SelectedValue != "Квартал 4")
                           {
                               if (dtGridDebit.Rows[1][i + 2] != DBNull.Value && dtGridDebit.Rows[1][i + 2].ToString() != string.Empty &&
                                   dtGridMO.Rows[0][i + 2] != DBNull.Value && dtGridMO.Rows[0][i + 2].ToString() != string.Empty)
                               {
                                   dtGridDebit.Rows[1][i + 2] = Convert.ToDouble(dtGridDebit.Rows[1][i + 2]) + Convert.ToDouble(dtGridMO.Rows[0][i + 2]);
                               }
                               else if (dtGridDebit.Rows[1][i + 2] == DBNull.Value && dtGridDebit.Rows[1][i + 2].ToString() == string.Empty &&
                                        dtGridMO.Rows[0][i + 2] != DBNull.Value && dtGridMO.Rows[0][i + 2].ToString() != string.Empty)
                               {
                                   dtGridDebit.Rows[1][i + 2] = Convert.ToDouble(dtGridMO.Rows[0][i + 2]);
                               }

                               // темп роста

                               if (dtGridDebit.Rows[1][i + 2] != DBNull.Value && dtGridDebit.Rows[1][i + 2].ToString() != string.Empty
                                   && dtGridDebit.Rows[1][i + 1] != DBNull.Value && dtGridDebit.Rows[1][i + 1].ToString() != string.Empty)
                               {
                                   dtGridDebit.Rows[1][i + 3] = Convert.ToDouble(dtGridDebit.Rows[1][i + 2]) / Convert.ToDouble(dtGridDebit.Rows[1][i + 1]);
                               }

                               if (dtGridDebit.Rows[1][i + 2] != DBNull.Value && dtGridDebit.Rows[1][i + 2].ToString() != string.Empty
                                   && dtGridDebit.Rows[1][i] != DBNull.Value && dtGridDebit.Rows[1][i].ToString() != string.Empty)
                               {
                                   dtGridDebit.Rows[1][i + 4] = Convert.ToDouble(dtGridDebit.Rows[1][i + 2]) / Convert.ToDouble(dtGridDebit.Rows[1][i]);
                               }
                           }
                           else if (dtGridDebit.Rows[1][i + 1] != DBNull.Value && dtGridDebit.Rows[1][i + 1].ToString() != string.Empty 
                               && dtGridDebit.Rows[1][i] != DBNull.Value && dtGridDebit.Rows[1][i].ToString() != string.Empty)
                           {
                               dtGridDebit.Rows[1][i + 2] = Convert.ToDouble(dtGridDebit.Rows[1][i + 1]) / Convert.ToDouble(dtGridDebit.Rows[1][i]);
                           }

                       }

                   typeDebit.Value = " [Задолженность].[Вид задолженности].[Все].[Кредиторская задолженность] ";
                   adminType.Value = string.Format("{0}.[КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, ВСЕГО]",dimentions.Value);
                   typeKredit.Value = "[Задолженность].[Вид задолженности].[Все].[Просроченная кредиторская задолженность]";
                   firstSTR.Value = string.Empty;
                   query = DataProvider.GetQueryText("FO_0002_0047_Grid");
                   DataTable dtGridKredit = new DataTable();
                   DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridKredit);

                   if (!CheckBox3.Checked) // выводим МО
                   {
                       municipalUnion.Value = "+ { [МО] } ";
                   }
                   else
                   {
                       municipalUnion.Value = string.Empty;
                   } 
                   query = DataProvider.GetQueryText("FO_0002_0047_GridMO");
                   dtGridMO = new DataTable();
                   DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridMO);
                   
                   if (dtGridMO.Rows.Count > 0)
                       {
                           for (int i = 1; i < dtGridKredit.Columns.Count - 1; i += index)
                           {
                               if (dtGridKredit.Rows[0][i] != DBNull.Value && dtGridKredit.Rows[0][i].ToString() != string.Empty &&
                                   dtGridMO.Rows[0][i] != DBNull.Value && dtGridMO.Rows[0][i].ToString() != string.Empty)
                               {
                                   dtGridKredit.Rows[0][i] = Convert.ToDouble(dtGridKredit.Rows[0][i]) + Convert.ToDouble(dtGridMO.Rows[0][i]);
                               }
                               else if (dtGridKredit.Rows[0][i] == DBNull.Value && dtGridKredit.Rows[0][i].ToString() == string.Empty &&
                                  dtGridMO.Rows[0][i] != DBNull.Value && dtGridMO.Rows[0][i].ToString() != string.Empty)
                               {
                                   dtGridKredit.Rows[0][i] = Convert.ToDouble(dtGridMO.Rows[0][i]);
                               }

                               if (dtGridKredit.Rows[0][i + 1] != DBNull.Value && dtGridKredit.Rows[0][i + 1].ToString() != string.Empty &&
                                  dtGridMO.Rows[0][i + 1] != DBNull.Value && dtGridMO.Rows[0][i + 1].ToString() != string.Empty)
                               {
                                   dtGridKredit.Rows[0][i + 1] = Convert.ToDouble(dtGridKredit.Rows[0][i + 1]) +
                                                            Convert.ToDouble(dtGridMO.Rows[0][i + 1]);
                               }
                               else if (dtGridKredit.Rows[0][i + 1] == DBNull.Value && dtGridKredit.Rows[0][i + 1].ToString() == string.Empty &&
                                 dtGridMO.Rows[0][i + 1] != DBNull.Value && dtGridMO.Rows[0][i + 1].ToString() != string.Empty)
                               {
                                   dtGridKredit.Rows[0][i + 1] = Convert.ToDouble(dtGridMO.Rows[0][i + 1]);
                               }

                               if (ComboQuarter.SelectedValue != "Квартал 4")
                               {
                                   if (dtGridKredit.Rows[0][i + 2] != DBNull.Value && dtGridKredit.Rows[0][i + 2].ToString() != string.Empty &&
                                       dtGridMO.Rows[0][i + 2] != DBNull.Value && dtGridMO.Rows[0][i + 2].ToString() != string.Empty)
                                   {
                                       dtGridKredit.Rows[0][i + 2] = Convert.ToDouble(dtGridKredit.Rows[0][i + 2]) +
                                                                     Convert.ToDouble(dtGridMO.Rows[0][i + 2]);
                                   }
                                   else if (dtGridKredit.Rows[0][i + 2] == DBNull.Value && dtGridKredit.Rows[0][i + 2].ToString() == string.Empty &&
                                            dtGridMO.Rows[0][i + 2] != DBNull.Value && dtGridMO.Rows[0][i + 2].ToString() != string.Empty)
                                   {
                                       dtGridKredit.Rows[0][i + 2] = Convert.ToDouble(dtGridMO.Rows[0][i + 2]);
                                   }

                                   //темп роста 
                                   if (dtGridKredit.Rows[0][i + 2] != DBNull.Value && dtGridKredit.Rows[0][i + 2].ToString() != string.Empty
                                      && dtGridKredit.Rows[0][i+1] != DBNull.Value && dtGridKredit.Rows[0][i+1].ToString() != string.Empty)
                                   {
                                       dtGridKredit.Rows[0][i + 3] = Convert.ToDouble(dtGridKredit.Rows[0][i + 2]) / Convert.ToDouble(dtGridKredit.Rows[0][i+1]);
                                   }

                                   // темп роста
                                   if (dtGridKredit.Rows[0][i + 2] != DBNull.Value && dtGridKredit.Rows[0][i + 2].ToString() != string.Empty
                                      && dtGridKredit.Rows[0][i] != DBNull.Value && dtGridKredit.Rows[0][i].ToString() != string.Empty)
                                   {
                                       dtGridKredit.Rows[0][i + 4] = Convert.ToDouble(dtGridKredit.Rows[0][i + 2]) / Convert.ToDouble(dtGridKredit.Rows[0][i]);
                                   }
                               }
                               else if (dtGridKredit.Rows[0][i + 1] != DBNull.Value && dtGridKredit.Rows[0][i + 1].ToString() != string.Empty
                                 && dtGridKredit.Rows[0][i] != DBNull.Value && dtGridKredit.Rows[0][i].ToString() != string.Empty)
                               {
                                 
                                   dtGridKredit.Rows[0][i + 2] = Convert.ToDouble(dtGridKredit.Rows[0][i + 1]) / Convert.ToDouble(dtGridKredit.Rows[0][i]);
                                   CRHelper.SaveToErrorLog((i+2).ToString());
                                   CRHelper.SaveToErrorLog(dtGridKredit.Rows[1][i + 2].ToString());
                               }
                           }
                       }

                      if (dtGridKredit.Rows.Count > 0)
                       {
                           foreach (DataRow row in dtGridKredit.Rows)
                           {
                               dtGridDebit.ImportRow(row);
                           }
                           dtGridDebit.AcceptChanges();
                       }
                       

                       if (dtGridMO.Rows.Count > 0)
                       {
                           foreach (DataRow row in dtGridMO.Rows)
                           {
                               dtGridDebit.ImportRow(row);
                           }
                           dtGridDebit.AcceptChanges();
                       }

                   for (int i = 0; i < dtGridDebit.Rows.Count; i++)
                   {
                       string captions = dtGridDebit.Rows[i][0].ToString().Replace("Дебиторская задолженность;", " ").Replace("Кредиторская задолженность;", " ");
                       dtGridDebit.Rows[i][0] = captions; 
                   }
                   
                   UltraWebGrid1.DataSource = dtGridDebit;
               }
           }
           else // выбран один вид задолженности
           {
               if (CheckBox1.Checked) // дебиторская задолженность
               {
                   typeDebit.Value = "[Задолженность].[Вид задолженности].[Все].[Дебиторская задолженность]";
                   adminType.Value = string.Format("{0}.[ ДЕБИТОРСКАЯ ЗАДОЛЖЕННОСТЬ, ВСЕГО]",dimentions.Value);
                   firstSTR.Value = string.Format("{0}.[Строка с произвольными данными для корректной работы формата],", dimentions.Value);
                   typeKredit.Value = "[Задолженность].[Вид задолженности].[Все].[Просроченная дебиторская задолженность]";
                   string query = DataProvider.GetQueryText("FO_0002_0047_Grid");
                   DataTable dtGridDebit = new DataTable();
                   DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridDebit);

                   if (dtGridDebit.Rows.Count > 0)
                   {
                       if (!CheckBox3.Checked) // выводим МО
                       {
                           municipalUnion.Value = "+ { [МО] } ";
                       }
                       else
                       {
                           municipalUnion.Value = string.Empty;
                       } 
                       query = DataProvider.GetQueryText("FO_0002_0047_GridMO");
                       dtGridMO = new DataTable();
                       DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridMO);
                         if (dtGridMO.Rows.Count > 0)
                           {
                               foreach (DataRow row in dtGridMO.Rows)
                               {
                                   dtGridDebit.ImportRow(row);
                               }
                               dtGridDebit.AcceptChanges();

                           }
                         for (int i = 1; i < dtGridDebit.Columns.Count - 1; i += index)
                               {
                                   if (dtGridDebit.Rows[1][i] != DBNull.Value && dtGridDebit.Rows[1][i].ToString() != string.Empty && dtGridMO.Rows[0][i] != DBNull.Value && dtGridMO.Rows[0][i].ToString() != string.Empty)
                                   {
                                       dtGridDebit.Rows[1][i] = Convert.ToDouble(dtGridDebit.Rows[1][i]) + Convert.ToDouble(dtGridMO.Rows[0][i]);
                                   }
                                   else if (dtGridDebit.Rows[1][i] == DBNull.Value && dtGridDebit.Rows[1][i].ToString() == string.Empty && dtGridMO.Rows[0][i] != DBNull.Value && dtGridMO.Rows[0][i].ToString() != string.Empty)
                                   {
                                       dtGridDebit.Rows[1][i] = Convert.ToDouble(dtGridMO.Rows[0][i]);
                                   }
                                   if (dtGridDebit.Rows[1][i + 1] != DBNull.Value && dtGridDebit.Rows[1][i + 1].ToString() != string.Empty && dtGridMO.Rows[0][i + 1] != DBNull.Value && dtGridMO.Rows[0][i + 1].ToString() != string.Empty)
                                   {
                                       dtGridDebit.Rows[1][i + 1] = Convert.ToDouble(dtGridDebit.Rows[1][i + 1]) + Convert.ToDouble(dtGridMO.Rows[0][i + 1]);
                                   }
                                   else if (dtGridDebit.Rows[1][i + 1] == DBNull.Value && dtGridDebit.Rows[1][i + 1].ToString() == string.Empty && dtGridMO.Rows[0][i + 1] != DBNull.Value && dtGridMO.Rows[0][i + 1].ToString() != string.Empty)
                                   {
                                       dtGridDebit.Rows[1][i + 1] = Convert.ToDouble(dtGridMO.Rows[0][i + 1]);
                                   }
                                   if (ComboQuarter.SelectedValue != "Квартал 4")
                                   {
                                       if (dtGridDebit.Rows[1][i + 2] != DBNull.Value && dtGridDebit.Rows[1][i + 2].ToString() != string.Empty &&
                                           dtGridMO.Rows[0][i + 2] != DBNull.Value && dtGridMO.Rows[0][i + 2].ToString() != string.Empty)
                                       {
                                           dtGridDebit.Rows[1][i + 2] = Convert.ToDouble(dtGridDebit.Rows[1][i + 2]) + Convert.ToDouble(dtGridMO.Rows[0][i + 2]);
                                       }
                                       else if (dtGridDebit.Rows[1][i + 2] == DBNull.Value && dtGridDebit.Rows[1][i + 2].ToString() == string.Empty &&
                                                dtGridMO.Rows[0][i + 2] != DBNull.Value && dtGridMO.Rows[0][i + 2].ToString() != string.Empty)
                                       {
                                           dtGridDebit.Rows[1][i + 2] = Convert.ToDouble(dtGridMO.Rows[0][i + 2]);
                                       }
                                       // темп роста к предыдущему году
                                       if (dtGridDebit.Rows[1][i + 2] != DBNull.Value && dtGridDebit.Rows[1][i + 2].ToString() != string.Empty
                                           && dtGridDebit.Rows[1][i+1] != DBNull.Value && dtGridDebit.Rows[1][i+1].ToString() != string.Empty)
                                       {
                                           dtGridDebit.Rows[1][i + 3] = Convert.ToDouble(dtGridDebit.Rows[1][i + 2])/ Convert.ToDouble(dtGridDebit.Rows[1][i+1]);
                                       }
                                       // темп роста к началу года
                                       if (dtGridDebit.Rows[1][i] != DBNull.Value && dtGridDebit.Rows[1][i].ToString() != string.Empty
                                       && dtGridDebit.Rows[1][i + 2] != DBNull.Value && dtGridDebit.Rows[1][i + 2].ToString() != string.Empty)
                                       {
                                           dtGridDebit.Rows[1][i + 4] = Convert.ToDouble(dtGridDebit.Rows[1][i + 2]) / Convert.ToDouble(dtGridDebit.Rows[1][i]);
                                       }

                                   }else if (dtGridDebit.Rows[1][i] != DBNull.Value && dtGridDebit.Rows[1][i].ToString() != string.Empty
                                       && dtGridDebit.Rows[1][i + 1] != DBNull.Value && dtGridDebit.Rows[1][i + 1].ToString() != string.Empty)
                                   {
                                       dtGridDebit.Rows[1][i + 2] = Convert.ToDouble(dtGridDebit.Rows[1][i + 1]) / Convert.ToDouble(dtGridDebit.Rows[1][i]);
                                   }// темп роста
                               }
                   }

                   for (int i = 0; i < dtGridDebit.Rows.Count; i++)
                   {
                       string captions = dtGridDebit.Rows[i][0].ToString().Replace("Дебиторская задолженность;", " ").Replace("Кредиторская задолженность;", " ");
                       dtGridDebit.Rows[i][0] = captions;
                   }

                   UltraWebGrid1.DataSource = dtGridDebit;
               }
               else  // кредиторская задолженность
               {
                   typeDebit.Value = " [Задолженность].[Вид задолженности].[Все].[Кредиторская задолженность] ";
                   adminType.Value = string.Format("{0}.[КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, ВСЕГО]", dimentions.Value);
                   firstSTR.Value = string.Format("{0}.[Строка с произвольными данными для корректной работы формата],",dimentions.Value);
                   typeKredit.Value = "[Задолженность].[Вид задолженности].[Все].[Просроченная кредиторская задолженность]";

                   string query = DataProvider.GetQueryText("FO_0002_0047_Grid");
                   DataTable dtGridKredit = new DataTable();
                   DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridKredit);

                   if (!CheckBox3.Checked) // выводим МО
                   {
                       municipalUnion.Value = "+ { [МО] } ";
                   }
                   else
                   {
                       municipalUnion.Value = string.Empty;
                   } 

                   query = DataProvider.GetQueryText("FO_0002_0047_GridMO");
                   dtGridMO = new DataTable();
                   DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGridMO);

                     if (dtGridMO.Rows.Count > 0)
                       {
                           foreach (DataRow row in dtGridMO.Rows)
                           {
                               dtGridKredit.ImportRow(row);
                           }
                           dtGridKredit.AcceptChanges();
                       }
                   
                   for (int i = 1; i < dtGridKredit.Columns.Count - 1; i += index)
                       {
                           if (dtGridKredit.Rows[1][i] != DBNull.Value && dtGridKredit.Rows[1][i].ToString() != string.Empty &&
                               dtGridMO.Rows[0][i] != DBNull.Value && dtGridMO.Rows[0][i].ToString() != string.Empty)
                           {
                               dtGridKredit.Rows[1][i] = Convert.ToDouble(dtGridKredit.Rows[1][i]) +
                                                        Convert.ToDouble(dtGridMO.Rows[0][i]);
                           }
                           else if (dtGridKredit.Rows[1][i] == DBNull.Value && dtGridKredit.Rows[1][i].ToString() == string.Empty &&
                              dtGridMO.Rows[0][i] != DBNull.Value && dtGridMO.Rows[0][i].ToString() != string.Empty)
                           {
                               dtGridKredit.Rows[1][i] = Convert.ToDouble(dtGridMO.Rows[0][i]);
                           }

                           if (dtGridKredit.Rows[1][i + 1] != DBNull.Value && dtGridKredit.Rows[1][i + 1].ToString() != string.Empty &&
                              dtGridMO.Rows[0][i + 1] != DBNull.Value && dtGridMO.Rows[0][i + 1].ToString() != string.Empty)
                           {
                               dtGridKredit.Rows[1][i + 1] = Convert.ToDouble(dtGridKredit.Rows[1][i + 1]) + Convert.ToDouble(dtGridMO.Rows[0][i + 1]);
                           }
                           else if (dtGridKredit.Rows[1][i + 1] == DBNull.Value && dtGridKredit.Rows[1][i + 1].ToString() == string.Empty &&
                             dtGridMO.Rows[0][i + 1] != DBNull.Value && dtGridMO.Rows[0][i + 1].ToString() != string.Empty)
                           {
                               dtGridKredit.Rows[1][i + 1] = Convert.ToDouble(dtGridMO.Rows[0][i + 1]);
                           }

                           if (ComboQuarter.SelectedValue != "Квартал 4")
                           {
                               if (dtGridKredit.Rows[1][i + 2] != DBNull.Value && dtGridKredit.Rows[1][i + 2].ToString() != string.Empty &&
                                   dtGridMO.Rows[0][i + 2] != DBNull.Value && dtGridMO.Rows[0][i + 2].ToString() != string.Empty)
                               {
                                   dtGridKredit.Rows[1][i + 2] = Convert.ToDouble(dtGridKredit.Rows[1][i + 2]) + Convert.ToDouble(dtGridMO.Rows[0][i + 2]);
                               }
                               else if (dtGridKredit.Rows[1][i + 2] == DBNull.Value && dtGridKredit.Rows[1][i + 2].ToString() == string.Empty &&
                                        dtGridMO.Rows[0][i + 2] != DBNull.Value && dtGridMO.Rows[0][i + 2].ToString() != string.Empty)
                               {
                                   dtGridKredit.Rows[1][i + 2] = Convert.ToDouble(dtGridMO.Rows[0][i + 2]);
                               }

                               // темп роста к аналогичному периоду прошлого года
                               if (dtGridKredit.Rows[1][i+2] != DBNull.Value && dtGridKredit.Rows[1][i+2].ToString() != string.Empty
                                 && dtGridKredit.Rows[1][i + 1] != DBNull.Value && dtGridKredit.Rows[1][i + 1].ToString() != string.Empty)
                               {
                                   dtGridKredit.Rows[1][i + 3] = Convert.ToDouble(dtGridKredit.Rows[1][i + 2]) / Convert.ToDouble(dtGridKredit.Rows[1][i+1]);
                               }
                               // темп роста на начало года
                               if (dtGridKredit.Rows[1][i + 2] != DBNull.Value && dtGridKredit.Rows[1][i + 2].ToString() != string.Empty
                                   && dtGridKredit.Rows[1][i] != DBNull.Value && dtGridKredit.Rows[1][i].ToString() != string.Empty)
                               {
                                   dtGridKredit.Rows[1][i + 4] = Convert.ToDouble(dtGridKredit.Rows[1][i + 2])/ Convert.ToDouble(dtGridKredit.Rows[1][i]);
                               }
                           }
                           else if (dtGridKredit.Rows[1][i] != DBNull.Value && dtGridKredit.Rows[1][i].ToString() != string.Empty
                                  && dtGridKredit.Rows[1][i + 1] != DBNull.Value && dtGridKredit.Rows[1][i + 1].ToString() != string.Empty)
                           {
                               dtGridKredit.Rows[1][i + 2] = Convert.ToDouble(dtGridKredit.Rows[1][i + 1]) / Convert.ToDouble(dtGridKredit.Rows[1][i]);
                           }
                         
                       }
                   

                   for (int i = 0; i < dtGridKredit.Rows.Count; i++)
                   {
                       string captions = dtGridKredit.Rows[i][0].ToString().Replace("Дебиторская задолженность;", " ").Replace("Кредиторская задолженность;", " ");
                       dtGridKredit.Rows[i][0] = captions;
                   }

                   UltraWebGrid1.DataSource = dtGridKredit;

               }
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

            e.Layout.Bands[0].Columns[UltraWebGrid1.Columns.Count - 1].Hidden = true;

            for (int i = 1; i < UltraWebGrid1.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(78);
            }
            if (ComboQuarter.SelectedValue == "Квартал 4")
            {
              for (int i = 3; i < UltraWebGrid1.Columns.Count; i+=index)
              {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
              }
            }
            else
            {
                for (int i = 4; i < UltraWebGrid1.Columns.Count; i += 5)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");

                    e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(90);
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                }
            }

            headerLayout.AddCell("Наименование");

            if (ComboQuarter.SelectedValue != "Квартал 4")
            {
                GridHeaderCell cell1 = headerLayout.AddCell("Всего",
                                                            string.Format(
                                                                "Всего задолженности по состоянию на {0:dd.MM.yyyy} г.",
                                                                currentMember));

                cell1.AddCell(string.Format("На 01.01.{0}", year));
                cell1.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell1.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell1.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
                cell1.AddCell("Темп роста к показателям на начало года",
                              "Темп роста к показателям на начало отчетного года");

                GridHeaderCell cell2 = headerLayout.AddCell("Бюджетные средства",
                                                            string.Format(
                                                                "Задолженность по бюджетным средствам по состоянию на {0:dd.MM.yyyy} г.",
                                                                currentMember));
                cell2.AddCell(string.Format("На 01.01.{0}", year));
                cell2.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell2.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell2.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
                cell2.AddCell("Темп роста к показателям на начало года",
                              "Темп роста к показателям на начало отчетного года");

                GridHeaderCell cell3 = headerLayout.AddCell("в том числе просроченная задолженность",
                                                            string.Format(
                                                                "в том числе просроченная задолженность {0:dd.MM.yyyy} г.",
                                                                currentMember));
                cell3.AddCell(string.Format("На 01.01.{0}", year));
                cell3.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell3.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell3.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
                cell3.AddCell("Темп роста к показателям на начало года",
                              "Темп роста к показателям на начало отчетного года");

                GridHeaderCell cell4 = headerLayout.AddCell("Внебюджетные средства",
                                                            string.Format(
                                                                "Задолженность по внебюджетным средствам по состоянию на {0:dd.MM.yyyy} г.",
                                                                currentMember));
                cell4.AddCell(string.Format("На 01.01.{0}", year));
                cell4.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell4.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell4.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
                cell4.AddCell("Темп роста к показателям на начало года",
                              "Темп роста к показателям на начало отчетного года");

                GridHeaderCell cell5 = headerLayout.AddCell("в том числе просроченная задолженность",
                                                            string.Format(
                                                                "Просроченная задолженность по внебюджетным средствам по состоянию на  {0:dd.MM.yyyy} г.",
                                                                currentMember));
                cell5.AddCell(string.Format("На 01.01.{0}", year));
                cell5.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell5.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell5.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
                cell5.AddCell("Темп роста к показателям на начало года",
                              "Темп роста к показателям на начало отчетного года");
            }
            else
            {
                GridHeaderCell cell1 = headerLayout.AddCell("Всего",
                                                            string.Format(
                                                                "Всего задолженности по состоянию на {0:dd.MM.yyyy} г.",
                                                                currentMember));

               
                cell1.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell1.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell1.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
            
                GridHeaderCell cell2 = headerLayout.AddCell("Бюджетные средства",
                                                            string.Format(
                                                                "Задолженность по бюджетным средствам по состоянию на {0:dd.MM.yyyy} г.",
                                                                currentMember));
               
                cell2.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell2.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell2.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
              
                GridHeaderCell cell3 = headerLayout.AddCell("в том числе просроченная задолженность",
                                                            string.Format(
                                                                "в том числе просроченная задолженность {0:dd.MM.yyyy} г.",
                                                                currentMember));
           
                cell3.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell3.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell3.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
           
                GridHeaderCell cell4 = headerLayout.AddCell("Внебюджетные средства",
                                                            string.Format(
                                                                "Задолженность по внебюджетным средствам по состоянию на {0:dd.MM.yyyy} г.",
                                                                currentMember));
               
                cell4.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell4.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell4.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
              

                GridHeaderCell cell5 = headerLayout.AddCell("в том числе просроченная задолженность",
                                                            string.Format(
                                                                "Просроченная задолженность по внебюджетным средствам по состоянию на  {0:dd.MM.yyyy} г.",
                                                                currentMember));
            
                cell5.AddCell(string.Format("На {0:dd.MM.yyyy}", prevCurDate));
                cell5.AddCell(string.Format("На {0:dd.MM.yyyy}", currentMember));
                cell5.AddCell("Темп роста к аналогичному периоду предыдущего года",
                              "Темп роста к аналогичному периоду предыдущего года");
               
            }

            headerLayout.ApplyHeaderInfo();

        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        { 

            if (e.Row.Index == 0)
            {
                e.Row.Hidden = true;
            }

            string level = string.Empty;
            if (e.Row.Cells[e.Row.Cells.Count - 1] != null)
            {
                level = e.Row.Cells[e.Row.Cells.Count - 1].ToString();
            }
           
           
          for (int i = 0; i < e.Row.Cells.Count; i++)
           {
                    switch (level)
                    {
                        case "1":
                            {
                                e.Row.Cells[i].Style.Font.Bold = true;
                                break;
                            }
                        case "2":
                            {
                                e.Row.Cells[i].Style.Font.Bold = true;
                                break;
                            }


                    }  
              
           }

          for (int i = 1; i < e.Row.Cells.Count; i++)
          {
              if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
              {
                  double value = Convert.ToDouble(e.Row.Cells[i].Value);
               //   e.Row.Cells[i].Value = string.Format("{0:N2}", value);
              }
          }
        int start = ComboQuarter.SelectedValue == "Квартал 4" ? 3 : 4;
        for (int i=start; i<e.Row.Cells.Count; i+=index )
        {
            if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            {

                double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                if (currentValue * 100 > 100)
                {
                    e.Row.Cells[i].Style.BackgroundImage =  "~/images/arrowRedUpBB.png";
                    e.Row.Cells[i].Title = "Рост задолженности";
                }
                else if (currentValue * 100 < 100)
                {
                    e.Row.Cells[i].Style.BackgroundImage =  "~/images/arrowGreenDownBB.png" ;
                    e.Row.Cells[i].Title = "Снижение задолженности";
                }

            }
            e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
           
            if (ComboQuarter.SelectedValue != "Квартал 4")
            {
                if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                {

                    double currentValue = Convert.ToDouble(e.Row.Cells[i + 1].Value);

                    if (currentValue*100 > 100)
                    {
                        e.Row.Cells[i + 1].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        e.Row.Cells[i + 1].Title = "Рост задолженности";
                    }
                    else if (currentValue*100 < 100)
                    {
                        e.Row.Cells[i + 1].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        e.Row.Cells[i + 1].Title = "Снижение задолженности";
                    }

                }
                e.Row.Cells[i + 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }
        
           
        
        }

        }
   #endregion 

    #region Обработчики диаграмм
        protected void UltraChart1_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0047_Chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart1);
            
            UltraChart1.Data.SwapRowsAndColumns = false;

            dtChart1.Columns[1].ColumnName = string.Format("На 01.01.{0}", year - 1);
            dtChart1.Columns[2].ColumnName = string.Format("На {0:dd.MM.yyyy}", currentMember);

            for (int i = 0; i < dtChart1.Rows.Count; i++ )
            {
                string captions = dtChart1.Rows[i][0].ToString().Replace("муниципальный район", "МР").Replace("муниципальное образование", "МО");
                dtChart1.Rows[i][0] = captions;
            }

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n <DATA_VALUE:N2> тыс. руб. ";
            UltraChart1.DataSource = dtChart1;
        }

        protected void UltraChart2_DataBinding(Object sender, EventArgs e)
        {
           string query = DataProvider.GetQueryText("FO_0002_0047_Chart2");
           dtChart2 = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма2", dtChart2);

           UltraChart2.Data.SwapRowsAndColumns = false;

           dtChart2.Columns[1].ColumnName = string.Format("На 01.01.{0}", year - 1);
           dtChart2.Columns[2].ColumnName = string.Format("На {0:dd.MM.yyyy}", currentMember);

           for (int i=0; i < dtChart2.Rows.Count; i++ )
           {
               string captions = dtChart2.Rows[i][0].ToString();
               switch (captions)
               {
                   case "Законодательное Собрание области":
                       {
                           dtChart2.Rows[i][0] = "ЗСО";
                           break;
                       }
                   case "Контрольно-счетная палата области":
                       {
                           dtChart2.Rows[i][0] = "Контрольно-счетная палата";
                           break;
                       }
                   case "Правительство области ":
                       {
                           dtChart2.Rows[i][0] = "Правительство области";
                           break;
                       }
                   case "Представительство области при Президенте Российской Федерации и Правительстве Российской Федерации":
                       {
                           dtChart2.Rows[i][0] = "Представительство области при Президенте РФ";
                           break;
                       }
                   case "Представительство области в Северо-Западном федеральном округе":
                       {
                           dtChart2.Rows[i][0] = "Представительство области в СЗФО";
                           break;
                       }
                   case "Департамент образования области":
                       {
                           dtChart2.Rows[i][0] = "Департамент образования";
                           break;
                       }
                   case "Департамент культуры и охраны объектов культурного наследия области":
                       {
                           dtChart2.Rows[i][0] = "Департамент культуры и охраны объектов культ. наследия";
                           break;
                       }
                   case "Департамент здравоохранения области":
                       {
                           dtChart2.Rows[i][0] = "Департамент здравоохранения";
                           break;
                       }
                   case "Департамент труда и социального развития области":
                       {
                           dtChart2.Rows[i][0] = "Департамент труда и соц. развития";
                           break;
                       }
                   case "Департамент сельского хозяйства, продовольственных ресурсов и торговли области":
                       {
                           dtChart2.Rows[i][0] = "Департамент с./х.";
                           break;
                       }
                   case "Департамент финансов области ":
                       {
                           dtChart2.Rows[i][0] = "Департамент финансов";
                           break;
                       }
                   case "Департамент дорожного хозяйства и транспорта области":
                       {
                           dtChart2.Rows[i][0] = "Департамент дор. хоз.";
                           break;
                       }
                   case "Департамент по обеспечению деятельности мировых судей области":
                       {
                           dtChart2.Rows[i][0] = "Департамент мировых судей";
                           break;
                       }
                   case "Департамент природных ресурсов и охраны окружающей среды области":
                       {
                           dtChart2.Rows[i][0] = "Департамент природных ресурсов";
                           break;
                       }
                   case "Департамент имущественных отношений области":
                       {
                           dtChart2.Rows[i][0] = "Департамент имущ. отношений";
                           break;
                       }
                   case "Департамент лесного комплекса области":
                       {
                           dtChart2.Rows[i][0] = "Департамент лесного комплекса";
                           break;
                       }
                   case "Комитет по физической культуре, спорту и молодежной политике области":
                       {
                           dtChart2.Rows[i][0] = "Комитет по физической культуре";
                           break;
                       }
                   case "Департамент международных, межрегиональных связей и туризма области":
                       {
                           dtChart2.Rows[i][0] = "Деп - т международ., межрегион. связей и туризма";
                           break;
                       }
                   case "Комитет информационной политики области":
                       {
                           dtChart2.Rows[i][0] = "Комитет информационной политики";
                           break;
                       }
                   case "Департамент топливно-энергетического комплекса области":
                       {
                           dtChart2.Rows[i][0] = "Департамент ТЭК";
                           break;
                       }
                   case "Комитет социальной безопасности области":
                       {
                           dtChart2.Rows[i][0] = "Комитет соц. без";
                           break;
                       }
                   case "Департамент государственной службы и кадровой политики области":
                       {
                           dtChart2.Rows[i][0] = "Департамент гос. службы";
                           break;
                       }
                   case "Управление по делам архивов области":
                       {
                           dtChart2.Rows[i][0] = "Управ-е по делам архивов";
                           break;
                       }
                   case "Управление ЗАГС области":
                       {
                           dtChart2.Rows[i][0] = "Управ-е ЗАГС";
                           break;
                       }
                   case "Главное управление архитектуры и градостроительства области":
                       {
                           dtChart2.Rows[i][0] = "Глав. управ-е архитектуры и град-а";
                           break;
                       }
                   case "Управление Государственной инспекции по надзору за техническим состоянием самоходных машин и других видов техники области":
                       {
                           dtChart2.Rows[i][0] = "Управ - е Гос. инспекции по надзору за тех. состоянием машин";
                           break;
                       }
                   case "Управление гражданской защиты и пожарной безопасности области":
                       {
                           dtChart2.Rows[i][0] = "Управление гражд. защиты";
                           break;
                       }
                   case "Инспекция государственного строительного надзора области":
                       {
                           dtChart2.Rows[i][0] = "Инспекция гос. строит. надзора";
                           break;
                       }
                   case "Государственная жилищная инспекция области":
                       {
                           dtChart2.Rows[i][0] = "Гос. жилищ. инспекция";
                           break;
                       }
                   case "Региональная энергетическая комиссия области":
                       {
                           dtChart2.Rows[i][0] = "РЭК";
                           break;
                       }
                   case "Избирательная комиссия области":
                       {
                           dtChart2.Rows[i][0] = "ИзбирКом";
                           break;
                       }
                   case "Комитет развития малого и среднего предпринимательства области":
                       {
                           dtChart2.Rows[i][0] = "Комитет развития малого и среднего предприним-а";
                           break;
                       }
                   case "Департамент по охране, контролю и регулированию использования объектов животного мира Вологодской области":
                       {
                           dtChart2.Rows[i][0] = "Департ-т по охране, контролю и рег-ю исп. объектов жив. мира";
                           break;
                       }
                   case "Департамент земельных отношений области":
                       {
                           dtChart2.Rows[i][0] = "Департ - т земельных отношений";
                           break;
                       }
                   case "Департамент занятости населения области":
                       {
                           dtChart2.Rows[i][0] = "Департ - т занятости ";
                           break;
                       }
                   case "Комитет информационных технологий и телекоммуникаций области":
                       {
                           dtChart2.Rows[i][0] = "Комитет инф. тех-й и телеком-й";
                           break;
                       }
                   case "ГУ \"Главное управление МЧС России по Вологодской области\"":
                       {
                           dtChart2.Rows[i][0] = "ГУ «Глав. управление МЧС»";
                           break;
                       }
                   case "Управление внутренних дел по Вологодской области":
                       {
                           dtChart2.Rows[i][0] = "Управ-е внутренних дел";
                           break;
                       }
                   case "Департамент развития муниципальных образований области":
                       {
                           dtChart2.Rows[i][0] = "Депар-т развития МО";
                           break;
                       }
                   case "Комитет государственного заказа области":
                       {
                           dtChart2.Rows[i][0] = "Комитет гос. заказа";
                           break;
                       }

               }
               
           }
            dtChart2.AcceptChanges();
            UltraChart2.DataSource = dtChart2;
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

            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.GridColumnWidthScale = 1.5;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1,chart1ElementCaption.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart2,chart2ElementCaption.Text, sheet3, 3);
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

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportPDFExporter1.Export(headerLayout, section1);
            UltraChart1.Width = 800;
            UltraChart2.Width = 800;
            ReportPDFExporter1.Export(UltraChart1,chart1ElementCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart2,chart2ElementCaption.Text, section3);
        }

   #endregion
  #endregion

    }
}
