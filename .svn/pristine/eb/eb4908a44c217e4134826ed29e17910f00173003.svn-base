using System;
using System.Data;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0018 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtData = new DataTable();
        private DataTable dtPerson = new DataTable();

        private double othersEthalon;
        private double salaryEthalon;
        private double commonEthalon;

        private string name = String.Empty;

        private DateTime date;

        #region Параметры запроса

        // мера План
        private CustomParam measurePlan;
        // мера Факт
        private CustomParam measureFact;
        // мера Остаток на начало
        private CustomParam measureStartBalance;
        // мера Остаток на конец
        private CustomParam measureEndBalance;
               

        #endregion

        public bool IsQuaterPlanType
        {
            get
            {
                return RegionSettingsHelper.Instance.CashPlanType.ToLower() == "quarter";
            }
        }

        public bool IsYar
        {
            get
            {
                return RegionSettingsHelper.Instance.Name.ToLower() == "ярославская область";
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            #region Инициализация параметров запроса

            measurePlan = UserParams.CustomParam("measure_plan");
            measureFact = UserParams.CustomParam("measure_fact");
            measureStartBalance = UserParams.CustomParam("measure_start_balance");
            measureEndBalance = UserParams.CustomParam("measure_end_balance");

            #endregion

            // мера Остаток на конец
            UserParams.Filter.Value = "Департамент здравоохранения и фармации ЯО";

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][5].ToString();

            if (!dtDate.Rows[0][4].ToString().Contains("Заключительные обороты"))
            {
                date = new DateTime(
                   Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                   CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                   Convert.ToInt32(dtDate.Rows[0][4].ToString()));
            }
            else
            {
                date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            }

            if (IsQuaterPlanType)
            {
                measurePlan.Value = "План";
                measureFact.Value = "Факт";
            }
            else
            {
                measurePlan.Value = "План_Нарастающий итог";
                measureFact.Value = "Факт_Нарастающий итог";
            }

            measureStartBalance.Value = (IsYar) ? "Остаток средств на начало квартала" : RegionSettingsHelper.Instance.CashPlanBalance;
            measureEndBalance.Value = (measureStartBalance.Value == "Остаток средств")
                                          ? measureStartBalance.Value
                                          : "Остаток средств на конец квартала";

            query = DataProvider.GetQueryText("data");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtData);


            query = DataProvider.GetQueryText("person");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtPerson);

            name = dtPerson.Rows[0][0].ToString();

            lbName.Text = dtPerson.Rows[0][0].ToString();
            HyperLinkSite.NavigateUrl = dtPerson.Rows[0][2].ToString();
            HyperLinkSite.Text = dtPerson.Rows[0][2].ToString();
            lbFIO.Text = dtPerson.Rows[0][3].ToString();
            lbPhone.Text = dtPerson.Rows[0][4].ToString();
            HyperLinkMail.NavigateUrl = "mailto:" + dtPerson.Rows[0][5];
            HyperLinkMail.Text = dtPerson.Rows[0][5].ToString();

            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath("~/ImagesRenameList.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            string imageName = dtPerson.Rows[0][1].ToString();
            foreach (XmlNode rootNode in doc.ChildNodes)
            {
                if (rootNode.Name == "Images")
                {
                    foreach (XmlNode imageNode in rootNode.ChildNodes)
                    {
                        if (imageNode.Attributes["name"].Value == imageName)
                        {
                            imageName = imageNode.Attributes["id"].Value;
                        }
                    }
                }
            }
            Image1.ImageUrl = String.Format("../../../images/{0}", imageName);

            othersEthalon = CRHelper.QuarterDaysCountToDate(date) / CRHelper.QuarterDaysCount(date);
            commonEthalon = CommonAssessionLimit();
            salaryEthalon = SalaryAssessionLimit();
            
            MakeHtmlTableDetailTable();
        }

        private void MakeHtmlTableDetailTable()
        {
            detailTable.CssClass = "HtmlTableCompact";
            
            AddHeaderRow(detailTable);

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell;
                
                if (i == 0)
                {
                    cell = GetNameCell("Итого по ГРБС", false);
                }
                else
                {
                    cell = GetNameCell(dtData.Rows[i][0].ToString(), true);
                }

                row.Cells.Add(cell);
                
                cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][1]));
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][2]));
                row.Cells.Add(cell);

                double ethalon;
                switch (i)
                {
                    case (0):
                        {
                            ethalon = 0;
                            break;
                        }
                    case (1):
                        {
                            ethalon = salaryEthalon;
                            break;
                        }
                    case (5):
                        {
                            ethalon = othersEthalon;
                            break;
                        }
                    default:
                        {
                            ethalon = commonEthalon;
                            break;
                        }
                }

                cell = GetPercentCell(dtData.Rows[i][3], ethalon);
                row.Cells.Add(cell);

                detailTable.Rows.Add(row);
            }
        }

        private TableCell GetValueCell(string value)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = value;
            lb.CssClass = "TableFont";
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Top;
            cell.Style.Add("padding-top", "5px");
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.Style.Add("padding-right", "3px");
            return cell;
        }

        private TableCell GetPercentCell(object value, double ethalon)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = string.Format("{0:P2}<br/>", value);
            lb.CssClass = "TableFont";
            cell.Controls.Add(lb);

            double convertedValue;
            Image image = new Image();
            string hintRowText = String.Empty;
            HtmlGenericControl control = new HtmlGenericControl("div");

            control.Style.Add("float", "right");

            if (ethalon != 0 && Double.TryParse(value.ToString(), out convertedValue))
            {
                image.ImageUrl = convertedValue >= ethalon ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";
                hintRowText = convertedValue >= ethalon
                                  ? String.Format("Соблюдается равномерность ({0:P2})&nbsp;", ethalon)
                                  : String.Format("Не соблюдается равномерность ({0:P2})&nbsp;", ethalon);
                lb = new Label();
                lb.Text = hintRowText;
                lb.CssClass = "ServeText";
                control.Controls.Add(lb);
                control.Controls.Add(image);
            }

            // Хинтовая строка
            if (!String.IsNullOrEmpty(hintRowText))
            {
                cell.Controls.Add(control);
            }

            cell.VerticalAlign = VerticalAlign.Top;
            cell.Style.Add("padding-top", "5px");

            cell.Style.Add("padding-right", "3px");
            cell.Style.Add("padding-left", "15px");
            cell.HorizontalAlign = HorizontalAlign.Left;
            return cell;
        }

        private TableCell GetNameCell(string name)
        {
            return GetNameCell(name, false);
        }

        private TableCell GetNameCell(string name, bool child)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = name;

            if (child)
            {
                lb.CssClass = "TableFontGrey";
                cell.Style.Add("padding-left", "10px");
            }
            else
            {
                lb.CssClass = "TableFont";
                cell.Style.Add("padding-left", "3px");
            }
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
            return cell;
        }

        private void AddHeaderRow(Table table)
        {
            TableCell cell;
            TableRow row;

            row = new TableRow();
            cell = new TableCell();
            cell.Width = 145;
            cell.CssClass = "HtmlTableHeader";
            
            cell.Text = " ";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);
            
            cell = new TableCell();
            cell.Width = 77;
            cell.CssClass = "HtmlTableHeader";
            
            cell.Text = "План, тыс.руб.";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 77;
            cell.CssClass = "HtmlTableHeader";

            cell.Text = "Факт тыс.руб.";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 250;
            cell.CssClass = "HtmlTableHeader";
           
            cell.Text = "% исполнения";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            table.Rows.Add(row);
        }
        

        /// <summary>
        /// Пороговое значение оценки зарплаты.
        /// </summary>
        /// <returns></returns>
        private double SalaryAssessionLimit()
        {
            // Берем номер месяца в квартале
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            int day = date.Day;
            if (day < 6)
            {
                // Выплат в этом месяце не было
                return (monthNum - 1) / 3;
            }
            if (day < 21)
            {
                // Была одна выплата
                return (monthNum - 1) / 3 + 1.0 / 6;
            }
            // Все выплаты
            return (monthNum) / 3;
        }

        /// <summary>
        /// Пороговое значение оценки остальных.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            // Берем номер месяца в квартале
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            // Если последний день месяца
            if (CRHelper.MonthLastDay(date.Month) ==
                date.Day)
            {
                return (monthNum) / 3;
            }
            else
            {
                return (monthNum - 1) / 3;
            }
        }

       
    }
}
