using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Orientation = Dundas.Maps.WebControl.Orientation;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0003_0001
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtMap;
        private int firstYear = 2007;
        private int endYear = 2010;
        private DateTime date;
        private string debtKindStr;
        private string shortDebtKindStr;
        private string populationDate;

        #endregion

        public bool FederalDebtSelected
        {
            get { return DebtKindButtonList.SelectedIndex == 0; }
        }

        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1000; }
        }

        private static int MinScreenWidth
        {
            get { return IsSmallResolution ? 780 : CustomReportConst.minScreenWidth; }
        }

        private static int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        #region ��������� �������

        // ��������� ����������� �����
        private CustomParam selectedFO;

        // ��������� ��� �����
        private CustomParam debtKind;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(MinScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(MinScreenHeight * 0.6 - 50);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            DundasMap.Width = CRHelper.GetChartWidth(MinScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.85);

            #region ������������� ���������� �������

            if (selectedFO == null)
            {
                selectedFO = UserParams.CustomParam("selected_fo");
            }

            if (debtKind == null)
            {
                debtKind = UserParams.CustomParam("debt_kind");
            }

            #endregion

            #region ��������� �����

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Left;
            //DundasMap.ZoomPanel.Orientation = Orientation.Horizontal;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Left;
            DundasMap.Viewport.EnablePanning = true;

            // ��������� �������
            DundasMap.Legends.Clear();
            // ��������� ������� ���������
            Legend legend = new Legend("DebtLegend");
            legend.Visible = true;
            legend.Dock = PanelDockStyle.Right;
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
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);

            // ��������� ���� ��� ���������
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("DebtValue");
            DundasMap.ShapeFields["DebtValue"].Type = typeof(double);
            DundasMap.ShapeFields["DebtValue"].UniqueIdentifier = false;

            // ��������� ������� ���������
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "DebtRule";
            rule.Category = String.Empty;
            rule.ShapeField = "DebtValue";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "DebtLegend";
            rule.LegendText = "#FROMVALUE{N2} - #TOVALUE{N2}";
            DundasMap.ShapeRules.Add(rule);

            #endregion

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);

            UltraGridExporter1.MultiHeader = true;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0003_0001_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "date", dtDate);
                date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
                endYear = date.Year;

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                String month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(date.Month));
                ComboMonth.Set�heckedState(month, true);

                ComboFO.Title = "��";
                ComboFO.Width = IsSmallResolution ? 240 : 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.Set�heckedState("��� ����������� ������", true);

                if (!string.IsNullOrEmpty(UserParams.Region.Value))
                {
                    ComboFO.Set�heckedState(UserParams.Region.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    string foName = RegionSettings.Instance.Id.ToLower() == "urfo"
                                         ? "��������� ����������� �����"
                                         : RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name);
                    ComboFO.Set�heckedState(foName, true);
                }
            }

            DebtKindButtonList.RepeatDirection = IsSmallResolution
                                                     ? RepeatDirection.Vertical
                                                     : RepeatDirection.Horizontal;

            DebtKindButtonList.Width = IsSmallResolution
                                         ? 180
                                         : 380;

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            debtKind.Value = FederalDebtSelected ? "���.����" : "�����.����";
            debtKindStr = FederalDebtSelected ? "���������������� ����� ���������" : "�������������� �����";
            shortDebtKindStr = FederalDebtSelected ? "���.����� ���������" : "�����.�����";

            if (DundasMap.Legends.Count != 0)
            {
                DundasMap.Legends[0].Title = string.Format("����� {0}\n�� ���� ���������, ���.���.", IsSmallResolution ? shortDebtKindStr : debtKindStr);
            }

            if (RFSelected)
            {
                selectedFO.Value = " ";
            }
            else
            {
                selectedFO.Value = string.Format(".[{0}]", ComboFO.SelectedValue);
            }

            string shortSelectedFOName = RFSelected ? "��" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue);

            int nextMonth = ComboMonth.SelectedIndex + 2;
            int nextYear = year;
            if (nextMonth == 13)
            {
                nextMonth = 1;
                nextYear++;
            }

            Label1.Text = string.Format("{0} ({1})", FederalDebtSelected ? "��������������� ���� ���������" : "������������ ����", shortSelectedFOName);
            Page.Title = Label1.Text;
            Label2.Text = FederalDebtSelected
                ? string.Format("����� ��������������� �������� ������������ �� ��������� �� 1 {0} {1} ����, ����� {2} �� ���� ���������", CRHelper.RusMonthGenitive(nextMonth), nextYear, debtKindStr)
                : string.Format("����� �������� ������������ ������������� ����������� �� ��������� �� 1 {0} {1} ����, ����� �������������� ����� �� ���� ���������", CRHelper.RusMonthGenitive(nextMonth), nextYear);
            mapElementCaption.Text = string.Format("����� {0} �� ���� ���������", debtKindStr);

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            // ��������� ����� �������
            string regionStr = shortSelectedFOName;

            DundasMap.Shapes.Clear();
            DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // ��������� ����� �������
            FillMapData(DundasMap);
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0003_0001_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(IsSmallResolution ? 240 : 300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            bool hideColumnForSmallResolution = IsSmallResolution;

            SetColumnParams(e.Layout, 0, 1, "", 60, hideColumnForSmallResolution);
            SetColumnParams(e.Layout, 0, 2, "N2", IsSmallResolution ? 100 : 140, false);
            SetColumnParams(e.Layout, 0, 3, "N2", IsSmallResolution ? 100 : 140, false);
            SetColumnParams(e.Layout, 0, 4, "N0", 130, hideColumnForSmallResolution);
            SetColumnParams(e.Layout, 0, 5, "N2", IsSmallResolution ? 80 : 150, false);
            SetColumnParams(e.Layout, 0, 6, IsSmallResolution ? "P0" : "P2", IsSmallResolution ? 60 : 80, false);
            SetColumnParams(e.Layout, 0, 7, "N0", IsSmallResolution ? 50 : 55, false);
            SetColumnParams(e.Layout, 0, 8, "N0", IsSmallResolution ? 50 : 55, false);
            SetColumnParams(e.Layout, 0, 9, "N0", 60, true);
            SetColumnParams(e.Layout, 0, 10, "N0", 60, true);

            e.Layout.Bands[0].Columns[7].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[9].CellStyle.Wrap = true;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "��", "����������� �����");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, string.Format("����� {0}, ���.���.", IsSmallResolution ? shortDebtKindStr : debtKindStr),
                string.Format("����� {0}", debtKindStr));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, string.Format("����� {0}, ������� ���, ���.���.", IsSmallResolution ? shortDebtKindStr : debtKindStr),
                string.Format("����� {0}, ������� ���", debtKindStr));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4,
                string.Format("����������� ����������� ��������� {0}, ���.", populationDate), "����������� ����������� ���������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, string.Format("����� {0} � ������� �� ���� ���������, ���.���./���.", IsSmallResolution ? shortDebtKindStr : debtKindStr),
                string.Format("��������� ������ {0} � ����������� ����������� ���������", debtKindStr));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "���� �����", "���� ����� ������ �������� ������������ � ������������ ������� �������� ����");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "���� ��",
                string.Format("���� (�����) �� ������ {0} �� ���� ��������� ����� ��������� ��� ������������ ������", debtKindStr));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8,
                "���� ��", string.Format("���� (�����) �� ������ {0} �� ���� ��������� ����� ���� ���������", debtKindStr));
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (!RFSelected)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == 7 || i == 8);
                bool rate = (i == 6);

                bool debt = (i == 2 || i == 3);
                int rankFOIndex = 7;
                int rankRFIndex = 8;

                string zeroValueText = "��� �����";
                string nullValueText = "��� ����������";

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 2].Value != null &&
                        e.Row.Cells[i].Value.ToString() != zeroValueText && e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 2].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("C���� ��������� ����� ����� �� ���� ��������� �� {0}", i == 6 ? "��" : "��");
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 2].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("C���� ������� ����� ����� �� ���� ��������� �� {0}", i == 6 ? "��" : "��");
                            
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                string subjectName = string.Empty;
                if (e.Row.Cells[0].Value != null)
                {
                    subjectName = e.Row.Cells[0].Value.ToString();
                }

                if (debt)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (i == 2)
                        {
                            double debtValue = Convert.ToDouble(e.Row.Cells[i].Value);

                            if (RegionsNamingHelper.IsSubject(subjectName) && debtValue == 0)
                            {
                                e.Row.Cells[rankFOIndex].Value = zeroValueText;
                                e.Row.Cells[rankFOIndex].Style.HorizontalAlign = HorizontalAlign.Center;
                                e.Row.Cells[rankRFIndex].Value = zeroValueText;
                                e.Row.Cells[rankRFIndex].Style.HorizontalAlign = HorizontalAlign.Center;
                            }
                        }
                    }
                    else if (RegionsNamingHelper.IsSubject(subjectName))
                    {
                        e.Row.Cells[i].Value = nullValueText;
                    }
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "������� � �������� ����";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "�������� � �������� ����";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                    {
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
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

        #region ����������� �����

        /// <summary>
        /// ����� ����� �����
        /// </summary>
        /// <param name="map">�����</param>
        /// <param name="patternValue">������� ��� �����</param>
        /// <returns>��������� �����</returns>
        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            string subject = patternValue.Replace("�������", "���.");
            subject = subject.Replace("���������� �����", "��");
            subject = subject.Replace("����������� �����", "��");
            bool isRepublic = patternValue.Contains("����������");
            bool isTown = patternValue.Contains("�.");

            string[] subjects = patternValue.Split(' ');
            // ���� ����� ������ ������ ������������� ���� ���������
            if (subjects.Length > 1)
            {
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
                            subject = (isRepublic || isTown) ? subjects[1] : subjects[0];
                            break;
                        }
                }
            }

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("MFRF_0003_0001_map");

            dtMap = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�������", dtMap);

            bool nullValueExists = false;

            foreach (DataRow row in dtMap.Rows)
            {
                // ��������� ����� �������
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();
                    string levelName = row[2].ToString();

                    bool useSubject = (RFSelected && levelName == "����������� �����" ||
                                       !RFSelected && levelName == "������� ��");

                    if (useSubject)
                    {
                        Shape shape = FindMapShape(map, regionName);
                        if (shape != null)
                        {
                            if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                            {
                                double debtValue = Convert.ToDouble(row[1]);

                                if (debtValue == 0)
                                {
                                    shape.Color = Color.LightSkyBlue;
                                    shape.ToolTip = string.Format("{0}\n{1}", shape.Name,
                                        FederalDebtSelected ? "��������������� ���� �����������" : "�������������� ���� �����������");
                                    shape.Text = shape.Name;
                                    shape.TextVisibility = TextVisibility.Shown;

                                    if (!nullValueExists && DundasMap.Legends.Count > 0)
                                    {
                                        nullValueExists = true;

                                        LegendItem item = new LegendItem();
                                        item.Text = "���� �����������";
                                        item.Color = Color.LightSkyBlue;
                                        DundasMap.Legends[0].Items.Add(item);
                                    }
                                }
                                else
                                {
                                    string debtStr = FederalDebtSelected
                                                         ? "���������������� ����� ��������"
                                                         : "�������������� �����";
  
                                    shape["Name"] = regionName;
                                    shape["DebtValue"] = debtValue;
                                    shape.ToolTip =
                                        string.Format("#NAME \n������������� ����� {1}: {0:N2} ���.���./���.", debtValue,
                                                      debtStr);
                                    if (regionName == "��������������� �������")
                                    {
                                        shape.TextAlignment = ContentAlignment.MiddleRight;
                                    }
                                    else
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                    shape.Offset.X = -15;

                                    shape.Text = string.Format("{0}\n{1:N2}", shape.Name, debtValue);

                                    shape.BorderWidth = 2;
                                    shape.TextColor = Color.Black;
                                    shape.TextVisibility = TextVisibility.Shown;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region ������� � Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            //    e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            //    e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            e.CurrentWorksheet.Columns[1].Width = 50 * 37;
            e.CurrentWorksheet.Columns[2].Width = 130 * 37;
            e.CurrentWorksheet.Columns[3].Width = 130 * 37;
            e.CurrentWorksheet.Columns[4].Width = 130 * 37;
            e.CurrentWorksheet.Columns[5].Width = 130 * 37;
            e.CurrentWorksheet.Columns[6].Width = 110 * 37;
            e.CurrentWorksheet.Columns[7].Width = 60 * 37;
            e.CurrentWorksheet.Columns[8].Width = 60 * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "0";

            // ����������� ����� � ����� ������
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 23 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 3;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            Worksheet sheet2 = workbook.Worksheets.Add("�����");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            sheet1.Rows[0].Cells[0].Value = Label1.Text;
            sheet1.Rows[1].Cells[0].Value = Label2.Text;

            sheet2.Rows[0].Cells[0].Value = Label1.Text;
            sheet2.Rows[1].Cells[0].Value = Label2.Text;
            sheet2.Rows[2].Cells[0].Value = mapElementCaption.Text;
            UltraGridExporter.MapExcelExport(sheet2.Rows[3].Cells[0], DundasMap);

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            UltraGridExporter1.GridElementCaption = Label2.Text;
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(string.Format("{0}", mapElementCaption.Text));

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion

    }
}
