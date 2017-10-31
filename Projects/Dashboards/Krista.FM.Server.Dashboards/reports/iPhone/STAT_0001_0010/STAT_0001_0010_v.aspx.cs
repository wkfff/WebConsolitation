using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class STAT_0001_0010_v : CustomReportPage
    {
        //private DataTable dt;
        private DataTable dtChart;
        private DateTime debtsCurrDateTime;
        private DateTime debtsLastDateTime;
        private DataTable dtMap1;
        private DataTable dtMap2;
        // ����
        private CustomParam periodDay;
        private CustomParam periodLastDay;

        private DataTable dtKoeff;

        private DateTime redundantLevelRFDateTime;

        private DateTime currDateTime;
        private DateTime lastDateTime;

        // ������� ����
        private CustomParam periodCurrentDate;
        // ������� ���� ��� �������������
        private CustomParam debtsPeriodCurrentDate;
        // �� ������ ����� ��� �������������
        private CustomParam debtsPeriodLastWeekDate;
        // �� ������ �����
        private CustomParam periodLastWeekDate;

        // ������� ���� ��� ������ ����������� �� ��
        private CustomParam redundantLevelRFDate;

        //private DateTime redundantLevelRFDateTime;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region ���������

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (debtsPeriodCurrentDate == null)
            {
                debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            }
            if (debtsPeriodLastWeekDate == null)
            {
                debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            if (redundantLevelRFDate == null)
            {
                redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");
            }

            #endregion

            UltraChart4.Width = 740;
            UltraChart4.Height = 240;

            string query = DataProvider.GetQueryText("STAT_0001_0010_chart1_date");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDateSakhalin);

            DateTime currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", currDateTimeSakhalin, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", lastDateTimeSakhalin, 5);

            #region ��������� ��������� 4

            UltraChart4.ChartType = ChartType.AreaChart;
            UltraChart4.Border.Thickness = 0;

            UltraChart4.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart4.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart4.Axis.X.Extent = 50;
            UltraChart4.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.X.Labels.Visible = true;
            //  UltraChart4.Axis.X.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart4.Axis.Y.Labels.Font = new Font("Verdana", 8);
            //  UltraChart4.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Extent = 40;

            UltraChart4.TitleLeft.Visible = true;
            UltraChart4.TitleLeft.Text = "���.���.";
            UltraChart4.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart4.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart4.TitleLeft.Extent = 40;
            UltraChart4.TitleLeft.Margins.Top = 0;
            UltraChart4.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart4.TitleLeft.FontColor = Color.White;

            UltraChart4.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart4.Data.EmptyStyle.Text = " ";
            UltraChart4.EmptyChartText = " ";

            UltraChart4.AreaChart.NullHandling = NullHandling.Zero;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 3;
            UltraChart4.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart4.Legend.Visible = true;
            UltraChart4.Legend.Location = LegendLocation.Top;
            //  UltraChart4.Legend.Margins.Right = IsSmallResolution ? 5 : Convert.ToInt32(UltraChart4.Width.Value) / 2;
            UltraChart4.Legend.SpanPercentage = 14;
            UltraChart4.Legend.Font = new Font("Verdana", 10);

            UltraChart4.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);
            UltraChart4.DataBinding += new EventHandler(UltraChart4_DataBinding);

            #endregion

            SetMapSettings(DundasMap1, "�������\n�����������", "#FROMVALUE{P2} - #TOVALUE{P2}");
            SetMapSettings(MapControl1, "�����������\n�����������\n�� 1 ��������", "#FROMVALUE{N1} - #TOVALUE{N1}");

            Text();

            // ��������� ����� �������
            DataTable dtDateCur = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0010_chart1_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtDateCur);
            periodCurrentDate.Value = dtDateCur.Rows[1][1].ToString();

            FillRegionMapCodes();

            FillMapData(DundasMap1, "STAT_0001_0010_tagCloud_data", 1, "{0} ({1:P2})");
            FillMapData(MapControl1, "STAT_0001_0010_grid_v", 11, "{0} ({1:N1})");

            UltraChart4.DataBind();

            query = DataProvider.GetQueryText("STAT_0001_0010_chart1_date");
            dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDateSakhalin);

            currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);
            lastDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", currDateTimeSakhalin, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", lastDateTimeSakhalin, 5);

            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.DataBind();
        }

        #region ����

        private GridHeaderLayout headerLayout;
        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            //e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 16;
            e.Layout.Bands[0].Columns[0].Width = 174;
            e.Layout.Bands[0].Columns[1].Width = 90;
            e.Layout.Bands[0].Columns[2].Width = 55;
            e.Layout.Bands[0].Columns[3].Width = 130;
            e.Layout.Bands[0].Columns[4].Width = 100;
            e.Layout.Bands[0].Columns[5].Width = 55;
            e.Layout.Bands[0].Columns[6].Width = 160;

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("");
            GridHeaderCell cell = headerLayout.AddCell("������� �����������");
            cell.AddCell("");
            cell.AddCell("����");

            headerLayout.AddCell("�����������. �����������, ���.");
            cell = headerLayout.AddCell("����������� ����������� �� 1 ��������");
            cell.AddCell("");
            cell.AddCell("����");
            headerLayout.AddCell("������������� �� ������� ���������� �����");

            headerLayout.ApplyHeaderInfo();
        }

        void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0010_grid_v");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtGrid);

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 11; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                row[0] = dtGrid.Rows[i][0].ToString().Replace("������������� �����", "�-�").Replace("����� ", "�.");

                AddColumnGroup(dtGrid, i, row, 1, 1);
                AddColumnGroup(dtGrid, i, row, 6, 3);
                AddColumnGroup(dtGrid, i, row, 11, 5);

                if (dtGrid.Rows[i][16] != DBNull.Value &&
                    dtGrid.Rows[i][16].ToString() != String.Empty &&
                    dtGrid.Rows[i][21] != DBNull.Value &&
                    dtGrid.Rows[i][21].ToString() != String.Empty)
                {
                    row[7] = String.Format("{0:N0} ���.���.<br/>{1:N0} �������", dtGrid.Rows[i][16], dtGrid.Rows[i][21]);
                }
                else
                {
                    row[7] = "�����������";
                }

                dtSource.Rows.Add(row);
            }

            dtSource.Columns.RemoveAt(10);
            dtSource.Columns.RemoveAt(9);
            dtSource.Columns.RemoveAt(8);
            dtSource.Columns.RemoveAt(4);

            UltraWebGrid.DataSource = dtSource;
        }

        private void AddColumnGroup(DataTable dtGrid, int i, DataRow row, int group, int column)
        {
            if (dtGrid.Rows[i][group] != DBNull.Value)
            {
                double value = Convert.ToDouble(dtGrid.Rows[i][group].ToString());
                double absoluteGrownValue = Convert.ToDouble(dtGrid.Rows[i][group + 1].ToString());
                string absoluteGrown;
                if (absoluteGrownValue > 0)
                {
                    switch (column)
                    {
                        case 1:
                            absoluteGrown = String.Format("+{0:P2}", absoluteGrownValue);
                            break;
                        case 3:
                            absoluteGrown = String.Format("+{0:N0}", absoluteGrownValue);
                            break;
                        default:
                            absoluteGrown = String.Format("+{0:N1}", absoluteGrownValue);
                            break;
                    }

                }
                else if (absoluteGrownValue < 0)
                {
                    switch (column)
                    {
                        case 1:
                            absoluteGrown = String.Format("-{0:P2}", Math.Abs(absoluteGrownValue));
                            break;
                        case 3:
                            absoluteGrown = String.Format("-{0:N0}", Math.Abs(absoluteGrownValue));
                            break;
                        default:
                            absoluteGrown = String.Format("-{0:N1}", Math.Abs(absoluteGrownValue));
                            break;
                    }
                }
                else
                {
                    switch (column)
                    {
                        case 1:
                            absoluteGrown = String.Format("{0:P2}", Math.Abs(absoluteGrownValue));
                            break;
                        case 3:
                            absoluteGrown = String.Format("{0:N0}", Math.Abs(absoluteGrownValue));
                            break;
                        default:
                            absoluteGrown = String.Format("{0:N1}", Math.Abs(absoluteGrownValue));
                            break;
                    }
                }

                double grownValue = Convert.ToDouble(dtGrid.Rows[i][group + 2].ToString());

                string grown = String.Empty;
                if (grownValue == 0)
                {
                    grown = String.Format("{0:P2}", grownValue);
                }
                else
                {
                    grown = grownValue > 0
                                   ? String.Format("+{0:P2}", grownValue)
                                   : String.Format("-{0:P2}", Math.Abs(grownValue));
                }

                string img = String.Empty;
                if (absoluteGrownValue != 0)
                {
                    img = absoluteGrownValue > 0
                              ? "<img src='../../../images/arrowRedUpBB.png'>"
                              : "<img src='../../../images/arrowGreenDownBB.png'>";
                }

                switch (column)
                {
                    case 1:
                        row[column] = String.Format("{0:P2}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    case 3:
                        row[column] = String.Format("{0:N0}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    default:
                        row[column] = String.Format("{0:N1}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                }

                string rankImg = String.Empty;
                if (dtGrid.Rows[i][group + 3].ToString() == "1")
                {
                    rankImg = "<img src='../../../images/StarYellow.png'>";
                }
                else if (dtGrid.Rows[i][group + 3].ToString() == dtGrid.Rows[i][group + 4].ToString())
                {
                    rankImg = "<img src='../../../images/StarGray.png'>";
                }

                row[column + 1] = String.Format("{0}&nbsp;{1}", rankImg, dtGrid.Rows[i][group + 3]);
            }
        }

        #endregion

        #region �����

        private void Text()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0010_chart1_date");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDateSakhalin);

            DateTime currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", currDateTimeSakhalin, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", lastDateTimeSakhalin, 5);

            query = DataProvider.GetQueryText("STAT_0001_0010_debts_mo_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDebtsDate);

            if (dtDebtsDate.Rows.Count > 1)
            {
                if (dtDebtsDate.Rows[0][1] != DBNull.Value && dtDebtsDate.Rows[0][1].ToString() != String.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != String.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
            }

            query = DataProvider.GetQueryText("STAT_0001_0010_debts_Sakhalin");
            DataTable dtCommentText = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtCommentText);
            double totalDebts = GetDoubleDTValue(dtCommentText, "C���� �������������");
            double totalLastWeekDebts = GetDoubleDTValue(dtCommentText, "C���� ������������� ������� ������");
            string dateTimeDebtsStr = String.Format("{0:dd.MM.yyyy}", debtsCurrDateTime.AddMonths(1));
            string dateLastTimeDebtsStr = String.Format("{0:dd.MM.yyyy}", debtsLastDateTime.AddMonths(1));
            double slavesCount = GetDoubleDTValue(dtCommentText, "���������� �������, ������� �������������");
            double debtsPercent = GetDoubleDTValue(dtCommentText, "������� �������������");
            string debtsPercentArrow = debtsPercent == 0
                                           ? "�� ����������"
                                           : debtsPercent > 0
                                           ? String.Format("����������� <img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'> �� <b>{0:N3}</b>&nbsp;���.���", Math.Abs(debtsPercent))
                                           : String.Format("����������� <img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'> �� <b>{0:N3}</b>&nbsp;���.���", Math.Abs(debtsPercent));

            string str10;
            if (totalLastWeekDebts == 0 && totalDebts == 0)
            {
                str10 = String.Format(@"��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;����������� ������������� �� ������� 
���������� �����<br/>", dateTimeDebtsStr, "����-����");
            }
            else if (totalDebts == 0)
            {
                str10 = String.Format(@"��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;����������� ������������� �� ������� ���������� �����. 
������������� � �����&nbsp;<span class='DigitsValue'><b>{2:N3}</b></span>&nbsp;���.���. ���� �������� �� ������ �&nbsp;<span class='DigitsValue'><b>{3}</b></span>&nbsp;��&nbsp;<span class='DigitsValue'><b>{0}</b></span>.<br/>",
dateTimeDebtsStr, "����-����", totalLastWeekDebts, dateLastTimeDebtsStr);
            }
            else
            {
                str10 = String.Format(@"��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;������������� �� ������� ���������� ����� ���������� 
&nbsp;<span class='DigitsValue'><b>{1:N3}</b></span>&nbsp;���.������ (<span class='DigitsValue'><b>{2:N0}</b></span>&nbsp;���.).<br/>�� ������ �&nbsp;<span class='DigitsValue'><b>{5}</b></span>&nbsp;��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;������������� {3}",
dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, "����-����", dateLastTimeDebtsStr);
            }

            string debtsMoList = String.Empty;

            query = DataProvider.GetQueryText("STAT_0001_0010_debts_mo_date");
            DataTable dtDebtsDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDebtsDateSakhalin);

            if (dtDebtsDateSakhalin.Rows.Count > 0)
            {
                debtsPeriodCurrentDate.Value = dtDebtsDateSakhalin.Rows[1][1].ToString();
                query = DataProvider.GetQueryText("STAT_0001_0010_debts_mo");
                DataTable dtDebtsMo = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDebtsMo);
                if (dtDebtsMo.Rows.Count > 0)
                {
                    debtsMoList =
                        "������������� �� ������� ���������� ����� ������������ � ��������� ������������� ������������: ";
                    foreach (DataRow row in dtDebtsMo.Rows)
                    {
                        debtsMoList = String.Format("{0} {1} ({2:N2} ���.���. {3:N0} ���.),", debtsMoList, row["������� ������������ "], row["�������������"], row["���-�� �������, ����� �������� ������� ������������� "]);
                    }
                    debtsMoList = debtsMoList.Trim(',');
                }
            }

            lbDebtDescription.Text = String.Format("{0} {1}", str10, debtsMoList);
        }

        #endregion

        #region �����

        public static Dictionary<string, string> dictRegionMapCodes;

        public void FillRegionMapCodes()
        {
            dictRegionMapCodes = new Dictionary<string, string>();
            string query = DataProvider.GetQueryText("STAT_0001_0010_region_map_codes");
            DataTable dtRMC = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��", dtRMC);
            foreach (DataRow row in dtRMC.Rows)
            {
                dictRegionMapCodes.Add(TrimRegionName(row["��"].ToString()), row["��� ������� �����"].ToString());
            }
        }

        public string TrimRegionName(string regionName)
        {
            return regionName.Replace(" �����", String.Empty).Replace("��������� ����� ", String.Empty).Replace(" ��������� �����", String.Empty).
                Replace("\"", String.Empty).Replace("������� ", String.Empty).Replace("����� ", String.Empty).Replace("�.", String.Empty).Replace(" - ", "-").Trim();
        }

        public void SetMapSettings(MapControl dundasMap, string legendTitle, string format)
        {
            mapCalloutOffsetY = 0.70;

            dundasMap.Width = 800;
            dundasMap.Height = 500;
            dundasMap.Shapes.Clear();

            string mapFolderName = "�������";

            dundasMap.ShapeFields.Add("Name");
            dundasMap.ShapeFields["Name"].Type = typeof(string);
            dundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            dundasMap.ShapeFields.Add("Complete");
            dundasMap.ShapeFields["Complete"].Type = typeof(double);
            dundasMap.ShapeFields["Complete"].UniqueIdentifier = false;

            AddMapLayer(dundasMap, mapFolderName, "��������", CRHelper.MapShapeType.Areas);
            //AddMapLayer(dundasMap, mapFolderName, "������_�������", CRHelper.MapShapeType.CalloutTowns);

            dundasMap.Meridians.Visible = false;
            dundasMap.Parallels.Visible = false;
            dundasMap.ZoomPanel.Visible = false;
            dundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            dundasMap.NavigationPanel.Visible = false;
            dundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            dundasMap.Viewport.EnablePanning = true;
            dundasMap.Viewport.OptimizeForPanning = false;
            dundasMap.Viewport.BackColor = Color.Black;

            // ��������� �������
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = true;
            legend.Dock = PanelDockStyle.Right;
            legend.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend.BackSecondaryColor = Color.Black;
            legend.BackGradientType = GradientType.TopBottom;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Black;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.FromArgb(192, 192, 192);
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.TitleColor = Color.White;
            legend.Title = legendTitle;
            legend.AutoFitMinFontSize = 7;
            dundasMap.Legends.Clear();
            dundasMap.Legends.Add(legend);

            Legend regionsLegend = new Legend("RegionCodeLegend");
            regionsLegend.Visible = true;
            regionsLegend.Dock = PanelDockStyle.Right;
            regionsLegend.BackColor = Color.FromArgb(75, 255, 255, 255);
            regionsLegend.BackSecondaryColor = Color.Black;
            regionsLegend.BackGradientType = GradientType.TopBottom;
            regionsLegend.BackHatchStyle = MapHatchStyle.None;
            regionsLegend.BorderColor = Color.Black;
            regionsLegend.BorderWidth = 1;
            regionsLegend.BorderStyle = MapDashStyle.Solid;
            regionsLegend.BackShadowOffset = 4;
            regionsLegend.TextColor = Color.FromArgb(192, 192, 192);
            regionsLegend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            regionsLegend.AutoFitText = true;
            regionsLegend.TitleColor = Color.White;
            regionsLegend.AutoFitMinFontSize = 7;
            regionsLegend.Title = "���� ����������";
            regionsLegend.ItemColumnSpacing = 100;
            dundasMap.Legends.Add(regionsLegend);

            // ��������� ������� ���������
            dundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Orange;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = format;
            dundasMap.ShapeRules.Add(rule);
        }

        // ����� �� ��������� ������ �������-������� �����
        private double mapCalloutOffsetY;

        /// <summary>
        /// �������� �� ����� �������-��������
        /// </summary>
        /// <param name="shape">�����</param>
        /// <returns>true, ���� ��������</returns>
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
        }

        /// <summary>
        /// ��������� ����� ����� (� ���������� ����� �� ������-�������)
        /// </summary>
        /// <param name="shape">�����</param>
        /// <returns>��� �����</returns>
        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name.Split('_')[0];
            }

            //shapeName = shapeName.Replace("������������� �����", "��");

            return shapeName;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(String.Format("../../../maps/��������/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "NAME", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        /// <summary>
        /// ����� ����� �����
        /// </summary>
        /// <param name="map">�����</param>
        /// <param name="patternValue">������� ��� �����</param>
        /// <returns>��������� �����</returns>
        public static ArrayList FindMapShape(MapControl map, string patternValue)
        {
            patternValue = patternValue.Replace("����� ����-���������", "�. ���� - ���������");
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (GetShapeName(shape) == patternValue)
                {
                    shapeList.Add(shape);
                }
            }

            return shapeList;
        }

        public void FillMapData(MapControl dundasMap, string queryName, int dataColumnIndex, string format)
        {
            DataTable dtMap = new DataTable();

            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "������������ ����������", dtMap);

            int[] codes = new int[0];
            LegendItem[] items = new LegendItem[0];
            foreach (DataRow row in dtMap.Rows)
            {
                // ��������� ����� �������
                if (row[0] != DBNull.Value && row[0].ToString() != String.Empty &&
                    row[dataColumnIndex] != DBNull.Value && row[dataColumnIndex].ToString() != String.Empty)
                {
                    string subject = row[0].ToString();
                    double value = Convert.ToDouble(row[dataColumnIndex]);
                    ArrayList shapeList = FindMapShape(dundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        string shapeCode = String.Empty;

                        if (dictRegionMapCodes.TryGetValue(TrimRegionName(shapeName), out shapeCode))
                        {
                            LegendItem item = new LegendItem();
                            LegendCell cell = new LegendCell(shapeCode);
                            cell.Alignment = System.Drawing.ContentAlignment.MiddleRight;
                            item.Cells.Add(cell);
                            cell = new LegendCell(String.Format(format, shapeName.Replace("��������� �����", "��").Replace("��������� �����", "��"), value));
                            cell.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
                            item.Cells.Add(cell);

                            Array.Resize(ref codes, codes.Length + 1);
                            Array.Resize(ref items, items.Length + 1);
                            codes[codes.Length - 1] = Convert.ToInt32(shapeCode);
                            items[items.Length - 1] = item;
                            //dundasMap.Legends["RegionCodeLegend"].Items.Add(item);

                            shape["Name"] = subject;
                            shape["Complete"] = Convert.ToDouble(row[dataColumnIndex]);

                            shape.Text = shapeCode;
                            shape.Font = new Font(shape.Font.Name, 10);
                            shape.TextColor = Color.White;
                            shape.TextVisibility = TextVisibility.Shown;

                        }
                    }
                }
            }
            Array.Sort(codes, items);
            foreach (LegendItem item in items)
                dundasMap.Legends["RegionCodeLegend"].Items.Add(item);
        }

        #endregion

        #region ���������

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0010_chart4");
            dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = String.Format("{0:01.MM.yy}", dateTime.AddMonths(1));
                    }
                }

                UltraChart4.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart4.Series.Add(series);
                }

                //UltraChart.DataSource = dtChart;
            }
        }

        void UltraChart4_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            string unit = " ���.���.";
                            point.DataPoint.Label = String.Format("{2}\n�� {3}\n {0:N2}{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

                        }
                    }
                }
            }
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            string currentYear = String.Empty;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 15;
                    }
                }
            }
        }

        #endregion

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

    }
}

