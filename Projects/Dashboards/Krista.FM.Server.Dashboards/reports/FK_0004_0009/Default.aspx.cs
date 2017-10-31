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
using Infragistics.Web.UI.GridControls;
using System.Web;

namespace Krista.FM.Server.Dashboards.reports.FK_0004_0009
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private string multiplierCaption = "млн.руб.";

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;

        #endregion


        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion
            rubMultiplier = UserParams.CustomParam("rub_multiplier");

            #region Настройка грида

            WebDataGrid1.Height = Convert.ToInt32(CustomReportConst.minScreenHeight - 150);
            WebDataGrid1.Width = CRHelper.GetGridWidth(Convert.ToInt32(CustomReportConst.minScreenWidth - 25));
            WebDataGrid1.InitializeRow += new Infragistics.Web.UI.GridControls.InitializeRowEventHandler(WebDataGrid1_InitializeRow);


            #endregion

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.Filter.Value = "SELF";


            if (!Page.IsPostBack)
            {
                CustomCalendar1.WebCalendar.SelectedDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0009_lastDate");
            }

            currentDate = CustomCalendar1.WebCalendar.SelectedDate;

            Page.Title = String.Format("Анализ показателей исполнения федерального бюджета по государственным программам и непрограммам");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на <b>{0:dd.MM.yyyy} г., {1}</b>", currentDate, multiplierCaption);

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 5);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            BindInfoText();
            GridDataBind();
        }

        private void BindInfoText()
        {
            string query = DataProvider.GetQueryText("Prog_0002_0001_info");
            DataTable dtInfo = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtInfo);

            if (dtInfo.Rows.Count < 0)
            {
                return;
            }

            lbInfo.Text = String.Format(@"<div> По состоянию на <b> {8:dd.MM.yyyy} </b> года: <br/> 
•	<b>доведено ЛБО </b> на {0:yyyy} год по государственным программам и непрограммам <b>{1:N2}</b> млн. руб. (<b>{2:N2}%</b> от уточненной бюджетной росписи);<br/>
•	<b>распределено ЛБО </b> на {0:yyyy} год по государственным программам и непрограммам <b>{3:N2}</b> (<b>{4:N2}%</b> от уточненной бюджетной росписи);<br/>
•	<b>фактически исполнено</b> по государственным программам и непрограммам <b>{5:N2}</b> млн.руб. (<b>{6:N2}%</b> от уточненной бюджетной росписи).</div>",
                currentDate,
                dtInfo.Rows[0][0],
                dtInfo.Rows[0][1],
                dtInfo.Rows[0][2],
                dtInfo.Rows[0][3],
                dtInfo.Rows[0][4],
                dtInfo.Rows[0][5],
                dtInfo.Rows[0][6],
                currentDate);

            query = DataProvider.GetQueryText("Prog_0002_0001_top");
            dtInfo = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtInfo);

            lbInfo.Text = String.Format(@"{0}<div style='width:49%; min-width: 800px; float: left; margin-right: 20px'>
<img src='../../images/StarGrayBB.png'>&nbsp;<b>Минимальный</b> процент исполнения:", lbInfo.Text);

            for (int i = 0; i < 5; i++)
            {
                if (dtInfo.Rows.Count > i)
                {
                    lbInfo.Text = String.Format(@"{0}<br/> <b>{1:N2}%</b> {2}", lbInfo.Text, dtInfo.Rows[i][2], dtInfo.Rows[i][1].ToString());
                }
            }

            lbInfo.Text = String.Format(@"{0}</div><div style='width:49%; min-width: 800px; float: left'>
<img src='../../images/StarYellowBB.png'>&nbsp;<b>Максимальный</b> процент исполнения:", lbInfo.Text);

            for (int i = 5; i < 10; i++)
            {
                if (dtInfo.Rows.Count > i)
                {
                    lbInfo.Text = String.Format(@"{0}<br/> <b>{1:N2}%</b> {2}", lbInfo.Text, dtInfo.Rows[i][2], dtInfo.Rows[i][1].ToString());
                }
            }
            lbInfo.Text = String.Format(@"{0}</div>", lbInfo.Text);
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("Prog_0002_0001_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Columns.Count > 0)
            {
                gridDt.Columns.RemoveAt(0);
            }

            DataTable newDt = new DataTable();

            for (int i = 0; i < 7; i++)
            {
                if (i != 2)
                {
                    newDt.Columns.Add(new DataColumn(i.ToString(), typeof(String)));
                }
                else
                {
                    newDt.Columns.Add(new DataColumn(i.ToString(), typeof(Double)));
                }
            }

            foreach (DataRow row in gridDt.Rows)
            {
                double value;

                DataRow newRow = newDt.NewRow();
                if  (row[0].ToString() == "0")
                {
                   row[0] = DBNull.Value;
                }
                newRow[0] = row[0];
                newRow[1] = row[1];

                newRow[2] = row[2];
                double.TryParse(row[4].ToString(), out value);
                newRow[3] = String.Format("<span style='font-size: 10pt'>{0:N0}</span><br/>{2}<br/>{1:N2}%", row[3], row[4], GetGaugeUrl(value, "Процент доведенных ЛБО от уточненной росписи, %"));
                double.TryParse(row[6].ToString(), out value);
                newRow[4] = String.Format("<span style='font-size: 10pt'>{0:N0}</span><br/>{2}<br/>{1:N2}%", row[5], row[6], GetGaugeUrl(value, "Процент распределенных ЛБО от доведенных ЛБО, %"));
                double.TryParse(row[8].ToString(), out value);
                newRow[5] = String.Format("<span style='font-size: 10pt'>{0:N0}</span><br/>{2}<br/>{1:N2}%", row[7], row[8], GetGaugeUrl(value, "Процент кассового исполнения от уточненной БР, %"));

                newRow[6] = row[9];
                newDt.Rows.Add(newRow);
            }

            WebDataGrid1.Behaviors.CreateBehavior<Sorting>();
            if (newDt.Rows.Count > 0)
            {
                
                WebDataGrid1.DataSource = newDt;
                if (!Page.IsPostBack)
                {
                    UltraWebGrid_InitializeLayout(WebDataGrid1, newDt);
                }
            }
           
        }

        void WebDataGrid1_InitializeRow(object sender, Infragistics.Web.UI.GridControls.RowEventArgs e)
        {
            if (e.Row.Items[e.Row.Items.Count - 1].Value != DBNull.Value &&
                e.Row.Items[e.Row.Items.Count - 1].Value.ToString() == "1")
            {
               e.Row.CssClass = "levelOne";
               e.Row.Items[2].CssClass = "Font10pt";
               
            }

            if (e.Row.Items[e.Row.Items.Count - 1].Value != DBNull.Value &&
            e.Row.Items[e.Row.Items.Count - 1].Value.ToString() == "2")
            {
                e.Row.CssClass  = "levelOne";
                e.Row.Items[1].CssClass = "paddind20";
                e.Row.Items[2].CssClass = "Font10pt";
            }

            if (e.Row.Items[e.Row.Items.Count - 1].Value != DBNull.Value &&
            e.Row.Items[e.Row.Items.Count - 1].Value.ToString() == "3")
            {
                e.Row.Items[1].CssClass = "paddind40";
                e.Row.Items[2].CssClass = "Font10pt";
            }
            
        }

        protected string GetGaugeUrl(object oValue, string alt)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue);
            if (value > 100)
                value = 100;
            string path = "Prog_0002_0001_gauge_" + value.ToString("N0") + ".png";
            string returnPath = String.Format("<img alt=\"{1}\" style=\" float: right; margin-left: 10px\" src=\"../../TemporaryImages/{0}\"/>", path, alt);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
            {
                return returnPath;
            }
            LinearGaugeScale scale = ((LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (value > 90)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else if (value < 50)
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

        public static void SetHeaderCaption(WebDataGrid grid, int columnIndex, string caption)
        {
            if (grid.Columns[columnIndex] != null)
            {
                grid.Columns[columnIndex].Header.Text = caption;
            }
        }

        protected void UltraWebGrid_InitializeLayout(WebDataGrid e, DataTable dtGrid)
        {
            e.Columns.Clear();
            for (int i = 0; i < dtGrid.Columns.Count; i++)
            {
                BoundDataField col = new BoundDataField();
                col.Key = i.ToString();
                col.DataFieldName = dtGrid.Columns[i].ColumnName;

                col.HtmlEncode = false;

                double width;
                switch (i)
                {
                    case 0:
                        {
                            width = 80;
                            col.DataFormatString = "{0:N0}";
                            break;
                        }
                    case 1:
                        {
                            width = 180;
                            break;
                        }
                    case 2:
                        {
                            width = 80;
                            col.DataFormatString = "{0:N0}";
                            break;
                        }
                    default:
                        {
                            width = 160;
                            break;
                        }
                }

                e.Columns.Add(col);
                e.Columns[i].Width = GetColumnWidth(width);

                if (i != 1)
                {
                    e.Columns[i].CssClass = "ValueCell";
                }
            }
            e.Columns[6].Hidden = true;

            SetHeaderCaption(e, 0, "Номер госпрограммы (непрограмы)");
            SetHeaderCaption(e, 1, "Наименование");
            SetHeaderCaption(e, 2, String.Format("Уточненная роспись на {0:yyyy}г., млн.руб", currentDate));
            SetHeaderCaption(e, 3, String.Format("Доведенные ЛБО на {0:yyyy}г., млн.руб", currentDate));
            SetHeaderCaption(e, 4, String.Format("Распределенные ЛБО на {0:yyyy}г., млн.руб", currentDate));
            SetHeaderCaption(e, 5, String.Format("Кассовое исполнение на {0:dd.MM.yyyy}г., млн.руб", currentDate));
        }

        public static int GetColumnWidth(double defaultWidth)
        {
            // Если ширины достаточная
            if ((int)HttpContext.Current.Session["width_size"] > CustomReportConst.minScreenWidth)
            {
                // Увеличиваем колонки на размер превышения
                defaultWidth =
                    defaultWidth * (int)HttpContext.Current.Session["width_size"] / CustomReportConst.minScreenWidth;
            }
            string browser = HttpContext.Current.Request.Browser.Browser;
            switch (browser)
            {
                case ("Firefox"):
                    {
                        return (int)(defaultWidth / 0.989);
                    }
                case ("AppleMAC-Safari"):
                    {
                        return (int)(defaultWidth * 1.15);
                    }
            }
            return (int)defaultWidth;
        }

        #endregion

       
    }
}