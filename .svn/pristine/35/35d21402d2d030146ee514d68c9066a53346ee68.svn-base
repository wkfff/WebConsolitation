using System;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class TV_0001_0001_v : CustomReportPage
    {
        private DataTable dtMain = new DataTable();
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            reportDate = new DateTime(2010, 6, 30);

            string query = DataProvider.GetQueryText("TV_0001_0001_V");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Имя", dtMain);

            HtmlTable tableTasks = new HtmlTable();
            HtmlTable tableActions = new HtmlTable();
            HtmlTable tableEvents = new HtmlTable();

            for (int i = 0; i < dtMain.Rows.Count; i++ )
            {
                DataRow row = dtMain.Rows[i];
                if (row["Уровень"].ToString() == "Задача")
                {
                    Label label = new Label();
                    label.CssClass = "ServeText";

                    DateTime dateStart;
                    DateTime dateEnd;

                    string realisationTime = String.Empty;

                    if (DateTime.TryParse(row["Срок реализации начало"].ToString(), out dateStart) &&
                       DateTime.TryParse(row["Срок реализации окончание"].ToString(), out dateEnd))
                    {
                        realisationTime = String.Format("<span style='color: white;'><b>Срок реализации:</b></span>&nbsp; с {0:dd.MM.yyyy} по {1:dd.MM.yyyy}", dateStart, dateEnd);
                    }

                    string financing = "нет";
                    if (!ZeroFinancing(row))
                    {
                        financing = GetFinancing(row);
                    }

                    label.Text = String.Format("<img src=\"../../../images/television.png\">&nbsp;<span style='color: white; font-size: 18px'><b>{0}</b></span>&nbsp;<span style='color: white; font-size: 18px'><b>{1}</b></span><br/><span style='color: white;'><b>Исполнитель:&nbsp;</b></span> {2}<br/>{3}<br/><span style='color: white;'><b>% исполнения:</b></span>&nbsp;{8}&nbsp;факт&nbsp;{4:N0}% (план&nbsp;{5:N0}%)<br/><span style='color: white;'><b>Финансирование:&nbsp;</b>{7}</span>{6}",
                        row["Обозначение"], row["Имя"], row["Исполнитель"], realisationTime, row["Процент исполнения факт"], row["Процент исполнения план"],
                        financing, LessFinance(row, String.Empty),
                        Math.Round(Convert.ToDouble(row["Процент исполнения факт"])) < Math.Round(Convert.ToDouble(row["Процент исполнения план"])) ? "<span style='color: red;'><b>недостаточный</b>&nbsp;</span><img src=\"../../../images/bell.png\">&nbsp;" : "");

                    tableTasks = new HtmlTable();
                    PlaceHolder1.Controls.Add(tableTasks);

                    HtmlTableRow htmlRow = new HtmlTableRow();
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableTasks.Rows.Add(htmlRow);
                    htmlCell.Style.Add("background-image", "url(../../../images/CollapseIpad.png)");
                    htmlCell.Style.Add("background-repeat", "no-repeat;");
                    htmlCell.Attributes.Add("onclick", "resize(this)");
                    htmlCell.Style.Add("padding-left", "20px");
                    htmlCell.Style.Add("padding-bottom", "15px");
                    htmlCell.Controls.Add(label);
                }
                else if (row["Уровень"].ToString() == "Мероприятие")
                {
                    Label label = new Label();
                    label.CssClass = "ServeText";

                    DateTime dateStartPlan;
                    DateTime dateEndPlan;
                    DateTime dateStartFact;
                    DateTime dateEndFact;

                    string status = "Ожидается";
                    string indication = "<img src=\"../../../images/date.png\">&nbsp;";

                    DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan);
                    DateTime.TryParse(row["Срок реализации плановый начало"].ToString(), out dateStartPlan);

                    string realisationTime = String.Format("<span style='color: white;'><b>Срок реализации:</b></span>&nbsp;<br/>плановый с {0:dd.MM.yyy} по {1:dd.MM.yyy}", dateStartPlan, dateEndPlan);

                    // Есть факт&nbsp;завершения
                    if (DateTime.TryParse(row["Срок реализации фактический окончание"].ToString(), out dateEndFact) &&
                        dateEndFact.Year > 2008)
                    {
                        if (DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan))
                        {
                            // и план&nbsp;и факт&nbsp;раньше текущей
                            if (reportDate > dateEndPlan &&
                                reportDate > dateEndFact)
                            {
                                indication = "<img src=\"../../../images/accept.png\">&nbsp;";
                                status = "Состоялось в срок";

                                DateTime.TryParse(row["Срок реализации фактический начало"].ToString(), out dateStartFact);
                                realisationTime = String.Format("<span style='color: white;'><b>Срок реализации:</b></span><br/>с {0:dd.MM.yyy} по {1:dd.MM.yyy}", dateStartFact, dateEndFact);
                            }
                        }
                    }

                    // Есть факт&nbsp;завершения
                    if (DateTime.TryParse(row["Срок реализации фактический окончание"].ToString(), out dateEndFact) &&
                        dateEndFact.Year > 2008)
                    {
                        if (DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan))
                        {
                            // и факт&nbsp;раньше текущей позже плана
                            if (reportDate > dateEndFact &&
                                dateEndFact > dateEndPlan)
                            {
                                indication = "<img src=\"../../../images/accept.png\">&nbsp;";
                                status = "Состоялось позднее запланированного срока";

                                DateTime.TryParse(row["Срок реализации плановый начало"].ToString(), out dateStartPlan);
                                DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan);
                                DateTime.TryParse(row["Срок реализации фактический начало"].ToString(), out dateStartFact);

                                realisationTime = String.Format("<span style='color: white;'><b>Срок реализации:</b></span>&nbsp;<span style='color: red;'>нарушен&nbsp;</span><img src=\"../../../images/clock_red.png\"><br/>фактический c {0:dd.MM.yyyy} по {1:dd.MM.yyyy}<br/>запланированный c {2:dd.MM.yyyy} по {3:dd.MM.yyyy}", dateStartFact, dateEndFact, dateStartPlan, dateEndPlan);

                            }
                        }
                    }

                    // нет факта завершения
                    if (!DateTime.TryParse(row["Срок реализации фактический окончание"].ToString(), out dateEndFact) ||
                        (DateTime.TryParse(row["Срок реализации фактический окончание"].ToString(), out dateEndFact) && dateEndFact.Year < 2009))
                    {
                        if (DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan))
                        {
                            // и план&nbsp;позже текущей
                            if (reportDate > dateEndPlan)
                            {
                                indication = "<img src=\"../../../images/cancel.png\">&nbsp;";
                                status = "Не состоялось";

                                DateTime.TryParse(row["Срок реализации фактический начало"].ToString(), out dateStartPlan);
                                realisationTime = String.Format("<span style='color: white;'><b>Срок реализации:</b></span><br/>запланированный с {0:dd.MM.yyy} по {1:dd.MM.yyy}", dateStartPlan, dateEndPlan);
                            }
                        }

                        if (DateTime.TryParse(row["Срок реализации фактический начало"].ToString(), out dateStartFact))
                        {
                            // и план&nbsp;позже текущей
                            if (reportDate > dateStartFact)
                            {
                                indication = "<img src=\"../../../images/inProc.png\">&nbsp;";
                                status = "Мероприятие реализуется";
                                DateTime.TryParse(row["Срок реализации фактический начало"].ToString(), out dateStartPlan);
                                realisationTime = String.Format("<span style='color: white;'><b>Срок реализации:</b></span><br/>запланированный с {0:dd.MM.yyy} по {1:dd.MM.yyy}<br/>фактический с {2:dd.MM.yyy}", dateStartPlan, dateEndPlan, dateStartFact);
                            }
                        }
                    }

                    string financing = "нет";
                    if (!ZeroFinancing(row))
                    {
                        financing = GetFinancing(row);
                    }

                    label.Text = String.Format("{7}<span style='color: white; font-size: 18px'><b>{0}</b></span>&nbsp;<span style='color: white; font-size: 16px'><b>{1}</b></span><br/><span style='color: white'><b>Исполнитель:</b></span>&nbsp;{2} <span style='color: white'><b><br/>Статус:</b></span>&nbsp;{8}<br/>{3}<br/><span style='color: white'><b>% исполнения:</b></span>&nbsp;{9}факт&nbsp;{4:N0}% (план&nbsp;{5:N0}%)<br/><span style='color: white'><b>Финансирование:&nbsp;</b></span>{10}{6}",
                        row["Обозначение"], row["Имя"], row["Исполнитель"], realisationTime, row["Процент исполнения факт"], row["Процент исполнения план"],
                        financing, indication, status.ToLower(), Convert.ToDouble(row["Процент исполнения факт"]) < Convert.ToDouble(row["Процент исполнения план"]) ? "<span style='color: red;'><b>недостаточный</b>&nbsp;</span><img src=\"../../../images/bell.png\">&nbsp;" : "", LessFinance(row, status));

                    tableActions = new HtmlTable();
                    HtmlTableRow htmlRow = new HtmlTableRow();
                    htmlRow.Attributes.Add("class", "ReportRowFirstState");
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableTasks.Rows.Add(htmlRow);
                    htmlCell.Controls.Add(tableActions);

                    htmlRow = new HtmlTableRow();
                    htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableActions.Rows.Add(htmlRow);
                    if (dtMain.Rows[i + 1]["Уровень"].ToString() == "Ключевое событие")
                    {
                        htmlCell.Style.Add("background-image", "url(../../../images/CollapseIpad.png)");
                        htmlCell.Style.Add("background-repeat", "no-repeat;");
                        htmlCell.Attributes.Add("onclick", "resize(this)");
                    }
                    htmlCell.Style.Add("padding-left", "40px");
                    htmlCell.Style.Add("padding-bottom", "15px");
                    htmlCell.Style.Add("background-position", "20px top");
                    htmlCell.Controls.Add(label);
                }
                #region событие
                else if (row["Уровень"].ToString() == "Ключевое событие")
                {
                    Label label = new Label();
                    label.CssClass = "ServeText";

                    DateTime datePlan;
                    DateTime dateFact;
                    DateTime dateKeyEvent = new DateTime(1, 1, 1);
                    string statusKeyEvent = "Ожидается";
                    string indication = "<img src=\"../../../images/date.png\">&nbsp;";

                    // Есть факт&nbsp;завершения
                    if (DateTime.TryParse(row["Дата ключевого события факт "].ToString(), out dateFact))
                    {
                        if (DateTime.TryParse(row["Дата ключевого события план "].ToString(), out datePlan))
                        {
                            // и план&nbsp;и факт&nbsp;раньше текущей
                            if (reportDate > datePlan &&
                                reportDate > dateFact)
                            {
                                indication = "<img src=\"../../../images/accept.png\">&nbsp;";
                                statusKeyEvent = "Состоялось в срок";
                            }
                        }
                    }

                    // Есть факт&nbsp;завершения
                    if (DateTime.TryParse(row["Дата ключевого события факт "].ToString(), out dateFact))
                    {
                        if (DateTime.TryParse(row["Дата ключевого события план "].ToString(), out datePlan))
                        {
                            // и факт&nbsp;раньше текущей позже плана
                            if (reportDate > dateFact &&
                                datePlan < dateFact)
                            {
                                indication = "<img src=\"../../../images/accept.png\">&nbsp;";
                                statusKeyEvent = "Состоялось позднее запланированного срока";
                            }
                        }
                    }

                    // нет факта завершения
                    if (!DateTime.TryParse(row["Дата ключевого события факт "].ToString(), out dateFact))
                    {
                        if (DateTime.TryParse(row["Дата ключевого события план "].ToString(), out datePlan))
                        {
                            // и план&nbsp;позже текущей
                            if (reportDate >= datePlan)
                            {
                                indication = "<img src=\"../../../images/cancel.png\">&nbsp;";
                                statusKeyEvent = "Не состоялось";
                            }
                        }
                    }

                    if (DateTime.TryParse(row["Дата ключевого события план "].ToString(), out datePlan))
                    {
                        dateKeyEvent = datePlan;
                    }
                    if (DateTime.TryParse(row["Дата ключевого события факт "].ToString(), out dateFact))
                    {
                        dateKeyEvent = dateFact;
                    }

                    label.Text = String.Format("{3}<span style='color: white'><b>{4:dd.MM.yyyy}<br/>{0}&nbsp;{1}</span><br/><span style='color: white'><b>Исполнитель:</b></span>&nbsp;{2}&nbsp;<br/><span style='color: white'><b>Статус:</b></span>&nbsp;{5}<br/>{6}",
                            row["Обозначение"], row["Имя"], row["Исполнитель"], indication, dateKeyEvent, statusKeyEvent.ToLower(),
                            statusKeyEvent == "Состоялось позднее запланированного срока" ? String.Format("<span style='color: white;'><b>Срок реализации:</b></span>&nbsp;<span style='color: red;'>нарушен&nbsp;</span><img src=\"../../../images/clock_red.png\"><br/>запланированный {0:dd.MM.yyyy}<br/>фактический {1:dd.MM.yyyy}", datePlan, dateFact) : "");

                    tableEvents = new HtmlTable();
                    HtmlTableRow htmlRow = new HtmlTableRow();
                    htmlRow.Attributes.Add("class", "ReportRowFirstState");
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableActions.Rows.Add(htmlRow);
                    htmlCell.Controls.Add(tableEvents);

                    htmlRow = new HtmlTableRow();
                    htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableEvents.Rows.Add(htmlRow);
                    htmlCell.Style.Add("padding-left", "60px");
                    htmlCell.Style.Add("background-position", "40px top");
                    htmlCell.Controls.Add(label);
                }
                #endregion
            }
        }
        
        private static string GetFinancing(DataRow row)
        {
            return
                String.Format(
                    "<br/>всего: факт&nbsp;<span style='color: white;'><b>{0:N2}</b>&nbsp;</span> млн.руб. (план&nbsp;<span style='color: white;'><b>{1:N2}</b>&nbsp;</span> млн.руб.)<br/>за январь-июнь 2010 года: факт&nbsp;<span style='color: white;'><b>{2:N2}</b>&nbsp;</span> млн.руб. (план&nbsp;<span style='color: white;'><b>{3:N2}</b>&nbsp;</span> млн.руб.)<br/>за июнь 2010 года: факт&nbsp;<span style='color: white;'><b>{4:N2}</b>&nbsp;</span> млн.руб. (план&nbsp;<span style='color: white;'><b>{5:N2}</b>&nbsp;</span> млн.руб.)",
                    row["Финансирование факт всего"], row["Финансирование план всего"],
                    row["Финансирование факт текущий год"], row["Финансирование план текущий год"],
                    row["Финансирование факт текущий месяц"], row["Финансирование план текущий месяц"]);
                                      
        }

        private static string LessFinance(DataRow row, string status)
        {
            bool less = (status != "Ожидается" &&
                         Convert.ToDouble(row["Финансирование факт текущий год"].ToString()) <
                         Convert.ToDouble(row["Финансирование план текущий год"].ToString()));
            return less ? String.Format("<span style='color: red;'><b>недостаточное</b>&nbsp;</span><img src=\"../../../images/money.png\">") : String.Empty;
        }

        private static bool ZeroFinancing(DataRow row)
        {
            return (Convert.ToDouble(row["Финансирование факт всего"].ToString()) == 0 &&
                    Convert.ToDouble(row["Финансирование план всего"].ToString()) == 0 &&
                    Convert.ToDouble(row["Финансирование факт текущий год"].ToString()) == 0 &&
                    Convert.ToDouble(row["Финансирование план текущий год"].ToString()) == 0 &&
                    Convert.ToDouble(row["Финансирование факт текущий месяц"].ToString()) == 0 &&
                    Convert.ToDouble(row["Финансирование план текущий месяц"].ToString()) == 0);
        }
    }
}