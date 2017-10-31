using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0004_NAO
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private DateTime currentDate;

        private bool onWall;
        private bool blackStyle;
        private double widthMultiplier;
        private int fontSizeMultiplier;
        private int primitiveSizeMultiplier;
        private int pageWidth;
        private int pageHeight;

        #region Параметры запроса

        private CustomParam yearComboValue;
        private CustomParam monthComboValue;
        private CustomParam subjectComboValue;
        private CustomParam levelComboValue;
        private CustomParam selectedLevel;

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

            #region Инициализация параметров запроса

            yearComboValue = UserParams.CustomParam("year_combo_value", true);
            monthComboValue = UserParams.CustomParam("month_combo_value", true);
            subjectComboValue = UserParams.CustomParam("subject_combo_value", true);
            levelComboValue = UserParams.CustomParam("level_combo_value", true);
            selectedLevel = UserParams.CustomParam("selected_level");

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
                string query = DataProvider.GetQueryText("FK_0001_0004_NAO_date");
                DataTable dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                yearComboValue.Value = endYear.ToString();
                monthComboValue.Value = month;
                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    subjectComboValue.Value = RegionSettings.Instance.Name;
                }
                else
                {
                    subjectComboValue.Value = "Амурская область";
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

                regionsCombo.Title = "Федеральный округ";
                regionsCombo.Width = 410;
                regionsCombo.MultiSelect = false;
                regionsCombo.ParentSelect = false;
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.SetСheckedState(subjectComboValue.Value, true);

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState(levelComboValue.Value, true);
            }

            yearComboValue.Value = ComboYear.SelectedValue;
            monthComboValue.Value = ComboMonth.SelectedValue;
            subjectComboValue.Value = regionsCombo.SelectedValue;
            levelComboValue.Value = ComboSKIFLevel.SelectedValue;

            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;
            selectedLevel.Value = levelComboValue.Value;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);

            Page.Title = String.Format("Доходы бюджета ({0})", UserParams.StateArea.Value);
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{3} за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboSKIFLevel.SelectedValue);

            UserParams.PeriodMonth.Value = monthComboValue.Value;
            UserParams.PeriodYear.Value = yearComboValue.Value;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            WallLink.Text = "Для&nbsp;видеостены";
            WallLink.NavigateUrl = String.Format("{0};onWall=true", UserParams.GetCurrentReportParamList());

            BlackStyleWallLink.Text = "Для&nbsp;видеостены&nbsp;(черный&nbsp;стиль)";
            BlackStyleWallLink.NavigateUrl = String.Format("{0};onWall=true;blackStyle=true", UserParams.GetCurrentReportParamList());
        }

        private void SetScaleSize()
        {
            fontSizeMultiplier = onWall ? 5 : 1;
            pageWidth = onWall ? 5600 : (int)Session["width_size"];
            pageHeight = onWall ? 1450 : (int)Session["height_size"] - 270;
            primitiveSizeMultiplier = onWall ? 4 : 1;

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
            
            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            EqualbilityHintLabel.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            EqualbilityHintLabel.ForeColor = fontColor;
            GrowRateHintLabel.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            GrowRateHintLabel.ForeColor = fontColor;
            ShareHintLabel.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            ShareHintLabel.ForeColor = fontColor;

            EqualbilityHintLabel.Text = String.Format("Исполнено % {0} / {1} - равномерность/неравномерность исполнения бюджета", greenGradientBar, redGradientBar);
            GrowRateHintLabel.Text = String.Format("Темп роста {0} / {1} - рост/снижение значения показателя относительно прошлого года", greenGradientBar, redGradientBar);
            ShareHintLabel.Text = String.Format("Доля {0} / {1} - рост/снижение доли показателя относительно прошлого года", greenGradientBar, redGradientBar);

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
            regionsCombo.Visible = !onWall;
            ComboSKIFLevel.Visible = !onWall;
            PopupInformer1.Visible = !onWall;
            RefreshButton1.Visible = !onWall;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_NAO_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = CRHelper.ToUpperFirstSymbol(row[0].ToString().ToLower());
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
             UltraWebGrid.Height = Unit.Empty;
             UltraWebGrid.Width = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count > 0)
            {
                e.Layout.Bands[0].HeaderStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[10].Hidden = true;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "КД", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Исполнено, млн.руб.", "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Исполнено прошлый год, млн.руб.", "Исполнено за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Доля", "Доля дохода в общей сумме доходов субъекта");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Доля в прошлом году", "Доля дохода в общей сумме фактических доходов в прошлом году");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Назначено, млн.руб.", "Плановые назначения на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Назначено прошлый год, млн.руб.", "Назначения в аналогичном периоде прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Исполнено %", "Процент выполнения назначений/ Оценка равномерности исполнения (1/12 годового плана в месяц)");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Исполнено % прошлый год", "Процент выполнения назначений за аналогичный период прошлого года");

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0) ?
                                                                             HorizontalAlign.Left :
                                                                             HorizontalAlign.Right;
                    double width;
                    switch (i)
                    {
                        case 0:
                            {
                                width = 300;
                                break;
                            }
                        case 1:
                        case 2:
                        case 6:
                        case 7:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                                width = 92;
                                break;
                            }
                        case 3:
                        case 4:
                        case 5:
                        case 8:
                        case 9:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                                width = 78;
                                break;
                            }
                        default:
                            {
                                width = 114;
                                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                                break;
                            }
                    }
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width);
                }
                e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(95);
                e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(90);

                int columnCount = e.Layout.Bands[0].Columns.Count;
                for (int i = 0; i < columnCount; i = i + 1)
                {
                    int columnWidth = Convert.ToInt32(e.Layout.Bands[0].Columns[i].Width.Value);
                    e.Layout.Bands[0].Columns[i].Width = onWall ? Convert.ToInt32(columnWidth * widthMultiplier) : columnWidth;
                }
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool complete = (i == 8);
                bool growRate = (i == 3);
                bool share = (i == 4);

                if (e.Row.Cells[i] == null)
                {
                    continue;
                }

                string img = String.Empty;
                int barHeight = onWall ? 60 : 17;
                int barBottomMargin = onWall ? -barHeight - 5 : -barHeight;

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;

                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            img = "../../images/RedGradientBarInverse.png";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        else
                        {
                            img = "../../images/GreenGradientBarInverse.png";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                        }

                        string format = e.Row.Band.Grid.Columns[i].Format;
                        string value = Convert.ToDouble(e.Row.Cells[i].Value).ToString(format);
                        e.Row.Cells[i].Value = String.Format("<div style='position: relative; z-index: 1; margin-bottom: {3}px; margin-right: -5px; width:100%; height:{2}px'><img src=\"{1}\" width=\"100%\" height=\"{2}px\"></div><div style='position: relative; z-index: 2;'>{0}</div>",
                                value, img, barHeight, barBottomMargin);
                    }
                }

                if (growRate && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    img = String.Empty;
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        img = "../../images/GreenGradientBarInverse.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        img = "../../images/RedGradientBarInverse.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }

                    if (img != String.Empty)
                    {
                        string format = e.Row.Band.Grid.Columns[i].Format;
                        string value = Convert.ToDouble(e.Row.Cells[i].Value).ToString(format);
                        e.Row.Cells[i].Value = String.Format(
                                "<div style='position: relative; z-index: 1; margin-bottom: {3}px; margin-right: -5px; width:100%; height:{2}px'><img src=\"{1}\" width=\"100%\" height=\"{2}px\"></div><div style='position: relative; z-index: 2;'>{0}</div>",
                                value, img, barHeight, barBottomMargin);
                    }
                }

                if (share && 
                    e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty &&
                    e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != String.Empty)
                {
                    img = String.Empty;
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < Convert.ToDouble(e.Row.Cells[i + 1].Value))
                    {
                        img = "../../images/RedGradientBarInverse.png";
                        e.Row.Cells[i].Title = "Доля снизилась с прошлого года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) > Convert.ToDouble(e.Row.Cells[i + 1].Value))
                    {
                        img = "../../images/GreenGradientBarInverse.png";
                        e.Row.Cells[i].Title = "Доля выросла с прошлого года";
                    }

                    if (img != String.Empty)
                    {
                        string format = e.Row.Band.Grid.Columns[i].Format;
                        string value = Convert.ToDouble(e.Row.Cells[i].Value).ToString(format);
                        e.Row.Cells[i].Value = String.Format(
                                "<div style='position: relative; z-index: 1; margin-bottom: {3}px; margin-right: -5px; width:100%; height:{2}px'><img src=\"{1}\" width=\"100%\" height=\"{2}px\"></div><div style='position: relative; z-index: 2;'>{0}</div>",
                                value, img, barHeight, barBottomMargin);
                    }
                }

                if (e.Row.Cells[10] != null && e.Row.Cells[10].Value.ToString() != String.Empty)
                {
                    string level = e.Row.Cells[10].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "Группа":
                            {
                                fontSize = 10;
                                bold = true;
                                break;
                            }
                        case "Подгруппа":
                            {
                                fontSize = 9;
                                italic = false;
                                break;
                            }
                        default:
                            {
                                fontSize = 8;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize * fontSizeMultiplier;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
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

        #endregion
    }
}