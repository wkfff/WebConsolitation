using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Xml;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0002_wm2 : CustomReportPage
    {
        private DataTable dt;
        private DataTable newDt;
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            //UserParams.ShortStateArea.Value = "АлтсК";
            //UserParams.StateArea.Value = "Алтайский край";
            //UserParams.Region.Value = "Сибирский федеральный округ";
            //UserParams.ShortRegion.Value = "СФО";

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = dtDate.Rows[0][0].ToString();
            UserParams.PeriodYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) + 1).ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr = CRHelper.RusManyMonthGenitive(monthNum);

            Label1.Font.Size = 9;

             Label1.Text = string.Format("Доходы {0} за {1} {2} {3}г.",
                            UserParams.ShortStateArea.Value,
                            monthNum,
                            monthStr,
                            yearNum);

            FillData();
            SetDataTable(newDt);
        }

        protected void FillData()
        {
            dt = new DataTable();

            newDt = new DataTable();
            newDt.Columns.Add("column1", typeof (string));
            newDt.Columns.Add("column2", typeof (double));

            string query = DataProvider.GetQueryText("FK_0001_0002");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataRow newRow = newDt.NewRow();

                double k = 1;
                string caption = string.Empty;

                switch (i)
                {
                    case 0:
                        {
                            k = 100;
                            caption = "Исполнено";
                            break;
                        }
                    case 1:
                        {
                            k = 0.000001;
                            caption = "Факт, млн.руб.";
                            break;
                        }
                    case 2:
                        {
                            k = 0.000001;
                            caption = "План, млн.руб.";
                            break;
                        }
                    case 7:
                        {
                            caption = "Численность постоянного населения, тыс.чел.";
                            break;
                        }
                    case 8:
                        {
                            k = 0.001;
                            caption = "Среднедушевые доходы, тыс.руб./чел.";
                            break;
                        }
                    case 13:
                        {
                            k = 100;
                            caption = "&nbsp;&nbsp;&nbspфактический";
                            break;
                        }
                    case 14:
                        {
                            k = 100;
                            caption = "&nbsp;&nbsp;&nbsp;плановый";
                            break;
                        }
                }

                newRow[0] = caption;
                if (k == 1 && i != 7)
                {
                    continue;
                }
                if (dt.Rows[0][i] != DBNull.Value)
                {
                    newRow[1] = Convert.ToDouble(dt.Rows[0][i]) * k;
                }

                newDt.Rows.Add(newRow);
            }

            DataRow row = newDt.NewRow();
            row[0] = string.Empty;
            newDt.Rows.InsertAt(row, 1);

            row = newDt.NewRow();
            row[0] = string.Empty;
            newDt.Rows.InsertAt(row, 6);

            row = newDt.NewRow();
            row[0] = "Темп роста доходов к прошлому году";
            newDt.Rows.InsertAt(row, 7);
        }

        private void SetDataTable(DataTable dtGrid)
        {
            int fontSize = 9;//CRHelper.fontFK0002O;
            Color fontColor = CRHelper.fontLightColor;

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                TableRow row = new TableRow();

                string cellText = string.Empty;
                if (i == 1 || i == 6)
                {
                    int k = (i == 1) ? 3 : 9;

                    Label label1 = new Label();
                    if (dt.Rows[0][k] != DBNull.Value)
                    {
                        label1.Text = string.Format("{0}", Convert.ToInt32(dt.Rows[0][k]));
                    }
                    Label label2 = new Label();
                    if (dt.Rows[0][k + 2] != DBNull.Value)
                    {
                        label2.Text = string.Format("{0}", Convert.ToInt32(dt.Rows[0][k + 2]));
                    }

                    string img1 = string.Empty;
                    if (dt.Rows[0][k] != DBNull.Value && Convert.ToInt32(dt.Rows[0][k]) == 1)
                    {
                        img1 = string.Format("<img height=\"12\" width=\"12\" src=\"../../../images/starYellow.png\">");
                    }
                    else if (dt.Rows[0][k] != DBNull.Value && dt.Rows[0][k + 1] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0][k]) == Convert.ToInt32(dt.Rows[0][k + 1]))
                    {
                        img1 = string.Format("<img height=\"12\" width=\"12\" src=\"../../../images/starGray.png\">");
                    }

                    string img2 = string.Empty;
                    if (dt.Rows[0][k + 2] != DBNull.Value && Convert.ToInt32(dt.Rows[0][k + 2]) == 1)
                    {
                        img2 = string.Format("<img height=\"12\" width=\"12\" src=\"../../../images/starYellow.png\">");
                    }
                    else if (dt.Rows[0][k + 2] != DBNull.Value && dt.Rows[0][k + 3] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0][k + 2]) == Convert.ToInt32(dt.Rows[0][k + 3]))
                    {
                        img2 = string.Format("<img height=\"12\" width=\"12\" src=\"../../../images/starGray.png\">");
                    }

                    cellText = string.Format("ранг {4}&nbsp;<font color=\"white\">{0}</font>&nbsp;{1}&nbsp;&nbsp;ранг РФ&nbsp;<font color=\"white\">{2}</font>&nbsp;{3}", label1.Text, img1, label2.Text, img2, UserParams.ShortRegion.Value);

                    CRHelper.AddDataCellL(row, string.Format("{0} ", cellText), fontSize, fontColor);
                    row.Cells[0].ColumnSpan = 2;
                    CRHelper.SetCellHBorderNone(row, true); 
                }
                else
                {
                    CRHelper.AddDataCellL(row, string.Format("{0} ", dtGrid.Rows[i][0]), fontSize, fontColor);

                    if (i == 0 || i == 8 || i == 9)
                    {
                        cellText = string.Format("{0:P2}", Convert.ToDouble(dtGrid.Rows[i][1]) / 100);
                    }
                    else
                    {
                        cellText = string.Format("{0:N2}", dtGrid.Rows[i][1]);
                    }
                    CRHelper.AddDataCellR(row, cellText, fontSize, Color.White);
                    CRHelper.SetCellVBorderNone(row, 0, 1);
                    
                    CRHelper.SetCellVBorderNone(row, 0, 1);
                    if (i == 0 || i == 5 || i == 7) CRHelper.SetCellHBorderNone(row);
                    if (i == 8) CRHelper.SetCellHBorderNone(row, true);
                    if (i == 0 || i == 4 || i == 7) CRHelper.SetCellHBorder(row, true);
                }

                gridTable.Rows.Add(row);
            }
        }
    }
}
