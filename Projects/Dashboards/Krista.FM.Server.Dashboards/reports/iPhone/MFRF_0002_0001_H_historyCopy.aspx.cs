using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;

using Image=System.Web.UI.WebControls.Image;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0002_0001_H_copy : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            SetIndicatorsData();
        }

        private DataTable dtIndicators = new DataTable();
        private DataTable dtDate = new DataTable();

        private void SetIndicatorsData()
        {
            LabelState.Text = UserParams.StateArea.Value;
            string query = DataProvider.GetQueryText("MFRF_0002_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDate.Rows[0][3]);

            Label.Text = string.Format("данные на {0} квартал {1} года", (dtDate.Rows[0][2].ToString()).Split()[1], dtDate.Rows[0][0]);
            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
                        
            query = DataProvider.GetQueryText("MFRF_0002_0001_H");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);
            for (int i = 0; i < dtIndicators.Rows.Count; i++)
            {
                double crime = 1;
                if (Double.TryParse(dtIndicators.Rows[i][6].ToString(), out crime))
                {
                    TableRow row = new TableRow();

                    TableCell cellDescription = new TableCell();
                    UltraGauge gauge = new UltraGauge();
                    gauge.Width = 340;
                    gauge.Height = 71;
                    gauge.DeploymentScenario.FilePath = "../../TemporaryImages";
                    gauge.DeploymentScenario.ImageURL = "../../TemporaryImages/gauge_imfrf02_01_#SEQNUM(100).png";

                    // Настраиваем гейдж
                    LinearGauge linearGauge = new LinearGauge();
                    linearGauge.CornerExtent = 10;
                    linearGauge.MarginString = "2, 10, 2, 10, Pixels";
                    
                    // Выбираем максимум шкалы 
                    double normativeValue = dtIndicators.Rows[i][3].ToString() == "=" ?
                        (double)dtIndicators.Rows[i][4] + 0.2 : (double)dtIndicators.Rows[i][4];
                    double factValue = (double)dtIndicators.Rows[i][2];
                    double avgFo = (double) dtIndicators.Rows[i][9];
                    double avgRf = (double) dtIndicators.Rows[i][10];
                    double endValue = Math.Max(normativeValue, factValue);
                    endValue = Math.Max(endValue, avgRf);
                    endValue = Math.Max(endValue, avgFo);

                    LinearGaugeScale scale = new LinearGaugeScale();
                    scale.EndExtent = 98;
                    scale.StartExtent = 2;
                    scale.OuterExtent = 93;
                    scale.InnerExtent = 45;

                    LinearGaugeRange range = new LinearGaugeRange();
                    range.EndValue = endValue;
                    range.StartValue = 0;
                    range.OuterExtent = 80;
                    range.BrushElements.Add(new SolidFillBrushElement(Color.White));
                    scale.Ranges.Add(range);
                    SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
                    gradientBrush.EndColor = Color.FromArgb(10, 255, 255, 255);
                    gradientBrush.StartColor = Color.FromArgb(80, 255, 255, 255);
                    scale.BrushElements.Add(gradientBrush);
                    linearGauge.Scales.Add(scale);

                    scale = new LinearGaugeScale();
                    scale.EndExtent = 90;
                    scale.StartExtent = 10;
                    range = new LinearGaugeRange();
                    range.EndValue = endValue;
                    range.StartValue = 0;
                    range.OuterExtent = 30;
                    range.InnerExtent = 20;
                    gradientBrush = new SimpleGradientBrushElement();
                    gradientBrush.EndColor = Color.FromArgb(70, 180, 180, 180);
                    gradientBrush.StartColor = Color.DarkGray;
                    gradientBrush.GradientStyle = Gradient.BackwardDiagonal;
                    range.BrushElements.Add(gradientBrush);
                    scale.Ranges.Add(range);
                    scale.MajorTickmarks.EndExtent = 28;
                    scale.MajorTickmarks.StartExtent = 20;
                    scale.MajorTickmarks.StrokeElement = new StrokeElement(Color.FromArgb(225, 225, 225));
                    scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 6));

                    scale.StrokeElement = new StrokeElement(Color.Transparent);
                    scale.StrokeElement.Thickness = 0;
                    linearGauge.Scales.Add(scale);

                    scale = new LinearGaugeScale();
                    scale.EndExtent = 90;
                    scale.StartExtent = 10;
                   
                    scale.MajorTickmarks.EndExtent = 38;
                    scale.MajorTickmarks.StartExtent = 20;
                    scale.MajorTickmarks.BrushElements.Add(new SolidFillBrushElement(Color.FromArgb(255, 61, 22)));
                    scale.MajorTickmarks.StrokeElement = new StrokeElement(Color.FromArgb(225, 225, 225));
                    scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 3));
                    linearGauge.Scales.Add(scale);

                    scale = new LinearGaugeScale();
                    scale.EndExtent = 90;
                    scale.StartExtent = 10;

                    scale.MinorTickmarks.EndExtent = 75;
                    scale.MinorTickmarks.StartExtent = 65;
//                    scale.MinorTickmarks.Frequency = 0.2;

                    Color crimeColor = Color.FromArgb(207, 23, 30);
                    Color legalColor = Color.FromArgb(63, 203, 43);

                    // Цвета для случая ">="
                    Color firstColor = crimeColor;
                    Color secondColor = legalColor;

                    switch (dtIndicators.Rows[i][3].ToString())
                    {
                        case "<=":
                        case "<":
                            {
                                firstColor = legalColor;
                                secondColor = crimeColor;
                                break;
                            }
                        case "=":
                            {
                                firstColor = crimeColor;
                                secondColor = crimeColor;
                                break;
                            }
                    }

                    range = new LinearGaugeRange();
                    range.EndValueString = normativeValue.ToString();
                    range.EndValue = normativeValue;
                    range.StartValue = 0;
                    range.OuterExtent = 20;
                    range.InnerExtent = 0;
                    gradientBrush = new SimpleGradientBrushElement();
                    gradientBrush.EndColor = firstColor;
                    gradientBrush.StartColor = Color.Transparent;
                    gradientBrush.GradientStyle = Gradient.Vertical;
                    range.BrushElements.Add(gradientBrush);
                    scale.Ranges.Add(range);

                    range = new LinearGaugeRange();
                    range.EndValueString = endValue.ToString();
                    range.StartValueString =  normativeValue.ToString();
                    range.EndValue = endValue;
                    range.StartValue = normativeValue;
                    range.OuterExtent = 20;
                    range.InnerExtent = 0;
                    gradientBrush = new SimpleGradientBrushElement();
                    gradientBrush.EndColor = secondColor;
                    gradientBrush.StartColor = Color.Transparent;
                    gradientBrush.GradientStyle = Gradient.Vertical;
                    range.BrushElements.Add(gradientBrush);
                    scale.Ranges.Add(range);
                    
		            LinearGaugeNeedle needle = new LinearGaugeNeedle();
                    needle.MidWidth = 4;
                    needle.EndWidth = 4;
                    needle.StartWidth = 4;
                    needle.MidExtent = 10;
                    needle.EndExtent = 27;
                    needle.Value = factValue;
                    needle.StartExtent = -5;
                    needle.StrokeElement = new StrokeElement(Color.Silver);
                    needle.StrokeElement.Thickness = 2;
                    Color needleColor = crime == 1 ? crimeColor : legalColor;
                    needle.BrushElements.Add(new SolidFillBrushElement(needleColor));
                    scale.Markers.Add(needle);

                    needle = new LinearGaugeNeedle();
                    needle.MidWidth = 4;
                    needle.EndWidth = 4;
                    needle.StartWidth = 4;
                    needle.MidExtent = 10;
                    needle.EndExtent = 27;
                    needle.Value = avgFo;
                    needle.StartExtent = -5;
                    needleColor = Color.Silver;
                    needle.BrushElements.Add(new SolidFillBrushElement(needleColor));
                    scale.Markers.Add(needle);

                    needle = new LinearGaugeNeedle();
                    needle.MidWidth = 4;
                    needle.EndWidth = 4;
                    needle.StartWidth = 4;
                    needle.MidExtent = 10;
                    needle.EndExtent = 27;
                    needle.Value = avgRf;
                    needle.StartExtent = -5;
                    needleColor = Color.Silver;
                    needle.BrushElements.Add(new SolidFillBrushElement(needleColor));
                    scale.Markers.Add(needle);

                    scale.MajorTickmarks.EndWidth = 2;
                    scale.MajorTickmarks.StartWidth = 2;
                    scale.MajorTickmarks.EndExtent = 40;
                    scale.MajorTickmarks.StartExtent = 20;
                    scale.MajorTickmarks.BrushElements.Add(new SolidFillBrushElement(Color.White));
                    scale.MajorTickmarks.StrokeElement = new StrokeElement(Color.DimGray);

                    scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 3));
                    scale.Labels.ZPosition = LinearTickmarkZPosition.AboveMarkers;
                    scale.Labels.Extent = 70;
                    Font font = new Font("Arial", 13);
                    scale.Labels.Font = font;
                    scale.Labels.EqualFontScaling = true;
                    
                    SolidFillBrushElement solidFillBrushElement = new SolidFillBrushElement(Color.White);
                    solidFillBrushElement.RelativeBoundsMeasure = Measure.Percent;
                    Rectangle rect = new Rectangle(0, 0, 80, 0);
                    solidFillBrushElement.RelativeBounds = rect;
                    scale.Labels.BrushElements.Add(solidFillBrushElement);
                    scale.Labels.Shadow.Depth = 2;
                    scale.Labels.FormatString = dtIndicators.Rows[i][5].ToString() == "тыс. руб"
                                     ? "<DATA_VALUE:N0>"
                                     : "<DATA_VALUE:N2>";
                    scale.Labels.EqualFontScaling = true;
                    scale.Labels.Shadow.BrushElements.Add(new SolidFillBrushElement());
                    linearGauge.Scales.Add(scale);

                    gradientBrush = new SimpleGradientBrushElement();
                    gradientBrush.EndColor = Color.FromArgb(150, 150, 150);
                    gradientBrush.StartColor = Color.Black;
                    gradientBrush.GradientStyle = Gradient.BackwardDiagonal;
                    linearGauge.BrushElements.Add(gradientBrush);

                    gauge.Gauges.Add(linearGauge);
                    cellDescription.Controls.Add(gauge);

                    Label indicatorName = new Label();
                    indicatorName.Text = string.Format("{0} ", dtIndicators.Rows[i][1].ToString().Split('(')[0]);
                    indicatorName.SkinID = "TableFont";
                    cellDescription.Controls.Add(indicatorName);
                    Label indicatorDescription = new Label();
                    indicatorDescription.Text = string.Format("{0}", dtIndicators.Rows[i][0]);
                    indicatorDescription.SkinID = "ServeText";
                    cellDescription.Controls.Add(indicatorDescription);
                    cellDescription.Width = 350;
                    cellDescription.VerticalAlign = VerticalAlign.Top;
                    row.Cells.Add(cellDescription);

                    TableCell cellValues = new TableCell();
                    cellValues.Width = 100;
                    
                    Label subjectName = new Label();
                    subjectName.SkinID = "InformationText";
                    subjectName.Text = dtIndicators.Rows[i][7].ToString();
                    Label value = new Label();
                    value.SkinID = "TableFont";
                    value.Text = dtIndicators.Rows[i][5].ToString() == "тыс. руб"
                                     ? string.Format("{0:N2}<br/>", dtIndicators.Rows[i][2])
                                     : string.Format("{0:N4}<br/>", dtIndicators.Rows[i][2]);
                    cellValues.VerticalAlign = VerticalAlign.Top;
                    cellValues.Controls.Add(value);
                    Label measure = new Label();
                    measure.Text = string.Format("{0}<br/>", dtIndicators.Rows[i][5]);
                    measure.SkinID = "ServeText";
                    cellValues.Controls.Add(measure);
                    Label condition = new Label();
                    condition.Text = string.Format("{0}{1}<br />", dtIndicators.Rows[i][3], dtIndicators.Rows[i][4]);
                    condition.SkinID = "ServeTextGreenYellow";
                    cellValues.Controls.Add(condition);

                    Label average = new Label();
                    average.Text = "Среднее:<br/>";
                    average.SkinID = "InformationText";
                    cellValues.Controls.Add(average);

                    Label Fo = new Label();
                    Fo.Text = string.Format("    {0} = ", dtIndicators.Rows[i][8]);
                    Fo.SkinID = "InformationText";
                    cellValues.Controls.Add(Fo);

                    Label avgFoValue = new Label();
                    avgFoValue.Text = dtIndicators.Rows[i][5].ToString() == "тыс. руб" ?
                        string.Format("{0:N2}<br/>", dtIndicators.Rows[i][9]) :
                        string.Format("{0:N4}<br/>", dtIndicators.Rows[i][9]);
                    avgFoValue.SkinID = "TableFont";
                    cellValues.Controls.Add(avgFoValue);

                    Label Rf = new Label();
                    Rf.Text = "    РФ = ";
                    Rf.SkinID = "InformationText";
                    cellValues.Controls.Add(Rf);

                    Label avgRfValue = new Label();
                    avgRfValue.Text = dtIndicators.Rows[i][5].ToString() == "тыс. руб" ?
                       string.Format("{0:N2}<br/>", dtIndicators.Rows[i][10]) :
                       string.Format("{0:N4}<br/>", dtIndicators.Rows[i][10]);
                    avgRfValue.SkinID = "TableFont";
                    cellValues.Controls.Add(avgRfValue);

                    cellValues.Style["border-right-style"] = "none";
                    row.Cells.Add(cellValues);
                    TableCell cellImage = new TableCell();
                    Image image = new Image();
                    image.ImageUrl = crime == 0
                                         ? "~/images/green.png"
                                         : "~/images/red.png";
                    cellImage.Controls.Add(image);
                    cellImage.VerticalAlign = VerticalAlign.Top;
                    cellImage.Style["border-left-style"] = "none";
                    row.Cells.Add(cellImage);
                    IndicatorsTable.Rows.Add(row);
                }
            }
        }
        
    }
}
