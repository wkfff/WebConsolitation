using System;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0007_NAO
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2003;
        private int endYear = 2011;
        private bool blackStyle;

        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;

        private bool onWall;

        #region Параметры запроса

        private CustomParam yearsComboValue;
        private CustomParam subjectComboValue;
        private CustomParam levelComboValue;
        private CustomParam kdComboValue;
        private CustomParam stackMode;
        private CustomParam yearDescenants;

        #endregion

        private bool UseStackMode
        {
            get { return useStack.Checked; }
        }

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);

            blackStyle = false;
            if (Session["blackStyle"] != null)
            {
                blackStyle = Convert.ToBoolean(((CustomParam)Session["blackStyle"]).Value);
            }

            string regionTheme = RegionSettings.Instance.Id;
            CRHelper.SetPageTheme(this, blackStyle ? regionTheme + "BlackStyle" : regionTheme);
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            Session["blackStyle"] = null;

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

            SetScaleSize();

            #region Инициализация параметров запроса

            yearsComboValue = UserParams.CustomParam("years_combo_value", true);
            kdComboValue = UserParams.CustomParam("kd_combo_value", true);
            subjectComboValue = UserParams.CustomParam("subject_combo_value", true);
            levelComboValue = UserParams.CustomParam("level_combo_value", true);
            stackMode = UserParams.CustomParam("stack_mode", true);
            yearDescenants = UserParams.CustomParam("year_descendants");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.SplineChart;
            UltraChart.BorderWidth = 0;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 6;
            UltraChart.Legend.Margins.Bottom = UltraChart.Axis.X.Extent;

            UltraChart.TitleLeft.Text = "Млн.руб.";
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Visible = true;
            
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <SERIES_LABEL>г.\n<DATA_VALUE:N3> млн.руб.";

            GradientEffect effect = new GradientEffect();
            effect.Coloring = GradientColoringStyle.Lighten;
            UltraChart.Effects.Add(effect);
            UltraChart.Effects.Enabled = true;

            UltraChart.SplineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion
            
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraChart.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            UltraChart.Width = pageWidth - 50;
            UltraChart.Height = pageHeight / 2;

            UltraWebGrid.Width = pageWidth - 50;
            UltraWebGrid.Height = pageHeight / 2;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0007_NAO_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                yearsComboValue.Value = String.Format(String.Format("{0},{1},{2}", endYear, endYear - 1, endYear - 2));

                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    subjectComboValue.Value = RegionSettings.Instance.Name;
                }
                else
                {
                    subjectComboValue.Value = "Амурская область";
                }
                
                kdComboValue.Value = "Доходы ВСЕГО ";
                levelComboValue.Value = "Консолидированный бюджет субъекта";
                stackMode.Value = "false";

                ComboYear.Width = 100;
                ComboYear.Title = "Годы";
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetMultipleСheckedState(yearsComboValue.Value, true);

                ComboKD.Width = 230;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillShortKDIncludingList());
                ComboKD.SetСheckedState(kdComboValue.Value, true);

                ComboSKIF.Width = 250;
                ComboSKIF.Title = "Уровень бюджета";
                ComboSKIF.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIF.ParentSelect = true;
                ComboSKIF.SetСheckedState(levelComboValue.Value, true);

                regionsCombo.Width = 250;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.ParentSelect = false;
                regionsCombo.SetСheckedState(subjectComboValue.Value, true);

                useStack.Checked = Convert.ToBoolean(stackMode.Value);
            }

            yearsComboValue.Value = ComboYear.SelectedValuesString;
            kdComboValue.Value = ComboKD.SelectedValue;
            subjectComboValue.Value = regionsCombo.SelectedValue;
            levelComboValue.Value = ComboSKIF.SelectedValue;
            stackMode.Value = UseStackMode.ToString();

            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;
            UserParams.SKIFLevel.Value = levelComboValue.Value;

            Page.Title = string.Format("Помесячная динамика поступления доходов");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;

            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {
                string kd = ComboKD.SelectedValue;
                if (kd != "НДПИ " && kd != "НДФЛ ")
                {
                    kd = CRHelper.ToLowerFirstSymbol(kd);
                }

                PageSubTitle.Text = string.Format("{0}, {1} за {2} {3} ({4}), млн.руб.",
                    UserParams.StateArea.Value, kd, CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                    ComboYear.SelectedValues.Count == 1 ? "год" : "годы", ComboSKIF.SelectedValue);

                string yearDescedants = string.Empty;
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];
                    yearDescedants += string.Format("Descendants ([Период].[Период].[Данные всех периодов].[{0}],[Период].[Период].[Месяц],SELF) + ", year);
                }
                yearDescedants = yearDescedants.Remove(yearDescedants.Length - 3, 2);
                yearDescenants.Value = string.Format("{1}{0}{2}", yearDescedants, '{', '}');
            }
            else
            {
                yearDescenants.Value = "{}";
            }

            UserParams.KDGroup.Value = kdComboValue.Value;
            UserParams.SelectItem.Value = (!UseStackMode) ? "Исполнено за период" : "Исполнено";

            UltraWebGrid.DataBind();
            UltraChart.DataBind();

            WallLink.Text = "Для&nbsp;видеостены";
            WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());

            BlackStyleWallLink.Text = "Для&nbsp;видеостены&nbsp;(черный&nbsp;стиль)";
            BlackStyleWallLink.NavigateUrl = String.Format("{0};onWall=true;blackStyle=true", UserParams.GetCurrentReportParamList());
        }

        private void SetScaleSize()
        {
            heightMultiplier = onWall ? 5 : 1;
            fontSizeMultiplier = onWall ? 5 : 1;
            primitiveSizeMultiplier = onWall ? 4 : 1;
            pageWidth = onWall ? 5600 : (int)Session["width_size"];
            pageHeight = onWall ? 2100 : (int)Session["height_size"];

            widthMultiplier = 1;
            if (Session["width_size"] != null && (int)Session["width_size"] != 0)
            {
                widthMultiplier = onWall ? 1.08 * 5600 / (int)Session["width_size"] : 1;
            }

            Font font7 = new Font("Verdana", Convert.ToInt32(7 * fontSizeMultiplier));
            Font font8 = new Font("Verdana", Convert.ToInt32(8 * fontSizeMultiplier));
            Color fontColor = blackStyle ? Color.White : Color.Black;

            UltraWebGrid.DisplayLayout.HeaderStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.BorderWidth = blackStyle ? 1 : onWall ? 3 : 1;

            UltraWebGrid.DisplayLayout.RowStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            UltraWebGrid.DisplayLayout.RowStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            UltraWebGrid.DisplayLayout.RowStyleDefault.BorderWidth = 1;

            //UltraChart.Axis.X.Labels.Font = font8;
            UltraChart.Axis.Y.Labels.Font = font8;
            UltraChart.TitleLeft.Font = font7;
            UltraChart.Legend.Font = font8;

            UltraChart.Axis.X.Labels.FontColor = fontColor;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Labels.FontColor = fontColor;
            UltraChart.TitleLeft.FontColor = fontColor;
            UltraChart.Legend.FontColor = fontColor;

            UltraChart.TitleLeft.Extent = Convert.ToInt32(35 * widthMultiplier);
            UltraChart.Axis.X.Extent = Convert.ToInt32(60 * heightMultiplier);
            UltraChart.Axis.Y.Extent = Convert.ToInt32(100 * widthMultiplier);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = primitiveSizeMultiplier > 2 ? SymbolIconSize.Large : SymbolIconSize.Medium;
            lineAppearance.LineStyle.MidPointAnchors = false;
            lineAppearance.Thickness = 3 * primitiveSizeMultiplier;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart.SplineChart.LineAppearances.Add(lineAppearance);

            UltraChart.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            UltraWebGrid.DisplayLayout.HeaderStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            UltraWebGrid.DisplayLayout.RowStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);

            PageTitle.Font.Size = Convert.ToInt32(14 * fontSizeMultiplier);
            PageSubTitle.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);
            
            GrowRateHintLabel.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            GrowRateHintLabel.ForeColor = fontColor;
            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            GrowRateHintLabel.Text = String.Format("Индикатор {0} / {1} - рост/снижение показателя относительно прошлого года", greenGradientBar, redGradientBar);

            if (onWall)
            {
                ComprehensiveDiv.Style.Add("width", "5600px");
                ComprehensiveDiv.Style.Add("height", "2100px");
                //ComprehensiveDiv.Style.Add("border", "medium solid #FF0000");
            }

            if (onWall && Page.Master is IMasterPage)
            {
                ((IMasterPage)Page.Master).SetHeaderVisible(false);
            }

            WallLink.Visible = !onWall;
            BlackStyleWallLink.Visible = !onWall;
            regionsCombo.Visible = !onWall;
            ComboKD.Visible = !onWall;
            ComboSKIF.Visible = !onWall;
            ComboYear.Visible = !onWall;
            useStack.Visible = !onWall;
            RefreshButton1.Visible = !onWall;
            PopupInformer1.Visible = !onWall;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0007_NAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            // заполняем сразу дататейбл для диаграммы
            dtChart = new DataTable();
            dtChart.Columns.Add("Год", typeof(string));

            DataTable dtGrid2 = new DataTable();
            dtGrid2.Columns.Add("Год", typeof(string));

            for (int i = 1; i <= 12; i++)
            {
                dtChart.Columns.Add(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), typeof(double));
                dtGrid2.Columns.Add(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Исполнено", typeof(double));
                dtGrid2.Columns.Add(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Темп роста", typeof(double));
            }

            List<int> inserts = new List<int>();

            DataRow chartRow = null;
            DataRow gridRow = null;
            for (int k = 0; k < dtGrid.Rows.Count; k++)
            {
                DataRow row = dtGrid.Rows[k];

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if ((i == 2) && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }

                if ((row[0] != DBNull.Value && (row[0].ToString() == "Январь" || k == 0)))
                {
                    inserts.Add(k + inserts.Count);

                    chartRow = dtChart.NewRow();
                    dtChart.Rows.Add(chartRow);
                    chartRow[0] = row[1].ToString();

                    gridRow = dtGrid2.NewRow();
                    dtGrid2.Rows.Add(gridRow);
                    gridRow[0] = row[1].ToString();
                }

                string columnName = row[0].ToString();
                if (chartRow != null && columnName != string.Empty)
                {
                    if (row[2] != DBNull.Value)
                    {
                        chartRow[columnName] = row[2].ToString();
                        gridRow[columnName + "; Исполнено"] = row[2].ToString();
                    }
                    if (row[3] != DBNull.Value)
                    {
                        gridRow[columnName + "; Темп роста"] = row[3].ToString();
                    }
                }
            }

            for (int i = 0; i < inserts.Count; i++)
            {
                DataRow r = dtGrid.NewRow();
                r[0] = dtGrid.Rows[inserts[i]].ItemArray[1].ToString();
                dtGrid.Rows.InsertAt(r, inserts[i]);
            }

            if (dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(1);
            }

            UltraWebGrid.DataSource = dtGrid2;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(40);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N1";
                int columnWidth = 44;

                if (i % 2 == 0)
                {
                    formatString = "P0";
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 0; i < columnCount; i = i + 1)
            {
                int columnWidth = Convert.ToInt32(e.Layout.Bands[0].Columns[i].Width.Value);
                e.Layout.Bands[0].Columns[i].Width = onWall ? Convert.ToInt32(columnWidth * widthMultiplier) : columnWidth;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Факт", string.Format("Фактические поступления {0}, млн.руб.", (useStack.Checked) ? "с начала года" : "за месяц"));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Темп роста", "Темп роста поступлений к аналогичному периоду предыдущего года");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i % 2 == 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 0;
                }
                else if (i % 2 != 0 && i != e.Row.Cells.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 0;
                }

                e.Row.Cells[i].Style.Padding.Left = 3;
                e.Row.Cells[i].Style.Padding.Right = 3;
                if ((i % 2 == 0 && i != 0) && e.Row.Cells[i].Value != null)
                {
                    string img = String.Empty;
                    int barHeight = onWall ? 60 : 17;
                    int barBottomMargin = onWall ? -barHeight - 5 : -barHeight;
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        img = "../../images/RedGradientBarInverse.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        img = "../../images/GreenGradientBarInverse.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }

                    string format = e.Row.Band.Grid.Columns[i].Format;
                    string value = Convert.ToDouble(e.Row.Cells[i].Value).ToString(format);
                    e.Row.Cells[i].Value = String.Format("<div style='position: relative; z-index: 1; margin-bottom: {3}px; margin-right: -5px; width:100%; height:{2}px'><img src=\"{1}\" width=\"100%\" height=\"{2}px\"></div><div style='position: relative; z-index: 2;'>{0}</div>",
                            value, img, barHeight, barBottomMargin);
                }

                UltraGridCell cell = e.Row.Cells[i];
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

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.Width = Unit.Empty;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            // данные получаются из dtGrid в обработчике UltraWebGrid_DataBinding
            UltraChart.DataSource = dtChart;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text) primitive;
                    axisText.bounds.Width = onWall ? 60 : 20;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new Font("Verdana", 8 * fontSizeMultiplier);
                    axisText.labelStyle.FontColor = blackStyle ? Color.White : Color.Black;
                    axisText.labelStyle.WrapText = false;

                    if (onWall)
                    {
                        axisText.bounds.Offset(-30, 0);
                    }
                }
            }
        }

        #endregion
    }
}