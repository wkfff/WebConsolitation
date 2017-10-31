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

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FZ_0083_0001 : CustomReportPage
    {
        private GridHeaderLayout headerLayout;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGridRF.Width = Unit.Empty;
            UltraWebGridRF.Height = Unit.Empty;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            //PageTitle.Text = "Сведения о сроках переходного периода по 83ФЗ";
            //PageSubTitle.Text = "Сроки окончания действия переходных положений по введению в действие Федерального закона от 8 мая 2010г. N 83-ФЗ «О внесении изменений в отдельные законодательные акты Российской Федерации в связи с совершенствованием правового положения государственных (муниципальных) учреждений»";

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear(); ;
            UltraWebGrid.DataBind();
        }
       
        private DataTable LoadIndicators()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/fz_0083_0001/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["table"];
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtGrid = LoadIndicators();

            BindRFData(dtGrid);

            dtGrid.Rows.RemoveAt(0);

            DataTable dtSorted = dtGrid.Clone();

            AddSortedRows(dtGrid, dtSorted, 0);
            AddSortedRows(dtGrid, dtSorted, 1);
            AddSortedRows(dtGrid, dtSorted, 3);
            AddSortedRows(dtGrid, dtSorted, 5);
            AddSortedRows(dtGrid, dtSorted, 7);
            AddSortedRows(dtGrid, dtSorted, 9);

            for (int i = dtSorted.Rows.Count - 1; i >= 0; i--)
            {
                dtSorted.Rows.InsertAt(dtSorted.NewRow(), i);
                dtSorted.Rows.InsertAt(dtSorted.NewRow(), i);
            }

            UltraWebGrid.DataSource = dtSorted;
        }

        private void AddSortedRows(DataTable dtGrid, DataTable dtSorted, int lastDateCellEthalonIndex)
        {
            for (int rowCount = 0; rowCount < dtGrid.Rows.Count; rowCount++)
            {

                int lastDateCellIndex = 0;
                for (int i = 1; i < dtGrid.Columns.Count - 2; i++)
                {
                    if (dtGrid.Rows[rowCount][i] != DBNull.Value
                        && dtGrid.Rows[rowCount][i].ToString() != String.Empty
                        && NeedCount(dtGrid.Rows[rowCount][i].ToString()))
                    {
                        lastDateCellIndex = i;
                    }
                }
                if (lastDateCellIndex == lastDateCellEthalonIndex)
                {
                    DataRow row = dtGrid.Rows[rowCount];
                    row["territory"] = row["npa"] != DBNull.Value && row["npa"].ToString() != String.Empty ?
                        String.Format("<a href='webcommand?showPopoverReport=fz_0083_0002_{0}&width=690&height=400&fitByHorizontal=true'>{1}</a>", 
                        CustomParams.GetSubjectIdByName(row["territory"].ToString()), row["territory"]) :
                        row["territory"];
                    dtSorted.ImportRow(dtGrid.Rows[rowCount]);
                }

            }
        }

        private void BindRFData(DataTable dtGrid)
        {
            DataTable dtRf = dtGrid.Clone();

            dtRf.Rows.Add(dtRf.NewRow());
            DataRow row = dtGrid.Rows[0];
            row["territory"] = row["npa"] != DBNull.Value && row["npa"].ToString() != String.Empty ?
                String.Format("<a href='{0}'>{1}</a>", row["npa"], row["territory"]) :
                row["territory"];
            dtRf.ImportRow(row);
            UltraWebGridRF.DataSource = dtRf;
            UltraWebGridRF.DataBind();
        }

        private bool NeedCount(string value)
        {
            return true;
        }

        private DataTable GetSecondGridDataSource(DataTable dtGrid)
        {
            DataTable dtGrid2 = new DataTable();

            DataColumn col = new DataColumn("Территория");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Финансирование бюджетных учреждений по бюджетной смете");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями доходов от сдачи в аренду имущества Казенные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями доходов от сдачи в аренду имущества Бюджетные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями средств от приносящей доход деятельности Казенные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями средств от приносящей доход деятельности Бюджетные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Основание");
            dtGrid2.Columns.Add(col);

            DataRow rfRow = dtGrid.Rows[0];
            DataRow newRow = dtGrid2.NewRow();
            newRow["Территория"] = rfRow["territory"];
            newRow["Основание"] = rfRow["hint"];

            for (int i = 1; i < dtGrid.Columns.Count - 1; i++)
            {
                if (rfRow[i] != DBNull.Value)
                {
                    string value = rfRow[i].ToString();
                    if (value.Contains("A"))
                    {
                        SetNewRowValue(newRow, i, 1);
                    }
                    if (value.Contains("B1"))
                    {
                        SetNewRowValue(newRow, i, 2);
                    }
                    if (value.Contains("B2"))
                    {
                        SetNewRowValue(newRow, i, 3);
                    }
                    if (value.Contains("C1"))
                    {
                        SetNewRowValue(newRow, i, 4);
                    }
                    if (value.Contains("C2"))
                    {
                        SetNewRowValue(newRow, i, 5);
                    }
                }

                dtGrid2.Rows.Add(newRow);
            }
            foreach (DataRow row in dtGrid.Rows)
            {

                newRow = dtGrid2.NewRow();
                newRow["Территория"] = row["territory"];
                newRow["Основание"] = row["npa"] != DBNull.Value && row["npa"].ToString() != String.Empty ?
                            String.Format("<a href='{0}'>{1}</a>", row["npa"], row["hint"]) :
                            row["hint"];

                for (int i = 1; i < dtGrid.Columns.Count - 1; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        string value = row[i].ToString();
                        if (value.Contains("A"))
                        {
                            SetNewRowValue(newRow, i, 1);
                        }
                        if (value.Contains("B1"))
                        {
                            SetNewRowValue(newRow, i, 2);
                        }
                        if (value.Contains("B2"))
                        {
                            SetNewRowValue(newRow, i, 3);
                        }
                        if (value.Contains("C1"))
                        {
                            SetNewRowValue(newRow, i, 4);
                        }
                        if (value.Contains("C2"))
                        {
                            SetNewRowValue(newRow, i, 5);
                        }
                    }


                    dtGrid2.Rows.Add(newRow);
                }
            }
            return dtGrid2;
        }

        private static void SetNewRowValue(DataRow newRow, int i, int cellIndex)
        {
            if (i == 1)
            {
                newRow[cellIndex] = "01.01.2011";
            }
            if (i == 3)
            {
                newRow[cellIndex] = "01.04.2011";
            }
            if (i == 5)
            {
                newRow[cellIndex] = "01.10.2011";
            }
            if (i == 7)
            {
                newRow[cellIndex] = "01.01.2012";
            }
            if (i == 9)
            {
                newRow[cellIndex] = "01.07.2012";
            }
        }

        protected void UltraWebGridRF_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(85);
            }

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(60);
            }

            UltraGridColumn col = e.Layout.Bands[0].Columns[1];

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(30);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[10].Width = CRHelper.GetColumnWidth(10);

            e.Layout.Bands[0].CellSpacing = 0;
            e.Layout.Bands[0].Grid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.None;
            e.Layout.Bands[0].Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.Bands[0].Grid.DisplayLayout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;

            e.Layout.Bands[0].HeaderStyle.BackColor = Color.White;

            e.Layout.Bands[0].Columns[11].Hidden = true;
            e.Layout.Bands[0].Columns[12].Hidden = true;

            e.Layout.Bands[0].Grid.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            e.Layout.Bands[0].Grid.DisplayLayout.CellClickActionDefault = CellClickAction.NotSet;

            int todayColumnIndex = GetTodayColumnIndex();

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            }

            e.Layout.Bands[0].HeaderLayout.Clear();

            e.Layout.Bands[0].Columns[todayColumnIndex].CellStyle.BorderDetails.StyleLeft = BorderStyle.None;
            e.Layout.Bands[0].Columns[todayColumnIndex].CellStyle.BorderDetails.WidthLeft = 1;
            e.Layout.Bands[0].Columns[todayColumnIndex].CellStyle.BorderDetails.ColorLeft = Color.FromArgb(0x7c7c7c);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(85);
            }

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(60);
            }

            UltraGridColumn col = e.Layout.Bands[0].Columns[1];

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(30);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[10].Width = CRHelper.GetColumnWidth(10);

            e.Layout.Bands[0].CellSpacing = 0;
            e.Layout.Bands[0].Grid.DisplayLayout.RowStyleDefault.BorderStyle = BorderStyle.None;
            e.Layout.Bands[0].Grid.DisplayLayout.RowSelectorsDefault = RowSelectors.No;
            e.Layout.Bands[0].Grid.DisplayLayout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;

            e.Layout.Bands[0].HeaderStyle.BackColor = Color.Black;
            e.Layout.Bands[0].Columns[0].Header.Style.BorderColor = Color.Transparent;
            e.Layout.Bands[0].Columns[0].Header.Style.BorderWidth = 0;

            e.Layout.Bands[0].Columns[11].Hidden = true;
            e.Layout.Bands[0].Columns[12].Hidden = true;

            e.Layout.Bands[0].Grid.DisplayLayout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            e.Layout.Bands[0].Grid.DisplayLayout.CellClickActionDefault = CellClickAction.NotSet;

            int todayColumnIndex = GetTodayColumnIndex();
            string todayText = String.Format("<div style='margin-left: 5px;margin-top: -20px;text-align:left'><span style='font-size: 12px; color: #C0C0C0;text-align:left'>&nbsp;</span></div>");

            GridHeaderCell parentCell = headerLayout.AddCell(" ");
            parentCell.AddCell(" ");

            parentCell.AddCell("<b>01.01.2011</b><br/><img style='margin-left: -80px; margin-top: -5px' src='../../../images/Date.png'/>");

            String cellText = todayColumnIndex == 2 ? todayText : " ";
            parentCell.AddCell(cellText); //2
            parentCell.AddCell("<b>01.04.2011</b><br/><img style='margin-left: -80px; margin-top: -5px' src='../../../images/Date.png'/>");
            cellText = todayColumnIndex == 4 ? todayText : " ";
            parentCell.AddCell(cellText); //4
            parentCell.AddCell("<b>01.10.2011</b><br/><img style='margin-left: -80px; margin-top: -5px' src='../../../images/Date.png'/>");
            cellText = todayColumnIndex == 6 ? todayText : " ";
            parentCell.AddCell(cellText); //6

            parentCell.AddCell("<b>01.01.2012</b><br/><img style='margin-left: -80px; margin-top: -5px' src='../../../images/Date.png'/>");
            cellText = todayColumnIndex == 8 ? todayText : " ";
            parentCell.AddCell(cellText); //8
            parentCell.AddCell("<b>01.07.2012</b><br/><img style='margin-left: -80px; margin-top: -5px' src='../../../images/Date.png'/>");
            cellText = todayColumnIndex == 10 ? todayText : " ";
            parentCell.AddCell(cellText); //10

            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].Header.Style.BackgroundImage = "~/images/timelineIphone.png";
            e.Layout.Bands[0].Columns[0].Header.Style.CustomRules = "background-repeat: no-repeat; background-position: right center; padding-right: 0px";
            e.Layout.Bands[0].Columns[0].Header.Style.Height = 50;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.BorderColor = Color.Transparent;
                e.Layout.Bands[0].Columns[i].Header.Style.BorderWidth = 0;

                e.Layout.Bands[0].Columns[i].Header.Style.BackgroundImage = "~/images/timelineIphone.png";
                e.Layout.Bands[0].Columns[i].Header.Style.CustomRules = "background-repeat: repeat-x; background-position: right center; padding-right: 0px";

                e.Layout.Bands[0].Columns[i].Header.Style.BorderStyle = BorderStyle.None;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            }

            e.Layout.Bands[0].Columns[todayColumnIndex].Header.Style.BorderDetails.StyleLeft = BorderStyle.None;
            e.Layout.Bands[0].Columns[todayColumnIndex].Header.Style.BorderDetails.WidthLeft = 1;
            e.Layout.Bands[0].Columns[todayColumnIndex].Header.Style.BorderDetails.ColorLeft = Color.FromArgb(0x7c7c7c);
            e.Layout.Bands[0].Columns[todayColumnIndex].CellStyle.BorderDetails.StyleLeft = BorderStyle.None;
            e.Layout.Bands[0].Columns[todayColumnIndex].CellStyle.BorderDetails.WidthLeft = 1;
            e.Layout.Bands[0].Columns[todayColumnIndex].CellStyle.BorderDetails.ColorLeft = Color.FromArgb(0x7c7c7c);
        }

        private int GetTodayColumnIndex()
        {
            if (new DateTime(2012, 1, 1) < DateTime.Now)
            {
                return 8;
            }
            if (new DateTime(2011, 10, 1) < DateTime.Now)
            {
                return 6;
            }
            if (new DateTime(2011, 4, 1) < DateTime.Now)
            {
                return 4;
            }
            if (new DateTime(2011, 1, 1) < DateTime.Now)
            {
                return 2;
            }
            return 9;
        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int lastDateCellIndex = 1;

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

                        if (value.Contains("A") && NeedCount("A"))
                        {
                            e.Row.Cells[i].Value += String.Format("<img style='margin-left: -10px' src='../../../images/A.png' title='{0}: Финансирование бюджетных учреждений по бюджетной смете'/>", period);
                            lastDateCellIndex = i;
                        }
                        if (value.Contains("C1") && NeedCount("C1"))
                        {
                            e.Row.Cells[i].Value += String.Format("<img style='margin-left: -10px' src='../../../images/B1.png' title='{0}: Использование учреждениями доходов от сдачи в аренду имущества'/>", period);
                            lastDateCellIndex = i;
                        }
                        if (value.Contains("C2") && NeedCount("C2"))
                        {
                            e.Row.Cells[i].Value += String.Format("<img style='margin-left: -10px' src='../../../images/B2.png' title='{0}: Использование учреждениями доходов от сдачи в аренду имущества'/>", period);
                            lastDateCellIndex = i;
                        }
                        if (value.Contains("B1") && NeedCount("B1"))
                        {
                            e.Row.Cells[i].Value += String.Format("<img style='margin-left: -10px' src='../../../images/C1.png' title='{0}: Использование учреждениями средств от приносящей доход деятельности'/>", period);
                            lastDateCellIndex = i;
                        }
                        if (value.Contains("B2") && NeedCount("B2"))
                        {
                            e.Row.Cells[i].Value += String.Format("<img style='margin-left: -10px' src='../../../images/C2.png' title='{0}: Использование учреждениями средств от приносящей доход деятельности'/>", period);
                            lastDateCellIndex = i;
                        }
                    }
                }
                for (int i = 1; i < lastDateCellIndex; i++)
                {
                    e.Row.Cells[i].Style.BackgroundImage = "~/images/progressbarCenter.png";
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: repeat-x; background-position: 0px center; padding-left: 0px; padding-right: 0px";
                }
                e.Row.Cells[lastDateCellIndex].Style.BackgroundImage = "~/images/progressbarRight.png";
                e.Row.Cells[lastDateCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-left: 0px";

                e.Row.Cells[0].Style.BackgroundImage = "~/images/progressbarLeft.png";
                e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: right center; padding-right: 0px";

                e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[0].Value = e.Row.Cells[0].Value;
                e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[0].ColSpan = 5;      
                e.Row.Cells[0].Value = String.Empty;
            }
            else
            {
                e.Row.Height = 0;
                e.Row.Style.Padding.Bottom = 0;
                e.Row.Style.Padding.Top = 0;          
                e.Row.Cells[0].Style.VerticalAlign = VerticalAlign.Bottom;
            }
        }       
    }
}