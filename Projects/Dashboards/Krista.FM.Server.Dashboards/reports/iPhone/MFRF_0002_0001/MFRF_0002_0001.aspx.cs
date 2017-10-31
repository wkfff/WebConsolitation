using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0002_0001 : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            InitializeBkku();
        }

        #region БККУ

        private void InitializeBkku()
        {
            SetIndicatorsData();
        }

        private DataTable dtDateYear = new DataTable();
        private DataTable dtDateMonths = new DataTable();

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

            if (mbtGroup.Rows[0][1] != DBNull.Value)
            {
                Rank.Text = string.Format("&nbsp;{0:N0}", mbtGroup.Rows[0][1]);
                Rank.CssClass = "DigitsValue";

                lbRankDescription.Text = GetMbtDescription((int)Convert.ToDouble(mbtGroup.Rows[0][1].ToString()));
                lbRankDescription.CssClass = "ServeText";

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
        }

        #endregion
    }
}
