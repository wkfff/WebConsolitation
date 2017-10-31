using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.HotReports
{
    public partial class FK_0001_0004_hot : GadgetControlBase, IHotReport
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int year = 2009;
        private int monthNum;
        private string middle = "В среднем по РФ";

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(350);

            if (Request.Params["embedded"] != null && !String.IsNullOrEmpty(UserParams.Region.Value))
            {
                UserParams.Filter.Value = String.Format("[Территории].[Сопоставимый].CurrentMember.Parent is [Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}]", UserParams.Region.Value);
                middle = String.Format("В среднем по {0}", UserParams.ShortRegion.Value);
            }
            else
            {
                UserParams.Filter.Value = "[Территории].[Сопоставимый].CurrentMember.Parent.Parent is [Территории].[Сопоставимый].[Все территории].[Российская  Федерация]";
            }

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0004_date", Server.MapPath("~/Reports/HotReports/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodMonth.Value = dtDate.Rows[0][4].ToString();
            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());
            UltraWebGrid1.DataBind();
            Label1.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);
            HyperLink1.Text = "Подробнее падение (рост) доходов по субъектам";
            HyperLink1.NavigateUrl = "~/reports/FK_0001_0004/DefaultCompare.aspx";
            HyperLink1.Attributes.Add("target", "_top");
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/3);
            UltraWebGrid1.Width = this.Width;
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_compare_Grid", Server.MapPath("~/Reports/HotReports/"));
            DataTable dtSource = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtSource);
            
            double average = CalculateAverage(dtSource, 1);
            int medianIndex = MedianIndex(dtSource.Rows.Count);
            dtGrid = dtSource.Clone();
            dtGrid.TableName = "dtGrid";
            DataRow[] rows = dtSource.Select("", "Налоговые и неналоговые доходы ; Темп роста ASC");
            for (int i = 0; i < rows.Length; i++)
            {
                dtGrid.ImportRow(rows[i]);
                if (i == medianIndex)
                {
                    DataRow row = dtGrid.NewRow();
                    row[0] = "Медиана";
                    row[1] = MedianValue(rows, 1);
                    dtGrid.Rows.Add(row);
                }
                
                double value;
                Double.TryParse(rows[i][1].ToString(), out value);
                if (value <= average)
                {
                    Double.TryParse(rows[i + 1][1].ToString(), out value);
                    if (value > average)
                    {
                        DataRow row = dtGrid.NewRow();

                        row[0] = middle;
                        row[1] = average;
                        dtGrid.Rows.Add(row);
                    }
                }
            }
            if (Request.Params["saveXml"] != null && Request.Params["saveXml"] == "y")
            {
                MemoryStream ms = new MemoryStream();
                dtGrid.WriteXml(ms);
                ms.Flush();
                StreamReader writer = new StreamReader(ms);
                ms.Position = 0;
                Response.Write(writer.ReadToEnd());
                writer.Close();
                Response.End();
            }
            UltraWebGrid1.DataSource = dtGrid;
        }

        private double CalculateAverage(DataTable dtSource, int columnIndex)
        {
            double average = 0;
            for (int i = 0; i < dtSource.Rows.Count; i++ )
            {
                double value;
                Double.TryParse(dtSource.Rows[i][columnIndex].ToString(), out value);
                average += value;
            }
            average /= dtSource.Rows.Count;
            return average;
        }
        
        private bool Even(int input)
        {
            if (input % 2 == 0)
            {
                return true;
            }
            return false;
        }
        
        private int MedianIndex(int length)
        {
            if (Even(length))
            {
                return (length > 0 ? length / 2 - 1 : 0);
            }
            else
            {
                return (length + 1) / 2 - 1;
            }
        }

        private double MedianValue(DataRow[] rows, int columnIndex)
        {
            if (Even(rows.Length))
            {
                double value1;
                double value2;
                Double.TryParse(
                        rows[MedianIndex(rows.Length)][columnIndex].ToString(), 
                        out value1);
                Double.TryParse(
                        rows[MedianIndex(rows.Length)+ 1][columnIndex].ToString(), 
                        out value2);
                return (value1 + value2) / 2;
            }
            else
            {
                double value;
                Double.TryParse(
                        rows[MedianIndex(rows.Length)][columnIndex].ToString(),
                        out value);
                return value;
            }
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");

            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;

            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginY = 1;
            e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginY = 1;

            e.Layout.Bands[0].Columns[0].Width = 150;
            e.Layout.Bands[0].Columns[1].Width = 80;
            e.Layout.Bands[0].Columns[2].Width = 80;

            ColumnHeader ch = new ColumnHeader(true);
            string[] captions = e.Layout.Bands[0].Columns[1].Header.Caption.Split(';');
            ch.Caption = captions[0];

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Доля в бюджете", "Доля доходов в общей сумме доходов");

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            e.Layout.Bands[0].Columns[3].Hidden = true;
            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            
            if (e.Row.Cells[1].Value != null && e.Row.Cells[1].Value.ToString() != string.Empty &&
                e.Row.Cells[3].Value != null && e.Row.Cells[3].Value.ToString() != string.Empty&&
                e.Row.Cells[4].Value != null && e.Row.Cells[4].Value.ToString() != string.Empty)
            {
                string executed = String.Format("Исполнено за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);
                string executedLast = String.Format("Исполнено за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year - 1);
                if (100 * Convert.ToDouble(e.Row.Cells[1].Value) < 100)
                {
                    e.Row.Cells[1].Style.CssClass = "ArrowDownRed";
                    
                    e.Row.Cells[1].Title = String.Format(
                            "Падение к прошлому году \n{0}: {1:N2} млн.руб.\n{2}: {3:N2} млн.руб.",
                             executed, e.Row.Cells[3].Value, executedLast, e.Row.Cells[4].Value);
                }
                else if (100 * Convert.ToDouble(e.Row.Cells[1].Value) > 100)
                {
                    e.Row.Cells[1].Style.CssClass = "ArrowUpGreen";
                    e.Row.Cells[1].Title = String.Format(
                            "Рост к прошлому году \n{0}: {1:N2} млн.руб.\n{2}: {3:N2} млн.руб.",
                            executed, e.Row.Cells[3].Value, executedLast, e.Row.Cells[4].Value);
                }
            }

            if (e.Row.Cells[2].Value != null && e.Row.Cells[2].Value.ToString() != string.Empty &&
                e.Row.Cells[5].Value != null && e.Row.Cells[5].Value.ToString() != string.Empty)
            {
                if (Convert.ToDouble(e.Row.Cells[2].Value) < Convert.ToDouble(e.Row.Cells[5].Value))
                {
                    e.Row.Cells[2].Style.CssClass = "ArrowDownRed";
                    e.Row.Cells[2].Title = String.Format("Доля упала с прошлого года (было {0:P2})", e.Row.Cells[5].Value);
                }
                else if (Convert.ToDouble(e.Row.Cells[2].Value) > Convert.ToDouble(e.Row.Cells[5].Value))
                {
                    e.Row.Cells[2].Style.CssClass = "ArrowUpGreen";
                    e.Row.Cells[2].Title = String.Format("Доля выросла с прошлого года (было {0:P2})", e.Row.Cells[5].Value);
                }
            }

            if (e.Row.Cells[0].Value != null &&
                (e.Row.Cells[0].Value.ToString() == "Медиана" || 
                e.Row.Cells[0].Value.ToString() == middle))
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        public int Width
        {
            get
            {
                return 360;
            }
        }

        public int Height
        {
            get
            {
                return 340;
            }
        }
    }
}