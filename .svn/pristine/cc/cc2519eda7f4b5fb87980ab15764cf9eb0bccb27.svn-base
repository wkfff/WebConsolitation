using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.DashboardFederal.Dashboard.reports.DashboardFederal
{
    public partial class MFRF_0001_0001_Gadget : GadgetControlBase, IHotReport
    {
        public int Width
        {
            get { return 410; }
        }

        private DataTable dt = new DataTable();
        private DataTable mbtGroup = new DataTable();

        private bool EmbeddedReport
        {
            get
            {
                // Да, если это указано в урле или сессии
                return (Request.Params["embedded"] != null &&
                        Request.Params["embedded"].ToLower() == "yes") ||
                       (Session["Embedded"] != null &&
                        (bool)Session["Embedded"]);
            }
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            HyperLink1.Text = "Оценка дефицита бюджетов";
            HyperLink1.NavigateUrl = "~/reports/FK_0001_0011/DefaultAssessment.aspx";

            HyperLink2.Text = "Мониторинг соблюдения требований БК и КУ";
            HyperLink2.NavigateUrl = TitleUrl;
            InitializeBkku();

            if (EmbeddedReport)
            {
                divMain.Attributes.Add("class", "bujetText");
                divMain.Style.Add("Width", "410px");
                divMain.Style.Add("padding-left", "10px");
            }
            else
            {
                divMain.Attributes.Add("class", "GadgetGridTD");
            }
        }

         #region БККУ

        private void InitializeBkku()
        {
            SetIndicatorsData();
        }

        private DataTable dtDateYear = new DataTable();
        private DataTable dtDateMonths = new DataTable();

        private static string GetMbtDescription(int group)
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
                        return "&nbsp;";
                    }
            }
        }

        private void SetIndicatorsData()
        {
            //IndicatorsTable.Style.Add("border-collapse", "collapse");

            SetDateParams("bk");

            mbtGroup = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_mbtGroup", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, mbtGroup);

            lbRankCaption.Text = String.Format("{0}: ", mbtGroup.Rows[0][0]);

            if (mbtGroup.Rows[0][1] != DBNull.Value)
            {
                Rank.Text = string.Format("&nbsp;{0:N0}", mbtGroup.Rows[0][1]);
                
                lbRankDescription.Text = GetMbtDescription((int)Convert.ToDouble(mbtGroup.Rows[0][1].ToString()));
                
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
                    default:
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

            query = DataProvider.GetQueryText("iPad_0001_0001_crimes", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Нарушений", dt);

            CrimesBKTitle.Text = String.Format("{0} в {1}:&nbsp;", "Количество нарушений требований бюджетного кодекса РФ", RegionsNamingHelper.ShortName(UserParams.StateArea.Value));
            CrimesBK.Text = dt.Rows[0][1].ToString();
            imgCrimesBK.ImageUrl = dt.Rows[0][1].ToString() == "0" ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";

            DataTable dtIndicators = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_Bk", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);

            SetDateParams("ku");

            query = DataProvider.GetQueryText("iPad_0001_0001_crimes", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Нарушений", dt);

            CrimesKUTitle.Text =
                String.Format("{0} в {1}:&nbsp;", "Количество нарушений условий качества управления бюджетами", RegionsNamingHelper.ShortName(UserParams.StateArea.Value));
            CrimesKU.Text = dt.Rows[0][2].ToString();
            imgCrimesKU.ImageUrl = dt.Rows[0][2].ToString() == "0" ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";

            if (Request.Params["saveXml"] != null && Request.Params["saveXml"] == "y")
            {
                dt.AcceptChanges();
                ReportObject reportObject = new ReportObject();
                reportObject.title = "Индикаторы БККУ";
                reportObject.region = UserParams.StateArea.Value;
                reportObject.regionId = HttpContext.Current.Session["CurrentSubjectID"].ToString();

                reportObject.link = "http://ifinmon.ru/reports/MFRF_0002_0002/Default.aspx";
                reportObject.link1 = "http://ifinmon.ru/reports/FK_0001_0011/DefaultAssessment.aspx";

                reportObject.seriesItem = new List<ReportDataItem>();
                                
                ReportDataItem item = new ReportDataItem();
                item.name = CrimesBKTitle.Text;
                item.value = CrimesBK.Text;
                item.group = imgCrimesBK.ImageUrl == "~/images/ballGreenBB.png" ? "4" : "1";
                item.description = periodBK.Text;
                reportObject.seriesItem.Add(item);

                item = new ReportDataItem();
                item.name = CrimesKUTitle.Text;
                item.value = CrimesKU.Text;
                item.group = imgCrimesKU.ImageUrl == "~/images/ballGreenBB.png" ? "4" : "1";
                item.description = periodKU.Text;
                reportObject.seriesItem.Add(item);

                item = new ReportDataItem();
                item.name = mbtGroup.Rows[0][0].ToString();
                item.value = mbtGroup.Rows[0][1].ToString();
                item.group = mbtGroup.Rows[0][1].ToString();
                item.description = GetMbtDescription((int)Convert.ToDouble(mbtGroup.Rows[0][1].ToString()));
                reportObject.seriesItem.Add(item);
                

                MemoryStream ms = new MemoryStream();
                XmlSerializer bf = new XmlSerializer(typeof(ReportObject));
                bf.Serialize(ms, reportObject);
                ms.Flush();

                StreamReader writer = new StreamReader(ms);
                ms.Position = 0;

                string xml = String.Empty;
                xml = writer.ReadToEnd();
                xml = xml.Replace("link1", "link");
                xml = xml.Replace(String.Format("<item>{0}", Environment.NewLine), "");
                xml = xml.Replace("</item>\r\n", "");

                xml = xml.Replace(String.Format("<seriesItem>{0}", Environment.NewLine), "");
                xml = xml.Replace("</seriesItem>\r\n", "");

                xml = xml.Replace("ReportDataItem", "item");
                xml = xml.Replace("ReportSeriesItem", "item");

                Response.Write(xml);
                writer.Close();
                Response.End();
            }

            dtIndicators = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_ku", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtIndicators);
        }

        private void SetDateParams(string queryPosrfix)
        {
            dt = new DataTable();

            string query = DataProvider.GetQueryText(String.Format("iPad_0001_0001_date_year_{0}", queryPosrfix), Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateYear);

            query = DataProvider.GetQueryText(String.Format("iPad_0001_0001_date_months_{0}", queryPosrfix), Server.MapPath("~/reports/DashboardFederal/"));
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
        }

        #endregion

        #region IWebPart Members

        public override string Description
        {
            get { return ""; }
        }

        public override string Title
        {
            get { return "Индикаторы БККУ"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/MFRF_0002_0002/Default.aspx"; }
        }
        #endregion

        public class ReportObject
        {
            public string title;
            public string link;
            public string link1;
            public string region;
            public string regionId;
            public List<ReportDataItem> seriesItem;
        }

        public class ReportDataItem
        {
            public string name;
            public string value;
            public string group;
            public string description;
        }
    }
}