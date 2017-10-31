using System;
using System.Data;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Text = Infragistics.UltraChart.Core.Primitives.Text;
using System.Drawing;
using System.Web;
using System.IO;
using System.Xml;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FST_0001_0003 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            SetChart(TagCloudFST_0001_0003_Text1, "электрическую энергию", "руб./кВт.ч.", "FST_0001_0003_chart_electro");
            SetChart(TagCloudFST_0001_0003_Text2, "тепловую энергию", "руб./Гкал.", "FST_0001_0003_chart");
            SetChart(TagCloudFST_0001_0003_Text3, "водоснабжение", "руб./куб.м.", "FST_0001_0003_chart");

            TagCloudFST_0001_0003_Text1.TaxName = "тариф для населения";
                        
            bool checkedAreaHeat = CheckedArea(((CustomParam)(HttpContext.Current.Session["state_area"])).Value, "~/FstRegionsHeat.xml");
            IPadElementHeader2.Text = checkedAreaHeat ? IPadElementHeader2.Text : IPadElementHeader2.Text += " (возможно уточнение тарифа)";

            bool checkedAreaWater = CheckedArea(((CustomParam)(HttpContext.Current.Session["state_area"])).Value, "~/FstRegionsWater.xml");
            IPadElementHeader3.Text = checkedAreaWater ? IPadElementHeader3.Text : IPadElementHeader3.Text += " (возможно уточнение тарифа)";
            
            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();

            IPadElementHeader1.MultitouchReport = String.Format("fst_0001_0013_{0}", CustomParams.GetSubjectIdByName(UserParams.StateArea.Value));
            IPadElementHeader2.MultitouchReport = String.Format("fst_0001_0023_{0}", CustomParams.GetSubjectIdByName(UserParams.StateArea.Value));
            IPadElementHeader3.MultitouchReport = String.Format("fst_0001_0033_{0}", CustomParams.GetSubjectIdByName(UserParams.StateArea.Value));
            IPadElementHeader4.MultitouchReport = String.Format("fst_0001_0043_{0}", CustomParams.GetSubjectIdByName(UserParams.StateArea.Value));
            //IPadElementHeader3.MultitouchReport = "FST_0001_0031";

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/FST_0001_0003/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/FST_0001_0003/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"fst_0001_0013_{0}\" bounds=\"x=0;y=0;width=768;height=235\" openMode=\"loaded\"/><element id=\"fst_0001_0023_{0}\" bounds=\"x=0;y=235;width=768;height=235\" openMode=\"loaded\"/><element id=\"fst_0001_0033_{0}\" bounds=\"x=0;y=470;width=768;height=235\" openMode=\"loaded\"/><element id=\"fst_0001_0043_{0}\" bounds=\"x=0;y=705;width=768;height=245\" openMode=\"loaded\"/></touchElements>", CustomParams.GetSubjectIdByName(UserParams.StateArea.Value)));
        }

        private void SetChart(Dashboard.FST_0001_0003_Original_Text textElement, string filter, string measure, string chartQueryName)
        {
            UserParams.Filter.Value = filter;
            CustomParam measureParam = UserParams.CustomParam("measure");
            measureParam.Value = measure;

            string chartQuery = DataProvider.GetQueryText(chartQueryName);
            string textQuery = DataProvider.GetQueryText("FST_0001_0003_text");

            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(chartQuery, "dummy", dtChart);
            DataTable dtText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(textQuery, "dummy", dtText);

            textElement.DtChart = dtChart;
            textElement.DtText = dtText;
        }

        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FST_0001_0003_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);
            
            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderLayout.Clear();

            e.Layout.Bands[0].Columns[0].Width = 500;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 260;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;           
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.ForeColor = Color.White;
            if (e.Row.Index > 0 && e.Row.Index < 5)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
                e.Row.Cells[0].Style.ForeColor = Color.FromArgb(0xd1d1d1);
                e.Row.Cells[0].Value = String.Format("<i>{0}</i>", e.Row.Cells[0].Value.ToString());
            }
        }

        #endregion

        private bool CheckedArea(string name, string fileName)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath(fileName);
            if (!File.Exists(xmlFile))
            {
                return true;
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);
                // Ищем узел регионов
                foreach (XmlNode rootNode in doc.ChildNodes)
                {
                    if (rootNode.Name == "RegionsList")
                    {
                        foreach (XmlNode regionNode in rootNode.ChildNodes)
                        {
                            if (regionNode.Attributes["name"].Value == name)
                            {
                                return true;
                            }
                        }
                    }
                }
            }           
            return false;
        }
    }
}
