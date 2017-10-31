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
    public partial class MFRF_0002_0001_H : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            SetIndicatorsData();
        }

        private string GetMbtDescription(int group)
        {
            switch (group)
            {
                case 1:
                    {
                        return "(доля МБТ более 60%)";
                    }
                case 2:
                    {
                        return "(доля МБТ от 20% до 60%)";
                    }
                case 3:
                    {
                        return "(доля МБТ от 5% до 20%)";
                    }
                case 4:
                    {
                        return "(доля МБТ менее 5%)";
                    }
                default:
                    {
                        return "(доля МБТ более 60%)";
                    }
            }
        }

        private DataTable dtIndicators = new DataTable();
        private DataTable dtDateYear = new DataTable();
        private DataTable dtDateMonths = new DataTable();

        private void SetIndicatorsData()
        {
            string query = DataProvider.GetQueryText("iPad_0001_0001_date_year_bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateYear);

            query = DataProvider.GetQueryText("iPad_0001_0001_date_months_bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateMonths);

            if (Convert.ToInt32(dtDateMonths.Rows[0][0].ToString()) > Convert.ToInt32(dtDateYear.Rows[0][0].ToString()))
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateMonths.Rows[0][4]);
                UserParams.Filter.Value = "По данным месячной отчетности";
            }
            else
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateYear.Rows[0][1]);
                UserParams.Filter.Value = "По данным годовой отчетности";
            }

            LabelState.Text = UserParams.StateArea.Value;
                        
            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            query = DataProvider.GetQueryText("MFRF_0002_0001_H_bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);

            TableRow rowHeader = new TableRow();
            TableCell cell = new TableCell();
            cell.Width = 220;
            cell.Height = 25;
            Label text = new Label();
            text.Text = "Индикатор";
            text.SkinID = "InformationText";
            cell.Controls.Add(text);
            cell.BackColor = Color.FromArgb(51, 51, 51);
            rowHeader.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 70;
            cell.Height = 25;
            text = new Label();
            text.Text = "Значение";
            text.SkinID = "InformationText";
            cell.Controls.Add(text);
            cell.Style["border-right-style"] = "none";
            cell.BackColor = Color.FromArgb(51, 51, 51);
            rowHeader.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 20;
            cell.Height = 25;
            cell.Style["border-left-style"] = "none";
            rowHeader.Cells.Add(cell);
            cell.BackColor = Color.FromArgb(51, 51, 51);
            IndicatorsTable.Rows.Add(rowHeader);

            cell = new TableCell();
            cell.Width = 110;
            cell.Height = 25;
            text = new Label();
            text.Text = "Среднее";
            text.SkinID = "InformationText";
            cell.Controls.Add(text);
            cell.BackColor = Color.FromArgb(51, 51, 51);
            rowHeader.Cells.Add(cell);

            DataTable mbtGroup = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_mbtGroup");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, mbtGroup);
            
            FillIndicatorsTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_date_year_ku");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateYear);

            query = DataProvider.GetQueryText("iPad_0001_0001_date_months_ku");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateMonths);

            if (Convert.ToInt32(dtDateMonths.Rows[0][0].ToString()) > Convert.ToInt32(dtDateYear.Rows[0][0].ToString()))
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateMonths.Rows[0][4]);
                UserParams.Filter.Value = "По данным месячной отчетности";
            }
            else
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateYear.Rows[0][1]);
                UserParams.Filter.Value = "По данным годовой отчетности";
            }
            query = DataProvider.GetQueryText("MFRF_0002_0001_H_ku");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);
            FillIndicatorsTable();
        }

        private void FillIndicatorsTable()
        {
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
                    indicatorName.SkinID = "TableFont";
                    cellDescription.Controls.Add(indicatorName);
                    Label indicatorDescription = new Label();
                    indicatorDescription.Text = string.Format("{0}", dtIndicators.Rows[i][0]);
                    indicatorDescription.SkinID = "ServeText";
                    cellDescription.Controls.Add(indicatorDescription);
                    cellDescription.Width = 220;
                    cellDescription.VerticalAlign = VerticalAlign.Top;
                    row.Cells.Add(cellDescription);

                    cellValues = new TableCell();
                    cellValues.Width = 70;

                    Label subjectName = new Label();
                    subjectName.SkinID = "InformationText";
                    subjectName.Text = dtIndicators.Rows[i][7].ToString();
                    value = new Label();
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
                    condition.Text = string.Format("{0} {1:N2}<br />", dtIndicators.Rows[i][3], Convert.ToDouble(dtIndicators.Rows[i][4]));
                    condition.SkinID = "ServeTextGreenYellow";
                    cellValues.Controls.Add(condition);
                    cellValues.Style["border-right-style"] = "none";
                    row.Cells.Add(cellValues);

                    cellImage = new TableCell();
                    Image image = new Image();
                    image.ImageUrl = crime == 0
                                         ? "~/images/ballGreenBB.png"
                                         : "~/images/ballRedBB.png";
                    cellImage.Controls.Add(image);
                    cellImage.VerticalAlign = VerticalAlign.Top;
                    cellImage.Style["border-left-style"] = "none";
                    row.Cells.Add(cellImage);

                    TableCell cellStats = new TableCell();
                    cellStats.Width = 150;
                    cellStats.VerticalAlign = VerticalAlign.Top;
//                    Label average = new Label();
//                    average.Text = "Среднее:<br/>";
//                    average.SkinID = "InformationText";
//                    cellStats.Controls.Add(average);

                    Label Fo = new Label();
                    Fo.Text = string.Format("    {0} = ", dtIndicators.Rows[i][8]);
                    Fo.SkinID = "InformationText";
                    cellStats.Controls.Add(Fo);

                    Label avgFoValue = new Label();
                    avgFoValue.Text = dtIndicators.Rows[i][5].ToString() == "тыс. руб"
                                          ?
                                      string.Format("{0:N2}<br/>", dtIndicators.Rows[i][9])
                                          :
                                      string.Format("{0:N4}<br/>", dtIndicators.Rows[i][9]);
                    avgFoValue.SkinID = "InformationText";
                    cellStats.Controls.Add(avgFoValue);

                    Label Rf = new Label();
                    Rf.Text = "    РФ = ";
                    Rf.SkinID = "InformationText";
                    cellStats.Controls.Add(Rf);

                    Label avgRfValue = new Label();
                    avgRfValue.Text = dtIndicators.Rows[i][5].ToString() == "тыс. руб"
                                          ?
                                      string.Format("{0:N2}<br/>", dtIndicators.Rows[i][10])
                                          :
                                      string.Format("{0:N4}<br/>", dtIndicators.Rows[i][10]);
                    avgRfValue.SkinID = "InformationText";
                    cellStats.Controls.Add(avgRfValue);
                    row.Cells.Add(cellStats);

                    IndicatorsTable.Rows.Add(row);
                }
            }
        }
    }
}
