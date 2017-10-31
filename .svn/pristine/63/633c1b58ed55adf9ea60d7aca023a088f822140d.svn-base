using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FK_0001_0001_Gadget : GadgetControlBase, IHotReport
    {
        public int Width
        {
            get { return 350; }
        }

        public int Height
        {
            get { return 300; }
        }

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

            UltraChart.Width = 307;
            string browser = HttpContext.Current.Request.Browser.Browser;
            switch (browser)
            {
                case ("Firefox"):
                    {
                        UltraChart.Height = 355;
                        break;
                    }
                case ("AppleMAC-Safari"):
                    {
                        UltraChart.Height = 354;
                        break;
                    }
                default:
                    {
                        UltraChart.Height = 355;
                        break;
                    }
            }
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            
            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart.Axis.Y.Extent = 80;
            UltraChart.Axis.Y2.Extent = 20;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            UltraChart.Axis.X.Visible = false;
            //UltraChart.Legend.Margins.Right = -2;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE_ITEM:P2>";

            UltraChart.Legend.MoreIndicatorText = " ";

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 68;
            UltraChart.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart_ChartDrawItem);
            UltraChart.DataBind();

            HyperLink1.Target = "_top";
            HyperLink2.Target = "_top";

            if (EmbeddedReport)
            {
               // UltraChart.Width = 410;
                UltraChart.Axis.Y.Extent = 100;
                UltraChart.Tooltips.Font.Name = "Arial";
                UltraChart.Tooltips.Font.Size = 9;
                UltraChart.Legend.Font = new Font("Arial", 9);
                UltraChart.Legend.FontColor = Color.FromArgb(85, 85, 85);
                UltraChart.Legend.BackgroundColor = Color.White;
                UltraChart.Legend.Margins.Left = -5;
                UltraChart.Legend.BorderThickness = 0;
                UltraChart.Legend.SpanPercentage = 75;
                UltraChart.Tooltips.FormatString = "<ITEM_LABEL><br/><DATA_VALUE_ITEM:P2>";
                HyperLink1.Target = "_blank";
                HyperLink2.Target = "_blank";

                UltraChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Arial", 9);
                UltraChart.Axis.Y.Labels.SeriesLabels.FontColor = Color.FromArgb(85, 85, 85);

                UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
                UltraChart.ColorModel.Skin.ApplyRowWise = false;
                UltraChart.ColorModel.Skin.PEs.Clear();
                for (int i = 1; i <= 10; i++)
                {
                    PaintElement pe = new PaintElement();
                    Color color = GetColor(i);
                    Color stopColor = GetStopColor(i);
                                        
                    pe.Fill = color;
                    pe.FillStopColor = stopColor;
                    pe.ElementType = PaintElementType.Gradient;
                    pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                    pe.FillOpacity = 150;
                    UltraChart.ColorModel.Skin.PEs.Add(pe); 
                }
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(110, 189, 241);
                    }
                case 2:
                    {
                        return Color.FromArgb(214, 171, 133);
                    }
                case 3:
                    {
                        return Color.FromArgb(141, 178, 105);
                    }
                case 4:
                    {
                        return Color.FromArgb(192, 178, 224);
                    }
                case 5:
                    {
                        return Color.FromArgb(245, 187, 102);
                    }
                case 6:
                    {
                        return Color.FromArgb(142, 164, 236);
                    }
                case 7:
                    {
                        return Color.FromArgb(217, 230, 117);
                    }
                case 8:
                    {
                        return Color.FromArgb(162, 154, 98);
                    }
                case 9:
                    {
                        return Color.FromArgb(143, 199, 219);
                    }
                case 10:
                    {
                        return Color.FromArgb(176, 217, 117);
                    }
            }
            return Color.White;
        }

        private static string GetColorString(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return "#6ebdf1";
                    }
                case 2:
                    {
                        return "#d6ab85";
                    }
                case 3:
                    {
                        return "#8db269";
                    }
                case 4:
                    {
                        return "#c0b2e0";
                    }
                case 5:
                    {
                        return "#f5bb66";
                    }
                case 6:
                    {
                        return "#8ea4ec";
                    }
                case 7:
                    {
                        return "#d9e675";
                    }
                case 8:
                    {
                        return "#a29a62";
                    }
                case 9:
                    {
                        return "#8ec7db";
                    }
                case 10:
                    {
                        return "#b0d975";
                    }
            }
            return "#FFFFFF";
        }

        private static Color GetStopColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(9, 135, 214);
                    }
                case 2:
                    {
                        return Color.FromArgb(138, 71, 10);
                    }
                case 3:
                    {
                        return Color.FromArgb(65, 124, 9);
                    }
                case 4:
                    {
                        return Color.FromArgb(44, 20, 91);
                    }
                case 5:
                    {
                        return Color.FromArgb(229, 140, 13);
                    }
                case 6:
                    {
                        return Color.FromArgb(11, 45, 160);
                    }
                case 7:
                    {
                        return Color.FromArgb(164, 184, 10);
                    }
                case 8:
                    {
                        return Color.FromArgb(110, 98, 8);
                    }
                case 9:
                    {
                        return Color.FromArgb(11, 100, 131);
                    }
                case 10:
                    {
                        return Color.FromArgb(102, 168, 9);
                    }
            }
            return Color.White;
        }

        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            Box box = e.Primitive as Box;
            if ((box != null) && !(string.IsNullOrEmpty(box.Path)) && 
                box.Path.EndsWith("Legend") && (box.rect.Width != box.rect.Height))
            {
              //  box.rect.Width = box.rect.Width + 4;
            }
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            CustomReportPage dashboard = GetCustomReportPage(this);

            DataTable dtDate = new DataTable();
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0001_date", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            Label1.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            dashboard.UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();

            HyperLink1.Text = "Сравнение структуры расходов субъектов РФ";
            HyperLink2.Text = string.Format("Подробнее {0}", dashboard.UserParams.StateArea.Value);
            HyperLink1.NavigateUrl = "~/reports/FK_0001_0001/DefaultCompare.aspx";
            HyperLink2.NavigateUrl = "~/reports/FK_0001_0001/DefaultDetail.aspx";
            
            query = DataProvider.GetQueryText("FK_0001_0001", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dt);

            if (dt.Rows.Count > 2)
            {
                dt.Rows[0][0] = string.Format("{0} факт", RegionsNamingHelper.ShortName(dashboard.UserParams.StateArea.Value));
                dt.Rows[1][0] = string.Format("{0} план", RegionsNamingHelper.ShortName(dashboard.UserParams.StateArea.Value));
                dt.Rows[2][0] = string.Format("{0}", RegionsNamingHelper.ShortName(dashboard.UserParams.Region.Value));
                dt.Rows[3][0] = "РФ";
            }

            foreach (DataColumn column in dt.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
                //if (EmbeddedReport &&
                //    column.ColumnName.Contains("безопасность"))
                //{
                //    column.ColumnName = "Нац.безоп. и правоохран. деят.";
                //}
            }
            dt.AcceptChanges();

            if (Request.Params["saveXml"] != null && Request.Params["saveXml"] == "y")
            {
                if (dt.Rows.Count > 2)
                {
                    dt.Rows[0][0] = string.Format("{0} факт", dashboard.UserParams.StateArea.Value);
                    dt.Rows[1][0] = string.Format("{0} план", dashboard.UserParams.StateArea.Value);
                    dt.Rows[2][0] = string.Format("{0}", dashboard.UserParams.Region.Value);
                    dt.Rows[3][0] = "РФ";
                }
                dt.AcceptChanges();
                ReportObject reportObject = new ReportObject();
                reportObject.title = String.Format("Структура расходов за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
                reportObject.region = UserParams.StateArea.Value;
                reportObject.regionId = HttpContext.Current.Session["CurrentSubjectID"].ToString();
                
                reportObject.link = "http://ifinmon.ru/reports/FK_0001_0001/DefaultCompare.aspx";
                reportObject.link1 = "http://ifinmon.ru/reports/FK_0001_0001/DefaultDetail.aspx";

                reportObject.seriesItem = new List<ReportSeriesItem>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ReportSeriesItem item = new ReportSeriesItem();
                    item.name = dt.Rows[i][0].ToString();
                    item.item = new List<ReportDataItem>();
                    for (int j = 1; j < dt.Columns.Count; j++ )
                    {
                        ReportDataItem dataItem = new ReportDataItem();
                        dataItem.value = dt.Rows[i][j].ToString();
                        dataItem.name = dt.Columns[j].ColumnName;
                        dataItem.color = GetColorString(j);
                        item.item.Add(dataItem);
                        
                    }

                    reportObject.seriesItem.Add(item);
                }

                MemoryStream ms = new MemoryStream();
                XmlSerializer bf = new XmlSerializer(typeof(ReportObject));
                bf.Serialize(ms, reportObject);
                ms.Flush();

                StreamReader writer = new StreamReader(ms);
                ms.Position = 0;

                string xml = String.Empty;
                xml = writer.ReadToEnd();
                xml = xml.Replace("link1","link");
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

            UltraChart.DataSource = dt;
        }

        #region IWebPart Members

        public override string Description
        {
            get { return "Раздел содержит данные Федерального казначейства об исполнении бюджетов субъектов РФ по расходам"; }
        }

        public override string Title
        {
            get { return "Структура расходов"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FK_0001_0001/DefaultCompare.aspx"; }
        }

        #endregion

        public class ReportObject
        {
            public string title;
            public string link;
            public string link1;
            public string region;
            public string regionId;
            public List<ReportSeriesItem> seriesItem;
        }

        public class ReportDataItem
        {
            public string name;
            public string value;
            public string color;
        }

        public class ReportSeriesItem
        {
            public string name;
            public List<ReportDataItem> item;
        }
    }
}