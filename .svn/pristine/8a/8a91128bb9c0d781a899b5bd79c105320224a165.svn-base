using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image=System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0002_0001_V : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
          /*  string query = DataProvider.GetQueryText("MFRF_0002_0001_date_year");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateYear);

            query = DataProvider.GetQueryText("MFRF_0002_0001_date_months");
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
            } */

            InitializeBkku();

            Label.Visible = false;

            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        private DataTable dt = new DataTable();
        private DataTable dtDateYear = new DataTable();
        private DataTable dtDateMonths = new DataTable();

        #region БККУ

        private void InitializeBkku()
        {
            SetIndicatorsData();
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
            dt = new DataTable();

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

            DataTable mbtGroup = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_mbtGroup");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, mbtGroup);

            lbRankCaption.Text = String.Format("{0}: ", mbtGroup.Rows[0][0]);
            lbRankCaption.CssClass = "ServeText";

            string imageUrl = String.Empty;
            switch (mbtGroup.Rows[0][1].ToString())
            {
                case "1":
                    {
                        imageUrl = "~/images/ballRedBB.png";
                        break;
                    }
                case "2":
                    {
                        imageUrl = "~/images/ballOrangeBB.png";
                        break;
                    }
                case "3":
                    {
                        imageUrl = "~/images/ballYellowBB.png";
                        break;
                    }
                case "4":
                    {
                        imageUrl = "~/images/ballGreenBB.png";
                        break;
                    }
            }

            imgMbt.ImageUrl = imageUrl;

            if (mbtGroup.Rows[0][1] != DBNull.Value)
            {
                Rank.Text = string.Format("&nbsp;{0:N0}", mbtGroup.Rows[0][1]);
                Rank.CssClass = "DigitsValue";

                lbRankDescription.Text = GetMbtDescription((int)Convert.ToDouble(mbtGroup.Rows[0][1].ToString()));
                lbRankDescription.CssClass = "ServeText";
            }
            else
            {
                Rank.Text = String.Empty;
                lbRankCaption.Text = String.Empty;
                lbRankDescription.Text = String.Empty;
            }

            query = DataProvider.GetQueryText("iPad_0001_0001_crimes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Нарушений", dt);

            CrimesBKTitle.Text =
                String.Format("{0} в {1}:&nbsp;", CrimesBKTitle.Text, RegionsNamingHelper.ShortName(UserParams.StateArea.Value));
            CrimesBK.Text = dt.Rows[0][1].ToString();
            imgCrimesBK.ImageUrl = dt.Rows[0][1].ToString() == "0" ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";

            DataTable dtIndicators = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_Bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);

            for (int i = 0; i < dtIndicators.Rows.Count; i++)
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
                value.SkinID = "TableFont";
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
                cellValues.Controls.Add(condition);
                cellValues.Style["border-right-style"] = "none";
                row.Cells.Add(cellValues);
                cellImage = new TableCell();
                Image image = new Image();
                image.ImageUrl = "~/images/ballRedBB.png";
                cellImage.Controls.Add(image);
                cellImage.VerticalAlign = VerticalAlign.Top;
                cellImage.Style["border-left-style"] = "none";
                row.Cells.Add(cellImage);
                IndicatorsTable.Rows.Add(row);
            }

            dt = new DataTable();

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

            query = DataProvider.GetQueryText("iPad_0001_0001_crimes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Нарушений", dt);

            CrimesKUTitle.Text =
                String.Format("{0} в {1}:&nbsp;", CrimesKUTitle.Text, RegionsNamingHelper.ShortName(UserParams.StateArea.Value));
            CrimesKU.Text = dt.Rows[0][2].ToString();
            imgCrimesKU.ImageUrl = dt.Rows[0][2].ToString() == "0" ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";



            dtIndicators = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_ku");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);

            for (int i = 0; i < dtIndicators.Rows.Count; i++)
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
                cellDescription.Width = 566;
                cellDescription.VerticalAlign = VerticalAlign.Top;
                row.Cells.Add(cellDescription);

                cellValues = new TableCell();
                cellValues.Width = 150;
                value = new Label();
                value.SkinID = "TableFont";
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
                cellValues.Controls.Add(condition);
                cellValues.Style["border-right-style"] = "none";
                row.Cells.Add(cellValues);
                cellImage = new TableCell();
                Image image = new Image();
                image.ImageUrl = "~/images/ballRedBB.png";
                cellImage.Controls.Add(image);
                cellImage.VerticalAlign = VerticalAlign.Top;
                cellImage.Style["border-left-style"] = "none";
                row.Cells.Add(cellImage);
                IndicatorsTable.Rows.Add(row);
            }
        }

        #endregion
    }
}
