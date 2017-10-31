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

        //���������
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
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";

            #region ������������� ����������

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

             #region ��������� ���������

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
            UltraChartFF.TitleTop.Text = "  ���.���";
            UltraChartFF.TitleTop.HorizontalAlign = StringAlignment.Near;
            UltraChartFF.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            
           #endregion

            CrossLink.Visible = true;
            CrossLink.Text = "�����������&nbsp;�����&nbsp;(��)";
            CrossLink.NavigateUrl = "~/reports/MFRF_0001_0004/Default_FF.aspx";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
       }
      
        protected override void Page_Load(Object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)   
            { //������������� ��������� ���������� ��� ������ ���������
              //lbSubject.Text = string.Empty;
              lbSubjectSub.Text = string.Empty;
                
              Collection<string> years = new Collection<string>();
               years.Add("2007");
               years.Add("2008");
               years.Add("2009");
               years.Add("2010");
               years.Add("2011 (����, ������� 2010)");
               years.Add("2012 (����, ������� 2010)");
               years.Add("2013 (����, ������� 2010)");

               ComboYear.Title = "���";
               ComboYear.Width = 227;
               ComboYear.MultiSelect = false;
               ComboYear.FillValues(years);
               ComboYear.Set�heckedState("2011 (����, ������� 2010)", true);
             
               UserParams.Filter.Value = "��� ����������� ������";
               ComboFO.Title = "��";
               ComboFO.Width = 254;
               ComboFO.MultiSelect =false;
               ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
               ComboFO.Set�heckedState(UserParams.Filter.Value,true);


                Collection <string> fonds = new Collection<string>();
                 fonds.Add("��� ����������� �����");
                 fonds.Add("����������� ���� ���������� ��������� ��������");
                 fonds.Add("����������� ���� �����������");
                 fonds.Add("����������� ���� ���������������� ��������");
                 fonds.Add("����������� ���� ������������� ��������");
                 fonds.Add("���� ������������ ����������");
                 
                 ComboFF.Title = "��";
                 ComboFF.Width = 254;
                 ComboFF.MultiSelect = false;
                 ComboFF.FillValues(fonds);
                 ComboFF.Set�heckedState("��� ����������� �����", true);
                
                  if (!string.IsNullOrEmpty(UserParams.Region.Value))
                {
                    ComboFO.Set�heckedState(UserParams.Region.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboFO.Set�heckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                }
            

            lbSubject.Text = string.Empty;
           }
            Label1.Text = "������������� ������� ����������� ������ ����� ���������� ��";
            
            SelectedFO.Value = ComboFO.SelectedValue;
             if (RFSelected)
            {
                SelectedFO.Value = " ";
            }
            else
            {
                SelectedFO.Value = string.Format(".[{0}]", ComboFO.SelectedValue);
            }

            // ����� ��
            
            switch (ComboFF.SelectedIndex)
            {
                case 0:
                    {
                        SelectedFF.Value = "[�� ��].[������ ����������].[��� ����������]";
                        Dolya.Value = "{Except([�� ��].[������ ����������].[���].AllMembers,{[�� ��].[������ ����������].[��� ����������].[���������������� ������]})}*{ [Measures].[��������� ������� ���], [Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� �����]}";
                        Fond.Value = "[Measures].[��� ����������� �����]";
                        break;
                    }
                case 1:
                    {
                        SelectedFF.Value = "[�� ��].[������ ����������].[��� ����������].[����������� ���� ���������� ��������� ��������]";
                        Dolya.Value = "{{[������ ������] - Tail ([������ ������])}*{[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� ������],[Measures].[���� � ����� �����]}} +{{Tail([������ ������])}* {[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� �����]}} ";
                        Fond.Value = "[�� ��].[������ ����������].[��� ����������].[����������� ���� ���������� ��������� ��������].LastChild";
                        break;
                    }
                case 2:
                    {
                        SelectedFF.Value = "[�� ��].[������ ����������].[��� ����������].[����������� ���� �����������]";
                        Dolya.Value = "{{[������ ������] - Tail ([������ ������])}*{[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� ������],[Measures].[���� � ����� �����]}} +{{Tail([������ ������])}* {[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� �����]}} ";
                        Fond.Value = "[�� ��].[������ ����������].[��� ����������].[����������� ���� �����������].LastChild";
                        break;
                    }
                case 3:
                    {
                        SelectedFF.Value = "[�� ��].[������ ����������].[��� ����������].[����������� ���� ���������������� ��������]";
                        Dolya.Value = "{{[������ ������] - Tail ([������ ������])}*{[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� ������],[Measures].[���� � ����� �����]}} +{{Tail([������ ������])}* {[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� �����]}} ";
                        Fond.Value = "[�� ��].[������ ����������].[��� ����������].[����������� ���� ���������������� ��������].LastChild";
                        break;
                    }
                case 4:
                    {
                        SelectedFF.Value = "[�� ��].[������ ����������].[��� ����������].[����������� ���� ������������� ��������]";
                        Dolya.Value = "{{[������ ������] - Tail ([������ ������])}*{[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� ������],[Measures].[���� � ����� �����]}} +{{Tail([������ ������])}* {[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� �����]}} ";
                        Fond.Value = "[�� ��].[������ ����������].[��� ����������].[����������� ���� ������������� ��������].LastChild";
                        break;
                    }
                case 5:
                    {
                        SelectedFF.Value = "[�� ��].[������ ����������].[��� ����������].[���� ������������ ����������]";
                        Dolya.Value = "{{[������ ������] - Tail ([������ ������])}*{[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� ������],[Measures].[���� � ����� �����]}} +{{Tail([������ ������])}* {[Measures].[��������� ������� ���],[Measures].[��������� ������� ���],[Measures].[���� �����_],[Measures].[���� � ����� �����]}} ";
                        Fond.Value = "[�� ��].[������ ����������].[��� ����������].[���� ������������ ����������].LastChild";
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
                    UserParams.VariantMesOtch.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2007 ���\"]";
                    UserParams.Filter.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2007 ���\"]";
                    break;    
                 }
              case 1: //2008
                  {
                      pValue = ComboYear.SelectedValue;
                      UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                      UserParams.VariantMesOtch.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2008 ��� � �� �������� ������2009 � 2010 �����\"]";
                      UserParams.Filter.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2007 ���\"]";
                      break;
                  }
                case 2: //2009
                  {
                      pValue = ComboYear.SelectedValue;
                      UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                      UserParams.VariantMesOtch.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2009 ��� � �� �������� ������2010 � 2011 �����\"].[�� � ����������� �� 2 ������� 2009 �. � 309-��]";
                      UserParams.Filter.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2008 ��� � �� �������� ������2009 � 2010 �����\"]";
                      break;
                  }
                case 3: //2010
                    {
                        pValue = ComboYear.SelectedValue;
                        UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
                        UserParams.Filter.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2009 ��� � �� �������� ������2010 � 2011 �����\"].[�� � ����������� �� 2 ������� 2009 �. � 309-��]";
                        UserParams.VariantMesOtch.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2010 ��� � �� �������� ������ 2011 � 2012 �����\"].[�� �� 3 ������ 2010 �. � 278-��]";
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
                        UserParams.Filter.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2010 ��� � �� �������� ������ 2011 � 2012 �����\"].[�� �� 3 ������ 2010 �. � 278-��]";
                        UserParams.VariantMesOtch.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2011 ��� � �� �������� ������ 2012 � 2013 �����\"].[�� �� 13 ������� 2010 �. � 357-��]";
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
                        UserParams.Filter.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2011 ��� � �� �������� ������ 2012 � 2013 �����\"].[�� �� 13 ������� 2010 �. � 357-��]";
                        UserParams.VariantMesOtch.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2011 ��� � �� �������� ������ 2012 � 2013 �����\"].[�� �� 13 ������� 2010 �. � 357-��]";
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
                        UserParams.Filter.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2011 ��� � �� �������� ������ 2012 � 2013 �����\"].[�� �� 13 ������� 2010 �. � 357-��]";
                        UserParams.VariantMesOtch.Value = "[�������].[���� �����].[���].[������, ������������ ����������� ������� \"� ����������� �������\"].[�� \"� ����������� ������� �� 2011 ��� � �� �������� ������ 2012 � 2013 �����\"].[�� �� 13 ������� 2010 �. � 357-��]";
                        break;
                    }
            }

            if (!Page.IsPostBack || (!UserParams.PeriodYear.ValueIs(pValue)) || (!UserParams.Filter.ValueIs(ComboYear.SelectedValue)) )
            {
                string currentYearStr = ComboYear.SelectedValue.Insert(4, " ���");

                if (ComboYear.SelectedIndex == 4 || ComboYear.SelectedIndex == 5 || ComboYear.SelectedIndex == 6)
                {
                    currentYearStr = currentYearStr.Replace("����, ������� 2010", "�� �� 13 ������� 2010 �. � 357-��");
                }

                Label2.Text = string.Format("���������� ������ �� ������������ ������� � ����������� ������ �������� �� �� {0}", currentYearStr);

                UserParams.PeriodYear.Value = pValue;

                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
                
                lbSubject.Font.Bold = true;
                if (ComboFO.SelectedIndex == 0)
                {
                    lbSubject.Text = "���������� ���������";
                }
                else
                {
                    lbSubject.Text = string.Format("{0}", ComboFO.SelectedValue);
                }

                lbSubjectSub.Text = string.Format(" {0}", ComboFF.SelectedValue);

              }
     #warning DataBind()���������� ��� ����

            if (UltraChartFF.DataSource == null)
            {
                UltraChartFF.DataBind();
            }
        }

        #region ���������� �����
       
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("mfrf_0001_0005_grid");
            dtData = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������� �����", dtData);
            if (ComboFF.SelectedIndex == 0) // ���� ������ �������� "��� ����������� �����"
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
            else //���� ������ ���� �� ������
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
                dtData.Columns[1].ColumnName = "��";

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
                UltraWebGrid.DataSource = CRHelper.SetDataTableFilter(dtData, "��", RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
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
                  string formatString = columnCaption.Contains("����") || columnCaption.Contains("����") ? "P2" : "N3";
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
             }
            
            headerLayout.AddCell("����������� �����");
            headerLayout.AddCell("��");
           
           if (ComboFF.SelectedIndex == 0) // ��� ����������� �����
           {
            
               for (int colNum = 2 ; colNum < UltraWebGrid.Columns.Count ; colNum +=4)
               {
                   string[] captions = e.Layout.Bands[0].Columns[colNum].Header.Caption.Split(';');
                   GridHeaderCell cell0 = headerLayout.AddCell(string.Format("{0}", captions));

                   cell0.AddCell("����� ������� ���, ���.���.", "����� ����������� � ������� ��������� �� � ������� ����");
                   cell0.AddCell("�����, ���.���.", "����� ����������� � ������� ��������� ��");
                   cell0.AddCell("���� �����", "���� ����� � �������� ����");
                   cell0.AddCell("���� � ����� ����� ������������ �����������", "���� � ����� ����� ������������ ����������� ���������� ��������");
               }
               headerLayout.ApplyHeaderInfo();

           }
           else // ���� ����������� ����
           {
              GridHeaderCell cell1 = headerLayout.AddCell(string.Format("{0}", ComboFF.SelectedValue));

               for (int colNum = 2; colNum < UltraWebGrid.Columns.Count; colNum += 5)
               {
                   string[] captions = e.Layout.Bands[0].Columns[colNum].Header.Caption.Split(';');
                   GridHeaderCell cell2 = cell1.AddCell(string.Format("{0}", captions));

                   cell2.AddCell("����� ������� ���, ���.���.", "����� ����������� � ������� ��������� �� � ������� ����");
                   cell2.AddCell("�����, ���.���.", "����� ����������� � ������� ��������� ��");
                   cell2.AddCell("���� �����", "���� ����� � �������� ����");
                   if (colNum + 3 != UltraWebGrid.Columns.Count - 1 )
                   {
                       cell2.AddCell("���� � ����� ������ �����", "���� ������� � ����� ������ ����� ���������� ��������");   
                   }
                   cell2.AddCell("���� � ����� ����� ������������ �����������", "���� � ����� ����� ������������ ����������� ���������� ��������");
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
                            hint = "���� � �������� ����";
                        }
                        else
                        {
                            if (rate < 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                hint = "�������� � �������� ����";
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
                             hint = "���� � �������� ����";
                         }
                         else
                         {
                             if (rate < 100)
                             {
                                 e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                 hint = "�������� � �������� ����";
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

        #region ���������� ��������
        protected void UltraChartFF_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("mfrf_0001_0005_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart);
            string currentYearStr = ComboYear.SelectedValue.Insert(4, " ���");
            UltraChartFF.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n{0}\n<DATA_VALUE:N3> ���.���.", currentYearStr);
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

        #region ������� � Excel

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

        #region ������� � Pdf

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



