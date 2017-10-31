using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class EO_0002_0005 : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private string multiplierCaption = "тыс.руб.";

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            #region Настройка грида

            GRBSGridBrick.Height = Unit.Empty;
            GRBSGridBrick.Width = Unit.Empty;
            GRBSGridBrick.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);
            GRBSGridBrick.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "EO_0002_0005_lastDate");

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", currentDate, 4);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            BindInfoText();
            GridDataBind();
        }

        private void BindInfoText()
        {
            // string queryCountText = string.Format("select count (*) from {0} {1}", "d_Org_FNSDebtor", filter);

            lbInfo.Text = String.Empty;
            string query = DataProvider.GetQueryText("EO_0002_0005_info");
            DataTable dtInfo = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtInfo);
            CreateDictionaryWords(Convert.ToInt32(dtInfo.Rows[0][1]));
            lbInfo.Text = String.Format(@"В Сахалинской области действует&nbsp;<span class='DigitsValueLarge'>{0:N0}</span>&nbsp;{7} на общую сумму&nbsp;<span class='DigitsValueLarge'>{1:N2}</span>&nbsp;тыс.руб.<br/>
                                        Плановый объем финансирования в&nbsp;<span class='DigitsValue'>{5:yyyy}</span>&nbsp;году по уточненной бюджетной росписи составил&nbsp;<span class='DigitsValueLarge'>{6:N2}</span>&nbsp;тыс.руб.<br/>
                                        По состоянию на&nbsp;<span class='DigitsValue'>{2:dd.MM.yyyy}</span>&nbsp;года&nbsp;<span class='DigitsValue'>фактически исполнено</span>&nbsp;за&nbsp;<span class='DigitsValue'>{5:yyyy}</span>&nbsp;год по муниципальным целевым программам&nbsp;<span class='DigitsValueLarge'>{3:N2}</span>&nbsp;тыс.руб. (<span class='DigitsValueLarge'>{4:P2}</span>&nbsp;от уточненной бюджетной росписи на&nbsp;<span class='DigitsValue'>{5:yyyy}</span>&nbsp;год)",
               dtInfo.Rows[0][1],
               dtInfo.Rows[0][2],
               currentDate.AddMonths(1),
               dtInfo.Rows[0][3],
               dtInfo.Rows[0][4],
               currentDate,
               dtInfo.Rows[0][5],
               dataDictionary[Convert.ToInt32(dtInfo.Rows[0][1])]);
        }
        private Dictionary<int, string> dataDictionary = new Dictionary<int, string>();
        private void CreateDictionaryWords(int countRec)
        {
            int i = 0;
            while (i <= countRec)
            {
                dataDictionary.Add(i, "муниципальных целевых программ");
                dataDictionary.Add(i + 1, "муниципальная целевая программа");
                dataDictionary.Add(i + 2, "муниципальные целевые программы");
                dataDictionary.Add(i + 3, "муниципальные целевые программы");
                dataDictionary.Add(i + 4, "муниципальные целевые программы");
                dataDictionary.Add(i + 5, "муниципальных целевых программ");
                dataDictionary.Add(i + 6, "муниципальных целевых программ");
                dataDictionary.Add(i + 7, "муниципальных целевых программ");
                dataDictionary.Add(i + 8, "муниципальных целевых программ");
                dataDictionary.Add(i + 9, "муниципальных целевых программ");
                i = i + 10;
            }
        }

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0002_0005_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);
            if (gridDt.Columns.Count > 0)
            {
                gridDt.Columns.RemoveAt(0);
            }

            DataTable newDt = new DataTable();

            for (int i = 0; i < 5; i++)
            {
                newDt.Columns.Add(new DataColumn(i.ToString(), typeof(String)));
            }

            foreach (DataRow row in gridDt.Rows)
            {
                double value;

                DataRow newRow = newDt.NewRow();
                newRow[0] = row[0];
                newRow[1] = String.Format("План на {0:yyyy} г.", currentDate);
                newRow[2] = String.Format("{0:N3}", row[1]);
                newDt.Rows.Add(newRow);

                newRow = newDt.NewRow();
                newRow[0] = row[0];
                newRow[1] = String.Format("Факт на {0:dd.MM.yyyy} г.", currentDate.AddMonths(1));
                newRow[2] = String.Format("{0:N3}", row[2]);
                double.TryParse(row[3].ToString(), out value);
                newRow[3] = String.Format("{0}", GetGaugeUrl(value, ""));
                newRow[4] = String.Format("{0:P2}", value);
                newDt.Rows.Add(newRow);
            }

            if (newDt.Rows.Count > 0)
            {
                GRBSGridBrick.DataSource = newDt;
                GRBSGridBrick.DataBind();
            }
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index % 2 == 0)
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
            }
        }

        protected string GetGaugeUrl(object oValue, string alt)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue) * 100;
            if (value > 100)
                value = 100;
            string path = "EO_0002_0005_gauge_" + value.ToString("N0") + ".png";
            string returnPath = String.Format("<img alt=\"{1}\" style=\" float: right; margin-left: 10px\" src=\"../../../TemporaryImages/{0}\"/>", path, alt);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
            {
                return returnPath;
            }
            LinearGaugeScale scale = ((LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (value > 70)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else if (value < 30)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
            }
            else
            {
                BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
            }

            Size size = new Size(100, 40);
            Gauge.SaveTo(Server.MapPath(serverPath), GaugeImageType.Png, size);
            return returnPath;
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            int columnCount = e.Layout.Bands[0].Columns.Count;

            if (columnCount == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.Bands[0].Columns[1].MergeCells = true;

            for (int i = 0; i < columnCount; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 10;
            }

            for (int i = 2; i < columnCount; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.VerticalAlign = VerticalAlign.Middle;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            //e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[1].Width = 150;
            e.Layout.Bands[0].Columns[2].Width = 117;
            e.Layout.Bands[0].Columns[3].Width = 120;
            e.Layout.Bands[0].Columns[4].Width = 120;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 14;

            GridHeaderLayout headerLayout = new GridHeaderLayout(GRBSGridBrick);

            headerLayout.AddCell("Наименование программы");
            headerLayout.AddCell(String.Format("", currentDate));
            headerLayout.AddCell(String.Format("", currentDate));
            headerLayout.AddCell(String.Format("", currentDate));
            headerLayout.AddCell(String.Format("", currentDate));
            headerLayout.AddCell(String.Format("", currentDate));

            headerLayout.ApplyHeaderInfo();
        }

        //private static IDatabase GetDataBase()
        //{
        //    try
        //    {
        //        HttpSessionState sessionState = HttpContext.Current.Session;
        //        LogicalCallContextData cnt =
        //            sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
        //        if (cnt != null)
        //            LogicalCallContextData.SetContext(cnt);
        //        IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];
        //        return scheme.SchemeDWH.DB;
        //    }
        //    finally
        //    {
        //        CallContext.SetData("Authorization", null);
        //    }
        //}
    }
}
