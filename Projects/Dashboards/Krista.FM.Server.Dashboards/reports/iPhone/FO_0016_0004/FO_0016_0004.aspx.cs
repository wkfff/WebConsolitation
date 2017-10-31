using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources.Appearance;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Components;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0016_0004: CustomReportPage
    {
        private DataTable dtGrid = new DataTable();
        private DataTable gridDate = new DataTable();
        private DataTable dtIndicatorDescription = new DataTable();

        private int indexCol_BK2;
        private int indexCol_BK4;
        private static Dictionary<string, string> indicatorNameList;
     
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Настройки таблицы

            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(Grid_InitializeRow);
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoWidth;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;

            #endregion

            IndicatorDescriptionDataBind();
            GridDataBind();
            
            //запись двух значений в ячейку
            for (int i = 2; i < GridBrick.Grid.Columns.Count - 1; i++)
            {
             
                for (int j = 1; j < GridBrick.Grid.Rows.Count; j++)
                {
               
                    GridBrick.Grid.Rows[j].Cells[i].Text = string.Format("{0}<br />{1}", GridBrick.Grid.Rows[j].Cells[i], GridBrick.Grid.Rows[j].Cells[i + 1]);
                }
            }

            //удаление столбцов
            for (int i = GridBrick.Grid.Columns.Count - 2; i > 2; i = i - 2)
            {
                GridBrick.Grid.Columns.RemoveAt(i);
            }
        }

        private void IndicatorDescriptionDataBind()
        {
            indicatorNameList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0016_0004_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();

                indicatorNameList.Add(code, name);
            }
        }

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0016_0004_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);

            if ((dtGrid.Rows.Count > 0) && (dtGrid.Columns.Count > 4))
            {
                dtGrid.Columns.RemoveAt(0);
              
                GridBrick.DataTable = dtGrid;
            }
        }
        

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            if (e.Layout.Bands[0].Columns.Count <= 2)
            {
                return;
            }

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("БК 2"))
                {
                    indexCol_BK2 = i;
                }
                if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("БК 4"))
                {
                    indexCol_BK4 = i;
                }

            }
            e.Layout.Bands[0].Columns[0].Width = 100;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = false;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Муниципальное образование";

            e.Layout.Bands[0].Columns[1].Width = 60;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].MergeCells = false;

            int widthColumn = 75;
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                e.Layout.Bands[0].Columns[i].MergeCells = false;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                e.Layout.Bands[0].Columns[i + 1].MergeCells = false;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                e.Layout.Bands[0].Columns[i + 1].Width = widthColumn;
                e.Layout.Bands[0].Columns[i + 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            headerLayout.AddCell("Группа МО");

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                string indicatorCode = e.Layout.Bands[0].Columns[i].Header.Caption;
                indicatorCode = indicatorCode.Remove(4);
                string indicatorName = indicatorCode;
                if (indicatorNameList.ContainsKey(indicatorCode))
                {
                    indicatorName = indicatorNameList[indicatorCode];
                    GridHeaderCell newCell = headerLayout.AddCell(indicatorCode + ' ' + indicatorName);
                    newCell.AddCell("Значение");
                    newCell.AddCell("Количество нарушений");
                }
            }
      
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].MergeCells = false;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Width = 75;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            headerLayout.AddCell("Общее количество нарушений");

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[1].Value = null;
                for (int i = 2; i < e.Row.Cells.Count; i++)
                {

                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[i].Style.Font.Bold = true;
                    if (i == indexCol_BK2)
                    {
                        e.Row.Cells[i].Text += " (0,50)";
                    }
                    if (i == indexCol_BK4)
                    {
                        e.Row.Cells[i].Text += " (0,05)";
                    }
                }
            }
           
        }

       
    
    }
}