using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0005
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtIndicatorDescription;
        private int firstYear = 2009;
        private int endYear = 2011;
        private static Dictionary<string, string> indicatorDescripitonList;
        private DataTable mainGridDt = new DataTable();

        #endregion

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;
        // выбранная группа МО
        private CustomParam selectedMOGroup;
        // уровень районов
        private CustomParam regionLevel;
        // уровень поселений
        private CustomParam settlementLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка гридов

            MainGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            MainGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 250);
            MainGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 20);
            MainGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(MainGrid_InitializeLayout);
            MainGridBrick.Grid.InitializeRow += new InitializeRowEventHandler(MainGrid__InitializeRow);

            #endregion

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }

            if (selectedMOGroup == null)
            {
                selectedMOGroup = UserParams.CustomParam("selected_mo_group");
            }
            regionLevel = UserParams.CustomParam("region_level");
            settlementLevel = UserParams.CustomParam("settlement_level");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            settlementLevel.Value = RegionSettingsHelper.Instance.SettlementLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0005_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string baseQuarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 170;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.SetСheckedState(baseQuarter, true);

                ComboMOGroup.Title = "Уровень дотационности";
                ComboMOGroup.Width = 350;
                ComboMOGroup.MultiSelect = false;
                ComboMOGroup.ParentSelect = true;
                ComboMOGroup.FillDictionaryValues(CustomMultiComboDataHelper.FillBKKUGroupMO());
                ComboMOGroup.SetСheckedState("Все группы ", true);
            }

            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            if (ComboMOGroup.SelectedValue == "Все группы ")
            {
                selectedMOGroup.Value = "(true)";
            }
            else
            {
                selectedMOGroup.Value = string.Format("([Показатели__БККУ_Сопоставимый].[Показатели__БККУ_Сопоставимый].[{1}], [Measures].[Значение ]) = {0}",
                    ComboMOGroup.SelectedIndex + 1, ComboYear.SelectedValue.Contains("2009") ? "БК 1" : "Группа МО ");
            }

            Page.Title = string.Format("Информация о превышении показателей мониторинга соблюдения органами местного самоуправления основных условий предоставления межбюджетных трансфертов");
            PageTitle.Text = Page.Title;
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = string.Format("за {0} квартал {1} года (в соответствии с приказом МФ и НП НСО от 30.03.2012 № 128)", quarterNum, yearNum);
            CommentTextLabel.Visible = false;

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", quarterNum);

            IndicatorDescriptionDataBind();
            
            MainGridDataBind();
        }

        #region Обработчики грида

        private string GetIndicatorName(string description)
        {
            if (indicatorDescripitonList != null && indicatorDescripitonList.Count != 0)
            {
                foreach (string key in indicatorDescripitonList.Keys)
                {
                    string indicatorName = indicatorNameDictionary[key];
                    if (subGroupNameDictionary.ContainsKey(key) && subGroupNameDictionary[key] != String.Empty)
                    {
                        indicatorName = subGroupNameDictionary[key];
                    }

                    if (indicatorDescripitonList[key] == description)
                    {
                        return String.Format("{0} ({1})", indicatorName, key);
                    }
                }
            }
            return description;
        }

        private Dictionary<string, string> codeNumDictionary;
        private Dictionary<string, string> groupNameDictionary;
        private Dictionary<string, string> subGroupNameDictionary;
        private Dictionary<string, string> indicatorNameDictionary;

        private void IndicatorDescriptionDataBind()
        {
            indicatorDescripitonList = new Dictionary<string, string>();
            codeNumDictionary = new Dictionary<string, string>();
            groupNameDictionary = new Dictionary<string, string>();
            subGroupNameDictionary = new Dictionary<string, string>();
            indicatorNameDictionary = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0016_0005_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();
                string limit = row[2].ToString().TrimEnd('_');
                string condition = row[3].ToString();
                string codeNum = row[4].ToString();
                string groupName = row[5].ToString();
                string subGroupName = row[6].ToString();

                string descriptionTable =
                    String.Format(@"<table width='100%' height='100%' style='font-weight:normal;'>
                                        <tr>
                                           <td align='left' width='150px' > 
                                             Код
                                           </td>
                                           <td align='right' width='*'> 
                                             {0}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td align='left' width='150px' > 
                                             Пороговое значение
                                           </td>
                                           <td align='right' width='*'> 
                                             {1}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td align='left'  width='150px' > 
                                             Условие
                                           </td>
                                           <td align='right' width='*'> 
                                             {2}
                                           </td>
                                        </tr>
                                    </table>", code, limit, condition);

                string nameTable = 
                    String.Format(@"<table width='100%' height='100%'>
                                        <tr>
                                           <td height='100px'> 
                                             {0}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td valing='bottom'> 
                                             {1}
                                           </td>
                                        </tr>
                                    </table>", subGroupName == String.Empty ? name : subGroupName, descriptionTable);

                indicatorDescripitonList.Add(code, nameTable);
                codeNumDictionary.Add(code, codeNum);
                groupNameDictionary.Add(code, groupName);
                subGroupNameDictionary.Add(code, subGroupName);
                indicatorNameDictionary.Add(code, name);
            }
        }

        private void MainGridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0016_0005_grid");
            mainGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", mainGridDt);

            if (mainGridDt.Rows.Count > 0)
            {
                foreach (DataRow row in mainGridDt.Rows)
                {
                    if (row[1] != DBNull.Value)
                    {
                        row[1] = row[1].ToString().Replace("район", "р-н");
                    }
                }

                FontRowLevelRule fontRule = new FontRowLevelRule(mainGridDt.Columns.Count - 1);
                fontRule.AddFontLevel("0", MainGridBrick.BoldFont8pt);
                MainGridBrick.AddIndicatorRule(fontRule);

                MainGridBrick.DataTable = mainGridDt;
            }
        }

        protected void MainGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string formatString = (i > 3 && i % 2 == 0) ? "N2" : "N0";
                int widthColumn = (i > 2) ? 110 : 100;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;


            GridHeaderLayout headerLayout = MainGridBrick.GridHeaderLayout;

            headerLayout.AddCell("Наименование МО", "", 5);
            headerLayout.AddCell("Район", "", 5);
            headerLayout.AddCell("Уровень дотационности", "", 5);
            headerLayout.AddCell("Общее количество нарушений", "", 5);

            for (int i = 4; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                int spanY;
                GridHeaderCell groupCell = GetGroupCell(groupNameDictionary[captions[0]], out spanY);
                GridHeaderCell indicatorCell = AddChildCell(groupCell, captions[0], spanY);

                indicatorCell.AddCell("Значение", "");
                indicatorCell.AddCell("Количество", "");
            }

            headerLayout.ApplyHeaderInfo();
        }

        private GridHeaderCell AddChildCell(GridHeaderCell groupCell, string cellName, int spanYOffset)
        {
            string subGroupName = subGroupNameDictionary[cellName];

            if (subGroupName != String.Empty)
            {
                string indicatorName = indicatorNameDictionary[cellName];
                GridHeaderCell subGroupCell = groupCell.GetChildCellByCaption(indicatorName);
                if (subGroupCell == groupCell)
                {
                    subGroupCell = groupCell.AddCell(indicatorName);
                }

                return subGroupCell.AddCell(indicatorDescripitonList[cellName]);
            }

            return groupCell.AddCell(indicatorDescripitonList[cellName], spanYOffset + 1);
        }

        private GridHeaderCell GetGroupCell(string name, out int spanY)
        {
            string[] nameParts = name.Split(';');
            string subName = String.Empty;

            switch (nameParts.Length)
            {
                case 2:
                    {
                        spanY = 1;
                        name = nameParts[0];
                        subName = nameParts[1];
                        break;
                    }
                default:
                    {
                        spanY = 2;
                        break;
                    }
            }

            GridHeaderCell groupCell = MainGridBrick.GridHeaderLayout.GetChildCellByCaption(name);
            if (groupCell == MainGridBrick.GridHeaderLayout)
            {
                groupCell = MainGridBrick.GridHeaderLayout.AddCell(name);
            }

            if (subName != String.Empty)
            {
                GridHeaderCell subGroupCell = groupCell.GetChildCellByCaption(subName);
                if (subGroupCell == groupCell)
                {
                    subGroupCell = groupCell.AddCell(subName);
                }

                return subGroupCell;
            }

            return groupCell;
        }

        protected void MainGrid__InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool crimeCountColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Количество");

                if (crimeCountColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    int value = Convert.ToInt32(e.Row.Cells[i].Value.ToString());
                    if (value == 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                        e.Row.Cells[i].Title = string.Format("Не соблюдается условие");
                    }
                    else if (value == 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                        e.Row.Cells[i].Title = string.Format("Соблюдается условие");
                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: right top; margin: 0px";
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void SetExportHeader(GridHeaderLayout headerLayout)
        {
            foreach (GridHeaderCell cell in headerLayout.childCells)
            {
                SetExportHeaderCell(cell);
            }
        }

        private void SetExportHeaderCell(GridHeaderCell headerCell)
        {
            headerCell.Caption = GetIndicatorName(headerCell.Caption);

            foreach (GridHeaderCell cell in headerCell.childCells)
            {
                SetExportHeaderCell(cell);
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            MainGridBrick.Grid.DisplayLayout.SelectedRows.Clear();

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            SetExportHeader(MainGridBrick.GridHeaderLayout);
            ReportExcelExporter1.Export(MainGridBrick.GridHeaderLayout, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            SetExportHeader(MainGridBrick.GridHeaderLayout);
            ReportPDFExporter1.Export(MainGridBrick.GridHeaderLayout);
        }

        #endregion
    }
}