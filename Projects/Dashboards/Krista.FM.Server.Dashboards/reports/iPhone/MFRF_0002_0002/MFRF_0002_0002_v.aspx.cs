using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Image=System.Web.UI.WebControls.Image;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0002_0002_V : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string query = DataProvider.GetQueryText("MFRF_0002_0002_date_year");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateYear);

            query = DataProvider.GetQueryText("MFRF_0002_0002_date_months");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateMonths);

            if (Convert.ToInt32(dtDateMonths.Rows[0][0].ToString()) > Convert.ToInt32(dtDateYear.Rows[0][0].ToString()))
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateMonths.Rows[0][4]);
                UserParams.Filter.Value = "По данным месячной отчетности";
                Label.Text = string.Format("данные за {0} квартал {1} года", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(dtDateMonths.Rows[0][3].ToString().ToLower())), dtDateMonths.Rows[0][0]);
            }
            else
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateYear.Rows[0][1]);
                UserParams.Filter.Value = "По данным годовой отчетности";
                Label.Text = string.Format("данные за {0} год", dtDateMonths.Rows[0][0]);
            }

            SetStatsData();
            SetIndicatorsData();
                        
            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        private DataTable dt = new DataTable();
        private DataTable dtDateYear = new DataTable();
        private DataTable dtDateMonths = new DataTable();

        private void SetStatsData()
        {
            string query = DataProvider.GetQueryText("MFRF_0002_0002_crimes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Нарушений", dt);
            
            #region первая строка
            TableRow row = new TableRow();

            TableCell cell = new TableCell();
            Label label = new Label();
            label.Text = "Нарушений";
            label.CssClass = "ServeText";
            cell.Width = 83;
            cell.Controls.Add(label);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 90;
            label = new Label();
            label.ForeColor = Color.White;
            label.Font.Name = "Arial";
            label.Font.Size = FontUnit.Parse("14px");
            label.Font.Bold = true;
            label.Text = dt.Rows[0][3].ToString();
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 68;
            label = new Label();
            label.CssClass = "ServeText";
            label.Text = string.Format("Среднее {0}", dt.Rows[0][4]);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 68;
            label = new Label();
            label.CssClass = "ServeText";
            label.Text = "Среднее РФ";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);
            
            statsTable.Rows.Add(row);

            #endregion

            #region третья строка
            row = new TableRow();

            cell = new TableCell();
            label = new Label();
            label.Text = "КУ";
            label.CssClass = "InformationText";
            cell.Width = 83;
            cell.Controls.Add(label);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 90;
            label = new Label();
            label.CssClass = "DigitsValueSmall";
            label.Text = dt.Rows[0][2].ToString();
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 68;
            label = new Label();
            label.CssClass = "InformationText";
            double value;
            string cellValue = String.Empty;
            if (double.TryParse(dt.Rows[1][2].ToString(), out value))
            {
                if (value != Double.NaN)
                {
                    cellValue = value.ToString("N2");
                }
            }
            label.Text = cellValue == "NaN" ? String.Empty : cellValue;
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 68;
            label = new Label();
            label.CssClass = "InformationText";
            if (double.TryParse(dt.Rows[2][2].ToString(), out value))
            {
                if (value != Double.NaN)
                {
                    cellValue = value.ToString("N2");
                }
            }
            label.Text = cellValue == "NaN" ? String.Empty : cellValue;
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Controls.Add(label);
            row.Cells.Add(cell);


            statsTable.Rows.Add(row);

            #endregion
        }

        private string GetMbtDescription(int group)
        {
            switch (group)
            {
                case 1:
                    {
                        return "&nbsp;(доля МБТ более 60%)";
                    }
                case 2:
                    {
                        return "&nbsp;(доля МБТ от 20% до 60%)";
                    }
                case 3:
                    {
                        return "&nbsp;(доля МБТ от 5% до 20%)";
                    }
                case 4:
                    {
                        return "&nbsp;(доля МБТ менее 5%)";
                    }
                default:
                    {
                        return "&nbsp;(доля МБТ более 60%)";
                    }
            }
        }

        private void SetIndicatorsData()
        {
            DataTable mbtGroup = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0002_0002_mbtGroup");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, mbtGroup);

            DataTable dtIndicators = new DataTable();
            query = DataProvider.GetQueryText("MFRF_0002_0002_V");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);

            for (int i = 0; i < dtIndicators.Rows.Count; i++)
            {
                double crime = 1;
                if (Double.TryParse(dtIndicators.Rows[i][6].ToString(), out crime))
                {
                    TableRow row;
                    TableCell cellDescription;
                    Label indicatorName;
                    TableCell cellValues;
                    Label value;
                    TableCell cellImage;

                    row = new TableRow();

                    cellDescription = new TableCell();

                    indicatorName = new Label();
                    indicatorName.Text = string.Format("{0} ", dtIndicators.Rows[i][1].ToString().Split('(')[0]);
                    indicatorName.CssClass = "TableFont";
                    cellDescription.Controls.Add(indicatorName);
                    Label indicatorDescription = new Label();
                    indicatorDescription.Text = string.Format("{0}", dtIndicators.Rows[i][0]);
                    indicatorDescription.CssClass = "ServeText";
                    cellDescription.Controls.Add(indicatorDescription);
                    cellDescription.Width = 222;
                    cellDescription.VerticalAlign = VerticalAlign.Top;
                    row.Cells.Add(cellDescription);

                    cellValues = new TableCell();
                    cellValues.Width = 77;
                    value = new Label();
                    value.CssClass = "TableFont";
                    value.Text = dtIndicators.Rows[i][5].ToString() == "тыс. руб"
                                     ? string.Format("{0:N2}<br/>", dtIndicators.Rows[i][2])
                                     : string.Format("{0:N4}<br/>", dtIndicators.Rows[i][2]);
                    cellValues.VerticalAlign = VerticalAlign.Top;
                    cellValues.Controls.Add(value);
                    Label measure = new Label();
                    measure.Text = string.Format("{0}<br/>", dtIndicators.Rows[i][5]);
                    measure.CssClass = "ServeText";
                    cellValues.Controls.Add(measure);
                    Label condition = new Label();
                    condition.Text = string.Format("{0} {1:N2}", dtIndicators.Rows[i][3], Convert.ToDouble(dtIndicators.Rows[i][4]));
                    condition.CssClass = "ServeTextGreenYellow";
                    cellValues.Style["border-right-style"] = "none";
                    cellValues.Controls.Add(condition);
                    row.Cells.Add(cellValues);
                    cellImage = new TableCell();
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
