using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class EO_0001_0001 : CustomReportPage
    {
        private DateTime currentDate;
        private DataTable commonDt;

        private CustomParam currentPeriod;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            currentPeriod = UserParams.CustomParam("current_period");

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "EO_0001_0001_lastDate");
            currentPeriod.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", currentDate, 4);

            CrossLink.Text = "<a href='webcommand?showReport=EO_0001_0003'>Объекты с низким освоением финансирования</a>";

            CommonGridCaption.Text = String.Format("на {0:dd.MM.yyyy}", currentDate.AddMonths(1));
            CommonGridBrick.BrowserSizeAdapting = false;
            CommonGridBrick.Height = Unit.Empty;
            CommonGridBrick.Width = Unit.Empty;
            CommonGridBrick.RedNegativeColoring = false;
            CommonGridBrick.HeaderVisible = false;
            CommonGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(CommonGrid_InitializeLayout);
            CommonGridBrick.Grid.InitializeRow += new InitializeRowEventHandler(CommonGrid_InitializeRow);

            CommonGridDataBind();
        }

        #region Обработчики главного грида

        private void CommonGridDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0001_0001_commonGrid");
            commonDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", commonDt);

            if (commonDt.Rows.Count > 0)
            {
                if (commonDt.Columns.Count > 0)
                {
                    commonDt.Columns.RemoveAt(0);
                }

                CommonGridBrick.DataTable = commonDt;
            }
        }

        private void CommonGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.White;
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 13;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 11;

            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Width = 220;
            e.Layout.Bands[0].Columns[1].Width = 220;
            e.Layout.Bands[0].Columns[2].Width = 140;
            e.Layout.Bands[0].Columns[3].Width = 180;

            e.Layout.Bands[0].Columns[4].Hidden = true;
        }

        protected void CommonGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count - 1;
            int rowType = -1;
            if (e.Row.Cells[cellCount].Value != null && e.Row.Cells[cellCount].Value.ToString() != String.Empty)
            {
                rowType = Convert.ToInt32(e.Row.Cells[cellCount].Value);
            }

            if (rowType == 0)
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
            }
            else if (rowType == 3)
            {
                e.Row.Style.BorderDetails.WidthBottom = 3;
            }

            string customerName = e.Row.Cells[0].Value.ToString();

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool nameColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Заказчик");
                bool valueColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Отчет");
                bool indicatorColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Колонка для индикации");

                if (valueColumn)
                {
                    switch (rowType)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Value = GetStringValue(e.Row.Cells[i].Value, "N0");
                                break;
                            }
                        case 1:
                        case 2:
                        case 3:
                            {
                                e.Row.Cells[i].Value = GetStringValue(e.Row.Cells[i].Value, "N2");
                                break;
                            }
                    }
                }
                else if (indicatorColumn)
                {
                    switch (rowType)
                    {
                        case 2:
                            {
                                double value = GetDoubleValue(e.Row.Cells[i].Value);
                                e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                                e.Row.Cells[i].Value = GetGaugeTable(value, "от стоимости строительства");
                                break;
                            }
                        case 3:
                            {
                                string[] valueParts = e.Row.Cells[i].Value.ToString().Split(';');
                                if (valueParts.Length > 1)
                                {
                                    double value1 = GetDoubleValue(valueParts[0]);
                                    double value2 = GetDoubleValue(valueParts[1]);

                                    e.Row.Cells[i].Value = String.Format("{0}<br/>{1}", GetGaugeTable(value1, "от финансирования"), GetGaugeTable(value2, "от стоимости строительства"));
                                }
                                break;
                            }
                    }
                }
                else if (nameColumn)
                {
                    if (customerName != "Всего по АИП")
                    {
                        string shortCustomerName = customerName.Replace("(заказчик Адресной инвестиционной программы)", String.Empty);
                        shortCustomerName = shortCustomerName.Replace("(государственный заказчик объектов Адресной инвестиционной программы)", String.Empty);

                        e.Row.Cells[i].Value = String.Format("<a href='webcommand?showPinchReport=EO_0001_0002_BUILDERCUSTOMER={0}'>{1}</a>",
                            CustomParams.GetBuilderCustomerIdByName(customerName), shortCustomerName.Replace("\"", "&quot;").TrimStart());
                    }
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
            }

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/EO_0001_0001/") + "TouchElementBounds.xml";

            if (e.Row.Index % 4 == 0)
            {
                if (e.Row.Index == 0)
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/EO_0001_0001/"));
                    File.WriteAllText(filename, "<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements>");
                }

                int rowHeight = 280;
                int rowIndex = e.Row.Index / 4;
                File.AppendAllText(filename, String.Format(
                    "<element id=\"EO_0001_0002_BUILDERCUSTOMER={2}\" bounds=\"x=0;y={0};width=768;height={1}\" openMode=\"\"/>",
                    rowIndex * rowHeight + 46, rowHeight, CustomParams.GetBuilderCustomerIdByName(customerName)));

                if (e.Row.Index == commonDt.Rows.Count - 4)
                {
                    File.AppendAllText(filename, "</touchElements>");
                }
            }
        }

        private string GetGaugeTable(double value, string caption)
        {
            if (value != Double.MinValue)
            {
                return String.Format("<span style=\"font-size: 12px;color: white;\"><b>{0}</b><br/>{1:P2}&nbsp;{2}</span>", GetGaugeUrl(100 * value), value, caption);
            }
            return String.Empty;
        }

        protected string GetGaugeUrl(object oValue)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue);
            if (value > 100)
                value = 100;
            string path = "Prog_0001_0001_gauge" + Convert.ToInt32(value) + ".png";
            string returnPath = String.Format("<img style=\"FLOAT: center;\" src=\"../../../TemporaryImages/{0}\"/>", path);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
                return returnPath;
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
            else
            {
                if (value < 50)
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
            }

            Size size = new Size(100, 40);
            Gauge.SaveTo(Server.MapPath(serverPath), GaugeImageType.Png, size);
            return returnPath;
        }

        private string GetStringValue(object strValue, string format)
        {
            double value = GetDoubleValue(strValue);
            if (value != Double.MinValue)
            {
                return value.ToString(format);
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
    }
}