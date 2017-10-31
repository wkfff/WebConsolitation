using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class STAT_0001_0002_h : CustomReportPage
    {
        private DataTable dt;

        // Дата
        private CustomParam periodDay;
        private CustomParam periodLastDay;

        private DateTime date;
        private DateTime lastDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (UserParams.Region.Value != "Уральский федеральный округ")
            {
                Label17.Text =
                    String.Format(
                        "По выбранному субъекту Российской Федерации <b>&nbsp;{0}&nbsp;</b> отсутствуют сведения по показателям мониторинга ситуации на рынке труда.<br/>На данный момент информация присутствует только по субъектам Уральского федерального округа.",
                        UserParams.StateArea.Value);
                tableMain.Visible = false;
                return;
            }
            Label17.Visible = false;

            form1.Style["margin"] = "0px";
            form1.Style["padding"] = "0px";
            form1.Style.Remove("width");
            form1.Style.Remove("height");

            periodDay = UserParams.CustomParam("period_day");
            periodLastDay = UserParams.CustomParam("period_last_day");

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            Label1.Text =
                string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4],
                              CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())),
                              dtDate.Rows[0][0]);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][5].ToString(), 3);
            lastDate = date.AddDays(-7);

            TextBox1.Text = string.Format("На {0:dd.MM.yyyy} по {1}", date, RegionsNamingHelper.ShortName(UserParams.StateArea.Value));

            periodDay.Value = dtDate.Rows[0][5].ToString();
            periodLastDay.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDate, 5);

            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBind();

            query = DataProvider.GetQueryText("STAT_0001_0002_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            if (dtDebtsDate.Rows.Count > 1)
            {
                if (dtDebtsDate.Rows[0][1] != DBNull.Value && dtDebtsDate.Rows[0][1].ToString() != string.Empty)
                {
                    periodLastDay.Value = dtDebtsDate.Rows[0][1].ToString();
                    lastDate = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != string.Empty)
                {
                    periodDay.Value = dtDebtsDate.Rows[1][1].ToString();
                    date = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
            }
                
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid1.DataBind();
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0002_debts_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((dt.Rows[i][0].ToString().Contains("Уровень регистрируемой безработицы") ||
                             dt.Rows[i][0].ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию")) && dt.Rows[i][0].ToString().Contains("Прирост"))
                {
                    dt.Rows[i][0] = "ранг по УрФО";
                }
                else
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Split(';')[0];
                }
            }
            ((UltraWebGrid)sender).DataSource = dt;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0002_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((dt.Rows[i][0].ToString().Contains("Уровень регистрируемой безработицы") ||
                             dt.Rows[i][0].ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию")) && dt.Rows[i][0].ToString().Contains("Прирост"))
                {
                    dt.Rows[i][0] = "ранг по УрФО";
                    dt.Rows[i][3] = DBNull.Value;
                    dt.Rows[i][4] = DBNull.Value;
                }
                else
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Split(';')[0];
                }
            }

            UltraWebGrid.DataSource = dt;
        }


        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].SelectTypeCell = SelectType.None;
            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[3].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;

            if (e.Layout.Bands.Count == 0)
                return;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, RegionsNamingHelper.ShortRegionsNames[e.Layout.Bands[0].Columns[1].Header.Caption.Split(';')[0]], 1, 0, 2, 1);
            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, e.Layout.Bands[0].Columns[3].Header.Caption.Split(';')[0], 3, 0, 2, 1);

            e.Layout.Bands[0].Columns[1].Header.Caption = lastDate.ToString("dd.MM");
            e.Layout.Bands[0].Columns[2].Header.Caption = date.ToString("dd.MM");
            e.Layout.Bands[0].Columns[3].Header.Caption = lastDate.ToString("dd.MM");
            e.Layout.Bands[0].Columns[4].Header.Caption = date.ToString("dd.MM");

            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.Bands[0].Columns[0].Width = 180;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = 72;
                //CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                //e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("12px");
                e.Layout.Bands[0].Columns[i].CellStyle.VerticalAlign = VerticalAlign.Top;
            }
        }

        private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Index % 3 == 0 && e.Row.Cells[i].Value != null)
                {
                    if (e.Row.Cells[0].Value.ToString() == "Уровень регистрируемой безработицы")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}%", e.Row.Cells[i].Value);
                    }
                    else if (e.Row.Cells[0].Value.ToString() == "Число зарегистрированных безработных в расчёте на 1 вакансию")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N2}", e.Row.Cells[i].Value);
                    }
                    else if (e.Row.Cells[0].Value.ToString() == "Сумма задолженности по выплате заработной платы (млн.руб.)")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}", e.Row.Cells[i].Value);
                    }
                    else
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N0}", e.Row.Cells[i].Value);
                    }
                    e.Row.Cells[i].Style.Padding.Top = 10;
                    e.Row.Cells[i].Style.Padding.Bottom = 15;
                    if (e.Row.Cells[0].Value.ToString().Contains("аналогичный период прошлого года"))
                    {
                        e.Row.Cells[0].Style.Font.Italic = true;
                    }
                }

                if (e.Row.Index % 3 == 1 && e.Row.Cells[i].Value != null)
                {
                    //    e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerGreen.gif";
                    e.Row.Style.BorderDetails.WidthBottom = 0;
                    double value;
                    if (Double.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.Padding.Top = 10;
                        e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.Padding.Bottom = 15;

                        if (value == 1)
                        {
                            e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerRed.gif";
                            e.Row.Cells[i].Value = String.Format("+{0:P0}", value);
                        }
                        else if (value == -1)
                        {
                            e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerGreen.gif";
                            e.Row.Cells[i].Value = String.Format("{0:P0}", value);
                        }
                        else if (value > 0)
                        {
                            e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerRed.gif";
                            e.Row.Cells[i].Value = String.Format("+{0:P2}", value);
                        }
                        else
                        {
                            e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerGreen.gif";
                            e.Row.Cells[i].Value = String.Format("{0:P2}", value);
                        }
                        
                    }
                    e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top;";
                }

                if (e.Row.Index % 3 == 2 && e.Row.Cells[i].Value != null)
                {
                    if (e.Row.Cells[0].Value.ToString() == "Сумма задолженности по выплате заработной платы (млн.руб.)")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}", e.Row.Cells[i].Value);
                    }
                    else
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N0}", e.Row.Cells[i].Value);
                    }
                }

                e.Row.Cells[i].Style.Padding.Right = 2;

                if (e.Row.Cells[0].Value.ToString() == "ранг по УрФО")
                {
                    e.Row.Band.Grid.Rows[e.Row.Index - 2].Cells[0].Style.BorderDetails.WidthBottom = 0;
                    e.Row.Style.BorderDetails.WidthTop = 0;
                    e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;

                    if (e.Row.Cells[i].ToString() == "6")
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "../../../images/StarYellow.png";
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left top;";
                    }
                    else if (e.Row.Cells[i].ToString() == "1")
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "../../../images/StarGray.png";
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left top;";
                    }
                }
                else
                {
                    e.Row.Cells[0].Style.BorderDetails.WidthBottom = 1;
                }
            }

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 2;
            }

            e.Row.Style.BorderDetails.WidthBottom = 0;
            e.Row.Style.BorderDetails.WidthTop = 0;

            if ((e.Row.Index + 1) % 3 == 0)
            {
                e.Row.Style.BorderDetails.WidthBottom = 1;
                e.Row.Style.BorderDetails.WidthTop = 0;
            }
        }
    }
}
