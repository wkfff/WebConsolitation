using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
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
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0001_0004_FF
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtFF;
        private DataTable dtChartFF1;
        private DataTable dtChartFF2;
        
        private GridHeaderLayout headerLayout;
        private CustomParam Period;
        private CustomParam SelectedFO;

        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGridFF.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 25);
            UltraChartFF1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.550 - 15);
            UltraChartFF2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.450 - 15);

            UltraWebGridFF.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 130);
            UltraChartFF1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 120);
            UltraChartFF2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 120);

            UltraWebGridFF.DataBound += new EventHandler(UltraWebGrid_DataBound);
            #region Инициализация параметров
            if (Period == null)
            {
                Period = UserParams.CustomParam("period");
            }
            if (SelectedFO == null)
            {
                SelectedFO = UserParams.CustomParam("select_FO");
            }
            #endregion
            #region Настройка диаграмм

            UltraChartFF1.ChartType = ChartType.StackColumnChart;
            UltraChartFF1.Axis.Y.Extent = 20;
            UltraChartFF1.Axis.X.Extent = 60;
            UltraChartFF1.Axis.Y2.Extent = 10;
            UltraChartFF1.Axis.X2.Extent = 10;
            UltraChartFF1.Legend.Visible = false;
            UltraChartFF1.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChartFF1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChartFF1.TitleLeft.Visible = true;
            UltraChartFF1.TitleLeft.Text = "                   Млн. руб.";
            UltraChartFF1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChartFF1.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N3> млн.руб.";
            UltraChartFF1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChartFF2.ChartType = ChartType.StackColumnChart;
            UltraChartFF2.Legend.SpanPercentage = 16;
            UltraChartFF2.Legend.Location = LegendLocation.Right;
            UltraChartFF2.Legend.Margins.Bottom = (int)(UltraChartFF2.Height.Value / 2);
            UltraChartFF2.Legend.Visible = true;
            UltraChartFF2.Axis.Y.Extent = 40;
            UltraChartFF2.Axis.X.Extent = 60;
            UltraChartFF2.Axis.Y2.Extent = 10;
            UltraChartFF2.Axis.X2.Extent = 10;
            UltraChartFF2.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChartFF2.TitleLeft.Visible = true;
            UltraChartFF2.TitleLeft.Text = "                   Млн. руб.";
            UltraChartFF2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChartFF2.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";
            UltraChartFF2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChartFF2.Data.SwapRowsAndColumns = true;

            #endregion
            
            GridSearch1.LinkedGridId = UltraWebGridFF.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportPDFExporter1.PdfExporter.EndExport +=new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                lbFO.Text = string.Empty;
                lbSybject.Text = string.Empty;
                lbFOSub.Text = string.Empty;
                lbSubjectSub.Text = string.Empty;

                chartsWebAsyncPanel.AddRefreshTarget(lbFO);
                chartsWebAsyncPanel.AddRefreshTarget(lbSybject);
                chartsWebAsyncPanel.AddRefreshTarget(lbFOSub);
                chartsWebAsyncPanel.AddRefreshTarget(lbSubjectSub);
                chartsWebAsyncPanel.AddRefreshTarget(UltraChartFF1);
                chartsWebAsyncPanel.AddRefreshTarget(UltraChartFF2);
                chartsWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGridFF);
                
                Collection<string> years = new Collection<string>();
                years.Add("2007");
                years.Add("2008");
                years.Add("2009");
                years.Add("2010");
                years.Add("2011 (план, декабрь 2010)");
                years.Add("2012 (план, декабрь 2010)");
                years.Add("2013 (план, декабрь 2010)");

                ComboYear.Title = "Год";
                ComboYear.Width = 270;
                ComboYear.MultiSelect = false;
                ComboYear.FillValues(years);
                ComboYear.SetСheckedState("2011 (план, декабрь 2010)", true);

                UserParams.Filter.Value = "Все федеральные округа";
                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState(UserParams.Filter.Value, true);

                if (!string.IsNullOrEmpty(UserParams.Region.Value))
                {
                    ComboFO.SetСheckedState(UserParams.Region.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboFO.SetСheckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                }

                lbSybject.Text = string.Empty;
            }

            Page.Title = string.Format("Федеральные фонды ({0})", ComboFO.SelectedIndex == 0 ? "РФ" :
                RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;

            string pValue = string.Empty;

            switch (ComboYear.SelectedIndex)
            {
                case 0: // 2007
                    {
                        pValue = ComboYear.SelectedValue;
                        UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2007 год\"]";
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2007 год\"]";
                        break;
                    }
                case 1:  // 2008
                    {
                        pValue = ComboYear.SelectedValue;
                        UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2008 год и на плановый период 2009 и 2010 годов\"]";
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2007 год\"]";
                        break;
                    }
                case 2: //2009
                    {
                        pValue = ComboYear.SelectedValue;
                        UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2009 год и на плановый период 2010 и 2011 годов\"].[ФЗ с изменениями от 2 декабря 2009 г. № 309-ФЗ]";
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2008 год и на плановый период 2009 и 2010 годов\"]";
                        break;
                    }
                case 3:  //2010
                    {
                        pValue = ComboYear.SelectedValue;
                        UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2010 год и на плановый период 2011 и 2012 годов\"].[ФЗ от 3 ноября 2010 г. № 278-ФЗ]";
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2009 год и на плановый период 2010 и 2011 годов\"].[ФЗ с изменениями от 2 декабря 2009 г. № 309-ФЗ]";
                        break;
                    }
                case 4:  //2011
                    {
                        string[] st = ComboYear.SelectedValue.Split(' ');
                        if (st.Length > 1)
                        {
                            pValue = st[0];
                            UserParams.PeriodLastYear.Value = (Convert.ToInt32(st[0]) - 1).ToString();
                        }
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2010 год и на плановый период 2011 и 2012 годов\"].[ФЗ от 3 ноября 2010 г. № 278-ФЗ]";
                        break;
                    }
              case 5:   //2012
                    {
                        string[] st = ComboYear.SelectedValue.Split(' ');
                        if (st.Length > 1)
                        {
                            pValue = st[0];
                            UserParams.PeriodLastYear.Value = (Convert.ToInt32(st[0]) - 1).ToString();
                        }
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                        break;
                    }
             case 6:   //2013
                   {
                        string[] st = ComboYear.SelectedValue.Split(' ');
                       if (st.Length > 1)
                       {
                         pValue = st[0];
                         UserParams.PeriodLastYear.Value = (Convert.ToInt32(st[0]) - 1).ToString();
                       }
                       UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                       UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                       break;
                 }
            }

            if (!Page.IsPostBack || !(UserParams.PeriodYear.ValueIs(pValue)) || !UserParams.Filter.ValueIs(ComboFO.SelectedValue))
            {
                string currentYearStr = ComboYear.SelectedValue.Insert(4, " год");
                
                SelectedFO.Value = ComboFO.SelectedValue;
                if (RFSelected)
                {
                    SelectedFO.Value = " ";
                }
                else
                {
                    SelectedFO.Value = string.Format(".[{0}]", ComboFO.SelectedValue);
                }
                UserParams.PeriodYear.Value = pValue;
                switch (ComboYear.SelectedIndex)
                {
                    case 0://2007
                        {
                            Period.Value = "[Период].[Период].[2007_],[Период].[Период].[2008_],[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010]";
                           
                            break;
                        }
                    case 1://2008
                        {
                            Period.Value = "[Период].[Период].[2007_],[Период].[Период].[2008_],[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010]";
                            break;
                        }
                    case 2://2009
                        {
                            Period.Value = "[Период].[Период].[2007_],[Период].[Период].[2008_],[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010]";
                            break;
                        }
                    case 3://2010
                        {
                            Period.Value = "[Период].[Период].[2008_],[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010],[Период].[Период].[2012brпланbrдекабрьbr2010]";
                            break;
                        }
                    case 4://2011
                        {
                            Period.Value = "[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010],[Период].[Период].[2012brпланbrдекабрьbr2010],[Период].[Период].[2013brпланbrдекабрьbr2010]";
                            break;
                        }

                    default:// 2012, 2013
                        {
                            Period.Value = "[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010],[Период].[Период].[2012brпланbrдекабрьbr2010],[Период].[Период].[2013brпланbrдекабрьbr2010]";
                            break;
                        }
                }
                if (ComboYear.SelectedIndex == 4 || ComboYear.SelectedIndex == 5 || ComboYear.SelectedIndex == 6)
                {
                    currentYearStr = currentYearStr.Replace("план, декабрь 2010", "ФЗ от 13 декабря 2010 г. № 357-ФЗ");
                }

                Label3.Text = string.Format("Межбюджетные трансферты субъектам РФ из Федеральных фондов на {0}, темп роста к {1} году", currentYearStr, UserParams.PeriodLastYear.Value);
                
                headerLayout = new GridHeaderLayout(UltraWebGridFF);
                UltraWebGridFF.Bands.Clear();
                UltraWebGridFF.DataBind();

                string patternValue = lbSybject.Text;
                int defaultRowIndex = 1;
                if (patternValue == string.Empty)
                {
                    patternValue = UserParams.StateArea.Value;
                    defaultRowIndex = 0;
                }

                UltraGridRow row = CRHelper.FindGridRow(UltraWebGridFF, patternValue, 0, defaultRowIndex);
                ActivateGridRow(row);
            }

#warning метод databind() иногда вызывается 2 раза

            if (UltraChartFF1.DataSource == null)
            {
                UltraChartFF1.DataBind();                
            }

            if (UltraChartFF2.DataSource == null)
            {
                UltraChartFF2.DataBind();
            }
        }

        #region Обработчики грида

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;
            lbSybject.Text = subject;

            if (RegionsNamingHelper.IsRF(subject))
            {
                UserParams.Subject.Value = "]";
                UserParams.SubjectFO.Value = "]";
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                UserParams.Region.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}]", UserParams.Region.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }
            else
            {
                UserParams.Region.Value = RegionsNamingHelper.FullName(row.Cells[1].Text);
                UserParams.StateArea.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }

            UltraChartFF1.DataBind();
            UltraChartFF2.DataBind();

            string currentYearStr = ComboYear.SelectedValue.Insert(4, " год");
            lbFOSub.Text = string.Format("На {0}", currentYearStr);

            switch (ComboYear.SelectedIndex)
            {
                case 0://2007
                    {
                        Period.Value = "[Период].[Период].[2007_],[Период].[Период].[2008_],[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010]";
                        lbSubjectSub.Text = "Динамика за 2007-2011 годы";
                        break;
                    }
                case 1://2008
                    {
                        Period.Value = "[Период].[Период].[2007_],[Период].[Период].[2008_],[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010]";
                        lbSubjectSub.Text = "Динамика за 2007-2011 годы";
                        break;
                    }
                case 2://2009
                    {
                        Period.Value = "[Период].[Период].[2007_],[Период].[Период].[2008_],[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010]";
                        lbSubjectSub.Text = "Динамика за 2007-2011 годы";
                        break;
                    }
                case 3://2010
                    {
                        Period.Value = "[Период].[Период].[2008_],[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010],[Период].[Период].[2012brпланbrдекабрьbr2010]";
                        lbSubjectSub.Text = "Динамика за 2008-2012 годы";
                        break;
                    }
                case 4://2011
                    {
                        Period.Value = "[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010],[Период].[Период].[2012brпланbrдекабрьbr2010],[Период].[Период].[2013brпланbrдекабрьbr2010]";
                        lbSubjectSub.Text = "Динамика за 2009-2013 годы";
                        break;
                    }

                default:// 2012
                    {
                        Period.Value = "[Период].[Период].[2009_],[Период].[Период].[2010_],[Период].[Период].[2011brпланbrдекабрьbr2010],[Период].[Период].[2012brпланbrдекабрьbr2010],[Период].[Период].[2013brпланbrдекабрьbr2010]";
                        lbSubjectSub.Text = "Динамика за 2009-2013 годы";
                        break;
                    }

            }
            
           
        }

        protected void UltraWebGridFF_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("mfrf_0001_0004_ff_grid");
            dtFF = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtFF);
            for (int i = 0; i < dtFF.Rows.Count; i++)
            {
                for (int j = 2; j < dtFF.Columns.Count; j++)
                {
                    if (dtFF.Rows[i][j] != DBNull.Value)
                    {
                        if (j % 2 == 0)
                        {
                            dtFF.Rows[i][j] = Convert.ToDouble(dtFF.Rows[i][j]) / 1000;
                        }
                    }
                }
            }

            if (dtFF.Columns.Count > 2)
            {
                dtFF.Columns[1].ColumnName = "ФО";
            }

            UserParams.Filter.Value = ComboFO.SelectedValue;
            if (dtFF.Columns.Count > 2)
            {
                UltraWebGridFF.DataSource = dtFF;
            }
            /*
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGridFF.DataSource = CRHelper.SetDataTableFilter(dtFF, "ФО", RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            }
            else
            {
                UltraWebGridFF.DataSource = dtFF;
            }
              */
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGridFF.Height = Unit.Empty;
            }
        }

        protected void UltraWebGridFF_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        

        protected void UltraWebGridFF_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(87);
            }

            for (int i = 2 + 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(85);
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Header.Caption = "ФО";
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(45);
            
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            headerLayout.AddCell(e.Layout.Bands[0].Columns[1].Header.Caption);

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                string headerCaption = captions[0].Replace(";", "_");

                GridHeaderCell header = headerLayout.AddCell(headerCaption);
                header.AddCell("Сумма, млн.руб.", "Сумма трансфертов в бюджеты субъектов РФ");
                header.AddCell("Темп роста", "Темп роста суммы трансфертов к предыдущему году"); 
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGridFF_InitializeRow(object sender, RowEventArgs e)
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

        #region Обработчики диаграмм

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("mfrf_0001_0004_ff_chart1");
            dtChartFF1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChartFF1);
            RegionsNamingHelper.ReplaceRegionNames(dtChartFF1, 0);

            UltraChartFF1.DataSource = dtChartFF1;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("mfrf_0001_0004_ff_chart2");
            dtChartFF2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Series", dtChartFF2);

            for (int i = 0; i < dtChartFF2.Rows.Count; i++)
            {
                //                if (i == 0 && dtChartFF2.Rows[i][0] != DBNull.Value)
                //                {
                //                    dtChartFF2.Rows[i][0] = dtChartFF2.Rows[i][0].ToString().TrimEnd('_');
                //                    dtChartFF2.Rows[i][0] = dtChartFF2.Rows[i][0].ToString().Replace("br", "\n");
                //                }
                for (int j = 1; j < dtChartFF2.Columns.Count; j++)
                {
                    dtChartFF2.Columns[j].ColumnName = dtChartFF2.Columns[j].ColumnName.TrimEnd('_');
                    dtChartFF2.Columns[j].ColumnName = dtChartFF2.Columns[j].ColumnName.Replace("br", "\n");
                    if (dtChartFF2.Rows[i][j] != DBNull.Value)
                    {
                        dtChartFF2.Rows[i][j] = Convert.ToDouble(dtChartFF2.Rows[i][j]) / 1000;
                    }
                }
            }

            UltraChartFF2.DataSource = dtChartFF2;
        }

        #endregion

        #region Экспорт

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();
            IText title = cell.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbSybject.Text);

            title = cell.AddText();
            font = new System.Drawing.Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbSubjectSub.Text);
            UltraChartFF2.Width = 550;
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChartFF2));

            cell = row.AddCell();
            title = cell.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(" ");
            title = cell.AddText();
            font = new System.Drawing.Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbFO.Text + " " + lbFOSub.Text);
            UltraChartFF1.Width = 650;
            UltraChartFF1.Width = Convert.ToInt32(UltraChartFF1.Width.Value * 0.9);
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChartFF1));
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            //ReportPDFExporter1.PageSubTitle = Label3.Text;

            ReportPDFExporter1.Export(headerLayout, Label3.Text);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label3.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChartFF2, lbSybject.Text + " " + lbSubjectSub.Text, sheet2, 3);
            UltraChartFF1.Width = UltraChartFF2.Width;
            UltraChartFF1.Legend = UltraChartFF2.Legend;
            ReportExcelExporter1.Export(UltraChartFF1, lbFOSub.Text, sheet3, 3);
        }
        
        #endregion
    }
}