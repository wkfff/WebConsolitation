using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FST_0002_0002_Grid : UserControl
    {
        #region Поля и свойства

        private DataTable dt;

        private string reportCode;
        public string ReportCode
        {
            get
            {
                return reportCode;
            }
            set
            {
                reportCode = value;
            }
        }

        private string reportPrefix;
        public string ReportPrefix
        {
            get
            {
                return reportPrefix;
            }
            set
            {
                reportPrefix = value;
            }
        }

        private DateTime reportDate;
        public DateTime ReportDate
        {
            get
            {
                return reportDate;
            }
            set
            {
                reportDate = value;
            }
        }

        private DateTime lastDate;
        public DateTime LastDate
        {
            get
            {
                return lastDate;
            }
            set
            {
                lastDate = value;
            }
        }

        private string serviceName = "Водоснабжение";
        public string ServiceName
        {
            get
            {
                return serviceName;
            }
            set
            {
                serviceName = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            SetGridBrick(UltraGridIncrease, "Increase");
            SetGridBrick(UltraGridDecrease, "Decrease");

            if (CustomParam.CustomParamFactory("state_area").Value == "Ханты-Мансийский автономный округ")
            {
                CustomParam.CustomParamFactory("state_area").Value = "Ханты-Мансийский автономный округ - Югра";
            }

            GridDataBind(UltraGridIncrease, "Increase", IncreaseLabel);
            GridDataBind(UltraGridDecrease, "Decrease", DecreaseLabel);

            IncomesHeader.Text = ServiceName;
        }

        private void SetGridBrick(UltraGridBrick brick, string queryPostfix)
        {
            brick.BrowserSizeAdapting = false;
            brick.Height = Unit.Empty;
            brick.Width = Unit.Empty;
            brick.RedNegativeColoring = false;

            brick.Grid.InitializeLayout += queryPostfix == "Increase" 
                ? new InitializeLayoutEventHandler(Grid_InitializeLayoutIncrease) 
                : new InitializeLayoutEventHandler(Grid_InitializeLayoutDecrease);

            GrowRateRule growRateRule = new GrowRateRule("Темп прироста, %");
            growRateRule.IncreaseImg = "~/images/ArrowRedUpIPad.png";
            growRateRule.DecreaseImg = "~/images/ArrowGreenDownIPad.png";
            growRateRule.Limit = 0;
            growRateRule.LeftPadding = 20;
            brick.AddIndicatorRule(growRateRule);
        }

        private void GridDataBind(UltraGridBrick grid, string queryPostfix, Label label)
        {
            bool isIncrease = queryPostfix == "Increase";

            string deviationText = isIncrease ? "Рост" : "Снижение";
            string deviationGenText = isIncrease ? "Роста" : "Снижения";
            CustomParam.CustomParamFactory("CompareOperator").Value = isIncrease ? ">" : "<";
            CustomParam.CustomParamFactory("ServiceName").Value = ServiceName;

            string query = DataProvider.GetQueryText(ReportCode + "_grid");
            dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            string lastMonthString = String.Format("{0} {1}", CRHelper.RusMonthTvorit(lastDate.Month), lastDate.Year);

            if (dt.Rows.Count > 0)
            {
                grid.Visible = true;
                if (dt.Columns.Count > 0)
                {
                    dt.Columns.RemoveAt(0);
                }

                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        string[] splitParts = row[0].ToString().Replace("\"", "'").Split(';');
                        row[0] = String.Format("<span class='DigitsValue'>{0}</span><br/>{1}<br/><i>{2}&nbsp;{3}</i>", 
                            splitParts[0].Replace("муниципальный район", "МР"), splitParts[1], splitParts[2], splitParts[3]);
                    }
                }

                grid.DataTable = dt;

                label.Text = String.Format("<span class='DigitsValue'>{3} тарифа</span>&nbsp;на&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;за&nbsp;<span class='DigitsValue'>{1} {2}</span>&nbsp;г. по сравнению с&nbsp;<span class='DigitsValue'>{4}</span>&nbsp;г. наблюдался в следующих муниципальных образованиях и организациях",
                    serviceName.ToLower(), CRHelper.RusMonth(reportDate.Month).ToLower(), reportDate.Year, deviationText, lastMonthString);
            }
            else
            {
                grid.Visible = false;
                label.Text = String.Format("<span class='DigitsValue'>{3} тарифа</span>&nbsp;на&nbsp;<span class='DigitsValue'>{0}</span>&nbsp;за&nbsp;<span class='DigitsValue'>{1} {2}</span>&nbsp;г. по сравнению с&nbsp;<span class='DigitsValue'>{4}</span>&nbsp;г. не наблюдалось",
                    serviceName.ToLower(), CRHelper.RusMonth(reportDate.Month).ToLower(), reportDate.Year, deviationGenText, lastMonthString);
            }
        }

        private void Grid_InitializeLayoutIncrease(object sender, LayoutEventArgs e)
        {
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            GridHeaderLayout headerLayout1 = UltraGridIncrease.GridHeaderLayout;
            headerLayout1.AddCell("");
            headerLayout1.AddCell(String.Format("{0}&nbsp;{1}&nbsp;г., руб.", CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(reportDate.Month)), reportDate.Year));
            headerLayout1.AddCell(String.Format("{0}&nbsp;{1}&nbsp;г., руб.", CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), lastDate.Year));
            headerLayout1.AddCell("Абс.откл., руб.");
            headerLayout1.AddCell("Темп прироста, %");

            headerLayout1.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].Width = 275;
            e.Layout.Bands[0].Columns[1].Width = 140;
            e.Layout.Bands[0].Columns[2].Width = 140;
            e.Layout.Bands[0].Columns[3].Width = 90;
            e.Layout.Bands[0].Columns[4].Width = 112;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[4].CellStyle.Font.Size = 12;
        }

        private void Grid_InitializeLayoutDecrease(object sender, LayoutEventArgs e)
        {
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            GridHeaderLayout headerLayout1 = UltraGridDecrease.GridHeaderLayout;
            headerLayout1.AddCell("");
            headerLayout1.AddCell(String.Format("{0}&nbsp;{1}&nbsp;г., руб.", CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(reportDate.Month)), reportDate.Year));
            headerLayout1.AddCell(String.Format("{0}&nbsp;{1}&nbsp;г., руб.", CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), lastDate.Year));
            headerLayout1.AddCell("Абс.откл., руб.");
            headerLayout1.AddCell("Темп прироста, %");

            headerLayout1.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].Width = 275;
            e.Layout.Bands[0].Columns[1].Width = 140;
            e.Layout.Bands[0].Columns[2].Width = 140;
            e.Layout.Bands[0].Columns[3].Width = 90;
            e.Layout.Bands[0].Columns[4].Width = 112;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[4].CellStyle.Font.Size = 12;
        }
    }
}