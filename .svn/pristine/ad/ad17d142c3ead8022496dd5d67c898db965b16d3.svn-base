using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image=System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0016 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtData = new DataTable();
        private DataTable dtExecuteOutcome = new DataTable();

        private double othersEthalon;
        private double salaryEthalon;
        private double commonEthalon;

        private DateTime date;

        private DataTable dtOutcomes = new DataTable();

        #region Параметры запроса

        // мера План
        private CustomParam measurePlan;
        // мера Факт
        private CustomParam measureFact;
        // мера Остаток на начало
        private CustomParam measureStartBalance;
        // мера Остаток на конец
        private CustomParam measureEndBalance;

        #endregion

        public bool IsQuaterPlanType
        {
            get
            {
                return RegionSettingsHelper.Instance.CashPlanType.ToLower() == "quarter";
            }
        }

        public bool IsYar
        {
            get
            {
                return RegionSettingsHelper.Instance.Name.ToLower() == "ярославская область";
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            Image1.ImageUrl = "../../../Images/CashIncomes.png";
            Image2.ImageUrl = "../../../Images/CashOutcomes.png";
            Image3.ImageUrl = "../../../Images/CashRests.png";

            #region Инициализация параметров запроса

            measurePlan = UserParams.CustomParam("measure_plan");
            measureFact = UserParams.CustomParam("measure_fact");
            measureStartBalance = UserParams.CustomParam("measure_start_balance");
            measureEndBalance = UserParams.CustomParam("measure_end_balance");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][5].ToString();
            

            if (!dtDate.Rows[0][4].ToString().Contains("Заключительные обороты"))
            {
                date = new DateTime(
                   Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                   CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                   Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = "&nbsp;за&nbsp;<span style='color: white;'>" + CRHelper.PeriodDescr(date, 3).Replace("года", "г.") + "</span>";
                    lbDate.Text = "&nbsp;(на&nbsp;<span style='color: white;'>" + date.ToString("dd.MM.yyyy") + "</span>) по областному бюджету, тыс.руб.";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = "&nbsp;на&nbsp;<span style='color: white;'>" + date.ToString("dd.MM.yyyy") + "</span>, тыс.руб.";
                }
            }
            else
            {
                date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = "&nbsp;за " + CRHelper.PeriodDescr(date, 3).Replace("года", "г.");
                    lbDate.Text = "<br/>за&nbsp;<span style='color: white;'>" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " г.</span>" + ", тыс.руб.";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = " за&nbsp;<span style='color: white;'>" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " г.</span>" + ", тыс.руб.";
                }
            }

            if (IsQuaterPlanType)
            {
                measurePlan.Value = "План";
                measureFact.Value = "Факт";
            }
            else
            {
                measurePlan.Value = "План_Нарастающий итог";
                measureFact.Value = "Факт_Нарастающий итог";
            }

            measureStartBalance.Value = (IsYar) ? "Остаток средств на начало квартала" : RegionSettingsHelper.Instance.CashPlanBalance;
            measureEndBalance.Value = (measureStartBalance.Value == "Остаток средств")
                                          ? measureStartBalance.Value
                                          : "Остаток средств на конец квартала";

            query = DataProvider.GetQueryText("data");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtData);

            double value;

            double planIncomes;
            double factIncomes;
            double planOutcomes;
            double factOutcomes;

            Label11.Visible = !IsQuaterPlanType;
            Label13.Visible = !IsQuaterPlanType;
            lbRestStartFact.Visible = !IsQuaterPlanType;

            if (measureStartBalance.Value == "Остаток средств")
            {
                Label1.Text = "Остаток средств";
                Label2.Visible = false;
                Label3.Visible = false;
                Label4.Visible = false;
                lbRestEndPlan.Visible = false;
                lbRestEndFact.Visible = false;
                restEndTR.Visible = false;
            }
            else
            {
                Label1.Text = "Остаток средств на начало квартала";
                Label2.Visible = true;
                Label3.Visible = true;
                Label4.Visible = true;
                lbRestEndPlan.Visible = true;
                lbRestEndFact.Visible = true;
                restEndTR.Visible = true;
            }

            if (Double.TryParse(dtData.Rows[0][1].ToString(), out value))
            {
                lbRestStartPlan.Text = (value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[0][2].ToString(), out value))
            {
                lbRestStartFact.Text = (value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[1][1].ToString(), out value))
            {
                lbRestEndPlan.Text = (value / 1000).ToString("N0") + "&nbsp;";
            }
            if (Double.TryParse(dtData.Rows[1][2].ToString(), out value))
            {
                lbRestEndFact.Text = (value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[2][1].ToString(), out planIncomes))
            {
                lbPlanIncomes.Text = (planIncomes / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[2][2].ToString(), out factIncomes))
            {
                lbFactIncomes.Text = (factIncomes / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[3][1].ToString(), out planOutcomes))
            {
                lbPlanOutcomes.Text = (planOutcomes / 1000).ToString("N0");
            }
            if (Double.TryParse(dtData.Rows[3][2].ToString(), out factOutcomes))
            {
                lbFactOutcomes.Text = (factOutcomes / 1000).ToString("N0");
            }

            IPadElementHeader1.Text = "Кассовые поступления " + (factIncomes / planIncomes).ToString("P2");
            IPadElementHeader2.Text = "Кассовые выплаты " + (factOutcomes / planOutcomes).ToString("P2");

            if (planIncomes != 0)
            {
                UltraGauge gauge = GetGauge(factIncomes / planIncomes * 100);
                PlaceHolderIncomes.Controls.Add(gauge);
            }
            if (planOutcomes != 0)
            {
                UltraGauge gauge1 = GetGauge(factOutcomes / planOutcomes * 100);
                PlaceHolderOutcomes.Controls.Add(gauge1);
            }

            othersEthalon = CRHelper.QuarterDaysCountToDate(date) / CRHelper.QuarterDaysCount(date);
            commonEthalon = CommonAssessionLimit();
            salaryEthalon = SalaryAssessionLimit();
            GetIncomesDetailData();
            MakeHtmlTableIncomes();
            GetOutcomesDetailData();
            MakeHtmlTableOutcomes();

            TagCloud1.startFontSize = 8;
            TagCloud1.groupCount = 5;
            TagCloud1.fontStep = 3;
            BindTagCloud();

            //incomes.Style.Add("margin-left", "-40px");
            incomes.Style.Add("margin-right", "0px");
            incomes.Style.Add("padding-right", "0px");
            incomes.Style.Add("margin-top", "3px");

            Outcomes.Style.Add("margin-left", "-2px");
            Outcomes.Style.Add("margin-right", "0px");
            Outcomes.Style.Add("padding-right", "0px");
            Outcomes.Style.Add("margin-top", "3px");
        }

        private void BindTagCloud()
        {
            string query = DataProvider.GetQueryText("TagCloud");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "Наименование показателя", dtExecuteOutcome);

            Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
            Collection<Color> colors = new Collection<Color>();
            foreach(DataRow row in dtExecuteOutcome.Rows)
            {
                Tag tag = new Tag();
                tag.weight = Convert.ToInt32(row[6]);
                tag.key = String.Format("{0} ({1:P0})", row[8], row[7]);
                tag.toolTip = String.Format("{0}<br/>план {1:N0} тыс. руб.<br/>факт {2:N0} тыс. руб.<br/>исполнено {3:P0}", row[0], row[6], row[9], row[7]);

                if (row[6] != DBNull.Value &&
                    row[6].ToString() != String.Empty)
                {
                    tags.Add(tag.key, tag);
                }
                bool crime = false;
                for (int i = 1; i < 6; i++)
                {
                    if (row[i] != DBNull.Value &&
                        row[i].ToString() != String.Empty)
                    {
                        string name = row.Table.Columns[i].ColumnName.Split(';')[0];
                        double ethalon;

                        switch (name)
                        {
                            case ("Заработная плата"):
                                {
                                    ethalon = salaryEthalon;
                                    break;
                                }
                            case ("Прочие расходы"):
                                {
                                    ethalon = othersEthalon;
                                    break;
                                }
                            default:
                                {
                                    ethalon = commonEthalon;
                                    break;
                                }
                        }
                        double value = Convert.ToDouble(row[i]);
                        if (value < ethalon)
                        {
                            crime = true;
                        }
                    }
                }
                if (crime)
                {
                    colors.Add(Color.Red);
                }
                else
                {
                    colors.Add(Color.Green);
                }
            }
            TagCloud1.ForeColors = colors;
            TagCloud1.Render(tags);
        }

        #region гейдж
        private static UltraGauge GetGauge(double markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 300;
            gauge.Height = 61;
            gauge.Style.Add("margin-bottom", "3px");
            gauge.Style.Add("margin-top", "-5px");
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = "../../../TemporaryImages/gauge_imfrf02_01_#SEQNUM(100).png";

            // Настраиваем гейдж
            LinearGauge linearGauge = new LinearGauge();
            linearGauge.CornerExtent = 10;
            linearGauge.MarginString = "2, 10, 2, 10, Pixels";

            // Выбираем максимум шкалы 
            double endValue = (Math.Max(100, markerValue));

            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = 98;
            scale.StartExtent = 2;
            scale.OuterExtent = 93;
            scale.InnerExtent = 52;

            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(0, 255, 255, 255);
            gradientBrush.StartColor = Color.FromArgb(120, 255, 255, 255);
            scale.BrushElements.Add(gradientBrush);
            linearGauge.Scales.Add(scale);
            AddMainScale(endValue, linearGauge, markerValue);
            AddMajorTickmarkScale(endValue, linearGauge);
            AddGradient(linearGauge);

            linearGauge.Margin.Top = 1;
            linearGauge.Margin.Bottom = 1;

            gauge.Gauges.Add(linearGauge);
            return gauge;
        }

        private const int ScaleStartExtent = 5;
        private const int ScaleEndExtent = 97;

        private static void AddMajorTickmarkScale(double endValue, LinearGauge linearGauge)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.MajorTickmarks.EndWidth = 2;
            scale.MajorTickmarks.StartWidth = 2;
            scale.MajorTickmarks.EndExtent = 40;
            scale.MajorTickmarks.StartExtent = 25;
            scale.MajorTickmarks.BrushElements.Add(new SolidFillBrushElement(Color.White));
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 10));
            linearGauge.Scales.Add(scale);
        }

        private static void AddGradient(LinearGauge linearGauge)
        {
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(150, 150, 150);
            gradientBrush.StartColor = Color.FromArgb(10, 255, 255, 255);
            gradientBrush.GradientStyle = Gradient.BackwardDiagonal;
            linearGauge.BrushElements.Add(gradientBrush);
        }

        private static void AddMainScale(double endValue, LinearGauge linearGauge, double markerValue)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 5));

            AddMainScaleRange(scale, markerValue);
            SetMainScaleLabels(scale);
            linearGauge.Scales.Add(scale);
        }

        private static void SetMainScaleLabels(LinearGaugeScale scale)
        {

            scale.Labels.ZPosition = LinearTickmarkZPosition.AboveMarkers;
            scale.Labels.Extent = 9;
            Font font = new Font("Arial", 9);
            scale.Labels.Font = font;
            scale.Labels.EqualFontScaling = false;
            SolidFillBrushElement solidFillBrushElement = new SolidFillBrushElement(Color.White);
            solidFillBrushElement.RelativeBoundsMeasure = Measure.Percent;
            Rectangle rect = new Rectangle(0, 0, 80, 0);
            solidFillBrushElement.RelativeBounds = rect;
            scale.Labels.BrushElements.Add(solidFillBrushElement);
            scale.Labels.Shadow.Depth = 2;
            scale.Labels.Shadow.BrushElements.Add(new SolidFillBrushElement());
        }

        private static void AddMainScaleRange(LinearGaugeScale scale, double markerValue)
        {
            LinearGaugeRange range = new LinearGaugeRange();
            range.EndValue = markerValue;
            range.StartValue = 0;
            range.OuterExtent = 80;
            range.InnerExtent = 20;
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(1, 51, 75);
            gradientBrush.StartColor = Color.FromArgb(8, 218, 164);
            gradientBrush.GradientStyle = Gradient.Vertical;
            range.BrushElements.Add(gradientBrush);
            scale.Ranges.Add(range);
        }
        #endregion

        private void MakeHtmlTableIncomes()
        {
            incomes.CssClass = "HtmlTableCompact";

            AddHeaderRow(incomes);

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell;
                if (dtData.Rows[i][0].ToString() == "Доходы областного бюджета")
                {
                    cell = GetNameCell(dtData.Rows[i][0].ToString(), false);
                    cell.Style.Add("padding-top", "2px");
                    cell.Style.Add("padding-bottom", "2px");
                    row.Cells.Add(cell);
                }
                else if (dtData.Rows[i][0].ToString() == "Поступления и выплаты из источников финансирования дефицита областного бюджета - всего")
                {
                    dtData.Rows[i][0] = "Поступления из источников финансирования дефицита областного бюджета";
                    cell = GetNameCell(dtData.Rows[i][0].ToString(), false);
                    cell.ColumnSpan = 4;
                    cell.Style.Add("padding-top", "2px");
                    cell.Style.Add("padding-bottom", "2px");
                    row.Cells.Add(cell);
                    incomes.Rows.Add(row);
                    continue;
                }
                else   
                {
                    cell = GetNameCell(dtData.Rows[i][0].ToString(), true);
                    cell.Style.Add("padding-top", "2px");
                    cell.Style.Add("padding-bottom", "2px");
                    row.Cells.Add(cell);
                }

                cell = GetValueCell(string.Format("{0:P0}", dtData.Rows[i][3]));
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][1]));
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][2]));
                row.Cells.Add(cell);

                incomes.Rows.Add(row);
            }
        }

        private void MakeHtmlTableOutcomes()
        {
            Outcomes.CssClass = "HtmlTableCompact";

            AddHeaderRow(Outcomes);

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell;
                if (dtData.Rows[i][0].ToString() == "Расходы")
                {
                    cell = GetNameCell(dtData.Rows[i][0].ToString(), false);
                    row.Cells.Add(cell);

                    cell = GetValueCell(string.Format("{0:P0}", dtData.Rows[i][3]));
                    row.Cells.Add(cell);

                    cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][1]));
                    row.Cells.Add(cell);

                    cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][2]));
                    row.Cells.Add(cell);

                    Outcomes.Rows.Add(row);

                    addInnerTable();
                }
                else if (dtData.Rows[i][0].ToString() == "Поступления и выплаты из источников финансирования дефицита областного бюджета - всего")
                {
                    dtData.Rows[i][0] = "Выплаты из источников финансирования дефицита областного бюджета";
                    cell = GetNameCell(dtData.Rows[i][0].ToString(), false);
                    cell.ColumnSpan = 4;
                    row.Cells.Add(cell);
                    Outcomes.Rows.Add(row);
                    continue;
                }
                else
                {
                    cell = GetNameCell(dtData.Rows[i][0].ToString(), true);
                    row.Cells.Add(cell);

                    cell = GetValueCell(string.Format("{0:P0}", dtData.Rows[i][3]));
                    row.Cells.Add(cell);

                    cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][1]));
                    row.Cells.Add(cell);

                    cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][2]));
                    row.Cells.Add(cell);

                    Outcomes.Rows.Add(row);
                }
            }
        }

        private TableCell GetValuesCell(string valueFact, string valuePercent)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = valueFact;
            lb.CssClass = "TableFont";
            cell.Controls.Add(lb);
            lb = new Label();
            lb.Text = valuePercent;
            lb.CssClass = "TableFontGrey";
            cell.Controls.Add(lb);
            cell.Style["border-left-style"] = "none";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Right;
            return cell;
        }

        private TableCell GetValueCell(string value)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = value;
            lb.CssClass = "TableFont";
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.Style.Add("padding-right", "3px");
            return cell;
        }

        private TableCell GetNameCell(string name)
        {
            return GetNameCell(name, false);
        }

        private TableCell GetNameCell(string name, bool child)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = name;
            
            if (child)
            {
                lb.CssClass = "TableFontGrey";
                cell.Style.Add("padding-left", "10px");
            }
            else
            {
                lb.CssClass = "TableFont";
                cell.Style.Add("padding-left", "3px");
            }
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
            return cell;
        }

        private void AddHeaderRow(Table table)
        {
            TableCell cell;
            TableRow row;

            row = new TableRow();
            cell = new TableCell();
            cell.Width = 225;
            cell.CssClass = "HtmlTableHeader";
            //cell.Style.Add("font-size", "14px");
            cell.Text = "Показатель";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 34;
            cell.CssClass = "HtmlTableHeader";
           // cell.Style.Add("font-size", "14px");
            cell.Text = "% исп.";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 77;
            cell.CssClass = "HtmlTableHeader";
           // cell.Style.Add("font-size", "14px");
            cell.Text = "План";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 77;
            cell.CssClass = "HtmlTableHeader";
           // cell.Style.Add("font-size", "14px");
            cell.Text = "Факт";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            table.Rows.Add(row);
        }

        private void addInnerTable()
        {
            TableCell cell;
            TableRow row;
                       

            for (int i = 0; i < dtOutcomes.Rows.Count; i++)
            {
                row = new TableRow();
                cell = new TableCell();

                string name = dtOutcomes.Rows[i][0].ToString();

                cell = GetNameCell(dtOutcomes.Rows[i][0].ToString(), true);
                cell.Style["border-bottom-color"] = "Transparent";
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:P0}", dtOutcomes.Rows[i][3]));
                cell.Style["border-bottom-color"] = "Transparent";
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:N0}", dtOutcomes.Rows[i][1]));
                cell.Style["border-bottom-color"] = "Transparent";
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:N0}", dtOutcomes.Rows[i][2]));
                cell.Style["border-bottom-color"] = "Transparent";
                row.Cells.Add(cell);

                //row.Style["border-bottom-style"] = "none";
                row.Style["border-bottom-color"] = "Red";
                Outcomes.Rows.Add(row);
                
                double value;
                string hintRowText = string.Empty;
                double ethalon;

                switch (name)
                {
                    case ("Заработная плата"):
                        {
                            ethalon = salaryEthalon;
                            break;
                        }
                    case ("Прочие расходы"):
                        {
                            ethalon = othersEthalon;
                            break;
                        }
                    default:
                        {
                            ethalon = commonEthalon;
                            break;
                        }
                }

                Image image = new Image();
                if (Double.TryParse(dtOutcomes.Rows[i][3].ToString(), out value))
                {
                    
                    image.ImageUrl = value >= ethalon ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";
                    hintRowText = value >= ethalon
                                      ? String.Format("Соблюдается равномерность ({0:P2})&nbsp;", ethalon)
                                      : String.Format("Не соблюдается равномерность ({0:P2})&nbsp;", ethalon);
                }

                // Хинтовая строка
                if (!String.IsNullOrEmpty(hintRowText))
                {
                    row = new TableRow();
                    cell = new TableCell();
                    //if (name == "Прочие расходы")
                    //{
                    //    row.Style["border-bottom"] = "#323232 4px solid";
                    //}
                    cell.CssClass = "HtmlTableCompact";
                    Label lb = new Label();
                    lb.Text = hintRowText;
                    lb.Font.Italic = true;
                    lb.CssClass = "ServeText";
                    cell.Controls.Add(lb);
                    cell.VerticalAlign = VerticalAlign.Middle;
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    cell.ColumnSpan = 4;
                    //cell.Style["border-top-style"] = "none";
                    row.Cells.Add(cell);
                    cell.Controls.Add(image);
                    //row.Style["border-top-style"] = "none";
                    cell.Style["border-top-color"] = "Transparent";
                    Outcomes.Rows.Add(row);
                }
            }
            
        }


        private void GetIncomesDetailData()
        {
            string query = DataProvider.GetQueryText("IncomesDetail");
            dtData = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtData);
        }

        private void GetOutcomesDetailData()
        {
            string query = DataProvider.GetQueryText("OutcomesDetail");
            dtData = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtData);

            query = DataProvider.GetQueryText("Outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "Показатель", dtOutcomes);
        }


        /// <summary>
        /// Пороговое значение оценки зарплаты.
        /// </summary>
        /// <returns></returns>
        private double SalaryAssessionLimit()
        {
            // Берем номер месяца в квартале
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            int day = date.Day;
            if (day < 6)
            {
                // Выплат в этом месяце не было
                return (monthNum - 1) / 3;
            }
            if (day < 21)
            {
                // Была одна выплата
                return (monthNum - 1) / 3 + 1.0 / 6;
            }
            // Все выплаты
            return (monthNum) / 3;
        }

        /// <summary>
        /// Пороговое значение оценки остальных.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            // Берем номер месяца в квартале
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            // Если последний день месяца
            if (CRHelper.MonthLastDay(date.Month) ==
                date.Day)
            {
                return (monthNum) / 3;
            }
            else
            {
                return (monthNum - 1) / 3;
            }
        }

    }
}
