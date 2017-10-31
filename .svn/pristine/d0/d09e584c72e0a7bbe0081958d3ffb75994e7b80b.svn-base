using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Collections.ObjectModel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
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

namespace Krista.FM.Server.Dashboards.reports.MFRF_0001_0005
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtData;
        private DataTable dtChart;

        //параметры
        private CustomParam SelectedFF;
        private CustomParam SelectedFO;
        private CustomParam Dolya;
        private CustomParam Fond;

        private GridHeaderLayout headerLayout;

        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45);

            UltraChartFF.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth);
            UltraChartFF.Height = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 - 100);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров

            if (SelectedFF == null)
            {
                SelectedFF = UserParams.CustomParam("select_FF");
            }
            if (SelectedFO == null)
            {
                SelectedFO = UserParams.CustomParam("select_FO");
            }
            if (Fond == null)
            {
                Fond = UserParams.CustomParam("fond");
            }
            if (Dolya == null)
            {
                Dolya = UserParams.CustomParam("dolya");
            }
            #endregion

             #region Настройка диаграммы

            UltraChartFF.ChartType = ChartType.ParetoChart;
            UltraChartFF.Data.SwapRowsAndColumns = false;
            UltraChartFF.BorderWidth = 0;
            UltraChartFF.Axis.X.Extent = 70;            
            UltraChartFF.Axis.Y.Extent = 40;
       
            UltraChartFF.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChartFF.Axis.Y2.RangeMin = 0;
            UltraChartFF.Axis.Y2.RangeMax = 100;
            UltraChartFF.Axis.Y2.Extent = 40;
            UltraChartFF.Axis.Y2.Visible = true;
            UltraChartFF.Axis.Y2.Labels.Visible = true;
            UltraChartFF.Axis.Y2.MajorGridLines.Visible = false;
            UltraChartFF.Axis.Y2.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            
            /*
             //
            UltraChartFF.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            UltraChartFF.Axis.X.Labels.Layout.BehaviorCollection.Clear();
            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.ClipText = true;
            behavior.Enabled = false;
            behavior.Trimming = StringTrimming.Word;
            behavior.UseOnlyToPreventCollisions = false;
            UltraChartFF.Axis.X.Labels.Layout.BehaviorCollection.Add(behavior);
            //*/
            
            UltraChartFF.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartFF_FillSceneGraph);
            UltraChartFF.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChartFF.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChartFF.ParetoChart.ColumnSpacing = 0;
            UltraChartFF.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChartFF.ColorModel.ColorBegin = Color.DarkGoldenrod;
            UltraChartFF.ColorModel.ColorEnd = Color.Navy;
            UltraChartFF.TitleLeft.Visible = false;
            UltraChartFF.TitleTop.Visible = true;
            UltraChartFF.TitleTop.Text = "  Млн.руб";
            UltraChartFF.TitleTop.HorizontalAlign = StringAlignment.Near;
            UltraChartFF.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            
           #endregion

            CrossLink.Visible = true;
            CrossLink.Text = "Федеральные&nbsp;фонды&nbsp;(РФ)";
            CrossLink.NavigateUrl = "~/reports/MFRF_0001_0004/Default_FF.aspx";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
       }
      
        protected override void Page_Load(Object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)   
            { //Инициализация элементов управления при первом обращении
              //lbSubject.Text = string.Empty;
              lbSubjectSub.Text = string.Empty;
                
              Collection<string> years = new Collection<string>();
               years.Add("2007");
               years.Add("2008");
               years.Add("2009");
               years.Add("2010");
               years.Add("2011 (план, декабрь 2010)");
               years.Add("2012 (план, декабрь 2010)");
               years.Add("2013 (план, декабрь 2010)");

               ComboYear.Title = "Год";
               ComboYear.Width = 227;
               ComboYear.MultiSelect = false;
               ComboYear.FillValues(years);
               ComboYear.SetСheckedState("2011 (план, декабрь 2010)", true);
             
               UserParams.Filter.Value = "Все федеральные округа";
               ComboFO.Title = "ФО";
               ComboFO.Width = 254;
               ComboFO.MultiSelect =false;
               ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
               ComboFO.SetСheckedState(UserParams.Filter.Value,true);


                Collection <string> fonds = new Collection<string>();
                 fonds.Add("Все федеральные фонды");
                 fonds.Add("Федеральный фонд финансовой поддержки регионов");
                 fonds.Add("Федеральный фонд компенсаций");
                 fonds.Add("Федеральный фонд софинансирования расходов");
                 fonds.Add("Федеральный фонд регионального развития");
                 fonds.Add("Иные межбюджетные трансферты");
                 
                 ComboFF.Title = "ФФ";
                 ComboFF.Width = 254;
                 ComboFF.MultiSelect = false;
                 ComboFF.FillValues(fonds);
                 ComboFF.SetСheckedState("Все федеральные фонды", true);
                
                  if (!string.IsNullOrEmpty(UserParams.Region.Value))
                {
                    ComboFO.SetСheckedState(UserParams.Region.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboFO.SetСheckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                }
            

            lbSubject.Text = string.Empty;
           }
            Label1.Text = "Распределение средств федеральных фондов между субъектами РФ";
            
            SelectedFO.Value = ComboFO.SelectedValue;
             if (RFSelected)
            {
                SelectedFO.Value = " ";
            }
            else
            {
                SelectedFO.Value = string.Format(".[{0}]", ComboFO.SelectedValue);
            }

            // Выбор фф
            
            switch (ComboFF.SelectedIndex)
            {
                case 0:
                    {
                        SelectedFF.Value = "[МФ РФ].[Сопост Показатели].[Все показатели]";
                        Dolya.Value = "{Except([МФ РФ].[Сопост Показатели].[Тип].AllMembers,{[МФ РФ].[Сопост Показатели].[Все показатели].[Несопоставленные данные]})}*{ [Measures].[Исполнено прошлый год], [Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общей сумме]}";
                        Fond.Value = "[Measures].[Все федеральные фонды]";
                        break;
                    }
                case 1:
                    {
                        SelectedFF.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Федеральный фонд финансовой поддержки регионов]";
                        Dolya.Value = "{{[Группы фондов] - Tail ([Группы фондов])}*{[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общем объеме],[Measures].[Доля в общей сумме]}} +{{Tail([Группы фондов])}* {[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общей сумме]}} ";
                        Fond.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Федеральный фонд финансовой поддержки регионов].LastChild";
                        break;
                    }
                case 2:
                    {
                        SelectedFF.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Федеральный фонд компенсаций]";
                        Dolya.Value = "{{[Группы фондов] - Tail ([Группы фондов])}*{[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общем объеме],[Measures].[Доля в общей сумме]}} +{{Tail([Группы фондов])}* {[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общей сумме]}} ";
                        Fond.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Федеральный фонд компенсаций].LastChild";
                        break;
                    }
                case 3:
                    {
                        SelectedFF.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Федеральный фонд софинансирования расходов]";
                        Dolya.Value = "{{[Группы фондов] - Tail ([Группы фондов])}*{[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общем объеме],[Measures].[Доля в общей сумме]}} +{{Tail([Группы фондов])}* {[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общей сумме]}} ";
                        Fond.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Федеральный фонд софинансирования расходов].LastChild";
                        break;
                    }
                case 4:
                    {
                        SelectedFF.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Федеральный фонд регионального развития]";
                        Dolya.Value = "{{[Группы фондов] - Tail ([Группы фондов])}*{[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общем объеме],[Measures].[Доля в общей сумме]}} +{{Tail([Группы фондов])}* {[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общей сумме]}} ";
                        Fond.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Федеральный фонд регионального развития].LastChild";
                        break;
                    }
                case 5:
                    {
                        SelectedFF.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Иные межбюджетные трансферты]";
                        Dolya.Value = "{{[Группы фондов] - Tail ([Группы фондов])}*{[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общем объеме],[Measures].[Доля в общей сумме]}} +{{Tail([Группы фондов])}* {[Measures].[Исполнено прошлый год],[Measures].[Исполнено текущий год],[Measures].[Темп роста_],[Measures].[Доля в общей сумме]}} ";
                        Fond.Value = "[МФ РФ].[Сопост Показатели].[Все показатели].[Иные межбюджетные трансферты].LastChild";
                        break;   
                    }
            }
           //
           
            string pValue = string.Empty;

            switch (ComboYear.SelectedIndex)
            { case 0: //2007
                {
                    pValue = ComboYear.SelectedValue;
                    UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                    UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2007 год\"]";
                    UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2007 год\"]";
                    break;    
                 }
              case 1: //2008
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
                case 3: //2010
                    {
                        pValue = ComboYear.SelectedValue;
                        UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2009 год и на плановый период 2010 и 2011 годов\"].[ФЗ с изменениями от 2 декабря 2009 г. № 309-ФЗ]";
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2010 год и на плановый период 2011 и 2012 годов\"].[ФЗ от 3 ноября 2010 г. № 278-ФЗ]";
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
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2010 год и на плановый период 2011 и 2012 годов\"].[ФЗ от 3 ноября 2010 г. № 278-ФЗ]";
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                        break;
                    }
                case 5:  //2012
                    {
                        string[] st = ComboYear.SelectedValue.Split(' ');
                        if (st.Length > 1)
                        {
                            pValue = st[0];
                            UserParams.PeriodLastYear.Value = (Convert.ToInt32(st[0]) - 1).ToString();
                        }
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                        break;
                    }
                case 6:  //2013
                    {
                        string[] st = ComboYear.SelectedValue.Split(' ');
                        if (st.Length > 1)
                        {
                            pValue = st[0];
                            UserParams.PeriodLastYear.Value = (Convert.ToInt32(st[0]) - 1).ToString();
                        }
                        UserParams.Filter.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                        UserParams.VariantMesOtch.Value = "[Вариант].[МФРФ Фонды].[Все].[Данные, утвержденные Федеральным законом \"О федеральном бюджете\"].[ФЗ \"О федеральном бюджете на 2011 год и на плановый период 2012 и 2013 годов\"].[ФЗ от 13 декабря 2010 г. № 357-ФЗ]";
                        break;
                    }
            }

            if (!Page.IsPostBack || (!UserParams.PeriodYear.ValueIs(pValue)) || (!UserParams.Filter.ValueIs(ComboYear.SelectedValue)) )
            {
                string currentYearStr = ComboYear.SelectedValue.Insert(4, " год");

                if (ComboYear.SelectedIndex == 4 || ComboYear.SelectedIndex == 5 || ComboYear.SelectedIndex == 6)
                {
                    currentYearStr = currentYearStr.Replace("план, декабрь 2010", "ФЗ от 13 декабря 2010 г. № 357-ФЗ");
                }

                Label2.Text = string.Format("Финансовая помощь из федерального бюджета в собственный бюджет субъекта РФ на {0}", currentYearStr);

                UserParams.PeriodYear.Value = pValue;

                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
                
                lbSubject.Font.Bold = true;
                if (ComboFO.SelectedIndex == 0)
                {
                    lbSubject.Text = "Российская Федерация";
                }
                else
                {
                    lbSubject.Text = string.Format("{0}", ComboFO.SelectedValue);
                }

                lbSubjectSub.Text = string.Format(" {0}", ComboFF.SelectedValue);

              }
     #warning DataBind()вызывается два раза

            if (UltraChartFF.DataSource == null)
            {
                UltraChartFF.DataBind();
            }
        }

        #region обработчик грида
       
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("mfrf_0001_0005_grid");
            dtData = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtData);
            if (ComboFF.SelectedIndex == 0) // если выбран параметр "все федеральные фонды"
            {
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    for (int j = 2; j < dtData.Columns.Count; j = j + 4)
                    {
                        if (dtData.Rows[i][j] != DBNull.Value)
                        {
                            dtData.Rows[i][j] = Convert.ToDouble(dtData.Rows[i][j]) / 1000;

                        }
                        if (dtData.Rows[i][j + 1] != DBNull.Value)
                        {
                            dtData.Rows[i][j + 1] = Convert.ToDouble(dtData.Rows[i][j + 1]) / 1000;
                        }
                       
                    }
                }

            }
            else //если выбран один из фондов
            {
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    for (int j = 2; j < dtData.Columns.Count; j = j + 5)
                    {
                        if (dtData.Rows[i][j] != DBNull.Value)
                        {
                            dtData.Rows[i][j] = Convert.ToDouble(dtData.Rows[i][j]) / 1000;

                        }
                        if (dtData.Rows[i][j + 1] != DBNull.Value)
                        {
                            dtData.Rows[i][j + 1] = Convert.ToDouble(dtData.Rows[i][j + 1]) / 1000;
                        }
                    }
                }
            }
            if (dtData.Columns.Count > 2)
            {
                dtData.Columns[1].ColumnName = "ФО";

            }

            foreach (DataColumn column in dtData.Columns)
            {
                column.ColumnName = column.ColumnName.Replace("\"", "'");
                column.Caption = column.Caption.Replace("\"", "'");
                column.ColumnName = column.ColumnName.Replace(@"
", "'");
                column.Caption = column.Caption.Replace(@"
", " ");
            }

            UserParams.Filter.Value = ComboFO.SelectedValue;
            if (dtData.Columns.Count > 2)
            {
                UltraWebGrid.DataSource = dtData;
            }
            /* if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.DataSource = CRHelper.SetDataTableFilter(dtData, "ФО", RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            }
            else
            {
                UltraWebGrid.DataSource = dtData;
            }*/
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
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(45);

             int i;
             for (i = 2; i < e.Layout.Bands[0].Columns.Count; i++ )
             {
                  e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                  string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption.ToLower();
                  string formatString = columnCaption.Contains("темп") || columnCaption.Contains("доля") ? "P2" : "N3";
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
             }
            
            headerLayout.AddCell("Федеральный округ");
            headerLayout.AddCell("ФО");
           
           if (ComboFF.SelectedIndex == 0) // все федеральные фонды
           {
            
               for (int colNum = 2 ; colNum < UltraWebGrid.Columns.Count ; colNum +=4)
               {
                   string[] captions = e.Layout.Bands[0].Columns[colNum].Header.Caption.Split(';');
                   GridHeaderCell cell0 = headerLayout.AddCell(string.Format("{0}", captions));

                   cell0.AddCell("Сумма прошлый год, млн.руб.", "Сумма трансфертов в бюджеты субъектов РФ в прошлом году");
                   cell0.AddCell("Сумма, млн.руб.", "Сумма трансфертов в бюджеты субъектов РФ");
                   cell0.AddCell("Темп роста", "Темп роста к прошлому году");
                   cell0.AddCell("Доля в общей сумме межбюджетных трансфертов", "Доля в общей сумме межбюджетных трансфертов выбранного субъекта");
               }
               headerLayout.ApplyHeaderInfo();

           }
           else // один федеральный фонд
           {
              GridHeaderCell cell1 = headerLayout.AddCell(string.Format("{0}", ComboFF.SelectedValue));

               for (int colNum = 2; colNum < UltraWebGrid.Columns.Count; colNum += 5)
               {
                   string[] captions = e.Layout.Bands[0].Columns[colNum].Header.Caption.Split(';');
                   GridHeaderCell cell2 = cell1.AddCell(string.Format("{0}", captions));

                   cell2.AddCell("Сумма прошлый год, млн.руб.", "Сумма трансфертов в бюджеты субъектов РФ в прошлом году");
                   cell2.AddCell("Сумма, млн.руб.", "Сумма трансфертов в бюджеты субъектов РФ");
                   cell2.AddCell("Темп роста", "Темп роста к прошлому году");
                   if (colNum + 3 != UltraWebGrid.Columns.Count - 1 )
                   {
                       cell2.AddCell("Доля в общем объеме фонда", "Доля средств в общем объеме фонда выбранного субъекта");   
                   }
                   cell2.AddCell("Доля в общей сумме межбюджетных трансфертов", "Доля в общей сумме межбюджетных трансфертов выбранного субъекта");
               }
               headerLayout.ApplyHeaderInfo();
           }

         

         /*  if (UltraWebGrid.Columns.Count <3)
           {
               UltraWebGrid.Bands.Clear();
           }
           */
         
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
             if (ComboFF.SelectedIndex == 0)
             {
                for (int i=4; i<e.Row.Cells.Count; i+=4)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double rate = 100 * Convert.ToDouble(e.Row.Cells[i].Value);
                        string hint = string.Empty;

                        if (rate >= 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            hint = "Рост к прошлому году";
                        }
                        else
                        {
                            if (rate < 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                hint = "Снижение к прошлому году";
                            }
                        }
                        e.Row.Cells[i].Title = hint;
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
             }
             else
             {
                 for (int i = 4; i < e.Row.Cells.Count; i = i + 5)
                 {
                     if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                     {
                         double rate = 100 * Convert.ToDouble(e.Row.Cells[i].Value);
                         string hint = string.Empty;

                         if (rate >= 100)
                         {
                             e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                             hint = "Рост к прошлому году";
                         }
                         else
                         {
                             if (rate < 100)
                             {
                                 e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                 hint = "Снижение к прошлому году";
                             }
                         }
                         e.Row.Cells[i].Title = hint;
                         e.Row.Cells[i].Style.CustomRules =
                             "background-repeat: no-repeat; background-position: left center; margin: 2px";
                     }
                 }
             }
        }

        #endregion

        #region обработчик диаграмм
        protected void UltraChartFF_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("mfrf_0001_0005_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart);
            string currentYearStr = ComboYear.SelectedValue.Insert(4, " год");
            UltraChartFF.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n{0}\n<DATA_VALUE:N3> млн.руб.", currentYearStr);
            RegionsNamingHelper.ReplaceRegionNames(dtChart,0);
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                for (int j = 1; j < dtChart.Columns.Count; j++)
                {
                   if (dtChart.Rows[i][j] != DBNull.Value)
                    {
                        dtChart.Rows[i][j] = Convert.ToDouble(dtChart.Rows[i][j])/1000;
                    }
                }
            }
            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
               NumericSeries series1 = CRHelper.GetNumericSeries(i, dtChart);
               UltraChartFF.Series.Add(series1);
            }
        }
        protected void UltraChartFF_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
             for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 30;
                    text.bounds.Height = 70;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Trimming = StringTrimming.None;
                    text.labelStyle.Font = new System.Drawing.Font("Verdana", 7);
                }
                 
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Series != null)
                        {   
                            box.DataPoint.Label = RegionsNamingHelper.FullName(box.DataPoint.Label);
                            
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

       private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.GridColumnWidthScale = 1.5;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChartFF, lbSubject.Text + lbSubjectSub.Text ,sheet2, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            //ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 40;
            ReportPDFExporter1.Export(headerLayout, Label2.Text, section1);
            UltraChartFF.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.86));
            ReportPDFExporter1.Export(UltraChartFF, lbSubject.Text + lbSubjectSub.Text +"\n"+ Label2.Text, section2);
        }

        #endregion

      }
}



