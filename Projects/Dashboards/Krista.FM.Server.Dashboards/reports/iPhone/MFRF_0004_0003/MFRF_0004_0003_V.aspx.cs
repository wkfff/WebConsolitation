using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0004_0003_V : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0004_0003_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)));

            Label3.Text = string.Format("Исполнение за {0} {1} {2} года", CRHelper.MonthNum(month), CRHelper.RusManyMonthGenitive(CRHelper.MonthNum(month)), year);
            Label4.Text = string.Format("Исп % = процент исполнения ассигнований за&nbsp;{0}&nbsp;{1}&nbsp;{2}&nbsp;года", CRHelper.MonthNum(month), CRHelper.RusManyMonthGenitive(CRHelper.MonthNum(month)), year);

            if (monthNum == 12)
            {
                monthNum = 1;
                year++;
            }
            else
            {
                monthNum++;
            }
            Label1.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), year);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            UltraWebGrid.DataBind();
        }

        private string GetThinGRBSName(string longGRBS)
        {
            switch (longGRBS)
            {
                case "Минздравсоцразвития России":
                    {
                        return "Минздравсоцразви- тия России";
                    }
                case "Росинформтехнологии":
                    {
                        return "Росинформтехноло- гии";
                    }
                case "Росалкогольрегулирование":
                    {
                        return "Росалкогольрегули- рование";
                    }
                case "Ростехрегулирование":
                    {
                        return "Ростехрегулирова- ние";
                    }
                default:
                    {
                        return longGRBS;
                    }
            }
        }

        private int minIndex = 0;
        private int maxIndex = 0;

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0004_0003_V");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row[0] != DBNull.Value)
                {
                    row[0] = GetThinGRBSName(row[0].ToString());
                }
                if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(row[2]);
                    value = value * 100;

                    if (dt.Rows[minIndex][2] != null && dt.Rows[minIndex][2].ToString() != string.Empty &&
                         value < Convert.ToDouble(dt.Rows[minIndex][2]))
                    {
                        minIndex = i;
                    }

                    if (dt.Rows[maxIndex][2] != null && dt.Rows[maxIndex][2].ToString() != string.Empty &&
                         value > Convert.ToDouble(dt.Rows[maxIndex][2]))
                    {
                        maxIndex = i;
                    }
                    row[2] = value;
                }
            }

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            
            e.Layout.Bands[0].Columns[0].Header.Caption = "ГРБС";
            e.Layout.Bands[0].Columns[1].Header.Caption = "Код";
            e.Layout.Bands[0].Columns[2].Header.Caption = "Исп %";

            e.Layout.Bands[0].Columns[0].Width = 163;
            e.Layout.Bands[0].Columns[1].Width = 48;
            e.Layout.Bands[0].Columns[2].Width = 100;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == minIndex)
            {
                e.Row.Cells[2].Style.BackgroundImage = "~/images/starGray.png";
                e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
            }

            if (e.Row.Index == maxIndex)
            {
                e.Row.Cells[2].Style.BackgroundImage = "~/images/starYellow.png";
                e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
            }
        }
    }
}
