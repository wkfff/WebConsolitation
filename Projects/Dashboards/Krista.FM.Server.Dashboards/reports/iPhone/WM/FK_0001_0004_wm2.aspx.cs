using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0004_wm2 : CustomReportPage
    {
        DataTable dt = new DataTable();
        int yearNum;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable(); 
            string query = DataProvider.GetQueryText("FK_0001_0004_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            
            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr = CRHelper.RusManyMonthGenitive(monthNum);

            LabelTitle.Font.Size = 9;

            LabelTitle.Text = string.Format("Темп роста фактических доходов за {0}&nbsp;{3}&nbsp;{1}&nbsp;года по {2}",
                                        monthNum,
                                        yearNum,
                                        UserParams.ShortStateArea.Value,
                                        monthStr);
            FillData();
            SetDataTable(dt);
        }

        protected void FillData()
        {
            string query = DataProvider.GetQueryText("FK_0001_0004");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[0].ToString().TrimEnd('_');
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i])*100;
                    }
                }
            }
        }

        private double GetCellValue(double value)
        {
            return Math.Round(value, 1);            
        }

        private void SetDataTable(DataTable dtGrid)
        {
            TableRow rowHeader = new TableRow();

            int fontSize = 9; // CRHelper.fontFK0004O; 
            Color fontColor = CRHelper.fontLightColor;
            Color captionColor = CRHelper.fontTableCaptionColor;
            Color mainColumnColor = CRHelper.fontTableDataColor;

            CRHelper.AddCaptionCell(rowHeader, "Доходы", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Доля в суб", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, string.Format("Темп роста к {0} году %", yearNum - 1), fontSize, captionColor);

            rowHeader.Cells[0].RowSpan = 2; 
            rowHeader.Cells[2].ColumnSpan = 3;
            gridTable.Rows.Add(rowHeader);

            rowHeader = new TableRow();
            CRHelper.AddCaptionCell(rowHeader, "%", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "суб", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "РФ", fontSize, captionColor);

            rowHeader.Cells[1].ColumnSpan = 2;

            gridTable.Rows.Add(rowHeader);

            string imagePath = string.Empty;
            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                double valueSub = 0;
                double valueFO = 0;
                double valueRF = 0;
                if (dtGrid.Rows[i][2] != DBNull.Value) valueSub = Convert.ToDouble(dtGrid.Rows[i][2]);
                if (dtGrid.Rows[i][3] != DBNull.Value) valueFO = Convert.ToDouble(dtGrid.Rows[i][3]);
                if (dtGrid.Rows[i][4] != DBNull.Value) valueRF = Convert.ToDouble(dtGrid.Rows[i][4]);
                 
                if (valueSub < 100 || valueSub < valueFO || valueSub < valueRF)
                { 
                    imagePath = "~/images/CornerRed.gif";
                }
                else
                {
                    imagePath = "~/images/CornerGreen.gif";
                }

                TableRow row = new TableRow();
                string value = dtGrid.Rows[i][0].ToString();
                if (i == 1 || i == 3 || i == 4) value = CRHelper.ToLowerFirstSymbol(value);
                value = value.Replace(" доходы", string.Empty);
                CRHelper.AddDataCellL(row, value, fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N1}", dtGrid.Rows[i][1]), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0}", GetCellValue(valueSub)), fontSize, mainColumnColor);

                TableCell cell = new TableCell();
                System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                image.ImageUrl = imagePath;
                image.ImageAlign = ImageAlign.Top;
                image.Width = 5;
                image.Height = 5;
                cell.Controls.Add(image);
                cell.VerticalAlign = VerticalAlign.Top;
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.BorderStyle = BorderStyle.None;
                row.Cells.Add(cell);

                CRHelper.SetCellVBorderNone(row, 2, 3);

                //CRHelper.AddDataCellR(row, string.Format("{0:N1}", GetCellValue(valueFO)), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0}", GetCellValue(valueRF)), fontSize, mainColumnColor);

                if (i > 0 && i < 5)
                {
                    row.Cells[0].Style["padding-left"] = "10px";
                    row.Cells[0].Style["padding-right"] = "0px";
                }

                gridTable.Rows.Add(row);
            }
        }
    }
}
