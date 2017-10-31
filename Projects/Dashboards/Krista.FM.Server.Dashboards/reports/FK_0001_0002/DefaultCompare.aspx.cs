using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Text;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0002
{
    public partial class DefaultCompare : CustomReportPage
    {
        #region ����

        private DataTable mapDt = new DataTable();
        private DataTable gridDt;
        private int firstYear = 2000;
        private string populationDate;

        private DateTime currentDate;

        private const string cookieStateAreaParamName = "StateArea";

        #endregion

        #region ��������

        /// <summary>
        /// ������� �� ��� ����������� ������
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����

            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 225);
            GridBrick.Width = CustomReportConst.minScreenWidth - 12;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;

            #endregion

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            CrossLink1.Text = "��������� ������������� ��������� �� ������������� �������";
            CrossLink1.NavigateUrl = "~/reports/FK_0001_0002/DefaultAllocation.aspx";
            CrossLink2.Text = "������&nbsp;��������&nbsp;��&nbsp;���������";
            CrossLink2.NavigateUrl = "~/reports/FK_0001_0004/DefaultDetail.aspx";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            GridSearch1.LinkedGridId = GridBrick.Grid.ClientID;
        }

        private void SetupMap()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;

            // ��������� �������
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = "������� ����������";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // ��������� ����
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Complete");
            DundasMap.ShapeFields["Complete"].Type = typeof(double);
            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
            DundasMap.ShapeFields.Add("CompletePercent");
            DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
            DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

            // ��������� ������� ���������
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText =  "#FROMVALUE{N2} - #TOVALUE{N2}";
            DundasMap.ShapeRules.Add(rule);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                currentDate = CubeInfoHelper.FkMonthReportIncomesInfo.LastDate;

                UserParams.PeriodYear.Value = currentDate.Year.ToString();
                UserParams.PeriodMonth.Value = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month));
              //  UserParams.Filter.Value = "��� ����������� ������";
                UserParams.KDGroup.Value = "������ ����� ";

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, currentDate.Year));
                ComboYear.Set�heckedState(currentDate.Year.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(UserParams.PeriodMonth.Value, true);

                ComboFO.Title = "��";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
             //   ComboFO.Set�heckedState(UserParams.Filter.Value, true);

                ComboKD.Width = 420;
                ComboKD.Title = "��� ������";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullKDIncludingList());
                ComboKD.RenameTreeNodeByName("������ ������� - ����� ", "������ ����� ");
                ComboKD.Set�heckedState(UserParams.KDGroup.Value, true);

                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboFO.Set�heckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                }
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            Page.Title = String.Format("���������� ������� ({0})", ComboFO.SelectedIndex == 0
                                                                       ? "��"
                                                                       : RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;
            Label2.Text = String.Format(
                    "���������� ����������������� �������� ��������� �� �� ������� ({3}) �� {0} {1} {2} ����", currentDate.Month,
                    CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year, ComboKD.SelectedValue);

            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.KDGroup.Value = ComboKD.SelectedValue;

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            GridDataBind();

            // ��������� ����� �������
            string regionStr = (ComboFO.SelectedIndex == 0) ? "���������� ���������" : ComboFO.SelectedValue;
            DundasMap.Shapes.Clear();

            DundasMap.LoadFromShapeFile(Server.MapPath(String.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);

//            DundasMap.Serializer.Format = SerializationFormat.Binary;
//            DundasMap.Serializer.Load((Server.MapPath(String.Format("../../maps/{0}/{0}.bin", RegionsNamingHelper.ShortName(regionStr)))));

            SetupMap();
            // ��������� ����� �������
            FillMapData();
        }

        #region ����������� �����

        /// <summary>
        /// ����� ����� �����
        /// </summary>
        /// <param name="map">�����</param>
        /// <param name="patternValue">������� ��� �����</param>
        /// <param name="searchFO">true, ���� ���� ��</param>
        /// <returns>��������� �����</returns>
        public static Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string subject = String.Empty;
            bool isRepublic = patternValue.Contains("����������");
            bool isTown = patternValue.Contains("�.");
            string[] subjects = patternValue.Split(' ');
            if (subjects.Length > 1)
            {
                // ���� ����� ������ ������ ������������� ���� ���������
                switch (subjects[0])
                {
                    case "���������":
                        {
                            subject = "�����";
                            break;
                        }
                    case "���������-����������":
                        {
                            subject = "���������-���������";
                            break;
                        }
                    case "���������-����������":
                        {
                            subject = "���������-��������";
                            break;
                        }
                    case "����������":
                        {
                            subject = "��������";
                            break;
                        }
                    case "���������":
                        {
                            subject = "�������";
                            break;
                        }
                    default:
                        {
                            subject = (isRepublic || isTown ) ? subjects[1] : subjects[0];
                            break;
                        }
                }
            }
           /* foreach (Shape name in map.Shapes)
            {
                CRHelper.SaveToErrorLog(name.Name);
            }*/
            
            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData()
        {
            string query = DataProvider.GetQueryText("FK_0001_0002_compare_map");
            mapDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�������", mapDt);

            const string completePercentColumnName = "������� ���������� ����������";

            foreach (DataRow row in mapDt.Rows)
            {
                // ��������� ����� �������
                if (row[0] != DBNull.Value && row[0].ToString() != String.Empty &&
                    row[completePercentColumnName] != DBNull.Value && row[completePercentColumnName].ToString() != String.Empty)
                {
                    string subject = row[0].ToString();
                    if (AllFO && RegionsNamingHelper.IsFO(subject) || !AllFO && RegionsNamingHelper.IsSubject(subject))
                    {
                        Shape shape = FindMapShape(DundasMap, subject, AllFO);
                        if (shape != null)
                        {
                            double value = 100*Convert.ToDouble(row[completePercentColumnName]);

                            if (subject.Contains("�������"))
                            {
                                subject = "��������� ����������";
                            }

                            shape["Name"] = subject;
                            shape["CompletePercent"] = value;
                            shape.ToolTip = "#NAME\n#COMPLETEPERCENT{N2}%";
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.Text = String.Format("{0}\n{1:N2}%", subject.Replace(" ", "\n"), value);

                            if (subject.Contains("�������"))
                            {
                                shape.CentralPointOffset.Y = 90000;
                            }

                            if (subject.Contains("����������"))
                            {
                                shape.CentralPointOffset.Y = -90000;
                            }

                            if (subject.Contains("���������"))
                            {
                                shape.TextAlignment = ContentAlignment.MiddleRight;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region ����������� �����

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0001_0002_compare_Grid");
            gridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�������", gridDt);
            
            if (gridDt.Rows.Count > 0)
            {
                foreach (DataRow row in gridDt.Rows)
                {
                    if (row[2] != DBNull.Value)
                    {
                        row[2] = Convert.ToDouble(row[2]) / 1000000;
                    }
                    if (row[3] != DBNull.Value)
                    {
                        row[3] = Convert.ToDouble(row[3]) / 1000000;
                    }
                }

                UserParams.Filter.Value = ComboFO.SelectedValue;
                if (ComboFO.SelectedIndex != 0)
                {
                    GridBrick.DataTable = CRHelper.SetDataTableFilter(gridDt, "��", RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
                }
                else
                {
                   GridBrick.DataTable = gridDt;
                }
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 3].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 4].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 5].Hidden = true;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("�������");
            headerLayout.AddCell("��", "����������� �����, �������� ����������� �������");
            headerLayout.AddCell("���������, ���.���", "����������� ���������� ����������� ������ � ������ ����");
            headerLayout.AddCell("���������, ���.���", "���� �� ���");
            headerLayout.AddCell("��������� %", "������� ���������� ����������. ������ ������������� ���������� (1/12 �������� ����� � �����)");
            headerLayout.AddCell("���� % ��", "���� (�����) �������� �� �������� ���������� ���������� ����� ��������� ��� ������������ ������");
            headerLayout.AddCell("���� % ��", "���� (�����) �� �������� ���������� ���������� ����� ���� ���������");
            headerLayout.AddCell(String.Format("����������� ����������� ��������� {0}, ���.���.", populationDate), "����������� ����������� ���������");
            headerLayout.AddCell("��������� ������ �� ���� ���������, ���./���.", "����� ������� ���������� ���� �� ���� ���������");
            headerLayout.AddCell("���� ���������. ��", "���� (�����) �������� �� ������������� ������� ����� ��������� ��� ������������ ������");
            headerLayout.AddCell("���� ���������. ��", "���� (�����) �� ������������� ������� ����� ���� ���������");
            headerLayout.AddCell("���� ��", "���� ������� �������� � ����� ����� ������� ������������ ������");
            headerLayout.AddCell("���� ��", "���� ������� �������� (������) � ����� ����� ������� ���� ��������� ��");
            
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(235);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(43);
            
            for (int i = 2; i < columnCount - 5; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(GetColumnWidth(columnName));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            #region ��������� �����������

            FontRowLevelRule levelRule = new FontRowLevelRule(columnCount - 1);
            levelRule.AddFontLevel("0", GridBrick.BoldFont8pt);
            GridBrick.AddIndicatorRule(levelRule);

            PerformanceUniformityRule uniformityRule = new PerformanceUniformityRule("��������� %", currentDate.Month);
            GridBrick.AddIndicatorRule(uniformityRule);

            RankIndicatorRule percentFORankRule = new RankIndicatorRule("���� % ��", "������ ���� ������� ��");
            percentFORankRule.BestRankHint = "����� ������� ������� ���������� � ����������� ������";
            percentFORankRule.WorseRankHint = "����� ������ ������� ���������� � ����������� ������";
            GridBrick.AddIndicatorRule(percentFORankRule);

            RankIndicatorRule percentRFRankRule = new RankIndicatorRule("���� % ��", "������ ���� ������� ��");
            percentRFRankRule.BestRankHint = "����� ������� ������� ���������� � ��";
            percentRFRankRule.WorseRankHint = "����� ������ ������� ���������� � ��";
            GridBrick.AddIndicatorRule(percentRFRankRule);

            RankIndicatorRule avgFORankRule = new RankIndicatorRule("���� ���������. ��", "������ ���� ��������� ��");
            avgFORankRule.BestRankHint = "����� ������� ����� �� ���� ��������� � ����������� ������";
            avgFORankRule.WorseRankHint = "����� ������ ����� �� ���� ��������� � ����������� ������";
            GridBrick.AddIndicatorRule(avgFORankRule);

            RankIndicatorRule avgRFRankRule = new RankIndicatorRule("���� ���������. ��", "������ ���� ��������� ��");
            avgRFRankRule.BestRankHint = "����� ������� ����� �� ���� ��������� � ��";
            avgRFRankRule.WorseRankHint = "����� ������ ����� �� ���� ��������� � ��";
            GridBrick.AddIndicatorRule(avgRFRankRule);

            #endregion
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("����"))
            {
                return "N0";
            }
            if (columnName.ToLower().Contains("����") || columnName.Contains("%"))
            {
                return "P2";
            }
            return "N3";
        }

        private static int GetColumnWidth(string columnName)
        {
            if (columnName.ToLower().Contains("����"))
            {
                return 66;
            }
            if (columnName.ToLower().Contains("��������� %"))
            {
                return 80;
            }
            if (columnName.ToLower().Contains("����") || columnName.Contains("%"))
            {
                return 60;
            }
            return 97;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        #endregion

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("�����");
            ReportExcelExporter1.Export(DundasMap, String.Format("������� ���������� ����� �� ��������� ��������� ({0})", ComboKD.SelectedValue.TrimEnd(' ')), sheet2, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;
            Report report = new Report();

            ISection section1 = report.AddSection();



            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, Label2.Text, section1);

            ISection section2 = report.AddSection();
            DundasMap.Width = Unit.Pixel(Convert.ToInt32(DundasMap.Width.Value * 0.85));
            IText title = section2.AddText();
            Font font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label2.Text);
            ReportPDFExporter1.Export(DundasMap,  section2);
        }

        #endregion
    }
}
