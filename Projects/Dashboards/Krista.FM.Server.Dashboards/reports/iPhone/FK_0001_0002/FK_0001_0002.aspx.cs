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
    public partial class FK_0001_0002 : CustomReportPage
    {
        private DataTable dt;
        private DataTable newDt;
        private string populationDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "месяц";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "месяца";
                        break;
                    }
                default:
                    {
                        monthStr = "месяцев";
                        break;
                    }
            }

            Label1.Text = string.Format("Доходы {0} за {1} {2} {3}г.",
                            UserParams.ShortStateArea.Value,
                            monthNum,
                            monthStr,
                            yearNum);
            
            UltraWebGrid.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
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
                            caption = string.Format("Численность постоянного населения, тыс.чел. ({0})", populationDate);
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
                            caption = "фактический";
                            break;
                        }
                    case 14:
                        {
                            k = 100;
                            caption = "плановый";
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

            UltraWebGrid.DataSource = newDt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.ColHeadersVisibleDefault = ShowMarginInfo.No;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[0].Width = 208;
                e.Layout.Bands[0].Columns[1].Width = 100;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
                e.Row.Cells[i].Style.Padding.Bottom = 2;
                e.Row.Cells[i].Style.Padding.Top = 2;
                e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[1].Style.Padding.Right = 2;

                if (e.Row.Index != 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.Transparent;
                }
                else
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.FromArgb(50, 50, 50);
                }

                if (e.Row.Index == 1 || e.Row.Index == 6)
                {
                    int k = (e.Row.Index == 1) ? 3 : 9;

                    Label label1 = new Label();
                    if (dt.Rows[0][k] != DBNull.Value)
                    {
                        label1.Text = string.Format("<span style=\"color:white;\">{0}</span>", Convert.ToInt32(dt.Rows[0][k]));
                    }
                    Label label2 = new Label();
                    if (dt.Rows[0][k + 2] != DBNull.Value)
                    {
                        label2.Text = string.Format("<span style=\"color:white;\">{0}</span>", Convert.ToInt32(dt.Rows[0][k + 2]));
                    }

                    string img1 = string.Empty;
                    if (dt.Rows[0][k] != DBNull.Value && Convert.ToInt32(dt.Rows[0][k]) == 1)
                    {
                        img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
                    }
                    else if (dt.Rows[0][k] != DBNull.Value && dt.Rows[0][k + 1] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0][k]) == Convert.ToInt32(dt.Rows[0][k + 1]))
                    {
                        img1 = string.Format("<img src=\"../../../images/starGray.png\">");
                    }
                    
                    string img2 = string.Empty;
                    if (dt.Rows[0][k + 2] != DBNull.Value && Convert.ToInt32(dt.Rows[0][k + 2]) == 1)
                    {
                        img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
                    }
                    else if (dt.Rows[0][k + 2] != DBNull.Value && dt.Rows[0][k + 3] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0][k + 2]) == Convert.ToInt32(dt.Rows[0][k + 3]))
                    {
                        img2 = string.Format("<img src=\"../../../images/starGray.png\">");
                    }

                    e.Row.Cells[0].Value = string.Format("ранг {4}&nbsp;{0}&nbsp;{1}&nbsp;&nbsp;ранг РФ&nbsp;{2}&nbsp;{3}", label1.Text, img1, label2.Text, img2, UserParams.ShortRegion.Value);
                    e.Row.Cells[0].Style.Wrap = false;
                }
                else if (e.Row.Index != 7)
                {
                    e.Row.Cells[0].Style.BorderDetails.ColorRight = Color.Transparent;
                    e.Row.Cells[1].Style.BorderDetails.ColorLeft = Color.Transparent;
                }

                if (e.Row.Index == 1 || e.Row.Index == 6 || e.Row.Index == 7)
                {
                    e.Row.Cells[0].ColSpan = 2;
                }

                if (e.Row.Index == 8 || e.Row.Index == 9)
                {
                    e.Row.Cells[0].Style.Padding.Left = 50;
                }

                if (e.Row.Index == 4 || e.Row.Index == 7)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.FromArgb(50, 50, 50);
                }

                if (e.Row.Index == 0 || e.Row.Index == 5 || e.Row.Index == 7)
                {
                     e.Row.Cells[i].Style.BorderDetails.ColorBottom = Color.Transparent;
                }

                if (e.Row.Index == newDt.Rows.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }

                if ((e.Row.Index == 0 || e.Row.Index == 8 || e.Row.Index == 9) &&
                    i == 1 && e.Row.Cells[i].Value != null)
                {
                    e.Row.Cells[i].Value = string.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value) / 100);
                }
            }
        }
    }
}
