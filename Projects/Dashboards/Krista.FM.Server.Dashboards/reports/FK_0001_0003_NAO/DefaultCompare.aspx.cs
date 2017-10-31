using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0003_NAO
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private string populationDate;

        private bool onWall;
        private bool blackStyle;
        private double widthMultiplier;
        private double heightMultiplier;
        private int primitiveSizeMultiplier;
        private int fontSizeMultiplier;
        private int pageWidth;
        private int pageHeight;

        /// <summary>
        /// Выбраны ли все федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        #region Параметры запроса

        private CustomParam yearComboValue;
        private CustomParam monthComboValue;
        private CustomParam foComboValue;
        private CustomParam rzprComboValue;
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
            rzprComboValue = UserParams.CustomParam("kd_combo_value", true);
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
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0003_NAO_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
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

                rzprComboValue.Value = "Расходы бюджета - ИТОГО";

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

                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState(foComboValue.Value, true);

                ComboFKR.Width = 420;
                ComboFKR.Title = "РзПр";
                ComboFKR.MultiSelect = false;
                ComboFKR.ParentSelect = true;
                ComboFKR.FillDictionaryValues(CustomMultiComboDataHelper.FillFKRNames(
                                                  DataDictionariesHelper.OutcomesFKRTypes,
                                                  DataDictionariesHelper.OutcomesFKRLevels));
                ComboFKR.SetСheckedState(rzprComboValue.Value, true);
            }

            yearComboValue.Value = ComboYear.SelectedValue;
            monthComboValue.Value = ComboMonth.SelectedValue;
            foComboValue.Value = ComboFO.SelectedValue;
            rzprComboValue.Value = ComboFKR.SelectedValue;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Page.Title = string.Format("Исполнение расходов ({0})", ComboFO.SelectedIndex == 0
                                                                        ? "РФ"
                                                                        :
                                                                    RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;
            Label2.Text =
                string.Format(
                    "Исполнение консолидированных бюджетов субъектов РФ по расходам ({3}) за {0}&nbsp;{1}&nbsp;{2}&nbsp;года",
                    monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboFKR.SelectedValue);


            int year = Convert.ToInt32(yearComboValue.Value);

            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = monthComboValue.Value;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            UserParams.KDGroup.Value = DataDictionariesHelper.OutcomesFKRTypes[rzprComboValue.Value];
            selectedFo.Value = AllFO ? ".Children" : String.Format(".[{0}]", foComboValue.Value);

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

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

            string bestStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starYellowBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string worseStar = String.Format("<img style=\"vertical-align:middle\" src=\"../../images/starGrayBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string redGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/RedGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            string greenGradientBar = String.Format("<img style=\"vertical-align:top\" src=\"../../images/GreenGradientBarInverse.png\" width=\"{0}px\" height=\"{0}px\">",
                    18 * primitiveSizeMultiplier);

            EqualbilityHintLabel.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            EqualbilityHintLabel.ForeColor = fontColor;
            StarsHintLabel.Font.Size = Convert.ToInt32(9 * fontSizeMultiplier);
            StarsHintLabel.ForeColor = fontColor;

            EqualbilityHintLabel.Text = String.Format("Индикатор {0} / {1} - равномерность/неравномерность исполнения бюджета", greenGradientBar, redGradientBar);
            StarsHintLabel.Text = String.Format("{0} - лучший ранг&nbsp;&nbsp;&nbsp;{1} - худший ранг", bestStar, worseStar);

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
            ComboFKR.Visible = !onWall;
            PopupInformer1.Visible = !onWall;
            RefreshButton1.Visible = !onWall;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0003_NAO_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);
            
            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
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

                if (dtGrid.Columns.Count > 2)
                {
                    dtGrid.Columns[1].ColumnName = "ФО";
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (!AllFO || dtGrid.Rows.Count < 15)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
            UltraWebGrid.Width = Unit.Empty;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count > 0)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(210);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

                bool rankHidden = ComboFKR.SelectedValue == "Расходы бюджета - ИТОГО";

                if (rankHidden)
                {
                    SetColumnParams(e.Layout, 0, 1, "", 45, false);
                    SetColumnParams(e.Layout, 0, 2, "N3", 110, false);
                    SetColumnParams(e.Layout, 0, 3, "N3", 110, false);
                    SetColumnParams(e.Layout, 0, 4, "P2", 110, false);
                    SetColumnParams(e.Layout, 0, 5, "N3", 100, false);
                    SetColumnParams(e.Layout, 0, 6, "N3", 110, false);
                    SetColumnParams(e.Layout, 0, 7, "N0", 100, false);
                    SetColumnParams(e.Layout, 0, 8, "N0", 90, true);
                    SetColumnParams(e.Layout, 0, 9, "N0", 100, false);
                    SetColumnParams(e.Layout, 0, 10, "N0", 90, true);
                    SetColumnParams(e.Layout, 0, 11, "P2", 90, false);
                }
                else
                {
                    SetColumnParams(e.Layout, 0, 1, "", 45, false);
                    SetColumnParams(e.Layout, 0, 2, "N3", 101, false);
                    SetColumnParams(e.Layout, 0, 3, "N3", 101, false);
                    SetColumnParams(e.Layout, 0, 4, "P2", 90, false);
                    SetColumnParams(e.Layout, 0, 5, "N3", 85, false);
                    SetColumnParams(e.Layout, 0, 6, "N2", 100, false);
                    SetColumnParams(e.Layout, 0, 7, "N0", 72, false);
                    SetColumnParams(e.Layout, 0, 8, "N0", 90, true);
                    SetColumnParams(e.Layout, 0, 9, "N0", 80, false);
                    SetColumnParams(e.Layout, 0, 10, "N0", 90, true);
                    SetColumnParams(e.Layout, 0, 11, "P2", 60, false);
                }
                SetColumnParams(e.Layout, 0, 12, "N0", 70, rankHidden);
                SetColumnParams(e.Layout, 0, 13, "N0", 90, true);
                SetColumnParams(e.Layout, 0, 14, "N0", 70, rankHidden);
                SetColumnParams(e.Layout, 0, 15, "N0", 90, true);

                int columnCount = e.Layout.Bands[0].Columns.Count;

                for (int i = 0; i < columnCount; i = i + 1)
                {
                    int columnWidth = Convert.ToInt32(e.Layout.Bands[0].Columns[i].Width.Value);
                    e.Layout.Bands[0].Columns[i].Width = onWall ? Convert.ToInt32(columnWidth * widthMultiplier) : columnWidth;
                }

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "ФО", "Федеральный округ, которому принадлежит субъект");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Исполнено, млн.руб", "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Назначено, млн.руб", "План на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Исполнено %", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, String.Format("Численность постоянного населения {0}, тыс.чел.", populationDate), "Численность постоянного населения");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Бюджетные расходы на душу населения,\n руб./чел.", "Бюджетные расходы, рублей на душу населения");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Ранг бюдж. расходов на душу населения ФО", "Ранг (место) субъекта по бюджетным расходам на душу населения среди субъектов его федерального округа");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Ранг бюдж. расходов на душу населения РФ", "Ранг (место) по бюджетным расходам на душу населения среди всех субъектов");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 11, "Доля в общей сумме расходов", "Доля расхода в общей сумме расходов субъекта");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 12, "Ранг доля ФО", "Ранг (место) субъекта по доле расхода в общей сумме расходов субъекта среди субъектов его федерального округа");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 14, "Ранг доля РФ", "Ранг (место) по доле расхода в общей сумме расходов  субъекта среди всех субъектов");
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[7].Value = string.Empty;
                e.Row.Cells[9].Value = string.Empty;
                e.Row.Cells[12].Value = string.Empty;
                e.Row.Cells[14].Value = string.Empty;
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Top = 5;
                e.Row.Cells[i].Style.Padding.Bottom = 5;

                bool rank = (i == 7 || i == 9 || i == 12 || i == 14);
                bool complete = (i == 4);
                
                string img = String.Empty;
                int barHeight = onWall ? 60 : 17;
                int barBottomMargin = onWall ? -barHeight - 5 : -barHeight;

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != String.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != String.Empty)
                    {
                        string region = (i == 9 || i == 14) ? "РФ" : "федеральном округе";
                        string obj = (i == 7 || i == 9) ? "доход на душу населения" : "процент исполнения";
                        string bestStr = (i == 7 || i == 9) ? "Самые высокие" : "Самая высокая";
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            img = "../../images/starYellowBBLarge.png";
                            
                            e.Row.Cells[i].Title = string.Format("{2} {1} в {0}", region, obj, bestStr);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            img = "../../images/starGrayBBLarge.png";
                            e.Row.Cells[i].Title = string.Format("Самый низкий {1} в {0}", region, obj, bestStr);
                        }

                        if (img != String.Empty)
                        {
                            string format = e.Row.Band.Grid.Columns[i].Format;
                            string value = Convert.ToDouble(e.Row.Cells[i].Value).ToString(format);

                            e.Row.Cells[i].Value =
                                String.Format(
                                    "<div style='float:left; margin-bottom: {3}px; margin-right: -5px; width:49%; height:{2}px'><img src=\"{1}\" height=\"{2}px\"></div><div style='float: right;'>{0}</div>",
                                    value, img, barHeight, barBottomMargin);
                        }
                    }
                }

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
    }
}