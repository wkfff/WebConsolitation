using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FZ_0083_0002 : CustomReportPage
    {
        private GridHeaderLayout headerLayout;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            //CustomParams.MakeRegionParams("5", "id");

            UltraWebGrid.DataBind();
            UltraWebGrid.Visible = false;
        }

        private DataTable LoadIndicators()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/FZ_0083_0001/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["table"];
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtGrid = LoadIndicators();

            dtGrid.Rows.RemoveAt(0);

            DataTable dtSorted = dtGrid.Clone();
            AddSortedRows(dtGrid, dtSorted);

            UltraWebGrid.DataSource = dtSorted;
        }

        private void AddSortedRows(DataTable dtGrid, DataTable dtSorted)
        {
            for (int rowCount = 0; rowCount < dtGrid.Rows.Count; rowCount++)
            {                
                if (UserParams.StateArea.Value == Regex.Replace(dtGrid.Rows[rowCount][0].ToString(), "<[\\s\\S]*?>", String.Empty))
                {
                    DataRow row = dtGrid.Rows[rowCount];
                    row[0] = row["npa"] != DBNull.Value && row["npa"].ToString() != String.Empty ?
                                String.Format("<a href='{0}'>{1}</a>", row["npa"], row["hint"]) :
                                row["hint"];
                    dtSorted.ImportRow(dtGrid.Rows[rowCount]);
                }
            }
        }

              
        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            
        }       


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0] != null &&
                    e.Row.Cells[0].Value != null &&
                 e.Row.Cells[0].Value.ToString() != String.Empty)
            {
                for (int i = 1; i < e.Row.Cells.Count - 1; i++)
                {
                    if (e.Row.Cells[i] != null &&
                        e.Row.Cells[i].Value != null)
                    {
                        string period = String.Empty;
                        if (i == 1)
                        {
                            period = "01.01.2011";
                        }
                        if (i == 3)
                        {
                            period = "01.04.2011";
                        }
                        if (i == 5)
                        {
                            period = "01.10.2011";
                        }
                        if (i == 7)
                        {
                            period = "01.01.2012";
                        }
                        if (i == 9)
                        {
                            period = "01.07.2012";
                        }
                        string value = e.Row.Cells[i].Value.ToString();
                        e.Row.Cells[i].Value = String.Empty;

                        if (value.Contains("A"))
                        {
                            lbDescription.Text += String.Format("<table Class='InformationTextPopup'><tr><td><img src='../../../images/A.png'/></td><td>&nbsp;<span class='TableFontPopup'><b>{0}:</b></span>&nbsp;<span class='TableFontPopup'>Перевод всех бюджетных учреждений на субсидии</span></td></tr></table>", period);
                        }
                        if (value.Contains("C1"))
                        {
                            lbDescription.Text += String.Format("<table Class='InformationTextPopup'><tr><td><img src='../../../images/B1.png'/></td><td>&nbsp;<span class='TableFontPopup'><b>{0}:</b></span>&nbsp;<span class='TableFontPopup'>Окончание действия временного порядка использования казенными учреждениями доходов от аренды</span></td></tr></table>", period);
                        }
                        if (value.Contains("C2"))
                        {
                            lbDescription.Text += String.Format("<table Class='InformationTextPopup'><tr><td><img src='../../../images/B2.png'/></td><td>&nbsp;<span class='TableFontPopup'><b>{0}:</b></span>&nbsp;<span class='TableFontPopup'>Окончание действия временного порядка использования бюджетными учреждениями доходов от аренды</span></td></tr></table>", period);
                        }
                        if (value.Contains("B1"))
                        {
                            lbDescription.Text += String.Format("<table Class='InformationTextPopup'><tr><td><img src='../../../images/C1.png'/></td><td>&nbsp;<span class='TableFontPopup'><b>{0}:</b></span>&nbsp;<span class='TableFontPopup'>Окончание действия временного порядка использования казенными учреждениями средств от приносящей доход деятельности</span></td></tr></table>", period);
                        }
                        if (value.Contains("B2"))
                        {
                            lbDescription.Text += String.Format("<table Class='InformationTextPopup'><tr><td><img src='../../../images/C2.png'/></td><td>&nbsp;<span class='TableFontPopup'><b>{0}:</b></span>&nbsp;<span class='TableFontPopup'>Окончание действия временного порядка использования бюджетными учреждениями средств от приносящей доход деятельности</span></td></tr></table>", period);
                        }
                    }
                }

                lbDescription.Text += String.Format("<table Class='InformationTextPopup'><tr><td>{0}</td></tr></table>", e.Row.Cells[0].Value);
                
            }
            
        }
    }
}