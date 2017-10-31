using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Shared.Styles;
using System.Net;
using System.IO;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using Graphics = System.Drawing.Graphics;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using System.Drawing.Imaging;
using Microsoft.VisualBasic;
using Infragistics.WebUI.UltraWebNavigator;
using System.Globalization;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraChart.Core.Layers;
namespace Krista.FM.Server.Dashboards.reports.HMAO_ARC._0007
{
    public partial class _default : CustomReportPage
    {
        DataTable BaseTable;
        DataTable MapTable;    
        private string style = "";
        GridHeaderLayout GHL;
        CustomParam SelectYear { get { return (UserParams.CustomParam("Year")); } }
        CustomParam SelectPrevYear { get { return (UserParams.CustomParam("PrevYear")); } }
        string page_title = "Мониторинг инвестиций в основной капитал, направленных на охрану окружающей среды и рациональное использование природных ресурсов";
       

        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        { 
            base.Page_PreLoad(sender, e);
            Label1.Text = page_title;
            Page.Title = page_title;
            
            if (!(IsSmallResolution))
            {
                Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
                Grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 150);
            }
            else
            {
                Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 500);
                Grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9);
            }
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

        }






        private void FillCombo()
        {
            DataTable Table = new DataTable();

            Dictionary<string, int> dict = new Dictionary<string, int>();


            Table = DataProvider.GetDataTableForChart("Table1", DataProvidersFactory.SpareMASDataProvider);


            foreach (DataRow row in Table.Rows)
            {
                dict.Add(row[0].ToString() + " год", 0);
            }

            if (!Page.IsPostBack)
            {
                ComboYear.FillDictionaryValues(dict);
                ComboYear.SelectLastNode();
                CheckBox1.AutoPostBack = true;
                ComboYear.Title = "Период";
                ComboYear.Width = 200;
                string page_sub_title = "Данные ежегодного мониторинга инвестиций в основной капитал, направленных на охрану окружающей среды и рациональное использование природных ресурсов за " + ComboYear.SelectedValue;
                Label2.Text = page_sub_title;
            }
         }

        private void ChosenParam()
        {
            SelectYear.Value = ComboYear.SelectedValue.Replace(" год", "");
            SelectPrevYear.Value = (int.Parse(SelectYear.Value) - 1).ToString();

        }

        object GetDeviation(object o1, object o2)
        {
            if (o1 != DBNull.Value && o2 != DBNull.Value)
            {
                return (decimal)o1 - (decimal)o2;
            }
            return DBNull.Value;
        }


        private object GetSpeedDeviation(object o1, object o2)
        {
            if (o1 != DBNull.Value && o2 != DBNull.Value)
            {
                return (((decimal)o1/(decimal)o2)*100);
            }
            return DBNull.Value;
        }


        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {

            public bool checkbox1;

            #region Члены IComparer<RegionValue>

            public int Compare(DataRow x, DataRow y)
            {
                return -Compare_(x, y);
            }

            public int Compare_(DataRow x, DataRow y)
            {
                
                string Xname = x[0].ToString().Split(";")[0].Replace("муниципальный район", "МР").Replace("Город", "г.");
                string Yname = y[0].ToString().Split(";")[0].Replace("муниципальный район", "МР").Replace("Город", "г.");
                int NXname = 0;
                int NYname = 0;
                if (checkbox1 == true)
                {
                    NXname = int.Parse(x[3].ToString());
                    NYname = int.Parse(y[3].ToString());
                }
                if (Xname == Yname)
                {
                    if (checkbox1 == true)
                    {
                        if (NXname > NYname)
                        {
                            return -1;
                        }
                        if (NXname < NYname)
                        {
                            return 1;
                        }
                        if (NXname == NYname)
                        {
                            return 0;
                        }
                    }
                    else { 
                    //тту смотрим номер, если болше то 1 иъесли менше то -1 а

                        return 0;
                    }
                }

                if (Xname.Contains("г. Ханты-Мансийск"))
                {
                    return 1;
                }

                if (Yname.Contains("г. Ханты-Мансийск"))
                {
                    return -1;
                }
                if ((Xname[0] == 'г') && (Yname[0] != 'г'))
                {
                    return 1;
                }

                if ((Xname[0] != 'г') && (Yname[0] == 'г'))
                {
                    return -1;
                }
                

                return Yname.CompareTo(Xname);
            }

            #endregion
        }

        DataTable SortTable(DataTable Table)
        {
            DataTable TableSort = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                //CRHelper.SaveToErrorLog(col.ColumnName + "|" + col.DataType.ToString());
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }

            List<DataRow> LR = new System.Collections.Generic.List<DataRow>();

            foreach (DataRow row in Table.Rows)
            {
                LR.Add(row);
            }
            SortDataRow ddd = new SortDataRow();
            ddd.checkbox1 = CheckBox1.Checked;
            LR.Sort(ddd);

            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);                
            }
            return TableSort;
        }
        
        
        void DataBindGrid()
        {

            Grid.Bands.Clear();
           
            if (CheckBox1.Checked)
            {
                 BaseTable = DataProvider.GetDataTableForChart("Table", DataProvidersFactory.SpareMASDataProvider);
            }
            else
            {
                 BaseTable = DataProvider.GetDataTableForChart("Table2", DataProvidersFactory.SpareMASDataProvider);
            }
            
           BaseTable = SortTable(BaseTable);

            DataTable TableReport = new DataTable();

            TableReport.Columns.Add("Region");
            if (CheckBox1.Checked)
            {
                TableReport.Columns.Add("Detail");
                TableReport.Columns.Add("number");
                TableReport.Columns.Add("EdIzm");
            }
            
            TableReport.Columns.Add("Val1;cur", typeof(decimal));
            TableReport.Columns.Add("Val1;prev", typeof(decimal));
            TableReport.Columns.Add("Val2;cur", typeof(decimal));
            TableReport.Columns.Add("Val2;prev", typeof(decimal));


            foreach (DataRow row in BaseTable.Rows)
            {
                DataRow NewRow = TableReport.NewRow();
                TableReport.Rows.Add(NewRow);

                DataRow DeviationRow = TableReport.NewRow();
                TableReport.Rows.Add(DeviationRow);

                DataRow SpeedDeviationRow = TableReport.NewRow();
                TableReport.Rows.Add(SpeedDeviationRow);



                NewRow["Region"] = row[0].ToString().Split(";")[0].Replace("муниципальный район", "МР").Replace("Город", "г.");
                DeviationRow["Region"] = row[0].ToString().Split(";")[0].Replace("муниципальный район", "МР").Replace("Город", "г.");
                SpeedDeviationRow["Region"] = row[0].ToString().Split(";")[0].Replace("муниципальный район", "МР").Replace("Город", "г.");
                
                if (CheckBox1.Checked)
                {
                    NewRow["Detail"] = row[0].ToString().Split(";")[1];
                    DeviationRow["Detail"] = row[0].ToString().Split(";")[1];
                    SpeedDeviationRow["Detail"] = row[0].ToString().Split(";")[1];
                    NewRow["number"] = row[3];
                    DeviationRow["number"] = row[3];
                    SpeedDeviationRow["number"] = row[3];
                    NewRow["EdIzm"] = row[4].ToString().Replace("Неизвестные данные", "");
                    DeviationRow["EdIzm"] = row[4];
                    SpeedDeviationRow["EdIzm"] = row[4];
                }
                NewRow["Val1;cur"] = row[1];
                DeviationRow["Val1;cur"] = GetDeviation(row[1],row[2]);
                SpeedDeviationRow["Val1;cur"] = GetSpeedDeviation(row[1], row[2]);
                NewRow["Val1;prev"] = row[2];

                NewRow["Val2;cur"] = row[5];
                DeviationRow["Val2;cur"] = GetDeviation(row[5], row[6]);
                SpeedDeviationRow["Val2;cur"] = GetSpeedDeviation(row[5], row[6]);
                NewRow["Val2;prev"] = row[6];
               
            }




            Grid.DataSource =TableReport;
            
            Grid.DataBind();
            Grid.Height = CRHelper.GetScreenHeight - 200;
        }

        


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            FillCombo();

            ChosenParam();

            DataBindGrid();

            ConfigurateGrid();

            ConfigurationMap();





        
        }

        private void FormatRow()
        {
            

            //Region
            UltraGridRow StartMerge =  Grid.Rows[3];
            UltraGridRow StartMerge1 = Grid.Rows[1];
            UltraGridRow StartMerge2 = Grid.Rows[2];
            UltraGridRow StartMerge3 = Grid.Rows[0];
            style = "background-repeat: no-repeat;background-position:15px";
            //else
            //{ style = "background-repeat: no-repeat;background-position: 40px"; }

            foreach (UltraGridRow row in Grid.Rows)
            {
                string Detail = row.Cells.FromKey("Detail").Text;
                string Region1 = row.Cells.FromKey("Region").Text;
                string SMRRegion2 = StartMerge.Cells.FromKey("Region").Text;
                string SMRRegion = StartMerge.Cells.FromKey("Detail").Text;
                string SMRRegion1 = StartMerge1.Cells.FromKey("Detail").Text;
                string SMRRegionN = StartMerge3.Cells.FromKey("Region").Text;
                            
                if ((Detail != SMRRegion)||(Region1 != SMRRegion2))
                {
                    //CRHelper.SaveToErrorLog((StartMerge.Index - row.Index - 1).ToString());
                    StartMerge.Cells.FromKey("Detail").RowSpan = row.Index - StartMerge.Index;
                    StartMerge.Cells.FromKey("number").RowSpan = row.Index - StartMerge.Index;
                    StartMerge.Cells.FromKey("EdIzm").RowSpan = row.Index - StartMerge.Index;
                    //
                    StartMerge2.PrevRow.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.NextRow.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                    //
                    StartMerge2.PrevRow.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.NextRow.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                    //
                    StartMerge2.PrevRow.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.NextRow.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                    //
                    StartMerge2.PrevRow.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.NextRow.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                    StartMerge = row;
                    StartMerge2 = row.NextRow;
                }
                if (Region1 != SMRRegionN)
                {
                    StartMerge3.Cells.FromKey("Region").RowSpan = row.Index - StartMerge3.Index;
                    StartMerge3 = row;                     
                }

                if (Math.Round(Convert.ToDouble(StartMerge2.Cells.FromKey("Val1;cur").Value), 2) < 0)
                {
                    StartMerge2.Cells.FromKey("Val1;cur").Title = "Отклонение от предыдущего периода";
                    StartMerge2.NextRow.Cells.FromKey("Val1;cur").Title = "Темп прироста к предыдущему периоду";
                    StartMerge2.Cells.FromKey("Val1;cur").Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                    StartMerge2.Cells.FromKey("Val1;cur").Style.CustomRules = style;   
                }
                if (Math.Round(Convert.ToDouble(StartMerge2.Cells.FromKey("Val1;cur").Value), 2) > 0)
                {
                    StartMerge2.Cells.FromKey("Val1;cur").Title = "Отклонение от предыдущего периода";
                    StartMerge2.NextRow.Cells.FromKey("Val1;cur").Title = "Темп прироста к предыдущему периоду";
                    StartMerge2.Cells.FromKey("Val1;cur").Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                    StartMerge2.Cells.FromKey("Val1;cur").Style.CustomRules = style;
                }
                if (Math.Round(Convert.ToDouble(StartMerge2.Cells.FromKey("Val2;cur").Value), 1) < 0)
                {
                    StartMerge2.Cells.FromKey("Val2;cur").Title = "Отклонение от предыдущего периода";
                    StartMerge2.NextRow.Cells.FromKey("Val2;cur").Title = "Темп прироста к предыдущему периоду";
                    StartMerge2.Cells.FromKey("Val2;cur").Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                    StartMerge2.Cells.FromKey("Val2;cur").Style.CustomRules = style;
                }
                if (Math.Round(Convert.ToDouble(StartMerge2.Cells.FromKey("Val2;cur").Value), 1) > 0)
                {
                    StartMerge2.Cells.FromKey("Val2;cur").Title = "Отклонение от предыдущего периода";
                    StartMerge2.NextRow.Cells.FromKey("Val2;cur").Title = "Темп прироста к предыдущему периоду";
                    StartMerge2.Cells.FromKey("Val2;cur").Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                    StartMerge2.Cells.FromKey("Val2;cur").Style.CustomRules = style;
                }

            }

            StartMerge.Cells.FromKey("Detail").RowSpan = Grid.Rows.Count - StartMerge.Index;
            StartMerge.Cells.FromKey("number").RowSpan = Grid.Rows.Count - StartMerge.Index;
            StartMerge.Cells.FromKey("EdIzm").RowSpan = Grid.Rows.Count - StartMerge.Index;
            StartMerge3.Cells.FromKey("Region").RowSpan = Grid.Rows.Count - StartMerge3.Index;
            StartMerge2.PrevRow.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.NextRow.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
            //
            StartMerge2.PrevRow.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.NextRow.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
            //
            StartMerge2.PrevRow.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.NextRow.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
            //
            StartMerge2.PrevRow.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.NextRow.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
        }


        private void FormatRowN()
        {


            //Region
            UltraGridRow StartMerge = Grid.Rows[3];
            UltraGridRow StartMerge1 = Grid.Rows[1];
            UltraGridRow StartMerge2 = Grid.Rows[2];
            style = "background-repeat: no-repeat;background-position:15px";
            //else
            //{ style = "background-repeat: no-repeat;background-position: 40px"; }

            foreach (UltraGridRow row in Grid.Rows)
            {
                string Region1 = row.Cells.FromKey("Region").Text;
                string SMRRegion2 = StartMerge.Cells.FromKey("Region").Text;
     
                if (Region1 != SMRRegion2)
                {
                    StartMerge.Cells.FromKey("Region").RowSpan = row.Index - StartMerge.Index; 
                    StartMerge2.PrevRow.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.NextRow.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                    //
                    StartMerge2.PrevRow.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.NextRow.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                    //
                    StartMerge2.PrevRow.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.NextRow.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                    //
                    StartMerge2.PrevRow.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
                    StartMerge2.NextRow.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
                    StartMerge = row;
                    StartMerge2 = row.NextRow;
                }
                if (Math.Round(Convert.ToDouble(StartMerge2.Cells.FromKey("Val1;cur").Value), 2) < 0)
                {
                    StartMerge2.Cells.FromKey("Val1;cur").Title = "Отклонение от предыдущего периода";
                    StartMerge2.NextRow.Cells.FromKey("Val1;cur").Title = "Темп прироста к предыдущему периоду";
                    StartMerge2.Cells.FromKey("Val1;cur").Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                    StartMerge2.Cells.FromKey("Val1;cur").Style.CustomRules = style;
                }
                if (Math.Round(Convert.ToDouble(StartMerge2.Cells.FromKey("Val1;cur").Value), 2) > 0)
                {
                    StartMerge2.Cells.FromKey("Val1;cur").Title = "Отклонение от предыдущего периода";
                    StartMerge2.NextRow.Cells.FromKey("Val1;cur").Title = "Темп прироста к предыдущему периоду";
                    StartMerge2.Cells.FromKey("Val1;cur").Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                    StartMerge2.Cells.FromKey("Val1;cur").Style.CustomRules = style;
                }
                if (Math.Round(Convert.ToDouble(StartMerge2.Cells.FromKey("Val2;cur").Value), 1) < 0)
                {
                    StartMerge2.Cells.FromKey("Val2;cur").Title = "Отклонение от предыдущего периода";
                    StartMerge2.NextRow.Cells.FromKey("Val2;cur").Title = "Темп прироста к предыдущему периоду";
                    StartMerge2.Cells.FromKey("Val2;cur").Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                    StartMerge2.Cells.FromKey("Val2;cur").Style.CustomRules = style;
                }
                if (Math.Round(Convert.ToDouble(StartMerge2.Cells.FromKey("Val2;cur").Value), 1) > 0)
                {
                    StartMerge2.Cells.FromKey("Val2;cur").Title = "Отклонение от предыдущего периода";
                    StartMerge2.NextRow.Cells.FromKey("Val2;cur").Title = "Темп прироста к предыдущему периоду";
                    StartMerge2.Cells.FromKey("Val2;cur").Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                    StartMerge2.Cells.FromKey("Val2;cur").Style.CustomRules = style;
                }

            }

            StartMerge.Cells.FromKey("Region").RowSpan = Grid.Rows.Count - StartMerge.Index;
            StartMerge2.PrevRow.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.NextRow.Cells.FromKey("Val1;cur").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
            //
            StartMerge2.PrevRow.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.NextRow.Cells.FromKey("Val1;prev").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
            //
            StartMerge2.PrevRow.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.NextRow.Cells.FromKey("Val2;cur").Style.BorderDetails.StyleBottom = BorderStyle.Solid;
            //
            StartMerge2.PrevRow.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.None;
            StartMerge2.NextRow.Cells.FromKey("Val2;prev").Style.BorderDetails.StyleBottom = BorderStyle.Solid;

        }


        void PDFMergRow(UltraWebGrid Grid, UltraGridRow Firstrow, UltraGridRow LastRow, int col)
        {
            string text = Firstrow.Cells[col].Text;

            for (int i = Firstrow.Index; i < LastRow.Index + 1; i++)
            {
                Grid.Rows[i].Cells[col].Text = " ";
            }

            for (int i = Firstrow.Index + 1; i < LastRow.Index; i++)
            {
                Grid.Rows[i].Cells[col].Style.BorderDetails.ColorTop = Color.White;
                Grid.Rows[i].Cells[col].Style.BorderDetails.ColorBottom = Color.White;
            }

            Firstrow.Cells[col].Style.BorderDetails.ColorBottom = Color.White;

            //if (!(string.IsNullOrEmpty(text)))
            //    if (text.Contains("|"))
            //    {
            //        text = text.Split('|')[1];
            //    }
            Grid.Rows[(Firstrow.Index + LastRow.Index) / 2].Cells[col].Text = text;
        }

        void MergeCellsGridFormPDF(UltraWebGrid Grid, int col)
        {
            UltraGridRow MerdgRow = Grid.Rows[0];
            foreach (UltraGridRow Row in Grid.Rows)
            {
                if (Row.Cells[col].RowSpan > 1)
                {
                    PDFMergRow(Grid, Row, Grid.Rows[Row.Index + Row.Cells[col].RowSpan - 1], col);
                }
            }
        }


        private void  ConfigurateGrid()
        {
            Grid.DisplayLayout.RowAlternateStyleDefault.BackColor = Color.White;

            GHL = new GridHeaderLayout(Grid);
            GHL.AddCell("МР ГО");
            if (CheckBox1.Checked)
            {
                GHL.AddCell("Статьи расходов");
                GHL.AddCell("№ строки");
                GHL.AddCell("Ед. изм.");
            }
            GridHeaderCell cell = GHL.AddCell("Ввод в действие мощностей и объектов за счет всех источников финансирования");
            cell.AddCell("Отчетный период");
            cell.AddCell("Предыдущий период");
            GridHeaderCell cell1 = GHL.AddCell("Инвестиции в основной капитал за счет всех источников финансирования, тыс. рублей");
            cell1.AddCell("Отчетный период");
            cell1.AddCell("Предыдущий период");
            
            GHL.ApplyHeaderInfo();

            

            foreach (HeaderBase hb in Grid.Bands[0].HeaderLayout)
            {
                hb.Style.Wrap = true;
                hb.Style.HorizontalAlign = HorizontalAlign.Center;
                hb.Style.VerticalAlign = VerticalAlign.Middle;
            }

            Grid.Columns[0].Width = 130;
            if (CheckBox1.Checked)
            {
                Grid.Columns[1].Width = 340;
                Grid.Columns[2].Width = 65;
                Grid.Columns[3].Width = 200;
                Grid.Columns[4].Width = 97;
                Grid.Columns[5].Width = 97;
                Grid.Columns[6].Width = 97;
                Grid.Columns[7].Width = 97;
            }
            else 
            {
                Grid.Columns[1].Width = 97;
                Grid.Columns[2].Width = 97;
                Grid.Columns[3].Width = 97;
                Grid.Columns[4].Width = 97;
             }


            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("Val1;cur"), "N2");//N2
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("Val1;prev"), "N2");//N2
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("Val2;cur"), "N1");//N2
            CRHelper.FormatNumberColumn(Grid.Columns.FromKey("Val2;prev"), "N1");//N2

            if (CheckBox1.Checked)
            {
                Grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                Grid.Columns[4].CellStyle.Padding.Right = 5;
                Grid.Columns[5].CellStyle.Padding.Right = 5;
                Grid.Columns[6].CellStyle.Padding.Right = 5;
                Grid.Columns[7].CellStyle.Padding.Right = 5;
                Grid.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                Grid.Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Grid.Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Grid.Columns[6].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Grid.Columns[7].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                Grid.Columns[1].CellStyle.Wrap = true;
                Grid.Columns[3].CellStyle.Wrap = true;
                Grid.Columns[1].CellStyle.Padding.Left = 5;
                Grid.Columns[2].CellStyle.Padding.Left = 5;
                FormatRow();
            }
            else 
            {
                Grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                Grid.Columns[1].CellStyle.Padding.Right = 5;
                Grid.Columns[2].CellStyle.Padding.Right = 5;
                Grid.Columns[3].CellStyle.Padding.Right = 5;
                Grid.Columns[4].CellStyle.Padding.Right = 5;
                Grid.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Grid.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Grid.Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Grid.Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                Grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
                FormatRowN();            
            }
            
        }

        


        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 30;
            if (CheckBox1.Checked)
            {
                MergeCellsGridFormPDF(Grid, 0);
                MergeCellsGridFormPDF(Grid, 1);
                MergeCellsGridFormPDF(Grid, 2);
                MergeCellsGridFormPDF(Grid, 3);
            }
            else
            {
                MergeCellsGridFormPDF(Grid, 0);
            }
            ReportPDFExporter1.Export(GHL, section1);

           

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));


            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            section2.PageSize = new PageSize(700, 900);
            ReportPDFExporter1.Export(DundasMap, Label3.Text, section2);
            section2.PageSize = new PageSize(700, 900);
        }
        #endregion


        #region DundasMap
        protected void CustomizeMapPanel()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = 1 == 1;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap.NavigationPanel.Visible = 1 == 1;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.Viewport.EnablePanning = true;
        }

        protected void AddShapeField()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Value");
            DundasMap.ShapeFields["Value"].Type = typeof(double);
            DundasMap.ShapeFields["Value"].UniqueIdentifier = false;
        }

        protected string SetHyphen(string BaseString)
        {


            string[] Word = BaseString.Split(' ');
            int Median = Word.Length / 2;

            string res = "";
            for (int i = 0; i < Median; i++)
            {
                res += Word[i] + " ";
            }
            res += "\n";
            for (int i = Median; i < Word.Length; i++)
            {
                res += Word[i] + " ";
            }

            return res;

        }

        protected void AddLegend()
        {
            DundasMap.Legends.Clear();
            Legend legend1 = new Legend("CompleteLegend");
            Label3.Text = "Инвестиции в основной капитал за счет всех источников финансирования в " + ComboYear.SelectedValue.Replace(" год","") + " году, тыс. рублей" + "\n" + "\n";
            legend1.Title = "Инвестиции в основной капитал," + "\n" + "тыс. рублей";
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Right;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            //legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;

            legend1.AutoFitMinFontSize = 7;

            DundasMap.Legends.Add(legend1);
        }

        protected void FillRule()
        {

            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorValueRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Value";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            
            rule.MiddleColor = Color.Yellow;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";

            rule.LegendText = "#FROMVALUE{N2} - #TOVALUE{N2}";//"LegendText";
            DundasMap.ShapeRules.Add(rule);
        }

        protected string ShorteningFO(string LongName)
        {
            switch (LongName)
            {
                case "Центральный федеральный округ": { return "ЦФО"; }
                case "Северо-Западный федеральный округ": { return "СФО"; }
                case "Южный федеральный округ": { return "ЮФО"; }
                case "Северо-Кавказский федеральный округ": { return "СКФО"; }
                case "Приволжский федеральный округ": { return "ПФО"; }
                case "Уральский федеральный округ": { return "УрФО"; }
                case "Сибирский федеральный округ": { return "СФО"; }
                case "Дальневосточный федеральный округ": { return "ДФО"; }
            }

            throw new Exception("Неизвестный регион");
        }

        protected void LoadMap()
        {

            string mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapPath");

            //что нють то точно загрузится ;)
            try
            {
                AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
            }
            catch { }
            //ии
            try
            {
                AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.Areas);
            }
            catch { }
            try
            {
                AddMapLayer(DundasMap, mapFolderName, "Граница", CRHelper.MapShapeType.SublingAreas);
            }
            catch { }

        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            //string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            string layerName = Server.MapPath(string.Format("../../../maps/Субъекты/ХМАОDeer/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;
            if (!(IsSmallResolution))
            {
                DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.94));
                DundasMap.Height = Unit.Pixel((int)(CustomReportConst.minScreenHeight * 0.7));
            }
            else
            {
                DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.5));
                DundasMap.Height = Unit.Pixel((int)(CustomReportConst.minScreenHeight * 0.5));
            }
            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        protected void ConfigurationMap()
        {
            CustomizeMapPanel();

            AddShapeField();

            AddLegend();

            FillRule();

            LoadMap();

            DataBindMap();
        }



        string GetNormalNameRegionMap(string s)
        {
            return s.Replace("_callout", "").Replace(" ", "").Replace("р-н", "");
        }

        string GetNormalNameRegion(string s)
        {
            return s.Split(";")[0].Replace("муниципальный район", "").Replace("Город", "г.").Replace(" ", "");
        }


        protected void DataBindMap()
        {
            MapTable = DataProvider.GetDataTableForChart("MapTable", DataProvidersFactory.SpareMASDataProvider);

            foreach (Shape sh2 in DundasMap.Shapes)
            {
                foreach (DataRow row in MapTable.Rows)
                {
                    //CRHelper.SaveToErrorLog(sh2.Name.Replace("_callout", "") + "|" + row[0].ToString().Split(";")[0].Replace("муниципальный район", "").Replace("Город", "г.").Replace(" ", ""));
                    if (GetNormalNameRegionMap(sh2.Name) == GetNormalNameRegion(row[0].ToString()))
                    {
                        //CRHelper.SaveToErrorLog(sh.Name.Replace("_callout", "") + "|" + row[0].ToString().Split(";")[0].Replace("муниципальный район", "").Replace("Город", "г.").Replace(" ", ""));
                        sh2.Text = row[0].ToString().Split(";")[0].Replace("муниципальный район", "р-н").Replace("Город", "г.");
                        if (row[1] != DBNull.Value)
                        {
                            sh2["Value"] = (Double)(Decimal)row[1];
                            sh2.Text = sh2.Text + "\n" + string.Format("{0:### ### ##0.00}", row[1]);
                            sh2.ToolTip = "Инвестиции в основной капитал " + string.Format("{0:### ### ##0.00}", row[1]) + " тыс. рублей, " + row[0].ToString().Split(";")[0].Replace("муниципальный район", "МР").Replace("Город", "г.");
                        }
                        //else
                        //{
                        //    sh2["Value"] = 0;
                        //    sh2.Text = sh2.Text + "\n" + string.Format("{0:### ### ##0.00}", 0);
                        //    sh2.ToolTip = "Инвестиции в основной капитал " + string.Format("{0:### ### ##0.00}", 0) + " тыс. рублей, " + row[0].ToString().Split(";")[0].Replace("муниципальный район", "МР").Replace("Город", "г.");
                        //}

                    }
                    sh2.TextVisibility = TextVisibility.Shown;

                }


            }

        }

        #endregion



        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            //new GridHeaderLayout(Grid);
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            //  SetExportGridParams(GHL.Grid);
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 0;
            ReportExcelExporter1.RowsAutoFitEnable = true;
            ReportExcelExporter1.Export(GHL, sheet1, 4);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

           


        }

       

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;



        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            if (!(CheckBox1.Checked))
            {
                e.CurrentWorksheet.MergedCellsRegions.RemoveAt(0);
                e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 10);
                e.CurrentWorksheet.MergedCellsRegions.RemoveAt(0);
                e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 10);
                e.CurrentWorksheet.Rows[0].Height = 800;
                e.CurrentWorksheet.Rows[1].Height = 800;
                int ob = 6;
                for (int i = 1; i <= Grid.Rows.Count / 3; i++)
                {
                    e.CurrentWorksheet.MergedCellsRegions.Add(ob, 0, ob + 2, 0);
                    ob = ob + 3;

                }

                e.Workbook.Worksheets["Карта"].PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
                e.Workbook.Worksheets["Карта"].PrintOptions.PaperSize = PaperSize.A4;
                e.Workbook.Worksheets["Карта"].PrintOptions.BottomMargin = 0.25;
                e.Workbook.Worksheets["Карта"].PrintOptions.TopMargin = 0.25;
                e.Workbook.Worksheets["Карта"].PrintOptions.LeftMargin = 0.25;
                e.Workbook.Worksheets["Карта"].PrintOptions.RightMargin = 0.25;
                e.Workbook.Worksheets["Карта"].PrintOptions.ScalingType = ScalingType.FitToPages;
                DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
                DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].CellFormat.Font.Height = 220;
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].Value = Label3.Text;
                e.Workbook.Worksheets["Карта"].Rows[0].Height = 550;
                ReportExcelExporter.MapExcelExport(e.Workbook.Worksheets["Карта"].Rows[1].Cells[0], DundasMap);
                e.Workbook.Worksheets["Карта"].Rows[3].Cells[0].CellFormat.Font.Name = e.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Font.Name;
                e.Workbook.Worksheets["Карта"].Rows[3].Cells[0].CellFormat.Font.Height = e.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Font.Height;


            }
            else
            {

                e.CurrentWorksheet.MergedCellsRegions.RemoveAt(0);
                e.CurrentWorksheet.MergedCellsRegions.Add(0, 0, 0, 7);
                e.CurrentWorksheet.MergedCellsRegions.RemoveAt(0);
                e.CurrentWorksheet.MergedCellsRegions.Add(1, 0, 1, 7);
                e.CurrentWorksheet.Rows[0].Height = 800;
                e.CurrentWorksheet.Rows[1].Height = 800;

                UltraGridRow StartMerge = Grid.Rows[0];
                int n = 6;
                foreach (UltraGridRow row in Grid.Rows)
                {
                    string Region1 = row.Cells.FromKey("Region").Text;
                    string SMRRegion2 = StartMerge.Cells.FromKey("Region").Text;

                    if (Region1 != SMRRegion2)
                    {
                        e.CurrentWorksheet.MergedCellsRegions.Add(n, 0, n - 1 + (row.Index - StartMerge.Index), 0);
                        n = n + (row.Index - StartMerge.Index);
                        StartMerge = row;                       
                    }
                }
                e.CurrentWorksheet.MergedCellsRegions.Add(n, 0, n - 1 + (Grid.Rows.Count - StartMerge.Index), 0);
                //e.CurrentWorksheet.MergedCellsRegions.Add(n, 0, n - 1 + (15), 0);
                int ob = 6;
                for (int i = 1; i <= Grid.Rows.Count/3; i++)
                {
                    e.CurrentWorksheet.MergedCellsRegions.Add(ob, 1, ob + 2, 1);
                    ob = ob + 3;
                    
                }
                ob = 6;
                for (int i = 1; i <= Grid.Rows.Count / 3; i++)
                {
                    e.CurrentWorksheet.MergedCellsRegions.Add(ob, 2, ob + 2, 2);
                    ob = ob + 3;

                }
                ob = 6;
                for (int i = 1; i <= Grid.Rows.Count / 3; i++)
                {
                    e.CurrentWorksheet.MergedCellsRegions.Add(ob, 3, ob + 2, 3);
                    ob = ob + 3;

                }
               // e.CurrentWorksheet.MergedCellsRegions.Add(ob, 1, ob + 3, 1);

                e.Workbook.Worksheets["Карта"].PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
                e.Workbook.Worksheets["Карта"].PrintOptions.PaperSize = PaperSize.A4;
                e.Workbook.Worksheets["Карта"].PrintOptions.BottomMargin = 0.25;
                e.Workbook.Worksheets["Карта"].PrintOptions.TopMargin = 0.25;
                e.Workbook.Worksheets["Карта"].PrintOptions.LeftMargin = 0.25;
                e.Workbook.Worksheets["Карта"].PrintOptions.RightMargin = 0.25;
                e.Workbook.Worksheets["Карта"].PrintOptions.ScalingType = ScalingType.FitToPages;
                DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
                DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6);
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].CellFormat.Font.Height = 220;
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
                e.Workbook.Worksheets["Карта"].Rows[0].Cells[0].Value = Label3.Text;
                e.Workbook.Worksheets["Карта"].Rows[0].Height = 550;
                ReportExcelExporter.MapExcelExport(e.Workbook.Worksheets["Карта"].Rows[1].Cells[0], DundasMap);
                // sheet1.Rows[3].Cells[0].Value = GridHeader.Text;
                e.Workbook.Worksheets["Карта"].Rows[3].Cells[0].CellFormat.Font.Name = e.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Font.Name;
                e.Workbook.Worksheets["Карта"].Rows[3].Cells[0].CellFormat.Font.Height = e.Workbook.Worksheets["Таблица"].Rows[1].Cells[0].CellFormat.Font.Height;

                //  e.CurrentWorksheet.MergedCellsRegions.Add(
            }
        }
        #endregion




        }





    }
