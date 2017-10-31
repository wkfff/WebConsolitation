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
    public partial class MFRF_0004_0003_H : CustomReportPage
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
            
            TextBox2.Text = string.Format("План = бюджетные ассигнования на {0} год", year);
            TextBox3.Text = string.Format("Факт = кассовое исполнение за {0} {1} {2} года", CRHelper.MonthNum(month), CRHelper.RusManyMonthGenitive(CRHelper.MonthNum(month)), year);
            TextBox4.Text = string.Format("Исп % = процент исполнения ассигнований за {0} {1} {2} года", CRHelper.MonthNum(month), CRHelper.RusManyMonthGenitive(CRHelper.MonthNum(month)), year);
            TextBox5.Text = string.Format("Темп роста,% = темп роста кассового исполнения к {0} году", year - 1);
            
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

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0004_0003_H");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = GetThinGRBSName(row[0].ToString());
                }

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && (i == 2 || i == 3))
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                    if (row[i] != DBNull.Value && (i == 4 || i == 5))
                    {
                        row[i] = Convert.ToDouble(row[i]) * 100;
                    }
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
            e.Layout.Bands[0].Columns[2].Header.Caption = "План";
            e.Layout.Bands[0].Columns[3].Header.Caption = "Факт";
            e.Layout.Bands[0].Columns[4].Header.Caption = "Исп %";
            e.Layout.Bands[0].Columns[5].Header.Caption = "Темп роста, %";

            e.Layout.Bands[0].Columns[0].Width = 163;
            e.Layout.Bands[0].Columns[1].Width = 40;
            e.Layout.Bands[0].Columns[2].Width = 85;
            e.Layout.Bands[0].Columns[3].Width = 67;
            e.Layout.Bands[0].Columns[4].Width = 39;
            e.Layout.Bands[0].Columns[5].Width = 80;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[5].Value != null && e.Row.Cells[5].Value.ToString() != string.Empty)
            {
                double value = Convert.ToDouble(e.Row.Cells[5].Value);
                e.Row.Cells[5].Style.BackgroundImage = (value > 100)
                    ? "../../../images/arrowUpGreen.png"
                    : "../../../images/arrowDownRed.png";
                e.Row.Cells[5].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
            }
        }
    }
}
