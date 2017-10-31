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

namespace Krista.FM.Server.Dashboards.reports.FZ_0083_0001
{
    public partial class Default : CustomReportPage
    {
        private GridHeaderLayout headerLayout;
        private GridHeaderLayout headerLayout1;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            Page.ClientTarget = "uplevel";

            base.Page_PreLoad(sender, e);
            
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 200);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                kindsCombo.Width = 720;
                kindsCombo.MultiSelect = false;
                kindsCombo.ParentSelect = true;
                kindsCombo.FillDictionaryValues(GetKindsDictionary());
                kindsCombo.Title = "Показатели переходных положений";
                kindsCombo.SetСheckedState("Все", true);

                foCombo.Title = "Федеральный округ";
                foCombo.Width = 390;
                foCombo.MultiSelect = false;
                foCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
            }

            PageTitle.Text = "Сведения о сроках переходного периода по 83ФЗ";
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = "Сроки окончания действия переходных положений по введению в действие Федерального закона от 8 мая 2010г. N 83-ФЗ «О внесении изменений в отдельные законодательные акты Российской Федерации в связи с совершенствованием правового положения государственных (муниципальных) учреждений»";


            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);

            UltraWebGrid1.Bands.Clear();
            DataTable dtGrid2 = GetSecondGridDataSource(LoadIndicators());

            UltraWebGrid1.DataSource = dtGrid2;
            UltraWebGrid1.DataBind();

            lbGrid1Caption.Text = "Сроки окончания действия переходных положений по 83ФЗ";
        }

           


        #region Обработчики грида

        private DataTable LoadIndicators()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/fz_0083_0001/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["table"];
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {           
           
        }

        private void AddSortedRows(DataTable dtGrid, DataTable dtSorted, int lastDateCellEthalonIndex)
        {
            for (int rowCount = 0; rowCount < dtGrid.Rows.Count; rowCount++)
            {
                if (foCombo.SelectedIndex == 0 || RegionsNamingHelper.GetFoBySubject(Regex.Replace(dtGrid.Rows[rowCount][0].ToString(), "<[\\s\\S]*?>", String.Empty)) == foCombo.SelectedValue)
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
                            String.Format("<a href='{0}'>{1}</a>", row["npa"], row["territory"]) :
                            row["territory"];
                        dtSorted.ImportRow(dtGrid.Rows[rowCount]);
                    }
                }
            }
        }

        private bool NeedCount(string value)
        {
            if (kindsCombo.SelectedIndex == 0)
            {
                if (RadioButtonList1.SelectedIndex == 0)
                {
                    return true;
                }
                if ((RadioButtonList1.SelectedIndex == 1 &&
                    (value.Contains("A") || value.Contains("1"))))
                {
                    return true;
                }
                if ((RadioButtonList1.SelectedIndex == 2 &&
                    (value.Contains("A") || value.Contains("2"))))
                {
                    return true;
                }
            }
            if (kindsCombo.SelectedIndex == 1 && value.Contains("A"))
            {
                return true;
            }
            if (kindsCombo.SelectedIndex == 3)
            {
                if (RadioButtonList1.SelectedIndex == 0 && value.Contains("B"))
                {
                    return true;
                }
                if ((RadioButtonList1.SelectedIndex == 1 && value.Contains("B1")))
                {
                    return true;
                }
                if ((RadioButtonList1.SelectedIndex == 2 && value.Contains("B2")))
                {
                    return true;
                }
            }
            if (kindsCombo.SelectedIndex == 2)
            {
                if (RadioButtonList1.SelectedIndex == 0 && value.Contains("C"))
                {
                    return true;
                }
                if ((RadioButtonList1.SelectedIndex == 1 && value.Contains("C1")))
                {
                    return true;
                }
                if ((RadioButtonList1.SelectedIndex == 2 && value.Contains("C2")))
                {
                    return true;
                }
            }
            return false;
        }

        private DataTable GetSecondGridDataSource(DataTable dtGrid)
        {
            DataTable dtGrid2 = new DataTable();

            DataColumn col = new DataColumn("Территория");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Финансирование бюджетных учреждений по бюджетной смете");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями средств от приносящей доход деятельности Казенные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями средств от приносящей доход деятельности Бюджетные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями доходов от сдачи в аренду имущества Казенные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями доходов от сдачи в аренду имущества Бюджетные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Основание");
            dtGrid2.Columns.Add(col);

            if (foCombo.SelectedIndex != 0)
            {
                DataRow rfRow = dtGrid.Rows[0];
                DataRow newRow = dtGrid2.NewRow();
                newRow["Территория"] = rfRow["territory"];
                newRow["Основание"] = rfRow["npa"] != DBNull.Value && rfRow["npa"].ToString() != String.Empty ?
                                String.Format("<a href='{0}'>{1}</a>", rfRow["npa"], rfRow["hint"]) :
                                rfRow["hint"];

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
                }

                dtGrid2.Rows.Add(newRow);
            }
            foreach (DataRow row in dtGrid.Rows)
            {
                if (foCombo.SelectedIndex == 0 ||
                    RegionsNamingHelper.GetFoBySubject(row[0].ToString()) == foCombo.SelectedValue)
                {
                    DataRow newRow = dtGrid2.NewRow();
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

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            UltraWebGrid1.DisplayLayout.StationaryMargins = Infragistics.WebUI.UltraWebGrid.StationaryMargins.Header;

            e.Layout.UseFixedHeaders = true;
            e.Layout.Bands[0].Columns[0].Header.Fixed = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(450);
            e.Layout.Bands[0].Columns[6].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[6].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout1.AddCell("Территория");

            GridHeaderCell cell = headerLayout1.AddCell("Срок окончания действия переходных положений");
            cell.AddCell("Перевод всех бюджетных учреждений на субсидии");
            GridHeaderCell childCell = cell.AddCell("Окончание действия временного порядка использования средств от приносящей доход деятельности");
            childCell.AddCell("Казенными учреждениями");
            childCell.AddCell("Бюджетными учреждениями");
            childCell = cell.AddCell("Окончание действия временного порядка использования доходов от аренды");
            childCell.AddCell("Казенными учреждениями");
            childCell.AddCell("Бюджетными учреждениями");
            headerLayout1.AddCell("Основание");

            headerLayout1.ApplyHeaderInfo();
        }

        private Dictionary<string, int> GetKindsDictionary()
        {
            Dictionary<string, int> kinds = new Dictionary<string, int>();
            kinds.Add("Все", 0);
            kinds.Add("Перевод всех бюджетных учреждений на субсидии", 0);
            kinds.Add("Окончание действия временного порядка использования доходов от аренды", 0);    
            kinds.Add("Окончание действия временного порядка использования средств от приносящей доход деятельности", 0);                  
            return kinds;
        }

        #endregion
    }
}