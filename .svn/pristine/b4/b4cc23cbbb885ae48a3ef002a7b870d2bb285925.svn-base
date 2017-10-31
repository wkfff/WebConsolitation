using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

using Infragistics.UltraChart.Core.Primitives;
using System.Xml;


using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
namespace Krista.FM.Server.Dashboards.reports.EO.EO_00060.EO_0000
{
    public partial class _default : CustomReportPage
    {
        string page_title = "Инвестиционный паспорт (Новоорский муниципальный район)";
        private String dir
        {
            get { return Server.MapPath("~") + "\\"; }
        }
        private String file_name = "data.xml";
        private String stuff_id = "default";
        string BN = "IE";
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender,e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            Label1.Text = page_title;
            Grid.DataBind();
            Grid.Width = 1230;
            UltraWebGrid1.DataBind();
            UltraWebGrid1.Width = 1230;
            WebPanel1.Width = UltraWebGrid1.Width;
            WebPanel2.Width = UltraWebGrid1.Width;
            WebPanel3.Width = UltraWebGrid1.Width;
            Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            UltraWebGrid1.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            if ((BN == "APPLEMAC-SAFARI")||(BN=="FIREFOX"))
            {
                Grid.Height = Grid.Rows.Count * 25;
            }
            else
            {
                Grid.Height = Grid.Rows.Count * 24;
            }
            
            if (BN == "FIREFOX")
            {
                UltraWebGrid1.Height = UltraWebGrid1.Rows.Count * 29;
            }
            if (BN=="IE")
            {
                UltraWebGrid1.Height = UltraWebGrid1.Rows.Count * 26;
            }
            if (BN == "APPLEMAC-SAFARI")
            {
                UltraWebGrid1.Height = UltraWebGrid1.Rows.Count * 27;
            }
            Grid.Rows[0].Cells[0].RowSpan = 2;
            Grid.Rows[Grid.Rows.Count - 1].Cells[0].ColSpan = 4;

        }
        public static String getLastBlock(String mdx_member)
        {
            if (mdx_member == null) return null;
            String[] list = mdx_member.Split('.');
            Int32 index = list.Length - 1;
            String total = list[index];
            total = total.Replace("[", "");
            total = total.Replace("]", "");
            return total;
        }
        private void loadFromXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(dir +"\\reports\\EO\\EO_0006\\EO_0000\\"+ file_name);
            XmlNode node = doc.SelectSingleNode(string.Format("//stuff[@id='{0}']", stuff_id));

            if (node != null)
            {
                String region_str = null;
                String[] list = node.InnerText.Split('\n');
                foreach (String str in list)
                {
                    region_str = str.Trim();
                    if (region_str == String.Empty) continue;
                    Label1.Text = Label1.Text + "df";
                  //  scombo.Items.Add(new ListItem(getLastBlock(region_str), region_str));
                }
            }
            else
            {
               // throw new Exception("Can't find StuffID: '" + stuff_id + "' in file" + file_name);
            }
        }

        





        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            XmlDocument aXmlDoc = new XmlDocument();
            DataSet aDataSet = new DataSet();
            aDataSet.ReadXml(dir + "\\reports\\EO\\EO_0006\\EO_0000\\data1.xml");
            aDataSet.Tables[0].Columns[0].ColumnName = "Параметры, характеризующие инвестиционную площадку";
            aDataSet.Tables[0].Columns[1].ColumnName = "Площадка №1";
            aDataSet.Tables[0].Columns[2].ColumnName = "Площадка №2";
            aDataSet.Tables[0].Columns[3].ColumnName = "Площадка №3";
            Grid.DataSource = aDataSet;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.34);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.2);
                e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.2);
                e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.2);
            }
            else
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.3);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.2);
                e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.2);
                e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.2);
            }
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            XmlDocument aXmlDoc = new XmlDocument();
            DataSet aDataSet = new DataSet();
            aDataSet.ReadXml(dir + "\\reports\\EO\\EO_0006\\EO_0000\\data.xml");
            aDataSet.Tables[0].Columns[0].ColumnName = "№п/п";
            aDataSet.Tables[0].Columns[1].ColumnName = "Полное наименование предприятия";
            aDataSet.Tables[0].Columns[2].ColumnName = "Адрес, тел./факс, e-mail";
            aDataSet.Tables[0].Columns[3].ColumnName = "Основные виды продукции";
            UltraWebGrid1.DataSource = aDataSet;
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (BN == "IE")
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.04);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.41);
                e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.19);
                e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.3);
            }
            else
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.04);
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.4);
                e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.16);
                e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(CustomReportConst.minScreenWidth * 0.3);
            }
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign= HorizontalAlign.Center;
            }
        }

        protected void WebPanel1_ExpandedStateChanged(object sender, Infragistics.WebUI.Misc.WebPanel.ExpandedStateChangedEventArgs e)
        {
        }

        protected void WebPanel1_ExpandedStateChanging(object sender, Infragistics.WebUI.Misc.WebPanel.ExpandedStateChangingEventArgs e)
        {
            e.Cancel = false;
        }

        protected void WebPanel2_ExpandedStateChanging(object sender, Infragistics.WebUI.Misc.WebPanel.ExpandedStateChangingEventArgs e)
        {
            e.Cancel = false;
        }

        protected void WebPanel3_ExpandedStateChanging(object sender, Infragistics.WebUI.Misc.WebPanel.ExpandedStateChangingEventArgs e)
        {
            e.Cancel = false;
        }
    }
}
