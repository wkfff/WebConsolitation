using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0014_2
{
    public partial class Default : GadgetControlBase, IHotReport
    {
        private DataTable dtChart1;

        private CustomParam bkkuDate;

        public int Width
        {
            get { return 260; }
        }

        public int Height
        {
            get { return 260; }
        }

        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                UltraChart1.Width = 230;
                UltraChart1.Height = CRHelper.GetChartHeight(270);


                // CRHelper.FillCustomColorModel(UltraChart1, 17, false);
                UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;

                UltraChart1.ChartType = ChartType.PieChart3D;
                UltraChart1.Border.Thickness = 0;
                //            UltraChart1.PieChart.ColumnIndex = 1;
                UltraChart1.PieChart3D.OthersCategoryPercent = 0;
               // UltraChart1.PieChart3D.Labels.Visible = false;
                UltraChart1.PieChart3D.Labels.Font = new Font("Arial", 9);
                UltraChart1.PieChart3D.Labels.FontColor = Color.FromArgb(85, 85, 85);

                UltraChart1.PieChart3D.Labels.LeaderLinesVisible = false;
                UltraChart1.PieChart3D.Labels.FormatString = "<PERCENT_VALUE:N2>%";
                UltraChart1.Tooltips.FormatString = "<DATA_VALUE:N2> тыс.руб.";//"<ITEM_LABEL>\nфакт <DATA_VALUE:N2> тыс.руб.\nдоля <PERCENT_VALUE:N2>%";
                
                UltraChart1.Tooltips.Font.Name = "Arial";
                UltraChart1.Tooltips.Font.Size = 9;
                UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

                UltraChart1.PieChart3D.RadiusFactor = 100;
                UltraChart1.PieChart3D.StartAngle = 300;
                UltraChart1.PieChart3D.BreakDistancePercentage = 3;

                UltraChart1.Transform3D.XRotation = -140;
                UltraChart1.Transform3D.YRotation = -50;
                UltraChart1.Transform3D.ZRotation = 0;

                UltraChart1.PieChart3D.BreakAllSlices = true;

                UltraChart1.Legend.Visible = true;
                UltraChart1.Legend.Location = LegendLocation.Bottom;
                UltraChart1.Legend.SpanPercentage = 30;
                UltraChart1.Legend.Margins.Left = -5;
                UltraChart1.Legend.BorderThickness = 0;
                UltraChart1.Legend.BackgroundColor = Color.White;

                UltraChart1.Legend.Font = new Font("Arial", 9);
                UltraChart1.Legend.FontColor = Color.FromArgb(85, 85, 85);

                UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);

                // CRHelper.FillCustomColorModel(UltraChart1, 17, false);
                UltraChart1.ColorModel.Skin.ApplyRowWise = true;

                string query = DataProvider.GetQueryText("FK_0001_0014_2_date", Server.MapPath("~/reports/FK_0001_0014_2/"));

                DataTable dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();

                Page.Title = "Основные показатели консолидированных бюджетов субъектов РФ";
                if (String.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    UserParams.StateArea.Value = "Ярославская область";
                }
                int year = endYear;
                UserParams.Region.Value = RegionsNamingHelper.GetFoBySubject(UserParams.StateArea.Value);
                reportDate = new DateTime(year, CRHelper.MonthNum(UserParams.PeriodMonth.Value), 1);
                UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate, 4);
                UserParams.Subject.Value =
                    string.Format("[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);

                bkkuDate = UserParams.CustomParam("bkkuDate");

                if (reportDate.Year == 2010)
                {
                    reportDate.AddYears(-1);
                }

                bkkuDate.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate, 4);
            }

            UltraChart1.DataBind();
        }

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                if (text.GetTextString().Contains("Безвозмездные"))
                {
                    text.SetTextString(String.Format("Безвозмездные поступления"));
                }
            }
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0014_2_chart1", Server.MapPath("~/reports/FK_0001_0014_2/"));
            dtChart1 = new DataTable("Incmomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            foreach (DataRow row in dtChart1.Rows)
            {
                row[0] = row[0].ToString().Replace("br", " ");
            }

            if (Request.Params["saveXml"] != null && Request.Params["saveXml"] == "y")
            {
                ReportObject reportObject = new ReportObject();
                reportObject.title = String.Format("Структура доходов бюджета за {0} {1} {2} года", reportDate.Month, CRHelper.RusManyMonthGenitive(reportDate.Month), reportDate.Year);
                reportObject.region = UserParams.StateArea.Value;
                reportObject.regionId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
                reportObject.link = "http://ifinmon.ru/reports/FK_0001_0004_1/DefaultDetail.aspx";

                reportObject.item = new List<ReportDataItem>();

                for (int i = 0; i < dtChart1.Rows.Count; i++)
                {
                    ReportDataItem item = new ReportDataItem();
                    item.name = dtChart1.Columns[1].ColumnName;
                    item.shortName = dtChart1.Rows[i][0].ToString();
                    item.value = dtChart1.Rows[i][1].ToString();
                    
                    item.shownValue =
                        (Convert.ToDouble(ReplaceDecimalSeparator(dtChart1.Rows[i][1].ToString())) /
                         (Convert.ToDouble(ReplaceDecimalSeparator(dtChart1.Rows[0][1].ToString())) + Convert.ToDouble(ReplaceDecimalSeparator(dtChart1.Rows[1][1].ToString())))).ToString("P2");
                    item.color = i == 1 ? "#477b02" : "#1a56d0";

                    reportObject.item.Add(item);
                }

                MemoryStream ms = new MemoryStream();
                XmlSerializer bf = new XmlSerializer(typeof(ReportObject));
                bf.Serialize(ms, reportObject);
                ms.Flush();
                
                StreamReader writer = new StreamReader(ms);
                ms.Position = 0;
                
                string xml = String.Empty;
                xml = writer.ReadToEnd();
                xml = xml.Replace(String.Format("<item>{0}", Environment.NewLine), "");
                xml = xml.Replace("</item>\r\n", "");
                xml = xml.Replace("ReportDataItem", "item");
                Response.Write(xml);
                writer.Close();
                Response.End();
            }

            UltraChart1.DataSource = dtChart1;
        }

        private string ReplaceDecimalSeparator(string value)
        {
            return value;
                value.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).
                    Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }

         //
         //                (Convert.ToDouble(dtChart1.Rows[0][1].ToString().Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)) + 
         //                Convert.ToDouble(dtChart1.Rows[1][1]).ToString().Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))).ToString("P2")


        public override string Title
        {
            get { return String.Format("Cтруктура доходной части бюджета {0}", RegionsNamingHelper.ShortName(UserParams.StateArea.Value)); }
            set { }
        }
    }

    public class ReportObject
    {
        public string title;
        public string link;
        public string region;
        public string regionId;
        public List<ReportDataItem> item;
    }

    public class ReportDataItem
    {
        public string name;
        public string shortName;
        public string value;
        public string shownValue;
        public string color;
    }
}
