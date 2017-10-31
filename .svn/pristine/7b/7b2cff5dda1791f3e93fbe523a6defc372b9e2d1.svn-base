using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0011 : CustomReportPage
    {
        DateTime date;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("fo_0003_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            UserParams.PeriodYear.Value = date.Year.ToString();
            UserParams.PeriodLastYear.Value = date.AddYears(-1).Year.ToString();
            CustomParam periodLastLastYear = UserParams.CustomParam("period_last_last_year");
            CustomParam periodThreeYearAgo = UserParams.CustomParam("period_3_last_year");
            CustomParam periodFourYearAgo = UserParams.CustomParam("period_4_last_year");
            periodLastLastYear.Value = date.AddYears(-2).Year.ToString();
            periodThreeYearAgo.Value = date.AddYears(-3).Year.ToString();
            periodFourYearAgo.Value = date.AddYears(-4).Year.ToString();

            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", date.AddYears(-1), 4);

            lbDescription.Text = GetIndicatorText();
        }

        private string GetIncomesHintText(string attention, DataTable dt, string controlId)
        {
            string hintText = String.Empty;

            if (attention == "Пусто в плане")
            {
                hintText = String.Format("<b>НЕТ ПЛАНОВОЙ СУММЫ</b><br/>У субъекта РФ&nbsp;<b>отсутствует</b>&nbsp;сумма уточненных годовых назначений консолидированного бюджета на&nbsp;<b>{0:yyyy}</b>&nbsp;год&nbsp;<b>({1})</b>&nbsp;по состоянию на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г.<br/><br/>Необходимо проверить корректность предоставленных данных.", date, Core.CustomParam.CustomParamFactory("control").Value);
            }
            else if (attention == "Пусто в сигме")
            {
                hintText = String.Format("<b>ПРЕДУПРЕЖДЕНИЕ</b><br/>За&nbsp;<b>{1:yyyy}-{4:yyyy}</b>&nbsp;гг у субъекта наблюдаются выбросы по темпам исполнения:<ul style='margin-bottom: -35px; margin-top: 0px'><li>{1:yyyy} год к {0:yyyy}&nbsp;<b>{5:P0}</b>&nbsp;</li><li>{2:yyyy} год к {1:yyyy}&nbsp;<b>{6:P0}</b>&nbsp;</li><li>{3:yyyy} год к {2:yyyy}&nbsp;<b>{7:P0}</b>&nbsp;</li><li>{4:yyyy} год к {3:yyyy}&nbsp;<b>{8:P0}</b>&nbsp;</li></ul><br/><br/>Необходимо уточнить возможные причины такого разброса.",
                    date.AddYears(-5), date.AddYears(-4), date.AddYears(-3), date.AddYears(-2), date.AddYears(-1),
                    dt.Rows[0]["Темп к прошлому году "], dt.Rows[1]["Темп к прошлому году "], dt.Rows[2]["Темп к прошлому году "], dt.Rows[3]["Темп к прошлому году "]);
            }
            else if (attention == "Завышение")
            {
                string executePercent = Convert.ToDouble(dt.Rows[0]["Доля в исполнении "].ToString()) < 0.01 ? "менее 1%" : String.Format("{0:P0}", dt.Rows[0]["Доля в исполнении "]);
                string planPercent = Convert.ToDouble(dt.Rows[0]["Доля в исполнении "].ToString()) < 0.01 ? "менее 1%" : String.Format("{0:P0}", dt.Rows[0]["Доля в уточненном плане "]);
                hintText = String.Format("<b>ЗАВЫШЕН ПЛАН</b><br/>Возможное&nbsp;<b>завышение</b>&nbsp;плана консолидированного бюджета субъекта РФ по доходам&nbsp;<b>({0})</b><br/><br/><div style='margin-top: -15px; margin-bottom: -33px'>Средний темп исполнения за&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;гг.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/>Темп уточненного плана на&nbsp;<b>{4:yyyy}</b>&nbsp;год по состоянию на&nbsp;<b>{11:dd.MM.yyyy}</b>&nbsp;г.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{5:P0}</b>&nbsp;<br/><b>Выше</b>&nbsp;среднего темпа исполнения за&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;гг.</div><br/><br/><b>Возможные причины:</b>&nbsp;{6}<br/><br/><b>Справочно:</b><br/>Среднеквадратическое отклонений темпа уточненного плана от среднего темпа исполнения за&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;гг.&nbsp;<b>{7:P0}</b>&nbsp;<br/>Порог завышения&nbsp;<b>{8:P0}</b>&nbsp;<br/>Исполнено за&nbsp;<b>{2:yyyy}</b>&nbsp;год &nbsp;<b>{9:N0}</b>&nbsp;тыс.руб. (доля в доходах&nbsp;<b>{12}</b>)<br/>Уточненный план&nbsp;<b>{4:yyyy}</b>&nbsp;года по состоянию на&nbsp;<b>{11:dd.MM.yyyy}</b>&nbsp;г.&nbsp;<b>{10:N0}</b>&nbsp;тыс.руб. (доля в доходах&nbsp;<b>{13}</b>)",
                    Core.CustomParam.CustomParamFactory("control").Value, date.AddYears(-4), date.AddYears(-1), dt.Rows[0]["Средний темп нормализованный"], date, dt.Rows[0]["Темп уточненного плана к прошлому году "], GetIncomesGrownReasons(controlId), dt.Rows[0]["Сигма нормализованных"], dt.Rows[0]["Верхняя граница"], dt.Rows[0]["Исполнено за прошлый год "], dt.Rows[0]["Уточненный план "], date.AddMonths(1), executePercent, planPercent);
            }
            else if (attention == "Занижение")
            {
                string executePercent = Convert.ToDouble(dt.Rows[0]["Доля в исполнении "].ToString()) < 0.01 ? "менее 1%" : String.Format("{0:P0}", dt.Rows[0]["Доля в исполнении "]);
                string planPercent = Convert.ToDouble(dt.Rows[0]["Доля в исполнении "].ToString()) < 0.01 ? "менее 1%" : String.Format("{0:P0}", dt.Rows[0]["Доля в уточненном плане "]);
                hintText = String.Format("<b>ЗАНИЖЕН ПЛАН</b><br/>Возможное&nbsp;<b>занижение</b>&nbsp;плана консолидированного бюджета субъекта РФ по доходам&nbsp;<b>({0})</b><br/><br/><div style='margin-top: -15px; margin-bottom: -33px'>Средний темп исполнения за&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;гг.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/>Темп уточненного плана на&nbsp;<b>{4:yyyy}</b>&nbsp;год по состоянию на&nbsp;<b>{11:dd.MM.yyyy}</b>&nbsp;г.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{5:P0}</b>&nbsp;<br/><b>Ниже</b>&nbsp;среднего темпа исполнения за&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;гг.</div><br/><br/><b>Возможные причины:</b>&nbsp;{6}<br/><br/><b>Справочно:</b><br/>Среднеквадратическое отклонений темпа уточненного плана от среднего темпа исполнения за&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;гг.&nbsp;<b>{7:P0}</b>&nbsp;<br/>Порог занижения&nbsp;<b>{8:P0}</b>&nbsp;<br/>Исполнено за&nbsp;<b>{2:yyyy}</b>&nbsp;год &nbsp;<b>{9:N0}</b>&nbsp;тыс.руб. (доля в доходах&nbsp;<b>{12}</b>)<br/>Уточненный план&nbsp;<b>{4:yyyy}</b>&nbsp;года по состоянию на&nbsp;<b>{11:dd.MM.yyyy}</b>&nbsp;г.&nbsp;<b>{10:N0}</b>&nbsp;тыс.руб. (доля в доходах&nbsp;<b>{13}</b>)",
                    Core.CustomParam.CustomParamFactory("control").Value, date.AddYears(-4), date.AddYears(-1), dt.Rows[0]["Средний темп нормализованный"], date, dt.Rows[0]["Темп уточненного плана к прошлому году "], GetIncomesFallenReasons(controlId), dt.Rows[0]["Сигма нормализованных"], dt.Rows[0]["Нижняя граница"], dt.Rows[0]["Исполнено за прошлый год "], dt.Rows[0]["Уточненный план "], date.AddMonths(1), executePercent, planPercent);
            }

            return hintText;
        }

        private string GetIncomesGrownReasons(string controlId)
        {
            string reason = String.Empty;

            switch (controlId)
            {
                case "1":
                    {
                        reason = "Необходимо провести анализ отдельных доходных источников для выявления возможных причин завышения плана.";
                        break;
                    }
                case "2":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>в регионе зарегистрировано новое предприятие?</li><li>в регионе рост прибыли предприятий?</li><li>изменение в налоговом законодательстве?</li><li>повышение качества администрирования доходов?</li></ul>";
                        break;
                    }
                case "3":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>индексация заработной платы?</li><li>увеличение уровня занятости населения?</li><li>переход к «белым» зарплатам?</li><li>изменение в налоговом законодательстве?</li><li>повышение качества администрирования доходов?</li></ul>";
                        break;
                    }
                case "4":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>увеличение поставок нефтепродуктов?</li><li>увеличение оборота алкогольной продукции?</li><li>увеличение предприятий нефтеперерабатывающего и алкогольного производства?</li><li>изменение в бюджетном и налоговом законодательстве?</li><li>повышение качества администрирования доходов?</li></ul>";
                        break;
                    }
                case "5":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>эффективность программ поддержки малого и среднего предпринимательства?</li><li>изменение в бюджетном и налоговом законодательстве?</li><li>повышение качества администрирования доходов?</li></ul>";
                        break;
                    }
                case "6":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>эффективность программ поддержки малого и среднего предпринимательства?</li><li>увеличение числа индивидуальных предпринимателей?</li><li>повышение качества администрирования доходов?</li><li>изменение в бюджетном и налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "7":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>эффективность программ поддержки сельского хозяйства и предприятий-сельхозтоваропроизводителей?</li><li>изменение в бюджетном и налоговом законодательстве?</li><li>повышение качества администрирования доходов?</li></ul>";
                        break;
                    }
                case "8":
                    {
                        reason = String.Empty;
                        break;
                    }
                case "9":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>завершенное строительство?</li><li>увеличение основных средств крупных предприятий?</li><li>расширение производства предприятий региона?</li><li>образование новых предприятий?</li><li>изменение в бюджетном и налоговом законодательстве?</li><li>повышение качества администрирования доходов?</li></ul>";
                        break;
                    }
                case "10":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>увеличение транспортных средств, зарегистрированных в регионе?</li><li>увеличение уровня жизни населения региона?</li><li>изменение в бюджетном и налоговом законодательстве?</li><li>повышение качества администрирования доходов?</li></ul>";
                        break;
                    }
                case "11":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>приватизация земли?</li><li>изменение в бюджетном и налоговом законодательстве?</li><li>повышение качества администрирования доходов?</li></ul>";
                        break;
                    }
                case "12":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>открыты новые месторождения полезных ископаемых?</li><li>увеличение объемов добычи полезных ископаемых?</li><li>повышение качества администрирования доходов?</li><li>изменения в бюджетном и налоговом законодательстве?</li></ul>";
                        break;
                    }
            }

            return reason;
        }

        private string GetIncomesFallenReasons(string controlId)
        {
            string reason = String.Empty;

            switch (controlId)
            {
                case "1":
                    {
                        reason = "Необходимо провести анализ отдельных доходных источников для выявления возможных причин занижения плана.";
                        break;
                    }
                case "2":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>в регионе снижение прибыли у предприятий?</li><li>убыточность организаций?</li><li>закрытие предприятий-налогоплательщиков?</li><li>увеличение издержек производства товаров, работ и услуг?</li><li>изменение в налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "3":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>сокращение фонда оплаты труда?</li><li>сокращение уровня занятости населения?</li><li>переход предприятий на неполное рабочее время?</li><li>закрытие крупных предприятий?</li><li>«серые» зарплаты?</li><li>изменения в налоговом законодательстве?</li><li>возвраты налога?</li></ul>";
                        break;
                    }
                case "4":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>сокращение поставок нефтепродуктов?</li><li>сокращение оборота алкогольной продукции?</li><li>сокращение предприятий нефтеперерабатывающего и алкогольного производства?</li><li>изменения в налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "5":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>расширение производства малых и средних предприятий?</li><li>изменения в налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "6":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>сокращение числа индивидуальных предпринимателей?</li><li>изменение в бюджетном и налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "7":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>сокращение числа предприятий-сельхозтоваропроизводителей?</li><li>изменение в бюджетном и налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "8":
                    {
                        reason = String.Empty;
                        break;
                    }
                case "9":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>сокращение производств?</li><li>закрытие крупных предприятий?</li><li>изменение в бюджетном и налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "10":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>сокращение транспортных средств, зарегистрированных в регионе?</li><li>сокращение уровня жизни населения региона?</li><li>изменение в бюджетном и налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "11":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>изменение в бюджетном и налоговом законодательстве?</li></ul>";
                        break;
                    }
                case "12":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>закрыты месторождения полезных ископаемых?</li><li>сокращение объемов добычи полезных ископаемых?</li><li>изменения в бюджетном и налоговом законодательстве?</li></ul>";
                        break;
                    }
            }

            return reason;
        }


        private string GetOutcomesHintText(string attention, DataTable dt, string controlId)
        {
            string hintText = String.Empty;

            if (attention == "Пусто в темпе")
            {
                hintText = String.Format("<b>НЕТ ИСПОЛНЕНИЯ</b><br/>У субъекта РФ&nbsp;<b>отсутствуют</b>&nbsp;расходы консолидированного бюджета&nbsp;<b>{1}</b>&nbsp;по состоянию на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г.<br/><br/>Необходимо проверить корректность предоставленных данных.", date, Core.CustomParam.CustomParamFactory("control").Value);
            }            
            else if (attention == "Завышение")
            {
                hintText = String.Format("<b>ТЕМП РОСТА ВЫШЕ СРЕДНЕГО</b><br/><b>{0}</b>. Темп исполнения консолидированного бюджета субъекта РФ на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г. к аналогичному периоду&nbsp;<b>{2:yyyy}</b>&nbsp;года&nbsp;<b>выше</b>&nbsp;среднего темпа исполнения по РФ.<br/><br/>Средний темп исполнения по РФ на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/>Темп исполнения на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{4:P0}</b><br/><b>Выше</b>&nbsp;среднего темпа исполнения по РФ.<br/><br/><b>Возможные причины:</b>&nbsp;{10}<br/><br/><b>Справочно:</b><br/>Среднеквадратическое отклонений темпа исполнения субъекта РФ от среднего темпа исполнения по РФ по состоянию на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г.&nbsp;<b>{5:P0}</b><br/>Порог завышения&nbsp;<b>{6:P0}</b><br/>Исполнено на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г.&nbsp;<b>{7:N0}</b>&nbsp;тыс.руб.<br/>Исполнено на&nbsp;<b>{9:dd.MM.yyyy}</b>&nbsp;г.&nbsp;<b>{8:N0}</b>&nbsp;тыс.руб.",
                    Core.CustomParam.CustomParamFactory("control").Value, date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["Средний темп по рф"], dt.Rows[0]["Темп к прошлому году"], dt.Rows[0]["Сигма"], dt.Rows[0]["Верхняя граница"], dt.Rows[0]["Исполнено за этот год "], dt.Rows[0]["Исполнено за прошлый год "], date.AddMonths(1).AddYears(-1), GetOutcomesGrownReasons(controlId));
            }
            else if (attention == "Занижение")
            {
                hintText = String.Format("<b>ТЕМП РОСТА НИЖЕ СРЕДНЕГО</b><br/><b>{0}</b>. Темп исполнения консолидированного бюджета субъекта РФ на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г. к аналогичному периоду&nbsp;<b>{2:yyyy}</b>&nbsp;года&nbsp;<b>ниже</b>&nbsp;среднего темпа исполнения по РФ.<br/><br/>Средний темп исполнения по РФ на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/>Темп исполнения на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{4:P0}</b><br/><b>Ниже</b>&nbsp;среднего темпа исполнения по РФ.<br/><br/><b>Возможные причины:</b>&nbsp;{10}<br/><br/><b>Справочно:</b><br/>Среднеквадратическое отклонений темпа исполнения субъекта РФ от среднего темпа исполнения по РФ по состоянию на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г.&nbsp;<b>{5:P0}</b><br/>Порог занижения&nbsp;<b>{6:P0}</b><br/>Исполнено на&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;г.&nbsp;<b>{7:N0}</b>&nbsp;тыс.руб.<br/>Исполнено на&nbsp;<b>{9:dd.MM.yyyy}</b>&nbsp;г.&nbsp;<b>{8:N0}</b>&nbsp;тыс.руб.",
                    Core.CustomParam.CustomParamFactory("control").Value, date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["Средний темп по рф"], dt.Rows[0]["Темп к прошлому году"], dt.Rows[0]["Сигма"], dt.Rows[0]["Нижняя граница"], dt.Rows[0]["Исполнено за этот год "], dt.Rows[0]["Исполнено за прошлый год "], date.AddMonths(1).AddYears(-1), GetOutcomesFallenReasons(controlId));
            }

            return hintText;
        }

        private string GetOutcomesGrownReasons(string controlId)
        {
            string reason = String.Empty;

            switch (controlId)
            {
                case "13":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>индикация заработной платы?</li><li>увеличение фонда оплаты труда?</li><li>рост минимального размера оплаты труда?</li><li>увеличение численности государственных и муниципальных служащих?</li><li>изменения в бюджетном законодательстве?</li></ul>";
                        break;
                    }
                case "14":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>увеличение числа льготников?</li><li>увеличение численности отдельных категорий граждан, получающих социальное обеспечение?</li><li>эффективность реализации национальных проектов по социальному обеспечению населения?</li><li>изменения в бюджетном законодательстве?</li></ul>";
                        break;
                    }
            }

            return reason;
        }

        private string GetOutcomesFallenReasons(string controlId)
        {
            string reason = String.Empty;

            switch (controlId)
            {
                case "13":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>оптимизация бюджетной сети?</li><li>перевод бюджетных учреждений на субсидирование (реализация 83-ФЗ)?</li><li>изменения в бюджетном законодательстве?</li></ul>";
                        break;
                    }
                case "14":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>сокращение числа льготников?</li><li>сокращение численности отдельных категорий граждан, получающих социальное обеспечение?</li><li>изменения в бюджетном законодательстве?</li></ul>";
                        break;
                    }
            }

            return reason;
        }


        private string GetIndicatorText()
        {
            DataTable dt = new DataTable();

            string controlID = HttpContext.Current.Session["CurrentСontrolID"].ToString();

            if (controlID == "13" ||
                controlID == "14")
            {
                string query = DataProvider.GetQueryText("fo_0003_0001_indicator_outcomes");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

                return GetOutcomesHintText(dt.Rows[0]["Внимание"].ToString(), dt, controlID);
            }
            else if (controlID == "15")
            {
                string query = DataProvider.GetQueryText("fo_0003_0001_241");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

                switch (dt.Rows[0]["Внимание "].ToString())
                {
                    case "0":
                        { 
                            return String.Empty;
                        }
                    case "1":
                        {
                            return String.Format("<b>ОДНОВРЕМЕННЫЙ РОСТ РАСХОДОВ НА СОДЕРЖАНИЕ БЮДЖЕТНЫХ УЧРЕЖДЕНИЙ И РОСТ БЕЗВОЗМЕЗДНЫХ ПЕРЕЧИСЛЕНИЙ БЮДЖЕТНЫМ УЧРЕЖДЕНИЯМ</b><br/><b>Рост</b>&nbsp;расходов консолидированного бюджета субъекта РФ&nbsp;<b>на содержание бюджетных учреждений (КОСГУ 211, 213, 224, 225)</b>&nbsp;и&nbsp;<b>на безвозмездные перечисления бюджетным учреждениям (КОСГУ 241)</b>&nbsp;по состоянию на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г.<br/><br/>Темп исполнения по расходам на содержание бюджетных учреждений на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г. к аналогичному периоду&nbsp;<b>{1:yyyy}</b>&nbsp;года:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{2:P0}</b><br/>Темп исполнения по расходам на безвозмездные перечисления бюджетным учреждениям на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г. к аналогичному периоду&nbsp;<b>{1:yyyy}</b>&nbsp;года:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/><br/><b>Возможные причины:</b><ul style='margin-bottom: -35px; margin-top: 0px'><li>увеличение количества бюджетных учреждений?</li><li>неисполнение 83-ФЗ?</li><li>изменения в бюджетном законодательстве?</li></ul>",
                                    date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["Темп к прошлому году "], dt.Rows[0]["Темп к прошлому году по безвозмездным"]);
                        }
                    case "2":
                        {
                            return String.Format("<b>ОПЕРЕЖАЮЩИЙ РОСТ БЕЗВОЗМЕЗДНЫХ ПЕРЕЧИСЛЕНИЙ БЮДЖЕТНЫМ УЧРЕЖДЕНИЯМ</b><br/><b>Опережающий рост</b>&nbsp;расходов консолидированного бюджета субъекта РФ&nbsp;<b>на безвозмездные перечисления бюджетным учреждениям (КОСГУ 241)</b>&nbsp;по состоянию на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г.<br/><br/>Темп исполнения по расходам на содержание бюджетных учреждений на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г. к аналогичному периоду&nbsp;<b>{1:yyyy}</b>&nbsp;года:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{2:P0}</b><br/>Темп исполнения по расходам на безвозмездные перечисления бюджетным учреждениям на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г. к аналогичному периоду&nbsp;<b>{1:yyyy}</b>&nbsp;года:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/><br/><b>Возможные причины:</b><ul style='margin-bottom: -35px; margin-top: 0px'><li>субсидии бюджетным учреждениям предоставлены, а расходы по сметам не сокращаются?</li><li>неисполнение 83-ФЗ?</li><li>изменения в бюджетном законодательстве?</li></ul>",
                                    date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["Темп к прошлому году "], dt.Rows[0]["Темп к прошлому году по безвозмездным"]);
                        }
                    case "3":
                        {
                            return String.Format("<b>ОДНОВРЕМЕННОЕ СНИЖЕНИЕ РАСХОДОВ НА СОДЕРЖАНИЕ БЮДЖЕТНЫХ УЧРЕЖДЕНИЙ И РОСТ БЕЗВОЗМЕЗДНЫХ ПЕРЕЧИСЛЕНИЙ БЮДЖЕТНЫМ УЧРЕЖДЕНИЯМ</b><br/><b>Снижение</b>&nbsp;расходов консолидированного бюджета субъекта РФ&nbsp;<b>на содержание бюджетных учреждений (КОСГУ 211, 213, 224, 225)</b>&nbsp;и&nbsp;<b>на безвозмездные перечисления бюджетным учреждениям (КОСГУ 241)</b>&nbsp;по состоянию на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г.<br/><br/>Темп исполнения по расходам на содержание бюджетных учреждений на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г. к аналогичному периоду&nbsp;<b>{1:yyyy}</b>&nbsp;года:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{2:P0}</b><br/>Темп исполнения по расходам на безвозмездные перечисления бюджетным учреждениям на&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;г. к аналогичному периоду&nbsp;<b>{1:yyyy}</b>&nbsp;года:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/><br/><b>Возможные причины:</b><ul style='margin-bottom: -35px; margin-top: 0px'><li>неисполнение 83-ФЗ?</li><li>изменения в бюджетном законодательстве?</li></ul>",
                                   date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["Темп к прошлому году "], dt.Rows[0]["Темп к прошлому году по безвозмездным"]);
                        }
                }
            }
            else
            {
                string query = DataProvider.GetQueryText("fo_0003_0001_indicator");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

                return GetIncomesHintText(dt.Rows[0]["Внимание"].ToString(), dt, controlID);
            }
            return String.Empty;
        }
    }
}
