using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.EO_0001_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private static MemberAttributesDigest objectDigest;
        private static MemberAttributesDigest periodDigest;
        private int rubMultiplier;

        #endregion

        #region Параметры запроса

        // выбранный объект
        private CustomParam selectedObject;
        // выбранный объект для комбобокса
        private CustomParam objectComboValue;
        // выбранный период
        private CustomParam selectedPeriod;

        #endregion

        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1200; }
        }

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Width = GridBrick.Width = IsSmallResolution ? 925 : Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow +=new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region Инициализация параметров запроса

            selectedObject = UserParams.CustomParam("selected_object");
            selectedPeriod = UserParams.CustomParam("selected_period");
            objectComboValue = UserParams.CustomParam("object_code", true);
            
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                objectComboValue.Value = "324";

                ComboPeriod.Title = "Период";
                ComboPeriod.Width = 180;
                ComboPeriod.MultiSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "EO_0001_0003_periodDigest");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();

                ComboObject.Title = "Объект";
                ComboObject.Width = 550;
                ComboObject.ParentSelect = false;
                ComboObject.MultiSelect = false;
                objectDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "EO_0001_0003_objectDigest");
                ComboObject.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(objectDigest.UniqueNames, objectDigest.MemberLevels));
                ComboObject.SetСheckedState(GetObjectByCode(objectComboValue.Value), true);
            }

            ComboObject.Visible = false;

            selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);
            currentDate = CRHelper.PeriodDayFoDate(selectedPeriod.Value);

            Page.Title = "Мониторинг объектов строительства";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{1} ({2}) на {0:dd.MM.yyyy}, {3}", currentDate.AddMonths(1), ComboObject.SelectedValue, ComboObject.SelectedNodeParent,
                IsThsRubSelected ? "тыс.руб." : "млн.руб.");

            selectedObject.Value = objectDigest.GetMemberUniqueName(ComboObject.SelectedValue);
            rubMultiplier = IsThsRubSelected ? 1000 : 1;

            GridDataBind();
        }

        private string GetObjectByCode(string code)
        {
            foreach (string objectName in objectDigest.MemberTypes.Keys)
            {
                if (objectDigest.MemberTypes[objectName] == code)
                {
                    return objectName;
                }
            }

            return String.Empty;
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0001_0003_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                foreach (DataRow row in gridDt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("на 01.01.11 г.", "на начало текущего года");
                        row[0] = row[0].ToString().Replace("на 01.01.11 года", "на начало текущего года");
                        row[0] = row[0].ToString().Replace("на 2011 год", "на текущий год");
                        row[0] = row[0].ToString().Replace("на 2012 год", "на следующий год");
                    }
                }

                FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", GridBrick.BoldFont8pt);
                GridBrick.AddIndicatorRule(levelRule);

                GridBrick.AddIndicatorRule(new PaddingRule(0, "Уровень", 10));

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(IsSmallResolution ? 370 : 510);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(IsSmallResolution ? 490 : 620);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[gridDt.Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[gridDt.Columns.Count - 2].Hidden = true;

            GridBrick.GridHeaderLayout.AddCell(" ");
            GridBrick.GridHeaderLayout.AddCell(" ");
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
//            int cellCount = e.Row.Cells.Count - 1;
//            int rowType = -1;
//            if (e.Row.Cells[cellCount].Value != null && e.Row.Cells[cellCount].Value.ToString() != String.Empty)
//            {
//                rowType = Convert.ToInt32(e.Row.Cells[cellCount].Value);
//            }
//
//            if (rowType == 0)
//            {
//                e.Row.Cells[0].ColSpan = 2;
//                e.Row.Cells[0].Style.Font.Size = 12;
//                e.Row.Cells[1].Value = null;
//            }

            string format = GetStringRowValue(e.Row, "Формат");
            int multiplier = format == "0" ? 1 : rubMultiplier;
            if (format != String.Empty)
            {
                double value = GetDoubleValue(e.Row.Cells[1].Value);
                if (value != Double.MinValue)
                {
                    e.Row.Cells[1].Value = (multiplier * value).ToString(format);
                    e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Center;
                    //e.Row.Cells[1].Style.Padding.Right = Convert.ToInt32(e.Row.Cells[1].Column.Width.Value / 2) - 20;
                }
            }
        }

        private static string GetStringRowValue(UltraGridRow row, string columnName)
        {
            for (int i = 0; i < row.Band.Columns.Count; i++)
            {
                if (row.Cells[i].Column.Header.Caption == columnName && row.Cells[i].Value != null)
                {
                    return row.Cells[i].Value.ToString();
                }
            }

            return String.Empty;
        }

        private Double GetDoubleValue(object strValue)
        {
            if (strValue != null)
            {

                decimal value;
                if (Decimal.TryParse(strValue.ToString(), out value))
                {
                    return Convert.ToDouble(value);
                }
            }

            return Double.MinValue;
        }

        #endregion
        
        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout);
        }

        #endregion
    }
}