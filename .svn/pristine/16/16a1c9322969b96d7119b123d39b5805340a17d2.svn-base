using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0013_NAO
{ 
    public partial class Default : CustomReportPage
    {
        private const int firstYear = 2007;
        private GridHeaderLayout headerLayout;
        private DateTime currentDate;

        private double widthMultiplier;
        private double heightMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;
        private bool onWall;
        private bool blackStyle;

        private bool IsRFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        #region Параметры запроса

        private CustomParam yearComboValue;
        private CustomParam monthComboValue;
        private CustomParam foComboValue;
        private CustomParam levelComboValue;
        private CustomParam selectedFo;

        #endregion

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

            #region Инициализация параметров запроса

            yearComboValue = UserParams.CustomParam("year_combo_value", true);
            monthComboValue = UserParams.CustomParam("month_combo_value", true);
            foComboValue = UserParams.CustomParam("fo_combo_value", true);
            levelComboValue = UserParams.CustomParam("level_combo_value", true);
            selectedFo = UserParams.CustomParam("selected_fo");

            #endregion

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
                Session["onWall"] = null;
            }

            SetScaleSize();

            UltraWebGrid.Width = pageWidth - 50;
            UltraWebGrid.Height = pageHeight;
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                string query = DataProvider.GetQueryText("FK_0001_0013_NAO_date");
                DataTable dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                yearComboValue.Value = endYear.ToString();
                monthComboValue.Value = month;
                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    foComboValue.Value = RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name);
                }
                else
                {
                    foComboValue.Value = "Все федеральные округа";
                }
                
                levelComboValue.Value = "Консолидированный бюджет субъекта";
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(yearComboValue.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(monthComboValue.Value, true);

                ComboFO.Title = "Федеральный округ";
                ComboFO.Width = 410;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState(foComboValue.Value, true);

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState(levelComboValue.Value, true);
            }

            yearComboValue.Value = ComboYear.SelectedValue;
            monthComboValue.Value = ComboMonth.SelectedValue;
            foComboValue.Value = ComboFO.SelectedValue;
            levelComboValue.Value = ComboSKIFLevel.SelectedValue;

            Page.Title = "Отдельные показатели исполнения бюджетов"; 
            Label1.Text = String.Format("Отдельные показатели исполнения бюджетов субъектов {0}", ComboFO.SelectedIndex == 0 ? "РФ" :
                RegionsNamingHelper.ShortName(ComboFO.SelectedValue)); ;
            
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);

            Label2.Text = String.Format("{3} за {0} {1} {2} года, млн.руб.", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboSKIFLevel.SelectedValue);

            UserParams.PeriodMonth.Value = monthComboValue.Value;
            UserParams.PeriodYear.Value = yearComboValue.Value;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            selectedFo.Value = IsRFSelected ? ".Children" : String.Format(".[{0}]", foComboValue.Value);
            UserParams.SKIFLevel.Value = levelComboValue.Value;
            UserParams.KDTotal.Value = (currentDate.Year < 2003) ? "Доходы бюджета - ВСЕГО" : "Доходы - всего в том числе:";

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

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

            Color fontColor = blackStyle ? Color.White : Color.Black;

            UltraWebGrid.DisplayLayout.HeaderStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            UltraWebGrid.DisplayLayout.HeaderStyleDefault.BorderWidth = blackStyle ? 1 : onWall ? 3 : 1;

            UltraWebGrid.DisplayLayout.RowStyleDefault.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            UltraWebGrid.DisplayLayout.RowStyleDefault.BorderColor = blackStyle ? Color.DarkGray : onWall ? Color.Black : Color.Gray;
            UltraWebGrid.DisplayLayout.RowStyleDefault.BorderWidth = 1;

            Label1.Font.Size = Convert.ToInt32(14 * fontSizeMultiplier);
            Label2.Font.Size = Convert.ToInt32(12 * fontSizeMultiplier);

            GrowRateHintLabel.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            GrowRateHintLabel.ForeColor = fontColor;
            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            GrowRateHintLabel.Text = String.Format("Индикатор {0} / {1} - рост/снижение показателя относительно прошлого / позапрошлого года", greenGradientBar, redGradientBar);

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
            ComboYear.Visible = !onWall;
            ComboMonth.Visible = !onWall;
            ComboFO.Visible = !onWall;
            ComboSKIFLevel.Visible = !onWall;
            RefreshButton1.Visible = !onWall;
            PopupInformer1.Visible = !onWall;
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0013_NAO_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);
            
            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (!IsRFSelected)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
            UltraWebGrid.Width = Unit.Empty;
        }

        private string GetIndicatorFormatString(string indicatorName)
        {
            return indicatorName.ToLower().Contains("темп роста") ? "P2" : "N2";
        }

        private int GetColumnWidth(string indicatorName)
        {
            return indicatorName.ToLower().Contains("темп роста") ? 76 : 76;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            if (columnCount == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(135);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string indicatorName = e.Layout.Bands[0].Columns[i].Header.Caption;
                string formatString = GetIndicatorFormatString(indicatorName);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(GetColumnWidth(indicatorName));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            for (int i = 0; i < columnCount; i = i + 1)
            {
                int columnWidth = Convert.ToInt32(e.Layout.Bands[0].Columns[i].Width.Value);
                e.Layout.Bands[0].Columns[i].Width = onWall ? Convert.ToInt32(columnWidth * widthMultiplier) : columnWidth;
            }

            headerLayout.AddCell("Субъект");

            for (int i = 1; i < columnCount; i = i + 5)
            {
                string headerCaption = String.Empty;
                int groupIndex = i > 10 ? 2 : (i - 1) / 5;
                switch (groupIndex)
                {
                    case 0:
                        {
                            headerCaption = "Итого доходов";
                            break;
                        }
                    case 1:
                        {
                            headerCaption = "Итого расходов";
                            break;
                        }
                    case 2:
                        {
                            headerCaption = "Профицит(+) / Дефицит(-)";
                            break;
                        }
                }

                GridHeaderCell headerCell = headerLayout.AddCell(headerCaption);
                headerCell.AddCell(String.Format("На {0:dd.MM.yyyy}", currentDate.AddYears(-2)));
                headerCell.AddCell(String.Format("На {0:dd.MM.yyyy}", currentDate.AddYears(-1)));
                headerCell.AddCell(String.Format("На {0:dd.MM.yyyy}", currentDate));
                headerCell.AddCell(String.Format("Темп роста к {0} году", currentDate.Year - 1));
                headerCell.AddCell(String.Format("Темп роста к {0} году", currentDate.Year - 2));
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i = i + 1)
            {
                string columnName = e.Row.Band.Columns[i].Header.Caption;
                bool isRateColumn = columnName.ToLower().Contains("темп роста");

                if (isRateColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    string img = String.Empty;
                    int barHeight = onWall ? 80 : 20;
                    int barBottomMargin = onWall ? 5 - barHeight : -barHeight;
                    if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        img = "../../images/RedGradientBarInverse.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }
                    else if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        img = "../../images/GreenGradientBarInverse.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }

                    string format = e.Row.Band.Grid.Columns[i].Format;
                    string value = Convert.ToDouble(e.Row.Cells[i].Value).ToString(format);
                    e.Row.Cells[i].Value = String.Format("<div style='position: relative; z-index: 1; margin-bottom: {3}px; margin-right: -5px; width:100%; height:{2}px'><img src=\"{1}\" width=\"100%\" height=\"{2}px\"></div><div style='position: relative; z-index: 2;'>{0}</div>",
                            value, img, barHeight, barBottomMargin);
                }
            }

            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != String.Empty)
            {
                if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                {
                    foreach (UltraGridCell cell in e.Row.Cells)
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (Decimal.TryParse(cell.Value.ToString(), out value))
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
    }
}